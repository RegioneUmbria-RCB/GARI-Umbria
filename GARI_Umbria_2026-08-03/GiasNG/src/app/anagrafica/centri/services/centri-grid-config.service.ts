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
import { DropdownListItem, DropdownListWithForm, EditingMode,
  GridCustomizations, KendoGridColumn, LoaderType, ModelEntry, RendererGridEvent
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of } from 'rxjs';
import { CentriWrapper, CentroKendoServerResult, KendoCentroModel } from '../centri.models';
import { ConfigTemplate } from 'gias-kendo-grid';
import { catchError, map, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { CentriService } from './centri.service';
import { FormGroup, Validators } from '@angular/forms';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GridPublicService } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { BudgetService } from 'app/Service/Budget/budget.service';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { capValidatorCentriGrid } from 'gias-ui-kit';
import {GiasIstatService} from '../../../Service/istat/gias-istat.service';

@Injectable()
export class CentriHttpService extends AbstractGridConfigService<CentroKendoServerResult> {
  gridId = 'CentriHttpService';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'chiave';
  objParametriAgenda: ObjParametriAgenda;

  /** Optional parameters */
  toolbar = new ToolbarSettings(true, false);
  views = new GridCustomizations({ enabled: true });

  public permessoEdit: boolean;
  public permessoRemove: boolean;
  public permessoInfo: boolean;

  private centriSelezionati: Map<CentroAziendale, string> = new Map<CentroAziendale, string>();

  columns: KendoGridColumn[];
  model: KendoCentroModel;
  comuneColumn: KendoGridColumn;

  aggregates = new AggregateSettings({
    enabled: true,
    descriptors: [
      { field: 'Superficie_Catastale', aggregate: 'sum', format: 'n4' },
      { field: 'Superficie_Convenzionale', aggregate: 'sum', format: 'n4' },
      { field: 'Superficie_Conversione', aggregate: 'sum', format: 'n4' },
      { field: 'Superficie_Biologico', aggregate: 'sum', format: 'n4' },
      { field: 'Superficie_Totale', aggregate: 'sum', format: 'n4' }
    ]
  });

  constructor(injector: Injector,
    private _centri: CentriService,
    private centriService: CentriAziendaliService,
    private objParametriService: ObjParametriAgendaService,
    private permessiUtenteService: PermessiUtenteService,
    private istatService: GiasIstatService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private router: Router,
    private renderer: Renderer2,
    private route: ActivatedRoute,
    private anagraficaService: AnagraficaService,
    private gridPublicSerivce: GridPublicService,
    private translocoService: TranslocoService,
    private treeContainer: TreeContainerService,
    private budgetService: BudgetService) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale, 0);

    this.gridPublicSerivce.changeDetected.pipe(
      takeUntil(this.signal)
    ).subscribe((event: any) => {
      if (event.action == 'add') {
        this.gridPublicSerivce.formGroup.pipe(take(1)).subscribe((fg) => {
          fg.controls['sa_cod'].setValue(0);
        })
      }
    })

    this.gridPublicSerivce.formGroup.pipe(
      tap((fg: FormGroup) => {
        if (!fg)
          return;

        this.initializeFields(fg);

        fg.controls['Stato_Cod'].valueChanges.GiasSubscribe(() => {
          this.onStateChangeUpdateFields(fg).pipe(take(1), map(gestioneGerarchia => {
            if (gestioneGerarchia == 1) {
              let prov = fg.controls["pro_cod_istat"].value;
              this.istatService.leggiComuni(prov).then((comuni) => {
                this.onStateChangeUpdateComune(comuni, fg);
              });
            } else {
              fg.controls["com_cod_istat"].setValue('000');
              fg.controls["com_des"].setValue('000');
            }

            fg.controls['CAP'].updateValueAndValidity({ onlySelf: true, emitEvent: false });
          })).subscribe();


        });

        fg.controls['pro_cod_istat'].valueChanges.GiasSubscribe(() => {

          let prov = fg.controls["pro_cod_istat"].value;
          let stato = fg.controls["Stato_Cod"].value;
          from(this.istatService.leggiProvincie(stato)).pipe(take(1), map(p => {
            //let comDef = p.filter(t => t.Istat_Prov == prov)[0].comuneDefault;
            //fg.controls['com_cod_istat'].setValue(comDef);
            from(this.istatService.leggiStati()).pipe(take(1), map(s => {
              let gestioneGerarchia = s.find(t => t.codice == stato).gestioneGerarchia;
              if (prov != null && prov != "" && (gestioneGerarchia == 1 || stato == '')) {
                if (prov != '000') {
                  fg.controls["com_cod_istat"].enable();
                }
                this.istatService.leggiComuni(prov).then((comuni) => {
                  this.onStateChangeUpdateComune(comuni, fg);
                });
              }
            })).subscribe();
          })).subscribe();
        });

        fg.controls["com_cod_istat"].valueChanges.GiasSubscribe(() => {
          let com = fg.controls["com_cod_istat"].value;
          let prov = fg.controls["pro_cod_istat"].value;
          let stato = fg.controls["Stato_Cod"].value;
          if (com != '' && com != '000' && prov != '' && prov != '000' && (stato == 'IT' || stato == '')) {
            this.istatService.leggiCAP(fg.controls["pro_cod_istat"].value, fg.controls["com_cod_istat"].value,).then((cap) => {
              fg.controls['CAP'].setValue(cap);
            })
          } else {
            fg.controls['CAP'].setValue('00000');
          }
        });


        let statoCodValue = fg.controls['Stato_Cod'].value;
        if (statoCodValue == null) {
          fg.controls['Stato_Cod'].setValue('IT');

          this.insertNonDefinedItemIfNotExists(fg, 'com_cod_istat', 'com_des');
          this.insertNonDefinedItemIfNotExists(fg, 'pro_cod_istat', 'pro_cod');
        }
      })
    ).GiasSubscribe((fb: any) => {
      let a = 0; //Commento per funzione vuota SonarQube
    });

    this.columns = [
      new KendoGridColumn(
        { field: 'sa_nome', title: this.translocoService.translate('Descrizione') },
        { resizable: true, editable: this.permessoEdit, validators: [Validators.required], width: 135 }
      ),
      new KendoGridColumn(
        { field: 'pro_cod_istat', title: this.translocoService.translate('Provincia') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'com_cod_istat', title: this.translocoService.translate('Comune') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'ind_des', title: this.translocoService.translate('Indirizzo') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'frz_des', title: this.translocoService.translate('Frazione') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'CAP', title: this.translocoService.translate('CAP') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135, validators: [capValidatorCentriGrid()] }
      ),
      new KendoGridColumn(
        { field: 'Stato_Cod', title: this.translocoService.translate('Stato') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'note', title: this.translocoService.translate('Note') },
        { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') },
        { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' } }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Convenzionale', title: this.translocoService.translate('SuperficieConvenzionaleAbbr') },
        { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Conversione', title: this.translocoService.translate('SuperficieConversioneAbbr') },
        { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Biologico', title: this.translocoService.translate('SuperficieBiologicoAbbr') },
        { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
      ),
      new KendoGridColumn(
        { field: 'Superficie_Totale', title: this.translocoService.translate('SuperficieTotaleAbbr') },
        { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
      ),
      new KendoGridColumn(
        { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
        { resizable: true, editable: this.permessoEdit, date: { defaultValue: AGRODATAINIZIO }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
      new KendoGridColumn(
        { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
        { resizable: true, editable: this.permessoEdit, date: { defaultValue: AGRODATAFINE }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
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
        { field: 'IndirizzoCompleto', title: this.translocoService.translate('Indirizzo') },
        { resizable: true, editable: false, media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
      ),
    ]

    this.model = {
      chiave: new ModelEntry(CELL_TYPES.STRING, false),
      sa_cod: new ModelEntry(CELL_TYPES.STRING, false),
      sa_nome: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      Piva: new ModelEntry(CELL_TYPES.STRING, false),
      Rag_Soc: new ModelEntry(CELL_TYPES.STRING, false),
      cod_indirizzo: new ModelEntry(CELL_TYPES.STRING, false),
      ind_des: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      frz_des: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      CAP: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      Stato2: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      Stato_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, this.permessoEdit),
      note: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      com_des: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      pro_cod: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      pro_cod_istat: new ModelEntry(CELL_TYPES.DROPDOWNLIST, this.permessoEdit),
      com_cod_istat: new ModelEntry(CELL_TYPES.DROPDOWNLIST, this.permessoEdit),
      CodiceOperatoreBio: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      tipoAttivitaCod: new ModelEntry(CELL_TYPES.STRING, this.permessoEdit),
      Superficie_Catastale: new ModelEntry(CELL_TYPES.NUMBER, false),
      Superficie_Convenzionale: new ModelEntry(CELL_TYPES.NUMBER, false),
      Superficie_Conversione: new ModelEntry(CELL_TYPES.NUMBER, false),
      Superficie_Biologico: new ModelEntry(CELL_TYPES.NUMBER, false),
      Superficie_Totale: new ModelEntry(CELL_TYPES.NUMBER, false),
      Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, this.permessoEdit),
      Validita_Fine: new ModelEntry(CELL_TYPES.DATE, this.permessoEdit),
      Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
      Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
      Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
      Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
      IndirizzoCompleto: new ModelEntry(CELL_TYPES.STRING, false),
    }

    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = this.permessoEdit;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.permessoEdit;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: this.permessoEdit,
      removeBtn: this.permessoRemove,
    });

    /*this.dettagliColumn = new DettagliColumnSettings({
        editBtn: this.permessoEdit,
        edit: this.onTemplateBtnClick.bind(this),
    })*/

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: this.permessoEdit,
      infoBtn: this.permessoInfo,
    });
    this.gridPublicSerivce.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case CommandsDropDownEvents.FULL_EDIT:
          this.onTemplateBtnClick(ev.dataItem);
          break;
      }
    })

    this.resizable.autoFitColumns = false;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title = ' ';
    this.resizable.isResizable = true;
    this.selectable.selectable.enabled = true;
    this.groups.groupable.enabled = false;
    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
      this.cmdColumn.editBtn = false;
      this.toolbar.newItem = false;
    }
  }

  insertNonDefinedItemIfNotExists(fg: FormGroup, formCtrlName: string, formCtrlValue: string) {
    let col = this.columns.find(s => s.field === formCtrlName);
    let itemNotDefined = col.ddl.data.find(s => s.id === '000');
    if (itemNotDefined == null)
      col.ddl.data = [new DropdownListItem('000', 'Non Definita')];

    fg.controls[formCtrlName].setValue('000');
    fg.controls[formCtrlValue].setValue('Non Definita');
  }

  initializeFields(fg: FormGroup) {
    from(this.istatService.leggiStati()).pipe(take(1), map(s => {
      let stato = fg.controls["Stato_Cod"].value;
      let gestioneGerarchia = s.filter(t => t.codice == stato)[0].gestioneGerarchia;
      if (gestioneGerarchia != 1 && stato != '') {
        fg.controls["com_cod_istat"].disable();
        fg.controls["pro_cod_istat"].disable();
        fg.controls["pro_cod_istat"].setValue('000');
        fg.controls["com_cod_istat"].setValue('000');
        fg.controls['CAP'].setValue('00000');
        fg.controls["com_des"].setValue('');
      }
    })).subscribe();
  }
  onStateChangeUpdateFields(fg: FormGroup): Observable<number> {
    let stato = fg.controls["Stato_Cod"].value;
    return from(this.istatService.leggiStati()).pipe(take(1), map(s => {
      let gestioneGerarchia = s.find(t => t.codice === stato).gestioneGerarchia;
      if (gestioneGerarchia != 1 && stato != '') {
        let col = this.columns.find(s => s.field === 'pro_cod_istat');
        //col.ddl.reload.next(true);
        fg.controls["com_cod_istat"].disable({ emitEvent: false });
        fg.controls["pro_cod_istat"].disable({ emitEvent: false });
        fg.controls["pro_cod_istat"].setValue('000', { emitEvent: false });
        fg.controls["com_cod_istat"].setValue('000', { emitEvent: false });
        fg.controls['CAP'].setValue('00000');
        fg.controls["com_des"].setValue('');
      } else {
        fg.controls["com_cod_istat"].enable({ emitEvent: false });
        fg.controls["pro_cod_istat"].enable({ emitEvent: false });
        fg.controls["pro_cod_istat"].setValue(stato == 'IT' ? '000' : stato + '000', { emitEvent: false });
        fg.controls["com_cod_istat"].setValue(stato == 'IT' ? '000' : stato + '000', { emitEvent: false });

        this.setProvComFromCompany(fg);

        // fg.controls["pro_cod_istat"].setValue('000');
        // fg.controls["com_cod_istat"].setValue('000');

        // fg.controls['CAP'].setValue('00000');
        // fg.controls["com_des"].setValue('');
        // let col = this.columns.find(s => s.field === 'pro_cod_istat');
        // col.ddl.reload.next(true);
      }
      return gestioneGerarchia;
    }))
  }

  setProvComFromCompany(fg: FormGroup) {
    const objP = this.objParametriService.getObjParamValue();
    from(this.impreseService.leggiImpresa(objP)).pipe(
      take(1),
      map((val) => {
        return val.RispostaStringa;
      }),
      tap((impresa) => {
        let i = impresa;
        const indirizzo = impresa.indirizzi[0].indirizzo;
        const prov = indirizzo.istatComune.prov;
        const com = indirizzo.istatComune.com;

        let colProv = this.columns.find(s => s.field === 'pro_cod_istat');
        let colCom = this.columns.find(s => s.field === 'com_cod_istat');
        if (fg.controls['Stato_Cod'].value == indirizzo.stato.codice) {
          fg.controls["pro_cod_istat"].setValue(prov);
        }
        colProv.ddl.reload.next(true);

        this.istatService.leggiComuni(prov).then((comuni) => {
          setTimeout(() => {
            let comS = comuni.find(s => s.codice === com);
            let colCom = this.columns.find(s => s.field === 'com_cod_istat');
            if (fg.controls['Stato_Cod'].value == indirizzo.stato.codice) {
              fg.controls["com_cod_istat"].setValue(comS.codice);
              fg.controls["com_des"].setValue(comS.descrizione);
            }
            colCom.ddl.reload.next(true);
          }, 250);
        });

      })
    ).subscribe();
  }

  onStateChangeUpdateComune(comuni, fg: FormGroup) {
    let stato = fg.controls["Stato_Cod"].value;
    from(this.istatService.leggiProvincie(stato)).pipe(take(1), map(p => {
      let prov = p.filter(t => t.Istat_Prov == fg.controls["pro_cod_istat"].value)[0];
      from(this.istatService.leggiComuni(prov.Istat_Prov)).pipe(take(1), map(c => {
        let com = c.find(s => s.codice === (prov.comuneDefault));
        let col = this.columns.find(s => s.field === 'com_cod_istat');
        fg.controls["com_cod_istat"].setValue(com.codice);
        fg.controls["com_des"].setValue(com.descrizione);
        col.ddl.reload.next(true);
      })).subscribe();
    })).subscribe();
  }

  read(): Observable<CentroKendoServerResult> {
    this.msgWarningDelete();
    // azzero Sa_Cod, altrimenti non vengono caricati tutti i centri dell'azienda
    let objParametriAgenda = this.objParametriService.getObjParamValue();
    objParametriAgenda.Sa_Cod = 0
    objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    let data = this.anagraficaService.filterData.getValue()
    let DataFiltro = AGRODATAINIZIO;
    if (data.filter) {
      objParametriAgenda.Data = data.data;
    }
    this.objParametriService.changeObjParametriAgenda(objParametriAgenda);

    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef })
    return this._centri.leggiCentri(objParametriAgenda.Piva).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        return of()
      }),
      map((data: CentriWrapper) => this.transform(data))
    );
  }

  private transform(data: CentriWrapper): CentroKendoServerResult {
    this.handleDropdowns();
    this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
    data?.centri?.forEach(val => {
      if (val.Stato_Cod == 'IT') {
        val.IndirizzoCompleto = val.ind_des + ' - ' + val.com_des + ' (' + val.pro_cod + ')' + ' CAP:' + val.CAP;
      } else {
        val.IndirizzoCompleto = val.ind_des + ' (' + val.frz_des + ') ZIP: ' + val.CAP;
      }

    })

    return {
      model: this.model,
      rows: data.centri,
      columns: this.columns
    };
  }

  perform(actionType: HttpAction, item: any): Observable<any[]> {
    let flag_cancellazione;
    let httpRisp: Observable<any>;
    if (actionType === HttpAction.UPDATE) {
      let objPAgenda = new ObjParametriAgenda
      objPAgenda.Piva = item['Piva'];
      objPAgenda.Sa_Cod = item['sa_cod'];
      httpRisp = this.centriService.leggiCentro(objPAgenda).pipe(
        take(1),
        switchMap((objCentro, index) => {
          return this._centri.modificaCancellaCentroAziendale(item, flag_cancellazione = false, this.loadingService, this.gridPublicSerivce.gridElRef, objCentro)
        })
      )
      //this._centri.modificaCancellaCentroAziendale(item, flag_cancellazione = false, this.loadingService, this.gridPublicSerivce.gridElRef);
    } else if (actionType === HttpAction.CREATE) {
      httpRisp = this._centri.modificaCancellaCentroAziendale(item, flag_cancellazione = false, this.loadingService, this.gridPublicSerivce.gridElRef, null)
    } else if (actionType === HttpAction.REMOVE) {
      httpRisp = this._centri.modificaCancellaCentroAziendale(item, flag_cancellazione = true, this.loadingService, this.gridPublicSerivce.gridElRef, null);
    }

    return httpRisp.pipe(map((risposta) => {
      this.treeContainer.selectedImpresaChangedSoUpdateTree = true;

      if (!risposta.RispostaOK) {
        //let messaggio = this.transloco.translate('centro.CentroAziendaleFailedDelete', { });
        //this.giasMessageService.errorMessage('Non è stato possibile eliminare il centro aziendale selezionato');
        if (flag_cancellazione) {
          this.addCentroSelezionato(item, "Non è stato possibile eliminare il centro");
        }
        return of([]);
      }
      if (!flag_cancellazione) {
        //let messaggio = this.transloco.translate('centro.CentroAziendaleModifiedSuccessfully', { });
        this.giasMessageService.successMessage(this.translocoService.translate('CentroAziendaleCorrettamenteModificato'));
      }
      else {
        //let messaggio = this.transloco.translate('centro.CentroAziendaleRemovedSuccessfully', { });
        //this.giasMessageService.successMessage(this.translocoService.translate('CentroAziendaleCorrettamenteCancellato'));
        this.addCentroSelezionato(item, "");
      }
      return of([]);
    }), switchMap((e) => { return of([]) }));

  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let a = gridElRef;
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      const row = visibleRows[index] as HTMLElement;
      if (el.Attivo === 0) {
        this.renderer.addClass(row, 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(row, 'nonAttivo');
      }
    });
    this.anagraficaService.applicaFiltri();
  }

  private handleDropdowns() {
    let col;
    let data;
    col = this.columns.find((c) => c.field === 'pro_cod_istat')
    data = [];
    col.ddl = new DropdownListWithForm('Istat_Prov', 'pro_cod_istat', 'Provincia_Des', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'pro_cod';
    col.ddl.loadFunction = this.caricaProvincie.bind(this);

    col = this.columns.find(s => s.field === 'Stato_Cod');
    data = [];
    col.ddl = new DropdownListWithForm('codice', 'Stato_Cod', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Stato2';
    col.ddl.loadFunction = this.caricaStati.bind(this);

    col = this.columns.find((c) => c.field === 'com_cod_istat')
    this.comuneColumn = col;
    const newLocal = data = [];
    col.ddl = new DropdownListWithForm('codice', 'com_cod_istat', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'com_des';
    col.ddl.loadFunction = this.caricaComuni.bind(this);
  }

  onTemplateBtnClick(dataItem) {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = (<string>dataItem.chiave).split('_')[0];
    const sa_cod = (<string>dataItem.chiave).split('_')[1];
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
    //this.router.navigate(['/Anagrafica/Centri/Centri-Edit']);
    this.router.navigate(['Centri-Edit'], { relativeTo: this.route });
  }

  onInfoBtnClick(dataItem) {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = (<string>dataItem.chiave).split('_')[0];
    const sa_cod = (<string>dataItem.chiave).split('_')[1];
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
    // this.router.navigate(['/Anagrafica/Centri/Centri-Edit']);
    this.router.navigate(['Centri-Edit'], { relativeTo: this.route });
  }

  caricaComuni(dataItem: any) {
    return from(this.istatService.leggiComuni(dataItem.pro_cod_istat));
  }

  caricaProvincie(dataItem: any) {
    return from(this.istatService.leggiProvincie(dataItem.Stato_Cod));
  }

  caricaStati(dataItem: any) {
    return from(this.istatService.leggiStati());
  }


  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;

    msg = this.translocoService.translate('anagrafica.Delete_Msg')
    opts.data.forEach(r => {
      msg = msg.concat('\n' + r['sa_nome']);
    })

    return msg;
  }

  addCentroSelezionato(centro: CentroAziendale, error: string) {
    this.centriSelezionati.set(centro, error);
  }

  removeCentroSelezionato(centro: CentroAziendale) {
    this.centriSelezionati.delete(centro);
  }

  getCentriSelezionati() {
    return this.centriSelezionati;
  }

  clearSelezionati(): void {
    this.centriSelezionati = new Map<CentroAziendale, string>();
  }

  msgWarningDelete() {
    let msgFailure = this.translocoService.translate('ErroreCancellazioneCentri')
    let msgSuccess = this.translocoService.translate('SuccessoCancellazioneCentri')
    let displaySuccess = false;
    let displayFailure = false;
    this.getCentriSelezionati().forEach((error, cen) => {
      if (error == "") {
        msgSuccess = msgSuccess.concat('\n' + cen['sa_nome']);
        displaySuccess = true;
      } else {
        msgFailure = msgFailure.concat('\n' + cen['sa_nome'] + ':' + error);
        displayFailure = true;
      }
    })
    if (displaySuccess) {
      this.giasMessageService.successMessage(msgSuccess);
    }
    if (displayFailure) {
      //this.giasMessageService.warningMessage(msgFailure);
      this.giasDialogService.baseError("", msgFailure);
    }
    this.clearSelezionati();
  }

}
