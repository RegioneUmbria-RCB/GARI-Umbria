import {Injectable, Injector} from "@angular/core";
import {
  AbstractGridConfigService,
  HttpAction
} from 'gias-kendo-grid';
import {
  GridCategorieMagazzinoServerResult
} from "../settings-categorie-magazzino/grid-settings-categorie-magazzino.service";
import { CELL_TYPES } from 'gias-ui-kit';
import { DropdownListItem, DropdownListWithForm,
  EditingMode, KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {forkJoin, map, Observable, of, take} from "rxjs";
import {
  CommandsColumnSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import {AjaxAgronicaAPIService} from "../../../../Service/ajax-agronica.api.service";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import {AbstractControl, FormArray, FormGroup, ValidationErrors, Validators} from "@angular/forms";
import {enum_Impostazioni_Utenti} from "../../../../Model/Impostazioni_Utenti.enum";
import {BaseCodeDescrVal} from "../../../../Model/baseClass/baseCodeDescrVal";
import {ImpostazioniUtentiService} from "../../../services/impostazioni/impostazioni-utenti.service";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";

/** Areas already with a filter set. Used in the validator `uniqueInstance` */
const added: number[] = [];
/** Used to keep track of the grid status. Only the first value is used. Used in the validator `uniqueInstance` */
const isEdit = [true];

@Injectable()
export class GridFiltroSqlMateriePrimeService extends AbstractGridConfigService<GridCategorieMagazzinoServerResult> {
  editingMode = EditingMode.IN_CELL;
  gridId = "gridSqlMatPrime";
  loader = LoaderType.SERVICE;
  rowId = "codice";

  /** FormGroup describing the setting. */
  private innerForm: FormGroup;
  private kendoRows = [];
  private kCols = [
    new KendoGridColumn({ field:'codice', title:this.transloco.translate('AreaGias')}, {editable: false}),
    new KendoGridColumn({ field:'valore', title:this.transloco.translate('Filtro')}, {editable: true}),
  ];
  private kModel = {
    codice: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    descrizione: new ModelEntry(CELL_TYPES.STRING),
    valore: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(injector: Injector,
    private datashare: ProfilazioneDataShareService,
    private impostazioniService: ImpostazioniUtentiService,
    private api: AjaxAgronicaAPIService,
  ) {
    super(injector);
    this.handleCustomizations();
    this.gridPublicService.changeDetected.GiasSubscribe((event: any) => this.handleEvents(event));
    this.addValidators();
  }

  private get valueStr(): string {
    if (this.kendoRows.length) {
      return this.kendoRows
        .map((row: BaseCodeDescrVal) => row.codice + "♪" +  row.valore)
        .reduce((acc: string, b: string) => acc + "♫" + b);
    }
    return "";
  }

  perform(actionType: HttpAction, items: any, oldRow: any): Observable<any> {
    return null;
  }

  read(options: any): Observable<GridCategorieMagazzinoServerResult> {
    if (!this.innerForm) {
      this.innerForm = this.datashare.formsMap.get(enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime);
    }
    const obs: Observable<any>[] = [];
    obs.push(this.loadGiasAreas());
    obs.push(this.impostazioniService.leggiDatiFiltroMono(
      '', enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime
    ));
    return forkJoin(obs).pipe(map(results => {
      if (this.innerForm.untouched) {
        this.kendoRows = results[1];
        this.patchAreasDescriptions(this.kendoRows);
        this.kendoRows.map(r => r.codice).forEach(c => added.push(c));
      }
      return new GridCategorieMagazzinoServerResult(this.kendoRows, this.kCols, this.kModel);
    }));
  }

  onCellClose = () => this.updateForm();

  private patchAreasDescriptions(rows: BaseCodeDescrVal[]) {
    const col = this.kCols.find(s => s.field === 'codice');
    this.loadGiasAreas().pipe(take(1))
      .subscribe(() => rows.forEach(v =>
        v.descrizione = col.ddl.data.find(d => d.id === v.codice).name ?? ""
      ));
  }

  private handleCustomizations() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
    });
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.behavior.excelSettings.enabled = true;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
  }

  private addValidators() {
    this.kCols.find(c => c.field === "codice").validators = [
      Validators.required,
      this.uniqueInstance
    ];
  }

  private uniqueInstance(control: AbstractControl): ValidationErrors | null {
    if (!isEdit[0] && control.value && added.find(x => x === control.value))
      return {"duplicateKey": true};
    return null;
  }

  /**
   * @return an `Observable` indicating that the areas have been loaded.
   * @private
   */
  private loadGiasAreas(): Observable<boolean> {
    const col = this.kCols.find(s => s.field === 'codice');
    if (col.ddl) {
      return of(true);
    }
    return this.api.ajaxAPIGet<any, BaseCodeDescr[]>("/MetaschemaNG/CaricaAreeGIAS", "")
      .pipe(take(1), map(R => {
        let data = R.RispostaStringa.map(tipo => new DropdownListItem(tipo.codice, tipo.descrizione));
        col.ddl = new DropdownListWithForm('codice', 'codice', 'descrizione', data);
        col.ddl.descriptionField = 'descrizione';
        return true;
      }));
  }

  private handleEvents(event: any) {
    switch (event.action) {
      case 'add': // pulsante nuova riga
        this.kCols.find(s => s.field === 'codice').editable = true;
        isEdit[0] = false;
        break;
      case 'save': // salva nuova riga
        this.add(event.dataItem);
      case 'cancel':
        this.kCols.find(s => s.field === 'codice').editable = false;
        isEdit[0] = true;
        break;
      case 'remove':
        this.remove(event.dataItem);
        break;
      default:
        console.log(event);
    }
  }

  private add(dataItem: BaseCodeDescrVal) {
    this.kendoRows.push(dataItem);
    added.push(dataItem.codice);
    this.updateForm();
    this.refresh(this.kendoRows);
  }

  private remove(dataItem: BaseCodeDescrVal) {
    let i = this.kendoRows.findIndex(r => r.codice === dataItem.codice);
    this.kendoRows.splice(i, 1);
    i = added.findIndex(x => x === dataItem.codice);
    added.splice(i, 1);
    this.updateForm();
    this.refresh(this.kendoRows);
  }

  private updateForm() {
    this.innerForm.get("valoreCorrente").patchValue(this.valueStr);
    this.innerForm.markAsTouched();
  }

  private refresh(newRows: BaseCodeDescrVal[]) {
    this.gridPublicService.refresh(true, new GridCategorieMagazzinoServerResult(
      newRows, this.kCols, this.kModel
    ));
  }

}
