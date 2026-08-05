import { Component, OnDestroy, OnInit } from "@angular/core";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { WidgetStatisticheClient } from "app/Service/net-core6-api.service";
import { GestioneMultiAziendaService } from "app/widget-config/gestione-multiazienda.service";
import { combineLatest, Subject, takeUntil } from "rxjs";


@Component({
    standalone: false,
    selector: 'app-paese-multiazienda-widget',
    templateUrl: './paese-multiazienda-widget.component.html',
    styleUrls: ['./paese-multiazienda-widget.component.scss']
})
export class PaeseMultiAziendaComponent implements OnInit, OnDestroy {

    constructor(
        private widgetsClient: WidgetStatisticheClient,
        private funzioniComuni: FunzioniComuniService,
        private gestioneMultiAziendaService: GestioneMultiAziendaService
    ) { }

    loading = false;
    public areaList = [];
    public yearList = [];
    selectedCountry;
    selectedYear;

    private signal$ = new Subject<void>();

    ngOnInit(): void {

        //caricati nel widgetsComponent
        this.areaList = this.gestioneMultiAziendaService.countriesList;
        this.yearList = this.gestioneMultiAziendaService.yearsList;

        this.gestioneMultiAziendaService.nazioneMultiAzienda$
            .pipe(takeUntil(this.signal$))
            .subscribe(nazione => {
                this.selectedCountry = nazione;
            });

        this.gestioneMultiAziendaService.annoMultiAzienda$
            .pipe(takeUntil(this.signal$))
            .subscribe(anno => {
                this.selectedYear = anno;
            });
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    onDDLValueChangeCountry(value: any) {
        this.gestioneMultiAziendaService.setNazioneMultiAzienda(value);
    }

    onDDLValueChangeYear(value: any) {
        this.gestioneMultiAziendaService.setAnnoMultiAzienda(value);
    }

}
