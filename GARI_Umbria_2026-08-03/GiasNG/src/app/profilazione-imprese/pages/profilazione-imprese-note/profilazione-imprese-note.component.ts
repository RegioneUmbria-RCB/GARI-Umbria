import { Component } from '@angular/core';
import { Nota, ProfilazioneImpreseService, Specie } from '../../services/profilazione-imprese.service';
import { BehaviorSubject, catchError, combineLatest, debounceTime, map, of, startWith, Subject, switchMap, tap } from 'rxjs';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { TranslocoService } from '@jsverse/transloco';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-note',
  templateUrl: './profilazione-imprese-note.component.html',
  styleUrls: ['./profilazione-imprese-note.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseNoteComponent {

  additionalSpecie = { Veg_Cod: 0, Veg_Des: 'Tutte le Specie' } as Specie;
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  dataLoading: boolean = false;
  gruppoNoteDialogOpened: boolean = false;

  globalSubject = this.service.globalSubject;
  specieSubject = new BehaviorSubject<number>(0);
  campoApplicativoSubject = new Subject<number>();
  gruppoOperazioneSubject = new Subject<string>();
  lavorazioneSubject = new BehaviorSubject<number>(0);
  noteSelectedSubject = new BehaviorSubject<number[]>([]);
  reloadSubject = new Subject();

  piva$ = this.objParametriAgendaService.currentObjParametriAgenda.pipe(map(x => x.Piva))
  data$ = combineLatest([this.piva$, this.specieSubject, this.lavorazioneSubject, this.globalSubject, this.reloadSubject.pipe(startWith(null))]).pipe(
    debounceTime(100),
    tap(() => this.dataLoading = true),
    switchMap(([piva, vegCod, lavCod, global, _]) => this.service.leggiNote(vegCod, lavCod, global ? undefined : piva)),
    catchError(() => {
      this.showMessage("Errore lettura note", false);
      return of([]);
    }),
    tap(() => this.dataLoading = false)
  );

  constructor(
    private service: ProfilazioneImpreseService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private transloco: TranslocoService
  ) { }

  save(vegCod: number, notaUtilizzoCod: number, lavCod: number, note: number[], global: boolean, piva: string): void {
    if (vegCod == null) {
      this.showMessage("Seleziona prima una specie", false);
      return;
    }

    if (lavCod == null) {
      this.showMessage("Seleziona prima una lavorazione", false);
      return;
    }

    this.service
      .salvaNote({ Specie: vegCod, Lavorazione: lavCod, NotaUtilizzoCod: notaUtilizzoCod, Note: note, Piva: global ? undefined : piva })
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage("Dati aggiornati correttamente", true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage("Errore aggiornamento", false);
      });
  }

  edit(item: Nota): void {
    this.specieSubject.next(item.Veg_cod);
    this.campoApplicativoSubject.next(item.notautilizzo_cod);
    this.lavorazioneSubject.next(item.Lav_cod);

    const macCods = item.nota_cods
      ?.split('|')
      .filter(x => x != "")
      .map(x => +x);
    this.noteSelectedSubject.next(macCods ?? []);
  }

  delete(item: Nota, global: boolean, piva: string): void {
    this.giasDialogService.dialogMessageObs_Result(this.transloco.translate('SeiSicuroDiEliminareQuestoElemento'), '')
      .pipe(tap(res => {
        if (res['returnObj']) {
          this.doDelete(item, global, piva);
        }
      })).subscribe();
  }

  private doDelete(item: Nota, global: boolean, piva: string): void {
    this.service
      .eliminaProfilazione('note', item.notautilizzo_cod, item.Lav_cod, item.Veg_cod, global ? undefined : piva)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage("Eliminato correttamente", true);
          this.reloadSubject.next(null);
          return;
        }

        this.showMessage("Errore eliminazione", false);
      });
  }

  private showMessage(message: string, success: boolean): void {
    if (success) {
      this.giasMessageService.successMessage(message);
      return;
    }

    this.giasMessageService.errorMessage(message);
  }
}
