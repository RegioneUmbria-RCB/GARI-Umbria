import { AfterViewInit, Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService, GiasMessageService } from 'gias-kendo-grid';
import { GiasSideFiltersTemplateComponent } from 'gias-ui-kit';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { RischiMeteoService } from '../../services/rischi-meteo.service';
import { FiltersRischiMeteoService } from '../../services/filters-rischi-meteo.service';
import { RiepilogoRischiGridConfigService } from '../../services/riepilogo-rischi-grid-config.service';
import { CalcoloRischiRequest } from '../../models/rischi-meteo.model';

@Component({
  standalone: false,
  selector: 'app-calcolo-rischi-page',
  templateUrl: './calcolo-rischi-page.component.html',
  styleUrls: ['./calcolo-rischi-page.component.scss'],
  providers: [
    ...generateGridProviders(RiepilogoRischiGridConfigService, CalcoloRischiPageComponent),
    FiltersRischiMeteoService
  ]
})
export class CalcoloRischiPageComponent implements OnInit, AfterViewInit, OnDestroy {

  private destroy$ = new Subject<void>();

  @ViewChild(GiasSideFiltersTemplateComponent) private sideFilters!: GiasSideFiltersTemplateComponent;

  selectedRowKeys: string[] = [];
  isCalcoloInProgress = false;
  searchInitiated = false;
  hasData = false;

  private currentPivaFiliera: string | null = null;
  private successMessageShown = false;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridConfigService: RiepilogoRischiGridConfigService,
    private rischiMeteoService: RischiMeteoService,
    private filtersService: FiltersRischiMeteoService,
    private gridPublicService: GridPublicService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService
  ) {}

  ngOnInit(): void {
    this.filtersService.pivaFiliera$
      .pipe(takeUntil(this.destroy$))
      .subscribe(piva => {
        this.currentPivaFiliera = piva;
        this.selectedRowKeys = [];
        this.hasData = false;
        this.successMessageShown = false;

        if (!piva) {
          this.searchInitiated = false;
          return;
        }

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

  onSidebarSearch(): void {
    this.filtersService.requestApply();
  }

  onRowsLoaded(): void {
    const rows = this.gridConfigService.lastRows;
    this.hasData = rows.length > 0;

    // if (this.hasData && !this.successMessageShown) {
    //   this.successMessageShown = true;
    //   setTimeout(() => {
    //     this.giasMessageService.successMessage(
    //       this.translocoService.translate('rischim_success_perimetro')
    //     );
    //   });
    // }
  }

  onSelectionChange(event: SelectionEvent): void {
    if (event.selectedRows) {
      event.selectedRows.forEach(row => {
        const key = row.dataItem?.rowKey as string | undefined;
        if (key && !this.selectedRowKeys.includes(key)) {
          this.selectedRowKeys = [...this.selectedRowKeys, key];
        }
      });
    }

    if (event.deselectedRows) {
      event.deselectedRows.forEach(row => {
        const key = row.dataItem?.rowKey as string | undefined;
        if (key) {
          this.selectedRowKeys = this.selectedRowKeys.filter(k => k !== key);
        }
      });
    }
  }

  get canValutaRischi(): boolean {
    return this.hasData && !this.isCalcoloInProgress && this.selectedRowKeys.length > 0;
  }

  onValutaRischi(): void {
    const title = this.translocoService.translate('rischim_dialog_conferma_title');
    const message = this.translocoService.translate('rischim_dialog_conferma_message');

    this.giasDialogService.dialogMessageObs_Result(title, message)
      .pipe(takeUntil(this.destroy$))
      .subscribe((result: any) => {
        if (result && result.returnObj) {
          this.avviaCalcolo();
        }
      });
  }

  private avviaCalcolo(): void {
    const selectedRows = this.gridConfigService.lastRows.filter(r =>
      this.selectedRowKeys.includes(r.rowKey)
    );

    const payload: CalcoloRischiRequest = {
      PivaFiliera: this.currentPivaFiliera,
      Perimetro: selectedRows.map(r => ({
        piva_azienda: r.piva_azienda,
        nome_azienda: r.nome_azienda,
        id_appezzamento: r.id_appezzamento,
        nome_appezzamento: r.nome_appezzamento,
        id_esercizio: r.id_esercizio,
        id_impianto: r.id_impianto,
        nazione: r.nazione,
        regione: r.regione,
        istat_reg: r.istat_reg,
        superficie_ha: r.superficie_ha,
        cod_specie: r.cod_specie,
        nome_specie: r.nome_specie,
        cod_varieta: r.cod_varieta,
        nome_varieta: r.nome_varieta,
        data_inizio_esercizio: this.toDateOnly(r.data_inizio_esercizio),
        data_fine_esercizio:   this.toDateOnly(r.data_fine_esercizio),
        nome_esercizio: r.nome_esercizio,
        sa_cod: r.sa_cod
      }))
    };

    this.isCalcoloInProgress = true;
    this.rischiMeteoService.avviaCalcoloRischi(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.isCalcoloInProgress = false;
          if (!response) { return; }

          const totale = response.TotaleEsercizi;
          const errori = response.TotaleErrori + (response.ErroriCostruzione?.length ?? 0);

          if (errori === 0) {
            this.giasMessageService.successMessage(
              this.translocoService.translate('rischim_success_calcolo')
            );
          } else if (errori < totale) {
            this.giasMessageService.warningMessage(
              this.translocoService.translate('rischim_warning_calcolo_parziale', { errori, totale })
            );
          } else {
            this.giasMessageService.errorMessage(
              this.translocoService.translate('rischim_error_calcolo_fallito', { totale })
            );
          }
        },
        error: () => {
          this.isCalcoloInProgress = false;
        }
      });
  }

  /** Normalizza una data in qualsiasi formato a 'YYYY-MM-DD'.
   *  Gestisce: Date, 'dd/MM/yyyy HH:mm:ss', 'dd/MM/yyyy', 'yyyy-MM-ddTHH:mm:ss', 'yyyy-MM-dd' */
  private toDateOnly(value: string | Date | null | undefined): string {
    if (!value) { return ''; }
    if (value instanceof Date) {
      const yyyy = value.getFullYear();
      const mm = String(value.getMonth() + 1).padStart(2, '0');
      const dd = String(value.getDate()).padStart(2, '0');
      return `${yyyy}-${mm}-${dd}`;
    }
    if (value.includes('/')) {
      // formato italiano: dd/MM/yyyy ...
      const [datePart] = value.split(' ');
      const [dd, mm, yyyy] = datePart.split('/');
      return `${yyyy}-${mm}-${dd}`;
    }
    // formato ISO: yyyy-MM-ddT...
    return value.split('T')[0];
  }
}
