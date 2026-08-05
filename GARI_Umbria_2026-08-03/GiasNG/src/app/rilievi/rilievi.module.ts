import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { RilieviRoutingModule } from "./rilievi.routing.module";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { HttpClientModule } from "@angular/common/http";
import { UikitModule } from "app/Utility/uikit.module";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { MenuRilieviComponent } from "./rilievi.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FiltersRilieviComponent } from "./filters-rilievi/filters-rilievi.component";
import { FiltersRilieviService } from "./filters-rilievi/service/filters-rilievi.service";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
    declarations: [
        MenuRilieviComponent,
        FiltersRilieviComponent
    ],
    exports: [
    ],
    providers: [
        FiltersRilieviService
    ],
    imports: [
        CommonModule,
        RilieviRoutingModule,
        TranslocoRootModule,
        HttpClientModule,
        UikitModule,
        // MenuAgendaModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        // DateInputsModule
        GiasKendoGridModule,
        GiasUikitModule
    ]
})
export class RilieviModule {}