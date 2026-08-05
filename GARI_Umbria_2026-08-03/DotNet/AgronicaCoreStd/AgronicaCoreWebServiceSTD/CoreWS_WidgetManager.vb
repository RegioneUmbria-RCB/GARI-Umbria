Imports InData.WidgetManager

Public Class CoreWS_WidgetManager
    Inherits APICallsBasic
    Public WMRequest As WidgetManagerRequest

    Public Sub New(
        ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
         WMRequest As WidgetManagerRequest
        )

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.WMRequest = WMRequest

    End Sub
End Class
