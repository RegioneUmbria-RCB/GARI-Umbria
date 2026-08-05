import { Component, ElementRef, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroupDirective } from '@angular/forms';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import {generateGridProviders} from 'gias-kendo-grid';

import { Subject, Subscription, takeUntil } from 'rxjs';
import { DatiCatastaliHttpService } from './dati-catastali-http.service';
import { CatastoAppezzamento_Extended, DatiCatastaliService } from './dati-catastali.service';

@Component({
  standalone: false,
  selector: 'app-dati-catastali',
  templateUrl: './dati-catastali.component.html',
  styleUrls: ['./dati-catastali.component.css'],
  providers:[
    ...generateGridProviders(DatiCatastaliHttpService, DatiCatastaliComponent)
  ]
})
export class DatiCatastaliComponent implements OnInit, OnDestroy {
  @Input() formGroupName: string;
  form: FormControl;
  formValue: string;
  objParametriAgenda: ObjParametriAgenda;

  subscriptions: Subscription[] = new Array<Subscription>();

  public signal$: Subject<void> = new Subject();

  public Visualizza_Macrousi: boolean = false;
  public Visualizza_Utilizzi: boolean = false;
  public Visualizza_Varieta: boolean = false;


  constructor(
    private daticatastaliservice: DatiCatastaliService,
    private rootFormGroup: FormGroupDirective,
    private fb: FormBuilder,
    private objParametriAgendaService: ObjParametriAgendaService,
    private ref: ElementRef,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService) {
    this.daticatastaliservice.loading.pipe(takeUntil(this.signal$)).subscribe(val => {
      this.loadingService.set_isLoading({ isLoading: val, component: this.ref })
    });
  }


  ngOnInit() {
    this.form = this.rootFormGroup.control.get(this.formGroupName) as FormControl;
    this.daticatastaliservice.setCatastoAppezzamento(this.form.value);
    this.formValue = JSON.stringify(this.form.value);
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
      this.formValue = JSON.stringify(el);
    }));

    this.form.registerOnChange((el) => {
      let a = 0; //Commento per funzione vuota SonarQube
    });
    this.subscriptions.push(this.daticatastaliservice.currentCatasto.subscribe((newCatastoAppezzamento: CatastoAppezzamento_Extended[]) => {
      this.form.patchValue(newCatastoAppezzamento, { emitEvent: true, onlySelf: false });
    }));

  }

  ngOnDestroy() {
    for (const subs of this.subscriptions) {
      subs.unsubscribe();
    }
    this.signal$.next();
    this.signal$.complete();
  }

  onChangeMacrousi(value: boolean) {
    if (!value) {
      this.Visualizza_Utilizzi = false;
      this.Visualizza_Varieta = false;
    }
    this.changeDatiCatastaliSettings();
  }

  onChangeUtilizzi(value: boolean) {
    if (!value) {
      this.Visualizza_Varieta = false;
    }
    this.changeDatiCatastaliSettings();
  }

  onChangeVarieta(value: boolean) {
    this.changeDatiCatastaliSettings();
  }

  changeDatiCatastaliSettings() {
    let currentDati = this.daticatastaliservice.getCaricaCatastoSettings()
    currentDati.chkMacrousi = this.Visualizza_Macrousi;
    currentDati.chkUtilizzi = this.Visualizza_Utilizzi;
    currentDati.chkVarieta = this.Visualizza_Varieta;
    this.daticatastaliservice.setCaricaCatastoSettings(currentDati);
  }

  mostraMacrousiSwitch(): boolean {
    return this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write;
  }

}

