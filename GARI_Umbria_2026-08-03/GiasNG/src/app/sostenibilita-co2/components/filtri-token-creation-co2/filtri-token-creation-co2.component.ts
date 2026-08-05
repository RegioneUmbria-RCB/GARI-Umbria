import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDropDownTemplateService } from 'gias-ui-kit';

import { FiliereCO2DisplayItem, FiliereCO2Item } from '../../models/sostenibilita-co2.model';
import { FiltersTokenCreationCo2Service } from '../../services/filters-token-creation-co2.service';
import { PerimetroCO2Service } from '../../services/perimetro-co2.service';

@Component({
  standalone: false,
  selector: 'app-filtri-token-creation-co2',
  templateUrl: './filtri-token-creation-co2.component.html',
  styleUrls: ['./filtri-token-creation-co2.component.scss'],
  providers: [GiasDropDownTemplateService]
})
export class FiltriTokenCreationCo2Component implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();

  form: FormGroup;

  filiereList: FiliereCO2DisplayItem[] = [];
  filiereListAll: FiliereCO2Item[] = [];
  filiereCaricate = false;

  anniList: number[] = [];
  anniLoading = false;

  constructor(
    private fb: FormBuilder,
    private perimetroCO2Service: PerimetroCO2Service,
    private filtersTokenCreationCo2Service: FiltersTokenCreationCo2Service,
    private translocoService: TranslocoService
  ) {}

  ngOnInit(): void {
    this.buildForm();

    this.form.get('filiera').valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(piva => {
        this.form.get('anno').reset(null);
        this.anniList = [];
        if (piva) {
          this.loadAnniByFiliera(piva);
        }
      });

    this.filtersTokenCreationCo2Service.applyRequest$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.applyFilters());

    this.restoreLastFilters();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get canApplyFilters(): boolean {
    if (!this.form) { return false; }
    const { filiera, anno } = this.form.getRawValue();
    return !!filiera && !!anno;
  }

  onFiliereOpen(): void {
    if (!this.filiereCaricate) {
      this.perimetroCO2Service.loadFiliere()
        .pipe(takeUntil(this.destroy$))
        .subscribe(filiere => {
          this.filiereListAll = filiere;
          this.filiereList = this.buildFiliereDisplayList(filiere);
          this.filiereCaricate = true;
        });
    }
  }

  handleFiliereFilter(value: string): void {
    const q = value?.toLowerCase() ?? '';
    if (!q) {
      this.filiereList = this.buildFiliereDisplayList(this.filiereListAll);
      return;
    }
    const directMatches = this.filiereListAll.filter(
      f => f.RagioneSociale.toLowerCase().includes(q) || f.Piva.toLowerCase().includes(q)
    );
    const expanded = new Map<string, FiliereCO2Item>();
    for (const f of directMatches) {
      expanded.set(f.Piva, f);
      // se è un padre → aggiungi tutti i figli
      this.filiereListAll.filter(c => c.Padre === f.Piva).forEach(c => expanded.set(c.Piva, c));
      // se è un figlio → aggiungi il padre
      if (f.Padre) {
        const parent = this.filiereListAll.find(p => p.Piva === f.Padre);
        if (parent) { expanded.set(parent.Piva, parent); }
      }
    }
    this.filiereList = this.buildFiliereDisplayList([...expanded.values()]);
  }

  applyFilters(): void {
    if (!this.canApplyFilters) { return; }

    const raw = this.form.getRawValue();
    const filieraItem = this.filiereListAll.find(f => f.Piva === raw.filiera);
    this.filtersTokenCreationCo2Service.publishFilters({
      filiera: raw.filiera,
      filieraLabel: filieraItem?.RagioneSociale ?? raw.filiera,
      anno: raw.anno
    });
  }

  resetFilters(): void {
    this.form.reset({ filiera: null, anno: null });
    this.anniList = [];
  }

  private buildFiliereDisplayList(rawItems: FiliereCO2Item[]): FiliereCO2DisplayItem[] {
    const allParentPivas = new Set(
      this.filiereListAll.filter(f => !!f.Padre).map(f => f.Padre)
    );

    const standalone = rawItems
      .filter(f => !f.Padre && !allParentPivas.has(f.Piva))
      .sort((a, b) => a.RagioneSociale.localeCompare(b.RagioneSociale, 'it'));

    const groupPivasNeeded = new Set<string>();
    rawItems.filter(f => !!f.Padre).forEach(f => groupPivasNeeded.add(f.Padre));
    rawItems.filter(f => !f.Padre && allParentPivas.has(f.Piva)).forEach(f => groupPivasNeeded.add(f.Piva));

    const result: FiliereCO2DisplayItem[] = standalone.map(f => ({
      Piva: f.Piva,
      RagioneSociale: f.RagioneSociale
    }));

    const sortedGroups = [...groupPivasNeeded]
      .map(piva => this.filiereListAll.find(f => f.Piva === piva))
      .filter(Boolean)
      .sort((a, b) => a.RagioneSociale.localeCompare(b.RagioneSociale, 'it'));

    for (const parent of sortedGroups) {
      result.push({ Piva: parent.Piva, RagioneSociale: parent.RagioneSociale });
      rawItems
        .filter(f => f.Padre === parent.Piva)
        .sort((a, b) => a.RagioneSociale.localeCompare(b.RagioneSociale, 'it'))
        .forEach(child => result.push({ Piva: child.Piva, RagioneSociale: child.RagioneSociale }));
    }

    return result;
  }

  private buildForm(): void {
    this.form = this.fb.group({
      filiera: [null, Validators.required],
      anno: [{ value: null, disabled: true }, Validators.required]
    });
  }

  private restoreLastFilters(): void {
    const last = this.filtersTokenCreationCo2Service.lastFilters;
    if (!last) { return; }

    this.perimetroCO2Service.loadFiliere()
      .pipe(takeUntil(this.destroy$))
      .subscribe(filiere => {
        this.filiereListAll = filiere;
        this.filiereList = this.buildFiliereDisplayList(filiere);
        this.filiereCaricate = true;
        this.form.get('filiera').setValue(last.filiera, { emitEvent: false });
        this.loadAnniByFiliera(last.filiera, last.anno);
      });
  }

  private loadAnniByFiliera(piva: string, restoreAnno?: number): void {
    this.anniLoading = true;
    this.form.get('anno').disable();
    this.perimetroCO2Service.getAnniLookup(piva)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: anni => {
          this.anniList = anni.sort((a, b) => b - a);
          this.form.get('anno').enable();
          if (restoreAnno != null) {
            this.form.get('anno').setValue(restoreAnno);
          }
          this.anniLoading = false;
        },
        error: () => {
          this.anniList = [];
          this.anniLoading = false;
        }
      });
  }
}

