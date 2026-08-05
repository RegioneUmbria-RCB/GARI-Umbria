Public Interface ICacheableData

    ReadOnly Property CACHE_KEY_PREFIX As String

    Function Dammi_Contenuto_Cache(ByVal tipoCache As String) As List(Of Agronica_Oggetto_Cache)
    Function Rimuovi_Elemento_Dalla_Cache(ByVal chiave As String) As Boolean

    Sub Pulisci_Cache()

End Interface
