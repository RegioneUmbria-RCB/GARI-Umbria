import { Injectable, OnDestroy } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import { Subscription } from "rxjs";
import { QdCProdottiService } from "../prodotti.service";
import { QdCService } from "../qdc.service";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {
  enum_doseQuantitaTotale,
  enum_SEMINA_TIPO,
  enum_TipoMezzo,
  enum_TipoOperazioneDB
} from "../../../../Model/TipiEnumerativi";
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {
  MultiColumnComboboxDose_Etichetta,
  MultiColumnComboboxSemina
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI} from "../../../../Model/CostantiPersonalizzate";
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";
import {LeggiProdotti, ProdottiService} from "../../../../Service/Anagrafica/prodotti.service";
import {FunzioniComuniService} from "../../../../Service/FunzioniComuni.service";
import {UtilityFunctions} from "../../../../Utility/UtilityFunctions";
import {Epoca} from "../../../../Model/metaschema/Epoca";
import {Disciplinare} from "../../../../Model/metaschema/Disciplinari";
import {GridDosiProdottiService} from "../grid-dosi-prodotti/grid-dosi-prodotti.service";
import {GridDosiProdottiControlliService} from "../grid-dosi-prodotti/grid-dosi-prodotti-controlli.service";
import {QuantitaSuImpianto} from "../../../../Model/attivita/dettagli/QuantitaSuImpianto";

@Injectable()

export class QdCDettagliSementiService implements OnDestroy{


    Subs: Subscription = new Subscription();

    Array_Sementi: Array<MultiColumnComboboxSemina> = [];

    SementiForm: FormGroup;

    Operazione: Lavorazione;

    Prima_Apertura_MultiColumn_Semente = false;

    constructor(private qdcprodottiservice: QdCProdottiService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private qdcservice: QdCService,
                private prodottiservice: ProdottiService,
                private funzionicomuniservice: FunzioniComuniService,
                private gridDosiProdottiService: GridDosiProdottiService,
                private gridcontrolliservice: GridDosiProdottiControlliService,){

        this.Subs.add(this.qdcprodottiservice.ObsProdottiForm.subscribe(form=>{
            this.SementiForm = form;

            this.Operazione = this.SementiForm?.get("Operazione").value;
        }));

    }


    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Letture Lato Server

    async getArray_Sementi(filtrodescr: string) {

        if(this.qdcprodottiservice.Msg_Seleziona_Impianti().length === 0){

            let LeggiProdotti = <LeggiProdotti>{
                lavorazione: this.Operazione,
                specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                filtroPerDescrizione: filtrodescr,
                data: this.qdcservice.TestataForm.get("Data").value,
                escludiGiacenzeZero: this.SementiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").value,
                impianti: this.qdcservice.GetImpiantiSelezionatiModel(),
                tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
                statoAttivita: this.qdcservice.TestataForm.get("Stato").value,
                magazziniAgenzie: this.SementiForm.get("Visualizza_Magazzini_Agenzie").value,
                magazziniEsterni: this.SementiForm.get("Visualizza_Magazzini_Esterni").value,
            };

            this.Array_Sementi = (await this.prodottiservice.Leggi_Sementi(LeggiProdotti)) as MultiColumnComboboxSemina[];

            //Descrizione Concatenata
            let Array_SementiMapped = this.Array_Sementi.map(p=>{
                p = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(SEMENTI,p) as MultiColumnComboboxSemina;
            });

            // Se c'è solo un Prodotto (diversa da quella già impostata nella combo altrimenti scatta la funzione di change)
            // lo imposto come default
            if (this.Array_Sementi.length === 1) {

                let Semente_Selezionata: MultiColumnComboboxSemina = this.SementiForm.get("Prodotto").value;

                if(Semente_Selezionata?.Codice_Concatenato !== this.Array_Sementi[0].Codice_Concatenato){

                    this.SementiForm.patchValue({
                        Prodotto: this.Array_Sementi[0],
                    });

                    await this.changeSemente(this.Array_Sementi[0]);
                }

            }

        }else{
            this.Array_Sementi = [];
        }

        return this.Array_Sementi;
    }

    //Fine Letture Lato Server


    //Oltre ai valori reimposto vuoti gli array dei controlli per la visibilità
    clearSezioneProdottiSementiForm(clearFormArrayDosiProdotti: boolean, clearProdotto: boolean){

        //Non resetto il form se sono in inserimento prodotti e ho già salvato la riga di prodotto
        let clear = true;

        if(!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.SementiForm.get("Riga_Salvata").value)
            clear = false;

        if(clear){
            this.SementiForm.patchValue({
                Sup_Calcolata: 0
            });

            if(clearProdotto){
                this.Array_Sementi = [];

                this.SementiForm.patchValue({
                    Prodotto:null
                });

                this.Prima_Apertura_MultiColumn_Semente = false;
            }


            this.qdcprodottiservice.clearSezioneProdottiForm(clearFormArrayDosiProdotti);
        }

    }

    Mostra_Sup_Calcolata(){

        let mostra = false;

        let Opzione_Semina = this.SementiForm.get("Opzioni_Semina").value;

        if(Opzione_Semina?.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default &&
            this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write){
            mostra = true;
        }

        return mostra;

    }

    async changeSemente(Prodotto_Selezionato: MultiColumnComboboxSemina){

        let obj_errore_gias_qdc = await this.qdcprodottiservice.Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(Prodotto_Selezionato);

        if(obj_errore_gias_qdc && obj_errore_gias_qdc.list_errori_filtrati && obj_errore_gias_qdc.list_errori_filtrati.length > 0){
          this.SementiForm.patchValue({
            Prodotto: null,
          },{emitEvent: false});
          return;
        }

        this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        this.clearSezioneProdottiSementiForm(false,false);

        if(Prodotto_Selezionato){

            if(Prodotto_Selezionato.prodotto.codice !== 0){

                await this.qdcprodottiservice.Imposta_Magazzino_Lotto_dal_Prodotto_Selezionato(Prodotto_Selezionato);

                if(Prodotto_Selezionato.MagazziniMovimentazioni &&
                    Prodotto_Selezionato.MagazziniMovimentazioni.length === 1){

                    let GiacenzaAllaData = Prodotto_Selezionato.MagazziniMovimentazioni[0].Qta;
                    let GiacenzaTotale = Prodotto_Selezionato.MagazziniMovimentazioni[0].QtaTot;

                    if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write){

                        let DoseTot_Ha = 0;

                        let Dose_Ha = 0;

                        let Sup_Calcolata_Per_Frazionamento = 0;

                        let Sup_Trattata = this.qdcservice.SuperficiForm.get("Sup_Trattata").value;

                        let UdM = this.SementiForm.get("UdM").value;

                        switch((this.SementiForm.get("Opzioni_Semina").value as BaseCodeDescr).codice){
                            case enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default:
                                //suggerisco la sup_calcolata (= sup_totale - quella già inserita)

                                let dosiProdotti: Array<any> = [];

                                if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                                    dosiProdotti = this.SementiForm.get("DosiProdotti").getRawValue();
                                }else{
                                    dosiProdotti = this.qdcservice.DosiProdotticonRigheSalvate(null,this.Operazione);
                                }

                                let Sup_Gia_Seminata: number = 0;

                                dosiProdotti.forEach(d=>{
                                    Sup_Gia_Seminata += d.Sup_Calcolata;
                                });

                                Sup_Calcolata_Per_Frazionamento = Sup_Trattata - Sup_Gia_Seminata;

                                if(GiacenzaAllaData >= 0){
                                    DoseTot_Ha = GiacenzaAllaData;

                                    if(GiacenzaAllaData > 0 && Sup_Calcolata_Per_Frazionamento > 0){
                                        Dose_Ha = this.funzionicomuniservice.roundNumber(GiacenzaAllaData/Sup_Calcolata_Per_Frazionamento,this.qdcservice.getProductNumericSettings(UdM,true).decimals);
                                    }
                                }

                                break;
                            default:

                                if(GiacenzaAllaData >= 0){

                                    DoseTot_Ha = GiacenzaAllaData;

                                    if(GiacenzaAllaData > 0 && Sup_Trattata > 0){
                                        Dose_Ha = this.funzionicomuniservice.roundNumber(GiacenzaAllaData/Sup_Trattata,this.qdcservice.getProductNumericSettings(UdM,true).decimals);
                                    }

                                }


                                break;
                        }

                        this.SementiForm.patchValue({
                            Dose_Ha: Dose_Ha,
                            DoseTot_Ha: DoseTot_Ha,
                            Sup_Calcolata: Sup_Calcolata_Per_Frazionamento,
                            flagDoseQuantitaTotale: enum_doseQuantitaTotale.Qta_Totale,
                            flagTipoDose: enum_TipoMezzo.Ettaro
                        });
                    }

                    this.qdcprodottiservice.Aggiorna_UdM(Prodotto_Selezionato.MagazziniMovimentazioni[0].udm);

                }
            }

        }
    }

    setArrayDDLSezioneProdottiSementiForm(dataItem: any){

        //Aggiorno anche gli Array che sono nel prodotti service così scattano gli ngIf che permettono
        //la visualizzazione di alcune ddl solo se hanno degli elemeneti

        this.qdcprodottiservice.setArrayDDLSezioneProdottiForm(dataItem);

        this.Array_Sementi = [];

        if(dataItem.Prodotto && dataItem.Prodotto.prodotto.codice > 0)
            this.Array_Sementi = [dataItem.Prodotto];

    }

    RicercaSementi(){
        let Prodotto = this.SementiForm.get("Prodotto").value;

        let descrizione = "";

        if(Prodotto){

            if(Prodotto.prodotto.codice === this.qdcservice.Obj_Empty_MultiColumnComboboxSemina.prodotto.codice){
                descrizione = Prodotto.Descrizione_Concatenata;
            }else{
                descrizione = Prodotto.prodotto.descrizione;
            }

        }

        let listerroriGias = this.qdcprodottiservice.RicercaProdotti(descrizione);

        if(listerroriGias && listerroriGias.length > 0){
            this.Array_Sementi = [];
        }else{

            UtilityFunctions.loadDropDownItems(this.qdcprodottiservice.MultiColumnComboboxProdotti,this.getArray_Sementi(descrizione));

            if(!this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.isOpen){
                this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.focus();
                this.qdcprodottiservice.MultiColumnComboboxProdotti.Multicolumncombobox.toggle(true);
            }
        }
    }

    SalvaDoseSemente(Nuova_Riga: boolean){

      let EpocaDPI: Epoca = null;

      let Array_Dosi: Array<MultiColumnComboboxDose_Etichetta> = null;

      let EpocaFertilizzazione: Epoca = null;

      let Utilizza_Direttiva_Nitrati: boolean = false;

      let Direttiva_Nitrati: Disciplinare = null;

      let Opzioni_Semina: BaseCodeDescr = this.SementiForm.get("Opzioni_Semina").value;

      let tipo_operazione_db = enum_TipoOperazioneDB.Modifica;

      let DosiProdottiGridrowId =  this.SementiForm.getRawValue().DosiProdottiGridrowId;

      let DosiProdottiFormArray = this.qdcservice.DosiProdottiFormArray(null,this.Operazione);

      let DosiProdotti:Array<any> = DosiProdottiFormArray.getRawValue();

      let Modalita_Applicazione: BaseCodeDescr = null;

      let Ripartizione_Trappole: BaseCodeDescr = null;

      let QuantitaSuImpianti: QuantitaSuImpianto[] = null;

      let row: FormGroup = this.gridDosiProdottiService.getRigaGridDosiProdotti(
        DosiProdotti,
        DosiProdottiGridrowId,
        this.SementiForm,
        null,
        null,
        this.Operazione,
        this.qdcservice.TestataForm.get("Data").value,
        this.SementiForm.get("flagTipoDose").value,
        this.SementiForm.get("flagDoseQuantitaTotale").value,
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
        null,[],[]).then(listerroriGias => {

        if(listerroriGias.length === 0){
          this.SementiForm.patchValue({
            Riga_Salvata: true
          });

          this.SementiForm.disable({emitEvent: false});


        }
      });
    }

}
