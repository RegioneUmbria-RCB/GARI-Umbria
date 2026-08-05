import {Component, OnInit, Output} from '@angular/core';
import {map, Subject, switchMap, tap} from 'rxjs';
import {FormControl, FormGroup} from "@angular/forms";
import {ZooFiltersHelperService} from "../../services/zoo-filters-helper.service";
import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";
import {GiasMessageService} from "../../../Service/gias-message.service";
import {ImpostazioniUtentiService} from "../../../profilazione/services/impostazioni/impostazioni-utenti.service";
import {Utente_Impostazioni} from "../../../Model/utente/utente_impostazioni";
import {enum_Impostazioni_Utenti} from "../../../Model/Impostazioni_Utenti.enum";

@Component({
  standalone: false,
  selector: 'zoo-favorites-editing',
  templateUrl: './zoo-favorites-editing.component.html',
  styleUrls: ['./zoo-favorites-editing.component.css'],
})
export class ZooFavoritesEditingComponent implements OnInit {
  @Output() done = new Subject<void>();

  protected canCustomize = false;
  protected filter = "";
  protected innerForm = new FormGroup({
    canCustomize: new FormControl(false)
  });
  private _operations: BaseCodeDescr[] = [];

  constructor(
    private message: GiasMessageService,
    private zooHelper: ZooFiltersHelperService,
    private settingsService: ImpostazioniUtentiService,
  ) {
  }

  public get operations() {
    return this._operations.filter((op: BaseCodeDescr) =>
      op.descrizione.toLowerCase().includes(this.filter.toLowerCase())
    );
  }

  public get favorites() {
    return this._operations.filter((op) => op['preferito']);
  }

  public get customMode(): boolean {
    return this.innerForm.get("canCustomize").value;
  }

  ngOnInit(): void {
    this.loadOperations();
  }

  public setFavorite(op) {
    if (this.customMode) {
      op.preferito = true;
      return;
    }
    this.message.errorMessage("CambiareTipoVisualizzazione", false, true);
  }

  public unsetFavorite(op) {
    if (this.customMode) {
      op.preferito = false;
      return;
    }
    this.message.errorMessage("CambiareTipoVisualizzazione", false, true);
  }

  /** Resets the interface to represent the user's current favorites. */
  public undoAction() {
    this.loadOperations();
  }

  /** Save the current favorites selection. */
  public execute() {
    let favs = "";
    if (this.favorites.length > 0) {
      favs = this.favorites.map(x => x.codice.toString()).reduce((acc, x) => acc + "|" + x);
    }
    this.settingsService.salvaImpostazioniUtenteObs(
      [new Utente_Impostazioni(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, favs)]
    ).subscribe((success) => {
      if (success) {
        this.message.successMessage("SalvataggioAvvenutoConSuccesso", false, true);
      } else {
        this.message.errorMessage("ErroreSalvataggio", false, true);
      }
    });
  }

  /**
   * Loads both all the available operations and the user's favorites ones.
   * @private
   */
  private loadOperations() {
    this.zooHelper.loadZooOperations()
      .pipe(
        map(operations => operations.map(o => new BaseCodeDescr(+o.codice, o.descrizione))),
        tap(operations => this._operations = operations),
        switchMap(() => this.zooHelper.loadFavorites())
      ).subscribe(favorites => {
        for (let f of favorites) {
          let found = this._operations.find(x => x.codice === +f.codice);
          found['preferito'] = true;
        }
    });
  }

}
