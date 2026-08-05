Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports Agronica.Helpers.GDALHelper
Imports AgronicaCoreGisDAL
Imports Newtonsoft.Json.Linq
Imports System.Threading

Public Class RasterOverlayGDAL_Algorithm
    Inherits LogProvider
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
                           Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui
        Dim resp As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim alg_ext As New AgronicaAlgoritmiProiezione.Algorithm_Extension

        Dim elementiGraficiW As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

        Dim messaggio As String = "Il poligono non interseca l'elemento"
        Dim newIDEntita As Int32 = 0

        Try
            LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod, objParametri_Server)

            Dim guidExec As String = Esecuzione_GUID

            If guidExec.Equals("") Then
                Esecuzione_GUID = CreaNuovoGUIDEsecuzione(Esecuzione_cod, objParametri_Server)
            End If

            'recupera info da poligono
            Dim pars = GetParametriElaborazione(Entita_cod_1, objParametri_Server)

            Dim gdalExecutor As New RasterOverlay
            Dim res As Dictionary(Of Single, Integer) = New Dictionary(Of Single, Integer)
            For Each tiff In pars.tifflinked
                Dim p = gdalExecutor.getFrequencyHistogram(tiff.GeoTiffPath, pars.wkt, 4326)
                For Each kvp In p
                    If res.Keys.Contains(kvp.Key) Then
                        res(kvp.Key) += kvp.Value
                    Else
                        res.Add(kvp.Key, kvp.Value)
                    End If
                Next
            Next

            If res IsNot Nothing AndAlso res.Count > 0 Then
                messaggio = String.Join(", ", res.OrderBy(Of Single)(Function(z) z.Key).Select(Of String)(Function(x) x.Key.ToString() + ": " + x.Value.ToString()).ToList())

                resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(Esecuzione_cod,
                                                                 LayerAnalysisConfig_Cod,
                                                                 Nothing,
                                                                 newIDEntita,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti,
                                                                 messaggio)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dell'elemento grafico dell'intersezione.")
                End If
            End If



            resp = xWrite.InserisciLogEsecuzioneAlgoritmo(LayerAnalysisConfig_Cod,
                                                      Esecuzione_cod,
                                                      Entita_cod_1,
                                                      Entita_cod_2,
                                                      newIDEntita,
                                                      1,
                                                      messaggio,
                                                      objParametri_Server,
                                                      pars.entitaguid)

            If Not resp Then
                Throw New Exception("Errore nella scrittura del log dell'operazione.")
            End If

            Dim esecuzione As DataTable = xRead.LeggiEsecuzioneDaGUID(Esecuzione_GUID, objParametri_Server)
            If esecuzione.Rows(0)("ParametriElaborazione") IsNot DBNull.Value AndAlso Not CStr(esecuzione.Rows(0)("ParametriElaborazione")).Equals("") Then
                'lavez - 24/09/2024 - costruttore responseObj
                Dim respObj As New JObject
                Dim valuelist As New JArray
                For Each kvp In res
                    Dim o As New JObject
                    o.Add(kvp.Key, kvp.Value)
                    valuelist.Add(o)
                Next

                respObj.Add("RisultatoOperazione", valueList)


                'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                Dim algExt As New Algorithm_Extension
                algExt.AlgorithmRaster_Extension(CInt(esecuzione.Rows(0)("LayerAnalysisConfig_Cod")),
                                                     CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                                                     Esecuzione_GUID,
                                                     CStr(esecuzione.Rows(0)("ParametriElaborazione")),
                                                     respObj,
                                                     objParametri_Server)
            End If

            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)
            End If

            gdalExecutor = Nothing

            'Lavez - 03/01/2025 - GarbageCollector Finalizer
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, "RasterOverlayGDAL_Algorithm.Esegui()", ex.Message & "- StackTrace: " + ex.StackTrace.ToString(), False)
            If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        End Try
        Return resp
    End Function

    Private Function CreaNuovoGUIDEsecuzione(ByVal Esecuzione_cod As Int32,
                                             ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim newGuidEsecuzione = Guid.NewGuid().ToString

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim resp = xWrite.SalvaGUIDEsecuzione(Esecuzione_cod, newGuidEsecuzione, objParametri_Server)

        If Not resp Then
            Throw New Exception("Impossibile salvare il nuovo GUID per l'esecuzione.")
        End If

        Return newGuidEsecuzione

    End Function

    Private Function GetObjRisposta(ByVal values As Dictionary(Of Single, Integer)) As JObject
        Dim jAr As New JArray
        For Each kvp In values
            jAr.Add(New JProperty(kvp.Key, kvp.Value))
        Next
        Return New JObject(New JProperty("RisultatoOperazione", jAr))
    End Function

    Private Function GetParametriElaborazione(ByVal Entita_Cod As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As ParametriElaborazione

        Dim ret As ParametriElaborazione = Nothing

        Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R
        Dim xEntitaR As New AgronicaCoreGisBIZ.GIS_Entita_R

        Try
            ret = New ParametriElaborazione


            Dim DTEnt = xRead.LeggiWKTConGUID(Entita_Cod, "", objParametri_Server)
            If DTEnt.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Entita gis {0} non trovata", Entita_Cod))
            End If

            If DTEnt.Rows(0)("wkt") Is DBNull.Value Then
                Throw New Exception(String.Format("wkt non recuperato. Entita gis {0}", Entita_Cod))
            End If

            ret.wkt = DTEnt.Rows(0)("wkt")
            ret.entitaguid = LeggiGUIDEntita(Entita_Cod, objParametri_Server)
            ret.tifflinked = New List(Of TiffLinked)

            Dim DT As DataTable = xRead.LeggiIntersezioneConLayerRasterDaGUID(ret.entitaguid, Layer_2.LayerElementiGrafici_Cod, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count < 1 Then
                Throw New Exception("Impossibile determinare la intersezione con il layer Raster.")
            End If

            For Each row In DT.Rows
                Dim Entita_Cod_Layer = CInt(row("Entita_Cod"))

                Dim DTDet = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_Cod_Layer, objParametri_Server)

                If DTDet Is Nothing OrElse DTDet.Rows.Count <> 1 Then
                    Throw New Exception("Impossibile leggere la descrizione dell'entita intersecata.")
                End If

                ret.tifflinked.Add(New TiffLinked() With {
                                    .descrizione = If(DTDet.Rows(0)("ElementoGrafico_Des") Is DBNull.Value, "", DTDet.Rows(0)("ElementoGrafico_Des").ToString()),
                                    .GeoTiffPath = xEntitaR.LeggiPathAllegatoDaEntita(Entita_Cod_Layer, objParametri_Server)
                                   })
            Next


            'Dim uriList As New JArray

            'If parametriVisualizzazioneArray.Count > 0 Then
            '    uriList = JArray.FromObject(parametriVisualizzazioneArray.Select(Function(p) System.IO.Path.GetFullPath(p("geoTiffPath").ToString)).ToList())
            'End If

            'If uriList Is Nothing OrElse uriList.Count <= 0 Then

            '    Throw New Exception("GeoTiff file non trovato")
            'Else
            '    ret.GeoTiffPath = uriList(0).ToString()
            'End If


        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, "RasterOverlayGDAL_Algorithm.GetParametriElaborazione()", ex.Message & "- Source: " + ex.Source.ToString() & "- StackTrace: " + ex.StackTrace.ToString(), False)
            ret = Nothing
            Throw ex
        End Try

        Return ret
    End Function

    Private Function LeggiGUIDEntita(ByVal entita_cod As Int32,
                                      ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim xRead As New AgronicaCoreGisBIZ.GIS_Entita_R
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim GUID_entita = xRead.LeggiGUIDDaEntita_Cod(entita_cod, objParametri_Server)

        If GUID_entita.Equals("") Then
            GUID_entita = Guid.NewGuid().ToString

            Dim resp = xWrite.SalvaGUIDEntita(entita_cod, GUID_entita, objParametri_Server)

            If Not resp Then
                Throw New Exception("Impossibile salvare il GUID per l'entità.")
            End If
        End If

        Return GUID_entita
    End Function

    Private Class ParametriElaborazione

        Public Property tifflinked As List(Of TiffLinked)
        Public Property wkt As String
        Public Property entitaguid As String
    End Class

    Public Class TiffLinked
        Public Property GeoTiffPath As String
        Public Property descrizione As String
    End Class

End Class

