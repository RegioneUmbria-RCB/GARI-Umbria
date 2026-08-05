Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Audit_log_R


End Class


Public Class Audit_log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function scrivi(
                           ByVal oldkey_audit_cod As Integer _
                            , ByVal oldkey_tipo As Integer _
                            , ByVal oldkey_regolamento As Integer _
                            , ByVal newkey_adit_cod As Integer _
                            , ByVal newkey_tipo As Integer _
                            , ByVal newkey_regolamento As Integer _
                            , ByVal oldkey_superuser As String _
                            , ByVal newkey_superuser As String _
                          , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_log_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Audit_log ( " & vbCrLf)
            StrSQL.Append("   [oldkey_audit_cod] " & vbCrLf)
            StrSQL.Append("  ,[oldkey_tipo] " & vbCrLf)
            StrSQL.Append("  ,[oldkey_regolamento] " & vbCrLf)
            StrSQL.Append("  ,[newkey_adit_cod] " & vbCrLf)
            StrSQL.Append("  ,[newkey_tipo] " & vbCrLf)
            StrSQL.Append("  ,[newkey_regolamento] " & vbCrLf)
            StrSQL.Append("  ,[oldkey_superuser] " & vbCrLf)
            StrSQL.Append("  ,[newkey_superuser] " & vbCrLf)
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append("  " & Agro_SQL_SaveNum(oldkey_audit_cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(oldkey_tipo) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(oldkey_regolamento) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(newkey_adit_cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(newkey_tipo) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(newkey_regolamento) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(oldkey_superuser) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(newkey_superuser) & "'" & vbCrLf)
            StrSQL.Append(") " & vbCrLf)
            '--------------------------------------------


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
