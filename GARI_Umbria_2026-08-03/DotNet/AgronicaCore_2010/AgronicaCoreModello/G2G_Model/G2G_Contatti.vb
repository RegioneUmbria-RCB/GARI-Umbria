Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Contatto_Allegati

    Public Property Entita As List(Of Alert_Entita)
    Public Property Documenti As List(Of Allegati_Documenti)
    Public Property Scadenze As List(Of Alert_Elenco)

End Class

Public Class G2G_Contatto

    Public Property Contatto As Contatti
    Public Property Codici As List(Of Contatti_Codici)
    Public Property Indirizzi As List(Of G2G_Indirizzo)
    Public Property RisorseUmane As List(Of Risorse_Umane)
    Public Property Rubrica As List(Of Rubrica)
    Public Property Costi As List(Of Prodotti_Costi)
    Public Property Allegati As G2G_Contatto_Allegati

End Class

Public Class G2G_Contatti
    Inherits G2G_Base

    Public Property ContattiToInsert As List(Of G2G_Contatto)
    Public Property ContattiToUpdate As List(Of G2G_Contatto)
    Public Property ContattiToDelete As List(Of G2G_Contatto)
    Public Property GestioneAllegati As Boolean

End Class

Public Class G2G_Contatti_Reverse
    Inherits G2G_Base

    Public Property ContattiToInsert As List(Of G2G_Contatto)
    Public Property ContattiToUpdate As List(Of G2G_Contatto)
    Public Property ContattiToDelete As List(Of G2G_Contatto)
    Public Property GestioneAllegati As Boolean

End Class
