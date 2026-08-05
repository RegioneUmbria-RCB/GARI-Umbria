import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ConfrontoPianoColturaleComponent} from './confronto-piano-colturale.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {UikitModule} from '../../Utility/uikit.module';
import {TranslocoRootModule} from '../../transloco/transloco-root.module';
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
import {DateInputsModule} from '@progress/kendo-angular-dateinputs';
import {IndicatorsModule} from '@progress/kendo-angular-indicators';
import {ChartsModule} from '@progress/kendo-angular-charts';
import {PopupModule} from '@progress/kendo-angular-popup';
import {UploadsModule} from '@progress/kendo-angular-upload';
import {ConfrontoPianoColturaleRoutingModule} from './confronto-piano-colturale-routing.module';
import {ConfrontoPcCatastoComponent} from './confronto-pc-catasto/confronto-pc-catasto.component';
import { PianoColturaleGridComponent } from './piano-colturale-grid/piano-colturale-grid.component';
import { ConfrontoPcCatastoGridComponent } from './confronto-pc-catasto/confronto-pc-catasto-grid/confronto-pc-catasto-grid.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  declarations: [
    ConfrontoPianoColturaleComponent,
    ConfrontoPcCatastoComponent,
    PianoColturaleGridComponent,
    ConfrontoPcCatastoGridComponent,
  ],
  providers:[
    ReactiveFormsModule,
    provideNgxMask()
  ],
  imports: [
    ConfrontoPianoColturaleRoutingModule,
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
  exports: [
    ConfrontoPianoColturaleComponent
  ]
})
export class ConfrontoPianoColturaleModule { }
