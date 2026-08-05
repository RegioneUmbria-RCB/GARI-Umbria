import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Macrouso } from 'app/Model/metaschema/Macrouso';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { from, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';

@Injectable({
    providedIn: 'root'
})
export class MacrousiService {
    private Macrousi: Macrouso[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(): Observable<Macrouso[]> {
        if (this.Macrousi == undefined || this.Macrousi.length == 0) {
            const parametri: CoreWS_Generic<Macrouso> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                new Macrouso('')
            );
            return (this.ajaxAgronicaService.ajaxCoreWSPost<Macrouso, Macrouso[]>(
                this.masterService.link_CoreWS + '/Metaschema/Macrousi.asmx/Leggi',
                parametri,
                false).pipe(map((risp) => {
                this.Macrousi = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.Macrousi);
        }
    }*/

    leggi(): Observable<Macrouso[]> {
        if (this.Macrousi == undefined || this.Macrousi.length == 0) {
            return (this.ajaxAgronicaAPIService.ajaxAPIPost<Macrouso, Macrouso[]>(
                'MetaschemaNG/LeggiMacrousi',
                new Macrouso(''),
                false).pipe(map((risp) => {
                this.Macrousi = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
        } else {
            return of(this.Macrousi);
        }
    }

}
