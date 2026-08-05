import { Injectable } from '@angular/core';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_PagineGiasNG, Enum_TipoComportamento_FiltroRicerca, Enum_TipoMostra_FiltroRicerca } from 'app/Model/TipiEnumerativi';
import { AjaxAgronicaService } from './ajax-agronica.service';
import { AgronicaCoreParametri_NG, AgronicaLink_NG, Imprese_Impostazioni, MasterService, VariabiliInSessione_NG } from './master.service';
import { ObjParametriAgendaService } from './obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { PermessiUtenteService } from './permessi-utente.service';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { cloneDeep } from 'lodash';
import { Redirect_To_GiasNG_Page } from '../quaderno-di-campagna/agenda-edit/service/qdc.service';
import { AjaxAgronicaAPIService } from './ajax-agronica.api.service';
import { map, Observable, of, switchMap } from 'rxjs';
import { Utente } from 'app/Model/utente/utente';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from './configurazione-siti.service';
import { ParametriFiltroRicercaNG } from 'app/filtro-ricerca/utils';
import { Utente_Impostazioni } from 'app/Model/utente/utente_impostazioni';

export class warmUpGiasNG_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
  unid: string;
}

export class warmUpEFGiasNG_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class warmUpGiasNG_Response {
  objP_super_server: AgronicaCoreParametri_NG;
  objP_server: AgronicaCoreParametri_NG;
  objP_utenti: AgronicaCoreParametri_NG;
  objParametri_Agenda: ObjParametriAgenda;
  utente: Utente;
  link: AgronicaLink_NG;
  VariabiliInSessione: VariabiliInSessione_NG;
  impresa_impostazioni: Imprese_Impostazioni[];
  Lingua_Cod: number;
}

export class ParametriAggiuntivi_QueryString {
  key: string;
  value: string;
  codifica: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class GestioneRichiesteService {

  isBack: boolean = false;

  //Viene valorizzato solamente qunado faccio il redirect ad un'altra
  //pagina di Angular.
  private _ObjPageToMemorize: any = null;

  constructor(
    private masterService: MasterService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private impostazioniUtenteService: PermessiUtenteService,
    private GiasIFrameWindowService: GiasIFrameWindowService,
    private translocoService: TranslocoService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) { }


  public get ObjPageToMemorize(): Redirect_To_GiasNG_Page {
    return cloneDeep(this._ObjPageToMemorize);
  }

  public set ObjPageToMemorize(objPageToMemorize: Redirect_To_GiasNG_Page) {
    this._ObjPageToMemorize = objPageToMemorize;
  }

  warmUpGiasNG(unid: string) {
    return new Promise<warmUpGiasNG_Response>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<string, warmUpGiasNG_Response>('UtilityNG/warmUpGiasNG', unid, true).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  warmUpEFGiasNG() {
    return new Promise<any>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIGet<any, any>('UtilityNG/warmUpEFGiasNG', '', true).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  private gestionePassaggioSitoAgenda(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = objParametriAgenda ?? this.objParametriAgendaService.getObjParamValue();
      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];
      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAgenda', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          //console.log(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });
  }

  private gestionePassaggioSitoSincronizzatore(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];


      this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('UtilityNG/PassaggioSitoSincronizzatore', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();


    });

  }

  private gestionePassaggioSitoAnalisi(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAnalisi', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });
  }

  private gestionePassaggioSitoPua(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];


      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoPua', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();

    });

  }

  private gestionePassaggioSitoPlanning(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoPlanning', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true
      ).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();

    });

  }

  private gestionePassaggioSitoGiasOnline(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1
  ) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoGiasOnline', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  private gestionePassaggioSitoProfilazione(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1
  ) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoProfilazione', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();


    });

  }

  private gestionePassaggioSitoAudit(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1
  ) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAudit', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }


  private gestionePassaggioSitoPianiCampionamento(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoPianiCampionamento', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();

    });

  }


  private gestionePassaggioSitoPianiSemina(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoPianiSemina', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();

    });

  }


  private gestionePassaggioSitoPianoConcimazione_2017(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoPianoConcimazione', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();

    });

  }

  private gestionePassaggioSitoAgronicaCheckCOOP(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1
  ) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAgronicaCheckCOOP', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  private gestionePassaggioSitoAgronicaLabQualita(
    PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1
  ) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAgronicaLabQualita', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  private gestionePassaggioSitoStampe_2010(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoStampe_2010', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  /*private gestionePassaggioSitoAgronicaUma_Old(PaginaRichiesta: number,
      AggiungiSoloParametriAggiuntivi: boolean,
      ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
      objParametriAgenda?: ObjParametriAgenda,
      IDSezione: number = -1) {
      return new Promise<string>(async (resolve, reject) => {

          var objPAgenda = this.objParametriAgendaService.getObjParamValue();

          if(objParametriAgenda !== undefined && objParametriAgenda !== null)
              objPAgenda = objParametriAgenda;

          objPAgenda.Pagina_Richiesta = PaginaRichiesta;

          if(ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
              ParametriAggiuntivi = [];

          const parametri: any = {
              objP_super_server: this.masterService.ObjParametri_Super_Server,
              objP_server: this.masterService.ObjParametri_Server,
              objP_utenti: this.masterService.ObjParametri_Utenti,
              VariabiliInSessione: this.masterService.variabiliInSessione,
              InData: objPAgenda,
              ParametriAggiuntivi: ParametriAggiuntivi,
              AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
              IDSezione: IDSezione
          };
          const R = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Utility.asmx/PassaggioSitoAgronicaUma', parametri);
          if (R.RispostaOK) {
              R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
              resolve(R.RispostaStringa);
          } else {
              reject(R.Errore);
          }
      })).subscribe();
      });

  }*/

  private gestionePassaggioSitoAgronicaUma(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAgronicaUma', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  private gestionePassaggioSitoAgronicaDomandaIrrigua(PaginaRichiesta: number,
    AggiungiSoloParametriAggiuntivi: boolean,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    IDSezione: number = -1) {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      if (objParametriAgenda !== undefined && objParametriAgenda !== null)
        objPAgenda = objParametriAgenda;

      objPAgenda.Pagina_Richiesta = PaginaRichiesta;

      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/PassaggioSitoAgronicaDomandaIrrigua', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi,
        AggiungiSoloParametriAggiuntivi: AggiungiSoloParametriAggiuntivi,
        IDSezione: IDSezione
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });

  }

  gestionePassaggioStessoSito(PaginaRichiesta: number,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    IDSezione: number = -1): Promise<string> {

    //Imposto come sito origine Gias_NG dato che sto navigando al suo interno
    let agenda = this.objParametriAgendaService.getObjParamValue();

    agenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

    agenda.Pagina_Provenienza_AltroSito = 0;

    this.objParametriAgendaService.changeObjParametriAgenda(agenda);

    return this.getPathFromPagina_Richiesta(PaginaRichiesta);
  }

  getPathFromPagina_Richiesta(PaginaRichiesta: number): Promise<string> {
    return new Promise<string>(async (resolve, reject) => {
      switch (PaginaRichiesta) {
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica:
          resolve('Anagrafica');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica_Imprese:
          resolve('Anagrafica/Imprese');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti:
          resolve('Anagrafica/Impianti');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica_Contatti:
          resolve('Anagrafica/Contatti');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati:
          resolve('Anagrafica/Fabbricati');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Anagrafica_Macchine:
          resolve('Anagrafica/Macchine');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Catasto:
          resolve('Anagrafica/Catasto/Catasto-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Centro:
          resolve('Anagrafica/Centri/Centri-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal:
          resolve('Anagrafica/Appezzamenti/Appezzamento-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Campo:
          resolve('Anagrafica/Campi/Campi-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Attivita:
          resolve('QdC');
          break;
        case enum_PagineGiasNG.Pagina_Configurazione_Operazioni_Culturali:
          resolve('QdC/ConfigurazioneOperazioniCulturali');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Macchina:
          resolve('Anagrafica/Macchine/Macchine-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Agenda:
          resolve('QdC/MenuAgenda');
          break;
        case enum_PagineGiasNG.Pagina_Budget_Menu_Anagrafica_Imprese:
          resolve('Budget/Anagrafica/Imprese');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Gruppi_Merce_Autorizzazioni:
          resolve('GruppiMerce/Autorizzazioni');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Gruppi_Merce_Anagrafica:
          resolve('GruppiMerce/Anagrafica');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Profilazione:
          resolve('Profilazione');
          break;
        case enum_PagineGiasNG.Pagina_Profilazione_Import_Utenti:
          resolve('Profilazione/Importazione-Utenti');
          break;
        case enum_PagineGiasNG.Pagina_GIS:
          resolve('GIS');
          break;
        case enum_PagineGiasNG.Pagina_GIS_cfg_proiezioni:
          resolve('GIS/cfg-proiezioni');
          break;
        case enum_PagineGiasNG.Pagina_Dashboard:
          resolve('dashboard');
          break;
        case enum_PagineGiasNG.Pagina_Preferiti_Config:
          resolve('preferiti-config');
          break;
        case enum_PagineGiasNG.Pagina_AmministrazioneSistema_ConsultaSincroDatiApp:
          resolve('Amministrazione-Sistema/Consulta-Sincro-Dati-App');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Visite:
          resolve('Visite/MenuVisite');
          break;
        case enum_PagineGiasNG.Pagina_Edit_Visite:
          resolve('QdC/Visite-Edit');
          break;
        case enum_PagineGiasNG.Pagina_Valutazioni:
          resolve('Valutazioni-Main/Valutazioni');
          break;
        case enum_PagineGiasNG.Pagina_PianoConti:
          resolve('Valutazioni-Main/PianoConti');
          break;
        case enum_PagineGiasNG.Pagina_Gruppi_Raccolta:
          resolve('gruppi-raccolta');
          break;
        case enum_PagineGiasNG.Pagina_Requisiti_Stabilimento:
        case enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_PianoColturale:
          resolve('requisiti-stabilimento/piano-colturale');
          break;
        case enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_Contratti:
          resolve('requisiti-stabilimento/contratti');
          break;
        case enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_VD_PianoColturale:
          resolve('requisiti-stabilimento/VisualizzaDettagli/piano-colturale');
          break;
        case enum_PagineGiasNG.Pagina_Requisiti_Stabilimento_VD_Contratti:
          resolve('requisiti-stabilimento/VisualizzaDettagli/contratti');
          break;
        case enum_PagineGiasNG.Pagina_Dati_Previsionali_Colture:
          resolve('dati-previsionali-colture');
          break;
        case enum_PagineGiasNG.Pagina_Confronto_Piano_Colturale:
          resolve('confronto-piano-colturale');
          break;
        case enum_PagineGiasNG.Pagina_Filtro_Ricerca:
          resolve('FiltroDiRicerca/Filtrone');
          break;
        case enum_PagineGiasNG.Pagina_Export_QdC_To_Agea:
          resolve('Export/ExportQdCToAgea');
          break;
        case enum_PagineGiasNG.Pagina_Profilazione_Imprese:
          resolve('profilazione-imprese');
          break;
        case enum_PagineGiasNG.Pagina_Analisi_Terreno_Menu:
          resolve('MenuAnalisiTerreno/AnalisiTerreno');
          break;
        case enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit:
          resolve('MenuAnalisiTerreno/AnalisiTerreno/edit');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Zoo:
          resolve('operazioni-zootecniche');
          break;
        case enum_PagineGiasNG.Pagina_Trattamento_Zoo:
          resolve('TrattamentoZoo');
          break;
        case enum_PagineGiasNG.Pagina_Lettura_Contatori:
          resolve('domanda-irrigua/lettura-contatori');
          break;
        case enum_PagineGiasNG.Pagina_Report_Impiego_Prodotti_Fitosanitari:
          resolve('ReportImpiegoProdottiFitosanitari');
          break;
        case enum_PagineGiasNG.Pagina_Menu_Rilievi:
          resolve('Rilievi/MenuRilievi');
          break;
        case enum_PagineGiasNG.Pagina_Report_Abilitazione_PdC:
          resolve('ReportAbilitazionePdC')
          break;
        case enum_PagineGiasNG.Pagina_Codifiche_Sistemi_Esterni:
          resolve('CodificheSistemiEsterni');
          break;
        case enum_PagineGiasNG.GestioneDisciplinari_Verifica_Disciplinare:
          resolve('qualita-tracciabilita/verifica-disciplinare');
          break;
        case enum_PagineGiasNG.Pagina_Terapia_Zoo:
          resolve('TerapiaZoo');
          break;
        case enum_PagineGiasNG.Pagina_SostenibitaCO2_SelezionePerimetro:
          resolve('SostenibitaCO2/SelezionePerimetro');
          break;
        case enum_PagineGiasNG.Pagina_SostenibitaCO2_CreazioneToken:
          resolve('SostenibitaCO2/CreazioneToken');
          break;
        case enum_PagineGiasNG.Pagina_RischiMeteo_CalcoloRischi:
          resolve('rischi-meteo/calcolo-rischi');
          break;
        case enum_PagineGiasNG.Pagina_RischiH20_SelezionePerimetro:
          resolve('RischiH20/SelezionePerimetro');
          break;
        case enum_PagineGiasNG.Pagina_PesateAccrescimento:
          resolve('PesateAccrescimento');
          break;
        case enum_PagineGiasNG.Pagina_Scadenza_Reinnesco_Trappole:
          resolve('ScadenzaReinnescoTrappole');
          break;
        case enum_PagineGiasNG.Pagina_DSS_Nutrizione:
          resolve('DSSNutrizione');
          break;
        default:
          reject('Funzione non ancora implementata');
          break;
      }
    });
  }

  gestionePassaggioAltroSito(
    SitoRichiesto: Enum_SiteRedirector,
    PaginaRichiesta: number,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    AggiungiSoloParametriAggiuntivi: boolean = true,
    IDSezione: number = -1
  ) {

    if (!objParametriAgenda) {
      objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    }
    if (!objParametriAgenda.Pagina_Provenienza || objParametriAgenda.Pagina_Provenienza == 0) {
      objParametriAgenda.Pagina_Provenienza = this.masterService.getCurrentPageAsValue();
    }

    //objParametriAgenda.Pagina_Provenienza = this.masterService.getCurrentPage();

    switch (SitoRichiesto) {
      case Enum_SiteRedirector.Sito_AgronicaAgenda_2010:
        return this.gestionePassaggioSitoAgenda(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.GiasNG:
        return this.gestionePassaggioStessoSito(PaginaRichiesta, ParametriAggiuntivi, IDSezione);
      case Enum_SiteRedirector.GiasLan:
        return this.gestionePassaggioSitoGiasOnline(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaPlanning:
        return this.gestionePassaggioSitoPlanning(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaProfilazione:
        return this.gestionePassaggioSitoProfilazione(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaSincronizzatore:
        return this.gestionePassaggioSitoSincronizzatore(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010:
        return this.gestionePassaggioSitoAnalisi(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaPUA:
        return this.gestionePassaggioSitoPua(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento:
        return this.gestionePassaggioSitoPianiCampionamento(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaPianiSemina:
        return this.gestionePassaggioSitoPianiSemina(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_PianoConcimazione_2017:
        return this.gestionePassaggioSitoPianoConcimazione_2017(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaAudit:
        return this.gestionePassaggioSitoAudit(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaStampe_2010:
        return this.gestionePassaggioSitoStampe_2010(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaUma:
        return this.gestionePassaggioSitoAgronicaUma(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua:
        return this.gestionePassaggioSitoAgronicaDomandaIrrigua(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_GiasOnline:
        return this.gestionePassaggioSitoGiasOnline(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaCheckCOOP:
        return this.gestionePassaggioSitoAgronicaCheckCOOP(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      case Enum_SiteRedirector.Sito_AgronicaLabQualita:
        return this.gestionePassaggioSitoAgronicaLabQualita(PaginaRichiesta, AggiungiSoloParametriAggiuntivi, ParametriAggiuntivi, objParametriAgenda, IDSezione);
      default:
        return this.funzioneNonImplementata_Promise();
    }
  }

  /** Use this function to correctly get path prefix when loading a page inside <app-iframe> tag*/
  public getPathPrefixGestionePassaggioAltroSitoIFrame(): Observable<string> {
    return this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.LinkAgronicaGiasNG).pipe(
      map(r => {
        let pathPrefix: string = '';
        if (window.location.hostname.toLowerCase() !== 'localhost') {
          if (r.Valore.includes(window.location.origin)) {
            let valoreSubstring: string = r.Valore.split(window.location.origin)[1];
            pathPrefix = valoreSubstring.slice(0, valoreSubstring.toLowerCase().indexOf('/gestionerichieste')).trim();
          } else {
            pathPrefix = r.Valore.slice(0, r.Valore.toLowerCase().indexOf('/gestionerichieste')).trim();
          }
        }
        return pathPrefix;
      })
    );
  }

  gestionePassaggioStessoSito_Aperto_in_Iframe(
    PaginaRichiesta: number,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    IDSezione: number = -1,
    iframetitle: string = '',
    iframeHideHeader: boolean = true,
    iframeheight: number = window.innerHeight * 0.9,
    iframewidth: number = window.innerWidth * 0.9
  ): Promise<GiasIFrameWindowService> {

    return new Promise<GiasIFrameWindowService>(async (resolve, reject) => {
      let title = '';

      if (iframetitle !== '')
        title = this.translocoService.translate(iframetitle);


      let resp = await this.gestionePassaggioStessoSito(PaginaRichiesta,
        null,
        IDSezione);

      if (iframeHideHeader) {
        resp += '?seFrame=1';
      }

      if (ParametriAggiuntivi) {
        if (iframeHideHeader) {
          resp += '&ParametriAggiuntivi=' + JSON.stringify(ParametriAggiuntivi);
        } else {
          resp += '?ParametriAggiuntivi=' + JSON.stringify(ParametriAggiuntivi);
        }
      }

      this.GiasIFrameWindowService.open({
        title: title,
        content: resp,
        height: iframeheight,
        width: iframewidth
      });

      resolve(this.GiasIFrameWindowService);
    });
  }

  gestionePassaggioAltroSito_Aperto_in_Iframe(SitoRichiesto: Enum_SiteRedirector,
    PaginaRichiesta: number,
    ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>,
    objParametriAgenda?: ObjParametriAgenda,
    AggiungiSoloParametriAggiuntivi: boolean = true,
    IDSezione: number = -1,
    iframetitle: string = '',
    iframeaggiungiQS: boolean = true,
    iframeheight: number = window.innerHeight * 0.9,
    iframewidth: number = window.innerWidth * 0.9,
  ): Promise<GiasIFrameWindowService> {

    return new Promise<GiasIFrameWindowService>(async (resolve, reject) => {
      let title = '';

      if (iframetitle !== '')
        title = this.translocoService.translate(iframetitle);

      if (iframeaggiungiQS) {
        if (!ParametriAggiuntivi)
          ParametriAggiuntivi = [];

        ParametriAggiuntivi.push({ key: 'Ifr', value: '1', codifica: true });
      }

      let resp = await this.gestionePassaggioAltroSito(SitoRichiesto,
        PaginaRichiesta,
        ParametriAggiuntivi,
        objParametriAgenda,
        AggiungiSoloParametriAggiuntivi,
        IDSezione);

      this.GiasIFrameWindowService.open({
        title: title,
        content: resp,
        height: iframeheight,
        width: iframewidth
      });

      resolve(this.GiasIFrameWindowService);
    });


  }

  /*gestioneRedirect_Old(): Promise<string> {
      return new Promise<string>(async (resolve, reject) => {

          var objPAgenda = this.objParametriAgendaService.getObjParamValue();

          const parametri: any = {
              objP_super_server: this.masterService.ObjParametri_Super_Server,
              objP_server: this.masterService.ObjParametri_Server,
              objP_utenti: this.masterService.ObjParametri_Utenti,
              VariabiliInSessione: this.masterService.variabiliInSessione,
              InData: objPAgenda
          };
          const R = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Utility.asmx/Redirect', parametri);
          if (R.RispostaOK) {
              R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
              resolve(R.RispostaStringa);
          } else {
              reject(R.Errore);
          }


      });
  }*/

  gestioneRedirect(): Promise<string> {
    return new Promise<string>(async (resolve, reject) => {

      var objPAgenda = this.objParametriAgendaService.getObjParamValue();

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/Redirect', {
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda
      }, true).pipe(map(R => {
        if (R.RispostaOK) {
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);
        } else {
          reject(R.Errore);
        }
      })).subscribe();
    });
  }

  funzioneNonImplementata_Promise(): Promise<any> {
    // return new Promise<string>(async (resolve, reject) => {
    //   reject('Funzione non ancora implementata');
    // });
    return Promise.reject('Funzione non ancora implementata');
  }

  /*gestioneStampe(report: number, ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>) {
      return new Promise<string>(async (resolve, reject) => {

          if(ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
              ParametriAggiuntivi = [];

          const objPAgenda = this.objParametriAgendaService.getObjParamValue();
          if (ParametriAggiuntivi == null) {
              ParametriAggiuntivi = [];
          }
          const parametri: any = {
              objP_super_server: this.masterService.ObjParametri_Super_Server,
              objP_server: this.masterService.ObjParametri_Server,
              objP_utenti: this.masterService.ObjParametri_Utenti,
              report: report,
              VariabiliInSessione: this.masterService.variabiliInSessione,
              InData: objPAgenda,
              ParametriAggiuntivi: ParametriAggiuntivi
          };
          const R = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Utility.asmx/GestioneStampe', parametri);
          R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
          resolve(R.RispostaStringa);

      });
  }*/

  gestioneStampe(report: number, ParametriAggiuntivi?: Array<ParametriAggiuntivi_QueryString>) {
    return new Promise<string>(async (resolve, reject) => {
      if (ParametriAggiuntivi === undefined || ParametriAggiuntivi === null)
        ParametriAggiuntivi = [];

      const objPAgenda = this.objParametriAgendaService.getObjParamValue();
      if (ParametriAggiuntivi == null) {
        ParametriAggiuntivi = [];
      }
      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('UtilityNG/GestioneStampe', {
        report: report,
        VariabiliInSessione: this.masterService.variabiliInSessione,
        InData: objPAgenda,
        ParametriAggiuntivi: ParametriAggiuntivi
      }, true).pipe(map(R => {
        R.RispostaStringa = this.getAbsolutePath(R.RispostaStringa);
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }


  getAbsolutePath(path: string): string {
    let retPath = '';
    const r = new RegExp('^(?:[a-z]+:)?//', 'i');
    if (r.test(path)) {
      retPath = path;
    } else {
      retPath = window.location.protocol + '//' + window.location.hostname + path;
    }
    return retPath;
  }

  goToFiltrino(sitoRedirect: number, paginaRedirect: number): Promise<string> {
    let objParametri_Agenda = new ObjParametriAgenda;

    // var impostazionePagRicerca = this.impostazioniUtenteService.getImpostazione_SuperUser(enum_Impostazioni_Utenti.SUPERUSER_Mod_Ricerca_Impresa);
    var modalitaFiltroRicerca = this.impostazioniUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_Mod_Filtro_Ricerca);

    if (!modalitaFiltroRicerca) {
      modalitaFiltroRicerca = new Utente_Impostazioni;
      modalitaFiltroRicerca.Impostazione_Cod = enum_Impostazioni_Utenti.SUPERUSER_Mod_Filtro_Ricerca;
      modalitaFiltroRicerca.Valore = '2';
    }

    // if (impostazionePagRicerca?.Valore == '1'){
    //   const parametriRedirect = [{
    //     key: 't',
    //     value: 'qwerty',
    //     codifica: true
    //   }, {
    //     key: 'sito',
    //     value: sitoRedirect.toString(),
    //     codifica: true
    //   }, {
    //     key: 'pagina',
    //     value: paginaRedirect.toString(),
    //     codifica: true
    //   },
    //     {
    //       key: 'd',
    //       value: paginaRedirect.toString(),
    //       codifica: true
    //     }]
    //   return this.gestionePassaggioSitoAgenda(enum_PagineAgenda_2010.Pagina_FiltrinoImprese, true, parametriRedirect,)
    // } else {
    if (modalitaFiltroRicerca?.Valore == '2') {

      let agenda = this.objParametriAgendaService.getObjParamValue();
      let paramFiltroRicercaNG = new ParametriFiltroRicercaNG;
      paramFiltroRicercaNG.TipoMostraGestitiChiamante = [Enum_TipoMostra_FiltroRicerca.Aziende];
      paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect = sitoRedirect;
      paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect = paginaRedirect;
      paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda;
      // agenda.GenericObj_string = Enum_TipoMostra_FiltroRicerca.Aziende.toString();
      agenda.GenericObj_string = JSON.stringify(paramFiltroRicercaNG);
      this.objParametriAgendaService.changeObjParametriAgenda(agenda);
      return this.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Filtro_Ricerca);

    } else {

      //WIP 20241217 DCA: da rimuovere in futuro
      return this.goToFiltrone(sitoRedirect, paginaRedirect);

    }
    // }
  }

  goToFiltrone(sitoRedirect: number, paginaRedirect: number): Promise<string> {

    let objParametri_Agenda = new ObjParametriAgenda;

    const parametriRedirect: ParametriAggiuntivi_QueryString[] = [
      {
        key: 's_o',
        value: Enum_SiteRedirector.GiasNG.toString(),
        codifica: true
      },
      {
        key: 'p_o',
        value: paginaRedirect.toString(),
        codifica: true
      },
      {
        key: 'p_d',
        value: paginaRedirect.toString(),
        codifica: true
      },
      {
        key: 's_d',
        value: Enum_SiteRedirector.GiasNG.toString(),
        codifica: true
      },
      {
        key: 't_f',
        value: '33',
        codifica: true
      },
      {
        key: 'c_s',
        value: '0',
        codifica: true
      },
      {
        key: 'cat',
        value: 'azienda',
        codifica: true
      },
      {
        key: 'f',
        value: '1',
        codifica: true
      }
    ]

    return this.gestionePassaggioSitoAgenda(enum_PagineAgenda_2010.Pagina_Filtrone, true, parametriRedirect,)

  }

  /*getLinkProfitosan_Old(){
      return new Promise<string>(async (resolve, reject) => {
          const parametri: any = {
              objP_super_server: this.masterService.ObjParametri_Super_Server,
              objP_server: this.masterService.ObjParametri_Server,
              objP_utenti: this.masterService.ObjParametri_Utenti
          };

          const R = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Utility.asmx/getLinkProfitosan', parametri);

          resolve(R.RispostaStringa);

      });
  }*/

  getLinkProfitosan() {
    return new Promise<string>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIGet<any, string>('UtilityNG/getLinkProfitosan', '', true).pipe(map(R => { resolve(R.RispostaStringa) })).subscribe();
    });
  }

}


export class KeyValuePair {

  public static Create(key: string, value: string, codifica: boolean = false): ParametriAggiuntivi_QueryString {
    return {
      key: key,
      value: value,
      codifica: codifica
    }
  }
}

