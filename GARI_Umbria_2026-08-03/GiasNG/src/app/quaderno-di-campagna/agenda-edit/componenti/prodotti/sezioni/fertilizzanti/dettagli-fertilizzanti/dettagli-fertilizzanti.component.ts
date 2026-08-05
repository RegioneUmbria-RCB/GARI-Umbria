import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { QdCService } from "../../../../../service/qdc.service";
import { QdCProdottiService } from "../../../../../service/prodotti.service";
import { QdCDettagliFertilizzantiService } from "../../../../../service/prodotti/dettagli-fertilizzanti.service";
import { QdCUnitadiMisuraService } from "../../../../../service/unita-di-misura.service";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GridDosiProdottiControlliService } from "../../../../../service/grid-dosi-prodotti/grid-dosi-prodotti-controlli.service";
import { pairwise, startWith, Subscription } from "rxjs";
import { Lavorazione } from "../../../../../../../Model/attivita/Lavorazione";
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { TranslocoService } from "@jsverse/transloco";
import { MultiColumnComboboxFertilizzazione } from "../../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { QdCFertilizzantiService } from "../../../../../service/prodotti/fertilizzanti.service";
import { Epoca } from "../../../../../../../Model/metaschema/Epoca";
import { Disciplinare } from "../../../../../../../Model/metaschema/Disciplinari";

@Component({
    standalone: false,
    selector: 'app-dettagli-fertilizzanti',
    templateUrl: './dettagli-fertilizzanti.component.html',
    styleUrls: ['./dettagli-fertilizzanti.component.scss'],
    providers: [QdCProdottiService, QdCDettagliFertilizzantiService, QdCUnitadiMisuraService, GiasDropDownTemplateService, GridDosiProdottiControlliService]
})
export class DettagliFertilizzantiComponent implements OnInit, OnDestroy {

    Subs: Subscription = new Subscription();

    Operazione: Lavorazione;

    FertilizzantiForm: FormGroup;

    objParametriAgenda: ObjParametriAgenda;

    constructor(public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        public qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
        public qdcfertilizzantiservice: QdCFertilizzantiService,
        public qdcservice: QdCService,
        private translocoservice: TranslocoService,
        @Inject(LOCALE_ID) public locale_id: string) { }

    async ngOnInit() {
        this.FertilizzantiForm = <FormGroup>this.parent.form;

        this.Operazione = this.FertilizzantiForm.get("Operazione").value;

        this.prodottiservice.ObsProdottiForm.next(this.FertilizzantiForm);

        //Triggero il validator del formgroup
        this.FertilizzantiForm.markAllAsTouched();

        this.prodottiservice.GestioneMagazzino_Abilitata();

        this.prodottiservice.GestioneGiacenze();

        this.prodottiservice.GestioneLotti();

        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()) {
            //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
            this.qdcdettaglifertilizzantiservice.setArrayDDLSezioneProdottiFertilizzantiForm(this.FertilizzantiForm.getRawValue());

            //Abilita/Disabilita i controlli dei Fertilizzanti
            this.qdcservice.Gestisci_Controlli_Fertilizzanti(this.FertilizzantiForm, + this.FertilizzantiForm.get("Operazione").value.primaryKey.codice);
        }

        this.Subs.add(
            this.qdcservice.TestataForm.get("Data").valueChanges.subscribe(d => {

                this.qdcdettaglifertilizzantiservice.clearSezioneProdottiFertilizzantiForm(false, true, false);

            })
        );

        this.Subs.add(
            this.qdcservice.TestataForm.get("Disciplinare").valueChanges.subscribe(async d => {

                //Commentato per richiesta del CAI di non pulire i valori impostati quando cambia il disciplinare
                // (che può variare in base agli impianti selezionati)
                //this.qdcdettaglifertilizzantiservice.clearSezioneProdottiFertilizzantiForm(false,true,true);

            })
        );

        this.Subs.add(
            this.qdcservice.TestataForm.get("Centro_Aziendale").valueChanges.subscribe(d => {

                this.qdcdettaglifertilizzantiservice.clearSezioneProdottiFertilizzantiForm(false, true, false);

            })
        );

        //Richiamo la funzione di change solo se la Direttiva Nitrati ha un valore diverso dal precedente
        this.Subs.add(this.FertilizzantiForm.get("Direttiva_Nitrati").valueChanges.pipe(
            startWith(this.FertilizzantiForm.get("Direttiva_Nitrati").value),
            pairwise()).subscribe(([prev, next]: [Disciplinare, Disciplinare]) => {

                if (next?.regolamentoConcimazione?.codice !== prev?.regolamentoConcimazione?.codice) {
                    this.qdcdettaglifertilizzantiservice.change_Direttiva_Nitrati(next);
                }
            })
        );

        //Richiamo la funzione di change solo se Utilizza_Direttiva_Nitrati ha un valore diverso dal precedente
        this.Subs.add(this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").valueChanges.pipe(
            startWith(this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").value),
            pairwise()).subscribe(async ([prev, next]: [{ [key: string]: boolean }, { [key: string]: boolean }]) => {

                if (next !== prev) {
                    await this.qdcdettaglifertilizzantiservice.change_Utilizza_Direttiva_Nitrati(next);
                }
            })
        );

        //Richiamo la funzione di change solo se EpocaFertilizzazione ha un valore diverso dal precedente
        this.Subs.add(this.FertilizzantiForm.get("EpocaFertilizzazione").valueChanges.pipe(
            startWith(this.FertilizzantiForm.get("EpocaFertilizzazione").value),
            pairwise()).subscribe(([prev, next]: [Epoca, Epoca]) => {

                if (next?.codice !== prev?.codice) {
                    this.qdcdettaglifertilizzantiservice.Gestisci_Efficienza_N_Utile(this.FertilizzantiForm, next).then();
                }
            })
        );

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(this.FertilizzantiForm.get("DosiProdottiGridrowId").value);

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    getNome_N_P_K_Cu(formName: string) {

        let descrizione = "";

        switch (formName) {
            case 'N':
                descrizione = this.translocoservice.translate("qdc.lbl_NResource1.Text");
                break;
            case 'P':
                descrizione = this.translocoservice.translate("qdc.lbl_P2O5Resource1.Text");
                break;
            case 'K':
                descrizione = this.translocoservice.translate("qdc.lblK2OResource1.Text");
                break;
            case 'Cu':
                descrizione = this.translocoservice.translate("qdc.lblCuResource1.Text");
                break;
        }

        //Concateno all'N_P_K_Cu la descrizione
        if (this.Mostra_N_P_K_Cu_calcolato(formName)) {
            descrizione += " (*) " + this.translocoservice.translate("qdc.MediaPesataValoriCarico");
        } else {

            let Fertilizzante_Selezionato: MultiColumnComboboxFertilizzazione = this.FertilizzantiForm.get("Prodotto").getRawValue();

            if (Fertilizzante_Selezionato?.prodotto?.codice > 0 &&
                Fertilizzante_Selezionato?.effluente?.carico > 0 &&
                this.qdcdettaglifertilizzantiservice.Mostra_Dichiarato_nel_PUA() && formName === "N") {

                descrizione += " " + this.translocoservice.translate("qdc.Dichiarato_nel_PUA");
            }


        }


        return descrizione;
    }

    Mostra_N_P_K_Cu_calcolato(formName: string) {

        let mostra = false;

        let Fertilizzante_Selezionato: MultiColumnComboboxFertilizzazione = this.FertilizzantiForm.get("Prodotto").value;

        if (Fertilizzante_Selezionato) {

            switch (formName) {
                case 'N':
                    if (Fertilizzante_Selezionato.N_Ponderato === true) {
                        mostra = true;
                    }
                    break;
                case 'P':
                    if (Fertilizzante_Selezionato.P_Ponderato === true) {
                        mostra = true;
                    }
                    break;
                case 'K':
                    if (Fertilizzante_Selezionato.K_Ponderato === true) {
                        mostra = true;
                    }
                    break;
                case 'Cu':
                    if (Fertilizzante_Selezionato.Cu_Ponderato === true) {
                        mostra = true;
                    }
                    break;
            }
        }

        return mostra;
    }


    mostraLbl_Dose_Consigliata() {

        let mostra = false;

        //Da mostrare solamente se la riga non è salvata
        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FertilizzantiForm.get("Riga_Salvata").value)
            return mostra;

        if (this.qdcdettaglifertilizzantiservice.Qta_Ha_Distribuibile_Prodotto !== 1000000) {
            mostra = true;
        }

        return mostra;
    }

    public InserisciFertilizzante() {
        this.prodottiservice.InserisciDoseProdotto.next(true);
    }

    mostraLbl_Dose_Consigliata_Ricetta() {
        let mostra = false;

        //Da mostrare solamente se la riga non è salvata
        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.FertilizzantiForm.get("Riga_Salvata").value)
            return mostra;

        if (this.qdcdettaglifertilizzantiservice.Qta_Ha_Distribuibile_Prodotto_Ricette !== 1000000) {
            mostra = true;
        }


        return mostra;
    }


    changePercentuale_N(value: number) {

        if (this.FertilizzantiForm.get("N_Percentuale_X_Prodotto").valid) {
            this.qdcdettaglifertilizzantiservice.Imposta_Dose_Ha_In_Base_Alla_Percentuale_N();
        }

    }


}
