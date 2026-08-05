Imports System.IO
Imports System.Xml
Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Attivo
Public Class SDI_CicloAttivo_LetturaEsiti

    Private ReadOnly _soapController As ISOAPControllerCicloAttivo
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _logger As EFatturaLogger
    Public Sub New(
                  ByVal soapController As ISOAPControllerCicloAttivo,
                ByVal fileManager As IFIleManager,
                  ByVal logger As EFatturaLogger
        )
        _soapController = soapController
        _fileManager = fileManager
        _logger = logger
    End Sub

    Public Function LeggiEsitiFattura(ByVal idSDI As String, ByRef response As InvoiceOutcomeResponseWrapper) As Boolean

        Dim nomeProcedura = "SDI_CicloAttivo_LetturaEsiti.LeggiEsitiFattura"

        response = New InvoiceOutcomeResponseWrapper With
        {
            .StatoSDI = AgronicaCoreEFatturaDAL.StatoFattura_SDI.NonDefinito,
            .MessaggiErrore = New List(Of Tuple(Of String, String)),
            .Descrizione = String.Empty
        }

        Try
            Dim eiOutcomes = _soapController.GetElectronicInvoiceOutcomes(idSDI)
            If eiOutcomes.ResultCode = ResultCode.Failure Then
                _logger.Logga(nomeProcedura, String.Format("La chiamata {0} per IDSDI {1} ha risposto con ResultCode.Failure", "GetElectronicInvoiceOutcomes", idSDI))
                Return False
            End If

            If Not eiOutcomes.ElectronicInvoiceOutcomes.Any() Then
                response.StatoSDI = AgronicaCoreEFatturaDAL.StatoFattura_SDI.EsitoNonAncoraDisponibile
                Return True
            End If

            Dim outcome = eiOutcomes.ElectronicInvoiceOutcomes.OrderByDescending(Function(o) o.DataCreazione).FirstOrDefault()
            response.TipoMessaggio = outcome.TipoMessaggio

            Dim fr = _soapController.GetFileElectronicInvoiceOutcome(outcome.Id)
            If fr.ResultCode = ResultCode.Failure Then
                _logger.Logga(nomeProcedura, String.Format("La chiamata {0} per IDSDI {1} e otcomeId {2} ha risposto con ResultCode.Failure", "GetFileElectronicInvoiceOutcome", idSDI, outcome.Id))
                Return False
            End If

            EstraiDettagliEsito(fr.File, response)

        Catch ex As MessageNamespaceNotManaged
            _logger.Logga("SDI_CicloAttivo_LetturaEsiti.LeggiEsitiFattura", ex)
            Throw ex
        Catch ex As Exception
            _logger.Logga("SDI_CicloAttivo_LetturaEsiti.LeggiEsitiFattura", ex)
            Return False
        End Try

        Return True

    End Function

    Private Sub EstraiDettagliEsito(
        ByVal data As Byte(), ByRef response As InvoiceOutcomeResponseWrapper
        )

        Dim stream = New MemoryStream(data)
        Dim xmlReader = New XmlTextReader(stream)
        xmlReader.Namespaces = False
        Dim doc = New XmlDocument()
        doc.Load(xmlReader)

        Dim messageNamespace = doc.DocumentElement.Name.ToLower()

        Dim errorList = doc.SelectSingleNode("//ListaErrori")
        If Not errorList Is Nothing Then
            For Each n As XmlNode In errorList
                response.MessaggiErrore.Add(New Tuple(Of String, String)(
                               n.SelectSingleNode("Codice").InnerText, n.SelectSingleNode("Descrizione").InnerText))
            Next
        Else
            Dim description = doc.SelectSingleNode("//Descrizione")
            If Not description Is Nothing Then
                response.Descrizione = description.InnerText
            End If
        End If

        Select Case messageNamespace
            Case "ns3:ricevutascarto"
                response.StatoSDI = AgronicaCoreEFatturaDAL.StatoFattura_SDI.RicevutaScato
            Case "ns3:ricevutaimpossibilitarecapito"
                response.StatoSDI = AgronicaCoreEFatturaDAL.StatoFattura_SDI.RicevutaMancataConsegna
            Case "ns3:ricevutaconsegna"
                response.StatoSDI = AgronicaCoreEFatturaDAL.StatoFattura_SDI.RicevutaConsegna
            Case Else
                Throw New NotImplementedException(String.Format("Tipo di messaggio non gestito {0}", messageNamespace))
        End Select

    End Sub
End Class

'AT: Perpetua Mancata Consegna - Solo PA
'DT: Decorrenza Termini - Solo PA
'MC: Mancata Consegna
'NS: Notifica di Scarto
'RC: Ricevuta di Consegna
'EC_ACCETTAZIONE: Esito Committente Accettazione - Solo PA
'EC_RIFIUTO: Esito Committente Rifiuto - Solo PA
'NONE: Esito non ancora arrivato, In attesa di esito
