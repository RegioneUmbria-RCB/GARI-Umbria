import {Injectable} from '@angular/core';
import {AjaxAgronicaAPIService} from '../../../../Service/ajax-agronica.api.service';
import {VerificaVicini_In, VerificaVicini_Out} from '../../../../Service/api.service';
import {Observable} from 'rxjs';
import {rispostaStandard} from '../../../../Service/master.service';

@Injectable()
export class SalvataggioAppezzamentoGISService {

    constructor(
        private ajaxApiService: AjaxAgronicaAPIService
    ) {  }

    public verificaVicini(vv: VerificaVicini_In): Observable<rispostaStandard<VerificaVicini_Out>> {
        return this.ajaxApiService.ajaxAPIPost<VerificaVicini_In, VerificaVicini_Out>(
            'Gis/VerificaVicini',
            vv,
            false,
            true
        );
    }
}
