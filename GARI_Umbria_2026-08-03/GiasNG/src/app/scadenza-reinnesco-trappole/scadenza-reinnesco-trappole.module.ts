import { NgModule } from "@angular/core";
import { TranslocoRootModule } from "../transloco/transloco-root.module";
import { ReactiveFormsModule } from "@angular/forms";
import { CommonModule, DatePipe, DecimalPipe } from "@angular/common";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { IconsModule } from "@progress/kendo-angular-icons";
import { LabelModule } from "@progress/kendo-angular-label";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { UikitModule } from "../Utility/uikit.module";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import {GridTrappoleComponent} from "./grid-trappole/grid-trappole.component";
import {RouterModule, RouterOutlet, Routes} from "@angular/router";
import {ScadenzaReinnescoTrappoleComponent} from "./scadenza-reinnesco-trappole.component";
import {FiltriTrappoleComponent} from "./grid-trappole/filtri/filtri-trappole.component";
import {ScadenzaReinnescoTrappoleService} from "./scadenza-reinnesco-trappole.service";
import {TRANSLOCO_SCOPE} from "@jsverse/transloco";
import {LOCALIZATION_LANGUAGES} from "../Model/CostantiPersonalizzate";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

const routes: Routes = [
  { path: '', component: ScadenzaReinnescoTrappoleComponent }
];

@NgModule({
  imports: [
    TranslocoRootModule,
    UikitModule,
    ReactiveFormsModule,
    CommonModule,
    FontAwesomeModule,
    LayoutModule,
    IconsModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    GiasKendoGridModule,
    GiasUikitModule,
    RouterOutlet,
    RouterModule,
    RouterModule.forChild(routes)
  ],
  declarations: [
    ScadenzaReinnescoTrappoleComponent,
    GridTrappoleComponent,
    FiltriTrappoleComponent
  ],
  exports: [
    ScadenzaReinnescoTrappoleComponent
  ],
  providers: [
      {
        provide: TRANSLOCO_SCOPE,
        useValue: {
          scope: 'trappole',
          loader,
          multi: true
        }
      },
      DatePipe,
      DecimalPipe,
      { provide: LOADING_TOKEN, useClass: LoadingService },
      ScadenzaReinnescoTrappoleService
  ]
})
export class ScadenzaReinnescoTrappoleModule {
}
