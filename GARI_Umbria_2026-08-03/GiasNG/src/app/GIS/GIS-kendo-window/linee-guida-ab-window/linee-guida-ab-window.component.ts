import { Component } from '@angular/core';
import { enum_FeatureGeometryType, enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { enum_zIndex } from 'app/GIS/GIS-enum/GIS-zIndex';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { WKTService } from 'app/GIS/services/wkt.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { Chiave_SalvaGrafica_Out, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GeoJSONAgroGisProp, GisClient, RispostaStandard_1OfSalvaNuovoAB_Out, SalvaNuovoAB_In } from 'app/Service/api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';

@Component({
  standalone: false,
  selector: 'app-linee-guida-ab-window',
  templateUrl: './linee-guida-ab-window.component.html',
  styleUrls: ['./linee-guida-ab-window.component.css']
})
export class LineeGuidaABWindowComponent {

  windowArgs: WindowArgs;
  feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;
  title: string | null = null;

  private firstMarker: google.maps.Marker;
  private secondMarker: google.maps.Marker;
  private polyline: google.maps.Polyline;

  constructor(
    private kendoWindowService: KendoWindowsService,
    private googleMapService: GoogleMapService,
    private featureService: FeatureService,
    private funzioniComuniService: FunzioniComuniService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private wktService: WKTService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private featureInformationService: FeatureInformationService
  ) {
    this.kendoWindowService
      .windowToggle$
      .subscribe(([windowTypes, args]) => {
        if (windowTypes === WindowTypes.LineeGuidaABWindow) {
          this.windowArgs = args;

          this.stop();
          if (this.windowArgs.openState) {
            this.start();
          }
        }
      })
  }

  save(): void {
    const chiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(FunzioniComuniService.chiaveAlberoRidottaToBig(this.feature.properties.chiavealbero));

    const body = {
      ChiaveAlbero: {
        TipoNodo: 9,
        Piva: chiaveAlbero.Piva,
        Sa_Cod: chiaveAlbero.Sa_Cod,
        Campo_Cod: 0,
        Appezza: chiaveAlbero.Appezza,
        Id_Imp: chiaveAlbero.Id_Imp,
        p_Part_Cod: "0",
        p_Provincia_Cod: "0",
        p_Comune_Cod: "0",
        p_Sezione: "0",
        p_Foglio: "0",
        p_Numero: "0",
        p_Subalterno: "0",
        Cod_Fiscale: "0",
        Fabbricato_Cod: "0",
        Prodotto_Cod: "0",
        Data_Lavorazione: "0",
        Analisi_Certificato_Cod: "0",
        Analisi_Testata_Cod: "0",
        Analisi_Dettaglio_Cod: "0",
        Analisi_Campione_Cod: "0",
        PianoConcimazioneTestata_Cod: "0",
        Progetto_Cod: "0",
        Programmazione_Cod: "0",
        Programmazione_Entita_Cod: "0",
        Id_Agenda: "0",
        PivaPadre: "",
        Ricetta_Cod: "0",
        RicettaOperazione_Cod: "0"
      },
      HiddenPunti_A: this.firstMarker.getPosition().toString(),
      HiddenPunti_B: this.secondMarker.getPosition().toString(),
      HiddenPunti_AB: ""
    } as SalvaNuovoAB_In;

    this.gisClient
      .gisSalvaNuovoAB(body)
      .subscribe({
        next: res => {
          this.giasDialogService.baseSuccess("gis.StrumentoLineeGuidaAB", "gis.StrumentoLineeGuidaABAggiuntoCorrettamente");
          this.addFeaturesToMap(res);
        },
        error: () => this.giasDialogService.baseError("gis.StrumentoLineeGuidaAB", "gis.StrumentoLineeGuidaABErrore"),
        complete: () => this.cancel()
      });
  }

  cancel(): void {
    this.stop();
    this.kendoWindowService.close(WindowTypes.LineeGuidaABWindow);
  }

  private start(): void {
    this.feature = this.featureService.getUltimaFeatureSelezionata();
    this.title = this.feature.properties.etichetta;

    const markerOptions = this.createMarkerOptions(this.feature);
    const polylineOptions = this.createPolylineOptions(markerOptions);

    this.firstMarker = new google.maps.Marker({ ...markerOptions, label: "A" });
    this.firstMarker.addListener('drag', () => this.handleDragEvent());

    this.secondMarker = new google.maps.Marker({ ...markerOptions, label: "B" });
    this.secondMarker.addListener('drag', () => this.handleDragEvent());

    this.polyline = new google.maps.Polyline(polylineOptions);
  }

  private createMarkerOptions(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.MarkerOptions {
    const markerOptions: google.maps.MarkerOptions = {};

    markerOptions.clickable = true;
    markerOptions.draggable = true;
    markerOptions.visible = true;
    markerOptions.position = this.getCenter(feature) ?? this.googleMapService.googleMapWrapper.getCenter();
    markerOptions.map = this.googleMapService.googleMapWrapper.googleMap;
    markerOptions.icon = {
      url: 'https://maps.google.com/mapfiles/kml/pushpin/red-pushpin.png',
      anchor: new google.maps.Point(11, 35),
      scaledSize: new google.maps.Size(36, 36),
    };

    return markerOptions;
  }

  private getCenter(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.LatLng | null {
    let minX = null, minY = null, maxX = null, maxY = null;

    this.featureInformationService.getPath(feature.properties.id).forEach(position => {
      minX = Math.min(minX ?? Number.MAX_VALUE, position.lat());
      minY = Math.min(minY ?? Number.MAX_VALUE, position.lng());

      maxX = Math.max(maxX ?? Number.MIN_VALUE, position.lat());
      maxY = Math.max(maxY ?? Number.MIN_VALUE, position.lng());
    });

    if (minX == null || minY == null || maxX == null || maxY == null) {
      return null;
    }

    return { lat: minX + ((maxX - minX) / 2), lng: minY + ((maxY - minY) / 2) } as google.maps.LatLng;
  }

  private createPolylineOptions(markerOptions: google.maps.MarkerOptions): google.maps.PolylineOptions {
    const polylineOptions: google.maps.PolylineOptions = {};

    polylineOptions.map = this.googleMapService.googleMapWrapper.googleMap;
    polylineOptions.clickable = false;
    polylineOptions.draggable = false;
    polylineOptions.editable = false;
    polylineOptions.strokeColor = 'red';
    polylineOptions.path = [markerOptions.position];
    polylineOptions.zIndex = enum_zIndex.strumentoMisurazione;

    return polylineOptions;
  }

  private handleDragEvent(): void {
    this.polyline.setPath([this.firstMarker.getPosition(), this.secondMarker.getPosition()]);
  }

  private stop(): void {
    this.feature = null;

    if (this.firstMarker != null) {
      this.firstMarker.setMap(null);
      this.firstMarker = null;
    }

    if (this.secondMarker != null) {
      this.secondMarker.setMap(null);
      this.secondMarker = null;
    }

    if (this.polyline != null) {
      this.polyline.setMap(null);
      this.polyline = null;
    }
  }

  private addFeaturesToMap(data: RispostaStandard_1OfSalvaNuovoAB_Out): void {
    const pointA = this.wktService.markerToDataGeometry(this.firstMarker);
    this.addFeatureToMap(pointA, data.RispostaStringa.hiddenPunti_A.Lista_ElementiGrafici[0]);

    const pointB = this.wktService.markerToDataGeometry(this.secondMarker);
    this.addFeatureToMap(pointB, data.RispostaStringa.hiddenPunti_B.Lista_ElementiGrafici[0]);

    const line = this.wktService.polylineToDataGeometry(this.polyline);
    const id = this.addFeatureToMap(line, data.RispostaStringa.hiddenPunti_AB.Lista_ElementiGrafici[0]);

    this.googleMapGeoJsonService.selezionaFeatureById(id, true, false, false);
  }

  private addFeatureToMap(geometry: google.maps.Data.Geometry, key: Chiave_SalvaGrafica_Out): string {
    const tipoLayer = this.sharedDataService.getTipoLayerSelezionato();
    const idFeature = this.funzioniComuniService.getIdFeature(key.Piva, key.Entita_Cod, tipoLayer, enum_LayerElementiGraficiStd.Precision);

    const properties = {
      layer: enum_LayerElementiGraficiStd.Precision,
      StandardEntita_layerDiAppartenenza: enum_LayerElementiGraficiStd.Precision,
      StandardEntita_layerDiAppartenenza_Icona32: '',
      id: idFeature,
      tipoicona: '',
      Entita_Cod: key.Entita_Cod,
      veg_cod: '-1',
      etichetta: '',
      inserimento: "True",
      modifica: "True",
      cancellazione: "True",
      informazioni: "True",
      chiavealbero: '',
      Testo: '',
      TipologiaGML: geometry.getType(),
      InOsservazione: 0,
      Colore_Primario: '',
      Colore_Retinatura: '',
      Trasparenza: 0,
      zindex: '1',
      AppIdRate: '',
      Area_Cod: 0,
      flag_gps: 'False',
      ParametriVisualizzazioneLayer: '',
      StandardEntita_layerDiAppartenenza_Des: ''
    } as GeoJSONAgroGisProp;

    const featureOptions = {
      geometry: geometry,
      id: idFeature,
      properties: properties
    } as google.maps.Data.FeatureOptions;

    this.googleMapGeoJsonService.addFeatureFromFeatureOptions(featureOptions);
    return idFeature;
  }
}
