Public Class CoreWS_AttivitaXCentri_Aziendali
    Inherits APICallsBasic

    Public piva As String
    Public Utilizzo_GiasAPP As Boolean
    Public flag_inclusa As Integer

    Public Sub New(ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)
        Me.piva = piva
        Me.Utilizzo_GiasAPP = True
        Me.flag_inclusa = -1

    End Sub
End Class
