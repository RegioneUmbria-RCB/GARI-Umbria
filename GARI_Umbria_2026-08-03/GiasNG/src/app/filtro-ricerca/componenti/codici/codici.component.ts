import {Component, Input, OnInit} from "@angular/core";
import {FormGroup} from "@angular/forms";
import {TranslocoService} from "@jsverse/transloco";
import { Enum_Entita_FiltroRicerca } from "app/Model/TipiEnumerativi";
import {
  AnagrafeClient
} from "app/Service/net-core6-api.service";

@Component({
    standalone: false,
    selector: 'app-codici',
    templateUrl: './codici.component.html',
    styleUrls: ['./codici.component.scss']
})
export class CodiciComponent implements OnInit {

    @Input() formGroupInput: FormGroup;
    @Input() tipoCodici: number;
    @Input() allCodici: any[];

    filterText: string = "";

    // ListaSelezionabili: any[];
    // ListaScelti: any[];

    constructor(
                private transloco: TranslocoService,
                private codiciService: AnagrafeClient,
    ) {}

    ngOnInit(): void {
      let codiciScelti = this.formGroupInput.getRawValue();

      if (codiciScelti.length > 0)
          codiciScelti.forEach(item => {
            let index = this.allCodici.findIndex(codItem => codItem.codice == item.codice && codItem.tipoCodice == item.tipoCodice);
            if (!item.master && index > -1) {
              this.allCodici[index].checked = false;
              this.changeFavorite(this.allCodici[index]);
            }
          });
      else
          this.allCodici.forEach(item => item.checked = false);
    }

    changeFavorite(preferito): void {

        //Se è true il check lo faccio diventare false e viceversa
        if(preferito.checked){
          preferito.checked = false;
        }else{
          preferito.checked = true;
        }

        if(preferito.master){

          this.allCodici.forEach(c=>{
            if(c.tipoCodice === preferito.tipoCodice){
              c.checked = preferito.checked;
            }
          });
        }else{

          if(this.ContainsMaster()){

            if(!preferito.checked){
              //Se non ho più nessun codice selezionato nascondo anche la voce 'master' tra i codici scelti
              let count_all_codici_no_master = this.allCodici.filter(c=>c.tipoCodice === preferito.tipoCodice && !c.master).length;
              let count_codici_same_checked = this.allCodici.filter(c=>c.tipoCodice === preferito.tipoCodice && c.checked === preferito.checked && !c.master).length;

              let difference = count_all_codici_no_master - count_codici_same_checked;

              if(difference === 0){
                this.allCodici[this.allCodici.findIndex(c=>c.tipoCodice === preferito.tipoCodice && c.master)].checked = preferito.checked;
              }
            }else{
              //Se ho selezionato almeno un codice seleziono anche il master
              this.allCodici[this.allCodici.findIndex(c=>c.tipoCodice === preferito.tipoCodice && c.master)].checked = preferito.checked;
            }
          }

        }
        this.formGroupInput.patchValue(this.allCodici.filter(item => item.checked));
    }

    SelectAll(){
        for(let c of this.allCodici){
          c.checked = true;
        }

       this.formGroupInput.patchValue(this.allCodici.filter(item => item.checked));
    }

    DeselectAll(){
      for(let c of this.allCodici){
        c.checked = false;
      }

      this.formGroupInput.patchValue(this.allCodici.filter(item => item.checked));
    }

    ContainsMaster(){
      return this.allCodici.findIndex(c=>c.master) > -1;
    }

    GetTitleCodici(){
      return this.transloco.translate("SelezionaDati");
    }

    GetTitleSelectedCodici(){
      return this.transloco.translate("DatiScelti");
    }

    ShowMasterRow(codice){
      return this.allCodici.filter(c=>!c.master && c.checked === false && c.tipoCodice === codice.tipoCodice).length > 0 && codice.master;
    }

    // openCodiciDDL(ddlEl: GiasDropDownTemplateSComponent) {
    //     ddlEl.loading = true;

    //     if (!ddlEl.listItems) {
    //         this.codiciService.anagrafeGetCodiciAnagrafe(this.tipoCodici).subscribe(r => {
    //             ddlEl.listItems = JSON.parse(r.RispostaStringa);
    //             // ddlEl.listItems.forEach(item => item.Descrizione = this.transloco.translate(item.Descrizione));
    //             ddlEl.loading = false;
    //         });
    //     } else
    //         ddlEl.loading = false;
    // }

    // ListeCodici = [
    //     {value: '1', text: 'test 1'},
    //     {value: '2', text: 'test 2'},
    //     {value: '3', text: 'test 3'},
    //     {value: '4', text: 'test 4'},
    //     {value: '5', text: 'test 5'},
    // ];

}
