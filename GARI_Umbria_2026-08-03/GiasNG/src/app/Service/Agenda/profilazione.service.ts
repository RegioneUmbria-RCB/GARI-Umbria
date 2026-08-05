import { Injectable } from '@angular/core';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard } from '../master.service';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {map, Observable} from 'rxjs';

export class LeggiProfilazione{

    impresa: Impresa;

    operazioni: Array<Lavorazione>;

    specie: Specie;

    data: Date;

    macchine: Array<ParcoMacchine>;

    contatti: Array<Contatto>;

}


@Injectable({
    providedIn: 'root'
})
export class ProfilazioneService {

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private masterService: MasterService) { }


    /*CaricaDefaultMacchine_QdC_Old(p:LeggiProfilazione) {

        return new Promise<any>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProfilazione> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<string, LeggiProfilazione>(this.masterService.link_CoreWS + '/Agenda/Profilazione.asmx/CaricaDefaultMacchine', parametri);

            resolve(R.RispostaStringa);
        });

    }*/

    CaricaDefaultMacchine_QdC(p:LeggiProfilazione) {

        return new Promise<any>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiProfilazione, string>('Agenda/CaricaDefaultMacchine', p).pipe(map(R => {
                resolve(R.RispostaStringa);})).subscribe();
        });

    }

    /*CaricaDefaultOperatori_QdC_Old(p:LeggiProfilazione) {

        return new Promise<any>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProfilazione> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<string, LeggiProfilazione>(this.masterService.link_CoreWS + '/Agenda/Profilazione.asmx/CaricaDefaultOperatori', parametri);

            resolve(R.RispostaStringa);
        });

    }*/

    CaricaDefaultOperatori_QdC(p:LeggiProfilazione) {

        return new Promise<any>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiProfilazione, string>('Agenda/CaricaDefaultOperatori', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });

    }

    /*CaricaRisorseMacchine_QdC_Old(p:LeggiProfilazione){

        return new Promise<any>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProfilazione> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<any, LeggiProfilazione>(this.masterService.link_CoreWS + '/Agenda/Profilazione.asmx/CaricaRisorseMacchine', parametri);

            resolve(R.RispostaStringa);
        });

    }*/

    CaricaRisorseMacchine_QdC(p:LeggiProfilazione):Observable<any>{
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiProfilazione, any>('Agenda/CaricaRisorseMacchine', p);
    }

    /*CaricaRisorseOperatori_QdC_Old(p:LeggiProfilazione){

        return new Promise<any>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProfilazione> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<any, LeggiProfilazione>(this.masterService.link_CoreWS + '/Agenda/Profilazione.asmx/CaricaRisorseOperatori', parametri);

            resolve(R.RispostaStringa);
        });

    }*/

    CaricaRisorseOperatori_QdC(p:LeggiProfilazione){

        return new Promise<any>(async (resolve, reject) => {

            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiProfilazione, any>('Agenda/CaricaRisorseOperatori', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();

        });

    }

    /*Scrivi_Profilazione_Dati_QdC_Old(p:LeggiProfilazione){
        return new Promise<any>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProfilazione> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<any, LeggiProfilazione>(this.masterService.link_CoreWS + '/Agenda/Profilazione.asmx/Scrivi_Profilazione_Dati', parametri);

            resolve(R);
        });
    }*/

    Scrivi_Profilazione_Dati_QdC(p:LeggiProfilazione){
        return new Promise<any>(async (resolve, reject) => {

            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiProfilazione, any>('Agenda/Scrivi_Profilazione_Dati', p).pipe(map(R => {
                resolve(R);
            })).subscribe();

        });
    }

}
