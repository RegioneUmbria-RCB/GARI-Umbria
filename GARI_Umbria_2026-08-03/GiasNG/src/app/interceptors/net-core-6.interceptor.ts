import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import {catchError, filter, Observable, switchMap, of as _observableOf, throwError, tap, of} from 'rxjs';
import { API_BASE_URL } from 'app/Service/api.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { MasterService } from 'app/Service/master.service';
import { mergeMap as _observableMergeMap, catchError as _observableCatch, take } from 'rxjs/operators';
import {NETCORE6_API_BASE_URL, RispostaStandard} from "../Service/net-core6-api.service";
import {GiasMessageService} from "../Service/gias-message.service";
import {QdCAC_API_BASE_URL} from "../Service/qdca-compliance-api.service";

@Injectable()
export class NetCore6Interceptor implements HttpInterceptor {

  constructor(
    private ajaxApiService: AjaxAgronicaAPIService,
    private masterService: MasterService,
    private messageService: GiasMessageService,
    @Inject(API_BASE_URL) private baseUrl?: string,
    @Inject(NETCORE6_API_BASE_URL) private baseNetCoreUrl?: string,
    @Inject(QdCAC_API_BASE_URL) private baseQdCAComplianceUrl?: string,
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.indexOf(this.baseNetCoreUrl) === -1
      && request.url.indexOf(this.masterService.link_NetCore6Api) === -1
    ) {
      return next.handle(request);
    }
    return next.handle(request).pipe(
      catchError((e) => {
        console.log("Error", e);
        (<Blob>e.error).text().then((val) => {
          console.log(val);
          try {
            let r = (<RispostaStandard>JSON.parse(val))
            this.messageService.errorMessage(r.Errore);
          } catch (e){
            this.messageService.errorMessage(val);
          }
        })
        throw  new Error(e);
        // return of(e);    // 2024-04-09 Salvatore Zammataro: modifica per permettere gestione errori read delle kendo grid
      })
    );
  }

}

