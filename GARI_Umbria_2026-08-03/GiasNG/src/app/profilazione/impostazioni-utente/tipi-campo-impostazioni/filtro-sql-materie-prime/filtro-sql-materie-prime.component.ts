import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { generateGridProviders } from 'gias-kendo-grid';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GridFiltroSqlMateriePrimeService } from "./grid-filtro-sql-materie-prime.service";
import { CategorieMagazzinoService } from "../../../services/categorie-magazzino.service";
import { ImpostazioniUtentiService } from "../../../services/impostazioni/impostazioni-utenti.service";
import { lastValueFrom } from "rxjs";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";

@Component({
  standalone: false,
  selector: 'app-filtro-sql-materie-prime',
  templateUrl: './filtro-sql-materie-prime.component.html',
  styleUrls: ['./filtro-sql-materie-prime.component.css'],
  providers: [
    ...generateGridProviders(GridFiltroSqlMateriePrimeService, FiltroSqlMateriePrimeComponent),
    GiasDropDownTemplateService
  ]
})
export class FiltroSqlMateriePrimeComponent implements OnInit {
  @Input('formGroup') form: FormGroup;

  constructor(private datashare: ProfilazioneDataShareService) { }

  private get impostazioneCod(): number {
    return this.form.value.guida.Impostazione_Cod;
  }

  ngOnInit(): void {
    this.datashare.formsMap.set(this.impostazioneCod, this.form);
  }

}
