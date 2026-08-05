import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DatiGeneraliGuard } from "app/Guard/datiGenerali-guard.service";
import { PermessiUtenteGuard } from "app/Guard/permessi-utente.guard.service";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { AnalisiTerrenoComponent } from "./analisi-terreno.component";
import { AnalisiTerrenoEditComponent } from "./analisi-terreno-edit/analisi-terreno-edit.component";
import { CurrentPageGuard } from "app/Guard/current-page-guard.service";

const routes: Routes = [
    {
        path: 'AnalisiTerreno',
        component: AnalisiTerrenoComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Gest_Analisi_AccessoMenu],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Analisi_Terreno_Menu
        },
        runGuardsAndResolvers: 'always'
    },
    {
        path: 'AnalisiTerreno/edit',
        component: AnalisiTerrenoEditComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Gest_Analisi_AccessoMenu],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AnalisiTerrenoRoutingModule { }