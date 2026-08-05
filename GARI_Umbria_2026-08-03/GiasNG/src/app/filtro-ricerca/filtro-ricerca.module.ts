import { NgModule } from "@angular/core";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { UikitModule } from "app/Utility/uikit.module";
import { ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { LabelModule } from "@progress/kendo-angular-label";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { FiltroRicercaComponent } from "./filtro-ricerca.component";
import { FiltroRicercaRoutingModule } from "./filtro-ricerca.routing.module";
import { GrigliaFiltroRicercaComponent } from "./griglia-filtro-ricerca/griglia-filtro-ricerca.component";
import { GeoFiltersComponent } from "./componenti/geo-filters/geo-filters.component";
import { CodiciComponent } from "./componenti/codici/codici.component";
import { IconsModule } from "@progress/kendo-angular-icons";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { ClearButtonComponent } from "./componenti/clearButton/clear-button.component";
import { RedSpotComponent } from "./componenti/red-spot/red-spot.component";
import { FiltriTemporaliComponent } from "./componenti/filtri-temporali/filtri-temporali.component";
import { PendingChangesGuard } from "./griglia-filtro-ricerca/service/pendingChanges-guard.service";
import { DialogWindowService } from "./griglia-filtro-ricerca/service/dialog-window.service";
import { TreeAziende } from "./componenti/tree-aziende/tree-aziende.component";
import { TreeAziendeService } from "./service/tree-aziende.service";
import { TreeAziendeFilters } from "./componenti/tree-aziende-filters/tree-aziende-filters.component";
import { PrintExportComponent } from "./componenti/print-export-button/print-export-button.component";
import { StampeFiltroRicercaService } from "./griglia-filtro-ricerca/service/stampe-filtro-ricerca.service";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';


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
        FiltroRicercaRoutingModule,
        IconsModule,
        FontAwesomeModule,
        GiasKendoGridModule,
        GiasUikitModule
    ],
    declarations: [
        FiltroRicercaComponent,
        GrigliaFiltroRicercaComponent,
        GeoFiltersComponent,
        CodiciComponent,
        ClearButtonComponent,
        RedSpotComponent,
        FiltriTemporaliComponent,
        TreeAziende,
        TreeAziendeFilters,
        PrintExportComponent
    ],
    exports: [],
    providers: [
        PendingChangesGuard,
        DialogWindowService,
        TreeAziendeService,
        StampeFiltroRicercaService
    ]
})
export class FiltroRicercaModule {
}