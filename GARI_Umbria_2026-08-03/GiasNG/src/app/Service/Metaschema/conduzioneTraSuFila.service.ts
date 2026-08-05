import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { Copertura } from 'app/Model/metaschema/Copertura';
import { TecnicaConduzioneSuFila } from 'app/Model/metaschema/DensitaImpianto/TecnicaConduzioneSuFila';
import { TecnicaConduzioneTraFila } from 'app/Model/metaschema/DensitaImpianto/TecnicaConduzioneTraFila';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class ConduzioneSuxSpecie {
    Specie: Specie;
    Copertura: TecnicaConduzioneSuFila[];
}

export class ConduzioneTraxSpecie {
    Specie: Specie;
    Copertura: TecnicaConduzioneTraFila[];
}

export class LeggiConduzione{
    specie: Specie;
}

@Injectable({
    providedIn: 'root'
})
export class ConduzioneService {
    private ConduzioneSuxSpecie: ConduzioneSuxSpecie[] = new Array();
    private ConduzioneTraxSpecie: ConduzioneTraxSpecie[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }


    /*leggiConduzioneSu_Old(specie: Specie): Promise<TecnicaConduzioneSuFila[]> {
        return new Promise<TecnicaConduzioneSuFila[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<TecnicaConduzioneSuFila>());
            }
            if (this.ConduzioneSuxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                const parametri: CoreWS_Generic<LeggiConduzione> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { specie: specie }
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<TecnicaConduzioneSuFila[], LeggiConduzione>(
                    this.masterService.link_CoreWS + '/Metaschema/Conduzione.asmx/CaricaTecnicaConduzioneSuFila_Modello',
                    parametri,
                    false);

                this.ConduzioneSuxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                resolve(this.ConduzioneSuxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            } else {

                resolve(this.ConduzioneSuxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }*/

    leggiConduzioneSu(specie: Specie): Promise<TecnicaConduzioneSuFila[]> {
        return new Promise<TecnicaConduzioneSuFila[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<TecnicaConduzioneSuFila>());
            }
            if (this.ConduzioneSuxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiConduzione, TecnicaConduzioneSuFila[]>(
                    'MetaschemaNG/CaricaTecnicaConduzioneSuFilaModello',
                    { specie: specie },
                    false).pipe(map(R => {
                        this.ConduzioneSuxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                        resolve(this.ConduzioneSuxSpecie.find((el) => {
                            if (el.Specie.codice == specie.codice) {
                                return el;
                            }
                        }).Copertura);
                    })).subscribe();
            } else {

                resolve(this.ConduzioneSuxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }
    
    /*leggiConduzioneTra_Old(specie: Specie): Promise<TecnicaConduzioneTraFila[]> {
        return new Promise<TecnicaConduzioneTraFila[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<TecnicaConduzioneTraFila>());
            }
            if (this.ConduzioneTraxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                const parametri: CoreWS_Generic<LeggiConduzione> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { specie: specie }
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<TecnicaConduzioneTraFila[], LeggiConduzione>(
                    this.masterService.link_CoreWS + '/Metaschema/Conduzione.asmx/CaricaTecnicaConduzioneTraFila_Modello',
                    parametri,
                    false);

                this.ConduzioneTraxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                resolve(this.ConduzioneTraxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            } else {

                resolve(this.ConduzioneTraxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }*/

    leggiConduzioneTra(specie: Specie): Promise<TecnicaConduzioneTraFila[]> {
        return new Promise<TecnicaConduzioneTraFila[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<TecnicaConduzioneTraFila>());
            }
            if (this.ConduzioneTraxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiConduzione, TecnicaConduzioneTraFila[]>(
                    'MetaschemaNG/CaricaTecnicaConduzioneTraFilaModello',
                    { specie: specie },
                    false).pipe(map(R => {
                        this.ConduzioneTraxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                        resolve(this.ConduzioneTraxSpecie.find((el) => {
                        if (el.Specie.codice == specie.codice) {
                            return el;
                        }
                    }).Copertura);
                })).subscribe();

            } else {

                resolve(this.ConduzioneTraxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }


}
