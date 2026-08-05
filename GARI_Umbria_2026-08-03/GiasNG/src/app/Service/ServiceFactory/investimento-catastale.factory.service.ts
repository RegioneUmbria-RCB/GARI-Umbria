import { Injectable, InjectionToken } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { InvestimentoCatastaleFiltriService } from "app/anagrafica/catasto/investimento-catastale/investimento-catastale-filtri.service";
import { Appezzamento } from "app/Model/anagrafiche/Appezzamento";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Impianto } from "app/Model/anagrafiche/Impianto";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { ParticelleCatastali } from "app/Model/anagrafiche/ParticelleCatastali";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import {MasterService, rispostaStandard} from 'app/Service/master.service';
import { Observable } from "rxjs";

export class LeggiInvestimentoCatastale {
  impresa: Impresa;
  centro: CentroAziendale;
  particella: ParticelleCatastali;

  impianto: Impianto;
  data: Date;
}

export enum TipoInvestimento {
  Catasto = 1,
  Impianti = 2
}

export const INVESTIMENTOCATASTALE_SERVICE_TOKEN = new InjectionToken<InvestimentoCatastaleFactoryService>('app.investimento-catastale.service');

@Injectable()
export abstract class InvestimentoCatastaleFactoryService {
  constructor(
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected masterService: MasterService,
    protected intlService: IntlService,
    protected investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService
  ) {  }

  abstract leggi(): Observable<any[]>;
}
