import {Component, OnDestroy, OnInit} from '@angular/core';
import { ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import {FormGroup} from '@angular/forms';
import {ConfrontoPianoColturaleService} from './confronto-piano-colturale.service';
import {ConfrontoPianoColturaleFormsService} from './confronto-piano-colturale-forms.service';
import {ConfrontoPianoColturaleDataService} from './confronto-piano-colturale-data.service';
import {Subject, tap} from 'rxjs';
import {IConfrontoPianoColturale, PianoColturale} from '../../Model/confronto-piano-colturale/confronto-piano-colturale';
import {GiasDialogService} from '../../Service/gias-dialog.service';
import {IntlService} from "@progress/kendo-angular-intl";
import {takeUntil} from "rxjs/operators";
import {IntervalloTemporale} from '../../Model/anagrafiche/IntervalloTemporale';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-confronto-piano-colturale',
  templateUrl: './confronto-piano-colturale.component.html',
  styleUrls: ['./confronto-piano-colturale.component.css'],
  providers: [ConfrontoPianoColturaleService, ConfrontoPianoColturaleFormsService, ConfrontoPianoColturaleDataService]
})
export class ConfrontoPianoColturaleComponent implements OnInit, OnDestroy {

  protected confrontoPCForm: FormGroup;

  private objParametriAgenda: ObjParametriAgenda;
  private signal$: Subject<void> = new Subject<void>();

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private confrontoPianoColturaleFormsService: ConfrontoPianoColturaleFormsService,
    private confrontoPianoColturaleDataService: ConfrontoPianoColturaleDataService,
    private intlService: IntlService,
    private giasDialogService: GiasDialogService
  ) {
    this.initConfrontoPCForm();
  }

  ngOnDestroy(): void {
    this.signal$.next();
  }

  ngOnInit(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.confrontoPCForm.valueChanges.pipe(
      takeUntil(this.signal$),
      tap((val) => {
        let cPC: IConfrontoPianoColturale = this.confrontoPianoColturaleDataService.confrontoPC;
        cPC = this.confrontoPCForm.getRawValue();
        this.confrontoPianoColturaleDataService.confrontoPC = cPC;
      })
    ).subscribe();
  }

  protected companySelected(): boolean {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }

  protected isPlanSelected(): boolean {
    let plan: PianoColturale = this.confrontoPianoColturaleDataService.pianoColturale;
    return plan != undefined && plan.Programmazione_Cod > 0;
  }

  // protected onChangeVarietaSwitch(value): void {
  //   let cPC: IConfrontoPianoColturale = this.confrontoPianoColturaleDataService.confrontoPC;
  //   cPC.considerVarieta = value;
  //   this.confrontoPianoColturaleDataService.confrontoPC = cPC;
  // }
  //
  // protected onChangeCatastoSwitch(value): void {
  //   let cPC: IConfrontoPianoColturale = this.confrontoPianoColturaleDataService.confrontoPC;
  //   cPC.considerCatasto = value;
  //   this.confrontoPianoColturaleDataService.confrontoPC = cPC;
  // }

  protected loadComparison(): void {
    let plan: PianoColturale = this.confrontoPianoColturaleDataService.pianoColturale;
    if (plan != undefined && plan.Programmazione_Cod > 0) {
      this.confrontoPianoColturaleDataService.emitLoadComparison();
    } else {
      this.giasDialogService.baseInfo('', 'SelezionareUnPlanning');
    }
  }

  private initConfrontoPCForm(): void {
    this.confrontoPCForm = this.confrontoPianoColturaleFormsService.getConfrontoPCForm();
    //this.confrontoPCForm.controls['considerCatasto'].disable();
    let anno = new Date().getFullYear();
    let dataInizio = this.intlService.parseDate('01/01/' + anno);
    let dataFine = this.intlService.parseDate('31/12/' + anno);
    (<FormGroup>this.confrontoPCForm.controls['validita']).controls['inizio'].setValue(dataInizio);
    (<FormGroup>this.confrontoPCForm.controls['validita']).controls['fine'].setValue(dataFine);
    this.confrontoPianoColturaleDataService.confrontoPC = {
      considerCatasto: this.confrontoPCForm?.controls['considerCatasto']?.value ?? true,
      considerVarieta: this.confrontoPCForm?.controls['considerVarieta']?.value ?? false,
      validita: new IntervalloTemporale(dataInizio, dataFine)
    };
  }

}
