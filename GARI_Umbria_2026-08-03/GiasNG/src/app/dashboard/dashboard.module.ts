import { OverlayModule } from "@angular/cdk/overlay";
import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { ChartsModule } from "@progress/kendo-angular-charts";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import { DialogsModule } from "@progress/kendo-angular-dialog";
import { ExcelExportModule } from "@progress/kendo-angular-excel-export";
import { GridModule } from "@progress/kendo-angular-grid";
import { IndicatorsModule } from "@progress/kendo-angular-indicators";
import { InputsModule, SliderModule } from "@progress/kendo-angular-inputs";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { TranslocoRootModule } from "app/transloco/transloco-root.module";
import { UikitModule } from "app/Utility/uikit.module";
import { WidgetConfigComponent } from "app/widget-config/widget-config.component";
import { WidgetGridComponent } from "app/widget-grid/widget-grid.component";
import { WidgetsComponent } from "app/widget/widgets.component";
import { ColtureWidgetComponent } from "app/widget/widgets/chart-widgets/colture-widget/colture-widget.component";
import { ConformitaWidgetComponent } from "app/widget/widgets/conformita-widget/conformita-widget.component";
import { IndicatoriWidgetDashboardComponent } from "app/widget/widgets/indicatori-widget/indicatori-widget-dashboard/indicatori-widget-dashboard.component";
import { IndicatoriWidgetFullComponent } from "app/widget/widgets/indicatori-widget/indicatori-widget-full/indicatori-widget-full.component";
import { IndicatoriWidgetGridComponent } from "app/widget/widgets/indicatori-widget/indicatori-widget-grid/indicatori-widget-grid.component";
import { IndicatoriWidgetComponent } from "app/widget/widgets/indicatori-widget/indicatori-widget.component";
import { MeteoWidgetFullComponent } from "app/widget/widgets/meteo-widget/meteo-widget-full/meteo-widget-full.component";
import { MeteoWidgetComponent } from "app/widget/widgets/meteo-widget/meteo-widget.component";
import { MonitoraggioWidgetComponent } from "app/widget/widgets/monitoraggio-widget/monitoraggio-widget.component";
import { OperazioniTemplateWidgetComponent } from "app/widget/widgets/operazioni-template-widget/operazioni-template-widget.component";
import { ProduzioneColtureWidgetComponent } from "app/widget/widgets/produzione-colture-widget/produzione-colture-widget.component";
import { RiepilogoMeteoWidgetComponent } from "app/widget/widgets/riepilogo-meteo-widget/riepilogo-meteo-widget.component";
import { UltimeAttivitaWidgetComponent } from "app/widget/widgets/ultime-attivita-widget/ultime-attivita-widget.component";
import { UltimeVisiteWidgetComponent } from "app/widget/widgets/ultime-visite-widget/ultime-visite-widget.component";
import { UltimiAcquistiWidgetComponent } from "app/widget/widgets/ultimi-acquisti-widget/ultimi-acquisti-widget.component";
import { UltimiProdottiWidgetComponent } from "app/widget/widgets/ultimi-prodotti-widget/ultimi-prodotti-widget.component";
import { UltimiRilieviWidgetComponent } from "app/widget/widgets/ultimi-rilievi-widget/ultimi-rilievi-widget.component";
import { WidgetResizerComponent } from "app/widget/widgets/widget-resizer/widget-resizer.component";
import { WidgetTemplateComponent } from "app/widget/widgets/widget-template/widget-template.component";
import { DashboardRoutingModule } from "./dashboard-routing.module";
import { DashboardComponent } from "./dashboard.component";
import { ChartWidgetComponent } from "app/widget/widgets/chart-widgets/chart-widget.component";
import { GhgColtureWidgetComponent } from "app/widget/widgets/chart-widgets/ghg-colture-widget/ghg-colture-widget.component";
import { StimeProduzioneColtureWidgetComponent } from "app/widget/widgets/chart-widgets/stime-produzione-colture-widget/stime-produzione-colture-widget.component";
import { ComparisonWidgetService } from "app/widget/widgets/comparison-widgets/comparison-widget.service";
import { ProduttivitaComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/produttivita-comparison-widget.component";
import { PlvComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/plv-comparison-widget.component";
import { ErosioneComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/erosione-comparison-widget.component";
import { ComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/comparison-widget.component";
import { Co2ComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/co2-comparison-widget.component";
import { RischioMeteoAggregatoComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-aggregato-comparison-widget.component";
import { RischioMeteoGelataComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-gelata-comparison-widget.component";
import { RischioMeteoGrandineComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-grandine-comparison-widget.component";
import { RischioMeteoSiccitaComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-siccita-comparison-widget.component";
import { RischioMeteoVentoForteComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-vento-forte-comparison-widget.component";
import { RischioMeteoAllagamentoComparisonWidgetComponent } from "app/widget/widgets/comparison-widgets/rischio-meteo-allagamento-comparison-widget.component";
import { UniqueRatingWidgetComponent } from "app/widget/widgets/unique-rating-widget/unique-rating-widget.component";
import { TargetSuperficieWidgetComponent } from "app/widget/widgets/target-superficie-widget/target-superficie-widget.component";
import { GaugesModule } from "@progress/kendo-angular-gauges";
import { LabelModule } from "@progress/kendo-angular-label";
import { SeminatiMultiAziendaWidgetComponent } from "app/widget/widgets/chart-widgets/seminati-multiazienda-widget/seminati-multiazienda-widget.component";
import { MappaMultiAziendaComponent } from "app/widget/widgets/mappa-multiazienda-widget/mappa-multiazienda-widget.component";
import { ColumnsChartWidgetComponent } from "app/widget/widgets/columns-chart-widget/columns-chart-widget.component";
import { DatiGeneraliFarmersComponent } from "app/widget/widgets/dati-generali-farmers-widget/dati-generali-farmers-widget.component";
import { PaeseMultiAziendaComponent } from "app/widget/widgets/paese-multiazienda-widget/paese-multiazienda-widget.component";
import { AziendeMultiPieWidgetComponent } from "app/widget/widgets/chart-widgets/aziende-multi-pie-widget/app-aziende-multi-pie-widget.component";
import { ColtureMultiAziendaWidget } from "app/widget/widgets/colture-multiazienda-widget/colture-multiazienda-widget.component";
import { AlertDocumentiWidgetComponent } from "app/widget/widgets/alert-documenti-widget/alert-documenti-widget.component";
import { RiepilogoMeteoWidgetService } from "app/widget/widgets/riepilogo-meteo-widget/riepilogo-meteo-widget.service";
import { MonitoraggioWidgetService } from "app/widget/widgets/monitoraggio-widget/monitoraggio-widget.service";
import { IndicatoriWidgetService } from "app/widget/widgets/indicatori-widget/indicatori-widget.service";
import { ImpreseServiceProvider } from "app/Service/ServiceFactory/imprese.factory.provider";
import { CostiRicaviAIWidgetComponent } from "app/widget/widgets/costi-ricavi-ai-widget/costi-ricavi-ai-widget.component";
import { InvalidAnimalsWidgetComponent } from "app/widget/widgets/invalid-animals-widget/invalid-animals-widget.component";
import { TreatmentsToDoWidgetComponent } from "app/widget/widgets/treatments-todo-widget/treatments-todo-widget.component";
import { TreatmentsToSendWidgetComponent } from "app/widget/widgets/treatments-tosend-widget/treatments-tosend-widget.component";
import { ExpiringDrugsWidgetComponent } from "app/widget/widgets/drugs-expiration-widget/expiring-drugs-widget.component";
import { PesateAccrescimentoWidgetComponent } from "app/widget/widgets/pesate-accrescimento-widget/pesate-accrescimento-widget.component";
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';
import { PDFExportModule } from "@progress/kendo-angular-pdf-export";
import { WidgetConfigListComponent } from "app/widget-config/section/widget-config-list/widget-config-list.component";
import { WidgetChecklistComponent } from "app/widget/widgets/widget-checklist/widget-checklist.component";

@NgModule({
    declarations: [
        DashboardComponent,
        WidgetGridComponent,
        WidgetConfigComponent,
        WidgetsComponent,
        WidgetTemplateComponent,
        OperazioniTemplateWidgetComponent,
        UltimiProdottiWidgetComponent,
        MeteoWidgetComponent,
        MeteoWidgetFullComponent,
        IndicatoriWidgetComponent,
        IndicatoriWidgetFullComponent,
        IndicatoriWidgetDashboardComponent,
        IndicatoriWidgetGridComponent,
        MonitoraggioWidgetComponent,
        ConformitaWidgetComponent,
        UltimiRilieviWidgetComponent,
        UltimeAttivitaWidgetComponent,
        UltimeVisiteWidgetComponent,
        ChartWidgetComponent,
        ColtureWidgetComponent,
        AziendeMultiPieWidgetComponent,
        ColtureMultiAziendaWidget,
        TargetSuperficieWidgetComponent,
        SeminatiMultiAziendaWidgetComponent,
        MappaMultiAziendaComponent,
        ColumnsChartWidgetComponent,
        DatiGeneraliFarmersComponent,
        PaeseMultiAziendaComponent,
        AlertDocumentiWidgetComponent,
        GhgColtureWidgetComponent,
        StimeProduzioneColtureWidgetComponent,
        ProduzioneColtureWidgetComponent,
        RiepilogoMeteoWidgetComponent,
        UltimiAcquistiWidgetComponent,
        WidgetResizerComponent,
        ComparisonWidgetComponent,
        ProduttivitaComparisonWidgetComponent,
        PlvComparisonWidgetComponent,
        ErosioneComparisonWidgetComponent,
        Co2ComparisonWidgetComponent,
        RischioMeteoAggregatoComparisonWidgetComponent,
        RischioMeteoAllagamentoComparisonWidgetComponent,
        RischioMeteoGelataComparisonWidgetComponent,
        RischioMeteoGrandineComparisonWidgetComponent,
        RischioMeteoSiccitaComparisonWidgetComponent,
        RischioMeteoVentoForteComparisonWidgetComponent,
        UniqueRatingWidgetComponent,
        CostiRicaviAIWidgetComponent,
        InvalidAnimalsWidgetComponent,
        TreatmentsToDoWidgetComponent,
        TreatmentsToSendWidgetComponent,
        ExpiringDrugsWidgetComponent,
        PesateAccrescimentoWidgetComponent,
        WidgetChecklistComponent
    ],
    imports: [
        // HttpClientModule,
        // NotificationModule,
        // NgbModule,
        // IconsModule,
        // MenusModule,
        // TooltipModule,
        // DropDownsModule,
        // GISModule,
        // NavigationModule,
        DashboardRoutingModule,
        CommonModule,
        LayoutModule,
        FontAwesomeModule,
        GridModule,
        ButtonsModule,
        FormsModule,
        ReactiveFormsModule,
        TranslocoRootModule,
        RouterModule,
        DialogsModule,
        OverlayModule,
        IndicatorsModule,
        UikitModule,
        ChartsModule,
        DateInputsModule,
        ExcelExportModule,
        InputsModule,
        SliderModule,
        GaugesModule,
        LabelModule,
        PDFExportModule,
        WidgetConfigListComponent,
        GiasKendoGridModule,
        GiasUikitModule
    ],
    providers: [
        //     FormGroupDirective,
        //     TranslocoPipe,
        //     DatePipe,
        ComparisonWidgetService,
        RiepilogoMeteoWidgetService,
        MonitoraggioWidgetService,
        IndicatoriWidgetService,
        ImpreseServiceProvider
    ]
})

export class DashboardModule {
}
