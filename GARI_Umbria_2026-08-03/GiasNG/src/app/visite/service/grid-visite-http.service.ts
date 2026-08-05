import { DatePipe } from '@angular/common';
import { ElementRef, Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_LAVCOD, enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { Elimina_Rilievi_Visita, LeggiVisite, VisiteService } from 'app/Service/Visite/visite.service';
import { RispostaStandard } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import {
  CommandsColumnSettings,
  CommandsDropDownSettings,
  DettagliColumnSettings,
  GroupSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  GridCustomizations,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Gias2010Redirector } from 'app/menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import { GridCommandItem, RibaltamentoTypes, TabTypes } from 'app/menu-agenda/components/utils';
import { filter, map, Observable, of, switchMap } from 'rxjs';
import { elimina_operazione_multipla } from 'app/menu-agenda/components/grid-qdc/qdc-config.service';
import { FiltersServiceVisite } from '../filters-visite/filters-visite.service';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { enum_visiteGridCommands } from '../utils';
import { MasterService } from "../../Service/master.service";
import { Tipo_Attivita } from 'gias-ui-kit';
import { BussinessMenuAgendaService } from 'app/menu-agenda/shared_services/bussiness-logic.service';

export class GridVisiteServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridVisiteHttpService extends AbstractGridConfigService<GridVisiteServerResult> {
  gridId = 'VisiteGridId';
  rowId = 'idVisiteGrid';
  GridVisiteServerResult: GridVisiteServerResult;
  GridVisite: any = {};
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  views = new GridCustomizations({ enabled: true });
  applicaFiltri: boolean;
  tabVisita: ElementRef;

  private permessoVisite_W: boolean;          //parametro su cui basare la visualizzazione del pulsante di modifica o meno
  private permessoCaricaDatiAPP: boolean;
  private permessoDocumentale_R: boolean;
  private permessoDocumentale_W: boolean;

  //rowOptions: Array<number> = [5, 25, 50, 100];            //array per la paginazione

  constructor(
    injector: Injector,
    public gridpublicService: GridPublicService,
    private translocoService: TranslocoService,
    private visiteService: VisiteService,
    private redirectService: Gias2010Redirector,
    private objParametriAgenda: ObjParametriAgendaService,
    private filters: FiltersServiceVisite,
    private permessiUtenteService: PermessiUtenteService,
    private datepipe: DatePipe,
    private dialogService: GiasDialogService,
    private masterService: MasterService,
    private bussinessLogic: BussinessMenuAgendaService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.permessoVisite_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Visite_Lista_NG, 2);
    this.permessoCaricaDatiAPP = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GiasAPP_VISITE, 2);
    this.permessoDocumentale_R = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Lista, 0);
    this.permessoDocumentale_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Inser, 2);

    this.setCustomizations();
    this.setCommands();
  }

  read(options?: any): Observable<GridVisiteServerResult> {
    this.applicaFiltri = false;
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    let param: LeggiVisite = this.getReadParams();

    return this.visiteService.leggiVisite(param)
      .pipe(map((r: any) => {
        let gridVisiteServerResult = new GridVisiteServerResult(
          this.setRowGridVisite(r),
          this.setColumnsGridVisite(r),
          JSON.parse(JSON.stringify(r)).kendo_model as KendoGridModel
        );
        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
        return gridVisiteServerResult;
      }));
  }

  setRowGridVisite(response) {
    let rows: Array<KendoGridRow> = [];
    for (let itemOfRows of response.kendo_rows) {
      itemOfRows['DaRemoto'] = itemOfRows['DaRemoto'] == 1 ? this.translocoService.translate('No') : this.translocoService.translate('Si');
      rows.push(itemOfRows);
    }
    return rows;
  }

  setColumnsGridVisite(response): Array<KendoGridColumn> {
    let columns: Array<KendoGridColumn> = [];
    for (let itemOfColumns of response.kendo_columns) {
      if (itemOfColumns.field == 'Azioni') continue;

      if (itemOfColumns.field == 'Veg_Des') {
        columns.push(new KendoGridColumn(
          { field: itemOfColumns.field, title: itemOfColumns.title },
          {
            resizable: true, editable: false, hidden: itemOfColumns.hidden,
            filterOrdering: (rows) => {
              const firstItem = rows.filter(item => item.Veg_Des == this.transloco.translate('NessunaSpecie'));
              const otherItems = rows.filter(item => item.Veg_Des != this.transloco.translate('NessunaSpecie'));
              otherItems.sort((a, b) => a.Veg_Des.localeCompare(b.Veg_Des));
              return [...firstItem, ...otherItems];
            }
          }
        ));
      } else {
        columns.push(new KendoGridColumn(
          { field: itemOfColumns.field, title: itemOfColumns.title },
          { resizable: true, editable: false, hidden: itemOfColumns.hidden }
        ));
      }
    }

    return columns;
  }

  setModelGridVisite() {
    const model: KendoGridModel = {
      Data_Movimento: new ModelEntry(CELL_TYPES.DATE),
      Piva: new ModelEntry(CELL_TYPES.STRING)
    };
    return model;
  }

  private deleteItem(rowsIn: KendoGridRow[] | KendoGridRow, proseguiInCasoDiAlert: boolean): Observable<any> {
    let rows: KendoGridRow[] = !rowsIn['length'] ? [rowsIn as KendoGridRow] : rowsIn as KendoGridRow[];
    let chiavi = this.datepipe.transform(rows[0]['Data_Movimento'] as Date, 'dd/MM/yyyy hh:mm:ss', undefined, undefined) + "_" + rows[0]['Id_Agenda'] + "_" + rows[0]['Lav_Cod'] + "_" + "0" + "_" + "0" + "_" + "0" + "_" + "0" + "_" + rows[0]['Piva'];
    const getParams: elimina_operazione_multipla = {
      strChiaviComposite: "del_elem|" + chiavi,
      proseguiInCasoDiAlert: proseguiInCasoDiAlert,
      variabiliInSessione_NG: this.masterService.variabiliInSessione
    }

    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

    return this.visiteService.deleteItem(getParams).pipe(
      switchMap((risposta: RispostaStandard | any) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        if (risposta.RispostaOK) {
          this.dialogService.baseSuccess('', this.transloco.translate('visite.VisitaCancellata', {}), false, false);
        } else if (risposta.RispostaConferma) {
          return this.dialogService.baseWarning('', risposta.Errore, false);
        } else {
          const errMsg = risposta.Errore ? risposta.Errore : this.transloco.translate("ImpossibileEliminareOperazione");
          this.dialogService.baseError('', errMsg, false);
        }
        return [];
      }),
      switchMap((result) => result['returnObj'] ? this.deleteItem(rows, true) : of([]))
    );
  }

  private showDialogConfirmDelete(msgParam: string) {
    let msg = this.transloco.translate(msgParam, {});
    return this.visiteService.createDialogWindow('', msg);
  }

  //funzione che viene richiamata quando si clicca sul bottone di delete della griglia
  perform(actionType: HttpAction, items: any[]): Observable<any[]> {
    switch (actionType) {
      case HttpAction.REMOVE:
        console.log(items);
        return this.deleteItem(items, false);
    }
    return of();
  }

  routingToNewOrEditVisita(editingModeVisita: boolean, data: any = null) {
    const paramsForRedirect = this.objParametriAgenda.getObjParamValue();
    this.redirectService.redirectToQdCfromMenuAgenda((editingModeVisita) ? data?.Id_Agenda : 0,
      0,
      (data) ? data.Piva : paramsForRedirect.Piva,
      (data) ? data.rag_soc : paramsForRedirect.RagSoc,
      enum_LAVCOD.VISITA,                          //lav_cod
      '',
      null,
      (editingModeVisita) ? Enum_DBTypeOperation.Update : Enum_DBTypeOperation.Write,
      TabTypes.QuadernoDiCampagna,
      new Date(),
      RibaltamentoTypes.Nessuno,
      paramsForRedirect.IdSezione);
  }

  getPermessoScritturaVisita() {
    return this.permessoVisite_W;
  }

  getPermessoCaricaDatiAPP() {
    return this.permessoCaricaDatiAPP;
  }

  private handleGridCommand() {
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case enum_visiteGridCommands.RICERCA_DOCUMENTI:
          const idAgendaRif = (ev.dataItem.Id_Agenda_Rif !== 0) ? ev.dataItem.Id_Agenda_Rif : ev.dataItem.Id_Agenda;
          this.visiteService.ApriKendoWindowRicercaDocumenti(idAgendaRif, ev.dataItem.Piva);
          break;

        case enum_visiteGridCommands.COPIA_VISITA:
          this.visiteService.copiaVisita(ev.dataItem);
          break;

        case enum_visiteGridCommands.CANCELLA_RILIEVO_ASSOCIATO:
          //this.visiteService.deleteRilievoVisita(ev.dataItem);
          this.deleteRilieviVisita(ev.dataItem, false).then();
          break;

        case enum_visiteGridCommands.VAI_A_RILIEVO:
          let objParam = this.objParametriAgenda.resettaObjAgenda(this.objParametriAgenda.getObjParamValue());
          objParam.Piva = ev.dataItem.Piva;
          objParam.RagSoc = ev.dataItem.rag_soc;
          objParam.Lav_Cod = ev.dataItem.Lav_Cod_Rif;
          objParam.Lav_Des = ev.dataItem.Lav_Des_Rif;
          objParam.Id_Agenda = ev.dataItem.Id_Agenda_Rif;
          objParam.TipoOperazioneDB = Enum_DBTypeOperation.Update;
          objParam.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
          objParam.Sa_Cod = ev.dataItem.Sa_Cod_Rif;
          objParam.Data = ev.dataItem.Validita_Inizio_Rilievo;
          objParam.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Visite;
          this.redirectService.gestisciRedirectToQdC(objParam).then();
          break;
      }
    })
  }


  private setButtonsVisibility() {
    this.gridPublicService.openCommands.GiasSubscribe(async attivita => {
      if (attivita) {
        this.cmdDropDown.removeAllCommand();

        if (this.permessoDocumentale_R) {
          this.bussinessLogic.CheckAttachedDocumentsOperationsAndVisits(attivita.Piva, attivita.Id_Agenda)
            .pipe(filter(hasAttachments => !!hasAttachments))
            .subscribe(() => this.cmdDropDown.addCommand(new GridCommandItem(
              "RicercaDocumenti",
              enum_visiteGridCommands.RICERCA_DOCUMENTI,
              'faQdCSearchDocument'
            )));
        }

        if (this.permessoVisite_W && this.permessoDocumentale_W && !this.cmdDropDown.cmdList.find(o => o.action === enum_visiteGridCommands.COPIA_VISITA)) {
          this.cmdDropDown.addCommand(new GridCommandItem(
            "visite.CopiaVisita",
            enum_visiteGridCommands.COPIA_VISITA,
            'faQdCSearchDocument'
          ));
        }
      }
    });
  }


  private async deleteRilieviVisita(param: any, proseguiInCasoDiAlert: boolean) {
    const paramToDelete: Elimina_Rilievi_Visita = {
      Piva: param.Piva,
      idAgendaRilievo: param.Id_Agenda_Rif,
      idAgendaVisita: param.Id_Agenda
    }
    let dialogResponse = await this.showDialogConfirmDelete("visite.ConfermaCancellazioneRilievo");

    if (dialogResponse['returnObj']) {
      this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
      this.visiteService.deleteRilievoVisita(paramToDelete)
        .subscribe((risposta: RispostaStandard | any) => {
          console.log(risposta);
          this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

          if (risposta.RispostaOK) {
            this.dialogService.baseSuccess('', this.transloco.translate('visite.RilievoCancellato', {}), false, false);
            this.gridPublicService.refresh(true);
          } else if (risposta.RispostaConferma) {
            return this.dialogService.baseWarning('', risposta.Errore, false);
          } else {
            const errMsg = risposta.Errore ? risposta.Errore : this.transloco.translate("ImpossibileEliminareOperazione");
            this.dialogService.baseError('', errMsg, false);
          }
        });
    }
  }

  public startLoadGrid() {
    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
  }

  public stopLoadGrid() {
    this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
  }

  private setCustomizations() {
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.selectable.shouldShowCheckbox = false;
    this.columnMenu.kendoGridColumnChooser = true;
    this.behavior.excelSettings.enabled = true;
    this.behavior.pdfSettings.enabled = false;
    this.generalSettings.performOnEdit = false;

    // this.generalSettings.height = 'auto';
    this.generalSettings.height = 450;

    // this.pagination.pageable = {
    //     buttonCount: 5,
    //     info: true,
    //     type: 'numeric',
    //     pageSizes: this.rowOptions,
    //     previousNext: true
    // };

    this.pagination.navigable = true;

    //test 7997
    // this.pagination.pageable = false;
    // this.pagination.scrollingType = ScrollingMode.Virtual;
    // this.pagination.gridState.skip = 0;
    // this.pagination.gridState.take = 100;
    // this.pagination.virtualScrolling.rowHeight = 36;
    // this.pagination.virtualScrolling.viewportHeight = 10;

    this.behavior.showDeletionConfirmation = true;
    this.groups = new GroupSettings({ groupable: { enabled: false, showFooter: false } }, this.translocoService);
    this.toolbar = new ToolbarSettings(false, false, 'Azioni', 150, '', false);
  }

  private getReadParams() {
    let param: LeggiVisite = new LeggiVisite();
    let filtersService = this.filters.filtersGetValue();

    if (filtersService.OperatoreVisita && filtersService.OperatoreVisita.Cod_RisUm != -1)
      param.operatore.Username = filtersService.OperatoreVisita.username;

    if (filtersService.AziendeVisita && filtersService.AziendeVisita.partitaIva != "-1")
      param.azienda = filtersService.AziendeVisita;

    if (filtersService.SpecieVisita && filtersService.SpecieVisita.hasOwnProperty("classType")) {
      let destUso = new DestinazioneUso();
      destUso.codice = filtersService.SpecieVisita.codice;
      destUso.descrizione = filtersService.SpecieVisita.descrizione;
      param.specie = destUso;
    } else if (filtersService.SpecieVisita && filtersService.SpecieVisita.codice != -1) {
      let varSpecie = new Varieta();
      varSpecie.specie = filtersService.SpecieVisita;
      param.specie = varSpecie;
    }

    if (filtersService.CentroAziendaleVisita && filtersService.CentroAziendaleVisita.primaryKey.partitaIva != "-1")
      param.centro_aziendale = filtersService.CentroAziendaleVisita;

    param.data_da = filtersService.Da;
    param.data_a = filtersService.A;

    if (filtersService.Impianti) {
      filtersService.Impianti.forEach(function (value) {
        param.impianti.push(value.chiave);
      });
    }

    param.operazioni = filtersService.TipoOperazione;
    if (!filtersService.TipoVisita) {
      param.tipoVisita = 0;
    } else if (filtersService.TipoVisita.key != -1) {
      param.tipoVisita = filtersService.TipoVisita.key;
    }

    param.risorsaZootecnica = filtersService.SpecieAnimaliVisita;
    //test per i dettagli del RIlievo, poi da collegare allo Switch
    param.withDettaglioRilievo = filtersService.WithDettaglioRilievo;
    return param;
  }

  private setCommands() {
    let filtersFormValues = this.filters.filtersGetValue();
    // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
    if (filtersFormValues.WithDettaglioRilievo) {
      this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
      this.toolbar = new ToolbarSettings(false, false);
      return;
    }

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
      onDisableInfoBtn: () => false,
      width: 10,
      title: 'Comandi'
    });
    this.dettagliColumn = new DettagliColumnSettings({
      editBtn: this.permessoVisite_W,
      edit: (data) => this.routingToNewOrEditVisita(true, data)
    });
    this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: false, removeBtn: false, infoBtn: false });

    if (this.permessoDocumentale_R) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "RicercaDocumenti",
        enum_visiteGridCommands.RICERCA_DOCUMENTI,
        'faQdCSearchDocument'
      ));
    }

    this.setButtonsVisibility();
    this.handleGridCommand();
  }

}