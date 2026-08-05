Public Class CoreWS_UnitaMisuraAlternativeLeggi
    Inherits APICallsBasic

    Public Property Udm_Cod_From As Integer
    Public Property Validita_Inizio As Date
    Public Property Validita_Fine As Date

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                   Udm_Cod_From As Integer,
                   Validita_Inizio As Date,
                   Validita_Fine As Date)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.Udm_Cod_From = Udm_Cod_From
        Me.Validita_Inizio = Validita_Inizio
        Me.Validita_Fine = Validita_Fine

    End Sub

End Class
