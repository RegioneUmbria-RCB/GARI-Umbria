export interface DatiModifica {
  /** Username dell'utente che ha creato l'oggetto. */
  UsernameCreazione: string;
  /** Data di creazione. */
  DataCreazione: Date;
  /** Username dell'utente che ha effettuato l'ultima modifica. */
  UsernameModifica: string;
  /** Data dell'ultima modifica. */
  DataModifica: Date;
}

/** @inheritDoc */
export class DatiModificaStandard implements DatiModifica {
  UsernameCreazione: string;
  DataCreazione: Date;
  UsernameModifica: string;
  DataModifica: Date;

  public init(
    UsernameCreazione: string, DataCreazione: Date,
    UsernameModifica: string, DataModifica: Date
  ) {
    this.UsernameCreazione = UsernameCreazione;
    this.DataCreazione = DataCreazione;
    this.UsernameModifica = UsernameModifica;
    this.DataModifica = DataModifica;
  }
}

