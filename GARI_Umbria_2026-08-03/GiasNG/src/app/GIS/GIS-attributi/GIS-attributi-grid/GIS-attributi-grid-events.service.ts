import { Injectable } from '@angular/core';
import { enum_FeatureGeometryType, enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { GisAttributiService, ReadTecniciParams } from 'app/GIS/services/gis-attributi.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { MappePrescrizioneService } from 'app/GIS/services/mappe-prescrizione.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { WKTService } from 'app/GIS/services/wkt.service';
import { DatiFeatureConAttributi } from 'app/Model/GIS/FeatureConAttributi';
import { FeatureType, GeoJson_Geometry_New } from 'app/Model/GIS/GisDataReadRval_New';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import {
  ChiaveAlbero,
  GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
  GeoJSONAgroGisProp,
  GisClient,
  GisDataReadRval_New_1OfGeoJSONAgroGisProp,
  PfPrescriptionUpdate_In,
  RispostaStandard_1OfSalvaEntitaConAttributi_Out,
  SalvaEntitaConAttributi_In
} from 'app/Service/api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { rispostaStandard } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { TreeGisFiltersService } from 'app/Utility/Template/kendo-tree/filters/gis-tree-filters.service';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { catchError, filter, of } from 'rxjs';
import { map, Observable, switchMap, tap } from 'rxjs';
import {GiasMarker, GiasPolyline} from '../../models/gias-drawings.model';
import { GeoJsonUtils } from 'app/GIS/utils/geo-json.utils';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';

@Injectable()
export class GISAttributiEventsService {

  private polygonValidator: any;

  constructor(
    private drawingService: DrawingService,
    private wktService: WKTService,
    private gisAttributiService: GisAttributiService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gisTreeFilterService: TreeGisFiltersService,
    private googleMapService: GoogleMapService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private layerService: LayerService,
    private featureService: FeatureService,
    private treeGisService: TreeGisService,
    private mappePrescrizioneService: MappePrescrizioneService,
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) { }

  public addItem(items: any): Observable<any> {
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Polygon) {
      return this.addPolygon(items);
    }
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Point) {
      return this.addMarker(items);
    }
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.LineString) {
      return this.addPolyline(items);
    }
  }

  private addPolygon(items: any): Observable<any> {
    let d = this.drawingService.polygons[0];

    const DrawingManager = require('../../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
    this.polygonValidator = new DrawingManager.PolygonValidator(d.polygon.getPath());

    if (this.polygonValidator.isValid) {
      // Custom save endpoint if layer is MappePrescrizione
      if (this.layerService.layerItemSelected[0].id == enum_LayerElementiGraficiStd.Mappe_Prescrizione) {
        return this.configurazioneSitiService
          .leggiChiave(EnumChiaviConfigurazioneSiti.MappePrescrizioneEngineIsActive)
          .pipe(
            map(config => config?.Valore?.toLowerCase() === 'true'),
            filter(isActive => !isActive), // read-only in raster engine: block if active
            switchMap(() => this.addItemToMappePrescrizione(items, d.polygon))
          );
      }

      let geoJson = GeoJsonUtils.polygonToGeoJson(d.polygon);
      let newEntity = this.prepareEntity(items, this.wktService.geoJsonToWKT(geoJson), d.tipologiaLayer.id);

      let datiFeature: DatiFeatureConAttributi = this.prepareDatiFeature(
        newEntity,
        this.wktService.polygonToDataGeometry(d.polygon),
        enum_FeatureGeometryType.Polygon,
        enum_TipoOperazioneDB.Scrittura
      )

      this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
      this.cancelPolygon();

      return this.gisAttributiService.gisSalvaEntitaConAttributi(newEntity);

    } else {
      DrawingManager.DisplayErrorPolyLines(this.polygonValidator.intersection, this.googleMapService.googleMapWrapper.googleMap);
      this.giasMessageService.errorMessage("gis.ImpossibileAggiungereElemento", false, true);
      return of(null);
    }
  }

  private addItemToMappePrescrizione(items: any, polygon: google.maps.Polygon): Observable<RispostaStandard_1OfSalvaEntitaConAttributi_Out> {
    const lastLoaded = this.mappePrescrizioneService.lastGeoJsonLoaded;
    if (lastLoaded?.geoJson == null) {
      return;
    }

    // Get the existing geoJSON
    const geoJson = { ...lastLoaded.geoJson.geoJsonCaricato };
    if (geoJson.features.length == 0) {
      return;
    }

    // Save the missing information needed for the new feature
    const type = geoJson.features[0].type;
    const properties = this.getMappePrescrizioneProperties(items, geoJson.features);

    // Filter features that were selected
    const selectedFeatures = this.featureService.getFeatureSelezionate();
    geoJson.features = geoJson.features.filter(feature => !selectedFeatures.map(selectedFeature => selectedFeature.properties.id).includes(feature.properties.id))

    // Add the new feature to the geoJSON
    const newPolygonGeoJson = GeoJsonUtils.polygonToGeoJson(polygon);
    const newPolygon = { coordinates: newPolygonGeoJson.coordinates, type: newPolygonGeoJson.type } as GeoJson_Geometry_New;
    geoJson.features.push({ type: type, geometry: newPolygon, properties: properties });

    const payload = {
      allegati_documenti_cod: lastLoaded.allegatoCode,
      jsonMap: JSON.stringify({ geoJsonCaricato: geoJson }),
      appIdRateRef: 'CAP_N+'
    } as PfPrescriptionUpdate_In;
    return this.gisClient.gisPfPrescriptionUpdate(payload)
      .pipe(
        switchMap(() => this.gisClient.gisCaricaDatiPrecisionXmlDaAllegati({ Flag_TipoCod: 1, Lista_RicettaOp_Cod: [lastLoaded.allegatoCode] })),
        catchError(() => {
          this.giasMessageService.errorMessage("gis.ErroreDuranteUnionePoligoni", false, true);
          return of({ RispostaStringa: { myGeoJson: lastLoaded?.geoJson } });
        }),
        tap(res => {
          const newGeoJson = res.RispostaStringa.myGeoJson as any;
          this.mappePrescrizioneService.setGeoJsonLoaded({ allegatoCode: lastLoaded.allegatoCode, geoJson: newGeoJson });
          this.googleMapGeoJsonService.addPrecisionDataFeatures(newGeoJson, false);
        }),
        map(() => ({ RispostaStringa: { ListaEntitaInserite: [+properties.Entita_Cod] } }))
      );
  }

  private getMappePrescrizioneProperties(items: any, features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): GeoJSONAgroGisProp {
    const key1 = 'CAP_N+';
    const key2 = 'Prod#_secc';

    const entry1 = items[key1];
    const userInput = `${key1}§ ${entry1}|${key2}§ ${items[key2]}|`;
    const entitaCod = Math.min(...features.map(f => +f.properties.Entita_Cod)) - 1;

    const properties = { ...features[0].properties };
    properties.id = properties.id.split('|')[0] + '|' + entitaCod;
    properties.Entita_Cod = entitaCod.toString();
    properties.Testo = userInput;
    properties.AppIdRate = userInput;

    return properties;
  }

  private addMarker(items: any): Observable<any> {
    let d: GiasMarker = this.drawingService.markers[0];

    let geoJson = GeoJsonUtils.markerToGeoJson(d.marker);
    let newEntity = this.prepareEntity(items, this.wktService.geoJsonToWKT(geoJson), d.tipologiaLayer.id);

    let datiFeature: DatiFeatureConAttributi = this.prepareDatiFeature(
      newEntity,
      this.wktService.markerToDataGeometry(d.marker),
      enum_FeatureGeometryType.Point,
      enum_TipoOperazioneDB.Scrittura
    )

    this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
    this.cancelMarker();

    return this.gisAttributiService.gisSalvaEntitaConAttributi(newEntity);
  }

  private addPolyline(items: any) {
    let d = this.drawingService.polylines[0];

    let geoJson = GeoJsonUtils.polylineToGeoJson(d.polyline);
    let newEntity = this.prepareEntity(items, this.wktService.geoJsonToWKT(geoJson), d.tipologiaLayer.id);

    let datiFeature: DatiFeatureConAttributi = this.prepareDatiFeature(
      newEntity,
      this.wktService.polylineToDataGeometry(d.polyline),
      enum_FeatureGeometryType.LineString,
      enum_TipoOperazioneDB.Scrittura
    );

    this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
    this.cancelPolyline();

    return this.gisAttributiService.gisSalvaEntitaConAttributi(newEntity);
  }

  private prepareEntity(items: any, cartografia: string, layer: string): SalvaEntitaConAttributi_In {
    let newEntity: SalvaEntitaConAttributi_In = {};

    newEntity.EntitaCod = 0;
    newEntity.Piva = this.objParametriAgendaService.getObjParamValue().Piva;
    newEntity.Cartografia = cartografia;
    newEntity.SaCod = this.getSaCod() ?? 0;
    newEntity.ElementoGraficoDes = this.getDataString(items);
    newEntity.LayerElementiGraficiCod = Number.parseInt(layer);
    newEntity.FlagGps = 0;

    if (this.isCampionamenti()) {
      const ca: ChiaveAlbero | null = this.getSelectedFeatureChiaveAlbero();
      if (ca != null) {
        newEntity.AnalisiCampioneCod = Number.parseInt(ca.Analisi_Campione_Cod);
      }
    }

    if (this.isFabbricati()) {
      const ca: ChiaveAlbero | null = this.getSelectedFeatureChiaveAlbero();
      if (ca != null) {
        newEntity.FabbricatoCod = Number.parseInt(ca.Fabbricato_Cod);
        newEntity.SaCod = ca.Sa_Cod;
      }
    }

    return newEntity;
  }

  private getSelectedFeatureChiaveAlbero() {
    const featureSelezionate = this.featureService.getFeatureSelezionate();
    const f = featureSelezionate[featureSelezionate.length - 1];

    let ca: ChiaveAlbero;
    if (f)
      ca = FunzioniComuniService.scomponiChiaveAlbero(this.featureService.getChiaveAlberoCompletaByFeature(f));
    else {
      const checkedKeys = this.sharedDataService.getTreeViewCheckedKeys();
      const node = this.treeGisService.getTreeNodeFromCheckedKey(checkedKeys[0]);

      if (!!node) {
        ca = FunzioniComuniService.scomponiChiaveAlbero(node?.id);
      }
    }
    return ca;
  }

  private prepareDatiFeature(
    newEntity: SalvaEntitaConAttributi_In,
    geometry: google.maps.Data.Geometry,
    FeatureGeometryType: enum_FeatureGeometryType,
    TipoOperazione: enum_TipoOperazioneDB
  ): DatiFeatureConAttributi {

    return {
      TipoOperazione: TipoOperazione,
      EntitaCod: newEntity.EntitaCod,
      Cartografia: newEntity.Cartografia,
      ElementoGraficoDes: newEntity.ElementoGraficoDes,
      LayerElementiGraficiCod: newEntity.LayerElementiGraficiCod,
      FlagGps: newEntity.FlagGps,
      FeatureGeometryType: FeatureGeometryType,
      GoogleMapsDataGeometry: geometry,
      DatiCompleti: false
    };
  }

  public updateItem(items: any): Observable<any> {
    let f = this.googleMapService.googleMapWrapper.data.getFeatureById(items['chiave']);
    let newEntity: SalvaEntitaConAttributi_In = this.prepareEntity(
      items,
      this.wktService.featureGeometryToWKT(f),
      f.getProperty(enum_FeatureProperty.layer) as string
    )
    newEntity.EntitaCod = Number.parseInt(f.getProperty(enum_FeatureProperty.entitaCod) as string);

    let datiFeature: DatiFeatureConAttributi;
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Polygon) {
      datiFeature = this.prepareDatiFeature(newEntity, f.getGeometry(), enum_FeatureGeometryType.Polygon, enum_TipoOperazioneDB.Modifica)
    }
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Point) {
      datiFeature = this.prepareDatiFeature(newEntity, f.getGeometry(), enum_FeatureGeometryType.Point, enum_TipoOperazioneDB.Modifica)
    }
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.LineString) {
      datiFeature = this.prepareDatiFeature(newEntity, f.getGeometry(), enum_FeatureGeometryType.LineString, enum_TipoOperazioneDB.Modifica)
    }
    if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Raster) {
      datiFeature = this.prepareDatiFeature(newEntity, f.getGeometry(), enum_FeatureGeometryType.Raster, enum_TipoOperazioneDB.Modifica)
    }
    datiFeature.FeatureModified = f;

    this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
    return this.gisAttributiService.gisSalvaEntitaConAttributi(newEntity);
  }

  private getSaCod(): number {
    let centri = this.gisTreeFilterService.getValue();
    let centroSelezionato = this.gisTreeFilterService.centroSelezionato.getValue();

    if (!centroSelezionato || centroSelezionato['id'] == '0') {
      return Number.parseInt(centri.find(c => c['id'] != '0')['id']);
    }

    return Number.parseInt(centroSelezionato.centro['id']);
  }

  public cancelPolygon(): void {
    let draw = this.drawingService.polygons[0];
    this.drawingService.removeEvent.next(draw);
  }

  public cancelMarker(): void {
    let draw: GiasMarker = this.drawingService.markers[0];
    this.drawingService.removeEvent.next(draw);
  }

  public cancelPolyline(): void {
    let draw: GiasPolyline = this.drawingService.polylines[0];
    this.drawingService.removeEvent.next(draw);
  }

  private getDataString(items: any): string {
    let data: string = '';

    for (let k in items) {
      if (k != 'chiave') {
        if (k == 'Descrizione') {
          data = items[k];
        } else {
          if (data.length > 0) data += '|';
          data += k + '§ ' + items[k].trim();
        }
      }
    }

    return data;
  }

  public focusOnFeature(chiave: string): void {
    this.googleMapGeoJsonService.selezionaFeatureById(chiave, true, false);
  }

  public selectFeature(chiave: string): void {
    this.googleMapGeoJsonService.selezionaFeatureById(chiave, true, false, false);
  }

  public isCampionamenti(): boolean {
    return this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.CAMPIONAMENTI;
  }

  public isFabbricati(): boolean {
    return this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.Fabbricati;
  }

  public isTecnici(): boolean {
    return this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.TECNICIINCAMPO;
  }

  public readTecnici(p: ReadTecniciParams): Observable<rispostaStandard<GisDataReadRval_New_1OfGeoJSONAgroGisProp>> {
    if (this.isTecnici()) {
      return this.gisAttributiService.leggiTecnici(p);
    }
  }
}
