Public Class ImpreseResult

    Public Piva As String
    Public RagioneSociale As String
    Public Cuaa As String
    Public UltimoFascicoloImportato As SchedaFascicolo
    Public UltimoFascicoloAGEA As SchedaFascicolo
    Public importata As Boolean
    Public tempoTotale As Integer
    Public Ris As String
    Public Err As String

    Public Overrides Function ToString() As String
        Return vbCrLf & "Piva:" & Piva & vbCrLf &
               "RagioneSociale:" & RagioneSociale & vbCrLf &
               "Cuaa:" & Cuaa & vbCrLf &
               "importata:" & CStr(importata) & vbCrLf &
               "tempoTotale:" & tempoTotale & vbCrLf &
               "Ris:" & Ris & vbCrLf &
               "Err:" & Err
    End Function

End Class