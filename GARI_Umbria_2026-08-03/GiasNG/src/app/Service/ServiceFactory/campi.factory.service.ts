import { Injectable, InjectionToken } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { CampiKendoServerResult } from 'app/anagrafica/campi/campi.model';
import { Campo, PKCampo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { JsonKendoResult, KendoGridColumn } from 'gias-kendo-grid';
import { Observable } from 'rxjs/internal/Observable';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard, rispostaStandard } from '../master.service';
import { SpecieVegetaliService } from '../Metaschema/specie-vegetali.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {LeggiInvestimentoCatastaleCampo} from '../../anagrafica/campi/investimento-catastale-campo/investimento-catastale-campo.service';
import {Specie} from '../../Model/metaschema/utilizzi/Specie';

export class  LeggiCampi {
    centro: CentroAziendale
    data: Date;
}

export const CAMPI_SERVICE_TOKEN = new InjectionToken<CampiFactoryService>('app.campi.service');


@Injectable()
export abstract class CampiFactoryService {

    public setDates(column: KendoGridColumn): void {
        if(column.field === 'Validita_Inizio') {
            column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
        }
        if(column.field === 'Validita_Fine') {
            column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
        }
        if(column.field === 'Data_Creazione') {
            column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
        }
        if(column.field === 'Data_Modifica') {
            column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
        }
    }
    public setNumericFields(column: KendoGridColumn): void {
        if(column.field === 'Superficie_Totale') {
            column.numeric = { defaultValue: 1, min: 0, max: 10 };
        }
        if(column.field === 'Superficie_Biologico') {
            column.numeric = { defaultValue: 1, min: 0, max: 10 };
        }
        if(column.field === 'Superficie_Convenzionale') {
            column.numeric = { defaultValue: 1, min: 0, max: 10 };
        }
        if(column.field === 'Superficie_Conversione') {
            column.numeric = { defaultValue: 1, min: 0, max: 10 };
        }
        if(column.field === 'Superficie_Catastale') {
            column.numeric = { defaultValue: 1, min: 0, max: 10 };
        }

    }

    constructor(protected masterService: MasterService,
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        protected translocoService: TranslocoService,
        protected specievegetaliservice: SpecieVegetaliService) { }

    abstract LeggiCampi(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string);

    abstract getCampi(): CampiKendoServerResult;

    abstract LeggiCampiAnagrafica(objParametriAgenda: ObjParametriAgenda): Observable<any[]>;

    abstract leggiCampo(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Campo>>;

    abstract leggiAppezzamentiCampo(objParametri: ObjParametriAgenda);

    abstract leggiParticellePerCentro(objParametri: ObjParametriAgenda): Observable<rispostaStandard<any>>;

    abstract leggi_SpecieVegetali(): Promise<Specie[]>;

    abstract leggiCampiCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]>;

    abstract leggiInvestimentoCatastale(filtro: LeggiInvestimentoCatastaleCampo): Observable<any>;

    abstract setupLeggiCampi(agenda: ObjParametriAgenda);

    abstract aggiornaCampo(campo: Campo, tipoOperazione: enum_TipoOperazioneDB): Promise<RispostaStandard>;

    abstract aggiornaCampoObs(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, deleteRibaltamento?: boolean);

    abstract aggiornaCampo_inLine(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, agenda: ObjParametriAgenda): Observable<RispostaStandard>;

    /*testLettura_Old(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {

        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objParametriAgenda
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
            this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Test_Campi_Archivio_Lettura',
            parametri);
    }*/

    testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
            'AnagraficaNG/Test_Campi_Archivio_Lettura',
            objParametriAgenda);
    }

    /*testScrittura_Old(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objParametriAgenda
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
            this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Test_Campi_Archivio_Scrittura',
            parametri)
    }*/

    testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
            'AnagraficaNG/Test_Campi_Archivio_Scrittura',
            objParametriAgenda)
    }

}
