Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Particella

    Public Property Particella As ParticelleCatastali
    Public Property ImpreseParticella As List(Of ImpreseXParticelle)
    Public Property Zone As List(Of ZonexParticelle)
    Public Property Macrousi As List(Of ParticelleCatastalixMacrousi)
    Public Property Utilizzi As List(Of ParticelleCatastalixMacrousixUtilizzo)

End Class

Public Class G2G_Particelle
    Inherits G2G_Base

    Public Property ParticelleToInsert As List(Of G2G_Particella)
    Public Property ParticelleToUpdate As List(Of G2G_Particella)
    Public Property ParticelleToDelete As List(Of G2G_Particella)

End Class

Public Class G2G_Particelle_Reverse
    Inherits G2G_Base

    Public Property ParticelleToInsert As List(Of G2G_Particella)
    Public Property ParticelleToUpdate As List(Of G2G_Particella)
    Public Property ParticelleToDelete As List(Of G2G_Particella)

End Class
