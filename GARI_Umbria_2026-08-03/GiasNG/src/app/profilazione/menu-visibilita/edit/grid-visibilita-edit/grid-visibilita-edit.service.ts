import {Injectable, Injector} from '@angular/core';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {
  GridProfilazioneServerResult
} from "../../../gestione-utenti/utenti/griglia-menu-profilazione/grid-menu-profilazione.service";
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode, JsonKendoResult,
  KendoGridColumn,
  LoaderType,
  ModelEntry, RendererGridEvent
} from 'gias-kendo-grid';
import {BehaviorSubject, filter, map, Observable, of, switchMap, takeUntil} from "rxjs";
import {VisibilitaService} from "../../../services/visibilita.service";
import {CommandsColumnSettings} from 'gias-kendo-grid';
import {ImpreseService} from "../../../../Service/Anagrafica/imprese.service";
import {catchError, take, tap} from "rxjs/operators";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {distinct} from "@progress/kendo-data-query";
import {GiasDialogService} from "../../../../Service/gias-dialog.service";
import { PadriGerarchia2 } from '../../../../Service/api.service';

@Injectable()
export class GridVisibilitaEditService extends AbstractGridConfigService<GridProfilazioneServerResult> {
  public editingMode = EditingMode.IN_PAGE;
  public gridId = "gridImprese";
  public loader = LoaderType.SERVICE;
  public rowId = "Piva";

  private impreseLoaded$ = new BehaviorSubject<any[]>([]);
  private rendererSubscribed = false;

  private kendoRows = [];
  private kendoModel = {
    Attivo : new ModelEntry(CELL_TYPES.STRING),
    Cap : new ModelEntry(CELL_TYPES.STRING),
    chiave : new ModelEntry(CELL_TYPES.STRING),
    Codice_Cuaa: new ModelEntry(CELL_TYPES.STRING),
    Codice_Fiscale: new ModelEntry(CELL_TYPES.STRING),
    Codice_Socio: new ModelEntry(CELL_TYPES.STRING),
    Com: new ModelEntry(CELL_TYPES.STRING),
    Com_Cod_Istat: new ModelEntry(CELL_TYPES.STRING),
    Contratto_Produzione: new ModelEntry(CELL_TYPES.STRING),
    Cooperativa_Referente: new ModelEntry(CELL_TYPES.STRING),
    Data_Creazione: new ModelEntry(CELL_TYPES.STRING),
    Data_Modifica: new ModelEntry(CELL_TYPES.STRING),
    frz_des: new ModelEntry(CELL_TYPES.STRING),
    ind_des: new ModelEntry(CELL_TYPES.STRING),
    Indirizzo: new ModelEntry(CELL_TYPES.STRING),
    Num_Padri: new ModelEntry(CELL_TYPES.STRING),
    Piva: new ModelEntry(CELL_TYPES.STRING),
    Piva_Padre: new ModelEntry(CELL_TYPES.STRING),
    partitaIvaReale: new ModelEntry(CELL_TYPES.STRING),
    Pro_Cod_Istat: new ModelEntry(CELL_TYPES.STRING),
    Prov: new ModelEntry(CELL_TYPES.STRING),
    Provincia: new ModelEntry(CELL_TYPES.STRING),
    Rag_Soc: new ModelEntry(CELL_TYPES.STRING),
    Rag_Soc_Padre: new ModelEntry(CELL_TYPES.STRING),
    SAU_Totale: new ModelEntry(CELL_TYPES.STRING),
    Sa_Cod: new ModelEntry(CELL_TYPES.STRING),
    Sa_Nome: new ModelEntry(CELL_TYPES.STRING),
    Stato: new ModelEntry(CELL_TYPES.STRING),
    Stato_Cod: new ModelEntry(CELL_TYPES.STRING),
    Superficie_Tare: new ModelEntry(CELL_TYPES.STRING),
    Superficie_Totale: new ModelEntry(CELL_TYPES.STRING),
    Tecnico_Referente: new ModelEntry(CELL_TYPES.STRING),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING),
    Validita_Inizio: new ModelEntry(CELL_TYPES.STRING),
    Validita_Fine: new ModelEntry(CELL_TYPES.STRING),
  };
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field:'Rag_Soc', title:this.transloco.translate('RagioneSociale')}, {editable: false}),
    new KendoGridColumn({ field:'partitaIvaReale', title:this.transloco.translate('PartitaIVA')}, {editable: false}),
    new KendoGridColumn({ field:'Codice_Cuaa', title:this.transloco.translate('CodiceUnicoAziendaAgricolaSigla')}, {editable: false}),
    new KendoGridColumn({ field:'Codice_Socio', title:this.transloco.translate('CodiceSocio')}, {editable: false,  hidden: true}),
    new KendoGridColumn({ field:'Rag_Soc_Padre', title:this.transloco.translate('ImpresaReferente')}, {editable: false}),
    new KendoGridColumn({ field:'Indirizzo', title:this.transloco.translate('Indirizzo')}, {editable: false}),
    new KendoGridColumn({ field:'Sa_Cod', title:this.transloco.translate('CentroCodice')}, {editable: false, hidden: true}),
    new KendoGridColumn({ field:'Sa_Nome', title:this.transloco.translate('NomeCentro')}, {editable: false, hidden: true}),
  ];

  constructor(
      injector: Injector,
      private impreseService: ImpreseService,
      private visibilitaService: VisibilitaService,
      private dataShare: ProfilazioneDataShareService,
      private dialog: GiasDialogService
  ) {
    super(injector);
    this.handleCustomizations();
    this.visibilitaService.$imprese.pipe(takeUntil(this.signal))
      .subscribe(imprese => this.impreseLoaded$.next(imprese));
    this.visibilitaService.gridRefresh$
      .pipe(takeUntil(this.signal))
      .subscribe(() => this.gridPublicService.refresh(true));
  }

  private get visibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private get gridRows(): any[] {
    if (this.gridPublicService?.gridComp?.data) {
      return this.gridPublicService.gridComp.data['data'];
    }
    return [];
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const visibleRows = this.visibleRows;
    const rows = this.gridRows;
    if (rows.length > 0 && visibleRows.length > 0) {
      const pive = this.kendoRows.map(r => r.piva);
      const padrePresente = (a) => pive.includes(a.Piva_Padre);
      if (rows.length > 0) {
        const deleteBtns = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('button#btnDelete');
        visibleRows.forEach((node, i) => {
          deleteBtns[i].hidden = padrePresente(rows[i]);
        });
      }
    }
  }

  read(options: any): Observable<GridProfilazioneServerResult> {
    const isImpresaSelezionata = (piva) =>
      !!this.visibilitaService.impreseSelezionate.find(d => d.piva === piva);
    if (this.visibilitaService.impreseSelezionate.length === 0) {
      return of(new GridProfilazioneServerResult(this.kendoModel, this.kendoColumns, this.kendoRows));
    }
    this.loadingService.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});
    console.debug('Loading started');
    return this.impreseLoaded$.pipe(
      filter(i => i.length > 0),
      tap((rows) => this.kendoRows = rows),
      tap(() => this.kendoRows.forEach(i => i.Indirizzo = i.Indirizzo + " - " + i.Provincia + ", " + i.Stato )),
      tap(() => this.dataShare.nImpreseInEdit = this.kendoRows.length),
      map(() => new GridProfilazioneServerResult(this.kendoModel, this.kendoColumns, this.kendoRows)),
      tap(() => {this.loadingService.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef}); console.debug('Loading complete')}),
      catchError(() => {
        this.loadingService.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});
        console.debug('Catching error during read() of visibilita-utente-grid');
        return of(new GridProfilazioneServerResult(this.kendoModel, this.kendoColumns, []));
      }),
    );
  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    if (actionType === HttpAction.REMOVE) {
      this.removeCompany(items);
      return of([]);
    }
    return null;
  }

  private removeCompany(items: any): void {
    const pivaPadre = items.Piva_Padre;
    let haFigli = this.kendoRows.some(r => r.Piva_Padre === items.Piva);
    let padrePresente = false;
    if (pivaPadre) {
      padrePresente = this.kendoRows.some(r => r.Piva === pivaPadre);
    }
    if (padrePresente) {
      this.dialog.baseError('_Errore', 'prof.ImpossibileElinareAziendaConPadreVisibile');
    } else {
      const msg = haFigli ? 'prof.WarningEliminazioneAziendaEFigliVisibilita' : 'prof.WarningEliminazioneAziendaVisibilita';
      this.dialog.warningThen('prof.EliminazioneAziendaVisibilita', msg, true, () => this.removeFormVisibilita(items));
    }
  }

  private removeFormVisibilita(azienda: any) {
    const username = this.visibilitaService.utentiSelezionati[0].UserName;
    this.visibilitaService.rimuoviImpreseDaVisibilita(this.visibilitaService.utentiSelezionati, [azienda])
      .pipe(take(1), switchMap(() => this.visibilitaService.leggiVisibilitaUtente(username, true)))
      .subscribe((imprese) => {
        this.visibilitaService.impreseSelezionate = imprese;
        const companies = this.impreseLoaded$.value;
        let updated: Set<string> = new Set([... imprese.map(x => x.piva)]);
        this.impreseLoaded$.next(companies.filter(x => updated.has(x.Piva)));
        this.visibilitaService.gridRefresh$.next(true);
      });
  }

  private handleCustomizations() {
    this.selectable.shouldShowCheckbox = false;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.columnMenu.kendoGridColumnChooser = false;
    this.views.enabled = false;
    this.groups.groupable.enabled = false;
    this.pagination.gridState.take = 10;
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
    });
  }

}
