import { Injectable } from "@angular/core";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { BehaviorSubject } from "rxjs";



@Injectable({
    providedIn: 'root'
})
export class SelezionaCentroService {
    public centroSelezionatoSubject = new BehaviorSubject<BaseCodeDescr>(new BaseCodeDescr(0));
}
