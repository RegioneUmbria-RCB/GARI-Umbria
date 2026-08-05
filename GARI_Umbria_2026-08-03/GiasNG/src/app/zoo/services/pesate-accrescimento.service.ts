import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Inject, Injectable, Optional } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { NETCORE6_API_BASE_URL } from 'app/Service/net-core6-api.service';
import {
  PesateAccrescimentoFilters,
  PesateAggregatedFilters,
  PesateAggregataResult,
  PesateCurvaAccrescimentoResult,
} from '../models/pesate-accrescimento.model';

/**
 * HTTP service for DS06 and DS07 endpoints (pesate / curva di accrescimento).
 *
 * DS06: GET /api/v1/animals/weighing-curves
 * DS07: GET /api/v1/animals/weighing-curves/export
 *
 * See UIDS001 — sections "Griglia Metriche Accrescimento" and "Export Dati Pesate".
 */
@Injectable({
  providedIn: 'root',
})
export class PesateAccrescimentoService {
  private readonly baseUrl: string;

  constructor(
    private http: HttpClient,
    @Optional() @Inject(NETCORE6_API_BASE_URL) baseUrl?: string
  ) {
    this.baseUrl = baseUrl ?? '';
  }

  /**
   * Retrieves the paged weighing metrics dataset.
   * Invoked on filter change, page change, or sort change.
   * See UIDS001 — "Griglia Metriche Accrescimento — Pagina Dedicata (Fase 1)".
   */
  getPesateCurveAccrescimento(
    filters: PesateAccrescimentoFilters,
    context?: HttpContext
  ): Observable<PesateCurvaAccrescimentoResult> {
    const params = this.buildQueryParams(filters);

    return this.http
      .get<{ RispostaOK: boolean; RispostaStringa: string }>(
        `${this.baseUrl}/api/v1/animals/weighing-curves`,
        { params, context }
      )
      .pipe(
        map((r) => {
          if (!r.RispostaOK) throw new Error('Risposta non valida dal server');
          return JSON.parse(r.RispostaStringa) as PesateCurvaAccrescimentoResult;
        })
      );
  }

  /**
   * Downloads the export file (CSV or XLS) as a Blob.
   * See UIDS001 — "Export Dati Pesate — Pagina Dedicata (Fase 1)".
   *
   * @param format 'CSV' | 'XLS'
   * @param viewMode 'detailed' | 'aggregated'
   * @param aggregationLevel 'animal' | 'razza_key' | 'stalla_key' (required when aggregated)
   */
  exportPesate(
    filters: PesateAccrescimentoFilters,
    format: 'CSV' | 'XLS' = 'CSV',
    viewMode: 'detailed' | 'aggregated' = 'detailed',
    aggregationLevel?: string,
    includeMetadata: boolean = true,
    columns?: string,
    context?: HttpContext
  ): Observable<Blob> {
    let params = this.buildQueryParams(filters);
    params = params
      .set('format', format)
      .set('view_mode', viewMode)
      .set('include_metadata', String(includeMetadata));

    if (aggregationLevel) {
      params = params.set('aggregation_level', aggregationLevel);
    }
    if (columns) {
      params = params.set('columns', columns);
    }

    return this.http.get(`${this.baseUrl}/api/v1/animals/weighing-curves/export`, {
      params,
      responseType: 'blob',
      context,
    });
  }

  // ── Private helpers ──────────────────────────────────────────────────────────────────────────

  /**
   * Retrieves the aggregated weighing dataset for the dashboard widget.
   * See UIDS001 — "Widget Pesate e Accrescimento (Fase 2)".
   */
  getAggregatedPesate(
    filters: PesateAggregatedFilters,
    context?: HttpContext
  ): Observable<PesateAggregataResult> {
    const params = this.buildAggregatedQueryParams(filters);

    return this.http
      .get<{ RispostaOK: boolean; RispostaStringa: string }>(
        `${this.baseUrl}/api/v1/animals/weighing-curves`,
        { params, context }
      )
      .pipe(
        map((r) => {
          if (!r.RispostaOK) throw new Error('Risposta non valida dal server');
          return JSON.parse(r.RispostaStringa) as PesateAggregataResult;
        })
      );
  }

  // ── Private helpers ──────────────────────────────────────────────────────────────────────────

  private buildQueryParams(filters: PesateAccrescimentoFilters): HttpParams {
    let params = new HttpParams();

    params = params.set('piva', filters.piva);

    if (filters.stalla_key) {
      params = params.set('stalla_key', filters.stalla_key);
    }
    if (filters.razza_key?.length) {
      params = params.set('razza_key', filters.razza_key.join(','));
    }
    if (filters.date_from) {
      params = params.set('date_from', filters.date_from.toISOString());
    }
    if (filters.date_to) {
      params = params.set('date_to', filters.date_to.toISOString());
    }

    params = params
      .set('view_mode', 'detailed')
      .set('kg_per_day', String(filters.kg_per_day))
      .set('alert_threshold_pct', String(filters.alert_threshold_pct))
      .set('page', String(filters.page))
      .set('limit', String(filters.limit))
      .set('sort_by', filters.sort_by)
      .set('sort_order', filters.sort_order);

    return params;
  }

  private buildAggregatedQueryParams(filters: PesateAggregatedFilters): HttpParams {
    return new HttpParams()
      .set('piva', filters.piva)
      .set('view_mode', 'aggregated')
      .set('aggregation_level', filters.aggregation_level)
      .set('sort_by', 'data_pesata')
      .set('sort_order', 'asc');
  }
}
