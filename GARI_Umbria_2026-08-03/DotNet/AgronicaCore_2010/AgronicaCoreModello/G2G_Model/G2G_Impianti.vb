Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Impianto

    Public Property Impianto As Reg_Impianti
    Public Property Codici As List(Of Reg_Impianti_Codici)

End Class


Public Class G2G_Impianti
    Inherits G2G_Base

    Public Property ImpiantiToInsert As List(Of G2G_Impianto)
    Public Property ImpiantiToUpdate As List(Of G2G_Impianto)
    Public Property ImpiantiToDelete As List(Of G2G_Impianto)

End Class

Public Class G2G_Impianti_Reverse
    Inherits G2G_Base

    Public Property ImpiantiToInsert As List(Of G2G_Impianto)
    Public Property ImpiantiToUpdate As List(Of G2G_Impianto)
    Public Property ImpiantiToDelete As List(Of G2G_Impianto)

End Class
