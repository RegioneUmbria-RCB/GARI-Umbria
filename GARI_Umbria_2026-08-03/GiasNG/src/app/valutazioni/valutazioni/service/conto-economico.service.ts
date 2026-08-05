import { Injectable } from "@angular/core";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { HttpService } from "app/Service/http.service";
import { MasterService, rispostaStandard } from "app/Service/master.service";
import { GridPublicService } from 'gias-kendo-grid';
import { Observable, tap, switchMap, of } from "rxjs";
import { LeggiDettaglio } from "./stato-patrimoniale.service";




@Injectable({ providedIn: 'root' })
export class ContoEconomicoService {

    gridPublicService: GridPublicService;

    constructor(private apiService: AjaxAgronicaAPIService,
        protected masterService: MasterService,
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxApiService: AjaxAgronicaAPIService,
        protected httpService: HttpService,
    ) { }


    public leggiContoEconomico(partitaIva: string, idTestata: number): Observable<any[]> {

        let leggiContoEconomico: LeggiDettaglio = {
            piva: partitaIva,
            idTestata: idTestata,
            contoCod: 0
        };

        return this.apiService.ajaxAPIPost<LeggiDettaglio, any[]>("Valutazioni/LeggiValutazioneDettaglioEcoGriglia", leggiContoEconomico).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }

}
