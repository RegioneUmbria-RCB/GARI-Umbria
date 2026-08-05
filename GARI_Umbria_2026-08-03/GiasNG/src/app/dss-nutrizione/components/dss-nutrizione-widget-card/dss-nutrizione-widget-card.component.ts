import {
  ChangeDetectorRef,
  Component,
  computed, ElementRef,
  Input,
  OnDestroy,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { drawDOM, exportImage, exportPDF } from '@progress/kendo-drawing';
import { saveAs } from '@progress/kendo-file-saver';
import { Subject, takeUntil } from 'rxjs';
import { faGlobe, faLeaf, faRotate, faCalendar, faFloppyDisk, faPrint, faChartLine } from '@fortawesome/free-solid-svg-icons';
import { DssNutrizioneService } from '../../services/dss-nutrizione.service';
import {
  AggregazioneConsiglioNutrizioneResult,
  AppezzamentoNutrizioneDto,
  FaseFenologicaCorrenteDto,
  SalvaConsiglioNutrizioneRequest,
  WidgetCardStatus,
} from '../../models/dss-nutrizione.model';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasPanelBar } from 'gias-ui-kit';

/**
 * Widget card component for a single appezzamento.
 * Loads its consiglio nutrizionale independently and asynchronously.
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento (UIL002 §2, UIL002 §3, UIL001 §3)
 */
@Component({
  standalone: false,
  selector: 'app-dss-nutrizione-widget-card',
  templateUrl: './dss-nutrizione-widget-card.component.html',
  styleUrls: ['./dss-nutrizione-widget-card.component.scss'],
})
export class DssNutrizioneWidgetCardComponent implements OnInit, OnDestroy {

  /** Appezzamento data from the list phase */
  @Input({ required: true }) appezzamento!: AppezzamentoNutrizioneDto;

  readonly faGlobe = faGlobe;
  readonly faLeaf = faLeaf;
  readonly faRotate = faRotate;
  readonly faCalendar = faCalendar;
  readonly faFloppyDisk = faFloppyDisk;
  readonly faPrint = faPrint;
  readonly faChartLine = faChartLine;

  readonly status = signal<WidgetCardStatus>('loading');
  readonly consiglio = signal<AggregazioneConsiglioNutrizioneResult | null>(null);
  /** True while the save-consiglio operation is in flight – FR012 */
  readonly isSaving = signal<boolean>(false);
  /** True while the PDF is being generated – FR013 */
  readonly isPrinting = signal<boolean>(false);
  /** True while the widget refresh request is in flight – FR014 */
  readonly isRefreshing = signal<boolean>(false);
  /** Timestamp of the last successful widget refresh – FR014 */
  readonly ultimoAggiornamento = signal<Date | null>(null);

  // ── FR011 – Fasi Fenologiche modal state ──────────────────────────────────
  /** True when the Fasi Fenologiche modal overlay is open. */
  readonly isFasiFenologicheModalOpen = signal<boolean>(false);
  /** True while the fasi fenologiche are being fetched from the API. */
  readonly isFasiFenologicheLoading = signal<boolean>(false);
  /** True when the fasi fenologiche fetch resulted in an error. */
  readonly fasiFenologicheLoadError = signal<boolean>(false);
  /** The list of phenological phases loaded for the current appezzamento. */
  readonly fasiFenologiche = signal<FaseFenologicaCorrenteDto[]>([]);

  // ── FR015 – Bilancio Azoto modal state ───────────────────────────────────
  /** True when the Bilancio Azoto modal is open. */
  readonly isBilancioAzotoOpen = signal<boolean>(false);

  /** True when the card has nutrient data to display */
  readonly hasData = computed(() => this.status() === 'hasData');
  /** True when the card is still loading */
  readonly isLoading = computed(() => this.status() === 'loading');
  /** True when there is no consiglio available */
  readonly isNoData = computed(() => this.status() === 'noData');
  /** True when the consiglio could not be loaded */
  readonly isError = computed(() => this.status() === 'error');

  private readonly destroy$ = new Subject<void>();

  readonly showWidgetIcon = signal<boolean>(true);

  /** Reference to the Dettagli Appezzamento panelbar for programmatic expand/collapse during PDF generation – FR013 */
  @ViewChild('dettagliPanel') dettagliPanelBar: GiasPanelBar;

  constructor(
    private readonly dssService: DssNutrizioneService,
    private readonly messageService: GiasMessageService,
    private readonly giasDialogService: GiasDialogService,
    private readonly cdr: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.loadConsiglio();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadConsiglio(): void {
    this.status.set('loading');
    this.dssService
      .getConsiglioNutrizionale(this.appezzamento)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.consiglio.set(data);
          this.status.set(
            data && data.ConsiglioNutrizione?.Elementi?.length ? 'hasData' : 'noData'
          );
        },
        error: () => {
          this.status.set('error');
        },
      });
  }

  /**
   * Badge theme color derived from aggregazioneStatus – UIL002 §3.2
   * full_success → success, partial_success → warning, failed → error
   */
  get consiglioThemeColor(): string {
    switch (this.consiglio()?.AggregazioneStatus) {
      case 'full_success': return 'success';
      case 'partial_success': return 'warning';
      default: return 'info';
    }
  }

  /**
   * Transloco key for the status badge label when data is present – UIL002 §3.2
   */
  get consiglioStatusLabel(): string {
    switch (this.consiglio()?.AggregazioneStatus) {
      case 'full_success': return 'dssNutrizione_badge_success';
      case 'partial_success': return 'dssNutrizione_badge_partial';
      default: return 'dssNutrizione_badge_neutral';
    }
  }

  /**
   * Retries loading the consiglio for this card – UIL002 §3 stato "Error".
   * Stops the click event from bubbling to the card's navigate handler.
   */
  retryLoad(event: Event): void {
    event.stopPropagation();
    this.loadConsiglio();
  }

  /**
   * Maps a raw nutrient element code (N, P, K) to its Transloco key.
   * Used by the Consiglio Nutrizione tooltip – UIL002 §C.4.1
   */
  getNutrienteLabel(elemento: string): string {
    const labelMap: Record<string, string> = {
      N: 'dssNutrizione_tooltip_azoto',
      P: 'dssNutrizione_tooltip_fosforo',
      K: 'dssNutrizione_tooltip_potassio',
    };
    return labelMap[elemento] ?? elemento;
  }

  /**
   * Persists the currently displayed consiglio nutrizionale for this appezzamento.
   * Disables the button and shows a spinner overlay while the request is in flight.
   * If the backend returns Esito = 'DUPLICATE', a confirmation dialog is shown;
   * on confirm the request is re-sent.
   * FR012 – Salvataggio Consiglio Nutrizionale nel Sistema GIAS
   * DS16-API §POST /dss/nutrizione/consigli/salva
   */
  salvaConsiglio(event: Event): void {
    event.stopPropagation();
    const consiglio = this.consiglio();
    if (this.isSaving() || !consiglio) {
      return;
    }
    this.performSalvaConsiglio(consiglio, true);
  }

  private performSalvaConsiglio(consiglio: AggregazioneConsiglioNutrizioneResult, controllaDuplicati: boolean): void {
    this.isSaving.set(true);
    const request: SalvaConsiglioNutrizioneRequest = {
      AggregazioneConsiglio: consiglio,
      ControllaDuplicati: controllaDuplicati,
    };
    this.dssService
      .salvaConsiglio(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.isSaving.set(false);
          if (!response || response.Esito === 'ERROR') {
            this.messageService.errorMessage('ErroreSalvataggioConsiglio', true, true, [], 6000);
            return;
          }
          if (response.Esito === 'DUPLICATE') {
            this.handleDuplicateConsiglio(consiglio);
            return;
          }
          // Esito === 'SUCCESS'
          this.messageService.successMessage('ConsiglioSalvatoConSuccesso', true, true, [], 4000);
        },
        error: () => {
          this.isSaving.set(false);
          this.messageService.errorMessage('ErroreSalvataggioConsiglio', true, true, [], 6000);
        },
      });
  }

  /**
   * Shows a confirmation dialog when the backend reports that a consiglio was already
   * saved for this appezzamento (Esito = 'DUPLICATE').
   * On user confirmation the save request is re-sent with ControllaDuplicati = false
   * to bypass the duplicate check and force persistence.
   * FR012 – DUPLICATE branch
   */
  private handleDuplicateConsiglio(consiglio: AggregazioneConsiglioNutrizioneResult): void {
    this.giasDialogService
      .baseWarning(
        'dssNutrizione_duplicate_title',
        'dssNutrizione_duplicate_body',
        true
      )
      .then((result) => {
        if (result['returnObj'] === true) {
          this.performSalvaConsiglio(consiglio, false);
        }
      });
  }

  /**
   * Generates a PDF snapshot of the widget card and triggers browser download.
   * Uses Kendo Drawing (drawDOM + exportPDF + saveAs) as specified in FR013.
   */
  stampaPdf(element: HTMLElement, event: Event): void {
    event.stopPropagation();
    if (this.isPrinting()) {
      return;
    }

    this.isPrinting.set(true);

    // Expand the Dettagli Appezzamento panelbar so its content is captured in the snapshot – FR013
    if (this.dettagliPanelBar) {
      this.dettagliPanelBar.expand = true;
    }

    this.showWidgetIcon.set(false);

    drawDOM(element, { paperSize: 'A4' })
      .then(group => {
        return exportPDF(group, { paperSize: 'A4' });
      })
      .then(dataUri => {
        const fileName = `consiglio_${this.appezzamento.NomeAppezzamento ?? 'appezzamento'}.pdf`;
        saveAs(dataUri, fileName);
        this.messageService.successMessage('dssNutrizione_stampa_success', false, true, [], 4000);
      })
      .catch(() => {
        this.messageService.errorMessage('dssNutrizione_stampa_error', false, true, [], 6000);
      })
      .finally(() => {
        // Collapse the Dettagli Appezzamento panelbar immediately after PDF generation – FR013
        if (this.dettagliPanelBar) {
          this.dettagliPanelBar.expand = false;
        }
        this.showWidgetIcon.set(true);
        this.isPrinting.set(false);
      });

  }

  /**
   * Navigates to the appezzamento detail page.
   * Navigation target is not yet defined (UIL003 §E – NON IMPLEMENTARLO).
   */
  navigateToDettaglio(): void {
    // Navigation destination not yet defined – UIL003 §E
  }

  /**
   * Opens the Fasi Fenologiche modal and fetches the phenological phase list
   * for the current appezzamento's impianto in the current calendar year.
   * An error during fetch shows the error state inside the modal without closing it.
   * FR011 – Recupero Fasi Fenologiche dell'Impianto
   * DS21-API §POST /api/fasi-fenologiche-registrate
   */
  apriVisualizzaFasiFenologiche(event: Event): void {
    event.stopPropagation();
    this.fasiFenologiche.set([]);
    this.fasiFenologicheLoadError.set(false);
    this.isFasiFenologicheLoading.set(true);
    this.isFasiFenologicheModalOpen.set(true);
    this.dssService
      .getFasiFenologiche(this.appezzamento)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (fasi) => {
          this.fasiFenologiche.set(fasi);
          this.isFasiFenologicheLoading.set(false);
        },
        error: () => {
          this.fasiFenologicheLoadError.set(true);
          this.isFasiFenologicheLoading.set(false);
        },
      });
  }

  /**
   * Closes the Fasi Fenologiche modal overlay without any further action.
   * FR011 – UIL006 §D: click X / "Chiudi" / backdrop / ESC
   */
  chiudiFasiFenologicheModal(): void {
    this.isFasiFenologicheModalOpen.set(false);
  }

  /**
   * Opens the Bilancio Azoto modal for the current appezzamento,
   * passing appezzamento, specie and fase fenologica corrente as context.
   * FR015 – Grafico Bilancio Azoto
   */
  apriBilancioAzoto(event: Event): void {
    event.stopPropagation();
    this.isBilancioAzotoOpen.set(true);
  }

  /**
   * Closes the Bilancio Azoto modal.
   * FR015 – Grafico Bilancio Azoto
   */
  chiudiBilancioAzoto(): void {
    this.isBilancioAzotoOpen.set(false);
  }

  /**
   * Refreshes the widget by re-fetching the consiglio nutrizionale for this appezzamento.
   * On success the consiglio and ultimoAggiornamento are updated; on error the previous
   * data is preserved and only a toast is shown.
   * FR014 – Ricarica Dati Widget Singolo Appezzamento
   */
  ricaricaWidget(event: Event): void {
    event.stopPropagation();
    if (this.isRefreshing()) {
      return;
    }
    this.isRefreshing.set(true);
    this.dssService
      .getConsiglioNutrizionale(this.appezzamento)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.isRefreshing.set(false);
          this.consiglio.set(data);
          this.status.set(
            data && data.ConsiglioNutrizione?.Elementi?.length ? 'hasData' : 'noData'
          );
          this.ultimoAggiornamento.set(new Date());
          this.messageService.successMessage('dssNutrizione_ricarica_success', false, true, [], 4000);
        },
        error: () => {
          this.isRefreshing.set(false);
          this.messageService.errorMessage('dssNutrizione_ricarica_error', false, true, [], 6000);
        },
      });
  }
}
