import {LeggiPrescrizioni as ILeggiPrescrizioni, IntervalloTemporale} from "../../Service/net-core6-api.service";
import {enum_TipoGrigliaTrattamentiZoo, enum_TipoPrescrizione} from "./tipo-prescrizione.enum";
import {TUTTI_CENTRI_AZIENDALI} from "../../Model/CostantiPersonalizzate";

export class LeggiPrescrizioni implements ILeggiPrescrizioni {
  constructor(
    public Piva: string,
    public Sa_Cod: number = TUTTI_CENTRI_AZIENDALI,
    public Sta_Num?: number | null,
    public validita?: IntervalloTemporale | null,
    public Tipo_Cod?: number | null, // enum_TipoPrescrizione
    public Tipo_Griglia?: number | null, // enum_TipoPrescrizione
    public Ricetta_Cod?: number | null
  ) {
  }
}

export class LeggiPrescrizioniIndicazioni extends LeggiPrescrizioni {
  constructor(
    public Piva: string,
    public Sa_Cod: number = TUTTI_CENTRI_AZIENDALI,
    public Sta_Num?: number | null,
    public validita?: IntervalloTemporale | null
  ) {
    super(Piva, Sa_Cod, Sta_Num, validita, enum_TipoPrescrizione.Indicazione_Terapeutica);
  }
}

export class LeggiPrescrizioniProtocolli extends LeggiPrescrizioni {
  constructor(
    public Piva: string,
    public Sa_Cod: number = TUTTI_CENTRI_AZIENDALI,
    public Sta_Num?: number | null,
    public validita?: IntervalloTemporale | null
  ) {
    super(Piva, Sa_Cod, Sta_Num, validita, enum_TipoPrescrizione.Protocollo_Terapeutico);
  }
}

export class LeggiPrescrizioniVeterinarie extends LeggiPrescrizioni {
  constructor(
    public Piva: string,
    public Sa_Cod: number = TUTTI_CENTRI_AZIENDALI,
    public Sta_Num?: number | null,
    public validita?: IntervalloTemporale | null
  ) {
    super(Piva, Sa_Cod, Sta_Num, validita, enum_TipoPrescrizione.UNDEFINED, enum_TipoGrigliaTrattamentiZoo.PrescrizioniVeterinarie);
  }
}
