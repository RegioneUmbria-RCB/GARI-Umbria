import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { LabelModule } from "@progress/kendo-angular-label";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { IconsModule } from "@progress/kendo-angular-icons";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { BreadcrumbsService } from "app/Utility/Template/breadcrumbs/breadcrumbs.service";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { GiasUikitModule } from 'gias-ui-kit';
import { GiasKendoGridModule } from "gias-kendo-grid";
import { TerapiaZooRoutingModule } from "./terapia-zoo.routing.module";
import { TerapiaZooComponent } from "./terapia-zoo.component";
import { TRANSLOCO_SCOPE } from "@jsverse/transloco";
import { ProtocolsGridConfigService } from "./protocols-grid/protocols-grid-config.service";
import { ProtocolsGridComponent } from "./protocols-grid/protocols-grid.component";
import { GridModule } from "@progress/kendo-angular-grid";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`../../i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
    imports: [
    TranslocoRootModule,
    ReactiveFormsModule,
    CommonModule,
    HttpClientModule,
    LayoutModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    IconsModule,
    FontAwesomeModule,
    GiasUikitModule,
    TerapiaZooRoutingModule,
    GiasKendoGridModule,
    FormsModule,
    GridModule
],
    declarations: [
        TerapiaZooComponent,
        ProtocolsGridComponent
    ],
    exports: [],
    providers: [
        BreadcrumbsService,
        ProtocolsGridConfigService,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'zoo',
                loader,
                multi: true
            }
        }
    ]
})
export class TerapiaZooModule {}