import { Component, Inject, OnInit } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { AlertDocumentiWidgetGridConfig, DEFAULT_TIMESTAMP_DOCUMENTI } from "./alert-documenti-widget-grid-config.service";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'app-alert-documenti-widget',
    templateUrl: './alert-documenti-widget.component.html',
    styleUrls: ['./alert-documenti-widget.component.scss'],
    providers: [
      ...generateGridProviders(AlertDocumentiWidgetGridConfig, AlertDocumentiWidgetComponent)
    ]
  })
export class AlertDocumentiWidgetComponent implements OnInit {

    public valueDateTimePicker: Date;
    public format = "MM/dd/yyyy HH:mm";

    constructor(
                @Inject(GRID_HTTP_TOKEN) private gridAlertService: AlertDocumentiWidgetGridConfig,
                private gridpublicService: GridPublicService
    ) {}


    ngOnInit(): void {

      let cookie = this.gridAlertService.getAlertDocumentiCookie();

      let value: Date;

      if (cookie)
        value = new Date(cookie);
      else
        value = DEFAULT_TIMESTAMP_DOCUMENTI;

      this.valueDateTimePicker = value;
    }

    reimpostaTimeStamp() {
      this.valueDateTimePicker = new Date();
      this.gridAlertService.setAlertDocumentiCookie(this.valueDateTimePicker);
      this.gridpublicService.refresh(true);
    }

    onChange(param: Date): void {
      this.gridAlertService.setAlertDocumentiCookie(param);
      this.gridpublicService.refresh(true);
    }

    redirectToScadLista() {
      this.gridAlertService.redirectToRicercaDocumenti();
    }

}
