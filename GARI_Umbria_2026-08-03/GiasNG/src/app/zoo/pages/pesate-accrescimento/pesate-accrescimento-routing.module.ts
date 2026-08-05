import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ImpresaRichiestaGuard } from 'app/Guard/impresaRichiesta-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { PesateAccrescimentoComponent } from './pesate-accrescimento.component';

const routes: Routes = [
  {
    path: '',
    component: PesateAccrescimentoComponent,
    canActivate: [DatiGeneraliGuard, ImpresaRichiestaGuard, PermessiUtenteGuard, TranslocoLoadedGuard],
    data: {
      impresaRichiestaGuardData: {
        sitoRedirect: Enum_SiteRedirector.GiasNG,
        paginaRedirect: enum_PagineGiasNG.Pagina_Corrente,
      },
      readPermissions: [enum_Security_Attivita.ZooPesatureAccrescimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_PesateAccrescimento,
    },
    runGuardsAndResolvers: 'always',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PesateAccrescimentoRoutingModule {}
