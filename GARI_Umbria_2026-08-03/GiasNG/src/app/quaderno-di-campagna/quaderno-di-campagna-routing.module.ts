import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ImpresaRichiestaGuard } from 'app/Guard/impresaRichiesta-guard.service';
import { QdCLoadedGuard } from 'app/menu-agenda/guards/transloco-qdc-guard.service';
import { MenuAgendaComponent } from 'app/menu-agenda/menu-agenda.component';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import {enum_PagineGiasNG, enum_Security_Attivita} from 'app/Model/TipiEnumerativi';
import { QuadernoDiCampagnaComponent } from './agenda-edit/quaderno-di-campagna.component';
import {PermessiUtenteGuard} from "../Guard/permessi-utente.guard.service";
import {CurrentPageGuard} from "../Guard/current-page-guard.service";
import { ConfigurazioneOperazioniCulturaliComponent } from './configurazione-operazioni-culturali/configurazione-operazioni-culturali/configurazione-operazioni-culturali.component';


const routes: Routes = [
    {
        path: 'MenuAgenda',
        component: MenuAgendaComponent,
        canActivate: [DatiGeneraliGuard],
        children: [
            {
                path: "",
                canActivate: [CurrentPageGuard, ImpresaRichiestaGuard],
                data: {
                    impresaRichiestaGuardData: {
                        sitoRedirect: Enum_SiteRedirector.GiasNG,
                        paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
                    },
                    page: enum_PagineGiasNG.Pagina_Menu_Agenda
                }
            }
        ],
        runGuardsAndResolvers: 'always'
    },
    {
        path: 'ConfigurazioneOperazioniCulturali',
        component: ConfigurazioneOperazioniCulturaliComponent,
        canActivate: [DatiGeneraliGuard, CurrentPageGuard],
        data: {
            page: enum_PagineGiasNG.Pagina_Configurazione_Operazioni_Culturali
        }
    },
    {
        path: '',
        component: QuadernoDiCampagnaComponent,
        canActivate: [DatiGeneraliGuard, CurrentPageGuard, QdCLoadedGuard,ImpresaRichiestaGuard,PermessiUtenteGuard],
        data: {
            impresaRichiestaGuardData: {
                sitoRedirect: Enum_SiteRedirector.GiasNG,
                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
            },
            readPermissions: [],
            writePermissions: [enum_Security_Attivita.Agenda_AccessoMenu_NG,enum_Security_Attivita.Gest_Ricette,enum_Security_Attivita.Brogliaccio],
            page: enum_PagineGiasNG.Pagina_Edit_Attivita
        },
        runGuardsAndResolvers: 'always'
    },
    {
        path: 'Visite-Edit',
        component: QuadernoDiCampagnaComponent,
        canActivate: [DatiGeneraliGuard, CurrentPageGuard, QdCLoadedGuard,ImpresaRichiestaGuard,PermessiUtenteGuard],
        data: {
            impresaRichiestaGuardData: {
                sitoRedirect: Enum_SiteRedirector.GiasNG,
                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
            },
            readPermissions: [],
            writePermissions: [enum_Security_Attivita.Agenda_AccessoMenu_NG,enum_Security_Attivita.Gest_Ricette,enum_Security_Attivita.Brogliaccio],
            page: enum_PagineGiasNG.Pagina_Edit_Visite
        },
        runGuardsAndResolvers: 'always'
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class QuadernoDiCampagnaRoutingModule { }
