import {Component, DestroyRef, EventEmitter, inject, Inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild} from '@angular/core';
import { ControlContainer, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { FaseCicloColturale } from 'app/Model/metaschema/fase';
import { FinalitaPianoConcimazione } from 'app/Model/metaschema/FinalitaPianoConcimazione';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { RegolamentoConcimazione } from 'app/Model/metaschema/RegolamentoConcimazione';
import { GruppoFinalita } from 'app/Model/metaschema/utilizzi/GruppoFinalita';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { CodificaInfoAggiuntiveService, enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod } from 'app/Service/Codifiche/codifica_InfoAggiuntive.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { DisciplinariService, LeggiDisciplinare, LeggiIAF } from 'app/Service/Metaschema/disciplinari.service';
import { GruppoFinalitaService } from 'app/Service/Metaschema/finalita.service';
import { PianoConcimazioneService } from 'app/Service/Metaschema/pianoConcimazione.service';
import { PuaService } from 'app/Service/Metaschema/pua.service';
import { RegolamentiService } from 'app/Service/Metaschema/regolamenti.service';
import { TabelleWsClientService } from 'app/Service/Metaschema/tabelleWsClient.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CODICI_TOKEN } from 'app/Utility/Template/codici-template/models/codici.model';
import { CodiciTemplateConfigService } from 'app/Utility/Template/codici-template/services/codici-template-config.service';
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { isNumber } from 'lodash';
import {
  audit,
  delay, filter, firstValueFrom,
  from,
  lastValueFrom,
  map,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
  take,
  takeUntil,
  tap
} from 'rxjs';
import { EserciziEditService } from './esercizio-edit-codici.service';
import { VincoliService } from "../../../Service/DPI/vincoli.service";
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Vincolo } from "../../../Model/metaschema/Vincoli";
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { GruppoRaccolta } from '../../../Model/metaschema/GruppoRaccolta';
import { LeggiProdotti, ProdottiService } from 'app/Service/Anagrafica/prodotti.service';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { GruppiRaccoltaService } from '../../../Service/GruppiRaccolta/gruppi-raccolta.service';
import { Impianto } from '../../../Model/anagrafiche/Impianto';
import { DatiPrevisionaliColtureRequest } from '../../../Model/anagrafiche/DatiPrevisionaliColture';
import { ImpiantiService } from '../../../Service/Anagrafica/impianti.service';
import { ApportoMacroelementi } from "../../../Model/metaschema/ApportoMacroelementi";
import { FiltroCalcoloNPK } from "../../../Model/filtri/filtroCalcoloNPK";
import { AppezzamentoEditService } from 'app/anagrafica/appezzamenti/appezzamenti-edit/appezzamento-edit.service';
import { UnitaDiMisura } from '../../../Model/metaschema/UnitaDiMisura';
import { enum_TipoOperazioneDB, enum_UnitaMisura } from '../../../Model/TipiEnumerativi';
import { UnitaDiMisuraService } from '../../../Service/Metaschema/UnitaDiMisura.service';
import { rispostaStandard } from '../../../Service/master.service';
import { IndirizzoAssociato } from '../../../Model/anagrafiche/addresses/IndirizzoAssociato';
import { CentriAziendaliService, LeggiIndirizziCentro } from '../../../Service/Anagrafica/centri.service';
import { IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService } from '../../../Service/ServiceFactory/impianti.factory.service';
import { ContributeService } from '../../../Service/Metaschema/contribute.service';
import { Contribute, ContributeType } from '../../../Model/metaschema/Contribute';
import { GiasDropDownTemplateComponent } from 'gias-ui-kit';
import {GiasIstatService} from '../../../Service/istat/gias-istat.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: false,
  selector: 'app-esercizio-edit',
  templateUrl: './esercizio-edit.component.html',
  styleUrls: ['./esercizio-edit.component.css'],
  providers: [
    GiasMultiSelectTemplateService, { provide: CODICI_TOKEN, useClass: EserciziEditService },
    ...generateGridProviders(CodiciTemplateConfigService, EsercizioEditComponent)
  ]
})
export class EsercizioEditComponent implements OnInit, OnDestroy {
  @Input() esercizioEditForm: FormGroup;
  @Output() faseChangeEvent = new EventEmitter<{ fase: number, id: number, validitaInizio: Date; }>();
  @Output() validitaInizioChangeEvent = new EventEmitter<{ fase: number, id: number, validitaInizio: Date; }>();
  @ViewChild('descriptionsDdl') descriptionsDdl: TemplateRef<any>;

  descrizioneEserciziForm: FormGroup;

  signal$: Subject<void> = new Subject();
  validitaBlur$: Subject<void> = new Subject<void>();

  protected GruppiRaccolta: Array<GruppoRaccolta>;
  protected AGRODATA_INIZIO: Date = AGRODATAINIZIO;
  protected AGRODATA_FINE: Date = AGRODATAFINE;

  protected ListaCertificazioneAziendale: Array<BaseCodeDescr>;

  protected defaultItem = { codice: 0, descrizione: '' };
  protected defaultItemStr = { codice: '', descrizione: '' };
  protected defaultItemContatto = { primaryKey: { partitaIva: '', codice: '' }, descrizione: '' };

  protected defaultItemDisciplinare: Disciplinare = {
    codice: '',
    descrizione: '',
    disciplinarePubblicoPrivato: 1,
    regolamentoConcimazione: { codice: 0, descrizione: '', tipo: 0 },
    idTr: 3,
    raggruppamentiColturaliDPI: null,
    gruppoFinalita: null,
    flagProtetto: 0,
    validita: new IntervalloTemporale()
  };

  private leggiProdotti: LeggiProdotti = {
    impresa: new Impresa(),
    tipoAttivita: undefined,
    statoAttivita: undefined,
    lavorazione: undefined,
    impianti: [],
    prodottiDaTrattare: [],
    specie: undefined,
    disciplinare: undefined,
    epocaDPI: undefined,
    avversitaGruppo: undefined,
    filtroPerDescrizione: '',
    data: undefined,
    escludiGiacenzeZero: false,
    magazziniAgenzie: false,
    magazziniEsterni: false
  };

  private defaultItemVincolo: Vincolo = {
    codice: '1',
    descrizione: 'Nessuno',
    disciplinare: this.defaultItemDisciplinare,
    regolamento: { codice: 1, descrizione: '' }
  };

  private objParametriAgenda: ObjParametriAgenda;
  private lastUtilizzoTerreno: UtilizzoTerreno;
  private specie: Specie;

  private impiantoEditForm: FormGroup;
  private appezzamentoEditForm: FormGroup;
  private destroyRef: DestroyRef = inject(DestroyRef);

  constructor(
    private fb: FormBuilder,
    private objParametriAgendaService: ObjParametriAgendaService,
    private regolamentiService: RegolamentiService,
    private disciplinariService: DisciplinariService,
    private codificaInfoAggiuntiveService: CodificaInfoAggiuntiveService,
    private controlContainer: ControlContainer,
    private contattiService: ContattiService,
    private tabelleWsClientService: TabelleWsClientService,
    private puaService: PuaService,
    private pianoConcimazioneService: PianoConcimazioneService,
    private finalitaService: GruppoFinalitaService,
    private vincoliService: VincoliService,
    private prodottiService: ProdottiService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    @Inject(CODICI_TOKEN) private esercizioCodiciService: EserciziEditService,
    private translocoService: TranslocoService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private permessiUtenteService: PermessiUtenteService,
    private gruppiRaccoltaservice: GruppiRaccoltaService,
    private impiantiService: ImpiantiService,
    private appezzamentoEditService: AppezzamentoEditService,
    private unitaMisuraService: UnitaDiMisuraService,
    private centriService: CentriAziendaliService,
    private istatService: GiasIstatService,
    private contributeService: ContributeService
  ) {
    this.descrizioneEserciziForm = this.fb.group({
      des: new FormControl({ codice: 0, descrizione: '' })
    });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  async ngOnInit() {
    this.impiantoEditForm = <FormGroup>this.controlContainer.control;
    this.appezzamentoEditForm = (<FormGroup>this.impiantoEditForm.parent.parent);
    this.impiantoEditForm.controls['utilizzoTerreno'].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe((ut: UtilizzoTerreno) => this.onUtilizzoTerrenoChange(ut));

    this.esercizioCodiciService.setCodici(this.esercizioEditForm.value.codici);
    this.esercizioEditForm.controls["codici"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((el) => {
      this.esercizioCodiciService.setCodici(el);
    });

    this.esercizioCodiciService.currentodici.pipe(takeUntil(this.signal$)).subscribe((vals) => {
      this.esercizioEditForm.controls['codici'].setValue(vals, { emitEvent: false });
    });

    let vincoloSelected = this.esercizioEditForm.get('vincolo').getRawValue();
    this.leggiVincoli().pipe(take(1)).subscribe((val) => {
      if (vincoloSelected != undefined) {
        if (vincoloSelected?.codice != undefined && vincoloSelected.codice != '1' && vincoloSelected.codice != '4') {
          let v = val.find((el) => el.codice.split('_')[1] == vincoloSelected.codice.split('_')[1]);
          if (v != undefined) {
            this.esercizioEditForm.get('vincolo').setValue(v, { emitEvent: false });
          }
        } else {
          let v = val.find((el) => el.codice == vincoloSelected.codice);
          if (v != undefined) {
            this.esercizioEditForm.get('vincolo').setValue(v, { emitEvent: false });
          }
        }
      }
    });

    if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      from(this.impreseService.leggiImpresa(this.objParametriAgendaService.getObjParamValue())).pipe(
        take(1),
        map(impresa => {
          if (impresa.RispostaStringa.certificazione.length != 0) {
            this.esercizioEditForm.controls['certificazioneAziendale'].setValue(impresa.RispostaStringa.certificazione);
          }

          if (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_Smart_NuovoImpianto_DefaultOrganismoReferente)?.Valore == '1') {
            from(this.leggiOrganismoReferente()).pipe(take(1), map(o => {
              let OrgRef = o.filter(t => t.tipo == 'azienda');
              if (OrgRef.length == 1) {
                let azienda: Contatto = OrgRef[0];
                let descrizione;
                if (azienda.ragione_Sociale != null && azienda.ragione_Sociale != "") {
                  descrizione = azienda.ragione_Sociale;
                } else {
                  descrizione = azienda.cognome + ' ' + azienda.nome;
                }
                this.esercizioEditForm.controls['organismo_Referente'].setValue({ codice: azienda.primaryKey.codice, descrizione: descrizione });
              }
            })).subscribe();
          }

          if (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovoImpianto_DefaultTecnico)?.Valore == '1') {
            if (impresa.RispostaStringa.tecnicoReferente) {
              let tecnico: Contatto = impresa.RispostaStringa.tecnicoReferente;
              this.esercizioEditForm.controls['tecnico'].setValue([{ codice: tecnico.primaryKey.codice, descrizione: tecnico.ragione_Sociale }]);
            }
          }

          if (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_DA_PADRE)?.Valore == '1') {
            let padriImpresa = impresa.RispostaStringa.impresaPadre;
            if (padriImpresa.length == 1) {
              let azienda: ImpresaPadre = padriImpresa[0];
              this.esercizioEditForm.controls['organismo_Referente'].setValue({ codice: azienda.partitaIva, descrizione: azienda.ragioneSociale });
            }
          }
        })).subscribe();
    }

    (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["tipologia"].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe((el) => {
      if (el.codice == 0) {
        (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["n"].setValue(0);
        (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["p2o5"].setValue(0);
        (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["k2o"].setValue(0);
        (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["mgo"].setValue(0);
      }

      this.impostaNPK();
    });

    (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["fase"].valueChanges.pipe(
      takeUntil(this.signal$),
      delay(0)
    ).subscribe(
      (v) => {
        this.impostaNPK();
        const fase = (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['fase'].value.codice;
        const validitaInizio: Date = (<FormGroup>this.esercizioEditForm.controls['validita']).controls['inizio'].value;
        const id: number = this.esercizioEditForm.controls['id'].value;
        this.faseChangeEvent.emit({ fase: fase, id: id, validitaInizio: validitaInizio });
      }
    );

    (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["pianoConcimazione"].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe((el) => {
      lastValueFrom(this.leggiFinalitaConcimazione().pipe(take(1))).then(val => {
        if (val.length > 0) {
          (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["tipologia"].setValue(val[0]);
        } else {
          (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["tipologia"].setValue(this.defaultItem);
        }
      });
    });

    (<FormGroup>this.esercizioEditForm.controls['validita']).controls['inizio'].valueChanges.pipe(
      takeUntil(this.signal$),
      audit(ev => this.validitaBlur$)
    ).subscribe(val => {
      const id: number = this.esercizioEditForm.controls['id'].value;
      const fase = (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['fase'].value.codice;
      this.validitaInizioChangeEvent.emit({ fase: fase, id: id, validitaInizio: val });
    });

    this.esercizioEditForm.controls["disciplinare"].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe(async (el: Disciplinare) => {
      if (el?.regolamentoConcimazione) {
        let a = await lastValueFrom(this.puaService.leggiRegolamentoConcimazionexImpianto(this.esercizioEditForm.controls["validita"].value).pipe(take(1)));
        const regFert = a.find((val) => val.codice == el.regolamentoConcimazione.codice);

        if (regFert != null && regFert != undefined) {
          (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["pianoConcimazione"].setValue(regFert);
        } else {
          (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["pianoConcimazione"].setValue({ codice: 0, descrizione: '' });
        }
        this.esercizioEditForm.controls["iaf"].patchValue([]);
      }
    });

    this.esercizioEditForm.controls["piante_Ha"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['piante_Impianto'].setValue(
            Math.round(val * this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.controls["piante_Impianto"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val: number) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['piante_Ha'].setValue(
            Math.round(val / this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.controls["piante_Ha_Femmine"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['Piante_Ha_Impianto_Femmine'].setValue(
            Math.round(val * this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.controls["Piante_Ha_Impianto_Femmine"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val: number) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['piante_Ha_Femmine'].setValue(
            Math.round(val / this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.controls["Piante_Ha_Maschi"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['Piante_Ha_Impianto_Maschi'].setValue(
            Math.round(val * this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.controls["Piante_Ha_Impianto_Maschi"].valueChanges.pipe(takeUntil(this.signal$)).subscribe((val: number) => {
      if (isNumber(val) && isNumber(this.impiantoEditForm.controls["superficie"].value)) {
        if (val > 0 && this.impiantoEditForm.controls["superficie"].value > 0) {
          this.esercizioEditForm.controls['Piante_Ha_Maschi'].setValue(
            Math.round(val / this.impiantoEditForm.controls["superficie"].value),
            { emitEvent: false }
          );
        }
      }
    });

    this.esercizioEditForm.get('validita').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(() => this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write),
      filter((v: IntervalloTemporale) => v?.inizio == undefined || v?.fine == undefined)
    ).subscribe(v => {
      const plantValidity: IntervalloTemporale = this.impiantoEditForm.get('validita').getRawValue();

      this.esercizioEditForm.get('validita').setValue(
        new IntervalloTemporale(v?.inizio ?? plantValidity.inizio, v?.fine ?? plantValidity.fine)
      );
    });

    this.esercizioEditForm.get('data_Semina_Trapianto_Prevista').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter((v: Date) => v == undefined)
    ).subscribe(() => {
      this.esercizioEditForm.get('data_Semina_Trapianto_Prevista').setValue(AGRODATAINIZIO);
    });

    this.esercizioEditForm.get('data_Raccolta_Prevista').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter((v: Date) => v == undefined)
    ).subscribe(() => {
      this.esercizioEditForm.get('data_Raccolta_Prevista').setValue(AGRODATAFINE);
    });

    this.esercizioEditForm.get('data_Fioritura_Prevista').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter((v: Date) => v == undefined)
    ).subscribe(() => {
      this.esercizioEditForm.get('data_Fioritura_Prevista').setValue(AGRODATAINIZIO);
    });

    const esercizioInitVal: Esercizio = this.esercizioEditForm.value;
    if (esercizioInitVal.apportiMassimiMacroelementi.pianoConcimazione?.codice > 0) {
      const RegolamentiConcimazioni = await lastValueFrom(this.puaService.leggiRegolamentoConcimazionexImpianto(this.esercizioEditForm.controls["validita"].value).pipe(take(1)));
      const RegolamentoConcimazioneSel = RegolamentiConcimazioni.find((el) => el.codice == esercizioInitVal.apportiMassimiMacroelementi.pianoConcimazione.codice);
      if (RegolamentoConcimazioneSel) {
        (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["pianoConcimazione"].setValue(
          RegolamentoConcimazioneSel,
          { emitEvent: false }
        );
      }
    }

    this.esercizioEditForm.controls['vincolo'].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe((vincolo: Vincolo) => {
      if (vincolo) {
        this.esercizioEditForm.controls['disciplinare'].setValue(vincolo.disciplinare);
        (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['pianoConcimazione'].setValue(vincolo.disciplinare.regolamentoConcimazione);
        this.esercizioEditForm.controls['regolamento'].setValue(vincolo.regolamento);
      }
    });

    this.setResaTotalePrevistaSubs();

    const prodottoControl = this.esercizioEditForm.get('prodotto');
    if (prodottoControl != null) {
      prodottoControl.valueChanges
        .pipe(
          takeUntil(this.signal$),
          tap((value: DettaglioRaccolta) => {
            const varietaControl = this.impiantoEditForm.controls['utilizzoTerreno'];
            this.lastUtilizzoTerreno = value.varieta;
            if (value.varieta != null && varietaControl.value?.codice == 0) {
              varietaControl.setValue(value.varieta);
            }

            const finalitaControl = this.impiantoEditForm.get('gruppoFinalita');
            if (value.finalita != null && finalitaControl.value?.codice == 0) {
              finalitaControl.setValue(value.finalita);
            }

            const vincoloControl = this.esercizioEditForm.controls['vincolo'];
            if (value.regolamento?.codice != null && value.regolamento?.codice == 1 && (vincoloControl?.value?.codice == 0 || vincoloControl?.value?.codice == null || vincoloControl?.value?.codice == '4')) {
              this.leggiVincoli().pipe(take(1)).subscribe(vincoli => {
                const vincolo = vincoli.find(v => v.regolamento?.codice == value.regolamento?.codice);
                if (vincolo) {
                  this.esercizioEditForm.get('vincolo').setValue(vincolo);
                  let metodoProduzioneControl = this.appezzamentoEditForm.controls['metodo_Produzione'];
                  if (metodoProduzioneControl.value && metodoProduzioneControl.value.codice == 3) {
                    this.appezzamentoEditForm.controls['metodo_Produzione'].setValue({ descrizione: 'Integrato', codice: 1 });
                  }
                }
              });
            } else if (value.regolamento?.codice != null && value.regolamento?.codice == 4 && (vincoloControl?.value == null || vincoloControl?.value?.codice != 4)) {
              this.leggiVincoli().pipe(take(1)).subscribe(vincoli => {
                const vincolo = vincoli.find(v => v.regolamento?.codice == value.regolamento?.codice);
                if (vincolo) {
                  this.esercizioEditForm.get('vincolo').setValue(vincolo);
                  let metodoProduzioneControl = this.appezzamentoEditForm.controls['metodo_Produzione'];
                  if (metodoProduzioneControl.value && metodoProduzioneControl.value.codice != 3) {
                    this.appezzamentoEditForm.controls['metodo_Produzione'].setValue({ descrizione: 'Biologico', codice: 3 });
                  }
                }

              });
            }
          })
        ).subscribe();
    }

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.disabilitacontrolli();
  }

  showDescriptionButton(): boolean {
    return this.impiantoEditForm.controls['algoritmoCodifica'].value !== '';
  }

  chooseDescription(): void {
    this.giasDialogService.dialogMessageObs_Result('', this.descriptionsDdl)
      .pipe(take(1))
      .subscribe((r) => {
        if (r['returnObj']) {
          this.esercizioEditForm.controls['descrizione'].setValue(this.descrizioneEserciziForm.value.des.descrizione);
        }
      });
  }

  calcoloNPKModello(filtro: FiltroCalcoloNPK): Observable<ApportoMacroelementi> {
    if (!this.appezzamentoEditService.savings.value) {
      return this.pianoConcimazioneService.calcoloNPKModello(filtro);
    } else {
      return of(<ApportoMacroelementi>{});
    }
  }

  disabilitacontrolli(): void {
    // Se sono in Lettura disabilito tutti i controlli
    if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read) {
      this.esercizioEditForm.disable({emitEvent: false});
    }
  }

  async openDdl(ddlEl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'regolamento':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.regolamentiService.leggi().pipe(take(1)))
        );
        break;
      case 'disciplinare':
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, lastValueFrom(this.LeggiDisciplinari().pipe(take(1))));
        break;
      case 'iaf':
        ddlEl.listItems = await lastValueFrom(this.LeggiIAF().pipe(take(1)));
        break;
      case 'capitolato_Privato':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          this.codificaInfoAggiuntiveService.leggiCapitolatoPrivato()
        );
        break;
      case 'residuo':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          this.codificaInfoAggiuntiveService.leggiResiduiDisponibili()
        );
        break;
      case 'certificazioneAziendale':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.impreseService.leggi_Certificazioni().pipe(take(1)))
        );
        break;
      case 'certificazioneProdotto':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          this.codificaInfoAggiuntiveService.leggiCertProdDisponibili()
        );
        break;
      case 'contributi':
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, this.esercizioCodiciService.leggiContributi());
        break;
      case 'tecnico':
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, this.impreseService.leggiCombo_Tecnici(
          this.objParametriAgenda.Piva).then(
          risposta => {
            return risposta.RispostaStringa.map(el => {
              return {codice: el.primaryKey.codice, descrizione: el.ragione_Sociale} as BaseCodeDescrStr;
            });
          }));
        break;
      case 'organismo_Referente':
        ddlEl.listItems = (await this.leggiOrganismoReferente()).map((el: Contatto) => {
          let descrizione;
          if (el.ragione_Sociale != null && el.ragione_Sociale != "") {
            descrizione = el.ragione_Sociale;
          } else {
            descrizione = el.cognome + ' ' + el.nome;
          }
          return {codice: el.primaryKey.codice, descrizione: descrizione};
        });
        break;

      case 'modalita_liquidazione':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          new Promise((resolve, reject) => this.leggiModalitaLiquidazione().GiasSubscribe(r => resolve(r)))
        );
        break;

      case 'origine_prodotto':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          new Promise((resolve, reject) => this.leggiOrigineProdotto().GiasSubscribe(r => resolve(r)))
        );
        break;

      case 'licenza_Coltivazione':
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, this.leggiLicenzaColtivazione());
        break;
      case 'riferimento_Trasferimento_Dati':
        ddlEl.listItems = (await this.leggiRiferimento_Trasferimento_Dati()).map((el: Contatto) => {
          let descrizione;
          if (el.ragione_Sociale != null && el.ragione_Sociale != "") {
            descrizione = el.ragione_Sociale;
          } else {
            descrizione = el.cognome + ' ' + el.nome;
          }
          return { codice: el.primaryKey.codice, descrizione: descrizione }
        });
        break;
      case 'magazzino_Conferimento':
        ddlEl.listItems = (await this.leggiMagazzinoConferimento()).map((el: Fabbricato) => {
          return {
            codice: el.primaryKey.codice + '|' + el.primaryKey.centroAziendalePK.codice + '|' + el.primaryKey.centroAziendalePK.partitaIva,
            descrizione: el.descrizione
          }
        });
        break;
      case 'piano_Semina':
        //ddlEl.listItems = await this.codificaInfoAggiuntiveService.leggiPianiSemina();
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, this.codificaInfoAggiuntiveService.leggiPianiSemina());
        break;
      case 'pianoConcimazione':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.puaService.leggiRegolamentoConcimazionexImpianto(this.esercizioEditForm.controls['validita'].value).pipe(take(1)))
        );
        break;
      case 'tipologia':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.leggiFinalitaConcimazione())
        );
        break;
      case 'fase':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          new Promise((resolve, reject) => this.leggiStatoImpianto().GiasSubscribe(r => resolve(r))));
        break;
      case 'lavorazione':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          new Promise((resolve, reject) => this.leggiLavorazione().GiasSubscribe(r => resolve(r)))
        );
        break;
      case 'specifica':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          new Promise((resolve, reject) => this.leggiSpecifica().GiasSubscribe(r => resolve(r)))
        );
        break;
      case 'vincolo':
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, firstValueFrom(this.leggiVincoli()));
        break;
      case 'prodotto':
        const varieta = this.impiantoEditForm.controls['utilizzoTerreno'].value as Varieta;
        const finalita = this.impiantoEditForm.get('gruppoFinalita').value as GruppoFinalita;
        const regolamento = this.esercizioEditForm.get('vincolo')?.value?.regolamento as Regolamenti | null;
        const impresa = new Impresa();
        impresa.partitaIva = this.objParametriAgendaService.getObjParamValue().Piva;
        if (varieta.descrizione.toLowerCase() == 'altre') {
          varieta.codice = 0;
        }
        const leggiProdotti = {
          ...this.leggiProdotti,
          impresa: impresa,
          specie: varieta.specie,
          varieta: varieta,
          finalita: finalita,
          regolamento: regolamento
        } as LeggiProdotti;
        const items = lastValueFrom(this.prodottiService.Leggi_Trasformati_Vegetali_Anagrafica_With_Default(leggiProdotti));
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, items);
        break;
      case 'des':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.appezzamentiService.generateDescriptions(this.objParametriAgenda.Piva))
        );
        break;
      case 'acaContributes':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddlEl,
          lastValueFrom(this.contributeService.read(new Contribute(0, ContributeType.ACA)).pipe(map(r => r.RispostaStringa)))
        );
        break;
    }
  }

  calcolaSuperficieClick() {
    const superficie = this.calcolaSuperficie().superficie;
    let appezzamento: Appezzamento = this.appezzamentoEditForm.getRawValue();
    if (appezzamento.catastoAppezzamento.length > 0) {
      this.giasMessageService.warningMessage(this.translocoService.translate('NonEPossibileModificareSuperficie'));
      return;
    }
    if (superficie > 0) {
      this.impiantoEditForm.controls['superficie'].setValue(superficie);
      this.appezzamentoEditForm.controls['superficie'].setValue(superficie);
      this.giasDialogService.alertMessage("Impostata superficie di " + superficie + "ha", null, 3)
    }
  }

  calcolaPianteClick(): void {
    const pianteCalcolate = this.calcolaPiante();
    if (pianteCalcolate.piante_ha > 0 && pianteCalcolate.piante_impianto > 0) {
      const currentPiante_Ha = this.esercizioEditForm.controls['piante_Ha'].value;
      const currentPiante_Impianto = this.esercizioEditForm.controls['piante_Impianto'].value;

      if (currentPiante_Ha == 0 && currentPiante_Impianto == 0) {
        this.assegnaValoriCalcolaPiante(pianteCalcolate.piante_ha, pianteCalcolate.piante_impianto)
        return;
      }

      if (currentPiante_Ha != pianteCalcolate.piante_ha || currentPiante_Impianto != pianteCalcolate.piante_impianto) {
        this.giasDialogService.dialogMessageObs_Result(
          this.translocoService.translate('Attenzione'),
          this.translocoService.translate('PianteDifferisconoModificare')).pipe(
          take(1),
          tap((res) => {
            if (res['returnObj']) {
              this.assegnaValoriCalcolaPiante(pianteCalcolate.piante_ha, pianteCalcolate.piante_impianto)
            }
          })
        ).pipe(takeUntil(this.signal$)).subscribe();
      }
    }
  }

  specieSelezionata(): boolean {
    const val: UtilizzoTerreno = this.impiantoEditForm.controls['utilizzoTerreno'].value;
    return val.classType == 'Varieta' && (<Varieta>val).specie.codice > 0;
  }

  gruppoRaccoltaChanged(ddlEl: GiasDropDownTemplateComponent | GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    let objParams: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    objParams.Data = new Date();
    UtilityFunctions.loadDropDownItems(
      <GiasDropDownTemplateSComponent>ddlEl,
      lastValueFrom(this.gruppiRaccoltaservice.leggiGruppiRaccoltaValidi(objParams).pipe(take(1)))
    );
  }

  private specieChanged(specie: Specie) {
    this.specie = specie;

    if (this.specie.codice == 0) {
      this.esercizioEditForm.get('piante_Impianto').setValue(0);
      this.esercizioEditForm.get('piante_Ha_Femmine').setValue(0);
      this.esercizioEditForm.get('Piante_Ha_Impianto_Femmine').setValue(0);
      this.esercizioEditForm.get('Piante_Ha_Maschi').setValue(0);
      this.esercizioEditForm.get('Piante_Ha_Impianto_Maschi').setValue(0);

      this.esercizioEditForm.get('data_Semina_Trapianto_Prevista').setValue(AGRODATAINIZIO);
      this.esercizioEditForm.get('data_Raccolta_Prevista').setValue(AGRODATAINIZIO);
      this.esercizioEditForm.get('data_Fioritura_Prevista').setValue(AGRODATAINIZIO);
      this.esercizioEditForm.get('resa_prevista').setValue(0);
      this.esercizioEditForm.get('resa_totale_prevista').setValue(0);

      this.esercizioEditForm.get('regolamento').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('disciplinare').setValue({ codice: '0', descrizione: '' });
      this.esercizioEditForm.get('vincolo').setValue(this.defaultItemVincolo);

      this.esercizioEditForm.get('iaf').setValue([]);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('pianoConcimazione').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('tipologia').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('fase').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('n').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('p2o5').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('k2o').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('mgo').setValue(0);
      this.resetProdotto();

    } else {
      this.esercizioEditForm.get('iaf').setValue([]);
      let vincoloSelected = this.esercizioEditForm.get('vincolo').getRawValue();
      this.leggiVincoli().pipe(take(1)).subscribe((val) => {
        if (vincoloSelected && vincoloSelected.codice) {
          this.esercizioEditForm.get('vincolo').setValue(vincoloSelected);
        } else {
          this.esercizioEditForm.get('vincolo').setValue(val[0]);
        }
      });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('pianoConcimazione').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('tipologia').setValue({ codice: 0, descrizione: '' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('fase').setValue({ codice: 102, descrizione: 'Impianto in Produzione' });
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('n').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('p2o5').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('k2o').setValue(0);
      this.esercizioEditForm.get('apportiMassimiMacroelementi').get('mgo').setValue(0);
      this.resetProdotto();
    }
  }

  private LeggiDisciplinari() {
    let specie: Specie = this.impiantoEditForm.get('utilizzoTerreno').value.specie;
    let lavorazione: Lavorazione = new Lavorazione('0');
    let regolamento: Regolamenti = { codice: 0, descrizione: '' } //this.EsercizioEditForm.get('regolamento').value;
    let leggiDisciplinari: LeggiDisciplinare = {
      regolamento: regolamento,
      specie: specie,
      data: this.esercizioEditForm.get('validita').get('inizio').value,
      lavorazione: lavorazione,
      privato: false
    };
    return this.disciplinariService.leggi(leggiDisciplinari);
  }

  private LeggiIAF() {
    let specie: Specie = this.impiantoEditForm.get('utilizzoTerreno').value.specie;
    let disciplinare: Disciplinare = this.esercizioEditForm.get('disciplinare').value;
    let leggiIAF: LeggiIAF = {
      disciplinare: disciplinare,
      specie: specie,
      data: this.esercizioEditForm.get('validita').get('inizio').value,
      privato: false
    };
    return this.disciplinariService.leggiIAF(leggiIAF);
  }

  private leggiOrganismoReferente() {
    const piva = this.objParametriAgendaService.getObjParamValue().Piva;
    let impresa = new Impresa();
    impresa.partitaIva = piva;
    return this.contattiService.LeggiOrganismiReferenti(impresa);
  }

  private leggiMagazzinoConferimento() {
    //const piva = this.impiantoEditForm.controls["primaryKey"].value.appezzamentoPK.centroAziendalePK.partitaIva;
    let impresa = new Impresa();
    impresa.partitaIva = "";
    return this.contattiService.LeggiMagazzinoConferimento(impresa);
  }

  private leggiRiferimento_Trasferimento_Dati() {
    const piva = this.impiantoEditForm.controls["primaryKey"].value.appezzamentoPK.centroAziendalePK.partitaIva;
    let impresa = new Impresa();
    impresa.partitaIva = piva;
    return this.contattiService.LeggiRiferimento_Trasferimento_Dati(impresa);
  }

  private leggiLicenzaColtivazione() {
    const piva = this.impiantoEditForm.controls["primaryKey"].value.appezzamentoPK.centroAziendalePK.partitaIva;
    return this.tabelleWsClientService.RicercaValoriParametriQualitativi(1330, piva);
  }

  private leggiFinalitaConcimazione() {
    const regolamento: RegolamentoConcimazione = (<FormGroup>this.esercizioEditForm.controls["apportiMassimiMacroelementi"]).controls["pianoConcimazione"].value;
    const utilizzo = this.impiantoEditForm.controls['utilizzoTerreno'].value as Varieta;
    const specie = utilizzo.specie;
    const finalita = this.impiantoEditForm.controls['gruppoFinalita'].value as GruppoFinalita;
    return this.pianoConcimazioneService.leggiRegolamentoConcimazionexImpianto(regolamento, specie, finalita);
  }

  private leggiStatoImpianto() {
    return this.finalitaService.Leggi_FasiCicloColturalexSpecie({
      specie: (<Varieta>this.impiantoEditForm.controls['utilizzoTerreno'].value).specie,
      finalita: <GruppoFinalita>this.impiantoEditForm.controls['gruppoFinalita'].value
    })
  }

  private leggiLavorazione() {
    return this.codificaInfoAggiuntiveService.LeggiCAC_Codifica_InfoAggiuntive({
      Argomento_Cod: enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Lavorazione,
      InfoAgg_Cod: "",
      Tipo_Codifica: 0
    }, true)
  }

  private leggiSpecifica() {
    return this.codificaInfoAggiuntiveService.LeggiCAC_Codifica_InfoAggiuntive({
      Argomento_Cod: enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Specifica,
      InfoAgg_Cod: "",
      Tipo_Codifica: 0
    }, true)
  }

  private leggiModalitaLiquidazione() {
    return this.codificaInfoAggiuntiveService.LeggiCAC_Codifica_InfoAggiuntive({
      Argomento_Cod: enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.ModalitaLiquidazione,
      InfoAgg_Cod: "",
      Tipo_Codifica: 0
    }, false)
  }

  private leggiOrigineProdotto() {
    return this.codificaInfoAggiuntiveService.LeggiCAC_Codifica_InfoAggiuntive({
      Argomento_Cod: enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.OrigineProdotto,
      InfoAgg_Cod: "",
      Tipo_Codifica: 0
    }, false)
  }

  private leggiVincoli() {
    const validita: IntervalloTemporale = new IntervalloTemporale(
      this.esercizioEditForm.controls["validita"].value.inizio,
      this.esercizioEditForm.controls["validita"].value.fine
    );

    return this.vincoliService.leggiVincoli(validita);
  }

  public calculateResaPrevista(event: Event): void {
    let impianto: Impianto = this.impiantoEditForm.getRawValue();
    let esercizio: Esercizio = this.esercizioEditForm.getRawValue();
    let appezzamento = this.appezzamentoEditForm.getRawValue();
    this.readIndirizzoCentroAziendale(appezzamento.primaryKey.centroAziendalePK.codice).pipe(
      switchMap(r => {
        if (impianto.utilizzoTerreno.classType == 'Varieta') {
          let indirizzo: IndirizzoAssociato = r.RispostaStringa[0];

          let params: DatiPrevisionaliColtureRequest = new DatiPrevisionaliColtureRequest();
          params.culCod = new BaseCodeDescr(impianto.utilizzoTerreno.codice, impianto.utilizzoTerreno.descrizione);
          params.vegCod = new BaseCodeDescr((<any>impianto.utilizzoTerreno).specie.codice, (<any>impianto.utilizzoTerreno).specie.descrizione);
          params.foralCod = new BaseCodeDescr(impianto.formaAllevamento.codice);
          params.grfiCod = new BaseCodeDescr(impianto.gruppoFinalita.codice);
          params.grvaCod = new BaseCodeDescr(impianto.gruppoVarietale.codice);
          params.portCod = new BaseCodeDescr(impianto.portinnesto.codice);
          params.reg = new BaseCodeDescrStr(indirizzo?.indirizzo?.istatComune.reg ?? '');
          params.prov = new BaseCodeDescrStr(indirizzo?.indirizzo?.istatComune.com ?? '');
          params.codiceStato = new BaseCodeDescrStr(indirizzo?.indirizzo?.stato.codice ?? '');
          params.regCod = new BaseCodeDescr(esercizio.vincolo.regolamento.codice);
          params.statoCod = new BaseCodeDescr(esercizio.apportiMassimiMacroelementi.fase.codice);
          params.validitaFine = esercizio.validita.fine;
          params.validitaInizio = esercizio.validita.inizio;
          params.parametroCod = new BaseCodeDescr(4);
          params.dettSpeciePersonalizzatoCod = new BaseCodeDescrStr(impianto.dettaglio_varieta_personalizzato.codice);

          return this.impiantiService.readDatiPrevisionaliColture(params);
        }
        return of(undefined);
      })
    ).subscribe(r => {
      if (r !== undefined) {
        if (!r.RispostaOK) {
          this.giasDialogService.baseError('appezzamento.datiPrevisione', r.Errore);
        } else {
          if (r.RispostaStringa.udm.codice !== 0) {
            this.esercizioEditForm.controls['resa_prevista'].setValue(this.unitaMisuraService.Converti(<UnitaDiMisura>r.RispostaStringa.udm, r.RispostaStringa.valore, new UnitaDiMisura(enum_UnitaMisura.KG__HA, '')) ?? r.RispostaStringa.valore);
          } else {
            this.esercizioEditForm.controls['resa_prevista'].setValue(r.RispostaStringa.valore);
          }
        }
      }
    });
  }

  private readIndirizzoCentroAziendale(sa_cod: number): Observable<rispostaStandard<IndirizzoAssociato[]>> {
    let indirizzo: IndirizzoAssociato;
    if (this.appezzamentoEditForm.getRawValue()?.indirizzi?.length > 0) {
      indirizzo = this.appezzamentoEditForm.getRawValue()?.indirizzi[0];
      return this.istatService.readRegionsByProvince(indirizzo.indirizzo.istatComune.prov).pipe(
        switchMap(r => {
          indirizzo.indirizzo.istatComune.reg = r?.codice ?? '000';
          let resp: rispostaStandard<IndirizzoAssociato[]> = new rispostaStandard<IndirizzoAssociato[]>();
          resp.RispostaStringa = [indirizzo];
          return of(resp);
        })
      );
    } else {
      let params: LeggiIndirizziCentro = new LeggiIndirizziCentro(this.objParametriAgenda.Piva, sa_cod);
      return this.centriService.readCentreAddresses(params);
    }
  }

  private assegnaValoriCalcolaPiante(piante_ha, piante_impianto) {
    this.esercizioEditForm.controls['piante_Ha'].setValue(piante_ha, { emitEvent: false });
    this.esercizioEditForm.controls['piante_Impianto'].setValue(piante_impianto, { emitEvent: false });
  }

  private calcolaPiante(): { piante_impianto: number, piante_ha: number } {
    const distanza_suFila = this.impiantoEditForm.get('su_Fila_M').value;
    const distanza_traFila = this.impiantoEditForm.get('tra_Fila_M').value;
    const interbina = this.impiantoEditForm.get('interbina').value;
    const germinabilita = this.impiantoEditForm.get('germinabilita').value;
    const superficie = this.impiantoEditForm.get('superficie').value;

    let flag = true;
    // Controllo se i campi richiesti sono stati riempiti
    if (distanza_suFila == 0 || distanza_traFila == 0) {
      this.giasMessageService.warningMessage(this.translocoService.translate('NecessarioCompilareICampi'));
      flag = false;
    }

    if (flag) {

      let denominatore;

      if (interbina > 0) {
        denominatore = Math.abs(distanza_traFila - interbina) * distanza_suFila;
      } else {
        denominatore = distanza_suFila * distanza_traFila;
      }

      let PianteHa: number;
      if (germinabilita != 0 && germinabilita != undefined) {
        PianteHa = 10000 / denominatore * (germinabilita / 100);
      } else {
        PianteHa = 10000 / denominatore;
      }

      if (PianteHa == Infinity) {
        this.impiantoEditForm.get('piante_Ha')?.setValue(null);
        this.impiantoEditForm.get('piante_Impianto')?.setValue(null);
        return;
      }
      const PianteImpianto = PianteHa * superficie;

      this.impiantoEditForm.get('piante_Ha')?.setValue(parseFloat(PianteHa.toFixed(0)));
      this.impiantoEditForm.get('piante_Impianto')?.setValue(parseFloat(PianteImpianto.toFixed(0)));
      return {
        piante_ha: parseFloat(PianteHa.toFixed(0)),
        piante_impianto: parseFloat(PianteImpianto.toFixed(0))
      }
    } else {
      this.impiantoEditForm.get('piante_Ha')?.setValue(null);
      this.impiantoEditForm.get('piante_Impianto')?.setValue(null);
      return {
        piante_ha: 0,
        piante_impianto: 0
      };
    }
  }

  private calcolaSuperficie(): { superficie: number } {
    const distanza_suFila = this.impiantoEditForm.get('su_Fila_M').value;
    const distanza_traFila = this.impiantoEditForm.get('tra_Fila_M').value;
    const interbina = this.impiantoEditForm.get('interbina').value;
    const germinabilita = this.impiantoEditForm.get('germinabilita').value;
    const numeroPiante = this.esercizioEditForm.get('piante_Impianto').value;

    let flag = true;
    // Controllo se i campi richiesti sono stati riempiti
    if (distanza_suFila == 0 || distanza_traFila == 0) {
      this.giasMessageService.warningMessage(this.translocoService.translate('NecessarioCompilareICampixPiante'));
      flag = false;
    }

    if (numeroPiante == undefined || numeroPiante == 0) {
      this.giasMessageService.warningMessage("impostare correttamente numero piante");
      flag = false;
    }

    if (flag) {

      let denominatore;

      if (interbina > 0) {
        denominatore = Math.abs(distanza_traFila - interbina) * distanza_suFila;
      } else {
        denominatore = distanza_suFila * distanza_traFila;
      }

      let PianteHa: number;
      if (germinabilita != 0 && germinabilita != undefined && germinabilita != null) {
        PianteHa = 10000 / denominatore * (germinabilita / 100);
      } else {
        PianteHa = 10000 / denominatore;
      }

      if (PianteHa == Infinity) {
        this.esercizioEditForm.get('piante_Ha').setValue(null);
        this.esercizioEditForm.get('piante_Impianto').setValue(null);
        return;
      }
      const superficie = numeroPiante / PianteHa;

      this.esercizioEditForm.get('piante_Ha').setValue(parseFloat(PianteHa.toFixed(0)));
      this.esercizioEditForm.get('piante_Impianto').setValue(parseFloat(numeroPiante.toFixed(0)));
      return {
        superficie: parseFloat(superficie.toFixed(4))
      }
    } else {
      return {
        superficie: 0
      }
    }
  }

  private resetProdotto(): void {
    this.esercizioEditForm.get('prodotto').setValue({ codice: 0, descrizione: '' });
  }

  public disableLotto(): boolean {
    return !!this.esercizioEditForm && this.impiantoEditForm?.controls['algoritmoCodifica']?.value !== '';
  }

  private updateResaTotalePrevista(): void {
    let resa = this.esercizioEditForm.controls['resa_prevista'].value ?? 0;
    let sup = this.impiantoEditForm.controls['superficie'].value ?? 0;
    this.esercizioEditForm.controls['resa_totale_prevista'].setValue(resa * sup);
  }

  private setResaTotalePrevistaSubs(): void {
    let resa = this.esercizioEditForm.controls['resa_prevista'];
    let sup = this.impiantoEditForm.controls['superficie'];

    resa.valueChanges.pipe(takeUntil(this.signal$), startWith(0)).subscribe(this.updateResaTotalePrevista.bind(this));
    sup.valueChanges.pipe(takeUntil(this.signal$), startWith(0)).subscribe(this.updateResaTotalePrevista.bind(this));
  }

  private impostaNPK() {
    this.calcoloNPKModello({
      regolamento: <RegolamentoConcimazione>(<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['pianoConcimazione'].value,
      specie: (<Varieta>this.impiantoEditForm.controls['utilizzoTerreno'].value).specie,
      finalita: <FinalitaPianoConcimazione>(<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['tipologia'].value,
      stato: <FaseCicloColturale>(<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['fase'].value
    }).pipe(
      takeUntil(this.signal$)
    ).subscribe(val => {
      if (val.n != undefined) {
        (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['n'].setValue(val.n);
      }
      if (val.p2o5 != undefined) {
        (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['p2o5'].setValue(val.p2o5);
      }
      if (val.k2o != undefined) {
        (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['k2o'].setValue(val.k2o);
      }
      if (val.mgo != undefined) {
        (<FormGroup>this.esercizioEditForm.controls['apportiMassimiMacroelementi']).controls['mgo'].setValue(val.mgo);
      }
    });
  }

  private onUtilizzoTerrenoChange(val: UtilizzoTerreno) {
    if (this.lastUtilizzoTerreno == null) {
      this.lastUtilizzoTerreno = val;
      return;
    }
    if (this.lastUtilizzoTerreno.classType != val.classType) {
      if (val.classType == 'Varieta') {
        this.specieChanged((<Varieta>val).specie);
      } else {
        this.specieChanged({ codice: 0, descrizione: '' });
      }
    } else if (val.classType == 'Varieta') {
      if ((<Varieta>this.lastUtilizzoTerreno).codice != (<Varieta>val).codice) {
        this.resetProdotto();
      }

      if ((<Varieta>this.lastUtilizzoTerreno)?.specie?.codice != (<Varieta>val)?.specie?.codice) {
        this.specieChanged((<Varieta>val).specie);
      }
    }
    this.lastUtilizzoTerreno = val;
  }
}
