import { Component, OnInit, ViewChild } from '@angular/core';
import { faCopy, faEye } from '@fortawesome/free-solid-svg-icons';
import { generateGridProviders } from 'gias-kendo-grid';
import { ImpostazioniAziendeCentriGridConfigService } from './grid-aziende-centri.service';
import { FormControl, FormGroup } from "@angular/forms";
import { of, switchMap } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { GiasDialogService } from "../../../../../Service/gias-dialog.service";
import { ImpostazioniFormService } from "../../../../services/impostazioni/impostazioni-form.service";
import {GiasDropDownTemplateService} from 'gias-ui-kit';
import { ImpresaDto } from "../../../../../Service/api.service";
import { ImpostazioniAziendeCentriService } from 'app/profilazione/services/impostazioni/impostazioni-aziende-centri.service';

@Component({
  standalone: false,
  selector: 'app-grid-aziende-centri',
  templateUrl: './grid-aziende-centri.component.html',
  styleUrls: ['./grid-aziende-centri.component.css'],
  providers: [
    ...generateGridProviders(ImpostazioniAziendeCentriGridConfigService, GridAziendeCentriComponent),
    GiasDropDownTemplateService
  ]
})
export class GridAziendeCentriComponent implements OnInit {
  @ViewChild('grid') grid: any;
  @ViewChild('copySettingsRef') copySettingsRef: any;

  public centersEnabled = false;
  public form = new FormGroup({
    showCenters: new FormControl(false),
    impresa: new FormControl<ImpresaDto>(null)
  });

  protected readonly visible = faEye;
  protected readonly notVisible = faEye;
  protected readonly faCopy = faCopy;
  protected impreseDdl = [];

  constructor(
    private transloco: TranslocoService,
    private dialogService: GiasDialogService,
    private settingsService: ImpostazioniFormService,
    private aziendeSettings: ImpostazioniAziendeCentriService
  ) { }

  ngOnInit(): void {
    this.form.get("showCenters").valueChanges.subscribe(() => this.showCenters());
  }

  async showCenters() {
    this.centersEnabled = !this.centersEnabled;
    this.aziendeSettings.impreseSelezionate = [];
    this.grid.rows.filter(r => r['Selected']).forEach(r => r['Selected'] = false);
    const emit = () => this.aziendeSettings.modeChangeEvent.emit({
      type: 'centersToggled',
      value: this.centersEnabled
    });
    if (this.aziendeSettings.ImpresexCentri.length === 0) {
      this.aziendeSettings.caricaImpreseXCentri().subscribe(() => emit());
    } else emit();
  }

  onEditSettings() {
    const sel = this.grid.rows.filter(row => row.Selected === true);
    if (sel.length > 0) {
      this.aziendeSettings.impreseSelezionate = sel;
      return;
    }
  }

  onSelectionChange(ev: any) {
    ev.selectedRows.forEach(row => {
      this.aziendeSettings.impreseSelezionate.push(row);
    });
    ev.deselectedRows.forEach(row => {
      let index = this.aziendeSettings.impreseSelezionate.findIndex(r => r.index === row.index);
      this.aziendeSettings.impreseSelezionate.splice(index, 1);
    });
    this.onEditSettings();
  }

  openCopySettingsDialog() {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      winRefs[i]?.classList.remove('resized');
    }
    const buildID = (azienda: ImpresaDto) => azienda.piva + '_' + azienda.Sa_Cod;
    this.impreseDdl = this.aziendeSettings.imprese;
    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('prof.SelezioneUtente'),
      this.copySettingsRef
    ).pipe(switchMap(res => {
      if (res['returnObj']) {
        const template = buildID(this.form.value.impresa);
        const base = this.aziendeSettings.impreseSelezionate.map(az => buildID(az));
        return this.settingsService.copiaImpostazioni(base, template);
      }
      return of(null);
    })).subscribe((res) => {
      this.form.get('impresa').patchValue(null);
    });
  }

}


