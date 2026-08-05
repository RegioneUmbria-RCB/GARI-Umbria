import { AfterViewInit, Component, Inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { ZooOperationsGridConfigService } from "./zoo-operations-grid.service";
import { faLock, faLockOpen } from "@fortawesome/free-solid-svg-icons";
import { DialogBooleanResult, GiasDialogService } from "../../../Service/gias-dialog.service";
import { filter, from, map, Observable, of, Subject, switchMap, take, takeUntil, tap } from "rxjs";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { BaseCodeDescr } from "../../../Model/baseClass/baseCodeDescr";
import { ZooRedirectorService } from "../../services/zoo-redirector.service";
import { ZooBDNform, ZooOperationsFilters } from "../../models/zoo-operations-filters.model";
import { ZooFiltersHelperService } from "../../services/zoo-filters-helper.service";
import { BussinessMenuAgendaService } from "../../../menu-agenda/shared_services/bussiness-logic.service";
import { ZooOperationGridFlatItem } from "../../models/zoo-operation-grid-item.model";
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProviders, GiasKendoGridComponent, GRID_HTTP_TOKEN, GridCommandItem, HttpAction, KendoGridRow, KendoServerResult } from 'gias-kendo-grid';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_PagineAgronicaSincro, enum_Security_Attivita, enum_TipoSincronizzazione_BDN, enum_TipoSincronizzazione_Treatments } from 'app/Model/TipiEnumerativi';
import { BaseCodeDescr as IBaseCodeDescr } from "../../../Service/api.service";
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { IntlService } from '@progress/kendo-angular-intl';

export enum enumZooSyncPages {
  SincronizzaBDN = 1,
  Genera_Modello4 = 2,
  SincronizzaVetInfo = 3,
  GeneraTrattamenti = 4,
  GeneraCarichi = 5,
  GeneraScarichi = 6,
  ControlloGiacenzeVetInfo = 7
}

@Component({
  standalone: false,
  selector: 'zoo-operations-grid',
  templateUrl: './zoo-operations-grid.component.html',
  styleUrls: ['./zoo-operations-grid.component.css'],
  providers: [
    BussinessMenuAgendaService,
    ...generateGridProviders(ZooOperationsGridConfigService, ZooOperationsGridComponent)
  ]
})
export class ZooOperationsGridComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('addOperationTemplate') addOperationTemplate: TemplateRef<any>;
  @ViewChild('selectCentreAndStableTemplate') selectCentreAndStableTemplate: TemplateRef<any>;
  @ViewChild('zooOpGrid') grid: GiasKendoGridComponent;
  @Input() filters$: Subject<ZooOperationsFilters>;
  @Output() editFavorites = new Subject<void>();

  protected readonly faLock = faLock;
  protected readonly faLockOpen = faLockOpen;
  protected readonly SHOW_FAVORITES_SETTINGS = true;
  protected innerForm: FormGroup;
  //protected editFavorites = false;
  protected readonly console = console;
  protected _favorites: BaseCodeDescr[];
  protected _operationTypes: BaseCodeDescr[];
  protected centers: IBaseCodeDescr[] = [];
  protected stables: IBaseCodeDescr[] = [];
  protected formCenterAndStable: FormGroup<ZooBDNform>;

  protected canEdit = true;
  protected canBDNwrite = false;
  protected canLockOperations = false;
  protected canUnlockOperations = false;

  private signal = new Subject<void>();
  public _lastUsedFilters: ZooOperationsFilters;

  commandsBDN: GridCommandItem[] = [
    new GridCommandItem("zoo.SincronizzaBDN", enumZooSyncPages.SincronizzaBDN, 'k-i-xi-sincro-bdn'),
    new GridCommandItem("zoo.ImportCapiMod4", enumZooSyncPages.Genera_Modello4, 'k-i-xi-import-capi-mod4'),
    new GridCommandItem("zoo.SincronizzaVETINFO", enumZooSyncPages.SincronizzaVetInfo, 'k-i-xi-vet-info'),
    new GridCommandItem("zoo.InviaTrattamentiVetInfo", enumZooSyncPages.GeneraTrattamenti, 'k-i-xi-vet-info-send'),
    new GridCommandItem("zoo.IngressiBDN", enumZooSyncPages.GeneraCarichi, 'k-i-xi-ingressi-bdn'),
    new GridCommandItem("zoo.UsciteBDN", enumZooSyncPages.GeneraScarichi, 'k-i-xi-uscite-bdn'),
    new GridCommandItem("zoo.GiacenzeGiasVetInfo", enumZooSyncPages.ControlloGiacenzeVetInfo, 'k-i-xi-vet-info')
  ];

  constructor(
    private transloco: TranslocoService,
    private dialog: GiasDialogService,
    private agenda: ObjParametriAgendaService,
    private zooRedirector: ZooRedirectorService,
    private zooHelper: ZooFiltersHelperService,
    private business: BussinessMenuAgendaService,
    private giasMessageService: GiasMessageService,
    private permissions: PermessiUtenteService,
    private intlService: IntlService
  ) {
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);
    this.canBDNwrite = this.permissions.canWritePermesso(enum_Security_Attivita.ManutenzioneArchivi_ImportaAnagraficheAnimali_XLS2GIAS);
    this.canLockOperations = this.permissions.canWritePermesso(enum_Security_Attivita.Agenda_Operazioni_Blocco);
    this.canUnlockOperations = this.permissions.canWritePermesso(enum_Security_Attivita.Agenda_Operazioni_Sblocco);
  }

  protected get operationTypes(): BaseCodeDescr[] {
    return this.innerForm.get('favorites')?.value ? this._favorites : this._operationTypes;
  }

  private get selected(): ZooOperationGridFlatItem[] {
    return this.grid.rows.filter(r => r['Selected']) as ZooOperationGridFlatItem[];
  }

  protected set operationTypes(operations: BaseCodeDescr[]) {
    this._operationTypes = operations;
  }

  private get currentPiva(): string {
    const agenda = this.agenda.getObjParamValue();
    return agenda.Piva ?? '';
  }

  ngOnInit(): void {
    this.loadOperations();
    this.initDialogForm();
  }

  ngAfterViewInit(): void {
    this.filters$.pipe(
      takeUntil(this.signal),
      filter(x => x !== null),
      tap(x => this._lastUsedFilters = x),
      switchMap(filters => this.grid.config.read(filters))
    ).subscribe(() => this.grid.publicService.refresh(true));
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  public newOperation() {
    this.innerForm = new FormGroup({
      operationType: new FormControl(''),
      favorites: new FormControl(false)
    });
    this.dialog.dialogMessageObs_Result(this.transloco.translate("NuovaAttività"), this.addOperationTemplate)
      .pipe(
        take(1),
        filter(r => r['returnObj'] === true),
        map(() => this.innerForm.value.operationType),
        tap(() => this.innerForm = null),
        switchMap((operationType: number) => {
          this.zooRedirector.redirectNewOperation(operationType);
          return of(true);
        })
      ).subscribe();
  }

  public removeSelected() {
    this.preventIfEmptySelection(() => {
      const prompt = this.selected.length > 1
        ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
        : this.transloco.translate('SoleDeletionConfirmation');
      this.dialog.dialogMessageObs_Result(this.transloco.translate('ActivityDeletion'), prompt)
        .pipe(
          filter(r => r['returnObj']),
          switchMap(() => this.grid.config.perform(HttpAction.REMOVE, this.selected)),
          tap(() => console.debug('After delete???')),
          switchMap(() => this.refresh())
        ).subscribe(() => console.debug('subscribe after delete???'));
    });
  }

  public lockOperation() {
    this.preventIfEmptySelection(() => {
      if (this.selected.every(i => i.Blocco_Flag === 1)) return;
      let prompt: string;
      if (this.selected.length > 1) {
        prompt = this.transloco.translate('MultipleLockConfirmation', [this.selected.length]);
      } else {
        prompt = this.transloco.translate('SoleLockConfirmation');
      }

      this.dialog.dialogMessageObs_Result(this.transloco.translate('ActivityLock'), prompt)
        .pipe(
          filter(r => r['returnObj']),
          switchMap(() => this.business.bloccaAttivitaZoo(this.selected)),
          filter(isOk => isOk),
          switchMap(() => this.refresh())
        ).subscribe();
    });
  }

  public unlockOperation() {
    this.preventIfEmptySelection(() => {
      if (this.selected.every(i => i.Blocco_Flag === 0)) return;
      let prompt: string;
      if (this.selected.length > 1) {
        prompt = this.transloco.translate('MultipleUnlockConfirmation', [this.selected.length]);
      } else {
        prompt = this.transloco.translate('SoleUnlockConfirmation');
      }

      this.dialog.dialogMessageObs_Result(this.transloco.translate('ActivityUnlock'), prompt)
        .pipe(
          filter(r => r['returnObj']),
          switchMap(() => this.business.sbloccaAttivitaZoo(this.selected)),
          filter(isOk => isOk),
          switchMap(() => this.refresh())
        ).subscribe();
    });
  }

  private preventIfEmptySelection(fun: Function) {
    if (this.selected.length > 0) {
      fun.call(this);
    } else {
      this.dialog.baseError('Errore_', 'SelezionaAttivitàPerContinuare');
    }
  }

  onRedirectToBDNoperations(event) {

    let rows: KendoGridRow[] = this.grid.rows;

    if (rows.length === 0 || rows.some(item => item['Piva'] !== rows[0]['Piva'] || item['Sa_Cod'] !== rows[0]['Sa_Cod'] || item['STA_NUM'] !== rows[0]['STA_NUM'])) {

      // se abbiamo un solo centro e una sola stalla, possiamo procedere direttamente al redirect
      if (this.centers.length === 2 && this.stables.length === 1) {
        const key = `${this.currentPiva}_${this.centers[0].codice}_${this.stables[0].codice}`;
        this.handleRedirect(event.action, key);
        return;
      }

      //altrimenti facciamo scegliere il centro e la stalla
      this.dialog.dialogMessageObs_Result(this.transloco.translate('zoo.SelezionaCentroStalla'), 
                                          this.selectCentreAndStableTemplate,
                                          undefined,
                                          undefined,
                                          undefined,
                                          (action: DialogBooleanResult) => {
                                            // Logica per prevenire l'azione
                                            if (action.returnObj && (this.formCenterAndStable.controls.center.value == 0 || this.formCenterAndStable.controls.stable.value == 0)) {
                                              this.giasMessageService.errorMessage(this.transloco.translate('zoo.CompilareCentroEStalla'));
                                              return true; // Blocca l'azione
                                            }
                                            return false; // Permetti l'azione
                                          })
      .pipe(
        filter(result => result['returnObj']), // Controlla se l'utente ha confermato
        map(result => {
          return ({
            centroAziendale: this.formCenterAndStable.controls.center.value,
            stalla: this.formCenterAndStable.controls.stable.value
        });}),
        filter(selection => selection !== null),
        switchMap(selection => {
          // Usa la selezione per creare la chiave
          const key = `${this.currentPiva}_${selection.centroAziendale}_${selection.stalla}`;
          return of(key);
        })
      ).subscribe(key => {
        // Continua con la logica di redirect
        this.handleRedirect(event.action, key);
      });
      return;
    }

    let chiave = rows[0]['Piva'] + '_' + rows[0]['Sa_Cod'] + '_' + rows[0]['STA_NUM'];
    this.handleRedirect(event.action, chiave);
  }

  private handleRedirect(action: any, chiave: string) {
    switch (action) {
      case enumZooSyncPages.SincronizzaBDN:
        this.zooRedirector.redirectToSincroBDN(chiave, enum_PagineAgronicaSincro.SincronizzatoreBDN, enum_TipoSincronizzazione_BDN.SincronizzaBDN);
        break;
      case enumZooSyncPages.Genera_Modello4:
        this.zooRedirector.redirectToSincroBDN(chiave, enum_PagineAgronicaSincro.SincronizzazioneStalleBDN, enum_TipoSincronizzazione_BDN.SincronizzaBDN);
        break;
      case enumZooSyncPages.SincronizzaVetInfo:
        this.zooRedirector.redirectToSincroBDN(chiave, enum_PagineAgronicaSincro.SincronizzatoreBDN, enum_TipoSincronizzazione_BDN.SincronizzaVetInfo);
        break;
      case enumZooSyncPages.GeneraTrattamenti:
        this.zooRedirector.redirectToSincroTreatments(chiave, enum_PagineAgronicaSincro.InvioTrattamentiZooVetInfo, enum_TipoSincronizzazione_Treatments.GeneraTrattamenti);
        break;
      case enumZooSyncPages.GeneraCarichi:
        this.zooRedirector.redirectToSincroBDN(chiave, enum_PagineAgronicaSincro.SincronizzatoreBDN, enum_TipoSincronizzazione_BDN.GeneraCarichi);
        break;
      case enumZooSyncPages.GeneraScarichi:
        this.zooRedirector.redirectToSincroBDN(chiave, enum_PagineAgronicaSincro.SincronizzatoreBDN, enum_TipoSincronizzazione_BDN.GeneraScarichi);
        break;
      case enumZooSyncPages.ControlloGiacenzeVetInfo:
        this.zooRedirector.redirectToSincroTreatments(chiave, enum_PagineAgronicaSincro.InvioTrattamentiZooVetInfo, enum_TipoSincronizzazione_Treatments.ControlloGiacenzeVetInfo);
        break;
    }
  }

  /**
   * Loads both all the available operations and the user's favorites ones.
   * @private
   */
  private loadOperations() {
    this.zooHelper.loadZooOperations()
      .pipe(
        map(operations => operations.map(o => new BaseCodeDescr(+o.codice, o.descrizione))),
        tap(operations => this.operationTypes = operations),
        switchMap(() => this.zooHelper.loadFavorites()),
        map(favorites => {
          let fav: BaseCodeDescr[] = [];
          for (let f of favorites) {
            const found = this._operationTypes.find(x => x.codice === +f.codice);
            fav.push(new BaseCodeDescr(+f.codice, found.descrizione));
          }
          return fav;
        })
      ).subscribe(favorites => this._favorites = favorites);
  }

  private initDialogForm() {
    this.formCenterAndStable = new FormGroup({
      center: new FormControl(0, Validators.required),
      stable: new FormControl(0, Validators.required)
    });

    this.zooHelper.loadBusinessCenters(this.currentPiva)
      .subscribe(centers => { 
                              this.centers = centers; 
                              if (centers.length == 2) {
                                this.formCenterAndStable.controls.center.setValue(centers[1].codice);
                                return;
                              }
                            });

    this.formCenterAndStable.controls.center.valueChanges
    .pipe(
      switchMap((center) =>  {
        this.console.log('Selected center changed to: ', center);
        if (center == 0) {
          return of([]);
        }
        return this.zooHelper.loadStables(this.currentPiva, center);
      })
    )
    .subscribe(stables => { 
      this.stables = stables;
      if (stables.length == 2) {
        this.formCenterAndStable.controls.stable.setValue(stables[1].codice);
        return;
      }
      if (stables.length == 1) {
        this.formCenterAndStable.controls.stable.setValue(stables[0].codice);
        return;
      }
      if (stables.length == 0) {
        this.stables = [{codice:0, descrizione:''}];
        this.formCenterAndStable.controls.stable.setValue(0);
        return;
      }
    });
  }

  private refresh(): Observable<KendoServerResult> {
    this._lastUsedFilters.forceReload = true;
    return this.grid.config.read(this._lastUsedFilters)
      .pipe(
        tap(() => this._lastUsedFilters.forceReload = false),
        tap(newData => this.grid.publicService.refresh(true, newData))
      );
  }

  public lastDateFiltered(): string{
    let firstDate = "";
    let lastDate = "";
    if (this._lastUsedFilters?.from != null && this._lastUsedFilters?.from != undefined){
      firstDate = this.intlService.formatDate(this._lastUsedFilters.from, "dd/MM/yy");
    }
    if (this._lastUsedFilters?.to != null && this._lastUsedFilters?.to != undefined){
      lastDate = this.intlService.formatDate(this._lastUsedFilters.to, "dd/MM/yy");
    }
    return firstDate + " - " + lastDate;
  }

  public selectedStable(): string {
    let rows: KendoGridRow[] = this.grid?.rows;
    let strSelectedStable = "";
    if (rows){
      if (!(rows.length === 0 || rows.some(item => item['Piva'] !== rows[0]['Piva'] || item['Sa_Cod'] !== rows[0]['Sa_Cod'] || item['STA_NUM'] !== rows[0]['STA_NUM']))) {
        strSelectedStable = rows[0]["STA_DES"];
      }
    }
    return strSelectedStable;
  }

}
