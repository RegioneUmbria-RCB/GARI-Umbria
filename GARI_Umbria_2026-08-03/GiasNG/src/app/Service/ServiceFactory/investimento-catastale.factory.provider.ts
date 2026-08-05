import { IntlService } from "@progress/kendo-angular-intl";
import { InvestimentoCatastaleFiltriService } from "app/anagrafica/catasto/investimento-catastale/investimento-catastale-filtri.service";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { InvestimentoCatastaleService } from "../Anagrafica/investimento-catastale.service";
import { BudgetService } from "../Budget/budget.service";
import { InvestimentoCatastaleBudgetService } from "../Budget/investimento-catastale.budget.service";
import { MasterService } from "../master.service";
import { INVESTIMENTOCATASTALE_SERVICE_TOKEN } from "./investimento-catastale.factory.service";


export let InvestimentoCatastaleServiceFactory = (
    ajaxAgronicaService: AjaxAgronicaService,
    ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    masterService: MasterService,
    intlService: IntlService,
    investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService,
    budgetService: BudgetService) => {
    const currentBudget = budgetService.getBudget();
    //budgetService.currentBudget.subscribe((currentBudget) => {
        if (!currentBudget.activeBudget) {
            return new InvestimentoCatastaleService(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, intlService, investimentoCatastaleFiltriService);
        } else {
            return new InvestimentoCatastaleBudgetService(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, intlService, investimentoCatastaleFiltriService, budgetService);
        }
    //})

};

export let InvestimentoCatastaleServiceProvider = {
  provide: INVESTIMENTOCATASTALE_SERVICE_TOKEN,
  useFactory: InvestimentoCatastaleServiceFactory,
    deps: [
      AjaxAgronicaService,
      AjaxAgronicaAPIService,
      MasterService,
        IntlService,
      InvestimentoCatastaleFiltriService,
      BudgetService
  ]
};
