import { Injectable } from '@angular/core';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { DettaglioFertilizzazione } from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { DettaglioSemina } from 'app/Model/attivita/dettagli/DettaglioSemina';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { Epoca } from 'app/Model/metaschema/Epoca';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import {forkJoin, lastValueFrom, map} from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { GruppoFinalita } from 'app/Model/metaschema/utilizzi/GruppoFinalita';
import {Impresa} from "../../Model/anagrafiche/Impresa";
import {Observable} from "rxjs";
import {take} from "rxjs/operators";
import {Prodotto} from "../../Model/attivita/risorse/Prodotto";
import {Pua} from "../../Model/metaschema/Pua";
import { MovimentoDiMagazzino } from '../api.service';
import { Tipo_Attivita, Stati } from 'gias-ui-kit';

export class LeggiProdotti{
    impresa?: Impresa;
    tipoAttivita: Tipo_Attivita;

    statoAttivita: Stati;

    lavorazione: Lavorazione;

    impianti: Array<Impianto>;
    prodottiDaTrattare: Array<MovimentoDiMagazzino>;

    specie: Specie;

    varieta?: Varieta;
    regolamento?: Regolamenti;
    finalita?: GruppoFinalita;
    disciplinare: Disciplinare;

    epocaDPI: Epoca;

    avversitaGruppo: AvversitaGruppo;

    filtroPerDescrizione: string;

    data: Date;

    escludiGiacenzeZero: boolean;

    magazziniAgenzie: boolean;

    magazziniEsterni: boolean;

    pua?: Pua;
}

export class Crea_Prodotti {
    varieta: Varieta;

    regolamento: Regolamenti;

    creaSemente: boolean;

    creaTrasformatoVegetale: boolean;
}

@Injectable({
    providedIn: 'root'
})

export class ProdottiService{

    constructor(private masterService: MasterService,
                private ajaxAgronicaService: AjaxAgronicaService,
                private APIService: AjaxAgronicaAPIService) {}


    /*Leggi_Formulati_Old(p: LeggiProdotti) {

        return new Promise<DettaglioTrattamento[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProdotti> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DettaglioTrattamento[], LeggiProdotti>(this.masterService.link_CoreWS + '/Anagrafica/Prodotti.asmx/Leggi_Formulati_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Formulati(p: LeggiProdotti) {

        return new Promise<DettaglioTrattamento[]>(async (resolve, reject) => {

            this.APIService.ajaxAPIPost<LeggiProdotti, DettaglioTrattamento[]>('AnagraficaNG/LeggiFormulatiQdC', p,true).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();

        });
    }


    /*Leggi_Fertilizzanti_Old(p: LeggiProdotti) {

        return new Promise<DettaglioFertilizzazione[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProdotti> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DettaglioFertilizzazione[], LeggiProdotti>(this.masterService.link_CoreWS + '/Anagrafica/Prodotti.asmx/Leggi_Fertilizzanti_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Fertilizzanti(p: LeggiProdotti) {

        return new Promise<DettaglioFertilizzazione[]>(async (resolve, reject) => {

            this.APIService.ajaxAPIPost<LeggiProdotti, DettaglioFertilizzazione[]>('AnagraficaNG/LeggiFertilizzantiQdC', p,true).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();

        });
    }

    /*Leggi_Sementi_Old(p: LeggiProdotti) {

        return new Promise<DettaglioSemina[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProdotti> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DettaglioSemina[], LeggiProdotti>(this.masterService.link_CoreWS + '/Anagrafica/Prodotti.asmx/Leggi_Sementi_QdC', parametri);

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Sementi(p: LeggiProdotti) {

        return new Promise<DettaglioSemina[]>(async (resolve, reject) => {

            this.APIService.ajaxAPIPost<LeggiProdotti, DettaglioSemina[]>('AnagraficaNG/LeggiSementiQdC', p,true).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();

        });
    }

    /*Leggi_Trasformati_Vegetali_Old(p: LeggiProdotti) {
        const linkProdotti = '/Anagrafica/Prodotti.asmx/';

        return new Promise<DettaglioSemina[]>(async (resolve, reject) => {

            const parametri: CoreWS_Generic<LeggiProdotti> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DettaglioRaccolta[], LeggiProdotti>(
                this.masterService.link_CoreWS + linkProdotti + 'Leggi_TrasformatiVegetali_QdC',
                parametri
            );

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_Trasformati_Vegetali_Qdc(p: LeggiProdotti) {
        return new Promise<DettaglioRaccolta[]>(async (resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiProdotti, DettaglioRaccolta[]>(
                'AnagraficaNG/LeggiTrasformatiVegetaliQdC',
                p
            ).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    Leggi_Trasformati_Vegetali_Anagrafica(p: LeggiProdotti): Observable<DettaglioRaccolta[]> {
        const linkProdotti = 'AnagraficaNG/LeggiTrasformatiVegetaliAnagrafica';
        return this.APIService
        .ajaxAPIPost<LeggiProdotti, DettaglioRaccolta[]>(linkProdotti, p)
        .pipe(
            take(1),
            map(r => r.RispostaStringa)
        );
    }

    Leggi_Trasformati_Vegetali_Anagrafica_With_Default(p: LeggiProdotti): Observable<Prodotto[]> {
        const defaultVarietaPayload = {...p, finalita: { codice: 0 }} as LeggiProdotti;

        const linkProdotti = 'AnagraficaNG/LeggiTrasformatiVegetaliAnagrafica';
        return forkJoin([
            this.APIService.ajaxAPIPost<LeggiProdotti, Prodotto[]>(linkProdotti, p),
            this.APIService.ajaxAPIPost<LeggiProdotti, Prodotto[]>(linkProdotti, defaultVarietaPayload),
        ])
        .pipe(
            take(1),
            map(([r1, r2]) =>
             {
                 let a = ProdottiService.getDistinctTrasformati(r1.RispostaStringa.concat(r2.RispostaStringa))
                 a.unshift({
                     "codice": 0,
                     "descrizione": "Nessuno",
                     "codice_alfanumerico": "",
                     "elemCod": 210,
                     "finalita": {
                         "specieCod": 0,
                         "codice": 0,
                         "descrizione": ""
                     },
                     "regolamento": {
                         "codice": 1,
                         "descrizione": ""
                     },
                     "specie": null,
                     "tipo": null,
                     "unitaDiMisura": null,
                     "varieta": {
                         "specie": {
                             "codice": 52,
                             "descrizione": "Pomodoro"
                         },
                         "classType": "Varieta",
                         "codice": 0,
                         "descrizione": ""
                     }
                 })
                 return a;
             }
            )
        );
    }

    Crea_Prodotti(p: Crea_Prodotti) {
        const url = 'AnagraficaNG/CreaProdotti';
        return lastValueFrom(this.APIService.ajaxAPIPost<Crea_Prodotti, any>(url, p, true))
    }

    public LeggiInsetti(i: LeggiProdotti) {
        const url = 'Anagrafica/Insetti';

        return new Promise((resolve, reject) => {
            this.APIService.ajaxAPIPost<LeggiProdotti, DettaglioTrattamento[]>(url, i, true)
                .GiasSubscribe(R => {
                    console.log(R);
                    resolve(R.RispostaStringa);
                });
        })
    }

    private static getDistinctTrasformati(trasformati: Prodotto[]): Prodotto[] {
        const result: Prodotto[] = [];
        for (const trasformato of trasformati) {
            if (result.find(t => t.codice == trasformato.codice) == null) {
                result.push(trasformato);
            }
        }

        return result;
    }
}
