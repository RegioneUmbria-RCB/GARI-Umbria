import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { Portinnesto } from 'app/Model/metaschema/DensitaImpianto/Portinnesto';
import { SeminaTrapianto } from 'app/Model/metaschema/SeminaTrapianto';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

@Injectable({
    providedIn: 'root'
})
export class SeminaTrapiantoService {

  public SeminaTrapiantoValues:SeminaTrapianto[] = [
    { codice: '-1', descrizione: '' },
    { codice: 'Trapiantato', descrizione: 'Trapiantato' },
    { codice: 'Seminato', descrizione: 'Seminato' }
  ]

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private masterService: MasterService) { }

    leggi(specie: Specie): Promise<SeminaTrapianto[]> {
        return new Promise<SeminaTrapianto[]>(async (resolve, reject) => {
            if (specie.codice != 0) {
                resolve(this.SeminaTrapiantoValues);
            } else {
                resolve([]);
            }
        });

    }

}
