import { FiltroTemporale } from "app/Service/api.service";

export class DateUtils {
  public static calcolaDataInizioSingolaData(dataValidita: Date): Date {
    let dataInizio = new Date(dataValidita);
    dataInizio.setDate(dataInizio.getDate() - 10);
    return dataInizio;
  }

  public static calcolaDataFineSingolaData(dataValidita: Date): Date {
    let dataFine = new Date(dataValidita);
    dataFine.setDate(dataFine.getDate() + 5);
    return dataFine;
  }
}

export class FiltroTemporaleAvanzato {
    public filtroTemporalePeriodo: FiltroTemporale;
    public filtroTemporaleSingolaData: FiltroTemporale;
}
