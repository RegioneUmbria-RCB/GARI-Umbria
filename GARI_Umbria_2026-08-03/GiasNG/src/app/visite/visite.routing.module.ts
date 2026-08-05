import { RouterModule, Routes } from "@angular/router";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { NgModule } from "@angular/core";
import { VisiteComponent } from "./visite.component";
import { ImpresaRichiestaGuard } from "app/Guard/impresaRichiesta-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import {QuadernoDiCampagnaComponent} from "../quaderno-di-campagna/agenda-edit/quaderno-di-campagna.component";
import {CurrentPageGuard} from "../Guard/current-page-guard.service";
import {QdCLoadedGuard} from "../menu-agenda/guards/transloco-qdc-guard.service";

const routes: Routes = [
    {
        path: 'MenuVisite',
        component: VisiteComponent,
        canActivate: [DatiGeneraliGuard, ImpresaRichiestaGuard, PermessiUtenteGuard],
        data: {
            impresaRichiestaGuardData: {
                sitoRedirect: Enum_SiteRedirector.GiasNG,
                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
            },
            readPermissions: [enum_Security_Attivita.Visite_Lista_NG],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Menu_Visite
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VisiteRoutingModule { }
