import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { Portinnesto } from 'app/Model/metaschema/DensitaImpianto/Portinnesto';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class PortinnestoxSpecie {
    Specie: Specie;
    Portinnesto: Portinnesto[];
}

export class LeggiPortinnesto{
    specie: Specie;
}

@Injectable({
    providedIn: 'root'
})
export class PortinnestoService {
    private PortinnestoxSpecie: PortinnestoxSpecie[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(specie: Specie): Promise<Portinnesto[]> {
        return new Promise<Portinnesto[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<Portinnesto>());
            }
            if (this.PortinnestoxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                const parametri: CoreWS_Generic<LeggiPortinnesto> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { specie: specie }
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Portinnesto[], LeggiPortinnesto>(
                    this.masterService.link_CoreWS + '/Metaschema/Portinnesti.asmx/CaricaComboPortinnesti_Modello',
                    parametri,
                    false);

                this.PortinnestoxSpecie.push({ Specie: specie, Portinnesto: R.RispostaStringa });
                resolve(this.PortinnestoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Portinnesto);

            } else {

                resolve(this.PortinnestoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Portinnesto);

            }
        });

    }*/

    leggi(specie: Specie): Promise<Portinnesto[]> {
        return new Promise<Portinnesto[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<Portinnesto>());
            }
            if (this.PortinnestoxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiPortinnesto, Portinnesto[]>(
                    'MetaschemaNG/CaricaComboPortinnestiModello',
                    { specie: specie },
                    false).pipe(map(R => {
                                        this.PortinnestoxSpecie.push({ Specie: specie, Portinnesto: R.RispostaStringa });
                                        resolve(this.PortinnestoxSpecie.find((el) => {
                                            if (el.Specie.codice == specie.codice) {
                                                return el;
                                            }
                                        }).Portinnesto);
                    })).subscribe();

            } else {

                resolve(this.PortinnestoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).Portinnesto);

            }
        });

    }

}
