import { Injectable } from '@angular/core';
import { HttpAction } from 'gias-kendo-grid';
import { KendoGridRow } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';

import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { CostoUnitarioChiave } from 'app/Model/anagrafiche/CostoUnitario';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { CostiMacchinaDataService } from './costi-macchina-data.service';

const defaultIndex = -1;

const itemIndex = (item: CostoUnitarioChiave, data: CostoUnitarioChiave[]): number => {
    for (let idx = 0; idx < data.length; idx++) {
        if (data[idx].chiave === item.chiave) {
            return idx;
        }
    }
    return defaultIndex;
};

@Injectable()
export class CostiMacchinaEditService {

    private costi: CostoUnitarioChiave[];

    constructor(
        private costiMacchinaDataService: CostiMacchinaDataService
    ) { }

    perform(actionType: HttpAction, row: any): Observable<KendoGridRow[]> {
        this.updateTable(actionType, row);
        return from([]);
    }

    public updateTable(action: HttpAction, row: any) {
        
        if (this.costi == undefined || this.costi.length == 0) {
            this.costi = this.costiMacchinaDataService.getCostiMacchina();
        }

        let cost = this.convertRowToCostoUnitarioChiave(row);

        switch(action) {
            case HttpAction.CREATE:
                if (this.costi != undefined && this.costi.length != 0){
                    const chiave = Math.max.apply(Math, this.costi.map(function (o) {
                        return o.chiave;
                    }));
                    cost.chiave = chiave + 1;
                } else {
                    const chiave = 0;
                    cost.chiave = chiave;
                }
                cost.codice = 0;
                this.costi.push(cost);
                break;
            case HttpAction.UPDATE:
                const index = itemIndex(cost, this.costi);
                this.costi.splice(index, 1, cost);
                break;
            case HttpAction.REMOVE:
                const _index = itemIndex(cost, this.costi);
                this.costi.splice(_index, 1);
                break;
        }

        this.costiMacchinaDataService.setCostiMacchina(this.costi);
    }

    private convertRowToCostoUnitarioChiave(row: any): CostoUnitarioChiave {
        let cost = new CostoUnitarioChiave();
        cost.unitaDiMisura = new UnitaDiMisura(row.Unita_Misura_Cod, row.Unita_Misura_Des);
        cost.validita = new IntervalloTemporale(row.Validita_Inizio, row.Validita_Fine);
        cost.codice = row.codice;
        cost.chiave = row.chiave;
        cost.prezzo = row.prezzo;
        cost.flag_cancellazione = false;

        return cost;
    }

}
