import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { ToolbarSettings, CommandsColumnSettings, SelectableSettings, GroupSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, from, map, tap, catchError, of, combineLatest, startWith } from "rxjs";
import { QdCService } from "../qdc.service";
import { KendoServerResult, KendoGridRow, KendoGridColumn, KendoGridModel, LoaderType, EditingMode, GridCustomizations, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AnagraficaClient, Attivita_Tipo_Attivita, LeggiProdotti, RispostaStandard_1OfList_1OfMovimentoDiMagazzino } from "app/Service/api.service";
import { ProdottoDaTrattareCDC } from "app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Tipo_Attivita} from 'gias-ui-kit';
import { ActivatedRoute } from "@angular/router";
import { RibaltamentoTypes } from "app/menu-agenda/components/utils";
import { MisceleService } from "../miscele.service";

export class GridProdottiDaTrattareServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], columns: KendoGridColumn[], model: KendoGridModel) {
    super(model, columns, rows);
  }
}

@Injectable()
export class GridProdottiDaTrattareHttpService extends AbstractGridConfigService<GridProdottiDaTrattareServerResult> {
  gridId: string = "ProdottiDaTrattareGridId";
  rowId: string = "id";

  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;

  views = new GridCustomizations({ enabled: true });

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'id', title: '' }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'centro', title: this.translocoService.translate('Centro') }, { editable: false }),
    new KendoGridColumn({ field: 'magazzino', title: this.translocoService.translate('Magazzino') }, { editable: false }),
    new KendoGridColumn({ field: 'finalita', title: this.translocoService.translate('Finalita') }, { editable: false }),
    new KendoGridColumn({ field: 'prodottoCod', title: '' }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'prodotto', title: this.translocoService.translate('Prodotto') }, { editable: false }),
    new KendoGridColumn({ field: 'partitaIvaReale', title: '' }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'saCod', title: '' }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'fabbricatoCod', title: '' }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'codice_alfanumerico', title: this.translocoService.translate('CodiceArticolo') }, { editable: false }),
    new KendoGridColumn({ field: 'Lotto', title: this.translocoService.translate('LottoDiAccettazione') }, { editable: false }),
    new KendoGridColumn({ field: 'giacenzaRilevata', title: this.translocoService.translate('GiacenzaRilevataQuintali') }, { editable: false }),
    new KendoGridColumn({ field: 'qta', title: this.translocoService.translate('QtaTrattataQuintali') }, { editable: this.qdcservice.abilitaGrid, format: '{0:' + this.qdcservice.get4DecimalNumericSettings().format + '}', numeric: this.qdcservice.get4DecimalNumericSettings() }),
  ];

  private kendoModel: KendoGridModel = {
    id: new ModelEntry(CELL_TYPES.STRING, false),
    centro: new ModelEntry(CELL_TYPES.STRING, false),
    magazzino: new ModelEntry(CELL_TYPES.STRING, false),
    finalita: new ModelEntry(CELL_TYPES.STRING, false),
    prodottoCod: new ModelEntry(CELL_TYPES.NUMBER, false),
    prodotto: new ModelEntry(CELL_TYPES.STRING, false),
    partitaIva: new ModelEntry(CELL_TYPES.STRING, false),
    saCod: new ModelEntry(CELL_TYPES.NUMBER, false),
    fabbricatoCod: new ModelEntry(CELL_TYPES.NUMBER, false),
    codice_alfanumerico: new ModelEntry(CELL_TYPES.NUMBER, false),
    Lotto: new ModelEntry(CELL_TYPES.STRING, false),
    giacenzaRilevata: new ModelEntry(CELL_TYPES.NUMBER, false),
    qta: new ModelEntry(CELL_TYPES.NUMBER, true),
    Cod_Progetto: new ModelEntry(CELL_TYPES.NUMBER, false),
    partitaIvaReale: new ModelEntry(CELL_TYPES.STRING, false)
  };

  private data: ProdottoDaTrattareCDC[] = [];
  private isInitialized: boolean = false;
  private initialTot: number = 0;

  constructor(
    injector: Injector,
    private objParamService: ObjParametriAgendaService,
    private qdcservice: QdCService,
    private translocoService: TranslocoService,
    private anagraficaClient: AnagraficaClient,
    private misceleService: MisceleService,
    private route: ActivatedRoute
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(false, false);
    this.pagination.gridState.take = 6;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.selectable.selectable = new SelectableSettings({ enabled: true, });
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title = ' ';
    this.selectable.columnSettings.includeInChooser = false;
    this.selectable.columnSettings.showSelectAll = true;
    this.resizable.autoFitColumns = true;

    this.resizable.isResizable = true;

    this.columnMenu.kendoGridColumnChooser = true;

    this.behavior.excelSettings.enabled = false;
    this.behavior.pdfSettings.enabled = false;

    this.generalSettings.height = 'auto';

    this.groups = new GroupSettings({ groupable: { enabled: false, showFooter: false } }, this.translocoService);
    this.gridPublicService.subscribe(() => this.onDataUpdate());
  }

  perform(actionType: HttpAction, _: any): Observable<any[]> {
    return from([]);
  }

  read(): Observable<GridProdottiDaTrattareServerResult> {
    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
    const prodottiSelezionati = (this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.value ?? []) as ProdottoDaTrattareCDC[];
    const piva = this.objParamService.getObjParamValue().Piva;

    const centro$ = this.qdcservice.TestataForm.controls.Centro_Aziendale.valueChanges
      .pipe(startWith(this.qdcservice.TestataForm.get('Centro_Aziendale').value));

    return combineLatest([this.anagraficaClient.anagraficaLeggiProdottiDaTrattareQdC({
      impresa: { partitaIva: piva },
      tipoAttivita: this.qdcservice.TestataForm.get("Tipo").value,
      statoAttivita: this.qdcservice.TestataForm.get("Stato").value,
      centroAziendale: { primaryKey: { codice: 0 } },
      lavorazione: this.qdcservice.TestataForm.get('Operazioni').getRawValue()[0],
      impianti: null,
      specie: this.qdcservice.TestataForm.get('Specie').getRawValue(),
      varieta: null,
      regolamento: null,
      finalita: null,
      disciplinare: null,
      epocaDPI: null,
      avversitaGruppo: null,
      filtroPerDescrizione: '',
      data: this.qdcservice.TestataForm.get('Data').getRawValue(),
      escludiGiacenzeZero: false,
      magazziniAgenzie: true,
      magazziniEsterni: true,
      pua: null
    } as LeggiProdotti), centro$])
      .pipe(
        catchError(() => of([null as RispostaStandard_1OfList_1OfMovimentoDiMagazzino, null])),
        map(([result, centro]) => new GridProdottiDaTrattareServerResult(this.parseResultAndManageVisibility(result, prodottiSelezionati, centro), [...this.kendoColumns], { ...this.kendoModel })),
        tap(() => {
          this.isInitialized = true;
          this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

          // This needs to be unrelated to the read logic and must take place after the rendering
          setTimeout(() => {
            if (this.isRibaltamento()) {
              this.handleRibaltamento();
            }
          }, 200);
        })
      );
  }

  private onDataUpdate(): void {
    if (!this.isInitialized) {
      return;
    }

    const tipo = this.qdcservice.TestataForm.get("Tipo").value;
    const rows: ProdottoDaTrattareCDC[] = [];
    const selected = this.gridPublicService?.getValue()?.data?.rows ?? [] as any[];
    for (const data of selected.filter(x => x.Selected)) {
      const existing = this.data.find(x =>
        x.giacenzaMagazzino.Lotto == data.Lotto
        && x.giacenzaMagazzino.Prodotto.codice == data.prodottoCod
        // Controllo chiave magazzino
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.centroAziendalePK?.partitaIva == data.partitaIva
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.centroAziendalePK?.codice == data.saCod
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.codice == data.fabbricatoCod
        // Per le ricette e per ribaltamento non teniamo conto del codice_progetto
        && (tipo == Tipo_Attivita.Ricetta || x.giacenzaMagazzino.codice_progetto == data.Cod_Progetto)
      );

      data.qta = data.qta == 0 ? data.giacenzaRilevata : data.qta;
      existing.qtaTrattata = data.qta;
      rows.push(existing);
    }

    for (const data of selected.filter(x => !x.Selected)) {
      data.qta = 0;
    }

    this.qdcservice.updateProdottiDaTrattareSelected(rows);
  };

  private parseResultAndManageVisibility(response: RispostaStandard_1OfList_1OfMovimentoDiMagazzino, prodottiSelezioniati: ProdottoDaTrattareCDC[], centro: CentroAziendale): KendoGridRow[] {
    if (response?.RispostaStringa == null) {
      return [];
    }

    this.data = [];
    const isRibaltamento = this.isRibaltamento();

    const tipo = this.qdcservice.TestataForm.get("Tipo").value;
    const result = [];
    for (const data of response?.RispostaStringa) {
      const existing = prodottiSelezioniati?.find(x =>
        x.giacenzaMagazzino.Lotto == data.Lotto
        && x.giacenzaMagazzino.Prodotto?.codice == data.Prodotto?.codice
        // Controllo chiave magazzino
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.centroAziendalePK?.partitaIva == data.Magazzino?.primaryKey?.centroAziendalePK?.partitaIva
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.centroAziendalePK?.codice == data.Magazzino?.primaryKey?.centroAziendalePK?.codice
        && x.giacenzaMagazzino.Magazzino?.primaryKey?.codice == data.Magazzino?.primaryKey?.codice
        // Per le ricette e per ribaltamento non teniamo conto del codice_progetto
        && (isRibaltamento || tipo == Tipo_Attivita.Ricetta || x.giacenzaMagazzino.codice_progetto == data.codice_progetto)
      );

      const prod = new ProdottoDaTrattareCDC();
      prod.giacenzaMagazzino = data;
      prod.qtaTrattata = existing?.qtaTrattata ?? 0;

      this.data.push(prod);

      if (centro.primaryKey.codice == 0 || data.Magazzino.primaryKey.centroAziendalePK.codice == centro.primaryKey.codice) {
        result.push({
          id: `${data.Lotto}-${data.Prodotto?.codice}-${data.codice_progetto}`,
          centro: data.Magazzino?.descrizione_centro,
          magazzino: data.Magazzino?.descrizione,
          finalita: data.Prodotto?.finalita?.descrizione,
          prodotto: data.Prodotto?.descrizione,
          prodottoCod: data.Prodotto?.codice,
          partitaIva: data.Magazzino?.primaryKey?.centroAziendalePK?.partitaIva,
          saCod: data.Magazzino?.primaryKey?.centroAziendalePK?.codice,
          fabbricatoCod: data.Magazzino?.primaryKey?.codice,
          codice_alfanumerico: data.Prodotto?.codice_alfanumerico,
          Lotto: data.Lotto,
          giacenzaRilevata: data.Qta,
          qta: existing?.qtaTrattata ?? 0,
          Cod_Progetto: data.codice_progetto,
          Selected: existing != null,
        } as KendoGridRow);
      }
    }

    // Order by selected
    result.sort((x, y) => x.Selected == y.Selected ? 0 : x.Selected ? -1 : 1);

    // Hide Cod_Progetto column if we don't have any
    const showCodProgettoColumn = result.some(x => x.Cod_Progetto > 0) && this.kendoColumns.find(x => x.field == 'Cod_Progetto') == null;
    if (showCodProgettoColumn) {
      this.kendoColumns.push(new KendoGridColumn({ field: 'Cod_Progetto', title: this.translocoService.translate('Esercizio') }, { editable: false, hidden: true }));
    }

    // Trigger update event
    this.qdcservice.updateProdottiDaTrattareSelected(prodottiSelezioniati);
    this.initialTot = prodottiSelezioniati.reduce((accumulator, prod) => accumulator + prod.qtaTrattata, 0);

    return result;
  }

  private isRibaltamento(): boolean {
    const ribaltamento = this.route.snapshot.queryParamMap.get('t_r');
    if (ribaltamento == null || Number.isNaN(+ribaltamento)) {
      return false;
    }

    return +ribaltamento == RibaltamentoTypes.Da_Ricetta_ad_Agenda;
  }

  private handleRibaltamento(): void {
    this.qdcservice.QuantitaForm.get("Qta_Trattata").setValue(this.initialTot, { emitEvent: false })
    this.misceleService.calcoloMiscele('Sup_Trattata', null, 0, this.qdcservice.QuantitaForm.get("Qta_Trattata").value);
    const newkendoserver = this.qdcservice.ripartizionaQtaTrattata();

    this.qdcservice.GridProdottiDaTrattarePublicService.refresh(false, newkendoserver.data);
  }
}
