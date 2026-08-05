import { Injectable } from '@angular/core';
import { Caratteristica } from 'app/Model/anagrafiche/ParcoMacchine';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { MasterService } from 'app/Service/master.service';
import { BehaviorSubject, map, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CaratteristicheService {

  caratteristiche: Caratteristica[] = new Array();

  public caratteristicheSource = new BehaviorSubject(this.caratteristiche);

  constructor(
    private masterService: MasterService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {  }

  public setCaratteristiche(caratteristiche: Caratteristica[]) {
    this.caratteristicheSource.next(caratteristiche);
  }

  public getCaratteristiche(): Caratteristica[] {
    return this.caratteristicheSource.getValue();
  }

  public getCaratteristicheDisponibili(class_code: string) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<string, any[]>(
      "AnagraficaNG/Leggi_Caratteristiche_Macchina_Anagrafica",
      class_code).pipe(take(1), map(el => {
      return el.RispostaStringa;
    }))
  }

  public checkPerditaCaratteristiche(carCorrenti: Caratteristica[], disponibili: any[]): boolean {
    let result = false;
    carCorrenti.forEach(caratt => {
      if(disponibili.length == 0) {
        result = true;
      } else {
        let listaInt = disponibili.map(t => t.codice);
        if (listaInt.indexOf(caratt.caratteristica.codice) < 0) {
          result = true;
        }
      }
    })
    return result;
  }
}
