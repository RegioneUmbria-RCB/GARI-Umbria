import { ElementRef, Injectable, Injector, Renderer2 } from '@angular/core';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import { MenuContestualeService } from 'app/Master/menu-contestuale/menu-contestuale.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { MasterService, RispostaStandard, VariabiliInSessione_NG } from 'app/Service/master.service';
import { isNullOrUndefined } from 'app/Service/utils';
import {
  AgrSelectableSettings,
  CommandsColumnSettings,
  CommandsDropDownSettings,
  DeletionMode,
  DettagliColumnSettings,
  EditingMode,
  ExcelSettings,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  RendererGridEvent
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  KendoGridColumn,
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of, throwError } from 'rxjs';
import { filter, map, skip, switchMap, takeUntil, tap } from 'rxjs/operators';
import { AllOperationsTableComponent } from './all-op-table.component';
import {
  construisciChiaviComposite,
  enum_menuAgendaGridCommands,
  GridCommandItem, IChiaveCompositaPiva,
  lav_cod_copiabili,
  Link_ElimOpMultipla,
  QdCRow
} from '../utils';
import { BussinessMenuAgendaService } from '../../shared_services/bussiness-logic.service';
import { FiltersService } from '../filters/filters.service';
import { MenuAgendaDataStore } from '../../shared_services/menu-agenda-datastore.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import {enum_LAVCOD, enum_Menu_Agenda_NG_Mode, enum_Security_Attivita} from 'app/Model/TipiEnumerativi';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { CookieService } from 'ngx-cookie-service';
import { distinct } from '@progress/kendo-data-query';
import { Gias2010Redirector } from './Gias2010Redirector.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {
  ImpostazioniAziendeCentriService
} from '../../../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { ElencoLavCodRilieviSenzaImpianti } from "../../../Model/CostantiPersonalizzate";

export class elimina_operazione_multipla {
  strChiaviComposite: string
  proseguiInCasoDiAlert: boolean
  variabiliInSessione_NG: VariabiliInSessione_NG
}

@Injectable()
export class TuttiTipiConfig extends AbstractGridConfigService<KendoServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string;
  gridId: string = "QdcConfigService";
  data: KendoServerResult;
  selectable: AgrSelectableSettings = new AgrSelectableSettings();
  qdcGridRef: ElementRef;
  applicaFiltri: boolean;

  private component: AllOperationsTableComponent;

  private permessoQdC_W: boolean;
  private permessoNuovoDocumento: boolean;
  private permessoRicercaDocumenti: boolean;
  private vizDocContabInQdc: boolean; // presa da impostazioni azienda/su

  constructor(
    injector: Injector,
    private gias2010Redirector: Gias2010Redirector,
    private filters: FiltersService,
    private datastore: MenuAgendaDataStore,
    private master: MasterService,
    private renderer: Renderer2,
    private bussinessLogic: BussinessMenuAgendaService,
    private menu: MenuContestualeService,
    private dialogService: GiasDialogService,
    private agendaService: ObjParametriAgendaService,
    private impostazioiImprese: ImpostazioniAziendeCentriService,
    private permessiUtenteService: PermessiUtenteService,
    private cookieService: CookieService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.filters.subscribeToFiltersChange().pipe(takeUntil(this.signal), skip(1)).subscribe((val) => {
      this.applicaFiltri = true;
      this.gridPublicService.refresh(true, null, true);
    });

    this.permessoQdC_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);
    this.permessoNuovoDocumento = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Inser, 2);
    this.permessoRicercaDocumenti = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Documentale_Lista, 0);

    this.modalitaDemetra = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra) ?
      this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra).Valore === '1' :
      false;

    const impVal = this.impostazioiImprese.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser(
      this.agendaService.getObjParamValue().Piva,
      this.agendaService.getObjParamValue().Sa_Cod,
      enum_Impostazioni_Utenti.MENU_AGENDA_VISIBILITA_DOC_CONTABILI
    ); // '1' | '0' | ''
    this.vizDocContabInQdc = (impVal === '1');

    this.handleCustomizations();
    this.setButtonsVisibility();
    this.handleCommands();
  }

  public SetComponentRef(component: AllOperationsTableComponent) {
    this.component = component;
  }

  read(): Observable<KendoServerResult> {
    const data = this.datastore.gridDataQdc;
    if (!isNullOrUndefined(data) && !this.applicaFiltri && !this.menu.pendingWorkDone) {
      return of(data);
    }
    this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
    this.applicaFiltri = false;
    const filters = this.getFilters();
    return this.datastore.GetQdCGridData(filters).pipe(
      tap((data: any) => {
        let columns = new Array<KendoGridColumn>();
        let table = data.RispostaStringa;
        let rows: Array<KendoGridRow> = table.kendo_rows;
        let model: KendoGridModel = table.kendo_model;

        this.valorizeColumns(table, columns, model);
        this.addFilterOrdering(columns);

        if (!this.datastore.setupShowDDT)
          rows = table.kendo_rows.filter(r => r.tipo !== 'E');
        if (data.ParametroDue)
          this.datastore.lav_cod_ricettabili = JSON.parse(data.ParametroDue_stringa);

        this.datastore.gridDataQdc = { columns: columns, model: model, rows: rows };
      }),
      tap(() => this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef })),
      tap(() => this.menu.markPendingWorkAsDone()),
      map(() => this.datastore.gridDataQdc)
    );
  }


  private valorizeColumns(table, columns: KendoGridColumn[], model: KendoGridModel) {
    table.kendo_columns.forEach(elem => {
      const col: KendoGridColumn = { ...elem };
      col.editable = false;
      if (model[col.field].type === CELL_TYPES.STRING) {
        col.showHTMLAsString = true;
      }
      columns.push(col);
    });
  }

  /**
   * Adds custom filter ordering logic to specific columns in a Kendo Grid.
   *
   * This method modifies the `filterOrdering` property of the columns to ensure
   * that rows with a specific translated value ("NessunaSpecie") appear first,
   * followed by other rows sorted alphabetically by the column's field value.
   *
   * @param columns - An array of `KendoGridColumn` objects representing the grid's columns.
   *
   * The following columns are processed:
   * - `Specie`: Rows are ordered with "NessunaSpecie" first, followed by others sorted alphabetically.
   * - `cul_Des`: Rows are ordered with "NessunaSpecie" first, followed by others sorted alphabetically.
   * - `Specie_Varieta`: Rows are ordered with "NessunaSpecie" first, followed by others sorted alphabetically.
   *
   * The translation for "NessunaSpecie" is retrieved using the `transloco.translate` method.
   */
  private addFilterOrdering(columns: KendoGridColumn[]) {
    let col = columns.find(c => c.field === 'Specie');
    if (col) {
      col.filterOrdering = (rows) => {
        const first = rows.filter(r => r['Specie'] === this.transloco.translate('NessunaSpecie'));
        const others = rows.filter(r => r['Specie'] !== this.transloco.translate('NessunaSpecie'));
        others.sort((a, b) => a['Specie'] > b['Specie'] ? 1 : -1);
        return [...first, ...others];
      };
    }

    col = columns.find(c => c.field === 'cul_Des');
    if (col) {
      col.filterOrdering = (rows) => {
        const first = rows.filter(r => r['cul_Des'] === this.transloco.translate('NessunaSpecie'));
        const others = rows.filter(r => r['cul_Des'] !== this.transloco.translate('NessunaSpecie'));
        others.sort((a, b) => a['cul_Des'] > b['cul_Des'] ? 1 : -1);
        return [...first, ...others];
      };
    }

    col = columns.find(c => c.field === 'Specie_Varieta');
    if (col) {
      col.filterOrdering = (rows) => {
        const first = rows.filter(r => r['Specie_Varieta'] === this.transloco.translate('NessunaSpecie'));
        const others = rows.filter(r => r['Specie_Varieta'] !== this.transloco.translate('NessunaSpecie'));
        others.sort((a, b) => a['Specie_Varieta'] > b['Specie_Varieta'] ? 1 : -1);
        return [...first, ...others];
      };
    }
  }

  private getFilters() {
    const filters = this.filters.getQdCTable();
    let filtersService = this.filters.filtersGetValue();

    return filters;
  }

  private MostraBtn_Importa_Nel_PUA(dataitem: any): boolean {
    return this.filters.Mode === enum_Menu_Agenda_NG_Mode.PUA
      && dataitem.AggiungiAlPua
      && dataitem.AggiungiAlPua.toLowerCase() === 'true';
  }

  private MostraBtn_Aggiungi_Ricetta(dataitem: any): boolean {
    return +dataitem.Ricetta_Cod === 0
      && this.datastore.lav_cod_ricettabili.indexOf(dataitem.Lav_cod) > -1
      && !this.bussinessLogic.NascondiModificaInstallazioneReinnescoTrappole(dataitem);
  }

  modalitaDemetra: boolean = false;

  override getMessagePrefix(rows:any[]): string {
    let prefix = '';
    let alsoBrogliaccio = rows.map(row => row?.Origine && row?.Origine.toLowerCase()).some(orig => orig === 'demetra') ||
      (rows.map(row => row?.Origine && row?.Origine.toLowerCase()).some(orig => orig === 'app') && this.modalitaDemetra);

    if (alsoBrogliaccio) {
        prefix += this.transloco.translate('MenuAgendaActivityDeletionWBrogliaccio') + ' ';
    }
    return prefix;
  }

  onRowClass = (event: RowClassArgs) => {
    return this.coloraRigheOperazioni(event.dataItem);
  }

  coloraRigheOperazioni(row: KendoGridRow) {
    let result: { [k: string]: boolean } = {
      agendaOperazPianificata: row['contabilizzato'] < 0,
      agendaOperazBloccata: row['blocco_flag'] === '1',
      agendaOperazDaRevisionare: row['blocco_flag'] === '2'
    };
    return result;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let rows: [] = grid.data['data'];
    let domElems = gridElRef.nativeElement.querySelectorAll('tbody tr');

    this.nascondiPulsantiOperazioniAgenda(this.renderer, domElems, rows);
    // this.bussinessLogic.applicaStiliGestioneCosti(domElems, rows, this.qdcGridRef);
  }

  perform(actionType: HttpAction, item: any): Observable<any[]> {
    if (actionType === HttpAction.REMOVE) {
      return this.deleteItem(item, false);
    }
    return of();
  }

  private handleCommands() {
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case enum_menuAgendaGridCommands.VAI_AI_COSTI:
          this.bussinessLogic.vaiAiCosti(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.AGGIUNGI_RICETTA:
          this.bussinessLogic.preparaModalRicetta(ev.dataItem, [ev.dataItem]);
          break;
        case enum_menuAgendaGridCommands.COPIA:
          this.copiaOperazione(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.INFO:
          this.gias2010Redirector.reindirizzaAiDettagli('info', ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.MODIFICA:
          this.gias2010Redirector.reindirizzaAiDettagli('edit', ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.CANCELLA:
          break; // Già gestito con gli eventi della griglia
        case enum_menuAgendaGridCommands.RICERCA_DOCUMENTI:
          this.bussinessLogic.ApriKendoWindowRicercaDocumenti(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.NUOVO_ALLEGATO:
          this.bussinessLogic.ApriKendoWindowAggiungiNuovoAllegato(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.IMPORTA_NEL_PUA:
          this.bussinessLogic.Importa_nel_PUA(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.VAI_ALLA_VISITA:
          this.bussinessLogic.Vai_Alla_Visita(ev.dataItem);
          break;
      }
    })
  }

  private copiaOperazione(dataItem: any): void {
    if (dataItem['Raccoglitore_Cod'] !== '0') {
      dataItem['Selected'] = true;
      this.massiveCopy();
      return;
    }

    this.master.set_isLoading({ message: '', isLoading: true });

    this.bussinessLogic.copiaOperazione(dataItem, [dataItem])
      .subscribe((r) => {
        if (r.RispostaOK) {
          let copiaOpRisp = r.RispostaStringa;
          this.bussinessLogic.copiaCambiaSito(copiaOpRisp);
        } else
          this.master.changeErrorMsgType({ show: true, msg: r.Errore, errorNumber: 0 });
      });
  }

  private massiveCopy() {
    let selected = this.datastore.gridDataQdc.rows.filter(r => r['Selected']);

    if (!selected.length) return;

    let RCods = distinct(selected.map((r: QdCRow) => r['Raccoglitore_Cod']))
      .filter(cod => cod !== '0');
    let allFromRaccoglitore = this.datastore.gridDataQdc.rows.filter((row: QdCRow) =>
      RCods.includes(row['Raccoglitore_Cod'])
    );

    allFromRaccoglitore.forEach((row: QdCRow) => {
      if (!selected.includes(row)) {
        row['Selected'] = true;
        selected.push(row);
      }
    });

    if (selected.some(dataItem => {
      return lav_cod_copiabili.indexOf(parseInt(dataItem['Lav_cod'])) < 0
        || dataItem['blocco_flag'] !== '0';
    })) {
      this.dialogService.baseError('Error', 'OperazioniNonCopiabili')
      return;
    }

    this.master.set_isLoading({ message: '', isLoading: true });

    this.bussinessLogic.copiaOperazione(selected[0], selected).subscribe((r) => {
      if (r.RispostaOK) {
        let copiaOpRisp = r.RispostaStringa;
        this.bussinessLogic.copiaCambiaSito(copiaOpRisp);
      } else
        this.master.changeErrorMsgType({ show: true, msg: r.Errore, errorNumber: 0 });
    });
  }

  /**
   * @param rows le righe da cancellare
   * @param proseguiInCasoDiAlert Richiede all'utente la conferma prima di cancellare
   * operazioni che hanno altri vincoli.
   */
  private deleteItem(rowsIn: QdCRow[] | QdCRow, proseguiInCasoDiAlert: boolean) {
    let rows: QdCRow[] = [];
    if (!rowsIn['length']) {
      let row = rowsIn as QdCRow;
      rows.push(row);
    } else {
      rows = rowsIn as QdCRow[];
    }

    let chiavi = construisciChiaviComposite(rows)
    let params: elimina_operazione_multipla = {
      strChiaviComposite: "del_elem|" + chiavi,
      proseguiInCasoDiAlert: proseguiInCasoDiAlert,
      variabiliInSessione_NG: this.master.variabiliInSessione
    }

    if (operationIsBlocked(rows)) {
      this.dialogService.baseError('Error', 'OpBloccata');
      return throwError(() => new Error());
    }

    return this.ajaxAgronicaAPIService.ajaxAPIPost<elimina_operazione_multipla, string>(Link_ElimOpMultipla, params, true)
      .pipe(
        switchMap((risposta: RispostaStandard) => {
          this.loadingService.set_isLoading({
            isLoading: false,
            message: '',
            component: this.gridPublicService.gridElRef
          });

          if (risposta.RispostaOK) {
            this.dialogService.baseSuccess('', risposta.RispostaStringa, false);
            this.datastore.resetGridData();
            this.gridPublicService.refresh(true);
          } else if (risposta.RispostaConferma) {
            return this.dialogService.baseWarning('', risposta.Errore, false);
          } else {
            const errMsg = risposta.Errore ? risposta.Errore : this.transloco.translate("ImpossibileEliminareOperazione");
            this.dialogService.baseError('', errMsg, false);
          }

          return [];
        }), switchMap((result) => {
          if (result['returnObj'])
            return this.deleteItem(rows, true);
          return of([]);
        }));
  }

  nascondiPulsantiOperazioniAgenda(renderer: any, domElems: any[], elems: any[]) {
    // btnCreaRicetta btnCopia btnModifica btnCancella lblRicetta

    elems.forEach((dataItem: any, index: number) => {
      let currDomRow = domElems[index];
      const hideEditBtn = (hide: boolean = true) => {
        let btn = currDomRow.getElementsByClassName("divDettagliColumn");
        if (btn && btn.length === 1) {
          renderer.setStyle(btn[0], 'display', hide ? 'none' : 'block');
        }
      }

      hideEditBtn(!this.permessoQdC_W);
      UtilityFunctions.setStyle(renderer, currDomRow, '.lblRicetta', 'display', 'block');

      if (this.bussinessLogic.SenzaPermessoDiModifica(dataItem) ||
        this.bussinessLogic.OperazioneBloccata(dataItem) ||
        this.bussinessLogic.lavcodNonEditabile(dataItem) ||
        this.bussinessLogic.NascondiModificaInstallazioneReinnescoTrappole(dataItem)) {
        hideEditBtn();
      } else {
        hideEditBtn(false);
      }

      if (this.filters.Mode === enum_Menu_Agenda_NG_Mode.PUA)
        hideEditBtn(true);

      //se c'è già una ricetta
      if (dataItem.Ricetta_Cod !== "0") {
        // UtilityFunctions.setStyle(renderer, currDomRow, '.btnCreaRicetta', 'display', 'none');
        UtilityFunctions.setStyle(renderer, currDomRow, '.lblRicetta', 'display', 'block');
      }

      //Se il tipo di operazione non è tra le ricettabili, nascondo il creaRicetta
      if (this.datastore.lav_cod_ricettabili.indexOf(dataItem.Lav_cod) < 0) {
        // UtilityFunctions.setStyle(renderer, currDomRow, '.btnCreaRicetta', 'display', 'none');
        UtilityFunctions.setStyle(renderer, currDomRow, '.lblRicetta', 'display', 'none');
      } else {
        UtilityFunctions.setStyle(renderer, currDomRow, '.lblRicetta', 'display', 'block');
      }
    });
  }

  /** Imposta la visibilità dei pulsanti nel menù a dropdown una volta che esso è stato aperto.
   * Essenzialmente usata per aggiungere/rimuovere dinamicamente pulsanti da esso.
   */
  private setButtonsVisibility() {
    this.gridPublicService.openCommands.pipe(takeUntil(this.signal), filter(attivita => !!attivita))
      .subscribe(async attivita => {
        //Il tasto di modifica ora è in una colonna dedicata
        //this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.MODIFICA);
        this.setCustom_cmdDropDown();

        let impostazione = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI);

        // Se è una operazione di campagna, se è attiva l'impostazione SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI
        // se l'utente hai il permesso per la pagina dei CDG allora mostro il bottone vai ai costi e se l'operazione non è un Rilievo senza Impiani
        if (impostazione && impostazione.Valore !== ""
          && +impostazione.Valore === 3 && this.permessiUtenteService.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_Da_QdC_CdG, 2)
          && attivita.tipo !== 'E'
          && !this.RilievoSenzaImpianti(attivita)
        ) {
          let css = await this.bussinessLogic.applicaStilePulsanteCosti(attivita);
          let iconClass = 'faQdCSaveGoCost ' + css;
          this.cmdDropDown.addCommandAt(new GridCommandItem(
            'Costi',
            enum_menuAgendaGridCommands.VAI_AI_COSTI,
            iconClass, '', false
          ), 3);
        } else {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.VAI_AI_COSTI);
        }

        // Se collegato alla mia operazione (per ora solo per i rilievi) c'è una visita mostro il pulsante
        // VAI_ALLA_VISITA altrimenti lo nascondo
        if (!attivita.Id_Agenda_Visita || attivita.Id_Agenda_Visita === 0) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.VAI_ALLA_VISITA);
        } else if (attivita.Id_Agenda_Visita > 0
          && this.cmdDropDown.cmdList.findIndex(v => v.action === enum_menuAgendaGridCommands.VAI_ALLA_VISITA) === -1
        ) {
          this.cmdDropDown.addCommand(new GridCommandItem(
            "VaiAllaVisita",
            enum_menuAgendaGridCommands.VAI_ALLA_VISITA,
            'faEditFull', undefined, false
          ));
        }

        if (this.permessoRicercaDocumenti) {
          this.bussinessLogic.CheckAttachedDocumentsOperationsAndVisits(attivita.Piva, +attivita.id_agenda)
            .pipe(filter(hasAttachments => !hasAttachments))
            .subscribe(() => this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.RICERCA_DOCUMENTI));
        } else {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.RICERCA_DOCUMENTI);
        }
        if (!this.permessoNuovoDocumento) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.NUOVO_ALLEGATO);
        }

        //Mostro il bottone Importa nel PUA se ho aperto il Menu Agenda dal Piano Concimazione - PUA e il lav_Cod
        // è Distribuzione Concime o Distribuzione Ammendanti con degli impianti di specie vegetali (non di destinazione d'uso)
        if (this.MostraBtn_Importa_Nel_PUA(attivita)) {
          this.cmdDropDown.removeAllCommand();
          if (this.MostraBtn_Aggiungi_Ricetta(attivita)) {
            this.cmdDropDown.addCommand(new GridCommandItem(
              "Ricetta",
              enum_menuAgendaGridCommands.AGGIUNGI_RICETTA,
              'faQdCGoToReciepe', undefined, false
            ));
          }
          this.cmdDropDown.addCommand(new GridCommandItem(
            "Importa_Nel_PUA",
            enum_menuAgendaGridCommands.IMPORTA_NEL_PUA,
            '', undefined, false
          ));
          return;
        } else {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.IMPORTA_NEL_PUA);
        }

        // Non ho i pemessi o l'operazione è bloccata
        if (!this.permessoQdC_W || attivita.blocco_flag !== '0') {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.AGGIUNGI_RICETTA);
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA);
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.NUOVO_ALLEGATO);
          return;
        }

        // Se il tipo di operazione non è tra le copiabili
        if (lav_cod_copiabili.indexOf(parseInt(attivita.Lav_cod)) < 0) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA);
        }

        // Se fa parte di un'operazione multi e una delle attività collegate non è copiabile
        // considero tutte le attività con lo stesso raccoglitore e controllo che almeno una non sia copiabile
        if (attivita.Raccoglitore_Cod !== '0' &&
          this.datastore.gridDataQdc.rows.filter(e => e['Raccoglitore_Cod'] === attivita.Raccoglitore_Cod)
            .some(e => lav_cod_copiabili.indexOf(parseInt(e['Lav_cod'])) < 0)
        ) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA)
        }

        // Se c'è già una ricetta o l'operazione non è ricettabile
        if (!this.MostraBtn_Aggiungi_Ricetta(attivita)) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.AGGIUNGI_RICETTA);
        }

        // Se ha collegato un brogliaccio
        if (attivita.Tipo_Ricetta === 301 && attivita.Invia_APP) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
        }

        //Se ho un'operazione di Cura permetto solo la cancellazione e nascondo tutti gli altri bottoni
        if(parseInt(attivita.Lav_cod) === enum_LAVCOD.CURA){
          let CancellaCommandItem: GridCommandItem = this.cmdDropDown.cmdList.find(v => v.action === enum_menuAgendaGridCommands.CANCELLA);
          this.cmdDropDown.removeAllCommand();
          this.cmdDropDown.addCommand(CancellaCommandItem);
        }

      })
  }

  public refresh() {
    this.applicaFiltri = true;
    this.gridPublicService.refresh(true);
  }

  private handleCustomizations() {
    this.selectable.selectable.enabled = !this.datastore.isMenuAgendaCalledFromGis;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.drag = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.columnSettings.width = 35;
    this.resizable.autoFitColumns = true;
    this.views.enabled = true;
    this.generalSettings.height = "100%";

    this.behavior.showDeletionConfirmation = true;
    //this.behavior.deletionMode = DeletionMode.HandleAllRowsTogether;
    this.behavior.deletionMode = DeletionMode.HandleSingleRowDeletionOnly;
    this.behavior.excelSettings = new ExcelSettings({ enabled: !this.datastore.isMenuAgendaCalledFromGis });

    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, removeBtn: false });

    this.dettagliColumn = new DettagliColumnSettings({
      editBtn: this.permessoQdC_W && !this.datastore.isMenuAgendaCalledFromGis,
      edit: (data) => this.gias2010Redirector.reindirizzaAiDettagli('edit', data)
    });

    this.cmdDropDown = new CommandsDropDownSettings({
      removeBtn: this.permessoQdC_W && !this.datastore.isMenuAgendaCalledFromGis,
      infoBtn: !this.datastore.isMenuAgendaCalledFromGis,
      fullEditBtn: false,
      hidecmdDropDown: (row) => this.hidecmdDropDownQdCGrid(row)
    });

    //this.setCustom_cmdDropDown();
  }

  hidecmdDropDownQdCGrid(row: KendoGridRow): boolean {
    return !this.datastore.isMenuAgendaCalledFromGis
      && this.filters.Mode === enum_Menu_Agenda_NG_Mode.PUA
      && !this.MostraBtn_Importa_Nel_PUA(row)
      && !this.MostraBtn_Aggiungi_Ricetta(row);
  }

  setCustom_cmdDropDown() {
    if (this.datastore.isMenuAgendaCalledFromGis) return;

    if (this.cmdDropDown.cmdList.findIndex(v => v.action === enum_menuAgendaGridCommands.COPIA) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "Copia",
        enum_menuAgendaGridCommands.COPIA,
        'faCopySingle02'
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === enum_menuAgendaGridCommands.AGGIUNGI_RICETTA) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "Ricetta",
        enum_menuAgendaGridCommands.AGGIUNGI_RICETTA,
        'faQdCGoToReciepe',
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === enum_menuAgendaGridCommands.RICERCA_DOCUMENTI) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "RicercaDocumenti",
        enum_menuAgendaGridCommands.RICERCA_DOCUMENTI,
        'faQdCSearchDocument'
      ));
    }
    if (this.cmdDropDown.cmdList.findIndex(v => v.action === enum_menuAgendaGridCommands.NUOVO_ALLEGATO) === -1) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "AggiungiNuovoAllegato",
        enum_menuAgendaGridCommands.NUOVO_ALLEGATO,
        'faQdCUploadFile'
      ));
    }
  }

  /**
   * Controlla se l'operazione è un rilievo senza impianti
   * @param row
   */
  private RilievoSenzaImpianti(row): boolean {
    let senzaimpianti: boolean = false;

    if (row) {
      if (ElencoLavCodRilieviSenzaImpianti.findIndex(x => x === + row.Lav_cod) > -1 && +row.Veg_cod === 0) {

        if (row.PK_Impianti && row.PK_Impianti !== "") {
          let array_PK = row.PK_Impianti.split(",");

          if (array_PK && array_PK.length > 0) {

            let piva: string = array_PK[0];

            let sa_cod: number = + array_PK[1];

            let appezza: number = + array_PK[2];

            let id_destinazione: number = + array_PK[3];

            if (piva !== "" && sa_cod !== 0 && appezza === 0 && id_destinazione === 0) {
              senzaimpianti = true;
            }

          }
        }

      }
    }

    return senzaimpianti;
  }

}

export function operationIsBlocked(riga: IChiaveCompositaPiva[]) {
  if (!riga.length) return riga['chiave_composita'].split("_")[5] === '1';
  else return riga.some((row) => (row.chiave_composita ?? "").split("_")[5] === '1');
}
