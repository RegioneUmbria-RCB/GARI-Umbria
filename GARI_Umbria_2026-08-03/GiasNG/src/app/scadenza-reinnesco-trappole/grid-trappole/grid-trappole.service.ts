import {ElementRef, Injectable, Injector, OnDestroy, Signal} from "@angular/core";
import {
  AbstractGridConfigService,
  AgrSelectableSettings,
  CommandsColumnSettings,
  ConfigTemplate, CustomColumnSettings,
  DeletionMode,
  EditingMode,
  ExcelSettings,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType
} from "gias-kendo-grid";
import {PermessiUtenteService} from "../../Service/permessi-utente.service";
import {map, skip, takeUntil} from "rxjs/operators";
import {enum_Menu_Agenda_NG_Mode, enum_Security_Attivita} from "../../Model/TipiEnumerativi";
import {Observable, of} from "rxjs";
import {CELL_TYPES} from "gias-ui-kit";
import {TranslocoService} from "@jsverse/transloco";
import {RowClassArgs} from "@progress/kendo-angular-grid";
import {NessunaSpecieQdC} from "../../Model/CostantiPersonalizzate";
import {AgendaService, CaricaOperazioni} from "../../Service/Agenda/Agenda.service";
import {ObjParametriAgendaService} from "../../Service/obj-parametri-agenda.service";
import {Filters} from "../../menu-agenda/components/utils";
import {DatePipe} from "@angular/common";
import {FiltriTrappole, ScadenzaReinnescoTrappoleService} from "../scadenza-reinnesco-trappole.service";

export class GridTrappoleServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], columns: KendoGridColumn[], model: KendoGridModel) {
    super(model, columns, rows);
  }
}

@Injectable()
export class GridTrappoleService extends AbstractGridConfigService<KendoServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = "GridTrappoleService_Row_ID";
  gridId: string = "GridTrappoleService";
  data: KendoServerResult;
  selectable: AgrSelectableSettings = new AgrSelectableSettings();
  qdcGridRef: ElementRef;
  applicaFiltri: boolean;

  private permessoQdC_W: boolean;

  constructor(
    injector: Injector,
    private permessiUtenteService: PermessiUtenteService,
    private agendaService: AgendaService,
    private translocoService: TranslocoService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private datepipe: DatePipe,
    private scadenzaTrappoleReinnescoService: ScadenzaReinnescoTrappoleService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.scadenzaTrappoleReinnescoService.RicaricaGriglia().pipe(takeUntil(this.signal), skip(1)).subscribe((val) => {
      this.gridPublicService.refresh(true);
    });

    this.permessoQdC_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);

    this.handleCustomizations();
  }

  private handleCustomizations() {
    this.resizable.autoFitColumns = true;
    this.views.enabled = true;

    this.behavior.showDeletionConfirmation = true;

    this.behavior.deletionMode = DeletionMode.HandleSingleRowDeletionOnly;
    this.behavior.excelSettings = new ExcelSettings({ enabled: true });

    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, removeBtn: false });

    this.customColumn = new CustomColumnSettings({
      showColumn: this.permessoQdC_W,
      useCustomColumnHeaderTemplate: false,
      useCustomColumnCellTemplate: true
    });
  }

  read(): Observable<KendoServerResult> {

    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

    let filtriTrappole: FiltriTrappole = this.scadenzaTrappoleReinnescoService.FiltriForm.getRawValue();

    let adjustedVegCod = filtriTrappole.Specie.veg_cod;
    let flag_TerrenoNudo: boolean = false;
    let flag_NessunaSpecieQdC: boolean = false;

    if (adjustedVegCod === (NessunaSpecieQdC).toString()) {
      adjustedVegCod = "0";
      flag_NessunaSpecieQdC = true;
    } else if (adjustedVegCod.startsWith('-') && adjustedVegCod != '-1') {
      adjustedVegCod = adjustedVegCod.replace('-', '');
      flag_TerrenoNudo = true;
    }

    const filters = new Filters({
      TipoGriglia: '2',
      xFiltroAggiuntivo_colturali: '',
      txt_Data1: this.datepipe.transform(filtriTrappole.Da, 'dd/MM/yyyy'),
      txt_Data2: this.datepipe.transform(filtriTrappole.A, 'dd/MM/yyyy'),
      flag_TerrenoNudo: flag_TerrenoNudo,
      sa_cod: filtriTrappole.CentroAziendale.sa_cod,
      veg_cod: adjustedVegCod,
      mode: enum_Menu_Agenda_NG_Mode.Standard,
      ricetta_cod: "0",
      tipoOperazione: [],
      impianti: [],
      flag_NessunaSpecieQdC: flag_NessunaSpecieQdC
    });

    const caricaOp: CaricaOperazioni = {
      filtro: JSON.stringify(filters),
      piva: this.objParametriAgendaService.getObjParamValue().Piva
    };

    return this.agendaService.CaricaGridTrappole(caricaOp).pipe(
      map((data: any) => {

        let table = data.RispostaStringa.map(d=>{
          return {
                  ...d,
                  GridTrappoleService_Row_ID: d["Id_Agenda"] +"_"+ d["Piva"] + "_"+ d["Sa_Cod"]+ "_"+ d["Pro_Cod"],
                  Feromone_Scaduto_Des: d["Feromone_Scaduto"] === 1 ? this.translocoService.translate("Si") : this.translocoService.translate("No"),
                  Feromone_InScadenza_Des: d["Feromone_InScadenza"] === 1 ? this.translocoService.translate("Si") : this.translocoService.translate("No"),
                  Trappola_Reinnescata_Des: d["Trappola_Reinnescata"] === 1 ? this.translocoService.translate("Si") : this.translocoService.translate("No")
                 };
        });

        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

        return new GridTrappoleServerResult(table,
          this.setColumnsGridTrappole(),
          this.setModelGridTrappole()
        );
      }));
  }

  perform(actionType: HttpAction, item: any): Observable<any[]> {
    if (actionType === HttpAction.REMOVE) {
      return null;
    }
    return of();
  }

  setModelGridTrappole() {

    let model: KendoGridModel = {
      GridTrappoleService_Row_ID: { type: CELL_TYPES.STRING },
      Piva: { type: CELL_TYPES.STRING },
      Rag_Soc: { type: CELL_TYPES.STRING },
      Sa_Cod: { type: CELL_TYPES.NUMBER },
      Sa_Nome: { type: CELL_TYPES.STRING },
      Id_Agenda: { type: CELL_TYPES.STRING },
      Raccoglitore_Cod: { type: CELL_TYPES.STRING },
      Des_Lib: { type: CELL_TYPES.STRING },
      Lav_Des: { type: CELL_TYPES.STRING },
      Data_Movimento: { type: CELL_TYPES.DATE },
      Pro_Cod: { type: CELL_TYPES.NUMBER },
      Fr_Des: { type: CELL_TYPES.STRING },
      DurataFeromone: { type: CELL_TYPES.NUMBER },
      Data_Scadenza_Feromone: { type: CELL_TYPES.DATE },
      Feromone_Scaduto: { type: CELL_TYPES.NUMBER },
      Feromone_Scaduto_Des: { type: CELL_TYPES.STRING },
      Feromone_InScadenza: { type: CELL_TYPES.NUMBER },
      Feromone_InScadenza_Des: { type: CELL_TYPES.STRING },
      Trappola_Reinnescata: { type: CELL_TYPES.NUMBER },
      Trappola_Reinnescata_Des: { type: CELL_TYPES.STRING },
      Veg_Des: { type: CELL_TYPES.STRING },
      Id_Des: { type: CELL_TYPES.STRING },
      Cul_Des: { type: CELL_TYPES.STRING },
      Specie_Destinazione: { type: CELL_TYPES.STRING },
      Reinneschi_Des_Lib: { type: CELL_TYPES.STRING }
    };

    return model;
  }

  setColumnsGridTrappole() {

    const columns: Array<KendoGridColumn> = [
      new KendoGridColumn({ field: 'Data_Movimento', title: this.translocoService.translate('Data') }, { resizable: true, filterable: true, editable: false  }),
      new KendoGridColumn({ field: 'Des_Lib', title: this.translocoService.translate('Operazione') }, { resizable: true, filterable: true, editable: false  }),
      new KendoGridColumn({ field: 'Fr_Des', title: this.translocoService.translate('Prodotto') }, { resizable: true, filterable: true, editable: false }),
      new KendoGridColumn({ field: 'Data_Scadenza_Feromone', title: this.translocoService.translate('DataScadenza') }, { resizable: true, filterable: true, editable: false }),
      new KendoGridColumn({ field: 'Feromone_Scaduto_Des', title: this.translocoService.translate('FeromoneScaduto') }, { resizable: true, filterable: true, editable: false }),
      new KendoGridColumn({ field: 'Feromone_InScadenza_Des', title: this.translocoService.translate('FeromoneInScadenza') }, { resizable: true, filterable: true, editable: false }),
      new KendoGridColumn({ field: 'Trappola_Reinnescata_Des', title: this.translocoService.translate('TrappolaReinnescata') }, { resizable: true, filterable: true, editable: false }),
      new KendoGridColumn({ field: 'Sa_Nome', title: this.translocoService.translate('CentroAziendale') }, { resizable: true, filterable: true, editable: false, hidden: false }),
      new KendoGridColumn({ field: 'Specie_Destinazione', title: this.translocoService.translate('SpecieVarieta') }, { resizable: true, filterable: true, editable: false, hidden: true }),
      new KendoGridColumn({ field: 'Id_Agenda', title: this.translocoService.translate('ID') }, { resizable: true, filterable: true, editable: false, hidden: true }),
      new KendoGridColumn({ field: 'Raccoglitore_Cod', title: this.translocoService.translate('CodMultiAttivita') }, { resizable: true, filterable: true, editable: false, hidden: true }),
      new KendoGridColumn({ field: 'Reinneschi_Des_Lib', title: this.translocoService.translate('ReinneschiEffettuati') }, { resizable: true, filterable: true, editable: false })
    ];

    return columns;
  }

  onRowClass = (event: RowClassArgs) => {
    return this.coloraRighe(event.dataItem);
  }

  coloraRighe(row: KendoGridRow) {

    let result: { [k: string]: boolean } = {
      OperazInneschiScaduti: this.IsScaduto(row),
      OperazInneschiInScadenza: this.IsInScadenza(row)
    };
    return result;
  }

  private IsScaduto(row: KendoGridRow): boolean{
    return row["Feromone_Scaduto"] === 1;
  }

  private IsInScadenza(row: KendoGridRow): boolean{
    return row["Feromone_InScadenza"] === 1;
  }

  private CheckDurataFeromone(row: KendoGridRow){
    let Durata_Feromone = (row["DurataFeromone"] as number);

    return  (Durata_Feromone !== null && Durata_Feromone > 0);
  }
}
