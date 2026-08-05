import { Injectable } from "@angular/core";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { FiltroCalcoloNPK } from "app/Model/filtri/filtroCalcoloNPK";
import { FiltroPC_Finalita_Rer } from "app/Model/filtri/filtroPC_Finalita_Rer";
import { ApportoMacroelementi } from "app/Model/metaschema/ApportoMacroelementi";
import { FinalitaPianoConcimazione } from "app/Model/metaschema/FinalitaPianoConcimazione";
import { Regolamenti } from "app/Model/metaschema/Regolamenti";
import { RegolamentoConcimazione } from "app/Model/metaschema/RegolamentoConcimazione";
import { GruppoFinalita } from "app/Model/metaschema/utilizzi/GruppoFinalita";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { resolve } from "dns";
import { Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MasterService } from "../master.service";

@Injectable({
  providedIn: 'root'
})
export class PianoConcimazioneService {
  constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

  leggiRegolamentoConcimazionexImpianto(regolamento: Regolamenti, specie: Specie, finalita: GruppoFinalita): Observable<FinalitaPianoConcimazione[]> {

    if (regolamento == null || regolamento.codice == 0 ||specie == null || specie.codice == 0 ||finalita == null || finalita.codice == 0) {
      return of([]);
    }

    return (this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroPC_Finalita_Rer, FinalitaPianoConcimazione[]>(
      "AgronicaCoreDPING/PCFinalitaRerWSModello",
      {
        regolamento: regolamento,
        specie: specie,
        finalita: finalita
      },
      false).pipe(map((risp) => {
      return (risp.RispostaStringa);
    })));
  }

  calcoloNPKModello(filtro: FiltroCalcoloNPK): Observable<ApportoMacroelementi>{
    if (filtro.regolamento == null || filtro.regolamento.codice == 0 ||
      filtro.specie == null || filtro.specie.codice == 0 ||
      filtro.finalita == null || filtro.finalita.codice == 0 ||
      filtro.stato == null || filtro.stato.codice == 0) {
      return of(new ApportoMacroelementi);
    }

    return (this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroCalcoloNPK, ApportoMacroelementi>(
      "AgronicaCoreDPING/CalcoloNPKModello",
      filtro,
      false).pipe(map((risp) => {
      return (risp.RispostaStringa);
    })));
  }

}

