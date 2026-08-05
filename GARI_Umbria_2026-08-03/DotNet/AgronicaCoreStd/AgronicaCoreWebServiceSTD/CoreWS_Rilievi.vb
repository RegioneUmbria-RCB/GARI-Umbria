Public Class CoreWS_Rilievi
    Inherits APICallsBasic

    Public Property lavCod As String
    Public Property vegCod As String
    Public Property dpiCod As String
    Public Property idRcdpi As String
    Public Property dpiPubblicoPrivato As String
    Public Property personalizzate As Boolean
    Public Property filtraSpecie As Boolean

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                   lavCod As String,
                   vegCod As String,
                   dpiCod As String,
                   idRcdpi As String,
                   dpiPubblicoPrivato As String,
                   personalizzate As Boolean,
                   filtraSpecie As Boolean)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.lavCod = lavCod
        Me.vegCod = vegCod
        Me.dpiCod = dpiCod
        Me.idRcdpi = idRcdpi
        Me.dpiPubblicoPrivato = dpiPubblicoPrivato
        Me.personalizzate = personalizzate
        Me.filtraSpecie = filtraSpecie

    End Sub

End Class
