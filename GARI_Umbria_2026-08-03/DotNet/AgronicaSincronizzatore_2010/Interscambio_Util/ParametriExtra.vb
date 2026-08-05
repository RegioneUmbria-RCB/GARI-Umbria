Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class ParametriExtra

    Public Enum enum_TipoInterscambio
        Nessuno = 0               'uso per importazione anagrafica secca, senza scambio continuo
        AnagraficheSuDb = 1       'Ruggeri
        SoloCodici = 2            'Aboca, Borgoluce, Coop Sole, Manara
    End Enum

    Public Enum enum_TipoChiave
        Nessuna = 0

        ''' <summary>
        ''' CONTATTO = codContatto, 
        ''' PRODOTTO = elemCod-codProdotto,
        ''' CONTATTO IN DOC = CONTATTO-codIndirizzo
        ''' </summary>
        Essenziale = 1

        ''' <summary>
        ''' CONTATTO = piva|codContatto|codRisUm|progressivo, 
        ''' PRODOTTO = elemCod|codProdotto|codArticolo,
        ''' CONTATTO IN DOC = CONTATTO_codIndirizzo
        ''' </summary>
        Completa = 2
    End Enum

    Public Enum enum_TipoModulo
        NonSpecificato = 0
        Cantina = 1
        Campagna = 2
        FreshFood = 3
    End Enum

    Public Enum enum_GestioneParametriQualitativi
        Nessuno = 0
        CreaSoloCarico = 1
        CreaSempre = 2
    End Enum

    Public Enum enum_VersContatti
        CONT1_1 = 1
        CONT2 = 2
        CONT3 = 3
    End Enum

    Public Enum enum_VersProdotti
        PROD1_1 = 1
        PROD1 = 2
        PROD2 = 3
    End Enum

    Public Class Interscambio_Api_Authorization
        Public Property Chiave As String
        Public Property Valore As String

        Public Sub New()
            Chiave = ""
            Valore = ""
        End Sub
    End Class

    Public Sub New(ByVal parametriExtra As String, ByRef objParametri As AgronicaCoreParametri)
        ConnessioneInterscambio = ""
        DataFrom = AGRODATAINIZIO
        DataTo = AGRODATAFINE
        FlagCarico = False
        FlagScarico = False
        StringLavCodExport = ""
        PivaMagExport = ""
        SaCodMagExport = 0
        StringCodMagExport = ""
        DirInput = "Dati_in_Ingresso"
        DirError = "Dati_con_Errori"
        DirComplete = "Dati_Processati"
        DirSaltaData = "Saltati_Filtro_Data"
        DirTemporanea = "Temp"
        SuffissoAltroGestionale = "ALTRO"
        TipoImport = enum_TipoInterscambio.Nessuno
        ContattoConIndirizzo = False
        PivaInInterscambioProprietariaContatto = False
        TipoChiave = enum_TipoChiave.Essenziale
        TipoModulo = enum_TipoModulo.NonSpecificato
        ParametriQualitativi = enum_GestioneParametriQualitativi.Nessuno

        ConsentiGenerazioneAutoNumDoc = False
        PrefissoDocDefault = ""
        SuffissoDocDefault = ""
        UsaSempreListino = False
        DocumentoPrevisionale = False

        GeneraNumAccettazione = True
        PrefissoAccettazione = ""
        SuffissoAccettazione = ""

        NoMapCategoria = False
        SdoppiaSementi = False
        SottoCat_Sementi = "10S"
        SottoCat_Piantine = "10P"

        NoMapUdm = False
        NoMapFitofarmaci = False
        PrefissoFito = ""
        DerogaSuNoMapFito = False
        ObbligaOP = False
        PermettiOpEsterni = False
        VerificaGiacenza = False
        RifMovObbligatorio = False

        LeggiIdDoc = False
        IdDocUnivoco = False

        Visibilita = SACOD_NOFILTRO
        QualificaDefDip = 0
        Piva = objParametri.PivaSuperUser
        DataUltimaEsportazione = AGRODATAINIZIO
        DataFineEsportazione = AGRODATAFINE

        Agri = True
        Zoo = False
        Imputazioni = False

        ImputazioniAgri = New Integer() {}
        ImputazioniIndirette = New Integer() {}
        ImputazioniZoo = New Integer() {}

        AttivitaEscludi = New Integer() {}
        AttivitaServiziIdrici = -9999
        AttivitaAffitto = -9999

        Cod_Op = True
        Cod_Traverso = False
        Cod_Commessa = False
        Superficie = False
        LineItemNumber = False

        InviaMail = True
        Debug = False
        Riepilogo = True
        NoMailSaltati = False

        FlagScriviSempreLotto = False
        DataInizioFittizia = AGRODATAINIZIO
        DataFineFittizia = AGRODATAFINE

        EscludiAttivitaNoMap = False

        VersioneContatti = enum_VersContatti.CONT1_1
        VersioneProdotti = enum_VersProdotti.PROD1

        AppiattisciRuoli = True
        UsaTipoRapportiContabili = False
        IdAsProgressivo = False
        RapportiContabiliExport = New Integer() {}
        TipiRappContabExport = New String() {}

        ImportaSoloPrimoIndirizzo = False

        UnisciNumDoc = False
        IdinNomeFile = False

        UsaGuida = False

        NumPerFile = 500

        TraduciCodici = False
        SiglaDirezione = enum_DirezioneFile.Nessuno

        ExportCdcRiga = False
        DesClasseCdc = ""
        DesClasseWbs = ""
        DesAttivitaCosti = ""
        DesAttivitaRicavi = ""
        CollegamentoOrdineObbligatorio = False
        OrdineEsportato = False
        BloccaPostExport = True

        UsaCategorieMerce = False

        UsaWS = False
        FileType = ""
        WSTimeOut = -1
        WsEndpoint = ""
        WsAuthEndpoint = ""
        WsAuthorizationType = enum_TipoAutorizzazioneWS.Altro
        WsOAuth2 = New Ws_Auth_OAUTH2()
        WsAuthorization = New Interscambio_Api_Authorization
        EscludiErrori = False

        AgendeConGestionePratica = False
        PraticheStatiDaEsportare = New Integer() {}
        PraticheStatoSuccesso = enum_WWorflow_WAnagraficaStati.DocContabili_Inviato
        PraticheStatoErrore = enum_WWorflow_WAnagraficaStati.DocContabili_InvioFallito

        UsaCausaleTrasporto = True
        MostraSeOrdineChiuso = False

        CodificaSuImpresaSpecifica = False
        CodificaPiva = ""

        TrasferisciIndirDefault = False

        EliminaGiacenzePrecedenti = False
        PermettiSovrascrittura = False
        MovimentoNumeroGiorniPrecedenti = 0
        ImportaSoloDocFatturati = False

        SdoppiaFitofarmaci = False
        SottoCat_Fitofarmaci = "FIT"
        SottoCat_Corroboranti = "COR"

        ModalitaDemetra = False
        CUAAExport = New String() {}
        ContattiPubbliciSoloSuperuser = False
        CuaaAsPiva = False
        SistemaEsterno = enum_SistemiEsterni.gias

        AbilitaGiasDettaglioImportLog = False
        AbilitaGiasDettaglioExportLog = False

        FiltroEsportazione = Enum_FiltroEsportazione_to_ElasticSearch.Nessuno 'Non esportare ad elasticSearch
        ES_LogIntermedi = Enum_FiltroEsportazione_to_ElasticSearch.Nessuno 'Non esportare ad elasticSearch
        Ambiente = ""

        IdRuolo = False

        BloccaRitentaPerProdotti = False

        ProseguiSCMAGPerErroriProdotti = False

        ValorizzaParametri(parametriExtra)

    End Sub

    Public Property ConnessioneInterscambio As String

    Public Property DataFrom As Date?

    Public Property DataTo As Date?

    Public Property FlagCarico As Boolean?

    Public Property FlagScarico As Boolean?

    Public Property StringLavCodExport As String

    Public Property PivaMagExport As String

    Public Property SaCodMagExport As Integer

    Public Property StringCodMagExport As String

    Public Property DirInput As String

    Public Property DirError As String

    Public Property DirComplete As String

    Public Property DirSaltaData As String

    Public Property DirTemporanea As String

    Public Property SuffissoAltroGestionale As String

    Public Property TipoImport As enum_TipoInterscambio
    Public Property ContattoConIndirizzo As Boolean
    Public Property PivaInInterscambioProprietariaContatto As Boolean

    Public Property TipoChiave As enum_TipoChiave

    Public Property TipoModulo As enum_TipoModulo

    Public Property ParametriQualitativi As enum_GestioneParametriQualitativi

    Public Property ConsentiGenerazioneAutoNumDoc As Boolean
    Public Property PrefissoDocDefault As String
    Public Property SuffissoDocDefault As String
    Public Property UsaSempreListino As Boolean
    Public Property DocumentoPrevisionale As Boolean

    Public Property GeneraNumAccettazione As Boolean
    Public Property PrefissoAccettazione As String
    Public Property SuffissoAccettazione As String

    ''' <summary>
    ''' Se True nei file il codice categoria prodotto che ci arriva è in realtà il nostro, quindi non è necessario mapping
    ''' </summary>
    Public Property NoMapCategoria As Boolean

    ''' <summary>
    ''' Se True mi aspetto 10S per sementi e 10P per piantine al posto del generico 10 e quando importo/esporto devo cambiare udm
    ''' </summary>
    Public Property SdoppiaSementi As Boolean

    ''' <summary>
    ''' Valore per la sotto categoria sementi all'interno della nostra Semente e materiale vivaistico (10). Per Aboca = 10S, altri = SEM
    ''' </summary>
    Public Property SottoCat_Sementi As String
    ''' <summary>
    ''' Valore per la sotto categoria piantine all'interno della nostra Semente e materiale vivaistico (10). Per Aboca = 10P, altri = PIANT
    ''' </summary>
    Public Property SottoCat_Piantine As String

    ''' <summary>
    ''' Se True nei file il codice udm che ci arriva è in realtà il nostro, quindi non è necessario mapping
    ''' </summary>
    Public Property NoMapUdm As Boolean

    ''' <summary>
    ''' Se True nei file il codice prodotto che ci arriva
    ''' è in realtà il numero di registrazione del prodotto (cioè il nostro Fr_Cod),
    ''' quindi il mapping viene fatto istantaneamente all'arrivo, direttamente in interscambio
    ''' </summary>
    Public Property NoMapFitofarmaci As Boolean

    ''' <summary>
    ''' Se valorizzato (e se NoMapFitofarmaci = True) allora non arriva diretto il num. di registrazione
    ''' ma arriva numero di registrazione più questo prefisso
    ''' </summary>
    Public Property PrefissoFito As String

    ''' <summary>
    ''' Se True (e se NoMapFitofarmaci = True) allora accetto che mi possano arrivare dei prodotti
    ''' con altri codici (diversi da prefisso + num reg) che faranno il giro standard dei prodotti banca dati
    ''' venendo mappati tramite CAC_Codifica_ProdottiAziendali
    ''' </summary>
    Public Property DerogaSuNoMapFito As Boolean

    Public Property ObbligaOP As Boolean

    Public Property PermettiOpEsterni As Boolean

    Public Property VerificaGiacenza As Boolean

    ''' <summary>
    ''' Se True, si aspetta obbligatoriamente la presenza nell'avpList di dettaglio della chiave <see cref="enum_avpListDettaglio.RIFERIMENTO_MOV"/>
    ''' </summary>
    Public Property RifMovObbligatorio As Boolean

    ''' <summary>
    ''' Se True, si aspetta la presenza nell'avpList di testata della chiave <see cref="enum_avpListTestata.ID_DOC"/>
    ''' </summary>
    Public Property LeggiIdDoc As Boolean

    ''' <summary>
    ''' Se True, il confronto sul doc già esistente viene fatto, non su numero doc + intestatario, ma sulla chiave nell'avpList di testata <see cref="enum_avpListTestata.ID_DOC"/>
    ''' </summary>
    Public Property IdDocUnivoco As Boolean

    ''' <summary>
    ''' SACOD_NOFILTRO = -99, Privato = 0, Pubblico = -1 (sia per Contatti che per Prodotti)
    ''' in export: Pubblico esporta quelli della piva + i pubblici
    ''' </summary>
    Public Property Visibilita As Integer

    ''' <summary>
    ''' Qualifica default per i contatti con ruolo = DIPENDENTE (Tabella Qualifiche, Colonna Qualifica_Cod)
    ''' </summary>
    Public Property QualificaDefDip As Integer

    ''' <summary>
    ''' Da usare per le esportazioni, per sapere i dati di quale azienda vanno esportati in caso di multi-azienda.
    ''' Se non impostata verrà considerata la PivaSuperUser
    ''' </summary>
    Public Property Piva As String

    Public Property DataUltimaEsportazione As Date?

    Public Property DataFineEsportazione As Date?

    'Le seguenti Agri/Zoo/Impu servono per indicare se vanno esportati i Proj/OP per agri e zoo e imputazioni(valigette)
    Public Property Agri As Boolean
    Public Property Zoo As Boolean
    Public Property Imputazioni As Boolean

    'Le seguenti contengono la/e chiave/i di Imputazioni_Tipi
    Public Property ImputazioniAgri As Integer()
    Public Property ImputazioniIndirette As Integer()
    Public Property ImputazioniZoo As Integer()

    Public Property AttivitaEscludi As Integer()
    Public Property AttivitaServiziIdrici As Integer
    Public Property AttivitaAffitto As Integer

    'Indica quali di questi codici vanno generati nei file dei consumi
    Public Property Cod_Op As Boolean
    Public Property Cod_Traverso As Boolean
    Public Property Cod_Commessa As Boolean
    Public Property Superficie As Boolean
    Public Property LineItemNumber As Boolean

    ''' <summary>
    ''' Se False, non invia nessuna mail, neanche in presenza di errori
    ''' </summary>
    Public Property InviaMail As Boolean
    Public Property Debug As Boolean
    Public Property Riepilogo As Boolean

    ''' <summary>
    ''' Se True, non invia la mail se tutti i record trovati sono stati saltati
    ''' </summary>
    Public Property NoMailSaltati As Boolean

    Public Property FlagScriviSempreLotto As Boolean

    Public Property DataInizioFittizia As Date?
    Public Property DataFineFittizia As Date?

    Public Property EscludiAttivitaNoMap As Boolean

    Public Property VersioneContatti As enum_VersContatti
    Public Property VersioneProdotti As enum_VersProdotti


    ''' <summary>
    ''' Se True nel file generato verrà creato un contatto per ogni rapporto contabile di quel contatto 
    ''' (ad es: un contatto è sia agente che cliente => vengono generati 2 blocchi contatto ognuno con 1 ruolo 
    ''' e le parti comuni dell'indirizzo, ecc.. si ripetono)
    ''' </summary>
    Public Property AppiattisciRuoli As Boolean


    ''' <summary>
    ''' Se True nel file generato, in presenza di rapporti contabili personalizzati,
    ''' verranno esportati con i loro sottostanti ruoli:
    ''' (ad es: esiste un rapporto contabile 'ristorante' che ha la tipologia 'cliente' => nel file viene esportato come 'CLI')
    ''' </summary>
    Public Property UsaTipoRapportiContabili As Boolean

    ''' <summary>
    ''' Se True L'id del contatto viene usato come progressivo nella risorsa umana, se non inserito nel tag xml Progressivo
    ''' </summary>
    Public Property IdAsProgressivo As Boolean

    ''' <summary>
    ''' Se True viene importato solo il primo indirizzo presente (sede legale/residenza) a prescindere da quanti siano arrivati sul file (su db vengono cmq creati altri 3 indirizzi vuoti)
    ''' </summary>
    Public Property ImportaSoloPrimoIndirizzo As Boolean

    ''' <summary>
    ''' Elenco dei rapporti contabili precisi da esportare (se non specificato = tutti)
    ''' </summary>
    Public Property RapportiContabiliExport As Integer()

    ''' <summary>
    ''' Elenco dei tipi rapporti contabili (loro caratteristiche) da esportare
    ''' </summary>
    Public Property TipiRappContabExport As String()

    ''' <summary>
    ''' Se True nel file generato verrà unito in unica string il num doc, altrimenti prefisso_num_suffisso
    ''' </summary>
    Public Property UnisciNumDoc As Boolean

    ''' <summary>
    ''' Se True nel nome di file generato sarà presente un identificativo del contenuto (ad esempio il numero di doc)
    ''' </summary>
    Public Property IdinNomeFile As Boolean

    ''' <summary>
    ''' Se True usa tabella guida (task sempre attivo e si risveglia quando trova la sua riga di guida con stato attivo, 
    ''' al termine rimette lo stato su spento)
    ''' </summary>
    Public Property UsaGuida As Boolean

    Public Property NumPerFile As Integer

    ''' <summary>
    ''' Se True negli Esportatori (consumi per il momento) vengono messi gli stessi codici che ci sono arrivati dall'altro gestionale
    ''' (derogando al principio che chi esporta mette i propri codici e chi riceve deve tradurre con i propri)
    ''' è assolutamente necessario per gli scambi con WS, per cui solo noi abbiamo accesso al database di interscambio
    ''' </summary>
    Public Property TraduciCodici As Boolean

    ''' <summary>
    ''' Indica la sigla di direzione, ad esempio G2S o S2G (se non specificato usa quelle già impostate fisse sul codice,
    ''' a seconda se l'utility era stata sviluppata per Aboca, Borgoluce, Bf, ...)
    ''' </summary>
    Public Property SiglaDirezione As enum_DirezioneFile

    Public Property ExportCdcRiga As Boolean
    Public Property DesClasseCdc As String
    Public Property DesClasseWbs As String
    Public Property DesAttivitaCosti As String
    Public Property DesAttivitaRicavi As String
    Public Property CollegamentoOrdineObbligatorio As Boolean
    Public Property OrdineEsportato As Boolean
    Public Property BloccaPostExport As Boolean

    Public Property UsaCategorieMerce As Boolean

    'Per richiamare WS in export
    Public Property UsaWS As Boolean

    ''' <summary>
    ''' -1 = lascia invariato il timeout di default (100secondi), altrimenti indicare il timeout desiderato in millisecondi
    ''' </summary>
    Public Property WSTimeOut As Integer
    Public Property WsEndpoint As String
    Public Property WsAuthEndpoint As String
    Public Property WsAuthorizationType As enum_TipoAutorizzazioneWS
    Public Property WsOAuth2 As Ws_Auth_OAUTH2
    Public Property WsAuthorization As Interscambio_Api_Authorization
    Public Property TokenBearer As String
    Public Property FileType As String
    Public Property EscludiErrori As Boolean
    Public Property AbilitaGiasDettaglioImportLog As Boolean
    Public Property AbilitaGiasDettaglioExportLog As Boolean
    Public Property AgendeConGestionePratica As Boolean
    Public Property PraticheStatiDaEsportare As Integer()
    Public Property PraticheStatoSuccesso As Integer
    Public Property PraticheStatoErrore As Integer

    Public Property UsaCausaleTrasporto As Boolean
    Public Property MostraSeOrdineChiuso As Boolean

    ''' <summary>
    ''' Se True, invece che utilizzare la piva destinataria del documento di import utilizza l'impresa indicata nella proprietà <see cref="CodificaPiva"/>
    ''' oppure se non specificata, sull'impresa superuser.
    ''' Per l'import dei contatti questa proprietà serve ad indicare la forzatura di piva proprietaria su cui scrivere il contatto
    ''' (a prescindere da sender e ImpresaPadre indicata sull'xml). La sostituzione di quello che è arrivato sull'xml con questa Piva viene fatto il prima possibile (sul WS prima che scriva il file),
    ''' in questo modo non c'è bisogno di mettere mano alla parte di processing vero e proprio dell'xml contatto
    ''' </summary>
    ''' <returns></returns>
    Public Property CodificaSuImpresaSpecifica As Boolean
    Public Property CodificaPiva As String

    ''' <summary>
    ''' Se attiva, in caso l'indirizzo indicato nel documento non sia quello presente in interscambio,
    ''' viene usato quest'ultimo invece di generare eccezione
    ''' </summary>
    ''' <returns></returns>
    Public Property TrasferisciIndirDefault As Boolean

    ''' <summary>
    ''' Se attiva, vengono cancellati i carichi di magazzino precedenti dell'azienda 
    ''' </summary>
    ''' <returns></returns>
    Public Property EliminaGiacenzePrecedenti As Boolean

    ''' <summary>
    ''' Se attiva, il documento eventualmente presente viene cancellato e poi riscritto mantenendo id_agenda ed id_mov di testata
    ''' </summary>
    ''' <returns></returns>
    Public Property PermettiSovrascrittura As Boolean

    ''' <summary>
    ''' Legge le operazioni di agenda la cui data movimento è maggiore o uguale alla data odierna meno questo valore
    ''' </summary>
    ''' <returns></returns>
    Public Property MovimentoNumeroGiorniPrecedenti As Integer

    ''' <summary>
    ''' Se attivata, indica di scrivere i documenti solo se sono considerati come fatturati nel file di importazione
    ''' </summary>
    ''' <returns></returns>
    Public Property ImportaSoloDocFatturati As Boolean

    ''' <summary>
    ''' Impostazione per gestire la divisione in due sottocategorie dei prodotti della categoria 191:
    ''' formulati con numero di registrazione ministeriale mappati automaticamente e
    ''' corroboranti senza numero di registrazione ministeriale che necessitano di mappatura manuale attraverso la tabella CAC
    ''' </summary>
    ''' <returns></returns>
    Public Property SdoppiaFitofarmaci As Boolean

    ''' <summary>
    ''' Valore per la sotto categoria di prodotti con num. ministeriale all'interno della nostra 191. Default = FIT
    ''' </summary>
    ''' <returns></returns>
    Public Property SottoCat_Fitofarmaci As String

    ''' <summary>
    ''' Valore per la sotto categoria di prodotti non registrati all'interno della nostra 191. Default = COR
    ''' </summary>
    ''' <returns></returns>
    Public Property SottoCat_Corroboranti As String

    ''' <summary>
    ''' Se attiva vengono eseguiti una serie di cambiamenti sul funzionamento regolare degli import/export per il caso specifico di Coldiretti/Demetra
    ''' </summary>
    Public Property ModalitaDemetra As Boolean

    ''' <summary>
    ''' Elenco dei CUAA a cui devono essere associati i contatti da esportare
    ''' se nothing = tutti
    ''' </summary>
    Public Property CUAAExport As String()

    ''' <summary>
    ''' Se il flag è attivo, tutti i contatti pubblici vengono inviati come fossero creati direttamente sotto l'impresa superuser
    ''' </summary>
    Public Property ContattiPubbliciSoloSuperuser As Boolean

    ''' <summary>
    ''' Se il flag è attivo, in import/export viene scambiato il cuaa al posto della piva
    ''' </summary>
    Public Property CuaaAsPiva As Boolean

    ''' <summary>
    ''' Indica il gestionale con il quale l'import/export si interfaccia
    ''' </summary>
    Public Property SistemaEsterno As enum_SistemiEsterni

    ''' <summary>
    ''' Indica da quale ambiente è partito l'export dei log ad ElasticSearch
    ''' </summary>
    Public Property Ambiente As String

    ''' <summary>
    ''' Indica quali log mandare ad ElasticSearch
    ''' </summary>
    Public Property FiltroEsportazione As Enum_FiltroEsportazione_to_ElasticSearch

    ''' <summary>
    ''' Indica quali log di testo da Scrivi_Log mandare ad ElasticSearch
    ''' </summary>
    Public Property ES_LogIntermedi As Enum_FiltroEsportazione_to_ElasticSearch

    ''' <summary>
    ''' Import contatti ver. CONT3: se attivo, l'attributo id per la risorsa umana è obbligatorio
    ''' </summary>
    Public Property IdRuolo As Boolean

    ''' <summary>
    ''' In import Mov Mag, blocca il comportamento di mantenere come sospesi i file per mappatura prodotti
    ''' </summary>
    Public Property BloccaRitentaPerProdotti As Boolean

    ''' <summary>
    ''' In import Mov Mag dei S/Carichi, se va in errore un prodotto nell'xml, continua ad elaborare gli altri (perché vengono create Agende separate)
    ''' </summary>
    Public Property ProseguiSCMAGPerErroriProdotti As Boolean

    Private Sub ValorizzaParametri(ByVal parametriExtra As String)

        Const nomeRoutine = "ParametriExtra.ValorizzaParametri"

        Try

            ConnessioneInterscambio = GetParametroStr(parametriExtra, "ConnessioneInterscambio", Nothing)
            'se non c'è quel parametro viene riscritto lo stesso valore della proprietà (assegnata col costruttore)
            DataFrom = GetParametroDate(parametriExtra, "DataFrom", DataFrom)
            DataTo = GetParametroDate(parametriExtra, "DataTo", DataTo)
            FlagCarico = GetParametroBool(parametriExtra, "Carico", FlagCarico)
            FlagScarico = GetParametroBool(parametriExtra, "Scarico", FlagScarico)
            StringLavCodExport = GetParametroStr(parametriExtra, "LavExport", Nothing)
            PivaMagExport = GetParametroStr(parametriExtra, "ExPiva", Nothing)
            SaCodMagExport = GetParametroInt(parametriExtra, "ExSaCod", SaCodMagExport)
            StringCodMagExport = GetParametroStr(parametriExtra, "ExDestString", Nothing)

            DirInput = GetParametroStr(parametriExtra, "DirI", DirInput)
            DirComplete = GetParametroStr(parametriExtra, "DirC", DirComplete)
            DirError = GetParametroStr(parametriExtra, "DirE", DirError)
            DirSaltaData = GetParametroStr(parametriExtra, "DirD", DirSaltaData)
            DirTemporanea = GetParametroStr(parametriExtra, "DirT", DirTemporanea)

            SuffissoAltroGestionale = GetParametroStr(parametriExtra, "Suffisso", SuffissoAltroGestionale)
            TipoImport = GetParametroInt(parametriExtra, "TipoImport", TipoImport)
            ContattoConIndirizzo = GetParametroBool(parametriExtra, "ContattoConIndirizzo", ContattoConIndirizzo)
            PivaInInterscambioProprietariaContatto = GetParametroBool(parametriExtra, "PivaInInterscambioProprietariaContatto", PivaInInterscambioProprietariaContatto)
            TipoChiave = GetParametroEnum(Of enum_TipoChiave)(parametriExtra, "TipoChiave", TipoChiave)
            TipoModulo = GetParametroInt(parametriExtra, "TipoModulo", TipoModulo)
            ParametriQualitativi = GetParametroInt(parametriExtra, "ParametriQualitativi", ParametriQualitativi)

            ConsentiGenerazioneAutoNumDoc = GetParametroBool(parametriExtra, "ConsentiGenerazioneAutoNumDoc", ConsentiGenerazioneAutoNumDoc)
            PrefissoDocDefault = GetParametroStr(parametriExtra, "PrefissoDocDefault", PrefissoDocDefault)
            SuffissoDocDefault = GetParametroStr(parametriExtra, "SuffissoDocDefault", SuffissoDocDefault)
            UsaSempreListino = GetParametroBool(parametriExtra, "UsaSempreListino", UsaSempreListino)
            DocumentoPrevisionale = GetParametroBool(parametriExtra, "DocumentoPrevisionale", DocumentoPrevisionale)

            GeneraNumAccettazione = GetParametroBool(parametriExtra, "GeneraNumAccettazione", GeneraNumAccettazione)
            PrefissoAccettazione = GetParametroStr(parametriExtra, "PrefissoAccettazione", PrefissoAccettazione)
            SuffissoAccettazione = GetParametroStr(parametriExtra, "SuffissoAccettazione", SuffissoAccettazione)

            NoMapCategoria = GetParametroBool(parametriExtra, "NoMapCategoria", NoMapCategoria)
            SdoppiaSementi = GetParametroBool(parametriExtra, "SdoppiaSementi", SdoppiaSementi)
            SottoCat_Sementi = GetParametroStr(parametriExtra, "SottoCat_Sementi", SottoCat_Sementi)
            SottoCat_Piantine = GetParametroStr(parametriExtra, "SottoCat_Piantine", SottoCat_Piantine)

            NoMapUdm = GetParametroBool(parametriExtra, "NoMapUdm", NoMapUdm)
            NoMapFitofarmaci = GetParametroBool(parametriExtra, "NoMapFitofarmaci", NoMapFitofarmaci)
            PrefissoFito = GetParametroStr(parametriExtra, "PrefissoFito", PrefissoFito)
            DerogaSuNoMapFito = GetParametroBool(parametriExtra, "DerogaSuNoMapFito", DerogaSuNoMapFito)
            ObbligaOP = GetParametroBool(parametriExtra, "ObbligaOP", ObbligaOP)
            PermettiOpEsterni = GetParametroBool(parametriExtra, "PermettiOpEsterni", PermettiOpEsterni)
            VerificaGiacenza = GetParametroBool(parametriExtra, "VerificaGiacenza", VerificaGiacenza)
            RifMovObbligatorio = GetParametroBool(parametriExtra, "RifMovObbligatorio", RifMovObbligatorio)

            LeggiIdDoc = GetParametroBool(parametriExtra, "LeggiIdDoc", LeggiIdDoc)
            IdDocUnivoco = GetParametroBool(parametriExtra, "IdDocUnivoco", IdDocUnivoco)

            Visibilita = GetParametroInt(parametriExtra, "Visibilita", Visibilita)
            QualificaDefDip = GetParametroInt(parametriExtra, "QualificaDefDip", QualificaDefDip)
            Piva = GetParametroStr(parametriExtra, "Piva", Piva)
            DataUltimaEsportazione = GetParametroDate(parametriExtra, "DataLastExp", DataUltimaEsportazione)
            DataFineEsportazione = GetParametroDate(parametriExtra, "DataFineExp", DataFineEsportazione)

            Agri = GetParametroBool(parametriExtra, "Agri", Agri)
            Zoo = GetParametroBool(parametriExtra, "Zoo", Zoo)
            Imputazioni = GetParametroBool(parametriExtra, "Impu", Imputazioni)

            ImputazioniAgri = GetParametroArrayInt(parametriExtra, "V_A", ImputazioniAgri)
            ImputazioniIndirette = GetParametroArrayInt(parametriExtra, "V_I", ImputazioniIndirette)
            ImputazioniZoo = GetParametroArrayInt(parametriExtra, "V_Z", ImputazioniZoo)

            AttivitaEscludi = GetParametroArrayInt(parametriExtra, "AttivitaEscludi", AttivitaEscludi)
            AttivitaServiziIdrici = GetParametroInt(parametriExtra, "AttivitaServiziIdrici", AttivitaServiziIdrici)
            AttivitaAffitto = GetParametroInt(parametriExtra, "AttivitaAffitto", AttivitaAffitto)

            Cod_Op = GetParametroBool(parametriExtra, "Cod_Op", Cod_Op)
            Cod_Traverso = GetParametroBool(parametriExtra, "Cod_Traverso", Cod_Traverso)
            Cod_Commessa = GetParametroBool(parametriExtra, "Cod_Commessa", Cod_Commessa)
            Superficie = GetParametroBool(parametriExtra, "Superficie", Superficie)
            LineItemNumber = GetParametroBool(parametriExtra, "LineItemNumber", LineItemNumber)

            InviaMail = GetParametroBool(parametriExtra, "InviaMail", InviaMail)
            Debug = GetParametroBool(parametriExtra, "Debug", Debug)
            Riepilogo = GetParametroBool(parametriExtra, "Riepilogo", Riepilogo)
            NoMailSaltati = GetParametroBool(parametriExtra, "NoMailSaltati", NoMailSaltati)
            FlagScriviSempreLotto = GetParametroBool(parametriExtra, "FlagScriviSempreLotto", FlagScriviSempreLotto)

            DataInizioFittizia = GetParametroDate(parametriExtra, "DataInizioFittizia", DataFineFittizia)
            DataFineFittizia = GetParametroDate(parametriExtra, "DataFineFittizia", DataFineFittizia)

            EscludiAttivitaNoMap = GetParametroBool(parametriExtra, "EscludiAttivitaNoMap", EscludiAttivitaNoMap)

            VersioneContatti = GetParametroEnum(Of enum_VersContatti)(parametriExtra, "VersioneContatti", VersioneContatti)
            VersioneProdotti = GetParametroEnum(Of enum_VersProdotti)(parametriExtra, "VersioneProdotti", VersioneProdotti)

            AppiattisciRuoli = GetParametroBool(parametriExtra, "AppiattisciRuoli", AppiattisciRuoli)
            UsaTipoRapportiContabili = GetParametroBool(parametriExtra, "UsaTipoRapportiContabili", UsaTipoRapportiContabili)
            IdAsProgressivo = GetParametroBool(parametriExtra, "IdAsProgressivo", IdAsProgressivo)
            RapportiContabiliExport = GetParametroArrayInt(parametriExtra, "RapportiContabiliExport", RapportiContabiliExport)
            TipiRappContabExport = GetParametroArrayString(parametriExtra, "TipiRappContabExport", TipiRappContabExport)

            ImportaSoloPrimoIndirizzo = GetParametroBool(parametriExtra, "ImportaSoloPrimoIndirizzo", ImportaSoloPrimoIndirizzo)

            UnisciNumDoc = GetParametroBool(parametriExtra, "UnisciNumDoc", UnisciNumDoc)
            IdinNomeFile = GetParametroBool(parametriExtra, "IdinNomeFile", IdinNomeFile)
            UsaGuida = GetParametroBool(parametriExtra, "UsaGuida", UsaGuida)

            NumPerFile = GetParametroInt(parametriExtra, "NumPerFile", NumPerFile)

            TraduciCodici = GetParametroBool(parametriExtra, "TraduciCodici", TraduciCodici)
            SiglaDirezione = GetParametroEnum(Of enum_DirezioneFile)(parametriExtra, "SiglaDirezione", SiglaDirezione)

            ExportCdcRiga = GetParametroBool(parametriExtra, "ExportCdcRiga", ExportCdcRiga)
            DesClasseCdc = GetParametroStr(parametriExtra, "DesClasseCdc", DesClasseCdc)
            DesClasseWbs = GetParametroStr(parametriExtra, "DesClasseWbs", DesClasseWbs)
            DesAttivitaRicavi = GetParametroStr(parametriExtra, "DesAttivitaRicavi", DesAttivitaRicavi)
            DesAttivitaCosti = GetParametroStr(parametriExtra, "DesAttivitaCosti", DesAttivitaCosti)
            CollegamentoOrdineObbligatorio = GetParametroBool(parametriExtra, "CollegamentoOrdineObbligatorio", CollegamentoOrdineObbligatorio)
            OrdineEsportato = GetParametroBool(parametriExtra, "OrdineEsportato", OrdineEsportato)
            BloccaPostExport = GetParametroBool(parametriExtra, "BloccaPostExport", BloccaPostExport)

            UsaCategorieMerce = GetParametroBool(parametriExtra, "UsaCategorieMerce", UsaCategorieMerce)

            UsaWS = GetParametroBool(parametriExtra, "UsaWS", UsaWS)
            FileType = GetParametroStr(parametriExtra, "FileType", FileType)
            WSTimeOut = GetParametroInt(parametriExtra, "WSTimeOut", WSTimeOut)
            WsEndpoint = GetParametroStr(parametriExtra, "WsEndpoint", WsEndpoint)
            WsAuthEndpoint = GetParametroStr(parametriExtra, "WsAuthEndpoint", WsAuthEndpoint)
            WsAuthorizationType = GetParametroEnum(Of enum_TipoAutorizzazioneWS)(parametriExtra, "WsAuthorizationType", WsAuthorizationType)
            WsOAuth2 = GetParametroJsonObject(Of Ws_Auth_OAUTH2)(parametriExtra, "WsOAuth2", WsOAuth2)
            WsAuthorization = GetParametroJsonObject(Of Interscambio_Api_Authorization)(parametriExtra, "WsAuthorization", WsAuthorization)
            EscludiErrori = GetParametroBool(parametriExtra, "EscludiErrori", EscludiErrori)
            AbilitaGiasDettaglioImportLog = GetParametroBool(parametriExtra, "AbilitaGiasDettaglioImportLog", AbilitaGiasDettaglioImportLog)
            AbilitaGiasDettaglioExportLog = GetParametroBool(parametriExtra, "AbilitaGiasDettaglioExportLog", AbilitaGiasDettaglioExportLog)


            AgendeConGestionePratica = GetParametroBool(parametriExtra, "AgendeConGestionePratica", AgendeConGestionePratica)
            PraticheStatiDaEsportare = GetParametroArrayInt(parametriExtra, "PraticheStatiDaEsportare", PraticheStatiDaEsportare)
            PraticheStatoSuccesso = GetParametroInt(parametriExtra, "PraticheStatoSuccesso", PraticheStatoSuccesso)
            PraticheStatoErrore = GetParametroInt(parametriExtra, "PraticheStatoErrore", PraticheStatoErrore)

            UsaCausaleTrasporto = GetParametroBool(parametriExtra, "UsaCausaleTrasporto", UsaCausaleTrasporto)
            MostraSeOrdineChiuso = GetParametroBool(parametriExtra, "MostraSeOrdineChiuso", MostraSeOrdineChiuso)

            CodificaSuImpresaSpecifica = GetParametroBool(parametriExtra, "CodificaSuImpresaSpecifica", CodificaSuImpresaSpecifica)
            CodificaPiva = GetParametroStr(parametriExtra, "CodificaPiva", CodificaPiva)

            TrasferisciIndirDefault = GetParametroBool(parametriExtra, "TrasferisciIndirDefault", TrasferisciIndirDefault)

            EliminaGiacenzePrecedenti = GetParametroBool(parametriExtra, "EliminaGiacenzePrecedenti", EliminaGiacenzePrecedenti)
            PermettiSovrascrittura = GetParametroBool(parametriExtra, "PermettiSovrascrittura", PermettiSovrascrittura)
            MovimentoNumeroGiorniPrecedenti = GetParametroInt(parametriExtra, "MovimentoNumeroGiorniPrecedenti", MovimentoNumeroGiorniPrecedenti)
            ImportaSoloDocFatturati = GetParametroBool(parametriExtra, "ImportaSoloDocFatturati", ImportaSoloDocFatturati)

            SdoppiaFitofarmaci = GetParametroBool(parametriExtra, "SdoppiaFitofarmaci", SdoppiaFitofarmaci)
            SottoCat_Fitofarmaci = GetParametroStr(parametriExtra, "SottoCat_Fitofarmaci", SottoCat_Fitofarmaci)
            SottoCat_Corroboranti = GetParametroStr(parametriExtra, "SottoCat_Corroboranti", SottoCat_Corroboranti)

            ModalitaDemetra = GetParametroBool(parametriExtra, "ModalitaDemetra", ModalitaDemetra)
            CUAAExport = GetParametroArrayString(parametriExtra, "CUAAExport", CUAAExport)
            ContattiPubbliciSoloSuperuser = GetParametroBool(parametriExtra, "ContattiPubbliciSoloSuperuser", ContattiPubbliciSoloSuperuser)
            CuaaAsPiva = GetParametroBool(parametriExtra, "CuaaAsPiva", CuaaAsPiva)
            SistemaEsterno = GetParametroEnum(Of enum_SistemiEsterni)(parametriExtra, "SistemaEsterno", SistemaEsterno)

            IdRuolo = GetParametroBool(parametriExtra, "IdRuolo", IdRuolo)

            FiltroEsportazione = GetParametroEnum(Of Enum_FiltroEsportazione_to_ElasticSearch)(parametriExtra, "FiltroEsportazione", FiltroEsportazione)
            ES_LogIntermedi = GetParametroEnum(Of Enum_FiltroEsportazione_to_ElasticSearch)(parametriExtra, "ES_LogIntermedi", ES_LogIntermedi)
            Ambiente = GetParametroStr(parametriExtra, "Ambiente", Ambiente)

            BloccaRitentaPerProdotti = GetParametroBool(parametriExtra, "BloccaRitentaPerProdotti", BloccaRitentaPerProdotti)

            ProseguiSCMAGPerErroriProdotti = GetParametroBool(parametriExtra, "ProseguiSCMAGPerErroriProdotti", ProseguiSCMAGPerErroriProdotti)

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Shared Function GetParametroStr(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As String) As String

        Const nomeRoutine = "ParametriExtra.GetParametroStr"
        Dim parametro As String = ""

        Try
            Dim vPara As String() = parametriExtra.Split("|")

            Dim item As String = Array.Find(vPara, Function(x) x.StartsWith(toSeek & "="))

            '07/2024 Amoroso: in questo if precedentemente il valore della chiave veniva recuperato semplicemente così:
            '  parametro = item.Split("=")(1)
            'Ora però c'è l'esigenza di avere un parametro il cui valore contenga l'uguale,
            'quindi aggiungo la variabile vParamVal ed un if per il quale se gli elementi trovati sono più di 2, il valore lo considero
            'come tutto il valore della stringa meno la chiave, altrimenti lo recupero come prima.
            'vParamVal e il relativo if sono per sicurezza in mancanza di tempo per testare su tutti i casi,
            'probabilmente si potrebbe direttamente sostituire lo split con il replace.
            If item IsNot Nothing Then

                Dim vParamVal As String() = item.Split("=")

                If vParamVal.Length > 2 Then
                    parametro = item.Replace(toSeek & "=", "")
                Else
                    parametro = vParamVal(1)
                End If

            ElseIf valDefault IsNot Nothing Then
                parametro = valDefault
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try

        Return parametro
    End Function

    Private Shared Function GetParametroDate(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As Date?) As Date?

        Const nomeRoutine = "ParametriExtra.GetParametroDate"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)

            If parStr Is Nothing OrElse parStr = "" Then
                Return valDefault
            ElseIf Not IsDate(parStr) Then
                Return Nothing
            Else
                Return CDate(parStr)
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try

    End Function

    Private Shared Function GetParametroBool(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As Boolean?) As Boolean?
        Const nomeRoutine = "ParametriExtra.GetParametroBool"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)

            If parStr IsNot Nothing AndAlso parStr <> "" AndAlso IsNumeric(parStr) Then
                Return CBool(parStr)
            Else
                Return valDefault
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

    Private Shared Function GetParametroInt(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As Integer?) As Integer?
        Const nomeRoutine = "ParametriExtra.GetParametroInt"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)

            If parStr IsNot Nothing AndAlso parStr <> "" AndAlso IsNumeric(parStr) Then
                Return CInt(parStr)
            Else
                Return valDefault
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

    Private Shared Function GetParametroArrayInt(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As Integer()) As Integer()
        Const nomeRoutine = "ParametriExtra.GetParametroArrayInt"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)
            Dim intArray As Integer() = valDefault

            If parStr IsNot Nothing AndAlso parStr <> "" Then
                Dim stringArray As String() = parStr.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

                If stringArray IsNot Nothing AndAlso stringArray.Length > 0 Then
                    intArray = Array.ConvertAll(stringArray, Function(x) Integer.Parse(x))
                End If
            End If

            Return intArray
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

    Private Shared Function GetParametroArrayString(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As String()) As String()
        Const nomeRoutine = "ParametriExtra.GetParametroArrayString"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)
            Dim strArray As String() = valDefault

            If parStr IsNot Nothing AndAlso parStr <> "" Then
                strArray = parStr.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
            End If

            Return strArray
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

    Private Shared Function GetParametroEnum(Of T)(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As T) As T
        Const nomeRoutine = "ParametriExtra.GetParametroEnum"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)

            If parStr IsNot Nothing AndAlso parStr <> "" Then
                Return [Enum].Parse(GetType(T), parStr)
            Else
                Return valDefault
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

    Private Shared Function GetParametroJsonObject(Of T)(ByVal parametriExtra As String, ByVal toSeek As String, ByVal valDefault As T) As T
        Const nomeRoutine = "ParametriExtra.GetParametroJsonObject"

        Try
            Dim parStr As String = GetParametroStr(parametriExtra, toSeek, Nothing)

            If parStr IsNot Nothing AndAlso parStr <> "" Then
                Return JsonConvert.DeserializeObject(parStr, GetType(T))
            Else
                Return valDefault
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] " & toSeek & " : " & ex.Message)
        End Try
    End Function

End Class
