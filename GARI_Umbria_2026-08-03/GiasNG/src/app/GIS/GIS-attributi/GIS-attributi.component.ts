import { Component, Input, OnChanges } from '@angular/core';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import {
  ChiaveAlbero,
  ConfigurazioneSuLayer,
  GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
  GisClient,
  LeggiElencoConfigurazioni_In,
  ObjOptionHTML_Out,
  ScriviConfigurazioneGisUtente,
  TipologiaLayer
} from 'app/Service/api.service';
import {
  BehaviorSubject,
  combineLatest,
  debounceTime,
  filter,
  map,
  Observable,
  share,
  startWith,
  switchMap,
  withLatestFrom
} from 'rxjs';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { LayerService } from '../services/layer.service';
import {
  AlgorithmConfigurationWindowService
} from '../GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration-window.service';
import { FeatureService } from '../services/feature.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { SharedDataService } from '../services/shared-data.service';
import { MasterService } from 'app/Service/master.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { OpzioniAgenda } from 'app/menu-agenda/dtos/opzioni-agenda.model';

interface Tabs {
  agenda: number;
  data: number;
  settings: number;
  upload: number;
  algorithms: number;
  export: number;
  rasterMasks: number;
}

@Component({
  standalone: false,
  selector: 'GIS-attributi',
  templateUrl: './GIS-attributi.component.html',
  styleUrls: ['./GIS-attributi.component.css']
})
export class GISAttributiComponent implements OnChanges {
  @Input() open: boolean = false;

  featureType = FeatureType;
  agendaOptionDialog = false;
  opzioniAgendaGis: OpzioniAgenda = new OpzioniAgenda({ nascondiFiltri: true, source: 'GIS' });
  tabSelectedSubject = new BehaviorSubject<number>(0);
  tabSelected$ = this.tabSelectedSubject.asObservable();
  openSubject = new BehaviorSubject<boolean>(this.open);

  layerSelected$ = this.layerService
    .layerItemSelected$
    .pipe(map(x => x[1] ? x[0] : null));

  tipoLayer$ = this.layerService.LayerSelected;

  forceReload$ = this.algorithmConfigurationWindowService
    .forceReload$
    .pipe(startWith(null));

  features$ = this.featureService.getFeatureSelezionate$();

  agendaVisible$ = this.sharedDataService
    .getCfgAlberoGisUtente$()
    .pipe(map(cfgAlbero => cfgAlbero[0]?.CfgGisUtente?.ckMostraOperazioniAgenda ?? false));

  configurations$ = combineLatest([this.layerSelected$, this.forceReload$, this.openSubject.asObservable()])
    .pipe(
      filter(([_1, _2, opened]) => opened),
      debounceTime(100),
      map(([layer, _1, _2]) => layer),
      filter(layer => layer != null),
      withLatestFrom(this.tipoLayer$),
      map(([layer, tipoLayer]) => ({
        LayerElementiGrafici_Cod: +layer.id,
        Entita_Cod: 0,
        TipologiaLayer_cod: +tipoLayer.Option_Value
      } as LeggiElencoConfigurazioni_In)),
      switchMap(payload => this.gisClient.gisLeggiConfigurazioniProiezioneSuLayer(payload)),
      map(x => x.RispostaStringa.elencoConfigurazioni),
      share()
    );

  tabs$: Observable<Tabs> = combineLatest([this.layerSelected$, this.configurations$.pipe(startWith([])), this.features$, this.agendaVisible$])
    .pipe(map(([layer, configurations, features, agendaVisible]) => this.populateTabs(layer, configurations, features, agendaVisible)));

  data = combineLatest([
    this.tabSelected$,
    this.tipoLayer$,
    this.layerSelected$,
    this.tabs$,
    this.configurations$.pipe(startWith([])),
    this.agendaVisible$
  ]).pipe(map(([tab, tipoLayer, layer, tabs, configurations, agendaVisible]) => ({
    tab: tab,
    tipoLayer: tipoLayer,
    layer: layer,
    tabs: tabs,
    configurations: configurations,
    agendaVisible: agendaVisible
  })));

  constructor(
    private layerService: LayerService,
    private gisClient: GisClient,
    private algorithmConfigurationWindowService: AlgorithmConfigurationWindowService,
    private featureService: FeatureService,
    private funzioniComuniService: FunzioniComuniService,
    private sharedDataService: SharedDataService,
    private masterService: MasterService,
    private giasMessageService: GiasMessageService,
    private permessiUtenteService: PermessiUtenteService
  ) {
  }

  ngOnChanges(): void {
    this.openSubject.next(this.open);
  }

  isAnyVisible(tabs: Tabs | null): boolean {
    if (tabs == null) {
      return false;
    }

    for (const key in tabs) {
      if (Object.prototype.hasOwnProperty.call(tabs, key) && tabs[key] != -1) {
        return true;
      }
    }

    return false;
  }

  changeTab($event: SelectEvent, tabs: Tabs, agendaVisible: boolean): void {
    if (tabs.agenda != $event.index || agendaVisible) {
      this.tabSelectedSubject.next($event.index);
      return;
    }

    this.agendaOptionDialog = true;
    $event.preventDefault();
  }

  setAgendaVisible(): void {
    const config = this.sharedDataService.getCfgAlberoGisUtente()[0];
    if (config?.CfgAlbero == null || config?.CfgGisUtente == null) {
      // Non dovrebbe mai succedere
      this.giasMessageService.errorMessage('SiEVerificatoUnErrore', false, true);
      return;
    }

    config.CfgGisUtente.ckMostraOperazioniAgenda = true;
    const payload = {
      MemorizzaSistemaDiRiferimentoPredefinito: false,
      MemorizzaOperazioneColturale: false,
      CfgGisUtente: FunzioniComuniService.getStandardConfigurazioneGisUtente(config.CfgGisUtente)
    } as ScriviConfigurazioneGisUtente;

    this.masterService.set_isLoading({ isLoading: true })
    this.gisClient
      .gisCfgGISSalva(payload)
      .subscribe({
        next: () => {
          this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
          config.CfgAlbero.Flag_Agenda = true;
          this.sharedDataService.setCfgAlberoGisUtente(config, true);
        },
        error: () => this.giasMessageService.errorMessage('SiEVerificatoUnErrore', false, true),
        complete: () => {
          this.masterService.set_isLoading({ isLoading: false })
          this.agendaOptionDialog = false;
        }
      });
  }

  isMuz(layer: TipologiaLayer, tipoLayer: ObjOptionHTML_Out): boolean {
    return layer.id == enum_LayerElementiGraficiStd.Muz && tipoLayer.Option_Value == enum_TipologiaLayer.Entita
  }

  private isAgendaVisible(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): boolean {
    const isImpianto = this.isFeatureImpianto(features);
    return isImpianto;
  }

  public static isDatiVisible(layerId: enum_LayerElementiGraficiStd | null): boolean {
    return layerId != enum_LayerElementiGraficiStd.CATASTO && layerId != enum_LayerElementiGraficiStd.IMPIANTI;
  }

  private isSettingsVisible(_: TipologiaLayer): boolean {
    return false;
  }

  private isUploadVisible(layer: TipologiaLayer): boolean {
    return layer?.id != enum_LayerElementiGraficiStd.IMPIANTI;
  }

  private isAlgorithmConfigurationVisible(_: TipologiaLayer, configurations: ConfigurazioneSuLayer[]): boolean {
    return configurations.length > 0;
  }

  private isExportVisible(_: TipologiaLayer): boolean {
    return this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_Esporta_Dati, 2);
  }

  private isRasterMasksVisible(layer: TipologiaLayer): boolean {
    const hasPermission = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 2);
    return +layer.FeatureTypeId == FeatureType.Raster && hasPermission;
  }

  private populateTabs(layer: TipologiaLayer, configurations: ConfigurazioneSuLayer[], features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], agendaVisible: boolean): Tabs {
    if (layer == null) {
      return;
    }

    let tabsCounter = 0;
    const result = {
      agenda: -1,
      data: -1,
      settings: -1,
      upload: -1,
      algorithms: -1,
      export: -1,
      rasterMasks: -1
    } as Tabs;

    if (this.isAgendaVisible(features)) {
      result.agenda = tabsCounter++;
      this.opzioniAgendaGis.impianti = this.getImpiantiForOpzioniAgenda(features);
    }

    if (GISAttributiComponent.isDatiVisible(layer?.id as enum_LayerElementiGraficiStd)) {
      result.data = tabsCounter++;
    }

    if (this.isSettingsVisible(layer)) {
      result.settings = tabsCounter++;
    }

    if (this.isUploadVisible(layer)) {
      result.upload = tabsCounter++;
    }

    if (this.isAlgorithmConfigurationVisible(layer, configurations)) {
      result.algorithms = tabsCounter++;
    }

    if (this.isExportVisible(layer)) {
      result.export = tabsCounter++;
    }

    if (this.isRasterMasksVisible(layer)) {
      result.rasterMasks = tabsCounter++;
    }

    // if agenda visible
    if (result.agenda == 0) {
      // if only agenda is visible but i don't have permissions, ask the user
      if (tabsCounter == 1 && !agendaVisible) {
        this.agendaOptionDialog = true;
      }
      // if only agenda is visible but there are more tabs, select 2nd one
      else if (tabsCounter > 1 && !agendaVisible) {
        this.tabSelectedSubject.next(1);
      }
    }

    return result;
  }

  private isFeatureImpianto(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): boolean {
    if (features.length === 0) {
      return false;
    }

    const indiceUltimaFeature = features.length - 1;
    const chiaveAlbero = this.featureService.getChiaveAlberoCompletaByFeature(features[indiceUltimaFeature]);
    const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);
    return this.funzioniComuniService.isTipoNodoImpianto(objChiaveAlbero.TipoNodo);
  }

  private getImpiantiForOpzioniAgenda(features) {
    return features.map(f => f.properties.chiavealbero)
      .map(chiaveAlbero => FunzioniComuniService.chiaveAlberoRidottaToBig(chiaveAlbero))
      .map(k => FunzioniComuniService.scomponiChiaveAlbero(k))
      .map((k: ChiaveAlbero) => [k.Piva, k.Sa_Cod ?? 0, k.Appezza ?? 0, k.Id_Imp ?? 0].join('_'))
      .map(k => ({ chiave: k, des: '' }));
  }
}
