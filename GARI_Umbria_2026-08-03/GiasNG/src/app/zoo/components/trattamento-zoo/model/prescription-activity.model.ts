import { AssociazionePK, Attivita, Attivita_Stati, Attivita_StatiWorkflowQdC, Attivita_Tipo_Attivita, Attivita_Tipo_Raccolta, Attivita_Tipo_Ricetta, AttivitaPersonalizzata, BaseCodeDescr, Blocco, CentroAziendale, CentroDiCosto, CentroDiCosto_CodeType, Disciplinare, Documento, Epoca, Job, NoteIntervento, RegistrazioneContabile, Risorsa, TestataRicetta, Tipo, UtilizzoTerreno } from "app/Service/net-core6-api.service";

export class PrescriptionActivity implements Attivita {
  /** Agenda.Id_Agenda o Ricette_Operazioni.Ricetta_Operazione_Cod */
  codice?: string | null;
  /** non usare per comporre il des_lib */
  descrizione?: string | null;
  /** Memorizzata due volte su db: Agenda.Validita_Inizio e nei record Movimenti.Data_Movimento */
  inizio?: Date;
  /** Agenda.Validita_Fine su db (al momento non è gestita in AgronicaCoreModello) 'TODO: approfondire con Vanni */
  fine?: Date;
  oraInizio?: Date;
  oraFine?: Date;
  centroAziendale?: CentroAziendale;
  fabbricatoCod?: number;
  centriDiCosto?: CentroDiCosto[] | null = [];
  risorse?: Risorsa[] | null = [];
  documenti?: Documento[] | null = [];
  disciplinare?: Disciplinare;
  utilizzoTerreno?: UtilizzoTerreno;
  registrazioneContabile?: RegistrazioneContabile;
  raccoglitore?: number;
  epoca?: Epoca;
  tipoRaccolta?: Attivita_Tipo_Raccolta;
  noteIntervento?: NoteIntervento[] | null = [];
  job?: Job;
  tipo?: Attivita_Tipo_Attivita;
  tipoRicetta?: Attivita_Tipo_Ricetta;
  stato?: Attivita_Stati;
  statoWorkflow?: Attivita_StatiWorkflowQdC;
  attivitaCollegate?: Attivita[] | null = [];
  /** Movimenti.mov_desc */
  note?: string | null;
  latitude?: number;
  longitude?: number;
  guid?: string | null;
  cancellato?: boolean;
  daRemoto?: boolean;
  /** Movimenti.modalita */
  modalita?: number;
  attivitaPersonalizzata?: AttivitaPersonalizzata;
  testataRicetta?: TestataRicetta;
  associazionePK?: AssociazionePK;
  /** Corrisponde alla campo "Invia_App" di Ricette_Operazioni */
  inviaRicetta?: boolean;
  codiceOperazioneRicetta?: string | null;
  /** Corrisponde alla campo "Origine" di Agenda/Ricetta */
  origine?: string | null;
  /** Corrisponde alla campo "APP_Ricetta_Operazione_ID" di Ricette_Operazioni */
  appRicettaOperazioneID?: string | null;
  blocco?: Blocco;
  modalitaApplicazione?: BaseCodeDescr;
}
