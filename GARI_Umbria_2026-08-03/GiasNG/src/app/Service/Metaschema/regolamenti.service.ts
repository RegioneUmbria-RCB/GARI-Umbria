import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { Macrouso } from 'app/Model/metaschema/Macrouso';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
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
export class RegolamentiService {
  private Regolamenti: Regolamenti[] = new Array();

  constructor(private ajaxAgronicaService: AjaxAgronicaService,
              private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
              private masterService: MasterService) { }

  /*leggi_Old(): Observable<Regolamenti[]> {
      if (this.Regolamenti == undefined || this.Regolamenti.length == 0) {
        let parametri: CoreWS_Generic<object> = new CoreWS_Generic(
              this.masterService.getCoreWSGenericObjP(),
            {}
        );
        return (this.ajaxAgronicaService.ajaxCoreWSPost<object, Regolamenti[]>(
              this.masterService.link_CoreWS + "/Metaschema/Regolamenti.asmx/CaricaComboRegolamenti_Modello",
              parametri,
              false).pipe(map((risp) => {
                  this.Regolamenti = risp.RispostaStringa;
                  return (risp.RispostaStringa);
              })));
      } else {
        return of(this.Regolamenti)
      }
  }*/

  leggi(): Observable<Regolamenti[]> {
    if (this.Regolamenti == undefined || this.Regolamenti.length == 0) {

      return (this.ajaxAgronicaAPIService.ajaxAPIGet<object, Regolamenti[]>(
            'MetaschemaNG/CaricaComboRegolamentiModello',
            {},
            false).pipe(map((risp) => {
                this.Regolamenti = risp.RispostaStringa;
                return (risp.RispostaStringa);
            })));
    } else {
      return of(this.Regolamenti)
    }
}

}
