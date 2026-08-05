import { Component, ContentChild, Inject, OnDestroy, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { faArrowRight, faBook } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { DialogCloseResult, DialogResult } from '@progress/kendo-angular-dialog';
import { GRID_HTTP_TOKEN, GridPublicService, generateGridProviders, GiasKendoGridComponent, HttpAction } from 'gias-kendo-grid';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { BrogliaccioGridConfig } from './brogliaccio-grid.service';
import { Observable, of, Subject, takeUntil, map, switchMap, take, from } from 'rxjs';
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { RispostaStandard } from 'app/Service/master.service';
import { Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import { enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { BussinessMenuAgendaService } from 'app/menu-agenda/shared_services/bussiness-logic.service';
import { MenuAgendaDataStore } from 'app/menu-agenda/shared_services/menu-agenda-datastore.service';
import { FiltersService } from '../filters/filters.service';
import { BrogliaccioRow } from '../utils';

@Component({
  standalone: false,
  selector: 'brogliaccio-table',
  templateUrl: './brogliaccio.component.html',
  styleUrls: ['./brogliaccio.component.scss'],
  providers: [
    ...generateGridProviders(BrogliaccioGridConfig, BrogliaccioGridComponent),
    BussinessMenuAgendaService
  ],
  encapsulation: ViewEncapsulation.None,
})
export class BrogliaccioGridComponent implements OnDestroy {

  ObjParametriAgenda: ObjParametriAgenda;
  faArrow = faArrowRight;
  faBook = faBook;
  signal: Subject<void> = new Subject();


  @ContentChild('stampeRef') stampeRef: TemplateRef<any>;
  @ViewChild('warningDialogRef', { static: true }) public warningDialogRef: TemplateRef<any>;
  public warningDialogMsg: string;

  permessoEdit: boolean = true;
  permessoMultiCancellazione: boolean = true;

  get impostazioniApp() {
    return this.filters.impostazioniApp;
  }

  /** Documentale */
  @ContentChild('gridCommands') gridCommands: TemplateRef<any>;

  @ViewChild(GiasKendoGridComponent) gridChild: GiasKendoGridComponent;


  constructor(private agenda: ObjParametriAgendaService,
    private bussiness: BussinessMenuAgendaService,
    private gpubService: GridPublicService,
    protected store: MenuAgendaDataStore,
    private filters: FiltersService,
    private transloco: TranslocoService,
    private dialogService: GiasDialogService,
    @Inject(GRID_HTTP_TOKEN) private conf: BrogliaccioGridConfig,
    private permessiUtenteService: PermessiUtenteService) {

    this.ObjParametriAgenda = this.agenda.getObjParamValue();
    this.agenda.currentObjParametriAgenda.subscribe((obj) => this.ObjParametriAgenda = obj);

    this.setPermissions();
  }

  get selected(): BrogliaccioRow[] {
    return this.store.gridDataBrogliaccio.rows.filter(r => r['Selected']) as BrogliaccioRow[];
  }

  private setPermissions() {
    this.permessoEdit = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Brogliaccio, 2);
    this.permessoMultiCancellazione = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.ManutenzioneArchivi_MultiCancellazioneInterventi, 2);
  }

  importaAgendaDaTabelleAPP() {
    if (this.impostazioniApp.SincroDatiApp) {
      this.store.CaricaAgendaDaTabelleAPP(this.impostazioniApp.ImportaSoloAziendaSelezionata, "10,90")
        .pipe(take(1), switchMap((risp: RispostaStandard) => {
          return this.showDialog(risp);
        }))
        .subscribe(() => {
          this.store.azzeraDatiGrigliaBrogliaccio();
          this.store.azzeraDatiGrigliaRicette();
          this.gpubService.refresh(true);
        });
    } else {
      this.store.ImportaRicetteDaTabelleAPP(this.impostazioniApp.ImportaSoloAziendaSelezionata)
        .pipe(takeUntil(this.signal), switchMap((risp: RispostaStandard) => {
          return this.showDialog(risp);
        }))
        .subscribe(() => {
          this.store.azzeraDatiGrigliaBrogliaccio();
          this.store.azzeraDatiGrigliaRicette();
          this.gpubService.refresh(true);
        });
    }
  }

  showDialog(risposta: RispostaStandard): any {
    const avioCaricamentoMsg = this.transloco.translate('AvioCaricamentoBrogliaccio');
    let msg: string;

    if (risposta.RispostaStringa != "")
      msg = risposta.RispostaStringa + "<br>" + avioCaricamentoMsg;
    else if (risposta.Errore != "")
      msg = risposta.Errore + "<br>" + avioCaricamentoMsg;
    else
      msg = avioCaricamentoMsg;

    const dialogTitle = this.transloco.translate('ImportazioneDatiApp');
    return from(this.dialogService.baseWarning(dialogTitle, msg, false))
  }

  /** La gestione del pulsante Codifica Prodotti APP */
  public CodificaProdottiAPP() {
    this.conf.ReindirizzaACodificaProdottiAPP();
  }

  public removeSelected() {
    this.validateSelected()
      .pipe(
        takeUntil(this.signal),
        map(canDelete => {
          if (canDelete) {
            return this.selected.length > 1
              ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
              : this.transloco.translate('SoleDeletionConfirmation');
          }
          return null;
        }),
        switchMap(prompt => {
          if (!!prompt && prompt !== '') this.showDeletionConfirm(prompt);
          else if (prompt === '') return this.conf.perform(HttpAction.REMOVE, this.selected);
          return of(null);
        })
      ).subscribe();
  }

  private showDeletionConfirm(prompt: string) {
    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('ActivityDeletion'), prompt
    ).pipe(
      takeUntil(this.signal),
      switchMap((R: DialogResult) => {
        if (R instanceof DialogCloseResult) return of(null);
        if (R['returnObj']) {
          return this.conf.perform(HttpAction.REMOVE, this.selected);
        }
      })
    ).subscribe();
  }

  private validateSelected(): Observable<boolean> {
    const selected = this.selected;
    if (!selected.length) {
      this.error('SelezionareAlmenoUnOperazione');
      return of(false);
    }
    const inUso = selected.filter(r => r.in_uso === '1');
    if (inUso.length > 0) {
      return this.messaggioWarnInUso();
    }
    return of(true)
  }

  private messaggioWarnInUso() {
    const inUso = this.selected.filter(r => r.in_uso === '1');
    const obs = new Subject<boolean>();
    const complete = (val: boolean) => {
      obs.next(val);
      obs.complete();
    }

    if (inUso.length === this.selected.length) {
      this.dialogService.baseError(
        '', this.transloco.translate('ErroreCancellazioneBrogliaccioSalvateAgenda'), false
      );
      complete(false);
    } else {
      let messaggio = this.transloco.translate('AttenzioneBrogliaccioSalvateAgenda');
      messaggio += "\n" + this.transloco.translate('Ricette') + ": ";
      messaggio += inUso.map(e => '- ' + e.Descrizione_Unica)
        .reduce((a, b) => a + '<BR/>' + b)
      this.dialogService.baseWarning('', messaggio, false).then(resp => {
        if (resp['returnObj'])
          inUso.forEach(r => r['Selected'] = false);
        complete(resp['returnObj']);
      });
    }

    return obs;
  }

  /** Gestione crea nuovo brogliaccio */
  public onNuovo() {
    this.bussiness.creaNuovaOperazione(Tipo_Ricetta.Standard_Destinazioni);
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  private error(content: string) {
    this.dialogService.baseError('Errore', content);
  }

  MostraBtnNuovoBrogliaccio(): boolean {
    let mostra = false;

    if (this.permessoEdit && !this.conf.SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO) {
      mostra = true;
    }

    return mostra;
  }
}

