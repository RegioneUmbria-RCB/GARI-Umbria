import { Injectable } from "@angular/core";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { Disciplinare } from "app/Model/metaschema/Disciplinari";
import { Epoca } from "app/Model/metaschema/Epoca";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { map } from "rxjs";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MasterService } from "../master.service";

export class LeggiEpoche {
    lavorazione: Lavorazione;

    specie: Specie;

    disciplinare: Disciplinare;
}

@Injectable({
    providedIn: "root",
})
export class EpocheService {
    constructor(
        private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService
    ) {}

    /*Leggi_EpocheDPI_QdC_Old(p: LeggiEpoche) {
        return new Promise<Epoca[]>(async (resolve, reject) => {
            const parametri: CoreWS_Generic<LeggiEpoche> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<
                Epoca[],
                LeggiEpoche
            >(
                this.masterService.link_CoreWS +
                    "/Metaschema/Epoche.asmx/Leggi_EpocheDPI_QdC",
                parametri
            );

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_EpocheDPI_QdC(p: LeggiEpoche) {
        return new Promise<Epoca[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<
                LeggiEpoche,    
                Epoca[]
            >(
                "MetaschemaNG/LeggiEpocheDPIQdC",
                p
            ).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }

    /*Leggi_EpocheFertilizzazione_QdC_Old(p: LeggiEpoche) {
        return new Promise<Epoca[]>(async (resolve, reject) => {
            const parametri: CoreWS_Generic<LeggiEpoche> = new CoreWS_Generic(
                this.masterService.getCoreWSGenericObjP(),
                p
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<
                Epoca[],
                LeggiEpoche
            >(
                this.masterService.link_CoreWS +
                    "/Metaschema/Epoche.asmx/Leggi_EpocheFertilizzazione_QdC",
                parametri
            );

            resolve(R.RispostaStringa);
        });
    }*/

    Leggi_EpocheFertilizzazione_QdC(p: LeggiEpoche) {
        return new Promise<Epoca[]>(async (resolve, reject) => {
            this.ajaxAgronicaAPIService.ajaxAPIPost<
                LeggiEpoche,
                Epoca[]
            >(
                    "MetaschemaNG/LeggiEpocheFertilizzazioneQdC",
                p
            ).pipe(map(R => {
                resolve(R.RispostaStringa);
            })).subscribe();
        });
    }
}
