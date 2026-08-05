import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { enum_Security_Attivita, enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import {CentriAziendaliService, LeggiCentriAziendali} from 'app/Service/Anagrafica/centri.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {
  AggregateSettings,
  AgrSelectableSettings,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  RemoveMultipleRowsParams,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DateSettings,
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  GridCustomizations,
  KendoGridColumn,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { forkJoin, from, Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { CampiGridEventsService } from './campi-grid-events.service';
import { CampiKendoServerResult, KendoCampiModel } from './campi.model';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import {rispostaStandard} from 'app/Service/master.service';
import { AnagraficaService } from '../anagrafica.service';
import { TranslocoService } from '@jsverse/transloco';
import { CampiShortDescriptionComponent } from './campiShortDescription/campi-short-description.component';
import { Validators } from '@angular/forms';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { GridCommandItem } from 'app/menu-agenda/components/utils';
import { faLayerGroup } from '@fortawesome/free-solid-svg-icons';
import {BudgetService} from '../../Service/Budget/budget.service';
import {GiasDialogService} from '../../Service/gias-dialog.service';
import { ObjParametriAgenda, RadioButtonValue } from 'gias-ui-kit';

const CATASTO: number = -1;

@Injectable()
export class CampiGridHttpService extends AbstractGridConfigService<CampiKendoServerResult> {
  gridId = 'CampiGridHttpService';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'chiave';
  views = new GridCustomizations({enabled: true});
  resizable = new ResizableSettings(true, true);

  aggregates = new AggregateSettings({
    enabled: true,
    descriptors: [
      {field: 'Superficie_Totale', aggregate: 'sum', format: 'n4'}
    ]}
  );

  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'Descrizione', title: this.translocoService.translate('Descrizione')},{ resizable:true, filterable:false, editable: false, media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)', component: CampiShortDescriptionComponent, width: 135 }
    ),
    new KendoGridColumn(
      {field: 'sa_cod', title: this.translocoService.translate('Centro')},{ resizable:true, editable: true, validators: [Validators.required], width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Campo', title: this.translocoService.translate('Campo') }, { resizable: true, editable: true, width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') }, { resizable: true, editable: true, date: new DateSettings(), width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') }, { resizable: true, editable: true, date: new DateSettings(), width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Gru_Cod', title: this.translocoService.translate('OrientamentoColturale') }, { resizable: true, editable: true, numeric: { }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135  }
    ),
    new KendoGridColumn(
      { field: 'Veg_Cod', title: this.translocoService.translate('Specie') }, { resizable: true, editable: true, numeric: { }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'rif_alfanumerico', title: this.translocoService.translate('RifAlfanumerico') }, { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Superficie_Totale', title: this.translocoService.translate('SuperficieTotaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Superficie_Biologico', title: this.translocoService.translate('SuperficieBiologicoAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Superficie_Convenzionale', title: this.translocoService.translate('SuperficieConvenzionaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Superficie_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'sup_contratto', title: this.translocoService.translate('SuperficieContrattoAbbr') }, { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'filiera', title: this.translocoService.translate('Filiera') }, { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Attivo', title: this.translocoService.translate('Attivo') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    )
  ];

  kendoModel: KendoCampiModel = {
    sa_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    Campo: new ModelEntry(CELL_TYPES.STRING, false),
    Campo_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Campo_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
    Gru_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Veg_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
    Gru_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    PIVA: new ModelEntry(CELL_TYPES.STRING, false),
    Superficie_Totale: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Convenzionale: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Biologico: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Conversione: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Catastale: new ModelEntry(CELL_TYPES.NUMBER, false),
    rif_alfanumerico: new ModelEntry(CELL_TYPES.STRING, false),
    sup_contratto: new ModelEntry(CELL_TYPES.NUMBER, false),
    filiera: new ModelEntry(CELL_TYPES.NUMBER, false),
    Attivo: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Descrizione: new ModelEntry(CELL_TYPES.CUSTOM, false),
    // ---------------------------------------------------------------------
    Ribaltato: new ModelEntry(CELL_TYPES.STRING, false),
    ribaltatoDes: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Ribaltamento: new ModelEntry(CELL_TYPES.DATE, false),
  };

  objParametriAgenda: ObjParametriAgenda;

  cmdColumn = new CommandsColumnSettings();
  toolbar = new ToolbarSettings();

  OrientamentoColturale: Array<RadioButtonValue> = [
    { name: this.translocoService.translate('NonImpostato'), value: 0, enable: true },
    { name: this.translocoService.translate('MonoSpecie'), value: 3, enable: true },
    { name: this.translocoService.translate('MultiSpecie'), value: 1, enable: true }
  ];

  constructor(
    injector: Injector,
    private objParametriAgendaService: ObjParametriAgendaService,
    private campiGridEventsService: CampiGridEventsService,
    private router: Router, // non togliere perché è necessario al funzionamento della funzione editColumn.edit
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private gridAPI: GridPublicService,
    private centriService: CentriAziendaliService,
    private renderer: Renderer2,
    private permessiUtenteService: PermessiUtenteService,
    private anagraficaService: AnagraficaService,
    private translocoService: TranslocoService,
    private budgetService: BudgetService,
    private giasDialogService: GiasDialogService
  ) {

    super(injector);
    this.objParametriAgenda = objParametriAgendaService.getObjParamValue();
    this.handleCustomizations();

    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case CommandsDropDownEvents.FULL_EDIT:
          this.campiGridEventsService.modificaCampoAngular(ev.dataItem, false, ev.rowIndex);
          break;
        case CATASTO:
          this.campiGridEventsService.onInvestimentoCatastale(ev.dataItem);
          break;
      }
    })

    this.gridPublicService.changeDetected.GiasSubscribe((event: any) => {
      if(event?.action === 'edit'){
        let fb = this.gridPublicService.formGroup.value
        if (fb != undefined) {
          fb.controls["sa_cod"].disable();
        }
      }
      if(event?.action === 'add'){
        let fb = this.gridPublicService.formGroup.value
        if (fb != undefined) {
          let col = this.kendoColumns.find(s => s.field === 'sa_cod');
          this.caricaCentri().GiasSubscribe(data => {
            if (data.length == 1) {
              fb.controls['sa_cod'].setValue(data[0].codice);
              fb.controls['sa_nome'].setValue(data[0].descrizione);
              col.ddl.reload.next(true);
            }
          });
          fb.controls['Validita_Inizio'].setValue(AGRODATAINIZIO);
          fb.controls['Validita_Fine'].setValue(AGRODATAFINE);
          fb.controls['Gru_Cod'].setValue(this.OrientamentoColturale[0].value);
          fb.controls["sa_cod"].enable();
        };
      }
    });

    // gestore del change della ddl Orientamento Colturale
    this.gridPublicService.formGroup.GiasSubscribe((fb) => {
      if (fb != undefined) {
        fb.controls["Gru_Cod"].valueChanges.GiasSubscribe((val) => {
          if (val != 3) {
            fb.controls["Veg_Cod"].disable();
            fb.controls["Veg_Cod"].setValue(0);
          } else {
            fb.controls["Veg_Cod"].enable();
          }
        });

        if (fb.controls['Gru_Cod'].value != 3) {
          fb.controls["Veg_Cod"].disable();
          fb.controls["Veg_Cod"].setValue(0);
        } else {
          fb.controls["Veg_Cod"].enable();
        };
      }
    });

    if (this.isBudget()) {
      this.kendoColumns.push(
        new KendoGridColumn(
          { field: 'ribaltatoDes', title: this.translocoService.translate('Ribaltato') },
          { resizable: true, editable: false, width: 165 }
        )
      );
      this.kendoColumns.push(
        new KendoGridColumn(
          { field: 'Data_Ribaltamento', title: this.translocoService.translate('DataRibaltamento') },
          { resizable: true, editable: false, width: 165 }
        )
      );
    }
  }

  read(): Observable<CampiKendoServerResult> {
    // azzero i valori di Campo_Cod e Sa_Cod, altrimenti legge solo i campi relativi al Sa_Cod presente
    let objP = this.objParametriAgendaService.getObjParamValue();
    // chiamo il warning se ritorno da una delete
    this.campiGridEventsService.msgWarningDelete();
    //objP.Sa_Cod = 0;
    objP.Campo_Cod = 0;
    objP.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    objP.Data = AGRODATAINIZIO;
    let data = this.anagraficaService.filterData.getValue()
    if (data.filter){
      objP.Data = data.data;
    }

    const catastoAzienda = this.campiService.LeggiCampiAnagrafica(objP);
    const observables = [catastoAzienda];
    this.loadingService.set_isLoading({ message: '', isLoading: true, component: this.gridAPI.gridElRef });
    return forkJoin(observables).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        return of()
      }),
      map(results => {
        let campi: any[] = results[0];
        campi.forEach(e => {
          if (e.Ribaltato?.trim().toLowerCase() == 'true') {
            e.ribaltatoDes = this.translocoService.translate('Si');
          } else {
            e.ribaltatoDes = this.translocoService.translate('No');
          }
        });
        this.loadingService.set_isLoading({ message: '', isLoading: false, component: this.gridAPI.gridElRef });
        if (results) {
          this.handleDropdowns();
          let result = new CampiKendoServerResult(this.kendoModel, this.kendoColumns, campi);
          return result;
        }
      })
    );
  }

  handleDropdowns(): void {

    // Orientamento Colturale
    let col = this.kendoColumns.find(s => s.field === 'Gru_Cod');
    let data: DropdownListItem[] = this.OrientamentoColturale.map(cod => new DropdownListItem(cod.value, cod.name));
    col.ddl = new DropdownListWithForm('OrientamentoColturale', 'Gru_Cod', 'name', data);
    col.ddl.valuePrimitive = true;

    // Specie Vegetali
    col = this.kendoColumns.find(s => s.field === 'Veg_Cod');
    data = [];
    col.ddl = new DropdownListWithForm('SpecieVegetali', 'Veg_Cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.id = 'codice';
    col.ddl.formControlValue = 'descrizione';
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Veg_Des';
    col.ddl.loadFunction = this.caricaSpecieVegetali.bind(this);

    // Centro
    col = this.kendoColumns.find(s => s.field === 'sa_cod');
    data = [];
    col.ddl = new DropdownListWithForm('Centro', 'sa_cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.id = 'codice';
    col.ddl.formControlValue = 'descrizione';
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'sa_nome';
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
    })
    this.anagraficaService.applicaFiltri();
  }

  caricaSpecieVegetali() {
    let specie = from(this.campiService.leggi_SpecieVegetali()).pipe(map(r => {
      if (!r.some(sv => sv.codice === 0)) {
        r.push({codice: 0, descrizione: this.translocoService.translate('NessunaColtura')});
      }

      return r;
    }));
    return specie;
  }

  caricaCentri() {
    this.prepareParameters(this.objParametriAgenda);
    let impresa = new Impresa;
    impresa.partitaIva = this.objParametriAgenda.Piva;
    return from(this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{ impresa: impresa, data: this.objParametriAgenda.Validita_Inizio }, false).then(vals => {
      return vals.map(el => { return { codice: el.primaryKey.codice, descrizione: el.nome } });
    }));
  }

  perform(actionType: HttpAction, item: any): Observable<any> {
    this.prepareParameters(this.objParametriAgenda);
    this.loadingService.set_isLoading({isLoading:true, component: this.gridPublicService.gridElRef})
    if(actionType === HttpAction.UPDATE) {
      return this.campiGridEventsService.updateCampo_inLine(item, enum_TipoOperazioneDB.Modifica);
    } else if(actionType === HttpAction.CREATE) {
      return this.campiGridEventsService.updateCampo_inLine(item, enum_TipoOperazioneDB.Scrittura);
    } else if(actionType === HttpAction.REMOVE) {
      return this.removeCampo(item);
    }
  }

  private prepareParameters(agenda: ObjParametriAgenda) {
    agenda.Validita_Inizio = AGRODATAINIZIO;
    agenda.Validita_Fine = AGRODATAFINE;
    agenda.Data = AGRODATAINIZIO;
    return agenda;
  }

  private removeCampo(item: any): Observable<any> {
    return of(item).pipe(
      switchMap((campo) => {
        let ribaltato: boolean = item?.Ribaltato == 'true';
        let dialog: Observable<any> = of(null);
        if (ribaltato && this.isBudget()) {
          let actions = [
            { text: this.translocoService.translate('Si'), primary: true, returnObj: true },
            { text: this.translocoService.translate('No'), returnObj: false }
          ];
          dialog = this.giasDialogService.dialogMessageObs_Result('', this.getCampoRibaltatoMsg(campo), actions);
        }
        return forkJoin([
          of(campo),
          dialog
        ]);
      }),
      switchMap((resp) => {
        return this.campiGridEventsService.eliminaCampoAngular(item, resp[1]?.returnObj ?? false)
      }),
      switchMap((r) => {
        this.campiGridEventsService.addCampoSelezionato(item, this.getDeletCampoErrorMsg(r));
        return of([]);
      })
    );
  }

  private getDeletCampoErrorMsg(r: rispostaStandard<any>): string {
    let error: string = r.RispostaOK ? '' : 'Non è stato possibile cancellare il campo.';
    if (r.ErroriGias.length > 0) {
      error = r.ErroriGias[0].messaggio;
    }
    return error;
  }

  private getCampoRibaltatoMsg(campo: any): string {
    let msg: string = campo.Campo_Des;
    msg = msg.concat('\n'.concat(this.translocoService.translate('EliminaCampoRibaltato')));
    return msg;
  }

  handleCustomizations(): void {
    const permessoEdit: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Campo, 2);
    const permessoRemove: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Campo, 2);
    const permessoInfo: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Campo, 0);

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = true;
    this.selectable.columnSettings.showSelectAll=true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title=' ';

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
      'Catasto', CATASTO, '', faLayerGroup
    ));

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

  private isBudget(): boolean {
    return this.budgetService?.isBudget();
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;

    msg = this.translocoService.translate('anagrafica.Delete_Msg')
    opts.data.forEach(r => {
      msg = msg.concat('\n '+ r['Campo']);
    })

    return msg;
  }
}
