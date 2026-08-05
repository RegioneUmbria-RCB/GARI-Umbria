import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GruppiRaccoltaRoutingModule } from './gruppi-raccolta-routing.module';
import { GruppiRaccoltaComponent } from './gruppi-raccolta.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { UikitModule } from '../../Utility/uikit.module';
import { TranslocoRootModule } from '../../transloco/transloco-root.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { ColorPickerModule, InputsModule, MaskedTextBoxModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { PagerModule } from '@progress/kendo-angular-pager';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { InputMaskModule } from '@ngneat/input-mask';
import { MenuAgendaModule } from '../../menu-agenda/menu-agenda.module';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { ChartsModule } from '@progress/kendo-angular-charts';
import { PopupModule } from '@progress/kendo-angular-popup';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  declarations: [
    GruppiRaccoltaComponent
  ],
  providers: [
    ReactiveFormsModule,
    provideNgxMask()
  ],
  imports: [
    CommonModule,
    GruppiRaccoltaRoutingModule,
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
    GiasUikitModule
  ],
  exports: [
    GruppiRaccoltaComponent
  ]
})
export class GruppiRaccoltaModule { }
