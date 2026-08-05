import {Component, Input, OnInit} from '@angular/core';
import {FormGroup} from '@angular/forms';
import {ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import {BaseCodeDescr} from '../../Model/baseClass/baseCodeDescr';
import {BudgetService} from '../../Service/Budget/budget.service';
import {BudgetTestata} from '../../Model/budget/budget.testata';
import {LeggiProdotti, ProdottiService} from '../../Service/Anagrafica/prodotti.service';
import {Impresa} from '../../Model/anagrafiche/Impresa';
import {from} from 'rxjs';
import {map} from 'rxjs/operators';
import {SpecieVegetaliService} from '../../Service/Metaschema/specie-vegetali.service';
import {Specie} from '../../Model/metaschema/utilizzi/Specie';
import {LeggiRapportoSpecifico} from '../../Service/api.service';
import {ContattiService} from '../../Service/Anagrafica/contatti.service';
import {Prodotto} from '../../Model/attivita/risorse/Prodotto';

@Component({
  standalone: false,
  selector: 'app-filtri-requisiti-stabilimento',
  templateUrl: './filtri.component.html',
  styleUrls: ['./filtri.component.css']
})
export class FiltriRequisitiStabilimentoComponent implements OnInit {

  @Input() filtriForm: FormGroup;

  public prodotti: Prodotto[] = [];
  public budgets: BudgetTestata[] = [];
  public cooperative: { Cod_RisUm: number, Cod_Contatto: string, Rag_Soc_Completa: string }[] = [];
  public specieVegetali: Specie[] = [];

  constructor(
    private objPrametriAgendaService: ObjParametriAgendaService,
    private budgetService: BudgetService,
    private prodottiService: ProdottiService,
    private specieVegetaliService: SpecieVegetaliService,
    private contattiService: ContattiService
  ) { }

  ngOnInit(): void {
    this.filtriForm.get('prodotto').disable();
    this.filtriForm.get('budget').disable();
  }

  public changeAssegnazione(e: Event): void {
    if (e) {
      this.filtriForm.get('budget').enable();
    } else {
      this.filtriForm.get('budget').setValue({id_Budget: 0, Nome_Budget: ''});
      this.filtriForm.get('budget').disable();
    }
  }

  public loadSpecieData(e: Event): void {
    if (this.specieVegetali.length === 0) {
      from(this.specieVegetaliService.leggi_FiltroUtente()).pipe(map((val) => {
        if (!(val.findIndex((sp) => { return sp.codice == 0 }) >= 0) ){
          val.push({ codice: 0, descrizione: '' });
        }
        return val;
      })).subscribe(sv => {
        this.specieVegetali = sv;
      });
    }
  }

  public changeSpecie(e: Event): void {
    this.filtriForm.get('prodotto').setValue({codice: 0, descrizione: ''});
    this.enableProductsDdl();
    if (this.filtriForm.get('prodotto').enabled) {
      this.loadProductsData(e['data']);
    }
  }

  private enableProductsDdl(): void {
    if (
      this.filtriForm.get('specie').getRawValue().codice != undefined &&
      this.filtriForm.get('specie').getRawValue().codice != 0 &&
      this.filtriForm.get('cooperativa').getRawValue().Cod_Contatto != undefined &&
      this.filtriForm.get('specie').getRawValue().Cod_Contatto != ''
    ) {
      this.filtriForm.get('prodotto').enable();
    } else
      this.filtriForm.get('prodotto').disable();
  }

  public loadProductsData(specie: BaseCodeDescr): void {
    let lp: LeggiProdotti = new LeggiProdotti();
    lp.impresa = new Impresa();
    lp.specie = this.filtriForm.get('specie').getRawValue();
    lp.impresa.partitaIva = this.filtriForm.get('cooperativa').getRawValue().Cod_Contatto;
    this.prodottiService.Leggi_Trasformati_Vegetali_Anagrafica_With_Default(lp).pipe(
      map((prod) => {
        let prodMapped = prod.map((el) => {
          if (el.codice_alfanumerico != ""){
            el.descrizione = el.codice_alfanumerico + " " + el.descrizione;
          }
          return el;
        })
        return prodMapped;
      })
    ).subscribe(r => {
      this.prodotti = r;
    });
  }

  public loadBudgetsData(e: Event): void {
    this.budgetService.leggiElencoBudgetTestata().subscribe(r => {
      this.budgets = r;
    });
  }

  public loadCoopData(e: Event): void {
    let params: LeggiRapportoSpecifico = {};
    params.rapportoAttivo = true;
    params.cliente = true;
    params.fornitore = true;
    this.contattiService.leggiRapportoSpecifico(params).subscribe(r => {
      this.cooperative = r.RispostaStringa.map(e => {
        return {Cod_Contatto: e.Cod_Contatto, Cod_RisUm: e.Cod_RisUm, Rag_Soc_Completa: e.Rag_Soc_Completa};
      })
    });
  }

  public changeCoop(e: Event): void {
    this.filtriForm.get('prodotto').setValue({codice: 0, descrizione: ''});
    this.enableProductsDdl();
    if (this.filtriForm.get('prodotto').enabled) {
      this.loadProductsData(e['data']);
    }
  }
}
