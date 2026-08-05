import { Injectable } from "@angular/core";
import { IntlService } from "@progress/kendo-angular-intl";
import { PKCentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import {BehaviorSubject, Observable, of} from 'rxjs';
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { ConversionService } from "../conversion.service";
import {MasterService, RispostaStandard, rispostaStandard} from "../master.service";
import { ObjParametriAgendaService } from "../obj-parametri-agenda.service"; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { map } from 'rxjs/operators';
import { BudgetTestata } from "app/Model/budget/budget.testata";
import {NavigationService} from '../navigation.service';
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import {Appezzamento} from "../../Model/anagrafiche/Appezzamento";

export class SelectedBudget {
  budgetId: number;
  activeBudget: boolean;
  piva: string;
}

@Injectable({ providedIn: 'root' })
export class BudgetService {
  private previousBudget: BudgetTestata;
  private budget = new SelectedBudget();
  private budgetNome: string;

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected conversionService: ConversionService,
    protected intlService: IntlService,
    private parametriAgenda: ObjParametriAgendaService,
    private navigationService: NavigationService
  ) {
    const centroPK = { partitaIva: '', codice: 0 } as PKCentroAziendale;
    const appezzamentoPK = { codice: 0, centroAziendalePK: centroPK };
    this.changeBudget({budgetId: 0, activeBudget: false, piva: ''})
  }

  private headerBudgetSource = new BehaviorSubject(this.budget);
  currentBudget: Observable<SelectedBudget> = this.headerBudgetSource.asObservable();

  changeBudget(upd_budgetModel: SelectedBudget) {
    this.headerBudgetSource.next(upd_budgetModel);
  }

  getBudget(): SelectedBudget {
    return this.headerBudgetSource.getValue();
  }

  leggiBudgetAttivo(): Observable<BudgetTestata> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, BudgetTestata>(
      'Budget/Leggi_Budget_Attivo',
      this.parametriAgenda.getObjParamValue()
    ).pipe(
      map((risposta: rispostaStandard<BudgetTestata>) => {
        if(risposta.RispostaOK) {
          return risposta.RispostaStringa
        }
        else
          alert("Qualcosa è andato storto.");
      })
    );
  }


  private isPublicBudget(bdg: BudgetTestata): boolean {
    let superuser: string = this.masterService.objP_server.PivaSuperUser;
    return bdg?.Piva == superuser;
  }

  public isBudget(): boolean {
    return (this.previousBudget != undefined || this.getBudget() != undefined) && this.getBudget().budgetId !== 0;
  }

  getParametriObjService(): ObjParametriAgendaService {
    return this.parametriAgenda;
  }

  leggiElencoBudgetTestata(piva: string = '', inUso: boolean = true, tipoBudget: number = 1): Observable<BudgetTestata[]> {
    let params: ReadBudgetTestataParams = new ReadBudgetTestataParams(
      piva,
      inUso,
      tipoBudget
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ReadBudgetTestataParams, BudgetTestata[]>(
      'Budget/LeggiElencoBudgetTestata',
      params
    ).pipe(map((risposta: rispostaStandard<BudgetTestata[]>) => {
      if(risposta.RispostaOK) {
        return risposta.RispostaStringa
      }
      else
        alert("Qualcosa è andato storto.");
    }));
  }

  ribaltaImpiantiBudget(impianti: BudgetAnagrafica<Appezzamento>[]): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<Appezzamento>[], string>(
      'Budget/Ribalta_BudgetReale',
      impianti
    );
  }

}

export class BudgetAnagrafica<T> {
  Id_Budget: number;
  ElementoAnagrafico: T;
  Delete_Reale_From_Ribaltamento: boolean = false;

  public constructor(idBudget: number, elemento: T, deleteRealeFromRibaltamento: boolean = false) {
    this.Id_Budget = idBudget;
    this.ElementoAnagrafico = elemento
    this.Delete_Reale_From_Ribaltamento = deleteRealeFromRibaltamento;
  }
}

export class ReadBudgetTestataParams {
  public piva: string;
  public inUso: boolean;
  public tipoBudget: number;

  public constructor(piva: string, inUso: boolean, tipoBudget: number) {
    this.piva = piva;
    this.inUso = inUso;
    this.tipoBudget =  tipoBudget;
  }
}
