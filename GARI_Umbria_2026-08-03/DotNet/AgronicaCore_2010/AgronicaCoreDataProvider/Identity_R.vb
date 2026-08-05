Imports System.Data.Common

Public Class Identity_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_Identity(
                            ByRef objConnessione As DbConnection,
                            ByRef objTransazione As DbTransaction,
                            ByVal StringaConnessione As String,
                            ByVal DirectoryLOG As String,
                            ByVal FileLOG As String,
                            ByVal IdentificatoreUtente As String,
                            ByVal objParametriServer As AgronicaCoreParametri
                            ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Identity_R.Leggi_Identity()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Risp As Int32 = -1

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT SCOPE_IDENTITY() ")

            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

                Risp = CInt(DT.Rows(0).Item(0))

            Else

                Risp = -1

            End If

        Catch ex As Exception

            Risp = -1
            MessaggioErrore = ex.Message

            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = IdentificatoreUtente,
                .LogDirectory = DirectoryLOG,
                .LogFileName = FileLOG
            }
            Scrivi_LOG(objParametriServer,
                       NomeRoutine,
                       MessaggioErrore,
                       CustomLOGParams:=customLOGParams)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        Finally

            DT = Nothing
            StrSQL = Nothing

        End Try

        Return Risp

    End Function



End Class
