Imports AgronicaCoreDataProvider.DataProviderExtensions
Public Class GIS_LayerElementiGraficiXTipoOggetto_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                ByVal PivaSuperUser As String,
                ByVal LayerElementiGrafici_Cod As Integer,
                ByVal GIS_TipoOggetto_Cod As Integer,
                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella insert (PivaSuperUser obbligatorio)")
            End If
            If (LayerElementiGrafici_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (LayerElementiGrafici_Cod obbligatorio)")
            End If

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM GIS_LayerElementiGraficiXTipoOggetto ")
                    StrSQL.Append(String.Format(" WHERE PivaSuperUser = '{0}' ", Agro_SQL_SaveText(PivaSuperUser)))
                    StrSQL.Append(String.Format(" AND LayerElementiGrafici_Cod = {0}", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))

                    If (GIS_TipoOggetto_Cod <> 0) Then
                        StrSQL.Append(String.Format(" AND GIS_TipoOggetto_Cod = {0}", Agro_SQL_SaveNum(GIS_TipoOggetto_Cod)))
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class

Public Class GIS_LayerElementiGraficiXTipoOggetto_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal GIS_TipoOggetto_Cod As Int32,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_W.Scrivi()"

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
            If (GIS_TipoOggetto_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (GIS_TipoOggetto_Cod obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO GIS_LayerElementiGraficiXTipoOggetto ")
            'Colonne
            StrSQL.AppendLine("(PivaSuperUser")
            StrSQL.AppendLine(",LayerElementiGrafici_Cod")
            StrSQL.AppendLine(",GIS_TipoOggetto_Cod")
            StrSQL.AppendLine(",inviato")
            StrSQL.AppendLine(",datainvio")
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
            StrSQL.AppendLine(String.Format(", {0} ", Agro_SQL_SaveNum(GIS_TipoOggetto_Cod)))
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

    Public Function Delete(pivaSuperUser As String,
                           layerCod As Int32,
                           tipoOggetto As Int32,
                           filtroAggiuntivo As String,
                           objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_W.Delete()"
        Dim resp As Boolean
        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.Length = 0

            'Controlli
            If pivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto pivaSuperUser")
            End If
            If (layerCod = 0) Then
                Throw New Exception("Parametro non corretto LayerCod")
            End If

            StrSQL.AppendLine("DELETE FROM GIS_LayerElementiGraficiXTipoOggetto")
            StrSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(pivaSuperUser)}'")
            StrSQL.AppendLine($"   AND LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(layerCod)}")

            If tipoOggetto <> 0 Then
                StrSQL.AppendLine($"    AND GIS_TipoOggetto_Cod = {Agro_SQL_SaveNum(tipoOggetto)}")
            End If

            If filtroAggiuntivo <> "" Then
                StrSQL.AppendLine($"   AND {Agro_SQL_Save_xFiltroAggiuntivo(filtroAggiuntivo)}")
            End If

            resp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return resp
    End Function

    Public Function Aggiorna(
        ByVal PivaSuperUser As String,
        ByVal LayerElementiGrafici_Cod As Int32,
        ByVal GIS_TipoOggetto_Cod As Int32,
        ByVal GIS_TipoOggetto_Cod_Prev As Int32,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_W.Aggiorna()"

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
            If (GIS_TipoOggetto_Cod = 0) Then
                Throw New Exception("Parametro non corretto nella insert (GIS_TipoOggetto_Cod obbligatorio)")
            End If
            If (GIS_TipoOggetto_Cod_Prev = 0) Then
                Throw New Exception("Parametro non corretto nella insert (GIS_TipoOggetto_Cod precedente obbligatorio)")
            End If
            'Comando
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE GIS_LayerElementiGraficiXTipoOggetto Set ")
            StrSQL.AppendLine(String.Format(" GIS_TipoOggetto_Cod = {0} ", Agro_SQL_SaveNum(GIS_TipoOggetto_Cod)))
            StrSQL.AppendLine(String.Format(", Data_Modifica = {0} ", Agro_SQL_SaveDate(Date.Now)))
            StrSQL.AppendLine(String.Format(", Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND GIS_TipoOggetto_Cod = {0} ", Agro_SQL_SaveNum(GIS_TipoOggetto_Cod_Prev)))

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

End Class
