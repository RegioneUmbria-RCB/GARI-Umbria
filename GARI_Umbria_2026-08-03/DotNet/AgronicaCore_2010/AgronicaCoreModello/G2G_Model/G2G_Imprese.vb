
Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Impresa

    Public Property Impresa As Imprese

    Public Property Utente As UtentiXImprese

    Public Property Gerarchia As GerarchiaImprese

    Public Property Codici As List(Of Imprese_Codici)

    Public Property Indirizzi As List(Of G2G_Indirizzo)

    Public Property ContattoImpresa As G2G_Contatto

End Class

Public Class G2G_Imprese
    Inherits G2G_Base

    Public Property To_Piva_Padre As String
    Public Property ImpreseToInsert As List(Of G2G_Impresa)
    Public Property ImpreseToUpdate As List(Of G2G_Impresa)
    Public Property ImpreseToDelete As List(Of G2G_Impresa)

End Class

Public Class G2G_Imprese_Reverse
    Inherits G2G_Base

    Public Property To_Piva_Padre As String
    Public Property ImpreseToInsert As List(Of G2G_Impresa)
    Public Property ImpreseToUpdate As List(Of G2G_Impresa)
    Public Property ImpreseToDelete As List(Of G2G_Impresa)

End Class
