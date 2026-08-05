import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { Copertura } from 'app/Model/metaschema/Copertura';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class CoperturaxSpecie {
    Specie: Specie;
    Copertura: Copertura[];
}

export class LeggiCopertura{
    specie: Specie;
}

@Injectable({
    providedIn: 'root'
})
export class CoperturaService {
    private CoperturaxSpecie: CoperturaxSpecie[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(specie: Specie): Promise<Copertura[]> {
        return new Promise<Copertura[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<Copertura>());
            }
            if (this.CoperturaxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                const parametri: CoreWS_Generic<LeggiCopertura> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { specie: specie }
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Copertura[], LeggiCopertura>(
                    this.masterService.link_CoreWS + '/Metaschema/Copertura.asmx/CaricaComboCopertura_Modello',
                    parametri,
                    false);

                this.CoperturaxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                resolve(this.CoperturaxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            } else {

                resolve(this.CoperturaxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }*/

    leggi(specie: Specie): Promise<Copertura[]> {
        return new Promise<Copertura[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<Copertura>());
            }
            if (this.CoperturaxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCopertura, Copertura[]>(
                    'MetaschemaNG/CaricaComboCoperturaModello',
                    { specie: specie },
                    false).pipe(map(R => {
                        this.CoperturaxSpecie.push({ Specie: specie, Copertura: R.RispostaStringa });
                        resolve(this.CoperturaxSpecie.find((el) => {
                            if (el.Specie.codice == specie.codice) {
                                return el;
                            }
                        }).Copertura);
                    })).subscribe();
            } else {

                resolve(this.CoperturaxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Copertura);

            }
        });

    }

}
