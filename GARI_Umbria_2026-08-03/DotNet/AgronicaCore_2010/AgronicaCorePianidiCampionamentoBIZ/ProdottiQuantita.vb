
Public Class ProdottiQuantita

    Private _Qta As Decimal

    Public Sub New()
        ListaPA = New List(Of PrincipioAttivo)
    End Sub

    Public Property ListaPA As List(Of PrincipioAttivo)
    Public Property FR_Cod As Integer
    Public Property FR_Des As String
    Public Property Data As Date

    Public Property Qta As String
        Get
            Return _Qta
        End Get
        Set(ByVal value As String)
            _Qta = value
        End Set
    End Property

End Class



'#######################################################
'#######################################################
'#######################################################
'#######################################################

Public Class Formulato

    Private _FR_Des As String

    Public Property FR_Cod As Integer

    Public Property FR_Des As String
        Get
            Return _FR_Des
        End Get
        Set(ByVal value As String)
            _FR_Des = value
        End Set
    End Property

    Public Property ListaFamiglie As List(Of FamigliePA)
    Public Property ListaPA As List(Of PrincipioAttivo)

    Public Sub New()
        FR_Cod = 0
        _FR_Des = 0
        ListaPA = New List(Of PrincipioAttivo)
        ListaFamiglie = New List(Of FamigliePA)
    End Sub

End Class



Public Class PrincipioAttivo

    Public Sub New()
        Pa_Cod = 0
        Pa_Des = ""
        Titolo = ""
        Famiglie = New List(Of FamigliePA)
    End Sub

    Public Property Famiglie As List(Of FamigliePA)
    Public Property Pa_Cod As Integer
    Public Property Pa_Des As String
    Public Property Titolo As String

End Class

Public Class FamigliePA

    Public Sub New()
        Fam_Cod = 0
        Fam_Des = ""
    End Sub

    Public Property Fam_Cod As String
    Public Property Fam_Des As String

End Class
