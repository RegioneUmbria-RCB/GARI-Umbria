import {Component, DestroyRef, inject, Inject, OnDestroy, OnInit, SecurityContext, ViewChild} from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogCloseResult } from '@progress/kendo-angular-dialog';
import { ExpansionPanelActionEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { CentroAziendale, PKCentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { IndirizzoAssociato } from '../../../Model/anagrafiche/addresses/IndirizzoAssociato';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { CentriAziendaliService, LeggiCentriAziendali } from 'app/Service/Anagrafica/centri.service';
import { IMPRESE_SERVICE_TOKEN, ImpreseFactoryService } from 'app/Service/ServiceFactory/imprese.factory.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import {GiasDialogAction, GiasDialogService} from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { BIO_Dati_OrientamentoProduttivoService } from 'app/Service/Metaschema/BIO_Dati_OrientamentoProduttivo.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {ObjParametriAgenda} from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CODICI_TOKEN } from 'app/Utility/Template/codici-template/models/codici.model';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import {catchError, distinctUntilChanged, filter, from, interval, lastValueFrom, of, Subject, take, takeUntil, tap, withLatestFrom} from 'rxjs';
import { AppezzamentoEditCodici } from './appezzamento-edit-codici.service';
import { AppezzamentoEditService } from './appezzamento-edit.service';
import { DatiCatastaliHttpService } from './dati-catastali/dati-catastali-http.service';
import { CatastoAppezzamento_Extended, DatiCatastaliService } from './dati-catastali/dati-catastali.service';
import { CAMPI_SERVICE_TOKEN, CampiFactoryService } from 'app/Service/ServiceFactory/campi.factory.service';
import { IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService } from 'app/Service/ServiceFactory/impianti.factory.service';
import {DatePipe, KeyValue, Location} from '@angular/common';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { DestinazioneUso } from '../../../Model/metaschema/utilizzi/DestinazioneUso';
import { MetodoProduzione } from '../../../Model/metaschema/MetodoProduzione';
import { TranslocoService } from '@jsverse/transloco';
import {Enum_AssociazioneGIS} from 'app/GIS/GIS-kendo-window/polygon-window/polygon-window.component';
import {  contestoPostMessage, messaggioPostMessage, PostMessageStrutturata } from 'gias-ui-kit';
import { SharedDataService } from '../../../GIS/services/shared-data.service';
import { SementieriParametrizzazione } from '../../../Model/GIS/SementieriParametrizzazione';
import { Vincolo } from '../../../Model/metaschema/Vincoli';
import { IndirizziParentFormDataService } from './impianto-edit-indirizzi/indirizzi-parent-from.service';
import { GruppiRaccoltaService } from '../../../Service/GruppiRaccolta/gruppi-raccolta.service';
import { cloneDeep } from 'lodash';
import { GiasNumericTemplateComponent } from 'gias-ui-kit';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { Irrigazione } from 'app/Model/metaschema/Irrigazione';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import { AppezzamentoFiltroTemporale } from 'app/Service/api.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import { SementieriService } from 'app/Service/sementieri.service';

@Component({
  standalone: false,
  selector: 'app-appezzamento-edit',
  templateUrl: './appezzamento-edit.component.html',
  styleUrls: ['./appezzamento-edit.component.css'],
  providers: [
    { provide: CODICI_TOKEN, useClass: AppezzamentoEditCodici },
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
    IndirizziParentFormDataService,
    ...generateGridProviders(DatiCatastaliHttpService, AppezzamentoEditComponent)
  ]
})
export class AppezzamentoEditComponent implements OnInit, OnDestroy {
  @ViewChild('tabstripImpianti') public stripImpianti: TabStripComponent;
  @ViewChild('txtSuperficie') public txtSuperficie: GiasNumericTemplateComponent;

  signal$: Subject<void> = new Subject();

  imagePath: SafeResourceUrl;
  imageExist: boolean = false;
  AppezzaValue: string = '';
  AppezzaObjValue: string = '';
  saving: boolean = false;

  protected edit: boolean = true;
  protected mostraForm: boolean = false;
  protected Enum_DBTypeOperation = Enum_DBTypeOperation;
  protected lista_Centri: { codice: number; descrizione: string }[];
  protected listaCampi: { codice: number; descrizione: string }[];
  protected defaultItem: { codice: 0, descrizione, '' };
  protected AppezzamentoEditForm: FormGroup;
  protected salvaNuovo: boolean = true;
  protected AGRODATA_INIZIO: Date = AGRODATAINIZIO;
  private tempSabbia: number = null;
  private tempArgilla: number = null;

  // ------------------------- proprieta per salvataggio GIS --------------------------
  public presaveForm: FormGroup;
  public salvataggioGIS: boolean = false;
  public isNewGIS: boolean = false;
  public datiAngraficaGIS: boolean = false;
  public createFrom: boolean = false;
  private area: number = 0;
  // ----------------------------------------------------------------------------------

  private inFrame: boolean = false;
  private checkModificaSuperficie = false;
  private caricamentoFormCompletato: Subject<void> = new Subject();
  private appezzamento: Appezzamento;
  private objParametriAgenda: ObjParametriAgenda;

  private vincoloBio: Vincolo = {
    "disciplinare": {
      "disciplinarePubblicoPrivato": 1,
      "regolamentoConcimazione": {
        "tipo": 0,
        "codice": 0,
        "descrizione": "Nessuno"
      },
      "raggruppamentiColturaliDPI": null,
      "gruppoFinalita": { codice: 0, descrizione: '', specieCod: 0 },
      "flagProtetto": 0,
      "idTr": 3,
      "validita": new IntervalloTemporale(),
      "codice": "0",
      "descrizione": ""
    },
    "regolamento": {
      "codice": 4,
      "descrizione": "Bio"
    },
    "codice": "4",
    "descrizione": "Bio"
  };

  private Array_Esposizione: Array<{ descrizione: string; codice: string }> = [
    { descrizione: 'nord', codice: 'nord' },
    { descrizione: 'nord-est', codice: 'nord-est' },
    { descrizione: 'est', codice: 'est' },
    { descrizione: 'sud-est', codice: 'sud-est' },
    { descrizione: 'sud-ovest', codice: 'sud-ovest' },
    { descrizione: 'ovest', codice: 'ovest' },
    { descrizione: 'nord-ovest', codice: 'nord-ovest' }
  ];

  private Array_Ubicazione: Array<{ descrizione: string; codice: string }> = [
    { descrizione: 'Pianura', codice: 'pianura' },
    { descrizione: 'Mezza Costa', codice: 'mezza costa' },
    { descrizione: 'Collina', codice: 'collina' },
    { descrizione: 'Montagna', codice: 'montagna' }
  ];

  private dftClasseTessitura = { codice: 0, descrizione: '' };
  private destroyRef = inject(DestroyRef);

  constructor(
    private route: ActivatedRoute,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    public appezzamentoEditService: AppezzamentoEditService,
    private fb: FormBuilder,
    private objParametriAgendaService: ObjParametriAgendaService,
    private masterservice: MasterService,
    private specievegetaliservice: SpecieVegetaliService,
    private daticatastaliservice: DatiCatastaliService,
    private fcService: FunzioniComuniService,
    private _sanitizer: DomSanitizer,
    private giasDialogService: GiasDialogService,
    private datePipe: DatePipe,
    private centriService: CentriAziendaliService,
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private bio_Dati_OrientamentoProduttivoService: BIO_Dati_OrientamentoProduttivoService,
    private giasMessageService: GiasMessageService,
    private gpubService: GridPublicService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private dialogService: GiasDialogService,
    private router: Router,
    @Inject(CODICI_TOKEN) private appezzamentoEditCodici: AppezzamentoEditCodici,
    private transloco: TranslocoService,
    private indirizziParentFormDataService: IndirizziParentFormDataService,
    private sharedDataService: SharedDataService,
    private gruppiRaccoltaservice: GruppiRaccoltaService,
    private funzioniComuniService: FunzioniComuniService,
    private location: Location,
    private sementieriService: SementieriService
  ) {
    this.AppezzamentoEditForm = this.appezzamentoEditService.getAppezzamentoForm();
    this.presaveForm = this.buildPresaveForm();

    this.presaveForm.controls['generaSuiLayer'].markAsTouched();

    this.presaveForm.controls['supNuova'].valueChanges.pipe(takeUntil(this.signal$)).subscribe(() => {
      this.updateSuperficieGIS();
    });

    this.presaveForm.controls['associa'].valueChanges.pipe(takeUntil(this.signal$)).subscribe(() => {
      if (!this.createFrom)
        this.updateSuperficieGIS();
    });

    this.AppezzamentoEditForm.get('validita').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter((v: IntervalloTemporale, _) => v?.inizio == undefined || v?.fine == undefined)
    ).subscribe((v: IntervalloTemporale) => {
      this.AppezzamentoEditForm.get('validita').setValue({inizio: v?.inizio ?? AGRODATAINIZIO, fine: v?.fine ?? AGRODATAFINE});
    });
  }

  ngOnInit() {
    // handle sementieri params first
    const queryParams = this.route.snapshot.queryParams;
    if (queryParams.cfgSementi != undefined && SementieriParametrizzazione.isInstanceOfSementieri(JSON.parse(queryParams.cfgSementi))) {
      this.sharedDataService.setCfgSementi(new SementieriParametrizzazione(JSON.parse(queryParams.cfgSementi)));
    }

    this.appezzamentoEditService.savings.next(false);
    this.caricamentoFormCompletato.pipe(takeUntil(this.signal$)).subscribe(() => {
      this.onCaricamentoFormCompletato();
    });

    this.caricaControlli();

    this.AppezzaValue = JSON.stringify(this.AppezzamentoEditForm.value);
    this.AppezzamentoEditForm.valueChanges.pipe(takeUntil(this.signal$)).subscribe(el => {
      this.AppezzaValue = JSON.stringify(this.AppezzamentoEditForm.value);
    });

    this.appezzamentiService.currentAppezzamento.pipe(
      withLatestFrom(this.sementieriService.isSementieriSportello()),
      takeUntil(this.signal$),
    ).subscribe(async ([newAppezza, isSementieri]) => {
      this.appezzamento = newAppezza;

      if (!!this.appezzamento) {
        this.updateFormFromAppezza(this.appezzamento, isSementieri);

        this.AppezzaObjValue = JSON.stringify(this.appezzamento);
        this.AppezzamentoEditForm.controls['catastoAppezzamento'].valueChanges.pipe(takeUntil(this.signal$)).subscribe((el) => {
          this.editCatastoAppezzamento(el);
        });

        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
          this.AppezzamentoEditForm.disable();
          this.edit = false;
        }

        // ------------------------------------ DISABILITO I FORM DEGLI IMPIANTI NON SELEZIONATI ------------------------------------
        if (this.salvataggioGIS) {
          (<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls.forEach(i => {
            if (i.value.primaryKey.codice != this.objParametriAgenda.Id_Reg || this.createFrom) {
              i.disable();
            }
          });
        }
        // ---------------------------- CREO NUOVO IMPIANTO SE SONO IN CREAZIONE DA POLIGONO SELEZIONATO ----------------------------
        if (this.createFrom)
          this.onAddImpianto();
        // --------------------------------------------------------------------------------------------------------------------------

        this.mostraForm = true;
        interval(0).pipe(take(1)).subscribe(() => this.disabilitaTxtSuperficie());
        this.appezzamentoEditCodici.setCodici(this.AppezzamentoEditForm.getRawValue().codici);

        this.AppezzamentoEditForm.controls["codici"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((el) => {
          this.appezzamentoEditCodici.setCodici(el);
        });

        this.appezzamentoEditCodici.currentodici.pipe(takeUntil(this.signal$)).subscribe((vals) => {
          this.AppezzamentoEditForm.controls['codici'].setValue(vals, { emitEvent: false });
        });

        this.daticatastaliservice.setCaricaCatastoSettings({
          Piva: this.appezzamento.primaryKey.centroAziendalePK.partitaIva,
          Sa_Cod: this.appezzamento.primaryKey.centroAziendalePK.codice,
          Campo_Cod: this.appezzamento.campoPK.codice,
          chkMacrousi: false,
          chkUtilizzi: false,
          chkVarieta: false,
          Validita_Inizio: this.appezzamento.validita.inizio,
          Validita_Fine: this.appezzamento.validita.fine
        });

        this.AppezzamentoEditForm.controls['metodo_Produzione'].valueChanges.pipe(
          takeUntil(this.signal$)
        ).subscribe((mp: MetodoProduzione) => {
          if (mp.codice == 3 || mp.codice == 2) {
            let mostraDialog: boolean = false;
            for (let i = 0; i < (<FormArray>this.AppezzamentoEditForm.controls['impianti']).length; i++) {
              let impianto: FormGroup = (<FormArray>this.AppezzamentoEditForm.controls['impianti']).at(i) as FormGroup;
              for (let j = 0; j < (<FormArray>impianto.controls['esercizi']).length; j++) {
                let esercizio: FormGroup = (<FormArray>impianto.controls['esercizi']).at(i) as FormGroup;
                if ((<FormGroup>esercizio?.controls['regolamento'])?.value?.codice != 4) {
                  mostraDialog = true;
                }
              }
            }

            if (mostraDialog && !this.saving) {
              let obsResp = this.giasDialogService.dialogMessageObs_Result(
                this.transloco.translate('RegolamentoBiologico'),
                this.transloco.translate('ImpostareRegTuttiEsercizi')
              );

              obsResp.pipe(take(1)).subscribe(resp => {
                if ((<any>resp).primary) {
                  for (let i = 0; i < (<FormArray>this.AppezzamentoEditForm.controls['impianti']).length; i++) {
                    let impianto: FormGroup = (<FormArray>this.AppezzamentoEditForm.controls['impianti']).at(i) as FormGroup;
                    for (let j = 0; j < (<FormArray>impianto.controls['esercizi']).length; j++) {
                      let esercizio: FormGroup = (<FormArray>impianto.controls['esercizi']).at(i) as FormGroup;
                      if (!esercizio) continue;
                      (<FormGroup>esercizio.controls['regolamento']).setValue({
                        codice: 4,
                        descrizione: "Reg. UE 848/2018 (Ex Reg. CE 834/07)"
                      });
                      (<FormGroup>esercizio.controls['vincolo']).setValue(this.vincoloBio);
                    }
                  }
                }
              })
            }
          }
        });

        interval(0).pipe(take(1)).subscribe(() => {
          this.caricamentoFormCompletato.next();
        });
      }
    });

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      let impresa: Impresa = new Impresa();
      impresa.partitaIva = this.objParametriAgenda.Piva;

      this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{
        impresa: impresa,
        data: (<FormGroup>this.AppezzamentoEditForm.controls["validita"]).controls["inizio"].value
      }, false)
        .then(vals => {
          if (vals.length === 0) {
            this.giasDialogService.dialogMessageRef(
              this.transloco.translate('NoCompanyCentre'),
              this.transloco.translate('CompanyCentreIsNecessary'),
              [new GiasDialogAction('OK', {res: true}, true)]
            ).result.pipe(take(1)).subscribe(() => {
              if (this.inFrame) {
                window.parent.postMessage(messaggioPostMessage.chiudiWindowGiasNG, this.funzioniComuniService.getOrigins());
              } else {
                this.location.back();
              }
            });
          }

          this.lista_Centri = vals.map(el => { return { codice: el.primaryKey.codice, descrizione: el.nome } });
          this.AppezzamentoEditForm.get('primaryKey').get('centroAziendalePK').get("codice").setValue(this.lista_Centri[0]?.codice);
        });

      this.AppezzamentoEditForm.get('primaryKey').get('centroAziendalePK').get("codice").valueChanges.pipe(
        takeUntil(this.signal$)
      ).subscribe(val => {
        const centro = new CentroAziendale({
          codice: this.AppezzamentoEditForm.get('primaryKey').get('centroAziendalePK').get("codice").value,
          partitaIva: this.objParametriAgenda.Piva
        });

        this.indirizziParentFormDataService.centroSelezionato = centro;
        this.campiService.LeggiCampi(
          {
            centro: centro,
            data: (<FormGroup>this.AppezzamentoEditForm.controls['validita']).controls['inizio'].value
          },
          false,
          ''
        ).then(fields => {
          this.listaCampi = fields.map(el => ({ codice: el.primaryKey.codice, descrizione: el.descrizione }));
          this.listaCampi.splice(0, 0, { codice: 0, descrizione: '' });
          this.AppezzamentoEditForm.get('campoPK').get('codice').setValue(0);
        });
      });

      this.AppezzamentoEditForm.controls['superficie'].valueChanges.pipe(
        takeUntil(this.signal$)
      ).subscribe((val: number) => {
        (<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls.forEach((impiantoForm: FormGroup) => {
          if ((!impiantoForm.controls['superficie'].touched || impiantoForm.controls['superficie'].value == 0) && this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
            impiantoForm.controls['superficie'].setValue(val);
          }
        });
      });

      this.AppezzamentoEditForm.controls['validita'].valueChanges.pipe(
        takeUntil(this.signal$),
        filter(() => this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write),
        filter((v: IntervalloTemporale, _) => v?.inizio != undefined && v?.fine != undefined),
        distinctUntilChanged()
      ).subscribe((val: IntervalloTemporale) => {
        if ((<FormArray>this.AppezzamentoEditForm.controls['impianti']).length == 1) {
          if (!(<FormGroup>(<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls['0']).controls['validita'].touched) {
            (<FormGroup>(<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls['0']).controls['validita'].setValue(val);
          }
        }
      });
    }

    this.AppezzamentoEditForm.get('primaryKey').get('centroAziendalePK').valueChanges
      .pipe(takeUntil(this.signal$))
      .subscribe(val => {
        let dati = this.daticatastaliservice.getCaricaCatastoSettings();
        dati.Sa_Cod = val.codice;
        this.daticatastaliservice.setCaricaCatastoSettings(dati);
      });

    this.AppezzamentoEditForm.get('campoPK').valueChanges.pipe(takeUntil(this.signal$)).subscribe(val => {
      let dati = this.daticatastaliservice.getCaricaCatastoSettings();
      dati.Campo_Cod = val.codice;
      this.daticatastaliservice.setCaricaCatastoSettings(dati);
    });

    this.handleQueryParams();

    if (this.salvataggioGIS) {
      ((this.AppezzamentoEditForm.controls['impianti'] as FormArray)?.controls[0] as FormGroup)?.controls['utilizzoTerreno'].valueChanges.subscribe(ut => {
        console.log(ut);
        this.presaveForm.controls['vegCod'].setValue(ut.specie.codice);
      });
      ((this.AppezzamentoEditForm.controls['impianti'] as FormArray)?.controls[0] as FormGroup)?.controls['gruppoVarietale'].valueChanges.subscribe(gv => {
        console.log(gv);
        this.presaveForm.controls['grvaCod'].setValue(gv.codice);
      });
      ((this.AppezzamentoEditForm.controls['impianti'] as FormArray)?.controls[0] as FormGroup)?.controls['validita'].valueChanges.subscribe(v => {
        (this.presaveForm.controls['validita'] as FormGroup).controls['inizio'].setValue(v.inizio);
        (this.presaveForm.controls['validita'] as FormGroup).controls['fine'].setValue(v.fine);
      });
    }
  }

  ngOnDestroy() {
    let objPAgenda = this.objParametriAgendaService.getObjParamValue();
    objPAgenda.Sa_Cod = 0;
    objPAgenda.Campo_Cod = 0;
    objPAgenda.Appezza = 0;
    objPAgenda.Id_Reg = 0;
    objPAgenda.Progetto_Cod = 0;
    this.objParametriAgendaService.changeObjParametriAgenda(objPAgenda);
    this.signal$.next();
    this.signal$.complete();
  }

  onBlurValidita(event: any) {
    if (event == 'inizio') {
      let data = this.AppezzamentoEditForm.get('validita').get('inizio').value;
      let dati = this.daticatastaliservice.getCaricaCatastoSettings();
      dati.Validita_Inizio = data;
      this.daticatastaliservice.setCaricaCatastoSettings(dati);
    } else if (event == 'fine') {
      let data = this.AppezzamentoEditForm.get('validita').get('fine').value;
      let dati = this.daticatastaliservice.getCaricaCatastoSettings();
      dati.Validita_Fine = data;
      this.daticatastaliservice.setCaricaCatastoSettings(dati);
    } else {
      console.log("QUI NON ME LO ASPETTAVO");
    }
  }

  onAddImpianto() {
    const arrFormImpianti = this.AppezzamentoEditForm.controls['impianti'] as FormArray;
    const appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();

    if (arrFormImpianti.length > 0) {
      const lastFormImpianto: FormGroup = arrFormImpianti.controls[arrFormImpianti.length - 1] as FormGroup;
      const lastImpiantoValue: Impianto = (arrFormImpianti.controls[arrFormImpianti.length - 1] as FormGroup).getRawValue();
      const deepCopy: FormGroup = this.fcService.cloneAbstractControl(lastFormImpianto) as FormGroup;
      const dataFine: Date = lastImpiantoValue.validita.fine as Date;

      this.appezzamentiService.getCodiceImpianto(
        this.objParametriAgenda.Piva,
        lastFormImpianto.value.validita.inizio.getFullYear()
      ).pipe(
        take(1)
      ).subscribe(r => {
        deepCopy.controls['codiceImpianto'].setValue(r.codiceImpianto);
        deepCopy.controls['algoritmoCodifica'].setValue(r.algoritmoCodifica);

        if (this.createFrom) {
          deepCopy.enable();
          deepCopy.controls['superficie'].setValue(Number(this.area));
        }

        if (dataFine.toUTCString() == AGRODATAFINE.toUTCString()) {
          this.giasMessageService.warningMessage(this.transloco.translate('PerInserireImpianto'), true);
        } else {
          let validitaInizio: Date = new Date(dataFine);
          validitaInizio.setDate(validitaInizio.getDate() + 1);

          let validitaFine: Date = new Date(validitaInizio);
          validitaFine.setFullYear(validitaInizio.getFullYear() + 1);
          validitaFine.setDate(validitaFine.getDate() - 1);

          deepCopy.controls['primaryKey'].setValue({
            codice: 0,
            appezzamentoPK: this.AppezzamentoEditForm.controls['primaryKey'].value
          });

          deepCopy.controls['validita'] = this.fb.group({
            inizio: validitaInizio,
            fine: validitaFine
          });

          if (appezzamento.validita.fine < validitaFine) {
            this.AppezzamentoEditForm?.get('validita')?.get('fine')?.setValue(validitaFine);
          }

          deepCopy.controls['id'].setValue(this.getMaxIdImpianto() + 1);
          deepCopy.controls['immagineBase64'].setValue('');
          let esercizi = (<FormArray>deepCopy.controls['esercizi']).getRawValue() as Esercizio[];

          for (let ii = 0; ii < esercizi.length; ii++) {
            if (ii == 0) {
              const DataInizioEsercizio = lastImpiantoValue.esercizi[ii].validita.inizio;
              const DataFineEsercizio = lastImpiantoValue.esercizi[ii].validita.fine;

              let validitaInizioEsercizio = new Date(DataInizioEsercizio);
              validitaInizioEsercizio.setFullYear(DataInizioEsercizio.getFullYear() + 1);
              let validitaFineEsercizio = new Date(DataFineEsercizio);
              validitaFineEsercizio.setFullYear(DataFineEsercizio.getFullYear() + 1);

              validitaInizioEsercizio = validitaInizio;
              validitaFineEsercizio = validitaFine;

              let esercizio = esercizi[ii];
              esercizio.codice = 0;
              esercizio.lotto = "";
              esercizio.esercizio_Chiuso = false;
              esercizio.validita.inizio = validitaInizioEsercizio;
              esercizio.validita.fine = validitaFineEsercizio;
            }
          }

          esercizi = cloneDeep([esercizi[0]]);

          if ((<FormArray>deepCopy.controls['esercizi']).value.length > 1) {
            for (let iii = ((<FormArray>deepCopy.controls['esercizi']).value.length - 1); iii > 0; iii--) {
              (<FormArray>deepCopy.controls['esercizi']).removeAt(iii);
            }
          }

          (<FormArray>deepCopy.controls['esercizi']).setValue(esercizi);

          // imposto la data di inizio produzione (impianto), pari alla validita inizio dell esercizio
          if (esercizi[0].apportiMassimiMacroelementi.fase.codice == 102) {
            deepCopy.get('data_Inizio_Produzione').setValue(esercizi[0].validita.inizio);
          }

          arrFormImpianti.push(deepCopy);

          setTimeout(() => {
            this.stripImpianti?.selectTab(arrFormImpianti.length - 1);
          }, 100);
        }
      });
    } else {
      const validitaInizio = this.AppezzamentoEditForm.controls['validita'].value.inizio;
      const validitaFine = this.AppezzamentoEditForm.controls['validita'].value.fine;

      // TODO Salvo: pulire tutto il codice relativo al metodo getCodiceImpianto(), quando ci sarà tempo di fare le cose per bene
      this.appezzamentiService.getCodiceImpianto(
        this.objParametriAgenda.Piva,
        validitaInizio.getFullYear()
      ).pipe(
        withLatestFrom(this.sementieriService.isSementieriSportello()),
        take(1)
      ).subscribe(([r, isSementieri]) => {
        const newImpianto = this.appezzamentoEditService.getImpiantoForm(
          0,
          this.AppezzamentoEditForm.controls['primaryKey'].value,
          validitaInizio,
          validitaFine,
          isSementieri
        );

        newImpianto.controls['codiceImpianto'].setValue(r.codiceImpianto);
        newImpianto.controls['algoritmoCodifica'].setValue(r.algoritmoCodifica);

        arrFormImpianti.push(newImpianto);
        setTimeout(() => {
          this.stripImpianti?.selectTab(0);
        }, 100);
      });
    }
  }

  onRemoveImpiantoTabStrip() {
    let ArrTabs = this.stripImpianti.tabs.toArray();
    let selectedIndex = ArrTabs.findIndex((el) => el.selected == true);
    const arrFormImpianti = this.AppezzamentoEditForm.controls['impianti'] as FormArray;
    arrFormImpianti.removeAt(selectedIndex);
    if (ArrTabs.length > selectedIndex && selectedIndex > 0) {
      this.stripImpianti?.selectTab(selectedIndex - 1);
    } else if (selectedIndex == 0) {
      this.stripImpianti?.selectTab(1);
    } else {
      this.stripImpianti?.selectTab(0);
    }
  }

  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'metodo_Produzione':
        ddlEl.listItems = this.appezzamentiService.getArray_Metodo_Produzione();
        break;
      case 'coltura_Precedente_1_Anno':
      case 'coltura_Precedente_2_Anno':
      case 'coltura_Precedente_3_Anno':
      case 'coltura_Precedente_4_Anno':
        ddlEl.listItems = await this.specievegetaliservice.leggi();
        break;
      case 'esposizione':
        ddlEl.listItems = this.Array_Esposizione;
        break;
      case 'ubicazione':
        ddlEl.listItems = this.Array_Ubicazione;
        break;
      case 'utilizzo_Terreno':
        ddlEl.listItems = await lastValueFrom(this.bio_Dati_OrientamentoProduttivoService.leggi().pipe(take(1)));
        break;
      case 'codice':
        switch (ddlEl.name) {
          case 'centro':
            let impresa = new Impresa();
            impresa.partitaIva = this.objParametriAgenda.Piva;
            ddlEl.listItems = (await this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: AGRODATAINIZIO }, false)).map(el => {
              return { codice: el.primaryKey.codice, descrizione: el.nome };
            });
            break;
          case 'campo':
            break;
        }
    }
  }

  async onValueChange(identifier: string, value: any) {
    switch (identifier) {
      case 'sabbia':
        this.tempSabbia = value;
        break;
      case 'argilla':
        this.tempArgilla = value;
        break;
    }
  }

  async onBlur(identifier: string) {
    switch (identifier) {
      case 'sabbia':
      case 'argilla':
        if (this.tempSabbia >= 0 && this.tempArgilla >= 0) {
          this.appezzamentiService.leggiClasseTessitura({ Id_ClasseTessitura: 0, sabbia: this.tempSabbia, argilla: this.tempArgilla }).then(classeTessitura => {
            if (classeTessitura == null)
              classeTessitura = this.dftClasseTessitura;
            this.AppezzamentoEditForm.controls['classeTessitura'].patchValue(classeTessitura);
          });
        } else {
          this.AppezzamentoEditForm.controls['classeTessitura'].patchValue(this.dftClasseTessitura);
        }
        break;
    }
  }

  salvaAppezzamento() {
    this.saving = true;
    this.appezzamentoEditService.savings.next(true);
    this.checkAppezzamento().pipe(
      takeUntil(this.signal$),
      catchError(e => {
        this.saving = false;
        return of(null);
      })
    ).subscribe((val) => {
      this.appezzamentoEditService.savings.next(false);
      this.saving = false;
      this.masterservice.set_isLoading({ isLoading: false });
      this.appezzamentoEditService.savings.next(false);
      if (!val) {
        if (this.salvataggioGIS && !this.datiAngraficaGIS) {
          this.datiAngraficaGIS = true;
        }
        return;
      }
      if (val.RispostaOK) {
        this.giasMessageService.successMessage(this.transloco.translate('AppezzamentoSalvato'));
        // TODO : Redirect da rivedere
        this.gestioneRedirect(val.RispostaStringa);
      } else {
        //this.masterservice.handleErrori_Gestiti(val.ErroriGias)
        //this.giasMessageService.errorMessage(this.transloco.translate('ErroreSalvataggio'));
      }
    });
  }

  salvaNuovoAppezzamento() {
    this.saving = true;
    this.appezzamentoEditService.savings.next(true);
    this.checkAppezzamento().pipe(
      takeUntil(this.signal$),
      catchError(e => {
        this.saving = false;
        return of(null);
      })
    ).subscribe((val) => {
      this.appezzamentoEditService.savings.next(false);
      this.saving = false;
      this.masterservice.set_isLoading({ isLoading: false });
      this.appezzamentoEditService.savings.next(false);
      if (!val) {
        return;
      }
      if (val != undefined && val.RispostaOK) {
        this.giasMessageService.successMessage(this.transloco.translate('AppezzamentoSalvato'));

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.Sa_Cod = 0;
        this.objParametriAgenda.Campo_Cod = 0;
        this.objParametriAgenda.Appezza = 0;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

        let currentUrl = this.router.url;
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.onSameUrlNavigation = 'reload';
        this.router.navigate([currentUrl]);
      } else {
        //this.giasMessageService.errorMessage(this.transloco.translate('ErroreSalvataggio'));
        //this.masterservice.handleErrori_Gestiti(val.ErroriGias)
      }
    });
  }

  datiAggiuntiviExpand(e: ExpansionPanelActionEvent) {
    if (e.action === 'expand') {
      this.gpubService.refresh();
    }
  }

  descrizioneImpianti(impianto: Impianto, i: number): string {
    let impiantoDescr = '';

    if (impianto?.utilizzoTerreno?.classType == 'Varieta') {
      impiantoDescr = (<Varieta>impianto.utilizzoTerreno).specie.descrizione + ' - ' + (<Varieta>impianto.utilizzoTerreno).descrizione;
    } else {
      impiantoDescr = (<DestinazioneUso>impianto.utilizzoTerreno)?.descrizione;
    }

    if (impianto?.utilizzoTerreno?.codice == 0) {
      impiantoDescr = "";
    }

    let dataInizioStr = '...';
    if (impianto?.validita?.inizio != undefined && this.datePipe.transform(impianto?.validita?.inizio, 'longDate') != this.datePipe.transform(AGRODATAINIZIO, 'longDate')) {
      dataInizioStr = this.datePipe.transform(impianto.validita.inizio, 'shortDate')
    }
    let dataFineStr = '...';
    if (impianto?.validita?.fine != undefined && this.datePipe.transform(impianto?.validita?.fine, 'longDate') != this.datePipe.transform(AGRODATAFINE, 'longDate')) {
      dataFineStr = this.datePipe.transform(impianto.validita.fine, 'shortDate')
    }
    let dateDescr = dataInizioStr + ' - ' + dataFineStr;
    return impiantoDescr + ' (' + dateDescr + ')';
  }

  modificaDatiAnagrafica(e: Event): void {
    this.datiAngraficaGIS = true;
    let appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();
    let index = appezzamento.impianti.findIndex((imp: Impianto) => imp.primaryKey.codice == this.objParametriAgenda.Id_Reg);

    if (index >= 0) {
      setTimeout(() => {
        this.stripImpianti?.selectTab(index);
        document.getElementById('tab').scrollIntoView();
      }, 0);
    }
  }

  ageaCodes(): KeyValue<string, string>[] {
    return [
      {
        key: this.transloco.translate('Agea_idAppezzamentoOrig'),
        value: this.AppezzamentoEditForm.controls['Agea_idAppezzamentoOrig'].value ?? ''
      },
      {
        key: this.transloco.translate('Agea_idSchedaValidazione'),
        value: this.AppezzamentoEditForm.controls['Agea_idSchedaValidazione'].value ?? ''
      },
      {
        key: this.transloco.translate('Agea_identificativoPianoColtivazione'),
        value: this.AppezzamentoEditForm.controls['Agea_identificativoPianoColtivazione'].value ?? ''
      },
      {
        key: this.transloco.translate('Agea_codiBarrScheVali'),
        value: this.AppezzamentoEditForm.controls['Agea_codiBarrScheVali'].value ?? ''
      },
      {
        key: this.transloco.translate('Agea_identificativoIsola'),
        value: this.AppezzamentoEditForm.controls['Agea_identificativoIsola'].value ?? ''
      },
      {
        key: this.transloco.translate('Agea_identificativoAppezzamento'),
        value: this.AppezzamentoEditForm.controls['Agea_identificativoAppezzamento'].value ?? ''
      }
    ];
  }

  private handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntil(this.signal$))
      .subscribe(params => {
        if (params.seFrame == 1) {
          this.salvaNuovo = false;
          this.inFrame = true;
        }
        if (params.wkt != undefined && params.wkt != '') {
          this.salvataggioGIS = true;
          this.isNewGIS = this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write;
        }
        if (params.area != undefined && params.area != '') {
          if (this.isNewGIS) {
            this.appezzamento.superficie = params.area;
            this.AppezzamentoEditForm.controls['superficie'].setValue(params.area);
          }
          this.area = params.area;
        }
        if (params.createFrom !== undefined && params.createFrom !== '') {
          this.createFrom = params.createFrom?.trim()?.toLowerCase() == 'true';
        }
        if (params.filtroTemporale != undefined && params.filtroTemporale != '') {
          this.sharedDataService.setFiltroTemporaleAvanzato(JSON.parse(params.filtroTemporale));
        }
      });
  }

  private buildPresaveForm(): FormGroup {
    return this.fb.group({
      generaSuiLayer: new FormControl({
        descrizione: '',
        codice: Enum_AssociazioneGIS.AppezzamentoEImpianto,
      }, [Validators.required]),
      supOriginaleApp: 0,
      supOriginaleImp: 0,
      supOriginale: 0,
      supCalcolata: 0,
      supNuova: 0,
      memorizza: new FormControl({
        descrizione: '',
        codice: Enum_AssociazioneGIS.AppezzamentoEImpianto,
      }, [Validators.required]),
      associa: new FormControl({
        descrizione: '',
        codice: Enum_AssociazioneGIS.Nessuno,
      }, [Validators.required]),
      vegCod: 0,
      grvaCod: 0,
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      })
    });
  }

  //#region GIS
  private updateSuperficieGIS() {
    switch (this.presaveForm.controls['associa'].value.codice) {
      case Enum_AssociazioneGIS.Nessuno.toString():
        if (!this.isNewGIS) {
          this.ripristinaSuperficieOriginale();
        }
        break;

      case Enum_AssociazioneGIS.Impianto.toString():
        this.setSuperficieImpiantoDaGis();
        break;

      case Enum_AssociazioneGIS.Appezzamento.toString():
        this.setSuperficieAppezzamentoDaGis();
        break;

      case Enum_AssociazioneGIS.AppezzamentoEImpianto.toString():
        this.setSuperficieAppezzamentoDaGis();
        this.setSuperficieImpiantoDaGis();
        break;
    }
  }

  private ripristinaSuperficieOriginale() {
    let superficieImpiantoForm = this.getSuperficieImpiantoFormCorrente();
    let superficieImpiantoOrig = this.presaveForm.controls['supOriginaleImp'];
    if (superficieImpiantoForm && superficieImpiantoForm.value !== superficieImpiantoOrig.value) {
      superficieImpiantoForm.setValue(superficieImpiantoOrig.value);
      superficieImpiantoForm.markAsTouched();
    }
    let superficieAppezzamentoForm = this.AppezzamentoEditForm.controls['superficie'];
    let superficieAppezzamentoOrig = this.presaveForm.controls['supOriginaleApp'];
    if (superficieAppezzamentoForm.value !== superficieAppezzamentoOrig.value) {
      superficieAppezzamentoForm.setValue(superficieAppezzamentoOrig.value);
      superficieAppezzamentoForm.markAsTouched();
    }
  }

  private setSuperficieImpiantoDaGis() {
    let superficieImpiantoForm = this.getSuperficieImpiantoFormCorrente();

    if (this.presaveForm.controls['supNuova'].value == 0) {
      superficieImpiantoForm.setValue(this.presaveForm.controls['supOriginaleImp'].value);
    } else {
      superficieImpiantoForm.setValue(this.presaveForm.controls['supNuova'].value);
    }

    superficieImpiantoForm.markAsTouched();
  }

  private setSuperficieAppezzamentoDaGis() {
    let superficieAppezzamentoForm = this.AppezzamentoEditForm.controls['superficie'];

    if (this.presaveForm.controls['supNuova'].value == 0) {
      superficieAppezzamentoForm.setValue(this.presaveForm.controls['supOriginaleApp'].value);
    } else {
      superficieAppezzamentoForm.setValue(this.presaveForm.controls['supNuova'].value);
    }

    superficieAppezzamentoForm.markAsTouched();
  }

  private gestioneDatiSalvataggioGis(appezzamento: Appezzamento) {
    if (this.isNewGIS) {
      this.gestioneDatiInserimentoDaGis(appezzamento);
    } else {
      this.gestioneDatiModificaDaGis(appezzamento);
    }
  }

  private gestioneDatiInserimentoDaGis(appezzamento: Appezzamento) {
    let tipoPoligono: number = parseInt(this.presaveForm.controls['generaSuiLayer'].value.codice);
    this.impostaCartografiaGis(appezzamento, tipoPoligono);
  }

  private gestioneDatiModificaDaGis(appezzamento: Appezzamento) {
    let tipoPoligono: number = parseInt(this.presaveForm.controls['memorizza'].value.codice);
    this.impostaCartografiaGis(appezzamento, tipoPoligono);
  }

  private impostaCartografiaGis(
    appezzamento: Appezzamento,
    tipoPoligono: number
  ) {
    switch (tipoPoligono) {
      case Enum_AssociazioneGIS.Appezzamento:
        appezzamento.cartografia = this.appezzamentiService.getAppezzamento().cartografia;
        break;
      case Enum_AssociazioneGIS.Impianto:
        appezzamento.impianti.find(i => i.primaryKey.codice == this.objParametriAgenda.Id_Reg).cartografia = this.appezzamentiService.getAppezzamento().cartografia;
        break;
      case Enum_AssociazioneGIS.AppezzamentoEImpianto:
        appezzamento.cartografia = this.appezzamentiService.getAppezzamento().cartografia;
        appezzamento.impianti.find(i => i.primaryKey.codice == this.objParametriAgenda.Id_Reg).cartografia = this.appezzamentiService.getAppezzamento().cartografia;
        break;
    }
  }
  //#endregion

  private getSuperficieImpiantoFormCorrente() {
    let formImpianto = <FormGroup>
      (<FormArray>this.AppezzamentoEditForm.controls['impianti'])
        .controls.find(i => i.value.primaryKey.codice == this.objParametriAgenda.Id_Reg);

    let formImpiantoSuperfice = null;

    if (formImpianto) {
      formImpiantoSuperfice = formImpianto.controls['superficie'];
    }

    return formImpiantoSuperfice;
  }

  private updateFormFromAppezza(appezzamento: Appezzamento, isSementieri: boolean): void {
    // --------------------------------- dati per GIS ---------------------------------
    this.presaveForm.controls['supOriginaleApp'].setValue(appezzamento.superficie);
    this.presaveForm.controls['supOriginaleImp'].setValue(appezzamento.impianti
      .find(i => i.primaryKey.codice == this.objParametriAgenda.Id_Reg)?.superficie);
    // --------------------------------------------------------------------------------
    this.imageExist = false;
    this.AppezzamentoEditForm.patchValue(appezzamento, { emitEvent: false });
    if (appezzamento.immagineBase64 !== null && appezzamento.immagineBase64 !== undefined && appezzamento.immagineBase64 !== '') {
      //this.imagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,' + appezzamento.immagineBase64);
      this.imagePath = this._sanitizer.sanitize(
        SecurityContext.URL,
        'data:image/jpg;base64,' + appezzamento.immagineBase64
      )
      this.imageExist = true;
    }

    this.tempArgilla = appezzamento.argilla;
    this.tempSabbia = appezzamento.sabbia;

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      this.impreseService.impresaBiologica(this.objParametriAgenda.Piva).pipe(
        take(1),
        tap((val) => {
          if (val) {
            for (let i = 0; i < (<FormArray>this.AppezzamentoEditForm.controls['impianti']).length; i++) {
              let impianto: FormGroup = (<FormArray>this.AppezzamentoEditForm.controls['impianti']).at(i) as FormGroup;
              for (let j = 0; j < (<FormArray>impianto.controls['esercizi']).length; j++) {
                let esercizio: FormGroup = (<FormArray>impianto.controls['esercizi']).at(i) as FormGroup;
                (<FormGroup>esercizio.controls['regolamento']).setValue({
                  codice: 4,
                  descrizione: "Reg. UE 848/2018 (Ex Reg. CE 834/07)"
                });
                (<FormGroup>esercizio.controls['vincolo']).setValue(this.vincoloBio);
              }
            }
            this.AppezzamentoEditForm.controls["metodo_Produzione"].setValue({ codice: 3, descrizione: "Biologico" });
          } else {
            this.AppezzamentoEditForm.controls["metodo_Produzione"].setValue({ codice: 1, descrizione: "Integrato" });
          }
        })
      ).subscribe();
    }

    if (appezzamento.impianti as Impianto[]) {
      this.updateAppezzamentoImpiantiForm(appezzamento, isSementieri);
    }

    if (appezzamento.indirizzi as IndirizzoAssociato[]) {
      this.AppezzamentoEditForm.controls['indirizzi'].setParent(this.AppezzamentoEditForm);
      appezzamento.indirizzi.forEach((c_indirizzo, i) => {
        const indirizzoForm = this.appezzamentoEditService.getIndirizzoAssociatoForm();
        indirizzoForm.patchValue(<any>c_indirizzo, { emitEvent: false });
        const faIndirizzi = <FormArray>this.AppezzamentoEditForm.controls['indirizzi'];
        faIndirizzi.push(indirizzoForm);
      });
    }

    this.AppezzamentoEditForm.updateValueAndValidity();
  }

  private handleIsSementieri() {
    let cfgSementi: SementieriParametrizzazione = this.sharedDataService.getCfgSementiAsValue();
    if (cfgSementi.SementiMappaturaLibera == 'True') {
      return;
    }

    let sementi = cfgSementi.Sementi.split('|');

    let dataFine = FunzioniComuniService.convertStringDdMmYyyyToDate(sementi.pop().split(' ')[0]);
    let dataInizio = FunzioniComuniService.convertStringDdMmYyyyToDate(sementi.pop().split(' ')[0]);

    this.AppezzamentoEditForm.get('validita').setValue({ inizio: dataInizio, fine: dataFine });
    (<FormArray>this.AppezzamentoEditForm.get('impianti'))
      .controls[0].get('validita')
      .setValue({ inizio: dataInizio, fine: dataFine });
    (<FormArray>(<FormArray>this.AppezzamentoEditForm.get('impianti')).controls[0].get('esercizi'))
      .controls[0].get('validita')
      .setValue({ inizio: dataInizio, fine: dataFine });

    this.AppezzamentoEditForm.get('validita').disable();
    (<FormArray>this.AppezzamentoEditForm.get('impianti'))
      .controls[0].get('validita').disable();
    (<FormArray>(<FormArray>this.AppezzamentoEditForm.get('impianti')).controls[0].get('esercizi'))
      .controls[0].get('validita').disable();
  }

  private editCatastoAppezzamento(newCatastoAppezzamento: CatastoAppezzamento_Extended[]) {
    if (Array.isArray(newCatastoAppezzamento) && newCatastoAppezzamento.length > 0) {
      let sum = 0;
      for (let i = 0; i < newCatastoAppezzamento.length; i++) {
        sum += newCatastoAppezzamento[i].area;
      }
      sum = parseFloat(sum.toFixed(4));
      if (this.AppezzamentoEditForm.controls['superficie'].value != sum) {
        this.AppezzamentoEditForm.controls['superficie'].setValue(sum);
        this.AppezzamentoEditForm.controls['superficie'].markAsTouched();
      }
      this.disabilitaTxtSuperficie();
    } else {
      this.disabilitaTxtSuperficie();
    }
  }

  private async caricaControlli() {
    this.masterservice.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });

    try {
      this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
      this.disabilitacontrolli();
    } catch (e) {
      console.log('Error:', e);
    }

    this.masterservice.set_isLoading({ isLoading: false, message: '' });
  }

  private disabilitacontrolli() {
    // Se sono in Lettura disabilito tutti i controlli
    if (this.objParametriAgenda.TipoOperazioneDB === this.Enum_DBTypeOperation.Read) {
      // this.AppezzamentoEditForm.disable();
    } else {
      this.disabilitaTxtSuperficie();
    }
  }

  private disabilitaTxtSuperficie() {
    // Se il Catasto ? maggiore di 0 (sono stati selezioanti dei datiCatastali nella grid)
    // allora disabilito la textbox della superficie, altrimenti rimane abilitata
    if (this.appezzamento && this.appezzamento.catastoAppezzamento && this.appezzamento.catastoAppezzamento.length > 0) {
      interval(0).pipe(take(1)).subscribe(() => this.AppezzamentoEditForm.controls['superficie'].disable());
      //this.AppezzamentoEditForm.controls['superficie'].disable();
      this.appezzamentiService.setRipartoCatastoPresente(true);
    } else {
      interval(0).pipe(take(1)).subscribe(() => this.AppezzamentoEditForm.controls['superficie'].enable());
      //this.AppezzamentoEditForm.controls['superficie'].enable();
      this.appezzamentiService.setRipartoCatastoPresente(false);
    }
  }

  private gestioneRedirect(appezzamento: Appezzamento) {
    if (this.inFrame) {
      let objPostMessage = new PostMessageStrutturata<Appezzamento>(
        messaggioPostMessage.chiudiWindowGiasNG,
        contestoPostMessage.SalvaAppezzamento,
        appezzamento
      );
      window.parent.postMessage(objPostMessage);
    } else {
      let currentUrl = this.router.url;
      let segments = currentUrl.split('/')
      segments.splice(segments.length - 2, segments.length)
      let newUrl = '';
      segments.forEach(segment => {
        newUrl = newUrl.concat('/' + segment);
      });
      this.router.navigate([newUrl + '/Impianti']);
    }
  }

  private getMaxIdImpianto(): number {
    const arrFormImpianti = this.AppezzamentoEditForm.controls['impianti'] as FormArray;
    let max = 0;
    for (let i = 0; i < arrFormImpianti.length; i++) {
      const imp = arrFormImpianti.controls[i] as FormGroup;
      if (imp.controls['id'].value > max) {
        max = imp.controls['id'].value;
      }
    }
    return max;
  }

  private async save() {
    if (!await this.checkChangeSuperficiImpianto()) {
      return;
    }

    if (this.AppezzamentoEditForm.valid && this.superficiModificate() && !this.checkModificaSuperficie) {
      let impianti: any[] = this.impiantiDifferenti();
      let impiantiString = '';

      impianti.forEach((sup, id, array) => {
        impiantiString = impiantiString.concat((sup.utilizzoTerreno.specie != undefined ? sup.utilizzoTerreno.specie.descrizione : sup.utilizzoTerreno.descrizione) + " (" + sup.superficie.toFixed(4) + " " + this.transloco.translate('Ettari') + ")");
        if (id != array.length - 1) {
          impiantiString = impiantiString.concat(", ");
        }
      });

      let resp = await lastValueFrom(
        this.dialogService.dialogMessageObs_Result(
          this.transloco.translate('SuperficiModificate'),
          this.transloco.translate('SuperficiNonCombaciano') +
          (impianti.length > 1 ? " " + this.transloco.translate('ImpiantiInteressati') + " " : " "
            + this.transloco.translate('ImpiantoInteressato') + " ") + impiantiString + ", "
          + this.transloco.translate('AppezzamentoSuperficieDi') + " " + this.AppezzamentoEditForm.get('superficie').value.toFixed(4) + " "
          + this.transloco.translate('Ettari') + ".",
          [
            { text: this.transloco.translate('Conferma'), primary: true, returnObj: true },
            { text: this.transloco.translate('Annulla'), returnObj: false }
          ],
          undefined,
          undefined,
          e => e instanceof DialogCloseResult
        )
      );

      if (!(<any>resp).returnObj) {
        return;
      }
    }

    if (this.AppezzamentoEditForm.valid) {
      this.masterservice.set_isLoading({ isLoading: true });
      let appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();

      if (this.salvataggioGIS) {
        this.gestioneDatiSalvataggioGis(appezzamento);
      }

      let maxDataImpianto: Date = new Date(Math.max.apply(null, appezzamento.impianti.map(impianto => impianto.validita.fine)));
      let minDataImpianto: Date = new Date(Math.min.apply(null, appezzamento.impianti.map(impianto => impianto.validita.inizio)));

      if (appezzamento.validita.fine < maxDataImpianto) {
        this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('LaDataDiFineNonPuoSuperareAppezza'), errorNumber: -1 });
        this.masterservice.set_isLoading({ isLoading: false });
        return;
      }

      if (appezzamento.validita.inizio > minDataImpianto) {
        this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('LaDataDiInizioNonPuoPrecedereAppezza'), errorNumber: -1 });
        this.masterservice.set_isLoading({ isLoading: false });
        return;
      }

      for (let i = 0; i < appezzamento.impianti.length; i++) {
        let impianto = appezzamento.impianti[i];
        let maxDataEsercizio: Date = new Date(Math.max.apply(null, impianto.esercizi.map(esercizio => esercizio.validita.fine)));
        let minDataEsercizio: Date = new Date(Math.min.apply(null, impianto.esercizi.map(esercizio => esercizio.validita.inizio)));

        if (impianto.validita.fine < maxDataEsercizio) {
          this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('LaDataDiFineNonPuoSuperareImpianto'), errorNumber: -1 });
          this.masterservice.set_isLoading({ isLoading: false });
          return;
        }

        if (impianto.validita.inizio > minDataEsercizio) {
          this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('LaDataDiInizioNonPuoPrecedereImpianto'), errorNumber: -1 });
          this.masterservice.set_isLoading({ isLoading: false });
          return;
        }

        if (impianto.flagImpiantoIsMacchina) {
          impianto.macchineIrrigazione = [new ParcoMacchine(impianto.irrigazione.codice)];
          impianto.irrigazione = new Irrigazione(0, '');
        }
      }

      for (let i = 0; i < appezzamento.impianti.length; i++) {
        for (let j = i + 1; j < appezzamento.impianti.length; j++) {
          if (i != j) {
            let impiantoBase = appezzamento.impianti[i];
            let impiantoConfronto = appezzamento.impianti[j];
            if ((impiantoBase.validita.inizio >= impiantoConfronto.validita.inizio && impiantoBase.validita.inizio <= impiantoConfronto.validita.fine) ||
              (impiantoBase.validita.fine >= impiantoConfronto.validita.inizio && impiantoBase.validita.fine <= impiantoConfronto.validita.fine)) {
              this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('DateValiditaImpiantiSovrapposte'), errorNumber: -1 });
              this.masterservice.set_isLoading({ isLoading: false });
              return;
            }
          }
        }
      }

      for (let appIndex = 0; appIndex < appezzamento.impianti.length; appIndex++) {
        let impianto = appezzamento.impianti[appIndex];
        for (let i = 0; i < impianto.esercizi.length; i++) {
          for (let j = 0; j < impianto.esercizi.length; j++) {
            if (i != j) {
              let esercizioBase = impianto.esercizi[i];
              let esercizioConfronto = impianto.esercizi[j];
              if ((esercizioBase.validita.inizio >= esercizioConfronto.validita.inizio && esercizioBase.validita.inizio <= esercizioConfronto.validita.fine) ||
                (esercizioBase.validita.fine >= esercizioConfronto.validita.inizio && esercizioBase.validita.fine <= esercizioConfronto.validita.fine)) {
                this.masterservice.changeErrorMsgType({ show: true, msg: this.transloco.translate('DateValiditaEserciziSovrapposte'), errorNumber: -1 });
                this.masterservice.set_isLoading({ isLoading: false });
                return;
              }
            }
          }
        }
      }

      if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
        let centroAziendalePK: PKCentroAziendale = { codice: appezzamento.primaryKey.centroAziendalePK.codice, partitaIva: this.objParametriAgenda.Piva };
        appezzamento.primaryKey.centroAziendalePK = centroAziendalePK;

        if (appezzamento.campoPK.codice != 0) {
          appezzamento.campoPK.centroAziendalePK = centroAziendalePK;
        }
      }

      appezzamento.impianti.forEach(impianto => {
        if (impianto.primaryKey.codice == 0) {
          impianto.primaryKey.appezzamentoPK = appezzamento.primaryKey;
        }
      });

      let sovrapposizione: boolean = false;
      appezzamento.impianti.forEach((impianto: Impianto) => {
        impianto.esercizi.forEach((esercizio: Esercizio) => {
          if ((<BaseCodeDescrStr>(esercizio.organismo_Referente as any)).codice != '0') {
            let organismoReferente = new Contatto();
            organismoReferente.primaryKey = {
              codice: (<BaseCodeDescrStr>(esercizio.organismo_Referente as any)).codice,
              partitaIva: appezzamento.primaryKey.centroAziendalePK.partitaIva
            }
            organismoReferente.ragione_Sociale = (<BaseCodeDescrStr>(esercizio.organismo_Referente as any)).descrizione
            esercizio.organismo_Referente = organismoReferente;
          }

          if ((<BaseCodeDescrStr>(esercizio.riferimento_Trasferimento_Dati as any)).codice != '0') {
            let riferimentoTrasferimentoDati = new Contatto();
            riferimentoTrasferimentoDati.primaryKey = {
              codice: (<BaseCodeDescrStr>(esercizio.riferimento_Trasferimento_Dati as any)).codice,
              partitaIva: appezzamento.primaryKey.centroAziendalePK.partitaIva
            }
            riferimentoTrasferimentoDati.ragione_Sociale = (<BaseCodeDescrStr>(esercizio.riferimento_Trasferimento_Dati as any)).descrizione
            esercizio.riferimento_Trasferimento_Dati = riferimentoTrasferimentoDati;
          }

          if ((<BaseCodeDescrStr>(esercizio.magazzino_Conferimento as any)).codice != '0') {
            let arrFabbr = (<BaseCodeDescrStr>(esercizio.magazzino_Conferimento as any)).codice.split("|")
            let fabbricato_cod = arrFabbr[0];
            let sa_cod = arrFabbr[1];
            let piva = arrFabbr[2];
            esercizio.magazzino_Conferimento = new Fabbricato({
              codice: parseInt(fabbricato_cod),
              centroAziendalePK: { codice: parseInt(sa_cod), partitaIva: piva }
            });
          }

          if (esercizio.resa_prevista == undefined) {
            esercizio.resa_prevista = 0;
          }

          if (esercizio.data_Semina_Trapianto_Prevista == undefined) {
            esercizio.data_Semina_Trapianto_Prevista = AGRODATAFINE;
          }

          if (esercizio.data_Raccolta == undefined) {
            esercizio.data_Raccolta = AGRODATAFINE;
          }

          if (esercizio.data_Fioritura_Prevista == undefined) {
            esercizio.data_Fioritura_Prevista = AGRODATAINIZIO;
          }

          if (this.funzioniComuniService.codiciSovrapposti(esercizio.codici)) {
            this.masterservice.set_isLoading({ isLoading: false });
            sovrapposizione = true;
          }
        })
      });

      if (this.sementieriService.isSementieriSportello()) {
        const sportello = this.sharedDataService.getCfgSementiAsValue();
        appezzamento.dati_sementieri = sportello;
      }

      if (this.funzioniComuniService.codiciSovrapposti(appezzamento.codici)) {
        this.masterservice.set_isLoading({ isLoading: false });
        return;
      }

      if (!sovrapposizione) {
        const filtroTemporale = this.sharedDataService.getFiltroTemporaleAvanzato();
        return filtroTemporale != null
          ? this.appezzamentiService.ScriviAppezzamentoFiltroTemporale({ Appezzamento: appezzamento as any, FiltroTemporale: filtroTemporale } as AppezzamentoFiltroTemporale).toPromise()
          : this.appezzamentiService.ScriviAppezzamento_Obs(appezzamento, true, false).toPromise();
      }
    } else {
      this.AppezzamentoEditForm.markAllAsTouched();
      this.AppezzamentoEditForm.setValue(this.AppezzamentoEditForm.getRawValue());
    }
  }

  private checkChangeSuperficiImpianto(): Promise<boolean> {
    return new Promise<boolean>(async (resolve, reject) => {
      (<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls.forEach((impiantoForm: FormGroup) => {
        if (impiantoForm.controls['superficie'].touched && this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Write) {
          this.appezzamentiService.verifica_Superficie(impiantoForm.getRawValue(), impiantoForm.controls['superficie'].getRawValue()).pipe(
            take(1),
            tap((val) => {
              if (val.RispostaStringa.toString() != '') {
                return this.giasDialogService.dialogMessageObs_Result(
                  '',
                  val.RispostaStringa.toString(),
                  [
                    { text: this.transloco.translate('Conferma'), primary: true, returnObj: true },
                    { text: this.transloco.translate('Annulla'), returnObj: false }
                  ],
                  undefined,
                  undefined,
                  e => e instanceof DialogCloseResult
                ).pipe(
                  take(1),
                  tap((dialogRes) => {
                    if ((<any>dialogRes).returnObj == true) {
                      resolve((<any>dialogRes).returnObj);
                    }
                  })
                ).subscribe();
              } else {
                resolve(true);
              }
            })
          ).subscribe();
        } else {
          resolve(true);
        }
      })
    });
  }

  private checkAppezzamento() {
    return from(this.save());
  }

  private superficiModificate(): boolean {
    const appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();
    const supAppezzamento = appezzamento.superficie;
    let superficiDifferenti = false;
    let superficiModificate = false;

    for (let i = 0; i < appezzamento.impianti.length; i++) {
      if (appezzamento.impianti[i].superficie != supAppezzamento) {
        superficiDifferenti = true;
      }
    }

    if (this.AppezzamentoEditForm.controls['superficie'].touched) {
      superficiModificate = true;
    }

    (<FormArray>this.AppezzamentoEditForm.controls['impianti']).controls.forEach((impiantoForm: FormGroup) => {
      if (impiantoForm.controls['superficieAlternativa'].value == null) {
        impiantoForm.controls['superficieAlternativa'].setValue(0);
      }

      if (impiantoForm.controls['superficie'].touched) {
        superficiModificate = true;
      }
    });

    return superficiModificate && superficiDifferenti;
  }

  private impiantiDifferenti(): Impianto[] {
    const appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();
    const supAppezzamento = appezzamento.superficie;
    let impianti: Impianto[] = [];
    for (let i = 0; i < appezzamento.impianti.length; i++) {
      if (appezzamento.impianti[i].superficie != supAppezzamento) {
        impianti.push(appezzamento.impianti[i])
      }
    }

    return impianti;
  }

  private onCaricamentoFormCompletato(): void {
    let appezzamento: Appezzamento = this.AppezzamentoEditForm.getRawValue();
    let index: number = appezzamento.impianti.findIndex((imp: Impianto) => imp.primaryKey.codice == this.objParametriAgenda.Id_Reg);
    if (index >= 0) {
      this.stripImpianti?.selectTab(index);
      setTimeout(() => {
        if (this.salvataggioGIS) {
          document.getElementById('gis').scrollIntoView();
        } else {
          document.getElementById('top').scrollIntoView();
        }
      }, 0);
    }

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      this.gruppiRaccoltaservice.leggiGruppoRaccoltaImpresa(this.objParametriAgendaService.getObjParamValue()).subscribe(
        gr => {
          ((((this.AppezzamentoEditForm
            .get('impianti') as FormArray).controls[0] as FormGroup)
            .get('esercizi') as FormArray).controls[0] as FormGroup)
            .get('gruppoRaccolta')
            .setValue({ codice: gr.codice, descrizione: gr.descrizione });
        }
      );

      if (this.sharedDataService.getCfgSementiAsValue() != undefined) {
        this.handleIsSementieri();
      }
    }
  }

  private updateAppezzamentoImpiantiForm(appezzamento: Appezzamento, isSementieri: boolean): void {
    // TODO Salvo: fa parte dei commenti sotto
    // let obs: Observable<Impianto>[] = [];

    appezzamento.impianti.forEach((impianto, i) => {
      this.updateFormImpianto(appezzamento, impianto, i, isSementieri);

      if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write) {
        this.appezzamentiService.getCodiceImpianto(
          this.objParametriAgenda.Piva,
          impianto.validita.inizio.getFullYear()
        ).pipe(
          take(1)
        ).subscribe(r => {
          ((this.AppezzamentoEditForm.get('impianti') as FormArray).controls[0] as FormGroup).get('codiceImpianto').setValue(r.codiceImpianto);
          ((this.AppezzamentoEditForm.get('impianti') as FormArray).controls[0] as FormGroup).get('algoritmoCodifica').setValue(r.algoritmoCodifica);
        });
      }

      // TODO Salvo: scommentare quando ci sarà tempo per scrivere il codice bene (funziona tutto, ma bisogna sistemare il rendering della pagina)
      // if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write) {
      //   obs.push(
      //     this.appezzamentiService.getCodiceImpianto(
      //       impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
      //       impianto.validita.inizio.getFullYear()
      //     ).pipe(
      //       take(1),
      //       map(r => {
      //         impianto.codiceImpianto = r.codiceImpianto;
      //         impianto.algoritmoCodifica = r.algoritmoCodifica;
      //         return impianto;
      //       })
      //     )
      //   );
      // } else {
      //   obs.push(of(impianto));
      // }
    });

    // TODO Salvo: fa parte del commento sopra, serve a fare l'update del form group degli impianti, solo una volta letti i codiciImpianto
    // forkJoin(obs).subscribe({
    //   next: (impianti) => {
    //     impianti.forEach((impianto, i) => {
    //       this.updateFormImpianto(appezzamento, impianto, i);
    //     });
    //   }
    // });
  }

  private updateFormImpianto(appezzamento: Appezzamento, impianto: Impianto, idx: number, isSementieri: boolean): void {
    if (impianto.germinabilita === 0) {
      impianto.germinabilita = 100;
    }

    const impiantoForm: FormGroup<any> = this.appezzamentoEditService.getImpiantoForm(
      idx,
      appezzamento.primaryKey,
      appezzamento.validita.inizio,
      appezzamento.validita.fine,
      isSementieri
    );

    impiantoForm.patchValue(<any>impianto, { emitEvent: false });

    const faImpianti = <FormArray>this.AppezzamentoEditForm.controls['impianti'];
    faImpianti.push(impiantoForm);

    if (impianto.esercizi as Esercizio[]) {
      impianto.esercizi.forEach((c_esercizio, j) => {
        this.updateFormEsercizio(impiantoForm, impianto, c_esercizio, j);
      });
    }
  }

  private updateFormEsercizio(impiantoForm: FormGroup<any>, impianto: Impianto, esercizio: Esercizio, idx: number): void {
    const esercizioForm = this.appezzamentoEditService.getEsercizioForm(
      idx,
      impianto.primaryKey,
      impianto.validita.inizio,
      impianto.validita.fine
    );

    esercizioForm.patchValue(<any>esercizio, { emitEvent: false });

    esercizioForm.controls['organismo_Referente'].setValue({ codice: 0, descrizione: '' });
    esercizioForm.controls['modalita_liquidazione'].setValue({ codice: 0, descrizione: '' });
    esercizioForm.controls['origine_prodotto'].setValue({ codice: 0, descrizione: '' });
    esercizioForm.controls['riferimento_Trasferimento_Dati'].setValue({ codice: 0, descrizione: '' });
    esercizioForm.controls['magazzino_Conferimento'].setValue({ codice: 0, descrizione: '' });
    esercizioForm.controls['lavorazione'].setValue({ codice: '', descrizione: '' });
    esercizioForm.controls['specifica'].setValue({ codice: '', descrizione: '' });

    if (esercizio.organismo_Referente) {
      esercizioForm.controls['organismo_Referente'].setValue(<any>{
        codice: esercizio.organismo_Referente?.primaryKey?.codice,
        descrizione: esercizio.organismo_Referente?.ragione_Sociale
      });
    }

    if (esercizio.riferimento_Trasferimento_Dati) {
      esercizioForm.controls['riferimento_Trasferimento_Dati'].setValue(<any>{
        codice: esercizio.riferimento_Trasferimento_Dati?.primaryKey?.codice,
        descrizione: esercizio.riferimento_Trasferimento_Dati?.ragione_Sociale
      });
    }

    if (esercizio.magazzino_Conferimento) {
      esercizioForm.controls['magazzino_Conferimento'].setValue(<any>{
        codice: esercizio.magazzino_Conferimento.primaryKey.codice + '|' + esercizio.magazzino_Conferimento.primaryKey.centroAziendalePK.codice + '|' + esercizio.magazzino_Conferimento.primaryKey.centroAziendalePK.partitaIva,
        descrizione: esercizio.magazzino_Conferimento.descrizione
      });
    }

    if (esercizio.specifica) {
      esercizioForm.controls['specifica'].setValue(<any>{
        codice: esercizio.specifica.codice,
        descrizione: esercizio.specifica.descrizione
      });
    }

    if (esercizio.lavorazione) {
      esercizioForm.controls['lavorazione'].setValue(<any>{
        codice: esercizio.lavorazione.codice,
        descrizione: esercizio.lavorazione.descrizione
      });
    }

    if (esercizio.modalita_liquidazione) {
      esercizioForm.controls['modalita_liquidazione'].setValue(<any>{
        codice: esercizio.modalita_liquidazione.codice,
        descrizione: esercizio.modalita_liquidazione.descrizione
      });
    }

    if (esercizio.origine_prodotto) {
      esercizioForm.controls['origine_prodotto'].setValue(<any>{
        codice: esercizio.origine_prodotto.codice,
        descrizione: esercizio.origine_prodotto.descrizione
      });
    }

    const faEsercizi: FormArray<any> = <FormArray>impiantoForm.controls['esercizi'];
    faEsercizi.push(esercizioForm);
  }

  //** This method will be used to open centri-edit page, to allow user to create a company centre without the need to go on anagrafiche*/
  // private openCentreEditPage() {
  //   window.addEventListener('message', (e) => {
  //     if (e.data == messaggioPostMessage.chiudiWindowGiasNG) {
  //       console.log(e.data);
  //     }
  //   });
  //
  //   const newObjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));
  //   newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal;
  //   const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];
  //
  //   this.gestioneRichiesteService.gestionePassaggioAltroSito(
  //     Enum_SiteRedirector.GiasNG,
  //     enum_PagineGiasNG.Pagina_Edit_Centro,
  //     parametriAggiuntivi,
  //     newObjParametriAgenda
  //   ).then(resp => {
  //     resp += '?seFrame=1';
  //
  //     this.giasIFrameWindowService.open({
  //       title: this.transloco.translate('gis.SalvataggioPoligono'),
  //       content: resp,
  //       height: window.innerHeight,
  //       width: window.innerWidth * 0.9
  //     });
  //   });
  // }
}
