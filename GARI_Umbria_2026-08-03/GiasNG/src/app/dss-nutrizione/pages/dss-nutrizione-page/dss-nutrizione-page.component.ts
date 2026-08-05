import {Component, computed, Inject, InjectionToken, OnDestroy, OnInit, signal} from '@angular/core';
import { skip, Subject, takeUntil } from 'rxjs';
import { DssNutrizioneService } from '../../services/dss-nutrizione.service';
import {
  AppezzamentoNutrizioneDto,
  AppezzamentoWidget,
  CentroAziendaleGroup,
  SpecieVegetale,
} from '../../models/dss-nutrizione.model';
import {GiasMultiSelectTemplateService, MultiSelectFormItem} from 'gias-ui-kit';
import {FormControl, FormGroup} from '@angular/forms';

/**
 * Page orchestrator for DSS Nutrizione.
 * Loads the active appezzamenti for the selected company, groups them by
 * Centro Aziendale and renders gias-expansionpanel sections with widget cards.
 * FR002 §1 (UIL001 §5, UIL003 §2, UIL003 §4), FR002 §2, FR002 §3
 */
@Component({
  standalone: false,
  selector: 'app-dss-nutrizione-page',
  templateUrl: './dss-nutrizione-page.component.html',
  styleUrls: ['./dss-nutrizione-page.component.scss'],
  providers: [GiasMultiSelectTemplateService]
})
export class DssNutrizionePageComponent implements OnInit, OnDestroy {

  /** Selected specie vegetali for filtering – FR002 §2 */
  public DssNutrizioneForm: FormGroup = new FormGroup({
    SpecieVegetali: new FormControl([])
  });

  /** Global page loading state – FR002 §1 (UIL001 §5) */
  readonly isLoading = signal<boolean>(false);

  /** Global error state – UIL003 §5 */
  readonly hasError = signal<boolean>(false);

  /** Today's date shown in the filter area – UIL001 §3 */
  readonly today = new Date();

  /** List of active appezzamenti from the API */
  private readonly allAppezzamenti = signal<AppezzamentoWidget[]>([]);

  /** Available specie for the multiselect filter – FR002 §2 */
  readonly specieList = signal<SpecieVegetale[]>([]);

  /** Whether the species list is still loading */
  readonly specieLoading = signal<boolean>(false);

  /**
   * Derived list of appezzamenti filtered by selected species.
   * If no species are selected all appezzamenti are shown.
   */
  private readonly filteredAppezzamenti = computed<AppezzamentoWidget[]>(() => {
    const selected = this.DssNutrizioneForm.get("SpecieVegetali").getRawValue();
    const all = this.allAppezzamenti();
    if (!selected.length) {
      return all;
    }
    const codes = new Set(selected.map(s => s.Veg_Cod));
    return all.filter(w => codes.has(w.appezzamento.SpecieCod));
  });

  /**
   * Appezzamenti grouped by Centro Aziendale (Sa_Cod).
   * Used to build the Panel Bar – FR002 §1 (UIL003 §2)
   */
  readonly centroAziendaleGroups = computed<CentroAziendaleGroup[]>(() => {
    const appezzamenti = this.filteredAppezzamenti();
    const groupMap = new Map<number, CentroAziendaleGroup>();
    for (const widget of appezzamenti) {
      const { SaCod, SaNome } = widget.appezzamento;
      if (!groupMap.has(SaCod)) {
        groupMap.set(SaCod, { saCod: SaCod, saDes: SaNome, appezzamenti: [] });
      }
      groupMap.get(SaCod)!.appezzamenti.push(widget);
    }
    return Array.from(groupMap.values());
  });

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly dssService: DssNutrizioneService,
    private readonly multiSelectService: GiasMultiSelectTemplateService
  ) { }

  ngOnInit(): void {
    this.loadPage();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }


  /** Reloads the full page data – UIL003 §5 retry button */
  reload(): void {
    this.hasError.set(false);
    this.allAppezzamenti.set([]);
    this.loadPage();
  }

  private loadPage(): void {
    this.isLoading.set(true);

    this.loadSpecieAziendali();

    this.dssService
      .getAppezzamentiAttivi()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (appezzamenti: AppezzamentoNutrizioneDto[]) => {
          const widgets: AppezzamentoWidget[] = appezzamenti.map(a => ({
            appezzamento: a,
            consiglio: null,
            status: 'loading',
          }));
          this.allAppezzamenti.set(widgets);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.hasError.set(true);
        },
      });

        /** Triggered when the user changes the specie multiselect – FR002 §2 */
      this.multiSelectService.currentMultiSelectValueObject
      .pipe(skip(1),takeUntil(this.destroy$))
      .subscribe((multiElem: MultiSelectFormItem) => {
              switch(multiElem.FormControlName){
                  case 'SpecieVegetali':
                      break;
              }
          });
  }

  private loadSpecieAziendali(): void {
    this.specieLoading.set(true);
    this.dssService
      .getSpecieAziendali()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (specie: SpecieVegetale[]) => {
          this.specieList.set(specie);
          this.specieLoading.set(false);
        },
        error: () => {
          this.specieLoading.set(false);
        },
      });
  }
}
