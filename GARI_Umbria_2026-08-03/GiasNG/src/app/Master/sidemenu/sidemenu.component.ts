import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import {LinkMenu, MenuClient} from 'app/Service/api.service';
import {GestioneRichiesteService} from 'app/Service/gestione-richieste.service';
import {MasterService} from 'app/Service/master.service';
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {catchError, filter, Observable, of, Subject, Subscription, switchMap, take, takeUntil, tap} from 'rxjs';
import {HeaderModel} from '../header/header.component';
import {GiasDialogService} from 'app/Service/gias-dialog.service';
import {TranslocoService} from '@jsverse/transloco';
import {Enum_SiteRedirector} from 'app/Model/siti.enum';
import {Router} from '@angular/router';
import {enum_PagineGiasNG} from "../../Model/TipiEnumerativi";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import { Tipo_Attivita} from 'gias-ui-kit';
import { FAVORITES_URL } from 'app/app-routing.module';
import { ExternalNavigationService } from 'app/Service/external-navigation.service';

const SIDE_MENU_TABS_KEY = 'gias-sidemenu-selected-tab';

enum SideMenuTabs {
  All,
  Favorites
}

@Component({
  standalone: false,
  selector: 'gias-sidemenu',
  templateUrl: './sidemenu.component.html',
  styleUrls: ['./sidemenu.component.scss']
})
export class SideMenuComponent implements OnInit, OnDestroy {

  allMenu: LinkMenu[] = [];
  favoritesMenu: LinkMenu[] = [];
  filterText = "";
  selectedTab = this.initSelectedTab();
  SideMenuTabs = SideMenuTabs;
  headerModel: HeaderModel | null = null;
  calcBodyPaddingTop: string = '0px';
  loadComplete: boolean = false;
  obsPaddingTopFrame: Subscription;

  public signal$: Subject<void> = new Subject();

  private favoritesSub: Subscription;

  constructor(
    private masterService: MasterService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private menuClient: MenuClient,
    private permessiUtenteService: PermessiUtenteService,
    private router: Router,
    private changeDetectionRef: ChangeDetectorRef,
    private externalNavigationService: ExternalNavigationService
  ) {
    this.masterService.currentHeader.subscribe(el => {
      this.headerModel = el;
    });

    this.masterService
      .initialLoadCompleteSource.pipe(filter((e) => { return e == true }), take(1)).subscribe((val) => {
        this.favoritesSub = this.permessiUtenteService
          .sidemenuBehaviorSubject
          .pipe(
            switchMap(() => this.menuClient.menuGetAlberoMenu()),
            tap(val => {
              this.allMenu = val.RispostaStringa.Menus ?? [];
              this.favoritesMenu = val.RispostaStringa.MenusPreferiti ?? [];

              this.masterService.InfoAlberoMenu = val.RispostaStringa;
            })
          )
          .subscribe()

        this.masterService.headerLoadedSource.pipe(takeUntil(this.signal$)).subscribe(() => {
          this.setBodyPaddingTop();
        })
      })
  }

  //funzionante backup
  //   ngAfterViewInit(): void {
  //   //   console.log("afterviewinit")

  //     let targetTabId = "leftsideTabServizi";
  //     let currentTabId = "leftsideTabPreferiti";
  //     let targetTabcontentId = "leftsideTabcontentServizi";
  //     let currentTabcontentId = "leftsideTabcontentPreferiti";
  //     if (target === "preferiti") {
  //       currentTabId = "leftsideTabServizi";
  //       targetTabId = "leftsideTabPreferiti";
  //       currentTabcontentId = "leftsideTabcontentServizi";
  //       targetTabcontentId = "leftsideTabcontentPreferiti";
  //   }
  //     const targetTabcontent = document.getElementById(targetTabcontentId);
  //     targetTabcontent.classList.remove("hide");
  //     const currentTabcontent = document.getElementById(currentTabcontentId);
  //     currentTabcontent.classList.add("hide");
  //                 this.allMenu = val.RispostaStringa.Menus ?? [];
  //                 this.favoritesMenu = val.RispostaStringa.MenusPreferiti ?? [];
  //             });
  //         this.loadComplete = true;
  //   }

  ngOnInit(): void {
    this.masterService
      .initialLoadCompleteSource
      .pipe(filter((e) => { return e == true }), take(1))
      .subscribe(() => this.init());

      this.obsPaddingTopFrame = this.masterService
                                .currentHeader.subscribe(headerModel => {
                                      if (!headerModel.visible) {
                                        this.calcBodyPaddingTop = '0px';
                                      }
                                });
  }

  ngOnDestroy(): void {
    this.favoritesSub.unsubscribe();
    this.obsPaddingTopFrame.unsubscribe();
    this.signal$.next();
    this.signal$.complete();
  }

  get isExternalLoad(): boolean {
    return this.externalNavigationService.externalLoad;
  }

  setBodyPaddingTop(): void {
    this.calcBodyPaddingTop = document.querySelector('header').offsetHeight + 'px';
  }
  inserisciInQueryString(queryString: string, chiave: string, valore: string) : string {
    let prefix: string = "";
    if(queryString.includes("?")){
      prefix = "&";
    }else{
      prefix = "?";
    }

    return queryString + prefix + `${chiave}=${valore}`;
  }
  gestioneRedirect(link: string, service: LinkMenu): void{
    const APRI_IN_FINESTRA_CORRENTE :number = 0;
    const APRI_IN_NUOVA_FINESTRA :number= 1;
    const APRI_IN_POPUP :number= 2;
    const APRI_IN_KENDO_WINDOW :number= 3;
    const linkSenzaNavigazione: string = this.inserisciInQueryString(link, "sidebar", "off");

    if (service.enum_TipoAperturaPagina == APRI_IN_FINESTRA_CORRENTE){
      window.location.href = link;
    } else if (service.enum_TipoAperturaPagina == APRI_IN_NUOVA_FINESTRA){
        window.open(linkSenzaNavigazione);
    } else if (service.enum_TipoAperturaPagina == APRI_IN_POPUP){
      //TODO cambiare il nome della scheda
      window.open(linkSenzaNavigazione, "testone", "height=700, width = 1000, menubar=yes, resizable=yes, scrollbars=yes, top=0, left=0");
    } else if (service.enum_TipoAperturaPagina == APRI_IN_KENDO_WINDOW){
//TODO KENDO POP UP
    }


  }
  redirect(service: LinkMenu) {
    let objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    objParametriAgenda.RedirectUrl = service.redirectUrl;
    if (service.richiedeAziendaSelezionata > 0 && objParametriAgenda.Piva == '') {
      this.giasDialogService.alertMessage(this.translocoService.translate('SelezionareImpresa'));
      return;
    }

      if (service.sitoRichiesto == Enum_SiteRedirector.GiasNG) {

          //Resetto l'objParametriAgenda lasciando solamente i dati che potrebbero essere utili

          let objParamToSave = {
              Piva: objParametriAgenda.Piva,
              RagSoc: objParametriAgenda.RagSoc,
              RedirectUrl: objParametriAgenda.RedirectUrl,
              Pagina_Provenienza: objParametriAgenda.Pagina_Provenienza,
              Pagina_Richiesta: objParametriAgenda.Pagina_Richiesta,
              IdSezione: objParametriAgenda.IdSezione,
              Sito_Provenienza: Enum_SiteRedirector.GiasNG,
              Pagina_Provenienza_AltroSito: 0
          };

          this.objParametriAgendaService.resettaObjAgenda(objParametriAgenda);

          Object.entries(objParamToSave).forEach(([key,value])=>{
              if(objParametriAgenda[key] !== undefined){
                  objParametriAgenda[key] = value;
              }
          });

          this.gestioneRichiesteService.gestionePassaggioStessoSito(service.paginaRichiesta, [], service.idSezione).then((val) => {

                switch(service.paginaRichiesta){
                    case enum_PagineGiasNG.Pagina_Edit_Attivita:
                        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                        objParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
                        objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
                        break;
                }

                this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);
                this.router.navigate([val]);

                document.getElementById("GiasSidenav").style.width = "0";
                document.getElementById("contenitore_principale").style.marginLeft = "0";
                document.getElementById("openMenu").classList.remove("hide")
                document.getElementById("closeMenu").classList.add("hide");
                document.getElementById("GiasSidenav").classList.remove("openSidebarMenu");
        })
      } else {
        this.gestioneRichiesteService
        .gestionePassaggioAltroSito(service.sitoRichiesto, service.paginaRichiesta, [], objParametriAgenda, true, service.idSezione)
        .then((val) => this.gestioneRedirect(val, service));
      }
  }

  navigateToFavorites(): void {
    const params = this.objParametriAgendaService.getObjParamValue();
    params.IdSezione = 0;
    this.objParametriAgendaService.changeObjParametriAgenda(params);

    this.router.navigateByUrl(FAVORITES_URL);
  }

  onFilterText(): void {
    this.changeDetectionRef.markForCheck();
  }

  selectTab(tab: SideMenuTabs): void {
    this.selectedTab = tab;
    localStorage.setItem(SIDE_MENU_TABS_KEY, this.selectedTab.toString());
  }

  private init(): void {
    this.menuClient
      .menuGetAlberoMenu()
      .pipe(
        takeUntil(this.signal$),
        catchError((err) => {
          console.error(err);
          return of({ RispostaStringa: { Menus: [], MenusPreferiti: [] } });
        })
      )
      .subscribe((val) => {
        this.allMenu = val.RispostaStringa.Menus ?? [];
        this.favoritesMenu = val.RispostaStringa.MenusPreferiti ?? [];

        if (this.allMenu.length == 0) {
          this.giasDialogService.baseError('', 'NonSonoStateTrovateVociDiMenuDisponibili');
        }
      });
    this.loadComplete = true;
  }

  private initSelectedTab(): SideMenuTabs {
    const storedValue = localStorage.getItem(SIDE_MENU_TABS_KEY);

    // Check if the stored value is a valid enum key
    if (storedValue != null && Object.values(SideMenuTabs).includes(Number(storedValue))) {
      return Number(storedValue) as SideMenuTabs;
    }

    // Default to Favorites if value is invalid or not present
    const defaultTab = SideMenuTabs.All;
    localStorage.setItem(SIDE_MENU_TABS_KEY, defaultTab.toString());
    return defaultTab;
  }
}
