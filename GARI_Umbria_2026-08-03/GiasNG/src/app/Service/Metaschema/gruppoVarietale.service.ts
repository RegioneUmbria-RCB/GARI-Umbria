import { Injectable } from '@angular/core';
import { GruppoVarietale } from 'app/Model/metaschema/GruppoVarietale';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { SementieriParametrizzazione } from '../../Model/GIS/SementieriParametrizzazione';

export class GruppoVarietalexSpecie {
    Specie: Specie;
    GruppoVarietale: GruppoVarietale[];
}

export class LeggiGruppoVarietale {
    specie: Specie;
    ParametriSementieri: SementieriParametrizzazione
}

@Injectable({
    providedIn: 'root'
})
export class GruppoVarietaleService {
    private GruppoVarietalexSpecie: GruppoVarietalexSpecie[] = new Array();

    constructor(
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    ) { }

    leggi(specie: Specie){
        return new Promise<GruppoVarietale[]>(async (resolve, reject) => {
            if (this.GruppoVarietalexSpecie.find((el) => {
                if(el.Specie?.codice == specie.codice) {
                    return el;
                }
            })  == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiGruppoVarietale, GruppoVarietale[]>(
                    'MetaschemaNG/LeggiGruppoVarietale',
                    {specie: specie, ParametriSementieri: null}, false).pipe(map(R => {
                        
                        this.GruppoVarietalexSpecie.push({ Specie: specie, GruppoVarietale: R.RispostaStringa });
        
                        resolve(this.GruppoVarietalexSpecie.find((el) => {
                            if (el.Specie.codice == specie.codice) {
                                return el;
                            }
                        }).GruppoVarietale);

                    })).subscribe();


            } else {

                resolve(this.GruppoVarietalexSpecie.find((el) => {
                    if (el.Specie?.codice == specie.codice) {
                        return el;
                    }
                }).GruppoVarietale);

            }
        });
    }


}