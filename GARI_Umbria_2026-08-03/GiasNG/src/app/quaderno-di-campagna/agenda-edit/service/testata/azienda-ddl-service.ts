import { Injectable } from "@angular/core";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { map } from "rxjs";

export class LeggiAziende {
    usernameOperatore: string;

    constructor(operatore: string) {
        this.usernameOperatore = operatore;
    }
}

@Injectable({providedIn: 'root'})
export class AziendaDDLService{
    
    constructor(
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService
    ) {}
    
    leggiListaAziende(param: LeggiAziende) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAziende, string>('Visite/LeggiListaAziende', param).pipe(map(r =>{
            return r.RispostaStringa;
        }));
    }

    leggiListaAgenzie(param: LeggiAziende) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAziende, string>('Visite/LeggiListaAgenzie', param).pipe(map(r =>{
            return r.RispostaStringa;
        }));
    }

}