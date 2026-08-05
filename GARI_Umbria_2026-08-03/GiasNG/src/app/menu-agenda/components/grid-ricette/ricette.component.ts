import { Component, ContentChild, Inject, OnDestroy, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import {faArrowUpFromBracket, faFileExport, faMoneyBillWheat, faTractor} from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { DialogCloseResult, DialogResult } from '@progress/kendo-angular-dialog';
import { GridPublicService, HttpAction, GRID_HTTP_TOKEN, generateGridProviders, GiasKendoGridComponent } from 'gias-kendo-grid';
import {Observable, of, Subject, switchMap, map, takeUntil, forkJoin} from 'rxjs';
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import {MasterService, rispostaStandard} from 'app/Service/master.service';
import { Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import { enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { MenuAgendaDataStore } from 'app/menu-agenda/shared_services/menu-agenda-datastore.service';
import { BussinessMenuAgendaService } from 'app/menu-agenda/shared_services/bussiness-logic.service';
import { RicettaRow } from '../utils';
import { RicetteService } from './ricette.service';
import {RicetteSmartTractorService, SendPrescriptionResponse} from './ricette-smart-tractor.service';
import {RicetteGridConfig} from './ricette-grid.service';
import {ObjParametriAgendaService} from '../../../Service/obj-parametri-agenda.service';

@Component({
  standalone: false,
  selector: 'grid-ricette',
  templateUrl: './ricette.component.html',
  styleUrls: ['./ricette.component.css'],
  providers: [
    ...generateGridProviders(RicetteGridConfig, RicetteComponent),
    BussinessMenuAgendaService
  ],
  encapsulation: ViewEncapsulation.None,
})
export class RicetteComponent implements OnDestroy {

  /** Pulsanti Documentale */
  @ContentChild('gridCommands') gridCommands: TemplateRef<any>;
  @ContentChild('stampeRef') stampeRef: TemplateRef<any>;

  @ViewChild(GiasKendoGridComponent) gridChild: GiasKendoGridComponent;

  permessoEdit: boolean = true;
  permessoMultiCancellazione: boolean = false;
  faExport = faFileExport;
  faUpload = faArrowUpFromBracket;
  signal: Subject<void> = new Subject();

  constructor(
    @Inject(GRID_HTTP_TOKEN) private grid: RicetteGridConfig,
    private bussiness: BussinessMenuAgendaService,
    private masterService: MasterService,
    protected store: MenuAgendaDataStore,
    private transloco: TranslocoService,
    private dialogService: GiasDialogService,
    private permessiUtenteService: PermessiUtenteService,
    private ricetteService: RicetteService,
    private ricetteSmartTractorService: RicetteSmartTractorService,
    private gridpublicService: GridPublicService,
    private objParametriAgendaService: ObjParametriAgendaService,
    @Inject(GRID_HTTP_TOKEN)
    private ricetteGridConfig: RicetteGridConfig
  ) {
    this.setPermissions();
  }

  get selected(): RicettaRow[] {
    return this.store.gridDataRicette.rows.filter(r => r['Selected']) as RicettaRow[];
  }

  ngOnDestroy() {
    this.signal.next();
    this.signal.complete();
  }

  private setPermissions() {
    this.permessoEdit = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.Gest_Ricette, 2);
    this.permessoMultiCancellazione = this.permessiUtenteService.getPermesso(
      enum_Security_Attivita.ManutenzioneArchivi_MultiCancellazioneInterventi, 2);
  }

  apriRicettaDaStampare(row: RicettaRow) {
    this.grid.apriRicettaDaStampare(row.Ricetta_Cod);
  }

  apriPianoLavoriDellaRicetta(row: RicettaRow) {
    this.grid.apriPianoLavoriDaStampare(row.Ricetta_Cod);
  }

  onSelectionChange($event: SelectionEvent): void {
    const selected: RicettaRow[] = $event.selectedRows.map(x => x.dataItem);
    const deselected: RicettaRow[] = $event.deselectedRows.map(x => x.dataItem);
    this.ricetteService.select(selected, deselected);
  }

  public async onInviaAdApp(item: any) {
    if (item.actionName !== 'InviaDatiAPP' || !this.gridChild || !this.gridChild.rows) return;
    let multi: string[] = this.gridChild.rows.filter(r => r['Selected'])
      .filter(row => row['Raccoglitore_Cod'] !== '0')
      .map(r => r['Raccoglitore_Cod']);

    if (multi.length)
      this.gridChild.rows.filter(r => multi.includes(r['Raccoglitore_Cod']))
        .forEach(r => r['Selected'] = true);

    let selected = this.gridChild.rows.filter(r => r['Selected']);

    if (!selected.length) {
      this.dialogService.baseError('', 'SelezionareAlmenoUnaRicetta', true);
      return;
    }
    if (selected.some(r => r['in_uso'] === '1')) {
      let trKey = selected.length === 1 ? 'ImpossibileInviareRicettaInUso' : 'ImpossibileInviareRicetteInUso';
      this.dialogService.baseError('', trKey, true);
      return;
    }

    if (selected.some(r => r['Invia_App'] === '1')) {
      let trKey = selected.length === 1 ? 'RicettaGiaInviataAllApp' : 'RicetteGiaInviateAllApp';
      this.dialogService.baseError('', trKey, true);
      return;
    }

    let resp = await this.dialogService.baseWarning(
      this.transloco.translate('InviaDatiAPP'),
      this.transloco.translate('ImpossibileModificareRicetteDopoConferma'),
      false
    );

    if (resp['returnObj']) {
      // Nella funzione lato serve uso il campo APP_Ricetta_Operazione_ID
      // come identificativo della ricetta
      this.masterService.set_isLoading({ isLoading: true });

      this.bussiness.inviaRicettaApp(selected as RicettaRow[]).then(r => {
        this.masterService.set_isLoading({ isLoading: false });
        //Ricarico la grid se la chiamata è andata a buon fine
        if (r.RispostaOK) {
          this.grid.applicaFiltri = true;
          this.gridpublicService.refresh(true);
          if (r.ErroriGias.length > 0) {
            this.dialogService.baseError('', r.ErroriGias[0].messaggio, false);
          } else {
            if (selected.length > 1) {
              this.dialogService.baseSuccess('', 'Ricette inviate correttamente all\'APP', false);
            } else {
              this.dialogService.baseSuccess('', 'Ricetta inviata correttamente all\'APP', false);
            }
          }
        }
      });
    }

  }

  public onInviaASmartTractor(item: any) {
    if (!this.gridChild || !this.gridChild.rows) return;

    let multi: string[] = this.gridChild.rows.filter(r => r['Selected'])
      .filter(row => row['Raccoglitore_Cod'] !== '0')
      .map(r => r['Raccoglitore_Cod']);

    if (multi.length)
      this.gridChild.rows.filter(r => multi.includes(r['Raccoglitore_Cod']))
        .forEach(r => r['Selected'] = true);

    let selected = this.gridChild.rows.filter(r => r['Selected']);

    if (selected.length !== 1) {
      this.dialogService.baseError('', 'SelezionareUnaRicetta', true);
      return;
    }
    if (selected.some(r => r['in_uso'] === '1')) {
      let trKey = selected.length === 1 ? 'ImpossibileInviareRicettaInUso' : 'ImpossibileInviareRicetteInUso';
      this.dialogService.baseError('', trKey, true);
      return;
    }

    if (selected.some(r => r['Invia_App'] === '1')) {
      let trKey = selected.length === 1 ? 'RicettaGiaInviataAllApp' : 'RicetteGiaInviateAllApp';
      this.dialogService.baseError('', trKey, true);
      return;
    }

    // if (selected.some(r => r['Invia_ST'] === '1')) {
    //   this.dialogService.baseError('', 'RicettaGiaInviataASmartTractor', true);
    //   return;
    // }

    this.dialogService.warningObs(
      this.transloco.translate('SendDataToSmartTractor'),
      '',
      false
    ).pipe(
      switchMap(dialogResp => {
        if (dialogResp) {
          this.masterService.set_isLoading({isLoading: true});
          return this.ricetteSmartTractorService.sendPrescriptionToSTEngine(selected[0]['Ricetta_Operazione_Cod']);
        } else {
          return of(null);
        }
      }),
      switchMap(apiResp => {
        this.masterService.set_isLoading({isLoading: false});
        if (apiResp != null) {
          const response = <rispostaStandard<SendPrescriptionResponse[]>>apiResp;
          if (response?.RispostaOK) {
            this.grid.applicaFiltri = true;
            this.gridpublicService.refresh(true);

            if (response?.RispostaStringa.some(r => r.Error != '')) {
              this.smartTractorSendErrorMessage(response?.RispostaStringa);
            } else {
              this.dialogService.baseSuccess('SendDataToSmartTractor', 'DispatchSuccessfull');
            }
          } else {
            this.dialogService.baseError('SendDataToSmartTractor', response?.ErroriGias[0].messaggio, false);
          }
        }
        return of(null);
      })
    ).subscribe();
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
          else if (prompt === '') return this.ricetteGridConfig.perform(HttpAction.REMOVE, this.selected, true);
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
          return this.ricetteGridConfig.perform(HttpAction.REMOVE, this.selected, true);
        }
      })
    ).subscribe();
  }

  public onNuovo() {
    this.bussiness.creaNuovaOperazione(Tipo_Ricetta.Standard_Destinazioni);
  }

  private error(content: string) {
    this.dialogService.baseError('Errore', content);
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
        '', this.transloco.translate('ErroreCancellazioneRicetteSalvateAgenda'), false
      );
      complete(false);
    } else {
      let messaggio = this.transloco.translate('AttenzioneRicetteSalvateAgenda');
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

  private smartTractorSendErrorMessage(responses: SendPrescriptionResponse[]): void {
    // let errors = new Map<number, string>();
    const errorResponses = responses.filter(r => r.Error != '');
    //.map(r => errors.set(r.MacCod, r.Error));

    // let obs: Observable<rispostaStandard<ParcoMacchine>>[] = [];
    // [...errors.keys()].forEach(k => {
    //   let objParams: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    //   objParams.Mac_Cod = k;
    //   // obs.push(this.macchineService.leggiMacchina(objParams));
    // });
    //
    // forkJoin(obs).subscribe(i => {
    //   console.log(i);
    // });

    let content: string = `${this.transloco.translate('ErrorsDuringDispatchToSmartTractor')} :`;
    errorResponses.forEach(r => {
      content = content.concat(`\n- ${r.Error}`);
    });
    this.dialogService.baseError('SendDataToSmartTractor', content);
  }

  protected readonly faTractor = faTractor;
}
