import { Injectable } from '@angular/core';
import { Caratteristica, MacchinaGerarchia, ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { MasterService } from 'app/Service/master.service';
import { BehaviorSubject, map, Observable, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GerarchiaService {

  gerarchia: MacchinaGerarchia[] = new Array();

  public gerarchiaSource = new BehaviorSubject(this.gerarchia);

  constructor(private masterService: MasterService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

  public setGerarchia(gerarchia: MacchinaGerarchia[]) {
    this.gerarchiaSource.next(gerarchia);
  }

  public getGerarchia(): MacchinaGerarchia[] {
    return this.gerarchiaSource.getValue();
  }

  /*public getCaratteristicheDisponibili_Old(class_code: string) {
    let parametri:CoreWS_Generic<String> = {
      objP: this.masterService.getCoreWSGenericObjP(),
      InData: class_code
    }
    const obs: Observable<any[]> =
      this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any[], String>(
          this.masterService.link_CoreWS + "/Anagrafica/Macchine.asmx/Leggi_Caratteristiche_Macchina_Anagrafica",
          parametri).pipe(take(1), map(el => {
          return el.RispostaStringa;
      }))
    return obs;
  }*/

  public getCaratteristicheDisponibili(class_code: string) {

    const obs: Observable<any[]> =
      this.ajaxAgronicaAPIService.ajaxAPIPost<String, any[]>(
          "AnagraficaNG/Leggi_Caratteristiche_Macchina_Anagrafica",
          class_code).pipe(take(1), map(el => {
          return el.RispostaStringa;
      }))
    return obs;
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
