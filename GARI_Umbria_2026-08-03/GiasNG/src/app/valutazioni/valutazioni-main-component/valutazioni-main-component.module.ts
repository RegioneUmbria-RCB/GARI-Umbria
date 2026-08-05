import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { ValutazioniMainComponentComponent } from './valutazioni-main-component.component';
import { ValutazioniModule } from '../valutazioni/valutazioni.module';
import { PianoContiModule } from '../piano-conti/piano-conti.module';
import { RouterModule } from '@angular/router';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  declarations: [
    ValutazioniMainComponentComponent
  ],
  providers: [
    ReactiveFormsModule,
    provideNgxMask()
  ],
  imports: [
    CommonModule,
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
    ValutazioniModule,
    PianoContiModule,
    RouterModule,
    NgxMaskDirective,
    NgxMaskPipe,
    GiasUikitModule
  ],
  exports: [
    ValutazioniMainComponentComponent
  ]
})
export class ValutazioniMainComponentModule { }
