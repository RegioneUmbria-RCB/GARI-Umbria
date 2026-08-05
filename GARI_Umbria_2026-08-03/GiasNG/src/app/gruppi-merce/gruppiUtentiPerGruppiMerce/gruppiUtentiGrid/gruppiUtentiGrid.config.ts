import { Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AgrSelectableSettings, DettagliColumnSettings } from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, KendoGridModel, LoaderType } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, map, Observable, of } from 'rxjs';
import { GruppiUtentiPerGruppiMerceSerivce } from '../gruppiUtentiPerGruppiMerce.service';
import { GruppiUtentiGridModel, GruppiUtentiGridRow, GruppiUtentiModel, GruppiUtentiRows, GruppiUtentiServerResult, GrUtPerGrMerceLinks, PublicServices, USE_MOCK_DATA } from '../utils';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import {HttpArgs, HttpService} from 'app/Service/http.service';


/**
 * TODO_RV:
 * Disable remove/edit butttons.
 */

@Injectable()
export class GruppiUtentiGridService extends AbstractGridConfigService<GruppiUtentiServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = 'chiave';
    gridId: string = 'GruppiUtentiGridService'

    protected columns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'Gruppi_Utente_des', title: this.translocoService.translate('Descrizione') })
    ];
    protected model: GruppiUtentiGridModel = GruppiUtentiModel;
    private rows: GruppiUtentiGridRow[] = GruppiUtentiRows;


    constructor(
        injector: Injector,
        private httpClient: HttpService,
        private masterPageService: GruppiUtentiPerGruppiMerceSerivce,
        private translocoService: TranslocoService,
        private ajaxAnagraficaAPIService: AjaxAgronicaAPIService)
    {
        super(injector);
        this.initializeGridBehavior();
        this.masterPageService.initializePubService(PublicServices.GruppiUtenti, this.gridPublicService);

        this.masterPageService.aggiornaGruppiMerceGrid.GiasSubscribe(_ => {
            if(!this.masterPageService.impreseCtrl.voceSelezionataMostraTutto())
                this.gridPublicService.refresh();
        });
    }

    initializeGridBehavior() {
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.selectable.checkboxOnly = true;

        this.cmdColumn.removeBtn = false;
        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;

        //this.resizable.autoFitColumns = true;

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: false
        });
    }

    read(options?: any): Observable<GruppiUtentiServerResult> {
        if(USE_MOCK_DATA)
            return of(new GruppiUtentiServerResult(this.model, this.columns, this.rows));

        const httpFn = (...args: HttpArgs) => {
            const agenda = args[0];
            const master = args[1];

            return {
                objParam_server: master.ObjParametri_Server,
                objParam_utenti: master.ObjParametri_Utenti,
            };
        }
        return this.ajaxAnagraficaAPIService.ajaxAPIGet<any, any>(GrUtPerGrMerceLinks.GRUPPI_UTENTI_LINK, {}).pipe(map(resp => {
            let rows: GruppiUtentiGridRow[] = resp.RispostaStringa;
            return new GruppiUtentiServerResult(this.model, this.columns, rows);
        }))

        /*return this.httpClient.post2<string>(GrUtPerGrMerceLinks.GRUPPI_UTENTI_LINK, httpFn)
            .pipe(map((json: string) => {
                let rows: GruppiUtentiGridRow[] = JSON.parse(json);
                return new GruppiUtentiServerResult(this.model, this.columns, rows);
        }));*/
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        throw new Error('Method not implemented.');
    }

}
