Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class GIS_LayerElementiGrafici_Anagrafica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Int32,
        ByVal Flag_Inserimento As String,
        ByVal Flag_Modifica As String,
        ByVal Flag_Cancellazione As String,
        ByVal Flag_Informazioni As String,
        ByVal Flag_SingolaDataSignificativa As String,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT  * ")
                    StrSQL.AppendLine("FROM GIS_LayerElementiGrafici_Anagrafica LayerAnag")
                    StrSQL.AppendLine("WHERE LayerAnag.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("AND LayerAnag.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    StrSQL.AppendLine("AND LayerAnag.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

                    If LayerElementiGrafici_Cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerAnag.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                    End If

                    If TipologiaLayer_cod <> 0 Then
                        StrSQL.AppendLine(" AND LayerAnag.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
                    End If

                    If Flag_Inserimento <> "" AndAlso IsNumeric(Flag_Inserimento) Then
                        StrSQL.AppendLine(" AND LayerAnag.Flag_Inserimento = " & Agro_SQL_SaveNum(Flag_Inserimento) & " ")
                    End If

                    If Flag_Modifica <> "" AndAlso IsNumeric(Flag_Modifica) Then
                        StrSQL.AppendLine(" AND LayerAnag.Flag_Modifica = " & Agro_SQL_SaveNum(Flag_Modifica) & " ")
                    End If

                    If Flag_Cancellazione <> "" AndAlso IsNumeric(Flag_Cancellazione) Then
                        StrSQL.AppendLine(" AND LayerAnag.Flag_Cancellazione = " & Agro_SQL_SaveNum(Flag_Cancellazione) & " ")
                    End If

                    If Flag_Informazioni <> "" AndAlso IsNumeric(Flag_Informazioni) Then
                        StrSQL.AppendLine(" AND LayerAnag.Flag_Informazioni = " & Agro_SQL_SaveNum(Flag_Informazioni) & " ")
                    End If

                    If Flag_SingolaDataSignificativa <> "" AndAlso IsNumeric(Flag_SingolaDataSignificativa) Then
                        StrSQL.AppendLine(" AND LayerAnag.Flag_SingolaDataSignificativa = " & Agro_SQL_SaveNum(Flag_SingolaDataSignificativa) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   LayerAnag.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   LayerAnag.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

            End Select

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class

Public Class GIS_LayerElementiGrafici_Anagrafica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal TipologiaLayer_cod As Int32,
        ByVal LayerElementiGrafici_Des As String,
        ByVal Flag_Inserimento As Integer,
        ByVal Flag_Modifica As Integer,
        ByVal Flag_Cancellazione As Integer,
        ByVal Flag_Informazioni As Integer,
        ByVal Flag_SingolaDataSignificativa As Integer,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            'Controlli
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella insert (PivaSuperUser obbligatorio)")
            End If
            If (LayerElementiGrafici_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (LayerElementiGrafici_Cod obbligatorio)")
            End If
            If (TipologiaLayer_cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (TipologiaLayer_cod obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO GIS_LayerElementiGrafici_Anagrafica ")
            'Colonne
            StrSQL.AppendLine("(PivaSuperUser")
            StrSQL.AppendLine(",LayerElementiGrafici_Cod")
            StrSQL.AppendLine(",TipologiaLayer_cod")
            StrSQL.AppendLine(",LayerElementiGrafici_Des")
            StrSQL.AppendLine(",Flag_Inserimento")
            StrSQL.AppendLine(",Flag_Modifica")
            StrSQL.AppendLine(",Flag_Cancellazione")
            StrSQL.AppendLine(",Flag_Informazioni")
            StrSQL.AppendLine(",Flag_SingolaDataSignificativa")
            StrSQL.AppendLine(",Inviato")
            StrSQL.AppendLine(",DataInvio")
            StrSQL.AppendLine(",Data_Creazione")
            StrSQL.AppendLine(",Data_Modifica")
            StrSQL.AppendLine(",UserName_Creazione")
            StrSQL.AppendLine(",UserName_Modifica")
            StrSQL.AppendLine(",Validita_Inizio")
            StrSQL.AppendLine(",Validita_Fine")
            StrSQL.AppendLine(") ")
            'Valori
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine(String.Format(" '{0}'", Agro_SQL_SaveText(PivaSuperUser)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(TipologiaLayer_cod)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(LayerElementiGrafici_Des)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(Flag_Inserimento)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(Flag_Modifica)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(Flag_Cancellazione)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(Flag_Informazioni)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(Flag_SingolaDataSignificativa)))
            StrSQL.AppendLine(", 0 ")
            StrSQL.AppendLine(", NULL ")
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Date.Now)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Date.Now)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(",'{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveDate(Validita_Fine)))
            StrSQL.AppendLine(")")
            'Esecuzione
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Delete(PivaSuperUser As String,
                           LayerElementiGrafici_Cod As Int32,
                           tipologiaLayerCod As Int32,
                           filtroAggiuntivo As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_W.Elimina()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSQL.Length = 0

            'Controlli
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto: PivaSuperUser")
            End If

            StrSQL.AppendLine("DELETE FROM GIS_LayerElementiGrafici_Anagrafica")
            StrSQL.AppendLine($"WHERE LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(LayerElementiGrafici_Cod)}")
            StrSQL.AppendLine($"    AND PivaSuperUser = '{Agro_SQL_SaveText(PivaSuperUser)}'")

            If tipologiaLayerCod <> 0 Then
                StrSQL.AppendLine($"    AND TipologiaLayer_cod = {Agro_SQL_SaveNum(tipologiaLayerCod)}")
            End If

            If filtroAggiuntivo <> "" Then
                StrSQL.AppendLine($"    AND {Agro_SQL_Save_xFiltroAggiuntivo(filtroAggiuntivo)}")
            End If

            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return resp
    End Function

    Public Function SalvaGUIDLayer(ByVal layerElementiGrafici_Cod As Int32,
                                   ByVal tipologiaLayer_cod As Int32,
                                   ByVal newGUIDLayer As String,
                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_W.SalvaGUIDLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerElementiGrafici_Anagrafica SET ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_GUID = '{0}' ", Agro_SQL_SaveText(newGUIDLayer)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not xRisp Then Return xRisp

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_DataStruct SET ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_GUID = '{0}' ", Agro_SQL_SaveText(newGUIDLayer)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
End Class
