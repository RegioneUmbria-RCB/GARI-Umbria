import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { Campo } from "app/Model/anagrafiche/Campo";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { ParticelleCatastali } from "app/Model/anagrafiche/ParticelleCatastali";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { FiltroImpresa } from "app/Model/filtri/FiltroImpresa";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { BehaviorSubject, map, Observable, of } from "rxjs";
import {BudgetAnagrafica, BudgetService} from "../../../Service/Budget/budget.service";

export class LeggiInvestimentoCatastaleCampo {
  impresa: Impresa;
  centro: CentroAziendale;
  particella: ParticelleCatastali;
  campo: Campo;
  data: Date;
}

@Injectable({ providedIn:'root' })
export class InvestimentoCatastaleCampoService {
  public filtri = new BehaviorSubject<LeggiInvestimentoCatastaleCampo>(null);

  constructor(
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private masterService: MasterService,
    private intlService: IntlService,
    private budgetService: BudgetService
  ) { }

  leggi(): Observable<any[]> {
    const filtro = this.filtri.getValue();
    if (filtro != undefined) {
      return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiInvestimentoCatastaleCampo, any[]>(
        'AnagraficaNG/LeggiInvestimentoCatastaleCampo',
        filtro
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

  leggiBudget(): Observable<any[]> {
    const filtro = this.filtri.getValue();
    if (filtro != undefined) {
      let filtroBudget: BudgetAnagrafica<LeggiInvestimentoCatastaleCampo> = new BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>(
        this.budgetService.getBudget().budgetId,
        filtro
      );
      const parametri: CoreWS_Generic<BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>> = {
        objP: this.masterService.getCoreWSGenericObjP(),
        InData: filtroBudget
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any[], BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>>(

        this.masterService.link_CoreWS + '/Budget/Catasto.asmx/Leggi_Investimento_Catastale_Campo',

        parametri).pipe(
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
