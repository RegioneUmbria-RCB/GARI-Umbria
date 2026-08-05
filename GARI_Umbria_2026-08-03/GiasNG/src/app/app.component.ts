import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  Inject,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild
} from '@angular/core';
import {NavigationEnd, NavigationStart, RouteConfigLoadEnd, RouteConfigLoadStart, Router} from '@angular/router';
import { PivaValidatorService } from './anagrafica/imprese/piva-validator.service';
import { FooterModel } from './Master/footer/footer.component';
import { HeaderModel } from './Master/header/header.component';
import { MenuContestualeService } from './Master/menu-contestuale/menu-contestuale.service';
import { CssService } from './Service/css.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { MasterService } from './Service/master.service';
import { ProvideAnagraficaTreeDeps } from './Utility/Template/kendo-tree/utility/providers';
import { GiasWindowsService } from 'gias-ui-kit';
import {take, tap} from "rxjs/operators";
import {filter, Subscription} from "rxjs";
import {LoadExternalStylesService} from "./Service/loadExternalStyles.service";
import {ConfigurazioneSitiService} from "./Service/configurazione-siti.service";
import {DOCUMENT} from "@angular/common";
import { TranslocoService } from '@jsverse/transloco';
import { SessionStorageService } from 'ngx-webstorage';
import { COOKIE_LINGUA, SESSION_LINGUA } from './Model/CostantiPersonalizzate';
import { CookieService } from 'ngx-cookie-service';
import { MatomoService } from './matomo/matomo.service';
import { MatomoConfig } from './matomo/matomo-config/model/matomo-config.model';

@Component({
    standalone: false,
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    providers: [{ provide: LOADING_TOKEN, useClass: LoadingService }],
})
export class AppComponent implements OnInit,OnDestroy {
    title = 'GIAS';
    header: HeaderModel;
    footer: FooterModel;
    loaded = false;
    showBackground = false;
    isGISRoute = false;
    @ViewChild('routerOutlet', { static: false }) divRouterOutlet: ElementRef;
    Subs: Subscription= new Subscription();
    translationsLoaded = false;

    constructor(private masterService: MasterService,
        private menuContestualeService: MenuContestualeService,
        private router: Router,
        private loadExternalStylesService: LoadExternalStylesService,
        @Inject(LOADING_TOKEN) private loadingService: LoadingService,
        private cssService: CssService,
        private giasWindowService: GiasWindowsService,
        private translocoService: TranslocoService,
        private cookieService: CookieService,
        private configurazioneSitiService: ConfigurazioneSitiService,
        private matomoService: MatomoService,
        @Inject(DOCUMENT) private _document: Document) {

            router.events.subscribe((val) => {
                if(val instanceof NavigationStart && val.url == '/GIS'){
                    this.isGISRoute = true;
                    document.body.classList.add('gis-page');
                }else if(val instanceof NavigationStart){
                    this.isGISRoute = false;
                    document.body.classList.remove('gis-page');
                }

              if(val instanceof NavigationStart || val instanceof NavigationEnd)
                this.CaricaPersonalizzazioni();
              
              if (val instanceof NavigationEnd) {
                // Notifica Matomo del cambio URL
                this.matomoService.trackPageView(val.urlAfterRedirects, document.title);
              }
            });

    }

  async ngOnInit() {
    // first load translations and the render the content
    const lingua = this.cookieService.get(COOKIE_LINGUA);
    this.translocoService
      .load(MasterService.getLangCode(lingua == null ? 1 : +lingua))
      .subscribe(async () => await this.load());
  }

  async load() {
    this.translationsLoaded = true;

    this.header = Object.assign(new HeaderModel, {
      visible: true,
      logo: '',
      BackButton: true,
      ColumnLeft: true,
      ColumnRight: true
    });
    this.footer = Object.assign(new FooterModel, {
      visible: true,
      logo: 'agronica/AB_Immagini/logo/logo_agronica_small.png',
      mail: 'assistenza@agronica.it',
      phoneImg: 'agronica/AB_Immagini/Icone32/tel_icon_32.png',
      mailImg: 'agronica/AB_Immagini/Icone32/email_icon_32.png'
    });

    this.menuContestualeService.changeMenuContestualeSettings({
      show: false,
      background_color: "",
      title: "titolo test",
      search: true,
      bookmarks: true,
      contextualMenu: true,
      IDTipoSezione: 0,
      IDSezionePadre: 0
    });

    this.masterService.changeFooter(this.footer);
    this.masterService.changeHeader(this.header);
    this.masterService.currentFullLoad.subscribe(async fullLoad => {
      if (fullLoad) {
        this.loaded = true;
      }
    });

    this.masterService.currentShowBackground.subscribe((val) => {
      this.showBackground = val;
    });

    this.masterService.set_FullLoad(true);

    // this.router.events.subscribe(event => {
    //     if (event instanceof RouteConfigLoadStart) {
    //         this.loadingService.set_isLoading({ isLoading: true, component: this.divRouterOutlet });
    //     } else if (event instanceof RouteConfigLoadEnd) {
    //         this.loadingService.set_isLoading({ isLoading: false, component: this.divRouterOutlet });
    //     }
    // });

    // this.masterService.initialLoadCompleteSource.pipe(
    //     filter((val) => {
    //         console.log("FILTER", val);
    //         return val
    //     }),
    //     take(1),
    //     tap((val) => {
    //         console.log("QUI CI ARRIVO", val);
    //         if (val){
    //             this.cssService.loadStyle('stylesColdiretti.css', 'ColdirettiTheme');
    //         }
    //     })
    // ).subscribe()
    this.cssService.loadStyle('stylesBootstrapClassic.css', 'client-theme');
  }

    /*
    * @description Applico le personalizzazioni sul titolo e la favicon
    * */
  private CaricaPersonalizzazioni(){

        // if(this.masterService.ObjParametri_Server && this.masterService.ObjParametri_Server != ""){
    //se il master service è pronto, carico le personalizzazioni e Matomo, se presenti le configurazioni
    this.masterService.initialLoadCompleteSource
                        .pipe(filter(v => v === true), take(1))
                        .subscribe(() => {
                                          if (this._document.title === "" ||
                                            !this._document.getElementById('appFavicon').getAttribute('href') ||
                                            this._document.getElementById('appFavicon').getAttribute('href') === "data:image/x-icon;base64" ||
                                            this._document.getElementById('appFavicon').getAttribute('href') === ""){

                                            this.Subs.add(this.configurazioneSitiService.leggiChiave("PersonalizzazioniGraficheCliente").subscribe(config=>{
                                              if(config && config.Valore !== "") {
                                                const personalizzazioni = JSON.parse(config.Valore);
                                                if (personalizzazioni.Logo_Favicon_PATH && personalizzazioni.Logo_Favicon_PATH !== "" && personalizzazioni.Title && personalizzazioni.Title !== "") { 
                                                  this._document.getElementById('appFavicon').setAttribute('href', this.masterService._link_GiasBase + personalizzazioni.Logo_Favicon_PATH);
                                                  this._document.title = personalizzazioni.Title;
                                                } else 
                                                  this.setDefaultLogoAndTitle();
                                              } else
                                                this.setDefaultLogoAndTitle();
                                              //console.log("Load Title and Favicon "+this._document.title);
                                            }));

                                          }
                                        // }

                                        // Carico Matomo se non è un superuser e se non è già stato caricato
                                        if (!this.matomoService.isLoaded && !this.masterService.isSuperuser()) {
                                          this.Subs.add(this.configurazioneSitiService.leggiChiave("ConfigMatomo").subscribe(configMatomo => {
                                            if (configMatomo && configMatomo.Valore !== "") {
                                              const matomoConfig: MatomoConfig = JSON.parse(configMatomo.Valore);
                                              if (matomoConfig.Domain && matomoConfig.Container) 
                                                this.matomoService.loadMatomo(matomoConfig.Domain, matomoConfig.Container, this.masterService.getCurrentUserUsername());
                                            }
                                          }));
                                        }
    });
    
    // catch (error) {
    //   // potremmo finire qui perché il MasterSercice non è ancora pronto
    // }

  }

  private setDefaultLogoAndTitle() {
      this._document.getElementById('appFavicon').setAttribute('href', './assets/favicon.ico');
      this._document.title = 'Agronica';
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }


}

