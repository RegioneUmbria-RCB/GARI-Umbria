import { DatePipe, DecimalPipe } from '@angular/common';
import { Inject, Injectable, LOCALE_ID, OnDestroy, Renderer2 } from '@angular/core';
import {
    AbstractControl,
    FormArray,
    FormBuilder,
    FormControl,
    FormGroup,
    ValidationErrors,
    ValidatorFn,
    Validators
} from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { RapportoContabile } from 'app/Model/anagrafiche/RapportoContabile';
import { RisorseUmane } from '../../../Model/anagrafiche/RisorseUmane';
import { Attivita, Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import { AttivitaPersonalizzata } from 'app/Model/attivita/AttivitaPersonalizzata';
import { EsercizioCDC } from 'app/Model/attivita/centri_di_costo/EsercizioCDC';
import { DettaglioFertilizzazione } from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { DettaglioSemina } from 'app/Model/attivita/dettagli/DettaglioSemina';
import {DettaglioTrattamento, enum_Ripartizione_Trappole} from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { Prodotto } from 'app/Model/attivita/risorse/Prodotto';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import {
    AGRODATAFINE,
    AGRODATAINIZIO,
    DpiBio, ElencoLavCodRilieviSenzaImpianti,
    FERTILIZZANTI,
  FORMULATI, INNESCHI,
    INSETTI, NessunaSpecieQdC,
    NessunDpi,
    NessunDpiNessunaEtichetta,
    SEMENTI,
    TRASFORMATI_VEGETALI
} from 'app/Model/CostantiPersonalizzate';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { Copertura } from 'app/Model/metaschema/Copertura';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { PrincipioAttivo } from 'app/Model/metaschema/PrincipioAttivo';
import { RegolamentoConcimazione } from 'app/Model/metaschema/RegolamentoConcimazione';
import { GruppoFinalita } from 'app/Model/metaschema/utilizzi/GruppoFinalita';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import {
  enum_Cod_Regolamento,
  enum_doseQuantitaTotale,
  enum_Gestione_Giacenze,
  enum_Gestione_Lotti,
  enum_LAVCOD,
  enum_OrigineApp,
  enum_PUARegolamenti_Tipo,
  enum_Security_Attivita,
  enum_SEMINA_TIPO,
  enum_TipoOperazioneDB,
  enum_UnitaMisura, enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO,
  Tipo_Polverulento
} from 'app/Model/TipiEnumerativi';
import { ImpostazioniAziendeCentriService } from '../../../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import {
    AgendaService,
    Attivita_Con_Parametri_Aggiuntivi,
    Inizializza_QdC,
    LeggiDefault_DPI_QdC,
    LeggiInizializza_QdC,
    LeggiPUA
} from 'app/Service/Agenda/Agenda.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogAction, Dialog_Type, GiasDialogService } from 'app/Service/gias-dialog.service';
import {
    enum_ErroreGias_Tipo,
    ErroreGias,
    ErroreGias_Severity,
    MasterService,
    rispostaStandard
} from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import {Enum_DBTypeOperation, enum_TipoControllo, DataValidator, MultiSelectValidator, GiasPanelBar} from 'gias-ui-kit';
import { KendoGridRow, NumericSettings, GridDataWithFilter, HttpAction } from 'gias-kendo-grid';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { Elenco_Opzioni_Semina } from '../componenti/prodotti/controlli-comuni/opzioni-semina/elenco-opzioni-semina';
import {
  Acqua,
  CodiciXOperazione, Dettaglio_Formulato,
  DropdownListAttivitaPersonalizzata,
  DropdownListAvversita,
  DropdownListCampo,
  DropdownListDisciplinare,
  DropdownListMagazzino, enum_Problema_DettaglioProdotto,
  GridImpiantoSelezionatoModel,
  GridMacchinaModel, MultiColumnComboboxAvversitaInnesco,
  MultiColumnComboboxDose_Etichetta,
  MultiColumnComboboxFertilizzazione,
  MultiColumnComboboxSemina,
  MultiColumnComboboxTrattamento,
  Obj_Errore_Gias_QdC,
  QdCFormModel,
  Sezione_Prodotto_Fertilizzanti,
  Sezione_Prodotto_Formulati,
  Sezione_Prodotto_Raccolta,
  Sezione_Prodotto_Sementi,
  Sezione_Rilievi,
  Superfici,
  Testata
} from '../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import { DestinazioneUso } from '../../../Model/metaschema/utilizzi/DestinazioneUso';
import { Varieta } from '../../../Model/metaschema/utilizzi/Varieta';
import { UtilizzoTerreno } from '../../../Model/metaschema/utilizzi/UtilizzoTerreno';
import { Impresa } from '../../../Model/anagrafiche/Impresa';
import { LeggiProfilazione, ProfilazioneService } from '../../../Service/Agenda/profilazione.service';
import { ExpansionPanelActionEvent } from '@progress/kendo-angular-layout';
import { UnitaDiMisura } from '../../../Model/metaschema/UnitaDiMisura';
import { UnitaDiMisuraService } from '../../../Service/Metaschema/UnitaDiMisura.service';
import { CookieService } from '../../../Service/cookie.service';
import { GisClient, VerificaEsistenzaEntitaPerImpianti_In } from '../../../Service/api.service';
import {
    enum_Generazione_Lotto_Raccolta,
    enum_Opzioni_Raccolta_Aggiornamento_Anagrafica,
    enum_Ripartizione_Raccolta
} from '../componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model';
import { BufferZone } from '../../../Model/metaschema/BufferZone';
import { CentraMappa, LatLng } from 'app/Model/GIS/Utility';
import { GisService } from 'app/GIS/GIS.service';
import { GridDosiProdottiValidator } from '../validators/grid-dosi-prodotti-validator';
import { UtilityFunctions } from '../../../Utility/UtilityFunctions';
import { GestioneRichiesteService } from "../../../Service/gestione-richieste.service";
import { Fabbricato } from "../../../Model/anagrafiche/Fabbricato";
import { GruppoNoteModel, NotaInterventoDdlItem } from "../componenti/grid-note/note.model";
import { RilevamentoDiMagazzino } from "../../../Model/attivita/RilevamentoDiMagazzino";
import { RibaltamentoTypes } from "../../../menu-agenda/components/utils";
import { Utente } from 'app/Model/utente/utente';
import { cloneDeep } from "lodash";
import { Pua } from "../../../Model/metaschema/Pua";
import { Blocco, Tipo_Blocco } from "../../../Model/attivita/Blocco";
import { RisorsaRegistrazione } from "../../../Model/attivita/risorse/RisorsaRegistrazione";
import { Utente_Impostazioni } from "../../../Model/utente/utente_impostazioni";
import { ProdottoDaTrattareCDC } from 'app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC';
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import { LeggiOperazione, OperazioneClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgenda, Stati, Tipo_Attivita } from 'gias-ui-kit';
import { DettaglioRilievo } from "../../../Model/attivita/dettagli/DettaglioRilievo";
import {SupTrattataValidator} from "../validators/sup-trattata-validator";
import {QuantitaSuImpianto} from "../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {AvversitaGruppo} from "../../../Model/metaschema/avversita/AvversitaGruppo";

export enum enum_trattamentoTabIndex {
    PianoColturale = 0,
    PianoColturaleGrafico = 1
}

export enum enum_Modalita_Inserimento_Prodotti {
    Non_In_Griglia = 0,
    In_Griglia = 1
}

export enum enum_Stato_Innesco {
  Valido = 0,
  In_Scadenza = 1,
  Scaduto = 2
}

export class Redirect_To_GiasNG_Page {
    objParametriAgenda: ObjParametriAgenda;
    QdCFormValue: QdCFormModel;

    constructor() {
        this.objParametriAgenda = null;
        this.QdCFormValue = null;
    }
}

export class GISPanelQdC {
    Panel: GiasPanelBar;
    ExpandPanel: boolean;
    Polygon: boolean;

    constructor() {
        this.Panel = null;
        this.ExpandPanel = false;
        this.Polygon = false;
    }
}

@Injectable()

export class QdCService implements OnDestroy {

    private ListaAttivitaLetta: BehaviorSubject<Attivita_Con_Parametri_Aggiuntivi[]> = new BehaviorSubject([]);

    //causali
    public listaOperazioniCausali = [];

    private Modalita_Inserimento_Prodotti: BehaviorSubject<enum_Modalita_Inserimento_Prodotti> = new BehaviorSubject<enum_Modalita_Inserimento_Prodotti>(enum_Modalita_Inserimento_Prodotti.Non_In_Griglia);

    public GISPanel: GISPanelQdC = new GISPanelQdC();

    public ImpiantiPanelBar: GiasPanelBar = null;

    public TrattamentoTabStripSelected: Subject<number> = new Subject<number>();

    private UdM_Product_Numeric_Settings = [];

    public digitsInfo_QdC_4_Decimal = "1.0-"+this.get4DecimalNumericSettings().decimals;

    public digitsInfo_QdC_Percentuale = "1.0-2";

    public digitsInfo_QdC_0_Decimal = "1.0-0";

    Obj_NessunDpiNessunaEtichetta = <Disciplinare>{
        codice: NessunDpiNessunaEtichetta,
        descrizione: this.translocoService.translate("NessunDisciplinareNessunaEtichetta"),
        disciplinarePubblicoPrivato: 0,
        flagProtetto: 0,
        gruppoFinalita: null,
        idTr: 0,
        raggruppamentiColturaliDPI: null,
        regolamentoConcimazione: <RegolamentoConcimazione>{
            codice: +NessunDpiNessunaEtichetta,
            descrizione: this.translocoService.translate("NessunDisciplinareNessunaEtichetta"),
            tipo: 0
        }
    };

    Obj_NessunDpi = <Disciplinare>{
        codice: NessunDpi,
        descrizione: this.translocoService.translate("SoloEtichetta"),
        disciplinarePubblicoPrivato: 0,
        flagProtetto: 0,
        gruppoFinalita: null,
        idTr: 0,
        raggruppamentiColturaliDPI: null,
        regolamentoConcimazione: <RegolamentoConcimazione>{
            codice: +NessunDpi,
            descrizione: this.translocoService.translate("SoloEtichetta"),
            tipo: 0
        }
    };

    Obj_DpiBIO = <Disciplinare>{
        codice: DpiBio,
        descrizione: this.translocoService.translate("BIO"),
        disciplinarePubblicoPrivato: 0,
        flagProtetto: 0,
        gruppoFinalita: null,
        idTr: 0,
        raggruppamentiColturaliDPI: null,
        regolamentoConcimazione: <RegolamentoConcimazione>{
            codice: +DpiBio,
            descrizione: this.translocoService.translate("BIO"),
            tipo: 0
        }
    };

    //Creato questo oggetto vuoto per permettere all'utente di scrivere nella multicolumncombobox e poi di ricercare quello
    //che ha scritto
    Obj_Empty_MultiColumnComboboxTrattamento = <MultiColumnComboboxTrattamento>{
        Descrizione_Concatenata: "",
        Codice_Concatenato: "",
        prodotto: <Prodotto>{
            codice: 0,
            descrizione: ""
        },
        MagazziniMovimentazioni: [],
        classificazioni: "",
        bufferzone: <BufferZone>{
            minimo: 0,
            massimo: 0
        },
        dataSmaltimentoScorte: AGRODATAINIZIO,
        tempoCarenza: 0,
        principiAttivi: [],
        polverulento: 0,
        Polverulento_Str: "",
        Tutti_Problemi_DettaglioProdotto: [],
        Problema_DettaglioProdotto_Da_Risolvere: enum_Problema_DettaglioProdotto.Nessuno,
        isImpollinatore: false,
        impollinatore: "",
        durataFeromone: 0,
        scadenzaFeromone: AGRODATAINIZIO,
        classType: "DettaglioTrattamento"
    };

    //Creato questo oggetto vuoto per permettere all'utente di scrivere nella multicolumncombobox e poi di ricercare quello
    //che ha scritto
    Obj_Empty_MultiColumnComboboxFertilizzazione = <MultiColumnComboboxFertilizzazione>{
        Descrizione_Concatenata: "",
        Codice_Concatenato: "",
        prodotto: <Prodotto>{
            codice: 0,
            descrizione: ""
        },
        MagazziniMovimentazioni: [],
        tipologieFertilizzante: [],
        N_Str: "",
        P_Str: "",
        K_Str: "",
        Cu_Str: "",
        N: null,
        P: null,
        K: null,
        Cu: null,
        effluente: null,
        Tutti_Problemi_DettaglioProdotto: [],
        Problema_DettaglioProdotto_Da_Risolvere: enum_Problema_DettaglioProdotto.Nessuno,
        classType: "DettaglioFertilizzazione"
    };

    //Creato questo oggetto vuoto per permettere all'utente di scrivere nella multicolumncombobox e poi di ricercare quello
    //che ha scritto
    Obj_Empty_MultiColumnComboboxSemina = <MultiColumnComboboxSemina>{
        Descrizione_Concatenata: "",
        Codice_Concatenato: "",
        prodotto: <Prodotto>{
            codice: 0,
            descrizione: ""
        },
        MagazziniMovimentazioni: [],
        codArticolo: "",
        Tutti_Problemi_DettaglioProdotto: [],
        Problema_DettaglioProdotto_Da_Risolvere: enum_Problema_DettaglioProdotto.Nessuno,
        classType: "DettaglioSemina"
    };

    Obj_TuttiICentriAziendali = <CentroAziendale>{
        primaryKey: {
            codice: 0,
            partitaIva: this.objParametriAgendaService.getObjParamValue().Piva
        },
        nome: this.translocoService.translate('TuttiICentriAziendali')
    };

    Obj_NessunaSpecie = <Specie>{
        codice: NessunaSpecieQdC,
        descrizione: this.translocoService.translate('NessunaSpecie')
    };

    Apertura_QdC = {
        flag_QdC_Aperta_da_Altra_Pagina: false,
        flag_Selezione_Impianti: false
    }

    abilitaGrid: boolean = true;

    //Per caricare l'interfaccia solo quando il Redirect è stato completato
    Mostra_QdC: boolean = false;

    //Variabile in cui mostro o nascondo la sezione prodotti
    Mostra_Sezioni_Prodotto: boolean = false;

    // Gestisce visualizzazione delle sezioni senza prodotti
    Mostra_Sezioni_Senza_Prodotto: boolean = false;

    //Ottiene default Disciplinare da Impostazione utente / Imprese_Codici
    Default_DPI_da_Impostazione: DropdownListDisciplinare = null;

    MostrabtnSalva: boolean = true;

    //Oggetto che mi serve avere valorizzato appena carico la pagina
    obj_Inizializza_QdC = new Inizializza_QdC();

    //Descrizione da visualizzare sotto la Acqua_Ha
    public obj_Acqua_Provenienza = {
        Descrizione: "",
        flag_from_taratura_uggello: false,
        flag_from_dosi: false
    };

    objParametriAgenda: ObjParametriAgenda;

    public MessaggioImpiantiVisualizzato = {
        DPI: false,
        Dichiarazione_Non_Utilizzo: false
    };

    // BehaviorSubject emesso al termine dell'aggiornamento degli esercizi selezionati
    // nel servizio grid-impianti
    public SelezioneEsercizi = new BehaviorSubject<FormArray>(undefined);

    // BehaviorSubject emesso al termine di AggiornaRigheSelezionateGriglia
    public eventiPostSelectionChangeGridImpianti = new BehaviorSubject<any>(undefined);

    // Elenco centri aziendale collegato da QdCTestataService

    public verificaCompatibilitaMicroirrigazione: boolean = false;

    public elencoCentriAziendali: CentroAziendale[] = [];

    public Opzioni_Ripartizione_Trappole: Array<BaseCodeDescr> = [];

    /** Valore dell'impostazione filtro lotti (181) per la determinazione dell'obbligatorietà del lotto per i vari Elem_Cod.
     *  0 = nessuna gestione, 1 = gestione obbligatoria, 2 = gestione facoltativa
     */
    public static filtroLotti: string;

    private toNumber = (stringNumber: string) => +stringNumber;
    private equalsOne = (stringNumber: string) => +stringNumber === 1;

    constructor(private PermessiUtenteService: PermessiUtenteService,
        private objParametriAgendaService: ObjParametriAgendaService,
        public funzionicomuniservice: FunzioniComuniService,
        public decimalpipe: DecimalPipe,
        public datepipe: DatePipe,
        private fb: FormBuilder,
        private agendaservice: AgendaService,
        public impostazioniaziendecentriService: ImpostazioniAziendeCentriService,
        public translocoService: TranslocoService,
        private renderer: Renderer2,
        private giasdialogservice: GiasDialogService,
        @Inject(LOCALE_ID) public locale_id: string,
        private profilazioneservice: ProfilazioneService,
        private unitadimisuraservice: UnitaDiMisuraService,
        private cookieService: CookieService,
        private gisClient: GisClient,
        private gisService: GisService,
        private gestioneRichiesteService: GestioneRichiesteService,
        public masterService: MasterService,
        private operazioneCausaleService: OperazioneClient
    ) {
        this.Obj_Empty_MultiColumnComboboxTrattamento = this.setCodDescrMultiColumnComboboxProdotto(FORMULATI, this.Obj_Empty_MultiColumnComboboxTrattamento) as MultiColumnComboboxTrattamento;
        this.Obj_Empty_MultiColumnComboboxFertilizzazione = this.setCodDescrMultiColumnComboboxProdotto(FERTILIZZANTI, this.Obj_Empty_MultiColumnComboboxFertilizzazione) as MultiColumnComboboxFertilizzazione;
        this.Obj_Empty_MultiColumnComboboxSemina = this.setCodDescrMultiColumnComboboxProdotto(SEMENTI, this.Obj_Empty_MultiColumnComboboxSemina) as MultiColumnComboboxSemina;

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.Opzioni_Ripartizione_Trappole.push(new BaseCodeDescr(enum_Ripartizione_Trappole.Manuale,this.translocoService.translate("Manuale")));

        this.Opzioni_Ripartizione_Trappole.push(new BaseCodeDescr(enum_Ripartizione_Trappole.Automatica,this.translocoService.translate("qdc.RaccoltaAutomaticaSuperficie")));

        QdCService.filtroLotti = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(
            this.masterService.objP_utenti.PivaSuperUser, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI,
            TRASFORMATI_VEGETALI
        );
    }

    Elenco_Operazioni_Fertilizzanti = [enum_LAVCOD.DISTRIBUZIONE_CONCIME, enum_LAVCOD.SARCHIATURA_CONCIMAZIONE, enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI,
    enum_LAVCOD.CONCIMAZIONE_FOGLIARE, enum_LAVCOD.FERTIRRIGAZIONE, enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA];

    Elenco_Operazioni_Formulati = [enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO, enum_LAVCOD.DISERBO, enum_LAVCOD.DISSECCAMENTO,
    enum_LAVCOD.GEODISINFESTAZIONE, enum_LAVCOD.CONCIA_SEME, enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE, enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE,
        enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA,enum_LAVCOD.INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, enum_LAVCOD.REINNESCO_TRAPPOLE];

    //Vengono gestiti graficamente come i formulati anche se hanno un elem_cod differente
    Elenco_Operazioni_Insetti = [enum_LAVCOD.DISTRIBUZIONE_INSETTI];

    Elenco_Operazioni_Trasformati_Vegetali = [enum_LAVCOD.RACCOLTA];

    Elenco_Operazioni_Sementi = [enum_LAVCOD.SEMINA, enum_LAVCOD.TRAPIANTO,
    enum_LAVCOD.SOVESCIO, enum_LAVCOD.SOD_SEDDING];

    Elenco_Operazioni_Non_Utilizzo = [enum_LAVCOD.FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO, enum_LAVCOD.TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO];

    Elenco_Operazioni_Abbattimento = [enum_LAVCOD.ABBATTIMENTOIMPIANTI];

    //Lavorazioni
    Elenco_Operazioni_Lavorazioni = [enum_LAVCOD.ANDANAMENTO, enum_LAVCOD.ARATURA, enum_LAVCOD.ASPORTAZIONE_ORGANI_INFETTI, enum_LAVCOD.ASSOLCATURA,
    enum_LAVCOD.CARICO_MANUALE_FRUTTA, enum_LAVCOD.CIMATURA, enum_LAVCOD.DIRADAMENTO_MANUALE, enum_LAVCOD.DISSODAMENTO,
    enum_LAVCOD.ERPICATURA, enum_LAVCOD.ESTIRPATURA, enum_LAVCOD.ESPIANTO, enum_LAVCOD.FALCIACONDIZIONATURA, enum_LAVCOD.FALCIATURA_ERBAI,
    enum_LAVCOD.FORMAZIONE_ARGINELLI, enum_LAVCOD.FRANGIZOLLATURA, enum_LAVCOD.FRESATURA, enum_LAVCOD.IMBALLO_FIENO_ROTOLI,
    enum_LAVCOD.INTERRAMENTO_PAGLIE, enum_LAVCOD.LAVORAZIONE_TRA_FILA, enum_LAVCOD.LAVORAZIONE_SU_FILA, enum_LAVCOD.LEGATURA,
    enum_LAVCOD.LIVELLAMENTO, enum_LAVCOD.MANUTENZIONE_ARGINI, enum_LAVCOD.MESSA_DIMORA_PIANTE, enum_LAVCOD.MIETITREBBIATURA,
    enum_LAVCOD.MINIMUM_TILLAGE, enum_LAVCOD.PACCIAMATURA, enum_LAVCOD.POTATURA_SECCA, enum_LAVCOD.POTATURA_VERDE, enum_LAVCOD.PRESSATURA,
    enum_LAVCOD.RACCOLTA_LEGNA_POTATURA, enum_LAVCOD.RACCOLTA_MANUALE, enum_LAVCOD.RACCOLTA_MECCANICA, enum_LAVCOD.RANGHINATURA,
    enum_LAVCOD.RINCALZATURA, enum_LAVCOD.RIPPATURA, enum_LAVCOD.RIPUNTATURA, enum_LAVCOD.RIVOLTAMENTO_FORAGGIO, enum_LAVCOD.RULLATURA,
    enum_LAVCOD.SARCHIATURA, enum_LAVCOD.SCARIFICATURA, enum_LAVCOD.SCASSO, enum_LAVCOD.TRINCIATURA, enum_LAVCOD.VANGATURA, enum_LAVCOD.ZAPPATURA,
    enum_LAVCOD.GEBIATURA, enum_LAVCOD.ROMPICROSTA, enum_LAVCOD.LAVORAZIONE_CONBINATA, enum_LAVCOD.ERPICATURA_ROTANTE, enum_LAVCOD.INTERVENTO_ANTIBRINA,
    enum_LAVCOD.STRIGLIATURA, enum_LAVCOD.PIRODISERBO, enum_LAVCOD.PASCOLAMENTO_PROPRIO, enum_LAVCOD.PASCOLAMENTO_TERZI];

    Elenco_Operazioni_con_Acqua = [enum_LAVCOD.CONCIMAZIONE_FOGLIARE, enum_LAVCOD.FERTIRRIGAZIONE, enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA,
    enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO, enum_LAVCOD.DISERBO, enum_LAVCOD.DISSECCAMENTO,
    enum_LAVCOD.GEODISINFESTAZIONE, enum_LAVCOD.CONCIA_SEME, enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE, enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE,
    enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA];

    Elenco_Operazioni_Rilievi = [enum_LAVCOD.DANNI_RACCOLTA, enum_LAVCOD.FASI_FENOLOGICHE, enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO, enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA,
    enum_LAVCOD.RILIEVO_INDICI_MATURITA, enum_LAVCOD.RILIEVO_ERBE_INFESTANTI,enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE];

    Elenco_Operazioni_Compatibili_Con_Visite = [
        enum_LAVCOD.FASI_FENOLOGICHE,
        enum_LAVCOD.RILIEVO_ERBE_INFESTANTI,
        enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO,
        //enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE,
        enum_LAVCOD.RILIEVO_INDICI_MATURITA,
        enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA,
        enum_LAVCOD.DANNI_RACCOLTA,
    ];

    Elenco_Operazioni_Con_Gestione_TrappoleFormulati = [
      enum_LAVCOD.INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
      enum_LAVCOD.REINNESCO_TRAPPOLE
    ];

    /*
    * @description:
    * Elenco di Unita di Misura che sono di sono già scritte su db con /ha alla fine e che non hanno bisogno di aggiungerlo
    * */
    Elenco_UdM_Radice_Con_Ha = [];

    /*
* @description:
* Elenco di Unita di Misura che sono di sono già scritte su db con /hl alla fine e che non hanno bisogno di aggiungerlo
* */
    Elenco_UdM_Radice_Con_Hl = [];

    /*
    * @description:
    * Elenco di operazioni che non hanno a video la dropdown del disciplinare ma che comunque da sotto
    * nel modello hanno impostato NessunDPI
    * */
    Elenco_Operazioni_Con_Default_DPI = [enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE,
      enum_LAVCOD.INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, enum_LAVCOD.REINNESCO_TRAPPOLE,
      enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA,enum_LAVCOD.CONCIA_SEME, enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE];

    Utente: Utente;

    private _QdCForm: FormGroup;

    Mostra_Avversita_Prima_Dei_Prodotti: boolean = false;

    //Impostati di default a true per caricare il default della Specie dai cookie
    CONSIDERATERRENONUDO: boolean = true;

    DETTAGLITERRENONUDO: boolean = true;
    leggiAncheImpiantiBloccati: boolean = false;
    visualizza_Kpin_BlockName = false;
    visualizza_codici_imp_app_prj = false;
    flag_MostraBtnSalvaCDG: boolean = false;
    Impostazione_Mag: string;
    ColonneVisibiliGridImpianti: Array<string>;
    GestioneLotti_Formulati: number = enum_Gestione_Lotti.Nessuna;
    GestioneLotti_Fertilizzanti: number = enum_Gestione_Lotti.Nessuna;
    GestioneLotti_Sementi: number = enum_Gestione_Lotti.Obbligatoria;
    GestioneLotti_Insetti: number = enum_Gestione_Lotti.Nessuna;
    GestioneLotti_Inneschi: number = enum_Gestione_Lotti.Nessuna;
    GestioneGiacenze_Formulati = enum_Gestione_Giacenze.TuttiProdotti;
    GestioneGiacenze_Fertilizzanti = enum_Gestione_Giacenze.TuttiProdotti;
    GestioneGiacenze_Sementi = enum_Gestione_Giacenze.TuttiProdotti;
    GestioneGiacenze_Insetti = enum_Gestione_Giacenze.TuttiProdotti;
    GestioneGiacenze_Inneschi = enum_Gestione_Giacenze.TuttiProdotti;
    GestioneMagazzino_Abilitata_Formulati: boolean = true;
    GestioneMagazzino_Abilitata_Fertilizzanti: boolean = true;
    GestioneMagazzino_Abilitata_Sementi: boolean = true;
    GestioneMagazzino_Abilitata_Insetti: boolean = true;
    GestioneMagazzino_Abilitata_Inneschi: boolean = true;
    Blocca_Soglia: boolean = false;
    Scrittura_DDT_Ricevuto: boolean = false;
    Scrittura_Carico_Magazzino: boolean = false;
    Scrittura_Modifica_Anagrafica_Contatto: boolean = false;
    Modifica_Contatti_Pubblici: boolean = false;
    Scrittura_Modifica_Anagrafica_Macchina: boolean = false;
    Modifica_Macchine_Pubbliche: boolean = false;
    Lettura_Piano_Nutrizionale: boolean = false;

    Blocca_Raccolta_Carenza_non_Rispettata: boolean = false;
    Chili_Litri: boolean = false;
    Utente_Cod_Default_Gestione_Magazzino: boolean = false;
    Utente_Cod_Blocca_se_Supera_Giacenze: boolean = false;
    Opzione_Raccolta: number = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI;
    Opzione_Abbattimento: number = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI;
    Utente_Cod_Blocca_Raccolta_Carenza_non_Rispettata: boolean = false;

    //Servizi delle Grid
    GridMacchineHttpService: any;
    GridMacchinePublicService: any;

    GridOperatoriHttpService: any;
    GridOperatoriPublicService: any;

    GridImpiantiHttpService: any;
    GridImpiantiPublicService: any;
    GridImpiantiElementRefRows: any;

    GridProdottiDaTrattarePublicService: any;

    GridNoteHttpService: any;
    GridNotePublicService: any;

    SubsRefreshGridProdottiXImpianti:BehaviorSubject<{ RicaricaGrid: boolean, Flag_Calcola_Qta_Su_Impianti: boolean }> = new BehaviorSubject({ RicaricaGrid: false, Flag_Calcola_Qta_Su_Impianti: true });

    /*
    * @description:
    * La pagina del QdC può essere aperta in un iframe oppure in una window
    * */
    inFrame: boolean = false;

    inWindow: boolean = false;

    /*
   @description
   TODO Lorenzo sarà da sostituire in tutti i punti in cui viene controllato se è
   da fare il ribaltamento con gli ObjParametri con il TipoRibaltamento
   */
    TipoRibaltamento: RibaltamentoTypes = RibaltamentoTypes.Nessuno;

    _flag_Reinnesco: boolean = false;

    Subs: Subscription = new Subscription();

    getListaAttivita(): Attivita_Con_Parametri_Aggiuntivi[] {
        return cloneDeep(this.ListaAttivitaLetta.getValue());
    }

    setListaAttivita_Con_Parametri_Aggiuntivi(attivitaConParametriAggiuntivi: Attivita_Con_Parametri_Aggiuntivi[]) {
        this.ListaAttivitaLetta.next(cloneDeep(attivitaConParametriAggiuntivi));
    }

    get flag_Reinnesco(): boolean{
      return this._flag_Reinnesco;
    }

    set flag_Reinnesco(value: boolean){
      this._flag_Reinnesco = value;
    }

    getCategoria_Magazzino(lav_cod: number) {

        let elem_cod = 0;

        if (this.Elenco_Operazioni_Fertilizzanti.includes(lav_cod)) {
            elem_cod = FERTILIZZANTI;
        } else if (this.Elenco_Operazioni_Formulati.includes(lav_cod)) {
            elem_cod = FORMULATI;
        } else if (this.Elenco_Operazioni_Sementi.includes(lav_cod)) {
            elem_cod = SEMENTI;
        } else if (this.Elenco_Operazioni_Trasformati_Vegetali.includes(lav_cod)) {
            elem_cod = TRASFORMATI_VEGETALI;
        } else if (this.Elenco_Operazioni_Insetti.includes(lav_cod)) {
            elem_cod = INSETTI;
        }

        return elem_cod;
    }

    getTipoTestata(lav_cod: number) {

        let Tipo_Testata = 0;

        switch (lav_cod) {
            case enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO:
            case enum_LAVCOD.CONCIA_SEME:
            case enum_LAVCOD.GEODISINFESTAZIONE:
            case enum_LAVCOD.CONFUSIONE_SESSUALE:
            case enum_LAVCOD.DISORIENTAMENTO_SESSUALE:
                Tipo_Testata = 0;
                break;
            case enum_LAVCOD.DISSECCAMENTO:
            case enum_LAVCOD.DISERBO:
                Tipo_Testata = 1;
                break;
            case enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE:
                Tipo_Testata = 2;
                break;
        }

        return Tipo_Testata;
    }

    public get QdCForm(): FormGroup {

        if (!this._QdCForm) {

            //Per i validator verificare se è necessario aggiungerli qui oppure nel multioperazioneservice.GestisciValidatorOperazione

            const MultiOperazioneValidator = new MultiSelectValidator(false, "primaryKey.codice");

            const DataOperazioneValidator = new DataValidator(AGRODATAINIZIO, null);

            this._QdCForm = this.fb.group({
                Trattamento: this.fb.group({
                    Testata: this.fb.group({
                        flagVisita: [false],
                        Codici_Attivita: [],
                        Origine: [""],
                        MultiCentro: [false],
                        Raccoglitore: [],
                        Tipo: [],
                        TipoRicetta: [],
                        Stato: [],
                        Latitude: 0,
                        Longitude: 0,
                        Visualizza_Solo_Operazioni_Preferite: [false],
                        Operazioni: [null, MultiOperazioneValidator.validate.bind(MultiOperazioneValidator)],
                        Data: [new Date(), DataOperazioneValidator.validate.bind(DataOperazioneValidator)],
                        Ora: [new Date(new Date().setHours(0, 0, 0, 0))],
                        Specie: [],
                        Disciplinare: [],
                        Centro_Aziendale: [],
                        Campo: [],
                        Attivita_Personalizzata: [],
                        Descrizione_Altre_Lavorazioni: [],
                        InviaRicetta: [false],
                        Ricetta_Des: [],
                        Ricetta_Des_Long: [],
                        Ricetta_Numero: [],
                        Ricetta_Note: new FormControl('', Validators.maxLength(4000)),
                        Ricetta_Data_Da: [null],
                        Ricetta_Data_A: [null],
                        Attivita_Collegate: [],
                        Blocco_Attivita: [null],
                        TestataVisita: this.fb.group({
                            Data_Visita: [new Date()],
                            Ora_Inizio_Visita: [new Date()],
                            Ora_Fine_Visita: [new Date()],
                            Operatore_Visita: [null],
                            Azienda_Visita: [null],
                            Da_Remoto_Visita: [false],
                            statoWorkflow_Visita: [false],
                            Visualizza_Specie: [false],
                            Aziende_Agenzie: [false],
                            impOrarioFine_Visita: [],
                            NrOreTotali_Visita: [],
                            SpecieAnimali_Visita: [],
                        })
                    }),
                    CostiAccessori: this.fb.group({
                        Operatori: this.fb.array([]),
                        Macchine: this.fb.array([])
                    }),
                    Impianti: this.fb.group({
                        ImpiantiSelezionati: this.fb.array([])
                    }),
                    ProdottiDaTrattare: this.fb.group({
                        ProdottiDaTrattareSelezionati: this.fb.array([])
                    }),
                    Superfici: this.fb.group({
                        Sup_Selezionata: [{ value: 0, disabled: true }],
                        Sup_Trattata: [0]
                    }),
                    Quantita: this.fb.group({
                        Qta_Selezionata: [{ value: 0, disabled: true }],
                        Qta_Trattata: [0]
                    }),
                    Acqua: this.fb.group({
                        Acqua_Ha: [0],
                        Acqua_Tot: [0],
                        Dose_Acqua: [0]
                    }),
                    Sezioni_Prodotto: this.fb.array([]),
                    Sezioni_Senza_Prodotto: this.fb.array([]),
                    Causali: [],
                    Nota_Testuale: [],
                    Note: this.fb.array([])
                })
            });
        }

        return this._QdCForm;

    }

    public set QdCForm(QdCForm: FormGroup) {
        this._QdCForm = QdCForm;
    }

    public get TestataForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("Testata");
    }

    public get CostiAccessoriForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("CostiAccessori");
    }

    public get ImpiantiForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("Impianti");
    }

    public get ProdottiDaTrattareForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("ProdottiDaTrattare");
    }

    public get SuperficiForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("Superfici");
    }

    public get QuantitaForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("Quantita");
    }

    public get AcquaForm(): FormGroup {
        return <FormGroup>this.QdCForm?.get("Trattamento")?.get("Acqua");
    }

    public get Sezioni_ProdottoFormArray(): FormArray {
        return <FormArray>this.QdCForm?.get("Trattamento")?.get("Sezioni_Prodotto");
    }

    public get Sezioni_Senza_ProdottoFormArray(): FormArray {
        return <FormArray>this.QdCForm?.get("Trattamento")?.get("Sezioni_Senza_Prodotto");
    }

    public DosiProdottiFormArray(Sezione_Prodotto: FormGroup, Operazione: Lavorazione): FormArray {

        let DosiProdottiFormArray = null;

        let index = -1;

        if (Sezione_Prodotto && !Operazione) {

            index = this.Sezioni_ProdottoFormArray.getRawValue().findIndex(s => s.Operazione.primaryKey.codice === Sezione_Prodotto.getRawValue().Operazione.primaryKey.codice);

        } else if (!Sezione_Prodotto && Operazione) {
            index = this.Sezioni_ProdottoFormArray.getRawValue().findIndex(s => s.Operazione.primaryKey.codice === Operazione.primaryKey.codice);
        }

        if (index > -1)
            DosiProdottiFormArray = <FormArray>this.Sezioni_ProdottoFormArray.controls[index].get("DosiProdotti");

        return DosiProdottiFormArray;
    }


    public Sezione_ProdottoFormGroup(Operazione: Lavorazione): FormGroup {
        let Sezione_Prodotto = null;

        if (Operazione) {
            let index = this.Sezioni_ProdottoFormArray.getRawValue().findIndex(d => d.Operazione.primaryKey.codice === Operazione.primaryKey.codice);

            if (index > -1) {
                Sezione_Prodotto = <FormGroup>this.Sezioni_ProdottoFormArray.controls[index];
            }
        }

        return Sezione_Prodotto;
    }

    public get ImpiantiSelezionatiFormArray(): FormArray {
        return <FormArray>this.ImpiantiForm?.get("ImpiantiSelezionati");
    }

    public get ProdottiDaTrattareSelezionatiFormArray(): FormArray {
        return <FormArray>this.ProdottiDaTrattareForm?.get("ProdottiDaTrattareSelezionati");
    }

    public get MacchineFormArray(): FormArray {
        return <FormArray>this.CostiAccessoriForm?.get("Macchine");
    }

    public get OperatoriFormArray(): FormArray {
        return <FormArray>this.CostiAccessoriForm?.get("Operatori");
    }

    /** Is set to `true` after the notes have been initialized.
     * Is set to `false` when the operation or the species are changed. */
    public isNoteInitialized = false;
    public get NoteFormArray(): FormArray {
        return <FormArray>this._QdCForm?.get("Trattamento")?.get("Note");
    }

    public get Nota_TestualeFormControl(): FormControl {
        return <FormControl>this._QdCForm?.get("Trattamento")?.get("Nota_Testuale");
    }

    public get CausaliFormControl(): FormControl {
        return <FormControl>this._QdCForm?.get("Trattamento")?.get("Causali");
    }

    public get TestataVisitaForm(): FormGroup {
        return <FormGroup>this.TestataForm?.get("TestataVisita");
    }

    getSezioneProdottiForm(elem_cod: number, Operazione: Lavorazione): FormGroup {

        let lav_cod: number = 0;

        if (Operazione && + Operazione.primaryKey.codice !== 0)
            lav_cod = + Operazione.primaryKey.codice;

        const ProdottiForm: FormGroup = this.fb.group(<any>{
            Operazione: [Operazione],
            Categoria_Magazzino: [elem_cod],
            //Nessun_Magazzino:[false],
            //magazzino:[],
            flagDoseQuantitaTotale: [0],
            flagTipoDose: [0],
            Visualizza_Magazzini_Agenzie: [false],
            Visualizza_Magazzini_Esterni: [false],
            Magazzino_Agenzia: [],
            Magazzino_Esterno: [],
            Visualizza_Solo_Prodotti_in_Giacenza: [true],
            Magazzino_del_Prodotto_Selezionato: [],
            Lotto: [""],
            UdM: [],
            Dose_Ha: [0],
            Dose_Hl: [0],
            DoseTot_Ha: [0],
            DosiProdotti: this.fb.array([]),
            Modalita_Applicazione: [null],
            Original_Row_Value: [null]
        });

        ProdottiForm.controls['Magazzino_del_Prodotto_Selezionato'].valueChanges.subscribe(x => console.log(x))

        if (elem_cod === FORMULATI || elem_cod === INSETTI) {

/*            Object.keys(Dettagli_Prodotto.controls).forEach((key: string) => {
                ProdottiForm.addControl(key, Dettagli_Prodotto.get(key));
            });*/

            let dettaglioTrattamento = new DettaglioTrattamento();

            dettaglioTrattamento.prodotto = new Prodotto(0);

            dettaglioTrattamento.prodotto.descrizione = "";

            ProdottiForm.addControl("Prodotto", new FormControl(dettaglioTrattamento));
            ProdottiForm.addControl("EpocaDPI", new FormControl());
            ProdottiForm.addControl("Avversita", new FormControl());
            ProdottiForm.addControl("Soglia_Avversita", new FormControl());
            ProdottiForm.addControl("Dose_Etichetta", new FormControl());
            ProdottiForm.addControl("Ripartizione_Trappole", new FormControl(this.Opzioni_Ripartizione_Trappole[0]));
            ProdottiForm.get("DosiProdotti").setValidators([Validators.required]);

        } else if (elem_cod === FERTILIZZANTI) {

            let dettaglioFertilizzazione = new DettaglioFertilizzazione();

            dettaglioFertilizzazione.prodotto = new Prodotto(0);

            dettaglioFertilizzazione.prodotto.descrizione = "";

            ProdottiForm.addControl("Utilizza_Direttiva_Nitrati", new FormControl(false));
            ProdottiForm.addControl("Direttiva_Nitrati", new FormControl());
            ProdottiForm.addControl("Prodotto", new FormControl(dettaglioFertilizzazione));
            ProdottiForm.addControl("N_Percentuale_X_Prodotto", new FormControl(0));
            ProdottiForm.addControl("EpocaFertilizzazione", new FormControl());
            ProdottiForm.addControl("Efficienza", new FormControl(0));
            ProdottiForm.addControl("N_Utile", new FormControl(0));
            ProdottiForm.addControl("N", new FormControl(0));
            ProdottiForm.addControl("P", new FormControl(0));
            ProdottiForm.addControl("K", new FormControl(0));
            ProdottiForm.addControl("Cu", new FormControl(0));

            this.Gestisci_Controlli_Fertilizzanti(ProdottiForm, lav_cod);

            ProdottiForm.get("DosiProdotti").setValidators([Validators.required]);

        } else if (elem_cod === SEMENTI) {

            let dettaglioSemina = new DettaglioSemina();

            dettaglioSemina.prodotto = new Prodotto(0);

            dettaglioSemina.prodotto.descrizione = "";

            ProdottiForm.addControl("Prodotto", new FormControl(dettaglioSemina));
            ProdottiForm.addControl("Opzioni_Semina", new FormControl());
            ProdottiForm.addControl("Sup_Calcolata", new FormControl(0));

            //Per le sementi non è obbligatorio aver selezionato un prodotto perchè se non seleziono nulla sto facendo la semina 'fast'
            //come se fosse una aratura

        } else if (elem_cod === TRASFORMATI_VEGETALI && lav_cod === enum_LAVCOD.RACCOLTA) {

            // Inizializzo formArray di DettagliRaccolta per i prodotti raccolti
            ProdottiForm.addControl("ProdottiRaccolti", new FormArray([]));
            ProdottiForm.get("ProdottiRaccolti").setValidators([
                this.validateRaccolta
            ]);

            ProdottiForm.addControl("Data_Raccolta", new FormControl(new Date()));
            // Usato per impedire salvataggio durante modifica riga raccolta ripartizione auto
            ProdottiForm.addControl("isNotEditing_Raccolta", new FormControl(true));
            ProdottiForm.get("isNotEditing_Raccolta").addValidators(Validators.requiredTrue)

            // Creo formGroup delle opzioni di raccolta
            ProdottiForm.addControl("Opzioni_Raccolta", new FormGroup({
                Chiusura: new FormControl(0),
                Ripartizione: new FormControl(''),
                Modalita: new FormControl(''),
                CarichiMagazzinoAttivi: new FormControl(false),
                GenerazioneLotto: new FormControl(enum_Generazione_Lotto_Raccolta.MANUALE)
            }));
            ProdottiForm.get("Opzioni_Raccolta").setValidators([
                this.validateOpzioniRaccolta
            ]);
        }

        return ProdottiForm;
    }

    getDettagliForm(elem_cod: number,Operazione: Lavorazione): FormGroup{
        let Form = null;

        switch(elem_cod){
            case FORMULATI:
            case INSETTI:
                Form = this.getDettagliFormulatiForm(elem_cod,Operazione);
                break;
            case FERTILIZZANTI:
                Form = this.getDettagliFertilizzantiForm(elem_cod,Operazione);
                break;
            case SEMENTI:
                Form = this.getDettagliSementiForm(elem_cod,Operazione);
            case TRASFORMATI_VEGETALI:
                if(Operazione && + Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA){
                  Form = this.getDettagliRaccoltaForm(elem_cod,Operazione);
                }
                break;
        }

        return Form;
    }

    getDettagli_Prodotto(elem_cod: number, Operazione: Lavorazione): FormGroup{

        const ProdottiForm: FormGroup = this.fb.group(<any>{
            Operazione: [Operazione],
            Categoria_Magazzino: [elem_cod],
            Centro_Aziendale: [],
            Piva: [""],
            Sa_Cod: [0],
            Fabbricato_Cod: [0],
            Fabbricato_Des: [""],
            Fr_Cod: [0],
            Fr_Des: [""],
            UdM: [],
            UdM_Magazzino: [],
            UdM_Indicata: [],
            Udm_Cod: [""],
            Udm_Des: [""],
            Udm_Cod_Trasformato: [0],
            Magazzino_del_Prodotto_Selezionato: [],
            Dose_Ha: [0],
            Dose_Hl: [0],
            DoseTot_Ha: [0],
            Ha_Hl: [0],
            Dose_QtaTot: [0],
            For_Veg_Cod: [0],
            Dose_Etichetta_Max: [0],
            Dose_Etichetta_Value: [0],
            Limite_Numero_Trattamenti: [0],
            Limite_Numero_Trattamenti_UDM_SIM: [0],
            Limite_Numero_Trattamenti_UDM_COD: [0],
            IntervalloTrattamenti_Min: [0],
            IntervalloTrattamenti_Max: [0],
            strPA_COD: [""],
            strPA_COD_Pesi: [""],
            strCLTOSS_COD: [""],
            MgO: [0],
            Dose_Fittizia: [0],
            Lotto: [""],
            Mat_Regolamento: [0],
            Mat_Veg_Cod: [0],
            Mat_Cul_Cod: [0],
            Piva_Rif: [""],
            Sa_Cod_Rif: [0],
            ID_Agenda_Rif: [0],
            ID_Mov_Rif: [0],
            ID_Mov_Det_Rif: [0],
            Lav_Cod_Rif: [0],
            Cau_Mov_Rif: [""],
            Des_Rif: [""],
            Qta_Rif: [0],
            flagTipoDose: [0],
            flagDoseQuantitaTotale: [0],
            Dose_Acqua: [0],
            DosiProdottiGridrowId: [-1],
            Visualizza_Magazzini_Agenzie: [false],
            Visualizza_Magazzini_Esterni: [false],
            Magazzino_Agenzia: [],
            Magazzino_Esterno: [],
            Visualizza_Solo_Prodotti_in_Giacenza: [true],
            Riga_Salvata: [false],
            Nuova_Riga: [false],
            Modalita_Applicazione: [null],
            Original_Row_Value: [null],
            Tutti_Problemi_DettaglioProdotto: [enum_Problema_DettaglioProdotto.Nessuno],
            Problema_DettaglioProdotto_Da_Risolvere: [enum_Problema_DettaglioProdotto.Nessuno]
            //Nessun_Magazzino:[false],
            //magazzino:[],
        });

        return ProdottiForm;
    }

    getDettagliFormulatiForm(elem_cod: number,Operazione: Lavorazione): FormGroup{

        let dettaglioTrattamento = new DettaglioTrattamento();

        dettaglioTrattamento.prodotto = new Prodotto(0);

        dettaglioTrattamento.prodotto.descrizione = "";

        const FormulatiForm: FormGroup = this.fb.group(<any>{
            Prodotto: [dettaglioTrattamento],
            Avversita: [],
            Av_Cod: [0],
            Av_Gru: [""],
            Av_Des: [""],
            Soglia_Avversita: [],
            Soglia_Value: [0],
            Soglia_Des: [""],
            Dose_Etichetta: [],
            Dosi_Etichetta: [],
            Dose_Etichetta_Des: [""],
            BufferZone: [],
            BufferMin: [0],
            BufferMax: [0],
            strBuffer: [""],
            Carenza: [0],
            Prima_Raccolta: [new Date()],
            Polverulento: [Tipo_Polverulento.NonPolverulento],
            EpocaDPI: [],
            PrincipiAttivi: [],
            QuantitaSuImpianti: [],
            Ripartizione_Trappole: [this.Opzioni_Ripartizione_Trappole[0]],
            Magazzino_Innesco: [],
            Lotto_Innesco: [""],
            UdM_Innesco: [],
            Magazzino_Agenzia_Innesco: [],
            DoseTot_Ha_Innesco: [0],
            Innesco_Incluso: [true],
            Visualizza_Solo_Inneschi_in_Giacenza: [false]
            //Ha_Hl: [0],

        });

        const Dettagli_ProdottoForm = this.getDettagli_Prodotto(elem_cod,Operazione);

        Object.keys(Dettagli_ProdottoForm.controls).forEach((key: string) => {
            FormulatiForm.addControl(key, Dettagli_ProdottoForm.get(key));
        });

        return FormulatiForm;
    }

    getDettagliFertilizzantiForm(elem_cod: number,Operazione: Lavorazione):FormGroup{

        let dettaglioFertilizzazione = new DettaglioFertilizzazione();

        dettaglioFertilizzazione.prodotto = new Prodotto(0);

        dettaglioFertilizzazione.prodotto.descrizione = "";

        const FertilizzantiForm: FormGroup = this.fb.group(<any>{
            Prodotto: [dettaglioFertilizzazione],
            Efficienza: [0],
            N: [0],
            N_Utile: [0],
            P: [0],
            K: [0],
            Cu: [0],
            EpocaFertilizzazione: [],
            Utilizza_Direttiva_Nitrati: [false],
            Direttiva_Nitrati: [],
            N_Percentuale_X_Prodotto: []
        });

        const Dettagli_ProdottoForm = this.getDettagli_Prodotto(elem_cod,Operazione);

        Object.keys(Dettagli_ProdottoForm.controls).forEach((key: string) => {
            FertilizzantiForm.addControl(key, Dettagli_ProdottoForm.get(key));
        });

        return FertilizzantiForm;
    }

    getDettagliSementiForm(elem_cod: number,Operazione: Lavorazione): FormGroup{

        let dettaglioSemina = new DettaglioSemina();

        dettaglioSemina.prodotto = new Prodotto(0);

        dettaglioSemina.prodotto.descrizione = "";

        const DettagliSementiForm: FormGroup = this.fb.group(<any>{
            Prodotto: [dettaglioSemina],
            Cod_Articolo: [""],
            Opzioni_Semina: [],
            Sup_Calcolata: [0]
        });

        const Dettagli_ProdottoForm = this.getDettagli_Prodotto(elem_cod,Operazione);

        Object.keys(Dettagli_ProdottoForm.controls).forEach((key: string) => {
            DettagliSementiForm.addControl(key, Dettagli_ProdottoForm.get(key));
        });

        return DettagliSementiForm;
    }

    getDettagliRaccoltaForm(elem_cod: number,Operazione: Lavorazione): FormGroup{

      const Dettagli_ProdottoForm = this.getDettagli_Prodotto(elem_cod,Operazione);

      return Dettagli_ProdottoForm;
    }

    private validateRaccolta(control: AbstractControl): ValidationErrors | null {
        const raccolteArray: Array<DettaglioRaccolta> = (control as FormArray).value;
        if (!raccolteArray || !raccolteArray.length) return null;

        /*** QUANTITY CHECK ***/
        let error = null;
        raccolteArray.forEach((r: DettaglioRaccolta) => {
            if (!!r.prodotto && r.prodotto.codice && (!r.quantitaTotaleReale || r.quantitaTotaleReale.toString() === '0'))
                error = { "invalidQuantity": true };
        })

        /*** UDM CHECK ***/
        let prodottoXudm = new Map<number, number>();
        raccolteArray.map(dettaglio => { return { cod: dettaglio.prodotto?.codice, udm: dettaglio.unitaDiMisura?.codice } })
            .forEach(prodotto => {
                if (!prodottoXudm.has(prodotto.cod)) {
                    prodottoXudm.set(prodotto.cod, prodotto.udm);
                }
                if (prodottoXudm.get(prodotto.cod) !== prodotto.udm) {
                    error = { "invalidUdM": true };
                }
            });
        if (error) return error;

        /*** STORAGE CHECK ***/
        let raccoltaHasStorage: Array<boolean> = raccolteArray.map(raccolta =>
            raccolta.MagazziniMovimentazioni?.map(movimento => !!movimento.Magazzino?.primaryKey?.codice) || []
        ).map(mov => mov.length > 0 && mov.reduce((a, b) => a || b));
        let hasAtLeastOneStorage: boolean = raccoltaHasStorage.some(b => b);
        let everyProductHasStorage: boolean = raccoltaHasStorage.every(b => b);
        // Every or none of the products must have specified a storage unit
        if (hasAtLeastOneStorage && !everyProductHasStorage) {
            return { "storageNotCompliant": true };
        }
        if (!hasAtLeastOneStorage) return error;

        /*** LOT CHECK ***/
        /* A product that was inserted twice should have the same storage unit
         unless the lot can distinguish the two loads */
        let repMode = control.parent?.value?.Opzioni_Raccolta?.Ripartizione;
        let genLotto = control.parent?.value?.Opzioni_Raccolta?.GenerazioneLotto;

        if (repMode !== enum_Ripartizione_Raccolta.MANUALE) return error;
        if (genLotto === enum_Generazione_Lotto_Raccolta.DA_DATA) {
            let ps = raccolteArray.map((r) => {
                let i = r.QuantitaSuImpianti.findIndex(q => q.Magazzino);
                return {
                    prod: r.prodotto.codice,
                    mag: r.QuantitaSuImpianti[i].Magazzino.primaryKey.codice + "_"
                        + r.QuantitaSuImpianti[i].Magazzino.primaryKey.centroAziendalePK.codice + "_"
                        + r.QuantitaSuImpianti[i].Magazzino.primaryKey.centroAziendalePK.partitaIva
                }
            })
            // Lot should be the same for every product, check if single product has multiple storages
            ps.forEach((p, i) => {
                if (ps.filter(s => s.prod === p.prod && s.mag !== p.mag).length)
                    error = { "storageAlreadySelected": true, "idx": i }
            })
            return error;
        }
        if (genLotto === enum_Generazione_Lotto_Raccolta.DA_DATA) {
            let map = new Array<{ prod: number, lotto: string, storage: string }>();
            for (let r of raccolteArray) {
                let pCod = r.prodotto.codice
                let i = r.QuantitaSuImpianti.findIndex(q => q.Lotto);
                if (i < 0) continue;
                let lot = r.QuantitaSuImpianti[i].Lotto;
                let storage = r.QuantitaSuImpianti[i].Magazzino.primaryKey.codice + "_"
                    + r.QuantitaSuImpianti[i].Magazzino.primaryKey.centroAziendalePK.codice + "_"
                    + r.QuantitaSuImpianti[i].Magazzino.primaryKey.centroAziendalePK.partitaIva;

                i = map.findIndex(i => i.prod === pCod && i.lotto === lot)
                if (i < 0) {
                    map.push({ prod: pCod, lotto: lot, storage: storage })
                    continue;
                }
                let item = map[i];
                if (item.storage !== storage)
                    return { "storageAlreadySelected": true, "idx": i };
            }
        }
        if ((QdCService.filtroLotti.includes(TRASFORMATI_VEGETALI + '_1') || QdCService.filtroLotti == '1') &&
            (genLotto === enum_Generazione_Lotto_Raccolta.MANUALE || genLotto === enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD)
        ) {
            // Every product should specify a lot
            const everyProductHasLot = raccolteArray.map(r =>
                r.QuantitaSuImpianti.map(suImpianto => suImpianto.Lotto).some(lotto => lotto)
            ).every(lotDefined => lotDefined);
            if (!everyProductHasLot) {
                return { "manualLotNotDefined": true };
            }
        }
        return error;
    }

    private validateOpzioniRaccolta(control: AbstractControl): ValidationErrors | null {
        let opzioni = control.value;
        let raccolteArray = control.parent?.value?.ProdottiRaccolti;
        if (opzioni.GenerazioneLotto !== enum_Generazione_Lotto_Raccolta.DA_DATA
            || raccolteArray.length === 0) return null;

        let error = null;
        let ps = raccolteArray.map((r) => {
            let i = r.QuantitaSuImpianti.findIndex(q => q.Magazzino);
            let magCod = "";
            if (i >= 0)
                magCod = r.QuantitaSuImpianti[i].Magazzino?.primaryKey?.codice + "_"
                    + r.QuantitaSuImpianti[i].Magazzino?.primaryKey?.centroAziendalePK?.codice + "_"
                    + r.QuantitaSuImpianti[i].Magazzino?.primaryKey?.centroAziendalePK?.partitaIva
            return {
                prod: r.prodotto.codice,
                mag: magCod
            }
        })
        // Lot would be the same for every product, check if single product has multiple storages
        ps.forEach((p, i) => {
            if (ps.filter(s => s.prod === p.prod && s.mag !== p.mag).length)
                error = { "storageAlreadySelected": true, "idx": i }
        })
        return error;
    }

    //Controlla se ci sono DosiProdotti salvate
    public CheckDosiProdottiSalvate(): boolean {

        let DosiProdotti_con_Righe_Salvate: boolean = false;

        if (this.Sezioni_ProdottoFormArray) {

            for (let s of this.Sezioni_ProdottoFormArray.getRawValue()) {

                let Operazione = s.Operazione;

                if (this.DosiProdottiFormArray(null, Operazione)?.getRawValue()?.findIndex(d => d.Riga_Salvata === true) > -1) {
                    DosiProdotti_con_Righe_Salvate = true;
                    break;
                }
            }

        }

        return DosiProdotti_con_Righe_Salvate;
    }

    //Controlla se ci sono DosiProdotti non salvate
    public CheckDosiProdottiNonSalvate(): boolean {

        let DosiProdotti_con_Righe_Non_Salvate: boolean = false;

        if (this.Sezioni_ProdottoFormArray) {

            for (let s of this.Sezioni_ProdottoFormArray.getRawValue()) {

                let Operazione = s.Operazione;

                if (this.DosiProdottiFormArray(null, Operazione)?.getRawValue()?.findIndex(d => d.Riga_Salvata === false) > -1) {
                    DosiProdotti_con_Righe_Non_Salvate = true;
                    break;
                }
            }

        }

        return DosiProdotti_con_Righe_Non_Salvate;
    }

    public DosiProdotticonRigheSalvate(Sezione_Prodotto: FormGroup, Operazione: Lavorazione): Array<any> {

        let DosiProdottiFormArray = this.DosiProdottiFormArray(Sezione_Prodotto, Operazione);

        let DosiProdotti: Array<any> = [];

        if (DosiProdottiFormArray)
            DosiProdotti = cloneDeep(DosiProdottiFormArray.getRawValue()).filter(d=>d.Riga_Salvata);

        return DosiProdotti;
    }

    Gestisci_Controlli_Fertilizzanti(FertilizzantiForm: FormGroup, lav_cod: number) {

        if (FertilizzantiForm) {
            FertilizzantiForm.get("Efficienza").disable({ emitEvent: false });
            FertilizzantiForm.get("N_Utile").disable({ emitEvent: false });

            //abilito gli apporti per la distribuzione ammendanti
            if (lav_cod === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) {
                FertilizzantiForm.get("N").enable({ emitEvent: false });
                FertilizzantiForm.get("P").enable({ emitEvent: false });
                FertilizzantiForm.get("K").enable({ emitEvent: false });
                FertilizzantiForm.get("Cu").enable({ emitEvent: false });
            } else {
                FertilizzantiForm.get("N").disable({ emitEvent: false });
                FertilizzantiForm.get("P").disable({ emitEvent: false });
                FertilizzantiForm.get("K").disable({ emitEvent: false });
                FertilizzantiForm.get("Cu").disable({ emitEvent: false });
            }

            if (this.Is_Ribaltamento_Ricetta_Da_PUA()) {
                FertilizzantiForm.get("Magazzino_del_Prodotto_Selezionato").enable({ emitEvent: false });
                FertilizzantiForm.get("Lotto").enable({ emitEvent: false });
            }

            if (this.Abilita_Modifica_Efficienza_Fertilizzanti(FertilizzantiForm))
                FertilizzantiForm.get("Efficienza").enable({ emitEvent: false });

        }

    }

    /*
    * @description Abilita la modifica manuale dell'Efficienza se si decide di non utilizzare la Direttiva Nitrati
    * */
    Abilita_Modifica_Efficienza_Fertilizzanti(FertilizzantiForm: FormGroup): boolean {

      let abilita_efficienza = false;

      if (FertilizzantiForm && FertilizzantiForm.get("Categoria_Magazzino").getRawValue() === FERTILIZZANTI) {
        let Utilizza_Direttiva_Nitrati: boolean = FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").getRawValue();

        if (!Utilizza_Direttiva_Nitrati)
          abilita_efficienza = true;
      }

      return abilita_efficienza;

    }

    getCONSIDERATERRENONUDO(): boolean {
        return this.CONSIDERATERRENONUDO;
    }

    setCONSIDERADETTAGLITERRENONUDO() {

        //Se sono nel multi operazione e una delle due operazioni ha considera-dettagli terreno nudo false allora tutte e due sono false

        //TODO quando verranno gestite le operazioni che non hanno il terreno nudo (es Irrigazione) dovrò aggiungere
        //il controllo sulla Specie /Destinazione d'uso
        let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").value;

        //Per ora non si possono fare delle Visite sul Terreno Nudo
        let flagVisita: boolean = this.TestataForm.get("flagVisita").getRawValue();

        if (flagVisita) {
            // this.CONSIDERATERRENONUDO = false;           //commentata per far vedere le destinazioni d'uso nella DDL delle specie aziendali
            // this.DETTAGLITERRENONUDO = false;
        } else {
            if (Operazioni) {

                let index = Operazioni.findIndex(o => {
                    let lav_cod = +o.primaryKey.codice;

                    if (lav_cod === enum_LAVCOD.INSTALLAZIONE_TRAPPOLE ||
                        lav_cod === enum_LAVCOD.CATTURE_MASSA ||
                        lav_cod === enum_LAVCOD.REINNESCO_TRAPPOLE ||
                        lav_cod === enum_LAVCOD.DISTRIBUZIONE_INSETTI ||
                        lav_cod === enum_LAVCOD.CONFUSIONE_SESSUALE ||
                        lav_cod === enum_LAVCOD.DISORIENTAMENTO_SESSUALE ||
                        lav_cod === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE ||
                        lav_cod === enum_LAVCOD.FASI_FENOLOGICHE ||
                        lav_cod === enum_LAVCOD.RILIEVO_INDICI_MATURITA ||
                        lav_cod === enum_LAVCOD.CONCIA_SEME ||
                        lav_cod === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO ||
                        lav_cod === enum_LAVCOD.IRRIGAZIONE ||
                        lav_cod === enum_LAVCOD.RILIEVO_ERBE_INFESTANTI ||
                        lav_cod === enum_LAVCOD.RILIEVO_PIOGGE) {
                        return o;
                    }
                });

                if (index > -1) {
                    this.CONSIDERATERRENONUDO = false;
                    this.DETTAGLITERRENONUDO = false;
                } else {
                    this.CONSIDERATERRENONUDO = true;
                    this.DETTAGLITERRENONUDO = true;
                }

            }
        }

    }


    Inizializza_QdC(): Promise<boolean> {

        return new Promise<boolean>((resolve, reject) => {

            const LeggiInizializza_QdC = <LeggiInizializza_QdC>{
                operazioni: this.TestataForm.get("Operazioni").value,
                tipoOperazioneDB: this.objParametriAgenda.TipoOperazioneDB,
                impresa: this.getImpresa_Model(),
                data: this.TestataForm.get("Data").value,
                tipoAttivita: this.TestataForm.get("Tipo").value,
                statoAttivita: this.TestataForm.get("Stato").value,
                tipoRicetta: this.TestataForm.get("TipoRicetta").value,
                lista_Attivita: this.getListaAttivita() as Attivita[],
                codiciAttivita: this.TestataForm.get("Codici_Attivita").value,
                disciplinare: this.getDisciplinareModelValue(enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)
            };

            this.agendaservice.Inizializza_QdC(LeggiInizializza_QdC).then((risp: rispostaStandard<Inizializza_QdC>) => {

                this.obj_Inizializza_QdC = risp.RispostaStringa;

                this.AggiornaQtaProdotti();

                this.Imposta_PUA(this.obj_Inizializza_QdC.pua, false).then();

                this.TestataForm.patchValue({
                    Data: this.obj_Inizializza_QdC.Data
                });

                if (!this.obj_Inizializza_QdC.SportelloAperto || (this.obj_Inizializza_QdC.Data < this.obj_Inizializza_QdC.Data_Min ||
                    this.obj_Inizializza_QdC.Data > this.obj_Inizializza_QdC.Data_Max) && !this.obj_Inizializza_QdC.AziendaInVerifica) {

                    this.MostrabtnSalva = false;

                } else if (this.obj_Inizializza_QdC.AziendaInVerifica) {
                    this.MostrabtnSalva = false;

                    this.QdCForm.disable({ emitEvent: false });

                } else {
                    if (!this.obj_Inizializza_QdC.PraticaTrovata) {
                        this.MostrabtnSalva = false;

                        this.giasdialogservice.errorPromise(
                            "",
                            "PassaggioAllePraticheInserimentoOpNonEffettuato",
                            true
                        ).then();
                    } else {
                        this.MostrabtnSalva = true;
                    }
                }

                resolve(true);

            });

        });

    }

    setImpostazione_Avversita_Prima_Dei_Prodotti(Operazioni: Lavorazione[]){

        //Se è stata selezionata un'Installaizone Trappole Catture Massa forzo la visualizzaiozne delle avversità dopo la ricerca dei prodotti
        if(Operazioni &&
          Operazioni.findIndex(o=>this.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ o.primaryKey.codice)) > -1){
          this.Mostra_Avversita_Prima_Dei_Prodotti = false;

          return;
        }

        let impostazione;

        switch (this.objParametriAgenda.TipoOperazioneAgenda) {
            case Tipo_Attivita.QuadernoDiCampagna:
                this.Mostra_Avversita_Prima_Dei_Prodotti = false;
                break;
            case Tipo_Attivita.Ricetta:
                this.Mostra_Avversita_Prima_Dei_Prodotti = true;
                break;
        }

        if (this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna) {
            //1, 0 Prodotto-->Avversita ; 2 Avversità-->Prodotto
            impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI);

            if (impostazione !== undefined && impostazione !== null && impostazione.Valore !== '') {
                if (impostazione.Valore === "2") {
                    this.Mostra_Avversita_Prima_Dei_Prodotti = true;
                } else {
                    this.Mostra_Avversita_Prima_Dei_Prodotti = false;
                }
            }
        }


        if (this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.Ricetta) {
            //1, 0 Prodotto-->Avversita ; 2 Avversità-->Prodotto
            impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI_RICETTE);

            if (impostazione !== undefined && impostazione !== null && impostazione.Valore !== '') {
                if (impostazione.Valore === "1") {
                    this.Mostra_Avversita_Prima_Dei_Prodotti = false;
                } else {
                    this.Mostra_Avversita_Prima_Dei_Prodotti = true;
                }
            }
        }
    }

    getUtente(Operazioni: Lavorazione[]) {

        this.Utente = this.PermessiUtenteService.getCurrentUser();

        this.Scrittura_Modifica_Anagrafica_Contatto = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Contatto, 2);

        this.Modifica_Contatti_Pubblici = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Modifica_Contatti_Pubblici, 2);

        this.Scrittura_Modifica_Anagrafica_Macchina = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine, 2);

        this.Modifica_Macchine_Pubbliche = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Macchine_Assegnazione_Pubblica, 2);

        this.Scrittura_DDT_Ricevuto = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Consegne_Acquisto, 2);

        this.Scrittura_Carico_Magazzino = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Consegne_Acquisto, 2);

        this.Lettura_Piano_Nutrizionale = this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Piano_Nutrizionale, 0);

        this.getImpostazioni_QdC(Operazioni);

        this.setVariabiliGlobali();

    }

    getImpostazioni_QdC(Operazioni: Lavorazione[]) {
        let impostazione;

        this.getImpostazioni_QdC_Centro_Azienda(Operazioni);

        impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName);
        if (impostazione !== undefined && impostazione !== null && impostazione.Valore === '1') {
            this.visualizza_Kpin_BlockName = true;
        }

        impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SuperUser_Visualizza_Codici_Anagrafici);
        if (impostazione !== undefined && impostazione !== null && impostazione.Valore === '1') {
            this.visualizza_codici_imp_app_prj = true;
        }

        this.Impostazione_Mag = this.getSettingValueAs(enum_Impostazioni_Utenti.UTENTE_GESTIONE_MAGAZZINO_2);

        impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI);
        if (impostazione !== undefined && impostazione !== null && impostazione.Valore === '1') {
            this.leggiAncheImpiantiBloccati = true;
        }

        // Recupero le colonne visibili della Grid Impianti
        impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda);
        if (impostazione !== undefined && impostazione !== null && impostazione.Valore !== '') {
            this.ColonneVisibiliGridImpianti = impostazione.Valore.split('|');
        } else {
            // Default colonne Visibili se non è specificato nell'Impostazione_Utente
            this.ColonneVisibiliGridImpianti = ['Centro Aziendale', 'App.', 'Varietà'];
        }

        //QUELLO SOPRA NON ATTIVATO PERCHE' PER IL LIVELLO INSTALLAZIONE UTILIZZIAMO IL FLAG CHE CI INDICA
        //CAB = 1
        //SBTF = 2
        //BASE = 3 --> Vecchio tipo

        //Se sono in una Ricetta o in un Brogliaccio nascondo il pulsante salva e vai ai costi
        if (this.objParametriAgenda.TipoOperazioneAgenda !== Tipo_Attivita.Ricetta) {
            impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI);
            if (impostazione && impostazione.Valore !== "" &&
                +impostazione.Valore === 3 && this.PermessiUtenteService.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_Da_QdC_CdG, 2)
                && !this.TestataForm.get('flagVisita').value) {
                this.flag_MostraBtnSalvaCDG = true;
            }
        }

        this.Blocca_Soglia = this.getSettingValueAs(enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI, value => value === "2", false);

        //Se non ci sono operazioni scelte imposto il flag Visualizza_Solo_Operazioni_Preferite altrimenti lo lascio così com'è
        if (!Operazioni || Operazioni.length === 0) {
            impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE);
            if (impostazione !== undefined && impostazione !== null && impostazione.Valore !== '' && impostazione.Valore.split("|").length > 0) {
                if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write) {
                    this.TestataForm.patchValue({ Visualizza_Solo_Operazioni_Preferite: true });
                } else {
                    //In modifica carica sempre tutte le Operazioni
                    this.TestataForm.patchValue({ Visualizza_Solo_Operazioni_Preferite: false });
                }
            } else {
                //Se non ci sono Operazioni preferite salvate carico tutte le Operazioni
                this.TestataForm.patchValue({ Visualizza_Solo_Operazioni_Preferite: false });
            }
        }

        this.Blocca_Raccolta_Carenza_non_Rispettata = this.getSettingValueAs(
            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA,  this.equalsOne, false
        );
        this.Chili_Litri = this.getSettingValueAs(enum_Impostazioni_Utenti.UTENTE_COD_CHILI_LITRI, this.equalsOne, false);
        //Utilizzo Magazzino --> valore: 0=no 1=si
        this.Utente_Cod_Default_Gestione_Magazzino = this.getSettingValueAs(
            enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO, (value) => +value === -1, false
        );
        this.Utente_Cod_Blocca_se_Supera_Giacenze = this.getSettingValueAs(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, this.equalsOne, false);
        this.Opzione_Abbattimento = this.getSettingValueAs(enum_Impostazioni_Utenti.Utente_OpzioneChiusuraAbbattimenti, this.toNumber, this.Opzione_Abbattimento);
        this.Utente_Cod_Blocca_Raccolta_Carenza_non_Rispettata = this.getSettingValueAs(
            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA, this.equalsOne, false
        );

        //Se siamo in modalità demetra forzo il default a LASCIA_ATTIVI con VINCOLO
        if(this.IsModalitaDemetra()){
          this.Opzione_Raccolta = enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_VINCOLO;
        }else{
          this.Opzione_Raccolta = this.getSettingValueAs(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO, this.toNumber, this.Opzione_Raccolta);
        }

    }

    private getSettingValueAs(settings: enum_Impostazioni_Utenti, mapper: (readVal: string) => any = (readVal) => readVal, defaultValue: any = '') {

        let impostazione: Utente_Impostazioni = this.PermessiUtenteService.getImpostazione_Utente(settings);
        if (impostazione !== undefined && impostazione !== null && impostazione.Valore !== '') {
            return mapper(impostazione.Valore);
        } else {
            return defaultValue
        }

    }

    getImpostazioni_Magazzino_Lotto_Giacenze(Piva: string,Operazioni: Lavorazione[]) {

        let TestataValue: Testata = this.TestataForm.getRawValue();

        let valore_impostazione: string = "";

        let index: number = -1;

        let obj_Impostazioni_Magazzino_Lotto_Giacenze = {
            GestioneMagazzino_Abilitata: [
                { Valore: true, Elem_Cod: FORMULATI },
                { Valore: true, Elem_Cod: FERTILIZZANTI },
                { Valore: true, Elem_Cod: SEMENTI },
                { Valore: true, Elem_Cod: INSETTI },
                { Valore: true, Elem_Cod: INNESCHI }
            ],
            GestioneGiacenze_Abilitata: [
                { Valore: enum_Gestione_Giacenze.TuttiProdotti, Elem_Cod: FORMULATI },
                { Valore: enum_Gestione_Giacenze.TuttiProdotti, Elem_Cod: FERTILIZZANTI },
                { Valore: enum_Gestione_Giacenze.TuttiProdotti, Elem_Cod: SEMENTI },
                { Valore: enum_Gestione_Giacenze.TuttiProdotti, Elem_Cod: INSETTI },
                { Valore: enum_Gestione_Giacenze.TuttiProdotti, Elem_Cod: INNESCHI }
            ],
            GestioneLotti: [
                { Valore: enum_Gestione_Lotti.Nessuna, Elem_Cod: FORMULATI },
                { Valore: enum_Gestione_Lotti.Nessuna, Elem_Cod: FERTILIZZANTI },
                { Valore: enum_Gestione_Lotti.Obbligatoria, Elem_Cod: SEMENTI },
                { Valore: enum_Gestione_Lotti.Nessuna, Elem_Cod: INSETTI },
                { Valore: enum_Gestione_Lotti.Nessuna, Elem_Cod: INNESCHI }
            ]
        };

        //Per il Reinnesco la parte di Magazzini e Lotti dei Formulati non deve apparire

        //Per ora nella lettura delle impostazioni a scalare dell'utilizza magazzino, gestione giacenze e gestione lotto non vado a fare filtro anche per centro ma solo per la piva

        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.findIndex(x => x.Elem_Cod === FORMULATI);

        if(this.flag_Reinnesco || (Operazioni && (Operazioni.findIndex(x=>+ x.primaryKey.codice === enum_LAVCOD.REINNESCO_TRAPPOLE)) > -1)){

          obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = false;

        }else{
          valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, FORMULATI);

          if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = (+valore_impostazione === 1) ? true : false;

          }
        }

        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, FERTILIZZANTI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.findIndex(x => x.Elem_Cod === FERTILIZZANTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = (+valore_impostazione === 1) ? true : false;

        }

        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, SEMENTI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.findIndex(x => x.Elem_Cod === SEMENTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = (+valore_impostazione === 1) ? true : false;

        }

        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, INSETTI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.findIndex(x => x.Elem_Cod === INSETTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = (+valore_impostazione === 1) ? true : false;

        }

      valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
        enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, INNESCHI);

      if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.findIndex(x=>x.Elem_Cod === INNESCHI);

        obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata[index].Valore = (+valore_impostazione === 1) ? true : false;

      }


      index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.findIndex(x => x.Elem_Cod === FORMULATI);

      if(this.flag_Reinnesco || (Operazioni && (Operazioni.findIndex(x=>+ x.primaryKey.codice === enum_LAVCOD.REINNESCO_TRAPPOLE)) > -1)){

        obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Nessuna;

      }else{
        // 0=no;1=obbligatorio;2=facoltativo
        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
          enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, FORMULATI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

          obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = +valore_impostazione;

          //Per le ricette forzo il Lotto (181) a facoltativo (2) se era obbligatorio (1)
          if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
            if (obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore === enum_Gestione_Lotti.Obbligatoria) {
              obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Facoltativa;
            }
          }

        }
      }
        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, FERTILIZZANTI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.findIndex(x => x.Elem_Cod === FERTILIZZANTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = +valore_impostazione;

            //Per le ricette forzo il Lotto (181) a facoltativo (2) se era obbligatorio (1)
            if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
                if (obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore === enum_Gestione_Lotti.Obbligatoria) {
                    obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Facoltativa;
                }
            }

        }

        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, SEMENTI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.findIndex(x => x.Elem_Cod === SEMENTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = +valore_impostazione;

            //Per le ricette forzo il Lotto (181) a facoltativo (2) se era obbligatorio (1)
            if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
                if (obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore === enum_Gestione_Lotti.Obbligatoria) {
                    obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Facoltativa;
                }
            }

        }

        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, INSETTI);


        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.findIndex(x => x.Elem_Cod === INSETTI);

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = +valore_impostazione;

            //Per le ricette forzo il Lotto (181) a facoltativo (2) se era obbligatorio (1)
            if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
                if (obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore === enum_Gestione_Lotti.Obbligatoria) {
                    obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Facoltativa;
                }
            }

        }

      valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
        enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, INNESCHI);


      if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.findIndex(x=>x.Elem_Cod === INNESCHI);

        obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = +valore_impostazione;

        //Per le ricette forzo il Lotto (181) a facoltativo (2) se era obbligatorio (1)
        if(TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire){
          if(obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore === enum_Gestione_Lotti.Obbligatoria){
            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti[index].Valore = enum_Gestione_Lotti.Facoltativa;
          }
        }

      }

      index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.findIndex(x => x.Elem_Cod === FORMULATI);

      if(this.flag_Reinnesco || (Operazioni && (Operazioni.findIndex(x=>+ x.primaryKey.codice === enum_LAVCOD.REINNESCO_TRAPPOLE)) > -1)){
        obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.SoloMovimentati;
      }else{
        //Per le ricette forzo la 180 a 2 tutti i prodotti senza neanche leggerla
        if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
          obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.TuttiProdotti;
        } else {

          // 0=soloMovimentati; 1=soloPresenti (default); 2=tutti

          valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
            enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, FORMULATI);

          if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = +valore_impostazione;

          }
        }
      }


        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.findIndex(x => x.Elem_Cod === FERTILIZZANTI);

        //Per le ricette forzo la 180 a 2 tutti i prodotti senza neanche leggerla
        if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.TuttiProdotti;
        } else {

            valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, FERTILIZZANTI);

            if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

                obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = +valore_impostazione;

            }
        }

        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.findIndex(x => x.Elem_Cod === SEMENTI);

        //Per le ricette forzo la 180 a 2 tutti i prodotti senza neanche leggerla
        if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.TuttiProdotti;
        } else {
            valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, SEMENTI);

            if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

                obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = +valore_impostazione;

            }
        }

        index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.findIndex(x => x.Elem_Cod === INSETTI);

        //Per le ricette forzo la 180 a 2 tutti i prodotti senza neanche leggerla
        if (TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire) {
            obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.TuttiProdotti;
        } else {
            valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
                enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, INSETTI);

            if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

                obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = +valore_impostazione;

            }
        }

      index = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.findIndex(x=>x.Elem_Cod === INNESCHI);

      //Per le ricette forzo la 180 a 2 tutti i prodotti senza neanche leggerla
      if(TestataValue.Tipo === Tipo_Attivita.Ricetta && TestataValue.Stato === Stati.Da_Eseguire){
        obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = enum_Gestione_Giacenze.TuttiProdotti;
      }else{
        valore_impostazione = this.impostazioniaziendecentriService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva, 0,
          enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, INNESCHI);

        if (valore_impostazione !== undefined && valore_impostazione !== null && valore_impostazione !== '') {

          obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata[index].Valore = +valore_impostazione;

        }
      }

        return obj_Impostazioni_Magazzino_Lotto_Giacenze;
    }

    getImpostazioni_QdC_Centro_Azienda(Operazioni: Lavorazione[]) {

        let Piva = this.getImpresa_Model().partitaIva;

        let obj_Impostazioni_Magazzino_Lotto_Giacenze = this.getImpostazioni_Magazzino_Lotto_Giacenze(Piva,Operazioni);

        this.GestioneLotti_Formulati = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x => x.Elem_Cod === FORMULATI).Valore;

        this.GestioneLotti_Fertilizzanti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x => x.Elem_Cod === FERTILIZZANTI).Valore;

        this.GestioneLotti_Sementi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x => x.Elem_Cod === SEMENTI).Valore;

        this.GestioneLotti_Insetti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x => x.Elem_Cod === INSETTI).Valore;

        this.GestioneLotti_Inneschi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x=>x.Elem_Cod === INNESCHI).Valore;

        this.GestioneGiacenze_Formulati = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x => x.Elem_Cod === FORMULATI).Valore;

        this.GestioneGiacenze_Fertilizzanti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x => x.Elem_Cod === FERTILIZZANTI).Valore;

        this.GestioneGiacenze_Sementi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x => x.Elem_Cod === SEMENTI).Valore;

        this.GestioneGiacenze_Insetti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x => x.Elem_Cod === INSETTI).Valore;

        this.GestioneGiacenze_Inneschi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x=>x.Elem_Cod === INNESCHI).Valore;

        this.GestioneMagazzino_Abilitata_Formulati = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x => x.Elem_Cod === FORMULATI).Valore;

        this.GestioneMagazzino_Abilitata_Fertilizzanti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x => x.Elem_Cod === FERTILIZZANTI).Valore;

        this.GestioneMagazzino_Abilitata_Sementi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x => x.Elem_Cod === SEMENTI).Valore;

        this.GestioneMagazzino_Abilitata_Insetti = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x => x.Elem_Cod === INSETTI).Valore;

        this.GestioneMagazzino_Abilitata_Inneschi = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x=>x.Elem_Cod === INNESCHI).Valore;
    }

    //@description: Aggiorna la Giacenza (Qta) dei Prodotti
    AggiornaQtaProdotti(): void {
        if (this.obj_Inizializza_QdC && this.obj_Inizializza_QdC.ListaGiacenzeXProdotto && this.obj_Inizializza_QdC.ListaGiacenzeXProdotto.length > 0) {
            let Sezioni_Prodotti: Array<Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta | Sezione_Prodotto_Formulati> = this.Sezioni_ProdottoFormArray.getRawValue();

            if (Sezioni_Prodotti && Sezioni_Prodotti.length > 0) {

                for (let Sezione_Prodotto of Sezioni_Prodotti) {

                    let Operazione: Lavorazione = Sezione_Prodotto.Operazione;
                    let Categoria_Magazzino: number = Sezione_Prodotto.Categoria_Magazzino;
                    let Dosi_Prodotto: Array<any> = this.DosiProdottiFormArray(null, Sezione_Prodotto.Operazione).getRawValue();

                    for (let Dose_Prodotto of Dosi_Prodotto) {

                        let Prodotto: MultiColumnComboboxSemina | MultiColumnComboboxFertilizzazione | MultiColumnComboboxTrattamento = Dose_Prodotto.Prodotto;

                        if (Prodotto && Prodotto.prodotto.codice > 0) {

                            let magazziniMovimentazioni: RilevamentoDiMagazzino[] = [];

                            switch (Categoria_Magazzino) {
                                case INSETTI:
                                case FORMULATI:
                                    let Prodotto_Trattamento: MultiColumnComboboxTrattamento = Prodotto as MultiColumnComboboxTrattamento;

                                    magazziniMovimentazioni = this.obj_Inizializza_QdC.ListaGiacenzeXProdotto.find(g =>
                                        g.Categoria_Magazzino === FORMULATI &&
                                        g.dettaglioTrattamento.prodotto.codice === Prodotto_Trattamento.prodotto.codice &&
                                        g.dettaglioTrattamento.dettaglioProdotto === Prodotto_Trattamento.dettaglioProdotto &&
                                        g.dettaglioTrattamento.tipoFormulato === Prodotto_Trattamento.tipoFormulato
                                    )?.dettaglioTrattamento?.MagazziniMovimentazioni;
                                    break;
                                case FERTILIZZANTI:
                                    let Prodotto_Fertilizzante: MultiColumnComboboxFertilizzazione = Prodotto as MultiColumnComboboxFertilizzazione;

                                    magazziniMovimentazioni = this.obj_Inizializza_QdC.ListaGiacenzeXProdotto.find(g =>
                                        g.Categoria_Magazzino === FERTILIZZANTI &&
                                        g.dettaglioFertilizzazione.prodotto.codice === Prodotto_Fertilizzante.prodotto.codice
                                    )?.dettaglioFertilizzazione?.MagazziniMovimentazioni;
                                    break;
                                case SEMENTI:
                                    let Prodotto_Semente: MultiColumnComboboxSemina = Prodotto as MultiColumnComboboxSemina;

                                    magazziniMovimentazioni = this.obj_Inizializza_QdC.ListaGiacenzeXProdotto.find(g =>
                                        g.Categoria_Magazzino === SEMENTI &&
                                        g.dettaglioSemina.prodotto.codice === Prodotto_Semente.prodotto.codice &&
                                        g.dettaglioSemina.prodotto.elemCod === Prodotto_Semente.prodotto.elemCod
                                    )?.dettaglioSemina?.MagazziniMovimentazioni;
                                    break;
                            }

                            if (magazziniMovimentazioni && magazziniMovimentazioni.length > 0) {

                                if (Prodotto.MagazziniMovimentazioni) {
                                    for (let magazzinoMovimentazione of Prodotto.MagazziniMovimentazioni) {
                                        let magazzinoMovimentazione_Aggiornato = magazziniMovimentazioni.find(m =>
                                            m.Magazzino.primaryKey.codice === magazzinoMovimentazione.Magazzino.primaryKey.codice &&
                                            m.Magazzino.primaryKey.centroAziendalePK.codice === magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice &&
                                            m.Magazzino.primaryKey.centroAziendalePK.partitaIva === magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva &&
                                            m.Lotto === magazzinoMovimentazione.Lotto
                                        );

                                        if (magazzinoMovimentazione_Aggiornato) {
                                            magazzinoMovimentazione.Qta = magazzinoMovimentazione_Aggiornato.Qta;
                                            magazzinoMovimentazione.QtaTot = magazzinoMovimentazione_Aggiornato.QtaTot;
                                            Prodotto = this.setCodDescrMultiColumnComboboxProdotto(Categoria_Magazzino, Prodotto);
                                        }
                                    }
                                }
                            }
                        }

                        let Avversita: DropdownListAvversita | MultiColumnComboboxAvversitaInnesco = Dose_Prodotto.Avversita;

                        if(Avversita && Avversita.codice > 0){
                          let magazziniMovimentazioni: RilevamentoDiMagazzino[] = [];

                          magazziniMovimentazioni = this.obj_Inizializza_QdC.ListaGiacenzeXProdotto.find(g =>
                            g.Categoria_Magazzino === INNESCHI &&
                            g.avversitaGruppo.codice === Avversita.codice
                          )?.avversitaGruppo?.MagazziniMovimentazioni;

                          if (magazziniMovimentazioni && magazziniMovimentazioni.length > 0) {

                            if (Avversita.MagazziniMovimentazioni) {
                              for (let magazzinoMovimentazione of Avversita.MagazziniMovimentazioni) {
                                let magazzinoMovimentazione_Aggiornato = magazziniMovimentazioni.find(m =>
                                  m.Magazzino.primaryKey.codice === magazzinoMovimentazione.Magazzino.primaryKey.codice &&
                                  m.Magazzino.primaryKey.centroAziendalePK.codice === magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice &&
                                  m.Magazzino.primaryKey.centroAziendalePK.partitaIva === magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva &&
                                  m.Lotto === magazzinoMovimentazione.Lotto
                                );

                                if (magazzinoMovimentazione_Aggiornato) {
                                  magazzinoMovimentazione.Qta = magazzinoMovimentazione_Aggiornato.Qta;
                                  magazzinoMovimentazione.QtaTot = magazzinoMovimentazione_Aggiornato.QtaTot;
                                  Avversita = this.setCodDescrDdlAvversita(Avversita, Dose_Prodotto.Operazione.primaryKey.codice);
                                }
                              }
                            }
                          }
                        }
                    }

                    this.Sezione_ProdottoFormGroup(Operazione).patchValue({
                        DosiProdotti: Dosi_Prodotto
                    }, { emitEvent: false });
                }
            }
        }
    }

    setVariabiliGlobali(): void {
        this.setCONSIDERADETTAGLITERRENONUDO();
    }

    RicaricaGridMacchine(): Promise<boolean> {

        return new Promise<boolean>((resolve, reject) => {

            if (this.TestataForm.get('flagVisita').value)
                return resolve(false);

            //I default delle Macchine in Scrittura li carico dalla
            //tabella Profilazione_Dati, mentre in Modifica lo prendo dal modello RisorsaMacchine
            if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
                this.TipoRibaltamento === RibaltamentoTypes.Nessuno) {

                const LeggiProfilazione: LeggiProfilazione = {
                    impresa: this.getImpresa_Model(),
                    operazioni: this.TestataForm.get('Operazioni').getRawValue(),
                    specie: this.TestataForm.get('Specie').value,
                    data: this.TestataForm.get('Data').value,
                    macchine: [],
                    contatti: []
                };

                this.profilazioneservice.CaricaDefaultMacchine_QdC(LeggiProfilazione).then((result: Array<any>) => {

                    //Le macchine già presenti nella grid non li sovrascrivo ma li reinserisco e aggiungo le macchine dai default

                    let newFormArrayMacchine: Array<any> = this.MacchineFormArray.getRawValue();

                    if (result && result.length > 0) {
                        result.forEach(r => {
                            if (newFormArrayMacchine.findIndex(m => m.Risorsa_Cod === r.Risorsa_Cod) === -1) {
                                newFormArrayMacchine.push(r);
                            }
                        });
                    }

                    this.AggiornaFormArrayMacchine(newFormArrayMacchine);

                    if (this.GridMacchinePublicService !== undefined &&
                        this.GridMacchinePublicService !== null) {
                        this.GridMacchinePublicService.refresh(true);
                    }

                    resolve(true);
                });

            } else {
                resolve(true);
            }
        });


    }

    AggiornaFormArrayMacchine(GridRows: Array<any>): void {
        this.MacchineFormArray.clear();

        if (!GridRows)
            GridRows = this.GridMacchinePublicService.getValue()?.data?.rows;

        GridRows.forEach(r => {
            this.MacchineFormArray.push(new FormControl(r));
        });
    }

    AggiornaFormArrayNote(rows?): void {
        let GridRows = rows ? rows : this.GridNotePublicService?.getValue()?.data?.rows;
        if (!GridRows) return;

        this.NoteFormArray.clear();

        if (GridRows) {
            GridRows.forEach((r: GruppoNoteModel) => {
                r.Nota_Cod.forEach((nota_cod_value: number, nota_cod_index: number) => {
                    r.Gruppo_Cod.forEach((gruppo_cod: number) => {
                        this.NoteFormArray.push(new FormControl(new NotaInterventoDdlItem(nota_cod_value, r.Nota_Des[nota_cod_index], gruppo_cod)));
                    });
                });
            });
        }
    }

    RicaricaGridOperatori(): Promise<boolean> {

        return new Promise<boolean>((resolve, reject) => {

            if (this.TestataForm.get('flagVisita').value)
                return resolve(false);

            //I default delle Operatori in Scrittura li carico dalla
            //tabella Profilazione_Dati, mentre in Modifica lo prendo dal modello RisorsaPersona

            if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
                this.TipoRibaltamento === RibaltamentoTypes.Nessuno) {

                const LeggiProfilazione: LeggiProfilazione = {
                    impresa: this.getImpresa_Model(),
                    operazioni: this.TestataForm.get('Operazioni').getRawValue(),
                    specie: this.TestataForm.get('Specie').value,
                    data: this.TestataForm.get('Data').value,
                    macchine: [],
                    contatti: []
                };

                this.profilazioneservice.CaricaDefaultOperatori_QdC(LeggiProfilazione).then((result: Array<any>) => {

                    //Gli operatori già presenti nella grid non li sovrascrivo ma li reinserisco e aggiungo gli operatori dai default

                    let newFormArrayOperatori: Array<any> = this.OperatoriFormArray.getRawValue();

                    if (result && result.length > 0) {
                        result.forEach(r => {
                            if (newFormArrayOperatori.findIndex(m => m.Risorsa_Cod === r.Risorsa_Cod) === -1) {
                                newFormArrayOperatori.push(r);
                            }
                        });
                    }

                    this.AggiornaFormArrayOperatori(newFormArrayOperatori);

                    if (this.GridOperatoriPublicService !== undefined &&
                        this.GridOperatoriPublicService !== null) {
                        this.GridOperatoriPublicService.refresh(true);
                    }

                    resolve(true);
                });

            } else {
                resolve(true);
            }
        });

    }

    AggiornaFormArrayOperatori(GridRows: Array<any>): void {
        this.OperatoriFormArray.clear();

        if (!GridRows)
            GridRows = this.GridOperatoriPublicService.getValue()?.data?.rows;

        GridRows.forEach(r => {
            this.OperatoriFormArray.push(new FormControl(r));
        });
    }

    AggiornaRigheSelezionateGriglia(
        selectedRows: any[],
        deselectedRows: any[]
    ): void {

        let rows = this.GridImpiantiPublicService.getValue().data.rows;

        if (!this.Controlla_Se_Possibile_Frazionamento_Semina()) {

            rows.forEach(r => {
                selectedRows.forEach(selectedRow => {
                    if (r['PIVA'] === selectedRow.PIVA &&
                        r['SA_COD'] === selectedRow.SA_COD &&
                        r['APPEZZA'] === selectedRow.APPEZZA &&
                        r['ID_REG'] === selectedRow.ID_REG &&
                        r['Progetto_Cod'] === selectedRow.Progetto_Cod) {
                        r['Selected'] = false;
                    }
                });
            });

            return;

        }

        this.Controlla_DataCarenza().then((Impianti_Non_Conformi_Raccolta: Array<any>) => {

            //Reimposto l'array delle selected rows
            if (Impianti_Non_Conformi_Raccolta && Impianti_Non_Conformi_Raccolta.length > 0)
                selectedRows = rows.filter(r => r['Selected']);

            if (selectedRows.length !== 0) {

                selectedRows.forEach(row => {

                    // Imposto la superficie trattata uguale alla superficie impianto
                    // meno eventuali riduzioni

                    row.Sup_Imp_help = this.CalcolaSuperficieTrattata(
                        row.Sup_Imp,
                        row.Sup_Riduzione_BufferZone
                    );

                    // Negli impianti selezionati vado a memorizzare solamente
                    // le chiavi e non tutta la riga

                    this.AggiornaFormArrayImpiantiSelezionati();

                });

            } else if (deselectedRows.length !== 0) {

                deselectedRows.forEach(row => {

                    // Imposto la superficie trattata a 0 se deseleziono la riga



                    row.Sup_Imp_help = 0;
                    this.resetValuesForNotSelectedRows(row);
                    this.AggiornaFormArrayImpiantiSelezionati();
                });

            }

            this.eventiPostSelectionChangeGridImpianti.next(rows);

        });

    }

    public resetValuesForNotSelectedRows(row: any): void {
        let operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").getRawValue();
        const fertirrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
        const irrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;

        if (!fertirrigazione && !irrigazione) {
            return;
        }

        if (!row.Selected) {
            this.reimpostaValoriDefaultFertirrigazioneEIrrigazione(row, irrigazione);
        }
    }

    public reimpostaValoriDefaultFertirrigazioneEIrrigazione(row: any, irrigazione: boolean) {
        let data = this.TestataForm.get('Data').value;

        this.GridImpiantiHttpService.LeggiMacchinaIrrigazioneDefault(row.PIVA, row.SA_COD, row.APPEZZA, row.ID_REG).subscribe({
            next: (defaultImpiantoMacchina: any) => {
                row.Portata = defaultImpiantoMacchina.Portata;
                row.Efficienza = defaultImpiantoMacchina.Efficienza;
                row.tipoIrrigazione = defaultImpiantoMacchina.tipoIrrigazione;
                row.macchinaIrrigazione = defaultImpiantoMacchina.macchinaIrrigazione;
                row.IrrigazioneUtilizzata_Codice = defaultImpiantoMacchina.IrrigazioneUtilizzata_Codice;
                if (row.macchinaIrrigazione) {
                    row.IrrigazioneUtilizzata_Descrizione = '---  ' + this.translocoService.translate(`MacchineIrrigazione`) + '  --- ' + defaultImpiantoMacchina.IrrigazioneUtilizzata_Descrizione;
                } else {
                    row.IrrigazioneUtilizzata_Descrizione = '---  ' + this.translocoService.translate(`ImpiantoIrrigazione`) + '  --- ' + defaultImpiantoMacchina.IrrigazioneUtilizzata_Descrizione;
                }
            }
        });

        row.OreIrrigazione = 0;
        row.DoseAcquaGiornaliera = 0;
        row.DataInizioIrrigazione = data;
        row.DataFineIrrigazione = data;
        row.FrequenzaIrrigazioneMedia = 1;

        if (irrigazione) {
            //consiglio, data consiglio, dose consiglio
            row.Consiglio_Codice = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.Consiglio_Codice;
            row.Consiglio_Descrizione = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.Consiglio_Descrizione;
            row.DataConsiglio = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.DataConsiglio;
            row.DoseAcquaConsiglio = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.DoseAcquaConsiglio;
            row.minDataTurno = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.minDataTurno;
            row.maxDataTurno = this.GridImpiantiHttpService.defaultConsiglioIrrigazione.maxDataTurno;
        }
    }

    updateProdottiDaTrattareSelected(rows: ProdottoDaTrattareCDC[]): void {
        this.ProdottiDaTrattareSelezionatiFormArray.clear();
        for (const selected of rows) {
            this.ProdottiDaTrattareSelezionatiFormArray.push(new FormControl(selected));
        }
    }

    CalcolaSuperficieTrattata(
        superficieImpianto: number,
        superficieRiduzioneBufferZone: number
    ): number {
        let superficieTrattata = 0;
        let superficieRidottaBZ = superficieImpianto - superficieRiduzioneBufferZone;
        if (superficieRidottaBZ > 0) {
            superficieTrattata = superficieRidottaBZ;
        }
        return superficieTrattata;
    }

    /** @returns la superficie totale degli impianti selezionati */
    public get superficieImpiantiSelezionati(): number {
        const selected = this.ImpiantiForm.value.ImpiantiSelezionati;
        return selected.map(i => i.Sup_Imp).reduce((s1, s2) => s1 + s2);
    }

    AggiornaFormArrayImpiantiSelezionati() {
        this.ImpiantiSelezionatiFormArray.clear();
        let GridRows = this.GridImpiantiPublicService.getValue().data.rows;
        if (GridRows) {
            GridRows.forEach(row => {
              let rowGridimpianto = this.GetRowGridImpiantoSelezionatoModel(row,false);

              if(rowGridimpianto){
                this.ImpiantiSelezionatiFormArray.push(new FormControl(rowGridimpianto));
              }
            });
            this.SelezioneEsercizi.next(this.ImpiantiSelezionatiFormArray);
        }
    }

    public GetRowGridImpiantoSelezionatoModel(row: any,tutteleRighe: boolean):GridImpiantoSelezionatoModel{

      let gridImpianto: GridImpiantoSelezionatoModel = null;

      if (tutteleRighe || row.Selected) {

        let flag_N_Max: boolean = false;
        let flag_P_Max: boolean = false;
        let flag_K_Max: boolean = false;
        let flag_Mg_Max: boolean = false;
        let flag_CU_Max: boolean = false;
        let flag_Dichiarazione_non_Utilizzo_Trattamenti: boolean = false;
        let flag_Dichiarazione_non_Utilizzo_Fertilizzazioni: boolean = false;

        if (row.N_Max !== "") {
          flag_N_Max = true;
        }
        if (row.P_Max !== "") {
          flag_P_Max = true;
        }
        if (row.K_Max !== "") {
          flag_K_Max = true;
        }
        if (row.Mg_Max !== "") {
          flag_Mg_Max = true;
        }
        if (row.CU_Max !== "") {
          flag_CU_Max = true;
        }
        if (row.Dichiarazione_Non_Utilizzo_Fertilizzazioni !== "") {
          flag_Dichiarazione_non_Utilizzo_Fertilizzazioni = true;
        }
        if (row.Dichiarazione_Non_Utilizzo_Trattamenti !== "") {
          flag_Dichiarazione_non_Utilizzo_Trattamenti = true;
        }

         gridImpianto = new GridImpiantoSelezionatoModel(
                                                                          row.PIVA,
                                                                          row.SA_COD,
                                                                          row.APPEZZA,
                                                                          row.ID_REG,
                                                                          row.Progetto_Cod,
                                                                          row.GRFI_COD,
                                                                          row.Grfi_Des,
                                                                          row.Cop_Cod,
                                                                          row.Copertura,
                                                                          row.Flag_Protetto,
                                                                          row.Sup_Imp_help,
                                                                          row.Sup_Riduzione_BufferZone,
                                                                          row.Perc_Riduzione_Deriva,
                                                                          row.CUL_COD,
                                                                          row.Cul_Des,
                                                                          row.ListaClassiTessitura,
                                                                          row.Validita_Inizio_Distinta_Date,
                                                                          row.Validita_Fine_Distinta_Date,
                                                                          row.App_Nome_Anagrafica,
                                                                          row.Obj_Disciplinare,
                                                                          row.Regolamento,
                                                                          row.Data_Raccolta_Date,
                                                                          row.Data_Raccolta_Prevista_Date,
                                                                          row.CarenzaStr,
                                                                          row.DataCarenza,
                                                                          row.Sup_Imp,
                                                                          row.DistBZ_CorpiIdrici,
                                                                          row.DistBZ_AreeResPub,
                                                                          row.DistBZ_Allevamenti,
                                                                          row.DistBZ_VegNatNonColt,
                                                                          row.SupBZ_Riduzione,
                                                                          row.Progetto,
                                                                          row.Stato_Cod,
                                                                          row.IrrigazioneUtilizzata_Codice,
                                                                          row.IrrigazioneUtilizzata_Descrizione,
                                                                          row.Consiglio_Descrizione,
                                                                          row.Consiglio_Codice,
                                                                          row.DataConsiglio,
                                                                          row.DoseAcquaConsiglio,
                                                                          row.UdmConsiglio,
                                                                          row.minDataTurno,
                                                                          row.maxDataTurno,
                                                                          row.UdmDose,
                                                                          row.DoseAcquaGiornaliera,
                                                                          row.OreIrrigazione,
                                                                          row.Portata,
                                                                          row.Efficienza,
                                                                          row.DataInizioIrrigazione,
                                                                          row.DataFineIrrigazione,
                                                                          row.FrequenzaIrrigazioneMedia,
                                                                          row.tipoIrrigazione,
                                                                          row.macchinaIrrigazione,
                                                                          flag_N_Max,
                                                                          flag_P_Max,
                                                                          flag_K_Max,
                                                                          flag_Mg_Max,
                                                                          flag_CU_Max,
                                                                          row.N_Max_Decimal,
                                                                          row.P_Max_Decimal,
                                                                          row.K_Max_Decimal,
                                                                          row.Mg_Max_Decimal,
                                                                          row.CU_Max_Decimal,
                                                                          row.N_Residuo_Decimal,
                                                                          row.P_Residuo_Decimal,
                                                                          row.K_Residuo_Decimal,
                                                                          row.Mg_Residuo_Decimal,
                                                                          row.CU_Residuo_Decimal,
                                                                          row.N_Residuo_Percentuale,
                                                                          row.P_Residuo_Percentuale,
                                                                          row.K_Residuo_Percentuale,
                                                                          row.Mg_Residuo_Percentuale,
                                                                          row.CU_Residuo_Percentuale,
                                                                          flag_Dichiarazione_non_Utilizzo_Trattamenti,
                                                                          flag_Dichiarazione_non_Utilizzo_Fertilizzazioni);
      }

      return gridImpianto;
    }

    public AggiornaImpiantiSelezionatiGrid() {
        let rows = this.GridImpiantiPublicService.getValue().data.rows;

        this.ImpiantiSelezionatiFormArray.controls.forEach(c => {
            let C = c;
            let selectedRow = rows.find((r) => {
                return (C.value.PIVA == r.PIVA &&
                    C.value.SA_COD == r.SA_COD &&
                    C.value.APPEZZA == r.APPEZZA &&
                    C.value.ID_REG == r.ID_REG &&
                    C.value.Progetto_Cod == r.Progetto_Cod &&
                    C.value.GRFI_COD == r.GRFI_COD);
            });

            selectedRow.Sup_Imp_help = c.value.Sup_Imp_help;
            selectedRow.Sup_Riduzione_BufferZone = c.value.Sup_Riduzione_BufferZone;
        });

        this.RicaricaGridImpianti();
    }

    RicaricaGridImpianti() {
        if (this.GridImpiantiPublicService !== undefined &&
            this.GridImpiantiPublicService !== null) {

            this.GridImpiantiPublicService.refresh(true);
        }
    }

    RicaricaProdottiDaMagazzino() {
        if (this.GridProdottiDaTrattarePublicService != null) {
            this.GridProdottiDaTrattarePublicService.refresh(true);
        }
    }

    caricaCausali(lavcod: enum_LAVCOD, data: any) {

        let param = {
            LavCod: lavcod,
            Data: data
        } as LeggiOperazione;

        this.operazioneCausaleService.operazioneGetOperazioneCausale(param).subscribe(r => {

            this.listaOperazioniCausali = JSON.parse(r.RispostaStringa);
            this.CausaliFormControl.patchValue(this.listaOperazioniCausali.filter(item1 => this.CausaliFormControl.getRawValue().some(item2 => item1.Id == item2.Id)));

        });
    }

    ripartizionaQtaTrattata(): any {
        const supSelezionata: number = this.QuantitaForm.get('Qta_Selezionata').value;
        const supTrattata: number = this.QuantitaForm.get('Qta_Trattata').value ?? 0;
        const attr = this.funzionicomuniservice.roundNumber(supTrattata / supSelezionata,this.get4DecimalNumericSettings().decimals);

        const result = this.GridProdottiDaTrattarePublicService.getValue();
        const rows = (result?.data?.rows as any[]) ?? [];
        const filtered = rows.filter(x => x.Selected);
        const nSel = filtered.length;

        let somma = 0;
        for (const [idx, dataItem] of filtered.entries()) {
            let daInserire;
            const app = parseFloat(dataItem.giacenzaRilevata);
            if (nSel - 1 !== idx) {
                daInserire = this.funzionicomuniservice.roundNumber((app * attr), this.get4DecimalNumericSettings().decimals);
                somma += daInserire;
            } else {
                daInserire = (+supTrattata.toFixed(this.get4DecimalNumericSettings().decimals) - +somma.toFixed(this.get4DecimalNumericSettings().decimals)).toFixed(this.get4DecimalNumericSettings().decimals);
            }

            dataItem.qta =  this.funzionicomuniservice.roundNumber(+daInserire, this.get4DecimalNumericSettings().decimals);

        }

        return result;
    }

    public Controlla_Permesso_Modifica_Operatore = (row: any): boolean => {

        //L'utente può modificare il contatto se ha il permesso di scrittura dell'anagrafica,
        //e se il contatto è pubblico ma non sono proprietario di quel contatto devo avere il permesso
        //di modifica dei contatti pubblici

        let modifica = false;

        if (!this.abilitaGrid)
            return modifica;

        let Piva: string = this.objParametriAgenda.Piva;

        if (this.Scrittura_Modifica_Anagrafica_Contatto) {
            if (Piva !== row.Piva) {
                if (this.Modifica_Contatti_Pubblici)
                    modifica = true;
            } else {
                modifica = true;
            }
        }
        return modifica;

    }

    public Controlla_Permesso_Modifica_Macchina = (row: any): boolean => {

        //L'utente può modificare la macchina se ha il permesso di scrittura dell'anagrafica,
        //e se la macchina è pubblica ma non sono proprietario di quella macchina devo avere il permesso
        //di modifica delle macchine pubbliche

        let modifica = false;

        if (!this.abilitaGrid)
            return modifica;

        let Piva: string = this.objParametriAgenda.Piva;

        if (this.Scrittura_Modifica_Anagrafica_Macchina) {
            if (Piva !== row.Piva) {
                if (this.Modifica_Macchine_Pubbliche)
                    modifica = true;
            } else {
                modifica = true;
            }
        }
        return modifica;

    }


    getDefaultMacchinefromGrid(): Array<ParcoMacchine> {

        let ArrayDefaultMacchine: Array<ParcoMacchine> = [];

        if (this.GridMacchinePublicService !== undefined && this.GridMacchinePublicService !== null) {

            let GridMacchineRows = this.GridMacchinePublicService.getValue().data.rows;

            if (GridMacchineRows !== undefined && GridMacchineRows !== null) {

                GridMacchineRows.forEach(row => {

                    let DefaultMacchine = new ParcoMacchine();

                    DefaultMacchine.codice = row.Mac_Cod;

                    DefaultMacchine.partitaIva = row.Piva;

                    ArrayDefaultMacchine.push(DefaultMacchine);
                });
            }
        }

        return ArrayDefaultMacchine;

    }

    getDefaulOperatorifromGrid(): Array<Contatto> {

        let ArrayDefaultOperatori: Array<Contatto> = [];

        if (this.GridOperatoriPublicService !== undefined && this.GridOperatoriPublicService !== null) {

            let GridOperatoriRows = this.GridOperatoriPublicService.getValue().data.rows;

            if (GridOperatoriRows !== undefined && GridOperatoriRows !== null) {

                GridOperatoriRows.forEach(row => {

                    let DefaultOperatore = new Contatto();

                    DefaultOperatore.primaryKey = { codice: row.Cod_Contatto, partitaIva: row.Piva };

                    //Viene salvato il cod_risum nella tabella Profilazione_Dati

                    DefaultOperatore.risorseUmane = new Array<RisorseUmane>();

                    DefaultOperatore.risorseUmane.push(<RisorseUmane>{ codice: row.Cod_RisUm });

                    DefaultOperatore.risorseUmane[0].rapportoContabile = new RapportoContabile(row.Cod_Rapporto);

                    DefaultOperatore.risorseUmane[0].rapportoContabile.descrizione = row.Rapporto_Des;

                    ArrayDefaultOperatori.push(DefaultOperatore);
                });
            }

        }


        return ArrayDefaultOperatori;

    }

    Controlla_Righe_Doppie(tuttelerighe: KendoGridRow[], riga_inserita_modificata: KendoGridRow, actionType: string, vecchiaRigaEditata: KendoGridRow): any {

        //Controllo che non ci siano delle righe doppie nelle grid delle macchine
        //e degli operatori

        let obj_doppio = null;

        if (riga_inserita_modificata) {

            let obj: KendoGridRow[] = [];

            if (actionType === HttpAction.UPDATE && vecchiaRigaEditata) {
                obj = tuttelerighe.filter((obj) => obj['Risorsa_Cod'] === riga_inserita_modificata['Risorsa_Cod'] && obj['Risorsa_Cod'] !== vecchiaRigaEditata['Risorsa_Cod']);
            } else {
                obj = tuttelerighe.filter((obj) => obj['Risorsa_Cod'] === riga_inserita_modificata['Risorsa_Cod']);
            }



            if (obj.length >= 1) {
                obj_doppio = obj[0];
            }
        }

        return obj_doppio;

    }

    GetSpeciefromUtilizzoTerreno(): Specie {

        let utilizzoTerreno = this.QdCForm.get('Trattamento').get('Testata').get('Specie').value;

        let specie = new Specie(0);

        specie.descrizione = "";

        if (utilizzoTerreno != null && utilizzoTerreno.classType !== "DestinazioneUso") {
            specie.codice = utilizzoTerreno.codice;
            specie.descrizione = utilizzoTerreno.descrizione;
        }

        return specie;

    }

    GetAppezzamentiSelezionatiModel(): Appezzamento[] {

        let lstAppezzamento: Appezzamento[] = [];

        let lstImpianti: Impianto[] = this.GetImpiantiSelezionatiModel();

        if (lstImpianti && lstImpianti.length > 0) {
            lstImpianti.forEach((imp: Impianto) => {

                if (lstAppezzamento.findIndex((app: Appezzamento) => app.primaryKey === imp.primaryKey.appezzamentoPK) === -1) {

                    let appezzamentoNew: Appezzamento = new Appezzamento(imp.primaryKey.appezzamentoPK);

                    //Recupero la descrizione dell'appezzamneto dalla riga della griglia degli impianti
                    //selezionati

                    let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").value;

                    appezzamentoNew.descrizione = gridImpiantiSelezionati.find(i => {
                        return i.PIVA === appezzamentoNew.primaryKey.centroAziendalePK.partitaIva &&
                            i.SA_COD === appezzamentoNew.primaryKey.centroAziendalePK.codice &&
                            i.APPEZZA === appezzamentoNew.primaryKey.codice
                    }).App_Nome;

                    lstAppezzamento.push(appezzamentoNew);
                }

            });
        }

        return lstAppezzamento;
    }

    GetImpiantiSelezionatiModel(): Impianto[] {

        let lstImpianti: Impianto[] = [];

        let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").value;

        if (gridImpiantiSelezionati && gridImpiantiSelezionati.length > 0) {

            gridImpiantiSelezionati.forEach((row: GridImpiantoSelezionatoModel) => {

                let impiantoNew: Impianto = this.GetImpiantoSelezionatoModel(row);

                lstImpianti.push(impiantoNew);
            });

        }

        return lstImpianti;
    }

    public GetImpiantoSelezionatoModel(row: GridImpiantoSelezionatoModel): Impianto {

        let centro_aziendale = new CentroAziendale(new CentroAziendale.PK(row.SA_COD, row.PIVA));
        let appezzamentoPK = new Appezzamento.PK(row.APPEZZA, centro_aziendale.primaryKey);
        let impiantoPK = new Impianto.PK(row.ID_REG, appezzamentoPK);
        let impianto = new Impianto(impiantoPK);

        impianto.gruppoFinalita = new GruppoFinalita(row.GRFI_COD);
        impianto.gruppoFinalita.descrizione = row.Grfi_Des;

        impianto.copertura = new Copertura(row.Cop_Cod);

        impianto.copertura.descrizione = row.Copertura;

        let esercizio: Esercizio = this.GetEsercizioModel(row);

        if (esercizio) {

            impianto.esercizi = [];

            impianto.esercizi.push(esercizio);
        }


        return impianto;
    }

    public GetEsercizioModel(row: GridImpiantoSelezionatoModel): Esercizio {

        let esercizio = null;

        if(row){

            let centro_aziendale = new CentroAziendale(new CentroAziendale.PK(row.SA_COD, row.PIVA));

            esercizio = new Esercizio(row.Progetto_Cod);

            esercizio.impiantoPK = new Impianto.PK(row.ID_REG, new Appezzamento.PK(row.APPEZZA, centro_aziendale.primaryKey))

            esercizio.validita = new IntervalloTemporale(row.Validita_Inizio_Distinta, row.Validita_Fine_Distinta);

            esercizio.data_Raccolta = row.Data_Raccolta;

            esercizio.data_Raccolta_Prevista = row.Data_Raccolta_Prevista;
        }

        return esercizio;

    }

    public GetEsercizioCDCModel (row: GridImpiantoSelezionatoModel): EsercizioCDC{

        let esercizioCDCNew = null;

        if(row){

          esercizioCDCNew = new EsercizioCDC();

          let esercizioNew = new Esercizio(row.Progetto_Cod, row.Progetto_Des);

          esercizioNew.impiantoPK = this.GetImpiantoSelezionatoModel(row).primaryKey;

          esercizioCDCNew.esercizio = esercizioNew;

          esercizioCDCNew.superficieTrattata = row.Sup_Imp_help;

          esercizioCDCNew.superficieRiduzioneBufferZone = row.Sup_Riduzione_BufferZone;

          esercizioCDCNew.percentualeRiduzioneDeriva = row.Perc_Riduzione_Deriva;
        }

        return esercizioCDCNew;
    }

    GetClassiTessituraImpiantiSelezionati(): ClasseTessitura[] {

        let classiTessitura: ClasseTessitura[] = [];

        let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").value;

        if (gridImpiantiSelezionati) {
            gridImpiantiSelezionati.forEach((i: GridImpiantoSelezionatoModel) => {
                if (i.ListaClassiTessitura) {
                    i.ListaClassiTessitura.forEach((classeTessitura: ClasseTessitura) => {
                        if (!classiTessitura.some(c => c.codice === classeTessitura.codice))
                            classiTessitura.push(classeTessitura);
                    });
                }
            });
        }

        return classiTessitura;
    }

    GetEserciziCDCModel(): EsercizioCDC[] {

      let lstEserciziCDC: EsercizioCDC[] = [];

      let GridRows = this.GridImpiantiPublicService.getValue().data.rows;

      if (GridRows) {
        GridRows.forEach(row => {
          let rowGridimpianto = this.GetRowGridImpiantoSelezionatoModel(row,false);

          if(rowGridimpianto){
            lstEserciziCDC.push(this.GetEsercizioCDC(this.GetRowGridImpiantoSelezionatoModel(row,true)));
          }
        });
      }

      return lstEserciziCDC;
    }

    GetEserciziCDCSelezionatiModel(): EsercizioCDC[] {

        let lstEserciziCDC: EsercizioCDC[] = [];

        let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").value;

        if (gridImpiantiSelezionati && gridImpiantiSelezionati.length > 0) {

            gridImpiantiSelezionati.forEach((row: GridImpiantoSelezionatoModel) => {
                lstEserciziCDC.push(this.GetEsercizioCDC(row));
            });
        }

        return lstEserciziCDC;
    }

    GetEsercizioCDC(row: GridImpiantoSelezionatoModel): EsercizioCDC {

        let impiantoNew: Impianto = this.GetImpiantoSelezionatoModel(row);

        let esercizioCDCNew = new EsercizioCDC();

        let esercizioNew = new Esercizio(row.Progetto_Cod, row.Progetto_Des);

        esercizioNew.impiantoPK = impiantoNew.primaryKey;

        esercizioCDCNew.esercizio = esercizioNew;

        esercizioCDCNew.superficieTrattata = row.Sup_Imp_help;

        esercizioCDCNew.superficieRiduzioneBufferZone = row.Sup_Riduzione_BufferZone;

        esercizioCDCNew.percentualeRiduzioneDeriva = row.Perc_Riduzione_Deriva;

        esercizioCDCNew.esercizio.validita = new IntervalloTemporale(row.Validita_Inizio_Distinta, row.Validita_Fine_Distinta);

        return esercizioCDCNew;

    }

    Gestisci_Sezioni(Operazioni_Scelte: Array<Lavorazione>) {
        this.Gestisci_Sezioni_Prodotto(Operazioni_Scelte);
        this.Gestisci_Sezioni_Senza_Prodotto(Operazioni_Scelte);
    }

    Gestisci_Sezioni_Prodotto(Operazioni_Scelte: Array<Lavorazione>) {
        //Aggiungo anche una nuova Sezione di Prodotti se l'operazione lo consente

        if (Operazioni_Scelte && Operazioni_Scelte.length > 0) {
            Operazioni_Scelte.forEach((o: Lavorazione) => {

                let elem_cod = this.getCategoria_Magazzino(+o.primaryKey.codice);

                if (elem_cod > 0) {
                    if (this.Sezioni_ProdottoFormArray.value.findIndex(
                        (p) =>
                            p.Operazione.primaryKey.codice === o.primaryKey.codice
                    ) === -1) {
                        const Nuova_Sezione_Prodotto = this.getSezioneProdottiForm(elem_cod, o);

                        //Opzione di salvataggio della Semina
                        if (elem_cod === SEMENTI) {

                            let Default_Opzione_Semina = this.get_Default_Opzione_Semina(Operazioni_Scelte);

                            Nuova_Sezione_Prodotto.patchValue({
                                Opzioni_Semina: Default_Opzione_Semina
                            });


                        }

                        //Per la Distribuzione Ammendanti imposto come default la direttiva nitrati e se è l'unica operazione presente
                        //imposto il Disciplinare a null
                        if (+ o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) {
                            Nuova_Sezione_Prodotto.patchValue({
                                Utilizza_Direttiva_Nitrati: true
                            });

                            if (Operazioni_Scelte.findIndex(l => + l.primaryKey.codice !== enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI && this.Operazioni_Con_Disciplinare(l)) === -1) {
                                this.TestataForm.patchValue({
                                    Disciplinare: null
                                }, { emitEvent: false });
                            }
                        }

                        //Imposto la Modalita di Applicazione
                        if (elem_cod === FORMULATI || elem_cod === FERTILIZZANTI) {
                            Nuova_Sezione_Prodotto.patchValue({
                                Modalita_Applicazione: new BaseCodeDescr(0, "")
                            });
                        }

                        this.Sezioni_ProdottoFormArray.push(Nuova_Sezione_Prodotto);
                    }
                }


            });

            //Escludo le sezioni delle operazioni che sono state deselezionate
            const indexes = this.Sezioni_ProdottoFormArray.getRawValue()
                .map((p, index) => {
                    if (
                        Operazioni_Scelte.findIndex(
                            (o) =>
                                o.primaryKey.codice ===
                                p.Operazione.primaryKey.codice
                        ) === -1
                    ) {
                        return index;
                    }
                })
                .filter((p) => p >= 0);

            if (indexes) {
                indexes.forEach((index: number) => {
                    this.Sezioni_ProdottoFormArray.removeAt(index);
                });
            }
        } else {
            //Se non ci sono operazioni selezionate resetto il formArray
            this.Sezioni_ProdottoFormArray.clear();
        }


        if (this.Sezioni_ProdottoFormArray.getRawValue() && this.Sezioni_ProdottoFormArray.getRawValue().length > 0) {
            this.Mostra_Sezioni_Prodotto = true;
        } else {
            this.Mostra_Sezioni_Prodotto = false;
        }


    }

    /**
     * Checks the code of the operation (`lav.primaryKey.codice`) to see whether it is an operation without products.
     * Any operation which code is contained in `this.Elenco_Operazioni_Rilievi` or is equal to `enum_LAVCOD.ABBATTIMENTOIMPIANTI`
     * is considered to be an operation without products.
     * @param lavorazione the `Lavorazione` to check
     * @private
     */
    private isOperationWithoutProduct(lav: Lavorazione): boolean {
        const lavCod = +lav.primaryKey.codice;
        return lavCod == enum_LAVCOD.ABBATTIMENTOIMPIANTI || this.Elenco_Operazioni_Rilievi.includes(lavCod);
    }

    // Aggiunge/Rimuove sezioni senza prodotto in base alle attività selezionate
    Gestisci_Sezioni_Senza_Prodotto(Operazioni_Scelte: Array<Lavorazione>) {
        if (!Operazioni_Scelte || Operazioni_Scelte.length <= 0) {
            this.Sezioni_Senza_ProdottoFormArray.clear();
            this.Mostra_Sezioni_Senza_Prodotto = false;
            return;
        }

        // Aggiungo le nuove operazioni selezionate
        for (let lav of Operazioni_Scelte) {
            // Salto le sezioni che non sono operazioni senza prodotto
            if (!this.isOperationWithoutProduct(lav)) continue;

            let i = this.Sezioni_Senza_ProdottoFormArray.value.findIndex((sez) =>
                sez.Operazione.primaryKey.codice === lav.primaryKey.codice
            )
            if (i >= 0) continue;

            this.Sezioni_Senza_ProdottoFormArray.push(
                this.get_Sezione_Senza_Prodotto_Form(lav)
            )
        }

        // Rimuovo le operazioni deselezionate
        let deselected = this.Sezioni_Senza_ProdottoFormArray.value
            .filter(sez => !Operazioni_Scelte.map(o => o.primaryKey.codice).includes(sez.Operazione.primaryKey.codice))
            .map((op, i) => i);
        deselected.forEach(i => this.Sezioni_Senza_ProdottoFormArray.removeAt(i));

        this.Mostra_Sezioni_Senza_Prodotto = this.Sezioni_Senza_ProdottoFormArray.length > 0;
    }

    /**
     *
     * @param lav lavorazione per cui creare il FormGroup
     */
    public get_Sezione_Senza_Prodotto_Form(lav: Lavorazione): FormGroup {
        let lav_cod: enum_LAVCOD | number = 0;
        if (lav && +lav.primaryKey.codice)
            lav_cod = +lav.primaryKey.codice;

        let form: FormGroup = this.fb.group({})

        if (this.Elenco_Operazioni_Rilievi.includes(+lav.primaryKey.codice)) {
            let sezioneRilievi = new Sezione_Rilievi();
            sezioneRilievi.Operazione = lav
            return sezioneRilievi.toFormGroup();
            // form.addControl('Rilievi', sezioneRilievi.toFormGroup());
        }

        form.addControl('Operazione', new FormControl(lav))
        return form;
    }

    get_Default_Opzione_Semina(Operazioni_Scelte: Lavorazione[]): BaseCodeDescr {

        let Opzione_Semina_Model: BaseCodeDescr = null;

        let Opzione_Semina: number = enum_SEMINA_TIPO.Solo_Semina_Default;

        if(this.IsModalitaDemetra()){
          //Se siamo in modalità demetra forzo il default a Solo_Semina_Vincolo
          Opzione_Semina = enum_SEMINA_TIPO.Solo_Semina_Vincolo;
        }else{

          if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
            this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna) {

            if (Operazioni_Scelte && Operazioni_Scelte.findIndex(x=>+x.primaryKey.codice === enum_LAVCOD.SOVESCIO) > -1 ) {

              Opzione_Semina = enum_SEMINA_TIPO.Solo_Semina_Default;

            } else {

              let impostazione = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_COD_SEMINA_TIPO);

              if (impostazione && impostazione.Valore && impostazione.Valore !== "") {
                Opzione_Semina = +impostazione.Valore;
              } else {
                Opzione_Semina = enum_SEMINA_TIPO.Solo_Semina_Default;
              }
            }
          }

          //Se sono in un multi operazione e scelgo la semina come seconda o terza operazione e di default c'è il frazionamento o modifica appezzamenti
          //forzo il default a registra solo semina perchè non è possibile il fraizonamento con il multi operazione
          if((Operazioni_Scelte && Operazioni_Scelte.length > 1) &&
            (Opzione_Semina === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default ||
              Opzione_Semina === enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default)) {

            Opzione_Semina = enum_SEMINA_TIPO.Solo_Semina_Default;

          }

        }

        if(Opzione_Semina){
          switch (Opzione_Semina) {
            case enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo:
              Opzione_Semina = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default;
              break;
            case enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Vincolo:
              Opzione_Semina = enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default;
              break;
            case enum_SEMINA_TIPO.Solo_Semina_Vincolo:
              Opzione_Semina = enum_SEMINA_TIPO.Solo_Semina_Default;
              break;
          }
        }

        Opzione_Semina_Model = Elenco_Opzioni_Semina.find(o => o.codice === Opzione_Semina);

        return Opzione_Semina_Model;
    }

    getGestioneMagazzinoAbilitata(elem_cod: number) {

        let GestioneMagazzinoAbilitata: boolean = false;

        if (elem_cod === FERTILIZZANTI) {
            GestioneMagazzinoAbilitata = this.GestioneMagazzino_Abilitata_Fertilizzanti;
        } else if (elem_cod === FORMULATI) {
            GestioneMagazzinoAbilitata = this.GestioneMagazzino_Abilitata_Formulati;
        } else if (elem_cod === SEMENTI) {
            GestioneMagazzinoAbilitata = this.GestioneMagazzino_Abilitata_Sementi;
        } else if (elem_cod === INSETTI) {
            GestioneMagazzinoAbilitata = this.GestioneMagazzino_Abilitata_Insetti;
        }

        return GestioneMagazzinoAbilitata;

    }

    setCodDescrDdlCampo(ddlcampo: DropdownListCampo) {

        ddlcampo = this.setCodDdlCampo(ddlcampo);

        ddlcampo = this.setDescrDdlCampo(ddlcampo);

        return ddlcampo;
    }

    private getCarattere_Separatore_Codice_Concatenato(): string {
        return "|";
    }


    private setCodDdlCampo(ddlcampo: DropdownListCampo) {

        ddlcampo.Codice_Concatenato = "";

        if (ddlcampo.primaryKey) {
            if (ddlcampo.primaryKey.codice !== 0) {
                ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlcampo.primaryKey.codice}`;
            } else {
                ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
            }

            if (ddlcampo.primaryKey.centroAziendalePK) {

                if (ddlcampo.primaryKey.centroAziendalePK.codice !== 0) {
                    ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlcampo.primaryKey.centroAziendalePK.codice}`;
                } else {
                    ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
                }

                if (ddlcampo.primaryKey.centroAziendalePK.partitaIva !== "") {
                    ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlcampo.primaryKey.centroAziendalePK.partitaIva}`;
                } else {
                    ddlcampo.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}''`;
                }

            }
        }

        return ddlcampo;
    }

    private setDescrDdlCampo(ddlcampo: DropdownListCampo) {

        ddlcampo.Descrizione_Concatenata = ddlcampo.descrizione;

        return ddlcampo;

    }

    setCodDescrDdlDisciplinare(ddldisciplinare: DropdownListDisciplinare) {

        if (ddldisciplinare) {

            ddldisciplinare = this.setCodDdlDisciplinare(ddldisciplinare);

            ddldisciplinare = this.setDescrDdlDisciplinare(ddldisciplinare);

        }

        return ddldisciplinare;
    }

    private setCodDdlDisciplinare(ddldisciplinare: DropdownListDisciplinare) {

        ddldisciplinare.Codice_Concatenato = "";

        if (ddldisciplinare.codice && ddldisciplinare.codice !== "") {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.codice}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}''`;
        }

        if (ddldisciplinare.disciplinarePubblicoPrivato && ddldisciplinare.disciplinarePubblicoPrivato !== 0) {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.disciplinarePubblicoPrivato}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (ddldisciplinare.regolamentoConcimazione && ddldisciplinare.regolamentoConcimazione.codice !== 0) {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.regolamentoConcimazione.codice}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (ddldisciplinare.raggruppamentiColturaliDPI && ddldisciplinare.raggruppamentiColturaliDPI.codice !== 0) {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.raggruppamentiColturaliDPI.codice}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (ddldisciplinare.gruppoFinalita && ddldisciplinare.gruppoFinalita.codice !== 0) {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.gruppoFinalita.codice}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (ddldisciplinare.flagProtetto && ddldisciplinare.flagProtetto !== 0) {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddldisciplinare.flagProtetto}`;
        } else {
            ddldisciplinare.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        return ddldisciplinare;
    }

    private setDescrDdlDisciplinare(ddldisciplinare: DropdownListDisciplinare) {

        ddldisciplinare.Descrizione_Concatenata = ddldisciplinare.descrizione;

        /*if (ddldisciplinare.raggruppamentiColturaliDPI && ddldisciplinare.raggruppamentiColturaliDPI.descrizione !== "") {
            ddldisciplinare.Descrizione_Concatenata += "-" + ddldisciplinare.raggruppamentiColturaliDPI.descrizione;
        }*/

        return ddldisciplinare;

    }

    setCodDescrMultiColumnComboboxProdotto(elem_cod: number, multicolumncomboboxprodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina,
        data_operazione: Date = this.TestataForm.get("Data").value) {

        multicolumncomboboxprodotto = this.setCodMultiColumnComboboxProdotto(elem_cod, multicolumncomboboxprodotto);

        multicolumncomboboxprodotto.Descrizione_Concatenata = multicolumncomboboxprodotto.prodotto.descrizione;

        multicolumncomboboxprodotto.Tutti_Problemi_DettaglioProdotto = [];

        multicolumncomboboxprodotto.Problema_DettaglioProdotto_Da_Risolvere = enum_Problema_DettaglioProdotto.Nessuno;

        switch (elem_cod) {
            case FORMULATI:
            case INSETTI:
                multicolumncomboboxprodotto = this.setDescrizioneConcatenataMultiColumnComboboxTrattamento(multicolumncomboboxprodotto as MultiColumnComboboxTrattamento, elem_cod, data_operazione);
                break;
            case FERTILIZZANTI:
                multicolumncomboboxprodotto = this.setDescrizioneConcatenataMultiColumnComboboxFertilizzazione(multicolumncomboboxprodotto as MultiColumnComboboxFertilizzazione);
                break;
            case SEMENTI:
                multicolumncomboboxprodotto = this.setDescrizioneConcatenataMultiColumnComboboxSemina(multicolumncomboboxprodotto as MultiColumnComboboxSemina);
                break;
        }

        return multicolumncomboboxprodotto;
    }

    setCodDescrDdlMagazzino(ddlmagazzino: DropdownListMagazzino) {
        ddlmagazzino = this.setCodDdlMagazzino(ddlmagazzino);

        ddlmagazzino = this.setDescrDdlMagazzino(ddlmagazzino);

        return ddlmagazzino;
    }

    private setCodDdlMagazzino(ddlmagazzino: DropdownListMagazzino) {

        ddlmagazzino.Codice_Concatenato = "";

        if (ddlmagazzino.primaryKey) {
            if (ddlmagazzino.primaryKey.codice && ddlmagazzino.primaryKey.codice !== 0) {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlmagazzino.primaryKey.codice}`;
            } else {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
            }

            if (ddlmagazzino.primaryKey.codice && ddlmagazzino.primaryKey.centroAziendalePK.codice !== 0) {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlmagazzino.primaryKey.centroAziendalePK.codice}`;
            } else {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
            }

            if (ddlmagazzino.primaryKey.codice && ddlmagazzino.primaryKey.centroAziendalePK.partitaIva !== "") {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlmagazzino.primaryKey.centroAziendalePK.partitaIva}`;
            } else {
                ddlmagazzino.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}''`;
            }
        }

        return ddlmagazzino;
    }

    private setDescrDdlMagazzino(ddlmagazzino: DropdownListMagazzino) {

        ddlmagazzino.Descrizione_Concatenata = ddlmagazzino.descrizione;

        return ddlmagazzino;
    }

    private setCodMultiColumnComboboxProdotto(elem_cod: number, multicolumncomboboxprodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina) {

        multicolumncomboboxprodotto.Codice_Concatenato = multicolumncomboboxprodotto.prodotto.codice.toString();

        switch (elem_cod) {
            case FORMULATI:
            case INSETTI:
                multicolumncomboboxprodotto = multicolumncomboboxprodotto as MultiColumnComboboxTrattamento;

                if (multicolumncomboboxprodotto.dettaglioProdotto && multicolumncomboboxprodotto.dettaglioProdotto > 0) {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${multicolumncomboboxprodotto.dettaglioProdotto.toString()}`;
                } else {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
                }

                if (multicolumncomboboxprodotto.tipoFormulato && multicolumncomboboxprodotto.tipoFormulato > 0) {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${multicolumncomboboxprodotto.tipoFormulato.toString()}`;
                } else {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
                }

                if (multicolumncomboboxprodotto.dataSmaltimentoScorte) {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${multicolumncomboboxprodotto.dataSmaltimentoScorte.getTime().toString()}`;
                } else {
                    multicolumncomboboxprodotto.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
                }

                break;
            case FERTILIZZANTI:
                break;
            case SEMENTI:
                break;
        }

        multicolumncomboboxprodotto.Codice_Concatenato += this.ConcatCodiceMagazzino(multicolumncomboboxprodotto.MagazziniMovimentazioni);

        return multicolumncomboboxprodotto;

    }


    showInfoGiacenza(MagazziniMovimentazioni: RilevamentoDiMagazzino[]) {

        let descr = "";

        if (MagazziniMovimentazioni && MagazziniMovimentazioni && MagazziniMovimentazioni.length === 1) {

            let udm_simbolo = "";

            if(MagazziniMovimentazioni[0].udm){
                udm_simbolo = MagazziniMovimentazioni[0].udm.simbolo;
            }

            let magazzino_descr = "";

            if(MagazziniMovimentazioni[0].Magazzino){
                magazzino_descr = MagazziniMovimentazioni[0].Magazzino.descrizione;
            }

            let Qta = this.decimalpipe.transform(MagazziniMovimentazioni[0].Qta,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            let QtaTot = this.decimalpipe.transform(MagazziniMovimentazioni[0].QtaTot,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (magazzino_descr !== "") {
                descr += "[" + this.translocoService.translate("Magazzino") + ": " + magazzino_descr + "]";
            }

            descr += " --- [" + this.translocoService.translate("Giacenza") + ": " + this.translocoService.translate("AllaData") + " " + Qta + " " + udm_simbolo + "; " + this.translocoService.translate("Totale") + " " + QtaTot + " " + udm_simbolo + "]";

            if(MagazziniMovimentazioni[0].Lotto !== ""){
                descr += " --- [" + this.translocoService.translate("Lotto2")+": "+MagazziniMovimentazioni[0].Lotto+"]";
            }

            if(MagazziniMovimentazioni[0].Agenzia){
                descr += "<br> <strong> [" + this.translocoService.translate("MagazzinoAgenzia")+ ": "+MagazziniMovimentazioni[0].Agenzia.descrizione+"] </strong>";
            }

            if(MagazziniMovimentazioni[0].registrazioniCollegate && MagazziniMovimentazioni[0].registrazioniCollegate.length > 0){

              const Magazzino_Esterno_con_Lotto = this.getMagazzino_Esterno_con_Lotto(MagazziniMovimentazioni);

                if (Magazzino_Esterno_con_Lotto) {
                    if (Magazzino_Esterno_con_Lotto.Magazzino_Esterno && Magazzino_Esterno_con_Lotto.Magazzino_Esterno.primaryKey.codice !== 0) {
                        descr += "<br> <strong> [" + this.translocoService.translate("MagazzinoEsterno") + ": " + Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione + "] </strong>";

                        if (Magazzino_Esterno_con_Lotto.Lotto !== "") {
                    		descr += "<strong> --- [" + this.translocoService.translate("Lotto2")+": "+MagazziniMovimentazioni[0].Lotto+"] </strong>";
                        }
                    }
                }

            }

        }

        return descr;

    }

    private setDescrizioneConcatenataMultiColumnComboboxTrattamento(multicolumncomboboxtrattamento: MultiColumnComboboxTrattamento, Categoria_Magazzino: number, data_operazione: Date) {

        if (Categoria_Magazzino === FORMULATI) {

            if (multicolumncomboboxtrattamento.prodotto && multicolumncomboboxtrattamento.prodotto.codice > 0)
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " (" + multicolumncomboboxtrattamento.prodotto.codice + ")";

            if (multicolumncomboboxtrattamento.principiAttivi && multicolumncomboboxtrattamento.principiAttivi.length > 0)
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + multicolumncomboboxtrattamento.principiAttivi.map(p => p.descrizione).join(",") + "]";

            if (multicolumncomboboxtrattamento.classificazioni && multicolumncomboboxtrattamento.classificazioni !== "")
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + multicolumncomboboxtrattamento.classificazioni + "]";

            if (multicolumncomboboxtrattamento.bufferzone && (multicolumncomboboxtrattamento.bufferzone.massimo > 0 || multicolumncomboboxtrattamento.bufferzone.minimo > 0)) {
                if (multicolumncomboboxtrattamento.bufferzone.minimo > 0 && multicolumncomboboxtrattamento.bufferzone.massimo === 0) {
                        multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- ["+this.translocoService.translate("Buffer_Zone")+" "+this.decimalpipe.transform(multicolumncomboboxtrattamento.bufferzone.minimo,this.digitsInfo_QdC_4_Decimal,this.locale_id)+" "+this.translocoService.translate("MetriAbbr")+"]";
                } else {
                        multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- ["+this.translocoService.translate("Buffer_Zone")+" "+this.decimalpipe.transform(multicolumncomboboxtrattamento.bufferzone.minimo,this.digitsInfo_QdC_4_Decimal,this.locale_id)+" - "+this.decimalpipe.transform(multicolumncomboboxtrattamento.bufferzone.massimo,this.digitsInfo_QdC_4_Decimal,this.locale_id)+" "+this.translocoService.translate("MetriAbbr")+"]";
                }
            }

            if (multicolumncomboboxtrattamento.tempoCarenza) {
                if (multicolumncomboboxtrattamento.tempoCarenza !== 0)
                        multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- ["+this.translocoService.translate("CarenzaStr")+" "+this.decimalpipe.transform(multicolumncomboboxtrattamento.tempoCarenza,this.digitsInfo_QdC_4_Decimal,this.locale_id)+" "+this.translocoService.translate("GiorniAbbr")+"]";

                if (this.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare)
                    multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + this.translocoService.translate("qdc.BoundFieldResource16.HeaderText") + " " + this.getDataPrimaRaccoltaUtile(multicolumncomboboxtrattamento.tempoCarenza, data_operazione) + "]";
            }

            if (multicolumncomboboxtrattamento.polverulento && multicolumncomboboxtrattamento.polverulento > 0) {
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + this.translocoService.translate("Polverulento") + "]";
                multicolumncomboboxtrattamento.Polverulento_Str = "X";
            } else {
                multicolumncomboboxtrattamento.Polverulento_Str = "";
            }

            if (multicolumncomboboxtrattamento.dataSmaltimentoScorte && multicolumncomboboxtrattamento.dataSmaltimentoScorte.getTime() !== AGRODATAINIZIO.getTime() && multicolumncomboboxtrattamento.dataSmaltimentoScorte.getTime() !== AGRODATAFINE.getTime())
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + this.translocoService.translate("FineScorta") + " " + this.datepipe.transform(multicolumncomboboxtrattamento.dataSmaltimentoScorte, 'dd/MM/yyyy', undefined, this.locale_id) + "]";

                //durataFeromone e scadenzaFeromone valorizzati solo nel lav_Cod CONFUSIONE_DISORIENTAMENTO_SESSUALE e INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA
            if (multicolumncomboboxtrattamento.durataFeromone > 0) {
                multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + this.translocoService.translate("qdc.DurataFeromoneGG") + " " + multicolumncomboboxtrattamento.durataFeromone + "]";
                if (multicolumncomboboxtrattamento.scadenzaFeromone && multicolumncomboboxtrattamento.scadenzaFeromone.getTime() !== AGRODATAINIZIO.getTime() && multicolumncomboboxtrattamento.scadenzaFeromone.getTime() !== AGRODATAFINE.getTime())
                    multicolumncomboboxtrattamento.Descrizione_Concatenata += " --- [" + this.translocoService.translate("Scadenza") + " " + this.datepipe.transform(multicolumncomboboxtrattamento.scadenzaFeromone, 'dd/MM/yyyy', undefined, this.locale_id) + "]";
            }
        }


        return multicolumncomboboxtrattamento;
    }

    private setDescrizioneConcatenataMultiColumnComboboxFertilizzazione(multicolumncomboboxfertilizzazione: MultiColumnComboboxFertilizzazione) {

        //Aggiungo l'asterisco se i valori N/P/K/Cu sono ponderati

        if (multicolumncomboboxfertilizzazione.N !== null) {
           multicolumncomboboxfertilizzazione.N_Str = this.decimalpipe.transform(multicolumncomboboxfertilizzazione.N,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (multicolumncomboboxfertilizzazione.N_Ponderato)
                multicolumncomboboxfertilizzazione.N_Str += "*";
        }

        if (multicolumncomboboxfertilizzazione.P !== null) {
           multicolumncomboboxfertilizzazione.P_Str = this.decimalpipe.transform(multicolumncomboboxfertilizzazione.P,this.digitsInfo_QdC_4_Decimal,this.locale_id);


            if (multicolumncomboboxfertilizzazione.P_Ponderato)
                multicolumncomboboxfertilizzazione.P_Str += "*";
        }

        if (multicolumncomboboxfertilizzazione.K !== null) {
        	multicolumncomboboxfertilizzazione.K_Str = this.decimalpipe.transform(multicolumncomboboxfertilizzazione.K,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (multicolumncomboboxfertilizzazione.K_Ponderato)
                multicolumncomboboxfertilizzazione.K_Str += "*";
        }

        if (multicolumncomboboxfertilizzazione.Cu !== null) {
        multicolumncomboboxfertilizzazione.Cu_Str = this.decimalpipe.transform(multicolumncomboboxfertilizzazione.Cu,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (multicolumncomboboxfertilizzazione.Cu_Ponderato)
                multicolumncomboboxfertilizzazione.Cu_Str += "*";
        }

        if (multicolumncomboboxfertilizzazione.N_Str !== "" && multicolumncomboboxfertilizzazione.P_Str !== "" &&
            multicolumncomboboxfertilizzazione.K_Str !== "" && multicolumncomboboxfertilizzazione.Cu_Str !== "") {

            multicolumncomboboxfertilizzazione.Descrizione_Concatenata += ' (' + multicolumncomboboxfertilizzazione.N_Str + '-' + multicolumncomboboxfertilizzazione.P_Str;

            multicolumncomboboxfertilizzazione.Descrizione_Concatenata += '-' + multicolumncomboboxfertilizzazione.K_Str + '-' + multicolumncomboboxfertilizzazione.Cu_Str + ') ';

        }

        //Informazioni degli effluenti utilizzati nel PUA
        if (multicolumncomboboxfertilizzazione.effluente && multicolumncomboboxfertilizzazione.effluente.carico > 0) {

        multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str = this.decimalpipe.transform(multicolumncomboboxfertilizzazione.effluente.carico,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (multicolumncomboboxfertilizzazione.effluente.udm && multicolumncomboboxfertilizzazione.effluente.udm.simbolo !== "") {
                multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str += " " + multicolumncomboboxfertilizzazione.effluente.udm.simbolo;
            }

            if (multicolumncomboboxfertilizzazione.effluente.N > 0){
                multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str += " - " + this.decimalpipe.transform(multicolumncomboboxfertilizzazione.effluente.N,this.digitsInfo_QdC_4_Decimal,this.locale_id) + " " + this.translocoService.translate('n');
            } else {
                multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str += " - " + this.decimalpipe.transform(multicolumncomboboxfertilizzazione.N, this.digitsInfo_QdC_4_Decimal,this.locale_id) + " " + this.translocoService.translate('n');
            }

            multicolumncomboboxfertilizzazione.Descrizione_Concatenata += "--- [" + this.translocoService.translate('qdc.Dichiarato_nel_PUA') + ": " + multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str + "]";
        } else {
            multicolumncomboboxfertilizzazione.Dichiarato_nel_PUA_Str = "";
        }

        return multicolumncomboboxfertilizzazione;
    }

    private setDescrizioneConcatenataMultiColumnComboboxSemina(multicolumncomboboxsemina: MultiColumnComboboxSemina) {

        if (multicolumncomboboxsemina.codArticolo &&
            multicolumncomboboxsemina.codArticolo !== "")
            multicolumncomboboxsemina.Descrizione_Concatenata += ' - ' + multicolumncomboboxsemina.codArticolo;

        return multicolumncomboboxsemina;
    }


    setCodDescrConcatenataMultiColumnComboboxMultiColumnComboboxDose_Etichetta(multicolumncomboboxdose_etichetta: MultiColumnComboboxDose_Etichetta) {

        if (multicolumncomboboxdose_etichetta.codice === 0) {

            multicolumncomboboxdose_etichetta.Codice_Concatenato = multicolumncomboboxdose_etichetta.codice.toString();

            if (multicolumncomboboxdose_etichetta.descrizione && multicolumncomboboxdose_etichetta.descrizione !== "")
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata = multicolumncomboboxdose_etichetta.descrizione;
        } else {

            multicolumncomboboxdose_etichetta.Codice_Concatenato = multicolumncomboboxdose_etichetta.CodiceConcatenato;

            //Imposta la descrizione concatenata della Dose Etichetta

            multicolumncomboboxdose_etichetta.Descrizione_Concatenata = this.decimalpipe.transform(multicolumncomboboxdose_etichetta.DoseMin,this.digitsInfo_QdC_4_Decimal,this.locale_id) + "-" + this.decimalpipe.transform(multicolumncomboboxdose_etichetta.DoseMax,this.digitsInfo_QdC_4_Decimal,this.locale_id);

            if (multicolumncomboboxdose_etichetta.Udm) {

                let Udm_Sim: string = multicolumncomboboxdose_etichetta.Udm.simbolo;

                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " " + Udm_Sim;
            }

            if (multicolumncomboboxdose_etichetta.UdmAcqua && multicolumncomboboxdose_etichetta.AcquaMin && multicolumncomboboxdose_etichetta.AcquaMax) {

                let AcquaUdm_Sim: string = multicolumncomboboxdose_etichetta.UdmAcqua.simbolo;

                let AcquaMin: number = multicolumncomboboxdose_etichetta.AcquaMin;

                let AcquaMax: number = multicolumncomboboxdose_etichetta.AcquaMax;

                if (AcquaMin !== 0 || AcquaMax !== 0)
                    multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " (Vol.Acqua " + this.decimalpipe.transform(AcquaMin,this.digitsInfo_QdC_4_Decimal,this.locale_id) + "-" + this.decimalpipe.transform(AcquaMax,this.digitsInfo_QdC_4_Decimal,this.locale_id) + " " + AcquaUdm_Sim + ")";
            }

            if (multicolumncomboboxdose_etichetta.UdmLimite && multicolumncomboboxdose_etichetta.Limite) {

                let Limite_Sim: string = multicolumncomboboxdose_etichetta.UdmLimite.simbolo;

                let Limite: number = multicolumncomboboxdose_etichetta.Limite;

                if (Limite !== 0)
                    multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " (Max " + Limite + " interventi " + Limite_Sim + ")";
            }


            if (multicolumncomboboxdose_etichetta.IntervalloTrattamenti_Min !== 0 || multicolumncomboboxdose_etichetta.IntervalloTrattamenti_Max !== 0)
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " da effettuare da " + multicolumncomboboxdose_etichetta.IntervalloTrattamenti_Min + " - " + multicolumncomboboxdose_etichetta.IntervalloTrattamenti_Max + " gg. dal precedente trattamento";

            if (multicolumncomboboxdose_etichetta.Epoca_Des && multicolumncomboboxdose_etichetta.Epoca_Des !== "")
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += multicolumncomboboxdose_etichetta.Epoca_Des;

            if (multicolumncomboboxdose_etichetta.Mdi && multicolumncomboboxdose_etichetta.Mdi.descrizione !== "")
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " - " + multicolumncomboboxdose_etichetta.Mdi.descrizione;

            if (multicolumncomboboxdose_etichetta.Flag_Protetto && multicolumncomboboxdose_etichetta.Flag_Protetto.descrizione !== "")
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " - " + multicolumncomboboxdose_etichetta.Flag_Protetto.descrizione;

            if (multicolumncomboboxdose_etichetta.DataSmaltimentoScorte && multicolumncomboboxdose_etichetta.DataSmaltimentoScorte !== "")
                multicolumncomboboxdose_etichetta.Descrizione_Concatenata += " --- [Fine Scorta al " + multicolumncomboboxdose_etichetta.DataSmaltimentoScorte + "]";
        }



        return multicolumncomboboxdose_etichetta;

    }

    setCodDescrDdlAttivitaPersonalizzata(dropdownlistattivitapersonalizzata: DropdownListAttivitaPersonalizzata) {

        dropdownlistattivitapersonalizzata.Codice_Concatenato = dropdownlistattivitapersonalizzata.codice.toString();

        if (dropdownlistattivitapersonalizzata.sigla && dropdownlistattivitapersonalizzata.sigla !== "") {
            dropdownlistattivitapersonalizzata.Descrizione_Concatenata = dropdownlistattivitapersonalizzata.sigla + " - " + dropdownlistattivitapersonalizzata.descrizione;
        } else {
            dropdownlistattivitapersonalizzata.Descrizione_Concatenata = dropdownlistattivitapersonalizzata.descrizione;
        }

        return dropdownlistattivitapersonalizzata;
    }

    setCodDescrDdlAvversita(ddlavversita: DropdownListAvversita | MultiColumnComboboxAvversitaInnesco, Lav_Cod: number){
        let Categoria_Magazzino = this.getCategoria_Magazzino(Lav_Cod);

        ddlavversita.Codice_Concatenato = ddlavversita.codice.toString();

        //In molti casi l'avversita gruppo codice è 0 quindi se siamo in un'avversità senza gruppo concateno il -1 come valore
        if (ddlavversita.gruppo && ddlavversita.gruppo.codice >= 0) {
            ddlavversita.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${ddlavversita.gruppo.codice.toString()}`;
        } else {
            ddlavversita.Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}-1`;
        }

        ddlavversita.Descrizione_Concatenata = ddlavversita.descrizione;

        if (Categoria_Magazzino === FORMULATI) {
            if (ddlavversita.dataSmaltimentoScorte && ddlavversita.dataSmaltimentoScorte.getTime() !== AGRODATAINIZIO.getTime() && ddlavversita.dataSmaltimentoScorte.getTime() !== AGRODATAFINE.getTime())
                ddlavversita.Descrizione_Concatenata += " --- [" + this.translocoService.translate("FineScorta") + " " + this.datepipe.transform(ddlavversita.dataSmaltimentoScorte, 'dd/MM/yy', undefined, this.locale_id) + "]";
        }

        //La descrizione delle avversita degli insetti arrivano con l'html
        if (Categoria_Magazzino === INSETTI) {
            ddlavversita.Descrizione_Concatenata = this.decodeHtmlText(ddlavversita.Descrizione_Concatenata);

            ddlavversita.descrizione = this.decodeHtmlText(ddlavversita.descrizione);
        }

        return ddlavversita;
    }

    setCodDescrConcatenataMultiColumnComboboxAvversitaInnesco(multicolumncomboboxavversitaInnesco: MultiColumnComboboxAvversitaInnesco, Lav_Cod: number){
      this.setCodDescrDdlAvversita(multicolumncomboboxavversitaInnesco,Lav_Cod);

      multicolumncomboboxavversitaInnesco.Codice_Concatenato += this.ConcatCodiceMagazzino(multicolumncomboboxavversitaInnesco.MagazziniMovimentazioni);

      return multicolumncomboboxavversitaInnesco;
    }

    private ConcatCodiceMagazzino(MagazziniMovimentazioni: RilevamentoDiMagazzino[]): string{

      let Codice_Concatenato :string = "";

      if (MagazziniMovimentazioni && MagazziniMovimentazioni.length === 1) {

        if (MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.partitaIva &&
          MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.partitaIva !== "") {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.partitaIva}`;

        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}''`;
        }

        if (MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice &&
          MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice !== 0) {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice}`;

        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (MagazziniMovimentazioni[0].Magazzino.primaryKey.codice &&
          MagazziniMovimentazioni[0].Magazzino.primaryKey.codice !== 0) {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].Magazzino.primaryKey.codice}`;

        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (MagazziniMovimentazioni[0].Lotto &&
          MagazziniMovimentazioni[0].Lotto !== "") {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].Lotto}`;
        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}''`;
        }

        if (MagazziniMovimentazioni[0].Qta &&
          MagazziniMovimentazioni[0].Qta !== 0) {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].Qta}`;
        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (MagazziniMovimentazioni[0].QtaTot &&
          MagazziniMovimentazioni[0].QtaTot !== 0) {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].QtaTot}`;
        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

        if (MagazziniMovimentazioni[0].udm &&
          MagazziniMovimentazioni[0].udm.codice !== 0) {

          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}${MagazziniMovimentazioni[0].udm.codice}`;
        } else {
          Codice_Concatenato += `${this.getCarattere_Separatore_Codice_Concatenato()}0`;
        }

      } else {
        Codice_Concatenato += `''${this.getCarattere_Separatore_Codice_Concatenato()}0${this.getCarattere_Separatore_Codice_Concatenato()}0${this.getCarattere_Separatore_Codice_Concatenato()}''${this.getCarattere_Separatore_Codice_Concatenato()}0${this.getCarattere_Separatore_Codice_Concatenato()}0${this.getCarattere_Separatore_Codice_Concatenato()}0`;
      }

      return Codice_Concatenato;
    }

    ImpostaValoriDefaultXOperazione(testata: Testata) {

        let Operazioni: Array<Lavorazione> = testata.Operazioni;

        if (Operazioni.findIndex(o => {
            return +o.primaryKey.codice === enum_LAVCOD.ALTRE_OPERAZIONI;
        }) > -1) {

            if (!testata.Attivita_Personalizzata) {
                const attivitapersonalizzata = new AttivitaPersonalizzata(0);

                attivitapersonalizzata.descrizione = "";

                testata.Attivita_Personalizzata = this.setCodDescrDdlAttivitaPersonalizzata(attivitapersonalizzata as DropdownListAttivitaPersonalizzata);
            }

            if (!testata.Descrizione_Altre_Lavorazioni)
                testata.Descrizione_Altre_Lavorazioni = "";

        } else {
            testata.Attivita_Personalizzata = null;
            testata.Descrizione_Altre_Lavorazioni = null;
        }

        if (Operazioni.findIndex(o => {
            return this.MostraDisciplinare(o, this.Sezioni_ProdottoFormArray);
        }) > -1) {

            if (!testata.Disciplinare) {
                testata.Disciplinare = this.setCodDescrDdlDisciplinare(this.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);
            }
        } else {
            testata.Disciplinare = null;
        }


        return testata;
    }

    getDisciplinareModelValue(lav_cod: number): Disciplinare {

        let disciplinare: Disciplinare = null;

        if (lav_cod === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) {

            const Sezioni_ProdottoFormArray = this.Sezioni_ProdottoFormArray;

            let ProdottiForm = Sezioni_ProdottoFormArray.controls.find((form: FormGroup) => {

                let sezione: Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta = form.getRawValue();

                if (sezione && +sezione.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)
                    return form;

            }) as FormGroup;

            if (ProdottiForm) {

                if (ProdottiForm.get("Utilizza_Direttiva_Nitrati").value) {
                    let direttiva_nitrati: Disciplinare = ProdottiForm.get("Direttiva_Nitrati").value;

                    if (direttiva_nitrati)
                        disciplinare = direttiva_nitrati
                } else {
                    disciplinare = this.TestataForm.get("Disciplinare").value;
                }

            }

        } else {
            disciplinare = this.TestataForm.get("Disciplinare").value;
        }

        return disciplinare;

    }

    ImpostaDisciplinare_Dagli_ImpiantiSelezionati(Array_Disciplinari: Array<DropdownListDisciplinare>) {
        //Impostare il disciplinare prendendo il più restrittivo tra quelli selezionati
        //Spiegazione teorica di questa funzione in \\rubino2\DOCUMENTAZIONE\GIAS --- Agenda 2021 Angular e CoreWS\nuove_operazioni_disciplinare.docx
        return new Promise<boolean>(async (resolve, reject) => {

            let dpi = await this.get_Disciplinare_Dagli_ImpiantiSelezionati(Array_Disciplinari);

            this.TestataForm.patchValue({
                Disciplinare: dpi
            });


            resolve(true);
        });
    }

    get_Disciplinare_Dagli_ImpiantiSelezionati(Array_Disciplinari: Array<DropdownListDisciplinare>) {
        //Impostare il disciplinare prendendo il più restrittivo tra quelli selezionati
        //Spiegazione teorica di questa funzione in \\rubino2\DOCUMENTAZIONE\GIAS --- Agenda 2021 Angular e CoreWS\nuove_operazioni_disciplinare.docx
        return new Promise<DropdownListDisciplinare>(async (resolve, reject) => {

            let disciplinare: DropdownListDisciplinare = null;

            let Veg_Cod = this.GetSpeciefromUtilizzoTerreno().codice;

            //Se è stata selezionata una destinazione d'uso c'è solo il Disciplinare Nessun Disciplinare Nessun Vincolo.
            if (Veg_Cod > 0) {

                let impianti_selezionati: Array<GridImpiantoSelezionatoModel> = this.ImpiantiSelezionatiFormArray.getRawValue();

                if (impianti_selezionati && impianti_selezionati.length > 0) {

                    let impianti_filtrati: Array<GridImpiantoSelezionatoModel> = [];

                    //1° Caso: Se tra gli impianti selezionati c'è almeno un impianto con Regolamento Bio allora il disciplinare sarà quello del Bio
                    impianti_filtrati = impianti_selezionati.filter(imp => {
                        return imp.Regolamento_Cod && imp.Regolamento_Cod === enum_Cod_Regolamento.Regolamento_bio;
                    });

                    if (impianti_filtrati && impianti_filtrati.length > 0) {
                        disciplinare = Array_Disciplinari.find(d => d.codice === DpiBio);
                    } else {

                        impianti_filtrati = impianti_selezionati.filter(imp => {
                            return imp.Obj_Disciplinare && imp.Obj_Disciplinare.codice !== "" && imp.Obj_Disciplinare.codice !== "0"
                        });

                        if (impianti_filtrati && impianti_filtrati.length > 0) {

                            //2° Caso: Se tra gli impianti selezionati c'è almeno un impianto con stesso Disciplinare codice,stesso flag Protetto e stesso Gruppo Finalità allora il disciplinare sarà quello
                            for (var d of Array_Disciplinari) {

                                for (var impianto of impianti_filtrati) {

                                    if (d.codice === impianto.Obj_Disciplinare.codice &&
                                        d.flagProtetto === impianto.Obj_Disciplinare.flagProtetto &&
                                        (d.gruppoFinalita && d.gruppoFinalita.codice === impianto.Obj_Disciplinare.gruppoFinalita.codice)) {

                                        disciplinare = d;

                                        break;

                                    }
                                }

                                if (disciplinare)
                                    break;
                            }

                            //3° Caso: Se tra gli impianti selezionati c'è almeno un impianto con stesso Disciplinare codice,stesso flag Protetto e Gruppo Finalità compatibile allora il disciplinare sarà quello
                            if (!disciplinare) {

                                for (var d of Array_Disciplinari) {

                                    if (impianti_filtrati.findIndex(impianto => d.codice === impianto.Obj_Disciplinare.codice &&
                                        d.flagProtetto === impianto.Obj_Disciplinare.flagProtetto &&
                                        this.Controlla_GruppoFinalita_Disciplinari(d, impianto.Obj_Disciplinare)) > -1) {

                                        disciplinare = d;

                                        break;

                                    }
                                }
                            }

                            //4° Caso: Se tra gli impianti selezionati c'è almeno un impianto con stesso Disciplinare codice, flag Protetto compatibile e stesso Gruppo Finalità allora il disciplinare sarà quello
                            if (!disciplinare) {

                                for (var d of Array_Disciplinari) {

                                    if (impianti_filtrati.findIndex(impianto => d.codice === impianto.Obj_Disciplinare.codice &&
                                        this.Controlla_Flag_Protetto_Impianti_con_Disciplinari(impianto.Obj_Disciplinare, d) &&
                                        (d.gruppoFinalita && d.gruppoFinalita.codice === impianto.Obj_Disciplinare.gruppoFinalita.codice)) > -1) {

                                        disciplinare = d;

                                        break;

                                    }
                                }
                            }

                            //5° Caso: Se tra gli impianti selezionati c'è almeno un impianto con stesso Disciplinare codice, flag Protetto compatibile e Gruppo Finalità compatibile allora il disciplinare sarà quello
                            //Per alcune Specie Tipo la Vite la Finalità potrebbe non essere valorizzata nel disciplinare quindi considero solamente il codice del disciplinare e
                            //e il flag_protetto

                            if (!disciplinare) {

                                for (var d of Array_Disciplinari) {

                                    if (impianti_filtrati.findIndex(impianto => d.codice === impianto.Obj_Disciplinare.codice &&
                                        this.Controlla_Flag_Protetto_Impianti_con_Disciplinari(impianto.Obj_Disciplinare, d) &&
                                        this.Controlla_GruppoFinalita_Disciplinari(d, impianto.Obj_Disciplinare)) > -1) {

                                        disciplinare = d;

                                        break;

                                    }
                                }
                            }
                        }
                    }

                    //6° Caso: Se gli Impianti non hanno un disciplinare prendo quello da DpiPredefinitoUtente
                    if (!disciplinare) {

                        if (!this.Default_DPI_da_Impostazione) {
                            const LeggiDefault_DPI_QdC = <LeggiDefault_DPI_QdC>{
                                operazioni: this.TestataForm.get("Operazioni").value,
                                impianti: this.GetImpiantiSelezionatiModel(),
                                disciplinari: Array_Disciplinari,
                                tipoOperazioneDB: this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB,
                                data: this.TestataForm.get("Data").value,
                                impresa: this.getImpresa_Model(),
                                tipoRicetta: this.TestataForm.get("TipoRicetta").value,
                                tipoAttivita: this.TestataForm.get("Tipo").value,
                                codiciAttivita: this.TestataForm.get("Codici_Attivita").value
                            };

                            this.Default_DPI_da_Impostazione = this.setCodDescrDdlDisciplinare(await this.agendaservice.Default_DPI_QdC(LeggiDefault_DPI_QdC) as DropdownListDisciplinare);
                        }

                        disciplinare = this.Default_DPI_da_Impostazione;
                    }

                }

                //7° Caso: Se non ho trovato un Disciplinare allora gli imposto Solo Etichetta
                if (!disciplinare)
                    disciplinare = this.setCodDescrDdlDisciplinare(this.Obj_NessunDpi as DropdownListDisciplinare);

                disciplinare = this.setCodDescrDdlDisciplinare(disciplinare as DropdownListDisciplinare);

            } else {

                disciplinare = this.setCodDescrDdlDisciplinare(this.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);
            }


            resolve(disciplinare);
        });
    }

    private AggiungiClassedpiOn(row: KendoGridRow): boolean {

        let dpiOn: boolean = false;

        let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").value;

        if (Operazioni) {

            if (!this.AggiungiClasse_dichiarazione_non_utilizzo(row)) {

                if (Operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.RACCOLTA) > -1) {

                    let Data: Date = this.TestataForm.get("Data").value;

                    if (row['DataCarenza'] && (row['DataCarenza'] as Date).getTime() > Data.getTime()) {
                        dpiOn = true;
                    } else {
                        dpiOn = false;
                    }


                } else {
                    let o = Operazioni.find(o => +o.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO ||
                        +o.primaryKey.codice === enum_LAVCOD.DISERBO ||
                        +o.primaryKey.codice === enum_LAVCOD.GEODISINFESTAZIONE ||
                        +o.primaryKey.codice === enum_LAVCOD.CONCIA_SEME ||
                        +o.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO ||
                        +o.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE ||
                        +o.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA ||
                        +o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE ||
                        +o.primaryKey.codice === enum_LAVCOD.CONCIMAZIONE_FOGLIARE ||
                        +o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI ||
                        +o.primaryKey.codice === enum_LAVCOD.SARCHIATURA_CONCIMAZIONE ||
                        +o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_CONCIME ||
                        +o.primaryKey.codice === enum_LAVCOD.CONFUSIONE_SESSUALE ||
                        +o.primaryKey.codice === enum_LAVCOD.DISORIENTAMENTO_SESSUALE)

                    if (o) {
                        let Disciplinare = this.getDisciplinareModelValue(0);

                        if (Disciplinare) {
                            if (Disciplinare.codice === NessunDpi || Disciplinare.codice === NessunDpiNessunaEtichetta) {
                                //dpi
                                if (row['Disciplinare_Cod'] !== 0) {
                                    dpiOn = true;
                                } else {
                                    dpiOn = false;
                                }
                                //bio
                                if (row['Regolamento'] === enum_Cod_Regolamento.Regolamento_bio) {
                                    dpiOn = true;
                                }
                            } else {
                                if (Disciplinare.codice !== DpiBio && row['Regolamento'] === enum_Cod_Regolamento.Regolamento_bio) {
                                    dpiOn = true;
                                } else {
                                    dpiOn = false;
                                }
                            }
                        }
                    }
                }
            }

        }

        return dpiOn;
    }

    private AggiungiClasse_dichiarazione_non_utilizzo(row: KendoGridRow) {

        let dichiarazione_non_utilizzo: boolean = false;

        let Operazioni: Array<Lavorazione> = this.TestataForm?.get("Operazioni")?.getRawValue();

        if (Operazioni) {
            if (Operazioni.findIndex(o => this.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice)) > -1) {
                if (row['Dichiarazione_Non_Utilizzo_Trattamenti'] && row['Dichiarazione_Non_Utilizzo_Trattamenti'] !== "") {
                    dichiarazione_non_utilizzo = true;
                }
            }

            if (!dichiarazione_non_utilizzo) {
                if (Operazioni.findIndex(o => this.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) > -1) {
                    if ((row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] && row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] !== "")) {
                        dichiarazione_non_utilizzo = true;
                    }
                }
            }
        }

        return dichiarazione_non_utilizzo;
    }

    GestisciColoreRigheGridImpianti() {

        if (this.GridImpiantiElementRefRows && this.GridImpiantiPublicService.getValue() as BehaviorSubject<GridDataWithFilter>) {

            let rows_visibili: KendoGridRow[] = this.GridImpiantiPublicService.gridComp.data.data;

            rows_visibili.forEach((row: KendoGridRow, index: number) => {

                let ElementRefRow = this.GridImpiantiElementRefRows[index];

                if (ElementRefRow) {

                    if (this.AggiungiClasse_dichiarazione_non_utilizzo(row)) {
                        this.renderer.addClass(ElementRefRow, 'dichiarazione_non_utilizzo');
                    } else {
                        this.renderer.removeClass(ElementRefRow, 'dichiarazione_non_utilizzo');
                    }

                    if (this.AggiungiClassedpiOn(row)) {
                        this.renderer.addClass(ElementRefRow, 'dpiOn');
                    } else {
                        this.renderer.removeClass(ElementRefRow, 'dpiOn');
                    }
                }
            });

        }

    }

    AvvisoImpianti(rows: KendoGridRow[]) {

        this.Avviso_dichiarazione_non_utilizzo(rows);

        this.AvvisoImpianticonDpiRestrittivo(rows)

    }

    Avviso_dichiarazione_non_utilizzo(rows: KendoGridRow[]) {
        //se è stata selezionata una riga grigia parte l'avviso

        if (rows) {

            let listErroriGias = [];

            for (var row of rows) {
                if (this.AggiungiClasse_dichiarazione_non_utilizzo(row) && row['Selected'] && !this.MessaggioImpiantiVisualizzato.Dichiarazione_Non_Utilizzo) {
                    //this.MessaggioImpiantiVisualizzato.Dichiarazione_Non_Utilizzo = true;

                    let Operazioni: Array<Lavorazione> = this.TestataForm?.get("Operazioni")?.getRawValue();

                    let messaggio = "";

                    //Compongo il messaggio da far visualizzare all'utente
                    if (Operazioni.findIndex(o => this.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice)) > -1 &&
                        Operazioni.findIndex(o => this.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) > -1 &&
                        row['Dichiarazione_Non_Utilizzo_Trattamenti'] && row['Dichiarazione_Non_Utilizzo_Trattamenti'] !== "" &&
                        row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] && row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] !== "") {

                        messaggio = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzo");
                    } else {

                        if (Operazioni.findIndex(o => this.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice)) > -1 &&
                            row['Dichiarazione_Non_Utilizzo_Trattamenti'] && row['Dichiarazione_Non_Utilizzo_Trattamenti'] !== "") {

                            messaggio = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzoTrattamenti");
                        }

                        if (messaggio === "") {
                            if (Operazioni.findIndex(o => this.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) > -1 &&
                                row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] && row['Dichiarazione_Non_Utilizzo_Fertilizzazioni'] !== "") {

                                messaggio = this.translocoService.translate("qdc.AttenzioneImpiantiConDichiarazioniDiNonUtilizzoFertilizzazioni");
                            }
                        }

                    }


                    listErroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.WarningBloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: messaggio
                    });

                    this.gestisci_ErroriGias(listErroriGias, false, false, "", Dialog_Type.info, '45%', '30%').then();

                    break;
                }
            }
        }
    }

    AvvisoImpianticonDpiRestrittivo(rows: KendoGridRow[]) {
        //se è stata selezionata una riga arancione parte l'avviso

        if (rows) {

            let listErroriGias = [];

            for (var row of rows) {
                if (this.AggiungiClassedpiOn(row) && row['Selected'] && !this.MessaggioImpiantiVisualizzato.DPI) {
                    this.MessaggioImpiantiVisualizzato.DPI = true;

                    listErroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.WarningBloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("qdc.AttenzioneImpiantiPiuRestrittiviRispettoAlDPI")
                    });

                    this.gestisci_ErroriGias(listErroriGias, false, false, "", Dialog_Type.info, '45%', '30%').then();

                    break;
                }
            }
        }

    }

    mostraAcqua() {
        let mostra = false;

        let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").getRawValue();

        let index = -1;

        if (Operazioni) {
            index = Operazioni.findIndex(o => {
                let lav_cod = +o.primaryKey.codice;

                if (this.Elenco_Operazioni_con_Acqua.includes(lav_cod))
                    return o;
            });
        }

        if (index > -1)
            mostra = true;


        return mostra;
    }

    //Spostato qui perchè mi serve saperlo nel servizio del calcolo miscele
    //Stesse condizioni della funzione mostraAcqua
    mostraDose_Hl(Operazione: Lavorazione) {
        let mostra = false;
        let lav_cod: number = +Operazione.primaryKey.codice;

        if (this.Elenco_Operazioni_con_Acqua.includes(lav_cod))
            mostra = true;

        return mostra;
    }

    getOpzione_Semina_Model(Operazione: Lavorazione): BaseCodeDescr {

        //Restituisce l'opzione semina

        let Opzione_Semina: BaseCodeDescr = null;

        if (Operazione && this.Elenco_Operazioni_Sementi.includes(+Operazione.primaryKey.codice)) {
            let index: number = this.Sezioni_ProdottoFormArray.getRawValue().findIndex(p => (p.Operazione as Lavorazione).primaryKey.codice === Operazione.primaryKey.codice);

            if (index > -1) {
                let Sezione_Semina: FormGroup = this.Sezioni_ProdottoFormArray.controls[index] as FormGroup;

                Opzione_Semina = Sezione_Semina.get("Opzioni_Semina").value;
            }

        }

        return Opzione_Semina;

    }

    Controlla_Se_Possibile_Frazionamento_Semina(): boolean {

        let fraziona = true;

        let Impianti_Selezionati = [];

        let Operazione_con_Sementi: Lavorazione = null;

        //Prendo gli impianti selezionati dal grid public service non dal FormArray perchè
        //questa funzione viene richiamata prima di popolare il form array
        if (this.GridImpiantiPublicService &&
            this.GridImpiantiPublicService.getValue() &&
            this.GridImpiantiPublicService.getValue().data.rows.length > 0) {

            Impianti_Selezionati = this.GridImpiantiPublicService.getValue().data.rows.filter(r => r.Selected === true);

        }

        let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").getRawValue();

        if (Operazioni && Operazioni.length > 0)
            Operazione_con_Sementi = Operazioni.find(o => this.Elenco_Operazioni_Sementi.includes(+o.primaryKey.codice));

        if (Impianti_Selezionati.length > 1) {

            if (Operazione_con_Sementi) {

                let Opzione_Semina = this.getOpzione_Semina_Model(Operazione_con_Sementi);

                //Se Opzione_Semina non è valorizzato può darsi che sia perchè non c'è ancora la Sezione apposita e
                //l'utente ha cliccato su seleziona tutte le colonne in questo caso perndo il valore dall'impostazione utente
                if (!Opzione_Semina &&
                    (!this.Sezioni_ProdottoFormArray.getRawValue() ||
                        this.Sezioni_ProdottoFormArray.getRawValue().length === 0)) {

                    Opzione_Semina = this.get_Default_Opzione_Semina([Operazione_con_Sementi]);

                }

                if (Opzione_Semina &&
                    Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {

                    let listErroriGias = [];

                    listErroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.WarningBloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate('qdc.AttenzioneOpzioneFrazionamentoImpiantiSelezionati')
                    });

                    this.gestisci_ErroriGias(listErroriGias, false, false).then();

                    fraziona = false;


                }
            }

        }

        return fraziona;
    }

    //Se la DataCarenza della Raccolta è maggiore della data Operazione che ho dò errore
    Controlla_DataCarenza(): Promise<Array<any>> {

        return new Promise<Array<any>>(async (resolve, reject) => {

            let Impianti_Non_Conformi_Raccolta: Array<any> = [];

            let impianti_selezionati: Array<any> = [];

            let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").getRawValue();

            if (Operazioni && Operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.RACCOLTA) > -1) {

                impianti_selezionati = (this.GridImpiantiPublicService.getValue().data.rows as Array<any>).filter(r => r.Selected === true);

                if (impianti_selezionati && impianti_selezionati.length > 0) {

                    let Data: Date = this.TestataForm.get("Data").value;

                    impianti_selezionati.forEach(i => {
                        if (i.DataCarenza && i.DataCarenza.getTime() > Data.getTime()) {
                            Impianti_Non_Conformi_Raccolta.push(i);
                        }
                    });


                    if (Impianti_Non_Conformi_Raccolta.length > 0) {
                        let Msg_Impianti_Non_Conformi_Raccolta: string = Impianti_Non_Conformi_Raccolta
                            .map(i => ' - ' + i.APP_NOME + ' - ' + i.CarenzaStr.split(' - ')[1])
                            .join("\n");
                        let nextDate = impianti_selezionati
                            .sort((a, b) => b.DataCarenza.getTime() - a.DataCarenza.getTime())
                            .at(0).DataCarenza;
                        let nextDateStr = this.translocoService.translate("PrimaDataUtileRaccolta") + ': ' + nextDate.toLocaleString();


                        let listErroriGias = [{
                            severity: this.Utente_Cod_Blocca_Raccolta_Carenza_non_Rispettata ? ErroreGias_Severity.Bloccante : ErroreGias_Severity.Warning,
                            messaggio: this.translocoService.translate('qdc.DataRaccoltaNonConforme') + "<br>" + this.translocoService.translate("qdc.ImpiantiSelezionatiNonConformiAllaData", { ImpiantiList: Msg_Impianti_Non_Conformi_Raccolta }) + " " + nextDateStr
                        } as ErroreGias];


                        let Obj = await this.gestisci_ErroriGias(listErroriGias, true, true);

                        if (!this.Utente_Cod_Blocca_Raccolta_Carenza_non_Rispettata && Obj.result.returnObj)
                            Impianti_Non_Conformi_Raccolta = [];


                    }

                }
            }

            if (Impianti_Non_Conformi_Raccolta && Impianti_Non_Conformi_Raccolta.length > 0) {

                this.GridImpiantiPublicService.getValue().data.rows.forEach(r => {

                    if (Impianti_Non_Conformi_Raccolta.findIndex(row_non_conforme => {
                        return r['PIVA'] === row_non_conforme.PIVA &&
                            r['SA_COD'] === row_non_conforme.SA_COD &&
                            r['APPEZZA'] === row_non_conforme.APPEZZA &&
                            r['ID_REG'] === row_non_conforme.ID_REG &&
                            r['Progetto_Cod'] === row_non_conforme.Progetto_Cod
                    }) > -1) {

                        r['Selected'] = false;

                    }
                });

                //Se tutti le righe non sono conformi deseleziono il checkbox nell'header column
                if (Impianti_Non_Conformi_Raccolta.length === impianti_selezionati.length)
                    this.GridImpiantiPublicService.giasGridComponent.selectAllState = "unchecked";

            }

            resolve(Impianti_Non_Conformi_Raccolta);

        });

    }

    /**
     * @description
     * Richiamato sia nel butta giù che nel salvataggio generale del QdC
     */
    public Controllo_Sup_Calcolata(Opzione_Semina: BaseCodeDescr, RigaDaInserire: any, RigheGiaInserite: Array<any>, listerroriGias: ErroreGias[]) {

        //let Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(RigaDaInserire.Operazione);

        if (Opzione_Semina && Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {

            let sup_calcolata_dosi = 0;

            if (RigheGiaInserite && RigheGiaInserite.length > 0) {
                RigheGiaInserite.forEach(r => {
                    if (RigaDaInserire) {
                        if (r.DosiProdottiGridrowId !== RigaDaInserire.DosiProdottiGridrowId) {
                            sup_calcolata_dosi += r.Sup_Calcolata;
                        }
                    } else {
                        sup_calcolata_dosi += r.Sup_Calcolata;
                    }
                });
            }

            if (RigaDaInserire) {
                sup_calcolata_dosi = sup_calcolata_dosi + RigaDaInserire.Sup_Calcolata;
            }

            let sup_trattata = 0;

            if (this.SuperficiForm.get("Sup_Trattata").value) {
                sup_trattata = this.SuperficiForm.get("Sup_Trattata").value;
            }

            if (sup_calcolata_dosi > sup_trattata) {
                listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.Bloccante,
                    tipo: enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate('qdc.AttenzioneSupCalcolataMaggioredellaIndicata')
                });
            }

        }
    }

    gestisci_ErroriGias(listErroriGias: ErroreGias[], cerca_errori_bloccanti: boolean, mostra_errori_bloccanti: boolean, key_msg_conferma: string = "Sidesideraproseguire", dialog_type: number = null,
                        maxHeight:string = "auto",maxWidth:string = "auto", complianceResultStatus: ComplianceResultStatus = undefined): Promise<Obj_Errore_Gias_QdC> {

        //Mi potrebbero arrivare nella listadierrorigias tipi diversi di messaggio bloccanti/warningbloccanti/warning.
        //se mi arrivano i bloccanti mostro quelli e nascondo i warning bloccanti e warning
        //se mi arrivano i warning bloccanti mostro quelli e nascondo i warning

        return new Promise<Obj_Errore_Gias_QdC>((resolve, reject) => {

            let Ok_Action: Array<GiasDialogAction> = [
                { text: this.translocoService.translate('Ok'), returnObj: true }
            ];

            let content: string = "";

            if (cerca_errori_bloccanti) {
                let list_errori_bloccanti: ErroreGias[] = listErroriGias.filter(e => e.severity === ErroreGias_Severity.Bloccante);

                if (mostra_errori_bloccanti && list_errori_bloccanti && list_errori_bloccanti.length > 0) {

                    content = this.componi_messaggio(list_errori_bloccanti, ErroreGias_Severity.Bloccante, key_msg_conferma);

                    if (!dialog_type || dialog_type === 0) {
                        dialog_type = Dialog_Type.error;
                    }

                    this.Subs.add(this.giasdialogservice.dialogMessageObs_Result('', content, Ok_Action, "auto",
                        "auto", null, dialog_type).subscribe((result: any) => {

                            resolve({
                                severity: ErroreGias_Severity.Bloccante,
                                result: null,
                                list_errori_filtrati: list_errori_bloccanti
                            } as Obj_Errore_Gias_QdC);

                            return;
                        }));
                }
            }

            let list_warning_bloccanti: ErroreGias[] = listErroriGias.filter(e => e.severity === ErroreGias_Severity.WarningBloccante);

            if (list_warning_bloccanti && list_warning_bloccanti.length > 0) {

                let errorAction: Array<GiasDialogAction>;

                if (complianceResultStatus && complianceResultStatus.Timeout){ //se la richiesta è andata in timeout diamo all'utente la possibilità di continuare ad aspettare
                    errorAction = [
                        {text: this.translocoService.translate('Ok'), returnObj: true},
                        {text: this.translocoService.translate('ContinuaLAttesa'), returnObj: 'continue'}
                    ];
                } else {
                    errorAction = [
                        {text: this.translocoService.translate('Ok'), returnObj: true}
                    ];
                }
                content = this.componi_messaggio(list_warning_bloccanti, ErroreGias_Severity.WarningBloccante, key_msg_conferma);

                if (!dialog_type || dialog_type === 0) {
                    dialog_type = Dialog_Type.error;
                }

                this.Subs.add(this.giasdialogservice.dialogMessageObs_Result('', content, errorAction, "auto",
                    "auto", null, dialog_type, maxHeight, maxWidth).subscribe((result: any) => {

                        resolve({
                            severity: ErroreGias_Severity.WarningBloccante,
                            result: result,
                            list_errori_filtrati: list_warning_bloccanti
                        } as Obj_Errore_Gias_QdC);

                        return;
                    }));

            } else {

                let list_warning: ErroreGias[] = listErroriGias.filter(e => e.severity === ErroreGias_Severity.Warning);

                if (list_warning && list_warning.length > 0) {

                    content = this.componi_messaggio(list_warning, ErroreGias_Severity.Warning, key_msg_conferma);

                    let Warning_Action: Array<GiasDialogAction>;

                    if (complianceResultStatus && complianceResultStatus.Timeout){
                        Warning_Action = [
                            { text: this.translocoService.translate('No'), returnObj: false },
                            { text: this.translocoService.translate('ContinuaLAttesa'), returnObj: 'continue' },
                            { text: this.translocoService.translate('Si'), primary: true, returnObj: true },
                        ];
                    } else {
                        Warning_Action = [
                            { text: this.translocoService.translate('No'), returnObj: false },
                            { text: this.translocoService.translate('Si'), primary: true, returnObj: true },
                        ];
                    }

                    if (!dialog_type || dialog_type === 0) {
                        dialog_type = Dialog_Type.warning;
                    }


                    this.Subs.add(this.giasdialogservice.dialogMessageObs_Result('', content, Warning_Action, "auto",
                        "auto", null, dialog_type).subscribe((result: any) => {

                            resolve({
                                severity: ErroreGias_Severity.Warning,
                                result: result,
                                list_errori_filtrati: list_warning
                            } as Obj_Errore_Gias_QdC);

                            return;
                        }));
                }

            }
        });


    }

    componi_messaggio(list_ErroreGias: ErroreGias[], tipo_errore: ErroreGias_Severity, key_msg_conferma: string): string {

        //Concateno il nuovo messaggio solo se non è già presente
        //(potrebbe capitare che mi arriva due volte lo stesso messaggio)

        let messaggio: string = "";

        list_ErroreGias.forEach(e => {
            if (!messaggio.includes(e.messaggio))
                messaggio += e.messaggio + "<br>";
        });

        if (tipo_errore === ErroreGias_Severity.Warning && key_msg_conferma !== "")
            messaggio += this.translocoService.translate(key_msg_conferma);

        return messaggio;
    }

    getUtilizzoTerrenoModel(): UtilizzoTerreno {

        let utilizzoTerrenoClassType = this.TestataForm.get("Specie").value?.classType;

        let utilizzoTerreno = null;

        if (utilizzoTerrenoClassType === "DestinazioneUso") {
            utilizzoTerreno = new DestinazioneUso();
            utilizzoTerreno.codice = this.TestataForm.get("Specie").value.codice;
            utilizzoTerreno.descrizione = this.TestataForm.get("Specie").value.descrizione;
        } else {
            utilizzoTerreno = new Varieta();
            (<Varieta>utilizzoTerreno).specie = new Specie(this.TestataForm.get("Specie").value?.codice ?? 0);
            (<Varieta>utilizzoTerreno).specie.descrizione = this.TestataForm.get("Specie").value?.descrizione ?? '';
            utilizzoTerreno.codice = 0;
            utilizzoTerreno.descrizione = "";
        }

        return utilizzoTerreno;
    }

    //Se la grid è dentro un 'expnasion panel ed ha molte colonne tutte in autofit column quando l'utente apre
    //l'expansion panel la grid non vinene visualizzata corretamente qiuindi devo rifare la funzione di refresh
    OnActionExpansionPanel(event: ExpansionPanelActionEvent, name: string) {

        if (event.action === "expand") {

            switch (name) {
                case 'Macchine_Operatori':
                    this.GridMacchinePublicService?.refresh(true);
                    this.GridOperatoriPublicService?.refresh(true);
                    break;
                case 'Impianti':
                    this.GridImpiantiPublicService?.refresh(true);
                    break;
                case 'Note':
                    this.GridNotePublicService?.refresh(true);
                    break;
            }

        }

    }

    Controlla_Se_Impostare_VolumiAcqua(AcquaMax: number, descrizione: string, flag_from_taratura_uggello: boolean, flag_from_dosi: boolean): Promise<boolean> {

        return new Promise<boolean>(async (resolve, reject) => {

            let imposta_volumi_acqua = true;

            //Il messaggi di conferma sì/no deve venire fuori solamnete se prima l' acqua era stata impostata da dose etichetta e ora verrebbe sovrascritta dalla
            //taratura ugello oppure se prima l' acqua era stata impostata da taratura ugello e ora verrebbe sovrascritta dalla dose etichetta

            if (this.obj_Acqua_Provenienza) {

                let listErroriGias = [];

                let Msg = "";

                if (flag_from_taratura_uggello && !flag_from_dosi && this.obj_Acqua_Provenienza.flag_from_dosi)
                    Msg = this.translocoService.translate("qdc.AcquaAtomizzatoreSovrascrive", {
                        Desc: descrizione
                    });

                if (flag_from_dosi && !flag_from_taratura_uggello && this.obj_Acqua_Provenienza.flag_from_taratura_uggello)
                    Msg = this.translocoService.translate("qdc.AcquaDoseEtichettaSovrascrive", {
                        Desc: descrizione
                    });

                if (Msg !== "") {
                    listErroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Warning,
                        messaggio: Msg,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        ex: ""
                    });

                    let res: Obj_Errore_Gias_QdC = await this.gestisci_ErroriGias(listErroriGias, false, false, "SidesideraproseguireAggiornandola");

                    if (res.severity === ErroreGias_Severity.Warning) {
                        if (res.result.returnObj === false) {
                            imposta_volumi_acqua = false;
                        }
                    }
                }


            }


            resolve(imposta_volumi_acqua);
        });


    }

    //replica Imposta_VolumiAcqua nella Trattamenti_2
    Imposta_VolumiAcqua(AcquaMax: number, AcquaUdm: UnitaDiMisura, Dose_Desc: string): Promise<boolean> {

        return new Promise<boolean>(async (resolve, reject) => {

            let Operazioni_con_Acqua = this.mostraAcqua();

            if (!Operazioni_con_Acqua) {
                resolve(true);

                return;
            }

            //se ho già inserito un prodotto in una multi operazione non modifico la dose acqua
            if (this.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()) {
                if (this.Sezioni_ProdottoFormArray.getRawValue().findIndex(s => s.DosiProdotti.length > 0) > -1) {

                    resolve(true);

                    return;
                }
            } else {

                if (this.CheckDosiProdottiSalvate()) {
                    resolve(true);

                    return;
                }
            }

            let TaraturaUgello_Obj = this.Get_TaraturaUgello();

            let AcquaTaraturaHL = TaraturaUgello_Obj.Taratura_Ugello;

            let Acqua_Ha = (this.AcquaForm.getRawValue() as Acqua).Acqua_Ha;

            let Sup_Trattata = this.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare
                ? (this.SuperficiForm.getRawValue() as Superfici).Sup_Trattata
                : this.QuantitaForm.getRawValue().Qta_Trattata;

            if (AcquaMax !== 0) {
                let AcquaMaxHL = 0;

                let obj = this.unitadimisuraservice.ScomponiUdm(AcquaUdm);

                if (obj.UDM_radice !== -1) {

                    if (obj.perHa_hl === enum_UnitaMisura.Ettaro) {

                        let sezioni_prodotto_con_acqua = this.Sezioni_ProdottoFormArray.getRawValue().filter(s => this.Elenco_Operazioni_con_Acqua.includes(+s.Operazione.primaryKey.codice));

                        sezioni_prodotto_con_acqua.forEach(s => {
                            const index = this.Sezioni_ProdottoFormArray.getRawValue().findIndex(sezione => +sezione.Operazione.primaryKey.codice === +s.Operazione.primaryKey.codice);

                            this.Sezioni_ProdottoFormArray.controls[index].patchValue({
                                flagDoseQuantitaTotale: enum_doseQuantitaTotale.Dose
                            });
                        });

                        if (AcquaMax !== 0) {
                            AcquaMaxHL = this.unitadimisuraservice.Converti(new UnitaDiMisura(obj.UDM_radice, "", ""),
                                AcquaMax, new UnitaDiMisura(enum_UnitaMisura.Ettolitro, "", ""));
                        }

                        if ((AcquaTaraturaHL > 0 && AcquaTaraturaHL > AcquaMaxHL) || AcquaTaraturaHL === 0) {

                            //controllo se l'acqua non è già stata inserita

                            if (Acqua_Ha > 0) {

                                let AcquaMaxHL_arrotondata = this.funzionicomuniservice.roundNumber(AcquaMaxHL, this.get4DecimalNumericSettings().decimals);

                                let imposta_volumi = await this.Controlla_Se_Impostare_VolumiAcqua(AcquaMaxHL_arrotondata, Dose_Desc, false, true);

                                if (imposta_volumi) {

                                    this.obj_Acqua_Provenienza.Descrizione = this.translocoService.translate('DaDosiDiEtichetta');

                                    this.obj_Acqua_Provenienza.flag_from_taratura_uggello = false;

                                    this.obj_Acqua_Provenienza.flag_from_dosi = true;

                                    this.AcquaForm.patchValue({
                                        Acqua_Ha: AcquaMaxHL_arrotondata
                                    });

                                    if (Sup_Trattata > 0) {
                                        this.AcquaForm.patchValue({
                                            Acqua_Tot: this.funzionicomuniservice.roundNumber(AcquaMaxHL * Sup_Trattata, this.get4DecimalNumericSettings().decimals)
                                        });
                                    }
                                }
                            } else {

                                this.obj_Acqua_Provenienza.Descrizione = this.translocoService.translate('DaVincoliEtichetta');

                                this.obj_Acqua_Provenienza.flag_from_taratura_uggello = false;

                                this.obj_Acqua_Provenienza.flag_from_dosi = true;

                                this.AcquaForm.patchValue({
                                    Acqua_Ha: this.funzionicomuniservice.roundNumber(AcquaMaxHL, this.get4DecimalNumericSettings().decimals)
                                });

                                if (Sup_Trattata > 0) {
                                    this.AcquaForm.patchValue({
                                        Acqua_Tot: this.funzionicomuniservice.roundNumber(AcquaMaxHL * Sup_Trattata, this.get4DecimalNumericSettings().decimals)
                                    });
                                }

                            }

                        }

                    }

                }
            } else if (AcquaTaraturaHL > 0) {

                let imposta_volumi = true;

                let AcquaTaraturaHL_arrotondata = this.funzionicomuniservice.roundNumber(AcquaTaraturaHL, this.get4DecimalNumericSettings().decimals);

                if (Acqua_Ha > 0) {
                    imposta_volumi = await this.Controlla_Se_Impostare_VolumiAcqua(AcquaTaraturaHL_arrotondata, TaraturaUgello_Obj.Descrizione, true, false);
                }

                if (imposta_volumi) {

                    this.obj_Acqua_Provenienza.flag_from_taratura_uggello = true;

                    this.obj_Acqua_Provenienza.flag_from_dosi = false;

                    this.obj_Acqua_Provenienza.Descrizione = this.translocoService.translate('DaTaraturaUgelli');

                    this.AcquaForm.patchValue({
                        Acqua_Ha: AcquaTaraturaHL_arrotondata
                    });

                    if (Sup_Trattata > 0) {
                        this.AcquaForm.patchValue({
                            Acqua_Tot: this.funzionicomuniservice.roundNumber(AcquaTaraturaHL * Sup_Trattata, this.get4DecimalNumericSettings().decimals)
                        });
                    }
                }


            }

            resolve(true);

        });
    }

    //Replica Get_TaraturaUgello della Trattamenti_2
    Get_TaraturaUgello(): any {

        //Prendo la prima macchina con taratura ugello > 0 dato che tecnicamente non ci dovrebbero essere due macchine scelte con la taratura ugello

        let TaraturaUgello_Obj = {
            Taratura_Ugello: 0,
            Descrizione: ""
        };

        if (this.MacchineFormArray &&
            this.MacchineFormArray.getRawValue().length > 0) {

            let gridmacchinemodel: GridMacchinaModel[] = this.MacchineFormArray.getRawValue();

            for (let m of gridmacchinemodel) {
                if (m.Taratura_Ugello > 0) {
                    TaraturaUgello_Obj.Taratura_Ugello = m.Taratura_Ugello;
                    TaraturaUgello_Obj.Descrizione = m.Risorsa_Des;
                    break;
                }
            }

        }

        return TaraturaUgello_Obj;
    }

    //Se ci sono dei Poligoni del GIS apro il pannello in automatico
    TrovaPoligoniGIS() {
        let TestataValue = (this.TestataForm.getRawValue() as Testata);
        this.gisService.setFilterServiceQdcConPoligoni(false);
        if ((TestataValue && TestataValue.Specie && TestataValue.Specie.codice > -1) || this.TestataForm.get("flagVisita").value) {
            const payload: VerificaEsistenzaEntitaPerImpianti_In = {
                Piva: TestataValue.Centro_Aziendale?.primaryKey?.partitaIva,
                SaCod: TestataValue.Centro_Aziendale?.primaryKey?.codice,
                CampoCod: TestataValue.Campo?.primaryKey?.codice,
                ValiditaInizio: TestataValue.Data,
                ValiditaFine: TestataValue.Data,
                UtilizzoTerreno: this.getUtilizzoTerrenoModel()
            };

            this.Subs.add(this.gisClient.gisVerificaEsistenzaEntitaPerImpianti(payload).subscribe(resp => {

                let pannelloGisPoligoniVisite: boolean = true;

                if (resp.RispostaOK && resp.RispostaStringa && resp.RispostaStringa.NumeroEntita > 0) {
                    this.GISPanel.Polygon = true;
                    this.gisService.setFilterServiceQdcConPoligoni(true);
                } else {
                    const latLngCentroAziendale = this.GetLatLngCentroAziendale();
                    if (latLngCentroAziendale.isValid) {
                        this.GISPanel.Polygon = true;
                        let objCentraMappa = new CentraMappa();
                        objCentraMappa.punto = { ...latLngCentroAziendale };
                        this.gisService.setFilterServiceCentraMappa(objCentraMappa);
                    } else {

                        //Per i rilievi anche se non ci sono dei Poligoni e non ci sono dei centri aziendali con latitudine e longitudine mostro comunque la
                        //mappa del GIS
                        if (TestataValue.Operazioni.findIndex(o => this.Elenco_Operazioni_Rilievi.includes(+ o.primaryKey.codice)) > -1
                            || this.TestataForm.get("flagVisita").value) {       //se arrivo dal menu Visite, mostro il pannello
                            this.GISPanel.Polygon = true;
                            if (this.TestataForm.get("flagVisita").value)
                                pannelloGisPoligoniVisite = false;
                        } else {
                            this.GISPanel.Polygon = false;
                        }
                    }
                }

                //Se c'è una Specie selezionata leggo dai cookie se aprire o chiudere il Panel del GIS
                if (this.GISPanel.Polygon) {
                    if (!this.TestataForm.get("flagVisita").value) //se non arrivo dalle visite, prendo dai cookie
                        this.GISPanel.ExpandPanel = this.getDefaultGISPanelFromCookie();
                    else
                        this.GISPanel.ExpandPanel = pannelloGisPoligoniVisite;
                } else {
                    this.GISPanel.ExpandPanel = false;
                }
            }));
        } else {
            this.GISPanel.Polygon = false;
            this.GISPanel.ExpandPanel = false;
        }

    }

    private GetLatLngCentroAziendale(): LatLng {
        let objLatLng = new LatLng();
        let CentroAziendale: CentroAziendale = this.TestataForm.get("Centro_Aziendale").value;
        // In caso di tutti i centri aziendali, viene cercato il primo in ordine ascendente
        // di codice con lat/lng valorizzato
        if (CentroAziendale) {
            if (CentroAziendale.primaryKey.codice === 0) {
                const primoCentroLatLng = this.elencoCentriAziendali.filter((centro) => {
                    return centro.lat !== undefined && centro.lat !== 0 &&
                        centro.lng !== undefined && centro.lng !== 0;
                })
                    .sort((primoCentro, secondoCentro) => primoCentro.primaryKey.codice - secondoCentro.primaryKey.codice);
                if (primoCentroLatLng?.length > 0) {
                    CentroAziendale = primoCentroLatLng[0];
                }
            }
            // Se sono presenti le coordinate, aggiorno lat/lng
            if (CentroAziendale.lat !== undefined && CentroAziendale.lng !== undefined) {
                objLatLng.lat = CentroAziendale.lat;
                objLatLng.lng = CentroAziendale.lng;
                if (objLatLng.lat !== 0 || objLatLng.lng !== 0) {
                    objLatLng.isValid = true;
                }
            }
        }

        return objLatLng;
    }

    EsisteProdottoPolverulento(): boolean {

        //Controlla se esiste un Prodotto Polverulento

        let polverulento = false;

        if (this.Sezioni_ProdottoFormArray && this.Sezioni_ProdottoFormArray.getRawValue().length > 0) {
            let Sezioni_con_Formulati: Array<Sezione_Prodotto_Formulati> = this.Sezioni_ProdottoFormArray.getRawValue().filter((s: Sezione_Prodotto_Formulati) => s.Categoria_Magazzino === FORMULATI);

            if (Sezioni_con_Formulati && Sezioni_con_Formulati.length > 0) {
                let Sezioni_Con_Polverulento = Sezioni_con_Formulati.filter(s => s.DosiProdotti.filter((d: any) => d.Polverulento === Tipo_Polverulento.Polverulento).length > 0);

                if (Sezioni_Con_Polverulento && Sezioni_Con_Polverulento.length > 0) {
                    polverulento = true;
                }
            }
        }

        return polverulento;

    }

    getDataPrimaRaccoltaUtile(carenza: number, data_operazione: Date): string {

        //Imposta la Prima Data di Raccolta Utile (quindi sarebbero i giorni di carenza + 1)
        //Preso dalla Trattamenti_2.ImgBtn_DoseInserisci_Click

        let Prima_Data: string = "";

        let data: Date = new Date(data_operazione);


        //Nella Trattamenti_2 lo fa solo dopo che hai salvato l'operazione e ci rientri in modifica, io invece lo faccio
        //anche nel butta giù
        data.setDate(data.getDate() + 1);

        if (carenza > 0)
            data.setDate(data.getDate() + carenza);

        Prima_Data = this.datepipe.transform(data, 'dd/MM/yyyy');

        return Prima_Data;

    }

    public Abilitato_Inserimento_Prodotti_da_Grid_Dosi() {
        let flag = false;

        if (this.Modalita_Inserimento_Prodotti.getValue() === enum_Modalita_Inserimento_Prodotti.In_Griglia)
            flag = true;

        return flag;
    }

    public Mostra_Btn_Gestione_Prodotto() {

        let mostra = false;

        if (!this.Abilitato_Inserimento_Prodotti_da_Grid_Dosi())
            mostra = true;

        return mostra;

    }

    MostraDisciplinare(l: Lavorazione, Sezione_ProdottiFormArray: FormArray) {

        let mostra = this.Operazioni_Con_Disciplinare(l);


        //Se sono solo in una DISTRIBUZIONE_AMMENDANTI con Utilizza_Direttiva_Nitrati a true allora nascondo la ddl del Disciplinare
        if (+ l.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) {

            let Sezioni_Prodotto: Array<Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta> = Sezione_ProdottiFormArray?.getRawValue();

            if (Sezioni_Prodotto && Sezioni_Prodotto.length === 1) {
                let utilizza_direttiva_nitrati = Sezioni_Prodotto.findIndex(
                    (s: Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta) => {

                        if (s.Operazione.primaryKey.codice === l.primaryKey.codice) {

                            s = s as Sezione_Prodotto_Fertilizzanti;

                            if (s.Utilizza_Direttiva_Nitrati)
                                return s;

                        }

                    }
                );

                if (utilizza_direttiva_nitrati > -1)
                    mostra = false;
            }


        }



        return mostra;
    }

    Operazioni_Con_Disciplinare(l: Lavorazione): boolean {

        let dpi = false;

        switch (+ l.primaryKey.codice) {
            case enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO:
            case enum_LAVCOD.DISERBO:
            // case enum_LAVCOD.CONCIA_SEME:
            case enum_LAVCOD.DISSECCAMENTO:
            case enum_LAVCOD.GEODISINFESTAZIONE:
            case enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE:
            case enum_LAVCOD.DISTRIBUZIONE_CONCIME:
            case enum_LAVCOD.SARCHIATURA_CONCIMAZIONE:
            case enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI:
            case enum_LAVCOD.CONCIMAZIONE_FOGLIARE:
            case enum_LAVCOD.FERTIRRIGAZIONE:
            case enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA:
            case enum_LAVCOD.CONFUSIONE_SESSUALE:
            case enum_LAVCOD.DISORIENTAMENTO_SESSUALE:
            case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
                // case enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA:
                dpi = true;
                break;
            default:
                break;
        }

        return dpi;
    }

    AggiornaFormArrayGridDosiProdotti(dataItem: any, index: number, Operazione: Lavorazione, tipo_operazione_db: enum_TipoOperazioneDB, DosiProdottiFormArray: FormArray, emitEvent: boolean = true) {

        if (Operazione) {

            let dosiProdotti: FormArray = DosiProdottiFormArray;

            if (!dosiProdotti)
                dosiProdotti = this.DosiProdottiFormArray(null, Operazione);

            let validator: ValidatorFn[] = [];

            if (tipo_operazione_db === enum_TipoOperazioneDB.Scrittura) {

                const elem_cod = this.getCategoria_Magazzino(+ Operazione.primaryKey.codice);

                const gridDosiProdottiValidator = new GridDosiProdottiValidator(elem_cod);

                validator.push(
                    gridDosiProdottiValidator.validate.bind(gridDosiProdottiValidator)
                );
            }

            UtilityFunctions.ManageFormArrayKendoGrid(dosiProdotti, dataItem, tipo_operazione_db, index, emitEvent, validator);

        }

    }

    public Mostra_Btn_AggiungiDoseProdotto() {
      let mostra = false;

      if(!this.Sola_Lettura_QdCForm() && !this.Is_Ribaltamento_Ricetta_Da_PUA()){
        mostra = true;
      }

      return mostra;
    }


    getDefaultSpecieFromCookie() {

        let Specie = null;

        let Specie_Str = this.cookieService.getCookie(this.getCookieKeySpecie());

        if (Specie_Str && Specie_Str !== "")
            Specie = <Specie | DestinazioneUso>JSON.parse(Specie_Str);

        return Specie;
    }

    getDefaultGISPanelFromCookie() {

        let Expand = true;

        let Expand_Str = this.cookieService.getCookie(this.getCookieKeyGISPanel());

        if (Expand_Str && Expand_Str !== "") {
            if (Expand_Str === "false") {
                Expand = false;
            }
        }

        return Expand;
    }

    getDefaultImpiantiPanelFromCookie() {

        let Expand = true;

        let Expand_Str = this.cookieService.getCookie(this.getCookieKeyImpiantiPanel());

        if (Expand_Str && Expand_Str !== "") {
            if (Expand_Str === "false") {
                Expand = false;
            }
        }

        return Expand;
    }

    /**
     * @description
     * Per le Operazioni di non utilizzo non è obbligatorio selezionare gli impianti
     */
    GestisciValidatorImpiantiSuperficie(Operazioni_Scelte: Array<Lavorazione>) {
        if (Operazioni_Scelte && Operazioni_Scelte.length > 0) {

            if ((Operazioni_Scelte.findIndex(o => this.Elenco_Operazioni_Non_Utilizzo.includes(+ o.primaryKey.codice)) > -1 &&
                Operazioni_Scelte.length === 1)
                ||                                                      //oppure arrivo dal menu Visite e seleziono Altre Lavorazioni, gli impianti non sono obbligatori
                (this.TestataForm.get("flagVisita").value)
                ||
                (this.RilievoSenzaImpianti())) {

                this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").clearValidators();

                this.QdCForm.get("Trattamento").get("Superfici").get("Sup_Trattata").clearValidators();

            } else {
                if (this.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare) {
                    const suptrattatavalidator = new SupTrattataValidator(this);

                    this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").setValidators([Validators.required]);
                    this.QdCForm.get("Trattamento").get("Superfici").get("Sup_Trattata").setValidators([Validators.required, Validators.min(0.0001), suptrattatavalidator.validate.bind(suptrattatavalidator)]);
                } else {
                    this.ProdottiDaTrattareSelezionatiFormArray.setValidators([Validators.required]);
                    this.QuantitaForm.get("Qta_Trattata").setValidators([Validators.required, Validators.min(0.0001)]);
                }
            }

            this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").updateValueAndValidity();

            this.QdCForm.get("Trattamento").get("Superfici").get("Sup_Trattata").updateValueAndValidity();

        } else {
            this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").clearValidators();

            this.QdCForm.get("Trattamento").get("Superfici").get("Sup_Trattata").clearValidators();

            this.QdCForm.get("Trattamento").get("Impianti").get("ImpiantiSelezionati").updateValueAndValidity();

            this.QdCForm.get("Trattamento").get("Superfici").get("Sup_Trattata").updateValueAndValidity();
        }
    }

    //Ottengo la percentuale della superficie da trattare cercando tra tutti i Prodotti inseriti per
    //il diserbo/disseccamento e per ogni prodotto prendo la minima superficie trattabile cercando tra i principi attivi
    // oppure la ottengo solo dal formulato selezionato
    OttieniPercentualeAbbattimentoDiserboDisseccamento(Formulato: MultiColumnComboboxTrattamento, Operazione: Lavorazione): number {

        let PercAbbMin: number = 0;

        if (this.obj_Inizializza_QdC.flagNuovoControlloRiduzioneDiserbo) {

            if (Formulato && Formulato.prodotto.codice > 0 && Operazione && (+ Operazione.primaryKey.codice === enum_LAVCOD.DISERBO || + Operazione.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO)) {

                if (Formulato.principiAttivi && Formulato.principiAttivi.length > 0) {
                    let percAbbMinxPa: PrincipioAttivo = Formulato.principiAttivi.reduce(function (prev, curr) {
                        return prev.percentualeSuperficieTrattabile < curr.percentualeSuperficieTrattabile ? prev : curr;
                    });

                    if (percAbbMinxPa && (PercAbbMin === 0 || (percAbbMinxPa.percentualeSuperficieTrattabile > 0 && percAbbMinxPa.percentualeSuperficieTrattabile < PercAbbMin))) {
                        PercAbbMin = percAbbMinxPa.percentualeSuperficieTrattabile;
                    }
                }

            } else {
                let Sezioni_Prodotto: Array<Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta> = this.Sezioni_ProdottoFormArray.getRawValue();

                if (Sezioni_Prodotto && Sezioni_Prodotto.length > 0) {
                    let Sezioni_Diserbo_Disseccamento = Sezioni_Prodotto.filter(s => +s.Operazione.primaryKey.codice === enum_LAVCOD.DISERBO ||
                        +s.Operazione.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO);

                    if (Sezioni_Diserbo_Disseccamento && Sezioni_Diserbo_Disseccamento.length > 0) {

                        Sezioni_Diserbo_Disseccamento.forEach(s => {
                            let DosiProdotti: Array<any> = s.DosiProdotti;

                            DosiProdotti.forEach(d => {
                                let dettaglioTrattamento: MultiColumnComboboxTrattamento = d.Prodotto;

                                if (dettaglioTrattamento && dettaglioTrattamento.prodotto.codice > 0 && dettaglioTrattamento.principiAttivi && dettaglioTrattamento.principiAttivi.length > 0) {
                                    let percAbbMinxPa: PrincipioAttivo = dettaglioTrattamento.principiAttivi.reduce(function (prev, curr) {
                                        return prev.percentualeSuperficieTrattabile < curr.percentualeSuperficieTrattabile ? prev : curr;
                                    });

                                    if (percAbbMinxPa && (PercAbbMin === 0 || (percAbbMinxPa.percentualeSuperficieTrattabile > 0 && percAbbMinxPa.percentualeSuperficieTrattabile < PercAbbMin))) {
                                        PercAbbMin = percAbbMinxPa.percentualeSuperficieTrattabile;
                                    }
                                }

                            });
                        });

                    }
                }
            }


        }

        return PercAbbMin;
    }

    ControllaSuperficieTrattabileXPercentualeAbbattimento(dataItem: any): ErroreGias[] {

        let messaggio = "";

        let listerroriGias: ErroreGias[] = [];

        let percAbb: number = this.OttieniPercentualeAbbattimentoDiserboDisseccamento(null, null);

        if (percAbb > 0 && percAbb < 100 && dataItem.Selected) {
            let supImpianto = dataItem.Sup_Imp;
            let supTrattata = dataItem.Sup_Imp_help;
            let supTrattabile = this.funzionicomuniservice.roundNumber(supImpianto * percAbb / 100 , this.get4DecimalNumericSettings().decimals);

            if (supTrattabile < supTrattata) {
                dataItem.Sup_Imp_help = supTrattabile;
                messaggio = this.translocoService.translate('qdc.AttenzioneMaxSuperficieProdotto', { PercAbb: percAbb });
            }
        }

        if (messaggio !== "") {

            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: messaggio
            });
        }

        return listerroriGias;

    }

    mostraTestataRicetta() {

        //Visibile per tutti i tipi di Ricette non per il brogliaccio

        let mostra: boolean = false;

        if (this.TestataForm && this.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta &&
            this.TestataForm.get("Stato").value === Stati.Da_Eseguire
        ) {
            mostra = true;
        }

        return mostra;
    }

    mostraTestataVisita() {
        return this.TestataForm.get("flagVisita").value;
    }

    MostraBtnSendDataToAPP(): boolean {
        return (
            this.mostraTestataRicetta() &&
            this.TestataForm.get('Operazioni').getRawValue().findIndex(op =>
              this.obj_Inizializza_QdC?.ListaLavCodNonGestitiSuAPP?.findIndex(lavCod => lavCod === + op.primaryKey.codice) >-1) === -1
        );
    }

    //@description:
    //Da richiamare quando si confrontano i due disciplinari della combo
    Controlla_Flag_Protetto_Disciplinari(disciplinare_scelto: DropdownListDisciplinare | Disciplinare, disciplinare_da_confrontare: DropdownListDisciplinare | Disciplinare): boolean {

        //Valori flag_Protetto DPI:
        //-1 - non dovrebbe succedere come caso ma viene trattato come pieno campo (no copertura)
        //0 - Non specificato (compatibile con tutti)
        //1 - Copertura
        //2 - Pieno Campo

        let compatibile = true;

        if ((disciplinare_scelto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === 0) || (disciplinare_scelto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === 1) || (disciplinare_scelto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === 2) || (disciplinare_scelto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === -1)) {
            //Copertura Compatibile
        } else if ((disciplinare_scelto.flagProtetto === 1 && disciplinare_da_confrontare.flagProtetto === 0) || (disciplinare_scelto.flagProtetto === 1 && disciplinare_da_confrontare.flagProtetto === 1)) {
            //Copertura Compatibile
        } else if ((disciplinare_scelto.flagProtetto === 2 && disciplinare_da_confrontare.flagProtetto === 0) || (disciplinare_scelto.flagProtetto === 2 && disciplinare_da_confrontare.flagProtetto === 2)) {
            //Copertura Compatibile
        } else if ((disciplinare_scelto.flagProtetto === -1 && disciplinare_da_confrontare.flagProtetto === 0) || (disciplinare_scelto.flagProtetto === -1 && disciplinare_da_confrontare.flagProtetto === -1)) {
            //Copertura Compatibile
        } else {
            compatibile = false;
        }

        return compatibile;

    }

    //@description:
    //Da richiamare quando si confrontano il disciplinare dell'impianto selezionato con il disciplinare della combo
    Controlla_Flag_Protetto_Impianti_con_Disciplinari(disciplinare_impianto: DropdownListDisciplinare | Disciplinare, disciplinare_da_confrontare: DropdownListDisciplinare | Disciplinare): boolean {

        //Valori flag_Protetto DPI:
        //-1 - non dovrebbe succedere come caso ma viene trattato come pieno campo (no copertura)
        //0 - Non specificato (compatibile con tutti)
        //1 - Copertura
        //2 - Pieno Campo

        //Il flag_protetto dell'impianto ha solamente come valore 0 e 1 (protetto/non protetto) a differenza del flag protetto del Disciplinare
        // che può avere come valori -1,0,1,2

        let compatibile = true;

        if ((disciplinare_impianto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === 0) || (disciplinare_impianto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === -1) || (disciplinare_impianto.flagProtetto === 0 && disciplinare_da_confrontare.flagProtetto === 2)) {
            //Copertura Compatibile
        } else if ((disciplinare_impianto.flagProtetto === 1 && disciplinare_da_confrontare.flagProtetto === 1) || (disciplinare_impianto.flagProtetto === 1 && disciplinare_da_confrontare.flagProtetto === 0)) {
            //Copertura Compatibile
        } else {
            compatibile = false;
        }

        return compatibile;

    }

    Controlla_GruppoFinalita_Disciplinari(disciplinare_scelto: DropdownListDisciplinare | Disciplinare, disciplinare_da_confrontare: DropdownListDisciplinare | Disciplinare) {
        let compatibile = true;

        if (disciplinare_scelto.gruppoFinalita?.codice === disciplinare_da_confrontare.gruppoFinalita?.codice || disciplinare_scelto.gruppoFinalita?.codice === 0) {
            ///Finalità compatibile
        } else {
            compatibile = false;
        }

        return compatibile;
    }

    IsMagazzinoAgenzia(magazzino: DropdownListMagazzino | Fabbricato) {

        let isMagazzinoAgenzia = false;

        if (magazzino) {
            if (this.obj_Inizializza_QdC && this.obj_Inizializza_QdC.ListaPivaAgenzie && this.obj_Inizializza_QdC.ListaPivaAgenzie.length > 0) {

                let piva = "";

                if (magazzino?.primaryKey?.centroAziendalePK?.partitaIva && magazzino?.primaryKey?.centroAziendalePK?.partitaIva !== "")
                    piva = magazzino?.primaryKey?.centroAziendalePK?.partitaIva;

                isMagazzinoAgenzia = this.obj_Inizializza_QdC.ListaPivaAgenzie.includes(piva);
            }
        }

        return isMagazzinoAgenzia;

    }

    IsMagazzinoEsterno(magazzino: DropdownListMagazzino | Fabbricato) {

        let isMagazzinoEsterno = false;

        if (magazzino) {

            if (magazzino.usoDaTerzi &&
                magazzino.primaryKey?.centroAziendalePK?.partitaIva !== this.objParametriAgendaService.getObjParamValue().Piva) {
                isMagazzinoEsterno = true;
            }

            if (!isMagazzinoEsterno && this.obj_Inizializza_QdC && this.obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi && this.obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi.length > 0) {

                if (magazzino.primaryKey) {
                    let index = this.obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi.findIndex(f => f.primaryKey.codice === magazzino.primaryKey.codice &&
                        f.primaryKey.centroAziendalePK.codice === magazzino.primaryKey.centroAziendalePK.codice &&
                        f.primaryKey.centroAziendalePK.partitaIva === magazzino.primaryKey.centroAziendalePK.partitaIva);
                    if (index > -1)
                        isMagazzinoEsterno = true;

                }
            }
        }

        return isMagazzinoEsterno;

    }

    public getMagazzino_Esterno_con_Lotto(rilevamentoMagazzino: RilevamentoDiMagazzino[]): { Magazzino_Esterno: Fabbricato, Lotto: string } {

        let Magazzino_Esterno_con_Lotto: { Magazzino_Esterno: Fabbricato, Lotto: string } = {
            Magazzino_Esterno: null,
            Lotto: ""
        };

        if (rilevamentoMagazzino &&
            rilevamentoMagazzino.length === 1) {

            if (this.IsMagazzinoEsterno(rilevamentoMagazzino[0].Magazzino)) {

                Magazzino_Esterno_con_Lotto.Magazzino_Esterno = rilevamentoMagazzino[0].Magazzino;

                Magazzino_Esterno_con_Lotto.Lotto = rilevamentoMagazzino[0].Lotto;
            }

            if (!Magazzino_Esterno_con_Lotto?.Magazzino_Esterno) {

                let Prodotto_Op_Campagna = rilevamentoMagazzino[0].Prodotto;

                if (rilevamentoMagazzino[0].registrazioniCollegate && rilevamentoMagazzino[0].registrazioniCollegate.length > 0) {
                    rilevamentoMagazzino[0].registrazioniCollegate.forEach(reg => {
                        if (reg && reg.risorse && reg.risorse.length > 0) {
                            let risorseRegistrazione = (reg.risorse.filter(risorsa => risorsa.classType === "RisorsaRegistrazione")) as RisorsaRegistrazione[];

                            if (risorseRegistrazione && risorseRegistrazione.length > 0) {
                                risorseRegistrazione.forEach(risorsaRegistrazione => {
                                    if (risorsaRegistrazione.MagazziniMovimentazioni && risorsaRegistrazione.MagazziniMovimentazioni.length > 0) {
                                        risorsaRegistrazione.MagazziniMovimentazioni.forEach(m => {

                                            if (this.IsMagazzinoEsterno(m.Magazzino) &&
                                                Prodotto_Op_Campagna.codice === m.Prodotto.codice &&
                                                Prodotto_Op_Campagna.elemCod === m.Prodotto.elemCod &&
                                                rilevamentoMagazzino[0].Lotto === m.Lotto) {

                                                Magazzino_Esterno_con_Lotto.Magazzino_Esterno = m.Magazzino;

                                                Magazzino_Esterno_con_Lotto.Lotto = m.Lotto;

                                                return Magazzino_Esterno_con_Lotto;
                                            }

                                        });
                                    }
                                });
                            }
                        }
                    })
                }
            }

        }

        return Magazzino_Esterno_con_Lotto;
    }

    //Se è stato scelto un prodotto da un Magazzino di un'agenzia (o da un Magazzino Esterno) controllo che poi venga scelto un Magazzino dell'azienda per scaricarlo
    public Controlla_Magazzino(RowGridDosi: any): boolean {

        let controllo_ok = true;

        if (RowGridDosi.Magazzino_Agenzia || RowGridDosi.Magazzino_Esterno) {
            if (!RowGridDosi.Magazzino_del_Prodotto_Selezionato || this.IsMagazzinoAgenzia(RowGridDosi.Magazzino_del_Prodotto_Selezionato) || this.IsMagazzinoEsterno(RowGridDosi.Magazzino_del_Prodotto_Selezionato)) {
                controllo_ok = false;
            }
        }

        return controllo_ok;
    }

    //@description: Restituisce lo Stato_Cod dei Centri dgeli Impianti Selezionati
    get_Stato_Cod_from_ImpiantiSelezionati() {

        let stato_cod: string = "";

        if (this.ImpiantiSelezionatiFormArray && this.ImpiantiSelezionatiFormArray.getRawValue().length > 0) {
            for (let imp of (this.ImpiantiSelezionatiFormArray.getRawValue()) as Array<GridImpiantoSelezionatoModel>) {
                if (imp.Stato_Cod && imp.Stato_Cod !== "") {

                    //Caso in cui ci sono dei centri in stati diversi restituisco stringa vuota perchè non
                    //so quale stato_cod utilizzare
                    if (stato_cod !== "" && imp.Stato_Cod !== "" && stato_cod !== imp.Stato_Cod) {
                        stato_cod = "";
                        break;
                    }

                    stato_cod = imp.Stato_Cod;
                }
            }
        }

        return stato_cod;

    }

    getImpresa_Model(): Impresa {
        let impresa = new Impresa();

        if (this.TestataForm.get("flagVisita").value) {
            if (this.TestataVisitaForm.get("Azienda_Visita").value) {
                impresa.partitaIva = this.TestataVisitaForm.get("Azienda_Visita").value.partitaIva;
                impresa.ragioneSociale = this.TestataVisitaForm.get("Azienda_Visita").value.ragioneSociale;
            } else {
                impresa.partitaIva = '0';
                impresa.ragioneSociale = '';
            }
        } else {
            const objParams = this.objParametriAgendaService.getObjParamValue();
            impresa.partitaIva = objParams.Piva;
            impresa.ragioneSociale = objParams.RagSoc;
        }

        return impresa;
    }

    getCentri_Aziendali_Model_from_ImpiantiSelezionati(): Array<CentroAziendale> {

        let centri: Array<CentroAziendale> = [];

        if (this.ImpiantiSelezionatiFormArray && this.ImpiantiSelezionatiFormArray.getRawValue().length > 0) {
            for (let imp of (this.ImpiantiSelezionatiFormArray.getRawValue()) as Array<GridImpiantoSelezionatoModel>) {
                if (centri.findIndex(c => c.primaryKey.codice === imp.SA_COD &&
                    c.primaryKey.partitaIva === imp.PIVA) === -1) {

                    centri.push(new CentroAziendale({
                        partitaIva: imp.PIVA,
                        codice: imp.SA_COD
                    }));

                }
            }
        }

        return centri;
    }

    private setDefaultSpecieFromCookie() {

        let Specie = this.TestataForm.get("Specie").value;

        if (Specie) {

            let key: string = this.getCookieKeySpecie();

            this.cookieService.deleteCookie(key);

            let Specie_Str = "";

            Specie_Str = JSON.stringify(Specie);

            this.cookieService.setCookie({
                name: key,
                value: Specie_Str
            });
        }

    }

    Is_Ricetta_Da_APP() {
        let flag: boolean = false;

        if (this.TestataForm?.get("Origine")?.getRawValue() === enum_OrigineApp.GiasApp &&
            this.TestataForm?.get("Codici_Attivita")?.getRawValue().findIndex(c => c.APP_RicettaOperazione_ID && c.APP_RicettaOperazione_ID !== "") > -1) {

            flag = true;

        }

        return flag;
    }

    Is_Ricetta_Da_Demetra() {
        let flag: boolean = false;

        if (this.TestataForm?.get("Origine")?.getRawValue() === enum_OrigineApp.Demetra &&
            this.TestataForm?.get("Codici_Attivita")?.getRawValue().findIndex(c => c.APP_RicettaOperazione_ID && c.APP_RicettaOperazione_ID !== "") > -1) {

            flag = true;

        }

        return flag;
    }

    Is_Ricetta_Da_PUA() {
        let flag: boolean = false;

        if (this.TestataForm?.get("Origine")?.getRawValue() === enum_OrigineApp.PUA &&
            this.TestataForm?.get("Codici_Attivita")?.getRawValue().findIndex(c => c.APP_RicettaOperazione_ID && c.APP_RicettaOperazione_ID !== "") > -1) {

            flag = true;

        }

        return flag;
    }

    Is_Ribaltamento_Ricetta_Da_Origine_Diversa() {
        let flag: boolean = false;

        if (this.TipoRibaltamento !== RibaltamentoTypes.Nessuno &&
            (this.Is_Ricetta_Da_APP() ||
                this.Is_Ricetta_Da_Demetra())) {

            flag = true;

        }

        return flag;
    }

    Is_Ribaltamento_Ricetta_Da_PUA() {
        let flag: boolean = false;

        if (this.TipoRibaltamento !== RibaltamentoTypes.Nessuno &&
            this.Is_Ricetta_Da_PUA()) {

            flag = true;

        }

        return flag;
    }

    get_Tipo_Ricetta(): Tipo_Ricetta {

        let tipo_ricetta = 0;

        if (this.TestataForm &&
            this.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta &&
            this.TestataForm.get("Stato").value === Stati.Da_Eseguire) {

            tipo_ricetta = this.TestataForm.get("TipoRicetta").value;

        }

        return tipo_ricetta;
    }

    /*
    * @description:
    * Richiamo questa funzione in generale quando devo impostare una nuova percentuale di N senza dovergli per forza passare l'operazione
    * */
    Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti_Tutti() {

        let Operazioni: Array<Lavorazione> = this.TestataForm?.get("Operazioni")?.getRawValue();

        if (Operazioni && Operazioni.length > 0) {
            let Operazioni_Con_Fertilizzanti = Operazioni.filter(o => this.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice));

            if (Operazioni_Con_Fertilizzanti && Operazioni_Con_Fertilizzanti.length === 1) {
                let DosiProdotti = this.DosiProdottiFormArray(null, Operazioni_Con_Fertilizzanti[0])?.getRawValue();

                if (DosiProdotti && DosiProdotti.length > 0) {
                    DosiProdotti.forEach(d => {
                        this.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(d.DosiProdottiGridrowId);
                    })
                }
            }
        }


    }

    /*
    * @description:
    * Calcola la percentuale di N da utilizzare in base alla riga di prodotto (DosiProdottiGridrowId)
    * */
    Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(DosiProdottiGridrowId: number) {

        let Operazioni: Array<Lavorazione> = this.TestataForm?.get("Operazioni")?.getRawValue();

        if (Operazioni && Operazioni.length > 0) {
            let Operazioni_Con_Fertilizzanti = Operazioni.filter(o => this.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice));

            if (Operazioni_Con_Fertilizzanti && Operazioni_Con_Fertilizzanti.length === 1) {
                let Sezione_Fertilizzanti = this.Sezione_ProdottoFormGroup(Operazioni_Con_Fertilizzanti[0]);

                if (Sezione_Fertilizzanti && this.mostraPercentualeN(Sezione_Fertilizzanti.getRawValue().Operazione)) {

                    let DosiProdottiFormArray: FormArray = this.DosiProdottiFormArray(null, Operazioni_Con_Fertilizzanti[0]);

                    if (DosiProdottiFormArray && DosiProdottiFormArray.controls.length > 0) {

                        let index = DosiProdottiFormArray.getRawValue().findIndex(d => DosiProdottiGridrowId === d.DosiProdottiGridrowId);

                        if (index > -1) {

                            let percentuale: number = DosiProdottiFormArray.controls[index].get("N_Percentuale_X_Prodotto").getRawValue();

                            let new_percentuale: number = 0;

                            let riga_N_Residuo_minore = this.get_Riga_Impianti_Con_N_Residuo_Minore();

                            if (riga_N_Residuo_minore) {

                                let Prodotto: MultiColumnComboboxFertilizzazione = DosiProdottiFormArray.controls[index].get("Prodotto").getRawValue();

                                if (Prodotto && Prodotto.Codice_Concatenato !== "") {

                                    let n_residuo_impianto = riga_N_Residuo_minore.N_Max;

                                    let dose_ha: number = DosiProdottiFormArray.controls[index].get("Dose_Ha").getRawValue();

                                    let N: number = DosiProdottiFormArray.controls[index].get("N").getRawValue();

                                    let efficienza: number = DosiProdottiFormArray.controls[index].get("Efficienza").getRawValue();

                                    let nuova_n_residua = this.funzionicomuniservice.roundNumber(dose_ha * N / 100 * efficienza, 3);

                                    new_percentuale = this.funzionicomuniservice.roundNumber((nuova_n_residua / n_residuo_impianto) * 100, 2);

                                } else {

                                    //Se il residuo percentuale dell'impianto selezionato è maggiore di 0 allora lo imposto come percentuale
                                    //utilizzabile per il prodotto altrimenti la imposto a 0
                                    if (riga_N_Residuo_minore.N_Residuo_Percentuale > 0) {

                                        new_percentuale = riga_N_Residuo_minore.N_Residuo_Percentuale;

                                        let n_percentuale_x_prodotto: number = this.getPercentualeN_Utilizzata_Operazione(Operazioni_Con_Fertilizzanti[0]);

                                        new_percentuale = this.funzionicomuniservice.roundNumber(new_percentuale - n_percentuale_x_prodotto, 2);
                                    }

                                }
                            }

                            if (new_percentuale !== percentuale) {
                                this.DosiProdottiFormArray(null, Operazioni_Con_Fertilizzanti[0]).controls[index].patchValue({
                                    N_Percentuale_X_Prodotto: new_percentuale
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    /*
    * @description Mostro la Percentuale di N solo per le Ricette di Fertilizzazione e
    * se ho scelto un disciplinare di tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF)
    * */
    mostraPercentualeN(Operazione: Lavorazione): boolean {
        let mostra = false;

        let lav_cod = + Operazione.primaryKey.codice;

        let riga: GridImpiantoSelezionatoModel = this.get_Riga_Impianti_Con_N_Residuo_Minore();

        if (riga && riga.flag_N_Max &&
            this.Elenco_Operazioni_Fertilizzanti.includes(lav_cod) &&
            this.TestataForm?.get("Tipo")?.getRawValue() === Tipo_Attivita.Ricetta &&
            this.TestataForm?.get("TipoRicetta")?.getRawValue() === Tipo_Ricetta.Standard_Destinazioni &&
            this.TestataForm?.get("Stato")?.getRawValue() === Stati.Da_Eseguire &&
            this.getDisciplinareModelValue(lav_cod)?.regolamentoConcimazione?.tipo === enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF) {

            mostra = true;

        }

        return mostra;
    }

    get_Riga_Impianti_Con_N_Residuo_Minore(): GridImpiantoSelezionatoModel {

        let riga: GridImpiantoSelezionatoModel = null;

        let righe_selezionate: GridImpiantoSelezionatoModel[] = (this.ImpiantiSelezionatiFormArray?.getRawValue()) as Array<GridImpiantoSelezionatoModel>;

        if (righe_selezionate && righe_selezionate.length > 0) {
            righe_selezionate = righe_selezionate.sort((a, b) => a.N_Residuo - b.N_Residuo);

            riga = righe_selezionate[0];
        }

        return riga;

    }

    /*
    * @description Ottengo i decimali da impostare in Dose_Ha,Dose_Hl e Dose_Tot in base alla Unita di Misura
    * */
    public getProductNumericSettings(UdM: UnitaDiMisura, default_UdM=false): NumericSettings{

      let settings = this.get4DecimalNumericSettings();

      if(!default_UdM && UdM && UdM.codice > 0){
        let index = this.UdM_Product_Numeric_Settings.findIndex(u=>u.codice === UdM.codice);

        if(index > -1)
          settings = this.UdM_Product_Numeric_Settings[index].settings;
      }

      return settings;
    }

    /*
    * @description Memorizzo i decimali da utilizzare in base all'Unita di Misura
    * */
    public setProductNumericSettings(UdM: UnitaDiMisura) {
      if(UdM && UdM.tipoControllo){
        let index = this.UdM_Product_Numeric_Settings.findIndex(u=>u.codice === UdM.codice);

        if(index === -1)

          switch (UdM.tipoControllo.codice){
            case enum_TipoControllo.NUMERO_DECIMALE:
              this.UdM_Product_Numeric_Settings.push({codice: UdM.codice, settings: this.get4DecimalNumericSettings()});
              break;
            case enum_TipoControllo.NUMERO_INTERO:
              this.UdM_Product_Numeric_Settings.push({codice: UdM.codice, settings: this.get0DecimalNumericSettings()});
              break;
          }


        }

    }

    public get4DecimalNumericSettings(): NumericSettings{
      return new NumericSettings({
        defaultValue: 0,
        format: 'n4',
        min: 0,
        step: 0.0001,
        decimals: 4
      });
    }

    public get0DecimalNumericSettings(): NumericSettings{
      return new NumericSettings({
        defaultValue: 0,
        format: '0',
        min: 0,
        step: 1,
        decimals: 0
      });
    }

    getPercentualeN_Utilizzata_Operazione(Operazione: Lavorazione) {
        let percentuale = 0;

        let DosiProdottiFormArray = this.DosiProdottiFormArray(null, Operazione);

        if (DosiProdottiFormArray && DosiProdottiFormArray.getRawValue().length > 0) {
            DosiProdottiFormArray.getRawValue().forEach(d => {
                percentuale += d.N_Percentuale_X_Prodotto;
            });
        }

        percentuale = this.funzionicomuniservice.roundNumber(percentuale, 2)

        return percentuale;
    }

    /*
    * @description
    * Imposto il PUA nel form della Testata leggendolo lato server oppure impostandolo
    * dal parametro che mi arriva
    * */
    Imposta_PUA(pua: Pua, flag_leggi_PUA: boolean): Promise<boolean> {

        return new Promise<boolean>((resolve, reject) => {

            if ((this.TestataForm.get("Operazioni").getRawValue() as Array<Lavorazione>).findIndex(o => + o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) > -1 &&
                (this.get_Tipo_Ricetta() === Tipo_Ricetta.Standard_Destinazioni || this.TestataForm.get("Tipo").getRawValue() === Tipo_Attivita.QuadernoDiCampagna)) {

                let AggiornaPUA_nella_TestataForm = (pua: Pua) => {
                    let codici_attivita: Array<CodiciXOperazione> = this.TestataForm?.get("Codici_Attivita")?.getRawValue();

                    if (codici_attivita && codici_attivita.length > 0) {
                        codici_attivita.forEach(c => {
                            c.pua = pua;
                        });

                        this.TestataForm.patchValue({
                            Codici_Attivita: codici_attivita
                        }, { emitEvent: false })
                    }
                };

                if (flag_leggi_PUA) {
                    const LeggiPua = <LeggiPUA>{
                        impresa: this.getImpresa_Model(),
                        data: this.TestataForm.get("Data").getRawValue(),
                        operazioni: this.TestataForm.get("Operazioni").getRawValue(),
                        tipo_Attivita: this.TestataForm.get("Tipo").getRawValue(),
                        tipo_Ricetta: this.TestataForm.get("TipoRicetta").getRawValue(),
                        stato: this.TestataForm.get("Stato").getRawValue(),
                        disciplinare: this.getDisciplinareModelValue(enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)
                    };

                    this.agendaservice.Recupera_Pua(LeggiPua).subscribe(risp => {
                        AggiornaPUA_nella_TestataForm(risp.RispostaStringa);

                        resolve(true);
                    });
                } else {
                    AggiornaPUA_nella_TestataForm(pua);

                    resolve(true);
                }
            } else {
                resolve(true);
            }
        });

    }

    public Sola_Lettura_QdCForm(): boolean {

        let flag_sola_lettura: boolean = false;

        let blocco: Blocco = this.TestataForm?.get("Blocco_Attivita")?.getRawValue();

        // Se sono in Lettura oppure l'operazione è bloccata disabilito tutti i controlli.
        //Anche se sto facendo un Reinnesco Trappole disabilito tutti i controlli tranne la Data e il bottone di cancellazione dei Prodotti
        if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read ||
            (blocco && blocco.tipo === Tipo_Blocco.QuadernoDiCampagna) ||
            this.flag_Reinnesco) {

            flag_sola_lettura = true;
        }

        return flag_sola_lettura;
    }

    public RicaricaTutteGridProdottixImpianti(flag_calcola_qta_su_impianti: boolean = true){
      if(this.TestataForm.get("Operazioni")?.getRawValue()?.findIndex((o: Lavorazione)=> this.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+o.primaryKey.codice)) > -1){
        this.SubsRefreshGridProdottiXImpianti.next({ RicaricaGrid: true, Flag_Calcola_Qta_Su_Impianti: flag_calcola_qta_su_impianti });
      }
    }

    public AggiornaDescrizioniGridProdottixImpianti(){

      let Operazioni: Array<Lavorazione> = this.TestataForm.get("Operazioni").getRawValue();

      if(Operazioni && Operazioni.length > 0){

        let Operazioni_Con_Trappole: Array<Lavorazione> = Operazioni.filter(o=>this.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+o.primaryKey.codice));

        if(Operazioni_Con_Trappole && Operazioni_Con_Trappole.length > 0){

          let Aggiorna_Descrizioni: boolean = false;

          for(let index_Operazione = 0; index_Operazione < Operazioni_Con_Trappole.length; index_Operazione++){

            let DosiProdotti: Array<Dettaglio_Formulato> = this.DosiProdottiFormArray(null,Operazioni_Con_Trappole[index_Operazione]).getRawValue();

            if(DosiProdotti && DosiProdotti.length > 0){

              for(let index_Dosi = 0; index_Dosi < DosiProdotti.length; index_Dosi++){

                if(DosiProdotti[index_Dosi].QuantitaSuImpianti && DosiProdotti[index_Dosi].QuantitaSuImpianti.length > 0){

                  if(DosiProdotti[index_Dosi].QuantitaSuImpianti.findIndex(qt=>qt.esercizioCDC.esercizio.descrizione === "") > -1){

                    Aggiorna_Descrizioni = true;

                    break;
                  }
                }
              }
            }
          }

          if(Aggiorna_Descrizioni)
            this.RicaricaTutteGridProdottixImpianti(false);

        }
      }
    }

    public QuantitaSuImpianto_Presente(QuantitaSuImpianti: QuantitaSuImpianto[], QuantitaSuImpianto_Da_Cercare: QuantitaSuImpianto): number{

      let index: number = -1

      if(QuantitaSuImpianti && QuantitaSuImpianti.length > 0 && QuantitaSuImpianto_Da_Cercare){
        index = QuantitaSuImpianti.findIndex(Qt=> Qt?.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.centroAziendalePK?.partitaIva === QuantitaSuImpianto_Da_Cercare.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.centroAziendalePK?.partitaIva &&
                                                  Qt?.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.centroAziendalePK?.codice === QuantitaSuImpianto_Da_Cercare.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.centroAziendalePK?.codice &&
                                                  Qt?.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.codice === QuantitaSuImpianto_Da_Cercare.esercizioCDC?.esercizio?.impiantoPK?.appezzamentoPK?.codice &&
                                                  Qt?.esercizioCDC?.esercizio?.impiantoPK?.codice === QuantitaSuImpianto_Da_Cercare.esercizioCDC?.esercizio?.impiantoPK?.codice &&
                                                  Qt?.esercizioCDC?.esercizio?.codice === QuantitaSuImpianto_Da_Cercare.esercizioCDC?.esercizio?.codice);
      }

      return index;
    }

    private decodeHtmlText(value: any): string {
        let htmlString: string = `<div><p>${value}</p></div>`;
        const temp = document.createElement("div");
        temp.innerHTML = htmlString;
        return temp.textContent || temp.innerText || '';
    }

    private setDefaultGISPanelFromCookie() {
        if (this.GISPanel && this.GISPanel.Polygon) {
            let key: string = this.getCookieKeyGISPanel();

            this.cookieService.deleteCookie(key);

            this.cookieService.setCookie({
                name: key,
                value: this.GISPanel?.Panel?.IsExpanded?.getValue().toString()
            });
        }
    }

    private setDefaultImpiantiPanelFromCookie() {
        if (this.ImpiantiPanelBar) {
            let key: string = this.getCookieKeyImpiantiPanel();

            this.cookieService.deleteCookie(key);

            this.cookieService.setCookie({
                name: key,
                value: this.ImpiantiPanelBar?.IsExpanded?.getValue().toString()
            });
        }
    }

    public RilievoSenzaImpianti(attivita: Attivita = null): boolean {

        let senzaImpianti: boolean = false;

        if (attivita) {
            if (attivita.job && ElencoLavCodRilieviSenzaImpianti.findIndex(x => x === + attivita.job.primaryKey.codice) > -1) {
                if (!attivita.utilizzoTerreno || (attivita.utilizzoTerreno.classType === "Varieta" && (attivita.utilizzoTerreno as Varieta).specie.codice === 0)) {

                    let eserciziCDC: EsercizioCDC[] = null;

                    if (attivita.centriDiCosto && attivita.centriDiCosto.length > 0) {
                        eserciziCDC = (attivita.centriDiCosto.filter(c => c.classType === "EsercizioCDC" &&
                            (c as EsercizioCDC).esercizio.codice === 0 &&
                            (c as EsercizioCDC).esercizio.impiantoPK.codice === 0 &&
                            (c as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.codice === 0 &&
                            (c as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice > 0 &&
                            (c as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva !== "")) as EsercizioCDC[];
                    }


                    if (!eserciziCDC || eserciziCDC.length === attivita.centriDiCosto.length) {
                        if (attivita.risorse && attivita.risorse.length > 0) {

                            let dettagliRilievo: DettaglioRilievo[] = (attivita.risorse.filter(r => r.classType === "DettaglioRilievo") as DettaglioRilievo[]);

                            let dettaglioRilievoSenzaImpianti: DettaglioRilievo[] = cloneDeep(dettagliRilievo).filter(d =>
                                (!d.esercizioCDC) ||
                                (d.esercizioCDC.esercizio.codice === 0 &&
                                    d.esercizioCDC.esercizio.impiantoPK.codice === 0 &&
                                    d.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice === 0 &&
                                    d.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice > 0 &&
                                    d.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva !== ""));

                            if (dettagliRilievo.length === dettaglioRilievoSenzaImpianti.length)
                                senzaImpianti = true;

                        }
                    }

                }
            }
        } else {
            const operazioni: Array<Lavorazione> = this.TestataForm?.get('Operazioni').getRawValue();

            if (operazioni && operazioni.length === 1 && operazioni.findIndex(o => ElencoLavCodRilieviSenzaImpianti.includes(+o.primaryKey.codice)) > -1) {
                const specie: Specie = this.GetSpeciefromUtilizzoTerreno();

                if (specie && specie.codice === NessunaSpecieQdC)
                    senzaImpianti = true;
            }
        }



        return senzaImpianti;
    }

    setDefaultFromCookie() {

        this.setDefaultSpecieFromCookie();

        this.setDefaultGISPanelFromCookie();

        this.setDefaultImpiantiPanelFromCookie();
    }

    public getCentroDiCostoTipo(): Tipo | null {
        const operazioni = this.TestataForm?.get('Operazioni') as FormControl<Lavorazione[]>;
        if (operazioni?.value == null || operazioni.value.length == 0) {
            return null;
        }

        if (this.isProdottoDaTrattare(+operazioni.value[0].primaryKey.codice)) {
            return Tipo.ProdottoDaTrattare;
        }

        return Tipo.Esercizio;
    }


    public isProdottoDaTrattare(codice: number): boolean {
        return codice == enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA ||
            codice == enum_LAVCOD.CONCIA_SEME;
    }

    public getIrrigazioneUtilizzata_Descrizione(flagIsMacchina: boolean, descr: string): string {
        if (flagIsMacchina) {
            return '---  ' + this.translocoService.translate(`MacchineIrrigazione`) + '  --- ' + descr;
        } else {
            return '---  ' + this.translocoService.translate(`ImpiantoIrrigazione`) + '  --- ' + descr;
        }
    }

    private getCookieKeySpecie() {

        this.Utente = this.PermessiUtenteService.getCurrentUser();

        return `Specie_QdC_${this.Utente.Username}`;
    }

    private getCookieKeyGISPanel() {

        this.Utente = this.PermessiUtenteService.getCurrentUser();

        return `GISPanel_QdC_${this.Utente.Username}`;
    }

    private getCookieKeyImpiantiPanel() {

        this.Utente = this.PermessiUtenteService.getCurrentUser();

        return `ImpiantiPanel_QdC_${this.Utente.Username}`;
    }

    /*
  * @description Modalita Applicazione visibile solo nel QdCA (No Ricette/Brogliaccio) e se è un'operazione
  * dell Esportazione Registro Trattamenti Agea che gestisce la Modalità applicazione (applicationTypeCode)
  * @link \\\rubino2\DOCUMENTAZIONE\GIAS --- Clienti --- AGEA\Interoperabilita_Quaderno di Campagna v.1.0n.docx
  * */
    mostraModalitaApplicazione(Operazione: Lavorazione): boolean {
        let mostra: boolean = false;

        if (this.TestataForm.get("Tipo").getRawValue() === Tipo_Attivita.QuadernoDiCampagna &&
            (Operazione &&
                (+ Operazione.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.DISERBO ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.GEODISINFESTAZIONE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_CONCIME ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.SARCHIATURA_CONCIMAZIONE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.CONCIMAZIONE_FOGLIARE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIBUTTERATURA ||
                    + Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI))) {

            mostra = true;

        }


        return mostra;
    }

    public IsModalitaDemetra(): boolean{
      return this.getSettingValueAs(
        enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra, this.equalsOne, false
      );
    }

    /*
    * @description Se il rilievo non ha impianti (è stata scelta la voce Nessuna specie), disabilito il pannello
    * @param readFromCookie: se true leggo il valore del cookie per espandere o meno il pannello (passato a true solamente al caricamento della pagina)
    * */
    public DisableExpandImpiantiPanel(readFromCookie: boolean = false) {

        if (this.ImpiantiPanelBar) {

            if (this.RilievoSenzaImpianti()) {
                this.ImpiantiPanelBar.expand = false;
                this.ImpiantiPanelBar.disabled = true;
            } else {

                if (readFromCookie) {
                    this.ImpiantiPanelBar.expand = this.getDefaultImpiantiPanelFromCookie();
                    this.ImpiantiPanelBar.disabled = false;
                } else {
                    if (this.ImpiantiPanelBar.disabled) {
                        this.ImpiantiPanelBar.expand = true;
                        this.ImpiantiPanelBar.disabled = false;
                        this.OnExpandPanel('Impianti');
                    }
                }

            }

        }
    }

    public OnExpandPanel(panelname: string) {
        setTimeout(() => {
            //Appena viene aperto il pannello eseguo l'autofitcolumn per sistemare le colonne della grid

            switch (panelname) {
                case 'Impianti':
                    this.GridImpiantiPublicService?.gridComp?.autoFitColumns();
                    break;
                case 'ProdottiMagazzino':
                    this.GridProdottiDaTrattarePublicService?.gridComp?.autoFitColumns();
                    break;
            }

        }, 100);
    }

    ngOnDestroy() {
        this.Subs.unsubscribe();

        if (!this.TestataForm.get("flagVisita").getRawValue())
            this.setDefaultFromCookie();
    }

}
