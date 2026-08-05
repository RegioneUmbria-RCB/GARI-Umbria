import { Injectable } from '@angular/core';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { Localizzazione } from '../../Model/metaschema/Localizzazione';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';

export class LeggiLocalizzazioni{
    lavorazione: Lavorazione;
    dettaglioTrattamento: DettaglioTrattamento;
    disciplinare: Disciplinare;
    avversitaGruppo: AvversitaGruppo;
    specie: Specie;

}

@Injectable({
    providedIn: 'root'
})
export class LocalizzazioniService {

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private masterService: MasterService) { }

    /*LeggiLocalizzazioni_Modello_Old(p: LeggiLocalizzazioni) {
        return new Promise<rispostaStandard<Localizzazione[]>>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiLocalizzazioni> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Localizzazione[], LeggiLocalizzazioni>(
                    this.masterService.link_CoreWS + '/Metaschema/Localizzazioni.asmx/LeggiLocalizzazioni_Modello',
                    parametri);

            resolve(R);

        });
    }*/

    LeggiLocalizzazioni_Modello(p: LeggiLocalizzazioni) {
        return new Promise<rispostaStandard<Localizzazione[]>>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiLocalizzazioni, Localizzazione[]>(
                'MetaschemaNG/LeggiLocalizzazioniModello',
                p).pipe(map(R => {
                    resolve(R);
            })).subscribe();
        });
    }

}
