import {Component, DoCheck, Inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridNoteHttpService } from '../../service/grid-note/grid-note-http.service';
import { NoteService } from '../../service/grid-note/note.service';
import { faGear } from '@fortawesome/free-solid-svg-icons';
import {QdCService} from "../../service/qdc.service";
import {GridPublicService} from 'gias-kendo-grid';
import {GRID_HTTP_TOKEN} from 'gias-kendo-grid';
import {GridImpiantiHttpService} from "../../service/grid-impianti/grid-impianti-http.service";

@Component({
  standalone: false,
  selector: 'app-grid-note',
  templateUrl: './grid-note.component.html',
  styleUrls: ['./grid-note.component.css'],
  providers: [
    ...generateGridProviders(GridNoteHttpService, GridNoteComponent)
  ]
})
export class GridNoteComponent implements OnInit, DoCheck, OnDestroy {

  @ViewChild('gridNote') gridNote;

  public isEmpty = false;
  public showNoteManager = false;

  faGear = faGear;

  constructor(private noteService: NoteService,
              private qdcservice: QdCService,
              private gridpublicService: GridPublicService,
              @Inject(GRID_HTTP_TOKEN) private gridhttpService: GridImpiantiHttpService) { }

  ngOnInit(): void {
    this.qdcservice.GridNoteHttpService = this.gridhttpService;
    this.qdcservice.GridNotePublicService = this.gridpublicService;
    this.noteService.foundGroups.GiasSubscribe(gruppi => {
      if (!gruppi) return;
      if (!gruppi.length) this.isEmpty = true;
    });
  }

  ngOnDestroy(): void {
    this.noteService.lastLeggiNote = null;
  }

  ngDoCheck(): void {
    //// Riordina colonne
    if (!this.gridNote) return;
    let columns = this.gridNote.grid.columns.toArray();
    if (!columns.length) return;

    let cmdCol = columns.find(c => c.title === '');
    if (cmdCol) {
        this.gridNote.grid.reorderColumn(cmdCol, 2);
        cmdCol.width = 50;
    }
  }

  editDefault(column) {
    console.log(column);
    this.showNoteManager = true;
  }

}
