export enum enum_Ripartizione_Raccolta {
    AUTO_SUPERFICIE,
    AUTO_PIANTE,
    MANUALE // Default
}

export enum enum_Modalita_Raccolta {
    MECCANICA = 0, // Default
    MANUALE = 1
}

/**
 * Non rappresenta i valori assunti dall'impostazione alla base (`UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO` o
 * `Utente_OpzioneChiusuraAbbattimenti`).
 * @see {@link enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO}
 */
export enum enum_Opzioni_Raccolta_Aggiornamento_Anagrafica {
  LASCIA_ATTIVI = 0, // Default per le raccolte
  CHIUDI_ESERCIZI = 1,
  CHIUDI_APRI_ESERCIZI = 4,
  CHIUDI_IMPIANTI_ESERCIZI = 2,
  CHIUDI_APRI_IMPIANTI_ESERCIZI = 5,
  CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI = 3 // default per gli abbattimenti
}

/** Modalità di generazione del lotto nell'operazione di raccolta. Default: `DA_PROGETTO_COD` (3). */
export enum enum_Generazione_Lotto_Raccolta {
    MANUALE,
    DA_DATA,
    UNIVOCO,
    DA_PROGETTO_COD // Default
}

export class OpzioniRaccolta {
    Chiusura: number = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI;
    Ripartizione: number = enum_Ripartizione_Raccolta.AUTO_SUPERFICIE;
    Modalita: number = enum_Modalita_Raccolta.MECCANICA;
    CarichiMagazzinoAttivi: number = 0;
    GenerazioneLotto: number = enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD;
}

/** Set default values in case some of the object's fields are not valorized.
 * Used to avoid passing nulla fields to server side.
 */
export function normalizeOpzioniRaccolta(opzioni: OpzioniRaccolta): void {
    if (!opzioni.Chiusura) opzioni.Chiusura = 0;
    if (!opzioni.Ripartizione) opzioni.Ripartizione = 0;
    if (!opzioni.Modalita) opzioni.Modalita = 0;
    if (!opzioni.CarichiMagazzinoAttivi) opzioni.CarichiMagazzinoAttivi = 0;
    if (!opzioni.GenerazioneLotto) opzioni.GenerazioneLotto = 0;
}
