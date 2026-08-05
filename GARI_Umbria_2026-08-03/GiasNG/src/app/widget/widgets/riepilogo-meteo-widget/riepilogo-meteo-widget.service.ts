import { Injectable } from "@angular/core";
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from "app/Model/siti.enum";
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { cloneDeep } from "lodash";

@Injectable()
export class RiepilogoMeteoWidgetService {

    constructor(
                private objParametriAgendaService: ObjParametriAgendaService,
                private gestioneRichiesteService: GestioneRichiesteService) {}

    
    ApriPlugInMeteo(stazioniDaPassare: Array<any>, stazione: any) {
        let objParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

        let title = "";

        let stazioniFormatoCorretto = [];

        //objParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
        objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        //Lav_Cod lo passo a 0 perchè viene in realtà considerato solo per le operazioni contabili non di campagna
        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

        let idx = 0;

        stazioniDaPassare.forEach((elem, index) => {
            stazioniFormatoCorretto.push({
                Descrizione: elem.Descrizione,
                UltimoAggiornamento: elem.UltimoAggiornamento,
                Meteo: {
                    Table: JSON.parse(elem.Meteo.Meteo_Table),
                    Charts: JSON.parse(elem.Meteo.Meteo_Charts),
                    Riepilogo: JSON.parse(elem.Meteo.Meteo_RiepilogoSensori)
                }
            });

            if (elem.Descrizione == stazione.Descrizione)
                idx = index;
        });

        this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_PlugIn_Riepilogo_Meteo,
            ParametriAggiuntivi,
            objParametriAgenda,
            true,
            -1,
            title,
            false).then(GiasIFrameWindowService => {
                GiasIFrameWindowService.window.content.instance.postMessageToParent({
                    stazioni: stazioniFormatoCorretto,
                    selected: idx,
                    height: 0
                });
            });
    }

}