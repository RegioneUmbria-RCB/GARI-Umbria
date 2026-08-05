import { ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';

export class GridRaccoltaObject {
    Progetto_Cod: number | string;
    Progetto_Des: string;
    Op_Cod: number;
    Op_Des: string; // descrizione operazione (es. Raccolta 1)
    Data_Operazione: string;
    Sa_Cod: number;
    Sa_Nome: string;
    APPEZ_Cod: number;
    APPEZ_Des: string; // appezzamenti/impianti/esercizi interessati
    Specie_Des: string;
    Specie_Cod: number;
    Cul_Des: string; // cultivar, varietà
    Cul_Cod: number;
    BIO: boolean;
    ID_Reg: number;
    Prod: string;
    UM_Des: string;
    UM_Cod: number;
    Qta: number;
    Lotto: string;
    Magazzino_Des: string;
    Magazzino_Cod: string;
    Ora_Ingresso: string;
    Sup: number;
    Prodotto_Cod: number;
    Prodotto_Des: string;
}

export class GridRaccoltaModel {
    Progetto_Cod = new ModelEntry(CELL_TYPES.STRING);
    Progetto_Des = new ModelEntry(CELL_TYPES.STRING);
    Op_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Op_Des = new ModelEntry(CELL_TYPES.STRING);
    Data_Operazione = new ModelEntry(CELL_TYPES.DATE);
    Sa_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Sa_Nome = new ModelEntry(CELL_TYPES.STRING);
    APPEZ_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    APPEZ_Des = new ModelEntry(CELL_TYPES.STRING);
    Specie_Des = new ModelEntry(CELL_TYPES.STRING);
    Specie_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Cul_Des = new ModelEntry(CELL_TYPES.STRING);
    Cul_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    BIO = new ModelEntry(CELL_TYPES.STRING);
    ID_Reg = new ModelEntry(CELL_TYPES.NUMBER);
    Prodotto_Cod = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
    Prodotto_Des = new ModelEntry(CELL_TYPES.STRING);
    UM_Des = new ModelEntry(CELL_TYPES.STRING);
    UM_Cod = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
    Qta = new ModelEntry(CELL_TYPES.NUMBER);
    Lotto = new ModelEntry(CELL_TYPES.STRING);
    Magazzino_Des = new ModelEntry(CELL_TYPES.STRING);
    Magazzino_Cod = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
    Ora_Ingresso = new ModelEntry(CELL_TYPES.STRING);
    Sup = new ModelEntry(CELL_TYPES.NUMBER);
};

