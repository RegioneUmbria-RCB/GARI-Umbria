import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from '../../Utility/uikit.module';
import { RouterModule, Routes } from '@angular/router';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { CentroEditComponent } from './centri-edit/centri-edit.component';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { CentriComponent } from './centri.component';
import { CentriHttpService } from './services/centri-grid-config.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DatiCentroComponent } from './centri-edit/components/dati-centro/dati-centro.component';
import { DatiAccessoriComponent } from './centri-edit/components/dati-accessori/accessori.component';
import { CentriCodiciGridComponent } from './centri-edit/components/dati-accessori/grid-codici/grid-codici.component';
import { BiologicoComponent } from './centri-edit/components/biologico/biologico.component';
import { LabelModule } from '@progress/kendo-angular-label';
import { CatastoServiceProvider } from 'app/Service/ServiceFactory/catasto.factory.provider';
import {DatiGeneraliGuard} from '../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../Model/TipiEnumerativi';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        CentroEditComponent,
        CentriComponent,
        DatiCentroComponent,
        DatiAccessoriComponent,
        CentriCodiciGridComponent,
        BiologicoComponent
    ],
    exports: [
        CentroEditComponent,
        CentriComponent
    ],
    providers: [
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'centro',
                loader
            }
        },
        { provide: CentriHttpService },
        CatastoServiceProvider
    ],
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: CentroEditComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [
                        enum_Security_Attivita.Anagrafica_CentroAziendale
                    ],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Edit_Centro
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
        LabelModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})
export class CentriModule { }
