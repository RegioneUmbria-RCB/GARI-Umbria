import { Validators } from '@angular/forms';
import { KendoCampiModel } from 'app/anagrafica/campi/campi.model';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { Campo } from 'app/Model/anagrafiche/Campo';
import { CatastoCampo } from 'app/Model/anagrafiche/CatastoCampo';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {  KendoGridColumn, KendoGridModel } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AppezzamentoCampo } from './appezzamento-campo-edit/appezzamento-campi-edit';

export class CodiciCampiKendoModel extends KendoGridModel {}

export class CampiCodiciTableData {
    // static columns: Array<KendoGridColumn> = [

    //     new KendoGridColumn({
    //         field: 'Val_Cod',
    //         title: 'Value'
    //     },
    //     {
    //         hidden: false,
    //         width: 200,
    //         resizable: true,
    //         validators: [Validators.required]
    //     }),
    //     new KendoGridColumn({
    //         field: 'Id_Cod',
    //         title: 'Descrizione'
    //     },
    //     {
    //         hidden: false,
    //         width:200,
    //         resizable: true,
    //         validators: [Validators.required]
    //     })
    // ];

    static model: CodiciCampiKendoModel = {
        chiave:{
            editable: true,
            type: CELL_TYPES.NUMBER
        },
        Val_Cod:
        {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Descrizione:
        {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Id_Cod:
        {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
        },
        Validita_Inizio:
        {
            editable: true,
            type: CELL_TYPES.DATE
        },
        Validita_Fine:
        {
            editable: true,
            type: CELL_TYPES.DATE,
        }
    };
}



export class CaricaDatiDto {
    Campo: Campo;
    ObjParamAgenda: ObjParametriAgenda;
}
