import { Component } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { DatiGruppoUtente, GisClient, PermessiXGruppiUtente, ProvisioningClient, SalvaPermessiLayerGruppiUtente_In } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { debounceTime, finalize, map, Observable, of, Subject, Subscription, switchMap, tap } from 'rxjs';
import { GISLayerPermissionsWindowService } from '../GIS-layer-permissions-window.service';

@Component({
  standalone: false,
  selector: 'gis-layer-groups-permissions',
  templateUrl: './GIS-layer-groups-permissions.component.html',
  styleUrls: ['./GIS-layer-groups-permissions.component.css'],
})
export class GISLayerGroupsPermissionsComponent {
  data: PermessiXGruppiUtenteResult[] = [];
  groups: DatiGruppoUtente[] = [];
  faDelete = faTrashAlt;
  pageSize = 5;
  isModalOpen = false;
  loading = false;
  newGroup: DatiGruppoUtente | null = null;
  filterSettings: DropDownFilterSettings = { caseSensitive: false, operator: "contains" };

  private allGroups: DatiGruppoUtente[] = [];
  private save$ = new Subject<void>();
  private subscriptions: Subscription[] = [];

  constructor(
    private gisClient: GisClient,
    private provisioningClient: ProvisioningClient,
    private gisLayerPermissionsWindowService: GISLayerPermissionsWindowService,
    private giasMessageService: GiasMessageService
  ) {
    this.subscriptions.push(
      this.gisLayerPermissionsWindowService
        .layerSelected$
        .pipe(
          switchMap(() => this.loadData()),
          tap(data => this.data = data),
          tap(() => this.computeTotAuths()),
          tap(() => this.updateGroups()))
        .subscribe()
    );

    this.subscriptions.push(
      this.save$
        .pipe(debounceTime(500))
        .subscribe(() => this.doSave())
    )

    this.provisioningClient
      .provisioningListaGruppiUtente()
      .pipe(tap(data => this.allGroups = data.RispostaStringa?.ListaDatiGruppoUtente ?? []))
      .subscribe(() => this.updateGroups());
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }
  }

  addPermesso(): void {
    if (this.newGroup == null) {
      return;
    }

    this.data = [...this.data, PermessiXGruppiUtenteResult.fromOriginal({
      Gruppo: +this.newGroup.Codice,
      GruppoDescr: this.newGroup.Descrizione,
      Flag_Amministrazione: 1,
      Flag_Cancellazione: 1,
      Flag_Informazioni: 1,
      Flag_Inserimento: 1,
      Flag_Modifica: 1,
      Flag_Rimozione: 0
    })];

    this.updateGroups();
    this.newGroup = null;
    this.isModalOpen = false;
    this.gisLayerPermissionsWindowService.addRemainingAuths(1);
    this.save$.next();
  }

  updatePermesso(): void {
    this.save$.next();
  }

  updatePermessoAmministra($event: Event): void {
    const remainingAuths = this.gisLayerPermissionsWindowService.currentRemainingAuths;

    // Permesso amministra aggiunto
    if (($event.currentTarget as HTMLInputElement).checked) {
      this.gisLayerPermissionsWindowService.addRemainingAuths(1);
      this.updatePermesso();
      return;
    }

    // Permesso amministra rimosso, ma non ho altre auth
    if (remainingAuths <= 1) {
      $event.preventDefault();
      this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
      return;
    }

    // Permesso amministra rimosso correttamente
    this.gisLayerPermissionsWindowService.addRemainingAuths(-1);
    this.updatePermesso();
  }

  deletePermesso(permesso: PermessiXGruppiUtenteResult): void {
    const remainingAuths = this.gisLayerPermissionsWindowService.currentRemainingAuths;
    if (permesso.Flag_Amministrazione && remainingAuths <= 1) {
      this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
      return;
    }

    this.data = this.data.filter(x => x != permesso);
    this.updateGroups();
    this.gisLayerPermissionsWindowService.addRemainingAuths(-1);
    this.save$.next();
  }

  private loadData(): Observable<PermessiXGruppiUtenteResult[]> {
    const layer = this.gisLayerPermissionsWindowService.currentLayerSelected;
    if (layer == null) {
      return of([]);
    }

    this.loading = true;

    return this.gisClient
      .gisLeggiElencoPermessiLayerGruppiUtente({ Layer_Cod: +layer.id })
      .pipe(
        map(data => data.RispostaStringa.gruppiutente_permessi.map(x => PermessiXGruppiUtenteResult.fromOriginal(x))),
        tap(() => this.updateGroups()),
        finalize(() => this.loading = false)
      );
  }

  private computeTotAuths(): void {
    const currentAuths = this.data.filter(x => x.Flag_Amministrazione).length;
    this.gisLayerPermissionsWindowService.addRemainingAuths(currentAuths);
  }

  private updateGroups(): void {
    this.groups = this.allGroups.filter(group => this.data.find(permesso => permesso.Gruppo == +group.Codice) == null);
  }

  private doSave(): void {
    const payload = {
      Layer_Cod: +this.gisLayerPermissionsWindowService.currentLayerSelected.id,
      gruppiutente_permessi: this.data.map(x => x.toOriginal())
    } as SalvaPermessiLayerGruppiUtente_In;

    this.gisClient
      .gisSalvaPermessiLayerGruppiUtente(payload)
      .subscribe(() => this.giasMessageService.successMessage('gis.PermessiSalvaOk', false, true));
  }
}

class PermessiXGruppiUtenteResult {
  constructor(
    public Gruppo: number,
    public GruppoDescr: string,
    public Flag_Inserimento: boolean,
    public Flag_Modifica: boolean,
    public Flag_Cancellazione: boolean,
    public Flag_Informazioni: boolean,
    public Flag_Amministrazione: boolean,
    public Flag_Rimozione: boolean
  ) { }

  toOriginal(): PermessiXGruppiUtente {
    return {
      Gruppo: this.Gruppo,
      GruppoDescr: this.GruppoDescr,
      Flag_Inserimento: +this.Flag_Inserimento,
      Flag_Modifica: +this.Flag_Modifica,
      Flag_Cancellazione: +this.Flag_Cancellazione,
      Flag_Informazioni: +this.Flag_Informazioni,
      Flag_Amministrazione: +this.Flag_Amministrazione,
      Flag_Rimozione: +this.Flag_Rimozione
    } as PermessiXGruppiUtente;
  }

  static fromOriginal(permesso: PermessiXGruppiUtente): PermessiXGruppiUtenteResult {
    return new PermessiXGruppiUtenteResult(
      permesso.Gruppo,
      permesso.GruppoDescr,
      permesso.Flag_Inserimento == 1,
      permesso.Flag_Modifica == 1,
      permesso.Flag_Cancellazione == 1,
      permesso.Flag_Informazioni == 1,
      permesso.Flag_Amministrazione == 1,
      permesso.Flag_Rimozione == 1
    );
  }

}
