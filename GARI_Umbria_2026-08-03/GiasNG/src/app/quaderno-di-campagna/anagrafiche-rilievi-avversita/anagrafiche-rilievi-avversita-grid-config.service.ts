import { Injectable, Injector } from "@angular/core";
import { AgendaClient, CategoriaOperazione, Job_PK, Lavorazione, LeggiDisciplinari, MisuraPerAvversitaAnagrafica_In, MisuraPerAvversitaAnagraficaExtended, ModelloClient, PopolaRilievo, RispostaStandard, RispostaStandard_1OfList_1OfDisciplinare, RispostaStandard_1OfString, TipiJob } from "app/Service/api.service";
import { GiasMessageService } from "app/Service/gias-message.service";
import { SpecieVegetaliService } from "app/Service/Metaschema/specie-vegetali.service";
import { CommandsColumnSettings, CommandsDropDownSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { catchError, debounceTime, forkJoin, from, map, Observable, of, share, startWith, Subject, switchMap, take, tap } from "rxjs";
import { Avversita } from "../rilievi-avversita/rilievi-avversita.service";
import { Disciplinare } from 'app/Service/api.service';

const DEFAULT_LAV_CODE = "113";
const AVVERSITA_LAVORAZIONI = [{
  categoriaOperazione: null as CategoriaOperazione,
  primaryKey: {
    classType: "Lavorazione",
    codice: "113"
  } as Job_PK,
  descrizione: "Rilievi Avversita' in Campo",
  tipo: TipiJob.LAVORAZIONE
}] as Lavorazione[];

export class AnagraficheRilieviAvversitaResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
      super(model, cols, rows);
    }
  }

export class AnagraficheRilieviAvversitaGridModel extends KendoGridModel {
    CodiceMisura: ModelEntry;
    CodiceAnagrafica: ModelEntry;
    Descrizione: ModelEntry;
    valoreAnagrafica: ModelEntry;
    DPI_FlagPrivatoPubblico: ModelEntry;
    DPI_COD: ModelEntry;
    modificabile: ModelEntry;
    cancellabile: ModelEntry;
    Veg_Cod: ModelEntry;
    Veg_Des: ModelEntry;
    Av_Cod: ModelEntry;
    DescrizioneAvversita: ModelEntry;
}

@Injectable()
export class AnagraficheRilieviAvversitaGridConfig extends AbstractGridConfigService<AnagraficheRilieviAvversitaResult> {
  
  gridId = 'AnagraficheRilieviAvversitaGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'CodiceAnagrafica';
  private reloadSubject = new Subject<void>();
  private read$: Observable<AnagraficheRilieviAvversitaResult> | null = null;
  
  constructor(injector: Injector,
              public gridpublicService: GridPublicService,
              private agendaClient: AgendaClient,
              private giasMessageService: GiasMessageService,
              private specieVegetaliService: SpecieVegetaliService,
              private modelloClient: ModelloClient
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true,
      onDisableRemoveBtn: (data: MisuraPerAvversitaAnagraficaExtended) => !data.cancellabile,
      onDisableEditBtn: (data: MisuraPerAvversitaAnagraficaExtended) => !data.modificabile
    });

    this.cmdColumn['widthSet'] = 25;
    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.generalSettings.performOnEdit = true;

    this.handleDropdowns();
   }

  ngOnInit(): void {
  }

  get reload$(): Observable<void> {
    return this.reloadSubject.asObservable();
  }

  public nextReload(): void {
    this.reloadSubject.next();
  }


  anagraficheRilieviAvversitaGridModel: KendoGridModel = {
    CodiceMisura: {
      editable: false,
      type: CELL_TYPES.NUMBER
    },
    CodiceAnagrafica: {
      editable: false,
      type: CELL_TYPES.NUMBER
    },
    Descrizione: {
      editable: true,
      type: CELL_TYPES.STRING
    },
    valoreAnagrafica: {
      editable: true,
      type: CELL_TYPES.NUMBER
    },
    DPI_FlagPrivatoPubblico: {
      editable: false,
      type: CELL_TYPES.NUMBER
    },
    DPI_COD: {
      editable: true,
      type: CELL_TYPES.DROPDOWNLIST
    },
    DPI_Des:{
      editable: true,
      type: CELL_TYPES.STRING
    },
    modificabile: {
      editable: false,
      type: CELL_TYPES.BOOLEAN
    },
    cancellabile: {
      editable: false,
      type: CELL_TYPES.BOOLEAN
    },
    Veg_Cod: {
      editable: true,
      type: CELL_TYPES.DROPDOWNLIST
    },
    Veg_Des: {
      editable: true,
      type: CELL_TYPES.STRING
    },
    Av_Cod: {
      editable: true,
      type: CELL_TYPES.DROPDOWNLIST
    },
    DescrizioneAvversita: {
      editable: true,
      type: CELL_TYPES.STRING
    },
    IdRcdpi: {
      editable: false,
      type: CELL_TYPES.NUMBER
    }
  };

  columns: KendoGridColumn[] = [
      new KendoGridColumn(
        { field: 'Veg_Cod', title: this.transloco.translate('Specie')},
        { resizable: true, filterable: true, editable: true, width: 200, disabledRule: (rowData) => rowData?.CodiceMisura != null }
      ),
      new KendoGridColumn(
        { field: 'DPI_COD', title: this.transloco.translate('Disciplinare')},
        { resizable: true, filterable: true, editable: true, width: 200, disabledRule: (rowData) => rowData?.CodiceMisura != null }
      ),
      new KendoGridColumn(
        { field: 'Av_Cod', title: this.transloco.translate('Avversità')},
        { resizable: true, filterable: true, editable: true, width: 200, disabledRule: (rowData) => rowData?.CodiceMisura != null }
      ),
      new KendoGridColumn(
        { field: 'Descrizione', title: this.transloco.translate('RilieviAvversitaDescrizione') },
        { resizable: true, filterable: true, editable: true, width: 200 }
      ),
      new KendoGridColumn(
        { field: 'valoreAnagrafica', title: this.transloco.translate('RilieviAvversitaValoreAnagrafica') },
        { resizable: true, filterable: true, editable: true, 
          width: 50, numeric: new NumericSettings({ format: '#', decimals: 0 })}
      )
    ];

  handleDropdowns(){
    const colSpecie = this.columns.find(s => s.field === 'Veg_Cod');
    const dataSpecie: DropdownListItem[] = [];
    colSpecie.ddl = new DropdownListWithForm('codice', 'Veg_Cod', 'descrizione', dataSpecie);
    colSpecie.ddl.valuePrimitive = true;
    colSpecie.ddl.descriptionField = 'Veg_Des';
    colSpecie.ddl.loadOnEdit = true;
    colSpecie.ddl.loadFunction = this.caricaDropdownSpecie.bind(this);

    const colDpi = this.columns.find(s => s.field === 'DPI_COD');
    const dataDpi: DropdownListItem[] = [];
    colDpi.ddl = new DropdownListWithForm('codice', 'DPI_COD', 'descrizione', dataDpi);
    colDpi.ddl.valuePrimitive = true;
    colDpi.ddl.descriptionField = 'DPI_Des';
    colDpi.ddl.loadOnEdit = true;
    colDpi.ddl.loadFunction = this.caricaDropdownDpi.bind(this);

    const colAvversita = this.columns.find(s => s.field === "Av_Cod");
    const dataAvversita: DropdownListItem[] = [];
    colAvversita.ddl = new DropdownListWithForm('cod', 'Av_Cod', 'des', dataAvversita);
    colAvversita.ddl.valuePrimitive = true;
    colAvversita.ddl.descriptionField = 'DescrizioneAvversita';
    colAvversita.ddl.loadOnEdit = true;
    colAvversita.ddl.loadFunction = this.caricaDropdownAvversita.bind(this);
  }

  private caricaDropdownSpecie() {
    return from(this.specieVegetaliService.leggi_FiltroUtente()).pipe(take(1), map(R => {
        return R.map(t => {
            return {codice:t.codice, descrizione:t.descrizione}
        })
    }));
  }

  disciplinariList: Disciplinare[] = [];
  avversitaList: Avversita[] = [];

  private caricaDropdownDpi(){
    let vegCod = this.gridPublicService.formGroup.value.controls['Veg_Cod'].value;
    let vegDes = this.gridPublicService.formGroup.value.controls['Veg_Des'].value;

    if (vegCod != null){
      const payload = {
        data: new Date(),
        privato: true,
        specie: {
          codice: vegCod,
          descrizione: vegDes
        },
        regolamento: null,
        lavorazioni: AVVERSITA_LAVORAZIONI
      } as LeggiDisciplinari;

      return this.modelloClient.modelloLeggiDisciplinariTestataConRegolamentoConcimazione(payload).pipe(
        tap((x: RispostaStandard_1OfList_1OfDisciplinare) => this.disciplinariList = x.RispostaStringa),
        map((x: RispostaStandard_1OfList_1OfDisciplinare) => {
          return x.RispostaStringa.map(t => {
            return { codice: +t.codice, descrizione: t.descrizione }
          })
        })
      );

    } else {
      return from([]);
    }
  }

  private caricaDropdownAvversita(){
    let vegCod = this.gridPublicService.formGroup.value.controls['Veg_Cod'].value;
    let dpiCod = this.gridPublicService.formGroup.value.controls['DPI_COD'].value;
    let dpiFlagPrivatoPubblico = this.gridPublicService.formGroup.value.controls['DPI_FlagPrivatoPubblico'].value;
    let idRcdpi = this.gridPublicService.formGroup.value.controls['IdRcdpi'].value;

    if (vegCod != null){
      const payload = {
        lavCod: DEFAULT_LAV_CODE,
        vegCod: vegCod,
        dpiCod: dpiCod,
        idRcdpi: idRcdpi?.toString() ?? '0',
        dpiPubblicoPrivato: dpiFlagPrivatoPubblico?.toString() ?? '0',
        personalizzate: false,
      } as PopolaRilievo;

      return this.agendaClient.agendaPopolaRilievo(payload).pipe(
        tap((x: RispostaStandard_1OfString) => (this.avversitaList = JSON.parse(x.RispostaStringa) as Avversita[])),
        map((x: RispostaStandard_1OfString) => (JSON.parse(x.RispostaStringa) as Avversita[]))
      );

    } else {
      return from([]);
    }
  }

  read(options?: any): Observable<AnagraficheRilieviAvversitaResult> {

    if (this.read$ != null) {
      return this.read$;
    }

    this.read$ = this.reload$.pipe(
      startWith(null),
      tap(() => this.isLoading(true)),
      debounceTime(100),
      switchMap(() => this.agendaClient.agendaLeggiMisurePerAvversitaAnagraficheExtended()),
      switchMap(data => {

        // Extract distinct veg_cod and corresponding veg_des values
        const specieList = new Map<number, string>();
        data.RispostaStringa.ListaMisure.forEach(item => {
          if (!specieList.has(item.Veg_Cod)) {
            specieList.set(item.Veg_Cod, item.Veg_Des);
          }
        });

        // Create an array of API call observables
        
        const apiCalls = Array.from(specieList.entries()).map(([vegCod, vegDes]) => {
          const payload = {
            data: new Date(),
            privato: true,
            specie: {
              codice: vegCod,
              descrizione: vegDes
            },
            regolamento: null,
            lavorazioni: AVVERSITA_LAVORAZIONI
          } as LeggiDisciplinari;

          return this.modelloClient.modelloLeggiDisciplinariTestataConRegolamentoConcimazione(payload).pipe(
            map(response => ({
              vegCod,
              dpiData: response.RispostaStringa 
            }))
          );
        });

        if (apiCalls.length === 0) {
          return of(new AnagraficheRilieviAvversitaResult(data.RispostaStringa.ListaMisure, this.columns, this.anagraficheRilieviAvversitaGridModel));
        }
        // Execute all API calls in parallel
        return forkJoin(apiCalls).pipe(
          map(results => {
            // Create a map of vegCod to dpiData
            const dpiDataMap = new Map(results.map(result => [result.vegCod, result.dpiData]));

            data.RispostaStringa.ListaMisure.forEach(item => {
              const dpiData = dpiDataMap.get(item.Veg_Cod);
              if (dpiData) {
                const dpiItem = dpiData.find(dpi => dpi.codice === item.DPI_COD.toString());
                if (dpiItem) {
                  item.DPI_Des = dpiItem.descrizione;
                  item.IdRcdpi = dpiItem.raggruppamentiColturaliDPI?.codice;
                }
              }
            });
            return new AnagraficheRilieviAvversitaResult(data.RispostaStringa.ListaMisure, this.columns, this.anagraficheRilieviAvversitaGridModel);
          }));

        // return of(new AnagraficheRilieviAvversitaResult(data.RispostaStringa.ListaMisure, this.columns, this.anagraficheRilieviAvversitaGridModel));
    }),
      tap(() => this.isLoading(false)),
      share()
    );

    return this.read$;
  }


  perform(actionType: HttpAction, row: MisuraPerAvversitaAnagraficaExtended): Observable<KendoGridRow[]> {
    of([row])
    .pipe(
      map((rows) => this.getPayload(rows, actionType)),
      switchMap(payload => this.agendaClient.agendaMisurePerAvversitaAnagrafiche(payload)),
      catchError(res => of({ Errore: res.toString() } as RispostaStandard))
    )
    .subscribe({
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

  private getPayload(rows: MisuraPerAvversitaAnagraficaExtended[], actionType: HttpAction): MisuraPerAvversitaAnagrafica_In {

    if (actionType === HttpAction.CREATE) {
      for (const row of rows) {

        const avversita = this.avversitaList.find(x => x.cod === row.Av_Cod.toString());
        const disciplinare = this.disciplinariList.find(x => x.codice === row.DPI_COD.toString());
        row.CodiceMisura ??= +avversita?.MxAV_Cod;
        row.CodiceAnagrafica ??= 0;
        row.cancellabile ??= true;
        row.modificabile ??= true;
        row.DPI_FlagPrivatoPubblico ??= disciplinare?.disciplinarePubblicoPrivato ?? 0;
        row.DPI_COD ??= +(disciplinare?.codice?.split("/")[0] ?? '0')
      }
    }

    let payload: MisuraPerAvversitaAnagrafica_In;
    switch (actionType) {
      case HttpAction.CREATE:
        payload = { MisureInsert: rows };
        break;
      case HttpAction.UPDATE:
        payload = { MisureUpdate: rows };
        break;
      case HttpAction.REMOVE:
        payload = { MisureDelete: rows };
        break;
    }

    return payload;
  }

}