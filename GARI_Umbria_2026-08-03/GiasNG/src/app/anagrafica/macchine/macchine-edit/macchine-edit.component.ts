import { Component, ElementRef, Inject, Injectable, OnDestroy, OnInit, Optional, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Caratteristica, ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { Carburante } from 'app/Model/metaschema/Carburante';
import { FinalitaMacchina } from 'app/Model/metaschema/FinalitaMacchina';
import { TipoTarga } from 'app/Model/metaschema/TipoTarga';
import { TitoloDiPossesso } from 'app/Model/metaschema/TitoloDiPossesso';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { enum_Security_Attivita, enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { firstValueFrom, from, lastValueFrom, map, Observable, of, Subject, Subscription, switchMap, take, takeUntil } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from '../../../Model/CostantiPersonalizzate';
import { Ditta, Tipo, Comune, Provincia } from '../../../Model/MetaschemaModel';
import { MasterService, rispostaStandard } from '../../../Service/master.service';
import { ObjParametriAgendaService } from '../../../Service/obj-parametri-agenda.service';
import { CostoUnitario } from 'app/Model/anagrafiche/CostoUnitario';
import { generateGridProviders } from 'gias-kendo-grid';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { DialogCloseResult, DialogResult } from '@progress/kendo-angular-dialog';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentriAziendaliService, LeggiCentriAziendali } from 'app/Service/Anagrafica/centri.service';
import { MacchineGridEventsService } from '../macchine-grid-events.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MACCHINE_SERVICE_TOKEN, MacchineFactoryService } from 'app/Service/ServiceFactory/macchine.factory.service';
import { MacchineServiceProvider } from 'app/Service/ServiceFactory/macchine.factory.provider';
import { CostiMacchinaEditConfigService } from './costi-macchina-edit/costi-macchina-edit-grid-config.service';
import { CostiMacchinaEditService } from './costi-macchina-edit/costi-macchina-edit.service';
import { checkDatesValidator, enumVisibilitaMacchina, latitudineValidator, longitudineValidator, MacchinaEditFormsService } from './macchina-edit-forms.service';
import { TranslocoService } from '@jsverse/transloco';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { MacchineEditImageStorageService } from './macchine-edit-image/macchine-edit-image-service/macchine-edit-image-storage.service';
import { CaratteristicheService } from './macchine-edit-caratteristiche/caratteristiche.service';
import { GerarchiaService } from './macchine-edit-gerarchia/gerarchia.service';
import { HubIoTPlatformDestination } from 'app/Model/metaschema/HubIoTPlatformDestination';
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { UtilityFunctions } from '../../../Utility/UtilityFunctions';
import { CodificheAgeaService } from '../../../Service/Metaschema/codifiche-agea.service';
import { CodificaMacchineAgeaRequest } from '../../../Model/metaschema/CodificheAgea';
import { BaseCodeDescrStr } from '../../../Model/baseClass/baseCodeDescrStr';
import { distinctUntilChanged, pairwise, startWith, tap } from 'rxjs/operators';
import { FunzioniComuniService } from "../../../Service/FunzioniComuni.service";
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { IrrigazioneService } from 'app/Service/Metaschema/irrigazione.service';
import { animate, style, transition, trigger } from '@angular/animations';
import { RateiTempoService } from './macchine-edit-turni-irrigua/service/turni-irrigua.service';
import { GiasIstatService } from 'app/Service/istat/gias-istat.service';
import { CodiciNazioniISO3166 } from 'app/Service/api.service';
import { Validators } from "@angular/forms";
import { BudgetService } from 'app/Service/Budget/budget.service';
import { ThemeService } from '@progress/kendo-angular-charts';

@Component({
  standalone: false,
  selector: 'app-macchine-edit',
  templateUrl: './macchine-edit.component.html',
  styleUrls: ['./macchine-edit.component.scss'],
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
    ...generateGridProviders(CostiMacchinaEditConfigService, MacchineEditComponent), MacchineServiceProvider, MacchineGridEventsService, CostiMacchinaEditService
  ]
})
export class MacchineEditComponent implements OnInit, OnDestroy {
  protected edit: boolean;

  protected Ditte: Array<Ditta>;
  protected Tipo: Array<Tipo>;
  protected Dettaglio1: Array<any>;
  protected Dettaglio2: Array<any>;
  protected CostoUnitario: Array<any>;

  protected macchinaForm: FormGroup;
  protected mostraform: boolean = false;

  private editSubscription: Subscription;
  private objParametriAgenda: ObjParametriAgenda;

  private lastTipo: any;
  private lastDet1: any;
  private lastDet2: any;

  private permessoAssegnazionePubblica: boolean;
  private permessoGestioneRateiIrrigui: boolean;

  private InstallationTypes: Array<string> = ['19', '20', '21']

  protected get showIrriguaTurniPanel(): boolean {
    return this.permessoGestioneRateiIrrigui && (this.macchinaForm.get('tipo').value.codice == '05');
  }

  protected get showInstallationDetailsPanel(): boolean {
    return this.InstallationTypes.includes(this.macchinaForm.get('tipo').value.codice);
  }

  protected get costi() {
    return this.macchinaForm.get('costi') as FormArray;
  }

  // array per ddl Tipo_Targa
  protected TipoTarga: Array<TipoTarga> = [
    { descrizione: 'Non Definito', codice: 0 },
    { descrizione: 'Senza Targa', codice: 1 },
    { descrizione: 'Stradale', codice: 2 },
    { descrizione: 'Rimorchio', codice: 3 },
    { descrizione: 'Triangolare', codice: 4 }
  ];

  // array per ddl Alimentazione
  protected TipoAlimentazione: Array<Carburante> = [
    { descrizione: 'Non Definita', codice: 0 },
    { descrizione: 'Benzina', codice: 1 },
    { descrizione: 'Gasolio', codice: 2 },
    { descrizione: 'Metano', codice: 3 },
    { descrizione: 'Gpl', codice: 4 },
    { descrizione: 'Elettricità', codice: 5 },
    { descrizione: 'Olio Combustibile', codice: 6 },
    { descrizione: 'Petrolio', codice: 7 }
  ];

  // array per ddl Unita_Misura
  protected TipoUnitaMisura: Array<UnitaDiMisura> = [
    { descrizione: '', codice: 0 },
    { descrizione: 'CV', codice: 5001028 },
    { descrizione: 'KW', codice: 5001027 }
  ];

  // array per ddl Titolo_Possesso
  protected TipoTitoloPossesso: Array<TitoloDiPossesso> = [
    { descrizione: 'Altro', codice: 0 },
    { descrizione: 'Proprietà', codice: 1 },
    { descrizione: 'Comodato d\'uso', codice: 2 },
    { descrizione: 'Affitto con contratto', codice: 3 },
    { descrizione: 'Affitto senza contratto', codice: 4 },
    { descrizione: 'In conto terzi', codice: 5 },
    { descrizione: 'In convenzione', codice: 6 },
    { descrizione: 'In compatercipazione', codice: 7 }
  ];

  // array per ddl Finalita
  protected TipoFinalita: Array<FinalitaMacchina> = [
    { descrizione: 'Agricola/Zootecnica', codice: 0 },
    { descrizione: 'Industriale', codice: 1 },
    { descrizione: 'Commerciale', codice: 2 }
  ];

  // array per ddl HubIoT
  protected HubIoT: Array<HubIoTPlatformDestination> = [
    { descrizione: this.transloco.translate('NonDefinito'), codice: 0 },
    { descrizione: 'JohnDeere', codice: 1 },
    { descrizione: 'Agrirouter', codice: 2 },
    { descrizione: 'AGCO_Trimble', codice: 3 },
    { descrizione: 'CNH1', codice: 4 }
  ];

  protected irrigationImplantTypes: Array<any> = [];

  protected data_inizio: Date = AGRODATAINIZIO;
  protected data_fine: Date = AGRODATAFINE;

  private datiMacchinaSub: Subscription;
  private dettaglio1Sub: Subscription;
  private dettaglio2Sub: Subscription;

  private valueChangesSub: Subscription;
  private signal: Subject<void> = new Subject();

  private centres: { codice: number | boolean; descrizione: string }[];
  private centresOnWhichIsUsed: number[] = [];
  private companiesByWhichIsUsed: string[] = [];

  //ddl geo stazioni
  protected StatiStazione: CodiciNazioniISO3166[] = [];
  protected ProvinceStazione: Provincia[] = [];
  protected ComuniStazione: Comune[] = [];

  constructor(
    @Optional() private masterService: MasterService,
    @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
    @Optional() private objParametriAgendaService: ObjParametriAgendaService,
    @Optional() private fb: FormBuilder,
    private router: Router,
    private giasMessageService: GiasMessageService,
    private permessiUtenteService: PermessiUtenteService,
    private giasDialogService: GiasDialogService,
    private centriService: CentriAziendaliService,
    private macchinaEditFormsService: MacchinaEditFormsService,
    private transloco: TranslocoService,
    private caratteristicheService: CaratteristicheService,
    private dialogService: GiasDialogService,
    private gerarchiaService: GerarchiaService,
    private route: ActivatedRoute,
    private contattiService: ContattiService,
    private codificheAgeaService: CodificheAgeaService,
    private rateiTempoService: RateiTempoService,
    private funzioniComuniService: FunzioniComuniService,
    private irrrigazioneService: IrrigazioneService,
    private budgetService: BudgetService,
    private istatService: GiasIstatService
  ) {
    this.macchinaForm = this.macchinaEditFormsService.getMacchinaForm();
    this.permessoAssegnazionePubblica = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Macchine_Assegnazione_Pubblica, 2);
    this.permessoGestioneRateiIrrigui = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.AnagraficaGestioneRateiIrriguiMacchine);

    this.edit = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write ||
      this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Update;

      this.irrrigazioneService.leggi(new Specie(-1))
        .pipe(take(1))
        .subscribe(r => this.irrigationImplantTypes = r);

      this.istatService.readCountries().pipe(
              take(1)
            ).subscribe(r => {
              this.StatiStazione = r;
            });
  }

  private manageGeoDdlStati(): void {

    this.macchinaForm.get('Stato_Installazione').valueChanges.pipe(
                                                                    startWith(this.macchinaForm.get('Stato_Installazione')!.value),
                                                                    takeUntil(this.signal)
                                                                  ).subscribe(stato => {

      if (stato == 'IT') {

        this.istatService.readProvinces(stato).pipe(
                                                                take(1)
        ).subscribe(r => {
          this.ProvinceStazione = r;
        });

        this.macchinaForm.get('Provincia_Istat_Installazione').setValidators([Validators.required]);

      } else {
        this.macchinaForm.get('Provincia_Istat_Installazione').patchValue(null);
        this.macchinaForm.get('Provincia_Istat_Installazione').clearValidators();
        this.ProvinceStazione = [];
      }

      this.macchinaForm.get('Provincia_Istat_Installazione').updateValueAndValidity();

    });

    this.macchinaForm.get('Provincia_Istat_Installazione').valueChanges.pipe(
                                                                              startWith(this.macchinaForm.get('Provincia_Istat_Installazione')!.value),
                                                                              takeUntil(this.signal)
                                                                            ).subscribe(provincia => {

      if (this.checkIfItaly() && provincia) {

        this.istatService.readCities(provincia ? provincia : '').pipe(
                                                                          take(1)
        ).subscribe(r => {
          this.ComuniStazione = r;
        });

        this.macchinaForm.get('Comune_Istat_Installazione').setValidators([Validators.required]);

      } else {
        this.macchinaForm.get('Comune_Istat_Installazione').patchValue(null);
        this.macchinaForm.get('Comune_Istat_Installazione').clearValidators();
        this.ComuniStazione = [];
      }

      this.macchinaForm.get('Comune_Istat_Installazione').updateValueAndValidity();
    });

    //di default, questo è su Italia
    // this.macchinaForm.get('Stato_Installazione').patchValue('IT')

  }

  onFormEnter(event: KeyboardEvent) {
     const target = event.target as HTMLElement;

    // Se l'evento è partito dentro il textarea → lascia passare normalmente
    if (target.closest('textarea')) {
      return; // niente preventDefault, Enter va a capo
    }

    // In tutti gli altri casi → blocca la submit implicita del form
    event.preventDefault();
  }

  private leggiMacchina(agenda: ObjParametriAgenda): Observable<rispostaStandard<ParcoMacchine>> {
    return this.macchineService.leggiMacchina(agenda);
  }

  async ngOnInit() {
    this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    agenda.Piva = '';
    let m: Observable<rispostaStandard<ParcoMacchine>> = null;
    if (<number>agenda.TipoOperazioneDB != <number>enum_TipoOperazioneDB.Scrittura) {
      if (agenda.Mac_Cod && agenda.Mac_Cod !== 0) {
        m = this.leggiMacchina(agenda);
      }
    } else {
      this.macchinaForm.patchValue(this.creaMacchinaVuota(), { emitEvent: false });
      this.mostraform = true;
    }

    const promiseDett1 = from(this.macchineService.leggi_CmbTipoMacchina(this.macchinaForm.controls['tipo'].value.codice, '', '', 2));
    this.dettaglio1Sub = promiseDett1.pipe(take(1)).subscribe(dett1 => {
      this.Dettaglio1 = <any>dett1;
    });

    if (m != null) {
      let dati: rispostaStandard<ParcoMacchine> = await firstValueFrom(m);
      let macchina: ParcoMacchine = (dati.RispostaStringa as ParcoMacchine);

      const promiseDett1 = from(this.macchineService.leggi_CmbTipoMacchina(macchina.tipo.codice, '', '', 2));
      this.dettaglio1Sub = promiseDett1.pipe(take(1)).subscribe(dett1 => {
        this.Dettaglio1 = <any>dett1;
      });

      const promiseDett2 = from(this.macchineService.leggi_CmbTipoMacchina(macchina.tipo.codice, macchina.dettaglio_1.codice, '', 3));
      this.dettaglio2Sub = promiseDett2.pipe(take(1)).subscribe(dett2 => {
        this.Dettaglio2 = <any>dett2;
      });

      this.updateFormMacchina(macchina);
      this.mostraform = true;

      this.handleMacchinaMovimentata();
      this.handleContattoValue();

      this.lastTipo = this.macchinaForm.controls['tipo'].getRawValue();
      this.lastDet1 = this.macchinaForm.controls['dettaglio_1'].getRawValue();
      this.lastDet2 = this.macchinaForm.controls['dettaglio_2'].getRawValue();

    }

    if (this.objParametriAgenda.Piva != undefined && this.objParametriAgenda.Piva !== '') {

      const PromisesResp = await Promise.all([
        this.macchineService.leggi_Ditte(),
        this.macchineService.leggi_CmbTipoMacchina('', '', '', 1)
      ]);

      this.Ditte = PromisesResp[0];
      this.Tipo = PromisesResp[1];

      if (<number>agenda.TipoOperazioneDB == <number>enum_TipoOperazioneDB.Scrittura) {
        this.macchinaForm.controls['tipo'].setValue(this.Tipo[0]);
      }

      this.populateCentresList();
    }

    this.checkPresenzaVIN().subscribe();

    // se sono in sola lettura disbilito l'intero form
    if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read) {
      this.macchinaForm.disable();
    }

    this.masterService.set_isLoading({ isLoading: false, message: '' });

    if (this.showInstallationDetailsPanel)
      this.setValidatorsStazione();

    this.manageGeoDdlStati();
    this.onContattoChange();
  }

  private populateCentresList(): void {
    let impresa = new Impresa();
    impresa.partitaIva = this.objParametriAgenda.Piva;

    this.centriService.leggiCentriAziendaliModelloQdC(
      <LeggiCentriAziendali>{
        impresa: impresa,
        data: (<FormGroup>this.macchinaForm.controls["validita"]).value.inizio
      },
      false
    ).then(vals => {
      this.centres = [{ descrizione: 'Macchina Privata', codice: enumVisibilitaMacchina.PRIVATA }];
      vals.forEach(el => this.centres.push({ codice: el.primaryKey.codice, descrizione: el.nome }));

      if (this.permessoAssegnazionePubblica) {
        this.centres.push({ descrizione: 'Macchina Pubblica', codice: enumVisibilitaMacchina.PUBBLICA });
      }
    });
  }

  private fetchCentresByWhichIsUsed(macchina: ParcoMacchine): void {
    this.macchineService.centresOnWhichIsUsed(macchina).pipe(take(1)).subscribe(r => {
      this.centresOnWhichIsUsed = r.RispostaStringa;
    });
  }

  private fetchCompaniesByWhichIsUsed(macchina: ParcoMacchine): void {
    this.macchineService.companiesByWichIsUsed(macchina).pipe(take(1)).subscribe(r => {
      this.companiesByWhichIsUsed = r.RispostaStringa;
    });
  }

  private handleMacchinaMovimentata(): void {
    this.macchineService.isMacchinaMovimentata(this.macchinaForm.getRawValue()).pipe(
      take(1),
      tap(r => {
        if (r.RispostaStringa) {
          this.macchinaForm.controls['tipo'].disable();
          this.macchinaForm.controls['dettaglio_1'].disable();
          this.macchinaForm.controls['dettaglio_2'].disable();

          this.macchinaForm.controls['contatto'].disable();
          // this.macchinaForm.controls['centroPK'].disable();

          this.fetchCentresByWhichIsUsed(this.macchinaForm.getRawValue());
          this.fetchCompaniesByWhichIsUsed(this.macchinaForm.getRawValue());
        }
      })
    ).subscribe();
  }

  private handleContattoValue(): void {
    if (this.macchinaForm.controls['contatto']?.value != undefined &&
      this.macchinaForm.controls['contatto'].value?.primaryKey?.codice != '') {

      let contatto = this.macchinaForm.controls['contatto'].value;
      (<FormGroup>this.macchinaForm.controls['centroPK']).controls['codice'].disable();
      let ragioneSociale: string;

      if (contatto.ragione_Sociale != undefined && contatto.ragione_Sociale != "") {
        ragioneSociale = contatto.ragione_Sociale;
      } else {
        ragioneSociale = contatto.nome + " " + contatto.cognome;
      }

      this.macchinaForm.controls['contatto'].patchValue({
        primaryKey: contatto.primaryKey.codice,
        ragione_Sociale: ragioneSociale
      });

    } else {
      let contattoVuoto: Contatto = new Contatto();
      this.macchinaForm.controls['contatto'].patchValue(contattoVuoto);
      if (this.InstallationTypes.includes(this.macchinaForm.controls['tipo']?.value?.codice ?? 0)) // se siamo nel caso di installazioni, la macchina è sempre privata
        (<FormGroup>this.macchinaForm.controls['centroPK']).controls['codice'].disable();
    }
  }

  private onContattoChange(): void {
      this.macchinaForm.controls['contatto'].valueChanges.pipe(map(contatto => {
        //DCA20251215: se non siamo nel caso di installazioni, per cui la macchina è sempre privata
        //evito di modificare la visibilità se il contatto cambia
        if (!this.showInstallationDetailsPanel) {
          const visibilitaForm = (<FormGroup>this.macchinaForm.controls['centroPK']).controls['codice'];
          if (contatto.primaryKey.codice != "0" && contatto.primaryKey.codice != "") {
            visibilitaForm.patchValue(contatto.sa_cod);
            visibilitaForm.disable();
          } else {
            visibilitaForm.enable();
          }
        }
      })).subscribe();
  }

  private updateFormMacchina(macchina: ParcoMacchine): void {
    this.caratteristicheService.setCaratteristiche(macchina.caratteristiche);
    this.macchinaForm = this.macchinaEditFormsService.getMacchinaForm();
    this.macchinaForm.patchValue(macchina, { emitEvent: false });

    if (macchina.costi as CostoUnitario[]) {
      const faCosti = <FormArray>this.macchinaForm.controls['costi'];
      macchina.costi.forEach((costo, i) => {
        const costoForm = this.macchinaEditFormsService.getCostoUnitarioChiaveForm();
        costoForm.patchValue(costo, { emitEvent: false });
        faCosti.push(costoForm);
      });
    }

    const faCaratteristiche = <FormArray>this.macchinaForm.controls['caratteristiche'];
    this.caratteristicheService.getCaratteristiche().forEach((caratteristica, i) => {
      const caratteristicaForm = this.macchinaEditFormsService.getCaratteristicaForm();
      caratteristicaForm.patchValue(caratteristica, { emitEvent: false });
      faCaratteristiche.push(caratteristicaForm);
    });

    const faGerarchia = <FormArray>this.macchinaForm.controls['gerarchiaFigli'];
    let maxcod: number = 0;
    macchina.gerarchiaFigli.forEach((macchina, i) => {
      const macchinaGerarchiaForm = this.macchinaEditFormsService.getMacchinaGerarchiaForm();
      macchina.codice = maxcod;
      macchinaGerarchiaForm.patchValue(macchina, { emitEvent: false });
      faGerarchia.push(macchinaGerarchiaForm);
      maxcod++;
    });

    const faRateiTempo = <FormArray>this.macchinaForm.controls['rateiTempo'];
    macchina.rateiTempo.forEach((rateo, i) => {
      const rateoTempoForm = this.macchinaEditFormsService.getRateoTempoForm();
      rateoTempoForm.patchValue(rateo, { emitEvent: false });
      faRateiTempo.push(rateoTempoForm);
    });

    if (this.macchinaForm.controls['VIN'].value == '' ||
      this.macchinaForm.controls['VIN'].value == null) {
      this.macchinaForm.controls['HubIoT_PlatformDestination'].patchValue(this.HubIoT[0]);
      this.macchinaForm.controls['HubIoT_PlatformDestination'].disable();
    } else {
      this.macchinaForm.controls['HubIoT_PlatformDestination'].patchValue({ codice: macchina.HubIoT_PlatformDestination.codice, descrizione: this.HubIoT[macchina.HubIoT_PlatformDestination.descrizione] });
    }

    this.macchinaForm.controls['portata'].patchValue(macchina.portata ?? 0);
    this.macchinaForm.controls['efficienza'].patchValue(macchina.efficienza ?? 0);
    this.macchinaForm.controls['codice_impianto'].patchValue(+macchina.codice_impianto);

    this.macchinaForm.updateValueAndValidity();
  }

  private isEditAllowed(actionType: Enum_DBTypeOperation) {
    if ((actionType == Enum_DBTypeOperation.Update && this.isVisibilitaOrTypeChanged())) {
      return this.macchineService.isEditAllowed(this.macchinaForm.getRawValue());
    } else {
      let resp = new rispostaStandard<boolean>();
      resp.RispostaOK = true;
      resp.RispostaStringa = true;
      return from([resp]);
    }
  }

  protected salvaNuovoMacchina(): void {
    this.save().subscribe(r => {
      if (r.RispostaOK) {
        this.giasMessageService.successMessage('MacchinaSalvataCorrettamente', false, true);
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

        let currentUrl = this.router.url;
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigate([currentUrl]);
      }
    });
  }

  protected salvaMacchina(): void {
    this.save().subscribe(r => {
      if (r?.RispostaOK) {
        this.giasMessageService.successMessage('MacchinaSalvataCorrettamente', false, true);

        //Chiudo la window se la pagina è stata aperta in questo modo
        if (this.route.snapshot.queryParamMap.get('seFrame') && + this.route.snapshot.queryParamMap.get('seFrame') === 1) {
          window.parent.postMessage('chiudiWindowGiasNG', this.funzioniComuniService.getOrigins());
        } else {
          if (!this.budgetService.getBudget().activeBudget)
            this.router.navigate(['/Anagrafica/Macchine']);
          else
            this.router.navigate(['Budget/Anagrafica/Macchine']);
        }
      }
      else {
        (r as any)?.pipe(map(t => {
          this.giasMessageService.errorMessage(JSON.parse((t as any).message).ErroriGias[0].messaggio);
        })).subscribe(); // FIX TEMPORANEO; DA MIGLIORARE
      }
    });
  }

  private sovrapposizioneData(inizioA: Date, fineA: Date, inizioB: Date, fineB: Date): boolean {
    if (inizioA >= inizioB && inizioA <= fineB) return true;
    if (fineA >= inizioB && fineA <= fineB) return true;
    if (inizioA <= inizioB && fineA >= fineB) return true;
    return false;
  }

  private checkSovrapposizione(): Caratteristica[] {
    let sovrapposizioni: Caratteristica[] = [];
    this.caratteristicheService.getCaratteristiche().forEach(caratteristica => {
      let confronti: Caratteristica[] = this.caratteristicheService.getCaratteristiche().map(x => Object.assign({}, x));
      confronti.splice(this.caratteristicheService.getCaratteristiche().indexOf(caratteristica), 1);
      confronti.forEach(confronto => {
        if (caratteristica.caratteristica.codice == confronto.caratteristica.codice && this.sovrapposizioneData(caratteristica.validita_inizio,
          caratteristica.validita_fine,
          confronto.validita_inizio,
          confronto.validita_fine)
        ) {
          sovrapposizioni.push(caratteristica);
        }
      })
    });
    return sovrapposizioni;
  }

  private save() {
    let tipoOp: enum_TipoOperazioneDB;

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      tipoOp = enum_TipoOperazioneDB.Scrittura;
      this.setPrimaryKey();
    } else {
      tipoOp = enum_TipoOperazioneDB.Modifica;
    }

    if (this.macchinaForm.controls['validita'].value.inizio == null) {
      this.macchinaForm.controls['validita'].patchValue({ inizio: AGRODATAINIZIO });
    }

    if (this.macchinaForm.controls['validita'].value.fine == null) {
      this.macchinaForm.controls['validita'].patchValue({ fine: AGRODATAFINE });
    }

    if (this.macchinaForm.controls['data_Ultima_Revisione'].value == null) {
      this.macchinaForm.controls['data_Ultima_Revisione'].patchValue(AGRODATAINIZIO);
    }

    if (this.macchinaForm.controls['data_Ultima_Manutenzione'].value == null) {
      this.macchinaForm.controls['data_Ultima_Manutenzione'].patchValue(AGRODATAINIZIO);
    }

    if (this.macchinaForm.controls['data_Rilascio_Autorizzazione'].value == null) {
      this.macchinaForm.controls['data_Rilascio_Autorizzazione'].patchValue(AGRODATAINIZIO);
    }

    this.setDettagli();
    this.setVisibilitaPubblica();

    let sovrapposizioni = this.checkSovrapposizione();

    if (sovrapposizioni.length > 0) {
      let msgWarning: string = '';
      sovrapposizioni.forEach(car => {
        msgWarning = msgWarning.concat(car.caratteristica.descrizione + " con valore " + car.valore + ", ");
      });
      this.giasMessageService.warningMessage("Le caratteristiche " + msgWarning + " hanno una sovrapposizione nelle loro durate di validità.")
      return of();
    }

    const faCaratteristiche = <FormArray>this.macchinaForm.controls['caratteristiche'];
    faCaratteristiche.clear();
    this.caratteristicheService.getCaratteristiche().forEach((caratteristica, i) => {
      const caratteristicaForm = this.macchinaEditFormsService.getCaratteristicaForm();
      caratteristicaForm.patchValue(caratteristica, { emitEvent: false });
      faCaratteristiche.push(caratteristicaForm);
    });

    const faGerarchia = <FormArray>this.macchinaForm.controls['gerarchiaFigli'];
    faGerarchia.clear();
    this.gerarchiaService.getGerarchia().forEach((macchina, i) => {
      const macchinaGerarchiaForm = this.macchinaEditFormsService.getMacchinaGerarchiaForm();
      macchinaGerarchiaForm.patchValue(macchina, { emitEvent: false });
      faGerarchia.push(macchinaGerarchiaForm);
    });

    const faRateiTempo = <FormArray>this.macchinaForm.controls['rateiTempo'];
    faRateiTempo.clear();
    this.rateiTempoService.getRateiTempo().forEach((rateo, i) => {
      const rateoTempoForm = this.macchinaEditFormsService.getRateoTempoForm();
      rateoTempoForm.patchValue(rateo, { emitEvent: false });
      faRateiTempo.push(rateoTempoForm);
    });

    if (this.macchinaForm.controls['VIN'].value === '') {
      this.macchinaForm.controls['HubIoT_PlatformDestination'].patchValue({ codice: 0, descrizione: '' });
    } else {
      this.macchinaForm.controls['HubIoT_PlatformDestination'].patchValue({
        codice: (this.macchinaForm.controls['HubIoT_PlatformDestination']?.value?.codice ?? 0),
        descrizione: this.macchinaForm.controls['HubIoT_PlatformDestination']?.value?.descrizione ?? ''
      });
    }

    this.caratteristicheService.setCaratteristiche([]);

    let contatto_cod = this.macchinaForm.controls['contatto'].value.primaryKey;
    if (typeof contatto_cod == 'object') {
      contatto_cod = contatto_cod.codice;
    }
    this.macchinaForm.controls['contatto'].value.primaryKey = { partitaIva: this.objParametriAgenda.Piva, codice: contatto_cod };

    return this.isEditAllowed(this.objParametriAgenda.TipoOperazioneDB).pipe(
      takeUntil(this.signal)).pipe(
        switchMap((res1) => {
          if (!res1.RispostaOK) {
            throw new Error(res1.Errore);
          } else if (!res1.RispostaStringa) {
            return this.giasDialogService.dialogMessageObs_Result(
              this.transloco.translate('MacchinaAttrezzatura'),
              this.transloco.translate(res1.Errore),
              [{ text: this.transloco.translate('Annulla'), returnObj: { returnObj: false } }]
            ).pipe(
              map((dialogRes) => (<any>dialogRes).returnObj)
            );
          } else {
            return of([null]);
          }
        }),
        switchMap((res2) => {
          if (!res2['returnObj'] && res2['returnObj'] != undefined && res2[0] == null) {
            return of(null);
          } else if (res2['returnObj'] || res2[0] == null) {

            const confirm$ = this.showInstallationDetailsPanel && (contatto_cod == "")  ? this.confirmSaveStazioneDialog() : of({ returnObj: true });

            return confirm$.pipe(
              switchMap((confirmRes) => {
                if (!confirmRes['returnObj'])
                  return of(null);
                this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
                return from(this.macchineService.aggiornaMacchina(this.macchinaForm.getRawValue(), tipoOp));
              })
            );

          } else {
            return of(null);
          }
        }),
        switchMap((res3) => {
          if (res3 == undefined) return of(res3);
          else {
            this.masterService.set_isLoading({ isLoading: false, message: '' });

            if (res3.RispostaOK) {
              //this.macchinaEditCostsService.resetCostsForm();
            }
            return of(res3);
          }
        })
      );
  }

  private confirmSaveStazioneDialog() {
    return this.giasDialogService.dialogMessageObs_Result(
      this.transloco.translate('MacchinaAttrezzatura'),
      this.transloco.translate('macchina.TipoInstallazioneSenzaContattoConferma', [this.macchinaForm.controls['tipo']?.value?.descrizione ?? '']),
      [
        { text: this.transloco.translate('Annulla'), returnObj: { returnObj: false } },
        { text: this.transloco.translate('Conferma'), returnObj: { returnObj: true } }
      ]
    ).pipe(
      map((dialogRes) => (<any>dialogRes).returnObj)
    );
  }

  private setVisibilitaPubblica(): void {
    this.macchinaForm.controls['visibilitaPubblica'].setValue(this.macchinaForm.value.centroPK?.codice == -1);
  }

  private setPrimaryKey(): void {
    this.macchinaForm.value.partitaIva = this.objParametriAgenda.Piva;
    this.macchinaForm.value.primaryKey = { codice: 0, impresaPK: { codice: 0, partitaIva: this.objParametriAgenda.Piva } };
  }

  private setDettagli(): void {
    if (this.macchinaForm.value.dettaglio_1 == '') {
      this.macchinaForm.controls['dettaglio_1'].setValue({ codice: '', descrizione: '' });
    }
    if (this.macchinaForm.value.dettaglio_2 == '') {
      this.macchinaForm.controls['dettaglio_2'].setValue({ codice: '', descrizione: '' });
    }
  }

  async Dettaglio1Change(val: any) {
    let saveDettaglio2 = this.macchinaForm.get('dettaglio_2').value;
    this.macchinaForm.get('dettaglio_2').setValue('');
    let disponibili = await lastValueFrom(this.caratteristicheService.getCaratteristicheDisponibili(this.getClassCode()))
    if (this.caratteristicheService.checkPerditaCaratteristiche(this.caratteristicheService.getCaratteristiche(), disponibili)) {
      let resp = await lastValueFrom(this.cambioTipologiaDialog());
      if ((<any>resp).returnObj) {
        this.lastDet1 = val.data;
        this.Dettaglio2 = await this.macchineService.leggi_CmbTipoMacchina(this.macchinaForm.get('tipo').value.codice, this.macchinaForm.get('dettaglio_1').value.codice, '', 3);
        this.rimuoviCaratteristiche();
      } else {
        this.macchinaForm.get('dettaglio_2').setValue(saveDettaglio2);
        this.macchinaForm.controls['dettaglio_1'].patchValue(this.lastDet1);
      }
    } else {
      this.lastDet1 = val.data;
      this.lastDet2 = this.macchinaForm.get('dettaglio_2').value
      this.Dettaglio2 = await this.macchineService.leggi_CmbTipoMacchina(this.macchinaForm.get('tipo').value.codice, this.macchinaForm.get('dettaglio_1').value.codice, '', 3);
    }
  }

  async Dettaglio2Change(val: any) {
    let disponibili = await lastValueFrom(this.caratteristicheService.getCaratteristicheDisponibili(this.getClassCode()))
    if (this.caratteristicheService.checkPerditaCaratteristiche(this.caratteristicheService.getCaratteristiche(), disponibili)) {
      let resp = await lastValueFrom(this.cambioTipologiaDialog());
      if ((<any>resp).returnObj) {
        this.lastDet2 = val.data;
        this.rimuoviCaratteristiche();
      } else {
        this.macchinaForm.controls['dettaglio_2'].patchValue(this.lastDet2);
      }
    }
  }

  async TipoChange(val: any) {
    let saveDettaglio1 = this.macchinaForm.get('dettaglio_1').value;
    let saveDettaglio2 = this.macchinaForm.get('dettaglio_2').value;
    this.macchinaForm.get('dettaglio_1').setValue('');
    this.macchinaForm.get('dettaglio_2').setValue('');
    let disponibili = await lastValueFrom(this.caratteristicheService.getCaratteristicheDisponibili(this.getClassCode()))
    if (this.caratteristicheService.checkPerditaCaratteristiche(this.caratteristicheService.getCaratteristiche(), disponibili)) {
      let resp = await lastValueFrom(this.cambioTipologiaDialog());
      if ((<any>resp).returnObj) {
        this.lastTipo = val.data;
        this.lastDet1 = this.macchinaForm.get('dettaglio_1').value;
        this.lastDet2 = this.macchinaForm.get('dettaglio_2').value
        this.Dettaglio2 = [];
        this.Dettaglio1 = await this.macchineService.leggi_CmbTipoMacchina(this.macchinaForm.get('tipo').value.codice, '', '', 2);
        this.rimuoviCaratteristiche();
      } else {
        this.macchinaForm.get('dettaglio_1').setValue(saveDettaglio1);
        this.macchinaForm.get('dettaglio_2').setValue(saveDettaglio2);
        this.macchinaForm.controls['tipo'].patchValue(this.lastTipo);
      }
    } else {
      this.lastTipo = val.data;
      this.lastDet1 = this.macchinaForm.get('dettaglio_1').value
      this.lastDet2 = this.macchinaForm.get('dettaglio_2').value
      this.Dettaglio2 = [];
      this.Dettaglio1 = await this.macchineService.leggi_CmbTipoMacchina(this.macchinaForm.get('tipo').value.codice, '', '', 2);
    }

    //gestiamo la visibilità della sezione Irrigua Turni
    //this.showIrriguaTurniPanel = (this.macchinaForm.get('tipo').value.codice == '05'); // codice per Macchina Irrigazione
    if (!this.showIrriguaTurniPanel) {
      // se non è una macchina irrigazione, resetto i turni
      this.clearArrayRateiTempo();
    }

    // se viene selezionata una tipologia di macchina "Stazione", mostro il pannello con i dettagli della stazione,
    // blocco la macchina a privata e carico i contatti che siano solo fornitori

    const visibilitaForm = (<FormGroup>this.macchinaForm.controls['centroPK']).controls['codice'];

    if (this.showInstallationDetailsPanel) {
      this.macchinaForm.get('Stato_Installazione').patchValue('IT');
      this.setValidatorsStazione();

      //resetto anche il contatto, potrebbe non essere un fornitore quello scelto precedentemente
      let contattoVuoto: Contatto = new Contatto();
      this.macchinaForm.controls['contatto'].patchValue(contattoVuoto);

      //blocco la macchina a privata
      visibilitaForm.patchValue(enumVisibilitaMacchina.PRIVATA);
      visibilitaForm.disable();

    } else {
      visibilitaForm.enable();
      // se non è una stazione, resetto la parte riguardante la stazione
      this.clearStazionePanel();
    }
  }

  private setValidatorsStazione() {

    this.macchinaForm.get('Contratto_Installazione').setValidators([Validators.required, Validators.pattern(/\S+/)]);
    this.macchinaForm.get('Contratto_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Tipologia_Installazione').setValidators([Validators.required, Validators.pattern(/\S+/)]);
    this.macchinaForm.get('Tipologia_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Data_Inizio_Installazione').setValidators([Validators.required, checkDatesValidator]);
    this.macchinaForm.get('Data_Inizio_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Data_Fine_Installazione').setValidators([Validators.required, checkDatesValidator]);
    this.macchinaForm.get('Data_Fine_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Stato_Installazione').setValidators([Validators.required]);
    this.macchinaForm.get('Stato_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Indirizzo_Installazione').setValidators([Validators.required, Validators.pattern(/\S+/)]);
    this.macchinaForm.get('Indirizzo_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Latitudine_Installazione').setValidators([Validators.required, latitudineValidator]);
    this.macchinaForm.get('Latitudine_Installazione').updateValueAndValidity();

    this.macchinaForm.get('Longitudine_Installazione').setValidators([Validators.required, longitudineValidator]);
    this.macchinaForm.get('Longitudine_Installazione').updateValueAndValidity();

    //aggiungiamo come obbligatorio l'inserimento del Codice_Stringa nel caso di selezione tipologia 'Stazione'
    this.macchinaForm.get('codice_stringa').setValidators([Validators.required, Validators.pattern(/\S+/)]);
    this.macchinaForm.get('codice_stringa').updateValueAndValidity();
  }

  private clearStazionePanel() {
      this.macchinaForm.patchValue({
        Distinta_Installazione: "",
        Contratto_Installazione: "",
        Tipologia_Installazione: "",
        Data_Inizio_Installazione: new Date(),
        Data_Fine_Installazione: new Date(),
        Stato_Installazione: null,
        Provincia_Istat_Installazione: null,
        Comune_Istat_Installazione: null,
        Indirizzo_Installazione: "",
        Latitudine_Installazione: 0,
        Longitudine_Installazione: 0
      });

      this.macchinaForm.get('Contratto_Installazione').clearValidators();
      this.macchinaForm.get('Contratto_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Tipologia_Installazione').clearValidators();
      this.macchinaForm.get('Tipologia_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Data_Inizio_Installazione').clearValidators();
      this.macchinaForm.get('Data_Inizio_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Data_Fine_Installazione').clearValidators();
      this.macchinaForm.get('Data_Fine_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Stato_Installazione').clearValidators();
      this.macchinaForm.get('Stato_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Indirizzo_Installazione').clearValidators();
      this.macchinaForm.get('Indirizzo_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Latitudine_Installazione').clearValidators();
      this.macchinaForm.get('Latitudine_Installazione').updateValueAndValidity();

      this.macchinaForm.get('Longitudine_Installazione').clearValidators();
      this.macchinaForm.get('Longitudine_Installazione').updateValueAndValidity();

      //aggiungiamo come obbligatorio l'inserimento del Codice_Stringa nel caso di selezione tipologia 'Stazione'
      this.macchinaForm.get('codice_stringa').clearValidators();
      this.macchinaForm.get('codice_stringa').updateValueAndValidity();
  }

  private cambioTipologiaDialog(): Observable<DialogResult> {
    return this.dialogService.dialogMessageObs_Result(this.transloco.translate('MacchinaAttrezzatura'),
      this.transloco.translate('PerditaCaratteristiche'),
      [
        { text: this.transloco.translate('Ok'), primary: true, returnObj: true },
        { text: this.transloco.translate('Annulla'), returnObj: false }
      ],
      undefined,
      undefined,
      e => e instanceof DialogCloseResult)
  }

  ngOnDestroy() {
    if (this.datiMacchinaSub) {
      this.datiMacchinaSub.unsubscribe();
    }
    if (this.dettaglio1Sub) {
      this.dettaglio1Sub.unsubscribe();
    }
    if (this.dettaglio2Sub) {
      this.dettaglio2Sub.unsubscribe();
    }
    if (this.editSubscription) {
      this.editSubscription?.unsubscribe();
    }
    if (this.valueChangesSub) {
      this.valueChangesSub.unsubscribe();
    }

    this.clearArrayRateiTempo();

    this.signal.next();
    this.signal.complete();
  }

  private clearArrayRateiTempo(): void {
    this.rateiTempoService.setRateiTempo([]);
  }

  protected getVisibilityList() {
    return this.centres;
  }

  protected disableVisibilityItems(itemArgs: { dataItem: { codice: number; descrizione: string; }; }): boolean {
    if (itemArgs.dataItem.codice === -1) {
      return false;
    } else if (itemArgs.dataItem.codice === 0) {
      // o la macchina non è stata utilizzata, o è stata utilizzata solo dall'azienda proprietaria
      return !(
        this.companiesByWhichIsUsed.length === 0 ||
        (this.companiesByWhichIsUsed.length === 1 && this.companiesByWhichIsUsed.includes(this.objParametriAgenda.Piva))
      );
    } else {
      // o la macchina non è stata utilizzata, o è stata utilizzato solo all'interno dello stesso centro
      return !(
        this.centresOnWhichIsUsed.length === 0 ||
        (this.centresOnWhichIsUsed.length === 1 && this.centresOnWhichIsUsed.includes(itemArgs.dataItem.codice))
      );
    }
  }

  private getClassCode(): string {
    return this.macchinaForm.get('tipo').value.codice +
      (this.macchinaForm.get('dettaglio_1').value ? (this.macchinaForm.get('dettaglio_1').value.codice != "" ? "." + this.macchinaForm.get('dettaglio_1').value.codice : "") : "") +
      (this.macchinaForm.get('dettaglio_2').value ? (this.macchinaForm.get('dettaglio_2').value.codice != "" ? "." + this.macchinaForm.get('dettaglio_2').value.codice : "") : "")
  }

  private rimuoviCaratteristiche(): void {

    let carCorrenti = this.caratteristicheService.getCaratteristiche()
    let class_code = this.macchinaForm.get('tipo').value.codice +
      (this.macchinaForm.get('dettaglio_1').value ? "." + this.macchinaForm.get('dettaglio_1').value.codice : "") +
      (this.macchinaForm.get('dettaglio_2').value ? "." + this.macchinaForm.get('dettaglio_2').value.codice : "")
    this.caratteristicheService.getCaratteristicheDisponibili(class_code).pipe(take(1)).subscribe(t => {
      let carNuove: Caratteristica[] = carCorrenti.map(x => Object.assign({}, x));
      carNuove.forEach(caratt => {
        let rimuovi = false;
        if (t.length == 0) {
          rimuovi = true;
        } else {
          let listaInt = t.map(t => t.codice);
          if (listaInt.indexOf(caratt.caratteristica.codice) < 0) {
            rimuovi = true;
          }
        }
        if (rimuovi) {
          carCorrenti.splice(carCorrenti.indexOf(caratt), 1)
        }
      })
      const faCaratteristiche = <FormArray>this.macchinaForm.controls['caratteristiche'];
      if (carCorrenti.length == 0) {
        faCaratteristiche.clear();
        this.caratteristicheService.setCaratteristiche(carCorrenti)
      } else {
        faCaratteristiche.clear();
        carCorrenti.forEach((caratteristica, i) => {
          const caratteristicaForm = this.macchinaEditFormsService.getCaratteristicaForm();
          caratteristicaForm.patchValue(caratteristica, { emitEvent: false });
          faCaratteristiche.push(caratteristicaForm);
          this.caratteristicheService.setCaratteristiche(carCorrenti)
        });
      }
    });
  }

  private checkPresenzaVIN(): Observable<any> {
    return this.macchinaForm.controls['VIN'].valueChanges.pipe(map(val => {
      if (val == "") {
        this.macchinaForm.controls['HubIoT_PlatformDestination'].patchValue(this.HubIoT[0]);
        this.macchinaForm.controls['HubIoT_PlatformDestination'].disable();
      } else {
        this.macchinaForm.controls['HubIoT_PlatformDestination'].enable();
      }
    }));
  }

  async openContattoDdl(ddlEl: GiasDropDownTemplateSComponent) {
    ddlEl.loading = true;

    let contattiObs: Observable<any[]> = (this.showInstallationDetailsPanel) ?
                                            this.contattiService.leggiContattiStazioniMeteo(this.objParametriAgenda.Piva)
                                            : this.contattiService.leggiContattiMacchina({
                                                                                          objNG: this.objParametriAgenda,
                                                                                          Flag_Pubblico_Privato: this.permessoAssegnazionePubblica,
                                                                                          Flag_Visibilita_Centri: true,
                                                                                          Cod_Contatto: ""
                                                                                        });

    contattiObs.pipe(take(1), map(r => {
      let contattoVuoto = new Contatto();
      contattoVuoto.primaryKey = { partitaIva: '', codice: '' };
      ddlEl.listItems = [contattoVuoto];
      r.forEach(contatto => {
        ddlEl.listItems.push(contatto);
      });
      ddlEl.loading = false;
    })).subscribe();
  }

  private isVisibilitaOrTypeChanged(): boolean {
    return this.macchinaForm.controls['centroPK'].dirty ||
      this.macchinaForm.controls['tipo'].dirty ||
      this.macchinaForm.controls['dettaglio_1'].dirty ||
      this.macchinaForm.controls['dettaglio_2'].dirty ||
      this.macchinaForm.controls['validita'].dirty;
  }

  private creaMacchinaVuota(): ParcoMacchine {
    let m: ParcoMacchine = new ParcoMacchine();
    let agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    m.partitaIva = agenda.Piva;
    return m;
  }

  protected async openDdl(ddlEl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'ageaCod':
        let params: CodificaMacchineAgeaRequest = {
          ageaCod: '',
          ageaDes: '',
          classCod: '',
        };
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.codificheAgeaService.readAgeaMachineCodes(params).pipe(
            take(1),
            map(r => [new BaseCodeDescrStr('', ''), ...r.RispostaStringa])
          ))
        );
        break;
    }
  }

  protected checkIfItaly(): boolean {
    return this.macchinaForm.get('Stato_Installazione').value/*?.codice*/ === 'IT';
  }
}

@Injectable()
export class UploadInterceptor implements HttpInterceptor {

  constructor(private macchineEditImageStorageService: MacchineEditImageStorageService) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (req.url === "saveLarge" || req.url === "saveSmall") {
      if (req.url === "saveLarge") {
        this.macchineEditImageStorageService.depositaImmagine(req.body.entries().next().value[1], true)
      } else {
        this.macchineEditImageStorageService.depositaImmagine(req.body.entries().next().value[1], false)
      }

      const success = of(new HttpResponse({ status: 200 })); //PROMEM
      return success;
    }

    if (req.url === "removeLarge" || req.url === "removaSmall") {
      return of(new HttpResponse({ status: 200 }));
    }
    return next.handle(req);
  }
}
