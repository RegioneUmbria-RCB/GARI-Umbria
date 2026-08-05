import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { InvestimentoCatastaleFiltriService } from "app/anagrafica/catasto/investimento-catastale/investimento-catastale-filtri.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { map, Observable, of} from "rxjs";
import {AjaxAgronicaAPIService} from "../ajax-agronica.api.service";
import { InvestimentoCatastaleFactoryService, LeggiInvestimentoCatastale } from "../ServiceFactory/investimento-catastale.factory.service";

@Injectable()
export class InvestimentoCatastaleService extends InvestimentoCatastaleFactoryService {
  constructor(
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected masterService: MasterService,
    protected intlService: IntlService,
    protected investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService
  ) {
    super(ajaxAgronicaService, masterService, intlService, investimentoCatastaleFiltriService)
  }

  leggi(): Observable<any[]> {
    const filtro = this.investimentoCatastaleFiltriService.filtri;
    if (filtro) {
      return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiInvestimentoCatastale, any[]>(
        'AnagraficaNG/LeggiInvestimentoCatastale',
        filtro).pipe(
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
