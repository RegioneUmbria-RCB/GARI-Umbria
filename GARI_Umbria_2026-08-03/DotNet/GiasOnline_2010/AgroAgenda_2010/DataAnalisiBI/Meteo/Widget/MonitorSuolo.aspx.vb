Imports AgronicaCoreDataProvider

Public Class MonitorSuolo
    Inherits System.Web.UI.Page

    Private Sub MonitorSuolo_Init(sender As Object, e As EventArgs) Handles Me.Init
        'AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False
    End Sub

End Class


