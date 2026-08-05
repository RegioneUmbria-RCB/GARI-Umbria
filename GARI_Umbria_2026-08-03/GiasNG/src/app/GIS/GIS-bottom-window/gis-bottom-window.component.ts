import { Component, AfterViewChecked, NgZone, OnDestroy } from '@angular/core';
import { enum_TreeContext } from 'app/Utility/Template/kendo-tree/enum/tree-context';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { ProvideAnagraficaTreeDeps } from 'app/Utility/Template/kendo-tree/utility/providers';
import { filter, map, startWith, Subject, takeUntil } from 'rxjs';
import { FeatureService } from '../services/feature.service';
import { SharedDataService } from '../services/shared-data.service';
import { LayerService } from '../services/layer.service';
import { TipologiaLayer } from '../../Service/api.service';
import { enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { EditFeatureService } from '../services/edit-feature.service';
import { DrawingService } from '../services/drawing.service';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { GISAttributiFileUploadService } from '../GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/net-core6-api.service';

@Component({
    standalone: false,
    selector: 'gis-bottom-window',
    templateUrl: './gis-bottom-window.component.html',
    styleUrls: ['./gis-bottom-window.component.css'],
    providers: [...ProvideAnagraficaTreeDeps()]
})
export class GisBottomWindow implements AfterViewChecked, OnDestroy {

    isRegistryOpen: boolean = false;
    isScheduleOpen: boolean = false;
    scheduleLeft: string = '0px';
    visualizzazioneTotale: boolean = false;

    destroy: boolean = true;

    isAttributi: boolean = false;

    private ultimaFeatureSelezionata: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;
    // famiglia di layer selezionati dalla ddl nella layer window
    tipoLayerSelezionato = "";
    // layer selezionato nella layer window
    layerItemSelected: [TipologiaLayer, boolean] = [null, false];
    agendaLoading$ = this.gisBottomWindowService.agendaLoading$;

    isAttributiVisible$ = this.kendoWindowsService.windowToggle$.pipe(
        filter(([windowType, _]) => windowType == WindowTypes.AnalisiMappeSatellitariWindow),
        map(([_, args]) => !args.openState),
        startWith(true)
    );

    readonly treeContextGis = enum_TreeContext.Gis;

    private signal = new Subject<void>();

    constructor(
        private treeContainerService: TreeContainerService,
        private zone: NgZone,
        private sharedDataService: SharedDataService,
        private featureService: FeatureService,
        private layerService: LayerService,
        private editFeatureService: EditFeatureService,
        private drawingService: DrawingService,
        private gisBottomWindowService: GISAttributiFileUploadService,
        private kendoWindowsService: KendoWindowsService
    ) {


        this.featureService.getFeatureSelezionate$()
            .pipe(takeUntil(this.signal)).subscribe(featureSelezionate => {
                if (this.isCampionamenti())
                    this.destroyAndRebuiltAttributiComponent();

                const indiceUltimaFeature = featureSelezionate.length - 1;
                this.ultimaFeatureSelezionata = featureSelezionate[indiceUltimaFeature];
            });

        this.sharedDataService.getVisualizzazioneTotale$()
            .pipe(takeUntil(this.signal)).subscribe(visualizzazioneTotale => {
                if (this.isScheduleOpen) {
                    this.toggleSchedule();
                }
                this.visualizzazioneTotale = visualizzazioneTotale;
            });

        this.sharedDataService.TipoLayerSelezionatoSource
            .pipe(takeUntil(this.signal)).subscribe(tipoLayerSelezionatoSource => {
                if (tipoLayerSelezionatoSource != '') {
                    this.destroyAndRebuiltAttributiComponent();
                    this.tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();
                }
            });

        this.layerService.layerItemSelected$
            .pipe(takeUntil(this.signal)).subscribe(layer => {
                if (
                    !this.layerItemSelected[0] ||
                    this.ultimaFeatureSelezionata?.properties.layer != this.layerItemSelected[0]?.id
                ) {
                    this.layerItemSelected = layer;
                    this.destroyAndRebuiltAttributiComponent();
                }

                this.layerItemSelected = layer;
            });

        this.editFeatureService.editFeature.pipe(takeUntil(this.signal)).subscribe(f => {
            if (f.getProperty(enum_FeatureProperty.layer) != '19' && f.getProperty(enum_FeatureProperty.layer) != '1') {
                this.isScheduleOpen = false;
                this.toggleSchedule();
            }
        });

        this.drawingService.addEvent.subscribe(d => {
            if (this.layerItemSelected[0].id == enum_LayerElementiGraficiStd.IMPIANTI) {
                return;
            }

            this.isScheduleOpen = false;
            this.toggleSchedule();
        });

        this.sharedDataService.closeAttributiTable.pipe(takeUntil(this.signal)).subscribe(() => {
            this.destroyAndRebuiltAttributiComponent();
        });

        this.featureService.featureDeleted.pipe(takeUntil(this.signal)).subscribe((chiave) => {
            this.destroyAndRebuiltAttributiComponent();
        });

        this.sharedDataService.geoJsonLoaded.subscribe(v => {
            this.destroyAndRebuiltAttributiComponent();
        });

        this.tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();

    }

    private destroyAndRebuiltAttributiComponent() {
        // le due righe sotto servono per distruggere e ricreare il component
        this.destroy = true;

        setTimeout(() => {
            this.destroy = false;
            // disattivo l'apertura automatica per tutti i layer
            this.isScheduleOpen = true;
            this.toggleSchedule();
        }, 100);
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    public isCampionamenti() {
        return this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.CAMPIONAMENTI;
    }

    ngAfterViewChecked() {
        this.setScheduleLeft();
    }

    public toggleRegistry() {
        this.isRegistryOpen = !this.isRegistryOpen;
        this.treeContainerService.expander.next(!this.treeContainerService.expander.value);
        this.zone.run(() => {
            this.setScheduleLeft();
        });
    }

    public toggleSchedule() {
        if (this.isScheduleDisabled()) {
            this.zone.run(() => {
                this.isScheduleOpen = false;
            });
            return;
        }

        this.zone.run(() => {
            this.isScheduleOpen = !this.isScheduleOpen;
            this.setScheduleLeft();
        });
    }

    isScheduleOpenable(): boolean {
        this.isAttributi = false;
        if (this.ultimaFeatureSelezionata != undefined)
            this.isAttributi = this.featureService.getPermessiFeature(this.ultimaFeatureSelezionata).informazioni;
        if (this.layerItemSelected != undefined && this.layerItemSelected[1])
            this.isAttributi = this.sharedDataService.getPermessiLayer(this.layerItemSelected[0]).informazioni;

        // TODO Salvo: al momento gestisco così l'autorizzazione ad aprire la tab per i CAMPIONAMENTI perché hanno permessi.informazioni == false
        if (this.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.CAMPIONAMENTI)
            this.isAttributi = true;

        if (this.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.CATASTO)
            this.isAttributi = true;

        if (this.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.IMPIANTI)
            this.isAttributi = true;

        if (this.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.Mappe_Prescrizione)
            this.isAttributi = true;

        if (this.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.Fabbricati)
            this.isAttributi = true;

        return this.isAttributi;
    }

    isScheduleDisabled() {
        return !this.isScheduleOpenable() && !this.isScheduleOpen;
    }

    setScheduleLeft() {
        if (window.innerWidth < 1000 && this.isScheduleOpen === true) {
            this.scheduleLeft = '0px';
        } else {
            this.scheduleLeft = (document.querySelector('.registry-container') as HTMLElement)?.offsetWidth + 'px';
        }
    }
}
