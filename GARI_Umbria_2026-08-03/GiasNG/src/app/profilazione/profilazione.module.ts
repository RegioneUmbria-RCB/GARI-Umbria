import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuProfilazioneComponent } from './menu-profilazione.component';
import { ProfilazioneRoutingModule } from './profilazione-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { UikitModule } from 'app/Utility/uikit.module';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { IconsModule } from '@progress/kendo-angular-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import {
  MenuProfilazioneGridComponent
} from './gestione-utenti/utenti/griglia-menu-profilazione/grid-menu-profiliazione.component';
import { GridProfiliComponent } from './profili-permessi/griglia-profili/grid-profili.component';
import { GridGruppiComponent } from './gruppi-utenti/grid-gruppi/grid-gruppi.component';
import { GridPermessiEditComponent } from './profili-permessi/grid-permessi-edit/grid-permessi-edit.component';
import { ProfiliPermessiComponent } from './profili-permessi/profili-permessi.component';
import { GridPermessiComponent } from './profili-permessi/grid-permessi/grid-permessi.component';
import {
  GridUtentiPermessiComponent
} from './gestione-utenti/utenti/grid-utenti-permessi/grid-utenti-permessi.component';
import { LabelModule } from '@progress/kendo-angular-label';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { GridProfiliPermessiComponent } from './profili-permessi/grid-profili-permessi/grid-profili-permessi.component';
import { FormImpostazioniComponent } from './impostazioni-utente/form-impostazioni/form-impostazioni.component';
import {
  GeneralFieldComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-general-field/settings-general-field.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import {
  SettingsAnnataAgrariaComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-annata-agraria/settings-annata-agraria.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import {
  SettingsSpecieVarietaComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-specie-varieta/settings-specie-varieta.component';
import {
  SettingsCategorieMagazzinoComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-categorie-magazzino/settings-categorie-magazzino.component';
import {
  SettingsProxyComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-proxy/settings-proxy.component';
import {
  MenuImpostazioniAziendeCentriComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/menu-impostazioni-aziende-centri.component';
import {
  GridAziendeCentriComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/pagina-impostazioni-massive/grid-aziende-centri/grid-aziende-centri.component';
import {
  PaginaImpostazioniMassiveComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/pagina-impostazioni-massive/pagina-impostazioni-massive.component';
import { ImpostazioniUtenteComponent } from './impostazioni-utente/impostazioni-utente/impostazioni-utente.component';
import {
  FormImpostazioniAziendeCentriComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/pagina-impostazioni-massive/form-impostazioni-aziende-centri/form-impostazioni-aziende-centri.component';
import {
  SettingsLavorazioniGridComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-lavorazioni-grid/settings-lavorazioni-grid.component';
import {
  PaginaAziendeCentriImpostazioniComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/pagina-aziende-centri-impostazioni/pagina-aziende-centri-impostazioni.component';
import {
  GridAziendeCentriImpostazioniComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/pagina-aziende-centri-impostazioni/grid-aziende-centri-impostazioni/grid-aziende-centri-impostazioni.component';
import {
  CardImpostazioneComponent
} from './impostazioni-utente/form-impostazioni/card-impostazione/card-impostazione.component';
import { SettingsManagerComponent } from './impostazioni-utente/settings-manager/settings-manager.component';
import {
  SezioneImpostazioniComponent
} from './impostazioni-utente/form-impostazioni/section-impostazioni/sezione-impostazioni.component';
import { GridPasswordComponent } from './gestione-utenti/components/grid-password/grid-password.component';
import { MenuUtentiComponent } from './gestione-utenti/utenti/menu-utenti.component';
import { VisibilitaUtenteEditComponent } from './menu-visibilita/edit/visibilita-utente-edit.component';
import { GridVisibilitaEditComponent } from './menu-visibilita/edit/grid-visibilita-edit/grid-visibilita-edit.component';
import { MenuVisibilitaComponent } from './menu-visibilita/menu-visibilita.component';
import { ImpreseServiceProvider } from 'app/Service/ServiceFactory/imprese.factory.provider';
import { GruppiUtentiComponent } from './gruppi-utenti/gruppi-utenti.component';
import {
  SettingsGiasAppComponent
} from './impostazioni-utente/tipi-campo-impostazioni/settings-gias-app/settings-gias-app.component';
import { GridGroupTransitionsComponent } from './gruppi-utenti/grid-group-transitions/grid-group-transitions.component';
import {
  FiltroSqlMateriePrimeComponent
} from './impostazioni-utente/tipi-campo-impostazioni/filtro-sql-materie-prime/filtro-sql-materie-prime.component';
import {
  FormatiStampaComponent
} from './impostazioni-utente/tipi-campo-impostazioni/formati-stampa/formati-stampa.component';
import { SettingsGisComponent } from './impostazioni-utente/tipi-campo-impostazioni/settings-gis/settings-gis.component';
import { ClientePermessiComponent } from './cliente-permessi/cliente-permessi.component';
import { FiltriUtenteComponent } from './gestione-utenti/utenti/filtri-utente/filtri-utente.component';
import { ClientePermessiGridComponent } from './cliente-permessi/cliente-permessi-grid/cliente-permessi-grid.component';
import { ImportUtentiComponent } from './import-utenti/import-utenti.component';
import { FileSelectModule } from "@progress/kendo-angular-upload";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import { WidgetConfigListComponent } from "../widget-config/section/widget-config-list/widget-config-list.component";
import { DashboardModule } from 'app/dashboard/dashboard.module';
import { MappaturaIsolamentiComponent } from './impostazioni-utente/tipi-campo-impostazioni/mappatura-isolamenti/mappatura-isolamenti.component';
import { DocumentaryManagementComponent } from './impostazioni-utente/tipi-campo-impostazioni/documentary-management/documentary-management.component';
import { ConsideraValiditaCellComponent } from './menu-visibilita/pratiche/considera-validita-cell/considera-validita-cell.component';
import { GrigliaPraticheComponent } from './menu-visibilita/pratiche/griglia-pratiche/griglia-pratiche.component';
import { TabPraticheComponent } from './menu-visibilita/pratiche/tab-pratiche/tab-pratiche.component';

export const SHARED_IMPORTS = [
  GiasUikitModule,
  GiasKendoGridModule,
  TranslocoRootModule,
  ReactiveFormsModule,
  FormsModule,
  CommonModule,
  FontAwesomeModule,
  LayoutModule,
  IconsModule,
  LabelModule,
  InputsModule,
  ButtonsModule,
];

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    MenuProfilazioneComponent,
    MenuProfilazioneGridComponent,
    GridProfiliComponent,
    GridGruppiComponent,
    GridPermessiEditComponent,
    ProfiliPermessiComponent,
    GridPermessiComponent,
    GridUtentiPermessiComponent,
    GridProfiliPermessiComponent,
    FormImpostazioniComponent,
    GeneralFieldComponent,
    SettingsAnnataAgrariaComponent,
    SettingsSpecieVarietaComponent,
    SettingsCategorieMagazzinoComponent,
    SettingsProxyComponent,
    MenuImpostazioniAziendeCentriComponent,
    GridAziendeCentriComponent,
    PaginaImpostazioniMassiveComponent,
    ImpostazioniUtenteComponent,
    FormImpostazioniAziendeCentriComponent,
    SettingsLavorazioniGridComponent,
    PaginaAziendeCentriImpostazioniComponent,
    GridAziendeCentriImpostazioniComponent,
    CardImpostazioneComponent,
    SettingsManagerComponent,
    SezioneImpostazioniComponent,
    GridPasswordComponent,
    MenuUtentiComponent,
    VisibilitaUtenteEditComponent,
    GridVisibilitaEditComponent,
    MenuVisibilitaComponent,
    GruppiUtentiComponent,
    SettingsGiasAppComponent,
    GridGroupTransitionsComponent,
    FiltroSqlMateriePrimeComponent,
    FormatiStampaComponent,
    SettingsGisComponent,
    FiltriUtenteComponent,
    ClientePermessiComponent,
    ClientePermessiGridComponent,
    ImportUtentiComponent,
    ConsideraValiditaCellComponent,
    GrigliaPraticheComponent,
    TabPraticheComponent
  ],
  imports: [
    CommonModule,
    ProfilazioneRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    IconsModule,
    ButtonsModule,
    InputsModule,
    LabelModule,
    LayoutModule,
    UikitModule,
    FontAwesomeModule,
    TranslocoRootModule,
    DateInputsModule,
    FormsModule,
    ReactiveFormsModule,
    LabelModule,
    FileSelectModule,
    GiasKendoGridModule,
    GiasUikitModule,
    DashboardModule,
    WidgetConfigListComponent,
    MappaturaIsolamentiComponent,
    DocumentaryManagementComponent
  ],
  providers: [
    ImpreseServiceProvider,
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'prof',
        loader
      }
    }
  ]
})
export class ProfilazioneModule {
}
