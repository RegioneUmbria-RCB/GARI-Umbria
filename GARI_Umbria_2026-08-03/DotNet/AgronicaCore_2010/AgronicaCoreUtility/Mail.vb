Imports System.Net
Imports System.Net.Mail
Imports System.ComponentModel
Imports AgronicaCoreDataProvider

Public Class Mail

    Public Function invia(objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         Mittente As String, MailA As String(), MailCC As String(), MailCCN As String(),
                         Oggetto As String, TestoMail As String, isBodyHTML As Boolean,
                         Allegati As String()) As String

        Dim stringaA As String = Nothing
        If Not IsNothing(MailA) Then
            stringaA = String.Join(",", MailA)
        End If

        Dim stringaCC As String = Nothing
        If Not IsNothing(MailCC) Then
            stringaCC = String.Join(",", MailCC)
        End If

        Dim stringaCCN As String = Nothing
        If Not IsNothing(MailCCN) Then
            stringaCCN = String.Join(",", MailCCN)
        End If

        Return invia(objParametri_Server,
                          Mittente, stringaA, stringaCC, stringaCCN,
                          Oggetto, TestoMail, isBodyHTML,
                          Allegati)

    End Function

    Public Function invia(objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Mittente As String, MailA As String, MailCC As String, MailCCN As String,
                          Oggetto As String, TestoMail As String, isBodyHTML As Boolean, Allegati As String(),
                          Optional ByVal ClientSMTP As String = "",
                          Optional ByVal ClientSMTP_Porta As String = "",
                          Optional ByVal user As String = "",
                          Optional ByVal password As String = "",
                          Optional ByVal enablessl As String = ""
                          ) As String

        Dim errore As String = ""

        If System.Net.ServicePointManager.SecurityProtocol <> System.Net.SecurityProtocolType.Tls12 Then
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
        End If

        Dim mail As New MailMessage()

        mail.From = New MailAddress(Mittente)

        If Not IsNothing(MailA) AndAlso MailA.Trim() <> "" Then
            mail.To.Add(MailA)
        End If

        If Not IsNothing(MailCC) AndAlso MailCC.Trim() <> "" Then
            mail.CC.Add(MailCC)
        End If

        If Not IsNothing(MailCCN) AndAlso MailCCN.Trim() <> "" Then
            mail.Bcc.Add(MailCCN)
        End If

        mail.Subject = Oggetto
        mail.IsBodyHtml = isBodyHTML
        mail.Body = TestoMail

        If Not IsNothing(Allegati) Then
            For Each all As String In Allegati

                If IO.File.Exists(all) Then
                    Dim infoFileDaAllegare As New IO.FileInfo(all)
                    Dim contentType = New Mime.ContentType
                    contentType.MediaType = Web.MimeMapping.GetMimeMapping(infoFileDaAllegare.Name)
                    mail.Attachments.Add(New Attachment(all, contentType))
                Else
                    mail.Attachments.Add(New Attachment(all))
                End If

            Next
        End If

        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Try

            If ClientSMTP = "" Then
                Dim dtConf As DataTable = objConf.Leggi(0, "ClientSMTP", "", "", objParametri_Server)
                ClientSMTP = dtConf.Rows(0).Item("valore")

                If ClientSMTP = "" Then
                    Throw New Exception("Client SMTP non configurato")
                End If
            End If

            If user = "" Then
                Dim dtConf As DataTable = objConf.Leggi(0, "user_smtp", "", "", objParametri_Server)
                user = dtConf.Rows(0).Item("valore")
            End If

            If password = "" Then
                Dim securityHelper As New SecurityHelper
                password = securityHelper.ReadEncryptedFieldFromDb("password_smtp", objParametri_Server)
            End If

            Dim send As New SmtpClient(ClientSMTP)
            send.Credentials = New NetworkCredential(user, password)

            If enablessl = "" Then
                Dim dtConf As DataTable = objConf.Leggi(0, "enablessl_smtp", "", "", objParametri_Server)
                If dtConf.Rows.Count > 0 AndAlso (dtConf.Rows(0).Item("valore").ToString.ToLower = "true" OrElse dtConf.Rows(0).Item("valore").ToString.ToLower = "false") Then
                    send.EnableSsl = CBool(dtConf.Rows(0).Item("valore"))
                End If
            Else
                If enablessl.ToLower = "true" OrElse enablessl.ToLower = "false" Then
                    send.EnableSsl = enablessl
                End If
            End If

            If ClientSMTP_Porta = "" Then
                Dim dtConf As DataTable = objConf.Leggi(0, "ClientSMTP_Porta", "", "", objParametri_Server)
                If dtConf.Rows.Count > 0 AndAlso IsNumeric(dtConf.Rows(0).Item("valore")) Then
                    send.Port = CInt(dtConf.Rows(0).Item("valore"))
                End If
            Else
                If IsNumeric(ClientSMTP_Porta) Then
                    send.Port = ClientSMTP_Porta
                End If
            End If

            'If AttendiRisposta Then
            send.Send(mail)
            'Else
            'AddHandler send.SendCompleted, AddressOf SendCompletedCallback
            ' The userState can be any object that allows your callback method to identify this send operation.
            ' For this example, the userToken is a string constant.
            ' https://msdn.microsoft.com/it-it/library/x5x13z6h(v=vs.110).aspx
            'Dim userState As String = Oggetto & "_" & DateTime.Now.ToShortDateString & "_" & DateTime.Now.ToShortTimeString
            'send.SendAsync(mail, "pippo")
            'DA SISTEMARE: 
            'Le operazioni asincrone non sono consentite in questo contesto. Per poter avviare 
            'un'operazione asincrona, una pagina deve avere l'attributo Async impostato su true e 
            'tale operazione può essere avviata in una pagina solo prima dell'evento PreRenderComplete.

            'End If
            'Dim t As New Threading.Thread(
            '        Sub() 
            '            send.SendAsync(mail, "pippo")
            '        End Sub
            ')
            't.Start()

        Catch ex As Exception
            errore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
        End Try

        mail.Dispose()

        Return errore

    End Function

    'Private Shared mailSent As Boolean = False
    'Private Shared Sub SendCompletedCallback(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
    '    ' Get the unique identifier for this asynchronous operation.
    '    Dim token As String = CStr(e.UserState)

    '    If e.Cancelled Then
    '        Console.WriteLine("[{0}] Send canceled.", token)
    '    End If
    '    If Not IsNothing(e.Error) Then
    '        Console.WriteLine("[{0}] {1}", token, e.Error.ToString())
    '    Else
    '        Console.WriteLine("Message sent.")
    '    End If
    '    mailSent = True
    'End Sub

End Class
