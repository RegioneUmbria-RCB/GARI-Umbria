Public Class CoreWS_CategorieXUnitaMisura
    Inherits APICallsBasic

    Public Elem_Cod As Integer
    Public Udm_Cod As Integer
    Public Cau_Mov As String
    Public Flag_Cantina As Boolean
    Public xFiltroAggiuntivo As String

    Public Sub New(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal Elem_Cod As Integer,
        ByVal Udm_Cod As Integer,
        ByVal Cau_Mov As String,
        ByVal Flag_Cantina As Boolean,
        ByVal xFiltroAggiuntivo As String
    )


        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.Elem_Cod = Elem_Cod
        Me.Udm_Cod = Udm_Cod
        Me.Cau_Mov = Cau_Mov
        Me.Flag_Cantina = Flag_Cantina
        Me.xFiltroAggiuntivo = xFiltroAggiuntivo


    End Sub


    Public Sub New(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String
    )


        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Elem_Cod = -100 'impostare per lettura da APP
        Cau_Mov = ""
        xFiltroAggiuntivo = ""


    End Sub

End Class
