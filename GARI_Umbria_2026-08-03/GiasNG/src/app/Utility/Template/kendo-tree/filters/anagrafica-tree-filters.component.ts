import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { map, Observable, skip, Subject, takeUntil } from 'rxjs';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeService } from '../services/tree-anagrafica.service';
import { TreeContainerService } from '../services/tree-container.service';
import { TreeFiltersService } from './anagrafica-tree-filters.service';
import { faLeaf} from '@fortawesome/free-solid-svg-icons';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';

const DEFAULT_ITEM_POSITION = 0;
export type AnagraficaTreeFilters = {
    showCatasto: boolean,
    centro: DropdownListItem
};

@Component({
    standalone: false,
    selector: 'tree-filters',
    templateUrl: './anagrafica-tree-filters.component.html',
    styleUrls: ['./anagrafica-tree-filters.component.scss'],
    providers: []
})
export class AnagraficaTreeFiltersComponent implements OnInit, OnDestroy {
    @Output() filtersChanged: EventEmitter<AnagraficaTreeFilters> = new EventEmitter();

    centriAziendaliDDL: Observable<DropdownListItem[]>;
    signal: Subject<void> = new Subject();
    currentCentro: DropdownListItem;
    showCatasto: boolean;
    faReload = faLeaf;
    // treeContainerContext: enum_TreeContext;
    private lastFilters = {};
    constructor(
        private manager: TreeFiltersService,
        public treeService: TreeService,
        public anagrafica: AnagraficaService,
        public treeContainer: TreeContainerService) {
        this.centriAziendaliDDL = manager.pipe(map((items) => {
            return items;
        }));

        manager.pipe(takeUntil(this.signal), skip(1)).subscribe(items => {
            this.currentCentro = items[DEFAULT_ITEM_POSITION];
            this.showCatasto = manager.getCatastoCookie();
            const filters = {
                showCatasto: this.showCatasto,
                centro: this.currentCentro
            };
            if (JSON.stringify(filters) != JSON.stringify(this.lastFilters)){
                this.lastFilters = filters;
                this.filtersChanged.emit(filters);
            }
        })

        this.treeService.watchObjParametriAgenda(this.signal).pipe(skip(1)).subscribe(s => {
            this.manager.caricaInteroAlberoConFiltri();
        });

        this.anagrafica.filterData.pipe(skip(1), takeUntil(this.signal)).subscribe(s => {
            this.ricaricaAlbero();
        })

        manager.caricaInteroAlberoConFiltri();
    }

    ngOnInit(): void {
        // this.treeContainerContext = this.treeContainer.treeContainerContext;
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    ricaricaAlbero() {
        this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
        this.manager.caricaInteroAlberoConFiltri();
    }

    onFiltersChanged() {
        const filters = {
            showCatasto: this.showCatasto,
            centro: this.currentCentro
        };
        if (JSON.stringify(filters) != JSON.stringify(this.lastFilters)){
            this.lastFilters = filters;
            this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
            this.manager.setCatastoCookie(filters.showCatasto);
            this.filtersChanged.emit(filters);
        }
    }

}
