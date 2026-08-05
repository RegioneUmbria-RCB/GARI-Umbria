import { Inject, Injectable, Injector, Renderer2, RendererFactory2 } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of } from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType, ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import {catchError, map} from 'rxjs/operators';
import { CellClickEvent, CellCloseEvent, RowArgs, SelectionEvent } from '@progress/kendo-angular-grid';
import { CatastoAppezzamento_Extended, DatiCatastaliService, MacrousiCatastoAppezzamento } from './dati-catastali.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { PKParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { AggregateSettings } from 'gias-kendo-grid';
import {CaricaCatastoSettings, ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';

export class DatiCatastaliServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class DatiCatastaliHttpService extends AbstractGridConfigService<DatiCatastaliServerResult>{
  gridId: string = 'DatiCatastaliHttpService';
  rowId: string = 'chiave';

  private renderer: Renderer2;
  private objParametriAgenda: ObjParametriAgenda;

  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;
  lastLoadedSettings: CaricaCatastoSettings = null;

  aggregates: AggregateSettings = new AggregateSettings({
    enabled: true,
    descriptors: [
      {field: 'SuperficieImpiegata', aggregate: 'sum', format: 'n4'}
    ]
  });

  private DatiCatastaliObs: any[];

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'CodiceIstat_Provincia', title: this.translocoLoc.translate('IstatProvinciaAbbr') },
      { hidden: true, resizable: true, editable: false, width: 130 }
    ),
    new KendoGridColumn(
      { field: 'CodiceIstat_Comune', title: this.translocoLoc.translate('IstatComuneAbbr') },
      { hidden: true, resizable: true, editable: false, width: 130 })
    ,
    new KendoGridColumn(
      { field: 'Part_Cod', title: this.translocoLoc.translate('CodiceParticella') },
      { hidden: true, resizable: true, numeric: { defaultValue: 0 }, editable: false, width: 130 }
    ),
    new KendoGridColumn(
      { field: 'Progressivo', title: this.translocoLoc.translate('Progressivo') },
      { hidden: true, title: this.translocoLoc.translate('Progressivo'), resizable: true, numeric: { defaultValue: 0 }, editable: false, width: 130 }
    ),
    new KendoGridColumn(
      { field: 'SuperficieImpiegata', title: this.translocoLoc.translate('SuperficieImpiegata') },
      { resizable: true, numeric: { defaultValue: 0 }, width: 140, editable: true }
    ),
    new KendoGridColumn(
      { field: 'Provincia', title: this.translocoLoc.translate('Provincia') },
      { resizable: true, editable: false, width: 110 }
    ),
    new KendoGridColumn(
      { field: 'Comune', title: this.translocoLoc.translate('Comune') },
      { resizable: true, editable: false, width: 200 }
    ),
    new KendoGridColumn(
      { field: 'Sezione', title: this.translocoLoc.translate('Sezione') },
      { resizable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Foglio', title: this.translocoLoc.translate('Foglio') },
      { resizable: true, editable: false, numeric: { defaultValue: 0, multiCheckFiltering: true }, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Numero', title: this.translocoLoc.translate('Numero') },
      { resizable: true, numeric: { defaultValue: 0, multiCheckFiltering: true }, width: 100, editable: false }
    ),
    new KendoGridColumn(
      { field: 'Subalterno', title: this.translocoLoc.translate('Subalterno') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'ZVN', title: this.translocoLoc.translate('ZVN') },
      { resizable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'SuperficieCondottaDisponibile', title: this.translocoLoc.translate('SuperficieCondottaDisponibileAbbr') },
      { resizable: true, numeric: { defaultValue: 0 }, editable: false, width: 200 }
    ),
    new KendoGridColumn(
      { field: 'SuperficieLorda', title: this.translocoLoc.translate('SuperficieCatastaleAbbr') },
      { resizable: true, numeric: { defaultValue: 0 }, editable: false, width: 170 }
    ),
    new KendoGridColumn(
      { field: 'Superficie', title: this.translocoLoc.translate('SuperficieCondottaAbbr') },
      { resizable: true, numeric: { defaultValue: 0 }, editable: false, width: 150 }
    )
  ];

  private kendoModel: KendoGridModel = {
    ChkSelezionaParticella: new ModelEntry(CELL_TYPES.STRING, false),
    CodiceIstat_Comune: new ModelEntry(CELL_TYPES.STRING, false),
    CodiceIstat_Provincia: new ModelEntry(CELL_TYPES.STRING, false),
    Comune: new ModelEntry(CELL_TYPES.STRING, false),
    Foglio: new ModelEntry(CELL_TYPES.NUMBER, false),
    Numero: new ModelEntry(CELL_TYPES.NUMBER, false),
    Part_Cod: new ModelEntry(CELL_TYPES.STRING, false),
    Progressivo: new ModelEntry(CELL_TYPES.STRING, false),
    Provincia: new ModelEntry(CELL_TYPES.STRING, false),
    Sezione: new ModelEntry(CELL_TYPES.STRING, false),
    Subalterno: new ModelEntry(CELL_TYPES.STRING, false),
    ZVN: new ModelEntry(CELL_TYPES.STRING, false),
    Superficie: new ModelEntry(CELL_TYPES.STRING, false),
    SuperficieCondottaDisponibile: new ModelEntry(CELL_TYPES.NUMBER, false),
    SuperficieImpiegata: new ModelEntry(CELL_TYPES.NUMBER, false),
    SuperficieLorda: new ModelEntry(CELL_TYPES.STRING, false),
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
  };

  constructor(
    injector: Injector,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private ObjParametriAgendaService: ObjParametriAgendaService,
    private daticatastali: DatiCatastaliService,
    private giasMessageService: GiasMessageService,
    private translocoLoc: TranslocoService,
    private rendererFactory: RendererFactory2
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.renderer = this.rendererFactory.createRenderer(null, null);

    this.lastLoadedSettings = null;
    this.objParametriAgenda = this.ObjParametriAgendaService.getObjParamValue();

    this.daticatastali.caricaCatastoSettings.GiasSubscribe(val => {
      if (this.catastoSettingsDifferences(this.lastLoadedSettings, val)) {
        this.DatiCatastaliObs = null;
        this.gridPublicService.refresh(true);
      }
    });

    this.handleCustomizations();
  }

  public SelezionaRigha = (e: RowArgs, component: GiasKendoGridComponent) => {
    return (
      this.daticatastali.ControllaSeSelezionareLaRiga(e.dataItem, this.daticatastali.getCatastoAppezzamento(), false) !== -1
    );
  }

  public override onCellClose = (event: CellCloseEvent, inputElementRef) => {
    // Aggiorno la textbox della Superficie
    const catastoAppezzamento: CatastoAppezzamento_Extended[] = this.daticatastali.getCatastoAppezzamento();
    const index: number = this.daticatastali.ControllaSeSelezionareLaRiga(event.dataItem, catastoAppezzamento, false);

    if (index !== -1) {
      event.dataItem.SuperficieCondottaDisponibile = parseFloat((event.dataItem.SuperficieCondottaDisponibile + (catastoAppezzamento[index].area - event.dataItem.SuperficieImpiegata)).toFixed(4));
      catastoAppezzamento[index].area = event.dataItem.SuperficieImpiegata;

      if (event.dataItem.SuperficieCondottaDisponibile < 0) {
        this.giasMessageService.warningMessage("Superficie selezionata superiore alla superficie condotta");
      }
    }

    this.daticatastali.setCatastoAppezzamento(catastoAppezzamento);
  };

  public override onCellClick = (event: CellClickEvent) => {
    if (event.dataItem.ChkSelezionaParticella == false || event.dataItem.ChkSelezionaParticella == null) {
      event.sender.closeRow(event.rowIndex);
    }
  };

  public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
    const selectedRows = event.selectedRows;
    const deselectedRows = event.deselectedRows;

    if (selectedRows.length !== 0) {
      this.handleSelectedRows(event);
    } else if (deselectedRows.length !== 0 ) {
      this.handleDeselectedRows(event);
    }

    this.gridPublicService.refresh(true);
  };

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  read(): Observable<DatiCatastaliServerResult> {
    // TODO Da completare quando sarà disponibile l'edit inline e controlla bene la funzione "CaricaKendo_Particelle" in Appezzamento.Edit
    //e guarda bene anche lato client cosa fa di preciso
    const DatiCatastali: any = {};

    let obs: Observable<any[]>
    if (this.DatiCatastaliObs == null) {
      const caricaCatastoSettings = this.daticatastali.getCaricaCatastoSettings();
      this.lastLoadedSettings = { ...caricaCatastoSettings };
      obs = this.appezzamentiService.CaricaDatiCatastaliObs(caricaCatastoSettings);
    } else {
      obs = of(this.DatiCatastaliObs)
    }

    this.daticatastali.loading.next(true);

    return obs.pipe(map((daticatastali: any[]) => {
      const rows = <KendoGridRow[]>daticatastali;
      this.DatiCatastaliObs = daticatastali;
      this.handleRead(rows);
      rows.sort(this.dynamicSort("SuperficieImpiegata"));
      rows.forEach((r:any) => {
        if (r.SuperficieImpiegata > 0){
          r.ChkSelezionaParticella = true;
        }
      })
      if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
        for (let i = 0; i < DatiCatastali?.kendo_columns?.length; i++){
          DatiCatastali.kendo_columns[i].editable = false;
        }
      }
      let col;
      let mod;
      if (this.gridPublicService?.giasGridComponent?.columns == undefined) {
        col = this.kendoColumns;
      } else {
        col = this.gridPublicService?.giasGridComponent?.columns;
      }
      if (this.gridPublicService?.giasGridComponent?.model == undefined) {
        mod = this.kendoModel;
      } else {
        mod = this.gridPublicService?.giasGridComponent?.model;
      }

      this.setModelGridDatiCatastali();
      this.setColumnsGridDatiCatastali();

      this.daticatastali.loading.next(false);

      return new DatiCatastaliServerResult(rows, col, mod);
    }), catchError(() => {
      this.daticatastali.loading.next(false);
      return of();
    }));
  }

  handleRead(rows: any[]) {
    this.assegnaAutomaticamenteMacrousiUtilizzi(rows, this.daticatastali.getCaricaCatastoSettings());

    const catastoAppezzamento = this.daticatastali.getCatastoAppezzamento();
    rows.forEach(row => {
      if (this.daticatastali.ControllaSeSelezionareLaRiga(row, catastoAppezzamento, true) >= 0) {
        row.Selected = true;
      } else {
        row.SuperficieImpiegata = 0;
        row.Selected = false;
      }
    });
  }

  assegnaAutomaticamenteMacrousiUtilizzi(rows: any[], settings: CaricaCatastoSettings) {
    let catastoAppezzamento = this.daticatastali.getCatastoAppezzamento();
    if (settings.chkMacrousi) {
      let particelleNoMacrousi = catastoAppezzamento.filter((el) => (el.macrousi == null) || (el.macrousi?.length == 0));

      particelleNoMacrousi.forEach((catasto) => {
        let rowsParticella = rows.filter((dataItem) => {
          const ProvItem = dataItem.CodiceIstat_Provincia;
          const ComuneItem = dataItem.CodiceIstat_Comune;
          let SezioneItem = dataItem.Sezione;
          const FoglioItem = dataItem.Foglio;
          const NumeroItem = dataItem.Numero;
          let SubalternoItem = dataItem.Subalterno;

          if (SezioneItem == '0') {
            SezioneItem = '';
          }

          if (SubalternoItem == '0') {
            SubalternoItem = '';
          }

          const ProvCatasto = catasto.particella['Prov'];
          const ComuneCatasto = catasto.particella['Com'];
          let SezioneCatasto = catasto.particella['Sezione'];
          const FoglioCatasto = catasto.particella['Foglio'];
          const NumeroCatasto = catasto.particella['Numero'];
          let SubalternoCatasto = catasto.particella['Subalterno'];

          if (SezioneCatasto == '0') {
            SezioneCatasto = '';
          }

          if (SubalternoCatasto == '0') {
            SubalternoCatasto = '';
          }

          return ProvCatasto === ProvItem &&
            ComuneCatasto === ComuneItem &&
            SezioneCatasto === SezioneItem &&
            FoglioCatasto === FoglioItem &&
            NumeroCatasto === NumeroItem &&
            SubalternoCatasto === SubalternoItem;
        });

        if (rowsParticella.length > 0) {
          catasto.macrousi = [{
            macrouso: { codice: rowsParticella[0].Macrouso_Cod, descrizione: rowsParticella[0].Macrouso_Des },
            Area: catasto.area,
            utilizzi: []
          }]
        }

      });

      if (settings.chkUtilizzi) {
        let macrousiNoUtilizzi: { particella: PKParticelleCatastali, macrouso: MacrousiCatastoAppezzamento}[] = [];

        catastoAppezzamento.forEach((el) => {
          let val = el.macrousi?.filter((macr) => (macr.utilizzi == null) || (macr.utilizzi?.length == 0));
          if (val) {
            val.forEach((nullVal) => {
              macrousiNoUtilizzi.push({particella: el.particella, macrouso: nullVal});
            })
          }
        });

        macrousiNoUtilizzi.forEach((catasto) => {

          let rowsParticella = rows.filter((dataItem) => {

            const ProvItem = dataItem.CodiceIstat_Provincia;
            const ComuneItem = dataItem.CodiceIstat_Comune;
            let SezioneItem = dataItem.Sezione;
            const FoglioItem = dataItem.Foglio;
            const NumeroItem = dataItem.Numero;
            let SubalternoItem = dataItem.Subalterno;

            if (SezioneItem == '0') {
              SezioneItem = '';
            }

            if (SubalternoItem == '0') {
              SubalternoItem = '';
            }

            const ProvCatasto: string = catasto.particella['Prov'];
            const ComuneCatasto: string = catasto.particella['Com'];
            let SezioneCatasto: string = catasto.particella['Sezione'];
            const FoglioCatasto: number = catasto.particella['Foglio'];
            const NumeroCatasto: number = catasto.particella['Numero'];
            let SubalternoCatasto: string = catasto.particella['Subalterno'];

            if (SezioneCatasto == '0') {
              SezioneCatasto = '';
            }

            if (SubalternoCatasto == '0') {
              SubalternoCatasto = '';
            }

            return ProvCatasto === ProvItem &&
              ComuneCatasto === ComuneItem &&
              SezioneCatasto === SezioneItem &&
              FoglioCatasto === FoglioItem &&
              NumeroCatasto === NumeroItem &&
              SubalternoCatasto === SubalternoItem &&
              dataItem.Macrouso_Cod === catasto.macrouso.macrouso.codice;
          })

          if (rowsParticella.length > 0) {
            catasto.macrouso.utilizzi = [{
              utilizzo: { codice: rowsParticella[0].Veg_Cod_Agea, descrizione: rowsParticella[0].Veg_Des_Agea },
              Area: catasto.macrouso.Area
            }]
          }

        });

      }

    }
  }

  dynamicSort(property) {
    var sortOrder = 1;
    if(property[0] === "-") {
      sortOrder = -1;
      property = property.substring(1);
    }
    return function (a,b) {
      /* next line works with strings and numbers,
      * and you may want to customize it to your needs
      */
      var result = (a[property] > b[property]) ? -1 : (a[property] < b[property]) ? 1 : 0;
      return result * sortOrder;
    }
  }

  setColumnsGridDatiCatastali(){
    let c: KendoGridColumn= null;

    let catastoSettings = this.daticatastali.getCaricaCatastoSettings();

    // In base ai vari flag nascondo o mostro le colonne
    if(catastoSettings.chkMacrousi && this.kendoColumns.find((el) => el.field == 'Macrouso_Cod') == undefined){

      c= new KendoGridColumn({field: 'Macrouso_Cod',title: this.translocoLoc.translate('CodiceMacrousoAbbr')},{hidden:true,resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c= new KendoGridColumn({field: 'Macrouso_Des',title: this.translocoLoc.translate('Macrouso')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c=new KendoGridColumn({field: 'Sup_Macrouso',title: this.translocoLoc.translate('SuperficieMacrousoAbbr')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c=new KendoGridColumn({field: 'SuperficieMacrousoDisponibile',title: this.translocoLoc.translate('SuperficieMacrousoDisponibileAbbr')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

    } else if (catastoSettings.chkMacrousi == false && this.kendoColumns.find((el) => el.field == 'Macrouso_Cod') != undefined) {
      let key = ['Macrouso_Cod', 'Macrouso_Des', 'Sup_Macrouso', 'SuperficieMacrousoDisponibile']
      key.forEach(element => {
        let index = this.kendoColumns.findIndex((e) => e.field == element);
        if (index >= 0) {
          this.kendoColumns.splice(index, 1)
        }
      });
    }

    if(catastoSettings.chkUtilizzi && this.kendoColumns.find((el) => el.field == 'Veg_Cod_Agea') == undefined){

      c=new KendoGridColumn({field: 'Veg_Cod_Agea',title: this.translocoLoc.translate('VegCodAgea')},{hidden:true,resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c=new KendoGridColumn({field: 'Veg_Des_Agea',title: this.translocoLoc.translate('SpecieAgea')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c=new KendoGridColumn({field: 'Sup_Utilizzo',title: this.translocoLoc.translate('SuperficieUtilizzoAbbr')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

      c=new KendoGridColumn({field: 'SuperficieUtilizzoDisponibile',title: this.translocoLoc.translate('SuperficieUtilizzoDisponibileAbbr')},{resizable:true,editable: false, width: 200});
      this.kendoColumns.push(c);

    } else if (catastoSettings.chkUtilizzi == false && this.kendoColumns.find((el) => el.field == 'Veg_Cod_Agea') != undefined) {
      let key = ['Veg_Cod_Agea', 'Veg_Des_Agea', 'Sup_Utilizzo', 'SuperficieUtilizzoDisponibile']
      key.forEach(element => {
        let index = this.kendoColumns.findIndex((e) => e.field == element);
        if (index >= 0) {
          this.kendoColumns.splice(index, 1)
        }
      });
    }
  }

  setModelGridDatiCatastali(){
    // In base ai vari flag nascondo o mostro le colonne

    let catastoSettings = this.daticatastali.getCaricaCatastoSettings();

    if(catastoSettings.chkMacrousi && this.kendoModel['Macrouso_Cod'] == undefined){
      this.kendoModel['Macrouso_Cod']={type: 'string'};
      this.kendoModel['Macrouso_Des']={type: 'string'};
      this.kendoModel['Sup_Macrouso']={type: 'string'};
      this.kendoModel['SuperficieMacrousoDisponibile']={type: 'string'};
    } else if (catastoSettings.chkMacrousi == false && this.kendoModel['Macrouso_Cod'] != undefined) {
      this.kendoModel['Macrouso_Cod'] = undefined;
      this.kendoModel['Macrouso_Des'] = undefined;
      this.kendoModel['Sup_Macrouso'] = undefined;
      this.kendoModel['SuperficieMacrousoDisponibile'] = undefined;
    }

    if(catastoSettings.chkUtilizzi && this.kendoModel['Veg_Cod_Agea'] == undefined){
      this.kendoModel['Veg_Cod_Agea']={type: 'string'};
      this.kendoModel['Veg_Des_Agea']={type: 'string'};
      this.kendoModel['Sup_Utilizzo']={type: 'string'};
      this.kendoModel['SuperficieUtilizzoDisponibile']={type: 'string'};
    } else if (catastoSettings.chkUtilizzi == false && this.kendoModel['Veg_Cod_Agea'] != undefined) {
      this.kendoModel['Veg_Cod_Agea']= undefined;
      this.kendoModel['Veg_Des_Agea']= undefined;
      this.kendoModel['Sup_Utilizzo']= undefined;
      this.kendoModel['SuperficieUtilizzoDisponibile']= undefined;
    }
  }

  catastoSettingsDifferences(oldSettings: CaricaCatastoSettings, newSettings: CaricaCatastoSettings): boolean{
    if (oldSettings == null && newSettings != null) {
      return true;
    }
    if (newSettings == null) {
      return true;
    }
    if (oldSettings.Piva == newSettings.Piva &&
      oldSettings.Sa_Cod == newSettings.Sa_Cod &&
      oldSettings.Campo_Cod == newSettings.Campo_Cod &&
      oldSettings.chkMacrousi == newSettings.chkMacrousi &&
      oldSettings.chkUtilizzi == newSettings.chkUtilizzi &&
      oldSettings.chkVarieta == newSettings.chkVarieta &&
      oldSettings.Validita_Inizio == newSettings.Validita_Inizio &&
      oldSettings.Validita_Fine == newSettings.Validita_Fine) {
      return false;
    }
    return true;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let a = gridElRef;
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      const row = visibleRows[index] as HTMLElement;
      if (el.SuperficieCondottaDisponibile < 0) {
        this.renderer.addClass(row, 'nonAttivo');
      } else {
        this.renderer.removeClass(row, 'nonAttivo');
      }
    });
  }

  private handleCustomizations(): void {
    // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
    this.cmdColumn.editBtn=false;
    this.cmdColumn.infoBtn=false;
    this.cmdColumn.removeBtn=false;
    // Setting colonna con checkbox
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.resizable.autoFitColumns=true;
    this.resizable.isResizable = true;

    // Setting generali altezza grid
    this.behavior.pdfSettings.enabled = false;
    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.generalSettings.height = 450;
    // Pre ordinamento colonne
    this.pagination.gridState.sort=[{
      field: 'Selected',
      dir: 'desc'
    }];

    if (this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read) {
      this.selectable.selectable.checkboxOnly = true;
      this.selectable.selectable.enabled = true;
      this.selectable.preselectedRows.isRowSelectedFn = this.SelezionaRigha;
      this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
      this.selectable.preselectedRows.selectedRows = [];
      this.selectable.columnSettings.showSelectAll = true;
      this.selectable.shouldShowCheckbox = true;
      this.selectable.columnSettings.title = ' ';
    }
  }

  private handleSelectedRows(event: SelectionEvent): void {
    const settings: CaricaCatastoSettings = this.daticatastali.getCaricaCatastoSettings();
    let catastoAppezzamento: CatastoAppezzamento_Extended[] = this.daticatastali.getCatastoAppezzamento();

    event.selectedRows.forEach(row => {
      if (row.dataItem.SuperficieCondottaDisponibile <= 0) {
        // segnalo il possibile errore, ma permetto comunque l'associazione della particella
        this.giasMessageService.warningMessage(this.transloco.translate('SuperficieDisponibileZero'));
      }

      row.dataItem.ChkSelezionaParticella = true;
      if (this.daticatastali.ControllaSeSelezionareLaRiga(row.dataItem, catastoAppezzamento, false) === -1) {
        let Sezione = row.dataItem.Sezione;
        let Subalterno = row.dataItem.Subalterno;

        if (Sezione === '&nbsp;' || Sezione === '') {
          Sezione = '0';
        }

        if (Subalterno === '&nbsp;' || Subalterno === '') {
          Subalterno = '0';
        }

        let findCatasto: boolean = false;
        let catasto: CatastoAppezzamento_Extended = catastoAppezzamento.find((el) => {
          return el.particella.Prov == row.dataItem.CodiceIstat_Provincia &&
            el.particella.Com == row.dataItem.CodiceIstat_Comune &&
            el.particella.Sezione == Sezione &&
            el.particella.Foglio == row.dataItem.Foglio &&
            el.particella.Numero == row.dataItem.Numero &&
            el.particella.Subalterno == Subalterno;
        });

        if (catasto == undefined) {
          catasto = new CatastoAppezzamento_Extended();
          catasto.area = row.dataItem.SuperficieImpiegata;
          catasto.particella = {} as any;
          catasto.particella['Prov'] = row.dataItem.CodiceIstat_Provincia;
          catasto.particella['Com'] = row.dataItem.CodiceIstat_Comune;
          catasto.particella['Sezione'] = Sezione;
          catasto.particella['Foglio'] = row.dataItem.Foglio;
          catasto.particella['Numero'] = row.dataItem.Numero;
          catasto.particella['Subalterno'] = Subalterno;
        } else {
          findCatasto = true;
        }

        if (!settings.chkMacrousi && row.dataItem.SuperficieCondottaDisponibile >= 0) {
          catasto.area = row.dataItem.SuperficieCondottaDisponibile;
        } else if (settings.chkMacrousi && !settings.chkUtilizzi && row.dataItem.SuperficieMacrousoDisponibile >= 0) {
          catasto.area = row.dataItem.SuperficieMacrousoDisponibile;

          if (!catasto.macrousi) {
            catasto.macrousi = [];
          }

          catasto.macrousi.push({
            macrouso: {codice: row.dataItem.Macrouso_Cod, descrizione: row.dataItem.Macrouso_Des},
            Area: row.dataItem.SuperficieMacrousoDisponibile,
            utilizzi: []
          });

          catasto.area = catasto.macrousi.reduce((sum, {Area}) => parseFloat((sum + Area).toFixed(4)), 0);
        } else if (settings.chkMacrousi && settings.chkUtilizzi && !settings.chkVarieta && row.dataItem.SuperficieUtilizzoDisponibile >= 0) {

          catasto.area = row.dataItem.SuperficieUtilizzoDisponibile;
          if (catasto.macrousi == undefined) {
            catasto.macrousi = [];
          }

          let macrousoFind: number = catasto.macrousi.findIndex((el) => el.macrouso.codice == row.dataItem.Macrouso_Cod);
          if (macrousoFind == -1) {
            catasto.macrousi.push({
              macrouso: {codice: row.dataItem.Macrouso_Cod, descrizione: row.dataItem.Macrouso_Des},
              Area: row.dataItem.SuperficieMacrousoDisponibile,
              utilizzi: [{
                utilizzo: {codice: row.dataItem.Veg_Cod_Agea, descrizione: row.dataItem.Veg_Des_Agea},
                Area: row.dataItem.SuperficieUtilizzoDisponibile
              }]
            });
          } else {
            let utilizzoFind: number = catasto.macrousi[macrousoFind].utilizzi.findIndex((el) => el.utilizzo.codice == row.dataItem.Veg_Cod_Agea);

            if (utilizzoFind == -1) {
              catasto.macrousi[macrousoFind].utilizzi.push({
                utilizzo: {codice: row.dataItem.Veg_Cod_Agea, descrizione: row.dataItem.Veg_Des_Agea},
                Area: row.dataItem.SuperficieUtilizzoDisponibile
              });
            }
          }

          catasto.macrousi.forEach((item) => {
            item.Area =
              item.utilizzi.reduce((sumUtilizzi, {Area}) => parseFloat((sumUtilizzi + Area).toFixed(4)), 0);
          });

          catasto.area = catasto.macrousi.reduce((sum, {Area}) => parseFloat((sum + Area).toFixed(4)), 0);
        } else if (settings.chkMacrousi && settings.chkUtilizzi && settings.chkVarieta && row.dataItem.SuperficieMacrousoDisponibile >= 0) {
          catasto.area = row.dataItem.SuperficieMacrousoDisponibile;
        }

        row.dataItem.SuperficieCondottaDisponibile -= catasto.area;
        row.dataItem.SuperficieCondottaDisponibile = parseFloat(row.dataItem.SuperficieCondottaDisponibile.toFixed(4));

        if (findCatasto) {
          let catastoIndex: number = catastoAppezzamento.findIndex((el) => {
            return el.particella.Prov == row.dataItem.CodiceIstat_Provincia &&
              el.particella.Com == row.dataItem.CodiceIstat_Comune &&
              el.particella.Sezione == Sezione &&
              el.particella.Foglio == row.dataItem.Foglio &&
              el.particella.Numero == row.dataItem.Numero &&
              el.particella.Subalterno == Subalterno;
          });
          catastoAppezzamento[catastoIndex] = catasto;
        } else {
          catastoAppezzamento.push(catasto);
        }
      }
    });

    this.daticatastali.setCatastoAppezzamento(catastoAppezzamento);
  }

  private handleDeselectedRows(event: SelectionEvent): void {
    const settings: CaricaCatastoSettings = this.daticatastali.getCaricaCatastoSettings();
    let catastoAppezzamento: CatastoAppezzamento_Extended[] = this.daticatastali.getCatastoAppezzamento();

    event.deselectedRows.forEach(row => {
      const index=this.daticatastali.ControllaSeSelezionareLaRiga(row.dataItem, catastoAppezzamento, false);
      row.dataItem.ChkSelezionaParticella = false;

      if (index !== -1) {
        if (settings.chkMacrousi) {
          const indexMacrouso: number = catastoAppezzamento[index].macrousi.findIndex(el => el.macrouso.codice == row.dataItem.Macrouso_Cod);

          if (settings.chkUtilizzi) {
            const indexUtilizzo: number = catastoAppezzamento[index].macrousi[indexMacrouso].utilizzi.findIndex(el => el.utilizzo.codice == row.dataItem.Veg_Cod_Agea);

            row.dataItem.SuperficieCondottaDisponibile += catastoAppezzamento[index].macrousi[indexMacrouso].utilizzi[indexUtilizzo].Area;
            row.dataItem.SuperficieCondottaDisponibile = parseFloat(row.dataItem.SuperficieCondottaDisponibile.toFixed(4));

            catastoAppezzamento[index].macrousi[indexMacrouso].utilizzi.splice(indexUtilizzo, 1);

            if (catastoAppezzamento[index].macrousi[indexMacrouso].utilizzi.length == 0) {
              catastoAppezzamento[index].macrousi.splice(indexMacrouso, 1);
            }

            catastoAppezzamento[index].macrousi.forEach((m) => {
              m.Area = m.utilizzi.reduce((sum, { Area }) => parseFloat((sum + Area).toFixed(4)), 0);
            });

            catastoAppezzamento[index].area = catastoAppezzamento[index].macrousi.reduce((sum, { Area }) => parseFloat((sum + Area).toFixed(4)), 0);
          } else {
            row.dataItem.SuperficieCondottaDisponibile += catastoAppezzamento[index].macrousi[indexMacrouso].Area;
            row.dataItem.SuperficieCondottaDisponibile = parseFloat(row.dataItem.SuperficieCondottaDisponibile.toFixed(4));

            catastoAppezzamento[index].macrousi.splice(indexMacrouso, 1);

            catastoAppezzamento[index].area = catastoAppezzamento[index].macrousi.reduce((sum, {Area}) => parseFloat((sum + Area).toFixed(4)), 0);
          }

          if (catastoAppezzamento[index].macrousi.length == 0) {
            catastoAppezzamento.splice(index, 1);
          }
        } else {
          row.dataItem.SuperficieCondottaDisponibile += catastoAppezzamento[index].area;
          row.dataItem.SuperficieCondottaDisponibile = parseFloat(row.dataItem.SuperficieCondottaDisponibile.toFixed(4));

          catastoAppezzamento.splice(index, 1);
        }
      }
    });

    this.daticatastali.setCatastoAppezzamento(catastoAppezzamento);
  }
}
