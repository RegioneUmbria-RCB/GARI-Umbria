import {Injectable, Injector} from "@angular/core";
import {from, Observable, of} from "rxjs";
import {QdCService} from "../../qdc.service";
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {enum_TipoOperazioneDB} from "../../../../../Model/TipiEnumerativi";
import {Lavorazione} from "../../../../../Model/attivita/Lavorazione";
import {
  Dettaglio_Formulato,
  GridImpiantoSelezionatoModel, Superfici
} from "../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {GridDosiProdottiService} from "../../grid-dosi-prodotti/grid-dosi-prodotti.service";
import {QuantitaSuImpianto} from "../../../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {CellClickEvent, CellCloseEvent} from "@progress/kendo-angular-grid";
import {EsercizioCDC} from "../../../../../Model/attivita/centri_di_costo/EsercizioCDC";
import {MisceleService} from "../../miscele.service";
import {FunzioniComuniService} from "../../../../../Service/FunzioniComuni.service";
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  ConfigTemplate, EditingMode,
  ExcelSettings, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType,
  ModelEntry,
  PaginationSettings,
  ToolbarSettings
} from "gias-kendo-grid";
import { CELL_TYPES } from "gias-ui-kit";

export class GridProdottixImpiantiServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()

export class GridProdottixImpiantiHttpService extends AbstractGridConfigService<GridProdottixImpiantiServerResult>{

  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'ProdottixImpiantiGridrowId';
  gridId = 'GridProdottixImpianti';

  FormulatiForm: FormGroup;

  GridProdottiXImpianti: GridProdottixImpiantiServerResult = new GridProdottixImpiantiServerResult([],[],null);

  //Se impostato a true calcola la Quantita di prodotto totale che deve andare su ogni impianto.
  //Aggiunto questo flag perchè così appena si apre la pagina di una operazione in modifica non viene sovrascritta la quantità di prodotto salvata
  Flag_Calcola_Qta_Su_Impianti: boolean = false;

  //Moltiplicatore che cambia in base all'unità di misura scelta, se ad esempio si sceglie Kg invece di q,
  //il moltiplicatore sarà 100 in modo da convertire la quantità inserita in Kg alla quantità in ql che è l'unità di misura con cui vengono salvate le quantità su impianti
  Moltiplicatore_Qta: number = 1;

  constructor(injector: Injector,
              private qdcservice: QdCService,
              private parent: FormGroupDirective,
              private misceleservice: MisceleService,
              private griddosiprodottiservice: GridDosiProdottiService,
              private funzionicomuniservice: FunzioniComuniService) {

    super(injector, ConfigTemplate.DefaultTemplate);
    this.FormulatiForm = this.parent.form;
    this.handleCustomizatons();
  }

  private handleCustomizatons() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });
    this.groups.groupable.enabled = false;
    this.behavior.excelSettings = new ExcelSettings({enabled: false});
    this.views.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;
    this.pagination = this.handlePagination();
    this.resizable.autoFitColumns=true;

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

  read(options?: any): Observable<GridProdottixImpiantiServerResult> {

      this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

      return of(this.setGridProdottiXImpianti());
  }

  setGridProdottiXImpianti(): GridProdottixImpiantiServerResult{

    this.GridProdottiXImpianti.rows = this.setRowsGridProdottiXImpianti();

    this.GridProdottiXImpianti.columns = this.setColumnsGridProdottiXImpianti();

    this.GridProdottiXImpianti.model = this.setModelGridProdottiXImpianti();

    this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

    return this.GridProdottiXImpianti;
  }

  private setColumnsGridProdottiXImpianti(){

    const numericsettings= this.qdcservice.getProductNumericSettings(this.FormulatiForm.get("UdM").getRawValue());

    numericsettings.autoCorrect = true;

    const columnEditable: boolean = this.qdcservice.abilitaGrid;

    let columns: Array<KendoGridColumn>=[
      new KendoGridColumn({field: 'App_Nome',title: this.transloco.translate('AppezzamentoAbbr')},{resizable:true,editable: false,showHTMLAsString:true}),
      new KendoGridColumn({field: 'Dose_Ha',title: this.transloco.translate('rbl_QtaTotResource1.Text')},{resizable:true,editable: columnEditable,format: '{0:'+numericsettings.format+'}',numeric: numericsettings})
    ];

    return columns;
  }

  private setModelGridProdottiXImpianti(){
      var model: KendoGridModel = {};

      model.ProdottixImpiantiGridrowId = new ModelEntry(CELL_TYPES.STRING);
      model.App_Nome = new ModelEntry(CELL_TYPES.STRING);
      model.Dose_Ha = new ModelEntry(CELL_TYPES.NUMBER);
      model.EsercizioCDC = new ModelEntry(CELL_TYPES.DROPDOWNLIST);

    return model;
  }

  private setRowsGridProdottiXImpianti(): KendoGridRow[]{

    let rows = [];

    let impiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

    let dettaglio_formulato: Dettaglio_Formulato = this.parent.form.getRawValue();

    if(dettaglio_formulato && dettaglio_formulato.QuantitaSuImpianti && dettaglio_formulato.QuantitaSuImpianti.length > 0 &&
       impiantiSelezionati && impiantiSelezionati.length > 0){

      let Sup_Trattata_Totale = (this.qdcservice.SuperficiForm.getRawValue() as Superfici).Sup_Trattata;

      let quantitaRimanente = dettaglio_formulato.DoseTot_Ha;

      const DoseTot_Ha = dettaglio_formulato.DoseTot_Ha;

      dettaglio_formulato.QuantitaSuImpianti.forEach((qt: QuantitaSuImpianto,index_qt: number) => {

        if(qt.esercizioCDC){

          let index = impiantiSelezionati.findIndex(imp=>imp.PIVA === qt.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &&
                                                                                          imp.SA_COD ===  qt.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &&
                                                                                          imp.APPEZZA ===  qt.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice &&
                                                                                          imp.ID_REG ===  qt.esercizioCDC.esercizio.impiantoPK.codice &&
                                                                                          imp.Progetto_Cod === qt.esercizioCDC.esercizio.codice);

          if(index > -1){
            qt.esercizioCDC.esercizio.descrizione = impiantiSelezionati[index].App_Nome;

            qt.esercizioCDC.superficieTrattata = impiantiSelezionati[index].Sup_Imp_help;
          }

          const UdM = this.FormulatiForm.get("UdM").getRawValue();

          if(this.Flag_Calcola_Qta_Su_Impianti){

            let proporzione = qt.esercizioCDC.superficieTrattata / Sup_Trattata_Totale;

            let quantitaPerImpianto =   this.funzionicomuniservice.roundNumber(DoseTot_Ha * proporzione,this.qdcservice.getProductNumericSettings(UdM).decimals);

            // Assicurati che non si superi la quantità totale
            if (quantitaPerImpianto > quantitaRimanente) {
              quantitaPerImpianto = quantitaRimanente;
            }

            qt.Qta = quantitaPerImpianto;
            quantitaRimanente -= quantitaPerImpianto;
          }else{

            qt.Qta = qt.Qta * this.Moltiplicatore_Qta;

            qt.Qta =  this.funzionicomuniservice.roundNumber(qt.Qta,this.qdcservice.getProductNumericSettings(UdM).decimals);
          }

        }

        rows.push({App_Nome: qt.esercizioCDC.esercizio.descrizione,Dose_Ha: qt.Qta,
                   ProdottixImpiantiGridrowId: this.setProdottixImpiantiGridrowId(index_qt),EsercizioCDC: qt.esercizioCDC});
      });

      // Se c'è ancora quantità rimanente, la distribuiamo in modo equo
      if (this.Flag_Calcola_Qta_Su_Impianti && quantitaRimanente > 0) {

        for(let x= 0; x < dettaglio_formulato.QuantitaSuImpianti.length; x++){
          if (quantitaRimanente > 0) {

            dettaglio_formulato.QuantitaSuImpianti[x].Qta++;

            quantitaRimanente--;

            let i = rows.findIndex(r=>(r.EsercizioCDC as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva && dettaglio_formulato.QuantitaSuImpianti[x].esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &&
                          (r.EsercizioCDC as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice && dettaglio_formulato.QuantitaSuImpianti[x].esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &&
                          (r.EsercizioCDC as EsercizioCDC).esercizio.impiantoPK.appezzamentoPK.codice && dettaglio_formulato.QuantitaSuImpianti[x].esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice &&
                          (r.EsercizioCDC as EsercizioCDC).esercizio.impiantoPK.codice && dettaglio_formulato.QuantitaSuImpianti[x].esercizioCDC.esercizio.impiantoPK.codice &&
                          (r.EsercizioCDC as EsercizioCDC).esercizio.codice && dettaglio_formulato.QuantitaSuImpianti[x].esercizioCDC.esercizio.codice);

            if(i > -1)
              rows[i].Dose_Ha = dettaglio_formulato.QuantitaSuImpianti[x].Qta;

          } else {
            break;
          }
        }
      }

      let index = this.griddosiprodottiservice.getIndexByDosiProdottiGridrowId(this.qdcservice.DosiProdottiFormArray(null,dettaglio_formulato.Operazione).getRawValue(),dettaglio_formulato.DosiProdottiGridrowId);

      this.qdcservice.AggiornaFormArrayGridDosiProdotti(dettaglio_formulato,index,dettaglio_formulato.Operazione,enum_TipoOperazioneDB.Modifica,null,false);

    }

    return rows;
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return from([]);
  }

  private setProdottixImpiantiGridrowId(index: number): string{

    let lav_cod: number = + ((this.FormulatiForm.get("Operazione").getRawValue()) as Lavorazione).primaryKey.codice;

    let DosiProdottiGridrowId = this.FormulatiForm.get("DosiProdottiGridrowId").getRawValue();

    return `${lav_cod}_${DosiProdottiGridrowId}_${index}`;
  }

  public override onCellClose = (event: CellCloseEvent, inputElementRef) => {

    if(event.dataItem){

      if(event.dataItem.Dose_Ha === null || event.dataItem.Dose_Ha === undefined )
        event.dataItem.Dose_Ha = 0;

      let newDose_Ha: number = event.dataItem.Dose_Ha;

      let rowEsercizioCDC: EsercizioCDC = event.dataItem.EsercizioCDC;

      let dettaglio_formulato: Dettaglio_Formulato = this.FormulatiForm.getRawValue();

      if(dettaglio_formulato && dettaglio_formulato.QuantitaSuImpianti && rowEsercizioCDC){
        let index_qt = dettaglio_formulato.QuantitaSuImpianti.findIndex(x=>x.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva === rowEsercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &&
                                                                              x.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice === rowEsercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &&
                                                                              x.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice === rowEsercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice &&
                                                                              x.esercizioCDC.esercizio.impiantoPK.codice === rowEsercizioCDC.esercizio.impiantoPK.codice &&
                                                                              x.esercizioCDC.esercizio.codice === rowEsercizioCDC.esercizio.codice);

        if(index_qt > -1)
          dettaglio_formulato.QuantitaSuImpianti[index_qt].Qta = newDose_Ha;


        let index = this.griddosiprodottiservice.getIndexByDosiProdottiGridrowId(this.qdcservice.DosiProdottiFormArray(null,dettaglio_formulato.Operazione).getRawValue(),dettaglio_formulato.DosiProdottiGridrowId);


        this.qdcservice.AggiornaFormArrayGridDosiProdotti(dettaglio_formulato,index,dettaglio_formulato.Operazione,enum_TipoOperazioneDB.Modifica,null,false);

        let TotDose_Ha = 0;

        dettaglio_formulato.QuantitaSuImpianti.forEach(Qt=>TotDose_Ha += Qt.Qta);

        this.FormulatiForm.patchValue({
          "DoseTot_Ha": TotDose_Ha
        },{emitEvent: false});

        this.misceleservice.calcoloMiscele("changeDoseTot_Ha",this.FormulatiForm,0,TotDose_Ha);

      }
    }

  }

  public override onCellClick = (event: CellClickEvent) => {
    //Se il prodotto è già stato confermato o la griglia è in sola lettura chiudo la cella
    if(!this.qdcservice.abilitaGrid || this.FormulatiForm.get("Riga_Salvata").getRawValue()){
      event.sender.closeRow(event.rowIndex);
    }
  };
}
