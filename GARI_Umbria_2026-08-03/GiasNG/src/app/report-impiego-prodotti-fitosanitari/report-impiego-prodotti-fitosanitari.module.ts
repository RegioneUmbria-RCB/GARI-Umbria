import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportImpiegoProdottiFitosanitariComponent } from './report-impiego-prodotti-fitosanitari.component';
import { ReportImpiegoProdottiFitosanitariRoutingModule } from './report-impiego-prodotti-fitosanitari.routing.module';
import { UikitModule } from 'app/Utility/uikit.module';
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { ProvinciaComuneMultiselectComponent } from './componenti/provincia-comune-multiselect/provincia-comune-multiselect.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { GiasKendoGridModule} from 'gias-kendo-grid'
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  declarations: [
    ReportImpiegoProdottiFitosanitariComponent,
    ProvinciaComuneMultiselectComponent
  ],
  providers:[ ],
  imports: [
    CommonModule,
    ReportImpiegoProdottiFitosanitariRoutingModule,
    UikitModule,
    ReactiveFormsModule,
    FormsModule,
    TranslocoRootModule,
    FontAwesomeModule,
    GiasKendoGridModule,
    GiasUikitModule
  ]
})
export class ReportImpiegoProdottiFitosanitariModule {

 }
