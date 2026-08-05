import { Injectable } from '@angular/core';
import { CostoUnitarioChiave } from 'app/Model/anagrafiche/CostoUnitario';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    providedIn:'root'
})
export class CostiMacchinaDataService{
    costiMacchina: CostoUnitarioChiave[] = new Array<CostoUnitarioChiave>();

    costiMacchinaSource = new BehaviorSubject(this.costiMacchina);

    public setCostiMacchina(costi: CostoUnitarioChiave[]) {
        for (const costo of costi) {
            if (costo.chiave == undefined || costo.chiave < 0) {
                costo.chiave = this.getNextId(costi);
            }
        }
        this.costiMacchinaSource.next(costi);
    }

    private getNextId(costi: CostoUnitarioChiave[]): number {
        let newChiave = 0;
        const chiave = Math.max.apply(Math, costi.map(function (o) {
            if (o.chiave != undefined) {
                return o.chiave;
            } else {
                return -1;
            }
        }));
        if (chiave >= 0) {
            newChiave = chiave + 1;
        }
        return newChiave;
    }

    public getCostiMacchina(): CostoUnitarioChiave[] {
        return this.costiMacchinaSource.getValue();
    }
}
