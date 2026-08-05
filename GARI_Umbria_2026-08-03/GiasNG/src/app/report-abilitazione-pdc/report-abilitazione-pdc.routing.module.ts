import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { ReportAbilitazionePdCComponent } from "./report-abilitazione-pdc.component";

const routes: Routes = [
    {
        path: '',
        component: ReportAbilitazionePdCComponent,
        canActivate: [DatiGeneraliGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Report_Abilitazione_PdC],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Report_Abilitazione_PdC
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReportAbilitazionePdCRoutingModule { }