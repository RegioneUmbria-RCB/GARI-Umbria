<System.AttributeUsage(System.AttributeTargets.Class,
                       AllowMultiple:=False)>
Public Class CachedDataProviderAttribute
    Inherits System.Attribute

    Private ReadOnly _nomeGruppoCache As String = String.Empty
    Private ReadOnly _nomeGruppoCacheLetturaCollegato As String = String.Empty

    Public ReadOnly Property NomeGruppoCache() As String
        Get
            Return _nomeGruppoCache
        End Get
    End Property

    Public ReadOnly Property NomeGruppoCacheLetturaCollegato() As String
        Get
            Return _nomeGruppoCacheLetturaCollegato
        End Get
    End Property

    Sub New(ByVal nomeGruppoCache As String)
        _nomeGruppoCache = nomeGruppoCache
    End Sub
    Sub New(ByVal nomeGruppoCache As String, ByVal nomeGruppoCacheLetturaCollegato As String)
        _nomeGruppoCache = nomeGruppoCache
        _nomeGruppoCacheLetturaCollegato = nomeGruppoCacheLetturaCollegato
    End Sub

End Class


