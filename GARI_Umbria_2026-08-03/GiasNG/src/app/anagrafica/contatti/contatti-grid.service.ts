import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AGRODATAINIZIO, AGRODATAFINE, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { CommandsColumnSettings, CommandsDropDownEvents, CommandsDropDownSettings, ToolbarSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { AnagraficaService } from '../anagrafica.service';

import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { ContattiFactoryService, CONTATTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/contatti.factory.service';
import { ContattiEventsService } from './contatti-events.service';
import {enum_Impostazioni_Utenti} from "../../Model/Impostazioni_Utenti.enum";
import { ObjParametriAgenda } from 'gias-ui-kit';

export class KendoContattiModel extends KendoGridModel {
  chiave: ModelEntry;
  CF: ModelEntry;
  sa_cod: ModelEntry;
  Contatto_Des: ModelEntry;
  Rapporto_Des: ModelEntry;
  Tipo_Contatto: ModelEntry;
  Impresa: ModelEntry;
  Settore_Des: ModelEntry;
  Data_Creazione: ModelEntry;
  Utente_Creazione: ModelEntry;
  Data_Modifica: ModelEntry;
  Utente_Modifica: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
}

export class ContattiServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class ContattiHttpService extends AbstractGridConfigService<ContattiServerResult>{
  gridId = 'ContattiHttpService';

  loader: LoaderType = LoaderType.SERVICE;

  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'chiave';
  cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });

  objParametriAgenda: ObjParametriAgenda;
  GridContattiServerResult: ContattiServerResult;
  GridContattiRows: KendoGridRow[];

  private renderer: Renderer2

  GridContattiModel: KendoContattiModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    CF: new ModelEntry(CELL_TYPES.STRING, false),
    sa_cod: {type: CELL_TYPES.STRING},
    Contatto_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Rapporto_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Tipo_Contatto: new ModelEntry(CELL_TYPES.STRING, false),
    Impresa: new ModelEntry(CELL_TYPES.STRING, false),
    Settore_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Attivita_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, true),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, true),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true)
  };

  GridContattiColumns: KendoGridColumn[] = [
    // new KendoGridColumn(
    //     { field: 'chiave', title: '' },
    //     { resizable: true, editable: false, width: 120 }
    // ),
    new KendoGridColumn(
      { field: 'CF', title: this.translocoService.translate('PartitaIVA') },
      { resizable: true, editable: false, width: 120 }
    ),
    // new KendoGridColumn(
    //     { field: 'sa_cod', title: 'Pubblico' },
    //     { resizable: true, editable: false, width: 120 }
    // ),
    new KendoGridColumn(
      { field: 'Contatto_Des', title: this.translocoService.translate('Contatto') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Rapporto_Des', title: this.translocoService.translate('Rapporto') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Tipo_Contatto', title: this.translocoService.translate('Tipo') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Impresa', title: this.translocoService.translate('Impresa') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Settore_Des', title: this.translocoService.translate('CodiceContatto') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Attivita_Des', title: this.translocoService.translate('Codice') },
      { resizable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }
    ),
    new KendoGridColumn(
      { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 155 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 155, date: { defaultValue: AGRODATAINIZIO } }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 155, date: { defaultValue: AGRODATAFINE } }
    ),
  ];

  //loadingService: LoadingService;

  constructor(injector: Injector,
              private gestioneRichiesteService: GestioneRichiesteService,
              @Inject(CONTATTI_SERVICE_TOKEN) private contattiService: ContattiFactoryService,
              private objParametriAgendaService: ObjParametriAgendaService,
              private permessiUtenteService: PermessiUtenteService,
              private translocoService: TranslocoService,
              private anagraficaService: AnagraficaService,
              private contattiEventsService: ContattiEventsService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.gridPublicService.changeDetected.pipe(
      tap((event: any) => {
        if (event.action == 'info') {
          this.contattiEventsService.info(event.dataItem);
        }
        else if (event.action == 'userBind') {
          this.contattiEventsService.bindUser(event.dataItem);
        } else if (event.action == 'remove') {
          this.contattiEventsService.remove(event.dataItem)
            .then(isDeleted => {
              if (isDeleted) this.gridPublicService.refresh(true);
            });
        }
      })
    ).GiasSubscribe(() => {
      let a = 0; //Commento per funzione vuota SonarQube
    });
  }

  handleCustomizations(): void {
      let ImpostazioneImpedisciEliminazione = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_IMPEDISCI_ELIMINAZIONE_CONTATTI_E_RISORSE_UMANE)
      let impedisciEliminazione: boolean = false;
      if (ImpostazioneImpedisciEliminazione?.Valore == '1'){
        impedisciEliminazione = true;
      }
    const permessoEdit: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Contatto, 2);
    const permessoRemove: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Contatto, 2) && !impedisciEliminazione;
    const permessoInfo: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Contatto, 0);
    const permessoUserBind: boolean = true;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: permessoRemove,
    });

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: permessoEdit,
      infoBtn: permessoInfo,
      userBindBtn: permessoUserBind,
    });
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      if (ev.command.action === CommandsDropDownEvents.FULL_EDIT)
        this.contattiEventsService.onTemplateBtnClick(ev.dataItem);
    })

    this.resizable.autoFitColumns = false;
    this.selectable.columnSettings.showSelectAll=true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title=' ';
    this.resizable.isResizable = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = permessoEdit;
    this.views.enabled = true;
    this.groups.groupable.enabled = false;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
      this.cmdColumn.editBtn = false;
      this.toolbar.newItem = false;
    }

  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    return of([])
  }

  read(): Observable<ContattiServerResult> {
    const objAgenda=new ObjParametriAgenda;

    objAgenda.Piva = this.objParametriAgenda.Piva;
    objAgenda.Sa_Cod = 0;
    objAgenda.Campo_Cod = 0;
    objAgenda.Appezza = 0;
    objAgenda.Id_Reg = 0;
    objAgenda.Progetto_Cod = 0;
    objAgenda.Data = AGRODATAINIZIO;
    let data = this.anagraficaService.filterData.getValue()
    if (data.filter){
      objAgenda.Data = data.data;
    }
    if (this.loadingService) {
      this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
    }

    return this.contattiService.leggiNG(objAgenda).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        return of()
      }),
      map((result => {
        if (this.loadingService) {
          this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        }

        this.GridContattiServerResult = new ContattiServerResult((<any>result), this.GridContattiColumns, this.GridContattiModel);
        return this.GridContattiServerResult;
      }))
    );
  }

  override applyRendererRules(optsç: RendererGridEvent): void {
    // const { grid, gridElRef } = { ...opts };
    // let a = gridElRef;
    // let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    // grid.view.forEach((el, index) => {
    //     if (el.Attivo === 0) {
    //         this.renderer.addClass(visibleRows[index], 'nonAttivo');
    //     }
    //     if (el.Attivo === 1) {
    //         this.renderer.removeClass(visibleRows[index], 'nonAttivo');
    //     }
    // });
    this.anagraficaService.applicaFiltri();
  }

}
