import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

import { BehaviorSubject, map, Subject, take } from 'rxjs';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeContainerService } from '../services/tree-container.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';


//const selezionaCentroDropdownLink_Old = '/AgronicaControlli_2010/AlberoAnagrafica2017.aspx/CaricaAlberoCentriDropdown';
const selezionaCentroDropdownLink = 'AgronicaControlli_2010/CaricaAlberoCentriDropdownNG';
const CATASTO_COOKIE_NAME = 'AlberoAnagraficaCatastoCookie';


@Injectable({providedIn: 'root'})
export class TreeFiltersService extends BehaviorSubject<DropdownListItem[]> {


    constructor(private cookies: CookieService,
                private treeContainer: TreeContainerService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private objParametriAgendaService: ObjParametriAgendaService) {
        super(null);
    }

    /*caricaInteroAlberoConFiltri_Old() {
        if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
            super.next(this.value);
            return;
        }

        const callback = (...httpArgs: HttpArgs) => {
            let agenda = httpArgs[0];
            let master = httpArgs[1];
            let params = agenda.getObjParamValue();
            return {
              piva: params.Piva,
              objP_server: master.ObjParametri_Server,
              objP_utenti: master.ObjParametri_Utenti
            };
        };

        this.http.post2(selezionaCentroDropdownLink, callback.bind(this), false).pipe(take(1), map((dati: any[]) => {
            let items = dati.map(s => new DropdownListItem(s.sa_cod, s.sa_nome));
            return items;
        })).subscribe(s => super.next(s));
    }*/

    caricaInteroAlberoConFiltri() {
        if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
            super.next(this.value);
            return;
        }

        this.ajaxAgronicaAPIService.ajaxAPIPost<string, any>(selezionaCentroDropdownLink, this.objParametriAgendaService.getObjParamValue().Piva, false).pipe(take(1), map(dati => {
            let items = dati.RispostaStringa.map(s => new DropdownListItem(s.sa_cod, s.sa_nome));
            return items;
        })).subscribe(s => super.next(s));
    }

    getCatastoCookie(): boolean {
        let cookie = this.cookies.get(CATASTO_COOKIE_NAME);
        return (/true/i).test(cookie);
    }

    setCatastoCookie(value: boolean) {
        this.cookies.set(
            CATASTO_COOKIE_NAME, JSON.stringify(value), { path: '/' }
        );
    }

}
