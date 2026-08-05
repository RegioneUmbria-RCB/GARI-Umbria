import { AfterViewInit, Component, DestroyRef, inject, Input, ViewChild } from '@angular/core';
import { filter, forkJoin, from, map, Observable, Subject, switchMap, tap } from "rxjs";
import { ZooOperationsFilters, ZooPrescriptionsFilters } from "../../models/zoo-operations-filters.model";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { generateGridProviders, GiasKendoGridComponent, GiasKendoGridModule, HttpAction, KendoGridRow, KendoServerResult } from "gias-kendo-grid";
import { enum_TipoPrescrizione } from "../../models/tipo-prescrizione.enum";
import { ZooPrescriptionsGridConfigService } from 'app/zoo/pages/protocolli/prescription-grid.service';
import { ZooPrescriptionGridFlatItem } from 'app/zoo/models/zoo-prescription-grid-item.model';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { PrescrizioniClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LabelModule } from '@progress/kendo-angular-label';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { KENDO_TOOLTIPS } from '@progress/kendo-angular-tooltip';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { enum_TypeTab_Zootecnia } from 'app/zoo/models/tipi-enumerativi-zoo';
import { Enum_DBTypeOperation, GiasDialogService, GiasUikitModule, ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { TranslocoService } from '@jsverse/transloco';

@Component({
  standalone: true,
  selector: 'zoo-protocolli',
  templateUrl: './protocolli.component.html',
  styleUrls: ['./protocolli.component.css', '../../components/zoo-operations-grid/zoo-operations-grid.component.css'],
  imports: [
    GiasUikitModule,
    GiasKendoGridModule,
    LayoutModule,
    IconsModule,
    LabelModule,
    InputsModule,
    ButtonsModule,
    KENDO_TOOLTIPS,
    TranslocoRootModule
  ],
  providers: [
    ...generateGridProviders(ZooPrescriptionsGridConfigService, ProtocolliComponent)
  ]
})
export class ProtocolliComponent implements AfterViewInit {
  
  @ViewChild('zooProtocolsGrid') grid: GiasKendoGridComponent;
  @Input() filters$: Subject<ZooOperationsFilters>;
  private destroyRef = inject(DestroyRef);
  private first = true;

  private _lastUsedFilters: ZooOperationsFilters;

  protected canEdit = true;

  private get selected(): ZooPrescriptionGridFlatItem[] {
    return this.grid.rows.filter(r => r['Selected']) as ZooPrescriptionGridFlatItem[];
  }

  constructor(private gestioneRichieste: GestioneRichiesteService, 
            protected prescriptions: PrescrizioniClient,
            protected agenda: ObjParametriAgendaService,
            private dialog: GiasDialogService,
            private transloco: TranslocoService,
            private permissions: PermessiUtenteService) {
              this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);
            }

  ngAfterViewInit() {
    this.filters$.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(x => x !== null),
      tap(x => this._lastUsedFilters = x),
      filter(() => !!this.grid),
      map(filters => {
        const ff = filters as ZooPrescriptionsFilters;
        ff.prescriptionType = enum_TipoPrescrizione.Protocollo_Terapeutico;
        return ff;
      }),
      filter(() => this.first),
      //tap(() => this.first = false),
      switchMap((filters) => { 
        return this.grid.config.read(filters) 
      }),
    ).subscribe(() => this.grid.publicService.refresh(true));
  }

  public removeSelected() {
          this.preventIfEmptySelection(() => {
            const prompt = this.selected.length > 1
              ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
              : this.transloco.translate('SoleDeletionConfirmation');
            this.dialog.dialogMessageObs_Result(this.transloco.translate('ActivityDeletion'), prompt)
              .pipe(
                filter(r => r['returnObj']),
                switchMap(() => this.grid.config.perform(HttpAction.REMOVE, this.selected)),
                tap(() => console.debug('After delete???')),
                switchMap(() => this.refresh())
              ).subscribe(() => console.debug('subscribe after delete???'));
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
    if (this.selected.length > 0) {
      fun.call(this);
    } else {
      this.dialog.baseError('Errore_', 'SelezionaAttivitàPerContinuare');
    }
  }

  public startProtocollo(dataItem: any) {
    this.createOperationFromProtocol(dataItem);
  }

  public programProtocollo(dataItem: any) {
    this.programOperationFromProtocol(dataItem);
  }
  private createOperationFromProtocol(protocol: ZooPrescriptionGridFlatItem) {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
        this.prescriptions.prescrizioniGetAttivitaFromPrescrizione(protocol.IdRicetta)
          .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
      ]).subscribe(([redirectUrl, attivita]) => {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        objP.GenericObj_string = JSON.stringify(attivita);
        let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.Prescriptions.toString()};
        this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
      });
  }

  private programOperationFromProtocol(protocol: ZooPrescriptionGridFlatItem) {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
        this.prescriptions.prescrizioniGetAttivitaFromPrescrizione(protocol.IdRicetta)
          .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
      ]).subscribe(([redirectUrl, attivita]) => {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        attivita.Tipo_Prescrizione = enum_TipoPrescrizione.Protocollo_Terapeutico_Programmato;
        objP.GenericObj_string = JSON.stringify(attivita);
        let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.Prescriptions.toString()};
        this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
      });
  }

    public selectedStable(): string {
            let rows: KendoGridRow[] = this.grid?.rows;
            let strSelectedStable = "";
            if (rows){
              if (!(rows.length === 0 || rows.some(item => item['Piva'] !== rows[0]['Piva'] || item['Sa_Cod'] !== rows[0]['Sa_Cod'] || item['STA_NUM'] !== rows[0]['STA_NUM']))) {
                strSelectedStable = rows[0]["STA_DES"];
              }
            }
            return strSelectedStable;
          }

}
