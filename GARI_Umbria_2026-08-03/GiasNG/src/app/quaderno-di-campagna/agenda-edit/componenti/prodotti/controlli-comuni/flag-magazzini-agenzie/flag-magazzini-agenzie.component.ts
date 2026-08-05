import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {Tipo_Ricetta} from "../../../../../../Model/attivita/Attivita";
import {QdCService} from "../../../../service/qdc.service";
import {Subscription} from "rxjs";
import { Tipo_Attivita, Stati } from 'gias-ui-kit';
import {enum_LAVCOD} from "../../../../../../Model/TipiEnumerativi";
import {Lavorazione} from "../../../../../../Model/attivita/Lavorazione";

@Component({
  standalone: false,
  selector: 'app-flag-magazzini-agenzie',
  templateUrl: './flag-magazzini-agenzie.component.html',
  styleUrls: ['./flag-magazzini-agenzie.component.css']
})
export class FlagMagazziniAgenzieComponent implements OnInit,OnDestroy {

    ProdottiForm: FormGroup;

    Subs = new Subscription();

    constructor(
        public parent: FormGroupDirective,
        public qdcservice: QdCService
    ) {}

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup>this.parent.form;

        this.Subs.add(this.ProdottiForm.get("Visualizza_Magazzini_Agenzie").valueChanges.subscribe(p=>{
            this.changeFlagMagazziniAgenzie()
        }));
    }


    //Se Visualizza_Magazzini_Agenzie è sì allora imposto a Sì Visualizza_Solo_Prodotti_in_Giacenza e lo disabilito
    //per non farlo cambiare all'utente
    changeFlagMagazziniAgenzie(){
        if(this.ProdottiForm.get("Visualizza_Magazzini_Agenzie").value){

            this.ProdottiForm.patchValue({
                Visualizza_Solo_Prodotti_in_Giacenza: true
            });

            this.ProdottiForm.patchValue({
              Visualizza_Magazzini_Esterni: false
            });

            this.ProdottiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").disable({emitEvent: false});

            this.ProdottiForm.get("Visualizza_Magazzini_Esterni").disable({emitEvent: false});
        }else{
            this.ProdottiForm.get("Visualizza_Solo_Prodotti_in_Giacenza").enable({emitEvent: false});

            this.ProdottiForm.get("Visualizza_Magazzini_Esterni").enable({emitEvent: false});
        }
    }

    ngOnDestroy() {
        this.Subs.unsubscribe();
    }

    mostraFlagMagazziniAgenzie(){
        //Visibile solo per la Ricetta non per il brogliaccio e se ci sono delle Agenzie

        let mostra = false;

        if(this.qdcservice.TestataForm && this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta &&
            this.qdcservice.TestataForm.get("TipoRicetta").value === Tipo_Ricetta.Standard_Destinazioni &&
            this.qdcservice.TestataForm.get("Stato").value === Stati.Da_Eseguire &&
            this.qdcservice.obj_Inizializza_QdC?.ListaPivaAgenzie?.length > 0 &&
            !this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA() &&
            this.qdcservice.TestataForm.get("Operazioni")?.getRawValue()?.findIndex((o: Lavorazione)=>this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+o.primaryKey.codice)) === -1){

            mostra = true;
        }

        return mostra;
    }

}
