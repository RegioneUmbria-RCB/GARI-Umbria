import { Component, Input, OnInit } from '@angular/core';
import { NotaProfilazione, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { BehaviorSubject, catchError, debounceTime, Observable, of, Subject, switchMap, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-note-list',
  templateUrl: './profilazione-imprese-note-list.component.html',
  styleUrls: ['./profilazione-imprese-note-list.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseNoteListComponent implements OnInit {

  @Input() campoApplicativoSubject: Subject<number> | null = null;
  @Input() noteSelectedSubject: BehaviorSubject<number[]> | null = null;

  noteLoading: boolean = false;
  note$: Observable<NotaProfilazione[]> | null = null;

  constructor(private service: ProfilazioneImpreseService) { }

  ngOnInit(): void {
    this.note$ = this.campoApplicativoSubject.pipe(
      debounceTime(100),
      tap(() => this.noteLoading = true),
      switchMap(x => this.service.leggiProfilazioneNote(x)),
      catchError(() => of([])),
      tap(() => this.noteLoading = false)
    );
  }

  selectNota(selected: boolean, nota: NotaProfilazione): void {
    const current = this.noteSelectedSubject.value;
    if (selected) {
      current.push(nota.Nota_Cod);
    } else {
      const index = current.indexOf(nota.Nota_Cod);
      if (index > -1) {
        current.splice(index, 1);
      }
    }

    this.noteSelectedSubject.next(current);
  }

  isNotaSelected(nota: NotaProfilazione): boolean {
    const result = this.noteSelectedSubject.value.find(x => nota.Nota_Cod == x) != null;
    return result;
  }
}
