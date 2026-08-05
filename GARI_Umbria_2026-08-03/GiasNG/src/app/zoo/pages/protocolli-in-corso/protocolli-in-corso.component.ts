import { AfterViewInit, Component, DestroyRef, inject, Input, ViewChild } from '@angular/core';
import { filter, forkJoin, from, map, Observable, Subject, switchMap, tap } from "rxjs";
import { ZooOperationsFilters, ZooPrescriptionsFilters } from "../../models/zoo-operations-filters.model";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { generateGridProviders, GiasKendoGridComponent, GiasKendoGridModule, HttpAction, KendoServerResult, KendoGridRow } from "gias-kendo-grid";
import { enum_TipoPrescrizione } from "../../models/tipo-prescrizione.enum";
import { ZooPrescriptionsInProgressGridConfigService } from './protocolli-in-corso-grid.service';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { KENDO_TOOLTIPS } from '@progress/kendo-angular-tooltip';
import { ZooPrescriptionGridFlatItem } from 'app/zoo/models/zoo-prescription-grid-item.model';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { PrescrizioniClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { enum_TypeTab_Zootecnia } from 'app/zoo/models/tipi-enumerativi-zoo';
import { Enum_DBTypeOperation, GiasUikitModule, ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  standalone: true,
  selector: 'zoo-protocolli-in-corso',
  templateUrl: './protocolli-in-corso.component.html',
  styleUrls: ['./protocolli-in-corso.component.css', '../../components/zoo-operations-grid/zoo-operations-grid.component.css'],
  imports: [
    GiasUikitModule,
    GiasKendoGridModule,
    LayoutModule,
    IconsModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    TranslocoRootModule,
    KENDO_TOOLTIPS
  ],
  providers: [
    ...generateGridProviders(ZooPrescriptionsInProgressGridConfigService, ProtocolliInCorsoComponent)
  ]
})
export class ProtocolliInCorsoComponent implements AfterViewInit {

  private _lastUsedFilters: ZooOperationsFilters;
  private destroyRef = inject(DestroyRef);
  private first = true;

  public canEdit: boolean = true;

  @ViewChild('zooProtocolsInProgressGrid') grid: GiasKendoGridComponent;
  @Input() filters$: Subject<ZooOperationsFilters>;

  constructor(
    private gestioneRichieste: GestioneRichiesteService,
    protected prescriptions: PrescrizioniClient,
    protected agenda: ObjParametriAgendaService,
    private dialog: GiasDialogService,
    private transloco: TranslocoService,
    private permissions: PermessiUtenteService,
    private intlService: IntlService
  ) {
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);
  }

  private get selected(): ZooPrescriptionGridFlatItem[] {
    return this.grid.rows.filter(r => r['Selected']) as ZooPrescriptionGridFlatItem[];
  }

  ngAfterViewInit() {
    this.filters$.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(x => x !== null),
      tap(x => this._lastUsedFilters = x),
      filter(() => !!this.grid),
      map(filters => {
        const ff = filters as ZooPrescriptionsFilters;
        ff.prescriptionType = enum_TipoPrescrizione.Da_Protocollo_GIAS;
        return ff;
      }),
      filter(() => this.first),
      //tap(() => this.first = false),
      switchMap((filters) => this.grid.config.read(filters)),
    )
    .subscribe(() => this.grid.publicService.refresh(true));
  }

  public toAgenda(dataItem: any) {
    const service = this.grid.config as ZooPrescriptionsInProgressGridConfigService;
    service.handleRegisterOperation(dataItem);
  }

  // private createOperationFromProtocol(protocol: ZooPrescriptionGridFlatItem) {
  //   forkJoin([
  //     from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
  //     this.prescriptions.prescrizioniGetAttivitaFromPrescrizione(protocol.IdRicetta, protocol.IdAgenda)
  //       .pipe(
  //         map(r => r.RispostaOK ? r.RispostaStringa as any : null))
  //   ])
  //   .subscribe(([redirectUrl, attivita]) => {
  //     const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
  //     objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
  //     objP.GenericObj_string = JSON.stringify(attivita);
  //     let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.FuturePrescriptions.toString()};
  //     this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
  //   });
  // }

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

  private refresh(): Observable<KendoServerResult> {
    this._lastUsedFilters.forceReload = true;
    return this.grid.config.read(this._lastUsedFilters)
      .pipe(
        tap(() => this._lastUsedFilters.forceReload = false),
        tap(newData => this.grid.publicService.refresh(true, newData))
      );
  }

  private preventIfEmptySelection(fun: Function) {
    if (this.selected.length > 0)
      fun.call(this);
    else
      this.dialog.baseError('Errore_', 'SelezionaAttivitàPerContinuare');
  }

  public lastDateFiltered(): string{
    let firstDate = "";
    let lastDate = "";
    if (this._lastUsedFilters?.from != null && this._lastUsedFilters?.from != undefined)
      firstDate = this.intlService.formatDate(this._lastUsedFilters.from, "dd/MM/yy");

    if (this._lastUsedFilters?.to != null && this._lastUsedFilters?.to != undefined)
      lastDate = this.intlService.formatDate(this._lastUsedFilters.to, "dd/MM/yy");

    return firstDate + " - " + lastDate;
  }

  public selectedStable(): string {
    let rows: KendoGridRow[] = this.grid?.rows;
    let strSelectedStable = "";
    if (rows)
      if (!(rows.length === 0 || rows.some(item => item['Piva'] !== rows[0]['Piva'] || item['Sa_Cod'] !== rows[0]['Sa_Cod'] || item['STA_NUM'] !== rows[0]['STA_NUM'])))
        strSelectedStable = rows[0]["STA_DES"];

    return strSelectedStable;
  }
}
