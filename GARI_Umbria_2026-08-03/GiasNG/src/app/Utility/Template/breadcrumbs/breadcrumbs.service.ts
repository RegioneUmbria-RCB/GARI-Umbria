/* eslint @typescript-eslint/member-ordering:0 */
/* eslint no-shadow:0 */
import { ChangeDetectorRef, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { BreadCrumbItem } from '@progress/kendo-angular-navigation';
import { AnagraficaRoutes } from 'app/anagrafica/anagrafica-routes';
import { CookieService } from 'app/Service/cookie.service';
import { MasterService, RispostaStandard } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { isNullOrUndefined } from 'app/Service/utils';
import { map, Observable, skip, Subject, take, takeUntil } from 'rxjs';
import { NextSelection } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { IQueryParamsService } from 'gias-kendo-grid';
import { addFilter, BreadCrumb, BreadCrumbCookieKey, BreadcrumbLevels, FilterBreadCrumb, removeNullAndNaNValues } from './breadcrumbs.models';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import {CompositeFilterDescriptor} from "@progress/kendo-data-query/dist/npm/filtering/filter-descriptor.interface";
import {cloneDeep} from "lodash";

const breadcrumbsLinks = {
  //caricaBreadcrumbs_Old:
  //    '/Anagrafica/AnagraficaSharedFns.asmx/DatiRelativiPercorsoBreadcrumbs'
  caricaBreadcrumbs:
    'AnagraficaNG/DatiRelativiPercorsoBreadcrumbs'
};


@Injectable()
/**
 * Utilizzo un servizio che viene iniettato dentro il grid public service.
 * Ho scelto questo metodo per agevolare l'aggiunta di nuove sezioni
 * dell'Anagrafica. Se avessi fatto l'aggancio fra il grid public
 * service e BreadcrumbsService al momento in cui questo veniva creato
 * (il public service) allora sarebbe stato bisogno aggiornare una
 * corrispondente parte ogni volta venisse aggiunta una nuova sezione.
 */
export class BreadcrumbsService implements IQueryParamsService {


  public items: BreadCrumb[] = [];
  public impresaPlaceholder: BreadCrumb = new BreadCrumb();
  public publicService: GridPublicService;
  public changeDetector: ChangeDetectorRef;

  private signal: Subject<void>;
  private currentPage: string;
  private currentSelectedOrDeselectedRow: any;

  constructor(
    public router: Router,
    private agendaService: ObjParametriAgendaService,
    private cookieService: CookieService,
    private transloco: TranslocoService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private masterService: MasterService) {
    //console.log('[breadcrumb.service] constructor');
    this.impresaPlaceholder.item = { text: this.transloco.translate('FiltroAzienda') }
  }

  componentInit(changeDetector: ChangeDetectorRef) {
    this.changeDetector = changeDetector;
    this.onImpresaSelectionUpdateView();
  }

  init(signal: Subject<void>) {
    this.signal = signal;
  }

  /**
   * This method is called automatically by the currently active grid.
   * It models the selection of nodes once a grid has been loaded.
   */
  execute(pubService: GridPublicService) {
    this.publicService = pubService;
    this.currentPage = this.trimLink(this.router.url);

    this.atGridInitialization(pubService);
  }

  /**
   * Number of sections: 6
   *
   * Entry point:
   * Questo è il punto di riferimento principale della pagina.
   */
  private atGridInitialization(pubService: GridPublicService) {
    this.restoreLastBreadcrumbs(pubService);
    this.createBreadcrumbsOnSelection(pubService);

    // Viene inoltrato un evento appena caricata la griglia.
    if(this.currentPageIsAmongGridSelectionCandidates()) {
      this.setGridSelection(this.correspondingBreadcrumbNode());
      this.applyGridFilters();
    } else if(this.existBreadCrumbs()){ // else among pages that require filtering
      this.applyGridFilters();
    }
  }

  restoreLastBreadcrumbs(pubService: GridPublicService) {
    let PivaSelected = this.agendaService.getObjParamValue().Piva;
    let strFilter = this.cookieService.getCookie(BreadCrumbCookieKey);
    if (strFilter != undefined && strFilter != '') {
      let lastFilter = (<FilterBreadCrumb>JSON.parse(strFilter));
      if (lastFilter.Piva == PivaSelected) {
        this.items = lastFilter.items;
        this.setobjParamtriAgendaFromBreadcrumbs();
        this.changeDetector?.detectChanges();
      }
    }
  }

  setobjParamtriAgendaFromBreadcrumbs(){
    let objP = cloneDeep(this.agendaService.getObjParamValue());
    let sa_cod = 0;
    let campo_cod = 0;

    if (this.items.length > 0){
      sa_cod = parseInt(this.items[0].gridRowId.split("_")[1]);
    }
    if (this.items.length > 1){
      campo_cod = parseInt(this.items[1].gridRowId.split("_")[2]);
    }

    objP.Sa_Cod = sa_cod;
    objP.Campo_Cod = campo_cod;

    this.agendaService.changeObjParametriAgenda(objP);
  }

  saveBreadcrumbsCookie(items: BreadCrumb[]) {
    let piva = this.agendaService.getObjParamValue().Piva;
    this.setobjParamtriAgendaFromBreadcrumbs();
    let objCookie: FilterBreadCrumb = {
      items: items,
      Piva: piva
    }
    this.cookieService.setCookie({
      name: BreadCrumbCookieKey,
      value: JSON.stringify(objCookie),
      session: true,
    })
  }

  private currentPageIsAmongGridSelectionCandidates() {
    return this.currentPageIs(AnagraficaRoutes.Centri) ||
      this.currentPageIs(AnagraficaRoutes.Campi);
  }

  private existBreadCrumbs() {
    return this.items.length > 0;
  }

  /* Section 1: On selection of a grid element
   * As a result of a grid selection (of centri or campi), we fetch
   * breadcrumbs that will be displayed and which will act as filters.
   */
  private createBreadcrumbsOnSelection(pubService: GridPublicService) {
    pubService.selection.getSelectedValue.pipe(skip(1), takeUntil(this.signal))
      .subscribe((selection: NextSelection) => {
        if(!this.currentPageIs(AnagraficaRoutes.Imprese)) {
          this.initSelection(selection);
          if (this.hasSelectedRows(selection)) {
            this.pushBreadcrumbs();
          } else if(this.hasDeselectedRows(selection)) {
            this.onDeselectionClearBreadcrumbs();
          }
        } else {
          this.clearBreadcrumbsIncludingLevel(0);
        }
      });
  }

  private initSelection(selection: NextSelection) {
    const dataItem = selection.value.selectedRows?.[0]?.dataItem ??
      selection.value.deselectedRows?.[0]?.dataItem;

    this.currentSelectedOrDeselectedRow = dataItem;
  }

  private hasSelectedRows(selection: NextSelection) {
    if (this.currentPageIs(AnagraficaRoutes.Centri)) {
      let validSelection = selection.value.selectedRows?.filter((c) => c.dataItem.sa_cod != null);
      return validSelection.length > 0;
    } else if (this.currentPageIs(AnagraficaRoutes.Campi)) {
      let validSelection = selection.value.selectedRows?.filter((c) => c.dataItem.sa_cod != null && c.dataItem.Campo_Cod != null);
      return validSelection.length > 0;
    } else {
      return selection.value.selectedRows.length > 0;
    }
  }

  private hasDeselectedRows(selection: NextSelection) {
    return selection.value.deselectedRows.length > 0;
  }

  private pushBreadcrumbs(): void {
    this.fetchBreadcrumbs()?.subscribe((breadcrumbs: BreadCrumb[]) => {
      breadcrumbs.forEach((crumb, index) => {
        crumb.breadcrumbLevel = index;
      });
      this.pushNodes(breadcrumbs);
    });
  }

  private fetchBreadcrumbs(): Observable<BreadCrumb[]> {
    switch (true) {
      case this.currentPageIs(AnagraficaRoutes.Centri):
        return this.getBreadCrumbs(AnagraficaRoutes.Centri);

      case this.currentPageIs(AnagraficaRoutes.Campi):
        return this.getBreadCrumbs(AnagraficaRoutes.Campi);

      default:
        return null;
    }
  }

  private pushNodes(nodes: BreadCrumb[]) {
    if (!isNullOrUndefined(nodes)) {
      this.items = [...nodes];
    }
    this.saveBreadcrumbsCookie(this.items);
    this.changeDetector.detectChanges();
  }

  /*private getBreadCrumbs_Old(tipoPagina: AnagraficaRoutes) {

      // eslint arrow-body-style: ["error", "always"]
      const params = ((...args: HttpArgs): any => {
          const master = args[1];
          const agenda = args[0].getObjParamValue();

          this.update(agenda);
          return {
              agenda: agenda,
              tipoPagina: tipoPagina.lvl,
              objParamServer: master.ObjParametri_Server,
              objParamUtenti: master.ObjParametri_Utenti
          };
      });

      return this.http.post2(breadcrumbsLinks.caricaBreadcrumbs, params, true)
          .pipe(map((risposta: RispostaStandard) => {
              return JSON.parse(risposta.RispostaStringa);
          }));
  }*/

  private getBreadCrumbs(tipoPagina: AnagraficaRoutes) {

    /* eslint arrow-body-style: ["error", "always"] */
    const params = ((...args): any => {
      const master = args[1];
      const agenda = args[0].getObjParamValue();

      this.update(agenda);
      return {
        agenda: agenda,
        tipoPagina: tipoPagina.lvl
      };
    });

    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(breadcrumbsLinks.caricaBreadcrumbs, params(this.agendaService, this.masterService))
      .pipe(map(risposta => {
        return risposta.RispostaStringa;
      }));
  }

  private update(agenda: ObjParametriAgenda) {
    switch (true) {
      case this.currentPageIs(AnagraficaRoutes.Centri):
        agenda.Piva = this.currentSelectedOrDeselectedRow.Piva;
        agenda.Sa_Cod = this.currentSelectedOrDeselectedRow.sa_cod;
        break;
      case this.currentPageIs(AnagraficaRoutes.Campi):
        agenda.Piva = this.currentSelectedOrDeselectedRow.PIVA;
        agenda.Sa_Cod = this.currentSelectedOrDeselectedRow.sa_cod;
        agenda.Campo_Cod = this.currentSelectedOrDeselectedRow.Campo_Cod;
        break;
      default:
        console.error('La pagina non è gestita: ', this.currentPage);
    }
  }

  /* Section 2: On deselection of a grid element
   * As a result of a grid deselection (of centri or campi), we remove
   * the corresponding breadcrumbs from the chain.
   */
  private onDeselectionClearBreadcrumbs() {
    switch (true) {
      case this.currentPageIs(AnagraficaRoutes.Centri):
        this.clearBreadcrumbsIncludingLevel(BreadcrumbLevels.centro);
        break;
      case this.currentPageIs(AnagraficaRoutes.Campi):
        this.clearBreadcrumbsIncludingLevel(BreadcrumbLevels.campo);
        break;
      default:
        break;
    }
  }

  private clearBreadcrumbsIncludingLevel(level: BreadcrumbLevels) {
    this.items = this.items.slice(0, level);
    this.saveBreadcrumbsCookie(this.items);
    this.changeDetector.detectChanges();
  }


  /* Section 3: Sets the initially selected centro/campo
   * Selects an already filtered node during the initialization of the grid.
   * This method is called automatically by the currently active grid.
   */
  private setGridSelection(ids: number[] | string[]) {
    if(isNullOrUndefined(ids) || ids.length === 0) {
      return;
    }

    this.publicService.selection.setSelected.next({ keys: ids, usePartialMatch: true, resetPreviousSelection: true });
  }

  private correspondingBreadcrumbNode() {
    if(this.currentPageIs(AnagraficaRoutes.Centri) &&
      this.existsCrumbAtLevel(BreadcrumbLevels.centro)) {
      /* because centri are located at location 0 in the hierarchy */
      return [ this.items[BreadcrumbLevels.centro].gridRowId ];
    }
    if(this.currentPageIs(AnagraficaRoutes.Campi) &&
      this.existsCrumbAtLevel(BreadcrumbLevels.campo)) {
      /* because campi are located at location 1 in the hierarchy */
      return [ this.items[BreadcrumbLevels.campo].gridRowId ];
    }
  }

  private existsCrumbAtLevel(level: number) {
    return !isNullOrUndefined(this.items[level]);
  }

  /** Section 4: Discard grid filters when clicking breadcrumbs
   *  Called from the view when a breadcrumb has been clicked.
   */
  public discardGridFilters(selected: BreadCrumbItem) {
    this.discardBreadcrumbsAfter(selected);
    this.applyGridFilters();
  }

  private discardBreadcrumbsAfter(selected: BreadCrumbItem) {
    const index = this.items
      .map((wrapper) => {
        return wrapper.item;
      })
      .findIndex((crumb) => {
        return crumb.text === selected.text;
      });

    this.items = this.items.slice(0, index + 1);
    this.saveBreadcrumbsCookie(this.items);
    this.publicService.refresh(true);
    this.changeDetector.detectChanges();
  }

  /* Section 5: Applying filters
   * Apply grid filters for any page which is not Centri or Campi.
   */
  private applyGridFilters() {
    // this.resetGridFilters();
    // if(this.currentPageIs(AnagraficaRoutes.Catasto)) {
    //   this.discardUnusedFiltersAbove(BreadcrumbLevels.centro);
    //   this.applyFiltersCatasto();
    // } if(this.currentPageIs(AnagraficaRoutes.Fabbricati)) {
    //   this.discardUnusedFiltersAbove(BreadcrumbLevels.centro);
    //   this.applyFiltersFabbricati();
    // } if(this.currentPageIs(AnagraficaRoutes.Impianti)) {
    //   this.applyFiltersImpianti();
    // } if(this.currentPageIs(AnagraficaRoutes.Campi)) {
    //   this.applyFiltersCampi();
    // }

    //this.publicService.refresh(false);
  }

  private discardUnusedFiltersAbove(currentLevel: BreadcrumbLevels) {
    this.items = [...this.items.filter((item) => {
      return item.breadcrumbLevel <= currentLevel;
    })];
    this.saveBreadcrumbsCookie(this.items);
    this.changeDetector.detectChanges();
  }

  private get gridFilters() {
    return this.publicService.filters.value;
    //this.publicService.gridComp.filter;
  }

  private get nomeCentro() {
    return this.items[BreadcrumbLevels.centro]?.item.text;
  }

  private get nomeCampo() {
    return this.items[BreadcrumbLevels.campo]?.item.text;
  }

  private get idCentro() {
    return +this.items[BreadcrumbLevels.centro]?.gridRowId.split('_')[1];
  }

  private get idCampo() {
    return +this.items[BreadcrumbLevels.campo]?.gridRowId.split('_')[2];
  }

  private applyFiltersCatasto() {
    let filter:CompositeFilterDescriptor = {
      filters: removeNullAndNaNValues([
        addFilter('Sa_Cod', this.idCentro)
      ]),
      logic: 'and'
    }
    this.applyFilters(filter)
    //this.gridFilters.filters.push(filter);
  }

  private applyFiltersFabbricati() {
    let filter:CompositeFilterDescriptor = {
      filters: removeNullAndNaNValues([
        addFilter('Sa_Nome', this.nomeCentro)
      ]),
      logic: 'and'
    }
    this.applyFilters(filter)
    //this.gridFilters.filters.push(filter);
  }

  private applyFiltersImpianti() {
    let filter:CompositeFilterDescriptor = {
      filters: removeNullAndNaNValues([
        addFilter('SA_COD', this.idCentro),
        addFilter('Campo_Cod', this.idCampo)
      ]),
      logic: 'and'
    }
    this.applyFilters(filter)
    //this.gridFilters.filters.push(filter);
  }

  private applyFiltersCampi() {
    let filter:CompositeFilterDescriptor = {
      filters: removeNullAndNaNValues([
        addFilter('sa_cod', this.idCentro)
      ]),
      logic: 'and'
    }
    this.applyFilters(filter)
    //this.gridFilters.filters.push(filter);
  }

  private resetGridFilters() {
    let com: CompositeFilterDescriptor;
    com = {logic:'or', filters: []};
    this.publicService.filters.next(com)
    //this.gridFilters.filters = [];
  }

  private applyFilters(compositeFilterDescriptor: CompositeFilterDescriptor){
    let val = cloneDeep(this.publicService.filters.value);
    val.filters.push(compositeFilterDescriptor)
    this.publicService.filters.next(val)
  }

  /** Section 6: On Impresa Selection
   *  Will show the node related to the business (impresa) once
   *  it has been selected in the chain.
   */
  public getSelectedImpresa(): BreadCrumb {
    if(this.agendaService.impresaIsSelected()) {
      return this.impresaPlaceholder;
    }
    return null;
  }

  private onImpresaSelectionUpdateView() {
    if(!this.agendaService.impresaIsSelected()) {
      this.agendaService.currentObjParametriAgenda.pipe(skip(1), take(1)).subscribe(() => {
        this.changeDetector.detectChanges();
      });
    }
  }

  /* Utility functions */
  private currentPageIs(expected: AnagraficaRoutes) {
    return this.currentPage === expected.link;
  }

  private trimLink(link: string) {
    if (link.includes('?')) {
      return link.split('?')[0];
    } else {
      return link;
    }
  }

}

