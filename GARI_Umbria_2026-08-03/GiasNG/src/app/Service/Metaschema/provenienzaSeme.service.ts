import { Injectable } from '@angular/core';
import { ProvenienzaSeme } from 'app/Model/metaschema/ProvenienzaSeme';
import { SeminaTrapianto } from 'app/Model/metaschema/SeminaTrapianto';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

@Injectable({
    providedIn: 'root'
})
export class ProvenienzaSemeService {

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private masterService: MasterService) { }

    leggi(specie: Specie): Promise<ProvenienzaSeme[]> {
        return new Promise<ProvenienzaSeme[]>(async (resolve, reject) => {
            if (specie.codice != 0) {
                resolve([
                    { codice: 0, descrizione: '' },
                    { codice: 1, descrizione: 'Biologica' },
                    { codice: 2, descrizione: 'Integrato' },
                    { codice: 3, descrizione: 'Deroga' }
                ]);
            } else {
                resolve([]);
            }
        });

    }

}
