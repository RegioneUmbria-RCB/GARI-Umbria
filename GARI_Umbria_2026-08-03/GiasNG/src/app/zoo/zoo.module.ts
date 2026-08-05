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
import { ZooComponent } from "./zoo.component";
import { ZooRoutingModule } from "./zoo-routing.module";
import { ZooOperationsGridComponent } from './components/zoo-operations-grid/zoo-operations-grid.component';
import { MenuAgendaModule } from "../menu-agenda/menu-agenda.module";
import { DatePickerModule } from "@progress/kendo-angular-dateinputs";
import { MenuZooComponent } from './pages/menu-zoo/menu-zoo.component';
import { ZooFavoritesEditingComponent } from './components/zoo-favorites-editing/zoo-favorites-editing.component';
import {TRANSLOCO_SCOPE} from "@jsverse/transloco";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import {ProtocolliComponent} from "./pages/protocolli/protocolli.component";
import {IndicazioniComponent} from "./pages/indicazioni/indicazioni.component";
import { ProtocolliInCorsoComponent } from "./pages/protocolli-in-corso/protocolli-in-corso.component";
import { TerapieComponent } from "./pages/terapie/terapie.component";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

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
    ZooRoutingModule,
    MenuAgendaModule,
    DatePickerModule,
    GiasKendoGridModule,
    GiasUikitModule,
    ProtocolliComponent,
    ProtocolliInCorsoComponent,
    IndicazioniComponent,
    TerapieComponent,
  ],
  declarations: [
    ZooComponent,
    ZooOperationsGridComponent,
    MenuZooComponent,
    ZooFavoritesEditingComponent
  ],
  exports: [
    ZooComponent
  ],
  providers: [
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'zoo',
        loader,
        multi: true
      }
    },
    {provide: LOADING_TOKEN, useClass: LoadingService}
  ]
})
export class ZooModule {
}
