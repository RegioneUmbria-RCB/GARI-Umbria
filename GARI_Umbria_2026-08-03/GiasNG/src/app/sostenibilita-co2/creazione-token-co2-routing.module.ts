import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TokenCreationCo2Component } from './pages/creazione-token-co2/token-creation-co2.component';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';

const routes: Routes = [
  {
    path: 'CreazioneToken',
    component: TokenCreationCo2Component,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.SostenibitaCO2_CreazioneToken],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_SostenibitaCO2_CreazioneToken
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CreazioneTokenCo2RoutingModule {}
