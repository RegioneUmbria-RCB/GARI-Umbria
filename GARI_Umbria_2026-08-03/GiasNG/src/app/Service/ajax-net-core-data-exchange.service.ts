import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, filter, map, Observable } from 'rxjs';
import { ConversionService } from './conversion.service';
import { GiasDialogService } from './gias-dialog.service';
import {
  ErroreGias,
  MasterService,
  rispostaStandard
} from './master.service';

@Injectable({ providedIn: 'root' })
export class AjaxNetCoreDataExchangeService {

  constructor(
    private http: HttpClient,
    private masterService: MasterService,
    private conversionService: ConversionService,
    private giasDialogService: GiasDialogService
  ) {}

  ajaxAPIPost<InType, OutType>(
    url: string,
    parametri: InType,
    setLoading: boolean = false,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<OutType>> {
    const link = url[0] === '/'
      ? this.masterService.link_NetCoreDataExchange + url
      : this.masterService.link_NetCoreDataExchange + '/' + url;
    return this.ajaxPost<OutType, InType>(link, parametri, setLoading, showErroriGestiti, showErroriNonGestiti) as Observable<rispostaStandard<OutType>>;
  }

  private ajaxPost<InType, OutType>(
    url: string,
    parametri: OutType,
    setLoading: boolean = false,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<InType>> {

    if (setLoading) {
      this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
    }

    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      withCredentials: true
    };

    return this.http.post<any>(url, JSON.stringify(parametri), httpOptions).pipe(
      catchError(err => this.errorHandling(err)),
      filter(r => {
        if (<string>r === '401') {
          if (setLoading) {
            this.masterService.set_isLoading({ isLoading: false, message: '' });
          }
          return false;
        }
        return true;
      }),
      map(r => (<rispostaStandard<InType>>r)),
      map(resp => {
        if (resp.RispostaOK === false && showErroriNonGestiti) {
          this.rispostaOK_FalseHandling(resp, showErroriGestiti);
          return resp;
        }
        return resp;
      }),
      map((r: rispostaStandard<InType>) => {
        if (r.RispostaOK === true) {
          let obj = r.RispostaStringa;
          if (typeof r.RispostaStringa === 'string' && r.RispostaStringa !== '') {
            try {
              obj = JSON.parse(r.RispostaStringa);
            } catch (_e) {
              obj = r.RispostaStringa;
            }
          }
          obj = this.conversionService.ConversionDateInObject(obj);
          this.conversionService.remove__type(obj);
          r.RispostaStringa = obj;
          if (setLoading) {
            this.masterService.set_isLoading({ isLoading: false, message: '' });
          }
        }
        return r;
      })
    );
  }

  private errorHandling(err: any): Observable<any> {
    if (err.status === 401) {
      return new Observable(observer => observer.next('401'));
    }
    const message: string = err.statusText ?? err.message ?? 'Errore sconosciuto';
    this.giasDialogService.alertMessage(message);
    throw err;
  }

  private rispostaOK_FalseHandling(resp: rispostaStandard<any>, showErroriGestiti: boolean): void {
    if (showErroriGestiti && resp.ErroriGias?.length) {
      const msg = resp.ErroriGias.map((e: ErroreGias) => e.messaggio).join('\n');
      this.giasDialogService.alertMessage(msg);
    }
  }
}
