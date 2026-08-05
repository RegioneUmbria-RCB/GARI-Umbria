import { Injectable } from '@angular/core';
import { KendoGridColumn, KendoGridModel, KendoGridRow } from '../models/grid.model';
import { KendoGridMasterDetailService } from './grid-master-detail.service';
import { CELL_TYPES } from 'gias-ui-kit';

@Injectable({ providedIn: 'root' })
export class GridErrorService {
    public loggingAttivo = false;

    private infoBGColor = 'background: #BDE5F8; color: #00529B';
    private errBGColor = 'background: #FFD2D2; color: #D8000C';

    /**
   * Expensive operation.
   */
    private runInDepthAnalysis = false;

    runDataConsistencyChecks(model: KendoGridModel, columns: KendoGridColumn[],
        righe: KendoGridRow[]) {
        if (!this.runInDepthAnalysis) {
            return;
        }

        columns.forEach((column) => {
            const type = model[column.field].type;

            righe.forEach(riga => {
                const value = riga[column.field];
                switch (type) {
                    case CELL_TYPES.STRING:
                    case CELL_TYPES.BOOLEAN:
                    case CELL_TYPES.DROPDOWNLIST:
                        break;
                    case CELL_TYPES.NUMBER:
                        this.checkValidNumber(value, column);
                        break;
                    case CELL_TYPES.DATE:
                        this.checkValidDate(value, column);
                        break;
                    case CELL_TYPES.DATETIME:
                        this.checkValidDate(value, column);
                        break;
                }
            });
        });
    }

    public checkThis(func: (...fnArgs) => any, ...args: any[]): any {
        if (!this.loggingAttivo) {
            return;
        }
        return (func.bind(this))(...args);
    }

    public checkRequiredFields(model: KendoGridModel,
        columns: Array<KendoGridColumn>, rows: KendoGridRow[]) {
        if (model == null) {
            this.logErr('Il modello non è stato impostato!');
        }

        if (columns == null) {
            this.logErr('Le collone non sono state impostate!');
        }

        for (const col of columns) {
            if (model[col.field] == null) {
                this.logErr('E\' stata trovata una colonna che non compare nel modello: ' + col.field);
            }
        }


        for (const col of columns) {
            const modelField = model[col.field];

            if (!modelField) {
                continue;
            }

            switch (model[col.field].type) {
                case CELL_TYPES.DATE:
                    // Deprecated
                    // this.logErr('Cell type is date but settings not provided. Field:' + col.field);
                    break;
                case CELL_TYPES.DATETIME:
                    // Deprecated
                    // this.logErr('Cell type is date but settings not provided. Field:' + col.field);
                    break;
                case CELL_TYPES.NUMBER:
                    if (!col.numeric) {
                        this.logErr('Cell type is numeric but settings not provided. Field: ' + col.field);
                    }
                    break;
                case CELL_TYPES.DROPDOWNLIST:
                    if (!col.ddl) {
                        this.logErr('Dropdown cell type but settings not provided. Field: ' + col.field);
                    }
                    break;
                case CELL_TYPES.STRING:
                    // Nothing to check.
                    break;
                case CELL_TYPES.CUSTOM:
                    // Nothing to check.
                    break;
                default:
                    alert('Not yet implemented. Field type: ' + model[col.field].type);
            }
        }
        this.runDataConsistencyChecks(model, columns, rows);
    }

    public modelEntryExists(entryExists: boolean, fieldName: string): void {
        if (!entryExists) {
            this.logErr('Il modello manca una collona chiave per il campo col nome: ' + fieldName);
        }
    }
    public logErr(msg: string): void {
        if (!this.loggingAttivo) {
            return;
        }

        console.log('%c' + msg, this.errBGColor);
    }

    public logInfo(msg: string, className: string = '', fnName: string = '', ...args): void {
        if (!this.loggingAttivo) {
            return;
        }

        console.log('%c' + '[' + className + ':' + fnName + ']' + msg, this.infoBGColor);
    }

    public generalErrors(errorType: GenericErrors, ...args) {
        if (!this.loggingAttivo) {
            return;
        }

        switch (errorType) {
            case GenericErrors.EditingModeNotFound:
                this.logErr('Was not able to find ' + [...args]);
        }
    }


    private checkValidDate(value: any, column: KendoGridColumn) {
        const min = column.date?.min;
        const max = column.date?.max;
        value = new Date(value);
        if (min > max) {
            this.logErr(`Field: ${column.field}. Inconsistent date value: ${value}.`);
            this.logErr(`Missconfigured date: min (${min}) > max (${max})`);
            return;
        }
        if (value != null && value < min || value > max) {
            this.logErr(`Field: ${column.field}. Inconsistent date value: ${value}.`);
            this.logErr(`min: ${column.date.min}, max: ${column.date.max}`);
        }
    }

    private checkValidNumber(value: any, column: KendoGridColumn) {
        const min = column.numeric?.min;
        const max = column.numeric?.max;
        if (min > max) {
            this.logErr(`Column: ${column.field}. Inconsistent date value: ${value}.`);
            this.logErr(`Missconfigured date: min (${min}) > max (${max})`);
            return;
        }
        if (value < min || value > max) {
            this.logErr(`Field: ${column.field}. Inconsistent numeric value: ${value}.`);
            this.logErr(`min: ${column.numeric.min}, max: ${column.numeric.max}`);
        }
    }

    public checkGridMasterDetailConfig(masterDetailService: KendoGridMasterDetailService, enable_master_detail: boolean) {
        if (enable_master_detail && !masterDetailService) {
            this.logErr(`Per utilizzare la funzione di MasterDetail della kendo grid aggiungere nel generategridProviders() della grid principale il provide al KendoGridMasterDetailService`);
        }
    }

}

export enum GenericErrors {
    EditingModeNotFound
}
