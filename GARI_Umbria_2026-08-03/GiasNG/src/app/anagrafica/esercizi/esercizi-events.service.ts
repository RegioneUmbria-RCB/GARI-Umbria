import {Inject, Injectable} from '@angular/core';
import { GridInfoCommandEvent } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { GiasWindowsService } from 'gias-ui-kit';
import {LeggiInvestimentoCatastale} from 'app/Service/ServiceFactory/investimento-catastale.factory.service';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { ConfigurazioneSitiService } from 'app/Service/configurazione-siti.service';
import {take} from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import {Gias2010Redirector} from "../../menu-agenda/components/grid-qdc/Gias2010Redirector.service";
import {FiltersService} from "../../menu-agenda/components/filters/filters.service";
import {
  InvestimentoCatastaleAppezzamentoComponent
} from './investimento-catastale-appezzamento/investimento-catastale-appezzamento.component';
import {
  InvestimentoCatastaleAppezzamentoDataService
} from './investimento-catastale-appezzamento/investimento-catastale-appezzamento-data.service';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../Service/ServiceFactory/impianti.factory.service';
import {GiasMessageService} from '../../Service/gias-message.service';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class EserciziEventsService {

  objParametriAgenda: ObjParametriAgenda;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private route: ActivatedRoute,
    private gestioneRichiesteService: GestioneRichiesteService,
    private windowService: GiasWindowsService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private translocoService: TranslocoService,
    private gias2010redirect: Gias2010Redirector,
    private filterService: FiltersService,
    private investimentoCatastaleAppezzamentodataService: InvestimentoCatastaleAppezzamentoDataService,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private giasMessageService: GiasMessageService
  ) {  }

  public infoAppezzamento(event: GridInfoCommandEvent) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.Piva = event.dataItem.PIVA;
    this.objParametriAgenda.Sa_Cod = parseInt(event.dataItem.SA_COD);
    this.objParametriAgenda.Appezza = parseInt(event.dataItem.APPEZZA);
    this.objParametriAgenda.Id_Reg = parseInt(event.dataItem.id_Reg);
    this.objParametriAgenda.Progetto_Cod = parseInt(event.dataItem.Progetto_Cod);
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['../Appezzamenti/Appezzamento-Edit'], { relativeTo: this.route });
  }

  public onTemplateBtnClick(dataItem): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.Piva = dataItem.PIVA;
    this.objParametriAgenda.Sa_Cod = parseInt(dataItem.SA_COD);
    this.objParametriAgenda.Appezza = parseInt(dataItem.APPEZZA);
    this.objParametriAgenda.Id_Reg = parseInt(dataItem.id_Reg);
    this.objParametriAgenda.Progetto_Cod = parseInt(dataItem.Progetto_Cod);
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['../Appezzamenti/Appezzamento-Edit'], { relativeTo: this.route });
  }

  public onNuovo(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.Sa_Cod = 0;
    this.objParametriAgenda.Campo_Cod = 0;
    this.objParametriAgenda.Appezza = 0;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['../Appezzamenti/Appezzamento-Edit'], {relativeTo: this.route});
  }

  public onNuovaAttivita(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.configurazioneSitiService.leggi().pipe(take(1)).subscribe(data => {
      let flagNuovo = data.find((val) => val.Chiave == 'OperazioniAgendaNG');
      if (flagNuovo !== undefined && flagNuovo.Valore == 'true') {

        //TODO ci saranno da impostare i dati di default (specie,centro,impianti selezionati) per le pagine
        // che non vengono gestite dal QdC NG (esempio Irrigazione,Rilievo,Fase fenologica,ecc..)
        this.gias2010redirect.gestisciRedirectToQdC(this.objParametriAgenda).then();
      } else {
        this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Trattamenti).then(resp => window.location.href = resp);
      }
    })
  };

  public onListaOperazioni(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const parametri: ParametriAggiuntivi_QueryString[] = [
      {key: 'OperazioniImpianto', value: '1', codifica: false}
    ];
    let parametriNG: Params = {};
    parametri.forEach(ciclo => {
      parametriNG[ciclo.key] = ciclo.value;
    });

    this.configurazioneSitiService.leggi().pipe(take(1)).subscribe(data => {
      let flag = true;
      let flagNuovo = data.find((val) => val.Chiave == 'MenuAgendaNG');
      if (flagNuovo !== undefined && flagNuovo.Valore == 'true') {
        this.gestioneRichiesteService.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Menu_Agenda).then(resp => {
          this.filterService.init();
          this.router.navigate([resp], {queryParams: parametriNG});
        });
      } else {
        this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Menu_BS, parametri).then(resp => window.location.href = resp);
      }
    });
  }

  public onCatasto(impianto: Impianto, item: any): void {
    this.setLeggiInvestimentoCatastale(impianto);

    let descrizioneImpianto: string = "";
    descrizioneImpianto += item.app_nome ?? '';

    if (item.veg_des != '') {
      descrizioneImpianto += " (" + item.veg_des + " - " + item.cul_des + ")";
    } else if (item.Destinazione_Uso_Des != '') {
      descrizioneImpianto += " (" + item.Destinazione_Uso_Des + ")";
    } else {
      descrizioneImpianto += "Terreno Nudo";
    }

    this.windowService.open({
      title: this.translocoService.translate('InvestimentoCatastale') + ' ' + descrizioneImpianto,
      content: InvestimentoCatastaleAppezzamentoComponent,
      left: 10,
      width: window.innerWidth - 30,
      autoFocusedElement: "#investimentoCatastaleAppezzamentoComponent"
    });
  }

  private setLeggiInvestimentoCatastale(impianto: Impianto): void {
    let leggiInvestimentoCatastale: LeggiInvestimentoCatastale = new LeggiInvestimentoCatastale();
    let impresa: Impresa = new Impresa();
    impresa.partitaIva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva;

    let centro: CentroAziendale = new CentroAziendale({
      partitaIva: impresa.partitaIva,
      codice: impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
    });

    let particella: ParticelleCatastali = new ParticelleCatastali();
    particella.primaryKey = {
      Prov: '',
      Com: '',
      Sezione: '',
      Foglio: 0,
      Numero: 0,
      Subalterno: '',
    };

    leggiInvestimentoCatastale.impresa = impresa;
    leggiInvestimentoCatastale.centro = centro;
    leggiInvestimentoCatastale.particella = particella;
    leggiInvestimentoCatastale.impianto = impianto;
    leggiInvestimentoCatastale.data = AGRODATAINIZIO;

    this.investimentoCatastaleAppezzamentodataService.leggiInvestimentoCatastale = leggiInvestimentoCatastale;
  }
}
