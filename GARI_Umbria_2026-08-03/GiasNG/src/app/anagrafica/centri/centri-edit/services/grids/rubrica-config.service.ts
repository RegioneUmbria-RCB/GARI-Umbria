import { Injectable, Injector } from '@angular/core';
import { AbstractControl, ValidatorFn, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { PropertyValidator } from 'app/anagrafica/centri/validators/ValidateProperty';
import { RubricaVociConChiave } from 'app/Model/anagrafiche/RubricaVoci';
import { BehaviorSettings, CommandsColumnSettings, ExcelSettings, PDFSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridRow, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable, of, Subject } from 'rxjs';
import { RubricaKendoGridRow, RubricaKendoServerResult, RubricaLoaded } from '../../utils';
import { EditCentroStore } from '../centri-store.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';

@Injectable()
export class RubricaConfigService extends
  AbstractGridConfigService<RubricaKendoServerResult>  {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'chiave';
  gridId: string = 'RubricaConfigService';
  behavior: BehaviorSettings = new BehaviorSettings({ saveExternalChanges: true });

  rubricaReady: Subject<RubricaLoaded> = new Subject();
  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({infoBtn: false, editBtn: false,  removeBtn: true })

  constructor(
    injector: Injector,
    private store: EditCentroStore,
    protected transloco: TranslocoService,
    private parametriAgendaService: ObjParametriAgendaService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar.newItem = true;
    this.generalSettings.performOnEdit = true;
    this.cmdColumn.editBtn = false;

    if ((<ObjParametriAgenda>this.parametriAgendaService.getObjParamValue()).TipoOperazioneDB == Enum_DBTypeOperation.Read){
      this.disableGrid();
    }
    
    this.columnMenu.columnMenu = false;
    this.columnMenu.filterable = false;
    this.behavior.excelSettings = new ExcelSettings({enabled: false});
    this.behavior.pdfSettings = new PDFSettings({enabled: false});
    this.generalSettings.reordable = false;
    this.generalSettings.performOnEdit = true;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;

    this.cmdColumn.editBtn = false;
  }

  read(options?: any): Observable<RubricaKendoServerResult> {
    return this.rubricaReady.pipe(map((s: RubricaLoaded) => {
      return this.GetModel(s.rubricaVoci as RubricaVociConChiave[]);
    }));
  }
  perform(actionType: HttpAction, item: any[]): Observable<any[]> {
    return of([]);
  }

  validateRubrica(row: KendoGridRow): ValidatorFn {
    let that = this;

    let customValidator = (control: AbstractControl): { [key: string]: boolean } | null => {
      let rows = that.gridPublicService.value.data.rows;
      let valoreAttuale = control.value;

      rows = rows.filter(s => s['tipologia'])
      let invalid = rows.filter(s => {
        if(row && row['tipologia'] === s['tipologia'])
          return false;

        let valore = UtilityFunctions.getPrimitiveValueDDL(s['tipologia']);
        return valore == valoreAttuale;
      }).length > 0;

      if(control.value == null)
        invalid = true;

      if (invalid) {
        return { codiceExists: valoreAttuale };
      }
      return null;
    };

    return customValidator;
  }

  GetModel(rows: RubricaVociConChiave[]) {
    let propertyValidator = new PropertyValidator('id', this.validateRubrica.bind(this));

    this.columns = [
      new KendoGridColumn(
        { field: 'tipologia', title: this.transloco.translate('Descrizione', {}) },
        { validators: [/*propertyValidator*/], width: 135}
      ),
      new KendoGridColumn(
        { field: 'valore', title: this.transloco.translate('Valore', {}) },
        { validators: [Validators.required], width: 135}
      )
    ];
    this.setTipologiaDDL(this.columns[0]);

    this.model = {
      valore: new ModelEntry(CELL_TYPES.STRING),
      tipologia: new ModelEntry(CELL_TYPES.DROPDOWNLIST)
    }

    let krows: RubricaKendoGridRow[] = [];
    rows?.forEach(s => {
      krows.push({ valore: s.valore, tipologia: s.rubrica.tipologia, chiave: s.chiave,
        codice: s.rubrica.codice, flag_cancellazione: s.flag_cancellazione });
    });

    if ((<ObjParametriAgenda>this.parametriAgendaService.getObjParamValue()).TipoOperazioneDB == Enum_DBTypeOperation.Read){
      this.makeCellsUneditable();
    }

    return new RubricaKendoServerResult(this.model, this.columns, krows);
  }

  setTipologiaDDL(col: KendoGridColumn) {
    let list: DropdownListItem[] = [
      { id: 'Telefono', name: 'Telefono' }, // 1
      { id: 'Cellulare', name: 'Cellulare' }, // 2
      { id: 'Fax', name: 'Fax' }, // 3
      { id: 'Email', name: 'Email' }, // 4
      { id: 'Social', name: 'Social' }, // 5
      { id: 'Web', name: 'Web' }, // 6
      { id: 'Pec', name: 'Pec' } // 7
    ];

    col.ddl = new DropdownListWithForm('tipologia', 'tipologia', 'tipologia', list, null);
    col.ddl.valuePrimitive = false;
  }
}

