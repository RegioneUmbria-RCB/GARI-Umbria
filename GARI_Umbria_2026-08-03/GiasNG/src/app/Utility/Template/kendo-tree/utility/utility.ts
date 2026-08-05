import { TreeItem } from '@progress/kendo-angular-treeview';
import { AnagraficaLevels, AnagraficaRoutes } from 'app/anagrafica/anagrafica-routes';
import { separatoreChiaveAlbero } from 'app/Service/utils';
import { TreeNode } from '../model';


export class NodeType {
    public static Utente: NodeType       = new NodeType([1], 'Utente');
    public static Impresa: NodeType      = new NodeType([2], 'Impresa');
    public static Centro: NodeType       = new NodeType([3], 'Centro');
    public static Campo: NodeType        = new NodeType([4], 'Campo');
    public static Appezzamento: NodeType = new NodeType([5], 'Appezzamento');
    public static Impianto: NodeType     = new NodeType([6,7,8,9], 'Impianto');
    public static Esercizi: NodeType     = new NodeType([47], 'Esercizi');
    public static Catasto: NodeType      = new NodeType([10,18], 'Catasto');
    public static Macchine: NodeType     = new NodeType([35], 'Macchine');
    public static Contatti: NodeType     = new NodeType([36], 'Contatti');
    public static Fabbricati: NodeType   = new NodeType([37], 'Fabbricati');

    private constructor(public values: number[], public name: string) {}
}


export abstract class Anagrafica_PageSelector {
    protected curPageLink: string;

    constructor(protected link: string) {
        this.curPageLink = this.parseLink(link);
    }

    private parseLink(link: string) {
        if(link.includes('?')) {
            return link.split('?')[0];
        } else {
            return link;
        }
    }


}

export class AnagraficaTree_PageSelector extends Anagrafica_PageSelector {
    public decision: Decision;
    private selection: TreeNode;

    constructor(
        link: string,
        selection: TreeItem) {
            super(link);
            this.selection = selection.dataItem;
    }

    public decideTarget() {
        const target = this.classify(this.selection.id);

        if(target === null) {
            this.decision = null;
            return;
        }

        const id = this.GetGridItemId(target);

        this.decision = {
            target: target,
            samePage: target.link === this.curPageLink,
            gridItemId: id
        };
    }

    protected classify(clickedNodeId: string): AnagraficaRoutes {
        const id = Number.parseInt(clickedNodeId);

        switch(true) {
            case NodeType.Impresa.values.includes(id):
                return AnagraficaRoutes.Imprese;

            case NodeType.Centro.values.includes(id):
                return AnagraficaRoutes.Centri;

            case NodeType.Campo.values.includes(id):
                return AnagraficaRoutes.Campi;

            case NodeType.Appezzamento.values.includes(id):
                return AnagraficaRoutes.Appezzamenti;

            case NodeType.Impianto.values.includes(id):
                return AnagraficaRoutes.Impianti;

            case NodeType.Esercizi.values.includes(id):
                return AnagraficaRoutes.Esercizi;

            case NodeType.Catasto.values.includes(id):
                return AnagraficaRoutes.Catasto;

            case NodeType.Macchine.values.includes(id):
                return AnagraficaRoutes.Macchine;

            case NodeType.Contatti.values.includes(id):
                return AnagraficaRoutes.Contatti;

            case NodeType.Fabbricati.values.includes(id):
                return AnagraficaRoutes.Fabbricati;
            case NodeType.Utente.values.includes(id):
            default:
                return null;
        }
    }


    GetGridItemId(target: AnagraficaRoutes) {
        const entries = this.selection.id.split(separatoreChiaveAlbero);

        let id = '';

        // Metodo ImpostaSelezioneAlbero in GiasOnline_2010.
        switch(target.lvl) {
            case AnagraficaLevels.Imprese:
                id = entries[1];
                break;
            case AnagraficaLevels.Centri:
                id = entries[1] + '_' + entries[2];
                break;
            case AnagraficaLevels.Campi:
                id = entries[1] + '_' + entries[2] + '_' + entries[3];
                break;
            case AnagraficaLevels.Appezzamenti:
                id = entries[1] + '_' + entries[2] + '_' + entries[4];
                break;
            case AnagraficaLevels.Impianti:
                id = entries[1] + '_' + entries[2] + '_' + entries[4] + '_'+ entries[5];
                break;
            case AnagraficaLevels.Esercizi:
                id = entries[1] + '_' + entries[2] + '_' + entries[4] + '_'+ entries[5];
                break;
            case AnagraficaLevels.Catasto:
                id =  entries[1] + '_' + entries[2] + '_' + entries[7] + '_' + entries[8] + '_' + entries[9] + '_' + entries[10] + '_' + entries[11] + '_' + entries[12] + '_' + entries[6];
                break;
            case AnagraficaLevels.Macchine:
                // Nothing to do.
                break;
            case AnagraficaLevels.Contatti:
                // Nothing to do.
                break;
            case AnagraficaLevels.Fabbricati:
                // Nothing to do.
                break;
        }

        return id;
    }

}


class Decision {
    target: AnagraficaRoutes;
    samePage: boolean;
    gridItemId: string;
}

