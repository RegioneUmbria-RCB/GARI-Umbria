import { Inject, Injectable } from '@angular/core';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { rispostaStandard } from 'app/Service/master.service';
import { CatastoFactoryService, CATASTO_SERVICE_TOKEN } from 'app/Service/ServiceFactory/catasto.factory.service';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable()
export class CatastoEditService {
    private particellaEdit: CatastoCentroAziendale = new CatastoCentroAziendale();
    particellaEditSource: BehaviorSubject<CatastoCentroAziendale> = new BehaviorSubject(this.particellaEdit);

    constructor(@Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService) {

    }

    changeParticellaEdit(item: CatastoCentroAziendale) {
        this.particellaEditSource.next(item);
    }


    getParticellaEdit(): CatastoCentroAziendale {
        return this.particellaEditSource.getValue();
    }

    leggiParticellaEdit(catasto: CatastoCentroAziendale) {
        if (catasto.centro == null || catasto.particella == null) {
            return;
        }
        this.catastoService.LeggiParticellaAzienda(catasto).subscribe((data) => {
            this.changeParticellaEdit(data);
        });
    }

    scriviParticella(oldValue: CatastoCentroAziendale, newValue: CatastoCentroAziendale): Observable<rispostaStandard<CatastoCentroAziendale>> {
        return this.catastoService.ScriviParticellaAzienda(oldValue, newValue);

        //---- vecchio controllo di alcuni errori con Alert ----
        // let controllo = this.controlloParticella(newValue);
        // if (controllo.ok) {
        //     return this.catastoService.ScriviParticellaAzienda(oldValue, newValue);
        // } else {
        //     alert(controllo.messaggio);
        //     return of(undefined);
        // }
    }

    private controlloParticella(newValue: CatastoCentroAziendale): {ok: boolean, messaggio: string} {
        let messaggio = '';

        if (newValue.centro == undefined) {
            messaggio = "E' necessario impostare un centro\n";
        }
        if (newValue.possessiParticella.length == 0) {
            messaggio = messaggio + "La particella deve aere almeno un possesso\n";
        }
        if (newValue.particella.primaryKey.Foglio == 0) {
            messaggio = messaggio + "Il campo Foglio non può essere 0\n";
        }
        if (newValue.particella.primaryKey.Numero == 0) {
            messaggio = messaggio + "Il campo Numero non può essere 0\n";
        }
        if (newValue.particella.primaryKey.Prov == '' || newValue.particella.primaryKey.Prov == '000') {
            messaggio = messaggio + "E' necessario impostare una Provincia valida\n";
        }
        if (newValue.particella.primaryKey.Com == '' || newValue.particella.primaryKey.Com == '000') {
            messaggio = messaggio + "E' necessario impostare un Comune valido\n";
        }

        return {ok: (messaggio.length == 0), messaggio: messaggio};
    }

}
