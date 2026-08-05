import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
  OnDestroy,
  QueryList,
  Renderer2,
  RendererFactory2,
  ViewChild,
  ViewChildren
} from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { PageChangeEvent } from '@progress/kendo-angular-pager';
import { FixedLayerProperty } from 'app/Model/GIS/FixedLayerProperty';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { DatiLayer, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { combineLatest, debounceTime, filter, map, merge, Observable, pairwise, startWith, Subject, take, takeUntil, tap } from 'rxjs';
import { GISModality, enum_FeatureGeometryType, enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { GISWindowComponent } from '../GIS-window/GIS-window.component';
import { GoogleMapService } from '../google-map/google-map.service';
import { LayerService } from '../services/layer.service';
import { SharedDataService } from '../services/shared-data.service';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { FeatureInformationService } from '../services/feature-information.service';
import { GoogleMapGeoJsonService } from '../google-map/google-map-geojson.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';


@Component({
  standalone: false,
  selector: 'gis-layer-window',
  templateUrl: './GIS-layer-window.component.html',
  styleUrls: ['./GIS-layer-window.component.css']
})
export class GISLayerWindowComponent implements OnDestroy, AfterViewInit {
  @ViewChild('window') window: GISWindowComponent;
  @ViewChildren('list') els: QueryList<ElementRef>;
  @Input() modality: GISModality | undefined = GISModality.Full;

  windowArgs: WindowArgs;
  dropdownData: ObjOptionHTML_Out[];
  layerSelected: ObjOptionHTML_Out | null = null;
  layersData: TipologiaLayer[] = [];
  pagedLayersData: TipologiaLayer[] = [];
  fixedLayersData: TipologiaLayer[] = [];
  itemSelected: TipologiaLayer;
  itemSelectedColor: string = 'none';
  enum_LayerElementiGraficiStd = enum_LayerElementiGraficiStd;
  enum_TipologiaLayer = enum_TipologiaLayer;
  enum_FeatureGeometryType = enum_FeatureGeometryType

  // isSnippetOpen = false;
  groupLabel = true;
  layerGrouping = true;
  showLabel = true;
  allLayerVisible = true;
  pageSize = 5;
  skip = 0;
  total = 0;
  filterValue = '';
  featureNumber = 0;

  mappePrescrizioneEngineActive$: Observable<boolean>;

  private saveNotifier = new Subject();
  private originalTitle: string | null = null;
  private renderer: Renderer2;

  private visibleFeatures = new Map<string, number>();
  private featuresNumberByLayer = new Map<string, number>();
  private mapBounds: google.maps.LatLngBounds | null = null;

  private signal = new Subject<void>();

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private layerService: LayerService,
    private sharedDataService: SharedDataService,
    private rendererFactory: RendererFactory2,
    private googleMapService: GoogleMapService,
    private changeDetector: ChangeDetectorRef,
    private transloco: TranslocoService,
    private featureInformationService: FeatureInformationService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {
    this.renderer = this.rendererFactory.createRenderer(null, null);

    this.mappePrescrizioneEngineActive$ = this.configurazioneSitiService
      .leggiChiave(EnumChiaviConfigurazioneSiti.MappePrescrizioneEngineIsActive)
      .pipe(
        map(config => config?.Valore?.toLowerCase() === 'true'),
        startWith(false)
      );

    const windowToggle$ = this.kendoWindowsService.windowToggle$
      .pipe(filter(([windowTypes, _]) => windowTypes === WindowTypes.LayerWindow));

    windowToggle$.pipe(takeUntil(this.signal)).subscribe(([windowTypes, args]) => {
      if (windowTypes === WindowTypes.LayerWindow) {
        this.windowArgs = { ...args };
        this.originalTitle = this.windowArgs.title;
        this.updateTitle(this.itemSelected, true);
      }
    });

    this.layerService.LayerItems
      .pipe(takeUntil(this.signal)).subscribe((items) => {
        this.dropdownData = items;
      });

    this.layerService.LayerSelected
      .pipe(takeUntil(this.signal)).subscribe(type => {
        this.layerSelected = type;
        this.layerService.refreshLayers(type);
      });

    this.layerService.ObservableLayer
      .pipe(takeUntil(this.signal)).subscribe(result => {
        this.layersData = result;
        this.total = this.layersData.length;
        this.skip = 0;
        this.getFixedLayers();
        if (this.sharedDataService.selezionatoTipoLayerEntita()) {
          this.fixedLayersData.forEach(fxLayersData => {
            if (fxLayersData.id === enum_LayerElementiGraficiStd.WMS) {
              let fxLayerProperty = new FixedLayerProperty();
              fxLayerProperty.Trasparenza = parseInt(this.layerService.getSliderTransparencyByOpacity(fxLayersData.trasparenza));
              fxLayerProperty.InfoClickMappa = this.sharedDataService.getFixedLayerProperty().InfoClickMappa;
              this.sharedDataService.setFixedLayerProperty(fxLayerProperty);
            }
          });
        }
        this.pageData();
      });

    merge(
      this.layerService.ObservableLayer,
      this.featureInformationService.featuresChanged$.pipe(map(() => this.layersData))
    )
      .pipe(
        filter(layers => layers.length > 0),
        takeUntil(this.signal))
      .subscribe(layers => {
        this.featureNumber = this.featureInformationService.getNumber();
        layers.forEach(layer => {
          const features = this.featureInformationService.getByLayer(layer.id);
          const total = features.reduce((acc, feature) => (acc + (feature.properties.Clustered == 'True' ? +feature.properties.Testo : 1)), 0);
          this.featuresNumberByLayer.set(layer.id, total);
        });
      });

    this.layerService
      .layerItemSelected$
      .pipe(takeUntil(this.signal)).subscribe(([item, selected]) => {
        if (selected) {
          this.itemSelected = item;
          if (+this.itemSelected.id > 0) {
            this.goToPage(item);
          }
          this.window?.setFocus();
        }

        this.updateTitle(item, selected);
      });

    this.layerService
      .GroupLabels
      .pipe(takeUntil(this.signal))
      .subscribe(raggruppaEtichette => {
        this.groupLabel = raggruppaEtichette;
      });

    this.layerService
      .ShowLabels
      .pipe(takeUntil(this.signal))
      .subscribe(visualizzaEtichette => {
        this.showLabel = visualizzaEtichette;
        for (const layer of this.layersData) {
          this.layerService.toggleLayerItemLabelVisible(layer, this.showLabel);
        }
        this.saveNotifier.next(null);
      });

    this.layerService
      .AllLayersVisible
      .pipe(takeUntil(this.signal))
      .subscribe(visualizzaTuttiLayer => {
        this.allLayerVisible = visualizzaTuttiLayer;
        for (const layer of this.layersData) {
          if (+layer.id < 0) {
            continue;
          }

          this.layerService.toggleLayerItemVisible(layer, this.allLayerVisible);
        }
      });

    this.layerService
      .AllLayerGrouping
      .pipe(takeUntil(this.signal))
      .subscribe(raggruppaTuttiLayer => {
        this.layerGrouping = raggruppaTuttiLayer;
        for (const layer of this.layersData) {
          this.layerService.toggleLayerItemGrouping(layer, this.layerGrouping);
        }
      });

    this.saveNotifier
      .pipe(debounceTime(1500))
      .subscribe(() => this.submit());

    merge(
      this.layerService.reloadPageNotifier.asObservable().pipe(map(() => this.mapBounds)),
      this.googleMapService.idle$.pipe(map(() => this.googleMapService.googleMapWrapper.getBounds()))
    )
      .pipe(
        filter(bounds => bounds != null),
        takeUntil(this.signal))
      .subscribe(bounds => {
        this.mapBounds = bounds;
        this.pagedLayersData.forEach(layer => {
          const features = this.featureInformationService.getByLayer(layer.id);
          const positions = features.map(x => [x, this.featureInformationService.getPosition(x.properties.id)]) as [GeoJson_Feature_New_1OfGeoJSONAgroGisProp, google.maps.LatLng][];

          let elementsCounter = 0;
          positions.forEach(([feature, position]) => {
            if (bounds?.contains(position)) {
              elementsCounter += (feature.properties.Clustered == 'True' ? +feature.properties.Testo : 1);
            }
          });
          this.visibleFeatures.set(layer.id, elementsCounter);
        });
        this.changeDetector.detectChanges();
      });

    // if a layer was deleted, reset selection if it was the one selected at the moment
    this.layerService.layerDeleted.pipe(
      takeUntil(this.signal)
    ).subscribe(layer => {
      if (layer?.id === this.itemSelected?.id) {
        this.setSelectedItem(null);
        this.updateTitle(null, false);
      }
    });

    this.kendoWindowsService.getWindowArgs$(WindowTypes.AnalisiMappeSatellitariWindow)
      .pipe(
        takeUntil(this.signal),
        map(args => args?.openState ?? false),
        startWith(false),
        pairwise(),
        filter(([oldValue, newValue]) => oldValue !== newValue),
        tap(([_, visible]) => {
          if (visible) this.kendoWindowsService.close(WindowTypes.LayerWindow);
          else this.kendoWindowsService.open(WindowTypes.LayerWindow);
        })
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.signal.next();
    this.signal.complete();
    this.saveNotifier.complete();
  }

  ngAfterViewInit(): void {
    const windowToggle$ = this.kendoWindowsService.windowToggle$
      .pipe(filter(([windowTypes, _]) => windowTypes === WindowTypes.LayerWindow));

    const initialLayer$ = this.layerService.LayerSelected.asObservable()
      .pipe(filter(layer => layer != null));

    combineLatest([initialLayer$, windowToggle$])
      .pipe(take(1))
      .subscribe(([layer, _]) => {
        if (layer.Option_Value == enum_TipologiaLayer.Entita) {
          return;
        }

        const args = { ...this.kendoWindowsService.getWindowArgs(WindowTypes.LayerWindow), startMinimized: false, state: 'default' } as WindowArgs;
        this.kendoWindowsService.open(WindowTypes.LayerWindow, args);
      });
  }

  get isSmartphone(): boolean {
    return window.innerWidth < SMARTPHONE_WIDTH;
  }

  selectLayer(type: ObjOptionHTML_Out) {
    this.setSelectedItem();
    this.layerService.LayerSelectedTrigger(type);
  }

  // toggleSnippet() {
  //     this.isSnippetOpen = !this.isSnippetOpen;
  // }

  drop(event: CdkDragDrop<TipologiaLayer[]>) {
    this.setSelectedItem();

    moveItemInArray(this.pagedLayersData, event.previousIndex, event.currentIndex);
    this.pagedLayersData.forEach((item, index) => item.zindex = (index + this.skip).toString());

    this.layerService.ObservableLayer.next(this.layersData);
    this.saveNotifier.next(null);
  }

  submit() {
    let payloadData: DatiLayer[] = this.layersData?.map(item => ({
      ID: item.id,
      Colore_Primario: item.colore_1,
      Colore_Secondario: null,
      Varianza: item.varianza,
      Trasparenza: item.trasparenza?.replace(',', '.'),
      ZIndex: item.zindex,
      Flag_Visibile: 1, //scommentare se si vuole salvare visibilità isNaN(+item.flagvisibile) ? 1 : +item.flagvisibile,
      MostraDescrizioneAssociata: item.MostraDescrizioneAssociata,
      TipologiaLayer_Cod: this.layerSelected.Option_Value
    } as DatiLayer));
    if (payloadData.length > 0) {
      this.layerService.submit(1, payloadData).subscribe();
    }
  }

  toggleSelectedItem(event: MouseEvent, item: TipologiaLayer) {
    event.stopPropagation();
    if (this.itemSelected?.id === item.id) {
      this.setSelectedItem();
      return;
    }

    this.setSelectedItem(item);
    this.itemSelectedColor = item.colore_1;
  }

  setStyle(item: TipologiaLayer): { [p: string]: any } | null | undefined {
    if (this.itemSelected?.id === item.id) {
      return { 'color': `#${item.colore_1}` };
    } else {
      return undefined;
    }
  }

  onPageChange(event: PageChangeEvent) {
    this.skip = event.skip;
    this.pageSize = event.take;
    this.pageData();
  }

  onFilterChange(): void {
    this.skip = 0;
    this.pageData();
  }

  toggleAllLayerGroupLabel() {
    this.layerService.GroupLabels.next(!this.groupLabel);
  }

  toggleAllLayerGrouping() {
    this.layerService.AllLayerGrouping.next(!this.layerGrouping);
  }

  toggleAllLayersLabels() {
    this.layerService.ShowLabels.next(!this.showLabel);
  }

  toggleAllLayersVisible() {
    this.layerService.AllLayersVisible.next(!this.allLayerVisible);
  }

  openLayerVisibilityConfigurationWindow() {
    const layerWindow = this.kendoWindowsService.getWindowArgs(WindowTypes.LayerWindow);
    const height = 500;
    const width = 600;
    const left = layerWindow.left - width - 10;
    const windowArgs = new WindowArgs(WindowTypes.LayerVisibilityConfigurationWindow, true, this.transloco.translate("gis.ImpostazioniLayers"), height, width, undefined, left, layerWindow.top, true, true, true);
    this.kendoWindowsService.open(WindowTypes.LayerVisibilityConfigurationWindow, windowArgs);
  }

  getNumberOfFeatures(idLayer?: string): number {
    if (idLayer == null) {
      return this.featureNumber;
    }

    return this.featuresNumberByLayer.get(idLayer);
  }

  isFullModality(): boolean {
    return this.modality === GISModality.Full;
  }

  get toolbarVisibility(): boolean {
    return (
      (!this.isSmartphone && this.isFullModality()) ||
      this.modality === GISModality.PaesePoligoniMultiAzienda
    );
  }

  private pageData() {
    const layers = this.getLayersFilteredAndSorted();
    this.total = layers.length;
    this.pagedLayersData = layers.slice(this.skip, this.skip + this.pageSize);
    this.visibleFeatures.clear();
    this.layerService.reloadPageNotifier.next(null);
  }

  private goToPage(item: TipologiaLayer) {
    this.filterValue = '';
    const layers = this.getLayersFilteredAndSorted();
    this.skip = Math.floor(layers.findIndex(x => x.id === item.id) / this.pageSize) * this.pageSize;
    this.pageData();
  }

  private getLayersFilteredAndSorted(): TipologiaLayer[] {
    return this.layersData
      .filter(x => +x.id >= 0)
      .filter(x => x.nome.toLowerCase().includes(this.filterValue.toLowerCase()))
      .sort((a, b) => +a?.zindex - +b?.zindex);
  }

  private setSelectedItem(item: TipologiaLayer = null) {
    const selected = item ?? this.itemSelected;
    if (selected != null) {
      this.layerService.toggleLayerItemSelected(selected, item != null);
    }

    this.itemSelected = item;
  }

  private getFixedLayers() {
    this.fixedLayersData = this.layersData
      .filter(x => +x.id < 0);
  }

  private updateTitle(item: TipologiaLayer, selected: boolean): void {
    if (this.windowArgs == null) {
      return;
    }

    if (item == null || !selected) {
      this.windowArgs.title = this.originalTitle;
      return;
    }

    this.windowArgs.title = `${this.originalTitle} - ${this.itemSelected.nome}`;
  }
}
