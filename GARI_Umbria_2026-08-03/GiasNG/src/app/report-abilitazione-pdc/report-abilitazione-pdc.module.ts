import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportAbilitazionePdCRoutingModule } from './report-abilitazione-pdc.routing.module';
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReportAbilitazionePdCComponent } from './report-abilitazione-pdc.component';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService, GiasUikitModule, MultiColumnComboboxService } from 'gias-ui-kit';

@NgModule({
  declarations: [
    ReportAbilitazionePdCComponent
  ],
  providers:[
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
    MultiColumnComboboxService
  ],
  exports: [
    ReportAbilitazionePdCComponent
  ],
  imports: [
    CommonModule,
    ReportAbilitazionePdCRoutingModule,
    GiasUikitModule,
    ReactiveFormsModule,
    FormsModule,
    TranslocoRootModule,
    FontAwesomeModule
  ]
})
export class ReportAbilitazionePdCModule {

 }
