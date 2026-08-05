import { Inject, Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { DettagliColumnSettings } from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, LoaderType, NumericSettings } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable, of } from 'rxjs';
import { GruppiUtentiPerGruppiMerceSerivce } from '../gruppiUtentiPerGruppiMerce.service';
import { GruppiUtentixGruppiMerceGridModel, GruppiUtentixGruppiMerceGridRow, GruppiUtentixGruppiMerceModel, GruppiUtentixGruppiMerceRows, GruppiUtentixGruppiMerceServerResult, GrUtPerGrMerceLinks, PublicServices, ScrivPermessiHttpDto, ScriviPermessiResult, USE_MOCK_DATA as USE_STUB_DATA, CancellaPermessiHttpDto, TranslocoException } from '../utils';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import {HttpArgs, HttpService} from 'app/Service/http.service';



@Injectable()
export class GruppiUtentixGruppiMerceGridService extends AbstractGridConfigService<GruppiUtentixGruppiMerceServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = 'chiave';
    gridId: string = 'GruppiUtentixGruppiMerceGridService';
    msgOutputter: TranslocoException = new TranslocoException();

    protected columns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'rag_soc', title: this.translocoS.translate('RagioneSociale') }, {  }),
        new KendoGridColumn({ field: 'Gruppi_Utente_des', title: this.translocoS.translate('GruppoUtenteDescrzione') }),
        new KendoGridColumn({ field: 'CodiceGruppoMerce', title: this.translocoS.translate("GruppoMerceCodice")}, { numeric: new NumericSettings(), hidden: true, width: 230 }),
        new KendoGridColumn({ field: 'DescrizioneGruppoMerce', title: this.translocoS.translate('GruppoMerceDescrizione') }, { width: 560 }),
    ];
    protected model: GruppiUtentixGruppiMerceGridModel = GruppiUtentixGruppiMerceModel;
    private rows: GruppiUtentixGruppiMerceGridRow[] = GruppiUtentixGruppiMerceRows;

    get mostraTuttoCtrl()
    {
        return this.masterPageService.impreseCtrl;
    }

    constructor(
        injector: Injector,
        private httpService: HttpService,
        private masterPageService: GruppiUtentiPerGruppiMerceSerivce,
        private notify: GiasMessageService,
        private translocoS: TranslocoService,
        @Inject(LOADING_TOKEN) private loading: LoadingService,
        private permessiUtenteService: PermessiUtenteService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
        super(injector);
        this.initializeGridBehavior();
        this.masterPageService.initializePubService(PublicServices.GruppiUtentiPerGruppiMerce, this.gridPublicService);

        this.masterPageService.aggiornaGruppiMerceGrid.GiasSubscribe(_ => {
            this.msgOutputter.impresaSelezionataCambiata = true;

            this.gridPublicService.refresh(true);
        });

    }

    initializeGridBehavior() {


        let modificaAbilitata = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gruppi_Merce, 2);
        if(modificaAbilitata){
            this.cmdColumn.removeBtn = true;
            this.selectable.selectable.enabled = true;
            this.selectable.shouldShowCheckbox = true;
            this.selectable.selectable.checkboxOnly = true;
        }
        else {
            this.cmdColumn.removeBtn = false;
            this.selectable.selectable.enabled = false;
            this.selectable.shouldShowCheckbox = false;
        }

        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;

        this.resizable.autoFitColumns = true;

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: false
        });
    }

    read(options?: any): Observable<GruppiUtentixGruppiMerceServerResult> {
        if (USE_STUB_DATA)
            return of(new GruppiUtentixGruppiMerceServerResult(this.model, this.columns, this.rows));

        this.isLoading(true);
        return this.readData().pipe(map((data: any) => {
            let result: ScriviPermessiResult = JSON.parse(data);

            if (this.msgOutputter.showTranslocoMessage()) {
                let inizio = this.translocoS.translate("grMerci.SonoStatiImportati", {});
                let fine = this.transloco.translate("grMerci.Permessi", {});
                this.notify.infoMessagge(inizio + result.NumElementiInseriti + fine);
            }
            else {
                this.msgOutputter.resetAllFlags();
            }

            this.isLoading(false);
            return new GruppiUtentixGruppiMerceServerResult(this.model, this.columns, result.DT);
        }));
    }



    readData() {
        const getRigheSelezionate = () => {
            switch (true) {
                case this.msgOutputter.feedEmptyData():
                    return new ScrivPermessiHttpDto();
                default:
                    return this.masterPageService.prepareSelectedRowsForSending();
            }
        }

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(GrUtPerGrMerceLinks.GRUPPI_UTENTIMERCE_LINK, {
            righeSelezionate: getRigheSelezionate(),
            piva: this.masterPageService.impreseCtrl.selected.id}
        );
    }

    perform(actionType: HttpAction, permissions: any[]): Observable<any[]> {
        switch (actionType) {
            case HttpAction.REMOVE:
                return this.removePermissions(permissions);
            default:
                return of();
                // altri casi non gestiti
        }
    }

    removePermissions(permissions: GruppiUtentixGruppiMerceGridRow[]) {
        const params = (...args: HttpArgs): CancellaPermessiHttpDto => {
            let master: MasterService = args[1];
            return {
                permessi: Array.isArray(permissions) ? permissions : [permissions],
                objP_Server: master.ObjParametri_Server,
                objP_Utenti: master.ObjParametri_Utenti
            };
        };

        this.isLoading(true);
        return this.httpService.post2<any[]>(GrUtPerGrMerceLinks.CANCELLA_PERMESSO, params)
            .pipe(map((_) => {
                this.notify.infoMessagge("grMerci.PermessoCancellatoConSuccesso", false, true);
                this.msgOutputter.datiSonoStatiAppenaCancellati = true;

                this.isLoading(false);
                return [];
            }));
    }
    isLoading(onoff: boolean)
    {
        this.loading.set_isLoading({ isLoading: onoff, component: this.gridPublicService.gridElRef });
    }

}
