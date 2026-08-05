import { Injectable, InjectionToken } from '@angular/core';
import { TranslocoPipe } from '@jsverse/transloco';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { MetodoProduzione } from 'app/Model/metaschema/MetodoProduzione';
import { TitoloDiPossesso } from 'app/Model/metaschema/TitoloDiPossesso';
import { JsonKendoResult } from 'gias-kendo-grid';
import { AnyTxtRecord } from 'dns';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";


export const CATASTO_SERVICE_TOKEN = new InjectionToken<CatastoFactoryService>('app.catasto.service');

export class ScriviCatasto{
    oldValue: CatastoCentroAziendale;
    newValue: CatastoCentroAziendale;
}

@Injectable()
export abstract class CatastoFactoryService {

    public metodiProduzione: MetodoProduzione[] = [
        { descrizione: 'Integrato', codice: 1},
        { descrizione: 'In Conversione', codice: 2},
        { descrizione: 'Biologico', codice: 3}
    ];

    public titoli_Di_Possesso: TitoloDiPossesso[] = [
        { descrizione: 'Altro', codice: 0},
        { descrizione: 'Proprietà', codice: 1},
        { descrizione: 'Comodato d\'uso', codice: 2},
        { descrizione: 'Affitto con contratto', codice: 3},
        { descrizione: 'Affitto senza contratto', codice: 4},
        { descrizione: 'In conto terzi', codice: 5},
        { descrizione: 'In convenzione', codice: 6},
        { descrizione: 'In compatercipazione', codice: 7}
    ];

    constructor(protected masterService: MasterService,
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected translocopipe: TranslocoPipe) {
    }

    public particellaEdit: CatastoCentroAziendale = new CatastoCentroAziendale();
    public particellaEditSource: BehaviorSubject<CatastoCentroAziendale> = new BehaviorSubject(this.particellaEdit);
    // currentParticellaEdit: Observable<CatastoCentroAziendale> = this.particellaEditSource.asObservable();

    abstract LeggiCatastoAziendaFromPiva(partitaIva: string, sa_cod: number, data: Date): Observable<JsonKendoResult>;

    abstract LeggiParticellaAzienda(catastoCentro: CatastoCentroAziendale): Observable<CatastoCentroAziendale>;

    abstract changeParticellaEdit(item: CatastoCentroAziendale);

    abstract getParticellaEdit(): CatastoCentroAziendale;

    abstract ScriviParticellaAzienda(oldValue: CatastoCentroAziendale, newValue: CatastoCentroAziendale): Observable<rispostaStandard<CatastoCentroAziendale>>

    abstract checkPossessi(catastoCentro: CatastoCentroAziendale): Observable<boolean>

    testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {

        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objParametriAgenda
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
            this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/Test_Catasto_Archivio_Lettura',
            parametri);
    }

    testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objParametriAgenda
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
            this.masterService.link_CoreWS + '/Anagrafica/Catasto.asmx/Test_Catasto_Archivio_Scrittura',
            parametri)
    }

    public ultimaParticellaInserita: {
        sa_cod:number,
        sa_des:string,
        prov: string,
        prov_des:string,
        com: string,
        com_des:string,
        sezione: string,
        foglio: number
    } = null;

}
