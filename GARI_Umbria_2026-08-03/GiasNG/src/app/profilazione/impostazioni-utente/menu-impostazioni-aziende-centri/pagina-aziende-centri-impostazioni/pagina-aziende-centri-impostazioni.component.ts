import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ImpostazioniAziendeCentriService } from '../../../services/impostazioni/impostazioni-aziende-centri.service';
import { ProfilazioneDataShareService } from '../../../services/profilazione-data-share.service';

@Component({
  standalone: false,
  selector: 'app-pagina-aziende-centri-impostazioni',
  templateUrl: './pagina-aziende-centri-impostazioni.component.html',
  styleUrls: ['./pagina-aziende-centri-impostazioni.component.css']
})
export class PaginaAziendeCentriImpostazioniComponent {

    constructor(
        private ACSettings: ImpostazioniAziendeCentriService,
        private dataShare: ProfilazioneDataShareService,
        private changeDetector: ChangeDetectorRef
    ) {
      this.dataShare.setChangeDetector(changeDetector);
    }

    public get editing(): boolean {
        return !!this.dataShare.cardInfo.value;
    }
}
