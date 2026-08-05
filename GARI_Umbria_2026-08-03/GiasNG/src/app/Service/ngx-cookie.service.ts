import { Injectable } from '@angular/core';
import {CookieService} from 'ngx-cookie-service';
import {CookieOptions, SameSite} from 'ngx-cookie-service/lib/cookie.service';

@Injectable({
  providedIn: 'root'
})
export class NgxCookieService {

  constructor(
    private cookieService: CookieService
  ) { }

  check(name: string): boolean {
    return this.cookieService.check(name);
  }

  get(name: string): string {
    return this.cookieService.get(name);
  }

  getAll() {
    return this.cookieService.getAll();
  }

  set(name: string, value: string, options?: CookieOptions): void {
    this.cookieService.set(name, value, options);
  }

  delete(name: string, path?: CookieOptions['path'], domain?: CookieOptions['domain'], secure?: CookieOptions['secure'], sameSite?: SameSite): void {
    this.cookieService.delete(name, path, domain, secure, sameSite);
  }

  deleteAll(path?: CookieOptions['path'], domain?: CookieOptions['domain'], secure?: CookieOptions['secure'], sameSite?: SameSite): void {
    this.cookieService.deleteAll(path, domain, secure, sameSite);
  }
}
