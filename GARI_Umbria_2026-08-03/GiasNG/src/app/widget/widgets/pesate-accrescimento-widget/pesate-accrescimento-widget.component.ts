import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';

import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService } from 'gias-kendo-grid';
import { GiasDropDownTemplateService } from 'gias-ui-kit';

import { PesateAccrescimentoService } from '../../../zoo/services/pesate-accrescimento.service';
import { GiasMessageService } from '../../../Service/gias-message.service';
import { PesataAggregataRow } from '../../../zoo/models/pesate-accrescimento.model';
import { PesateAccrescimentoWidgetGridConfig } from './pesate-accrescimento-widget-grid-config.service';

export interface AggregationLevelItem {
  value: 'animal' | 'razza_key' | 'stalla_key';
  label: string;
}

@Component({
  standalone: false,
  selector: 'app-pesate-accrescimento-widget',
  templateUrl: './pesate-accrescimento-widget.component.html',
  styleUrls: ['./pesate-accrescimento-widget.component.scss'],
  providers: [
    ...generateGridProviders(PesateAccrescimentoWidgetGridConfig, PesateAccrescimentoWidgetComponent),
    GiasDropDownTemplateService,
  ],
})
export class PesateAccrescimentoWidgetComponent implements OnInit, OnDestroy {
  @Input() piva: string | null = null;

  private readonly chartPalette = [
    '#ff6358', '#ffe162', '#4cd180', '#4b5ffa', '#ac58ff',
    '#ff5892', '#eb7b56', '#00d0e1', '#f7c400', '#3cb8c3',
  ];

  viewMode: 'grid' | 'chart' = 'grid';
  chartData: PesataAggregataRow[] = [];
  chartCategories: string[] = [];
  chartBarData: { value: number; color: string }[] = [];
  chartAlertData: number[] = [];

  aggregationLevels: AggregationLevelItem[] = [];

  filterForm = new FormGroup({
    aggregationLevel: new FormControl<'animal' | 'razza_key' | 'stalla_key'>('stalla_key'),
  });

  private formSub: Subscription;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridConfig: PesateAccrescimentoWidgetGridConfig,
    private gridPublicService: GridPublicService,
    private transloco: TranslocoService,
    private pesateService: PesateAccrescimentoService,
    private giasMessageService: GiasMessageService,
  ) {}

  ngOnInit(): void {
    this.aggregationLevels = [
      { value: 'stalla_key', label: this.transloco.translate('widget_pesate_agg_stalla') },
      { value: 'razza_key', label: this.transloco.translate('widget_pesate_agg_razza') },
    ];

    const cookie = this.gridConfig.getWidgetCookie();
    const rawLevel = cookie?.aggregation_level;
    const initialLevel: 'razza_key' | 'stalla_key' = rawLevel === 'razza_key' ? 'razza_key' : 'stalla_key';
    this.viewMode = cookie?.view_mode ?? 'grid';
    this.filterForm.get('aggregationLevel').setValue(initialLevel, { emitEvent: false });

    this.applyFilters(initialLevel);

    this.formSub = this.filterForm.get('aggregationLevel').valueChanges.subscribe((level) => {
      this.gridConfig.setWidgetCookie({ aggregation_level: level, view_mode: this.viewMode });
      this.applyFilters(level);
    });
  }

  ngOnDestroy(): void {
    this.formSub?.unsubscribe();
  }

  onExportCsv(): void {
    this.export('CSV');
  }

  onExportXls(): void {
    this.export('XLS');
  }

  setViewMode(mode: 'grid' | 'chart'): void {
    this.viewMode = mode;
    const level = this.filterForm.get('aggregationLevel').value;
    this.gridConfig.setWidgetCookie({ aggregation_level: level, view_mode: mode });
  }

  columnLabelContent = (e: { value: number | null }): string =>
    e.value != null ? `${(+e.value).toFixed(1)}%` : '';

  onRowsLoaded(): void {
    this.chartData = [...this.gridConfig.chartRows];
    this.chartCategories = this.chartData.map(r => r.gruppo_des);
    this.chartAlertData = this.chartData.map(r => r.alert_count);
    this.chartBarData = this.chartData.map((row, idx) => ({
      value: row.scostamento_medio_pct,
      color: this.chartPalette[idx % this.chartPalette.length],
    }));
  }

  private applyFilters(aggregation_level: 'animal' | 'razza_key' | 'stalla_key'): void {
    this.gridConfig.param = {
      piva: this.piva,
      aggregation_level,
    };
    this.gridPublicService.refresh(true);
  }

  private export(format: 'CSV' | 'XLS'): void {
    const level = this.filterForm.get('aggregationLevel').value;
    const ext = format === 'CSV' ? 'csv' : 'xlsx';
    const ts = new Date().toISOString().replace(/[-:T]/g, '').substring(0, 14);
    const filename = `pesate_aggr_${ts}.${ext}`;

    this.pesateService
      .exportPesate(
        {
          piva: this.piva,
          stalla_key: null,
          razza_key: [],
          date_from: null,
          date_to: null,
          kg_per_day: 0.8,
          alert_threshold_pct: 20,
          page: 1,
          limit: 99999,
          sort_by: 'scostamento_pct',
          sort_order: 'desc',
        },
        format,
        'aggregated',
        level,
        false
      )
      .subscribe({
        next: (blob: Blob) => {
          const url = URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = filename;
          a.click();
          URL.revokeObjectURL(url);
        },
        error: () => {
          this.giasMessageService.errorMessage(this.transloco.translate('widget_pesate_export_error'));
        },
      });
  }
}
