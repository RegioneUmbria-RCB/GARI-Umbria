import { Component, ViewChild } from '@angular/core';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { Observable, tap } from 'rxjs';
import { GISGestionePianoRateoService } from './GIS-gestione-piano-rateo.service';
import { GISAnalisiMappeSatellitariWindowService } from 'app/GIS/GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';

@Component({
  standalone: false,
  selector: 'gis-gestione-piano-rateo',
  templateUrl: './GIS-gestione-piano-rateo.component.html',
  styleUrls: ['./GIS-gestione-piano-rateo.component.css']
})
export class GISGestionePianoRateoComponent {
  @ViewChild('picker') picker: ISubmitComponent;
  @ViewChild('loader') loader: ISubmitComponent;

  isLoading: boolean = false;
  hasUploadPermission = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Caricamento_Utilizzo_Mappe_Prescrizione_Personalizzate, 0) || true; // TODO remove true
  tabSelected = 0;

  isOpen$ = this.gisGestionePianoRateoService.isOpen$
    .pipe(tap(open => {
      if (open) {
        this.gisAnalisiMappeSatellitariWindowService.nextExternalLoadOnPolygons(true);
      }
    }));

  constructor(
    private gisGestionePianoRateoService: GISGestionePianoRateoService,
    private permessiUtenteService: PermessiUtenteService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService
  ) { }

  close(): void {
    this.gisGestionePianoRateoService.close();
  }

  isValid(): boolean {
    if (this.tabSelected == 0) {
      return this.picker?.isValid();
    }

    if (this.tabSelected == 1) {
      return this.loader?.isValid();
    }
  }

  submit(): void {
    if (this.tabSelected == 0) {
      this.picker?.submit();
      return;
    }

    if (this.tabSelected == 1) {
      this.loader?.submit();
      return;
    }
  }
}

export interface ISubmitComponent {
  isValid(): boolean;
  submit(): void;
}
