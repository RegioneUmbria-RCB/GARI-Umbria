import { Component, Input, OnInit } from '@angular/core';
import { CssService } from '../../Service/css.service';
import { AgronicaCoreParametri_NG, enum_InputType, MasterService } from '../../Service/master.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from '../../Service/permessi-utente.service';
import { getBrowserLang, TranslocoService } from '@jsverse/transloco';
import { MessageService } from '@progress/kendo-angular-l10n';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { CustomMessagesService } from 'app/Service/kendo-messages.service';
import { AGRODATAFINE, AGRODATAINIZIO, SACOD_NOFILTRO } from 'app/Model/CostantiPersonalizzate';
import { ImpostazioniAziendeCentriService } from '../../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import {switchMap} from "rxjs";

export class HeaderModel {
    visible: boolean;
    logo: string;
    username: string;
    userLabel: string;
    timeVisibility_From: Date;
    timeVisibility_To: Date;
    last_access: Date;
    company: string;
    HomeButton: boolean;
    BackButton: boolean;
    ColumnLeft:boolean;
    ColumnRight:boolean;
}

@Component({
    standalone: false,
    selector: 'gias-master-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss']
})
/** header component*/
export class HeaderComponent implements OnInit {
    headerComponent: HeaderModel;

    ObjParametri_Server: string;
    ObjParametri_Utenti: string;
    ObjParametri_Super_Server: string;

    ObjParametri_Agenda: ObjParametriAgenda;

    link_GiasBase: string;
    link_CoreWS: string;

    link_jszip: string;
    link_kendo_all: string;
    link_kendo_messages: string;
    link_kendo_culture: string;

    link_FunzioniComuni_kendoGrid: string;

    isLoading = false;
    LoadingMessage = '';

    b_showErrorMsg: boolean;

    rag_soc = '';

    utente_collegato_username: string;
    utente_collegato_nome: string;
    timeout: number = 500;

    loadComplete: boolean = false;
    firsLoad: boolean = true;

    link_logo: string;

    objP_server: AgronicaCoreParametri_NG = null;
    public AGRODATAINIZIO = AGRODATAINIZIO;
    public AGRODATAFINE = AGRODATAFINE;

    constructor(private masterService: MasterService,
        private cssService: CssService,
        private giasMessageService: GiasMessageService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private objPermessi_UtenteService: PermessiUtenteService,
        private translocoService: TranslocoService,
        public kendoMessages: MessageService,
        private impostazioniaziendecentriservice: ImpostazioniAziendeCentriService) {

    }

    async ngOnInit() {

        this.masterService.initialLoadCompleteSource.subscribe((loadComplete) => {
            if (loadComplete && this.firsLoad) {
                this.firsLoad = false;
                this.loadComplete = loadComplete;
                this.ObjParametri_Server = this.masterService.ObjParametri_Server;
                this.ObjParametri_Utenti = this.masterService.ObjParametri_Utenti;
                this.ObjParametri_Super_Server = this.masterService.ObjParametri_Super_Server;
                this.link_GiasBase = this.masterService.link_GiasBase;
                this.link_CoreWS = this.masterService.link_CoreWS;

                this.link_jszip = this.link_GiasBase + '/kendoui/2021.1.119/js/jszip.min.js';
                this.link_kendo_all = this.link_GiasBase + '/kendoui/2021.1.119/js/kendo.all.min.js';
                this.link_kendo_messages = this.link_GiasBase + '/kendoui/2021.1.119/js/messages/kendo.messages.it-IT.min.js';
                this.link_kendo_culture = this.link_GiasBase + '/kendoui/2021.1.119/js/cultures/kendo.culture.it-IT.min.js';

                this.link_FunzioniComuni_kendoGrid = this.link_GiasBase + '/kendoui/ScriptsGiasKendo/2021.1.119/funzioniComuniKendoGrid.js';

                this.masterService.currentHeader.subscribe((el) => {
                    this.headerComponent = el;
                });

                this.headerComponent = this.masterService.getHeader();

                this.link_logo = this.masterService.link_GiasBase + '/' + this.headerComponent.logo;
                this.masterService.changeHeader(this.headerComponent);

                // this.masterService.currentIsLoading.subscribe(val => {
                //     if (val.isLoading) {
                //         setTimeout(() => {
                //             const isLoading = this.masterService.get_isLoading();
                //             this.isLoading = isLoading.isLoading;
                //             this.LoadingMessage = isLoading.message;
                //         }, this.timeout)

                //     } else {
                //         this.isLoading = val.isLoading;
                //         this.LoadingMessage = val.message;
                //     }
                // });

                this.masterService.currentErrorMsgType.subscribe((el)=> {
                    if (el.show){
                        this.showErrorMsg(el.msg, el.errorNumber);
                        //this.masterService.changeErrorMsgType({ show:false, msg:'' });
                    }
                });

                this.objP_server = this.masterService.objP_server;

                this.objPermessi_UtenteService.currentUtente_Permessi.subscribe((el) => {
                    if (el){
                        this.utente_collegato_username = el.Username;
                        if (el.Rag_Soc != ''){
                            this.utente_collegato_nome = el.Rag_Soc;
                        } else {
                            this.utente_collegato_nome = el.Nome + ' ' + el.Cognome;
                        }
                    }

                });

                this.objParametriAgendaService.currentObjParametriAgenda.pipe(switchMap ((el) => {
                    this.rag_soc = el.RagSoc;
                    return this.impostazioniaziendecentriservice.getImprese_Impostazioni(el.Piva,SACOD_NOFILTRO);
                })).subscribe();

                this.translocoService.setActiveLang(getBrowserLang());
            }
        })
    }

    changeInputs(inputType: enum_InputType){
        this.masterService.changeInputType(inputType);
    }


    showErrorMsg(msg: string, errorNumber: number) {
        let content = msg;
        if (errorNumber > 0) {
            content = 'Errore ' + errorNumber + ': ' + msg;
        }
        this.giasMessageService.errorMessage(content, true);
    }

    changeLanguage(ln: string) {
        this.translocoService.setActiveLang(ln);
        this.changeKendoWidgetsLanguage(ln)
    }

    changeKendoWidgetsLanguage(ln: string) {
        const svc = <CustomMessagesService>this.kendoMessages;

        svc.language = ln;
    }

    showFinestraTemporale() {
        return JSON.stringify(this.AGRODATAINIZIO) != JSON.stringify(this.objP_server.FinestraTemporaleInizio) || JSON.stringify(this.AGRODATAFINE) != JSON.stringify(this.objP_server.FinestraTemporaleFine)
    }

    showFinestraTemporaleInizio() {
        return JSON.stringify(this.AGRODATAINIZIO) != JSON.stringify(this.objP_server.FinestraTemporaleInizio);
    }

    showFinestraTemporaleFine() {
        return JSON.stringify(this.AGRODATAFINE) != JSON.stringify(this.objP_server.FinestraTemporaleFine);
    }

}
