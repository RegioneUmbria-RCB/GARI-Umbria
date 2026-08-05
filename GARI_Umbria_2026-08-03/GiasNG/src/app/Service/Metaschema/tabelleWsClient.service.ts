import { Injectable } from "@angular/core";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { FiltroValoriParametriQualitativi } from "app/Model/filtri/filtroValoriParametriQualitativi";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MasterService } from "../master.service";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { map } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class TabelleWsClientService{
    valoriParamQualitativiList = [];

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*public RicercaValoriParametriQualitativi_Old(Tabella_ID, piva): Promise<any[]> {
        return new Promise<any[]>(async (resolve, reject) => {
            var risultato_lettura_FF;
            var found = false;

            if (this.valoriParamQualitativiList !== undefined && this.valoriParamQualitativiList.length > 0) {
                for (var iValParam = 0; iValParam < this.valoriParamQualitativiList.length; iValParam++) {
                    var vpq = this.valoriParamQualitativiList[iValParam];
                    if (vpq.Tabella_ID == Tabella_ID) {
                        risultato_lettura_FF = vpq.Dati;
                        found = true;
                        break;
                    }
                }
            }

            if (!found) {
                let impresa = new Impresa()
                impresa.partitaIva = piva;
                let parametri: CoreWS_Generic<FiltroValoriParametriQualitativi> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { impresa: impresa, tabella: Tabella_ID }
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<BaseCodeDescr[], FiltroValoriParametriQualitativi>(
                    this.masterService.link_CoreWS + "/FreshAndFood/FreshAndFood.asmx/LeggiValoriParametriQualitativi_Modello",
                    parametri,
                    false);

                risultato_lettura_FF = R.RispostaStringa;
                this.valoriParamQualitativiList.push({ Tabella_ID: Tabella_ID, Dati: risultato_lettura_FF });
            }
            resolve(risultato_lettura_FF);

        });
    }*/

    public RicercaValoriParametriQualitativi(Tabella_ID, piva): Promise<any[]> {
        return new Promise<any[]>(async (resolve, reject) => {
            var risultato_lettura_FF;
            var found = false;

            if (this.valoriParamQualitativiList !== undefined && this.valoriParamQualitativiList.length > 0) {
                for (var iValParam = 0; iValParam < this.valoriParamQualitativiList.length; iValParam++) {
                    var vpq = this.valoriParamQualitativiList[iValParam];
                    if (vpq.Tabella_ID == Tabella_ID) {
                        risultato_lettura_FF = vpq.Dati;
                        found = true;
                        break;
                    }
                }
            }

            if (!found) {
                let impresa = new Impresa()
                impresa.partitaIva = piva;

                this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroValoriParametriQualitativi, BaseCodeDescr[]>(
                    "FreshAndFood/LeggiValoriParametriQualitativi_Modello",
                    { impresa: impresa, tabella: Tabella_ID },
                    false).pipe(map(R => {
                        risultato_lettura_FF = R.RispostaStringa;
                        this.valoriParamQualitativiList.push({ Tabella_ID: Tabella_ID, Dati: risultato_lettura_FF });
                        resolve(risultato_lettura_FF);
                })).subscribe();

            }
            resolve(risultato_lettura_FF);

        });
    }
}
