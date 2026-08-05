import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";
import { ReportImpiegoProdottiFitosanitariComponent } from "./report-impiego-prodotti-fitosanitari.component";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";

const routes: Routes = [
    {
        path: '',
        component: ReportImpiegoProdottiFitosanitariComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard],
        data: {
            readPermissions: [],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Report_Impiego_Prodotti_Fitosanitari
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReportImpiegoProdottiFitosanitariRoutingModule { }