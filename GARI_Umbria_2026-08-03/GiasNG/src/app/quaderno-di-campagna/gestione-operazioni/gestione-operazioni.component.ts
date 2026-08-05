import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {LeggiOperazioni, OperazioniService, SalvaOperazioniPreferite} from 'app/Service/Metaschema/operazioni.service';
import {QdCTestataService} from '../agenda-edit/service/testata/testata.service';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../Service/master.service";
import {QdCService} from "../agenda-edit/service/qdc.service";
import {GiasMessageService} from "../../Service/gias-message.service";

@Component({
  standalone: false,
  selector: 'app-gestione-operazioni',
  templateUrl: './gestione-operazioni.component.html',
  styleUrls: ['./gestione-operazioni.component.css']
})
export class GestioneOperazioniComponent implements OnInit {

  public form = new FormGroup({
    canCustomize: new FormControl(true)
  });

  filter = "";

  private Operazioni_: Lavorazione[] = [];

  constructor(
              private operazioniservice: OperazioniService,
              private translocoService: TranslocoService,
              public testataservice: QdCTestataService,
              private qdcservice: QdCService,
              private giasmessaggeservice: GiasMessageService
  ) { }

  public get Operazioni(): Lavorazione[] {
    return this.Operazioni_.filter((op: Lavorazione) =>
      op.descrizione.toLowerCase().includes(this.filter.toLowerCase())
    );
  }

  public get Preferite(): Lavorazione[] {
    return this.Operazioni_.filter((op: Lavorazione) => op['preferito']);
  }

  public get customMode(): boolean {
    return this.form.get("canCustomize").value;
  }

  ngOnInit() {
    this.readCurrentSettings();

  }

  setFavorite(op) {
    if (this.customMode) {
      op.preferito = true;
      return;
    }

    let listerroriGias:ErroreGias[] = [];

    listerroriGias.push(<ErroreGias>{
          severity: ErroreGias_Severity.Bloccante,
          tipo:  enum_ErroreGias_Tipo.Generico,
          messaggio: this.translocoService.translate("CambiareTipoVisualizzazione")
    });

    this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();

  }

  unsetFavorite(op) {
    if (this.customMode) {
      op.preferito = false;
      return;
    }

    let listerroriGias:ErroreGias[] = [];

    listerroriGias.push(<ErroreGias>{
          severity: ErroreGias_Severity.Bloccante,
          tipo:  enum_ErroreGias_Tipo.Generico,
          messaggio: this.translocoService.translate("CambiareTipoVisualizzazione")
    });

    this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();

  }

  undoAction() {
    this.readCurrentSettings();
  }

  execute() {
    const params: SalvaOperazioniPreferite = {
        lista_Lav_Cod: this.Preferite.map(fav => +fav.primaryKey.codice),
        full_update: true
    };

    this.operazioniservice.SalvaOperazioniPreferite(params).then(r=>{
        if(r.RispostaOK)
            this.giasmessaggeservice.successMessage(this.translocoService.translate('qdc.Salvataggio_Operazioni_Preferite_Completato'),false);
    });
  }

  private async readCurrentSettings() {

    const params: LeggiOperazioni = {
      gruppiOperazioni: [],
      lista_Lav_Cod: [],
      FiltraImpostazioniUtente: true,
      Visualizza_Solo_Operazioni_Preferite: false,
      tipo_Attivita: this.qdcservice.TestataForm.get("Tipo").value,
      tipo_Ricetta: this.qdcservice.TestataForm.get("TipoRicetta").value,
      stato: this.qdcservice.TestataForm.get("Stato").value
    };
    this.Operazioni_ = await this.testataservice.getArray_Operazioni(params,false);


    const favParams: LeggiOperazioni = {
      gruppiOperazioni: [],
      lista_Lav_Cod: [],
      FiltraImpostazioniUtente: true,
      Visualizza_Solo_Operazioni_Preferite: true,
      tipo_Attivita: this.qdcservice.TestataForm.get("Tipo").value,
      tipo_Ricetta: this.qdcservice.TestataForm.get("TipoRicetta").value,
      stato: this.qdcservice.TestataForm.get("Stato").value
    };
    this.testataservice.getArray_Operazioni(favParams,false)
        .then((favorites: Lavorazione[]) => favorites
            .map(fav => fav.primaryKey.codice)
            .forEach(lavCod => {
                let op = this.Operazioni_.find(o => o.primaryKey.codice === lavCod);

                if (!!op) {
                  op['preferito'] = true;
                }
            })
        );
  }

}
