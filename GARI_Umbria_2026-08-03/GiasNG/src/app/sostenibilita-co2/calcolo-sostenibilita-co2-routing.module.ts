import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SelezionePerimetroComponent } from './pages/selezione-perimetro-co2/selezione-perimetro-co2.component';
import { GestioneCO2PageComponent } from './pages/gestione-co2/gestione-co2.component';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';

const routes: Routes = [
  {
    path: 'SelezionePerimetro',
    component: SelezionePerimetroComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.SostenibitaCO2_CalcoloSostenibitaCO2],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_SostenibitaCO2_SelezionePerimetro
    }
  },
  {
    path: 'GestioneCO2',
    component: GestioneCO2PageComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.SostenibitaCO2_CalcoloSostenibitaCO2],
      writePermissions: [],
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CalcoloSostenibilitaCO2RoutingModule {}
