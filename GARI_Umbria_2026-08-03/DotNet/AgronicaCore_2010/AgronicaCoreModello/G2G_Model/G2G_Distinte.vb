Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Distinta

    Public Property Distinta As Imprese_Progetti
    Public Property Codici As List(Of Reg_Impianti_Codici)
    Public Property Fasi As List(Of Imprese_Progetto_Fasi)

End Class

Public Class G2G_Distinte
    Inherits G2G_Base

    Public Property DistinteToInsert As List(Of G2G_Distinta)
    Public Property DistinteToUpdate As List(Of G2G_Distinta)
    Public Property DistinteToDelete As List(Of G2G_Distinta)

End Class

Public Class G2G_Distinte_Reverse
    Inherits G2G_Base

    Public Property DistinteToInsert As List(Of G2G_Distinta)
    Public Property DistinteToUpdate As List(Of G2G_Distinta)
    Public Property DistinteToDelete As List(Of G2G_Distinta)

End Class
