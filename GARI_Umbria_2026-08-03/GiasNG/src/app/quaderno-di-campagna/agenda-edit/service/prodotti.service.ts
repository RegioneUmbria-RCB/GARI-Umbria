import {Injectable, OnDestroy} from "@angular/core";
import {FormArray, FormGroup} from "@angular/forms";
import {RilevamentoDiMagazzino} from "app/Model/attivita/RilevamentoDiMagazzino";
import {UnitaDiMisura} from "app/Model/metaschema/UnitaDiMisura";
import {
  enum_Gestione_Giacenze,
  enum_Gestione_Lotti,
  enum_LAVCOD,
  enum_PUARegolamenti_Tipo
} from "app/Model/TipiEnumerativi";
import {
    FabbricatiService,
    LeggiMagazzini_QdC,
    LeggiUltimo_Magazzino_Prodotto_Movimentato
} from "app/Service/Anagrafica/fabbricati.service";
import {LeggiUnitaDiMisura, UnitaDiMisuraService} from "app/Service/Metaschema/UnitaDiMisura.service";
import {BehaviorSubject, lastValueFrom, map, Observable, Subscription} from "rxjs";
import {
  Controlli_Giacenze_Lotti,
  DropdownListMagazzino, MultiColumnComboboxAvversitaInnesco,
  MultiColumnComboboxFertilizzazione,
  MultiColumnComboboxSemina,
  MultiColumnComboboxTrattamento, Obj_Errore_Gias_QdC
} from "../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {QdCService} from "./qdc.service";
import {
  DpiBio,
  FERTILIZZANTI,
  FORMULATI, INNESCHI,
  INSETTI,
  NessunDpiNessunaEtichetta,
  SEMENTI
} from '../../../Model/CostantiPersonalizzate';
import {DettaglioTrattamento} from "../../../Model/attivita/dettagli/DettaglioTrattamento";
import {DettaglioSemina} from "../../../Model/attivita/dettagli/DettaglioSemina";
import {DettaglioFertilizzazione} from "../../../Model/attivita/dettagli/DettaglioFertilizzazione";
import {DoseEtichetta} from "../../../Model/metaschema/DoseEtichetta";
import {
  enum_ErroreGias_Tipo,
  ErroreGias,
  ErroreGias_Severity,
} from "../../../Service/master.service";
import {TranslocoService} from "@jsverse/transloco";
import { GiasMultiColumnComboboxTemplateComponent } from 'gias-ui-kit';
import {Lavorazione} from "../../../Model/attivita/Lavorazione";
import {CalcoloSuperficiService} from "./calcolo-superfici.service";
import {GridImpiantiService} from "./grid-impianti/grid-impianti.service";
import {RibaltamentoTypes} from "../../../menu-agenda/components/utils";
import {enum_Impostazioni_Utenti} from "../../../Model/Impostazioni_Utenti.enum";
import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";
import {AgendaService, Leggi_Modalita_Applicazione} from "../../../Service/Agenda/Agenda.service";
import {RispostaStandard} from "../../../Service/api.service";
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";
import {AvversitaGruppo} from "../../../Model/metaschema/avversita/AvversitaGruppo";

@Injectable()
export class QdCProdottiService implements OnDestroy {

    Subs: Subscription = new Subscription();

    //Questa Form cambia in base alle istanze del FormArray
    public ProdottiForm: FormGroup;

    public ObsProdottiForm = new BehaviorSubject(null);

    public ObsMsg_Seleziona_Impianti_Prodotti = new BehaviorSubject(true);

    public btn_Inserisci_Modifica_Prodotto_Text: string = this.translocoService.translate('qdc.InserisciProdottoInMiscela');

    public InserisciDoseProdotto = new BehaviorSubject(false);

    Array_Magazzini: Array<DropdownListMagazzino> = [];

    Array_UdM: Array<UnitaDiMisura> = [];

    private _GestioneLotti: number = enum_Gestione_Lotti.Nessuna;

    private _GestioneGiacenze: number = enum_Gestione_Giacenze.SoloMovimentati;

    private _GestioneMagazzino_Abilitata: boolean = false;

    private _MultiColumnComboboxProdotti: GiasMultiColumnComboboxTemplateComponent = null;

    get MultiColumnComboboxProdotti(){
        return this._MultiColumnComboboxProdotti;
    }

    set MultiColumnComboboxProdotti(MultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent){
        this._MultiColumnComboboxProdotti = MultiColumnCombobox;
    }

    public controlli_Giacenze_Lotti = new Controlli_Giacenze_Lotti();

    public controlli_Giacenze_Lotti_Innesco = new Controlli_Giacenze_Lotti();

    //Servizi delle Grid
    GridDosiProdottiHttpService: any;
    GridDosiProdottiPublicService: any;

    constructor(
        private qdcservice: QdCService,
        private fabbricatiservice: FabbricatiService,
        private unitadimisuraservice: UnitaDiMisuraService,
        private translocoService: TranslocoService,
        private calcoloSuperfici: CalcoloSuperficiService,
        private gridimpiantiservice: GridImpiantiService) {


        this.Subs.add(this.ObsProdottiForm.subscribe(form => {
            this.ProdottiForm = form;
        }));

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Se è abilitata la gestione del magazzino e la gestione giacenze è 0/1 allora non
    //c'è il server filtering ma la multicolumn viene caricata all'evento di open
    Abilita3CaratteriServerFiltering() {

        let serverfiltering = true;

        if (this.GestioneMagazzino_Abilitata() &&
            (this.GestioneGiacenze() === enum_Gestione_Giacenze.SoloMovimentati ||
                this.GestioneGiacenze() === enum_Gestione_Giacenze.SoloPresenti)) {

            serverfiltering = false;
        }

        return serverfiltering;
    }

    public PlaceHolderRicercaProdotti(): string{

        /*Vecchia versione senza il bottone di Ricerca dei Prodotti
        let placeholder = "";

        if(this.Abilita3CaratteriServerFiltering())
            placeholder = "(" + this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")+")";

        return placeholder;*/

        let placeholder = "";

        if(this.Disabilita_Btn_Ricerca_Prodotti(""))
            placeholder = "(" + this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")+")";

        return placeholder;

    }

    GestioneMagazzino_Abilitata(): boolean {

        let lav_cod: number = +this.ProdottiForm.get("Operazione").value.primaryKey.codice;

        if (this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(lav_cod)) {
            this._GestioneMagazzino_Abilitata = this.qdcservice.GestioneMagazzino_Abilitata_Fertilizzanti;
        } else if (this.qdcservice.Elenco_Operazioni_Formulati.includes(lav_cod)) {
            this._GestioneMagazzino_Abilitata = this.qdcservice.GestioneMagazzino_Abilitata_Formulati;
        } else if (this.qdcservice.Elenco_Operazioni_Sementi.includes(lav_cod)) {
            this._GestioneMagazzino_Abilitata = this.qdcservice.GestioneMagazzino_Abilitata_Sementi;
        } else if (this.qdcservice.Elenco_Operazioni_Insetti.includes(lav_cod)){
            this._GestioneMagazzino_Abilitata = this.qdcservice.GestioneMagazzino_Abilitata_Insetti;
        }

        return this._GestioneMagazzino_Abilitata;
    }

    GestioneGiacenze(): number {

        let lav_cod: number = +this.ProdottiForm.get("Operazione").value.primaryKey.codice;

        if (this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(lav_cod)) {
            this._GestioneGiacenze = this.qdcservice.GestioneGiacenze_Fertilizzanti;
        } else if (this.qdcservice.Elenco_Operazioni_Formulati.includes(lav_cod)) {
            this._GestioneGiacenze = this.qdcservice.GestioneGiacenze_Formulati;
        } else if (this.qdcservice.Elenco_Operazioni_Sementi.includes(lav_cod)) {
            this._GestioneGiacenze = this.qdcservice.GestioneGiacenze_Sementi;
        } else if (this.qdcservice.Elenco_Operazioni_Insetti.includes(lav_cod)){
            this._GestioneGiacenze = this.qdcservice.GestioneGiacenze_Insetti;
        }

        return this._GestioneGiacenze;
    }

    GestioneLotti(): number {

        let lav_cod: number = +this.ProdottiForm.get("Operazione").value.primaryKey.codice;

        if (this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(lav_cod)) {
            this._GestioneLotti = this.qdcservice.GestioneLotti_Fertilizzanti;
        } else if (this.qdcservice.Elenco_Operazioni_Formulati.includes(lav_cod)) {
            this._GestioneLotti = this.qdcservice.GestioneLotti_Formulati;
        } else if (this.qdcservice.Elenco_Operazioni_Sementi.includes(lav_cod)) {
            this._GestioneLotti = this.qdcservice.GestioneLotti_Sementi;
        } else if (this.qdcservice.Elenco_Operazioni_Insetti.includes(lav_cod)){
            this._GestioneLotti = this.qdcservice.GestioneLotti_Insetti;
        }

        return this._GestioneLotti;
    }

    async getArray_Magazzini() {

        const LeggiMagazzini = <LeggiMagazzini_QdC>{
            lavorazione: this.ProdottiForm.get("Operazione").value,
            impresa: this.qdcservice.getImpresa_Model(),
            data: this.qdcservice.TestataForm.get("Data").value
        };

        let risp = await lastValueFrom(this.fabbricatiservice.Leggi_Magazzini(LeggiMagazzini)) as DropdownListMagazzino[];

        if(risp){
            let rispMapped = risp.map(m=>{
                m = this.qdcservice.setCodDescrDdlMagazzino(m);
            });
        }

        this.Array_Magazzini = risp;

        return this.Array_Magazzini;
    }

    async getArray_UdM(FormValue: any,impostaArrayallaDDL: boolean = true,flag_innesco: boolean = false) {

        let dettaglioTrattamento: DettaglioTrattamento = null;

        let doseEtichetta: DoseEtichetta = null;

        let dettaglioSemina: DettaglioSemina = null;

        let dettaglioFertilizzazione: DettaglioFertilizzazione = null;

        let avversita: AvversitaGruppo = null;

        switch(FormValue.Categoria_Magazzino){
            case FORMULATI:
                if(!flag_innesco){
                  dettaglioTrattamento = (FormValue.Prodotto as DettaglioTrattamento);
                  doseEtichetta = (FormValue?.Dose_Etichetta as DoseEtichetta);
                }else{
                  avversita = (FormValue?.Avversita as AvversitaGruppo);
                }
                break;
            case FERTILIZZANTI:
                dettaglioFertilizzazione = (FormValue.Prodotto as DettaglioFertilizzazione);
                break;
            case SEMENTI:
                dettaglioSemina = (FormValue.Prodotto as DettaglioSemina);
                break;
            case INSETTI:
                dettaglioTrattamento = (FormValue.Prodotto as DettaglioTrattamento);
                break;
        }

        const LeggiUnitaDiMisura: LeggiUnitaDiMisura = {
            lavorazione: FormValue.Operazione,
            elem_cod: flag_innesco ? INNESCHI : FormValue.Categoria_Magazzino,
            avversita: avversita,
            tipo_Attivita: this.qdcservice.TestataForm.get("Tipo").value,
            tipo_Ricetta: this.qdcservice.TestataForm.get("TipoRicetta").value,
            dettaglioTrattamento: dettaglioTrattamento,
            dettaglioFertilizzazione: dettaglioFertilizzazione,
            dettaglioSemina: dettaglioSemina,
            doseEtichetta: doseEtichetta,
            unitaDiMisura: flag_innesco ? FormValue.UdM_Innesco : FormValue.UdM
        };

        let risp: Array<UnitaDiMisura> = await this.unitadimisuraservice.Leggi_UnitaDiMisura_QdC(LeggiUnitaDiMisura);

        if (impostaArrayallaDDL)
            this.Array_UdM = risp;

        return risp;
    }

    async getUltimo_Magazzino_Prodotto_Movimentato() {

        //Prendo l'ultimo Magazzino in cui è stato movimentato quel prodotto dall'AGRODATAINIZIO alla data dell'operazione

        let magazzino:DropdownListMagazzino = null;

        const LeggiUltimo_Magazzino_Prodotto_Movimentato = <LeggiUltimo_Magazzino_Prodotto_Movimentato>{
            impresa: this.qdcservice.getImpresa_Model(),
            data: this.qdcservice.TestataForm.get("Data").value,
            dettaglioFertilizzazione: null,
            dettaglioTrattamento: null,
            dettaglioSemina: null
        };

        switch(this.ProdottiForm.get("Categoria_Magazzino").value){
            case INSETTI:
            case FORMULATI:
                LeggiUltimo_Magazzino_Prodotto_Movimentato.dettaglioTrattamento = this.ProdottiForm.get("Prodotto").value;
                break;
            case FERTILIZZANTI:
                LeggiUltimo_Magazzino_Prodotto_Movimentato.dettaglioFertilizzazione = this.ProdottiForm.get("Prodotto").value;
                break;
            case SEMENTI:
                LeggiUltimo_Magazzino_Prodotto_Movimentato.dettaglioSemina = this.ProdottiForm.get("Prodotto").value;
                break;
        }

        let risp = await this.fabbricatiservice.Leggi_Ultimo_Magazzino_Prodotto_Movimentato(LeggiUltimo_Magazzino_Prodotto_Movimentato)

        if(risp){
            magazzino = this.qdcservice.setCodDescrDdlMagazzino(risp as DropdownListMagazzino);
        }

        return magazzino;
    }

    Gestisci_Controlli_Lotti_Giacenze() {

        //Se la _GestioneMagazzino_Abilitata non è abilitata nascondo Lotto,Magazzino e flag visualizza giacenze 0
        //Il flag_visualizza_solo_prodotti_in_giacenza è visibile se la gestione giacenze è enum_Gestione_Giacenze.SoloMovimentati o enum_Gestione_Giacenze.TuttiProdotti
        if (this._GestioneMagazzino_Abilitata) {
            let Prodotto_Selezionato = this.ProdottiForm.get("Prodotto").value;

            if (Prodotto_Selezionato &&
                Prodotto_Selezionato.prodotto.codice > 0
            ) {

                //La DDL UdM è sempre abilitata obbligatoria e visibile indipendentemente dalle impostazioni
                this.controlli_Giacenze_Lotti.ddl_UdM = {
                    abilitato: true,
                    obbligatorio: true,
                    visibile: true
                };

                this.ProdottiForm.get("UdM").enable({
                    emitEvent: false,
                });

                this.controlli_Giacenze_Lotti.txt_Lotto.visibile = this.MostraLotto();

                let magazziniMovimentazioni: RilevamentoDiMagazzino[] = Prodotto_Selezionato.MagazziniMovimentazioni;

                // GestioneGIacenze è l'impostazione utente 180
                switch (this._GestioneGiacenze) {
                    case enum_Gestione_Giacenze.SoloMovimentati:
                    case enum_Gestione_Giacenze.SoloPresenti:

                        this.controlli_Giacenze_Lotti.ddl_Magazzino.obbligatorio = false;

                        this.controlli_Giacenze_Lotti.ddl_Magazzino.visibile = true;

                        this.controlli_Giacenze_Lotti.txt_Lotto.obbligatorio = false;

                        //Se ho selezionato un Magazzino Esterno o un Magazzino di un'Agenzia l'utente deve comunque selezionare
                        // un magazzino dell'azienda anche se ha l'impostazione delle Giacenze a SoloMovimentati o SoloPresenti
                        if(!this.qdcservice.IsMagazzinoAgenzia(magazziniMovimentazioni[0].Magazzino) &&
                            !this.qdcservice.IsMagazzinoEsterno(magazziniMovimentazioni[0].Magazzino)){

                          this.controlli_Giacenze_Lotti.ddl_Magazzino.abilitato = false;

                          this.controlli_Giacenze_Lotti.txt_Lotto.abilitato = false;

                          this.ProdottiForm.get(
                            "Magazzino_del_Prodotto_Selezionato"
                          ).disable({emitEvent: false});

                          this.ProdottiForm.get("Lotto").disable({
                            emitEvent: false,
                          });

                        }else{

                          this.controlli_Giacenze_Lotti.ddl_Magazzino.abilitato = true;

                          this.controlli_Giacenze_Lotti.txt_Lotto.abilitato = true;

                          this.ProdottiForm.get(
                            "Magazzino_del_Prodotto_Selezionato"
                          ).enable({emitEvent: false});

                          this.ProdottiForm.get("Lotto").enable({
                            emitEvent: false,
                          });

                        }

                        if(this._GestioneGiacenze === enum_Gestione_Giacenze.SoloMovimentati){
                            this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;
                        }else{
                            this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile = false;
                        }

                        break;

                    case enum_Gestione_Giacenze.TuttiProdotti:

                        this.controlli_Giacenze_Lotti.ddl_Magazzino.visibile = true;
                        this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;

                        //Prodotto in giacenza alla data
                        if (magazziniMovimentazioni &&
                            magazziniMovimentazioni.length === 1 &&
                            magazziniMovimentazioni[0].Qta > 0 &&
                            !this.qdcservice.IsMagazzinoAgenzia(magazziniMovimentazioni[0].Magazzino) &&
                            !this.qdcservice.IsMagazzinoEsterno(magazziniMovimentazioni[0].Magazzino)
                        ) {

                            this.controlli_Giacenze_Lotti.ddl_Magazzino.abilitato = false;
                            this.controlli_Giacenze_Lotti.txt_Lotto.abilitato = false;

                            this.ProdottiForm.get(
                                "Magazzino_del_Prodotto_Selezionato"
                            ).disable({emitEvent: false});

                            this.ProdottiForm.get("Lotto").disable({
                                emitEvent: false,
                            });
                        } else {

                            this.controlli_Giacenze_Lotti.ddl_Magazzino.obbligatorio = true;
                            this.controlli_Giacenze_Lotti.ddl_Magazzino.abilitato = true;
                            this.controlli_Giacenze_Lotti.txt_Lotto.abilitato = true;

                            this.ProdottiForm.get(
                                "Magazzino_del_Prodotto_Selezionato"
                            ).enable({emitEvent: false});

                            this.ProdottiForm.get("Lotto").enable({
                                emitEvent: false,
                            });

                            // questa è l'impostazione utente 181 (_GestioneLotti) e scatta solo se 180 è = 2
                            switch (this._GestioneLotti) {
                                case enum_Gestione_Lotti.Nessuna:
                                    this.controlli_Giacenze_Lotti.txt_Lotto.abilitato =
                                        false;
                                    this.controlli_Giacenze_Lotti.txt_Lotto.obbligatorio =
                                        false;
                                    this.ProdottiForm.get("Lotto").disable({
                                        emitEvent: false,
                                    });
                                    break;
                                case enum_Gestione_Lotti.Facoltativa:
                                case enum_Gestione_Lotti.Obbligatoria:
                                    this.controlli_Giacenze_Lotti.txt_Lotto.abilitato = true;

                                    if (this._GestioneLotti === enum_Gestione_Lotti.Obbligatoria) {
                                        this.controlli_Giacenze_Lotti.txt_Lotto.obbligatorio = true;
                                    } else {
                                        this.controlli_Giacenze_Lotti.txt_Lotto.obbligatorio = false;
                                    }
                                    break;
                            }
                        }

                        break;
                }
            } else {
                this.controlli_Giacenze_Lotti.ddl_Magazzino.visibile = false;
                this.controlli_Giacenze_Lotti.txt_Lotto.visibile = false;

                if (this._GestioneGiacenze === enum_Gestione_Giacenze.TuttiProdotti ||
                    this._GestioneGiacenze === enum_Gestione_Giacenze.SoloMovimentati) {
                    this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;
                } else {
                    this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile = false;
                }
            }
        } else {
            this.controlli_Giacenze_Lotti = {
                flag_visualizza_solo_prodotti_in_giacenza: {visibile: false},
                ddl_UdM: {abilitato: false, obbligatorio: false, visibile: true},
                ddl_Magazzino: {
                    abilitato: false,
                    obbligatorio: false,
                    visibile: false,
                },
                txt_Lotto: {abilitato: false, obbligatorio: false, visibile: false}
            };
        }

        //Se ho preso un prodotto da un Magazzino Esterno il Lotto non è editabile per evitare
        //di creare ulteriori scarichi non corretti editando il lotto
        if(this.ProdottiForm.get("Visualizza_Magazzini_Esterni").getRawValue()){
          this.controlli_Giacenze_Lotti.txt_Lotto = {abilitato: false, obbligatorio: false, visibile: true};

          this.ProdottiForm.get("Lotto").disable({
            emitEvent: false,
          });
        }

        if(this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA()){
          this.controlli_Giacenze_Lotti.ddl_UdM = {abilitato: false, obbligatorio: true, visibile: true};

          this.ProdottiForm.get("UdM").disable({
            emitEvent: false,
          });

          this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza = {visibile: false};
        }


        return this.controlli_Giacenze_Lotti;
    }

    MostraLotto(flag_innesco: boolean = false) {
        //Il Lotto lo mostro se Imp. SU 181 (Lotti) <> 0

        let gestioneLotti: number = 0;

        if(!flag_innesco){
          gestioneLotti = this.GestioneLotti();

          this._GestioneLotti = gestioneLotti;
        }else{
          gestioneLotti = this.qdcservice.GestioneLotti_Inneschi;
        }

        let mostra = false;

        if (gestioneLotti !== enum_Gestione_Lotti.Nessuna) {
            mostra = true;
        }

        return mostra;
    }


    //Oltre ai valori reimposto vuoti gli array dei controlli per la visibilità
    clearSezioneProdottiForm(clearFormArrayDosiProdotti: boolean) {

        this.Array_Magazzini = [];

        this.Array_UdM = [];

        this.ProdottiForm.patchValue({
            //Nessun_Magazzino:[false]
            //magazzino:[],
            flagDoseQuantitaTotale: 0,
            flagTipoDose: 0,
            Magazzino_del_Prodotto_Selezionato: null,
            Lotto: "",
            Dose_Ha: 0,
            Dose_Hl: 0,
            DoseTot_Ha: 0
        });

        this.Aggiorna_UdM(null);

        if (clearFormArrayDosiProdotti){

            if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                this.qdcservice.DosiProdottiFormArray(this.ProdottiForm,null).clear();
            }else{
                this.qdcservice.DosiProdottiFormArray(this.qdcservice.Sezione_ProdottoFormGroup(this.ProdottiForm.get("Operazione").value),null).clear();
            }

        }



    }


    /**
     * @description
     * Per fare patchValue dell'UdM utilizzare questa funzione
     * che triggera il subscribe nell'unita di misura component per avere memorizzato
     * il precedente valore della combo
     */
    Aggiorna_UdM(UdM: UnitaDiMisura) {
        //Per UdM ho bisogno di memorizzare il precedente elemento quindi ho bisogno dell'emitEvent
        this.ProdottiForm.patchValue({UdM: UdM}, {emitEvent: true, onlySelf: true});
    }

    getRilevamentoMagazzinoProdotto(): RilevamentoDiMagazzino {

        let rilevamentoDiMagazzino: RilevamentoDiMagazzino = null;

        let Prodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina = this.ProdottiForm.get("Prodotto").value;

        if (Prodotto && Prodotto.prodotto.codice > 0 &&
            Prodotto.MagazziniMovimentazioni && Prodotto.MagazziniMovimentazioni.length === 1)
            rilevamentoDiMagazzino = Prodotto.MagazziniMovimentazioni[0];

        return rilevamentoDiMagazzino;
    }

    async Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(Prodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina,
                                                           AvversitaInnesco: MultiColumnComboboxAvversitaInnesco = null) {

        let flag_innesco: boolean = false;
        let MagazziniMovimentazioni: RilevamentoDiMagazzino[] = [];

        if(AvversitaInnesco && !Prodotto){
          MagazziniMovimentazioni = AvversitaInnesco.MagazziniMovimentazioni;
          flag_innesco = true;
        }else if(!AvversitaInnesco && Prodotto){
          MagazziniMovimentazioni = Prodotto.MagazziniMovimentazioni;
        }

        if(flag_innesco){
          this.ProdottiForm.patchValue({
            Lotto_Innesco: ""
          });
        }else{
          this.ProdottiForm.patchValue({
            Lotto: ""
          });
        }


        if (!this.Array_Magazzini || this.Array_Magazzini.length === 0) {
            await this.getArray_Magazzini();
        }


        if (MagazziniMovimentazioni && MagazziniMovimentazioni.length === 1) {

            if(this.qdcservice.IsMagazzinoAgenzia(MagazziniMovimentazioni[0].Magazzino)){

                let ultimo_magazzino_in_cui_movimentato_prodotto: DropdownListMagazzino = await this.getUltimo_Magazzino_Prodotto_Movimentato();

                if(flag_innesco){
                  this.ProdottiForm.patchValue({
                    Magazzino_Innesco: ultimo_magazzino_in_cui_movimentato_prodotto,
                    Lotto_Innesco: MagazziniMovimentazioni[0].Lotto
                  });
                }else{
                  this.ProdottiForm.patchValue({
                    Magazzino_del_Prodotto_Selezionato: ultimo_magazzino_in_cui_movimentato_prodotto,
                    Lotto: MagazziniMovimentazioni[0].Lotto
                  });
                }


            }else{
                let magazzino_da_impostare = this.Array_Magazzini.find(magazzino => {
                    return magazzino.primaryKey.codice === MagazziniMovimentazioni[0].Magazzino.primaryKey.codice &&
                        magazzino.primaryKey.centroAziendalePK.codice === MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice &&
                        magazzino.primaryKey.centroAziendalePK.partitaIva === MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.partitaIva
                });

                if(flag_innesco){
                  this.ProdottiForm.patchValue({
                    Magazzino_Innesco: magazzino_da_impostare,
                    Lotto_Innesco: MagazziniMovimentazioni[0].Lotto
                  });
                }else{
                  this.ProdottiForm.patchValue({
                    Magazzino_del_Prodotto_Selezionato: magazzino_da_impostare,
                    Lotto: MagazziniMovimentazioni[0].Lotto
                  });
                }

            }

        }

        //Imposto il default al primo magazzino se non sono in ribaltamento brogliaccio agenda
        if(!this.ProdottiForm.get("Magazzino_del_Prodotto_Selezionato").value){
            if (this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Nessuno &&
                this.controlli_Giacenze_Lotti.ddl_Magazzino.visibile) {

              if(this.Array_Magazzini && this.Array_Magazzini.length > 0){
                if(flag_innesco){
                  if(!this.ProdottiForm.get("Magazzino_Innesco").value){
                    this.ProdottiForm.patchValue({
                      Magazzino_Innesco: this.Array_Magazzini[0]
                    });
                  }
                }else{
                  if(!this.ProdottiForm.get("Magazzino_del_Prodotto_Selezionato").value){
                    this.ProdottiForm.patchValue({
                      Magazzino_del_Prodotto_Selezionato: this.Array_Magazzini[0]
                    });
                  }
                }
              }
            }
        }

    }

    mostrabtnInserisciDoseProdotto(){
        return this.qdcservice.abilitaGrid;
    }


    Msg_Seleziona_Impianti(): ErroreGias[]{

        let listerroriGias = [];

        if(this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare && (!this.qdcservice.ProdottiDaTrattareSelezionatiFormArray || this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.getRawValue().length === 0)){
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.WarningBloccante,
                tipo:  enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareAlmenoUnProdottoDaTrattare")
            });

            if(this.ObsMsg_Seleziona_Impianti_Prodotti.getValue()){

                this.MultiColumnComboboxProdotti?.Multicolumncombobox?.toggle(false);

                this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false).then();

                this.ObsMsg_Seleziona_Impianti_Prodotti.next(false);
            }

        }

        if(this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare && (!this.qdcservice.ImpiantiSelezionatiFormArray || this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().length === 0)){

            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.WarningBloccante,
                tipo:  enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareAlmenoUnImpiantoColturale")
            });

            if(this.ObsMsg_Seleziona_Impianti_Prodotti.getValue()){

                this.MultiColumnComboboxProdotti?.Multicolumncombobox?.toggle(false);

                this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false).then();

                this.ObsMsg_Seleziona_Impianti_Prodotti.next(false);
            }

        }

        return listerroriGias;
    }

    public Disabilita_Btn_Ricerca_Prodotti(filtroProdotti: string){

        //Se FER non Bio e FIT senza disciplinare e etichetta(caso nessuno nessuno con codice -999): sempre obbligatori i 3 chr
        //Se il flag Visualizza_Solo_Prodotti_in_Giacenza è true  oppure ho l'impostazione 180 a SoloMovimentati o SoloPresenti con la 208 attiva
        // allora non sono obbligatori i 3 caratteri

        let disabilita = false;


        if(this.GestioneMagazzino_Abilitata() === true &&
            (this.GestioneGiacenze() === enum_Gestione_Giacenze.SoloMovimentati ||
                this.GestioneGiacenze() === enum_Gestione_Giacenze.SoloPresenti)){

            return disabilita;

        }

        let Operazione: Lavorazione = this.ProdottiForm.get("Operazione").value;

        let elem_cod: number = this.ProdottiForm.get("Categoria_Magazzino").value;

        let dpi = this.qdcservice.Operazioni_Con_Disciplinare(Operazione);

        let Lav_cod: number = + Operazione.primaryKey.codice;

        if(dpi) {
            let Disciplinare = this.qdcservice.getDisciplinareModelValue(Lav_cod);

            switch (elem_cod) {
                case FERTILIZZANTI:
                    if(Disciplinare?.codice !== DpiBio && Disciplinare?.regolamentoConcimazione?.tipo !== enum_PUARegolamenti_Tipo.PUA){
                        if(!filtroProdotti || filtroProdotti.length < 3){
                            //Controllo flag Prodotti In Giacenza
                            if(this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile){
                                if(this.ProdottiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value === false){
                                    disabilita = true;
                                }
                            }else{
                                disabilita = true;
                            }
                        }
                    }
                    break;
                case FORMULATI:
                    if(Disciplinare?.codice === NessunDpiNessunaEtichetta){
                        if(!filtroProdotti || filtroProdotti.length < 3){
                            //Controllo flag Prodotti In Giacenza
                            if(this.controlli_Giacenze_Lotti.flag_visualizza_solo_prodotti_in_giacenza.visibile){
                                if(this.ProdottiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value === false){
                                    disabilita = true;
                                }
                            }else{
                                disabilita = true;
                            }
                        }
                    }
                    break;
            }

        }

        return disabilita;
    }

    EventiPostRemoveGridDosiProdotti(){

        let Categoria_Magazzino = this.ProdottiForm.get('Categoria_Magazzino').value;
        let Operazione = this.ProdottiForm.get('Operazione').value;
        let DosiProdotti: Array<any> = [];

        if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
            DosiProdotti = (<FormArray> this.ProdottiForm.get('DosiProdotti')).getRawValue();
        }else{
            DosiProdotti = this.qdcservice.DosiProdotticonRigheSalvate(this.ProdottiForm,null);
        }

        if(Categoria_Magazzino === FORMULATI &&
            (+ Operazione.primaryKey.codice === enum_LAVCOD.DISERBO ||
                + Operazione.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO)){
            this.gridimpiantiservice.AggiornaGridImpiantiDopoCambioPercentualeAbbatimento();
        }

        this.calcoloSuperfici.ricalcoloSuperfici_daDosi();

    }

    setArrayDDLSezioneProdottiForm(dataItem: any){

        //Aggiorno anche gli Array che sono nel prodotti service così scattano gli ngIf che permettono
        //la visualizzazione di alcune ddl solo se hanno degli elemeneti

        this.Array_Magazzini = [];
        this.Array_UdM = [];

        if(dataItem.Magazzino_del_Prodotto_Selezionato && dataItem.Magazzino_del_Prodotto_Selezionato.primaryKey.codice > 0)
            this.Array_Magazzini = [dataItem.Magazzino_del_Prodotto_Selezionato];

        if(dataItem.UdM && dataItem.UdM.codice > 0)
            this.Array_UdM = [dataItem.UdM];


    }

    public setInfoLabelImpostazioni(){
        let descr: string = "";

        if(this.GestioneMagazzino_Abilitata()){
            if(this.qdcservice.Utente_Cod_Blocca_se_Supera_Giacenze){
                descr = this.translocoService.translate("InBaseAlleImpostazioniSoloProdottiaMagazzino");
            }else{
                switch(this.GestioneGiacenze()){
                    case enum_Gestione_Giacenze.SoloMovimentati:
                        descr = this.translocoService.translate("InBaseAlleImpostazioniSoloProdottiMovimentati");
                        break;
                    case enum_Gestione_Giacenze.SoloPresenti:
                        descr = this.translocoService.translate("InBaseAlleImpostazioniSoloProdottiaMagazzino");
                        break;
                    case enum_Gestione_Giacenze.TuttiProdotti:
                        descr = this.translocoService.translate("InBaseAlleImpostazioniImpiegareProdottiaMagazzieMaiMovimenati");
                        break;
                }
            }
        }else{
            descr = this.translocoService.translate("InBaseAlleImpostazioniNonScaricareProdotti");
        }

        return descr;
    }

    Is_Nuovo_Prodotto(): boolean{

        let nuovo_prodotto = false;

        if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
            if(this.ProdottiForm.get("Nuova_Riga").getRawValue()){
                nuovo_prodotto = true;
            }
        }

        return nuovo_prodotto;
    }

    Imposta_In_Automatico_Elemento_alla_DDL(): boolean{

        let imposta = false;

        if(this.qdcservice.Is_Ribaltamento_Ricetta_Da_Origine_Diversa() && this.Is_Nuovo_Prodotto()){
            imposta = true;
        }

        return imposta;
    }

    public RicercaProdotti(descrizione: string): ErroreGias[]{

        let elem_cod = this.ProdottiForm.get("Categoria_Magazzino").value;

        let listerroriGias = [];

        if(this.ProdottiForm.get("Visualizza_Magazzini_Esterni").getRawValue() === true){
          if(!this.GestioneMagazzino_Abilitata()){
            listerroriGias.push(<ErroreGias>{
              severity: ErroreGias_Severity.WarningBloccante,
              tipo:  enum_ErroreGias_Tipo.Generico,
              messaggio: this.translocoService.translate("AbilitareGestioneMagazzinoPerProdottiMagazzinoEsterno",{RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale})
            });
          }
        }

        if(listerroriGias.length === 0){
          switch(elem_cod){
            case FORMULATI:

              if(this.Disabilita_Btn_Ricerca_Prodotti(descrizione)){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.WarningBloccante,
                  tipo:  enum_ErroreGias_Tipo.Generico,
                  messaggio: this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")
                });
              }

              break;
            case FERTILIZZANTI:

              if(this.Disabilita_Btn_Ricerca_Prodotti(descrizione)){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.WarningBloccante,
                  tipo:  enum_ErroreGias_Tipo.Generico,
                  messaggio: this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")
                });
              }
              break;
            case SEMENTI:

              if(this.Disabilita_Btn_Ricerca_Prodotti(descrizione)){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.WarningBloccante,
                  tipo:  enum_ErroreGias_Tipo.Generico,
                  messaggio: this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")
                });
              }
              break;
            case INSETTI:

              if(this.Disabilita_Btn_Ricerca_Prodotti(descrizione)){
                listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.WarningBloccante,
                  tipo:  enum_ErroreGias_Tipo.Generico,
                  messaggio: this.translocoService.translate("ScriviAlmeno3CaratteriPerFiltrareProdotti")
                });
              }
              break;
          }
        }

        if(listerroriGias && listerroriGias.length > 0){
            this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false).then();
        }

        return listerroriGias;

    }

    async Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(Prodotto_Scelto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina): Promise<Obj_Errore_Gias_QdC>{

      let obj_errore_gias_qdc: Promise<Obj_Errore_Gias_QdC> = null;

      let listerroriGias: ErroreGias[] = [];

      if(Prodotto_Scelto){

        if(Prodotto_Scelto.MagazziniMovimentazioni && Prodotto_Scelto.MagazziniMovimentazioni.length > 0){

          let Magazzino_Esterno_con_Lotto = this.qdcservice.getMagazzino_Esterno_con_Lotto(Prodotto_Scelto.MagazziniMovimentazioni);

          if(Magazzino_Esterno_con_Lotto && Magazzino_Esterno_con_Lotto.Magazzino_Esterno){
            let Piva_Magazzino_Esterno = Magazzino_Esterno_con_Lotto.Magazzino_Esterno.primaryKey.centroAziendalePK.partitaIva;

            let Piva = this.qdcservice.getImpresa_Model().partitaIva;

            if(Piva_Magazzino_Esterno !== "" && Piva_Magazzino_Esterno !== Piva){

              await lastValueFrom(this.qdcservice.impostazioniaziendecentriService.getImprese_Impostazioni(Piva_Magazzino_Esterno,0));

              let Elem_Cod: number = +this.ProdottiForm.get("Categoria_Magazzino").getRawValue();

              let Operazioni: Lavorazione[] = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

              let obj_Impostazioni_Magazzino_Lotto_Giacenze = this.qdcservice.getImpostazioni_Magazzino_Lotto_Giacenze(Piva_Magazzino_Esterno,Operazioni);

              let gestioneMagazzino_Azienda_Esterna = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneMagazzino_Abilitata.find(x=>x.Elem_Cod === Elem_Cod).Valore;

              let gestioneGiacenze_Azienda_Esterna = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneGiacenze_Abilitata.find(x=>x.Elem_Cod === Elem_Cod).Valore;

              let gestioneLotto_Azienda_Esterna = obj_Impostazioni_Magazzino_Lotto_Giacenze.GestioneLotti.find(x=>x.Elem_Cod === Elem_Cod).Valore;

              let ImpostazioneLottiQdC_Descrizione = "";

              switch(this.GestioneLotti()){
                case enum_Gestione_Lotti.Nessuna:
                  ImpostazioneLottiQdC_Descrizione = this.translocoService.translate("Nessuna");
                  break;
                case enum_Gestione_Lotti.Obbligatoria:
                  ImpostazioneLottiQdC_Descrizione = this.translocoService.translate("Obbligatoria");
                  break;
                case enum_Gestione_Lotti.Facoltativa:
                  ImpostazioneLottiQdC_Descrizione = this.translocoService.translate("Facoltativa");
                  break;
              }

              let ImpostazioneLottiEsterna_Descrizione = "";

              switch(gestioneLotto_Azienda_Esterna){
                case enum_Gestione_Lotti.Nessuna:
                  ImpostazioneLottiEsterna_Descrizione = this.translocoService.translate("Nessuna");
                  break;
                case enum_Gestione_Lotti.Obbligatoria:
                  ImpostazioneLottiEsterna_Descrizione = this.translocoService.translate("Obbligatoria");
                  break;
                case enum_Gestione_Lotti.Facoltativa:
                  ImpostazioneLottiEsterna_Descrizione = this.translocoService.translate("Facoltativa");
                  break;
              }

              //Controllo Impostazioni in comune tra Azienda Terzista e Azienda QdC
              if(this.GestioneMagazzino_Abilitata() && gestioneMagazzino_Azienda_Esterna){

                if(this.GestioneLotti() === enum_Gestione_Lotti.Obbligatoria && gestioneLotto_Azienda_Esterna === enum_Gestione_Lotti.Obbligatoria && Magazzino_Esterno_con_Lotto.Lotto === ""){

                  listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.WarningBloccante,
                    tipo:  enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("ImpostazioneLottiObbligatoriaPerAziendaQdCEsterna",{RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, MagazzinoDes: Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione})
                  });

                }else{

                  if(gestioneLotto_Azienda_Esterna=== enum_Gestione_Lotti.Obbligatoria && Magazzino_Esterno_con_Lotto.Lotto === ""){
                    listerroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.WarningBloccante,
                      tipo:  enum_ErroreGias_Tipo.Generico,
                      messaggio: this.translocoService.translate("ImpostazioneLottiObbligatoriaPerAziendaMagazzinoEsterno",{MagazzinoDes: Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione, RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, ImpostazioneLottiQdC: ImpostazioneLottiQdC_Descrizione})
                    });
                  }

                  if(this.GestioneLotti()=== enum_Gestione_Lotti.Obbligatoria && Magazzino_Esterno_con_Lotto.Lotto === ""){
                    listerroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.WarningBloccante,
                      tipo:  enum_ErroreGias_Tipo.Generico,
                      messaggio: this.translocoService.translate("ImpostazioneLottiObbligatoriaPerAziendaQdC",{ RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, ImpostazioneLottiEsterna: ImpostazioneLottiEsterna_Descrizione})
                    });
                  }

                }

                if(this.GestioneLotti() === enum_Gestione_Lotti.Nessuna && gestioneLotto_Azienda_Esterna === enum_Gestione_Lotti.Nessuna && Magazzino_Esterno_con_Lotto.Lotto !== ""){
                  listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.WarningBloccante,
                    tipo:  enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("ImpostazioneLottiNessunaPerAziendaQdCEsterna",{ RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, MagazzinoDes: Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione})
                  });
                }else{

                  if(gestioneLotto_Azienda_Esterna=== enum_Gestione_Lotti.Nessuna && Magazzino_Esterno_con_Lotto.Lotto !== ""){
                    listerroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.WarningBloccante,
                      tipo:  enum_ErroreGias_Tipo.Generico,
                      messaggio: this.translocoService.translate("ImpostazioneLottiNessunaPerAziendaMagazzinoEsterno",{MagazzinoDes: Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione, RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, ImpostazioneLottiQdC: ImpostazioneLottiQdC_Descrizione})
                    });
                  }

                  if(this.GestioneLotti()=== enum_Gestione_Lotti.Nessuna && Magazzino_Esterno_con_Lotto.Lotto !== ""){
                    listerroriGias.push(<ErroreGias>{
                      severity: ErroreGias_Severity.WarningBloccante,
                      tipo:  enum_ErroreGias_Tipo.Generico,
                      messaggio: this.translocoService.translate("ImpostazioneLottiNessunaPerAziendaQdC",{ RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale, ImpostazioneLottiEsterna: ImpostazioneLottiEsterna_Descrizione})
                    });
                  }

                }


              }else{

                if(this.GestioneMagazzino_Abilitata() && !gestioneMagazzino_Azienda_Esterna){
                  listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.WarningBloccante,
                    tipo:  enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("AbilitareGestioneMagazzinoImpresaEsterna",{MagazzinoDes: Magazzino_Esterno_con_Lotto.Magazzino_Esterno.descrizione})
                  });
                }

                if(!this.GestioneMagazzino_Abilitata() && gestioneMagazzino_Azienda_Esterna){
                  listerroriGias.push(<ErroreGias>{
                    severity: ErroreGias_Severity.WarningBloccante,
                    tipo:  enum_ErroreGias_Tipo.Generico,
                    messaggio: this.translocoService.translate("AbilitareGestioneMagazzinoPerProdottiMagazzinoEsterno",{RagSoc: this.qdcservice.getImpresa_Model().ragioneSociale})
                  });
                }
              }
            }

          }
        }else if(this.ProdottiForm.get("Visualizza_Magazzini_Esterni").getRawValue() === true){

          listerroriGias.push(<ErroreGias>{
            severity: ErroreGias_Severity.WarningBloccante,
            tipo:  enum_ErroreGias_Tipo.Generico,
            messaggio: this.translocoService.translate("SelezionareProdottoDaMagazzinoEsterno")
          });

        }

      }

      if(listerroriGias && listerroriGias.length > 0){
        return this.qdcservice.gestisci_ErroriGias(listerroriGias, true,true);
      }else{
        return obj_errore_gias_qdc;
      }

    }

  getDescrizioneDose_HA(): string{
    let descrizione = "";

    let elem_cod: number = this.ProdottiForm.get("Categoria_Magazzino").value;

    if(elem_cod === SEMENTI) {
      descrizione = this.translocoService.translate("qdc.Densita");
    } else if(elem_cod === INSETTI) {
      return this.translocoService.translate('qdc.QuantitaEttaro');
    } else {
      descrizione = this.translocoService.translate("qdc.rbl_DoseHAResource1.Text");
    }

    if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
      descrizione += 'q';
    } else {
      descrizione += 'Ha';
    }

    return descrizione;
  }

  Gestisci_Controlli_Lotti_Giacenze_Innesco() {

    //Se la _GestioneMagazzino_Abilitata non è abilitata nascondo Lotto,Magazzino e flag visualizza giacenze 0
    //Il flag_visualizza_solo_prodotti_in_giacenza è visibile se la gestione giacenze è enum_Gestione_Giacenze.SoloMovimentati o enum_Gestione_Giacenze.TuttiProdotti
    if(this.qdcservice.TestataForm.get("Operazioni").getRawValue().findIndex((x: Lavorazione)=>this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ x.primaryKey.codice)) > -1){

      let Innesco_Incluso: boolean =  this.ProdottiForm.get("Innesco_Incluso").getRawValue();

      if (this.qdcservice.GestioneMagazzino_Abilitata_Inneschi && !Innesco_Incluso) {

        let Innesco: MultiColumnComboboxAvversitaInnesco = this.ProdottiForm.get("Avversita").value;

        if (Innesco &&
          Innesco.Codice_Concatenato !== ""
        ) {

          //La DDL UdM è sempre abilitata obbligatoria e visibile indipendentemente dalle impostazioni
          this.controlli_Giacenze_Lotti_Innesco.ddl_UdM = {
            abilitato: true,
            obbligatorio: true,
            visibile: true
          };

          this.ProdottiForm.get("UdM_Innesco").enable({
            emitEvent: false,
          });

          this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.visibile = this.MostraLotto(true);

          let magazziniMovimentazioni: RilevamentoDiMagazzino[] = Innesco.MagazziniMovimentazioni;

          // GestioneGIacenze è l'impostazione utente 180
          switch (this.qdcservice.GestioneGiacenze_Inneschi) {
            case enum_Gestione_Giacenze.SoloMovimentati:
            case enum_Gestione_Giacenze.SoloPresenti:

              this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.obbligatorio = false;

              this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.visibile = true;

              this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.obbligatorio = false;

              //Se ho selezionato un Magazzino Esterno o un Magazzino di un'Agenzia l'utente deve comunque selezionare
              // un magazzino dell'azienda anche se ha l'impostazione delle Giacenze a SoloMovimentati o SoloPresenti
              if(magazziniMovimentazioni &&
                magazziniMovimentazioni.length > 0 &&
                !this.qdcservice.IsMagazzinoAgenzia(magazziniMovimentazioni[0].Magazzino) &&
                !this.qdcservice.IsMagazzinoEsterno(magazziniMovimentazioni[0].Magazzino)){

                this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.abilitato = false;

                this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato = false;

                this.ProdottiForm.get(
                  "Magazzino_Innesco"
                ).disable({emitEvent: false});

                this.ProdottiForm.get("Lotto_Innesco").disable({
                  emitEvent: false,
                });

              }else{

                this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.abilitato = true;

                this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato = true;

                this.ProdottiForm.get(
                  "Magazzino_Innesco"
                ).enable({emitEvent: false});

                this.ProdottiForm.get("Lotto_Innesco").enable({
                  emitEvent: false,
                });

              }

              if(this.qdcservice.GestioneGiacenze_Inneschi === enum_Gestione_Giacenze.SoloMovimentati){
                this.controlli_Giacenze_Lotti_Innesco.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;
              }else{
                this.controlli_Giacenze_Lotti_Innesco.flag_visualizza_solo_prodotti_in_giacenza.visibile = false;
              }

              break;

            case enum_Gestione_Giacenze.TuttiProdotti:

              this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.visibile = true;
              this.controlli_Giacenze_Lotti_Innesco.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;

              //Prodotto in giacenza alla data
              if (magazziniMovimentazioni &&
                magazziniMovimentazioni.length === 1 &&
                magazziniMovimentazioni[0].Qta > 0 &&
                !this.qdcservice.IsMagazzinoAgenzia(magazziniMovimentazioni[0].Magazzino) &&
                !this.qdcservice.IsMagazzinoEsterno(magazziniMovimentazioni[0].Magazzino)
              ) {

                this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.abilitato = false;
                this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato = false;

                this.ProdottiForm.get(
                  "Magazzino_Innesco"
                ).disable({emitEvent: false});

                this.ProdottiForm.get("Lotto_Innesco").disable({
                  emitEvent: false,
                });
              } else {

                this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.obbligatorio = true;
                this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.abilitato = true;
                this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato = true;

                this.ProdottiForm.get(
                  "Magazzino_Innesco"
                ).enable({emitEvent: false});

                this.ProdottiForm.get("Lotto_Innesco").enable({
                  emitEvent: false,
                });

                // questa è l'impostazione utente 181 (_GestioneLotti) e scatta solo se 180 è = 2
                switch (this.qdcservice.GestioneLotti_Inneschi) {
                  case enum_Gestione_Lotti.Nessuna:
                    this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato =
                      false;
                    this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.obbligatorio =
                      false;
                    this.ProdottiForm.get("Lotto_Innesco").disable({
                      emitEvent: false,
                    });
                    break;
                  case enum_Gestione_Lotti.Facoltativa:
                  case enum_Gestione_Lotti.Obbligatoria:
                    this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.abilitato = true;

                    if (this.qdcservice.GestioneLotti_Inneschi === enum_Gestione_Lotti.Obbligatoria) {
                      this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.obbligatorio = true;
                    } else {
                      this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.obbligatorio = false;
                    }
                    break;
                }
              }

              break;
          }
        } else {
          this.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.visibile = false;
          this.controlli_Giacenze_Lotti_Innesco.txt_Lotto.visibile = false;

          if (this.qdcservice.GestioneGiacenze_Inneschi === enum_Gestione_Giacenze.TuttiProdotti ||
            this.qdcservice.GestioneGiacenze_Inneschi === enum_Gestione_Giacenze.SoloMovimentati) {
            this.controlli_Giacenze_Lotti_Innesco.flag_visualizza_solo_prodotti_in_giacenza.visibile = true;
          } else {
            this.controlli_Giacenze_Lotti_Innesco.flag_visualizza_solo_prodotti_in_giacenza.visibile = false;
          }
        }
      } else {

        this.controlli_Giacenze_Lotti_Innesco = {
          flag_visualizza_solo_prodotti_in_giacenza: {visibile: false},
          ddl_UdM: {abilitato: false, obbligatorio: false, visibile: false},
          ddl_Magazzino: {
            abilitato: false,
            obbligatorio: false,
            visibile: false,
          },
          txt_Lotto: {abilitato: false, obbligatorio: false, visibile: false}
        };
      }
    }



    return this.controlli_Giacenze_Lotti_Innesco;
  }

}
