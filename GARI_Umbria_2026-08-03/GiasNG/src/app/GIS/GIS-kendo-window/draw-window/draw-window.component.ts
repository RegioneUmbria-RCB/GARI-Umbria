import { Component, Input, OnDestroy, OnInit, Optional, ViewChild } from '@angular/core';
import { GISWindowComponent } from 'app/GIS/GIS-window/GIS-window.component';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { enum_GISDrawingOperations } from '../../GIS-enum/GIS-drawing-operations';
import {FeatureService} from 'app/GIS/services/feature.service';
import { enum_LayerElementiGraficiStd } from '../../GIS-enum/GIS-layer-elementi-grafici';
import { enum_FeatureGeometryType, GISModality } from 'app/GIS/GIS-enum/GIS-feature';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import {combineLatest, debounceTime, filter, map, pairwise, startWith, Subject, takeUntil, tap, withLatestFrom} from 'rxjs';
import { GoogleMapService } from '../../google-map/google-map.service';
import { enum_TipologiaLayer } from "app/GIS/GIS-enum/GIS-tipologia-layer";
import { TranslocoService } from '@jsverse/transloco';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';
import { DrawWindowOperationService } from './draw-window-operation.service';
import { LayerService } from '../../services/layer.service';
import { DrawingManagerService } from '../../services/drawing-manager.service';
import { TreeGisService } from '../../../Utility/Template/kendo-tree/services/tree-gis.service';
import { enum_TreeDataItemType } from '../../../Utility/Template/kendo-tree/enum/tree-dataitem-type';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { isLayerMergeable } from '../polygon-merge-window/tool-polygon-merge/tool-polygon-merge.component';
import { GISAttributiMuzService } from 'app/GIS/GIS-attributi/GIS-attributi-muz-grid/GIS-attributi-muz.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import {enum_Security_Attivita} from '../../../Model/TipiEnumerativi';
import {PermessiUtenteService} from '../../../Service/permessi-utente.service';
import { TreeNode } from 'app/Utility/Template/kendo-tree/model';

export class VisualizzaBottone {
  constructor(
    public pulisciSelezione: boolean =  true,
    public disegnaPoligono: boolean =  true,
    public strumentiDisegnoPunti: boolean =  true,
    public scomponiModificaPunti: boolean =  true,
    public strumentoDisegnoAvanzato: boolean =  true,
    public strumentoUnisci: boolean =  true,
    public modificaImpianto: boolean =  true,
    public nuovoImpiantoAppezzamentoSelezionato: boolean =  true,
    public copiaFeature: boolean =  true,
    public incollaFeature: boolean =  true,
    public eliminaPoligono: boolean =  true,
    public lineeGuidaAB: boolean =  true,
    public datiAgricolturaPrecisone: boolean = true
  ) { }
}

@Component({
  standalone: false,
  selector: 'draw-window',
  templateUrl: './draw-window.component.html',
  styleUrls: ['./draw-window.component.css']
})
export class DrawWindowComponent implements OnInit, OnDestroy {
  @Input() modality: GISModality | undefined = GISModality.Full;

  @ViewChild('window') window: GISWindowComponent;

  // Visibilità bottoni

  // visualizzaBottone: VisualizzaBottone = {
  //   pulisciSelezione: true,
  //   disegnaPoligono: true,
  //   strumentiDisegnoPunti: true,
  //   scomponiModificaPunti: true,
  //   strumentoDisegnoAvanzato: true,
  //   strumentoUnisci: true,
  //   modificaImpianto: true,
  //   nuovoImpiantoAppezzamentoSelezionato: true,
  //   copiaFeature: true,
  //   incollaFeature: true,
  //   eliminaPoligono: true,
  //   lineeGuidaAB: true,
  //   datiAgricolturaPrecisone: true
  // };
  protected  visualizzaBottone: VisualizzaBottone = new VisualizzaBottone();
  GISModality = GISModality;

  //---

  private polygonValidator: any;

  opened: boolean = true;
  windowArgs: WindowArgs;
  GISDrawingOperations = enum_GISDrawingOperations;

  private signal = new Subject<void>();
  private enableDrawingBtnsSignal$: Subject<any> = new Subject<{
    featureSelezionate: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[],
    visualizzazioneTotale: boolean,
    ricetteSelected: boolean
  }>();
  private selectedOperation: enum_GISDrawingOperations;
  private ricette$ = this.ricetteService.selected$.pipe(startWith([]));

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private featureService: FeatureService,
    private sharedDataService: SharedDataService,
    private googleMapService: GoogleMapService,
    private translocoService: TranslocoService,
    private drawWindowOperationService: DrawWindowOperationService,
    private layerService: LayerService,
    private drawingManagerService: DrawingManagerService,
    private treeGisService: TreeGisService,
    private giasDialogService: GiasDialogService,
    private featureInformationService: FeatureInformationService,
    private permessiUtenteService: PermessiUtenteService,
    @Optional() private ricetteService: RicetteService,
    @Optional() private gisAttributiMuzService: GISAttributiMuzService
  ) {
    this.kendoWindowsService.windowToggle$
      .pipe(takeUntil(this.signal)).subscribe(([windowTypes, args]) => {
      if (windowTypes === WindowTypes.DrawWindow) {
        this.windowArgs = args;
      }

      if (windowTypes === WindowTypes.MarkerWindow
        && args.openState == true
        && this.selectedOperation == enum_GISDrawingOperations.marker) {
        this.setOperation(enum_GISDrawingOperations.none);
      }
    });

    this.featureService.getFeatureSelezionate$()
      .pipe(
        withLatestFrom(this.ricette$),
        takeUntil(this.signal)
      ).subscribe(([featureSelezionate, ricette]) => {
      this.enableDrawingBtnsSignal$.next({
        featureSelezionate: featureSelezionate,
        visualizzazioneTotale: null,
        ricetteSelected: ricette.length > 0
      });
    });

    this.sharedDataService.getVisualizzazioneTotale$()
      .pipe(takeUntil(this.signal)).subscribe(visualizzazioneTotale => {
      this.enableDrawingBtnsSignal$.next({
        featureSelezionate: null,
        visualizzazioneTotale: visualizzazioneTotale,
        ricetteSelected: true
      });
    });

    this.sharedDataService.TipoLayerSelezionatoSource
      .pipe(takeUntil(this.signal)).subscribe(TipoLayerSelezionato => {
      if (TipoLayerSelezionato != '') {
        this.enableDrawingBtnsSignal$.next({
          featureSelezionate: null,
          visualizzazioneTotale: null,
          ricetteSelected: true
        });
      }
    });

    this.sharedDataService.getLayerSelezionatoSourceAsObs()
      .pipe(
        withLatestFrom(this.ricette$),
        takeUntil(this.signal)
      ).subscribe(([LayerSelezionato, ricette]) => {
      if (LayerSelezionato !== null) {
        this.drawingManagerService.stopDrawingMode();
        this.enableDrawingBtnsSignal$.next({
          featureSelezionate: null,
          visualizzazioneTotale: null,
          ricetteSelected: ricette.length > 0
        });
      }
    });

    combineLatest([
      this.drawWindowOperationService.operation$,
      this.ricette$
    ])
      .pipe(takeUntil(this.signal))
      .subscribe(([operation, ricette]) => this.doSetOperation(operation, ricette.length > 0));

    this.enableDrawingBtnsSignal$.pipe(
      takeUntil(this.signal),
      debounceTime(500)
    ).subscribe(p => {
      this.abilitaDisabilitaPulsantiDisegno(p.featureSelezionate, p.visualizzazioneTotale, p.ricetteSelected);
    });

    if (this.kendoWindowsService.getOpenState(WindowTypes.DrawWindow)) {
      this.windowArgs = this.kendoWindowsService.getWindowArgs(WindowTypes.DrawWindow);
    }

    this.kendoWindowsService.getWindowArgs$(WindowTypes.AnalisiMappeSatellitariWindow)
      .pipe(
        takeUntil(this.signal),
        map(args => args?.openState ?? false),
        startWith(false),
        pairwise(),
        filter(([oldValue, newValue]) => oldValue !== newValue),
        tap(([_, visible]) => {
          if (visible) this.kendoWindowsService.close(WindowTypes.DrawWindow);
          else this.kendoWindowsService.open(WindowTypes.DrawWindow);
        })
      )
      .subscribe();
  }

  ngOnInit(): void {
    if (this.windowArgs) {
      if (this.modality === GISModality.Trattamento) {
        this.windowArgs.title = this.translocoService.translate("Deseleziona");
      } else {
        this.windowArgs.title = this.translocoService.translate("gis.Disegna");
      }
    }
  }

  ngOnDestroy() {
    this.signal.next();
    this.signal.complete();
  }

  setOperation(operation: enum_GISDrawingOperations): void {
    const isMuzLayer = this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.Muz;
    const defaultPlot = this.gisAttributiMuzService?.isDefaultPlot() ?? false;
    if (isMuzLayer && defaultPlot && (
      operation == enum_GISDrawingOperations.marker ||
      operation == enum_GISDrawingOperations.strumentoDisegnoAvanzato ||
      operation == enum_GISDrawingOperations.polygon
    )) {
      this.giasDialogService.baseInfo('', 'gis.SelezionaUnAppezzamentoDallaSchedaDati', true);
      return;
    }

    const isFabbricatiLayer = this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.Fabbricati;
    const fabbricatoSelected = this.getSelectedFabbricato();
    if (isFabbricatiLayer && fabbricatoSelected == null && operation == enum_GISDrawingOperations.polygon) {
      this.giasDialogService.baseInfo('', 'gis.SelezionaUnFabbricatoDallaSchedaAnagrafica', true);
      return;
    }

    this.drawWindowOperationService.setOperation(operation);
  }

  private doSetOperation(operation: enum_GISDrawingOperations, ricetteSelected: boolean) {
    if (operation === enum_GISDrawingOperations.save) {
      let f = this.featureService.getUltimaFeatureSelezionata();
      if (f?.geometry.type == enum_FeatureGeometryType.Polygon && !this.checkPolygonValidity())
        return;
    }

    this.selectedOperation = operation;
    this.enableDrawingBtnsSignal$.next({
      featureSelezionate: null,
      visualizzazioneTotale: null,
      ricetteSelected: ricetteSelected
    });
  }

  private checkPolygonValidity(): boolean {
    const DrawingManager = require('../../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
    let f = this.featureService.getUltimaFeatureSelezionata();
    const path: google.maps.MVCArray<google.maps.LatLng> = new google.maps.MVCArray<google.maps.LatLng>((<google.maps.Data.LinearRing>(<google.maps.Data.Polygon>this.featureInformationService.getGeometry(f.properties.id)).getArray()[0]).getArray());
    this.polygonValidator = new DrawingManager.PolygonValidator(path);

    if (this.polygonValidator.isValid) {
      return true;
    } else {
      DrawingManager.DisplayErrorPolyLines(this.polygonValidator.intersection, this.googleMapService.googleMapWrapper.googleMap);
      return false;
    }
  }

  private abilitaDisabilitaPulsantiDisegno(
    featureSelezionate: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[],
    visualizzazioneTotale: boolean,
    ricetteSelected: boolean
  ) {

    if (featureSelezionate === null) {
      featureSelezionate = this.featureService.getFeatureSelezionate();
    }

    if (visualizzazioneTotale === null) {
      visualizzazioneTotale = this.sharedDataService.getVisualizzazioneTotale();
    }

    let aggiornaVisibilitaBottoni = new VisualizzaBottone(true, true, true, true, true, true, true, true, true, true, true, true, true);

    const bPermessiPrecision: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Precision_Farming, 2);
    const bPermessiAnagraficaImpianto: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2);
    const bPermessiAnagraficaAppezzamento: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento, 2);

    const bPermessiAnagrafica: boolean = bPermessiAnagraficaImpianto && bPermessiAnagraficaAppezzamento;
    const bTipologiaLayerStandard: boolean = this.layerService.LayerSelected.getValue()?.Option_Value == enum_TipologiaLayer.Entita;

    const cfgSementieri = this.sharedDataService.getCfgSementiAsValue();
    const hasSementieriAuth = !this.isSementiCase() || cfgSementieri.DatiPassaggio.split('|')[4] == '2' || cfgSementieri.SementiMappaturaLibera == 'True';

    switch (true) {

      case visualizzazioneTotale && !this.isSementiCase():
        //--------------------------------------------------------------------------------
        // Visualizzazione totale attiva
        //--------------------------------------------------------------------------------
        // if (this.isSementiCase()) {
        //   this.handleSementiCase(aggiornaVisibilitaBottoni, featureSelezionate);
        // } else {
        aggiornaVisibilitaBottoni.disegnaPoligono = false;
        aggiornaVisibilitaBottoni.strumentiDisegnoPunti = false;
        aggiornaVisibilitaBottoni.scomponiModificaPunti = false;
        aggiornaVisibilitaBottoni.strumentoDisegnoAvanzato = false;
        aggiornaVisibilitaBottoni.strumentoUnisci = false;
        aggiornaVisibilitaBottoni.modificaImpianto = false;
        aggiornaVisibilitaBottoni.nuovoImpiantoAppezzamentoSelezionato = false;
        aggiornaVisibilitaBottoni.copiaFeature = false;
        aggiornaVisibilitaBottoni.incollaFeature = false;
        aggiornaVisibilitaBottoni.eliminaPoligono = false;
        aggiornaVisibilitaBottoni.lineeGuidaAB = false;
        aggiornaVisibilitaBottoni.datiAgricolturaPrecisone = false;
        // }
        break;

      case featureSelezionate.length > 0:
        //--------------------------------------------------------------------------------
        // Esistono feature selezionate
        //--------------------------------------------------------------------------------
        const ultimaFeatureSelezionata = this.featureService.getUltimaFeatureSelezionata();
        const permessiFeature = this.featureService.getPermessiFeature(ultimaFeatureSelezionata);
        const layerAppartenenza = ultimaFeatureSelezionata.properties.layer;

        // Salvo 24-02-2023: il layer CAMPIONAMENTI autorizza il dispaly del btn-disegna se e solo se vi è una feature selezionata
        aggiornaVisibilitaBottoni.disegnaPoligono =
          permessiFeature.inserimento &&
          this.isCampionamenti() &&
          this.isOneSamplingSelected() &&
          hasSementieriAuth;
        aggiornaVisibilitaBottoni.strumentiDisegnoPunti =
          permessiFeature.inserimento &&
          this.isCampionamenti() &&
          this.isOneSamplingSelected() &&
          !this.drawingManagerService.isDrawingModeRunning() &&
          hasSementieriAuth;

        aggiornaVisibilitaBottoni.strumentoDisegnoAvanzato = aggiornaVisibilitaBottoni.strumentiDisegnoPunti && hasSementieriAuth;
        aggiornaVisibilitaBottoni.copiaFeature = bTipologiaLayerStandard && hasSementieriAuth;
        aggiornaVisibilitaBottoni.incollaFeature = bTipologiaLayerStandard && hasSementieriAuth;

        aggiornaVisibilitaBottoni.scomponiModificaPunti = this.isScomponiAvailable(featureSelezionate) && hasSementieriAuth;
        aggiornaVisibilitaBottoni.lineeGuidaAB = bPermessiPrecision && layerAppartenenza === enum_LayerElementiGraficiStd.IMPIANTI && hasSementieriAuth;
        aggiornaVisibilitaBottoni.datiAgricolturaPrecisone = bPermessiPrecision && layerAppartenenza === enum_LayerElementiGraficiStd.IMPIANTI && ricetteSelected && hasSementieriAuth;
        aggiornaVisibilitaBottoni.strumentoUnisci = isLayerMergeable(layerAppartenenza) && hasSementieriAuth;

        // Modifica
        if (permessiFeature.modifica) {
          aggiornaVisibilitaBottoni.modificaImpianto = true && hasSementieriAuth;
          aggiornaVisibilitaBottoni.nuovoImpiantoAppezzamentoSelezionato = (layerAppartenenza === enum_LayerElementiGraficiStd.APPEZZAMENTI) && hasSementieriAuth;
        } else {
          aggiornaVisibilitaBottoni.modificaImpianto = false;
          aggiornaVisibilitaBottoni.nuovoImpiantoAppezzamentoSelezionato = false;
        }
        // Cancellazione
        aggiornaVisibilitaBottoni.eliminaPoligono = permessiFeature.cancellazione && featureSelezionate.length === 1 && hasSementieriAuth;
        // Informazioni

        if (this.selectedOperation == enum_GISDrawingOperations.scomponiModificaPunti) {
          this.kendoWindowsService.close(WindowTypes.DrawWindow);
        }

        break;

      default:
        //--------------------------------------------------------------------------------
        // Nessuna feature selezionata
        //--------------------------------------------------------------------------------
        aggiornaVisibilitaBottoni.scomponiModificaPunti = false;
        aggiornaVisibilitaBottoni.lineeGuidaAB = false;
        aggiornaVisibilitaBottoni.datiAgricolturaPrecisone = false;
        aggiornaVisibilitaBottoni.strumentoDisegnoAvanzato = false;
        aggiornaVisibilitaBottoni.copiaFeature = false;
        aggiornaVisibilitaBottoni.incollaFeature = false;
        aggiornaVisibilitaBottoni.eliminaPoligono = false;
        aggiornaVisibilitaBottoni.nuovoImpiantoAppezzamentoSelezionato = false;
        aggiornaVisibilitaBottoni.modificaImpianto = false;

        let layerSelezionato: TipologiaLayer = this.sharedDataService.getLayerSelezionato();
        let isThereALayerSelected: boolean = true;

        const isLayerImpiantiVisible: boolean =  this.isLayerImpiantiVisibleTest();
        if (layerSelezionato === null) {
          aggiornaVisibilitaBottoni.strumentoDisegnoAvanzato = false;
          layerSelezionato = this.getDefaultLayer();
          isThereALayerSelected = false;
        }

        aggiornaVisibilitaBottoni.strumentoUnisci = isThereALayerSelected && layerSelezionato != null && isLayerMergeable(layerSelezionato.id) && hasSementieriAuth;;

        const permessiLayer = this.sharedDataService.getPermessiLayer(layerSelezionato);
        // Inserimento
        if ( (permessiLayer.inserimento || this.isLayerMuz(layerSelezionato)) && isThereALayerSelected) {
          aggiornaVisibilitaBottoni.disegnaPoligono = true && hasSementieriAuth;
          aggiornaVisibilitaBottoni.strumentiDisegnoPunti = true && hasSementieriAuth;
          aggiornaVisibilitaBottoni.strumentoDisegnoAvanzato = false;
        } else {
          // Salvo 24-02-2023: il layer CAMPIONAMENTI autorizza il dispaly del btn-disegna se e solo se vi è un campionamento selezionato
          const visibileSuLayerNonCampioni: boolean = !isThereALayerSelected && isLayerImpiantiVisible && bPermessiAnagrafica && hasSementieriAuth;
          aggiornaVisibilitaBottoni.disegnaPoligono = this.isCampionamenti() ? this.isOneSamplingSelected() : visibileSuLayerNonCampioni;
          aggiornaVisibilitaBottoni.strumentiDisegnoPunti = this.isCampionamenti() ? this.isOneSamplingSelected() : visibileSuLayerNonCampioni;
        }

        // Le operazioni marker e polygon sono mutualmente esclusive
        if (this.selectedOperation == enum_GISDrawingOperations.marker) {
          this.kendoWindowsService.close(WindowTypes.DrawWindow);
        }

        // if (this.selectedOperation == enum_GISDrawingOperations.polygon) {
        if (this.drawingManagerService.isDrawingModeRunning()) {
          aggiornaVisibilitaBottoni.strumentiDisegnoPunti = true;
        }

        break;

    }

    let tipologiaLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();

    // Salvatore Zammataro 31-01-2023: modifiche aggiunte per permettere di copia-incollare feature quando si tratta di layer standard (Entita)
    if (this.featureService.getFeatureDaCopiare() != undefined && tipologiaLayerSelezionato === enum_TipologiaLayer.Entita) {
      aggiornaVisibilitaBottoni.copiaFeature = true && hasSementieriAuth;
      aggiornaVisibilitaBottoni.incollaFeature = true && hasSementieriAuth;
    }

    this.visualizzaBottone = aggiornaVisibilitaBottoni;

    if (this.window) {
      this.window.setFocus();
    }

  }

  private isLayerAppezzamento(l: TipologiaLayer): boolean {
    return this.isLayerOf(l, enum_LayerElementiGraficiStd.APPEZZAMENTI);
  }

  private isLayerCampi(l: TipologiaLayer): boolean {
    return  this.isLayerOf(l, enum_LayerElementiGraficiStd.CAMPI);
  }

  private isLayerMuz(l: TipologiaLayer): boolean {
    return this.isLayerOf(l, enum_LayerElementiGraficiStd.Muz);
  }

  private isLayerOf(l: TipologiaLayer, t: string): boolean {
    if (l === undefined) {
      return false;
    }
    if (l === null) {
      return false;
    }
    const rval: boolean = (l.id === t);
    return rval;

  }

  private isLayerImpiantiVisibleTest(): boolean {
    const  ll: TipologiaLayer[] = this.sharedDataService.getTipologiaLayer();
    let found: boolean = false;
    for (const l of ll) {
      if (l.id === '19' && l.flagvisibile.toLowerCase() === '1') {
        found = true;
      }
    }
    return found;
  }

  private isCampionamenti() {
    return this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.CAMPIONAMENTI;
  }

  private isOneSamplingSelected(): boolean {
    return this.areSamplingNodesSelected() && this.sharedDataService.getTreeViewCheckedKeys().length == 1;
  }

  private areSamplingNodesSelected(): boolean {
    let checkedKeys = this.sharedDataService.getTreeViewCheckedKeys();

    if (checkedKeys.length) {
      for (let key of checkedKeys) {
        let node = this.treeGisService.getTreeNodeFromCheckedKey(key)
        if (node.type == enum_TreeDataItemType.Campionamenti) return true;
      }
      return false;
    }

    return false;
  }

  private getSelectedFabbricato(): TreeNode | null {
    const checkedKeys = this.sharedDataService.getTreeViewCheckedKeys();

    for (const key of checkedKeys) {
      const node = this.treeGisService.getTreeNodeFromCheckedKey(key);
      if (node.type == enum_TreeDataItemType.Fabbricati) {
        return node;
      }
    }
    return null;
  }

  private isScomponiAvailable(featureSelezionate: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): boolean {
    return featureSelezionate.length === 1 &&                                                           // Only one feature selected
      featureSelezionate[0].properties.TipologiaGML == enum_FeatureGeometryType.Polygon &&        // The selected feature must be a Polygon
      this.sharedDataService.getTipologiaLayerById(enum_LayerElementiGraficiStd.IMPIANTI) != null; // We must have the impianti layer
  }

  private getDefaultLayer(): TipologiaLayer {

    let layerSelezionato = null;

    const tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();

    if (tipoLayerSelezionato !== '') {

      if (tipoLayerSelezionato === enum_TipologiaLayer.Entita) {

        // Se tipo layer standard, assumo che il layer selezionato sia un impianto
        layerSelezionato = this.sharedDataService.getTipologiaLayerById(enum_LayerElementiGraficiStd.IMPIANTI);

      } else {

        // In caso contrario, prendo il primo layer non fisso
        layerSelezionato = this.sharedDataService.getTipologiaLayerFiltered(true, true)[0];

      }

    }

    return layerSelezionato;

  }

  private isSementiCase(): boolean {
    return this.sharedDataService.getCfgSementiAsValue() != undefined &&
      (
        this.layerService.LayerSelected.getValue()?.Option_Value == enum_TipologiaLayer.OrganizzazioneAppartenenza ||
        this.layerService.LayerSelected.getValue()?.Option_Value == enum_TipologiaLayer.GruppoColturale ||
        this.layerService.LayerSelected.getValue()?.Option_Value == enum_TipologiaLayer.Cultivar ||
        this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.IMPIANTI ||
        this.layerService.layerItemSelected[0]?.id == enum_LayerElementiGraficiStd.APPEZZAMENTI ||
        (
          this.layerService.LayerSelected.getValue()?.Option_Value == enum_TipologiaLayer.Entita &&
          this.layerService.layerItemSelected[0] == undefined
        )
      );
  }
}
