import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { ModuliAttivi } from "app/Service/moduli-attivi.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { map, Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class ModuliAttiviGuard  {

    constructor(private moduliAttiviService: ModuliAttivi,
                private objParametriAgendaService: ObjParametriAgendaService) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        const agenda = this.objParametriAgendaService.getObjParamValue();
        return this.moduliAttiviService.caricaModuli(agenda.Piva).pipe(
            map((el) => {
                return true;
            })
        );
    }
}
