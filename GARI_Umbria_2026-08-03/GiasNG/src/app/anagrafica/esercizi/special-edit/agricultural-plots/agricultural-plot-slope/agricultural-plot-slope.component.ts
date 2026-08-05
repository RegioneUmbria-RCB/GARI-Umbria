import {Component, DestroyRef, inject, Inject, Input, OnDestroy, OnInit} from '@angular/core';
import {TranslocoPipe} from '@jsverse/transloco';
import {BehaviorSubject, catchError, of, ReplaySubject, Subject, take} from 'rxjs';
import {UikitModule} from '../../../../../Utility/uikit.module';
import {GiasKendoGridModule, KendoGridRow} from 'gias-kendo-grid';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {GiasMessageService} from '../../../../../Service/gias-message.service';
import {ObjParametriAgendaService} from '../../../../../Service/obj-parametri-agenda.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {Appezzamento} from '../../../../../Model/anagrafiche/Appezzamento';
import {ButtonComponent} from '@progress/kendo-angular-buttons';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { GiasUikitModule } from 'gias-ui-kit';
import {AgriculturalPlotSlopeService} from './agricultural-plot-slope.service';
import {AnagraficaService} from '../../../../anagrafica.service';
import {AgriculturalItem} from '../../agricultural-item.model';

class GroundSlopeFG {
  slope: FormControl<number> = new FormControl<number>(
    0,
    [
      Validators.min(0),
      (c) => Number.isNaN(c.value) ? {slope: 'isNaN'} : null,
      (c) => c?.value == undefined ? {slope: 'undefined'} : null
    ]
  );
}

@Component({
  standalone: true,
  selector: 'app-agricultural-plot-slope',
  imports: [
    TranslocoPipe,
    UikitModule,
    GiasKendoGridModule,
    GiasUikitModule,
    ButtonComponent,
    ReactiveFormsModule
  ],
  templateUrl: './agricultural-plot-slope.component.html',
  styleUrl: './agricultural-plot-slope.component.css',
  providers: [AgriculturalPlotSlopeService]
})
export class AgriculturalPlotSlopeComponent implements OnInit, OnDestroy{
  @Input() private windowHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  @Input() private checkedRows$: ReplaySubject<KendoGridRow[]> = new ReplaySubject<KendoGridRow[]>(1);

  protected listViewHeight$: ReplaySubject<number> = new ReplaySubject<number>();
  protected rows$: BehaviorSubject<AgriculturalItem[]> = new BehaviorSubject<AgriculturalItem[]>([]);
  protected removeDataItem$: Subject<{ index: number, deleteCount: number; }> = new Subject<{ index: number; deleteCount: number; }>();
  protected resetStates$: Subject<boolean> = new Subject();
  protected slopeForm: FormGroup<GroundSlopeFG> = new FormGroup<GroundSlopeFG>(new GroundSlopeFG());
  protected loading$ = this.plotSlopeService.loading$;

  private _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private plotsService: ImpiantiFactoryService,
    private giasMessageService: GiasMessageService,
    private objParametriService: ObjParametriAgendaService,
    private plotSlopeService: AgriculturalPlotSlopeService,
    private anagraficaService: AnagraficaService
  ) {  }

  ngOnDestroy(): void {
  }

  ngOnInit(): void {

    this.listViewHeight$.next(0.45 * window.innerHeight);

    this.checkedRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(crs => {
      this.rows$.next([]);
      this.plotSlopeService.checkedRows = crs;
      this.plotSlopeService.dateFilter = new Date(this.anagraficaService.filterData.getValue().data);
      this.plotSlopeService.setListViewRows();
    });

    this.plotSlopeService.listViewRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(rr => {
      this.rows$.next([...rr]);
    });
  }

  showListViewDeleteBtn(): boolean {
    return false;
  }

  submit(): void {
    if (this.slopeForm.valid) {
      let plots: Appezzamento[] = this.rows$.value.map(r => r.toAppezzamento());
      plots.forEach(p => {
        p.pendenza = this.slopeForm.controls.slope.value;
      });

      this.plotsService.updateSlope(plots).pipe(
        take(1),
        catchError(() => of(false))
      ).subscribe(r => {
        if (r) {
          this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
        } else {
          this.giasMessageService.errorMessage('ErroreDuranteIlSalvataggio', false, true);
        }
      });
    }
  }
}
