import { Injectable } from "@angular/core";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { RegolamentoConcimazione } from "app/Model/metaschema/RegolamentoConcimazione";
import { Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MasterService } from "../master.service";


@Injectable({
  providedIn: 'root'
})
export class PuaService {
  private RegolamentiConcimazioni: RegolamentoConcimazione[] = new Array();

  constructor(private ajaxAgronicaService: AjaxAgronicaService,
              private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
              private masterService: MasterService) { }

  /*leggiRegolamentoConcimazionexImpianto_Old(validita: IntervalloTemporale): Observable<RegolamentoConcimazione[]> {
      //if (this.RegolamentiConcimazioni == undefined || this.RegolamentiConcimazioni.length == 0) {
        let parametri: CoreWS_Generic<IntervalloTemporale> = new CoreWS_Generic(
            this.masterService.getCoreWSGenericObjP(),
            { inizio: validita.inizio, fine: validita.fine }
        );
        return (this.ajaxAgronicaService.ajaxCoreWSPost<IntervalloTemporale, RegolamentoConcimazione[]>(
              this.masterService.link_CoreWS + "/Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_RegolamentixEditImpianto_Modello",
              parametri,
              false).pipe(map((risp) => {
                  this.RegolamentiConcimazioni = risp.RispostaStringa;
                  return (risp.RispostaStringa);
              })));
    //   } else {
    //     return of(this.RegolamentiConcimazioni)
    //   }
  }*/

  leggiRegolamentoConcimazionexImpianto(validita: IntervalloTemporale): Observable<RegolamentoConcimazione[]> {
    return (this.ajaxAgronicaAPIService.ajaxAPIPost<IntervalloTemporale, RegolamentoConcimazione[]>(
      "MetaschemaNG/LeggiPUARegolamentixEditImpiantoModello",
      new IntervalloTemporale(validita.inizio, validita.fine),
      false).pipe(map((risp) => {
      this.RegolamentiConcimazioni = risp.RispostaStringa;
      return (risp.RispostaStringa);
    })));
  }
}
