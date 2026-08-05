import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import { AgrSelectableSettings, CommandsColumnSettings, CommandsDropDownEvents, CommandsDropDownSettings, RemoveMultipleRowsParams, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, LoaderType, ModelEntry, RendererGridEvent } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { forkJoin, from, Observable, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { KendoMacchineModel, MacchinaKendoServerResult } from './macchine.model';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Router } from '@angular/router';
import { filter, map, mergeMap, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { MacchineGridEventsService } from './macchine-grid-events.service';
import { FormGroup, Validators } from '@angular/forms';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import {AnagraficaService, FilterData} from '../anagrafica.service';
import {CentriAziendaliService, LeggiCentriAziendali} from 'app/Service/Anagrafica/centri.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { TranslocoService } from '@jsverse/transloco';
import { MacchinaShortDescriptionComponent } from './macchinaShortDescription/macchina-short-description.component';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { CaratteristicheService } from './macchine-edit/macchine-edit-caratteristiche/caratteristiche.service';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { GiasMessageService } from 'app/Service/gias-message.service';
import {BudgetService} from "../../Service/Budget/budget.service";
import {ContattiService} from 'app/Service/Anagrafica/contatti.service';
import {CodificaMacchineAgeaRequest} from '../../Model/metaschema/CodificheAgea';
import {CodificheAgeaService} from '../../Service/Metaschema/codifiche-agea.service';
import {BaseCodeDescrStr} from '../../Model/baseClass/baseCodeDescrStr';
import {AnagraficaBusinessLogicService} from "../services/anagrafica-business-logic.service";
import {GridCommandItem} from "../../menu-agenda/components/utils";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class MacchineConfigService extends AbstractGridConfigService<MacchinaKendoServerResult> {
  gridId: string = 'MacchineConfigService';

  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId: string = 'chiave';
  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({editBtn: false, infoBtn: false, removeBtn: false});
  resizable: ResizableSettings;

  private objParametriAgenda: ObjParametriAgenda;
  private KendoMacchine: any;
  private macchineSelezionate: Map<ParcoMacchine, string> = new Map<ParcoMacchine, string>();

  private macchineRows: any[];


  private permessoNuovoDocumento: boolean;
  private permessoRicercaDocumenti: boolean;
  private permessoAssegnazionePubblica: boolean;
  private prevContatto: any;

  private marche: any;
  private tipo: any;

  private centresOnWhichIsUsed: number[] = [];
  private companiesByWhichIsUsed: string[] = [];

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'Descrizione', title: this.translocoService.translate('Descrizione')},
      {
        resizable: true,
        filterable: false,
        editable: false,
        media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)',
        component: MacchinaShortDescriptionComponent,
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Sa_Cod', title: this.translocoService.translate('Visibilità')},
      {resizable: true, editable: true, width: 135}
    ),
    new KendoGridColumn(
      {field: 'rag_soc', title: this.translocoService.translate('Impresa')},
      {resizable: true, editable: false, width: 135}
    ),
    new KendoGridColumn(
      {field: 'Cod_Contatto', title: this.translocoService.translate('Contatto')},
      {resizable: true, editable: true, width: 135}
    ),
    new KendoGridColumn(
      {field: 'CLASS_CODE', title: this.translocoService.translate('Tipologia')},
      {resizable: true, editable: true, width: 135}
    ),
    new KendoGridColumn(
      {field: 'Agea_Cod', title: this.translocoService.translate('CodificaAgea')},
      {resizable: true, editable: true, width: 135}
    ),
    new KendoGridColumn(
      {field: 'Ditta_Cod', title: this.translocoService.translate('Marca')}, {
        resizable: true,
        editable: true,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Modello', title: this.translocoService.translate('Modello')}, {
        resizable: true,
        editable: true,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Macchina', title: this.translocoService.translate('Descrizione')}, {
        resizable: true,
        editable: true,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Telaio', title: this.translocoService.translate('Telaio')}, {
        resizable: true,
        editable: true,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Targa', title: this.translocoService.translate('Targa')}, {
        resizable: true,
        editable: true,
        validators: [Validators.maxLength(10)],
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Codice', title: this.translocoService.translate('Codice')}, {
        resizable: true,
        editable: true,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Ultima_Manutenzione', title: this.translocoService.translate('Data_Ultima_Manutenzione')}, {
        resizable: true,
        editable: true,
        date: {defaultValue: AGRODATAINIZIO},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Ultima_Revisione', title: this.translocoService.translate('Data_Ultima_Revisione')}, {
        resizable: true,
        editable: true,
        date: {defaultValue: AGRODATAINIZIO},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio')}, {
        resizable: true,
        editable: true,
        date: {defaultValue: AGRODATAINIZIO},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine')}, {
        resizable: true,
        editable: true,
        date: {defaultValue: AGRODATAFINE},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione')}, {
        resizable: true,
        editable: false,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione')}, {
        resizable: true,
        editable: false,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Data_Modifica', title: this.translocoService.translate('DataModifica')}, {
        resizable: true,
        editable: false,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    ),
    new KendoGridColumn(
      {field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica')}, {
        resizable: true,
        editable: false,
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 135
      }
    )
  ];

  private kendoModel: KendoMacchineModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    Piva: new ModelEntry(CELL_TYPES.STRING, false),
    Sa_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    rag_soc: new ModelEntry(CELL_TYPES.STRING, false),
    Visibilita: new ModelEntry(CELL_TYPES.STRING, false),
    Mac_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Cod_Contatto: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Contatto_Des: new ModelEntry(CELL_TYPES.STRING, false),
    tipologia: new ModelEntry(CELL_TYPES.STRING, false),
    CLASS_CODE: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    AGEA_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Ditta_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Ditta_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Modello: new ModelEntry(CELL_TYPES.STRING, true),
    Macchina: new ModelEntry(CELL_TYPES.STRING, true),
    Telaio: new ModelEntry(CELL_TYPES.STRING, true),
    Targa: new ModelEntry(CELL_TYPES.STRING, true),
    Codice: new ModelEntry(CELL_TYPES.STRING, true),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true),
    Ultima_Manutenzione: new ModelEntry(CELL_TYPES.DATE, true),
    Ultima_Revisione: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
    Descrizione: new ModelEntry(CELL_TYPES.CUSTOM, false),
  };

  constructor(
    injector: Injector,
    @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private macchineGridEventsService: MacchineGridEventsService,
    private renderer: Renderer2,
    private router: Router, // non togliere perché è necessario al funzionamento della funzione editColumn.edit
    private permessiUtenteService: PermessiUtenteService,
    private anagraficaService: AnagraficaService,
    private centriService: CentriAziendaliService,
    private masterService: MasterService,
    private translocoService: TranslocoService,
    private giasDialogService: GiasDialogService,
    private caratteristicheService: CaratteristicheService,
    private giasMessageService: GiasMessageService,
    private budgetService: BudgetService,
    private contattiService: ContattiService,
    private codificheAgeaService: CodificheAgeaService,
    private businessLogic: AnagraficaBusinessLogicService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.macchineGridEventsService.loadingService = this.loadingService;
    this.macchineGridEventsService.component = this.gridPublicService.gridElRef;

    this.objParametriAgenda = objParametriAgendaService.getObjParamValue();

    this.handleCustomizations();

    this.gridPublicService.changeDetected.GiasSubscribe((event: any) => {
      if (event?.action === 'add') {
        let fb = this.gridPublicService.formGroup.value;
        fb.controls['Cod_Contatto'].setValue('');
        fb.controls['Contatto_Des'].setValue('');

        fb.controls['Sa_Cod'].setValue({id: '0', name: 'Macchina Privata'});
        fb.controls['Validita_Inizio'].setValue(AGRODATAINIZIO);
        fb.controls['Validita_Fine'].setValue(AGRODATAFINE);
        this.contattoChange(fb).subscribe();

        if (this.prevContatto) {
          let col = this.kendoColumns.find(s => s.field === 'Cod_Contatto');
          col.ddl.data = [new DropdownListItem(this.prevContatto.codice, this.prevContatto.descrizione)];
          fb.controls['Cod_Contatto'].patchValue(this.prevContatto.codice);
          fb.controls['Contatto_Des'].patchValue(this.prevContatto.descrizione);
        }

        fb.controls['CLASS_CODE'].setValue('15');
        let col = this.kendoColumns.find(s => s.field === 'CLASS_CODE');

        if (col) {
          col.ddl.reload.next(true);
        }
      }

      if (event?.action == 'edit') {
        let fb = this.gridPublicService.formGroup.value;
        if (fb != undefined) {
          this.macchineService.isMacchinaMovimentata(this.macchineGridEventsService.prepareParcoMacchine(fb.getRawValue())).pipe(
            take(1),
            switchMap(r => {
              // se la macchina è movimentata impedisco la modifica della tipologia, e del contatto
              if (r.RispostaStringa) {
                fb.controls['CLASS_CODE'].disable();
                fb.controls['Cod_Contatto'].disable();
              } else {
                if (fb.controls['Cod_Contatto'].value != '') {
                  fb.controls['Sa_Cod'].disable();
                } else {
                  fb.controls['Sa_Cod'].enable();
                }
                this.contattoChange(fb).subscribe();
              }

              if (fb.controls['Piva'].getRawValue() != this.objParametriAgendaService.getObjParamValue().Piva) {
                this.giasDialogService.baseError('', this.translocoService.translate('MacchinaAssociataAltraAzienda'), false);
                this.gridPublicService.giasGridComponent.closeEditor(event.sender, event.rowIndex);
                return;
              }

              return of(null);
            })
          ).subscribe();
        }
      }

      if (event?.action == 'remove') {
        if (event?.dataItem.Piva != this.objParametriAgendaService.getObjParamValue().Piva) {
          this.giasDialogService.baseError('', this.translocoService.translate('MacchinaAssociataAltraAzienda'), false);
          return;
        }
      }
    });

    this.permessoAssegnazionePubblica = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Macchine_Assegnazione_Pubblica, 2);
    this.permessoRicercaDocumenti = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Documentale_Lista);
    this.permessoNuovoDocumento = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Documentale_Inser);
  }

  read(): Observable<MacchinaKendoServerResult> {
    this.objParametriAgenda.Data = AGRODATAINIZIO;
    let data: FilterData = this.anagraficaService.filterData.getValue();
    if (data.filter) {
      this.objParametriAgenda.Data = data.data;
    }
    this.msgWarningDelete();
    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
    this.macchineGridEventsService.component = this.gridPublicService.gridElRef;
    const obs: Promise<any>[] = [
      this.macchineService.leggiMacchine(this.objParametriAgenda),
      this.macchineService.leggi_CmbTipoMacchina_Tutto(),
      this.macchineService.leggi_Ditte()
    ];

    return forkJoin(obs).pipe(map(result => {
      this.KendoMacchine = result[0].RispostaStringa;
      this.macchineRows = <Array<any>>(this.KendoMacchine.kendo_rows);

      this.tipo = result[1];
      this.marche = result[2];

      this.handleDropdowns();

      const tableData = new MacchinaKendoServerResult(
        this.kendoModel,
        this.kendoColumns,
        this.macchineRows
      );

      this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
      return tableData;
    }));
  }

  perform(actionType: HttpAction, item: any): Observable<any> {
    return this.controlloMovimentazioniMacchina(item, actionType).pipe(mergeMap((r) => {
      if (r.RispostaOK) {
        return this.performActions(actionType, item, r.RispostaStringa);
      } else {
        throw new Error(r.Errore);
      }
    }));
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;

    msg = this.translocoService.translate('anagrafica.Delete_Msg');
    opts.data.forEach(r => {
      msg = msg.concat('\n - ' + r['Macchina'] + r['tipologia']);
    });

    return msg;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const {grid, gridElRef} = {...opts};
    let a = gridElRef;
    let rows: [] = grid.data['data'];

    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      if (el.Attivo === 0) {
        this.renderer.addClass(visibleRows[index], 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(visibleRows[index], 'nonAttivo');
      }
    });

    this.nascondiPulsantiEdit(this.renderer, visibleRows, rows);
    this.anagraficaService.applicaFiltri();
  }

  private performActions(actionType: HttpAction, item: any, isMovimentata: boolean) {
    return this.rimozioneCaratteristicheCheck(actionType, item).pipe(
      take(1),
      switchMap((dialogRes) => {
        if (dialogRes) {
          return this.macchinaMovimentataCheck(actionType, isMovimentata, item).pipe(
            take(1),
            switchMap((dialogRes) => {
              if (dialogRes) {
                if (actionType === HttpAction.UPDATE) {
                  if (item.CLASS_CODE.startsWith("19")) {
                      this.macchineGridEventsService.modificaMacchinaAngular(item, false, undefined);
                      return of([]);
                  } else
                        return this.macchineGridEventsService.updateMacchina(item);
                } else if (actionType === HttpAction.CREATE) {
                  if (item.CLASS_CODE.startsWith("19")) {
                      this.macchineGridEventsService.modificaMacchinaAngular(item, true, undefined);
                      return of([]);
                  } else
                        return this.macchineGridEventsService.scriviMacchina(item);
                } else if (actionType === HttpAction.REMOVE) {
                  if (item.Piva == this.objParametriAgendaService.getObjParamValue().Piva) {
                    return this.macchineGridEventsService.eliminaMacchinaAngular(item).pipe(
                      switchMap((r) => {
                        this.masterService.set_isLoading({isLoading: false, message: ''});
                        if (r.RispostaOK) {
                          this.addMacchinaSelezionata(item, '');
                        } else {
                          this.addMacchinaSelezionata(item, r.ErroriGias[0].messaggio);
                        }
                        return of([]);
                      })
                    );
                  } else {
                    this.masterService.set_isLoading({isLoading: false, message: ''});
                  }
                }
              } else {
                return of([]);
              }
            })
          );
        } else {
          return of([]);
        }
      })
    );
  }

  private macchinaMovimentataCheck(actionType: HttpAction, isMovimentata: boolean, item: any): Observable<boolean> {
    if (actionType == HttpAction.UPDATE && isMovimentata) {
      return this.macchineService.isEditAllowed(this.macchineGridEventsService.prepareParcoMacchine(item)).pipe(
        tap(r => {
          if (!r['RispostaStringa']) {
            this.giasDialogService.baseError(
              this.translocoService.translate('MacchinaAttrezzatura'),
              this.translocoService.translate(r['Errore'])
            );
          }
        }),
        map(r => r.RispostaStringa)
      );
    } else if (actionType == HttpAction.REMOVE && isMovimentata) {
      this.addMacchinaSelezionata(item, ' '.concat(this.translocoService.translate('MenuBS_Anagrafica_DeleteElemento_ImpossibileCancellareMacchinarioConMovimenti')));
      return of(false);
    }
    return of(true);
  }

  private rimozioneCaratteristicheCheck(actionType: HttpAction, item: any): Observable<boolean> {
    let macchinatemp = undefined;
    let class_code = item.CLASS_CODE;
    let objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    objParametriAgenda.Mac_Cod = item.Mac_Cod;
    objParametriAgenda.Piva = item.Piva;
    if (actionType == 'create') {
      return of(true);
    }
    return this.macchineService.leggiMacchina(objParametriAgenda).pipe(
      take(1),
      switchMap(macchina => {
        macchinatemp = macchina.RispostaStringa;
        return this.caratteristicheService.getCaratteristicheDisponibili(class_code);
      }), switchMap(car_disp => {
        if (
          actionType == HttpAction.UPDATE &&
          this.caratteristicheService.checkPerditaCaratteristiche(macchinatemp.caratteristiche, car_disp)
        ) {
          item.caratteristiche = [];
          return this.giasDialogService.dialogMessageObs_Result(this.transloco.translate('MacchinaAttrezzatura'),
            this.transloco.translate('PerditaCaratteristiche'),
            [
              {text: this.transloco.translate('Ok'), primary: true, returnObj: true},
              {text: this.transloco.translate('Annulla'), returnObj: false}
            ],
            undefined,
            undefined
          ).pipe(
            map((dialogRes) => (<any>dialogRes).returnObj)
          );
        }
        return of(true);
      })
    );
  }

  private handleCustomizations(): void {
    const permessoEdit: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine, 2) && this.budgetService.getBudget().activeBudget === false;
    const permessoRemove: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine, 2) && this.budgetService.getBudget().activeBudget === false;
    const permessoInfo: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine, 0);

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = permessoEdit;
    this.selectable.shouldShowCheckbox = permessoEdit;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = permessoEdit;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: permessoEdit,
      removeBtn: permessoRemove,
    });

    this.setupDdlCommandsMenu(permessoEdit, permessoInfo)
    this.handleCommandEvent();

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

  private handleDropdowns(): void {
    let data: any;
    let col: KendoGridColumn;

    col = this.kendoColumns.find(s => s.field === 'Sa_Cod');
    col.ddl = new DropdownListWithForm('VisibilitaPubblica', 'Sa_Cod', 'descrizione', [], new DropdownListItem(0, ''));
    col.ddl.valuePrimitive = false;
    col.validators = [Validators.required];
    col.ddl.id = 'codice';
    col.ddl.formControlValue = 'descrizione';
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Visibilita';
    col.ddl.loadFunction = this.caricaCentri.bind(this);
    col.ddl.itemDisabledFn = this.disableVisibilityItems.bind(this);

    col = this.kendoColumns.find(s => s.field === 'Cod_Contatto');
    col.ddl = new DropdownListWithForm('Contatti', 'Cod_Contatto', 'Contatto_Des', []);
    col.ddl.descriptionField = 'Contatto_Des';
    col.ddl.valuePrimitive = true;
    col.ddl.id = 'primaryKey';
    col.ddl.formControlValue = 'ragione_Sociale';
    col.ddl.loadOnEdit = true;
    col.ddl.loadFunction = this.caricaContatti.bind(this);

    col = this.kendoColumns.find(s => s.field === 'CLASS_CODE');
    data = this.tipo.map(cod => new DropdownListItem(cod.CLASS_CODE, cod.CLASS_DESC));
    col.ddl = new DropdownListWithForm('Tipologia', 'CLASS_CODE', 'tipologia', data);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'tipologia';
    col.validators = [Validators.required];

    col = this.kendoColumns.find(s => s.field === 'Ditta_Cod');
    data = this.marche.map(cod => new DropdownListItem(cod.codice, cod.descrizione));
    col.ddl = new DropdownListWithForm('Marche', 'Ditta_Cod', 'Ditta_Des', data);
    col.ddl.descriptionField = 'Ditta_Des';
    col.ddl.valuePrimitive = true;

    col = this.kendoColumns.find(s => s.field === 'Agea_Cod');
    col.ddl = new DropdownListWithForm('codice', 'Agea_Cod', 'descrizione', []);
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'AGEA_Des';
    col.ddl.loadFunction = this.loadCodificheAgea.bind(this);
  }

  private caricaCentri(dataItem?: any) {
    this.prepareParameters(this.objParametriAgenda);

    let impresa: Impresa = new Impresa();
    impresa.partitaIva = this.objParametriAgenda.Piva;

    let m: ParcoMacchine = new ParcoMacchine();
    m.codice = dataItem['Mac_Cod'] ?? 0;
    m.partitaIva = dataItem['Piva'] ?? this.objParametriAgenda.Piva;

    let centres$ = from(this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{
      impresa: impresa,
      data: this.objParametriAgenda.Validita_Inizio
    }, false).then(vals => {
      let visibilitaPubblica = [{codice: 0, descrizione: 'Macchina Privata'}];
      vals.forEach(el => {
        visibilitaPubblica.push({codice: el.primaryKey.codice, descrizione: el.nome});
      });
      if (this.permessoAssegnazionePubblica) {
        visibilitaPubblica.push({codice: -1, descrizione: 'Macchina Pubblica'});
      }
      return visibilitaPubblica;
    }));

    let usedCentres$ = this.fetchCentresByWhichIsUsed(m);
    let companies$ = this.fetchCompaniesByWhichIsUsed(m);

    return forkJoin([centres$, usedCentres$, companies$]).pipe(map(r => {
      this.centresOnWhichIsUsed = r[1].RispostaStringa ?? [];
      this.companiesByWhichIsUsed = r[2].RispostaStringa ?? [];

      return r[0];
    }));
  }

  private caricaContatti(): Observable<any[]> {
    return this.contattiService.leggiContattiMacchina({
      objNG: this.objParametriAgenda,
      Flag_Pubblico_Privato: this.permessoAssegnazionePubblica,
      Flag_Visibilita_Centri: true,
      Cod_Contatto: ''
    }).pipe(
      switchMap(r => {
        let contatti: any[] = [{primaryKey: '', ragione_Sociale: ''}];
        contatti = contatti.concat(r);
        return of(contatti);
      })
    );
  }

  private loadCodificheAgea(): Observable<BaseCodeDescrStr[]> {
    let params: CodificaMacchineAgeaRequest = {
      ageaCod: '',
      ageaDes: '',
      classCod: '',
    };
    return this.codificheAgeaService.readAgeaMachineCodes(params).pipe(
      map(r => {
        return [new BaseCodeDescrStr('', ''), ...r.RispostaStringa];
      })
    );
  }

  private prepareParameters(agenda: ObjParametriAgenda) {
    agenda.Validita_Inizio = AGRODATAINIZIO;
    agenda.Validita_Fine = AGRODATAFINE;
    agenda.Data = AGRODATAINIZIO;
    return agenda;
  }

  private nascondiPulsantiEdit(renderer: any, domElems: any[], elems: any[]) {
    elems.forEach((dataItem: any, index: number) => {
      let currDomRow = domElems[index];
      if (dataItem.Sa_Cod == -1 && !this.permessoAssegnazionePubblica) {
        UtilityFunctions.setStyle(renderer, currDomRow, '.editBtn', 'display', 'none');
        UtilityFunctions.setStyle(renderer, currDomRow, '#btnDelete', 'display', 'none');
      } else {
        UtilityFunctions.setStyle(renderer, currDomRow, '.editBtn', 'display', 'block');
        UtilityFunctions.setStyle(renderer, currDomRow, '#btnDelete', 'display', 'block');
      }
    });
  }

  private selectMacchina_old(item: any) {
    let macchina_old = this.macchineRows.find(m => m.chiave == item.chiave);
    return macchina_old;
  }

  private isVisibilitaOrTypeOrValidityChanged(macchina_new: any) {
    let macchina_old = this.selectMacchina_old(macchina_new);
    return macchina_new.Sa_Cod.id != macchina_old.Sa_Cod ||
           macchina_new.CLASS_CODE != macchina_old.CLASS_CODE ||
           macchina_new.Validita_Fine != macchina_old.Validita_Fine ||
           macchina_new.Validita_Inizio != macchina_old.Validita_Inizio;
  }

  private controlloMovimentazioniMacchina(macchina_new: any, actionType: HttpAction) {
    if (
      (actionType == HttpAction.UPDATE && this.isVisibilitaOrTypeOrValidityChanged(macchina_new)) ||
      actionType == HttpAction.REMOVE
    ) {
      let agenda = this.objParametriAgendaService.getObjParamValue();
      agenda.Piva = macchina_new.Piva;
      agenda.Mac_Cod = macchina_new.Mac_Cod;
      return this.macchineService.isMacchinaMovimentata(this.macchineGridEventsService.prepareParcoMacchine(macchina_new));
    } else {
      let resp = new rispostaStandard<boolean>();
      resp.RispostaOK = true;
      resp.RispostaStringa = false;
      return from([resp]);
    }
  }

  private addMacchinaSelezionata(macchina: ParcoMacchine, error: string) {
    this.macchineSelezionate.set(macchina, error);
  }

  private getMacchineSelezionate() {
    return this.macchineSelezionate;
  }

  private msgWarningDelete() {
    let msgFailure = this.translocoService.translate('ErroreCancellazioneMacchine');
    let msgSuccess = this.translocoService.translate('SuccessoCancellazioneMacchine');
    let displaySuccess = false;
    let displayFailure = false;
    this.getMacchineSelezionate().forEach((error, mac) => {
      let mac_des = '';
      if (mac['Modello'] != '') {
        mac_des = mac['Modello'];
      } else if (mac['Macchina'] != '') {
        mac_des = mac['Macchina'];
      } else if (mac['tipologia'] != '') {
        mac_des = mac['tipologia'];
      }
      if (error == '') {
        msgSuccess = msgSuccess.concat('\n' + mac_des);
        displaySuccess = true;
      } else {
        msgFailure = msgFailure.concat('\n' + mac_des + ':' + error);
        displayFailure = true;
      }
    });
    if (displaySuccess) {
      this.giasMessageService.successMessage(msgSuccess);
    }
    if (displayFailure) {
      this.giasDialogService.baseError('', msgFailure);
    }
    this.clearSelezionate();
  }

  private contattoChange(fb: FormGroup): Observable<any> {
    return fb.controls['Cod_Contatto'].valueChanges.pipe(map(cod_contatto => {
      if (cod_contatto != '') {
        this.contattiService.leggiContattiMacchina({
          objNG: this.objParametriAgenda,
          Flag_Pubblico_Privato: true,
          Flag_Visibilita_Centri: true,
          Cod_Contatto: cod_contatto
        }).pipe(take(1), map(r => {
          let contatto = r[0];
          this.caricaCentri().pipe(take(1), tap((vals) => {
            fb.controls['Sa_Cod'].setValue({id: contatto.sa_cod, name: vals.find(s => s.codice == contatto.sa_cod).descrizione});
            fb.controls['Visibilita'].setValue(vals.find(s => s.codice == contatto.sa_cod).descrizione);
            fb.controls['Sa_Cod'].disable();
          })).subscribe();
          this.prevContatto = {codice: cod_contatto, descrizione: contatto.ragione_Sociale};
        })).subscribe();
      } else {
        fb.controls['Sa_Cod'].enable();
        fb.controls['Cod_Contatto'].patchValue('0', {emitEvent: false});
      }
    }));
  }

  private clearSelezionate(): void {
    this.macchineSelezionate = new Map<ParcoMacchine, string>();
  }

  private disableVisibilityItems(itemArgs: { dataItem: { id: number; name: string; data: any }; }): boolean {
    if (itemArgs.dataItem.id === -1) {
      return false;
    } else if (itemArgs.dataItem.id === 0) {
      // o la macchina non è stata utilizzata, o è stata utilizzata solo dall'azienda proprietaria
      return !(
        this.companiesByWhichIsUsed.length === 0 ||
        (this.companiesByWhichIsUsed.length === 1 && this.companiesByWhichIsUsed.includes(this.objParametriAgenda.Piva))
      );
    } else {
      // o la macchina non è stata utilizzata, o è stata utilizzato solo all'interno dello stesso centro
      return !(
        this.centresOnWhichIsUsed.length === 0 ||
        (this.centresOnWhichIsUsed.length === 1 && this.centresOnWhichIsUsed.includes(itemArgs.dataItem.id))
      );
    }
  }

  private fetchCentresByWhichIsUsed(macchina: ParcoMacchine): Observable<rispostaStandard<number[]>> {
    return this.macchineService.centresOnWhichIsUsed(macchina).pipe(take(1));
  }

  private fetchCompaniesByWhichIsUsed(macchina: ParcoMacchine): Observable<rispostaStandard<string[]>> {
    return this.macchineService.companiesByWichIsUsed(macchina).pipe(take(1));
  }

  private setupDdlCommandsMenu(permessoEdit: boolean, permessoInfo: boolean) {
    this.cmdDropDown = new CommandsDropDownSettings({
      inlineEditBtn: false,
      fullEditBtn: permessoEdit,
      infoBtn: permessoInfo,
      removeBtn: false,
    })

    this.cmdDropDown.addCommand(new GridCommandItem(
      "RicercaDocumenti",
      this.businessLogic.commands.RICERCA_DOCUMENTI,
      'faAnagraficaSearchDocument'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "AggiungiNuovoAllegato",
      this.businessLogic.commands.NUOVO_ALLEGATO,
      'faAnagraficaUploadFile'
    ));

    this.gridPublicService.openCommands
      .pipe(takeUntil(this.signal), filter(macchina => !!macchina))
      .subscribe(macchina => {
        console.log(macchina)
        if (!this.permessoNuovoDocumento) {
          this.cmdDropDown.removeCommand(this.businessLogic.commands.NUOVO_ALLEGATO);
        }

        if (!this.permessoRicercaDocumenti) {
          this.cmdDropDown.removeCommand(this.businessLogic.commands.RICERCA_DOCUMENTI);
        } else {
          this.businessLogic.CheckAttachedDocumentsMacchine(macchina.Piva, macchina.Mac_Cod)
            .pipe(filter(hasAttachments => !hasAttachments))
	          .subscribe(() => this.cmdDropDown.removeCommand(this.businessLogic.commands.RICERCA_DOCUMENTI));
        }
      });
  }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent
      .pipe(takeUntil(this.signal), filter(ev => !!ev))
      .subscribe(ev => {
        switch(ev.command.action) {
          case CommandsDropDownEvents.FULL_EDIT:
            this.macchineGridEventsService.modificaMacchinaAngular(ev.dataItem, false, ev.rowIndex);
            break;
          case this.businessLogic.commands.NUOVO_ALLEGATO:
            this.businessLogic.apriKWindowMacchine(ev.dataItem, this.businessLogic.commands.NUOVO_ALLEGATO);
            break;
          case this.businessLogic.commands.RICERCA_DOCUMENTI:
            this.businessLogic.apriKWindowMacchine(ev.dataItem, this.businessLogic.commands.RICERCA_DOCUMENTI);
            break;
        }
      });
  }
}
