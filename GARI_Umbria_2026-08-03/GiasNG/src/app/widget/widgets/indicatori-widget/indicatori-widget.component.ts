import {Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import { MenuClient, Utente, WidgetsClient, Widget_Modelli_Previsionali_Indicatori_IN } from 'app/Service/api.service';
import { Indicatore, IndicatoriData } from './indicatori-widget.models';
import {takeUntil} from "rxjs/operators";
import {Subject} from "rxjs";
import { IndicatoriWidgetService } from './indicatori-widget.service';
import {isSuperUser} from '../../../Service/utils';
import {FormControl, FormGroup} from '@angular/forms';
import {DialogRef} from '@progress/kendo-angular-dialog';
import {GiasDialogService} from 'gias-ui-kit';
import {TranslocoService} from '@jsverse/transloco';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../../Model/CostantiPersonalizzate';
import {ObjParametriAgendaService} from '../../../Service/obj-parametri-agenda.service';
import {NgxCookieService} from '../../../Service/ngx-cookie.service';
import {CookieOptions} from 'ngx-cookie-service';

const DSS_STORAGE_KEY = "DSS.Indicatori.Periodo";

const ProcessingStatus: Record<string, string> = {
  '-1': 'outofrange',
  '1': 'warning',
  '2': 'error',
  '3': 'progress',
  '0': 'valid'
};

const WeatherStatus: Record<string, string> = {
  '0': 'none',
  '1': 'info',
  '2': 'warning',
  '3': 'error'
};

function startDateDefault(): Date {
  return new Date(new Date().getFullYear(), 0, 1); // First day of the year
}

function endDateDefault(): Date {
  let endDate: Date = new Date();
  endDate.setHours(0, 0, 0, 0);
  return endDate;
}

class TimeIntervalFG {
  start: FormControl<Date>;
  end: FormControl<Date>;

  constructor(start: Date, end: Date) {
    this.start = new FormControl<Date>(start);
    this.end = new FormControl<Date>(end);
  }
}

@Component({
  standalone: false,
  selector: 'app-indicatori-widget',
  templateUrl: './indicatori-widget.component.html',
  styleUrls: ['./indicatori-widget.component.css']
})
export class IndicatoriWidgetComponent implements OnInit, OnDestroy {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  @ViewChild('timeIntervalTemplate') timeIntervalTemplate: TemplateRef<unknown> | undefined;

  data: Indicatore[] = [];
  loading: boolean = true;
  error: boolean = false;
  fullWidgetOpened: boolean = false;
  series: { value: string, color: string }[] = [];
  gaugesMessges: string[] = [];
  user: Utente | null = null;

  startDate: Date;
  endDate: Date;

  protected timeIntervalForm: FormGroup<TimeIntervalFG>;

  private signal$: Subject<void> = new Subject();
  private destroyed = false;

  constructor(
    private widgetsClient: WidgetsClient,
    private menuClient: MenuClient,
    private indicatoriWidgetService: IndicatoriWidgetService,
    private giasDialogService: GiasDialogService,
    private transloco: TranslocoService,
    private cookieService: NgxCookieService,
    private objParamsService: ObjParametriAgendaService
  ) {
    this.timeIntervalForm = new FormGroup<TimeIntervalFG>(new TimeIntervalFG(AGRODATAINIZIO, AGRODATAFINE));

    this.loadDates();
    this.initDates(false);

    this.menuClient
      .menuInformazioniUtente()
      .subscribe((data) => this.user = data.RispostaStringa);
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
    this.destroyed = true;
  }

  ngOnInit(): void {
    const startDateIsCustom: boolean = this.startDate.valueOf() !== startDateDefault().valueOf();
    const endDateIsCustom: boolean = this.endDate.valueOf() !== endDateDefault().valueOf();
    const addExtraParams: boolean = startDateIsCustom || endDateIsCustom;
    this.getData(addExtraParams);
  }

  resetDates(): void {
    this.initDates(true);
    this.getData(false);
  }

  confirmDialog(): void {
    this.startDate = this.timeIntervalForm.controls.start.value;
    this.endDate = this.timeIntervalForm.controls.end.value;

    this.saveDates();
    this.getData(true);
  }

  rejectDialog(): void {
    this.timeIntervalForm.controls.start.setValue(this.startDate);
    this.timeIntervalForm.controls.end.setValue(this.endDate);
  }

  areDateControlsVisible(): boolean {
    return this.user != null && isSuperUser(this.user);
  }

  onClickChart() {
    this.indicatoriWidgetService.ApriPlugInIndicatoriDSS();
  }

  onSetTimeInterval(): void {
    const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
      this.transloco.translate('Impostazione periodo calcolo indicatori emergenze DSS'),
      this.timeIntervalTemplate,
      undefined,
      undefined,
      undefined,
      (p) => p['returnObj'] && this.timeIntervalForm.controls.start.value >= this.timeIntervalForm.controls.end.value
    );

    dialog.result.pipe().subscribe(resp => {
      if (resp['returnObj']) {
        this.confirmDialog();
      } else {
        this.rejectDialog();
      }
    });
  }

  private initDates(reset = false): void {
    if (reset || this.startDate == null) {
      this.startDate = startDateDefault();
    }

    if (reset || this.endDate == null) {
      this.endDate = endDateDefault();
    }

    this.timeIntervalForm.controls.start.setValue(this.startDate);
    this.timeIntervalForm.controls.end.setValue(this.endDate);

    this.saveDates();
  }

  private saveDates(): void {
    const payload = JSON.stringify({ startDate: this.startDate, endDate: this.endDate });
    // localStorage.setItem(`${DSS_STORAGE_KEY}.${this.piva ?? this.objParamsService.getObjParamValue().Piva}`, payload);

    const options: CookieOptions = {path: '/', secure: false};
    this.cookieService.set(`${DSS_STORAGE_KEY}.${this.piva ?? this.objParamsService.getObjParamValue().Piva}`, payload, options);
  }

  private loadDates(): void {
    const entry = this.cookieService.get(`${DSS_STORAGE_KEY}.${this.piva ?? this.objParamsService.getObjParamValue().Piva}`);
    // const entry = localStorage.getItem(`${DSS_STORAGE_KEY}.${this.piva ?? this.objParamsService.getObjParamValue().Piva}`);
    if (entry == null || entry == '') {
      return;
    }

    const payload = JSON.parse(entry);
    if (payload.startDate != null) {
      this.startDate = new Date(payload.startDate);
    }

    if (payload.endDate != null) {
      this.endDate = new Date(payload.endDate);
    }
  }

  private getData(addExtraParams: boolean): void {
    this.loading = true;
    let payload: Widget_Modelli_Previsionali_Indicatori_IN = {
      Piva: this.piva
    } as Widget_Modelli_Previsionali_Indicatori_IN;

    if (addExtraParams) {
      payload['ParamExtra'] = {
        DataInizio: this.startDate,
        DataFine: this.endDate
      };
    }

    this.widgetsClient
      .widgetsModelliPrevisionaliElaboraIndicatori(payload)
      .pipe(takeUntil(this.signal$))
      .subscribe((data) => {
        if (!this.destroyed) {
          this.processData(data.RispostaStringa, addExtraParams);
        }
      });
  }

  private processData(data: IndicatoriData, addExtraParams: boolean): void {
    this.loading = false;
    this.error = false;
    this.series.splice(0);
    this.gaugesMessges.splice(0);

    if (data == null || data.Stato == 0) {
      setTimeout(() => this.getData(addExtraParams), 500);
      return;
    }

    if (data.Stato == -1) {
      this.error = true;
      return;
    }

    this.data = data.Indicatori;

    let colors = {};
    for (const indic of this.data) {
      let col = "";

      switch (indic.Risultato.Status) {
        case '2':
        case "-1":
        case "3":
          col = "#ccc";
          break;
        default:
          for (const band of indic.Risultato.Bands) {
            if (indic.Risultato.Value <= band.Value) {
              col = band.Color;
              break;
            }
          }
      }

      this.gaugesMessges.push(`${indic.Avversita} : ${indic.Risultato.StatusMsg}`);

      if (col !== "") {
        col = "C_" + col;
        if (!colors.hasOwnProperty(col)) {
          colors[col] = 0;
        }
        colors[col] += 1;
      }
    }

    for (const k of Object.keys(colors)) {
      this.series.push({
        value: colors[k],
        color: k.substring(2)
      });
    }

    this.indicatoriWidgetService.paramIndicatori = data.Indicatori;

    this.indicatoriWidgetService.paramIndicatori.sort(function (elem0, elem1) {
      let v0 = [elem0.Stazione.toUpperCase(), elem0.Specie.toUpperCase()];
      let v1 = [elem1.Stazione.toUpperCase(), elem1.Specie.toUpperCase()];
      let cmp = 0;
      for (let idx = 0; (cmp === 0 && idx < v0.length); idx++) {
        if (v0[idx] < v1[idx]) {
          cmp = -1;
        } else if (v0[idx] > v1[idx]) {
          cmp = 1;
        }
      }
      return cmp;
    });

    this.indicatoriWidgetService.paramIndicatori.forEach((indic) => {
      indic.Risultato.Status = ProcessingStatus[indic.Risultato.Status];

      if (indic.Risultato.MeteoStatus == undefined) {
        indic.Risultato.MeteoStatus = "0";
        indic.Risultato.MeteoMsg = "";
      }

      indic.Risultato.MeteoStatus = WeatherStatus[indic.Risultato.MeteoStatus];
    });
  }
}
