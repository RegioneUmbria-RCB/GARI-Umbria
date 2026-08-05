import { Injectable } from '@angular/core';
import { Observable, of, forkJoin, map, take, switchMap } from 'rxjs';
import { AuthDispatcherClient, RichiestaAbacoType, RichiestaAbacoUrl_In, RichiestaSignedUrl_In, RispostaStandard_1OfElencoUrlFirmati, UrlFirmato, Gis_Sat_Sentinel_Overlay, MascheraLayerRaster, EntitaAlgoritmoCartografico, GisClient, RichiestaSatUrl_In, RichiestaSatUrl } from 'app/Service/api.service';
import { RasterParameterVisualizationLayer, RasterParameterVisualizationType } from '../services/raster-overlay.service';
import { Tile } from '../utils/mercator.utils';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';

export const SDAC_DEFAULT = 53;

@Injectable({ providedIn: 'root' })
export class SatelliteOverlayService {


  constructor(
    private authDispatcherClient: AuthDispatcherClient,
    private gisClient: GisClient,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {
  }

  handleVisualizationParameter(layerParams: RasterParameterVisualizationLayer[], tiles: Tile[], overlay: Gis_Sat_Sentinel_Overlay, sensor: string): Observable<[UrlFirmato[] | UrlFirmato, Gis_Sat_Sentinel_Overlay]> {
    return this.configurazioneSitiService
      .leggiChiave(EnumChiaviConfigurazioneSiti.SatEngineIsActive)
      .pipe(
        map(config => config?.Valore?.toLowerCase() === 'true'),
        take(1),
        switchMap(isSatActive => {
          const requests: Observable<RispostaStandard_1OfElencoUrlFirmati>[] = [];
          for (const layerParam of layerParams) {
            if (layerParam.type == RasterParameterVisualizationType.SATELLITE) {
              return SatelliteOverlayService.getSatelliteRequests(layerParam, overlay);
            }

            if (layerParam.type == RasterParameterVisualizationType.PUBLIC || layerParam.type == RasterParameterVisualizationType.ABACO_PUBLIC) {
              return SatelliteOverlayService.getRasterPublicRequests(tiles, layerParam, overlay, sensor);
            }

            if (layerParam.type == RasterParameterVisualizationType.PRIVATE) {
              requests.push(this.authDispatcherClient.authDispatcherRichiediSignedURL({
                elencoRichieste: tiles.map(tile => ({
                  Bucket: layerParam.bucket,
                  Object: layerParam.obj,
                  Coords: `${tile.zoom}/${tile.x}/${tile.y}`
                }))
              } as RichiestaSignedUrl_In));
            }

            if (layerParam.type == RasterParameterVisualizationType.ABACO_PRIVATE) {
              const param = SatelliteOverlayService.getColorTableParam(layerParam, sensor);
              if (isSatActive) {
                return forkJoin([this.authDispatcherClient.authDispatcherRichiediSatUrl({
                  elencoRichieste: tiles.map(tile => ({
                    Sensor: sensor,
                    Coords: `${tile.zoom}/${tile.x}/${tile.y}`,
                    Data: overlay.DataRiferimento
                  }) as RichiestaSatUrl)
                } as RichiestaSatUrl_In)
                  .pipe(map(res => {
                    res.RispostaStringa.elencoUrlFirmati.forEach(y => {
                      y.SignedUrl = `${y.SignedUrl}&${param}`;

                      // for SAT engine we need to use DataRiferimentoSAT as oat parameter to include the time since we can have multiple observations on the same day
                      // for non-SAT engine we can use the original DataRiferimento
                      // we add 1 second to the oat parameter to avoid potential issues when DataRiferimentoSAT is the exact time
                      if (overlay.DataRiferimentoSAT != null) {
                        const oatDate = new Date(new Date(overlay.DataRiferimentoSAT).getTime() + 1000);
                        y.SignedUrl = y.SignedUrl.replace(/([?&]oat=)[^&]*/, `$1${encodeURIComponent(oatDate.toISOString())}`);
                      }
                    });
                    return res;
                  }))]);
              };

              const request = this.authDispatcherClient.authDispatcherRichiediAbacoURL({
                RichiestaAbacoType: RichiestaAbacoType.Satellite,
                elencoRichieste: tiles.map(tile => ({
                  Bucket: layerParam.bucket,
                  Object: layerParam.obj,
                  Coords: `${tile.zoom}/${tile.x}/${tile.y}`
                }))
              } as RichiestaAbacoUrl_In)
                .pipe(map(res => {
                  res.RispostaStringa.elencoUrlFirmati.forEach(y => y.SignedUrl = `${y.SignedUrl}&${param}`);
                  return res;
                }));

              requests.push(request);
            }
          }

          return forkJoin(requests);

        }),
        map(res => [res.flatMap(r => r.RispostaStringa.elencoUrlFirmati).filter(SatelliteOverlayService.distinctUrls), overlay])
      );
  }

  static getSatelliteRequests(layerParam: RasterParameterVisualizationLayer, overlay: Gis_Sat_Sentinel_Overlay): Observable<[UrlFirmato, Gis_Sat_Sentinel_Overlay]> {
    const url = layerParam.baseUrl.replace(/\/$/, '');
    return of([{ Key: ``, SignedUrl: url } as UrlFirmato, overlay]);
  }

  static getRasterPublicRequests(tiles: Tile[], layerParam: RasterParameterVisualizationLayer, overlay: Gis_Sat_Sentinel_Overlay, sensor: string): Observable<[UrlFirmato[], Gis_Sat_Sentinel_Overlay]> {
    const result = tiles.map(tile => {
      const coods = `${tile.zoom}/${tile.x}/${tile.y}`;
      const url = layerParam.baseUrl.replace(/\/$/, '');
      if (layerParam.type == RasterParameterVisualizationType.PUBLIC) {
        return { Key: coods, SignedUrl: `${url}/${coods}` } as UrlFirmato;
      }

      const param = SatelliteOverlayService.getColorTableParam(layerParam, sensor);
      return { Key: coods, SignedUrl: `${url}/${coods}?${param}` } as UrlFirmato;
    });

    return of([result, overlay]);
  }

  static getColorTableParam(layerParam: RasterParameterVisualizationLayer, sensor: string): string {
    const obj = layerParam.obj.split('/')[0];
    return `colorTable=${layerParam.bucket}/${obj}/${sensor}.txt`;
  }

  static distinctUrls(value: UrlFirmato, index: number, array: UrlFirmato[]): boolean {
    return array.findIndex(x => x.SignedUrl == value.SignedUrl) === index;
  }

  static hasValidFeatures(isEngineActive: boolean, data: { overlay: Gis_Sat_Sentinel_Overlay, features: any[] }[]): boolean {
    if (isEngineActive) {
      return true;
    }

    for (const element of data) {
      const urls = element.overlay.Passaggi.map(p => p.url);
      for (const feature of element.features) {
        const code = feature.properties.Entita_GUID;
        if (code == null || code.trim() == '') {
          continue;
        }

        for (const url of urls) {
          if (url.includes(code)) {
            return true;
          }
        }
      }
    }

    return false;
  }

  static getEntitiesFromFeatures(codes: number[]): EntitaAlgoritmoCartografico[] {
    return codes.map(code => ({ entita_cod_1: code, entita_cod_2: 0, entita_cod_risultato: 0 } as EntitaAlgoritmoCartografico));
  }

  static applyOverlaysOnPolygons(permessiUtenteService: PermessiUtenteService, masks: MascheraLayerRaster[]): boolean {
    // If I have active masks (without checking any permissions), view overlays on polygons
    const hasMasks = masks.filter(x => x.isAttivaPerUtenteCorrente).length > 0;
    if (hasMasks) {
      return true;
    }

    // If I don't have permissions, view overlays on polygons (if no feature is selected, no overlay is loaded)
    const hasPermission = permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
    if (!hasPermission) {
      return true;
    }

    // No active mask, but I have permissions to load the whole overlay
    return false;
  }

  loadSatelliteAlgorithm(entityCode: number): Observable<number> {
    return this.gisClient.gisLeggiConfigurazioniProiezioneSuLayer({
      LayerElementiGrafici_Cod: 19,
      Entita_Cod: entityCode,
      TipologiaLayer_cod: 1,
    }).pipe(
      map(response => response?.RispostaStringa?.elencoConfigurazioni
        ?.find((config) => config.LayerAnalysisConfig_Algorithm_Cod == 1)
        ?.LayerAnalysisConfig_Cod ?? SDAC_DEFAULT)
    );
  }

  buildAttivazioneBody(codes: number[]): Observable<{ configurazioneProiezione_Cod: number, layer_cod: number, isAttivo: boolean, tipologia_layer_cod: number, listaEntita: EntitaAlgoritmoCartografico[] }> {
    return this.loadSatelliteAlgorithm(codes[0])
      .pipe(map(result => ({
        configurazioneProiezione_Cod: result,
        layer_cod: 0,
        isAttivo: true,
        tipologia_layer_cod: 1,
        listaEntita: SatelliteOverlayService.getEntitiesFromFeatures(codes)
      })));
  }
}
