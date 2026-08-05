import { Injectable } from '@angular/core';
import {AjaxAgronicaAPIService} from "./ajax-agronica.api.service";
import {BaseCodeDescr} from "../Model/baseClass/baseCodeDescr";
import {map, take, Observable} from "rxjs";

export class CambioLinguaObj {
    constructor(
        public Username: string,
        public LinguaCod: number
    ) { }
}

@Injectable({
  providedIn: 'root'
})
export class LingueService {

    constructor(private apiService: AjaxAgronicaAPIService) { }

    public leggiLingue(): Observable<BaseCodeDescr[]> {
        return this.apiService.ajaxAPIGet('MetaschemaNG/LeggiLingue', '').pipe(
          take(1),
          map(R => R.RispostaOK ? R.RispostaStringa as BaseCodeDescr[] : [])
        );
    }

    public cambiaLinguaUtente(username: string, linguaCod: number) {
        return this.apiService.ajaxAPIPost(
            'Profilazione/ImpostaLingua',
            new CambioLinguaObj(username, linguaCod)
        );
    }
}
