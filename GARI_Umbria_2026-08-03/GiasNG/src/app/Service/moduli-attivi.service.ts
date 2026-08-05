import { Injectable } from "@angular/core";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { SessionStorageService } from "ngx-webstorage";
import { map, Observable, of, tap } from "rxjs";
import { AjaxAgronicaService } from "./ajax-agronica.service";
import { MasterService } from "./master.service";
import { AjaxAgronicaAPIService } from "./ajax-agronica.api.service";

export class ModuliGias {
    Modulo_Cantine: boolean;
    Modulo_FreshFood: boolean;
    Modulo_Tabacco: boolean;
    Modulo_Zoo: boolean;
}

export const ModuliAttiviStoreKey: string = 'ModuliGiasAttivi';

@Injectable({ providedIn: 'root' })
export class ModuliAttivi {
    ImpresexModuli: { Piva: string, moduli: ModuliGias }[] = null;


    constructor(private masterService: MasterService,
        private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private sessionSt: SessionStorageService) { }

    caricaModuli(Piva: string): Observable<ModuliGias> {

        if (this.ImpresexModuli == null) {
            if (this.sessionSt.retrieve(ModuliAttiviStoreKey) != null) {
                this.ImpresexModuli = this.sessionSt.retrieve(ModuliAttiviStoreKey);
            } else {
                this.ImpresexModuli = [];
            }
        }

        const filteredObj = this.ImpresexModuli.filter((el) => el.Piva == Piva);

        if (filteredObj.length > 0) {

            return of(filteredObj[0].moduli);

        } else {

            return this.fetchModuli(Piva).pipe(
                tap((el) => {

                    this.ImpresexModuli.push({
                        Piva: Piva,
                        moduli: el
                    });

                    this.sessionSt.store(ModuliAttiviStoreKey, this.ImpresexModuli);

                })
            );

        }
    }

    getModuli(Piva: string): ModuliGias {

        const filteredObj = this.ImpresexModuli.filter((el) => el.Piva == Piva);

        if (filteredObj.length > 0) {

            return filteredObj[0].moduli;

        } else {

            throw Error('getModuli non trovato per Piva:' + Piva);

        }

    }

    getModuliAll(): { Piva: string, moduli: ModuliGias }[] {
        return this.ImpresexModuli;
    }

    /*private fetchModuli_Old(Piva: string): Observable<ModuliGias>{

        const parametri: CoreWS_Generic<string> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: Piva
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<ModuliGias, string>(
            this.masterService.link_CoreWS + '/Utility.asmx/getModuli', parametri).pipe(
                map((el) => el.RispostaStringa)
        );

    }*/

    private fetchModuli(Piva: string): Observable<ModuliGias>{

        return this.ajaxAgronicaAPIService.ajaxAPIPost<string, ModuliGias>(
            'UtilityNG/getModuli', Piva).pipe(
                map((el) => 
                el.RispostaStringa)
        );

    }

}
