import { NgModule } from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DatiGeneraliGuard} from '../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../Model/TipiEnumerativi';
import {ConfrontoPianoColturaleComponent} from './confronto-piano-colturale.component';

const routes: Routes = [
  {
    path: '',
    component: ConfrontoPianoColturaleComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Confronto_Piano_Colturale],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Confronto_Piano_Colturale
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfrontoPianoColturaleRoutingModule { }
