import { Component } from '@angular/core';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { catchError, combineLatest, filter, map, of, startWith, Subject, switchMap, tap, withLatestFrom } from 'rxjs';
import { GruppoNota, Nota, NotaXGruppo, NoteIntervento, ProfilazioneImpreseService } from '../../services/profilazione-imprese.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-gruppo-note-dialog',
  templateUrl: './profilazione-imprese-gruppo-note-dialog.component.html',
  styleUrls: ['./profilazione-imprese-gruppo-note-dialog.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseGruppoNoteDialogComponent {

  groupDialog: GruppoNota | null = null;
  groupDialogLoading: boolean = false;
  notaDialog: string | null = null;
  noteLoading: boolean = false;

  gruppoNotaSubject = new Subject<GruppoNota>();
  reloadGroupsSubject = new Subject();
  reloadNoteSubject = new Subject();
  gruppoNota$ = this.reloadGroupsSubject
    .pipe(
      startWith(null),
      switchMap(() => this.service.leggiGruppoNota()),
      tap(x => x.forEach(y => y.NotaGruppo_Des = y.NotaGruppo_Cod < 0 ? `${y.NotaGruppo_Des} (Standard)` : y.NotaGruppo_Des))
    );
  campoApplicativo$ = this.service.leggiNoteIntervento();
  note$ = combineLatest([this.gruppoNotaSubject, this.reloadNoteSubject.asObservable().pipe(startWith(null))])
    .pipe(
      tap(() => this.noteLoading = true),
      map(([group, _]) => group),
      switchMap(group => this.service.leggiNotePerGruppoNota(group.NotaGruppo_Cod)),
      catchError(() => of([])),
      map(notes => this.compactNotes(notes)),
      tap(() => this.noteLoading = false)
    );

  utilizzo$ = this.gruppoNotaSubject.pipe(
    filter(gruppo => gruppo?.NotaGruppo_Des != null),
    switchMap(gruppo => this.service.leggiNoteInterventoUtilizzoGruppi(gruppo.NotaGruppo_Cod)),
    withLatestFrom(this.campoApplicativo$),
    map(([gruppiUtilizzo, utilizzo]) => this.mapGruppiUtilizzo(gruppiUtilizzo, utilizzo))
  );

  constructor(
    private service: ProfilazioneImpreseService,
    private giasMessageService: GiasMessageService,
    private transloco: TranslocoService,
    private giasDialogService: GiasDialogService
  ) { }

  addGroup(): void {
    this.groupDialogLoading = true;
    this.service
      .salvaGruppoNote(this.groupDialog.NotaGruppo_Des)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.GruppoNoteAggiuntoCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioGruppoNote'), true);
        }

        this.groupDialogLoading = false;
        this.groupDialog = null;
        this.reloadGroupsSubject.next(null);
      });
  }

  editGroupName(): void {
    this.groupDialogLoading = true;
    this.service
      .aggiornaGruppoNote(this.groupDialog)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.gruppoNotaSubject.next(this.groupDialog);
          this.showMessage(this.transloco.translate('profImprese.GruppoNoteAggiornatoCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioGruppoNote'), true);
        }

        this.groupDialogLoading = false;
        this.groupDialog = null;
        this.reloadGroupsSubject.next(null);
      });
  }

  deleteGroup(dataItem: GruppoNota, note: NotaXGruppo[], utilizzi: NoteIntervento[]): void {
    if (note.length > 0) {
      this.showMessage(this.transloco.translate('profImprese.NonPuoiEliminareQuestoGruppoPercheHaAncoraDelleNoteAssociate'), false);
      return;
    }

    if (utilizzi.filter(x => x['Associato']).length > 0) {
      this.showMessage(this.transloco.translate('profImprese.NonPuoiEliminareQuestoGruppoPercheHaAncoraDegliUtilizziAssociati'), false);
      return;
    }

    this.giasDialogService.dialogMessageObs_Result(this.transloco.translate('SeiSicuroDiEliminareQuestoElemento'), '')
      .pipe(tap(res => {
        if (res['returnObj']) {
          this.doDelete(dataItem.NotaGruppo_Cod);
        }
      })).subscribe();
  }

  addNota(gruppoNotaCod: GruppoNota): void {
    this.service
      .aggiungiNota(this.notaDialog, gruppoNotaCod.NotaGruppo_Cod)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.NoteAggiuntaCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioNota'), true);
        }

        this.notaDialog = null;
        this.reloadNoteSubject.next(null);
      });
  }

  deleteNota(dataItem: NotaXGruppo): void {
    this.giasDialogService.dialogMessageObs_Result(this.transloco.translate('SeiSicuroDiEliminareQuestoElemento'), '')
      .pipe(tap(res => {
        if (res['returnObj']) {
          this.doDeleteNota(dataItem.Nota_Cod);
        }
      })).subscribe();
  }

  save(gruppo: GruppoNota, utilizzi: NoteIntervento[], note: NotaXGruppo[]): void {
    const realUsage = utilizzi.filter(x => x['Associato']).map(x => x.NotaUtilizzo_Cod);
    const notes = note.map(x => ({ NotaCod: x.Nota_Cod, Visible: x.Visibile }));
    this.service
      .salvaGruppoNoteCompleto({ NotaGruppoCod: gruppo.NotaGruppo_Cod, Visible: gruppo.visibile, NoteUtilizzoCod: realUsage, NoteVisibili: notes })
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.GruppoAggiornatoCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioGruppo'), true);
        }

        this.reloadGroupsSubject.next(null);
        this.gruppoNotaSubject.next(gruppo);
      });
  }

  private doDelete(notaGruppoCod: number): void {
    this.service
      .cancellaGruppoNote(notaGruppoCod)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.GruppoNoteEliminatoCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioGruppoNote'), true);
        }

        this.reloadGroupsSubject.next(null);
        this.gruppoNotaSubject.next(null);
      });
  }

  private doDeleteNota(notaCod: number): void {
    this.service
      .cancellaNota(notaCod)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        if (res) {
          this.showMessage(this.transloco.translate('profImprese.NotaEliminatoCorrettamente'), true);
        } else {
          this.showMessage(this.transloco.translate('profImprese.ErroreDiSalvataggioNota'), true);
        }

        this.reloadNoteSubject.next(null);
      });
  }

  private showMessage(message: string, success: boolean): void {
    if (success) {
      this.giasMessageService.successMessage(message);
      return;
    }

    this.giasMessageService.errorMessage(message);
  }

  private compactNotes(notes: NotaXGruppo[]): NotaXGruppo[] {
    const result: NotaXGruppo[] = [];
    for (const row of notes) {
      const notaCod = row.Nota_Cod;
      if (result.find(x => x.Nota_Cod == notaCod) != null) {
        continue;
      }

      row.NotaUtilizzo_Des = notes.filter(x => x.Nota_Cod == notaCod).map(x => x.NotaUtilizzo_Des).join(" - ")
      result.push(row);
    }

    return result;
  }

  private mapGruppiUtilizzo(gruppiUtilizzo: NotaXGruppo[], utilizzi: NoteIntervento[]): NoteIntervento[] {
    const result: NoteIntervento[] = [];
    for (const utilizzo of utilizzi) {
      utilizzo['Associato'] = gruppiUtilizzo.filter(x => x.NotaUtilizzo_Cod == utilizzo.NotaUtilizzo_Cod).length > 0;
      result.push(utilizzo);
    }

    return result;
  }
}
