import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize, map } from 'rxjs/operators';
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import { MasterService } from 'app/Service/master.service';
import { FiliereCO2Item } from 'app/sostenibilita-co2/models/sostenibilita-co2.model';
import { PerimetroCO2Service } from 'app/sostenibilita-co2/services/perimetro-co2.service';
import {
  CalcoloRischiRequest,
  CalcoloRischiResponse,
  PerimetroRischiMeteoItem,
  RiepilogoRischiRequest,
  RiepilogoRischiResponse
} from '../models/rischi-meteo.model';

@Injectable({ providedIn: 'root' })
export class RischiMeteoService {

  constructor(
    private ajaxService: AjaxAgronicaNetCore6ApiService,
    private masterService: MasterService,
    private perimetroCO2Service: PerimetroCO2Service
  ) {}

  loadFiliere(): Observable<FiliereCO2Item[]> {
    return this.perimetroCO2Service.loadFiliere();
  }

  getRiepilogoRischi(pivaFiliera: string): Observable<PerimetroRischiMeteoItem[]> {
    const body: RiepilogoRischiRequest = { PivaFiliera: pivaFiliera };
    return this.ajaxService.ajaxAPIPost<RiepilogoRischiRequest, RiepilogoRischiResponse>(
      '/v1/rischi/riepilogo-raccolti',
      body,
      true
    ).pipe(map(r => r.RispostaStringa.Perimetro));
  }

  avviaCalcoloRischi(payload: CalcoloRischiRequest): Observable<CalcoloRischiResponse | null> {
    return this.ajaxService.ajaxAPIPost<CalcoloRischiRequest, CalcoloRischiResponse>(
      '/v1/rischi/calcolo-rischi',
      payload,
      true
    ).pipe(
      map(r => r.RispostaOK ? r.RispostaStringa as CalcoloRischiResponse : null),
      finalize(() => this.masterService.set_isLoading({ isLoading: false, message: '' }))
    );
  }
}
