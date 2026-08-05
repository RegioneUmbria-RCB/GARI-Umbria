import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { enum_LAVCOD, enum_PUARegolamenti_Tipo } from 'app/Model/TipiEnumerativi';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { skip, Subscription } from 'rxjs';
import { QdCFertilizzantiService } from "../../../../service/prodotti/fertilizzanti.service";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { UtilityFunctions } from "../../../../../../Utility/UtilityFunctions";
import { Tipo_Ricetta } from "../../../../../../Model/attivita/Attivita";

@Component({
    standalone: false,
    selector: 'app-fertilizzanti',
    templateUrl: './fertilizzanti.component.html',
    styleUrls: ['./fertilizzanti.component.scss'],
    providers: [GiasDropDownTemplateService, QdCFertilizzantiService]
})
export class FertilizzantiComponent implements OnInit, OnDestroy {

    Subs: Subscription = new Subscription();

    Operazione: Lavorazione;

    FertilizzantiForm: FormGroup;

    objParametriAgenda: ObjParametriAgenda;

    constructor(public parent: FormGroupDirective,
        public qdcservice: QdCService,
        public qdcfertilizzantiservice: QdCFertilizzantiService,
        private ddlService: GiasDropDownTemplateService
    ) { }

    async ngOnInit() {
        this.FertilizzantiForm = <FormGroup>this.parent.form;

        this.Operazione = this.FertilizzantiForm.get("Operazione").value;

        await this.qdcfertilizzantiservice.getArray_Direttiva_Nitrati();

        await this.qdcfertilizzantiservice.getArray_EpocheFertilizzazione(this.FertilizzantiForm);

        this.Subs.add(
            this.qdcservice.TestataForm.get("Data").valueChanges.subscribe(d => {

                //Al cambio della data ricarico la Direttiva Nitrati
                if (+ this.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)
                    this.qdcfertilizzantiservice.getArray_Direttiva_Nitrati().then();

            })
        );

        this.Subs.add(
            this.qdcservice.TestataForm.get("Disciplinare").valueChanges.subscribe(async d => {

                await this.qdcfertilizzantiservice.getArray_EpocheFertilizzazione(this.FertilizzantiForm);

            })
        );

        this.Subs.add(
            this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem => {
                switch (ddlElem.FormControlName) {
                    case 'Direttiva_Nitrati':
                        break;
                    case 'EpocaFertilizzazione':
                        break;
                }
            })
        );

        //Aggiorno il valore di Utilizza_Direttiva_Nitrati in tutti i DosiProdotti
        this.Subs.add(this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").valueChanges.subscribe(value => {

            if (value !== null && value !== undefined) {
                let DosiProdotti = this.qdcservice.DosiProdottiFormArray(null, this.FertilizzantiForm.get("Operazione").value);

                if (DosiProdotti && DosiProdotti.controls.length > 0) {
                    for (let i = 0; i < DosiProdotti.controls.length; i++) {
                        DosiProdotti.controls[i].patchValue({
                            Utilizza_Direttiva_Nitrati: value
                        });
                    }
                }
            }
        }));

        //Aggiorno il valore di Direttiva_Nitrati in tutti i DosiProdotti
        this.Subs.add(this.FertilizzantiForm.get("Direttiva_Nitrati").valueChanges.subscribe(value => {

            let DosiProdotti = this.qdcservice.DosiProdottiFormArray(null, this.FertilizzantiForm.get("Operazione").value);

            if (DosiProdotti && DosiProdotti.controls.length > 0) {
                for (let i = 0; i < DosiProdotti.controls.length; i++) {
                    DosiProdotti.controls[i].patchValue({
                        Direttiva_Nitrati: value
                    });
                }
            }
        }));

        //Aggiorno il valore di EpocaFertilizzazione in tutti i DosiProdotti
        this.Subs.add(this.FertilizzantiForm.get("EpocaFertilizzazione").valueChanges.subscribe(value => {

            let DosiProdotti = this.qdcservice.DosiProdottiFormArray(null, this.FertilizzantiForm.get("Operazione").value);

            if (DosiProdotti && DosiProdotti.controls.length > 0) {
                for (let i = 0; i < DosiProdotti.controls.length; i++) {
                    DosiProdotti.controls[i].patchValue({
                        EpocaFertilizzazione: value
                    });
                }
            }
        }));

        //Disabilito la Direttiva Nitrati e obbligo l'utente ad utilizzare la Direttiva che mi è arrivata dal PUA
        if (this.qdcservice.TestataForm.get("Operazioni").getRawValue().findIndex((o: Lavorazione) => + o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI) > -1 &&
            this.qdcservice.get_Tipo_Ricetta() === Tipo_Ricetta.PianoDistribuzionePua) {

            this.FertilizzantiForm.get("Utilizza_Direttiva_Nitrati").disable({ emitEvent: false });

            this.FertilizzantiForm.get("Direttiva_Nitrati").disable({ emitEvent: false });
        }

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Carico la ddl solo quando scatta l'evento di open
    async openControlFertilizzanti(
        ddlEl: GiasDropDownTemplateSComponent,
        formName: string
    ) {
        let fn: any;

        switch (formName) {
            case "EpocaFertilizzazione":
                fn = async () => { await this.qdcfertilizzantiservice.getArray_EpocheFertilizzazione(this.FertilizzantiForm) };
                UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
                break;
            case "Direttiva_Nitrati":
                break;
        }
    }

    get_Descrizione_PercentualeN_Utilizzabile(): string {

        let descrizione = "";

        let percentuale_N = 0;

        let riga = this.qdcservice.get_Riga_Impianti_Con_N_Residuo_Minore();

        if (riga && riga.N_Residuo_Percentuale > 0) {
            percentuale_N = riga.N_Residuo_Percentuale;
        }

        let str_percentuale_n_utilizzabile = this.qdcservice.decimalpipe.transform(percentuale_N, this.qdcservice.digitsInfo_QdC_Percentuale, this.qdcservice.locale_id) + "%";

        descrizione = this.qdcservice.translocoService.translate("qdc.PercentualeNUtilizzabile", { Percentuale: str_percentuale_n_utilizzabile });

        return descrizione;
    }

    get_Descrizione_PercentualeN_Utilizzata() {

        let descrizione = "";

        let percentuale_N = this.qdcservice.getPercentualeN_Utilizzata_Operazione(this.FertilizzantiForm.get('Operazione').getRawValue());

        let str_percentuale_n_utilizzata = this.qdcservice.decimalpipe.transform(percentuale_N, this.qdcservice.digitsInfo_QdC_Percentuale, this.qdcservice.locale_id) + "%";

        descrizione = this.qdcservice.translocoService.translate("qdc.PercentualeNUtilizzataDuePunti", { Percentuale: str_percentuale_n_utilizzata });

        return descrizione;
    }

    public GetClassForDivFertilizzanti(formName: string): string {
        let DivClass: string = "";

        if (formName !== "") {
            if (this.qdcfertilizzantiservice.mostraDivDirettivaNitrati(this.FertilizzantiForm)) {
                switch (formName) {
                    case "EpocaFertilizzazione":
                        DivClass = "epoca-container-with-nitrati";
                        break;
                    case "Modalita_Applicazione":
                        DivClass = "modalita-applicazione-container-with-nitrati";
                        break;
                }
            } else {
                switch (formName) {
                    case "EpocaFertilizzazione":
                        DivClass = "epoca-container-without-nitrati";
                        break;
                    case "Modalita_Applicazione":
                        DivClass = "modalita-applicazione-container-without-nitrati";
                        break;
                }
            }
        }

        return DivClass;
    }

}
