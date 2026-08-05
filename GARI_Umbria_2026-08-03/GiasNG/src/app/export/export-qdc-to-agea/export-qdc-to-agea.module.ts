import { ExportQdCToAgeaComponent } from "./export-qdc-to-agea.component";
import { UikitModule } from "../../Utility/uikit.module";
import { TranslocoRootModule } from "../../transloco/transloco-root.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgModule } from "@angular/core";
import { RouterModule, RouterOutlet, Routes } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { CommonModule } from "@angular/common";
import { ExportQdcToAgeaFarmFilterComponent } from "./export-qdc-to-agea-farm-filter/export-qdc-to-agea-farm-filter.component";
import { LabelModule } from "@progress/kendo-angular-label";
import { FormFieldModule, InputsModule } from "@progress/kendo-angular-inputs";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { IconsModule } from "@progress/kendo-angular-icons";
import { IntlModule } from "@progress/kendo-angular-intl";
import { ExportQdcToAgeaJsonDialogComponent } from "./export-qdc-to-agea-json-dialog/export-qdc-to-agea-json-dialog.component";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

const routes: Routes = [
  { path: '', component: ExportQdCToAgeaComponent }
];

@NgModule({
  declarations: [
    ExportQdCToAgeaComponent,
    ExportQdcToAgeaFarmFilterComponent,
    ExportQdcToAgeaJsonDialogComponent
  ],
  providers: [
    ReactiveFormsModule,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    UikitModule,
    TranslocoRootModule,
    RouterOutlet,
    RouterModule,
    RouterModule.forChild(routes),
    FontAwesomeModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    FormFieldModule,
    FormsModule,
    IntlModule,
    DateInputsModule,
    IconsModule,
    GiasKendoGridModule,
    GiasUikitModule
  ],
  exports: [ExportQdCToAgeaComponent]
})
export class ExportQdCToAgeaModule { }
