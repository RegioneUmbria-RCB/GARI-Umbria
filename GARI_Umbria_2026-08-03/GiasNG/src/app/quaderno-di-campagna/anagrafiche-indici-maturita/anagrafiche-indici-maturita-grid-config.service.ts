import { Injectable, Injector } from "@angular/core";
import { AgendaClient, LeggiMisuraPerIndiciMaturitaAnagrafiche_IN, MisuraPerIndiciMaturita_In, MisuraPerIndiciMaturitaAnagrafica, RispostaStandard } from "app/Service/api.service";
import { GiasMessageService } from "app/Service/gias-message.service";
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Observable, of, map, from, Subject, switchMap, catchError, startWith, tap, debounceTime, share } from "rxjs";

export class AnagraficheIndiciMaturitaResult extends KendoServerResult{
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class AnagraficheIndiciMaturitaGridModel extends KendoGridModel {
    Ind_Mat_Cod: ModelEntry;
    Ind_Mat_Des: ModelEntry;
    DescrizioneMisura: ModelEntry;
    Udm_Cod: ModelEntry;
    Udm_Des: ModelEntry;
    Anag_Cod: ModelEntry;
    Anag_Des: ModelEntry;
    Anag_Valore: ModelEntry;
}

@Injectable()
export class AnagraficheIndiciMaturitaGridConfig extends AbstractGridConfigService<AnagraficheIndiciMaturitaResult>{
    gridId = 'AnagraficheIndiciMaturitaGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;
    rowId = 'CodiceAnagrafica';
    private reloadSubject = new Subject<void>();
    private read$: Observable<AnagraficheIndiciMaturitaResult> | null = null;

    constructor(injector: Injector,
        private giasMessageService: GiasMessageService,
        public gridpublicService: GridPublicService,
        private agendaClient: AgendaClient
    ){
        super(injector, ConfigTemplate.DefaultTemplate);
        this.toolbar = new ToolbarSettings(true);
        this.resizable = new ResizableSettings(true, true);
        this.cmdColumn = new CommandsColumnSettings({
          editBtn: true,
          infoBtn: false,
          removeBtn: true,
          onDisableRemoveBtn: (data: MisuraPerIndiciMaturitaAnagrafica) => !data.Cancellabile,
          onDisableEditBtn: (data: MisuraPerIndiciMaturitaAnagrafica) => !data.Modificabile
        });

        this.cmdColumn['widthSet'] = 25;
        this.behavior.excelSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.groups.groupable.enabled = false;
        this.views.enabled = false;
        this.generalSettings.performOnEdit = true;
        
        this.handleDropdowns();
    }

    get reload$(): Observable<void> {
        return this.reloadSubject.asObservable();
      }
    
      public nextReload(): void {
        this.reloadSubject.next();
      }

    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'CodiceCompostoMisura', title: this.transloco.translate('IndiceMaturita')},
            { resizable: true, filterable: true, editable: true, width: 200, disabledRule: (rowData) => rowData?.Anag_Cod != null }
        ),
        new KendoGridColumn(
            { field: 'Anag_Des', title: this.transloco.translate('RilieviAvversitaDescrizione')},
            { resizable: true, filterable: true, editable: true, width: 200 }
        ),
        new KendoGridColumn(
            { field: 'Anag_Valore', title: this.transloco.translate('RilieviAvversitaValoreAnagrafica')},
            { resizable: true, filterable: true, editable: true, width: 200 , numeric: new NumericSettings({ format: '#', decimals: 0 })}
        )
    ]

    anagraficheIndiciMaturitaGridModel: KendoGridModel = {
        Ind_Mat_Cod: {
            editable: false,
            type: CELL_TYPES.DROPDOWNLIST
        },
        Ind_Mat_Des: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        CodiceCompostoMisura: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
        },
        DescrizioneMisura: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Udm_Cod: {
            editable: false,
            type: CELL_TYPES.DROPDOWNLIST
        },
        Udm_Des: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        Anag_Cod: {
            editable: false,
            type: CELL_TYPES.NUMBER
        },
        Anag_Des: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Anag_Valore: {
            editable: true,
            type: CELL_TYPES.NUMBER
        }
    }

    handleDropdowns(){
        const colCodiceCompostoMisura = this.columns.find(x => x.field === 'CodiceCompostoMisura');
        const dati: DropdownListItem[] = [];
        colCodiceCompostoMisura.ddl = new DropdownListWithForm('codice', 'CodiceCompostoMisura', 'descrizione', dati);
        colCodiceCompostoMisura.ddl.valuePrimitive = true;
        colCodiceCompostoMisura.ddl.descriptionField = 'DescrizioneMisura';
        colCodiceCompostoMisura.ddl.loadOnEdit = true;
        colCodiceCompostoMisura.ddl.loadFunction = this.caricaDropDownMisura.bind(this);
    }

    private caricaDropDownMisura(){

        return from(this.agendaClient.agendaLeggiMisureIndiciMaturita()).pipe(map((r:any) =>{
            const listaMisure = r.RispostaStringa.ListaMisure;
            return listaMisure.map(t => {
                return {codice: t.Codice, descrizione: t.Descrizione}
            });
        }));
    }

    read(options?: any): Observable<AnagraficheIndiciMaturitaResult> {

        if (this.read$ != null) {
            return this.read$;
        }

        this.read$ = this.reload$.pipe(
            startWith(null),
            tap(() => this.isLoading(true)),
            debounceTime(100),
            switchMap(() => {
                const leggiMisuraPerIndiciMaturitaAnagrafiche_IN: LeggiMisuraPerIndiciMaturitaAnagrafiche_IN = {
                    Ind_Mat_Cod: -1,
                    Udm_Cod: -1,
                    Anag_Cod: 0
                }

                return this.agendaClient.agendaLeggiMisureIndiciMaturitaAnagrafiche2(leggiMisuraPerIndiciMaturitaAnagrafiche_IN);
            }),
            switchMap(data => of(new AnagraficheIndiciMaturitaResult(data.RispostaStringa.ListaMisure, this.columns, this.anagraficheIndiciMaturitaGridModel))),
            tap(() => this.isLoading(false)),
            share()
          );
    
        return this.read$;

    }

    perform(actionType: HttpAction, row: MisuraPerIndiciMaturitaAnagrafica): Observable<KendoGridRow[]> {
        of([row])
         .pipe(
            map((rows) => this.getPayload(rows, actionType)),
            switchMap(payload => this.agendaClient.agendaMisurePerIndiciMaturita(payload)),
            catchError(res => of({ Errore: res.toString() } as RispostaStandard))
         ).subscribe({
            next: res => {
              this.nextReload();
              if (res?.RispostaStringa != null && res.RispostaStringa != "") {
                this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
                return;
              }
      
              this.giasMessageService.errorMessage(res.Errore, false, false);
            },
            error: error => {
              this.nextReload();
              this.giasMessageService.errorMessage(error.Errore, false);
            }
          });

         return of([row]);
    }

    private getPayload(rows: MisuraPerIndiciMaturitaAnagrafica[], actionType: HttpAction): MisuraPerIndiciMaturita_In {
        if (actionType === HttpAction.CREATE) {
            for (const row of rows) {
                row.Ind_Mat_Cod = +row.CodiceCompostoMisura.split("_")[0];
                row.Udm_Cod = +row.CodiceCompostoMisura.split("_")[1];
                row.Anag_Cod = 0;
            }
        }

        let payload: MisuraPerIndiciMaturita_In;
        switch (actionType) {
            case HttpAction.CREATE:
                payload = { AnagraficaInsert: rows };
                break;
            case HttpAction.UPDATE:
                payload = { AnagraficaUpdate: rows };
                break;
            case HttpAction.REMOVE:
                payload = { AnagraficaDelete: rows };
                break;
        }

        return payload;
    }
}