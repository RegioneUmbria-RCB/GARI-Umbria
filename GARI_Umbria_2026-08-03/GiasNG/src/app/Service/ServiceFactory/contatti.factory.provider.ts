import { IntlService } from "@progress/kendo-angular-intl";
import { InvestimentoCatastaleFiltriService } from "app/anagrafica/catasto/investimento-catastale/investimento-catastale-filtri.service";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { ContattiService } from "../Anagrafica/contatti.service";
import { InvestimentoCatastaleService } from "../Anagrafica/investimento-catastale.service";
import { BudgetService } from "../Budget/budget.service";
import { ContattiBudgetService } from "../Budget/contatti.budget.service";
import { InvestimentoCatastaleBudgetService } from "../Budget/investimento-catastale.budget.service";
import { MasterService } from "../master.service";
import { CONTATTI_SERVICE_TOKEN } from "./contatti.factory.service";
import { INVESTIMENTOCATASTALE_SERVICE_TOKEN } from "./investimento-catastale.factory.service";
import { AnagraficaNGClient } from "../api.service";

export let ContattiServiceFactory = (
    ajaxAgronicaService: AjaxAgronicaService,
    ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    masterService: MasterService,
    anagraficaNGClient: AnagraficaNGClient,
    budgetService: BudgetService) => {
    if (!budgetService || typeof budgetService.getBudget !== 'function') {
        return new ContattiService(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, anagraficaNGClient);
    }
    const currentBudget = budgetService.getBudget();
    if (!currentBudget?.activeBudget) {
        return new ContattiService(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, anagraficaNGClient);
    } else {
        return new ContattiBudgetService(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, anagraficaNGClient);
    }
};

export let ContattiServiceProvider = {
  provide: CONTATTI_SERVICE_TOKEN,
  useFactory: ContattiServiceFactory,
    deps: [
        AjaxAgronicaService,
        AjaxAgronicaAPIService,
        MasterService,
        BudgetService
  ]
};
