import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { NotificationModule } from '@progress/kendo-angular-notification';

import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

import { RischiMeteoRoutingModule } from './rischi-meteo-routing.module';
import { CalcoloRischiPageComponent } from './pages/calcolo-rischi/calcolo-rischi-page.component';
import { FiltriCalcoloRischiComponent } from './components/filtri-calcolo-rischi/filtri-calcolo-rischi.component';

@NgModule({
  declarations: [
    CalcoloRischiPageComponent,
    FiltriCalcoloRischiComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslocoRootModule,
    ButtonsModule,
    LayoutModule,
    IndicatorsModule,
    NotificationModule,
    GiasKendoGridModule,
    GiasUikitModule,
    RischiMeteoRoutingModule
  ]
})
export class RischiMeteoModule {}
