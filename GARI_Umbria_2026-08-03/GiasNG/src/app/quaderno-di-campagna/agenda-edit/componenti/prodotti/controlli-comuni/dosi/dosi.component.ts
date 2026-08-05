import {Component, OnInit, Optional} from "@angular/core";
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import {INSETTI, SEMENTI} from 'app/Model/CostantiPersonalizzate';
import { QdCProdottiService } from "app/quaderno-di-campagna/agenda-edit/service/prodotti.service";
import { QdCService } from "app/quaderno-di-campagna/agenda-edit/service/qdc.service";
import {MisceleService} from "../../../../service/miscele.service";
import {QdCDettagliSementiService} from "../../../../service/prodotti/dettagli-sementi.service";
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";

@Component({
    standalone: false,
    selector: "app-dosi",
    templateUrl: "./dosi.component.html",
    styleUrls: ["./dosi.component.css"],
})
export class DosiComponent implements OnInit {

    Lav_Cod: number;

    Categoria_Magazzino: number;

    ProdottiForm: FormGroup;

    constructor(
        public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        public qdcservice: QdCService,
        private translocoservice: TranslocoService,
        private misceleservice: MisceleService,
        @Optional() private qdcdettaglisementiservice: QdCDettagliSementiService
    ) {  }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup>this.parent.form;
        this.Categoria_Magazzino = this.ProdottiForm.get("Categoria_Magazzino").value;
        this.Lav_Cod = + (this.ProdottiForm.get("Operazione").value as Lavorazione).primaryKey.codice;
    }

    changeDose_Ha(value){
        let Sup_Calcolata_Per_Frazionamento = 0;

        if(this.Categoria_Magazzino === SEMENTI && this.qdcdettaglisementiservice?.Mostra_Sup_Calcolata()){
            Sup_Calcolata_Per_Frazionamento = this.ProdottiForm.get("Sup_Calcolata").value;
        }

        this.misceleservice.calcoloMiscele('changeDose_Ha',this.ProdottiForm,Sup_Calcolata_Per_Frazionamento);
    }

    changeDose_Hl(value){
        this.misceleservice.calcoloMiscele('changeDose_Hl',this.ProdottiForm,0);
    }

    changeDoseTot_Ha(value){

        let Sup_Calcolata_Per_Frazionamento = 0;

        if(this.Categoria_Magazzino === SEMENTI && this.qdcdettaglisementiservice?.Mostra_Sup_Calcolata()){
            Sup_Calcolata_Per_Frazionamento = this.ProdottiForm.get("Sup_Calcolata").value;
        }

        this.misceleservice.calcoloMiscele('changeDoseTot_Ha',this.ProdottiForm,Sup_Calcolata_Per_Frazionamento);
    }

    getDescrizioneDoseTot_Ha(): string{
        let descrizione = "";

        if(this.Categoria_Magazzino === SEMENTI){
            descrizione = this.translocoservice.translate("qdc.rblAcqua_TotResource1.Text");
        } else if(this.Categoria_Magazzino === INSETTI) {
            descrizione = this.translocoservice.translate('qdc.rblAcqua_TotResource1.Text');
        } else {
            descrizione = this.translocoservice.translate("qdc.rbl_QtaTotResource1.Text");
        }

        return descrizione;
    }

    public getDecimals(formControlName: string): number {

        let default_Udm = false;

        if(formControlName === "Dose_Ha")
          default_Udm = true;

        if(this.Categoria_Magazzino === INSETTI)
            return 0;
        else
            return this.qdcservice.getProductNumericSettings(this.ProdottiForm.get('UdM').value,default_Udm).decimals;
    }

    public getFormat(formControlName: string): string {
        let default_Udm = false;

        if(formControlName === "Dose_Ha")
          default_Udm = true;

        if(this.Categoria_Magazzino === INSETTI)
            return 'n0';
        else
            return this.qdcservice.getProductNumericSettings(this.ProdottiForm.get('UdM').value,default_Udm).format;
    }
}
