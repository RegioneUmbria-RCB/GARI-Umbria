Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim MyComputer = New Microsoft.VisualBasic.Devices.Computer
        lblUA.Text = MyComputer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt")
    End Sub

End Class