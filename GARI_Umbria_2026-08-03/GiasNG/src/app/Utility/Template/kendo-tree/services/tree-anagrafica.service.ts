import {Inject, Injectable, Optional} from '@angular/core';
import { Router } from '@angular/router';
import { TreeItem } from '@progress/kendo-angular-treeview';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import {BehaviorSubject, tap} from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { AnagraficaTree_PageSelector } from '../utility/utility';
import { TreeCfg, TreeNode, UpdateTree } from '../model';
import { TreeContainerService } from './tree-container.service';
import { TreeGridService } from 'app/anagrafica/services/TreeGrid.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { AnagraficaTreeFilters } from '../filters/anagrafica-tree-filters.component';
import { isNull } from 'lodash';
import {AjaxAgronicaService} from '../../../../Service/ajax-agronica.service';
import {MasterService} from '../../../../Service/master.service';
import {CoreWS_Generic} from '../../../../Model/CoreWS/CoreWS_Generic';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import {TreeFiltersService} from "../filters/anagrafica-tree-filters.service";

//const getNodesLink_Old = '/AgronicaControlli_2010/AlberoAnagrafica2017.aspx/GetNodesAlberoAnagrafeNG';
const getNodesLink = 'AgronicaControlli_2010/GetNodesAlberoAnagrafeNG';

@Injectable()
export class TreeService
    extends BehaviorSubject<TreeNode[]> {
    public filterTerm = '';
    highlightItems: BehaviorSubject<UpdateTree> = new BehaviorSubject(null);

    constructor(
        private anagrafica$: AnagraficaService,
        private router: Router,
        private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private drawer$: TreeContainerService,
        private grid$: TreeGridService,
        private agenda: ObjParametriAgendaService,
        private treeContainer: TreeContainerService,
        @Inject(LOADING_TOKEN) private loadingService: LoadingService,
        private masterService: MasterService
    ) {
        super(null);
    }

    public initUsingCurrentObjParametriAgenda() {
        const currentAgenda = this.agenda.getObjParamValue();
        this.agenda.changeObjParametriAgenda(currentAgenda);
    }

    public watchObjParametriAgenda(signal) {
        return this.agenda.currentObjParametriAgenda.pipe(takeUntil(signal));
    }

    /*public loadData_Old(filters: AnagraficaTreeFilters) {
        if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
            return;
        }

	    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.drawer$.drawerRef})

        const parametri: CoreWS_Generic<any> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: this.getParams(filters)
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any, any>(this.masterService.link_CoreWS + getNodesLink, parametri)
            .pipe(map((data) => {
                this.treeContainer.selectedImpresaChangedSoUpdateTree = false;
                this.loadingService.set_isLoading({isLoading: false, message: '', component: this.drawer$.drawerRef})
                const nodes: TreeNode[]= data.RispostaStringa;
                super.next(nodes);
            })).subscribe();
    }*/

    public loadData(filters: AnagraficaTreeFilters) {
        if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
            return;
        }

	    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.drawer$.drawerRef})

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(getNodesLink, this.getParams(filters))
            .pipe(map((data) => {
                this.treeContainer.selectedImpresaChangedSoUpdateTree = false;
                this.loadingService.set_isLoading({isLoading: false, message: '', component: this.drawer$.drawerRef})
                const nodes: TreeNode[]= data.RispostaStringa;
                super.next(nodes);
            })).subscribe();
    }

    private getParams(filters: AnagraficaTreeFilters) {
        const agenda = this.agenda.getObjParamValue();

        const cfg = TreeCfg;

        cfg.Piva = agenda.Piva;
        cfg.Sa_Cod = filters.centro.id;
        cfg.Flag_CatastoAziendale = filters.showCatasto;

        // cfg.DataInizio = this.setDataInizioBasedOnFilter();

        cfg.DataInizio = AGRODATAINIZIO.toISOString().slice(0, 10);
        cfg.DataFine =  AGRODATAFINE.toISOString().slice(0, 10);

        return {
            cfgSerialized: JSON.stringify(cfg),
            id: '0',
            PathRoot: ''
        };
    }

    setDataInizioBasedOnFilter(): string {
        let dataInizio: string = agroDataInizio();

        const filtrino = this.anagrafica$.filterData.value;
        if (filtrino.filter) {
            if (!isNull(filtrino.data)) {
                dataInizio = filtrino.data.toISOString().slice(0, 10);
            }
        }

        return dataInizio;
    }

    /**
   * Determines update behavior of the grid.
   * This could be using query parameters (if redirecting to a different page)
   * or using next on @selected subject if updating the same page.
   * @param e the clicked tree item.
   */
    public handleSelection(e: TreeItem) {
        const selector = new AnagraficaTree_PageSelector(this.router.url, e);
        selector.decideTarget();

        if(selector.decision?.samePage) {
            // La riga corrispondente dell'albero rimane selezionata.
            // Seleziona la riga corrispondente della griglia.
            this.grid$.gridSelection([selector.decision.gridItemId]);
            this.drawer$.expander.next(false);
        } else if(selector.decision?.target) {
            this.anagrafica$.navigateTo(selector.decision.target,
                {
                    treeIndex: e.index,
                    gridDataId: selector.decision.gridItemId
                });
        }
    }

    parseItemIndex(index: string) {
        const parts = index.split('_');
        const result = [];
        for(let i = 0; i < parts.length; ++i) {
            if(i === 0) {
                result.push(parts[i]);
            } else {
                result.push(result[i-1] + '_' + parts[i]);
            }
        }

        this.highlightItems.next({selected: [index], expanded: result});
    }

}

function agroDataInizio() {
    return AGRODATAINIZIO.toISOString().slice(0, 10);
}
