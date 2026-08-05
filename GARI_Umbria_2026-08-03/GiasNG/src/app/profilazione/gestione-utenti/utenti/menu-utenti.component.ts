import {Component, DoCheck} from '@angular/core';
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../../models/profilazione.model";
import {ProfilazioneUtentiService} from "../../services/profilazione-utenti.service";
import { TranslocoService } from '@jsverse/transloco';
import {ConfigurazioneSitiService} from "../../../Service/configurazione-siti.service";
import {ProfilazioneDataShareService} from "../../services/profilazione-data-share.service";
import {FormArray} from "@angular/forms";

const MAX_USERS_LOADED_DEFAULT = 200;

@Component({
  standalone: false,
  selector: 'app-menu-utenti',
  templateUrl: './menu-utenti.component.html',
  styleUrls: ['./menu-utenti.component.css']
})
export class MenuUtentiComponent implements DoCheck {
  public permessoLetturaProfili = false;
  protected isExpanded = false;
  protected tooManyUsers = false;
  protected noUsersLoaded = false;
  private maxUsersLoadedByDefault = MAX_USERS_LOADED_DEFAULT;

  constructor(
    private transloco: TranslocoService,
    private permessi: PermessiUtenteService,
    private userService: ProfilazioneUtentiService,
    private config: ConfigurazioneSitiService,
    private datashare: ProfilazioneDataShareService
  ) {
    this.setPermessi();
    this.config.leggiChiave('AgroProfilazione_MaxUtentiCaricatiDefault')
      .subscribe(chiave => {
        try {
          if (chiave) this.maxUsersLoadedByDefault = +chiave.Valore;
        } catch (ex) {
          this.maxUsersLoadedByDefault = MAX_USERS_LOADED_DEFAULT;
        }
    });
    this.checkUserFilters();
  }

  ngDoCheck(): void {
    if(this.userService.$utenti?.value) {
      this.noUsersLoaded = this.userService.$utenti?.value?.length === 0;
      this.tooManyUsers = this.userService.$utenti?.value?.length > this.maxUsersLoadedByDefault;
      this.isExpanded = this.isExpanded && !(this.tooManyUsers || this.noUsersLoaded);
    }
  }

  getTooltipShowOnProperty() {
    return this.tooManyUsers || this.noUsersLoaded ? 'hover' : 'none';
  }

  getTooltipText(): string {
    if (this.tooManyUsers)
      return this.transloco.translate('prof.ImpossibileCaricarePermessiTroppiUtenti');
    else if (this.noUsersLoaded)
      return this.transloco.translate('prof.ImpossibileCaricarePermessiNoUtentiCaricati');
    else return "";
  }

  private setPermessi() {
    this.permessoLetturaProfili = this.permessi.getPermesso(enum_Security_Attivita.Gest_UtentiProfili, enum_TipoPermesso.LETTURA);
  }

  private checkUserFilters() {
    const filters = this.datashare.loadUsersFilters;
    const currValue = filters?.value;
    if (currValue && currValue.ctrls) {
      const isFirstTime = currValue.ctrls.find(c => c.field === 'firstTime').value == 'true';
      const otherEmpty = currValue.ctrls.filter(c => c.field !== 'firstTime').every(c => c.value === '');
      if (!isFirstTime && otherEmpty) {
        this.datashare.filterService.resetFilters();
      }
    }
  }

}
