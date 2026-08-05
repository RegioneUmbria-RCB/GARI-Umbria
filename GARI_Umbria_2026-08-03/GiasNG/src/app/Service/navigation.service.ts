import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { GestioneRichiesteService } from './gestione-richieste.service';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { ConfigurazioneSitiService } from './configurazione-siti.service';
import { AjaxAgronicaAPIService } from './ajax-agronica.api.service';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';

@Injectable({
    providedIn: 'root'
})
export class NavigationService {
    private history: string[] = [];
    private isBack = false;

    constructor(private aRoute: ActivatedRoute,
        private router: Router,
        private location: Location,
        private gestioneRichiesteService: GestioneRichiesteService,
        private configurazioneSiti: ConfigurazioneSitiService,
        private apiService: AjaxAgronicaAPIService) {
        this.router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                if (!this.isBack) {
                    this.history.push(event.urlAfterRedirects);
                    this.isBack = false;
                }
            }
        });
    }

    back() {
        this.history.pop();
        if (this.history.length > 0) {
            this.isBack = true;
            const url = this.history[this.history.length - 1];
            if (!url.includes('GestioneRichieste')) {
                this.router.navigateByUrl(url);
            }
        }
        this.home();
    }

    home(): void {
        this.apiService.ajaxAPIGet("Menu/VersioneHeader", {}).subscribe((resp) => {
            //console.log(resp);
            if (resp.RispostaStringa == 2022) {
                this.gestioneRichiesteService.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Dashboard).then((resp: string) => {
                    this.router.navigate([resp]);
                })
            } else {
                this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, 1).then((resp: string) => {
                    if (resp !== '') {
                        window.location.href = resp;
                    }
                });
            }
        })
        //this.configurazioneSiti.currentConfigurazione_Siti.subscribe((val) => console.log(val));

    }

    public getPreviousUrl(routeArray): string {
        let prevRoute = '';
        for (let i = 0; i < routeArray.length - 1; i++) {
            if (routeArray[i].url._value[0].length > 0) {
                prevRoute += routeArray[i].url._value[0].path + '/';
            }
        }
        return prevRoute.slice(0, -1);
    }

}
