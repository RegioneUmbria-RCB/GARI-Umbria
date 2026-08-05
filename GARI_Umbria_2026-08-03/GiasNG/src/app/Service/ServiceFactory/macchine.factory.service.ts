import { Injectable, InjectionToken, Injector } from '@angular/core';
import { MacchinaKendoServerResult } from 'app/anagrafica/macchine/macchine.model';
import { CostoUnitario } from 'app/Model/anagrafiche/CostoUnitario';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { EditingMode, LoaderType } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable } from 'rxjs';
import { CoreWSRequest } from '../../Model/AppConfig';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard, rispostaStandard } from '../master.service';
import { ObjParametriAgendaService } from '../obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {Parametri_ObjParametriAgenda_NG} from '../api.service';

export class Aggiorna_Macchina_Edit_Request {
  macchina: ParcoMacchine;
  tipoOperazione: enum_TipoOperazioneDB;
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class Leggi_Macchine_Ditte_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class Leggi_Macchine_CmbTipo_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;

  cmbTipo: any;
  cmbDettaglio1: any;
  cmbDettaglio2: any;
  livello: number;
}

export class MachinesXTypeReadParams {
  parametriAgenda: Parametri_ObjParametriAgenda_NG
}

export const MACCHINE_SERVICE_TOKEN = new InjectionToken<MacchineFactoryService>('app.macchine.service');

@Injectable({ providedIn: 'root' })
export abstract class MacchineFactoryService {

  CostoUnitario: CostoUnitario[];

  protected ditte: Array<any>;
  protected tipo: Array<any>;

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {  }

  abstract leggiMacchine(objParametri: ObjParametriAgenda): Promise<RispostaStandard>;

  abstract readMachinesByClassCode(params: { parametriAgenda: ObjParametriAgenda, type: string, det1: string, det2: string }): Observable<RispostaStandard>;

  abstract leggiMacchina(objParametri: ObjParametriAgenda): Observable<rispostaStandard<ParcoMacchine>>;

  abstract aggiornaMacchina(macchina: ParcoMacchine, tipoOperazione: enum_TipoOperazioneDB): Promise<RispostaStandard>;

  abstract leggi_Ditte(): Promise<any[]>;

  abstract leggi_CmbTipoMacchina(Tipo: string, Dettaglio1: string, Dettaglio2: string, Livello: number): Promise<any[]>;

  abstract leggi_CmbTipoMacchina_Tutto(): Promise<any[]>;

  // abstract isMacchinaMovimentata(macchina: ParcoMacchine): Observable<rispostaStandard<boolean>>;

  isMacchinaMovimentata(macchina: ParcoMacchine): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ParcoMacchine, boolean>(
      'AnagraficaNG/IsMacchinaMovimentata',
      macchina
    );
  }

  isEditAllowed(macchina: ParcoMacchine): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ParcoMacchine, boolean>(
      'AnagraficaNG/IsEditAllowed',
      macchina
    );
  }

  centresOnWhichIsUsed(macchina: ParcoMacchine): Observable<rispostaStandard<number[]>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ParcoMacchine, number[]>(
      'AnagraficaNG/CentresOnWhichIsUsed',
      macchina
    );
  }

  companiesByWichIsUsed(macchina: ParcoMacchine): Observable<rispostaStandard<string[]>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ParcoMacchine, string[]>(
      'AnagraficaNG/CompaniesByWichIsUsed',
      macchina
    );
  }

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Macchine_Archivio_Lettura',
      objParametriAgenda);
  }

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Macchine_Archivio_Scrittura',
      objParametriAgenda)
  }

}
