Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Fabbricato

    Public Property Fabbricato As Fabbricati

    Public Property Indirizzo As Indirizzi

    Public Property Codici As List(Of Fabbricati_Codici)

End Class

Public Class G2G_Fabbricati
    Inherits G2G_Base

    Public Property FabbricatiToInsert As List(Of G2G_Fabbricato)
    Public Property FabbricatiToUpdate As List(Of G2G_Fabbricato)
    Public Property FabbricatiToDelete As List(Of G2G_Fabbricato)

End Class

Public Class G2G_Fabbricati_Reverse
    Inherits G2G_Base

    Public Property FabbricatiToInsert As List(Of G2G_Fabbricato)
    Public Property FabbricatiToUpdate As List(Of G2G_Fabbricato)
    Public Property FabbricatiToDelete As List(Of G2G_Fabbricato)

End Class