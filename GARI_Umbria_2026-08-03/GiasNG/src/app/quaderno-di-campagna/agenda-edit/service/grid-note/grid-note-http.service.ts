import {Injectable, Injector} from '@angular/core';
import {
  CommandsColumnSettings,
  CustomColumnSettings,
  PaginationSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {GridPublicService} from 'gias-kendo-grid';
import {filter, from, map, Observable, of, take, takeUntil, tap} from 'rxjs';
import {QdCService} from '../qdc.service';
import {NoteService} from './note.service';
import {cloneDeep} from 'lodash';
import {GruppoNoteModel, NotaInterventoDdlItem} from '../../componenti/grid-note/note.model';
import {TranslocoService} from '@jsverse/transloco';
import { Tipo_Attivita} from 'gias-ui-kit';

export class GridNoteServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()

export class GridNoteHttpService extends AbstractGridConfigService<GridNoteServerResult>{
    editingMode: EditingMode = EditingMode.IN_CELL;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Nota_Cod';
    gridId = 'NoteGridId';

    GridNoteServerResult: GridNoteServerResult;
    rows: KendoGridRow[] = [];
    rowIndex = 0;

    private selectedRowChanges = [];

    constructor(injector: Injector,
                private noteService: NoteService,
                public gridpublicService: GridPublicService,
                public qdcservice: QdCService,
                private translocoService: TranslocoService,
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);
        this.customizeGrid();
        this.handleChanges();
    }

    perform(actionType: HttpAction, item: any): Observable<any[]> {
        this.qdcservice.AggiornaFormArrayNote();
        return from([]);
    }

    read(): Observable<GridNoteServerResult> {
        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
        return this.setKendoGridData().pipe(
            map(() => {
              if (this.qdcservice.TestataForm.get('Tipo').value === Tipo_Attivita.Ricetta)
                  this.readNoteFromArray();
              this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
              this.qdcservice.isNoteInitialized = true;
              this.qdcservice.AggiornaFormArrayNote(this.rows);
              return new GridNoteServerResult (this.rows, this.columns, this.model);
            }),
            tap(grid => this.GridNoteServerResult = grid)
        );
    }

    private customizeGrid() {
        // Nascondo colonna Azioni con i bottoni di Info, Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
        });
        // this.cmdColumn.title= null;
        this.columnMenu.filterable = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.groups.groupable.enabled = false;
        this.views.enabled = false;

        this.pagination = this.getPaginationSettings();
        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;

        this.behavior.excelSettings.enabled=false;
        this.behavior.pdfSettings.enabled=false;

        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = false;
        this.generalSettings.height = 'auto';

        this.customColumn = new CustomColumnSettings({showColumn: true,useCustomColumnHeaderTemplate: true,showBtn: false});
    }

    private handleChanges() {
        this.refreshOnTestataChanges();
        this.gridpublicService.changeDetected
            .pipe(takeUntil(this.signal), tap((event: any) => console.log('event!::',event)))
            .subscribe(() => this.updateSelected());
    }

    private refreshOnTestataChanges() {
      let prevSpecie = this.qdcservice.TestataForm.get('Specie').value;
      let prevOps = this.qdcservice.TestataForm.get("Operazioni").value;
      this.qdcservice.TestataForm.get('Specie').valueChanges
        .pipe(
          takeUntil(this.signal),
          filter(s => prevSpecie.codice !== s.codice),
          tap(s => prevSpecie = s)
        ).subscribe(() => this.gridpublicService.refresh(true));
      this.qdcservice.TestataForm.get("Operazioni").valueChanges
        .pipe(
          takeUntil(this.signal),
          filter(ops => {
            const isSame = JSON.stringify(prevOps) === JSON.stringify(ops);
            prevOps = ops;
            return !isSame;
          }),
        ).subscribe(() => this.gridpublicService.refresh(true));
    }

    private updateSelected() {
      const curr: GruppoNoteModel = this.gridPublicService.formGroup?.getValue()?.value;
      const rows = this.gridPublicService?.getValue()?.data?.rows as GruppoNoteModel[];
      let group: GruppoNoteModel = rows.find((gr: GruppoNoteModel) => gr.id === curr.id)
      if (group) {
        group.Nota_Cod = curr.Nota_Cod;
        group.Nota_Des = curr.Nota_Des;
        this.noteService.isUsingPresets = false;
        this.qdcservice.AggiornaFormArrayNote();
      }
    }

    /** Imposta dati in righe, layout colonne, ddl, modello oggetti tabella  */
    private setKendoGridData() {
        this.columns = [
            new KendoGridColumn({ field: 'Gruppo_Des', title: this.translocoService.translate('Gruppo')},{editable: false}),
            new KendoGridColumn({ field: 'Nota_Cod', title: this.translocoService.translate('NoteCliccare')},{editable: this.qdcservice.abilitaGrid})
        ];
        this.configureDDL();
        this.model = {
            id: new ModelEntry(CELL_TYPES.STRING),
            Gruppo_Cod: new ModelEntry(CELL_TYPES.STRING, false),
            Gruppo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Nota_Cod: new ModelEntry(CELL_TYPES.MULTI_DROPDOWNLIST, false),
            Nota_Des: new ModelEntry(CELL_TYPES.STRING, false),
            isRadio: new ModelEntry(CELL_TYPES.BOOLEAN)
        }
        return this.noteService.caricaNoteIniziali(this.qdcservice.NoteFormArray.getRawValue())
            .pipe(take(1), tap(rows => this.rows = rows))
    }

    private configureDDL() {
        let col = this.columns.find(s => s.field === 'Nota_Cod');
        let data = [];
        col.ddl = new DropdownListWithForm('id', 'Nota_Cod', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.id = 'id';
        col.ddl.formControlValue = 'descrizione';
        col.ddl.loadOnEdit = true;
        col.ddl.descriptionField = 'Nota_Des';
        col.ddl.loadFunction = this.loadNotes.bind(this);
        col.ddl.defaultOpen = true;

        //this.handleRadio();
    }

    private loadNotes(row: KendoGridRow): Observable<any> {
        this.selectedRowChanges.push(cloneDeep(row));
        let p = this.noteService.getLeggiNote();
        return this.noteService.leggiNote(row, p);
    }

    /** Note con campo isRadio: true possono avere al massimo un valore
     Se vengono selezionati più valori, solo l'ultimo rimane */
    private handleRadio() {
        this.gridpublicService.changeDetected.GiasSubscribe( e => {
            let data = e.dataItem;
            while (data.isRadio && data.Nota_Cod.length > 1) {
                let change = this.selectedRowChanges.splice(0,1).at(0);
                let descs: string[] = change.Nota_Des;

                let index = data.Nota_Des.findIndex( el =>  descs.includes(el) );
                if (index > -1) {
                    // le descrizioni appaiono in ordine alfabetico
                    // i codici in base all'ordine con cui sono stati selezionati
                    data.Nota_Des.splice(index,1)
                    data.Nota_Cod.splice(0,1);
                }
            }
            this.selectedRowChanges.splice(0);
        })
    }

    private readNoteFromArray() {
        if (!this.rows) this.rows = [];

        let note: NotaInterventoDdlItem[] = this.qdcservice.NoteFormArray.value;
        if (!note) return;

        for (let nota of note) {
            let grp = this.rows.find((r: GruppoNoteModel) =>
                r.Gruppo_Cod.includes(nota.NotaGruppo_Cod)) as GruppoNoteModel;
            if (grp) {
                grp.Nota_Cod.push(nota.id);
                grp.Nota_Des.push(nota.descrizione);
            }
        }
    }

    private getPaginationSettings(): PaginationSettings {
      const result: PaginationSettings = new PaginationSettings();
      result.take(100);
      result.pageable = false;
      result.navigable = false;
      return result;
    }

}
