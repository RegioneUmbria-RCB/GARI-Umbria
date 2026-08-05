Imports AgronicaCoreEntityFramework_POCO

Public Class RelazioneCantineInterniVasche
    Public Property CellaPresente As Boolean
    Public Property CancellazioneTotale As Boolean
    Public Property Cantine_Interni As Cantina_Insiemi
    Public Property CantinaVasche As List(Of Cantina_Vasche)
End Class
