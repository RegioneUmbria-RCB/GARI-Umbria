import { Injectable } from '@angular/core';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { HttpService } from 'app/Service/http.service';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
//import { ScriviCentroLink } from 'app/anagrafica/centri/centri-edit/utils';
import { Observable, of, tap, switchMap, BehaviorSubject } from 'rxjs';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { PKValutazioneTestata, ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import { GridPublicService } from 'gias-kendo-grid';
import { PKValutazioneTestataxAnno, ValutazioneTestataxAnno } from 'app/Model/valutazioni/ValutazioneTestataxAnno';
import { SessionStorageService } from 'ngx-webstorage';
import { PianoConti } from 'app/Model/valutazioni/PianoConti';
import { PianoContixTree } from 'app/Model/valutazioni/PianoContixTree';

export class LeggiPianoConti {
    piva: string | undefined;
    pianoCod: number | undefined;
}

export class LeggiPianoContixConti {
    piva: string | undefined;
    pianoCod: number | undefined;
    contoCod : number | undefined;
}



@Injectable({ providedIn: 'root' })
export class PianoContiService {
    private leggiPiano: LeggiPianoConti = new (LeggiPianoConti);
    private leggiPianoSource = new BehaviorSubject(this.leggiPiano);
    currentLeggiPiano: Observable<LeggiPianoConti> = this.leggiPianoSource.asObservable();


    private numValAssociate: number = 0;
    private ValAssociatePianoSource = new BehaviorSubject(this.numValAssociate);
    currentNUmValAssociato: Observable<number> = this.ValAssociatePianoSource.asObservable();


    gridPublicService: GridPublicService

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
        }

        return this.apiService.ajaxAPIPost<LeggiPianoConti, any[]>("Valutazioni/LeggiValutazionePianoContiGriglia", leggiPianoConti).pipe(
            switchMap(x => { return of(x.RispostaStringa) })
        );
    }

    public leggiPianoContiContiExTree(partitaIva: string, pianocode: number): Observable<any[]> {
        let leggiPianoConti: LeggiPianoContixConti = {
            piva: partitaIva,
            pianoCod: pianocode,
            contoCod : 0
        }

        return this.apiService.ajaxAPIPost<LeggiPianoConti, any[]>("Valutazioni/LeggiValutazionePianoContixContiTree", leggiPianoConti).pipe(
            switchMap(x => { return of(x.RispostaStringa) })
        );
    }

    changePianoConti(upd_PianoConti: LeggiPianoConti) {
        this.sessionSt.store('ultima_piano_conti',upd_PianoConti);
        this.leggiPianoSource.next(upd_PianoConti);
    }

    getPianoContoValue(): LeggiPianoConti {

        let leggiPiano : LeggiPianoConti = this.leggiPianoSource.getValue();
        if(leggiPiano.pianoCod == undefined){
            leggiPiano = this.sessionSt.retrieve('ultima_piano_conti');
        }

        return Object.assign({}, leggiPiano);
    }


    changeValAssociato(upd_valAssociato: number) {
        this.sessionSt.store('num_val_associato',upd_valAssociato);
        this.ValAssociatePianoSource.next(upd_valAssociato);
    }

    getNumValAssociato(): number {
        let leggivalAssociato : number = this.ValAssociatePianoSource.getValue();        
        return leggivalAssociato;
    }

    public scriviPianoConti(dataItem : PianoConti,flagCancellazione : boolean): Observable<any> {
        
        console.log("scrivipianoconti");
        dataItem.flag_cancellazione = flagCancellazione;

        return this.apiService.ajaxAPIPost<PianoConti, any>("Valutazioni/ScriviPianoConti", dataItem).pipe(
            switchMap(x => { return of(x) })
        );
    }

    public scriviPianoContixTree(dataItem : PianoContixTree): Observable<any> {
        

        return this.apiService.ajaxAPIPost<PianoContixTree, any>("Valutazioni/ScriviPianoContixContiTree", dataItem).pipe(
            switchMap(x => { return of(x) })
        );
    }
   
}