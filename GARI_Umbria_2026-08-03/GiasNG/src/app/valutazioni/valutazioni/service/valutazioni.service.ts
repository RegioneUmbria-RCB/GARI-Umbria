import { Injectable } from '@angular/core';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { HttpService } from 'app/Service/http.service';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
import { Observable, of, tap, switchMap, BehaviorSubject } from 'rxjs';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { PKValutazioneTestata, ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import { GridPublicService } from 'gias-kendo-grid';
import { PKValutazioneTestataxAnno, ValutazioneTestataxAnno } from 'app/Model/valutazioni/ValutazioneTestataxAnno';
import { SessionStorageService } from 'ngx-webstorage';
import { saveAs } from 'file-saver';

export class LeggiGriglia{
    jsonGriglia : string;

    constructor(jsonGriglia: string) {
        this.jsonGriglia = jsonGriglia;
    }
}

export class LeggiTestata {
    piva: string | undefined;
    idTestata: number | undefined;
    includiAnno: boolean | undefined;
}

export class LeggiDettaglio {
    piva: string | undefined;
    idTestata: number | undefined;
    ContoCod: number | undefined;
}

export class LeggiPianoConti {
    piva: string | undefined;
    pianoCod: number | undefined;
}


export class RemovedYears {

    constructor(year: string, flag: boolean) {
        this.year = year;
        this.flag = flag;
    }

    year: string;
    flag: boolean;
}

export class ValutazioneExcel {
    public FileName : string;
    public Extension: string;
    public Data: string;
}

@Injectable({ providedIn: 'root' })
export class ValutazioniService {
    private leggiTestata: LeggiTestata = new (LeggiTestata);

    private leggiTestataSource = new BehaviorSubject(this.leggiTestata);
    currentLeggiTestata: Observable<LeggiTestata> = this.leggiTestataSource.asObservable();

    gridPublicService: GridPublicService;
    removedYearsObjects: RemovedYears[] = [];

    public yearsList: string[] = ["2020", "2021", "2022", "2023", "2024", "2025", "2026", "2027", "2028", "2029", "2030", "2031", "2032", "2033", "2034", "2035", "2036", "2037", "2038", "2039", "2040"];

    constructor(private apiService: AjaxAgronicaAPIService,
        protected masterService: MasterService,
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxApiService: AjaxAgronicaAPIService,
        protected httpService: HttpService,
        private sessionSt: SessionStorageService,
    ) { }

    public leggiPianoConti(partitaIva: string, pianocode: number): Observable<any[]> {
        let leggiPianoConti: LeggiPianoConti = {
            piva: partitaIva,
            pianoCod: pianocode
        };

        return this.apiService.ajaxAPIPost<LeggiPianoConti, any[]>("Valutazioni/LeggiValutazionePianoConti", leggiPianoConti).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }

    public leggiRagioneSociale(): Observable<any[]> {

        return this.apiService.ajaxAPIGet<null,any[]>("AnagraficaNG/LeggiImpreseConFiltroUtente_Modello",null).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }

    public leggiValutazioni(partitaIva: string): Observable<any[]> {

        let leggiTestata: LeggiTestata = {
            piva: partitaIva,
            idTestata: 0,
            includiAnno: true
        };

        return this.apiService.ajaxAPIPost<LeggiTestata, any[]>("Valutazioni/LeggiValutazioneTestataGriglia", leggiTestata).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }

    public leggiValutazioneObservable(val_Testata: any): Observable<any[]> {


        let leggiTestata: LeggiTestata = {
            piva: val_Testata.piva,
            idTestata: val_Testata.idTestata,
            includiAnno: true
        };

        return this.apiService.ajaxAPIPost<LeggiTestata, any[]>("Valutazioni/LeggiValutazioneTestataGriglia", leggiTestata).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }

    public leggiValutazioneObservableForDelete(val_Testata: any): Observable<any[]> {


        let leggiTestata: LeggiTestata = {
            piva: val_Testata.Piva,
            idTestata: val_Testata.Id_Testata,
            includiAnno: true
        };

        return this.apiService.ajaxAPIPost<LeggiTestata, any[]>("Valutazioni/LeggiValutazioneTestataGriglia", leggiTestata).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );
    }


    public ScriviValutazione(valutazioneTestata: ValutazioneTestata): Observable<rispostaStandard<any>> {

        if (valutazioneTestata.flag_cancellazione != true) {
            valutazioneTestata.Valutazione_TestataxAnno = [];

            if (this.gridPublicService.getValue().data.rows.length != 0) {
                this.gridPublicService.getValue().data.rows.forEach(x => {

                    let row = x as any;

                    let year = row.name;

                    let type = this.selectCodeType(row);

                    let pkvalTestata: PKValutazioneTestata = {
                        Piva: valutazioneTestata.primaryKey.Piva,
                        Id_Testata: valutazioneTestata.primaryKey.Id_Testata
                    };

                    let valTextAnnoPk: PKValutazioneTestataxAnno = {
                        Anno: year,
                        valutazioneTestataPK: pkvalTestata
                    };
                    let valTextAnno: ValutazioneTestataxAnno = {
                        primaryKey: valTextAnnoPk,
                        Anno_Tipo: this.selectCodeNumber(row),
                        Anno_Tipo_Des: type,
                        flag_cancellazione: false
                    };

                    valutazioneTestata.Valutazione_TestataxAnno.push(valTextAnno);
                });
            }

            this.removedYearsObjects.forEach(x => {

                let year = x.year;
                let type = this.selectCodeType("-1");

                let pkvalTestata: PKValutazioneTestata = {
                    Piva: valutazioneTestata.primaryKey.Piva,
                    Id_Testata: valutazioneTestata.primaryKey.Id_Testata
                };

                let valTextAnnoPk: PKValutazioneTestataxAnno = {
                    Anno: +year,
                    valutazioneTestataPK: pkvalTestata
                };
                let valTextAnno: ValutazioneTestataxAnno = {
                    primaryKey: valTextAnnoPk,
                    Anno_Tipo: -1,
                    Anno_Tipo_Des: type,
                    flag_cancellazione: true
                };

                valutazioneTestata.Valutazione_TestataxAnno.push(valTextAnno);
            });
        }

        return this.apiService.ajaxAPIPost<ValutazioneTestata, rispostaStandard<any>>("Valutazioni/ScriviTestata", valutazioneTestata).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        );

    }

    selectCodeNumber(row: any): number {

        let type = 0;
        if (row != -1) {
            if (row.id_.id == undefined) {
                type = row.id_;
            } else {
                type = row.id_.id;
            }
        } else {
            type = -1;
        }

        return type;
    }

    selectCodeType(row: any): string {

        let type = "";
        if (row != "-1") {
            if (row.id_.id == undefined) {
                type = row.id_;
            } else {
                type = row.id_.id;
            }
        } else {
            type = "-1";
        }

        switch (type) {
            case "-1":
            case "1":
                return "Chiuso";
            case "2":
                return "Aperto";
            case "3":
                return "Previsionale";
        }
    }

    changeLeggiTestata(upd_leggiTestata: LeggiTestata) {
        this.sessionSt.store('ultima_valutazione',upd_leggiTestata);
        this.leggiTestataSource.next(upd_leggiTestata);
    }

    getLeggiTestataValue(): LeggiTestata {

        let leggiTestata : LeggiTestata = this.leggiTestataSource.getValue();
        if(leggiTestata.idTestata == undefined){
            leggiTestata = this.sessionSt.retrieve('ultima_valutazione');
        }

        return Object.assign({}, leggiTestata);
    }

    public scriviDettaglioGriglia(jsonElement : string) : Observable<any>{

        let leggiGriglia : LeggiGriglia = new LeggiGriglia(jsonElement);
        leggiGriglia.jsonGriglia = jsonElement;


        return this.apiService.ajaxAPIPost<LeggiGriglia, rispostaStandard<any>>("Valutazioni/ScriviDettaglioGriglia", leggiGriglia).pipe(
            switchMap(x => { return of(x); })
        );
    }

    public scriviDettaglioGrigliaFiglia(jsonElement : string) : Observable<any>{

        let leggiGriglia : LeggiGriglia = new LeggiGriglia(jsonElement);
        leggiGriglia.jsonGriglia = jsonElement;


        return this.apiService.ajaxAPIPost<LeggiGriglia, rispostaStandard<any>>("Valutazioni/ScriviDettaglioSpecificoGriglia", leggiGriglia).pipe(
            switchMap(x => { return of(x); })
        );
    }

    public aggiornaDatiDettaglio(leggiTestata : LeggiTestata) : Observable<any>{
        return this.apiService.ajaxAPIPost<LeggiTestata, rispostaStandard<any>>("Valutazioni/AggiornaDettaglioSpecifico", leggiTestata).pipe(
            switchMap(x => { return of(x); })
        );
    }

    public aggiornaDatiDettaglioSingolo(dataItem : any) : Observable<any>{
 
        console.log(dataItem);
        let leggiDettaglio : LeggiDettaglio = {
            piva : dataItem.Piva,
            idTestata : dataItem.Id_Testata,
            ContoCod : dataItem.Valutazione_Conto_Cod
        };


        return this.apiService.ajaxAPIPost<LeggiDettaglio, rispostaStandard<any>>("Valutazioni/AggiornaDettaglioSingolo", leggiDettaglio).pipe(
            switchMap(x => { return of(x); })
        );
    }

    exportToExcel(piva: any,idTestata : any, filename: string) {

        console.log("exportExcel");
        let leggiTestata: LeggiTestata = {
            piva: piva,
            idTestata: idTestata,
            includiAnno: true
        };

        this.apiService.ajaxAPIPost<LeggiTestata, any>("Valutazioni/ReportExcel", leggiTestata).pipe(
            switchMap(x => { return of(x.RispostaStringa); })
        ).subscribe(resp => {
            console.log(resp);


            let binaryData: Uint8Array = resp.Data;
            let filename = resp.FileName;

            const blob = new Blob([this.base64ToArrayBuffer(binaryData)], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            saveAs(blob, filename);

        });
      }


      base64ToArrayBuffer(base64) {
        let binaryString = window.atob(base64);
        let binaryLen = binaryString.length;
        let bytes = new Uint8Array(binaryLen);
        for (let i = 0; i < binaryLen; i++) {
           let ascii = binaryString.charCodeAt(i);
           bytes[i] = ascii;
        }
        return bytes;
     }
}
