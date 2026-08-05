import { Injectable, Optional } from '@angular/core';
import { BehaviorSubject, Observable, Subject, takeUntil } from 'rxjs';
import { MeasureDistanceService } from '../services/measure-distance.service';
import { TranslocoService } from '@jsverse/transloco';
import {GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString} from 'app/Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, enum_PagineAnalisi_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { FeatureService } from '../services/feature.service';
import { SharedDataService } from '../services/shared-data.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { createMask } from '@ngneat/input-mask';
import { PositionService } from '../services/position.service';
import { enum_FormatoCoordinate } from '../GIS-enum/GIS-coordinate';
import { enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { getServiceIdAndLog } from 'app/Service/utils';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_PagineGiasNG, enum_Security_Attivita, enum_Security_Operazione } from 'app/Model/TipiEnumerativi';
import { GiasDialogService } from '../../Service/gias-dialog.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { GISGestionePianoRateoService } from '../GIS-kendo-window/GIS-gestione-piano-rateo/GIS-gestione-piano-rateo.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { Utente_Impostazioni } from 'app/Model/utente/utente_impostazioni';
import { DropDownButtonComponent } from '@progress/kendo-angular-buttons';
import {GeoJson_Feature_New_1OfGeoJSONAgroGisProp} from 'app/Service/api.service';
import {FeatureInformationService} from '../services/feature-information.service';
import {ExportCartographyDataService} from './services/export-cartography-data.service';

export const DEFAULT_TOP_POSITION = 165;

@Injectable()
export class GisToolbarService {

  private _configurazioneBtnClick$ = new Subject<void>();
  private _calendarioBtnClick$ = new Subject<void>();

  private readonly classToolbarBtnAttivo = 'gis-toolbar-btn-attivo';

  public strumentoGpsAttivo: boolean = false;

  public strumentoMisurazioneAttivo: boolean = false;

  public permessoSetupVisualizzazione: boolean = false;

  public latValueSubject: BehaviorSubject<string> = new BehaviorSubject('');
  public lngValueSubject: BehaviorSubject<string> = new BehaviorSubject('');
  public latLngSgnSubject: BehaviorSubject<any> = new BehaviorSubject({});
  private markerPlacerActive = new BehaviorSubject<boolean>(false);
  private toolsDropdown: DropDownButtonComponent | null = null;

  public get configurazioneBtnClick$(): Observable<void> {
    return this._configurazioneBtnClick$.asObservable();
  }

  public get calendarioBtnClick$(): Observable<void> {
    return this._calendarioBtnClick$.asObservable();
  }

  featureSelezionata: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;

  public toolsList: Array<any> = [];

  maskIdx = enum_FormatoCoordinate.GradiDecimali;
  latPlaceholder = ['±;GG.GGGGGGG°', '±;GG°MM\'SS"'];
  lngPlaceholder = ['±;GGG.GGGGGGG°', '±;GGG°MM\'SS"'];
  latlngSpecialCharacters = ['°', '\'', '"', '.'];
  showLatMaskTyped = false;
  showLngMaskTyped = false;
  latValue = '';
  lngValue = '';
  latSgn = '';
  lngSgn = '';
  latMask = [createMask('[-]99.9999999°'), createMask('[-]99°99\'99"')];
  lngMask = [createMask('[-]999.9999999°'), createMask('[-]999°99\'99"')];

  searchAddressTxtBox: string;
  windowsPositionApplied = false;

  private signal = new Subject<void>();

  private serviceId = null;

  private readonly DEFAULT_FOCUS_ZOOM_LEVEL: number = 17;

  constructor(
    private measureDistanceService: MeasureDistanceService,
    private transloco: TranslocoService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private giasIFrameWindowService: GiasIFrameWindowService,
    private featureService: FeatureService,
    private giasDialogService: GiasDialogService,
    private sharedDataService: SharedDataService,
    private kendoWindowService: KendoWindowsService,
    private positionService: PositionService,
    private permessiUtenteService: PermessiUtenteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private featureInformationService: FeatureInformationService,
    private exportCatographyDataService: ExportCartographyDataService,
    @Optional() private gisGestionePianoRateoService: GISGestionePianoRateoService
  ) {

    this.serviceId = getServiceIdAndLog('GisToolbarService', 'constructor');

    this.featureService.getFeatureSelezionate$()
      .pipe(takeUntil(this.signal)).subscribe(f => {
      if (f.length > 0) {
        this.featureSelezionata = f[0];
      }
    });

    this.toolsList = this.getToolsList();

    this.getPermessoSetupVisualizzazione();

  }

  private getPermessoSetupVisualizzazione() {
    const permessoScrittura = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_SetupVisualizzazione, 2);
    this.permessoSetupVisualizzazione = permessoScrittura;
  }

  public configurazioneBtnClick(): void {
    this._configurazioneBtnClick$.next();
  }

  public calendarioBtnClick(): void {
    this._calendarioBtnClick$.next();
  }

  public setLatLng(lat: number, lng: number): void {
    let latMask: string = `${lat >= 0 ? '+' : '-'}`;
    let lngMask: string = `${lng >= 0 ? '+' : '-'}`;
    let latAbs: number = Math.abs(lat);
    let lngAbs: number = Math.abs(lng);

    latMask += `${latAbs >= 10 ? latAbs.toFixed(7).toString() : '0' + latAbs.toFixed(7).toString()}`;
    lngMask += `${lngAbs >= 100 ? lngAbs.toFixed(7).toString() : (lngAbs >= 10 ? '0' + lngAbs.toFixed(7).toString() : '00' + lngAbs.toFixed(7).toString())}`;

    if (this.maskIdx === enum_FormatoCoordinate.GradiMinutiSecondi) {
      let latLngGms = this.conversioneInGradiMinutiSecondi(latMask, lngMask);
      this.latValue = latLngGms.lat;
      this.lngValue = latLngGms.lng;
    } else {
      this.latValue = latMask;
      this.lngValue = lngMask;
    }
    this.latValueSubject.next(this.latValue);
    this.lngValueSubject.next(this.lngValue);
  }

  public getLatLngGradiDecimali(): google.maps.LatLng {

    let latLng: google.maps.LatLng = null;
    let lat: number = null;
    let lng: number = null;

    try {
      if (this.maskIdx === enum_FormatoCoordinate.GradiMinutiSecondi) {
        let latGms = this.latValueSubject.getValue();
        let lngGms = this.lngValueSubject.getValue();
        let latLngDec = this.conversioneInGradiDecimali(latGms, lngGms);
        lat = parseFloat(latLngDec.lat);
        lng = parseFloat(latLngDec.lng);
      } else {
        lat = parseFloat(this.latValueSubject.getValue());
        lng = parseFloat(this.lngValueSubject.getValue());
      }
    }
    catch (e) {
    }

    if (lat && lng) {
      latLng = new google.maps.LatLng(lat, lng);
    }

    return latLng;
  }

  public getClasseGpsAttivo(): string {
    let classe: string = "";
    if (this.strumentoGpsAttivo) {
      classe = this.classToolbarBtnAttivo;
    }
    return classe;
  }

  public getClasseMisurazioneAttivo(): string {
    let classe: string = "";
    if (this.strumentoMisurazioneAttivo) {
      classe = this.classToolbarBtnAttivo;
    }
    return classe;
  }

  public startStopMeasure(): void {
    this.strumentoMisurazioneAttivo = !this.strumentoMisurazioneAttivo;
    this.measureDistanceService.startStopMeasure();
  }

  public setCurrentToolsDropdownInstance(toolsDropdown: DropDownButtonComponent): void {
    this.toolsDropdown = toolsDropdown;
  }

  getToolsList(): any[] {
    let toolsList: Array<{ description: string, icon: string, click: () => void }> = [
      {
        description: this.transloco.translate('StampaImmagine'),
        icon: 'xi-tools-print-image',
        click: () => this.printGis()
      },
      // {
      //   description: this.transloco.translate('Importazione'),
      //   icon: 'xi-tools-import',
      //   click: () => this.importWindowToggle()
      // },
      // {
      //     description: this.transloco.translate('gis.LineaGuida'),
      //     icon: 'xi-tools-insert-guideline'
      // },
      // {
      //     description: this.transloco.translate('gis.GestioneFasciaSelezione'),
      //     icon: 'xi-tools-management-repect-band'
      // },
      // {
      //     description: this.transloco.translate('gis.ImpostaImpresaDaSelezione'),
      //     icon: 'xi-tools-companytax'
      // }
    ];

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Precision_Farming, 2))
      toolsList.push({
        description: this.transloco.translate('gis.GestionePianoRateo'),
        icon: 'xi-tools-plan-management',
        click: () => this.openGestionePianoRateo()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_Esporta_Dati, 2))
      toolsList.push({
        description: this.transloco.translate('Esportazione'),
        icon: 'xi-tools-export',
        click: () => this.exportWindowToggle()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Configurazione_Algoritmi_Cartografici, 2))
      toolsList.push({
        description: this.transloco.translate('gis.ConfigurazioneAlgoritmi'),
        icon: 'xi-tools-configuration',
        click: () => this.algorithmConfiguration()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Configurazione_Algoritmi_Clustering, 2))
      toolsList.push({
        description: this.transloco.translate('gis.ConfigurazioneAlgoritmiClustering'),
        icon: 'xi-tools-configuration',
        click: () => this.clusteringAlgorithmConfiguration()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_Catasto, 2))
      toolsList.push({
        description: this.transloco.translate('StrumentoRipartoApri'),
        icon: 'xi-tools-open-land-registry',
        click: () => this.RipartoCatastoWindowToggle()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu, 2))
      toolsList.push({
        description: this.transloco.translate('gis.ApriAnalisi'),
        icon: 'xi-tools-open-analyses',
        click: () => this.openAnalisi()
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Analisi_Dati_Meteo, 2))
      toolsList.push({
        description: this.transloco.translate('gis.GestioneAnalisiDatiMeteoDaSelezione'),
        icon: 'xi-plus',
        click: () => this.gestioneRedirectToAgenda_2010(enum_PagineAgenda_2010.Pagina_Analisi_Meteo)
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Analisi_Modelli_Previsionali, 2))
      toolsList.push({
        description: this.transloco.translate('gis.GestioneAnalisiDSSDaSelezione'),
        icon: 'iconaMacro',
        click: () => this.gestioneRedirectToAgenda_2010(enum_PagineAgenda_2010.Pagina_DSS_Difesa)
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Analisi_Curve_Maturazione, 2))
      toolsList.push({
        description: this.transloco.translate('gis.AnalisiDatiSchedeRilievi'),
        icon: 'xi-plus',
        click: () => this.gestioneRedirectToAgenda_2010(enum_PagineAgenda_2010.Pagina_Analisi_Rilievi)
      });

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.ReteAcqua_AnalisiDati, 2))
      toolsList.push({
        description: this.transloco.translate('gis.AnalisiDatiReteAcqua'),
        icon: 'xi-plus',
        click: () => this.gestioneRedirectToAgenda_2010(enum_PagineAgenda_2010.Pagina_DatiReteAcqua)
      });

    return toolsList;
  }

  public importWindowToggle(): void {
    this.gestioneRichiesteService
      .gestionePassaggioAltroSito(
        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
        enum_PagineAgenda_2010.Pagina_Gis_ImportazioneDati,
      )
      .then(val => {
        this.giasIFrameWindowService.open({
          title: this.transloco.translate('gis.StrumentoImportazioneDati'),
          content: val,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        });
      });
  }

  public exportWindowToggle(): void {
    const selectedCentreTreeKeys: string[] = this.sharedDataService.getTreeViewCheckedKeys().filter((k: string) => k.split('_').length === 3);

    if (selectedCentreTreeKeys.length > 1) {
      this.giasDialogService.baseError('', 'SelezionareUnSoloCentroAziendale');
    } else if (selectedCentreTreeKeys.length < 1) {
      this.giasDialogService.baseError('', 'SelezionareUnCentroAziendale');
    } else {
      const saCod: number = this.exportCatographyDataService.saCod;
      let objP: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
      objP.Sa_Cod = saCod;

      const queryStringParams: ParametriAggiuntivi_QueryString[] = [
        KeyValuePair.Create('piva', objP.Piva),
        KeyValuePair.Create('sa_cod', saCod.toString())
      ];

      this.gestioneRichiesteService
        .gestionePassaggioAltroSito(
          Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
          enum_PagineAgenda_2010.Pagina_Gis_EsportazioneDati,
          queryStringParams,
          objP
        )
        .then(val => {
          this.giasIFrameWindowService.open({
            title: this.transloco.translate("gis.StrumentoEsportazioneDati"),
            content: val,
            height: window.innerHeight * 0.9,
            width: window.innerWidth * 0.9
          });
        });
    }
  }

  public algorithmConfiguration(): void {
    const current = this.kendoWindowService.getWindowArgs(WindowTypes.AlgorithmConfigurationWindow);
    const windowArgs = current ?? new WindowArgs(WindowTypes.AlgorithmConfigurationWindow, false, this.transloco.translate('gis.ConfigurazioneAlgoritmi'), null, 500, 800, undefined, DEFAULT_TOP_POSITION + 100, true, true, true, false, true);
    this.kendoWindowService.open(WindowTypes.AlgorithmConfigurationWindow, windowArgs, false);
  }

  public clusteringAlgorithmConfiguration(): void {
    const current = this.kendoWindowService.getWindowArgs(WindowTypes.ClusteringAlgorithmConfigurationWindow);
    const windowArgs = current ?? new WindowArgs(WindowTypes.ClusteringAlgorithmConfigurationWindow, false, this.transloco.translate('gis.ConfigurazioneAlgoritmiClustering'), null, 500, 800, undefined, DEFAULT_TOP_POSITION + 100, true, true, true, false, true);
    this.kendoWindowService.open(WindowTypes.ClusteringAlgorithmConfigurationWindow, windowArgs, false);
  }

  public RipartoCatastoWindowToggle() {
    if (this.featureSelezionata == null) {
      // this.giasMessageService.errorMessage(this.transloco.translate("gis.SelezionareFeature"));
      this.giasDialogService.baseError('Riparto catasto', 'gis.SelezionareFeature');
      return;
    }

    const entita_cod = this.featureSelezionata.properties.Entita_Cod;
    this.gestioneRichiesteService
      .gestionePassaggioAltroSito(
        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
        enum_PagineAgenda_2010.Pagina_Gis_StrumentoDiRipartoCatasto,
        [{ key: 'entita', value: entita_cod, codifica: false }]
      )
      .then(val => {
        this.giasIFrameWindowService.open({
          title: this.transloco.translate("gis.StrumentoRipartoCatasto"),
          content: val,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        });
        this.giasIFrameWindowService.window.window.onDestroy(() => {
          this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.StrumentoRipartoCatasto);
          this.sharedDataService.setRicaricaFeature(true);
          this.sharedDataService.setRicaricaAlbero(true);
        });
      });
  }

  public openAnalisi(): void {
    let analisiTestataCod: string = this.featureService.getObjChiaveAlberoByFeature(this.featureService.getFeatureSelezionate()[0]).Analisi_Testata_Cod;

    const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgendaService.getObjParamValue()));

    const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

    //DCA20241114 qui distinguere il valore dell'impostazione, in caso chiamare il vecchio o il nuovo

    let modalitaAnalisiTerreno = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_Mod_Analisi_Terreno);

    if (!modalitaAnalisiTerreno) {
      modalitaAnalisiTerreno = new Utente_Impostazioni;
      modalitaAnalisiTerreno.Impostazione_Cod = enum_Impostazioni_Utenti.SUPERUSER_Mod_Analisi_Terreno;
      modalitaAnalisiTerreno.Valore = "1";
    }

    if (modalitaAnalisiTerreno.Valore == "1") {

      newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Edit_Attivita;

      newobjParametriAgenda.GenericObj_string = JSON.stringify({ Analisi_Testata_Cod: analisiTestataCod });

      this.gestioneRichiesteService.gestionePassaggioAltroSito(
        Enum_SiteRedirector.Sito_AgronicaAnalisi_2010,
        enum_PagineAnalisi_2010.Pagina_Analisi,
        parametriAggiuntivi,
        newobjParametriAgenda
      ).then(resp => {
        this.giasIFrameWindowService.open({
          title: this.transloco.translate('gis.AnalisiTerreno'),
          content: resp,
          height: window.innerHeight * 1.0,
          width: window.innerWidth * 0.4
        });
      });

    } else {

      // this.analisiService.LeggiAnalisiTerreno({ Analisi_Cod: Number(analisiTestataCod) }).subscribe(r => {
      //   let objResponse = r as AnalisiTerreno;

      newobjParametriAgenda.GenericObj_string = JSON.stringify({ Analisi_Testata_Cod: Number(analisiTestataCod) });
      newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
      newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_GIS;

      this.objParametriAgendaService.changeObjParametriAgenda(newobjParametriAgenda);

      // this.gestioneRichiesteService.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit).then(resp => {
      //   this.GiasIFrameWindowService.open({
      //     title: this.transloco.translate('gis.AnalisiTerreno'),
      //     content: resp,
      //     height: window.innerHeight * 1.0,
      //     width: window.innerWidth * 0.4
      //   });
      // });;

      this.gestioneRichiesteService.gestionePassaggioStessoSito_Aperto_in_Iframe(enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit,
        parametriAggiuntivi,
        -1,
        this.transloco.translate('gis.AnalisiTerreno'),
        true,
        window.innerHeight * 1.0,
        window.innerWidth * 0.6
      ).then();
      // });
    }

  }

  public layerBtnToggle(rightPosition: number): void {
    let width: number = 500;
    let left: number = window.innerWidth - width - rightPosition;
    let current: WindowArgs = this.kendoWindowService.getWindowArgs(WindowTypes.LayerWindow);
    let windowArgs: WindowArgs = current ?? new WindowArgs(WindowTypes.LayerWindow, false, this.transloco.translate('gis.Layers'), null, width, 250, left, DEFAULT_TOP_POSITION, false, false, true, true, true, true);
    this.btnToggle(windowArgs);
  }

  public drawBtnToggle(rightPosition: number): void {
    let width: number = 150;
    let left: number = window.innerWidth - width - rightPosition;
    let current: WindowArgs = this.kendoWindowService.getWindowArgs(WindowTypes.DrawWindow);
    let windowArgs = current ?? new WindowArgs(WindowTypes.DrawWindow, false, this.transloco.translate('gis.Disegna'), null, width, width, left, DEFAULT_TOP_POSITION, false, false, true, true, true, true);
    this.btnToggle(windowArgs);
  }

  public themeBtnToggle(
    toOpen?: boolean,
    setSide?: number, setWidth?: number
  ): void {
    if (toOpen === undefined) {
      toOpen = null;
    }
    let width = 0, minWidth = 0, left = 0, top = 0;
    if (setWidth && setSide) {
      width = setWidth;
      minWidth = width * 0.7;
      left = setSide;
      top = 10;
    } else {
      width = 800;
      minWidth = 650;
      left = 80;
      top = this.kendoWindowService.getWindowArgs(WindowTypes.LayerWindow).top;
    }
    let windowArgs: WindowArgs = new WindowArgs(
      WindowTypes.ThemeWindow, false,
      this.transloco.translate("gis.Theme"),
      null, width, minWidth, left, top,
      false, false, true, false, true
    );
    this.btnToggle(windowArgs, toOpen);
  }

  private printGis(): void {
    this.toolsDropdown?.toggle(false);
    window.print();
  }

  private openGestionePianoRateo(): void {
    if (this.gisGestionePianoRateoService == null) {
      this.giasDialogService.baseError('', 'gis.FunzionalitaNonDisponibile', true);
      return;
    }

    this.gisGestionePianoRateoService.open();
  }

  private btnToggle(windowArgs: WindowArgs, toOpen: boolean | null = null): void {
    // toOpen is used to force open or close the window
    if (toOpen != null) {
      if (toOpen === false) {
        this.kendoWindowService.close(windowArgs.windowType);
        return;
      }

      this.kendoWindowService.open(windowArgs.windowType, windowArgs, false);
      return;
    }

    if (this.getToggleState(windowArgs.windowType)) {
      this.kendoWindowService.close(windowArgs.windowType);
      return;
    }

    this.kendoWindowService.open(windowArgs.windowType, windowArgs);
  }

  public getToggleState(window: WindowTypes): boolean {
    return this.kendoWindowService.getOpenState(window) ?? false;
  }

  public changeLatFocusHandlerService(focusin: boolean): void {
    if (focusin) {
      this.showLatMaskTyped = true;
      if (this.latValue == '')
        this.latValue = this.latSgn;
    } else {
      this.showLatMaskTyped = false;
      if (this.latValue.length == 1) this.latValue = '';
    }
  }

  public changeLngFocusHandlerService(focusin: boolean): void {
    if (focusin) {
      this.showLngMaskTyped = true;
      if (this.lngValue == '')
        this.lngValue = this.lngSgn;
    } else {
      this.showLngMaskTyped = false;
      if (this.lngValue.length == 1) this.lngValue = '';
    }
  }

  public goToPosition(lat: string, lng: string): void {
    if (lat != '' && lng != '') {
      if (this.maskIdx == 0) {
        let latitude: number = Number.parseFloat(lat);
        let longitude: number = Number.parseFloat(lng);
        this.positionService.goToPosition(latitude, longitude, this.DEFAULT_FOCUS_ZOOM_LEVEL);
      } else {
        let latitude: number = this.calculateLatDecimalDeg(lat);
        let longitude: number = this.calculateLngDecimalDeg(lng);
        this.positionService.goToPosition(latitude, longitude, this.DEFAULT_FOCUS_ZOOM_LEVEL);
      }
    }
  }

  public goToNumericPosition(lat: number, lng: number): void {
    this.positionService.goToPosition(lat, lng);
  }

  public changeMask(): void {
    this.convertValue();
    this.maskIdx = this.maskIdx == enum_FormatoCoordinate.GradiDecimali
      ? enum_FormatoCoordinate.GradiMinutiSecondi
      : enum_FormatoCoordinate.GradiDecimali;
  }

  public toggleMarkerPlacer(): void {
    this.markerPlacerActive.next(!this.markerPlacerActive.getValue());
  }

  get isMarkerPlacerActive$(): Observable<boolean> {
    return this.markerPlacerActive.asObservable();
  }

  get isMarkerPlacerActive(): boolean {
    return this.markerPlacerActive.getValue();
  }

  private convertValue(): void {
    if (this.maskIdx == enum_FormatoCoordinate.GradiDecimali) {
      let latLngGms = this.conversioneInGradiMinutiSecondi(this.latValue, this.lngValue)
      this.latValue = latLngGms.lat;
      this.lngValue = latLngGms.lng;
    } else {
      let latLngDec = this.conversioneInGradiDecimali(this.latValue, this.lngValue)
      this.latValue = latLngDec.lat;
      this.lngValue = latLngDec.lng;
    }
  }

  conversioneInGradiMinutiSecondi(latDec: string, lngDec: string): { lat: string, lng: string } {
    let latLngGms = {
      lat: "",
      lng: ""
    }
    let latitude: number = Number.parseFloat(latDec == '' ? '0' : latDec);
    let longitude: number = Number.parseFloat(lngDec == '' ? '0' : lngDec);
    if (latDec != '') {
      latLngGms.lat = this.convertNumber(latitude, true);
    }
    if (lngDec != '') {
      latLngGms.lng = this.convertNumber(longitude, false);
    }
    return latLngGms;
  }

  private convertNumber(n: number, isLat: boolean): string {
    let result = '';
    result = `${n >= 0 ? '+' : '-'}`;
    result += `${isLat ? (Math.floor(n) >= 10 ? Math.floor(n) : '0' + Math.floor(n)) : (Math.floor(n) >= 100 ? Math.floor(n) : (Math.floor(n) >= 10 ? '0' + Math.floor(n) : '00' + Math.floor(n)))}°`;
    result += `${Math.floor((n % 1) * 60) >= 10 ? Math.floor((n % 1) * 60) : '0' + Math.floor((n % 1) * 60)}'`;
    result += `${Math.floor((((n % 1) * 60) % 1) * 60) >= 10 ? Math.floor((((n % 1) * 60) % 1) * 60) : '0' + Math.floor((((n % 1) * 60) % 1) * 60)}"`;
    return result;
  }

  conversioneInGradiDecimali(latGms: string, lngGms: string): { lat: string, lng: string } {
    let latLngDec = {
      lat: "",
      lng: ""
    }
    if (latGms != '') {
      let latitude: number = this.calculateLatDecimalDeg(latGms);
      latLngDec.lat = `${latitude >= 0 ? '+' : '-'}${latitude >= 10 ? latitude.toFixed(7).toString() : '0' + latitude.toFixed(7).toString()}`;
    }
    if (lngGms != '') {
      let longitude: number = this.calculateLngDecimalDeg(this.lngValue);
      latLngDec.lng = `${longitude >= 0 ? '+' : '-'}${longitude >= 100 ? longitude.toFixed(7).toString() : (longitude >= 10 ? '0' + longitude.toFixed(7).toString() : '00' + longitude.toFixed(7).toString())}`;
    }
    return latLngDec;
  }

  private calculateLatDecimalDeg(lat: string): number {
    let latitude: number = Number.parseFloat(lat.split('°')[0]);
    let latP: string = lat.split('°')[1].split('\'')[0];
    let latS: string = lat.split('°')[1].split('\'')[1]?.split('"')[0];
    latitude += Number.parseFloat(latP == '' || latP == undefined ? '0' : latP) / 60;
    latitude += Number.parseFloat(latS == '' || latS == undefined ? '0' : latS) / 3600;

    return latitude;
  }

  private calculateLngDecimalDeg(lng: string): number {
    let longitude = Number.parseFloat(lng.split('°')[0]);
    let lngP = lng.split('°')[1].split('\'')[0];
    let lngS = lng.split('°')[1].split('\'')[1]?.split('"')[0];
    longitude += Number.parseFloat(lngP == '' || lngP == undefined ? '0' : lngP) / 60;
    longitude += Number.parseFloat(lngS == '' || lngS == undefined ? '0' : lngS) / 3600;

    return longitude;
  }

  private gestioneRedirectToAgenda_2010(p: enum_PagineAgenda_2010): void {
    if (this.featureService.getUltimaFeatureSelezionata() != undefined) {
      this.redirectToAgenda_2010(p);
    } else {
      this.giasDialogService.alertMessage(this.transloco.translate('gis.NecessariaSelezioneElementoGrafico'));
    }
  }

  private redirectToAgenda_2010(page: enum_PagineAgenda_2010): void {
    if (
      page == enum_PagineAgenda_2010.Pagina_DSS_Difesa &&
      !this.permessiUtenteService.getPermesso(enum_Security_Attivita.Analisi_Modelli_Previsionali, enum_Security_Operazione.Modifica)
    ) return;

    if (
      page == enum_PagineAgenda_2010.Pagina_DatiReteAcqua &&
      !this.permessiUtenteService.getPermesso(enum_Security_Attivita.ReteAcqua_AnalisiDati, enum_Security_Operazione.Lettura)
    ) return;

    const newObjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgendaService.getObjParamValue()));
    newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_GIS;
    const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

    newObjParametriAgenda.Chiave = this.extractParametriChiave(page);
    newObjParametriAgenda.Veg_Cod = +this.featureService.getUltimaFeatureSelezionata().properties.veg_cod;

    this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      page,
      parametriAggiuntivi,
      newObjParametriAgenda,
      true,
      -1,
      this.getIFrameTitle(page),
    ).then();
  }

  private extractLatLngToXYZ(): Array<ParametriAggiuntivi_QueryString> {
    const feature = this.featureService.getUltimaFeatureSelezionata();
    const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
    const path: Array<google.maps.LatLng> = this.featureInformationService.getPath(feature.properties.id);
    const position: google.maps.LatLng = Mappa.polylabel(path);

    let xyz: any = { X: position.lng(), Y: position.lat() }
    return xyz;
  }

  private extractParametriChiave(p: enum_PagineAgenda_2010): string {
    switch (p) {
      case enum_PagineAgenda_2010.Pagina_DSS_Difesa:
        return JSON.stringify(this.extractLatLngToXYZ());
      case enum_PagineAgenda_2010.Pagina_Analisi_Meteo:
        return JSON.stringify(this.extractLatLngToXYZ());
      case enum_PagineAgenda_2010.Pagina_Analisi_Rilievi:
        return JSON.stringify(this.extractLatLngToXYZ());
      case enum_PagineAgenda_2010.Pagina_DatiReteAcqua:
        return JSON.stringify(this.extractLatLngToXYZ());
      default:
        return '';
    }
  }

  private getIFrameTitle(page: enum_PagineAgenda_2010): string {
    switch (page) {
      case enum_PagineAgenda_2010.Pagina_DSS_Difesa:
        return this.transloco.translate('gis.GestioneAnalisiDSSDaSelezione');
      case enum_PagineAgenda_2010.Pagina_Analisi_Meteo:
        return this.transloco.translate('gis.AnalisiMeteo');
      case enum_PagineAgenda_2010.Pagina_Analisi_Rilievi:
        return this.transloco.translate('gis.AnalisiDatiSchedeRilievi');
      case enum_PagineAgenda_2010.Pagina_DatiReteAcqua:
        return this.transloco.translate('gis.AnalisiDatiReteAcqua');
      default:
        return '';
    }
  }
}
