import { HttpClient, HttpHeaders, HttpParams, HttpResponseBase } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, filter, map, Observable, of } from "rxjs";
import { isJSON } from 'gias-ui-kit';
import { ConversionService } from "./conversion.service";
import { GiasDialogService } from "./gias-dialog.service";
import { enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity, MasterService, rispostaStandard } from "./master.service";

@Injectable({ providedIn: 'root' })
export class AjaxAgronicaNetCore6ApiService {
  private pako;

  constructor(private http: HttpClient,
    private masterService: MasterService,
    private conversionService: ConversionService,
    private giasDialogService: GiasDialogService) {
    this.pako = require('pako');
  }

  /**
   * @param showErroriGestiti indica se mostrare gli errori contenuti nel campo `.ErroriGias`
   * @param showErroriNonGestiti indica se mostrare gli errori contenuti nel campo `.Errore` (errori server, mostrati nel popup in basso a destra)
   */
  ajaxAPIPost<InType, OutType>(url: string,
    parametri: InType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true): Observable<rispostaStandard<OutType>> {
    let link = ""
    if (url[0] == "/") {
      link = this.masterService.link_NetCore6Api + url;
    } else {
      link = this.masterService.link_NetCore6Api + "/" + url;
    }
    return this.ajaxPost(link, parametri, setLoading, compressione, showErroriGestiti, showErroriNonGestiti) as Observable<rispostaStandard<OutType>>;
  }

  ajaxAPIGet<InType, OutType>(url: string,
    parametri: InType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true): Observable<rispostaStandard<OutType>> {
    let link = ""
    if (url[0] == "/") {
      link = this.masterService.link_NetCore6Api + url;
    } else {
      link = this.masterService.link_NetCore6Api + "/" + url;
    }
    return this.ajaxGet(link, parametri, setLoading, compressione, showErroriGestiti, showErroriNonGestiti) as Observable<rispostaStandard<OutType>>;
  }

  private ajaxPost<InType, OutType>(url,
    parametri: OutType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true): Observable<rispostaStandard<InType>> {

    if (setLoading) {
      this.masterService.set_isLoading({
        isLoading: true,
        message: 'Caricamento in corso'
      });
    }

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      //'x-compressione': compressione.toString(),
      //'Access-Control-Allow-Origin': window.location.origin,
      //'Access-Control-Allow-Headers': 'Access-Control-Allow-Headers, Origin, Accept, Authorization, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers',
      //'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
      //'Access-Control-Allow-Credentials': 'true'
    });

    const httpOptions = {
      headers: headers,
      withCredentials: true
    };

    // const p1 = { InData: parametri };
    const p1 = parametri;

    // this.conversionService.remove__type(p1);
    const risposta = this.http.post<any>(url, JSON.stringify(p1), httpOptions)
      .pipe(
        catchError((err) => {
          return this.errorHandling(err);
        }),
        filter((r) => {
          if (<string>r == "401") {
            if (setLoading) {
              this.masterService.set_isLoading({
                isLoading: false,
                message: 'Caricamento in corso'
              });
            }
            return false;
          }
          return true;
        }),
        map((r) => {
          const resp = (<rispostaStandard<InType>>r);
          return resp;
        }),
        map((val) => {
          return this.decomprimi(val)
        }),
        map(resp => {
          if (resp.RispostaOK === false && showErroriNonGestiti) {
            this.rispostaOK_FalseHandling(resp, showErroriGestiti);
            return resp;
          }
          return resp;
        })).pipe(map((r: rispostaStandard<InType>) => {
          if (r.RispostaOK === true) {
            let obj = r.RispostaStringa;
            if (typeof r.RispostaStringa == 'string' && r.RispostaStringa != "") { // && isJSON(r.RispostaStringa)
              try {
                obj = JSON.parse(r.RispostaStringa);
              } catch (e) {
                obj = r.RispostaStringa;
              }
            }
            // const labelConversionDateInObject = url + " - ConversionDateInObject";
            // console.time(labelConversionDateInObject);
            // let start = new Date();
            obj = this.conversionService.ConversionDateInObject(obj);
            // let end = new Date();
            // let seconds = (end.getTime() - start.getTime()) / 1000;
            // console.log("ConversionDateInObject " + seconds + 's');
            // console.timeEnd(labelConversionDateInObject);
            this.conversionService.remove__type(obj);
            r.RispostaStringa = obj;
            if (setLoading) {
              this.masterService.set_isLoading({ isLoading: false, message: '' });
            }
            return r;
          } else {
            return r;
          }

        }));

    return risposta;
  }

  private ajaxGet<InType, OutType>(url,
    parametri: OutType,
    setLoading: boolean = false,
    compressione: boolean = true,
    showErroriGestiti: boolean = true,
    showErroriNonGestiti: boolean = true): Observable<rispostaStandard<InType>> {

    if (setLoading) {
      this.masterService.set_isLoading({
        isLoading: true,
        message: 'Caricamento in corso'
      });
    }

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      //'x-compressione': compressione.toString(),
      // 'Access-Control-Allow-Origin': url,
      // 'Access-Control-Allow-Headers': 'Access-Control-Allow-Headers, Origin, Accept, Authorization, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers',
      // 'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
      // 'Access-Control-Allow-Credentials': 'true',
      //'InData': parametri
    });

    const httpOptions = {
      headers: headers,
      withCredentials: true
    };
    let queryParams: HttpParams = new HttpParams();


    try {
      let keys = Object.getOwnPropertyNames(parametri);
      for (let i = 0; i < keys.length; i++) {
        queryParams = queryParams.append(keys[i], parametri[keys[i]]);
      }
    } catch (e) {
      let indata = JSON.stringify(parametri);

      queryParams = new HttpParams({ fromObject: indata as any });
    }

    // this.conversionService.remove__type(p1);
    const risposta = this.http.get<any>(url, { params: queryParams, headers: headers, withCredentials: true })
      .pipe(
        catchError((err) => {
          return this.errorHandling(err);
        }),
        map((r) => {
          const resp = (<rispostaStandard<InType>>r);
          return resp;
        }),
        map((val) => {
          return this.decomprimi(val)
        }),
        map(resp => {
          if (resp.RispostaOK === false && showErroriNonGestiti) {
            this.rispostaOK_FalseHandling(resp, showErroriGestiti);
            return resp;
          }
          return resp;
        })).pipe(map((r: rispostaStandard<InType>) => {
          if (r.RispostaOK === true) {
            let obj = r.RispostaStringa;
            if (typeof r.RispostaStringa == 'string' && r.RispostaStringa != "" && isJSON(r.RispostaStringa)) {
              obj = JSON.parse(r.RispostaStringa);
            }
            this.conversionService.ConversionDateInObject(obj);
            this.conversionService.remove__type(obj);
            r.RispostaStringa = obj;
            if (setLoading) {
              this.masterService.set_isLoading({ isLoading: false, message: '' });
            }
            return r;
          } else {
            return r;
          }

        }));
    return risposta;
  }

  private errorHandling(error: HttpResponseBase): Observable<any> {
    if (error.status != 401) {
      this.masterService.set_isLoading({ isLoading: false });
      let messaggio = "statusText:" + error.statusText + " - url:" + error.url;
      if ((<any>error).error) {
        messaggio += " error: " + JSON.stringify((<any>error).error)
      }
      //this.masterService.changeErrorMsgType({ show: true, msg: error.statusText, errorNumber: error.status });
      return of((<any>error).error);
    } else {
      return of("401")
    }
  }

  private rispostaOK_FalseHandling(r: rispostaStandard<any>, showErroriGestiti: boolean): Observable<any> {
    this.masterService.set_isLoading({ isLoading: false, message: '' });
    if (r.ErroriGias && r.ErroriGias.length > 0) {

      const erroriNonGestiti = r.ErroriGias.filter((e) => e.tipo == enum_ErroreGias_Tipo.NonGestito);
      this.handleErrori_NonGestiti(erroriNonGestiti);

      if (showErroriGestiti) {
        const erroriGestiti = r.ErroriGias.filter((e) => e.tipo != enum_ErroreGias_Tipo.NonGestito);
        this.handleErrori_Gestiti(erroriGestiti);
      }

    } else {

      this.masterService.changeErrorMsgType({ show: true, msg: r.Errore, errorNumber: 500 });

    }
    return of(new Error(JSON.stringify(r)));
  }

  private handleErrori_NonGestiti(errori: ErroreGias[]) {
    errori.forEach((e) => {
      this.masterService.changeErrorMsgType({ show: true, msg: e.messaggio, errorNumber: 500 });
    })
  }

  private handleErrori_Gestiti(errori: ErroreGias[]) {
    const bloccanti = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
    bloccanti.forEach((e) => {
      this.giasDialogService.baseError('', e.messaggio);
    });
    const warning = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
    const info = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
  }

  private decomprimi(r: rispostaStandard<any>): rispostaStandard<any> {
    if (r == undefined && r.Compressa && this.pako) {
      if (r.RispostaCompressa !== undefined && r.RispostaCompressa !== null && r.RispostaCompressa.length > 0) {
        try {
          r.RispostaStringa = this.pako.inflate(r.RispostaCompressa, { to: 'string' });
          r.RispostaCompressa = null;
        } catch (e) {
          console.log(e);
        }
      }
    }
    return r;
  }

}
