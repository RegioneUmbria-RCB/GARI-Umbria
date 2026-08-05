import {Injectable} from '@angular/core';
import {BaseCodeDescrStr} from '../../Model/baseClass/baseCodeDescrStr';
import {Observable, of} from 'rxjs';
import {CodificaMacchineAgeaRequest} from '../../Model/metaschema/CodificheAgea';
import {rispostaStandard} from '../master.service';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';

@Injectable({providedIn: 'root'})
export class CodificheAgeaService {

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {
  }
  public readAgeaMachineCodes(params: CodificaMacchineAgeaRequest): Observable<rispostaStandard<BaseCodeDescrStr[]>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<CodificaMacchineAgeaRequest, BaseCodeDescrStr[]>(
      'MetaschemaNG/ReadAgeaMachineCodes',
      params
    );
  }
}
