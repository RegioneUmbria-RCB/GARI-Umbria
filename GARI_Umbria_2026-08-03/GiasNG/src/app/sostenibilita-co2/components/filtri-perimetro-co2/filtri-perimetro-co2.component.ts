import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { catchError, takeUntil } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { GiasMessageService } from 'gias-kendo-grid';

import { ColturaCO2Item, FiliereCO2DisplayItem, FiliereCO2Item, ModalitaCalcolo } from '../../models/sostenibilita-co2.model';
import { FiltersPerimetroService } from '../../services/filters-perimetro.service';
import { PerimetroCO2Service } from '../../services/perimetro-co2.service';

@Component({
  standalone: false,
  selector: 'app-filtri-perimetro-co2',
  templateUrl: './filtri-perimetro-co2.component.html',
  styleUrls: ['./filtri-perimetro-co2.component.scss'],
  providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService]
})
export class FiltriPerimetroComponent implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();

  form: FormGroup;

  filiereList: FiliereCO2DisplayItem[] = [];
  filiereListAll: FiliereCO2Item[] = [];
  filiereCaricate = false;

  coltureList: ColturaCO2Item[] = [];

  anniList: number[] = [];

  tutteLeColturePlaceholder: ColturaCO2Item = { VegCod: -1, VegDes: '' };

  constructor(
    private fb: FormBuilder,
    private perimetroCO2Service: PerimetroCO2Service,
    private filtersPerimetroService: FiltersPerimetroService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService
  ) {}

  ngOnInit(): void {
    this.tutteLeColturePlaceholder = {
      VegCod: -1,
      VegDes: this.translocoService.translate('sco2_label_tutte_colture')
    };

    this.initAnni();
    this.buildForm();

    this.form.get('filiera').valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(piva => {
        this.form.get('coltura').reset([]);
        this.coltureList = [];
        if (piva) {
          this.loadColtureByFiliera(piva);
        }
      });

    this.form.get('modalita').valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((aziendale: boolean) => {
        const colturaCtrl = this.form.get('coltura');
        if (aziendale) {
          colturaCtrl.disable();
          colturaCtrl.setValue([this.tutteLeColturePlaceholder.VegDes]);
        } else {
          colturaCtrl.enable();
          colturaCtrl.setValue([]);
        }
      });

    this.filtersPerimetroService.applyRequest$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.applyFilters());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get isPerAzienda(): boolean {
    return this.form?.get('modalita')?.value === true;
  }

  get canApplyFilters(): boolean {
    if (!this.form) { return false; }
    const { filiera, anno } = this.form.getRawValue();
    return !!filiera && !!anno;
  }

  onFiliereOpen(): void {
    if (!this.filiereCaricate) {
      this.perimetroCO2Service.loadFiliere()
        .pipe(
          takeUntil(this.destroy$),
          catchError(() => {
            this.giasMessageService.errorMessage(
              this.translocoService.translate('sco2_error_message')
            );
            return EMPTY;
          })
        )
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
    const modalita: ModalitaCalcolo = raw.modalita ? 'Aziendale' : 'Colture';
    const coltureSelezionate: string[] =
      raw.modalita
        ? []
        : (raw.coltura as ColturaCO2Item[])?.map(c => c.VegDes) ?? [];

    this.filtersPerimetroService.publishFilters({
      filiera: raw.filiera,
      modalita,
      coltura: coltureSelezionate,
      anno: raw.anno
    });
  }

  resetFilters(): void {
    const anno = this.getCurrentYear();
    this.form.reset({
      modalita: false,
      filiera: null,
      anno,
      coltura: []
    });
    this.coltureList = [];
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
    const anno = this.getCurrentYear();
    this.form = this.fb.group({
      modalita: [false],
      filiera: [null, Validators.required],
      anno: [anno, Validators.required],
      coltura: [[]]
    });
  }

  private initAnni(): void {
    const currentYear = this.getCurrentYear();
    this.anniList = [];
    for (let y = currentYear; y >= currentYear - 10; y--) {
      this.anniList.push(y);
    }
  }

  private getCurrentYear(): number {
    return new Date().getFullYear();
  }

  private loadColtureByFiliera(piva: string): void {
    this.perimetroCO2Service.loadColtureByFiliera(piva)
      .pipe(takeUntil(this.destroy$))
      .subscribe(colture => {
        this.coltureList = colture;
      });
  }
}
