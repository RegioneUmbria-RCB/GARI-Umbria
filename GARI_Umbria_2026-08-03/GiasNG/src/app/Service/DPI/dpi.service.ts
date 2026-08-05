import { Injectable } from '@angular/core';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
import {map, Observable} from "rxjs";

export class LeggiDisciplinari {
    lavorazioni: Lavorazione[];

    specie: Specie;

    data: Date;

    leggiPianoNutrizionale: boolean;
}

@Injectable({
    providedIn: 'root'
})

export class DpiService{

    private Disciplinari: Disciplinare[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private masterService: MasterService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

    // leggiDisciplinari_Old(specie: Specie,data: Date,
    //     flag_disciplinareprivato: boolean,regolamento: Regolamenti){
    //
    //     return new Promise<Disciplinare[]>(async (resolve, reject) => {
    //         if  (this.Disciplinari == undefined || this.Disciplinari.length == 0) {
    //             const parametri = {
    //                 objP_server: this.masterService.ObjParametri_Server,
    //                 objP_utenti: this.masterService.ObjParametri_Utenti,
    //                 veg_cod: specie.codice,
    //                 data: data,
    //                 flag_disciplinareprivato: flag_disciplinareprivato,
    //                 reg_cod: regolamento.codice
    //             };
    //             const R = await this.ajaxAgronicaService.ajaxAgronicaG<Disciplinare[]>(this.masterService.link_CoreWS + '/AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare_Modello', parametri);
    //             this.Disciplinari = R.RispostaStringa;
    //             resolve(this.Disciplinari);
    //         } else {
    //             resolve(this.Disciplinari);
    //         }
    //     });
    //
    // }

    leggiDisciplinari(specie: Specie,data: Date,
                      flag_disciplinareprivato: boolean,regolamento: Regolamenti){

        const leggiDisciplinari = new LeggiDisciplinari()
        leggiDisciplinari.lavorazioni = [];
        leggiDisciplinari.specie = specie;
        leggiDisciplinari.data = data;

        return new Promise<Disciplinare[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDisciplinari, Array<Disciplinare>>(
                'AgronicaCoreDPING/CaricaComboDisciplinareModello', leggiDisciplinari
            ).GiasSubscribe(R => {
                this.Disciplinari = R.RispostaStringa;
                resolve(this.Disciplinari);
            })
        });
    }

    /*Leggi_Disciplinari_Testata_conRegolamentoConcimazione_Old(p: LeggiDisciplinari){

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDisciplinari,Disciplinare[]>('Modello/Disciplinari/Leggi_Disciplinari_Testata_conRegolamentoConcimazione',p).pipe(map(r =>{
            return r.RispostaStringa;
        }))

        return Obs;
    }*/

    Leggi_Disciplinari_Testata_conRegolamentoConcimazione(p: LeggiDisciplinari){
        return new Promise<Disciplinare[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDisciplinari, Disciplinare[]>('AgronicaCoreDPING/Leggi_Disciplinari_Testata_conRegolamentoConcimazione', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    /*Leggi_Disciplinari_Testata_DirettivaNitrati_Old(p: LeggiDisciplinari){
        return new Promise<Disciplinare[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiDisciplinari> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Disciplinare[], LeggiDisciplinari>(this.masterService.link_CoreWS + '/AgronicaCoreDPI/DPI.asmx/Leggi_Disciplinari_Testata_DirettivaNitrati', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Disciplinari_Testata_DirettivaNitrati(p: LeggiDisciplinari){
        return new Promise<Disciplinare[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDisciplinari, Disciplinare[]>('AgronicaCoreDPING/Leggi_Disciplinari_Testata_DirettivaNitrati', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

}
