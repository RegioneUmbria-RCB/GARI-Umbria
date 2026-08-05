import { ModuleWithProviders, NgModule } from '@angular/core';
import { ImpreseService } from './Anagrafica/imprese.service';
import { CssService } from './css.service';
import { GestioneRichiesteService } from './gestione-richieste.service';
import { MasterService } from './master.service';
import { FormeGiurificheService } from './Metaschema/forme-giuridiche.service';
import { ObjParametriAgendaService } from './obj-parametri-agenda.service';
import { PermessiUtenteService } from './permessi-utente.service';

@NgModule({
    imports: [],
})
export class SharedServicesModule {
    static forRoot(): ModuleWithProviders<SharedServicesModule> {
        return {
            ngModule: SharedServicesModule,
            providers: [
                ImpreseService,
                FormeGiurificheService,
                ObjParametriAgendaService,
                PermessiUtenteService,
                CssService,
                MasterService,
                GestioneRichiesteService
            ]
        };
    }
}
