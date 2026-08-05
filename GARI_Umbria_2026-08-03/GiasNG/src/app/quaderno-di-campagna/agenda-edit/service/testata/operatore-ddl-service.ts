import { Injectable } from "@angular/core";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { map } from "rxjs";

@Injectable({providedIn: 'root'})
export class OperatoreDDLService{
    
    constructor(
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService
    ) {}

    leggiTecnicoOCapo() {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, string>('Visite/LeggiUtenteTecnicoOCapo', "").pipe(map(r =>{
            return r.RispostaStringa;
        }));
    }

    leggiListaTecnici() {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, string>('Visite/LeggiListaTecnici', "").pipe(map(r =>{
            return r.RispostaStringa;
        }));
    }
    
}