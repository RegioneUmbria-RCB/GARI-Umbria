import { HttpContext } from "@angular/common/http";
import { Injectable } from "@angular/core";
import {
  FiltroTemporale,
  FiltroTemporale_enum_OperatoreFiltroTemporale,
  FiltroTemporale_enum_TipoFiltroTemporale,
  GisClient,
  GisDataReadParam,
  RispostaStandard_1OfElencoConfigurazioniProiezione,
  RispostaStandard_1OfString,
} from "app/Service/api.service";
import { Observable, take } from "rxjs";
import { GISModality } from "../GIS-enum/GIS-feature";
import { SharedDataService } from "./shared-data.service";
import { enum_TipologiaLayer } from "../GIS-enum/GIS-tipologia-layer";
import { enum_LayerElementiGraficiStd } from "../GIS-enum/GIS-layer-elementi-grafici";
import {
  enum_OrigineChiamataFilterService,
  enum_OrigineChiamataLoadGeoJson,
} from "../GIS-enum/GIS-origine-chiamata";
import { GisToolbarService } from "../GIS-toolbar/gis-toolbar.service";
import { WKTService } from "./wkt.service";
import { GoogleMapService } from "../google-map/google-map.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { GeoJsonFilterService } from "./geojson-filter.service";
import { GeoJsonFilterServiceParam } from "app/Model/GIS/GeoJsonFilterServiceParam";
import {
  Dialog_Type,
  GiasDialogService,
} from "app/Service/gias-dialog.service";
import { TranslocoService } from "@jsverse/transloco";
import { DialogCloseResult } from "@progress/kendo-angular-dialog";
import { DateUtils } from "app/Utility/date-utils";

interface WindowSize {
  width: number;
  height: number;
}

/**
 * This service is used to handle almost every aspect of the
 * initialization of the GIS shown in the widgets MultiAzienda.
 */
@Injectable()
export class MultiAziendaService {
  private _country: string;
  private _year: number;

  private _windowSize?: WindowSize;

  constructor(
    private sharedDataService: SharedDataService,
    private gisClient: GisClient,
    private gisToolbarService: GisToolbarService,
    private giasDialogService: GiasDialogService,
    private wktService: WKTService,
    private googleMapService: GoogleMapService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private geoJsonFilterService: GeoJsonFilterService,
    private translocoService: TranslocoService
  ) {
    this.country = "";
    this.year = 0;
  }

  /**
   * @param nation the country to locate the data in
   */
  public set country(nation: string) {
    this._country = nation;
  }

  /**
   * @param year the year to base the data in
   */
  public set year(year: number) {
    this._year = year;
  }

  /**
   * @returns the payload to send to the API containing the parameters
   */
  private get payload(): string {
    return JSON.stringify({CodicePaeseISO3166: this._country, Anno: this._year.toString()});
  }

  /**
   * @param size the size of the window containing the GIS
   */
  public set windowSize(size: WindowSize) {
    this._windowSize = size;
  }

  /**
   * @returns the width and the left margin of the theme bar if used in the widget,
   *          or undefined if the window size is not set
   */
  public get themeBarWidgetStyle(): { width: number; side: number } | undefined {
    return {
      width: this._windowSize?.width * 0.1,
      side: this._windowSize?.width * 0.8,
    };
  }

  /**
   * Starts the initialization of the GIS based on the modality,
   * calling every endpoint needed to load the data.
   * @param modality the modality of the GIS to initialize
   */
  public initMapMultiAzienda(modality: GISModality): void {
    this.sharedDataService.setTipoLayerSelezionato(enum_TipologiaLayer.Entita);
    // First loads the layer code used for each modality
    this.loadLayerCod().subscribe((risposta: any) => {
      if (risposta.RispostaStringa.elencoConfigurazioniProiezione.length > 0) {
        let layerCod =
          risposta.RispostaStringa.elencoConfigurazioniProiezione[0].Layer2
            .LayerElementiGrafici_Cod;
        switch (modality) {
          case GISModality.PaeseDistribuzioneMultiAzienda:
            this.initMapDistribuzione(layerCod);
            break;
          case GISModality.PaesePoligoniMultiAzienda:
            // Loads the WKT of the layer code to center the map on it
            this.loadWKT(layerCod).subscribe((risposta: any) => {
              let wkt = risposta.RispostaStringa;
              let year = this._year.toString();
              this.initMapPoligoni(wkt, year);
            });
            break;
        }
      } else {
        this.openWrongParamsWindow();
      }
    });
  }

  /**
   * Used to set up the call to the endpoint to load the layer code
   * @returns an observable containing the response from the API
   */
  private loadLayerCod(): Observable<RispostaStandard_1OfElencoConfigurazioniProiezione> {
    let obs: (
      body?: string | null | undefined,
      httpContext?: HttpContext
    ) => Observable<RispostaStandard_1OfElencoConfigurazioniProiezione>;
    obs = this.gisClient.gisLeggiConfigurazioniProiezioneFiltrati.bind(
      this.gisClient
    );
    return obs(this.payload);
  }

  /**
   * Used to set up the call to the endpoint to load the WKT from a layer code
   * @param layerCod the layer code to load the WKT of
   * @returns an observable containing the response from the API
   */
  private loadWKT(layerCod: number): Observable<RispostaStandard_1OfString> {
    let obs: (
      body?: number | undefined,
      httpContext?: HttpContext
    ) => Observable<RispostaStandard_1OfString>;
    obs = this.gisClient.gisGetEnvelopeWKT.bind(this.gisClient);
    return obs(layerCod);
  }

  /**
   * Finishes the initialization of the "Ditribuzione" widget
   * @param layerCod the layer code to use for the initialization
   */
  private initMapDistribuzione(layerCod: enum_LayerElementiGraficiStd): void {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(
      enum_OrigineChiamataLoadGeoJson.WidgetMultiAzienda
    );
    this.setGeoJsonFilterServiceDistribuzione(layerCod);
    this.sharedDataService.geoJsonLoaded.pipe(take(1)).subscribe(() => {
      let size = this.themeBarWidgetStyle;
      this.gisToolbarService.themeBtnToggle(true, size.width, size.side);
    });
  }

  /**
   * Finishes the initialization of the "Poligoni" widget
   * @param wkt the wkt to load the features from and center the map
   * @param year the year to filter the features
   */
  private initMapPoligoni(wkt: string, year: string): void {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(
      enum_OrigineChiamataLoadGeoJson.WidgetMultiAzienda
    );
    this.setGeoJsonFilterServicePoligoni(wkt, year);
    this.sharedDataService.geoJsonLoaded.pipe(take(1)).subscribe(() => {
      this.wktService.centerGMapOnWKT(
        wkt,
        this.googleMapService.googleMapWrapper.googleMap
      );
    });
  }

  /**
   * Sets up the default values to filter the features
   * to load for the "Distribuzione" widget
   * @param layerCod the layer code to filter
   */
  private setGeoJsonFilterServiceDistribuzione(
    layerCod: enum_LayerElementiGraficiStd
  ): void {
    let readParams = {} as GisDataReadParam;
    let objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    readParams.piva = objParametriAgenda.Piva;
    readParams.sa_cod = "0";
    readParams.TipologiaLayerSelezionata =
      this.sharedDataService.getTipoLayerSelezionato();

    const filtroTemporaleAvanzato = this.sharedDataService.getFiltroTemporaleAvanzato();
    readParams.filtroTemporale = filtroTemporaleAvanzato.filtroTemporalePeriodo;
    readParams.filtroTemporaleSingolaData = filtroTemporaleAvanzato.filtroTemporaleSingolaData;
    // every value is default except for this one
    readParams.layerElementiGrafici_cod.push(parseInt(layerCod));
    this.geoJsonFilterService.setGisDataReadParam(readParams);

    let filterServiceParam = new GeoJsonFilterServiceParam(
      enum_OrigineChiamataFilterService.WidgetMultiAzienda,
      true,
      true
    );
    // This also triggers the loading of the GeoJson
    this.geoJsonFilterService.setGeoJsonFilterServiceParam(filterServiceParam);
  }

  /**
   * Sets up the default values to filter the features
   * to load for the "Poligoni" widget
   * @param wkt the wkt to filter
   * @param year the year to filter
   */
  private setGeoJsonFilterServicePoligoni(wkt: string, year: string): void {
    let readParams = {} as GisDataReadParam;
    readParams.piva = "";
    readParams.sa_cod = "0";
    readParams.campo_cod = "0";
    readParams.TipologiaLayerSelezionata =
      this.sharedDataService.getTipoLayerSelezionato();
    // Sets up the WKT to filter
    readParams.wktBoundaySTIntersects = wkt;
    if (wkt != null && wkt != '') {
      readParams.zoomLevel = this.googleMapService.googleMapWrapper.getZoom();
    }

    // Searches for data in the given year
    readParams.filtroTemporale = {
      TipoFiltroTemporale:  FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      DataInizio: new Date(year + "-01-01T00:00:00"),
      DataFine: new Date(year + "-12-31T00:00:00"),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale
    } as FiltroTemporale;

    const today = new Date();
    today.setHours(0, 0, 0, 0);
    readParams.filtroTemporaleSingolaData = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(today),
      DataFine: DateUtils.calcolaDataFineSingolaData(today),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    readParams.layerElementiGrafici_cod.push(parseInt(enum_LayerElementiGraficiStd.IMPIANTI));
    this.geoJsonFilterService.setGisDataReadParam(readParams);

    let filterServiceParam = new GeoJsonFilterServiceParam(
      enum_OrigineChiamataFilterService.WidgetMultiAzienda,
      true,
      true
    );
    // This also triggers the loading of the GeoJson
    this.geoJsonFilterService.setGeoJsonFilterServiceParam(filterServiceParam);
  }

  // Opens a window to inform the user that the parameters are wrong
  private openWrongParamsWindow(): void {
    this.giasDialogService.dialogMessageRef(
      "",
      this.translocoService.translate("ParametriWidgetMancanti"),
      [{ text: this.translocoService.translate("Ok"), returnObj: true }],
      this._windowSize?.width != null ? this._windowSize.width * 0.5 : 300,
      this._windowSize?.height != null ? this._windowSize?.height * 0.7 : 250,
      (p) => p instanceof DialogCloseResult,
      Dialog_Type.info
    );
  }
}
