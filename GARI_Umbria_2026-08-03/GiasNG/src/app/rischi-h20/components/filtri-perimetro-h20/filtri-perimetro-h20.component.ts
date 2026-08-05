import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService } from 'gias-ui-kit';

import { ColturaH20Item, FiliereH20Item, ModalitaCalcolo } from '../../models/rischi-h20.model';
import { FiltersPerimetroH20Service } from '../../services/filters-perimetro-h20.service';
import { PerimetroH20Service } from '../../services/perimetro-h20.service';

@Component({
  standalone: false,
  selector: 'app-filtri-perimetro-h20',
  templateUrl: './filtri-perimetro-h20.component.html',
  styleUrls: ['./filtri-perimetro-h20.component.scss'],
  providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService]
})
export class FiltriPerimetroH20Component implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();

  form: FormGroup;

  filiereList: FiliereH20Item[] = [];
  filiereListAll: FiliereH20Item[] = [];
  filiereCaricate = false;

  coltureList: ColturaH20Item[] = [];

  anniList: number[] = [];

  tutteLeColturePlaceholder: ColturaH20Item = { VegCod: -1, VegDes: '' };

  constructor(
    private fb: FormBuilder,
    private perimetroH20Service: PerimetroH20Service,
    private filtersPerimetroH20Service: FiltersPerimetroH20Service,
    private translocoService: TranslocoService
  ) {}

  ngOnInit(): void {
    this.tutteLeColturePlaceholder = {
      VegCod: -1,
      VegDes: this.translocoService.translate('rischi_h2o_label_tutte_colture')
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

    this.filtersPerimetroH20Service.applyRequest$
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
      this.perimetroH20Service.loadFiliere()
        .pipe(takeUntil(this.destroy$))
        .subscribe(filiere => {
          this.filiereList = filiere;
          this.filiereListAll = filiere;
          this.filiereCaricate = true;
        });
    }
  }

  handleFiliereFilter(value: string): void {
    const q = value?.toLowerCase() ?? '';
    this.filiereList = this.filiereListAll.filter(
      f => f.RagioneSociale.toLowerCase().includes(q) || f.Piva.toLowerCase().includes(q)
    );
  }

  applyFilters(): void {
    if (!this.canApplyFilters) { return; }

    const raw = this.form.getRawValue();
    const modalita: ModalitaCalcolo = raw.modalita ? 'Aziendale' : 'Colture';
    const coltureSelezionate: string[] =
      raw.modalita
        ? []
        : (raw.coltura as ColturaH20Item[])?.map(c => c.VegDes) ?? [];

    this.filtersPerimetroH20Service.publishFilters({
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
    this.perimetroH20Service.loadColtureByFiliera(piva)
      .pipe(takeUntil(this.destroy$))
      .subscribe(colture => {
        this.coltureList = colture;
      });
  }
}
