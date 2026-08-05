import {Injectable} from '@angular/core';
import {map, Observable, of} from 'rxjs';
import {RispostaStandard, rispostaStandard} from '../master.service';
import {
  DatiPrevisionaliColture,
  DatiPrevisionaliColtureComplete,
  DatiPrevisionaliColtureRequest
} from '../../Model/anagrafiche/DatiPrevisionaliColture';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';

@Injectable({
  providedIn: 'root'
})
export class DatiPrevisionaliService {

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {  }
  public readDatiPrevisionaliColture(): Observable<rispostaStandard<DatiPrevisionaliColture>> {
    return of(undefined);
  }

  public readDatiPrevisionaliColtureDT(params: DatiPrevisionaliColtureRequest): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<DatiPrevisionaliColtureRequest, any>(
      'AnagraficaNG/readDatiPrevisionaliColtureDT',
      params
    ).pipe(map((data) => {
      return <any[]>(data.RispostaStringa == '' ? [] : data.RispostaStringa);
    }));
  }

  public editDatiPrevisionaliColture(params: DatiPrevisionaliColtureComplete): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<DatiPrevisionaliColtureComplete, RispostaStandard>(
      'AnagraficaNG/editDatiPrevisionaliColture',
      params
    ).pipe(map((data) => {
      return data.RispostaStringa;
    }));
  }

  public createDatiPrevisionaliColture(params: DatiPrevisionaliColtureComplete): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<DatiPrevisionaliColtureComplete, RispostaStandard>(
      'AnagraficaNG/createDatiPrevisionaliColture',
      params
    ).pipe(map((data) => {
      return data.RispostaStringa;
    }));
  }

  public deleteDatiPrevisionaliColture(params: DatiPrevisionaliColtureComplete): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<DatiPrevisionaliColtureComplete, RispostaStandard>(
      'AnagraficaNG/deleteDatiPrevisionaliColture',
      params
    ).pipe(map((data) => {
      return data.RispostaStringa;
    }));
  }

}
