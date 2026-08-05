import { Injectable } from '@angular/core';
import { ParticelleCatastaliClassamento } from 'app/Model/anagrafiche/ParticelleCatastaliClassamento';
import { BehaviorSubject } from 'rxjs';

export class ParticelleCatastaliClassamentoId extends ParticelleCatastaliClassamento{
    id: number;
}

@Injectable({
    providedIn:'root'
})
export class ClassamentoCatastoService{
    classamentoParticella: ParticelleCatastaliClassamentoId[] = new Array();

    classamentoParticellaSource = new BehaviorSubject(this.classamentoParticella);
    public setClassamentoParticella(classamentoCatasto: ParticelleCatastaliClassamentoId[]) {
        for (const clasamentoCat of classamentoCatasto) {
            if (clasamentoCat.id == undefined || clasamentoCat.id < 0) {
                clasamentoCat.id = this.getNextId(classamentoCatasto);
            }
        }
        this.classamentoParticellaSource.next(classamentoCatasto);
    }

    private getNextId(classamentoCatasto: ParticelleCatastaliClassamentoId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, classamentoCatasto.map(function (o) {
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

    public getClassamentoCatasto(): ParticelleCatastaliClassamentoId[] {
        return this.classamentoParticellaSource.getValue();
    }

}
