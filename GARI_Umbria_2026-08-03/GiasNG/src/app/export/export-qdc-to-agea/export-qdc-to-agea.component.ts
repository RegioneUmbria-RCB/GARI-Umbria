import { Component, OnDestroy, OnInit } from '@angular/core';
import { Impresa } from "../../Model/anagrafiche/Impresa";
import { UtilityFunctions } from "../../Utility/UtilityFunctions";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { BehaviorSubject, combineLatest, forkJoin, interval, Observable, of, Subscription } from "rxjs";
import { ImpreseService } from "../../Service/Anagrafica/imprese.service";
import { catchError, debounceTime, map, skip, switchMap, tap } from "rxjs/operators";
import { ObjParametriAgendaService } from "../../Service/obj-parametri-agenda.service";
import { FormBuilder, FormControl } from '@angular/forms';
import { AgeaService, ExportQdCtoAgeaPaginated, HubAgeaResult } from 'app/Service/Metaschema/agea.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { faFileCode, faFileText, faRotate } from '@fortawesome/free-solid-svg-icons';
import { PermessiUtenteService } from "../../Service/permessi-utente.service";
import { enum_Security_Attivita } from "../../Model/TipiEnumerativi";
import { ErroreGias, ErroreGias_Severity } from "../../Service/master.service";
import { GiasDialogAction, Dialog_Type, GiasDialogService } from "../../Service/gias-dialog.service";

const LOADING_STAGE_COMPLETED = 'COMPLETED';
const LOADING_STAGE_ERROR = 'ERROR';
const LOADING_STAGE_QUEUED = 'QUEUED';
const LOADING_STAGE_PROCESSING = 'PROCESSING';
const LOADING_STAGE_TRANSMISSION_ERROR = 'TRANSMISSION ERROR';
const SUBMISSION_QUEUED = 'QUEUED';
const SUBMISSION_ERROR = 'ERROR';
const SUBMISSION_TRANSMISSION_ERROR = 'TRANSMISSION ERROR';

@Component({
  standalone: false,
  selector: 'app-export-qdc-to-agea',
  templateUrl: './export-qdc-to-agea.component.html',
  styleUrls: ['./export-qdc-to-agea.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class ExportQdCToAgeaComponent implements OnInit, OnDestroy {

  subs: Subscription = new Subscription();
  listAnni: Array<number> = [];
  isLoading = false;
  jsonViewOpen: string | null = null;
  stateSubject = new BehaviorSubject<State>({ skip: 0, take: 5 });
  form = this.fb.group({
    anno: new FormControl<number | null>(null),
    impresa: new FormControl<Impresa | null>(
      {
        partitaIva: this.objParametriAgendaService.getObjParamValue().Piva,
        ragioneSociale: this.objParametriAgendaService.getObjParamValue().RagSoc
      } as Impresa),
  });

  faFileCode = faFileCode;
  faFileText = faFileText;
  faRotate = faRotate;

  data$ = combineLatest([this.form.controls.anno.valueChanges, this.form.controls.impresa.valueChanges, this.stateSubject.asObservable()])
    .pipe(
      debounceTime(100),
      switchMap(([year, impresa, state]) => this.readData(year, impresa, state)),
      catchError(() => of({ data: [], total: 0 })),
      tap(() => this.isLoading = false)
    );

  private oldCount = 0;
  private oldAnno: number | null = null;
  private oldImpresa: string | null = null;

  public AuthorizedUserForLogs: boolean = false;

  constructor(
    private fb: FormBuilder,
    private ddlService: GiasDropDownTemplateService,
    private impreseService: ImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private ageaService: AgeaService,
    private translocoService: TranslocoService,
    private messageService: GiasMessageService,
    private permessiUtenteservice: PermessiUtenteService,
    private giasdialogservice: GiasDialogService
  ) { }

  ngOnInit(): void {
    const year = new Date().getFullYear();

    //Lista di Anni deve essere compresa tra 2000 e 2100
    for (let i = 2000; i <= 2100; i++) {
      this.listAnni.push(i);
    }

    this.subs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(ddlElem => {
      switch (ddlElem.FormControlName) {
        case 'Impresa':
          break;
      }
    }));

    this.AuthorizedUserForLogs = this.permessiUtenteservice.getPermesso(enum_Security_Attivita.Visualizza_Comandi_Avanzati_Esportazione_Agea_NG, 0);

    this.subs.add(
      interval(30000).subscribe(() => {
        this.stateSubject.next({ ...this.stateSubject.value });
      })
    );

    setTimeout(() => {
      this.form.controls.anno.setValue(year);
    }, 100);
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  openDdlImpresaForm(ddlEl: GiasDropDownTemplateSComponent) {
    UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefinedObs(ddlEl, this.impreseService.LeggiImpreseConFiltroUtente());
  }

  openJson(bundleId: number): void {
    this.ageaService
      .readJson(bundleId)
      .pipe(map(x => x.RispostaStringa))
      .subscribe(data => this.jsonViewOpen = JSON.parse(data));
  }

  downloadLogs(item: ExportQdCToAgeaModel): void {
    const filename = (path: string) => path.replace(/^.*[\\/]/, '');
    const subLog = item.submissionInstanceId != null ? this.ageaService.readLog(filename(item.submissionInstanceId)) : of({ RispostaStringa: '' });
    const loadLog = item.loadingStageInstanceId != null ? this.ageaService.readLog(filename(item.loadingStageInstanceId)) : of({ RispostaStringa: '' });

    forkJoin([subLog, loadLog])
      .pipe(map(x => [x[0].RispostaStringa, x[1].RispostaStringa].join('\n\n').trim()))
      .subscribe(data => {
        const element = document.createElement('a');
        element.setAttribute('href', `data:text/plain;charset=utf-8,${encodeURIComponent(data)}`);
        element.setAttribute('download', `log_fornitura_${item.bundleId}.txt`);

        const event = new MouseEvent("click");
        element.dispatchEvent(event);
      });
  }

  export(erroriDaBypassare: string[]): void {
    const payload = { ...this.form.getRawValue(), erroriDaBypassare: erroriDaBypassare };
    this.ageaService
      .exportQdCtoAgea(payload)
      .subscribe(risp => {
        if (risp.RispostaOK) {
          this.messageService.successMessage(this.translocoService.translate('EsportazioneEseguita'));

          // Reload grid from first page
          this.stateSubject.next({ ...this.stateSubject.value, skip: 0 });
        } else {
          if (risp.ErroriGias && risp.ErroriGias.length > 0) {

            let ExportWarning = risp.ErroriGias.filter(e => e.severity === ErroreGias_Severity.Warning);

            if (ExportWarning && ExportWarning.length > 0) {
              this.showExportWarning(ExportWarning);
            }
          }
        }
      });
  }

  parseRawState(item: HubAgeaResult): string {
    if (item.loadingStageState?.state != null) {
      return `LOADING STAGE => ${item.loadingStageState.state}`;
    }

    if (item.submissionState?.state != null) {
      return `SUBMISSION => ${item.submissionState.state}`;
    }

    return '';
  }

  parseState(item: HubAgeaResult): string {
    if (item.loadingStageState?.state == LOADING_STAGE_COMPLETED) {
      return 'Fornitura valida';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_ERROR) {
      return 'Fornitura non accettata (in fase di validazione)';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_QUEUED || item.loadingStageState?.state == LOADING_STAGE_PROCESSING || item.loadingStageState?.state == LOADING_STAGE_TRANSMISSION_ERROR) {
      return 'Verifica validità in corso';
    }

    if (item.submissionState?.state == SUBMISSION_QUEUED) {
      return 'Trasmessa correttamente, in attesa di validazione';
    }

    if (item.submissionState?.state == SUBMISSION_ERROR) {
      return 'Fornitura non accettata (in trasmissione)';
    }

    if (item.submissionState?.state == SUBMISSION_TRANSMISSION_ERROR) {
      return 'Trasmissione in corso';
    }

    return '';
  }

  parseMessage(item: HubAgeaResult): string {
    if (item.loadingStageState?.state == LOADING_STAGE_COMPLETED) {
      return '';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_ERROR) {
      return item.loadingStageState?.message;
    }

    if (item.loadingStageState?.state == LOADING_STAGE_QUEUED || item.loadingStageState?.state == LOADING_STAGE_PROCESSING) {
      return '';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_TRANSMISSION_ERROR) {
      return item.loadingStageState?.message;
    }

    if (item.submissionState?.state == SUBMISSION_QUEUED) {
      return '';
    }

    if (item.submissionState?.state == SUBMISSION_ERROR) {
      const segnalazizoni = JSON.parse(item.submissionState?.message);
      let messaggio = '';
      if ((segnalazizoni != undefined) && (segnalazizoni != null)) {
        for (let segnalazizone in segnalazizoni.AgeaError) {
          if (messaggio != '')
            messaggio += "<br />";
          messaggio += segnalazizoni.AgeaError[segnalazizone];
        }
      } else {
        messaggio = item.submissionState?.message;
      }

      return messaggio;
    }

    if (item.submissionState?.state == SUBMISSION_TRANSMISSION_ERROR) {
      const segnalazizoni = JSON.parse(item.submissionState?.message);
      let messaggio = '';
      if ((segnalazizoni != undefined) && (segnalazizoni != null)) {
        for (let segnalazizone in segnalazizoni.GiasError) {
          if (messaggio != '')
            messaggio += "<br />";
          messaggio += segnalazizoni.GiasError[segnalazizone];
        }
      } else {
        messaggio = item.submissionState?.message;
      }

      return messaggio;
    }
  }

  parseGiasMessage(item: HubAgeaResult): string {
    if (item.loadingStageState?.state == LOADING_STAGE_COMPLETED) {
      return '';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_ERROR) {
      return item.loadingStageState?.message;
    }

    if (item.loadingStageState?.state == LOADING_STAGE_QUEUED || item.loadingStageState?.state == LOADING_STAGE_PROCESSING) {
      return '';
    }

    if (item.loadingStageState?.state == LOADING_STAGE_TRANSMISSION_ERROR) {
      return item.loadingStageState?.message;
    }

    if (item.submissionState?.state == SUBMISSION_QUEUED) {
      const segnalazizoni = JSON.parse(item.submissionState?.message);
      let messaggio = '';
      if ((segnalazizoni != undefined) && (segnalazizoni != null)) {
        for (let segnalazizone in segnalazizoni.GiasWarning) {
          if (messaggio != '')
            messaggio += "<br />";
          messaggio += segnalazizoni.GiasWarning[segnalazizone];
        }
      } else {
        messaggio = item.submissionState?.message;
      }

      return messaggio;
    }

    if (item.submissionState?.state == SUBMISSION_ERROR) {
      const segnalazizoni = JSON.parse(item.submissionState?.message);
      let messaggio = '';
      if ((segnalazizoni != undefined) && (segnalazizoni != null)) {
        for (let segnalazizone in segnalazizoni.GiasError) {
          if (messaggio != '')
            messaggio += "<br />";
          messaggio += segnalazizoni.GiasError[segnalazizone];
        }
      } else {
        messaggio = item.submissionState?.message;
      }
      return messaggio;
    }

    if (item.submissionState?.state == SUBMISSION_TRANSMISSION_ERROR) {
      const segnalazizoni = JSON.parse(item.submissionState?.message);
      let messaggio = '';
      if ((segnalazizoni != undefined) && (segnalazizoni != null)) {
        for (let segnalazizone in segnalazizoni.GiasError) {
          if (messaggio != '')
            messaggio += "<br />";
          messaggio += segnalazizoni.GiasError[segnalazizone];
        }
      } else {
        messaggio = item.submissionState?.message;
      }
      return messaggio;
    }
  }

  private readData(anno: number | null, impresa: Impresa | null, state: State): Observable<{ data: ExportQdCToAgeaModel[], total: number }> {
    const doCount = this.oldAnno != anno || this.oldImpresa != impresa?.partitaIva;
    this.isLoading = true;
    this.oldAnno = anno;
    this.oldImpresa = impresa?.partitaIva;

    if (impresa == null || impresa.partitaIva == '' || anno == null) {
      return of({ data: [], total: 0 });
    }

    const payload = ({ anno: anno, impresa: impresa, skip: state.skip, top: state.take } as ExportQdCtoAgeaPaginated);
    const count$ = doCount ? this.ageaService.countBundleState(payload).pipe(map(x => x.RispostaStringa)) : of(this.oldCount);
    return forkJoin([
      this.ageaService.getAllBundleState(payload).pipe(map(x => x.RispostaStringa), catchError(() => of([]))),
      count$.pipe(catchError(() => of(0)))
    ]).pipe(
      map(([data, count]: [HubAgeaResult[], number]) => {
        this.oldCount = count;
        return { data: this.parseData(data), total: count };
      }),
      catchError(() => {
        this.oldCount = 0;
        return of({ data: [], total: 0 });
      })
    );
  }

  private parseData(res: HubAgeaResult[]): ExportQdCToAgeaModel[] {
    return res.map(element => ({
      bundleId: element.bundleId,
      ageaToken: element.ageaToken,
      creationDate: element.creationDate,
      creationUser: element.creationUser,
      state: this.parseState(element),
      rawState: this.parseRawState(element),
      date: element.loadingStageState?.date ?? element.submissionState?.date,
      giasMessage: this.parseGiasMessage(element),
      message: this.parseMessage(element),
      submissionInstanceId: element.submissionState?.instanceId,
      loadingStageInstanceId: element.loadingStageState?.instanceId,
    }) as ExportQdCToAgeaModel);
  }

  refreshData(): void {
    this.stateSubject.next({ ...this.stateSubject.value });
  }

  public ShowJSONBtn() {
    return this.AuthorizedUserForLogs;
  }

  public ShowLOGBtn(dataItem) {
    return this.AuthorizedUserForLogs && (dataItem.submissionInstanceId != null || dataItem.loadingStageInstanceId != null);
  }

  showExportWarning(ExportWarning: ErroreGias[]) {

    let content = "";

    let KeyErroreGias: string[] = [];

    ExportWarning.forEach(e => {

      if (e.messaggio !== "")
        content += e.messaggio + "<br/>";

      if (e.ex !== "")
        KeyErroreGias.push(e.ex);

    });

    if (content !== "" && KeyErroreGias.length > 0) {

      let Warning_Action: Array<GiasDialogAction> = [
        { text: this.translocoService.translate('No'), returnObj: false },
        { text: this.translocoService.translate('Si'), primary: true, returnObj: true },
      ];

      let title = this.translocoService.translate('ErroriDuranteEsportazione');

      content += "<br/>" + this.translocoService.translate('InviareComunqueadAGEA');

      this.subs.add(this.giasdialogservice.dialogMessageObs_Result(title, content, Warning_Action, "auto",
        "auto", null, Dialog_Type.warning).subscribe((result: any) => {
          if (result && result.returnObj)
            this.export(KeyErroreGias);
        }));
    }

  }
}

export interface ExportQdCToAgeaModel {
  bundleId: number;
  ageaToken: string;
  creationDate: Date;
  creationUser: string;
  state: string;
  rawState: string;
  user: string;
  date: Date;
  giasMessage: string;
  message: string;
  submissionInstanceId?: string;
  loadingStageInstanceId?: string;
}

interface State {
  skip: number;
  take: number;
}
