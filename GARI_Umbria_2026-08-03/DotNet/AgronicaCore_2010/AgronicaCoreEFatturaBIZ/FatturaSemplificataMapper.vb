Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUtility.Stringhe

Public Class FatturaSemplificataMapper : Implements IFatturaMapper

    Private ReadOnly _decodificheMapper As IDecodificheMapper
    Private ReadOnly _contabilitaHelper As AgronicaCoreContabHLP.Contabilita
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _logger As EFatturaLogger
    Private ReadOnly _debug As Boolean

    Public ReadOnly Property ModuloGenerazione As enum_Omni_Modulo_Generazione Implements IFatturaMapper.ModuloGenerazione

        Get
            Return _decodificheMapper.ModuloGenerazione()
        End Get

    End Property

    Public ReadOnly Property DecodificheMapper As IDecodificheMapper Implements IFatturaMapper.DecodificheMapper
        Get
            Return _decodificheMapper
        End Get
    End Property

    Public ReadOnly Property TipoImpresaGerarchia As enum_TipoImpresaGerarchia Implements IFatturaMapper.TipoImpresaGerarchia
        Get
            Return _decodificheMapper.TipoImpresaGerarchia
        End Get
    End Property

    Public Sub New(ByVal decodificheMapper As IDecodificheMapper,
                   ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtente As AgronicaCoreParametri,
                   ByVal logger As EFatturaLogger,
                   ByVal debug As Boolean)
        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _decodificheMapper = decodificheMapper
        _logger = logger
        _debug = debug
        _contabilitaHelper = New AgronicaCoreContabHLP.Contabilita()
    End Sub

    Public Sub Inizializza(ByVal piva As String, ByVal agendeXLavCod As List(Of AgendaXLavCod)) Implements IFatturaMapper.Inizializza
        _decodificheMapper.Inizializza(piva, agendeXLavCod, "")
    End Sub

    Public Function MappaFatturaAttiva(ByVal fatturaGias As FatturaGias,
                                       ByVal tipoFattura As FatturaElettronicaType,
                                       ByVal id_cod_cliente As Integer) As IFatturaElettronica Implements IFatturaMapper.MappaFatturaAttiva

        Try
            Return MapFatturaSemplificata(fatturaGias, id_cod_cliente)
        Catch ex As Exception
            Throw New GiasEntityToEFatturaMapException(ex.Message)
        End Try

    End Function
    Public Function RegimeFiscaleXSezionale(ByVal PIVA As String, ByVal sezionale As Integer?) As Integer Implements IFatturaMapper.RegimeFiscaleXSezionale

        Return _decodificheMapper.RegimeFiscaleXSezionale(PIVA, sezionale)

    End Function
    Public Function ContattoCodiciPerPIVA(ByVal PIVA As String, ByVal cod_Contatto As String) As List(Of Contatto_Codice) Implements IFatturaMapper.ContattoCodiciPerPIVA
        Return _decodificheMapper.ContattoCodiciPerPIVA(PIVA, cod_Contatto).ToList()
    End Function

    Private Function MapFatturaSemplificata(ByVal fatturaGias As FatturaGias, ByVal id_cod_cliente As Integer) As IFatturaElettronica

        Dim fatturaSemplificata = New Semplificata.FatturaElettronicaType()

        fatturaSemplificata.versione = fatturaGias.OttieniFormatoTrasmissione()
        fatturaSemplificata.FatturaElettronicaHeader = MapFatturaElettronicaHeader_Semplificata(fatturaGias)
        fatturaSemplificata.FatturaElettronicaBody = New Semplificata.FatturaElettronicaBodyType() _
            {MapFatturaElettronicaBody_Semplificata(fatturaGias, id_cod_cliente)}

        Return fatturaSemplificata

    End Function

    Private Function MapFatturaElettronicaHeader_Semplificata(ByVal fatGias) As Semplificata.FatturaElettronicaHeaderType

        Dim fatturaElettronicaHeader = New Semplificata.FatturaElettronicaHeaderType()

        ' obbligatorio
        fatturaElettronicaHeader.DatiTrasmissione = MapDatiTrasmissione_Semplificata(fatGias)
        'obbligatorio
        fatturaElettronicaHeader.CedentePrestatore = MapCedentePrestatore_Semplificata(fatGias)

        'il cedente / prestatore si configura come soggetto non residente che effettua nel
        'territorio dello stato italiano operazioni rilevanti ai fini IVA e che si avvale, in Italia,
        'di un rappresentante fiscale

        'fatturaElettronicaHeader.RappresentanteFiscale = Nothing

        fatturaElettronicaHeader.CessionarioCommittente = MapCessionarioCommittente_Semplificata(fatGias)

        'l 'impegno di emettere fattura elettronica per conto del cedente/prestatore è
        'assunto da un terzo sulla base di un accordo preventivo; il cedente/prestatore
        'rimane responsabile dell'adempimento fiscale
        'fatturaElettronicaHeader.TerzoIntermediarioOSoggettoEmittente = MapTerzoIntermediarioOSoggettoEmittente_PA(fatGias)

        ' se la fattura è emessa da un soggetto diverso dal cedente/ prestatore.
        'fatturaElettronicaHeader.SoggettoEmittente = Entita.Semplificata.SoggettoEmittenteType.CC

        Return fatturaElettronicaHeader

    End Function



    Private Function MapDatiTrasmissione_Semplificata(ByVal fatGias As FatturaGias) As Semplificata.DatiTrasmissioneType

        Dim mapped = New Semplificata.DatiTrasmissioneType()

        Dim codiceCUAA = fatGias.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.CodiceCUAA)
        Dim idCodice = If(String.IsNullOrEmpty(codiceCUAA), fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA, codiceCUAA)

        mapped.IdTrasmittente = New Semplificata.IdFiscaleType With
        {
            .IdCodice = idCodice,
            .IdPaese = "IT"
        }

        mapped.ProgressivoInvio = fatGias.ProgressivoFattura.ToString().PadLeft(10, "0")
        mapped.FormatoTrasmissione = fatGias.OttieniFormatoTrasmissione()
        mapped.CodiceDestinatario = fatGias.OttieniCodiceDestinatario()

        If mapped.CodiceDestinatario = "0000000" OrElse mapped.CodiceDestinatario = "999999" OrElse mapped.CodiceDestinatario = "XXXXXXX" Then
            Dim pecDestinatario = fatGias.Cessionario.DatiPrincipali.PEC
            If Not String.IsNullOrEmpty(pecDestinatario) Then
                mapped.PECDestinatario = pecDestinatario
            End If
        End If

        Return mapped

    End Function

    Private Function MapCedentePrestatore_Semplificata(ByVal fatGias As FatturaGias) As Semplificata.CedentePrestatoreType

        Dim mapped As New Semplificata.CedentePrestatoreType()

        Dim id_cf = fatGias.Cedente.DatiPrincipali.Anagrafica.Id_CF

        mapped.IdFiscaleIVA = New Semplificata.IdFiscaleType With
        {
             .IdPaese = "IT",
             .IdCodice = fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA
        }

        If fatGias.Cedente.TipoImpresa = enum_TipoImpresaGerarchia.DittaIndividuale AndAlso Not fatGias.LegaleRappresentante Is Nothing Then
            If Not String.IsNullOrEmpty(fatGias.LegaleRappresentante.Anagrafica.Cod_Contatto) Then
                mapped.CodiceFiscale = TroncaStringa(fatGias.LegaleRappresentante.Anagrafica.Cod_Contatto, 16)
            End If
        End If

        If _debug Then
            ' PIVA di Agronica Fissa 
            mapped.IdFiscaleIVA.IdCodice = "03487210407"
        End If

        If Not fatGias.Cedente.TipoImpresa = enum_TipoImpresaGerarchia.DittaIndividuale Then
            If id_cf = enum_Contatti_IdCf.PersonaFisica Then
                mapped.ItemsElementName = New Semplificata.ItemsChoiceType1() _
                    {
                        Semplificata.ItemsChoiceType1.Nome, Semplificata.ItemsChoiceType1.Cognome
                    }
                mapped.Items = New String() _
                    {
                        TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.Nome), 60),
                        TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.Cognome), 60)
                    }
            Else
                mapped.ItemsElementName = New Semplificata.ItemsChoiceType2() _
                   {
                       Semplificata.ItemsChoiceType1.Denominazione
                   }
                mapped.Items = New String() _
                   {
                       TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.rag_soc), 80)
                   }
            End If
        Else
            mapped.ItemsElementName = New Semplificata.ItemsChoiceType1() _
            {
                Semplificata.ItemsChoiceType1.Nome, Semplificata.ItemsChoiceType1.Cognome
            }
            mapped.Items = New String() _
                {
                    TroncaStringa(Trim(fatGias.LegaleRappresentante.Anagrafica.Nome), 60),
                    TroncaStringa(Trim(fatGias.LegaleRappresentante.Anagrafica.Cognome), 60)
                }
        End If

        mapped.RegimeFiscale = _decodificheMapper.DecodificaRegimeFiscale_Semplificata(fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA,
                                                                                  fatGias.Fattura.Testata.Sezionale_Cod,
                                                                                fatGias.Cedente.DatiPrincipali.Anagrafica.CodRegimeFiscale)

        Dim sede = New Semplificata.IndirizzoType With {
            .CAP = fatGias.Cedente.DatiPrincipali.Indirizzo.CAP,
            .Comune = TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaComune(id_cf, _decodificheMapper)), 60),
            .Indirizzo = TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Indirizzo.ind_des), 60),
            .Nazione = fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf),
            .Provincia = fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaProvincia(_decodificheMapper)
        }

        ' tale ha l'obbligo di indicare in tutti i documenti anche i dati relativi all’iscrizione 
        Dim iscrizioneRea = New Semplificata.IscrizioneREAType With
        {
            .Ufficio = fatGias.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.UfficioRea),
            .NumeroREA = fatGias.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.NumeroRea)
        }
        Dim capSoc As Decimal = fatGias.OttieniCapitaleSociale()
        If capSoc <> Decimal.MinValue Then
            iscrizioneRea.CapitaleSociale = capSoc
            iscrizioneRea.CapitaleSocialeSpecified = If(capSoc <> 0, True, False)
        End If

        Dim socioUnico = fatGias.OttieniSocioUnico()
        If socioUnico <> Int32.MinValue Then
            iscrizioneRea.SocioUnico = socioUnico
            iscrizioneRea.SocioUnicoSpecified = True
        End If

        Dim statoLiquidazione = fatGias.OttieniStatoLiquidazione()
        If statoLiquidazione <> Int32.MinValue Then
            iscrizioneRea.StatoLiquidazione = statoLiquidazione
        End If

        mapped.Sede = sede
        mapped.IscrizioneREA = iscrizioneRea

        Return mapped

    End Function

    Private Function MapCessionarioCommittente_Semplificata(ByVal fatGias As FatturaGias) As Semplificata.CessionarioCommittenteType

        Dim mapped As New Semplificata.CessionarioCommittenteType()

        Dim isAssociazione = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto.IsAssociazione()
        Dim id_cf = fatGias.Cessionario.DatiPrincipali.Anagrafica.Id_CF

        Dim denominazione = fatGias.Cessionario.DatiPrincipali.Anagrafica.rag_soc
        If String.IsNullOrEmpty(denominazione) Then
            denominazione = String.Concat(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cognome, " ", fatGias.Cessionario.DatiPrincipali.Anagrafica.Nome)
        End If

        Dim identificativiFiscali = New Semplificata.IdentificativiFiscaliType
        Dim altriDatiIdentificativi = New Semplificata.AltriDatiIdentificativiType

        Select Case id_cf

            Case enum_Contatti_IdCf.PersonaFisica

                ' PERSONE FISICHE >>>>>>>>>>>>>>>>>
                identificativiFiscali.CodiceFiscale = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto
                If UtilityProvider.VerificaEspressioneRegolare(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto, "", enum_EspressioniRegolari.RegExp_PartitaIVA) Then

                    ' PERSONE FISICHE CON PARTITA IVA (ASSOCIAZIONI O ENTI CON COD FISCALE UGUALE = PIVA) >>>>
                    altriDatiIdentificativi.ItemsElementName = New Semplificata.ItemsChoiceType2() _
                    {
                        Semplificata.ItemsChoiceType1.Denominazione
                    }
                    altriDatiIdentificativi.Items = New String() _
                    {
                        TroncaStringa(Trim(denominazione), 80)
                    }
                Else

                    ' PERSONA FISICA VERA A PROPRIA CON COD FISCALE LUNGO 16
                    altriDatiIdentificativi.ItemsElementName = New Semplificata.ItemsChoiceType1() _
                    {
                        Semplificata.ItemsChoiceType1.Nome, Semplificata.ItemsChoiceType1.Cognome
                    }
                    altriDatiIdentificativi.Items = New String() _
                    {
                        TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Anagrafica.Nome), 60),
                        TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cognome), 60)
                    }

                End If

            Case enum_Contatti_IdCf.PersonaGiuridica

                ' PERSONE GIURIDICHE
                altriDatiIdentificativi.ItemsElementName = New Semplificata.ItemsChoiceType2() _
                        {
                            Semplificata.ItemsChoiceType1.Denominazione
                        }
                altriDatiIdentificativi.Items = New String() _
                        {
                             TroncaStringa(Trim(denominazione), 80)
                        }

                If isAssociazione Then
                    ' PERSONA GIURIDICA MA ASSOCIAZIONE
                    identificativiFiscali.CodiceFiscale = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto
                Else
                    Dim piva = NormalizzaPIVA(fatGias.OttieniPivaCessionario(), fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf))
                    Dim codiceFiscale = TroncaStringa(fatGias.Cessionario.DatiPrincipali.Anagrafica.Codice_Fiscale.Trim(), 16)

                    ' PERSONA GIURIDICA ITALIANA
                    identificativiFiscali.IdFiscaleIVA = New Semplificata.IdFiscaleType With
                    {
                        .IdCodice = piva,
                        .IdPaese = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
                    }

                    ' PER GRUPPI IVA VA INDICATO ANCHE CODICE FISCALE
                    If Not String.IsNullOrEmpty(codiceFiscale) _
                        AndAlso piva.ToLower <> codiceFiscale Then
                        identificativiFiscali.CodiceFiscale = codiceFiscale
                    End If

                End If

            Case Else

                ' PERSONA GIURIDICA ESTERA
                altriDatiIdentificativi.ItemsElementName = New Semplificata.ItemsChoiceType2() _
                        {
                            Semplificata.ItemsChoiceType1.Denominazione
                        }
                altriDatiIdentificativi.Items = New String() _
                        {
                             TroncaStringa(Trim(denominazione), 80)
                        }

                identificativiFiscali.IdFiscaleIVA = New Semplificata.IdFiscaleType With
                {
                    .IdCodice = NormalizzaPIVA(fatGias.OttieniPivaCessionario(), fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)),
                    .IdPaese = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
                }

        End Select

        ' obbligatorio
        Dim sede = New Semplificata.IndirizzoType With {
            .CAP = fatGias.Cessionario.DatiPrincipali.Indirizzo.CAP,
            .Comune = TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaComune(id_cf, _decodificheMapper)), 60),
            .Indirizzo = TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Indirizzo.ind_des), 60),
            .Nazione = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
        }
        If id_cf <> enum_Contatti_IdCf.ContattoEstero Then
            sede.Provincia = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaProvincia(_decodificheMapper)
        End If
        altriDatiIdentificativi.Sede = sede

        ' il cessionario / committente è un soggetto che non risiede In Italia ma che, in
        'Italia, dispone di una stabile organizzazione attraverso la quale svolge la
        'propria attività oggetto di fatturazione
        If id_cf = enum_Contatti_IdCf.ContattoEstero AndAlso Not fatGias.Cessionario.StabileOrganizzazione Is Nothing Then

            Dim so = fatGias.Cessionario.StabileOrganizzazione

            If Not so Is Nothing Then
                Dim stabileOrg = New Semplificata.IndirizzoType With {
                   .CAP = so.Indirizzo.CAP,
                   .Comune = TroncaStringa(Trim(so.Indirizzo.com_des), 60),
                   .Indirizzo = TroncaStringa(Trim(so.Indirizzo.ind_des), 60),
                   .Nazione = so.Indirizzo.NormalizzaStato(id_cf),
                   .Provincia = so.Indirizzo.pro_cod
                }
                altriDatiIdentificativi.StabileOrganizzazione = stabileOrg
            End If

        End If

        ' Obbligatorio il cessionario / committentee si configura come soggetto non residente che effettua
        'nel territorio dello stato italiano operazioni rilevanti ai fini IVA e che si avvale, in
        'Italia, di un rappresentante fiscale
        If id_cf = enum_Contatti_IdCf.ContattoEstero Then

            Dim rfRisUm = fatGias.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.RappresentanteFiscale)
            If Not String.IsNullOrEmpty(rfRisUm) Then

                Dim rf = _decodificheMapper.OttieniRappresentanteFiscale(CInt(rfRisUm))
                If Not rf Is Nothing Then
                    Dim rappresentanteFiscale = New Semplificata.RappresentanteFiscaleType With {
                        .IdFiscaleIVA = New Semplificata.IdFiscaleType With
                    {
                        .IdCodice = rf.Cod_Contatto,
                        .IdPaese = "IT"
                    },
                        .ItemsElementName = New Semplificata.ItemsChoiceType2() _
                       {
                           Semplificata.ItemsChoiceType1.Denominazione
                       },
                        .Items = New String() _
                       {
                           TroncaStringa(Trim(rf.Rag_Soc), 80)
                       }
                    }
                    altriDatiIdentificativi.RappresentanteFiscale = rappresentanteFiscale
                End If

            End If

        End If

        mapped.IdentificativiFiscali = identificativiFiscali
        mapped.AltriDatiIdentificativi = altriDatiIdentificativi

        Return mapped

    End Function

    Private Function MapFatturaElettronicaBody_Semplificata(ByVal fatGias As FatturaGias, ByVal id_cod_cliente As Integer) As Semplificata.FatturaElettronicaBodyType

        Dim body = New Semplificata.FatturaElettronicaBodyType()
        Dim datiBeniServizi = New List(Of Semplificata.DatiBeniServiziType)

        body.DatiGenerali = MapBodyDatiGenerali(fatGias)
        body.DatiBeniServizi = MapBodyDatiBeniServizi(fatGias).ToArray()

        Dim filePdf As Byte() = Nothing

        Try
            filePdf = Stampa(id_cod_cliente,
                             fatGias.Fattura.PIVA,
                             fatGias.Fattura.Testata.Id_Agenda,
                             fatGias.Fattura.Testata.Lav_cod,
                             _objParametriServer, _objParametriUtente, _objParametriSuperServer)
        Catch ex As Exception
            _logger.Logga("FatturaMapper.MapFatturaElettronicaBody_PA (Stampa)", ex)
        End Try

        If Not filePdf Is Nothing Then
            body.Allegati = New Semplificata.AllegatiType() {New Semplificata.AllegatiType With
                {
                    .FormatoAttachment = "pdf",
                    .Attachment = filePdf,
                    .NomeAttachment = GetNomeAllegato(fatGias)
                }
            }
        End If

        Return body

    End Function

    Private Function Stampa(ByVal progressivoGias As Integer,
                            ByVal piva As String,
                            ByVal idAgenda As Integer,
                            ByVal lavCod As Integer,
                            ByRef objParametriServer As AgronicaCoreParametri,
                            ByRef objParametriUtenti As AgronicaCoreParametri,
                            ByRef objParametriSuperServer As AgronicaCoreParametri
                            ) As Byte()

        Dim myRes As Net.WebResponse = Nothing
        Dim myReq As Net.HttpWebRequest
        Dim mySourceStream As Stream = Nothing
        'Memory stream to store data  
        Dim myTempStream As MemoryStream = Nothing

        Try

            Dim report As enum_CodificaStampe

            Select Case lavCod

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    report = enum_CodificaStampe.DDT_Contabilizzato_Emesso

                Case LAVCOD_BOLLA_EMESSA
                    report = enum_CodificaStampe.Bolle

                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                    report = enum_CodificaStampe.Fatture

                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    report = enum_CodificaStampe.Nota_Accredito

                Case LAVCOD_RICEVUTA_EMESSA
                    report = enum_CodificaStampe.RicevuteFiscali

                Case LAVCOD_DOCO_EMESSO
                    report = enum_CodificaStampe.DOCO

                Case LAVCOD_DAA_EMESSO
                    report = enum_CodificaStampe.DAA

                Case LAVCOD_CONFERIMENTO
                    report = enum_CodificaStampe.Bolle_Conferimento_Soci

                Case LAVCOD_CONFERIMENTO_DIVERSI
                    report = enum_CodificaStampe.Bolle_Conferimento_Diversi

                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    report = enum_CodificaStampe.Buono_Accettazione_Diversi

                Case Else
                    Throw New Exception("Tipo di stampa non disponibile.")

            End Select

            Dim objStampeUtility As New AgronicaCoreStampeDAL.Utility
            Dim url As String = objStampeUtility.GetUrlStampaDocumento(piva, idAgenda,
                                                                       lavCod, report,
                                                                       progressivoGias,
                                                                       objParametriSuperServer,
                                                                       objParametriServer,
                                                                       objParametriUtenti,
                                                                       Enum_SiteRedirector.AgroGSB)

            myReq = Net.WebRequest.Create(url)
            myReq.AllowAutoRedirect = True
            myReq.KeepAlive = True
            myReq.CookieContainer = New Net.CookieContainer()

            myRes = myReq.GetResponse()

            'Solo se mi è tornato il pdf, altrimenti vuol dire che c'è stato qualche errore
            If Not myRes Is Nothing AndAlso myRes.ContentType = "application/pdf" Then

                'Source stream with requested document  
                mySourceStream = myRes.GetResponseStream()

                'SourceStream has no ReadAll, so we must read data block-by-block  
                'Temporary Buffer and block size  
                Dim buffer(4096) As Byte, blockSize As Integer
                myTempStream = New MemoryStream

                Do
                    blockSize = mySourceStream.Read(buffer, 0, 4096)
                    If blockSize > 0 Then
                        myTempStream.Write(buffer, 0, blockSize)
                    End If
                Loop While blockSize > 0

            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            If Not IsNothing(mySourceStream) Then
                mySourceStream.Close()
            End If

            If Not IsNothing(myRes) Then
                myRes.Close()
            End If
        End Try

        If Not myTempStream Is Nothing Then
            'return the document binary data  
            Return myTempStream.ToArray()
        Else
            Return Nothing
        End If

    End Function

    Private Function GetNomeAllegato(ByVal fatGias As FatturaGias) As String

        Dim tipoDoc As String = If(fatGias.Fattura.Testata.Lav_cod = LAVCOD_NOTA_ACCREDITO_EMESSA, "NotaAccredito", "Fattura")

        Dim ragSoc As String = fatGias.Cessionario.DatiPrincipali.Anagrafica.rag_soc

        If String.IsNullOrEmpty(ragSoc) Then
            ragSoc = String.Concat(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cognome,
                                   " ",
                                   fatGias.Cessionario.DatiPrincipali.Anagrafica.Nome)
        End If

        Dim nomeFile As String = tipoDoc & "_" &
                                 Format(fatGias.Fattura.Testata.Data_Movimento, "yyyy-MM-dd") & "_" &
                                 "n" & fatGias.Fattura.NumeroFattura & "_" &
                                 Replace(ragSoc, ".", "")

        Return TroncaStringa(Trim(EliminaCaratteriSpecialiFile(nomeFile)), 56) & ".pdf"
    End Function

    Private Function MapBodyDatiGenerali(ByVal fatGias As FatturaGias) As Semplificata.DatiGeneraliType

        Dim mapped As New Semplificata.DatiGeneraliType

        Dim datiGeneraliDocumento = New Semplificata.DatiGeneraliDocumentoType With
        {
            .TipoDocumento = fatGias.OttieniTipoDocumento_Semplificata(),
            .Divisa = "EUR",
            .Data = fatGias.Fattura.Testata.Data_Movimento,
            .Numero = fatGias.OttieniNumeroFattura()
        }

        mapped.DatiGeneraliDocumento = datiGeneraliDocumento

        ' DDT Collegati
        'TODO
        'If fatGias.Fattura.TipoFattura = "TD04" Then

        '    ' Nota di credito
        '    If Not fatGias.RifNotaCreditoDemito Is Nothing Then
        '        Dim ddts = New List(Of Semplificata.DatiDDTType)
        '        Dim ddt = New Semplificata.DatiDDTType With {
        '            .NumeroDDT = fatGias.RifNotaCreditoDemito.N_Nota_DDT,
        '            .DataDDT = fatGias.RifNotaCreditoDemito.Data_Nota_DDT
        '        }
        '        ddts.Add(ddt)
        '        mapped.DatiDDT = ddts.ToArray()

        '    End If

        'Else
        '    Dim docCollegati = fatGias.DocumentiCollegati.Documenti
        '    Dim agendeCollegate = docCollegati.Select(Function(a) a.Id_Agenda_Rif).Distinct().ToList()
        '    If Not docCollegati Is Nothing AndAlso docCollegati.Any() Then

        '        Dim ddts = New List(Of Semplificata.DatiDDTType)
        '        agendeCollegate.ForEach(Sub(agenda)

        '                                    Dim dc = docCollegati.FirstOrDefault(Function(d) d.Id_Agenda_Rif = agenda)
        '                                    Dim ddt = New Semplificata.DatiDDTType With
        '                                {
        '                                   .DataDDT = dc.Data_Movimento,
        '                                   .NumeroDDT = dc.OttieniNumeroFattura()
        '                                }

        '                                    Dim ddtProdCount = fatGias.DocumentiCollegati.Riepilogo.FirstOrDefault(Function(r) r.Id_Agenda = agenda)
        '                                    If Not ddtProdCount Is Nothing Then
        '                                        Dim ddtProdInFattura = docCollegati.Where(Function(d) d.Id_Agenda_Rif = agenda)
        '                                        If ddtProdInFattura.Count() < ddtProdCount.NumMovimenti Then

        '                                            Dim rifLinee = ddtProdInFattura.Select(Function(p) p.Ordine_Det.ToString()).OrderBy(Function(p) p)
        '                                            ddt.RiferimentoNumeroLinea = rifLinee.ToArray()
        '                                        End If
        '                                    End If

        '                                    ddts.Add(ddt)
        '                                End Sub)
        '        mapped.DatiDDT = ddts.ToArray()
        '    End If

        'End If

        Return mapped

    End Function

    Private Function MapBodyDatiBeniServizi(ByVal fatGias As FatturaGias) As List(Of Semplificata.DatiBeniServiziType)

        Dim mapped As New List(Of Semplificata.DatiBeniServiziType)
        Dim prgRiga As Integer = 1
        Dim ivaDefault As Integer = IvaDefaultPerRigheDescrittive(fatGias.Fattura.Dettagli.ProdottiServizi)

        'For Each prod In fatGias.Fattura.Dettagli.ProdottiServizi

        '    Dim det = prod.Movimento

        '    If det.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA Then
        '        dettagli.Add(DettaglioLineaNormale(prgRiga, prod, fatGias))
        '    Else
        '        prod.Movimento.Cod_Iva = ivaDefault
        '        dettagli.Add(DettaglioLineaDescrizioneLibera(prgRiga, prod, fatGias))
        '    End If

        '    prgRiga = prgRiga + 1

        'Next


        'Dim prodottiServiziReali = fatGias.Fattura.Dettagli.ProdottiServizi.Where(Function(d)
        '                                                                              Return d.Movimento.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA
        '                                                                          End Function)

        '' Righe aggiuntive storno per omaggi con senza rivalsa iva
        'Dim righeAggiuntive = prodottiServiziReali.Where(Function(d)
        '                                                     Return d.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva Or
        '                                                            d.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_SenzaRivalsaIva
        '                                                 End Function)

        'For Each prod In righeAggiuntive

        '    Dim det = prod.Movimento

        '    Dim unitaMisura = TroncaStringa(Trim(_decodificheMapper.DecodificaUnitaMisura(det.Udm_Cod)), 10)

        '    Dim livelloPrezzo = prod.LivellaPrezzo(unitaMisura, _decodificheMapper.ModuloGenerazione, _decodificheMapper)
        '    Dim prezzoTotale As Decimal = det.OttieniPrezzoTotaleDettaglio(fatGias.Fattura.Testata.Lav_cod, _contabilitaHelper)
        '    Dim alIva As Decimal = _decodificheMapper.DecodificaAliqotaIva(1).NormalizzaImporto(2)
        '    Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(1)

        '    Dim valoreIva = _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod, det.Iva)
        '    Dim importoLinea = If(det.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva, prezzoTotale * -1, (prezzoTotale + valoreIva) * -1)
        '    Dim descrizione = "Storno valori omaggi {0} rivalsa IVA"
        '    Dim dettaglioLinea = New Semplificata.DettaglioLineeType() With
        '    {
        '        .NumeroLinea = prgRiga,
        '        .descrizione = String.Format(descrizione, If(det.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva, "con", "senza")),
        '        .PrezzoUnitario = NormalizzaImporto(importoLinea, 8),
        '        .prezzoTotale = NormalizzaImporto(importoLinea, 8),
        '        .AliquotaIVA = alIva
        '    }
        '    If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
        '        dettaglioLinea.Natura = natura
        '        dettaglioLinea.NaturaSpecified = True
        '    End If

        '    dettagli.Add(dettaglioLinea)
        '    prgRiga = prgRiga + 1

        'Next

        'mapped.DettaglioLinee = dettagli.ToArray()

        'Dim riepilogo = New List(Of Semplificata.DatiRiepilogoType)

        '' Castelletto iva senza righe omaggi con e senza rivalsa iva
        'Dim movDet = fatGias.Fattura.Dettagli.ProdottiServizi.Select(Function(ps) ps.Movimento).ToList()
        'riepilogo.AddRange(CastellettoIva(movDet, fatGias))


        '' Castelletto iva per omaggi con rivalsa Iva
        'movDet = prodottiServiziReali.Where(Function(p) p.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva).
        '                                                                Select(Function(ps) ps.Movimento).ToList()
        'riepilogo.AddRange(CastellettoIvaPerOmaggi(True, movDet, fatGias))

        '' Castelletto iva per omaggi senza rivalsa Iva
        'movDet = prodottiServiziReali.Where(Function(p) p.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_SenzaRivalsaIva).
        '                                                                Select(Function(ps) ps.Movimento).ToList()
        'riepilogo.AddRange(CastellettoIvaPerOmaggi(False, movDet, fatGias))

        'mapped.DatiRiepilogo = riepilogo.ToArray()


        Return mapped

    End Function

    Private Function IvaDefaultPerRigheDescrittive(ByVal prodottiServizi As List(Of ProdottoServizioMap)) As Integer

        Dim prodottiReali = prodottiServizi.Where(Function(p) p.Movimento.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA).Select(Function(p) p.Movimento).ToList()
        Dim primoValido = prodottiReali.Where(Function(m) m.Cod_Iva > 0).FirstOrDefault()
        If Not primoValido Is Nothing Then
            Return primoValido.Cod_Iva
        Else
            ' Fuori Campo IVA
            Return 1
        End If

    End Function

    'Private Function DettaglioLineaNormale(ByVal prgRiga As Integer,
    '                                ByVal prod As ProdottoServizioMap,
    '                                ByVal fatGias As FatturaGias
    '                                ) As Semplificata.DettaglioLineeType

    '    Dim det = prod.Movimento

    '    ' decodifica udm xche serve nel livello prezzo
    '    Dim unitaMisura = TroncaStringa(Trim(_decodificheMapper.DecodificaUnitaMisura(det.Udm_Cod)), 10)

    '    Dim livelloPrezzo = prod.LivellaPrezzo(unitaMisura, _decodificheMapper.ModuloGenerazione, _decodificheMapper)
    '    Dim prezzoTotale As Decimal = det.OttieniPrezzoTotaleDettaglio(fatGias.Fattura.Testata.Lav_cod, _contabilitaHelper)
    '    Dim alIva As Decimal = _decodificheMapper.DecodificaAliqotaIva(det.Cod_Iva).NormalizzaImporto(2)
    '    Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(det.Cod_Iva)

    '    Dim dettaglioLinea = New Semplificata.DettaglioLineeType() With
    '            {
    '                .NumeroLinea = prgRiga,
    '                .Descrizione = TroncaStringa(Trim(prod.Descrizione), 1000),
    '                .Quantita = livelloPrezzo.Qta,
    '                .QuantitaSpecified = True,
    '                .unitaMisura = TroncaStringa(Trim(livelloPrezzo.UDM), 10),
    '                .PrezzoUnitario = livelloPrezzo.Prezzo_Unitario,
    '                .prezzoTotale = prezzoTotale,
    '                .AliquotaIVA = alIva
    '            }
    '    If det.Sconto_Modalita <> enModalitaSconto.Percentuale Then
    '        dettaglioLinea.TipoCessionePrestazione = Semplificata.TipoCessionePrestazioneType.AB
    '        dettaglioLinea.TipoCessionePrestazioneSpecified = True

    '        Dim rifTesto = String.Empty
    '        Select Case det.Sconto_Modalita
    '            Case enModalitaSconto.Omaggio_ConRivalsaIva
    '                rifTesto = "Omaggio con rivalsa #OC#"
    '            Case enModalitaSconto.Omaggio_SenzaRivalsaIva
    '                rifTesto = "Omaggio senza rivalsa #OS#"
    '            Case enModalitaSconto.Campioni_Gratuiti
    '                rifTesto = "Campioni gratuiti"
    '            Case enModalitaSconto.Sconto_Merce
    '                rifTesto = "Sconto merce #SM#"
    '        End Select

    '        Dim adg = New List(Of Semplificata.AltriDatiGestionaliType)
    '        adg.Add(New Semplificata.AltriDatiGestionaliType With
    '                     {
    '                        .TipoDato = "AswTRiga",
    '                        .RiferimentoTesto = rifTesto
    '                     })
    '        dettaglioLinea.AltriDatiGestionali = adg.ToArray()
    '    End If

    '    If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
    '        dettaglioLinea.Natura = natura
    '        dettaglioLinea.NaturaSpecified = True
    '    End If

    '    ' Sconti
    '    'se si tratta di omaggio con rivalsa Iva o senza rivalsa iva non scrivo il nodo dello sconto
    '    If det.Sconto_Modalita <> enModalitaSconto.Omaggio_ConRivalsaIva AndAlso det.Sconto_Modalita <> enModalitaSconto.Omaggio_SenzaRivalsaIva Then
    '        If det.Sconto.HasValue AndAlso det.Sconto.Value <> CDbl(0) OrElse Not String.IsNullOrEmpty(det.Sconto_Testo) Then

    '            Dim scontiMaggiorazioni = New List(Of Semplificata.ScontoMaggiorazioneType)

    '            Dim sconti = New List(Of Double) From {
    '                    det.Sconto * -1
    '                }
    '            If Not String.IsNullOrEmpty(det.Sconto_Testo) Then
    '                sconti.AddRange(det.Sconto_Testo.Split("-").ToList().ConvertAll(Function(s) CDbl(s)))
    '            End If

    '            sconti.ForEach(Sub(s)
    '                               Dim sm = New Semplificata.ScontoMaggiorazioneType With
    '                                   {
    '                                        .Tipo = Semplificata.TipoScontoMaggiorazioneType.SC,
    '                                        .Percentuale = NormalizzaImporto(s, 4),
    '                                        .PercentualeSpecified = True
    '                                   }
    '                               scontiMaggiorazioni.Add(sm)
    '                           End Sub)

    '            dettaglioLinea.ScontoMaggiorazione = scontiMaggiorazioni.ToArray()
    '        End If
    '    End If

    '    Return dettaglioLinea

    'End Function

    'Private Function DettaglioLineaDescrizioneLibera(ByVal prgRiga As Integer,
    '                               ByVal prod As ProdottoServizioMap,
    '                               ByVal fatGias As FatturaGias
    '                               ) As Semplificata.DettaglioLineeType

    '    Dim det = prod.Movimento
    '    Dim alIva As Decimal = _decodificheMapper.DecodificaAliqotaIva(det.Cod_Iva).NormalizzaImporto(2)
    '    Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(det.Cod_Iva)

    '    Dim dettaglioLinea = New Semplificata.DettaglioLineeType() With
    '            {
    '                .NumeroLinea = prgRiga,
    '                .Descrizione = TroncaStringa(Trim(prod.Descrizione), 1000),
    '                .PrezzoUnitario = NormalizzaImporto(0, 8),
    '                .PrezzoTotale = NormalizzaImporto(0, 8),
    '                .AliquotaIVA = alIva
    '            }

    '    If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
    '        dettaglioLinea.Natura = natura
    '        dettaglioLinea.NaturaSpecified = True
    '    End If

    '    Dim adg = New List(Of Semplificata.AltriDatiGestionaliType)
    '    adg.Add(New Semplificata.AltriDatiGestionaliType With
    '                     {
    '                        .TipoDato = "AswTRiga",
    '                        .RiferimentoTesto = "Descrittivo #DE#"
    '                     })
    '    dettaglioLinea.AltriDatiGestionali = adg.ToArray()
    '    Return dettaglioLinea

    'End Function
    'Private Function CastellettoIva(ByVal movDet As List(Of Movimenti_dettagli),
    '                                        ByVal fatGias As FatturaGias) As List(Of Semplificata.DatiRiepilogoType)

    '    Dim riepilogo = New List(Of Semplificata.DatiRiepilogoType)

    '    Dim dettagliRaggrupati = (From det In movDet
    '                              Order By det.Cod_Iva
    '                              Group By CODIVA = det.Cod_Iva
    '                              Into movimenti = Group, Count()
    '                              Order By CODIVA).ToList()

    '    Dim lavCod = fatGias.Fattura.Testata.Lav_cod

    '    For Each iva In dettagliRaggrupati

    '        Dim riepilogoIva = New Semplificata.DatiRiepilogoType With
    '        {
    '            .AliquotaIVA = _decodificheMapper.DecodificaAliqotaIva(iva.CODIVA).NormalizzaImporto(2),
    '            .ImponibileImporto = NormalizzaImporto(iva.movimenti.Sum(Function(m) _contabilitaHelper.Leggi_Imponibile_PositivoNegativo(lavCod, m.Imponibile_Netto)), 2),
    '            .Imposta = NormalizzaImporto(
    '                _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod,
    '                                                              (iva.movimenti.Sum(Function(m) m.Iva))), 2)
    '        }
    '        If riepilogoIva.AliquotaIVA = 0 Then
    '            Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(iva.CODIVA)
    '            If natura <> Int32.MinValue Then
    '                riepilogoIva.Natura = natura
    '                riepilogoIva.NaturaSpecified = True
    '                'se specificato il campo natura ci va riferimento normativo
    '                riepilogoIva.RiferimentoNormativo = _decodificheMapper.DecodificaRiferimentoNormativo(iva.CODIVA)
    '            End If
    '        Else
    '            Dim codSez = fatGias.Fattura.Testata.Sezionale_Cod
    '            Dim esigibilitàIva = _decodificheMapper.DecodificaEsigibilitaIVA(codSez)
    '            If esigibilitàIva <> Int32.MinValue Then
    '                riepilogoIva.EsigibilitaIVA = esigibilitàIva
    '                riepilogoIva.EsigibilitaIVASpecified = True
    '            End If

    '        End If

    '        riepilogo.Add(riepilogoIva)
    '    Next

    '    Return riepilogo

    'End Function

    'Private Function CastellettoIvaPerOmaggi(
    '                                        ByVal conRivalsaIva As Boolean,
    '                                        ByVal movDet As List(Of Movimenti_dettagli),
    '                                        ByVal fatGias As FatturaGias) As List(Of Semplificata.DatiRiepilogoType)

    '    Dim riepilogo = New List(Of Semplificata.DatiRiepilogoType)

    '    Dim dettagliRaggrupati = (From det In movDet
    '                              Order By det.Cod_Iva
    '                              Group By CODIVA = det.Cod_Iva
    '                              Into movimenti = Group, Count()
    '                              Order By CODIVA).ToList()

    '    For Each iva In dettagliRaggrupati

    '        Dim prezzoTotale = iva.movimenti.Sum(Function(m) _contabilitaHelper.Leggi_Imponibile_PositivoNegativo(fatGias.Fattura.Testata.Lav_cod, m.Imponibile_Netto))
    '        Dim importoIva = _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod,
    '                                                              (iva.movimenti.Sum(Function(m) m.Iva)))
    '        Dim importoLinea = If(conRivalsaIva, prezzoTotale * -1, (prezzoTotale + importoIva) * -1)

    '        Dim riepilogoIva = New Semplificata.DatiRiepilogoType With
    '        {
    '            .AliquotaIVA = _decodificheMapper.DecodificaAliqotaIva(1).NormalizzaImporto(2),
    '            .ImponibileImporto = NormalizzaImporto(importoLinea, 2),
    '            .Imposta = NormalizzaImporto(0, 2)
    '        }
    '        If riepilogoIva.AliquotaIVA = 0 Then
    '            Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(1)
    '            If natura <> Int32.MinValue Then
    '                riepilogoIva.Natura = natura
    '                riepilogoIva.NaturaSpecified = True
    '                'se specificato il campo natura ci va riferimento normativo
    '                riepilogoIva.RiferimentoNormativo = _decodificheMapper.DecodificaRiferimentoNormativo(1)
    '            End If
    '        End If

    '        riepilogo.Add(riepilogoIva)
    '    Next

    '    Return riepilogo

    'End Function

End Class

