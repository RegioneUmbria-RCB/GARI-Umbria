Public Class RigheSelezionate
    Public Gruppi_Utente_codici As Integer()
    Public Ids_Gruppo_Merce As Integer()
End Class

Public Class ScriviDatiResult
    Public DT As DataTable
    Public NumElementiInseriti As Integer
End Class

Public Module Constants
    Public ReadOnly NO_IMPORTED_ELEMENTS As Integer = 0
    Public ReadOnly SUCCESS As Integer = 1
End Module

Public Class ChiaveCancellaPermesso
    Public Gruppi_Utente_cod As Integer
    Public Id_Gruppo_Merce As Integer
    Public Piva As String
End Class

Public Class RispostaTentativoCancellazioneGruppoMerce
    Public Risultato As RisultatoCancellazione
    Public NumberConflicts As Integer
End Class

Public Enum RisultatoCancellazione
    SUCCESS = 0
    RICHIEDE_CONFERMA_CANCELLAZIONE = 1
    FAILURE = 2
End Enum
