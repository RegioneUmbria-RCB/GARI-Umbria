import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GISAttributiMuzService } from '../GIS-attributi-muz.service';
import { map } from 'rxjs';
import { Plot } from '../GIS-attributi-muz-grid.component';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { AnagraficaNGClient, DatiMUZVisibili_Out, Enum_Operazioni_MUZ, GisClient, MUZ } from 'app/Service/api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { MasterService } from 'app/Service/master.service';

@Component({
  standalone: false,
  selector: 'gis-edit-muz',
  templateUrl: './GIS-edit-muz.component.html',
  styleUrls: ['./GIS-edit-muz.component.css']
})
export class GISEditMuzComponent {
  @Input() inputMuz: MUZ;
  @Input() inputPlot: Plot | null;
  @Output() onMuzEdit = new EventEmitter<DatiMUZVisibili_Out>();

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  plot: string | null = null;
  piva = this.objParametriAgendaService.getObjParamValue().Piva;

  groups$ = this.gisAttributiMuzService.groups$;
  analysis$ = this.anagraficaNGClient
    .anagraficaNGLeggiAnalisiTestataPerPiva(this.piva)
    .pipe(map(res => (JSON.parse(res.RispostaStringa) as any[]).map(x => ({ ...x, analisi_testata_cod: +x.analisi_testata_cod } as MuzAnalysis))));

  constructor(
    private gisAttributiMuzService: GISAttributiMuzService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private masterService: MasterService,
    private anagraficaNGClient: AnagraficaNGClient
  ) { }

  selectAnalysis(analysis: MuzAnalysis): void {
    if (this.inputMuz.Analisi_Testate[0] != null && this.inputMuz.Analisi_Testate_Aggiungi[0] == null) {
      this.inputMuz.Analisi_Testate_Elimina = [...this.inputMuz.Analisi_Testate.map(x => x.Analisi_Testata_Cod)];
    }

    this.inputMuz.Analisi_Testate[0] = { Analisi_Testata_Cod: analysis.analisi_testata_cod };
    this.inputMuz.Analisi_Testate_Aggiungi[0] = analysis.analisi_testata_cod;
  }

  submit(): void {
    this.inputMuz.Operazione_Cod = this.inputMuz.Area_Cod == 0 ? Enum_Operazioni_MUZ.INSERT : Enum_Operazioni_MUZ.UPDATE;

    this.masterService.set_isLoading({ isLoading: true });
    this.gisClient
      .gisOperazioniMUZ(this.inputMuz)
      .subscribe({
        next: () => {
          this.masterService.set_isLoading({ isLoading: false });
          this.giasDialogService.baseSuccess('', 'gis.DatiMuzAggiornatiCorrettamente', true);
          this.onMuzEdit.emit(this.inputMuz);
        },
        error: () => {
          this.masterService.set_isLoading({ isLoading: false });
          this.giasDialogService.baseError('', 'gis.ErroreNellAggiornamentoDeiDatiMuz', true);
          this.onMuzEdit.emit(this.inputMuz);
        },
      });
  }
}

interface MuzAnalysis {
  analisi_testata_cod: number;
  nome: string;
}
