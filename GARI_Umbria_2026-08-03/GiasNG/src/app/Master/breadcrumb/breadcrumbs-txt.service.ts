import { Injectable } from '@angular/core';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { DASHBOARD_URL, FAVORITES_URL } from 'app/app-routing.module';
import { BreadcrumbsInfo, MenuClient } from 'app/Service/api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BreadcrumbsTxtService {
  private history: number[] = [];
  private pageIndex = 0;

  private data = new BehaviorSubject<BreadcrumbsInfo | null>(null);
  private isBack = false;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private translocoService: TranslocoService,
    private menuClient: MenuClient
  ) {
    this.router.events.subscribe((val) => {
      if (val instanceof NavigationStart && val.navigationTrigger === 'popstate') {
        this.isBack = true;
      }

      if (val instanceof NavigationEnd) {
        this.loadData(this.objParametriAgendaService.getObjParamValue());
      }
    });

    this.loadData(this.objParametriAgendaService.getObjParamValue());
  }

  public breadcrumbsInfo$(): Observable<BreadcrumbsInfo | null> {
    return this.data.asObservable();
  }

  private loadData(params: ObjParametriAgenda): void {
    const url = this.router.url;

    // Don't show breadcrumbs if in dashboard or favorites pages
    if (url == `/${DASHBOARD_URL}` || url == `/${FAVORITES_URL}`) {
      this.pageIndex += 1;
      this.history[this.pageIndex] = 0;
      this.isBack = false;
      this.data.next(null);
      return;
    }

    let sectionId = 0;
    if (this.isBack && this.pageIndex > 0) {
      this.pageIndex -= 1;
      sectionId = this.history[this.pageIndex] ?? params?.IdSezione;
      this.isBack = false;
    } else {
      this.pageIndex += 1;
      sectionId = params?.IdSezione;
      this.history[this.pageIndex] = sectionId;
    }

    // Remove breadcrumbs if IdSezione is set to 0
    if (sectionId == null || sectionId <= 0) {
      this.data.next(null);
      return;
    }

    this.menuClient
      .menuOttieniBreadcrumbs(sectionId)
      .subscribe(x => {
        const res = x.RispostaStringa;
        const txt = this.getBreadcrumbText(sectionId, url);
        res.ClasseCssIcona = res.ClasseCssIcona.replace(/\./g, " ");
        res.TestoFiglio = txt != '' ? txt : res.TestoFiglio;
        res.Colore = res.Colore != "" ? res.Colore : "#000000"; 

        this.data.next(res);
      });
  }

  private getBreadcrumbText(sectionId: number, url: string): string {
    const objParams = this.objParametriAgendaService.getObjParamValue();

    switch (sectionId) {
      case Enum_PaginaPadre.Anagrafica:
        return this.getAnagraficaText(url, objParams.TipoOperazioneDB);
      default:
        return '';
    }
  }

  private getAnagraficaText(url: string, tipoOp: Enum_DBTypeOperation): string {
    let prefix: string;
    let suffix: string;

    switch (tipoOp) {
      case Enum_DBTypeOperation.Read:
        prefix = this.translocoService.translate('Lettura');
        break;
      case Enum_DBTypeOperation.Update:
        prefix = this.translocoService.translate('Edit');
        break;
    }

    switch (url) {
      case Enum_AnagarficaUrl.Azienda:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuova');
        suffix = this.translocoService.translate('Impresa');
        break;
      case Enum_AnagarficaUrl.Centri:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuovo');
        suffix = this.translocoService.translate('Centro');
        break;
      case Enum_AnagarficaUrl.Catasto:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuova');
        suffix = this.translocoService.translate('Particella');
        break;
      case Enum_AnagarficaUrl.Campi:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuovo');
        suffix = this.translocoService.translate('Campo');
        break;
      case Enum_AnagarficaUrl.Impianti:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuovo');
        suffix = this.translocoService.translate('Impianto');
        break;
      case Enum_AnagarficaUrl.Macchine:
        if (tipoOp == Enum_DBTypeOperation.Write) prefix = this.translocoService.translate('Nuova');
        suffix = this.translocoService.translate('MacchinaAttrezzatura');
        break;
      default:
        return '';
    }
    return prefix + ' ' + suffix;
  }
}

export enum Enum_PaginaPadre {
  Anagrafica = 250,
  GIS = 296
}

export enum Enum_AnagarficaUrl {
  Azienda = '/Anagrafica/Imprese/Impresa-Edit',
  Centri = '/Anagrafica/Centri/Centri-Edit',
  Fabbricati = '',
  Catasto = '/Anagrafica/Catasto/Catasto-Edit',
  Campi = '/Anagrafica/Campi/Campi-Edit',
  Impianti = '/Anagrafica/Appezzamenti/Appezzamento-Edit',
  Contatti = '',
  Macchine = '/Anagrafica/Macchine/Macchine-Edit'
}
