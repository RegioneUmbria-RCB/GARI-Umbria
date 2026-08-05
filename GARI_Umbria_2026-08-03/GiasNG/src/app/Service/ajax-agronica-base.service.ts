import { HttpClient, HttpHeaders, HttpParams, HttpResponseBase } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, filter, map, Observable, of } from 'rxjs';
import { IGiasApiService, isJSON } from 'gias-ui-kit';
import { ConversionService } from './conversion.service';
import { GiasDialogService } from './gias-dialog.service';
import {
  enum_ErroreGias_Tipo,
  ErroreGias,
  ErroreGias_Severity,
  MasterService,
  rispostaStandard
} from './master.service';

@Injectable()
export abstract class AbstractAjaxAgronicaService implements IGiasApiService {
  private readonly _pako: any;

  constructor(
    protected readonly http: HttpClient,
    protected readonly masterService: MasterService,
    protected readonly conversionService: ConversionService,
    protected readonly giasDialogService: GiasDialogService,
  ) {
    this._pako = require('pako');
  }

  /** Each concrete subclass provides its own API base URL. */
  protected abstract get baseUrl(): string;

  // ── Public API ──────────────────────────────────────────────────────────────

  ajaxAPIPost<InType, OutType>(
    url: string,
    parametri: InType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<OutType>> {
    return this.ajaxPost<OutType, InType>(
      this.buildUrl(url), parametri, setLoading, compressione, showErroriGestiti, showErroriNonGestiti,
    ) as Observable<rispostaStandard<OutType>>;
  }

  ajaxAPIGet<InType, OutType>(
    url: string,
    parametri: InType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<OutType>> {
    return this.ajaxGet<OutType, InType>(
      this.buildUrl(url), parametri, setLoading, compressione, showErroriGestiti, showErroriNonGestiti,
    ) as Observable<rispostaStandard<OutType>>;
  }

  // ── Protected HTTP helpers (available to subclasses) ──────────────────────

  protected buildUrl(url: string): string {
    return url[0] === '/' ? this.baseUrl + url : `${this.baseUrl}/${url}`;
  }

  protected ajaxPost<InType, OutType>(
    url: string,
    parametri: OutType,
    setLoading: boolean = false,
    _compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<InType>> {
    if (setLoading) {
      this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
    }
    const headers: HttpHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });
    const source$ = this.http.post<any>(url, JSON.stringify(parametri), { headers, withCredentials: true });
    return this.buildPipeline<InType>(source$, setLoading, showErroriGestiti, showErroriNonGestiti, true);
  }

  protected ajaxPostFormData<OutType>(
    url: string, parametri: FormData,
    setLoading: boolean = false,
    _compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<OutType>> {
    if (setLoading) {
      this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
    }
    const source$ = this.http.post<any>(url, parametri);
    return this.buildPipeline<OutType>(source$, setLoading, showErroriGestiti, showErroriNonGestiti, true);
  }

  protected ajaxGet<InType, OutType>(
    url: string,
    parametri: OutType,
    setLoading: boolean = false,
    _compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true
  ): Observable<rispostaStandard<InType>> {
    if (setLoading) {
      this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
    }
    const headers: HttpHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });
    const queryParams: HttpParams = this.buildQueryParams(parametri);
    const source$ = this.http.get<any>(url, { params: queryParams, headers, withCredentials: true });
    return this.buildPipeline<InType>(source$, setLoading, showErroriGestiti, showErroriNonGestiti, false);
  }

  // ── Private helpers ────────────────────────────────────────────────────────

  private buildQueryParams<T>(parametri: T): HttpParams {
    let params: HttpParams = new HttpParams();
    try {
      for (const key of Object.getOwnPropertyNames(parametri)) {
        params = params.append(key, (parametri as any)[key]);
      }
    } catch {
      params = new HttpParams({ fromObject: JSON.stringify(parametri) as any });
    }
    return params;
  }

  /**
   * Unified response pipeline: error handling → 401 filter → decompress
   * → RispostaOK=false handling → parse & convert.
   *
   * @param tryJsonParse true = try/catch JSON.parse (POST); false = isJSON guard (GET)
   */
  private buildPipeline<T>(
    source$: Observable<any>,
    setLoading: boolean,
    showErroriGestiti: boolean,
    showErroriNonGestiti: boolean,
    tryJsonParse: boolean
  ): Observable<rispostaStandard<T>> {
    return source$.pipe(
      catchError((err: HttpResponseBase) => this.errorHandling(err)),
      filter((r: any): boolean => {
        if (r === '401') {
          if (setLoading) {
            this.masterService.set_isLoading({ isLoading: false, message: '' });
          }
          return false;
        }
        return true;
      }),
      map((r: any): rispostaStandard<T> => r),
      map((val) => this.decomprimi(val)),
      map((resp) => {
        if (resp.RispostaOK === false && showErroriNonGestiti) {
          this.rispostaOK_FalseHandling(resp, showErroriGestiti);
        }
        return resp;
      }),
      map((r) => this.parseAndConvert(r, setLoading, tryJsonParse)),
    ) as Observable<rispostaStandard<T>>;
  }

  private parseAndConvert<T>(
    r: rispostaStandard<T>,
    setLoading: boolean,
    tryJsonParse: boolean
  ): rispostaStandard<T> {
    if (r.RispostaOK !== true) {
      return r;
    }
    let obj: any = r.RispostaStringa;
    if (typeof obj === 'string' && obj !== '') {
      if (tryJsonParse) {
        try { obj = JSON.parse(obj); } catch { /* keep raw string */ }
      } else if (isJSON(obj)) {
        obj = JSON.parse(obj);
      }
    }
    obj = this.conversionService.ConversionDateInObject(obj); // result must be stored back
    this.conversionService.remove__type(obj);
    r.RispostaStringa = obj;
    if (setLoading) {
      this.masterService.set_isLoading({ isLoading: false, message: '' });
    }
    return r;
  }

  private errorHandling(error: HttpResponseBase): Observable<any> {
    if (error.status !== 401) {
      this.masterService.set_isLoading({ isLoading: false });
      return of((error as any).error);
    }
    return of('401');
  }

  private rispostaOK_FalseHandling(r: rispostaStandard<any>, showErroriGestiti: boolean): void {
    this.masterService.set_isLoading({ isLoading: false, message: '' });
    if (r.ErroriGias?.length > 0) {
      const erroriNonGestiti = r.ErroriGias.filter((e) => e.tipo === enum_ErroreGias_Tipo.NonGestito);
      this.handleErrori_NonGestiti(erroriNonGestiti);
      if (showErroriGestiti) {
        const erroriGestiti = r.ErroriGias.filter((e) => e.tipo !== enum_ErroreGias_Tipo.NonGestito);
        this.handleErrori_Gestiti(erroriGestiti);
      }
    } else {
      this.masterService.changeErrorMsgType({ show: true, msg: r.Errore, errorNumber: 500 });
    }
  }

  private handleErrori_NonGestiti(errori: ErroreGias[]): void {
    errori.forEach((e) =>
      this.masterService.changeErrorMsgType({ show: true, msg: e.messaggio, errorNumber: 500 }),
    );
  }

  private handleErrori_Gestiti(errori: ErroreGias[]): void {
    errori
      .filter((e) => e.severity === ErroreGias_Severity.Bloccante)
      .forEach((e) => this.giasDialogService.baseError('', e.messaggio));
    // TODO: add handling for Warning and Info severities
  }

  private decomprimi(r: rispostaStandard<any>): rispostaStandard<any> {
    if (r.Compressa && this._pako && r.RispostaCompressa?.length > 0) {
      try {
        r.RispostaStringa = this._pako.inflate(r.RispostaCompressa, { to: 'string' });
        r.RispostaCompressa = null;
      } catch (e) {
        console.log(e);
      }
    }
    return r;
  }
}
