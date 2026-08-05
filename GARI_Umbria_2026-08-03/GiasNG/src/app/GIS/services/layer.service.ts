import { Injectable } from '@angular/core';
import { BackEndColor } from 'app/Model/GIS/BackEndColor';
import {
  AggiornaElencoTipologie_In,
  DatiLayer,
  GisClient,
  GisDataReadParam,
  ObjOptionHTML_Out,
  RispostaStandard,
  RispostaStandard_1OfElencoTipologieLayer,
  SalvaColoriLayer2_In,
  TipologiaLayer,
  AttributoLayer
} from 'app/Service/api.service';
import { getServiceIdAndLog } from 'app/Service/utils';
import {BehaviorSubject, from, Observable, of, ReplaySubject, share, Subject, switchMap} from 'rxjs';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { SharedDataService } from './shared-data.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { DrawingManagerService } from './drawing-manager.service';
import { enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { MasterService } from 'app/Service/master.service';
import { GISUtility } from '../../Service/utils/GIS-utils/GIS-utility';
import { HttpContext } from '@angular/common/http';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { tap } from 'rxjs';
import { map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { SementieriService } from 'app/Service/sementieri.service';
import { take } from 'rxjs';

export class paramLoadGeoJson {
  forzaCaricamento: boolean;
  readParameters: GisDataReadParam;
}

@Injectable()
export class LayerService {

  public endLoadLayers: Subject<[boolean, GisDataReadParam]> = new Subject<[boolean, GisDataReadParam]>();

  // Lettura, refresh e cambio ordinamento layer
  public ObservableLayer: ReplaySubject<TipologiaLayer[]> = new ReplaySubject<TipologiaLayer[]>(1);

  // Raggruppa etichette
  public GroupLabels: Subject<boolean> = new Subject<boolean>();

  // Raggruppamento per tutti i layers
  public AllLayerGrouping: Subject<boolean> = new Subject<boolean>();

  // Mostra etichette
  public ShowLabels: Subject<boolean> = new Subject<boolean>();

  // Tutti i layer visibili
  public AllLayersVisible: Subject<boolean> = new Subject<boolean>();

  // fires when a layer is deleted
  public layerDeleted: Subject<TipologiaLayer> = new Subject<TipologiaLayer>();

  // Layer selezionato
  private LayerItemSelected: BehaviorSubject<[TipologiaLayer, boolean]> = new BehaviorSubject<[TipologiaLayer, boolean]>([undefined, false]);
  public get layerItemSelected(): [TipologiaLayer, boolean] {
    return this.LayerItemSelected.getValue();
  }
  public get layerItemSelected$(): Observable<[TipologiaLayer, boolean]> {
    return this.LayerItemSelected.asObservable();
  }
  public setLayerItemSelected(layerSelected: [TipologiaLayer, boolean]): void {
    this.LayerItemSelected.next(layerSelected);
    if (layerSelected[1])
      this.sharedDataService.setLayerSelezionato(layerSelected[0]);
  }

  // Etichetta layer visibile
  public LayerItemLabelVisible: Subject<[TipologiaLayer, boolean]> = new Subject<[TipologiaLayer, boolean]>();

  // Raggruppamento del layer
  public LayerItemGrouping: Subject<[TipologiaLayer, boolean]> = new Subject<[TipologiaLayer, boolean]>();

  // Layer visibile
  public LayerItemVisible: Subject<[TipologiaLayer, boolean]> = new Subject<[TipologiaLayer, boolean]>();

  // Layer cambio colore
  public LayerItemChangeColor: Subject<TipologiaLayer> = new Subject<TipologiaLayer>();

  // Layer centra mappa
  public LayerItemCenterMap: BehaviorSubject<string> = new BehaviorSubject<string>('');

  // Layer selezionato dal dropdown
  public LayerSelected: BehaviorSubject<ObjOptionHTML_Out> = new BehaviorSubject<ObjOptionHTML_Out>(undefined);

  // Lista Layer Items
  public LayerItems: Subject<ObjOptionHTML_Out[]> = new ReplaySubject<ObjOptionHTML_Out[]>();

  // Layer visibile
  public ListLayerItemVisible: BehaviorSubject<Array<{ TipologiaLayer: TipologiaLayer, visible: boolean }>> = new BehaviorSubject<Array<{ TipologiaLayer: TipologiaLayer, visible: boolean }>>([]);

  // Layer con temi
  public esistonoLayerConTemi: boolean = false;

  public reloadPageNotifier = new Subject();

  private visibleLayersMap = new Map<number, BehaviorSubject<boolean>>;

  private layerAttributesByIdSource = new Map<number, AttributoLayer[]>();

  // Permessi

  //---- Avversità + Fenologia
  public permessoAgendaAccessoMenu = false;
  //---- Rilievi Vegeto Produttivi + Analisi Cronologia Agenda
  public permessoAnalisiCurveMaturazione = false;
  //---- Percorsi
  public permessoReportRaccolteGIS = false;
  //---- Organizzazione di Appartenenza
  public permessoLinkAgronicaSementi = false;

  // Debug

  private serviceId = null;

  constructor(
    private gisClient: GisClient,
    private sharedDataService: SharedDataService,
    private permessiUtenteService: PermessiUtenteService,
    private masterService: MasterService,
    private route: ActivatedRoute,
    private sementieriService: SementieriService
  ) {

    this.serviceId = getServiceIdAndLog('LayerService', 'constructor');

    this.getPermessiTipologieLayer();
    this.getLayers();

    this.ObservableLayer.subscribe(list => {
      this.ListLayerItemVisible.next(list.map(e => {
        return { TipologiaLayer: e, visible: true }
      }));
    });
  }

  private getPermessiTipologieLayer() {
    this.permessoAgendaAccessoMenu = this.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG);
    this.permessoAnalisiCurveMaturazione = this.getPermesso(enum_Security_Attivita.Analisi_Curve_Maturazione);
    this.permessoReportRaccolteGIS = this.getPermesso(enum_Security_Attivita.ReportRaccolteGIS);
    this.permessoLinkAgronicaSementi = this.getPermesso(enum_Security_Attivita.Link_AgronicaSementi);
  }

  private getPermesso(enumPermesso: enum_Security_Attivita): boolean {
    const permessoLettura = this.permessiUtenteService.getPermesso(enumPermesso, 0);
    const permessoScrittura = this.permessiUtenteService.getPermesso(enumPermesso, 2);
    return permessoLettura || permessoScrittura;
  }

  public getLayers() {
    this.gisClient
      .gisCaricaDdlTipologiaLayer()
      .subscribe(result => {
        const isIframe = this.route.snapshot.queryParams.seFrame == 1;
        let selectedLayer = isIframe ? enum_TipologiaLayer.Entita : localStorage.getItem("selectedLayer");
        if (!selectedLayer) {
          selectedLayer = enum_TipologiaLayer.Entita;
        }
        let layerSelected = result.RispostaStringa?.find(item => item.Option_Value === selectedLayer);
        this.LayerSelected.next(layerSelected);
        // TODO Andrea (3): filtro temporaneo per primo rilascio
        let ddlTipologiaLayer = result.RispostaStringa?.filter(item => this.isTipologiaLayerAbilitati(item.Option_Value));
        this.LayerItems.next(ddlTipologiaLayer);
      });
  }

  public getLayerAttributesById(layerId: number): Observable<AttributoLayer[]> {
    if (!this.layerAttributesByIdSource.has(layerId)) {
      this.layerAttributesByIdSource.set(layerId, []);
      return this.gisClient.gisLeggiImpostazioniAvanzateLayer(layerId)
        .pipe(
          map(res => res.RispostaStringa.ListaAttributiLayer),
          tap(attributi => this.layerAttributesByIdSource.set(layerId, attributi)),
          share()
        );
    }

    return of(this.layerAttributesByIdSource.get(layerId));
  }

  public setLayerAttributesById(layerId: number, attributes: AttributoLayer[]): void {
    this.layerAttributesByIdSource.set(layerId, attributes);
  }

  isTipologiaLayerAbilitati(tipologiaLayerCod: string): boolean {
    let tipologieLayerAbilitati = [
      enum_TipologiaLayer.Entita.toString(),
      enum_TipologiaLayer.GruppoColturale.toString(),
      enum_TipologiaLayer.Cultivar.toString()
    ];

    if (this.permessoAgendaAccessoMenu) {
      tipologieLayerAbilitati.push(enum_TipologiaLayer.Avversita.toString());
      tipologieLayerAbilitati.push(enum_TipologiaLayer.Fenologia.toString());
    }
    if (this.permessoAnalisiCurveMaturazione) {
      tipologieLayerAbilitati.push(enum_TipologiaLayer.RilieviVegetoProduttivi.toString());
    }
    if (this.permessoLinkAgronicaSementi)
      tipologieLayerAbilitati.push(enum_TipologiaLayer.OrganizzazioneAppartenenza.toString());

    return tipologieLayerAbilitati.includes(tipologiaLayerCod);
  }

  public refreshFilterableLayers(
    forzaCaricamento: boolean,
    readParameters: GisDataReadParam
  ) {
    let tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();
    if (this.sharedDataService.tipiLayerDaFiltrare.includes(tipoLayerSelezionato)) {
      let objParamLoadGeoJson: paramLoadGeoJson = {
        forzaCaricamento: forzaCaricamento,
        readParameters: readParameters
      };
      this.refreshLayers(
        { Option_Value: tipoLayerSelezionato },
        objParamLoadGeoJson
      );
    } else {
      this.endLoadLayers.next([forzaCaricamento, readParameters]);
    }
  }

  public refreshLayers(item: ObjOptionHTML_Out, objParamLoadGeoJson?: paramLoadGeoJson, defaultValues: boolean = false) {
    if (item == null) {
      return;
    }

    localStorage.setItem("selectedLayer", item.Option_Value);

    const payload = {
      Layer_Selezionato: item.Option_Value
    } as AggiornaElencoTipologie_In;

    this.masterService.set_isLoading({ isLoading: true });
    let obs: (body?: AggiornaElencoTipologie_In | undefined, httpContext?: HttpContext) => Observable<RispostaStandard_1OfElencoTipologieLayer>;
    if (defaultValues) {
      obs = GISUtility.defaultGisAggiornaElencoTipologie;
    } else {
      obs = this.readLayersFromBackend.bind(this);
    }

    obs(payload)
      .subscribe(result => {
        if (result.RispostaStringa?.ListaTipologieLayer.every((layer: TipologiaLayer) => layer.MostraDescrizioneAssociata === '0')) {
          this.ShowLabels.next(false);
        }
        // Forzatura iniziale visibilità = false per fixed layer di tipo entità (es. WMS)
        let listaTipologieLayer: TipologiaLayer[] = [];
        if (result.RispostaStringa?.ListaTipologieLayer) {
          listaTipologieLayer = result.RispostaStringa.ListaTipologieLayer;
        }
        if (listaTipologieLayer && payload.Layer_Selezionato === enum_TipologiaLayer.Entita) {
          listaTipologieLayer.forEach(tipologiaLayer => {
            let idLayer = parseInt(tipologiaLayer.id);
            if (idLayer < 0) {
              tipologiaLayer.flagvisibile = '0';
            }
          });
        }

        listaTipologieLayer = this.checkFilterMultiAzienda(
          listaTipologieLayer,
          objParamLoadGeoJson
        );

        // Ritorno observable
        this.ObservableLayer.next(listaTipologieLayer); // se array non preordinato -> .sort((a, b)=> +a?.zindex - +b?.zindex));
        this.SeLayerConTemi(listaTipologieLayer);
        if (objParamLoadGeoJson) {
          this.endLoadLayers.next([
            objParamLoadGeoJson.forzaCaricamento,
            objParamLoadGeoJson.readParameters
          ]);
        }

        this.updateLayerVisible(listaTipologieLayer);

        this.masterService.set_isLoading({ isLoading: false });
      });
  }

  public readLayersFromBackend(payload: AggiornaElencoTipologie_In): Observable<RispostaStandard_1OfElencoTipologieLayer> {
    return this.sementieriService.isSementieriSportello()
      .pipe(
        take(1),
        switchMap(isSementieriSportello => {
          if (isSementieriSportello) {
            const sementieriParams = this.sharedDataService.getCfgSementiAsValue().Sementi;
            if (sementieriParams != null) {
              const sementieriSportelloConfigCod = sementieriParams.split('|')[4];
              payload.Sementieri_Sportello_Configurazione_cod = sementieriSportelloConfigCod;
            }
          }
          return this.gisClient.gisAggiornaElencoTipologie4(payload);
        }));
  }

  /**
   * Checks if we are loading filters inside a widget,
   * if so we need to filter the layers to show only
   * the ones selected in the widget
   * @param listaLayer the list containing all the layers
   * @param paramGeoJson the parameters to filter the layers
   * @returns the filtered list of layers or the original list
   */
  private checkFilterMultiAzienda(
    listaLayer: TipologiaLayer[],
    paramGeoJson: paramLoadGeoJson
  ): TipologiaLayer[] {
    let origineChiamata = this.sharedDataService.getOrigineChiamataLoadGeoJson();
    if (
      paramGeoJson?.readParameters.layerElementiGrafici_cod.length > 0 &&
      origineChiamata === enum_OrigineChiamataLoadGeoJson.WidgetMultiAzienda
    ) {
      let list = listaLayer.filter(
        (layer) =>
          paramGeoJson?.readParameters.layerElementiGrafici_cod.includes(parseInt(layer.id))
      );
      this.setLayerItemSelected([list[0], true]);
      return list;
    } else {
      return listaLayer;
    }
  }

  private SeLayerConTemi(listaTipologieLayer: TipologiaLayer[]) {
    this.esistonoLayerConTemi = listaTipologieLayer.some(tl => tl.tiles != undefined && tl.tiles.length > 0);
  }

  public submit(tipologia: number, payloadData: DatiLayer[]): Observable<RispostaStandard> {
    const payload = {
      Tipologia: tipologia,
      ListaDatiLayer: payloadData
    } as SalvaColoriLayer2_In;

    return this.gisClient.gisSalvaColoriLayer2(payload);
  }

  public reloadLayers(layersList: TipologiaLayer[]) {
    this.ObservableLayer.next(layersList);
  }

  public toggleLayerItemSelected(layer: TipologiaLayer, selected: boolean) {
    this.setLayerItemSelected([layer, selected]);
  }

  public toggleLayerItemVisible(layer: TipologiaLayer, visible?: boolean) {
    const isCurrentlyVisible = layer.flagvisibile === '1';
    const newVisibility = visible ?? !isCurrentlyVisible;
    layer.flagvisibile = newVisibility ? '1' : '0';
    this.LayerItemVisible.next([layer, newVisibility]);
    this.updateListLayerItemVisibile(layer, newVisibility);

    this.updateLayerVisible([layer]);
  }

  public layerItemCenterMap(idLayer: string) {
    this.LayerItemCenterMap.next(idLayer);
  }

  private updateListLayerItemVisibile(layer: TipologiaLayer, visible: boolean): void {
    let list = this.ListLayerItemVisible.getValue();

    this.ListLayerItemVisible.next(list.map(e => {
      if (e.TipologiaLayer == layer)
        e.visible = visible;

      return e;
    }));
  }

  public toggleLayerItemGrouping(layer: TipologiaLayer, active?: boolean) {
    const isCurrentlyActive = layer.RaggruppaDescrizioneAssociata === '1';
    const newActivation = active ?? !isCurrentlyActive;
    layer.RaggruppaDescrizioneAssociata = newActivation ? '1' : '0';
    this.LayerItemGrouping.next([layer, newActivation]);
  }

  public toggleLayerItemLabelVisible(layer: TipologiaLayer, visible?: boolean) {
    const isCurrentlyVisible: boolean = layer.MostraDescrizioneAssociata === '1';
    const newVisibility: boolean = visible ?? !isCurrentlyVisible;
    layer.MostraDescrizioneAssociata = newVisibility ? '1' : '0';
    this.LayerItemLabelVisible.next([layer, newVisibility]);
  }

  public LayerSelectedTrigger(type: ObjOptionHTML_Out) {
    this.LayerSelected.next(type);
  }

  public getBackEndColorByHex(hexColor: string, defaultTransparency: string): BackEndColor {
    // Il colore viene inviato al BackEnd in esadecimale senza #,
    // mentre la trasparenza in formato stringa '0.x'.
    // E' necessario dividere il colore dalla trasparenza nel colore in output
    // e convertire la trasparenza in numero e successivamente in stringa.

    let backEndColor = new BackEndColor();

    // Trasparenza
    const transparency = hexColor.length > 7 ? (+('0x' + hexColor.substring(7)) / 0xff).toFixed(1) : 1 //defaultTransparency;
    backEndColor.Transparency = transparency.toString().substring(0, 4);

    // Colore
    backEndColor.Color = hexColor.substring(1, 7);

    // Ritorno
    return backEndColor;
  }

  public getSliderTransparencyByHex(hexColor: string): string {
    const backEndColor = this.getBackEndColorByHex(hexColor, '1');
    return this.getSliderTransparencyByOpacity(backEndColor.Transparency);
  }

  public getSliderTransparencyByOpacity(opacity: string): string {
    const opacityNumber = parseFloat(opacity.replace(',', '.'));
    const transparencyNumber = 100 - (opacityNumber * 100);
    return transparencyNumber.toFixed(0);
  }

  public getWhiteHexColorBySliderTransparency(sliderTransparency: string): string {
    const whiteHexColor = "#ffffff";

    const opacityNumber = 100 - parseInt(sliderTransparency);

    const opacityHexNumber: number = parseInt((opacityNumber / 100 * 255).toFixed(0));
    const opacityHex = opacityHexNumber.toString(16);

    return whiteHexColor + opacityHex;
  }

  public isLayerVisible(layerId: string): boolean {
    return this.ListLayerItemVisible.getValue().find(e => e.TipologiaLayer.id == layerId)?.visible;
  }

  public static areLayersToBeReloaded(oldLayers: TipologiaLayer[], newLayers: TipologiaLayer[]): boolean {
    if (oldLayers.length != newLayers.length) {
      return true;
    }

    for (let i = 0; i < newLayers.length; i++) {
      const oldLayer = oldLayers[i];
      const newLayer = newLayers[i];
      if (JSON.stringify(oldLayer) != JSON.stringify(newLayer)) {
        return true;
      }
    }

    return false;
  }

  public getIsLayerVisible$(tipologiaLayer: number): Observable<boolean> {
    if (this.visibleLayersMap.has(+tipologiaLayer)) {
      return this.visibleLayersMap.get(+tipologiaLayer).asObservable();
    }

    const newSubject = new BehaviorSubject<boolean>(false);
    this.visibleLayersMap.set(+tipologiaLayer, newSubject);
    return newSubject.asObservable();
  }

  private updateLayerVisible(visibleLayers: TipologiaLayer[]): void {
    visibleLayers.forEach(layer => {
      if (this.visibleLayersMap.has(+layer.id)) {
        this.visibleLayersMap.get(+layer.id).next(layer.flagvisibile == '1' && layer.flagattivo == '1');
      } else {
        this.visibleLayersMap.set(+layer.id, new BehaviorSubject<boolean>(layer.flagvisibile === '1'));
      }
    });
  }
}
