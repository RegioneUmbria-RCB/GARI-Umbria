Imports AgronicaCoreEFatturaBIZ.Integrazione2c

Public Interface ICicloAttivo

    Function InviaFattura() As Boolean
    Function InviaFattura(ByVal nomeFIleXml As String, ByRef response As SendInvoiceResponseWrapper) As Boolean
    Function LeggiEsitiFattura(ByVal idSDI As String, ByRef response As InvoiceOutcomeResponseWrapper) As Boolean


End Interface
