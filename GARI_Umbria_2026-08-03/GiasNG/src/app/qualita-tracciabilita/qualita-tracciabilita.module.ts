import { NgModule } from "@angular/core";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { IconsModule } from "@progress/kendo-angular-icons";
import { LabelModule } from "@progress/kendo-angular-label";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { UikitModule } from "app/Utility/uikit.module";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { DatePickerModule } from "@progress/kendo-angular-dateinputs";
import { TRANSLOCO_SCOPE } from "@jsverse/transloco";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { QualitaTracciabilitaRoutingModule } from "./qualita-tracciabilita-routing.module";
import { KENDO_DROPDOWNS } from "@progress/kendo-angular-dropdowns";
import { KENDO_LABELS } from "@progress/kendo-angular-label";
import { KENDO_INPUTS } from "@progress/kendo-angular-inputs";
import { KENDO_BUTTONS } from "@progress/kendo-angular-buttons";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

export const SHARED_IMPORTS = [
  GiasUikitModule,
  GiasKendoGridModule,
  TranslocoRootModule,
  KENDO_BUTTONS,
  KENDO_DROPDOWNS,
  KENDO_LABELS,
  KENDO_INPUTS,
  ReactiveFormsModule,
  FormsModule,
  CommonModule,
  FontAwesomeModule,
  LayoutModule,
  IconsModule,
  LabelModule,
  InputsModule,
  ButtonsModule,
  DatePickerModule,
];

@NgModule({
  imports: [
    TranslocoRootModule,
    UikitModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    FontAwesomeModule,
    LayoutModule,
    IconsModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    DatePickerModule,
    GiasKendoGridModule,
    GiasUikitModule,
    QualitaTracciabilitaRoutingModule
  ],
  declarations: [],
  exports: [],
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'qet',
        loader,
        multi: true
      }
    },
    { provide: LOADING_TOKEN, useClass: LoadingService }
  ]
})
export class QualitaTracciabilitaModule {
}
