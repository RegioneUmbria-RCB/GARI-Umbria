import { Component, effect, input, Input, OnInit, output } from '@angular/core';
import {
  RispostaStandard_1OfWidget_Complete_Configuration,
  Widget_Configuration,
  WidgetsClient
} from "../../../Service/api.service";
import { IUtenteDTO } from "../../../profilazione/models/utente-dto.model";
import { finalize, Observable, tap } from 'rxjs';
import { WidgetConfigService } from 'app/widget-config/widget-config.service';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { LoaderComponent } from "@progress/kendo-angular-indicators";
import { TranslocoPipe } from "@jsverse/transloco";
import { TooltipDirective } from "@progress/kendo-angular-tooltip";
import { GestioneMultiAziendaService } from "../../gestione-multiazienda.service";
import { NgClass } from "@angular/common";

@Component({
  standalone: true,
  selector: 'app-widget-config-list',
  imports: [
    ReactiveFormsModule,
    LoaderComponent,
    TranslocoPipe,
    TooltipDirective,
    FormsModule,
    NgClass
  ],
  templateUrl: './widget-config-list.component.html',
  styleUrl: './widget-config-list.component.css'
})
export class WidgetConfigListComponent implements OnInit {
  @Input() user: IUtenteDTO | null = null;
  useMultiCompany = input<boolean>(false);
  customSelected = input<boolean>(true);
  /** Prevents the saving on change of the widgets */
  preventDefaultSave = input<boolean>(false);
  listID = input<string>('widget_list');

  /** Emitted when the check for a widget is changed */
  configChange = output<Widget_Configuration>();
  /** Emitted once the widgets have been loaded */
  loadedWidgets = output<Widget_Configuration[]>();

  protected widgets: Widget_Configuration[] = [];
  protected widgetsBlocked: boolean;
  protected isLoading = false;

  private presetWidgets: Widget_Configuration[] = [];
  private userWidgets: Widget_Configuration[] = [];

  constructor(
    private widgetsClient: WidgetsClient,
    private widgetConfigService: WidgetConfigService,
    private gestioneMultiAziendaService: GestioneMultiAziendaService
  ) {
    effect(() => this.handleMultiCompanyChange(this.useMultiCompany()));
  }

  ngOnInit() {
    this.getData();
  }

  public getData(): void {
    this.isLoading = true;
    this.widgetsClient
      .widgetsElencoWidgetsXConfigurazione(this.user.UserName)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe((x: RispostaStandard_1OfWidget_Complete_Configuration) => {
        this.widgetConfigService.$widgetsBlocked.next(x.RispostaStringa.WidgetsBloccati);
        this.widgetsBlocked = x.RispostaStringa.WidgetsBloccati;
        this.presetWidgets = x.RispostaStringa.PresetIniziale.sort(this.sort);
        this.userWidgets = this.getAllWidgets(this.presetWidgets, x.RispostaStringa.UserWidgets).sort(this.sort);

        this.handleUsernameMissing();
        this.forceMultiCompanyCountryWidget()

        this.widgets = this.userWidgets.filter(x => x.MultiAziendale === this.useMultiCompany());
        this.loadedWidgets.emit(this.widgets);
        console.debug(this.widgets);
      });
  }

  public resetConfiguration(): Observable<any> {
    return this.widgetsClient.widgetsConfigurazioneReset(this.user.UserName)
      .pipe(tap(() => this.widgetConfigService.changeConfiguration(this.presetWidgets)));
  }

  onConfigChange(widgetConfig: Widget_Configuration): void {
    if (!this.preventDefaultSave()) {
      this.widgetsClient
        .widgetsConfigurazioneAggiorna(widgetConfig)
        .subscribe(() => this.widgetConfigService.changeConfiguration([widgetConfig]));
    }
    this.configChange.emit(widgetConfig);
  }

  /**
   * Se non sono presenti widget per l'utente corrente, i widget caricati non avranno valorizzato il campo `UserName`.
   * Occorre valorizzare il campo con l'username dell'utente corrente per permetterne il salvataggio in seguito.
   * @private
   */
  private handleUsernameMissing() {
    if (this.userWidgets.every(w => !w.UserName)) {
      console.debug('loaded widget w/o username');
      this.userWidgets.forEach(w => w.UserName = this.user.UserName);
    }
  }

  /** occorre forzare la visibilità del finto Widget Paese MultiAzienda */
  private forceMultiCompanyCountryWidget() {
    const nazioneMultiAziendaWidget = this.userWidgets.filter(x => x.Codice == 'PaeseWidgetsMultiAzienda');
    if (nazioneMultiAziendaWidget.length > 0) {
      if (!nazioneMultiAziendaWidget[0].Visibile) {
        nazioneMultiAziendaWidget[0].Visibile = true;
        this.onConfigChange(nazioneMultiAziendaWidget[0]);
      }
    }
  }

  private handleMultiCompanyChange(checked: boolean) {
    if (checked) {
      // mi carico prima gli ultimi 5 anni
      const currentYear = new Date().getFullYear();
      for (let i = 0; i < 5; i++) {
        this.gestioneMultiAziendaService.yearsList.push({ 'value': i, 'text': currentYear - i });
      }

      this.gestioneMultiAziendaService.setAnnoMultiAzienda(this.gestioneMultiAziendaService.yearsList[0]);
      this.gestioneMultiAziendaService.getCountriesListFromAPI(this.gestioneMultiAziendaService.yearsList[0]);
    }

    this.gestioneMultiAziendaService.widgetsMultiAzienda = checked;
    this.widgets = this.userWidgets.filter(x => x.MultiAziendale === checked);

    if (this.widgets.length > 0)    //forzo il ricaricamento della matrice dei widget in questo modo
      this.widgetConfigService.changeConfiguration(this.widgets);
  }

  private sort(a: Widget_Configuration, b: Widget_Configuration): number {
    return a.Titolo?.localeCompare(b.Titolo);
  }

  private getAllWidgets(presetWidgets: Widget_Configuration[], userWidgets: Widget_Configuration[]): Widget_Configuration[] {
    const missing = presetWidgets.filter(preset => userWidgets.find(user => user.IdWidget == preset.IdWidget) == null)
      .map(x => ({ ...x, new: true, Visibile: false }));
    return userWidgets.concat(missing);
  }

}
