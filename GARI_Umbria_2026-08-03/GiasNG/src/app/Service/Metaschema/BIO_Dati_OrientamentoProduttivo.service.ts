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
export class BIO_Dati_OrientamentoProduttivoService {
    private BIO_Dati_OrientamentoProduttivo: BaseCodeDescr[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(): Observable<BaseCodeDescr[]> {
        if (this.BIO_Dati_OrientamentoProduttivo == undefined || this.BIO_Dati_OrientamentoProduttivo.length == 0) {
            const parametri: CoreWS_Generic<object> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                {}
            );

            return (this.ajaxAgronicaService.ajaxCoreWSPost<object, BaseCodeDescr[]>(
                this.masterService.link_CoreWS + '/Metaschema/Biologico.asmx/Leggi_BIO_Dati_OrientamentoProduttivo',
                parametri,
                false).pipe(map((risp) => {
                this.BIO_Dati_OrientamentoProduttivo = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.BIO_Dati_OrientamentoProduttivo);
        }
    }*/

    leggi(): Observable<BaseCodeDescr[]> {
        if (this.BIO_Dati_OrientamentoProduttivo == undefined || this.BIO_Dati_OrientamentoProduttivo.length == 0) {
            return (this.ajaxAgronicaAPIService.ajaxAPIGet<object, BaseCodeDescr[]>(
                'MetaschemaNG/LeggiBIODatiOrientamentoProduttivo',
                {},
                false).pipe(map((risp) => {
                this.BIO_Dati_OrientamentoProduttivo = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.BIO_Dati_OrientamentoProduttivo);
        }
    }


}
