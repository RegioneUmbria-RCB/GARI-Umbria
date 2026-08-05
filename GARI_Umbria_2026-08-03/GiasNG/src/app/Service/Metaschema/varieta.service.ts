import {Injectable} from '@angular/core';
import {Specie} from 'app/Model/metaschema/utilizzi/Specie';
import {Varieta} from 'app/Model/metaschema/utilizzi/Varieta';
import {filter, map, Observable, of, take, tap} from 'rxjs';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {MasterService} from '../master.service';

export class VarietaxSpecie {
  Specie: Specie;
  Varieta: Varieta[];
}

export class LeggiCultivar {
  constructor(
    public specie: Specie,
    public cache: boolean = true,
  ) {  }
}

@Injectable({
  providedIn: 'root'
})
export class VarietaService {
  private VarietaxSpecie: VarietaxSpecie[] = new Array();

  /** Maps every species to its cultivars. */
  private _VxSMap: Map<number, Varieta[]> = new Map();

  constructor(
    private masterService: MasterService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
  ) {  }

  /**
   * Reads the cultivars available for the specified species.
   * @param specie the species for which read the cultivars
   * @param serverCache if `true` caches the result server side
   */
  leggiAsObs(specie: Specie, serverCache: boolean = true): Observable<Varieta[]> {
    if (specie.codice == 0) {
      return of([])
    } else if (this._VxSMap.has(specie.codice)) {
      return of(this._VxSMap.get(specie.codice));
    } else {
      return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCultivar, Varieta[]>(
        'MetaschemaNG/LeggiFiltroUtente', new LeggiCultivar(specie, serverCache), false
      ).pipe(
        take(1), filter(R => R.RispostaOK === true),
        map(R => R.RispostaOK ? R.RispostaStringa : []),
        tap((varieta: Varieta[]) => this._VxSMap.set(specie.codice, varieta))
      );
    }
  }

  leggi(specie: Specie): Promise<Varieta[]> {
    return new Promise<Varieta[]>(async (resolve, reject) => {
      if (specie.codice == 0) {
        resolve(new Array<Varieta>());
      }
      if (this.VarietaxSpecie.find((el) => el.Specie.codice == specie.codice) == undefined) {

        this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCultivar, Varieta[]>('MetaschemaNG/LeggiFiltroUtente',
          new LeggiCultivar(specie),
          false).pipe(map(R => {
          this.VarietaxSpecie.push({Specie: specie, Varieta: R.RispostaStringa});
          resolve(this.VarietaxSpecie.find((el) => {
            if (el.Specie.codice == specie.codice) {
              return el;
            }
          }).Varieta);
        })).subscribe();

      } else {

        resolve(this.VarietaxSpecie.find((el) => {
          if (el.Specie.codice == specie.codice) {
            return el;
          }
        }).Varieta);

      }
    });
  }

  /**
   * Reads the cultivar 'Other' for the specified species.
   * @param specie the species for which read the cultivar
   */
  leggiVarietaAltreAsObs(specie: Specie): Observable<Varieta> {
    if (specie.codice == 0) {
      return of({codice: 0, descrizione: '', specie: {codice: 0, descrizione: ''}} as Varieta)
    } else if (this._VxSMap.has(specie.codice)) {
      const elencoVarieta: Varieta[] = this._VxSMap.get(specie.codice);
      const varietaAltre = elencoVarieta.find((el) => el.descrizione.toLocaleLowerCase() == 'altre');
      return of(varietaAltre);
    } else {
      return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCultivar, Varieta[]>(
        'MetaschemaNG/LeggiFiltroUtente', new LeggiCultivar(specie), false
      ).pipe(
        take(1), filter(R => R.RispostaOK === true),
        map(R => R.RispostaOK ? R.RispostaStringa : []),
        tap((R: Varieta[]) => this._VxSMap.set(specie.codice, R)),
        map(R => R.find((el) => el.descrizione.toLocaleLowerCase() == 'altre'))
      );
    }
  }

}
