import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService, GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { DssNutrizioneRoutingModule } from './dss-nutrizione-routing.module';
import { DssNutrizioneService } from './services/dss-nutrizione.service';
import { DssNutrizionePageComponent } from './pages/dss-nutrizione-page/dss-nutrizione-page.component';
import { DssNutrizioneWidgetCardComponent } from './components/dss-nutrizione-widget-card/dss-nutrizione-widget-card.component';
import { UikitModule } from 'app/Utility/uikit.module';
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";

@NgModule({
  declarations: [
    DssNutrizionePageComponent,
    DssNutrizioneWidgetCardComponent
  ],
  imports: [
    FontAwesomeModule,
    FormsModule,
    UikitModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LayoutModule,
    IndicatorsModule,
    GiasUikitModule,
    TranslocoRootModule,
    DssNutrizioneRoutingModule
  ],
  providers: [
    DssNutrizioneService,
    { provide: LOADING_TOKEN, useClass: LoadingService }
  ],
})
export class DssNutrizioneModule { }
