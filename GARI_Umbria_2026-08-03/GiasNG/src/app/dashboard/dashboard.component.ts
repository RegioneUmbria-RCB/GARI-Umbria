import {Component, OnInit, ViewChild} from '@angular/core';
import {MenuClient, Utente} from 'app/Service/api.service';
import {MasterService} from 'app/Service/master.service';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {WidgetConfigComponent} from 'app/widget-config/widget-config.component';

@Component({
  standalone: false,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  @ViewChild(WidgetConfigComponent) widgetConfigComponent: WidgetConfigComponent;

  companySelected: boolean | null = null;
  user: Utente | null = null;

  protected showWidgetsConfigDialog = false;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private menuClient: MenuClient,
    private masterService: MasterService
  ) {
  }

  ngOnInit(): void {
    this.masterService.set_isLoading({isLoading: true});

    const company = this.objParametriAgendaService.getObjParamValue();
    this.companySelected = company?.Piva != null && company.Piva.trim() !== '';

    this.menuClient
      .menuInformazioniUtente()
      .subscribe((data) => this.user = data.RispostaStringa);
  }

  openWidgetConfig(): void {
    this.showWidgetsConfigDialog = true;
  }

  closeWidgetConfig() {
    this.widgetConfigComponent.close();
    this.showWidgetsConfigDialog = false;
  }

  widgetsLoaded(): void {
    this.masterService.set_isLoading({isLoading: false});
  }
}
