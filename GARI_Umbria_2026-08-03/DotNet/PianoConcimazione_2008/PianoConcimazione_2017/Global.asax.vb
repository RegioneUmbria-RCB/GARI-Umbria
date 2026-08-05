Imports System.Web.SessionState
Imports AgronicaControlli_2010

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando l'applicazione viene avviata
        Dim GiasVersioneCorrente As String = ""
        Try
            GiasVersioneCorrente = "versione=" + My.Computer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt").Replace(" ", "").Replace(",", "_").Replace(":", "_").Replace("""", "").Replace("\", "").Replace("-", "_")

        Catch ex As Exception

        End Try
        Application("GiasVersioneCorrente") = GiasVersioneCorrente

        ' inizializza stringhe connessione in application
        GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando la sessione viene avviata
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'inizio di ogni richiesta
        GlobalAsax_Helper.Manage_BeginRequest_Security(sender, e, Me)
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al tentativo di autenticare l'utilizzo
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando si verifica un errore
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine della sessione
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine dell'applicazione
    End Sub

    Private Sub Global_asax_PreRequestHandlerExecute(sender As Object, e As EventArgs) Handles Me.PreRequestHandlerExecute

        GlobalAsax_Helper.Manage_PreRequestHandlerExecute_TimeZone(sender, e, Me)

    End Sub

    Private Sub Global_asax_PostMapRequestHandler(sender As Object, e As EventArgs) Handles Me.PostMapRequestHandler
        GlobalAsax_Helper.Manage_PostMapRequestHandler_Security(sender, e)
    End Sub
End Class