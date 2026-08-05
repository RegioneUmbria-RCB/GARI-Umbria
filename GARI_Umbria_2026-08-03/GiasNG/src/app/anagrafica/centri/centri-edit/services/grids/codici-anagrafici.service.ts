import { Injectable, Injector } from '@angular/core';
import { AbstractControl, ValidatorFn, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { PropertyValidator } from 'app/anagrafica/centri/validators/ValidateProperty';
import { CodiciServerResult } from 'app/Utility/Template/codici-template/models/codici.model';
import { BehaviorSettings, CommandsColumnSettings, ExcelSettings, PDFSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridRow, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable, of, Subject } from 'rxjs';
import { CodiceKendoGridRow, CodiciAnagraficiServerResult, CodiciLoaded } from '../../utils';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';

@Injectable()
export class CodiciAnagraficiConfigService extends
  AbstractGridConfigService<CodiciAnagraficiServerResult>  {

  public codiciReady: Observable<CodiciLoaded> = new Subject();

  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'chiave';
  gridId: string = 'CodiciAnagrafici';
  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({infoBtn: false, editBtn: false,  removeBtn: true })
  behavior: BehaviorSettings = new BehaviorSettings({ saveExternalChanges: true });

  constructor(injector: Injector, 
              protected transloco: TranslocoService,
            private parametriAgendaService: ObjParametriAgendaService) {
    super(injector);


    if ((<ObjParametriAgenda>this.parametriAgendaService.getObjParamValue()).TipoOperazioneDB == Enum_DBTypeOperation.Read){
      this.disableGrid();
    }
    this.toolbar.newItem = true;
    this.columnMenu.columnMenu = false;
    this.cmdColumn.editBtn = false;
    this.columnMenu.filterable = false;
    this.behavior.excelSettings = new ExcelSettings({enabled: false});
    this.behavior.pdfSettings = new PDFSettings({enabled: false});
    this.generalSettings.reordable = false;
    this.generalSettings.performOnEdit = true;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;

  }

  read(options?: any): Observable<CodiciServerResult> {

    return this.codiciReady.pipe(map(s => {
      if(!s.codici || !s.ddl ) {
        throw new Error('Data not yet ready');
      }

      let result: CodiciServerResult = this.GetModel(s);
      if ((<ObjParametriAgenda>this.parametriAgendaService.getObjParamValue()).TipoOperazioneDB == Enum_DBTypeOperation.Read){
        this.makeCellsUneditable();
    }
      return result;
    }))
  }
  perform(actionType: HttpAction, items: any[]): Observable<any[]> {

    return of([]);
  }


  GetModel(data: CodiciLoaded): CodiciServerResult {

    let propertyValidator = new PropertyValidator('id', this.validateCodice.bind(this));

    this.columns = [
      new KendoGridColumn({ field: 'codice', title: this.transloco.translate('Codice') },
        { validators: [Validators.required], width: 135}),
      new KendoGridColumn(
        { field: 'valore', title: this.transloco.translate('Valore') },
        { validators: [Validators.required], width: 135}
      ),
      new KendoGridColumn({ field: 'dal', title: this.transloco.translate('ValiditàInizio') }, { hidden: false, date: { defaultValue: AGRODATAINIZIO } }),
      new KendoGridColumn({ field: 'al', title: this.transloco.translate('ValiditàFine') }, { hidden: false, date: { defaultValue: AGRODATAFINE } }),
    ];
    setCodiceDDL(this.columns[0], data.ddl);

    this.model = {
      valore: new ModelEntry(CELL_TYPES.STRING, false),
      descrizione: new ModelEntry(CELL_TYPES.STRING, false),
      codice: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
      dal: new ModelEntry(CELL_TYPES.DATE, false),
      al: new ModelEntry(CELL_TYPES.DATE, false)
    }


    let krows: CodiceKendoGridRow[] = [];
    data.codici?.forEach((s, index) => {
      krows.push({
        chiave: s.chiave,
        valore: s.valore,
        codice: s.codiceAnagrafe.codice,
        descrizione: s.codiceAnagrafe.descrizione,
        dal: s.validita.inizio,
        al: s.validita.fine
      });
    });

    return new CodiciServerResult(this.model, this.columns, krows);
  }

  validateCodice(row: KendoGridRow): ValidatorFn {
    let that = this;

    let customValidator = (control: AbstractControl): { [key: string]: boolean } | null => {
      let rows = that.gridPublicService.value.data.rows;

      let valoreAttuale = control.value;

      rows = rows.filter(s => s['codice'])
      let invalid = rows.filter(s => {
        if(row && row['codice'] === s['codice'])
          return false;

        let valore = UtilityFunctions.getPrimitiveValueDDL(s['codice']);
        return valore == valoreAttuale;
      }).length > 0;

      if(control.value === null)
        invalid = true;

      if (invalid) {
        return { codiceExists: valoreAttuale };
      }
      return null;
    };

    return customValidator;

  }
}




function setCodiceDDL(col: KendoGridColumn, ddl: DropdownListItem[]) {
  col.ddl = new DropdownListWithForm('codiciCentro', 'codice', 'descrizione', ddl, null);
  col.ddl.valuePrimitive = true;
  col.ddl.descriptionField = 'descrizione';
}
