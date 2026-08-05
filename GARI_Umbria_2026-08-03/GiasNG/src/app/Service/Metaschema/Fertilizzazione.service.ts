import { Injectable } from '@angular/core';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { Effluente } from 'app/Model/metaschema/Effluente';
import { Epoca } from 'app/Model/metaschema/Epoca';
import { TipoAllevamento } from 'app/Model/metaschema/TipoAllevamento';
import { TipoFertilizzante } from 'app/Model/metaschema/TipoFertilizzante';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class LeggiEfficienza{

    tipoFertilizzante: TipoFertilizzante;

    epoca: Epoca;

    disciplinare: Disciplinare;

    effluente: Effluente;

    tipoAllevamento: TipoAllevamento;

    valoreDose: number;

    classiTessitura: ClasseTessitura[];

    specie: Specie;
}

@Injectable({
    providedIn: 'root'
})
export class FertilizzazioneService {

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private masterService: MasterService) { }

    /*LeggiEfficienza_PUA_2007_Old(p: LeggiEfficienza) {

        return new Promise<number>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiEfficienza> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<number, LeggiEfficienza>(this.masterService.link_CoreWS + '/Metaschema/Fertilizzazione.asmx/LeggiEfficienza_PUA_2007', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    LeggiEfficienza_PUA_2007(p: LeggiEfficienza) {
        return new Promise<number>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiEfficienza, number>('MetaschemaNG/LeggiEfficienzaPUA2007', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    /*LeggiEfficienza_Old(p: LeggiEfficienza) {

        return new Promise<number>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiEfficienza> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<number, LeggiEfficienza>(this.masterService.link_CoreWS + '/Metaschema/Fertilizzazione.asmx/LeggiEfficienza', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    LeggiEfficienza(p: LeggiEfficienza) {
        return new Promise<number>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiEfficienza, number>('MetaschemaNG/LeggiEfficienza', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }




}
