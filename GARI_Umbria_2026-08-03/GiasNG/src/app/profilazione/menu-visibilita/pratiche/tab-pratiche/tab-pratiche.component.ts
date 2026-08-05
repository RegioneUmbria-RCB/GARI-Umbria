import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { EMPTY, Subject, forkJoin, switchMap, takeUntil, tap } from 'rxjs';
import { PraticheVisibilitaService } from 'app/profilazione/services/pratiche-visibilita.service';
import {
  ConfigurazionePraticheUtente,
  PraticaSelezionabile,
} from 'app/profilazione/models/pratiche-visibilita.model';
import { GrigliaPraticheComponent } from '../griglia-pratiche/griglia-pratiche.component';
import { MasterService } from 'app/Service/master.service';

@Component({
  standalone: false,
  selector: 'app-tab-pratiche',
  templateUrl: './tab-pratiche.component.html',
  styleUrls: ['./tab-pratiche.component.scss'],
})
export class TabPraticheComponent implements OnInit, OnChanges, AfterViewInit, OnDestroy {
  @ViewChild('grigliaRef') grigliaRef: GrigliaPraticheComponent;

  @Input() username: string;
  @Input() operatoreOR = false;
  @Output() operatoreORChange = new EventEmitter<boolean>();
  @Input() filtroPraticheAttivo = false;
  @Output() filtroPraticheAttivoChange = new EventEmitter<boolean>();
  @Output() hasUnsavedChanges = new EventEmitter<boolean>();

  protected pratiche: PraticaSelezionabile[] = [];
  protected isLoading = false;
  protected isError = false;
  protected selectedCount = 0;
  isDirty = false;
  isSaving = false;

  private destroy$ = new Subject<void>();
  private _baseline = '';

  constructor(
    private praticheService: PraticheVisibilitaService,
    private master: MasterService
  ) { }

  ngOnInit(): void {
    this.caricaDati();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['operatoreOR'] || changes['filtroPraticheAttivo']) && this._baseline) {
      this.recomputeDirty();
      this.grigliaRef?.setConfig(this.username, this.operatoreOR, this.filtroPraticheAttivo);
    }
  }

  ngAfterViewInit(): void {
    this.grigliaRef.setConfig(this.username, this.operatoreOR, this.filtroPraticheAttivo);
    this.grigliaRef.savedSuccessfully$.pipe(takeUntil(this.destroy$)).subscribe(() => {
      this._baseline = this.computeSnapshot();
      this.isDirty = false;
      this.hasUnsavedChanges.emit(false);
    });

    this.grigliaRef.configRefreshed$.pipe(takeUntil(this.destroy$)).subscribe((config) => {
      this.operatoreOR = config.OperatoreFiltri?.toUpperCase() === 'OR';
      this.filtroPraticheAttivo = config.FiltroPraticheAttivo ?? false;
      this.operatoreORChange.emit(this.operatoreOR);
      this.filtroPraticheAttivoChange.emit(this.filtroPraticheAttivo);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected caricaDati(): void {
    if (!this.username) return;

    this.isLoading = true;
    this.isError = false;
    this.master.set_isLoading({ isLoading: true });
    forkJoin([
      this.praticheService.caricaCatalogoPratiche(),
      this.praticheService.caricaConfigurazioneUtente(this.username),
    ])
      .pipe(takeUntil(this.destroy$), tap(() => this.master.set_isLoading({ isLoading: false })))
      .subscribe({
        next: ([catalogo, config]) => {
          this.applicaConfigurazione(catalogo, config);
          this.isLoading = false;
        },
        error: () => {
          this.isError = true;
          this.isLoading = false;
        },
      });
  }

  private applicaConfigurazione(
    catalogo: PraticaSelezionabile[],
    config: ConfigurazionePraticheUtente,
  ): void {
    const selectedSet = new Set((config?.Pratiche ?? []).map((p) => p.Servizio_Cod));
    const validitaMap = new Map(
      (config?.Pratiche ?? []).map((p) => [p.Servizio_Cod, p.ConsideraValiditaTemporale]),
    );

    this.pratiche = catalogo.map((p) => ({
      ...p,
      Selected: selectedSet.has(p.Servizio_Cod),
      ConsideraValiditaTemporale: validitaMap.get(p.Servizio_Cod) ?? false,
    }));

    this.operatoreOR = config?.OperatoreFiltri?.toUpperCase() === 'OR';
    this.filtroPraticheAttivo = config?.FiltroPraticheAttivo ?? false;
    this._baseline = this.computeSnapshot();
    this.isDirty = false;
    this.hasUnsavedChanges.emit(false);
    this.operatoreORChange.emit(this.operatoreOR);
    this.filtroPraticheAttivoChange.emit(this.filtroPraticheAttivo);
  }

  protected onSelectionCountChange(count: number): void {
    const wasZero = this.selectedCount === 0;
    const isNowZero = count === 0;
    this.selectedCount = count;

    if (wasZero && !isNowZero) {
      this.filtroPraticheAttivo = true;
      this.filtroPraticheAttivoChange.emit(true);
      this.recomputeDirty();
      this.grigliaRef?.setConfig(this.username, this.operatoreOR, this.filtroPraticheAttivo);
    } else if (!wasZero && isNowZero) {
      this.filtroPraticheAttivo = false;
      this.filtroPraticheAttivoChange.emit(false);
      this.recomputeDirty();
      this.grigliaRef?.setConfig(this.username, this.operatoreOR, this.filtroPraticheAttivo);
    }
  }

  protected onGridChange(): void {
    this.recomputeDirty();
  }

  private computeSnapshot(): string {
    const selected = this.pratiche
      .filter((p) => p.Selected)
      .map((p) => ({ c: p.Servizio_Cod, v: p.ConsideraValiditaTemporale }))
      .sort((a, b) => a.c - b.c);
    return JSON.stringify({
      or: this.operatoreOR,
      fa: this.filtroPraticheAttivo,
      p: selected,
    });
  }

  save(): void {
    if (!this.username || this.isSaving) return;
    this.isSaving = true;
    this.praticheService
      .eseguiSalvataggio(this.username, this.pratiche.filter((p) => p.Selected), this.operatoreOR, this.filtroPraticheAttivo)
      .pipe(
        takeUntil(this.destroy$),
        switchMap((success) => success ? this.praticheService.caricaConfigurazioneUtente(this.username) : EMPTY),
      )
      .subscribe({
        next: (config) => {
          this.operatoreOR = config.OperatoreFiltri?.toUpperCase() === 'OR';
          this.filtroPraticheAttivo = config.FiltroPraticheAttivo ?? false;
          this.grigliaRef.setConfig(this.username, this.operatoreOR, this.filtroPraticheAttivo);
          this._baseline = this.computeSnapshot();
          this.isDirty = false;
          this.hasUnsavedChanges.emit(false);
          this.isSaving = false;
          this.operatoreORChange.emit(this.operatoreOR);
          this.filtroPraticheAttivoChange.emit(this.filtroPraticheAttivo);
        },
        error: () => {
          this.isSaving = false;
        },
        complete: () => {
          this.isSaving = false;
        },
      });
  }

  private recomputeDirty(): void {
    const dirty = this.computeSnapshot() !== this._baseline;
    if (dirty !== this.isDirty) {
      this.isDirty = dirty;
      this.hasUnsavedChanges.emit(dirty);
    }
  }

}

