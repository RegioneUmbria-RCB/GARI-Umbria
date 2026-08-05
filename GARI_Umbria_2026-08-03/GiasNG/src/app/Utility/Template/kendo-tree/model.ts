export class TreeNode {
    expanded: boolean;
    id: string;
    imageUrl: string;
    items?: TreeNode[];
    style: string;
    text: string;
    selected?: boolean;
    type: string;
    startDate?: Date;
    endDate?: Date;

    constructor(node?: Required<TreeNode>) {
        this.text = node.text;
        this.imageUrl = node.imageUrl;
        this.selected = node.selected ?? false;
        this.items = node.items ?? [];
    }
}

export class SelectionTarget {

}

export class TreeResolverInput {
}

export class TreeResolverResult {

}

export const TreeCfg = {

    'LetturaViaSQLJson':false,
    'Flag_Esplodi_Tutto':true,
    'Flag_CheckBox':false,
    'Flag_Planning':false,
    'Flag_Anagrafica':true,
    'Flag_Contatti':true,
    'Flag_Analisi':false,
    'Flag_PianoConcimazione':false,
    'Flag_Esercizio':false,
    'Flag_ParcoMacchine':true,
    'Flag_CatastoAziendale':false,
    'Flag_CatastoAppezzamento':false,
    'Flag_Fabbricati':true,
    'Flag_PortafoglioProdotti':false,
    'Flag_Singola_Selezione':true,
    'Flag_Appezzamenti_Filtra_Tecnico':false,
    'Flag_Agenda':false,
    'FiltroImpiantiIdTestataTemp':0,
    'ordinaDataUltimoImpianto':false,
    'visualizzaRiferimentoAlfanumericoImpianto':false,
    'TipoOperazioneColturale':null,
    'Piva':'',
    'Sa_Cod':'0',
    'Veg_Cod':0,
    'Cul_Cod':0,
    'Flag_Carica_Primo_Giro':false,
    'dataInizio':'1900-01-01T00:00:00',
    'dataFine':'2100-12-31T00:00:00',
    'CheckBoxes':{
        'Flag_CheckBoxUtente':false,
        'Flag_CheckBoxImpresa':false,
        'Flag_CheckBoxContatti':false,
        'Flag_CheckBoxParcoMacchine':false,
        'Flag_CheckBoxCentro':false,
        'Flag_CheckBoxCatasto':false,
        'Flag_CheckBoxParticella':false,
        'Flag_CheckBoxProdotti':false,
        'Flag_CheckBoxFabbricati':false,
        'Flag_CheckBoxMagazzino':false,
        'Flag_CheckBoxGiacenze':false,
        'Flag_CheckBoxMovimenti':false,
        'Flag_CheckBoxAppezzamento':false,
        'Flag_CheckBoxImpianto':false,
        'Flag_CheckBoxCampo':false,
        'Flag_CheckBoxSerra':false,
        'Flag_CheckBoxAnalisi':false,
        'Flag_CheckBoxCampioni':false,
        'Flag_CheckBoxRicette':false
    },
    'DatiSportelloSementieri':null,
    'ParametriAgendaData':'1900-01-01T00:00:00',
    'Elenco_Icone_SpecieVegetali':'1:2:3:9:11:13:15:16:17:21:22:23:25:28:30:31:32:33:34:35:38:39:40:41:43:44:45:46:47:48:49:50:51:52:53:54:56:57:58:59:60:62:63:64:65:66:69:70:71:72:73:74:75:76:77:78:79:80:81:83:84:85:86:87:91:101:102:108:110:116:117:118:120:125:126:127:209:210:235:294:311:347:350:5000123:5000309:5000310:9999999',
    'PivaPadre':'',
    'Valore_Albero':null,
    'FlagModalitaSementieri':false,
    'GruppoOperazioneColturale':null,
    'Flag_Ricette':false,
    'TipologiaLayer_Cod':1,
    'Contesto':1,
    'DataInizio':'2021-11-09T23:00:00.000Z',
    'DataFine':'2021-11-09T23:00:00.000Z'
};

export class UpdateTree {
    selected: string[];
    expanded: string[];
}
