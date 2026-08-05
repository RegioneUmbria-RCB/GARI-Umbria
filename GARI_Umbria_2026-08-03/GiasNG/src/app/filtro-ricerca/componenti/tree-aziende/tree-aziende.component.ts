import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { IconDefinition, faArrowDown, faArrowRight, faCheckCircle, faEllipsis, faFontAwesome, faIndustry, faObjectGroup, faUser } from "@fortawesome/free-solid-svg-icons";
import { DrawerComponent } from "@progress/kendo-angular-layout";
import { FilterExpandSettings } from "@progress/kendo-angular-treeview";
import { enum_TipoNodo } from "app/Model/TipiEnumerativi";
import { TreeAziendeService } from "app/filtro-ricerca/service/tree-aziende.service";
import { Subscription, of } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-tree-aziende',
    templateUrl: './tree-aziende.component.html',
    styleUrls: ['./tree-aziende.component.scss']
})
export class TreeAziende implements AfterViewInit {

    @ViewChild('drawer') drawer: DrawerComponent;
    @ViewChild('drawer', {static: false, read: ElementRef}) drawerElRef: ElementRef;

    @Output() pivaEmitter: EventEmitter<string> = new EventEmitter<string>();
    @Output() impresaEmitter: EventEmitter<any> = new EventEmitter<any>();
    @Output() pivaRagSocEmitter: EventEmitter<any> = new EventEmitter<any>();

    @Input() showImpostaAziendaCorrente: boolean;

    @Input() withPadding: boolean;

    public isPopupOpen = false;
    shouldApplyTreeAziende = false;

    constructor (public service: TreeAziendeService) {}

    private expanderSub: Subscription | undefined;
    expanded = false;
    containerWidth = 550;

    faArrowDown = faArrowDown;
    faArrowRight = faArrowRight;
    faCheckCircle = faCheckCircle;
    faEllipsis = faEllipsis;

    expandedKeys: any[] = [];
    selectedKeys: any[] = [];
    selectedNode: any;

    hasChildren = (item: any) => item.Items && item.Items.length > 0;
    fetchChildren = (item: any) => of(item.Items);

    onExpandedKeysChange(event: any) {
        this.expandedKeys = event;
    }

    ngAfterViewInit(): void {
        this.service.drawerRef = this.drawerElRef;
        this.service.getTreeAziende();
        this.expanderSub = this.service.expander.subscribe((state) => {
            this.shouldApplyTreeAziende = state;
            this.drawer.toggle(state);
        });
    }

    onFilterStateChange(event: any) {
        console.log('Filter state changed', event);
    }

    handleSelection(event: any) {
        this.selectedNode = event.dataItem;
    }

    isSelected(dataItem: any): boolean {
        return (this.selectedNode === dataItem) && (dataItem.TipoNodo == enum_TipoNodo.x_OP || dataItem.TipoNodo == enum_TipoNodo.x_Consorzio || dataItem.TipoNodo == enum_TipoNodo.x_Cooperativa || dataItem.TipoNodo == enum_TipoNodo.Impresa);
    }

    onButtonClick(button: string, node: any): void {
        console.log(`${button} clicked for node:`, node);
    }

    getIconForTree(dataItem): IconDefinition | string {

        let classeIcona = 'icon icon-popup ';

        switch(dataItem.TipoNodo) {

            case enum_TipoNodo.Impresa:
                return faIndustry;
            case enum_TipoNodo.x_Cooperativa:
                if (this.isSelected(dataItem))
                    classeIcona += 'icon-coop';
                else
                    classeIcona += 'icon-coop-off';
            break;
            case enum_TipoNodo.x_Consorzio:
                if (this.isSelected(dataItem))
                    classeIcona += 'icon-consorzio';
                else
                    classeIcona += 'icon-consorzio-off';
            break;
            case enum_TipoNodo.x_OP:
                if (this.isSelected(dataItem))
                    classeIcona += 'icon-op';
                else
                    classeIcona += 'icon-op-off';
            break;
        }

        return classeIcona;
    }

    onCancel() {
        this.shouldApplyTreeAziende = false;
    }

    isIconDefinition(value: any): value is IconDefinition {
        return value && typeof value === 'object' && 'iconName' in value;
    }

    getServiceFilterFields() {
        return ['text'];
    }

    onClickImpresaReferente(dataItem: any) {
        this.pivaEmitter.emit(dataItem.Piva);
    }

    onClickImpostaPIVA(dataItem: any) {
        this.pivaRagSocEmitter.emit(dataItem);
    }

    onClickImpostaAzienda(dataItem: any) {
        this.impresaEmitter.emit(dataItem);
    }

    showImpresaReferente(tipoNodo: any) {
        return tipoNodo == enum_TipoNodo.x_OP || tipoNodo == enum_TipoNodo.x_Consorzio || tipoNodo == enum_TipoNodo.x_Cooperativa;
    }

    togglePopup(): void {
        this.isPopupOpen = !this.isPopupOpen;
    }

    public readonly filterExpandSettings: FilterExpandSettings = {
        expandedOnClear: "initial",
        expandMatches: true
    };

    expandAll(val: boolean) {
        if (val) {
          this.expandedKeys = this.expandAllKey('', 0, this.service.filteredNodes);
        //   console.log(this.expandedKeys);
        } else {
          this.expandedKeys = [];
        }
    }

    private expandAllKey(prefix: string, index: number, nodes: any): string[] {
        let arr = new Array<string>();
        nodes.forEach((node, i) => {
            if (node.Items && node.Items.length > 0) {
                // console.log(node.text, i, prefix, node.items.length);
                let correctPrefix = "";
                if (prefix != ""){
                    correctPrefix = prefix + "_" + i;
                } else {
                    correctPrefix = i.toString();
                }
                arr.push(correctPrefix);
                arr = arr.concat(this.expandAllKey(correctPrefix, i, node.Items));
            }
        })
        return arr;
    }

}
