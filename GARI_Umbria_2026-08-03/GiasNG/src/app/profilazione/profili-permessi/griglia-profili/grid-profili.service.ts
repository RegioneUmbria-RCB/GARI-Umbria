import {Injectable, Injector} from '@angular/core';
import {Validators} from '@angular/forms';
import {TipologiaUtente} from '../../models/profili-permessi/tipologia-utente.model';
import {TipologieUtentiService} from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import {MasterService} from 'app/Service/master.service';
import {
  CommandsColumnSettings,
  CommandsDropDownSettings,
  SelectableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {map, Observable, takeUntil} from 'rxjs';
import {GridCommandItem} from "../../../menu-agenda/components/utils";
import {faCopy, faUserGear} from "@fortawesome/free-solid-svg-icons";
import {GiasDialogService} from "../../../Service/gias-dialog.service";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../../models/profilazione.model";

export class GridProfiliServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export enum GridProfiliMenuActions {
  SYNCH_CONNECTED,
  COPY,
  SET_SETTINGS
}

@Injectable()
export class GridProfiliService extends AbstractGridConfigService<GridProfiliServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Profilo_Cod';
    gridId = 'ProfileGridID';

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;

    private rows: TipologiaUtente[] = [];
    private kendoModel = {
      codice: new ModelEntry(CELL_TYPES.STRING),
      descrizione: new ModelEntry(CELL_TYPES.STRING),
      Note: new ModelEntry(CELL_TYPES.STRING),
      Permessi: new ModelEntry(CELL_TYPES.STRING)
    };
    private kendoColumns: KendoGridColumn[] = [
      new KendoGridColumn({field: 'codice', title: this.transloco.translate('Codice')}, {
        hidden: true,
        editable: false
      }),
      new KendoGridColumn({field: 'descrizione', title: this.transloco.translate('Profilo')}, {validators: [Validators.required]}),
      new KendoGridColumn({field: 'Note', title: this.transloco.translate('Note')}),
    ];

    constructor(injector: Injector,
        private profileService: TipologieUtentiService,
        private permessiService: PermessiUtenteService,
        private dialog: GiasDialogService,
        private masterService: MasterService,
    ) {
        super(injector);
        this.permessoEdit = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiProfili, enum_TipoPermesso.LETTURA_SCRITTURA);
        this.permessoInfo = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiProfili, enum_TipoPermesso.LETTURA);
        this.permessoRemove = this.permessoEdit;
        this.handleCustomization();
        this.handleEvents();
    }

    read(options?: any): Observable<GridProfiliServerResult> {
        this.masterService.set_isLoading({ isLoading: true, message: ''});
        return this.profileService.readTipologie().pipe(map( result => {
            this.rows = result;
            this.masterService.set_isLoading({ isLoading: false, message: ''});
            return new GridProfiliServerResult(result, this.kendoColumns, this.kendoModel);
        }));
    }

    perform(actionType: HttpAction, tipologia: TipologiaUtente): Observable<any[]> { // chiamato prima di handleEvent
        if(actionType === HttpAction.CREATE) {
          let base = new TipologiaUtente(-1, tipologia.descrizione);
          base.Note = tipologia.Note;
          this.profileService.saveTipologie([base])
            .subscribe(res => {
              if (res) this.gridPublicService.refresh(true);
            });
        } else if (actionType === HttpAction.UPDATE) {
            return this.profileService.saveTipologie([tipologia])
              .pipe(map(() => [tipologia]));
        } else if(actionType === HttpAction.REMOVE) {
            this.onDeleteProfile(tipologia);
        }
        return null;
    }

  /** Gestisce gli eventi scaturiti dalla pressione di un bottone nel menu dropdown.
   * @private
   */
  private handleEvents() {
    this.gridPublicService.commandEvent.pipe(takeUntil(this.signal))
      .subscribe(ev => {
        if (!ev) return;
        // if (ev.command.action === GridProfiliMenuActions.SYNCH_CONNECTED) {
        //   this.profileService.updateConnectedUsers(ev.dataItem);
        // }
        if (ev.command.action === GridProfiliMenuActions.COPY || ev.command.action === GridProfiliMenuActions.SET_SETTINGS) {
          this.profileService.profileGridEvent$.next({event: ev.command.action, item: ev.dataItem});
        }
      });
    }

    private handleCustomization() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: this.permessoEdit,
            infoBtn: false,
            removeBtn: this.permessoEdit,
        });
        this.setDdlMenu();
        this.selectable.selectable = new SelectableSettings({
            enabled: true,
            checkboxOnly: false,
            mode: "single"
        });
        this.selectable.shouldShowCheckbox =  true;
        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = this.permessoEdit;
        this.groups.groupable.enabled = false;
    }

    private setDdlMenu() {
      const canEditSettings = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni, enum_TipoPermesso.LETTURA_SCRITTURA);
      if (this.permessoEdit || canEditSettings) {
        this.cmdDropDown = new CommandsDropDownSettings();
      // this.cmdDropDown.addCommand(new GridCommandItem(
      //   'prof.AggiornaPermessiUtentiCollegati', GridProfiliMenuActions.SYNCH_CONNECTED, '', faArrowsRotate, true
      // ));
      } else return;
      if (this.permessoEdit) {
        this.cmdDropDown.addCommand(new GridCommandItem(
          'Copia', GridProfiliMenuActions.COPY, '', faCopy, true
        ));
      // if (canEditSettings) {
        this.cmdDropDown.addCommand(new GridCommandItem(
          'prof.ModificaImpostazioni', GridProfiliMenuActions.SET_SETTINGS, '', faUserGear, true
        ));
      // }
      }
    }

    private onDeleteProfile(profile: TipologiaUtente) {
      const title = this.transloco.translate('Attenzione');
      const content = this.transloco.translate('StaiPerEliminareIlProfilo') + " "
        + profile.descrizione + ". " + this.transloco.translate('VuoiConfermare');
      this.dialog.warningThen( title, content, false,
        () => this.profileService.deleteTipologia(profile)
          .subscribe(() => this.gridPublicService.refresh(true))
      );
    }

}
