import { Component, EventEmitter, Input, OnChanges, Output } from "@angular/core";
import { tap } from "rxjs";
import { TipologiaLayer, Enum_Tipo_Operazione, ObjOptionHTML_Out } from "app/Service/api.service";
import { TranslocoService } from "@jsverse/transloco";
import { Dialog_Type, GiasDialogService } from "app/Service/gias-dialog.service";
import { ContestoColore, GisLayerColorPickerService } from "app/GIS/GIS-layer-color-picker-window/GIS-layer-color-picker-window.service";
import { TemaSelezionato, TipologiaLabelDto } from "../theme-window.component";

const DEFAULT_STARTING_COLOR = '#eeeeee';

export class ImpostazioniTema {
  discrete: boolean;
  colore1: string;
  colore2: string;
  varianza: string;
  tileLabels: TipologiaLabelDto[] | null;

  constructor() {
    this.colore1 = '';
    this.colore2 = '';
    this.varianza = '';
    this.discrete = false;
    this.tileLabels = null;
  }
}

@Component({
  standalone: false,
  selector: 'theme-window-settings',
  templateUrl: './theme-window-settings.component.html',
  styleUrls: ['./theme-window-settings.component.css']
})
export class ThemeWindowSettingsComponent implements OnChanges {

  @Input() layerSelezionato: TipologiaLayer;
  @Input() temaSelezionato: TemaSelezionato;
  @Input() tipoLayerSelected: ObjOptionHTML_Out;

  @Output() cancel = new EventEmitter<void>();
  @Output() save = new EventEmitter<ImpostazioniTema>();

  private overlappingDiscreteIndexes: [number, number];

  impostazioniTema: ImpostazioniTema;
  enum_Tipo_Operazione = Enum_Tipo_Operazione;

  constructor(
    private translocoService: TranslocoService,
    private giasDialogService: GiasDialogService,
    private gisLayerColorPickerService: GisLayerColorPickerService,
  ) { }

  ngOnChanges(): void {
    this.impostazioniTema = {
      colore1: this.temaSelezionato.colore1,
      colore2: this.temaSelezionato.colore2,
      varianza: this.temaSelezionato.varianza.toString(),
      discrete: this.temaSelezionato.tileLabels?.length > 0,
      tileLabels: JSON.parse(JSON.stringify(this.temaSelezionato.tileLabels))
    };

    if (this.impostazioniTema.tileLabels?.length == 0) {
      // Start with one label
      this.addTileLabel();
    }
  }

  addTileLabel(): void {
    this.impostazioniTema.tileLabels ??= [];
    const currentMin = Math.max(...this.impostazioniTema.tileLabels.map(x => x.valore_max), 0);
    const currentMax = this.temaSelezionato.valoreMax ?? Math.max(...this.impostazioniTema.tileLabels.map(x => x.valore_max), 0);
    this.impostazioniTema.tileLabels.push({
      colore: DEFAULT_STARTING_COLOR,
      valore_min: currentMin,
      valore_max: currentMax == 0 ? currentMin + 1 : currentMax,
      label: '',
      valore_associato: 0,
      LayerElementiGrafici_Cod: +this.layerSelezionato.id,
      LayerTiles_Cod: this.temaSelezionato.id,
      LayerTilesDescrizione_Cod: 0,
      TipologiaLayer_cod: +this.tipoLayerSelected?.Option_Value,
      operation: Enum_Tipo_Operazione.INSERT,
      visible: true,
      singleValue: false
    });
  }

  onClickSettingCross(): void {
    this.cancel.emit();
  }

  onClickSettingCheck(event: Event): void {
    if (!this.impostazioniTema.discrete) {
      if (this.isVarianzaCorrect()) {
        this.save.emit(this.impostazioniTema);
      }
      return;
    }

    this.computeOverlappings();
    if (this.overlappingDiscreteIndexes != null) {
      event.preventDefault();
      return;
    }

    this.resetDiscreteValues(this.impostazioniTema);
    this.save.emit(this.impostazioniTema);
  }

  onClickTogglePicker(event: Event, indiceColore: number, colore: string): void {
    event.stopPropagation();

    const coloreSenzaCancelletto = colore.substring(1);
    this.gisLayerColorPickerService
      .open(coloreSenzaCancelletto, ContestoColore.Tema)
      .pipe(
        tap(coloreModificato => {
          if (coloreModificato == null || coloreModificato == '') {
            return;
          }
          this.modificaColore(indiceColore, coloreModificato);
        })
      )
      .subscribe();
  }

  onClickTogglePickerDiscrete(event: Event, tileLabel: TipologiaLabelDto): void {
    event.stopPropagation();

    const coloreSenzaCancelletto = tileLabel.colore.substring(1);
    this.gisLayerColorPickerService
      .open(coloreSenzaCancelletto, ContestoColore.Tema)
      .pipe(
        tap(coloreModificato => {
          if (coloreModificato == null || coloreModificato == '') {
            return;
          }
          tileLabel.colore = coloreModificato;
        })
      )
      .subscribe();
  }

  onClickSettingDelete(impostazioniTema: ImpostazioniTema, i: number): void {
    const tileLabel = impostazioniTema.tileLabels[i];
    if (tileLabel == null) {
      this.giasDialogService.baseError("", "gis.TemaLabelsNull");
      return;
    }

    if (tileLabel.operation != Enum_Tipo_Operazione.INSERT) {
      tileLabel.operation = Enum_Tipo_Operazione.DELETE;
      return;
    }

    impostazioniTema.tileLabels.splice(i, 1);
  }

  isOverlapping(index: number): boolean {
    return this.overlappingDiscreteIndexes != null && (this.overlappingDiscreteIndexes[0] == index || this.overlappingDiscreteIndexes[1] == index);
  }

  isMinGreaterThanMax(tileLabel: TipologiaLabelDto): boolean {
    return tileLabel.valore_min > tileLabel.valore_max || tileLabel.valore_min < 0;
  }

  computeOverlappings(): void {
    const tileLabels = (JSON.parse(JSON.stringify(this.impostazioniTema.tileLabels)) as TipologiaLabelDto[] | null)?.filter(x => x.operation != Enum_Tipo_Operazione.DELETE);
    if (tileLabels == null) {
      this.overlappingDiscreteIndexes = null;
      return;
    }

    for (let i = 0; i < tileLabels.length; i++) {
      for (let j = 0; j < tileLabels.length; j++) {
        // Avoid check on the same element
        if (i == j) {
          continue;
        }

        // Check overlapping min value
        if (tileLabels[i].valore_min > tileLabels[j].valore_min && tileLabels[i].valore_min < tileLabels[j].valore_max) {
          this.overlappingDiscreteIndexes = [i, j];
          return;
        }

        // Check overlapping max value
        if (tileLabels[i].valore_max > tileLabels[j].valore_min && tileLabels[i].valore_max < tileLabels[j].valore_max) {
          this.overlappingDiscreteIndexes = [i, j];
          return;
        }
      }
    }

    this.overlappingDiscreteIndexes = null;
  }

  changeSingleValue(tileLabel: TipologiaLabelDto): void {
    tileLabel.singleValue = !tileLabel.singleValue;
  }

  private resetDiscreteValues(impostazioniTema: ImpostazioniTema): void {
    for (const tileLabel of impostazioniTema.tileLabels) {
      if (tileLabel.singleValue) {
        tileLabel.valore_min = 0;
        tileLabel.valore_max = 0;
      } else {
        tileLabel.valore_associato = 0;
      }
    }
  }

  private modificaColore(indiceColore: number, coloreModificato: string): void {
    if (indiceColore === 1) {
      this.impostazioniTema.colore1 = coloreModificato;

    }
    if (indiceColore === 2) {
      this.impostazioniTema.colore2 = coloreModificato;
    }
  }

  private isVarianzaCorrect(): boolean {
    let varianzaNumerico = Number(this.impostazioniTema.varianza);
    if (this.impostazioniTema.varianza.trim() === '' || isNaN(varianzaNumerico)) {
      this.giasDialogService.alertMessage(this.translocoService.translate('gis.VarianzaNonNumerica'), null, Dialog_Type.error);
      return false;
    }

    if (varianzaNumerico < 2) {
      this.giasDialogService.alertMessage(this.translocoService.translate('gis.VarianzaMinoreDue'), null, Dialog_Type.error);
      return false;
    }

    if (varianzaNumerico !== Math.trunc(varianzaNumerico)) {
      this.giasDialogService.alertMessage(this.translocoService.translate('gis.VarianzaConDecimali'), null, Dialog_Type.error);
      return false;
    }

    return true;
  }
}
