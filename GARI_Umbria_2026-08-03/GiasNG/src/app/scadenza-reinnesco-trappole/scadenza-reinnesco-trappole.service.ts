import {Injectable} from "@angular/core";
import {FormControl, FormGroup} from "@angular/forms";
import {BehaviorSubject} from "rxjs";
import {CentroItem, SpecieItem} from "gias-kendo-grid";
import {AGRODATAFINE, AGRODATAINIZIO} from "../Model/CostantiPersonalizzate";

@Injectable()

export class FiltriTrappole{
  CentroAziendale: CentroItem;
  Specie: SpecieItem;
  Da: Date;
  A: Date;

  constructor(centroAziendale: CentroItem,specie: SpecieItem) {
    this.CentroAziendale = centroAziendale;
    this.Specie = specie;
    this.Da = AGRODATAINIZIO;
    this.A = AGRODATAFINE;
  }
}

export class ScadenzaReinnescoTrappoleService{

  public FiltriForm: FormGroup = new FormGroup({
    CentroAziendale: new FormControl(''),
    Specie: new FormControl(''),
    Da: new FormControl(''),
    A: new FormControl('')
  });

  private _applicafiltri: BehaviorSubject<boolean> = new BehaviorSubject(false);

  get applicaFiltri(){
    return this._applicafiltri.getValue();
  }

  set applicaFiltri(valore: boolean) {
    this._applicafiltri.next(valore);
  }

  RicaricaGriglia(){
    return this._applicafiltri.asObservable();
  }

  constructor() { }

}
