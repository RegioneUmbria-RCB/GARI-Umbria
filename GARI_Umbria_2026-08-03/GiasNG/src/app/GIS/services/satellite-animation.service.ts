import { Injectable } from "@angular/core";
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { enum_LayerElementiGraficiStd } from "../GIS-enum/GIS-layer-elementi-grafici";
import { UrlFirmato } from "app/Service/api.service";
import { MappeSatellitariData } from "../GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service";
import { PolygonInterceptionMapType } from "./polygon-interception-map-type";
import { RasterOverlayService, SATELLITE_DATA_MAP_TYPE } from "./raster-overlay.service";
import { MultipleImageMapType } from "./multiple-image-map-type";
import { FeatureInformationService } from "./feature-information.service";

@Injectable()
export class SatelliteAnimationService {

  private animationOverlay: AnimationStepData[] = [];

  private get googleMap(): google.maps.Map | null {
    return this.googleMapService.googleMapWrapper?.data?.getMap();
  }

  constructor(private googleMapService: GoogleMapService, private featureInformationService: FeatureInformationService) { }

  public loadAllFrames(data: MappeSatellitariData[]): void {
    const map = this.googleMap;
    if (map == null) {
      return;
    }

    RasterOverlayService.clearRasterOverlays(+enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI, map);
    this.animationOverlay = [];

    if (data.length > 0) {
      this.loadMapTypesAnimation(map, data);
    }
  }

  public showAnimationStep(currentDate: string, opacity: number = 1): void {
    const map = this.googleMap;
    if (map == null) {
      return;
    }

    const id = `Animation${currentDate}`;
    this.showFrame(map, id, opacity);
  }

  private loadMapTypesAnimation(map: google.maps.Map, data: MappeSatellitariData[]): void {
    for (const element of data) {
      this.loadMapTypeFrame(map, element);
    }
  }

  private loadMapTypeFrame(map: google.maps.Map, data: MappeSatellitariData): void {
    // This has been extracted from the for below to try improve performances
    // If you are experiencing a bug involving transparency try move this back in the for block
    const animationId = `Animation${data.overlay.DataRiferimento}`;
    const mapType = this.getMapType(data, 0, map);
    if (mapType == null) {
      return;
    }

    mapType.description = animationId;
    for (let i = 0; i < data.overlay.Passaggi.length; i++) {
      this.animationOverlay.push({
        Id: animationId,
        Tile: data.overlay.Passaggi[i].Tile?.Tile,
        SensoreElaborazione: data.sensor,
        DataRiferimento: data.overlay.DataRiferimento,
        Descrizione: `Sentinel 2 - ${data.overlay.DataRiferimento} - ${data.sensor}`,
        url: data.urls,
        opacity: data.opacity,
        mapType: mapType,
      });

      map.overlayMapTypes.push(mapType);
    }
  }

  private showFrame(map: google.maps.Map, id: string, opacity: number): void {
    // Reset opacity
    const overlayMapTypes = map.overlayMapTypes.getArray().filter(x => x.name == SATELLITE_DATA_MAP_TYPE);
    for (const overlay of overlayMapTypes) {
      (overlay as PolygonInterceptionMapType).setOpacity(0);
    }

    const animationId = this.parseAnimationId(id, this.animationOverlay);
    const elements = this.animationOverlay.filter(x => x.Id == animationId);
    for (const element of elements) {
      const index = this.animationOverlay.indexOf(element);
      const overlay = overlayMapTypes[index];
      if (overlay == null || overlay.name != SATELLITE_DATA_MAP_TYPE) {
        continue;
      }

      (overlay as PolygonInterceptionMapType).setOpacity(opacity);
    }
  }

  private getMapType(data: MappeSatellitariData, opacity: number, map: google.maps.Map): PolygonInterceptionMapType | MultipleImageMapType | null {
    const urls = Array.isArray(data.urls) ? data.urls : [data.urls];
    if (data.applyToPolygon) {
      const polygons = data.features.map(f => this.featureInformationService.getGeometry(f.properties.id) as google.maps.Data.Polygon);
      return new PolygonInterceptionMapType(+enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI, polygons, map, urls, opacity);
    }

    return new MultipleImageMapType(urls, opacity);
  }

  private parseAnimationId(currentId: string, stepData: AnimationStepData[]): string {
    // currentId can be in the format "Animationdd/MM/yyyy" or "Animationyyyy-MM-dd"
    // we want to standardize it to stepData format (avoid creating date objeect to avoid timezone issues)

    if (stepData.length == 0) {
      return currentId;
    }

    const targetDate = stepData[0].DataRiferimento;
    if (targetDate == null) {
      return currentId;
    }

    if (targetDate.includes('/')) {
      // target format is dd/MM/yyyy
      const dateParts = currentId.replace("Animation", "").split('-');
      if (dateParts.length != 3) {
        return currentId;
      }
      const [year, month, day] = dateParts;
      return `Animation${day}/${month}/${year}`;
    }

    if (targetDate.includes('-')) {
      // target format is yyyy-MM-dd
      const dateParts = currentId.replace("Animation", "").split('/');
      if (dateParts.length != 3) {
        return currentId;
      }
      const [day, month, year] = dateParts;
      return `Animation${day}/${month}/${year}`;
    }

    return currentId;
  }
}

interface AnimationStepData {
  Id: string;
  Tile: string;
  SensoreElaborazione: string;
  DataRiferimento: string;
  Descrizione: string;
  url: UrlFirmato | UrlFirmato[];
  opacity: number;
  mapType: PolygonInterceptionMapType | MultipleImageMapType;
}
