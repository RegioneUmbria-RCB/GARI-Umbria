import { Component, ViewChild } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { RequisitiStabilimentoGridConfigurationService } from './requisiti-stabilimento-grid-configuration.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { RequisitiStabilimentoService } from '../requisiti-stabilimento.service';
import { Observable, catchError, forkJoin, of, switchMap, tap } from 'rxjs';
import { RequisitiStabilimentoAPIService } from 'app/Service/RequisitiStabilimento/requisiti-stabilimento.service';

@Component({
  standalone: false,
  selector: 'app-requisiti-stabilimento-grid',
  templateUrl: './requisiti-stabilimento-grid.component.html',
  styleUrls: ['./requisiti-stabilimento-grid.component.css'],
  providers: [
    ...generateGridProviders(RequisitiStabilimentoGridConfigurationService, RequisitiStabilimentoGridComponent)
  ]
})
export class RequisitiStabilimentoGridComponent {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  loader$: Observable<any>;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private requisitiStabilimentoService: RequisitiStabilimentoService,
    private requisitiStabilimentoAPIService: RequisitiStabilimentoAPIService
  ) {
    this.loader$ = this.requisitiStabilimentoService.requisitiPayload$
      .pipe(
        switchMap(params => forkJoin([
          this.requisitiStabilimentoAPIService.readRequisitiStabilimento(params),
          this.requisitiStabilimentoAPIService.readContracts(params)
        ])),
        catchError(() => of([], [])),
        tap(([data, contracts]) => {
          this.requisitiStabilimentoService.nextResult(data);
          this.requisitiStabilimentoService.nextContracts(contracts);
          this.kendoGrid.forceReload();
        })
      );
  }

  get impresaSelezionata(): boolean {
    const piva = this.objParametriAgendaService.getObjParamValue().Piva;
    return piva != null && piva !== '';
  }
}
