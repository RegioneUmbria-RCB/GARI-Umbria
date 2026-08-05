import { Component } from '@angular/core';
import { PROFILAZIONE_IMPRESE_MACCHINE_OPERATORI_PER_LAVORAZIONI_URL, PROFILAZIONE_IMPRESE_NOTE_URL, PROFILAZIONE_IMPRESE_DEFAULT_PIANI_COLTURALI_URL, PROFILAZIONE_IMPRESE_BASE_URL } from './profilazione-imprese-routing.module';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese',
  templateUrl: './profilazione-imprese.component.html',
  styleUrls: ['./profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseComponent {

  tabs = [
    {
      title: 'profImprese.MacchineOperatoriPerLavoazioni',
      path: PROFILAZIONE_IMPRESE_MACCHINE_OPERATORI_PER_LAVORAZIONI_URL
    },
    {
      title: 'profImprese.Note',
      path: PROFILAZIONE_IMPRESE_NOTE_URL
    },
    {
      title: 'profImprese.DefaultPianiColturali',
      path: PROFILAZIONE_IMPRESE_DEFAULT_PIANI_COLTURALI_URL
    },
  ];

  selected = this.tabs.findIndex(x => document.location.href.includes(`${PROFILAZIONE_IMPRESE_BASE_URL}/${x.path}`))

  constructor(private router: Router) { }

  onTabSelect(e: SelectEvent): void {
    const path = this.tabs[e.index].path;
    this.router.navigate([PROFILAZIONE_IMPRESE_BASE_URL, path]);
  }
}
