Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Passivo
Public Interface ISOAPControllerCicloPassivo : Inherits ICicloPassivo
    Function ContattoHub() As Boolean

    Function CheckClienteFatturaPA() As Boolean

    Function LeggiFatture(ByVal dataInizio As Date) As Integer

    Function GetFatture(ByVal dataInizio As Date) As List(Of DatiFattura)

    Function GetFattura(ByVal idSDI As Long, ByVal codiceUfficio As String) As Fattura

End Interface
