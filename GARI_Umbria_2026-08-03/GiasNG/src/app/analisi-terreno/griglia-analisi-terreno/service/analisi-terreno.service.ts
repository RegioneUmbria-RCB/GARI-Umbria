import { Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { LeggiAnalisiTerreno, LeggiParametriAnalisiDettagli, LeggiSchemiAnalisi, ScriviAnalisiTerreno, ScriviListaAnalisiTerreno } from "app/Service/api.service";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { BehaviorSubject, Observable, map } from "rxjs";

export class LeggiListaAnalisiTerreno {
    public Piva: string;
    public Data: Date;
    public ApplicaVisibilitaUMA: boolean = false;

    constructor(piva: string, data: Date, applicaVisibilitaUMA: boolean = false) {
        this.Piva = piva;
        this.Data = data;
        this.ApplicaVisibilitaUMA = applicaVisibilitaUMA;
    }
}


@Injectable({providedIn: 'root'})
export class AnalisiTerrenoService {

    filterData = new BehaviorSubject<LeggiListaAnalisiTerreno>(null);

    constructor(
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private transloco: TranslocoService
    ) {}

    public LeggiListaAnalisiTerreno(p: LeggiListaAnalisiTerreno): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiListaAnalisiTerreno, string>('AnalisiTerreno/LeggiListaAnalisiTerreno',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public LeggiAnalisiTerreno(p: LeggiAnalisiTerreno): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAnalisiTerreno, string>('AnalisiTerreno/LeggiAnalisiTerreno',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public ModificaAnalisiTerreno(p: ScriviAnalisiTerreno): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviAnalisiTerreno, string>('AnalisiTerreno/ModificaAnalisiTerrenoInLine',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public CancellaAnalisiTerreno(p: ScriviAnalisiTerreno): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviAnalisiTerreno, string>('AnalisiTerreno/CancellaAnalisiTerreno',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public CancellaListaAnalisiTerreno(p: ScriviListaAnalisiTerreno): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviListaAnalisiTerreno, string>('AnalisiTerreno/CancellaListaAnalisiTerreno',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public LeggiLaboratori(): Observable<Array<any>> {

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<null, Array<any>>('AnalisiTerreno/LeggiLaboratori', null).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public LeggiSchemiAnalisi(p: LeggiSchemiAnalisi): Observable<Array<any>> {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSchemiAnalisi, Array<any>>('AnalisiTerreno/LeggiSchemiAnalisi',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }
    
    public LeggiParametriAnalisi(p: LeggiParametriAnalisiDettagli): Observable<string> {
        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiParametriAnalisiDettagli, string>('AnalisiTerreno/LeggiParametriAnalisi', p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public LeggiCampiEntita(objParametriAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/LeggiCampiAnagrafica',
          objParametriAgenda).pipe(map((data) => data.RispostaStringa));
    }

    public LeggiImpiantiEntita(objParametriAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/LeggiEserciziAnagrafica', 
          objParametriAgenda).pipe(map((data) => data.RispostaStringa));
    }

    public LeggiCatastoEntita(objPAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>(
          'AnagraficaNG/LeggiCatastoAnagrafica',
          objPAgenda
        ).pipe(map((data) => data.RispostaStringa));
    }

    public LeggiCentriEntita(objParametriAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/Centri', 
          objParametriAgenda).pipe(map((data) => data.RispostaStringa));
    }

    public LeggiAppezzamentiEntita(objParametriAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/LeggiAppezzamentiAnagrafica', 
          objParametriAgenda).pipe(map((data) => data.RispostaStringa));
    }

    public ScriviAnalisiTerreno(scriviAnalisiTerreno: ScriviAnalisiTerreno): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviAnalisiTerreno, any>('AnalisiTerreno/ScriviAnalisiTerreno', 
            scriviAnalisiTerreno);
    }

}