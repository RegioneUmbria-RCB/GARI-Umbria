import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { FormaAllevamento } from 'app/Model/metaschema/DensitaImpianto/FormaAllevamento';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class FormaAllevamentoxSpecie {
    Specie: Specie;
    FormaAllevamento: FormaAllevamento[];
}

export class LeggiFormaAllevamento{
    specie: Specie;
}

@Injectable({
    providedIn: 'root'
})
export class FormaAllevamentoService {
    private FormaAllevamentoxSpecie: FormaAllevamentoxSpecie[] = new Array();

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(specie: Specie): Promise<FormaAllevamento[]> {
        return new Promise<FormaAllevamento[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<FormaAllevamento>());
            }
            if (this.FormaAllevamentoxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                const parametri: CoreWS_Generic<LeggiFormaAllevamento> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { specie: specie }
                );

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<FormaAllevamento[], LeggiFormaAllevamento>(
                    this.masterService.link_CoreWS + '/Metaschema/FormeAllevamento.asmx/CaricaFormeAllevamento_Modello',
                    parametri,
                    false);

                this.FormaAllevamentoxSpecie.push({ Specie: specie, FormaAllevamento: R.RispostaStringa });
                resolve(this.FormaAllevamentoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).FormaAllevamento);

            } else {

                resolve(this.FormaAllevamentoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).FormaAllevamento);

            }
        });

    }*/

    leggi(specie: Specie): Promise<FormaAllevamento[]> {
        return new Promise<FormaAllevamento[]>(async (resolve, reject) => {
            if (specie.codice == 0) {
                resolve(new Array<FormaAllevamento>());
            }
            if (this.FormaAllevamentoxSpecie.find((el) => {
                if (el.Specie.codice == specie.codice) {
                    return el;
                }
            }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiFormaAllevamento, FormaAllevamento[]>(
                    'MetaschemaNG/CaricaFormeAllevamentoModello',
                    { specie: specie },
                    false).pipe(map(R => {
                        this.FormaAllevamentoxSpecie.push({ Specie: specie, FormaAllevamento: R.RispostaStringa });
                        resolve(this.FormaAllevamentoxSpecie.find((el) => {
                            if (el.Specie.codice == specie.codice) {
                                return el;
                            }
                        }).FormaAllevamento);
                    })).subscribe();

            } else {

                resolve(this.FormaAllevamentoxSpecie.find((el) => {
                    if (el.Specie.codice == specie.codice) {
                        return el;
                    }
                }).FormaAllevamento);

            }
        });

    }

}
