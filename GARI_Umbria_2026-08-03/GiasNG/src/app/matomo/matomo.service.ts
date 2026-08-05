import { Injectable } from '@angular/core';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { WidgetData } from 'app/widget/widgets.component';
import { BehaviorSubject } from 'rxjs';

declare global {
  interface Window { _mtm: any[]; _paq: any[]; }
}

@Injectable({ providedIn: 'root' })
export class MatomoService {
  private _isLoaded = false;

  constructor(
              private objParamAgendaService: ObjParametriAgendaService 
  ) {}

  get isLoaded(): boolean {
    return this._isLoaded;
  }

  set isLoaded(value: boolean) {
    this._isLoaded = value;
  }

  loadMatomo(url: string, siteId: string, userNameLogged: string): void {
    if (this.isLoaded || !url || !siteId) return;
    this.isLoaded = true;

    // push dello username
    window._paq = window._paq || [];

    if (userNameLogged) {
      window._paq.push(['setUserId', userNameLogged]);
    }

    //comunico anche l'azienda 
    const objParam = this.objParamAgendaService.getObjParamValue();
    this.pushCompanySelected(objParam?.Piva, objParam?.RagSoc);

    // Inizializza _mtm (Tag Manager)
    window._mtm = window._mtm || [];
    window._mtm.push({
      'mtm.startTime': new Date().getTime(),
      event: 'mtm.Start'
    });

    const g = document.createElement('script');
    g.async = true;
    g.src = `https://${url}/js/container_${siteId}.js`; // deve essere del tipo https://.../container_XXXX.js
    const s = document.getElementsByTagName('script')[0];
    s.parentNode?.insertBefore(g, s);

  }

  pushCompanySelected(piva: string, ragioneSociale: string) {
    if (!this.isLoaded || !window._paq) return;

    window._paq.push(['trackEvent', 'general', 'company_select', piva + ' - ' + ragioneSociale]);

  }

  pushEnabledWidgets(widgets: WidgetData[]) {
    if (!this.isLoaded || !window._paq) return;

    window._paq = window._paq || [];
    widgets.forEach(widget => {
                                window._paq.push(['trackEvent', 'dashboard', 'widget_used', widget.Titolo]);
    });
  }

  trackPageView(url: string, title: string): void {
    if (!this.isLoaded || !window._paq) return;
    
    window._paq.push(['setCustomUrl', url]);
    window._paq.push(['setDocumentTitle', title]);
    window._paq.push(['trackPageView']);

  }

}