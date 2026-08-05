import {Injectable, OnDestroy} from '@angular/core';
import {TipoAllevamento} from 'app/Model/metaschema/TipoAllevamento';
import {Epoca} from 'app/Model/metaschema/Epoca';
import { enum_LAVCOD, enum_PUARegolamenti_Tipo, enum_TipoOperazioneDB, enum_UnitaMisura} from 'app/Model/TipiEnumerativi';
import {FertilizzazioneService, LeggiEfficienza} from 'app/Service/Metaschema/Fertilizzazione.service';
import {QdCProdottiService} from '../prodotti.service';
import {QdCService} from '../qdc.service';
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {ClasseTessitura} from 'app/Model/anagrafiche/ClasseTessitura';
import { FormGroup} from '@angular/forms';
import {
  DropdownListDisciplinare, GridImpiantoSelezionatoModel, MultiColumnComboboxDose_Etichetta,
  MultiColumnComboboxFertilizzazione
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Subscription} from 'rxjs';
import {DpiService} from 'app/Service/DPI/dpi.service';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {LeggiProdotti, ProdottiService} from '../../../../Service/Anagrafica/prodotti.service';
import {  AgendaService,  ControllaMassimali_QdC,  LeggiDisponibilitaAttualeFertilizzante} from '../../../../Service/Agenda/Agenda.service';
import {GridDosiProdottiControlliService} from '../grid-dosi-prodotti/grid-dosi-prodotti-controlli.service';
import {GridDosiProdottiService} from '../grid-dosi-prodotti/grid-dosi-prodotti.service';
import {ObjParametriAgendaService} from '../../../../Service/obj-parametri-agenda.service';
import {QdCFertilizzantiService} from './fertilizzanti.service';
import {Disciplinare} from '../../../../Model/metaschema/Disciplinari';
import {FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI} from "../../../../Model/CostantiPersonalizzate";
import {QdCTestataService} from '../testata/testata.service';
import {UtilityFunctions} from '../../../../Utility/UtilityFunctions';
import {  enum_ErroreGias_Tipo,  ErroreGias,  ErroreGias_Severity, rispostaStandard} from '../../../../Service/master.service';
import {MisceleService} from '../miscele.service';
import {ColumnCombobox} from 'gias-ui-kit';
import { CELL_TYPES } from 'gias-ui-kit';
import {DettaglioFertilizzazione} from '../../../../Model/attivita/dettagli/DettaglioFertilizzazione';
import { Tipo_Attivita, Stati } from 'gias-ui-kit';
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";
import {QuantitaSuImpianto} from "../../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {EpocheService} from "../../../../Service/Metaschema/epoche.service";
import {FunzioniComuniService} from "../../../../Service/FunzioniComuni.service";

@Injectable()

export class QdCDettagliFertilizzantiService implements OnDestroy{

    Subs: Subscription = new Subscription();

    Array_Fertilizzanti: Array<MultiColumnComboboxFertilizzazione> = [];

    FertilizzantiForm: FormGroup;

    Operazione: Lavorazione;

    Qta_Ha_Distribuibile_Prodotto = 1000000;

    Qta_Ha_Distribuibile_Prodotto_Ricette = 1000000;

    Prima_Apertura_MultiColumn_Fertilizzante = false;

    public ColumnComboboxFertilizzanti: Array<ColumnCombobox> = [];

    Lav_Cod: number = 0;

    public Disponibilita_Attuale_Fertilizzante: number = 0;

    constructor(private qdcprodottiservice: QdCProdottiService,
                private qdcservice: QdCService,
                private fertilizzazioneservice: FertilizzazioneService,
                private epocheservice: EpocheService,
                private funzionicomuniservice: FunzioniComuniService,
                private dpiservice: DpiService,
                private prodottiservice: ProdottiService,
                private agendaservice: AgendaService,
                private gridcontrolliservice: GridDosiProdottiControlliService,
                private gridosiprodottiservice: GridDosiProdottiService,
                private objparametriAgendaService: ObjParametriAgendaService,
                private qdcfertilizzantiservice: QdCFertilizzantiService,
                private testataservice: QdCTestataService,
                private misceleservice: MisceleService){


        this.Subs.add(this.qdcprodottiservice.ObsProdottiForm.subscribe(form=>{
            this.FertilizzantiForm = form;

            this.Operazione = this.FertilizzantiForm?.get('Operazione')?.value;

            this.Lav_Cod = + this.Operazione?.primaryKey?.codice;
        }));

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Letture Lato Server

    async getArray_Fertilizzanti(filtrodescr: string,impostaInAutomaticoPrimoElementoAllaDDL:boolean = true) {

        if(this.qdcprodottiservice.Msg_Seleziona_Impianti().length === 0){

            let lav_cod: number = + this.Operazione.primaryKey.codice;

            let LeggiProdotti = <LeggiProdotti>{
                lavorazione: this.Operazione,
                disciplinare: this.qdcservice.getDisciplinareModelValue(lav_cod),
                filtroPerDescrizione: filtrodescr,
                data: this.qdcservice.TestataForm.get("Data").value,
                escludiGiacenzeZero: this.FertilizzantiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value,
                impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
                statoAttivita: this.qdcservice.TestataForm.get("Stato").value,
                magazziniAgenzie: this.FertilizzantiForm.get("Visualizza_Magazzini_Agenzie").value,
                magazziniEsterni: this.FertilizzantiForm.get("Visualizza_Magazzini_Esterni").value,
                pua: this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().find(c=>+c.Operazione.primaryKey.codice === lav_cod).pua
            };

            this.Array_Fertilizzanti = (await this.prodottiservice.Leggi_Fertilizzanti(LeggiProdotti)) as MultiColumnComboboxFertilizzazione[];

            //Descrizione Concatenata
            let ArrayFertilizzanti_Mapped = this.Array_Fertilizzanti.map(p=>{
                p = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(FERTILIZZANTI,p) as MultiColumnComboboxFertilizzazione;
            });

            // Se c'è solo un Prodotto (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
            // lo imposto come default
            if (impostaInAutomaticoPrimoElementoAllaDDL && this.Array_Fertilizzanti.length === 1) {

                let Fertilizzante_Selezionato: MultiColumnComboboxFertilizzazione = this.FertilizzantiForm.get("Prodotto").value;

                if(Fertilizzante_Selezionato?.Codice_Concatenato !== this.Array_Fertilizzanti[0].Codice_Concatenato){
                    this.FertilizzantiForm.patchValue({
                        Prodotto: this.Array_Fertilizzanti[0],
                    });

                    await this.changeFertilizzante(this.Array_Fertilizzanti[0]);
                }

            }
        }else{
            this.Array_Fertilizzanti = [];
        }



        return this.Array_Fertilizzanti;
    }

    //Fine Letture Lato Server

    Gestisci_Efficienza_N_Utile(Form: FormGroup, Epoca: Epoca): Promise<boolean>{

        return new Promise<boolean>(async (resolve, reject)=>{

            let unitaDiMisura: UnitaDiMisura = Form.get("UdM").value;

            let Fertilizzante: MultiColumnComboboxFertilizzazione = Form.get("Prodotto").value;

            let Specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();

            let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice);

            let ClassiTessitura: ClasseTessitura[] = this.qdcservice.GetClassiTessituraImpiantiSelezionati();

            let TipoFertilizzante = 0;
            let Efficienza = 1;
            let EpocaCod = 0;
            let Tipo_PuaRegolamento = 0;
            let LeggiEfficienza: LeggiEfficienza;

            if(Disciplinare && Disciplinare.regolamentoConcimazione && Disciplinare.regolamentoConcimazione.tipo !== 0)
                Tipo_PuaRegolamento = Disciplinare.regolamentoConcimazione.tipo;

            Form.patchValue({
                Efficienza: 1
            },{emitEvent: false});

            //-----------
            //EFFICIENZA
            //-----------

            if(Fertilizzante){

                let N = Form.get("N").value;

                if(Fertilizzante.tipoFertilizzante && Fertilizzante.tipoFertilizzante.codice !== 0){

                    TipoFertilizzante = Fertilizzante.tipoFertilizzante.codice;

                    if(Epoca && Epoca.codice)
                        EpocaCod = Epoca.codice;

                    if(TipoFertilizzante !== 0 && EpocaCod !== 0){
                        switch(Tipo_PuaRegolamento){
                            case 0:
                                Form.patchValue({
                                    Efficienza: Efficienza,
                                    N_Utile: N * Efficienza
                                },{emitEvent: false});
                                break;
                            case 1:
                                //pua 2007
                                LeggiEfficienza = <LeggiEfficienza>{
                                    tipoFertilizzante:  Fertilizzante.tipoFertilizzante,
                                    epoca: Epoca
                                };

                                Efficienza = await this.fertilizzazioneservice.LeggiEfficienza_PUA_2007(LeggiEfficienza);

                                Form.patchValue({
                                    Efficienza: Efficienza,
                                    N_Utile: N * Efficienza
                                },{emitEvent: false});

                                break;
                            default:
                                //pan successivi

                                //TODO sviluppare tutta la parte degli animali
                                let TipoAllevamentoCod = 0;

                                let Dose = 1;
                                let qta =  Form.get("Dose_Ha").value;
                                let qta_trasformata = 0;
                                let qta_N = 0;

                                switch(unitaDiMisura?.codice){

                                    case enum_UnitaMisura.KG:
                                    case enum_UnitaMisura.Litri:
                                        qta_trasformata = qta;
                                        break;
                                    case enum_UnitaMisura.Quintali:
                                        qta_trasformata = qta * 100;
                                        break;
                                    case enum_UnitaMisura.Tonnellate:
                                        qta_trasformata = qta * 1000;
                                        break;
                                    case enum_UnitaMisura.Metri_Cubi:
                                        qta_trasformata = qta * 1000
                                        break;
                                }

                                qta_N = qta * N / 100;

                                if(qta_N > 125)
                                    Dose = 2;

                                LeggiEfficienza = <LeggiEfficienza>{
                                    effluente: Fertilizzante.effluente,
                                    epoca: Epoca,
                                    tipoAllevamento: new TipoAllevamento(TipoAllevamentoCod),
                                    valoreDose: Dose,
                                    disciplinare: Disciplinare,
                                    classiTessitura: ClassiTessitura,
                                    specie: Specie
                                };

                                Efficienza = await this.fertilizzazioneservice.LeggiEfficienza(LeggiEfficienza);

                                Form.patchValue({
                                    Efficienza: Efficienza,
                                    N_Utile: N * Efficienza
                                },{emitEvent: false});

                                break;
                        }
                    }else{
                        Form.patchValue({
                            N_Utile: N * Efficienza
                        },{emitEvent: false});
                    }
                }else{
                    Form.patchValue({
                        N_Utile: N * Efficienza
                    },{emitEvent: false});
                }
            }

            this.Imposta_Dose_Ha_In_Base_Alla_Percentuale_N();

            resolve(true);
        });
    }

    //Oltre ai valori reimposto vuoti gli array dei controlli per la visibilità
    clearSezioneProdottiFertilizzantiForm(clearFormArrayDosiProdotti: boolean, clearProdotto: boolean, changeDPIDirettiva_Nitrati: boolean){

        //Non resetto il form se sono in inserimento prodotti e ho già salvato la riga di prodotto
        let clear = true;

        if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FertilizzantiForm.get("Riga_Salvata").value)
            clear = false;

        if(clear){
            this.FertilizzantiForm.patchValue({
                Efficienza:0,
                N_Utile:0,
                N:0,
                P:0,
                K:0,
                Cu:0
            });

            if(clearProdotto){
                this.Array_Fertilizzanti = [];

                this.FertilizzantiForm.patchValue({
                    Prodotto:null
                });

                this.Prima_Apertura_MultiColumn_Fertilizzante = false;
            }

            if(changeDPIDirettiva_Nitrati){
                this.qdcfertilizzantiservice.Array_EpocheFertilizzazione = [];

                this.FertilizzantiForm.patchValue({
                    EpocaFertilizzazione:null
                },{emitEvent: false});

                if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){

                    let DosiProdotti = this.qdcservice.DosiProdottiFormArray(this.FertilizzantiForm,null);

                    if(DosiProdotti && DosiProdotti.controls.length > 0){
                        for(let i = 0; i< DosiProdotti.controls.length;i++){
                            DosiProdotti.controls[i].patchValue({
                                EpocaFertilizzazione: null
                            },{emitEvent: false});
                        }
                    }

                }else{

                    this.qdcservice.Sezione_ProdottoFormGroup(this.FertilizzantiForm.get("Operazione").value).patchValue({
                        EpocaFertilizzazione: null
                    },{emitEvent: false});

                }

            }


            this.qdcprodottiservice.clearSezioneProdottiForm(clearFormArrayDosiProdotti);
        }


    }

    //Aggiorno N Utile al cambio di N
    changeN(Form: FormGroup){
      this.calcolaN_Utile(Form);
    }

    //Aggiorno N Utile al cambio dell Efficienza
    changeEfficienza(Form: FormGroup){
      this.calcolaN_Utile(Form);
    }

    calcolaN_Utile(Form: FormGroup){
      let _N = Form.get("N").value;
      let _Efficienza = Form.get("Efficienza").value;
      let valore = 0;

      if (_N && _N !== 0) {
        if (_Efficienza && _Efficienza != 0) {
          valore = _N * _Efficienza;
        }
      }

      Form.patchValue({
            N_Utile: this.funzionicomuniservice.roundNumber(valore,this.qdcservice.get4DecimalNumericSettings().decimals)
      },{emitEvent: false});


      //Ricalcolo la Dose Ha dato che è cambiata la N_Utile
      this.Imposta_Dose_Ha_In_Base_Alla_Percentuale_N();
    }

    async changeFertilizzante(Prodotto_Selezionato: MultiColumnComboboxFertilizzazione){

        let obj_errore_gias_qdc = await this.qdcprodottiservice.Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(Prodotto_Selezionato);

        if(obj_errore_gias_qdc && obj_errore_gias_qdc.list_errori_filtrati && obj_errore_gias_qdc.list_errori_filtrati.length > 0){
          this.FertilizzantiForm.patchValue({
            Prodotto: null,
          },{emitEvent: false});
          return;
        }

        this.FertilizzantiForm.patchValue({
            Efficienza: 1
        });

        this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        this.clearSezioneProdottiFertilizzantiForm(false,false,false);

        if(Prodotto_Selezionato){

            if(Prodotto_Selezionato.prodotto.codice !== 0){

                this.Imposta_N_P_K_Cu(Prodotto_Selezionato);



                this.Gestisci_Efficienza_N_Utile(this.FertilizzantiForm,this.FertilizzantiForm.get("EpocaFertilizzazione").getRawValue()).then(async risp=>{

                    if(risp){

                        await this.qdcprodottiservice.Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(Prodotto_Selezionato);

                        if(Prodotto_Selezionato.MagazziniMovimentazioni &&
                            Prodotto_Selezionato.MagazziniMovimentazioni.length >0 &&
                            Prodotto_Selezionato.MagazziniMovimentazioni[0].udm &&
                            Prodotto_Selezionato.MagazziniMovimentazioni[0].udm.codice > 0){
                            this.qdcprodottiservice.Aggiorna_UdM( Prodotto_Selezionato.MagazziniMovimentazioni[0].udm);
                        }else{
                            //Se il Fertilizzante non è stato caricaro a magazzino ma è un Effluente (esempio Letame Bovino),
                            //allora prendo l'udm dall'effluente

                            if(Prodotto_Selezionato.effluente &&
                                Prodotto_Selezionato.effluente.udm &&
                                Prodotto_Selezionato.effluente.udm.codice > 0){

                                this.qdcprodottiservice.Aggiorna_UdM( Prodotto_Selezionato.effluente.udm);
                            }

                        }

                        await this.Componi_Qta_Ha_Distribuibile_Prodotto(-1);

                        this.Imposta_Dose_Ha_In_Base_Alla_Percentuale_N();

                        this.CaricaDisponibilitaAttualeFertilizzante();
                    }


                });


            }

        }
    }

    async change_Direttiva_Nitrati(disciplinare: Disciplinare, ricarica_impianti: boolean = true){

        //Per ora faccio come il cambio di Disciplinare che non svuoto la maschera perchè se ne occupano i
        //controlli lato server (richiesta CAI)
        //this.clearSezioneProdottiFertilizzantiForm(false,true,true);

        await this.qdcfertilizzantiservice.getArray_EpocheFertilizzazione(this.FertilizzantiForm);

        if(ricarica_impianti)
            this.qdcservice.RicaricaGridImpianti();

        this.qdcservice.Imposta_PUA(null,true).then(value=>{
            this.setColumnComboboxFertilizzanti();
        });
    }

    async change_Utilizza_Direttiva_Nitrati(Utilizza_Direttiva_Nitrati: { [key: string]: boolean }){


        if(Utilizza_Direttiva_Nitrati){

            //Se è stata selezionata anche un'altra Operazione che utilizza il Disciplinare non lo imposto a null

            let operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").value;

            if(operazioni.findIndex(o=> + o.primaryKey.codice !== enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI && this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray)) === -1){

                this.qdcservice.TestataForm.patchValue({
                    Disciplinare: null
                },{emitEvent: false});

                this.testataservice.CambioDisciplinare(null);
            }
        }else{

            this.qdcservice.Sezione_ProdottoFormGroup(this.Operazione).patchValue({
                Direttiva_Nitrati: null
            },{emitEvent: false});

            this.FertilizzantiForm.patchValue({
                Direttiva_Nitrati: null
            },{emitEvent: false});

            //Se la Direttiva Nitrati la imposto a null non ricarico la grid degli impianti per la classe tessitura
            await this.change_Direttiva_Nitrati(null,false);

            //Reimposto il disciplinare dagli impianti selezionati
            if(!this.testataservice.Array_Disciplinari || this.testataservice.Array_Disciplinari.length === 0)
                await this.testataservice.getArray_Disciplinari(true);

            await this.qdcservice.ImpostaDisciplinare_Dagli_ImpiantiSelezionati(this.testataservice.Array_Disciplinari);

            this.testataservice.CambioDisciplinare(this.qdcservice.getDisciplinareModelValue(+ this.Operazione.primaryKey.codice) as DropdownListDisciplinare);

            this.qdcservice.Imposta_PUA(null,false).then(value=>{
                this.setColumnComboboxFertilizzanti();
            });
        }

        this.qdcservice.Gestisci_Controlli_Fertilizzanti(this.FertilizzantiForm,+ this.Operazione.primaryKey.codice);

        await this.Gestisci_Efficienza_N_Utile(this.FertilizzantiForm,this.FertilizzantiForm.get("EpocaFertilizzazione").getRawValue());

        //Per ora faccio come il cambio di Disciplinare che non svuoto la maschera perchè se ne occupano i
        //controlli lato server (richiesta CAI)
        //this.clearSezioneProdottiFertilizzantiForm(false,true,true);
    }

    setArrayDDLSezioneProdottiFertilizzantiForm(dataItem: any){

        //Aggiorno anche gli Array che sono nel prodotti service così scattano gli ngIf che permettono
        //la visualizzazione di alcune ddl solo se hanno degli elemeneti

        this.qdcprodottiservice.setArrayDDLSezioneProdottiForm(dataItem);

        this.Array_Fertilizzanti = [];

        if(dataItem.Prodotto && dataItem.Prodotto.prodotto.codice > 0)
            this.Array_Fertilizzanti = [dataItem.Prodotto];

    }

    async MostraMessaggiBottoniEditProttoFertilizzantiForm(){

        let FertilizzantiFormValue = this.FertilizzantiForm.getRawValue();

        if(FertilizzantiFormValue){

            await this.Componi_Qta_Ha_Distribuibile_Prodotto(FertilizzantiFormValue.DosiProdottiGridrowId);

            this.CaricaDisponibilitaAttualeFertilizzante();

        }

    }

    //Mostro l'indicazione della Dose consentita anche quando vado in modifica del prodotto scelto non solo quando lo seleziono
    //dalla ddl. In questo caso quando lo devo mostrare in modifica passo nella grid dosi tutte le righe (anche quella che sto modificando) poi viene esclusa
    //lato server mentre invece la Trattamenti_2 sbaglia e conteggia anche la riga di prodotto che sto modificando
    async Componi_Qta_Ha_Distribuibile_Prodotto(DosiProdottiGridrowId_Modificata: number){

        let Fertilizzante_Selezionato: MultiColumnComboboxFertilizzazione = this.FertilizzantiForm.get("Prodotto").value;

        if(Fertilizzante_Selezionato && Fertilizzante_Selezionato.prodotto.codice !== 0){

            let tipo_operazione_db : enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura;

            let row: FormGroup = null;

            let DosiProdotti: Array<any> = [];

            if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                DosiProdotti = this.FertilizzantiForm.get("DosiProdotti").getRawValue();
            }else{
                if(DosiProdottiGridrowId_Modificata > -1){
                    DosiProdotti = this.qdcservice.DosiProdottiFormArray(null,this.FertilizzantiForm.get("Operazione").getRawValue()).getRawValue();

                    row = this.FertilizzantiForm;

                    tipo_operazione_db = enum_TipoOperazioneDB.Modifica;
                }else{

                    DosiProdotti = this.qdcservice.DosiProdotticonRigheSalvate(null,this.FertilizzantiForm.get("Operazione").value);

                    row = this.gridosiprodottiservice.getRigaGridDosiProdotti(DosiProdotti,
                        DosiProdottiGridrowId_Modificata,
                        this.FertilizzantiForm,
                        null,
                        null,
                        this.Operazione,
                        this.qdcservice.TestataForm.get("Data").value,
                        this.FertilizzantiForm.get('flagTipoDose').value,
                        this.FertilizzantiForm.get('flagDoseQuantitaTotale').value,
                        this.qdcservice.AcquaForm.get('Dose_Acqua').value,
                        null,
                        null,
                        this.FertilizzantiForm.get("EpocaFertilizzazione").value,
                        this.FertilizzantiForm.get('Utilizza_Direttiva_Nitrati').value,
                        this.FertilizzantiForm.get('Direttiva_Nitrati').value,
                        null,
                        this.FertilizzantiForm.get('Modalita_Applicazione').value,
                        null,
                        null,
                        false,false,false);
                }
            }

            const Leggi = this.gridcontrolliservice.Componi_Obj_Controllo_Inserimento_Dose_Prodotto(row.getRawValue(),DosiProdotti,this.FertilizzantiForm,tipo_operazione_db,DosiProdottiGridrowId_Modificata,[],null);

            let massimali: ControllaMassimali_QdC = await this.agendaservice.ControllaMassimali(Leggi);

            if(massimali){
                this.Qta_Ha_Distribuibile_Prodotto = massimali.Qta_Ha_Distribuibile_Prodotto;

                //Per tutte le Ricette mostro anche la Dose/ha Preventivata
                if(this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta &&
                    this.qdcservice.TestataForm.get("Stato").value === Stati.Da_Eseguire ){
                    this.Qta_Ha_Distribuibile_Prodotto_Ricette = massimali.Qta_Ha_Distribuibile_Prodotto_Ricette;
                }
            }
        }


    }

    RicercaFertilizzanti(impostaInAutomaticoPrimoElementoAllaDDL: boolean, apriInAutomaticolaDDL: boolean){
        let Prodotto = this.FertilizzantiForm.get("Prodotto").value;

        let descrizione = "";

        if(Prodotto){

            if(Prodotto.prodotto.codice === this.qdcservice.Obj_Empty_MultiColumnComboboxFertilizzazione.prodotto.codice){
                descrizione = Prodotto.Descrizione_Concatenata;
            }else{
                descrizione = Prodotto.prodotto.descrizione;
            }

        }

        let listerroriGias = [];

        //Controllo che sia stata scelta prima una Direttiva Nitrati se il flag Utilizza Direttiva Nitrati è true

        let lav_cod: number = + this.Operazione.primaryKey.codice;

        if(lav_cod === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
            this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").getRawValue() &&
            ! this.qdcservice.getDisciplinareModelValue(lav_cod)){

            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.WarningBloccante,
                tipo:  enum_ErroreGias_Tipo.Generico,
                messaggio: this.qdcservice.translocoService.translate("qdc.SelezionaUnaDirettivaNitrati")
            });

            this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false).then();
        }

        if(listerroriGias.length === 0)
            listerroriGias = this.qdcprodottiservice.RicercaProdotti(descrizione);

        if(listerroriGias && listerroriGias.length > 0){
            this.Array_Fertilizzanti = [];
        }else{

            UtilityFunctions.loadDropDownItems(this.qdcprodottiservice.MultiColumnComboboxProdotti,this.getArray_Fertilizzanti(descrizione,impostaInAutomaticoPrimoElementoAllaDDL)).then(r=>{
              if(apriInAutomaticolaDDL){
                if(!this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.isOpen){
                  this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.focus();
                  this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.toggle(true);
                }
              }
            });
        }
    }


    /*+
    * @description:
    * Calcolo la Dose Ha in base alla percentuale di N Utilizzabile indicata nel prodotto
    * */
    Imposta_Dose_Ha_In_Base_Alla_Percentuale_N(){

        let riga: GridImpiantoSelezionatoModel = this.qdcservice.get_Riga_Impianti_Con_N_Residuo_Minore();

        let Prodotto: MultiColumnComboboxFertilizzazione = this.FertilizzantiForm.get("Prodotto").getRawValue();

        if(this.qdcservice.mostraPercentualeN(this.Operazione) && riga && Prodotto && Prodotto.Codice_Concatenato !== ""){

            let n_utile: number = this.FertilizzantiForm.get("N_Utile").getRawValue();

            if(n_utile > 0){
                let nuova_percentuale_n: number = this.FertilizzantiForm.get("N_Percentuale_X_Prodotto").getRawValue();

                let UdM: UnitaDiMisura = this.FertilizzantiForm.get("UdM").getRawValue();

                let n_residuo: number = this.funzionicomuniservice.roundNumber((nuova_percentuale_n / 100) * riga.N_Max,3);

                let dose_Ha: number = this.funzionicomuniservice.roundNumber(n_residuo / ( n_utile / 100),this.qdcservice.getProductNumericSettings(UdM,true).decimals);

                this.FertilizzantiForm.patchValue({
                    Dose_Ha: dose_Ha
                });

                this.misceleservice.calcoloMiscele('changeDose_Ha',this.FertilizzantiForm,0);
            }

        }

    }

    setColumnComboboxFertilizzanti() {
        //Configura le colonne della combobox
        //in base alla categoria di magazzino (elem_cod)
        //N.B. Tenere allineato con Obj_Empty_MultiColumnComboboxFertilizzazione

        this.ColumnComboboxFertilizzanti = [];

        this.ColumnComboboxFertilizzanti.push(
            new ColumnCombobox({
                    field: "prodotto.descrizione",
                    title: this.qdcservice.translocoService.translate('Prodotto')
                }
                /*,
                {
                    width: 200
                }*/
            ),
            new ColumnCombobox(
                {
                    field: "tipologieFertilizzante",
                    title: this.qdcservice.translocoService.translate('Tipologia')
                },
                {
                    isArray: true,
                    Arrayfield: "tipologieFertilizzante.descrizione",
                }
            ),
            new ColumnCombobox(
                {
                    field: "N_Str",
                    title: this.qdcservice.translocoService.translate('n')
                },{
                  width: 55
                }),
            new ColumnCombobox(
                {
                    field: "P_Str",
                    title: this.qdcservice.translocoService.translate('p2o5')
                },{
                    width: 55
                }),
            new ColumnCombobox(
                {
                    field: "K_Str",
                    title: this.qdcservice.translocoService.translate('qdc.lblK2OResource1.Text')
                },{
                    width: 55
                }),
            new ColumnCombobox(
                {
                    field: "Cu_Str",
                    title: this.qdcservice.translocoService.translate('Cu')
                },{
                    width: 55
                }));

        //Mostro le colonne del Magazzino solo se la GestioneMagazzino è Abilitata
        if (this.qdcprodottiservice.GestioneMagazzino_Abilitata()) {
            this.ColumnComboboxFertilizzanti.push(
                new ColumnCombobox(
                    {
                        field: "Qta",
                        title: this.qdcservice.translocoService.translate('GiacenzaAllaData')
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
                        title: this.qdcservice.translocoService.translate('GiacenzaTotale')
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
                        title: this.qdcservice.translocoService.translate('um'),
                    },
                    {
                        isArray: true,
                        Arrayfield: "MagazziniMovimentazioni.udm.simbolo",
                        width: 60
                    }
                ),
                new ColumnCombobox(
                    {
                        field: "Magazzino_descrizione",
                        title: this.qdcservice.translocoService.translate('Magazzino')
                    },
                    {
                        isArray: true,
                        Arrayfield: "MagazziniMovimentazioni.Magazzino.descrizione",
                    }
                )
            );

            //Colonna Lotto da Mostrare solo se la gestione lotti è abilitata
            if (this.qdcprodottiservice.MostraLotto() || this.FertilizzantiForm.get("Visualizza_Magazzini_Esterni").getRawValue()) {
                this.ColumnComboboxFertilizzanti.push(
                    new ColumnCombobox(
                        {
                            field: "Lotto",
                            title: this.qdcservice.translocoService.translate('Lotto2')
                        },
                        {
                            isArray: true,
                            Arrayfield: "MagazziniMovimentazioni.Lotto",
                            width: 200
                        }
                    )
                );
            }

          if(this.Mostra_Dichiarato_nel_PUA()){

            this.ColumnComboboxFertilizzanti.push(
              new ColumnCombobox(
                {
                  field: "Dichiarato_nel_PUA_Str",
                  title: this.qdcservice.translocoService.translate('qdc.Dichiarato_nel_PUA')
                })
            );

          }
        }
    }

    CaricaDisponibilitaAttualeFertilizzante(){

        if(this.mostraLbl_Disponibilita_Attuale()){
            const LeggiDisponibilitaAttualeFertilizzante = <LeggiDisponibilitaAttualeFertilizzante>{
                dettaglioFertilizzazione: this.FertilizzantiForm.get("Prodotto").getRawValue() as DettaglioFertilizzazione,
                pua: this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().find(c=>+c.Operazione.primaryKey.codice === this.Lav_Cod).pua,
                data: this.qdcservice.TestataForm.get("Data").getRawValue()
            };

            this.agendaservice.CaricaDisponibilitaAttualeFertilizzante(LeggiDisponibilitaAttualeFertilizzante).subscribe((risp: rispostaStandard<number>)=>{
                this.Disponibilita_Attuale_Fertilizzante = risp.RispostaStringa;
            });
        }else{
          this.Disponibilita_Attuale_Fertilizzante = 0;
        }

    }

    mostraLbl_Disponibilita_Attuale(){
        let mostra = false;

        //Da mostrare solamente se la riga non è salvata
        if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FertilizzantiForm.get("Riga_Salvata").value)
            return mostra;

        if(this.FertilizzantiForm.get("Prodotto").getRawValue() &&
            this.FertilizzantiForm.get("Prodotto").getRawValue().Codice_Concatenato !== "" &&
            this.qdcservice.get_Tipo_Ricetta() > 0 &&
            this.Lav_Cod === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
            this.qdcservice.TestataForm &&
            this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().find(c=>+c.Operazione.primaryKey.codice === this.Lav_Cod)?.pua?.codice > 0 &&
            this.qdcservice.getDisciplinareModelValue(this.Lav_Cod)?.regolamentoConcimazione?.tipo === enum_PUARegolamenti_Tipo.PUA){
            mostra = true;
        }


        return mostra;
    }

    private Imposta_N_P_K_Cu(Prodotto_Selezionato: MultiColumnComboboxFertilizzazione){

      //Se c'è l'effluente con un N dichiarato nel PUA prendo quell'N se invece quell'effluente
      //dichiarato nel PUA è stato anche caricato a magazzino con un N differente allora prendo l'N del carico.
      //Il P,K Cu non vengono dichiarati nel PUA e quindi prendo sempre quelli del fertilizzante

      let N: number = 0;

      if(Prodotto_Selezionato.effluente && Prodotto_Selezionato.effluente.N > 0){
        N = Prodotto_Selezionato.effluente.N;
      }

      if(Prodotto_Selezionato.N_Ponderato && Prodotto_Selezionato.N > 0){
        N = Prodotto_Selezionato.N;
      }

      if(N === 0){
        N = Prodotto_Selezionato.N;
      }

      this.FertilizzantiForm.patchValue({
        N: N,
        P: Prodotto_Selezionato.P,
        K: Prodotto_Selezionato.K,
        Cu:  Prodotto_Selezionato.Cu
      });
    }

    Mostra_Dichiarato_nel_PUA(): boolean{

      let mostra: boolean = false;

      let disciplinare = this.qdcservice.getDisciplinareModelValue(+ this.Operazione?.primaryKey?.codice);

      if(+ this.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
        disciplinare?.regolamentoConcimazione?.tipo === enum_PUARegolamenti_Tipo.PUA &&
        this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().find(c=>+ c.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)?.pua?.codice > 0){

        mostra = true;

      }

      return mostra;
    }


    SalvaDoseFertilizzante(Nuova_Riga: boolean){

      let EpocaDPI: Epoca = null;

      let Array_Dosi: Array<MultiColumnComboboxDose_Etichetta> = null;

      let EpocaFertilizzazione: Epoca = this.FertilizzantiForm.get("EpocaFertilizzazione").value;

      let Utilizza_Direttiva_Nitrati: boolean = this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").value;

      let Direttiva_Nitrati: Disciplinare = this.FertilizzantiForm.get("Direttiva_Nitrati").value;

      let Opzioni_Semina: BaseCodeDescr = null;

      let tipo_operazione_db = enum_TipoOperazioneDB.Modifica;

      let DosiProdottiGridrowId =  this.FertilizzantiForm.getRawValue().DosiProdottiGridrowId;

      let DosiProdottiFormArray = this.qdcservice.DosiProdottiFormArray(null,this.Operazione);

      let DosiProdotti:Array<any> = DosiProdottiFormArray.getRawValue();

      let Modalita_Applicazione: BaseCodeDescr =  this.FertilizzantiForm.get("Modalita_Applicazione").value;

      let Ripartizione_Trappole: BaseCodeDescr = null;

      let QuantitaSuImpianti: QuantitaSuImpianto[] = null;

      let row: FormGroup = this.gridosiprodottiservice.getRigaGridDosiProdotti(
        DosiProdotti,
        DosiProdottiGridrowId,
        this.FertilizzantiForm,
        null,
        null,
        this.Operazione,
        this.qdcservice.TestataForm.get("Data").value,
        this.FertilizzantiForm.get("flagTipoDose").value,
        this.FertilizzantiForm.get("flagDoseQuantitaTotale").value,
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

      this.qdcservice.AggiornaFormArrayGridDosiProdotti(row.getRawValue(),index,this.Operazione,enum_TipoOperazioneDB.Modifica,null,false);

      if(Nuova_Riga){
        DosiProdotti = DosiProdotti.filter(d=>d.DosiProdottiGridrowId !== DosiProdottiGridrowId);

        tipo_operazione_db = enum_TipoOperazioneDB.Scrittura;
      }

      this.gridcontrolliservice.ControllaSeInserireDoseProdotto(
        DosiProdotti,
        row.getRawValue(),
        row,
        tipo_operazione_db,
        DosiProdottiGridrowId,
        null,
        [],
        []
      ).then(listerroriGias => {

        if(listerroriGias.length === 0){
          this.FertilizzantiForm.patchValue({
            Riga_Salvata: true
          });

          this.FertilizzantiForm.disable({emitEvent: false});

        }
      });
    }


}
