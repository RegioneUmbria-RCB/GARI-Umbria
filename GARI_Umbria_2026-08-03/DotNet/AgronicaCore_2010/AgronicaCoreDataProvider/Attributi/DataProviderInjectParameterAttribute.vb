
<System.AttributeUsage(System.AttributeTargets.Class Or
                       System.AttributeTargets.Method,
                       AllowMultiple:=True)>
Public Class DataProviderInjectParameterAttribute
    Inherits System.Attribute

    Public ReadOnly Property UsaInjectionParametri() As Boolean
        Get
            Return _usaInjectionParametri
        End Get
    End Property

    Public ReadOnly Property IdentificatoreMetodo() As Guid
        Get
            Return _identificatoreMetodo
        End Get
    End Property


    Private _usaInjectionParametri As Boolean
    Private _identificatoreMetodo As Guid

    Sub New(ByVal usaInjectionParametri As Boolean, ByVal id As String)
        _usaInjectionParametri = usaInjectionParametri
        _identificatoreMetodo = Guid.Parse(id)
        Console.WriteLine(_identificatoreMetodo.ToString())
    End Sub

End Class