import { Inject, Injectable, OnDestroy } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { TranslocoService, TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { Tipo_Attivita} from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Subscription } from 'rxjs';
import {enum_PagineGiasNG} from "../../Model/TipiEnumerativi";
import {enum_PagineAgenda_2010} from "../../Model/siti.enum";

@Injectable()
export class QdCLoadedGuard  implements OnDestroy {

    Subs = new Subscription();

    constructor(private translocoService: TranslocoService,
                private objParametriAgendaService: ObjParametriAgendaService,
                @Inject(TRANSLOCO_SCOPE) private scope){}

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        return new Promise<boolean>(async (resolve, reject) => {
            this.Subs.add(this.translocoService.selectTranslate('Quaderno_di_Campagna', {}, this.scope).subscribe(e => {

                this.fixObjParametriAgenda();

                resolve(true);
            }));
        });
    }

    fixObjParametriAgenda(){

        //Se il Tipo Operazione è lettura o modifica ma non ho valorizzato ne l'id_agenda e neacnche il ricetta_operazione_cod forzo
        // l'operazione di scrittura (come per esempio se la pagina viene aperta direttamente dal Menu_bs)

        let objParametriAgenda=this.objParametriAgendaService.getObjParamValue();

        switch(objParametriAgenda.TipoOperazioneDB){
            case Enum_DBTypeOperation.Update:
            case Enum_DBTypeOperation.Read:

                if(objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna){

                    if(objParametriAgenda.Id_Agenda <= 0){
                        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                    }

                }else if(objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.Ricetta){

                    if(objParametriAgenda.Ricetta_Operazione_Cod <= 0){
                        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                    }

                }

                break;
            case Enum_DBTypeOperation.Write:
                break;
        }

        this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }
}
