Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports System.Net
Imports System.Net.Mail

Public Class RichiesteIscrizioni
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    <WebMethod(EnableSession:=False)>
    Public Shared Function Registrazione(ByVal Nome As String, ByVal Cognome As String, ByVal RagioneSociale As String, ByVal PartitaIva As String, ByVal Email As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_server) Then
                r.Sessione = False
                Return r
            End If

            If Email Is Nothing Or Email.Equals("") Then
                Throw New Exception("Email non può essere vuota")
            End If

            Dim msg As String =
                "<h2>RICHIESTA CODICE PERSONALE PER GIAS APP</h2><br/><h4>Grazie per averci contattato!</h4><br/><div>Il tuo codice personale è: 7gvfAp</div><h4>Sarai ricontattato al più presto da un nostro commerciale per eventuali servizi integrativi sulla piattaforma GIAS di Agronica.</h4><br><table><tr><th>Nome</th><td>$nome</td></tr><tr><th>Cognome</th><td>$cognome</td></tr><tr><th>RagioneSociale</th><td>$ragionesociale</td></tr><tr><th>PartitaIva</th><td>$partitaiva</td></tr><tr><tr><th><tr><th>PartitaIva</th><td>$partitaiva</td></tr><tr><th>Email</th><td>$email</td></tr></table>"

            msg = msg.Replace("$nome", Nome).Replace("$cognome", Cognome).Replace("$ragionesociale", RagioneSociale).Replace("$partitaiva", PartitaIva).Replace("$email", Email)

            Dim err = invia(objParametri_server, "service@agronicagroup.it", Email, "", "ufficiotecnico@diagramgroup.it", "Richiesta credenziali app", msg, True, Nothing, "", "", "", "", "")
            If err <> "" Then
                Throw New Exception(err)
            End If


            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Private Shared Function invia(objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Mittente As String, MailA As String, MailCC As String, MailCCN As String,
                          Oggetto As String, TestoMail As String, isBodyHTML As Boolean, Allegati As String(),
                          Optional ByVal ClientSMTP As String = "",
                          Optional ByVal ClientSMTP_Porta As String = "",
                          Optional ByVal user As String = "",
                          Optional ByVal password As String = "",
                          Optional ByVal enablessl As String = ""
                          ) As String


        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12

        Dim errore As String = ""

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
            send.UseDefaultCredentials = False
            send.Credentials = New NetworkCredential(user, password)
            send.DeliveryMethod = SmtpDeliveryMethod.Network

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

End Class