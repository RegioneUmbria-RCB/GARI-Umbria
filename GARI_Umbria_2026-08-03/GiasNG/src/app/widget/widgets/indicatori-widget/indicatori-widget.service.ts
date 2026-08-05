import { Injectable } from "@angular/core";
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from "app/Model/siti.enum";
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { cloneDeep } from "lodash";
import { Indicatore } from "./indicatori-widget.models";
import {CookieOptions, CookieService} from 'ngx-cookie-service';

@Injectable()
export class IndicatoriWidgetService {

  private functionMessageEventListener: any;

  paramIndicatori: Indicatore[] = [];
  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private cookieService: CookieService
  ) {}


  ApriPlugInIndicatoriDSS() {
    let objParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

    let title = "";

    let stazioniFormatoCorretto = [];

    //objParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
    objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

    //Lav_Cod lo passo a 0 perchè viene in realtà considerato solo per le operazioni contabili non di campagna
    let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

    this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_PlugIn_IndicatoriDSS,
      ParametriAggiuntivi,
      objParametriAgenda,
      true,
      -1,
      title,
      false).then(GiasIFrameWindowService => {
      GiasIFrameWindowService.window.content.instance.postMessageToParent({
        elemID: 'Gauges-Window',
        indicatori: this.paramIndicatori,
        view_grid: true,
        height: 0,
        rag_soc: objParametriAgenda.RagSoc
      });

      this.functionMessageEventListener = (event => {
        if (event != undefined && event.data != undefined && event.data.sourceId != undefined) {
          if (event.data.sourceId == 'Gauges-Window') {
            if (event.data.callbackParams != undefined) {
              const cookieUseDateRangeName = `DSS.Indicatori.Periodo.${objParametriAgenda.Piva}.UseDateRange`;
              const options: CookieOptions = {path: '/', secure: false};
              this.cookieService.set(cookieUseDateRangeName, JSON.stringify(true), options);

              let params: Array<ParametriAggiuntivi_QueryString> = [{key: 'params', value: event.data.callbackParams, codifica: false}];
              this.gestioneRichiesteService.gestionePassaggioAltroSito(
                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                enum_PagineAgenda_2010.Pagina_DSS_Difesa,
                params,
                objParametriAgenda
              ).then((resp) => {window.location.href = resp;});
            }
          }
        }
      }).bind(this);

      window.addEventListener('message', this.functionMessageEventListener);
    });
  }

}
