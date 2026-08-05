import { FormControl } from '@angular/forms';
import { KendoGridModel, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';

// ── API response shapes (DS06-API) ─────────────────────────────────────────────────────────────

/**
 * Single weighing row returned by GET /api/v1/animals/weighing-curves.
 * See UIDS001 — "Campi Response" table.
 */
export interface PesataCurvaAccrescimentoRow {
  lid: string;
  data_pesata: string;
  peso_kg: number;
  eta_giorni: number;
  peso_teorico: number;
  scostamento_pct: number;
  alert_flag: boolean;
  /** Derived display field: 'Sì' | 'No' — added client-side before grid rendering. */
  alert_label?: string;
  razza_key: string;
  razza_des: string;
  stalla_key: string;
  sta_des: string;
}

export interface PesateDateRange {
  from: string;
  to: string;
}

export interface PesatePagination {
  page: number;
  limit: number;
  total_pages: number;
  has_next: boolean;
}

export interface PesateCurvaAccrescimentoMetadata {
  query_timestamp: string;
  stalla_key_filtro: string[];
  razza_key_filtro: string[];
  date_range: PesateDateRange;
  kg_per_day: number;
  alert_threshold_pct: number;
  total_animals: number;
  total_weighings: number;
  pagination: PesatePagination;
}

export interface PesateCurvaAccrescimentoResult {
  metadata: PesateCurvaAccrescimentoMetadata;
  data: PesataCurvaAccrescimentoRow[];
}
/** KendoGridModel for the pesate / curva di accrescimento grid. */
export const PesataCurvaAccrescimentoModel: KendoGridModel = {
  lid:            new ModelEntry(CELL_TYPES.STRING),
  data_pesata:    new ModelEntry(CELL_TYPES.DATE),
  peso_kg:        new ModelEntry(CELL_TYPES.NUMBER),
  eta_giorni:     new ModelEntry(CELL_TYPES.NUMBER),
  peso_teorico:   new ModelEntry(CELL_TYPES.NUMBER),
  scostamento_pct: new ModelEntry(CELL_TYPES.NUMBER),
  alert_flag:     new ModelEntry(CELL_TYPES.BOOLEAN),
  alert_label:    new ModelEntry(CELL_TYPES.STRING),
  razza_key:      new ModelEntry(CELL_TYPES.STRING),
  razza_des:      new ModelEntry(CELL_TYPES.STRING),
  stalla_key:     new ModelEntry(CELL_TYPES.STRING),
  sta_des:        new ModelEntry(CELL_TYPES.STRING),
};
// ── Chart helpers ────────────────────────────────────────────────────────────────────────────────

/**
 * Single data point for the Kendo scatter series (real weighing).
 * See UIDS001 — "Serie Scatter — Pesate Reali".
 */
export interface PesataChartPoint {
  /** X axis: giorni dalla nascita */
  eta_giorni: number;
  /** Y axis: peso reale (kg) */
  peso_kg: number;
  /** Tooltip extras */
  lid: string;
  data_pesata: string;
  /** Pre-formatted date string (dd/MM/yyyy) — avoids Angular pipe inside Kendo tooltip overlay */
  data_pesata_fmt: string;
  peso_teorico: number;
  scostamento_pct: number;
}

/**
 * Single point for the theoretical growth curve overlay.
 * Computed client-side from peso_teorico.
 * See UIDS001 — "Linea Curva Teorica (linea rossa) — Calcolo Frontend".
 */
export interface CurvaTeoricaPoint {
  eta_giorni: number;
  peso_teorico: number;
  fascia_sup: number;
  fascia_inf: number;
}

// ── Filter form ─────────────────────────────────────────────────────────────────────────────────

/**
 * Reactive form group shape for the pesate accrescimento filter panel.
 * See UIDS001 — "Filtri Input UI → Query Parameters".
 */
export interface PesateAccrescimentoFiltersForm {
  center: FormControl<number>;
  stalla_key: FormControl<string | null>;
  razza_key: FormControl<string[]>;
  date_from: FormControl<Date | null>;
  date_to: FormControl<Date | null>;
  kg_per_day: FormControl<number>;
  alert_threshold_pct: FormControl<number>;
}

/**
 * Plain filters object passed to the grid config service read() and the HTTP service.
 */
export interface PesateAccrescimentoFilters {
  piva: string;
  stalla_key: string | null;
  razza_key: string[];
  date_from: Date | null;
  date_to: Date | null;
  kg_per_day: number;
  alert_threshold_pct: number;
  page: number;
  limit: number;
  sort_by: string;
  sort_order: string;
}

// ── Aggregated API shapes (DS06-API — view_mode=aggregated) ────────────────────────────────────

/**
 * Single row returned by GET /api/v1/animals/weighing-curves?view_mode=aggregated.
 * See UIDS001 — "Widget Pesate e Accrescimento (Fase 2)".
 */
export interface PesataAggregataRow {
  gruppo_key: string;
  gruppo_des: string;
  numero_animali: number;
  numero_pesate: number;
  scostamento_medio_pct: number;
  scostamento_max_pct: number;
  scostamento_min_pct: number;
  alert_count: number;
}

export interface PesateAggregataResult {
  metadata: PesateCurvaAccrescimentoMetadata;
  data: PesataAggregataRow[];
}

/** KendoGridModel for the aggregated pesate widget grid. */
export const PesataAggregataModel: KendoGridModel = {
  gruppo_key:             new ModelEntry(CELL_TYPES.STRING),
  gruppo_des:             new ModelEntry(CELL_TYPES.STRING),
  numero_animali:         new ModelEntry(CELL_TYPES.NUMBER),
  numero_pesate:          new ModelEntry(CELL_TYPES.NUMBER),
  scostamento_medio_pct:  new ModelEntry(CELL_TYPES.NUMBER),
  scostamento_max_pct:    new ModelEntry(CELL_TYPES.NUMBER),
  scostamento_min_pct:    new ModelEntry(CELL_TYPES.NUMBER),
  alert_count:            new ModelEntry(CELL_TYPES.NUMBER),
};

/**
 * Simplified filter object for the aggregated widget API call.
 */
export interface PesateAggregatedFilters {
  piva: string;
  aggregation_level: 'animal' | 'razza_key' | 'stalla_key';
}

/** Cookie stored by the pesate-accrescimento widget to persist the user selection. */
export interface PesateWidgetCookie {
  aggregation_level: 'animal' | 'razza_key' | 'stalla_key';
  view_mode?: 'grid' | 'chart';
}
