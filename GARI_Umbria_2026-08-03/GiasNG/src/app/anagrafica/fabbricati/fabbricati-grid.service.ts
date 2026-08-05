import { Injectable, Injector, Renderer2 } from '@angular/core';
import {
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn, LoaderType, ModelEntry, RendererGridEvent
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { catchError, map, take, tap } from 'rxjs/operators';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { AnagraficaService } from '../anagrafica.service';
import { TranslocoService } from '@jsverse/transloco';
import { Validators } from '@angular/forms';
import { FabbricatiModel, FabbricatoKendoServerResult } from "./fabbricati.model";
import { FabbricatiService } from "../../Service/Anagrafica/fabbricati.service";
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from "../../Model/siti.enum";
import { GestioneRichiesteService } from "../../Service/gestione-richieste.service";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { SelezionaCentroComponent } from "./seleziona-centro/seleziona-centro.component";
import { SelezionaCentroService } from "./seleziona-centro/seleziona-centro.service";
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { BudgetService } from "../../Service/Budget/budget.service";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class FabbricatiGridService extends AbstractGridConfigService<FabbricatoKendoServerResult> {
  gridId = 'AnagraficaFabbricatiGrid';
  loader = LoaderType.SERVICE;
  editingMode = EditingMode.IN_LINE;
  rowId = 'chiave';

  private fabbricatiSelezionati: Map<Fabbricato, string> = new Map<Fabbricato, string>();

  cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });

  objParametriAgenda: ObjParametriAgenda;

  originalEditedLine: any;

  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Sa_Nome', title: this.translocoService.translate('Centro') }, { resizable: true, editable: true, width: 250, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Fabbricato_des', title: this.translocoService.translate('Fabbricato') }, { resizable: true, editable: true, width: 250, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Tipo', title: this.translocoService.translate('Tipo') }, { resizable: true, editable: true, width: 250, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130, date: { defaultValue: AGRODATAINIZIO } }),
    new KendoGridColumn({ field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135, date: { defaultValue: AGRODATAFINE } }),
    new KendoGridColumn({ field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }),
    new KendoGridColumn({ field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
    new KendoGridColumn({ field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130 }),
    new KendoGridColumn({ field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }),
  ];

  kendoModel: FabbricatiModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    Piva: new ModelEntry(CELL_TYPES.STRING, false),
    Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Fabbricato_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Fabbricato_des: new ModelEntry(CELL_TYPES.STRING, true),
    Sa_Nome: new ModelEntry(CELL_TYPES.STRING, false),
    Tipo_Fabbricato_Cod: new ModelEntry(CELL_TYPES.STRING, false),
    Tipo: new ModelEntry(CELL_TYPES.STRING, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false)
  };

  public permessoEdit: boolean;
  public permessoRemove: boolean;
  public permessoInfo: boolean;

  constructor(
    injector: Injector,
    private fabbricatiService: FabbricatiService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private renderer: Renderer2,
    private giasMessageService: GiasMessageService,
    private anagraficaService: AnagraficaService,
    private permessiUtenteService: PermessiUtenteService,
    private dialogService: DialogService,
    private selezionaCentroService: SelezionaCentroService,
    private translocoService: TranslocoService,
    private budgetService: BudgetService,
    private gestioneRichiesteService: GestioneRichiesteService) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato, 0);

    this.handleCustomizations();

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      if (ev.command.action === CommandsDropDownEvents.FULL_EDIT)
        this.onTemplateBtnClick(ev.dataItem);
    })

    this.gridPublicService.changeDetected.pipe(
      tap((event: any) => {
        if (event.action == 'info') {
          this.info(event.dataItem);
        } else if (event.action == 'remove') {
          this.remove(event.dataItem);
        }
      })
    ).GiasSubscribe(() => {
      let a = 0; //Commento per funzione vuota SonarQube
    });

  }

  read(): Observable<FabbricatoKendoServerResult> {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.Data = AGRODATAINIZIO;
    let data = this.anagraficaService.filterData.getValue()
    if (data.filter) {
      this.objParametriAgenda.Data = data.data;
    }
    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef })
    return this.fabbricatiService.Leggi_Fabbricati(this.objParametriAgenda).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        return of()
      }),
      map((data) => {

        const result = new FabbricatoKendoServerResult(this.kendoModel, this.kendoColumns, data);
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef })
        return result;

      }));
  };


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

  perform(actionType: HttpAction, items: any): Observable<any> {
    return of("")
  }


  public onTemplateBtnClick(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgenda.Sa_Cod = dataItem.Sa_Cod;
    this.objParametriAgenda.Fabbricato = dataItem.Fabbricato_Cod;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    let operazione = 2;
    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Fabbricato).then((val) => {
        window.location.href = val;
      });
  }

  public onNuovo() {
    const dialog: DialogRef = this.dialogService.open({
      title: this.translocoService.translate('SelezionaCentro'),
      content: SelezionaCentroComponent,
      width: 400,
      actions: [
        { text: this.translocoService.translate('CreaFabbricato'), primary: true },
        { text: this.translocoService.translate('Annulla') }
      ]
    });

    dialog.result.pipe(take(1)).subscribe((result) => {
      if ((<any>result).primary == true) {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.Fabbricato = 0;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        this.objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati;
        const centroSelezionato = this.selezionaCentroService.centroSelezionatoSubject.getValue()
        if (centroSelezionato != null && centroSelezionato != undefined && centroSelezionato.codice != 0) {
          this.objParametriAgenda.Sa_Cod = centroSelezionato.codice;
          this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
          this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
          this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Anagrafica_Fabbricato).then((val) => {
            window.location.href = val;
          });
        }
      }
    })
  }

  public info(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgenda.Sa_Cod = dataItem.Sa_Cod;
    this.objParametriAgenda.Fabbricato = dataItem.Fabbricato_Cod;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    let piva = (<string>dataItem.chiave).split('_')[0];
    let cod_contatto = (<string>dataItem.chiave).split('_')[2];
    let operazione = 0;
    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Fabbricato,
      [
        { key: 'codcont', value: cod_contatto, codifica: true },
        { key: 'piva', value: piva, codifica: true },
        { key: 'o', value: operazione.toString(), codifica: true }
      ]).then((val) => {
        window.location.href = val;
      });
    //this.gestionePassaggioPaginaContatto((<string>dataItem.chiave).split('_')[0], (<string>dataItem.chiave).split('_')[2], 0).subscribe((val) => window.location.href = val);
  }

  public remove(dataItem) {
    this.giasMessageService.warningMessage(this.translocoService.translate('FunzioneNonAncoraAttivata'));
  }

  handleCustomizations(): void {

    // this.selectable.selectable.checkboxOnly = false;
    // this.selectable.selectable.enabled = false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.permessoEdit;
    this.toolbar.resetChanges = false;
    this.toolbar.newItem = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: this.permessoRemove,
    });

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: this.permessoEdit,
      infoBtn: this.permessoInfo,
    });

    this.resizable.autoFitColumns = true;
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

  addFabbricatoSelezionato(fabbricato: Fabbricato, error: string) {
    this.fabbricatiSelezionati.set(fabbricato, error);
  }

  removeFabbricatoSelezionato(fabbricato: Fabbricato) {
    this.fabbricatiSelezionati.delete(fabbricato);
  }

  getFabbricatiSelezionati() {
    return this.fabbricatiSelezionati;
  }

  clearSelezionati(): void {
    this.fabbricatiSelezionati = new Map<Fabbricato, string>();
  }

  msgWarningDelete() {
    if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Delete) {
      let msgFailure = this.translocoService.translate('ErroreCancellazioneFabbricati')
      let msgSuccess = this.translocoService.translate('SuccessoCancellazioneFabbricati')
      let displaySuccess = false;
      let displayFailure = false;
      this.getFabbricatiSelezionati().forEach((error, fab) => {
        if (error == "") {
          msgSuccess = msgSuccess.concat('\n' + fab['Sa_Nome']);
          displaySuccess = true;
        } else {
          msgFailure = msgFailure.concat('\n' + fab['Sa_Nome'] + ':' + error);
          displayFailure = true;
        }
      })
      if (displaySuccess) {
        this.giasMessageService.successMessage(msgSuccess);
      }
      if (displayFailure) {
        this.giasMessageService.warningMessage(msgFailure);
      }
    }
    this.clearSelezionati();
  }

}
