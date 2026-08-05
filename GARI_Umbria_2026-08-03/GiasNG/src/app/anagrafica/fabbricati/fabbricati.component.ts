import { Component, OnInit } from '@angular/core';
import { FabbricatiGridService } from "./fabbricati-grid.service";
import { ObjParametriAgendaService } from "../../Service/obj-parametri-agenda.service";
import { enum_PagineGiasNG, enum_Security_Attivita } from "../../Model/TipiEnumerativi";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from "../../Model/siti.enum";
import { PermessiUtenteService } from "../../Service/permessi-utente.service";
import { GestioneRichiesteService } from "../../Service/gestione-richieste.service";
import { DialogRef, DialogService } from "@progress/kendo-angular-dialog";
import { SelezionaCentroComponent } from "./seleziona-centro/seleziona-centro.component";
import { take } from "rxjs/operators";
import { SelezionaCentroService } from "./seleziona-centro/seleziona-centro.service";
import { TranslocoService } from '@jsverse/transloco';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-fabbricati',
  templateUrl: './fabbricati.component.html',
  styleUrls: ['./fabbricati.component.css'],
  providers: [
    ...generateGridProvidersAnagrafica(FabbricatiGridService, FabbricatiComponent)
  ]
})
export class FabbricatiComponent implements OnInit {

  objParametriAgenda: ObjParametriAgenda;
  permessoEdit: boolean;

  constructor(
    private selezionaCentroService: SelezionaCentroService,
    private objParametriService: ObjParametriAgendaService,
    private dialogService: DialogService,
    private permessiUtenteService: PermessiUtenteService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private translocoService: TranslocoService) { }

  ngOnInit(): void {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Fabbricato, 2)
  }

  public impresaSelezionata() {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }


  public onNuovo() {

    const dialog: DialogRef = this.dialogService.open({
      title: this.translocoService.translate('SelezionaCentro'),
      content: SelezionaCentroComponent,
      width: 400,
      actions: [
        { text: this.translocoService.translate('CreaFabbricato'), primary: true },
        { text: this.translocoService.translate('Annulla') }
      ]
    });

    dialog.result.pipe(take(1)).subscribe((result) => {
      if ((<any>result).primary == true) {
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        this.objParametriAgenda.Fabbricato = 0;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        this.objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Anagrafica_Fabbricati;
        const centroSelezionato = this.selezionaCentroService.centroSelezionatoSubject.getValue()
        if (centroSelezionato != null && centroSelezionato != undefined && centroSelezionato.codice != 0) {
          this.objParametriAgenda.Sa_Cod = centroSelezionato.codice;
          this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
          this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
          this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Anagrafica_Fabbricato).then((val) => {
            window.location.href = val;
          });
        }
      }
    })

  }

}
