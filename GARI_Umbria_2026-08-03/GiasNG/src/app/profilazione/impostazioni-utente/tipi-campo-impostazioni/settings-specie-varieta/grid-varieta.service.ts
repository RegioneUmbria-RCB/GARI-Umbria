import { Injectable, Injector } from "@angular/core";
import { ImpostazioniFormService } from "../../../services/impostazioni/impostazioni-form.service";
import { CommandsColumnSettings } from 'gias-kendo-grid';
import {
  KendoServerResult,
  KendoGridRow,
  KendoGridColumn,
  KendoGridModel,
  EditingMode,
  LoaderType,
  ModelEntry,
  DropdownListItem, DropdownListWithForm
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  AbstractGridConfigService,
  HttpAction
} from 'gias-kendo-grid';
import { BehaviorSubject, filter, forkJoin, from, map, Observable, take, takeUntil } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { enum_Impostazioni_Utenti } from "../../../../Model/Impostazioni_Utenti.enum";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { AbstractControl, FormGroup, ValidationErrors, Validators } from "@angular/forms";
import { SpecieVegetaliService } from "../../../../Service/Metaschema/specie-vegetali.service";
import { VarietaService } from "../../../../Service/Metaschema/varieta.service";
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import {
  GridCategorieMagazzinoServerResult
} from "../settings-categorie-magazzino/grid-settings-categorie-magazzino.service";
import { BaseCodeDescr } from "../../../../Service/api.service";
import { ImpostazioniUtentiService } from "app/profilazione/services/impostazioni/impostazioni-utenti.service";

/** Used to keep track of the grid status. Only the first value is used. Used in the validator `uniqueInstance` */
const added: Set<string> = new Set<string>();

export class GridVarietaServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridVarietaService extends AbstractGridConfigService<GridVarietaServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'chiave';
  gridId = 'gridVarietaID';

  // private loaded = new BehaviorSubject<boolean>(null);
  /** FormGroup describing the setting. */
  private innerForm: FormGroup;
  private kendoRows: any[] = [];
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Veg_Cod', title: this.translocoService.translate('SpecieVegetale') }),
    new KendoGridColumn({ field: 'Cul_Cod', title: this.translocoService.translate('Varietà') })
  ];
  private kendoModel: KendoGridModel = {
    Veg_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING),
    Cul_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Cul_Des: new ModelEntry(CELL_TYPES.STRING),
    chiave: new ModelEntry(CELL_TYPES.STRING)
  };

  constructor(
    injector: Injector,
    private formImpostazioniService: ImpostazioniFormService,
    private impostazioniService: ImpostazioniUtentiService,
    private specieService: SpecieVegetaliService,
    private cultivarService: VarietaService,
    private datashare: ProfilazioneDataShareService,
    private translocoService: TranslocoService
  ) {
    super(injector);
    this.handleCustomization();
    this.setDdlLoadingFunctions();
    this.kendoColumns.find(c => c.field === "Cul_Cod").validators = [
      Validators.required,
      this.uniqueInstance
    ];
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((event: any) => this.handleEvents(event));
    this.refreshOnSpeciesChange();
  }

  private get username(): string {
    if (this.formImpostazioniService.utentiSelezionati.length > 0) {
      return this.formImpostazioniService.utentiSelezionati[0];
    }
    return '';
  }

  read(options?: any): Observable<GridVarietaServerResult> {
    if (!this.innerForm) {
      this.innerForm = this.datashare.formsMap.get(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA);
    }

    const obs: Observable<any>[] = [];
    // obs.push(this.loaded.pipe(filter(x => !!x), take(1)));
    obs.push(this.impostazioniService.leggiDatiFiltroMono(
      this.username, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA
    ).pipe(take(1), map((vars) => vars.map(v => ({
      Cul_Cod: v.codice,
      Cul_Des: v.descrizione,
      Veg_Cod: v.specie.codice,
      Veg_Des: v.specie.descrizione,
      chiave: v.specie.codice + "_" + v.codice
    })))));
    return forkJoin(obs).pipe(
      map(results => results[0]),
      map(vars => {
        if (this.innerForm.untouched) {
          this.kendoRows = vars;
        }
        this.kendoRows.forEach(r => added.add(r.chiave));
        this.model = this.kendoModel;
        this.columns = this.kendoColumns;
        return new GridVarietaServerResult(this.kendoRows, this.kendoColumns, this.kendoModel);
      })
    );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return null;
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
      width: 30
    });
    this.toolbar.newItem = true;
    this.behavior.excelSettings.enabled = true;
    this.behavior.pdfSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
  }

  private setDdlLoadingFunctions() {
    this.setSpecieDdl();
    this.setVarietaDdl();
  }

  private setSpecieDdl() {
    const vegCol = this.kendoColumns.find(s => s.field === 'Veg_Cod');
    vegCol.ddl = new DropdownListWithForm('codice', 'Veg_Cod', 'descrizione', []);
    vegCol.ddl.descriptionField = 'Veg_Des';
    vegCol.ddl.loadOnEdit = true;
    vegCol.ddl.loadFunction = this.loadSpecie.bind(this);
  }

  private setVarietaDdl() {
    const culCol = this.kendoColumns.find(s => s.field === 'Cul_Cod');
    culCol.ddl = new DropdownListWithForm('codice', 'Cul_Cod', 'descrizione', []);
    culCol.ddl.descriptionField = 'Cul_Des';
    culCol.ddl.loadOnEdit = true;
    culCol.ddl.loadFunction = this.loadVarieta.bind(this);
  }

  private loadSpecie(dataItem: any): Observable<BaseCodeDescr[]> {
    if (dataItem) {
      const imp = this.formImpostazioniService.findGuidaImpostazione(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI);
      return from(this.specieService.leggi()).pipe(
        take(1),
        map((s: BaseCodeDescr[]) => {
          const impValue = imp.value as unknown as number[];
          return s.filter(x => impValue.includes(x.codice));
        })
      );
    }
  }

  private loadVarieta(dataItem: any): Observable<Varieta[]> {
    if (dataItem && dataItem.Veg_Cod)
      return this.cultivarService.leggiAsObs(new Specie(dataItem.Veg_Cod), false);
    return from([]);
  }

  private refreshOnSpeciesChange() {
    let orig: number[] = this.formImpostazioniService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI)
      .get("valoreCorrente").value;
    this.formImpostazioniService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI)
      .get("valoreCorrente").valueChanges
      .pipe(
        takeUntil(this.signal),
        map((ch: number[]) => {
          let toRemove = [];
          if (orig.length > ch.length) {
            toRemove = orig.filter(x => !ch.includes(x));
          }
          orig = ch;
          return toRemove;
        }),
        map((toRemove: number[]) => this.kendoRows.filter(x => toRemove.includes(x.Veg_Cod))),
        filter((toRemove: any[]) => toRemove.length > 0)
      ).subscribe((toRemove: any[]) => {
        for (let dataItem of toRemove) {
          let i = this.kendoRows.findIndex(r => r.chiave === dataItem.chiave);
          this.kendoRows.splice(i, 1);
          added.delete(dataItem.chiave);
        }
        this.updateForm();
        this.refresh(this.kendoRows);
      });
  }

  private handleEvents(event: any) {
    switch (event.action) {
      case 'add': // pulsante nuova riga
        this.kendoColumns.find(s => s.field === 'Veg_Cod').editable = true;
        this.kendoColumns.find(s => s.field === 'Cul_Cod').editable = true;
        break;
      case 'save': // salva nuova riga
        this.add(event.dataItem);
      case 'cancel':
        this.kendoColumns.find(s => s.field === 'Veg_Cod').editable = false;
        this.kendoColumns.find(s => s.field === 'Cul_Cod').editable = false;
        break;
      case 'remove':
        this.remove(event.dataItem);
        break;
      default:
        console.log(event);
    }
  }

  private uniqueInstance(control: AbstractControl): ValidationErrors | null {
    if (control && control.parent && control.parent.value.Veg_Cod) {
      const dataItem = control.parent.value;
      if (added.has(dataItem.Veg_Cod + "_" + control.value)) {
        return { "duplicateKey": true };
      }
      return null;
    }
    return { "noValue": true };
  }

  private add(dataItem: any) {
    dataItem.chiave = dataItem.Veg_Cod + "_" + dataItem.Cul_Cod;
    this.kendoRows.push(dataItem);
    added.add(dataItem.chiave);
    this.updateForm();
    this.refresh(this.kendoRows);
  }

  private remove(dataItem: any) {
    let i = this.kendoRows.findIndex(r => r.chiave === dataItem.chiave);
    this.kendoRows.splice(i, 1);
    added.delete(dataItem.chiave);
    this.updateForm();
    this.refresh(this.kendoRows);
  }

  /**
   * Updates the form with the current values from `kendoRows` and marks relevant fields as touched.
   * 
   * - Constructs a string (`valueStr`) by concatenating the `Cul_Cod` property of each row in `kendoRows`,
   *   separated by a pipe (`|`) character.
   * - Updates the `valoreCorrente` field in `innerForm` with the constructed string.
   * - Marks the `innerForm` as touched to indicate that it has been interacted with.
   * - Marks the settings forms for `UTENTE_COD_FILTRO_SPECIE_VEGETALI` and `UTENTE_COD_FILTRO_GRUPPI_VEGETALI`
   *   as touched to ensure their values are saved, even when they are read for scaling purposes.
   */
  private updateForm() {
    let valueStr = "";
    if (this.kendoRows.length)
      valueStr = this.kendoRows.map(r => r.Cul_Cod).reduce((acc, v) => acc + "|" + v);
    this.innerForm.get("valoreCorrente").patchValue(valueStr);
    this.innerForm.markAsTouched();
    // Segno le due impostazioni da cui dipendo as touched in modo da salvarne sempre i valori anche quando essi sono letti per scalare.
    this.formImpostazioniService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI).markAllAsTouched();
    this.formImpostazioniService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI).markAllAsTouched();
  }

  private refresh(newRows: any[]) {
    this.gridPublicService.refresh(true, new GridCategorieMagazzinoServerResult(
      newRows, this.kendoColumns, this.kendoModel
    ));
  }

}
