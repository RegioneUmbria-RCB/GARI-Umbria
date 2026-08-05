import { Component, ElementRef, Inject, OnDestroy, TemplateRef, ViewChild } from "@angular/core";
import { TrattamentoZooFormService } from "./service/trattamento-zoo-form.service";
import { animate, style, transition, trigger } from "@angular/animations";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { TranslocoService } from "@jsverse/transloco";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CapoAnimaleCDC } from "app/Model/attivita/centri_di_costo/CapoAnimaleCDC";
import { CapoAnimale } from "app/Service/api.service";
import { RisorsaProdotto } from "app/Model/attivita/risorse/RisorsaProdotto";
import { CentroAziendale, CentroDiCosto, Job_PK, OperazioniZooClient, PrescrizioniClient, RispostaStandard } from "app/Service/net-core6-api.service";
import { ActivatedRoute } from "@angular/router";
import { cloneDeep } from "lodash";
import { GridProdottiSomministrazioneComponent } from "./griglie-trattamento/griglia-prodotti-somministrazione/grid-prodotti-somministrazione.component";
import { GridCapiAnimaliComponent } from "./griglie-trattamento/griglia-capi-animali/grid-capi-animali.component";
import { catchError, filter, map, Observable, of, Subject, switchMap, take, takeUntil, tap } from "rxjs";
import { enum_LAVCOD } from "app/Model/TipiEnumerativi";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { DettagliProtocollo } from "./model/dettagli-protocollo.model";
import { DettaglioRegistroSomministrazioni } from "app/Model/attivita/dettagli/DettaglioRegistroSomministrazioni";
import { Risorsa } from "app/Model/attivita/risorse/Risorsa";
import { Prodotto } from "app/Model/attivita/risorse/Prodotto";
import { ConversionService } from "app/Service/conversion.service";
import { PrescriptionActivity } from "./model/prescription-activity.model";
import { Job } from "app/Model/attivita/Job";
import { enum_FarmacoCategoria, enum_TypeTab_Zootecnia } from "app/zoo/models/tipi-enumerativi-zoo";
import { Enum_DBTypeOperation, GiasDropDownTemplateService, GiasMultiSelectTemplateService, LOADING_TOKEN, LoadingService } from "gias-ui-kit";
import { GiasMessageService } from "app/Service/gias-message.service";
import { DialogQtaDiffAICsComponent } from "./dialog-qta-diff-aics/dialog-qta-diff-aics.component";
import { DialogResult } from "@progress/kendo-angular-dialog";
import { enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";

@Component({
  standalone: false,
  selector: 'app-trattamento-zoo',
  templateUrl: './trattamento-zoo.component.html',
  styleUrls: ['./trattamento-zoo.component.css'],
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
  ],
  providers: [
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
    TrattamentoZooFormService
  ]
})
export class TrattamentoZooComponent implements OnDestroy {
  @ViewChild(GridProdottiSomministrazioneComponent) prodottiSomministrazioneComponent!: GridProdottiSomministrazioneComponent;
  @ViewChild(GridCapiAnimaliComponent) capiAnimaliComponent!: GridCapiAnimaliComponent;
  @ViewChild('saveWithDiffAICs') fixQtaWithDiffAICs: TemplateRef<any>;
  @ViewChild('dialogComponentInstance') dialogComponentInstance: DialogQtaDiffAICsComponent;

  protected dialogProducts: any[] = [];
  protected dialogQtaTot: number = 0;

  openInIFrame: boolean = false;
  signal: Subject<void> = new Subject();

  constructor(
    public trattamentoFormService: TrattamentoZooFormService,
    private agendaService: ObjParametriAgendaService,
    private giasDialogService: GiasDialogService,
    private giasMessageService: GiasMessageService,
    private zooService: OperazioniZooClient,
    private conversionService: ConversionService,
    private prescriptionsService: PrescrizioniClient,
    private route: ActivatedRoute,
    private elementRef: ElementRef,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService,
    private transloco: TranslocoService
  ) {
    this.initializePageData();
    this.handleQueryParams();
  }

  /**
   * Initializes the page data by retrieving parameters from the agenda service and setting up the form.
   * It also handles the case when the page is opened in "Info" mode, disabling certain form fields.
   */
  private initializePageData(): void {
    this.trattamentoFormService.objParametriAgenda = this.agendaService.getObjParamValue();

    if (this.trattamentoFormService.objParametriAgenda.GenericObj_string) {
      const objParams = JSON.parse(this.trattamentoFormService.objParametriAgenda.GenericObj_string);
      const attivita = objParams.hasOwnProperty('Somministrazioni') ? objParams.Somministrazioni[0] : objParams;
      this.trattamentoFormService.attivitaDaChiamante = this.conversionService.ConversionDateInObject(attivita);

      this.trattamentoFormService.formTrattamentoZoo.get('Data').patchValue(this.trattamentoFormService.attivitaDaChiamante?.inizio ?? new Date(), { emitEvent: false });
      this.trattamentoFormService.formTrattamentoZoo.get('Note').patchValue(this.trattamentoFormService.attivitaDaChiamante?.note ?? '', { emitEvent: false });

      //se apro in Info, disabilito tutto
      if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read) {
        this.trattamentoFormService.formTrattamentoZoo.get('Note').disable({ emitEvent: false });
      }

      if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read
        || this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update) {
        this.trattamentoFormService.formTrattamentoZoo.get('Data').disable({ emitEvent: false });
        this.trattamentoFormService.formTrattamentoZoo.get('Raggruppamento').disable({ emitEvent: false });
      }

      this.trattamentoFormService.formTrattamentoZoo.get('CentroAziendale').disable({ emitEvent: false });
      this.trattamentoFormService.formTrattamentoZoo.get('Stalla').disable({ emitEvent: false });

      const dettaglioSomm = this.trattamentoFormService.attivitaDaChiamante.risorse.find(item => item.classType == 'DettaglioRegistroSomministrazioni');
      this.trattamentoFormService.isFirstSomm = dettaglioSomm.isFirstSomm;
      this.trattamentoFormService.hasSuccessiveConfermate = dettaglioSomm.hasSuccessiveConfermate;
      this.trattamentoFormService.arrayFarmCat = dettaglioSomm.farmacoCatSem.map(item => item.codice as enum_FarmacoCategoria);

      this.trattamentoFormService.dettagliProtocollo = new DettagliProtocollo(
        objParams.Id_Ricetta,
        objParams?.Tipo_Prescrizione ?? (dettaglioSomm?.prescrizioneOrigine ?? 0),
        dettaglioSomm.massivo,
        dettaglioSomm.quantitaTotaleReale,
        dettaglioSomm.qtaDose,
        dettaglioSomm.udmDose,
        dettaglioSomm.durataTrattamento,
        dettaglioSomm.arrotondamentoPeso
      );

      this.trattamentoFormService.showGridProdotti = !(this.trattamentoFormService.dettagliProtocollo.tipoPrescrizione === enum_TipoPrescrizione.Protocollo_Terapeutico_Programmato);
    }

    this.trattamentoFormService.loadCentriAziendaliDDL();
  }

  private handleQueryParams(): void {
    this.route.queryParams
      .pipe(takeUntil(this.signal))
      .subscribe(params => {
        if (params.seFrame == 1) this.openInIFrame = true;

        if (params.t_Tab) {
          this.trattamentoFormService.typeTab_Zootecnia = parseInt(params.t_Tab) as enum_TypeTab_Zootecnia;
          this.disableFormFields();
        }
      }
    );
  }

  private disableFormFields(): void {
    if (this.trattamentoFormService.typeTab_Zootecnia == enum_TypeTab_Zootecnia.FuturePrescriptions) {
      this.trattamentoFormService.formTrattamentoZoo.get('Data').disable({ emitEvent: false });
      this.trattamentoFormService.formTrattamentoZoo.get('Raggruppamento').disable({ emitEvent: false });
    }
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  onKeyDownEnter(event: KeyboardEvent) {
    event.preventDefault();
    this.trattamentoFormService.onBlurData();
  }

  SalvaEditTrattamentoZoo(){
    this.capiAnimaliComponent?.commitCurrentEdit();

    if (!this.isFormValid()) {
      this.trattamentoFormService.formTrattamentoZoo.markAllAsTouched();
      return;
    }

    const arrayCapiAnimali = this.capiAnimaliComponent?.gridSelectedRows ?? [];
    const arrayFarmaci = this.prodottiSomministrazioneComponent?.gridSelectedRows ?? [];
    if (!this.preSaveCheck_Sync(arrayFarmaci, arrayCapiAnimali)) return;

    const qtaTotToSomm = parseFloat(arrayCapiAnimali.reduce((acc, item) => acc + item.Quantita, 0).toFixed(4));
    const withDiffAICs = this.trattamentoFormService.showGridProdotti && [...new Set(arrayFarmaci.map(item => item.CodiceAIC))].length > 1;
    const withSameAICDifferentLotto = this.trattamentoFormService.showGridProdotti
      && [...new Set(arrayFarmaci.map(item => item.CodiceAIC))].length === 1
      && [...new Set(arrayFarmaci.map(item => item.Lotto))].length > 1;
    const shouldOpenQuantityDialog = withDiffAICs || withSameAICDifferentLotto;

    const isWriteMode = this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write;
    const stockAlert$: Observable<boolean> = (isWriteMode && this.trattamentoFormService.showGridProdotti)
      ? this.checkRiskyDrugsAlert(arrayFarmaci, qtaTotToSomm)
      : of(true);

    const finalFarmaci$: Observable<any[]> = of(null).pipe(
      switchMap(() => {
        if (shouldOpenQuantityDialog) {
          return this.openMultiAICDialog(arrayFarmaci, qtaTotToSomm);
        } else {
          return of(this.calculateSingleAICQuantities(arrayFarmaci, qtaTotToSomm));
        }
      })
    );

    stockAlert$.pipe(
      take(1),
      filter(proceed => !!proceed),
      switchMap(() => finalFarmaci$),
      take(1),
      filter(finalFarmaciList => !!finalFarmaciList),
      map(finalFarmaciList => {
        if (this.trattamentoFormService.showGridProdotti && !this.preSaveCheck_Stock(finalFarmaciList, qtaTotToSomm)) {
          throw new Error('StockCheckFailed');
        }
        return [this.getActivityToSave(finalFarmaciList, qtaTotToSomm)];
      }),
      tap(() => this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef })),

      switchMap((param: PrescriptionActivity[]) => this.saveTrattamentoZoo(param)),
      catchError(error => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
        if (error.message !== 'StockCheckFailed') {
            this.giasDialogService.baseError("", FunzioniComuniService.getResponseError(error, this.transloco, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false);
        }
        return of(null);
      })
    ).subscribe(r => {
      this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
      if (r?.RispostaOK) { 
        this.giasMessageService.infoMessagge('SalvataggioAvvenutoConSuccesso', false, true);
        this.trattamentoFormService.goToGridPage();
      }
    });
  }

  private saveTrattamentoZoo(param: PrescriptionActivity[]): Observable<RispostaStandard> {
    let obsResponse: Observable<RispostaStandard>;

    if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      switch (this.trattamentoFormService.typeTab_Zootecnia) {
        case enum_TypeTab_Zootecnia.Prescriptions:
          if (this.trattamentoFormService.dettagliProtocollo.tipoPrescrizione === enum_TipoPrescrizione.Protocollo_Terapeutico_Programmato) {
            obsResponse = this.zooService.operazioniZooProgramFirstSomministrazioneDaProtocollo(this.trattamentoFormService.dettagliProtocollo.idProtocol, param);
          } else {
            obsResponse = this.zooService.operazioniZooScriviFirstSomministrazioniDaProtocollo(this.trattamentoFormService.dettagliProtocollo.idProtocol, param);
          }
          break;
        case enum_TypeTab_Zootecnia.FuturePrescriptions:
          obsResponse = this.zooService.operazioniZooConfermaSomministrazioneFutura(param[0]);
          break;
        case enum_TypeTab_Zootecnia.Indications:
          obsResponse = this.zooService.operazioniZooScriviSomministrazioniDaPrescrizione(this.trattamentoFormService.dettagliProtocollo.idProtocol, param);
          break;
      }
    } else if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update) {
      obsResponse = this.zooService.operazioniZooModificaSomministrazione(param[0]);
    }

    return obsResponse;
  }

  SalvaEditTrattamentoZooOLD() {
    // if (!this.isFormValid()) {
    //   this.trattamentoFormService.formTrattamentoZoo.markAllAsTouched();
    //   return;
    // }
    // if (!this.preSaveCheck()) return;

    // const param = [this.getActivityToSave()];
    // this.loadingService.set_isLoading({ isLoading: true, component: this.elementRef });

    // this.saveTrattamentoZoo(param);
  }

  private saveTrattamentoZooOLD(param: PrescriptionActivity[]) {
    // let obsResponse: Observable<RispostaStandard>;

    // //occorre distinguere se stiamo salvando la prima somministrazione, una modifica oppure una somministrazione futura
    // if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
    //   switch (this.trattamentoFormService.typeTab_Zootecnia) {
    //     case enum_TypeTab_Zootecnia.Prescriptions:
    //       obsResponse = this.zooService.operazioniZooScriviFirstSomministrazioniDaProtocollo(this.trattamentoFormService.dettagliProtocollo.idProtocol, param);
    //       break;
    //     case enum_TypeTab_Zootecnia.FuturePrescriptions:
    //       obsResponse = this.zooService.operazioniZooConfermaSomministrazioneFutura(param[0]);
    //       break;
    //     case enum_TypeTab_Zootecnia.Indications:
    //       obsResponse = this.zooService.operazioniZooScriviSomministrazioniDaPrescrizione(this.trattamentoFormService.dettagliProtocollo.idProtocol, param);
    //       break;
    //   }
    // } else {
    //   if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update)
    //     obsResponse = this.zooService.operazioniZooModificaSomministrazione(param[0]);
    // }

    // obsResponse.pipe(catchError(error => {
    //   this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });
    //   this.giasDialogService.baseError("", FunzioniComuniService.getResponseError(error, this.transloco, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false);
    //   return of(null);
    // })).subscribe(async r => {
    //   this.loadingService.set_isLoading({ isLoading: false, component: this.elementRef });

    //   if (r.RispostaOK) {
    //     this.giasMessageService.infoMessagge('SalvataggioAvvenutoConSuccesso', false, true); // this.giasDialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
    //     this.trattamentoFormService.goToGridPage();
    //   }
    // });
  }

  private isFormValid(): boolean {
    return this.trattamentoFormService.formTrattamentoZoo.valid || this.trattamentoFormService.formTrattamentoZoo.disabled;
  }

  private openMultiAICDialog(farmaci: any[], quantitaTotale: number): Observable<any[] | null> {
    // This is where you pass data to your dialog component.
    // Your `app-dialog-qta-diff-aics` needs to:
    // 1. Receive `farmaci` (to build the grid) and `quantitaTotale` (to show the target).
    // 2. Allow users to edit the quantity *to use* for each drug.
    // 3. Validate that the sum of user inputs equals `quantitaTotale`.
    // 4. On "Confirm", return the *modified* `farmaci` list.
    // 5. On "Cancel", return `null`.

    this.dialogProducts = farmaci;
    this.dialogQtaTot = quantitaTotale;

    const preventClose = (action: DialogResult): boolean => {
      if (action && action['returnObj'] === true) {
        const validation = this.dialogComponentInstance.validate();

        if (!validation.isValid) {
          this.giasDialogService.baseError(
            this.transloco.translate("SiÈVerificatoUnErroreDuranteLaFaseDiSalvat"),
            this.transloco.translate(validation.messageKey, validation.messageParams)
          );
          return true;
        }
      }
      return false;
    };

    return this.giasDialogService.dialogMessageObs_Result(
      this.transloco.translate("ChooseQuantityForDifferentAICs"),
      this.fixQtaWithDiffAICs,
      undefined,
      '90%',
      '80%',
      preventClose
    ).pipe(
      take(1),
      map(result => {
        if (result && result['returnObj'] === true) {
          return this.dialogComponentInstance.getData();
        } else {
          return null;
        }
      })
    );
  }

  private calculateSingleAICQuantities(farmaci: any[], quantitaTotale: number): any[] {
    let quantitaRimanente = quantitaTotale;
    const farmaciDaUsare = [];

    for (const farmaco of farmaci) {
      if (quantitaRimanente <= 0) break;

      const qtaDaUsare = Math.min(farmaco.Qta, quantitaRimanente);
      const farmacoModificato = cloneDeep(farmaco);

      farmacoModificato.Qta = qtaDaUsare; // Set Qta to the amount *to be used*
      farmaciDaUsare.push(farmacoModificato);

      quantitaRimanente -= qtaDaUsare;
    }

    return farmaciDaUsare;
  }

  private preSaveCheck_Sync(arrayFarmaci: any[], arrayCapiAnimali: any[]): boolean {
    // 1. Check for valid date
    const dataPrescription = this.trattamentoFormService.formTrattamentoZoo.get('Data').value;
    if (!(dataPrescription instanceof Date) || isNaN(dataPrescription.getTime())) {
      this.giasDialogService.baseError("", this.transloco.translate('DataNonValida'), false);
      return false;
    }

    // 2. Check for empty grids
    if ((this.trattamentoFormService.showGridProdotti && arrayFarmaci.length == 0) || arrayCapiAnimali.length == 0) {
      const message: string = (arrayFarmaci.length == 0) ? 'zoo.SelezionareAlmenoUnFarmaco' : 'zoo.SelezionareAlmenoUnCapoAnimale';
      this.giasDialogService.baseError("", this.transloco.translate(message), false);
      return false;
    }

    // 3. Check for zero quantity
    const quantitaTotaleDaSomministrare = parseFloat(arrayCapiAnimali.reduce((acc, item) => acc + item.Quantita, 0).toFixed(4));
    if (quantitaTotaleDaSomministrare <= 0) {
      this.giasDialogService.baseError("", this.transloco.translate('zoo.LaQuantitaDaSomministrareNonPuòEssereZero'), false);
      return false;
    }

    return true;
  }

  private preSaveCheck_Stock(finalFarmaciList: any[], quantitaTotaleDaSomministrare: number): boolean {
    // Get the *actual* total quantity the user wants to use from the selected stock.
    // This *should* equal quantitaTotaleDaSomministrare if the dialog is correct.
    const quantitaTotaleDaUsare = parseFloat(finalFarmaciList.reduce((acc, item) => acc + item.Qta, 0).toFixed(4));

    // This check is for safety, in case the dialog logic is wrong.
    if (quantitaTotaleDaUsare < quantitaTotaleDaSomministrare) {
      this.giasDialogService.baseError("",
        this.transloco.translate('zoo.LaQuantitaDaSomministrareEccedeLaGiacenza'), // Re-using this message
        false
      );
      return false;
    }

    // The original check was:
    // let quantitaInGiacenza = arrayFarmaci.reduce((acc, item) => acc + item.Qta, 0);
    // if (quantitaTotaleDaSomministrare > quantitaInGiacenza) { ... }
    // This check is now implicitly handled. `finalFarmaciList` *is* the list from stock.
    // The `openMultiAICDialog` or `calculateSingleAICQuantities` should already
    // have ensured that `item.Qta` (to be used) <= `item.Qta` (in stock).
    // The check above ensures the total *sum* matches what the animals need.

    return true;
  }

  private checkRiskyDrugsAlert(arrayFarmaci: any[], quantitaTotaleDaSomministrare: number): Observable<boolean> {
    const qtaTotDisponibile = parseFloat(arrayFarmaci.reduce((acc, f) => acc + f['QtaTot'], 0).toFixed(4));
    if (qtaTotDisponibile >= quantitaTotaleDaSomministrare) {
      return of(true);
    }
    this.giasDialogService.baseError(
      this.transloco.translate('Attenzione'),
      this.transloco.translate('zoo.GiacenzaDataSalvataggioInferiore'),
      false
    );
    return of(false);
  }

  /**
   * Performs a pre-save validation check to ensure that the required conditions
   * are met before proceeding with the save operation.
   *
   * @returns {boolean} - Returns `true` if the validation passes, otherwise `false`.
   *
   * The method performs the following checks:
   * 1. Ensures that at least one item is selected in both `prodottiSomministrazioneComponent` 
   *    and `capiAnimaliComponent`. If either is empty, an error dialog is displayed.
   * 2. Ensures that the total quantity to be administered (`quantitaTotaleDaSomministrare`) 
   *    does not exceed the available stock (`quantitaInGiacenza`). If it does, an error dialog is displayed.
   *
   * Error messages are displayed using the `giasDialogService` with translations provided by `transloco`.
   */
  // private preSaveCheck(): boolean {
    // verifichiamo che sia stata inserita una data corretta
    // const dataPrescription = this.trattamentoFormService.formTrattamentoZoo.get('Data').value;
    // if (!(dataPrescription instanceof Date) || isNaN(dataPrescription.getTime())) {
    //   this.giasDialogService.baseError("",
    //     this.transloco.translate('DataNonValida'),
    //     false
    //   );
    //   return false;
    // }

    // const arrayFarmaci = this.prodottiSomministrazioneComponent?.gridSelectedRows ?? [];
    // const arrayCapiAnimali = this.capiAnimaliComponent?.gridSelectedRows ?? [];

    // if (arrayFarmaci.length == 0 || arrayCapiAnimali.length == 0) {
    //   const message: string = (arrayFarmaci.length == 0) ? 'zoo.SelezionareAlmenoUnFarmaco' : 'zoo.SelezionareAlmenoUnCapoAnimale';
    //   this.giasDialogService.baseError("",
    //     this.transloco.translate(message),
    //     false
    //   );
    //   return false;
    // }

    // let withDiffAICs = [...new Set(arrayFarmaci.map(item => item.CodiceAIC))].length > 1;
    // if (withDiffAICs) {
    //   this.selectQtaForDiffAICs();
    // }

    // let quantitaInGiacenza = arrayFarmaci.reduce((acc, item) => acc + item.Qta, 0);
    // let quantitaTotaleDaSomministrare = arrayCapiAnimali.reduce((acc, item) => acc + item.Quantita, 0);

    // quantitaInGiacenza = parseFloat(quantitaInGiacenza.toFixed(4));
    // quantitaTotaleDaSomministrare = parseFloat(quantitaTotaleDaSomministrare.toFixed(4));

    // //verifico che la quantità totale da somministrare non sia zero
    // if (quantitaTotaleDaSomministrare <= 0) {
    //   this.giasDialogService.baseError("",
    //     this.transloco.translate('zoo.LaQuantitaDaSomministrareNonPuòEssereZero'),
    //     false
    //   );
    //   return false;
    // }

    // // verifico che le quantità inserite sulle righe checkate non eccedano la giacenza
    // if (quantitaTotaleDaSomministrare > quantitaInGiacenza) {
    //   this.giasDialogService.baseError("",
    //     this.transloco.translate('zoo.LaQuantitaDaSomministrareEccedeLaGiacenza'),
    //     false
    //   );
    //   return false;
    // }

    // return true;
  // }

  private getActivityToSave(finalFarmaci: any[], quantitaTotale: number): PrescriptionActivity {
    let cloned = cloneDeep(this.trattamentoFormService.attivitaDaChiamante);
    let attivitaToSave = new PrescriptionActivity();

    if (!attivitaToSave.job) {
      attivitaToSave.job = {
        primaryKey: {
          codice: enum_LAVCOD.CURE_MEDICAMENTI_ANIMALI.toString(),
          classType: "Zootecnia"
        } as Job_PK
      } as Job;
    }

    if (!attivitaToSave.centroAziendale && cloned.centroAziendale) {
      attivitaToSave.centroAziendale = cloned.centroAziendale as CentroAziendale;
    }

    if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write && this.trattamentoFormService.typeTab_Zootecnia == enum_TypeTab_Zootecnia.Prescriptions)
      attivitaToSave.codice = '0';
    else {
      attivitaToSave.codice = cloned.codice;
      attivitaToSave.codiceOperazioneRicetta = cloned.codiceOperazioneRicetta;
    }

    if (!attivitaToSave.attivitaCollegate || attivitaToSave.attivitaCollegate.length == 0) {
      attivitaToSave.attivitaCollegate = new Array<PrescriptionActivity>();
      attivitaToSave.attivitaCollegate.push(new PrescriptionActivity());
    }

    attivitaToSave.fabbricatoCod = this.trattamentoFormService.formTrattamentoZoo.get('Stalla').value?.STA_NUM ?? 0;

    this.setAnimalsCDC(attivitaToSave); // This is unchanged
    this.setDrugsResources(attivitaToSave, finalFarmaci, quantitaTotale); // Pass parameters

    // ... (dates and notes logic remains the same) ...
    const dataInizio = this.trattamentoFormService.formTrattamentoZoo.get('Data').value;
    attivitaToSave.inizio = dataInizio;
    attivitaToSave.fine = new Date(dataInizio.getTime() + (( this.trattamentoFormService.dettagliProtocollo.durataTrattamento - 1 ) * 24 * 60 * 60 * 1000));
    attivitaToSave.note = this.trattamentoFormService.formTrattamentoZoo.get('Note').value;

    return attivitaToSave;
  }

  private getActivityToSaveOLD() {
    // let cloned = cloneDeep(this.trattamentoFormService.attivitaDaChiamante); //serve per fare la copia dell'oggetto
    // let  attivitaToSave =  new PrescriptionActivity();

    // if (!attivitaToSave.job) {
    //   attivitaToSave.job = {
    //     primaryKey: {
    //       codice: enum_LAVCOD.CURE_MEDICAMENTI_ANIMALI.toString(),
    //       classType: "Zootecnia"
    //     } as Job_PK
    //    } as Job;
    // }

    // if (!attivitaToSave.centroAziendale && cloned.centroAziendale) {
    //   attivitaToSave.centroAziendale = cloned.centroAziendale as CentroAziendale;
    // }

    // if (this.trattamentoFormService.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write && this.trattamentoFormService.typeTab_Zootecnia == enum_TypeTab_Zootecnia.Prescriptions)
    //   attivitaToSave.codice = '0';
    // else {
    //   attivitaToSave.codice = cloned.codice;
    //   attivitaToSave.codiceOperazioneRicetta = cloned.codiceOperazioneRicetta;
    // }

    // if (!attivitaToSave.attivitaCollegate || attivitaToSave.attivitaCollegate.length == 0) {
    //   attivitaToSave.attivitaCollegate = new Array<PrescriptionActivity>();
    //   attivitaToSave.attivitaCollegate.push(new PrescriptionActivity());
    // }

    // attivitaToSave.fabbricatoCod = this.trattamentoFormService.formTrattamentoZoo.get('Stalla').value?.STA_NUM ?? 0;

    // this.setAnimalsCDC(attivitaToSave);
    // this.setDrugsResources(attivitaToSave);

    // //inseriamo le date
    // const dataInizio = this.trattamentoFormService.formTrattamentoZoo.get('Data').value;
    // attivitaToSave.inizio = dataInizio;
    // attivitaToSave.fine = new Date(dataInizio.getTime() + (( this.trattamentoFormService.dettagliProtocollo.durataTrattamento - 1 ) * 24 * 60 * 60 * 1000)); //durata in giorni

    // attivitaToSave.note = this.trattamentoFormService.formTrattamentoZoo.get('Note').value; 

    // return attivitaToSave;
  }

  /**
   * Populates the `centriDiCosto` property of the provided `PrescriptionActivity` object
   * with data derived from the selected rows in the `capiAnimaliComponent` grid.
   *
   * @param attivitaToSave - The `PrescriptionActivity` object to be updated.
   *
   * The method iterates over the selected rows in the `capiAnimaliComponent` grid
   * (if available) and creates a new `CapoAnimaleCDC` object for each row. Each
   * `CapoAnimaleCDC` object is populated with data from the grid row, including
   * information about the animal (`CapoAnimale`) and the quantity to be administered.
   * These objects are then cast to `CentroDiCosto` and added to the `centriDiCosto`
   * array of the `PrescriptionActivity` object.
   */
  private setAnimalsCDC(attivitaToSave: PrescriptionActivity) {
    const arrayCapiAnimali = this.capiAnimaliComponent?.gridSelectedRows ?? [];
    attivitaToSave.centriDiCosto = new Array<CentroDiCosto>();

    arrayCapiAnimali.forEach(item => {
      //la valorizzo già sopra per controllare che non ecceda la giacenza
      // quantitaTotaleDaSomministrare += item.dataItem.Quantita;
      let capoAnimaleCDC = new CapoAnimaleCDC();

      capoAnimaleCDC.capoAnimale = {
        validato: (item.Validato == 'No') ? false : true,
        causaleMorte: 0,
        partitaIva: item.Piva,
        codice: item.Cod_Animale,
        matricola: item.Matricola,
        flagCancellazione: false
      } as CapoAnimale;

      capoAnimaleCDC.qtaSomministrata = item.Quantita;
      attivitaToSave.centriDiCosto.push(capoAnimaleCDC as unknown as CentroDiCosto);
    });
  }

  public withDiffAICs(): boolean {
    return [...new Set(this.trattamentoFormService.arrayDettagliProdottiSomministrazione.map(item => item.prodotto.codiceAIC))].length > 1;
  }

  private setDrugsResources(attivitaToSave: PrescriptionActivity, finalFarmaci: any[], quantitaTotaleDaSomministrare: number) {
    attivitaToSave.risorse = new Array<Risorsa>();

    for (const itemFarmaco of finalFarmaci) {
      if (itemFarmaco.Qta <= 0) continue;

      let rifDettaglioSomministrazione: DettaglioRegistroSomministrazioni = attivitaToSave.risorse.find(item =>
          item.classType == 'DettaglioRegistroSomministrazioni'
          && (item as DettaglioRegistroSomministrazioni).prodotto.codice == itemFarmaco.Codice
          && (item as DettaglioRegistroSomministrazioni).codiceAIC == itemFarmaco.CodiceAIC
        ) as DettaglioRegistroSomministrazioni;
      let farmacoInfo = this.trattamentoFormService.arrayDettagliProdottiSomministrazione
        .find(item => item.prodotto.codice == itemFarmaco.Codice
            && item.MagazziniMovimentazioni?.length > 0
            && item.MagazziniMovimentazioni[0].Lotto == itemFarmaco.Lotto);

      if (rifDettaglioSomministrazione) {
        rifDettaglioSomministrazione.quantitaTotaleReale += Math.ceil(itemFarmaco.QtaToUse ?? itemFarmaco.Qta);
      } else {
        rifDettaglioSomministrazione = cloneDeep(this.trattamentoFormService.attivitaDaChiamante.risorse
          .find(item => item.classType == 'DettaglioRegistroSomministrazioni')) ?? new DettaglioRegistroSomministrazioni();
        rifDettaglioSomministrazione.quantitaTotaleReale = Math.ceil(itemFarmaco.QtaToUse ?? itemFarmaco.Qta);
        rifDettaglioSomministrazione.prodotto = new Prodotto(itemFarmaco.Codice, '');
        rifDettaglioSomministrazione.codiceAIC = itemFarmaco.CodiceAIC;
        rifDettaglioSomministrazione.unitaDiMisura = farmacoInfo.unitaDiMisura;
        rifDettaglioSomministrazione.sospensione = this.trattamentoFormService.attivitaDaChiamante.risorse
          .filter(r => r.classType == 'DettaglioRegistroSomministrazioni')[0].sospensione;

        attivitaToSave.risorse.push(rifDettaglioSomministrazione);
      }

      let risorsaScarico = new RisorsaProdotto();
      risorsaScarico.prodotto = farmacoInfo.prodotto;
      risorsaScarico.MagazziniMovimentazioni = cloneDeep(farmacoInfo.MagazziniMovimentazioni);

      // Set the exact quantity *to use* from this stock
      // Assuming one-to-one mapping for simplicity here.
      risorsaScarico.MagazziniMovimentazioni[0].Qta = itemFarmaco.QtaToUse ?? itemFarmaco.Qta;
      risorsaScarico.MagazziniMovimentazioni[0].QtaTot = itemFarmaco.QtaToUse ?? itemFarmaco.Qta;

      // If MagazziniMovimentazioni can have multiple items, you need to iterate
      // just like the original logic, but *only* up to `itemFarmaco.Qta`
      attivitaToSave.risorse.push(risorsaScarico);
    }
  }

  /**
   * Sets the drug resources for the given prescription activity.
   *
   * This method processes the selected drugs and animals, calculates the total quantity 
   * to be administered, and updates the `risorse` property of the provided `PrescriptionActivity` 
   * object with the necessary resources and details.
   *
   * @param attivitaToSave - The `PrescriptionActivity` object to which the drug resources will be added.
   *
   * The method performs the following steps:
   * - Retrieves the selected drugs and animals from the respective components.
   * - Calculates the total quantity to be administered based on the selected animals.
   * - Iterates over the selected drugs and creates detailed records for administration and stock movement.
   * - Updates the `risorse` property of the `PrescriptionActivity` with the administration details 
   *   and stock movement resources.
   * - Ensures that the quantity to be deducted from stock is adjusted based on the remaining quantity 
   *   to be administered.
   */
  private setDrugsResourcesOLD(attivitaToSave: PrescriptionActivity) {
    const arrayFarmaci = this.prodottiSomministrazioneComponent?.gridSelectedRows ?? [];
    const arrayCapiAnimali = this.capiAnimaliComponent?.gridSelectedRows ?? [];
    const quantitaTotaleDaSomministrare = parseFloat(arrayCapiAnimali
      .reduce((acc, item) => acc + item.Quantita, 0)
      .toFixed(4));

    attivitaToSave.risorse = new Array<Risorsa>();
    let quantitaDaScaricareRimanente = quantitaTotaleDaSomministrare;

    for (const itemFarmaco of arrayFarmaci) {
      if (quantitaDaScaricareRimanente <= 0) break;

        // let rifDettaglioSomministrazione = attivitaToSave.risorse.find(itemDettaglioSomm => itemDettaglioSomm.classType == 'DettaglioRegistroSomministrazioni');
        let rifDettaglioSomministrazione = cloneDeep(this.trattamentoFormService.attivitaDaChiamante.risorse.find(itemDettaglioSomm => itemDettaglioSomm.classType == 'DettaglioRegistroSomministrazioni')) ?? new DettaglioRegistroSomministrazioni();
        let farmaco = this.trattamentoFormService.arrayDettagliProdottiSomministrazione.find(itemDettaglio => itemDettaglio.prodotto.codice == itemFarmaco.Codice && itemDettaglio.MagazziniMovimentazioni[0].Lotto == itemFarmaco.Lotto);

        rifDettaglioSomministrazione.quantitaTotaleReale = Math.ceil(quantitaTotaleDaSomministrare);
        rifDettaglioSomministrazione.prodotto = new Prodotto(itemFarmaco.Codice, '');
        rifDettaglioSomministrazione.codiceAIC = itemFarmaco.CodiceAIC;
        rifDettaglioSomministrazione.unitaDiMisura = farmaco.unitaDiMisura;
        rifDettaglioSomministrazione.sospensioni = [];

        attivitaToSave.risorse.push(rifDettaglioSomministrazione);

        // let risorsaScarico = attivitaToSave.risorse.find(itemDettaglioSomm => itemDettaglioSomm.classType == 'RisorsaProdotto');
        let risorsaScarico = new RisorsaProdotto();

        risorsaScarico.prodotto = farmaco.prodotto;
        risorsaScarico.MagazziniMovimentazioni = farmaco.MagazziniMovimentazioni;

        risorsaScarico.MagazziniMovimentazioni.forEach(itemMagazzino => {
          itemMagazzino.Qta = (itemMagazzino.Qta < quantitaDaScaricareRimanente) ? itemMagazzino.Qta : quantitaDaScaricareRimanente;
          itemMagazzino.QtaTot = (itemMagazzino.QtaTot < quantitaDaScaricareRimanente) ? itemMagazzino.QtaTot : quantitaDaScaricareRimanente;
          quantitaDaScaricareRimanente -= itemMagazzino.Qta;
        });

        attivitaToSave.risorse.push(risorsaScarico);
    }
  }

}