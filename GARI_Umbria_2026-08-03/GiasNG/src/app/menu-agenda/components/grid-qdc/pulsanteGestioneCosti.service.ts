import { ElementRef, Renderer2 } from '@angular/core';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_LAVCOD, enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { isNullOrUndefined, NumToStr } from 'app/Service/utils';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { map, Observable, of } from 'rxjs';
import { MenuAgendaDataStore } from '../../shared_services/menu-agenda-datastore.service';
import { QdCRow } from '../utils';
import { ObjParametriAgenda } from 'gias-ui-kit';


interface Dependencies {
    dataStore: MenuAgendaDataStore;
    agenda: ObjParametriAgendaService;
    notifications: GiasMessageService;
    gestioneRichieste: GestioneRichiesteService;
    renderer: Renderer2;
    ajaxAgronicaAPIService: AjaxAgronicaAPIService;
    objParametriAgendaService: ObjParametriAgendaService;
};

interface ApplicaStili {
    qdcGridRef: ElementRef;
    domElems: any;
    rows: QdCRow[];
}

const links = {
    //OperazioniVisibili_Old: "Agenda/Agenda.asmx/CaricaGrigliaOperazioni",
    OperazioniVisibili: "Agenda/CaricaGrigliaOperazioni",
    CostiCollegati: "Agenda/Agenda.asmx/OperazioneAgenda"
};

export class PulsanteGestioneCostiService {

    /**
     * Services
     */
    private dataStore: MenuAgendaDataStore;
    private agenda: ObjParametriAgendaService;
    private notifications: GiasMessageService;
    private gestioneRichieste: GestioneRichiesteService;
    private renderer: Renderer2;
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService;
    private objParametriAgendaService: ObjParametriAgendaService;

    /**
     * Global variables
     */
    private alreadyElaboratedRowIds = "";

    constructor(deps: Dependencies) {
        this.dataStore = deps.dataStore;
        this.agenda = deps.agenda;
        this.notifications = deps.notifications;
        this.gestioneRichieste = deps.gestioneRichieste;
        this.renderer = deps.renderer;
        this.ajaxAgronicaAPIService = deps.ajaxAgronicaAPIService;
        this.objParametriAgendaService = deps.objParametriAgendaService;
    }

    /**
     * Getters/Setters
     */
    get lavCodOperazioniVisibili(): number[] {
        return this.dataStore.lavCodOpVisibili;
    }

    /**
     * Public API
     */

    public applyStyle(deps: ApplicaStili) {
        this._applyStylePolicy(deps);
    }

    public getCostiStyleClass(row: QdCRow): Promise<string>{
        return new Promise((resolve, reject) => {
            this.getOperazioniVisibiliDeiCosti().GiasSubscribe(lavList => {
                this.getCostiCollegatiFor(row.id_agenda).GiasSubscribe( costi => {
                    if (!costi.length && !this.erroriRiscontrati(row)) {
                        const css: string = this.getBtnStyle(costi, row.id_agenda);
                        resolve(css);
                    }
                    resolve('');
                });
            });
        });
    }

    public vaiAiCosti(row: QdCRow) {
        this._vaiAiCostiPolicy(row);
    }

    /**
     * Functions used by the public API.
     */

    private _applyStylePolicy(deps: ApplicaStili) {
        this.getOperazioniVisibiliDeiCosti().subscribe((costi: number[]) => {
            const rowIdsAwaitingElaboration = this.hideBtns(deps.domElems, deps.rows);

            this.setEachButtonAStyle(rowIdsAwaitingElaboration, deps.rows, deps.domElems);
            this.alreadyElaboratedRowIds = "";
        });
    }

    /*private getOperazioniVisibiliDeiCosti_Old(): Observable<number[]> {

        if(!isNullOrUndefined(this.lavCodOperazioniVisibili))
            return of(this.lavCodOperazioniVisibili);

        const params = (...args: HttpArgs) => {
            const master = args[1];
            return {
                filtroAggiuntivo: this.getFiltroAggiuntivoOpVisibili(),
                objP_server: master.ObjParametri_Server
            };
        };

        return this.http.post2(links.OperazioniVisibili, params.bind(this), true)
            .pipe(map((risultato: RispostaStandard) => {
                const rows: any[]= JSON.parse(risultato.RispostaStringa);
                const opVisibili = rows.map((row) => row.LAV_COD);
                this.dataStore.lavCodOpVisibili = opVisibili;
                return this.lavCodOperazioniVisibili;
            }));
    }*/

    private getOperazioniVisibiliDeiCosti(): Observable<number[]> {

        if(!isNullOrUndefined(this.lavCodOperazioniVisibili))
            return of(this.lavCodOperazioniVisibili);

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any[]>(links.OperazioniVisibili, this.getFiltroAggiuntivoOpVisibili())
            .pipe(map((risultato) => {
                const rows: any[]= risultato.RispostaStringa;
                const opVisibili = rows.map((row) => row.LAV_COD);
                this.dataStore.lavCodOpVisibili = opVisibili;
                return this.lavCodOperazioniVisibili;
            }));
    }

    private getFiltroAggiuntivoOpVisibili() {
        return "LAV_COD < 1000 AND (GRU_OP in (3,4,5) or Lav_cod = " +
            + enum_LAVCOD.RACCOLTA + ")";
    }

    private setEachButtonAStyle(rowIdsAwaitingElaboration: string,
        rows: QdCRow[], domElems: any) {

        if(!this.expensiveOperationToDisplay(rows.length) &&
           !this.workAlreadyFinishedFor(rowIdsAwaitingElaboration)) {

            this.alreadyElaboratedRowIds = rowIdsAwaitingElaboration;

            this.getCostiCollegatiFor(rowIdsAwaitingElaboration)
                .subscribe((costi: any[])=> {
                    if(costi.length > 0) {
                        rows.forEach((row: QdCRow, index) => {
                            if(!this.erroriRiscontrati(row)) {
                                const css = this.getBtnStyle(costi, row.id_agenda);

                                const elem = domElems[index];
                                UtilityFunctions.addClass(this.renderer, elem, ".btnCosti", css);
                            }
                        });

                    }
                });
        }
    }

    private expensiveOperationToDisplay(numRigheVisibile: number) {
        const threshold = 100;
        return numRigheVisibile > threshold;
    }

    private workAlreadyFinishedFor(rowIds: string) {
        return rowIds === this.alreadyElaboratedRowIds;
    }

    /*private getCostiCollegatiFor_Old(rowIdsAwaitingElaboration: string) {
        const params = (...args: HttpArgs): any => {
            const agenda = args[0];
            const master = args[1];
            return {
                Piva: agenda.getObjParamValue().Piva,
                listaIdAgenda: rowIdsAwaitingElaboration,
                objP_server: master.ObjParametri_Server
            };
        };
        return this.http.post2(links.CostiCollegati, params)
            .pipe(map((risposta: string) => {
                return JSON.parse(risposta);
            }));
    }*/

    private getCostiCollegatiFor(rowIdsAwaitingElaboration: string) {

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any[]>('Agenda/OperazioneAgenda', {
            Piva: this.objParametriAgendaService.getObjParamValue().Piva,
            listaIdAgenda: rowIdsAwaitingElaboration,
            })
            .pipe(map(risposta => {
                return risposta.RispostaStringa;
            }));
    }


    private getBtnStyle(costi: any[], idAgenda: string) {
        for(let costo of costi) {
            if(idAgenda === costo.Id_Agenda.toString()) {
                if(costo.EsistonoCostiCollegatiCDG)
                    return "btn-black";
            }
        }
        return "btn-warning";
    }

    private hideBtns(domElems: any, rows: QdCRow[]): string {
        let rowIdsAwaitingElaboration = "";
        rows.forEach((dataItem: QdCRow, index: number) => {
            let currDomRow = domElems[index];

            if(!this.operazioneVisibile(dataItem)) {
                UtilityFunctions.hideElem(this.renderer, currDomRow, '.btnCosti');
            } else {
                UtilityFunctions.showElem(this.renderer, currDomRow, '.btnCosti');
            }

            if (rowIdsAwaitingElaboration !== "") {
                rowIdsAwaitingElaboration += ",";
            }

            rowIdsAwaitingElaboration += dataItem.ID;
        });
        return rowIdsAwaitingElaboration;
    }

    /****/

    private _vaiAiCostiPolicy(row: QdCRow) {
        if (this.erroriRiscontrati(row)) {
            this.notifications.errorMessage('impossibileInserireCosti', false, true);
            return;
        }

        this.vaiAiCostiDaGestioneRichieste(row);
    }

    private erroriRiscontrati(row: QdCRow): boolean {
        return +row.Lav_cod > 1000 || !this.operazioneVisibile(row);
    }

    private vaiAiCostiDaGestioneRichieste(row: QdCRow) {
        let objAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.agenda.getObjParamValue()));
        objAgenda.Id_Agenda = +row.id_agenda;
        objAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;

        const parametri: ParametriAggiuntivi_QueryString[] = [
            KeyValuePair.Create("p", row.Piva,true),
            KeyValuePair.Create("id_agenda", row.id_agenda,true),
            KeyValuePair.Create("entrata_diretta", NumToStr(0),true)
        ];
        this.gestioneRichieste.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_Gestione_Costi,
            parametri,
            objAgenda).then(resp => window.location.href = resp);
    }


    /** Utility functions */
    private operazioneVisibile(dataItem: QdCRow) {
        return this.lavCodOperazioniVisibili.includes(+dataItem.Lav_cod);
    }
}
