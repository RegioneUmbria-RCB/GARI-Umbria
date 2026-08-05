import { Injectable } from '@angular/core';
import { FormeGiuridiche } from 'app/Model/metaschema/FormeGiuridiche';
import { map, of } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

// REQUEST
export class Leggi_Forme_Giuridiche_Request {
    FG_cod: number;
    objP_server: string;
}

@Injectable({ providedIn: 'root' })
export class FormeGiurificheService {
    FormeGiuridiche: FormeGiuridiche[];

    constructor(private masterService: MasterService,
        private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
    }

    /*leggiFormeGiurifiche_Old(FG_cod: number) {
        return new Promise<FormeGiuridiche[]>(async (resolve, reject) => {
            if (this.FormeGiuridiche == undefined) {
                const parametri: Leggi_Forme_Giuridiche_Request = {
                    FG_cod: FG_cod,
                    objP_server: this.masterService.ObjParametri_Server
                };
                const R = await this.ajaxAgronicaService.ajaxAgronicaG<FormeGiuridiche[]>(this.masterService.link_CoreWS + '/Metaschema/FormeGiuridiche.asmx/Leggi_Modello', parametri);
                this.FormeGiuridiche = R.RispostaStringa;
                resolve(this.FormeGiuridiche);
            } else {
                resolve(this.FormeGiuridiche);
            }
        });
    }*/

    leggiFormeGiurifiche(FG_cod: number) {
        return new Promise<FormeGiuridiche[]>(async (resolve, reject) => {
            if (this.FormeGiuridiche == undefined) {
                this.ajaxAgronicaAPIService.ajaxAPIPost<any, FormeGiuridiche[]>('MetaschemaNG/LeggiModello', {FG_cod: FG_cod}).pipe(map(
                    (data) => {
                        this.FormeGiuridiche = data.RispostaStringa;
                        resolve(this.FormeGiuridiche);
                    })).subscribe();
            } else {
                resolve(this.FormeGiuridiche);
            }
        });
    }

}
