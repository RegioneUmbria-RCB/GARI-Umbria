Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Attivo
Public Interface ISOAPControllerCicloAttivo : Inherits ICicloAttivo

    Function ContattoHub() As Boolean

    Function CheckClienteFatturaPA(ByRef errore As String) As Boolean
    Function GetNomeFileZipFatturaPA() As String
    Function UploadFileFatturaPA(ByVal zipFileFullPath As String)
    Function SendElectronicInvoice(ByVal zipFileFullPath As String, ByVal emails As String) As SendElectronicInvoiceResponse

    Function GetElectronicInvoiceOutcomes(ByVal idSDI As Long) As ElectronicInvoiceOutcomeResponse

    Function GetFileElectronicInvoiceOutcome(ByVal idOutcome As String) As FileResponse

End Interface
