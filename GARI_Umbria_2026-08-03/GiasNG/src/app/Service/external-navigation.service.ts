import { Injectable } from '@angular/core';
import { CookieService } from './cookie.service';
import { Observable, filter, map, of } from 'rxjs';
import { GiasDialogService } from './gias-dialog.service';
import { NavigationEnd, Router } from '@angular/router';
import {SpecialNavigation} from '../Model/TipiEnumerativi';

const AGRONICA_SPECIAL_NAVIGATION_COOKIE = 'AgronicaSpecialNavigation';
const AGRONICA_SPECIAL_NAVIGATION_FEEDBACK = 'AgronicaFeedback';

const GESTIONE_RICHIESTE_PATH = 'GestioneRichieste';

@Injectable({ providedIn: 'root' })
export class ExternalNavigationService {
  private readonly _externalLoad: boolean = false;
  private readonly _specialNavigationCookie: string | null;
  private history: string[] = [];

  constructor(
    private cookieService: CookieService,
    private giasDialogMessage: GiasDialogService,
    private router: Router
  ) {
    this._specialNavigationCookie = this.cookieService.getCookie(AGRONICA_SPECIAL_NAVIGATION_COOKIE);

    if (Object.values(SpecialNavigation).some(n => n === this._specialNavigationCookie)) {
      this._externalLoad = this.specialNavigationCookie != SpecialNavigation.None;
    } else {
      this._specialNavigationCookie = SpecialNavigation.None;
    }

    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        map((event: NavigationEnd) => event.url),
      )
      .subscribe(url => {
        this.history.push(url);
      });
  }

  get externalLoad(): boolean {
    return this._externalLoad;
  }
  get specialNavigationCookie(): string {
    return this._specialNavigationCookie;
  }

  get isBackInvalid(): boolean {
    return this._externalLoad != null && this.history.length > 1 && this.history[this.history.length - 2].includes(GESTIONE_RICHIESTE_PATH);
  }

  public closeNavigation(): Observable<void> {
    if (this._specialNavigationCookie == SpecialNavigation.OnNewWindow) {
      window.close();
    } else if (this._specialNavigationCookie == SpecialNavigation.OnIframe) {
      window.parent[AGRONICA_SPECIAL_NAVIGATION_FEEDBACK].call();
    } else {
      this.giasDialogMessage.baseError('', 'CookieDiNavigazioneEsternaNonValido', true);
    }

    return of(null);
  }

  public goBack(): void {
    this.history.pop(); // Current page
    this.history.pop(); // Previous page (will be readded on NavigationEnd event)
  }
}
