Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis

Public Class GIS_LayerTilesDescrizione_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal layerTiles_Cod As Integer,
                          ByVal tipologiaLayer_cod As Integer,
                          ByVal layerElementiGrafici_Cod As Integer,
                          ByVal layerTilesDescrizione_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByVal selezione_Tabella As AgronicaCoreParametri.enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_R.Leggi()"
        Dim MessaggioErrore As String

        Dim DT As DataTable

        Dim StrSQL As New StringBuilder

        Try

            StrSQL.Length = 0

            Select Case selezione_Tabella
                Case AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT " & vbCrLf)
                    StrSQL.AppendLine("       LayerElementiGrafici_Cod " & vbCrLf)
                    StrSQL.AppendLine("       ,LayerTiles_Cod " & vbCrLf)
                    StrSQL.AppendLine("       ,LayerTilesDescrizione_Cod " & vbCrLf)
                    StrSQL.AppendLine("       ,LayerTilesDescrizione_Des " & vbCrLf)
                    StrSQL.AppendLine("       ,Colore_Base " & vbCrLf)
                    StrSQL.AppendLine("       ,TipologiaLayer_cod " & vbCrLf)
                    StrSQL.AppendLine("       ,Valore_a " & vbCrLf)
                    StrSQL.AppendLine("       ,Valore_da " & vbCrLf)
                    StrSQL.AppendLine("       ,Valore_associato " & vbCrLf)
                    StrSQL.AppendLine(" FROM GIS_LayerTilesDescrizione " & vbCrLf)

                    StrSQL.AppendLine(" WHERE " & vbCrLf)
                    StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)) & vbCrLf)
                    StrSQL.AppendLine(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)

                    If layerTiles_Cod <> 0 Then
                        StrSQL.AppendLine(String.Format(" AND LayerTiles_Cod = {0} ", Agro_SQL_SaveNum(layerTiles_Cod)) & vbCrLf)
                    End If

                    If tipologiaLayer_cod <> 0 Then
                        StrSQL.AppendLine(String.Format(" AND TipologiaLayer_cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_cod)) & vbCrLf)
                    End If

                    If layerElementiGrafici_Cod <> 0 Then
                        StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)) & vbCrLf)
                    End If

                    If layerTilesDescrizione_Cod <> 0 Then
                        StrSQL.AppendLine(String.Format(" AND LayerTilesDescrizione_Cod = {0} ", Agro_SQL_SaveNum(layerTilesDescrizione_Cod)) & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(String.Format(" AND {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)) & vbCrLf)
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)) & vbCrLf)
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
Public Class GIS_LayerTilesDescrizione_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(newLTD As TipologiaLabel,
                           data_inizio_validita As Date,
                           data_fine_validita As Date,
                           objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim xRisp As Boolean
        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W.Scrivi()"
        Dim MessaggioErrore As String

        Dim StrSql As New StringBuilder

        Try

            Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            Dim LayerTilesDescrizione_Cod = ObjSequenze.NuovoId_Tabella("GIS_LayerTilesDescrizione",
                                                                        0,
                                                                        2000000000,
                                                                        objParametri)

            StrSql.Length = 0

            StrSql.AppendLine(" INSERT INTO GIS_LayerTilesDescrizione " & vbCrLf)
            StrSql.AppendLine(" (PivaSuperUser " & vbCrLf)
            StrSql.AppendLine("       ,Utente " & vbCrLf)
            StrSql.AppendLine("       ,LayerTilesDescrizione_Cod " & vbCrLf)
            StrSql.AppendLine("       ,TipologiaLayer_cod " & vbCrLf)
            StrSql.AppendLine("       ,LayerElementiGrafici_Cod " & vbCrLf)
            StrSql.AppendLine("       ,LayerTiles_Cod " & vbCrLf)
            StrSql.AppendLine("       ,LayerTilesDescrizione_Des " & vbCrLf)
            StrSql.AppendLine("       ,Colore_Base " & vbCrLf)
            StrSql.AppendLine("       ,Valore_a " & vbCrLf)
            StrSql.AppendLine("       ,Valore_da " & vbCrLf)
            StrSql.AppendLine("       ,Valore_associato " & vbCrLf)
            StrSql.AppendLine("       ,Username_Creazione " & vbCrLf)
            StrSql.AppendLine("       ,Username_Modifica " & vbCrLf)
            StrSql.AppendLine("       ,Validita_Inizio " & vbCrLf)
            StrSql.AppendLine("       ,Validita_Fine " & vbCrLf)
            StrSql.AppendLine(" ) VALUES ( " & vbCrLf)

            StrSql.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione_Cod)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.TipologiaLayer_cod)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.LayerElementiGrafici_Cod)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.LayerTiles_Cod)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(newLTD.label)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(newLTD.colore)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.valore_min)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.valore_max)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newLTD.valore_associato)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(data_inizio_validita)) & vbCrLf)
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(data_fine_validita)) & vbCrLf)

            StrSql.AppendLine(" ) " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function
    Public Function Aggiorna(LayerTilesDescrizione As TipologiaLabel,
                             objParametri As AgronicaCoreParametri
                            ) As Boolean

        Dim xRisp As Boolean
        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W.Aggiorna()"
        Dim MessaggioErrore As String

        Dim StrSql As New StringBuilder

        Try

            StrSql.Length = 0

            StrSql.AppendLine(" UPDATE GIS_LayerTilesDescrizione SET " & vbCrLf)
            StrSql.AppendLine(String.Format(" LayerTilesDescrizione_Des = '{0}' ", Agro_SQL_SaveText(LayerTilesDescrizione.label)) & vbCrLf)
            StrSql.AppendLine(String.Format(" ,Colore_Base = '{0}' ", Agro_SQL_SaveText(LayerTilesDescrizione.colore)) & vbCrLf)
            StrSql.AppendLine(String.Format(" ,Valore_a = {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione.valore_min)) & vbCrLf)
            StrSql.AppendLine(String.Format(" ,Valore_da = {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione.valore_max)) & vbCrLf)
            StrSql.AppendLine(String.Format(" ,Valore_associato = {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione.valore_associato)) & vbCrLf)
            StrSql.AppendLine(String.Format(" ,Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSql.AppendLine(" ,Data_Modifica = GETDATE() " & vbCrLf)

            StrSql.AppendLine(" WHERE " & vbCrLf)
            StrSql.AppendLine(String.Format(" LayerTilesDescrizione_Cod = {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione.LayerTilesDescrizione_Cod)) & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Public Function Elimina(LayerTilesDescrizione As TipologiaLabel,
                             objParametri As AgronicaCoreParametri
                            ) As Boolean

        Dim xRisp As Boolean
        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W.Elimina()"
        Dim MessaggioErrore As String

        Dim StrSql As New StringBuilder

        Try

            StrSql.Length = 0

            StrSql.AppendLine(" DELETE FROM GIS_LayerTilesDescrizione " & vbCrLf)

            StrSql.AppendLine(" WHERE " & vbCrLf)
            StrSql.AppendLine(String.Format(" LayerTilesDescrizione_Cod = {0} ", Agro_SQL_SaveNum(LayerTilesDescrizione.LayerTilesDescrizione_Cod)) & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function


    Public Function DeleteAllTilesDescriptionsPerLayer(pivaSuperUser As String,
                                                       layerCod As Int32,
                                                       objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim routineName As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.DeleteAllTilesDescriptionsPerLayer()"
        Dim strSQL As New Text.StringBuilder

        Try
            If String.IsNullOrWhiteSpace(pivaSuperUser) AndAlso pivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto: pivaSuperUser")
            End If

            If pivaSuperUser = 0 Then
                Throw New Exception("Parametro non corretto: layerCod")
            End If

            strSQL.Length = 0
            strSQL.AppendLine("DELETE FROM GIS_LayerTilesDescrizione")
            strSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(pivaSuperUser)}'")
            strSQL.AppendLine($"    AND LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(layerCod)}")

            Return EseguiQuery_Scrittura(objParametriServer, strSQL.ToString, routineName)
        Catch ex As Exception
            Scrivi_LOG(objParametriServer, routineName, ex.Message)
            Throw New Exception($"[{routineName}] : {ex.Message}")
        End Try
    End Function

End Class
