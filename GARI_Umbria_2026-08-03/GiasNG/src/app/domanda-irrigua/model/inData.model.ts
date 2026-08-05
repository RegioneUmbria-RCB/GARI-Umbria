import {
  IntervalloTemporale as IIntervalloTemporale,
  LeggiContatori_IN as ILeggiContatori_IN,
  LettureContatori_IN,
  ScriviLetturaContatore as IScriviLetturaContatore
} from "../../Service/net-core6-api.service";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";

export namespace InData.DSS {

  export class Letture_IN implements LettureContatori_IN {
    constructor(
      public Piva: string = "",
      public PeriodoLettura: IIntervalloTemporale = new IntervalloTemporale()
    ) { }
  }

  export class LeggiContatori_IN implements ILeggiContatori_IN {
    constructor(
      public Piva: string = "",
      public DataLettura?: Date// = new Date()
    ) { }
  }

  export class ScriviLetturaContatore implements IScriviLetturaContatore {
    constructor(
      public Id_Lettura?: number,
      public Piva?: string | null,
      public Id_Contatore?: number,
      public Valore_Lettura?: number,
      public Data_Lettura?: Date,
      public Flag_Cancellazione?: boolean,
    ) { }
  }

}
