import { TranslocoPipe, TranslocoService } from "@jsverse/transloco";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { CampiService } from "../Anagrafica/campi.service";
import { CatastoService } from "../Anagrafica/catasto.service";
import { BudgetService } from "../Budget/budget.service";
import { CampiBudgetService } from "../Budget/campi.budget.service";
import { MasterService } from "../master.service";
import { SpecieVegetaliService } from "../Metaschema/specie-vegetali.service";
import { CAMPI_SERVICE_TOKEN } from "./campi.factory.service";
import {IntlService} from '@progress/kendo-angular-intl';

export let CampiServiceFactory = (
    masterService: MasterService,
    ajaxAgronicaService: AjaxAgronicaService,
    ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    translocoService: TranslocoService,
    specievegetaliservice: SpecieVegetaliService,
    budgetService: BudgetService,
    intlService: IntlService
    ) => {
    const currentBudget = budgetService.getBudget();
    //budgetService.currentBudget.subscribe((currentBudget) => {
        if (!currentBudget.activeBudget) {
            return new CampiService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocoService, specievegetaliservice, intlService);
        } else {
            return new CampiBudgetService(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocoService, specievegetaliservice, budgetService, intlService);
        }
    //})

};

export let CampiServiceProvider = {
  provide: CAMPI_SERVICE_TOKEN,
  useFactory: CampiServiceFactory,
  deps: [
      MasterService,
      AjaxAgronicaService,
      AjaxAgronicaAPIService,
      TranslocoPipe,
      SpecieVegetaliService,
      BudgetService
  ]
};
