import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Widget_Configuration, WidgetsClient } from 'app/Service/api.service';
import { skip, combineLatest } from 'rxjs';
import { WidgetConfigService } from './widget-config.service';
import { GestioneMultiAziendaService } from './gestione-multiazienda.service';
import { IUtenteDTO } from "../profilazione/models/utente-dto.model";
import { WidgetConfigListComponent } from "./section/widget-config-list/widget-config-list.component";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  standalone: false,
  selector: 'app-widget-config',
  templateUrl: './widget-config.component.html',
  styleUrls: ['./widget-config.component.css']
})
export class WidgetConfigComponent implements OnInit, OnDestroy {
  @Input() user: IUtenteDTO | null = null;
  @ViewChild('widgetList') widgetList: WidgetConfigListComponent;

  isConfirmationOpen = false;
  customSelected = true;
  useMultiCompany = false;

  widgets: Widget_Configuration[] = [];
  widgetsBlocked: boolean;

  constructor(
    private widgetsClient: WidgetsClient,
    private widgetConfigService: WidgetConfigService,
    private gestioneMultiAziendaService: GestioneMultiAziendaService
  ) {
    //nel caso cambi la nazione o l'anno nei widget MultiAzienda, vanno ricaricati
    combineLatest({
      nazione: this.gestioneMultiAziendaService.nazioneMultiAzienda$,
      anno: this.gestioneMultiAziendaService.annoMultiAzienda$
    })
      .pipe(skip(1), takeUntilDestroyed())
      .subscribe(() => this.widgetConfigService.changeConfiguration(this.widgets));

    this.useMultiCompany = this.gestioneMultiAziendaService.widgetsMultiAzienda;
  }

  ngOnDestroy(): void {
  }

  ngOnInit(): void {
    if (this.user == null) {
      throw new Error("Error in WidgetConfigComponent. Parameter user cannot be null");
    }
  }

  changeCustomSelected(event: Event): void {
    // Premo il preset iniziale e aspetto l'OK dell'utente
    if ((event.currentTarget as HTMLInputElement).checked) {
      this.customSelected = true;
      return;
    }

    if (this.user?.UserName != null) {
      event.preventDefault();
      this.isConfirmationOpen = true;
    }
  }

  changeMultiAzienda(event: Event) {
    this.useMultiCompany = (event.currentTarget as HTMLInputElement).checked;
  }

  onWidgetsBlocked(event: Event): void {
    const checked = (event.target as any).checked;
    this.widgetConfigService.$widgetsBlocked.next(checked);
    this.widgetsBlocked = checked;
  }

  close(): void {
    this.customSelected = true;
  }

  resetConfiguration(): void {
    if (this.widgetList) {
      this.widgetList.resetConfiguration().subscribe(() => {
        this.customSelected = false;
        this.isConfirmationOpen = false;
      })
    }
  }

  hideSwitchMultiAzienda() {
    return this.gestioneMultiAziendaService.getPermessoWidgetMultiAzienda();
  }

}
