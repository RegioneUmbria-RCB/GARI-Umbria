import {Component, EventEmitter, OnDestroy, OnInit, Output, AfterViewChecked, HostListener, Predicate} from '@angular/core';
import { MasterService } from '../../Service/master.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { MessageService } from '@progress/kendo-angular-l10n';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { CustomMessagesService } from 'app/Service/kendo-messages.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import {debounceTime, filter, Observable, of, Subject, Subscription, switchMap, take, takeUntil, tap } from "rxjs";
import {Router, ActivatedRoute, NavigationEnd, NavigationStart} from '@angular/router';
import { MenuClient, Utente, Impresa, LogoutClient } from 'app/Service/api.service';
import {enum_PagineGiasNG, getMasterWithEditPages, SpecialNavigation} from 'app/Model/TipiEnumerativi';
import { SACOD_NOFILTRO } from "../../Model/CostantiPersonalizzate";
import { ImpostazioniAziendeCentriService } from "../../profilazione/services/impostazioni/impostazioni-aziende-centri.service";
import { MenuContestualeService } from '../menu-contestuale/menu-contestuale.service';
import { CookieService } from 'ngx-cookie-service';
import { DASHBOARD_URL, FAVORITES_URL } from 'app/app-routing.module';
import { ExternalNavigationService } from 'app/Service/external-navigation.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { Messaggio_Utente_Permessi } from 'app/Model/utente/messaggio_utente_permessi';
import { ConfigurazioneSitiService } from 'app/Service/configurazione-siti.service';
import { AjaxAgronicaAPIService } from '../../Service/ajax-agronica.api.service';
import { MatomoService } from 'app/matomo/matomo.service';
import { SementieriParametrizzazione } from 'app/Model/GIS/SementieriParametrizzazione';


export class HeaderModel {
  visible: boolean;
  logo: string;
  username: string;
  userLabel: string;
  timeVisibility_From: Date;
  timeVisibility_To: Date;
  last_access: Date;
  company: string;
  HomeButton: boolean;
  BackButton: boolean;
  ColumnLeft:boolean;
  ColumnRight:boolean;
}

export class UserModel {
  id: number;
  nome: string;
  cognome: string;
  email: string;
  visibilita: string;
  last_login: Date;
  last_notifica: string;
  id_company: number;
}

export class CompanyModel {
  id: number;
  ragioneSociale: string;
  desc: string;
  piva: string;
}

export const Not_ReloadPages: number[] = [enum_PagineGiasNG.Pagina_GIS];

@Component({
  standalone: false,
  selector: 'gias-master-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']  // 'legend-style.component.scss', './font-awesome.css'
})
/** header component*/
export class HeaderComponent implements OnInit, OnDestroy, AfterViewChecked {

  private subs: Subscription[] = [];
  private widthFromLeftContent: string = '350px';
  private widthFromLeftContentMobile: string = '100%';
  private widthLeftSidebar: string = (window.innerWidth < 991) ? this.widthFromLeftContentMobile : this.widthFromLeftContent;

  user: Utente | null = null;
  lastSelectedCompanies: Impresa[] = [];
  searchBoxAzienda: string;
  searchedCompanies: Impresa[] = [];
  company: CompanyModel = new CompanyModel();
  language: string;
  headerModel: HeaderModel | null = null;
  isLogoutConfirmationOpen: boolean = false;
  isDashboard: boolean = false;
  isInEditPage: boolean;
  validitaPermessiLicenza: Messaggio_Utente_Permessi;
  logoutUrl: string = '';

  private initialLoaded: boolean = false;
  private ricercaAzienda$ = new Subject<string>();

  public signal$: Subject<void> = new Subject();

  private subscriptionList: Subscription[];
  @Output() loaded = new EventEmitter<void>();

  constructor(
    public kendoMessages: MessageService,
    private masterService: MasterService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private menuClient: MenuClient,
    private impostazioniaziendecentriservice: ImpostazioniAziendeCentriService,
    private menuContestualeService: MenuContestualeService,
    private cookieService: CookieService,
    private logoutClient: LogoutClient,
    private externalNavigationService: ExternalNavigationService,
    private permessiUtentiService: PermessiUtenteService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private matomoService: MatomoService
  ) {
    this.translocoService.langChanges$.subscribe(x => this.language = x.toUpperCase());
    this.handleQueryParams();

    this.subs.push(this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.isDashboard = event.url == `/${DASHBOARD_URL}` || event.url == `/${FAVORITES_URL}`;
      }

      if (event instanceof NavigationStart || event instanceof NavigationEnd) {
        this.changeLogo();
      }
    }));

    this.subs.push(
      this.ricercaAzienda$
        .pipe(debounceTime(500))
        .subscribe(x => this.doRicercaAzienda(x))
    );
  }

  get isExternalLoad(): boolean {
    return this.externalNavigationService.externalLoad;
  }

  @HostListener('document:click', ['$event'])
  documentClick(event: MouseEvent) {
    const element = event.target as HTMLElement;
    // Both false or both true, close the nav
    if ((element.closest('#GiasSidenav') != null) === (element.closest('#openMenu') != null)) {
      this.closeNav();
    }

    const companyDrop = element.closest('#navbar_company');
    if (document.getElementById('hCompaniesDrop')) {
      if (companyDrop == null && document.getElementById('hCompaniesDrop').classList.contains('show')) {
        this.showToggleDropdown('hCompaniesDrop');
      }
    }

    const userDrop = element.closest('#navbar_user');
    if (document.getElementById('userAccount')) {
      if (userDrop == null && document.getElementById('userAccount').classList.contains('show')) {
        this.showToggleDropdown('userAccount');
      }
    }
  }

  ngOnDestroy() {
    this.subs.forEach(sub => sub.unsubscribe());
    this.signal$.next();
    this.signal$.complete();
  }

  ngOnInit() {
    this.masterService
      .initialLoadCompleteSource
      .pipe(filter((e) => {
        return e == true;
      }), take(1))
      .subscribe(() => this.init(this.masterService.getHeader()));

    this.objParametriAgendaService.currentObjParametriAgenda.pipe(
      takeUntil(this.signal$),
      switchMap(el => {
        if (el.Piva !== '') {
          return this.impostazioniaziendecentriservice.getImprese_Impostazioni(el.Piva, SACOD_NOFILTRO);
        }
        return of([]);
      })).subscribe();

    this.masterService.getCurrentPageAsObs().subscribe(cp => {
      this.checkIsInEditPage(cp);
    });
  }

  ngAfterViewChecked(): void {
    if (this.initialLoaded) {
      return;
    }
    if (document.querySelector('header') == undefined) {
      return;
    }
    if (document.querySelector('header').offsetHeight == 0) {
      return;
    }

    this.masterService.headerLoadedSource.next();

    this.initialLoaded = true;
  }

  private init(headerModel: HeaderModel): void {
    this.headerModel = headerModel;
    if (!headerModel.visible) {
      return;
    }

    this.menuClient
      .menuInformazioniUtente()
      .subscribe((data) => this.user = data.RispostaStringa);

    this.getLastSelectedCompanies();

    this.permessiUtentiService.currentUtente_Permessi.subscribe((data) => {
      if (data) {
        this.validitaPermessiLicenza = data.ValiditaUtentePermessiLicenza;
      }
    });

    this.subs.push(this.masterService.currentErrorMsgType.subscribe((el) => {
      if (el.show) {
        this.showErrorMsg(el.msg, el.errorNumber);
        //this.masterService.changeErrorMsgType({ show:false, msg:'' });
      }
    }));

    this.subs.push(this.objParametriAgendaService.currentObjParametriAgenda.subscribe((objSelected) => {
      this.getCompany(objSelected);
      if (objSelected.Piva !== this.company.piva) {
        this.cambiaAzienda({
          partitaIva: objSelected.Piva,
          ragioneSociale: objSelected.RagSoc
        }, false);
      }
    }));

  }

  /*
  * @description Decido quale logo applicare sull'header
  * */
  private changeLogo(): void {

    this.masterService.initialLoadCompleteSource.pipe(
      take(1),
      filter((val) => {
        return val;
      }),
      filter((val) => {
        return this.masterService.getHeader()?.logo === '';
      }),
      switchMap((val) => {
        return this.configurazioneSitiService.leggiChiave('PersonalizzazioniGraficheCliente');
      }),
      tap((data) => {
        let logo = 'assets/img_giasng/navbar/gias_icon_256x256.png';

        if (data && data.Valore !== '') {
          const personalizzazioni = JSON.parse(data.Valore);
          if (personalizzazioni.Logo_Navbar_PATH && personalizzazioni.Logo_Navbar_PATH !== '')
            logo = this.masterService._link_GiasBase + personalizzazioni.Logo_Navbar_PATH;
        }

        let header = this.masterService.getHeader();
        header.logo = logo;
        this.masterService.changeHeader(header);

        //console.log('Load logo header ' + logo);
      })
    ).subscribe();

  }

  private handleQueryParams(): void {
    this.activatedRoute.queryParams.pipe(takeUntil(this.signal$))
      .subscribe(params => {
        if (params.seFrame == 1) {
          this.headerModel.visible = false;
          this.masterService.changeHeader(this.headerModel);
          let footer = this.masterService.getFooter();
          footer.visible = false;
          this.masterService.changeFooter(footer);
          let menuContestuale = this.menuContestualeService.getMenuContestualeSettings();
          menuContestuale.show = false;
          this.menuContestualeService.changeMenuContestualeSettings(menuContestuale);
        }
      });
  }

  /**
   * This function will open the sidenav menu
   */
  openNav() {
    document.getElementById('GiasSidenav').style.width = this.widthLeftSidebar;
    document.getElementById('contenitore_principale').style.marginLeft = '0';
    // document.getElementById("contenitore_principale").style.marginLeft = this.widthLeftSidebar;
    document.getElementById('openMenu').classList.add('hide');
    document.getElementById('closeMenu').classList.remove('hide');
    document.getElementById('GiasSidenav').classList.add('openSidebarMenu');
  }

  /* Set the width of the side navigation to 0 and the left margin of the page content to 0 */
  /**
   * This function will close the sidenav menu
   */
  closeNav() {
    if (document.getElementById('GiasSidenav')) {
      document.getElementById('GiasSidenav').style.width = '0';
      document.getElementById('GiasSidenav').classList.remove('openSidebarMenu');
    }
    if (document.getElementById('contenitore_principale')) {
      document.getElementById('contenitore_principale').style.marginLeft = '0';
    }

    if (document.getElementById('openMenu')) {
      document.getElementById('openMenu').classList.remove('hide');
    }

    if (document.getElementById('closeMenu')) {
      document.getElementById('closeMenu').classList.add('hide');
    }
  }

  /**
   * function to toggle show/hide dropdown element on header section
   * @param id
   */
  showToggleDropdown(id) {
    if (this.companySelectionAllowed()) {
      var currentElement = $('#' + id).clone();
      $('.dropdown-content').each(function (i, el) {
        $(this).removeClass('show');
      });
      if (!currentElement.hasClass('show')) {
        $('#' + id).addClass('show');
      }
    }
  }

  filtrinoImprese() {
    let objP = this.objParametriAgendaService.getObjParamValue();
    objP.Pagina_Provenienza = this.masterService.getCurrentPageAsValue();

    this.gestioneRichiesteService.goToFiltrino(Enum_SiteRedirector.GiasNG, objP.Pagina_Provenienza).then((resp) => {
      window.location.href = resp;
    });

    // // this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    // //   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    // //   enum_PagineAgenda_2010.Pagina_FiltrinoImprese, [], objP).then((val) => {
    // //     window.location.href = val;
    // //   });
  }

  ricercaAziendaOld() {
    this.ricercaAzienda$.next(this.searchBoxAzienda);
  }

  private doRicercaAzienda(azienda: string): void {
    if (azienda.length > 2) {
      this.menuClient
        .menuRicercaAzienda(azienda)
        .pipe(debounceTime(500))
        .subscribe((data) => this.searchedCompanies = data.RispostaStringa);

      return;
    }

    this.searchedCompanies = [];
  }

  ricercaAzienda() {
    if (this.searchBoxAzienda.length > 2) {
      this.subscriptionList?.forEach(subscription => subscription.unsubscribe());
      this.subscriptionList = [];
      let sub = this.menuClient
        .menuRicercaAzienda(this.searchBoxAzienda).subscribe((data) => {
          this.searchedCompanies = data.RispostaStringa;
          this.subscriptionList = [];
        });
      this.subscriptionList.push(sub);
      return;
    }

    this.subscriptionList = [];
    this.searchedCompanies = [];
  }

  cambiaAzienda(azienda, reload: boolean): void {
    const piva = azienda.partitaIva?.trim();

    // task #203765
    const ragioneSociale = azienda.ragioneSociale?.trim();

    if (piva != null && piva != '' && ragioneSociale != null && ragioneSociale != '') {
      const obj = this.objParametriAgendaService.getObjParamValue();
      const queryStringFiltrino = obj.QueryStringFiltrino; 
      this.objParametriAgendaService.resettaObjAgenda(obj);

      // avoid resetting QueryStringFiltrino if Sementieri
      try {
        if (queryStringFiltrino != null && queryStringFiltrino != '' && SementieriParametrizzazione.isInstanceOfSementieri(JSON.parse(queryStringFiltrino))) {
          obj.QueryStringFiltrino = queryStringFiltrino;
        }
      } catch (_) {
        // do nothing
      }

      obj.Piva = piva;
      obj.RagSoc = ragioneSociale;
      this.company.piva = piva;
      this.company.ragioneSociale = ragioneSociale;
      this.objParametriAgendaService.changeObjParametriAgenda(obj);
      this.searchBoxAzienda = '';

      //devo comunicare a Matomo se è cambiata l'azienda
      this.matomoService.pushCompanySelected(piva, ragioneSociale);

      this.menuClient
        .menuAggiornaAttivitaNavigazioneAziende({piva: piva})
        .subscribe(() => {
          this.getLastSelectedCompanies();
          if (reload) {
            this.showToggleDropdown('hCompaniesDrop');
            //Caso particolare del GIS, il cambio azienda viene gestito in altro modo senza il ricaricamento della pagina
            //Creato array per altri casi di questo tipo
            if (Not_ReloadPages.indexOf(this.masterService.getCurrentPageAsValue()) < 0) {
              this.reloadPage();
            }
          }
        });
    }
  }

  showErrorMsg(msg: string, errorNumber: number): void {
    let content = msg;
    if (errorNumber > 0) {
      content = 'Errore ' + errorNumber + ': ' + msg;
    }
    this.giasMessageService.errorMessage(content);
  }

  changeLanguage(ln: string): void {
    this.translocoService.setActiveLang(ln);
    const svc = <CustomMessagesService>this.kendoMessages;
    svc.language = ln;
  }

  // navigateToFavorites(): void {
  //   const params = this.objParametriAgendaService.getObjParamValue();
  //   params.IdSezione = 0;
  //   this.objParametriAgendaService.changeObjParametriAgenda(params);

  //   this.router.navigateByUrl(FAVORITES_URL);
  // }

  logout(): void {
    this.isLogoutConfirmationOpen = true;
  }

  doLogout(): void {
    // if (this.externalLoad != null) {
    //     this.externalNavigationService.closeNavigation().subscribe(() => this.isLogoutConfirmationOpen = false);
    //     return;
    // }
    this.getLogoutUrl().pipe(
      take(1),
      tap(() => {
        this.logoutClient.logoutLogout().pipe(take(1), tap(() => {
          this.cookieService.delete('Authorization', '/', 'localhost');
          window.location.assign(this.logoutUrl);
        })).subscribe();
      })
    ).subscribe();

  }

  showCompany(): boolean {
    return !this.isExternalLoad || this.externalNavigationService.specialNavigationCookie !== SpecialNavigation.OnIframe;
  }

  showDashboardBtn(): boolean {
    return !this.isExternalLoad || this.externalNavigationService.specialNavigationCookie !== SpecialNavigation.OnIframe;
  }

  companySelectionAllowed(): boolean {
    return !this.isInEditPage && this.externalNavigationService.specialNavigationCookie === SpecialNavigation.None;
  }

  headerCompanyName(company: Impresa, companies: Impresa[]): string {
    let name: string = `${company.ragioneSociale}`;
    // task 203765: mostro sempre CUAA
    // if (companies.some(c => c.ragioneSociale === company.ragioneSociale && c.partitaIva !== company.partitaIva)) {
    //   name = name + ` ${company.CUAA}`;
    // }

    return name;
  }

  headerCompanyCuaa(company: Impresa, companies: Impresa[]): string {
    return company.CUAA;
  }

  private getLogoutUrl(): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<object, string>('Logout/LinkHomePageLogin', {}).pipe(take(1), switchMap(resp => {
      let logoutUrl = resp.RispostaStringa;
      if (logoutUrl.toLowerCase().includes('pivasuperuser')) {
        return logoutUrl;
      }
      logoutUrl += logoutUrl.includes('?') ? '&' : '?';
      logoutUrl += `pivasuperuser=${this.masterService.objP_server.PivaSuperUser}`;
      this.logoutUrl = logoutUrl;
      return of(logoutUrl);
    }));
  }

  private reloadPage(): void {
    let currentUrl: string = this.router.url;
    let oldStrategy = this.router.routeReuseStrategy.shouldReuseRoute;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigate([currentUrl]).then(() => {
      // window.location.reload() // if we need hard reload
      //this.router.routeReuseStrategy.shouldReuseRoute = () => true;
      this.router.routeReuseStrategy.shouldReuseRoute = oldStrategy;
    });
  }

  private getLastSelectedCompanies(): void {
    this.menuClient
      .menuOttieniUltimeAziendeSelezionate()
      .subscribe((data) => this.lastSelectedCompanies = data.RispostaStringa);
  }

  private getCompany(company: ObjParametriAgenda): void {
    this.company.ragioneSociale = company.RagSoc;
    this.company.desc = '';
    if (company.RagSoc == '' || company.RagSoc == undefined) {
      this.company.desc = company.Cod_Contatto;
    }
    this.company.piva = company.Piva;
  }

  checkIsInEditPage(cp: number): void {
    let array: Array<any> = getMasterWithEditPages();
    this.isInEditPage = false;
    let el = array.find((_enum) => {
      if (_enum.editPage.findIndex(x => x === cp) > -1) {
        this.isInEditPage = true;
      }
    });
  }

  ShowColLeft(): boolean {
    return !!this.headerModel && this.headerModel.visible && this.headerModel.ColumnLeft;
  }

  ShowColRight() {
    return !!this.headerModel && this.headerModel.visible && this.headerModel.ColumnRight;
  }
}
