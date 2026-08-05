import { Component, Inject, Input, OnInit, OnDestroy  } from "@angular/core";
import { FormGroup, FormControl } from '@angular/forms';
import { map, Observable, Subscription } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";

import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService } from 'gias-kendo-grid';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService } from 'gias-ui-kit';

import { OperazioniZooClient, Widget_Zoo_IN } from "app/Service/net-core6-api.service";
import { TreatmentsToDoWidgetGridConfig } from './treatments-todo-grid-widget-config.service';

export class Stalla {
  chiave: string;
  Piva: string;
  Sa_Cod: number;
  Sa_Des: string;
  Sta_Num: number;
  Sta_Des: string;
  BDN_Allev_IdFiscale: string;
  BDN_Codice_Azienda: string;

  constructor(piva, saCod, saDes, staNum, staDes, BDNproprietario, BDNcodAzienda) {
    this.chiave = piva + '_' + saCod + '_' + staNum;
    this.Piva = piva;
    this.Sa_Cod = saCod;
    this.Sa_Des = saDes;
    this.Sta_Num = staNum;
    this.Sta_Des = staDes;
    this.BDN_Allev_IdFiscale = BDNproprietario;
    this.BDN_Codice_Azienda = BDNcodAzienda;
  }
}

@Component({
  standalone: false,
  selector: 'app-treatments-todo-widget',
  templateUrl: './treatments-todo-widget.component.html',
  styleUrls: ['./treatments-todo-widget.component.css'],
  providers: [
    ...generateGridProviders(TreatmentsToDoWidgetGridConfig, TreatmentsToDoWidgetComponent),
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService
  ]
})
export class TreatmentsToDoWidgetComponent implements OnInit, OnDestroy {
  @Input() piva: string | null = null;
  format = "MM/dd/yyyy HH:mm";

  timeStart: Date;
  defaultStalla: Stalla;
  arrayStalle: Array<Stalla> = [];
  ddlStalle: HTMLElement;
  stallaForm: FormGroup = new FormGroup({
    stallaControl: new FormControl()
  });
  private stallaSubscription: Subscription;

  constructor(
    @Inject(GRID_HTTP_TOKEN) private gridAlertService: TreatmentsToDoWidgetGridConfig,
    private gridpublicService: GridPublicService,
    public transloco: TranslocoService,
    private zooService: OperazioniZooClient,
  ) {
    this.timeStart = new Date();
    this.defaultStalla = new Stalla(this.piva, 0, 0, 0, this.transloco.translate('TutteLeStalle'), "", "");
    this.ddlStalle = document.getElementById('ddlStalle');
  }

  private useDefaultStalla() {
    if (!this.arrayStalle.includes(this.defaultStalla)) this.arrayStalle.push(this.defaultStalla);
    this.stallaForm.get('stallaControl').setValue(this.defaultStalla);
    this.gridAlertService.param = {
      Piva: this.piva,
      Sa_Cod: 0,
      Sta_Num: 0,
      timeStart: this.timeStart
    };
  }

  ngOnInit(): void {
    this.gridAlertService.param = {
      Piva: this.piva,
      Sa_Cod: 0,
      Sta_Num: 0,
      timeStart: this.timeStart
    };

    this.loadStalleDDL(this.piva, 0)
      .subscribe((stalleResult) => {
        this.arrayStalle = [...stalleResult];

        let cookie = this.gridAlertService.getTreatmentsToDoCookie();
        if (cookie) {
          let stallaFromCookie = this.arrayStalle
            .find(s => s.Piva === cookie.Piva && s.Sa_Cod === cookie.Sa_Cod && s.Sta_Num === cookie.Sta_Num);
          if (stallaFromCookie) {
            this.stallaForm.get('stallaControl').setValue(stallaFromCookie);
            this.gridAlertService.param = cookie;
            this.timeStart = cookie.timeStart ? new Date(cookie.timeStart) : new Date();
          } else {
            this.useDefaultStalla();
          }
        } else {
          this.useDefaultStalla();
        }
    });

    this.stallaSubscription = this.stallaForm.get('stallaControl').valueChanges
      .subscribe((selectedValue: Stalla) => this.onChangeStalla(selectedValue));
  }

  ngOnDestroy(): void {
    const selectedStalla = this.ddlStalle ? (this.arrayStalle.find(s => String(s.Sta_Num) === (this.ddlStalle as any).value)) : null;
    const params: Widget_Zoo_IN = {
      Piva: selectedStalla ? selectedStalla.Piva : this.piva,
      Sa_Cod: selectedStalla ? selectedStalla.Sa_Cod : this.gridAlertService.param.Sa_Cod,
      Sta_Num: selectedStalla ? selectedStalla.Sta_Num : this.gridAlertService.param.Sta_Num,
      timeStart: this.timeStart
    };
    this.gridAlertService.setTreatmentsToDoCookie(params);
    if (this.stallaSubscription) this.stallaSubscription.unsubscribe();
  }

  reimpostaTimeStamp() {
    this.timeStart = new Date();
    this.gridAlertService.param.timeStart = this.timeStart;
    this.gridpublicService.refresh(true);
  }

  onChangeData(valueDateTimePicker: Date): void {
    this.timeStart = valueDateTimePicker;
    this.gridAlertService.param.timeStart = this.timeStart;
    this.gridpublicService.refresh(true);
  }

  onChangeStalla(value: any): void {
    if (value) {
      this.gridAlertService.param.Piva = value.Piva || this.piva;
      this.gridAlertService.param.Sa_Cod = value.Sa_Cod;
      this.gridAlertService.param.Sta_Num = value.Sta_Num;
      this.gridAlertService.stopLoading = false;
      this.gridpublicService.refresh(true);
    }
  }

  loadStalleDDL(piva: string, saCod: number) : Observable<Stalla[]> {
    return this.zooService.operazioniZooGetStalle(piva, saCod)
      .pipe(
        map(r => {
          let resp = JSON.parse(r.RispostaStringa);
          return resp.map((x: { [x: string]: any; }) => new Stalla(x['PIVA'], x['sa_cod'], x['sa_des'], x['STA_NUM'], x['STA_DES'], x['BDN_Allev_IdFiscale'], x['BDN_Codice_Azienda']));
        })
      );
  }

  redirectToAgenda(dataItem: any) {
    this.gridAlertService.toAgenda(dataItem);
  }
}
