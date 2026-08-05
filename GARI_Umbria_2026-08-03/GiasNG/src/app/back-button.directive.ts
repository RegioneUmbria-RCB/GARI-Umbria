import { Directive, HostListener } from '@angular/core';
import { NavigationService } from './Service/navigation.service';
import { Location } from '@angular/common';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { GestioneRichiesteService } from './Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from './Model/siti.enum';
import { MasterService } from './Service/master.service';
import { ObjParametriAgendaService } from "./Service/obj-parametri-agenda.service";
import { ExternalNavigationService } from './Service/external-navigation.service';
import { filter, map, pairwise, startWith } from 'rxjs';
import { GiasMessageService } from './Service/gias-message.service';

@Directive({ standalone: false,
    selector: '[backButton]'
})
export class BackButtonDirective {
    isBack = false;

    @HostListener('window:popstate', ['$event'])
    onPopState(event) {
        if (window.location.pathname == '/GestioneRichieste') {
            //this.masterService.set_isLoading({ isLoading: true });
        }
    }
    constructor(
        private location: Location,
        private giasMessageService: GiasMessageService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private router: Router,
        private objParametriAgendaService: ObjParametriAgendaService,
        private externalNavigationService: ExternalNavigationService
    ) {
        this.router.events.subscribe((event: NavigationStart) => {
            if (event.url?.includes('GestioneRichieste') && this.isBack == true) {
                this.gestioneRichiesteService.isBack = true;
                this.isBack = false;
                //this.location.back();
                //PER ORA TORNO AL MENU PRINCIPALE
                //this.masterService.set_isLoading({ isLoading: true });
                this.gestionePulsanteIndietro_verso_altroSito();
            }
            this.isBack = false;
        });
    }

    @HostListener('click')
    onClick(): void {
        if (this.externalNavigationService.isBackInvalid) {
            this.giasMessageService.errorMessage('NonPuoiUtilizzareQuestaFunzionalitaInQuestoPuntoUsaIlMenuLateralePerLaNavigazione', false, true);
            return;
        }

        this.isBack = true;
        this.externalNavigationService.goBack();
        this.location.back();
    }

    private gestionePulsanteIndietro_verso_altroSito() {

        let sito = Enum_SiteRedirector.Sito_AgronicaAgenda_2010;

        let pagina = enum_PagineAgenda_2010.Menu;

        if (this.objParametriAgendaService.getObjParamValue().Sito_Provenienza && this.objParametriAgendaService.getObjParamValue().Sito_Provenienza > 0 && this.objParametriAgendaService.getObjParamValue().Sito_Provenienza !== Enum_SiteRedirector.GiasNG) {
            sito = this.objParametriAgendaService.getObjParamValue().Sito_Provenienza;
        }

        if (this.objParametriAgendaService.getObjParamValue().Pagina_Provenienza_AltroSito && this.objParametriAgendaService.getObjParamValue().Pagina_Provenienza_AltroSito > 0) {
            pagina = this.objParametriAgendaService.getObjParamValue().Pagina_Provenienza_AltroSito;
        }

        this.gestioneRichiesteService.gestionePassaggioAltroSito(sito, pagina).then((resp) => {
            window.location.href = resp;
        });
    }

}

