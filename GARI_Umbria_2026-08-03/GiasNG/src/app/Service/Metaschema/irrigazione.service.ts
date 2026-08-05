import { Injectable } from '@angular/core';
import { Irrigazione } from 'app/Model/metaschema/Irrigazione';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Observable, map, of, tap, take } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';

export class IrrigazionexSpecie {
    Specie: Specie;
    Irrigazione: Irrigazione[];
}

export class LeggiIrrigazione{
    specie: Specie;
}

export class Legg_Macchine_Irrigazione{
    piva: string;
}

@Injectable({
    providedIn: 'root'
})
export class IrrigazioneService {
    private IrrigazionexSpecie: IrrigazionexSpecie[] = new Array();

    constructor(
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    ) { }

    leggi(specie: Specie): Observable<Irrigazione[]> {
        if (specie.codice == 0) {
            return of(new Array<Irrigazione>());
        } else if (this.IrrigazionexSpecie.find((el) => el.Specie.codice == specie.codice)) {
            return of(this.IrrigazionexSpecie.find((el) => el.Specie.codice == specie.codice).Irrigazione);
        } else {
            // If specie is -1, treat it as 0 to load all irrigations
            if (specie.codice == -1) specie = new Specie(0);
            return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiIrrigazione, Irrigazione[]>(
                'MetaschemaNG/CaricaImpiantiIrrigazioniModello',
                { specie: specie },
                false
            ).pipe(
                tap(R => this.IrrigazionexSpecie.push({ Specie: specie, Irrigazione: R.RispostaStringa })),
                map(() => this.IrrigazionexSpecie.find((el) => el.Specie.codice == specie.codice).Irrigazione)
            );
        }
    }

    leggiMacchineIrrigazione(piva: string): Observable<any[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<Legg_Macchine_Irrigazione, any[]>(
                'AnagraficaNG/LeggiMacchineIrrigazione',
                { piva: piva },
                false
            ).pipe(take(1), map(R => {
                console.log(R.RispostaStringa);
                return R.RispostaOK ?  R.RispostaStringa : [];
            }));
    }

}
