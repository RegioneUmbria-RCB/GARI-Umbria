import { Injectable } from "@angular/core";
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { Tile } from "../utils/mercator.utils";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { AuthDispatcherClient, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, MascheraLayerRaster, RichiestaAbacoType, RichiestaAbacoUrl_In, RichiestaSignedUrl_In, UrlFirmato } from "app/Service/api.service";
import { map, switchMap, Observable, of } from "rxjs";
import { PolygonInterceptionMapType } from "./polygon-interception-map-type";
import { FeatureInformationService } from "./feature-information.service";

export const TILE_SIZE = 256;
export const TILE_OVERLAY_LOADING_MAX_ZOOM = 21;
export const TILE_OVERLAY_LOADING_MIN_ZOOM = 0;
export const SATELLITE_DATA_MAP_TYPE = 'CustomMapType';

export interface RasterParameterVisualizationLayer {
  type: RasterParameterVisualizationType,
  baseUrl: string,
  bucket: string,
  obj: string,
  guid: string;
  views: string[] | null;
}

export enum RasterParameterVisualizationType {
  PRIVATE = 1,
  SATELLITE = 2,
  PUBLIC = 3,
  ABACO_PUBLIC = 4,
  ABACO_PRIVATE = 5,
}

@Injectable()
export class RasterOverlayService {
  constructor(
    private googleMapService: GoogleMapService,
    private authDispatcherClient: AuthDispatcherClient,
    private featureInformationService: FeatureInformationService
  ) { }

  public load(rasterId: number, masks: MascheraLayerRaster[], layerParams: RasterParameterVisualizationLayer[], features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], hasPermissions: boolean, opacity: number = 1): void {
    const gMap = this.googleMapService.googleMapWrapper.data.getMap();
    const zoom = gMap.getZoom();
    if (zoom > TILE_OVERLAY_LOADING_MAX_ZOOM || zoom < TILE_OVERLAY_LOADING_MIN_ZOOM) {
      return;
    }

    if (layerParams.length == 0) {
      return;
    }

    // If user has and no masks, load all raster
    const visibleMasks = masks.filter(mask => mask.isAttivaPerUtenteCorrente);
    if (visibleMasks.length == 0 && hasPermissions) {
      for (const layerParam of layerParams) {
        FunzioniComuniService.getCurrentlyVisibleTiles(gMap)
          .pipe(switchMap(tiles => this.handleVisualizationParameter(layerParam, tiles)))
          .subscribe(responses => {
            const mapType = this.getImageMapType(rasterId, opacity, responses);
            gMap.overlayMapTypes.push(mapType)
          });
      }
      return;
    }

    const validLayers = visibleMasks.map(mask => mask.LayerElementiGrafici_cod);
    const validFeatures = features.filter(f => validLayers.includes(+f.properties.layer));

    const polygons = validFeatures.map(f => this.featureInformationService.getGeometry(f.properties.id) as google.maps.Data.Polygon);
    if (polygons.length == 0) {
      return;
    }

    RasterOverlayService.clearRasterOverlays(rasterId, gMap);
    for (const layerParam of layerParams) {
      FunzioniComuniService.getCurrentlyVisibleTiles(gMap)
        .pipe(switchMap(tiles => this.handleVisualizationParameter(layerParam, tiles)))
        .subscribe(responses => {
          const mapType = new PolygonInterceptionMapType(rasterId, polygons, gMap, responses, opacity);
          gMap.overlayMapTypes.push(mapType)
        });
    }
  }

  public setOpacity(rasterId: number, opacity: number): void {
    const gMap = this.googleMapService.googleMapWrapper.data.getMap();
    for (const overlay of gMap.overlayMapTypes.getArray()) {
      if (overlay.name == SATELLITE_DATA_MAP_TYPE && overlay['id'] == rasterId) {
        (overlay as PolygonInterceptionMapType | google.maps.ImageMapType).setOpacity(opacity);
      }
    }
  }

  private handleVisualizationParameter(layerParam: RasterParameterVisualizationLayer, tiles: Tile[]): Observable<UrlFirmato[]> {
    if (layerParam.type == RasterParameterVisualizationType.SATELLITE || layerParam.type == RasterParameterVisualizationType.PUBLIC) {
      const result = tiles.map(tile => {
        const coods = `${tile.zoom}/${tile.x}/${tile.y}`;
        const url = layerParam.baseUrl.replace(/\/$/, ''); // remove final / if present
        return { Key: coods, SignedUrl: `${url}/${coods}` } as UrlFirmato;
      });
      return of(result);
    }

    if (layerParam.type == RasterParameterVisualizationType.ABACO_PUBLIC) {
      const result = tiles.map(tile => {
        const coods = `${tile.zoom}/${tile.x}/${tile.y}`;
        const url = layerParam.baseUrl.replace(/\/$/, ''); // remove final / if present
        const colorTableParam = this.getColorTableParam(layerParam);
        return { Key: coods, SignedUrl: `${url}/${coods}?${colorTableParam}` } as UrlFirmato;
      });
      return of(result);
    }

    if (layerParam.type == RasterParameterVisualizationType.PRIVATE) {
      const request = {
        elencoRichieste: tiles.map(tile => ({
          Bucket: layerParam.bucket,
          Object: layerParam.obj,
          Coords: `${tile.zoom}/${tile.x}/${tile.y}`
        }))
      } as RichiestaSignedUrl_In;

      return this.authDispatcherClient
        .authDispatcherRichiediSignedURL(request)
        .pipe(map(res => res.RispostaStringa.elencoUrlFirmati));
    }

    const authRequest = {
      elencoRichieste: tiles.map(tile => ({
        Bucket: layerParam.bucket,
        Object: layerParam.obj,
        Coords: `${tile.zoom}/${tile.x}/${tile.y}`
      })),
      RichiestaAbacoType: RichiestaAbacoType.Raster
    } as RichiestaAbacoUrl_In;

    return this.authDispatcherClient.authDispatcherRichiediAbacoURL(authRequest)
      .pipe(map(res => {
        const colorTable = this.getColorTableParam(layerParam);
        res.RispostaStringa.elencoUrlFirmati.forEach(x => x.SignedUrl = `${x.SignedUrl}&${colorTable}`);
        return res.RispostaStringa.elencoUrlFirmati;
      }));
  }

  private getImageMapType(rasterId: number, opacity: number, urls: UrlFirmato[]): google.maps.ImageMapType {
    const mapType = new google.maps.ImageMapType({
      name: SATELLITE_DATA_MAP_TYPE,
      getTileUrl: function (tile, z) {
        const getLegacyUrl = function (zoom: number, tileX: number, tileY: number): string {
          const url = (urls as UrlFirmato).SignedUrl;

          const ymax = 1 << zoom;
          const y = ymax - tileY - 1;
          const coords = `${zoom}/${tileX}/${y}`; // legacy computation
          return `${url}/${coords}.png`;
        };

        return Array.isArray(urls) ? urls.find(url => url.Key == `${z}/${tile.x}/${tile.y}`)?.SignedUrl : getLegacyUrl(z, tile.x, tile.y);
      },
      tileSize: new google.maps.Size(256, 256),
      maxZoom: TILE_OVERLAY_LOADING_MAX_ZOOM,
      minZoom: TILE_OVERLAY_LOADING_MIN_ZOOM,
      opacity: opacity
    });

    mapType['id'] = rasterId;

    return mapType;
  }

  public static clearRasterOverlays(rasterId: number, gmap: google.maps.Map): void {
    const overlays = gmap.overlayMapTypes;

    let i = 0;
    while (i < overlays.getLength()) {
      const overlay = overlays.getAt(i);
      if (overlay.name == SATELLITE_DATA_MAP_TYPE && overlay['id'] == rasterId) {
        overlays.removeAt(i);
      } else {
        i++;
      }
    }
  }

  private getColorTableParam(layerParam: RasterParameterVisualizationLayer): string {
    const obj = layerParam.obj.split('/')[0];
    const view = layerParam.views[0];
    return `colorTable=${layerParam.bucket}/${obj}/${layerParam.guid}/${view}.txt`;
  }
}