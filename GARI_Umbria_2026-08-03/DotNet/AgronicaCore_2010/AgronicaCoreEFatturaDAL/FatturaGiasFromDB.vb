
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO

Public Enum FatturaElettronicaType

    FatturaPa
    FatturaSemplificata

End Enum
Public Class FatturaGiasFromDB

    Public FatturaType As FatturaElettronicaType

    Public Cedente As CedenteFromDb

    Public Cessionario As Object

    Public CessionarioDiverso As Object

    Public DatiFattura As DatiFatturaFromDB

    Public DocumentiCOllegati As IEnumerable(Of Object)

    Public RiepilogoDocumentiCollegati As IEnumerable(Of Object)

    Public OrdiniAcquisto As IEnumerable(Of Object)

    Public RifNotaCreditoDebito As Object

    Public FlagPa As Boolean

    Public DatiPagamento As IEnumerable(Of Object)

    Public LegaleRappresentante As Object

    Public PivaReale As String

    Public Sub New()
        FatturaType = FatturaElettronicaType.FatturaPa
    End Sub

End Class

Public Class CedenteFromDb

    Public TipoImpresa As enum_TipoImpresaGerarchia
    Public DatiPrincipali As Object
    Public DatiAggiuntivi As IEnumerable(Of Object)

End Class

Public Class DatiFatturaFromDB

    Public Testata As Object
    Public Dettagli As IEnumerable(Of Object)
    Public Cessionario As Object

End Class