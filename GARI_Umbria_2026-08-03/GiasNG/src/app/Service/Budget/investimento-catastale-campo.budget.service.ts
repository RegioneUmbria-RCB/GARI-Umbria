import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { LeggiInvestimentoCatastaleCampo } from "app/anagrafica/campi/investimento-catastale-campo/investimento-catastale-campo.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { BehaviorSubject, map, Observable, of } from "rxjs";
import { BudgetAnagrafica, BudgetService } from "./budget.service";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";


@Injectable({ providedIn:'root' })
export class InvestimentoCatastaleCampoBudgetService {
  public filtri = new BehaviorSubject<BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>>(null);

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private intlService: IntlService,
    private budgetService: BudgetService
  ) {  }

  leggi(): Observable<any[]> {
    const filtro = this.filtri.getValue();
    if (filtro) {
      const parametri: BudgetAnagrafica<LeggiInvestimentoCatastaleCampo> = new BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>(
        this.budgetService.getBudget().budgetId,
        {
          campo:filtro.ElementoAnagrafico.campo,
          centro:filtro.ElementoAnagrafico.centro,
          data:filtro.ElementoAnagrafico.data,
          impresa:filtro.ElementoAnagrafico.impresa,
          particella:filtro.ElementoAnagrafico.particella
        }
      );

      return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>, any[]>(
        'Budget/Leggi_Investimento_Catastale_Campo',
        parametri
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
      return of([]);
    }
  }

}
