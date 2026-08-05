import {
    AfterViewInit,
    OnDestroy,
    OnInit,
    ViewChild,
    ViewEncapsulation,
    ElementRef,
    Component,
    ChangeDetectorRef,
    Input,
    Optional} from '@angular/core';

import { faSave, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { DrawerComponent } from '@progress/kendo-angular-layout';
import { Subject, Subscription } from 'rxjs';
import { AnagraficaTreeFilters } from '../filters/anagrafica-tree-filters.component';
import { GisTreeFilters } from '../filters/gis-tree-filters.component';
import { TreeService } from '../services/tree-anagrafica.service';
import { TreeGisService } from '../services/tree-gis.service';
import { TreeContainerService } from '../services/tree-container.service';
import { enum_TreeContext } from '../enum/tree-context';
import { consoleLogDebugParam } from 'app/Service/utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { SharedDataService } from 'app/GIS/services/shared-data.service';

@Component({
    standalone: false,
    selector: 'app-kendo-tree',
    templateUrl: './tree-container.component.html',
    styleUrls: ['./tree-container.component.scss'],
    encapsulation: ViewEncapsulation.None,
})
export class KendoTreeContainerComponent implements OnInit, OnDestroy, AfterViewInit {
    @Input() treeContainerContext: enum_TreeContext;
    @ViewChild('drawer') drawer: DrawerComponent;
    @ViewChild('drawer', {static: false, read: ElementRef}) drawerElRef: ElementRef;

    visualizzazioneTotale$ = this.sharedDataService.getVisualizzazioneTotale$();
    expanded = false;
    containerWidth = 440;
    resizeIcon: IconDefinition = faSave;

    signal$: Subject<void> = new Subject();
    private expanderSub: Subscription | undefined;

    constructor(
        public service: TreeContainerService,
        private treeService: TreeService,
        private sharedDataService: SharedDataService,
        @Optional() private treeGisService: TreeGisService,
        private detector: ChangeDetectorRef) {
        this.service.drawerRef = this.drawerElRef;
        this.service.selectedImpresaChangedSoUpdateTree = true;
    }

    ngAfterViewInit(): void {
        this.service.drawerRef = this.drawerElRef;
        this.expanderSub = this.service.expander.subscribe((state) => {
            this.drawer.toggle(state);
        });
    }

    ngOnInit() {
        this.service.treeContainerContext = this.treeContainerContext;
    }

    reReadTree(event: AnagraficaTreeFilters) {
        this.treeService.loadData(event);
    }

    reReadTreeGis(event: GisTreeFilters) {
        consoleLogDebugParam(
            enum_logDebugArea.App,
            enum_logDebugTipo.CambioAzienda,
            this.constructor.name,
            'reReadTreeGis',
            event
        );
        this.treeGisService.loadData(event);
        this.treeGisService.ricaricaFeatureRiletturaAlbero();
    }

    ngOnDestroy() {
        this.signal$.next();
        this.signal$.complete();
        this.service.expander.next(false);
        this.expanderSub?.unsubscribe();
    }

    updateContainerWidth(newWidth: number) {
        this.containerWidth = newWidth;
        this.detector.detectChanges();
    }

}
