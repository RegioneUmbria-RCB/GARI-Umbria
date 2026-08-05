import { Component } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { AuditCompletamentoModel, WidgetsClient } from "app/Service/api.service";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";

interface ChartDataItem {
    categoria: string;
    completate: number;
    nonCompletate: number;
    audit_Tipo: number;
    audit_Nome: string;
}

interface OtherAuditChart {
    audit_Tipo: number;
    audit_Nome: string;
    categories: string[];
    completate: number[];
    nonCompletate: number[];
    chartData: ChartDataItem[];
    plotAreaHeight: number;
}

@Component({
  standalone: false,
  selector: 'app-widget-checklist',
  templateUrl: './widget-checklist.component.html',
  styleUrls: ['./widget-checklist.component.css']
})
export class WidgetChecklistComponent {

    loading = false;

    protected data: AuditCompletamentoModel[];

    // Dati per grafico BIO (Audit_Tipo 20 con colture)
    protected titleBIO: string = "";
    protected colture: string[] = [];
    protected completate: number[] = [];
    protected nonCompletate: number[] = [];
    protected chartDataBIO: ChartDataItem[] = [];

    // Dati per altri audit (tipo 12, ecc.) - array di grafici
    protected otherAuditCharts: OtherAuditChart[] = [];

    // Altezza per mantenere lo stesso spessore delle barre
    protected readonly barHeight = 50; // altezza fissa per barra nel plotArea
    protected readonly barGap = 1; // gap tra le categorie (relativo alla barra)
    protected plotAreaHeightBIO = 150;

    constructor(
                private widgetsService: WidgetsClient,
                private gestioneRichiesteService: GestioneRichiesteService,
                private transloco: TranslocoService,
                private objParamAgendaService: ObjParametriAgendaService) {
                    
        this.loading = true;

        this.widgetsService.widgetsCompletamentoCheckList("").subscribe(risp => {

            this.data = risp.RispostaStringa;

            this.elaboraDati();

            this.loading = false;
        });
    }

    private elaboraDati(): void {
        this.titleBIO = this.transloco.translate('CheckListBIO');

        // Separa dati BIO (Audit_Tipo 20 con colture) dagli altri
        const datiBIO = this.data.filter(d => 
            d.Coltura && d.Coltura.trim() !== '' && d.Audit_Tipo === 20
        );
        
        // Altri audit (tutti quelli che non sono tipo 20 e non hanno coltura)
        const datiAltriAudit = this.data.filter(d => 
            (!d.Coltura || d.Coltura.trim() === '') && d.Audit_Tipo !== 20
        );

        // === ELABORA GRAFICO BIO (Audit_Tipo 20) ===
        if (datiBIO.length > 0) {
            // Le colture sono già uniche, prendi direttamente
            this.colture = datiBIO.map(item => item.Coltura);

            // Popola dati per il grafico (ogni riga ha già Completate e Non_Completate)
            this.chartDataBIO = datiBIO.map(item => ({
                categoria: item.Coltura,
                completate: (item as any).Completate || 0,
                nonCompletate: (item as any).Non_Completate || 0,
                audit_Tipo: item.Audit_Tipo || 0,
                audit_Nome: item.Audit_Nome || ''
            }));

            // Array per kendo-chart
            this.completate = this.chartDataBIO.map(d => d.completate);
            this.nonCompletate = this.chartDataBIO.map(d => d.nonCompletate);

            let effectiveHeightPerBar = ((this.barHeight) * (1 + this.barGap)); 
            effectiveHeightPerBar = (datiAltriAudit.length > 0) ? effectiveHeightPerBar - 30 : effectiveHeightPerBar;
            this.plotAreaHeightBIO = this.colture.length * effectiveHeightPerBar;
        }

        // === ELABORA ALTRI AUDIT (tipo 12, ecc.) ===
        if (datiAltriAudit.length > 0) {
            // Raggruppa per Audit_Tipo
            const groupedByType = datiAltriAudit.reduce((acc, item) => {
                const tipo = item.Audit_Tipo || 0;
                if (!acc[tipo]) {
                    acc[tipo] = [];
                }
                acc[tipo].push(item);
                return acc;
            }, {} as { [key: number]: AuditCompletamentoModel[] });

            // Crea un grafico per ogni Audit_Tipo
            this.otherAuditCharts = Object.keys(groupedByType).map(tipoKey => {
                const tipo = parseInt(tipoKey);
                const items = groupedByType[tipo];
                
                const chartData: ChartDataItem[] = items.map(item => ({
                    categoria: item.Audit_Nome || `Audit ${tipo}`,
                    completate: (item as any).Completate || 0,
                    nonCompletate: (item as any).Non_Completate || 0,
                    audit_Tipo: item.Audit_Tipo || 0,
                    audit_Nome: item.Audit_Nome || ''
                }));

                const categories = chartData.map(d => d.categoria);
                const completate = chartData.map(d => d.completate);
                const nonCompletate = chartData.map(d => d.nonCompletate);
                
                // Calcola altezza (di solito sarà una sola categoria)
                const effectiveHeightPerBar = ((this.barHeight) * (1 + this.barGap));
                const plotAreaHeight = categories.length * effectiveHeightPerBar;

                return {
                    audit_Tipo: tipo,
                    audit_Nome: items[0]?.Audit_Nome || `Audit ${tipo}`,
                    categories,
                    completate,
                    nonCompletate,
                    chartData,
                    plotAreaHeight
                };
            });
        }

        console.log('Chart Data BIO:', this.chartDataBIO);
        console.log('Other Audit Charts:', this.otherAuditCharts);
    }

    public labelContent(e: any): string {
        return e.value > 0 ? e.value.toString() : '';
    }

    public onBarClickBIO(event: any): void {
        const categoryIndex = this.colture.indexOf(event.category);
        
        if (categoryIndex >= 0 && this.chartDataBIO[categoryIndex]) {
            const item = this.chartDataBIO[categoryIndex];

            this.goToSiteAudit(item.audit_Tipo);
        }
    }

    public onBarClickOtherAudit(event: any, chart: OtherAuditChart): void {
        const categoryIndex = chart.categories.indexOf(event.category);
        
        if (categoryIndex >= 0 && chart.chartData[categoryIndex]) {
            const item = chart.chartData[categoryIndex];

            this.goToSiteAudit(item.audit_Tipo);
        }
    }

    private goToSiteAudit(audit_Tipo: number): void {
        let objParams = this.objParamAgendaService.getObjParamValue();
        this.gestioneRichiesteService.gestionePassaggioAltroSito(
                                                                Enum_SiteRedirector.Sito_AgronicaAudit,
                                                                audit_Tipo
                                    ).then((resp) => {
                                                            window.location.href = resp;
                                    });
    }
}