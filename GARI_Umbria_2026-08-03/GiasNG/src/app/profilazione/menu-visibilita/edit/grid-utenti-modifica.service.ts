import {Injectable, Injector} from '@angular/core';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {
  GridProfilazioneServerResult
} from "../../gestione-utenti/utenti/griglia-menu-profilazione/grid-menu-profilazione.service";
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {map, Observable, of, take} from "rxjs";
import {ProfilazioneDataShareService} from "../../services/profilazione-data-share.service";
import {UtenteFlatModel} from "../../models/utente.model";
import {CommandsColumnSettings} from 'gias-kendo-grid';
import {VisibilitaService} from "../../services/visibilita.service";
import {GiasDialogService} from "../../../Service/gias-dialog.service";

@Injectable()
export class GridUtentiModificaService extends AbstractGridConfigService<GridProfilazioneServerResult> {
  public editingMode = EditingMode.IN_PAGE;
  public gridId = 'gridUtentiVisibiilita';
  public rowId = 'UserName';
  public loader = LoaderType.SERVICE;

  private kendoRows = [];
  private kendoColumns = [
    new KendoGridColumn({field: 'UserName', title: this.transloco.translate('Username')}, {editable: false}),
    new KendoGridColumn({field: 'Azienda_Persona', title: this.transloco.translate('prof.TipoUtente')}, {editable: false}),
    new KendoGridColumn({field: 'CognomeNome', title: this.transloco.translate('CognomeNome')}, {editable: false}),
    new KendoGridColumn({field: 'CodFisc', title: this.transloco.translate('CodiceFiscale')}, {editable: false}),
    new KendoGridColumn({field: 'UserNameCommerciale', title: this.transloco.translate('Qualifica')}, {editable: false}),
  ];
  private kendoModel = {
    UserName: new ModelEntry(CELL_TYPES.STRING),
    UserNameCommerciale: new ModelEntry(CELL_TYPES.STRING),
    CodFisc: new ModelEntry(CELL_TYPES.STRING),
    Azienda_Persona: new ModelEntry(CELL_TYPES.STRING),
    CognomeNome: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    injector: Injector,
    private dataShare: ProfilazioneDataShareService,
    private visibilitaService: VisibilitaService,
    private dialog: GiasDialogService
  ) {
    super(injector);
    this.handleCustomizations();
  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    if (actionType === HttpAction.REMOVE) {
      this.dialog.warningThen('prof.EliminazioneAziendaVisibilita', 'prof.WarningRimozioneVisibilita', true,
        () => {
          this.visibilitaService.rimuoviVisibilita([items])
            .pipe(take(1))
            .subscribe(isOk => {
              if (isOk) {
                let i = this.kendoRows.findIndex(u => u.UserName === items.UserName);
                this.kendoRows.splice(i, 1);
              }
              if (this.kendoRows.length === 0)
                this.visibilitaService.saved$.next(true);
            });

        })
    }
    return of([]);
  }

  read(options: any): Observable<GridProfilazioneServerResult> {
    this.kendoRows = this.dataShare.utentiEditVisibilita;
    this.kendoRows.forEach((u: UtenteFlatModel) => u['CognomeNome'] = u.Cognome + ' ' + u.Nome);
    return of(new GridProfilazioneServerResult(this.kendoModel, this.kendoColumns, this.kendoRows));
  }

  private handleCustomizations() {
    this.selectable.shouldShowCheckbox = false;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.columnMenu.kendoGridColumnChooser = false;
    this.views.enabled = false;
    this.groups.groupable.enabled = false;
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
    });
  }

}
