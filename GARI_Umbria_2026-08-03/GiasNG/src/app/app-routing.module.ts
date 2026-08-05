import {NgModule} from '@angular/core';
import {PreloadAllModules, RouterModule, Routes} from '@angular/router';
import {AccessoNegatoComponent} from './accesso-negato/accesso-negato.component';
import {CounterComponent} from './counter/counter.component';
import {GestioneRichiesteComponent} from './gestione-richieste/gestione-richieste.component';
import {CurrentPageGuard} from './Guard/current-page-guard.service';
import {DatiGeneraliGuard} from './Guard/datiGenerali-guard.service';
import {ModuliAttiviGuard} from './Guard/moduli-attivi-guard.service';
import {PermessiUtenteGuard} from './Guard/permessi-utente.guard.service';
import {TranslocoLoadedGuard} from './Guard/transloco-guard.service';
import {LoginComponent} from './login/login.component';
import {enum_PagineGiasNG, enum_Security_Attivita} from './Model/TipiEnumerativi';
import {PageNotFoundComponent} from './page-not-found/page-not-found.component';
import {PreferitiConfigComponent} from './preferiti-config/preferiti-config.component';
import {TestComponent} from './test/test.component';
import {PROFILAZIONE_IMPRESE_BASE_URL} from './profilazione-imprese/profilazione-imprese-routing.module';
import { CacCodificheComponent } from 'app/cac-codifiche/cac-codifiche.component';
import {ScadenzaReinnescoTrappoleModule} from "./scadenza-reinnesco-trappole/scadenza-reinnesco-trappole.module";

export const DASHBOARD_URL = "dashboard";
export const FAVORITES_URL = "preferiti-config";
export const MENU_ZOO_URL = "operazioni-zootecniche";
export const MENU_QUALITA_TRACCIABILITA_URL = "qualita-tracciabilita";
export const PESATE_ACCRESCIMENTO_URL = "pesate-accrescimento";

const routes: Routes = [
  {path: '', component: PageNotFoundComponent},
  {path: 'counter', component: CounterComponent},
  {
    path: 'CodificheSistemiEsterni', component: CacCodificheComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.ManutenzioneArchivi_GestioneSistemi_Esterni],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Codifiche_Sistemi_Esterni
    }
  },
  {
    path: DASHBOARD_URL, loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule),
    canActivate: [DatiGeneraliGuard, CurrentPageGuard, TranslocoLoadedGuard],
    data: {page: enum_PagineGiasNG.Pagina_Dashboard}
  },
  {path: 'preferiti-config', component: PreferitiConfigComponent, canActivate: [DatiGeneraliGuard]},
  {
    path: 'Test',
    component: TestComponent,
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard]
  },
  {path: 'AccessoNegato', component: AccessoNegatoComponent, pathMatch: 'full'},
  {
    path: 'Anagrafica',
    loadChildren: () => import('./anagrafica/anagrafica.module').then(m => m.AnagraficaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Anagrafica
    }
  },
  {
    path: 'Budget', loadChildren: () => import('./budget/budget.module').then(m => m.BudgetModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Anagrafica
    }
  },
  {
    path: 'QdC', loadChildren: () => import('./quaderno-di-campagna/quaderno-di-campagna.module').then(m => m.QuadernoDiCampagnaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Edit_Attivita
    }
  },
  {
    path: 'Visite', loadChildren: () => import('./visite/visite.module').then(m => m.VisiteModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Visite
    }
  },
  {
    path: 'QdC/ConfigurazioneOperazioniCulturali',
    loadChildren: () => import('./quaderno-di-campagna/quaderno-di-campagna.module').then(m => m.QuadernoDiCampagnaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.AgronicaManutenzione_MisuraAvversita],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Configurazione_Operazioni_Culturali
    }
  },
  {
    path: 'Profilazione',
    loadChildren: () => import('./profilazione/profilazione.module').then(m => m.ProfilazioneModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Profilazione
    }
  },
  {
    path: MENU_ZOO_URL, loadChildren: () => import('./zoo/zoo.module').then(m => m.ZooModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Zoo
    }
  },
   {
    path: MENU_QUALITA_TRACCIABILITA_URL, loadChildren: () => import('./qualita-tracciabilita/qualita-tracciabilita.module').then(m => m.QualitaTracciabilitaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.VerificaConformitaNG],
      writePermissions: [],
      page: enum_PagineGiasNG.GestioneDisciplinari_Verifica_Disciplinare
    }
  },
  {
    path: PROFILAZIONE_IMPRESE_BASE_URL,
    loadChildren: () => import('./profilazione-imprese/profilazione-imprese.module').then(m => m.ProfilazioneImpreseModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.ProfilazioneImprese_NG],
      writePermissions: [enum_Security_Attivita.ProfilazioneImprese_NG],
      page: enum_PagineGiasNG.Pagina_Profilazione_Imprese
    }
  },
  {
    path: 'Amministrazione-Sistema',
    loadChildren: () => import('./amministrazione-sistema/amministrazione-sistema.module').then(m => m.AmministrazioneSistemaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.GiasAPP_ConsultaSincroDatiAppDaWeb],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_AmministrazioneSistema_ConsultaSincroDatiApp
    }
  },
  {
    path: 'GruppiMerce',
    loadChildren: () => import('./gruppi-merce/gruppi-merce.module').then(m => m.GruppiMerceModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Gruppi_Merce],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Gruppi_Merce_Anagrafica
    }
  },
  {
    path: 'GIS',
    loadChildren: () => import('./GIS/GIS.module').then(m => m.GISModule),
    // DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_GIS
    }
  },
  {
    path: './GIS/cfg-proiezioni', loadChildren: () => import('./GIS/GIS.module').then(m => m.GISModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.GIS_Configurazione_Algoritmi_Cartografici],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_GIS_cfg_proiezioni
    }
  },
  {
    path: 'Valutazioni-Main',
    loadChildren: () => import('./valutazioni/valutazioni-main-component/valutazioni-main-component.module').then(m => m.ValutazioniMainComponentModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Valutazioni_Rischio],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Valutazioni
    }
  },
  {path: 'Login', component: LoginComponent},
  {
    path: 'gruppi-raccolta',
    loadChildren: () => import('./gruppi-raccolta/gruppi-raccolta/gruppi-raccolta.module').then(m => m.GruppiRaccoltaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Gruppi_Raccolta],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Gruppi_Raccolta
    }
  },
  {
    path: 'requisiti-stabilimento',
    loadChildren: () => import('./requisiti-stabilimento/requisisti-stabilimento.module').then(m => m.RequisistiStabilimentoModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Requisiti_Stabilimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Requisiti_Stabilimento
    }
  },
  {
    path: 'dati-previsionali-colture',
    loadChildren: () => import('./dati-previsionali-colture/dati-previsionali-colture.module').then(m => m.DatiPrevisionaliColtureModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Dati_Previsionali_Colture],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Dati_Previsionali_Colture
    }
  },
  {
    path: 'domanda-irrigua',
    loadChildren: () => import('./domanda-irrigua/domanda-irrigua.module').then(m => m.DomandaIrriguaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Domanda_Irrigua
    }
  },
  {
    path: 'confronto-piano-colturale',
    loadChildren: () => import('./confronto-pc/confronto-piano-colturale/confronto-piano-colturale.module').then(m => m.ConfrontoPianoColturaleModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Confronto_Piano_Colturale],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Confronto_Piano_Colturale
    }
  },
  {
    path: 'Export/ExportQdCToAgea',
    loadChildren: () => import('./export/export-qdc-to-agea/export-qdc-to-agea.module').then(m => m.ExportQdCToAgeaModule),
    canActivate: [DatiGeneraliGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Esportazione_Agea_NG],
      writePermissions: [enum_Security_Attivita.Esportazione_Agea_NG],
      page: enum_PagineGiasNG.Pagina_Export_QdC_To_Agea
    }
  },
  {
    path: 'FiltroDiRicerca',
    loadChildren: () => import('./filtro-ricerca/filtro-ricerca.module').then(m => m.FiltroRicercaModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Filtro_Ricerca
    }
  },
  {
    path: 'MenuAnalisiTerreno',
    loadChildren: () => import('./analisi-terreno/analisi-terreno.module').then(m => m.AnalisiTerrenoModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Analisi_Terreno_Menu
    }
  },
  {
    path: 'ReportImpiegoProdottiFitosanitari',
    loadChildren: () => import('./report-impiego-prodotti-fitosanitari/report-impiego-prodotti-fitosanitari.module').then(m => m.ReportImpiegoProdottiFitosanitariModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Report_Impiego_Prodotti_Fitosanitari
    }
  },
  {
    path: 'Rilievi', loadChildren: () => import('./rilievi/rilievi.module').then(m => m.RilieviModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Menu_Rilievi
    }
  },
  {
    path: 'TrattamentoZoo',  loadChildren: () => import('./zoo/components/trattamento-zoo/trattamento-zoo.module').then(m => m.TrattamentoZooModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Trattamento_Zoo
    }
  },
  { path: 'GestioneRichieste', component: GestioneRichiesteComponent, canActivate: [TranslocoLoadedGuard] },
  {
  path: 'ReportAbilitazionePdC', loadChildren: () => import('./report-abilitazione-pdc/report-abilitazione-pdc.module').then(m => m.ReportAbilitazionePdCModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Report_Abilitazione_PdC],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Report_Abilitazione_PdC
    }
  },
  {
    path: 'TerapiaZoo',  loadChildren: () => import('./zoo/components/terapia-zoo/terapia-zoo.module').then(m => m.TerapiaZooModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Terapia_Zoo
    }
  },
  {
    path: PESATE_ACCRESCIMENTO_URL,
    loadChildren: () => import('./zoo/pages/pesate-accrescimento/pesate-accrescimento.module').then(m => m.PesateAccrescimentoModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.ZooPesatureAccrescimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_PesateAccrescimento
    }
  },
  {
    path: 'SostenibitaCO2',
    loadChildren: () => import('./sostenibilita-co2/sostenibilita-co2.module').then(m => m.SostenibitaCO2Module),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_SostenibitaCO2_SelezionePerimetro
    }
  },
  {
    path: 'rischi-meteo',
    loadChildren: () => import('./rischi-meteo/rischi-meteo.module').then(m => m.RischiMeteoModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.RischiMeteo_CalcoloRischi],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_RischiMeteo_CalcoloRischi
    }
  },
  {
    path: 'RischiH20',
    loadChildren: () => import('./rischi-h20/rischi-h20.module').then(m => m.RischiH20Module),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.RischiH2O_CalcoloRischiH2O],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_RischiH20_SelezionePerimetro
    }
  },
  {
    path: 'PesateAccrescimento',
    loadChildren: () => import('./zoo/pages/pesate-accrescimento/pesate-accrescimento.module').then(m => m.PesateAccrescimentoModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.ZooPesatureAccrescimento],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_PesateAccrescimento
    }
  },
  {
    path: 'ScadenzaReinnescoTrappole',
    loadChildren: () => import('./scadenza-reinnesco-trappole/scadenza-reinnesco-trappole.module').then(m => m.ScadenzaReinnescoTrappoleModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.Agenda_AccessoMenu_NG],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_Scadenza_Reinnesco_Trappole
    }
  },
  {
    path: 'DSSNutrizione',
    loadChildren: () => import('./dss-nutrizione/dss-nutrizione.module').then(m => m.DssNutrizioneModule),
    canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, TranslocoLoadedGuard, ModuliAttiviGuard],
    data: {
      readPermissions: [enum_Security_Attivita.DSS_Nutrizione],
      writePermissions: [],
      page: enum_PagineGiasNG.Pagina_DSS_Nutrizione
    }
  },
  {
    path: '**',
    //redirectTo: '',
    component: PageNotFoundComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    preloadingStrategy: PreloadAllModules, anchorScrolling: 'enabled'
  })],
  exports: [RouterModule]
})

export class AppRoutingModule {
}
