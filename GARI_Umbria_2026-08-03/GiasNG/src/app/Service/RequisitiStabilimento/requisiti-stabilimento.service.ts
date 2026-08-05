import { Injectable } from '@angular/core';
import {Observable} from 'rxjs/internal/Observable';
import {JsonKendoResult} from 'gias-kendo-grid';
import {map} from 'rxjs';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {IntervalloTemporale} from '../../Model/anagrafiche/IntervalloTemporale';

@Injectable({
  providedIn: 'root'
})
export class RequisitiStabilimentoAPIService {

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  public readRequisitiStabilimento(params: ReadRequisitiStabilimento): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ReadRequisitiStabilimento, any[]>(
      'RequisitiStabilimento/readRequisitiStabilimento',
      params
    ).pipe(map((data) => <any[]>data.RispostaStringa));
  }

  public readPianoColturale(params: ReadRequisitiStabilimento): Observable<JsonKendoResult> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ReadRequisitiStabilimento, any>(
      'RequisitiStabilimento/readPianoColturaleRequisitiStabilimento',
      params
    ).pipe(map((data) => <JsonKendoResult>data.RispostaStringa));
  }

  public readContracts(params: ReadRequisitiStabilimento): Observable<JsonKendoResult> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ReadRequisitiStabilimento, any>(
      'RequisitiStabilimento/readContracts',
      params
    ).pipe(map((data) => <JsonKendoResult>data.RispostaStringa));
  }

  public saveRequisitiStabilimento(params: SaveRequisitiStabilimento): Observable<JsonKendoResult> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<SaveRequisitiStabilimento, any>(
      'RequisitiStabilimento/saveRequisitiStabilimento',
      params
    ).pipe(map((data) => <JsonKendoResult>data.RispostaStringa));
  }

  public readDettaglioAziendale(params: ReadDettaglioAziendale): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ReadDettaglioAziendale, any>(
      'RequisitiStabilimento/readDettaglioAziendale',
      params
    ).pipe(map((data) => <any[]>data.RispostaStringa));
  }

}

export class ReadRequisitiStabilimento {
  validita: IntervalloTemporale;
  matCod: number;
  idBudget: number;
  risUm: number;
  mostraAssegnazioni: boolean;
}

export class ReadDettaglioAziendale{
  validita: IntervalloTemporale;
  matCod: number;
  idBudget: number;
  piva: string;
}

export class SaveRequisitiStabilimento {
  rows: any[];
}
