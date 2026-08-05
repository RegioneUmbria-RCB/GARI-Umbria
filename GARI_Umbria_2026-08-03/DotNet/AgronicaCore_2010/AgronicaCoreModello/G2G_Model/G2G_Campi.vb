Imports AgronicaCoreEntityFramework_POCO


Public Class G2G_Campo

    Public Property Campo As Campi
    Public Property Utente As UtentiXCampi
    Public Property Codici As List(Of Campi_Codici)

End Class

Public Class G2G_Campi
    Inherits G2G_Base

    Public Property CampiToInsert As List(Of G2G_Campo)
    Public Property CampiToUpdate As List(Of G2G_Campo)
    Public Property CampiToDelete As List(Of G2G_Campo)

End Class

Public Class G2G_Campi_Reverse
    Inherits G2G_Base

    Public Property CampiToInsert As List(Of G2G_Campo)
    Public Property CampiToUpdate As List(Of G2G_Campo)
    Public Property CampiToDelete As List(Of G2G_Campo)

End Class
