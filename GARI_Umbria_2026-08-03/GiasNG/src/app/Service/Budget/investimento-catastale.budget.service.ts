import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { InvestimentoCatastaleFiltriService } from "app/anagrafica/catasto/investimento-catastale/investimento-catastale-filtri.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { map, Observable, of} from "rxjs";
import { InvestimentoCatastaleFactoryService, LeggiInvestimentoCatastale } from "../ServiceFactory/investimento-catastale.factory.service";
import {BudgetAnagrafica, BudgetService} from "./budget.service";
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
@Injectable()
export class InvestimentoCatastaleBudgetService extends InvestimentoCatastaleFactoryService {
  constructor(
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected masterService: MasterService,
    protected intlService: IntlService,
    protected investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService,
    protected budgetService: BudgetService
  ) {
    super(ajaxAgronicaService,  masterService, intlService, investimentoCatastaleFiltriService)
  }

  leggi(): Observable<any[]> {
    const filtro = this.investimentoCatastaleFiltriService.filtri;
    if (filtro) {

      return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiInvestimentoCatastale>, any[]>(
        'Budget/LeggiInvestimentoCatastale',
        new BudgetAnagrafica(this.budgetService.getBudget().budgetId, filtro)
      ).pipe(
        map((data) => {
          let Resp = data.RispostaStringa;
          Resp.forEach(el => {
            el.Validita_Inizio = this.intlService.parseDate(el.Validita_Inizio);
            el.Validita_Fine = this.intlService.parseDate(el.Validita_Fine);
          });
          return Resp;
        })
      );
    } else {
      return of([])
    }
  }
}
