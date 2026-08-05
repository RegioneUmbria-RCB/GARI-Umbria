import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { TranslocoLoadedGuard } from 'app/Guard/transloco-guard.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { AnagraficaComponent } from './anagrafica.component';
import { AppezzamentiComponent } from './appezzamenti/appezzamenti.component';
import { CampiComponent } from './campi/campi.component';
import { CatastoComponent } from './catasto/catasto.component';
import { CentriComponent } from './centri/centri.component';
import { CentriResolver } from './centri/services/centri-resolver.service';
import { ContattiComponent } from './contatti/contatti.component';
import { EserciziComponent } from './esercizi/esercizi.component';
import { FabbricatiComponent } from './fabbricati/fabbricati.component';
import { ImpiantiComponent } from './impianti/impianti.component';
import { ImpreseComponent } from './imprese/imprese.component';
import { MacchineComponent } from './macchine/macchine.component';

const routes: Routes = [
    {
        path: '',
        component: AnagraficaComponent,
        children: [
            {
                path: 'Imprese',
                component: ImpreseComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Imprese
                },
            },
            {
                path: 'Centri', component: CentriComponent,
                resolve: { tableData: CentriResolver },
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_CentroAziendale],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Centri
                }
            },
            {
                path: 'Fabbricati',
                component: FabbricatiComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
                        enum_Security_Attivita.Anagrafica_CentroAziendale,
                        enum_Security_Attivita.Anagrafica_Fabbricato],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati
                }
            },
            {
                path: 'Catasto',
                component: CatastoComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa, enum_Security_Attivita.Anagrafica_ParticellaCatastale],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Catasto
                }
            },
            {
                path: 'Campi',
                component: CampiComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
                        enum_Security_Attivita.Anagrafica_CentroAziendale,
                        enum_Security_Attivita.Anagrafica_Campo],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Campi
                }
            },
            // {
            //     path: 'Appezzamenti',
            //     component: AppezzamentiComponent,
            //     canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
            //     data: {
            //         readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
            //             enum_Security_Attivita.Anagrafica_CentroAziendale,
            //             enum_Security_Attivita.Anagrafica_Appezzamento],
            //         writePermissions: [],
            //         page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti
            //     }
            // },
            // {
            //     path: 'Impianti',
            //     component: ImpiantiComponent,
            //     canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
            //     data: {
            //         readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
            //             enum_Security_Attivita.Anagrafica_CentroAziendale,
            //             enum_Security_Attivita.Anagrafica_Appezzamento,
            //             enum_Security_Attivita.Anagrafica_Impianto],
            //         writePermissions: [],
            //         page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti
            //     }
            // },
            {
                path: 'Impianti',
                component: EserciziComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
                        enum_Security_Attivita.Anagrafica_CentroAziendale,
                        enum_Security_Attivita.Anagrafica_Appezzamento,
                        enum_Security_Attivita.Anagrafica_Impianto],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti
                }
            },
            {
                path: 'Contatti',
                component: ContattiComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
                        enum_Security_Attivita.Anagrafica_Contatto],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Contatti
                }
            },
            {
                path: 'Macchine',
                component: MacchineComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Anagrafica_Impresa,
                        enum_Security_Attivita.Anagrafica_ParcoMacchine],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Anagrafica_Macchine
                }
            },
        ]
    },
    {
        path: 'Imprese/Impresa-Edit',
        loadChildren: () => import('./imprese/impresa-edit/impresa.module').then(m => m.ImpresaModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    },
    {
        path: 'Centri/Centri-Edit',
        loadChildren: () => import('./centri/centri.module').then(m => m.CentriModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    },
    {
        // path: 'Appezzamenti/Appezzamento-Edit/:piva/:sa_cod/:appezza',
        path: 'Appezzamenti/Appezzamento-Edit',
        loadChildren: () => import('./appezzamenti/appezzamenti.module').then(m => m.AppezzamentoModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    },
    {
        path: 'Macchine/Macchine-Edit',
        loadChildren: () => import('./macchine/macchine-edit/macchina.module').then(m => m.MacchinaModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    },
    {
        path: 'Campi/Campi-Edit',
        loadChildren: () => import('./campi/campi-edit/campo.module').then(m => m.CampoModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    },
    {
        path: 'Catasto/Catasto-Edit',
        loadChildren: () => import('./catasto/catasto.module').then(m => m.CatastoModule),
        pathMatch: 'full', canActivate: [DatiGeneraliGuard, ModuliAttiviGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class AnagraficaRoutingModule { }
