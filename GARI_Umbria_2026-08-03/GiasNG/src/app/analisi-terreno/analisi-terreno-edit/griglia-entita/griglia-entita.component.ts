import { Component, Inject, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { GiasKendoGridComponent, GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridEntitaHttpService } from "./service/grid-griglia-entita.service";
import { generateGridProviders } from 'gias-kendo-grid';
import { SelectionEvent } from "@progress/kendo-angular-grid";
import { AnalisiTerrenoFormService } from "app/analisi-terreno/griglia-analisi-terreno/service/analisi-terreno-form.service";
import { GISGeometrySelectionService } from "app/GIS/services/gis-geometry-selection.service";
import { enum_Entita_Analisi } from "app/Model/TipiEnumerativi";
import { Subject, takeUntil, take } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-griglia-entita',
    templateUrl: './griglia-entita.component.html',
    styleUrls: ['./griglia-entita.component.scss'],
    providers: [...generateGridProviders(GridEntitaHttpService, GrigliaEntitaComponent)],
    encapsulation: ViewEncapsulation.None
})
export class GrigliaEntitaComponent implements OnInit, OnDestroy {

    @ViewChild('GridEntita') GridEntitaElRef: GiasKendoGridComponent;
    @Input() entitaGrid: enum_Entita_Analisi;

    signal: Subject<void> = new Subject();
    private rowsLoaded$ = new Subject<boolean>();

    constructor(
        @Inject(GRID_HTTP_TOKEN) private gridEntitaHttpService: GridEntitaHttpService,
        private gisGeometrySelectionService: GISGeometrySelectionService,
        private analisiFormService: AnalisiTerrenoFormService
    ) { }

    ngOnInit(): void {
        this.gridEntitaHttpService.entitaCoinvolta.next(this.entitaGrid);

        this.gisGeometrySelectionService.geometrySelectedFromMap$
            .pipe(takeUntil(this.signal))
            .subscribe(keys => this.handleSelectedGeometries(keys));

        this.rowsLoaded$
            .asObservable()
            .pipe(take(1))
            .subscribe(() => this.handleSelectedGeometries(this.gisGeometrySelectionService.getCurrentSelection()));

        this.analisiFormService.formAnalisiTerreno.get('validita').valueChanges.pipe(takeUntil(this.signal)).subscribe(values => {
            this.gridEntitaHttpService.handlerChangeDates(values, this.entitaGrid);
        });
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    onSelected(event: SelectionEvent): void {
        const entitaCoinvolte = this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value;
        if (entitaCoinvolte.length == 0) {
            return;
        }

        const entita = entitaCoinvolte[entitaCoinvolte.length - 1];
        const selected = event.selectedRows.map((item) => this.getKeyFromSelectedItem(entita, item.dataItem));
        this.gisGeometrySelectionService.addFromOutside(...selected as string[]);

        const deselected = event.deselectedRows.map((item) => this.getKeyFromSelectedItem(entita, item.dataItem));
        this.gisGeometrySelectionService.removeFromOutside(...deselected as string[]);
    }

    rowsLoaded() {
        this.rowsLoaded$.next(true);
    }

    private handleSelectedGeometries(keys: [string, boolean][]): void {
        if (this.GridEntitaElRef.rows == null) {
            return;
        }

        const entitaCoinvolte = this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value;
        if (entitaCoinvolte.length == 0) {
            return;
        }

        const entita = entitaCoinvolte[entitaCoinvolte.length - 1];

        // if service is not initialized, get present selection from grid
        if (!this.gisGeometrySelectionService.isInitialized()) {
            this.initGeometrySelectionService();
            return;
        }

        for (const row of this.GridEntitaElRef.rows) {
            const rowKey = this.getKeyFromSelectedItem(entita, row);
            const key = keys.find((item) => item[0].includes(rowKey));
            if (key != null) {
                const isSelected = key[1];
                row['Selected'] = isSelected;
                const index = this.analisiFormService.arrayEntita.findIndex(item => item.chiave == row['chiave']);
                if (isSelected && index == -1) {
                    this.analisiFormService.arrayEntita.push(row);
                } else if (!isSelected && index != -1) {
                    this.analisiFormService.arrayEntita.splice(index, 1);
                }
            }
        }

        GridEntitaHttpService.applySort(this.GridEntitaElRef.rows);
        this.GridEntitaElRef.refresh();
    }

    private initGeometrySelectionService() {
        const entitaCoinvolte = this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value;
        if (entitaCoinvolte.length == 0) {
            return;
        }

        const entita = entitaCoinvolte[entitaCoinvolte.length - 1];
        const preSelected = this.GridEntitaElRef.rows
            .filter(row => row['Selected'] === true)
            .map(row => this.getKeyFromSelectedItem(entita, row));

        if (preSelected.length > 0) {
            this.gisGeometrySelectionService.addFromOutside(...preSelected as string[]);
        }
    }

    private getKeyFromSelectedItem(entita: enum_Entita_Analisi, item: any): string {
        switch (entita) {
            case enum_Entita_Analisi.Centro:
                return `§${item.Piva}§${item.sa_cod}`;
            case enum_Entita_Analisi.Campo:
                return `§${item.PIVA}§${item.sa_cod}§${item.Campo_Cod}`
            case enum_Entita_Analisi.Appezzamento:
                return `§${item.Piva}§${item.sa_cod}§${item.Campo_Cod}§${item.Appezza}`
            case enum_Entita_Analisi.Impianto:
                return `§${item.PIVA}§${item.SA_COD}§${item.Campo_Cod}§${item.APPEZZA}§${item.id_Reg}`
            case enum_Entita_Analisi.Particella:
                return `§${item.PIVA}§${item.SA_COD}§${item.PROV}§${item.COM}§${item.SEZIONE}§${item.FOGLIO}§${item.NUMERO}§${item.SUBALTERNO}`
            case enum_Entita_Analisi.Fabbricato:
            case enum_Entita_Analisi.EntitaGrafica:
            case enum_Entita_Analisi.NonDefinito:
            case enum_Entita_Analisi.Impresa:
            default:
                return '';
        }
    }
}
