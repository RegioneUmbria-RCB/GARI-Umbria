Imports AgronicaCoreDataProvider
Public Class TraduzionePOEditor_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiConfigurazioneI18N(ByRef StringaConnessioneMatrice As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.LeggiTabelleI18N()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT DISTINCT [Nome_Tabella_originale], [Nome_Tabella_traduzioni], [Campi_da_tradurre], [Clausola_Join]")
            StrSQL.AppendLine("FROM [GIAS_Server_MATRICE].[dbo].[Tabelle_I18N]")
            StrSQL.AppendLine("WHERE [AttivaPOEditor] = 1")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiTraduzioniTerminiDaDbXPoEditorDT(query As String, ByRef StringaConnessioneMatrice As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.LeggiTraduzioniTerminiDaDbXPoEditorDT()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("" & query & "" & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function LeggiTerminiDaTradurre(query As String, ByRef StringaConnessioneMatrice As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.LeggiTerminiDaTradurre()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable


        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("" & query & "" & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function LeggiLingue(ByRef StringaConnessioneMatrice As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.LeggiLingue()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select Lingua_Cod, CodiceISO from lingue Where Nome not like '%Italiano%' " & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function


End Class
Public Class TraduzionePOEditor_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiornaStatoInvioPoEditor(ByRef StringaConnessioneMatrice As String, ByRef updateQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.AggiornaStatoInvioPoEditor()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, updateQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            ' Registra l'eccezione
            ' Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function

    Public Sub InsertTabelleI18NPO(ByRef StringaConnessioneMatrice As String, ByRef StringQuery As String)

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.InsertTabelleI18NPO()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(StringQuery & vbCrLf)
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


    Public Function UpdateTermineNONUnivoco(ByRef StringaConnessioneMatrice As String, ByRef StringQuery As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.UpdateTermineNONUnivoco()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(StringQuery & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function InsertTraduzioneXLingue(ByRef query As String, ByRef StringaConnessioneMatrice As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreTraduzioneDAL.TraduzionePOEditor.InsertTraduzioneXLingue()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(StringaConnessioneMatrice, query, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            ' Registra l'eccezione
            ' Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function




End Class
