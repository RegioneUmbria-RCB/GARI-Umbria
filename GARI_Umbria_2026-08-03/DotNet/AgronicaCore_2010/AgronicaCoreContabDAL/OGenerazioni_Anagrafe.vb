Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class OGenerazioni_Anagrafe_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Tipo_Generazione As Integer,
                          ByVal Codice_Generazione As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Distinct OGenerazioni_Anagrafe_Moduli.*, OGenerazioni_Anagrafe.*  ")
            strSql.AppendLine(" FROM   OGenerazioni_Anagrafe_Moduli")
            strSql.AppendLine(" LEFT JOIN  OGenerazioni_Anagrafe")
            strSql.AppendLine(" ON  OGenerazioni_Anagrafe.Modulo_Generazione = OGenerazioni_Anagrafe_Moduli.Modulo_Generazione")
            strSql.AppendLine(" WHERE  OGenerazioni_Anagrafe.Validita_Inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            strSql.AppendLine(" AND    OGenerazioni_Anagrafe.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")

            If Tipo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe.Tipo_Generazione = " & Agro_SQL_SaveNum(Tipo_Generazione) & " ")
            End If

            If Codice_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe.Codice_Generazione = " & Agro_SQL_SaveNum(Codice_Generazione) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" ORDER BY OGenerazioni_Anagrafe.Modulo_Generazione, OGenerazioni_Anagrafe.Tipo_Generazione, OGenerazioni_Anagrafe.Codice_Generazione, OGenerazioni_Anagrafe.Descrizione ASC")

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

    Public Function Leggi_ElencoStatiProdotto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.OGenerazioni_Anagrafe_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM OGenerazioni_Anagrafe")
            strSql.AppendLine("WHERE Tipo_Generazione = 4")
            strSql.AppendLine("AND Modulo_Generazione = 2")
            strSql.AppendLine("AND Elem_Cod = 210")
            strSql.AppendLine("AND Validita_Inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            strSql.AppendLine("AND Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")

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