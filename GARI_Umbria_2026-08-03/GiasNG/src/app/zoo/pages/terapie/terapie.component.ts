import { AfterViewInit, Component, DestroyRef, inject, Input, ViewChild } from '@angular/core';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { IconsModule } from '@progress/kendo-angular-icons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { KENDO_TOOLTIPS } from '@progress/kendo-angular-tooltip';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { generateGridProviders, GiasKendoGridComponent, GiasKendoGridModule, HttpAction, KendoServerResult } from 'gias-kendo-grid';
import { Enum_DBTypeOperation, GiasUikitModule, ObjParametriAgenda } from 'gias-ui-kit';
import { ZooTherapiesGridConfigService } from './terapie-grid.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { TerapieClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { filter, forkJoin, from, map, Observable, Subject, switchMap, tap } from 'rxjs';
import { ZooOperationsFilters } from 'app/zoo/models/zoo-operations-filters.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { enum_TypeTab_Zootecnia } from 'app/zoo/models/tipi-enumerativi-zoo';
import { ZooTherapiesFilters, ZooTherapiesGridFlatItem } from 'app/zoo/models/zoo-therapies.model';

@Component({
  standalone: true,
  selector: 'zoo-terapie',
  templateUrl: './terapie.component.html',
  styleUrls: ['./terapie.component.css'],
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
      ...generateGridProviders(ZooTherapiesGridConfigService, TerapieComponent)
  ]
})
export class TerapieComponent implements AfterViewInit {
  @Input() filters$: Subject<ZooOperationsFilters>;
  @ViewChild('zooTherapiesGrid') grid: GiasKendoGridComponent;

  private first = true;
  private destroyRef = inject(DestroyRef);
  private _lastUsedFilters: ZooOperationsFilters;

  public canEdit: boolean = true;

  constructor(
    private gestioneRichieste: GestioneRichiesteService,
    protected therapiesClient: TerapieClient,
    protected agenda: ObjParametriAgendaService,
    private dialog: GiasDialogService,
    private transloco: TranslocoService,
    private permissions: PermessiUtenteService
  ) {
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);
  }

  private get selected(): ZooTherapiesGridFlatItem[] {
    return this.grid.rows.filter(r => r['Selected']) as ZooTherapiesGridFlatItem[];
  }

  private openTherapyFromGrid(dataItem: ZooTherapiesGridFlatItem, action: Enum_DBTypeOperation) {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Terapia_Zoo)),
      this.therapiesClient.terapieGetTerapia(dataItem.Id_Terapia)
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
      ]).subscribe(([redirectUrl, terapia]) => {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = action;
        objP.GenericObj_string = JSON.stringify(terapia);
        let queryParams: { [key:string] : string | string[] } = {
          "t_Tab":enum_TypeTab_Zootecnia.Therapies.toString()
        };
        this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
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

  ngAfterViewInit() {
    this.filters$.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(x => x !== null),
      tap(x => this._lastUsedFilters = x),
      filter(() => !!this.grid),
      map(filters => filters as ZooOperationsFilters),
      filter(() => this.first),
      //tap(() => this.first = false),
      switchMap((filters) => this.grid.config.read(filters)),
    ).subscribe(() => this.grid.publicService.refresh(true));
  }

  public toTherapy(dataItem: any, action: Enum_DBTypeOperation) {
    this.openTherapyFromGrid(dataItem, action);
  }

  public openNewTherapy() {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Terapia_Zoo)),
      this.therapiesClient.terapieGetTerapia()
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
      ]).subscribe(([redirectUrl, terapia]) => {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        objP.GenericObj_string = JSON.stringify(terapia);
        objP.Piva = this.agenda.getObjParamValue().Piva;
        objP.Sa_Cod = this._lastUsedFilters.center;
        objP.Fabbricato = this._lastUsedFilters.stable;

        let queryParams: { [key:string] : string | string[] } = {
          "t_Tab":enum_TypeTab_Zootecnia.Therapies.toString()
        };
        this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
      });
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

}
