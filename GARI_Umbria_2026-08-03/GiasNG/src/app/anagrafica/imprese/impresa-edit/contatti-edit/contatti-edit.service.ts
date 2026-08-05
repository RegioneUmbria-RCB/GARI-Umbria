import { Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, GridCustomizations, HttpAction } from 'gias-kendo-grid';
import { combineLatest, Observable, of } from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, take } from 'rxjs/operators';
import { ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import { MetodoProduzione } from 'app/Model/metaschema/MetodoProduzione';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ContattiEditGridModel, ContattiEditServerResult } from './contattiGrid.models';
import { ContattiRootService } from './contattiRoot.service';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { TranslocoService } from '@jsverse/transloco';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { Validators } from '@angular/forms';
import { enum_RapportiContabili_SaCod } from 'app/Model/TipiEnumerativi';
import {RapportoContabile} from '../../../../Model/anagrafiche/RapportoContabile';
import {BaseCodeDescr} from '../../../../Model/baseClass/baseCodeDescr';

export class ContattiKendo extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class ContattiGridService extends AbstractGridConfigService<ContattiEditServerResult> {
  gridId = 'Contatti';
  rowId = 'codice';

  isCreate: boolean;
  objParametriAgenda: ObjParametriAgenda;
  toolbar = new ToolbarSettings(true, false);
  defaultValueRapportoContabile: BaseCodeDescr[] = [];
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;
  kendoColumns: Array<KendoGridColumn>;
  view = new GridCustomizations();
  firstRun = true;
  kendoModel: ContattiEditGridModel = {
    descrizione: {
      editable: false,
      type: CELL_TYPES.STRING
    },
    codice: {
      editable: true,
      type: CELL_TYPES.DROPDOWNLIST
    },
    settore: {
      editable: false,
      type: CELL_TYPES.STRING
    },
    attivita: {
      editable: false,
      type: CELL_TYPES.STRING
    },
    aziendaProprietaria: {
      editable: false,
      type: CELL_TYPES.STRING
    },
    visibilitaPubblica: {
      editable: false,
      type: CELL_TYPES.STRING
    },
  };
  constructor(
    injector: Injector,
    protected masterService: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private contattiRootService: ContattiRootService,
    private translocoService: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
  ) {

    super(injector, ConfigTemplate.DefaultTemplate);

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.isCreate = true;
    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read ||
      this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update) {
      this.isCreate = false;
    }

    this.kendoColumns = [];

    if (this.isCreate) {
      this.kendoColumns.push(
        new KendoGridColumn(
          { field: "codice", title: this.translocoService.translate('RapportoContabile') },
          { resizable: true, editable: this.isCreate, width: 135, validators: [Validators.required] }
        ),
        new KendoGridColumn(
          { field: 'settore', title: this.translocoService.translate('Progressivo') },
          { resizable: true, editable: this.isCreate, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'attivita', title: this.translocoService.translate('Attività') },
          { resizable: true, editable: this.isCreate, width: 135 }
        )
      );
    }
    else {
      this.kendoColumns.push(
        new KendoGridColumn(
          { field: 'aziendaProprietaria', title: this.translocoService.translate('AziendaProprietaria') },
          { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'visibilitaPubblica', title: this.translocoService.translate('Tipo') },
          { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: "codice", title: this.translocoService.translate('RapportoContabile') },
          { resizable: true, editable: this.isCreate, width: 135, validators: [Validators.required] }
        ),
        new KendoGridColumn(
          { field: 'settore', title: this.translocoService.translate('Progressivo') },
          { resizable: true, editable: this.isCreate, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'attivita', title: this.translocoService.translate('Attività') },
          { resizable: true, editable: this.isCreate, width: 135 }
        )
      );
    }

    this.handleCustomizations();
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {

    switch (actionType) {
      case HttpAction.CREATE:
        break;
      case HttpAction.REMOVE:
        break;
      case HttpAction.UPDATE:
        break;
    }
    this.contattiRootService.gridRowsContattiSource.next(this.gridPublicService.value.data.rows)
    return of()
  }

  read(): Observable<ContattiEditServerResult> {

    if (this.isCreate) {

      let impostazione = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_DefaultRappContabile);

      return this.caricaRapportoContabileAPI()
        .pipe(map((risultato: any) => {
          this.handleDropdowns(risultato);

          const found = risultato.find((item: BaseCodeDescr) => item.codice.toString() === impostazione.Valore);

          if (found) {
            this.defaultValueRapportoContabile[0] = { codice: found.codice, descrizione: found.descrizione };
          }

          const currentRows = this.contattiRootService.getRowsContatti();
          if (!currentRows || currentRows.length === 0) {
            let startRow = {
              attivita: '',
              codice: this.defaultValueRapportoContabile[0].codice,
              descrizione: this.defaultValueRapportoContabile[0].descrizione,
              settore: ''
            };
            let startRows = [startRow];
            this.contattiRootService.setRowsContatti(startRows);
          }

          const finalRows = this.contattiRootService.getRowsContatti();

          return {
            model: this.kendoModel,
            columns: this.kendoColumns,
            rows: finalRows,
          };
        }));
    }

    const stream1 = this.contattiRootService.gridRowsContattiSource;

    return combineLatest([stream1, this.caricaRapportoContabileAPI()])
      .pipe(take(1), map((risultato: any[]) => {
        this.handleDropdowns(risultato[1])
        return {
          model: this.kendoModel,
          columns: this.kendoColumns,
          rows: risultato[0]
        };
      }));
  }

  handleDropdowns(ddlist: MetodoProduzione[]): void {
    const col = this.kendoColumns.find(s => s.field === 'codice');

    const data: DropdownListItem[] = ddlist.map(ddlItem => new DropdownListItem(ddlItem.codice, ddlItem.descrizione));

    col.ddl = new DropdownListWithForm('RapportoContabile', 'codice', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'descrizione';
  }

  caricaRapportoContabileAPI(): Observable<RapportoContabile> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<any, RapportoContabile>('MetaschemaNG/GetRapportiContabCodDescr',
      enum_RapportiContabili_SaCod.PersoneGiuridiche
    ).pipe(map((risposta: rispostaStandard<RapportoContabile>) => {

      if (risposta.RispostaOK) {
        return risposta.RispostaStringa;
      } else {
        alert("Qualcosa è andato storto.");
      }
    }));
  }

  private handleCustomizations() {
    // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
    this.cmdColumn.editBtn = false;
    this.cmdColumn.infoBtn = false;
    this.cmdColumn.removeBtn = this.isCreate;
    this.toolbar.newItem = this.isCreate;
    this.toolbar.resetChanges = false;
    this.views.enabled = false;
    // this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;

    // Setting generali
    this.columnMenu.columnMenu = false;
    this.columnMenu.filterable = false;
    this.behavior.saveExternalChanges = true;
    this.behavior.excelSettings = new ExcelSettings({ enabled: false });
    this.behavior.pdfSettings = new PDFSettings({ enabled: false });
    this.generalSettings.reordable = false;
    this.generalSettings.performOnEdit = true;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
  }
}
