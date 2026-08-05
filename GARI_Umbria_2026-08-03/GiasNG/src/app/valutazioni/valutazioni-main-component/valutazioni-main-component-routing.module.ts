import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PianoContiComponent } from '../piano-conti/piano-conti.component';


const routes: Routes = [
    {
        path: 'Valutazioni', loadChildren: () => import('../valutazioni/valutazioni.module').then(m => m.ValutazioniModule),
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Valutazioni_Rischio],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Valutazioni
        }
    },
    {
        path: 'PianoConti', loadChildren: () => import('../piano-conti/piano-conti.module').then(m => m.PianoContiModule),
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Valutazioni_Rischio],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_PianoConti
        }
    }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ValutazioniMainRoutingModule { }
