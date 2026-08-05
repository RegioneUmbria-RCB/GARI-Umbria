Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class MacchinexCaratteristiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Mac_Car_Cod As Integer, _
                          ByVal Class_Code As String, _
                          ByVal xFiltroAggiuntivo As String, _
                           ByVal xOrderBy As String, _
                          ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MacchinexCaratteristiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT MacchinexCaratteristiche.*, Macchine_Caratteristiche.*  " + vbCrLf)
            StrSQL.Append(" FROM MacchinexCaratteristiche INNER JOIN Macchine_Caratteristiche ON MacchinexCaratteristiche.Mac_Car_Cod = Macchine_Caratteristiche.Mac_Car_Cod ")
            StrSQL.Append(" WHERE 1=1")

            If Mac_Car_Cod <> 0 Then
                StrSQL.Append(" AND MacchinexCaratteristiche.Mac_Car_Cod= " & Agro_SQL_SaveNum(Mac_Car_Cod) & "  ")
            End If

            If Class_Code <> "" Then
                StrSQL.Append(" AND MacchinexCaratteristiche.Class_Code='" + Agro_SQL_SaveText(Class_Code) + "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   MacchinexCaratteristiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   MacchinexCaratteristiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
