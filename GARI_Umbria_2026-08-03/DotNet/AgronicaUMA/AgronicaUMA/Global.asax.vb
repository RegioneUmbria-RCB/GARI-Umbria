Imports System.Web
Imports System.Web.SessionState
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider

Public Class Global_asax
    Inherits System.Web.HttpApplication

#Region " Codice generato da Progettazione componenti "

    Public Sub New()
        MyBase.New()

        'Chiamata richiesta da Progettazione componenti.
        InitializeComponent()

        'Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent()

    End Sub

    'Richiesto da Progettazione componenti
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione componenti.
    'Può essere modificata in Progettazione componenti.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region


    '################################################################################
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'apertura della sessione

    End Sub


    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'avvio dell'applicazione
        Dim GiasVersioneCorrente As String = ""
        Try
            Dim MyComputer = New Microsoft.VisualBasic.Devices.Computer
            GiasVersioneCorrente = "versione=" + MyComputer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt").Replace(" ", "").Replace(",", "_").Replace(":", "_").Replace("""", "").Replace("\", "").Replace("-", "_")

        Catch ex As Exception

        End Try
        Application("GiasVersioneCorrente") = GiasVersioneCorrente

        ' inizializza stringhe connessione in application
        GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

    End Sub


    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'inizio di ogni richiesta
        GlobalAsax_Helper.Manage_BeginRequest_Security(sender, e, Me)
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato durante il tentativo di autenticazione dell'utente
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando si verifica un errore
        'lo faccio solo se non è scaduta la sessione, altrimenti non riuscirei a memorizzare lo stack
        If TypeOf Context.Handler Is IRequiresSessionState OrElse
            TypeOf Context.Handler Is IReadOnlySessionState Then
            Session("objTrace") = Server.GetLastError
        End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato alla fine della sessione
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato alla chiusura dell'applicazione
    End Sub

    Private Sub Global_asax_PostMapRequestHandler(sender As Object, e As EventArgs) Handles Me.PostMapRequestHandler
        GlobalAsax_Helper.Manage_PostMapRequestHandler_Security(sender, e)
    End Sub
End Class
