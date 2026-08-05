import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {GruppiRaccoltaComponent} from './gruppi-raccolta.component';
import {DatiGeneraliGuard} from '../../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../../Model/TipiEnumerativi';

const routes: Routes = [
    {   path: '',
        component: GruppiRaccoltaComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
            readPermissions: [enum_Security_Attivita.Gruppi_Raccolta],
            writePermissions: [],
            page: enum_PagineGiasNG.Pagina_Gruppi_Raccolta
        }
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GruppiRaccoltaRoutingModule { }
