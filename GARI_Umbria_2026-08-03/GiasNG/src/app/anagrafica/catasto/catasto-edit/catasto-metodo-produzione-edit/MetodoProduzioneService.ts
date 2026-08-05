import { Injectable } from '@angular/core';
import { ParticelleCatastaliMetodoProduzione } from 'app/Model/anagrafiche/ParticelleCatastaliMetodoProduzione';
import { BehaviorSubject, Observable } from 'rxjs';

export class MetodoProduzioneId extends ParticelleCatastaliMetodoProduzione{
    id: number;
}

@Injectable({
    providedIn:'root'
})
export class MetodoProduzioneService{
    metodoProduzioneParticella: MetodoProduzioneId[] = new Array();

    metodoProduzioneParticellaSource = new BehaviorSubject(this.metodoProduzioneParticella);
    public setMetodoProduzioneParticella(metodoProduzioneCatasto: MetodoProduzioneId[]) {
        for (const metodoProduzioneCat of metodoProduzioneCatasto) {
            if (metodoProduzioneCat.id == undefined || metodoProduzioneCat.id < 0) {
                metodoProduzioneCat.id = this.getNextId(metodoProduzioneCatasto);
            }
        }
        this.metodoProduzioneParticellaSource.next(metodoProduzioneCatasto);
    }

    private getNextId(metodoProduzioneCatasto: MetodoProduzioneId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, metodoProduzioneCatasto.map(function (o) {
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

    public getMetodoProduzioneCatasto(): MetodoProduzioneId[] {
        return this.metodoProduzioneParticellaSource.getValue();
    }

}
