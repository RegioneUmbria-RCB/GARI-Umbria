import { Injectable } from '@angular/core';
import { ParticelleCatastaliZona } from 'app/Model/anagrafiche/ParticelleCatastaliZona';
import { BehaviorSubject } from 'rxjs';

export class ParticelleCatastaliZonaId extends ParticelleCatastaliZona{
    id: number;
}

@Injectable({
    providedIn:'root'
})
export class ZoneCatastoService{
    zoneParticella: ParticelleCatastaliZonaId[] = new Array();

    zoneParticellaSource = new BehaviorSubject(this.zoneParticella);
    public setZoneParticella(zoneCatasto: ParticelleCatastaliZonaId[]) {
        for (const zonaCat of zoneCatasto) {
            if (zonaCat.id == undefined || zonaCat.id < 0) {
                zonaCat.id = this.getNextId(zoneCatasto);
            }
        }
        this.zoneParticellaSource.next(zoneCatasto);
    }

    private getNextId(zonaCatasto: ParticelleCatastaliZonaId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, zonaCatasto.map(function (o) {
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

    public getZoneCatasto(): ParticelleCatastaliZonaId[] {
        return this.zoneParticellaSource.getValue();
    }

}
