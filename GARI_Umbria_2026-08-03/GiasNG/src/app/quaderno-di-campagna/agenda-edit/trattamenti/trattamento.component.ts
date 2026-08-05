import {AfterViewChecked, AfterViewInit, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {PanelBarExpandEvent, SelectEvent} from '@progress/kendo-angular-layout';
import {GISModality} from 'app/GIS/GIS-enum/GIS-feature';
import {KendoWindowsService, WindowArgs, WindowTypes} from 'app/Service';
import {Subscription} from 'rxjs';
import {QdCService} from '../service/qdc.service';
import {TranslocoService} from '@jsverse/transloco';
import {GiasPanelBar} from 'gias-ui-kit';
import {Lavorazione} from '../../../Model/attivita/Lavorazione';
import {enum_LAVCOD} from '../../../Model/TipiEnumerativi';
import {GisToolbarService} from '../../../GIS/GIS-toolbar/gis-toolbar.service';
import {GISComponent} from '../../../GIS/GIS.component';
import { Operazione } from 'app/Service/api.service';
import { FormControl } from '@angular/forms';
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';

@Component({
    standalone: false,
    selector: 'app-trattamento',
    templateUrl: './trattamento.component.html',
    styleUrls: ['./trattamento.component.scss']
})

export class TrattamentoComponent implements OnInit, OnDestroy, AfterViewInit, AfterViewChecked {

    GISModality = GISModality;
    showToolbar = false;
    isTabLoaded: boolean[] = [false];
    isSpecieSelected = false;
    tabSelected = 0;
    showRilievoPiogge: boolean = false;

    private valueChangesSub: Subscription = new Subscription();
    private operationChangeSub: Subscription = new Subscription();
    private GISToolbarService: GisToolbarService;

    private latChangesSub: Subscription = new Subscription();
    private lngChangesSub: Subscription = new Subscription();
    private coordsChangesSub: Subscription = new Subscription();
    private clickListener = (ev) => {
        if (this.showToolbar) {
            this.GIS.setMarker(ev.latLng);
            if (this.qdcservice.TestataForm.controls.flagVisita) {    //nel caso delle Visite, blocco la selezione delle Specie e il resto
                this.qdcservice.TestataForm.controls.Specie.disable();
                this.qdcservice.TestataVisitaForm.controls.Operatore_Visita.disable();
                this.qdcservice.TestataVisitaForm.controls.Azienda_Visita.disable();
                this.qdcservice.TestataVisitaForm.controls.Visualizza_Specie.disable();
                this.qdcservice.TestataVisitaForm.controls.Aziende_Agenzie.disable();
            }
        }
    };

    @ViewChild("GISPanel") GISPanelBar: GiasPanelBar;
    @ViewChild("GIS") GIS: GISComponent;
    @ViewChild("ImpiantiPanel") ImpiantiPanelBar: GiasPanelBar;

    constructor(
        public qdcservice: QdCService,
        private kendoWindowsService: KendoWindowsService,
        private translocoService: TranslocoService
    ) {
        this.updateSpecieValidity(this.qdcservice.TestataForm.controls.Specie.value);
        this.valueChangesSub = this.qdcservice.TestataForm.controls.Specie
            .valueChanges
            .subscribe(value => {
              this.updateSpecieValidity(value);

              this.qdcservice.DisableExpandImpiantiPanel();
            });

        this.operationChangeSub = this.qdcservice.TestataForm.controls
            .Operazioni.valueChanges
            .subscribe(value =>
            {
                this.checkRilievo(value);
                this.updateShowRilievoPiogge(value);
            });

    }

    ngOnInit(): void {
        const windowArgs = new WindowArgs(WindowTypes.DrawWindow, true, "Disegna", null, 150, null, 100, 100, false, false, true, true);
        this.kendoWindowsService.open(WindowTypes.DrawWindow, windowArgs);
        this.updateShowRilievoPiogge(this.qdcservice.TestataForm.controls.Operazioni.value);

        if (this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare) {
            this.qdcservice.TrovaPoligoniGIS();
            this.checkRilievo(this.qdcservice.TestataForm.controls.Operazioni.value);
        }
    }

    ngOnDestroy(): void {
        this.valueChangesSub.unsubscribe();
        this.operationChangeSub.unsubscribe();
        this.unsubscribeFromGIS();
    }

    onTabSelect(event: SelectEvent): void {
        this.isTabLoaded[event.index] = true;
        this.tabSelected = event.index;
        this.qdcservice.TrattamentoTabStripSelected.next(event.index);
    }

    HeaderTextGISPanelBar(){
        let text = this.translocoService.translate('PianoColturaleGrafico');

        if(!this.qdcservice.GISPanel.Polygon){
            text += " (" + this.translocoService.translate('qdc.NessunPoligonoTrovatoConICriteri')+")";
        }

        return text;

    }

    mostraGis(): boolean {
        const isPostRaccolta = this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare;
        if (isPostRaccolta) {
            return false;
        }

        // Other checks here...

        return true;
    }

    mostraImpianti(): boolean {
        const isPostRaccolta = this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare;
        if (isPostRaccolta) {
            return false;
        }

        // Other checks here...

        return true;
    }

    mostraProdottiMagazzino(): boolean {
        const isPostRaccolta = this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare;
        if (isPostRaccolta) {
            return true;
        }

        // Other checks here...

        return false;
    }

    private updateSpecieValidity(specie: { codice: number, descrizione: string } | null): void {
        const res = specie != null && specie.codice != -1;
        this.isSpecieSelected = res;
        if (!res) {
            this.onTabSelect({ index: 0 } as SelectEvent);
        }
    }

    private checkRilievo(lavorazioni: Lavorazione[]) {
        const isRilevo = (cod: number) => {
            const lavCodRilievi = [
                enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO,
                enum_LAVCOD.RILIEVO_ERBE_INFESTANTI,
                enum_LAVCOD.RILIEVO_INDICI_MATURITA,
                enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA,
                enum_LAVCOD.DANNI_RACCOLTA,
                enum_LAVCOD.FASI_FENOLOGICHE
            ]
            return lavCodRilievi.includes(cod);
        }
        this.showToolbar = (lavorazioni.map(lav => +lav.primaryKey.codice)
                                      .some(lavCod => isRilevo(lavCod))
                            || (this.qdcservice.TestataForm.get("flagVisita").value)
                            );

    }

    private updateShowRilievoPiogge(lavorazioni: Lavorazione[]){
        this.showRilievoPiogge = lavorazioni.findIndex(lav => +lav.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE || +lav.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;
    }

    private formatCoordinateWithSign(num: number, iDigits: number = 3, dDigits: number = 7) {
        const sign = num < 0 ? '-' : '+';
        const absNum = Math.abs(num);
        let [intPart, decPart] = absNum.toFixed(dDigits).split('.');
        intPart = intPart.padStart(iDigits, '0'); // Ensure 3-digit units
        return `${sign}${intPart}.${decPart}`;
    }

    private subscribeToGis(): void {
        const lat = this.qdcservice.TestataForm.get('Latitude').value
        const lng = this.qdcservice.TestataForm.get('Longitude').value
        this.qdcservice.TestataForm.get("Operazioni").valueChanges.GiasSubscribe(op => this.handleOperationChange(op));


        if (!!lat && !!lng) {
            this.GIS.setMarker(new google.maps.LatLng(lat, lng), true, 1500);
        }

        this.GISToolbarService.latValueSubject.next(this.formatCoordinateWithSign(lat, 2));
        this.GISToolbarService.lngValueSubject.next(this.formatCoordinateWithSign(lng, 3));

        this.latChangesSub = this.GISToolbarService.latValueSubject.subscribe(val => {
            this.qdcservice.TestataForm.get('Latitude')?.patchValue(+val);
        });
        this.lngChangesSub = this.GISToolbarService.lngValueSubject.subscribe(val => {
            this.qdcservice.TestataForm.get('Longitude')?.patchValue(+val);
        });
        this.coordsChangesSub = this.GIS.lastClickedCoords.subscribe(coords => {
            if (!coords) return;
            this.GIS.setMarker(coords);
        })

        this.GIS.mapService.googleMapWrapper.googleMap.addListener("click", this.clickListener)
    }

    private handleOperationChange(lavorazioni: Lavorazione[]) {
        const isRilievo = (lavCod: number) => {
            return lavCod === enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA
                || lavCod === enum_LAVCOD.RILIEVO_INDICI_MATURITA
                || lavCod === enum_LAVCOD.DANNI_RACCOLTA
                || lavCod === enum_LAVCOD.FASI_FENOLOGICHE
                || lavCod === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO
                || lavCod === enum_LAVCOD.RILIEVO_ERBE_INFESTANTI;
        }

        if (lavorazioni.some(op => isRilievo(+op.primaryKey.codice))) return;

        this.unsubscribeFromGIS();
    }

    private unsubscribeFromGIS() {
        this.GIS?.removeMarkers();
        this.latChangesSub.unsubscribe();
        this.lngChangesSub.unsubscribe();
        this.coordsChangesSub.unsubscribe();
        this.GISToolbarService = null;
        this.GIS?.abilitaDeselezione();
    }

    OnExpandGISPanel(event: PanelBarExpandEvent) {
        // this.qdcservice.TrattamentoTabStripSelected.next(enum_trattamentoTabIndex.PianoColturaleGrafico);
    }

    ngAfterViewInit() {
        this.qdcservice.GISPanel.Panel = this.GISPanelBar;

        this.qdcservice.ImpiantiPanelBar = this.ImpiantiPanelBar;

        this.qdcservice.DisableExpandImpiantiPanel(true);

    }

    ngAfterViewChecked(): void {
        if (this.GIS && !this.GISToolbarService && this.showToolbar
            && !!this.GIS.mapService.googleMapWrapper) {
            this.GISToolbarService = this.GIS.toolbarService;
            this.GIS.disabilitaDeselezione();
            this.subscribeToGis();
        }
    }



}
