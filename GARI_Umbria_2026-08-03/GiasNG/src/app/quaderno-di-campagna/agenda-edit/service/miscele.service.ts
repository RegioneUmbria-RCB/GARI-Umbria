import { Injectable } from "@angular/core";
import { FormArray, FormGroup} from '@angular/forms';
import { DoseAcqua } from "app/Model/attivita/risorse/RisorsaAcqua";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { SEMENTI } from "app/Model/CostantiPersonalizzate";
import { enum_doseQuantitaTotale, enum_LAVCOD, enum_SEMINA_TIPO, enum_TipoMezzo, SIMBOLO_M3_HA, SIMBOLO_MM } from "app/Model/TipiEnumerativi";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import {QdCService} from "./qdc.service";
import {Lavorazione} from "../../../Model/attivita/Lavorazione";
import {
  GridImpiantoSelezionatoModel,
  Obj_Dose_Ha_Hl,
  Obj_Dose_Ha_Tot,
  Obj_Dose_Hl_Tot, Sezione_Prodotto, Superfici
} from "../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";
import {QuantitaSuImpianto} from "../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {enum_Ripartizione_Trappole} from "../../../Model/attivita/dettagli/DettaglioTrattamento";
import { ErroreGias_Severity, ErroreGias } from "app/Service/master.service";
import { TranslocoService } from "@jsverse/transloco";
import { CodiceDescrizione } from "app/Utility/UtilityFunctions";
import {UnitaDiMisura} from "../../../Model/metaschema/UnitaDiMisura";
import { IrrigazioneAcquaService } from "./irrigazione-acqua.service";

@Injectable()
export class MisceleService {

    constructor(
        private funzionicomuniservice: FunzioniComuniService,
        private qdcservice: QdCService,
        private translocoService: TranslocoService,
        private irrigazioneAcquaService: IrrigazioneAcquaService
    ) {  }

    public calcoloMiscele(campo: string, sezione_prodottoForm: FormGroup, Sup_Calcolata_per_Frazionamento: number, new_value?: number) {

        //Verifico se è un'Operazione o multi Operazioni con Acqua
        let acqua: boolean = this.qdcservice.mostraAcqua();

        let Opzione_Semina: BaseCodeDescr = null;

        if(sezione_prodottoForm){
            let Operazione = sezione_prodottoForm.get("Operazione").value;
            Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(Operazione);
        }

        switch (campo) {
            case'Sup_Trattata' : return this.sup_trattata_change(this.qdcservice.QdCForm, acqua, new_value);
            case'changeDose_Ha' : return this.dose_ha_change(this.qdcservice.QdCForm, sezione_prodottoForm, acqua, Opzione_Semina, Sup_Calcolata_per_Frazionamento)
            case'changeDose_Hl' : return this.dose_hl_change(this.qdcservice.QdCForm, sezione_prodottoForm, acqua)
            case'changeDoseTot_Ha' : return this.dose_tot_ha_change(this.qdcservice.QdCForm, sezione_prodottoForm, acqua, Opzione_Semina, Sup_Calcolata_per_Frazionamento)
            case'changeAcqua_Ha' : return this.acqua_ha_change(this.qdcservice.QdCForm);
            case'changeAcqua_Tot' : return this.acqua_tot_change(this.qdcservice.QdCForm);
            //case'changeSup_Calcolata' : return this.misceleservice.sup_calcolata_change(this.QdCForm)
        }
    }

    // ------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------

    public sup_trattata_change(QdCForm: FormGroup, acqua: boolean, new_sup?: number) {

        const Sup_Trattata = this.getSupTrattata(new_sup, QdCForm);

        // ---------------------------- ACQUA ----------------------------
        if (acqua) {
            if (QdCForm.get('Trattamento.Acqua.Dose_Acqua').value == DoseAcqua.TOTALE) {
                const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
                if (Sup_Trattata == 0) {
                    QdCForm.get('Trattamento.Acqua').patchValue({
                        Acqua_Ha: 0
                    });
                } else {
                    QdCForm.get('Trattamento.Acqua').patchValue({
                        Acqua_Ha: this.calculateAcquaHa(Acqua_Tot, Sup_Trattata)
                    });
                }
            } else {
                const Acqua_Ha = QdCForm.get('Trattamento.Acqua').value['Acqua_Ha'];
                if (Sup_Trattata == 0) {
                    QdCForm.get('Trattamento.Acqua').patchValue({
                        Acqua_Tot: 0
                    });
                } else {
                    QdCForm.get('Trattamento.Acqua').patchValue({
                        Acqua_Tot: this.calculateAcquaTot(Sup_Trattata, Acqua_Ha)
                    });
                }
            }
        }

        // ---------------------------- DOSI ----------------------------
        //Cambio i Dosaggi dei Prodotti che non sono ancora stati salvati
        if (QdCForm.get('Trattamento.Sezioni_Prodotto') && QdCForm.get('Trattamento.Sezioni_Prodotto').value.length > 0) {

            const fa_SezioniProdotto = (QdCForm.controls['Trattamento'] as FormGroup).controls['Sezioni_Prodotto'] as FormArray;

            if (QdCForm.get('Trattamento.Acqua.Dose_Acqua').value == DoseAcqua.HA) {
                // AcquaHa fissa
                if(acqua){
                    fa_SezioniProdotto.controls.forEach((element: FormGroup, index) => {

                        if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                            if (element.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                                // DoseTotaleHa fissa
                                this.changeDoseHl(element);
                                this.changeDoseHa(element, new_sup);
                            } else {
                                // if (element.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                                //     // DoseHl fissa
                                //     this.changeDoseTotHa(element, new_sup);
                                // } else {
                                //     // DoseHa fissa
                                //     this.changeDoseTotHa(element, new_sup);
                                // }
                              this.changeDoseTotHa(element, new_sup);
                            }
                        }else{
                            (element.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {
                                if(fc.controls['Riga_Salvata'].value === false){
                                    if (fc.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                                        // DoseTotaleHa fissa
                                        this.changeDoseHl(fc);
                                        this.changeDoseHa(fc, new_sup);
                                    } else {
                                        // if (fc.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                                        //     // DoseHl fissa
                                        //     this.changeDoseTotHa(fc, new_sup);
                                        // } else {
                                        //     // DoseHa fissa
                                        //     this.changeDoseTotHa(fc, new_sup);
                                        // }
                                      this.changeDoseTotHa(fc, new_sup);
                                    }
                                }
                            });
                        }
                    });
                }
            } else {
              fa_SezioniProdotto.controls.forEach((element: FormGroup, index) => {

                  if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                      if (element.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                          // DoseTotaleHa fissa
                          this.changeDoseHa(element, new_sup);
                      } else {
                          if (element.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                              // DoseHl fissa
                              this.changeDoseHa(element, new_sup);
                          } else {
                              // DoseHa fissa
                              this.changeDoseTotHa(element, new_sup);
                              this.changeDoseHl(element);
                          }
                      }
                  }else{
                      (element.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {

                          if(fc.controls['Riga_Salvata'].value === false){
                              if (fc.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                                  // DoseTotaleHa fissa
                                  this.changeDoseHa(fc, new_sup);
                              } else {
                                  if (fc.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                                      // DoseHl fissa
                                      this.changeDoseHa(fc, new_sup);
                                  } else {
                                      // DoseHa fissa
                                      this.changeDoseTotHa(fc, new_sup);
                                      this.changeDoseHl(fc);
                                  }
                              }
                          }
                      });
                  }
              });
            }

            this.SupTrattata_Ha_Prodotti_change(QdCForm,  new_sup);
             // Update irrigation data in the grid when water values change

        }
    }

    public acqua_ha_change(QdCForm: FormGroup) {
        const Acqua_Ha = QdCForm.get('Trattamento.Acqua').value['Acqua_Ha'];
        const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);
        const fa_SezioniProdotto = (QdCForm.controls['Trattamento'] as FormGroup).controls['Sezioni_Prodotto'] as FormArray;

        QdCForm.get('Trattamento.Acqua').patchValue({
            Dose_Acqua: DoseAcqua.HA,
            Acqua_Tot: this.calculateAcquaTot(Sup_Trattata, Acqua_Ha)
        });

        //Cambio i Dosaggi dei Prodotti che non sono ancora stati salvati
        if (QdCForm.get('Trattamento.Sezioni_Prodotto') != undefined) {
            const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];

            fa_SezioniProdotto.controls.forEach((element: FormGroup, index) => {

                if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                    if (element.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                        //------------------------------------------------------------------------------------------------------------------
                        const Dose_Hl = element.getRawValue().Dose_Hl;
                        const UdM = element.getRawValue().UdM;
                        element.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM), {emitEvent: false});
                        //------------------------------------------------------------------------------------------------------------------
                        this.changeDoseHa(element);
                    } else {
                        this.changeDoseHl(element);
                    }
                }else{
                    (element.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {
                        if(fc.controls['Riga_Salvata'].value === false){

                            if (fc.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                                //------------------------------------------------------------------------------------------------------------------
                                const Dose_Hl = fc.getRawValue().Dose_Hl;
                                const UdM = element.getRawValue().UdM;
                                fc.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM), {emitEvent: false});
                                //------------------------------------------------------------------------------------------------------------------
                                this.changeDoseHa(fc);
                            } else {
                                this.changeDoseHl(fc);
                            }
                        }
                    });
                }

            });
        }

        this.Acqua_Prodotti_change(QdCForm);

        // Update irrigation data in the grid when water values change
        this.updateGridIrrigationData(QdCForm);
    }

    public acqua_tot_change(QdCForm: FormGroup) {
        const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
        const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);
        const fa_SezioniProdotto = (QdCForm.controls['Trattamento'] as FormGroup).controls['Sezioni_Prodotto'] as FormArray;

        QdCForm.get('Trattamento.Acqua').patchValue({
            Dose_Acqua: DoseAcqua.TOTALE,
            Acqua_Ha: this.calculateAcquaHa(Acqua_Tot, Sup_Trattata)
        });

        //Cambio i Dosaggi dei Prodotti che non sono ancora stati salvati
        if (QdCForm.get('Trattamento.Sezioni_Prodotto') != undefined) {
            fa_SezioniProdotto.controls.forEach((element: FormGroup, index) => {

                if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                    if (element.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                        //------------------------------------------------------------------------------------------------------------------
                        const Dose_Hl = element.getRawValue().Dose_Hl;
                        const UdM = element.getRawValue().UdM;
                        element.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM), {emitEvent: false});
                        //------------------------------------------------------------------------------------------------------------------
                        this.changeDoseHa(element);
                    } else {
                        this.changeDoseHl(element);
                    }
                }else{
                    (element.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {
                       if(fc.controls['Riga_Salvata'].value === false){
                            if (fc.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
                                //------------------------------------------------------------------------------------------------------------------
                                const Dose_Hl = fc.getRawValue().Dose_Hl;
                                const UdM = element.getRawValue().UdM;
                                fc.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM), {emitEvent: false});
                                //------------------------------------------------------------------------------------------------------------------
                                this.changeDoseHa(fc);
                            } else {
                                this.changeDoseHl(fc);
                            }
                        }
                    });
                }


            });
        }

        this.Acqua_Prodotti_change(QdCForm);

        // Update irrigation data in the grid when water values change
        this.updateGridIrrigationData(QdCForm);
    }

    //Questa funzione di change viene richiamata sia dal cambio della Dose Ha fuori dalla griglia sia dal cambio
    //Dose Ha dentro la griglia
    public dose_ha_change(QdCForm: FormGroup, Form: FormGroup, acqua: boolean, Opzione_Semina: BaseCodeDescr, Sup_Calcolata_per_Frazionamento: number) {

        Form.controls['flagDoseQuantitaTotale'].setValue(enum_doseQuantitaTotale.Dose, {emitEvent: false});
        Form.controls['flagTipoDose'].setValue(enum_TipoMezzo.Ettaro, {emitEvent: false});

        if(
            Form.get("Categoria_Magazzino").value === SEMENTI &&
            Opzione_Semina &&
            Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default
        ) {

            this.sup_calcolata_change(Form,Sup_Calcolata_per_Frazionamento);

        } else {

            const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);
            const Dose_Ha = Form.getRawValue().Dose_Ha;
            const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
            const UdM = Form.getRawValue().UdM;

            let obj_Dose_Hl_Tot = this.calcola_dose_hl_tot_ha_da_dose_ha(Form.controls['Dose_Hl'].value,Form.controls['DoseTot_Ha'].value,
                                                                                                        Dose_Ha, Sup_Trattata, acqua,Acqua_Tot, Form.controls['Operazione'].value,UdM)

            Form.controls['DoseTot_Ha'].setValue(obj_Dose_Hl_Tot.DoseTot_Ha, {emitEvent: false});

            Form.controls['Dose_Hl'].setValue(obj_Dose_Hl_Tot.Dose_Hl, {emitEvent: false});

            this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(Form.getRawValue().DosiProdottiGridrowId);
        }

    }

    public calcola_dose_hl_tot_ha_da_dose_ha (Dose_Hl:number,DoseTot_Ha: number,Dose_Ha: number,Sup_Trattata: number, flag_acqua: boolean,
                                              Acqua_Tot: number,Operazione: Lavorazione, Udm: UnitaDiMisura): Obj_Dose_Hl_Tot{

        let obj_Dose_Hl_Tot: Obj_Dose_Hl_Tot = {
            Dose_Hl: Dose_Hl,
            DoseTot_Ha: DoseTot_Ha
        };

        obj_Dose_Hl_Tot.DoseTot_Ha = this.calculateQtaTotale(Dose_Ha, Sup_Trattata,Udm);

        if(flag_acqua){

            if(this.qdcservice.mostraDose_Hl(Operazione)){
                if (Acqua_Tot == 0) {
                    obj_Dose_Hl_Tot.Dose_Hl = this.calculateDoseHl(0, 1,Udm);
                } else {
                    obj_Dose_Hl_Tot.Dose_Hl = this.calculateDoseHl(obj_Dose_Hl_Tot.DoseTot_Ha, Acqua_Tot,Udm);
                }
            }

        }

        return obj_Dose_Hl_Tot;
    }

    //Questa funzione di change viene richiamata sia dal cambio della Dose Hl fuori dalla griglia sia dal cambio
    //Dose Hl dentro la griglia
    public dose_hl_change(QdCForm: FormGroup, Form: FormGroup, acqua: boolean) {
        const Dose_Hl = Form.getRawValue().Dose_Hl;
        const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);
        const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
        const UdM = Form.getRawValue().UdM;

        Form.controls['flagDoseQuantitaTotale'].setValue(enum_doseQuantitaTotale.Dose, {emitEvent: false});
        Form.controls['flagTipoDose'].setValue(enum_TipoMezzo.Ettolitro, {emitEvent: false});

        let obj_Dose_Ha_Tot = this.calcola_dose_ha_tot_ha_da_dose_hl(Form.getRawValue().Dose_Ha,Form.getRawValue().DoseTot_Ha,
                                                                                        Dose_Hl,Sup_Trattata,acqua,Acqua_Tot,UdM);

        Form.controls['DoseTot_Ha'].setValue(obj_Dose_Ha_Tot.DoseTot_Ha, {emitEvent: false});

        Form.controls['Dose_Ha'].setValue(obj_Dose_Ha_Tot.Dose_Ha, {emitEvent: false});

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(Form.controls['DosiProdottiGridrowId'].getRawValue());
    }

    public calcola_dose_ha_tot_ha_da_dose_hl (Dose_Ha:number,DoseTot_Ha: number,Dose_Hl: number,Sup_Trattata: number,
                                              flag_acqua: boolean, Acqua_Tot: number, UdM: UnitaDiMisura): Obj_Dose_Ha_Tot{

        let obj_Dose_Ha_Tot: Obj_Dose_Ha_Tot = {
            Dose_Ha: Dose_Ha,
            DoseTot_Ha: DoseTot_Ha
        };

        if (flag_acqua) {
            obj_Dose_Ha_Tot.DoseTot_Ha = this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM);
        }

        if (Sup_Trattata == 0) {
            obj_Dose_Ha_Tot.Dose_Ha = this.calculateDoseHa(0, 1,UdM);
        } else {
            obj_Dose_Ha_Tot.Dose_Ha = this.calculateDoseHa(obj_Dose_Ha_Tot.DoseTot_Ha, Sup_Trattata,UdM);
        }

        return obj_Dose_Ha_Tot;
    }

    //Questa funzione di change viene richiamata sia dal cambio della Dose Totale fuori dalla griglia sia dal cambio
    //Dose Totale dentro la griglia
    public dose_tot_ha_change(QdCForm: FormGroup, Form: FormGroup, acqua: boolean, Opzione_Semina: BaseCodeDescr , Sup_Calcolata_per_Frazionamento: number) {

        const DoseTot_Ha = Form.getRawValue().DoseTot_Ha;

        Form.controls['flagDoseQuantitaTotale'].setValue(enum_doseQuantitaTotale.Qta_Totale, {emitEvent: false});
        // si mette flagTipoDose = 1 di default, ma non ha alcun valore se flagDoseQuantitaTotale = 10
        Form.controls['flagTipoDose'].setValue(enum_TipoMezzo.Ettaro, {emitEvent: false});

        if(Form.get("Categoria_Magazzino").value === SEMENTI &&
            Opzione_Semina &&
            Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default){

            this.sup_calcolata_change(Form,Sup_Calcolata_per_Frazionamento);

        }else{

            const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);

            const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];

            const UdM = Form.getRawValue().UdM;

            let obj_Dose_ha_hl = this.calcola_dose_ha_hl_da_dose_tot(Form.controls['Dose_Ha'].value,Form.controls['Dose_Hl'].value,Sup_Trattata,
                                                                                                acqua,Acqua_Tot,DoseTot_Ha,Form.controls['Operazione'].value,UdM);

            Form.controls['Dose_Ha'].setValue(obj_Dose_ha_hl.Dose_Ha, {emitEvent: false});


            Form.controls['Dose_Hl'].setValue(obj_Dose_ha_hl.Dose_Hl, {emitEvent: false});

            this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(Form.controls['DosiProdottiGridrowId'].getRawValue());
        }

    }

    public calcola_dose_ha_hl_da_dose_tot(Dose_Ha:number,Dose_Hl:number,Sup_Trattata: number,
                                          flag_acqua: boolean,Acqua_Tot:number,
                                          DoseTot_Ha: number, Operazione: Lavorazione,UdM: UnitaDiMisura): Obj_Dose_Ha_Hl{

        let obj_Dose_Ha_Hl: Obj_Dose_Ha_Hl = {
            Dose_Ha: Dose_Ha,
            Dose_Hl: Dose_Hl
        };

        if (Sup_Trattata == 0) {
            obj_Dose_Ha_Hl.Dose_Ha = this.calculateDoseHa(0, 1,UdM);
        } else {
            obj_Dose_Ha_Hl.Dose_Ha = this.calculateDoseHa(DoseTot_Ha, Sup_Trattata,UdM);
        }

        if (flag_acqua) {

            if(this.qdcservice.mostraDose_Hl(Operazione)){
                if (Acqua_Tot == 0) {
                    obj_Dose_Ha_Hl.Dose_Hl = this.calculateDoseHl(0, 1,UdM);
                } else {
                    obj_Dose_Ha_Hl.Dose_Hl = this.calculateDoseHl(DoseTot_Ha, Acqua_Tot,UdM);
                }
            }

        }

        return obj_Dose_Ha_Hl;
    }

    // ------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------


    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    //Richiamo questa funzione anche nel caso in cui cambio la Dose_ha e la Dose_Totale perchè così vengono ricalcolati
    public sup_calcolata_change(Form: FormGroup,_SupCalcolata: number){

            let flagTipoDose: number = Form.get("flagTipoDose").value;
            let flagDoseQuantitaTotale: number = Form.get("flagDoseQuantitaTotale").value;

            let _DoseTotaleHA = Form.get("DoseTot_Ha").value;
            let _DoseHA = Form.get("Dose_Ha").value;

            let _UdM = Form.get("UdM").value;

            if(flagDoseQuantitaTotale === enum_doseQuantitaTotale.Qta_Totale){

                if (_SupCalcolata != 0 && _DoseTotaleHA != 0) {
                    Form.controls['Dose_Ha'].setValue(this.calculateDoseHa(_DoseTotaleHA,_SupCalcolata,_UdM), {emitEvent: false});

                    this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(Form.controls['DosiProdottiGridrowId'].getRawValue());
                }

            }else{

                if(flagTipoDose === enum_TipoMezzo.Ettaro){
                    if (_SupCalcolata != 0 && _DoseTotaleHA != 0) {
                        Form.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(_DoseHA,_SupCalcolata,_UdM), {emitEvent: false});
                    }
                }

            }

            if(this.qdcservice.mostraDose_Hl(Form.controls['Operazione'].value)){
                Form.controls['Dose_Hl'].setValue(0, {emitEvent: false});
            }

    }

    private changeDoseHa(element: FormGroup, new_sup?: number): void {
        const DoseTot_Ha = element.getRawValue().DoseTot_Ha;
        const Sup_Trattata = this.getSupTrattata(new_sup, this.qdcservice.QdCForm);

        if (Sup_Trattata == 0) {
            element.controls['Dose_Ha'].setValue(0, {emitEvent: false});

            this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(element.controls['DosiProdottiGridrowId'].value);
        } else {

            const UdM = element.getRawValue().UdM;

            element.controls['Dose_Ha'].setValue(this.calculateDoseHa(DoseTot_Ha, Sup_Trattata,UdM), {emitEvent: false});
        }
    }

    private changeDoseHl(element: FormGroup): void {
        const DoseTot_Ha = element.getRawValue().DoseTot_Ha;
        const Acqua_Tot = this.qdcservice.QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];

        if(this.qdcservice.mostraDose_Hl(element.controls['Operazione'].value)){
            if (Acqua_Tot == 0) {
                element.controls['Dose_Hl'].setValue(0, {emitEvent: false});
            } else {
                const UdM = element.getRawValue().UdM;
                element.controls['Dose_Hl'].setValue(this.calculateDoseHl(DoseTot_Ha, Acqua_Tot,UdM), {emitEvent: false});
            }
        }
    }

    private changeDoseTotHa(element: FormGroup, new_sup?: number): void {
        /// se DoseHl fissa doseTotHa = AcquaTot * DoseHl
        /// se DoseHa fissa doseTotHa = DoseHa * SupTrattata

        const UdM = element.getRawValue().UdM;

        if (element.controls['flagTipoDose'].value == enum_TipoMezzo.Ettolitro) {
            // DoseHl fissa
            const Acqua_Tot = this.qdcservice.QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
            const Dose_Hl = element.getRawValue().Dose_Hl;

            element.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Acqua_Tot, Dose_Hl,UdM), {emitEvent: false});
        } else {
            // DoseHa fissa
            const Dose_Ha = element.getRawValue().Dose_Ha
            const Sup_Trattata = this.getSupTrattata(new_sup, this.qdcservice.QdCForm);

            element.controls['DoseTot_Ha'].setValue(this.calculateQtaTotale(Dose_Ha, Sup_Trattata,UdM), {emitEvent: false});
        }

        this.qdcservice.RicaricaTutteGridProdottixImpianti();
    }

    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


    private calculateAcquaTot(Sup_Trattata: number, Acqua_Ha: number) {
        return this.funzionicomuniservice.roundNumber((Sup_Trattata * Acqua_Ha),this.qdcservice.get4DecimalNumericSettings().decimals);
    }

    private calculateAcquaHa(Acqua_Tot: number, Sup_Trattata: number): number {
        return this.funzionicomuniservice.roundNumber((Acqua_Tot / Sup_Trattata),this.qdcservice.get4DecimalNumericSettings().decimals)
    }

    private calculateQtaTotale(acquaTotale_DoseHa: number, doseHl_supTrattata: number, UdM: UnitaDiMisura) {
        /// se DoseHl fissa doseTotHa = AcquaTot * DoseHl
        /// se DoseHa fissa doseTotHa = DoseHa * SupTrattata
        return this.funzionicomuniservice.roundNumber(acquaTotale_DoseHa * doseHl_supTrattata, this.qdcservice.getProductNumericSettings(UdM).decimals);
    }

    private calculateDoseHa(qtaTotale: number, supTrattata: number, UdM: UnitaDiMisura) {
        return this.funzionicomuniservice.roundNumber(qtaTotale / supTrattata,this.qdcservice.getProductNumericSettings(UdM,true).decimals);
    }

    private calculateDoseHl(qtaTotale: number, acquaTotale: number, UdM: UnitaDiMisura) {
        return this.funzionicomuniservice.roundNumber(qtaTotale / acquaTotale,this.qdcservice.getProductNumericSettings(UdM).decimals);
    }


    // #######################################################################
    // ##################### Change Parametri in Griglia #####################
    // #######################################################################
    private SupTrattata_Ha_Prodotti_change(QdCForm: FormGroup,new_sup?: number) {

        const fa_SezioniProdotto = (QdCForm.controls['Trattamento'] as FormGroup).controls['Sezioni_Prodotto'] as FormArray;

        //Cambio i Dosaggi dei Prodotti che sono stati salvati

        (fa_SezioniProdotto as FormArray).controls.forEach((sezione_prodotto:FormGroup) => {
            (sezione_prodotto.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {

              if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() ||
                  (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && fc.controls['Riga_Salvata'].value === true)){

                  if (fc.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                      this.QtaTotDist_fissa(QdCForm, fc, new_sup);
                  } else {
                      if (fc.value['flagTipoDose'] == enum_TipoMezzo.Ettolitro) {
                          if(this.qdcservice.mostraDose_Hl(fc.value.Operazione)){
                              this.supTrattataHa_DoseHl_fissa(QdCForm, fc, new_sup);
                          }
                      } else {
                          this.supTrattataHa_DoseHa_fissa(QdCForm, fc, new_sup);
                      }
                  }

              }

            });
        });
    }

    private supTrattataHa_DoseHl_fissa(QdCForm: FormGroup, fc: FormGroup, new_sup?: number) {
        const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
        const Sup_Trattata = this.getSupTrattata(new_sup, QdCForm);
        const Dose_Hl = fc.value['Dose_Hl'];
        const UdM = fc.value['UdM'];

        const Qta_Totale_Distribuita = this.calculateQtaTotale(Acqua_Tot, Dose_Hl, UdM);

        fc.controls['DoseTot_Ha'].setValue(Qta_Totale_Distribuita);

        if (Sup_Trattata == 0) {
            fc.controls['Dose_Ha'].setValue(0);
        } else {
            fc.controls['Dose_Ha'].setValue(this.calculateDoseHa(Qta_Totale_Distribuita ,Sup_Trattata, UdM));
        }

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(fc.controls['DosiProdottiGridrowId'].getRawValue());
    }

    private supTrattataHa_DoseHa_fissa(QdCForm: FormGroup, fc: FormGroup, new_sup?: number) {
        const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
        const Sup_Trattata = this.getSupTrattata(new_sup, QdCForm);
        const Dose_Ha = fc.value['Dose_Ha'];
        const UdM = fc.value['UdM'];

        const Qta_Totale_Distribuita = this.calculateQtaTotale(Dose_Ha,Sup_Trattata,UdM);

        fc.controls['DoseTot_Ha'].setValue(Qta_Totale_Distribuita);

        if(this.qdcservice.mostraDose_Hl(fc.value['Operazione'])){

            if(Acqua_Tot == 0){
                fc.controls['Dose_Hl'].setValue(0);
            }else{
                const UdM = fc.value['UdM'];

                fc.controls['Dose_Hl'].setValue(this.calculateDoseHl(Qta_Totale_Distribuita, Acqua_Tot,UdM));
            }

        }

        this.qdcservice.RicaricaTutteGridProdottixImpianti();
    }

    private QtaTotDist_fissa(QdCForm: FormGroup, fc: FormGroup, new_sup?: number) {
        const Sup_Trattata = this.getSupTrattata(new_sup, QdCForm);
        const Qta_Totale_Distribuita = fc.value['DoseTot_Ha'];
        const UdM = fc.value['UdM'];

        fc.controls['Dose_Ha'].setValue(this.calculateDoseHa(Qta_Totale_Distribuita, Sup_Trattata,UdM));

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(fc.controls['DosiProdottiGridrowId'].getRawValue());

        if(this.qdcservice.mostraDose_Hl(fc.value['Operazione'])){
            const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];

            if(Acqua_Tot == 0){
                fc.controls['Dose_Hl'].setValue(0);
            }else{
                fc.controls['Dose_Hl'].setValue(this.calculateDoseHl(Qta_Totale_Distribuita, Acqua_Tot,UdM));
            }
        }
    }

    private Acqua_Prodotti_change(QdCForm: FormGroup) {

        //Cambio i Dosaggi dei Prodotti che sono stati salvati

        const Acqua_Tot = QdCForm.get('Trattamento.Acqua').value['Acqua_Tot'];
        const Sup_Trattata = this.getSupTrattata(undefined, QdCForm);
        const fa_SezioniProdotto = (QdCForm.controls['Trattamento'] as FormGroup).controls['Sezioni_Prodotto'] as FormArray;

        (fa_SezioniProdotto as FormArray).controls.forEach((sezione_prodotto:FormGroup) => {
            (sezione_prodotto.get('DosiProdotti') as FormArray).controls.forEach((fc:FormGroup) => {

                if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() ||
                    (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && fc.controls['Riga_Salvata'].value === true)){

                    const UdM = fc.value['UdM'];

                    if (fc.controls['flagDoseQuantitaTotale'].value == enum_doseQuantitaTotale.Qta_Totale) {
                        this.QtaTotDist_fissa(QdCForm, fc);
                    } else {

                        if (fc.value['flagTipoDose'] == enum_TipoMezzo.Ettolitro) {

                            if (this.qdcservice.mostraDose_Hl(fc.value['Operazione'])) {
                                const Dose_Hl = fc.value['Dose_Hl'];
                                const Qta_Totale_Distribuita = this.calculateQtaTotale(Acqua_Tot, Dose_Hl,UdM);

                                fc.controls['DoseTot_Ha'].setValue(Qta_Totale_Distribuita);

                                if (Sup_Trattata == 0) {
                                    fc.controls['Dose_Ha'].setValue(0);
                                } else {
                                    fc.controls['Dose_Ha'].setValue(this.calculateDoseHa(Qta_Totale_Distribuita, Sup_Trattata,UdM));
                                }

                                this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(fc.controls['DosiProdottiGridrowId'].value);
                            }

                        } else {

                            const Dose_Ha = fc.value['Dose_Ha'];
                            const Qta_Totale_Distribuita = this.calculateQtaTotale(Dose_Ha, Sup_Trattata, UdM);

                            fc.controls['DoseTot_Ha'].setValue(Qta_Totale_Distribuita);

                            if(this.qdcservice.mostraDose_Hl(fc.value['Operazione'])){

                                if(Acqua_Tot == 0){
                                    fc.controls['Dose_Hl'].setValue(0);
                                }else{
                                    fc.controls['Dose_Hl'].setValue(this.calculateDoseHl(Qta_Totale_Distribuita, Acqua_Tot,UdM));
                                }
                            }

                        }
                    }

                }

            });
        });
    }

    private getSupTrattata(new_sup: number, QdCForm: FormGroup<any>) {
        return new_sup != undefined
            ? new_sup
            : this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare
                ? QdCForm.get('Trattamento.Quantita').value['Qta_Trattata']
                : QdCForm.get('Trattamento.Superfici').value['Sup_Trattata'];
    }

    public aggiornaUdmDoseIrrigazione(udmDose: CodiceDescrizione){
        if (this.qdcservice.GridImpiantiHttpService) {
            const impianti = this.qdcservice.GridImpiantiPublicService?.getValue()?.data?.rows;

            if (impianti && impianti.length > 0) {
                let grid =this.qdcservice.GridImpiantiPublicService.getValue();
                let gridRows = grid.data.rows;

                impianti.forEach((impianto: any) => {
                    if (impianto.UdmDose !== udmDose.descrizione){
                        if (impianto.UdmDose === SIMBOLO_M3_HA && udmDose.descrizione === SIMBOLO_MM){
                            impianto.UdmDose = SIMBOLO_MM;
                            impianto.DoseAcquaGiornaliera = impianto.DoseAcquaGiornaliera / 10;
                        } else if (impianto.UdmDose === SIMBOLO_MM && udmDose.descrizione === SIMBOLO_M3_HA){
                            impianto.UdmDose = SIMBOLO_M3_HA;
                            impianto.DoseAcquaGiornaliera = impianto.DoseAcquaGiornaliera * 10;
                        }
                    }

                    if (gridRows){
                        const rowIndex = gridRows.findIndex((row: any) =>
                            row.PIVA === impianto.PIVA &&
                            row.SA_COD === impianto.SA_COD &&
                            row.APPEZZA === impianto.APPEZZA &&
                            row.ID_REG === impianto.ID_REG &&
                            row.Progetto_Cod === impianto.Progetto_Cod
                        );

                        if (rowIndex !== -1) {
                            gridRows[rowIndex].DoseAcquaGiornaliera = impianto.DoseAcquaGiornaliera;
                        }
                    }
                });

                grid.data.rows = gridRows;
                this.qdcservice.GridImpiantiPublicService.refresh(false, grid.data);
                this.qdcservice.AggiornaFormArrayImpiantiSelezionati();
            }
        }
    }

    public updateGridIrrigationData(QdCForm: FormGroup, changedField?: string): void {

        let operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();
        const fertirrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
        const irrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;

        if(!fertirrigazione && !irrigazione){
            return;
        }

        let portataChanged = false;
        let oreIrrigazioneChanged = false;
        let otherFieldChanged = false;
        if (changedField){
            if (changedField === 'Portata'){
                portataChanged = true;
            } else if (changedField === 'OreIrrigazione'){
                oreIrrigazioneChanged = true;
            } else {
                otherFieldChanged = true;
            }
        } else {
            otherFieldChanged = true;
        }

        // Check if grid service is available and update irrigation data
        if (this.qdcservice.GridImpiantiHttpService) {
            const acquaData = QdCForm.get('Trattamento.Acqua')?.value;
            const impianti = this.qdcservice.GridImpiantiPublicService?.getValue()?.data?.rows;
            
            let acquaHa: number = 0;
            if (fertirrigazione && otherFieldChanged && !acquaData){ //per le fertirrigazioni abbiamo bisogno dell'acqua nel footer
                return;
            } else if (fertirrigazione && otherFieldChanged && acquaData){
                acquaHa = acquaData.Acqua_Ha;
            }

            if (impianti && impianti.length > 0) {
                let grid =this.qdcservice.GridImpiantiPublicService.getValue();
                let gridRows = grid.data.rows;

                impianti.forEach((impianto: any) => {

                    if (otherFieldChanged){
                        if (fertirrigazione){
                            this.irrigazioneAcquaService.calculateFertirrigationWaterValues(impianto, acquaHa, impianto.Sup_Imp_help, impianto.DataInizioIrrigazione, impianto.DataFineIrrigazione, impianto.Efficienza);
                        } else if (irrigazione) {
                            this.irrigazioneAcquaService.calculateIrrigationWaterValues(impianto, impianto.DoseAcquaGiornaliera, impianto.Sup_Imp_help, impianto.DataInizioIrrigazione, impianto.DataFineIrrigazione, impianto.Efficienza);
                        }
                    } else if (portataChanged){
                        if (fertirrigazione){
                            this.irrigazioneAcquaService.calcolaOreFertirrigazione(impianto);
                        } else if (irrigazione) {
                            this.irrigazioneAcquaService.gestisciCambioPortataIrrigazione(impianto, impianto.Sup_Imp_help, impianto.DataInizioIrrigazione, impianto.DataFineIrrigazione, impianto.Efficienza, impianto.Portata);
                        }
                    } else if (oreIrrigazioneChanged){
                        if (fertirrigazione){
                            this.irrigazioneAcquaService.calcolaPortataFertirrigazione(impianto);
                        } else if (irrigazione) {
                            this.irrigazioneAcquaService.gestisciCambioOreIrrigazione(impianto, impianto.Sup_Imp_help, impianto.DataInizioIrrigazione, impianto.DataFineIrrigazione, impianto.Efficienza, impianto.OreIrrigazione);
                        }
                    }

                    // Find and update the corresponding row in the grid data
                    if (gridRows) {
                        const rowIndex = gridRows.findIndex((row: any) =>
                            row.PIVA === impianto.PIVA &&
                            row.SA_COD === impianto.SA_COD &&
                            row.APPEZZA === impianto.APPEZZA &&
                            row.ID_REG === impianto.ID_REG &&
                            row.Progetto_Cod === impianto.Progetto_Cod
                        );

                        if (rowIndex !== -1) {

                            gridRows[rowIndex].DoseAcquaGiornaliera = impianto.DoseAcquaGiornaliera;
                            gridRows[rowIndex].QtaTotaleAcquaUtilizzataPeriodo = impianto.QtaTotaleAcquaUtilizzataPeriodo;
                            gridRows[rowIndex].QtaTotaleAcquaAssorbitaPeriodo = impianto.QtaTotaleAcquaAssorbitaPeriodo;
                            gridRows[rowIndex].QtaTotaleAcquaGiornaliera = impianto.QtaTotaleAcquaGiornaliera;
                            gridRows[rowIndex].QtaTotaleAcquaAssorbitaGiornaliera = impianto.QtaTotaleAcquaAssorbitaGiornaliera;
                            gridRows[rowIndex].OreIrrigazione = impianto.OreIrrigazione;
                            gridRows[rowIndex].Portata = impianto.Portata;
                        }
                    }
                });

                grid.data.rows = gridRows;
                // Force multiple refresh strategies
                this.qdcservice.GridImpiantiPublicService.refresh(false, grid.data);

                this.qdcservice.AggiornaFormArrayImpiantiSelezionati();
            }
        }
    }
}
