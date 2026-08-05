Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_CentroAziendale

    Public Property Centro As Centri_Aziendali
    Public Property Utente As UtentiXStrutture
    Public Property Codici As List(Of Centri_Aziendali_Codici)
    Public Property Indirizzi As List(Of G2G_Indirizzo)

    'Public Property Fabbricati As G2G_Fabbricati

End Class

Public Class G2G_CentriAziendali
    Inherits G2G_Base

    Public Property CentriAziendaliToInsert As List(Of G2G_CentroAziendale)
    Public Property CentriAziendaliToUpdate As List(Of G2G_CentroAziendale)
    Public Property CentriAziendaliToDelete As List(Of G2G_CentroAziendale)

End Class

Public Class G2G_CentriAziendali_Reverse
    Inherits G2G_Base

    Public Property CentriAziendaliToInsert As List(Of G2G_CentroAziendale)
    Public Property CentriAziendaliToUpdate As List(Of G2G_CentroAziendale)
    Public Property CentriAziendaliToDelete As List(Of G2G_CentroAziendale)

End Class
