import { Injectable } from '@angular/core';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { MasterService } from '../master.service';
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
import {map, Observable, of} from "rxjs";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";
import {Vincolo} from "../../Model/metaschema/Vincoli";


export class LeggiVincoli {
    validita: IntervalloTemporale;
}

export class VincolixData {
    chiave: string;
    valori: Vincolo[];
}

@Injectable({
    providedIn: 'root'
})

export class VincoliService{

    private vincolixDate: VincolixData[] = [];

    constructor(private masterService: MasterService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
    }

    leggiVincoli(validita: IntervalloTemporale): Observable<Vincolo[]>{
        let chiave = JSON.stringify(validita);
        let vincoloxData = this.vincolixDate.find((el) =>  {return el.chiave == chiave });
        if (vincoloxData) {
            return of(vincoloxData.valori);
        }
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiVincoli, Vincolo[]>(
            'AnagraficaNG/LeggiVincoli',
        {
                    validita:validita
                }).pipe(
            map(el => {
              if (el.RispostaStringa){
                this.vincolixDate.push({
                  chiave: chiave,
                  valori: el.RispostaStringa
                })
              }
              return el.RispostaStringa;
            })
        );


    }

    leggiVincoliOrdered(validita: IntervalloTemporale): Observable<Vincolo[]>{
        let chiave = JSON.stringify(validita);
        let vincoloxData = this.vincolixDate.find((el) =>  {return el.chiave == chiave });
        if (vincoloxData) {
            return of(vincoloxData.valori);
        }
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiVincoli, Vincolo[]>(
            'AnagraficaNG/LeggiVincoliOrdered',
        {
                    validita:validita
                }).pipe(
            map(el => {
              if (el.RispostaStringa){
                this.vincolixDate.push({
                  chiave: chiave,
                  valori: el.RispostaStringa
                })
              }
              return el.RispostaStringa;
            })
        );


    }

}
