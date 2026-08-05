import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatastoEditComponent } from './catasto-edit/catasto-edit.component';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from 'app/Utility/uikit.module';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { RouterModule } from '@angular/router';
import { CatastoPossessiEditComponent } from './catasto-edit/catasto-possessi-edit/catasto-possessi-edit.component';
import { PossessiService } from './catasto-edit/catasto-possessi-edit/Possessi.service';
import { CatastoMetodoProduzioneEditComponent } from './catasto-edit/catasto-metodo-produzione-edit/catasto-metodo-produzione-edit.component';
import { CatastoMacrousiEditComponent } from './catasto-edit/catasto-macrousi-edit/catasto-macrousi-edit.component';
import { CatastoZoneEditComponent } from './catasto-edit/catasto-zone-edit/catasto-zone-edit.component';
import { CatastoClassamentoEditComponent } from './catasto-edit/catasto-classamento-edit/catasto-classamento-edit.component';
import { MetodoProduzioneService } from './catasto-edit/catasto-metodo-produzione-edit/MetodoProduzioneService';
import { MacrousiCatastoService } from './catasto-edit/catasto-macrousi-edit/MacrousiCatasto.service';
import { ClassamentoCatastoService } from './catasto-edit/catasto-classamento-edit/ClassamentoCatasto.service';
import {DatiGeneraliGuard} from '../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../Model/TipiEnumerativi';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';



export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] =  () => import(`./i18n/${lang}.json`);
    return acc;
}, {});


@NgModule({
    declarations: [
        CatastoEditComponent,
        CatastoPossessiEditComponent,
        CatastoMetodoProduzioneEditComponent,
        CatastoMacrousiEditComponent,
        CatastoZoneEditComponent,
        CatastoClassamentoEditComponent
    ],
    exports: [
        CatastoEditComponent,
    ],
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: CatastoEditComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [
                        enum_Security_Attivita.Anagrafica_ParticellaCatastale
                    ],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Edit_Catasto
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
        GiasKendoGridModule,
        GiasUikitModule
    ],
    providers: [
        PossessiService,
        MetodoProduzioneService,
        MacrousiCatastoService,
        ClassamentoCatastoService,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'catasto',
                loader
            }
        }
    ],
    //entryComponents: [CatastoEditComponent]
})
export class CatastoModule { }
