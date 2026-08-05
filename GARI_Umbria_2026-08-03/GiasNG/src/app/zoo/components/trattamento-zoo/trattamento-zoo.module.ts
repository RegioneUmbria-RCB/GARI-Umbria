import { NgModule } from "@angular/core";
import { GiasUikitModule } from 'gias-ui-kit';
import { CommonModule } from "@angular/common";
import { TRANSLOCO_SCOPE } from "@jsverse/transloco";
import { GiasKendoGridModule } from "gias-kendo-grid";
import { HttpClientModule } from "@angular/common/http";
import { IconsModule } from "@progress/kendo-angular-icons";
import { LabelModule } from "@progress/kendo-angular-label";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { TrattamentoZooComponent } from "./trattamento-zoo.component";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { TrattamentoZooRoutingModule } from "./trattamento-zoo.routing.module";
import { BreadcrumbsService } from "app/Utility/Template/breadcrumbs/breadcrumbs.service";
import { DialogQtaDiffAICsComponent } from "./dialog-qta-diff-aics/dialog-qta-diff-aics.component";
import { DialogGridProdsSommComponent } from "./dialog-qta-diff-aics/grid/dialog-grid-prods-somm.component";
import { GridCapiAnimaliComponent } from "./griglie-trattamento/griglia-capi-animali/grid-capi-animali.component";
import { CapiAnimaliConfigService } from "./griglie-trattamento/griglia-capi-animali/service/capi-animali-config.service";
import { GridProdottiSomministrazioneComponent } from "./griglie-trattamento/griglia-prodotti-somministrazione/grid-prodotti-somministrazione.component";
import { ProdottoSomministrazioneConfigService } from "./griglie-trattamento/griglia-prodotti-somministrazione/service/prodotto-somministrazione-config.service";
import { DialogGridProdsSommConfigService } from "./dialog-qta-diff-aics/grid/dialog-grid-prods-somm-config.service";

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`../../i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
    imports: [
        TranslocoRootModule,
        // UikitModule,
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
        TrattamentoZooRoutingModule,
        GiasKendoGridModule,
        FormsModule
    ],
    declarations: [
        GridProdottiSomministrazioneComponent,
        GridCapiAnimaliComponent,
        TrattamentoZooComponent,
        DialogQtaDiffAICsComponent,
        DialogGridProdsSommComponent
    ],
    exports: [],
    providers: [
        BreadcrumbsService,
        CapiAnimaliConfigService,
        ProdottoSomministrazioneConfigService,
        DialogGridProdsSommConfigService,
        {
              provide: TRANSLOCO_SCOPE,
              useValue: {
                scope: 'zoo',
                loader,
                multi: true
              }
        },
    ]
})
export class TrattamentoZooModule {}