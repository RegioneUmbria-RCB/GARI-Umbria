import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfilazioneImpreseComponent } from './profilazione-imprese.component';
import { ProfilazioneImpreseMacchineOperatoriComponent } from './pages/profilazione-imprese-macchine-operatori/profilazione-imprese-macchine-operatori.component';
import { ProfilazioneImpreseDefaultPianiColturaliComponent } from './pages/profilazione-imprese-default-piani-colturali/profilazione-imprese-default-piani-colturali.component';
import { ProfilazioneImpreseNoteComponent } from './pages/profilazione-imprese-note/profilazione-imprese-note.component';

export const PROFILAZIONE_IMPRESE_BASE_URL = 'profilazione-imprese';
export const PROFILAZIONE_IMPRESE_MACCHINE_OPERATORI_PER_LAVORAZIONI_URL = 'macchine-operatori-per-lavorazioni';
export const PROFILAZIONE_IMPRESE_NOTE_URL = 'note';
export const PROFILAZIONE_IMPRESE_DEFAULT_PIANI_COLTURALI_URL = 'default-piani-colturali';

const routes: Routes = [
  {
    path: '',
    component: ProfilazioneImpreseComponent,
    children: [
      {
        path: PROFILAZIONE_IMPRESE_MACCHINE_OPERATORI_PER_LAVORAZIONI_URL,
        component: ProfilazioneImpreseMacchineOperatoriComponent
      },
      {
        path: PROFILAZIONE_IMPRESE_NOTE_URL,
        component: ProfilazioneImpreseNoteComponent
      },
      {
        path: PROFILAZIONE_IMPRESE_DEFAULT_PIANI_COLTURALI_URL,
        component: ProfilazioneImpreseDefaultPianiColturaliComponent
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: PROFILAZIONE_IMPRESE_MACCHINE_OPERATORI_PER_LAVORAZIONI_URL
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfilazioneImpreseRoutingModule { }
