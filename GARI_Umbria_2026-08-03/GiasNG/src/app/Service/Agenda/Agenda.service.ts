import { Injectable } from '@angular/core';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { Attivita, Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import { DettaglioFertilizzazione } from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import { DettaglioSemina } from 'app/Model/attivita/dettagli/DettaglioSemina';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { RisorsaAcqua } from 'app/Model/attivita/risorse/RisorsaAcqua';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { AvversitaGruppo } from 'app/Model/metaschema/avversita/AvversitaGruppo';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { DoseEtichetta } from 'app/Model/metaschema/DoseEtichetta';
import { Soglia } from 'app/Model/metaschema/Soglia';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import {
    CodiciXOperazione,
    Parametri_Aggiuntivi_Attivita,
    Parametri_Aggiuntivi_ControllaDosi
} from 'app/quaderno-di-campagna/agenda-edit/quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import {BehaviorSubject, map, Observable, take} from 'rxjs';
import { MasterService, RispostaStandard, rispostaStandard, VariabiliInSessione_NG } from '../master.service';
import { ObjParametriAgendaService } from '../obj-parametri-agenda.service';
import { UtilizzoTerreno } from "../../Model/metaschema/utilizzi/UtilizzoTerreno";
import { Epoca } from "../../Model/metaschema/Epoca";
import { BaseCodeDescr } from "../../Model/baseClass/baseCodeDescr";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { Pua } from "../../Model/metaschema/Pua";
import { Tipo_Attivita, Stati } from 'gias-ui-kit';
import { EsercizioCDC } from 'app/Model/attivita/centri_di_costo/EsercizioCDC';
import { AvversitaTrappole } from 'app/Model/attivita/dettagli/AvversitaTrappole';
import {FiltersConfig} from "../../menu-agenda/components/models";

//#region Class Export
export class Controllo_Sportello {
    Piva: string;

    Servizio_Cod: number;

    Data_Riferimento: Date;

    SportelloAperto: boolean;

    Data_Min: Date;

    Data_Max: Date;
}

export class LeggiInizializza_QdC {

    operazioni: Array<Lavorazione>;

    tipoOperazioneDB: Enum_DBTypeOperation;

    data: Date;

    impresa: Impresa;

    tipoAttivita: Tipo_Attivita;

    statoAttivita: Stati;

    tipoRicetta: Tipo_Ricetta;

    lista_Attivita: Array<Attivita>;

    codiciAttivita: CodiciXOperazione[];

    disciplinare: Disciplinare;
}

export class Inizializza_QdC {

    SportelloAperto: boolean;

    AziendaInVerifica: boolean;

    Data: Date;

    Data_Min: Date;

    Data_Max: Date;

    flagNuovoControlloRiduzioneDiserbo: boolean;

    ListaPivaAgenzie: Array<string>;

    ListaFabbricatiConUsodaTerzi: Array<Fabbricato>;

    ListaGiacenzeXProdotto: Array<GiacenzeXProdotto>;

    pua: Pua;

    PraticaTrovata: boolean;

    ListaLavCodNonGestitiSuAPP: Array<number>;

    constructor() {
        this.SportelloAperto = true;

        this.AziendaInVerifica = false;

        this.Data = new Date();

        this.Data_Min = AGRODATAINIZIO;

        this.Data_Max = AGRODATAFINE;

        this.flagNuovoControlloRiduzioneDiserbo = false;

        this.ListaPivaAgenzie = [];

        this.ListaFabbricatiConUsodaTerzi = [];

        this.ListaGiacenzeXProdotto = [];

        this.pua = null;

        this.PraticaTrovata = true;

        this.ListaLavCodNonGestitiSuAPP = [];
    }

}

export class GiacenzeXProdotto {
    Operazione: Lavorazione;
    Categoria_Magazzino: number;
    dettaglioTrattamento: DettaglioTrattamento;
    dettaglioFertilizzazione: DettaglioFertilizzazione;
    dettaglioSemina: DettaglioSemina;
    avversitaGruppo: AvversitaGruppo;
}

export class Attivita_Con_Parametri_Aggiuntivi extends Attivita {
    parametri_aggiuntivi: Parametri_Aggiuntivi_Attivita[];

    constructor() {
        super();
    }
}

export class Controllo_Inserimento_Dose_Prodotto {

    tipoAttivita: Tipo_Attivita;

    statoAttivita: Stati;

    parametri_aggiuntivi_list: Parametri_Aggiuntivi_ControllaDosi[];

    id_agenda: number;

    raccoglitore_cod: number;

    DoseConsentitaDiserbo: number;

    DosiProdottiGridrowId: number;

    risorsaAcqua: RisorsaAcqua;

    row_grid_impianti: Object[];

    row_grid_prodottiDaTrattare: Object[];

    row_grid_dosi_altre_operazioni: Object[];

    lavorazione: Lavorazione;

    dettaglioTrattamento: DettaglioTrattamento;

    dettaglioSemina: DettaglioSemina;

    dettaglioFertilizzazione: DettaglioFertilizzazione;

    unitadiMisura: UnitaDiMisura;

    avversitaGruppo: AvversitaGruppo;

    soglia_Avversita: Soglia;

    dosi_Etichetta: DoseEtichetta[];

    N: number;

    N_Utile: number;

    P: number;

    K: number;

    Cu: number;

    flagDoseQuantitaTotale: number;

    flagTipoDose: number;

    doseHa: number;

    doseHl: number;

    doseQ: number;

    quantitaTotale: number;

    Magazzino: Fabbricato;

    Lotto: string;

    data: Date;

    disciplinare: Disciplinare;

    utilizzoTerreno: UtilizzoTerreno;

    epocaFertilizzazione: Epoca;

    tipoRicetta: Tipo_Ricetta;

    sup_Trattata: number;

    sup_Calcolata: number;

    qtaProdotto_Trattata: number;

    tipo_Semina: BaseCodeDescr;

    ricetta_cod: number;

    efficienza: number;

    row_grid_dosi: Object[];

    Piva_Rif: string;

    Sa_Cod_Rif: number;

    ID_Agenda_Rif: number;

    ID_Mov_Rif: number;

    ID_Mov_Det_Rif: number;

    Lav_Cod_Rif: number;

    Cau_Mov_Rif: string;

    Des_Rif: string;

    Qta_Rif: number;

    ricetta_operazione_cod: number;

    pua: Pua;

    Magazzino_Esterno: Fabbricato;

    Categoria_Magazzino: number;

    impresa: Impresa;

    Visualizza_Magazzini_Esterni: boolean;

    Original_Row_Value: Object[];

    Magazzino_Innesco: Fabbricato;

    Lotto_Innesco: string;

    UdM_Innesco: UnitaDiMisura;

    Magazzino_Agenzia_Innesco: Fabbricato;

    DoseTot_Ha_Innesco: number;

}

export class LeggiDoseConsentitaDiserbo {

    attivita: Attivita;

    dettaglioTrattamento: DettaglioTrattamento;

    fabbricato: Fabbricato;

    lotto: string;

    avversitaGruppo: AvversitaGruppo;

}

export class DoseConsentitaDiserbo {

    D_HA_Max_Diserbo: number;

    lbl_qta_residua: string;

    Udm_Radice_HA: number;

    Lbl_Dose_Consigliata: string;

    Div_DettaglioDoseConsentita: boolean;

    Lbl_Dose_Consigliata2: string;

    percAbbDaApplicare: number;

    principiAttiviPercAbb: string;

    constructor() {
        this.D_HA_Max_Diserbo = 0;
        this.lbl_qta_residua = "";
        this.Udm_Radice_HA = 0;
        this.Lbl_Dose_Consigliata = "";
        this.Div_DettaglioDoseConsentita = false;
        this.Lbl_Dose_Consigliata2 = "";
        this.percAbbDaApplicare = 0;
        this.principiAttiviPercAbb = "";
    }

}

export class Ricetta_Operazione {
    constructor(
        public ricetta_cod?: number,
        public ricetta_operazione_cod?: number,
        public data?: Date,
        public in_uso?: number,
        public app_ricetta_operazione_id?: string
    ) { }
}

export class APP_Ricetta_Operazione {
    W_Anagrafica_Stati_Cod: number;
    Ricetta_Cod: number;
    Ricetta_Operazione_Cod: number;
    Ricetta_Operazione_Cod_RIF: number;
    Ricetta_Operazione_Des: string;
    Note: string;
    Lav_Cod: number;
    Extra_Int: number;
    Mezzo: number;
    Bozza: number;
}

export class LeggiLink_Operazione {
    id_agenda: string;
    codiceRicetta: string;
    codiceOperazioneRicetta: string;
    tipo_operazione: number;
    impresa: Impresa;
    centroaziendale: CentroAziendale;
    specie: Specie;
    data: Date;
    lavorazione: Lavorazione;
    tipo: Tipo_Attivita;
    tiporicetta: Tipo_Ricetta;
    stato: Stati;
    variabiliInSessione_NG: VariabiliInSessione_NG;
}

export class Link_Operazione {
    Operazione: Lavorazione;
    Link: string;
}

export class LeggiDefault_DPI_QdC {
    operazioni: Lavorazione[];
    impianti: Impianto[];
    disciplinari: Disciplinare[];
    tipoOperazioneDB: Enum_DBTypeOperation;
    data: Date;
    impresa: Impresa;
    tipoRicetta: Tipo_Ricetta;
    tipoAttivita: Tipo_Attivita;
    codiciAttivita: CodiciXOperazione[];
}

export class ScriviListaAttivita {
    attivita_list: Attivita[];
    parametri_aggiuntivi_list: Parametri_Aggiuntivi_Attivita[];
}

export class ControllaMassimali_QdC {
    Qta_Ha_Distribuibile_Prodotto: number;
    Qta_Ha_Distribuibile_Prodotto_Ricette: number;
}

export class LeggiMenuRicette {
    codiciAttivita: CodiciXOperazione[];
}

export class MenuRicette {
    tipo: number;
    des: string;
    codiciAttivita: CodiciXOperazione[];
    data: Date;
    ribaltata: boolean;
    inviata_ad_app: boolean;
    raccoglitore_cod: number;
}

export class AggiungiRicettaAlPUA {
    data: string;
    id_agenda_checked: string;
    id_agenda: string;
    lav_cod_checked: string;
    lav_cod: string;
    piva: string;
    sa_cod: string;
    ricetta_cod: string;
}

export class LeggiDisponibilitaAttualeFertilizzante {
    dettaglioFertilizzazione: DettaglioFertilizzazione;
    pua: Pua;
    data: Date;
}

export class LeggiPUA {
    impresa: Impresa;
    data: Date;
    operazioni: Array<Lavorazione>;
    tipo_Attivita: Tipo_Attivita;
    tipo_Ricetta: Tipo_Ricetta;
    stato: Stati;
    disciplinare: Disciplinare;
}

export class Leggi_Modalita_Applicazione {
    Operazione: Lavorazione;
}

export class Leggi_Numero_Trappole {
  eserciziCDC: EsercizioCDC[];
  avversitaGruppo: AvversitaGruppo;
}

export class CaricaOperazioni{
  filtro: string;
  piva: string;
}

//#end region
//#region Agenda Service
@Injectable({
    providedIn: 'root'
})
export class AgendaService {

    private Elenco_Link_OperazioniSource = new BehaviorSubject([]);
    currentElenco_Link_Operazioni = this.Elenco_Link_OperazioniSource.asObservable();

    constructor(
        private masterService: MasterService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService
    ) { }

    private changeElenco_Link_Operazioni(link_operazione: Link_Operazione[]): void {
        this.Elenco_Link_OperazioniSource.next(link_operazione);
    }

    private getCurrentElenco_Link_Operazioni(): Link_Operazione[] {
        return this.Elenco_Link_OperazioniSource.getValue();
    }

    ScriviListaAttivita(p: ScriviListaAttivita) {
        return new Promise<rispostaStandard<Attivita[]>>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/ScriviListaAttivitaToRaccoglitore', p, true).pipe(map(R => {
                resolve(R);
            }), take(1)).subscribe();
        });
    }

    // CheckListaAttivitaWS(listaAttivita: Attivita[]) {
    //
    //     return new Promise<RispostaStandard>(async (resolve, reject) => {
    //
    //         const parametri: CoreWS_Generic<any> = new CoreWS_Generic
    //         (
    //             this.masterService.getCoreWSGenericObjP(),
    //             listaAttivita
    //         );
    //
    //         const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<any, RispostaStandard>(this.masterService.link_CoreWS + '/Agenda/Agenda.asmx/CheckListaAttivita', parametri);
    //
    //         resolve(R);
    //     });
    //
    // }

    CheckActivityCompliance(p: ScriviListaAttivita) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/VerificaConformitaAttivita', p, true)
            .pipe(take(1));
    }


    LeggiListaAttivitaWS(piva: String, id_agenda: Number) {
        return new Promise<Attivita[]>(async (resolve, reject) => {

            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/LeggiListaAttivitaDaAgenda', { piva: piva, Id_Agenda: id_agenda }, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        }); //IMPORTANTE
    }

    //Legge una ricetta e la restituisce di tipo Quaderno di Campagna (utilizata per il ribaltamento da brogliaccio ad agenda)
    LeggiListaAttivitaDaRicettaOperazionePerAgendaWS(Ricetta_Operazione_Cod: number) {
        return new Promise<Attivita[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/LeggiListaAttivitaDaRicettaOperazionePerAgenda', { Ricetta_Operazione_Cod: Ricetta_Operazione_Cod }, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    //Legge una ricetta e la restituisce di tipo Brogliaccio (utilizata per il ribaltamento da ricetta a brogliaccio)
    LeggiListaAttivitaDaRicettaOperazionePerBrogliaccioWS(Ricetta_Operazione_Cod: number) {
        return new Promise<Attivita[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/LeggiListaAttivitaDaRicettaOperazionePerBrogliaccio', { Ricetta_Operazione_Cod: Ricetta_Operazione_Cod }, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    //Legge una ricetta o brogliaccio
    LeggiListaAttivitaDaRicettaOperazionePerRicettaWS(Ricetta_Operazione_Cod: number) {
        return new Promise<Attivita[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Attivita[]>('Agenda/LeggiListaAttivitaDaRicettaOperazionePerRicetta', { Ricetta_Operazione_Cod: Ricetta_Operazione_Cod }, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    // LeggiAttivitaAPI(piva: String, id_agenda: Number) {
    //
    //     return new Promise<any>(async (resolve, reject) => {
    //         const parametri: object = new Object (
    //             {piva, id_agenda}
    //         );
    //         //const r = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreAPI + '/Agenda/AttivitaDaAgenda', parametri);
    //         const r = await this.ajaxAgronicaService.ajaxAgronica('http://192.168.1.106:5000/Agenda/AttivitaDaAgenda', parametri);
    //
    //         resolve(r.RispostaStringa);
    //     });
    //
    // }

    Controllo_Sportello(p: Controllo_Sportello) {
        return new Promise<Controllo_Sportello>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<Controllo_Sportello, Controllo_Sportello>('Agenda/Controllo_Sportello', p, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    Inizializza_QdC(p: LeggiInizializza_QdC) {
        return new Promise<rispostaStandard<Inizializza_QdC>>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiInizializza_QdC, Inizializza_QdC>('Agenda/Inizializza_QdC_NG', p, true).pipe(map(R => {
                resolve(R);
            }), take(1)).subscribe();
        });
    }

    Controllo_Inserimento_Dose_Prodotto(p: Controllo_Inserimento_Dose_Prodotto) {
        return new Promise<any>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<Controllo_Inserimento_Dose_Prodotto, RispostaStandard>('Agenda/Controllo_Inserimento_Dose_Prodotto', p, true).pipe(map(R => {
                resolve(R);
            }), take(1)).subscribe();
        });
    }

    Imposta_DoseConsentitaDiserbo(p: LeggiDoseConsentitaDiserbo) {
        return new Promise<DoseConsentitaDiserbo>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDoseConsentitaDiserbo, DoseConsentitaDiserbo>('Agenda/Imposta_DoseConsentitaDiserbo', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    CaricaRicetteOperazioni(Ricetta_Operazione_Cod: number, Ricetta_Cod: number) {
        return new Promise<Ricetta_Operazione[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, Ricetta_Operazione[]>('Agenda/CaricaRicetteOperazioni', { Ricetta_Operazione_Cod: Ricetta_Operazione_Cod, Ricetta_Cod: Ricetta_Cod }).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    CaricaMenuRicette(p: LeggiMenuRicette) {
        return new Promise<MenuRicette[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiMenuRicette, MenuRicette[]>('Agenda/CaricaMenuRicette', p, true).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    Redirect_In_Base_Al_Lav_Cod(p: LeggiLink_Operazione) {
        return new Promise<string>(async (resolve, reject) => {
            let Elenco_Link_Operazioni: Link_Operazione[] = this.getCurrentElenco_Link_Operazioni();
            let link: string = "";

            if (Elenco_Link_Operazioni && Elenco_Link_Operazioni.length > 0) {
                let index: number = Elenco_Link_Operazioni
                    .findIndex(l => l.Operazione.primaryKey.codice === p.lavorazione.primaryKey.codice);
                if (index > -1)
                    link = Elenco_Link_Operazioni[index].Link;
            }

            if (link === "") {
                p.variabiliInSessione_NG = this.masterService.variabiliInSessione;
                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiLink_Operazione, string>('Agenda/Redirect_In_Base_Al_Lav_Cod', p, true)
                    .pipe(map(R => {
                        link = R.RispostaStringa;
                        Elenco_Link_Operazioni.push({
                            Operazione: p.lavorazione,
                            Link: link
                        });

                        this.changeElenco_Link_Operazioni(Elenco_Link_Operazioni);
                        resolve(link);
                    }), take(1)).subscribe();
            } else {
                resolve(link);
            }
        });
    }

    Default_DPI_QdC(p: LeggiDefault_DPI_QdC) {
        return new Promise<Disciplinare>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDefault_DPI_QdC, Disciplinare>('Agenda/Default_DPI_QdC_NG', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    Ricetta_Numero_Default(piva: string, data_operazione: string) {
        return new Promise<string>(async (resolve, reject) => {
            const parametri: any = {
                piva: piva,
                data_operazione: data_operazione
            };
            this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('Agenda/ricetta_numero_default', parametri).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    ControllaMassimali(p: Controllo_Inserimento_Dose_Prodotto) {
        return new Promise<ControllaMassimali_QdC>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<Controllo_Inserimento_Dose_Prodotto, ControllaMassimali_QdC>('Agenda/ControllaMassimali', p).pipe(map(R => {
                resolve(R.RispostaStringa);
            }), take(1)).subscribe();
        });
    }

    public Aggiungi_Ricetta_Al_PUA_MenuAgenda(p: AggiungiRicettaAlPUA) {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<AggiungiRicettaAlPUA, RispostaStandard>('Agenda/Menu/Aggiungi_Al_PUA', p, true);
        return Obs;
    }

    public Recupera_Pua(p: LeggiPUA) {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiPUA, Pua>('Agenda/Recupera_Pua', p);
        return Obs;
    }

    public CaricaDisponibilitaAttualeFertilizzante(p: LeggiDisponibilitaAttualeFertilizzante) {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiDisponibilitaAttualeFertilizzante, number>('Agenda/CaricaDisponibilitaAttualeFertilizzante', p);
        return Obs;
    }

    public Leggi_Modalita_Applicazione_QdC(p: Leggi_Modalita_Applicazione) {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<Leggi_Modalita_Applicazione, Array<BaseCodeDescr>>('Agenda/Leggi_Modalita_Applicazione_QdC', p);
        return Obs;
    }

    public Controlla_Giacenza_Fertilizzanti(p: ScriviListaAttivita) {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviListaAttivita, string>('Agenda/Controlla_Giacenza_Fertilizzanti', p, true);
        return Obs;
    }

    public Ottieni_Numero_Trappole_Registrate(p: Leggi_Numero_Trappole) {
      const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<Leggi_Numero_Trappole, AvversitaTrappole[]>('Agenda/Ottieni_Numero_Trappole_Registrate', p, true);
      return Obs;
    }

    public CaricaGridTrappole(p: CaricaOperazioni): Observable<unknown> {
      const Obs =  this.ajaxAgronicaAPIService.ajaxAPIPost<CaricaOperazioni, any>('Agenda/CaricaTrappole', p);
      return Obs;
    }

}
