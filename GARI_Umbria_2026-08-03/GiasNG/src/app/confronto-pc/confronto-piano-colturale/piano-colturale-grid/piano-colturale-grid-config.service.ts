import {Injectable, Injector} from '@angular/core';
import { EditingMode, KendoGridColumn, LoaderType, ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {map, Observable, of, Subject, takeUntil} from 'rxjs';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {PianoColturaleKendoModel, PianoColturaleKendoServerResult} from './piano-colturale-grid.model';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../../Model/CostantiPersonalizzate';
import {TranslocoService} from '@jsverse/transloco';
import {ConfrontoPianoColturaleDataService} from '../confronto-piano-colturale-data.service';
import {PianoColturaleClient} from '../../../Service/net-core6-api.service';
import {ObjParametriAgendaService} from '../../../Service/obj-parametri-agenda.service';
import {
  AgrSelectableSettings,
  CommandsColumnSettings
} from 'gias-kendo-grid';
import {PianoColturale} from '../../../Model/confronto-piano-colturale/confronto-piano-colturale';

@Injectable()
export class PianoColturaleGridConfigService extends AbstractGridConfigService<PianoColturaleKendoServerResult> {
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  gridId: string = 'piano-colturale-grid';
  rowId: string = 'Programmazione_Cod';

  public signal$: Subject<void> = new Subject<void>();

  private kendoModel: PianoColturaleKendoModel = {
    Piva: new ModelEntry(CELL_TYPES.STRING, false),
    Programmazione_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Programmazione_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Note: new ModelEntry(CELL_TYPES.STRING, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATE, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATE, false),
    Username_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Username_Modifica: new ModelEntry(CELL_TYPES.STRING, false)
  };

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Programmazione_Des', title: this.translocoService.translate('Programmazione') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Note', title: this.translocoService.translate('Note') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
      { resizable: true, date: { defaultValue: AGRODATAINIZIO }, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
      { resizable: true, date: { defaultValue: AGRODATAFINE }, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') },
      { resizable: true, date: { defaultValue: AGRODATAINIZIO }, editable: false, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') },
      { resizable: true, date: { defaultValue: AGRODATAFINE }, editable: false, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Username_Creazione', title: this.translocoService.translate('UsernameCreazione') },
      { resizable: true, date: { defaultValue: AGRODATAINIZIO }, editable: false, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Username_Modifica', title: this.translocoService.translate('UsernameModifica') },
      { resizable: true, date: { defaultValue: AGRODATAFINE }, editable: false, width: 135 }
    )
  ];

  constructor(
    injector: Injector,
    private translocoService: TranslocoService,
    private objParametriagendaService: ObjParametriAgendaService,
    private pianoColturaleClientService: PianoColturaleClient,
    private confrontoPianoColturaleDataService: ConfrontoPianoColturaleDataService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();
    this.setSubs();
  }

  read(options?: any): Observable<PianoColturaleKendoServerResult> {
    let piva: string = this.objParametriagendaService.getObjParamValue().Piva;
    return this.pianoColturaleClientService.pianoColturalePlannings(piva).pipe(
      map(r => {
        return new PianoColturaleKendoServerResult(this.kendoModel, this.kendoColumns, JSON.parse(r.RispostaStringa));
      })
    );
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return of(null);
  }

  private handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.mode = 'single';

    this.views.enabled = false;
    this.behavior.excelSettings.enabled = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    this.groups.groupable.enabled = false;
  }

  private setSubs(): void {
    this.gridPublicService.selection.getSelectedValue.pipe(
      takeUntil(this.signal$)
    ).subscribe((event) => {
      if (event?.value.selectedRows != undefined && event?.value.selectedRows.length > 0) {
        this.confrontoPianoColturaleDataService.pianoColturale = this.mapDataItemToPianoColturale(event?.value.selectedRows[0].dataItem);
      } else {
        this.confrontoPianoColturaleDataService.pianoColturale = undefined;
      }
    });
  }

  private mapDataItemToPianoColturale(dataItem: any): PianoColturale {
    return {
      Piva: dataItem.Piva,
      Programmazione_Cod: dataItem.Programmazione_Cod,
      Programmazione_Des: dataItem.Programmazione_Des,
      Note: dataItem.Note,
      Validita_Inizio: dataItem.Validita_Inizio,
      Validita_Fine: dataItem.Validita_Fine
    }
  }
}
