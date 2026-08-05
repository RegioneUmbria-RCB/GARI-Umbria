import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {MenuProfilazioneComponent} from './menu-profilazione.component';
import {ProfiliPermessiComponent} from './profili-permessi/profili-permessi.component';
import {
  MenuImpostazioniAziendeCentriComponent
} from './impostazioni-utente/menu-impostazioni-aziende-centri/menu-impostazioni-aziende-centri.component';
import {ImpostazioniUtenteComponent} from './impostazioni-utente/impostazioni-utente/impostazioni-utente.component';
import {MenuUtentiComponent} from "./gestione-utenti/utenti/menu-utenti.component";
import {MenuVisibilitaComponent} from './menu-visibilita/menu-visibilita.component';
import {GruppiUtentiComponent} from "./gruppi-utenti/gruppi-utenti.component";
import {ClientePermessiComponent} from './cliente-permessi/cliente-permessi.component';
import {DatiGeneraliGuard} from "../Guard/datiGenerali-guard.service";
import {PermessiUtenteGuard} from "../Guard/permessi-utente.guard.service";
import {CurrentPageGuard} from "../Guard/current-page-guard.service";
import {TranslocoLoadedGuard} from "../Guard/transloco-guard.service";
import {ModuliAttiviGuard} from "../Guard/moduli-attivi-guard.service";
import {enum_PagineGiasNG, enum_Security_Attivita} from "../Model/TipiEnumerativi";
import {ImportUtentiComponent} from "./import-utenti/import-utenti.component";

const routes: Routes = [
  {
    path: '',
    component: MenuProfilazioneComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Profilazione_NG],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Profilazione
    },
    children: [
      { path: '', redirectTo: 'Utenti', pathMatch: 'full' },
      {
        path: 'Utenti',
        component: MenuUtentiComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Profilazione_NG],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Profili-Permessi',
        component: ProfiliPermessiComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Gest_UtentiProfili],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Gruppi',
        component: GruppiUtentiComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Gest_UtentiGruppi],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Impostazioni-Utente',
        component: ImpostazioniUtenteComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Gest_UtentiImpostazioni],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Impostazioni-Aziende-Centri',
        component: MenuImpostazioniAziendeCentriComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.ImpostazioniImprese_NG],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Visibilita',
        component: MenuVisibilitaComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Profilazione_NG],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      },
      {
        path: 'Cliente-Permessi',
        component: ClientePermessiComponent,
        canActivate: [PermessiUtenteGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Profilazione_NG],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Menu_Profilazione
        }
      }
    ]
  },
  {
    path: 'Importazione-Utenti',
    component: ImportUtentiComponent,
    canActivate: [PermessiUtenteGuard],
    data: {
      readPermissions: [enum_Security_Attivita.ManutenzioneArchivi_Import_Utenti_Da_Excel],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Profilazione
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfilazioneRoutingModule {
}
