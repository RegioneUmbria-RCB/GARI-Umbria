import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DatiGeneraliGuard} from '../Guard/datiGenerali-guard.service';
import {PermessiUtenteGuard} from '../Guard/permessi-utente.guard.service';
import {CurrentPageGuard} from '../Guard/current-page-guard.service';
import {ModuliAttiviGuard} from '../Guard/moduli-attivi-guard.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from '../Model/TipiEnumerativi';
import {RequisitiStabilimentoComponent} from './requisiti-stabilimento.component';
import {ImpreseComponent} from '../anagrafica/imprese/imprese.component';
import {RequisitiStabilimentoGridComponent} from './requisiti-stabilimento-grid/requisiti-stabilimento-grid.component';
import {
  RequisitiStabilimentoContrattiGridComponent
} from './requisiti-stabilimento-contratti-grid/requisiti-stabilimento-contratti-grid.component';
import {VisualizzaDettagliComponent} from "./visualizza-dettagli/visualizza-dettagli.component";
import {VDPianoColturaleComponent} from "./visualizza-dettagli/v-d-piano-colturale/v-d-piano-colturale.component";
import {VDContrattiComponent} from "./visualizza-dettagli/v-d-contratti/v-d-contratti.component";

const routes: Routes = [
  {
    path: '',
    component: RequisitiStabilimentoComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento
    },
    children: [
      {
        path: 'piano-colturale',
        component: RequisitiStabilimentoGridComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_PianoColturale
        },
      },
      {
        path: 'contratti',
        component: RequisitiStabilimentoContrattiGridComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_Contratti
        },
      }
    ]
  },
  {
    path: 'VisualizzaDettagli',
    component: VisualizzaDettagliComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento
    },
    children: [
      {
        path: 'piano-colturale',
        component: VDPianoColturaleComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_VD_PianoColturale
        },
      },
      {
        path: 'contratti',
        component: VDContrattiComponent,
        canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
        data: {
          readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
          writePermissions: [],
          page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_VD_Contratti
        },
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequisistiStabilimentoRoutingModule { }
