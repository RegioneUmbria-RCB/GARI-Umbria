import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { Appezzamento } from "app/Model/anagrafiche/Appezzamento";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { ParticelleCatastali } from "app/Model/anagrafiche/ParticelleCatastali";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { FiltroImpresa } from "app/Model/filtri/FiltroImpresa";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { LeggiInvestimentoCatastale, TipoInvestimento } from "app/Service/ServiceFactory/investimento-catastale.factory.service";
import { BehaviorSubject, map, Observable, of } from "rxjs";

@Injectable({providedIn:'root'})
export class InvestimentoCatastaleFiltriService {
  private _filtri$: BehaviorSubject<LeggiInvestimentoCatastale> = new BehaviorSubject<LeggiInvestimentoCatastale>(null);
  private _tipo: TipoInvestimento;

  get tipo(): TipoInvestimento {
    return this._tipo;
  }

  set tipo(value: TipoInvestimento) {
    this._tipo = value;
  }

  get filtri(): LeggiInvestimentoCatastale {
    return this._filtri$.getValue();
  }

  set filtri(value: LeggiInvestimentoCatastale) {
    this._filtri$.next(value);
  }
}
