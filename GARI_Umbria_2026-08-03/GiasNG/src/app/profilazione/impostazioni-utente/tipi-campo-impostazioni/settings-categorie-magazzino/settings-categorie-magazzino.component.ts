import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListItem,
  DropdownListWithForm,
  generateGridProviders,
  GiasKendoGridComponent,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  ModelEntry
} from 'gias-kendo-grid';
import { CategorieMagazzinoService } from '../../../services/categorie-magazzino.service';
import {
  GridCategorieMagazzinoServerResult,
  GridCategorieMagazzinoService
} from './grid-settings-categorie-magazzino.service';
import { enum_Impostazioni_Utenti } from "../../../../Model/Impostazioni_Utenti.enum";
import { TranslocoService } from "@jsverse/transloco";
import { UnitaDiMisuraService } from "../../../../Service/Metaschema/UnitaDiMisura.service";
import {
  CATEGORIE_MAGAZZINO_POSITIVE,
  enum_PaginaImpostazioni,
  FILTRO_CATEGORIE_MAGAZZINO,
  FILTRO_CATEGORIE_MAGAZZINO_APP
} from "../../impostazioni.model";
import { BehaviorSubject, filter, forkJoin, Observable, of, take, takeUntil } from 'rxjs';
import { BaseCodeDescr } from "../../../../Model/baseClass/baseCodeDescr";
import { UnitaDiMisura } from "../../../../Model/metaschema/UnitaDiMisura";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { ImpostazioniFormService } from "../../../services/impostazioni/impostazioni-form.service";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { enum_Dati_App, enum_Import_App } from 'app/amministrazione-sistema/dati-app/consulta-sincro-dati-app/consulta-sincro.service';
import { hashString } from 'app/profilazione/models/Impostazioni/ImpostazioniApp.model';
import { enum_Security_Attivita } from 'gias-kendo-grid/lib/shared/TipiEnumerativi';

interface ICategory {
  Elem_Cod: number;
  ValueCod: number;
  ValueDes: string;
  Impostazione: number;
  NomeComune: string;
}


@Component({
  standalone: false,
  selector: 'app-settings-categorie-magazzino',
  templateUrl: './settings-categorie-magazzino.component.html',
  styleUrls: ['./settings-categorie-magazzino.component.css'],
  providers: [
    ...generateGridProviders(GridCategorieMagazzinoService, SettingsCategorieMagazzinoComponent),
    GiasDropDownTemplateService
  ]
})
export class SettingsCategorieMagazzinoComponent implements OnInit, AfterViewInit {
  @Input('formGroup') form: FormGroup;
  @Input() ddlListItems: DropdownListItem[] = [];

  @ViewChild('grid') grid: GiasKendoGridComponent;

  private _indiceUdM: Map<number, UnitaDiMisura> = new Map<number, UnitaDiMisura>();
  private udmLoaded$ = new BehaviorSubject<boolean>(false);

  private readonly rowModel: KendoGridModel = {
    NomeComune: new ModelEntry(CELL_TYPES.STRING),
    Elem_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    ValueCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    ValueDes: new ModelEntry(CELL_TYPES.STRING),
    Impostazione: new ModelEntry(CELL_TYPES.NUMBER)
  };
  private readonly gridCols: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'NomeComune', title: this.transloco.translate('Categoria') }, {
      editable: false,
      style: { "text-align": "left" }
    }),
    new KendoGridColumn({ field: 'ValueCod', title: '' })
  ];

  constructor(
    private transloco: TranslocoService,
    private udmService: UnitaDiMisuraService,
    private datashare: CategorieMagazzinoService,
    private settingsFormService: ImpostazioniFormService,
    private pd: ProfilazioneDataShareService
  ) {
    this.udmService.Leggi_Tutte_UnitaDiMisura().pipe(take(1))
      .subscribe(list => {
        list?.forEach(udm => this._indiceUdM.set(udm.codice, udm));
        this.udmLoaded$.next(true);
      });
    this.pd.reloadSetting$.pipe(
      takeUntil(this.pd.exitProfilazione),
      filter(signal => this.grid
        && signal.component === 'SettingsCategorieMagazzinoComponent'
        && signal.setting === this.form.value.guida.Impostazione_Cod
      )
    ).subscribe((comp) => {
      console.log(this.form.value.guida.Impostazione_Cod, this.form.value.valoreCorrente);
      this.ddlListItems = this.getDdlItems();
      this.loadGrid();
    });
  }

  private get isDisabled(): boolean {
    return this.form.disabled;
  }

  private get impostazioneCod(): number {
    return this.form.value.guida.Impostazione_Cod;
  }

  /**
   * Retrieves the initial value for the component as an array of objects,
   * each containing a `codice` and `valore` property. The method processes
   * the `valori` field from the form's value, splitting its first element
   * (if available) by the '|' delimiter and further splitting each segment
   * by the '_' delimiter to extract numeric values.
   * 
   * @returns An array of objects where each object has:
   *          - `codice`: A number representing the first part of the split value (Elem_Cod).
   *          - `valore`: A number representing the second part of the split value (ddl item code).
   */
  private get initialValue(): { codice: number; valore: number }[] {
    const valore = this.form.value.valori?.at(0)?.valore ?? "";
    if (!valore) return [];
    return valore.split('|')
      .map(v => v.split("_"))
      .map(v => ({ codice: +v[0], valore: +v[1] }));
  }

  private get defaultDdlValue(): BaseCodeDescr {
    const col = this.gridCols.find(c => c.field === 'ValueCod');
    let item = new BaseCodeDescr(0);
    if (col.ddl.defaultValue) {
      item.codice = col.ddl.defaultValue.id;
      item.descrizione = col.ddl.defaultValue.name;
    }
    return item;
  }

  ngOnInit(): void {
    this.pd.formsMap.set(this.impostazioneCod, this.form);
    if (!this.ddlListItems.length)
      this.ddlListItems = this.getDdlItems();
  }

  ngAfterViewInit(): void {
    if (this.grid) {
      this.loadGrid();
    } else console.error('view init: absent');
  }

  private setInitalValue(rows: any[]) {
    const filtroStr = rows.filter(u => u.ValueCod !== undefined)
      .map(u => u.Elem_Cod + "_" + u.ValueCod)
      .reduce((a, b) => a + "|" + b);
    if (this.form.get('valoreCorrente').value !== filtroStr) {
      this.form.get('valoreCorrente').patchValue(filtroStr);
      this.form.markAsUntouched();
    }
  }

  private loadGrid() {
    let obs: Observable<any>[] = [];
    obs.push(this.loadRows());
    obs.push(this.setChoiceLoader());
    forkJoin(obs).subscribe(([results, choices]) => {
      // TODO ANNY: vedi come vengono valorizzate rows in `getValorizedRows`
      const rows = this.getValorizedRows(results);
      this.setInitalValue(rows);
      // console.log("refresh", this.impostazioneCod, rows);
      this.grid.publicService.refresh(true, new GridCategorieMagazzinoServerResult(rows, this.gridCols, this.rowModel));
      if (this.isDisabled) {
        setTimeout(() => this.grid.publicService.disable(), 500);
      }
    });
  }

  private setChoiceLoader(): Observable<any> {
    const col = this.gridCols.find(c => c.field === 'ValueCod');
    switch (this.impostazioneCod) {
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT:
        this.setDdl(col, 'UnitàDiMisura');
        col.ddl.loadOnEdit = true;
        col.ddl.loadFunction = this.loadUDM.bind(this);
        return this.udmLoaded$.value
          ? of(true)
          : this.udmLoaded$.pipe(filter(value => value === true), take(1));
      default:
        this.setDdl(col, 'Filtro');
        col.ddl.loadOnEdit = false;
        col.ddl.data = this.ddlListItems;
        col.ddl.defaultValue = this.ddlListItems[0];
        return of(true);
    }
  }

  private setDdl(col: KendoGridColumn, keyTitle: string) {
    col.title = this.transloco.translate(keyTitle);
    col.ddl = new DropdownListWithForm('codice', 'ValueCod', 'descrizione', []);
    col.ddl.id = 'codice';
    col.ddl.formControlValue = 'descrizione';
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'ValueDes';
  }

  private loadUDM(row: KendoGridRow): Observable<BaseCodeDescr[]> {
    return this.datashare.leggiUnitaMisura(row['Elem_Cod']);
  }

  private loadRows(): Observable<any[]> {
    let params = null;
    if (this.settingsFormService.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      params = this.datashare.getLeggiCategorieMagazzinoImpostazioni(
        FILTRO_CATEGORIE_MAGAZZINO, this.impostazioneCod
      );
    }
    switch (this.impostazioneCod) {
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT:
        return this.datashare.leggiCategorieMagazzino(CATEGORIE_MAGAZZINO_POSITIVE);
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI:
      case enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco:
        return this.datashare.leggiFiltroLottiCategorieMagazzino(params);
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE:
        return this.datashare.leggiFiltroGiacenzeCategorieMagazzino(params);
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP:
        params = this.datashare.getLeggiCategorieMagazzinoImpostazioni(
          FILTRO_CATEGORIE_MAGAZZINO_APP, this.impostazioneCod
        );
        return this.datashare.leggiFiltroGiacenzeCategorieMagazzino(params);
      case (enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI + hashString('Importazioni') * 1000):
        return this.datashare.getRowsForImportApp();
      default:
        return this.datashare.leggiCategorieMagazzino(CATEGORIE_MAGAZZINO_POSITIVE);
    }
  }

  /**
   * Processes an array of category rows and updates their properties based on initial values.
   *
   * @param rows - An array of category objects implementing the `ICategory` interface.
   * @returns The updated array of category rows with modified properties.
   *
   * Each row in the input array is updated with:
   * - `ValueCod`: The code of the current value, or a default value if not found.
   * - `ValueDes`: The description of the current value, or a default value if not found.
   * - `Impostazione`: A predefined setting code (`impostazioneCod`).
   *
   * The method uses `initialValue` to find matching entries for each row and applies
   * transformations using `getCurrentValue` or assigns a default value (`defaultDdlValue`).
   */
  private getValorizedRows(rows: ICategory[]) {
    const values = this.initialValue;

    for (let row of rows) {
      if (!this.handleSpecialRowValorization(row, values)) {
        let current = values.find(v => v.codice == row.Elem_Cod);
        let value = (current && this.isValorized(current.valore)) ? this.getCurrentValue(current.valore) : this.defaultDdlValue;
        row.ValueCod = value.codice;
        row.ValueDes = value.descrizione;
      }
      row.Impostazione = this.impostazioneCod;
    }
    return rows;
  }

  /**
   * Handles the special valorization of a row based on specific conditions.
   *
   * @param row - The category row to be processed.
   * @param values - An array of objects containing `codice` and `valore` properties.
   * @returns A boolean indicating whether the row was successfully valorized.
   *
   * @NotesUsage
   * Used for the case in which the component refers to the GiasApp import settings
   * and the user had previously save a single number as setting value
   */
  private handleSpecialRowValorization(row: ICategory, values: { codice: number; valore: number }[]): boolean {
    if (this.impostazioneCod == (enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI + hashString('Importazioni') * 1000)) { // It applies the saved value to all the categories        
      return this.handleAppImportsRowValorize(row, values);
    }
    return false;
  }

  private handleAppImportsRowValorize(row: ICategory, values: { codice: number; valore: number }[]): boolean {
    const defaults = [
      { codice: enum_Dati_App.OperazioniDiCampagna, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.OreMacchineCdG, valore: enum_Import_App.Nessuno },
      { codice: enum_Dati_App.Ricette, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.RilieviEVisiteConRilievi, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.Visite, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.DocumentiFileFotoFilmatiAudio, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.MovimentiDiMagazzino, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.Acquisti, valore: enum_Import_App.Completo },
      { codice: enum_Dati_App.ManutenzioniMacchine, valore: enum_Import_App.Completo }
    ];
    if (values.length === 1 && !values[0].valore) {
      const value = this.ddlListItems.find(i => i.id === values[0].codice);
      if (value) {
        row.ValueCod = value.id;
        row.ValueDes = value.name;
      }
      return true;
    } else {
      let value = values.find(v => v.codice == row.Elem_Cod) || defaults.find(v => v.codice == row.Elem_Cod);
      if (!value) {
        return false;
      }
      const item = this.ddlListItems.find(i => i.id === value.valore);
      if (item) {
        row.ValueCod = item.id;
        row.ValueDes = item.name;
        return true;
      }
    }
    return false;
  }

  private getCurrentValue(valueCod: number): BaseCodeDescr {
    if (this.impostazioneCod === enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT) {
      return this._indiceUdM.get(valueCod);
    } else {
      let item = this.ddlListItems.find(i => i.id === valueCod);
      return new BaseCodeDescr(item.id, item.name);
    }
  }

  /**
   * Ritorna la lista di possibili scelte per ogni elemento in griglia.
   *
   * @usageNotes Il primo elemento è quello di default.
   * @private
   */
  private getDdlItems() {
    switch (this.impostazioneCod) {
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI:
      case enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco:
        return [
          new DropdownListItem(0, this.transloco.translate('Nessuna')),
          new DropdownListItem(1, this.transloco.translate('Obbligatoria')),
          new DropdownListItem(2, this.transloco.translate('Facoltativa')),
        ];
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE:
        return [
          new DropdownListItem(2, this.transloco.translate('TuttiProdotti')),
          new DropdownListItem(0, this.transloco.translate('SoloMovimentati')),
          new DropdownListItem(1, this.transloco.translate('SoloPresenti')),
        ];
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP:
        return [
          new DropdownListItem(0, this.transloco.translate('TuttiProdotti')),
          new DropdownListItem(1, this.transloco.translate('SoloMovimentati')),
          new DropdownListItem(2, this.transloco.translate('SoloPresenti')),
        ];
      case enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA:
        return [
          new DropdownListItem(1, this.transloco.translate('Si')),
          new DropdownListItem(0, this.transloco.translate('No')),
        ];
      case (enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI + hashString('Importazioni') * 1000):
        return [
          new DropdownListItem(enum_Import_App.Nessuno, this.transloco.translate('Manuale')),
          new DropdownListItem(enum_Import_App.Parziale, this.transloco.translate('Schedulato')),
          new DropdownListItem(enum_Import_App.Completo, this.transloco.translate('Automatico')),
        ];
      default:
        return [];
    }
  }

  private isValorized(value: string | number | boolean): boolean {
    return value !== undefined && value !== null && !Number.isNaN(value);
  }

}
