import {Component, OnDestroy} from '@angular/core';
import {GiasDropDownTemplateService} from 'gias-ui-kit';
import {GruppiUtentiService} from "../services/gruppi-utenti.service";
import {Subject, take, takeUntil} from "rxjs";
import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {GruppoUtente} from '../models/gruppi-utenti/GruppoUtente.model';
import {FormGroup} from '@angular/forms';
import {PermessiUtenteService} from "../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../models/profilazione.model";

@Component({
  standalone: false,
  selector: 'app-gruppi-utenti',
  templateUrl: './gruppi-utenti.component.html',
  styleUrls: ['./gruppi-utenti.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class GruppiUtentiComponent implements OnDestroy {

  public transitionForm = new FormGroup({
  });
  protected isEditing = false;
  protected canEditUsersGroups: boolean;
  protected serviceList: Array<BaseCodeDescr> = [];
  private signal =  new Subject<boolean>();

  constructor(
    private gruppiService: GruppiUtentiService,
    private permessi: PermessiUtenteService,
  ) {
    this.canEditUsersGroups = this.permessi.getPermesso(enum_Security_Attivita.Gest_UtentiGruppi, enum_TipoPermesso.LETTURA_SCRITTURA);
    this.gruppiService.caricaServiziTransazioniStatoXGruppi()
      .pipe(take(1))
      .subscribe(data => this.serviceList = data);
    if (this.canEditUsersGroups) this.handleSelection();
  }

  private get gruppo(): GruppoUtente {
    return this.gruppiService.gruppo$.value;
  }

  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  // TODO
  public aggiungiTransizione() {
  }

  private handleSelection() {
    this.gruppiService.gruppo$.pipe(takeUntil(this.signal)).subscribe( (gr: GruppoUtente) => {
      if (gr) {
        this.isEditing = true;
        this.gruppiService.gridRefresh$.next(true);
      }
    });
  }

}
