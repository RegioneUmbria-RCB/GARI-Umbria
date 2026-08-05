import { Component } from '@angular/core';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { FeatureService } from 'app/GIS/services/feature.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { GisClient, LeggiElencoConfigurazioni_In } from 'app/Service/api.service';
import { BehaviorSubject, combineLatest, debounceTime, filter, map, Observable, share, startWith, switchMap, withLatestFrom } from 'rxjs';
import { AlgorithmConfigurationWindowService } from './GIS-algorithm-configuration-window.service';

@Component({
  standalone: false,
  selector: 'gis-algorithm-configuration-window',
  templateUrl: './GIS-algorithm-configuration-window.component.html',
  styleUrls: ['./GIS-algorithm-configuration-window.component.css']
})
export class GisAlgorithmConfigurationWindowComponent {

  windowArgs$: Observable<WindowArgs> = this.kendoWindowsService
    .windowToggle$
    .pipe(
      filter(([windowTypes, _]) => windowTypes == WindowTypes.AlgorithmConfigurationWindow),
      map(([_, args]) => args)
    );

  private feature$ = combineLatest([
    this.featureService.getFeatureSelezionate$(),
    this.windowArgs$
  ])
    .pipe(
      filter(([_, args]) => args.openState),
      map(([features, _]) => features[0])
    );

  private tipoLayer$ = this.layerService.LayerSelected;

  private layer$ = this.layerService
    .layerItemSelected$
    .pipe(map(([layer, selected]) => selected ? layer : null));

  tabSelectedSubject = new BehaviorSubject<number>(0);

  forceReload$ = this.algorithmConfigurationWindowService
    .forceReload$
    .pipe(startWith(null));

  configurations$ = combineLatest([this.feature$, this.forceReload$])
    .pipe(
      debounceTime(100),
      map(([feature, _]) => feature),
      filter(feature => feature != null),
      withLatestFrom(this.tipoLayer$),
      map(([feature, tipoLayer]) => ({ LayerElementiGrafici_Cod: +feature.properties.layer, Entita_Cod: +feature.properties.Entita_Cod, TipologiaLayer_cod: +tipoLayer.Option_Value } as LeggiElencoConfigurazioni_In)),
      switchMap(payload => this.gisClient.gisLeggiConfigurazioniProiezioneSuLayer(payload)),
      map(x => x.RispostaStringa.elencoConfigurazioni),
      share(),
      startWith([])
    );

  data$ = combineLatest([
    this.feature$,
    this.tabSelectedSubject.asObservable(),
    this.tipoLayer$,
    this.layer$
  ]).pipe(map(([feature, tab, tipoLayer, layer]) => ({
    feature: feature,
    tab: tab,
    tipoLayer: tipoLayer,
    layer: layer
  })));

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private featureService: FeatureService,
    private gisClient: GisClient,
    private layerService: LayerService,
    private algorithmConfigurationWindowService: AlgorithmConfigurationWindowService
  ) { }
}
