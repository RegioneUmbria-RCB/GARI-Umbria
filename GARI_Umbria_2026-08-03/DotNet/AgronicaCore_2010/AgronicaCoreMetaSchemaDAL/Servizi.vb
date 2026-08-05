Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Servizi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Servizio_Cod As Int32, _
                          ByVal Servizio_Des As String, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Servizi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Servizi ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Servizio_Des <> "" Then
                StrSQL.Append(" AND Servizio_Des like '%" & Agro_SQL_SaveText(Servizio_Des) & "%' ")
            End If
         
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Servizio_Cod ")
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

