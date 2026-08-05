import { Injectable } from '@angular/core';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { Copertura } from 'app/Model/metaschema/Copertura';
import { TecnicaConduzioneSuFila } from 'app/Model/metaschema/DensitaImpianto/TecnicaConduzioneSuFila';
import { TecnicaConduzioneTraFila } from 'app/Model/metaschema/DensitaImpianto/TecnicaConduzioneTraFila';
import { DettaglioVarietaPersonalizzato } from 'app/Model/metaschema/DettaglioVarietaPersonalizzato';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { Observable, map } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class CodificaInfoAggiuntiveService {
    private dettaglioVarietaPersonalizzato: DettaglioVarietaPersonalizzato[] = null;
    private capitolatoPrivato: BaseCodeDescrStr[] = null;
    private residuo: BaseCodeDescrStr[] = null;
    private pianoSemina: BaseCodeDescrStr[] = null;
    private prodotti: BaseCodeDescrStr[] = null;
    private certificazioneProdotto: BaseCodeDescrStr[] = null;

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }


    /*leggiDettaglioVarietaPersonalizzato_Old(): Promise<DettaglioVarietaPersonalizzato[]> {
        return new Promise<DettaglioVarietaPersonalizzato[]>(async (resolve, reject) => {

            if (!this.dettaglioVarietaPersonalizzato) {
                const parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    {}
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DettaglioVarietaPersonalizzato[], object>(
                    this.masterService.link_CoreWS + '/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiDettaglioVarietaPersonalizzato',
                    parametri,
                    false);

                this.dettaglioVarietaPersonalizzato = R.RispostaStringa;
                resolve(this.dettaglioVarietaPersonalizzato);
            } else {
                resolve(this.dettaglioVarietaPersonalizzato);
            }
        })

    }*/

    leggiDettaglioVarietaPersonalizzato(): Promise<DettaglioVarietaPersonalizzato[]> {
        return new Promise<DettaglioVarietaPersonalizzato[]>(async (resolve, reject) => {

            if (!this.dettaglioVarietaPersonalizzato) {
                this.ajaxAgronicaAPIService.ajaxAPIGet<object, DettaglioVarietaPersonalizzato[]>(
                    'Codifiche/LeggiDettaglioVarietaPersonalizzato',
                    {},
                    false).pipe(map(R => {
                        this.dettaglioVarietaPersonalizzato = [{ codice: '0', descrizione: '' }];
                        R.RispostaStringa.forEach(dettaglio => {
                            this.dettaglioVarietaPersonalizzato.push(dettaglio);
                        })
                        resolve(this.dettaglioVarietaPersonalizzato);
                    })).subscribe();
            } else {
                resolve(this.dettaglioVarietaPersonalizzato);
            }
        })

    }


    /*leggiCapitolatoPrivato_Old(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            if (!this.capitolatoPrivato) {
                let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    {}
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescrStr[], object>(
                    this.masterService.link_CoreWS + "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCapitolatoPrivato",
                    parametri,
                    false);

                this.capitolatoPrivato = R.RispostaStringa;
                resolve(this.capitolatoPrivato);
            } else {
                resolve(this.capitolatoPrivato)
            }
        })

    }*/

    leggiCapitolatoPrivato(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            if (!this.capitolatoPrivato) {
                this.ajaxAgronicaAPIService.ajaxAPIGet<object, BaseCodeDescrStr[]>(
                    "Codifiche/LeggiCapitolatoPrivato",
                    {},
                    false).pipe(map(R => {
                        this.capitolatoPrivato = R.RispostaStringa;
                        resolve(this.capitolatoPrivato);
                    })).subscribe();
            } else {
                resolve(this.capitolatoPrivato)
            }
        })

    }


    /*leggiResiduiDisponibili_Old(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                {}
            );
            let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescrStr[], object>(
                this.masterService.link_CoreWS + "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiResiduiDisponibili",
                parametri,
                false);

             this.residuo = R.RispostaStringa;
             resolve(this.residuo);
        })
    }*/

    leggiResiduiDisponibili(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIGet<object, BaseCodeDescrStr[]>(
                "Codifiche/LeggiResiduiDisponibili",
                {},
                false).pipe(map(R => {
                    this.residuo = R.RispostaStringa;
                    resolve(this.residuo);
            })).subscribe();
        })
    }


    /*leggiCertCommDisponibili(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                {}
            );
            let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescrStr[], object>(
                this.masterService.link_CoreWS + "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCertCommDisponibili",
                parametri,
                false);

             this.certificazioneCommerciale = R.RispostaStringa;
             resolve(this.certificazioneCommerciale);
        })
    }*/

    /*leggiCertProdDisponibili_Old(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                {}
            );
            let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescrStr[], object>(
                this.masterService.link_CoreWS + "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCertProdDisponibili",
                parametri,
                false);

             this.certificazioneProdotto = R.RispostaStringa;
             resolve(this.certificazioneProdotto);
        })
    }*/

    leggiCertProdDisponibili(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIGet<object, BaseCodeDescrStr[]>(
                "Codifiche/LeggiCertProdDisponibili",
                {},
                false).pipe(map(R => {
                    this.certificazioneProdotto = R.RispostaStringa;
                    resolve(this.certificazioneProdotto);
            })).subscribe();
        })
    }

    /*leggiPianiSemina_Old(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            if (!this.pianoSemina) {
                let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    {}
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescrStr[], object>(
                    this.masterService.link_CoreWS + "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiPianiSemina",
                    parametri,
                    false);

                R.RispostaStringa = R.RispostaStringa.map((el) => {
                    return {
                        codice: el.codice,
                        descrizione: el.codice + ' ' + el.descrizione
                    }
                });

                this.pianoSemina = R.RispostaStringa;
                resolve(this.pianoSemina);
            } else {
                resolve(this.pianoSemina);
            }
        });

    }*/

    leggiPianiSemina(): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            if (!this.pianoSemina) {

                this.ajaxAgronicaAPIService.ajaxAPIGet<object, BaseCodeDescrStr[]>(
                    "Codifiche/LeggiPianiSemina",
                    {},
                    false).pipe(map(R => {
                        R.RispostaStringa = R.RispostaStringa.map((el) => {
                            return {
                                codice: el.codice,
                                descrizione: el.codice + ' ' + el.descrizione
                            }
                        });

                        this.pianoSemina = R.RispostaStringa;
                        resolve(this.pianoSemina);
                })).subscribe();

            } else {
                resolve(this.pianoSemina);
            }
        });

    }

    leggiProdotti(piva: string): Promise<BaseCodeDescrStr[]> {
        return new Promise<BaseCodeDescrStr[]>(async (resolve, reject) => {

            var parametri = {
                Cau_Mov: "7300",
                Piva: piva,
                Sa_Cod: 0,
                Id_Destinazione: 0,
                Elem_Cod: 700,
                Flag_Negativo: false,
                RicercaTesto: "",
                RicercaTestoJArray: "",
                Flag_VisualizzaProCod: false,
                Flag_CaricaUdmCod: false,
                Pro_Cod: 0,
                Udm_Cod: 0,
                DataFiltroFormulati: new Date(),
                PUA_RegolamentoCod: 0,
                TipoRichiesto: 0,
                Flag_LeggiGiacenze: false,
                Flag_FiltraRevocati: false,
                RegolamentoCod_Operazioni: 0,
                Tipo_PuaRegolamento: 0,
                Flag_IncludiNPK_Desc: false,
                Flag_IncludiClassificazione: false,
                Flag_QtaNoZero: false,
                xFiltroAggiuntivo: "",
                Flag_Filtra_MateriePrime_Per_Piva: true,
                Flag_Filtra_MateriePrime_Pubblici: true,
                Flag_CodArticolo_In_Descrizione: false
            };

            this.ajaxAgronicaAPIService.ajaxAPIPost<object, BaseCodeDescrStr[]>(
                "AnagraficaNG/Prodotti_x_CAC_NG",
                parametri,
                false).pipe(map(R => {
                    this.prodotti = [{codice: "", descrizione: ""}];
                    R.RispostaStringa.map((p: any) => {return {codice: p.pro_cod, descrizione: p.pro_des}}).forEach(t => {
                        this.prodotti.push(t);
                    })
                    resolve(this.prodotti);
            })).subscribe();

        });

    }

    LeggiCAC_Codifica_InfoAggiuntive(filtro: LeggiCAC_Codifica_InfoAggiuntive, aggiungi_vuoto: boolean): Observable<BaseCodeDescrStr[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCAC_Codifica_InfoAggiuntive, BaseCodeDescrStr[]>(
            "Codifiche/LeggiCAC_Codifica_InfoAggiuntive",
            filtro).pipe(map(R => {
                let result = [];
                if (aggiungi_vuoto) result.push({codice: "", descrizione: ''});
                R.RispostaStringa.forEach(res => {
                    result.push(res);
                })
                return(result);
        }));
    }

}

export class LeggiCAC_Codifica_InfoAggiuntive
{
    Argomento_Cod: number;
    InfoAgg_Cod: string;
    Tipo_Codifica: number;
}

export enum enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod {
    CapitolatoPrivato = 1,
    DettaglioSpeciePersonalizzato = 2,
    CAA_Agrea = 3,
    CAA_Anagrafe = 4,
    Piano_Semina = 5,
    Revisione_Reportistica = 6,
    Residuo = 7,
    Certificazione_Prodotto = 8,
    CodificaRazzeTRACESNT = 9,
    Lavorazione = 10,
    Specifica = 11,
    ModalitaLiquidazione = 12,
    OrigineProdotto = 13
}
