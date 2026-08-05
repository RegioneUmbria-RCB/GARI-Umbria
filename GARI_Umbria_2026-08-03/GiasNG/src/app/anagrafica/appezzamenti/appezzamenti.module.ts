import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppezzamentoEditComponent } from './appezzamenti-edit/appezzamento-edit.component';
import { ImpiantoEditComponent } from '../impianti/impianto-edit/impianto-edit.component';
import { EsercizioEditComponent } from '../esercizi/esercizio-edit/esercizio-edit.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from '../../Utility/uikit.module';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { DatiCatastaliComponent } from './appezzamenti-edit/dati-catastali/dati-catastali.component';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { UtilizzoTerrenoComponent } from '../impianti/impianto-edit/utilizzo-terreno/utilizzo-terreno.component';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { DestinazioneUsoService } from 'app/Service/Metaschema/destinazioneUso.service';
import { VarietaService } from 'app/Service/Metaschema/varieta.service';
import { GruppoFinalitaService } from 'app/Service/Metaschema/finalita.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GruppoVarietaleService } from 'app/Service/Metaschema/gruppoVarietale.service';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { ImpiantiServiceProvider } from 'app/Service/ServiceFactory/impianti.factory.provider';
import { IndirizziComponent } from './appezzamenti-edit/impianto-edit-indirizzi/indirizzi.component';
import { AppezzamentoGlobalEditComponent } from './appezzamento-global-edit/appezzamento-global-edit.component';
import {SalvataggioAppezzamentoGISComponent} from './appezzamenti-edit/salvataggio-appezzamento-GIS/salvataggio-appezzamento-GIS.component';
import {EserciziComponent} from '../esercizi/esercizi.component';
import {DatiGeneraliGuard} from '../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../Model/TipiEnumerativi';
import {DatiCatastaliService} from './appezzamenti-edit/dati-catastali/dati-catastali.service';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import {HttpClientModule, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {AgeaCodesComponent} from './appezzamenti-edit/agea-codes/agea-codes.component';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    AppezzamentoEditComponent,
    ImpiantoEditComponent,
    EsercizioEditComponent,
    AppezzamentoGlobalEditComponent,
    DatiCatastaliComponent,
    UtilizzoTerrenoComponent,
    IndirizziComponent,
    SalvataggioAppezzamentoGISComponent
  ],
  exports: [
    AppezzamentoEditComponent,
    ImpiantoEditComponent,
    EsercizioEditComponent,
    AppezzamentoGlobalEditComponent,
    IndirizziComponent,
    SalvataggioAppezzamentoGISComponent
  ],
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppezzamentoGlobalEditComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [
                        enum_Security_Attivita.Anagrafica_Appezzamento,
                        enum_Security_Attivita.Anagrafica_Impianto
                    ],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal
                }
            }
        ]),
        CommonModule,
        FormsModule,
        UikitModule,
        ReactiveFormsModule,
        LayoutModule,
        IconsModule,
        TranslocoRootModule,
        FontAwesomeModule,
        ButtonsModule,
        InputsModule,
        LabelModule,
        GiasKendoGridModule,
        GiasUikitModule,
        AgeaCodesComponent
    ],
  providers:[
    SpecieVegetaliService,
    DestinazioneUsoService,
    GruppoFinalitaService,
    GruppoVarietaleService,
    VarietaService,
    FunzioniComuniService,
    DatiCatastaliService,
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'appezzamento',
        loader
      }
    },
    ImpiantiServiceProvider
  ]
})
export class AppezzamentoModule { }
