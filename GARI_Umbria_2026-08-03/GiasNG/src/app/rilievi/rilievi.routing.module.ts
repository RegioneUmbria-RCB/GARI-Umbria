import { RouterModule, Routes } from "@angular/router";
import { MenuRilieviComponent } from "./rilievi.component";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { ImpresaRichiestaGuard } from "app/Guard/impresaRichiesta-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
        path: 'MenuRilievi',
        component: MenuRilieviComponent,
        canActivate: [DatiGeneraliGuard, ImpresaRichiestaGuard, PermessiUtenteGuard],
        data: {
            impresaRichiestaGuardData: {
                sitoRedirect: Enum_SiteRedirector.GiasNG,
                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
            },
            readPermissions: [enum_Security_Attivita.MenuRilievi],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Menu_Rilievi
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RilieviRoutingModule { }