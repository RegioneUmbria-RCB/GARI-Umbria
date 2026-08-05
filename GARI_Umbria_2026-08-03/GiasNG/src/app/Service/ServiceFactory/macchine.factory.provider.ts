import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MacchineService } from "../Anagrafica/macchine.service";
import { BudgetService } from "../Budget/budget.service";
import { MacchineBudgetService } from "../Budget/macchine.budget.service";
import { MasterService } from "../master.service";
import { MACCHINE_SERVICE_TOKEN } from "./macchine.factory.service";

export let MacchineServiceFactory = (
    masterService: MasterService,
    ajaxAgronicaService: AjaxAgronicaService,
    ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    budgetService: BudgetService) => {
    const currentBudget = budgetService.getBudget();
    //budgetService.currentBudget.subscribe((currentBudget) => {
        if (!currentBudget.activeBudget) {
            return new MacchineService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService);
        } else {
            return new MacchineBudgetService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService);
        }
    //})

};

export let MacchineServiceProvider = {
  provide: MACCHINE_SERVICE_TOKEN,
  useFactory: MacchineServiceFactory,
  deps: [
      MasterService,
      AjaxAgronicaService,
      AjaxAgronicaAPIService,
      BudgetService
  ]
};
