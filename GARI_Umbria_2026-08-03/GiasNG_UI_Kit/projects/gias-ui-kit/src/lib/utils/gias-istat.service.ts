import { Inject, Injectable } from '@angular/core';
import {map, Observable, of, Subject, tap} from 'rxjs';
import { GIAS_API_SERVICE_TOKEN, IGiasApiService } from './gias-api.service';
import { CodiciNazioniISO3166, Comune, GetProvince, ISTAT_GetCAP_Request, Provincia } from './models';
import {debounceTime} from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class GiasIstatService {
  Provincie: Provincia[];
  ComunixProv: Map<string, Comune[]> = new Map<string, Comune[]>();
  Stati: CodiciNazioniISO3166[];

  constructor(@Inject(GIAS_API_SERVICE_TOKEN) private apiService: IGiasApiService) { }

  getProvincie() {
    return this.Provincie.slice();
  }

  getStati() {
    return this.Stati.slice();
  }

  leggiProvincie(Stato_Cod: string, regione: string = ''): Promise<Provincia[]> {
    return new Promise<Provincia[]>(async (resolve, reject) => {
      let parametri: GetProvince = new GetProvince();
      parametri.stato = Stato_Cod;
      parametri.regione = regione;

      this.apiService.ajaxAPIPost<GetProvince, Provincia[]>('Modello/Province', parametri).pipe(map(data => {
        this.Provincie = [];
        data.RispostaStringa?.map((p: any) => {
          this.Provincie.push({
            sigla: p.sigla,
            regione_cod: p.regione.codice,
            regione_des: p.regione.descrizione,
            Provincia_Des: p.descrizione,
            Istat_Prov: p.codice,
            Stato_Country: p.stato.codice,
            comuneDefault: p.comuneDefault
          });
        });
        resolve(this.Provincie);
      })).subscribe();
    });
  }

  readProvinces(Stato_Cod: string, regione: string = ''): Observable<Provincia[]> {
    let parametri: GetProvince = new GetProvince();
    parametri.stato = Stato_Cod;
    parametri.regione = regione;

    return this.apiService.ajaxAPIPost<GetProvince, Provincia[]>('Modello/Province', parametri).pipe(
      map(data => data.RispostaStringa.map((p: any) => {
        return <Provincia>{
          sigla: p.sigla,
          regione_cod: p.regione.codice,
          regione_des: p.regione.descrizione,
          Provincia_Des: p.descrizione,
          Istat_Prov: p.codice,
          Stato_Country: p.stato.codice,
          comuneDefault: p.comuneDefault
        }
      })),
      tap(d => this.Provincie = d)
    );
  }

  readRegioni(cod_stato: string): Observable<any[]> {
    return this.apiService.ajaxAPIPost<string, any[]>('MetaschemaNG/readRegioni', cod_stato).pipe(
      map(r => r.RispostaStringa)
    );
  }

  public readRegionsByProvince(prov: string): Observable<any> {
    return this.apiService.ajaxAPIPost<string, any>('MetaschemaNG/readRegionsByProvince', prov).pipe(
      map(r => r.RispostaStringa)
    );
  }

  /**
   * @deprecated use readCities() instead
   */
  leggiComuni(Prov: string): Promise<Comune[]> {
    return new Promise<Comune[]>(async (resolve, reject) => {
      if (!this.ComunixProv.has(Prov)) {
        this.apiService.ajaxAPIGet<any, Comune[]>('Modello/Comuni/' + Prov,
          Prov
        ).pipe(map(data => {
          this.ComunixProv.set(Prov, data.RispostaStringa);
          resolve(this.ComunixProv.get(Prov));
        })).subscribe();
      } else {
        resolve(this.ComunixProv.get(Prov));
      }
    });
  }

  readCities(province: string): Observable<Comune[]> {
    let result: Observable<Comune[]>;

    if (!this.ComunixProv.has(province)) {
      result = this.apiService.ajaxAPIGet<any, Comune[]>('Modello/Comuni/' + province, province).pipe(
        map(r => r.RispostaStringa),
        tap(d => this.ComunixProv.set(province, d)));
    } else {
      result = of(this.ComunixProv.get(province));
    }

    return result;
  }

  /**
   * @deprecated use readCountries() instead
   */
  leggiStati(): Promise<CodiciNazioniISO3166[]> {
    return new Promise<CodiciNazioniISO3166[]>(async (resolve, reject) => {
      if (this.Stati == undefined) {
        this.apiService.ajaxAPIPost<any, CodiciNazioniISO3166[]>('MetaschemaNG/GetStatiModello', { ValoreRicerca: "" }).pipe(map(
          (data) => {
            this.Stati = data.RispostaStringa;
            resolve(this.Stati);
          })).subscribe();
      } else {
        resolve(this.Stati);
      }
    });
  }

  readCountries(): Observable<CodiciNazioniISO3166[]> {
    let result: Observable<CodiciNazioniISO3166[]>;

    if (this.Stati == undefined || this.Stati.length === 0) {
      result = this.apiService.ajaxAPIPost<any, CodiciNazioniISO3166[]>(
        'MetaschemaNG/GetStatiModello',
        { ValoreRicerca: "" }
      ).pipe(
        map(r => r.RispostaStringa),
        tap((data) => {
          this.Stati = data;
        })
      );
    } else {
      result = of(this.Stati);
    }

    return result;
  }

  /**
   * @deprecated use readPostalCode() instead
   */
  leggiCAP(Prov: string, Com: string): Promise<string> {
    return new Promise<string>(async (resolve, reject) => {
      if (Prov == null || Com == null || Prov == '' || Com == '') {
        resolve('00000');
        return;
      }
      if (Prov == '000' || Com == '000') {
        resolve('00000');
        return;
      } else {
        const parametri: ISTAT_GetCAP_Request = {
          Prov: Prov,
          Com: Com
        };
        this.apiService.ajaxAPIPost<ISTAT_GetCAP_Request, string>('MetaschemaNG/GetCAP', parametri).pipe(map(
          (data) => {
            resolve(data.RispostaStringa.toString());
          })
        ).subscribe();
      }
    });
  }

  readPostalCode(province: string, city: string): Observable<string> {
    let result: Observable<string>;
    if (province == null || city == null || province == '' || city == '' || province == '000' || city == '000') {
      result = of('00000');
    } else {
      const parametri: ISTAT_GetCAP_Request = {
        Prov: province,
        Com: city
      };
      result = this.apiService.ajaxAPIPost<ISTAT_GetCAP_Request, string>('MetaschemaNG/GetCAP', parametri).pipe(
        map((data) => data.RispostaStringa.toString())
      );
    }

    return result;
  }
}
