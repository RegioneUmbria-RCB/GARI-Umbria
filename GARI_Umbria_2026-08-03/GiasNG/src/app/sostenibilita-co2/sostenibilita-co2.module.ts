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

import { CalcoloSostenibilitaCO2RoutingModule } from './calcolo-sostenibilita-co2-routing.module';
import { CreazioneTokenCo2RoutingModule } from './creazione-token-co2-routing.module';
import { SelezionePerimetroComponent } from './pages/selezione-perimetro-co2/selezione-perimetro-co2.component';
import { FiltriPerimetroComponent } from './components/filtri-perimetro-co2/filtri-perimetro-co2.component';
import { GestioneCO2PageComponent } from './pages/gestione-co2/gestione-co2.component';
import { CarburantiGridComponent } from './components/carburanti-grid/carburanti-grid.component';
import { EnergiaGridComponent } from './components/energia-grid/energia-grid.component';
import { TokenCreationCo2Component } from './pages/creazione-token-co2/token-creation-co2.component';
import { FiltriTokenCreationCo2Component } from './components/filtri-token-creation-co2/filtri-token-creation-co2.component';
import { FiltersPerimetroService } from './services/filters-perimetro.service';
import { FiltersTokenCreationCo2Service } from './services/filters-token-creation-co2.service';

@NgModule({
  declarations: [
    SelezionePerimetroComponent,
    FiltriPerimetroComponent,
    GestioneCO2PageComponent,
    CarburantiGridComponent,
    EnergiaGridComponent,
    TokenCreationCo2Component,
    FiltriTokenCreationCo2Component
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
    CalcoloSostenibilitaCO2RoutingModule,
    CreazioneTokenCo2RoutingModule
  ],
  providers: [
    FiltersPerimetroService,
    FiltersTokenCreationCo2Service
  ]
})
export class SostenibitaCO2Module {}
