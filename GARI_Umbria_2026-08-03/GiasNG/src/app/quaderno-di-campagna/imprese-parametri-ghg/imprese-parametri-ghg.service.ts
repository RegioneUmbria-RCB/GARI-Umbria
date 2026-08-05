import { Injectable } from '@angular/core';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { BehaviorSubject, Observable, from, map, of } from 'rxjs';

export class ImpreseParametriGHGForm {
    Piva: number;
    Specie: string;
    Varieta: string;
    Regolamento: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    EEC: number;
}

export class ImpreseParametriGHGServerResult {
    ID: number;
    Piva: string;
    Rag_Soc: string;
    Specie: number;
    Veg_Des: string;
    Varieta: number;
    Cul_Des: string;
    Regolamento: number;
    Reg_Des: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    EEC: number;
}

@Injectable({
    providedIn:'root'
})
export class ImpreseParametriGHGService{
    impreseParametriGHG: ImpreseParametriGHGForm[] = new Array();

    impreseParametriGHGSource = new BehaviorSubject(this.impreseParametriGHG);

    impreseParametriGHGGridRows: any[] = []

    impreseParametriGHGGridRowsSource = new BehaviorSubject(this.impreseParametriGHGGridRows);

    impresaSelezionata: BaseCodeDescrStr = {codice:"", descrizione:""};

    impresaSelezionataSource = new BehaviorSubject(this.impresaSelezionata);

    filtroRicerca: GHGFiltroRicerca = {
        piva: "",
        specie: [],
        culCod: 0,
        regolamentoCod: 0,
        dataValiditaInizio: AGRODATAINIZIO,
        dataValiditaFine: AGRODATAFINE
    };

    filtroRicercaSource = new BehaviorSubject(this.filtroRicerca);

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService){

    }

    public setImpreseParametriGHG(form: ImpreseParametriGHGForm[]) {
        this.impreseParametriGHGSource.next(form);
    }

    public getImpreseParametriGHG(): ImpreseParametriGHGForm[] {
        return this.impreseParametriGHGSource.getValue();
    }

    public setImpresaSelezionata(impresa: BaseCodeDescrStr) {
        this.impresaSelezionataSource.next(impresa);
    }

    public getImpresaSelezionata(): BaseCodeDescrStr {
        return this.impresaSelezionataSource.getValue();
    }


    public setImpreseParametriGHGGridRows(rows: any[]) {
        this.impreseParametriGHGGridRowsSource.next(rows);
    }

    public getImpreseParametriGHGGridRows(): any[] {
        return this.impreseParametriGHGGridRowsSource.getValue();
    }

    public setFiltroRicerca(filtro) {
        this.filtroRicercaSource.next(filtro);
    }

    public getFiltroRicerca() {
        return this.filtroRicercaSource.getValue();
    }

    public caricaImpreseParametriGHG(): Observable<ImpreseParametriGHGServerResult[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, ImpreseParametriGHGServerResult[]>('AnagraficaNG/LeggiImpreseParametri', this.getFiltroRicerca()).pipe(map(R => {
            return R.RispostaStringa;
        }));
    }

    public scriviImpreseParametriGHG(oggetto: any): Observable<boolean> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, boolean>('AnagraficaNG/ScriviImpreseParametri', oggetto).pipe(map(R => {
            return R.RispostaOK;
        }))
    }

    public modificaImpreseParametriGHG(oggetto: any): Observable<boolean> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, boolean>('AnagraficaNG/ModificaImpreseParametri', oggetto).pipe(map(R => {
            return R.RispostaOK;
        }))
    }

    public cancellaImpreseParametriGHG(oggetto: any): Observable<boolean> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, boolean>('AnagraficaNG/CancellaImpreseParametri', oggetto).pipe(map(R => {
            return R.RispostaOK;
        }))
    }

}

class GHGFiltroRicerca {
    piva: string = "";
    specie: number[] = [];
    culCod: number = 0;
    regolamentoCod: number = 0;
    dataValiditaInizio: Date = AGRODATAINIZIO;
    dataValiditaFine: Date = AGRODATAFINE;
}