Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Trasformazioni_Riferimenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_x_Track( _
                ByVal Lotto As String, _
                ByVal xFiltroAggiuntivo As String, _
                ByVal xOrderBy As String, _
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT ... " + vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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






Public Class Trasformazioni_Riferimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '########################################################################
    Public Function Update_ValiditaInizio_ConOra(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Trasformazioni_Riferimenti_W.Update_ValiditaInizio_ConOra()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE trasformazioni_riferimenti " & vbCrLf)
            StrSQL.Append(" SET Validita_Inizio = ( " & vbCrLf)
            StrSQL.Append("                         SELECT ora " & vbCrLf)
            StrSQL.Append("                         FROM Movimenti	" & vbCrLf)
            StrSQL.Append("                         WHERE Movimenti.cau_mov = '" + Agro_SQL_SaveText(CAU_LINEA_PRODUZIONE) + "' " + vbCrLf)
            StrSQL.Append("                         AND Id_Agenda = trasformazioni_riferimenti.id_agenda " & vbCrLf)
            StrSQL.Append("                         ) " & vbCrLf)
            StrSQL.Append(" " & vbCrLf)

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

