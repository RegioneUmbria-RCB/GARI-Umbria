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
export class ZoneService {
    private Zone: BaseCodeDescr[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(): Observable<BaseCodeDescr[]> {
        if (this.Zone == undefined || this.Zone.length == 0) {
            const parametri: CoreWS_Generic<BaseCodeDescr> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                new BaseCodeDescr(0)
            );

            return (this.ajaxAgronicaService.ajaxCoreWSPost<BaseCodeDescr, BaseCodeDescr[]>(
                this.masterService.link_CoreWS + '/Metaschema/Zone.asmx/Leggi',
                parametri,
                false).pipe(map((risp) => {
                this.Zone = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.Zone);
        }
    }*/

    leggi(): Observable<BaseCodeDescr[]> {
        if (this.Zone == undefined || this.Zone.length == 0) {

            return (this.ajaxAgronicaAPIService.ajaxAPIPost<BaseCodeDescr, BaseCodeDescr[]>(
                'MetaschemaNG/LeggiZone',
                new BaseCodeDescr(0),
                false).pipe(map((risp) => {
                this.Zone = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.Zone);
        }
    }

}
