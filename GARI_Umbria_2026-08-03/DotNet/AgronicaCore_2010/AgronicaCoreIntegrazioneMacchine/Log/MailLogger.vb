Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Public Class MailLogger

    Private ReadOnly _configurazione As ConfigurazioneSmtpClient = Nothing
    Private _objParametriServer As AgronicaCoreParametri

    Sub New(ByVal configurazione As ConfigurazioneSmtpClient, ByRef objParametriServer As AgronicaCoreParametri)

        _configurazione = configurazione
        _objParametriServer = objParametriServer

    End Sub

    Public Sub [Error](ByVal subject As String, ByVal body As String)

        If Not Debugger.IsAttached Then
            If Not String.IsNullOrEmpty(_configurazione.Mittente) AndAlso Not String.IsNullOrEmpty(_configurazione.Destinatari) Then
                ThreadPool.QueueUserWorkItem(Sub() ComponiESpedisciMail(subject, body))
            End If
        End If

    End Sub


    Private Sub ComponiESpedisciMail(ByVal subject As String, ByVal body As String)

        Try

            Dim objMail As New Mail
            Dim erroreMail As String = ""

            erroreMail = objMail.invia(_objParametriServer, _configurazione.Mittente, _configurazione.Destinatari, "", "",
                                               subject, body, False, Nothing)


        Catch ex As Exception
        End Try

    End Sub



End Class
