import { NgModule } from "@angular/core";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { AgendaFiltersComponent } from "./components/filters/filters.component";
import { MenuAgendaComponent } from "./menu-agenda.component";
import { ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { CommonModule, DatePipe, DecimalPipe } from "@angular/common";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { IconsModule } from "@progress/kendo-angular-icons";
import { LabelModule } from "@progress/kendo-angular-label";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { QdCLoadedGuard } from "./guards/transloco-qdc-guard.service";
import { TRANSLOCO_SCOPE } from "@jsverse/transloco";
import { GisService } from "app/GIS/GIS.service";
import { GoogleMapGeoJsonService } from "app/GIS/google-map/google-map-geojson.service";
import { GoogleMapService } from "app/GIS/google-map/google-map.service";
import { DataLayerStyleService } from "app/GIS/services/data-layer-style.service";
import { FeatureService } from "app/GIS/services/feature.service";
import { LayerStyleService } from "app/GIS/services/layer-style.service";
import { PolygonLabelService } from "app/GIS/services/polygon-label.service";
import { WKTService } from "app/GIS/services/wkt.service";
import { WmsService } from "app/GIS/services/wms.service";
import { TreeGisService } from "app/Utility/Template/kendo-tree/services/tree-gis.service";
import { BrogliaccioGridComponent } from "./components/grid-brogliaccio/brogliaccio.component";
import { RicetteComponent } from "./components/grid-ricette/ricette.component";
import { AllOperationsTableComponent } from "./components/grid-qdc/all-op-table.component";
import { OperationsListComponent } from "./components/operations-list/operations-list.component";
import { UikitModule } from "app/Utility/uikit.module";
import { LOCALIZATION_LANGUAGES } from "app/Model/CostantiPersonalizzate";
import { OperationsMassEditComponent } from './components/operations-mass-edit/operations-mass-edit.component';
import { GridAddMacchineComponent } from './components/operations-mass-edit/grid-add-macchine/grid-add-macchine.component';
import { GridAddOperaiComponent } from './components/operations-mass-edit/grid-add-operai/grid-add-operai.component';
import { RicetteService } from "./components/grid-ricette/ricette.service";
import { GoogleMapDataService } from '../GIS/services/google.maps-services/google-map-data.service';
import { RetinaturaService } from '../GIS/services/retinatura.service';
import { PolygonLabelInfowindowService } from "app/GIS/services/polygon-label-infowindow.service";
import { InfowindowClustererService } from '../GIS/infowindow-clusterer/infowindow-clusterer.service';
import { GoogleMapGeoJsonLazyService } from "app/GIS/services/google.maps-services/google-map-geojson-lazy.service";
import { GoogleMapFeatureService } from "app/GIS/services/google.maps-services/google-map-feature.service";
import { FeatureInformationService } from "app/GIS/services/feature-information.service";
import { EditFeatureWindowService } from "app/GIS/services/edit-feature-window.service";
import { EditFeatureService } from "app/GIS/services/edit-feature.service";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import {ExportCartographyDataService} from '../GIS/GIS-toolbar/services/export-cartography-data.service';
import {RicetteSmartTractorService} from './components/grid-ricette/ricette-smart-tractor.service';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    imports: [
        TranslocoRootModule,
        UikitModule,
        ReactiveFormsModule,
        CommonModule,
        HttpClientModule,
        FontAwesomeModule,
        LayoutModule,
        IconsModule,
        LabelModule,
        InputsModule,
        ButtonsModule,
        GiasKendoGridModule,
        GiasUikitModule
    ],
    declarations: [
        MenuAgendaComponent,
        AgendaFiltersComponent,
        BrogliaccioGridComponent,
        RicetteComponent,
        AllOperationsTableComponent,
        OperationsListComponent,
        OperationsMassEditComponent,
        GridAddMacchineComponent,
        GridAddOperaiComponent
    ],
    exports: [
        MenuAgendaComponent,
        AgendaFiltersComponent
    ],
    providers: [
        QdCLoadedGuard,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'qdc',
                loader,
                multi: true
            }
        },
        DatePipe,
        DecimalPipe,
        GisService,
        GoogleMapGeoJsonService,
        GoogleMapGeoJsonLazyService,
        GoogleMapService,
        GoogleMapDataService,
        RetinaturaService,
        DataLayerStyleService,
        LayerStyleService,
        PolygonLabelService,
        FeatureService,
        TreeGisService,
        ExportCartographyDataService,
        WKTService,
        WmsService,
        RicetteService,
      RicetteSmartTractorService,
        PolygonLabelInfowindowService,
        InfowindowClustererService,
        GoogleMapFeatureService,
        FeatureInformationService,
        EditFeatureWindowService,
        EditFeatureService,
        { provide: LOADING_TOKEN, useClass: LoadingService },
    ]
})
export class MenuAgendaModule {
}
