import { Injectable } from "@angular/core";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { map, Observable } from "rxjs";


export class LeggiFiltro
{
  Ricerca: string
}

@Injectable({ providedIn: 'root' })
export class ImpreseFilterService {
    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) {

    }

    /*filtraImprese_Old(filtroRicerca: string): Observable<Impresa[]> {
        const parametri: CoreWS_Generic<string> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: filtroRicerca
        }


        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<Impresa[], string>(
            this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Imprese_Filtro',
            parametri).pipe(
                map((resp) => {
                    return resp.RispostaStringa;
                })
            );
    }*/

    filtraImprese(filtroRicerca: string): Observable<Impresa[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiFiltro, Impresa[]>(
            'AnagraficaNG/LeggiImpreseFiltro',
          {Ricerca: filtroRicerca }).pipe(
                map((resp) => {
                    return resp.RispostaStringa;
                })
            );
    }

}
