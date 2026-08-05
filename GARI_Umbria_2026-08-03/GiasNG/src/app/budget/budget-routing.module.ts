import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { BudgetComponent } from './budget.component';
import { BudgetLoadedGuard } from './guards/transloco-budget-guard.service';

const routes: Routes = [
    {
        path: '',
        component: BudgetComponent,
        children: [
           {
                path: 'Anagrafica', loadChildren: () => import('./../anagrafica/anagrafica.module').then(m => m.AnagraficaModule),
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, BudgetLoadedGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica
                }
            },
        ]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class BudgetRoutingModule { }
