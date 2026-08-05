import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { generateGridProviders } from 'gias-kendo-grid';
import { IndirizziConfigService } from './indirizzi-grid-config.service';
import { Subscription } from 'rxjs';
import { IndirizziAppezzamentoDataService, IndirizzoAssociatoChiave } from './indirizzi-data.service';
import { Indirizzo } from '../../../../Model/anagrafiche/addresses/Indirizzo';
import { Istat } from 'app/Model/metaschema/Istat';
import { CodiciNazioniISO3166 } from 'app/Model/metaschema/CodiciNazioniISO3166';
import { IndirizziParentFormDataService } from './indirizzi-parent-from.service';
import {IndirizziAppezzamentoService} from './indirizzi.service';

@Component({
  standalone: false,
  selector: 'app-appezzamento-edit-indirizzi',
  templateUrl: './indirizzi.component.html',
  providers:[
    ...generateGridProviders(IndirizziConfigService, IndirizziComponent),
    IndirizziAppezzamentoDataService,
    IndirizziAppezzamentoService
  ]
})
export class IndirizziComponent implements OnInit,OnDestroy {
  @Input('indirizzi') IndirizziFormArray: FormArray = this.fb.array([]);

  private indirizziDataSub: Subscription;

  constructor(
    private fb: FormBuilder,
    public intl: IntlService,
    private indirizziAppezzamentoDataService: IndirizziAppezzamentoDataService,
    private indirizziParentFormDataService: IndirizziParentFormDataService,
  ) {  }

  ngOnInit() {
    let indirizzi: IndirizzoAssociatoChiave[] = [];
    this.IndirizziFormArray.value.forEach(ind => {
      let indirizzo: IndirizzoAssociatoChiave = new IndirizzoAssociatoChiave();
      indirizzo.indirizzo = new Indirizzo();
      indirizzo.indirizzo.istatComune = new Istat();
      indirizzo.indirizzo.stato = new CodiciNazioniISO3166(ind.indirizzo.stato.codice, ind.indirizzo.stato.descrizione, '', '', 0);

      indirizzo.indirizzo.cap = ind.indirizzo.cap;
      indirizzo.indirizzo.codice = ind.indirizzo.codice;
      indirizzo.indirizzo.frazione = ind.indirizzo.frazione;
      indirizzo.indirizzo.istatComune.com = ind.indirizzo.istatComune.com;
      indirizzo.indirizzo.istatComune.localita = ind.indirizzo.istatComune.localita;
      indirizzo.indirizzo.istatComune.prov = ind.indirizzo.istatComune.prov;
      indirizzo.indirizzo.istatComune.comuni_prov = ind.indirizzo.istatComune.comuni_prov;
      indirizzo.indirizzo.istatComune.cap = ind.indirizzo.istatComune.cap;
      indirizzo.indirizzo.note = ind.indirizzo.note;
      indirizzo.indirizzo.via = ind.indirizzo.via;
      indirizzo.tipo_Indirizzo = ind.tipo_indirizzo;
      indirizzi.push(indirizzo);
    });

    this.indirizziAppezzamentoDataService.indirizziAppezzamento = indirizzi;
    this.indirizziParentFormDataService.indirizziParentForm = <FormGroup>this.IndirizziFormArray.parent;

    this.indirizziDataSub = this.indirizziAppezzamentoDataService.indirizziAppezzamentoSource$
      .subscribe(i => {
        this.setIndirizziFormArray(i);
      });
  }

  ngOnDestroy() {
    this.indirizziDataSub.unsubscribe();
  }

  private setIndirizziFormArray(i: IndirizzoAssociatoChiave[]) {
    this.IndirizziFormArray.controls.splice(0);
    i.forEach(ind => {
      this.IndirizziFormArray.controls.push(this.fb.group(ind));
    });
  }

}
