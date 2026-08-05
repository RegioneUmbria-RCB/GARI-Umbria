import {Inject, Injectable, Injector} from '@angular/core';
import {
  AggregateSettings,
  AgrSelectableSettings,
  CommandsColumnSettings,
  ExcelSettings,
  PDFSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import {  ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { map } from 'rxjs/operators';
import { CatastoCampoKendoServerResult } from './catasto-campo-edit.model';
import { Validators } from '@angular/forms';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import {CellClickEvent, SelectionEvent} from '@progress/kendo-angular-grid';
import { CatastoCampoId, CatastoCampoService } from './catasto-campo.service';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { CampiEditDataService } from '../campi-edit-data.service';
import {GiasDialogService} from '../../../../Service/gias-dialog.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class CatastoCampoEditService extends AbstractGridConfigService<CatastoCampoKendoServerResult> {
  gridId: string = 'catastoCampo';
  rowId: string = 'chiave';

  toolbar: ToolbarSettings = new ToolbarSettings(false, false);

  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;

  aggregates: AggregateSettings = new AggregateSettings({
    enabled: true,
    descriptors: [
      {field: 'SuperficieIntersezione', aggregate: 'sum', format: 'n4'}
    ]}
  );

  private KendoCatastoCampo: any;
  private catastoRows: any[];

  constructor(
    injector: Injector,
    private catastoCampoService: CatastoCampoService,
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private translocoLoc: TranslocoService,
    private campiEditDataService: CampiEditDataService,
    private giasDialogService: GiasDialogService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();

    // gestore del change della colonna SuperficieIntersezione
    this.gridPublicService.formGroup.GiasSubscribe((fb) => {
      if (fb != undefined) {
        fb.controls["SuperficieIntersezione"].valueChanges.GiasSubscribe((supIntersezioneNew) => {
          if (supIntersezioneNew != undefined) {
            let particella: CatastoCampoId = this.findRowCorrenspondingParticella(fb.getRawValue());
            let supDisponibileOld = fb.controls["SuperficieDisponibile"].value;
            let increment: number = (particella.area ?? 0) - supIntersezioneNew;  // corresponds to: supIntersezioneOld - supIntersezioneNew
            fb.controls["SuperficieDisponibile"].setValue(parseFloat((supDisponibileOld + increment).toFixed(4)));

            // aggiorno il BehaviorSubjcet
            this.updateCatastoCampoBS(fb.getRawValue());
          }
        });
      }
    });

    this.objParametriAgendaService.currentObjParametriAgenda.GiasSubscribe(p => {
      this.gridPublicService.refresh(true);
    });
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    switch (actionType) {
      case HttpAction.CREATE:
        break;
      case HttpAction.REMOVE:
        break;
      case HttpAction.UPDATE:
        this.updateArea(items);
        break;
    }

    return from([]);
  }

  read(): Observable<CatastoCampoKendoServerResult> {
    const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const particelle = this.campiService.leggiParticellePerCentro(agenda);

    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

    return particelle.pipe(map(result => {
      this.KendoCatastoCampo = result.RispostaStringa;
      this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
      const righe = <any[]>(this.KendoCatastoCampo.kendo_rows);
      const model = this.setKendoModel();
      const cols = this.setKendoColumns();

      this.setKendoRows(righe);
      this.manageRowSelection(this.catastoRows);

      return new CatastoCampoKendoServerResult(
        model,
        cols,
        this.catastoRows
      );
    }));
  }

  handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
      onDisableInfoBtn: () => false
    });

    this.resizable.autoFitColumns = false;
    this.selectable.columnSettings.showSelectAll=true;
    this.selectable.shouldShowCheckbox = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB != Enum_DBTypeOperation.Read;
    this.selectable.columnSettings.title=' ';
    this.selectable.preselectedRows.selectionChangeFn = this.selectionChange;

    this.onCellClick = this.onCellClickBehavior;

    this.resizable.isResizable = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.views.enabled = false;
    this.columnMenu.columnMenu = false;

    // this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;

    // Setting generali
    this.behavior.saveExternalChanges = true;
    this.behavior.excelSettings = new ExcelSettings({enabled: false});
    this.behavior.pdfSettings = new PDFSettings({enabled: false});
    this.generalSettings.reordable = true;
    this.generalSettings.performOnEdit = true;
    this.groups.groupable.enabled = false;
  }

  private setKendoColumns (): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'COMUNI_PROV', title: this.translocoLoc.translate('Provincia') },
        { resizable: true, editable: false, validators: [Validators.required], width: 120 }
      ),
      new KendoGridColumn(
        { field: 'LOCALITA', title: this.translocoLoc.translate('Comune') },
        { resizable: true, editable: false, validators: [Validators.required], width: 200 }
      ),
      new KendoGridColumn(
        { field: 'Sezione', title: this.translocoLoc.translate('Sezione') },
        { resizable: true, editable: false, validators: [Validators.required], width: 120 }
      ),
      new KendoGridColumn(
        { field: 'Foglio', title: this.translocoLoc.translate('Foglio') },
        { resizable: true, editable: false, validators: [Validators.required], width: 100, numeric: {multiCheckFiltering: true} }
      ),
      new KendoGridColumn(
        { field: 'Numero', title: this.translocoLoc.translate('Numero') },
        { resizable: true, editable: false, validators: [Validators.required], width: 100, numeric: {multiCheckFiltering: true} }
      ),
      new KendoGridColumn(
        { field: 'Subalterno', title: this.translocoLoc.translate('Subalterno') },
        { resizable: true, editable: false, validators: [Validators.required],width: 120 }
      ),
      new KendoGridColumn(
        { field: 'Superficie', title: this.translocoLoc.translate('Superficie') },
        { resizable: true, editable: false, validators: [Validators.required], width: 120 }
      ),
      new KendoGridColumn(
        { field: 'SuperficieDisponibile', title: this.translocoLoc.translate('SuperficieDisponibile') },
        { resizable: true, editable: false, numeric: { format: 'n4' }, validators: [Validators.required], width: 140 }
      ),
      new KendoGridColumn(
        { field: 'SuperficieIntersezione', title: this.translocoLoc.translate('SuperficieIntersecataAbbr') },
        { resizable: true, editable: true, numeric: { defaultValue: 0, min: 0, format: 'n4' }, width: 140 }
      )
    ];
  }

  private setKendoModel () {
    return {
      Com: {editable: false, type: CELL_TYPES.STRING},
      COMUNI_PROV: {editable: false, type: CELL_TYPES.STRING},
      Foglio: {editable: false, type: CELL_TYPES.NUMBER},
      LOCALITA: {editable: false, type: CELL_TYPES.STRING},
      Numero: {editable: false, type: CELL_TYPES.NUMBER},
      Part_Cod: {editable: false, type: CELL_TYPES.NUMBER},
      particella_are: {editable: false, type: CELL_TYPES.NUMBER},
      Particella_centiare: {editable: false, type: CELL_TYPES.NUMBER},
      Particella_ettari: {editable: false, type: CELL_TYPES.NUMBER},
      Prov: {editable: false, type: CELL_TYPES.STRING},
      Sezione: {editable: false, type: CELL_TYPES.STRING},
      Subalterno: {editable: false, type: CELL_TYPES.STRING},
      Sup_Condotta: {editable: false, type: CELL_TYPES.NUMBER},
      Superficie: {editable: false, type: CELL_TYPES.NUMBER},
      SuperficieDisponibile: {editable: false, type: CELL_TYPES.NUMBER},
      SuperficieUtilizzata: {editable: false, type: CELL_TYPES.NUMBER},
      AreaSuAppLiberi: {editable: false, type: CELL_TYPES.NUMBER},
      AreaSuCampiSquadri: {editable: false, type: CELL_TYPES.NUMBER},
      AreaSuAppSuCampiNonSquadri: {editable: false, type: CELL_TYPES.NUMBER},
      SuperficieIntersezione: {editable: true, type: CELL_TYPES.NUMBER}
    };
  }

  private setKendoRows(righe: any[]) {
    this.catastoRows = righe.map((item) => ({
      chiave: item.Prov + '_' + item.Com + '_' + item.Sezione + '_' + item.Foglio + '_' + item.Numero + '_' + item.Subalterno,
      Prov: item.Prov,
      Com: item.Com,
      COMUNI_PROV: item.COMUNI_PROV,
      LOCALITA: item.LOCALITA,
      Sezione: (item.Sezione == '' ? '0' : item.Sezione),
      Foglio: item.Foglio,
      Numero: item.Numero,
      Subalterno: (item.Subalterno == '' ? '0' : item.Subalterno),
      Superficie: item.Superficie,
      Particella_ettari: item.Particella_ettari,
      Particella_are: item.Particella_are,
      Particella_centiare: item.Particella_centiare,
      SuperficieDisponibile: item.SuperficieDisponibile,
      SuperficieUtilizzata: item.SuperficieUtilizzata,
      AreaSuAppLiberi: item.AreaSuAppLiberi ?? 0,
      AreaSuCampiSquadri: item.AreaSuCampiSquadri ?? 0,
      AreaSuAppSuCampiNonSquadri: item.AreaSuAppSuCampiNonSquadri ?? 0,
      SuperficieIntersezione: 0
    }));
  }

  private setRowSelected(item: any): void {
    const particelleCampo: CatastoCampoId[] = this.campiEditDataService.catastoCampo;
    for (const particella of particelleCampo) {
      if(this.isSameParcel(item, particella)) {
        // per le particelle checked imposto anche la Superficie di Intersezione
        item.SuperficieIntersezione = particella.area;
        item.Selected = true;
        return;
      }
    }
    item.Selected = false;
  }

  private manageRowSelection(rows: any[]): void {
    for (let i= 0; i < rows.length; i++) {
      this.setRowSelected(rows[i]);
    }
  }

  private selectionChange = (event: SelectionEvent, component: GiasKendoGridComponent) => {
    this.handleSelectedRows(event);
    this.handleDeselectedRows(event);

    // forzo il ricalcolo dei totali, altrimenti tale operazione non viene fatta sinché non si va a modificare a mano il dato nella cella
    this.gridPublicService.giasGridComponent.refreshAggregateTotal();
  };

  private handleSelectedRows(event: SelectionEvent): void {
    const catastoCampo = this.campiEditDataService.catastoCampo;
    const selectedRows = event.selectedRows;

    if (selectedRows.length !== 0) {
      event.selectedRows.forEach(row => {
        if (row.dataItem.SuperficieDisponibile > 0) {
          // imposto la superficie utilizzata pari al massimo della superficie disponibile
          row.dataItem.SuperficieIntersezione = row.dataItem.SuperficieDisponibile;
          row.dataItem.SuperficieDisponibile = 0;

          const catasto: CatastoCampoId = this.prepareCatastoCampoId(row.dataItem, catastoCampo);
          catastoCampo.push(catasto);

          this.campiEditDataService.catastoCampo = catastoCampo;
          this.updateCatastoCampoBS(row.dataItem);
        } else {
          this.giasDialogService.baseInfo('', this.transloco.translate('AssociazioneParticellaSuperficieDisponibileZero'));
          this.gridPublicService.giasGridComponent.deselectRows([row]);
        }
      });
    }
  }

  private handleDeselectedRows(event: SelectionEvent): void {
    const catastoCampo = this.campiEditDataService.catastoCampo;
    const deselectedRows = event.deselectedRows;

    if (deselectedRows.length !== 0 ) {
      event.deselectedRows.forEach(row => {
        // imposto la superficie utilizzata pari al massimo della superficie disponibile
        row.dataItem.SuperficieDisponibile += row.dataItem.SuperficieIntersezione;
        row.dataItem.SuperficieIntersezione = 0;

        const index: number = this.catastoCampoService.getIndexOfCatastoCampoId(row.dataItem, catastoCampo);

        // chiudo tutte le celle della riga selezionata
        this.gridPublicService.gridComp.closeRow(row.index);

        if (index!==-1) {
          catastoCampo.splice(index, 1);
        }

        this.campiEditDataService.catastoCampo = catastoCampo;
        this.updateCatastoCampoBS(row.dataItem);
      });
    }
  }

  /** previene che si possa modificare la riga se essa non è stata selezionata */
  private onCellClickBehavior = (event: CellClickEvent) => {
    //Se la riga non è stata selezionata chiudo la cella
    if(!event.dataItem.Selected) {
      event.sender.closeRow(event.rowIndex);
    }
  };

  private updateArea(item: any) {
    let catasto: CatastoCampoId[] = this.campiEditDataService.catastoCampo;

    catasto.forEach(c => {
      if(this.isSameParcel(item[0], c)) {
        c.area = item[0].SuperficieIntersezione;
      }
    });

    this.campiEditDataService.catastoCampo = catasto;
  }

  private findRowCorrenspondingParticella(row: any): CatastoCampoId {
    const particelleCampo: CatastoCampoId[] = this.campiEditDataService.catastoCampo;
    return particelleCampo.find(pc => this.isSameParcel(row, pc));
  }

  private updateCatastoCampoBS(row: any): void {
    this.campiEditDataService.catastoCampo = this.campiEditDataService.catastoCampo.map(pc => {
      if (this.isSameParcel(row, pc)) {
        pc.area = row.SuperficieIntersezione;
        pc.particella.Area = row.SuperficieIntersezione;
      }
      return pc;
    });
  }

  private isSameParcel(row: any, pc: CatastoCampoId): boolean {
    return (
      row.Com == pc.particella.primaryKey.Com &&
      row.Prov == pc.particella.primaryKey.Prov &&
      row.Foglio == pc.particella.primaryKey.Foglio &&
      row.Numero == pc.particella.primaryKey.Numero &&
      (row.Sezione == '' ? '0' == pc.particella.primaryKey.Sezione : row.Sezione == pc.particella.primaryKey.Sezione) &&
      (row.Subalterno == '' ? '0' == pc.particella.primaryKey.Subalterno : row.Subalterno == pc.particella.primaryKey.Subalterno)
    );
  }

  private prepareCatastoCampoId(dataItem: any, catastoCampo: CatastoCampoId[]): CatastoCampoId {
    const catasto: CatastoCampoId = new CatastoCampoId();
    catasto.particella = new ParticelleCatastali();
    catasto.particella.primaryKey = new ParticelleCatastali.PK(
      dataItem.chiave.split('_')[0],
      dataItem.chiave.split('_')[1],
      (dataItem.chiave.split('_')[2] == '' ? '0' : dataItem.chiave.split('_')[2]),
      dataItem.chiave.split('_')[3],
      dataItem.chiave.split('_')[4],
      (dataItem.chiave.split('_')[5] == '' ? '0' : dataItem.chiave.split('_')[5])
    );

    const index = Math.max.apply(Math, catastoCampo.map(function (o) {
      return o.id;
    }));
    catasto.id = index + 1;

    return catasto;
  }

}
