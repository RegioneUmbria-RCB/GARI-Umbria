import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';
import { faLayerGroup, faLeaf } from '@fortawesome/free-solid-svg-icons';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { CampiServiceProvider } from 'app/Service/ServiceFactory/campi.factory.provider';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GridPublicService } from 'gias-kendo-grid';
import { Subscription } from 'rxjs';
import { CampiGridHttpService } from './campi-grid-configuration.service';
import { CampiGridEventsService } from './campi-grid-events.service';
import { CampiKendoServerResult } from './campi.model';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-campi',
    templateUrl: './campi.component.html',
    styleUrls: ['./campi.component.css'],
    providers: [
        ...generateGridProvidersAnagrafica(CampiGridHttpService, CampiComponent), CampiGridEventsService, CampiServiceProvider
    ]
})

export class CampiComponent implements OnInit {

    tableDataSub: Subscription;
    ddlSub: Subscription;
    objParamSub: Subscription;

    objParametriAgenda: ObjParametriAgenda;
    changeDetectedSub = new Subscription();

    permessoEdit: boolean

    faLayerGroup = faLayerGroup;
    faLeaf = faLeaf;
    SMARTPHONE_WIDTH = SMARTPHONE_WIDTH;
    constructor(
        private route: ActivatedRoute,
        private objParametriAgendaService: ObjParametriAgendaService,
        private kendoGridService: GridPublicService,
        private campiGridEventssService: CampiGridEventsService,
        private permessiUtenteService: PermessiUtenteService) {
    }

    async ngOnInit() {
        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Campo, 2);
        this.objParamSub = this.objParametriAgendaService.currentObjParametriAgenda.subscribe((data: ObjParametriAgenda) => {
            this.objParametriAgenda = data;
        });

        this.changeDetectedSub = this.kendoGridService.changeDetected.subscribe((event: any) => {
            if(event?.action === 'edit'){
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
            }
            if(event?.action === 'remove'){
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Delete;
                this.campiGridEventssService.addCampoSelezionato(event?.dataItem, "");
            }
            if(event?.action === 'info'){
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
                this.campiGridEventssService.infoCampoAngular(event);
            }
            if(event?.action === 'add'){
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
            }
        });

        this.tableDataSub = this.route.data.subscribe(
            async (dict: Data) => {
                const data: CampiKendoServerResult = dict['tableData'];
                if(data == null) {
                    return;
                }
            }
        );
    }

    public impresaSelezionata() {
        return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
    }

    ngOnDestroy() {
        if (this.tableDataSub != undefined){
            this.tableDataSub.unsubscribe();
        }
        if (this.ddlSub != undefined){
            this.ddlSub.unsubscribe();
        }
        if (this.objParamSub != undefined){
            this.objParamSub.unsubscribe();
        }

    }

    ottieniDati(eventRowItem: any){
        let indici = [0, 1, 2];
        if('Id_Budget' in eventRowItem){
            //Nel caso sono nel budget allora shifto gli indici, questo perche la chiave e' IdBudge_Piva_SaCod_CampoCod
            //Rispetto al normale Piva_SaCod_CampoCod
           indici = indici.map(x => x + 1);
        }

        let chiavi = eventRowItem.chiave.split("_");

        return indici.map(index => chiavi[index]);
    }

    campoSelezionato(event: any) {
        event.selectedRows.forEach(row => {
            this.campiGridEventssService.addCampoSelezionato(row.dataItem, "");
        });
        event.deselectedRows.forEach(row => {
            this.campiGridEventssService.removeCampoSelezionato(row.dataItem);
        });
        if (event.selectedRows.length > 0 && event.selectedRows[0].dataItem.chiave != null){
            let [piva, sa_cod, campo_Cod] = this.ottieniDati(event.selectedRows[0].dataItem);

            this.objParametriAgenda.Campo_Cod = parseInt(campo_Cod);
            this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
            this.objParametriAgenda.Piva = piva;

            this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        }
    }

    createCampoAngular() {
        this.campiGridEventssService.modificaCampoAngular(undefined, true, undefined);
    }

    onCatasto(event: Event, dataItem: any) {
        this.campiGridEventssService.onInvestimentoCatastale(dataItem)
    }

}
