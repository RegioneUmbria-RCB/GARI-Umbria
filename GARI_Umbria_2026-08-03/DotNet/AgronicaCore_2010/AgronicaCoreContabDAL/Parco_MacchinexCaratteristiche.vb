Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Parco_MacchinexCaratteristiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_MacchineCaratteristicheXChiaveAPI(ByVal Codice As String,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Parco_MacchinexCaratteristiche_R.Leggi_MacchineCaratteristicheXCodice()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" select ")
            strSql.AppendLine(" 	a.Mac_Cod, a.Mac_Car_Cod, b.Mac_Car_Des,  ")
            strSql.AppendLine(" 	a.Valore, b.Mac_Car_Tipo, b.Mac_Car_Udm_Cod, ")
            strSql.AppendLine(" 	c.UDM_DES, a.Validita_Inizio, a.Validita_Fine  ")
            strSql.AppendLine(" from ")
            strSql.AppendLine("     Parco_Macchine pm inner join  ")
            strSql.AppendLine(" 	Parco_MacchinexCaratteristiche a on (pm.Mac_Cod=a.Mac_Cod) inner join ")
            strSql.AppendLine(" 	(Macchine_Caratteristiche b left join ")
            strSql.AppendLine(" 		UnitaMisura c on (b.Mac_Car_Udm_Cod=c.UDM_COD)) ")
            strSql.AppendLine(" 	on (a.Mac_Car_Cod=b.Mac_Car_Cod) ")
            strSql.AppendLine(" where ")
            strSql.AppendLine(" 	ExternalAPIKey='" + Agro_SQL_SaveText(Codice) + "' ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

Public Class Parco_MacchinexCaratteristiche_W

End Class
