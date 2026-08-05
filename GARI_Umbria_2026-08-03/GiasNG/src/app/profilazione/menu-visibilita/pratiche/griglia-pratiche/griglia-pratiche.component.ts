import {
  AfterViewInit,
  Component,
  DoCheck,
  EventEmitter,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { generateGridProviders, GiasKendoGridComponent, GridBatchHandlerService, GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { Observable, Subject } from 'rxjs';
import { ConfigurazionePraticheUtente, PraticaSelezionabile } from 'app/profilazione/models/pratiche-visibilita.model';
import { GrigliaPraticheGridService } from './griglia-pratiche-grid.service';

@Component({
  standalone: false,
  selector: 'app-griglia-pratiche',
  templateUrl: './griglia-pratiche.component.html',
  styleUrls: ['./griglia-pratiche.component.scss'],
  providers: [...generateGridProviders(GrigliaPraticheGridService, GrigliaPraticheComponent)],
})
export class GrigliaPraticheComponent implements OnChanges, DoCheck, AfterViewInit, OnDestroy {
  @ViewChild('grid') grid: GiasKendoGridComponent;

  @Input() pratiche: PraticaSelezionabile[] = [];
  @Input() isLoading = false;
  @Input() isError = false;

  @Output() selectionCountChange = new EventEmitter<number>();
  @Output() retryClick = new EventEmitter<void>();
  @Output() gridChange = new EventEmitter<void>();

  get savedSuccessfully$(): Observable<void> {
    return this.gridService.savedSuccessfully$;
  }

  get configRefreshed$(): Observable<ConfigurazionePraticheUtente> {
    return this.gridService.configRefreshed$;
  }

  protected selectedCount = 0;

  private origSelectedCount = 0;
  private destroy$ = new Subject<void>();
  private previousPratiche: PraticaSelezionabile[] = [];

  private get batchHandler(): GridBatchHandlerService {
    return (this.grid as any).gridBatchHandlerService as GridBatchHandlerService;
  }

  get allSelected(): boolean {
    const rows = this.gridService.getRows();
    return rows.length > 0 && rows.every(r => r.Selected);
  }

  get someSelected(): boolean {
    return this.gridService.getRows().some(r => r.Selected);
  }

  constructor(@Inject(GRID_HTTP_TOKEN) private gridService: GrigliaPraticheGridService) { }

  ngAfterViewInit(): void {
    this.selectedCount = this.pratiche?.filter((p) => p.Selected)?.length ?? 0;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['pratiche'] && this.pratiche) {
      this.gridService.setRows(this.pratiche);
      this.selectedCount = this.pratiche.filter((p) => p.Selected).length;
      this.selectionCountChange.emit(this.selectedCount);
      if (this.grid) {
        this.grid.publicService.refresh();
      }
    }
  }

  ngDoCheck(): void {
    if (this.pratiche && this.pratiche !== this.previousPratiche) {
      this.previousPratiche = this.pratiche;
      return;
    }
    if (this.pratiche) {
      const currentCount = this.pratiche.filter((p) => p.Selected).length;
      if (currentCount !== this.selectedCount) {
        this.selectedCount = currentCount;
        this.selectionCountChange.emit(this.selectedCount);
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  setConfig(username: string, operatoreOR: boolean, filtroPraticheAttivo: boolean): void {
    this.gridService.setConfig(username, operatoreOR, filtroPraticheAttivo);
  }

  onRetry(): void {
    this.retryClick.emit();
  }

  onCellClick(): void {
    this.gridChange.emit();
  }

  onToggleSelectAll(checked: boolean): void {
    this.gridService.toggleSelectAll(checked);
    this.gridService.getRows().forEach(row =>
      this.batchHandler.update({ ...row }, 'Servizio_Cod')
    );
    this.grid.publicService.refresh(true);
    this.gridChange.emit();
  }

  onRowToggleSelected(dataItem: any, checked: boolean): void {
    this.gridService.toggleRow(dataItem.Servizio_Cod, checked);
    this.batchHandler.update({ ...dataItem, Selected: checked }, 'Servizio_Cod');
    this.grid.publicService.refresh(true);
    this.gridChange.emit();
  }

}
