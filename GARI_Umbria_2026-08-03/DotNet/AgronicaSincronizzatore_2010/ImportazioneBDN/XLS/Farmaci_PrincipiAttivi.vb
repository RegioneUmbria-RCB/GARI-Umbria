Public Class Farmaci_PrincipiAttivi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ImportaDati(ByVal Path_FileSoci_XLS As String,
                                ByRef Messaggio As String,
                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim Dt_Schedario As New DataTable
        Dim Dt_Impianti As New DataTable
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"

        Dim provider As String
        If Environment.Is64BitProcess Then
            provider = "PROVIDER=Microsoft.ACE.OLEDB.12.0"
        Else
            provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"
        End If
        Dim StringaConnessione As String = provider
        StringaConnessione &= ";Extended Properties='Excel 8.0;HDR=No;IMEX=1';data source="
        StringaConnessione &= "'" & Path_FileSoci_XLS & "'"

        Dim dtPrincipiAttivi = Leggi_Farmaci_PrincipiAttivi_xls("", objParametri_Server)

        Dim i = 0
        For Each row In dtPrincipiAttivi.Rows
            Dim AIC = CStr(row("AIC")).Trim
            Dim Medicinale = CStr(row("Medicinale")).Trim
            Dim Farm_Cod = Leggi_Farm_Cod(AIC, objParametri_Server)
            Dim Principi_Attivi_Str = row("Principio_Attivo")

            Dim principi_attivi_arr = CStr(Principi_Attivi_Str).Split("|")

            For Each principio_attivo_str In principi_attivi_arr
                Dim pra_arr = principio_attivo_str.Split("-")
                Dim Principio_Attivo = pra_arr(0)
                Dim Posologia = ""
                If pra_arr.Length > 1 Then
                    Posologia = pra_arr(1)
                End If


                Scrivi_Farmaci_Principi_Attivi_Calc(Farm_Cod, AIC, Medicinale, Principio_Attivo, Posologia, objParametri_Server)

            Next

            i += 1
        Next

    End Function

    Private Sub Crea_Dt_Impianti(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet As String = ""
            For Each sheet In dtSheet.Rows
                If Not sheet("TABLE_NAME").ToString().Contains("Legenda") Then
                    firstSheet = sheet("TABLE_NAME").ToString()
                    Exit For
                End If
            Next

            Dim da As New System.Data.OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)

        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try

    End Sub

    Private Function Leggi_Farmaci_PrincipiAttivi_xls(AIC As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Farmaci_PrincipiAttivi_xls ")
            If AIC <> "" Then
                StrSQL.AppendLine(" WHERE  AIC = '" & Agro_SQL_SaveText(AIC) & "' ")
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

    Private Function Scrivi_Farmaci_Principi_Attivi_Calc(Farm_Cod As Integer,
                                                         AIC As String,
                                                         Medicinale As String,
                                                         PrincipioAttivo As String,
                                                         Posologia As String,
                                                         objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As Boolean

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO [dbo].[Farmaci_Principi_Attivi_Calc] ")
            StrSQL.AppendLine("            ([Farm_Cod] ")
            StrSQL.AppendLine("            ,[AIC] ")
            StrSQL.AppendLine("            ,[Medicinale] ")
            StrSQL.AppendLine("            ,[Principio_Attivo] ")
            StrSQL.AppendLine("            ,[Posologia]) ")
            StrSQL.AppendLine("      VALUES ")
            StrSQL.AppendLine("            (" & Agro_SQL_SaveNum(Farm_Cod) & " ")
            StrSQL.AppendLine("            ,'" & Agro_SQL_SaveText(AIC) & "' ")
            StrSQL.AppendLine("            ,'" & Agro_SQL_SaveText(Medicinale) & "' ")
            StrSQL.AppendLine("            ,'" & Agro_SQL_SaveText(PrincipioAttivo) & "' ")
            StrSQL.AppendLine("            ,'" & Agro_SQL_SaveText(Posologia) & "') ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Function Leggi_Farm_Cod(AIC As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Farm_Cod As Integer = 0
        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Farmaci ")
            StrSQL.AppendLine(" WHERE  AIC = '" & Agro_SQL_SaveText(AIC) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Farm_Cod = DT.Rows(0)("Farm_Cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Farm_Cod

    End Function

End Class
