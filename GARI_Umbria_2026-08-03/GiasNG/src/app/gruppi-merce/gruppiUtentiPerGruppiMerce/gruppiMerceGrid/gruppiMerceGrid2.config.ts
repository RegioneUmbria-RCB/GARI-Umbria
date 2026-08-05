import { Inject, Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { DettagliColumnSettings } from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, KendoServerResult, LoaderType, NumericSettings } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable, of } from 'rxjs';
import { GruppiUtentiPerGruppiMerceSerivce } from '../gruppiUtentiPerGruppiMerce.service';
import { GruppiMerceGridModel, GruppiMerceGridRow, GruppiMerceModel, GruppiMerceRows, GruppiMerceServerResult, GrUtPerGrMerceLinks, PublicServices, USE_MOCK_DATA } from '../utils';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import {HttpArgs, HttpService} from 'app/Service/http.service';


@Injectable()
export class GruppiMerceGrid2Service extends AbstractGridConfigService<KendoServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = 'chiave';
    gridId: string = 'GruppiMerceGridService'

    protected columns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'Codice', title: this.translocoService.translate('Codice') }, { numeric: new NumericSettings(), width: 150 }),
        new KendoGridColumn({ field: 'Descrizione', title: this.translocoService.translate('Descrizione') })
    ];
    protected model: GruppiMerceGridModel = GruppiMerceModel;
    private rows: GruppiMerceGridRow[] = GruppiMerceRows;


    constructor(
        injector: Injector,
        private httpClient: HttpService,
        private masterPageService: GruppiUtentiPerGruppiMerceSerivce,
        private translocoService: TranslocoService,
        @Inject(LOADING_TOKEN) private loading: LoadingService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService
    ) {
        super(injector);
        this.initializeGridBehavior();
        this.masterPageService.initializePubService(PublicServices.GruppiMerce, this.gridPublicService);


        this.masterPageService.aggiornaGruppiMerceGrid.GiasSubscribe(_ => {
            if (!this.masterPageService.impreseCtrl.voceSelezionataMostraTutto())
                this.gridPublicService.refresh(true);
        });
    }

    initializeGridBehavior() {
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.selectable.checkboxOnly = true;

        this.cmdColumn.removeBtn = false;
        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;

        // this.resizable.autoFitColumns = true;

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: false
        });
    }

    read(options?: any): Observable<GruppiMerceServerResult> {
        if (USE_MOCK_DATA)
            return of(new GruppiMerceServerResult(this.model, this.columns, this.rows));

        this.isLoading(true);
        const httpFn = (...args: HttpArgs) => {
            const agenda = args[0];
            const master = args[1];

            return {
                objParam_server: master.ObjParametri_Server,
                objParam_utenti: master.ObjParametri_Utenti,
                piva: this.masterPageService.impreseCtrl.selected.id
            };
        }
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(GrUtPerGrMerceLinks.GRUPPI_MERCE_LINK, {piva: this.masterPageService.impreseCtrl.selected.id}).pipe(map(resp => {
            let rows: GruppiMerceGridRow[] = resp.RispostaStringa;
            this.isLoading(false);

            return new GruppiMerceServerResult(this.model, this.columns, rows);
        }))

        /*return this.httpClient.post2<string>(GrUtPerGrMerceLinks.GRUPPI_MERCE_LINK, httpFn)
            .pipe(map((json: string) => {
                let rows: GruppiMerceGridRow[] = JSON.parse(json);
                this.isLoading(false);

                return new GruppiMerceServerResult(this.model, this.columns, rows);
            }));*/

    }

    isLoading(onoff: boolean) {
        this.loading.set_isLoading({ isLoading: onoff, component: this.gridPublicService.gridElRef });
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return of([]);
    }
}
