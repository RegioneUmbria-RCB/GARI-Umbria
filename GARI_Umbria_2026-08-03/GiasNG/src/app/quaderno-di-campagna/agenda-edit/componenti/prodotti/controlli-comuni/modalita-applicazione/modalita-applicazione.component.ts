import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { QdCService } from "../../../../service/qdc.service";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import {
  UtilityFunctions
} from "../../../../../../Utility/UtilityFunctions";
import { AgendaService, Leggi_Modalita_Applicazione } from "../../../../../../Service/Agenda/Agenda.service";
import { map, Subscription } from "rxjs";
import { rispostaStandard } from "../../../../../../Service/master.service";
import { BaseCodeDescr } from "../../../../../../Model/baseClass/baseCodeDescr";

@Component({
  standalone: false,
  selector: 'app-modalita-applicazione',
  templateUrl: './modalita-applicazione.component.html',
  styleUrls: ['./modalita-applicazione.component.css']
})
export class ModalitaApplicazioneComponent implements OnInit, OnDestroy {

  constructor(public parent: FormGroupDirective,
    public qdcservice: QdCService,
    public agendaservice: AgendaService) { }

  ProdottiForm: FormGroup;

  Subs: Subscription = new Subscription();

  ngOnInit(): void {
    this.ProdottiForm = <FormGroup>this.parent.form;

    //Aggiorno il valore di Modalita_Applicazione in tutti i DosiProdotti
    this.Subs.add(this.ProdottiForm.get("Modalita_Applicazione").valueChanges.subscribe(value => {

      let DosiProdotti = this.qdcservice.DosiProdottiFormArray(null, this.ProdottiForm.get("Operazione").value);

      if (DosiProdotti && DosiProdotti.controls.length > 0) {
        for (let i = 0; i < DosiProdotti.controls.length; i++) {
          DosiProdotti.controls[i].patchValue({
            Modalita_Applicazione: value
          });
        }
      }
    }));
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

  public openControlModalitaApplicazione(ddlEl: GiasDropDownTemplateSComponent) {

    const Leggi_Modalita = <Leggi_Modalita_Applicazione>{
      Operazione: this.ProdottiForm.get("Operazione").getRawValue()
    };

    let fn = this.agendaservice.Leggi_Modalita_Applicazione_QdC(Leggi_Modalita).pipe(map((r: rispostaStandard<Array<BaseCodeDescr>>) => {
      if (r.RispostaStringa && r.RispostaStringa.length > 0) {
        r.RispostaStringa.unshift(new BaseCodeDescr(0, ""));
      }
      return r.RispostaStringa;
    }));

    UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefinedObs(ddlEl, fn);
  }

}
