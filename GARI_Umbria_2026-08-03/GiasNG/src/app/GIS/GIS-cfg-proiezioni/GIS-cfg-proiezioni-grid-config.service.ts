import { Injectable, Injector } from '@angular/core';
import {forkJoin, map, Observable, of, share, switchMap, takeUntil, tap} from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  CommandsColumnSettings,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { AlgoritmoProiezione, ConfigurazioneProiezione, GisClient, MisuraPerAvversitaAnagrafica, ProiezioneLayer, TipologiaLayer } from 'app/Service/api.service';
import {GridCommandItem} from '../../menu-agenda/components/utils';
import {GISCfgProiezioniDataService} from './gis-cfg-proiezioni-data.service';
import {GisCfgProiezioniPermissionsService} from './GIS-cfg-proiezioni-permissions/GIS-cfg-proiezioni-permissions.service';
import {GISCfgProiezioniService} from './gis-cfg-proiezioni.service';
import { LayerService } from '../services/layer.service';

export class GISCfgProiezioniResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISCfgProiezioniGridModel extends KendoGridModel {
  ConfigurazioneProiezione_Cod: ModelEntry;
  ConfigurazioneProiezione_Des: ModelEntry;
  ConfigurazioneProiezione_GUID: ModelEntry;
  AlgoritmoProiezione_Des: ModelEntry;
  AttivoTuttiLayer: ModelEntry;
  canActivate: ModelEntry;
  Layer1_Des: ModelEntry;
  Layer2_Des: ModelEntry;
  LayerRisultato_Des: ModelEntry;
}

interface ConfigurazioneProiezioneGridModel {
  AlgoritmoProiezione_Cod: number;
  AlgoritmoProiezione_Des: string;
  ConfigurazioneProiezione_Cod: number;
  ConfigurazioneProiezione_Des: string;
  ConfigurazioneProiezione_GUID: string;
  AttivoTuttiLayer: boolean;
  canActivate: boolean;
  canEditCfg: boolean;
  cfg: string;
  Layer1_Des: string;
  Layer2_Des: string;
  LayerRisultato_Des: string;
  Layer1: ProiezioneLayer;
  Layer2: ProiezioneLayer;
  LayerRisultato: ProiezioneLayer;
}

@Injectable()
export class GISCfgProiezioniGridConfig extends AbstractGridConfigService<GISCfgProiezioniResult> {
  gridId = 'GISCfgProiezioniGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'ConfigurazioneProiezione_Cod';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'ConfigurazioneProiezione_Cod', title: ''},
      {hidden: true}
    ),
    new KendoGridColumn(
      {field: 'ConfigurazioneProiezione_Des', title: this.transloco.translate('DescrizioneConfigurazione')},
      {resizable: true, filterable: true, editable: false, width: 200},
    ),
    new KendoGridColumn(
      {field: 'ConfigurazioneProiezione_GUID', title: ''},
      {hidden: true}
    ),
    new KendoGridColumn(
      {field: 'AlgoritmoProiezione_Des', title: this.transloco.translate('Algoritmo')},
      {resizable: true, filterable: true, editable: false, width: 200},
    ),
    new KendoGridColumn(
      {field: 'AttivoTuttiLayer', title: this.transloco.translate('AttivoTuttiLayer')},
      {resizable: true, filterable: true, editable: false, width: 50},
    ),
    new KendoGridColumn(
      {field: 'Layer1_Des', title: this.transloco.translate('DescrizioneLayer1')},
      {resizable: true, filterable: true, editable: true, width: 200},
    ),
    new KendoGridColumn(
      {field: 'Layer2_Des', title: this.transloco.translate('DescrizioneLayer2')},
      {resizable: true, filterable: true, editable: true, width: 200},
    ),
    new KendoGridColumn(
      {field: 'LayerRisultato_Des', title: this.transloco.translate('DescrizioneLayerRisultato')},
      {resizable: true, filterable: true, editable: true, width: 200}
    )
  ];

  GISCfgProiezioniGridModel: GISCfgProiezioniGridModel = {
    ConfigurazioneProiezione_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    ConfigurazioneProiezione_Des: new ModelEntry(CELL_TYPES.STRING),
    ConfigurazioneProiezione_GUID: new ModelEntry(CELL_TYPES.STRING),
    AlgoritmoProiezione_Des: new ModelEntry(CELL_TYPES.STRING),
    AttivoTuttiLayer: new ModelEntry(CELL_TYPES.BOOLEAN),
    canActivate: new ModelEntry(CELL_TYPES.BOOLEAN),
    canEditCfg: new ModelEntry(CELL_TYPES.BOOLEAN),
    Layer1_Des: new ModelEntry(CELL_TYPES.STRING),
    Layer2_Des: new ModelEntry(CELL_TYPES.STRING),
    LayerRisultato_Des: new ModelEntry(CELL_TYPES.STRING),
  };

  private algorithms$: Observable<AlgoritmoProiezione[]>;
  private layers$: Observable<TipologiaLayer[]>;

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private gisClient: GisClient,
    private gisCfgProiezioniDataService: GISCfgProiezioniDataService,
    private gisCfgProiezioniPermissionsService: GisCfgProiezioniPermissionsService,
    private gisCfgProiezioniService: GISCfgProiezioniService,
    private layerService: LayerService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.handleCustomization();
    this.addVariableCommandBtns();
    this.handleCommands();

    this.algorithms$ = this.gisClient
      .gisLeggiAlgoritmiProiezione()
      .pipe(
        map(algorithms => algorithms.RispostaStringa.Algoritmi),
        share()
      );

    this.layers$ = this.layerService
      .readLayersFromBackend({Layer_Selezionato: '1', leggiLayerNonVisibili: true})
      .pipe(
        map(layers => layers.RispostaStringa.ListaTipologieLayer),
        share()
      );
  }

  public read(): Observable<GISCfgProiezioniResult> {
    this.isLoading(true);
    return this.gisClient.gisLeggiConfigurazioniProiezione()
      .pipe(
        switchMap(response => forkJoin([of(response), this.algorithms$, this.layers$])),
        map(([response, algorithms, layers]) => this.mapResponse(response.RispostaStringa.elencoConfigurazioniProiezione, algorithms, layers)),
        map(data => new GISCfgProiezioniResult(data, this.columns, this.GISCfgProiezioniGridModel)),
        tap(() => this.isLoading(false)),
        share()
      );
  }

  public perform(actionType: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    return of(rows);
  }

  private mapResponse(response: ConfigurazioneProiezione[], algorithms: AlgoritmoProiezione[], layers: TipologiaLayer[]): ConfigurazioneProiezioneGridModel[] {
    const result: ConfigurazioneProiezioneGridModel[] = [];
    for (const el of response) {
      result.push({
        AlgoritmoProiezione_Cod: el.AlgoritmoProiezione_Cod,
        AlgoritmoProiezione_Des: algorithms.find(x => x.Algoritmo_Cod == el.AlgoritmoProiezione_Cod)?.Algoritmo_Des,
        ConfigurazioneProiezione_Cod: el.ConfigurazioneProiezione_Cod,
        ConfigurazioneProiezione_Des: el.ConfigurazioneProiezione_Des,
        ConfigurazioneProiezione_GUID: el.ConfigurazioneProiezione_GUID,
        AttivoTuttiLayer: el.AttivoTuttiLayer,
        canActivate: el.canActivate,
        canEditCfg: el.canEditCfg,
        cfg: el.cfg,
        Layer1_Des: layers.find(x => +x.id == el.Layer1.LayerElementiGrafici_Cod)?.nome ?? '-',
        Layer2_Des: layers.find(x => +x.id == el.Layer2.LayerElementiGrafici_Cod)?.nome ?? '-',
        LayerRisultato_Des: layers.find(x => +x.id == el.LayerRisultato.LayerElementiGrafici_Cod)?.nome ?? '-',
        Layer1: el.Layer1,
        Layer2: el.Layer2,
        LayerRisultato: el.LayerRisultato
      });
    }

    return result;
  }

  private handleCustomization(): void {
    this.toolbar = new ToolbarSettings();
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.toolbar.customToolbar = true;

    this.setCustomCommandColumn();
  }

  private setCustomCommandColumn(): void {
    if(this.cmdDropDown.cmdList.findIndex(v => v.action === EnumCfgProiezioniActions.EDIT) === -1){
      this.cmdDropDown.addCommand(new GridCommandItem(
        this.transloco.translate('Modifica'),
        EnumCfgProiezioniActions.EDIT,
        'fa fa-edit color-std'
      ));
    }

    if(this.cmdDropDown.cmdList.findIndex(v => v.action === EnumCfgProiezioniActions.PERMISSIONS) === -1){
      this.cmdDropDown.addCommand(new GridCommandItem(
        this.transloco.translate('ModificaPermessi'),
        EnumCfgProiezioniActions.PERMISSIONS,
        'fa fa-user color-std'
      ));
    }
  }

  private addVariableCommandBtns(): void {
    this.gridPublicService.openCommands.pipe(
      takeUntil(this.signal)
    ).subscribe(e => {
      this.setCustomCommandColumn();

      if (e == undefined) {
        return;
      }

      if (e.canActivate) {
        if(this.cmdDropDown.cmdList.findIndex(v=>v.action === EnumCfgProiezioniActions.ACTIVATION) === -1) {
          const cmd: GridCommandItem = new GridCommandItem(
            this.transloco.translate('AttivaAlgoritmo'),
            EnumCfgProiezioniActions.ACTIVATION,
            'fa fa-toggle-on color-std'
          );
          this.cmdDropDown.addCommand(cmd);
        }
      } else {
        this.cmdDropDown.removeCommand(EnumCfgProiezioniActions.ACTIVATION);
      }

      if (e.canEditCfg) {
        if(this.cmdDropDown.cmdList.findIndex(v=> v.action === EnumCfgProiezioniActions.CONFIGURATION) === -1) {
          if(this.cmdDropDown.cmdList.findIndex(v => v.action === EnumCfgProiezioniActions.CONFIGURATION) === -1){
            this.cmdDropDown.addCommand(new GridCommandItem(
              this.transloco.translate('ModificaCfgAlgoritmo'),
              EnumCfgProiezioniActions.CONFIGURATION,
              'fa fa-list-ul color-std'
            ));
          }
        }
      } else {
        this.cmdDropDown.removeCommand(EnumCfgProiezioniActions.CONFIGURATION);
      }
    });
  }

  private handleCommands(): void {
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal)
    ).subscribe(e => {
      if (e == undefined) {
        return;
      }

      switch (e.command.action) {
        case EnumCfgProiezioniActions.EDIT:
          this.edit(e.dataItem);
          break;
        case EnumCfgProiezioniActions.PERMISSIONS:
          this.editPermissions(e.dataItem);
          break;
        case EnumCfgProiezioniActions.ACTIVATION:
          this.activateAlgorithm(e.dataItem);
          break;
        case EnumCfgProiezioniActions.CONFIGURATION:
          this.editAlgorithmConfigurationCfg(e.dataItem);
          break;
        default:
          console.error('Operation not yet implemented');
      }
    });
  }

  edit(configuration: ConfigurazioneProiezione): void {
    this.gisCfgProiezioniDataService.selectedConfiguration = configuration;
    this.gisCfgProiezioniDataService.openConfigurationEditDialog = true;
  }

  editPermissions(configuration: ConfigurazioneProiezione): void {
    this.gisCfgProiezioniPermissionsService.resetAuths();
    this.gisCfgProiezioniDataService.selectedConfiguration = configuration;
    this.gisCfgProiezioniDataService.openPermissionDialog = true;
  }

  activateAlgorithm(configuration: ConfigurazioneProiezione): void {
    this.gisCfgProiezioniService.activateAlgorithm(configuration);
  }

  private editAlgorithmConfigurationCfg(configuration: ConfigurazioneProiezione): void {
    this.gisCfgProiezioniDataService.selectedConfiguration = configuration;
    this.gisCfgProiezioniDataService.openCfgDialog = true;
  }
}

export enum EnumCfgProiezioniActions {
  EDIT,
  PERMISSIONS,
  ACTIVATION,
  CONFIGURATION
}
