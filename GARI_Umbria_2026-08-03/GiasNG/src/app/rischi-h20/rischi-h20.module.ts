import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { NotificationModule } from '@progress/kendo-angular-notification';

import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

import { RischiH20RoutingModule } from './rischi-h20.routing.module';
import { SelezionePerimetroH20Component } from './pages/selezione-perimetro-h20/selezione-perimetro-h20.component';
import { FiltriPerimetroH20Component } from './components/filtri-perimetro-h20/filtri-perimetro-h20.component';
import { FiltersPerimetroH20Service } from './services/filters-perimetro-h20.service';

@NgModule({
  declarations: [
    SelezionePerimetroH20Component,
    FiltriPerimetroH20Component
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslocoRootModule,
    ButtonsModule,
    LayoutModule,
    IndicatorsModule,
    NotificationModule,
    GiasKendoGridModule,
    GiasUikitModule,
    RischiH20RoutingModule
  ],
  providers: [
    FiltersPerimetroH20Service
  ]
})
export class RischiH20Module {}
