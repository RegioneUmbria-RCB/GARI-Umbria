import { Injectable } from '@angular/core';
import { GridInfoCommandEvent } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { GiasIFrameWindowService } from 'gias-ui-kit';


@Injectable({providedIn: 'root'})
export class AppezzamentiEventsService {

    objParametriAgenda: ObjParametriAgenda;

    constructor(
        private masterService: MasterService,
        public objParametriAgendaService: ObjParametriAgendaService,
        private router: Router,
        private gestioneRichiesteService: GestioneRichiesteService,
        private windowService: GiasIFrameWindowService
    ) { }

    editAppezzamento(piva: string, sa_cod: number, appezza: number){
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit', piva, sa_cod, appezza]);
    }

    infoAppezzamento(event: GridInfoCommandEvent){
        this.objParametriAgenda.Piva=event.dataItem.Piva;
        this.objParametriAgenda.Sa_Cod=event.dataItem.Sa_Cod;
        this.objParametriAgenda.Appezza=event.dataItem.Appezza;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    }

    onTemplateBtnClick(dataItem) {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.Piva = dataItem.Piva;
        this.objParametriAgenda.Sa_Cod = dataItem.Sa_Cod;
        this.objParametriAgenda.Appezza = dataItem.Appezza;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    }

    onAperturaChiusuraEsercizi() {
        this.gestioneRichiesteService.gestionePassaggioAltroSito
        (Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Gestione_Esercizi).then(resp => {
            this.windowService.open({
                title: this.objParametriAgenda.RagSoc + ' - ' + 'Chiusura/Apertura Esercizi',
                content: resp,
                height: window.innerHeight * 0.9,
                width: window.innerWidth * 0.9
            }
            );
        });
    }

    onNuovo() {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.Sa_Cod = 0;
        this.objParametriAgenda.Campo_Cod = 0;
        this.objParametriAgenda.Appezza = 0;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    }

}
