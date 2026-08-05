import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { OperazioniService } from 'app/Service/Metaschema/operazioni.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { ImpostazioniLavorazioniGridConfigService } from './settings-lavorazioni-grid.service';
import { GiiasMultiselectTemplateSComponent, GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { ImpostazioniFormService } from "../../../services/impostazioni/impostazioni-form.service";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { enum_Impostazioni_Utenti } from "../../../../Model/Impostazioni_Utenti.enum";
import { filter, map, Observable, of, switchMap, take } from "rxjs";
import { BaseCodeDescr } from "../../../../Model/baseClass/baseCodeDescr";
import { isArray } from 'lodash';

@Component({
  standalone: false,
  selector: 'app-settings-lavorazioni-grid',
  templateUrl: './settings-lavorazioni-grid.component.html',
  styleUrls: ['./settings-lavorazioni-grid.component.css'],
  providers: [
    GiasDropDownTemplateService,
    GiasMultiSelectTemplateService,
    ...generateGridProviders(ImpostazioniLavorazioniGridConfigService, SettingsLavorazioniGridComponent)
  ]
})
export class SettingsLavorazioniGridComponent implements OnInit {
  @ViewChild('opMulti') multiOperazioni: GiiasMultiselectTemplateSComponent;
  @Input('formGroup') form: FormGroup;

  protected readonly internalForm: FormGroup = new FormGroup({
    gruppoOp: new FormControl(''),
    operazioni: new FormControl(''),
  });
  protected listaOperazioneDaGrOp: BaseCodeDescr[] = [];
  private lastGroupsRead: string[];

  constructor(
    private operazioniService: OperazioniService,
    private datashare: ProfilazioneDataShareService,
    private settingsService: ImpostazioniFormService
  ) {
  }

  private get impostazioneCod(): number {
    return this.form.get("guida").value.Impostazione_Cod;
  }

  ngOnInit(): void {
    this.datashare.formsMap.set(this.impostazioneCod, this.form);
    if (!this.form.value.valoreCorrente && this.form.value.valori?.at(0)?.valore) {
      let parsed: number[] = this.form.get("valori").value[0].valore
        .split("|")
        .map(cod => +cod) ?? [];
      this.form.get("valoreCorrente").patchValue(parsed);
    }
    this.handleGroupSettingChange();
    this.reloadOperations().subscribe(ops => this.listaOperazioneDaGrOp = ops);
  }

  async openDdl(ddlEl: GiiasMultiselectTemplateSComponent) {
    ddlEl.loading = true;
    this.reloadOperations().subscribe(ops => {
      this.listaOperazioneDaGrOp = ops;
      ddlEl.loading = false;
    });
  }

  private reloadOperations(): Observable<BaseCodeDescr[]> {
    let toRead = this.getOperationsGroups();
    if (this.setEquals(this.lastGroupsRead, toRead)) {
      return of(this.listaOperazioneDaGrOp);
    } else {
      this.lastGroupsRead = toRead;
      return this.operazioniService.caricaComboLavorazioni_PerImpostazioni(toRead)
        .pipe(take(1), map(ops => ops.map(o => new BaseCodeDescr(o.lav_cod, o.lav_des))));
    }
  }

  private handleGroupSettingChange() {
    this.settingsService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI)
      .valueChanges.pipe(
        map(ch => this.getOperationsGroups(ch)),
        filter(toRead => !this.setEquals(this.lastGroupsRead, toRead)),
        switchMap(toRead => this.operazioniService.caricaComboLavorazioni_PerImpostazioni(toRead)),
        map(newAdmissibles => newAdmissibles.map(o => new BaseCodeDescr(o.lav_cod, o.lav_des)))
      ).subscribe((newAdmissibles) => this.removeNotAdmissible(newAdmissibles));
  }

  /**
   * 
   * @param baseSettingFrom Eventually, the value of the setting
   * enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI.
   * @returns list of groups of operations selected
   */
  private getOperationsGroups(baseSettingFrom?: { valoreCorrente: string | string[] }): string[] {
    if (baseSettingFrom == undefined) {
      baseSettingFrom = this.settingsService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI).value;
    }
    let gruppi = baseSettingFrom.valoreCorrente;
    let toRead: string[] = [];
    if (typeof (gruppi) === 'string') {
      toRead = gruppi.split('|').filter(x => x !== '').map(x => x);
    } else if (isArray(gruppi)) {
      toRead = gruppi;
    }
    return toRead;
  }

  private setEquals<T>(l1: T[], l2: T[]): boolean {
    if (!l1 && !l2) return true;
    if ((!l1 && !!l2) || (!!l1 && !l2)) return false;
    if (l1.length !== l2.length) return false;
    const allL1inL2 = l1.every(item => l2.includes(item));
    const allL2inL1 = l2.every(item => l1.includes(item));
    return allL1inL2 && allL2inL1;
  }

  private removeNotAdmissible(newAdmissibles: BaseCodeDescr[]) {
    const currentValue = this.form.get("valoreCorrente").value;
    if (isArray(currentValue)) {
      const newValue = currentValue.filter(x => newAdmissibles.map(o => o.codice).includes(x));
      this.form.get("valoreCorrente").patchValue(newValue);
    }
  }

}
