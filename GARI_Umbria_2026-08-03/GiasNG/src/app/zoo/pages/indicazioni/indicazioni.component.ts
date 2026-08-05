import { AfterViewInit, Component, DestroyRef, inject, Input, ViewChild } from '@angular/core';
import { filter, forkJoin, from, map, Observable, Subject, switchMap, tap } from "rxjs";
import { ZooOperationsFilters, ZooPrescriptionsFilters } from "../../models/zoo-operations-filters.model";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { generateGridProviders, GiasKendoGridComponent, GiasKendoGridModule, HttpAction, KendoGridRow, KendoServerResult } from "gias-kendo-grid";
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { enum_TipoPrescrizione } from "../../models/tipo-prescrizione.enum";
import { ZooIndicationsGridConfigService } from './indications-grid.service';
import { Enum_DBTypeOperation, GiasDialogService, GiasUikitModule, ObjParametriAgenda } from 'gias-ui-kit';
import { KENDO_TOOLTIPS } from '@progress/kendo-angular-tooltip';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { PrescrizioniClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { enum_TypeTab_Zootecnia } from 'app/zoo/models/tipi-enumerativi-zoo';
import { IntlService } from '@progress/kendo-angular-intl';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { TranslocoService } from '@jsverse/transloco';

@Component({
  standalone: true,
  selector: 'zoo-indicazioni',
  templateUrl: './indicazioni.component.html',
  styleUrls: ['./indicazioni.component.css', '../../components/zoo-operations-grid/zoo-operations-grid.component.css'],
  imports: [
    GiasUikitModule,
    GiasKendoGridModule,
    ButtonsModule,
    KENDO_TOOLTIPS,
    TranslocoRootModule
  ],
  providers: [
    ...generateGridProviders(ZooIndicationsGridConfigService, IndicazioniComponent)
  ]
})
export class IndicazioniComponent implements AfterViewInit {
  @ViewChild('zooIndicationsGrid') grid: GiasKendoGridComponent;
  @Input() filters$: Subject<ZooOperationsFilters>;
  private _lastUsedFilters: ZooOperationsFilters;
  private destroyRef = inject(DestroyRef);
  private first = true;
  public canEdit: boolean = true;

  constructor(private gestioneRichieste: GestioneRichiesteService,
    protected prescriptions: PrescrizioniClient,
    protected agenda: ObjParametriAgendaService,
    private permissions: PermessiUtenteService,
    private transloco: TranslocoService,
    private dialog: GiasDialogService,
    private intlService: IntlService) {
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.ZooProtocolliTerapeutici);
  }

  ngAfterViewInit() {
    this.filters$.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(x => x !== null),
      tap(x => this._lastUsedFilters = x),
      filter(() => !!this.grid),
      map(filters => {
        const ff = filters as ZooPrescriptionsFilters;
        ff.prescriptionType = enum_TipoPrescrizione.Indicazione_Terapeutica;
        return ff;
      }),
      filter(() => this.first),
      //tap(() => this.first = false),
      switchMap((filters) => this.grid.config.read(filters)),
    )
    .subscribe(() => this.grid.publicService.refresh(true));
  }

  public startProtocollo(dataItem: any) {
    // Implement the logic to start a protocol from the selected indication
    // This could involve navigating to a different component or opening a dialog
    this.createOperationFromIndication(dataItem);
  }

  private createOperationFromIndication(indication: any) {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
      this.prescriptions.prescrizioniGetAttivitaFromPrescrizione(indication.IdRicetta)
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
    ]).subscribe(([redirectUrl, attivita]) => {
      const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
      objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
      objP.GenericObj_string = JSON.stringify(attivita);
      let queryParams: { [key: string]: string | string[] } = { "t_Tab": enum_TypeTab_Zootecnia.Indications.toString() };
      this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
    })
  }

  public removeSelected() {
    this.preventIfEmptySelection(() => {
      const prompt = this.selected.length > 1
        ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
        : this.transloco.translate('SoleDeletionConfirmation');
      this.dialog.dialogMessageObs_Result(this.transloco.translate('ActivityDeletion'), prompt)
        .pipe(
          filter(r => !!r['returnObj']),
          switchMap(() => this.grid.config.perform(HttpAction.REMOVE, this.selected)),
          // tap(() => console.debug('After delete???')),
          switchMap(() => this.refresh())
        )
        .subscribe()
        // .subscribe(() => console.debug('subscribe after delete???'))
        ;
    });
  }

  private preventIfEmptySelection(fun: Function) {
    if (this.selected.length > 0)
      fun.call(this);
    else
      this.dialog.baseError('Errore_', 'SelezionaAttivitàPerContinuare');
  }

  private get selected(): any[] {
    return this.grid.rows.filter(r => r['Selected']) as any[];
  }

  private refresh(): Observable<KendoServerResult> {
    this._lastUsedFilters.forceReload = true;
    return this.grid.config.read(this._lastUsedFilters)
      .pipe(
        tap(() => this._lastUsedFilters.forceReload = false),
        tap(newData => this.grid.publicService.refresh(true, newData))
      );
  }

}
