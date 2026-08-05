import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import { Campo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { Location } from '@angular/common';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import {CentriAziendaliService, LeggiCentriAziendali} from 'app/Service/Anagrafica/centri.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { BIO_Dati_OrientamentoProduttivoService } from 'app/Service/Metaschema/BIO_Dati_OrientamentoProduttivo.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CODICI_TOKEN } from 'app/Utility/Template/codici-template/models/codici.model';
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { from, lastValueFrom, Subject, Subscription, take, takeUntil, tap } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from '../../../Model/CostantiPersonalizzate';
import { MasterService, RispostaStandard } from '../../../Service/master.service';
import { ObjParametriAgendaService } from '../../../Service/obj-parametri-agenda.service';
import { CampiEditService } from './campi-edit.service';
import { Form_CampiEdit_Service } from './form-campi-edit.service';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { AppezzamentoCampoService } from './appezzamento-campo-edit/appezzamento-campo.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import {MenuContestualeService} from '../../../Master/menu-contestuale/menu-contestuale.service';
import { contestoPostMessage, messaggioPostMessage, PostMessageStrutturata} from 'gias-ui-kit';
import { CampiEditDataService } from './campi-edit-data.service';
import {IntervalloTemporale} from '../../../Model/anagrafiche/IntervalloTemporale';
import { RadioButtonValue } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-campi-edit',
  templateUrl: './campi-edit.component.html',
  styleUrls: ['./campi-edit.component.css'],
  providers: [
    GiasDropDownTemplateService,
    { provide: CODICI_TOKEN, useClass: CampiEditService },
    AppezzamentoCampoService,
    CampiEditService,
    CampiEditDataService
  ],
})
export class CampiEditComponent implements OnInit, OnDestroy {
  protected saving = false;
  protected campoSelezionato: boolean;
  protected datiCaricati: boolean = false;
  protected objParametriAgenda: ObjParametriAgenda;

  protected binaryButtonValue: boolean;
  protected binaryButtonName: string;

  protected lista_Centri: Array<{ codice: number; descrizione: string }>;
  protected SpecieVegetali: Array<Specie>;
  private campo: Campo;
  private cartografia: string = '';
  private inFrame: boolean;
  protected salvaNuovo: boolean = true;

  private formCaricaDati: Subscription;
  private valueChangesSub: Subscription;

  public signal$: Subject<void> = new Subject();

  protected campiEditForm: FormGroup = this.fb.group({
    primaryKey: this.fb.group({
      codice: 0,
      centroAziendalePK: this.fb.group({
        codice: 0,
        partitaIva: ''
      })
    }),
    codice: [0],
    campo_Codice: [0],
    descrizione: [''],
    orientamento_Colturale: [0],
    specie: [{
      codice: -1,
      descrizione: ''
    }],
    serra: [false],
    catastoCampo: this.fb.array([]),
    appezzamentoCampo: this.fb.array([]),
    codici: this.fb.array([]),
    validita: this.fb.group({
      inizio: [AGRODATAINIZIO],
      fine: [AGRODATAFINE]
    }),
    cartografia: ['']
  });

  protected OrientamentoColturale: Array<RadioButtonValue> = [
    { name: 'Non Impostato', value: 0, enable:true },
    { name: 'Mono-Specie', value: 3, enable: true },
    { name: 'Multi-Specie', value: 1, enable: true }
  ];

  protected Array_Esposizione: Array<{descrizione: string;codice: string}>=[
    {descrizione:'nord',codice:'nord'},
    {descrizione:'nord-est',codice:'nord-est'},
    {descrizione:'est',codice:'est'},
    {descrizione:'sud-est',codice:'sud-est'},
    {descrizione:'sud-ovest',codice:'sud-ovest'},
    {descrizione:'ovest',codice:'ovest'},
    {descrizione:'nord-ovest',codice:'nord-ovest'}
  ];

  protected Array_Ubicazione: Array<{descrizione: string;codice: string}>=[
    {descrizione:'Pianura',codice:'pianura'},
    {descrizione:'Mezza Costa',codice:'mezza costa'},
    {descrizione:'Collina',codice:'collina'},
    {descrizione:'Montagna',codice:'montagna'}
  ];

  constructor(
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private masterService: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    @Inject(CODICI_TOKEN) private campiEditService: CampiEditService,
    private formCampiEditService: Form_CampiEdit_Service,
    private funzioniComuniService: FunzioniComuniService,
    private fb: FormBuilder,
    private router: Router,
    private location: Location,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private specievegetaliservice: SpecieVegetaliService,
    private bio_Dati_OrientamentoProduttivoService: BIO_Dati_OrientamentoProduttivoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private centriService: CentriAziendaliService,
    private translocoService: TranslocoService,
    private route: ActivatedRoute,
    private menuContestualeService: MenuContestualeService,
    private campiEditDataService: CampiEditDataService
  ) {
    this.campiEditForm.get('primaryKey.centroAziendalePK').valueChanges.pipe(takeUntil(this.signal$)).subscribe(v => {
      this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
      this.objParametriAgenda.Sa_Cod = v.codice;
      this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    });

    this.campiEditForm.controls['catastoCampo'].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe(c => {
      this.campiEditDataService.catastoCampo = c;
    });

    this.campiEditDataService.catastoCampo$.pipe(
      takeUntil(this.signal$)
    ).subscribe(cc => {
      this.campiEditForm.value.catastoCampo = [];
      this.campiEditForm.value.catastoCampo = cc;
    });
  }

  async ngOnInit() {
    this.saving = false;
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.valueChangesSub = this.campiEditForm.valueChanges.subscribe(form => {
      this.campiEditService.updateForm(form);
    });

    await this.loadSpecies();

    this.handleQueryParams();
    this.initDati();

    if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
      let impresa = new Impresa();
      impresa.partitaIva = this.objParametriAgenda.Piva;

      this.centriService.leggiCentriAziendaliModelloQdC(
        <LeggiCentriAziendali> {
          impresa: impresa,
          data: (<FormGroup>this.campiEditForm.controls["validita"]).value.inizio
        }, false
      ).then(vals => {
        this.lista_Centri = vals.map(el => ({ codice: el.primaryKey.codice, descrizione: el.nome }));
        this.campiEditForm.get('primaryKey').get('centroAziendalePK').get("codice").setValue(this.lista_Centri[0].codice);
      });
    }
  }

  private async loadSpecies() {
    const PromisesResp = await Promise.all([
      this.campiService.leggi_SpecieVegetali()
    ]);
    this.SpecieVegetali = PromisesResp[0];

    if (!this.SpecieVegetali.some(sv => sv.codice === 0)) {
      this.SpecieVegetali.push({codice: 0, descrizione: this.translocoService.translate('NessunaColtura')});
    }
  }

  private initDati() {
    const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if (<number>agenda.TipoOperazioneDB != <number>enum_TipoOperazioneDB.Scrittura) {
      this.formCaricaDati = this.campiEditService.caricaDati().subscribe((data) => {
        this.objParametriAgenda = data.ObjParamAgenda;
        this.campo = data.Campo;

        // la cartografia se presente arriva in query string dalla pagina del GIS
        if(this.cartografia != '') {
          this.campo.cartografia = this.cartografia;
        }

        this.initForm(this.campo);
        this.abilitaDisabilitaForm();
        this.setSwitch();
        this.enableSpecieDdl();
        this.datiCaricati = true;
      });
    } else {
      this.objParametriAgenda = agenda;
      this.campo = this.creaCampoVuoto(agenda);

      // la cartografia se presente arriva in query string dalla pagina del GIS
      if(this.cartografia != '') {
        this.campo.cartografia = this.cartografia;
      }

      this.initForm(this.campo);
      this.abilitaDisabilitaForm();
      this.setSwitch();
      this.enableSpecieDdl();
      this.datiCaricati = true;
    }

    this.campoSelezionato = true;
  }

  private creaCampoVuoto(agenda: ObjParametriAgenda): Campo {
    let primaryKeyCentro = new CentroAziendale.PK(agenda.Sa_Cod, agenda.Piva);
    let primaryKeyCampo = new Campo.PK(
      0,
      primaryKeyCentro
    );

    let c = new Campo(primaryKeyCampo);
    c.campo_Codice = '';
    c.descrizione = '';
    c.orientamento_Colturale = 0;
    c.specie = {
      codice: -1,
      descrizione: 'SELEZIONARE'
    };
    c.serra = false;
    c.catastoCampo = [];
    c.appezzamentoCampo = [];
    c.codici = [];
    c.validita = new IntervalloTemporale();

    return c;
  }

  abilitaDisabilitaForm() {
    const operazione = this.objParametriAgenda.TipoOperazioneDB;
    if (operazione == Enum_DBTypeOperation.Read) {
      this.campiEditForm.disable();
    } else if (operazione == Enum_DBTypeOperation.Update) {
      this.campiEditForm.enable();
    } else if (operazione == Enum_DBTypeOperation.Write) {
      this.campiEditForm.enable();
    }
  }

  private setSwitch(): void {
    this.binaryButtonName = 'KendoBinaryButtonName';
    this.binaryButtonValue = (this.campo.catastoCampo != undefined && this.campo.catastoCampo.length > 0);
  }

  private initForm(campo: Campo) {
    this.campiEditForm.patchValue(campo);

    if (campo.codici != undefined) {
      for (const codice of campo.codici) {
        const codiceForm = this.formCampiEditService.getFormCodiciCampo();
        codiceForm.patchValue(<any>codice, {emitEvent: false});
        const faCodiciCampo = <FormArray>this.campiEditForm.controls['codici'];
        faCodiciCampo.push(codiceForm);
      }
      this.campiEditForm.value.codici = campo.codici;
    }

    if (campo.appezzamentoCampo != undefined) {
      for (const appezzamento of campo.appezzamentoCampo) {
        const appezzamentoForm = this.formCampiEditService.getFormAppezzamentoCampo();
        appezzamentoForm.patchValue(<any>appezzamento, {emitEvent: false});
        const faAppezzamentiCampo = <FormArray>this.campiEditForm.controls['appezzamentoCampo'];
        faAppezzamentiCampo.push(appezzamentoForm);
      }

      this.campiEditForm.value.appezzamentoCampo = campo.appezzamentoCampo;
    }

    if(campo.catastoCampo != undefined) {
      for (const particella of campo.catastoCampo) {
        const particellaForm = this.formCampiEditService.getFormCatastoCampo();
        particellaForm.patchValue(particella, {emitEvent: false});
        const faCatastoCampo = <FormArray>this.campiEditForm.controls['catastoCampo'];
        faCatastoCampo.push(particellaForm);
      }

      this.campiEditForm.value.catastoCampo = campo.catastoCampo;
    }

  }

  onChangeChkConCatasto(event: boolean) {
    this.binaryButtonValue = event;

    if (!event) {
      this.campiEditForm.controls['catastoCampo'] = new FormArray([]);
    }
  }

  protected enableSpecieDdl() {
    if (!this.campiEditForm.enabled) {
      return;
    }
    let orientamentoValue = this.campiEditForm.controls['orientamento_Colturale'].value;
    let nessunaColt = this.SpecieVegetali.find(s => s.codice == 0);
    if (orientamentoValue == undefined || orientamentoValue == 0 || orientamentoValue == 1) {
      this.campiEditForm.controls['specie'].setValue({
        codice: nessunaColt?.codice ?? 0,
        descrizione: nessunaColt?.descrizione ?? this.translocoService.translate('NessunaColtura')
      });
      this.campiEditForm.controls['specie'].updateValueAndValidity();
      this.campiEditForm.controls['specie'].disable();
    } else {
      this.campiEditForm.controls['specie'].enable();
    }
  }

  SalvaCampo() {
    this.saving = true;
    from(this.onSubmit()).pipe(
      take(1),
      tap((resp: RispostaStandard) => {
        this.saving = false;
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('CampoCorrettamenteModificato'));
          this.masterService.set_isLoading({ message: '', isLoading: false });
          this.handleRedirect(resp.RispostaStringa as any as Campo);
        } else {
          if (resp.ErroriGias.length > 0) {
            resp.ErroriGias.forEach((el) => {
              this.giasDialogService.baseError('Errore', el.messaggio, false);
            })
          } else {
            this.giasDialogService.baseError('Errore', resp.Errore, false);
          }
        }
      })
    ).subscribe();
  }

  SalvaNuovoCampo() {
    this.saving = true;
    from(this.onSubmit()).pipe(take(1)).subscribe(resp => {
      this.saving = false;
      this.masterService.set_isLoading({ isLoading: false, message: '' });
      if (resp.RispostaOK) {
        this.giasMessageService.successMessage(this.translocoService.translate('CampoCorrettamenteCreato'));
        let objParametriAgenda = this.objParametriAgendaService.getObjParamValue()
        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        objParametriAgenda.Campo_Cod = 0;
        let currentUrl = this.router.url;
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.onSameUrlNavigation = 'reload';
        this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);
        this.router.navigate([currentUrl]);
      } else {
        this.giasMessageService.errorMessage(this.translocoService.translate('ErroreSalvataggio') + resp);
      }
    });
  }

  onSubmit(): Promise<RispostaStandard> {
    console.log('onSubmit:')
    console.log(this.campiEditForm.value)
    if (!this.funzioniComuniService.codiciSovrapposti(this.campiEditService.codici)) {
      if (this.campiEditForm.valid && (this.funzioniComuniService.codiciValidi(this.campiEditService.codici).length == 0)) {
        this.campiEditService.updateForm(this.campiEditForm.getRawValue());
        this.campiEditForm.value.codici = this.campiEditService.codici;

        this.campiEditService.postForm();
      }

      if (this.campiEditForm.valid) {

        this.masterService.set_isLoading({ isLoading: true, message: this.translocoService.translate('Caricamento') });

        let campo: Campo = this.createCampo_perSalvataggio(this.campiEditForm);

        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write){
          this.setPrimaryKey();
          return (this.campiService.aggiornaCampo(campo, enum_TipoOperazioneDB.Scrittura));
        } else {
          return (this.campiService.aggiornaCampo(campo, enum_TipoOperazioneDB.Modifica));
        }

      } else {
        this.campiEditForm.markAllAsTouched();
        let els = document.getElementsByClassName('ng-invalid')
      }
    }
  }

  private setPrimaryKey() {
    this.campiEditForm.value.primaryKey = {codice: 0,
      impresaPK: { codice: 0,
        partitaIva: this.objParametriAgenda.Piva
      }
    };
  }

  private createCampo_perSalvataggio(campoEditForm: FormGroup): Campo {
    let campo: Campo = new Campo(campoEditForm.controls['primaryKey'].value);
    campo.appezzamentoCampo = campoEditForm.value.appezzamentoCampo;
    campo.catastoCampo = campoEditForm.value.catastoCampo;
    campo.campo_Codice = campoEditForm.controls['campo_Codice'].value;
    campo.codici = campoEditForm.value.codici;
    campo.descrizione = campoEditForm.value.descrizione;
    campo.orientamento_Colturale = campoEditForm.controls['orientamento_Colturale'].value;
    campo.serra = campoEditForm.controls['serra'].value;
    campo.specie = campoEditForm.controls['specie'].value;
    campo.validita = campoEditForm.controls['validita'].value;
    campo.cartografia = campoEditForm.controls['cartografia'].value;

    return campo;
  }

  ngOnDestroy(): void {
    if (this.formCaricaDati != undefined) {
      this.formCaricaDati.unsubscribe();
    }
    if(this.valueChangesSub != undefined) {
      this.valueChangesSub.unsubscribe();
    }

    this.signal$.next();
    this.signal$.complete();
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

  private handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntil(this.signal$))
      .subscribe(params => {
        if (params.seFrame == 1) {
          let header = this.masterService.getHeader();
          header.visible = false;
          this.masterService.changeHeader(header);

          let footer = this.masterService.getFooter();
          footer.visible = false;
          this.masterService.changeFooter(footer);

          let menuContestuale = this.menuContestualeService.getMenuContestualeSettings();
          menuContestuale.show = false;
          this.menuContestualeService.changeMenuContestualeSettings(menuContestuale);

          this.inFrame = true;
          this.salvaNuovo = false;
        }
        if(params.wkt != undefined && params.wkt != '') {
          this.cartografia = params.wkt;
        }
      });
  }

  private handleRedirect(campo: Campo): void{
    if (this.inFrame) {
      let objPostMessage: PostMessageStrutturata<Campo> = new PostMessageStrutturata<Campo>(
        messaggioPostMessage.chiudiWindowGiasNG,
        contestoPostMessage.SalvaCampo,
        campo
      );
      window.parent.postMessage(objPostMessage);
      // console.log('postMessage effettuato',objPostMessage);
    } else {
      this.location.back();
    }
  }
}
