import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { FiliereCO2Item } from 'app/sostenibilita-co2/models/sostenibilita-co2.model';
import { RischiMeteoService } from '../../services/rischi-meteo.service';
import { FiltersRischiMeteoService } from '../../services/filters-rischi-meteo.service';

@Component({
  standalone: false,
  selector: 'app-filtri-calcolo-rischi',
  templateUrl: './filtri-calcolo-rischi.component.html',
  styleUrls: ['./filtri-calcolo-rischi.component.scss'],
  providers: [GiasDropDownTemplateService]
})
export class FiltriCalcoloRischiComponent implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();

  form: FormGroup;

  filiereList: FiliereCO2Item[] = [];
  filiereListAll: FiliereCO2Item[] = [];
  filiereCaricate = false;

  constructor(
    private fb: FormBuilder,
    private rischiMeteoService: RischiMeteoService,
    private filtersService: FiltersRischiMeteoService
  ) {}

  ngOnInit(): void {
    this.buildForm();

    this.filtersService.applyRequest$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.applyFilters());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onFiliereOpen(): void {
    if (!this.filiereCaricate) {
      this.rischiMeteoService.loadFiliere()
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
    const piva: string | null = this.form.get('filiera')?.value ?? null;
    this.filtersService.publishFiliera(piva);
  }

  resetFilters(): void {
    this.form.reset({ filiera: null });
  }

  private buildForm(): void {
    this.form = this.fb.group({
      filiera: [null, Validators.required]
    });
  }
}
