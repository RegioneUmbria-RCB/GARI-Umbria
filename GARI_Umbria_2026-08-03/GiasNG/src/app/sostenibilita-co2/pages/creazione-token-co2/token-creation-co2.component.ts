import { AfterViewInit, Component, Inject, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Subject, from } from 'rxjs';
import { catchError, concatMap, map, takeUntil } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GRID_HTTP_TOKEN, GiasKendoGridComponent, GridPublicService } from 'gias-kendo-grid';
import { GiasMessageService } from 'gias-kendo-grid';
import { GiasSideFiltersTemplateComponent } from 'gias-ui-kit';
import { Dialog_Type, GiasDialogAction, GiasDialogService } from 'app/Service/gias-dialog.service';

import { FiltersTokenCreationCo2Service } from '../../services/filters-token-creation-co2.service';
import { TokenCreationCo2GridConfigService } from '../../services/token-creation-data-co2-grid-config.service';
import { TokenCo2Service } from '../../services/token-co2.service';
import { CreaTokenBlockchainRequest, TokenCreationFilters, TokenGenerabileRow } from '../../models/sostenibilita-co2.model';

interface TokenProcessingResult {
  aziendaLabel: string;
  success: boolean;
  token?: string;
  errorMessage?: string;
}

@Component({
  standalone: false,
  selector: 'app-token-creation-co2',
  templateUrl: './token-creation-co2.component.html',
  styleUrls: ['./token-creation-co2.component.css'],
  providers: [
    ...generateGridProviders(TokenCreationCo2GridConfigService, TokenCreationCo2Component)
  ]
})
export class TokenCreationCo2Component implements OnInit, AfterViewInit, OnDestroy {

  private destroy$ = new Subject<void>();

  @ViewChild(GiasSideFiltersTemplateComponent) private sideFilters!: GiasSideFiltersTemplateComponent;
  @ViewChild(GiasKendoGridComponent) private kendoGrid!: GiasKendoGridComponent;
  @ViewChild('confirmDialogTpl') private confirmDialogTpl!: TemplateRef<unknown>;
  @ViewChild('resultsDialogTpl') private resultsDialogTpl!: TemplateRef<unknown>;

  isLoading = false;
  hasError = false;
  hasData = false;
  searchInitiated = false;
  isProcessingTokens = false;
  currentFilters: TokenCreationFilters | null = null;

  // Data for dialog templates
  confirmTokens: TokenGenerabileRow[] = [];
  confirmMessage = '';
  processingResults: TokenProcessingResult[] = [];
  allResultsSuccess = false;

  private successMessageShown = false;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridConfigService: TokenCreationCo2GridConfigService,
    private filtersTokenCreationCo2Service: FiltersTokenCreationCo2Service,
    private gridPublicService: GridPublicService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private tokenCo2Service: TokenCo2Service
  ) {}

  ngOnInit(): void {
    this.filtersTokenCreationCo2Service.filters$
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => {
        this.currentFilters = filters;
        this.hasError = false;
        this.successMessageShown = false;
        this.gridConfigService.clearSelection();
        this.searchInitiated = true;
        this.gridPublicService.refresh(true);
      });
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.sideFilters?.onOpenFilters());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onRowsLoaded(): void {
    this.isLoading = false;
    const rows = this.gridConfigService.lastRows;
    this.hasData = rows.length > 0;

    if (this.hasData && !this.successMessageShown) {
      this.successMessageShown = true;
      setTimeout(() => {
        this.giasMessageService.successMessage(
          this.translocoService.translate('sco2_tc_success_loaded')
        );
      });
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onSelectionChange(_event: SelectionEvent): void { /* managed by TokenCreationCo2GridConfigService */ }

  onSidebarSearch(): void {
    this.filtersTokenCreationCo2Service.requestApply();
  }

  retryLoad(): void {
    if (this.currentFilters) {
      this.isLoading = true;
      this.hasError = false;
      this.gridPublicService.refresh(true);
    }
  }

  get canPredisponi(): boolean {
    return this.hasData && !this.isLoading && !this.isProcessingTokens && this.gridConfigService.selectionCount > 0;
  }

  onAnnulla(): void {
    this.gridConfigService.clearSelection();
    this.sideFilters?.onOpenFilters();
  }

  onPredisponi(): void {
    const selectedTokens = this.gridConfigService.lastRows.filter(
      (r: TokenGenerabileRow) => this.gridConfigService.selectedRowKeys.has(r.rowKey)
    );
    if (!selectedTokens.length || !this.currentFilters) {
      return;
    }

    const filieraLabel = this.currentFilters.filieraLabel ?? this.currentFilters.filiera ?? '';
    const title = this.translocoService.translate('sco2_tc_dialog_conferma_title');
    const count = selectedTokens.length;
    this.confirmMessage = this.translocoService.translate('sco2_tc_dialog_conferma_message', {
      count,
      filiera: filieraLabel,
      anno: this.currentFilters.anno
    });
    this.confirmTokens = selectedTokens;
    const filters = this.currentFilters;

    this.giasDialogService.dialogMessageObs_Result(title, this.confirmDialogTpl, undefined, 580, 560)
      .pipe(takeUntil(this.destroy$))
      .subscribe((result: any) => {
        if (result?.returnObj) {
          this.processTokens(selectedTokens, filters);
        }
      });
  }

  private processTokens(tokens: TokenGenerabileRow[], filters: TokenCreationFilters): void {
    this.isProcessingTokens = true;
    this.processingResults = [];

    from(tokens).pipe(
      concatMap(token => {
        const request: CreaTokenBlockchainRequest = {
          Azienda: token.aziendaPiva,
          Filiera: filters.filiera,
          IdInvocazione: token.idInvocazione,
          PayloadLookupSostenibilitaCO2: token.jsonRisposta
        };
        return this.tokenCo2Service.creaTokenBlockchain(request).pipe(
          map((generatedToken: string) => ({ aziendaLabel: token.aziendaLabel, success: true, token: generatedToken } as TokenProcessingResult)),
          catchError((err: unknown) => [{ aziendaLabel: token.aziendaLabel, success: false, errorMessage: this.resolveErrorMessage(err) } as TokenProcessingResult])
        );
      }),
      takeUntil(this.destroy$)
    ).subscribe({
      next: r => this.processingResults.push(r),
      complete: () => {
        this.isProcessingTokens = false;
        this.showResultsDialog();
      },
      error: () => {
        this.isProcessingTokens = false;
      }
    });
  }

  private showResultsDialog(): void {
    this.allResultsSuccess = this.processingResults.every(r => r.success);
    const title = this.translocoService.translate('sco2_tc_dialog_risultati_title');
    const dialogType = this.allResultsSuccess ? Dialog_Type.success : Dialog_Type.warning;
    const actions = [new GiasDialogAction(this.translocoService.translate('Ok'), true, true)];
    this.giasDialogService.dialogMessageObs_Result(title, this.resultsDialogTpl, actions, 580, 560, undefined, dialogType)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.gridConfigService.clearSelection();
        this.kendoGrid?.deselectAllRows();
      });
  }

  private resolveErrorMessage(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 401 || err.status === 403) {
        return this.translocoService.translate('sco2_pt_error_401_403');
      }
      const backendMessage: string | undefined = err.error?.Errore;
      if (backendMessage) {
        return backendMessage;
      }
      if (err.status === 400) {
        return this.translocoService.translate('sco2_pt_error_400');
      }
      if (err.status === 429) {
        return this.translocoService.translate('sco2_pt_error_429');
      }
      if (err.status >= 500) {
        return this.translocoService.translate('sco2_pt_error_5xx');
      }
    }
    if (err instanceof Error && err.message) {
      return err.message;
    }
    return this.translocoService.translate('sco2_pt_error_generic');
  }
}

