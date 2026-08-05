import { Component } from '@angular/core';
import { MacchineXContatti, ProfilazioneImpreseService, Specie } from '../../services/profilazione-imprese.service';
import { BehaviorSubject, catchError, combineLatest, debounceTime, map, of, startWith, Subject, switchMap, tap, withLatestFrom } from 'rxjs';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { MasterService } from 'app/Service/master.service';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import { TranslocoService } from '@jsverse/transloco';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-macchine-operatori',
  templateUrl: './profilazione-imprese-macchine-operatori.component.html',
  styleUrls: ['./profilazione-imprese-macchine-operatori.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseMacchineOperatoriComponent {
  additionalSpecie = { Veg_Cod: 0, Veg_Des: 'Tutte le Specie' } as Specie;
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  faCircleSave = faCheckCircle;

  macchineLoading: boolean = false;
  contattiLoading: boolean = false;
  dataLoading: boolean = false;
  idProfilazione: number | null = null;
  isTimeBeingEdited: boolean = false;

  globalSubject = this.service.globalSubject;
  specieSubject = new BehaviorSubject<number>(0);
  gruppoOperazioneSubject = new Subject<string>();
  lavorazioneSubject = new BehaviorSubject<number>(null);
  macchineSelectedSubject = new BehaviorSubject<number[]>([]);
  contattiSelectedSubject = new BehaviorSubject<number[]>([]);
  reloadSubject = new Subject();

  piva$ = this.objParametriAgendaService.currentObjParametriAgenda.pipe(map(x => x.Piva));
  data$ = combineLatest([this.piva$, this.specieSubject, this.globalSubject, this.reloadSubject.pipe(startWith(null))]).pipe(
    debounceTime(100),
    tap(() => this.dataLoading = true),
    switchMap(([piva, vegCod, global, _]) => this.service.leggiProfilazioneMacchineXContatti(global ? '0' : piva, vegCod)),
    catchError(() => {
      this.showMessage(this.transloco.translate('profImprese.ErroreLetturaDatiProfilazione'), false);
      return of([] as MacchineXContatti[]);
    }),
    tap((data) => this.selectMacchineContatti(data.find(x => x.veg_cod == this.specieSubject.value && x.lav_cod == this.lavorazioneSubject.value))),
    tap(() => this.dataLoading = false)
  );

  constructor(
    private service: ProfilazioneImpreseService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private transloco: TranslocoService,
    private masterService: MasterService
  ) { }

  save(vegCod: number, lavCod: number | null, macchine: string[], contatti: string[], global: boolean, piva: string): void {
    if (vegCod == null) {
      this.showMessage(this.transloco.translate('profImprese.SelezionaPrimaUnaSpecie'), false);
      return;
    }

    if (lavCod == null) {
      this.showMessage(this.transloco.translate('profImprese.SelezionaPrimaUnaLavorazione'), false);
      return;
    }

    this.service
      .salvaImpresa({ Specie: vegCod, Lavorazione: lavCod, Macchine: macchine, Contatti: contatti, Piva: global ? '0' : piva })
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.DatiAggiornatiCorrettamente'), true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage(this.transloco.translate('profImprese.ErroreAggiornamento'), false);
      });
  }

  edit(item: MacchineXContatti): void {
    this.specieSubject.next(item.veg_cod);
    this.lavorazioneSubject.next(item.lav_cod);

    this.selectMacchineContatti(item);
  }

  delete(item: MacchineXContatti, global: boolean, piva: string): void {
    this.giasDialogService.dialogMessageObs_Result(this.transloco.translate('SeiSicuroDiEliminareQuestoElemento'), '')
      .pipe(tap(res => {
        if (res['returnObj']) {
          this.doDelete(item, global, piva);
        }
      })).subscribe();
  }

  propagate(item: MacchineXContatti, global: boolean, piva: string): void {
    this.giasDialogService.dialogMessageObs_Result('Propagare questo default su tutte le operazioni?', '')
      .pipe(tap(res => {
        if (res['returnObj']) {
          this.doPropagate(item, global, piva);
        }
      })).subscribe();
  }

  updateTimes(dataItem: MacchineXContatti): void {
    of(true)
      .pipe(
        withLatestFrom(this.globalSubject, this.piva$),
        switchMap(([_, global, piva]) => this.service.aggiornaOreMinuti({
          Piva: global ? '' : piva,
          LavCod: dataItem.lav_cod,
          VegCod: dataItem.veg_cod,
          Ore: dataItem.ore,
          Minuti: dataItem.minuti
        })),
        catchError(() => of(false)))
      .subscribe(res => {
        this.isTimeBeingEdited = false;
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.AggiornatoCorrettamente'), true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage(this.transloco.translate('profImprese.ErroreAggiornamento'), false);
      });
  }

  isRowSelected(item: MacchineXContatti): boolean {
    return this.specieSubject.value == item.veg_cod && this.lavorazioneSubject.value == item.lav_cod
  }

  rowCallback = (context: RowClassArgs) => {
    return { 'data-selected': this.isRowSelected(context.dataItem) };
  };

  private doDelete(item: MacchineXContatti, global: boolean, piva: string): void {
    this.service
      .eliminaProfilazione('macXlav', item.lav_cod, item.lav_cod, item.veg_cod, global ? '0' : piva)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.EliminatoCorrettamente'), true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage(this.transloco.translate('profImprese.ErroreEliminazione'), false);
      });
  }

  private doPropagate(item: MacchineXContatti, global: boolean, piva: string) {
    this.masterService.set_isLoading({ isLoading: true });
    this.dataLoading = true;
    this.service
      .propagaSuTutteLeLavorazioni(item.lav_cod, item.veg_cod, global ? '0' : piva)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        this.masterService.set_isLoading({ isLoading: false });
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.PropagatoCorrettamente'), true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage(this.transloco.translate('profImprese.ErrorePropagazione'), false);
      })
  }

  private showMessage(message: string, success: boolean): void {
    if (success) {
      this.giasMessageService.successMessage(message);
      return;
    }

    this.giasMessageService.errorMessage(message);
  }

  private selectMacchineContatti(item: MacchineXContatti | null): void {
    if (item == null) {
      this.macchineSelectedSubject.next([]);
      this.contattiSelectedSubject.next([]);
      return;
    }

    const macCods = item.mac_cods
      ?.split('|')
      .filter(x => x != '')
      .map(x => +x);
    this.macchineSelectedSubject.next(macCods ?? []);

    const contCods = item.cont_cods
      ?.split('|')
      .filter(x => x != '')
      .map(x => +x);
    this.contattiSelectedSubject.next(contCods ?? []);
  }
}
