import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CalcoloRischiPageComponent } from './pages/calcolo-rischi/calcolo-rischi-page.component';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';

const routes: Routes = [
  {
    path: 'calcolo-rischi',
    component: CalcoloRischiPageComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.RischiMeteo_CalcoloRischi],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_RischiMeteo_CalcoloRischi
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RischiMeteoRoutingModule {}
