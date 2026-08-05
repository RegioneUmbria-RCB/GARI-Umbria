import { Injectable } from '@angular/core';
import {AjaxAgronicaAPIService} from "../../../../Service/ajax-agronica.api.service";
import {Observable, map, tap, of} from "rxjs";
import {Specie} from "../../../../Model/metaschema/utilizzi/Specie";
import {BaseCodeDescr, Disciplinare} from "../../../../Service/api.service";

class LeggiMisureAvversita {
  public piva: string = "";
  public vegCod: number = 0;
  public dpi: Disciplinare = null;
  public udmCod: number = 0;
  public avvCod: number = 0;
}

@Injectable({
  providedIn: 'root'
})
export class MisureIndiciMaturitaAnagraficaService {

  constructor(private APIService: AjaxAgronicaAPIService) { }

  public getMisureIndiciMaturita(indMat: number, udmCod: number, specie: Specie): Observable<BaseCodeDescr[]> {

    const params =  this.getRequestParams(indMat, udmCod, specie);
    return this.APIService.ajaxAPIPost<LeggiMisureAvversita, any[]>(
      "Agenda/Rilievi/indiciMaturitaAnagrafiche", params
    ).pipe(
      map(R => R.RispostaOK ? R.RispostaStringa : [])
    );
  }

  private getRequestParams(indMat: number, udmCod: number, specie: Specie): LeggiMisureAvversita {
    const p = new LeggiMisureAvversita();
    p.avvCod = indMat;
    p.udmCod = udmCod;
    p.vegCod = specie.codice;
    return p;
  }

}
