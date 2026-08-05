import { Component, Input, OnDestroy, OnInit, ViewEncapsulation, Optional, ElementRef, ViewChild,
         AfterViewInit, OnChanges, SimpleChanges, AfterViewChecked, AfterContentChecked } from '@angular/core';
import {
  faCompressAlt, faExpand, faExpandAlt, faGripHorizontal, faHome, faIndustry, faLayerGroup, faLeaf, faMap, faObjectGroup,
         faTruckPickup, faUser, faUsers, faWarehouse } from '@fortawesome/free-solid-svg-icons';
import { CheckableSettings, FilterExpandSettings, FilterState, MatcherFunction, TreeItem, TreeItemLookup, TreeViewFilterSettings } from '@progress/kendo-angular-treeview';
import { Observable, of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TreeNode } from '../model';
import { TreeService } from '../services/tree-anagrafica.service';
import { TreeGisService } from '../services/tree-gis.service';
import { TreeContainerService } from '../services/tree-container.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { enum_OrigineChiamata } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { enum_FilterMode, enum_FilterOperator } from '../enum/tree-view-filter-settings';
import { separatoreChiaveAlbero, separatoreFiltroAlbero } from 'app/Service/utils';
import { enum_TipoNodo } from 'app/Model/TipiEnumerativi';
import {AnagraficaService} from '../../../../anagrafica/anagrafica.service';
import {LayerService} from '../../../../GIS/services/layer.service';
import {enum_LayerElementiGraficiStd} from '../../../../GIS/GIS-enum/GIS-layer-elementi-grafici';
import {enum_TreeDataItemType} from '../enum/tree-dataitem-type';
import {ConversionService} from "../../../../Service/conversion.service";

class FiltroCatasto {
    Provincia: string;
    Comune: string;
    Foglio: string;
    Particella: string;

    constructor(
        provincia: string,
        comune: string,
        foglio: string,
        particella: string
    ) {
        this.Provincia = provincia;
        this.Comune = comune;
        this.Foglio = foglio;
        this.Particella = particella;
    }

    ProvinciaApplicabile(): boolean {
        return this.Provincia.length >= 1;
    }

    ComuneApplicabile(): boolean {
        return this.Comune.length >= 3;
    }

    FoglioApplicabile(): boolean {
        return this.Foglio.length >= 1;
    }

    ParticellaApplicabile(): boolean {
        return this.Particella.length >= 1;
    }

    Applicabile(): boolean {
        return this.ProvinciaApplicabile() ||
               this.ComuneApplicabile() ||
               this.FoglioApplicabile() ||
               this.ParticellaApplicabile()
    }

}

const layersSelectableFromAnagrafiche = new Map<string, enum_LayerElementiGraficiStd>([
    [enum_TreeDataItemType.Campionamenti, enum_LayerElementiGraficiStd.CAMPIONAMENTI],
    [enum_TreeDataItemType.Fabbricati, enum_LayerElementiGraficiStd.Fabbricati]
]);

@Component({
    standalone: false,
    selector: 'app-treeview',
    templateUrl: './tree.component.html',
    styleUrls: ['./tree.component.scss'],
    encapsulation: ViewEncapsulation.None,
})
export class TreeViewComponent implements
    OnInit, OnDestroy, AfterViewInit, OnChanges, AfterViewChecked, AfterContentChecked {
    @Input() draggable = false;
    @ViewChild("treeview") treeview: ElementRef;

    treeviewInstance: any;

    faUser = faUser;
    faIndustry = faIndustry;
    faHome = faHome;
    faLayerGroup = faLayerGroup;
    faMap = faMap;
    faLeaf = faLeaf;
    faTruckPickup = faTruckPickup;
    faWarehouse = faWarehouse;
    faUsers = faUsers;
    faExpand = faExpand;
    faGripHorizontal = faGripHorizontal;
    faObjectGroup = faObjectGroup;
    faCompressAlt = faCompressAlt;
    faExpandAlt = faExpandAlt;

    //--------------------------------------------------------------------------------
    // CheckableSettings Properties
    //--------------------------------------------------------------------------------
    public checkedKeys: any[] = [];
    public enableCheck = false;
    public checkChildren = false;
    public checkDisabledChildren = false;
    public checkParents = false;
    public checkOnClick = false;
    public checkMode: any = "multiple";
    public selectionMode: any = "single";

    //--------------------------------------------------------------------------------
    // TreeViewFilterSettings Properties
    //--------------------------------------------------------------------------------
    public ignoreCase = true;
    public mode = enum_FilterMode.lenient;
    public operator: any = enum_FilterOperator.contains;

    //--------------------------------------------------------------------------------

    public readonly filterExpandSettings: FilterExpandSettings = {
        expandedOnClear: "initial",
        expandMatches: true
    };

    public expandedKeys: any[] = [];
    public selectedKeys: any[] = [];
    public scrollToSelectedNode = false;
    nodes: Observable<TreeNode[]>;

    public allParentNodes = [];

    readonly host = 'http://localhost';
    private signal = new Subject<void>();

    private readonly openingExpandedKeysGis: any[] = ['0', '0_0', '0_0_0', '0_0_0_0', '0_0_0_1'];
    private readonly openingExpandedKeysAnagrafiche: any[] = ['0', '0_0', '0_0_2', '0_0_2_1', '0_0_2_1_0'];

    public contextGis: boolean;

    constructor(
        private anagraficaService: AnagraficaService,
        public service: TreeService,
        private treeContainerService: TreeContainerService,
        private conversionService: ConversionService,
        @Optional() public serviceGis: TreeGisService,
        @Optional() private featureService: FeatureService,
        @Optional() private sharedDataService: SharedDataService,
        @Optional() private layerService: LayerService
    ) {
        this.aggiornaCheckedKeys([],null);

        if (this.featureService) {
            this.featureService.getFeatureSelezionate$()
                .pipe(takeUntil(this.signal)).subscribe(featureSelezionate => {
                this.seRicaricaCheckedKeysFromFeature(featureSelezionate);
            });
        }

        if (this.treeContainerService.isContextGis()){
            this.applicaMatcherFunction();
        }

      this.contextGis = false;
      if (this.treeContainerService.isContextGis()){
        this.contextGis = true;
      }
    }

    //--------------------------------------------------------------------------------
    // checkableSettings
    //--------------------------------------------------------------------------------
    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: this.checkChildren,
            checkDisabledChildren: this.checkDisabledChildren,
            checkParents: this.checkParents,
            enabled: this.enableCheck,
            mode: this.checkMode,
            checkOnClick: this.checkOnClick };
    }

    //--------------------------------------------------------------------------------
    // filterSettings
    //--------------------------------------------------------------------------------
    public get filterSettings(): TreeViewFilterSettings {
        return {
            ignoreCase: this.ignoreCase,
            mode: this.mode,
            operator: this.operator };
    }

    ngOnInit(): void {
        if (this.treeContainerService.isContextGis()){

            this.enableCheck = true;

            this.serviceGis.highlightItems
                .pipe(takeUntil(this.signal)).subscribe(data => {
                if(data) {
                    this.expandedKeys = data.expanded;
                    this.selectedKeys = data.selected;
                }
            });

            this.nodes = this.serviceGis;

            this.expandedKeys = this.openingExpandedKeysGis;

        } else {

            this.service.highlightItems
                .pipe(takeUntil(this.signal)).subscribe(data => {
                if(data) {
                    this.expandedKeys = data.expanded;
                    this.selectedKeys = data.selected;
                }
            });

            this.nodes = this.service;

            this.expandedKeys = this.openingExpandedKeysAnagrafiche;

        }

        // Expand first node
        // if(!this.draggable) {
        //     this.expandedKeys.push('0');
        // }

        this.inizializzaCheckedKeysDaFeature();

    }

    ngAfterViewInit(){
        this.treeviewInstance = this.treeview;
        // console.log('[ngAfterViewInit]');
    }

    public hasChildren = (item: any) => item.items && item.items.length > 0;

    public fetchChildren = (item: any) => of(item.items);

    getServiceFilterFields() {
        if (this.treeContainerService.isContextGis()) {
            let filterFieldsValues = '';
            if (this.esistonoCampiFiltoValorizzati()) {
                const filtroCastasto = new FiltroCatasto(
                    this.serviceGis.filtroCatasto.Provincia,
                    this.serviceGis.filtroCatasto.Comune,
                    this.serviceGis.filtroCatasto.Foglio,
                    this.serviceGis.filtroCatasto.Particella);
                if (this.filtroGenericoApplicabile(this.serviceGis.filterTerm) || filtroCastasto.Applicabile()) {
                    filterFieldsValues = this.componiFiltroCombinato();
                }
            }
            return filterFieldsValues;
        } else {
            return this.service.filterTerm;
        }
    }

    private esistonoCampiFiltoValorizzati(): boolean {
        return this.serviceGis.filterTerm !== '' ||
               this.serviceGis.filtroCatasto.Provincia !=='' ||
               this.serviceGis.filtroCatasto.Comune !=='' ||
               this.serviceGis.filtroCatasto.Foglio !=='' ||
               this.serviceGis.filtroCatasto.Particella!=='';
    }

    private componiFiltroCombinato(): string {
        return this.serviceGis.filterTerm + separatoreFiltroAlbero +
               this.serviceGis.filtroCatasto.Provincia + separatoreFiltroAlbero +
               this.serviceGis.filtroCatasto.Comune + separatoreFiltroAlbero +
               this.serviceGis.filtroCatasto.Foglio + separatoreFiltroAlbero +
               this.serviceGis.filtroCatasto.Particella;
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    private applicaMatcherFunction() {
        const matcher: MatcherFunction = (
            dataItem: TreeNode, searchTerm: string
        ) => (this.isFiltroSoddisfatto(dataItem,searchTerm));
        this.operator = matcher;
    }

    private isFiltroSoddisfatto(
        dataItem: TreeNode,
        searchTerm: string
    ): boolean {

        const filtriAlbero: string[] = searchTerm.split(separatoreFiltroAlbero);

        // Filtro Generico

        const esitoFiltroGenerico = this.getEsitoFiltroGenerico(dataItem, filtriAlbero[0]);

        // Filtro Catasto

        const filtroCastasto = new FiltroCatasto(filtriAlbero[1],filtriAlbero[2],filtriAlbero[3],filtriAlbero[4]);

        const esitoFiltroCastasto = this.getEsitoFiltroCatasto(dataItem,filtroCastasto);

        // Ritorno esito combinato

        return esitoFiltroGenerico && esitoFiltroCastasto;

    }

    private getEsitoFiltroGenerico(
        dataItem: TreeNode,
        filtroGenerico: string
    ): boolean {

        let esitoFiltroGenerico = true;

        if (this.filtroGenericoApplicabile(filtroGenerico)) {
            esitoFiltroGenerico = dataItem.text.toUpperCase().indexOf(filtroGenerico.toUpperCase()) >= 0;
        }

        return esitoFiltroGenerico;

    }

    private filtroGenericoApplicabile(filtroGenerico: string): boolean {
        return filtroGenerico.length >= 3;
    }

    private getEsitoFiltroCatasto(
        dataItem: TreeNode,
        filtroCatasto: FiltroCatasto
    ): boolean {

        let esitoFiltroCastasto = true;

        if (filtroCatasto.Applicabile()) {
            esitoFiltroCastasto = false;
            const tipoNodo = parseInt(dataItem.id.split(separatoreChiaveAlbero)[0]);
            if (tipoNodo !== enum_TipoNodo.Particella ) {
                esitoFiltroCastasto = false;
            } else {
                const inizTestoCatasto = dataItem.text.indexOf('{') + 1;
                const fineTestoCatasto = dataItem.text.indexOf('}');
                const testoCatasto = dataItem.text.substring(inizTestoCatasto,fineTestoCatasto);
                const colonneCatasto: string[] = testoCatasto.split(':');
                if (colonneCatasto && colonneCatasto.length > 0) {
                    esitoFiltroCastasto = this.getEsitoFiltroCastastoColonne(filtroCatasto,colonneCatasto);
                }
            }
        }

        return esitoFiltroCastasto;

    }

    private getEsitoFiltroCastastoColonne(
        filtroCatasto: FiltroCatasto,
        colonneCatasto: string[]
    ): boolean {

        let esitoFiltroCastasto = true;

        if (filtroCatasto.ProvinciaApplicabile()) {
            const colonnaProvincia = colonneCatasto[0];
            esitoFiltroCastasto =
                esitoFiltroCastasto &&
                colonnaProvincia.toUpperCase().indexOf(filtroCatasto.Provincia.toUpperCase()) >= 0;
        }

        if (filtroCatasto.ComuneApplicabile()) {
            const colonnaComune = colonneCatasto[2];
            esitoFiltroCastasto =
                esitoFiltroCastasto &&
                colonnaComune.toUpperCase().indexOf(filtroCatasto.Comune.toUpperCase()) >= 0;
        }

        if (filtroCatasto.FoglioApplicabile()) {
            const colonnaFoglio = colonneCatasto[4];
            esitoFiltroCastasto =
                esitoFiltroCastasto &&
                colonnaFoglio.toUpperCase().indexOf(filtroCatasto.Foglio.toUpperCase()) >= 0;
        }

        if (filtroCatasto.ParticellaApplicabile()) {
            const colonnaParticella = colonneCatasto[5];
            esitoFiltroCastasto =
                esitoFiltroCastasto &&
                colonnaParticella.toUpperCase().indexOf(filtroCatasto.Particella.toUpperCase()) >= 0;
        }

        return esitoFiltroCastasto;

    }

    handleSelection(e: TreeItem) {
        if(this.isActive(e.dataItem)) {
            if (this.treeContainerService.isContextGis()) {
                this.serviceGis.parseItemIndex(e.index);
                const modifiedCheckedKeys = this.serviceGis.handleSelection(e, this.checkedKeys);
                this.aggiornaCheckedKeys(modifiedCheckedKeys);
                this.setLayerItemSelected();
                // console.log(`[handleSelection] ${checkedKeys.join(",")}`);
            } else {
                this.service.parseItemIndex(e.index);
                this.service.handleSelection(e);
            }
        }
    }

    handleChecked(e: TreeItemLookup){
        this.aggiornaCheckedKeys(this.checkedKeys);
        this.serviceGis.handleCheck(e, this.checkedKeys);
        this.setLayerItemSelected();
    }

    onFilterStateChange(datiFiltro: FilterState) {
        //--------------------------------------------------------------------------------
        // Gestito tramite FilterExpandSettings
        //--------------------------------------------------------------------------------
        // if (this.treeContainerService.isContextGis()) {
        //     if (datiFiltro.term !== '' && datiFiltro.nodes) {
        //         if (this.nodes) {
        //             this.nodes.subscribe(nodi => {
        //                 if (nodi){
        //                     this.getAllParentNodes(nodi, 0);
        //                     this.expandedKeys = this.allParentNodes.slice();
        //                 }
        //             });
        //         }
        //     } else {
        //         this.expandedKeys = this.openingExpandedKeysGis;
        //     }
        // }
    }

    public getAllParentNodes(
        items: TreeNode[],
        livello: number,
        treeKey?: number[]
        ) {
        if (treeKey === undefined){
            treeKey = [];
        }
        items.forEach((elemento, indice) =>{
            if (elemento.items) {
                treeKey[livello] = indice;
                let currentTreeKey = treeKey.slice(0,livello + 1).join('_');
                // console.log(currentTreeKey);
                this.allParentNodes.push(currentTreeKey);
                this.getAllParentNodes(elemento.items, livello+1, treeKey);
            }
        });
    }

    inizializzaCheckedKeysDaFeature() {
        if (this.featureService && this.nodes) {
            const featureSelezionate = this.featureService.getFeatureSelezionate();
            if (featureSelezionate.length > 0) {
                this.nodes.pipe(takeUntil(this.signal)).subscribe( () => {
                    // console.log('[this.nodes.subscribe]');
                    this.seRicaricaCheckedKeysFromFeature(featureSelezionate);
                });
            }
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        // console.log('[ngOnChanges] changes:',changes);
    }

    ngAfterContentChecked(): void {
        // console.log('[ngAfterContentChecked]');
    }

    ngAfterViewChecked(): void {
        // console.log('[ngAfterViewChecked]');
        this.scorriAlNodoSelezionato();
    }

    scorriAlNodoSelezionato() {
        if (this.scrollToSelectedNode && this.selectedKeys.length === 1) {
            const selettoreNodo = `li[data-treeindex='${this.selectedKeys[0]}']`;
            const nodiAlbero = document.querySelectorAll(selettoreNodo);
            if (nodiAlbero.length > 0) {
                setTimeout( ()=> {
                    nodiAlbero[0].scrollIntoView({
                        behavior: "smooth",
                        block: "start",
                        inline: "start"
                    });
                    this.scrollToSelectedNode = false;
                },100);
            }
        }
    }

    seRicaricaCheckedKeysFromFeature(featureSelezionate: any[]) {
        // const date = new Date();
        // console.log(`[${date.toISOString()}] featureSelezionate: ${featureSelezionate.length}`);
        if (this.nodes) {
            const origineChiamata = this.featureService.getOrigineChiamata();
            if (this.isOrigineMappa(origineChiamata)) {
                this.ricaricaCheckedKeysFromFeature(featureSelezionate, origineChiamata);
            } else {
                // TODO Andrea (4): valutare come migliorare
                // Se sono sull'albero, se le feature selezionate non corrispondono
                // ai checkedKeys, li ricarico
                if (featureSelezionate.length !== this.checkedKeys.length) {
                    this.ricaricaCheckedKeysFromFeature(featureSelezionate, origineChiamata);
                }
            }
        }
    }

    ricaricaCheckedKeysFromFeature(
            featureSelezionate: google.maps.Data.Feature[],
            origineChiamata: enum_OrigineChiamata
        ) {
        this.aggiornaCheckedKeys([],null);
        if (featureSelezionate.length > 0) {
            const elencoChiaviAlberoFeatureCompleteSelezionate = this.featureService.getElencoChiaviAlberoFeatureCompleteSelezionate();
            const checkedKeys = this.serviceGis.getCheckedKeysFromFeature(elencoChiaviAlberoFeatureCompleteSelezionate);
            this.aggiornaCheckedKeys(checkedKeys);
            // console.log(`[ricaricaCheckedKeysFromFeature] ${this.checkedKeys.join(",")}`);
            if (this.isOrigineMappa(origineChiamata)) {
                this.expandParentNodesCheckedKeys();
                this.selectNodeLastCheckedKey();
            }
        }
    }

    expandParentNodesCheckedKeys() {
        // console.log(`[expandParentNodesCheckedKeys] Prima: ${this.expandedKeys.join(",")}`);
        let newExpandedKeys = this.expandedKeys.slice();
        this.checkedKeys.forEach(checkedKey => {
            this.expandParentNode(checkedKey, newExpandedKeys);
        });
        if (newExpandedKeys.length > this.expandedKeys.length) {
            this.expandedKeys = newExpandedKeys.slice();
        }
        // console.log(`[expandParentNodesCheckedKeys] Dopo : ${this.expandedKeys.join(",")}`);
    }

    expandParentNode(nodeKey: any, newExpandedKeys: any[]) {
        const parentKey = this.getParentKey(nodeKey);
        if (parentKey) {
            if (this.isNodeCollapsed(parentKey)) {
                newExpandedKeys.push(parentKey);
            }
            this.expandParentNode(parentKey, newExpandedKeys);
        }
    }

    getParentKey(nodeKey: any): any{
        let parentKey = null;
        const stringNodeKey: string = nodeKey;
        const ultimoSeparatore = stringNodeKey.lastIndexOf("_");
        if (ultimoSeparatore > 0) {
            parentKey = stringNodeKey.substring(0,ultimoSeparatore);
        }
        return parentKey;
    }

    isNodeCollapsed(nodeKey: any): any{
        const indexNodeKeyExpanded = this.expandedKeys.findIndex(element => element === nodeKey);
        return indexNodeKeyExpanded === -1;
    }

    selectNodeLastCheckedKey(){
        let lastSelectedNode = this.checkedKeys.slice(-1);
        // console.log('[selectNodeLastCheckedKey]');
        // console.log(`checkedKeys: ${this.checkedKeys.join(",")}`);
        // console.log(`lastSelectedNode: ${lastSelectedNode[0]}`);
        this.selectedKeys=[lastSelectedNode[0]];
        this.scrollToSelectedNode=true;
        // setTimeout(x => {
        //     this.treeviewInstance.focus(lastSelectedNode[0]);
        // },100);
    }

    isOrigineMappa(origineChiamata: enum_OrigineChiamata): boolean{
        return origineChiamata === enum_OrigineChiamata.Mappa;
    }

    onExpandedKeysChange(expandedKeys: any[]) {
      let a = 0; //Commento per funzione vuota SonarQube
    }

    aggiornaCheckedKeys(
        modifiedCheckedKeys: any[],
        lastCheckedKeyFeatureId?: any
        ) {
        this.checkedKeys = modifiedCheckedKeys;
        if (this.sharedDataService) {
            this.sharedDataService.setTreeViewCheckedKeys(modifiedCheckedKeys);
            if (lastCheckedKeyFeatureId !== undefined) {
                this.sharedDataService.setLastCheckedKeyFeatureId(lastCheckedKeyFeatureId);
            }
        }
    }

    // TODO Salvo: questa è una pezza fatta per urgenza, da sistemare assolutamente in modo che funzioni per ogni layer
    private setLayerItemSelected() {
        if (this.layerService == null  || this.serviceGis == null) {
            return;
        }

        const checkedKeys = this.sharedDataService.getTreeViewCheckedKeys();
        if (checkedKeys.length == 0) {
            return;
        }

        const lastCheckedKey = checkedKeys[checkedKeys.length - 1];
        const node = this.serviceGis.getTreeNodeFromCheckedKey(lastCheckedKey);
        if (layersSelectableFromAnagrafiche.has(node.type)) {
            const layerId = layersSelectableFromAnagrafiche.get(node.type);
            this.layerService.setLayerItemSelected([this.sharedDataService.getTipologiaLayerById(layerId), true]);
        }
    }

    public getNodeStyle(dataItem: any): string {
        // if (this.treeContainerService.isContextGis()) {
        //     if(this.isActive(dataItem))
        //         return '';
        //     else
        //         return 'DimGray';
        // } else {
        //     if(this.isActive(dataItem))
        //         return '';
        //     else
        //         return 'DimGray';
        // }
      if(this.isActive(dataItem))
        return '';
      else
        return 'DimGray';
    }

    private isActive(dataItem: any): boolean {
        if (this.treeContainerService.isContextGis()) {
            let dateFilter = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporalePeriodo;
            return dataItem.startDate <= dateFilter.DataInizio && dataItem.endDate >= dateFilter.DataInizio;
        } else {
            let dateFilter = this.anagraficaService.filterData.getValue();
            if (typeof dateFilter.data == 'string') {
              dateFilter.data = this.conversionService.convertStringToDate(dateFilter.data);
            }
            return dateFilter.data == undefined ? true : (dataItem.startDate <= dateFilter.data && dataItem.endDate >= dateFilter.data);
        }
    }

  protected readonly faReload = faLeaf;

  expandAll(val: boolean){
    if (val) {
      this.expandedKeys = this.expandAllKey('', 0, this.service.getValue());
      console.log(this.expandedKeys);
    } else {
      this.expandedKeys = [];
    }
  }

  private expandAllKey(prefix: string, index: number, nodes: TreeNode[]): string[]{
    let arr = new Array<string>();
    nodes.forEach((node, i) => {
      if (node.items && node.items.length > 0){
        console.log(node.text, i, prefix, node.items.length);
        let correctPrefix = "";
        if (prefix != ""){
          correctPrefix = prefix + "_" + i;
        } else {
          correctPrefix = i.toString();
        }
        arr.push(correctPrefix);
        arr = arr.concat(this.expandAllKey(correctPrefix, i, node.items));
      }
    })
    return arr;
  }

}

