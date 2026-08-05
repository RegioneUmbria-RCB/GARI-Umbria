import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { GISModality } from "app/GIS/GIS-enum/GIS-feature";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { GestioneMultiAziendaService } from "app/widget-config/gestione-multiazienda.service";
import { combineLatest, Subject, takeUntil } from 'rxjs';

@Component({
  standalone: false,
  selector: "app-mappa-multiazienda-widget",
  templateUrl: "./mappa-multiazienda-widget.component.html",
  styleUrls: ["./mappa-multiazienda-widget.component.scss"],
})
export class MappaMultiAziendaComponent implements OnInit, OnDestroy {
  @Input() codice: string | null = null;
  loading: boolean;
  urlGIS: string;

  private signal$ = new Subject<void>();

  constructor(
    private gestioneRichiesteService: GestioneRichiesteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneMultiAziendaService: GestioneMultiAziendaService
  ) { }

  ngOnInit(): void {
    combineLatest({
      nazione: this.gestioneMultiAziendaService.nazioneMultiAzienda$,
      anno: this.gestioneMultiAziendaService.annoMultiAzienda$
    })
    .pipe(takeUntil(this.signal$))
    .subscribe(({ nazione, anno }) => {
      this.loadMap(nazione, anno);
    });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  loadMap(nazione: any, anno: any): void {
    this.loading = true;

    this.gestioneRichiesteService.getPathPrefixGestionePassaggioAltroSitoIFrame()
      .subscribe(prefix => {
        this.gestioneRichiesteService.gestionePassaggioAltroSito(
          Enum_SiteRedirector.GiasNG,
          enum_PagineGiasNG.Pagina_GIS,
          [],
          this.objParametriAgendaService.getObjParamValue()
        ).then((resp) => {
          let modality: GISModality;

          if (this.codice == "MappaPaesePoligoni")
            modality = GISModality.PaesePoligoniMultiAzienda;
          else modality = GISModality.PaeseDistribuzioneMultiAzienda;

          let year = anno?.text ?? "";
          let country = nazione?.code ?? "";

          this.urlGIS =
            this.gestioneRichiesteService.getAbsolutePath(
              ":" + window.location.port + prefix + "/" + resp
            ) +
            "?seFrame=1&modalita=" + modality
            + "&nazione=" + country
            + "&anno=" + year;
        }).finally(() => {
          setTimeout(() => this.loading = false, 500);
        });
      });
  }
}
