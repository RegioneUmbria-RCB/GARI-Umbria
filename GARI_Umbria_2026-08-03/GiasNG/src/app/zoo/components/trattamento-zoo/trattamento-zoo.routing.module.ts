import { RouterModule, Routes } from "@angular/router";
import { TrattamentoZooComponent } from "./trattamento-zoo.component";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { ImpresaRichiestaGuard } from "app/Guard/impresaRichiesta-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { NgModule } from "@angular/core";

const routes: Routes = [
    {
        path: '',
        component: TrattamentoZooComponent,
        canActivate: [DatiGeneraliGuard, ImpresaRichiestaGuard, PermessiUtenteGuard],
        data: {
            impresaRichiestaGuardData: {
                sitoRedirect: Enum_SiteRedirector.GiasNG,
                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
            },
            readPermissions: [enum_Security_Attivita.TrattamentoZoo],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Trattamento_Zoo
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class TrattamentoZooRoutingModule { }