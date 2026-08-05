import { animate, style, transition, trigger } from '@angular/animations';
import { FormArray } from '@angular/forms';
import { Component, ElementRef, Inject, OnDestroy, ViewChild } from '@angular/core';
import { ConversionService, GiasDialogService, GiasDropDownTemplateService, GiasMultiSelectTemplateService, LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { TerapiaZooFormService } from './terapia-zoo.service';
import { Observable, Subject, takeUntil } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { ActivatedRoute } from '@angular/router';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PrescrizioniClient, TerapieClient } from 'app/Service/net-core6-api.service';
import { GiasMessageService } from 'gias-kendo-grid';
import { cloneDeep } from 'lodash';
import { ZooIntervention, ZooProtocolxIntervento, ZooTherapy } from 'app/zoo/models/zoo-therapies.model';

@Component({
  standalone: false,
  selector: 'app-terapia-zoo',
  templateUrl: './terapia-zoo.component.html',
  styleUrls: ['./terapia-zoo.component.css'],
  providers: [
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
    TerapiaZooFormService
  ],
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [ // Quando l'elemento entra
        style({ opacity: 0 }), // Stato iniziale
        animate('300ms ease-in', style({ opacity: 1 })), // Transizione
      ]),
      transition(':leave', [ // Quando l'elemento esce
        animate('300ms ease-out', style({ opacity: 0 })) // Transizione inversa
      ]),
    ]),
  ]
})
export class TerapiaZooComponent implements OnDestroy {
  // @ViewChild(TabInterventoComponent) tabInterventoComponent!: TabInterventoComponent;

  openInIFrame: boolean = false;
  signal: Subject<void> = new Subject();

  public selectedTabIndex: number = 0;

  constructor(
    public terapiaFormService: TerapiaZooFormService,
    private agendaService: ObjParametriAgendaService,
    private giasDialogService: GiasDialogService,
    private giasMessageService: GiasMessageService,
    private terapiaService: TerapieClient,
    private conversionService: ConversionService,
    private prescriptionsService: PrescrizioniClient,
    private route: ActivatedRoute,
    private elementRef: ElementRef,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService,
    private transloco: TranslocoService
  ) {
    this.initializePageData();
    this.handleQueryParams();

    this.subscribeToInterventionAdded();
  }

  private initializePageData(): void {
    this.terapiaFormService.loadCentriAziendaliDDL();

    if (!this.terapiaFormService.isNewMode && this.terapiaFormService.objP_Agenda.GenericObj_string) {
      const strTherapy = JSON.parse(this.terapiaFormService.objP_Agenda.GenericObj_string);
      this.terapiaFormService.therapyObj = this.conversionService.ConversionDateInObject(strTherapy);

      this.terapiaFormService.formTerapiaZoo.get('CentroAziendale').disable({ emitEvent: false });
      this.terapiaFormService.formTerapiaZoo.get('Stalla').disable({ emitEvent: false });
    }
  }

  private disableFormFields(): void {
    if (this.terapiaFormService.isInfoMode) {
      this.terapiaFormService.formTerapiaZoo.get('CentroAziendale').disable({ emitEvent: false });
      this.terapiaFormService.formTerapiaZoo.get('Stalla').disable({ emitEvent: false });
      this.terapiaFormService.formTerapiaZoo.get('Nome').disable({ emitEvent: false });
      this.terapiaFormService.formTerapiaZoo.get('Interventi').disable({ emitEvent: false });
    }
  }

  private handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntil(this.signal))
      .subscribe(params => {
        if (params.seFrame == 1)
          this.openInIFrame = true;
        if (params.t_Tab) {
          // this.terapiaFormService.typeTab_Zootecnia = parseInt(params.t_Tab) as enum_TypeTab_Zootecnia;
          this.disableFormFields();
        }
    });
  }

  private subscribeToInterventionAdded() {
    this.terapiaFormService.interventionAdded$
      .pipe(takeUntil(this.signal))
      .subscribe((newIndex) => {
        setTimeout(() => this.selectedTabIndex = newIndex);
      });
  }

  get interventiFormArray(): FormArray {
    return this.terapiaFormService.interventiFormArray;
  }

  addIntervento(): void {
    this.terapiaFormService.addIntervento();
  }

  removeIntervento(index: number, event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.terapiaFormService.removeIntervento(index);
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  onKeyDownEnter(event: KeyboardEvent) {
    event.preventDefault();
  }

  isFormValid(): boolean {
    return this.terapiaFormService.formTerapiaZoo.valid || this.terapiaFormService.formTerapiaZoo.disabled;
  }

  private preSaveCheck(): boolean {
    if (this.interventiFormArray.length === 0) {
      this.giasDialogService.baseError("", this.transloco.translate('zoo.SelezionareAlmenoUnIntervento'), false);
      return false;
    }

    if (!this.interventiFormArray.valid) {
      this.giasDialogService.baseError("", this.transloco.translate('zoo.DatiInterventoNonValidi'), false);
      return false;
    }
    return true;
  }

  private getTherapyToSave() {
    let cloned = cloneDeep(this.terapiaFormService.therapyObj);
    let therapyToSave = new ZooTherapy();

    if(this.terapiaFormService.isNewMode)
      therapyToSave.Id_Terapia = 0;
    else
      therapyToSave.Id_Terapia = cloned.Id_Terapia;
      therapyToSave.Piva = this.terapiaFormService.objP_Agenda.Piva;
      therapyToSave.Sa_Cod = this.terapiaFormService.formTerapiaZoo.get('CentroAziendale').value?.codice ?? 0;
      therapyToSave.Sta_Num = this.terapiaFormService.formTerapiaZoo.get('Stalla').value?.Sta_Num ?? 0;
      therapyToSave.Descrizione = this.terapiaFormService.formTerapiaZoo.get('Nome').value ?? '';

      therapyToSave.Interventi = this.terapiaFormService.getInterventiToSave().map(interventoForm => {
        let interventoToSave = new ZooIntervention();
        interventoToSave.Id_Intervento = interventoForm.Id ?? 0;
        interventoToSave.Descrizione = interventoForm.Nome;
        interventoToSave.Ordine = interventoForm.Ordine;
        interventoToSave.Protocolli = interventoForm.Protocolli.map(protocolloForm => {
          let protocolloToSave = new ZooProtocolxIntervento();
          protocolloToSave.Id_Protocollo = protocolloForm.Id;
          protocolloToSave.Id_Protocollo_Alt = protocolloForm.Prot_Alt ?? 0;
          return protocolloToSave;
        });
        return interventoToSave;
      }) ?? [];
    return therapyToSave;
  }

  saveEditTherapy() {
    if (!this.isFormValid()) {
      this.terapiaFormService.formTerapiaZoo.markAllAsTouched();
      return;
    }
    if (!this.preSaveCheck()) return;

    const param = this.getTherapyToSave();
    this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });

    let obs: Observable<any>;

    if (this.terapiaFormService.isUpdateMode)
        obs = this.terapiaService.terapieUpdateTerapia(param);
    else if (this.terapiaFormService.isNewMode)
        obs = this.terapiaService.terapieCreateTerapia(param);
    else
      throw new Error("Invalid operation mode");

    obs
      .pipe(takeUntil(this.signal))
      .subscribe({
        next: (response) => {
          this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
          if (response.RispostaOK) {
            this.giasMessageService.successMessage(this.transloco.translate('zoo.TerapiaSalvataConSuccesso'));
            this.terapiaFormService.goToGridPage();
          } else {
            this.giasDialogService.baseError("", response.Errore, false);
          }
        },
        error: (err) => {
          this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
          this.giasDialogService.baseError("", this.transloco.translate('zoo.ErroreSalvataggioTerapia'), false);
        }
    });
  }
}
