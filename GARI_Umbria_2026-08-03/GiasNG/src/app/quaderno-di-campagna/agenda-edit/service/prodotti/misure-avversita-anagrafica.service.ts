import { Injectable } from '@angular/core';
import {AjaxAgronicaAPIService} from '../../../../Service/ajax-agronica.api.service';
import {MisuraAvversita} from '../../../../Model/metaschema/avversita/MisuraAvversita';
import {forkJoin, map, Observable, of, Subject, tap} from 'rxjs';
import {Avversita} from '../../../../Model/metaschema/avversita/Avversita';
import {distinct} from '@progress/kendo-data-query';
import {Disciplinare} from "../../../../Model/metaschema/Disciplinari";

export class LeggiMisuraPerAvversitaAnagrafica_In {
    public codice: number = 0;
    public disciplinare: Disciplinare = null;

    constructor(codice: number, disciplinare: Disciplinare) {
        this.codice = codice || 0;
        this.disciplinare = disciplinare || null;
    }
}

@Injectable({
  providedIn: 'root'
})
export class MisureAvversitaAnagraficaService {

    constructor(
        private APIService: AjaxAgronicaAPIService,
    ) { }

    /**
     * Restituisce i record relativi alle misure avversità.
     * @param MxAv_Cod il codice MxAv_Cod delle {@link Avversita}
     */
    public getMisureAvversita(MxAv_Cod: number,disciplinare: Disciplinare): Observable<MisuraAvversita[]> {
        return this.readMisuraAvversitaAnagrafica(MxAv_Cod, disciplinare);
    }

    /**
     * Restituisce i record relativi alle misure avversità per le avversità specificate.
     * @param avversita le avversità di cui si desidera conoscere le misure
     */
    public getMisureFromAvversita(avversita: Avversita[],disciplinare: Disciplinare) {
        const sub = new Subject<any>();
        const p: Observable<any>[] = [];
        let codici: number[] = distinct(avversita.map(a => a.MxAV_Cod))
            .filter(cod => !!cod);
        codici.forEach(cod => p.push((this.getMisureAvversita(cod,disciplinare))));
        forkJoin(p).subscribe(res => {
            sub.complete();
        })
        return sub;
    }

    private readMisuraAvversitaAnagrafica(MxAv_Cod: number, disciplinare: Disciplinare): Observable<MisuraAvversita[]> {
        const params = new LeggiMisuraPerAvversitaAnagrafica_In(MxAv_Cod,disciplinare);
        return this.APIService.ajaxAPIPost<LeggiMisuraPerAvversitaAnagrafica_In, any>(
            "Agenda/LeggiMisurePerAvversitaAnagrafiche", params
        ).pipe(map(R =>  R.RispostaOK ? R.RispostaStringa.ListaMisure : []));
    }


}
