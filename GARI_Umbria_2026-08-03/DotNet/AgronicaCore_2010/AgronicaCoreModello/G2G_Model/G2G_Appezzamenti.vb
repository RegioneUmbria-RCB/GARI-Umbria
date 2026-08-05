Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Appezzamento

    Public Property Appezzamento As Appezzamento
    Public Property Utente As UtentiXAppezzamenti
    Public Property Codici As List(Of Appezzamento_Codici)
    Public Property Particelle As List(Of AppezzamentiXParticelle)
    Public Property Macrousi As List(Of AppezzamentiXParticellexMacrousi)
    Public Property Utilizzi As List(Of AppezzamentiXParticellexMacrousixUtilizzo)

End Class

Public Class G2G_Appezzamenti
    Inherits G2G_Base

    Public Property AppezzamentiToInsert As List(Of G2G_Appezzamento)
    Public Property AppezzamentiToUpdate As List(Of G2G_Appezzamento)
    Public Property AppezzamentiToDelete As List(Of G2G_Appezzamento)

End Class

Public Class G2G_Appezzamenti_Reverse
    Inherits G2G_Base

    Public Property AppezzamentiToInsert As List(Of G2G_Appezzamento)
    Public Property AppezzamentiToUpdate As List(Of G2G_Appezzamento)
    Public Property AppezzamentiToDelete As List(Of G2G_Appezzamento)

End Class

