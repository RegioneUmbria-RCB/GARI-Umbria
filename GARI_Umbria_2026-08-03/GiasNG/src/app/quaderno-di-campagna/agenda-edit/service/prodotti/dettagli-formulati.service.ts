import {Injectable, OnDestroy} from '@angular/core';
import {Epoca} from 'app/Model/metaschema/Epoca';
import {
  enum_doseQuantitaTotale,
  enum_LAVCOD,
  enum_TipoFormulato,
  enum_TipoMezzo,
  enum_TipoOperazioneDB,
  enum_UnitaMisura
} from 'app/Model/TipiEnumerativi';
import {
  DropdownListAvversita,
  enum_Problema_DettaglioProdotto, MultiColumnComboboxAvversitaInnesco,
  MultiColumnComboboxDose_Etichetta, MultiColumnComboboxFertilizzazione,
  MultiColumnComboboxTrattamento
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {QdCProdottiService} from '../prodotti.service';
import {QdCService} from '../qdc.service';
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {EpocheService} from 'app/Service/Metaschema/epoche.service';
import {FunzioniComuniService} from 'app/Service/FunzioniComuni.service';
import {lastValueFrom, Subscription} from "rxjs";
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {
  AGRODATAINIZIO,
  DpiBio,
  FORMULATI,
  INNESCHI,
  INSETTI,
  LAVCOD_DISTRIBUZIONE_INSETTI,
  NessunDpi,
  NessunDpiNessunaEtichetta,
} from '../../../../Model/CostantiPersonalizzate';
import {LeggiProdotti, ProdottiService} from '../../../../Service/Anagrafica/prodotti.service';
import {AgendaService, DoseConsentitaDiserbo, LeggiDoseConsentitaDiserbo} from '../../../../Service/Agenda/Agenda.service';
import {GridDosiProdottiService} from '../grid-dosi-prodotti/grid-dosi-prodotti.service';
import {ObjParametriAgendaService} from '../../../../Service/obj-parametri-agenda.service';
import {Attivita} from '../../../../Model/attivita/Attivita';
import {
  DettaglioTrattamento,
  enum_Ripartizione_Trappole
} from '../../../../Model/attivita/dettagli/DettaglioTrattamento';
import {Localizzazione} from '../../../../Model/metaschema/Localizzazione';
import {Soglia} from '../../../../Model/metaschema/Soglia';
import {AvversitaGruppo} from '../../../../Model/metaschema/avversita/AvversitaGruppo';
import {DestinazioneUso} from '../../../../Model/metaschema/utilizzi/DestinazioneUso';
import {Fabbricato} from '../../../../Model/anagrafiche/Fabbricato';
import {ColumnCombobox, GiasMultiColumnComboboxTemplateComponent} from 'gias-ui-kit';
import {
  CELL_TYPES
} from 'gias-ui-kit';
import {LeggiUnitaDiMisura, UnitaDiMisuraService} from '../../../../Service/Metaschema/UnitaDiMisura.service';
import {AvversitaService, LeggiAvversita} from '../../../../Service/Metaschema/avversita.service';
import {TranslocoService} from '@jsverse/transloco';
import {DosiEtichettaService, LeggiDosiEtichetta} from '../../../../Service/Metaschema/DosiEtichetta.service';
import {GiasMessageService} from '../../../../Service/gias-message.service';
import {QdCFormToAttivitaService} from '../quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service';
import {GridImpiantiService} from '../grid-impianti/grid-impianti.service';
import {LeggiLocalizzazioni, LocalizzazioniService} from '../../../../Service/Metaschema/localizzazioni.service';
import {DatePipe} from '@angular/common';
import {Specie} from 'app/Model/metaschema/utilizzi/Specie';
import {QdCFormulatiService} from './formulati.service';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from '../../../../Service/master.service';
import * as _ from 'lodash';
import {UtilityFunctions} from '../../../../Utility/UtilityFunctions';
import {Disciplinare} from "../../../../Model/metaschema/Disciplinari";
import {DitteService} from "../../../../Service/Anagrafica/ditte.service";
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";
import {GridDosiProdottiControlliService} from "../grid-dosi-prodotti/grid-dosi-prodotti-controlli.service";
import {QuantitaSuImpianto} from "../../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {cloneDeep} from "lodash";
import {GridPublicService, KendoGridColumn} from "gias-kendo-grid";
import {
  GridProdottixImpiantiServerResult
} from "../dettagli-formulati/gestione-trappole/grid-prodotti-impianti-http.service";
import { QdCUnitadiMisuraService } from '../unita-di-misura.service';
import {RilevamentoDiMagazzino} from "../../../../Model/attivita/RilevamentoDiMagazzino";

@Injectable()

export class QdCDettagliFormulatiService implements OnDestroy {

    Array_Formulati: Array<MultiColumnComboboxTrattamento> = [];
    Subs: Subscription = new Subscription();
    Array_Dosi: Array<MultiColumnComboboxDose_Etichetta> = [];
    private _Array_Avversita: Array<DropdownListAvversita> = [];
    Array_Soglie_Avversita: Array<Soglia> = [];

    FormulatiForm: FormGroup;
    Operazione: Lavorazione;
    Categoria_Magazzino: number;
    Lav_Cod: number = 0;

    Prima_Apertura_MultiColumn_Trattamento = false;
    ColumnComboboxFormulati: Array<ColumnCombobox> = [];
    public Verifica_Soglia: string = "";
    public Riga_NoteProdotto: string = "";

    //Valorizzato solamente se siamo nel caso d'inserimento prodotto senza la grid e se la dose etichetta scelta
    //ha un'altra dose compatibile
    public Str_Dosi_Compatibili: string = "";

    //Valorizzato solamente nel diserbo
    doseConsentitaDiserbo = new DoseConsentitaDiserbo();

    GridProdottiXImpiantiHttpService: any;
    GridProdottiXImpiantiPublicService: GridPublicService;

    private _Array_Avversita_Inneschi: Array<MultiColumnComboboxAvversitaInnesco> = [];

    ColumnComboboxAvversitaInneschi: Array<ColumnCombobox> = [];

    Array_Udm_Inneschi: UnitaDiMisura[] = [];

    //Valore precedente della DDL Unita di Misura Innesco
    _unita_innesco:UnitaDiMisura;

    Aggiorna_UdM_Innesco(UdM: UnitaDiMisura) {
      //Per UdM ho bisogno di memorizzare il precedente elemento quindi ho bisogno dell'emitEvent
      this.FormulatiForm.patchValue({UdM_Innesco: UdM}, {emitEvent: true, onlySelf: true});
    }

    public get Array_Avversita(): Array<DropdownListAvversita> | Array<MultiColumnComboboxAvversitaInnesco> {
      if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){
        return this._Array_Avversita_Inneschi;
      }else{
        return this._Array_Avversita;
      }
    }

    public set Array_Avversita(arr: Array<DropdownListAvversita> | Array<MultiColumnComboboxAvversitaInnesco>){
      if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){
        this._Array_Avversita_Inneschi = (arr as Array<MultiColumnComboboxAvversitaInnesco>);
      }else{
        this._Array_Avversita = (arr as Array<DropdownListAvversita>);
      }
    }

    private _MultiColumnComboboxAvversitaInneschi: GiasMultiColumnComboboxTemplateComponent = null;

    get MultiColumnComboboxAvversitaInneschi(){
      return this._MultiColumnComboboxAvversitaInneschi;
    }

    set MultiColumnComboboxAvversitaInneschi(MultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent){
      this._MultiColumnComboboxAvversitaInneschi = MultiColumnCombobox;
    }

    constructor(
        private qdcservice: QdCService,
        private qdcprodottiservice: QdCProdottiService,
        private unitamisuraservice: UnitaDiMisuraService,
        private funzionicomuniservice: FunzioniComuniService,
        private avversitaservice: AvversitaService,
        private translocoService: TranslocoService,
        private dosietichettaservice: DosiEtichettaService,
        private giasmessageservice: GiasMessageService,
        private epocheservice: EpocheService,
        private agendaservice: AgendaService,
        private qdcformtoattivitaservice: QdCFormToAttivitaService,
        private gridimpiantiservice: GridImpiantiService,
        private localizzazioniservice: LocalizzazioniService,
        private prodottiservice: ProdottiService,
        private datepipe: DatePipe,
        private objParametriAgendaService: ObjParametriAgendaService,
        private griddosiprodottiservice: GridDosiProdottiService,
        private qdcformulatiservice: QdCFormulatiService,
        private ditteservice: DitteService,
        private gridcontrolliservice: GridDosiProdottiControlliService,
        private qdcUnitMisuraService: QdCUnitadiMisuraService){

        this.Subs.add(this.qdcprodottiservice.ObsProdottiForm.subscribe(form => {
            this.FormulatiForm = form;
            this.Operazione = this.FormulatiForm?.get("Operazione")?.value;
            this.Categoria_Magazzino = this.FormulatiForm?.get("Categoria_Magazzino")?.value;

            this.Lav_Cod = + this.Operazione?.primaryKey?.codice;
        }));

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Letture Lato Server

    async getArray_Formulati(filtrodescr: string,impostaInAutomaticoPrimoElementoAllaDDL:boolean = true) {

        if(this.qdcprodottiservice.Msg_Seleziona_Impianti().length === 0){


                //Se devo leggere i formulati devo passare anche il valore delle epoche e
                //delle avversita
                let LeggiProdotti = <LeggiProdotti>{
                    lavorazione: this.Operazione,
                    impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                    prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
                    specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                    disciplinare: this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice),
                    epocaDPI: this.FormulatiForm.get("EpocaDPI").value,
                    avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo),
                    filtroPerDescrizione: filtrodescr,
                    data: this.qdcservice.TestataForm.get("Data").value,
                    escludiGiacenzeZero: this.FormulatiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value,
                    tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
                    statoAttivita: this.qdcservice.TestataForm.get("Stato").value,
                    magazziniAgenzie: this.FormulatiForm.get("Visualizza_Magazzini_Agenzie").value,
                    magazziniEsterni: this.FormulatiForm.get("Visualizza_Magazzini_Esterni").value
                };

                this.Array_Formulati = (await this.prodottiservice.Leggi_Formulati(LeggiProdotti)) as MultiColumnComboboxTrattamento[];



            //Descrizione Concatenata
            let Array_FormulatiMapped = this.Array_Formulati.map(p=>{
                p = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(FORMULATI,p) as MultiColumnComboboxTrattamento;
            });

            // Se c'è solo un Prodotto (diversa da quella già impostata nella combo altrimenti scatta la funzione di change) lo imposto come default
            if (impostaInAutomaticoPrimoElementoAllaDDL && this.Array_Formulati.length === 1) {

                let Formulato_Selezionato: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

                if(Formulato_Selezionato?.Codice_Concatenato !== this.Array_Formulati[0].Codice_Concatenato){
                    this.FormulatiForm.patchValue({
                        Prodotto: this.Array_Formulati[0],
                    });

                    await this.changeFormulato(this.Array_Formulati[0]);
                }


            }

        }else{
            this.Array_Formulati = [];
        }



        return this.Array_Formulati;
    }

    async getArray_Insetti(filtrodescr: string) {
        if(this.qdcprodottiservice.Msg_Seleziona_Impianti().length === 0){

            let LeggiProdotti: LeggiProdotti = {
                lavorazione: this.Operazione,
                impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
                specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                disciplinare: undefined,
                epocaDPI: undefined,
                avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo),
                filtroPerDescrizione: filtrodescr,
                data: this.qdcservice.TestataForm.get("Data").value,
                escludiGiacenzeZero: this.FormulatiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value,
                tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
                statoAttivita: this.qdcservice.TestataForm.get("Stato").value,
                magazziniAgenzie: this.FormulatiForm.get("Visualizza_Magazzini_Agenzie").value,
                magazziniEsterni: this.FormulatiForm.get("Visualizza_Magazzini_Esterni").value
            };

            this.Array_Formulati = (await this.prodottiservice.LeggiInsetti(LeggiProdotti))as MultiColumnComboboxTrattamento[];

            //Descrizione Concatenata
            let Array_FormulatiMapped = this.Array_Formulati.map(p=>{
                p = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(INSETTI, p) as MultiColumnComboboxTrattamento;
                p.impollinatore = p.isImpollinatore ? 'X' : '';
            });

            // Se c'è solo un Prodotto (diversa da quella già impostata nella combo altrimenti scatta la funzione di change) lo imposto come default
            if (this.Array_Formulati.length === 1) {
                let Formulato_Selezionato: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

                if(Formulato_Selezionato?.Codice_Concatenato !== this.Array_Formulati[0].Codice_Concatenato){
                    this.FormulatiForm.patchValue({
                        Prodotto: this.Array_Formulati[0],
                    });
                    await this.changeInsetto(this.Array_Formulati[0]);
                }
            }

        } else {
            this.Array_Formulati = [];
        }

        return this.Array_Formulati;
    }

    async getArray_Avversita(impostaInAutomaticoPrimoElementoAllaDDL: boolean = true) {

        let Prodotto: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

        if(this.flag_LeggiAvversita()) {
            if (this.Categoria_Magazzino === FORMULATI) {

              if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){

                await this.getArray_Avversita_Innesco(impostaInAutomaticoPrimoElementoAllaDDL);

              }else{

                const LeggiAvversita = <LeggiAvversita>{
                  tipoAttivita: this.qdcservice.TestataForm.get('Tipo').value,
                  lavorazione: this.Operazione,
                  impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                  // prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
                  specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                  disciplinare: this.qdcservice.getDisciplinareModelValue(+this.Operazione.primaryKey.codice),
                  epocaDPI: this.FormulatiForm.get('EpocaDPI').value,
                  data: this.qdcservice.TestataForm.get('Data').value,
                  dettaglioTrattamento: null
                };

                //Passo anche il dettaglioTrattamento se sono nel caso Prodotto -> Avversita
                if (!this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
                  LeggiAvversita.dettaglioTrattamento = (Prodotto as DettaglioTrattamento);
                }

                let risp = (await this.avversitaservice.Leggi_Avversita_QdC(LeggiAvversita)) as DropdownListAvversita [];

                if (risp) {
                  let rispMapped = risp.map(a => {
                    a = this.qdcservice.setCodDescrDdlAvversita(a as DropdownListAvversita,this.Lav_Cod);
                  });

                  this.Array_Avversita = risp;

                  // Se c'è solo una Avversità (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
                  // lo imposto come default e carico le Dosi Etichetta
                  if (this.Array_Avversita.length === 1 && impostaInAutomaticoPrimoElementoAllaDDL) {

                    let Avversita_Selezionata: DropdownListAvversita = this.FormulatiForm.get('Avversita').value;

                    if (Avversita_Selezionata?.Codice_Concatenato !== this.Array_Avversita[0].Codice_Concatenato) {
                      this.FormulatiForm.patchValue({
                        Avversita: this.Array_Avversita[0],
                      });

                      await this.changeAvversita(this.Array_Avversita[0]);
                    }

                  }

                  this.Componi_Messaggio_Avversita();


                }

              }
            } else if (this.Categoria_Magazzino === INSETTI) {
                await this.getArray_Avversita_X_Insetti(impostaInAutomaticoPrimoElementoAllaDDL);
            }

        }

        return this.Array_Avversita;
    }

    async getArray_Dosi(impostaArrayallaDDL: boolean = true,impostaInAutomaticoPrimoElementoAllaDDL:boolean = true) {

        let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

        let risp = [];

        if(this.MostraNascondiDDLSezioneFormulati("Dose_Etichetta")){

            const LeggiDosiEtichetta = <LeggiDosiEtichetta>{
                specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                dettaglioTrattamento: this.FormulatiForm.get("Prodotto").value,
                avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo),
                impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
                disciplinare: Disciplinare,
                data: this.qdcservice.TestataForm.get("Data").value,
            };

            risp = (await this.dosietichettaservice.Leggi_DosiEtichetta_QdC(LeggiDosiEtichetta)) as MultiColumnComboboxDose_Etichetta[];

            //Descrizione Concatenata
            let rispMapped = risp.map(d=>{
                d = this.qdcservice.setCodDescrConcatenataMultiColumnComboboxMultiColumnComboboxDose_Etichetta(d);
            });

            if(impostaArrayallaDDL){

                this.Array_Dosi = risp;

                // Se c'è solo una sola Dose (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
                // lo imposto come default
                if (this.Array_Dosi.length === 1 && impostaInAutomaticoPrimoElementoAllaDDL) {

                    let Dose_Selezionata: MultiColumnComboboxDose_Etichetta = this.FormulatiForm.get("Dose_Etichetta").value;

                    if(Dose_Selezionata?.Codice_Concatenato !== this.Array_Dosi[0].Codice_Concatenato){
                        this.FormulatiForm.patchValue({
                            Dose_Etichetta: this.Array_Dosi[0],
                        });

                        await this.changeDoseEtichetta(this.Array_Dosi[0]);
                    }

                }
            }

        }else{

            risp = [];

            if(impostaArrayallaDDL)
                this.Array_Dosi = risp;

        }


        return risp;
    }

    private async getArray_Avversita_X_Insetti(impostaInAutomaticoPrimoElementoAllaDDL: boolean = true){

        let Prodotto: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

        const LeggiAvversita = <LeggiAvversita>{
            tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
            lavorazione: this.Operazione,
            impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
            // prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
            specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
            data: this.qdcservice.TestataForm.get("Data").value,
            dettaglioTrattamento: new DettaglioTrattamento()
        };

        //Passo anche il dettaglioTrattamento se sono nel caso Prodotto -> Avversita
        if(!this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti)
            LeggiAvversita.dettaglioTrattamento = (Prodotto as DettaglioTrattamento);

        let risp: Array<DropdownListAvversita> = (await this.avversitaservice.LeggiAvversitaDistribuzioneInsetti(LeggiAvversita)) as DropdownListAvversita[];

        if(risp){
            let rispMapped = risp.map(a=>{
                a = this.qdcservice.setCodDescrDdlAvversita(a as DropdownListAvversita,this.Lav_Cod);
            });

            this.Array_Avversita = risp;

            // Se c'è solo una Avversità (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
            // lo imposto come default e carico le Dosi Etichetta
            if (this.Array_Avversita.length === 1 && impostaInAutomaticoPrimoElementoAllaDDL) {
                let Avversita_Selezionata: DropdownListAvversita = this.FormulatiForm.get("Avversita").value;

                if(Avversita_Selezionata?.Codice_Concatenato !== this.Array_Avversita[0].Codice_Concatenato){
                    this.FormulatiForm.patchValue({
                        Avversita: this.Array_Avversita[0],
                    });
                    await this.changeAvversita(this.Array_Avversita[0]);
                }
            }

            this.Componi_Messaggio_Avversita();
        }
    }

    async getArray_Soglie_Avversita(impostaInAutomaticoPrimoElementoAllaDDL:boolean = true) {

        //Carico le Soglie Avversita
        //solo se è stato scelto un disciplinare

        if(this.MostraNascondiDDLSezioneFormulati("Soglia_Avversita")){

            let lav_cod:number = +this.Operazione.primaryKey.codice;

            let disciplinare: Disciplinare = this.qdcservice.getDisciplinareModelValue(lav_cod);

            const LeggiAvversita = <LeggiAvversita>{
                disciplinare: disciplinare,
                avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo)
            };

            this.Array_Soglie_Avversita =
                await this.avversitaservice.Leggi_SoglieAvversita_QdC(
                    LeggiAvversita
                );

            // Se c'è solo una Soglia Avversita (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
            // lo imposto come default
            if (this.Array_Soglie_Avversita.length === 1 && impostaInAutomaticoPrimoElementoAllaDDL) {

                let Soglie_Selezionata: Soglia = this.FormulatiForm.get("Soglia_Avversita").value;

                if(Soglie_Selezionata?.codice !== this.Array_Soglie_Avversita[0].codice){
                    this.FormulatiForm.patchValue({
                        Soglia_Avversita: this.Array_Soglie_Avversita[0],
                    });

                    this.changeSoglia(this.Array_Soglie_Avversita[0]);
                }
            }

        }else{
            this.Array_Soglie_Avversita = [];
        }


        return this.Array_Soglie_Avversita;
    }

    getDescrizioneLocalizzazione(){
        return new Promise<string>(async (resolve, reject) => {

            let DescrizioneLocalizzazione = "";

            let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

            if(Disciplinare?.codice !== NessunDpiNessunaEtichetta &&
                Disciplinare?.codice !== NessunDpi &&
                Disciplinare?.codice !== DpiBio){

                //Carico le localizzazioni
                const LeggiLocalizzazioni = <LeggiLocalizzazioni>{
                    lavorazione: this.Operazione,
                    dettaglioTrattamento: this.FormulatiForm.get("Prodotto").value,
                    disciplinare: Disciplinare,
                    avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo),
                    specie: this.qdcservice.GetSpeciefromUtilizzoTerreno()
                };

                this.localizzazioniservice.LeggiLocalizzazioni_Modello(LeggiLocalizzazioni).then(risp =>{

                    let localizzazioni: Array<Localizzazione> = risp.RispostaStringa;

                    if(!localizzazioni || !risp.RispostaOK){
                        DescrizioneLocalizzazione = risp.Errore;
                    } else {
                        if(localizzazioni && localizzazioni.length > 0){
                            if(localizzazioni[0].codice !== 0)
                                DescrizioneLocalizzazione = localizzazioni[0].descrizione;
                        }
                    }

                    resolve(DescrizioneLocalizzazione);

                });

            }else{
                resolve(DescrizioneLocalizzazione);
            }

        });

    }

    //Fine Letture Lato Server

    Componi_Messaggio_Avversita(){
        //Messaggio che compare nel caso non ci siano delle Avversità

        let Operazione: Lavorazione = this.Operazione;

        let Lav_Cod: number = + Operazione.primaryKey.codice;

        let Dpi_Cod:number = + this.qdcservice.getDisciplinareModelValue(Lav_Cod)?.codice;

        let tipoTestata: number = this.qdcservice.getTipoTestata(Lav_Cod);

        let messaggio: string = "";

        if(this.Array_Avversita.length === 0){

            if(Dpi_Cod > 0){

                if(tipoTestata === 0){
                    messaggio = this.translocoService.translate('qdc.NoCorrispondenzaAvversita',{Operazione: Operazione.descrizione});
                }else{
                    messaggio = this.translocoService.translate('qdc.NoCorrispondenzaInfestanti',{Operazione: Operazione.descrizione});
                }

            } else {

                if(tipoTestata === 0){
                    messaggio = this.translocoService.translate('qdc.NessunaAvversitaTrovata',{Operazione: Operazione.descrizione});
                }else{
                    messaggio =  this.translocoService.translate('qdc.NessunInfestanteTrovato',{Operazione: Operazione.descrizione});
                }
            }
        }

        if(messaggio !== "") {

            let listerroriGias: ErroreGias[] = [];

            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: messaggio
            });

            this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();
        }


    }


    //Deve replicare il Imposta_DosiEtichetta nella Trattamenti_2
    async changeDoseEtichetta(doseEtichetta: MultiColumnComboboxDose_Etichetta){

        let FormulatiForm = <FormGroup> this.FormulatiForm;

        let Dose_HL:number = FormulatiForm.get("Dose_Hl").value;

        let Dose_HA:number = FormulatiForm.get("Dose_Ha").value;

        let DoseTot_HA:number = FormulatiForm.get("DoseTot_Ha").value;

        let flagTipoDose: number = FormulatiForm.get("flagTipoDose").value;

        let flagDoseQuantitaTotale: number = FormulatiForm.get("flagDoseQuantitaTotale").value;

        let UdM: UnitaDiMisura =  FormulatiForm.get("UdM").value;

        let copiaEtichetta: boolean = false;

        if(doseEtichetta && doseEtichetta.codice !== 0){

            let DoseMax: number = doseEtichetta.DoseMax;

            let Udm_Etichetta:UnitaDiMisura = doseEtichetta.Udm;

            let obj = this.unitamisuraservice.ScomponiUdm(Udm_Etichetta);

            //Se l'impostazione chili_litri è true imposto chili o litri nella combo delle udm
            //(Forzo a chili o litri solo quando si sceglie un dosaggio a etichetta come fa nella Trattamenti_2)
            if(this.qdcservice.Chili_Litri){

              let udm_kg_litri: UnitaDiMisura = null;

              let udm_convertita_kg_litri_impostazione = this.unitamisuraservice.ConvertiToKG_L(new UnitaDiMisura(obj.UDM_radice,""));

              if(udm_convertita_kg_litri_impostazione.Udm_Cod_Trasformato === enum_UnitaMisura.KG ||
                udm_convertita_kg_litri_impostazione.Udm_Cod_Trasformato === enum_UnitaMisura.Litri){

                let tutte_le_udm = await lastValueFrom(this.unitamisuraservice.Leggi_Tutte_UnitaDiMisura());

                if(tutte_le_udm && tutte_le_udm.length > 0){
                  udm_kg_litri = tutte_le_udm.find(u=>u.codice === udm_convertita_kg_litri_impostazione.Udm_Cod_Trasformato);
                }

              }

              if(udm_kg_litri && udm_kg_litri.codice > 0){

                obj.MoltiplicatoreDose = this.unitamisuraservice.Ottieni_Moltiplicatore(Udm_Etichetta,udm_kg_litri);

                Udm_Etichetta = this.unitamisuraservice.Componi_UdM_con_ha_hl(udm_kg_litri,obj.perHa_hl);

                obj.UDM_radice = this.unitamisuraservice.ScomponiUdm(Udm_Etichetta).UDM_radice;
              }
            }

            //Se la Udm è disabilitata perchè non può essere cambiata per esempio per l'impostazione delle giacenze 180 a 0 o 1 non posso
            //sovrascrivere l'Udm dell'etichetta
            if(UdM &&
                UdM.codice !== 0 &&
                !this.qdcprodottiservice.controlli_Giacenze_Lotti.ddl_UdM.abilitato &&
                this.qdcprodottiservice.controlli_Giacenze_Lotti.ddl_UdM.visibile){

                obj.MoltiplicatoreDose = this.unitamisuraservice.Ottieni_Moltiplicatore(Udm_Etichetta,UdM);

                Udm_Etichetta = this.unitamisuraservice.Componi_UdM_con_ha_hl(UdM,obj.perHa_hl);

                obj.UDM_radice = this.unitamisuraservice.ScomponiUdm(Udm_Etichetta).UDM_radice;
            }
            //Acqua
            await this.qdcservice.Imposta_VolumiAcqua(doseEtichetta.AcquaMax, doseEtichetta.UdmAcqua,doseEtichetta.Descrizione_Concatenata);

            let Acqua_Tot: number = this.qdcservice.AcquaForm.get("Acqua_Tot").value;

            let Sup_Trattata: number = this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare
                ? this.qdcservice.SuperficiForm.get("Sup_Trattata").value
                : this.qdcservice.QuantitaForm.get("Qta_Trattata").value;

            if(this.doseConsentitaDiserbo &&
                this.doseConsentitaDiserbo.D_HA_Max_Diserbo !== 0){

                let udm_sorgente_app: UnitaDiMisura = new UnitaDiMisura(obj.UDM_radice,"");
                let udm_sorgente_trasformata: number = 0;
                let Moltiplicatore_app: number = 0;

                let udmconvertita = this.unitamisuraservice.ConvertiToKG_L(udm_sorgente_app);

                udm_sorgente_trasformata = udmconvertita.Udm_Cod_Trasformato;

                Moltiplicatore_app = udmconvertita.Moltiplicatore;

                let dose_HA_app = DoseMax * Moltiplicatore_app * obj.MoltiplicatoreDose;

                if(dose_HA_app < this.doseConsentitaDiserbo.D_HA_Max_Diserbo){
                    copiaEtichetta = true;
                }else{
                    switch(obj.perHa_hl){
                        case enum_UnitaMisura.Ettolitro:
                            Dose_HL = this.doseConsentitaDiserbo.D_HA_Max_Diserbo;
                            DoseTot_HA = Dose_HL * Acqua_Tot;

                            if(Sup_Trattata > 0){
                                Dose_HA = DoseTot_HA / Sup_Trattata;
                            }
                            break;
                        case enum_UnitaMisura.Ettaro:
                            Dose_HA = this.doseConsentitaDiserbo.D_HA_Max_Diserbo;
                            //calcolo la dose totale
                            DoseTot_HA = Dose_HA * Sup_Trattata;
                            if (Acqua_Tot > 0) {
                                Dose_HL = DoseTot_HA / Acqua_Tot;
                            }else{
                                Dose_HL = 0;
                            }
                            break;
                    }

                    switch(this.doseConsentitaDiserbo.Udm_Radice_HA){
                        //peso
                        case enum_UnitaMisura.Grammi:
                        case enum_UnitaMisura.Milligrammi:
                        case enum_UnitaMisura.KG:
                        case enum_UnitaMisura.Tonnellate:
                        case enum_UnitaMisura.Quintali:
                            this.doseConsentitaDiserbo.Udm_Radice_HA = enum_UnitaMisura.KG;
                            break;
                        //acqua
                        case enum_UnitaMisura.CentimetriCubi:
                        case enum_UnitaMisura.Millilitri:
                        case enum_UnitaMisura.Litri:
                            this.doseConsentitaDiserbo.Udm_Radice_HA = enum_UnitaMisura.Litri;
                            break;
                    }
                }

            }else{
                copiaEtichetta = true;
            }

            if(copiaEtichetta === true){
                if(DoseMax !== 0){
                    switch(obj.perHa_hl){
                        case enum_UnitaMisura.Ettolitro:
                            flagTipoDose = enum_TipoMezzo.Ettolitro;
                            Dose_HL = DoseMax * obj.MoltiplicatoreDose;
                            DoseTot_HA = Dose_HL * Acqua_Tot;
                            if(Sup_Trattata > 0){
                                Dose_HA = DoseTot_HA / Sup_Trattata;
                            }
                            break;
                        case enum_UnitaMisura.Ettaro:
                            flagTipoDose = enum_TipoMezzo.Ettaro;
                            Dose_HA = DoseMax * obj.MoltiplicatoreDose;
                            //calcolo la dose totale
                            DoseTot_HA = Dose_HA * Sup_Trattata;
                            if(Acqua_Tot > 0){
                                Dose_HL = DoseTot_HA / Acqua_Tot;
                            }else{
                                Dose_HL = 0;
                            }
                            break;
                        case enum_UnitaMisura.Quintali:
                            flagTipoDose = enum_TipoMezzo.Quintale;

                            Dose_HA = DoseMax * obj.MoltiplicatoreDose;

                            //calcolo la dose totale
                            DoseTot_HA = Dose_HA * Sup_Trattata;
                            if(Acqua_Tot > 0){
                                Dose_HL = DoseTot_HA / Acqua_Tot;
                            }else{
                                Dose_HL = 0;
                            }
                            break;
                        case enum_UnitaMisura.Tonnellate:
                            flagTipoDose = enum_TipoMezzo.Quintale;

                            Dose_HA = DoseMax * obj.MoltiplicatoreDose / 10;

                            //calcolo la dose totale
                            DoseTot_HA = Dose_HA * Sup_Trattata;
                            if(Acqua_Tot > 0){
                                Dose_HL = DoseTot_HA / Acqua_Tot;
                            }else{
                                Dose_HL = 0;
                            }
                            break;
                    }

                }
            }

            flagDoseQuantitaTotale = enum_doseQuantitaTotale.Dose;

            let simbolo_concatenato: string[] = [];

            //Imposto la Unita di Misura

            if(this.qdcservice.Elenco_UdM_Radice_Con_Ha.includes(Udm_Etichetta.codice)){
                simbolo_concatenato.push(Udm_Etichetta.simbolo);
                simbolo_concatenato.push("ha");
            }else if (this.qdcservice.Elenco_UdM_Radice_Con_Hl.includes(Udm_Etichetta.codice)) {
                simbolo_concatenato.push(Udm_Etichetta.simbolo);
                simbolo_concatenato.push("hl");
            }else{
                simbolo_concatenato = Udm_Etichetta.simbolo.split("/");
            }

            if(simbolo_concatenato
                && simbolo_concatenato.length === 2
                && (
                    obj.perHa_hl === enum_UnitaMisura.Ettolitro
                    || obj.perHa_hl === enum_UnitaMisura.Ettaro
                    || obj.perHa_hl === enum_UnitaMisura.Quintali
                    || obj.perHa_hl === enum_UnitaMisura.Tonnellate
                   )
                && Udm_Etichetta.codice !== 0){
              UdM = new UnitaDiMisura(obj.UDM_radice, "", simbolo_concatenato[0]);
              UdM.tipoControllo = Udm_Etichetta.tipoControllo;
            }

          FormulatiForm.patchValue({
            Dose_Hl: this.funzionicomuniservice.roundNumber(Dose_HL,this.qdcservice.getProductNumericSettings(UdM).decimals),
            Dose_Ha: this.funzionicomuniservice.roundNumber(Dose_HA,this.qdcservice.getProductNumericSettings(UdM,true).decimals),
            DoseTot_Ha: this.funzionicomuniservice.roundNumber(DoseTot_HA,this.qdcservice.getProductNumericSettings(UdM).decimals),
            flagTipoDose: flagTipoDose,
            flagDoseQuantitaTotale: flagDoseQuantitaTotale
          });

          if(UdM)
            this.qdcprodottiservice.Aggiorna_UdM(UdM);




        }else{
            FormulatiForm.patchValue({
                Dose_Hl: this.funzionicomuniservice.roundNumber(Dose_HL,this.qdcservice.getProductNumericSettings(UdM).decimals),
                Dose_Ha: this.funzionicomuniservice.roundNumber(Dose_HA,this.qdcservice.getProductNumericSettings(UdM,true).decimals),
                DoseTot_Ha: this.funzionicomuniservice.roundNumber(DoseTot_HA,this.qdcservice.getProductNumericSettings(UdM).decimals),
                flagTipoDose: flagTipoDose,
                flagDoseQuantitaTotale: flagDoseQuantitaTotale
            });

          if(UdM)
            this.qdcprodottiservice.Aggiorna_UdM(UdM);
        }

      if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){
        this.qdcservice.RicaricaTutteGridProdottixImpianti();
      }

}

//Deve replicare il cambioSoglia nella Trattamenti_2
changeSoglia(soglia_avversita: Soglia){

  this.Componi_Verifica_Soglia();

}

//Deve replicare il Btn_ComboEpocheDPI_Click nella Trattamenti_2
async changeEpocaDPI(epoca: Epoca){

  if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti){
    this.clearSezioneProdottiFormulatiForm(false,true,false,true,false);
  }else{
    this.clearSezioneProdottiFormulatiForm(false,true,false,true,true);
  }

  let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

  if(Disciplinare && Disciplinare.codice !== NessunDpi && Disciplinare.codice !== "2"){
    await this.getArray_Avversita();
  }
}

//Deve replicare il cambioAvversita nella Trattamenti_2

async changeAvversitaInsetti(Avversita_Selezionata: AvversitaGruppo){

  this.FormulatiForm.patchValue({
    Dose_Ha: 0,
    Dose_Hl: 0,
    DoseTot_Ha: 0
  });

  let Formulato: MultiColumnComboboxTrattamento  = this.FormulatiForm.get("Prodotto").value;

  if(!Formulato || Formulato.prodotto.codice === 0)
    return;

  if(!Avversita_Selezionata || Avversita_Selezionata.codice === 0)
    return;

  if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti)
    this.clearSezioneProdottiFormulatiForm(false,true,false,false,false);

}
async changeAvversita(Avversita_Selezionata: AvversitaGruppo){

  this.FormulatiForm.patchValue({
    Dose_Ha: 0,
    Dose_Hl: 0,
    DoseTot_Ha: 0
  });

  this.doseConsentitaDiserbo.D_HA_Max_Diserbo = 0;
  let DoseTrovata = false;
  let Formulato: MultiColumnComboboxTrattamento  = this.FormulatiForm.get("Prodotto").value;
  let Specie:  Specie | DestinazioneUso = this.qdcservice.TestataForm.get("Specie").value;

  let Disciplinare =  this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

  if(!Formulato || Formulato.prodotto.codice === 0)
    return;

  if(!Avversita_Selezionata || Avversita_Selezionata.codice === 0)
    return;

  this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

  if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti)
    this.clearSezioneProdottiFormulatiForm(false,true,false,false,false);

  if(
    Specie && Specie.codice !== 0 &&
    Disciplinare && Disciplinare.codice !== NessunDpiNessunaEtichetta
  ) {

    await this.Imposta_DoseConsentitaDiserbo(false);

    //Carico le Dosi Etichetta se scelgo un'Avversita

    this.FormulatiForm.patchValue({
      Dose_Etichetta: null
    });

    await this.getArray_Dosi();

    let Dose: MultiColumnComboboxDose_Etichetta = this.FormulatiForm.get("Dose_Etichetta").value;

    if(Dose && Dose.codice !== 0) {

      DoseTrovata = true;

      await this.qdcservice.Imposta_VolumiAcqua(Dose.AcquaMax,Dose.UdmAcqua,Dose.descrizione);

      let RilevamentoMagazzinoProdotto = this.qdcprodottiservice.getRilevamentoMagazzinoProdotto();

      let Udm_Da_Magazzino = -99;

      if(RilevamentoMagazzinoProdotto &&
        RilevamentoMagazzinoProdotto.udm && RilevamentoMagazzinoProdotto.udm.codice > 0)
        Udm_Da_Magazzino = RilevamentoMagazzinoProdotto.udm.codice;


      let obj = this.unitamisuraservice.ScomponiUdm(Dose.Udm);

      if(Udm_Da_Magazzino !== -99) {
        let v = 0;

        switch(obj.UDM_radice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Quintali:
            v =  enum_UnitaMisura.KG;
            break;
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.Litri:
            v =  enum_UnitaMisura.Litri;
            break;
        }

        if(Udm_Da_Magazzino === v) {
          await this.changeDoseEtichetta(Dose);
        }
      } else {
        await this.changeDoseEtichetta(Dose);
      }

      /* if(Udm_Da_Magazzino === -99){
          //dosi multiple
          let kg  = false;
          let l  = false;

          this.qdcformulatiservice.Array_Dosi.forEach(d=>{
              let obj = this.unitadimisuraservice.ScomponiUdm(d.Udm);

              switch(obj.UDM_radice){
                  case enum_UnitaMisura.Grammi:
                  case enum_UnitaMisura.Milligrammi:
                  case enum_UnitaMisura.KG:
                  case enum_UnitaMisura.Tonnellate:
                  case enum_UnitaMisura.Quintali:
                      kg = true;
                      break;
                  case enum_UnitaMisura.CentimetriCubi:
                  case enum_UnitaMisura.Millilitri:
                  case enum_UnitaMisura.Litri:
                      l = true;
                      break;
              }


          });
      } */

    }

    if(!DoseTrovata) {
      const LeggiUnitaDiMisura = <LeggiUnitaDiMisura> {
        dettaglioTrattamento: Formulato as DettaglioTrattamento
      };

      let udm: UnitaDiMisura = await this.unitamisuraservice.Recupera_UdM_da_FrCod(LeggiUnitaDiMisura);
      this.qdcprodottiservice.Aggiorna_UdM(udm);
    }
  }

  //Carico le Soglie Avversita

  this.FormulatiForm.patchValue({
    Soglia_Avversita: null
  });

  await this.getArray_Soglie_Avversita();

  await this.Componi_Riga_NoteProdottoLocalizzazione();

  //Caso Inneschi
  if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){

    if(Avversita_Selezionata) {

      if (Avversita_Selezionata.codice !== 0) {

        await this.qdcprodottiservice.Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(null,this.FormulatiForm.get("Avversita").getRawValue());

        if (
          Avversita_Selezionata.MagazziniMovimentazioni &&
          Avversita_Selezionata.MagazziniMovimentazioni.length > 0
        ) {
          this.Aggiorna_UdM_Innesco(Avversita_Selezionata.MagazziniMovimentazioni[0].udm);
        }else{
          //Imposto l'UdM del'Innesco se non è valorizzato da magazzino
          let udm = await this.getArray_UdM_Innesco(this.FormulatiForm.getRawValue());

          if(udm && udm.length === 1){
            this.Aggiorna_UdM_Innesco(udm[0]);
          }
        }
      }
    }
  }


}

//Oltre ai valori reimposto vuoti gli array dei controlli per la visibilità
clearSezioneProdottiFormulatiForm(clearFormArrayDosiProdotti: boolean, clearProdotto: boolean, changeDPI: boolean,clearAvversita: boolean,clearArray_Avversita: boolean){

  //Non resetto il form se sono in inserimento prodotti e ho già salvato la riga di prodotto
  let clear = true;

  if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FormulatiForm.get("Riga_Salvata").value)
    clear = false;

  if(clear){

    this.Array_Dosi = [];

    this.FormulatiForm.patchValue({
      Dose_Etichetta:null
    });

    this.clearSoglia_Avversita();

    if(clearAvversita)
      this.clearAvversita(clearArray_Avversita);

    if(changeDPI){

      this.qdcformulatiservice.Array_EpocheDPI = [];

      this.FormulatiForm.patchValue({
        EpocaDPI: null
      },{emitEvent: false});

      if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){

        let DosiProdotti = this.qdcservice.DosiProdottiFormArray(this.FormulatiForm,null);

        if(DosiProdotti && DosiProdotti.controls.length > 0){
          for(let i = 0; i< DosiProdotti.controls.length;i++){
            DosiProdotti.controls[i].patchValue({
              EpocaDPI: null
            },{emitEvent: false});
          }
        }

      }else{

        this.qdcservice.Sezione_ProdottoFormGroup(this.FormulatiForm.get("Operazione").value).patchValue({
          EpocaDPI: null
        },{emitEvent: false});

      }
    }

    if(clearProdotto){
      this.Array_Formulati = [];

      this.FormulatiForm.patchValue({
        Prodotto:null
      });

      this.Prima_Apertura_MultiColumn_Trattamento = false;
    }

    this.clearControlliMagazzinoInnesco();

    this.qdcprodottiservice.clearSezioneProdottiForm(clearFormArrayDosiProdotti);
  }

}

clearSoglia_Avversita(){
  this.Array_Soglie_Avversita = [];

  this.FormulatiForm.patchValue({
    Soglia_Avversita:null
  });
}

clearControlliMagazzinoInnesco(){
  this.FormulatiForm.patchValue({
    Magazzino_Innesco: null,
    Lotto_Innesco: "",
    Magazzino_Agenzia_Innesco: null,
    DoseTot_Ha_Innesco: 0
  });

  this.Aggiorna_UdM_Innesco(null);
}

clearAvversita(clearArray_Avversita: boolean){
  if(clearArray_Avversita)
    this.Array_Avversita = [];

  this.FormulatiForm.patchValue({
    Avversita:null
  });
}

Imposta_DoseConsentitaDiserbo(from_edit_prodotto: boolean): Promise<DoseConsentitaDiserbo>{

  return new Promise<DoseConsentitaDiserbo>((resolve,reject) => {

    this.doseConsentitaDiserbo.D_HA_Max_Diserbo = 0;
    this.doseConsentitaDiserbo.lbl_qta_residua = "";
    this.doseConsentitaDiserbo.Udm_Radice_HA = 0;
    this.doseConsentitaDiserbo.Lbl_Dose_Consigliata = "";
    this.doseConsentitaDiserbo.Div_DettaglioDoseConsentita = false;


    if(!from_edit_prodotto) {
      this.doseConsentitaDiserbo.percAbbDaApplicare = 0;
      this.doseConsentitaDiserbo.principiAttiviPercAbb = "";
      this.doseConsentitaDiserbo.Lbl_Dose_Consigliata2 = "";
    }

    let Operazione: Lavorazione = this.Operazione;
    let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);
    let Formulato_Selezionato = this.FormulatiForm.get("Prodotto").value as DettaglioTrattamento;

    if(
      (
        + Operazione.primaryKey.codice === enum_LAVCOD.DISERBO ||
        + Operazione.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO
      ) &&
      Disciplinare.codice !== NessunDpi &&
      Disciplinare.codice !== NessunDpiNessunaEtichetta &&
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice !== 0 &&
      Formulato_Selezionato && Formulato_Selezionato.prodotto.codice !== 0
    ) {

      //modifico la selezione degli rbl per salvare correttamente i dati dato che salvo sempre e solo la dose/ha
      this.FormulatiForm.patchValue({
        flagTipoDose: enum_TipoMezzo.Ettaro,
        flagDoseQuantitaTotale: enum_doseQuantitaTotale.Dose
      });

      let attivita: Attivita = null;

      if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
        attivita = JSON.parse(JSON.stringify(this.qdcformtoattivitaservice.mapQdCFormToListaAttivita(this.FormulatiForm,[])[0]));
      }else{

        //Se ho appena cliccato sull'edit prodotto quindi prodotto già salvato(flag from_edit_prodotto) gli passo tutto il form
        //della sezione con i prodotti salvati altrimenti se il from_edit_prodotto è false allora sto richiamando la funzione al cambio del Prodotto/Avversità.
        //Quindi devo escludere dal FormArray DosiProdotti il prodotto che sto inserendo/modificando.

        let Form = _.cloneDeep(this.qdcservice.Sezione_ProdottoFormGroup(this.FormulatiForm.get("Operazione").value));

        if(!from_edit_prodotto){
          let DosiProdotti = (Form.get("DosiProdotti") as FormArray).getRawValue().filter(d=>d.DosiProdottiGridrowId !== this.FormulatiForm.getRawValue().DosiProdottiGridrowId);

          (Form.get("DosiProdotti") as FormArray).clear();

          DosiProdotti.forEach(d=>{
            (Form.get("DosiProdotti") as FormArray).push(new FormControl(d));
          });
        }

        attivita = JSON.parse(JSON.stringify(this.qdcformtoattivitaservice.mapQdCFormToListaAttivita(Form,[])[0]));
      }

      const LeggiDoseConsentitaDiserbo  = <LeggiDoseConsentitaDiserbo> {
        attivita: attivita,
        dettaglioTrattamento: Formulato_Selezionato,
        fabbricato: (this.FormulatiForm.get("Magazzino_del_Prodotto_Selezionato").getRawValue() as Fabbricato),
        lotto: this.FormulatiForm.get("Lotto").getRawValue(),
        avversitaGruppo: (this.FormulatiForm.get("Avversita").value as AvversitaGruppo)
      };

      this.agendaservice.Imposta_DoseConsentitaDiserbo(LeggiDoseConsentitaDiserbo).then((dose: DoseConsentitaDiserbo)=>{

        if(!from_edit_prodotto){
          //Aggiorno il campo percentualeSuperficieTrattabile nel modello del principioattivo
          let multicolumcomboboxTrattamento:MultiColumnComboboxTrattamento =  this.FormulatiForm.get("Prodotto").value;

          if(dose.principiAttiviPercAbb && dose.principiAttiviPercAbb !== "" &&
            multicolumcomboboxTrattamento.principiAttivi && multicolumcomboboxTrattamento.principiAttivi.length > 0){

            let ListPrincipiAttivixPercAbb: Array<string> = dose.principiAttiviPercAbb.split("|");

            ListPrincipiAttivixPercAbb.forEach(strPrxPercAbb=>{
              if(strPrxPercAbb && strPrxPercAbb !== ""){
                let ListPrxPercAbb: Array<string> = strPrxPercAbb.split("§");

                if(ListPrxPercAbb && ListPrxPercAbb.length === 2){
                  multicolumcomboboxTrattamento.principiAttivi.forEach(p=>{
                    let codice_principio_attivo: number = +ListPrxPercAbb[0];

                    let percentuale_abbattimento: string = ListPrxPercAbb[1];

                    if(p.codice === codice_principio_attivo){
                      p.percentualeSuperficieTrattabile = parseFloat(percentuale_abbattimento.replace(/,/g, '.'));
                    }
                  });
                }
              }
            });
          }

          this.FormulatiForm.patchValue({
            Prodotto: multicolumcomboboxTrattamento
          })

          this.doseConsentitaDiserbo.percAbbDaApplicare = dose.percAbbDaApplicare;
          this.doseConsentitaDiserbo.principiAttiviPercAbb = dose.principiAttiviPercAbb;
          this.doseConsentitaDiserbo.Lbl_Dose_Consigliata2 = dose.Lbl_Dose_Consigliata2;
          this.gridimpiantiservice.AggiornaGridImpiantiDopoCambioPercentualeAbbatimento();
        }

        this.doseConsentitaDiserbo.D_HA_Max_Diserbo = dose.D_HA_Max_Diserbo;
        this.doseConsentitaDiserbo.lbl_qta_residua = dose.lbl_qta_residua;
        this.doseConsentitaDiserbo.Udm_Radice_HA = dose.Udm_Radice_HA;
        this.doseConsentitaDiserbo.Lbl_Dose_Consigliata = dose.Lbl_Dose_Consigliata;
        this.doseConsentitaDiserbo.Div_DettaglioDoseConsentita = dose.Div_DettaglioDoseConsentita;

        resolve(this.doseConsentitaDiserbo);
      });

    }else{

      resolve(this.doseConsentitaDiserbo);

    }
  });

}

setColumnComboboxFormulati() {
  //Configura le colonne della combobox
  //in base alla categoria di magazzino (elem_cod)
  //N.B. Tenere allineato con Obj_Empty_MultiColumnComboboxTrattamento

  let disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione?.primaryKey?.codice);
  this.ColumnComboboxFormulati = [];

  this.ColumnComboboxFormulati.push(
    new ColumnCombobox({
        field: "prodotto.descrizione",
        title: this.translocoService.translate('Prodotto')
      }
      /*,
      {
          width: 200
      }*/
    ),
    new ColumnCombobox({
        field: "prodotto.codice",
        title: this.translocoService.translate('CodiceProdotto')
      },
      {
        type: CELL_TYPES.NUMBER
      })
  )

  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI &&
    this.Lav_Cod != enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE){

    this.ColumnComboboxFormulati.push(new ColumnCombobox({
        field: "dataSmaltimentoScorte",
        title: this.translocoService.translate('FineScorta')
      },
      {
        type: CELL_TYPES.DATE,
        width: 80
      }));
  }

  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI &&
    this.Lav_Cod != enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE){

    //Mostro la colonna della Carenza solo se il disciplinare non è NessunDpiNessunaEtichetta
    if(disciplinare && disciplinare.codice !== NessunDpiNessunaEtichetta){
      this.ColumnComboboxFormulati.push(new ColumnCombobox({
          field: "tempoCarenza",
          title: this.translocoService.translate('Carenza')
        },
        {
          type: CELL_TYPES.NUMBER,
          formatNumbertolocal: true,
          digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal,
          width: 75
        }));
    }
  }

  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI &&
    this.Lav_Cod != enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE){

    this.ColumnComboboxFormulati.push(new ColumnCombobox(
      {
        field: "principiAttivi",
        title: this.translocoService.translate('SostanzeAttive')
      },
      {
        //width: 300,
        isArray: true,
        Arrayfield: "principiAttivi.descrizione",
      }
    ))
  }

  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI){
    this.ColumnComboboxFormulati.push(new ColumnCombobox({
        field: "classificazioni",
        title: this.translocoService.translate('Classificazione'),
      }
      /*,
      {
          width: 150
      }*/
    ));
  }


  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI &&
    this.Lav_Cod != enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE){

    //Mostro le colonne delle Bufferzone solo se il disciplinare non è NessunDpiNessunaEtichetta
    if(disciplinare && disciplinare.codice !== NessunDpiNessunaEtichetta){
      this.ColumnComboboxFormulati.push(new ColumnCombobox({
            field: "bufferzone.minimo",
            title: this.translocoService.translate('BufferZoneMin')
          },
          {
            type: CELL_TYPES.NUMBER,
            formatNumbertolocal: true,
            digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
          }),
        new ColumnCombobox({
            field: "bufferzone.massimo",
            title: this.translocoService.translate('BufferZoneMax')
          },
          {
            type: CELL_TYPES.NUMBER,
            formatNumbertolocal: true,
            digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
          })
      );
    }
  }

  if(this.Lav_Cod != LAVCOD_DISTRIBUZIONE_INSETTI &&
    this.Lav_Cod != enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE){

    this.ColumnComboboxFormulati.push(new ColumnCombobox({
        field: "Polverulento_Str",
        title: this.translocoService.translate('Polverulento')
      },
      {
                    width: 85
      }));
  }

  if(this.Lav_Cod === enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE ||
     this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(this.Lav_Cod)){

    this.ColumnComboboxFormulati.push(
      new ColumnCombobox({
          field: "durataFeromone",
          title: this.translocoService.translate('qdc.DurataFeromoneGG')
        },
        {
          width: 80,
          type: CELL_TYPES.NUMBER,
          formatNumbertolocal: true,
          digitsInfo: "1.0-0"
        }),
      new ColumnCombobox({
          field: "scadenzaFeromone",
          title: this.translocoService.translate('Scadenza')
        },
        {
          width: 80,
          type: CELL_TYPES.DATE
        }));
  }

  //Mostro le colonne del Magazzino solo se la GestioneMagazzino è Abilitata
  if (this.qdcprodottiservice.GestioneMagazzino_Abilitata()) {
    this.ColumnComboboxFormulati.push(
      new ColumnCombobox(
        {
          field: "Qta",
          title: this.translocoService.translate('GiacenzaAllaData')
        },
        {
          type: CELL_TYPES.NUMBER,
          isArray: true,
          Arrayfield: "MagazziniMovimentazioni.Qta",
          formatNumbertolocal: true,
          digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
        }
      ),
      new ColumnCombobox(
        {
          field: "QtaTot",
          title: this.translocoService.translate('GiacenzaTotale')
        },
        {
          type: CELL_TYPES.NUMBER,
          isArray: true,
          Arrayfield: "MagazziniMovimentazioni.QtaTot",
          formatNumbertolocal: true,
          digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
        }
      ),
      new ColumnCombobox(
        {
          field: "udm",
          title: this.translocoService.translate('um')
        },
        {
          isArray: true,
          Arrayfield: "MagazziniMovimentazioni.udm.simbolo",
          width: 72
        }
      ),
      new ColumnCombobox(
        {
          field: "Magazzino_descrizione",
          title: this.translocoService.translate('Magazzino')
        },
        {
          isArray: true,
          Arrayfield: "MagazziniMovimentazioni.Magazzino.descrizione"
        }
      )
    );

    //Colonna Lotto da Mostrare solo se la gestione lotti è abilitata
    if (this.qdcprodottiservice.MostraLotto() || this.FormulatiForm.get("Visualizza_Magazzini_Esterni").getRawValue()) {
      this.ColumnComboboxFormulati.push(
        new ColumnCombobox(
          {
            field: "Lotto",
            title: this.translocoService.translate('Lotto2')
          },
          {
            isArray: true,
            Arrayfield: "MagazziniMovimentazioni.Lotto",
            width: 200
          }
        )
      );
    }
  }

  if(+this.Operazione.primaryKey.codice === LAVCOD_DISTRIBUZIONE_INSETTI){
    this.ColumnComboboxFormulati.push(new ColumnCombobox(
      {
        field: "impollinatore",
        title: this.translocoService.translate('Impollinatore')
      },
      {
        type: CELL_TYPES.STRING
      }
    ));
  }

}

async changeInsetto(Formulato_Selezionato: MultiColumnComboboxTrattamento) {

  this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze();

  if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti){
    this.clearSezioneProdottiFormulatiForm(false,false,false,false,false);
  }else{
    this.clearSezioneProdottiFormulatiForm(false,false,false,true,true);
  }

  if(Formulato_Selezionato) {

    if(Formulato_Selezionato.prodotto.codice !== 0) {

      await this.qdcprodottiservice.Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(Formulato_Selezionato);

      this.Componi_Riga_NoteProdotto();

      if (Formulato_Selezionato.MagazziniMovimentazioni &&
        Formulato_Selezionato.MagazziniMovimentazioni.length > 0) {

        this.qdcprodottiservice.Aggiorna_UdM(Formulato_Selezionato.MagazziniMovimentazioni[0].udm);

      }
    }

    if(!this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti && !this.isImpollinatore())
      await this.getArray_Avversita();
    else if (this.isImpollinatore() && this.FormulatiForm.get("Avversita").value?.codice != -1) {
      this.FormulatiForm.get("Avversita").setValue({codice: -1, gruppo: { codice: - 1}})
    }
  }
}

async changeFormulato(Formulato_Selezionato: MultiColumnComboboxTrattamento){

  let obj_errore_gias_qdc = await this.qdcprodottiservice.Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(Formulato_Selezionato);

  if(obj_errore_gias_qdc && obj_errore_gias_qdc.list_errori_filtrati && obj_errore_gias_qdc.list_errori_filtrati.length > 0){
    this.FormulatiForm.patchValue({
      Prodotto: null,
    },{emitEvent: false});
    return;
  }

  this.doseConsentitaDiserbo = new DoseConsentitaDiserbo();
  this.Componi_Riga_NoteProdotto();
  this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze();

  if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti){
    this.clearSezioneProdottiFormulatiForm(false,false,false,false,false);
  }else{
    this.clearSezioneProdottiFormulatiForm(false,false,false,true,true);
  }

  if(Formulato_Selezionato) {

    if(Formulato_Selezionato.prodotto.codice !== 0){

      await this.qdcprodottiservice.Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(Formulato_Selezionato);

      this.Componi_Riga_NoteProdotto();

      if(
        Formulato_Selezionato.MagazziniMovimentazioni &&
        Formulato_Selezionato.MagazziniMovimentazioni.length > 0
      ) {
        this.qdcprodottiservice.Aggiorna_UdM(Formulato_Selezionato.MagazziniMovimentazioni[0].udm);
      }

      let LeggiDosi: boolean = true;
      let disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

      switch(Formulato_Selezionato.tipoFormulato){
        case enum_TipoFormulato.Coadiuvanti:
        case enum_TipoFormulato.Fitoregolatori:
        case enum_TipoFormulato.Disseccanti:
        case enum_TipoFormulato.Corroboranti_Fisiofarmaci:
          LeggiDosi = true;
          break;
        default:

          if(!this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti){
            await this.getArray_Avversita();

            LeggiDosi = false;

            //TODO Chiedere con la fede se seleziono un formulato e sono in questo caso el avversità
            //ce le ho in alto ed in automatico se ho solo un'avversità carico le dosi, ma in realtà nella Trattamenti_2
            //scatta il cambio avversità quando vengono caricate le dosi(come devo fare in questo caso?)


          }else{
            LeggiDosi = true;
          }
          break;
      }

      if(disciplinare?.codice === NessunDpiNessunaEtichetta){
        this.FormulatiForm.patchValue({
          Dose_Ha: 0,
          Dose_Hl: 0
        });

        LeggiDosi = false;
      }


      if(LeggiDosi){
        if(this.qdcservice.GetSpeciefromUtilizzoTerreno().codice !== 0 &&
          this.FormulatiForm.get("Prodotto").value.prodotto.codice !== 0 &&
          disciplinare.codice !== NessunDpiNessunaEtichetta){

          await this.Imposta_DoseConsentitaDiserbo(false);

          this.FormulatiForm.patchValue({
            Dose_Etichetta: null
          });

          await this.getArray_Dosi();
        }
      }

      this.gridimpiantiservice.AggiornaGridImpiantiXBufferzone();
    }

  }

}

setArrayDDLSezioneProdottiFormulatiForm(dataItem: any,setArrayAvversita: boolean){

  //Aggiorno anche gli Array che sono nel prodotti service così scattano gli ngIf che permettono
  //la visualizzazione di alcune ddl solo se hanno degli elemeneti

  this.qdcprodottiservice.setArrayDDLSezioneProdottiForm(dataItem);

  this.Array_Formulati = [];

  if(setArrayAvversita)
    this.Array_Avversita = [];

  this.Array_Soglie_Avversita = [];
  this.Array_Dosi = [];

  if(dataItem.Prodotto && dataItem.Prodotto.prodotto.codice > 0)
    this.Array_Formulati = [dataItem.Prodotto];

  if(setArrayAvversita && dataItem.Avversita && (dataItem.Avversita.codice > 0 || dataItem.Avversita.codice === -1))
    this.Array_Avversita = [dataItem.Avversita];

  if(dataItem.Soglia_Avversita && dataItem.Soglia_Avversita.codice > 0)
    this.Array_Soglie_Avversita = [dataItem.Soglia_Avversita];

  if(dataItem.Dose_Etichetta && dataItem.Dose_Etichetta.codice > 0)
    this.Array_Dosi = [dataItem.Dose_Etichetta];

}

async MostraMessaggiBottoniSenzaGridDosiProdottiFormulatiForm(){

  let FormulatiFormValue = this.FormulatiForm.getRawValue();

  if(FormulatiFormValue){

    await this.Componi_Verifica_Soglia();

    this.Componi_Lbl_Dose_Consigliata2_SuperficieTrattabile(FormulatiFormValue);

  }

}

private Componi_Lbl_Dose_Consigliata2_SuperficieTrattabile(FormulatiFormValue: any){
  //Questa funzione viene richiamata al caricamento del componente del dettagli-formulati e mostra l'eventuale
  //percentuale di abbattimento (superficie trattabile) senza chiamata al web service

  let PercAbbMin: number = 0;

  PercAbbMin = this.qdcservice.OttieniPercentualeAbbattimentoDiserboDisseccamento(FormulatiFormValue.Prodotto, FormulatiFormValue.Operazione);

  if(PercAbbMin > 0 && PercAbbMin < 100){

    this.doseConsentitaDiserbo.principiAttiviPercAbb = "";

    this.doseConsentitaDiserbo.percAbbDaApplicare = PercAbbMin;

    this.doseConsentitaDiserbo.Lbl_Dose_Consigliata2 = "<FONT color=red><strong>" + this.translocoService.translate("qdc.AttenzioneEPossibileIntervenireAlMassimo",{Percentuale: this.doseConsentitaDiserbo.percAbbDaApplicare}) +"</strong> </FONT>";

  }
}

async MostraMessaggiBottoniEditProttoFormulatiForm(){

  let FormulatiFormValue = this.FormulatiForm.getRawValue();

  if(FormulatiFormValue){

    this.Componi_Riga_NoteProdotto();

    await this.Componi_Riga_NoteProdottoLocalizzazione();

    await this.Imposta_DoseConsentitaDiserbo(true);

  }

}

Componi_Riga_NoteProdotto(){

  this.Riga_NoteProdotto = "";

  let Formulato_Selezionato: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

  if(Formulato_Selezionato  && Formulato_Selezionato.prodotto.codice !== 0){

    if(Formulato_Selezionato.inRevisione && Formulato_Selezionato.inRevisione !== ""){
      if(Formulato_Selezionato.dataAttoNormativo && Formulato_Selezionato.dataAttoNormativo.getTime() !== AGRODATAINIZIO.getTime()){
        this.Riga_NoteProdotto = this.translocoService.translate('qdc.IlProdottoSelezionatoInRevisionedaDecreto', { DataAttoNormativo: this.datepipe.transform(Formulato_Selezionato.dataAttoNormativo, 'dd/MM/yyyy')});
      }else{
        this.Riga_NoteProdotto = this.translocoService.translate('qdc.IlProdottoSelezionatoInRevisione');
      }
    }

    switch(this.qdcservice.get_Stato_Cod_from_ImpiantiSelezionati()){
      case "FR":
        this.Riga_NoteProdotto = this.translocoService.translate('qdc.ProdottoCommercializzatoinFrancia');
        break;
      case "BR":
        this.Riga_NoteProdotto = this.translocoService.translate('qdc.ProdottoCommercializzatoinBrasile');
        break;
      default:
        break;
    }
  }

}

async Componi_Riga_NoteProdottoLocalizzazione(){

  let Formulato_Selezionato: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

  let Avversita_Selezionata: DropdownListAvversita = this.FormulatiForm.get("Avversita").value;

  if(Formulato_Selezionato && Formulato_Selezionato.prodotto.codice !== 0 &&
    Avversita_Selezionata && Avversita_Selezionata.codice !== 0){

    let DescrizioneLocalizzazione = await this.getDescrizioneLocalizzazione();

    if(DescrizioneLocalizzazione !== ""){
      if(this.Riga_NoteProdotto !== ""){
        this.Riga_NoteProdotto += ". " + DescrizioneLocalizzazione;
      }else{
        this.Riga_NoteProdotto = DescrizioneLocalizzazione;
      }
    }

  }

}

async Componi_Verifica_Soglia(){
  this.Verifica_Soglia = "";

  let Soglia_avversita_scelta: Soglia = this.FormulatiForm.get("Soglia_Avversita")?.value;

  if(Soglia_avversita_scelta && Soglia_avversita_scelta.codice !== 0){
    if(this.qdcservice.Blocca_Soglia) {
      const LeggiAvversita = <LeggiAvversita>{
        soglia: Soglia_avversita_scelta,
        impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
        // prodottiDaTrattare: this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value?.map(x => x.giacenzaMagazzino),
        appezzamenti: this.qdcservice.GetAppezzamentiSelezionatiModel(),
        data: this.qdcservice.TestataForm.get("Data").value,
        specie: this.qdcservice.GetSpeciefromUtilizzoTerreno()
      };

      let strVerificaSoglia = await this.avversitaservice.Controlla_Soglia_Avversita_Soddisfatta_QdC(LeggiAvversita);

      if(strVerificaSoglia && strVerificaSoglia !== "")
        this.Verifica_Soglia = this.translocoService.translate('qdc.Soglia_non_ancora_rilevata_sugli_app', { Appezzamenti: strVerificaSoglia});
    }
  }


}

async componiLblDosiCompatibili(leggi_dosi: boolean){

  this.Str_Dosi_Compatibili = "";

  let doseEtichetta: MultiColumnComboboxDose_Etichetta = this.FormulatiForm.get("Dose_Etichetta")?.value;

  if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && doseEtichetta && doseEtichetta.Codice_Concatenato !== ""){
    let array_dosi: Array<MultiColumnComboboxDose_Etichetta> = this.Array_Dosi;

    if(leggi_dosi)
      array_dosi = await this.getArray_Dosi(false);

    let dosiEtichetta: MultiColumnComboboxDose_Etichetta[]= this.griddosiprodottiservice.getDosi_Etichetta(array_dosi,this.FormulatiForm.get("Dose_Etichetta").value);

    if(dosiEtichetta && dosiEtichetta.length > 1){
      let [, ...dosiCompatibili]: MultiColumnComboboxDose_Etichetta[] = dosiEtichetta;

      if(dosiCompatibili && dosiCompatibili.length > 0){
        let Descrizione = dosiCompatibili.map((d: MultiColumnComboboxDose_Etichetta) =>
          d.Descrizione_Concatenata
        ).join(",");

        this.Str_Dosi_Compatibili = this.translocoService.translate("qdc.ControlloAttivoAncheSullaDose",{DosiCompatibili: Descrizione});
      }
    }
  }


}

/*
* @description: Forzo il Caricamento dell' Avversita, Soglia, Dose durante il ribaltamento di una ricetta da APP perchè potrebbe non arrivarmi da APP
* ma il prodotto potrebbe richiederlo
*/
async Carica_Controlli_Ribaltamento_in_Agenda_Di_Ricetta_da_APP_Dettagli_Formulati(){

  if(this.qdcservice.Is_Ribaltamento_Ricetta_Da_Origine_Diversa() &&
            (!this.FormulatiForm.get("Problema_DettaglioProdotto_Da_Risolvere").getRawValue() || this.FormulatiForm.get("Problema_DettaglioProdotto_Da_Risolvere").getRawValue() === enum_Problema_DettaglioProdotto.Nessuno)){

    let imposta = this.qdcprodottiservice.Imposta_In_Automatico_Elemento_alla_DDL();

    await this.Carica_DDL_Formulati(imposta);
  }

}

/*
* @description: Carico le ddl dei formulati per mostrarle
* */
async Carica_DDL_Formulati(imposta: boolean){
  let sezione_formulato: any = this.FormulatiForm.getRawValue();

  if(sezione_formulato){

    let promise_Array: Promise<any>[] = [];

    let avversita: DropdownListAvversita = sezione_formulato.Avversita;

    if(!avversita || avversita?.Codice_Concatenato === "" || !this.Array_Avversita ||this.Array_Avversita.length === 0){
      promise_Array.push(this.getArray_Avversita(imposta));
    }

    let soglia: Soglia = sezione_formulato.Soglia_Avversita;

    if(!soglia || soglia.codice === 0 || !this.Array_Soglie_Avversita ||this.Array_Soglie_Avversita.length === 0){
      promise_Array.push(this.getArray_Soglie_Avversita(imposta));
    }

    let dose: MultiColumnComboboxDose_Etichetta = sezione_formulato.Dose_Etichetta;

    if(!dose || dose.Codice_Concatenato === "" || !this.Array_Dosi ||this.Array_Dosi.length === 0){
      promise_Array.push(this.getArray_Dosi(true,imposta));
    }

    await Promise.all(promise_Array);
  }
}

RicercaFormulati(impostaInAutomaticoPrimoElementoAllaDDL: boolean, apriInAutomaticolaDDL: boolean){

  let Prodotto = this.FormulatiForm.get("Prodotto").value;

  let descrizione = "";

  if(Prodotto){

    if(Prodotto.prodotto.codice === this.qdcservice.Obj_Empty_MultiColumnComboboxTrattamento.prodotto.codice){
      descrizione = Prodotto.Descrizione_Concatenata;
    }else{
      descrizione = Prodotto.prodotto.descrizione;
    }

  }

  let listerroriGias = this.qdcprodottiservice.RicercaProdotti(descrizione);

        if(!listerroriGias || listerroriGias.length === 0){

          let EpocaDPI: Epoca = this.FormulatiForm.get("EpocaDPI").getRawValue();

          let Operazione: Lavorazione = this.FormulatiForm.get("Operazione").getRawValue();

          this.qdcformulatiservice.Controlla_EpocaDPI(EpocaDPI,Operazione,listerroriGias);

          if(listerroriGias && listerroriGias.length > 0)
            this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();

        }

  if(listerroriGias && listerroriGias.length > 0){
    this.Array_Formulati = [];

  }else{

    UtilityFunctions.loadDropDownItems(this.qdcprodottiservice.MultiColumnComboboxProdotti,this.getArray_Formulati(descrizione,impostaInAutomaticoPrimoElementoAllaDDL)).then(r=>{
      if(apriInAutomaticolaDDL){
        if(!this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.isOpen){
          this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.focus();
          this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.toggle(true);
        }
      }
    });


  }


}

public isImpollinatore(): boolean {
  return (this.FormulatiForm.get("Prodotto").value == undefined ? true : this.FormulatiForm.get("Prodotto").value?.isImpollinatore);
}

/*
* @description: Gestisce la visibilità delle ddl della sezione formulati per decidere se mostrarle/caricarle
* */
MostraNascondiDDLSezioneFormulati(FormControlName: string){
  let mostra = false;

  let lav_cod:number = +this.Operazione.primaryKey.codice;

  let disciplinare: Disciplinare = this.qdcservice.getDisciplinareModelValue(lav_cod);

  switch(FormControlName){
    case "Avversita":
      if(lav_cod === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO ||
        lav_cod === enum_LAVCOD.DISERBO ||
        lav_cod === enum_LAVCOD.GEODISINFESTAZIONE ||
        lav_cod === enum_LAVCOD.CONCIA_SEME ||
        lav_cod === enum_LAVCOD.DISTRIBUZIONE_INSETTI ||
        lav_cod === enum_LAVCOD.CONFUSIONE_DISORIENTAMENTO_SESSUALE ||
        lav_cod === enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA ||
        this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)){

        mostra = true;

      }
      break;
    case "Soglia_Avversita":
      //Deve essere scelto anche un disciplinare
      if (this.Categoria_Magazzino === FORMULATI &&
        (lav_cod === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO ||
          lav_cod === enum_LAVCOD.DISERBO ||
          lav_cod === enum_LAVCOD.GEODISINFESTAZIONE ||
          lav_cod === enum_LAVCOD.CONCIA_SEME) &&
        disciplinare.codice !== NessunDpiNessunaEtichetta &&
        disciplinare.codice !== NessunDpi &&
        disciplinare.codice !== DpiBio &&
        this.FormulatiForm.get("Prodotto").value &&
        this.FormulatiForm.get("Prodotto").value.prodotto.codice !== 0
      ) {
        mostra = true;
      }
      break;
    case "Dose_Etichetta":
      if(this.Categoria_Magazzino === FORMULATI &&
        this.qdcservice.GetSpeciefromUtilizzoTerreno().codice !== 0 &&
        this.FormulatiForm.get("Prodotto").value &&
        this.FormulatiForm.get("Prodotto").value.prodotto.codice !== 0 &&
        disciplinare.codice !== NessunDpiNessunaEtichetta){
        mostra = true;
      }
      break;
  }

  return mostra;
}

RicercaInsetti(){

  let Prodotto = (this.FormulatiForm.get("Prodotto").value as MultiColumnComboboxTrattamento);
  let descrizione = "";

  if(Prodotto) {
    if (Prodotto.prodotto.codice === this.qdcservice.Obj_Empty_MultiColumnComboboxTrattamento.prodotto.codice) {
      descrizione = Prodotto.Descrizione_Concatenata;
    } else {
      descrizione = Prodotto.prodotto.descrizione;
    }
  }

  let listerroriGias = this.qdcprodottiservice.RicercaProdotti(descrizione);

  if(listerroriGias && listerroriGias.length > 0){
    this.Array_Formulati = [];
  }else{
    UtilityFunctions.loadDropDownItems(this.qdcprodottiservice.MultiColumnComboboxProdotti, this.getArray_Insetti(descrizione));
  }
    }

    public PlaceHolderRicercaFormulati(): string {

        let placeholder = this.qdcprodottiservice.PlaceHolderRicercaProdotti();

        let elem_cod: number = this.FormulatiForm.get("Categoria_Magazzino").getRawValue()

        switch(elem_cod){
          case FORMULATI:

            let EpocaDPI: Epoca = this.FormulatiForm.get("EpocaDPI").getRawValue();

            let Operazione: Lavorazione = this.FormulatiForm.get("Operazione").getRawValue();

            let listerroriGias: ErroreGias[] = [];

            this.qdcformulatiservice.Controlla_EpocaDPI(EpocaDPI, Operazione, listerroriGias);

            if(listerroriGias && listerroriGias.length === 1)
              placeholder += " (" + listerroriGias[0].messaggio + ")";

            break;
          case INSETTI:
            break;
        }

        return placeholder;
}

SalvaDoseFormulato(Nuova_Riga: boolean){

  return new Promise<any>(async(resolve,reject) => {

    let EpocaDPI: Epoca = this.FormulatiForm.get("EpocaDPI").value;

    let Array_Dosi: Array<MultiColumnComboboxDose_Etichetta> = this.Array_Dosi;

    let EpocaFertilizzazione: Epoca = null;

    let Utilizza_Direttiva_Nitrati: boolean = false;

    let Direttiva_Nitrati: Disciplinare = null;

    let Opzioni_Semina: BaseCodeDescr = null;

    let tipo_operazione_db = enum_TipoOperazioneDB.Modifica;

    let DosiProdottiGridrowId =  this.FormulatiForm.getRawValue().DosiProdottiGridrowId;

    let DosiProdottiFormArray = this.qdcservice.DosiProdottiFormArray(null,this.Operazione);

    let DosiProdotti:Array<any> = DosiProdottiFormArray.getRawValue();

    let Modalita_Applicazione: BaseCodeDescr =  this.FormulatiForm.get("Modalita_Applicazione").value;

    let QuantitaSuImpianti: QuantitaSuImpianto[] = this.FormulatiForm.get("QuantitaSuImpianti").getRawValue();

    let Ripartizione_Trappole: BaseCodeDescr = this.FormulatiForm.get("Ripartizione_Trappole").getRawValue();

    let row: FormGroup = this.griddosiprodottiservice.getRigaGridDosiProdotti(
      DosiProdotti,
      DosiProdottiGridrowId,
      this.FormulatiForm,
      null,
      null,
      this.Operazione,
      this.qdcservice.TestataForm.get("Data").value,
      this.FormulatiForm.get("flagTipoDose").value,
      this.FormulatiForm.get("flagDoseQuantitaTotale").value,
      this.qdcservice.AcquaForm.get('Dose_Acqua').value,
      Array_Dosi,
      EpocaDPI,
      EpocaFertilizzazione,
      Utilizza_Direttiva_Nitrati,
      Direttiva_Nitrati,
      Opzioni_Semina,
      Modalita_Applicazione,
      QuantitaSuImpianti,
      Ripartizione_Trappole,
      false,
      false,
      false
    );

    let index = DosiProdotti.findIndex((r: any) => r.DosiProdottiGridrowId === DosiProdottiGridrowId);

    if(index > -1){
      this.qdcservice.AggiornaFormArrayGridDosiProdotti(row.getRawValue(),index,this.Operazione,enum_TipoOperazioneDB.Modifica,null,false);
    }


    if(Nuova_Riga){
      DosiProdotti = DosiProdotti.filter(d=>d.DosiProdottiGridrowId !== DosiProdottiGridrowId);

      tipo_operazione_db = enum_TipoOperazioneDB.Scrittura;
    }

    let listerroriGias = await this.gridcontrolliservice.ControllaSeInserireDoseProdotto(DosiProdotti,
      row.getRawValue(),
      row,
      tipo_operazione_db,
      DosiProdottiGridrowId,
      this.doseConsentitaDiserbo,
      this.Array_Soglie_Avversita,
      this.Array_Dosi);

    if(listerroriGias.length === 0){


      if(this.Categoria_Magazzino === FORMULATI)
        this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.FormulatiForm);

    }

    resolve({ listerroriGias: listerroriGias, Row: row.getRawValue() });

  });
}

AbilitaDisabilitaDoseHaDoseTot(){

  let lav_cod: number = + (this.FormulatiForm.get("Operazione").getRawValue() as Lavorazione).primaryKey.codice;

  let Riga_Salvata: boolean = this.FormulatiForm.get("Riga_Salvata").getRawValue();

  if(!Riga_Salvata &&
    this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)){

    let ripartizione_trappole: BaseCodeDescr = this.FormulatiForm.get("Ripartizione_Trappole").getRawValue();

    if(ripartizione_trappole){
      switch(ripartizione_trappole.codice){
        case enum_Ripartizione_Trappole.Manuale:
          this.FormulatiForm.get("Dose_Ha").disable();
          this.FormulatiForm.get("DoseTot_Ha").disable();
          break;
        case enum_Ripartizione_Trappole.Automatica:
          this.FormulatiForm.get("Dose_Ha").enable();
          this.FormulatiForm.get("DoseTot_Ha").enable();
          break;
      }
    }
  }
}

RicaricaGridProdottiXImpianti(moltiplicatore: number = 1){

  if(this.GridProdottiXImpiantiPublicService.value){

    let data: GridProdottixImpiantiServerResult = cloneDeep(this.GridProdottiXImpiantiPublicService.value.data);

    let columns: KendoGridColumn[] = data.columns;

    this.GridProdottiXImpiantiHttpService.Moltiplicatore_Qta = moltiplicatore;

    if(columns){
      let index = columns.findIndex(c=>c.field === "Dose_Ha");

      if(index > -1)
        this.GridProdottiXImpiantiPublicService.refresh(true);
    }


  }
}

  flag_LeggiAvversita(): boolean{

    let Prodotto: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

    let leggiAvversita: boolean = false;

    if(this.MostraNascondiDDLSezioneFormulati('Avversita')){
      if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
        //Nel caso di avversità prima dei prodotti carico sempre le avversità anche se dopo l'utente potrebbe scegliere un Coadiuvanti,
        //Fitoregolatori, Disseccanti o Corroboranti_Fisiofarmaci

        leggiAvversita = true;
      } else {
        if (Prodotto?.tipoFormulato !== enum_TipoFormulato.Coadiuvanti &&
          Prodotto?.tipoFormulato !== enum_TipoFormulato.Fitoregolatori &&
          Prodotto?.tipoFormulato !== enum_TipoFormulato.Disseccanti &&
          Prodotto?.tipoFormulato !== enum_TipoFormulato.Corroboranti_Fisiofarmaci) {

          //nel caso filtro prod --> avv se non ho scelto un formulato non carico nulla
          if (!Prodotto ||
            Prodotto.prodotto.codice === 0) {
            this.Array_Avversita = [];
          } else {
            leggiAvversita = true;
          }

        } else {
          this.Array_Avversita = [];
        }
      }
    }

    return  leggiAvversita;
  }

  async getArray_Avversita_Innesco(impostaInAutomaticoPrimoElementoAllaDDL: boolean = true){

    let Prodotto: MultiColumnComboboxTrattamento = this.FormulatiForm.get("Prodotto").value;

    const LeggiAvversita = <LeggiAvversita>{
      tipoAttivita: this.qdcservice.TestataForm.get('Tipo').value,
      statoAttivita: this.qdcservice.TestataForm.get('Stato').value,
      lavorazione: this.Operazione,
      impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
      specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
      disciplinare: this.qdcservice.getDisciplinareModelValue(+this.Operazione.primaryKey.codice),
      epocaDPI: this.FormulatiForm.get('EpocaDPI').value,
      data: this.qdcservice.TestataForm.get('Data').value,
      dettaglioTrattamento: null,
      visualizzaMovimentiMagazzino: !this.FormulatiForm.get('Innesco_Incluso').value,
      escludiGiacenzeZero: this.FormulatiForm.get('Visualizza_Solo_Inneschi_in_Giacenza').value,
      magazziniAgenzie: false
    };

    //Passo anche il dettaglioTrattamento se sono nel caso Prodotto -> Avversita
    if (!this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
      LeggiAvversita.dettaglioTrattamento = (Prodotto as DettaglioTrattamento);
    }

    let risp = (await this.avversitaservice.Leggi_Avversita_Inneschi(LeggiAvversita)) as MultiColumnComboboxAvversitaInnesco [];

    if (risp) {
      let rispMapped = risp.map(a => {
        a = this.qdcservice.setCodDescrConcatenataMultiColumnComboboxAvversitaInnesco(a as MultiColumnComboboxAvversitaInnesco,this.Lav_Cod);
      });

      this.Array_Avversita = risp;

      // Se c'è solo una Avversità (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
      // lo imposto come default e carico le Dosi Etichetta
      if (this.Array_Avversita.length === 1 && impostaInAutomaticoPrimoElementoAllaDDL) {

        let Avversita_Selezionata: DropdownListAvversita = this.FormulatiForm.get('Avversita').value;

        if (Avversita_Selezionata?.Codice_Concatenato !== this.Array_Avversita[0].Codice_Concatenato) {
          this.FormulatiForm.patchValue({
            Avversita: this.Array_Avversita[0],
          });

          await this.changeAvversita(this.Array_Avversita[0]);
        }

      }

    }
  }

  setColumnComboboxAvversitaInneschi() {

    //Configura le colonne della combobox
    this.ColumnComboboxAvversitaInneschi = [];

    if(this.qdcservice.TestataForm.get("Operazioni").getRawValue().findIndex((o: Lavorazione)=>
      this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ o.primaryKey.codice)) > -1){

      this.ColumnComboboxAvversitaInneschi.push(
        new ColumnCombobox({
            field: "Descrizione_Concatenata",
            title: this.translocoService.translate('lbl_avversitaResource1.Text')
          }
        )
      );

      //Mostro le colonne del Magazzino solo se la GestioneMagazzino è Abilitata
      if (this.qdcservice.GestioneMagazzino_Abilitata_Inneschi &&
          !this.FormulatiForm.get("Innesco_Incluso").getRawValue()) {

        this.ColumnComboboxAvversitaInneschi.push(
          new ColumnCombobox(
            {
              field: "Qta",
              title: this.translocoService.translate('GiacenzaAllaData')
            },
            {
              type: CELL_TYPES.NUMBER,
              isArray: true,
              Arrayfield: "MagazziniMovimentazioni.Qta",
              formatNumbertolocal: true,
              digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
            }
          ),
          new ColumnCombobox(
            {
              field: "QtaTot",
              title: this.translocoService.translate('GiacenzaTotale')
            },
            {
              type: CELL_TYPES.NUMBER,
              isArray: true,
              Arrayfield: "MagazziniMovimentazioni.QtaTot",
              formatNumbertolocal: true,
              digitsInfo: this.qdcservice.digitsInfo_QdC_4_Decimal
            }
          ),
          new ColumnCombobox(
            {
              field: "udm",
              title: this.translocoService.translate('um')
            },
            {
              isArray: true,
              Arrayfield: "MagazziniMovimentazioni.udm.simbolo",
              width: 72
            }
          ),
          new ColumnCombobox(
            {
              field: "Magazzino_descrizione",
              title: this.translocoService.translate('Magazzino')
            },
            {
              isArray: true,
              Arrayfield: "MagazziniMovimentazioni.Magazzino.descrizione"
            }
          )
        );

        //Colonna Lotto da Mostrare solo se la gestione lotti è abilitata
        if (this.qdcservice.GestioneLotti_Inneschi) {
          this.ColumnComboboxAvversitaInneschi.push(
            new ColumnCombobox(
              {
                field: "Lotto",
                title: this.translocoService.translate('Lotto2')
              },
              {
                isArray: true,
                Arrayfield: "MagazziniMovimentazioni.Lotto",
                width: 80
              }
            )
          );
        }
      }

    }

  }

  async getArray_UdM_Innesco(FormValue: any,impostaArrayallaDDL: boolean = true) {

        let avversita: AvversitaGruppo = (FormValue?.Avversita as AvversitaGruppo);

        const LeggiUnitaDiMisura: LeggiUnitaDiMisura = {
            lavorazione: FormValue.Operazione,
            elem_cod: INNESCHI,
            avversita: avversita,
            tipo_Attivita: this.qdcservice.TestataForm.get("Tipo").value,
            tipo_Ricetta: this.qdcservice.TestataForm.get("TipoRicetta").value,
            dettaglioTrattamento: null,
            dettaglioFertilizzazione: null,
            dettaglioSemina: null,
            doseEtichetta: null,
            unitaDiMisura: FormValue.UdM_Innesco
        };

        let risp: Array<UnitaDiMisura> = await this.unitamisuraservice.Leggi_UnitaDiMisura_QdC(LeggiUnitaDiMisura);

        if (impostaArrayallaDDL)
            this.Array_Udm_Inneschi = risp;

        return risp;
  }

  private Get_UnitaDiMisura_Innesco(unita: UnitaDiMisura): number {

    let final = 0;

    if (this._unita_innesco && unita && this._unita_innesco?.codice != unita?.codice) {

        final = this.qdcUnitMisuraService.Get_Moltiplicatore(this._unita_innesco,unita);

    }else{
        final = 1;
    }

   this._unita_innesco = unita;

    return final;
  }

  changeUdM_Innesco(unitaDiMisura: UnitaDiMisura){

    let unit = this.Get_UnitaDiMisura_Innesco(unitaDiMisura);

    let valTot = this.FormulatiForm.get("DoseTot_Ha_Innesco").value;

    valTot = valTot.toFixed(6) * unit;

    valTot = this.funzionicomuniservice.roundNumber(valTot,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals);

    this.FormulatiForm.patchValue({
      DoseTot_Ha_Innesco: valTot
    },{emitEvent: false});
  }

  changeInnesco_Incluso(){

    this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

    this.clearAvversita(false);

    this.clearControlliMagazzinoInnesco();

    this.setColumnComboboxAvversitaInneschi();

    //Apro la DDL delle Avversità dell'Innesco in automatico se non è già aperta e se non è disabilitata
    UtilityFunctions.loadDropDownItems(this.MultiColumnComboboxAvversitaInneschi,this.getArray_Avversita_Innesco(false)).then(r=>{

      if(!this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.isOpen &&
         !this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.isDisabled){

        this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.focus();
        this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.toggle(true);
      }
    });

  }

  changeVisualizza_Solo_Inneschi_in_Giacenza(){

    this.clearAvversita(false);

    this.clearControlliMagazzinoInnesco();

    //Apro la DDL delle Avversità dell'Innesco in automatico se non è già aperta e se non è disabilitata
    UtilityFunctions.loadDropDownItems(this.MultiColumnComboboxAvversitaInneschi,this.getArray_Avversita_Innesco()).then(r=>{

      if(!this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.isOpen &&
         !this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.isDisabled){

        this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.focus();
        this.MultiColumnComboboxAvversitaInneschi.Multicolumncombobox.toggle(true);
      }
    });
  }

}
