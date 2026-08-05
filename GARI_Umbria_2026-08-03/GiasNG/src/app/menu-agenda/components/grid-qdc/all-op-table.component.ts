import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ContentChild,
  ElementRef,
  Inject,
  OnDestroy,
  Renderer2,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {faArrowRight, faCircleInfo, faLock, faLockOpen, faPrint} from '@fortawesome/free-solid-svg-icons';
import {MasterService, RispostaStandard} from 'app/Service/master.service';
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { GridInfoCommandEvent, GRID_HTTP_TOKEN, KendoGridRow } from 'gias-kendo-grid';
import {GridPublicService} from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import {Subject} from 'rxjs';
import {filter, skip, switchMap, take, takeUntil} from 'rxjs/operators';
import {BussinessMenuAgendaService} from '../../shared_services/bussiness-logic.service';
import {FiltersService} from '../filters/filters.service';
import {MenuAgendaDataStore} from '../../shared_services/menu-agenda-datastore.service';
import {OperationsService} from '../operations-list/operations.service';

import { lav_cod_copiabili, QdCRow, RicettaForm} from '../utils';
import { LOADING_TOKEN, LoadingService, ObjParametriAgenda } from 'gias-ui-kit';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { TuttiTipiConfig } from './qdc-config.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import {enum_LAVCOD, enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { Gias2010Redirector } from './Gias2010Redirector.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {DialogAction, DialogCloseResult, DialogResult} from '@progress/kendo-angular-dialog';
import { HttpAction } from 'gias-kendo-grid';
import {distinct} from '@progress/kendo-data-query';

@Component({
    standalone: false,
  selector: 'all-operations-table',
  templateUrl: './all-op-table.component.html',
  styleUrls: ['./all-op-table.component.scss'],
  providers: [
    ...generateGridProviders(TuttiTipiConfig, AllOperationsTableComponent),
    BussinessMenuAgendaService
  ],
  encapsulation: ViewEncapsulation.None,
})
export class AllOperationsTableComponent implements AfterViewInit, OnDestroy {
  @ViewChild(GiasKendoGridComponent) gridChild: GiasKendoGridComponent;
  @ViewChild('creaRicettaInfo') modalRicettaInfo: ElementRef;

  @ViewChild('grid') set content(content: ElementRef) {
    if (content)
      this.gridConfig.qdcGridRef = content;
  }

  ObjParametriAgenda: ObjParametriAgenda;
  signal: Subject<void> = new Subject<void>();
  openModalImportazioniApp = false;
  faArrow = faArrowRight;
  ricettaForm = CreateFormGroup(this.fb);
  messageCaricaDatiApp: string;
  destroyGrid = false;
  faPrint = faPrint;

  // Ricette
  openModalRicette: boolean;

  // Stampe
  @ContentChild('stampeRef') stampeRef: TemplateRef<any>;
  @ContentChild('gridCommands') gridCommands: TemplateRef<any>;

  // Blocco/Sblocco Attivita
  permessoBlocca: boolean = true;
  permessoSblocca: boolean = true;
  faLockOpen = faLockOpen;
  faLock = faLock;
  faInfo = faCircleInfo;

  permessoMultiModifica: boolean = false;
  openMultipleEdit: boolean = false;
  permessoMultiCancellazione: boolean = false;
  permessoEdit: boolean = true;
  permessoExportQdCToAgea: boolean = false;

  constructor(
    private gias2010Redirector: Gias2010Redirector,
    private datastore: MenuAgendaDataStore,
    private changeDetector: ChangeDetectorRef,
    private gpubService: GridPublicService,
    private agenda: ObjParametriAgendaService,
    private operations: OperationsService,
    private filters: FiltersService,
    private fb: FormBuilder,
    private bussiness: BussinessMenuAgendaService,
    private renderer: Renderer2,
    private master: MasterService,
    @Inject(GRID_HTTP_TOKEN) public gridConfig: TuttiTipiConfig,
    public gridService: GridPublicService,
    public gestioneRichieste: GestioneRichiesteService,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService,
    public intlService: IntlService,
    public dialogService: GiasDialogService,
    public transloco: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
  ) {
    this.ObjParametriAgenda = this.agenda.getObjParamValue();
    this.agenda.currentObjParametriAgenda.subscribe(objParam => this.ObjParametriAgenda = objParam);

    this.setPermissions();

    // le funzioni crea_ricetta e CreaRicetta inviano un messaggio.
    this.datastore.recipeModalData.pipe(takeUntil(this.signal), skip(1))
      .subscribe(s => {
        if (!s) {
          // Nel caso in cui il salvataggio della ricetta è andato a buon fine. Vedi: CreaRicetta().
          this.ricettaForm.patchValue(new RicettaForm());
          this.openModalRicette = false;
          this.changeDetector.detectChanges();
          return;
        }
        if (s.prevent) return;

        this.ricettaForm.patchValue(s.ricettaForm);
        this.ricettaForm.get('descrizione')?.addValidators(Validators.required);
        this.operations.drawerZIndex.next(0);

        if (s.stesso_vag_cod === true) {
          this.openModalRicette = true;
          this.changeDetector.detectChanges();

          let html = s.lblHtmlRicettaDaCreare;
          this.renderer.setProperty(this.modalRicettaInfo.nativeElement, 'innerHTML', html);
        } else {
          this.master.changeErrorMsgType({show: true, msg: s.ErroreSingolaSpecie, errorNumber: 0});
        }
        this.changeDetector.detectChanges();
      });

    this.ricettaForm.valueChanges.GiasSubscribe((ch: RicettaForm) => {
      const modelData = this.datastore.recipeModalData.getValue();
      modelData.ricettaForm.descrizione = ch.descrizione;
      modelData.ricettaForm.dataInizio = ch.dataInizio;
      modelData.ricettaForm.dataFine = ch.dataFine;
      modelData.ricettaForm.numero = ch.numero;
      modelData.ricettaForm.nota = ch.nota;
      modelData.prevent = true;
      this.datastore.recipeModalData.next(modelData);
    })

    this.gpubService.changeDetected
      .pipe(takeUntil(this.signal), filter((event: GridInfoCommandEvent) => event.action === 'edit'))
      .subscribe((event: GridInfoCommandEvent) => this.gias2010Redirector.reindirizzaAiDettagli(event.action, event.dataItem));
  }

  // Pulsanti impostazioni app qdc (operazioni) e brogliaccio
  get impostazioniApp() {
    return this.filters.impostazioniApp;
  }

  get selected() {
    return this.datastore.gridDataQdc.rows.filter(R => R['Selected']) as QdCRow[];
  }

  Info(dataItem: any) {
    this.gias2010Redirector.reindirizzaAiDettagli('info', dataItem);
  }

  ngAfterViewInit(): void {
    this.gridConfig.SetComponentRef(this);
  }

  onAggiungiRicette(ev: any, dataItem: any) {
    let righeSelezionate: KendoGridRow[] = [];
    if (!ev) { // ho premuto pulsante massivo in header
      righeSelezionate = this.gridChild.rows.filter(row => row['Selected']);
    } else {
      righeSelezionate = [dataItem];
    }
    // invia messaggio tramite this.datastore.recipeModalData
    this.bussiness.preparaModalRicetta(dataItem, righeSelezionate);
  }

  importaAgendaDaTabelleAPP() {
    if (this.impostazioniApp.SincroDatiApp) {
      this.datastore.CaricaAgendaDaTabelleAPP(this.impostazioniApp.ImportaSoloAziendaSelezionata, "20")
        .pipe(
          take(1),
          switchMap(async (risp: RispostaStandard) => await this.showDialog(risp)),
          filter(action => action?.returnObj)
        ).subscribe((action) => {
        this.datastore.azzeraDatiGrigliaQdc();
        this.gpubService.refresh(true);
      });
    } else {
      this.loadingService.set_isLoading({isLoading: true, component: this.gridService.gridElRef});
      this.datastore.ImportaAgendaDaTabelleAPP(this.impostazioniApp.ImportaSoloAziendaSelezionata)
        .pipe(takeUntil(this.signal))
        .subscribe((risp: RispostaStandard) => {
          this.operations.drawerZIndex.next(0);
          this.datastore.azzeraDatiGrigliaQdc();
          this.destroyGrid = true;
          this.changeDetector.detectChanges();
          this.destroyGrid = false;

          this.openModalImportazioniApp = true;
          this.messageCaricaDatiApp = risp.RispostaStringa;

          this.loadingService.set_isLoading({isLoading: false, component: this.gridService.gridElRef});
        });
    }
  }

  showDialog(risposta: RispostaStandard): any {
    const avvioCaricamentoMsg = this.transloco.translate('AvioCaricamento');
    if (risposta.RispostaStringa != "")
      return this.dialogService.baseWarning(
        this.transloco.translate('ImportazioneDatiApp'),
        risposta.RispostaStringa + "<br>" + avvioCaricamentoMsg,
        false
      );
    else if (risposta.Errore != "")
      return this.dialogService.baseWarning(
        this.transloco.translate('ImportazioneDatiApp'),
        risposta.Errore + "<br>" + avvioCaricamentoMsg,
        false
      );
    else
      return this.dialogService.baseWarning('ImportazioneDatiApp', 'AvioCaricamento');
  }

  closeDatiApp(modal: string) {
    this.operations.drawerZIndex.next(1);
    if (modal === 'ImportazioniDatiApp') {
      this.openModalImportazioniApp = false;
    } else if (modal === 'Ricette') {
      this.openModalRicette = false;
    }
  }

  closeRicette(action: 'back' | 'create') {
    this.operations.drawerZIndex.next(1);
    this.openModalRicette = false;
    if (action === 'create') {
      this.bussiness.CreaRicetta(this.gridChild.rows);
    }
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  copiaOperazione(dataItem: any): void {
    if (dataItem['Raccoglitore_Cod'] !== '0') {
      dataItem['Selected'] = true;
      this.massiveCopy();
      return;
    }

    this.master.set_isLoading({message: '', isLoading: true});

    this.bussiness.copiaOperazione(dataItem, [dataItem])
      .subscribe((r) => {
        if (r.RispostaOK) {
          let copiaOpRisp = r.RispostaStringa;
          this.bussiness.copiaCambiaSito(copiaOpRisp);
        } else
          this.master.changeErrorMsgType({show: true, msg: r.Errore, errorNumber: 0});
      });
  }


  public onNuovo() {
    this.bussiness.creaNuovaOperazione(Tipo_Ricetta.Standard);
  }

  public vaiAiCosti(row: QdCRow) {
    this.bussiness.vaiAiCosti(row);
  }

  public massiveRecipe(e: any) {
    let selected = this.gridChild.rows.filter(r => r['Selected']);

    if (selected.some(r =>
      this.datastore.lav_cod_ricettabili.indexOf(r['Lav_cod']) < 0
      || r['blocco_flag'] === '1'
      || this.bussiness.NascondiModificaInstallazioneReinnescoTrappole(r))
    ) {
      this.error('qdc.AttivitaNonRicettabili');
      return;
    }

    selected = this.addWithSameRaccoglitore(selected as QdCRow[]);
    selected.forEach(dataItem => this.onAggiungiRicette(null, dataItem));
  }

  // Seleziono le attività collegate alle operazioni multiple
  private addWithSameRaccoglitore(rows: QdCRow[]) {
    let multi = distinct(rows
      .filter(r => r['Raccoglitore_Cod'] !== '0')
      .map(r => r['Raccoglitore_Cod']))

    multi.forEach(rCod => this.gridChild.rows
      .filter(r => r['Raccoglitore_Cod'] === rCod)
      .forEach(r => r['Selected'] = true))

    rows = this.gridChild.rows.filter(r => r['Selected']) as QdCRow[];

    return rows;
  }

  public massiveCopy() {
    let selected = this.selected;

    if (!selected.length) {
      this.error('SelezionareAlmenoUnOperazione');
      return;
    }

    selected = this.addWithSameRaccoglitore(selected as QdCRow[]);

    if (selected.some(dataItem => {
      return lav_cod_copiabili.indexOf(parseInt(dataItem['Lav_cod'])) < 0
        || dataItem['blocco_flag'] !== '0';
    })) {
      this.error('OperazioniNonCopiabili');
      return;
    }

    this.master.set_isLoading({message: '', isLoading: true});

    this.bussiness.copiaOperazione(selected[0], selected).subscribe((r) => {
      if (r.RispostaOK) {
        let copiaOpRisp = r.RispostaStringa;
        this.bussiness.copiaCambiaSito(copiaOpRisp);
      } else
        this.master.changeErrorMsgType({show: true, msg: r.Errore, errorNumber: 0});
    });
  }

  public removeSelected() {
    if (!this.selected.length) {
      this.error('SelezionareAlmenoUnOperazione');
      return;
    }

    let prompt = this.selected.length > 1
    ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
    : this.transloco.translate('SoleDeletionConfirmation');

    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('ActivityDeletion'), prompt
    ).GiasSubscribe((R: DialogResult) => {
      if (R instanceof DialogCloseResult) return;
      if (R['returnObj']) {
        this.gridConfig.perform(HttpAction.REMOVE, this.selected)
          .GiasSubscribe(r => null);
      }
    })
  }

  public onBloccaAttivita() {
    let prompt: string;

    if (this.selected.length === 0) {
      this.error('SelezionareAlmenoUnOperazione');
      return;
    }
    if (this.selected.every(i => i.blocco_flag === '1')) return;
    if (this.selected.length > 1) {
      prompt = this.transloco.translate('MultipleLockConfirmation', [this.selected.length])
    } else {
      prompt = this.transloco.translate('SoleLockConfirmation');
    }

    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('ActivityLock'), prompt
    ).GiasSubscribe((R: DialogResult) => {
      if (R instanceof DialogCloseResult) return;
      let action = R as DialogAction;
      if (action.text == this.transloco.translate('Conferma')) {
        this.bussiness.bloccaAttivitaAgenda(this.selected)
          .GiasSubscribe(isOk => {
            if (isOk) this.gridConfig.refresh();
          });
      }
    })
  }

  public onSbloccaAttivita() {
    let prompt: string;

    if (this.selected.length === 0) {
      this.error('SelezionareAlmenoUnOperazione');
      return;
    }
    if (this.selected.every(i => i.blocco_flag === '0')) return;
    if (this.selected.length > 1) {
      prompt = this.transloco.translate('MultipleUnlockConfirmation', [this.selected.length])
    } else {
      prompt = this.transloco.translate('SoleUnlockConfirmation');
    }

    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('ActivityUnlock'), prompt
    ).GiasSubscribe((R: DialogResult) => {
      if (R instanceof DialogCloseResult) return;
      let action = R as DialogAction;
      if (action.text == this.transloco.translate('Conferma')) {
        this.bussiness.sbloccaAttivitaAgenda(this.selected)
          .GiasSubscribe(isOk => {
            if (isOk) this.gridConfig.refresh();
          });
      }
    });
  }

  public editSelected() {
    if (this.selected.length === 0) {
      this.error('SelezionareAlmenoUnOperazione');
      return;
    }

    // Aggiungo tutte le operazioni multiple collegate
    this.selected.filter(row => row['Raccoglitore_Cod'] !== "0")
      .forEach(row => this.gridChild.rows
        .filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod'])
        .forEach(c => c['Selected'] = true)
      )

    if (this.selected.some(a => a.blocco_flag === '1')) {
      this.error('qdc.LockedActivityError');
      return;
    }
    if (this.selected.some(a => a.id_agenda === '-1' || a.Lav_cod === '-1'
      || a.Lav_cod === '1022' || + a.Lav_cod === enum_LAVCOD.CURA)) {
      this.error('qdc.NotCulturalPracticeError');
      return;
    }
    if(this.selected.some(a=> this.bussiness.NascondiModificaInstallazioneReinnescoTrappole(a))) {
      this.error('ImpossibileModificaReinneschiInstallazioni');
      return;
    }

    this.openMultipleEdit = true;
  }

  public onCloseMultiEdit() {
    this.openMultipleEdit = false;
    this.gridConfig.refresh();
  }

  public async verificaConformita() {
    this.gias2010Redirector.redirectTo_Verifica_Conformita(null, enum_PagineGiasNG.Pagina_Menu_Agenda);
  }

  private setPermissions() {
    this.permessoSblocca = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Agenda_Operazioni_Blocco, 2);
    this.permessoBlocca = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Agenda_Operazioni_Sblocco, 2);
    this.permessoEdit = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);

    this.permessoMultiModifica = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.ManutenzioneArchivi_MultiModificaInterventi, 2) && this.permessoEdit;
    this.permessoMultiCancellazione = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.ManutenzioneArchivi_MultiCancellazioneInterventi, 2) && this.permessoEdit;

    this.permessoExportQdCToAgea = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Esportazione_Agea_NG, 0);
  }

  private error(content: string) {
    this.dialogService.baseError('Errore', content);
  }

  public ExportQdCToAgea() {
    this.gestioneRichieste.gestionePassaggioStessoSito_Aperto_in_Iframe(enum_PagineGiasNG.Pagina_Export_QdC_To_Agea,
      [], -1, this.transloco.translate("EsportazioneRegistroTrattamentiAgea"));
  }
}

function CreateFormGroup(fb: FormBuilder) {
  return fb.group({
    descrizione: new FormControl(''),
    numero: new FormControl(''),
    dataInizio: new FormControl(null),
    dataFine: new FormControl(null),
    nota: new FormControl(''),
  });
}
