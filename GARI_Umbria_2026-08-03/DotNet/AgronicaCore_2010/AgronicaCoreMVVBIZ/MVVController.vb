Imports AgronicaCoreDataProvider
Imports AgronicaCoreMVVBIZ.Persisters
Imports AgronicaCoreMVVDal
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class MVVController

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _soapController As SoapController_MVV
    Private ReadOnly _xmlGenerator As MVV_XmlGenerator
    Private ReadOnly _fsPersister As FileSystemPersister
    Private ReadOnly _memoryPersister As MemoryPersister
    Private ReadOnly _xmlValidator As XMLValidator
    Private ReadOnly _logHelper As MVV_Log_Helper
    Private ReadOnly _configMin As MVVServiceConfigMin
    Private ReadOnly _logger As MVVLogger
    Private ReadOnly _agendaWriter As Agenda_W

    Sub New(ByRef objParametriServer As AgronicaCoreParametri,
            ByRef objParametriUtenti As AgronicaCoreParametri,
            ByVal soapController As SoapController_MVV,
            ByVal fsPersister As FileSystemPersister,
            ByVal xmlValidator As XMLValidator,
            ByVal configMin As MVVServiceConfigMin,
            ByVal logger As MVVLogger)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _soapController = soapController
        _xmlGenerator = New MVV_XmlGenerator(objParametriServer, objParametriUtenti, configMin, logger)
        _fsPersister = fsPersister
        _memoryPersister = New MemoryPersister()
        _xmlValidator = xmlValidator
        _logHelper = New MVV_Log_Helper(objParametriServer, configMin.TipoTracciato)
        _agendaWriter = New Agenda_W(objParametriServer)
        _configMin = configMin
        _logger = logger

    End Sub

    Public Function GeneraMVV(ByVal piva As String, ByVal idAgenda As Integer) As Integrazione.SIAN.MVV.MVVSiRPVInput

        Return Nothing

    End Function

    Public Function GeneraMVVTest(ByVal CodTipoProd As String, ByVal CodProdotto As String) As Integrazione.SIAN.MVV.MVVSiRPVInput

        Dim prodotto As New Integrazione.SIAN.MVV.ProdMVV With {
                    .CodTipoProdMVV = CodTipoProd,
                    .Prodotto = New Integrazione.SIAN.MVV.ProdMVVProdotto With {
                        .Item = New Integrazione.SIAN.MVV.CodiceProdotto With {
                            .CodPrimario = _configMin.CodIcqrf,
                            .CodSecondario = CodProdotto
                        }
                    },
                    .UniMis = "l",
                    .Quant = 560
                }

        Dim mvv As New Integrazione.SIAN.MVV.MVVSiRPVInput With {
                .CodOper = New Integrazione.SIAN.MVV.CUAA With {
                    .Item = _configMin.CodOper,
                    .ItemElementName = If(_configMin.PersonaFisica, Integrazione.SIAN.MVV.ItemChoiceType.PersonaFisica, Integrazione.SIAN.MVV.ItemChoiceType.PersonaGiuridica)
                },
                .CodIcqrfSped = _configMin.CodIcqrf,
                .Destinatario = New Integrazione.SIAN.MVV.MVVSiRPVInputDestinatario With {
                    .Item = New Integrazione.SIAN.MVV.DettaglioSoggettoMVV With {
                        .TipoSoggMVV = Integrazione.SIAN.MVV.DettaglioSoggettoMVVTipoSoggMVV.IT,
                        .IndirizzoSoggMVV = New Integrazione.SIAN.MVV.Indirizzo With {
                            .Indirizzo1 = "Indirizzo di Prova",
                            .Stato = "380"
                        }
                    }
                },
                .Trasportatore = New Integrazione.SIAN.MVV.MVVSiRPVInputTrasportatore With {
                    .Item = New Integrazione.SIAN.MVV.DettaglioSoggettoMVV With {
                        .TipoSoggMVV = Integrazione.SIAN.MVV.DettaglioSoggettoMVVTipoSoggMVV.IT,
                        .IndirizzoSoggMVV = New Integrazione.SIAN.MVV.Indirizzo With {
                            .Indirizzo1 = "Vettore di Prova",
                            .Stato = "380"
                        }
                    }
                },
                .TipmeCod = "03",
                .MezzoTarga = "XYZ",
                .TitrCod = "mm",
                .DataTrasp = New Date(2020, 12, 20),
                .OraTrasp = Integrazione.SIAN.MVV.MVVSiRPVInputOraTrasp.Item10,
                .MinutiTrasp = Integrazione.SIAN.MVV.MVVSiRPVInputMinutiTrasp.Item38,
                .ProdottiMVV = New Integrazione.SIAN.MVV.ProdMVV() {prodotto}
            }

        Return mvv

    End Function

    Public Function InviaMVV(ByVal piva As String, ByVal idAgenda As Integer) As List(Of String)

        Dim nomeProcedura = "MVVController.InviaMVV"

        Dim dataXML As Date = Date.Now
        Dim errori As New List(Of String)
        Dim mvv As Object = GeneraMVV(piva, idAgenda, errori)
        'Dim mvv As Object = GeneraMVVTest("01", "LAMBROSSO") ' test BARONE
        'Dim mvv As Object = GeneraMVVTest("06", "13SG3") ' test RUGGERI

        If IsNothing(mvv) Then
            _logger.Logga(nomeProcedura, "XML non generabile")
            _logHelper.ScriviLog(piva, idAgenda, "", "", AGRODATAINIZIO, "", "", StatoMVV_Gias.XMLNonGenerabile, StatoMVV_Sian.NonDefinito)
            Return errori
        End If


        ' TODO da riguardare
        'Dim mvvStream = _memoryPersister.ToMemoryStream(mvv)
        'Dim postValidazioneOk As Boolean = _xmlValidator.Validate(mvvStream, errori)
        'If Not postValidazioneOk Then
        '    _logger.Logga(nomeProcedura, "XML non valido")
        '    _logHelper.ScriviLog(piva, idAgenda, "", "", AGRODATAINIZIO, "", "", StatoMVV_Gias.XMLNonGeneratoPerErrori, StatoMVV_Sian.NonDefinito)
        '    Return errori
        'End If

        ' scrive XML
        Dim nomeFile = "MVV" & piva & "_" & idAgenda & ".xml"
        _logger.Logga(nomeProcedura, "Scrive XML " & nomeFile)
        _fsPersister.Persist(mvv, nomeFile)

        ' invio MVV
        Dim response = _soapController.InviaMvv(mvv, errori)
        Dim numMVV As String = response.MVVSiRPVOutput.NumMVV
        Dim marcaTemp As String = response.MVVSiRPVOutput.MarcaTemp
        Dim esito = response.MVVSiRPVOutput.Esito

        ' log MVV
        If Not String.IsNullOrEmpty(numMVV) Then

            Dim nomeFilePdf As String = Replace(numMVV, "/", "_") & ".pdf"
            _logger.Logga(nomeProcedura, "Scrive PDF " & nomeFilePdf)

            ' sposta xml nei spediti
            _fsPersister.SpostaXML(nomeFile)

            ' salvo file pdf ricevuto
            If response.MVVSiRPVOutput.filePdf IsNot Nothing Then
                _fsPersister.Persist(nomeFilePdf, response.MVVSiRPVOutput.filePdf)
            End If

            ' scrivo il log OK
            _logger.Logga(nomeProcedura, "Invia XML " & nomeFile)
            _logHelper.ScriviLog(piva, idAgenda, numMVV, marcaTemp, dataXML, nomeFile, numMVV & ".pdf", StatoMVV_Gias.InviatoMVV_OK, StatoMVV_Sian.Validato)
            _logHelper.ScriviLogDettaglio(piva, idAgenda, "", esito.codice, 0, esito.messaggio)

            ' aggiorno campo doc_numero_sin dell'mmv tramite piva e id_agenda
            _agendaWriter.Aggiorna_MVV(piva, idAgenda, Formatta_NumeroMVV_X_Agenda(numMVV))

        Else

            ' scrivo il log KO
            _logger.Logga(nomeProcedura, "ERRORE Invio XML " & nomeFile)
            _logHelper.ScriviLog(piva, idAgenda, "", "", dataXML, nomeFile, "", StatoMVV_Gias.InviatoMVV_KO, StatoMVV_Sian.NonDefinito)
            _logHelper.ScriviLogDettaglio(piva, idAgenda, "", esito.codice, 0, esito.messaggio)
            errori.Add(esito.messaggio)

        End If

        Return errori

    End Function

    Public Function ConsultaMVV(ByRef errori As List(Of String)) As String

        Return _soapController.ConsultaMvv(errori)

    End Function

    Public Function ScaricaMVV(ByVal piva As String, ByVal numMVV As String, ByRef fileMVV As Byte()) As String

        Dim messaggio = _soapController.ScaricaMvv(numMVV, fileMVV)
        Dim nomeFilePdf As String = Replace(numMVV, "/", "_") & ".pdf"

        If fileMVV IsNot Nothing Then
            _fsPersister.Persist(nomeFilePdf, fileMVV)
        Else
            _logHelper.ScriviLogDettaglio(piva, 0, numMVV, "", 0, If(messaggio = "", "Servizio PrnMVVSiRPV non disponibile", messaggio))
            _fsPersister.LeggiPDF(nomeFilePdf, fileMVV)
            If fileMVV Is Nothing Then
                messaggio = "File non disponibile"
            End If
        End If

        Return messaggio

    End Function

    Public Function AnnullaMVV(ByVal piva As String, ByVal numMVV As String) As String

        Dim messaggio As String = ""
        Dim errori As New List(Of String)
        Dim response = _soapController.AnnullaMvv(numMVV, errori)
        Dim esito = response.AnnMVVSiRPVOutput.Esito

        If errori.Count > 0 Then
            messaggio = errori.FirstOrDefault
        ElseIf response.AnnMVVSiRPVOutput.Esito IsNot Nothing Then
            If esito.codice = "012" Then
                _logHelper.AggiornaLog(piva, 0, numMVV, StatoMVV_Gias.InviatoMVV_OK, StatoMVV_Sian.Annullato)
            End If
            _logHelper.ScriviLogDettaglio(piva, 0, numMVV, esito.codice, 0, esito.messaggio)
            messaggio = esito.messaggio
        End If

        Return messaggio

    End Function

    Private Function GeneraMVV(ByVal PIVA As String, ByVal idAgenda As Integer, ByRef errori As List(Of String)) As Integrazione.SIAN.MVV.MVVSiRPVInput

        Return _xmlGenerator.GeneraMVV(_soapController.CodOper, PIVA, idAgenda, errori)

    End Function

    Public Function ControlliPreliminariMVV(ByVal PIVA As String, ByVal idAgenda As Integer) As List(Of String)

        Return _xmlGenerator.CheckPreliminari(PIVA, idAgenda)

    End Function

    Private Function Formatta_NumeroMVV_X_Agenda(ByVal numeroMVVSian As String) As String

        If String.IsNullOrEmpty(numeroMVVSian) Then
            Return String.Empty
        End If

        ' MVV-E/TV/288/1/2021
        Try
            Dim arrDati = numeroMVVSian.Split("/")
            Return String.Format("MVVE{0}/{1}", arrDati(3), arrDati(4).Substring(2))

        Catch
            Return String.Empty
        End Try

    End Function





    'Private Function Formatta_NumeroMVV_X_Agenda(ByVal numeroMVVSian As String) As String

    '    If String.IsNullOrEmpty(numeroMVVSian) Then
    '        Return String.Empty
    '    End If

    '    ' MVV-E/TV/288/1/2021
    '    Try
    '        Dim arrDati = numeroMVVSian.Split("/")
    '        Return String.Format("{0}/{1}/{2}", arrDati(0), arrDati(3), arrDati(4))
    '    Catch
    '        Return String.Empty
    '    End Try

    'End Function


End Class
