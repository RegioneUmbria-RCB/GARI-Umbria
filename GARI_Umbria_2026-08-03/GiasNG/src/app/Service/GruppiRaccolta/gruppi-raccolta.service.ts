import { Injectable } from '@angular/core';
import {Observable, of} from 'rxjs';
import {GruppoRaccolta, ScriviGruppoRaccolta} from '../../Model/metaschema/GruppoRaccolta';
import {rispostaStandard} from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {map} from 'rxjs/operators';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {enum_TipoOperazioneDB} from '../../Model/TipiEnumerativi';

@Injectable({
    providedIn: 'root'
})
export class GruppiRaccoltaService {

    constructor(
        private ajaxApiService: AjaxAgronicaAPIService
    ) { }

    public leggiGruppiRaccolta(objParametri: ObjParametriAgenda): Observable<Array<GruppoRaccolta>> {
        return (this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, GruppoRaccolta[]>(
            'AnagraficaNG/LeggiGruppiRaccolta',
            objParametri,
            false).pipe(map((risp) => {
            return (risp. RispostaStringa);
        })));
    }

    public scriviModificaCancella_GruppoRaccolta(gr: GruppoRaccolta, operation: enum_TipoOperazioneDB): Observable<rispostaStandard<GruppoRaccolta>> {
        let sgr: ScriviGruppoRaccolta = new ScriviGruppoRaccolta();
        sgr.gruppoRaccolta = gr;
        sgr.tipoOperazione = operation;

        return (this.ajaxApiService.ajaxAPIPost<ScriviGruppoRaccolta, rispostaStandard<GruppoRaccolta>>(
            'AnagraficaNG/ScriviModificaCancella_GruppoRaccolta',
            sgr,
            false).pipe(map((risp) => {
            return (risp. RispostaStringa);
        })));
    }

    leggiGruppiRaccoltaValidi(objParametri: ObjParametriAgenda): Observable<GruppoRaccolta[]> {
        return (this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, GruppoRaccolta[]>(
            'AnagraficaNG/LeggiGruppiRaccoltaValidi',
            objParametri,
            false).pipe(map((risp) => {
            return (risp.RispostaStringa);
        })));
    }

    leggiGruppoRaccoltaImpresa(objParametri: ObjParametriAgenda): Observable<GruppoRaccolta> {
        return (this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, GruppoRaccolta>(
            'AnagraficaNG/LeggiGruppoRaccoltaImpresa',
            objParametri,
            false).pipe(map((risp) => {
            return (risp.RispostaStringa);
        })));
    }
}
