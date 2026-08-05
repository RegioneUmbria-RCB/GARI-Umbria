import { ElementRef, Inject, Injectable } from "@angular/core";
import { LoadingService, LOADING_TOKEN } from 'gias-ui-kit';
import { AnagrafeClient } from "app/Service/net-core6-api.service";
import { BehaviorSubject } from "rxjs";

@Injectable()
export class TreeAziendeService {
    
    public expander: BehaviorSubject<boolean | null> = new BehaviorSubject(false);
    public drawerRef: ElementRef;
    public allNodes: any[] = new Array();
    public filteredNodes;
    public filterTerm = '';

    pivaFilterSubject = new BehaviorSubject<Array<string>>(null);
    arrPivaFilter;

    constructor(
                private anagrafeService: AnagrafeClient,
                @Inject(LOADING_TOKEN) private loadingService: LoadingService) {
                    
        this.pivaFilterSubject.subscribe(data => {
            this.arrPivaFilter = data;
        });
    }

    //carico l'albero delle aziende in questa funzione, così da richiamare il loading
    getTreeAziende() {

        // se non ho ancora chimato l'endpoint
        if (this.allNodes.length == 0) {
            if (this.drawerRef)
                this.loadingService.set_isLoading({isLoading: true, message: '', component: this.drawerRef});

            this.anagrafeService.anagrafeGetAlberoGerarchiaImprese().subscribe((r) => {
                if (this.drawerRef)
                    this.loadingService.set_isLoading({isLoading: false, message: '', component: this.drawerRef});
                this.allNodes = JSON.parse(r.RispostaStringa).Items;
                this.filteredNodes = this.allNodes;
            });
        }
    }

    filterTreeDataByPivaArray(data: any[], filterValues: number[]) {

        if (filterValues.length == 0)
            return this.allNodes;

        return data
                .map(node => ({
                                ...node,
                                Items: node.Items ? this.filterTreeDataByPivaArray(node.Items, filterValues) : []
                }))
                .filter(node => filterValues.includes(node.Piva) || (node.Items && node.Items.length > 0));
    }

    filterTreeData(data: any[], filterString: string) {

        filterString = filterString.trim();

        if (!filterString)
            return this.allNodes;

        return data
                .map(node => ({
                                ...node,
                                Items: node.Items ? this.filterTreeData(node.Items, filterString) : []
                }))
                .filter(node => node.Text.toLowerCase().includes(filterString) || node.Piva.includes(filterString) || node.CUAA.includes(filterString) || (node.Items && node.Items.length > 0));
    }

}