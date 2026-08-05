import { Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { CatastoCampo } from 'app/Model/anagrafiche/CatastoCampo';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { BehaviorSubject, Observable, from, map, of } from 'rxjs';

export class ConsultaSincroForm {
    Chiave: number;
    Utente: string;
    Azienda: string;
    Data_Sincro: Date;
    Dati: string;
    Tipo_Dato: string
    Descrizione: string
    Tipo_Aggiornamento: string
    Stato_Applicazione: string
    Errori: string
}

export class ConsultaSincroServerResult {
    utente: string;
    azienda_cod: string;
    azienda_des: string;
    data_sincro: Date;
    Dati: string;
    tipo_dato: number;
    Riferimento: string;
    descrizione: string;
    tipo_aggiornamento: string;
    stato_applicazione: number;
    errori: string;
}

@Injectable({
    providedIn: 'root'
})
export class ConsultaSincroService {
    consultaSincro: ConsultaSincroForm[] = new Array();

    consultaSincroSource = new BehaviorSubject(this.consultaSincro);

    filtroRicerca: any = {};

    filtroRicercaSource = new BehaviorSubject(this.filtroRicerca);

    dettagliAggiuntivi: boolean = false;

    dettagliAggiuntiviSource = new BehaviorSubject(this.dettagliAggiuntivi);

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {

    }

    public setCatastoCampo(particelle: ConsultaSincroForm[]) {
        this.consultaSincroSource.next(particelle);
    }

    public getCatastoCampo(): ConsultaSincroForm[] {
        return this.consultaSincroSource.getValue();
    }

    public setFiltroRicerca(filtro) {
        this.filtroRicercaSource.next(filtro);
    }

    private getFiltroRicerca() {
        return this.filtroRicercaSource.getValue();
    }

    public caricaSincroLog(): Observable<ConsultaSincroServerResult[]> {
        const consoleCaricaSincroLog = '--- caricaSincroLog';
        // console.log(consoleCaricaSincroLog, 'inizio');
        console.time(consoleCaricaSincroLog);
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('Sincro/ConsultaSincroDatiApp', this.getFiltroRicerca()).pipe(map(R => {
            console.log(consoleCaricaSincroLog, 'rk:', R.RispostaStringa.length);
            console.timeEnd(consoleCaricaSincroLog);
            // console.log(consoleCaricaSincroLog, 'fine');
            return R.RispostaStringa;
        }));
    }

    public caricaLogInterscambio(): Observable<ConsultaSincroServerResult[]> {
        const consoleCaricaLogInterscambio = '--- caricaSincroLog';
        // console.log(consoleCaricaSincroLog, 'inizio');
        console.time(consoleCaricaLogInterscambio);
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('Sincro/ConsultaLogInterscambio', this.getFiltroRicerca()).pipe(map(R => {
            console.log(consoleCaricaLogInterscambio, 'rk:', R.RispostaStringa.length);
            console.timeEnd(consoleCaricaLogInterscambio);
            // console.log(consoleCaricaSincroLog, 'fine');
            return R.RispostaStringa;
        }));
    }

    public controllaModalitaDemetra(): Observable<boolean> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<string, boolean>('Sincro/ControllaModalitaDemetra', "").pipe(map(R => {
            return R.RispostaStringa;
        }));
    }

    public getDettagliAggiuntivi() {
        return this.dettagliAggiuntiviSource.getValue();
    }

    public setDettagliAggiuntivi(val: boolean) {
        this.dettagliAggiuntiviSource.next(val);
    }
}

/**
Attenzione: questi enum sono da tenere aggiornati con la controparte lato server!
Inoltre, ad ogni valore di questo enum corrisponde una traduzione in src/assets/i18n/it.json, da aggiornare di conseguenza.
Su consulta-sincro-dati-app.component.ts, invece, è da aggiungere una riga per ogni nuovo enum nella dropdown di selezione, inizializzata nella funzione "openDdl".
*/
export enum enum_Dati_App {
    Imprese = 1,
    Appezzamenti = 4,
    OperazioniDiCampagna = 10,
    OreMacchineCdG = 11,
    Ricette = 12,
    RilieviEVisiteConRilievi = 20,
    Visite = 30,
    DocumentiFileFotoFilmatiAudio = 40,
    MovimentiDiMagazzino = 50,
    Acquisti = 51,
    ManutenzioniMacchine = 60,
    AttivitaZoo = 70,
    PianoCampionamento = 80,
    AttivitaDemetra = 90,
    RicetteDemetra = 112,
};

export enum enum_Stato_Applicazione {
    NonImportato = 0,
    Importato = 1,
    ImportatoBrogliaccio = 2,
    ImportatoQuaderno = 3
};

export enum enum_Esito {
    OK = 0,
    KO = 1,
    BLK = 2
};

export enum enum_Tipo_Aggiornamento {
    Aggiornamento = 0,
    Cancellazione = 1,
    Inserimento = 2
};

export enum enum_Filtro_Importati {
    Importati = 0,
    Non_Importati = 1,
    Tutti = 2
}

export enum enum_Import_App {
    Nessuno = 0, // Solo frontiera
    Parziale = 1, // x AgroGSB
    Completo = 2, // Master APP
}

