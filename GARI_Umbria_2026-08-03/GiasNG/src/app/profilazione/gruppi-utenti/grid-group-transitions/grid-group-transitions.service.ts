import {Injectable, Injector} from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import { DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {forkJoin, map, takeUntil, Observable, take} from 'rxjs';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {GridGruppiServerResult} from "../grid-gruppi/grid-gruppi.service";
import {GruppiUtentiService} from "../../services/gruppi-utenti.service";
import {CommandsColumnSettings, ToolbarSettings} from 'gias-kendo-grid';
import {Validators} from '@angular/forms';
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../../models/profilazione.model";
import {BaseCodeDescrStr} from "../../../Model/baseClass/baseCodeDescrStr";

@Injectable()
export class GridGroupTransitionsService extends AbstractGridConfigService<GridGruppiServerResult> {
  public editingMode = EditingMode.IN_LINE;
  public loader = LoaderType.SERVICE;
  public gridId = "gridGrTransitions";
  public rowId = "cod";

  private kModel = {
    transizione_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    transizione: new ModelEntry(CELL_TYPES.STRING),
    servizio_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    servizio: new ModelEntry(CELL_TYPES.STRING)
  };
  private kCols: KendoGridColumn[] = [
    new KendoGridColumn({field: 'transizione_cod', title: this.transloco.translate('Pratica')}, { resizable: true, editable: true, validators: [Validators.required] }),
    new KendoGridColumn({field: 'servizio_cod', title: this.transloco.translate('TransizioniStato')}, { resizable: true, editable: true, validators: [Validators.required]}),
  ];

  constructor(
    injector: Injector,
    private gruppiService: GruppiUtentiService,
    private permessiService: PermessiUtenteService
  ) {
    super(injector);
    this.gruppiService.gridRefresh$.pipe(takeUntil(this.signal))
      .subscribe(mustRefresh => this.gridPublicService.refresh(mustRefresh));
    this.handleCustomization();
  }

  perform(action: HttpAction, items: any): Observable<any[]> {
    if (action === HttpAction.CREATE) {
      this.gruppiService.aggiungiTransizioneUsata(this.gruppiService.gruppo$.value, [{codice: items.servizio_cod, descrizione: ""}]);
    } else if (action === HttpAction.REMOVE) {
      this.gruppiService.rimuoviTransizioni(this.gruppiService.gruppo$.value, [{codice: items.servizio_cod, descrizione: ""}]);
    }
    return null;
  }

  read(options?: any): Observable<GridGruppiServerResult> {
    let caricaTransizioniXGruppoObs = this.gruppiService.caricaTransizioniXGruppo(this.gruppiService.gruppo$.value);
    let caricaServiziTransazioniStatoXGruppiObs = this.gruppiService.caricaServiziTransazioniStatoXGruppi();
    return forkJoin([caricaTransizioniXGruppoObs, caricaServiziTransazioniStatoXGruppiObs])
      .pipe(
        map(T => T[0].map(row => {
            let codiceTransizione = parseInt(row.codice.split("_")[2]);
            let descrizioneTransizione = T[1].find(f => f.codice === codiceTransizione).descrizione;
            let codiceServizio = row.codice;
            let descrizioneServizio = row.descrizione;
            return {
              transizione: descrizioneTransizione,
              transizione_cod: codiceTransizione,
              servizio: descrizioneServizio,
              servizio_cod: codiceServizio
            };
          })),
        map(rows => new GridGruppiServerResult(rows, this.kCols, this.kModel))
      );
  }

  private handleDropdowns() {
    const trCol = this.kCols.find(s => s.field === 'transizione_cod');
    this.gruppiService.caricaServiziTransazioniStatoXGruppi().pipe(take(1))
      .subscribe(pr => {
        let data = pr.map(s => new DropdownListItem(s.codice, s.descrizione));
        trCol.ddl = new DropdownListWithForm('codice', 'transizione_cod', 'descrizione', data);
        trCol.ddl.valuePrimitive = true;
        trCol.ddl.loadOnEdit = false;
        trCol.ddl.descriptionField = 'transizione';
      });

    let col = this.kCols.find(s => s.field === 'servizio_cod');
    col.ddl = new DropdownListWithForm('codice', 'servizio_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'servizio';
    col.ddl.loadFunction = this.loadStateTransictions.bind(this);
  }

  private loadStateTransictions(dataItem): Observable<BaseCodeDescrStr[]> {
    return this.gruppiService.caricaTransizioniStatoXServizio(dataItem.transizione_cod);
  }

  private handleCustomization() {
    const canEdit = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiGruppi, enum_TipoPermesso.LETTURA_SCRITTURA);
    this.cmdColumn = new CommandsColumnSettings({
      infoBtn: false,
      editBtn: false,
      removeBtn: canEdit,
    });
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = canEdit;
    this.groups.groupable.enabled = false;
    this.selectable.selectable.enabled = false;
    this.handleDropdowns();
  }
}
