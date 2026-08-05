import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { Observable, catchError, map, of, switchMap, tap } from 'rxjs';
import { GISAttributiMuzService } from '../GIS-attributi-muz.service';
import { Enum_Tipo_OperazioneGruppi, GisClient, GruppoAreaOmogenea, OperazioniGruppiMUZ_In, RispostaStandard, RispostaStandard_1OfList_1OfGruppoAreaOmogenea } from 'app/Service/api.service';
import { Plot } from '../GIS-attributi-muz-grid.component';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { MasterService } from 'app/Service/master.service';

@Component({
  standalone: false,
  selector: 'gis-init-muz-group',
  templateUrl: './GIS-init-muz-group.component.html',
  styleUrls: ['./GIS-init-muz-group.component.css']
})
export class GISInitMuzGroupComponent {
  @Input() inputPlot: Plot | null;

  @Output() onNewGroup = new EventEmitter<GruppoAreaOmogenea>();

  private defautItem = { Gruppo_Area_Cod: -1, Gruppo_Area_Des: 'Nessuno' } as GruppoAreaOmogenea;

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  selectedGroup = this.defautItem.Gruppo_Area_Cod;
  newGroupName: string = '';
  groups$ = this.gisAttributiMuzService.groups$
    .pipe(map(groups => [this.defautItem, ...groups]));

  constructor(
    private gisAttributiMuzService: GISAttributiMuzService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private masterService: MasterService
  ) { }

  submit(newGroupName: string, selectedGroup: GruppoAreaOmogenea | null): void {
    const payload = {
      Piva: this.inputPlot.Piva,
      Sa_Cod: this.inputPlot.Sa_Cod,
      Appezza: this.inputPlot.Appezza,
    } as OperazioniGruppiMUZ_In;

    if (selectedGroup != null && newGroupName == null) {
      payload.operazione = Enum_Tipo_OperazioneGruppi.COPIAAPPEZZAMENTO;
      payload.Gruppo_Area_Cod_Esistente = selectedGroup.Gruppo_Area_Cod;
    } else if (selectedGroup != null && newGroupName != null) {
      payload.operazione = Enum_Tipo_OperazioneGruppi.CLONAGRUPPO;
      payload.Gruppo_Area_Cod_Esistente = selectedGroup.Gruppo_Area_Cod;
      payload.New_Gruppo_Area_Des = newGroupName;
    } else if (selectedGroup == null && newGroupName != null) {
      payload.operazione = Enum_Tipo_OperazioneGruppi.CREAGRUPPOVUOTO;
      payload.New_Gruppo_Area_Des = newGroupName;
    }

    this.masterService.set_isLoading({ isLoading: true });
    this.gisClient
      .gisOperazioniGruppiMUZ(payload)
      .pipe(
        tap(() => this.giasDialogService.baseSuccess('', 'gis.GruppiMuzAggiornatiCorrettamente', true)),
        catchError(() => {
          this.giasDialogService.baseError('', 'gis.ErroreNellAggiornamentoDeiGruppiMuz', true);
          return of(null);
        }),
        switchMap(() => this.gisClient.gisLeggiListaGruppiMUZ(this.inputPlot.Piva)),
        catchError(() => {
          this.giasDialogService.baseError('', 'gis.ErroreNellAggiornamentoDeiGruppiMuz', true);
          return of({ RispostaStringa: [] } as RispostaStandard_1OfList_1OfGruppoAreaOmogenea);
        }),
        map(res => res.RispostaStringa)
      )
      .subscribe(groups => {
        this.gisAttributiMuzService.nextGroups(groups);
        this.masterService.set_isLoading({ isLoading: false });

        this.inputPlot.GroupCod = selectedGroup?.Gruppo_Area_Cod ?? groups.find(g => g.Gruppo_Area_Des == newGroupName)?.Gruppo_Area_Cod;
        this.inputPlot.Group = newGroupName ?? groups.find(g => g.Gruppo_Area_Cod == selectedGroup?.Gruppo_Area_Cod)?.Gruppo_Area_Des;
        this.gisAttributiMuzService.nextSelectedPlot(this.inputPlot);

        this.onNewGroup.emit();
      });
  }
}
