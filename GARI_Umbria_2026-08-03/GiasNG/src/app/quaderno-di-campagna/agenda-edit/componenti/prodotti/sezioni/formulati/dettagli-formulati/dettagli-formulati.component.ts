import { Component, DoCheck, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { QdCProdottiService } from '../../../../../service/prodotti.service';
import { TranslocoService } from '@jsverse/transloco';
import {QdCService, Redirect_To_GiasNG_Page} from '../../../../../service/qdc.service';
import {MultiColumnComboboxService, rispostaStandard} from 'gias-ui-kit';
import { FunzioniComuniService } from '../../../../../../../Service/FunzioniComuni.service';
import { GestioneRichiesteService, } from '../../../../../../../Service/gestione-richieste.service';
import {
  enum_ErroreGias_Tipo,
  ErroreGias,
  ErroreGias_Severity,
  MasterService
} from '../../../../../../../Service/master.service';
import { QdCFormulatiService } from '../../../../../service/prodotti/formulati.service';
import { QdCFormToAttivitaService } from '../../../../../service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service';
import { ObjParametriAgendaService } from '../../../../../../../Service/obj-parametri-agenda.service';
import { GiasDropDownTemplateService, ImpiantiAgendaNG, ObjParametriAgenda } from 'gias-ui-kit';
import {map, pairwise, skip, startWith, Subscription} from 'rxjs';
import { Lavorazione } from '../../../../../../../Model/attivita/Lavorazione';
import { ColumnCombobox } from 'gias-ui-kit';
import { CELL_TYPES } from 'gias-ui-kit';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { UtilityFunctions } from '../../../../../../../Utility/UtilityFunctions';
import { DpiBio, FORMULATI, NessunDpi, NessunDpiNessunaEtichetta } from '../../../../../../../Model/CostantiPersonalizzate';
import {
  enum_LAVCOD, enum_PagineGiasNG,
  enum_TipoOperazioneDB
} from '../../../../../../../Model/TipiEnumerativi';
import { Attivita } from '../../../../../../../Model/attivita/Attivita';
import {
  DettaglioTrattamento,
  enum_Ripartizione_Trappole
} from '../../../../../../../Model/attivita/dettagli/DettaglioTrattamento';
import { QdCUnitadiMisuraService } from '../../../../../service/unita-di-misura.service';
import { GridDosiProdottiControlliService } from '../../../../../service/grid-dosi-prodotti/grid-dosi-prodotti-controlli.service';
import { QdCDettagliFormulatiService } from '../../../../../service/prodotti/dettagli-formulati.service';
import { Epoca } from '../../../../../../../Model/metaschema/Epoca';
import { DropdownListDisciplinare, GridImpiantoSelezionatoModel, MultiColumnComboboxDose_Etichetta, Sezione_Rilievi } from '../../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import { Gias2010Redirector } from '../../../../../../../menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import { DettaglioRilievo } from '../../../../../../../Model/attivita/dettagli/DettaglioRilievo';
import { EsercizioRilievoCDC } from '../../../../../../../Model/attivita/centri_di_costo/EsercizioRilievoCDC';
import { GiasWindowsService } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { RibaltamentoTypes } from "../../../../../../../menu-agenda/components/utils";
import {GridDosiProdottiService} from "../../../../../service/grid-dosi-prodotti/grid-dosi-prodotti.service";
import {cloneDeep} from "lodash";
import {BaseCodeDescr} from "../../../../../../../Model/baseClass/baseCodeDescr";
import { Soglia } from 'app/Model/metaschema/Soglia';
import {AvversitaGruppo} from "../../../../../../../Model/metaschema/avversita/AvversitaGruppo";
import {CentroAziendale} from "../../../../../../../Model/anagrafiche/CentroAziendale";
import {AgendaService, Leggi_Numero_Trappole} from "../../../../../../../Service/Agenda/Agenda.service";
import {EsercizioCDC} from "../../../../../../../Model/attivita/centri_di_costo/EsercizioCDC";
import {RisorsaProdotto} from "../../../../../../../Model/attivita/risorse/RisorsaProdotto";
import { AvversitaTrappole } from 'app/Model/attivita/dettagli/AvversitaTrappole';

@Component({
    standalone: false,
    selector: 'app-dettagli-formulati',
    templateUrl: './dettagli-formulati.component.html',
    styleUrls: ['./dettagli-formulati.component.scss'],
    providers: [QdCProdottiService, QdCDettagliFormulatiService, QdCUnitadiMisuraService, GiasDropDownTemplateService, MultiColumnComboboxService, GridDosiProdottiControlliService]
})
export class DettagliFormulatiComponent implements OnInit, DoCheck, OnDestroy {

    Subs: Subscription = new Subscription();

    public img_profitosan = "";

    Operazione: Lavorazione;
    Categoria_Magazzino: number;
    FormulatiForm: FormGroup;

    public ColumnComboboxDose_Etichetta: Array<ColumnCombobox> = [];

    @ViewChild('trattamentorilievo') trattamentorilievo: TemplateRef<any>;
    private isWindowOpen = false;

    constructor(
        public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        private translocoService: TranslocoService,
        public qdcservice: QdCService,
        private multicolumncomboboxservice: MultiColumnComboboxService,
        private funzionicomuniservice: FunzioniComuniService,
        private gestionerichiesteservice: GestioneRichiesteService,
        private giasRedirector: Gias2010Redirector,
        private windowService: GiasWindowsService,
        private masterService: MasterService,
        private transloco: TranslocoService,
        public qdcdettagliformulatiservice: QdCDettagliFormulatiService,
        private qdcformulatiservice: QdCFormulatiService,
        private qdCFormToAttivitaService: QdCFormToAttivitaService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private ddlService: GiasDropDownTemplateService,
        private gridDosiProdottiService: GridDosiProdottiService,
        private agendaService: AgendaService
    ) { }

    async ngOnInit() {

        this.img_profitosan = this.masterService.link_GiasBase + "/agronica/AB_Immagini/Icone24/profitosan.png";
        this.FormulatiForm = <FormGroup>this.parent.form;
        this.prodottiservice.ObsProdottiForm.next(this.FormulatiForm);

        this.Operazione = <Lavorazione>(
            this.FormulatiForm.get("Operazione").value
        );

        this.Categoria_Magazzino = this.FormulatiForm.get("Categoria_Magazzino").value;

        //Triggero il validator del formgroup
        this.FormulatiForm.markAllAsTouched();
        this.setColumnComboboxDose_Etichetta();
        this.prodottiservice.GestioneMagazzino_Abilitata();
        this.prodottiservice.GestioneGiacenze();
        this.prodottiservice.GestioneLotti();
        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();
        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()) {
            //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
            this.qdcdettagliformulatiservice.setArrayDDLSezioneProdottiFormulatiForm(this.FormulatiForm.getRawValue(), true);

            //Gestisce i messaggi e bottoni che sono da mostrare sempre anche quando è già stato salvato il prodotto
            await this.qdcdettagliformulatiservice.MostraMessaggiBottoniSenzaGridDosiProdottiFormulatiForm();
        }


        this.Subs.add(this.multicolumncomboboxservice.currentMultiColumnComboboxValueObject.pipe(skip(1)).subscribe(async ddlElem => {
            switch (ddlElem.FormControlName) {
                case 'Dose_Etichetta':
                    await this.qdcdettagliformulatiservice.changeDoseEtichetta(ddlElem.Value);
                    break;
            }
        }));

        this.Subs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem => {
            switch (ddlElem.FormControlName) {
                case 'Soglia_Avversita':
                    this.qdcdettagliformulatiservice.changeSoglia(ddlElem.Value);
                    break;
            }
        }));

        this.Subs.add(
            this.qdcservice.TestataForm.get("Disciplinare")?.valueChanges.subscribe(async (d: DropdownListDisciplinare) => {

                //Commentato per richiesta del CAI di non pulire i valori impostati quando cambia il disciplinare
                // (che può variare in base agli impianti selezionati)

                if (this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
                    //this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false,true,true,true,false);

                    await this.qdcdettagliformulatiservice.getArray_Avversita();
                } else {
                    //this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false,true,true,true,true);
                }

                this.qdcdettagliformulatiservice.setColumnComboboxFormulati();

                //Se viene cambiato Disciplinare in BIO,NessunDPi,NessunDPIEtichetta pulisco le Soglie avversita
                if (d &&
                    (d.codice === NessunDpiNessunaEtichetta ||
                        d.codice === NessunDpi ||
                        d.codice === DpiBio)) {

                    this.qdcdettagliformulatiservice.clearSoglia_Avversita();

                }

            })
        );

        this.Subs.add(
            this.qdcservice.TestataForm.get("Data").valueChanges.subscribe(d => {

                if (this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
                    this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false, true, false, true, false);
                } else {
                    this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false, true, false, true, true);
                }

            })
        );

        this.Subs.add(
            this.qdcservice.TestataForm.get("Centro_Aziendale").valueChanges.subscribe(d => {

                if (this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
                    this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false, true, false, true, false);
                } else {
                    this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false, true, false, true, true);
                }

            })
        );

        //Richiamo la funzione di change solo se l'Epoca ha un valore diverso dal precedente
        this.Subs.add(this.FormulatiForm.get("EpocaDPI")?.valueChanges.pipe(
            startWith(this.FormulatiForm.get("EpocaDPI").value),
            pairwise()).subscribe(([prev, next]: [Epoca, Epoca]) => {

                if (next?.codice !== prev?.codice) {
                    this.qdcdettagliformulatiservice.changeEpocaDPI(next);
                }
            })
        );

        this.Subs.add(this.FormulatiForm.get("Ripartizione_Trappole")?.valueChanges.subscribe((value: BaseCodeDescr)=>{
          this.qdcdettagliformulatiservice.AbilitaDisabilitaDoseHaDoseTot();
        }));

        this.qdcdettagliformulatiservice.AbilitaDisabilitaDoseHaDoseTot();

        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi())
            await this.qdcdettagliformulatiservice.componiLblDosiCompatibili(true);

        this.Subs.add(this.FormulatiForm.get("Dose_Etichetta")?.valueChanges.subscribe(async (d: MultiColumnComboboxDose_Etichetta) => {
            if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && d)
                await this.qdcdettagliformulatiservice.componiLblDosiCompatibili(false);
        }));

        await this.qdcdettagliformulatiservice.Carica_Controlli_Ribaltamento_in_Agenda_Di_Ricetta_da_APP_Dettagli_Formulati();

        //Carico le ddl perchè ci potrebbero essere dei casi in cui l'utente non ha specificato alcuni dati in
        //inserimento perchè nn erano obbligatori (esempio Soglia) ma comunque in modifica/ribaltamento la vuole visualizzare
        if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Update ||
            this.qdcservice.TipoRibaltamento !== RibaltamentoTypes.Nessuno) {
            await this.qdcdettagliformulatiservice.Carica_DDL_Formulati(false);
        }
    }


    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    setColumnComboboxDose_Etichetta(): void {
        this.ColumnComboboxDose_Etichetta.push(
            new ColumnCombobox({
                field: "DoseMin",
                title: this.translocoService.translate('DoseMinima')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "DoseMax",
                title: this.translocoService.translate('DoseMassima')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "Udm.simbolo",
                title: this.translocoService.translate('UnitàDiMisura'),
            }),
            new ColumnCombobox({
                field: "AcquaMin",
                title: this.translocoService.translate('VolAcquaMinima')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "AcquaMax",
                title: this.translocoService.translate('VolAcquaMassima')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "UdmAcqua.simbolo",
                title: this.translocoService.translate('Unita_Misura_Acqua'),
            }),
            new ColumnCombobox({
                field: "Limite",
                title: this.translocoService.translate('Limite')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "UdmLimite.simbolo",
                title: this.translocoService.translate('TipoLimite')
            }),
            new ColumnCombobox({
                field: "Flag_Fioritura.descrizione",
                title: this.translocoService.translate('Tipo')
            }),
            new ColumnCombobox({
                field: "IntervalloTrattamenti_Min",
                title: this.translocoService.translate('IntervalloMinimoTrattamento')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "IntervalloTrattamenti_Max",
                title: this.translocoService.translate('IntervalloPrecedenteTrattamento')
            },
                {
                    type: CELL_TYPES.NUMBER,
                    formatNumbertolocal: true,
                    digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
                }),
            new ColumnCombobox({
                field: "Epoca_Des",
                title: this.translocoService.translate('Epoca')
            }),
            new ColumnCombobox({
                field: "Mdi.descrizione",
                title: this.translocoService.translate('ModalitàImpiego')
            }),
            new ColumnCombobox({
                field: "Flag_Protetto.descrizione",
                title: this.translocoService.translate('Tipo')
            }),
            new ColumnCombobox({
                field: "DataSmaltimentoScorte",
                title: this.translocoService.translate('FineScorta')
            },
                {
                    width: 100
                })
        );
    }

    //Carico la ddl solo quando scatta l'evento di open
    async openControlDettagliFormulati(
        ddlEl: GiasDropDownTemplateSComponent,
        formName: string
    ) {
        let fn: any;

        let imposta = this.prodottiservice.Imposta_In_Automatico_Elemento_alla_DDL();

        switch (formName) {
            case "Soglia_Avversita":
                fn = await this.qdcdettagliformulatiservice.getArray_Soglie_Avversita(imposta);
                UtilityFunctions.loadDropDownItems(ddlEl, fn);
                break;
            case "Dose_Etichetta":
                fn = await this.qdcdettagliformulatiservice.getArray_Dosi(true, imposta);
                UtilityFunctions.loadDropDownItems(ddlEl, fn);
                break;
        }
    }


    mostraParticolaritaDiserbo(): boolean {

        let mostra = false;

        if (this.Categoria_Magazzino === FORMULATI) {
            if (this.qdcdettagliformulatiservice.doseConsentitaDiserbo.lbl_qta_residua !== '' ||
                this.qdcdettagliformulatiservice.doseConsentitaDiserbo.Lbl_Dose_Consigliata !== '' ||
                this.qdcdettagliformulatiservice.doseConsentitaDiserbo.Div_DettaglioDoseConsentita ||
                (this.qdcservice.obj_Inizializza_QdC.flagNuovoControlloRiduzioneDiserbo &&
                    this.qdcdettagliformulatiservice.doseConsentitaDiserbo.Lbl_Dose_Consigliata2 !== '')) {
                mostra = true;
            }
        }

        return mostra;
    }


    mostraSoglieGiustificazioni() {
        let mostra = false;
        if (this.qdcdettagliformulatiservice.MostraNascondiDDLSezioneFormulati("Soglia_Avversita") &&
            this.qdcdettagliformulatiservice.Array_Soglie_Avversita &&
            this.qdcdettagliformulatiservice.Array_Soglie_Avversita.length > 0
        ) {
            mostra = true;
        }

        return mostra;
    }

    mostraVerificaSoglia() {
        let mostra = false;

        if (this.Categoria_Magazzino === FORMULATI) {
            if (this.mostraSoglieGiustificazioni() && this.qdcdettagliformulatiservice.Verifica_Soglia !== '')
                mostra = true;
        }

        return mostra;
    }

    mostraDose() {
        let mostra = false;

        if (this.qdcdettagliformulatiservice.MostraNascondiDDLSezioneFormulati("Dose_Etichetta") &&
            this.qdcdettagliformulatiservice.Array_Dosi &&
            this.qdcdettagliformulatiservice.Array_Dosi.length > 0) {
            mostra = true;
        }

        return mostra;
    }

    public async rilevaAvversita() {

        let eserciziCDC: EsercizioCDC[] = this.qdcservice.GetEserciziCDCSelezionatiModel();

        let Soglia_Avversita: Soglia = this.FormulatiForm.getRawValue().Soglia_Avversita;

        let disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

        let Avversita: AvversitaGruppo = this.FormulatiForm.getRawValue().Avversita;

        if (disciplinare.codice !== NessunDpiNessunaEtichetta && Soglia_Avversita && Soglia_Avversita.codice > 0 &&
            eserciziCDC && eserciziCDC.length > 0 && Avversita && Avversita.codice > 0) {

            switch(+ Soglia_Avversita.lavorazione.primaryKey.codice){
              //Nel caso di Rilievo Avversità delle Trappole devo ottenere gli impianti associati alle trappole (prodotto)
              // registrate per l'avversità selezionata e passarli alla pagina di rilievo avversità per poter mostrare solo quelli associati all'avversità stessa
              case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:

                const leggiNumeroTrappole = <Leggi_Numero_Trappole>{
                  eserciziCDC: eserciziCDC,
                  avversitaGruppo: Avversita
                };

                this.agendaService.Ottieni_Numero_Trappole_Registrate(leggiNumeroTrappole).subscribe(async r=>{

                  if(r.RispostaOK && r.RispostaStringa && r.RispostaStringa.length > 0){

                    await this.redirectToRilievoAvversita(Soglia_Avversita,Avversita,r.RispostaStringa);

                  }else{

                    const listerroriGias: ErroreGias[] = [];

                    listerroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.WarningBloccante,
                      tipo: enum_ErroreGias_Tipo.Generico,
                      messaggio: this.translocoService.translate("NoInstallazioniXAvvXImp", {NomeOp: Soglia_Avversita.lavorazione.descrizione})
                    });

                    await this.qdcservice.gestisci_ErroriGias(listerroriGias, true, true);
                  }
                });
                break;

              case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
                await this.redirectToRilievoAvversita(Soglia_Avversita,Avversita,[]);
                break;
            }
        }
    }

    private async redirectToRilievoAvversita(Soglia_Avversita: Soglia, Avversita: AvversitaGruppo, avversitaTrappole: AvversitaTrappole[]): Promise<void> {

      this.setNewObjParametri(Soglia_Avversita,Avversita,avversitaTrappole);

      let iframeWindowService = await this.gestionerichiesteservice.gestionePassaggioStessoSito_Aperto_in_Iframe(enum_PagineGiasNG.Pagina_Edit_Attivita,[],
        -1,this.transloco.translate('qdc.RilevaAvversita.Text'))

      iframeWindowService.window.window.onDestroy(async ()=>{
        await this.handelWindowClose();
      });
    }

    private async handelWindowClose() {
        this.isWindowOpen = false;
        this.gestionerichiesteservice.ObjPageToMemorize = null;
        await this.qdcdettagliformulatiservice.Componi_Verifica_Soglia();
    }

    private setNewObjParametri(Soglia_Avversita: Soglia, Avversita: AvversitaGruppo,avversitaTrappole: AvversitaTrappole[]) {

        const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgendaService.getObjParamValue()));

        newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        newobjParametriAgenda.Lav_Cod = + Soglia_Avversita.lavorazione.primaryKey.codice;
        newobjParametriAgenda.Lav_Des = Soglia_Avversita.lavorazione.descrizione;
        newobjParametriAgenda.Veg_Cod = this.qdcservice.GetSpeciefromUtilizzoTerreno().codice;
        newobjParametriAgenda.Veg_Des = this.qdcservice.GetSpeciefromUtilizzoTerreno().descrizione;
        newobjParametriAgenda.Sa_Cod = (this.qdcservice.TestataForm.get("Centro_Aziendale").getRawValue() as CentroAziendale).primaryKey.codice;
        newobjParametriAgenda.SaNome = (this.qdcservice.TestataForm.get("Centro_Aziendale").getRawValue() as CentroAziendale).nome;
        newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();

        newobjParametriAgenda.Impianti = [];

        if (this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue() && this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().length > 0) {

           let Impianti:GridImpiantoSelezionatoModel[]  = [];

            switch(newobjParametriAgenda.Lav_Cod){
              //Prendo solo gli impianti associati alle trappole che hanno registrato l'avversità selezionata
              case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:
                Impianti = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().filter((el: GridImpiantoSelezionatoModel)=>{
                  if(avversitaTrappole.findIndex(a=>a.utilizzi &&
                    a.utilizzi.findIndex(u=>u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva === el.PIVA &&
                      u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice === el.SA_COD &&
                      u.impianto.primaryKey.appezzamentoPK.codice === el.APPEZZA &&
                      u.impianto.primaryKey.codice === el.ID_REG) > -1) > -1){

                    return true;

                  }
                });
                break;
              case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
                Impianti = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();
                break;
            }


            if(Impianti && Impianti.length > 0){
              newobjParametriAgenda.Impianti = Impianti.map((el: GridImpiantoSelezionatoModel) => {
                return <ImpiantiAgendaNG>{
                  Piva: el.PIVA,
                  Sa_Cod: el.SA_COD,
                  Appezza: el.APPEZZA,
                  Id_Reg: el.ID_REG,
                  Progetto_Cod: el.Progetto_Cod,
                  Sup_Imp: el.Sup_Imp,
                  Sup_Imp_help: el.Sup_Imp_help,
                  Sup_Riduzione_BufferZone: el.Sup_Riduzione_BufferZone,
                  Perc_Riduzione_Deriva: el.Perc_Riduzione_Deriva
                }
              });

              newobjParametriAgenda.Data = this.qdcservice.TestataForm.get("Data").getRawValue();

              let obj = this.giasRedirector.setQdCFormModelforRedirect(newobjParametriAgenda, RibaltamentoTypes.Nessuno);

              this.autoValorizeRilievoAvversita(obj,Soglia_Avversita,Avversita,avversitaTrappole,Impianti);

              newobjParametriAgenda.GenericObj_string = JSON.stringify(obj);
            }
        }

        this.objParametriAgendaService.changeObjParametriAgenda(newobjParametriAgenda);
    }

    private autoValorizeRilievoAvversita(toMemorize: Redirect_To_GiasNG_Page,Soglia_Avversita: Soglia,
                                         Avversita: AvversitaGruppo,avversitaTrappole: AvversitaTrappole[],Impianti: GridImpiantoSelezionatoModel[]) {

        let sezione = new Sezione_Rilievi();
        sezione.Operazione = Soglia_Avversita.lavorazione;
        sezione.DettagliRilievi = [];

        switch(+ sezione.Operazione.primaryKey.codice){
          case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:

            avversitaTrappole.forEach(a=>{
              a.utilizzi.forEach(u=>{

                //Aggiungo solamente il dettaglio rilievo che non è già presente nella sezione dei rilievi

                let index_imp: number = Impianti.findIndex(i => i.PIVA === u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva &&
                  i.SA_COD === u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice &&
                  i.APPEZZA === u.impianto.primaryKey.appezzamentoPK.codice &&
                  i.ID_REG === u.impianto.primaryKey.codice);

                let index_Dettaglio_Rilievo: number = sezione.DettagliRilievi.findIndex(i => i.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva === u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva &&
                  i.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice === u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice &&
                  i.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice === u.impianto.primaryKey.appezzamentoPK.codice &&
                  i.esercizioCDC.esercizio.impiantoPK.codice === u.impianto.primaryKey.codice &&
                  i.risorsaProdotto && i.risorsaProdotto.prodotto.codice === u.risorsaProdotto.prodotto.codice &&
                  i.avversitaGruppo && i.avversitaGruppo.codice === Avversita.codice);

                if(index_imp > -1 && index_Dettaglio_Rilievo === -1){

                  sezione.DettagliRilievi.push(this.getDettaglioRilievoForRedirect(Soglia_Avversita,Avversita,u.risorsaProdotto,sezione.DettagliRilievi.length.toString(),
                                                      Impianti[index_imp].PIVA, Impianti[index_imp].SA_COD,Impianti[index_imp].APPEZZA, Impianti[index_imp].ID_REG,
                                                      Impianti[index_imp].Progetto_Cod, Impianti[index_imp].Sup_Imp));
                }

              });
            });
            break;
          case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
            for (let impianto of toMemorize.objParametriAgenda.Impianti) {

              sezione.DettagliRilievi.push(this.getDettaglioRilievoForRedirect(Soglia_Avversita,Avversita,null,sezione.DettagliRilievi.length.toString(),
                                            impianto.Piva, impianto.Sa_Cod,impianto.Appezza, impianto.Id_Reg,
                                            impianto.Progetto_Cod, impianto.Sup_Imp));
            }
            break;
        }

        toMemorize.QdCFormValue.Trattamento.Sezioni_Senza_Prodotto.push(sezione);
    }

    private getDettaglioRilievoForRedirect(Soglia_Avversita: Soglia, Avversita: AvversitaGruppo,risorsaProdotto: RisorsaProdotto,CodRilievo: string,
                                           Piva: string, Sa_Cod: number,Appezza: number, Id_Reg: number, Progetto_Cod: number, Sup_Trattata: number):DettaglioRilievo{

      let dettaglio = new DettaglioRilievo();
      dettaglio.unitaDiMisura = Soglia_Avversita.udm;
      dettaglio.avversitaGruppo = Avversita as any;
      dettaglio.descrizione = Avversita.descrizione
        + ' ' + Soglia_Avversita.descrizione;
      dettaglio.qtaRilevata = Soglia_Avversita.quantita || 1;
      dettaglio.qtaRilevataString = dettaglio.qtaRilevata.toString();
      dettaglio.esercizioCDC = new EsercizioRilievoCDC();
      dettaglio.esercizioCDC.setKey(
        Piva, Sa_Cod,
        Appezza, Id_Reg, Progetto_Cod);
      dettaglio.esercizioCDC.superficieTrattata = Sup_Trattata;
      dettaglio.codRilievo = CodRilievo;
      dettaglio.risorsaProdotto = risorsaProdotto;
      dettaglio.note = "";

      return dettaglio;

    }

    public InserisciFormulato() {
        this.prodottiservice.InserisciDoseProdotto.next(true);
    }

    public Mostra_Lbl_Dose_Consigliata() {

        let mostra = false;

        //Da mostrare solamente se la riga non è salvata
        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FormulatiForm.get("Riga_Salvata").value) {
            return mostra;
        }

        if (this.qdcdettagliformulatiservice.doseConsentitaDiserbo.lbl_qta_residua !== "" ||
            this.qdcdettagliformulatiservice.doseConsentitaDiserbo.Lbl_Dose_Consigliata !== "") {

            mostra = true;
        }

        return mostra;
    }

    public Mostra_Lbl_Dose_Consigliata2() {

        let mostra = false;

        if (this.qdcservice.obj_Inizializza_QdC.flagNuovoControlloRiduzioneDiserbo &&
            this.qdcdettagliformulatiservice.doseConsentitaDiserbo.Lbl_Dose_Consigliata2 !== '') {
            mostra = true;
        }

        return mostra;

    }

    ngDoCheck(): void {
        if (this.isWindowOpen) {
            // style window content
            document.getElementsByClassName('k-window-content').item(0)?.setAttribute('style', 'overflow: auto !important');
            document.getElementsByClassName('editPageContent').item(1)?.setAttribute('style', 'margin-bottom: 16px !important');
            document.getElementsByClassName('qdc-footer').item(1)?.setAttribute('style', 'position: inherit');
            document.getElementsByClassName('k-appbar-top').item(1)?.setAttribute('style', 'position: inherit');
        }
    }

    public MostraGridProdottixImpianti(): boolean{
      let mostra: boolean = false;

      if(this.Operazione &&
        this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ this.Operazione.primaryKey.codice) &&
         this.FormulatiForm.get("Ripartizione_Trappole").getRawValue()?.codice === enum_Ripartizione_Trappole.Manuale){
        mostra = true;
      }

      return mostra;
    }

    public MostraTemplateAvversitaInnesco(): boolean{
      let mostra: boolean = false;

      if(this.Operazione && this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ this.Operazione.primaryKey.codice))
        mostra = true;

      return mostra;
    }

    /*
    changeButtonGroupNessunMagazzino(value){

        if(value){
            this.ProdottiForm.patchValue({magazzino: []});
        }
    }

    OpzNessunaGestionedelMagazzinoAz(){

        this.disabilitaOpzNessunaGestionedelMagazzinoAz = false;

        let Lav_Cod = + this.TestataForm.get('Categoria_Operazione').value.primaryKey.codice;

        //Se ho l'impostazione per forzare una semina che non è quella semplice allora il magazzino deve essere obbligatorio
        if(Lav_Cod === enum_LAVCOD.TRAPIANTO ||
            Lav_Cod === enum_LAVCOD.SEMINA ||
            Lav_Cod === enum_LAVCOD.SOVESCIO ||
            Lav_Cod === enum_LAVCOD.SOD_SEDDING){

            if(this.qdcservice.Tipo_Semina_Val === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo ||
                this.qdcservice.Tipo_Semina_Val === enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Vincolo){

                    this.disabilitaOpzNessunaGestionedelMagazzinoAz = true;

            }
        }

        if(this.qdcservice.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO &&
            this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write){

                this.disabilitaOpzNessunaGestionedelMagazzinoAz = true;

        }

    } */

}
