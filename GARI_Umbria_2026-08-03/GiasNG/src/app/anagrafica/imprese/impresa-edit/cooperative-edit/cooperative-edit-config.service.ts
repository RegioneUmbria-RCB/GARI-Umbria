import { Injectable, Injector } from '@angular/core';
import { FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { BehaviorSettings, CommandsColumnSettings, ExcelSettings, PDFSettings } from 'gias-kendo-grid';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Observable, of } from 'rxjs';
import { ContattiRootService } from '../contatti-edit/contattiRoot.service';
import { CooperativeKendoGridRow, CooperativeKendoServerResult } from './utils';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class CooperativeConfigService extends
  AbstractGridConfigService<CooperativeKendoServerResult>{
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'chiave';
  gridId: string = 'CooperativeConfigService';
  behavior: BehaviorSettings = new BehaviorSettings({ saveExternalChanges: true });

  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({infoBtn: false, editBtn: false,  removeBtn: true });
  padri: ImpresaPadre[];

  cooperativeFormGroup: FormGroup;

  constructor(
    injector: Injector,
    private gridPubService: GridPublicService,
    protected transloco: TranslocoService,
    private contattiRootService: ContattiRootService,
    private parametriAgendaService: ObjParametriAgendaService,
    private rootFormGroup: FormGroupDirective
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

  read(): Observable<CooperativeKendoServerResult> {
    let rows: CooperativeKendoGridRow[] = [];
    this.cooperativeFormGroup = this.rootFormGroup.form as FormGroup;

    this.cooperativeFormGroup.get('impresaPadre').value.forEach(padre => {
      rows.push({
        chiave: padre.partitaIva,
        codcooperativa: padre.partitaIva,
        cooperativa: padre.ragioneSociale,
        numero: padre.codice_iscrizione_libro_soci,
        data: padre.data_iscrizione_libro_soci ? padre.data_iscrizione_libro_soci : AGRODATAINIZIO,
        flag_cancellazione: false
      });
    });

    const tableData = this.GetModel(rows);
    if ((<ObjParametriAgenda>this.parametriAgendaService.getObjParamValue()).TipoOperazioneDB == Enum_DBTypeOperation.Read){
      this.makeCellsUneditable();
    }
    return of(tableData);
  }

  perform(actionType: HttpAction, item: any[]): Observable<any[]> {
    let cooperativePadre: ImpresaPadre[] = this.cooperativeFormGroup.get('impresaPadre').value;

    switch(actionType){
      case HttpAction.CREATE:
        item[0].codcooperativa.data.codice_iscrizione_libro_soci = item[0].numero;
        item[0].codcooperativa.data.data_iscrizione_libro_soci = item[0].data ? item[0].data : new Date(1900, 0, 1, 0, 0, 0, 0);
        cooperativePadre.push(item[0].codcooperativa.data);
        this.cooperativeFormGroup.get('impresaPadre').patchValue(cooperativePadre);
        break;
      case HttpAction.UPDATE:
        let modifica = cooperativePadre.find(impresa => item[0].chiave == impresa.partitaIva);
        if(item[0].codcooperativa.data) {
          for(let k in item[0].codcooperativa.data) modifica[k] = item[0].codcooperativa.data[k];
        }
        modifica.codice_iscrizione_libro_soci = item[0].numero;
        modifica.data_iscrizione_libro_soci = item[0].data ? item[0].data : new Date(1900, 0, 1, 0, 0, 0, 0);
        this.cooperativeFormGroup.get('impresaPadre').patchValue(cooperativePadre);
        break;
      case HttpAction.REMOVE:
        cooperativePadre = cooperativePadre.filter(impresa => impresa.partitaIva != item[0].codcooperativa);
        this.cooperativeFormGroup.get('impresaPadre').patchValue(cooperativePadre);
        break;
    }

    if(cooperativePadre.length != 1) {
      this.contattiRootService.singoloPadreSource.next(false);
      this.contattiRootService.setSalvaInPadre(false);
    } else {
      this.contattiRootService.singoloPadreSource.next(true);
    }
    this.gridPubService.refresh(true);
    return of([]);
  }

  GetModel(rows: CooperativeKendoGridRow[]) {
    this.columns = [
      new KendoGridColumn(
        { field: 'codcooperativa', title: this.transloco.translate('CooperativaReferente', {}) },
        { validators: [Validators.required], width: 135}
      ),
      new KendoGridColumn(
        { field: 'numero', title: this.transloco.translate('NumeroLibroSoci', {}) },
        { width: 135}
      ),
      new KendoGridColumn(
        { field: 'data', title: this.transloco.translate('DataIscrizioneLibroSoci', {}) },
        { width: 135, date: new DateSettings({ defaultValue: AGRODATAINIZIO })}
      )
    ];
    this.setTipologiaDDL(this.columns[0]);

    this.model = {
      codcooperativa: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
      cooperativa: new ModelEntry(CELL_TYPES.STRING, false),
      numero: new ModelEntry(CELL_TYPES.STRING),
      data: new ModelEntry(CELL_TYPES.DATE)
    };

    return new CooperativeKendoServerResult(this.model, this.columns, rows);
  }

  setTipologiaDDL(col: KendoGridColumn) {
    let list: DropdownListItem[] = [];
    col.ddl = new DropdownListWithForm('partitaIva', 'codcooperativa', 'ragioneSociale', list, null);
    col.ddl.valuePrimitive = false;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = "cooperativa";
    col.ddl.loadFunction = this.caricaPadri.bind(this);
  }

  caricaPadri(selezionata: any) {
    const emptyCompany: ImpresaPadre = new ImpresaPadre('');
    emptyCompany.ragioneSociale = '';
    if (this.padri.findIndex(c => c.partitaIva === emptyCompany.partitaIva) === -1 ) {
      this.padri.splice(0, 0, emptyCompany);
    }

    let cooperativePadre: ImpresaPadre[] = this.cooperativeFormGroup.get('impresaPadre').value;
    if(selezionata.codcooperativa){
      cooperativePadre = cooperativePadre.filter(impresa => impresa.partitaIva != selezionata.codcooperativa.id);
    }
    return of(this.padri.filter(a => !cooperativePadre.map(b=>b.partitaIva).includes(a.partitaIva)));
  }

  setPadri(padri: any){
    this.padri = padri;
  }
}

