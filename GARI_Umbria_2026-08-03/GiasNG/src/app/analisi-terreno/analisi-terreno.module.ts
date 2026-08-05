import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { LabelModule } from "@progress/kendo-angular-label";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { UikitModule } from "app/Utility/uikit.module";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { AnalisiTerrenoRoutingModule } from "./analisi-terreno.routing.module";
import { IconsModule } from "@progress/kendo-angular-icons";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { AnalisiTerrenoComponent } from "./analisi-terreno.component";
import { GrigliaAnalisiTerrenoComponent } from "./griglia-analisi-terreno/griglia-analisi-terreno.component";
import { AnalisiDocumentsService } from "./griglia-analisi-terreno/service/analisi-documents.service";
import { BreadcrumbsService } from "app/Utility/Template/breadcrumbs/breadcrumbs.service";
import { AnalisiTerrenoEditComponent } from "./analisi-terreno-edit/analisi-terreno-edit.component";
import { AnalisiTerrenoFormService } from "./griglia-analisi-terreno/service/analisi-terreno-form.service";
import { ParametriAnalisiTerrenoComponent } from "./analisi-terreno-edit/parametri-analisi/parametri-analisi-terreno.component";
import { GISModule } from "app/GIS/GIS.module";
import { GrigliaEntitaComponent } from "./analisi-terreno-edit/griglia-entita/griglia-entita.component";
import { EntitaConfigService } from "./analisi-terreno-edit/griglia-entita/service/entita-config.service";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GisService } from "app/GIS/GIS.service";
import { GisToolbarService } from "app/GIS/GIS-toolbar/gis-toolbar.service";
import { MeasureDistanceService } from "app/GIS/services/measure-distance.service";
import { PositionService } from "app/GIS/services/position.service";
import { GiasUikitModule } from 'gias-ui-kit';
import {ExportCartographyDataService} from '../GIS/GIS-toolbar/services/export-cartography-data.service';

@NgModule({
    imports: [
        TranslocoRootModule,
        UikitModule,
        ReactiveFormsModule,
        CommonModule,
        HttpClientModule,
        LayoutModule,
        LabelModule,
        InputsModule,
        ButtonsModule,
        AnalisiTerrenoRoutingModule,
        IconsModule,
        GISModule,
        FontAwesomeModule,
        GiasKendoGridModule,
        GiasUikitModule
    ],
    declarations: [
        AnalisiTerrenoComponent,
        AnalisiTerrenoEditComponent,
        GrigliaAnalisiTerrenoComponent,
        ParametriAnalisiTerrenoComponent,
        GrigliaEntitaComponent
    ],
    exports: [],
    providers: [
        AnalisiDocumentsService,
        BreadcrumbsService,
        EntitaConfigService,
        AnalisiTerrenoFormService,
        GisService,
        GisToolbarService,
        ExportCartographyDataService,
        MeasureDistanceService,
        PositionService
    ]
})
export class AnalisiTerrenoModule {}
