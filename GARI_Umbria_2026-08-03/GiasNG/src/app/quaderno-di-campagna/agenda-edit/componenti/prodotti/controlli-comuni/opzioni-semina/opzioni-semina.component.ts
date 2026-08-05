import { Component, OnDestroy, OnInit, Optional } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { enum_SEMINA_TIPO } from 'app/Model/TipiEnumerativi';
import { QdCProdottiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { skip, Subscription } from 'rxjs';
import { Elenco_Opzioni_Semina } from './elenco-opzioni-semina';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { UtilityFunctions } from "../../../../../../Utility/UtilityFunctions";

@Component({
  standalone: false,
  selector: 'app-opzioni-semina',
  templateUrl: './opzioni-semina.component.html',
  styleUrls: ['./opzioni-semina.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class OpzioniSeminaComponent implements OnInit, OnDestroy {

  SementiForm: FormGroup;

  Subs = new Subscription();

  public Elenco_Opzioni_Semina = Elenco_Opzioni_Semina;

  constructor(private translocoService: TranslocoService,
    private ddlService: GiasDropDownTemplateService,
    private objParametriAgendaService: ObjParametriAgendaService,
    public parent: FormGroupDirective,
    private qdcservice: QdCService,
    @Optional() private prodottiservice: QdCProdottiService) {

    this.Elenco_Opzioni_Semina[0].descrizione = this.translocoService.translate('qdc.RegistraSoloSeminaTrapianto');
    this.Elenco_Opzioni_Semina[1].descrizione = this.translocoService.translate('qdc.RegistraSeminaTrapiantoAggiornaAnagrafica');
    this.Elenco_Opzioni_Semina[2].descrizione = this.translocoService.translate('qdc.FrazionagliAppezzamenti');
  }

  ngOnInit(): void {

    this.SementiForm = <FormGroup>this.parent.form;

    this.Subs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem => {
      switch (ddlElem.FormControlName) {
        case 'Opzioni_Semina':

          if (this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()) {
            this.prodottiservice.GridDosiProdottiPublicService?.refresh(true);
          }

          break;
      }
    }));

    //Aggiorno il valore di Opzioni_Semina in tutti i DosiProdotti
    this.Subs.add(this.SementiForm.get("Opzioni_Semina").valueChanges.subscribe(value => {

      let DosiProdotti = this.qdcservice.DosiProdottiFormArray(null, this.SementiForm.get("Operazione").value)

      if (DosiProdotti && DosiProdotti.controls.length > 0) {
        for (let i = 0; i < DosiProdotti.controls.length; i++) {
          DosiProdotti.controls[i].patchValue({
            Opzioni_Semina: value
          });
        }
      }
    }));

  }

  ngOnDestroy(): void {
    this.Subs.unsubscribe();
  }

  openControlOpzioni_Semina(
    ddlEl: GiasDropDownTemplateSComponent,
    formName: string
  ) {
    let fn: any;

    switch (formName) {
      case 'Opzioni_Semina':

        let Opzioni = this.Elenco_Opzioni_Semina;

        //Se siamo nel caso di multioperazione nascondo la voce frazionamento e modifica anagrafica oppure se ho selezionato
        // più di un impianto nascondo solo la voce del frazionamento
        if (this.qdcservice.TestataForm.get("Operazioni").value && this.qdcservice.TestataForm.get("Operazioni").value.length > 1) {

          Opzioni = Opzioni.filter(o => o.codice !== enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default);

          Opzioni = Opzioni.filter(o => o.codice !== enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default);
        }

        if (this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().length > 1) {
          Opzioni = Opzioni.filter(o => o.codice !== enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default);
        }

        //fn = new Promise((resolve, reject)=>resolve(Opzioni));
        fn = Promise.resolve(Opzioni);

        UtilityFunctions.loadDropDownItems(ddlEl, fn);
        break;
    }
  }

}
