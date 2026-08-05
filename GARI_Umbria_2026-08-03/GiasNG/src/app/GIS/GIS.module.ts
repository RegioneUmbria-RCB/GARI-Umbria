import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';

import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { GISRoutingModule } from './GIS-routing.module';
import { GISComponent } from './GIS.component';

import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { GoogleMapsModule } from '@angular/google-maps';
import { GoogleMapComponent } from './google-map/google-map.component';
import { PolygonWindowComponent } from './GIS-kendo-window/polygon-window/polygon-window.component';
import { DrawWindowComponent } from './GIS-kendo-window/draw-window/draw-window.component';
import { UikitModule } from 'app/Utility/uikit.module';
import { ThemeWindowComponent } from './GIS-kendo-window/theme-window/theme-window.component';
import { WMSWindowComponent } from './GIS-kendo-window/wms-window/wms-window.component';
import { GISConfigurationModalComponent } from './GIS-configuration-modal/gis-configuration-modal.component';
import { GISCalendarModalComponent } from './GIS-calendar-modal/gis-calendar-modal.component';
import { GISWindowComponent } from './GIS-window/GIS-window.component';
import { ColorPickerModule, InputsModule, MaskedTextBoxModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { GisToolbarComponent } from './GIS-toolbar/gis-toolbar.component';
import { GISLayerListItemMappeSatellitariComponent } from './GIS-layer-window/GIS-layer-list-item-mappe-satellitari/GIS-layer-list-item-mappe-satellitari.component';
import { GISLayerWindowComponent } from './GIS-layer-window/GIS-layer-window.component';
import { GISLayerColorPickerWindowComponent } from './GIS-layer-color-picker-window/GIS-layer-color-picker-window.component';
import { GISFixedLayerPropertyWindowComponent } from './GIS-fixed-layer-property-window/GIS-fixed-layer-property-window.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { PagerModule } from '@progress/kendo-angular-pager';
import { TreeWindowComponent } from './GIS-kendo-window/tree-window/tree-window.component';
import { GisCheckboxComponent } from './GIS-configuration-modal/GIS-checkbox/gis-checkbox.component';
import { GISLayerVisibilityConfigurationWindowComponent } from './GIS-layer-visibility-configuration-window/GIS-layer-visibility-configuration-window.component';
import { InputMaskModule } from '@ngneat/input-mask';
import { GISDeleteFeatureWindowComponent } from './GIS-delete-feature-window/GIS-delete-feature-window.component';
import { GisBottomWindow } from './GIS-bottom-window/gis-bottom-window.component';
import { GisToolbarMobileComponent } from './GIS-toolbar/gis-toolbar-mobile/gis-toolbar-mobile.component';
import { GisLoadedGuard } from './guard/GisLoadedGuard';
import { MenuAgendaModule } from 'app/menu-agenda/menu-agenda.module';
import { LayerService } from './services/layer.service';
import { GISAttributiComponent } from './GIS-attributi/GIS-attributi.component';
import { MarkerWindowComponent } from './GIS-kendo-window/marker-window/marker-window.component';
import { ToolScomponiPuntiComponent } from './GIS-kendo-window/marker-window/tool-scomponi-punti/tool-scomponi-punti.component';
import { ToolDisegnaPuntiComponent } from './GIS-kendo-window/marker-window/tool-disegna-punti/tool-disegna-punti.component';
import { GISLayerPermissionsWindowComponent } from './GIS-layer-permissions-window/GIS-layer-permissions-window.component';
import { GISLayerUsersPermissionsComponent } from './GIS-layer-permissions-window/GIS-layer-users-permissions/GIS-layer-users-permissions.component';
import { GISLayerGroupsPermissionsComponent } from './GIS-layer-permissions-window/GIS-layer-groups-permissions/GIS-layer-groups-permissions.component';
import { GISTooltipDirective } from './services/gis-tooltip.directive';
import { GISLayerAdvancedSettingsWindowComponent } from './GIS-layer-advanced-settings-window/gis-layer-advanced-settings-window.component';
import { LineeGuidaABWindowComponent } from './GIS-kendo-window/linee-guida-ab-window/linee-guida-ab-window.component';
import { GISGestionePianoRateoComponent } from './GIS-kendo-window/GIS-gestione-piano-rateo/GIS-gestione-piano-rateo.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { GISAnalisiMappeSatellitariWindowComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.component';
import { GISAnalisiMappeSatellitariNotificheComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-notifiche/GIS-analisi-mappe-satellitari-notifiche.component';
import { GISAnalisiMappeSatellitariCalendarioComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-calendario/GIS-analisi-mappe-satellitari-calendario.component';
import { GISAnalisiMappeSatellitariGraficiComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-grafici/GIS-analisi-mappe-satellitari-grafici.component';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { ChartsModule } from '@progress/kendo-angular-charts';
import { GISAnalisiMappeSatellitariAnimationComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-animation/GIS-analisi-mappe-satellitari-animation.component';
import { GISAnalisiMappeSatellitariOpzioniComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-opzioni/GIS-analisi-mappe-satellitari-opzioni.component';
import { PopupModule } from '@progress/kendo-angular-popup';
import { GISLayerListItemComponent } from './GIS-layer-window/GIS-layer-list-item/GIS-layer-list-item.component';
import { AfterValueChangedDirective } from './directives/after-value-changed-directive';
import { ThemeWindowSettingsComponent } from './GIS-kendo-window/theme-window/theme-window-settings/theme-window-settings.component';
import { ThemeWindowBandSliderComponent } from './GIS-kendo-window/theme-window/theme-window-band-slider/theme-window-band-slider.component';
import { GISGestionePianoRateoPickerComponent } from './GIS-kendo-window/GIS-gestione-piano-rateo/GIS-gestione-piano-rateo-picker/GIS-gestione-piano-rateo-picker.component';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { GISGestionePianoRateoLoaderComponent } from './GIS-kendo-window/GIS-gestione-piano-rateo/GIS-gestione-piano-rateo-loader/GIS-gestione-piano-rateo-loader.component';
import { GISAttributiGridComponent } from './GIS-attributi/GIS-attributi-grid/GIS-attributi-grid.component';
import { GISAttributiSettingsComponent } from './GIS-attributi/GIS-attributi-settings/GIS-attributi-settings.component';
import { GISAttributiFileUploadComponent } from './GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.component';
import { GISCfgProiezioniComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni.component';
import { GISAttributiFileUploadRasterComponent } from './GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload-raster/GIS-attributi-file-upload-raster.component';
import { GISAttributiFileUploadStandardComponent } from './GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload-standard/GIS-attributi-file-upload-standard.component';
import { GISAttributiFileUploadCatastoComponent } from './GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload-catasto/GIS-attributi-file-upload-catasto.component';
import { GISCfgProziezioniLayerComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni-dialog/GIS-cfg-proiezioni-layer/GIS-cfg-proiezioni-layer.component';
import { GISCfgProiezioniDialogComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni-dialog/GIS-cfg-proiezioni-dialog.component';
import { GISCfgProziezioniPermissionsComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni-permissions/GIS-cfg-proiezioni-permissions.component';
import { GISCfgProiezioniUsersPermissionsComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni-permissions/GIS-cfg-proiezioni-users-permissions/GIS-cfg-proiezioni-users-permissions.component';
import { GISCfgProiezioniGroupsPermissionsComponent } from './GIS-cfg-proiezioni/GIS-cfg-proiezioni-permissions/GIS-cfg-proiezioni-groups-permissions/GIS-cfg-proiezioni-groups-permissions.component';
import { GISAttributiAlgorithmConfigurationComponent } from './GIS-attributi/GIS-attributi-algorithm-configuration/GIS-attributi-algorithm-configuration.component';
import { GisAlgorithmConfigurationWindowComponent } from './GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration-window.component';
import { GisAlgorithmConfigurationLogComponent } from './GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration-log/GIS-algorithm-configuration-log.component';
import { GisAlgorithmConfigurationComponent } from './GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration/GIS-algorithm-configuration.component';
import { ToolDisegnoAvanzatoComponent } from './GIS-kendo-window/marker-window/tool-disegno-avanzato/tool-disegno-avanzato.component';
import { ToolPolygonMergeComponent } from './GIS-kendo-window/polygon-merge-window/tool-polygon-merge/tool-polygon-merge.component';
import { PolygonMergeWindowComponent } from './GIS-kendo-window/polygon-merge-window/polygon-merge-window.component';
import { GISAttributiExportComponent } from './GIS-attributi/GIS-attributi-export/GIS-attributi-export.component';
import { GISRasterConfigurationWindowComponent } from './GIS-raster-configuration-window/GIS-raster-configuration-window.component';
import { GISLayerListItemRasterComponent } from './GIS-layer-window/GIS-layer-list-item-raster/GIS-layer-list-item-raster.component';
import { GISLayerListItemMappePrescrizioneComponent } from './GIS-layer-window/GIS-layer-list-item-mappe-prescrizione/GIS-layer-list-item-mappe-prescrizione.component';
import { GISAttributiRasterMasksComponent } from './GIS-attributi/GIS-attributi-raster-masks/GIS-attributi-raster-masks.component';
import { GISAttributiRasterMasksPermissionsComponent } from './GIS-attributi/GIS-attributi-raster-masks/GIS-cfg-proiezioni-permissions/GIS-attributi-raster-masks-permissions.component';
import { GISAttributiRasterMasksUsersPermissionsComponent } from './GIS-attributi/GIS-attributi-raster-masks/GIS-cfg-proiezioni-permissions/GIS-attributi-raster-masks-users-permissions/GIS-attributi-raster-masks-users-permissions.component';
import { GISAttributiRasterMasksGroupsPermissionsComponent } from './GIS-attributi/GIS-attributi-raster-masks/GIS-cfg-proiezioni-permissions/GIS-attributi-raster-masks-groups-permissions/GIS-attributi-raster-masks-groups-permissions.component';
import { GISLayerWindowToolbarComponent } from './GIS-layer-window/GIS-layer-window-toolbar/GIS-layer-window-toolbar.component';
import { GISBookmarksWindowComponent } from './GIS-kendo-window/GIS-bookmarks-window/GIS-bookmarks-window.component';
import { GISAttributiMuzGridComponent } from './GIS-attributi/GIS-attributi-muz-grid/GIS-attributi-muz-grid.component';
import { GISInitMuzGroupComponent } from './GIS-attributi/GIS-attributi-muz-grid/GIS-init-muz-group/GIS-init-muz-group.component';
import { GISEditMuzComponent } from './GIS-attributi/GIS-attributi-muz-grid/GIS-edit-muz/GIS-edit-muz.component';
import { GISParticelleCatastaliComponent } from './GIS-particelle-catastali/GIS-particelle-catastali.component';
import { GISEditMuzParticelleCatastaliComponent } from './GIS-attributi/GIS-attributi-muz-grid/GIS-edit-muz/GIS-edit-muz-particelle-catastali/GIS-edit-muz-particelle-catastali.component';
import { GISCfgProiezioniConfigComponent } from './GIS-cfg-proiezioni/gis-cfg-proiezioni-config/gis-cfg-proiezioni-config.component';
import { GISAlgorithmResultComponent } from './GIS-kendo-window/GIS-algorithm-configuration-window/gis-algorithm-result/gis-algorithm-result.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import { GISTranslationsGridComponent } from './GIS-translations-grid/gis-translations-grid.component';
import { GISAnalisiMappeSatellitariDailyDataComponent } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-daily-data/GIS-analisi-mappe-satellitari-daily-data.component';
import { GisClusteringAlgorithmConfigurationWindowComponent } from './GIS-kendo-window/GIS-clustering-algorithm-configuration-window/GIS-clustering-algorithm-configuration-window.component';
import { GisClusteringAlgorithmGridComponent } from './GIS-kendo-window/GIS-clustering-algorithm-configuration-window/GIS-clustering-algorithm-grid/GIS-clustering-algorithm-grid.component';
import { GisClusteringAlgorithmDetailsComponent } from './GIS-kendo-window/GIS-clustering-algorithm-configuration-window/GIS-clustering-algorithm-details/GIS-clustering-algorithm-details.component';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] = () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        GISTooltipDirective,
        GISComponent,
        GoogleMapComponent,
        DrawWindowComponent,
        PolygonWindowComponent,
        ThemeWindowComponent,
        ThemeWindowSettingsComponent,
        ThemeWindowBandSliderComponent,
        WMSWindowComponent,
        GISConfigurationModalComponent,
        GISCalendarModalComponent,
        GISWindowComponent,
        GISLayerListItemComponent,
        GISLayerListItemMappeSatellitariComponent,
        GISLayerListItemRasterComponent,
        GISLayerListItemMappePrescrizioneComponent,
        GisToolbarComponent,
        GISLayerWindowComponent,
        GISLayerColorPickerWindowComponent,
        GISFixedLayerPropertyWindowComponent,
        TreeWindowComponent,
        GisCheckboxComponent,
        GISLayerVisibilityConfigurationWindowComponent,
        GISDeleteFeatureWindowComponent,
        GisBottomWindow,
        GisToolbarMobileComponent,
        GISAttributiComponent,
        GISAttributiGridComponent,
        GISAttributiSettingsComponent,
        GISAttributiFileUploadComponent,
        GISAttributiFileUploadStandardComponent,
        GISAttributiFileUploadRasterComponent,
        GISAttributiFileUploadCatastoComponent,
        GISAttributiAlgorithmConfigurationComponent,
        GISAttributiRasterMasksComponent,
        GISAttributiRasterMasksPermissionsComponent,
        GISAttributiRasterMasksUsersPermissionsComponent,
        GISAttributiRasterMasksGroupsPermissionsComponent,
        GISAttributiExportComponent,
        MarkerWindowComponent,
        ToolDisegnaPuntiComponent,
        ToolScomponiPuntiComponent,
        ToolDisegnoAvanzatoComponent,
        PolygonMergeWindowComponent,
        ToolPolygonMergeComponent,
        GISLayerPermissionsWindowComponent,
        GISLayerUsersPermissionsComponent,
        GISLayerGroupsPermissionsComponent,
        GISLayerAdvancedSettingsWindowComponent,
        LineeGuidaABWindowComponent,
        GISGestionePianoRateoComponent,
        GISGestionePianoRateoPickerComponent,
        GISGestionePianoRateoLoaderComponent,
        GISAnalisiMappeSatellitariWindowComponent,
        GISAnalisiMappeSatellitariCalendarioComponent,
        GISAnalisiMappeSatellitariGraficiComponent,
        GISAnalisiMappeSatellitariNotificheComponent,
        GISAnalisiMappeSatellitariAnimationComponent,
        GISAnalisiMappeSatellitariDailyDataComponent,
        GISAnalisiMappeSatellitariOpzioniComponent,
        GISCfgProiezioniComponent,
        GISCfgProiezioniDialogComponent,
        GISCfgProziezioniLayerComponent,
        GISCfgProziezioniPermissionsComponent,
        GISCfgProiezioniUsersPermissionsComponent,
        GISCfgProiezioniGroupsPermissionsComponent,
        GisAlgorithmConfigurationWindowComponent,
        GisAlgorithmConfigurationComponent,
        GisAlgorithmConfigurationLogComponent,
        GISRasterConfigurationWindowComponent,
        GISLayerWindowToolbarComponent,
        GISBookmarksWindowComponent,
        GISAttributiMuzGridComponent,
        GISInitMuzGroupComponent,
        GISEditMuzComponent,
        GISEditMuzParticelleCatastaliComponent,
        GISParticelleCatastaliComponent,
        AfterValueChangedDirective,
        GISCfgProiezioniConfigComponent,
        GISAlgorithmResultComponent,
        GISTranslationsGridComponent,
        GisClusteringAlgorithmConfigurationWindowComponent,
        GisClusteringAlgorithmGridComponent,
        GisClusteringAlgorithmDetailsComponent
    ],
    exports: [
        NavigationModule,
        GISComponent
    ],
    providers: [
        GisLoadedGuard,
        ReactiveFormsModule,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'gis',
                loader
            }
        },
        // GoogleMapGeoJsonService,
        // GoogleMapService,
        // GisService,
        // GisToolbarService,
        // MeasureDistanceService,
        // PositionService,
        LayerService,
        provideNgxMask()
    ],
    imports: [
        CommonModule,
        GoogleMapsModule,
        ReactiveFormsModule,
        FormsModule,
        UikitModule,
        TranslocoRootModule,
        FontAwesomeModule,
        NavigationModule,
        GISRoutingModule,
        IconsModule,
        ButtonsModule,
        WindowModule,
        LayoutModule,
        HttpClientModule,
        HttpClientJsonpModule,
        MaskedTextBoxModule,
        LabelModule,
        ColorPickerModule,
        DragDropModule,
        PagerModule,
        TextBoxModule,
        InputMaskModule,
        MenuAgendaModule,
        DateInputsModule,
        IndicatorsModule,
        ChartsModule,
        PopupModule,
        InputsModule,
        UploadsModule,
        NgxMaskDirective,
        NgxMaskPipe,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})

export class GISModule {
}
