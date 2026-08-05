import { AfterViewInit, Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService } from 'gias-kendo-grid';
import { GiasMessageService } from 'gias-kendo-grid';
import { GiasSideFiltersTemplateComponent } from 'gias-ui-kit';
import { GiasDialogService } from 'app/Service/gias-dialog.service';

import { FiltersPerimetroH20Service } from '../../services/filters-perimetro-h20.service';
import { RiepilogoPerimetroH20GridConfigService } from '../../services/riepilogo-perimetro-h20-grid-config.service';
import { PerimetroH20Service } from '../../services/perimetro-h20.service';
import { CalcoloH20Request, EsercizioH20Payload, PerimetroFilters, PerimetroH20Payload } from '../../models/rischi-h20.model';

@Component({
  standalone: false,
  selector: 'app-selezione-perimetro-h20',
  templateUrl: './selezione-perimetro-h20.component.html',
  styleUrls: ['./selezione-perimetro-h20.component.scss'],
  providers: [
    ...generateGridProviders(RiepilogoPerimetroH20GridConfigService, SelezionePerimetroH20Component)
  ]
})
export class SelezionePerimetroH20Component implements OnInit, AfterViewInit, OnDestroy {

  private destroy$ = new Subject<void>();

  @ViewChild(GiasSideFiltersTemplateComponent) private sideFilters!: GiasSideFiltersTemplateComponent;

  selectedRowKeys: string[] = [];

  isLoading = false;
  isCalcoloInProgress = false;
  hasData = false;
  searchInitiated = false;
  currentFilters: PerimetroFilters | null = null;

  private successMessageShown = false;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridConfigService: RiepilogoPerimetroH20GridConfigService,
    private filtersPerimetroH20Service: FiltersPerimetroH20Service,
    private perimetroH20Service: PerimetroH20Service,
    private gridPublicService: GridPublicService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService
  ) {}

  ngOnInit(): void {
    this.filtersPerimetroH20Service.filters$
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => {
        this.currentFilters = filters;
        this.successMessageShown = false;
        const wasAlreadyInitiated = this.searchInitiated;
        this.searchInitiated = true;

        if (wasAlreadyInitiated) {
          this.gridPublicService.refresh(true);
        }
      });
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.sideFilters?.onOpenFilters());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get isPerAzienda(): boolean {
    return this.currentFilters?.modalita === 'Aziendale';
  }

  onRowsLoaded(): void {
    this.isLoading = false;
    const rows = this.gridConfigService.lastRows;
    this.hasData = rows.length > 0;
    this.selectedRowKeys = [];

    // if (this.hasData && !this.successMessageShown) {
    //   this.successMessageShown = true;
    //   setTimeout(() => {
    //     this.giasMessageService.successMessage(
    //       this.translocoService.translate('rischi_h2o_success_perimetro')
    //     );
    //   });
    // }
  }

  onSelectionChange(event: SelectionEvent): void {
    if (event.selectedRows) {
      event.selectedRows.forEach(row => {
        const key = row.dataItem?.rowKey;
        if (key && !this.selectedRowKeys.includes(key)) {
          this.selectedRowKeys = [...this.selectedRowKeys, key];
        }
      });
    }

    if (event.deselectedRows) {
      event.deselectedRows.forEach(row => {
        const key = row.dataItem?.rowKey;
        if (key) {
          this.selectedRowKeys = this.selectedRowKeys.filter(k => k !== key);
        }
      });
    }
  }

  onSidebarSearch(): void {
    this.filtersPerimetroH20Service.requestApply();
  }

  get canAvviaCalcolo(): boolean {
    return this.hasData && !this.isLoading && !this.isCalcoloInProgress && this.selectedRowKeys.length > 0;
  }

  onAvviaCalcolo(): void {
    const title = this.translocoService.translate('rischi_h2o_dialog_avvia_title');
    const message = this.translocoService.translate('rischi_h2o_dialog_avvia_message');

    this.giasDialogService.dialogMessageObs_Result(title, message)
      .pipe(takeUntil(this.destroy$))
      .subscribe((result: any) => {
        if (result && result.returnObj) {
          this.eseguiCalcolo();
        }
      });
  }

  private eseguiCalcolo(): void {
    const selectedRows = this.gridConfigService.lastRows.filter(
      r => this.selectedRowKeys.includes(r.rowKey)
    );
    const filters = this.currentFilters;

    const perimetro: PerimetroH20Payload = {
      Aziende: [...new Set(selectedRows.map(r => r.piva))],
      Filiera: filters?.filiera ?? '',
      Anno: filters?.anno ?? 0,
      Colture: filters?.coltura ?? [],
      Modalita: filters?.modalita ?? 'Colture'
    };

    const esercizi: EsercizioH20Payload[] = selectedRows.map(r => ({
      id_esercizio: r.id_esercizio,
      nazione: r.nazione,
      istat_reg: r.istat_reg
    }));

    const payload: CalcoloH20Request = {
      Perimetro: perimetro,
      Esercizi: esercizi
    };

    this.isCalcoloInProgress = true;
    this.perimetroH20Service.avviaCalcoloH20(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.isCalcoloInProgress = false;
          if (!response?.RispostaOK) {
            const errore = (response?.Errore || '').split('   at ')[0].trim()
              || this.translocoService.translate('rischi_h2o_error_calcolo_generico');
            this.giasMessageService.errorMessage(errore);
            return;
          }
          this.giasMessageService.successMessage(
            this.translocoService.translate('rischi_h2o_success_calcolo')
          );
        },
        error: () => {
          this.isCalcoloInProgress = false;
          this.giasMessageService.errorMessage(
            this.translocoService.translate('rischi_h2o_error_calcolo_generico')
          );
        }
      });
  }
}
