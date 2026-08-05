import { Injectable } from '@angular/core';
import { PossessoParticella } from 'app/Model/anagrafiche/PossessoParticella';
import { isNumber } from 'lodash';
import { BehaviorSubject, Observable } from 'rxjs';
import {ParticelleCatastali} from "../../../../Model/anagrafiche/ParticelleCatastali";
import {CatastoCentroAziendale} from "../../../../Model/anagrafiche/CatastoCentroAziendale";

export class PossessoParticellaId extends PossessoParticella{
    id: number;
}

@Injectable({
    providedIn:'root'
})
export class PossessiService{
    possessoParticella: PossessoParticellaId[] = new Array();

    possessoParticellaSource = new BehaviorSubject(this.possessoParticella);

    particellaVal: CatastoCentroAziendale;

    public setPossessoParticella(catastoAppezzamento: PossessoParticellaId[]) {
        for (const catastoApp of catastoAppezzamento) {
            if (catastoApp.id == undefined || catastoApp.id < 0) {
                catastoApp.id = this.getNextId(catastoAppezzamento);
            }
        }
        this.possessoParticellaSource.next(catastoAppezzamento);
    }

    private getNextId(catastoAppezzamento: PossessoParticellaId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, catastoAppezzamento.map(function (o) {
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

    public getPossessoParticella(): PossessoParticellaId[] {
        return this.possessoParticellaSource.getValue();
    }

}
