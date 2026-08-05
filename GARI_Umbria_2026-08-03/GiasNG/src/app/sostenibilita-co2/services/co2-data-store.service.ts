import { Injectable } from '@angular/core';
import {
  AziendaPerimetro,
  CarburantiCO2Model,
  EnergiaCO2Model,
  PerimetroFilters,
  RiepilogoPerimetroRow,
  TokenCreationFilters,
  TokenGenerabileRow
} from '../models/sostenibilita-co2.model';

@Injectable({ providedIn: 'root' })
export class Co2DataStoreService {

  private _selectedRows: RiepilogoPerimetroRow[] = [];
  private _currentFilters: PerimetroFilters | null = null;
  private _carburanti: CarburantiCO2Model[] = [];
  private _energia: EnergiaCO2Model[] = [];

  // ── perimetro ───────────────────────────────────────────────────────────────

  setPerimetro(rows: RiepilogoPerimetroRow[], filters: PerimetroFilters): void {
    this._selectedRows = rows;
    this._currentFilters = filters;
    this._carburanti = [];
    this._energia = [];
  }

  get selectedRows(): RiepilogoPerimetroRow[] {
    return this._selectedRows;
  }

  get currentFilters(): PerimetroFilters | null {
    return this._currentFilters;
  }

  get aziende(): AziendaPerimetro[] {
    const seen = new Set<string>();
    return this._selectedRows
      .filter(r => {
        if (seen.has(r.piva)) { return false; }
        seen.add(r.piva);
        return true;
      })
      .map(r => ({ azienda: r.azienda, piva: r.piva }));
  }

  // ── carburanti snapshot (written by CarburantiGridConfigService) ─────────────

  setCarburantiSnapshot(rows: CarburantiCO2Model[]): void {
    this._carburanti = rows;
  }

  get carburanti(): CarburantiCO2Model[] {
    return this._carburanti;
  }

  // ── energia snapshot (written by EnergiaGridConfigService) ───────────────────

  setEnergiaSnapshot(rows: EnergiaCO2Model[]): void {
    this._energia = rows;
  }

  get energia(): EnergiaCO2Model[] {
    return this._energia;
  }

  // ── reset (after submission) ────────────────────────────────────────────────

  reset(): void {
    this._carburanti = [];
    this._energia = [];
  }

  // ── FS2.07.1 — Token Creation selection ─────────────────────────────────────

  private _selectedTokens: TokenGenerabileRow[] = [];
  private _tokenFilters: TokenCreationFilters | null = null;

  setTokenSelections(tokens: TokenGenerabileRow[], filters: TokenCreationFilters): void {
    this._selectedTokens = tokens;
    this._tokenFilters = filters;
  }

  get selectedTokens(): TokenGenerabileRow[] {
    return this._selectedTokens;
  }

  get tokenFilters(): TokenCreationFilters | null {
    return this._tokenFilters;
  }
}
