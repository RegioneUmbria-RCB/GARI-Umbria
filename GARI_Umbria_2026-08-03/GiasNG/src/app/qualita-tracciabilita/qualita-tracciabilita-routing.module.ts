import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DatiGeneraliGuard} from "../Guard/datiGenerali-guard.service";
import {PermessiUtenteGuard} from "../Guard/permessi-utente.guard.service";
import {CurrentPageGuard} from "../Guard/current-page-guard.service";
import {TranslocoLoadedGuard} from "../Guard/transloco-guard.service";
import {ModuliAttiviGuard} from "../Guard/moduli-attivi-guard.service";
import {enum_PagineGiasNG} from "../Model/TipiEnumerativi";
import {VerificaConformitaComponent} from "./pages/verifica-conformita/verifica-conformita.component";

const routes: Routes = [
  {
    path: 'verifica-disciplinare',
    component: VerificaConformitaComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.GestioneDisciplinari_Verifica_Disciplinare
    }
  },
  {
    path: '',
    component: VerificaConformitaComponent, // TODO: change?
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: 0
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QualitaTracciabilitaRoutingModule {
}