import {Injectable} from '@angular/core';
import {CostoUnitario} from 'app/Model/anagrafiche/CostoUnitario';
import {ParcoMacchine} from 'app/Model/anagrafiche/ParcoMacchine';
import {enum_TipoOperazioneDB} from 'app/Model/TipiEnumerativi';
import {lastValueFrom, map, Observable} from 'rxjs';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {AjaxAgronicaService} from '../ajax-agronica.service';
import {MasterService, RispostaStandard, rispostaStandard} from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {MacchineFactoryService} from '../ServiceFactory/macchine.factory.service';


@Injectable({providedIn: 'root'})
export class MacchineService extends MacchineFactoryService {

  CostoUnitario: CostoUnitario[];

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {
    super(masterService, ajaxAgronicaService, ajaxAgronicaAPIService)
  }

  leggiMacchine(objParametri: ObjParametriAgenda) {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('AnagraficaNG/LeggiMacchineAnagrafica', objParametri));
  }

  readMachinesByClassCode(params: { parametriAgenda: ObjParametriAgenda, type: string, det1: string, det2: string }): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('AnagraficaNG/ReadMachinesByClassCode', params);
  }

  leggiMacchina(objParametri: ObjParametriAgenda): Observable<rispostaStandard<ParcoMacchine>> {

    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, ParcoMacchine>(
      'AnagraficaNG/LeggiMacchinaAnagrafica',
      objParametri);
  }

  aggiornaMacchina(macchina: ParcoMacchine, tipoOperazione: enum_TipoOperazioneDB) {
    return new Promise<RispostaStandard>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('AnagraficaNG/ScriviMacchinaAnagrafica', {
        macchina: macchina,
        tipoOperazione: tipoOperazione
      }).pipe(map(r => {
        resolve(r);
      })).subscribe();
    });
  }

  leggi_Ditte() {
    return new Promise<Array<any>>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIGet<any, any[]>('MetaschemaNG/LeggiDitte', "").pipe(map(r => {
        this.ditte = r.RispostaStringa;
        resolve(this.ditte);
      })).subscribe();
    });
  }

  leggi_CmbTipoMacchina(Tipo: string, Dettaglio1: string, Dettaglio2: string, Livello: number) {
    return new Promise<Array<any>>(async (resolve, reject) => {

      this.ajaxAgronicaAPIService.ajaxAPIPost<any, any[]>('MetaschemaNG/LeggiTipo', {
        cmbTipo: Tipo,
        cmbDettaglio1: Dettaglio1,
        cmbDettaglio2: Dettaglio2,
        livello: Livello
      }).pipe(map(r => {
        this.tipo = r.RispostaStringa;
        if (Livello == 1) {
          let index = this.tipo.findIndex((el) => el.codice == '');
          if (index >= 0) {
            this.tipo.splice(index, 1);
          }
        }
        resolve(this.tipo);
      })).subscribe();
    });
  }

  leggi_CmbTipoMacchina_Tutto(): Promise<Array<any>> {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIGet<any, any[]>(
        'MetaschemaNG/LeggiTutteMacchine', ""
      ).pipe(map(r => {
        this.tipo = r.RispostaStringa;
        return this.tipo;
      }))
    );
  }

  // isMacchinaMovimentata(macchina: ParcoMacchine): Observable<rispostaStandard<boolean>> {
  //   return this.ajaxAgronicaAPIService.ajaxAPIPost<ParcoMacchine, boolean>(
  //     'AnagraficaNG/IsMacchinaMovimentata',
  //     macchina
  //   );
  // }

}
