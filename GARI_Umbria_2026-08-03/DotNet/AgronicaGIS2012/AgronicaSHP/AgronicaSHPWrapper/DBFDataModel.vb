
Public Class DBFDataModel_MappaturaDati_Costanti
    Public Const Operazione_RimappaEtichetta As String = "RimappaEtichetta"
    Public Const Operazione_NuovaEtichetta As String = "NuovaEtichetta"
    Public Const Operazione_RimuoviEtichetta As String = "RimuoviEtichetta"
End Class

Public Class DBFDataModel_MappaturaDati
    Public Property CampoDaRimappare As String
    Public Property NuovoCampo As String
    Public Property Operazione As String
    Public Property ValoreDefault As String
End Class
