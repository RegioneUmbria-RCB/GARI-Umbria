import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { ImpostazioniFormService } from '../../../services/impostazioni/impostazioni-form.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridVarietaService } from "./grid-varieta.service";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { enum_Impostazioni_Utenti } from "../../../../Model/Impostazioni_Utenti.enum";
import { ImpostazioniFormItem } from 'gias-ui-kit';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { BaseCodeDescr } from "../../../../Service/api.service";
import { enum_TipoControllo } from 'gias-ui-kit';
import { map, Observable, of, switchMap, takeUntil, tap } from 'rxjs';
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-settings-specie-varieta',
  templateUrl: './settings-specie-varieta.component.html',
  styleUrls: ['./settings-specie-varieta.component.css'],
  providers: [
    GiasMultiSelectTemplateService,
    GiasDropDownTemplateService,
    ...generateGridProviders(GridVarietaService, SettingsSpecieVarietaComponent)]
})
export class SettingsSpecieVarietaComponent implements OnInit {
  @Input() formGroup: FormGroup;
  @ViewChild('grid') grid: GiasKendoGridComponent;

  public readonly impostazioneFiltroVarieta = enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA;
  public readonly impostazioneFiltroSpecieVegetali = enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI;
  protected readonly multiselect = enum_TipoControllo.MULTISELECT;
  protected specieDaGruVeg: BaseCodeDescr[] = [];

  constructor(
    private specieVegetaliService: SpecieVegetaliService,
    private settingsService: ImpostazioniFormService,
    private datashare: ProfilazioneDataShareService,
  ) { }

  protected get impostazioneCod(): number {
    return this.formGroup.value.guida.Impostazione_Cod;
  }

  protected get formGroupValue(): ImpostazioniFormItem {
    return this.formGroup.value as ImpostazioniFormItem;
  }

  private get isDisabled(): boolean {
    return this.formGroup.disabled;
  }

  private get filtroGruppiVegetali(): number[] {
    let gruppi = this.settingsService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI)
      .get("valoreCorrente").value;
    if (typeof (gruppi) === 'string') {
      gruppi = gruppi.split('|').filter(x => x !== '').map(x => +x);
    }
    return gruppi;
  }
  ngOnInit(): void {
    this.datashare.formsMap.set(this.impostazioneCod, this.formGroup);
    if (this.impostazioneCod === this.impostazioneFiltroSpecieVegetali) {
      this.initFiltroSpecie();
    } else if (this.impostazioneCod === this.impostazioneFiltroVarieta) {
      setTimeout(() => this.disableGridIfReadonly(), 300);
    }
  }

  protected onOpenFiltroSpecie(ev: GiiasMultiselectTemplateSComponent) {
    ev.loading = true;
    this.loadSpecieFromGruppiVegetali().subscribe((list) => {
      this.specieDaGruVeg = list;
      ev.loading = false;
    });
  }

  private initFiltroSpecie() {
    if (!this.formGroupValue.valoreCorrente && this.formGroupValue.valori?.at(0)?.valore) {
      let parsed: number[] = this.formGroup.get("valori").value[0].valore
        .split("|")
        .map(cod => +cod) ?? [];
      this.formGroup.get("valoreCorrente").patchValue(parsed);
    }
    this.loadSpecieFromGruppiVegetali().subscribe(list => this.specieDaGruVeg = list);
    this.handleSpecieEvents();
  }

  private loadSpecieFromGruppiVegetali(): Observable<Specie[]> {
    let gruppi = this.filtroGruppiVegetali;
    if (gruppi.length > 0) {
      return this.specieVegetaliService.leggiDaGruppiVegetali(gruppi);
    } else {
      return of([]);
    }
  }

  private handleSpecieEvents() {
    this.updateSpecieOnGroupsChange();
    this.formGroup.get("valoreCorrente").valueChanges.subscribe(() => {
      this.settingsService
        .findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI)
        .markAllAsTouched();
    });
  }

  private updateSpecieOnGroupsChange() {
    let orig = this.filtroGruppiVegetali;
    this.settingsService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI)
      .get("valoreCorrente").valueChanges
      .pipe(
        takeUntil(this.datashare.exitProfilazione),
        map(newVal => {
          let toRemove = [];
          if (orig.length > newVal.length) {
            toRemove = orig.filter(x => !newVal.includes(x));
          }
          orig = newVal;
          return toRemove;
        }),
        switchMap((toRemove: number[]) => {
          if (toRemove.length > 0) {
            return this.specieVegetaliService.leggiDaGruppiVegetali(toRemove);
          } else {
            return of([]);
          }
        })
      ).subscribe((toRemove: Specie[]) => {
        const cods = toRemove.map(s => s.codice);
        const currentValue = this.formGroup.get("valoreCorrente").value;
        const newValue = currentValue.filter(x => !cods.includes(x));
        this.formGroup.get("valoreCorrente").patchValue(newValue);
      });
  }

  private disableGridIfReadonly() {
    if (this.isDisabled && this.grid) {
      this.grid.publicService.disable();
    }
  }

}
