import { Injectable } from '@angular/core';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {Observable} from 'rxjs';
import {rispostaStandard} from '../master.service';
import {Contribute} from '../../Model/metaschema/Contribute';

@Injectable({
  providedIn: 'root'
})
export class ContributeService {

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  read(params: Contribute): Observable<rispostaStandard<Contribute[]>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Contribute, Contribute[]>(
      'MetaschemaNG/ReadContributes',
      params
    );
  }
}
