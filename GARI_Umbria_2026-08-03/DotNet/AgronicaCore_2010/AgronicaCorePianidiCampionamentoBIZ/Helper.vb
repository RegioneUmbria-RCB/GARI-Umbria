Imports AgronicaCoreDataProvider

Public MustInherit Class Helper

    Public MustOverride Sub Salva(ByVal oggetto As Object, ByVal objParametri As AgronicaCoreParametri)

    Public MustOverride Sub Cancella(ByVal oggetto As Object, ByVal objParametri As AgronicaCoreParametri)

End Class
