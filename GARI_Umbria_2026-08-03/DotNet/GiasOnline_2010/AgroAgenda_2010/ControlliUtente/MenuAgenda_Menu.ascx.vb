Public Class MenuAgenda_Menu
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Public WriteOnly Property Enable() As Boolean
        Set(ByVal value As Boolean)
            Me.Menu.Enabled = value
        End Set
    End Property

End Class