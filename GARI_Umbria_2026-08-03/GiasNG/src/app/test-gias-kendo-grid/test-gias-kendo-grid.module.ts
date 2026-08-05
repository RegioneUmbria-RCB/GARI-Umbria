import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IconsModule } from '@progress/kendo-angular-icons';
import { TestGiasGiasKendoGridComponent } from './test-gias-kendo-grid.component';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { TestGiasKendoGridRoutingModule } from './test-gias-kendo-grid-routing.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { GiasKendoGridModule } from 'gias-kendo-grid';

@NgModule({
  declarations: [
    TestGiasGiasKendoGridComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    IconsModule,
    ButtonsModule,
    InputsModule,
    LabelModule,
    LayoutModule,
    FontAwesomeModule,
    TestGiasKendoGridRoutingModule,
    GiasKendoGridModule
  ],
  providers: [
  ]
})
export class TestGiasKendoGridModule {
}
