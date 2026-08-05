import { Component } from '@angular/core';
import { faAddressCard, faLeaf } from '@fortawesome/free-solid-svg-icons';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { ValutazioniService } from '../../service/valutazioni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-valutazione-edit-dettaglio',
  templateUrl: './valutazione-edit-dettaglio.component.html',
  styleUrls: ['./valutazione-edit-dettaglio.component.css']
})
export class ValutazioneEditDettaglioComponent {

  faLeaf = faLeaf;
  faProfili = faAddressCard;
  permessoEdit: boolean;

  page: number;
  constructor(private permessiUtenteService: PermessiUtenteService,
    private valutazioniService: ValutazioniService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private objParametriService: ObjParametriAgendaService,

  ) {

    let objParametriAgenda = this.objParametriService.getObjParamValue();


    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2) && (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update)

    this.page = 1;
  }

  setPage(num: number): void {
    this.page = num;
  }

  aggiornatiDatiDettaglio(): any {
    return new Promise((resolve, reject) => {
      this.giasDialogService.baseWarning(
          '', this.translocoService.translate('AggiornaDatiDettaglioDomanda'),false
      ).then((resp: DialogResult) => {
          if(!resp['returnObj']) return;
          this.aggiornaDati();
      });
  });

  }

  aggiornaDati() : any{
    let testata_lettura = this.valutazioniService.getLeggiTestataValue();

    if(testata_lettura != null){
      this.valutazioniService.aggiornaDatiDettaglio(testata_lettura).subscribe(x => {
        if(x.RispostaOK){
          console.log("DatiAggiornati");
          let tmp : any = new Object(this.page);
          this.page = 0;
          setTimeout(()=>{this.page = +tmp}, 1000);

        }
      });
    }
  }

}
