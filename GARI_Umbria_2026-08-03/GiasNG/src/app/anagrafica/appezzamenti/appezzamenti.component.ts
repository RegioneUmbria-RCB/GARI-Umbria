import { Component, OnDestroy, OnInit } from '@angular/core';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { AppezzamentiEventsService } from './appezzamenti-events.service';
import { AppezzamentiHttpService } from './appezzamenti-http.service';

@Component({
    standalone: false,
    selector: 'app-appezzamenti',
    templateUrl: './appezzamenti.component.html',
    styleUrls: ['./appezzamenti.component.css'],
    providers: [
      ...generateGridProvidersAnagrafica(AppezzamentiHttpService, AppezzamentiComponent)
    ]
})
export class AppezzamentiComponent implements OnInit, OnDestroy {

    PromisesResp: Array<any>;
    changeDetectedSub: Subscription;
    objParamSub: Subscription;
    objParametriAgenda: ObjParametriAgenda;

    public signal$: Subject<void> = new Subject();

    constructor(
        private objParametriAgendaService: ObjParametriAgendaService,
        private kendoGridService: GridPublicService,
        private masterservice: MasterService,
        private appezzamentiEventsService: AppezzamentiEventsService
    ) { }

    ngOnInit() {

        this.masterservice.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });

        try {

            this.changeDetectedSub = this.kendoGridService.changeDetected.pipe(takeUntil(this.signal$)).subscribe((event: any) => {
                if(event?.action === 'edit'){
                    // this.AppezzamentiHttpService.editAppezzamentoInLine(event);
                }
                if(event?.action === 'remove'){
                    // this.eliminaAziendaAngular(event);
                }
                if(event?.action === 'info'){
                    this.appezzamentiEventsService.infoAppezzamento(event);
                }
            });

            this.objParamSub = this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntil(this.signal$)).subscribe((data: ObjParametriAgenda) => {
                this.objParametriAgenda = data;
            });

            this.objParametriAgenda=this.objParametriAgendaService.getObjParamValue();

            // this.cookieService.set(this.permessiUtenteService.getCurrentUser().Username + '_Anagrafica','Appezzamenti');
        } catch (e) {
            console.log('Error:', e);
        }

        this.masterservice.set_isLoading({ isLoading: false, message: '' });

    }



    // editAppezzamento(piva: string, sa_cod: number, appezza: number){
    //     this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    //     this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    //     this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit', piva, sa_cod, appezza]);
    // }

    public impresaSelezionata() {
        return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
    }

    ngOnDestroy(){
        this.objParamSub.unsubscribe();
        this.changeDetectedSub.unsubscribe();
        this.signal$.next();
        this.signal$.complete();
    }

    shouldDisableCmd(row: any) {
        return false;
    }

    // infoAppezzamento(event: GridInfoCommandEvent){
    //     this.objParametriAgenda.Piva=event.dataItem.Piva;
    //     this.objParametriAgenda.Sa_Cod=event.dataItem.Sa_Cod;
    //     this.objParametriAgenda.Appezza=event.dataItem.Appezza;
    //     this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    //     this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    //     this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    // }

    // onTemplateBtnClick(dataItem) {
    //     this.objParametriAgenda.Piva = dataItem.Piva;
    //     this.objParametriAgenda.Sa_Cod = dataItem.Sa_Cod;
    //     this.objParametriAgenda.Appezza = dataItem.Appezza;
    //     this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    //     this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    //     this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    // }

    // onAperturaChiusuraEsercizi() {
    //     this.gestioneRichiesteService.gestionePassaggioAltroSito
    //     (Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Gestione_Esercizi).then(resp => {
    //         this.windowService.open({
    //             title: this.objParametriAgenda.RagSoc + ' - ' + 'Chiusura/Apertura Esercizi',
    //             content: resp,
    //             height: window.innerHeight * 0.9,
    //             width: window.innerWidth * 0.9
    //         }
    //         );
    //     });
    // }

    onAperturaChiusuraEsercizi() {
        this.appezzamentiEventsService.onAperturaChiusuraEsercizi();
    }

    // onNuovo() {
    //     this.objParametriAgenda.Sa_Cod = 0;
    //     this.objParametriAgenda.Campo_Cod = 0;
    //     this.objParametriAgenda.Appezza = 0;
    //     this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    //     this.router.navigate(['Anagrafica/Appezzamenti/Appezzamento-Edit']);
    // }

    onNuovo() {
        this.appezzamentiEventsService.onNuovo();
    }

}
