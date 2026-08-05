import { Component, OnInit, AfterViewChecked } from '@angular/core';
import { enum_PaginaImpostazioni } from '../impostazioni.model';
import { ImpostazioniFormService } from '../../services/impostazioni/impostazioni-form.service';
import { ProfilazioneDataShareService } from "../../services/profilazione-data-share.service";
import { enum_PagineProfilazione } from "../../models/PaginaProfilazione.model";
import { Utente, Widget_Configuration, WidgetsClient } from "../../../Service/api.service";
import { TranslocoService } from "@jsverse/transloco";
import { catchError, forkJoin, map, Observable, of, switchMap, take } from 'rxjs';
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { MappaturaIsolamentiComponent } from 'app/profilazione/impostazioni-utente/tipi-campo-impostazioni/mappatura-isolamenti/mappatura-isolamenti.component';
import { MappaturaIsolamentiService } from 'app/profilazione/impostazioni-utente/tipi-campo-impostazioni/mappatura-isolamenti/mappatura-isolamenti.service';

@Component({
  standalone: false,
  selector: 'app-impostazioni-utente',
  templateUrl: './impostazioni-utente.component.html',
  styleUrls: ['./impostazioni-utente.component.css'],
})
export class ImpostazioniUtenteComponent implements OnInit, AfterViewChecked {

  public readonly MASTER_EDIT = this.impostazioniService.MASTER_EDIT;
  user: Utente | null = null;
  widgetTabTitle: string;
  PMITabTitle: string;
  AuditTabTitle: string;
  showPMITab: boolean = false;

  private _widgets: Widget_Configuration[] = [];

  constructor(
    private impostazioniService: ImpostazioniFormService,
    private datashare: ProfilazioneDataShareService,
    private transloco: TranslocoService,
    private dialog: GiasDialogService,
    private widgetsClient: WidgetsClient,
    private mappaturaIsolamentiService: MappaturaIsolamentiService
  ) {
    this.user = { UserName: '-1' };
    this.widgetTabTitle = this.transloco.translate("Widgets");
    this.PMITabTitle = this.transloco.translate("ParametriMappaturaIsolamenti");
    this.AuditTabTitle = this.transloco.translate("AuditDocumentale");
    this.mappaturaIsolamentiService.loadClassiSpecieSementieri()
      .subscribe((res: any[]) => this.showPMITab = (res != undefined && res.length > 0));
  }

  public get usernameText(): string {
    let username = this.impostazioniService.masterService.objP_utenti.UtenteUsername;
    if (this.impostazioniService.masterService.isSuperuser()) {
      username += ' (superuser)';
    }
    return username;
  }

  ngOnInit(): void {
    this.setUsageArea();
    this.impostazioniService.utentiSelezionati = [];
  }
  ngAfterViewChecked(): void {
    const tabs = document.querySelectorAll("li[role=tab]");
    for (let i = 0; i < tabs.length; i++) {
      if ((tabs[i] as HTMLElement).innerText == this.PMITabTitle) {
        (tabs[i] as HTMLElement).style.display = this.showPMITab ? "block" : "none";
      }
    }
  }

  onSave() {
    forkJoin([
      this.impostazioniService.salvaImpostazioniModificateObs(),
      this.saveWidgets()
    ]).pipe(take(1)).subscribe((rs) => {
      if (rs.every(ok => ok)) {
        this.dialog.salvataggioOk();
      } else {
        this.dialog.baseError('Error_', 'QualcosaEAndatoStorto');
      }
    })
  }

  private setUsageArea() {
    let url = this.datashare.currentPage.value;
    if (url === enum_PagineProfilazione.UTENTI_IMPOSTAZIONI) {
      this.impostazioniService.USAGE_AREA = enum_PaginaImpostazioni.SUPERUSER;
    } else if (url === enum_PagineProfilazione.IMPRESE_IMPOSTAZIONI) {
      this.impostazioniService.USAGE_AREA = enum_PaginaImpostazioni.AZIENDE_CENTRI;
    }
  }

  onLoadedWidgets(widgetList: Widget_Configuration[]) {
    this._widgets = this._widgets.concat(widgetList);
  }

  private saveWidgets(): Observable<boolean> {
    return this.widgetsClient.widgetsScriviAggiornaWidgetUtente({ widgets: this._widgets })
      .pipe(
        map(r => r.RispostaOK ? r.RispostaStringa : ['']),
        map((res: string[]) => res.every(r => r == 'OK'))
      );
  }
}
