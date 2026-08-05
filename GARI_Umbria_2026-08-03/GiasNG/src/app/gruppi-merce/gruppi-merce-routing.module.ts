import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CurrentPageGuard } from 'app/Guard/current-page-guard.service';
import { DatiGeneraliGuard } from 'app/Guard/datiGenerali-guard.service';
import { ImpresaRichiestaGuard } from 'app/Guard/impresaRichiesta-guard.service';
import { ModuliAttiviGuard } from 'app/Guard/moduli-attivi-guard.service';
import { PermessiUtenteGuard } from 'app/Guard/permessi-utente.guard.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { EditGruppiMerceComponent } from './gestioneGruppiMerce/editGruppiMerce.component';
import { GruppiMerceMasterComponent } from './gruppi-merce-master.component';
import { GruppiUtentiPerGruppiMerce } from './gruppiUtentiPerGruppiMerce/gruppiUtentiPerGruppiMerce.component';

const routes: Routes = [
    {
        path: '',
        component: GruppiMerceMasterComponent,
        children: [
            {
                path: 'Anagrafica', 
                component: EditGruppiMerceComponent,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Gruppi_Merce],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Gruppi_Merce_Anagrafica,
                },
                children: [
                    {
                        path: "",
                        canActivate: [ImpresaRichiestaGuard],
                        data: {
                            impresaRichiestaGuardData: {
                                sitoRedirect: Enum_SiteRedirector.GiasNG,
                                paginaRedirect: enum_PagineGiasNG.Pagina_Corrente
                            }
                        }
                    }
                ]
            },
            {
                path: 'Autorizzazioni',
                component: GruppiUtentiPerGruppiMerce,
                canActivate: [DatiGeneraliGuard, PermessiUtenteGuard, CurrentPageGuard, ModuliAttiviGuard],
                data: {
                    readPermissions: [enum_Security_Attivita.Gruppi_Merce],
                    writePermissions: [],
                    page: enum_PagineGiasNG.Pagina_Menu_Gruppi_Merce_Autorizzazioni
                }
            }
        ]
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GruppiMerceRoutingModule { }

