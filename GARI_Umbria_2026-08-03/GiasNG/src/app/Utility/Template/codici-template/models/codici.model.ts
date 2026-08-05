import { InjectionToken } from '@angular/core';
import { KendoServerResult } from 'gias-kendo-grid';
import { ICodiciTemplateService } from '../services/codici-template.service';

export const CODICI_TOKEN = new InjectionToken<ICodiciTemplateService>('app.codici.service');

export class CodiciServerResult extends KendoServerResult {
    constructor(model, cols, rows) {
        super(model, cols, rows);
    }
}

// export class CodiceDDL {
//   codice: string;
//   descrizione: string;
// }

export interface ICodiceTemplateResult {
    codiciDropdown: any[];
    rows: any[];

    textField: string;
    valueField: string;
    dateControl: boolean;
}

