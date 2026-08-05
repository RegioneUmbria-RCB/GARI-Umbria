import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { MasterService } from "app/Service/master.service";
import { Observable } from "rxjs";
import {ObjParametriAgendaService} from "../Service/obj-parametri-agenda.service";
import {Enum_SiteRedirector} from "../Model/siti.enum";
import {getMasterWithEditPages} from "../Model/TipiEnumerativi";


@Injectable({ providedIn: 'root' })
export class CurrentPageGuard  {

    constructor(private router: Router,
                private masterService: MasterService,
                private objParametriAgendaService: ObjParametriAgendaService) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        const requestedPage = route.data.page;
        if (requestedPage > 0) {
            this.masterService.changeCurrentPage(requestedPage);

            //Reimposto l'idSezione corretto preso dal menu
            if(this.masterService.InfoAlberoMenu){


                let idSezione = this.getIdSezione(requestedPage);

                if(idSezione === 0){
                  let array = getMasterWithEditPages();

                  if(array && array.length > 0){
                    for(let a of array){
                      if(a.editPage.findIndex(i=>i === requestedPage) > -1){
                        idSezione = this.getIdSezione(a.masterPage);
                        break;
                      }
                    }
                  }
                }

                if(idSezione > 0){
                  let objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

                  if(idSezione !== objParametriAgenda.IdSezione){
                    objParametriAgenda.IdSezione = idSezione;

                    this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);
                  }
                }


            }
        }

        return true;
    }

    private getIdSezione(requestedPage: number): number{
      let idSezione = 0;

      let Voci_Menu = this.masterService.InfoAlberoMenu.Menus;

      if(Voci_Menu && Voci_Menu.length > 0){
        for(let v of Voci_Menu){

          if(idSezione > 0)
            break;

          if(v.paginaRichiesta === requestedPage){
            idSezione = v.idSezione;

            break;
          }else{

            let figli_NG = v.Figli.filter(f=>f.sitoRichiesto === Enum_SiteRedirector.GiasNG);


            if(figli_NG && figli_NG.length > 0){
              for(let figlio of figli_NG){
                if(figlio.paginaRichiesta === requestedPage){
                  idSezione = figlio.idSezione;

                  break;
                }
              }
            }

          }
        }
      }

      return idSezione;
    }
}
