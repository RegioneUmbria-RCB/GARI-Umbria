import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { FiltroRicercaComponent } from "./filtro-ricerca.component";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";
import { PendingChangesGuard } from "./griglia-filtro-ricerca/service/pendingChanges-guard.service";

const routes: Routes = [
    {
        path: 'Filtrone',
        component: FiltroRicercaComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard],
        canDeactivate: [PendingChangesGuard],
        data: {
            readPermissions: [],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Filtro_Ricerca
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FiltroRicercaRoutingModule { }