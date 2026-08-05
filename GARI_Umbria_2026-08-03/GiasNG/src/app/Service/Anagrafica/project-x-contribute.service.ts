import { Injectable } from '@angular/core';
import {Esercizio} from '../../Model/anagrafiche/Esercizio';
import {rispostaStandard} from '../master.service';
import {map} from 'rxjs';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectXContributeService {

  constructor(
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  addContributes(exs: Esercizio[]) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Esercizio[], rispostaStandard<boolean>>(
      'AnagraficaNG/WriteProjectsXContributesRecords', exs).pipe(
      map(resp => resp.RispostaStringa)
    );
  }
}
