import { Injectable } from '@angular/core';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { map } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';

export class CaricaComboDestinazioniUso {
    objP_server: string;
}

export class LeggiDestinazioniUso {
}

@Injectable({
    providedIn: 'root'
})
export class DestinazioneUsoService {
    private DestinazioniUso: DestinazioneUso[];

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService) { }

    /*leggi_Old(){
        return new Promise<DestinazioneUso[]>(async (resolve, reject) => {
            if (this.DestinazioniUso == undefined || this.DestinazioniUso.length == 0) {
                const parametri: CoreWS_Generic<LeggiDestinazioniUso> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    new LeggiDestinazioniUso);

                const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<DestinazioneUso[], LeggiDestinazioniUso>(
                    this.masterService.link_CoreWS + '/Metaschema/SpecieVegetali.asmx/LeggiDestinazioniUso',
                    parametri,
                    false);

                this.DestinazioniUso = R.RispostaStringa;
                resolve(this.DestinazioniUso);
            } else {
                resolve(this.DestinazioniUso);
            }
        });
    }*/

    leggi(){
        return new Promise<DestinazioneUso[]>(async (resolve, reject) => {
            if (this.DestinazioniUso == undefined || this.DestinazioniUso.length == 0) {
                this.ajaxAgronicaAPIService.ajaxAPIGet<LeggiDestinazioniUso, DestinazioneUso[]>(
                    'Modello/DestinazioniUso',
                    new LeggiDestinazioniUso,
                    false).pipe(map(R => {
                        this.DestinazioniUso = R.RispostaStringa;
                        resolve(this.DestinazioniUso);
                    })).subscribe();
            } else {
                resolve(this.DestinazioniUso);
            }
        });
    }

}
