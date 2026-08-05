import { ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {FormControl} from "@angular/forms";
import {IntervalloTemporale as IIntervalloTemporale} from "../../Service/net-core6-api.service";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";
import {BaseCodeDescr as IBaseCodeDescr} from "../../Service/api.service";
import {WaterCounter} from "./WaterCounter";

export class LetturaContatoriGridItem {
  constructor (
    public Id_Lettura: number,
    public Id_Contatore: number,
    public Descrizione: string,
    public Targa: string,
    public Modello: string,
    public ValoreMC: number,
    public DataLettura: Date,
    public Validita_Inizio: Date,
    public Validita_Fine: Date,
    public piva: string
  ) { }
}

export class LetturaContatoriGridItemModel {
  public Id_Lettura = new ModelEntry(CELL_TYPES.NUMBER);
  public Id_Contatore = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
  public Descrizione = new ModelEntry(CELL_TYPES.STRING);
  public Modello = new ModelEntry(CELL_TYPES.STRING);
  public Targa = new ModelEntry(CELL_TYPES.STRING);
  public ValoreMC = new ModelEntry(CELL_TYPES.NUMBER);
  public DataLettura = new ModelEntry(CELL_TYPES.DATETIME);
  public Validita_Inizio = new ModelEntry(CELL_TYPES.DATE);
  public Validita_Fine = new ModelEntry(CELL_TYPES.DATE);
  public piva = new ModelEntry(CELL_TYPES.STRING);
}

export class LetturaContatoriGridItemForm {
  DataLettura = new FormControl(new Date());
  Descrizione = new FormControl('');
  Id_Contatore = new FormControl(0);
  Id_Lettura = new FormControl(0);
  ValoreMC = new FormControl(0);
  Validita_Inizio = new FormControl(new Date());
  Validita_Fine = new FormControl(new Date());
  piva = new FormControl('');
}

export interface ILetturaContatori {
  Id_Lettura: number;
  contatore: IBaseCodeDescr;
  dataLettura: Date;
  valoreLetto: number;
  validita: IIntervalloTemporale;
  piva: string;
}

export class LetturaContatoriModel implements ILetturaContatori {
  public Id_Lettura: number;
  public contatore: WaterCounter;
  public dataLettura: Date;
  public valoreLetto: number;
  public validita: IIntervalloTemporale;
  public piva: string;

  public fromFlatGridItem(gridItem: LetturaContatoriGridItem): LetturaContatoriModel {
    this.Id_Lettura = gridItem.Id_Lettura;
    this.dataLettura = gridItem.DataLettura;
    this.valoreLetto = gridItem.ValoreMC;
    this.piva = gridItem.piva;
    this.validita = new IntervalloTemporale(gridItem.Validita_Inizio, gridItem.Validita_Fine);
    this.contatore = new WaterCounter(gridItem.Id_Contatore, gridItem.Descrizione);
    return this;
  }

  public toFlatGridItem(): LetturaContatoriGridItem {
    return new LetturaContatoriGridItem(
      this.Id_Lettura,
      this.contatore.codice,
      this.contatore.descrizione,
      this.contatore.modello,
      this.contatore.targa,
      this.valoreLetto,
      this.dataLettura,
      this.validita.inizio,
      this.validita.fine,
      this.piva
    );
  }
}
