import { Injectable } from '@angular/core';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { Observable, of, take } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';
import {Tipo_Ricetta} from "../../Model/attivita/Attivita";
import { Tipo_Attivita, Stati } from 'gias-ui-kit';

export class LeggiOperazioni {
    gruppiOperazioni: string[];
    lista_Lav_Cod: number[];
    FiltraImpostazioniUtente: boolean;
    Visualizza_Solo_Operazioni_Preferite: boolean;
    tipo_Attivita: Tipo_Attivita;
    tipo_Ricetta: Tipo_Ricetta;
    stato: Stati;
}

export class SalvaOperazioniPreferite {
    lista_Lav_Cod: number[];
    /** Se impostato a `true` esegue un aggiornamento completo delle
     * impostazioni, sostituendo le operazioni selezionate a quelle
     * impostate in precedenza, in caso contrario va semplicemente ad
     * aggiungere le nuove operazioni selezionate. Impostato `false` di default.
     **/
    public full_update?: boolean = false;
}

@Injectable({ providedIn: 'root' })

export class OperazioniService{
    private listOperazioni:Lavorazione[] = null;
    private listComboLavorazioni: Lavorazione[] = null;

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

    Leggi_Operazioni_Modello_QdC(operazioni: LeggiOperazioni){
        return new Promise<Lavorazione[]>(async (resolve, reject) => {
            if (this.listOperazioni == null) {
                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiOperazioni, Lavorazione[]>('MetaschemaNG/LeggiOperazioniModello', operazioni).pipe(map(R => {
                    resolve(R.RispostaStringa);
                })).subscribe();
            } else {
                resolve(this.listOperazioni);
            }
        });
    }

    caricaComboLavorazioni_PerImpostazioni(codiciGruppiOperazioni: string[]) {
      if (!codiciGruppiOperazioni)
        codiciGruppiOperazioni = [];
      return this.ajaxAgronicaAPIService.ajaxAPIPost<string[], any[]>(
        'MetaschemaNG/CaricaComboLavorazioni_PerImpostazioni', codiciGruppiOperazioni
      ).pipe(take(1), map((r) => r.RispostaOK ? r.RispostaStringa : []));
    }

    CaricaComboLavorazioni(listGruppoOperazioni: string[]): Observable<Lavorazione[]>{
        if (this.listComboLavorazioni == null) {
            let leggiOp = new LeggiOperazioni();
            leggiOp.gruppiOperazioni = listGruppoOperazioni;

            return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiOperazioni, Lavorazione[]>('MetaschemaNG/CaricaComboLavorazioniModel', leggiOp)
                .pipe(map((r) => { return r.RispostaStringa }));

        } else {
            return of(this.listComboLavorazioni)
        }
    }

    SalvaOperazioniPreferite(p: SalvaOperazioniPreferite){
        return new Promise<rispostaStandard<string>>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<SalvaOperazioniPreferite, any>('MetaschemaNG/SalvaOperazioniPreferite', p)
            .subscribe(R => resolve(R));
        });
    }

    LeggiOperazioniVisite(): Observable<Lavorazione[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, Array<Lavorazione>>('Visite/LeggiVisiteOperazioni', "")
            .pipe(map(r => r.RispostaStringa));
    }

}
