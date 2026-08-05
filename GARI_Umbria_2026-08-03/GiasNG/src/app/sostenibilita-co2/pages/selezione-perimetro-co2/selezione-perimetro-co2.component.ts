import { AfterViewInit, Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService } from 'gias-kendo-grid';
import { GiasMessageService } from 'gias-kendo-grid';
import { GiasSideFiltersTemplateComponent } from 'gias-ui-kit';
import { GiasDialogService } from 'app/Service/gias-dialog.service';

import { FiltersPerimetroService } from '../../services/filters-perimetro.service';
import { RiepilogoPerimetroGridConfigService } from '../../services/riepilogo-perimetro-grid-config.service';
import { Co2DataStoreService } from '../../services/co2-data-store.service';
import { PerimetroFilters, RiepilogoPerimetroRow } from '../../models/sostenibilita-co2.model';

@Component({
  standalone: false,
  selector: 'app-selezione-perimetro-co2',
  templateUrl: './selezione-perimetro-co2.component.html',
  styleUrls: ['./selezione-perimetro-co2.component.scss'],
  providers: [
    ...generateGridProviders(RiepilogoPerimetroGridConfigService, SelezionePerimetroComponent)
  ]
})
export class SelezionePerimetroComponent implements OnInit, AfterViewInit, OnDestroy {

  private destroy$ = new Subject<void>();

  @ViewChild(GiasSideFiltersTemplateComponent) private sideFilters!: GiasSideFiltersTemplateComponent;

  selectedRowKeys: string[] = [];

  isLoading = false;
  hasData = false;
  searchInitiated = false;
  currentFilters: PerimetroFilters | null = null;

  private successMessageShown = false;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridConfigService: RiepilogoPerimetroGridConfigService,
    private filtersPerimetroService: FiltersPerimetroService,
    private gridPublicService: GridPublicService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private co2DataStore: Co2DataStoreService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.filtersPerimetroService.filters$
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => {
        this.currentFilters = filters;
        this.successMessageShown = false;
        const wasAlreadyInitiated = this.searchInitiated;
        this.searchInitiated = true;

        if (wasAlreadyInitiated) {
          // Grid already in DOM: refresh explicitly
          this.gridPublicService.refresh(true);
        }
        // else: grid will be created by *ngIf and call read() on its own ngOnInit
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

    if (this.hasData && this.isPerAzienda) {
      const keys = rows.map(r => r.rowKey);
      this.selectedRowKeys = keys;
      setTimeout(() => {
        this.gridPublicService.selection.setSelected.next({
          keys,
          usePartialMatch: false,
          resetPreviousSelection: true
        });
      });
    }

    // if (this.hasData && !this.successMessageShown) {
    //   this.successMessageShown = true;
    //   setTimeout(() => {
    //     this.giasMessageService.successMessage(
    //       this.translocoService.translate('sco2_success_perimetro')
    //     );
    //   });
    // }
  }

  onSelectionChange(event: SelectionEvent): void {
    if (this.isPerAzienda) {
      this.selectedRowKeys = this.gridConfigService.lastRows.map(r => r.rowKey);
      return;
    }

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
    this.filtersPerimetroService.requestApply();
  }

  get canProsegui(): boolean {
    return this.hasData && !this.isLoading && this.selectedRowKeys.length > 0;
  }

  onProsegui(): void {
    const title = this.translocoService.translate('sco2_dialog_conferma_title');
    const message = this.translocoService.translate('sco2_dialog_conferma_message');

    this.giasDialogService.dialogMessageObs_Result(title, message)
      .pipe(takeUntil(this.destroy$))
      .subscribe((result: any) => {
        if (result && result.returnObj) {
          const selectedRows = this.gridConfigService.lastRows.filter(
            r => this.selectedRowKeys.includes(r.rowKey)
          );
          this.co2DataStore.setPerimetro(selectedRows, this.currentFilters);
          this.router.navigate(['SostenibitaCO2', 'GestioneCO2']);
        }
      });
  }
}
