import { ThisReceiver } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { TranslocoPipe } from '@jsverse/transloco';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { MetodoProduzione } from 'app/Model/metaschema/MetodoProduzione';
import { TitoloDiPossesso } from 'app/Model/metaschema/TitoloDiPossesso';
import { JsonKendoResult } from 'gias-kendo-grid';
import { take } from 'rxjs';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { CatastoFactoryService } from '../ServiceFactory/catasto.factory.service';
import {AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";


export class ScriviCatasto{
  oldValue: CatastoCentroAziendale;
  newValue: CatastoCentroAziendale;
}

@Injectable({ providedIn: 'root' })
export class CatastoService extends CatastoFactoryService {

  constructor(protected masterService: MasterService,
              protected ajaxAgronicaService: AjaxAgronicaService,
              protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
              protected translocopipe: TranslocoPipe) {
    super(masterService, ajaxAgronicaService, translocopipe);
  }


  // currentParticellaEdit: Observable<CatastoCentroAziendale> = this.particellaEditSource.asObservable();

  /*LeggiCatastoAzienda_Old(partitaIva: string): Observable<JsonKendoResult> {
      const objPAgenda = new ObjParametriAgenda;
      objPAgenda.Piva = partitaIva;

      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objPAgenda
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any, ObjParametriAgenda>(
          this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/Leggi_Catasto_Anagrafica',
          parametri).pipe(map((data) => {
              return <JsonKendoResult>data.RispostaStringa;
          }
          ));
  }*/

  LeggiCatastoAziendaFromPiva(partitaIva: string, sa_cod: number, data: Date = AGRODATAINIZIO): Observable<JsonKendoResult> {
    const objPAgenda = new ObjParametriAgenda;
    objPAgenda.Piva = partitaIva;
    objPAgenda.Sa_Cod = sa_cod;
    objPAgenda.Data = data;
    return this.LeggiCatastoAzienda(objPAgenda);
  }

  LeggiCatastoAzienda(objPAgenda: ObjParametriAgenda): Observable<JsonKendoResult> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>(
      'AnagraficaNG/LeggiCatastoAnagrafica',
      objPAgenda
    ).pipe(map((data) => <JsonKendoResult>data.RispostaStringa));
  }


  /*LeggiParticellaAzienda_Old(catastoCentro: CatastoCentroAziendale): Observable<CatastoCentroAziendale> {
      const parametri: CoreWS_Generic<CatastoCentroAziendale> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: catastoCentro
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<CatastoCentroAziendale, CatastoCentroAziendale>(
          this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/Leggi_Particella_Anagrafica',
          parametri).pipe(map((data) => data.RispostaStringa));
  }*/

  LeggiParticellaAzienda(catastoCentro: CatastoCentroAziendale): Observable<CatastoCentroAziendale> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<CatastoCentroAziendale, CatastoCentroAziendale>('AnagraficaNG/LeggiParticellaAnagrafica',
      catastoCentro).pipe(map((data) => data.RispostaStringa));
  }

  changeParticellaEdit(item: CatastoCentroAziendale) {
    this.particellaEditSource.next(item);
  }

  getParticellaEdit(): CatastoCentroAziendale {
    return this.particellaEditSource.getValue();
  }

  /*ScriviParticellaAzienda_Old(oldValue: CatastoCentroAziendale, newValue: CatastoCentroAziendale): Observable<rispostaStandard<CatastoCentroAziendale>> {
      const parametri: CoreWS_Generic<ScriviCatasto> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: {
              oldValue: oldValue,
              newValue: newValue
          }
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<CatastoCentroAziendale, ScriviCatasto>(
          this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/Scrivi_Particella_Anagrafica',
          parametri,true,false).pipe(map((data) => {
              return data
          })
      );
  }*/

  ScriviParticellaAzienda(oldValue: CatastoCentroAziendale, newValue: CatastoCentroAziendale): Observable<rispostaStandard<CatastoCentroAziendale>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviCatasto, CatastoCentroAziendale>('AnagraficaNG/ScriviParticellaAnagrafica', {
      oldValue: oldValue,
      newValue: newValue
            }, false, false, false, false).pipe(map((data) => {
        return data
      })
    );
  }

  /*checkPossessi_Old(catastoCentro: CatastoCentroAziendale): Observable<boolean> {
      const parametri: CoreWS_Generic<CatastoCentroAziendale> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: catastoCentro
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, CatastoCentroAziendale>(
          this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/CheckPossessi',
          parametri).pipe(map((data) => {
              return data.RispostaStringa
          })
      );
  }*/

  checkPossessi(catastoCentro: CatastoCentroAziendale): Observable<boolean> {

    return this.ajaxAgronicaAPIService.ajaxAPIPost<CatastoCentroAziendale, boolean>('AnagraficaNG/CheckPossessi',
      catastoCentro).pipe(map((data) => {
        return data.RispostaStringa
      })
    );
  }
}
