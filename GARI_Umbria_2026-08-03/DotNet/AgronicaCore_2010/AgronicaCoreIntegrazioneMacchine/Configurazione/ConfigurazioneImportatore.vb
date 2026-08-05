
Public Class Importatori
    Public Configurazioni As New List(Of ConfigurazioneImportatore)
End Class
Public Class ConfigurazioneImportatore
    Public PercorsoInput As String
    Public PercorsoOutput As String
    Public PercorsoLog As String
    Public PercorsoLavoro As String
    Public AbilitaStorico As Boolean = False
    Public Abilitato As Boolean = False
    Public OnDemand As Boolean = False
    Public IntervalloPollingInSecondi As Integer = -1
    Public Type As String
    Public IdServizio As String
    Public ParametriAgg As String
    Public ImportazioneImmediata As Boolean = True
End Class

