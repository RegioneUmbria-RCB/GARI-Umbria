import {Injectable, Injector} from '@angular/core';
import {
    EditingMode,
    KendoGridColumn,
    KendoServerResult,
    LoaderType,
    NumericSettings,
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {map, Observable, takeUntil} from 'rxjs';
import {GridRilieviModel, GridRilieviObject} from '../rilievi.model';
import {
    CommandsColumnSettings,
    ExcelSettings,
    PaginationSettings,
    ToolbarSettings
} from 'gias-kendo-grid';
import {QdCRilieviService} from '../../../../service/prodotti/rilievi.service';
import {enum_LAVCOD} from '../../../../../../Model/TipiEnumerativi';
import { enum_TipoControllo } from 'gias-ui-kit';
import {
    DynamicInputComponent
} from 'gias-kendo-grid';


export class GridRilieviServerResult extends KendoServerResult{
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class GridRilieviConfigService extends AbstractGridConfigService<GridRilieviServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Row_Key';
    gridId = 'GridRilievi';

    private percentagesettings: NumericSettings;

    private kendoRows: Array<GridRilieviObject> = [];
    private kendoModel = new GridRilieviModel();
    private kendoColumns: KendoGridColumn[] = [];

    constructor(injector: Injector,
                private rilieviService: QdCRilieviService
    ) {
        super(injector);
        this.handleCustomizatons();
        this.handleEvents();
        this.handleUpdates();
        this.rilieviService.reloadRowsSignal$
          .pipe(takeUntil(this.signal))
          .subscribe(() => this.reloadRows());
    }

    private get lavCodRilievo(): number {
        return +this.rilieviService.Sezione_Rilievo?.value?.Operazione?.primaryKey?.codice;
    }

    read(options?: any): Observable<GridRilieviServerResult> {
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef});
        return this.rilieviService.loadRilieviRows().pipe(map(rows => {
            this.kendoRows = rows;
            this.setColumns();
            this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef});
            return new GridRilieviServerResult(
                this.kendoModel, this.kendoColumns, this.kendoRows
            );
        }));
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        if (actionType === HttpAction.REMOVE) {
            console.log('remove:', items);
        }
        return null;
    }

    private setColumns() {

      let qtaColTitle:string = "";

      switch(this.lavCodRilievo){
          case enum_LAVCOD.FASI_FENOLOGICHE:
              qtaColTitle = 'Data';
              break;
          case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:
              qtaColTitle = 'NumMaxCatturaXTrap';
              break;
          default:
              qtaColTitle = 'qta';
              break;
      }

      this.kendoColumns = [
        new KendoGridColumn({ field:'Impianto_Des', title: this.transloco.translate('Impianto')},{ editable: false ,showHTMLAsString: true }),
        new KendoGridColumn({ field:'Op_Des', title: this.transloco.translate('Operazione')},{ editable: false, hidden: true,showHTMLAsString: true }), //per le visite
        new KendoGridColumn({ field:'Descriz_Des', title: this.transloco.translate('Descrizione')},{ editable: false,showHTMLAsString: true }),
        new KendoGridColumn({ field:'QtaObj', title: this.transloco.translate(qtaColTitle)}, { editable: true, component: DynamicInputComponent,filterable: false})
      ];

      if (this.lavCodRilievo === enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA) {
        // 11/03/2024: Aggiungo colonna con ultima resa registrata
        const col = new KendoGridColumn({ field:'ResaAnagrafica', title: this.transloco.translate('ResaDaUltimoRilievo')},{ editable: false, hidden: false, showHTMLAsString: true, style: { "text-align": "left" } });
        this.kendoColumns.push(col);
      }

      if(this.lavCodRilievo === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE){
        const col = new KendoGridColumn({ field:'Pro_Des', title: this.transloco.translate('Prodotto')},{ editable: false, hidden: false, showHTMLAsString: true});
        this.kendoColumns.push(col);
      }
    }

    private handleCustomizatons() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: true,
        });
        this.cmdColumn.width = 30;
        this.groups.groupable.enabled = false;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.views.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = false; //permessoInserimento;
        this.toolbar.resetChanges = false;
        this.pagination = this.handlePagination();

        this.percentagesettings  = new NumericSettings();
    }

    private handlePagination(): PaginationSettings {
        const result: PaginationSettings = new PaginationSettings();
        result.gridState = {
            sort: [],
            skip: 0,
            group: [],
            take: 10,
            filter: {
                logic: 'and',
                filters: [],
            },
        };
        result.pageable = {
            buttonCount: 4,
            info: true,
            type: 'input',
            pageSizes: [10, 25, 50, 100, {
                text: this.transloco.translate('Tutti'),
                value: "all",
            } as any as number]
        };
        result.navigable = false;
        return result;
    }

    private handleEvents() {
        this.gridPublicService.changeDetected.GiasSubscribe((event) => {
            switch (event['action']) {
                case 'remove':
                    this.rilieviService.removeRilievo(event['dataItem']);
                    break;
                default:
                    break;
            }
        });
    }

    private handleUpdates() {
        this.gridPublicService.formGroup.GiasSubscribe(fg => {
            if (!fg) return;
            fg.valueChanges.GiasSubscribe(newVal => {
                this.rilieviService.updateRilievo(newVal);
            });
        });
    }

    private reloadRows() {
        this.gridPublicService.refresh(true);
    }

}
