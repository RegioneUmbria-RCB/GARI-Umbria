Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Transactions

Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUmaDal.UMA_Richieste_Restituzioni_R
Public Class UMA_Causali_BIZ
    Inherits AgronicaCoreDataProvider.LogProvider
    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable



        Dim AgronicaDAL As New UMA_Causali_R
        Dim MessaggioErrore As String
        Dim result As DataTable
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "UMA_Causali_BIZ.Leggi()"
        Try
            result = AgronicaDAL.Leggi(objParametri)
        Catch ex As Exception
            result = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result

    End Function

End Class
