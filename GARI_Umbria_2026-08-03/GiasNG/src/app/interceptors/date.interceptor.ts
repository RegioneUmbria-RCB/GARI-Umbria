import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import { Observable, of, switchMap, tap } from 'rxjs';
import { API_BASE_URL } from 'app/Service/api.service';
import { ConversionService } from "../Service/conversion.service";

@Injectable()
export class DateInterceptor implements HttpInterceptor {

    constructor(private conversionService: ConversionService) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let bodyRet;
        try { // Added because [FormData] bodies cannot be json parsed
            bodyRet = this.changeBody(request.body)
        } catch (error) {
            bodyRet = request.body;
        }

        request = request.clone({
            body: bodyRet
        })
        return next.handle(request).pipe(
            switchMap((resp) => {
                return of(resp);
            }));
    }

    changeBody(iBody: string): string {
        let retBody = iBody;
        if (iBody != undefined && iBody != null) {
            let bodyStr = iBody;
            let bodyObj = JSON.parse(bodyStr);
            let convertedBody = this.conversionService.ConversionStrDateInStrDateTimeToServer(bodyObj);
            retBody = JSON.stringify(convertedBody);
        }
        return retBody;
    }

}


