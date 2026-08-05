import { Inject, Injectable, Optional } from '@angular/core';
import { TreeItem, TreeItemLookup } from '@progress/kendo-angular-treeview';
import { BehaviorSubject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { TreeNode, UpdateTree} from '../model';
import { TreeContainerService } from './tree-container.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {LOADING_TOKEN, LoadingService, ObjParametriAgenda} from 'gias-ui-kit';
import { GisTreeFilters } from '../filters/gis-tree-filters.component';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { ChiaveAlbero, ConfigurazioneAlbero } from 'app/Service/api.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { enum_TipoNodo } from 'app/Model/TipiEnumerativi';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { TranslocoService } from '@jsverse/transloco';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { enum_OrigineChiamataLoadGeoJson } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { consoleLogDebugMultiParam, consoleLogDebugParam, separatoreChiaveAlbero } from 'app/Service/utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { enum_TreeDataItemType } from '../enum/tree-dataitem-type';
import {MasterService} from '../../../../Service/master.service';
import {AjaxAgronicaService} from '../../../../Service/ajax-agronica.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { SementieriService } from 'app/Service/sementieri.service';
import {ExportCartographyDataService} from '../../../../GIS/GIS-toolbar/services/export-cartography-data.service';

const getNodesLink = 'AgronicaControlli_2010/GetNodesAlberoAnagrafeNG';

export enum enum_livelloGerarchicoAlbero {
  Indefinito = 0,
  Utente = 1,
  Impresa = 2,
  Centro = 3,
  Anagrafica = 4,
  Campo = 5,
  Appezzamento = 6,
  Impianto = 7
}

export enum enum_TipoRicerca {
  ChiaveAlbero = 1,
  TreeNode = 2,
  TreeNodeAndParent = 3
}

export class FiltroCatasto {
  Provincia: string;
  Comune: string;
  Foglio: string;
  Particella: string;
}

export class objTreeNode {
  treeNode: TreeNode;
  treeNodeParent: TreeNode;
}

@Injectable()
export class TreeGisService extends BehaviorSubject<TreeNode[]> {
  // Filtro albero
  filterTerm = '';

  // Filtro catasto
  filtroCatasto: FiltroCatasto = {
    Provincia: '',
    Comune: '',
    Foglio: '',
    Particella: '',
  };

  highlightItems: BehaviorSubject<UpdateTree> = new BehaviorSubject(null);

  private checkedKeysSelected: {checkedKey: string, featureIndex: number}[] = [];
  private gisTreeFilters: GisTreeFilters;
  private inputCheckedKey: string;
  private chiaveAlberoCheckedKey: string;
  private treeNodeCheckedKey: TreeNode;
  private objTreeNodeCheckedKey: objTreeNode;
  private tipoRicerca: enum_TipoRicerca;
  private inputChiaveAlbero: string;
  private inputNodo: TreeNode;
  private nodoAggiunto: boolean;
  private nodoModificato: boolean;
  private nodoTrovato: boolean;
  private inputObjChiaveAlbero: ChiaveAlbero;
  private inputChiaveAlberoSenzaTipoNodo: string;
  private objCheckPadriAlbero: {
    padreCompatibile: boolean;
    aggiungiNodo: boolean
  }

  private treeFiltersOld: {
    piva: string;
    saCod: string;
    dataInizio: Date;
    dataFine: Date;
  } = {
    piva: "",
    saCod: "",
    dataInizio: AGRODATAINIZIO,
    dataFine: AGRODATAFINE
  };

  private readonly prefissoLog = '###';
  private readonly logAggiungiNodo = false;

  constructor(
    private drawer$: TreeContainerService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private treeContainer: TreeContainerService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService,
    private funzioniComuniService: FunzioniComuniService,
    private masterService: MasterService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private sementieriService: SementieriService,
    private exportCartographyDataService: ExportCartographyDataService,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService,
    @Optional() private sharedDataService?: SharedDataService,
    @Optional() private googleMapGeoJsonService?: GoogleMapGeoJsonService,
    @Optional() private featureService?: FeatureService
  ) {
    super(null);
  }

  public watchObjParametriAgenda(signal) {
    return this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntil(signal));
  }

  public loadDataInternalFilters(forceLoad: boolean){
    if (this.gisTreeFilters) {
      this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
      this.loadData(this.gisTreeFilters,forceLoad);
    }
  }

  public setSelectedImpresaChangedSoUpdateTree() {
    this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
  }

  public loadData(filters: GisTreeFilters, forceLoad?: boolean) {
    this.gisTreeFilters = filters;

    if(!this.treeContainer.selectedImpresaChangedSoUpdateTree) {
      return;
    }

    if(forceLoad === undefined){
      forceLoad = false;
    }

    if (!forceLoad && !this.checkReadTree()) {
      return;
    }

    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.drawer$.drawerRef});

    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(getNodesLink, this.getParams(filters))
      .pipe(map((data) => {
        this.treeContainer.selectedImpresaChangedSoUpdateTree = false;
        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.drawer$.drawerRef})
        const nodes: TreeNode[]= data.RispostaStringa;
        super.next(nodes);
      })).subscribe();
  }

  private getParams(filters: GisTreeFilters) {
    const agenda = this.objParametriAgendaService.getObjParamValue();

    let cfg: ConfigurazioneAlbero = this.sharedDataService.getCfgAlberoGisUtente()[0].CfgAlbero;

    // Piva & SaCod
    cfg.Piva = agenda.Piva;
    cfg.Sa_Cod = filters.centro.id;

    consoleLogDebugMultiParam(
      enum_logDebugArea.App,
      enum_logDebugTipo.CambioAzienda,
      this.constructor.name,
      'loadData',
      [cfg.Piva,cfg.Sa_Cod]
    );

    // Forzatura flag agenda
    cfg.Flag_Agenda = false;

    // Filtro temporale
    const filtroTemporale = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporalePeriodo;
    cfg.dataInizio = filtroTemporale.DataInizio;
    cfg.dataFine = filtroTemporale.DataFine;

    if (this.sementieriService.isSementieriSportello()) {
      cfg.FlagModalitaSementieri = true;
      cfg.DatiSportelloSementieri = this.sharedDataService.getCfgSementiAsValue()?.Sementi;
    }

    return {
      cfgSerialized: JSON.stringify(cfg),
      id: '0',
      PathRoot: '',
      objP_server: this.masterService.ObjParametri_Server,
      objP_utenti: this.masterService.ObjParametri_Utenti
    };
  }

  private checkReadTree(): boolean {
    let readTree: boolean = true;

    const agenda = this.objParametriAgendaService.getObjParamValue();
    const filtroTemporale = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporalePeriodo;

    if (this.treeFiltersOld.piva === agenda.Piva &&
      this.treeFiltersOld.saCod === this.gisTreeFilters.centro.id &&
      this.treeFiltersOld.dataInizio === filtroTemporale.DataInizio &&
      this.treeFiltersOld.dataFine === filtroTemporale.DataFine) {
      readTree = false;
    }

    this.treeFiltersOld.piva = agenda.Piva;
    this.treeFiltersOld.saCod = this.gisTreeFilters.centro.id;
    this.treeFiltersOld.dataInizio = filtroTemporale.DataInizio;
    this.treeFiltersOld.dataFine = filtroTemporale.DataFine;

    return readTree;
  }

  /**
   * Determines update behavior of the grid.
   * This could be using query parameters (if redirecting to a different page)
   * or using next on @selected subject if updating the same page.
   * @param e the clicked tree item.
   */
  public handleSelection(e: TreeItem, checkedKeys: any[]): any[] {
    consoleLogDebugParam(
      enum_logDebugArea.Gis,
      enum_logDebugTipo.SelezioneFeatureNodo,
      this.constructor.name,
      'nodoSelezionato',
      e.dataItem);

    let checkedKeysOut = checkedKeys.slice();
    let index = this.getIndexCheckedTreeItem(e, checkedKeysOut);
    let isChecked: boolean = null;
    if (index >= 0) {
      checkedKeysOut.splice(index);
      isChecked = false;
    } else {
      checkedKeysOut = [];
      checkedKeysOut.push(e.index);
      isChecked = true;
    }
    this.googleMapGeoJsonService.selezionaFeatureByChiaveAlbero(e.dataItem.id, isChecked, false);
    return checkedKeysOut;
  }

  public handleCheck(e: TreeItemLookup, checkedKeys: any[]) {
    let isChecked: boolean = (this.getIndexCheckedTreeItem(e.item, checkedKeys)) >= 0;
    this.googleMapGeoJsonService.selezionaFeatureByChiaveAlbero(e.item.dataItem.id, isChecked, true);

    if (isChecked) {
      this.handleChecked(checkedKeys);
    } else {
      this.handleUnchecked(checkedKeys);
    }
  }

  getIndexCheckedTreeItem(e: TreeItem, checkedKeys: any[]): number {
    const indexClicked = e.index;
    return checkedKeys.findIndex(element => element === indexClicked);
  }

  getChiaveAlberoFromCheckedKey(checkedKey: string): string {
    this.tipoRicerca = enum_TipoRicerca.ChiaveAlbero;
    this.inputCheckedKey = checkedKey;
    this.chiaveAlberoCheckedKey = "";
    this.leggiAlberoRicerca();
    return this.chiaveAlberoCheckedKey;
  }

  getTreeNodeFromCheckedKey(checkedKey: string): TreeNode {
    this.tipoRicerca = enum_TipoRicerca.TreeNode;
    this.inputCheckedKey = checkedKey;
    this.treeNodeCheckedKey = null;
    this.leggiAlberoRicerca();
    return this.treeNodeCheckedKey;
  }

  getTreeNodeAndParentFromCheckedKey(checkedKey: string): objTreeNode {
    this.tipoRicerca = enum_TipoRicerca.TreeNodeAndParent;
    this.inputCheckedKey = checkedKey;
    this.objTreeNodeCheckedKey = {
      treeNode: this.funzioniComuniService.getNewTreeNode(),
      treeNodeParent: this.funzioniComuniService.getNewTreeNode()
    };
    this.leggiAlberoRicerca();

    return this.objTreeNodeCheckedKey;
  }

  private leggiAlberoRicerca(): void {
    const nodiRadice: TreeNode[] = super.getValue();
    this.nodoTrovato = false;
    for (let indiceRadice = 0; indiceRadice < nodiRadice?.length; indiceRadice++) {
      this.leggiAlberoRicorsivoRicerca(nodiRadice[indiceRadice], indiceRadice, 0);
      if (this.nodoTrovato) {
        break;
      }
    }
  }

  private leggiAlberoRicorsivoRicerca(
    nodo: any,
    indice: number,
    livello: number,
    treeKey?: number[],
    nodoPadre?: any
  ): void {
    if (nodoPadre === undefined) {
      nodoPadre = null;
    }
    treeKey = this.updateTreeKey(treeKey, livello, indice);
    let currentTreeKey = this.getCurrentTreeKey(treeKey, livello);
    //
    consoleLogDebugParam(
      enum_logDebugArea.Gis,
      enum_logDebugTipo.LetturaAlbero,
      this.constructor.name,
      'currentTreeKey',
      nodo.text
    );
    //
    if (currentTreeKey === this.inputCheckedKey){
      this.nodoTrovato = true;
      switch (this.tipoRicerca) {
        case enum_TipoRicerca.ChiaveAlbero:
          this.chiaveAlberoCheckedKey = nodo.id;
          break;
        case enum_TipoRicerca.TreeNode:
          this.treeNodeCheckedKey = <TreeNode> nodo;
          break;
        case enum_TipoRicerca.TreeNodeAndParent:
          this.objTreeNodeCheckedKey.treeNode = <TreeNode> nodo;
          this.objTreeNodeCheckedKey.treeNodeParent = <TreeNode> nodoPadre;
          break;
      }
      return;
    }
    if (nodo.items !== null && nodo.items.length > 0) {
      for (let indiceFiglio = 0; indiceFiglio < nodo.items.length; indiceFiglio++) {
        this.leggiAlberoRicorsivoRicerca(
          nodo.items[indiceFiglio],
          indiceFiglio,
          livello + 1,
          treeKey,
          nodo
        );
        if (this.nodoTrovato) {
          break;
        }
      };
    }
  }

  private updateTreeKey(
    treeKey: number[],
    livello: number,
    indice: number
  ): number[] {
    if (treeKey === undefined) {
      treeKey = [];
    }
    treeKey[livello] = indice;
    return treeKey;
  }

  private getCurrentTreeKey(
    treeKey: number[],
    livello: number
  ): string {
    return treeKey.slice(0,livello + 1).join('_');
  }

  parseItemIndex(index: string) {
    const parts = index.split('_');
    const result = [];
    for(let i = 0; i < parts.length; ++i) {
      if(i === 0) {
        result.push(parts[i]);
      } else {
        result.push(result[i-1] + '_' + parts[i]);
      }
    }

    this.highlightItems.next({selected: [index], expanded: result});
  }

  public getCheckedKeysFromFeature(elencoChiaviAlberoFeatureCompleteSelezionate: string[]): string[] {
    let checkedKeysSelectedOut: string[] = [];
    const nodiRadice: TreeNode[] = super.getValue();

    if (nodiRadice) {
      this.checkedKeysSelected = [];

      nodiRadice.forEach( (nodoRadice, indice) => {
        this.leggiAlberoRicorsivo(elencoChiaviAlberoFeatureCompleteSelezionate, nodoRadice, indice, 0);
      });

      const checkedKeysOrdered = this.checkedKeysSelected.sort((a,b) => (a.featureIndex > b.featureIndex) ? 1 : ((b.featureIndex > a.featureIndex) ? -1 : 0))

      checkedKeysOrdered.forEach( (element, index) => {
        checkedKeysSelectedOut.push(element.checkedKey);
        if (this.featureService && index === checkedKeysOrdered.length - 1) {
          const feature = this.featureService.getFeatureSelezionate()[element.featureIndex];
          const featureId = feature.properties.id;
          this.sharedDataService.setLastCheckedKeyFeatureId(featureId);
        }
      });
    }

    return checkedKeysSelectedOut;
  }

  private leggiAlberoRicorsivo(
    chiaviAlberoSelezionate: string[],
    nodo: any,
    indice: number,
    livello: number,
    treeKey?: number[]
  ) {
    treeKey = this.updateTreeKey(treeKey, livello, indice);
    let currentTreeKey = this.getCurrentTreeKey(treeKey, livello);
    // console.log(`--------------------------------------------------------------------------------`);
    // console.log(`Livello: ${livello}, Indice: ${indice}, TreeKey: ${currentTreeKey}`);
    // console.log(`Testo: ${nodo.text}`);
    // console.log(`Chiave: ${nodo.id}`);
    const chiaveAlbero = nodo.id;
    chiaviAlberoSelezionate.forEach( (chiaveAlberoFeatureSelezionata, indiceFeature) => {
      if (chiaveAlberoFeatureSelezionata === chiaveAlbero) {
        this.checkedKeysSelected.push({checkedKey: currentTreeKey, featureIndex: indiceFeature});
        // console.log(`selezionata: ${currentTreeKey}`)
      }
    });
    if (nodo.items !== null && nodo.items.length > 0) {
      nodo.items.forEach( (nodoFiglio, indiceFiglio) => {
        this.leggiAlberoRicorsivo(chiaviAlberoSelezionate, nodoFiglio, indiceFiglio, livello + 1, treeKey);
      });
    }
  }

  ricaricaFeatureRiletturaAlbero(){
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.RiletturaAlbero);
    this.googleMapGeoJsonService.loadGeoJsonBase(true);
  }

  ricaricaFeatureAziendaCorrente() {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.RipristinoVisualizzazioneTotale);
    this.googleMapGeoJsonService.loadGeoJsonForzato(false);
  }

  ricaricaFeatureDaAlbero(impostaCentroMappa: boolean) {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.RicaricaFeatureDaAlbero);
    this.googleMapGeoJsonService.loadGeoJsonForzato(impostaCentroMappa);
  }

  cancellaNodoAlbero(chiaveAlbero: string) {
    this.inputChiaveAlbero = chiaveAlbero;
    const nodiRadice: TreeNode[] = super.getValue();
    if (nodiRadice) {
      nodiRadice.forEach( (nodoRadice, indice) => {
        this.leggiAlberoRicorsivoCancella(nodoRadice, indice, 0);
      });
      // Qui sopra viene cancellato solo il nodo.
      // La selezione viene aggiornata tramite "featureSelezionateSource.subscribe"
      // nel sorgente "tree.component.ts" al momento della cancellazione effettiva
      // della feature.
    }
  }

  private leggiAlberoRicorsivoCancella(
    nodo: TreeNode,
    indice: number,
    livello: number,
    treeKey?: number[]
  ) {
    treeKey = this.updateTreeKey(treeKey, livello, indice);
    if (nodo.items !== null && nodo.items.length > 0) {
      let nodiDaCancellare: number[] = [];
      nodo.items.forEach( (nodoFiglio, indiceFiglio) => {
        if (nodoFiglio.id === this.inputChiaveAlbero) {
          nodiDaCancellare.push(indiceFiglio);
        } else {
          this.leggiAlberoRicorsivoCancella(nodoFiglio, indiceFiglio, livello + 1, treeKey);
        }
      });
      nodiDaCancellare.forEach( indiceNodo => {
        nodo.items.splice(indiceNodo,1);
      });
    }
  }

  aggiungiNodoAlbero(nodo: TreeNode) {
    this.inputNodo = nodo;

    this.logNodoDaAggiungere(nodo);

    this.inputObjChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(nodo.id);
    this.nodoAggiunto = false;
    let nodiRadice: TreeNode[] = super.getValue();
    if (nodiRadice) {
      nodiRadice.forEach( (nodoRadice, indice) => {
        this.leggiAlberoRicorsivoAggiungi(nodoRadice, indice, 0);
      });
      if (this.nodoAggiunto) {
        let nodiRadiceNew = [...nodiRadice];
        super.next(nodiRadiceNew);
      } else {
        const messaggioErrore = this.translocoService.translate("gis.ErroreInserimentoNodoAlbero");
        this.giasMessageService.errorMessage(messaggioErrore);
      }
    }
  }

  private logNodoDaAggiungere(nodo: TreeNode) {
    if (this.logAggiungiNodo) {
      console.log('='.repeat(80));
      console.log('Nodo da aggiungere:',nodo.type);
      console.log('='.repeat(80));
      console.log(nodo.text);
      console.log(nodo.id);
      console.log('-'.repeat(80));
    }
  };

  private leggiAlberoRicorsivoAggiungi(
    nodo: TreeNode,
    indice: number,
    livello: number,
    treeKey?: number[]
  ) {

    if (this.nodoAggiunto) {
      return;
    }

    let objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(nodo.id);
    this.checkPadreCompatibileTipoNodo(objChiaveAlbero);

    if (!this.objCheckPadriAlbero.padreCompatibile) {
      return;
    }

    this.logNodoCorrente(livello,nodo);

    if (this.objCheckPadriAlbero.aggiungiNodo) {
      this.logNodoInserito(livello);
      this.inizializzaNodoItems(nodo);
      nodo.items.push(this.inputNodo);
      this.nodoAggiunto = true;
      return;
    }

    treeKey = this.updateTreeKey(treeKey, livello, indice);
    this.seInserisciNodoAnagrafica(objChiaveAlbero, nodo);

    if (nodo.items !== null && nodo.items.length > 0) {
      nodo.items.forEach( (nodoFiglio, indiceFiglio) => {
        this.leggiAlberoRicorsivoAggiungi(nodoFiglio, indiceFiglio, livello + 1, treeKey);
      });
    }
  }

  private logNodoCorrente(livello: number, nodo: TreeNode) {
    if (this.logAggiungiNodo) {
      console.log(this.prefissoLog,livello,nodo.text);
      if (nodo.id) {
        console.log(this.prefissoLog,
          ' '.repeat(livello.toString.length),
          nodo.id);
      }
    }
  }

  private logNodoInserito(livello: number) {
    if (this.logAggiungiNodo) {
      console.log(this.prefissoLog,
        ' '.repeat(livello.toString.length),
        '---> Inserito nodo');
    }
  }

  private inizializzaNodoItems(nodo: TreeNode) {
    if (nodo.items === null) {
      nodo.items = [];
    }
  }

  private seInserisciNodoAnagrafica(
    objChiaveAlbero: ChiaveAlbero,
    nodo: TreeNode
  ) {
    // Previsto inserimento nodo di tipo Anagrafica se non esiste per il centro.
    // Si verifica quando ancora non esistono campi e/o appezzamenti per il centro.
    if (objChiaveAlbero.TipoNodo === enum_TipoNodo.Centro) {
      this.inizializzaNodoItems(nodo);
      let nodoAnagrafica = undefined;
      if (nodo.items.length > 0) {
        nodoAnagrafica = nodo.items.find((obj)=> {
          return obj.type === enum_TreeDataItemType.Anagrafica;
        });
      }
      if (nodoAnagrafica === undefined) {
        let nuovoNodoAnagrafica = this.funzioniComuniService.getNewTreeNode();
        nuovoNodoAnagrafica.text = this.translocoService.translate("Anagrafica");
        nuovoNodoAnagrafica.type = enum_TreeDataItemType.Anagrafica;
        nuovoNodoAnagrafica.id = enum_TipoNodo.Anagrafica_Generica.toString();
        nodo.items.push(nuovoNodoAnagrafica);
      }
    }
  }

  private checkPadreCompatibileTipoNodo(objChiaveAlbero: ChiaveAlbero) {
    this.objCheckPadriAlbero = {
      padreCompatibile: false,
      aggiungiNodo: false
    };

    switch (true) {

      case this.inputObjChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento:
      case this.funzioniComuniService.isTipoNodoImpianto(this.inputObjChiaveAlbero.TipoNodo):
        this.checkGerarchiaPadri(objChiaveAlbero);
        break;

    }

    this.logCheckPadreCompatibile(objChiaveAlbero);
  }

  private logCheckPadreCompatibile(objChiaveAlbero: ChiaveAlbero) {
    if (this.logAggiungiNodo) {
      console.log(
        this.prefissoLog,
        'check',
        '- tipoNodo :', objChiaveAlbero.TipoNodo,
        '- padreCompatibile :', this.objCheckPadriAlbero.padreCompatibile
      );
    }
  };

  private checkGerarchiaPadri(objChiaveAlbero: ChiaveAlbero) {
    let padreCompatibile = false;
    let livelloNodo = enum_livelloGerarchicoAlbero.Indefinito;
    let livelloControllo = enum_livelloGerarchicoAlbero.Indefinito;

    if (this.inputObjChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento) {
      // Appezzamento
      if (this.inputObjChiaveAlbero.Campo_Cod === 0) {
        livelloControllo = enum_livelloGerarchicoAlbero.Anagrafica;
      } else {
        livelloControllo = enum_livelloGerarchicoAlbero.Campo;
      }
    } else {
      // Impianto
      livelloControllo = enum_livelloGerarchicoAlbero.Appezzamento;
    }

    switch (true) {
      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Anagrafica_Generica:
        livelloNodo = enum_livelloGerarchicoAlbero.Anagrafica;
        padreCompatibile = true;
        break;

      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Utente:
        livelloNodo = enum_livelloGerarchicoAlbero.Utente;
        padreCompatibile = true;
        break;

      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Impresa:
        livelloNodo = enum_livelloGerarchicoAlbero.Impresa;
        if (livelloControllo < enum_livelloGerarchicoAlbero.Impresa) {
          padreCompatibile = false;
          break;
        }
        if (this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva) {
          padreCompatibile = true;
        }
        break;

      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Centro:
        livelloNodo = enum_livelloGerarchicoAlbero.Centro;
        if (livelloControllo < enum_livelloGerarchicoAlbero.Centro) {
          padreCompatibile = false;
          break;
        }
        if (this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva &&
          this.inputObjChiaveAlbero.Sa_Cod === objChiaveAlbero.Sa_Cod) {
          padreCompatibile = true;
        }
        break;

      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Campo:
        livelloNodo = enum_livelloGerarchicoAlbero.Campo;
        if (livelloControllo < enum_livelloGerarchicoAlbero.Campo) {
          padreCompatibile = false;
          break;
        }
        if (this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva &&
          this.inputObjChiaveAlbero.Sa_Cod === objChiaveAlbero.Sa_Cod &&
          this.inputObjChiaveAlbero.Campo_Cod === objChiaveAlbero.Campo_Cod) {
          padreCompatibile = true;
        }
        break;

      case objChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento:
        livelloNodo = enum_livelloGerarchicoAlbero.Appezzamento;
        if (livelloControllo < enum_livelloGerarchicoAlbero.Appezzamento) {
          padreCompatibile = false;
          break;
        }
        if (this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva &&
          this.inputObjChiaveAlbero.Sa_Cod === objChiaveAlbero.Sa_Cod &&
          this.inputObjChiaveAlbero.Campo_Cod === objChiaveAlbero.Campo_Cod &&
          this.inputObjChiaveAlbero.Appezza === objChiaveAlbero.Appezza) {
          padreCompatibile = true;
        }
        break;
    }

    if (padreCompatibile && livelloNodo === livelloControllo) {
      this.objCheckPadriAlbero.aggiungiNodo = true;
    }

    this.objCheckPadriAlbero.padreCompatibile = padreCompatibile;
  }

  checkAggiungiNodo(chiaveAlbero: string): boolean {
    let aggiungiNodo = false;
    let objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);

    if (this.inputObjChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento) {
      if (this.inputObjChiaveAlbero.TipoNodo === objChiaveAlbero.TipoNodo &&
        this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva &&
        this.inputObjChiaveAlbero.Sa_Cod === objChiaveAlbero.Sa_Cod &&
        this.inputObjChiaveAlbero.Campo_Cod === objChiaveAlbero.Campo_Cod) {
        aggiungiNodo = true;
      }
    }

    if (this.funzioniComuniService.isTipoNodoImpianto(this.inputObjChiaveAlbero.TipoNodo)) {
      if (this.inputObjChiaveAlbero.TipoNodo === objChiaveAlbero.TipoNodo &&
        this.inputObjChiaveAlbero.Piva === objChiaveAlbero.Piva &&
        this.inputObjChiaveAlbero.Sa_Cod === objChiaveAlbero.Sa_Cod &&
        this.inputObjChiaveAlbero.Campo_Cod === objChiaveAlbero.Campo_Cod &&
        this.inputObjChiaveAlbero.Appezza === objChiaveAlbero.Appezza) {
        aggiungiNodo = true;
      }
    }

    return aggiungiNodo;
  }

  modificaNodoAlbero(nodo: TreeNode) {
    this.inputNodo = nodo;
    this.inputObjChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(nodo.id);
    this.inputChiaveAlberoSenzaTipoNodo = this.getChiaveAlberoSenzaTipoNodo(nodo.id);
    this.nodoModificato = false;
    this.nodoTrovato = false;
    let nodiRadice: TreeNode[] = super.getValue();

    if (nodiRadice) {
      nodiRadice.forEach( (nodoRadice, indice) => {
        this.leggiAlberoRicorsivoModifica(nodoRadice, indice, 0);
      });
      if (this.nodoModificato) {
        let nodiRadiceNew = [...nodiRadice];
        super.next(nodiRadiceNew);
      }
      if (!this.nodoTrovato) {
        const messaggioErrore = this.translocoService.translate("gis.ErroreModificaNodoAlbero");
        this.giasMessageService.errorMessage(messaggioErrore);
      }
    }
  }

  private leggiAlberoRicorsivoModifica(
    nodo: TreeNode,
    indice: number,
    livello: number,
    treeKey?: number[]
  ) {
    if (this.nodoTrovato) {
      return;
    }

    if (this.isNodoDaModificare(nodo.id)) {
      this.nodoTrovato = true;
      if (nodo.id !== this.inputNodo.id || nodo.text !== this.inputNodo.text) {
        nodo.id = this.inputNodo.id;
        nodo.text = this.inputNodo.text;
        nodo.startDate = this.inputNodo.startDate;
        nodo.endDate = this.inputNodo.endDate;
        this.nodoModificato = true;
      }
      return;
    }

    treeKey = this.updateTreeKey(treeKey, livello, indice);
    if (nodo.items !== null && nodo.items.length > 0) {
      nodo.items.forEach( (nodoFiglio, indiceFiglio) => {
        this.leggiAlberoRicorsivoModifica(nodoFiglio, indiceFiglio, livello + 1, treeKey);
      });
    }
  }

  private isNodoDaModificare(chiaveAlberoCorrente: string): boolean {
    let nodoDaModificare: boolean = false;

    let nodiImpianto: boolean = false;
    if (this.funzioniComuniService.isTipoNodoImpianto(this.inputObjChiaveAlbero.TipoNodo)) {
      const chiaveAlberoCorrenteObj = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlberoCorrente);
      if (this.funzioniComuniService.isTipoNodoImpianto(chiaveAlberoCorrenteObj.TipoNodo)) {
        nodiImpianto = true;
      }
    }

    if (nodiImpianto) {
      const chiaveAlberoCorrenteSenzaTipoNodo = this.getChiaveAlberoSenzaTipoNodo(chiaveAlberoCorrente);
      nodoDaModificare = chiaveAlberoCorrenteSenzaTipoNodo === this.inputChiaveAlberoSenzaTipoNodo;
    } else {
      nodoDaModificare = chiaveAlberoCorrente === this.inputNodo.id;
    }

    return nodoDaModificare;
  }

  private getChiaveAlberoSenzaTipoNodo(chiaveAlbero): string {
    const chiaveAlberoArray: string[] = chiaveAlbero.split(separatoreChiaveAlbero);
    return chiaveAlberoArray.slice(1).join(separatoreChiaveAlbero);
  }

  // Used to set ObjParametriAgenda.Sa_Cod to the last Sa_Cod selected in gis tree
  private handleChecked(checkedKeys: any[]): void {
    const node: TreeNode = this.getTreeNodeFromCheckedKey(checkedKeys.pop());
    const treeKey: ChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(node.id);

    if (treeKey.TipoNodo === enum_TipoNodo.Centro) {
      this.exportCartographyDataService.saCod = treeKey.Sa_Cod;
    }
  }

  // Used to set ObjParametriAgenda.Sa_Cod to another selected Sa_Cod in gis tree
  private handleUnchecked(checkedKeys: string[]): void {
    const key: string = checkedKeys.find(k => {
      const treeKey: ChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(this.getTreeNodeFromCheckedKey(k).id);
      return treeKey.TipoNodo === enum_TipoNodo.Centro;
    });

    if (key != undefined) {
      const centerKey: ChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(this.getTreeNodeFromCheckedKey(key).id);
      this.exportCartographyDataService.saCod = centerKey.Sa_Cod;
    } else {
      this.exportCartographyDataService.saCod = 0;
    }
  }
}
