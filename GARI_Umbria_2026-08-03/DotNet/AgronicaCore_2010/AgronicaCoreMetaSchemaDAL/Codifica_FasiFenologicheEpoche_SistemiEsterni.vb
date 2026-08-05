Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Codifica_FasiFenologicheEpoche_SistemiEsterni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID As Integer,
                          ByVal Sistema_Cod As Integer,
                          ByVal Codice_Esterno As String,
                          ByVal ID_BBCH As String,
                          ByVal Gru_Cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Codifica_FasiFenologicheEpoche_SistemiEsterni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Codifica_FasiFenologicheEpoche_SistemiEsterni ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If ID <> 0 Then
                StrSQL.Append(" AND ID  = " & Agro_SQL_SaveNum(ID) & "   " + vbCrLf)
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.Append(" AND Sistema_Cod  = " & Agro_SQL_SaveNum(Sistema_Cod) & "   " + vbCrLf)
            End If

            If ID_BBCH <> "" Then
                StrSQL.Append(" AND ID_BBCH  = '" & Agro_SQL_SaveText(ID_BBCH) & "'   " + vbCrLf)
            End If

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND Gru_Cod  = '" & Agro_SQL_SaveNum(Gru_Cod) & "'   " + vbCrLf)
            End If

            If Codice_Esterno <> "" Then
                StrSQL.Append(" AND Codice_Esterno  = '" & Agro_SQL_SaveText(Codice_Esterno) & "'   " + vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descrizione_Esterno ASC")
            End If

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
