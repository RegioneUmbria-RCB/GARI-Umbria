import { Injectable } from "@angular/core";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { BehaviorSubject, map, Observable } from "rxjs";

export class MenuContestualeSettings {
    show: boolean;
    background_color?: string;
    color?: string;
    title: string;
    search: boolean;
    bookmarks: boolean;
    contextualMenu: boolean;
    IDTipoSezione: number;
    IDSezionePadre: number;
}

export class LeggiSezioni {
    IDTipoSezione: number;
    IDSezionePadre: number;
    Preferiti: boolean;
}

export class LinkMenu {
    colore: string;
    testo: string;
    idHtml: string;
    idSezione: number;
    sitoRichiesto: number;
    paginaRichiesta: number;
    richiedeAziendaSelezionata: number;
    redirectUrl: string;
    preferito: number;
    enum_TipoAperturaPagina: number;
}

@Injectable({ providedIn: 'root' })
export class MenuContestualeService {
    private _pendingWork: boolean;

    private menuContestualeSettings: MenuContestualeSettings = null;

    private menuContestualeSettingsSource = new BehaviorSubject(this.menuContestualeSettings);
    currentMenuContestualeSettings: Observable<MenuContestualeSettings> = this.menuContestualeSettingsSource.asObservable();

    constructor(private masterService: MasterService,
                private ajaxAgronicaService: AjaxAgronicaService) {

    }

    /** May be used to refresh grid's data once a bussiness (azienda) 
     *  has been changed.
     **/
    public markPendingWorkAsDone() {
        this._pendingWork = false;
    }

    get pendingWorkDone() {
        return this._pendingWork;
    }

    changeMenuContestualeSettings(upd_menuContestualeSettings: MenuContestualeSettings) {
        this._pendingWork = true;
        this.menuContestualeSettingsSource.next(upd_menuContestualeSettings);
    }

    getMenuContestualeSettings(): MenuContestualeSettings {
        return this.menuContestualeSettingsSource.getValue();
    }



    LeggiPreferiti(): Observable<LinkMenu[]> {
        const menuContestualeValue = this.getMenuContestualeSettings();
        const inData: LeggiSezioni = {
            IDSezionePadre: 0,
            IDTipoSezione: menuContestualeValue.IDTipoSezione,
            Preferiti: true
        }

        const parametri: CoreWS_Generic<LeggiSezioni> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: inData
        }


        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<LinkMenu[], LeggiSezioni>(
            this.masterService.link_CoreWS + '/Menu/Menu.asmx/LeggiSezioni', //IMPORTANTE
            parametri).pipe(
                map((resp) => {
                    return resp.RispostaStringa;
                })
            );
    }



    LeggiMenu(): Observable<LinkMenu[]> {
        const menuContestualeValue = this.getMenuContestualeSettings();
        const inData: LeggiSezioni = {
            IDSezionePadre: menuContestualeValue.IDSezionePadre,
            IDTipoSezione: menuContestualeValue.IDTipoSezione,
            Preferiti: false
        }

        const parametri: CoreWS_Generic<LeggiSezioni> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: inData
        }


        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<LinkMenu[], LeggiSezioni>(
            this.masterService.link_CoreWS + '/Menu/Menu.asmx/LeggiSezioni', //IMPORTANTE
            parametri).pipe(
                map((resp) => {
                    return resp.RispostaStringa;
                })
            );

    }

}
