import { Injectable } from "@angular/core";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { FeatureService } from "../services/feature.service";
import { GISAnalisiMappeSatellitariWindowService } from "./GIS-analisi-mappe-satellitari-window.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { GoogleMapService } from "../google-map/google-map.service";
import { enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GisClient, Gis_Sat_Sentinel_Overlay, MascheraLayerRaster, RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out } from "app/Service/api.service";
import { startWith, withLatestFrom, map, Observable, debounceTime, switchMap, catchError, of, tap, forkJoin, filter, pairwise, combineLatest } from "rxjs";
import { Tile } from "../utils/mercator.utils";
import { GiasMessageService } from "app/Service/gias-message.service";
import { NotificationRef } from "@progress/kendo-angular-notification";
import { GoogleMapUtils } from "../utils/google-map.utils";
import { FeatureInformationService } from "../services/feature-information.service";
import { PolygonInterceptionUtils } from "../services/polygon-interception-utils";
import { KendoWindowsService, WindowTypes } from "app/Service";

@Injectable()
export class SatelliteLocalDataLoader {
  private notificationRef: NotificationRef | null = null;

  constructor(
    private gisClient: GisClient,
    private featureService: FeatureService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private permessiUtenteService: PermessiUtenteService,
    private kendoWindowsService: KendoWindowsService,
    private googleMapService: GoogleMapService,
    private giasMessageService: GiasMessageService,
    private featureInformationService: FeatureInformationService
  ) { }

  public load(): Observable<{ layers: Gis_Sat_Sentinel_Overlay[], features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], tiles: Tile[] }> {
    const zoomChange$ = this.googleMapService.idle$.pipe(
      map(() => this.googleMapService.googleMapWrapper.getZoom()),
      startWith(this.googleMapService.googleMapWrapper.getZoom()),
      pairwise(),
      filter(([oldZoom, newZoom]) => oldZoom != newZoom),
      tap(() => {
        if (this.featureService.getFeatureSelezionate().length > 0 && this.gisAnalisiMappeSatellitariWindowService.hasOverlays()) {
          this.notificationRef?.hide();
          this.notificationRef = this.giasMessageService.infoMessagge('gis.RicaricamentoDeiDatiSatellitari', false, true);
        }
      })
    );

    return combineLatest([
      this.featureService.getFeatureSelezionate$().pipe(startWith(this.featureService.getFeatureSelezionate())),
      zoomChange$.pipe(startWith([0, 0]))
    ])
      .pipe(
        debounceTime(100),
        filter(() => this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow)),
        withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.masks$),
        map(([[features, _], masks]: [[GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], [number, number]], MascheraLayerRaster[]]) => {
          const hasPermissions = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
          const visibleMasks = masks.filter(mask => !hasPermissions || mask.isAttivaPerUtenteCorrente);
          const layers = visibleMasks.map(mask => mask.LayerElementiGrafici_cod);
          return features.filter(f => layers.includes(+f.properties.layer));
        }),
        switchMap(features => {
          const tiles = FunzioniComuniService.computeCurrentlyVisibleTiles(this.googleMapService.googleMapWrapper?.data?.getMap());
          if (features.length == 0) {
            // no features selected reset data
            return of([{ RispostaStringa: { ListaOverlayer: [] } } as RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out, []]);
          }

          if (features.length == 1) {
            // 1 feature selected, use the polygon from the feature
            const realTiles = this.getIntersectedTiles(tiles, features[0]);
            return forkJoin([this.gisClient.gisCustomMapOverlayBaseEntitaGISInizializzaCalendario(+features[0].properties.Entita_Cod), of(realTiles)]);
          }

          // more features selected, use map
          const mapBounds = GoogleMapUtils.getMapBoundsWKT(this.googleMapService.googleMapWrapper.getBounds());
          const gMap = this.googleMapService.googleMapWrapper.data.getMap();
          const geometries = features.map(f => this.featureInformationService.getGeometry(f.properties.id));
          return forkJoin([this.gisClient.gisCustomMapOverlayBaseInizializzaCalendario(mapBounds), of(FunzioniComuniService.getTilesWithFeatures(geometries, gMap))]);
        }),
        catchError(() => of([{ RispostaStringa: { ListaOverlayer: [] } } as RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out, [] as Tile[]])),
        map(([res, tiles]: [RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out, Tile[]]) => {
          const layers = res.RispostaStringa.ListaOverlayer.sort((a, b) => a.DataRiferimento.localeCompare(b.DataRiferimento));
          this.gisAnalisiMappeSatellitariWindowService.nextCalendarSatelliteData(layers);

          const groupedLayers = GISAnalisiMappeSatellitariWindowService.groupLayersByDate(layers);
          this.gisAnalisiMappeSatellitariWindowService.nextSatelliteData(groupedLayers);

          return { layers: layers, features: this.featureService.getFeatureSelezionate(), tiles: tiles };
        }),
      ) as Observable<{ layers: Gis_Sat_Sentinel_Overlay[], features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], tiles: Tile[] }>;
  }

  private getIntersectedTiles(tiles: Tile[], feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): Tile[] {
    const result: Tile[] = [];

    for (const tile of tiles) {
      const geometry = this.featureInformationService.getGeometry(feature.properties.id);
      if (PolygonInterceptionUtils.getPointPolygonIntersection(geometry as google.maps.Data.Polygon, new google.maps.Point(tile.x, tile.y), tile.zoom).length > 0) {
        result.push(tile);
      }
    }

    return result;
  }
}
