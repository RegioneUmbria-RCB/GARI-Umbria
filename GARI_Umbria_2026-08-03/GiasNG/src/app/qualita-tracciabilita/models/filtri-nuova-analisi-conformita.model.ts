import { FormControl } from '@angular/forms';
import { OperationTypes } from "./operation-types.enum";
import { LeggiAgendaHubQDCNew } from "app/Service/qdca-compliance-api.service";
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

const currentYear = new Date().getFullYear();
const firstDayOfYear = new Date(currentYear, 0, 1);
const lastDayOfYear = new Date(currentYear, 11, 31);

export class FiltriAnalisiConformita {
  data_a: Date = firstDayOfYear;
  data_da: Date = lastDayOfYear;
  data_richiesta: Date = new Date();

  piva: string = "";
  sa_cod: number[] = [];
  veg_cod: number[] = [];
  impianti: string[] = [];
  operazioni: number[] = [];
  dpi: string = "";

  flagIaf: boolean = false;
  flagImpostazioni: boolean = true;
  flagNormative: boolean = true;
  flagMagazzino: boolean = false;

  public static toLeggiAgendaHubQDCNew(toParse: FiltriAnalisiConformita): LeggiAgendaHubQDCNew {
    return {
      piva: toParse.piva ?? '',
      saCod: toParse.sa_cod ?? [0],
      specieVegetale: toParse.veg_cod ?? [0],
      operazioni: toParse.operazioni ?? [0],
      disciplinare: { codice: toParse.dpi ?? '' },
      intervallo: new IntervalloTemporale(toParse.data_da, toParse.data_a),
      verificaIAF: toParse.flagIaf ?? false,
      verificaSoloControlliUtente: toParse.flagImpostazioni ?? false,
      verificaMagazzino: toParse.flagMagazzino ?? false,
      verificaNormative: toParse.flagNormative ?? false
    };
  }
}

export class FiltriAnalisiConformitaDes extends FiltriAnalisiConformita {
  rag_soc: string = "";
  sa_nome: string = "";
  veg_des: string = "";
  impianti_des: string = "";
  operazioni_des: string = "";
  dpi_des: string = "";
}

export class FiltriAnalisiConformitaForm {
  data_a: FormControl<Date>;
  data_da: FormControl<Date>;

  piva: FormControl<string>;
  sa_cod: FormControl<number[]>;
  veg_cod: FormControl<number[]>;
  impianti: FormControl<string[]>;
  operazioni: FormControl<number[]>;
  dpi: FormControl<string>;

  flagIaf: FormControl<boolean>;
  flagImpostazioni: FormControl<boolean>;
  flagMagazzino: FormControl<boolean>;
  flagNormative: FormControl<boolean>;

  public constructor(init?: Partial<FiltriAnalisiConformita>) {
    this.data_da = new FormControl(init?.data_da ?? firstDayOfYear);
    this.data_a = new FormControl(init?.data_a ?? lastDayOfYear);
    this.piva = new FormControl(init?.piva ?? "");
    this.dpi = new FormControl(init?.dpi ?? "");
    this.flagIaf = new FormControl(init?.flagIaf ?? false);
    this.flagImpostazioni = new FormControl(init?.flagImpostazioni ?? false);
    this.flagMagazzino = new FormControl(init?.flagMagazzino ?? false);
    this.flagNormative = new FormControl(init?.flagNormative ?? true);

    this.sa_cod = new FormControl(init?.sa_cod ?? []);
    this.veg_cod = new FormControl(init?.veg_cod ?? []);
    this.impianti = new FormControl(init?.impianti ?? []);
    this.operazioni = new FormControl(init?.operazioni ?? [OperationTypes.All]);
  }
}