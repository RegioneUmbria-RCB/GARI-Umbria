Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Attivo
Imports AgronicaCoreEFatturaDAL

Public Class InvoiceOutcomeResponseWrapper

    Public TipoMessaggio As SdiTipoNotificaAR
    Public StatoSDI As StatoFattura_SDI
    Public MessaggiErrore As List(Of Tuple(Of String, String))
    Public Descrizione As String

End Class
