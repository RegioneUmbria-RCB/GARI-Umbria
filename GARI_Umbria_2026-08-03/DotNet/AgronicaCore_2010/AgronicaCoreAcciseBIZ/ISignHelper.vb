Imports System.IO

Public Interface ISignHelper

    Function Sign(ByVal plainData As Byte(), ByVal percorsoFile As String) As String
    Sub Verify()
    Function LeggiContenutoSegnato(ByVal data As Byte(), ByVal nomeFileRicevuta As String) As MemoryStream


End Interface
