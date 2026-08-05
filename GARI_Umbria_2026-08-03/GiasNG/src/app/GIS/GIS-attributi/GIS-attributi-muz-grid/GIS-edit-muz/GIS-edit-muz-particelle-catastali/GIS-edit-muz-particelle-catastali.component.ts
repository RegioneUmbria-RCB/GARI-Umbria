import { Component, Input, ViewChild } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { GISEditMuzParticelleCatastaliGridConfigService } from "./GIS-edit-muz-particelle-catastali-grid-config.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { GISParticelleCatastaliModel } from "app/GIS/GIS-particelle-catastali/GIS-particelle-catastali.service";
import { Enum_TitoloPossesso_MUZ, MUZ, ParticelleCatastali_MUZ } from "app/Service/api.service";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";

@Component({
  standalone: false,
  selector: 'gis-edit-muz-particelle-catastali',
  templateUrl: './GIS-edit-muz-particelle-catastali.component.html',
  styleUrls: ['./GIS-edit-muz-particelle-catastali.component.css'],
  providers: [...generateGridProviders(GISEditMuzParticelleCatastaliGridConfigService, GISEditMuzParticelleCatastaliComponent)]
})
export class GISEditMuzParticelleCatastaliComponent {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  @Input() inputMuz: MUZ;

  selectParticelleCatastali = false;
  piva = this.objParametriAgendaService.getObjParamValue().Piva;
  faDelete = faTrashAlt;

  constructor(private objParametriAgendaService: ObjParametriAgendaService) { }

  getParticelleCatastaliIds(): string[] {
    return this.inputMuz.Particelle_Catastali.map(x => GISEditMuzParticelleCatastaliGridConfigService.getGisParticelleCatastaliMuzRowId(x));
  }

  addParticelleCatastali(particelleCatastali: GISParticelleCatastaliModel[]): void {
    const particelle = particelleCatastali.map(x => GISEditMuzParticelleCatastaliComponent.parseModel(x));

    this.inputMuz.Particelle_Catastali_Aggiungi = this.inputMuz.Particelle_Catastali_Aggiungi.concat(particelle);
    this.inputMuz.Particelle_Catastali = this.inputMuz.Particelle_Catastali.concat(particelle);

    this.selectParticelleCatastali = false;
    this.kendoGrid.forceReload();
  }

  deleteParticelleCatastali(particellaCatastale: ParticelleCatastali_MUZ): void {
    const key = GISEditMuzParticelleCatastaliGridConfigService.getGisParticelleCatastaliMuzRowId(particellaCatastale);
    this.inputMuz.Particelle_Catastali = this.inputMuz.Particelle_Catastali.filter(x => GISEditMuzParticelleCatastaliGridConfigService.getGisParticelleCatastaliMuzRowId(x) != key);

    if (this.inputMuz.Particelle_Catastali_Aggiungi.find(x => GISEditMuzParticelleCatastaliGridConfigService.getGisParticelleCatastaliMuzRowId(x) == key)) {
      // This item has been added but not saved
      this.inputMuz.Particelle_Catastali_Aggiungi = this.inputMuz.Particelle_Catastali_Aggiungi.filter(x => GISEditMuzParticelleCatastaliGridConfigService.getGisParticelleCatastaliMuzRowId(x) != key);
    } else {
      // This item was loaded and not just added
      this.inputMuz.Particelle_Catastali_Elimina.push(particellaCatastale);
    }

    this.kendoGrid.forceReload();
  }

  private static parseModel(item: GISParticelleCatastaliModel): ParticelleCatastali_MUZ {
    return {
      Provincia: item.Prov,
      Comune: item.Com,
      Sezione: item.SEZIONE,
      Foglio: item.FOGLIO,
      Numero: item.NUMERO,
      Subalterno: item.SUBALTERNO,
      Sup_Condotta: item.Sup_Condotta,
      Condotta_Validita_Inizio: GISEditMuzParticelleCatastaliComponent.parseDate(item.Validita_Inizio, AGRODATAINIZIO),
      Condotta_Validita_Fine: GISEditMuzParticelleCatastaliComponent.parseDate(item.Validita_Fine, AGRODATAFINE),
      Titolo_Possesso: Object.keys(Enum_TitoloPossesso_MUZ)[item.Titolo_possesso_cod],
      Provincia_Esteso: item.PROVINCIA,
      Comune_Esteso: item.COMUNE,
    } as ParticelleCatastali_MUZ;
  }


  private static parseDate(date: string, defaultDate: Date): Date {
    if (date == null) {
      return defaultDate;
    }

    const datetime = date.split(' ');
    if (datetime.length != 2) {
      return defaultDate;
    }

    const splitDate = datetime[0].split('/');
    if (splitDate.length != 3) {
      return defaultDate;
    }

    const result = new Date(+splitDate[2], +splitDate[1], +splitDate[0]);
    return result instanceof Date && !isNaN(result.getTime()) ? result : defaultDate;
  }
}
