import { Component, OnInit, OnDestroy, ViewChild, TemplateRef, Inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faFaucetDrip, faLock, faLockOpen, faMoneyBillWheat, faPenToSquare, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { DialogService, DialogRef, DialogAction, DialogResult } from '@progress/kendo-angular-dialog';
import { Appezzamento, PKAppezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { AppezzamentoXParcoMacchine } from 'app/Model/anagrafiche/appezzamento-x-parco-macchine';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { LinkedMachine } from 'app/Model/anagrafiche/ParcoMacchine';
import { SMARTPHONE_WIDTH, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { Enum_SiteRedirector, enum_PagineAgenda_2010, enum_CodificaPagPlanning } from 'app/Model/siti.enum';
import { enum_Security_Attivita, enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { BudgetService, SelectedBudget, BudgetAnagrafica } from 'app/Service/Budget/budget.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService } from 'app/Service/ServiceFactory/impianti.factory.service';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { GridPublicService, GiasMessageService } from 'gias-kendo-grid';
import { GiasDialogService, enum_TipoPermesso, DialogBooleanResult, ObjParametriAgenda, ImpiantiAgendaNG } from 'gias-ui-kit';
import { Subscription, Subject, takeUntil, tap, take, catchError, of, Observable, forkJoin, zip, switchMap, map, share } from 'rxjs';
import { EserciziCopiaSpostaAppezzamentiService } from './esercizi-copiaSpostaAppezzamenti/esercizi-copia-sposta-appezzamenti.service';
import { EserciziEventsService } from './esercizi-events.service';
import { EserciziHttpService } from './esercizi-grid.service';
import { AgriculturalPlotMachineLinkComponent } from './special-edit/agricultural-plots/agricultural-plots-machines-link/agricultural-plot-machine-link.component';
import { AxpMessageContentComponent } from './special-edit/agricultural-plots/agricultural-plots-machines-link/axp-message-content/axp-message-content.component';
import { AgriculturalExerciseContributeLinkComponent } from './special-edit/exercices/agricultural-exercise-contribute-link/agricultural-exercise-contribute-link.component';
import { SpecialEditComponent } from './special-edit/special-edit.component';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import {AgriculturalItem} from './special-edit/agricultural-item.model';

@Component({
  standalone: false,
  selector: 'app-esercizi',
  templateUrl: './esercizi.component.html',
  styleUrls: ['./esercizi.component.css'],
  providers: [
    ...generateGridProvidersAnagrafica(EserciziHttpService, EserciziComponent), EserciziEventsService
  ]
})
export class EserciziComponent implements OnInit, OnDestroy {
  changeDetectedSub: Subscription;
  objParamSub: Subscription;
  objParametriAgenda: ObjParametriAgenda;

  faLock: IconDefinition = faLock;
  faFaucetDrip: IconDefinition = faFaucetDrip;
  faMoneyBillWheat: IconDefinition = faMoneyBillWheat;
  faLockOpen: IconDefinition = faLockOpen;
  faPenToSquare: IconDefinition = faPenToSquare;

  SMARTPHONE_WIDTH: number = SMARTPHONE_WIDTH;

  permessoEdit: boolean;
  permessoRibaltaBdg: boolean;
  permessoRead: boolean;
  permessoAgendaRead: boolean;
  permessoAgendaEdit: boolean;
  permessoBloccaSblocca: boolean;
  permessoCopiaSposta: boolean = false;

  // we just care about the sementieri server, not the active sportello
  isSementieri$ = this.configurazioneSitiService
    .leggiChiave(EnumChiaviConfigurazioneSiti.Is_Sementieri)
    .pipe(
      take(1),
      map(configurazioneChiave => configurazioneChiave?.Valore?.toLowerCase() === 'true'),
      share()
    );

  private signal$: Subject<void> = new Subject();
  private readonly waterCountersClassCode: string = '05.07';

  private get checkedRows() {
    return this.kendoGridService.getValue().data.rows.filter((row: any) => row.Selected);
  }

  constructor(
    public objParametriAgendaService: ObjParametriAgendaService,
    private kendoGridService: GridPublicService,
    private masterservice: MasterService,
    public eserciziEventsService: EserciziEventsService,
    private permessiUtenteService: PermessiUtenteService,
    public dialogService: DialogService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private budgetService: BudgetService,
    private translocoService: TranslocoService,
    private copiaSpostaService: EserciziCopiaSpostaAppezzamentiService,
    private giasIFrameWindowService: GiasIFrameWindowService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) { }

  ngOnInit() {
    this.isSementieri$.subscribe(isSementieri => this.initComponent(isSementieri));
  }

  initComponent(isSementieri: boolean): void {
    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2) && !isSementieri;
    this.permessoRead = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 0);
    this.permessoAgendaRead = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 0) && this.budgetService.getBudget().activeBudget == false;
    this.permessoAgendaEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoBloccaSblocca = this.permessiUtenteService.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_SbloccaAnagrafe, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoCopiaSposta = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Appezzamento_CopiaSposta, 2) && this.budgetService.getBudget().activeBudget == false;
    this.permessoRibaltaBdg = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Budget_Ribaltamento_Su_Reale, 2) && this.budgetService.getBudget().activeBudget == true;
    this.masterservice.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });

    try {
      this.changeDetectedSub = this.kendoGridService.changeDetected.pipe(takeUntil(this.signal$)).subscribe((event: any) => {
        if (event?.action === 'info') {
          this.eserciziEventsService.infoAppezzamento(event);
        }
      });

      this.objParamSub = this.objParametriAgendaService.currentObjParametriAgenda.subscribe((data: ObjParametriAgenda) => {
        this.objParametriAgenda = data;
      });

      this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    } catch (e) {
      console.log('Error:', e);
    }

    this.masterservice.set_isLoading({ isLoading: false, message: '' });
  }

  isBudget(): boolean {
    return this.budgetService.isBudget();
  }

  getBudget(): SelectedBudget {
    return this.budgetService.getBudget();
  }

  impresaSelezionata() {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }

  ngOnDestroy() {
    this.objParamSub.unsubscribe();
    this.changeDetectedSub.unsubscribe();
    this.signal$.next();
    this.signal$.complete();
  }

  shouldDisableCmd(row: any) {
    return false;
  }

  canHandleMachinesLink(): boolean {
    return this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  canHandleContibutes(): boolean {
    return this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.GestioneContributiACA,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  specialEditAllowed(): boolean {
    return this.canEditWeaving() || this.canEditSlope() || this.canEditConstrain();
  }

  canEditWeaving(): boolean {
    return this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.GestioneAppezzamentiTessitura,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  canEditSlope(): boolean {
    return this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.GestioneAppezzamentiPendenza,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  canEditConstrain(): boolean {
    // TODO Salvo: sistemare l'enum per leggere il permesso corretto
    return this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.GestioneAppezzamentiPendenza, //GestioneEserciziVincoli
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  specialEditBtnTitle(): string {
    return `
    ${this.translocoService.translate('Modifica')}
    ${this.canEditWeaving() ? this.translocoService.translate('Tessitura') : ''}
    ${this.canEditSlope() ? (this.canEditWeaving() ? '/' : '') + this.translocoService.translate('Pendenza') : ''}
    ${this.canEditConstrain() ? (this.canEditSlope() || this.canEditWeaving() ? '/' : '') + this.translocoService.translate('Vincolo')  : ''}
    `;
  }

  onNuovo() {
    this.eserciziEventsService.onNuovo();
  }

  onSbloccaAppezzamenti() {
    this.onBloccaSbloccaAppezzamenti(false);
  }

  onBloccaAppezzamenti() {
    this.onBloccaSbloccaAppezzamenti(true);
  }

  onCopiaSpostaAppezzamenti() {
    let SelectedAppezza: Appezzamento[] = [];
    this.kendoGridService.getValue().data.rows.filter((row: any) => row.Selected).forEach((el: any) => {
      let indexElement = SelectedAppezza.findIndex((a) => {
        if (a.primaryKey.codice == el.APPEZZA &&
          a.primaryKey.centroAziendalePK.codice == el.SA_COD &&
          a.primaryKey.centroAziendalePK.partitaIva == el.PIVA) {
          return true;
        } else {
          return false;
        }
      });
      if (indexElement == -1) {
        SelectedAppezza.push(new Appezzamento({
          codice: el.APPEZZA,
          centroAziendalePK: {
            codice: el.SA_COD,
            partitaIva: el.PIVA
          }
        }));
      }
    });
    if (SelectedAppezza?.length == 0) {
      this.giasDialogService.baseError('Errore', 'MenuBS_Anagrafica_SelezionareImpianti');
    } else {
      this.copiaSpostaService.setAppezzamenti(SelectedAppezza);
      this.copiaSpostaService.setMostraPopup(true);
    }
  }

  onBloccaSbloccaAppezzamenti(blocca: boolean) {
    const SelectedAppezza = this.kendoGridService.getValue().data.rows.filter((row: any) => row.Selected).map((el: any) => {
      return new Appezzamento({
        codice: el.APPEZZA,
        centroAziendalePK: {
          codice: el.SA_COD,
          partitaIva: el.PIVA
        }
      });
    });
    if (SelectedAppezza.length == 0) {
      this.giasMessageService.warningMessage(this.translocoService.translate('MenuBS_Anagrafica_SelezionareAppezzamentiNonBloccati'));
      return;
    } else {
      //this.giasMessageService.infoMessagge('Selezionati ' + SelectedAppezza.length + ' appezzamenti' );
      this.masterservice.set_isLoading({ message: '', isLoading: true });
      this.appezzamentiService.bloccaSbloccaAppezzamenti(SelectedAppezza, blocca).pipe(takeUntil(this.signal$)).subscribe((val) => {
        this.masterservice.set_isLoading({ message: '', isLoading: false });
        if (val.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('OperazioneCompletataSuccesso'));
          let kg = this.kendoGridService.getValue();
          kg.data.rows = [];
          kg.forceRefresh = true;
          this.kendoGridService.next(kg);
          this.kendoGridService.refresh(true);
        }
      });
    }
  }

  onModificaMulptipla() {
    this.objParametriAgenda.Impianti = [];
    const nSelected = this.checkedRows.length;
    if (nSelected < 1) {
      this.giasMessageService.warningMessage(this.translocoService.translate('SelezionareAlmenoUnImpiantoColturale'));
      return;
    }

    this.populateParametriAgendaImpianti(this.checkedRows, this.mapRowsToImpiantiAgendaNG);
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Modifica_Multipla_PianoColturale,
      [{ key: 'f', value: 'ng', codifica: false }],
      this.objParametriAgenda).then(r => {
      window.location.href = r;
    });
  }

  ribaltaImpiantiBDG() {
    const selected = (<any[]>this.kendoGridService.getValue().data.rows.filter((row: any) => row.Selected));
    if (selected.length == 0) {
      this.giasMessageService.warningMessage('SelezionareAlmenoUnImpiantoColturale', false, true);
      return;
    }

    const idBudget = this.budgetService.getBudget().budgetId;
    let bdgSelectect: BudgetAnagrafica<Appezzamento>[] = selected.map((c) => {
      let pk = {
        codice: c.APPEZZA,
        centroAziendalePK: {
          codice: c.SA_COD,
          partitaIva: c.PIVA
        }
      };
      let elementoAnagrafico = new Appezzamento(pk);
      return new BudgetAnagrafica<Appezzamento>(idBudget, elementoAnagrafico);
    });

    this.masterservice.set_isLoading({ isLoading: true });
    this.budgetService.ribaltaImpiantiBudget(bdgSelectect).pipe(
      takeUntil(this.signal$),
      tap((resp) => {
        this.masterservice.set_isLoading({ isLoading: false });
        if (resp.RispostaOK) {
          if (resp.ErroriGias?.length > 0) {
            this.giasDialogService.baseInfo('', resp.ErroriGias[0].messaggio);
          } else {
            this.giasDialogService.baseSuccess('', resp.RispostaStringa);
            this.kendoGridService.refresh(true);
          }
        }
      })
    ).subscribe();
  }

  onLinkAppezzaToMachines(): void {
    if (this.checkedRows.length < 1) {
      this.giasMessageService.warningMessage(this.translocoService.translate('SelezionareAlmenoUnImpiantoColturale'));
      return;
    }

    const paramDate: Date = this.objParametriAgendaService.getObjParamValue().Data;
    if (paramDate == undefined || paramDate === AGRODATAINIZIO) {
      this.giasMessageService.warningMessage(this.translocoService.translate('ImpostareFiltroData'));
      return;
    }

    const actions: any[] = [
      { text: this.translocoService.translate('CancellaTutteLeAssociazioni'), primary: true, returnObj: { confirm: true, unlink: true } },
      { text: this.translocoService.translate('Conferma'), returnObj: { confirm: true, unlink: false } }
    ];

    const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
      this.translocoService.translate('AgriculturalPlotsWaterCounterLink'),
      AgriculturalPlotMachineLinkComponent,
      actions,
      window.innerWidth * 0.9,
      window.innerHeight,
      this.preventConfirmPlotsMachinesLink.bind(this)
    );

    const dialogInstance = dialog.content.instance;
    dialogInstance.listViewHeight$.next(0.6 * +dialog.dialog.instance.height);
    dialogInstance.checkedRows$.next(this.checkedRows);

    dialog.result.pipe().subscribe(resp => {
      let plots = dialogInstance.chosenAgriculturalPlots;
      let chosenMachine = dialogInstance.selectedMachine;
      let linkValidity: IntervalloTemporale = dialogInstance.linkValidity;

      if (resp['returnObj']?.confirm) {
        if (resp['returnObj']?.unlink) {
          // Unlink agricultural plots and machines
          this.removePlotsMachinesLinks(plots, chosenMachine);
        } else {
          // Link agricultural plots and machines
          this.linkAgriculturalPlotsToMachine(plots, chosenMachine, linkValidity);
        }
      }
      this.kendoGridService.refresh(true);
    });
  }

  onLinkExerciceToContribute(): void {
    if (!(this.checkedRows.length >= 1)) {
      this.giasMessageService.warningMessage(this.translocoService.translate('SelezionareAlmenoUnImpiantoColturale'));
      return;
    }

    const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
      this.translocoService.translate('AgriculturalExercicesContributesLink'),
      AgriculturalExerciseContributeLinkComponent,
      [],
      window.innerWidth * 0.9,
      window.innerHeight
    );

    const dialogInstance = dialog.content.instance;
    dialogInstance.listViewHeight$.next(0.61 * +dialog.dialog.instance.height);
    dialogInstance.checkedRows$.next(this.checkedRows);

    dialog.result.pipe().subscribe(resp => {
      this.kendoGridService.refresh(true);
    });
  }

  onSpecialEdit(): void {
    if (!(this.checkedRows.length >= 1)) {
      this.giasMessageService.warningMessage(this.translocoService.translate('SelezionareAlmenoUnImpiantoColturale'));
      return;
    }

    const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
      this.translocoService.translate('AgriculturalExercicesContributesLink'),
      SpecialEditComponent,
      [],
      window.innerWidth * 0.9,
      window.innerHeight
    );

    const dialogInstance = dialog.content.instance;
    dialogInstance.windowHeight$.next(+dialog.dialog.instance.height);
    dialogInstance.checkedRows$.next(this.checkedRows);

    dialog.result.pipe().subscribe(resp => {
      this.kendoGridService.refresh(true);
    });
  }

  redirectToPlanning(dataItem: any): void {
    this.generalRedirectToPlanning(dataItem, enum_CodificaPagPlanning.Prenotazioni_Piante_BS_EditPrenotazione, 'RichiestaMaterialeVivaistico');
  }

  redirectToViewOrderPlanning(dataItem: any): void {
    this.generalRedirectToPlanning(dataItem, enum_CodificaPagPlanning.Prenotazioni_Piante, 'VisualizzaOrdineMaterialeVivaistico');
  }

  private generalRedirectToPlanning(dataItem: any, pagina: enum_CodificaPagPlanning, keyTrasloco: any) {
    const newObjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));
    newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Budget_Menu_Anagrafica_Impianti;


    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaPlanning,
      pagina,
      [
        { key: 'modal', value: 'true', codifica: false },
        { key: 'bdg', value: JSON.stringify(this.prepareBdgForPlanning(dataItem)), codifica: true }
      ],
      newObjParametriAgenda
    ).then(resp => {
      resp += '';

      this.giasIFrameWindowService.open({
        title: this.translocoService.translate(keyTrasloco),
        content: resp,
        height: window.innerHeight,
        width: window.innerWidth * 0.9
      }, () => {
        console.log('refresh');
        this.kendoGridService.refresh(true);
      });
    });
  }

  private prepareBdgForPlanning(dataItem: any): BudgetAnagrafica<Appezzamento> {
    let bdg: BudgetAnagrafica<Appezzamento> = new BudgetAnagrafica<Appezzamento>(
      dataItem['Id_Budget'],
      this.prepareAppezzamentoForPlanning(dataItem)
    );
    return bdg;
  }

  private prepareAppezzamentoForPlanning(dataItem: any): Appezzamento {
    let appezzamento: Appezzamento = new Appezzamento({
      codice: dataItem['APPEZZA'],
      centroAziendalePK: { codice: dataItem['SA_COD'], partitaIva: dataItem['PIVA'] }
    });

    appezzamento.impianti = [
      new Impianto({ codice: dataItem['id_Reg'], appezzamentoPK: appezzamento.primaryKey })
    ];

    appezzamento.impianti[0].esercizi = [
      new Esercizio(dataItem.Progetto_Cod)
    ];

    return appezzamento;
  }

  private populateParametriAgendaImpianti(values: any[], mapFunc?: (...values) => ImpiantiAgendaNG[]): void {
    if (!!mapFunc) {
      this.objParametriAgenda.Impianti = mapFunc(values);
    } else {
      this.objParametriAgenda.Impianti = values;
    }
  }

  private mapRowsToImpiantiAgendaNG(rows: any[]): ImpiantiAgendaNG[] {
    return rows.map(el => {
      let impianto: ImpiantiAgendaNG = new ImpiantiAgendaNG();
      impianto.Piva = el.PIVA;
      impianto.Sa_Cod = el.SA_COD;
      impianto.Appezza = el.APPEZZA;
      impianto.Id_Reg = el.id_Reg;
      impianto.Progetto_Cod = el.Progetto_Cod;

      if (el.veg_cod > 0) {
        impianto.Veg_Cod = el.veg_cod;
        impianto.Id_Cod = 0;
      } else {
        impianto.Veg_Cod = 0;
        impianto.Id_Cod = el.Destinazione_Uso_Cod;
      }

      return impianto;
    });
  }

  private linkAgriculturalPlotsToMachine(plots: AgriculturalItem[], chosenMachine: object, validity: IntervalloTemporale): void {
    this.checkPlotsMultiAssociation(plots, this.waterCountersClassCode).pipe(
      tap(r => {
        if (r) {
          const plts: Appezzamento[] = plots.slice().map(p => {
            let plot: Appezzamento = p.toAppezzamento();
            plot.descrizione = plot.descrizione.split('-')[0];

            let machine: LinkedMachine<PKAppezzamento> = new LinkedMachine<PKAppezzamento>(plot.primaryKey, chosenMachine['Mac_Cod']);
            machine.partitaIva = chosenMachine['Piva'];
            machine.descrizione = chosenMachine['Mac_Des'];
            machine.linkValidity = validity;
            machine.validita = new IntervalloTemporale(chosenMachine['Validita_Inizio'], chosenMachine['Validita_Fine']);

            plot.linkedMachines = [machine];
            return plot;
          });

          this.appezzamentiService.writeAppezzamentiXParcoMacchine(plts).pipe(
            take(1),
            catchError(() => {
              this.giasMessageService.errorMessage('ErroreSalvataggio', false, true);
              return of(null);
            })
          ).subscribe(resp => {
            if (resp) {
              this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
              this.kendoGridService.refresh(true);
            } else {
              this.giasMessageService.errorMessage('ErroreSalvataggio', false, true);
            }
          });
        }
      })
    ).subscribe();
  }

  private removePlotsMachinesLinks(plots: AgriculturalItem[], chosenMachine: object): void {
    const plts: Appezzamento[] = plots.map(p => {
      let plot: Appezzamento = p.toAppezzamento();
      let machine: LinkedMachine<PKAppezzamento> = new LinkedMachine<PKAppezzamento>(plot.primaryKey, chosenMachine['Mac_Cod']);
      machine.partitaIva = chosenMachine['Piva'];
      machine.descrizione = chosenMachine['Mac_Des'];

      plot.linkedMachines = [machine];

      return plot;
    });

    this.appezzamentiService.deleteAppezzamentiXParcoMacchineRecords(plts).pipe(
      take(1),
      catchError(() => {
        this.giasMessageService.errorMessage('ErroreSalvataggio', false, true)
        return of(null);
      })
    ).subscribe(r => {
      if (r) {
        this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
        this.kendoGridService.refresh(true);
      } else {
        this.giasMessageService.errorMessage('ErroreSalvataggio', false, true);
      }
    });
  }

  private preventConfirmPlotsMachinesLink(ev: DialogBooleanResult | DialogAction, dialog?: DialogRef) {
    if (ev['returnObj']?.confirm && !ev['returnObj']?.unlink) {
      const machineFG = (dialog.content.instance as AgriculturalPlotMachineLinkComponent).machinesDdlForm;
      const selectedMachine = (dialog.content.instance as AgriculturalPlotMachineLinkComponent).selectedMachine;
      const plots: AgriculturalItem[] = (dialog.content.instance as AgriculturalPlotMachineLinkComponent).chosenAgriculturalPlots;
      const validityForm: FormGroup = (dialog.content.instance as AgriculturalPlotMachineLinkComponent).validityForm;

      if (selectedMachine['Mac_Cod'] == undefined) {
        this.giasMessageService.warningMessage('SelezionareUnContatoreAcqua', false, true);
        machineFG.get('machine').markAsTouched();
      }

      if (validityForm.invalid) {
        this.giasMessageService.warningMessage('IntervalloTemporaleNonValido', false, true);
      }

      if (plots.some(p => p.error)) {
        this.giasDialogService.baseError('ErroreAssociazione', 'WarningAssociazioneAppezzamentiContatoriValidita', true);
      }

      return selectedMachine['Mac_Cod'] == undefined || plots.some(p => p.error) || !validityForm.valid;
    }
  }

  private checkPlotsMultiAssociation(plots: AgriculturalItem[], classCode: string = ''): Observable<boolean> {
    const agriculturalPlots$ = of(plots.slice());
    const actualLinks$ = forkJoin(plots.slice().map(p => {
      const axp: AppezzamentoXParcoMacchine = new AppezzamentoXParcoMacchine(p.piva, p.saCod, p.appezza, 0, classCode);
      return this.appezzamentiService.readAppezzamentiXParcoMacchine(axp);
    }));

    const actions = [
      { text: this.translocoService.translate('Prosegui'), primary: true, returnObj: true },
      { text: this.translocoService.translate('Annulla'), returnObj: false }
    ];

    return zip(agriculturalPlots$, actualLinks$).pipe(
      switchMap(([pp, ll]) => {
        let resp: Observable<boolean>;
        if (ll.some(l => l.length > 0)) {
          const dialog: DialogRef = this.giasDialogService.dialogMessageRef('', AxpMessageContentComponent, actions);
          const dialogInstance = dialog.content.instance;
          const appPKs: PKAppezzamento[] = ll.filter(l => l.length > 0).flat().map(l => l.linkedItemPK);

          // picks only already linked plots descriptions
          dialogInstance.axpMultiLinkPlots = pp.filter(p => appPKs.some(l => {
            const key: PKAppezzamento = {
              codice: p.appezza,
              centroAziendalePK: {
                codice: p.saCod,
                partitaIva: p.piva
              }
            }
            return JSON.stringify(l) == JSON.stringify(key);
          })).map(p => p.description);

          resp = dialog.result.pipe(map<DialogResult, boolean>(r => r['returnObj']));
        } else {
          resp = of(true);
        }

        return resp;
      })
    );
  }
}
