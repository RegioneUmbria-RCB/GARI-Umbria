import { NgxIndexedDBService } from "ngx-indexed-db";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { ImpreseService } from "../Anagrafica/imprese.service";
import { AnagraficaNGClient } from "../api.service";
import { BudgetService } from "../Budget/budget.service";
import { ImpreseBudgetService } from "../Budget/imprese.budget.service";
import { MasterService } from "../master.service";
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from "./imprese.factory.service";

export let ImpreseServiceFactory = (
  masterService: MasterService,
  ajaxAgronicaService: AjaxAgronicaService,
  ajaxApiService: AjaxAgronicaAPIService,
  budgetService: BudgetService,
  dbService: NgxIndexedDBService,
  anagraficaNG: AnagraficaNGClient
) => {
  const currentBudget = budgetService.getBudget();
  //budgetService.currentBudget.subscribe((currentBudget) => {
  if (!currentBudget.activeBudget) {
    return new ImpreseService(masterService, ajaxAgronicaService, ajaxApiService, dbService, anagraficaNG);
  } else {
    return new ImpreseBudgetService(masterService, ajaxAgronicaService, ajaxApiService, dbService, anagraficaNG);
  }
};

export let ImpreseServiceProvider = {
  provide: IMPRESE_SERVICE_TOKEN,
  useFactory: ImpreseServiceFactory,
  deps: [
    MasterService,
    AjaxAgronicaService,
    AjaxAgronicaAPIService,
    BudgetService,
    NgxIndexedDBService,
    AnagraficaNGClient
  ]
};
