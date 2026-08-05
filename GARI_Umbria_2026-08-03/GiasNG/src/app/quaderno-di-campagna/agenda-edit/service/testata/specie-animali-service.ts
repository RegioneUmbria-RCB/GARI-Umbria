import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { RisorsaZootecnica } from 'app/Model/attivita/risorse/RisorsaZootecnica';
import { Genere } from 'app/Model/metaschema/utilizzi/Genere';
import { IndirizzoProduttivo } from 'app/Model/metaschema/utilizzi/IndirizzoProduttivo';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { MultiColumnComboboxGiasTemplate } from 'gias-ui-kit';
import { map } from 'rxjs';

export interface DropdownListSpecieAnimali extends MultiColumnComboboxGiasTemplate, RisorsaZootecnica { }

@Injectable({ providedIn: 'root' })
export class SpecieAnimaliService {

    constructor(
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private translocoService: TranslocoService
    ) { }

    leggiListaSpecieAnimali() {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, string>('Visite/LeggiRisorseZootecniche', '').pipe(map(r => {
            return r.RispostaStringa;
        }));
    }

    leggiListaSpecieAnimaliDDL() {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, string>('Visite/LeggiRisorseZootecniche', '').pipe(map(r => {
            let arrSpecieAnimali = JSON.parse(JSON.stringify(r.RispostaStringa));
            arrSpecieAnimali.map((c) => this.setCodDescrDDLSpecieAnimale(c));
            return arrSpecieAnimali;
        }));
    }


    setCodDescrDDLSpecieAnimale(ris: DropdownListSpecieAnimali) {
        ris.Codice_Concatenato = '';
        ris.Codice_Concatenato = ris.genere.codice + '-' + ris.specie.codice + '-' + ris.indirizzoProd.codice;
        ris.Descrizione_Concatenata = ris.descrizione;

        return ris;
    }

    setDefaultDDLSpecieAnimale(ris: DropdownListSpecieAnimali, defaultDesc: string) {

        ris.genere = new Genere(-1);
        ris.specie = new Specie(-1);
        ris.indirizzoProd = new IndirizzoProduttivo(-1);
        ris.descrizione = this.translocoService.translate(defaultDesc);

        ris.Codice_Concatenato = '';
        ris.Codice_Concatenato = ris.genere.codice + '-' + ris.specie.codice + '-' + ris.indirizzoProd.codice;
        ris.Descrizione_Concatenata = ris.descrizione;

        return ris;

    }
}