import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';
import { DatePipe, registerLocaleData } from '@angular/common';
import localeIt from '@angular/common/locales/it';
registerLocaleData(localeIt);
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { FooterComponent } from './Master/footer/footer.component';
import { HeaderComponent } from './Master/header/header.component';
import { SideMenuComponent } from './Master/sidemenu/sidemenu.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import '@progress/kendo-angular-intl/locales/it/all';
import '@progress/kendo-angular-intl/locales/it/calendar';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { GridModule } from '@progress/kendo-angular-grid';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { MenusModule } from '@progress/kendo-angular-menu';
import { faHome, faReply, fas } from '@fortawesome/free-solid-svg-icons';
import { BackButtonDirective } from './back-button.directive';
import { HomeButtonDirective } from './home-button.directive';
import { TranslocoRootModule } from './transloco/transloco-root.module';
import { GiasKendoGridModule, GRID_HTTP_TOKEN, GRID_WORKER_URL } from 'gias-kendo-grid';
import { DefaultHttpService } from 'gias-kendo-grid';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TranslocoPipe, TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { LOCALIZATION_LANGUAGES } from './Model/CostantiPersonalizzate';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { AccessoNegatoComponent } from './accesso-negato/accesso-negato.component';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { FormGroupDirective, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WindowMessaggioErroreComponent } from './Master/window-messaggio-errore/window-messaggio-errore.component';
import { DBConfig, NgxIndexedDBModule } from 'ngx-indexed-db';
import { MenuContestualeComponent } from './Master/menu-contestuale/menu-contestuale.component';
import { ImpreseFilterComponent } from './Master/menu-contestuale/imprese-filter/imprese-filter.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { OverlayModule } from '@angular/cdk/overlay';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { LoginComponent } from './login/login.component';
import { TestComponent } from './test/test.component';
import { TestAnagraficaComponent } from './test/test-anagrafica/test-anagrafica.component';
import { MessageService } from '@progress/kendo-angular-l10n';
import { CustomMessagesService } from './Service/kendo-messages.service';
import { UikitModule } from './Utility/uikit.module';
import { TestAnagraficaImpreseComponent } from './test/test-anagrafica/test-anagrafica-imprese/test-anagrafica-imprese.component';
import { TestAnagraficaCentriComponent } from './test/test-anagrafica/test-anagrafica-centri/test-anagrafica-centri.component';
import { TestAnagraficaCampiComponent } from './test/test-anagrafica/test-anagrafica-campi/test-anagrafica-campi.component';
import { TestAnagraficaCatastoComponent } from './test/test-anagrafica/test-anagrafica-catasto/test-anagrafica-catasto.component';
import { TestAnagraficaImpiantiComponent } from './test/test-anagrafica/test-anagrafica-impianti/test-anagrafica-impianti.component';
import { TestAnagraficaMacchineComponent } from './test/test-anagrafica/test-anagrafica-macchine/test-anagrafica-macchine.component';
import { API_BASE_URL } from './Service/api.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { CompressioneInterceptor } from './interceptors/compressione.interceptor';
import { DialogComponent } from './dialog/dialog.component';
import { BreadcrumbComponent } from './Master/breadcrumb/breadcrumb.component';
import { PreferitiConfigComponent } from './preferiti-config/preferiti-config.component';
import { PreferitoCardComponent } from './preferiti-config/preferito-card/preferito-card.component';
import { GISModule } from './GIS/GIS.module';
import { SideMenuServicesComponent } from './Master/sidemenu/sidemenu-services/sidemenu-services.component';
import { SideMenuFooterComponent } from './Master/sidemenu/sidemenu-footer/sidemenu-footer.component';
import { ChartsModule } from '@progress/kendo-angular-charts';
import 'hammerjs';
import { SidemenuChildComponent } from './Master/sidemenu/sidemenu-child/sidemenu-child.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ExcelExportModule } from '@progress/kendo-angular-excel-export';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { FilterPipe } from './Master/sidemenu/filter.pipe';
import { DateInterceptor } from "./interceptors/date.interceptor";
import { UploadsModule } from '@progress/kendo-angular-upload';
import { VisiteModule } from './visite/visite.module';
import { FilterLinkMenuChildrenPipe } from './Master/sidemenu/sidemenu-services/filter-children.pipe';
import { CoreWSInterceptor } from "./interceptors/coreWS.interceptor";
import { CircularGaugeModule, GaugesModule } from '@progress/kendo-angular-gauges';
import { PercentGridViewerComponent } from "./requisiti-stabilimento/percent-grid-viewer/percent-grid-viewer.component";
import { NETCORE6_API_BASE_URL } from './Service/net-core6-api.service';
import { NetCore6Interceptor } from "./interceptors/net-core-6.interceptor";
import { ClearInputButtonDirective } from './clear-input-button.directive';
import { MasterService } from './Service/master.service';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { provideNgxWebstorage, withLocalStorage, withNgxWebstorageConfig, withSessionStorage } from 'ngx-webstorage';
import { TranslocoRootModule as TranslocoRootModule_alias } from './transloco/transloco-root.module';
import { ICON_SETTINGS, IconSettingsService, IconsService } from "@progress/kendo-angular-icons";
import { PermessiUtenteService } from './Service/permessi-utente.service';
import { ObjParametriAgendaService } from './Service/obj-parametri-agenda.service';
import { AjaxAgronicaAPIService } from './Service/ajax-agronica.api.service';
import { GIAS_MASTER_SERVICE_TOKEN, GIAS_API_SERVICE_TOKEN, GiasUikitModule, GIAS_USERNAME_TOKEN, GIAS_PARAMETRI_AGENDA_TOKEN } from 'gias-ui-kit';
import { CacCodificheComponent } from './cac-codifiche/cac-codifiche.component';
import { QdCAC_API_BASE_URL } from 'app/Service/qdca-compliance-api.service';

const dbConfig: DBConfig = {
  name: 'GiasDB',
  version: 1,
  objectStoresMeta: [{
    store: 'imprese',
    storeConfig: { keyPath: 'id', autoIncrement: false },
    storeSchema: [
      { name: 'row', keypath: 'row', options: { unique: false } },
    ]
  }]
};


export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`../assets/i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    FooterComponent,
    HeaderComponent,
    PreferitiConfigComponent,
    SideMenuComponent,
    BreadcrumbComponent,
    PreferitoCardComponent,
    BackButtonDirective,
    HomeButtonDirective,
    ClearInputButtonDirective,
    AccessoNegatoComponent,
    WindowMessaggioErroreComponent,
    MenuContestualeComponent,
    ImpreseFilterComponent,
    LoginComponent,
    TestComponent,
    TestAnagraficaComponent,
    TestAnagraficaImpreseComponent,
    TestAnagraficaCentriComponent,
    TestAnagraficaCampiComponent,
    TestAnagraficaCatastoComponent,
    TestAnagraficaImpiantiComponent,
    TestAnagraficaMacchineComponent,
    DialogComponent,
    SideMenuServicesComponent,
    SidemenuChildComponent,
    SideMenuFooterComponent,
    FilterPipe,
    FilterLinkMenuChildrenPipe,
    PercentGridViewerComponent,
 
  ],
  imports: [
    // BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserModule,
    LayoutModule,
    AppRoutingModule,
    HttpClientModule,
    FontAwesomeModule,
    GridModule,
    NotificationModule,
    NgxIndexedDBModule.forRoot(dbConfig),
    // StoreModule.forRoot(reducers, {
    //     metaReducers,
    //     runtimeChecks: {
    //         strictStateImmutability: true,
    //         strictActionImmutability: true
    //     }
    // }),
    ButtonsModule,
    FormsModule,
    IconsModule,
    MenusModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    TranslocoRootModule,
    TooltipModule,
    NavigationModule,
    DialogsModule,
    DropDownsModule,
    OverlayModule,
    IndicatorsModule,
    UikitModule,
    GISModule,
    ChartsModule,
    DateInputsModule,
    ExcelExportModule,
    InputsModule,
    UploadsModule,
    VisiteModule,
    GaugesModule,
    CircularGaugeModule,
    NgxMaskDirective,
    NgxMaskPipe,
    TranslocoRootModule_alias,
    GiasKendoGridModule,
    GiasUikitModule,
    
  ],
  providers: [
    {
      provide: LOCALE_ID,
      useValue: 'it-IT',
    },
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'global',
        loader
      },
    },
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'giasgrid',
      }
    },
    {
      provide: GRID_HTTP_TOKEN, useClass: DefaultHttpService
    },
    FormGroupDirective,
    TranslocoPipe,
    DatePipe,
    MasterService,
    { provide: GIAS_MASTER_SERVICE_TOKEN, useExisting: MasterService },
    { provide: GIAS_USERNAME_TOKEN, useExisting: PermessiUtenteService },
    { provide: GIAS_PARAMETRI_AGENDA_TOKEN, useExisting: ObjParametriAgendaService },
    { provide: GIAS_API_SERVICE_TOKEN, useExisting: AjaxAgronicaAPIService },
    { provide: MessageService, useClass: CustomMessagesService },
    // { provide: API_BASE_URL, useValue: environment.link_API }
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: DateInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CompressioneInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: CoreWSInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: NetCore6Interceptor, multi: true },
    { provide: API_BASE_URL, useValue: "https://localhost:5001" },
    { provide: NETCORE6_API_BASE_URL, useValue: "https://localhost:5011" },
    { provide: QdCAC_API_BASE_URL, useValue: "https://localhost:5021" },
    { provide: GRID_WORKER_URL, useValue: "assets\\grid.worker.js" },
    provideNgxMask(),
    provideNgxWebstorage(
      withNgxWebstorageConfig({ separator: ':', caseSensitive: true }),
      withLocalStorage(),
      withSessionStorage()
    ),
    IconSettingsService,
    IconsService,
    {
      provide: ICON_SETTINGS,
      useValue: { type: 'font' },
    },
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
  constructor(private library: FaIconLibrary) {
    library.addIconPacks(fas);
    library.addIcons(faHome, faReply);
  }
}
