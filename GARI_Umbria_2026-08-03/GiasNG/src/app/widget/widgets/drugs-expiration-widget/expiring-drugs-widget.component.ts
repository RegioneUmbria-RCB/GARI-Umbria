import { Component, Inject, Input, OnInit, OnDestroy  } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";

import { generateGridProviders } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService } from 'gias-ui-kit';

import { Widget_Zoo_IN } from "app/Service/net-core6-api.service";
import { ExpiringDrugsWidgetGridConfig } from './expiring-drugs-grid-widget-config.service';
import { OperazioniZooClient } from "app/Service/net-core6-api.service";

@Component({
  standalone: false,
  selector: 'app-expiring-drugs-widget',
  templateUrl: './expiring-drugs-widget.component.html',
  styleUrls: ['./expiring-drugs-widget.component.scss'],
  providers: [
    ...generateGridProviders(ExpiringDrugsWidgetGridConfig, ExpiringDrugsWidgetComponent),
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService
  ]
})
export class ExpiringDrugsWidgetComponent implements OnInit, OnDestroy {
  @Input() piva: string | null = null;
  format = "MM/dd/yyyy HH:mm";

  timeStart: Date;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridAlertService: ExpiringDrugsWidgetGridConfig,
    private gridpublicService: GridPublicService,
    public transloco: TranslocoService,
    private zooService: OperazioniZooClient,
  ) {
    this.timeStart = new Date();
  }

  ngOnInit(): void {
    const params: Widget_Zoo_IN = {
      Piva: this.piva,
      timeStart: this.timeStart
    };
    this.gridAlertService.param = params;
    this.gridpublicService.refresh(true);
  }

  ngOnDestroy(): void {
    const params: Widget_Zoo_IN = {
      Piva: this.piva,
      timeStart: this.timeStart
    };
    this.gridAlertService.setExpiringDrugsCookie(params);
  }

  reimpostaTimeStamp() {
    this.timeStart = new Date();
    this.gridAlertService.param.timeStart = this.timeStart;
    this.gridpublicService.refresh(true);
  }

  onChangeData(valueDateTimePicker: Date): void {
    this.timeStart = valueDateTimePicker;
    this.gridpublicService.refresh(true);
  }

  redirectToScadLista() {
    this.gridAlertService.redirectToRicercaDocumenti();
  }

}
