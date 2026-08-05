import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {DomandaIrriguaRoutingModule} from "./domanda-irrigua-routing.module";
import {ButtonsModule} from "@progress/kendo-angular-buttons";
import {InputsModule} from "@progress/kendo-angular-inputs";
import {DomandaIrriguaComponent} from "./domanda-irrigua.component";
import { LettureContatoriComponent } from './pages/letture-contatori/letture-contatori.component';
import { LettureContatoriAziendaliGridComponent } from './components/letture-contatori-aziendali-grid/letture-contatori-aziendali-grid.component';
import {UikitModule} from "../Utility/uikit.module";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import {ReactiveFormsModule} from "@angular/forms";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
  declarations: [
    DomandaIrriguaComponent,
    LettureContatoriComponent,
    LettureContatoriAziendaliGridComponent
  ],
  providers: [ ],
  imports: [
    DomandaIrriguaRoutingModule,
    CommonModule,
    ButtonsModule,
    InputsModule,
    UikitModule,
    TranslocoRootModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    GiasKendoGridModule,
    GiasUikitModule
  ],
  exports: []

})
export class DomandaIrriguaModule { }
