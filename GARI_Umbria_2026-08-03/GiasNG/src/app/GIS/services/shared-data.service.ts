import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable, Subject, withLatestFrom} from 'rxjs';
import { TipologiaLayer, CfgAlbero_CfgGisUtente, ConfigurazioniGisGenerali, FiltroTemporale, FiltroTemporale_enum_OperatoreFiltroTemporale, FiltroTemporale_enum_TipoFiltroTemporale, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { FixedLayerProperty } from 'app/Model/GIS/FixedLayerProperty';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { DatiFeatureConAttributi } from 'app/Model/GIS/FeatureConAttributi';
import { DatiColoreTema, LimitiValoriTema, TemaSelezionato } from '../GIS-kendo-window/theme-window/theme-window.component';
import { enum_FeatureGeometryType  } from '../GIS-enum/GIS-feature';
import {SementieriParametrizzazione} from '../../Model/GIS/SementieriParametrizzazione';
import { RgbUtility } from 'gias-ui-kit';
import { DateUtils, FiltroTemporaleAvanzato } from 'app/Utility/date-utils';

export class PermessiLayer {
  inserimento: boolean = false;
  modifica: boolean = false;
  cancellazione: boolean = false;
  informazioni: boolean = false;
}

@Injectable({
  providedIn: 'root',
})
export class SharedDataService{

  //--------------------------------------------------------------------------------
  // Filtro temporale
  //--------------------------------------------------------------------------------

  filtroTemporaleAvanzato: FiltroTemporaleAvanzato = null;

  filtroTemporaleAvanzatoSource = new BehaviorSubject(this.filtroTemporaleAvanzato);

  //--------------------------------------------------------------------------------
  // Configurazioni
  //--------------------------------------------------------------------------------

  private cfg_GisGenerali: ConfigurazioniGisGenerali = null;

  private cfg_GisGeneraliSource = new BehaviorSubject(this.cfg_GisGenerali);

  private cfg_Albero_GisUtente: CfgAlbero_CfgGisUtente = null;

  private ricaricaAlberoCfg: boolean = false;

  private cfgAlberoGisUtenteSource = new BehaviorSubject<[CfgAlbero_CfgGisUtente,boolean]>(
    [this.cfg_Albero_GisUtente,this.ricaricaAlberoCfg]
  );

  private cfgSementi: BehaviorSubject<SementieriParametrizzazione> = new BehaviorSubject<SementieriParametrizzazione>(null);

  // public readonly iAutoZoomSuVisTotaleLocalStorage = false;

  //--------------------------------------------------------------------------------
  // Layer
  //--------------------------------------------------------------------------------

  readonly tipiLayerDaFiltrare: string[] = [
    enum_TipologiaLayer.GruppoColturale,
    enum_TipologiaLayer.Avversita,
    enum_TipologiaLayer.Fenologia,
    enum_TipologiaLayer.RilieviVegetoProduttivi,
    enum_TipologiaLayer.Cultivar
  ];

  TipologiaLayerSource: BehaviorSubject<Array<TipologiaLayer>> = new BehaviorSubject(new Array<TipologiaLayer>());

  TipoLayerSelezionatoSource: BehaviorSubject<string> = new BehaviorSubject('');

  private LayerSelezionatoSource: BehaviorSubject<TipologiaLayer> = new BehaviorSubject(null);
  private layersById: Map<string, TipologiaLayer> = new Map<string, TipologiaLayer>();
  public getLayerSelezionatoSourceAsValue(): TipologiaLayer {
    return this.LayerSelezionatoSource.getValue();
  }
  public getLayerSelezionatoSourceAsObs(): Observable<TipologiaLayer> {
    return this.LayerSelezionatoSource.asObservable();
  }

  FixedLayerProperty = new FixedLayerProperty();

  FixedLayerPropertySource = new BehaviorSubject(this.FixedLayerProperty);

  //--------------------------------------------------------------------------------
  // Tematizzazioni
  //--------------------------------------------------------------------------------

  TemaSelezionato: TemaSelezionato | null = null;

  TemaSelezionatoSource = new BehaviorSubject<TemaSelezionato | null>(this.TemaSelezionato);

  ScalaColoriTema: DatiColoreTema[] = [];

  ScalaColoriTemaSource = new BehaviorSubject(this.ScalaColoriTema);

  //--------------------------------------------------------------------------------
  // Albero
  //--------------------------------------------------------------------------------

  private treeViewCheckedKeysSource = new BehaviorSubject<any[]>([]);
  private lastCheckedKeyFeatureIdSource = new BehaviorSubject(null);
  private ricaricaAlberoSource = new BehaviorSubject<boolean>(false);
  private visualizzazioneTotaleSource = new BehaviorSubject<boolean>(false);
  private resetVisualizzazioneTotaleSource = new Subject<boolean>();

  //--------------------------------------------------------------------------------
  // Feature
  //--------------------------------------------------------------------------------

  public ricaricaFeature: boolean = false;

  ricaricaFeatureSource = new BehaviorSubject(this.ricaricaFeature);

  public origineChiamataLoadGeoJson: enum_OrigineChiamataLoadGeoJson = enum_OrigineChiamataLoadGeoJson.Indefinito;

  origineChiamataLoadGeoJsonSource = new BehaviorSubject(this.origineChiamataLoadGeoJson);

  public geoJsonLoaded: Subject<boolean> = new Subject<boolean>();

  public datiFeatureConAttributi: DatiFeatureConAttributi = {
    DatiCompleti: false
  };

  datiFeatureConAttributiSource = new BehaviorSubject(this.datiFeatureConAttributi);

  public closeAttributiTable: Subject<any> = new Subject<any>();

  // loadGeoJsonForzato true/false + impostaCentroMappa: true/false
  public loadGeoJsonForzato: Subject<[boolean, boolean]> = new Subject<[boolean, boolean]>();

  //--------------------------------------------------------------------------------
  // Variabili private
  //--------------------------------------------------------------------------------

  private validazioneRGB = /^#[0-9A-F]{6}$/i;

  //--------------------------------------------------------------------------------
  // Layer
  //--------------------------------------------------------------------------------

  public setTipologiaLayer(tipologiaLayer: TipologiaLayer[]) {
    this.TipologiaLayerSource.next(tipologiaLayer);

    this.layersById = new Map<string, TipologiaLayer>(tipologiaLayer.map(layer => [layer.id, layer]));
  }

  public getTipologiaLayer(): TipologiaLayer[] {
    return this.TipologiaLayerSource.getValue();
  }

  public getTipologiaLayerFiltered(movableLayer: boolean, fixedLayer: boolean): TipologiaLayer[] {
    const elencoLayer = this.TipologiaLayerSource.getValue();
    // Elenco completo
    if (movableLayer && fixedLayer) {
      return elencoLayer;
    }
    let elencoLayerFiltered = null;
    // Elenco layer movibili
    if (movableLayer) {
      elencoLayerFiltered = elencoLayer.filter(obj => parseInt(obj.id) >= 0);
    }
    // Elenco layer fissi
    if (fixedLayer) {
      elencoLayerFiltered = elencoLayer.filter(obj => parseInt(obj.id) < 0);
    }
    // Elenco filtrato
    return elencoLayerFiltered;
  }

  public getTipologiaLayerById(idLayer: string): TipologiaLayer {
    return this.layersById.get(idLayer);
  }

  public getLayerPresente(idLayer: string): boolean {
    let datiLayer = this.getTipologiaLayerById(idLayer);
    return datiLayer != null;
  }

  public getColoreLayerRGB(idLayer: string): string {
    let nessunErrore = true;
    let coloreRGB = "#";
    let colore = "";
    let datiLayer = this.getTipologiaLayerById(idLayer);

    if (datiLayer != null) {
      colore = datiLayer.colore_1;
    } else {
      // console.log(`[getColoreLayerRGB] Dati layer ${idLayer} non trovati`);
      nessunErrore = false;
    }

    if (nessunErrore) {
      coloreRGB += colore;

      if (!this.validazioneRGB.test(coloreRGB)) {
        // console.log(`[getColoreLayerRGB] Colore layer ${idLayer} errato: ${coloreRGB}`)
        nessunErrore = false;
      }
    }

    if (!nessunErrore) {
      coloreRGB = 'red';
    }

    return coloreRGB;
  }

  public getTrasparenzaLayer(idLayer: string): number {
    let trasparenzaStringa = "0.4";
    let datiLayer = this.getTipologiaLayerById(idLayer);

    if (datiLayer != null) {
      trasparenzaStringa = datiLayer.trasparenza;
    }

    return  parseFloat(trasparenzaStringa.replace(',','.'));
  }

  public getMostraDescrizioneAssociata(idLayer: string): boolean {
    let mostraDescrizioneAssociata = true;
    let datiLayer = this.getTipologiaLayerById(idLayer);

    if (datiLayer != null) {
      mostraDescrizioneAssociata = datiLayer.MostraDescrizioneAssociata === "1";
    }

    return mostraDescrizioneAssociata;
  }

  public getRaggruppaDescrizioneAssociata(idLayer: string): boolean {
    let raggruppaDescrizioneAssociata = true;
    let datiLayer = this.getTipologiaLayerById(idLayer);

    if (datiLayer != null) {
      raggruppaDescrizioneAssociata = (datiLayer.RaggruppaDescrizioneAssociata === "1") ? true : false;
    }

    return raggruppaDescrizioneAssociata;
  }

  public getFlagVisibile(idLayer: string): boolean {
    let flagVisibile = true;
    let datiLayer = this.getTipologiaLayerById(idLayer);

    if (datiLayer != null) {
      flagVisibile = this.getFlagVisibileBoolean(datiLayer.flagvisibile);
    }

    return flagVisibile;
  }

  public getFlagVisibileBoolean(flagVisibile: string): boolean {
    return flagVisibile === "1";
  }

  public setTipoLayerSelezionato(tipoLayerSelezionato: string) {
    this.TipoLayerSelezionatoSource.next(tipoLayerSelezionato);
  }

  public getTipoLayerSelezionato(): string {
    return this.TipoLayerSelezionatoSource.getValue();
  }

  public selezionatoTipoLayerEntita() {
    return this.getTipoLayerSelezionato() === enum_TipologiaLayer.Entita
  }

  public setLayerSelezionato(layerSelezionato: TipologiaLayer) {
    this.LayerSelezionatoSource.next(layerSelezionato);
  }

  public getLayerSelezionato(): TipologiaLayer {
    return this.LayerSelezionatoSource.getValue();
  }

  public getPermessiLayerSelezionato(): PermessiLayer {
    let layer = this.LayerSelezionatoSource.getValue();
    return this.getPermessiLayer(layer);
  }

  public getPermessiLayer(layer: TipologiaLayer): PermessiLayer {
    let permessiLayer: PermessiLayer = new PermessiLayer();

    if (layer != undefined) {
      permessiLayer.inserimento = layer.FlagInserimento === '1';
      permessiLayer.modifica = layer.FlagModifica === '1';
      permessiLayer.cancellazione = layer.FlagCancellazione === '1';
      permessiLayer.informazioni = layer.FlagInformazioni === '1';
    }

    return permessiLayer;
  }

  public setFixedLayerProperty(fixedLayerProperty: FixedLayerProperty) {
    this.FixedLayerPropertySource.next(fixedLayerProperty);
  }

  public getFixedLayerProperty(): FixedLayerProperty {
    return this.FixedLayerPropertySource.getValue();
  }

  //--------------------------------------------------------------------------------
  // Tematizzazioni
  //--------------------------------------------------------------------------------

  public setTemaSelezionato(temaSelezionato: TemaSelezionato) {
    this.TemaSelezionatoSource.next(temaSelezionato);
  }

  public getTemaSelezionato(): TemaSelezionato | null {
    return this.TemaSelezionatoSource.getValue();
  }

  public setScalaColoriTema(scalaColoriTema: DatiColoreTema[]) {
    this.ScalaColoriTemaSource.next(scalaColoriTema);
  }

  public getScalaColoriTema(): DatiColoreTema[] {
    return this.ScalaColoriTemaSource.getValue();
  }

  //--------------------------------------------------------------------------------
  // Feature
  //--------------------------------------------------------------------------------

  public setRicaricaFeature(ricaricaFeature: boolean) {
    this.ricaricaFeatureSource.next(ricaricaFeature);
  }

  public getRicaricaFeature(): boolean {
    return this.ricaricaFeatureSource.getValue();
  }

  public setOrigineChiamataLoadGeoJson(origineChiamataLoadGeoJson: enum_OrigineChiamataLoadGeoJson) {
    this.origineChiamataLoadGeoJsonSource.next(origineChiamataLoadGeoJson);
  }

  public getOrigineChiamataLoadGeoJson(): enum_OrigineChiamataLoadGeoJson {
    return this.origineChiamataLoadGeoJsonSource.getValue();
  }

  public setDatiFeatureConAttributi(datiFeatureConAttributi: DatiFeatureConAttributi) {
    this.datiFeatureConAttributiSource.next(datiFeatureConAttributi);
  }

  public getDatiFeatureConAttributi(): DatiFeatureConAttributi {
    return this.datiFeatureConAttributiSource.getValue();
  }

  //--------------------------------------------------------------------------------
  // Filtro temporale
  //--------------------------------------------------------------------------------

  public setCustomTimeFilter(startDate: Date, endDate: Date): void {
    // Filtro Temporale
    let filtroTemporaleAvanzatoDefault = new FiltroTemporaleAvanzato();

    filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      DataInizio: new Date(startDate),
      DataFine: new Date(endDate),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    const dataInizio: Date = filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo.DataInizio;
    const dataFine: Date = filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo.DataFine;
    dataInizio.setHours(0, 0, 0, 0);

    filtroTemporaleAvanzatoDefault.filtroTemporaleSingolaData = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(dataInizio),
      DataFine: DateUtils.calcolaDataFineSingolaData(dataFine),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale
    } as FiltroTemporale;;

    this.setFiltroTemporaleAvanzato(filtroTemporaleAvanzatoDefault);
  }

  public setFiltroTemporaleAvanzato(filtroTemporaleAvanzato: FiltroTemporaleAvanzato) {
    this.filtroTemporaleAvanzatoSource.next(filtroTemporaleAvanzato);
  }

  public getFiltroTemporaleAvanzato(): FiltroTemporaleAvanzato {
    return this.filtroTemporaleAvanzatoSource.getValue();
  }

  //--------------------------------------------------------------------------------
  // Configurazioni
  //--------------------------------------------------------------------------------

  public setCfgGisGenerali(cfg_GisGenerali: ConfigurazioniGisGenerali) {
    this.cfg_GisGeneraliSource.next(cfg_GisGenerali);
  }

  public getCfgGisGenerali(): ConfigurazioniGisGenerali {
    return this.cfg_GisGeneraliSource.getValue();
  }

  public getCfgGisGenerali$(): Observable<ConfigurazioniGisGenerali> {
    return this.cfg_GisGeneraliSource.asObservable();
  }

  public setCfgAlberoGisUtente(
    cfg_Albero_GisUtente: CfgAlbero_CfgGisUtente,
    ricaricaAlberoCfg: boolean
  ) {
    this.cfgAlberoGisUtenteSource.next([cfg_Albero_GisUtente,ricaricaAlberoCfg]);
  }

  public getCfgAlberoGisUtente(): [CfgAlbero_CfgGisUtente,boolean] {
    return this.cfgAlberoGisUtenteSource.getValue();
  }

  public getCfgAlberoGisUtente$(): Observable<[CfgAlbero_CfgGisUtente,boolean]> {
    return this.cfgAlberoGisUtenteSource.asObservable();
  }

  public getCfgSementiAsValue(): SementieriParametrizzazione {
    return this.cfgSementi.getValue();
  }

  public getCfgSementiAsObs(): Observable<SementieriParametrizzazione> {
    return this.cfgSementi.asObservable();
  }

  public setCfgSementi(s: SementieriParametrizzazione): void {
    this.cfgSementi.next(s);
  }

  //--------------------------------------------------------------------------------
  // Albero
  //--------------------------------------------------------------------------------

  public setTreeViewCheckedKeys(treeViewCheckedKeys: any[]) {
    this.treeViewCheckedKeysSource.next([...treeViewCheckedKeys]);
  }

  public getTreeViewCheckedKeys(): any[] {
    return this.treeViewCheckedKeysSource.getValue();
  }

  public setLastCheckedKeyFeatureId(lastCheckedKeyFeatureId: any): void {
    this.lastCheckedKeyFeatureIdSource.next(lastCheckedKeyFeatureId);
  }

  public getLastCheckedKeyFeatureId(): any {
    return this.lastCheckedKeyFeatureIdSource.getValue();
  }

  public setRicaricaAlbero(ricaricaAlbero: boolean) {
    this.ricaricaAlberoSource.next(ricaricaAlbero);
  }

  public getRicaricaAlbero(): boolean {
    return this.ricaricaAlberoSource.getValue();
  }

  public getRicaricaAlbero$(): Observable<boolean> {
    return this.ricaricaAlberoSource.asObservable();
  }

  public setVisualizzazioneTotale(visualizzazioneTotale: boolean) {
    this.visualizzazioneTotaleSource.next(visualizzazioneTotale);
  }

  public getVisualizzazioneTotale(): boolean {
    return this.visualizzazioneTotaleSource.getValue();
  }

  public getVisualizzazioneTotale$(): Observable<boolean> {
    return this.visualizzazioneTotaleSource.asObservable();
  }

  public setResetVisualizzazioneTotale(resetVisualizzazioneTotale: boolean) {
    this.resetVisualizzazioneTotaleSource.next(resetVisualizzazioneTotale);
  }

  public getResetVisualizzazioneTotale$(): Observable<boolean> {
    return this.resetVisualizzazioneTotaleSource.asObservable();
  }

  public isFeatureAvversita(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): boolean {
    return this.getTipoLayerSelezionato() == enum_TipologiaLayer.Avversita &&
      +feature.properties.layer > 0 &&
      feature.geometry.type == enum_FeatureGeometryType.Point;
  }

  public getFeatureScaleByLayerAndTheme(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, layer: TipologiaLayer, themeName: string): number {
    const nomeTema = this.getNomeTemaTipoLayer(layer.nome, themeName);
    let scale = this.getValoreScalaFeature(feature, nomeTema);
    return scale;
  }

  public getColoreTemaFeature(layerSelezionatoNome: string, temaSelezionatoNome: string, feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, scalaColori?: DatiColoreTema[]): string {
    if (scalaColori === undefined) {
      scalaColori = this.getScalaColoriTema();
    }

    return this.getColoreScalaFeature(feature, layerSelezionatoNome, temaSelezionatoNome, scalaColori);
  }

  public getLimitiTematizzazioneLayer(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], layerSelezionato: TipologiaLayer, nomeTema: string): LimitiValoriTema {
    const limitiValori: LimitiValoriTema = {
      valoreMin: 999999,
      valoreMax: 0
    };

    features.forEach(feature => {
      const layerFeature = feature.properties.layer;
      if (layerFeature === layerSelezionato.id) {
        let valoreScala = this.getValoreScalaFeature(feature, nomeTema);
        if (valoreScala < limitiValori.valoreMin) {
          limitiValori.valoreMin = valoreScala;
        }
        if (valoreScala > limitiValori.valoreMax) {
          limitiValori.valoreMax = valoreScala;
        }
      }
    });

    return limitiValori;
  }

  public getValoreScalaFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, nomeTema: string): number {
    let valoreScala: number = 0;

    const stringaDatiTematizzazioni: string = feature.properties.AppIdRate;
    stringaDatiTematizzazioni.split('|').forEach(stringaNomeValore => {
      const nomeValore = stringaNomeValore.split('§');
      let nome: string = nomeValore[0];
      // Controllo la corrispondenza del nome e che il valore sia numerico
      const valore = nomeValore[1]?.trim();
      if (nome === nomeTema && !isNaN(Number(valore))) {
        valoreScala = Number(valore);
      }
    });

    return valoreScala;
  }

  public getColoreScalaFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, layerSelezionatoNome: string, temaSelezionatoNome: string, scalaColori: DatiColoreTema[]): string {
    let coloreTemaFeature = null;
    const nomeTema = this.getNomeTemaTipoLayer(layerSelezionatoNome, temaSelezionatoNome);
    let valoreScalaFeature = this.getValoreScalaFeature(feature, nomeTema);
    const elementoScala = scalaColori.find(elemento => elemento.valoreMax >= valoreScalaFeature && (elemento.valoreMin == null || elemento.valoreMin <= valoreScalaFeature));
    if (elementoScala) {
      if (elementoScala.visible === false) {
        return '#ffffff00';
      }

      const rgbUtils = new RgbUtility();
      coloreTemaFeature = rgbUtils.rgb2hex(
        elementoScala.colore.r,
        elementoScala.colore.g,
        elementoScala.colore.b);
    }

    return coloreTemaFeature;
  }

  public getNomeTemaTipoLayer(layerSelezionatoNome: string, temaSelezionatoNome: string): string {
    const tipoLayerSelezionato = this.getTipoLayerSelezionato();
    if (tipoLayerSelezionato === enum_TipologiaLayer.Avversita ||
      tipoLayerSelezionato === enum_TipologiaLayer.RilieviVegetoProduttivi) {
      return layerSelezionatoNome + ' ' + temaSelezionatoNome;
    } else {
      return temaSelezionatoNome;
    }
  }
}
