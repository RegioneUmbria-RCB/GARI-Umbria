import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { LeggiNote, NoteService } from 'app/quaderno-di-campagna/agenda-edit/service/grid-note/note.service';
import { GruppoNoteModel, NotaInterventoDdlItem } from '../note.model';
import {Observable, take, tap} from "rxjs";

@Component({
  standalone: false,
  selector: 'app-gestione-note',
  templateUrl: './gestione-note.component.html',
  styleUrls: ['./gestione-note.component.css']
})
export class GestioneNoteComponent implements OnInit {

  public filter = "";
  public listaGruppiNote: Array<GruppoNoteModel> = [];
  public listaNote: Array<any> = [];

  public form: FormGroup = new FormGroup({
    canCustomize: new FormControl(true)
  });

  private readonly _leggiNote: LeggiNote;
  /** Contiene tutte le note di tutti i gruppi. */
  private _listaNote: Array<any> = [];
  private _defaults: Array<number> = [];

  constructor(private noteService: NoteService) {
    this._leggiNote =  this.noteService.getLeggiNote();
  }

  ngOnInit() {
    this.noteService.leggiGruppi(this._leggiNote)
      .pipe(
        take(1),
        tap(gr => {
          gr.forEach(g => g['Selected'] = g.Defaults.length > 0);
          this._defaults = gr.flatMap(g => g.Defaults).map(d => d.codice);
        }),
      ).subscribe(groups => this.listaGruppiNote = groups);
  }

  public onSelectGroup(gruppo: GruppoNoteModel) {
    this.listaNote = this._listaNote.filter(n =>
      gruppo.Gruppo_Cod.includes(n.NotaGruppo_Cod
    ));
    if (!this.listaNote.length)
      this.loadNotes(gruppo).subscribe(() => this.selectIntoGroups(gruppo));
    else
      this.selectIntoGroups(gruppo);
  }

  setDefault(nota: NotaInterventoDdlItem) {
    nota['Selected'] = !nota['Selected'];
    const idx = this._defaults.findIndex(v => v === nota.id);
    const group = this.listaGruppiNote.find(x => x.Gruppo_Cod.includes(nota.NotaGruppo_Cod));
    if (idx > -1) {
      this._defaults.splice(idx, 1);
      group.Defaults.splice(group.Defaults.findIndex(x => x.codice === nota.id), 1);
    } else  {
      this._defaults.push(nota.id);
      group.Defaults.push({ codice: nota.id, descrizione: nota.descrizione });
    }

    group['Selected'] = group.Defaults.length > 0
  }

  undoAction() {
    this._listaNote = [];
    this.listaNote.forEach((note: NotaInterventoDdlItem) => {
      let group = this.listaGruppiNote.find(gr => gr.Gruppo_Cod.includes(note.NotaGruppo_Cod));
      if (!group) return;
      let defaults = group.Defaults.map(d => d.codice);
      note['Selected'] = defaults.includes(note.id);
    });
  }

  execute() {
    this.noteService.saveDefaults(this._defaults);
  }

  private loadNotes(gruppo: GruppoNoteModel): Observable<any> {
    return this.noteService.leggiNote(gruppo, this._leggiNote).pipe(
      tap(notes => {
        this.listaNote = notes;
        this.listaNote.forEach(note => this._listaNote.push(note));
      })
    );
  }

  private selectIntoGroups(gruppo: GruppoNoteModel) {
    if (!gruppo.Defaults.length) return;
    gruppo.Defaults.map(d => d.codice).forEach(cod => {
      let nota = this.listaNote.find(n => n.id === cod);
      if (nota) nota['Selected'] = true;
    });
  }

}
