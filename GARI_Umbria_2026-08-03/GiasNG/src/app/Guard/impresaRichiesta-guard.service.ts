import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';

const NESSUNA_IMPRESA_SELEZIONATA = "";

@Injectable({ providedIn: 'root' })
export class ImpresaRichiestaGuard  {

    constructor(
        private gestioneRichiesteService: GestioneRichiesteService,
        private agenda: ObjParametriAgendaService,
        private masterService: MasterService) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        let objAgenda = this.agenda.getObjParamValue();
        let pivaCorrente = objAgenda.Piva;
        let guardData = route.data.impresaRichiestaGuardData;

        if(guardData.sitoRedirect == null || guardData.paginaRedirect == null)
            throw Error("Missing guard data. Parametri sitoRedirect and paginaRedirect are required.");

        let paginaRedirect = guardData.paginaRedirect;
        if(guardData.paginaRedirect == enum_PagineGiasNG.Pagina_Corrente)
            paginaRedirect = this.masterService.getCurrentPageAsValue();

        if(pivaCorrente == NESSUNA_IMPRESA_SELEZIONATA)
            this.gestioneRichiesteService.goToFiltrino(guardData.sitoRedirect, paginaRedirect).then((link) => {
                window.location.href = link;
                return false;
            });
        else
            return true;
    }
}
