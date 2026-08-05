import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import {
  AggregateSettings,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  RemoveMultipleRowsParams,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  GridCustomizations,
  JsonKendoResult,
  KendoGridColumn,
  ModelEntry,
  LoaderType,
  RendererGridEvent
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { forkJoin, from, Observable, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { catchError, map, switchMap, take, tap } from 'rxjs/operators';
import { Comune, Provincia } from 'app/Model/MetaschemaModel';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CatastoKendoServerResult, KendoCatastoModel } from './catasto.models';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentriAziendaliService, LeggiCentriAziendali } from 'app/Service/Anagrafica/centri.service';
import { AbstractControl, Validators } from '@angular/forms';
import { CatastoGridEventsService } from './catasto-grid-events.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { AnagraficaService } from '../anagrafica.service';
import { TranslocoService } from '@jsverse/transloco';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { CatastoFactoryService, CATASTO_SERVICE_TOKEN } from 'app/Service/ServiceFactory/catasto.factory.service';
import { BudgetService } from 'app/Service/Budget/budget.service';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { GridCommandItem } from 'app/menu-agenda/components/utils';
import { faLeaf } from '@fortawesome/free-solid-svg-icons';
import {GiasIstatService} from '../../Service/istat/gias-istat.service';

const InvestimentoCatastale: number = -1;

export function forbiddenComuneValidator(control: AbstractControl) {
  if (control.value == undefined || control.value == '000' || control.value == '') {
    return { 'Com': true };
  }
  return null;
}

@Injectable()
export class CatastoHttpService extends AbstractGridConfigService<CatastoKendoServerResult> {
  gridId = 'CatastoHttpService';
  loader: LoaderType = LoaderType.SERVICE;

  editingMode: EditingMode = EditingMode.IN_LINE;
  isNew: boolean;
  rowId: string = 'chiave';
  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });

  objParametriAgenda: ObjParametriAgenda;

  public provincie: Provincia[] = new Array<Provincia>();
  toolbar = new ToolbarSettings(true, true);
  views = new GridCustomizations({ enabled: true });

  aggregates = new AggregateSettings({
    enabled: true,
    descriptors: [
      { field: 'Sup_Catastale', aggregate: 'sum', format: 'n4' },
      { field: 'Sup_Condotta', aggregate: 'sum', format: 'n4' }
    ]
  }
  );

  kendoModel: KendoCatastoModel = {
    Codice: new ModelEntry(CELL_TYPES.NUMBER, false),
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    Sa_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Sa_Nome: new ModelEntry(CELL_TYPES.STRING, false),
    Prov: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    PROVINCIA: new ModelEntry(CELL_TYPES.STRING, true),
    Com: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    COMUNE: new ModelEntry(CELL_TYPES.STRING, true),
    SEZIONE: new ModelEntry(CELL_TYPES.STRING, true),
    FOGLIO: new ModelEntry(CELL_TYPES.NUMBER, true, null, [Validators.required, Validators.min(1)]),
    NUMERO: new ModelEntry(CELL_TYPES.NUMBER, true, null, [Validators.required, Validators.min(1)]),
    SUBALTERNO: new ModelEntry(CELL_TYPES.STRING, true),
    Sup_Catastale: new ModelEntry(CELL_TYPES.NUMBER, true, null, [Validators.required, Validators.min(0.0001)]),
    Sup_Condotta: new ModelEntry(CELL_TYPES.NUMBER, true, null, [Validators.required, Validators.min(0.0001)]),
    Titolo_Possesso_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Titolo_possesso: new ModelEntry(CELL_TYPES.STRING, true),
    MetodoProduzione_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    MetodoProduzione_Des: new ModelEntry(CELL_TYPES.STRING, true),
    ZVN: new ModelEntry(CELL_TYPES.STRING, true),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
    cod_particella: new ModelEntry(CELL_TYPES.STRING, false),
    Attivo: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Proprietario: new ModelEntry(CELL_TYPES.STRING, false),
  };

  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Sa_Cod', title: this.translocoService.translate('Centro') },
      { resizable: true, editable: true, validators: [Validators.required], media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Prov', title: this.translocoService.translate('Provincia') },
      { resizable: true, editable: true, validators: [Validators.required], width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Com', title: this.translocoService.translate('Comune') },
      { resizable: true, editable: true, validators: [Validators.required, forbiddenComuneValidator], width: 120 }
    ),
    new KendoGridColumn(
      { field: 'SEZIONE', title: this.translocoService.translate('SezioneAbbr') },
      { resizable: true, editable: true, width: 80, validators: [Validators.maxLength(2)] }
    ),
    new KendoGridColumn(
      { field: 'FOGLIO', title: this.translocoService.translate('FoglioAbbr') },
      { resizable: true, editable: true, numeric: { min: 0, format: 'n0', multiCheckFiltering: true }, validators: [Validators.required, Validators.min(1)], width: 80 }
    ),
    new KendoGridColumn(
      { field: 'NUMERO', title: this.translocoService.translate('NumeroAbbr') },
      { resizable: true, editable: true, numeric: { min: 0, format: 'n0', multiCheckFiltering: true }, validators: [Validators.required, Validators.min(1)], width: 80 }
    ),
    new KendoGridColumn(
      { field: 'SUBALTERNO', title: this.translocoService.translate('SubalternoAbbr') },
      { resizable: true, editable: true, width: 80, validators: [Validators.maxLength(3)] }
    ),
    new KendoGridColumn(
      { field: 'Titolo_Possesso_Cod', title: this.translocoService.translate('TitoloDiPossesso') },
      { resizable: true, editable: true, validators: [Validators.required], media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 160 }
    ),
    new KendoGridColumn(
      { field: 'MetodoProduzione_Cod', title: this.translocoService.translate('MetodoProduzione') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 160 }
    ),
    new KendoGridColumn(
      { field: 'Sup_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') },
      { resizable: true, editable: true, numeric: { min: 0, format: 'n4' }, validators: [Validators.required, Validators.min(0.0001)], media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Sup_Condotta', title: this.translocoService.translate('SuperficieCondottaAbbr') },
      { resizable: true, editable: true, numeric: { min: 0, format: 'n4' }, validators: [Validators.required, Validators.min(0.0001)], width: 120 }
    ),
    new KendoGridColumn(
      { field: 'cod_particella', title: this.translocoService.translate('CodiceParticella') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 160 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
      { resizable: true, editable: true, date: { defaultValue: AGRODATAINIZIO }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
      { resizable: true, editable: true, date: { defaultValue: AGRODATAFINE }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'ZVN', title: this.translocoService.translate('ZVN') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Attivo', title: this.translocoService.translate('Attivo') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Proprietario', title: this.translocoService.translate('Proprietario') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
  ];

  constructor(
    injector: Injector,
    private objParametriAgendaService: ObjParametriAgendaService,
    private istatService: GiasIstatService,
    @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private centriService: CentriAziendaliService,
    private catastoGridEventsService: CatastoGridEventsService,
    private renderer: Renderer2,
    private anagraficaService: AnagraficaService,
    private permessiUtenteService: PermessiUtenteService,
    private translocoService: TranslocoService,
    private budgetService: BudgetService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();
    this.handleCommands();

    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case CommandsDropDownEvents.FULL_EDIT:
          this.catastoGridEventsService.onTemplateBtnClick(ev.dataItem);
          break;
        case InvestimentoCatastale:
          this.catastoGridEventsService.onInvestimentoCatastale(ev.dataItem);
          break;
      }
    })

    this.isNew = false
    // gestore ddl campi chiave

    this.gridPublicService.changeDetected.pipe(
      tap((event: any) => {
        const fb = this.gridPublicService.formGroup.getValue();
        if (event?.action != 'info' && event?.action != 'remove') {
          fb.controls["Titolo_Possesso_Cod"].disable({ emitEvent: false })
          fb.controls["Sup_Condotta"].disable({ emitEvent: false })
          fb.controls["Validita_Inizio"].disable({ emitEvent: false })
          fb.controls["Validita_Fine"].disable({ emitEvent: false })
        }

        if ((event?.action) != 'add') {
          this.isNew = false
        } else {
          this.isNew = true
        }

        if (event?.action === 'edit') {
          fb.controls["Sa_Cod"].disable({ emitEvent: false });
          fb.controls["Prov"].disable({ emitEvent: false });
          fb.controls["Com"].disable({ emitEvent: false });
          fb.controls["SEZIONE"].disable({ emitEvent: false });
          fb.controls["FOGLIO"].disable({ emitEvent: false });
          fb.controls["NUMERO"].disable({ emitEvent: false });
          fb.controls["SUBALTERNO"].disable({ emitEvent: false });
          const particella = this.catastoGridEventsService.preparePKPart(fb.getRawValue());
          this.catastoService.LeggiParticellaAzienda(particella).pipe(
            take(1),
            tap((particella) => {
              if (particella.particella.metodoProduzione.length > 1) {
                fb.controls["MetodoProduzione_Cod"].disable();
                fb.controls["MetodoProduzione_Des"].disable();
              }
            })
          ).subscribe();

          this.catastoService.checkPossessi(particella).pipe(take(1), tap((data) => {
            if (!data) {
              fb.controls["Titolo_Possesso_Cod"].enable({ emitEvent: false })
              fb.controls["Sup_Condotta"].enable({ emitEvent: false })
              fb.controls["Validita_Inizio"].enable({ emitEvent: false })
              fb.controls["Validita_Fine"].enable({ emitEvent: false })
            }
          })).subscribe();
        }
        if (event?.action === 'add') {
          fb.controls["Sa_Cod"].enable({ emitEvent: false });
          fb.controls["Prov"].enable({ emitEvent: false });
          fb.controls["Com"].disable({ emitEvent: false });
          fb.controls["SEZIONE"].enable({ emitEvent: false });
          fb.controls["FOGLIO"].enable({ emitEvent: false });
          fb.controls["NUMERO"].enable({ emitEvent: false });
          fb.controls["SUBALTERNO"].enable({ emitEvent: false });
          fb.controls["Titolo_Possesso_Cod"].enable({ emitEvent: false })
          fb.controls["Sup_Catastale"].enable({ emitEvent: false })
          fb.controls["Sup_Condotta"].enable({ emitEvent: false })
          fb.controls["Validita_Inizio"].enable({ emitEvent: false })
          fb.controls["Validita_Fine"].enable({ emitEvent: false })
          fb.controls["cod_particella"].enable({ emitEvent: false })
          this.caricaTitoliPossesso().pipe(
            take(1),
            tap((vals) => {
              fb.controls["Titolo_Possesso_Cod"].setValue(1);
              let col = this.kendoColumns.find(s => s.field === 'Titolo_Possesso_Cod');
              col.ddl.reload.next(true);
              col.ddl.data = vals.map((el) => { return new DropdownListItem(el.codice, el.descrizione, vals) })
            })
          ).subscribe();

          if (this.catastoService.ultimaParticellaInserita == null) {
            this.caricaCentri({}).pipe(
              take(1),
              tap((vals) => {
                if (vals.length == 1) {
                  fb.controls["Sa_Cod"].setValue(vals[0].codice);
                  fb.controls["Sa_Nome"].setValue(vals[0].descrizione);
                  let col = this.kendoColumns.find(s => s.field === 'Sa_Cod');
                  fb.controls["Com"].enable({ emitEvent: false });
                  col.ddl.reload.next(true);
                }
              })
            ).subscribe();
          } else {
            this.caricaCentri({}).pipe(
              take(1),
              tap((vals) => {
                let centro = vals.find((el) => { return el.codice == this.catastoService.ultimaParticellaInserita.sa_cod });
                if (centro != undefined && centro != null) {
                  fb.controls['Sa_Cod'].setValue(centro.codice);
                  fb.controls['Sa_Nome'].setValue(centro.descrizione);
                  let col = this.kendoColumns.find(s => s.field === 'Sa_Cod');
                  col.ddl.data = [{ id: centro.codice, name: centro.descrizione }]
                  col.ddl.reload.next(true);
                }
              })
            ).subscribe();
          }

          this.impreseService.impresaBiologica(this.objParametriAgenda.Piva).pipe(
            take(1),
            tap((val) => {
              if (val) {
                fb.controls["MetodoProduzione_Cod"].setValue(3);
                fb.controls["MetodoProduzione_Des"].setValue("Biologico");
              } else {
                fb.controls["MetodoProduzione_Cod"].setValue(1);
                fb.controls["MetodoProduzione_Des"].setValue("Integrato");
              }
              let col = this.kendoColumns.find(s => s.field === 'MetodoProduzione_Cod');
              col.ddl.reload.next(true);

            })
          ).subscribe();
        }

        if (event?.action === 'info') {
          this.catastoGridEventsService.infoCatasto(event)
        }

        if (event?.action === 'remove') {
        }

      })
    ).GiasSubscribe((event: any) => {
      let a = 0; //Commento per funzione vuota SonarQube
    });

    // gestore del change della ddl Provincia
    this.gridPublicService.formGroup.GiasSubscribe((fb) => {
      if (fb != undefined) {
        fb.controls["Prov"].valueChanges.GiasSubscribe(() => {
          let prov = fb.controls["Prov"].value;
          if (prov != null && prov != "" && prov != "000") {
            const formVal = fb.getRawValue();
            if (formVal.Codice <= 0) {
              fb.controls["Com"].enable();
            }
            this.istatService.leggiComuni(prov).then((comuni) => {
              let com = comuni.find(s => s.codice === '000');
              let col = this.kendoColumns.find(s => s.field === 'Com');
              fb.controls["Com"].setValue(com.codice);
              fb.controls["COMUNE"].setValue(com.descrizione);
              col.ddl.reload.next(true);
            })
          }
        });
        fb.controls["Sa_Cod"].valueChanges.GiasSubscribe(() => {
          let sa_cod = fb.controls["Sa_Cod"].value;
          if (sa_cod != null && sa_cod != "" && sa_cod != 0 && this.catastoService.ultimaParticellaInserita == null) {
            const objP = this.objParametriAgendaService.getObjParamValue();
            let impresa = new Impresa()
            impresa.partitaIva = this.objParametriAgenda.Piva;
            this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: this.objParametriAgenda.Validita_Inizio }, false)
              .then(centri => {
                let centro = centri.find(c => c.primaryKey.codice == fb.controls["Sa_Cod"].value);
                const indirizzo = centro.indirizzi[0].indirizzo;
                const prov = indirizzo.istatComune.prov;
                const com = indirizzo.istatComune.com;
                fb.controls["Prov"].setValue(prov, { emitEvent: false });
                let colProv = this.kendoColumns.find(s => s.field === 'Prov');
                colProv.ddl.reload.next(true);
                if (prov != '000') {
                  if (fb.getRawValue().Codice == null) {
                    fb.controls["Com"].setValue(com, { emitEvent: false });
                    fb.controls["Com"].enable({ emitEvent: false });
                    let colCom = this.kendoColumns.find(s => s.field === 'Com');
                    colCom.ddl.reload.next(true);
                  }
                }
              });
          } else if (this.catastoService.ultimaParticellaInserita != null) {
            let colCentro = this.kendoColumns.find(s => s.field === 'Sa_Cod');
            colCentro.ddl.reload.next(true);
            fb.controls["Prov"].setValue(this.catastoService.ultimaParticellaInserita.prov, { emitEvent: false });
            fb.controls['PROVINCIA'].setValue(this.catastoService.ultimaParticellaInserita.prov_des);
            let colProv = this.kendoColumns.find(s => s.field === 'Prov');
            colProv.ddl.data = [{ id: this.catastoService.ultimaParticellaInserita.prov, name: this.catastoService.ultimaParticellaInserita.prov_des }]
            colProv.ddl.reload.next(true);
            if (this.catastoService.ultimaParticellaInserita.prov != '000') {
              if (fb.getRawValue().Codice == null) {
                fb.controls["Com"].setValue(this.catastoService.ultimaParticellaInserita.com, { emitEvent: false });
                fb.controls['COMUNE'].setValue(this.catastoService.ultimaParticellaInserita.com_des);
                fb.controls["Com"].enable({ emitEvent: false });
                let colCom = this.kendoColumns.find(s => s.field === 'Com');
                colCom.ddl.data = [{ id: this.catastoService.ultimaParticellaInserita.com, name: this.catastoService.ultimaParticellaInserita.com_des }]
                colCom.ddl.reload.next(true);
              }
            }
            fb.controls["SEZIONE"].setValue(this.catastoService.ultimaParticellaInserita.sezione, { emitEvent: false });
            fb.controls["FOGLIO"].setValue(this.catastoService.ultimaParticellaInserita.foglio, { emitEvent: false });
          }
        });

        fb.controls["Sup_Catastale"].valueChanges.pipe(tap(
          val => {
            if (this.isNew) {
              if (!fb.get('Sup_Condotta').touched) {
                fb.get('Sup_Condotta').setValue(val);
              }
            }
          }
        )).GiasSubscribe(() => {
          let a = 0; //Commento per funzione vuota SonarQube
        });
      }
    });
  }

  read(): Observable<CatastoKendoServerResult> {
    let objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    let data = this.anagraficaService.filterData.getValue()
    objParametriAgenda.Data = AGRODATAINIZIO
    if (data.filter) {
      objParametriAgenda.Data = data.data;
    }
    // chiamo il warning se ritorno da una delete
    this.catastoGridEventsService.msgWarningDelete()
    // azzero Sa_Cod altrimenti non vengono lette tutte le particelle
    objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);
    return this.readData()
  }

  readData(): Observable<CatastoKendoServerResult> {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const objP = this.objParametriAgendaService.getObjParamValue();
    const catastoAzienda = this.catastoService.LeggiCatastoAziendaFromPiva(objP.Piva, objP.Sa_Cod, objP.Data);
    const provincie = from(this.istatService.leggiProvincie(''));
    const observables = [provincie, catastoAzienda];

    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
    return forkJoin(observables).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        return of()
      }),
      map(
        (result) => {
          this.provincie = result[0] as Provincia[];
          const data = result[1] as JsonKendoResult;
          if (data) {
            this.handleDropdowns();
            const result = new CatastoKendoServerResult(this.kendoModel, this.kendoColumns, data.kendo_rows);
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
            return result;
          }
        }
      ));
  }

  handleDropdowns(): void {
    // Titolo di Possesso
    let col = this.kendoColumns.find(s => s.field === 'Titolo_Possesso_Cod');
    let data = [];
    col.ddl = new DropdownListWithForm('codice', 'Titolo_Possesso_Cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Titolo_possesso';
    col.ddl.loadFunction = this.caricaTitoliPossesso.bind(this);

    col = this.kendoColumns.find(s => s.field === 'MetodoProduzione_Cod');
    col.ddl = new DropdownListWithForm('codice', 'MetodoProduzione_Cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'MetodoProduzione_Des';
    col.ddl.loadFunction = this.caricaMetodoProduzione.bind(this);

    // Provincia
    col = this.kendoColumns.find(s => s.field === 'Prov');
    data = [];
    col.ddl = new DropdownListWithForm('Istat_Prov', 'Prov', 'Provincia_Des', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'PROVINCIA';
    col.ddl.loadFunction = this.caricaProvincie.bind(this);

    // Comune
    col = this.kendoColumns.find(s => s.field === 'Com');
    data = [];
    col.ddl = new DropdownListWithForm('codice', 'Com', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'COMUNE';
    col.ddl.loadFunction = this.caricaComuni.bind(this);

    // Centro
    col = this.kendoColumns.find(s => s.field === 'Sa_Cod');
    data = [];
    col.ddl = new DropdownListWithForm('Centro', 'Sa_Cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.id = 'codice';
    col.ddl.formControlValue = 'descrizione';
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Sa_Nome';
    col.ddl.loadFunction = this.caricaCentri.bind(this);

    // Attivo
    col = this.kendoColumns.find(s => s.field === 'Attivo');
    data = [
      new DropdownListItem(0, 'No'),
      new DropdownListItem(1, 'Sì')
    ];
    col.ddl = new DropdownListWithForm('codice', 'Attivo', 'descrizione', data);
    col.ddl.valuePrimitive = true;
  }

  handleCustomizations(): void {
    const permessoEdit: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale, 2) && this.budgetService.getBudget().activeBudget == false;
    const permessoRemove: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale, 2) && this.budgetService.getBudget().activeBudget == false;
    const permessoInfo: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale, 0);

    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = permessoEdit;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = permessoEdit;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: permessoEdit,
      removeBtn: permessoRemove,
    });

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: permessoEdit,
      infoBtn: permessoInfo,
    });
    this.cmdDropDown.addCommand(new GridCommandItem(
      'Impianti', InvestimentoCatastale, '', faLeaf, true
    ));

    this.resizable.autoFitColumns = false;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title = ' ';
    this.resizable.isResizable = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = permessoEdit;

    this.groups.groupable.enabled = false;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
      this.cmdColumn.editBtn = false;
      this.toolbar.newItem = false;
    }

    this.setCustomCmdDropDown();
  }

  private setCustomCmdDropDown(): void {
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === CatastoCustomOperations.ContrattoDiAffitto) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "AggiungiContrattoAffitto",
        CatastoCustomOperations.ContrattoDiAffitto,
        'faCatastoUploadFile'
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === CatastoCustomOperations.MappaCatastale) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "AggiungiMappaCatastale",
        CatastoCustomOperations.MappaCatastale,
        'faCatastoPointerMap'
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === CatastoCustomOperations.VisuraCatastale) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "AggiungiVisuraCatastale",
        CatastoCustomOperations.VisuraCatastale,
        'faCatastoSaveCreateSchedule'
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === CatastoCustomOperations.VisioneContrattoDiAffitto) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "VisualizzaDocumentiCatasto", //cambiare la descrizione se si mettono anche gli atri due btns
        CatastoCustomOperations.VisioneContrattoDiAffitto,
        'faCatastoGoToDocs'
      ));
    }
    // if(this.cmdDropDown.cmdList.findIndex(v=> v.action === CatastoCustomOperations.VisioneVisuraCatastale) === -1){
    //   this.cmdDropDown.addCommand(new GridCommandItem(
    //     "VisioneVisuraCatastale",
    //     CatastoCustomOperations.VisioneVisuraCatastale,
    //     'faAnagraficaUploadFile'
    //   ));
    // }
    // if(this.cmdDropDown.cmdList.findIndex(v=> v.action === CatastoCustomOperations.VisioneMappaCatastale) === -1){
    //   this.cmdDropDown.addCommand(new GridCommandItem(
    //     "VisioneMappaCatastale",
    //     CatastoCustomOperations.VisioneMappaCatastale,
    //     'faAnagraficaUploadFile'
    //   ));
    // }
  }

  private handleCommands() {
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case CatastoCustomOperations.ContrattoDiAffitto:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.ContrattoDiAffitto);
          break;
        case CatastoCustomOperations.MappaCatastale:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.MappaCatastale);
          break;
        case CatastoCustomOperations.VisuraCatastale:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.VisuraCatastale);
          break;
        case CatastoCustomOperations.NuovoAllegato:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.NuovoAllegato);
          break;
        case CatastoCustomOperations.VisioneContrattoDiAffitto:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.VisioneContrattoDiAffitto);
          break;
        case CatastoCustomOperations.VisioneVisuraCatastale:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.VisioneVisuraCatastale);
          break;
        case CatastoCustomOperations.VisioneMappaCatastale:
          this.catastoGridEventsService.apriKendoWindowPaginaScadenziario(ev.dataItem, CatastoCustomOperations.VisioneMappaCatastale);
          break;
      }
    });
  }

  caricaComuni(dataItem: any): Observable<Comune[]> {
    if (dataItem.Prov !== null && dataItem.Prov != "" && dataItem.Prov != "000") {
      return from(this.istatService.leggiComuni(dataItem.Prov));
    } else {
      return from([]);
    }
  }

  caricaProvincie(dataItem: any) {
    return from(this.istatService.leggiProvincie(''));
  }

  caricaCentri(dataItem: any) {
    this.prepareParameters(this.objParametriAgenda);
    let impresa = new Impresa;
    impresa.partitaIva = this.objParametriAgenda.Piva;
    return from(this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: this.objParametriAgenda.Validita_Inizio }, false).then(vals => {
      return vals.map(el => { return { codice: el.primaryKey.codice, descrizione: el.nome } });
    }));
  }

  caricaTitoliPossesso() {
    return of(this.catastoService.titoli_Di_Possesso);
  }

  caricaMetodoProduzione() {
    return of(this.catastoService.metodiProduzione);
  }

  private prepareParameters(agenda: ObjParametriAgenda) {
    agenda.Validita_Inizio = AGRODATAINIZIO;
    agenda.Validita_Fine = AGRODATAFINE;
    agenda.Data = AGRODATAINIZIO;
    return agenda;
  }

  perform(actionType: HttpAction, item: any): Observable<any[]> {
    const selectedCatasto = new CatastoCentroAziendale()
    selectedCatasto.centro = new CentroAziendale.PK(item.Sa_Cod, item.Piva);
    selectedCatasto.particella = new ParticelleCatastali();
    selectedCatasto.particella.primaryKey = new ParticelleCatastali.PK(item.Prov, item.Com, item.SEZIONE, item.FOGLIO, item.NUMERO, item.SUBALTERNO)
    if (actionType == HttpAction.REMOVE) {
      selectedCatasto.flag_cancellazione = true
      return this.catastoService.checkPossessi(selectedCatasto).pipe(switchMap(r => {
        if (r) {
          return of([])
        } else {
          this.loadingService.set_isLoading({ isLoading: true, message: 'Salvataggio in corso', component: this.gridPublicService.gridElRef });
          return this.catastoGridEventsService.perform(actionType, item);
        }
      }))
    } else {
      this.loadingService.set_isLoading({ isLoading: true, message: 'Salvataggio in corso', component: this.gridPublicService.gridElRef });
      return this.catastoGridEventsService.perform(actionType, item).pipe(tap((part) => {
        if (part == undefined) {
          this.loadingService.set_isLoading({ isLoading: false, message: 'Salvataggio in corso', component: this.gridPublicService.gridElRef });
          return of(part);
        }
        if (part == false) {
          this.loadingService.set_isLoading({ isLoading: false, message: 'Salvataggio in corso', component: this.gridPublicService.gridElRef });
          return of(part);
        }
        var particella: CatastoCentroAziendale = part[0];
        if (particella != null) {
          this.catastoService.ultimaParticellaInserita = {
            sa_cod: particella.centro.codice,
            sa_des: item.Sa_Nome,
            prov: particella.particella.primaryKey.Prov,
            prov_des: item.PROVINCIA,
            com: particella.particella.primaryKey.Com,
            com_des: item.COMUNE,
            sezione: particella.particella.primaryKey.Sezione,
            foglio: particella.particella.primaryKey.Foglio,
          }
        }
      }));
    }
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let a = gridElRef;
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      if (el.Attivo === 0) {
        this.renderer.addClass(visibleRows[index], 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(visibleRows[index], 'nonAttivo');
      }
    });
    this.anagraficaService.applicaFiltri();
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;

    msg = this.translocoService.translate('anagrafica.Delete_Msg')
    opts.data.forEach(r => {
      msg = msg.concat('\n' + r['Sa_Nome'] + ' ' + r['PROVINCIA'] + ' ' + r['COMUNE'] + ' ' + r['FOGLIO'] + ' ' + r['NUMERO']);
    })

    return msg;
  }
}

export enum CatastoCustomOperations {
  NuovoAllegato,
  ContrattoDiAffitto,
  VisuraCatastale,
  MappaCatastale,
  VisioneContrattoDiAffitto,
  VisioneVisuraCatastale,
  VisioneMappaCatastale
}
