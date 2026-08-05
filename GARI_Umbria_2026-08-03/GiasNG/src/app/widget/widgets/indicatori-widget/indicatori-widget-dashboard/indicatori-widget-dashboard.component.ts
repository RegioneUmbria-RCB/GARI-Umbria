import { Component, Input, OnInit } from '@angular/core';
import { Band, Indicatore } from '../indicatori-widget.models';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from 'app/Model/siti.enum';

const DSS_PARAM_KEY = 'parametri';
const DSS_EXTERNAL_LOAD_KEY = 'externalLoad';

@Component({
  standalone: false,
  selector: 'app-indicatori-widget-dashboard',
  templateUrl: './indicatori-widget-dashboard.component.html',
  styleUrls: ['./indicatori-widget-dashboard.component.css'],
})
export class IndicatoriWidgetDashboardComponent implements OnInit {
  @Input() data: Indicatore[] = [];

  bandStyles = new Map<Band, string>();
  groups: string[] = [];

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneRichiesteService: GestioneRichiesteService
  ) { }

  ngOnInit(): void {
    for (const indic of this.data) {
      indic.BackgroundColor = "linear-gradient(90deg, "
      for (const band of indic.Risultato.Bands) {
        this.bandStyles.set(band, this.computeBandStyle(indic, band));
      }

      indic.BackgroundColor = indic.BackgroundColor.slice(0, -1) + ");";
    }

    this.groups = [...new Set(this.data.map(x => x.Stazione))].sort((a, b) => a.localeCompare(b));
  }

  getStatusString(status: number): string {
    switch (status) {
      case -1:
        return "outofrange";
      case 1:
        return "warning";
      case 2:
        return "error";
      case 3:
        return "progress";
      default: // or 0
        return "valid";
    }
  }

  gaugeClick(indic: Indicatore): void {
    const params = {
      DataInizio: indic.Risultato.DataInizio,
      DataFine: indic.Risultato.DataFine,
      Tipo_Sorgente: indic.Parametri.Tipo_Sorgente,
      Stazione_Cod: indic.Parametri.Stazione_Cod,
      Mod_Cod: indic.Parametri.Mod_Cod,
      Veg_Cod: indic.Parametri.Veg_Cod,
      Avv_Cod: indic.Parametri.Avv_Cod,
      Alg_Cod: indic.Parametri.Alg_Cod,
      ParametriElaborazione: indic.Parametri.ParametriElaborazione
    };

    const queryParams = [
      { key: DSS_PARAM_KEY, value: encodeURIComponent(JSON.stringify(params)), codifica: false },
      { key: DSS_EXTERNAL_LOAD_KEY, value: true, codifica: false }
    ] as ParametriAggiuntivi_QueryString[];

    this.gestioneRichiesteService
      .gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_DSS_Difesa, queryParams, this.objParametriAgendaService.getObjParamValue(), true, 0)
      .then(url => window.location.href = url);
  }

  getPercFact(indic: Indicatore): number {
    const scale_Max = indic.Risultato.Bands[indic.Risultato.Bands.length - 2].Value * (10.0 / 9.0); //Rosso per l'ultimo 10% della banda
    return 100.0 / (scale_Max - indic.Risultato.Scale_Min);
  }

  getValuePerc(indic: Indicatore): number {
    const percFact = this.getPercFact(indic);
    return Math.max(5, Math.min(95, (indic.Risultato.Value - indic.Risultato.Scale_Min) * percFact));
  }

  getBandStyle(band: Band): string {
    return this.bandStyles.get(band);
  }

  private computeBandStyle(indic: Indicatore, band: Band): string {
    const count = indic.Risultato.Bands.length;
    const i = indic.Risultato.Bands.indexOf(band);
    const nclr = parseInt(band.Color.slice(1), 16);
    const rgba = "rgba(" + (nclr >> 16) + ", " + ((nclr >> 8) & 0x00FF) + ", " + (nclr & 0x0000FF);
    const valuePerc = this.getValuePerc(indic);
    const percFact = this.getPercFact(indic);

    const prevPerc = i == 0 || i - 1 > count ? 0 : indic.Risultato.Bands[i - 1].CurrentPerc;
    const currPerc = Math.round(Math.min(100.0, band.Value * percFact));
    band.CurrentPerc = currPerc;

    if (prevPerc > 0) {
      indic.BackgroundColor += rgba + ", 0.4) " + prevPerc + "%,";
    }

    if (i < count - 1) {
      indic.BackgroundColor += rgba + ", 0.4) " + currPerc + "%,";
    }

    let ledCss = "position:absolute; width:22px; height:22px; border-radius:50%; ";
    ledCss += "left:" + ((prevPerc + currPerc) * 0.5) + "%; top:50%; transform:translateX(-50%) translateY(-50%); ";

    if (prevPerc < valuePerc && valuePerc <= currPerc) {
      ledCss += "background-color:" + band.Color + "; ";
      ledCss += "box-shadow:rgba(0, 0, 0, 0.2) 0 -1px 7px 1px, inset #444444 0 -1px 6px, " + band.Color + " 0 2px 12px; ";
    } else {
      ledCss += "background-color:" + rgba + ", 0.2); ";
      ledCss += "box-shadow:rgba(0, 0, 0, 0.2) 0 -1px 7px 0px, inset #888888 0 -1px 6px; ";
    }

    return ledCss;
  }
}
