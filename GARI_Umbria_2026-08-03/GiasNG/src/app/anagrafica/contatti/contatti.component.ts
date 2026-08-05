import { Component, OnInit } from '@angular/core';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ContattiEventsService } from './contatti-events.service';
import { ContattiHttpService } from './contatti-grid.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-contatti',
  templateUrl: './contatti.component.html',
  styleUrls: ['./contatti.component.css'],
  providers: [
    ...generateGridProvidersAnagrafica(ContattiHttpService, ContattiComponent), ContattiEventsService
  ]
})
export class ContattiComponent implements OnInit {

  objParametriAgenda: ObjParametriAgenda;
  permessoEdit: boolean;

  constructor(
    private objParametriService: ObjParametriAgendaService,
    private permessiUtenteService: PermessiUtenteService,
    private gestioneRichiesteService: GestioneRichiesteService,
  ) { }

  ngOnInit(): void {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Contatto, 2)
  }

  public impresaSelezionata() {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }

  public onNuovo() {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    this.objParametriAgenda.Cod_Contatto = "";
    this.objParametriAgenda.Sa_Cod = 0;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New).then((val) => {
        window.location.href = val;
      });
  }

}
