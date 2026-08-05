import { Injectable } from '@angular/core';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { from, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';

@Injectable({
    providedIn: 'root'
})
export class QualitaCatastoService {
    private QualitaCatasto: BaseCodeDescr[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(): Observable<BaseCodeDescr[]> {
        if (this.QualitaCatasto == undefined || this.QualitaCatasto.length == 0) {
            const parametri: CoreWS_Generic<BaseCodeDescr> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                new BaseCodeDescr(0)
            );

            return (this.ajaxAgronicaService.ajaxCoreWSPost<BaseCodeDescr, BaseCodeDescr[]>(
                this.masterService.link_CoreWS + '/Metaschema/QualitaCatasto.asmx/Leggi',
                parametri,
                false).pipe(map((risp) => {
                this.QualitaCatasto = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.QualitaCatasto);
        }
    }*/

    leggi(): Observable<BaseCodeDescr[]> {
        if (this.QualitaCatasto == undefined || this.QualitaCatasto.length == 0) {
            return (this.ajaxAgronicaAPIService.ajaxAPIGet<BaseCodeDescr, BaseCodeDescr[]>(
                'MetaschemaNG/LeggiQualitaCatasto',
                new BaseCodeDescr(0),
                false).pipe(map((risp) => {
                    this.QualitaCatasto = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.QualitaCatasto);
        }
    }

}
