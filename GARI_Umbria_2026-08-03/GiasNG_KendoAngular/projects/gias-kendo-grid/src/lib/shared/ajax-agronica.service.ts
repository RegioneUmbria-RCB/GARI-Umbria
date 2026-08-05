import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponseBase } from '@angular/common/http';
import { catchError, map, take } from 'rxjs/operators';
import { lastValueFrom, Observable, of } from 'rxjs';
import { CommonFunctionsService } from '../common-function.service';
import { inflate } from 'pako';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService, rispostaStandard, RispostaStandard, enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity, ConversionService, GiasDialogService } from 'gias-ui-kit';

export class CoreWS_GenericPayload {
    constructor(public obj: CoreWS_GenericObjP) {

    }
}

export class CoreWS_Generic<T> extends CoreWS_GenericPayload {
    InData: T;

    constructor(objP: CoreWS_GenericObjP, InData: T) {
        super(objP);
        this.InData = InData;
    }
}

export class CoreWS_GenericObjP {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

@Injectable({ providedIn: 'root' })
export class AjaxAgronicaService {
    constructor(private http: HttpClient,
        @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterService: IGiasMasterService,
        private conversionService: ConversionService,
        private giasDialogService: GiasDialogService) {
    }

    ajaxAgronicaCoreWS_Promise<InType, OutType>(
        url,
        parametri: CoreWS_Generic<OutType>,
        setLoading: boolean = true,
        showErroriGestiti: boolean = true): Promise<rispostaStandard<InType>> {
        console.error('La funzione ha chiamato direttamente i CoreWS (' + url + '), ritornando una Promise. Da cambiare in una chiamata WebAPI!')
        return this.ajaxPost(url, parametri, true, setLoading, showErroriGestiti) as Promise<rispostaStandard<InType>>;
    }

    ajaxCoreWSPost<InType, OutType>(url: string,
        parametri: CoreWS_Generic<InType>,
        setLoading: boolean = true,
        compressione: boolean = true,
        showErroriGestiti: boolean = true): Observable<rispostaStandard<OutType>> {
        console.error('La funzione ha eseguito una chiamata Post sui CoreWS (' + url + '). Da cambiare in una chiamata WebAPI!')
        return this.ajaxPost(url, parametri, false, setLoading, compressione, showErroriGestiti) as Observable<rispostaStandard<OutType>>;
    }

    ajaxAgronica(url, parametri, compressione: boolean = true, showErroriGestiti: boolean = true): Promise<RispostaStandard> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        return lastValueFrom(this.http.post<any>(url, JSON.stringify(parametri), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<RispostaStandard>r.d);
                    return resp;
                }),
                map((val) => {
                    return this.decomprimi(val)
                }),
                map(resp => {
                    if (resp.RispostaOK === false) {
                        this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                        return resp;
                    }
                    return resp;
                })
            )
        );
    }

    ajaxAgronicaObs(url, parametri, compressione: boolean = true, showErroriGestiti: boolean = true): Observable<RispostaStandard> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        return this.http.post<any>(url, JSON.stringify(parametri), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<RispostaStandard>r.d);
                    return resp;
                }),
                map((val) => {
                    return (<any>this.decomprimi(val));
                }),
                map(r => {
                    const resp = (<any>r);
                    if (resp.RispostaOK === false) {
                        return this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                    }
                    return resp;
                })
            );
    }

    ajaxAgronicaG<T>(url, parametri, compressione: boolean = true, showErroriGestiti: boolean = true): Promise<rispostaStandard<T>> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        return lastValueFrom(this.http.post<any>(url, JSON.stringify(parametri), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<RispostaStandard>r.d);
                    return resp;
                }),
                map((val) => {
                    return (<any>this.decomprimi(val));
                }),
                map(r => {
                    const resp = (<rispostaStandard<T>>r);
                    if (resp.RispostaOK === false) {
                        return this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                    }
                    return r;
                }),
                map((r: rispostaStandard<T>) => {
                    const obj = r.RispostaStringa;

                    this.conversionService.ConversionDateInObject(obj);
                    this.conversionService.remove__type(obj);
                    return r;
                })))
    }

    /**
   * Per le chiamate utilizzando codice vecchio non ancora adattato alla
   * nuova modalità di caricare i dati (tramite RispostaStandard generic).
   * @param url Endpoint.
   * @param parametri I parametri che compaiono nella testata della chiamata.
   * @returns il risultato della chiamata http.
   */
    legacy_get(url, parametri, displayErrorMessage: boolean = true, compressione: boolean = true): Observable<RispostaStandard> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        return this.http.post<any>(url, JSON.stringify(parametri), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<RispostaStandard>r.d);
                    return resp;
                }),
                map((val) => {
                    return (<any>this.decomprimi(val));
                }),
                map(r => {
                    const resp = (<RispostaStandard>r);
                    if (resp.RispostaOK === false && displayErrorMessage) {
                        return this.rispostaOK_FalseHandling(resp, displayErrorMessage).pipe(
                            take(1),
                            map((val) => {
                                return val;
                            })
                        );
                    }
                    return r;
                }));
    }

    post<T>(url, parametri, compressione: boolean = true, gestisciErrore = true, showErroriGestiti: boolean = true): Observable<rispostaStandard<T>> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        return this.http.post<any>(url, JSON.stringify(parametri), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<RispostaStandard>r.d);
                    return resp;
                }),
                map((val) => {
                    return (<any>this.decomprimi(val));
                }),
                map(r => {
                    const resp = (<rispostaStandard<T>>r);
                    if (resp.RispostaOK === false && !resp.RispostaConferma && gestisciErrore) {
                        return this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                    }
                    return r;
                })).pipe(map((r: rispostaStandard<T>) => {
                    const obj = r.RispostaStringa;
                    this.conversionService.ConversionDateInObject(obj);
                    this.conversionService.remove__type(obj);
                    return r;
                }));
    }

    ajaxAgronicaCoreWS_GenericsObs<T1, T2>(url, parametri: CoreWS_Generic<T2>, compressione: boolean = true, showErroriGestiti: boolean = true): Observable<rispostaStandard<T1>> {
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };
        console.error('La funzione ha chiamato direttamente i CoreWS (' + url + '), ritornando un Observable. Da cambiare in una chiamata WebAPI!')
        const p1 = { InData: parametri };
        return this.ajaxCoreWSPost(url, parametri, false, compressione, showErroriGestiti);
    }

    public ajaxCoreWSGet<OutType>(url: string, showErroriGestiti: boolean = true): Observable<rispostaStandard<OutType>> {
        this.giasMasterService.set_isLoading({
            isLoading: true,
            message: 'Caricamento in corso'
        });

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            })
        };

        return this.http.get<any>(url, httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map(r => {
                    const resp = (<rispostaStandard<OutType>>r.d);
                    if (resp.RispostaOK === false) {
                        return this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                    }
                    return r.d;
                }));
    }

    private ajaxPost<InType, OutType>(url,
        parametri: CoreWS_Generic<OutType>,
        returnPromise: boolean = true,
        setLoading: boolean = true,
        compressione: boolean = true,
        showErroriGestiti: boolean = true): Promise<rispostaStandard<InType>> | Observable<rispostaStandard<InType>> {

        if (setLoading) {
            this.giasMasterService.set_isLoading({
                isLoading: true,
                message: 'Caricamento in corso'
            });
        }

        // let headers = new Headers();
        // headers.append('Content-Type', 'application/json');
        // headers.append('x-compressione', compressione.toString());

        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'x-compressione': compressione.toString()
            })
        };

        const p1 = { InData: parametri };

        // this.conversionService.remove__type(p1);
        const risposta = this.http.post<any>(url, JSON.stringify(p1), httpOptions)
            .pipe(
                catchError((err) => {
                    return this.errorHandling(err);
                }),
                map((r) => {
                    const resp = (<rispostaStandard<InType>>r.d);
                    return resp;
                }),
                map((val) => {
                    return this.decomprimi(val)
                }),
                map(resp => {
                    if (resp.RispostaOK === false) {
                        this.rispostaOK_FalseHandling(resp, showErroriGestiti);
                        return resp;
                    }
                    return resp;
                })).pipe(map((r: rispostaStandard<InType>) => {
                    if (r.RispostaOK === true || r.RispostaStringa) {
                        let obj = r.RispostaStringa;
                        if (typeof r.RispostaStringa == 'string' && r.RispostaStringa != "" && CommonFunctionsService.isJSON(r.RispostaStringa)) {
                            obj = JSON.parse(r.RispostaStringa);
                        }
                        this.conversionService.ConversionDateInObject(obj);
                        this.conversionService.remove__type(obj);
                        r.RispostaStringa = obj;
                        if (setLoading) {
                            this.giasMasterService.set_isLoading({ isLoading: false, message: '' });
                        }
                        return r;
                    } else {
                        return r;
                    }
                }));
        if (returnPromise) {
            return risposta.toPromise();
        } else {
            return risposta;
        }
    }


    errorHandling(error: HttpResponseBase): Observable<any> {
        let messaggio = "statusText:" + error.statusText + " - url:" + error.url;
        if ((<any>error).error) {
            messaggio += " error: " + JSON.stringify((<any>error).error)
        }
        this.giasMasterService.changeErrorMsgType({ show: true, msg: error.statusText, errorNumber: error.status });
        return of(new Error(messaggio));
    }

    rispostaOK_FalseHandling(r: rispostaStandard<any>, showErroriGestiti: boolean): Observable<any> {
        this.giasMasterService.set_isLoading({ isLoading: false, message: '' });
        if (r.ErroriGias && r.ErroriGias.length > 0) {

            const erroriNonGestiti = r.ErroriGias.filter((e) => e.tipo == enum_ErroreGias_Tipo.NonGestito);
            this.handleErrori_NonGestiti(erroriNonGestiti);

            if (showErroriGestiti) {
                const erroriGestiti = r.ErroriGias.filter((e) => e.tipo != enum_ErroreGias_Tipo.NonGestito);
                this.handleErrori_Gestiti(erroriGestiti);
            }


        } else {

            this.giasMasterService.changeErrorMsgType({ show: true, msg: r.Errore, errorNumber: 500 });

        }
        return of(new Error(JSON.stringify(r)));
    }

    handleErrori_NonGestiti(errori: ErroreGias[]) {
        errori.forEach((e) => {
            this.giasMasterService.changeErrorMsgType({ show: true, msg: e.messaggio, errorNumber: 500 });
        })
    }

    handleErrori_Gestiti(errori: ErroreGias[]) {
        const bloccanti = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
        bloccanti.forEach((e) => {
            this.giasDialogService.baseError('', e.messaggio);
        });
        const warning = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
        const info = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
    }

    private decomprimi(r: rispostaStandard<any>): rispostaStandard<any> {
        if (r && r?.Compressa) {
            if (r.RispostaCompressa !== undefined && r.RispostaCompressa !== null && r.RispostaCompressa.length > 0) {
                r.RispostaStringa = inflate(r.RispostaCompressa, { to: 'string' });
                r.RispostaCompressa = null;
            }
        }
        return r;
    }

}
