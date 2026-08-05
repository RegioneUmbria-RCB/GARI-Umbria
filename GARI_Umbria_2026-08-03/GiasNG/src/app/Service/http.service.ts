import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Observable, throwError, timer } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AjaxAgronicaService } from './ajax-agronica.service';
import { MasterService, rispostaStandard, RispostaStandard } from './master.service';
import { ObjParametriAgendaService } from './obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { AjaxAgronicaAPIService } from './ajax-agronica.api.service';

@Injectable({ providedIn: 'root' })
export class HttpService {

    constructor(private agenda: ObjParametriAgendaService,
        private master: MasterService,
        private ajaxService: AjaxAgronicaService,
        private ajaxAPIService: AjaxAgronicaAPIService) {

    }


    // public post<InType,OutType>(
    //     partialLink: string,
    //     data: any,
    //     pure: boolean = false,
    //     loading: boolean = true) {
    //
    //     const parametri: CoreWS_Generic<InType> = new CoreWS_Generic(
    //         this.master.getCoreWSGenericObjP(),
    //         data);
    //
    //     const result = this.ajaxService.ajaxCoreWSPost<InType, OutType>(this.getTarget(partialLink), parametri, loading)
    //         .pipe(map((risposta: rispostaStandard<OutType>) => this.manageResponse(risposta, pure)));
    //     return result;
    // }

    public post2<TOut>(
        partialLink: string,
        handleInput: (...params) => any = null,
        pure: boolean = false,
        gestisciErrore = true) {

        const args = [this.agenda, this.master];
        const request = handleInput?.call(handleInput, ...args);

        const result = this.ajaxService.post<TOut>(this.getTarget(partialLink), request, true, gestisciErrore)
            .pipe(map((risposta: rispostaStandard<TOut>) => this.manageResponse(risposta, pure)));
        return result;
    }

    /**
   * @deprecated Use get instead.
   */
    public getUsingAgenda<T>(partialLink: string,
        customizeAgenda: (agenda: ObjParametriAgenda) => ObjParametriAgenda = null,
        getRispostaStandard: boolean = true): Observable<RispostaStandard | T> {

        const agendaParams = this.agenda.getObjParamValue();
        const agenda = customizeAgenda?.call(customizeAgenda, agendaParams);
        const req = this.master.getCoreWSRequest(agenda);
        if(!req) {
            return;
        }

        return this.ajaxService.legacy_get(this.getTarget(partialLink), req)
            .pipe(map((risposta: RispostaStandard) => {
                if(getRispostaStandard) {
                    return risposta;
                }

                return JSON.parse(risposta.RispostaStringa) as T;
            }));
    }

    public post_legacy_API(partialLink: string,
        handleInput: (...params) => any = null,
        pure: boolean = true,
        displayDetailedErrorMessage: boolean = true) {

        const args = [this.agenda, this.master];
        const request = handleInput?.call(handleInput, ...args);

        return this.ajaxAPIService.ajaxAPIPost(partialLink, request, false, true, displayDetailedErrorMessage)
            .pipe(map((data) => {
                let risp: any = data;
                if(!pure && data.RispostaOK) {
                    risp = data.RispostaStringa;
                }
                return risp;
            }));

    }

    private getTarget(target) {
        return this.master.link_CoreWS + target;
    }


    private manageResponse<TOut>(risposta: rispostaStandard<TOut>, pure: boolean) {
        if(pure) {
            return risposta;
        }

        return risposta.RispostaStringa as TOut;
    }

}

export type Obs<T> = Observable<T>;
export type HttpArgs = [ObjParametriAgendaService, MasterService];
