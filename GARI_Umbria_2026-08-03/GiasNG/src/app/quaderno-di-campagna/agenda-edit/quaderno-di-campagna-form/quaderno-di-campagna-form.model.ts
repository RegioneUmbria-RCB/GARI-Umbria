import { Campo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import {Attivita, Tipo_Ricetta} from 'app/Model/attivita/Attivita';
import { AttivitaPersonalizzata } from 'app/Model/attivita/AttivitaPersonalizzata';
import { DettaglioFertilizzazione } from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { DettaglioSemina } from 'app/Model/attivita/dettagli/DettaglioSemina';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { DoseAcqua } from 'app/Model/attivita/risorse/RisorsaAcqua';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { DoseEtichetta } from 'app/Model/metaschema/DoseEtichetta';
import { Epoca } from 'app/Model/metaschema/Epoca';
import { Soglia } from 'app/Model/metaschema/Soglia';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { MultiColumnComboboxGiasTemplate } from 'gias-ui-kit';
import { NotaInterventoDdlItem } from '../componenti/grid-note/note.model';
import { OpzioniRaccolta } from '../componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model';
import {ErroreGias, ErroreGias_Severity} from "../../../Service/master.service";
import {DittaMacchina} from "../../../Model/metaschema/DittaMacchina";
import {MacchineDettaglio1} from "../../../Model/metaschema/MacchineDettaglio1";
import {MacchineDettaglio2} from "../../../Model/metaschema/MacchineDettaglio2";
import {Macchine} from "../../../Model/metaschema/Macchine";
import {DettaglioRilievo} from '../../../Model/attivita/dettagli/DettaglioRilievo';
import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {AssociazionePK} from "../../../Model/attivita/AssociazionePK";
import { MenuRicette } from "../../../Service/Agenda/Agenda.service";
import { Avversita } from 'app/Model/metaschema/avversita/Avversita';
import { Operatore } from 'app/Model/metaschema/utilizzi/Operatore';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import {Pua} from "../../../Model/metaschema/Pua";
import { DropdownListSpecieAnimali } from '../service/testata/specie-animali-service';
import {Blocco} from "../../../Model/attivita/Blocco";
import { ProdottoDaTrattareCDC } from 'app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC';
import {Tipo_Polverulento} from "../../../Model/TipiEnumerativi";
import {PrincipioAttivo} from "../../../Model/metaschema/PrincipioAttivo";
import { Stati, Tipo_Attivita } from 'gias-ui-kit';
import {IntervalloTemporale} from "../../../Model/anagrafiche/IntervalloTemporale";
import { BufferZone } from 'app/Model/metaschema/BufferZone';
import {QuantitaSuImpianto} from "../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {TipoIrrigazione} from "../../../Model/attivita/dettagli/DettaglioIrrigazione";
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import {DropdownListItem} from "gias-kendo-grid/lib/Template/kendo-grid/models/grid.model";

export class QdCFormModel{

    Trattamento: Trattamento;

    constructor(){
        this.Trattamento=new Trattamento();
    }
}

export class Trattamento{

    Testata: Testata;
    CostiAccessori: CostiAccessori;
    Impianti: Impianti;
    ProdottiDaTrattare: ProdottiDaTrattare;
    Superfici: Superfici;
    Acqua: Acqua;
    Sezioni_Prodotto: Array<Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta>;
    Sezioni_Senza_Prodotto: Array<Sezione_Rilievi>;
    Causali: Array<Causale>;
    Nota_Testuale: string;
    Note: Array<NotaInterventoDdlItem>;

    constructor(){
        this.Testata=new Testata();
        this.CostiAccessori= new CostiAccessori();
        this.Impianti = new Impianti();
        this.ProdottiDaTrattare = new ProdottiDaTrattare();
        this.Superfici=new Superfici();
        this.Sezioni_Prodotto = [];
        this.Sezioni_Senza_Prodotto = [];
        this.Causali = [];
        this.Nota_Testuale = "";
        this.Note = [];
    }
}

export class Testata{
    Codici_Attivita: Array<CodiciXOperazione>;
    Origine: string;

    flagVisita: boolean;    //valorizzato a true solo in caso si arrivi dal monitor Menu Visite

    //MultiCentro valorizzato a true solo in modifica di una operazione multicentro e insieme con la proprietà
    //Centro_Aziendale valorizzata nell'Array Codici_Attivita
    MultiCentro: boolean;
    Raccoglitore: number;
    Tipo: Tipo_Attivita;
    TipoRicetta: Tipo_Ricetta;
    Stato: Stati;
    Latitude: number;
    Longitude: number;
    Visualizza_Solo_Operazioni_Preferite: boolean;
    Operazioni: Array<Lavorazione>;
    Data: Date;
    Ora: Date;
    Specie: Specie | DestinazioneUso;
    Disciplinare: DropdownListDisciplinare;
    Centro_Aziendale: CentroAziendale;
    Campo: DropdownListCampo;
    Attivita_Personalizzata: DropdownListAttivitaPersonalizzata;
    Descrizione_Altre_Lavorazioni: string;
    Attivita_Collegate: Array<Attivita>;
    Blocco_Attivita: Blocco;
    //Valorizzati solo per le Ricette
    InviaRicetta: boolean;
    Ricetta_Des: string;
    Ricetta_Des_Long: string;
    Ricetta_Numero: string;
    Ricetta_Note: string;
    Ricetta_Data_Da: Date;
    Ricetta_Data_A: Date;
    TestataVisita: Testata_Visita;
    constructor(){
        this.Codici_Attivita = [];
        this.Operazioni = [];
        this.Attivita_Collegate = [];
        this.TestataVisita = new Testata_Visita();
    }
}

export class Testata_Visita{
    //valorizzati per le Visite
    // Data_e_Ora_Visita: Date;
    Data_Visita: Date;
    Ora_Inizio_Visita: Date;
    Ora_Fine_Visita: Date;
    Operatore_Visita: Operatore;
    Azienda_Visita: Impresa;
    Da_Remoto_Visita: boolean;
    statoWorkflow_Visita: boolean
    Aziende_Agenzie: boolean;
    impOrarioFine_Visita: number;
    NrOreTotali_Visita: number;
    SpecieAnimali_Visita: DropdownListSpecieAnimali;
    Visualizza_Specie: boolean;

}

export class Impianti{
    ImpiantiSelezionati: Array<GridImpiantoSelezionatoModel>;

    constructor(){
        this.ImpiantiSelezionati = [];
    }
}

export class ProdottiDaTrattare{
    ProdottiDaTrattareSelezionati: Array<ProdottoDaTrattareCDC>;

    constructor(){
        this.ProdottiDaTrattareSelezionati = [];
    }
}

export class Superfici{
    Sup_Selezionata: number;
    Sup_Trattata: number;
}

export class Sezione {
  Operazione: Lavorazione;
}

export class Dettaglio_Prodotto extends Sezione{
  Categoria_Magazzino: number;
  Centro_Aziendale: CentroAziendale;
  Piva: string;
  Sa_Cod: number;
  Fabbricato_Cod: number;
  Fabbricato_Des: string;
  Fr_Cod: number;
  Fr_Des: string;
  UdM: UnitaDiMisura;
  UdM_Magazzino: UnitaDiMisura;
  UdM_Indicata: UnitaDiMisura;
  Udm_Cod: string;
  Udm_Des: string;
  Udm_Cod_Trasformato: number;
  Magazzino_del_Prodotto_Selezionato: DropdownListMagazzino;
  Dose_Ha: number;
  Dose_Hl: number;
  DoseTot_Ha: number;
  Ha_Hl: number;
  Dose_QtaTot: number;
  For_Veg_Cod: number;
  Dose_Etichetta_Max: number;
  Dose_Etichetta_Value: number;
  Limite_Numero_Trattamenti: number;
  Limite_Numero_Trattamenti_UDM_SIM: number;
  Limite_Numero_Trattamenti_UDM_COD: number;
  IntervalloTrattamenti_Min: number;
  IntervalloTrattamenti_Max: number;
  strPA_COD: string;
  strPA_COD_Pesi: string;
  strCLTOSS_COD: string;
  MgO: number;
  Dose_Fittizia: number;
  Lotto: string;
  Mat_Regolamento: number;
  Mat_Veg_Cod: number;
  Mat_Cul_Cod: number;
  Piva_Rif: string;
  Sa_Cod_Rif: number;
  ID_Agenda_Rif: number;
  ID_Mov_Rif: number;
  ID_Mov_Det_Rif: number;
  Lav_Cod_Rif: number;
  Cau_Mov_Rif: string;
  Des_Rif: string;
  Qta_Rif: number;
  flagTipoDose: number;
  flagDoseQuantitaTotale: number;
  Dose_Acqua: number;
  DosiProdottiGridrowId: number;
  Visualizza_Magazzini_Agenzie: boolean;
  Visualizza_Magazzini_Esterni: boolean;
  Magazzino_Agenzia: Fabbricato;
  Magazzino_Esterno: Fabbricato;
  Visualizza_Solo_Prodotti_in_Giacenza: boolean;
  Riga_Salvata: boolean;
  Nuova_Riga: boolean;
  Modalita_Applicazione: BaseCodeDescr;
  //Memorizzo tutto il valore della sezione durante la creazione nel quaderno-di-campagna-form.service.ts
  Original_Row_Value: object;
  Tutti_Problemi_DettaglioProdotto: Array<enum_Problema_DettaglioProdotto>;
  Problema_DettaglioProdotto_Da_Risolvere: enum_Problema_DettaglioProdotto;
  //Nessun_Magazzino:boolean;
  //magazzino: Fabbricato;

  constructor() {
    super();
  }
}

export class Dettaglio_Formulato extends Dettaglio_Prodotto{
  Ripartizione_Trappole: BaseCodeDescr;
  Prodotto: MultiColumnComboboxTrattamento;
  Avversita: DropdownListAvversita;
  Av_Cod: number;
  Av_Gru: string;
  Av_Des: string;
  Soglia_Avversita: Soglia;
  Soglia_Value: number;
  Soglia_Des: string;
  Dose_Etichetta: MultiColumnComboboxDose_Etichetta;
  Dosi_Etichetta: MultiColumnComboboxDose_Etichetta[];
  Dosi_Etichetta_Des: string;
  BufferZone: BufferZone;
  BufferMin: number;
  BufferMax: number;
  strBuffer: string;
  Carenza: number;
  Prima_Raccolta: string;
  Polverulento: Tipo_Polverulento;
  EpocaDPI: Epoca;
  PrincipiAttivi: PrincipioAttivo[];
  QuantitaSuImpianti: QuantitaSuImpianto[];
  Magazzino_Innesco: DropdownListMagazzino;
  Lotto_Innesco: string;
  UdM_Innesco: UnitaDiMisura;
  Magazzino_Agenzia_Innesco: Fabbricato;
  DoseTot_Ha_Innesco: number;
  Piva_Innesco: string;
  Sa_Cod_Innesco: number;
  Fabbricato_Cod_Innesco: number;
  Innesco_Incluso: boolean;
  Visualizza_Solo_Inneschi_in_Giacenza: boolean;

  constructor() {
    super();
  }
}

export class Dettaglio_Fertilizzante extends Dettaglio_Prodotto{
  Prodotto: MultiColumnComboboxFertilizzazione;
  Efficienza: number;
  N: number;
  N_Utile: number;
  P: number;
  K: number;
  Cu: number;
  EpocaFertilizzazione: Epoca;
  Utilizza_Direttiva_Nitrati: boolean;
  Direttiva_Nitrati: Disciplinare;
  N_Percentuale_X_Prodotto: number;
  constructor() {
    super();
  }
}

export class Dettaglio_Semente extends Dettaglio_Prodotto{
  Prodotto: MultiColumnComboboxSemina;
  Cod_Articolo: string;
  Opzioni_Semina: BaseCodeDescr;
  Sup_Calcolata: number;

  constructor() {
    super();
  }
}

export class Sezione_Prodotto extends Sezione {
  Categoria_Magazzino: number;
  //Nessun_Magazzino:boolean;
  //magazzino: Fabbricato;
  flagDoseQuantitaTotale: number;
  flagTipoDose: number;
  Visualizza_Solo_Prodotti_in_Giacenza: boolean;
  Magazzino_del_Prodotto_Selezionato: DropdownListMagazzino;
  Lotto: string;
  UdM: UnitaDiMisura;
  Dose_Ha: number;
  Dose_Hl: number;
  DoseTot_Ha: number;
  Modalita_Applicazione: BaseCodeDescr;
  DosiProdotti: Array<any>;

  constructor(){
    super();
    this.DosiProdotti = [];
  }
}

export class Sezione_Prodotto_Formulati extends Dettaglio_Formulato{
    /*Prodotto: MultiColumnComboboxTrattamento;
    EpocaDPI: Epoca;
    Avversita: DropdownListAvversita;
    Soglia_Avversita: Soglia;
    Dose_Etichetta: MultiColumnComboboxDose_Etichetta;*/

    DosiProdotti: Array<Dettaglio_Formulato>;

    constructor(){
        super();
        this.DosiProdotti = [];
    }
}

export class Sezione_Prodotto_Fertilizzanti extends Dettaglio_Fertilizzante{
    /*Utilizza_Direttiva_Nitrati: boolean;
    Direttiva_Nitrati: Disciplinare;
    Prodotto: MultiColumnComboboxFertilizzazione;
    N_Percentuale_X_Prodotto: number;
    EpocaFertilizzazione: Epoca;
    Efficienza: number;
    N_Utile: number;
    N: number;
    P: number;
    K: number;
    Cu: number;*/

    DosiProdotti: Array<Dettaglio_Fertilizzante>;

    constructor(){
        super();
        this.DosiProdotti = [];
    }
}

export class Sezione_Prodotto_Sementi extends Dettaglio_Semente{
/*    Prodotto: MultiColumnComboboxSemina;
    Opzioni_Semina: BaseCodeDescr;
    Sup_Calcolata: number;*/

    DosiProdotti: Array<Dettaglio_Semente>;

    constructor(){
        super();
        this.DosiProdotti = [];
    }
}

export class Sezione_Prodotto_Raccolta extends Sezione_Prodotto{
    ProdottiRaccolti: Array<DettaglioRaccolta>;
    Opzioni_Raccolta: OpzioniRaccolta;
    Data_Raccolta: Date;
    // Valorizzato alla creazione della sezione raccolta,
    // serve in caso vengano rimossi tutti i prodotti (anche 'Nessuna Selezione')
    FastHarvestTemplate: DettaglioRaccolta;

    constructor(){
        super();
    }
}

export  class  Sezione_Rilievi extends Sezione {
    DettagliRilievi: Array<DettaglioRilievo> = [];

    constructor() {
        super();
    }

    public toFormGroup(): FormGroup {
        const form = new FormGroup({});

        let rilievi: FormArray = new FormArray([]);
        this.DettagliRilievi.forEach(r => rilievi.push(r.toFormGroup()));
        form.addControl('DettagliRilievi', rilievi);

        form.addControl('Operazione', new FormControl(this.Operazione))

        return form;
    }
}

export class Acqua{
    Acqua_Ha: number;
    Acqua_Tot: number;
    Dose_Acqua: DoseAcqua;
}

export class CostiAccessori{
    Macchine: Array<GridMacchinaModel>;
    Operatori: Array<GridOperatoreModel>;

    constructor(){
        this.Macchine = [];
        this.Operatori = [];
    }
}

export class GridImpiantoSelezionatoModel{
    PIVA: string;//chiave dell'impianto
    SA_COD: number;//chiave dell'impianto
    APPEZZA: number;//chiave dell'impianto
    ID_REG: number;//chiave dell'impianto
    Progetto_Cod: number;//chiave dell'esercizio
    Progetto_Des: string;//nome dell'esercizio
    GRFI_COD: number;
    Grfi_Des: string;
    Cop_Cod: number;
    Copertura: string;
    Flag_Protetto: number;
    App_Nome: string;
    Sup_Imp_help: number;
    Sup_Riduzione_BufferZone: number;
    Perc_Riduzione_Deriva: number;
    CUL_COD: number;
    Cul_Des: string;
    ListaClassiTessitura: Array<ClasseTessitura>;
    Validita_Inizio_Distinta: Date;
    Validita_Fine_Distinta: Date;
    Obj_Disciplinare: Disciplinare;
    Regolamento_Cod: number;
    Data_Raccolta: Date;
    Data_Raccolta_Prevista: Date;
    CarenzaStr: string;
    DataCarenza: Date;
    Stato_Cod: string;
    flag_N_Max: boolean;
    flag_P_Max: boolean;
    flag_K_Max: boolean;
    flag_Mg_Max: boolean;
    flag_CU_Max: boolean;
    N_Max: number;
    P_Max: number;
    K_Max:number;
    Mg_Max:number;
    CU_Max:number;
    N_Residuo: number;
    P_Residuo: number;
    K_Residuo:number;
    Mg_Residuo:number;
    CU_Residuo:number;
    N_Residuo_Percentuale:number;
    P_Residuo_Percentuale:number;
    K_Residuo_Percentuale:number;
    Mg_Residuo_Percentuale:number;
    CU_Residuo_Percentuale:number;
    //-----------------------------------------------------------------------------------
    Sup_Imp: number;
    DistBZ_CorpiIdrici: number;
    DistBZ_AreeResPub: number;
    DistBZ_Allevamenti: number;
    DistBZ_VegNatNonColt: number;
    SupBZ_Riduzione: number;
    Dichiarazione_Non_Utilizzo_Trattamenti: boolean;
    Dichiarazione_Non_Utilizzo_Fertilizzazioni: boolean;
    //-----------------------------------------------------------------------------------
    IrrigazioneUtilizzata_Codice: string | DropdownListItem;
    IrrigazioneUtilizzata_Descrizione: string;
    Consiglio_Descrizione: string;
    Consiglio_Codice: number | DropdownListItem;
    DataConsiglio: Date;
    DoseAcquaConsiglio: number;
    UdmConsiglio: UnitaDiMisura;
    minDataTurno: Date;
    maxDataTurno: Date;
    UdmDose: string;
    DoseAcquaGiornaliera: number;
    OreIrrigazione: number;
    Portata: number;
    Efficienza: number;
    DataInizioIrrigazione: Date;
    DataFineIrrigazione: Date;
    FrequenzaIrrigazioneMedia: number;
    tipoIrrigazione: TipoIrrigazione;
    macchinaIrrigazione: ParcoMacchine;

    constructor(
        PIVA: string,
        SA_COD: number,
        APPEZZA: number,
        ID_REG: number,
        Progetto_Cod: number,
        GRFI_COD: number,
        Grfi_Des: string,
        Cop_Cod: number,
        Copertura: string,
        Flag_Protetto: number,
        Sup_Imp_help: number,
        Sup_Riduzione_BufferZone: number,
        Perc_Riduzione_Deriva: number,
        CUL_COD: number,
        Cul_Des: string,
        ListaClassiTessitura: Array<ClasseTessitura>,
        Validita_Inizio_Distinta: Date,
        Validita_Fine_Distinta: Date,
        App_Nome: string,
        Obj_Disciplinare: Disciplinare,
        Regolamento_Cod: number,
        Data_Raccolta: Date,
        Data_Raccolta_Prevista: Date,
        CarenzaStr: string,
        DataCarenza: Date,
        Sup_Imp: number,
        DistBZ_CorpiIdrici: number,
        DistBZ_AreeResPub: number,
        DistBZ_Allevamenti: number,
        DistBZ_VegNatNonColt: number,
        SupBZ_Riduzione: number,
        Progetto_Des: string = "",
        Stato_Cod:string,
        IrrigazioneUtilizzata_Codice: string = "",
        IrrigazioneUtilizzata_Descrizione: string = "",
        Consiglio_Descrizione: string = "",
        Consiglio_Codice: number = 0,
        DataConsiglio: Date = new Date(),
        DoseAcquaConsiglio: number = 0,
        UdmConsiglio: UnitaDiMisura = null,
        minDataTurno: Date = null,
        maxDataTurno: Date = null,
        UdmDose: string = "",
        DoseAcquaGiornaliera: number = 0,
        OreIrrigazione: number = 0,
        Portata: number = 0,
        Efficienza: number = 0,
        DataInizioIrrigazione: Date = new Date(),
        DataFineIrrigazione: Date = new Date(),
        FrequenzaIrrigazioneMedia: number = 0,
        tipoIrrigazione: TipoIrrigazione = null,
        macchinaIrrigazione: ParcoMacchine = null,
        flag_N_Max: boolean = false,
        flag_P_Max: boolean = false,
        flag_K_Max: boolean = false,
        flag_Mg_Max: boolean = false,
        flag_CU_Max: boolean = false,
        N_Max: number = 0,
        P_Max: number = 0,
        K_Max:number = 0,
        Mg_Max:number = 0,
        CU_Max:number = 0,
        N_Residuo: number = 0,
        P_Residuo: number = 0,
        K_Residuo:number = 0,
        Mg_Residuo:number = 0,
        CU_Residuo:number = 0,
        N_Residuo_Percentuale:number = 0,
        P_Residuo_Percentuale:number = 0,
        K_Residuo_Percentuale:number = 0,
        Mg_Residuo_Percentuale:number = 0,
        CU_Residuo_Percentuale:number = 0,
        Dichiarazione_Non_Utilizzo_Trattamenti: boolean = false,
        Dichiarazione_Non_Utilizzo_Fertilizzazioni: boolean = false,
    ) {

        this.PIVA = PIVA;
        this.SA_COD = SA_COD;
        this.APPEZZA = APPEZZA;
        this.ID_REG = ID_REG;
        this.Progetto_Cod = Progetto_Cod;
        this.Progetto_Des = Progetto_Des;
        this.GRFI_COD = GRFI_COD;
        this.Grfi_Des = Grfi_Des;
        this.Cop_Cod = Cop_Cod;
        this.Copertura = Copertura;
        this.Flag_Protetto = Flag_Protetto;
        this.Sup_Imp_help = Sup_Imp_help;
        this.Sup_Riduzione_BufferZone = Sup_Riduzione_BufferZone;
        this.Perc_Riduzione_Deriva = Perc_Riduzione_Deriva;
        this.CUL_COD = CUL_COD;
        this.Cul_Des = Cul_Des;
        this.ListaClassiTessitura = ListaClassiTessitura;
        this.Obj_Disciplinare = Obj_Disciplinare;
        this.Regolamento_Cod = Regolamento_Cod;
        this.Validita_Inizio_Distinta = Validita_Inizio_Distinta;
        this.Validita_Fine_Distinta = Validita_Fine_Distinta;
        this.Data_Raccolta = Data_Raccolta;
        this.Data_Raccolta_Prevista = Data_Raccolta_Prevista;

        this.App_Nome = App_Nome;

        this.CarenzaStr = CarenzaStr;

        this.DataCarenza = DataCarenza;

        this.Stato_Cod = Stato_Cod;

        this.Sup_Imp = Sup_Imp;
        this.DistBZ_CorpiIdrici = DistBZ_CorpiIdrici;
        this.DistBZ_AreeResPub = DistBZ_AreeResPub;
        this.DistBZ_Allevamenti = DistBZ_Allevamenti;
        this.DistBZ_VegNatNonColt = DistBZ_VegNatNonColt;
        this.SupBZ_Riduzione = SupBZ_Riduzione;

        this.flag_N_Max = flag_N_Max;
        this.flag_P_Max = flag_P_Max;
        this.flag_K_Max = flag_K_Max;
        this.flag_Mg_Max = flag_Mg_Max;
        this.flag_CU_Max = flag_CU_Max;

        this.N_Max = N_Max;
        this.P_Max = P_Max;
        this.K_Max = K_Max;
        this.Mg_Max = Mg_Max;
        this.CU_Max = CU_Max;
        this.N_Residuo = N_Residuo;
        this.P_Residuo = P_Residuo;
        this.K_Residuo = K_Residuo;
        this.Mg_Residuo = Mg_Residuo;
        this.CU_Residuo= CU_Residuo;
        this.N_Residuo_Percentuale = N_Residuo_Percentuale;
        this.P_Residuo_Percentuale = P_Residuo_Percentuale;
        this.K_Residuo_Percentuale = K_Residuo_Percentuale;
        this.Mg_Residuo_Percentuale = Mg_Residuo_Percentuale;
        this.CU_Residuo_Percentuale = CU_Residuo_Percentuale;
        this.Dichiarazione_Non_Utilizzo_Fertilizzazioni = Dichiarazione_Non_Utilizzo_Fertilizzazioni;
        this.Dichiarazione_Non_Utilizzo_Trattamenti = Dichiarazione_Non_Utilizzo_Trattamenti;
        this.IrrigazioneUtilizzata_Codice = IrrigazioneUtilizzata_Codice;
        this.IrrigazioneUtilizzata_Descrizione = IrrigazioneUtilizzata_Descrizione;
        this.Consiglio_Descrizione = Consiglio_Descrizione;
        this.Consiglio_Codice = Consiglio_Codice;
        this.DataConsiglio = DataConsiglio;
        this.DoseAcquaConsiglio = DoseAcquaConsiglio;
        this.UdmConsiglio = UdmConsiglio;
        this.minDataTurno = minDataTurno;
        this.maxDataTurno = maxDataTurno;
        this.UdmDose = UdmDose;
        this.DoseAcquaGiornaliera = DoseAcquaGiornaliera;
        this.OreIrrigazione = OreIrrigazione;
        this.Portata = Portata;
        this.Efficienza = Efficienza;
        this.DataInizioIrrigazione = DataInizioIrrigazione;
        this.DataFineIrrigazione = DataFineIrrigazione;
        this.FrequenzaIrrigazioneMedia = FrequenzaIrrigazioneMedia;
        this.tipoIrrigazione = tipoIrrigazione;
        this.macchinaIrrigazione = macchinaIrrigazione;
    }
}

export class GridMacchinaModel{
    Risorsa_Cod: string;
    Risorsa_Des: string;
    Piva: string;
    Mac_Cod: number;
    Taratura_Ugello: number;
    Modello: string;
    Marca: DittaMacchina;
    Data_Scadenza_Taratura: Date;
    Mac_Des: string;
    Tipo: Macchine;
    Dettaglio_1: MacchineDettaglio1;
    Dettaglio_2: MacchineDettaglio2;
    Validita: IntervalloTemporale;

    constructor(Risorsa_Cod: string,Risorsa_Des: string,Piva: string,Mac_Cod: number,Taratura_Ugello: number,
                Modello: string,Marca:DittaMacchina,Data_Scadenza_Taratura: Date,Mac_Des: string,
                Tipo: Macchine,Dettaglio_1: MacchineDettaglio1, Dettaglio_2: MacchineDettaglio2, Validita: IntervalloTemporale){

        this.Risorsa_Cod = Risorsa_Cod;
        this.Risorsa_Des = Risorsa_Des;
        this.Piva = Piva;
        this.Mac_Cod = Mac_Cod;
        this.Taratura_Ugello = Taratura_Ugello;
        this.Modello = Modello;
        this.Marca= Marca;
        this.Data_Scadenza_Taratura = Data_Scadenza_Taratura;
        this.Mac_Des = Mac_Des;
        this.Tipo = Tipo;
        this.Dettaglio_1 = Dettaglio_1;
        this.Dettaglio_2 = Dettaglio_2;
        this.Validita = Validita;
    }
}

export class GridOperatoreModel{
    Piva: string;
    Cod_Contatto: string;
    Cod_RisUm: number;
    Risorsa_Cod: string;
    Risorsa_Des: string;
    Cod_Rapporto: number;
    Rapporto_Des: string;
    Nome: string;
    Cognome: string;
    Data_Scadenza_Patentino: Date;

    constructor(Piva: string,Cod_Contatto: string,Cod_RisUm: number,Risorsa_Cod: string,Risorsa_Des: string,
                Cod_Rapporto: number, Rapporto_Des: string,Nome: string, Cognome: string,Data_Scadenza_Patentino: Date ){
        this.Piva = Piva;
        this.Cod_Contatto = Cod_Contatto;
        this.Cod_RisUm = Cod_RisUm;
        this.Risorsa_Cod = Risorsa_Cod;
        this.Risorsa_Des = Risorsa_Des;
        this.Cod_Rapporto = Cod_Rapporto;
        this.Rapporto_Des = Rapporto_Des;
        this.Nome = Nome;
        this.Cognome = Cognome;
        this.Data_Scadenza_Patentino = Data_Scadenza_Patentino;
    }
}
 //@description:
//Aggiunto questa classe per sapere in fase di salvataggio di una Attività il suo codice e
//se ne sto aggiungendo una nuova in modifica
//La chiave ora è CodiceAttivita, CodiceRicetta, CodiceOperazioneRicetta,Operazione,Centro_Aziendale
//N.B. Il Centro_Aziendale è da valorizzare solamente in modifica di una Attivita
export class CodiciXOperazione  {
    CodiceAttivita: string;
    CodiceRicetta: string;
    CodiceOperazioneRicetta: string;
    Operazione: Lavorazione;
    Associazione_PK: AssociazionePK;
    Centro_Aziendale: CentroAziendale;
    APP_RicettaOperazione_ID: string;
    CodiceAttivitaVisita: string; //Viene Memorizzato l'id agenda a cui ho associato l'operazione che sto facendo
    pua: Pua;

    constructor(CodiceAttivita: string,CodiceRicetta: string,CodiceOperazioneRicetta: string, Operazione:Lavorazione,Associazione_PK: AssociazionePK,Centro_Aziendale: CentroAziendale,APP_RicettaOperazione_ID: string, CodiceAttivitaVisita: string,pua: Pua){
        this.CodiceAttivita = CodiceAttivita;
        this.CodiceRicetta = CodiceRicetta;
        this.CodiceOperazioneRicetta = CodiceOperazioneRicetta;
        this.Operazione = Operazione;
        this.Associazione_PK = Associazione_PK;
        this.Centro_Aziendale = Centro_Aziendale;
        this.APP_RicettaOperazione_ID = APP_RicettaOperazione_ID;
        this.CodiceAttivitaVisita = CodiceAttivitaVisita;
        this.pua = pua;
    }
}

export class SupTrattata_x_DettaglioSemina
{
    Mat_Cod: number;
    Sup_Trattata: number;
}

export class Codici_Attivita_x_CentriAziendali{
    Id_Agenda: number;
    Sa_Cod : number;
    Lav_Cod: number;
    Ricetta_Cod: number;
    Ricetta_Operazione_Cod: number;
}

export class DataCarenzaRaccolta_x_Impianto
{
    KeyImpianto: string; //PIVA | SA_COD | APPEZZA | ID_REG
    App_Nome: string;
    DataCarenza: string;
    CarenzaStr: string;
}


export interface MultiColumnComboboxTrattamento extends MultiColumnComboboxProdotto, DettaglioTrattamento{
    Polverulento_Str: string;
    impollinatore: string;
}

export interface MultiColumnComboboxFertilizzazione extends MultiColumnComboboxProdotto, DettaglioFertilizzazione{
    N_Str: string;
    P_Str: string;
    K_Str: string;
    Cu_Str: string;
    Dichiarato_nel_PUA_Str: string;
}

export interface MultiColumnComboboxSemina extends MultiColumnComboboxProdotto, DettaglioSemina{}

export interface MultiColumnComboboxDose_Etichetta extends MultiColumnComboboxGiasTemplate, DoseEtichetta{}

export interface MultiColumnComboboxProdotto extends MultiColumnComboboxGiasTemplate {
    Tutti_Problemi_DettaglioProdotto: Array<enum_Problema_DettaglioProdotto>;
    Problema_DettaglioProdotto_Da_Risolvere: enum_Problema_DettaglioProdotto;
}

export interface DropdownListDisciplinare extends MultiColumnComboboxGiasTemplate, Disciplinare{}

export interface DropdownListAttivitaPersonalizzata extends MultiColumnComboboxGiasTemplate, AttivitaPersonalizzata{}

export interface DropdownListMagazzino extends MultiColumnComboboxGiasTemplate, Fabbricato{}

export interface DropdownListCampo extends MultiColumnComboboxGiasTemplate, Campo{}

export interface DropdownListAvversita extends MultiColumnComboboxGiasTemplate, AvversitaGruppo,Avversita{}

export interface MultiColumnComboboxAvversitaInnesco extends MultiColumnComboboxGiasTemplate, AvversitaGruppo,Avversita {}

export class Parametri_Aggiuntivi_Attivita{
    operazione: Lavorazione;
    key: string;
    value: string;
}

export class Parametri_Aggiuntivi_ControllaDosi{
    key: string;
    value: string;
}

//@description:
//Parametri Aggiuntivi comuni sia per il salvataggio dell'attivita sia per il controlla_dosi
export enum Key_Parametri_Aggiuntivi{
    lista_Codici_Attivita_x_CentriAziendali = "lista_Codici_Attivita_x_CentriAziendali",
    Formulati_Non_Corretti = "Formulati_Non_Corretti",
    Formulati_Ambigui = "Formulati_Ambigui",
    Avversita_Non_Corrette = "Avversita_Non_Corrette",
    Avversita_Ambigue = "Avversita_Ambigue",
    Avversita_Non_Valorizzate = "Avversita_Non_Valorizzate",
    Id_Visita_Collegata = "Id_Visita_Collegata",
    Fertilizzanti_Non_Corretti = "Fertilizzanti_Non_Corretti",
    Fertilizzanti_Ambigui = "Fertilizzanti_Ambigui",
}

export enum Key_Parametri_Aggiuntivi_Attivita{
    lista_SupTrattata_x_DettaglioSemina = "lista_SupTrattata_x_DettaglioSemina",
    DataCarenzaRaccolta_x_Impianto= "list_DataCarenzaRaccolta_x_Impianto",
    Opzione_Semina = "Opzione_Semina",
    CaricoMagazzinoAutomatico = "CaricoMagazzinoAutomatico",
    lista_MostraWarning = "lista_MostraWarning",
    mostraWarning_CheckListaAttivita = "mostraWarning_CheckListaAttivita",
    mostraWarning_CheckMagazzino = "mostraWarning_CheckMagazzino",
    mostraWarning_ScriviAttivitaToAgenda = "mostraWarning_ScriviAttivitaToAgenda",
    mostraWarning_CheckDPI = "mostraWarning_CheckDPI",
    mostraWarning_InviaRicettaAdAPP = "mostraWarning_InviaRicettaAdAPP",
    mostraWarning_Dichiarazione_non_Utilizzo = "mostraWarning_Dichiarazione_non_Utilizzo",
    IdTestataVerificaConformita = "IdTestataVerificaConformita"
}

export enum Key_Parametri_Aggiuntivi_ControlloDosi
{
    mostraWarning_CheckGiacenza = "mostraWarning_CheckGiacenza",
    mostraWarning_CheckMassimali = "mostraWarning_CheckMassimali",
    mostraWarning_CheckEtichetta = "mostraWarning_CheckEtichetta",
    mostraWarning_CheckProdottoInRibaltamento = "mostraWarning_CheckProdottoInRibaltamento"

}

export enum enum_Problema_DettaglioProdotto{
    Nessuno = 0,
    Formulato_Non_Corretto = 1,
    Formulato_Ambiguo = 2,
    Avversita_Non_Corretta = 3,
    Avversita_Ambigua = 4,
    Avversita_Non_Valorizzata = 5,
    Fertilizzante_Non_Corretto = 6,
    Fertilizzante_Ambiguo = 7
}

export class Obj_Errore_Gias_QdC{
    severity: ErroreGias_Severity;
    result: any;
    list_errori_filtrati: ErroreGias[];
}

export class Voci_Menu{
  text: string;
  value: MenuRicette | CodiciXOperazione;
  disabled: boolean
}

export class Obj_Dose_Hl_Tot{
    Dose_Hl: number;
    DoseTot_Ha: number;
}

export class Obj_Dose_Ha_Tot{
    Dose_Ha: number;
    DoseTot_Ha: number;
}

export class Obj_Dose_Ha_Hl{
    Dose_Ha: number;
    Dose_Hl: number;
}

export class Causale {
    Id: number;
    Causale: string;

    constructor(id: number, causale: string) {
        this.Id = id;
        this.Causale = causale;
    }
}

export class Controlli_Giacenze_Lotti{
  flag_visualizza_solo_prodotti_in_giacenza: {visibile: boolean};
  ddl_UdM: {abilitato: boolean, obbligatorio: boolean, visibile: boolean};
  ddl_Magazzino: {
    abilitato: boolean,
    obbligatorio: boolean,
    visibile: boolean,
  };
  txt_Lotto: {abilitato: boolean, obbligatorio: boolean, visibile: boolean}

  constructor() {
    this.flag_visualizza_solo_prodotti_in_giacenza = {visibile: false};
    this.ddl_UdM = {abilitato: false, obbligatorio: false, visibile: true};
    this.ddl_Magazzino = {abilitato: false, obbligatorio: false, visibile: false};
    this.txt_Lotto = {abilitato: false, obbligatorio: false, visibile: false};
  }
}

export class Controlli_Giacenze_Lotti_X_Elem_Cod{
  Controlli: Controlli_Giacenze_Lotti;
  Elem_Cod: number;

  constructor(elem_Cod: number){
    this.Elem_Cod = elem_Cod;
    this.Controlli = new Controlli_Giacenze_Lotti();
  }
}
