import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, filter, Observable, switchMap, of as _observableOf, throwError, tap } from 'rxjs';
import { API_BASE_URL } from 'app/Service/api.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { MasterService } from 'app/Service/master.service';
import { mergeMap as _observableMergeMap, catchError as _observableCatch, take } from 'rxjs/operators';
import { NETCORE6_API_BASE_URL } from "../Service/net-core6-api.service";
import { QdCAC_API_BASE_URL } from "../Service/qdca-compliance-api.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private ajaxApiService: AjaxAgronicaAPIService,
    private masterService: MasterService,
    @Inject(API_BASE_URL) private baseUrl?: string,
    @Inject(NETCORE6_API_BASE_URL) private baseNetCoreUrl?: string,
    @Inject(QdCAC_API_BASE_URL) private baseQdCAComplianceUrl?: string,
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.indexOf(this.baseUrl) === -1 &&
      request.url.indexOf(this.baseNetCoreUrl) === -1 &&
      request.url.indexOf(this.baseQdCAComplianceUrl) === -1
    ) {
      const dupReq = request.clone();
      request = dupReq;
    } else if (request.url.indexOf(this.baseUrl) != -1) {

      let newUrl = request.url.replace(this.baseUrl, this.masterService.link_API);
      const dupReq = request.clone({ url: newUrl });
      request = dupReq;

    } else if (request.url.indexOf(this.baseNetCoreUrl) != -1) {
      let newUrl = request.url.replace(this.baseNetCoreUrl, this.masterService.link_NetCore6Api);
      const dupReq = request.clone({ url: newUrl });
      request = dupReq;
    } else if (request.url.indexOf(this.baseQdCAComplianceUrl) != -1) {
      let newUrl = request.url.replace(this.baseQdCAComplianceUrl, this.masterService.link_QdCA_Compliance);
      const dupReq = request.clone({ url: newUrl });
      request = dupReq;
    }

    if (request.headers.has('skipWithCredentials')) {
      request = request.clone({
        headers: request.headers.delete('skipWithCredentials')
      });
    } else {
      request = request.clone({
        withCredentials: true
      });
    }

    let httpError = {
      url: request.url,
      error: false
    };

    return next.handle(request).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            if (!this.masterService.getIsRefreshing()) {
              this.masterService.changeIsRefreshing(true)
              httpError.error = true;
              return this.ajaxApiService.ajaxAPIPost('Login/RefreshToken', {}).pipe(
                switchMap((resp) => {
                  this.masterService.changeIsRefreshing(false);
                  return next.handle(request);
                })
              );
            } else {
              return this.masterService.isRefreshingSource.pipe(
                filter((val) => { return !val }),
                take(1),
                switchMap((resp) => {
                  return next.handle(request);
                })
              )
            }
          } else {
            return throwError(() => new HttpErrorResponse(error));
          }
        }
      }),
      //   filter((val) => {
      //   if (httpError.error) {
      //     return true;
      //   }
      //   return true;
      // })
    );
  }

  // private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
  //   console.log("handle401Error");
  //   if (!this.isRefreshing) {
  //     this.isRefreshing = true;
  //     return this.ajaxApiService.ajaxAPIPost('Login/RefreshToken', {}).subscribe((resp) => {
  //       this.isRefreshing = false;
  //     });
  //   }
  // }

  // private consoleLogResponseError(response: HttpErrorResponse) {
  //     console.log('viewResponseError');
  //     const status = response.status;
  //     const responseBlob =
  //         response instanceof HttpErrorResponse ? response.error :
  //         (response as any).error instanceof Blob ? (response as any).error : undefined;
  //     let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }};
  //     this.blobToText(responseBlob).subscribe(_responseText => {
  //         console.log(JSON.parse(_responseText));
  //     });
  // }

  // private blobToText(blob: any): Observable<string> {
  //     return new Observable<string>((observer: any) => {
  //         if (!blob) {
  //             observer.next("");
  //             observer.complete();
  //         } else {
  //             let reader = new FileReader();
  //             reader.onload = event => {
  //                 observer.next((event.target as any).result);
  //                 observer.complete();
  //             };
  //             reader.readAsText(blob);
  //         }
  //     });
  // }

}

