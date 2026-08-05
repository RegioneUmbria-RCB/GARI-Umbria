import { CentroAziendale } from '../anagrafiche/CentroAziendale';
import { CentroDiCosto } from './centri_di_costo/CentroDiCosto';
import { Risorsa } from './risorse/Risorsa';
import { Disciplinare } from "../metaschema/Disciplinari";
import { Epoca } from "../metaschema/Epoca";
import { NoteIntervento } from "./note_intervento/NoteIntervento";
import { Job } from './Job';
import { UtilizzoTerreno } from '../metaschema/utilizzi/UtilizzoTerreno';
import { AttivitaPersonalizzata } from './AttivitaPersonalizzata';
import {Documento} from "../documenti/Documento";
import {TestataRicetta} from "./TestataRicetta";
import {RegistrazioneContabile} from "./contabilita/RegistrazioneContabile";
import {AssociazionePK} from "./AssociazionePK";
import { enum_statiWorkflowQdC } from '../TipiEnumerativi';
import {Blocco} from "./Blocco";
import {BaseCodeDescr} from "../baseClass/baseCodeDescr";
import { Stati, Tipo_Attivita } from 'gias-ui-kit';

export class Attivita {
    // Agenda.Id_Agenda o Ricette_Operazioni.Ricetta_Operazione_Cod
    codice: string;
    descrizione: string;
    inizio: Date;
    fine: Date;
    oraInizio: Date;        //Aggiunta per le Visite
    oraFine: Date;          //Aggiunta per le Visite
    daRemoto: boolean;      //Aggiunta per le Visite
    statoWorkflow: enum_statiWorkflowQdC; //Aggiunta per le Visite
    attivitaCollegate: Attivita[];
    centroAziendale: CentroAziendale;
    centriDiCosto: CentroDiCosto[];
    risorse: Risorsa[];
    documenti: Documento[];
    disciplinare: Disciplinare;
    utilizzoTerreno: UtilizzoTerreno;
    registrazioneContabile: RegistrazioneContabile;
    raccoglitore: number;
    epoca: Epoca;
    tipoRaccolta: Tipo_Raccolta;
    noteIntervento: NoteIntervento[];
    job: Job;
    tipo: Tipo_Attivita;
    tipoRicetta: Tipo_Ricetta;
    stato: Stati;
    note: string;
    latitude: number;
    longitude: number;
    guid: string;
    modalita: number;
    attivitaPersonalizzata: AttivitaPersonalizzata;
    testataRicetta: TestataRicetta;
    associazionePK: AssociazionePK;
    inviaRicetta: boolean;
    origine: string;
    appRicettaOperazioneID: string;
    blocco: Blocco;
    modalitaApplicazione: BaseCodeDescr;
}

export enum enum_Tipo_Operazione_Agenda_Target {
    Not_Set = 0,
    Reale = 1,
    Planning = 2
}

export enum Tipo_Ricetta
{
    Standard = 0,
    Costi = 1,
    PUA = 2,
    Budget_Globale = 3,
    Budget_Utente = 4,
    Standard_Destinazioni = 5,
    PianoDistribuzioneConcimi = 6,
    ControlloDiGestione = 7,
    Standard_Destinazioni_Planning = 8,
    PianoDistribuzionePua = 9,
    RichiestaUMA = 10
}

export enum Tipo_Raccolta {
    Fast = 10, //data e impianti
    Leggera = 20, //data, impianti e qta prodotto
    Leggera_Con_Dettagli_Magazzino = 30 //data, impianti, qta prodotto e carico magazzino
}
