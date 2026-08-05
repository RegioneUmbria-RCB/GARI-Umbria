Imports System.Web.SessionState
Imports AgronicaControlli_2010

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'avvio dell'applicazione
        Dim GiasVersioneCorrente As String = ""
        Try
            GiasVersioneCorrente = "versione=" & My.Computer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt").Replace(" ", "").Replace(",", "_").Replace(":", "_").Replace("""", "").Replace("\", "").Replace("-", "_")

        Catch ex As Exception

        End Try
        Application("GiasVersioneCorrente") = GiasVersioneCorrente

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'avvio della sessione

    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

        Dim xCors As Boolean = True
        Try
            xCors = CBool(ConfigurationManager.AppSettings("abilitaCORS"))
        Catch ex As Exception

        End Try


        If xCors Then
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")

            If HttpContext.Current.Request.HttpMethod = "OPTIONS" Then
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST")
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, authorization")
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000")
                HttpContext.Current.Response.[End]()
            End If
        End If

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

End Class