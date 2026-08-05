Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_MateriePrime
    Inherits G2G_Base

    Public Property MateriePrimeToInsert As List(Of Materie_Prime)
    Public Property MateriePrimeToUpdate As List(Of Materie_Prime)
    Public Property MateriePrimeToDelete As List(Of Materie_Prime)
    Public Property Materie_Prime_XLingueToInsert As List(Of Materie_Prime_XLingue)
    Public Property Materie_Prime_XLingueToDelete As List(Of Materie_Prime_XLingue)


End Class

Public Class G2G_MateriePrime_Reverse
    Inherits G2G_Base

    Public Property MateriePrimeToInsert As List(Of Materie_Prime)
    Public Property MateriePrimeToUpdate As List(Of Materie_Prime)
    Public Property MateriePrimeToDelete As List(Of Materie_Prime)

End Class