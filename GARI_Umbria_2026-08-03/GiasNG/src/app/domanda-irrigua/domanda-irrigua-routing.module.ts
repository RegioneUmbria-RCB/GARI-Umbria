import {RouterModule, Routes} from "@angular/router";
import {DatiGeneraliGuard} from "../Guard/datiGenerali-guard.service";
import {PermessiUtenteGuard} from "../Guard/permessi-utente.guard.service";
import {CurrentPageGuard} from "../Guard/current-page-guard.service";
import {TranslocoLoadedGuard} from "../Guard/transloco-guard.service";
import {ModuliAttiviGuard} from "../Guard/moduli-attivi-guard.service";
import {enum_PagineGiasNG, enum_Security_Attivita} from "../Model/TipiEnumerativi";
import {NgModule} from "@angular/core";
import {DomandaIrriguaComponent} from "./domanda-irrigua.component";
import {LettureContatoriComponent} from "./pages/letture-contatori/letture-contatori.component";

const routes: Routes = [
  {
    path: '',
    component: DomandaIrriguaComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: { //TODO: aggiornare i permessi necessari
      readPermissions: [enum_Security_Attivita.DomandaIrrigua, enum_Security_Attivita.LettureContatoriAziendali_NG],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Domanda_Irrigua
    },
  },
  {
    path: 'lettura-contatori',
    component: LettureContatoriComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.LettureContatoriAziendali_NG],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Lettura_Contatori
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DomandaIrriguaRoutingModule { }
