import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GisClient, Gis_Sat_Sentinel_Overlay, RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out } from "app/Service/api.service";
import { Observable, debounceTime, switchMap, catchError, of, map, tap, filter, startWith } from "rxjs";
import { GISAnalisiMappeSatellitariWindowService } from "./GIS-analisi-mappe-satellitari-window.service";
import { GoogleMapService } from "../google-map/google-map.service";
import { Injectable } from "@angular/core";
import { Tile } from "../utils/mercator.utils";
import { GiasMessageService } from "app/Service/gias-message.service";
import { GoogleMapUtils } from "../utils/google-map.utils";
import { FeatureInformationService } from "../services/feature-information.service";
import { KendoWindowsService, WindowTypes } from "app/Service";

const GIS_MAPPA_IDLE_DISTANZA_UPDATE = 30000;

@Injectable()
export class SatelliteGlobalDataLoader {
  private lastBounds: { bounds: google.maps.LatLngBounds, zoom: number } | null = null;

  constructor(
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private googleMapService: GoogleMapService,
    private featureInformationService: FeatureInformationService,
    private kendoWindowsService: KendoWindowsService
  ) { }

  public load(): Observable<{ layers: Gis_Sat_Sentinel_Overlay[], features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], tiles: Tile[] }> {
    let firstLoad = true;
    this.lastBounds = null;
    return this.googleMapService.idle$.pipe(
      startWith({}),
      debounceTime(100),
      map(() => this.googleMapService.googleMapWrapper.getBounds()),
      filter(bounds => this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow) && this.checkAndUpdateBounds(bounds, this.googleMapService.googleMapWrapper.getZoom())),
      switchMap(bounds => {
        const wkt = GoogleMapUtils.getMapBoundsWKT(bounds);
        return this.gisClient.gisCustomMapOverlayBaseInizializzaCalendario(wkt);
      }),
      catchError(() => of({ RispostaString: { ListaOverlayer: [] } } as RispostaStandard_1OfLista_GisSat_SentinelOverlay_Out)),
      map(x => x.RispostaStringa.ListaOverlayer.sort((a, b) => a.DataRiferimento.localeCompare(b.DataRiferimento))),
      tap((layers: Gis_Sat_Sentinel_Overlay[]) => {
        this.gisAnalisiMappeSatellitariWindowService.nextCalendarSatelliteData(layers);

        const groupedLayers = GISAnalisiMappeSatellitariWindowService.groupLayersByDate(layers);
        this.gisAnalisiMappeSatellitariWindowService.nextSatelliteData(groupedLayers);

        if (layers.length == 0) {
          this.giasMessageService.errorMessage('gis.NessunDatoSatellitareDisponibileInQuestaZona', false, true);
        } else if (firstLoad) {
          this.giasMessageService.infoMessagge('gis.VerrannoCaricatiGliOverlayCompleti', false, true);
          firstLoad = false;
        } else {
          this.giasMessageService.infoMessagge('gis.RicaricamentoDeiDatiSatellitari', false, true);
        }
      }),
      map((layers: Gis_Sat_Sentinel_Overlay[]) => {
        const gmap = this.googleMapService.googleMapWrapper.data.getMap();
        const features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] = [];
        this.featureInformationService.getAll().forEach(f => {
          const guid = f.properties.Entita_GUID;
          if (guid != null && guid != '') {
            features.push(f);
          }
        });
        const geometries = features.map(f => this.featureInformationService.getGeometry(f.properties.id));
        return { layers: layers, features: [], tiles: FunzioniComuniService.getTilesWithFeatures(geometries, gmap) };
      })
    );
  }

  private checkAndUpdateBounds(currentBounds: google.maps.LatLngBounds, zoom: number): boolean {
    // Primo update
    if (this.lastBounds == null || this.lastBounds.zoom != zoom) {
      this.lastBounds = { bounds: currentBounds, zoom: zoom };
      return true;
    }

    //lat, lng precedenti
    const latitude1 = this.lastBounds.bounds.getNorthEast().lat();
    const longitude1 = this.lastBounds.bounds.getNorthEast().lng();

    //lat, lgn correnti
    const latitude2 = currentBounds.getNorthEast().lat();
    const longitude2 = currentBounds.getNorthEast().lng();

    const distance = google.maps.geometry.spherical.computeDistanceBetween(new google.maps.LatLng(latitude1, longitude1), new google.maps.LatLng(latitude2, longitude2));
    if (distance > GIS_MAPPA_IDLE_DISTANZA_UPDATE) {
      //re-imposto distanza precedente su corrente..:
      this.lastBounds = { bounds: currentBounds, zoom: zoom };
      return true;
    }

    return false;
  }
}
