import {Component, ElementRef, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {take} from 'rxjs/operators';
import {catchError, forkJoin, of, startWith, Subject, switchMap, takeUntil} from 'rxjs';
import {TranslocoService} from '@jsverse/transloco';

import {AGRODATAFINE, AGRODATAINIZIO} from '../../../../Model/CostantiPersonalizzate';
import {
  ElencoElaborazioniMassivePerTipoCheckList,
  GisClient,
  IncludiEsludiImpresaISCC_In,
  LeggiElencoElaborazioniMassive_In, LeggiElencoElaborazioniMassive_Out, SottomettiElaborazioneMassivaGISCheckList_In,
  VerificaAziendaAbilitataISCC_In
} from '../../../../Service/api.service';
import { ObjParametriAgendaService} from '../../../../Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import {GiasMessageService} from '../../../../Service/gias-message.service';
import {GiasDialogService} from '../../../../Service/gias-dialog.service';
import {MasterService} from '../../../../Service/master.service';
import {DatePipe} from '@angular/common';
import {faArrowsRotate} from '@fortawesome/free-solid-svg-icons';
import {PermessiUtenteService} from '../../../../Service/permessi-utente.service';
import {enum_Security_Attivita} from '../../../../Model/TipiEnumerativi';

/**
 * keep aligned with enum_GIS_CheckList_Type in TipiEnumerativi in AgronicaCore_2010 solution
 */
export enum CheckListEnum {
  Null = 0,
  ISCC = 1,
}

export enum EnableCompanyEnum {
  Include = 1,
  Exclude = 0
}

@Component({
  standalone: false,
  selector: 'gis-algorithm-result',
  templateUrl: './gis-algorithm-result.component.html',
  styleUrls: ['./gis-algorithm-result.component.css']
})
export class GISAlgorithmResultComponent implements OnInit, OnDestroy {
  get currentCompanyExecutions(): ElencoElaborazioniMassivePerTipoCheckList[] {
    return this._executions.elenco
      .find(e => e.piva === this.objParametriAgenda.Piva)?.elencoElaborazioniPerTipo;
  }

  get executions(): LeggiElencoElaborazioniMassive_Out {
    return this._executions;
  }

  set executions(value: LeggiElencoElaborazioniMassive_Out) {
    this._executions = value;
  }

  get types(): { codice: CheckListEnum; descrizione: string }[] {
    return this._types;
  }

  set types(value: { codice: CheckListEnum; descrizione: string }[]) {
    this._types = value;
  }

  public disableDates = (date: Date): boolean => {
    return date < new Date(this.lastExecutionDate);
  }

  public typeForm: FormGroup;
  public timeFilterForm: FormGroup;
  public dateForm: FormGroup;
  public lastExecutionDate: Date = AGRODATAINIZIO;
  public refreshCheckList$: Subject<void> = new Subject<void>();
  public showMassiveAlert: boolean = false;

  @ViewChild('dateTemplate') public dateTemplate: TemplateRef<any>;
  @ViewChild('checkList') public checkList: ElementRef<any>;

  protected readonly AGRODATAINIZIO: Date = AGRODATAINIZIO;
  protected readonly AGRODATAFINE: Date = AGRODATAFINE;
  protected readonly TODAY: Date = new Date();
  protected readonly faReload = faArrowsRotate;

  private objParametriAgenda: ObjParametriAgenda;
  private _types: { codice: CheckListEnum, descrizione: string; }[] = [];
  private _executions: LeggiElencoElaborazioniMassive_Out;
  private _signal$: Subject<void> = new Subject<void>();
  private _companyAllowed: boolean = false;
  private _massiveQueueAllowed: boolean = false;

  constructor(
    private fb: FormBuilder,
    private translocoService: TranslocoService,
    private gisClient: GisClient,
    private objParametriService: ObjParametriAgendaService,
    private giasMessageService : GiasMessageService,
    private giasDialogService: GiasDialogService,
    private masterService: MasterService,
    private permissionService: PermessiUtenteService,
    private datePipe: DatePipe
  ) { }

  ngOnDestroy(): void {
    this._signal$.next();
  }

  ngOnInit(): void {
    this.typeForm = this.fb.group({
      type: {
        codice: 0,
        descrizione: ''
      }
    });

    let startDate: Date = new Date();
    startDate.setMonth(new Date().getMonth() - 3);

    this.timeFilterForm = this.fb.group({
      start: startDate,
      end: new Date()
    });

    this.dateForm = this.fb.group({
      date: new Date()
    });

    this.objParametriService.currentObjParametriAgenda.pipe(
      takeUntil(this._signal$),
      startWith(this.objParametriService.getObjParamValue())
    ).subscribe(p => {
      this.objParametriAgenda = p;
      this.refreshCheckList$.next();
    });

    this.refreshCheckList$.pipe(takeUntil(this._signal$)).subscribe(() => {
      forkJoin([
        this.companyAllowed(),
        of(this.permissionService.canWritePermesso(enum_Security_Attivita.CalcoloMassivo_Compliance_ISCC_AziendeInVisibilita))
      ]).subscribe(r => {
        this._companyAllowed = r[0].RispostaOK && r[0]?.RispostaStringa;
        this._massiveQueueAllowed = r[1];

        // only if the company is allowed fetch the data
        if (this._companyAllowed) {
          this.fetchChecklistExecutions();
        }
      });
    });

    this.fetchTypes();
  }

  public showCheckList(): boolean {
    // check if there are data to show
    return this._companyAllowed && !!this.executions && this.validCheckListTypeSelected();
  }

  public showQueueBtn(): boolean {
    // check if the company is allowed to execute that type of checklist
    return this._companyAllowed && this.validCheckListTypeSelected()
  }

  public showMassiveQueueBtn(): boolean {
    // check if the company is allowed to execute that type of checklist
    return this._companyAllowed && this.validCheckListTypeSelected() && this._massiveQueueAllowed;
  }

  public enableCompany(): void {
    let params: IncludiEsludiImpresaISCC_In = {
      checkListType: this.typeForm.get('type').value.codice,
      elencoAziende: [{
        piva: this.objParametriAgenda.Piva,
        flag_includi: EnableCompanyEnum.Include
      }]
    };

    if (this.typeForm.get('type').value.codice === CheckListEnum.ISCC) {
      this.masterService.set_isLoading({ isLoading: true, component: this.checkList });

      this.gisClient.gisIncludiEsludiImpresaDaControlloComplianceISCC(params).pipe(take(1)).subscribe(r => {
        if (r.RispostaOK && r.RispostaStringa) {
          this.giasMessageService.successMessage('SuccessoAggiornamentoComplianceISCC', false, true);
        } else {
          this.giasMessageService.errorMessage('ErroreAggiornamentoComplianceISCC', false, true);
        }

        this.refreshCheckList$.next();
        this.masterService.set_isLoading({ isLoading: false });
      });
    } else {
      this.giasMessageService.errorMessage('TipoChecklistNonGestito', false, true);
    }
  }

  public queueExecution(massive: boolean): void {
    let params: SottomettiElaborazioneMassivaGISCheckList_In = {
      checkListType: this.typeForm.get('type').value.codice,
      dataRiferimento: new Date(),
      elencoPiva: massive ? [] : [this.objParametriAgenda.Piva], // if queue execution massive we don't need pivas list
      elaboraAziendeInVisibilita: massive
    };

    this.lastExecutionDate = this.extractLastExcutionDate() ?? AGRODATAINIZIO;
    this.showMassiveAlert = massive;

    this.giasDialogService.dialogMessageObs_Result(
      this.translocoService.translate('DataProgrammazioneEsecuzioneCompliance'),
      this.dateTemplate,
      undefined,
      undefined,
      undefined,
      () => this.disableDates(this.dateForm?.get('date')?.value) // check if selected date is one of the disabled
    ).pipe(
      take(1),
      switchMap((r) => {
        if (r['returnObj']) {
          if (!this.disableDates(this.dateForm?.get('date')?.value)) { // check if selected date is NOT one of the disabled
            this.masterService.set_isLoading({ isLoading: true, component: this.checkList });

            params.dataRiferimento = this.dateForm?.get('date')?.value ?? new Date();

            this.gisClient.gisSottomettiElaborazioneMassivaGISCheckList(params).pipe(
              take(1),
              catchError((a, c) => {
                this.masterService.set_isLoading({ isLoading: false });
                this.giasMessageService.errorMessage(a.message);
                return of(null);
              })
            ).subscribe(r => {
              if (r.RispostaOK && r.RispostaStringa) {
                this.giasMessageService.successMessage('RichiestaSottomessaCorrettamente', false, true);
                this.refreshCheckList$.next(); // immediately reload datas
              } else {
                this.giasMessageService.errorMessage('ErroreSottomissioneRichiestaComplianceISCC', false, true);
              }
              this.masterService.set_isLoading({ isLoading: false });
            });
          } else {
            this.giasDialogService.baseError('DataNonCoerente', 'ErroreDataEsecuzioneISCC');
          }
          params.dataRiferimento = this.dateForm?.get('date')?.value;
        }

        return of(null);
      })
    ).subscribe();
  }

  public onTypeChange(): void {
    this.refreshCheckList$.next();
  }

  public statusClass(esito: string): string {
    switch (esito) {
      case 'OK':
        return 'success';
      case 'KO':
        return 'error';
      case 'IN ELABORAZIONE':
        return 'working';
      default:
        return '';
    }
  }

  public companyName(): string {
    return this.objParametriAgenda.RagSoc;
  }

  public extractLastExcutionDate(): Date {
    let date: Date;
    let dates: Date[] = this.executions?.elenco
      .find(e => e.piva === this.objParametriAgenda.Piva)?.elencoElaborazioniPerTipo
      .flatMap(chks => {
        return chks.elencoElaborazioni.flatMap(e => new Date(e.dataElaborazione));
      })

    if (dates?.length > 0) {
      date = dates.reduce((previous, current, i, a) => {
        let c: number = current.getDate();
        let p: number = previous.getDate();
        return c >= p ? current : previous;
      });
    } else {
      date = AGRODATAINIZIO;
    }

    return date;
  }

  public onTimeFilterChange(): void {
    this.refreshCheckList$.next();
  }

  public validCheckListTypeSelected(): boolean {
    const type = this.typeForm.get('type').value.codice;
    return Object.values(CheckListEnum).includes(type) && type !== CheckListEnum.Null;
  }

  public transformToShortDate(date: Date): string {
    return this.datePipe.transform(date, 'shortDate');
  }

  private timeFilterValidator(): boolean {
    return !!this.timeFilterForm?.get('start')?.value && !!this.timeFilterForm?.get('end')?.value
      && this.timeFilterForm.get('start').value <= this.timeFilterForm.get('end').value
  }

  private fetchChecklistExecutions(): void {
    let params: LeggiElencoElaborazioniMassive_In = {
      checkListType: this.typeForm.get('type').value.codice,
      dataLetturaInizio: this.timeFilterForm.get('start').value,
      dataLetturaFine: this.timeFilterForm.get('end').value,
      piva: this.objParametriAgenda.Piva
    };

    if (this.timeFilterValidator()) {
      if (this.validCheckListTypeSelected()) {
        this.masterService.set_isLoading({isLoading: true, component: this.checkList});
        this.gisClient.gisLeggiElencoElaborazioniMassiveGISCheckList(params).pipe(take(1)).subscribe(r => {
          if (r.RispostaOK) {
            // this.executions = new DummyData().dummyData()
            this.executions = r.RispostaStringa;
          } else {
            this.executions = undefined;
          }
          this.masterService.set_isLoading({isLoading: false});
        });
      } else if (params.checkListType !== CheckListEnum.Null) {
        this.giasMessageService.errorMessage('TipoChecklistNonGestito', false, true);
        // if an invalid type is selected delete all previous data
        this.executions = undefined;
      }
    } else {
      this.giasMessageService.errorMessage('IntervalloTemporaleNonValido', false, true);
      // if an invalid time interval is selected delete all previous data
      this.executions = undefined;
    }
  }

  private fetchTypes(): void {
    // maybe later there will be an API to fetch all the types (Salvatore Zammataro, 28/08/2024)
    this.types = [
      {codice: CheckListEnum.Null, descrizione: ''},
      {codice: CheckListEnum.ISCC, descrizione: 'ISCC'}
    ];
  }

  private companyAllowed() {
    // check if the company is allowed to execute the checklist type
    let params: VerificaAziendaAbilitataISCC_In = {
      checkListType: this.typeForm.get('type').value.codice,
      piva: this.objParametriAgenda.Piva
    }

    return this.gisClient.gisVerificaAziendaAbilitataISCC(params)
      .pipe(take(1));
  }
}
