import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UikitModule } from '../../../Utility/uikit.module';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { TranslocoRootModule } from '../../../transloco/transloco-root.module';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { LOCALIZATION_LANGUAGES } from '../../../Model/CostantiPersonalizzate';
import { MacchineEditComponent, UploadInterceptor } from './macchine-edit.component';
import { CostiMacchinaEditComponent } from './costi-macchina-edit/costi-macchina-edit.component';
import { MacchineEditImageComponent } from './macchine-edit-image/macchine-edit-image.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { MacchinaEditCaratteristicheComponent } from './macchine-edit-caratteristiche/macchine-edit-caratteristiche.component';
import { MacchineEditGerarchiaComponent } from './macchine-edit-gerarchia/macchine-edit-gerarchia.component';
import {DatiGeneraliGuard} from '../../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../../Model/TipiEnumerativi';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import { MacchineEditTurniIrriguaComponent } from './macchine-edit-turni-irrigua/macchine-edit-turni-irrigua.component';
import { RateiTempoService } from './macchine-edit-turni-irrigua/service/turni-irrigua.service';
import { CustomDateTimePickerComponent } from './macchine-edit-turni-irrigua/components/custom-datetimepicker.component';
import { DatePickerComponent, TimePickerComponent, DateTimePickerComponent } from "@progress/kendo-angular-dateinputs";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});


@NgModule({
    declarations: [MacchineEditComponent, 
                   CostiMacchinaEditComponent, 
                   MacchineEditImageComponent, 
                   MacchinaEditCaratteristicheComponent, 
                   MacchineEditGerarchiaComponent,
                   MacchineEditTurniIrriguaComponent,
                   CustomDateTimePickerComponent],
    exports: [MacchineEditComponent, CostiMacchinaEditComponent],
    imports: [
    RouterModule.forChild([
        {
            path: '',
            component: MacchineEditComponent,
            canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
            data: {
                readPermissions: [
                    enum_Security_Attivita.Anagrafica_ParcoMacchine
                ],
                writePermissions: [],
                page: enum_PagineGiasNG.Pagina_Edit_Macchina
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
    HttpClientModule,
    UploadsModule,
    InputsModule,
    ButtonsModule,
    GiasKendoGridModule,
    GiasUikitModule,
    DatePickerComponent,
    TimePickerComponent,
    DateTimePickerComponent
],
    providers: [
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'macchina',
                loader
            }
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UploadInterceptor,
            multi: true,
        },
        RateiTempoService
    ],
    //entryComponents: [MacchineEditComponent]
})
export class MacchinaModule {
}
