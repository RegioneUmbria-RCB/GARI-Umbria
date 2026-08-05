import { Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  DatiPrevisionaliColtureKendoServerResult, KendoDatiPrevisionaliColtureModel
} from './dati-previsionali-colture-grid.model';
import { CELL_TYPES } from 'gias-ui-kit';
import { DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import { from, Observable, of, Subscription } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { ConfigTemplate } from 'gias-kendo-grid';
import {
  AgrSelectableSettings,
  CommandsColumnSettings,
  RemoveMultipleRowsParams,
  ToolbarSettings
} from 'gias-kendo-grid';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from '../../Model/CostantiPersonalizzate';
import { DatiPrevisionaliService } from '../../Service/DatiPrevisionali/dati-previsionali.service';
import { map, switchMap, tap } from 'rxjs/operators';
import { DatiPrevisionaliColtureComplete, DatiPrevisionaliColtureRequest } from '../../Model/anagrafiche/DatiPrevisionaliColture';
import { SpecieVegetaliService } from '../../Service/Metaschema/specie-vegetali.service';
import { VarietaService } from '../../Service/Metaschema/varieta.service';
import { GruppoFinalitaService } from '../../Service/Metaschema/finalita.service';
import { toInteger } from 'lodash';
import { GruppoVarietaleService } from '../../Service/Metaschema/gruppoVarietale.service';
import { FaseCicloColturale } from '../../Model/metaschema/fase';
import { Specie } from '../../Model/metaschema/utilizzi/Specie';
import { GruppoFinalita } from '../../Model/metaschema/utilizzi/GruppoFinalita';
import { Vincolo } from '../../Model/metaschema/Vincoli';
import { FiltroSpecieFinalita } from '../../Model/filtri/filtroSpecieFinalita';
import { IntervalloTemporale } from '../../Model/anagrafiche/IntervalloTemporale';
import { VincoliService } from '../../Service/DPI/vincoli.service';
import { Disciplinare } from '../../Model/metaschema/Disciplinari';
import { Regolamenti } from '../../Model/metaschema/Regolamenti';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Provincia } from '../../Model/MetaschemaModel';
import { CodiciNazioniISO3166 } from '../../Model/metaschema/CodiciNazioniISO3166';
import { PortinnestoService } from '../../Service/Metaschema/portinnesto.service';
import { FormaAllevamentoService } from '../../Service/Metaschema/formaAllevamento.service';
import { UnitaDiMisuraService } from '../../Service/Metaschema/UnitaDiMisura.service';
import { FormGroup, Validators } from '@angular/forms';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { enum_UnitaMisura } from '../../Model/TipiEnumerativi';
import { Varieta } from '../../Model/metaschema/utilizzi/Varieta';
import { GruppoVarietale } from '../../Model/metaschema/GruppoVarietale';
import { Portinnesto } from '../../Model/metaschema/DensitaImpianto/Portinnesto';
import { FormaAllevamento } from '../../Model/metaschema/DensitaImpianto/FormaAllevamento';
import { CodificaInfoAggiuntiveService } from '../../Service/Codifiche/codifica_InfoAggiuntive.service';
import {GiasIstatService} from '../../Service/istat/gias-istat.service';

@Injectable()
export class DatiPrevisionaliColtureGridConfigurationService extends AbstractGridConfigService<DatiPrevisionaliColtureKendoServerResult> {
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  gridId: string = 'dati-previsionali-colture-grid';
  rowId: string = 'Id';
  private objParametriAgenda: ObjParametriAgenda;
  private columnSubs: Subscription[] = [];

  private readonly RESA_PREVISTA_COD = 4;
  private readonly NESSUNO = 1;
  private readonly BIO = 4;

  private parametroDdlData: DropdownListItem[] = [
    { id: this.RESA_PREVISTA_COD, name: this.translocoService.translate('ResaPrevista') }
  ];

  private vincoliDdlData: DropdownListItem[] = [
    { id: this.NESSUNO, name: this.translocoService.translate('Nessuno') },
    { id: this.BIO, name: this.translocoService.translate('Bio') }
  ];

  private kendoModel: KendoDatiPrevisionaliColtureModel = {
    Piva: new ModelEntry(CELL_TYPES.STRING, false),
    Id: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    veg_des: new ModelEntry(CELL_TYPES.STRING, false),
    Cul_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    cul_des: new ModelEntry(CELL_TYPES.STRING, false),
    Codice: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    parametro_des: new ModelEntry(CELL_TYPES.STRING, true),
    Valore: new ModelEntry(CELL_TYPES.NUMBER, true),
    grfi_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    grfi_des: new ModelEntry(CELL_TYPES.STRING, false),
    grva_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    grva_des: new ModelEntry(CELL_TYPES.STRING, false),
    dettSpeciePersonalizzatoCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    dettSpeciePersonalizzatoDes: new ModelEntry(CELL_TYPES.STRING, false),
    stato_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    stato_des: new ModelEntry(CELL_TYPES.STRING, true),
    codice_stato: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    codice_stato_des: new ModelEntry(CELL_TYPES.STRING, true),
    reg: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Regione_Des: new ModelEntry(CELL_TYPES.STRING, true),
    prov: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    PROVINCIA: new ModelEntry(CELL_TYPES.STRING, true),
    port_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    port_des: new ModelEntry(CELL_TYPES.STRING, false),
    foral_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    foral_des: new ModelEntry(CELL_TYPES.STRING, false),
    reg_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    regolamento_des: new ModelEntry(CELL_TYPES.STRING, true),
    udm_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    udm_des: new ModelEntry(CELL_TYPES.STRING, true),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true)
  };

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Veg_Cod', title: this.translocoService.translate('Specie') },
      { resizable: true, filterable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Cul_Cod', title: this.translocoService.translate('Varietà') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Codice', title: this.translocoService.translate('Parametro') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Valore', title: this.translocoService.translate('valore') },
      { resizable: true, editable: true, width: 135, numeric: { defaultValue: 0, min: 0, format: 'n4' } }
    ),
    new KendoGridColumn(
      { field: 'udm_cod', title: this.translocoService.translate('Unita_Misura') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'grfi_cod', title: this.translocoService.translate('Finalità') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'grva_cod', title: this.translocoService.translate('TipologiaVarietale') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'dettSpeciePersonalizzatoCod', title: this.translocoService.translate('DettaglioVarietaPersonalizzato') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'stato_cod', title: this.translocoService.translate('StatoImpianto') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'codice_stato', title: this.translocoService.translate('Stato') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'reg', title: this.translocoService.translate('Regione') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'prov', title: this.translocoService.translate('Provincia') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'port_cod', title: this.translocoService.translate('Portinnesto') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'foral_cod', title: this.translocoService.translate('FormaAllevamento') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'reg_cod', title: this.translocoService.translate('Regolamento') },
      { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
      { resizable: true, date: { defaultValue: AGRODATAINIZIO }, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
      { resizable: true, date: { defaultValue: AGRODATAFINE }, editable: true, width: 135 }
    )
  ];

  constructor(
    injector: Injector,
    private translocoService: TranslocoService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private datiPrevisionaliColtureService: DatiPrevisionaliService,
    private specieVegetaliService: SpecieVegetaliService,
    private varietaService: VarietaService,
    private gruppoFinalitaService: GruppoFinalitaService,
    private gruppoVarietaleService: GruppoVarietaleService,
    private vincoliService: VincoliService,
    private istatService: GiasIstatService,
    private portinnestoService: PortinnestoService,
    private formaAllevamentoService: FormaAllevamentoService,
    private unitaDiMisuraService: UnitaDiMisuraService,
    private sharedDataService: SharedDataService,
    private codificaInfoAggiuntiveService: CodificaInfoAggiuntiveService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.handleCustomizations();

    this.configValueChangesBehavior();

    this.gridPublicService.formGroup.subscribe(fb => {
      // se disponibile un solo parametro, imposto quello di default
      if (this.getParametroDdlData().length === 1) {
        fb?.controls['Codice'].setValue(this.getParametroDdlData()[0].id);
      }
    });

    this.gridPublicService.changeDetected.subscribe((event: any) => {
      if (event?.action === 'add') {
        // imposto default del vincolo a 'Nessuno'
        const fb: FormGroup<any> = this.gridPublicService.formGroup.getValue();
        fb?.controls['reg_cod'].setValue(this.getVincoliDdlData()[0].id);
      }
    });
  }

  perform(actionType: HttpAction, item: any): Observable<any> {
    switch (actionType) {
      case HttpAction.REMOVE:
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        return this.datiPrevisionaliColtureService.deleteDatiPrevisionaliColture(this.mapRowToDatiPrevisionali(item)).pipe(
          tap((r) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
          }),
          map(r => [])
        );
      case HttpAction.CREATE:
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        return this.datiPrevisionaliColtureService.createDatiPrevisionaliColture(this.mapRowToDatiPrevisionali(item)).pipe(
          tap((r) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
          }),
          map(r => [])
        );
      case HttpAction.UPDATE:
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        return this.datiPrevisionaliColtureService.editDatiPrevisionaliColture(this.mapRowToDatiPrevisionali(item)).pipe(
          tap((r) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
          }),
          map(r => [])
        );
    }
  }

  read(options: any): Observable<DatiPrevisionaliColtureKendoServerResult> {
    let params: DatiPrevisionaliColtureRequest = new DatiPrevisionaliColtureRequest();
    return this.datiPrevisionaliColtureService.readDatiPrevisionaliColtureDT(params).pipe(
      map(r => {
        this.handleDropdowns();
        return new DatiPrevisionaliColtureKendoServerResult(this.kendoModel, this.kendoColumns, r.map(row => {
          row.Valore = Number.parseFloat((<string>row.Valore).replace(',', '.'));
          return row;
        }));
      })
    );
  }

  private getParametroDdlData(): DropdownListItem[] {
    return this.parametroDdlData;
  }

  private getVincoliDdlData(): DropdownListItem[] {
    return this.vincoliDdlData;
  }

  private handleDropdowns(): void {
    let col: KendoGridColumn;

    col = this.kendoColumns.find(s => s.field === 'Veg_Cod');
    col.ddl = new DropdownListWithForm('codice', 'Veg_Cod', 'descrizione', []);
    col.validators = [Validators.required];
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'veg_des';
    col.ddl.loadFunction = this.caricaSpecieVegetali.bind(this);

    col = this.kendoColumns.find(s => s.field === 'Cul_Cod');
    col.ddl = new DropdownListWithForm('codice', 'Cul_Cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'cul_des';
    col.ddl.loadFunction = this.caricaCultivar.bind(this);

    col = this.kendoColumns.find(s => s.field === 'Codice');
    col.ddl = new DropdownListWithForm('id', 'Codice', 'name', this.getParametroDdlData());
    col.validators = [Validators.required];
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = false;
    col.ddl.descriptionField = 'parametro_des';

    col = this.kendoColumns.find(s => s.field === 'grfi_cod');
    col.ddl = new DropdownListWithForm('codice', 'grfi_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'grfi_des';
    col.ddl.loadFunction = this.caricaFinalita.bind(this);

    col = this.kendoColumns.find(s => s.field === 'grva_cod');
    col.ddl = new DropdownListWithForm('codice', 'grva_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'grva_des';
    col.ddl.loadFunction = this.caricaGruppoVarietale.bind(this);

    col = this.kendoColumns.find(s => s.field === 'stato_cod');
    col.ddl = new DropdownListWithForm('codice', 'stato_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'stato_des';
    col.ddl.loadFunction = this.caricaStatoImpianto.bind(this);

    col = this.kendoColumns.find(s => s.field === 'reg');
    col.ddl = new DropdownListWithForm('codice', 'reg', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Regione_Des';
    col.ddl.loadFunction = this.caricaRegioni.bind(this);

    col = this.kendoColumns.find(s => s.field === 'prov');
    col.ddl = new DropdownListWithForm('Istat_Prov', 'prov', 'Provincia_Des', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'PROVINCIA';
    col.ddl.loadFunction = this.caricaProvincie.bind(this);

    col = this.kendoColumns.find(s => s.field === 'codice_stato');
    col.ddl = new DropdownListWithForm('codice', 'codice_stato', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'codice_stato_des';
    col.ddl.loadFunction = this.caricaStati.bind(this);

    col = this.kendoColumns.find(s => s.field === 'port_cod');
    col.ddl = new DropdownListWithForm('codice', 'port_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'port_des';
    col.ddl.loadFunction = this.caricaPortinnesto.bind(this);

    col = this.kendoColumns.find(s => s.field === 'foral_cod');
    col.ddl = new DropdownListWithForm('codice', 'foral_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'foral_des';
    col.ddl.loadFunction = this.caricaFormaAllevamento.bind(this);

    col = this.kendoColumns.find(s => s.field === 'reg_cod');
    col.ddl = new DropdownListWithForm('id', 'reg_cod', 'name', this.getVincoliDdlData());
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = false;
    col.ddl.descriptionField = 'regolamento_des';
    // col.ddl.loadFunction = this.caricaVincoli.bind(this);

    col = this.kendoColumns.find(s => s.field === 'udm_cod');
    col.ddl = new DropdownListWithForm('codice', 'udm_cod', 'descrizione', []);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'udm_des';
    col.ddl.loadFunction = this.caricaUnitaDiMisura.bind(this);

    col = this.kendoColumns.find(s => s.field === 'dettSpeciePersonalizzatoCod');
    col.ddl = new DropdownListWithForm('codice', 'dettSpeciePersonalizzatoCod', 'descrizione', []);
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'dettSpeciePersonalizzatoDes'
    col.ddl.loadFunction = this.loadDettaglioSpeciePersonalizzato.bind(this);
  }

  private caricaSpecieVegetali(dataItem: any) {
    return from(this.specieVegetaliService.leggi_FiltroUtente()).pipe(map((val) => {
      if (!(val.findIndex((sp) => { return sp.codice == 0 }) >= 0)) {
        val = [{ codice: 0, descrizione: '' }, ...val];
      }
      return val;
    }));
  }

  private caricaCultivar(dataItem: any) {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0 && dataItem.Veg_Cod != 'null') {
      return this.varietaService.leggiAsObs({ codice: dataItem.Veg_Cod, descrizione: '' }).pipe(map((val) => {
        if (!(val.findIndex((v) => { return v.codice == 0 }) >= 0)) {
          let empty = new Varieta();
          empty.codice = 0;
          empty.descrizione = '';
          val = [empty, ...val];
        }
        return val;
      }));
    } else {
      return of([]);
    }
  }

  private caricaFinalita(dataItem: any) {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0) {
      return from(this.gruppoFinalitaService.leggi({ codice: dataItem.Veg_Cod, descrizione: '' }, this.sharedDataService?.getCfgSementiAsValue())).pipe(
        map(val => {
          if (!(val.findIndex((v) => { return v.codice == 0; }) >= 0)) {
            let empty = new GruppoFinalita(0);
            val = [empty, ...val];
          }
          return val;
        })
      );
    } else {
      return of([{ codice: 0, descrizione: '' }]);
    }
  }

  private caricaGruppoVarietale(dataItem: any) {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0) {
      return from(this.gruppoVarietaleService.leggi({ codice: toInteger(dataItem.Veg_Cod), descrizione: dataItem.veg_des })).pipe(
        map(val => {
          if (!(val.findIndex((v) => { return v.codice == 0; }) >= 0)) {
            let empty = new GruppoVarietale(0);
            val = [empty, ...val];
          }
          return val;
        })
      );
    } else {
      return of([{ codice: 0, descrizione: '' }]);
    }
  }

  private caricaStatoImpianto(dataItem: any): Observable<FaseCicloColturale[]> {
    let specie: Specie = {
      codice: dataItem.Veg_Cod,
      descrizione: dataItem.veg_des
    };
    let finalita: GruppoFinalita = {
      specieCod: dataItem.Veg_Cod,
      codice: dataItem.grfi_cod,
      descrizione: dataItem.grfi_des
    };
    let filtro: FiltroSpecieFinalita = {
      specie: specie,
      finalita: finalita
    };
    return this.gruppoFinalitaService.Leggi_FasiCicloColturalexSpecie(filtro).pipe(
      switchMap((fasi) => {
        if (specie && specie.codice > 0) {
          let fase102 = fasi.find((el) => { return el.codice == 102 });
          if (fase102 == undefined) {
            fasi.push({
              codice: 102,
              descrizione: 'In produzione',
              disciplinarePubblicoPrivato: true
            });
          }
        }
        if (!(fasi.findIndex((v) => { return v.codice == 0; }) >= 0)) {
          let empty = new FaseCicloColturale(0);
          fasi = [empty, ...fasi];
        }
        return of(fasi);
      })
    )
  }

  private leggiVincoloDaCodice(codice: string): Observable<Vincolo> {
    let validita: IntervalloTemporale = new IntervalloTemporale();
    return this.vincoliService.leggiVincoli(validita).pipe(
      switchMap((vincoli: Vincolo[]) => {
        let vincoloSelected = vincoli.find((el) => { return el.codice == codice })
        return of(vincoloSelected);
      })
    )
  }

  private caricaVincoli(dataItem: any): Observable<Vincolo[]> {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0) {
      let validita: IntervalloTemporale = new IntervalloTemporale(dataItem.Validita_Inizio, dataItem.Validita_Fine);
      return this.vincoliService.leggiVincoli(validita);
    } else {
      let vincoloDefault = new Vincolo('1');
      let disciplinare: Disciplinare = new Disciplinare('0');
      let regolamento: Regolamenti = new Regolamenti(1);
      vincoloDefault.descrizione = 'Nessuno';
      vincoloDefault.disciplinare = disciplinare;
      vincoloDefault.regolamento = regolamento;
      return of([vincoloDefault]);
    }
  }

  private caricaProvincie(dataItem: any): Observable<Provincia[]> {
    return from(this.istatService.leggiProvincie(dataItem.codice_stato, dataItem.reg ?? '')).pipe(
      map(val => {
        if (!(val.findIndex((v) => { return v.Istat_Prov == '000'; }) >= 0)) {
          let empty = new Provincia();
          empty.Istat_Prov = '000';
          val = [empty, ...val];
        }
        return val;
      })
    );
  }

  private caricaStati(dataItem: any): Observable<CodiciNazioniISO3166[]> {
    return from(this.istatService.leggiStati()).pipe(
      map(val => {
        if (!(val.findIndex((v) => { return v.codice == ''; }) >= 0)) {
          let empty = new CodiciNazioniISO3166('', '', '', '', 0);
          val = [empty, ...val];
        }
        return val;
      })
    );
  }

  private caricaRegioni(dataItem: any) {
    return this.istatService.readRegioni(dataItem.codice_stato);
  }

  private caricaPortinnesto(dataItem: any) {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0) {
      return from(this.portinnestoService.leggi({ codice: toInteger(dataItem.Veg_Cod), descrizione: dataItem.veg_des })).pipe(
        map(val => {
          if (!(val.findIndex((v) => { return v.codice == 0; }) >= 0)) {
            let empty = new Portinnesto();
            empty.codice = 0;
            val = [empty, ...val];
          }
          return val;
        })
      );
    } else {
      return of([{ codice: 0, descrizione: '' }]);
    }
  }

  private caricaFormaAllevamento(dataItem: any) {
    if (dataItem.Veg_Cod != null && dataItem.Veg_Cod != 0) {
      return from(this.formaAllevamentoService.leggi({ codice: toInteger(dataItem.Veg_Cod), descrizione: dataItem.veg_des })).pipe(
        map(val => {
          if (!(val.findIndex((v) => { return v.codice == 0; }) >= 0)) {
            let empty = new FormaAllevamento();
            empty.codice = 0;
            val = [empty, ...val];
          }
          return val;
        })
      );
    } else {
      return of([{ codice: 0, descrizione: '' }]);
    }
  }

  private caricaUnitaDiMisura(dataItem: any) {
    return this.unitaDiMisuraService.Leggi_Tutte_UnitaDiMisura().pipe(
      map(udms => {
        // Se siamo in inserimento Resa Prevista, allora le udm ammesse sono kg su ettaro e quintali su ettaro
        if (dataItem['Codice'] === this.RESA_PREVISTA_COD)
          return udms.filter(udm => udm.codice === enum_UnitaMisura.KG__HA || udm.codice === enum_UnitaMisura.QUINTALI__HA);
        else
          return udms;
      })
    );
  }

  private loadDettaglioSpeciePersonalizzato(dataItem: any) {
    let objParams: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    objParams.Data = new Date();
    return from(this.codificaInfoAggiuntiveService.leggiDettaglioVarietaPersonalizzato());
  }

  private handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = true;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      removeBtn: true,
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    this.groups.groupable.enabled = false;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = false;
      this.cmdColumn.editBtn = false;
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
    }
  }

  private configValueChangesBehavior(): void {
    this.gridPublicService.changeDetected.subscribe((event: any) => {
      if (event?.action === 'edit' || event?.action === 'add') {
        this.setColumnSubscriptions();
      } else if (event.action === 'cancel' || event.action === 'save') {
        this.columnSubs.forEach(s => s.unsubscribe());
        this.columnSubs = [];
      }
    });
  }

  private setColumnSubscriptions(): void {
    const fb: FormGroup<any> = this.gridPublicService.formGroup.getValue();
    this.columnSubs.push(
      fb.get('codice_stato').valueChanges.subscribe((val) => {
        fb.get('reg').setValue('000');
        fb.get('Regione_Des').setValue('');
        let col = this.kendoColumns.find(s => s.field === 'reg');
        col.ddl.reload.next(true);
        console.log(val);
      }),
      fb.get('reg').valueChanges.subscribe((val) => {
        fb.get('prov').setValue('000');
        fb.get('PROVINCIA').setValue('');
        let col = this.kendoColumns.find(s => s.field === 'prov');
        col.ddl.reload.next(true);
      }),
      fb.get('Veg_Cod').valueChanges.subscribe((val) => {
        fb.get('Cul_Cod').setValue(0);
        fb.get('cul_des').setValue('');
        let col = this.kendoColumns.find(s => s.field === 'Cul_Cod');
        col.ddl.reload.next(true);
        console.log(val);
        fb.get('grfi_cod').setValue(0);
        fb.get('grva_cod').setValue(0);
        fb.get('stato_cod').setValue(0);
        fb.get('port_cod').setValue(0);
        fb.get('foral_cod').setValue(0);
      })
    );
  }

  private mapRowToDatiPrevisionali(item: any): DatiPrevisionaliColtureComplete {
    let d: DatiPrevisionaliColtureComplete = new DatiPrevisionaliColtureComplete();
    d.piva = item.Piva ?? this.objParametriAgenda.Piva;
    d.id = item.Id ?? 0;
    d.vegCod = item.Veg_Cod;
    d.culCod = item.Cul_Cod ?? 0;
    d.parametroCod = item.Codice;
    d.valore = item.Valore ?? 0;
    d.grfiCod = item.grfi_cod ?? 0;
    d.grvaCod = item.grva_cod ?? 0;
    d.statoCod = item.stato_cod ?? 0;
    d.dettSpeciePersonalizzatoCod = item.dettSpeciePersonalizzatoCod ?? '';
    d.codiceStato = item.codice_stato ?? 0;
    d.reg = item.reg ?? '';
    d.prov = item.prov ?? '';
    d.portCod = item.port_cod ?? 0;
    d.foralCod = item.foral_cod ?? 0;
    d.regCod = item.reg_cod ?? 1;
    d.udm = item.udm_cod ?? (d.parametroCod == this.RESA_PREVISTA_COD ? enum_UnitaMisura.KG__HA : 0);   /*solo in caso di inserimento ResaPrevista imposto default udm KG__HA*/
    d.validitaInizio = item.Validita_Inizio ?? AGRODATAINIZIO;
    d.validitaFine = item.Validita_Fine ?? AGRODATAFINE;
    return d;
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;
    msg = this.translocoService.translate('datiPrevisionaliColture.DeleteMsg')
    opts.data.forEach(r => {
      msg = msg.concat('<br>' + ' ' + r['veg_des'] + ' - ' + r['parametro_des'] + ': ' + r['Valore'] + (r['udm_cod'] === 0 ? '' : (' ' + r['udm_des'])));
    })
    return msg;
  }

}
