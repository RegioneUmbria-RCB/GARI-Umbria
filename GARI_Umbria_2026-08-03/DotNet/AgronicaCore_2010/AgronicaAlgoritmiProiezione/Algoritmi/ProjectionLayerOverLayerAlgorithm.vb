Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.exceptions
Imports Newtonsoft.Json

Public Class ProjectionLayerOverLayerAlgorithm
    Implements IAlgoritmoProiezione

    Private ReadOnly LayerAnalysisConfig_Algorithm_Cod As Int32
    Private ReadOnly LayerAnalysisConfig_AlgorithmType_Cod As Int32

    Private Layer_1 As ProiezioneLayer
    Private Layer_2 As ProiezioneLayer
    Private Result_Layer As ProiezioneLayer

    Private configuration As ConfigurazioneProiezione

    Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32)
        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod
    End Sub

    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Me.configuration = xRead.LeggiLayerDaConfigurazione(LayerAnalysisConfig_Cod, objParametri)

        Me.Layer_1 = configuration.Layer1
        Me.Layer_2 = configuration.Layer2
        Me.Result_Layer = configuration.LayerRisultato
    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione
        Throw New NotImplementedException()
    End Sub

    Public Function Esegui(LayerAnalysisConfig_Cod As Integer,
                           Entita_cod_1 As Integer,
                           Entita_cod_2 As Integer,
                           Entita_cod_Risultato As Integer,
                           Esecuzione_cod As Integer,
                           Esecuzione_GUID As String,
                           ParametriEsecuzione As String,
                           ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                           ByRef objParametri_Super_Server As AgronicaCoreParametri,
                           Optional override_transazione As Boolean = False
                           ) As Boolean Implements IAlgoritmoProiezione.Esegui

        Dim resp As Boolean

        Try
            LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod, objParametri_Server)

            resp = Me.ExecuteOverLayer(
                LayerAnalysisConfig_Cod,
                Me.Layer_1.LayerElementiGrafici_Cod,
                Me.Layer_2.LayerElementiGrafici_Cod,
                Me.Result_Layer.LayerElementiGrafici_Cod,
                Esecuzione_cod,
                Esecuzione_GUID,
                ParametriEsecuzione,
                objParametri_Server,
                objParametri_Utenti,
                objParametri_Super_Server,
                override_transazione
                )

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        End Try

        Return resp
    End Function

    Public Function ExecuteOverLayer(LayerAnalysisConfig_Cod As Integer,
                                     Layer1Cod As Integer,
                                     Layer2Cod As Integer,
                                     LayerOutCod As Integer,
                                     Esecuzione_cod As Integer,
                                     Esecuzione_GUID As String,
                                     ParametriEsecuzione As String,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                     Optional override_transazione As Boolean = False
                                     ) As Boolean

        Dim entityDes As String
        Dim elementiGraficiR As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R
        Dim elementiGraficiW As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W
        Dim resp As Boolean
        Dim messaggio As String

        Dim DT As DataTable

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                    FlagConnessioneLocale,
                    FlagTransazioneLocale,
                    objParametri_Server
                    )
            End If

            DT = elementiGraficiR.CalculateIntersectionsOnLayers(
                Layer1Cod,
                Layer2Cod,
                Me.Result_Layer.Params(1).LayerElementiGrafici_Etichetta, ' number of polygons label
                Me.Result_Layer.Params(0).LayerElementiGrafici_Etichetta, ' intersection area label
                objParametri_Server,
                validityYearLayer1:=extractYearFromCfgObject() ' get year if present in cfg json
                )

            Dim dv = DT.AsDataView
            dv.RowFilter = Me.AnyChangesWhereClause() ' check if at least a difference from previous situation was found
            Dim filteredDT As DataTable = dv.ToTable

            If filteredDT.Rows.Count > 0 Then
                For Each row In filteredDT.Rows
                    Dim dtLayer As DataTable

                    If Me.Result_Layer.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
                        dtLayer = elementiGraficiR.LeggiDatiMinimiDaCodiceEntita(row("Entita_Cod"), objParametri_Server)
                    Else
                        dtLayer = elementiGraficiR.LeggiDescrizioneElementoDaCodiceEntita(row("Entita_Cod"), objParametri_Server)
                    End If

                    If dtLayer Is Nothing OrElse dtLayer.Rows.Count <> 1 Then
                        Throw New Exception("Error in finding the graphic element.")
                    End If

                    entityDes = ReadEntityDescription(dtLayer.Rows(0)("ElementoGrafico_Des").ToString,
                                                      Layer_1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI)

                    ' this could be an error, since the algorithm works on layer1 and layer2, so it could not make any sense to store results on a third layer
                    If Me.Layer_2.LayerElementiGrafici_Cod <> Me.Result_Layer.LayerElementiGrafici_Cod Then
                        Throw New GiasException("Layer2 and ResultLayer are not the same.")
                    End If

                    Dim newDes = ElaborateNewEntityDescription(entityDes, row)

                    elementiGraficiW.UpdateGraphicElementDescription(row("Entita_Cod"), row("ElementoGrafico_Cod"), newDes, objParametri_Server)
                Next

                messaggio = String.Format("Sono state rilevate {0} sovrapposizioni del layer {1} sul layer {2}", filteredDT.Rows.Count, Layer1Cod, Layer2Cod)
                resp = xWrite.InserisciLogEsecuzioneAlgoritmo(
                    LayerAnalysisConfig_Cod,
                    Esecuzione_cod,
                    Layer1Cod,
                    Layer2Cod,
                    LayerOutCod,
                    1,
                    messaggio, objParametri_Server)
            Else
                ' no intersection was found between the layers
                resp = xWrite.InserisciLogEsecuzioneAlgoritmo(
                    LayerAnalysisConfig_Cod,
                    Esecuzione_cod,
                    0,
                    0,
                    0,
                    1,
                    String.Format("No intersection was found between the layers {0} and {1}.", Layer1Cod, Layer2Cod),
                    objParametri_Server
                    )
            End If

            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try

        Return resp
    End Function

    Private Function ReadEntityDescription(ByVal descriptionFromDB As String, ByVal isImpianto As Boolean) As String
        If isImpianto Then
            Return descriptionFromDB
        End If

        Dim partiDescrizione = descriptionFromDB.ToString.Split("|")

        Dim elementoDescrizione = partiDescrizione.Where(Function(s) s.StartsWith(Layer_1.Params.FirstOrDefault.LayerElementiGrafici_Etichetta))

        If elementoDescrizione Is Nothing OrElse elementoDescrizione.Count <> 1 Then
            Throw New GiasException("Impossibile determinare la descrizione dell'elemento grafico iniziale.")
        End If

        Return elementoDescrizione.FirstOrDefault.Replace(String.Format("{0}§ ", Layer_1.Params.FirstOrDefault.LayerElementiGrafici_Etichetta), "").Trim
    End Function

    Private Function ElaborateNewEntityDescription(entityDes As String,
                                                   row As DataRow) As String

        Dim descriptionParts = entityDes.Split("|").ToList.
            Select(Function(x)
                       Return New KeyValuePair(Of String, String)(x.Split("§")(0), x.Split("§")(1))
                   End Function).ToList

        Dim val1 As String = CStr(row(Me.Result_Layer.Params(0).LayerElementiGrafici_Etichetta)).Replace(",", ".")
        Dim val2 As String = row(Me.Result_Layer.Params(1).LayerElementiGrafici_Etichetta)

        Dim newDp1 = New KeyValuePair(Of String, String)(Me.Result_Layer.Params(0).LayerElementiGrafici_Etichetta, val1)
        Dim newDp2 = New KeyValuePair(Of String, String)(Me.Result_Layer.Params(1).LayerElementiGrafici_Etichetta, val2)

        Dim idx1 = descriptionParts.FindIndex(Function(dp) dp.Key = newDp1.Key)
        Dim idx2 = descriptionParts.FindIndex(Function(dp) dp.Key = newDp2.Key)

        If idx1 = -1 Then
            descriptionParts.Add(newDp1) ' if the field is not found in entity description it is added
        Else
            descriptionParts.Item(descriptionParts.FindIndex(Function(dp) dp.Key = newDp1.Key)) = newDp1
        End If

        If idx2 = -1 Then
            descriptionParts.Add(newDp2) ' if the field is not found in entity description it is added
        Else
            descriptionParts.Item(descriptionParts.FindIndex(Function(dp) dp.Key = newDp2.Key)) = newDp2
        End If

        Dim newDes = String.Join("|", descriptionParts.Select(Of String)(Function(dp) String.Format("{0}§ {1}", dp.Key, dp.Value))).Replace("'", "''")

        Return newDes
    End Function

    ''' <summary>
    ''' Use this method to get a WHERE clause that checks if there is any intersection between the layers in the resulting DT
    ''' </summary>
    ''' <returns></returns>
    Private Function AnyIntersectionWhereClause()
        Return String.Format("{0} > 0", Me.Result_Layer.Params(0).LayerElementiGrafici_Etichetta)
    End Function

    ''' <summary>
    ''' Use this method to get a WHERE clause that checks if there is any change from previous situation
    ''' </summary>
    ''' <returns></returns>
    Private Function AnyChangesWhereClause()
        Return String.Format(
            "(NOT (('|' + ElementoGrafico_Des) LIKE ('%|{0}§ ' + [{0}] + '%'))) OR (NOT (('|' + ElementoGrafico_Des) LIKE ('%|{1}§ ' + [{1}] + '%')))",
            Me.Result_Layer.Params(0).LayerElementiGrafici_Etichetta, ' intersection area label
            Me.Result_Layer.Params(1).LayerElementiGrafici_Etichetta  ' number of polygons label
            )
    End Function

    Private Function extractCfgObject(ByVal cfgJson As String) As Object
        Return JsonConvert.DeserializeObject(Of Object)(cfgJson)
    End Function

    Private Function extractYearFromCfgObject() As Integer
        Dim year As Integer
        Try
            Dim cfgObj = Me.extractCfgObject(Me.configuration.cfg.ToLower)
            If cfgObj IsNot Nothing Then
                year = cfgObj("anno").value
            End If
        Catch ex As Exception
            year = Nothing
        End Try
        Return year
    End Function
End Class
