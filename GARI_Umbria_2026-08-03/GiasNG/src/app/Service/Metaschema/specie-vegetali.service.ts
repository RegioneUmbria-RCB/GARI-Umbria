import { Injectable } from '@angular/core';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import {map, Observable, take} from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {SementieriParametrizzazione} from '../../Model/GIS/SementieriParametrizzazione';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { SPECI_LNK } from 'gias-kendo-grid';
import { Varieta as IVarieta, UtilizzoTerreno, LeggiSpecieQdC, Impresa as IImpresa, CentroAziendale as ICentroAziendale } from 'app/Service/api.service'
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';

export class LeggiSpecie{
  parametriSementieri: SementieriParametrizzazione;
  gruppiVegetali: number[];

  constructor(cfgSementi: SementieriParametrizzazione = undefined, grVeg: number[] = []) {
    this.parametriSementieri = cfgSementi;
    this.gruppiVegetali = grVeg;
  }
}

@Injectable({
  providedIn: 'root'
})
export class SpecieVegetaliService {
  private SpecieVegetali: Specie[] = new Array();
  private ColturePrecedenti: Specie[]= new Array();

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  leggi_FiltroUtente(sementieriParametrizzazione?: SementieriParametrizzazione) {
    return new Promise<Specie[]>(async (resolve, reject) => {
      if (this.SpecieVegetali == undefined || this.SpecieVegetali.length == 0) {
        this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecie, Specie[]>(
          'Modello/Specie',
          new LeggiSpecie(sementieriParametrizzazione),
          false
        ).pipe(map(data => {
          this.SpecieVegetali = data.RispostaStringa;
          resolve(this.SpecieVegetali);
        })).subscribe();
      } else {
        resolve(this.SpecieVegetali);
      }
    });
  }

  leggi() {
    return new Promise<Specie[]>(async (resolve, reject) => {
      if(this.ColturePrecedenti== undefined || this.ColturePrecedenti.length == 0){
        this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecie, Specie[]>(
          'MetaschemaNG/LeggiSpecieVegetali',
          new LeggiSpecie(),
          false).pipe(map( R => {
          this.ColturePrecedenti = R.RispostaStringa;
          resolve(this.ColturePrecedenti);
        })).subscribe();
      }else{
        resolve(this.ColturePrecedenti);
      }
    });
  }

  leggiTutteLeSpecieAPI(): Observable<Specie[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<string, Array<Specie>>('Modello/Specie', "")
      .pipe(map(r => r.RispostaStringa));
  }

  leggi_Da_Cultivar(cultivar: Varieta) {
    return new Promise<Specie>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<Varieta, Specie>(
        'MetaschemaNG/Leggi_Da_Cultivar',
        cultivar,
        false).pipe(map( R => {
        resolve(R.RispostaStringa);
      })).subscribe();
    })
  }

  leggiDaGruppiVegetali(gruppi: number[]): Observable<Specie[]> {
    const params = new LeggiSpecie();
    params.gruppiVegetali = gruppi ?? [];
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecie, Specie[]>(
      'MetaschemaNG/LeggiSpecieVegetali', params, false
    ).pipe(take(1), map( R => R.RispostaStringa));
  }

  leggiPerCentroAziendale(piva: string, sa_cod: number = 0): Observable<BaseCodeDescr[]> {
    const center = CentroAziendale.Empty(piva);
    center.primaryKey.codice = sa_cod;
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecieQdC, UtilizzoTerreno[]>(
          SPECI_LNK, {
          centroAziendale: center as unknown as ICentroAziendale,
          data: new Date(),
          impresa: new Impresa(piva) as unknown as IImpresa,
          consideraTerrenoNudo: true,
          soloAttiviAllaData: false
    }).pipe(
      take(1),
      map(r => this.mapUtilizzoTerrenoToSpecie(r.RispostaStringa))
    );
  }

   private mapUtilizzoTerrenoToSpecie(utilizzi: UtilizzoTerreno[]): BaseCodeDescr[] {
      const result: BaseCodeDescr[] = [];
      for (const utilizzoTerreno of utilizzi) {
        if (utilizzoTerreno.classType === 'Varieta') {
          const varieta = utilizzoTerreno as IVarieta
          result.push(new BaseCodeDescr(varieta.specie.codice, varieta.specie.descrizione));
        } else {
          // Destinazione d'uso --> necessario attivare flag_TerrenoNudo in filtri di ricerca per leggere
          // correttamente le operazioni. Il '-' è stato messo per non confondere il record con le varietà.
          result.push(new BaseCodeDescr(-utilizzoTerreno.codice, utilizzoTerreno.descrizione));
        }
      }
      return result;
    }

}
