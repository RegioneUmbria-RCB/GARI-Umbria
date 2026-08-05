import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { isDate } from 'lodash';
import { SessionStorageService } from 'ngx-webstorage';
import { BehaviorSubject, Observable } from 'rxjs';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { MasterService } from './master.service';
import { enum_Tipo_Operazione_Agenda_Target } from 'app/Model/attivita/Attivita';
import { consoleLogDebugParam } from './utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { ConversionService } from "./conversion.service";
import { IObjParametriAgendaService, ObjParametriAgenda, Tipo_Attivita } from 'gias-ui-kit';


@Injectable({ providedIn: 'root' })
export class ObjParametriAgendaService implements IObjParametriAgendaService {

    private objParametriAgenda: ObjParametriAgenda = new ObjParametriAgenda();

    constructor(
        private masterService: MasterService,
        private router: Router,
        private sessionSt: SessionStorageService,
        private conversionService: ConversionService
    ) {
        this.objParametriAgenda = new ObjParametriAgenda();
        this.objParametriAgenda.Data = AGRODATAINIZIO;
    }

    private objParametriAgendaSource = new BehaviorSubject(this.objParametriAgenda);
    currentObjParametriAgenda: Observable<ObjParametriAgenda> = this.objParametriAgendaSource.asObservable();


    getParametri() {
        return {
            objP_super_server: this.masterService.ObjParametri_Super_Server,
            objP_server: this.masterService.ObjParametri_Server,
            objP_utenti: this.masterService.ObjParametri_Utenti,
        };
    }

    changeObjParametriAgenda(upd_objParametriAgenda: ObjParametriAgenda) {
        this.checkObjAgenda(upd_objParametriAgenda);
        this.sessionSt.store('objParametri_Agenda', JSON.stringify(upd_objParametriAgenda));

        consoleLogDebugParam(
            enum_logDebugArea.App,
            enum_logDebugTipo.CambioAzienda,
            this.constructor.name,
            'changeObjParametriAgenda',
            upd_objParametriAgenda
        );

        this.objParametriAgendaSource.next(upd_objParametriAgenda);
    }

    private checkObjAgenda(objAgenda: ObjParametriAgenda): ObjParametriAgenda {
        if (objAgenda.TipoOperazioneDB == null || objAgenda.TipoOperazioneDB == undefined) {
            objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        }

        if (objAgenda.Piva == null || objAgenda.Piva == undefined) {
            objAgenda.Piva = '';
        }

        if (objAgenda.Sa_Cod == null || objAgenda.Sa_Cod == undefined) {
            objAgenda.Sa_Cod = 0;
        }

        if (objAgenda.Sa_Cod < 0) {
            objAgenda.Sa_Cod = 0;
        }

        if (objAgenda.Fabbricato == null || objAgenda.Fabbricato == undefined) {
            objAgenda.Fabbricato = 0;
        }

        if (objAgenda.Campo_Cod == null || objAgenda.Campo_Cod == undefined) {
            objAgenda.Campo_Cod = 0;
        }

        if (objAgenda.Appezza == null || objAgenda.Appezza == undefined) {
            objAgenda.Appezza = 0;
        }

        if (objAgenda.Id_Reg == null || objAgenda.Id_Reg == undefined) {
            objAgenda.Id_Reg = 0;
        }

        if (objAgenda.Progetto_Cod == null || objAgenda.Progetto_Cod == undefined) {
            objAgenda.Progetto_Cod = 0;
        }

        if (objAgenda.Veg_Cod == null || objAgenda.Veg_Cod == undefined) {
            objAgenda.Veg_Cod = 0;
        }

        if (objAgenda.Id_Cod == null || objAgenda.Id_Cod == undefined) {
            objAgenda.Id_Cod = 0;
        }

        if (objAgenda.Cau_Mov == null || objAgenda.Cau_Mov == undefined) {
            objAgenda.Cau_Mov = 0;
        }

        if (objAgenda.Mac_Cod == null || objAgenda.Mac_Cod == undefined) {
            objAgenda.Mac_Cod = 0;
        }

        if (objAgenda.Lav_Cod == null || objAgenda.Lav_Cod == undefined) {
            objAgenda.Lav_Cod = 0;
        }

        if (objAgenda.Pagina_Provenienza == null || objAgenda.Pagina_Provenienza == undefined) {
            objAgenda.Pagina_Provenienza = 0;
        }

        if (objAgenda.Pagina_Richiesta == null || objAgenda.Pagina_Richiesta == undefined) {
            objAgenda.Pagina_Richiesta = 0;
        }

        if (typeof objAgenda.Data == 'string') {
            try {
                objAgenda.Data = this.conversionService.convertStringToDate(objAgenda.Data)
            } catch (e) {
                console.log("ERROR ObjParametriAgendaService on convertStringToDate", objAgenda.Data, e)
            }
        }

        if (objAgenda.Data == null || objAgenda.Data == undefined || !isDate(objAgenda.Data) || objAgenda.Data < AGRODATAINIZIO) {
            objAgenda.Data = AGRODATAINIZIO;
        }

        if (objAgenda.Validita_Inizio == null || objAgenda.Validita_Inizio == undefined || !isDate(objAgenda.Validita_Inizio) || objAgenda.Validita_Inizio < AGRODATAINIZIO) {
            objAgenda.Validita_Inizio = AGRODATAINIZIO;
        }

        if (objAgenda.Validita_Fine == null || objAgenda.Validita_Fine == undefined || !isDate(objAgenda.Validita_Fine) || objAgenda.Validita_Fine < AGRODATAINIZIO) {
            objAgenda.Validita_Fine = AGRODATAFINE;
        }

        if (objAgenda.RedirectUrl == null || objAgenda.RedirectUrl == undefined) {
            objAgenda.RedirectUrl = '';
        }

        if (objAgenda.Ricetta_Cod == null || objAgenda.RedirectUrl == undefined) {
            objAgenda.Ricetta_Cod = 0;
        }

        if (objAgenda.Sito_Provenienza == null || objAgenda.Sito_Provenienza == undefined) {
            objAgenda.Sito_Provenienza = 0;
        }

        if (objAgenda.GenericObj_string == null || objAgenda.GenericObj_string == undefined) {
            objAgenda.GenericObj_string = "";
        }

        if (objAgenda.Regolamento_Cod == null || objAgenda.Regolamento_Cod == undefined) {
            objAgenda.Regolamento_Cod = 0;
        }

        if (objAgenda.Tipo_Regolamento == null || objAgenda.Tipo_Regolamento == undefined) {
            objAgenda.Tipo_Regolamento = 0;
        }


        return objAgenda;
    }

    getObjParamValue(): any {
        return Object.assign({}, this.objParametriAgendaSource.getValue());
    }


    setValiditaInizio(date: Date): void {
        this.objParametriAgenda.Validita_Inizio = date;
    }

    setValiditaFine(date: Date): void {
        this.objParametriAgenda.Validita_Fine = date;
    }

    navigateTo(link: string, queryParams: { [key: string]: string | string[] }, objAgenda: ObjParametriAgenda, reloadThePage: boolean) {

        let params: ObjParametriAgenda;

        if (objAgenda) {
            params = objAgenda;
            this.changeObjParametriAgenda(params);
        } else {
            params = this.getObjParamValue();
        }

        if (reloadThePage === true) {
            this.router.routeReuseStrategy.shouldReuseRoute = () => false;
            this.router.onSameUrlNavigation = 'reload';
        }

        this.router.navigate([link], { queryParams: queryParams });
    }

    resettaObjAgenda(objAgenda: ObjParametriAgenda): ObjParametriAgenda {
        objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        objAgenda.Piva = ''
        objAgenda.Sa_Cod = 0;
        objAgenda.Fabbricato = 0;
        objAgenda.Campo_Cod = 0;
        objAgenda.Appezza = 0;
        objAgenda.Id_Reg = 0;
        objAgenda.Progetto_Cod = 0;
        objAgenda.Validita_Inizio = AGRODATAINIZIO;
        objAgenda.Validita_Fine = AGRODATAFINE;
        objAgenda.Veg_Cod = 0;
        objAgenda.Veg_Des = '';
        objAgenda.Id_Cod = 0;
        objAgenda.Id_Des = '';
        objAgenda.Cau_Mov = 0;
        objAgenda.Mac_Cod = 0;
        objAgenda.Lav_Cod = 0;
        objAgenda.Lav_Des = '';
        objAgenda.SaNome = '';
        objAgenda.RagSoc = '';
        objAgenda.Pagina_Provenienza = 0;
        objAgenda.Pagina_Richiesta = 0;
        objAgenda.Data = AGRODATAINIZIO;
        objAgenda.Cod_Contatto = '';
        objAgenda.QueryStringFiltrino = '';
        objAgenda.Impianti = [];
        objAgenda.Id_Agenda = 0;
        objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
        objAgenda.TipoRicetta = 0;
        objAgenda.Stato = 0;
        objAgenda.Ricetta_Operazione_Cod = 0;
        objAgenda.Ricetta_Cod = 0;
        objAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Not_Set;
        objAgenda.Programmazione_Cod = 0;
        objAgenda.GenericObj_string = '';
        objAgenda.Regolamento_Cod = 0;
        objAgenda.Tipo_Regolamento = 0;

        return objAgenda;
    }


    public impresaIsSelected() {
        const agenda = this.getObjParamValue();
        return agenda.Piva != null && agenda.Piva !== '';
    }
}
