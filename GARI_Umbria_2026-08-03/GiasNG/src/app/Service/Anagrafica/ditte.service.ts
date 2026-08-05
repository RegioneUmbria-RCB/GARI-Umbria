import {Injectable} from "@angular/core";
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
import {map} from "rxjs";
import {Trappola} from "../../Model/attivita/dettagli/Trappola";
import {Ditta} from "../../Model/metaschema/Ditta";

export class LeggiDitteTrappole{
    trappola: Trappola;
}

@Injectable({providedIn: 'root'})
export class DitteService{

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
    }
}
