Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Macchina

    Public Property Macchina As Parco_Macchine

    'Public Property Codici As List(Of Parco_Macchine_Codici)

    Public Property Costi As List(Of Prodotti_Costi)

End Class

Public Class G2G_Parco_Macchine
    Inherits G2G_Base

    Public Property ParcoMacchineToInsert As List(Of G2G_Macchina)
    Public Property ParcoMacchineToUpdate As List(Of G2G_Macchina)
    Public Property ParcoMacchineToDelete As List(Of G2G_Macchina)

End Class

Public Class G2G_Parco_Macchine_Reverse
    Inherits G2G_Base

    Public Property ParcoMacchineToInsert As List(Of G2G_Macchina)
    Public Property ParcoMacchineToUpdate As List(Of G2G_Macchina)
    Public Property ParcoMacchineToDelete As List(Of G2G_Macchina)

End Class
