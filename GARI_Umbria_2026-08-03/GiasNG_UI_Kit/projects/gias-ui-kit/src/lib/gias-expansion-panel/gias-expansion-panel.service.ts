import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({ providedIn: 'root' })
export class GiasExpansionPanelService {

  constructor(private cookieService: CookieService) { }

  async getConfigCookies(id: string): Promise<IPanelCookie> {
    return new Promise((resolve, rejects) => {
      const json: string = this.cookieService.get(id);

      let data: IPanelCookie = null;
      try {
        data = JSON.parse(json);
        resolve(data);
      } catch (ex) {
        data = { isExpanded: false };
        resolve(data);
      }
    });
  }

  async storeConfigCookie(id: string, data: IPanelCookie) {
    // It is good practice to specify a path.
    this.cookieService.set(
      id, JSON.stringify(data), { path: '/' }
    );
  }
}

export class ExpansionPanelCookie implements IPanelCookie {
  isExpanded: boolean;
}

export interface IPanelCookie {
  isExpanded: boolean;
}
