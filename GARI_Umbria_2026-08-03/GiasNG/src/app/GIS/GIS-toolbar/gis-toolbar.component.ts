import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Optional, ViewChild, OnDestroy, Input } from '@angular/core';
import { GisToolbarService } from './gis-toolbar.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { PositionService } from '../services/position.service';
import { SharedDataService } from '../services/shared-data.service';
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { Subject, takeUntil } from 'rxjs';
import { enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { consoleLogDebugParam } from 'app/Service/utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { GoogleMapService } from '../google-map/google-map.service';
import { DropDownButtonComponent } from '@progress/kendo-angular-buttons';

@Component({
  standalone: false,
  selector: 'gis-toolbar',
  templateUrl: './gis-toolbar.component.html',
  styleUrls: ['./gis-toolbar.component.css']
})
export class GisToolbarComponent implements AfterViewInit, OnDestroy {
  @Input() forRilievi: boolean | undefined = false;
  @Input() isAnalisiTerreno: boolean | undefined = false;
  @Input() analisiTerrenoEdit?: boolean;

  @ViewChild('dropdownButton', { static: false }) dropdownButton: ElementRef;
  @ViewChild('btnCentraSuAzienda', { static: false }) btnCentraSuAzienda: ElementRef;
  @ViewChild('goToPosition') rilieviLastBtn: ElementRef;
  @ViewChild('toolsDropdown') toolsDropdown: DropDownButtonComponent;

  lastButtonToolbar: ElementRef = null;

  displaySettingsDropdown = false;
  rightDropdown = '80px';

  signal: Subject<void> = new Subject();

  pivaPrecedente = null;
  addingMarkers = false;
  isMarkerPlacer$ = this.gisToolbarService.isMarkerPlacerActive$;

  constructor(
    public gisToolbarService: GisToolbarService,
    public googleMapGeoJsonService: GoogleMapGeoJsonService,
    private positionService: PositionService,
    private changeDetector: ChangeDetectorRef,
    private sharedDataService: SharedDataService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private googleMapService: GoogleMapService,
    @Optional() private treeGisService: TreeGisService
  ) {
    // console.log('[gis-toolbar.component] constructor');

    this.gisToolbarService.latValueSubject
      .pipe(takeUntil(this.signal)).subscribe(lat => {
      this.gisToolbarService.latValue = lat;
    });

    this.gisToolbarService.lngValueSubject
      .pipe(takeUntil(this.signal)).subscribe(lng => {
      this.gisToolbarService.lngValue = lng;
    });

    this.sharedDataService.getCfgAlberoGisUtente$()
      .pipe(takeUntil(this.signal)).subscribe(cfg => {
      let cfgAlbero = cfg[0].CfgAlbero;
      let ricarica = cfg[1];
      if (cfgAlbero !== undefined && ricarica) {
        this.seRicaricaAlberoForzato();
      }
    });

    this.sharedDataService.getRicaricaAlbero$()
      .pipe(
        takeUntil(this.signal)
      ).subscribe( flagRicarica => {
      if (flagRicarica === true) {
        this.seRicaricaAlberoForzato();
      }
    });

    this.objParametriAgendaService.currentObjParametriAgenda
      .pipe(takeUntil(this.signal))
      .subscribe(parametriAgenda => {
        if (this.pivaPrecedente && this.pivaPrecedente !== parametriAgenda.Piva) {
          //================================================================================
          // CAMBIO AZIENDA
          //================================================================================
          consoleLogDebugParam(
            enum_logDebugArea.App,
            enum_logDebugTipo.CambioAzienda,
            this.constructor.name,
            'currentObjParametriAgenda',
            parametriAgenda.Piva
          );

          this.resetCentroAziendaleAlbero();

          //--------------------------------------------------------------------------------
          // Ricarico Albero
          //--------------------------------------------------------------------------------
          this.ricaricaAlberoCambioAzienda();

          //--------------------------------------------------------------------------------
          // Ricarico Feature
          // 17/06/2025 Mattia: Ricarico Filtro Temporale Avanzato prima di ricaricare le feature
          //--------------------------------------------------------------------------------
          //this.ricaricaFeatureCambioAzienda();
          this.ricaricaFiltroTemporaleAvanzatoCambioAzienda()
          .then(() => {this.ricaricaFeatureCambioAzienda()});

        }
        this.pivaPrecedente = parametriAgenda.Piva;
      });

    this.sharedDataService.ricaricaFeatureSource.subscribe( flagRicarica => {
      if (flagRicarica === true) {
        this.ricaricaFeatureSharedDataService();
      }
    });

  }

  openTools(): void {
    this.gisToolbarService.setCurrentToolsDropdownInstance(this.toolsDropdown);
  }

  private get rightPosition(): number {
    this.getLastButtonToolbar();
    const padding = window.getComputedStyle(this.lastButtonToolbar.nativeElement, null).paddingRight;
    const rightPositionComputed =
      parseInt(padding, 10) +
      window.innerWidth -
      (this.lastButtonToolbar.nativeElement.getBoundingClientRect().x +
        this.lastButtonToolbar.nativeElement.getBoundingClientRect().width + 20);
    return rightPositionComputed;
  }

  public get topPosition(): number {
    this.getLastButtonToolbar();
    const topPositionComputed =
      this.lastButtonToolbar.nativeElement.getBoundingClientRect().y +
      this.lastButtonToolbar.nativeElement.getBoundingClientRect().height + 25;
    return topPositionComputed;
  }

  private getLastButtonToolbar() {
    if (this.gisToolbarService.permessoSetupVisualizzazione) {
      this.lastButtonToolbar = this.dropdownButton;
    } else {
      this.lastButtonToolbar = this.btnCentraSuAzienda;
    }
    if (this.forRilievi || this.isAnalisiTerreno) {
      this.lastButtonToolbar = this.rilieviLastBtn;
    }
  }

  ngAfterViewInit(): void {
    this.positionService.createPlaceAutocomplete(<HTMLInputElement>document.getElementById('searchAddressTxtBox'));
    if(window.innerWidth > 991){
      requestAnimationFrame(() => {
        if (window.innerWidth > 991 && !this.gisToolbarService.windowsPositionApplied) {
          this.gisToolbarService.layerBtnToggle(this.rightPosition);
          this.gisToolbarService.drawBtnToggle(this.rightPosition);
          this.gisToolbarService.windowsPositionApplied = true;
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  isCustom(): boolean {
    return this.isAnalisiTerreno || this.forRilievi;
  }

  public toggleAddMarkersModality(): void {
     this.gisToolbarService.toggleMarkerPlacer()
  }

  public toggleSettingsDropdown() {
    this.rightDropdown = this.rightPosition + 'px';
    this.displaySettingsDropdown = !this.displaySettingsDropdown;
  }

  private configurazioneBtnClick() {
    this.displaySettingsDropdown = !this.displaySettingsDropdown;
    this.gisToolbarService.configurazioneBtnClick();
  }

  public changeLatFocusHandler(focusin: boolean) {
    this.gisToolbarService.changeLatFocusHandlerService(focusin);
    this.changeDetector.detectChanges();
    //focusin ? this.showLatMaskTyped = true : this.showLatMaskTyped = false;
  }

  public changeLngFocusHandler(focusin: boolean) {
    this.gisToolbarService.changeLngFocusHandlerService(focusin);
    this.changeDetector.detectChanges();
    //focusin ? this.showLngMaskTyped = true : this.showLngMaskTyped = false;
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

  //--------------------------------------------------------------------------------
  // Ricarica Feature e Albero
  //--------------------------------------------------------------------------------
  // Sono gestiti tramite observable SOLO su gis-toolbar.component e non anche
  // gis-toolbar-mobile.component per evitare di fare 2 volte le stesse cose,
  // visto che entrambi i componenti sono attivi.
  //--------------------------------------------------------------------------------

  ricaricaAlberoCambioAzienda() {
    // A livello di albero il cambio azienda viene intercettato nel gis-tree-filters.component.ts;
    // vedi riga sorgente: this.manager.caricaInteroAlberoConFiltri().
    // L'istruzione a seguire fa in modo che la lettura avvenga effettivamente.
    this.setSelectedImpresaChangedSoUpdateTree();
  }

  ricaricaFeatureCambioAzienda(){
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CambioAzienda);
    this.googleMapGeoJsonService.loadGeoJsonBase(true);
  }

  ricaricaFiltroTemporaleAvanzatoCambioAzienda(){
    return this.googleMapGeoJsonService.loadFiltroTemporale();
  }

  ricaricaFeatureSharedDataService(){
    // Se serve verificare la provenienza, utilizzare la seguente funzione:
    // this.sharedDataService.getOrigineChiamataLoadGeoJson();
    this.googleMapGeoJsonService.loadGeoJsonBase(true);
  }

  centraSuAzienda(){
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CentraSuAzienda);
    this.googleMapGeoJsonService.loadGeoJsonForzato(true);
  }

  seRicaricaAlberoForzato() {
    if (this.treeGisService) {
      this.treeGisService.loadDataInternalFilters(true);
    }
  }

  resetCentroAziendaleAlbero() {
    let cfg = this.sharedDataService.getCfgAlberoGisUtente()[0];
    if (cfg.CfgAlbero !== undefined) {
      cfg.CfgAlbero.Sa_Cod = '0';
    }
  }

  setSelectedImpresaChangedSoUpdateTree() {
    if (this.treeGisService) {
      this.treeGisService.setSelectedImpresaChangedSoUpdateTree();
    }
  }

}
