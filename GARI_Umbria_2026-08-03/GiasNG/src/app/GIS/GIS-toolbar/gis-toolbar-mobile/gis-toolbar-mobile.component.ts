import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, Optional, ViewChild } from '@angular/core';
import { GisToolbarService } from '../gis-toolbar.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { enum_OrigineChiamataLoadGeoJson } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
    standalone: false,
    selector: 'gis-toolbar-mobile',
    templateUrl: './gis-toolbar-mobile.component.html',
    styleUrls: ['./gis-toolbar-mobile.component.css']
})

export class GisToolbarMobileComponent implements AfterViewInit, OnDestroy {
    selectedPanel = 1;
    isToolbarOpen = false;

    displaySettingsDropdown = false;
    rightDropdown = '80px';

    private signal = new Subject<void>();

    constructor(
        public gisToolbarService: GisToolbarService,
        public googleMapGeoJsonService: GoogleMapGeoJsonService,
        private changeDetector: ChangeDetectorRef,
        private sharedDataService: SharedDataService
    ) {

        this.gisToolbarService.latValueSubject
            .pipe(takeUntil(this.signal)).subscribe(lat => {
            this.gisToolbarService.latValue = lat;
        });

        this.gisToolbarService.lngValueSubject
            .pipe(takeUntil(this.signal)).subscribe(lng => {
            this.gisToolbarService.lngValue = lng;
        });

    }

    ngAfterViewInit(): void {
        // Le azioni necessarie sono eseguite da ngAfterViewInit del gis-toolbar.component
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    public changeLatFocusHandler(focusin: boolean) {
        this.gisToolbarService.changeLatFocusHandlerService(focusin);
        this.changeDetector.detectChanges();
    }

    public changeLngFocusHandler(focusin: boolean) {
        this.gisToolbarService.changeLngFocusHandlerService(focusin);
        this.changeDetector.detectChanges();
    }

    public selectPanel (panel:number) {
        this.selectedPanel = panel;
    }

    public toggleToolbar() {
        this.isToolbarOpen = !this.isToolbarOpen;
    }

    public positionToggle() {
        this.gisToolbarService.strumentoGpsAttivo = !this.gisToolbarService.strumentoGpsAttivo;
        this.googleMapGeoJsonService.gestioneStrumentoGps(this.gisToolbarService.strumentoGpsAttivo);
    }

    public findAddress() {
        this.googleMapGeoJsonService.ricercaIndirizzo(this.gisToolbarService.searchAddressTxtBox);
    }

    public navigateTo() {
        this.googleMapGeoJsonService.apriGoogleMaps(this.gisToolbarService.searchAddressTxtBox);
    }

    centraSuAzienda(){
        this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CentraSuAzienda);
        this.googleMapGeoJsonService.loadGeoJsonForzato(true);
    }

}
