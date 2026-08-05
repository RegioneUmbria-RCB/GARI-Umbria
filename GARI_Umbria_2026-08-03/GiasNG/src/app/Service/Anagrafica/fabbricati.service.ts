import { Injectable } from '@angular/core';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {lastValueFrom, map, of, switchMap, take} from "rxjs";
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {DettaglioTrattamento} from "../../Model/attivita/dettagli/DettaglioTrattamento";
import {DettaglioFertilizzazione} from "../../Model/attivita/dettagli/DettaglioFertilizzazione";
import {DettaglioSemina} from "../../Model/attivita/dettagli/DettaglioSemina";

export class LeggiMagazzini_QdC{

    lavorazione: Lavorazione;

    impresa: Impresa;

    data: Date;
}

export class LeggiUltimo_Magazzino_Prodotto_Movimentato{

    impresa: Impresa;

    dettaglioTrattamento: DettaglioTrattamento;

    dettaglioFertilizzazione: DettaglioFertilizzazione;

    dettaglioSemina: DettaglioSemina;

    data: Date;
}

@Injectable({
    providedIn: 'root'
})

export class FabbricatiService {

    constructor(private masterService: MasterService,
                private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

    /*Leggi_Magazzini_Old(p: LeggiMagazzini_QdC) {

        return new Promise<Fabbricato[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiMagazzini_QdC> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Fabbricato[], LeggiMagazzini_QdC>(this.masterService.link_CoreWS + '/Anagrafica/Fabbricati.asmx/Leggi_Magazzini_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Magazzini(p: LeggiMagazzini_QdC) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiMagazzini_QdC, Fabbricato[]>('AnagraficaNG/LeggiMagazziniQdC', p)
          .pipe(take(1), map(R => R.RispostaStringa));
    }

    /*Leggi_Fabbricati_Old(objParametri: ObjParametriAgenda) {

        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objParametri
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any[], ObjParametriAgenda>(this.masterService.link_CoreWS + 'Anagrafica/Fabbricati.asmx/Leggi_Fabbricati_Anagrafica', parametri).pipe(
            map(r => r.RispostaStringa)
        );
    }*/

    Leggi_Fabbricati(objParametri: ObjParametriAgenda) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any[]>('AnagraficaNG/Leggi_Fabbricati_Anagrafica', objParametri).pipe(
            map(r => r.RispostaStringa)
        );
    }

    // Leggi_Ultimo_Magazzino_Prodotto_Movimentato(p: LeggiUltimo_Magazzino_Prodotto_Movimentato) {
    //
    //     return new Promise<Fabbricato>(async (resolve, reject) => {
    //
    //         const parametri: CoreWS_Generic<LeggiUltimo_Magazzino_Prodotto_Movimentato> = new CoreWS_Generic
    //         (
    //             this.masterService.getCoreWSGenericObjP(),
    //             p
    //         );
    //
    //         const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Fabbricato, LeggiUltimo_Magazzino_Prodotto_Movimentato>(this.masterService.link_CoreWS + '/Anagrafica/Fabbricati.asmx/Ultimo_Magazzino_Prodotto_Movimentato', parametri);
    //
    //         resolve(R.RispostaStringa);
    //     });
    // }

    Leggi_Ultimo_Magazzino_Prodotto_Movimentato(p: LeggiUltimo_Magazzino_Prodotto_Movimentato): Promise<Fabbricato> {

        // return new Promise<Fabbricato>(async (resolve, reject) => {
        //
        //     const parametri: CoreWS_Generic<LeggiUltimo_Magazzino_Prodotto_Movimentato> = new CoreWS_Generic
        //     (
        //         this.masterService.getCoreWSGenericObjP(),
        //         p
        //     );
        //
        //     const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Fabbricato, LeggiUltimo_Magazzino_Prodotto_Movimentato>(this.masterService.link_CoreWS + '/Anagrafica/Fabbricati.asmx/Ultimo_Magazzino_Prodotto_Movimentato', parametri);
        //
        //     resolve(R.RispostaStringa);
        // });
        let url = "AnagraficaNG/Leggi_Ultimo_Magazzino_Prodotto_Movimentato";
        return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiUltimo_Magazzino_Prodotto_Movimentato, Fabbricato>(url, p).pipe(switchMap((r) => { return of(r.RispostaStringa) })))
    }

    /** Compares two objects of type Fabbricato.
     * @return True if `primaryKey` has the same values.
     */
    public Equals (curr: Fabbricato, other: Fabbricato): boolean {
        if (!other || !curr) return false;

        let piva = curr.primaryKey.centroAziendalePK.partitaIva;
        let sa_cod = curr.primaryKey.centroAziendalePK.codice;

        return other.primaryKey.centroAziendalePK.partitaIva ===  piva
               && other.primaryKey.centroAziendalePK.codice === sa_cod
               && other.primaryKey.codice === curr.primaryKey.codice;
    }

}
