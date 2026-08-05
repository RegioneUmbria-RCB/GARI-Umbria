import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import {
  AggregateSettings,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings, GridCommandItem,
  RemoveMultipleRowsParams,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListItem,
  DropdownListWithForm, EditingMode,
  KendoGridColumn, LoaderType, ModelEntry, RendererGridEvent
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of } from 'rxjs';
import { ImpresaKendoServerResult, ImpresaModel } from './imprese.model';
import { ConfigTemplate } from 'gias-kendo-grid';
import { catchError, filter, map, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { ImpresaEditService } from './impresa-edit/impresa-edit.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { PivaValidatorService } from './piva-validator.service';
import { DeleteMessageService } from 'app/Service/delete-message.service';
import { enum_CodiciAnagrafe, enum_RapportiContabili_SaCod, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { AnagraficaService } from '../anagrafica.service';
import { TranslocoService } from '@jsverse/transloco';
import { FormGroup, Validators } from '@angular/forms';
import { CodiciAnagrafeValori } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { RisorseUmane } from '../../Model/anagrafiche/RisorseUmane';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ConfigurazioneSitiService } from '../../Service/configurazione-siti.service';
import { GruppoRaccolta } from "../../Model/metaschema/GruppoRaccolta";
import { AnagraficaBusinessLogicService } from "../services/anagrafica-business-logic.service";
import { GruppiRaccoltaService } from "../../Service/GruppiRaccolta/gruppi-raccolta.service";
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { IntervalloTemporale } from '../../Model/anagrafiche/IntervalloTemporale';
import { BudgetService } from 'app/Service/Budget/budget.service';
import { GiasIstatService } from '../../Service/istat/gias-istat.service';
import { ContattiService, ImpostazioneSemplice } from 'app/Service/Anagrafica/contatti.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import {handleDdlConfig} from '../../Utility/Template/kendo-grid/grid-dropdown';
import {RapportoContabile} from '../../Model/anagrafiche/RapportoContabile';
import {Utente_Impostazioni} from '../../Model/utente/utente_impostazioni';

@Injectable()
export class ImpreseGridHttpService extends AbstractGridConfigService<ImpresaKendoServerResult> {
  gridId = 'ImpreseGridHttpService';
  loader = LoaderType.SERVICE;
  editingMode = EditingMode.IN_LINE;
  rowId = 'chiave';
  cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false, onDisableInfoBtn: () => false });

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

  objParametriAgenda: ObjParametriAgenda;
  originalEditedLine: any;

  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Rag_Soc', title: this.translocoService.translate('RagioneSociale') },
      { resizable: true, editable: true, width: 250, validators: [Validators.required] }
    ),
    new KendoGridColumn(
      { field: 'partitaIvaReale', title: this.translocoService.translate('PartitaIVA') },
      { resizable: true, editable: true, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Codice_Cuaa', title: this.translocoService.translate('CodiceUnicoAziendaAgricolaSigla') },
      { resizable: true, editable: true, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Codice_Socio', title: this.translocoService.translate('CodiceSocio') },
      { resizable: true, editable: true, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Piva_Padre', title: this.translocoService.translate('CooperativaReferente') },
      { resizable: true, editable: true, width: 250 }
    ),
    new KendoGridColumn(
      { field: 'codice_iscrizione_libro_soci', title: this.translocoService.translate('NumeroLibroSoci') },
      { resizable: true, editable: true, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'data_iscrizione_libro_soci', title: this.translocoService.translate('DataIscrizioneLibroSoci') },
      { resizable: true, editable: true, width: 140, date: { defaultValue: AGRODATAFINE } }
    ),
    new KendoGridColumn(
      { field: 'Stato_Cod', title: this.translocoService.translate('Stato') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 80 }
    ),
    new KendoGridColumn(
      { field: 'Pro_Cod_Istat', title: this.translocoService.translate('Provincia') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 105 }
    ),
    new KendoGridColumn(
      { field: 'Com_Cod_Istat', title: this.translocoService.translate('Comune') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 175 }
    ),
    new KendoGridColumn(
      { field: 'frz_des', title: this.translocoService.translate('Frazione') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100 }
    ),
    new KendoGridColumn(
      { field: 'ind_des', title: this.translocoService.translate('Indirizzo') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 205 }
    ),
    new KendoGridColumn(
      { field: 'Cap', title: this.translocoService.translate('CAP') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 80 }
    ),
    new KendoGridColumn(
      { field: 'Rapporti_Codice', title: this.translocoService.translate('RapportoContabile') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150 }
    ),
    new KendoGridColumn(
      { field: 'Tipo', title: this.translocoService.translate('Tipo') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150 }
    ),
    new KendoGridColumn(
      { field: 'Rag_Soc_Proprietario', title: this.translocoService.translate('AziendaProprietaria') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150 }
    ),
    new KendoGridColumn(
      { field: 'GruppoRaccolta_Cod', title: this.translocoService.translate('GruppoRaccolta') },
      { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 80 }
    ),
    new KendoGridColumn(
      { field: 'Superficie_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') },
      { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0 }, }
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
      { resizable: true, editable: true, date: { defaultValue: AGRODATAINIZIO }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
      { resizable: true, editable: true, date: { defaultValue: AGRODATAFINE }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }
    ),
    new KendoGridColumn(
      { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130 }
    ),
    new KendoGridColumn(
      { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
    ),
    new KendoGridColumn(
      { field: 'Attivo', title: this.translocoService.translate('Attivo') },
      { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 85 }
    ),
    new KendoGridColumn(
      { field: 'IndirizzoCompleto', title: this.translocoService.translate('Indirizzo') },
      { resizable: true, editable: false, media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)', width: 250 }
    ),
  ];

  kendoModel: ImpresaModel = {
    Rag_Soc: new ModelEntry(CELL_TYPES.STRING, true),
    Piva: new ModelEntry(CELL_TYPES.STRING, true),
    partitaIvaReale: new ModelEntry(CELL_TYPES.STRING, true),
    Codice_Cuaa: new ModelEntry(CELL_TYPES.STRING, true),
    Codice_Socio: new ModelEntry(CELL_TYPES.STRING, true),
    Piva_Padre: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Rag_Soc_Padre: new ModelEntry(CELL_TYPES.STRING, false),
    codice_iscrizione_libro_soci: new ModelEntry(CELL_TYPES.STRING, true),
    data_iscrizione_libro_soci: new ModelEntry(CELL_TYPES.DATE, true),
    Stato_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Stato: new ModelEntry(CELL_TYPES.STRING, false),
    Pro_Cod_Istat: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Provincia: new ModelEntry(CELL_TYPES.STRING, false),
    Com_Cod_Istat: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Comune: new ModelEntry(CELL_TYPES.STRING, false),
    frz_des: new ModelEntry(CELL_TYPES.STRING, true),
    ind_des: new ModelEntry(CELL_TYPES.STRING, true),
    Cap: new ModelEntry(CELL_TYPES.STRING, true),
    Rapporti_Codice: new ModelEntry(CELL_TYPES.MULTI_DROPDOWNLIST, true),
    Rapporti_Des: new ModelEntry(CELL_TYPES.STRING, true),
    Tipo: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    TipoDes: new ModelEntry(CELL_TYPES.STRING, false),
    Rag_Soc_Proprietario: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Rag_Soc_ProprietarioDes: new ModelEntry(CELL_TYPES.STRING, false),
    GruppoRaccolta_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Superficie_Catastale: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Convenzionale: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Conversione: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Biologico: new ModelEntry(CELL_TYPES.NUMBER, false),
    Superficie_Totale: new ModelEntry(CELL_TYPES.NUMBER, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
    Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
    Attivo: new ModelEntry(CELL_TYPES.STRING, false),
    IndirizzoCompleto: new ModelEntry(CELL_TYPES.STRING, false)
  };

  public permessoEdit: boolean;
  public permessoRemove: boolean;
  public permessoInfo: boolean;
  private readonly permessoNuovoDocumento: boolean;
  private readonly permessoRicercaDocumenti: boolean;
  defaultParentCompany = null;

  constructor(
    injector: Injector,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private impreseEditService: ImpresaEditService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private istatService: GiasIstatService,
    private masterService: MasterService,
    private pivaValidatorService: PivaValidatorService,
    private renderer: Renderer2,
    private router: Router,
    private deleteMessageService: DeleteMessageService,
    private giasMessageService: GiasMessageService,
    private anagraficaService: AnagraficaService,
    private route: ActivatedRoute,
    private permessiUtenteService: PermessiUtenteService,
    private translocoService: TranslocoService,
    private budgetService: BudgetService,
    private giasDialogService: GiasDialogService,
    private gruppiRaccoltaService: GruppiRaccoltaService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private businessLogic: AnagraficaBusinessLogicService,
    private ContattiClientService: ContattiService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impresa, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impresa, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impresa, 0);
    this.permessoRicercaDocumenti = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Documentale_Lista);
    this.permessoNuovoDocumento = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Documentale_Inser);
    this.handleCustomizations();
    this.handleEdits();

    this.objParametriAgenda = this.budgetService.getBudget() ? this.budgetService.getParametriObjService().getObjParamValue() : this.objParametriAgendaService.getObjParamValue();

    this.pagination.gridState.sort = [
      { field: 'Selected', dir: 'desc' },
      { field: 'Rag_Soc', dir: 'asc' }
    ];
  }

  read(): Observable<ImpresaKendoServerResult> {
    let data = this.anagraficaService.filterData.getValue();
    let DataFiltro = AGRODATAINIZIO;
    if (data.filter) {
      DataFiltro = data.data;
    }
    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

    return this.impreseService.leggiImprese(this.impreseService.getCaricaTutteImprese() ? "" : this.objParametriAgenda.Piva, DataFiltro)
      .pipe(
        catchError(() => {
          this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
          return of();
        }),
        tap(() => this.handleDropdowns()),
        map(gridData => gridData.kendo_rows.map(row => this.fixRowData(row))),
        tap((rows) => {
          const impresaSelezionata: any = rows.find((el: any) => el.Piva == this.objParametriAgenda.Piva);
          if (impresaSelezionata != undefined) {
            impresaSelezionata.Selected = true;
          }
        }),
        map((rows) => new ImpresaKendoServerResult(this.kendoModel, this.kendoColumns, rows)),
        tap(() => this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef }))
      );
  };

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      if (el.Attivo === 0) {
        this.renderer.addClass(visibleRows[index], 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(visibleRows[index], 'nonAttivo');
      }
    });
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string = this.translocoService.translate('anagrafica.Delete_Msg');
    opts.data.forEach(r => {
      msg = msg.concat('\n' + r['Rag_Soc']);
    });
    return msg;
  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    const item = items as any;
    let impresaBase: Impresa = new Impresa();
    let objParams = new ObjParametriAgenda();
    objParams.Piva = item.Piva;
    switch (actionType) {
      case HttpAction.CREATE:
        impresaBase = this.impreseEditService.GetFormGroupImpresa().getRawValue();
        impresaBase.partitaIva = item.Piva;
        impresaBase.partitaIvaReale = item.partitaIvaReale;

        let risorsaUmanaBase = new RisorseUmane();
        let risorseUmane = [risorsaUmanaBase];
        impresaBase.contattoAzienda.risorseUmane = risorseUmane.map(ris_um => {
          let contatto = ris_um.contatto;
          let dataNascita = contatto?.data_Nascita;
          return ({
            ...ris_um,
            contatto: !!contatto
              ? {...contatto, data_Nascita: dataNascita ? new Date(dataNascita) : null}
              : undefined
          }) as any;
        });
        impresaBase.indirizzi[0].indirizzo.codice = 0;
        this.fillImpresaBase(impresaBase, item, actionType);

        let impresaPadre: ImpresaPadre = new ImpresaPadre();
        impresaPadre.partitaIva = item.Piva_Padre;
        impresaPadre.ragioneSociale = item.Rag_Soc_Padre;
        impresaPadre.codice_iscrizione_libro_soci = item.codice_iscrizione_libro_soci;
        impresaPadre.data_iscrizione_libro_soci = item.data_iscrizione_libro_soci;
        impresaBase.impresaPadre = [impresaPadre];

        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        return this.impreseService.ScriviImpresa(impresaBase, false).pipe(
          switchMap((r) => {
            if (r.RispostaOK) {
              this.giasMessageService.successMessage(this.translocoService.translate('Impresa') + ': ' + impresaBase.ragioneSociale + ' ' + this.translocoService.translate('CreataCorrettamente'));
              let objP = this.objParametriAgendaService.getObjParamValue();
              objP.Piva = r.RispostaStringa.partitaIva;
              objP.RagSoc = r.RispostaStringa.ragioneSociale;
              this.objParametriAgendaService.changeObjParametriAgenda(objP);
              this.gridPublicService.refresh();
              this.gridPublicService.refresh();
            }
            return of(1);
          }));
      case HttpAction.REMOVE:
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

        return this.impreseService.leggiImpresaObservable(objParams).pipe(
          switchMap((r) => {
            impresaBase = r.RispostaStringa;
            impresaBase.flag_cancellazione = true;
            return this.impreseService.ScriviImpresa(impresaBase, false);
          }),
          tap((r) => {
            if (r.RispostaOK) {
              this.deleteMessageService.impreseDeleteMsg_Succ(r);
              objParams = this.objParametriAgendaService.getObjParamValue();
              objParams.Piva = "";
              objParams.RagSoc = "";
              this.objParametriAgendaService.changeObjParametriAgenda(objParams);
              this.gridPublicService.refresh();
            } else {
              this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
              this.deleteMessageService.impreseDeleteMsg_Fail(r);
            }
          })
        );
      case HttpAction.UPDATE:
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        return from(this.impreseService.leggiImpresa(objParams)).pipe(
          switchMap((r) => {
            impresaBase = r.RispostaStringa;
            this.fillImpresaBase(impresaBase, item, actionType);
            if (this.originalEditedLine.Piva_Padre != item.Piva_Padre) {
              if (item.Piva_Padre == '') {
                if (impresaBase.impresaPadre.length > 1) {
                  const index = impresaBase.impresaPadre.findIndex((el: Impresa) => el.partitaIva == this.originalEditedLine.Piva_Padre);
                  if (index > -1) {
                    impresaBase.impresaPadre.splice(index, 1);
                  }
                } else {
                  this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
                  this.giasMessageService.errorMessage(this.translocoService.translate('ErroreSelezionaCooperativa'));
                }
              } else {
                const index = impresaBase.impresaPadre.findIndex((el: Impresa) => el.partitaIva == this.originalEditedLine.Piva_Padre);

                if (index > -1) {
                  impresaBase.impresaPadre.splice(index, 1);
                }
                let impresaPadre = new ImpresaPadre();
                impresaPadre.partitaIva = item.Piva_Padre;
                impresaPadre.ragioneSociale = item.Rag_Soc_Padre;
                impresaPadre.codice_iscrizione_libro_soci = item.codice_iscrizione_libro_soci == this.originalEditedLine.codice_iscrizione_libro_soci ? "" : item.codice_iscrizione_libro_soci;
                impresaPadre.data_iscrizione_libro_soci = item.data_iscrizione_libro_soci == this.originalEditedLine.data_iscrizione_libro_soci ? AGRODATAINIZIO : item.data_iscrizione_libro_soci;
                impresaBase.impresaPadre.push(impresaPadre);
              }
            }

            return this.impreseService.ScriviImpresa(impresaBase, false).pipe(
              tap((resp) => {
                if (resp.RispostaOK) {
                  this.giasMessageService.successMessage(this.translocoService.translate('Impresa') + ': ' + impresaBase.ragioneSociale + ' ' + this.translocoService.translate('ModificataCorrettamente'));
                  let objP = this.objParametriAgendaService.getObjParamValue();
                  objP.Piva = resp.RispostaStringa.partitaIva;
                  objP.RagSoc = resp.RispostaStringa.ragioneSociale;
                  this.objParametriAgendaService.changeObjParametriAgenda(objP);
                  this.gridPublicService.refresh();
                } else {
                  this.giasMessageService.errorMessage(this.translocoService.translate('ErroreSalvataggio'));
                }
              }));
          }));
    }
    return this.read() as any;
  }

  getCodiceAnagrafeSocio(codice_socio): CodiciAnagrafeValori {
    return {
      codiceAnagrafe: {
        codice: enum_CodiciAnagrafe.Codice_Socio,
        descrizione: 'codice socio',
        lunghezza: 0,
        picture: '',
        tipo: '',
        gruppo: '',
        genitore: 0,
        creatore: '',
        validita: new IntervalloTemporale(),
        flag_cancellazione: false
      },
      valore: codice_socio,
      validita: new IntervalloTemporale()
    };
  }

  onTemplateBtnClick(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.Piva = dataItem.Piva;
    this.objParametriAgenda.RagSoc = dataItem.Rag_Soc;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['Impresa-Edit'], { relativeTo: this.route });
  }

  handleDropdowns(): void {
    let col: KendoGridColumn;

    handleDdlConfig(this.kendoColumns, 'Pro_Cod_Istat', 'Provincia_Des', 'Istat_Prov', 'Provincia', this.caricaProvincie.bind(this));
    handleDdlConfig(this.kendoColumns, 'Com_Cod_Istat', 'descrizione', 'codice', 'Comune', this.caricaComuni.bind(this));
    handleDdlConfig(this.kendoColumns, 'Stato_Cod', 'descrizione', 'codice', 'Stato', this.caricaStati.bind(this));
    handleDdlConfig(this.kendoColumns, 'Piva_Padre', 'ragioneSociale', 'partitaIva', 'Rag_Soc_Padre', this.caricaPadri.bind(this));
    handleDdlConfig(this.kendoColumns, 'GruppoRaccolta_Cod', 'descrizione', 'codice', 'GruppoRaccolta_Des', this.caricaGruppiRaccolta.bind(this));
    handleDdlConfig(this.kendoColumns, 'Rag_Soc_Proprietario', 'descrizione', 'codice', 'Rag_Soc_ProprietarioDes', this.caricaAziendeProprietarie.bind(this));
    handleDdlConfig(this.kendoColumns, 'Rapporti_Codice', 'descrizione', 'codice', 'Rapporti_Des', this.caricaRapportiContabili.bind(this));

    // Tipo
    col = this.kendoColumns.find(s => s.field === 'Tipo');
    let dataType = [
      new DropdownListItem(-1, this.translocoService.translate('Pubblico')),
      new DropdownListItem(0, this.translocoService.translate('Privato')),
    ];
    col.ddl = new DropdownListWithForm('id', 'Tipo', 'name', dataType);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'TipoDes';
    col.ddl.loadOnEdit = false;
  }

  caricaComuni(dataItem: any) {
    return from(this.istatService.leggiComuni(dataItem.Pro_Cod_Istat));
  }

  caricaProvincie(dataItem: any) {
    return from(this.istatService.leggiProvincie(dataItem.Stato_Cod));
  }

  caricaStati() {
    return from(this.istatService.leggiStati());
  }

  caricaPadri(dataItem: any) {
    return from(this.impreseService.leggiPadri()).pipe(
      map(vals => {
        vals = vals.filter(el => el.partitaIva !== this.gridPublicService.formGroup.getValue().controls['Piva'].value);

        vals = vals.map((el) => {
          if (el.CUAA != null && el.CUAA != '') {
            el.ragioneSociale = el.ragioneSociale + ' (' + el.CUAA + ')';
          }
          return el;
        });

        let impresaDefault = new Impresa('');
        impresaDefault.ragioneSociale = '';

        let index = vals.findIndex((el) => el.partitaIva == '');
        if (index < 0) {
          vals.splice(0, 0, impresaDefault);
        }

        return vals;
      })
    );
  }

  caricaGruppiRaccolta() {
    let objPAgenda = this.objParametriAgendaService.getObjParamValue();
    return from(this.gruppiRaccoltaService.leggiGruppiRaccolta(objPAgenda)).pipe(tap((vals) => {

      let gruppoRaccoltaDefault = new GruppoRaccolta(0);
      gruppoRaccoltaDefault.descrizione = '';
      vals.splice(0, 0, gruppoRaccoltaDefault);
    }));
  }

  caricaAziendeProprietarie() {
    return this.ContattiClientService.LeggiImpostazioniProprietaContatti().pipe(
      map((result: ImpostazioneSemplice[]) => {
        const data = result[0].data.map(d => ({ codice: d.codice, descrizione: d.descrizione }));
        let propContattoDefault = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_ProprietaContatti);
        this.defaultParentCompany = data.find(d => d.codice == propContattoDefault.Valore) || null;
        return data;
      })
    );
  }

  caricaRapportiContabili(): Observable<Partial<RapportoContabile>[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<any, RapportoContabile[]>(
      'MetaschemaNG/GetRapportiContabCodDescr',
      enum_RapportiContabili_SaCod.PersoneGiuridiche
    ).pipe(
      map((risposta: any) => {
        if (risposta.RispostaOK) {
          const rapporti = risposta.RispostaStringa || [];
          // Ensure each rapporto has the correct structure for dropdown
          return rapporti.map(r => ({
            codice: r.codice,
            descrizione: r.descrizione,
            id: r.codice,
            name: r.descrizione
          }));
        } else {
          return [];
        }
      }),
      catchError(() => of([]))
    );
  }

  handleCustomizations(): void {
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = this.permessoEdit;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.permessoEdit;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: this.permessoEdit,
      infoBtn: false,
      removeBtn: this.permessoRemove,
    });
    this.setupDdlCommandsMenu();
    this.handleCommandEvent();

    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.selectable.selectable.enabled = true;
    this.groups.groupable.enabled = false;
    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
      this.cmdColumn.editBtn = false;
    }
  }

  private setupDdlCommandsMenu() {
    this.cmdDropDown = new CommandsDropDownSettings({
      inlineEditBtn: false,
      fullEditBtn: this.permessoEdit,
      infoBtn: this.permessoInfo,
      removeBtn: false,
    });

    if (this.permessoRicercaDocumenti
      && this.cmdDropDown.cmdList.findIndex(v => v.action === this.businessLogic.commands.RICERCA_DOCUMENTI) === -1
    ) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "RicercaDocumenti",
        this.businessLogic.commands.RICERCA_DOCUMENTI,
        'faAnagraficaSearchDocument'
      ));
      if (this.permessoNuovoDocumento
        && this.cmdDropDown.cmdList.findIndex(v => v.action === this.businessLogic.commands.NUOVO_ALLEGATO) === -1
      ) {
        this.cmdDropDown.addCommand(new GridCommandItem(
          "AggiungiNuovoAllegato",
          this.businessLogic.commands.NUOVO_ALLEGATO,
          'faAnagraficaUploadFile'
        ));

        this.gridPublicService.openCommands
          .pipe(takeUntil(this.signal), filter(macchina => !!macchina))
          .subscribe(impresa => {
            if (!this.permessoNuovoDocumento) {
              this.cmdDropDown.removeCommand(this.businessLogic.commands.NUOVO_ALLEGATO);
            }

            if (!this.permessoRicercaDocumenti) {
              this.cmdDropDown.removeCommand(this.businessLogic.commands.RICERCA_DOCUMENTI);
            } else {
              this.businessLogic.CheckAttachedDocumentsImprese(impresa.Piva)
                .pipe(filter(hasAttachments => !hasAttachments))
                .subscribe(() => this.cmdDropDown.removeCommand(this.businessLogic.commands.RICERCA_DOCUMENTI));
            }
          });
      }
    }
  }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent
      .pipe(takeUntil(this.signal), filter(ev => !!ev))
      .subscribe(ev => {
        switch (ev.command.action) {
          case CommandsDropDownEvents.FULL_EDIT:
            this.onTemplateBtnClick(ev.dataItem);
            break;
          case this.businessLogic.commands.NUOVO_ALLEGATO:
            this.businessLogic.apriKWindowImprese(ev.dataItem, this.businessLogic.commands.NUOVO_ALLEGATO);
            break;
          case this.businessLogic.commands.RICERCA_DOCUMENTI:
            this.businessLogic.apriKWindowImprese(ev.dataItem, this.businessLogic.commands.RICERCA_DOCUMENTI);
            break;
        }
      });
  }

  private handleEdits() {
    this.handleInlineEdits();
    this.onChange();
  }

  private onChange() {
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((event: any) => {
        if (event?.action === 'edit') {
          this.handleOnChangeEdit();
        }
        if (event?.action === 'add') {
          this.handleOnChangeAdd();
        }
      });
  }

  private handleOnChangeEdit() {
    let fb = this.gridPublicService.formGroup.getValue();
    if (fb != undefined) {
      fb.controls["Piva"].disable();
      fb.controls["Tipo"].disable();
      fb.controls["Rag_Soc_Proprietario"].disable();
      fb.controls["Rapporti_Codice"].disable();
      this.originalEditedLine = fb.getRawValue();
    }
  }

  private handleOnChangeAdd() {
    let fb = this.gridPublicService.formGroup.getValue();
    if (fb != undefined) {
      fb.controls['Piva'].disable();

      fb.controls['Stato_Cod'].setValue('IT');
      fb.controls['Pro_Cod_Istat'].setValue('000');
      fb.controls['Com_Cod_Istat'].setValue('000');

      let contattoPubblicoVisibleDefault = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_ContattiPubblici);

      if (contattoPubblicoVisibleDefault.Valore == "1") {
        fb.controls['Tipo'].setValue(-1);
        fb.controls['TipoDes'].setValue(this.translocoService.translate('Pubblico'));
      } else {
        fb.controls['Tipo'].setValue(0);
        fb.controls['TipoDes'].setValue(this.translocoService.translate('Privato'));
      }

      let col_tipo = this.kendoColumns.find(s => s.field === 'Tipo');
      col_tipo.ddl.reload.next(true);

      this.caricaAziendeProprietarie().pipe(
        take(1),
        tap((aziendeProprietarie) => {
          let aziendaProprietaria = this.kendoColumns.find(s => s.field === 'Rag_Soc_Proprietario');
          const dropdownData = aziendeProprietarie.map(item =>
            new DropdownListItem(item.codice, item.descrizione)
          );
          aziendaProprietaria.ddl.data = dropdownData;
          if (this.defaultParentCompany) {
            fb.controls['Rag_Soc_Proprietario'].setValue(this.defaultParentCompany.codice);
            fb.controls['Rag_Soc_ProprietarioDes'].setValue(this.defaultParentCompany.descrizione);
          }
          aziendaProprietaria.ddl.reload.next(true);
        })
      ).subscribe();

      this.caricaRapportiContabili().pipe(
        take(1),
        tap((rapportiContabili) => {
          let col_rapportoContabile = this.kendoColumns.find(s => s.field === 'Rapporti_Codice');
          col_rapportoContabile.ddl.data = rapportiContabili.map(item =>
            new DropdownListItem(item.codice, item.descrizione)
          );

          let rapportoContabileDefault = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_DefaultRappContabile);
          const defaultRapporto = rapportiContabili.find(r => r.codice == Number(rapportoContabileDefault.Valore)) || rapportiContabili[0];

          if (defaultRapporto) {
            const defaultDropdownItem = new DropdownListItem(defaultRapporto.codice, defaultRapporto.descrizione);
            fb.controls['Rapporti_Codice'].setValue([defaultDropdownItem]);
            fb.controls['Rapporti_Des'].setValue(defaultRapporto.descrizione);
          }

          fb.controls['Rapporti_Codice'].valueChanges.pipe(
            takeUntil(this.signal)
          ).subscribe((value) => {

            if (!value || (Array.isArray(value) && value.length === 0)) {

              if (defaultRapporto) {
                const defaultDropdownItem = new DropdownListItem(defaultRapporto.codice, defaultRapporto.descrizione);

                setTimeout(() => {
                  fb.controls['Rapporti_Codice'].setValue([defaultDropdownItem], { emitEvent: false });
                  fb.controls['Rapporti_Des'].setValue(defaultRapporto.descrizione);
                }, 0);
              }
            } else if (Array.isArray(value) && value.length > 0) {
              const descriptions = value.map(item => item.name || item.descrizione || '').join(', ');
              fb.controls['Rapporti_Des'].setValue(descriptions);
            }
          });

          col_rapportoContabile.ddl.reload.next(true);
        })
      ).subscribe();

      let col_stato = this.kendoColumns.find(s => s.field === 'Stato_Cod');
      col_stato.ddl.reload.next(true);
      let col_prov = this.kendoColumns.find(s => s.field === 'Pro_Cod_Istat');
      col_prov.ddl.reload.next(true);
      let col_com = this.kendoColumns.find(s => s.field === 'Com_Cod_Istat');
      col_com.ddl.reload.next(true);

      this.caricaPadri(fb.getRawValue()).pipe(
        take(1),
        tap((padri) => {
          let col_padri = this.kendoColumns.find(s => s.field === 'Piva_Padre');
          col_padri.ddl.reload.next(true);

          const defaultFatherCompany = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.ImpresaPadreDefault);
          const impresaSuperUser = padri.find(el => el.partitaIva == this.masterService.objP_server.PivaSuperUser);

          if (defaultFatherCompany?.Valore != undefined && defaultFatherCompany.Valore != '') {
            fb.controls['Piva_Padre'].setValue(defaultFatherCompany.Valore);
            fb.controls['Rag_Soc_Padre'].setValue(padri.find(el => el.partitaIva === defaultFatherCompany.Valore));
          } else if (impresaSuperUser) {
            fb.controls['Piva_Padre'].setValue(impresaSuperUser.partitaIva);
            fb.controls['Rag_Soc_Padre'].setValue(impresaSuperUser.ragioneSociale);
          }
        })
      ).subscribe();
      this.originalEditedLine = null;
    }
  }

  private handleInlineEdits() {
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter((fb) => fb != undefined)
    ).subscribe((fb) => {
      fb.controls['Piva'].addAsyncValidators(this.pivaValidatorService.validate.bind(this));
      fb.controls['Piva'].updateValueAndValidity();
      this.listenToProvinceChange(fb);
      this.listenToStateChange(fb);
      this.listenToComChange(fb);
    });
  }

  private listenToProvinceChange(fb: FormGroup) {
    fb.controls["Pro_Cod_Istat"].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => {
      let prov = fb.controls["Pro_Cod_Istat"].value;
      let stato = fb.controls["Stato_Cod"].value;
      if (prov != null && prov != "") {
        if (prov != '000') {
          fb.controls["Com_Cod_Istat"].enable();
        }

        this.istatService.leggiComuni(prov).then((comuni) => {
          this.istatService.leggiProvincie(stato).then((provincie) => {
            let provincia = provincie.find(p => p.Istat_Prov === prov);
            let com = comuni.find(s => s.codice === provincia.comuneDefault);
            let col = this.kendoColumns.find(s => s.field === 'Com_Cod_Istat');
            fb.controls["Com_Cod_Istat"].setValue(com.codice);
            col.ddl.reload.next(true);
          });
        });
      }
    });
  }

  private listenToStateChange(fb: FormGroup) {
    fb.controls["Stato_Cod"].valueChanges.pipe(takeUntil(this.signal)).subscribe((value) => {
      from(this.istatService.leggiStati()).pipe(take(1), map(resp => {
        let gestioneGerarchia = resp.filter(t => t.codice == value)[0].gestioneGerarchia;
        let stato = fb.controls["Stato_Cod"].value;

        if (gestioneGerarchia != 1 && stato != '') {
          fb.controls["Com_Cod_Istat"].disable();
          fb.controls["Pro_Cod_Istat"].disable({ emitEvent: false });
          fb.controls["Pro_Cod_Istat"].setValue('000', { emitEvent: false });
          fb.controls["Com_Cod_Istat"].setValue('000', { emitEvent: false });
          fb.controls['Cap'].setValue('00000');
        } else {
          fb.controls["Com_Cod_Istat"].enable();
          fb.controls["Pro_Cod_Istat"].enable({ emitEvent: false });
          fb.controls["Pro_Cod_Istat"].setValue(value == 'IT' ? '000' : value + '000');
          fb.controls["Com_Cod_Istat"].setValue(value == 'IT' ? '000' : value + '000', { emitEvent: false });
          fb.controls['Cap'].setValue('00000');
          let col = this.kendoColumns.find(s => s.field === 'Pro_Cod_Istat');
          col.ddl.reload.next(true);
        }
      })).subscribe();
    });
  }

  private listenToComChange(fb: FormGroup) {
    fb.controls["Com_Cod_Istat"].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => {
      let com = fb.controls["Com_Cod_Istat"].value;
      let prov = fb.controls["Pro_Cod_Istat"].value;
      let stato = fb.controls["Stato_Cod"].value;
      if (com != '' && com != '000' && prov != '' && prov != '000' && (stato == 'IT' || stato == '')) {
        this.istatService.leggiCAP(fb.controls['Pro_Cod_Istat'].value, fb.controls['Com_Cod_Istat'].value,)
          .then((cap) => fb.controls['Cap'].setValue(cap));
      } else {
        fb.controls['Cap'].setValue('00000');
      }
    });
  }

  private fixRowData(row) {
    row.Selected = false;
    row.IndirizzoCompleto = row.Stato_Cod == 'IT'
      ? row.ind_des + ' - ' + row.Com + ' (' + row.Prov + ')' + ' CAP:' + row.Cap
      : row.ind_des + ' (' + row.frz_des + ') ZIP: ' + row.Cap;

    if (row.Rag_Soc_Proprietario && !row.Rag_Soc_ProprietarioDes) {
      row.Rag_Soc_ProprietarioDes = row.Rag_Soc_Proprietario;
    }
    // Handle Rapporti_Codice and Rapporti_Des mapping
    row.Rapporti_Codice = row.Rapporti_Codice?.length > 0 ? row.Rapporti_Codice.split(', ') : [];
    row.Rapporti_Des = row.Rapporti_Des?.length > 0 ? row.Rapporti_Des.split(', ') : [];
    return row;
  }

  private fillImpresaBase(impresaBase: Impresa, item: any, action?: HttpAction) {
    impresaBase.ragioneSociale = item.Rag_Soc;
    impresaBase.CUAA = item.Codice_Cuaa;
    impresaBase.partitaIva = item.Piva;
    impresaBase.partitaIvaReale = item.partitaIvaReale;

    impresaBase.contattoAzienda = {};
    impresaBase.contattoAzienda.aziendaCorrentePIVA = this.objParametriAgenda.Piva;
    impresaBase.contattoAzienda.contattoPubblico = item.Tipo === -1;
    impresaBase.contattoAzienda.proprietarioContattoAzienda = action === HttpAction.CREATE ? item.Rag_Soc_Proprietario : 0;

    impresaBase.validita = new IntervalloTemporale(item.Validita_Inizio, item.Validita_Fine);

    impresaBase.indirizzi[0].indirizzo.frazione = item.frz_des;
    impresaBase.indirizzi[0].indirizzo.via = item.ind_des;
    impresaBase.indirizzi[0].indirizzo.istatComune.prov = item.Pro_Cod_Istat ?? '000';
    impresaBase.indirizzi[0].indirizzo.istatComune.com = item.Com_Cod_Istat ?? '000';
    impresaBase.indirizzi[0].indirizzo.cap = item.Cap;
    impresaBase.indirizzi[0].indirizzo.stato.codice = item.Stato_Cod;

    impresaBase.contattoAzienda.risorseUmane = [];

    if (!!item.Rapporti_Codice) {
      from([...item.Rapporti_Codice]).subscribe(r => {
        let humanResource: RisorseUmane = new RisorseUmane();

        if (typeof r == 'string') {
          humanResource.rapportoContabile = new RapportoContabile(Number.parseInt(r));
        } else if (typeof r == 'object') {
          humanResource.rapportoContabile = new RapportoContabile(Number.parseInt(r.id), r.name);
        }

        impresaBase.contattoAzienda.risorseUmane.push(humanResource);
      });
    } else {
      const defaultRapporto: Utente_Impostazioni = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_DefaultRappContabile);
      const rapportoContabile: RapportoContabile = new RapportoContabile(Number(defaultRapporto.Valore), "");

      let humanResource: RisorseUmane = new RisorseUmane();
      humanResource.rapportoContabile = rapportoContabile;

      impresaBase.contattoAzienda.risorseUmane.push(humanResource);
    }

    let padreInteressato = impresaBase.impresaPadre.find(imp => imp.partitaIva == item.Piva_Padre);

    if (padreInteressato) {
      padreInteressato.codice_iscrizione_libro_soci = item.codice_iscrizione_libro_soci;
      padreInteressato.data_iscrizione_libro_soci = item.data_iscrizione_libro_soci;
    }

    if (item.GruppoRaccolta_Cod) {
      impresaBase.gruppoRaccolta = { codice: item.GruppoRaccolta_Cod, descrizione: '' };
    } else {
      impresaBase.gruppoRaccolta = { codice: 0, descrizione: '' };
    }

    if (item.Codice_Socio != '') {
      let findIndex = impresaBase.codici.findIndex((val) => val.codiceAnagrafe.codice == enum_CodiciAnagrafe.Codice_Socio);

      if (findIndex < 0) {
        impresaBase.codici.push(this.getCodiceAnagrafeSocio(item.Codice_Socio));
      } else {
        impresaBase.codici[findIndex] = this.getCodiceAnagrafeSocio(item.Codice_Socio);
      }
    } else {
      let findIndex = impresaBase.codici.findIndex((val) => val.codiceAnagrafe.codice == enum_CodiciAnagrafe.Codice_Socio);

      if (findIndex >= 0) {
        impresaBase.codici.splice(findIndex, 1);
      }
    }
  }
}
