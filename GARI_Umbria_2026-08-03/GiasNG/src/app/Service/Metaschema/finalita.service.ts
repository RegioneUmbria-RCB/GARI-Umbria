import { Injectable } from '@angular/core';
import { FiltroFinalita2 } from 'app/Model/filtri/filtroFinalita2';
import { FaseCicloColturale } from 'app/Model/metaschema/fase';
import { GruppoFinalita } from 'app/Model/metaschema/utilizzi/GruppoFinalita';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import {map, Observable, of} from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import {SementieriParametrizzazione} from '../../Model/GIS/SementieriParametrizzazione';
import {CaricaComboFinalita2} from '../api.service';
import { Leggi_FasiCicloColturalexSpecie } from 'app/Model/anagrafiche/Leggi_FasiCicloColturalexSpecie';
import { FiltroSpecieFinalita } from 'app/Model/filtri/filtroSpecieFinalita';

export class FinalitaxSpecie {
  Specie: Specie;
  Finalita: GruppoFinalita[];
}

export class LeggiFinalita {
  specie: Specie;
  parametriSementieri: SementieriParametrizzazione

  constructor(specie: Specie, cfgSementieri?: SementieriParametrizzazione) {
    this.specie = specie;
    this.parametriSementieri = cfgSementieri;
  }
}

@Injectable({
  providedIn: 'root'
})
export class GruppoFinalitaService {
  private FinalitaxSpecie: FinalitaxSpecie[] = new Array();
  private FasiCicloColturale: {chiave:string, valori: FaseCicloColturale[]}[] = []
  constructor(
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  leggi(specie: Specie, cfgSementi: SementieriParametrizzazione){
    return new Promise<GruppoFinalita[]>(async (resolve, reject) => {
      if (specie.codice == 0) {
        resolve([]);
      }

      if (this.FinalitaxSpecie.find((el) => {
        if(el.Specie.codice == specie.codice) {
          return el;
        }
      })  == undefined) {
        let params = new LeggiFinalita(specie, cfgSementi);
        this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiFinalita, GruppoFinalita[]>(
          'MetaschemaNG/LeggiGruppoFinalita',
          params,
          false
        ).pipe(map(R => {
          this.FinalitaxSpecie.push({ Specie: specie, Finalita: R.RispostaStringa });

          resolve(this.FinalitaxSpecie.find((el) => {
            if (el.Specie.codice == specie.codice) {
              return el;
            }
          }).Finalita);
        })).subscribe();

      } else {

        resolve(this.FinalitaxSpecie.find((el) => {
          if (el.Specie.codice == specie.codice) {
            return el;
          }
        }).Finalita);

      }
    });
  }

  leggiFinalitaApportiMassimiMacroelementi(filtro: FiltroFinalita2): Promise<FaseCicloColturale[]> {
    return new Promise<FaseCicloColturale[]>(async (resolve, reject) => {
      let chiave = JSON.stringify(filtro);
      let chiaveArr = this.FasiCicloColturale.find((el) => {return el.chiave == chiave});
      if (chiaveArr){
        resolve(chiaveArr.valori);
      }
      if (filtro.finalita == null || filtro.finalita.codice == 0 ||
        filtro.specie == null || filtro.specie.codice == 0) {
        resolve([]);
        return;
      }

      const params: CaricaComboFinalita2 = {
        Veg_Cod: filtro.specie.codice,
        Grfi_Cod: filtro.finalita.codice,
        Regolamento_Cod: filtro.regolamentoConcimazione.codice,
        StringaCerca: "",
        FiltroAggiuntivo: "",
        Ordinamento: ""
      };
      this.ajaxAgronicaAPIService.ajaxAPIPost<CaricaComboFinalita2, any>('MetaschemaNG/CaricaComboFinalita2', params)
        .GiasSubscribe(R => {
          let arrResp = R.RispostaStringa;
          arrResp = arrResp.map(t => { return {codice: t.grfi_cod, descrizione: t.grfi_des} })

          let fase102 = arrResp.find((el) => { return el.codice == 17 });
          if (!fase102){
            arrResp.push({
              codice: 102,
              descrizione: 'In produzione',
              disciplinarePubblicoPrivato: true
            })
          } else {
            arrResp.find((el) => { return el.codice == 17}).codice = 102;
          }

          this.FasiCicloColturale.push({chiave: chiave, valori: arrResp});
          resolve(arrResp);
        })
    });
  }

  Leggi_FasiCicloColturalexSpecie(filtro: FiltroSpecieFinalita): Observable<FaseCicloColturale[]> {
    let chiave = JSON.stringify(filtro);
    let chiaveArr = this.FasiCicloColturale.find((el) => {return el.chiave == chiave});

    if (chiaveArr) {
      return of(chiaveArr.valori);
    }

    if (filtro.specie?.codice == null || filtro.specie.codice == 0) {
      return of([]);
    }

    let parametri: Leggi_FasiCicloColturalexSpecie = {
      Veg_Cod: filtro.specie.codice
    }

    return this.ajaxAgronicaAPIService.ajaxAPIPost<Leggi_FasiCicloColturalexSpecie, any[]>('MetaschemaNG/Leggi_FasiCicloColturalexSpecie', parametri)
      .pipe(
        map(R => {
          let arrResp = R.RispostaStringa ?? [];
          arrResp = arrResp.map(t => { return new FaseCicloColturale(t.fase_cod, t.fase_des) })

          let fase102 = arrResp.find((el) => { return el.codice == 17 });
          if (!fase102){
            arrResp.push({
              codice: 102,
              descrizione: 'In produzione',
              disciplinarePubblicoPrivato: true
            })
          } else {
            arrResp.find((el) => { return el.codice == 17}).codice = 102;
          }

          this.FasiCicloColturale.push({chiave: chiave, valori: arrResp});
          return arrResp;
        })
      );
  }
}
