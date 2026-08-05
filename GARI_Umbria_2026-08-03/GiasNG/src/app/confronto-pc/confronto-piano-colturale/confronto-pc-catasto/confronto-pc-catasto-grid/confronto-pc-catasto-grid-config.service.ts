import {Injectable, Injector, Renderer2, RendererFactory2} from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import {map, Observable, of, Subject, Subscription, takeUntil} from 'rxjs';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {CatastoKendoModel, CatastoKendoServerResult} from './catasto-grid.model';
import {TranslocoService} from '@jsverse/transloco';
import {ConfrontoCatastoClient, ParametriTipoConfronto} from '../../../../Service/net-core6-api.service';
import {ConfrontoPianoColturaleDataService} from '../../confronto-piano-colturale-data.service';
import {IConfrontoPianoColturale, PianoColturale} from '../../../../Model/confronto-piano-colturale/confronto-piano-colturale';
import {enum_TipoConfrontoCatasto} from '../../../../Model/TipiEnumerativi';
import {ObjParametriAgendaService} from '../../../../Service/obj-parametri-agenda.service';
import {
  AggregateSettings,
  AgrSelectableSettings,
  CommandsColumnSettings
} from 'gias-kendo-grid';
import {catchError} from "rxjs/operators";
import {UtilityFunctions} from "../../../../Utility/UtilityFunctions";
import {RispostaStandard} from '../../../../Service/master.service';
import {ColumnComponent, GridComponent} from '@progress/kendo-angular-grid';

@Injectable()
export class ConfrontoPcCatastoGridConfigService extends AbstractGridConfigService<CatastoKendoServerResult> {
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  gridId: string = 'piano-colturale-grid';
  rowId: string = 'Programmazione_Cod';

  private subs: Subscription;

  aggregates = new AggregateSettings(
    {
      enabled: true,
      descriptors: [
        { field: 'Superficie_Ori', aggregate: 'sum', format: 'n4' },
        { field: 'Superficie_Act', aggregate: 'sum', format: 'n4' },
        { field: 'Superficie_Diff', aggregate: 'sum', format: 'n4' },
      ]
    }
  );

  private signal$: Subject<void> = new Subject<void>();

  private kendoModel: CatastoKendoModel = {
    Prov: new ModelEntry(CELL_TYPES.STRING, false),
    Prov_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Com: new ModelEntry(CELL_TYPES.STRING, false),
    Com_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Sezione: new ModelEntry(CELL_TYPES.STRING, false),
    Foglio: new ModelEntry(CELL_TYPES.STRING, false),
    Numero: new ModelEntry(CELL_TYPES.STRING, false),
    Subalterno: new ModelEntry(CELL_TYPES.STRING, false),
    Veg_cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Cul_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Cul_Des: new ModelEntry(CELL_TYPES.STRING, false),
    id_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Ori: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Act: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Diff: new ModelEntry(CELL_TYPES.NUMBER, false)
  };

  private kendoColumns: KendoGridColumn[] = [];

  private renderer: Renderer2;

  constructor(
    injector: Injector,
    private translocoService: TranslocoService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private confrontoCatastoClientService: ConfrontoCatastoClient,
    private rendererFactory: RendererFactory2,
    private confrontoPianoColturaleDataService: ConfrontoPianoColturaleDataService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();
    this.setSubs();
    this.renderer = this.rendererFactory.createRenderer(null, null);
  }

  read(options?: any): Observable<CatastoKendoServerResult> {
    let plan: PianoColturale = this.confrontoPianoColturaleDataService.pianoColturale;
    if (plan != undefined && plan.Programmazione_Cod > 0) {
      let cPC: IConfrontoPianoColturale = this.confrontoPianoColturaleDataService.confrontoPC;
      let params: ParametriTipoConfronto = {
        tipoConfronto: enum_TipoConfrontoCatasto.Planning_PianoColturale,
        partitaIva: this.objParametriAgendaService.getObjParamValue().Piva,
        programmazioneCod: plan.Programmazione_Cod,
        dataInizio: cPC.validita.inizio,
        dataFine: cPC.validita.fine,
        showCatasto: cPC.considerCatasto,
        showVarieta: cPC.considerVarieta
      };
      return this.confrontoCatastoClientService.confrontoCatastoConfrontoCatasto(params).pipe(
        catchError(e => {
          // console.error(e);
          let r = new RispostaStandard();
          r.RispostaOK = true;
          r.RispostaStringa = JSON.stringify([]);
          return of(r);
        }),
        map(r => {
          let arr:any[] = JSON.parse(r.RispostaStringa);
          arr.forEach((val) => {
            let Superficie_Act = val.Superficie_Act;
            if (Superficie_Act == null) {
              Superficie_Act = 0;
            }
            let Superficie_Ori = val.Superficie_Ori;
            if (Superficie_Ori == null) {
              Superficie_Ori = 0;
            }
            val.Superficie_Diff = parseFloat((Superficie_Act - Superficie_Ori).toFixed(4));
          })
          return new CatastoKendoServerResult(this.kendoModel, this.getKendoColumns(), arr);
        })
      );
    } else {
      return of(new CatastoKendoServerResult(this.kendoModel, this.kendoColumns, []));
    }
  }
  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return of(null);
  }

  private getKendoColumns(): KendoGridColumn[] {
    let cols: KendoGridColumn[] = [];
    if (this.confrontoPianoColturaleDataService.confrontoPC.considerCatasto) {
      cols = [...this.getCatastoColumns()]
    }
    cols = [...cols, ...this.getUtilizzoColumns()];
    if (this.confrontoPianoColturaleDataService.confrontoPC.considerVarieta) {
      cols = [...cols, ...this.getVarietaColumns()];
    }
    return [...cols, ...this.getLastColumns()];
  }

  private getCatastoColumns(): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'Prov_Des', title: this.translocoService.translate('ProvinciaAbbr') },
        { resizable: true, editable: true, width: 80 }
      ),
      new KendoGridColumn(
        { field: 'Com_Des', title: this.translocoService.translate('Comune') },
        { resizable: true, editable: true, width: 150 }
      ),
      new KendoGridColumn(
        { field: 'Sezione', title: this.translocoService.translate('SezioneAbbr') },
        { resizable: true, editable: true, width: 80 }
      ),
      new KendoGridColumn(
        { field: 'Foglio', title: this.translocoService.translate('FoglioAbbr') },
        { resizable: true, editable: true, width: 80 }
      ),
      new KendoGridColumn(
        { field: 'Numero', title: this.translocoService.translate('NumeroAbbr') },
        { resizable: true, editable: true, width: 80 }
      ),
      new KendoGridColumn(
        { field: 'Subalterno', title: this.translocoService.translate('SubalternoAbbr') },
        { resizable: true, editable: true, width: 80 }
      )
    ];
  }

  private getUtilizzoColumns(): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'Veg_Des', title: this.translocoService.translate('Utilizzo') },
        { resizable: true, editable: true, width: 135 }
      )
    ];
  }

  private getVarietaColumns(): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'Cul_Des', title: this.translocoService.translate('Varieta') },
        { resizable: true, editable: true, width: 150 }
      )
    ];
  }

  private getLastColumns(): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'Superficie_Ori', title: this.translocoService.translate('SuperficiePlanningFascicolo') },
        { resizable: true, editable: true, width: 135, numeric: { format:'{0:n4}' } }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Act', title: this.translocoService.translate('SuperficiePianoColturale') },
        { resizable: true, editable: true, width: 135, numeric: { format:'{0:n4}' } }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Diff', title: this.translocoService.translate('DifferenzaHa') },
        { resizable: true, editable: true, width: 100, numeric: { format:'{0:n4}' } }
      )
    ];
  }

  private handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();

    this.views.enabled = true;
    this.behavior.excelSettings.enabled = true;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    this.groups.groupable.enabled = false;
  }

  private setSubs(): void {
    this.confrontoPianoColturaleDataService.loadComparison$.pipe(
      takeUntil(this.signal$)
    ).subscribe(() => {
      this.onChangePianoColturale();
    });
  }

  private onChangePianoColturale(): void {
    this.gridPublicService.refresh(true);
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };

    let a = gridElRef;
    let indexDifferenzaColumn = -1;
    let visibleColumns: ColumnComponent[] = <ColumnComponent[]>grid.columns.filter((c) => !c.hidden);
    let visibleField:string[] = visibleColumns.map((c:any) => { return c.field })

    // se le colonne non sono state riordinate ancora una volta, tutti gli indici sono tutti 0
    let isReorderd: boolean = visibleColumns.filter(c => {
      return c.orderIndex === 0;
    }).length === 1;

    if (isReorderd) {
      indexDifferenzaColumn = visibleColumns.find(c => {
        return c.field == 'Superficie_Diff';
      })?.orderIndex;
    } else {
      indexDifferenzaColumn = visibleField.indexOf('Superficie_Diff');
    }

    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');

    grid.view.forEach((el, index) => {
      const row = visibleRows[index] as HTMLElement;
      if (el.Superficie_Act != null && el.Superficie_Ori == null) {
        this.renderer.addClass(row, 'green');
      } else {
        this.renderer.removeClass(row, 'green');
      }

      if (el.Superficie_Act == null && el.Superficie_Ori != null) {
        this.renderer.addClass(row, 'red');
      } else {
        this.renderer.removeClass(row, 'red');
      }

      for (let i = 0; i < row.children.length; i++) {
        this.renderer.removeClass(row.children[i], 'textGreen');
        this.renderer.removeClass(row.children[i], 'textRed');
      }

      if (indexDifferenzaColumn > -1) {
        if(el.Superficie_Diff > 0 && (el.Superficie_Act != null && el.Superficie_Ori != null)) {
          this.renderer.addClass(row.children[indexDifferenzaColumn], 'textGreen');
          this.renderer.removeClass(row.children[indexDifferenzaColumn], 'textRed');
        } else if (el.Superficie_Diff < 0 && (el.Superficie_Act != null && el.Superficie_Ori != null)) {
          this.renderer.addClass(row.children[indexDifferenzaColumn], 'textRed');
          this.renderer.removeClass(row.children[indexDifferenzaColumn], 'textGreen');
        } else {
          this.renderer.removeClass(row.children[indexDifferenzaColumn], 'textGreen');
          this.renderer.removeClass(row.children[indexDifferenzaColumn], 'textRed');
        }
      }
    });
  }
}
