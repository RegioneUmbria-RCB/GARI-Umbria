import { Injectable } from '@angular/core';

export interface Cookie {
  name: string;
  value: any;
  expireDays: number;
  /** If set to `true` the cookie will not expire. */
  session: boolean;
  /** If set to `true` and the page is not https then secure will not apply. */
  secure: boolean;
  /** If not set or value is not greater than 0, the default value will be root. */
  path: string;
  port: number;
}

@Injectable({
  providedIn: 'root'
})
export class CookieService {

  /**
   * @param name the name with which the cookie was saved
   * @returns the value of the cookie or an empty string if the cookie was not found
   */
  public getCookie(name: string): string {
    const ca: string[] = document.cookie.split(';');
    const caLen: number = ca.length;
    const cookieName = `${name}=`;
    let c: string;
    for (let i = 0; i < caLen; i += 1) {
      c = ca[i].replace(/^\s+/g, '');
      if (c.indexOf(cookieName) == 0) {
        let cookie_value: string = c.substring(cookieName.length, c.length);
        return this.GetValue(cookie_value);
      }
    }
    return "";
  }

  public deleteCookie(cookieName: string) {
    this.setCookie({ name: cookieName, value: '', expireDays: -1 });
  }

  /**
   * Expires default 1 day.
   * If params.session is set and true expires is not added.
   * If params.path is not set or value is not greater than 0 its default value will be root "/".
   * Secure flag can be activated only with https implemented.
   * @Example
   * {service instance}.setCookie({name:'token',value:'abcd12345', session:true }); // This cookie will not expire
   * {service instance}.setCookie({name:'userName',value:'John Doe', secure:true }); // If page is not https then secure will not apply
   * {service instance}.setCookie({name:'niceCar', value:'red', expireDays:10 }); // For all this examples if path is not provided default will be root
   */
  public setCookie(params: Partial<Cookie>) {
    let d: Date = new Date();
    d.setTime(d.getTime() + (params.expireDays ? params.expireDays : 1) * 24 * 60 * 60 * 1000);
    let value: string = this.FixCookieValue(params.value);

    document.cookie =
      (params?.name ?? '') + '=' + (value) + ';' +
      (params.session && params.session == true ? '' : 'expires=' + d.toUTCString() + ';') +
      'path=' + (params.path && params.path.length > 0 ? params.path : '/') + ';' +
      (location.protocol === 'https:' && params.secure && params.secure == true ? 'secure' : '');
  }

  private FixCookieValue(value: string): string {
    return value ? value.replace(/;/g, "£%£") : "";
  }

  private GetValue(cookie_value: string): string {
    return cookie_value.replace(/£%£/gi, ";");
  }
}
