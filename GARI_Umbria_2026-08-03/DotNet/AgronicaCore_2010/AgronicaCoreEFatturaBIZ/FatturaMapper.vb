Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaBIZ.Entita.FatturaPa
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUtility.Stringhe

Public Class FatturaMapper : Implements IFatturaMapper

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
                                       ByVal tipoFattura As AgronicaCoreEFatturaDAL.FatturaElettronicaType,
                                       ByVal id_cod_cliente As Integer) As IFatturaElettronica Implements IFatturaMapper.MappaFatturaAttiva

        Try
            Return MapFatturaPa(fatturaGias, id_cod_cliente)
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

    Private Function MapFatturaPa(ByVal fatturaGias As FatturaGias, ByVal id_cod_cliente As Integer) As IFatturaElettronica

        Dim fatturaPa = New FatturaPa.FatturaElettronicaType()

        fatturaPa.versione = fatturaGias.OttieniFormatoTrasmissione()
        fatturaPa.FatturaElettronicaHeader = MapFatturaElettronicaHeader_PA(fatturaGias)
        fatturaPa.FatturaElettronicaBody = New FatturaPa.FatturaElettronicaBodyType() _
            {MapFatturaElettronicaBody_PA(fatturaGias, id_cod_cliente)}

        Return fatturaPa

    End Function
    Private Function MapFatturaSemplificata(ByVal fatturaGias) As IFatturaElettronica

        Return New Semplificata.FatturaElettronicaType()

    End Function

    Private Function MapFatturaElettronicaHeader_PA(ByVal fatGias As FatturaGias) As FatturaPa.FatturaElettronicaHeaderType

        Dim fatturaElettronicaHeader As New FatturaPa.FatturaElettronicaHeaderType()

        ' obbligatorio
        fatturaElettronicaHeader.DatiTrasmissione = MapDatiTrasmissione_PA(fatGias)
        'obbligatorio
        fatturaElettronicaHeader.CedentePrestatore = MapCedentePrestatore_PA(fatGias)

        'il cedente / prestatore si configura come soggetto non residente che effettua nel
        'territorio dello stato italiano operazioni rilevanti ai fini IVA e che si avvale, in Italia,
        'di un rappresentante fiscale

        'fatturaElettronicaHeader.RappresentanteFiscale = Nothing

        fatturaElettronicaHeader.CessionarioCommittente = MapCessionarioCommittente_PA(fatGias, fatturaElettronicaHeader.DatiTrasmissione)

        Select Case fatGias.Fattura.TipoFattura
            'Integrazioni per fatture con reverse Charge
            Case TipoDocumentoType.TD16.ToString(),
                TipoDocumentoType.TD17.ToString(), TipoDocumentoType.TD18.ToString(), TipoDocumentoType.TD19.ToString(),
                TipoDocumentoType.TD20.ToString(), TipoDocumentoType.TD28.ToString, TipoDocumentoType.TD29.ToString

                InversioneCedenteCessionario(fatturaElettronicaHeader)

                ' se la fattura è emessa da un soggetto diverso dal cedente/ prestatore.
                fatturaElettronicaHeader.SoggettoEmittente = SoggettoEmittenteType.CC
                fatturaElettronicaHeader.SoggettoEmittenteSpecified = True

        End Select


        'l 'impegno di emettere fattura elettronica per conto del cedente/prestatore è
        'assunto da un terzo sulla base di un accordo preventivo; il cedente/prestatore
        'rimane responsabile dell'adempimento fiscale
        'fatturaElettronicaHeader.TerzoIntermediarioOSoggettoEmittente = MapTerzoIntermediarioOSoggettoEmittente_PA(fatGias)

        ' se la fattura è emessa da un soggetto diverso dal cedente/ prestatore.
        'fatturaElettronicaHeader.SoggettoEmittente = SoggettoEmittenteType.CC

        Return fatturaElettronicaHeader

    End Function
    
    Private Sub InversioneCedenteCessionario(ByRef fatturaElettronicaHeader As FatturaPa.FatturaElettronicaHeaderType)

        Dim nuovoCedente As New FatturaPa.CedentePrestatoreType With {
            .DatiAnagrafici = New DatiAnagraficiCedenteType() With {
                .IdFiscaleIVA = fatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.IdFiscaleIVA,
                .CodiceFiscale = fatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.CodiceFiscale,
                .Anagrafica = fatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica,
                .RegimeFiscale = RegimeFiscaleType.RF18
            },
            .Sede = fatturaElettronicaHeader.CessionarioCommittente.Sede,
            .StabileOrganizzazione = fatturaElettronicaHeader.CessionarioCommittente.StabileOrganizzazione
        }
       
        Dim nuovoCessionario As New FatturaPa.CessionarioCommittenteType With {
            .DatiAnagrafici = New DatiAnagraficiCessionarioType() With {
                .IdFiscaleIVA = fatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA,
                .CodiceFiscale = fatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.CodiceFiscale,
                .Anagrafica = fatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica
            },
            .Sede = fatturaElettronicaHeader.CedentePrestatore.Sede,
            .StabileOrganizzazione = fatturaElettronicaHeader.CedentePrestatore.StabileOrganizzazione
        }


        fatturaElettronicaHeader.CedentePrestatore = nuovoCedente
        fatturaElettronicaHeader.CessionarioCommittente = nuovoCessionario

        'Va cambiato anche il codice sdi
        'TODO: usare il codice reale "T04ZHR3" oppure "0000000"?!?
        fatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario = "0000000"
        'fatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario = "T04ZHR3"
        fatturaElettronicaHeader.DatiTrasmissione.PECDestinatario = Nothing

    End Sub

    Private Function MapDatiTrasmissione_PA(ByVal fatGias As FatturaGias) As FatturaPa.DatiTrasmissioneType

        Dim mapped = New FatturaPa.DatiTrasmissioneType()

        Dim codiceCUAA = fatGias.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.CodiceCUAA)
        Dim idCodice = If(String.IsNullOrEmpty(codiceCUAA), fatGias.Cedente.DatiPrincipali.Anagrafica.PivaReale, codiceCUAA)

        mapped.IdTrasmittente = New FatturaPa.IdFiscaleType With
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

    Private Function MapCedentePrestatore_PA(ByVal fatGias As FatturaGias) As FatturaPa.CedentePrestatoreType

        Dim mapped As New FatturaPa.CedentePrestatoreType()

        Dim id_cf = fatGias.Cedente.DatiPrincipali.Anagrafica.Id_CF

        Dim datiAnagrafici As New FatturaPa.DatiAnagraficiCedenteType()
        datiAnagrafici.IdFiscaleIVA = New FatturaPa.IdFiscaleType With
        {
             .IdPaese = "IT",
             .IdCodice = fatGias.Cedente.DatiPrincipali.Anagrafica.PivaReale
        }

        If fatGias.Cedente.TipoImpresa = enum_TipoImpresaGerarchia.DittaIndividuale AndAlso fatGias.LegaleRappresentante IsNot Nothing Then
            If Not String.IsNullOrEmpty(fatGias.LegaleRappresentante.Anagrafica.Cod_Contatto) Then
                datiAnagrafici.CodiceFiscale = TroncaStringa(fatGias.LegaleRappresentante.Anagrafica.Cod_Contatto, 16)
            End If
        End If

        If _debug Then
            ' PIVA di Agronica Fissa 
            datiAnagrafici.IdFiscaleIVA.IdCodice = "03487210407"
        End If

        'Campi non obbligatori commentati
        'datiAnagrafici.CodiceFiscale = "PIVA O CF se persona fisica o giuridica"    ' consigliato ma non obbligatorio
        'datiAnagrafici.AlboProfessionale = "Ing / Dott"                             ' non obbligatorio
        'datiAnagrafici.ProvinciaAlbo = "FC"                                         ' non obbligatorio
        'datiAnagrafici.DataIscrizioneAlbo = DateTime.Now                            ' non obbligatorio
        'datiAnagrafici.NumeroIscrizioneAlbo = ""                                    ' non obbligatorio


        Dim anagrafica As FatturaPa.AnagraficaType = Nothing

        If fatGias.Cedente.TipoImpresa <> enum_TipoImpresaGerarchia.DittaIndividuale Then
            anagrafica = AnagraficaCedenteNonDittaIndividuale(id_cf, fatGias)
        Else
            anagrafica = AnagraficaCedenteDittaIndividuale(fatGias)
        End If

        datiAnagrafici.Anagrafica = anagrafica
        datiAnagrafici.RegimeFiscale = _decodificheMapper.DecodificaRegimeFiscale(fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA,
                                                                                  fatGias.Fattura.Testata.Sezionale_Cod,
                                                                                fatGias.Cedente.DatiPrincipali.Anagrafica.CodRegimeFiscale)

        Dim sede = New FatturaPa.IndirizzoType With {
            .CAP = fatGias.Cedente.DatiPrincipali.Indirizzo.CAP,
            .Comune = TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaComune(id_cf, _decodificheMapper)), 60),
            .Indirizzo = TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Indirizzo.ind_des), 60),
            .Nazione = fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf),
            .Provincia = fatGias.Cedente.DatiPrincipali.Indirizzo.NormalizzaProvincia(_decodificheMapper)
        }

        ' Da compilare se il cedente / prestatore è un soggetto che non risiede In Italia ma che, in Italia,
        'dispone di una stabile organizzazione attraverso la quale svolge la propria
        'attività(cessioni di beni o prestazioni di servizi oggetto di fatturazione)
        'If fatGias.Cedente.DatiPrincipali.Id_CF = enum_Contatti_IdCf.ContattoEstero Then

        '    Dim stabileOrg = New FatturaPa.IndirizzoType With {
        '    .CAP = "",
        '    .Comune = "",
        '    .Indirizzo = "",
        '    .Nazione = "",
        '    .NumeroCivico = "",
        '    .Provincia = ""
        '    }
        '    mapped.StabileOrganizzazione = stabileOrg
        'End If

        ' Obbligatorio se il cedente / prestatore è una società iscritta nel registro delle imprese e come
        ' tale ha l'obbligo di indicare in tutti i documenti anche i dati relativi all’iscrizione 
        Dim iscrizioneRea = New FatturaPa.IscrizioneREAType With
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

        'Contatti (Non obbligatorio)
        'mapped.Contatti = New FatturaPa.ContattiType With
        '{
        '    .Email = "",
        '    .Fax = "",
        '    .Telefono = ""
        '}



        ' consigliato ma non obbligatorio
        'mapped.RiferimentoAmministrazione = ""

        mapped.DatiAnagrafici = datiAnagrafici
        mapped.Sede = sede
        mapped.IscrizioneREA = iscrizioneRea

        Return mapped

    End Function

    Private Function AnagraficaCedenteNonDittaIndividuale(ByVal id_cf As Integer, ByVal fatGias As FatturaGias) As FatturaPa.AnagraficaType

        Dim anagrafica = New FatturaPa.AnagraficaType()
        'anagrafica.CodEORI = String.Empty
        'anagrafica.Titolo = String.Empty

        If id_cf = enum_Contatti_IdCf.PersonaFisica Then
            anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType1() _
                {
                    FatturaPa.ItemsChoiceType1.Nome, FatturaPa.ItemsChoiceType1.Cognome
                }
            anagrafica.Items = New String() _
                {
                    TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.Nome), 60),
                    TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.Cognome), 60)
                }
        Else
            anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType2() _
               {
                   FatturaPa.ItemsChoiceType1.Denominazione
               }
            anagrafica.Items = New String() _
               {
                   TroncaStringa(Trim(fatGias.Cedente.DatiPrincipali.Anagrafica.rag_soc), 80)
               }
        End If
        Return anagrafica

    End Function

    Private Function AnagraficaCedenteDittaIndividuale(ByVal fatGias As FatturaGias) As FatturaPa.AnagraficaType

        Dim anagrafica = New FatturaPa.AnagraficaType()
        'anagrafica.CodEORI = String.Empty
        'anagrafica.Titolo = String.Empty

        anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType1() _
            {
                FatturaPa.ItemsChoiceType1.Nome, FatturaPa.ItemsChoiceType1.Cognome
            }

        If Not String.IsNullOrEmpty(fatGias.LegaleRappresentante.Anagrafica.rag_soc) Then
            Dim nomeCognome = fatGias.LegaleRappresentante.Anagrafica.rag_soc.Split(" ").ToList
            anagrafica.Items = New String() _
               {
                   TroncaStringa(Trim(nomeCognome(1)), 60),
                   TroncaStringa(Trim(nomeCognome(0)), 60)
               }
        Else

            anagrafica.Items = New String() _
                {
                    TroncaStringa(Trim(fatGias.LegaleRappresentante.Anagrafica.Nome), 60),
                    TroncaStringa(Trim(fatGias.LegaleRappresentante.Anagrafica.Cognome), 60)
                }
        End If

        Return anagrafica

    End Function

    Private Function MapCessionarioCommittente_PA(ByVal fatGias As FatturaGias, ByVal datiTrasmittente As FatturaPa.DatiTrasmissioneType) As FatturaPa.CessionarioCommittenteType

        Dim mapped As New FatturaPa.CessionarioCommittenteType()

        Dim datiAnagrafici = New FatturaPa.DatiAnagraficiCessionarioType()

        Dim isAssociazione = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto.IsAssociazione()
        Dim id_cf = fatGias.Cessionario.DatiPrincipali.Anagrafica.Id_CF

        Dim anagrafica = New FatturaPa.AnagraficaType()

        Dim denominazione = fatGias.Cessionario.DatiPrincipali.Anagrafica.rag_soc
        If String.IsNullOrEmpty(denominazione) Then
            denominazione = String.Concat(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cognome, " ", fatGias.Cessionario.DatiPrincipali.Anagrafica.Nome)
        End If

        Select Case id_cf

            Case enum_Contatti_IdCf.PersonaFisica

                ' PERSONE FISICHE >>>>>>>>>>>>>>>>>
                datiAnagrafici.CodiceFiscale = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto
                If UtilityProvider.VerificaEspressioneRegolare(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto, "", enum_EspressioniRegolari.RegExp_PartitaIVA) Then

                    ' PERSONE FISICHE CON PARTITA IVA (ASSOCIAZIONI O ENTI CON COD FISCALE UGUALE = PIVA) >>>>
                    anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType2() _
                    {
                        FatturaPa.ItemsChoiceType1.Denominazione
                    }
                    anagrafica.Items = New String() _
                    {
                        TroncaStringa(Trim(denominazione), 80)
                    }
                Else

                    ' PERSONA FISICA VERA A PROPRIA CON COD FISCALE LUNGO 16
                    anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType1() _
                    {
                        FatturaPa.ItemsChoiceType1.Nome, FatturaPa.ItemsChoiceType1.Cognome
                    }
                    anagrafica.Items = New String() _
                    {
                        TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Anagrafica.Nome), 60),
                        TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Anagrafica.Cognome), 60)
                    }

                End If

            Case enum_Contatti_IdCf.PersonaGiuridica

                ' PERSONE GIURIDICHE
                anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType2() _
                        {
                            FatturaPa.ItemsChoiceType1.Denominazione
                        }
                anagrafica.Items = New String() _
                        {
                             TroncaStringa(Trim(denominazione), 80)
                        }

                If isAssociazione Then
                    ' PERSONA GIURIDICA MA ASSOCIAZIONE
                    datiAnagrafici.CodiceFiscale = fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto
                Else
                    ' PERSONA GIURIDICA 
                    datiAnagrafici.IdFiscaleIVA = New FatturaPa.IdFiscaleType With
                    {
                        .IdCodice = NormalizzaPIVA(fatGias.OttieniPivaCessionario(), fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)),
                        .IdPaese = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
                    }

                End If

                'Codice fiscale per Gruppi Iva
                If Not String.IsNullOrEmpty(fatGias.Cessionario.DatiPrincipali.Anagrafica.Codice_Fiscale) Then

                    ' Verifico che Piva e Codice Fiscale siano diversi
                    If fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto.Trim() <> fatGias.Cessionario.DatiPrincipali.Anagrafica.Codice_Fiscale Then

                        ' Se si tratta di una Partita Iva scrivo il nodo codicefiscale
                        If UtilityProvider.VerificaEspressioneRegolare(fatGias.Cessionario.DatiPrincipali.Anagrafica.Codice_Fiscale, "", enum_EspressioniRegolari.RegExp_PartitaIVA) Then

                            datiAnagrafici.CodiceFiscale = fatGias.Cessionario.DatiPrincipali.Anagrafica.Codice_Fiscale.Trim()
                            datiTrasmittente.CodiceDestinatario = "0000000"

                        End If

                    End If

                End If

            Case Else

                ' PERSONA GIURIDICA ESTERA
                anagrafica.ItemsElementName = New FatturaPa.ItemsChoiceType2() _
                        {
                            FatturaPa.ItemsChoiceType1.Denominazione
                        }
                anagrafica.Items = New String() _
                        {
                             TroncaStringa(Trim(denominazione), 80)
                        }

                datiAnagrafici.IdFiscaleIVA = New FatturaPa.IdFiscaleType With
                {
                    .IdCodice = NormalizzaPIVA(fatGias.OttieniPivaCessionario(), fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)),
                    .IdPaese = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
                }

        End Select

        datiAnagrafici.Anagrafica = anagrafica
        mapped.DatiAnagrafici = datiAnagrafici

        ' obbligatorio
        Dim sede = New FatturaPa.IndirizzoType With {
            .CAP = fatGias.Cessionario.DatiPrincipali.Indirizzo.CAP,
            .Comune = TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaComune(id_cf, _decodificheMapper)), 60),
            .Indirizzo = TroncaStringa(Trim(fatGias.Cessionario.DatiPrincipali.Indirizzo.ind_des), 60),
            .Nazione = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaStato(id_cf)
        }
        If id_cf <> enum_Contatti_IdCf.ContattoEstero Then
            sede.Provincia = fatGias.Cessionario.DatiPrincipali.Indirizzo.NormalizzaProvincia(_decodificheMapper)
        End If
        mapped.Sede = sede

        ' il cessionario / committente è un soggetto che non risiede In Italia ma che, in
        'Italia, dispone di una stabile organizzazione attraverso la quale svolge la
        'propria attività oggetto di fatturazione
        If id_cf = enum_Contatti_IdCf.ContattoEstero AndAlso fatGias.Cessionario.StabileOrganizzazione IsNot Nothing Then

            Dim so = fatGias.Cessionario.StabileOrganizzazione

            If so IsNot Nothing Then
                Dim stabileOrg = New FatturaPa.IndirizzoType With {
                   .CAP = so.Indirizzo.CAP,
                   .Comune = TroncaStringa(Trim(so.Indirizzo.com_des), 60),
                   .Indirizzo = TroncaStringa(Trim(so.Indirizzo.ind_des), 60),
                   .Nazione = so.Indirizzo.NormalizzaStato(id_cf),
                   .Provincia = so.Indirizzo.pro_cod
                }
                mapped.StabileOrganizzazione = stabileOrg
            End If

        End If

        ' Obbligatorio il cessionario / committente si configura come soggetto non residente che effettua
        'nel territorio dello stato italiano operazioni rilevanti ai fini IVA e che si avvale, in
        'Italia, di un rappresentante fiscale
        If id_cf = enum_Contatti_IdCf.ContattoEstero Then

            Dim rfRisUm = fatGias.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.RappresentanteFiscale)
            If Not String.IsNullOrEmpty(rfRisUm) Then

                Dim rf = _decodificheMapper.OttieniRappresentanteFiscale(CInt(rfRisUm))
                If rf IsNot Nothing Then
                    Dim rappresentanteFiscale = New FatturaPa.RappresentanteFiscaleCessionarioType With {
                        .IdFiscaleIVA = New FatturaPa.IdFiscaleType With
                    {
                        .IdCodice = rf.Cod_Contatto,
                        .IdPaese = "IT"
                    },
                        .ItemsElementName = New FatturaPa.ItemsChoiceType2() _
                       {
                           FatturaPa.ItemsChoiceType1.Denominazione
                       },
                        .Items = New String() _
                       {
                           TroncaStringa(Trim(rf.Rag_Soc), 80)
                       }
                    }
                    mapped.RappresentanteFiscale = rappresentanteFiscale
                End If

            End If

        End If


        Return mapped

    End Function

    Private Function MapTerzoIntermediarioOSoggettoEmittente_PA(ByVal fatGias As FatturaGias) As FatturaPa.TerzoIntermediarioSoggettoEmittenteType

        Return New FatturaPa.TerzoIntermediarioSoggettoEmittenteType()

    End Function

    Private Function MapFatturaElettronicaBody_PA(ByVal fatGias As FatturaGias, ByVal id_cod_cliente As Integer) As FatturaPa.FatturaElettronicaBodyType

        Dim body = New FatturaPa.FatturaElettronicaBodyType()

        body.DatiGenerali = MapBodyDatiGenerali(fatGias)
        body.DatiBeniServizi = MapBodyDatiBeniServizi(fatGias)
        body.DatiPagamento = MapBodyDatiPagamento(fatGias)

        AzioniPerSplitPayment(fatGias, body)

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

        If filePdf IsNot Nothing Then
            body.Allegati = New FatturaPa.AllegatiType() {New FatturaPa.AllegatiType With
                {
                    .FormatoAttachment = "pdf",
                    .Attachment = filePdf,
                    .NomeAttachment = GetNomeAllegato(fatGias)
                }
            }
        End If

        Return body

    End Function

    Private Function MapBodyDatiPagamento(fatGias As FatturaGias) As FatturaPa.DatiPagamentoType()

        If fatGias.Pagamenti Is Nothing OrElse Not fatGias.Pagamenti.Any() Then
            Return Nothing
        End If

        ' se anche una sola riga di pagamenti non ha modalità di pagamento impostata non genero il nodo pagamenti
        If fatGias.Pagamenti.Any(Function(p) p.CausalePagamento.Tipo = enum_PagamentiCausali.NonImpostato) Then
            Return Nothing
        End If

        Dim mapped = New List(Of FatturaPa.DatiPagamentoType)
        Dim importoTotaleDocumento = fatGias.OttieniImportoTotaleDocumento()
        Dim rate = fatGias.Pagamenti.OrderByDescending(Function(p) p.DatiPagamento.DataScadenza_Manuale)

        If rate.Count() = 1 AndAlso rate.FirstOrDefault().DatiPagamento.Percentuale = 100 Then

            'pagamento completo / anticipato

            Dim isAnticipato As Boolean = rate.FirstOrDefault().DatiPagamento.DataScadenza_Manuale < fatGias.Fattura.Testata.Data_Movimento

            Dim pag = New FatturaPa.DatiPagamentoType With {.CondizioniPagamento = If(isAnticipato, FatturaPa.CondizioniPagamentoType.TP03, FatturaPa.CondizioniPagamentoType.TP02)}
            Dim rata = rate.FirstOrDefault()

            Dim dettagli = New List(Of FatturaPa.DettaglioPagamentoType) From {MappaDettaglioPagamento(rata, importoTotaleDocumento)}
            pag.DettaglioPagamento = dettagli.ToArray()
            mapped.Add(pag)

        Else

            ' pagamento a rate
            Dim pag = New FatturaPa.DatiPagamentoType With {.CondizioniPagamento = FatturaPa.CondizioniPagamentoType.TP01}
            Dim dettagli = New List(Of FatturaPa.DettaglioPagamentoType)

            For Each p As PagamentoMap In rate
                dettagli.Add(MappaDettaglioPagamento(p, importoTotaleDocumento))
            Next
            pag.DettaglioPagamento = dettagli.ToArray()
            mapped.Add(pag)
        End If

        ' se anche una sola riga di pagamenti non è stata decodificata in modo corretto genero il nodo pagamenti
        If mapped.FirstOrDefault().DettaglioPagamento.Any(Function(dp) dp.ModalitaPagamento = Int32.MinValue) Then
            Return Nothing
        End If

        Return mapped.ToArray()

    End Function

    Private Function MappaDettaglioPagamento(ByVal p As PagamentoMap, ByVal importoTotaleDocumento As Decimal) As FatturaPa.DettaglioPagamentoType

        Dim importo As Decimal = p.DatiPagamento.Importo
        If importo = 0 Then
            If importoTotaleDocumento <> 0 Then
                importo = (importoTotaleDocumento * p.DatiPagamento.Percentuale) / 100
            End If
        End If

        Dim dettaglio = New FatturaPa.DettaglioPagamentoType With
                {
                    .ModalitaPagamento = _decodificheMapper.DecodificaModalitaPagamento(p.CausalePagamento.Tipo),
                    .DataScadenzaPagamento = p.DatiPagamento.DataScadenza_Manuale,
                    .DataScadenzaPagamentoSpecified = True,
                    .ImportoPagamento = NormalizzaImporto(importo, 2)
                }
        If Not String.IsNullOrEmpty(p.DatiBancari.ToString()) Then
            dettaglio.IBAN = p.DatiBancari.ToString()
        End If
        If Not String.IsNullOrEmpty(p.DatiBancari.Bic) Then
            dettaglio.BIC = p.DatiBancari.Bic
        End If

        Return dettaglio

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
            myReq.Timeout = 60000 * 20
            myReq.AllowAutoRedirect = True
            myReq.KeepAlive = True
            myReq.CookieContainer = New Net.CookieContainer()

            myRes = myReq.GetResponse()

            'Solo se mi è tornato il pdf, altrimenti vuol dire che c'è stato qualche errore
            If myRes IsNot Nothing AndAlso myRes.ContentType = "application/pdf" Then

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

            Else
                ' Response nothing oppure content type <> application/pdf
                If myRes IsNot Nothing Then
                    _logger.Logga("FatturaMapper.Stampa", String.Format("La stampa richiesta all'Url {0} ha inviato una response di tipo {1}", url, myRes.ContentType))
                    If myRes.ContentType.ToLower().StartsWith("text/") Then
                        mySourceStream = myRes.GetResponseStream()
                        Dim strReader As New StreamReader(mySourceStream, Encoding.UTF8)
                        Dim contenutoRisposta As String = strReader.ReadToEnd
                        _logger.Logga("FatturaMapper.Stampa", String.Format("Contenuto della risposta: {0} {1}", vbCrLf, contenutoRisposta))

                    End If
                Else
                    _logger.Logga("FatturaMapper.Stampa", String.Format("La stampa richiesta all'Url {0} ha inviato una response Nothing", url))
                End If

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

        If myTempStream IsNot Nothing Then
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

    Private Function MapBodyDatiGenerali(ByVal fatGias As FatturaGias) As FatturaPa.DatiGeneraliType

        Dim mapped As New FatturaPa.DatiGeneraliType

        Dim datiGeneraliDocumento = New FatturaPa.DatiGeneraliDocumentoType With
        {
            .TipoDocumento = fatGias.OttieniTipoDocumento(_decodificheMapper),
            .Divisa = "EUR",
            .Data = fatGias.Fattura.Testata.Data_Movimento,
            .Numero = fatGias.OttieniNumeroFattura()
        }

        Dim bolli = fatGias.Fattura.Dettagli.Bolli
        If bolli IsNot Nothing AndAlso bolli.Any Then

            Dim importo = bolli.Sum(Function(b) (b.Qta * b.Prezzo_Unitario))

            Dim datiBollo = New FatturaPa.DatiBolloType With
            {
                .BolloVirtuale = FatturaPa.BolloVirtualeType.SI,
                .ImportoBollo = NormalizzaImporto(importo, 8)
            }
            datiGeneraliDocumento.DatiBollo = datiBollo
        End If

        Dim importoTotale = fatGias.OttieniImportoTotaleDocumento()
        datiGeneraliDocumento.ImportoTotaleDocumento = importoTotale
        datiGeneraliDocumento.ImportoTotaleDocumentoSpecified = True

        datiGeneraliDocumento.Arrotondamento = 0
        Dim desLib As String = TroncaStringa(Trim(fatGias.Fattura.Testata.Des_lib), 200)
        Dim causaleTrasporto = TroncaStringa(Trim(fatGias.Fattura.Testata.Causale_Trasporto), 200)

        If causaleTrasporto = String.Empty OrElse causaleTrasporto = "0" Then
            datiGeneraliDocumento.Causale = New String() {desLib}
        Else
            datiGeneraliDocumento.Causale = New String() {causaleTrasporto}
        End If

        mapped.DatiGeneraliDocumento = datiGeneraliDocumento

        ' DDT Collegati

        If fatGias.Fattura.TipoFattura = TipoDocumentoType.TD04.ToString() Then ' Nota di credito

            If fatGias.RifNotaCreditoDebito IsNot Nothing Then
                Dim ddts As New List(Of FatturaPa.DatiDDTType)
                Dim ddt As New FatturaPa.DatiDDTType With {
                        .NumeroDDT = fatGias.RifNotaCreditoDebito.N_Nota_DDT,
                        .DataDDT = fatGias.RifNotaCreditoDebito.Data_Nota_DDT
                        }
                ddts.Add(ddt)
                mapped.DatiDDT = ddts.ToArray()

            End If

        Else

            'Documenti collegati
            Select Case fatGias.Fattura.TipoFattura
                Case TipoDocumentoType.TD05.ToString(), ' Nota di debito
                    TipoDocumentoType.TD16.ToString(), ' Integrazione Reverse Charge interno
                    TipoDocumentoType.TD17.ToString(), ' Integrazione acquisti servizi dall'estero
                    TipoDocumentoType.TD18.ToString(), ' Integrazione acquisti beni intra-comunitari
                    TipoDocumentoType.TD19.ToString(), ' Integrazione acquisti beni ex art 17 c2
                    TipoDocumentoType.TD20.ToString(), ' Autofattura per regolarizzazione (ex art 6 commi 8-9bis)
                    TipoDocumentoType.TD21.ToString(), ' Autofattura per splafonamento
                    TipoDocumentoType.TD22.ToString(), ' Estrazione beni da deposito IVA
                    TipoDocumentoType.TD23.ToString() ' Estrazione beni da deposito IVA con versamento IVA

                    If fatGias.RifNotaCreditoDebito IsNot Nothing Then
                        Dim fatts As New List(Of FatturaPa.DatiDocumentiCorrelatiType)
                        Dim fatt As New FatturaPa.DatiDocumentiCorrelatiType With {
                            .IdDocumento = fatGias.RifNotaCreditoDebito.N_Nota_Fattura
                        }

                        If fatGias.RifNotaCreditoDebito.Data_Nota_Fattura <> AGRODATAINIZIO Then
                            fatt.Data = fatGias.RifNotaCreditoDebito.Data_Nota_Fattura
                            fatt.DataSpecified = True
                        End If

                        fatts.Add(fatt)
                        mapped.DatiFattureCollegate = fatts.ToArray()

                    End If

            End Select


            Dim docCollegati = fatGias.DocumentiCollegati.Documenti
            Dim agendeCollegate = docCollegati.Select(Function(a) a.Id_Agenda_Rif).Distinct().ToList()
            If docCollegati IsNot Nothing AndAlso docCollegati.Any() Then

                Dim ddts = New List(Of FatturaPa.DatiDDTType)
                agendeCollegate.ForEach(Sub(agenda)

                                            Dim dc = docCollegati.FirstOrDefault(Function(d) d.Id_Agenda_Rif = agenda)
                                            Dim ddt As New FatturaPa.DatiDDTType With {
                                                .DataDDT = dc.Data_Movimento,
                                                .NumeroDDT = dc.OttieniNumeroFattura()
                                            }

                                            Dim ddtProdCount = fatGias.DocumentiCollegati.Riepilogo.FirstOrDefault(Function(r) r.Id_Agenda = agenda)
                                            If ddtProdCount IsNot Nothing Then
                                                Dim ddtProdInFattura = docCollegati.Where(Function(d) d.Id_Agenda_Rif = agenda)
                                                If ddtProdInFattura.Count() < ddtProdCount.NumMovimenti Then

                                                    Dim rifLinee = ddtProdInFattura.Select(Function(p) p.Ordine_Det.ToString()).OrderBy(Function(p) p)
                                                    ddt.RiferimentoNumeroLinea = rifLinee.ToArray()
                                                End If
                                            End If

                                            ddts.Add(ddt)
                                        End Sub)
                mapped.DatiDDT = ddts.ToArray()
            End If

        End If


        ' Dati Trasporto (al momento solo indirizzo resa)
        Dim id_cf = fatGias.CessionarioDiverso.Anagrafica.Id_CF
        Dim indirizzoResa = New FatturaPa.IndirizzoType With
        {
            .CAP = fatGias.CessionarioDiverso.Indirizzo.CAP,
            .Comune = TroncaStringa(Trim(fatGias.CessionarioDiverso.Indirizzo.NormalizzaComune(id_cf, _decodificheMapper)), 60),
            .Indirizzo = TroncaStringa(Trim(fatGias.CessionarioDiverso.Indirizzo.ind_des), 60),
            .Nazione = fatGias.CessionarioDiverso.Indirizzo.NormalizzaStato(id_cf)
        }
        If id_cf <> enum_Contatti_IdCf.ContattoEstero Then
            indirizzoResa.Provincia = fatGias.CessionarioDiverso.Indirizzo.NormalizzaProvincia(_decodificheMapper)
        End If

        mapped.DatiTrasporto = New FatturaPa.DatiTrasportoType With
        {
            .IndirizzoResa = indirizzoResa
        }

        ' Dati Ordini di Acquisto
        If fatGias.Fattura.TipoFattura = TipoDocumentoType.TD04.ToString() Then

            'TODO: Da verificare se è stato fatto volutamente o bug?!? 

            ' Nota di credito
            If fatGias.RifNotaCreditoDebito IsNot Nothing Then
                Dim ordiniAcquisto = New List(Of FatturaPa.DatiDocumentiCorrelatiType)
                Dim oa = New FatturaPa.DatiDocumentiCorrelatiType()
                oa.IdDocumento = fatGias.RifNotaCreditoDebito.N_Nota_Fattura
                If fatGias.RifNotaCreditoDebito.Data_Nota_Fattura <> AGRODATAINIZIO Then
                    oa.Data = fatGias.RifNotaCreditoDebito.Data_Nota_Fattura
                    oa.DataSpecified = True
                End If
                ordiniAcquisto.Add(oa)
                mapped.DatiOrdineAcquisto = ordiniAcquisto.ToArray()
            End If

        Else
            If fatGias.OrdiniAcquisto IsNot Nothing AndAlso fatGias.OrdiniAcquisto.Any Then

                Dim ordiniRaggruppati = (From o In fatGias.OrdiniAcquisto
                                         Order By o.Data_Doc_Cliente Ascending
                                         Group By NumDoc = o.N_Doc_Cliente
                                      Into og = Group, Count()
                                         Order By NumDoc).ToList()

                Dim ordiniAcquisto = New List(Of FatturaPa.DatiDocumentiCorrelatiType)

                For Each rag In ordiniRaggruppati

                    Dim dataOrdine = rag.og.FirstOrDefault()

                    Dim ddc = New FatturaPa.DatiDocumentiCorrelatiType With
                    {
                        .IdDocumento = TroncaStringa(rag.NumDoc.Trim(), 20),
                        .RiferimentoNumeroLinea = rag.og.Select(Function(l)
                                                                    Return l.Ordine_Det.ToString()
                                                                End Function).ToList().ToArray()
                    }
                    If dataOrdine IsNot Nothing Then
                        ddc.Data = dataOrdine.Data_Doc_Cliente
                        ddc.DataSpecified = True
                    End If
                    ordiniAcquisto.Add(ddc)
                Next


                If ordiniAcquisto IsNot Nothing AndAlso ordiniAcquisto.Any() Then
                    mapped.DatiOrdineAcquisto = ordiniAcquisto.ToArray()
                End If

            End If

        End If

        Return mapped

    End Function

    Private Function MapBodyDatiBeniServizi(ByVal fatGias As FatturaGias) As FatturaPa.DatiBeniServiziType

        Dim mapped As New FatturaPa.DatiBeniServiziType
        Dim dettagli = New List(Of FatturaPa.DettaglioLineeType)
        Dim prgRiga As Integer = 1
        Dim ivaDefault As Integer = IvaDefaultPerRigheDescrittive(fatGias.Fattura.Dettagli.ProdottiServizi)

        For Each prod In fatGias.Fattura.Dettagli.ProdottiServizi

            Dim det = prod.Movimento

            If det.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA Then
                dettagli.Add(DettaglioLineaNormale(prgRiga, prod, fatGias))
            Else
                prod.Movimento.Cod_Iva = ivaDefault
                dettagli.Add(DettaglioLineaDescrizioneLibera(prgRiga, prod, fatGias))
            End If

            prgRiga = prgRiga + 1

        Next


        Dim prodottiServiziReali = fatGias.Fattura.Dettagli.ProdottiServizi.Where(Function(d)
                                                                                      Return d.Movimento.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA
                                                                                  End Function)

        ' Righe aggiuntive storno per omaggi con senza rivalsa iva
        Dim righeAggiuntive = prodottiServiziReali.Where(Function(d)
                                                             Return d.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva OrElse
                                                                    d.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_SenzaRivalsaIva
                                                         End Function)

        For Each prod In righeAggiuntive

            Dim det = prod.Movimento

            Dim unitaMisura = TroncaStringa(Trim(_decodificheMapper.DecodificaUnitaMisura(det.Udm_Cod)), 10)

            Dim livelloPrezzo = prod.LivellaPrezzo(unitaMisura, _decodificheMapper.ModuloGenerazione, _decodificheMapper)
            Dim prezzoTotale As Decimal = det.OttieniPrezzoTotaleDettaglio(fatGias.Fattura.Testata.Lav_cod, _contabilitaHelper)
            Dim alIva As Decimal = _decodificheMapper.DecodificaAliquotaIva(1).NormalizzaImporto(2)
            Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(1)

            Dim valoreIva = _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod, det.Iva)
            Dim importoLinea = If(det.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva, prezzoTotale * -1, (prezzoTotale + valoreIva) * -1)
            Dim descrizione = "Storno valori omaggi {0} rivalsa IVA"
            Dim dettaglioLinea = New FatturaPa.DettaglioLineeType() With
            {
                .NumeroLinea = prgRiga,
                .Descrizione = String.Format(descrizione, If(det.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva, "con", "senza")),
                .PrezzoUnitario = NormalizzaImporto(importoLinea, 8),
                .PrezzoTotale = NormalizzaImporto(importoLinea, 8),
                .AliquotaIVA = alIva
            }
            If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
                dettaglioLinea.Natura = natura
                dettaglioLinea.NaturaSpecified = True
            End If

            dettagli.Add(dettaglioLinea)
            prgRiga = prgRiga + 1

        Next
        mapped.DettaglioLinee = dettagli.ToArray()

        Dim riepilogo = New List(Of FatturaPa.DatiRiepilogoType)

        ' Castelletto iva senza righe omaggi con e senza rivalsa iva
        Dim movDet = fatGias.Fattura.Dettagli.ProdottiServizi.Select(Function(ps) ps.Movimento).ToList()
        riepilogo.AddRange(CastellettoIva(movDet, fatGias))

        ' Castelletto iva per omaggi con rivalsa Iva
        movDet = prodottiServiziReali.Where(Function(p) p.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_ConRivalsaIva).
                                                                        Select(Function(ps) ps.Movimento).ToList()
        riepilogo.AddRange(CastellettoIvaPerOmaggi(True, movDet, fatGias))

        ' Castelletto iva per omaggi senza rivalsa Iva
        movDet = prodottiServiziReali.Where(Function(p) p.Movimento.Sconto_Modalita = enModalitaSconto.Omaggio_SenzaRivalsaIva).
                                                                        Select(Function(ps) ps.Movimento).ToList()
        riepilogo.AddRange(CastellettoIvaPerOmaggi(False, movDet, fatGias))

        mapped.DatiRiepilogo = riepilogo.ToArray()


        Return mapped

    End Function

    Private Function IvaDefaultPerRigheDescrittive(ByVal prodottiServizi As List(Of ProdottoServizioMap)) As Integer

        Dim prodottiReali = prodottiServizi.Where(Function(p) p.Movimento.Elem_Cod <> RIGA_DESCRIZIONE_LIBERA).Select(Function(p) p.Movimento).ToList()
        Dim primoValido = prodottiReali.Where(Function(m) m.Cod_Iva > 0).FirstOrDefault()
        If primoValido IsNot Nothing Then
            Return primoValido.Cod_Iva
        Else
            ' Fuori Campo IVA
            Return 1
        End If

    End Function

    Private Function DettaglioLineaNormale(ByVal prgRiga As Integer,
                                    ByVal prod As ProdottoServizioMap,
                                    ByVal fatGias As FatturaGias
                                    ) As FatturaPa.DettaglioLineeType

        Dim det = prod.Movimento
        Dim adg = New List(Of FatturaPa.AltriDatiGestionaliType)

        ' decodifica udm xche serve nel livello prezzo
        Dim unitaMisura = TroncaStringa(Trim(_decodificheMapper.DecodificaUnitaMisura(det.Udm_Cod)), 10)

        Dim livelloPrezzo = prod.LivellaPrezzo(unitaMisura, _decodificheMapper.ModuloGenerazione, _decodificheMapper)
        Dim prezzoTotale As Decimal = det.OttieniPrezzoTotaleDettaglio(fatGias.Fattura.Testata.Lav_cod, _contabilitaHelper)
        Dim alIva As Decimal = _decodificheMapper.DecodificaAliquotaIva(det.Cod_Iva).NormalizzaImporto(2)
        Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(det.Cod_Iva)

        Dim dettaglioLinea = New FatturaPa.DettaglioLineeType() With
                {
                    .NumeroLinea = prgRiga,
                    .Descrizione = TroncaStringa(Trim(prod.Descrizione), 1000),
                    .Quantita = livelloPrezzo.Qta,
                    .QuantitaSpecified = True,
                    .UnitaMisura = TroncaStringa(Trim(livelloPrezzo.UDM), 10),
                    .PrezzoUnitario = livelloPrezzo.Prezzo_Unitario,
                    .PrezzoTotale = prezzoTotale,
                    .AliquotaIVA = alIva
                }
        If det.Sconto_Modalita <> enModalitaSconto.Percentuale Then
            dettaglioLinea.TipoCessionePrestazione = FatturaPa.TipoCessionePrestazioneType.AB
            dettaglioLinea.TipoCessionePrestazioneSpecified = True

            Dim rifTesto = String.Empty
            Select Case det.Sconto_Modalita
                Case enModalitaSconto.Omaggio_ConRivalsaIva
                    rifTesto = "Omaggio con rivalsa #OC#"
                Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                    rifTesto = "Omaggio senza rivalsa #OS#"
                Case enModalitaSconto.Campioni_Gratuiti
                    rifTesto = "Campioni gratuiti"
                Case enModalitaSconto.Sconto_Merce
                    rifTesto = "Sconto merce #SM#"
            End Select
            adg.Add(New FatturaPa.AltriDatiGestionaliType With
                         {
                            .TipoDato = "AswTRiga",
                            .RiferimentoTesto = rifTesto
                         })

        End If

        If Not IsNothing(natura) AndAlso natura = NaturaType.N35 Then

            Dim di = OttieniNodoDichiarazioneIntento(fatGias)
            If Not IsNothing(di) Then
                adg.Add(di)
            End If
        End If

        If adg.Any Then
            dettaglioLinea.AltriDatiGestionali = adg.ToArray()
        End If

        If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
            dettaglioLinea.Natura = natura
            dettaglioLinea.NaturaSpecified = True
        End If

        ' Sconti
        'se si tratta di omaggio con rivalsa Iva o senza rivalsa iva non scrivo il nodo dello sconto
        If det.Sconto_Modalita <> enModalitaSconto.Omaggio_ConRivalsaIva AndAlso det.Sconto_Modalita <> enModalitaSconto.Omaggio_SenzaRivalsaIva Then
            If det.Sconto.HasValue AndAlso det.Sconto.Value <> CDbl(0) OrElse Not String.IsNullOrEmpty(det.Sconto_Testo) Then

                Dim scontiMaggiorazioni = New List(Of FatturaPa.ScontoMaggiorazioneType)

                Dim sconti = New List(Of Double) From {
                        det.Sconto * -1
                    }
                If Not String.IsNullOrEmpty(det.Sconto_Testo) Then
                    sconti.AddRange(det.Sconto_Testo.Split("-").ToList().ConvertAll(Function(s) CDbl(s)))
                End If

                sconti.ForEach(Sub(s)
                                   Dim sm = New FatturaPa.ScontoMaggiorazioneType With
                                       {
                                            .Tipo = FatturaPa.TipoScontoMaggiorazioneType.SC,
                                            .Percentuale = NormalizzaImporto(s, 4),
                                            .PercentualeSpecified = True
                                       }
                                   scontiMaggiorazioni.Add(sm)
                               End Sub)

                dettaglioLinea.ScontoMaggiorazione = scontiMaggiorazioni.ToArray()
            End If
        End If

        Return dettaglioLinea

    End Function

    Private Function DettaglioLineaDescrizioneLibera(ByVal prgRiga As Integer,
                                   ByVal prod As ProdottoServizioMap,
                                   ByVal fatGias As FatturaGias
                                   ) As FatturaPa.DettaglioLineeType

        Dim adg = New List(Of FatturaPa.AltriDatiGestionaliType)
        Dim det = prod.Movimento
        Dim alIva As Decimal = _decodificheMapper.DecodificaAliquotaIva(det.Cod_Iva).NormalizzaImporto(2)
        Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(det.Cod_Iva)

        Dim dettaglioLinea = New FatturaPa.DettaglioLineeType() With
                {
                    .NumeroLinea = prgRiga,
                    .Descrizione = TroncaStringa(Trim(prod.Descrizione), 1000),
                    .PrezzoUnitario = NormalizzaImporto(0, 8),
                    .PrezzoTotale = NormalizzaImporto(0, 8),
                    .AliquotaIVA = alIva
                }

        If dettaglioLinea.AliquotaIVA = 0 AndAlso natura <> Int32.MinValue Then
            dettaglioLinea.Natura = natura
            dettaglioLinea.NaturaSpecified = True
        End If

        adg.Add(New FatturaPa.AltriDatiGestionaliType With
                         {
                            .TipoDato = "AswTRiga",
                            .RiferimentoTesto = "Descrittivo #DE#"
                         })

        If Not IsNothing(natura) AndAlso natura = NaturaType.N35 Then

            Dim di = OttieniNodoDichiarazioneIntento(fatGias)
            If Not IsNothing(di) Then
                adg.Add(di)
            End If

        End If
        dettaglioLinea.AltriDatiGestionali = adg.ToArray()

        Return dettaglioLinea

    End Function

    Private Function OttieniNodoDichiarazioneIntento(ByVal fatGias As FatturaGias) As AltriDatiGestionaliType

        If IsNothing(fatGias) Then
            Return Nothing
        End If
        If IsNothing(fatGias.Cessionario) Then
            Return Nothing
        End If
        If IsNothing(fatGias.Cessionario.DatiAggiuntivi) OrElse Not fatGias.Cessionario.DatiAggiuntivi.Any Then
            Return Nothing
        End If

        Dim numeroProtocollo As String = fatGias.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo)
        Dim dataProtocollo As String = fatGias.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione)

        ' Se uno dei due dati per la dichiarazione di intenti non è valorizzato non creo il nodo
        If String.IsNullOrEmpty(numeroProtocollo) OrElse String.IsNullOrEmpty(dataProtocollo) Then
            Return Nothing
        End If

        ' Controllo sulla validita della data (perchè arriva da un campo stringa e potenzialmente potrebbe essere errato)
        Dim checkData As DateTime = DateTime.MinValue
        If Not DateTime.TryParse(dataProtocollo, checkData) Then
            Return Nothing
        End If

        Dim di As New FatturaPa.AltriDatiGestionaliType With
                         {
                            .TipoDato = "Intento",
                            .RiferimentoTesto = numeroProtocollo,
                            .RiferimentoData = checkData,
                            .RiferimentoDataSpecified = True
                         }

        Return di

    End Function


    Private Function CastellettoIva(ByVal movDet As List(Of Movimenti_dettagli),
                                            ByVal fatGias As FatturaGias) As List(Of FatturaPa.DatiRiepilogoType)

        Dim riepilogo = New List(Of FatturaPa.DatiRiepilogoType)

        Dim dettagliRaggrupati = (From det In movDet
                                  Order By det.Cod_Iva
                                  Group By CODIVA = det.Cod_Iva
                                  Into movimenti = Group, Count()
                                  Order By CODIVA).ToList()

        Dim lavCod = fatGias.Fattura.Testata.Lav_cod

        For Each iva In dettagliRaggrupati

            Dim riepilogoIva = New FatturaPa.DatiRiepilogoType With
            {
                .AliquotaIVA = _decodificheMapper.DecodificaAliquotaIva(iva.CODIVA).NormalizzaImporto(2),
                .ImponibileImporto = NormalizzaImporto(iva.movimenti.Sum(Function(m) _contabilitaHelper.Leggi_Imponibile_PositivoNegativo(lavCod, m.Imponibile_Netto)), 2),
                .Imposta = NormalizzaImporto(
                    _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod,
                                                                  (iva.movimenti.Sum(Function(m) m.Iva))), 2)
            }
            If riepilogoIva.AliquotaIVA = 0 Then
                Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(iva.CODIVA)
                If natura <> Int32.MinValue Then
                    riepilogoIva.Natura = natura
                    riepilogoIva.NaturaSpecified = True
                    'se specificato il campo natura ci va riferimento normativo
                    riepilogoIva.RiferimentoNormativo = _decodificheMapper.DecodificaRiferimentoNormativo(iva.CODIVA)
                End If
            Else
                Dim codSez = fatGias.Fattura.Testata.Sezionale_Cod
                Dim esigibilitàIva = _decodificheMapper.DecodificaEsigibilitaIVA(codSez)
                If esigibilitàIva <> Int32.MinValue Then
                    riepilogoIva.EsigibilitaIVA = esigibilitàIva
                    riepilogoIva.EsigibilitaIVASpecified = True
                End If

            End If

            riepilogo.Add(riepilogoIva)
        Next

        Return riepilogo

    End Function

    Private Function CastellettoIvaPerOmaggi(
                                            ByVal conRivalsaIva As Boolean,
                                            ByVal movDet As List(Of Movimenti_dettagli),
                                            ByVal fatGias As FatturaGias) As List(Of FatturaPa.DatiRiepilogoType)

        Dim riepilogo = New List(Of FatturaPa.DatiRiepilogoType)

        Dim dettagliRaggrupati = (From det In movDet
                                  Order By det.Cod_Iva
                                  Group By CODIVA = det.Cod_Iva
                                  Into movimenti = Group, Count()
                                  Order By CODIVA).ToList()

        For Each iva In dettagliRaggrupati

            Dim prezzoTotale = iva.movimenti.Sum(Function(m) _contabilitaHelper.Leggi_Imponibile_PositivoNegativo(fatGias.Fattura.Testata.Lav_cod, m.Imponibile_Netto))
            Dim importoIva = _contabilitaHelper.Leggi_IVA_PositivaNegativa(fatGias.Fattura.Testata.Lav_cod,
                                                                  (iva.movimenti.Sum(Function(m) m.Iva)))
            Dim importoLinea = If(conRivalsaIva, prezzoTotale * -1, (prezzoTotale + importoIva) * -1)

            Dim riepilogoIva = New FatturaPa.DatiRiepilogoType With
            {
                .AliquotaIVA = _decodificheMapper.DecodificaAliquotaIva(1).NormalizzaImporto(2),
                .ImponibileImporto = NormalizzaImporto(importoLinea, 2),
                .Imposta = NormalizzaImporto(0, 2)
            }
            If riepilogoIva.AliquotaIVA = 0 Then
                Dim natura = _decodificheMapper.DecodificaNaturaEsclusione(1)
                If natura <> Int32.MinValue Then
                    riepilogoIva.Natura = natura
                    riepilogoIva.NaturaSpecified = True
                    'se specificato il campo natura ci va riferimento normativo
                    riepilogoIva.RiferimentoNormativo = _decodificheMapper.DecodificaRiferimentoNormativo(1)
                End If
            End If

            riepilogo.Add(riepilogoIva)
        Next

        Return riepilogo

    End Function

    Private Sub AzioniPerSplitPayment(ByVal fatGias As FatturaGias, ByRef body As FatturaPa.FatturaElettronicaBodyType)


        ' Se nel sezionale è indicata esiginbilità iva S (Split Payment) all' ImportoTotaleDocumento
        ' va aggiunto l'importo di tutte le IVE

        Dim codSez = fatGias.Fattura.Testata.Sezionale_Cod
        If IsNothing(codSez) OrElse Not codSez.HasValue Then
            Return
        End If


        Dim esigibilitàIva = _decodificheMapper.DecodificaEsigibilitaIVA(codSez)
        If esigibilitàIva = EsigibilitaIVAType.S Then

            Dim totaleImposta As Decimal = 0

            If Not IsNothing(body) AndAlso Not IsNothing(body.DatiBeniServizi) AndAlso Not IsNothing(body.DatiBeniServizi.DatiRiepilogo) Then
                For Each ri As DatiRiepilogoType In body.DatiBeniServizi.DatiRiepilogo
                    totaleImposta += ri.Imposta
                Next
            End If

            If Not IsNothing(body.DatiGenerali) AndAlso Not IsNothing(body.DatiGenerali.DatiGeneraliDocumento) Then
                Dim totDocPiuIva = body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento + totaleImposta
                body.DatiGenerali.DatiGeneraliDocumento.ImportoTotaleDocumento = NormalizzaImporto(totDocPiuIva, 8)
            End If

        End If



    End Sub

End Class

