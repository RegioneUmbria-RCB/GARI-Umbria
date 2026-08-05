import { Component, OnInit } from '@angular/core';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { SvgSolver, SvgColor, hex2rgb } from 'gias-ui-kit';
import { LinkMenu, MenuClient, Preferiti_in } from 'app/Service/api.service';

@Component({
  standalone: false,
  selector: 'app-preferiti-config',
  templateUrl: './preferiti-config.component.html',
  styleUrls: ['./preferiti-config.component.css']
})
export class PreferitiConfigComponent implements OnInit {

  allServices: LinkMenu[] = [];
  filterText: string = "";
  isCustomChecked: boolean;
  isConfirmationOpen = false;

  constructor(
    private permessiUtente: PermessiUtenteService,
    protected ajaxApiService: AjaxAgronicaAPIService,
    private giasMessageService: GiasMessageService,
    private menuClient: MenuClient
  ) { }

  ngOnInit(): void {
    this.permessiUtente.getSidebarMenu();
    this.permessiUtente
      .sidemenuBehaviorSubject
      .subscribe(val => {
        this.allServices = val;
        this.computeCustomChecked();
      });
  }

  doServiceUndo(): void {
    this.permessiUtente.getSidebarMenu();
  }

  doServiceExecute(): void {
    const serviziFigliPreferiti = [];
    for (const parent of this.allServices) {
      for (const serviceFavorited of parent.Figli.filter(figlio => figlio.preferito)) {
        serviziFigliPreferiti.push(serviceFavorited.idSezione);
      }
    }

    const payload = { preferiti: serviziFigliPreferiti } as Preferiti_in;
    this.menuClient
      .menuAggiornaPreferiti(payload)
      .subscribe(() => {
        this.permessiUtente.sidemenuBehaviorSubject.next(this.allServices); // old logic
        this.giasMessageService.successMessage('ListaPreferitiAggiornata', false, true);
      });
  }

  changeCustomSelected($event: Event): void {
    if (($event.currentTarget as HTMLInputElement).checked) {
      this.isCustomChecked = true;
      return;
    }

    // ask user confirmation before resetting services
    $event.preventDefault();
    this.isConfirmationOpen = true;
  }

  resetConfiguration(): void {
    for (const service of this.allServices.flatMap(x => x.Figli)) {
      service.preferito = service.presetIniziale;
    }

    this.isCustomChecked = false;
    this.isConfirmationOpen = false;
  }

  changeFavorite(preferito: LinkMenu, favorite: boolean): void {
    if (!this.isCustomChecked) {
      this.giasMessageService.warningMessage('CambiareTipoVisualizzazione', false, true);
      return;
    }

    preferito.preferito = favorite;
    preferito.Figli.forEach(element => element.preferito = favorite);

    this.computeCustomChecked();
  }

  hasFavoriteChildren(service: LinkMenu): boolean {
    return service.Figli.filter(f => f.preferito).length > 0
  }

  private computeCustomChecked(): boolean {
    for (const service of this.allServices.flatMap(x => x.Figli)) {
      if (service.preferito != service.presetIniziale) {
        this.isCustomChecked = true;
        return;
      }
    }

    this.isCustomChecked = false;
  }
}
