import { LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from '../../../Utility/uikit.module';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { TranslocoRootModule } from '../../../transloco/transloco-root.module';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { LOCALIZATION_LANGUAGES } from '../../../Model/CostantiPersonalizzate';
import { CampiEditComponent } from './campi-edit.component';
import { AppezzamentiCampoEditComponent } from './appezzamento-campo-edit/appezzamento-campo-edit.component';
import { CatastoCampoEditComponent } from './catasto-campo-edit/catasto-campo-edit.component';
import { IntlModule } from '@progress/kendo-angular-intl';
import { CampiServiceProvider } from 'app/Service/ServiceFactory/campi.factory.provider';
import { ImpiantiServiceProvider } from 'app/Service/ServiceFactory/impianti.factory.provider';
import { CampiEditService } from './campi-edit.service';
import { AppezzamentoCampoService } from './appezzamento-campo-edit/appezzamento-campo.service';
import {DatiGeneraliGuard} from '../../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../../Model/TipiEnumerativi';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';



export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});


@NgModule({
    declarations: [
        CampiEditComponent,
        AppezzamentiCampoEditComponent,
        CatastoCampoEditComponent
    ],
    exports: [CampiEditComponent],
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: CampiEditComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [
                        enum_Security_Attivita.Anagrafica_Campo
                    ],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Edit_Campo
                }
            }
        ]),
        CommonModule,
        FormsModule,
        UikitModule,
        ReactiveFormsModule,
        LayoutModule,
        IconsModule,
        ButtonsModule,
        TranslocoRootModule,
        IntlModule,
        GiasKendoGridModule,
        GiasUikitModule
    ],
    providers: [
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'campo',
                loader
            }
        },
        DatePipe,
        CampiServiceProvider,
        ImpiantiServiceProvider
    ],
    //entryComponents: [CampiEditComponent]
})
export class CampoModule {
}
