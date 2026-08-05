import {Component, OnDestroy, OnInit, Optional} from '@angular/core';
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {QdCService} from "../../../../service/qdc.service";
import {Subscription} from "rxjs";
import {FERTILIZZANTI, FORMULATI, INSETTI} from "../../../../../../Model/CostantiPersonalizzate";
import {QdCDettagliFormulatiService} from "../../../../service/prodotti/dettagli-formulati.service";
import {QdCDettagliFertilizzantiService} from "../../../../service/prodotti/dettagli-fertilizzanti.service";
import {enum_LAVCOD} from "../../../../../../Model/TipiEnumerativi";
import {Job} from "../../../../../../Model/attivita/Job";
import {Lavorazione} from "../../../../../../Model/attivita/Lavorazione";

@Component({
  standalone: false,
  selector: 'app-flag-magazzini-esterni',
  templateUrl: './flag-magazzini-esterni.component.html',
  styleUrls: ['./flag-magazzini-esterni.component.css']
})
export class FlagMagazziniEsterniComponent implements OnInit,OnDestroy {

  ProdottiForm: FormGroup;

  Subs = new Subscription();

  constructor(public parent: FormGroupDirective,
              public qdcservice: QdCService,
              @Optional() public dettagliformulatiservice: QdCDettagliFormulatiService,
              @Optional() public dettaglifertilizzantiservice: QdCDettagliFertilizzantiService) { }

  ngOnInit(): void {

    this.ProdottiForm = <FormGroup>this.parent.form;

    this.Subs.add(this.ProdottiForm.get("Visualizza_Magazzini_Esterni").valueChanges.subscribe(p=>{
      this.changeFlagMagazziniEsterni()
    }));
  }

  changeFlagMagazziniEsterni(){
    if(this.ProdottiForm.get("Visualizza_Magazzini_Esterni").value){

      this.ProdottiForm.patchValue({
        Visualizza_Magazzini_Agenzie: false
      });

      this.ProdottiForm.get("Visualizza_Magazzini_Agenzie").disable({emitEvent: false});
    }else{
      this.ProdottiForm.get("Visualizza_Magazzini_Agenzie").enable({emitEvent: false});
    }

    switch(this.ProdottiForm.get("Categoria_Magazzino").getRawValue()){
      case FERTILIZZANTI:
        this.dettaglifertilizzantiservice.setColumnComboboxFertilizzanti();
        break;
      case FORMULATI:
      case INSETTI:
        this.dettagliformulatiservice.setColumnComboboxFormulati();
        break;
    }
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

  mostraFlagMagazziniEsterni(){
    let mostra = false;

    if(this.qdcservice.TestataForm &&
      this.qdcservice.obj_Inizializza_QdC?.ListaFabbricatiConUsodaTerzi?.length > 0 &&
      !this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA() &&
      this.qdcservice.TestataForm.get("Operazioni")?.getRawValue()?.findIndex((o: Lavorazione)=>this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+o.primaryKey.codice)) === -1){
      mostra = true;
    }

    return mostra;
  }

}
