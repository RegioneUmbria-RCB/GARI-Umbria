import { Component, Inject, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, catchError, lastValueFrom, map, of, ReplaySubject, Subject, take, takeUntil } from 'rxjs';
import {AGRODATAFINE, AGRODATAINIZIO, GiiasMultiselectTemplateSComponent, IListViewItem} from 'gias-ui-kit';
import { GiasDropDownTemplateSComponent, GiasDropDownTemplateService, ObjParametriAgenda } from 'gias-ui-kit';

import { validityValidator } from 'gias-ui-kit';
import { AppezzamentoXParcoMacchine } from 'app/Model/anagrafiche/appezzamento-x-parco-macchine';
import { IntervalloTemporale } from 'app/Service/api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ImpiantiServiceProvider } from 'app/Service/ServiceFactory/impianti.factory.provider';
import { IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService } from 'app/Service/ServiceFactory/impianti.factory.service';
import { MacchineServiceProvider } from 'app/Service/ServiceFactory/macchine.factory.provider';
import { MACCHINE_SERVICE_TOKEN, MacchineFactoryService } from 'app/Service/ServiceFactory/macchine.factory.service';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { AgriculturalPlotsMachinesLinkService } from './agricultural-plots-machines-link.service';
import {KendoGridRow} from 'gias-kendo-grid';
import {GiasMessageService} from '../../../../../Service/gias-message.service';
import {AgriculturalItem} from '../../agricultural-item.model';

class DdlMachineFG {
  machine: FormControl<object> = new FormControl<object>(
    {},
    [(c) => !!c.value['Mac_Cod'] ? null : { machine: 'required' }]
  );

  machineType: FormControl<string> = new FormControl<string>('05.07');

  constructor() { }
}

@Component({
  standalone: false,
  selector: 'app-agricultural-plots-machines-link',
  templateUrl: './agricultural-plot-machine-link.component.html',
  styleUrls: ['./agricultural-plot-machine-link.component.css'],
  providers: [GiasDropDownTemplateService, MacchineServiceProvider, ImpiantiServiceProvider, AgriculturalPlotsMachinesLinkService],
  encapsulation: ViewEncapsulation.None
})
export class AgriculturalPlotMachineLinkComponent implements OnInit, OnDestroy {
  @Input() protected listViewHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  @Input() private checkedRows$: Subject<KendoGridRow[]> = new Subject<KendoGridRow[]>();
  protected rows$: BehaviorSubject<AgriculturalItem[]> = new BehaviorSubject<AgriculturalItem[]>([]);
  protected removeDataItem$: Subject<{ index: number, deleteCount: number; }> = new Subject<{ index: number; deleteCount: number; }>();
  protected resetStates$: Subject<boolean> = new Subject();
  protected loading$ = this.agriculturalPlotsMachinesLink.loading$;

  get selectedMachineType(): string {
    return this.machinesDdlForm.get('machineType').getRawValue();
  }

  get selectedMachine(): object {
    return this.machinesDdlForm.get('machine').getRawValue();
  }

  get linkValidity(): IntervalloTemporale {
    return this.validityForm.getRawValue();
  }

  get chosenAgriculturalPlots(): AgriculturalItem[] {
    return this.rows$.getValue();
  }

  machinesDdlForm: FormGroup;
  validityForm: FormGroup;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private giasMessageService: GiasMessageService,
    private objParametriService: ObjParametriAgendaService,
    private agriculturalPlotsMachinesLink: AgriculturalPlotsMachinesLinkService
  ) {
    this.checkedRows$.pipe(takeUntil(this.destroy$)).subscribe(crs => {
      this.rows$.next([]);
      this.agriculturalPlotsMachinesLink.checkedRows = crs;
      this.agriculturalPlotsMachinesLink.setListViewRows();
    });

    this.agriculturalPlotsMachinesLink.listViewRows$.pipe(takeUntil(this.destroy$)).subscribe(rr => {
      this.rows$.next([...this.rows$.value, ...rr]);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    this.machinesDdlForm = new FormGroup<DdlMachineFG>(new DdlMachineFG());

    this.validityForm = new FormGroup({
      inizio: new FormControl(AGRODATAINIZIO, [Validators.required]),
      fine: new FormControl(AGRODATAFINE, [Validators.required])
    }, { asyncValidators: validityValidator('inizio', 'fine') });

    this.machinesDdlForm.get('machine').valueChanges.pipe(takeUntil(this.destroy$)).subscribe(v => {
      this.onMachineChange(v);
    });
  }

  showListViewDeleteBtn(): boolean {
    return true;
  }

  onDelete(params: { item: IListViewItem, itemIndex: number; }): void {
    if (this.validityForm.invalid) {
      this.giasMessageService.warningMessage('IntervalloTemporaleNonValido', false, true);
    } else {
      const agr: AgriculturalItem = params.item as AgriculturalItem;
      let axp: AppezzamentoXParcoMacchine = new AppezzamentoXParcoMacchine(
        agr.piva,
        agr.saCod,
        agr.appezza,
        this.selectedMachine['Mac_Cod'],
        this.selectedMachineType,
        this.linkValidity.inizio,
        this.linkValidity.fine
      );

      this.appezzamentiService.deleteAppezzamentiXParcoMacchine(axp).pipe(
        take(1),
        catchError(() => {
          this.giasMessageService.errorMessage('ErroreSalvataggio', false, true);
          return of(null);
        })
      ).subscribe(r => {
        if (r) {
          this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
          this.removeDataItem$.next({ index: params.itemIndex, deleteCount: 1 });
        } else {
          this.giasMessageService.errorMessage('ErroreSalvataggio', false, true);
        }
      });
    }
  }

  openDdl(ddl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    switch (ddl.giasFormControlName) {
      case 'machine':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddl, lastValueFrom(
            this.macchineService.readMachinesByClassCode(this.pepareMachinesXTypeReadParams()).pipe(take(1), map(r => r.RispostaStringa))
          )
        );
        break;
      default:
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddl, lastValueFrom(of([]).pipe(take(1))));
        break;
    }
  }

  private pepareMachinesXTypeReadParams(): { parametriAgenda: ObjParametriAgenda, type: string, det1: string, det2: string } {
    // feel free to write better this function...
    // my idea was to put a ddl containing all possible machine types (from Macchine table on SQL Server) and pass corresponding class_code to this function
    const type: string = this.selectedMachineType.split('.')[0] ?? '';
    const det1: string = this.selectedMachineType.split('.')[1] ?? '';
    const det2: string = this.selectedMachineType.split('.')[2] ?? '';

    return {
      parametriAgenda: this.objParametriService.getObjParamValue(),
      type: type,
      det1: det1,
      det2: det2
    };
  }

  private onMachineChange(machine: object): void {
    this.resetStates$.next(false);
    this.agriculturalPlotsMachinesLink.compareMachinePlotsValidity(this.rows$.value, machine);
  }
}
