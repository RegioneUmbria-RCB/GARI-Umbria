import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from 'app/Service/api.service';

@Injectable()
export class CompressioneInterceptor implements HttpInterceptor {

  constructor(@Inject(API_BASE_URL) private baseUrl?: string) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.indexOf(this.baseUrl) === -1) {
      return next.handle(request);
    }
    return next.handle(request);
  }
}

