import {Component, Inject, OnInit, ViewEncapsulation} from '@angular/core';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { GridPublicService } from 'gias-kendo-grid';
import { Subscription, take, tap } from 'rxjs';
import { MasterService } from '../../Service/master.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { MacchineConfigService as MacchineGridConfigService } from './macchine-grid-config.service';
import { MacchineGridEventsService } from './macchine-grid-events.service';
import { AnagraficaBusinessLogicService } from '../services/anagrafica-business-logic.service';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-macchine',
    templateUrl: './macchine.component.html',
    styleUrls: ['./macchine.component.scss'],
    providers: [
        ...generateGridProvidersAnagrafica(MacchineGridConfigService, MacchineComponent),
      MacchineGridEventsService,
      AnagraficaBusinessLogicService
    ],
    encapsulation: ViewEncapsulation.None
})

/** Macchine Component*/
export class MacchineComponent implements OnInit {

    objParametriAgenda: ObjParametriAgenda;
    editSubscription: Subscription;
    permessoEdit: boolean;

    constructor(
        private macchineGridEventsService: MacchineGridEventsService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private kendoGridService: GridPublicService,
        private permessiUtenteService: PermessiUtenteService,
        @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
        private masterService: MasterService
    ) {  }

    async ngOnInit() {
        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine, 2);
        this.editSubscription = this.kendoGridService.changeDetected.subscribe((event: any) => {
            if(event?.action === 'info'){
                this.macchineGridEventsService.infoMacchinaAngular(event);
            }
        });
        try {
            this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
            if(this.objParametriAgenda.Piva === null || this.objParametriAgenda.Piva === undefined) {
                this.objParametriAgenda.Piva = '';
            }

        } catch (e) {
            console.log('Error:', e);
        }
    }

    ngOnDestroy(){
        this.editSubscription?.unsubscribe();
    }

    MacchinaSelezionata(event: any){
        if (event.selectedRows.length > 0){
            this.objParametriAgenda.Mac_Cod = parseInt(event.selectedRows[0].dataItem.chiave.split('_')[2]);
            this.objParametriAgenda.Sa_Cod = parseInt(event.selectedRows[0].dataItem.chiave.split('_')[1]);
            this.objParametriAgenda.Piva = event.selectedRows[0].dataItem.chiave.split('_')[0];
            this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        }
    }

    nuovaMacchinaAngular(add_operation?: boolean) {
        this.macchineGridEventsService.modificaMacchinaAngular(undefined, add_operation, undefined);
    }

    public impresaSelezionata() {
        return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
    }

    testMacchine() {
        this.masterService.set_isLoading({ isLoading: true });
        this.macchineService.testLettura(this.objParametriAgendaService.getObjParamValue()).pipe(take(1), tap((r) => {
            this.masterService.set_isLoading({ isLoading: false });
        })).subscribe();
    }

}

