import { Injectable, OnDestroy, Optional } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FERTILIZZANTI } from "app/Model/CostantiPersonalizzate";
import { UnitaDiMisura } from "app/Model/metaschema/UnitaDiMisura";
import { enum_doseQuantitaTotale, enum_TipoMezzo, enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { Subscription } from "rxjs";
import { QdCDettagliFertilizzantiService } from "./prodotti/dettagli-fertilizzanti.service";
import { QdCService } from "./qdc.service";
import {Epoca} from "../../../Model/metaschema/Epoca";
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";

@Injectable()
export class QdCUnitadiMisuraService implements OnDestroy{

    //Valore precedente della DDL Unita di Misura nella Form fuori la griglia dei dosaggi
    _unita:UnitaDiMisura;

    //Valore precedente della DDL Unita di Misura dentro la griglia dei dosaggi
    _unita_grid:UnitaDiMisura;

    Subs: Subscription = new Subscription();

    constructor(@Optional() private qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
                private funzionicomuniservice: FunzioniComuniService,
                private qdcservice: QdCService){
    }

    //Questa funzione di change viene richiamata sia dal cambio dell'Udm fuori dalla griglia sia dal cambio
    //Udm dentro la griglia
    changeUdM(unitaDiMisura: UnitaDiMisura, Form: FormGroup, flag_UdM_Grid:boolean){

        //Replica il AggiornaDOSI_con_unita nell'OperazioneBootstrap
        this.AggiornaDOSI_con_unita(unitaDiMisura,Form,flag_UdM_Grid);


        //Sub Cmb_UdM_SelectedIndexChanged nella Trattamenti_2
        if(Form.get("Categoria_Magazzino").value === FERTILIZZANTI){

            //Recupero l'epoca fertilizzazione
            let EpocaFertilizzazione:Epoca = null;

            if(flag_UdM_Grid){
                EpocaFertilizzazione = this.qdcservice.Sezioni_ProdottoFormArray?.getRawValue().find(s=>+ s.Operazione.primaryKey.codice === + Form.get("Operazione").value.primaryKey.codice)?.EpocaFertilizzazione;
            }else{
                EpocaFertilizzazione = Form.get("EpocaFertilizzazione").getRawValue();
            }

            //Evito di reimpostare l'Efficienza a 1 se è abilitata la textbox di edit manuale dell Efficienza
            if(!this.qdcservice.Abilita_Modifica_Efficienza_Fertilizzanti(Form))
              this.qdcdettaglifertilizzantiservice.Gestisci_Efficienza_N_Utile(Form, EpocaFertilizzazione).then();
        }


    }


    AggiornaDOSI_con_unita(unitaDiMisura: UnitaDiMisura, Form: FormGroup,flag_UdM_Grid: boolean){

        let flagTipoDose = Form.get("flagTipoDose").value;
        let flagDoseQuantitaTotale = Form.get("flagDoseQuantitaTotale").value;
        let _SupTrattata = this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare
            ? this.qdcservice.SuperficiForm.get("Sup_Trattata").value
            : this.qdcservice.QuantitaForm.get("Qta_Trattata").value;
        let _Acqua =  this.qdcservice.AcquaForm.get("Acqua_Tot").value;
        let unit = this.Get_UnitaDiMisura(unitaDiMisura,flag_UdM_Grid);

        let valTot = Form.get("DoseTot_Ha").value;

        valTot = valTot.toFixed(6) * unit;

        valTot = this.funzionicomuniservice.roundNumber(valTot,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals);

        Form.patchValue({
            DoseTot_Ha: valTot
        },{emitEvent: false});

        if (flagTipoDose === enum_TipoMezzo.Ettaro || flagTipoDose === enum_TipoMezzo.Quintale) {

            let val =  Form.get("Dose_Ha").value;
            val = val.toFixed(6) * unit;
            val = this.funzionicomuniservice.roundNumber(val,this.qdcservice.getProductNumericSettings(unitaDiMisura,true).decimals);

            Form.patchValue({
                Dose_Ha: val
            },{emitEvent: false});

            //LAVORO A HA
            if(flagDoseQuantitaTotale === enum_doseQuantitaTotale.Qta_Totale){
                //LAVORO A TAL QUALE
                if(_SupTrattata > 0){
                    let _DoseTotaleHA = Form.get("DoseTot_Ha").value;
                    let _DoseHA = _DoseTotaleHA / _SupTrattata;

                    Form.patchValue({
                        Dose_Ha: this.funzionicomuniservice.roundNumber(_DoseHA,this.qdcservice.getProductNumericSettings(unitaDiMisura,true).decimals)
                    },{emitEvent: false});

                    //VERIFICO L'acqua
                    if (_Acqua > 0) {
                        let _DoseHL = _DoseTotaleHA / _Acqua;

                        Form.patchValue({
                            Dose_Hl: this.funzionicomuniservice.roundNumber(_DoseHL,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals)
                        },{emitEvent: false});
                    }else if(_Acqua == 0){

                        Form.patchValue({
                            Dose_Hl: 0
                        },{emitEvent: false});
                    }

                }
            } else {

                //LAVORO A DOSE
                if (_SupTrattata > 0) {
                    let _DoseHA = Form.get("Dose_Ha").value;
                    let _DoseTotaleHA = _SupTrattata * _DoseHA;

                    Form.patchValue({
                        DoseTot_Ha: this.funzionicomuniservice.roundNumber(_DoseTotaleHA,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals)
                    },{emitEvent: false});

                    //VERIFICO L'acqua
                    if (_Acqua > 0) {
                        let _DoseHL = _DoseTotaleHA / _Acqua;

                        Form.patchValue({
                            Dose_Hl: this.funzionicomuniservice.roundNumber(_DoseHL,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals)
                        },{emitEvent: false});
                    }else if (_Acqua == 0) {
                        Form.patchValue({
                            Dose_Hl: 0
                        },{emitEvent: false});
                    }
                }
            }

        }else{

            let val = Form.get("Dose_Hl").value;
            val = val.toFixed(6) * unit;
            val = this.funzionicomuniservice.roundNumber(val,this.qdcservice.getProductNumericSettings(unitaDiMisura,true).decimals);
            Form.patchValue({
                Dose_Hl: val
            },{emitEvent: false});

            //LAVORO A HL
            if (flagDoseQuantitaTotale === enum_doseQuantitaTotale.Qta_Totale) {
                //LAVORO A TAL QUALE
                if (_Acqua > 0) {
                    let _DoseTotaleHL = Form.get("DoseTot_Ha").value;
                    let _DoseHL = _DoseTotaleHL / _Acqua;
                    Form.patchValue({
                        Dose_Hl: this.funzionicomuniservice.roundNumber(_DoseHL,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals)
                    },{emitEvent: false});
                    //VERIFICO L'acqua
                    if (_SupTrattata > 0) {
                        let _DoseHA = _DoseTotaleHL / _SupTrattata;

                        Form.patchValue({
                            Dose_Ha: this.funzionicomuniservice.roundNumber(_DoseHA,this.qdcservice.getProductNumericSettings(unitaDiMisura,true).decimals)
                        },{emitEvent: false});
                    }
                }else if (_Acqua == 0) {
                    Form.patchValue({
                        Dose_Hl: 0
                    },{emitEvent: false});
                }
            }
            else {
                //LAVORO A DOSE
                if (_Acqua > 0) {
                    let _DoseHL = Form.get("Dose_Hl").value;
                    let _DoseTotaleHL = _Acqua * _DoseHL;

                    //VERIFICO L'acqua
                    if (_SupTrattata > 0) {
                        Form.patchValue({
                            DoseTot_Ha: this.funzionicomuniservice.roundNumber(_DoseTotaleHL,this.qdcservice.getProductNumericSettings(unitaDiMisura).decimals)
                        },{emitEvent: false});

                        let _DoseHA = _DoseTotaleHL / _SupTrattata;

                        Form.patchValue({
                            Dose_Ha: this.funzionicomuniservice.roundNumber(_DoseHA,this.qdcservice.getProductNumericSettings(unitaDiMisura,true).decimals)
                        },{emitEvent: false});
                    }
                }else if (_Acqua == 0) {
                    Form.patchValue({
                        Dose_Ha: 0
                    },{emitEvent: false});
                }
            }
        }

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(Form.get("DosiProdottiGridrowId").value);
    }

    // funzione che in base all'unità di misura selezionata definisce il moltiplicatore
    Get_UnitaDiMisura(unita: UnitaDiMisura,flag_UdM_Grid: boolean): number {

        let final = 0;

        if(flag_UdM_Grid){

            //Lo imposto a 1 perchè dentro la Grid l'utente può inserire la stessa unita di misura
            //ma può cambiare da ha a hl e viceversa mentre fuorti la Grid no
            final = 1;

            if (this._unita_grid?.codice != unita.codice) {

                final = this.Get_Moltiplicatore(this._unita,unita);

                this._unita_grid = unita;
            }

        }else{

            if (this._unita && unita && this._unita?.codice != unita?.codice) {

                final = this.Get_Moltiplicatore(this._unita,unita);
            }else{
                final = 1;
            }

            this._unita = unita;

        }

        return final;
    }

    Get_Moltiplicatore(prev_unita:UnitaDiMisura,new_unita: UnitaDiMisura): number{

      let moltiplicatore_base = this.Get_Moltiplicatore_Base(prev_unita);

      let moltiplicatore = this.Get_Moltiplicatore_Base(new_unita);

      return moltiplicatore_base / moltiplicatore;
    }

    private Get_Moltiplicatore_Base(unita:UnitaDiMisura){

        let moltiplicatore = 0;

        if(unita){
            switch(unita.codice) {
                case enum_UnitaMisura.KG:
                    // kg
                    moltiplicatore = 1;
                    break;
                case enum_UnitaMisura.Grammi:
                    // gr
                    moltiplicatore = 0.001;
                    break;
                case enum_UnitaMisura.Quintali:
                    // q
                    moltiplicatore = 100;
                    break;
                case enum_UnitaMisura.Tonnellate:
                    // t
                    moltiplicatore = 1000;
                    break;
                case enum_UnitaMisura.Litri:
                    // l
                    moltiplicatore = 1;
                    break;
                case enum_UnitaMisura.CentimetriCubi:
                    // cc
                    moltiplicatore = 0.001;
                    break;
                case enum_UnitaMisura.Millilitri:
                    // ml
                    moltiplicatore = 0.001;
                    break;
                case enum_UnitaMisura.Unita_Seme:
                    //
                    moltiplicatore = 1;
                    break;
                case enum_UnitaMisura.Num_Piante:
                    //
                    moltiplicatore = 1;
                    break;
                case enum_UnitaMisura.Confezioni:
                    //
                    moltiplicatore = 1;
                    break;
                case enum_UnitaMisura.Metri_Cubi:
                    // m3
                    moltiplicatore = 1000;
                    break;
            }
        }


        return moltiplicatore;
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }
}
