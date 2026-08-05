import {Injectable} from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import {PermessiUtenteService} from "app/Service/permessi-utente.service";
import {map, Observable} from "rxjs";
import {enum_PagineGiasNG, enum_Security_Attivita} from "../Model/TipiEnumerativi";
import {ObjParametriAgendaService} from "../Service/obj-parametri-agenda.service";
import {Tipo_Ricetta} from "../Model/attivita/Attivita";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import { Tipo_Attivita, Stati } from 'gias-ui-kit';

@Injectable({ providedIn: 'root' })
export class PermessiUtenteGuard  {

    constructor(private permessiUtenteService: PermessiUtenteService,
                private router: Router,
                private objParametriAgendaService: ObjParametriAgendaService) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        return this.permessiUtenteService.currentUtente_Permessi.pipe(map((val) => {
            if (val) {

                const page = route.data.page;

                const readPermissions = route.data.readPermissions as Array<number>;

                if (readPermissions != null && readPermissions != undefined) {
                    for (let i = 0; i < readPermissions.length; i++){
                        if (!this.permessiUtenteService.getPermesso(readPermissions[i], 0)) {
                            this.router.navigate(['/AccessoNegato']);
                            return false;
                        }
                    }
                }

                const writePermissions = route.data.writePermissions as Array<number>;

                if (writePermissions != null && writePermissions != undefined) {

                    if(page){

                        switch(page){
                            case enum_PagineGiasNG.Pagina_Edit_Attivita:
                            case enum_PagineGiasNG.Pagina_Edit_Visite:

                                let objAgenda = this.objParametriAgendaService.getObjParamValue();

                                if(objAgenda){

                                    if(objAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Read){

                                        let attivita: number = 0;

                                        switch(objAgenda.TipoOperazioneAgenda){

                                            case Tipo_Attivita.QuadernoDiCampagna:

                                                let creazione_modifica_operazione = writePermissions.filter(w=>w === enum_Security_Attivita.Agenda_AccessoMenu_NG);

                                                if(creazione_modifica_operazione.length === 1)
                                                    attivita = enum_Security_Attivita.Agenda_AccessoMenu_NG;

                                                break;

                                            case Tipo_Attivita.Ricetta:

                                                if(objAgenda.TipoRicetta === Tipo_Ricetta.Standard_Destinazioni){
                                                    if(objAgenda.Stato === Stati.Da_Eseguire){
                                                        let creazione_modifica_ricetta = writePermissions.filter(w=>w === enum_Security_Attivita.Gest_Ricette);

                                                        if(creazione_modifica_ricetta.length === 1)
                                                            attivita = enum_Security_Attivita.Gest_Ricette;

                                                    }else if(objAgenda.Stato === Stati.Eseguita){
                                                        let creazione_modifica_brogliaccio = writePermissions.filter(w=>w === enum_Security_Attivita.Brogliaccio);

                                                        if(creazione_modifica_brogliaccio.length === 1)
                                                            attivita = enum_Security_Attivita.Brogliaccio;
                                                    }
                                                }
                                                break;

                                        }

                                        if(attivita > 0){
                                            if (!this.permessiUtenteService.getPermesso(attivita, 2)) {
                                                this.router.navigate(['/AccessoNegato']);
                                                return false;
                                            }
                                        }
                                    }

                                }else{
                                    this.router.navigate(['/AccessoNegato']);
                                    return false;
                                }

                                break;
                            default:
                                for (let i = 0; i < writePermissions.length; i++){
                                    if (!this.permessiUtenteService.getPermesso(writePermissions[i], 2)) {
                                        this.router.navigate(['/AccessoNegato']);
                                        return false;
                                    }
                                }
                                break;
                        }
                    }


                }

                return true;
            }
        }))
    }
}
