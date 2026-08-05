import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { KendoGridModel, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { Impostazione } from 'gias-ui-kit';

export enum enum_TipoDato {
    STRINGA = 'text',
    NUMERO = 'number',
    DATA = 'date',
    RADIO = 'radiobutton',
    CHECKBOX = 'checkbox',
    DDL = 'ddl',
    MULTISELECT = 'multi',

    ANNATA_AGRARIA = 'annataAgraria',
    SPECIE_VARIETA = 'specieVarieta',
    CATEGORIE_MAGAZZINO = 'categorieMagazzino',
    PROXY = 'proxy',
    FILTRO_SQL_MATERIE_PRIME = 'filtroMateriePrime',
    FILTRO_GIACENZE_CATEGORIE_MAGAZZINO = 'filtroGiacenze',
    FILTRO_LOTTI_CATEGORIE_MAGAZZINO = 'filtroLotti',
    STAMPA_DOCUMENTI_CONTABILI = 'stampaDocContab',
}

export const FILTRO_CATEGORIE_MAGAZZINO = 'Elem_Cod != 500 AND Elem_Cod != 400';
export const FILTRO_CATEGORIE_MAGAZZINO_APP = 'Elem_Cod IN (3, 191, 196, 197, 198, 10)';
export const CATEGORIE_MAGAZZINO_POSITIVE = 'Elem_Cod > 0';


export enum enum_LayoutFormatiStampaDoc {
    STANDARD,
    PESO,
    RISCONTRO_TOTALE,
    RISCONTRO,
    PREZZO
}

export class settingsFieldData {
    Data_Creazione: Date;
    Data_Modifica: Date;
    Data_Invio: Date;

    Impostazione_Cod: number;
    Impostazione_Des: string = "";
    Impostazione_SuperUser: number;
    Impostazione_AziendaCentro: number;
    Impostazione_AziendaCentroSpecie: number;
    Flag_InApp: number;

    Inviato: number;
    Note: string;
    Ordine: number;
    Tipo_Campo: string;
    Valore_Default: string;

    Sezione_Cod: number;
    Sezione_Des: string = "";
    SottoSezione_Cod: number;
    SottoSezione_Des: string = "";
    Livello_Cod: number;
    Livello_Des: string = "";

    Username_Creazione: string;
    Username_Modifica: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
}

/** Indica l'area d'uso del componente SectionImpostazioniUtenteComponent. */
export enum enum_PaginaImpostazioni {
    UNDEFINED = '',
    UTENTI = 'Impostazioni-Utente',
    SUPERUSER = 'Impostazioni-SuperUser',
    AZIENDE_CENTRI = 'Impostazioni-Aziende-Centri',
    AZIENDE_CENTRI_SPECIE_VARIETA = 'Impostazioni-Aziende-Centri-Specie',
}

export enum enum_LivelloImpostazione {
    SEZIONE = 0,
    SOTTOSEZIONE = 1,
    LIVELLO = 2,
}

export class AziendeCentriGridModel extends KendoGridModel {
    Attivo = new ModelEntry(CELL_TYPES.STRING);
    Cap = new ModelEntry(CELL_TYPES.STRING);
    chiave = new ModelEntry(CELL_TYPES.STRING);
    Codice_Cuaa = new ModelEntry(CELL_TYPES.STRING);
    Codice_Fiscale = new ModelEntry(CELL_TYPES.STRING);
    Codice_Socio = new ModelEntry(CELL_TYPES.STRING);
    Com = new ModelEntry(CELL_TYPES.STRING);
    Com_Cod_Istat = new ModelEntry(CELL_TYPES.STRING);
    Contratto_Produzione = new ModelEntry(CELL_TYPES.STRING);
    Cooperativa_Referente = new ModelEntry(CELL_TYPES.STRING);
    Data_Creazione = new ModelEntry(CELL_TYPES.STRING);
    Data_Modifica = new ModelEntry(CELL_TYPES.STRING);
    frz_des = new ModelEntry(CELL_TYPES.STRING);
    ind_des = new ModelEntry(CELL_TYPES.STRING);
    Indirizzo = new ModelEntry(CELL_TYPES.STRING);
    Num_Padri = new ModelEntry(CELL_TYPES.STRING);
    piva = new ModelEntry(CELL_TYPES.STRING);
    Piva_Padre = new ModelEntry(CELL_TYPES.STRING);
    Pro_Cod_Istat = new ModelEntry(CELL_TYPES.STRING);
    Prov = new ModelEntry(CELL_TYPES.STRING);
    Provincia = new ModelEntry(CELL_TYPES.STRING);
    rag_soc = new ModelEntry(CELL_TYPES.STRING);
    Rag_Soc_Padre = new ModelEntry(CELL_TYPES.STRING);
    SAU_Totale = new ModelEntry(CELL_TYPES.STRING);
    Sa_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Sa_Nome = new ModelEntry(CELL_TYPES.STRING);
    Stato = new ModelEntry(CELL_TYPES.STRING);
    Stato_Cod = new ModelEntry(CELL_TYPES.STRING);
    Superficie_Tare = new ModelEntry(CELL_TYPES.STRING);
    Superficie_Totale = new ModelEntry(CELL_TYPES.STRING);
    Tecnico_Referente = new ModelEntry(CELL_TYPES.STRING);
    Utente_Creazione = new ModelEntry(CELL_TYPES.STRING);
    Utente_Modifica = new ModelEntry(CELL_TYPES.STRING);
    Validita_Inizio = new ModelEntry(CELL_TYPES.STRING);
    Validita_Fine = new ModelEntry(CELL_TYPES.STRING);

    Specie_Cod = new ModelEntry(CELL_TYPES.STRING);
    Specie_Des = new ModelEntry(CELL_TYPES.STRING);
    Cultivar_Cod = new ModelEntry(CELL_TYPES.STRING);
    Cultivar_Des = new ModelEntry(CELL_TYPES.STRING);
}

export class AziendaCentro {
    Attivo: number;
    Cap: string;
    chiave: string;
    Codice_Cuaa: string;
    Codice_Fiscale: string;
    Codice_Socio: string;
    Com: string;
    Com_Cod_Istat: string;
    Contratto_Produzione: string;
    Cooperativa_Referente: string;
    Data_Creazione: string;
    Data_Modifica: string;
    frz_des: string;
    ind_des: string;
    Indirizzo: string;
    Num_Padri: string;
    numeroCentri: number;
    Piva: string;
    Piva_Padre: string;
    Pro_Cod_Istat: string;
    Prov: string;
    Provincia: string;
    rag_soc: string;
    Rag_Soc_Padre: string;
    SAU_Totale: string;
    Sa_Cod: number;
    Sa_Nome: string;
    Stato: string;
    Stato_Cod: string;
    Superficie_Tare: string;
    Superficie_Totale: string;
    Tecnico_Referente: string;
    Utente_Creazione: string;
    Utente_Modifica: string;
    Validita_Inizio: string;
    Validita_Fine: string;

    Specie_Cod: number;
    Specie_Des: string;
    Cultivar_Cod: number;
    Cultivar_Des: string;

    constructor(piva?: string, Sa_Cod?: number) {
        this.Piva = piva;
        this.Sa_Cod = Sa_Cod;
    }

    get piva(): string {
        return this.Piva;
    }

    fromObject(obj: any) {
        this.Attivo = obj.Attivo;
        this.Cap = obj.Cap;
        this.chiave = obj.chiave;
        this.Codice_Cuaa = obj.Codice_Cuaa;
        this.Codice_Fiscale = obj.Codice_Fiscale;
        this.Codice_Socio = obj.Codice_Socio;
        this.Com = obj.Com;
        this.Com_Cod_Istat = obj.Com_Cod_Istat;
        this.Contratto_Produzione = obj.Contratto_Produzione;
        this.Cooperativa_Referente = obj.Cooperativa_Referente;
        this.Data_Creazione = obj.Data_Creazione;
        this.Data_Modifica = obj.Data_Modifica;
        this.frz_des = obj.frz_des;
        this.ind_des = obj.ind_des;
        this.Indirizzo = obj.Indirizzo;
        this.Num_Padri = obj.Num_Padri;
        this.numeroCentri = obj.numeroCentri;
        this.Piva = obj.piva ?? obj.Piva;
        this.Piva_Padre = obj.Piva_Padre;
        this.Pro_Cod_Istat = obj.Pro_Cod_Istat;
        this.Prov = obj.Prov;
        this.Provincia = obj.Provincia;
        this.rag_soc = obj.rag_soc;
        this.Rag_Soc_Padre = obj.Rag_Soc_Padre;
        this.SAU_Totale = obj.SAU_Totale;
        this.Sa_Cod = !obj.Sa_Cod ? 0 : obj.Sa_Cod;
        this.Sa_Nome = !obj.Sa_Nome ? 'Tutti i Centri' : obj.Sa_Nome;
        this.Stato = obj.Stato;
        this.Stato_Cod = obj.Stato_Cod;
        this.Superficie_Tare = obj.Superficie_Tare;
        this.Superficie_Totale = obj.Superficie_Totale;
        this.Tecnico_Referente = obj.Tecnico_Referente;
        this.Utente_Creazione = obj.Utente_Creazione;
        this.Utente_Modifica = obj.Utente_Modifica;
        this.Validita_Inizio = obj.Validita_Inizio;
        this.Validita_Fine = obj.Validita_Fine;
        this.Specie_Cod = obj.Specie_Cod;
        this.Specie_Des = !obj.Specie_Des ? 'Tutte le Specie' : obj.Specie_Des;
        this.Cultivar_Cod = obj.Cultivar_Cod;
        this.Cultivar_Des = !obj.Cultivar_Des ? 'Tutte le Varietà' : obj.Cultivar_Des;
    }
}

export interface Piva_Sa_Cod {
    piva: string;
    rag_soc: string;
    sa_cod: number;
    sa_nome: string;
}

export interface Azienda_Centro_Specie_Varieta {
    piva: string;
    Sa_Cod: number;
    Specie_Cod: number;
    Cultivar_Cod: number;
}
