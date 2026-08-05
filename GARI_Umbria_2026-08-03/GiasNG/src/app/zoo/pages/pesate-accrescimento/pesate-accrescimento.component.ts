import {
  AfterViewInit,
  Component,
  DestroyRef,
  inject,
  ViewChild,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ChartsModule } from '@progress/kendo-angular-charts';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GiasKendoGridComponent, GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasDialogService, GiasDropDownTemplateService, GiasMultiSelectTemplateService, GiasUikitModule } from 'gias-ui-kit';
import { catchError, filter, of, skip, Subject, switchMap, take, tap } from 'rxjs';
import { process, CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { GiasMessageService } from '../../../Service/gias-message.service';
import { ObjParametriAgendaService } from '../../../Service/obj-parametri-agenda.service';
import { CookieService } from '../../../Service/cookie.service';
import { TranslocoRootModule } from '../../../transloco/transloco-root.module';
import { BaseCodeDescr as IBaseCodeDescr } from '../../../Service/api.service';
import {
  CurvaTeoricaPoint,
  PesataChartPoint,
  PesataCurvaAccrescimentoRow,
  PesateCurvaAccrescimentoResult,
  PesateAccrescimentoFilters,
  PesateAccrescimentoFiltersForm,
} from '../../models/pesate-accrescimento.model';
import { PesateAccrescimentoGridConfigService } from '../../services/pesate-accrescimento-grid-config.service';
import { PesateAccrescimentoService } from '../../services/pesate-accrescimento.service';
import { ZooFiltersHelperService } from '../../services/zoo-filters-helper.service';

/**
 * Pagina dedicata "Pesate e Accrescimento" (Fase 1).
 * Implementa:
 *  - Pannello filtri laterale (gias-side-filters-template)
 *  - Griglia paginata delle pesate (gias-kendo-grid)
 *  - Grafico time-series scatter + curva teorica + fascia confidenza (kendo-chart)
 *  - Export CSV / XLS (DS07-API)
 *
 * Vedere UIDS001 — sezione "Griglia Metriche Accrescimento — Pagina Dedicata (Fase 1)".
 */
@Component({
  standalone: true,
  selector: 'zoo-pesate-accrescimento',
  templateUrl: './pesate-accrescimento.component.html',
  styleUrls: ['./pesate-accrescimento.component.scss'],
  imports: [
    GiasUikitModule,
    GiasKendoGridModule,
    ChartsModule,
    ButtonsModule,
    LayoutModule,
    ReactiveFormsModule,
    TranslocoRootModule,
  ],
  providers: [
    ...generateGridProviders(PesateAccrescimentoGridConfigService, PesateAccrescimentoComponent),
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
  ],
})
export class PesateAccrescimentoComponent implements AfterViewInit {
  @ViewChild('pesateGrid') grid: GiasKendoGridComponent;

  // ── Reactive state ──────────────────────────────────────────────────────────────────────────

  /** Current full API response (metadata + data), used by both grid and chart. */
  protected currentResult: PesateCurvaAccrescimentoResult | null = null;

  /** Scatter points for the real weighings chart series. */
  protected chartScatterPoints: PesataChartPoint[] = [];

  /** Points for the theoretical growth curve overlay. */
  protected chartCurvaTeeoricaPoints: CurvaTeoricaPoint[] = [];

  /** True once the zoo i18n scope has finished loading — gates template translation pipes. */
  protected zooLoaded = false;

  /** Export format selection. 'CSV' | 'XLS' */
  protected exportFormat: 'CSV' | 'XLS' = 'CSV';

  protected isExporting = false;

  // ── Filter form ─────────────────────────────────────────────────────────────────────────────

  protected stableItems: { text: string; value: string }[] = [];
  protected razzaItems: { text: string; value: string }[] = [];
  protected centerItems: IBaseCodeDescr[] = [];

  protected filters = new FormGroup<PesateAccrescimentoFiltersForm>({
    center: new FormControl<number>(0, { nonNullable: true }),
    stalla_key: new FormControl<string | null>(null),
    razza_key: new FormControl<string[]>([], { nonNullable: true }),
    date_from: new FormControl<Date | null>(this.defaultDateFrom()),
    date_to: new FormControl<Date | null>(new Date()),
    kg_per_day: new FormControl<number>(0.8, { nonNullable: true }),
    alert_threshold_pct: new FormControl<number>(10.0, { nonNullable: true }),
  });

  /** Subject used to trigger grid loads — mirrors pattern in zoo-component children. */
  private filters$ = new Subject<PesateAccrescimentoFilters>();

  /** Last column filter applied by the user in the grid header — used to keep the chart in sync. */
  private _lastColumnFilter: CompositeFilterDescriptor | null = null;

  /** Tracks last applied filters for export + chart refresh. */
  private _currentFilters: PesateAccrescimentoFilters = this.buildFilters(1);

  private destroyRef = inject(DestroyRef);

  constructor(
    private pesateService: PesateAccrescimentoService,
    private messageService: GiasMessageService,
    private dialog: GiasDialogService,
    private transloco: TranslocoService,
    private agenda: ObjParametriAgendaService,
    private zooFiltersHelper: ZooFiltersHelperService,
    private cookies: CookieService,
  ) {}

  private get currentPiva(): string {
    return this.agenda.getObjParamValue()?.Piva ?? '';
  }

  ngAfterViewInit(): void {
    const piva = this.currentPiva;

    // Load centers and stables for the filter dropdowns.
    this.zooFiltersHelper.loadBusinessCenters(piva).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(centers => {
      this.centerItems = centers;
    });

    // Reload stables whenever the center selection changes.
    // Mirror zoo.component logic: if exactly 1 real stable exists, auto-select it;
    // otherwise auto-select "Tutte le stalle" (null = no stalla filter).
    this.filters.controls.center.valueChanges.pipe(
      switchMap(center => this.zooFiltersHelper.loadStables(piva, center)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(stalls => {
      this.stableItems = this.mapStableItems(stalls, piva);
      if (stalls.length === 2) {
        // Only one real stable for this center — auto-select it.
        this.filters.controls.stalla_key.setValue(`${piva}_${stalls[1].key}`);
      } else {
        // Multiple stables or none — reset to "Tutte le stalle".
        this.filters.controls.stalla_key.setValue(null);
      }
    });

    // Initial load of all stables (center = 0 → all centers).
    this.zooFiltersHelper.loadStables(piva, 0).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(stalls => {
      this.stableItems = this.mapStableItems(stalls, piva);
    });

    // Load all races for this farm from the DB, independent of date/stall filters.
    this.zooFiltersHelper.loadRazzeZoo().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(razze => {
      this.razzaItems = razze;
    });

    // Load data whenever filters$ emits (including on first load from applyFilters() call in init).
    // Pattern: call config.read(filters) directly to populate _gridRows, then refresh(true)
    // so the grid calls conf.read(null) which returns the cached _gridRows.
    this.filters$.pipe(
      filter(() => !!this.grid),
      tap((f) => (this._currentFilters = f)),
      switchMap((f) =>
        (this.grid.config as unknown as PesateAccrescimentoGridConfigService).read(f).pipe(
          catchError(() => {
            this.messageService.errorMessage(
              this.transloco.translate('ErroreCaricamentoPesate')
            );
            return of(null);
          })
        )
      ),
      filter((r) => r !== null),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => {
      const configSvc = this.grid.config as unknown as PesateAccrescimentoGridConfigService;
      this.currentResult = configSvc.lastResult;

      // Populate razza filter items from first load.
      const data = this.currentResult?.data ?? [];

      this._lastColumnFilter = null; // reset column filter when new server data arrives
      this.grid.publicService.refresh(true);
      this.refreshChart();
    });

    // Sync chart with grid column filters (client-side filtering by Kendo).
    // GridPublicService.filters emits the current CompositeFilterDescriptor on every filter change.
    // Skip the initial BehaviorSubject emission (null) and react only to user-driven changes.
    this.grid.publicService.filters.pipe(
      skip(1),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe((f: CompositeFilterDescriptor | null) => {
      this._lastColumnFilter = f;
      this.refreshChart();
    });

    // Restore saved filters from cookies before the initial load.
    this.readSavedFilters();

    // Delay the initial load until the zoo i18n scope is ready to avoid
    // "Missing translation" warnings when gridColumns is built.
    const scopeLang = `zoo/${this.transloco.getActiveLang()}`;
    const scopeAlreadyLoaded = !!Object.keys(this.transloco.getTranslation(scopeLang) ?? {}).length;
    if (scopeAlreadyLoaded) {
      this.zooLoaded = true;
      this.applyFilters();
    } else {
      this.transloco.events$.pipe(
        filter(e => e.type === 'translationLoadSuccess'),
        take(1),
        takeUntilDestroyed(this.destroyRef)
      ).subscribe(() => {
        this.zooLoaded = true;
        this.applyFilters();
      });
    }
  }

  // ── User actions ─────────────────────────────────────────────────────────────────────────────

  /**
   * Apply current filter form values and reload the grid (page 1).
   * Called by (search) output of gias-side-filters-template.
   * See UIDS001 — "Pattern Filtri — gias-side-filters-template".
   */
  protected applyFilters(): void {
    this.rememberFilters();
    this.filters$.next(this.buildFilters(1));
  }

  /**
   * Trigger file export.
   * See UIDS001 — "Export Dati Pesate — Pagina Dedicata (Fase 1)" and DS07-API.
   */
  protected onExport(): void {
    this.isExporting = true;

    // Column order matches PesateAccrescimentoGridConfigService.gridColumns definition order.
    const columns = 'sta_des,razza_des,lid,peso_kg,peso_teorico,scostamento_pct,data_pesata,eta_giorni,alert_label';

    this.pesateService
      .exportPesate(this._currentFilters, this.exportFormat, 'detailed', undefined, true, columns)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError(() => {
          this.messageService.errorMessage(
            this.transloco.translate('ErroreDuranteEsportazione')
          );
          this.isExporting = false;
          return of(null);
        })
      )
      .subscribe((blob) => {
        this.isExporting = false;
        if (!blob) return;

        const ts = new Date()
          .toISOString()
          .replace(/[-:T]/g, '')
          .substring(0, 14);
        const ext = this.exportFormat === 'XLS' ? 'xlsx' : 'csv';
        const fileName = `pesate_accrescimento_detailed_${ts}.${ext}`;

        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName;
        anchor.click();
        window.URL.revokeObjectURL(url);

        this.messageService.successMessage(
          this.transloco.translate('EsportazioneEseguita')
        );
      });
  }

  // ── Chart helpers ────────────────────────────────────────────────────────────────────────────

  /**
   * Rebuilds chart data from the current result.
   * See UIDS001 — "Grafico Time-Series Pesate vs Curva Teorica".
   */
  private refreshChart(): void {
    const allRows = (this.currentResult?.data ?? []) as PesataCurvaAccrescimentoRow[];
    const rows: PesataCurvaAccrescimentoRow[] =
      this._lastColumnFilter?.filters?.length
        ? (process(allRows as any[], { filter: this._lastColumnFilter }).data as PesataCurvaAccrescimentoRow[])
        : allRows;

    this.chartScatterPoints = rows.map((r) => {
      const d = r.data_pesata ? new Date(r.data_pesata) : null;
      const data_pesata_fmt = d
        ? `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
        : '';
      return {
        eta_giorni: r.eta_giorni,
        peso_kg: r.peso_kg,
        lid: r.lid,
        data_pesata: r.data_pesata,
        data_pesata_fmt,
        peso_teorico: r.peso_teorico,
        scostamento_pct: r.scostamento_pct,
      };
    });

    // Theoretical curve: use peso_teorico values, sorted by eta_giorni.
    const sorted = [...rows].sort((a, b) => a.eta_giorni - b.eta_giorni);
    this.chartCurvaTeeoricaPoints = sorted.map((r) => ({
      eta_giorni: r.eta_giorni,
      peso_teorico: r.peso_teorico,
      fascia_sup: +(r.peso_teorico * 1.05).toFixed(2),
      fascia_inf: +(r.peso_teorico * 0.95).toFixed(2),
    }));
  }

  // ── Private helpers ──────────────────────────────────────────────────────────────────────────

  private buildFilters(page: number): PesateAccrescimentoFilters {
    const v = this.filters.value;
    return {
      piva: this.currentPiva,
      stalla_key: v.stalla_key ?? null,
      razza_key: v.razza_key ?? [],
      date_from: v.date_from ?? null,
      date_to: v.date_to ?? null,
      kg_per_day: v.kg_per_day ?? 0.8,
      alert_threshold_pct: v.alert_threshold_pct ?? 10.0,
      page,
      limit: 100,
      sort_by: 'lid',
      sort_order: 'ASC',
    };
  }

  private defaultDateFrom(): Date {
    const d = new Date();
    d.setDate(d.getDate() - 90);
    return d;
  }

  /** Saves current filter values to cookies (mirrors zoo.component pattern). */
  private rememberFilters(): void {
    const v = this.filters.value;
    this.cookies.setCookie({ name: 'pesate_piva',                value: this.currentPiva,                                  expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_center',              value: String(v.center ?? 0),                             expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_stalla_key',          value: v.stalla_key ?? '',                                expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_razza_key',           value: (v.razza_key ?? []).join('|'),                     expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_date_from',           value: v.date_from?.toISOString() ?? '',                  expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_date_to',             value: v.date_to?.toISOString() ?? '',                    expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_kg_per_day',          value: String(v.kg_per_day ?? 0.8),                       expireDays: 10 * 365 });
    this.cookies.setCookie({ name: 'pesate_alert_threshold_pct', value: String(v.alert_threshold_pct ?? 10.0),             expireDays: 10 * 365 });
  }

  /** Restores saved filter values from cookies (only when same PIVA). */
  private readSavedFilters(): void {
    const savedPiva = this.cookies.getCookie('pesate_piva');
    if (savedPiva !== this.currentPiva) return;

    const center       = parseInt(this.cookies.getCookie('pesate_center') ?? '0');
    const stallaKey    = this.cookies.getCookie('pesate_stalla_key') || null;
    const razzaRaw     = this.cookies.getCookie('pesate_razza_key') ?? '';
    const dateFromRaw  = this.cookies.getCookie('pesate_date_from');
    const dateToRaw    = this.cookies.getCookie('pesate_date_to');
    const kgPerDay     = parseFloat(this.cookies.getCookie('pesate_kg_per_day') ?? '0.8');
    const alertThresh  = parseFloat(this.cookies.getCookie('pesate_alert_threshold_pct') ?? '10.0');

    if (!isNaN(center))       this.filters.controls.center.patchValue(center);
    if (stallaKey !== null)   this.filters.controls.stalla_key.patchValue(stallaKey);
    if (razzaRaw)             this.filters.controls.razza_key.patchValue(razzaRaw.split('|').filter(x => !!x));
    if (dateFromRaw)          this.filters.controls.date_from.patchValue(new Date(dateFromRaw));
    if (dateToRaw)            this.filters.controls.date_to.patchValue(new Date(dateToRaw));
    if (!isNaN(kgPerDay))    this.filters.controls.kg_per_day.patchValue(kgPerDay);
    if (!isNaN(alertThresh)) this.filters.controls.alert_threshold_pct.patchValue(alertThresh);
  }

  /** Maps StableItems from ZooFiltersHelperService to dropdown items.
   *  stables[0] is always 'Tutte le stalle' (codice === 0) → value null.
   */
  private mapStableItems(stalls: { codice: number; key: string; descrizione: string }[], piva: string): { text: string; value: string | null }[] {
    return stalls.map(s => ({
      text: s.descrizione,
      value: s.codice === 0 ? null : `${piva}_${s.key}`,
    }));
  }
}
