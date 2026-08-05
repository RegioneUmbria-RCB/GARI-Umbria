import { Component, OnInit, Optional} from '@angular/core';
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {QdCProdottiService} from "../../../service/prodotti.service";
import {QdCDettagliFertilizzantiService} from "../../../service/prodotti/dettagli-fertilizzanti.service";
import {QdCService} from "../../../service/qdc.service";
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../../Service/master.service";
import {TranslocoService} from "@jsverse/transloco";
import {QdCDettagliFormulatiService} from "../../../service/prodotti/dettagli-formulati.service";
import {QdCDettagliSementiService} from "../../../service/prodotti/dettagli-sementi.service";
import {FERTILIZZANTI, FORMULATI, SEMENTI , INSETTI} from "../../../../../Model/CostantiPersonalizzate";
import {UtilityFunctions} from "../../../../../Utility/UtilityFunctions";
import {MultiColumnComboboxTrattamento} from "../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";

@Component({
    standalone: false,
    selector: 'app-bottone-ricerca-prodotti',
    templateUrl: './bottone-ricerca-prodotti.html',
    styleUrls: ['./bottone-ricerca-prodotti.css']
})
export class BottoneRicercaProdotti implements OnInit {

    ProdottiForm: FormGroup;

    constructor(
        private parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        private qdcservice: QdCService,
        @Optional() private qdcdettagliformulatiservice: QdCDettagliFormulatiService,
        @Optional() private qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
        @Optional() private qdcdettaglisementiservice: QdCDettagliSementiService,
        private translocoService: TranslocoService
    ) {  }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup> this.parent.form;
    }


    DisabilitaBtnRicercaProdotti(){
        let disabilita = false;

        if((!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.ProdottiForm.get("Riga_Salvata").value) ||
             this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA())
            disabilita = true;

        return disabilita;
    }

    public Ricerca(){
        let elem_cod = this.ProdottiForm.get("Categoria_Magazzino").value;

        switch(elem_cod){
            case FORMULATI:
                this.qdcdettagliformulatiservice.RicercaFormulati(true,true);
                break;
            case FERTILIZZANTI:
                this.qdcdettaglifertilizzantiservice.RicercaFertilizzanti(true,true);
                break;
            case SEMENTI:
                this.qdcdettaglisementiservice.RicercaSementi();
                break;
            case INSETTI:
                this.qdcdettagliformulatiservice.RicercaInsetti();
                break;
        }
    }

}
