Imports AgronicaCoreDataProvider

Public Class MenuBS_Anagrafica_Permessi

    Public Property PermessiZoo As Boolean
    Public Property PermessiMeteoDSS As Boolean

End Class


Public Class MenuBS_Anagrafica_AvModAlg_Inizializza_Response

    Public Property ModelliPrevisionali As List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg)

    Public Property ModelliPrevisionaliTree As List(Of KendoHierarchicalDataSource)
    Public Property ModelliPrevisionaliTreeListaCheck As List(Of String)

    Public Property AssociazioneCentroOrigine As Boolean



End Class
