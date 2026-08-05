Imports System.IO

Public Interface ISignHelper

    Function Segna(ByVal dati As Byte()) As String
    Function Segna(ByVal percorsoFileCompleto As String) As String
    Function LeggiContenutoSegnato(ByVal data As Byte()) As MemoryStream
    Function LeggiContenutoSegnato(ByVal percorsoFileCompleto As String) As MemoryStream
    Function VerificaFirmaValida(ByVal dati As Byte()) As Boolean
    Function VerificaFirmaValida(ByVal percorsoFileCompleto As String) As Boolean

    Function Verifica_Se_Firmato(ByVal dati As Byte()) As Boolean
    Function Verifica_Se_Firmato(ByVal percorsoFileCompleto As String) As Boolean

End Interface
