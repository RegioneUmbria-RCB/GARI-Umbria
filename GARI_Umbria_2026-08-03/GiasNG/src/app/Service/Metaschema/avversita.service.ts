import { Injectable } from '@angular/core';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import {Stati, Tipo_Attivita} from 'gias-ui-kit';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { Epoca } from 'app/Model/metaschema/Epoca';
import { Soglia } from 'app/Model/metaschema/Soglia';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import {LeggiProdotti} from '../Anagrafica/prodotti.service';
import {LeggiDisciplinari} from "../DPI/dpi.service";

export class LeggiAvversita {
    tipoAttivita: Tipo_Attivita;
    statoAttivita: Stati;
    lavorazione: Lavorazione;
    impianti: Impianto[];
    specie: Specie;
    disciplinare: Disciplinare;
    epocaDPI: Epoca;
    dettaglioTrattamento: DettaglioTrattamento;
    data: Date;
    avversitaGruppo: AvversitaGruppo;
    soglia: Soglia;
    appezzamenti: Appezzamento[];
    visualizzaMovimentiMagazzino: boolean;
    escludiGiacenzeZero: boolean;
    magazziniAgenzie: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class AvversitaService {

    constructor(
        private ajaxAgronicaService: AjaxAgronicaService,
        private masterService: MasterService,
        private APIService: AjaxAgronicaAPIService) { }

    /*Leggi_Avversita_QdC_Old(p: LeggiAvversita) {

        return new Promise<AvversitaGruppo[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiAvversita> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<AvversitaGruppo[], LeggiAvversita>(this.masterService.link_CoreWS + '/Metaschema/Avversita.asmx/Leggi_Avversita_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Avversita_QdC(p: LeggiAvversita) {
        return new Promise<AvversitaGruppo[]>(async (resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiAvversita, AvversitaGruppo[]>('MetaschemaNG/LeggiAvversitaQdC', p,true).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    public  LeggiAvversitaDistribuzioneInsetti(i: LeggiAvversita): Promise<AvversitaGruppo[]> {
        const url = 'Agenda/Avversita/Leggi_AvversitaInsettiUtili_QdC';

        return new Promise((resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiAvversita, AvversitaGruppo[]>(url, i, true)
                .GiasSubscribe(R => {
                    console.log(R);
                    resolve(R.RispostaStringa);
                });
        })
    }

    /*Leggi_SoglieAvversita_QdC(p: LeggiAvversita): Promise<Array<Soglia>> {

        return new Promise<Soglia[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiAvversita> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Soglia[], LeggiAvversita>(this.masterService.link_CoreWS + '/Metaschema/Avversita.asmx/Leggi_SoglieAvversita_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_SoglieAvversita_QdC(p: LeggiAvversita) {
        return new Promise<Soglia[]>(async (resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiAvversita, Soglia[]>('MetaschemaNG/LeggiSoglieAvversitaQdC', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    /*Controlla_Soglia_Avversita_Soddisfatta_QdC_Old(p: LeggiAvversita){

        return new Promise<string>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiAvversita> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<string, LeggiAvversita>(this.masterService.link_CoreWS + '/Metaschema/Avversita.asmx/Controlla_Soglia_Avversita_Soddisfatta_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Controlla_Soglia_Avversita_Soddisfatta_QdC(p: LeggiAvversita){
        return new Promise<string>(async (resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiAvversita, string>('MetaschemaNG/ControllaSogliaAvversitaSoddisfattaQdC', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    Leggi_Avversita_Inneschi(p: LeggiAvversita) {
      return new Promise<AvversitaGruppo[]>(async (resolve, reject) => {
        this.APIService.ajaxAPIPost<LeggiAvversita, AvversitaGruppo[]>('MetaschemaNG/LeggiAvversitaInneschiQdC', p,true).pipe(map(R => {
          resolve(R.RispostaStringa);
        })).subscribe();
      });
    }
}
