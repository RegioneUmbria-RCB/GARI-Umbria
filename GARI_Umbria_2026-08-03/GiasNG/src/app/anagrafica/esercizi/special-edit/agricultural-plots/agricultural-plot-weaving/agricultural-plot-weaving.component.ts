import {Component, DestroyRef, inject, Inject, Input, OnInit} from '@angular/core';
import {GiasKendoGridModule, KendoGridRow} from 'gias-kendo-grid';
import {BehaviorSubject, catchError, of, ReplaySubject, Subject, take} from 'rxjs';
import {TranslocoPipe} from '@jsverse/transloco';
import {AgriculturalPlotWeavingService} from './agricultural-plot-weaving.service';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {GiasMessageService} from '../../../../../Service/gias-message.service';
import {ObjParametriAgendaService} from '../../../../../Service/obj-parametri-agenda.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {Appezzamento} from '../../../../../Model/anagrafiche/Appezzamento';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UikitModule} from '../../../../../Utility/uikit.module';
import {ButtonComponent} from '@progress/kendo-angular-buttons';
import {ClasseTessitura} from '../../../../../Model/anagrafiche/ClasseTessitura';
import { GiasUikitModule } from 'gias-ui-kit';
import {AnagraficaService} from '../../../../anagrafica.service';
import {AgriculturalItem} from '../../agricultural-item.model';

class WeavingFG {
  sand: FormControl<number> = new FormControl<number>(
    0,
    [
      Validators.min(0),
      Validators.max(100),
      (c) => Number.isNaN(c.value) ? {sand: 'isNaN'} : null,
      (c) => c?.value == undefined ? {sand: 'undefined'} : null
    ]
  );

  clay: FormControl<number> = new FormControl<number>(
    0,
    [
      Validators.min(0),
      Validators.max(100),
      (c) => Number.isNaN(c.value) ? {clay: 'isNaN'} : null,
      (c) => c?.value == undefined ? {clay: 'undefined'} : null
    ]
  );

  silt: FormControl<number> = new FormControl<number>(
    0,
    [
      Validators.min(0),
      Validators.max(100),
      (c) => Number.isNaN(c.value) ? {silt: 'isNaN'} : null,
      (c) => c?.value == undefined ? {silt: 'undefined'} : null
    ]
  );

  constructor() {
  }
}

class WeavingClassFG {
  code: FormControl<number> = new FormControl<number>(0);
  description: FormControl<string> = new FormControl<string>('');
}

@Component({
  standalone: true,
  selector: 'app-agricultural-plot-weaving',
  imports: [
    TranslocoPipe,
    UikitModule,
    ReactiveFormsModule,
    ButtonComponent,
    GiasUikitModule,
    GiasKendoGridModule
  ],
  templateUrl: './agricultural-plot-weaving.component.html',
  styleUrl: './agricultural-plot-weaving.component.css',
  providers: [AgriculturalPlotWeavingService]
})
export class AgriculturalPlotWeavingComponent implements OnInit{
  @Input() private checkedRows$: ReplaySubject<KendoGridRow[]> = new ReplaySubject<KendoGridRow[]>(1);

  protected listViewHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  protected rows$: BehaviorSubject<AgriculturalItem[]> = new BehaviorSubject<AgriculturalItem[]>([]);
  protected removeDataItem$: Subject<{ index: number, deleteCount: number; }> = new Subject<{ index: number; deleteCount: number; }>();
  protected resetStates$: Subject<boolean> = new Subject();
  protected weavingForm: FormGroup<WeavingFG> = new FormGroup<WeavingFG>(new WeavingFG());
  protected weavingClassForm: FormGroup<WeavingClassFG> = new FormGroup<WeavingClassFG>(new WeavingClassFG());
  protected loading$ = this.plotWeavingService.loading$;

  private _destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private plotsService: ImpiantiFactoryService,
    private giasMessageService: GiasMessageService,
    private objParametriService: ObjParametriAgendaService,
    private plotWeavingService: AgriculturalPlotWeavingService,
    private anagraficaService: AnagraficaService
  ) {
    this.weavingClassForm.disable();
  }

  ngOnInit(): void {
    this.listViewHeight$.next(0.25 * window.innerHeight);

    this.checkedRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(crs => {
      this.rows$.next([]);
      this.plotWeavingService.checkedRows = crs;
      this.plotWeavingService.dateFilter = new Date(this.anagraficaService.filterData.getValue().data);
      this.plotWeavingService.setListViewRows();
    });

    this.plotWeavingService.listViewRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(rr => {
      this.rows$.next([...this.rows$.value, ...rr]);
    });
  }

  showListViewDeleteBtn(): boolean {
    return false;
  }

  submit(): void {
    if (this.weavingForm.valid) {
      let plots: Appezzamento[] = this.rows$.value.map(r => r.toAppezzamento());
      plots.forEach(p => {
        p.sabbia = this.weavingForm.controls.sand.value;
        p.argilla = this.weavingForm.controls.clay.value;
        p.limo = this.weavingForm.controls.silt.value;
        p.classeTessitura = new ClasseTessitura(this.weavingClassForm.controls.code.value, this.weavingClassForm.controls.description.value);
      });

      this.plotsService.updateWeaving(plots).pipe(
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

  onChange(): void {
    this.plotsService.leggiClasseTessituraObs({
      Id_ClasseTessitura: 0,
      sabbia: this.weavingForm.controls.sand.value,
      argilla: this.weavingForm.controls.clay.value
    }).subscribe(wc => {
      if (wc == null) {
        console.log(wc);
        this.weavingClassForm.patchValue({code: 0, description: ''});
      } else {
        console.log(wc);
        const weavingClass = {code: wc.codice, description: wc.descrizione};
        this.weavingClassForm.patchValue(weavingClass);
      }
    });
  }
}
