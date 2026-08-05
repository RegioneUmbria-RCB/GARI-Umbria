

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Audit_Impostazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiImpostazione(
        ByVal Impostazione_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef MessaggioErrore As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione.LeggiImpostazione()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * From Audit_Impostazioni ")
            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Impostazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod))
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Audit_Impostazioni.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Audit_Impostazioni.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Audit_Impostazioni.PivaSuperuser, Audit_Impostazioni.Impostazione_Cod ASC ")
            End If

            Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function
End Class

Public Class Audit_Impostazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviImpostazione(
        ByVal Impostazione_Cod As Integer,
        ByVal value As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione.LeggiImpostazione()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine($"IF EXISTS (SELECT 1 FROM Audit_Impostazioni WHERE Impostazione_Cod = {Agro_SQL_SaveNum(Impostazione_Cod)})")
            StrSQL.AppendLine("BEGIN")

            StrSQL.AppendLine("UPDATE Audit_Impostazioni SET")
            StrSQL.AppendLine($"  Valore1 = '{Agro_SQL_SaveText(Trim(value))}',")
            StrSQL.AppendLine($"  Data_Modifica = GETDATE(),")
            StrSQL.AppendLine($"  Username_Modifica = '{Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione))}'")
            StrSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser))}'")
            If Impostazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod))
            End If

            StrSQL.AppendLine("END")
            StrSQL.AppendLine("ELSE")
            StrSQL.AppendLine("BEGIN")

            StrSQL.AppendLine("INSERT INTO Audit_Impostazioni")
            StrSQL.AppendLine("(PivaSuperUser, Impostazione_Cod, Valore1, inviato, Username_Creazione, Username_Modifica, Data_Creazione, Data_Modifica, Validita_Inizio, Validita_Fine)")
            StrSQL.AppendLine("VALUES")
            StrSQL.AppendLine($"(")
            StrSQL.AppendLine($"    '{Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser))}', ")
            StrSQL.AppendLine($"    {Agro_SQL_SaveNum(Impostazione_Cod)},")
            StrSQL.AppendLine($"    '{Agro_SQL_SaveText(Trim(value))}',")
            StrSQL.AppendLine($"    0,")
            StrSQL.AppendLine($"    '{Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione))}',")
            StrSQL.AppendLine($"    '{Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione))}',")
            StrSQL.AppendLine($"    {Agro_SQL_SaveDate(DateTime.Now)},")
            StrSQL.AppendLine($"    {Agro_SQL_SaveDate(DateTime.Now)},")
            StrSQL.AppendLine($"    {Agro_SQL_SaveDate(AGRODATAINIZIO)},")
            StrSQL.AppendLine($"    {Agro_SQL_SaveDate(AGRODATAFINE)}")
            StrSQL.AppendLine($")")

            StrSQL.AppendLine("END")

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function
End Class

