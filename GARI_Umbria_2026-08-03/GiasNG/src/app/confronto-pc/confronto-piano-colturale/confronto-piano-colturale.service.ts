import { Injectable } from '@angular/core';
import {AjaxAgronicaNetCore6ApiService} from "../../Service/ajax-agronica-net-core6-api.service";
import {Observable} from "rxjs";
import {ParametriTipoConfronto} from "../../Service/net-core6-api.service";

@Injectable({
  providedIn: 'root'
})
export class ConfrontoPianoColturaleService {

  constructor(
    private ajaxAgronicaNetCore6ApiService: AjaxAgronicaNetCore6ApiService
  ) { }

  public confrontoCatasto(): Observable<any> {
    let params: ParametriTipoConfronto = {
      tipoConfronto: 0,
      partitaIva: "01389150333",
      programmazioneCod: 44,
      showCatasto: false,
      showVarieta: false,
    }
    return this.ajaxAgronicaNetCore6ApiService.ajaxAPIPost<any, any>('/LeggiConfrontoCatasto', params, false)
  }
}
