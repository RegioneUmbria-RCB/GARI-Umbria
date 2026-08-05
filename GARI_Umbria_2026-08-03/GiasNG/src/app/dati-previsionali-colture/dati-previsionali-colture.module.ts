import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {UikitModule} from '../Utility/uikit.module';
import {TranslocoRootModule} from '../transloco/transloco-root.module';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {NavigationModule} from '@progress/kendo-angular-navigation';
import {IconsModule} from '@progress/kendo-angular-icons';
import {ButtonsModule} from '@progress/kendo-angular-buttons';
import {WindowModule} from '@progress/kendo-angular-dialog';
import {LayoutModule} from '@progress/kendo-angular-layout';
import {HttpClientJsonpModule, HttpClientModule} from '@angular/common/http';
import {ColorPickerModule, InputsModule, MaskedTextBoxModule, TextBoxModule} from '@progress/kendo-angular-inputs';
import {LabelModule} from '@progress/kendo-angular-label';
import {DragDropModule} from '@angular/cdk/drag-drop';
import {PagerModule} from '@progress/kendo-angular-pager';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import {InputMaskModule} from '@ngneat/input-mask';
import {MenuAgendaModule} from '../menu-agenda/menu-agenda.module';
import {DateInputsModule} from '@progress/kendo-angular-dateinputs';
import {IndicatorsModule} from '@progress/kendo-angular-indicators';
import {ChartsModule} from '@progress/kendo-angular-charts';
import {PopupModule} from '@progress/kendo-angular-popup';
import {UploadsModule} from '@progress/kendo-angular-upload';
import {DatiPrevisionaliColtureRoutingModule} from './dati-previsionali-colture-routing.module';
import {DatiPrevisionaliColtureComponent} from './dati-previsionali-colture.component';
import { DatiPrevisionailiColtureGridComponent } from './dati-previsionali-colture-grid/dati-previsionaili-colture-grid.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

// export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
//   acc[lang] = () => import(`./i18n/${lang}.json`);
//   return acc;
// }, {});

@NgModule({
  declarations: [
    DatiPrevisionaliColtureComponent,
    DatiPrevisionailiColtureGridComponent
  ],
  providers: [
    ReactiveFormsModule,
    provideNgxMask()
    // {
    //   provide: TRANSLOCO_SCOPE,
    //   useValue: {
    //     scope: 'dati-previsionali-colture',
    //     loader
    //   }
    // },
  ],
  imports: [
    CommonModule,
    DatiPrevisionaliColtureRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    UikitModule,
    TranslocoRootModule,
    FontAwesomeModule,
    NavigationModule,
    IconsModule,
    ButtonsModule,
    WindowModule,
    LayoutModule,
    HttpClientModule,
    HttpClientJsonpModule,
    MaskedTextBoxModule,
    LabelModule,
    ColorPickerModule,
    DragDropModule,
    PagerModule,
    TextBoxModule,
    InputMaskModule,
    MenuAgendaModule,
    DateInputsModule,
    IndicatorsModule,
    ChartsModule,
    PopupModule,
    InputsModule,
    UploadsModule,
    NgxMaskDirective,
    NgxMaskPipe,
    GiasKendoGridModule,
    GiasUikitModule
  ],
  exports: [DatiPrevisionaliColtureComponent]

})
export class DatiPrevisionaliColtureModule { }
