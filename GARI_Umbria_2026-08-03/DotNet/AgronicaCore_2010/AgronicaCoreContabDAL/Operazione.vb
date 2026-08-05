Imports System.Text
Imports AgronicaCoreDataProvider

Public Class Operazione_Causale_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_CausaleDes_From_CausaleId(ByVal Id As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Operazione_Causale_R.Leggi_CausaleDes_From_CausaleId()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT oc.Id, oc.Causale ")
            strSql.AppendLine(" FROM Operazione_Causale oc ")
            strSql.AppendLine(" WHERE oc.Id = " & Agro_SQL_SaveNum(Id) & " ")
            strSql.AppendLine(" AND oc.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND oc.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" ORDER BY oc.Causale ASC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
