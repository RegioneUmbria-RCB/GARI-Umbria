import { ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {DynamicInputSettigs} from 'gias-kendo-grid';
import {Specie} from "../../../../../Model/metaschema/utilizzi/Specie";
import {Prodotto} from "../../../../../Model/attivita/risorse/Prodotto";

export class GridRilieviObject {
    Impianto_Cod: number;
    Impianto_Des: string;
    Op_Cod: number;
    Op_Des: string; // descrizione operazione
    Descriz_Cod: number;
    Descriz_Des: string;
    Qta: string;
    QtaNumber: number;
    QtaBoolean: boolean;
    QtaDate: Date;
    ResaAnagrafica: string;

    Impianto_Key: string;
    Descriz_Key: string;

    QtaObj: DynamicInputSettigs

    SpecieVegetale: Specie;

    Row_Key: string

    Prodotto_Obj: Prodotto;
    Pro_Des: string;
}

export class GridRilieviModel {
    Impianto_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Impianto_Des = new ModelEntry(CELL_TYPES.STRING);
    Op_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Op_Des = new ModelEntry(CELL_TYPES.STRING);
    Descriz_Cod = new ModelEntry(CELL_TYPES.NUMBER);
    Descriz_Key = new ModelEntry(CELL_TYPES.STRING);
    Descriz_Des = new ModelEntry(CELL_TYPES.STRING);
    Qta = new ModelEntry(CELL_TYPES.STRING);
    QtaNumber = new ModelEntry(CELL_TYPES.NUMBER); //può essere una data, un numero...
    QtaBoolean = new ModelEntry(CELL_TYPES.BOOLEAN);
    QtaDate = new ModelEntry((CELL_TYPES.DATE));

    QtaObj = new ModelEntry((CELL_TYPES.CUSTOM));
    ResaAnagrafica = new ModelEntry(CELL_TYPES.STRING);
    SpecieVegetale = new ModelEntry(CELL_TYPES.OBJECT);
    Prodotto_Obj = new ModelEntry(CELL_TYPES.OBJECT);
    Pro_Des = new ModelEntry(CELL_TYPES.STRING);
};
