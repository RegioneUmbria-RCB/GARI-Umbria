import { Injectable } from '@angular/core';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { DoseEtichetta } from 'app/Model/metaschema/DoseEtichetta';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { MovimentoDiMagazzino } from '../api.service';

export class LeggiDosiEtichetta{

    specie: Specie;

    dettaglioTrattamento: DettaglioTrattamento;

    avversitaGruppo: AvversitaGruppo;

    impianti: Impianto[];

    disciplinare: Disciplinare;

    prodottiDaTrattare: MovimentoDiMagazzino[];

    data: Date;
}

@Injectable({
    providedIn: 'root'
})
export class DosiEtichettaService {

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private masterService: MasterService) { }

    /*Leggi_DosiEtichetta_QdC_Old(p: LeggiDosiEtichetta) {

        return new Promise<DoseEtichetta[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiDosiEtichetta> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DoseEtichetta[], LeggiDosiEtichetta>(this.masterService.link_CoreWS + '/Metaschema/DosiEtichetta.asmx/Leggi_DosiEtichetta_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_DosiEtichetta_QdC(p: LeggiDosiEtichetta) {
        return new Promise<DoseEtichetta[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDosiEtichetta, DoseEtichetta[]>('MetaschemaNG/LeggiDosiEtichettaQdC', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }
}
