import { NgModule } from "@angular/core";
import { MenuVisiteComponent } from "./menu-visite/menu-visite.component";
import { TRANSLOCO_SCOPE } from "@jsverse/transloco";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { VisiteRoutingModule } from "./visite.routing.module";
import { UikitModule } from "../Utility/uikit.module";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { MenuAgendaModule } from "../menu-agenda/menu-agenda.module";
import { VisiteComponent } from "./visite.component";
import { FiltersVisiteComponent } from "./filters-visite/filters-visite.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        MenuVisiteComponent,
        VisiteComponent,
        FiltersVisiteComponent
    ],
    exports: [
        MenuVisiteComponent,
        VisiteComponent,
        FiltersVisiteComponent
    ],
    providers: [
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'visite',
                loader
            }
        }
    ],
    imports: [
        CommonModule,
        VisiteRoutingModule,
        TranslocoRootModule,
        HttpClientModule,
        UikitModule,
        MenuAgendaModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        DateInputsModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})
export class VisiteModule {}