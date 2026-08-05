import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ValutazioniComponent } from './valutazioni.component';
import { MainValutazioniEditComponent } from './main-valutazioni-edit/main-valutazioni-edit.component';
import { ValutazioniMainRoutingModule } from '../valutazioni-main-component/valutazioni-main-component-routing.module';
import { ValutazioniMainComponentComponent } from '../valutazioni-main-component/valutazioni-main-component.component';


const routes: Routes = [
    {
        path: '',
        component: ValutazioniMainComponentComponent,
        children: [
            {
                path: 'Valutazioni',
                component: ValutazioniComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Valutazioni_Rischio],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Valutazioni
                }
            }]
    },
    {
        path: 'Valutazioni/Valutazioni-Edit',
        component: MainValutazioniEditComponent,
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
            readPermissions: [
                enum_Security_Attivita.Valutazioni_Rischio
            ],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Valutazioni_Edit
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ValutazioniRoutingModule { }
