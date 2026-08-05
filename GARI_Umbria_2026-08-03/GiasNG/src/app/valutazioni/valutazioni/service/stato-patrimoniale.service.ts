import { Injectable } from "@angular/core";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { HttpService } from "app/Service/http.service";
import { MasterService } from "app/Service/master.service";
import { GridPublicService } from 'gias-kendo-grid';
import { Observable, tap, switchMap, of } from "rxjs";

export class LeggiDettaglio {
    piva: string | undefined;
    idTestata: number | undefined;
    contoCod: number | undefined;
}

@Injectable({ providedIn: 'root' })
export class StatoPatrimonialeService {
    gridPublicService: GridPublicService;

    constructor(private apiService: AjaxAgronicaAPIService,
        protected masterService: MasterService,
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxApiService: AjaxAgronicaAPIService,
        protected httpService: HttpService,
    ) { }


    public leggiStatoPatrimoniale(partitaIva: string, idTestata: number): Observable<any[]> {

        let leggiStatoPatrimoniale: LeggiDettaglio = {
            piva: partitaIva,
            idTestata: idTestata,
            contoCod: 0
        };

        return this.apiService.ajaxAPIPost<LeggiDettaglio, any[]>("Valutazioni/LeggiValutazioneDettaglioPatGriglia", leggiStatoPatrimoniale).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }
}
