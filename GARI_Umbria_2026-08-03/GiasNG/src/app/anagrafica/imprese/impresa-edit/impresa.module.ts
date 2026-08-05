import { LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from '../../../Utility/uikit.module';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { ImpresaEditComponent } from './impresa-edit.component';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { ImpreseServiceProvider } from 'app/Service/ServiceFactory/imprese.factory.provider';
import { ContattiEditComponent } from './contatti-edit/contatti-edit.component';
import { LabelModule } from '@progress/kendo-angular-label';
import { CooperativeComponent } from './cooperative-edit/cooperative-edit.component';
import {SwitchModule} from "@progress/kendo-angular-inputs";
import {DatiGeneraliGuard} from '../../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../../Model/TipiEnumerativi';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import {FaIconComponent} from '@fortawesome/angular-fontawesome';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] =  () => import(`./i18n/${lang}.json`);
    return acc;
}, {});


@NgModule({
    declarations: [
        ImpresaEditComponent,
        ContattiEditComponent,
        CooperativeComponent
    ],
    exports: [ImpresaEditComponent],
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: ImpresaEditComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [
                        enum_Security_Attivita.Anagrafica_Impresa
                    ],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Edit_Impresa
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
        LabelModule,
        SwitchModule,
        GiasKendoGridModule,
        GiasUikitModule,
        FaIconComponent
    ],
    providers: [
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'impresa',
                loader
            }
        },
        ImpreseServiceProvider
    ],
    //entryComponents: [ImpresaEditComponent]
})
export class ImpresaModule {
}
