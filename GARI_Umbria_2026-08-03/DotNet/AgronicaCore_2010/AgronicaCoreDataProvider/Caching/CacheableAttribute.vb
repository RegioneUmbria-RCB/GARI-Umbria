<System.AttributeUsage(System.AttributeTargets.Method,
                       AllowMultiple:=False)>
Public Class CacheableAttribute
    Inherits System.Attribute

    Private ReadOnly _cacheable As Boolean = False
    Private _nomeClasse As String = String.Empty
    Private _nomeMetodo As String = String.Empty

    Public ReadOnly Property Cacheable() As Boolean
        Get
            Return _cacheable
        End Get
    End Property

    Public Property NomeClasse() As String
        Get
            Return _nomeClasse
        End Get
        Set(value As String)
            _nomeClasse = value
        End Set

    End Property

    Public Property NomeMetodo() As String
        Get
            Return _nomeMetodo
        End Get
        Set(value As String)
            _nomeMetodo = value
        End Set
    End Property

    Sub New(ByVal cacheable As Boolean)
        _cacheable = cacheable
    End Sub
    Sub New(ByVal cacheable As Boolean, ByVal nomeClasse As String)
        Me.New(cacheable)
        _nomeClasse = nomeClasse
    End Sub
    Sub New(ByVal cacheable As Boolean, ByVal nomeClasse As String, ByVal nomeMetodo As String)
        Me.New(cacheable, nomeClasse)
        _nomeMetodo = nomeMetodo
    End Sub

End Class



