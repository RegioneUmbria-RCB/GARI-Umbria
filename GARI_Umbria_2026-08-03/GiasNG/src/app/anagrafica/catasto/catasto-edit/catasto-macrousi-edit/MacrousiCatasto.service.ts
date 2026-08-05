import { Injectable } from '@angular/core';
import { ParticelleCatastaliMacrouso } from 'app/Model/anagrafiche/ParticelleCatastaliMacrouso';
import { ParticelleCatastaliMetodoProduzione } from 'app/Model/anagrafiche/ParticelleCatastaliMetodoProduzione';
import { BehaviorSubject, Observable } from 'rxjs';

export class ParticelleCatastaliMacrousoId extends ParticelleCatastaliMacrouso{
    id: number;
}

@Injectable({
    providedIn:'root'
})
export class MacrousiCatastoService{
    macrousiParticella: ParticelleCatastaliMacrousoId[] = new Array();
    macrousiParticellaSource = new BehaviorSubject(this.macrousiParticella);

    public setMacrousiParticella(macrousiCatasto: ParticelleCatastaliMacrousoId[]) {
        for (const macrousoCat of macrousiCatasto) {
            if (macrousoCat.id == undefined || macrousoCat.id < 0) {
                macrousoCat.id = this.getNextId(macrousiCatasto);
            }
        }
        this.macrousiParticellaSource.next(macrousiCatasto);
    }

    private getNextId(macrousoCatasto: ParticelleCatastaliMacrousoId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, macrousoCatasto.map(function (o) {
            if (o.id != undefined) {
                return o.id;
            } else {
                return -1;
            }
        }));
        if (chiave >= 0) {
            newId = chiave + 1;
        }
        return newId;
    }

    public getMacrousiCatasto(): ParticelleCatastaliMacrousoId[] {
        return this.macrousiParticellaSource.getValue();
    }

}
