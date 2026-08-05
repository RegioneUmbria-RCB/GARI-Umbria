import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BehaviorSubject, map, Subject, take } from 'rxjs';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeContainerService } from '../services/tree-container.service';
import {GisTreeFilters} from './gis-tree-filters.component';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';

// const selezionaCentroDropdownLink = '/AgronicaControlli_2010/AlberoAnagrafica2017.aspx/CaricaAlberoCentriDropdown';
const selezionaCentroDropdownLink = 'AgronicaControlli_2010/CaricaAlberoCentriDropdownNG';

@Injectable({providedIn: 'root'})
export class TreeGisFiltersService extends BehaviorSubject<DropdownListItem[]> {

    public centroSelezionato: BehaviorSubject<GisTreeFilters> = new BehaviorSubject<GisTreeFilters>(undefined);

    private _apriFiltroImpiantiBtnClick$ = new Subject<void>();

    constructor(private treeContainer: TreeContainerService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
        super(null);
    }

    public get apriFiltroImpiantiBtnClick$(): Observable<void> {
        return this._apriFiltroImpiantiBtnClick$.asObservable();
    }

    public apriFiltroImpiantiBtnClick() {
        this._apriFiltroImpiantiBtnClick$.next();
    }

    caricaInteroAlberoConFiltri() {
        if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
            super.next(this.value);
            return;
        }

        // const callback = (...httpArgs: HttpArgs) => {
        //     let agenda = httpArgs[0];
        //     let master = httpArgs[1];
        //     let params = agenda.getObjParamValue();
        //     return {
        //       piva: params.Piva,
        //       objP_server: master.ObjParametri_Server,
        //       objP_utenti: master.ObjParametri_Utenti
        //     };
        // };

        // this.http.post2(selezionaCentroDropdownLink, callback.bind(this), false).pipe(take(1), map((dati: any[]) => {
        //     let items = dati.map(s => new DropdownListItem(s.sa_cod, s.sa_nome));
        //     return items;
        // })).subscribe(s => super.next(s));

        this.ajaxAgronicaAPIService.ajaxAPIPost<string, any>(selezionaCentroDropdownLink, this.objParametriAgendaService.getObjParamValue().Piva, false).pipe(take(1), map(dati => {
            let items = dati.RispostaStringa.map(s => new DropdownListItem(s.sa_cod, s.sa_nome));
            return items;
        })).subscribe(s => super.next(s));

    }

}
