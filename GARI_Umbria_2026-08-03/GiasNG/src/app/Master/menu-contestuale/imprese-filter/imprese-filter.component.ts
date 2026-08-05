import { AfterViewInit, Component, OnDestroy, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { AutoCompleteComponent } from "@progress/kendo-angular-dropdowns";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { filter, map, Subject, switchMap, takeUntil, tap } from "rxjs";
import { ImpreseFilterService } from "./imprese-filter.service";


@Component({
    standalone: false,
    selector: 'gias-imprese-filter',
    templateUrl: './imprese-filter.component.html',
    styleUrls: ['./imprese-filter.component.scss']
})
export class ImpreseFilterComponent implements AfterViewInit, OnDestroy {
    @ViewChild("autocompleteImpreseFilter", { static: false }) autocompleteImpreseFilter: AutoCompleteComponent;
    //@Output() valueChange = new EventEmitter<Impresa>();
    public signal$: Subject<void> = new Subject();

    public data: Array<{text: string, value: string}> = []

    constructor(private impreseFilterService: ImpreseFilterService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private router: Router) {
    }


    ngAfterViewInit(): void {
        this.autocompleteImpreseFilter.filterChange
            .asObservable()
            .pipe(
                takeUntil(this.signal$),
                filter((val) => val.length >= 3),
                tap(() => (this.autocompleteImpreseFilter.loading = true)),
                switchMap((searchTerm) => {
                    return this.impreseFilterService.filtraImprese(searchTerm);
                }),
                map((vals) => {
                    return vals.map((el) => { return { text: el.partitaIva + ' - ' + el.ragioneSociale + ' (' + el.CUAA + ')' , value: el.partitaIva } })
                })
        )
            .subscribe((data) => {
                this.data = data;
                this.autocompleteImpreseFilter.loading = false;
            });



        this.autocompleteImpreseFilter.valueChange.pipe(
            takeUntil(this.signal$)
        ).subscribe()

    }

    valueChange(event: string) {
        const arrayData = event.split('-')
        const pivaSelezionata = arrayData[0].trim();
        if (pivaSelezionata != '') {
            const ragioneSociale = arrayData[1]?.split('(')[0]?.trim();
            if (ragioneSociale) {
                let obj = this.objParametriAgendaService.getObjParamValue();
                this.objParametriAgendaService.resettaObjAgenda(obj);
                obj.Piva = pivaSelezionata;
                obj.RagSoc = ragioneSociale;
                this.objParametriAgendaService.changeObjParametriAgenda(obj);
                let currentUrl = this.router.url;
                this.router.routeReuseStrategy.shouldReuseRoute = () => false;
                this.router.onSameUrlNavigation = 'reload';
                this.router.navigate([currentUrl]);
                this.autocompleteImpreseFilter.value = '';
            }
        }
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

}
