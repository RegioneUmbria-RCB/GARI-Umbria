import { TranslocoPipe } from "@jsverse/transloco";
import { NgxIndexedDBService } from "ngx-indexed-db";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { CatastoService } from "../Anagrafica/catasto.service";
import { ImpreseService } from "../Anagrafica/imprese.service";
import { BudgetService } from "../Budget/budget.service";
import { CatastoBudgetService } from "../Budget/catasto.budget.service";
import { ImpreseBudgetService } from "../Budget/imprese.budget.service";
import { MasterService } from "../master.service";
import { CATASTO_SERVICE_TOKEN } from "./catasto.factory.service";
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from "./imprese.factory.service";

export let CatastoServiceFactory = (
    masterService: MasterService,
    ajaxAgronicaService: AjaxAgronicaService,
    ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    translocopipe: TranslocoPipe,
    budgetService: BudgetService) => {
    const currentBudget = budgetService.getBudget();
    //budgetService.currentBudget.subscribe((currentBudget) => {
        if (!currentBudget.activeBudget) {
            return new CatastoService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocopipe);
        } else {
            return new CatastoBudgetService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocopipe);
        }
    //})

};

export let CatastoServiceProvider = {
  provide: CATASTO_SERVICE_TOKEN,
  useFactory: CatastoServiceFactory,
  deps: [
      MasterService,
      AjaxAgronicaService,
      AjaxAgronicaAPIService,
      TranslocoPipe,
      BudgetService
  ]
};
