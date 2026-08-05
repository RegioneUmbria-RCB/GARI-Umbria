import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SelezionePerimetroH20Component } from './pages/selezione-perimetro-h20/selezione-perimetro-h20.component';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';

const routes: Routes = [
  {
    path: 'SelezionePerimetro',
    component: SelezionePerimetroH20Component,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.RischiH2O_CalcoloRischiH2O],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_RischiH20_SelezionePerimetro
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RischiH20RoutingModule {}
