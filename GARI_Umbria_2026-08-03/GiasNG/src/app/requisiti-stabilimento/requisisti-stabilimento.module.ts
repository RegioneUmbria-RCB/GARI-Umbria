import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequisistiStabilimentoRoutingModule } from './requisisti-stabilimento-routing.module';
import {RequisitiStabilimentoComponent} from './requisiti-stabilimento.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TRANSLOCO_SCOPE} from '@jsverse/transloco';
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
import {FiltriRequisitiStabilimentoComponent} from './filtri/filtri.component';
import {RequisitiStabilimentoGridComponent} from './requisiti-stabilimento-grid/requisiti-stabilimento-grid.component';
import {LOCALIZATION_LANGUAGES} from '../Model/CostantiPersonalizzate';
import { RequisitiStabilimentoContrattiGridComponent } from './requisiti-stabilimento-contratti-grid/requisiti-stabilimento-contratti-grid.component';
import { VisualizzaDettagliComponent } from "./visualizza-dettagli/visualizza-dettagli.component";
import {VDPianoColturaleComponent} from "./visualizza-dettagli/v-d-piano-colturale/v-d-piano-colturale.component";
import {VDContrattiComponent} from "./visualizza-dettagli/v-d-contratti/v-d-contratti.component";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    RequisitiStabilimentoComponent,
    FiltriRequisitiStabilimentoComponent,
    RequisitiStabilimentoGridComponent,
    RequisitiStabilimentoContrattiGridComponent,
    VisualizzaDettagliComponent,
    VDPianoColturaleComponent,
    VDContrattiComponent
  ],
  providers: [
    ReactiveFormsModule,
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'RdS',
        loader
      }
    },
    provideNgxMask()
  ],
  imports: [
    CommonModule,
    RequisistiStabilimentoRoutingModule,
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
  exports: [
    RequisitiStabilimentoComponent,
    RequisitiStabilimentoGridComponent,
    VisualizzaDettagliComponent,
    VDPianoColturaleComponent,
    VDContrattiComponent
  ]
})
export class RequisistiStabilimentoModule { }
