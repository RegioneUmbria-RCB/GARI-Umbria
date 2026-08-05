import {Component, ViewChild} from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {GridAziendeCentriImpostazioniGridConfigService} from './grid-aziende-centri-impostazioni.service';
import {GiasDialogService} from "../../../../../Service/gias-dialog.service";
import {GiasKendoGridComponent} from 'gias-kendo-grid';
import {Imprese_Impostazioni} from "../../../../../Service/master.service";
import { ImpostazioniAziendeCentriService } from 'app/profilazione/services/impostazioni/impostazioni-aziende-centri.service';

@Component({
  standalone: false,
  selector: 'app-grid-aziende-centri-impostazioni',
  templateUrl: './grid-aziende-centri-impostazioni.component.html',
  styleUrls: ['./grid-aziende-centri-impostazioni.component.css'],
  providers: [
    ...generateGridProviders(GridAziendeCentriImpostazioniGridConfigService, GridAziendeCentriImpostazioniComponent)
  ]
})
export class GridAziendeCentriImpostazioniComponent {
  @ViewChild('grid') grid: GiasKendoGridComponent;

  constructor(
    private dialogService: GiasDialogService,
    private ACService: ImpostazioniAziendeCentriService,
  ) {  }

  private get selected(): any[] {
    return this.grid.rows.filter(r => r['Selected']);
  }

  onDelete() {
    if (this.selected.length === 0) {
      this.dialogService.baseError('SelezioneNulla', 'prof.SelezionaImpostazioniReset');
    } else {
      const content = this.selected.length > 1 ? 'prof.WarningResetDefaults' : 'prof.WarningResetDefault';
      this.dialogService.warningThen('Attenzione', content, true, () => {
        const toDelete = this.selected.map(i => new Imprese_Impostazioni(i.Piva, i.Impostazione_Cod, i.Impostazione_Valore, i.Sa_Cod));
        this.ACService.resetImpostazioni(toDelete)
          .subscribe(() => this.grid.publicService.refresh(true));
      });
    }
  }

}
