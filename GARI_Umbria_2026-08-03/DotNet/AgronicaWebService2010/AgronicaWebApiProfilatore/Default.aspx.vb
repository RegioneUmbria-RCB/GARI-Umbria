Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        lblUA.Text = My.Computer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt")

    End Sub

End Class


