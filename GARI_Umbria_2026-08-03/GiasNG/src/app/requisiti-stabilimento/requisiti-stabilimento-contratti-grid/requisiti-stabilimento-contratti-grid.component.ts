import {Component, ViewChild} from '@angular/core';
import {GiasKendoGridComponent} from 'gias-kendo-grid';
import {catchError, forkJoin, Observable, of, switchMap, tap} from 'rxjs';
import {ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import {RequisitiStabilimentoService} from '../requisiti-stabilimento.service';
import {RequisitiStabilimentoAPIService} from '../../Service/RequisitiStabilimento/requisiti-stabilimento.service';
import {generateGridProviders} from 'gias-kendo-grid';
import {RequisitiStabilimentoContrattiGridConfigurationService} from './requisiti-stabilimento-contratti-grid-configuration.service';

@Component({
  standalone: false,
  selector: 'app-requisiti-stabilimento-contratti-grid',
  templateUrl: './requisiti-stabilimento-contratti-grid.component.html',
  styleUrls: ['./requisiti-stabilimento-contratti-grid.component.css'],
  providers: [
    ...generateGridProviders(RequisitiStabilimentoContrattiGridConfigurationService, RequisitiStabilimentoContrattiGridComponent)
  ]
})
export class RequisitiStabilimentoContrattiGridComponent {
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
          this.requisitiStabilimentoAPIService.readContracts(params)
        ])),
        catchError(() => of([], [])),
        tap(([contracts]) => {
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
