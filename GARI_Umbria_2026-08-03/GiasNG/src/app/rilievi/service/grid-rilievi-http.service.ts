import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { LeggiRilievi, MetaschemaNGClient, RispostaStandard } from "app/Service/api.service";
import { CommandsColumnSettings, CommandsDropDownSettings, GiasMessageService } from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, catchError, map, of, switchMap, takeUntil } from "rxjs";
import { FiltersRilieviService } from "../filters-rilievi/service/filters-rilievi.service";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { DestinazioneUso } from "app/Model/metaschema/utilizzi/DestinazioneUso";
import { Varieta } from "app/Model/metaschema/utilizzi/Varieta";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { Gias2010Redirector } from "app/menu-agenda/components/grid-qdc/Gias2010Redirector.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { Link_ElimOpMultipla, enum_menuAgendaGridCommands } from "app/menu-agenda/components/utils";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Tipo_Attivita} from 'gias-ui-kit';
import { DatePipe } from "@angular/common";
import { MasterService } from "app/Service/master.service";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { elimina_operazione_multipla } from "app/menu-agenda/components/grid-qdc/qdc-config.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { ConversionService } from "app/Service/conversion.service";

export class GridRilieviServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridRilieviHttpService extends AbstractGridConfigService<GridRilieviServerResult>{

    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;

    rowId: string = 'idRowRilieviGrid';
    gridId: string = 'RilieviGridId';

    objParametriAgenda: ObjParametriAgenda;

    private permessoQdC_W: boolean;

    private firstLoad: boolean = true;

    constructor(injector: Injector,
        private filterRilieviService: FiltersRilieviService,
        private gias2010Redirector: Gias2010Redirector,
        private permessiUtenteService: PermessiUtenteService,
        private agendaService: ObjParametriAgendaService,
        private conversionService: ConversionService,
        private translocoService: TranslocoService,
        private datepipe: DatePipe,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private dialogService: GiasDialogService,
        private rilieviService: MetaschemaNGClient,
        private giasMessageService: GiasMessageService,
        private masterService: MasterService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.permessoQdC_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);
        this.objParametriAgenda = this.agendaService.getObjParamValue();

        this.objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Rilievi;
        this.agendaService.changeObjParametriAgenda(this.objParametriAgenda);

        this.resizable.autoFitColumns = true;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: true,
            removeBtn: false,
            onDisableInfoBtn: () => false,
            width: 10
        });

        this.cmdDropDown = new CommandsDropDownSettings({
            removeBtn: false, infoBtn: false
        });

        this.filterRilieviService.filters.subscribe(value => {

            if (value) {                //evitiamo la prima chiamata alla lie finché non vengono inseriti i filtri
                this.firstLoad = false;
            }

            this.gridPublicService.refresh(true);
        });

        this.gridPublicService.changeDetected.pipe(
            takeUntil(this.signal)
        ).subscribe((event: any) => {
            //per bottone di info
            this.goToRilievo(event.action, event.dataItem);
        });

        this.handleCommands();

    }

    read(options?: any): Observable<GridRilieviServerResult> {

        if (this.firstLoad) {
          this.giasMessageService.infoMessagge(this.translocoService.translate('PopolareFiltriPerVisualizzareRilievi'));
          return of(new GridRilieviServerResult([], [], {}));
        }
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        let param = {
            AziendaRilievi: null,
            CentroAziendaleRilievi: null,
            TipoRilievi: null,
            SpecieRilievi:  null,
            Da: AGRODATAINIZIO,
            A: AGRODATAFINE
        } as LeggiRilievi;

        let filtersValue = this.filterRilieviService.filters.getValue();

        if (filtersValue) {

            if (filtersValue.AziendaRilievi)
              if (filtersValue.AziendaRilievi.partitaIva != "-1")
                param.AziendaRilievi = filtersValue.AziendaRilievi;

            if (filtersValue.CentroAziendaleRilievi)
                if (filtersValue.CentroAziendaleRilievi.primaryKey.partitaIva != "-1")
                    param.CentroAziendaleRilievi = filtersValue.CentroAziendaleRilievi;

            if (filtersValue.TipoRilievi)
              if (filtersValue.TipoRilievi.codice != -1)
                  param.TipoRilievi = filtersValue.TipoRilievi;

            if (filtersValue.SpecieRilievi) {
                if (filtersValue.SpecieRilievi.hasOwnProperty("classType")) {
                    let destUso = new DestinazioneUso();
                    destUso.codice = filtersValue.SpecieRilievi.codice;
                    destUso.descrizione = filtersValue.SpecieRilievi.descrizione;
                    param.SpecieRilievi = destUso;
                  } else {
                    if (filtersValue.SpecieRilievi.codice != -1) {
                      let varSpecie = new Varieta();
                      varSpecie.specie = filtersValue.SpecieRilievi;
                      param.SpecieRilievi = varSpecie;
                    }
                  }
            }
            // param.SpecieRilievi = filtersValue.SpecieRilievi;
            param.Da = filtersValue.Da;
            param.A = filtersValue.A;
        }

        return this.rilieviService.metaschemaNGLeggiRilievi(param).pipe(catchError((err) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
            return of();
        }), map((r:any) => {

            this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

            let kendoObj = this.conversionService.ConversionDateInObject(JSON.parse(r.RispostaStringa));

            let gridListaRilieviServerResult = new GridRilieviServerResult(
                kendoObj.kendo_rows,
                this.setColumnsFilterSorted(kendoObj.kendo_columns),
                kendoObj.kendo_model as KendoGridModel
            );

            return gridListaRilieviServerResult;
        }));

    }

    setColumnsFilterSorted(columns: KendoGridColumn[]): KendoGridColumn[] {
      columns.forEach((col: KendoGridColumn) => {
        if (col.field == 'Veg_Des') {
          col.filterOrdering = (rows) => {
            const firstItem = rows.filter(item => item.Veg_Des == this.transloco.translate('NessunaSpecie'));
            const otherItems = rows.filter(item => item.Veg_Des != this.transloco.translate('NessunaSpecie'));

            otherItems.sort((a, b) => a.Veg_Des.localeCompare(b.Veg_Des));

            return [...firstItem, ...otherItems];
          };
        }
      });

      return columns;

    }

    // goToEditPage() {

    // }

    private goToRilievo(operationType: string, dataItem: any) {

        let objParam =  this.agendaService.resettaObjAgenda(this.agendaService.getObjParamValue());

        objParam.Piva = dataItem.Piva;

        objParam.RagSoc = dataItem.rag_soc;

        objParam.Lav_Cod = dataItem.Lav_cod;

        objParam.Lav_Des = dataItem.Lav_Des;

        objParam.Id_Agenda = dataItem.id_agenda;

        objParam.TipoOperazioneDB = (operationType == 'info') ? Enum_DBTypeOperation.Read : Enum_DBTypeOperation.Update;

        objParam.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;

        objParam.Sa_Cod = dataItem.Sa_Cod;

        objParam.Data = dataItem.Validita_Inizio_Rilievo;

        objParam.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Rilievi;

        this.gias2010Redirector.gestisciRedirectToQdC(objParam).then();
    }

    private handleCommands() {
        this.gridPublicService.commandEvent.GiasSubscribe(ev => {
            if (!ev) return;
            switch (ev.command.action) {
                case enum_menuAgendaGridCommands.INFO:
                    this.goToRilievo('info', ev.dataItem);
                break;
            }
        });
    }

    private deleteItem(rowsIn: KendoGridRow[] | KendoGridRow, proseguiInCasoDiAlert: boolean): Observable<any> {

        let rows: KendoGridRow[] = [];
        if (!rowsIn['length']) {
          let row = rowsIn as KendoGridRow;
          rows.push(row);
        } else {
          rows = rowsIn as KendoGridRow[];
        }

        let chiavi = this.datepipe.transform(rows[0]['Data2'] as Date,'dd/MM/yyyy hh:mm:ss',undefined, undefined) + "_" + rows[0]['id_agenda'] + "_" + rows[0]['Lav_cod'] + "_" + "0" + "_" + "0" + "_" + "0" + "_" + "0" + "_" + rows[0]['Piva'];

        const getParams: elimina_operazione_multipla = {
          strChiaviComposite: "del_elem|" + chiavi,
          proseguiInCasoDiAlert: proseguiInCasoDiAlert,
          variabiliInSessione_NG: this.masterService.variabiliInSessione
        }

        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

        return this.ajaxAgronicaAPIService.ajaxAPIPost<elimina_operazione_multipla, string>(Link_ElimOpMultipla, getParams).pipe(switchMap((risposta: RispostaStandard | any) => {

          this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

          if (risposta.RispostaOK) {
            this.dialogService.baseSuccess('', this.transloco.translate('RilievoCancellato', { }), false, false);
          }
          else if(risposta.RispostaConferma) {
            return this.dialogService.baseWarning('', risposta.Errore, false);
          }
          else {
            const errMsg = risposta.Errore ? risposta.Errore : this.transloco.translate("ImpossibileEliminareOperazione");
            this.dialogService.baseError('', errMsg, false);
          }

          return [];
        }), switchMap((result)=> {
          if(result['returnObj'])
            return this.deleteItem(rows, true);
          return of([]);
        }));

        //}

        //return of([]);
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        switch(actionType) {
            case HttpAction.REMOVE:
              console.log(items);
              return this.deleteItem(items, false);
        }
          //return from(items);
        return of();
    }

}
