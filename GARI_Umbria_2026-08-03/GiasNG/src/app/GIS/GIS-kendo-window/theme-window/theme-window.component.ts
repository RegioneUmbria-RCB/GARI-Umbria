import { Component, Input, OnDestroy } from "@angular/core";
import { KendoWindowsService, WindowArgs, WindowTypes } from "app/Service";
import { LayerService } from "app/GIS/services/layer.service";
import { distinctUntilChanged, filter, forkJoin, map, Observable, of, pairwise, startWith, Subject, switchMap, takeUntil, tap, withLatestFrom } from "rxjs";
import { SalvaColoriLayer2_In, DatiTema, TipologiaLayer, TipologiaTile, GisClient, TipologiaLabel, LayerTilesDescrizione_In, Enum_Tipo_Operazione, ObjOptionHTML_Out, LeggiLayerTilesDescrizione_In, AttributoLayer } from "app/Service/api.service";
import { GisToolbarService } from "app/GIS/GIS-toolbar/gis-toolbar.service";
import { TranslocoService } from "@jsverse/transloco";
import { GoogleMapGeoJsonService } from "app/GIS/google-map/google-map-geojson.service";
import { SharedDataService } from "app/GIS/services/shared-data.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { GiasMessageService } from "app/Service/gias-message.service";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { GoogleMapService } from "app/GIS/google-map/google-map.service";
import { ImpostazioniTema } from "./theme-window-settings/theme-window-settings.component";
import { FeatureInformationService } from "app/GIS/services/feature-information.service";
import { GISModality } from "app/GIS/GIS-enum/GIS-feature";
import { hex2rgb } from "gias-ui-kit";
import { MasterService } from "app/Service/master.service";

export interface DatiColoreTema {
  valoreMin?: number;
  valoreMax?: number;
  colore?: { r: number, g: number, b: number };
  visible: boolean;
}

export interface TemaSelezionato {
  layerId: string;
  id: number;
  nome: string;
  colore1: string;
  colore2: string;
  varianza: number;
  valoreMax: number;
  applicaTema: boolean;
  tileLabels: TipologiaLabelDto[] | null
}

export interface LimitiValoriTema {
  valoreMin: number;
  valoreMax: number;
}

export interface TipologiaLabelDto extends TipologiaLabel {
  operation: Enum_Tipo_Operazione;
  visible: boolean;
  singleValue: boolean;
}

interface TipologiaTileTradotto extends TipologiaTile {
  nomeTradotto: string;
}

@Component({
  standalone: false,
  selector: 'theme-window',
  templateUrl: './theme-window.component.html',
  styleUrls: ['./theme-window.component.css']
})
export class ThemeWindowComponent implements OnDestroy {
  @Input() modality: GISModality | undefined = GISModality.Full;

  windowArgs$: Observable<WindowArgs>;

  tipoLayerSelected$: Observable<ObjOptionHTML_Out>;
  layerSelected$: Observable<TipologiaLayer>;
  tiles$: Observable<TipologiaTileTradotto[]>;

  enum_Tipo_Operazione = Enum_Tipo_Operazione;
  // tiles: TipologiaTileTradotto[] = [];
  temaSelezionato: TemaSelezionato = {} as TemaSelezionato;
  scalaColori: DatiColoreTema[] = [];
  visualizzaImpostazioniTema: boolean = false;

  private signal$: Subject<void> = new Subject();
  private loadTiles = new Subject<boolean>();

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private layerService: LayerService,
    private gisToolbarService: GisToolbarService,
    private translocoService: TranslocoService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private sharedDataService: SharedDataService,
    private giasDialogService: GiasDialogService,
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private funzioniComuniService: FunzioniComuniService,
    private masterService: MasterService,
    private featureInformationService: FeatureInformationService
  ) {
    this.windowArgs$ = this.kendoWindowsService.windowToggle$
      .pipe(
        filter(([windowTypes, _]) => windowTypes === WindowTypes.ThemeWindow),
        map(([_, args]) => args)
      );

    this.tipoLayerSelected$ = this.layerService.LayerSelected.asObservable();

    const layerSelected$ = this.layerService.layerItemSelected$
      .pipe(
        map(layer => layer[1] ? layer[0] : null),
        distinctUntilChanged((prev, curr) => prev?.id == curr?.id)
      );

    this.layerSelected$ = layerSelected$
      .pipe(
        pairwise(),
        withLatestFrom(this.windowArgs$.pipe(startWith({ openState: false }))),
        map(([[oldLayer, newLayer], args]) => {
          if (this.temaSelezionato?.id != null) {
            this.temaSelezionato = {} as TemaSelezionato;
            this.ripristinaColoreLayer(oldLayer);
          }

          if (args?.openState) {
            this.chiudiFinestra(false);
          }

          return newLayer;
        })
      );

    this.tiles$ = this.loadTiles.asObservable()
      .pipe(
        withLatestFrom(layerSelected$),
        switchMap(([_, layer]) => this.gisClient.gisLeggiImpostazioniAvanzateLayer(+layer.id).pipe(map(res => ({layer: layer, attributi: res.RispostaStringa.ListaAttributiLayer})))),
        map(data => this.getValidTiles(data.layer, data.attributi))
      );

    this.windowArgs$
      .pipe(
        withLatestFrom(layerSelected$),
        takeUntil(this.signal$)
      )
      .subscribe(([args, layer]) => {
        this.inizializzaTemaSelezionato(layer);
        this.visualizzaImpostazioniTema = false;

        // Chiusura finestra
        if (!args.openState) {
          this.ripristinaColoreLayer(layer);
          return;
        }

        this.loadTiles.next(true);
        const tiles = this.getValidTiles(layer, []);
        if (tiles.length == 0) {
          this.chiudiFinestra(true);
          return;
        }

        this.kendoWindowsService.setTitle(WindowTypes.ThemeWindow, layer.nome);

        if (this.temaSelezionato == null) {
          this.inizializzaAperturaFinestra(layer);
        }

        if (tiles.length > 0) {
          // Cambio layer selezionato
          if (this.temaSelezionato != null) {
            this.ripristinaColoreLayer(layer);
          }

          this.inizializzaAperturaFinestra(layer);
        }
      });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  onChangeTheme(tileSelezionato: TipologiaTileTradotto, layer: TipologiaLayer): void {
    const nuovoTileSelezionato = layer.tiles.find(tile => tile.id === tileSelezionato.id);
    this.impostaTemaSelezionato(nuovoTileSelezionato, layer);
    if (nuovoTileSelezionato) {
      this.applicaTematizzazioneLayer(layer);
    } else {
      this.ripristinaColoreLayer(layer);
    }
  }

  onClickThemeSettings(): void {
    this.visualizzaImpostazioniTema = !this.visualizzaImpostazioniTema;
  }

  chiudiFinestra(warning: boolean): void {
    this.gisToolbarService.themeBtnToggle(false);

    if (warning) {
      this.giasMessageService.infoMessagge(this.translocoService.translate('gis.NessunDatoTrovatoTematizzazione'));
    }
  }

  getValidTiles(layer: TipologiaLayer, attributi: AttributoLayer[]): TipologiaTileTradotto[] {
    const tiles = [];
    if (layer == null) {
      return [];
    }

    for (const tile of layer.tiles) {
      const features = this.featureInformationService.getByLayer(layer.id);
      const limitiTema = this.sharedDataService.getLimitiTematizzazioneLayer(features, layer, tile.nome);
      if (limitiTema.valoreMax !== 0) {
        const translation = attributi
          ?.find(x => x.NomeAttributo == tile.nome)
          ?.Traduzioni
          ?.find(x => +x.Lingua_Cod == this.masterService.getCurrentLingua())
          ?.Traduzione;

        tiles.push({ ...tile, nomeTradotto: translation == null || translation.trim() == '' ? tile.nome : translation } as TipologiaTileTradotto);
      }
    }

    return tiles;
  }

  isThemeDiscrete(): boolean {
    return this.temaSelezionato?.tileLabels?.length > 0;
  }

  isFullModality(): boolean {
    return this.modality == GISModality.Full;
  }

  toggleLabel(tileLabel: TipologiaLabelDto, layer: TipologiaLayer): void {
    tileLabel.visible = !tileLabel.visible;

    this.caricaScalaColoriDiscreta(this.temaSelezionato.tileLabels);
    this.googleMapGeoJsonService.applicaTematizzazioneLayer(layer, this.temaSelezionato, this.scalaColori);
  }

  onSettingsClose(): void {
    this.visualizzaImpostazioniTema = false;
  }

  onSettingsSave(settings: ImpostazioniTema, layer: TipologiaLayer, tipoLayerSelected: ObjOptionHTML_Out): void {
    if (settings.discrete) {
      this.salvaImpostazioniTemaDiscreto(settings, layer, tipoLayerSelected);
      return;
    }

    this.salvaImpostazioniTemaContinuo(settings, layer, tipoLayerSelected);
  }

  focusVisibleItems(layer: TipologiaLayer): void {
    this.googleMapGeoJsonService.focusThemeVisibleFeatures(layer, this.temaSelezionato, this.scalaColori);
  }

  setContinuousInterval($event: [number, number], layer: TipologiaLayer) {
    this.caricaScalaColoriContinua(this.temaSelezionato, layer, $event);
    this.applicaTematizzazioneLayer(layer);
  }

  private inizializzaAperturaFinestra(layer: TipologiaLayer): void {
    if (layer != null) {
      this.impostaTemaSelezionato(null, layer);
    }

    this.visualizzaImpostazioniTema = false;
  }

  private salvaImpostazioniTemaDiscreto(settings: ImpostazioniTema, layer: TipologiaLayer, tipoLayerSelected: ObjOptionHTML_Out): void {
    const tileLabels = JSON.parse(JSON.stringify(settings.tileLabels)) as TipologiaLabelDto[] | null;
    const tileLabelsFiltered = tileLabels?.filter(x => x.operation != Enum_Tipo_Operazione.DELETE);

    // Aggiorna tema selezionato locale
    const temaModificato = Object.assign({}, this.temaSelezionato);
    temaModificato.tileLabels = tileLabelsFiltered;
    this.impostaTemaModificato(temaModificato, layer);
    this.applicaTematizzazioneLayer(layer);

    const tileLabelsColorFixed = tileLabelsFiltered.map(dto => ({ ...dto, colore: dto.colore?.substring(1) } as TipologiaLabelDto));

    // Aggiorna tema layer selezionato
    const indice = layer.tiles.findIndex(tile => tile.id === this.temaSelezionato.id?.toString());
    layer.tiles[indice].tilelabels = tileLabelsColorFixed;

    // Chiusura impostazioni
    this.visualizzaImpostazioniTema = false;

    // Salvataggio su database
    this.salvaImpostazioniTemaDatabaseDiscreto(tileLabels, layer, tipoLayerSelected);

    // Update tiles
    this.loadTiles.next(true);
  }

  private salvaImpostazioniTemaDatabaseDiscreto(tileLabelsDto: TipologiaLabelDto[] | null, layer: TipologiaLayer, tipoLayerSelected: ObjOptionHTML_Out): void {
    if (tileLabelsDto == null) {
      this.giasDialogService.baseError("", "gis.TemaLabelsNull");
      return;
    }

    const tileLabelsColorFixed = tileLabelsDto.map(dto => ({ ...dto, colore: dto.colore?.substring(1) } as TipologiaLabelDto));

    // Insert
    const insertPayload = { tipoOperazione: Enum_Tipo_Operazione.INSERT, tipologiaLabel: tileLabelsColorFixed.filter(x => x.operation == Enum_Tipo_Operazione.INSERT) } as LayerTilesDescrizione_In;
    const updatePayload = { tipoOperazione: Enum_Tipo_Operazione.UPDATE, tipologiaLabel: tileLabelsColorFixed.filter(x => x.operation == Enum_Tipo_Operazione.UPDATE) } as LayerTilesDescrizione_In;
    const deletePayload = { tipoOperazione: Enum_Tipo_Operazione.DELETE, tipologiaLabel: tileLabelsColorFixed.filter(x => x.operation == Enum_Tipo_Operazione.DELETE) } as LayerTilesDescrizione_In;
    const readPayload = {
      TipologiaLayer_cod: +tipoLayerSelected.Option_Value,
      LayerElementiGrafici_Cod: +layer.id,
      LayerTiles_Cod: this.temaSelezionato.id
    } as LeggiLayerTilesDescrizione_In;

    forkJoin([
      insertPayload.tipologiaLabel.length == 0 ? of([]) : this.gisClient.gisLayerTilesDescrizione(insertPayload),
      updatePayload.tipologiaLabel.length == 0 ? of([]) : this.gisClient.gisLayerTilesDescrizione(updatePayload),
      deletePayload.tipologiaLabel.length == 0 ? of([]) : this.gisClient.gisLayerTilesDescrizione(deletePayload)
    ])
      .pipe(
        switchMap(() => this.gisClient.gisLeggiLayerTilesDescrizione(readPayload)),
        tap(res => this.temaSelezionato.tileLabels = this.parseTipologiaLabelDto(res.RispostaStringa))
      )
      .subscribe({
        next: () => this.giasDialogService.baseSuccess("", "gis.TemaSalvatoOk"),
        error: () => this.giasDialogService.baseError("", "gis.TemaSalvatoNonOk")
      });
  }

  private salvaImpostazioniTemaContinuo(settings: ImpostazioniTema, layer: TipologiaLayer, tipoLayerSelected: ObjOptionHTML_Out): void {
    const indice = layer.tiles.findIndex(tile => tile.id === this.temaSelezionato.id?.toString());

    // Aggiorna tema selezionato locale
    const temaModificato = Object.assign({}, this.temaSelezionato);
    temaModificato.colore1 = settings.colore1;
    temaModificato.colore2 = settings.colore2;
    temaModificato.varianza = Number(settings.varianza);
    temaModificato.tileLabels = null;

    this.impostaTemaModificato(temaModificato, layer);
    this.applicaTematizzazioneLayer(layer);

    // Aggiorna tema layer selezionato
    layer.tiles[indice].colore_primario = settings.colore1?.substring(1);
    layer.tiles[indice].colore_secondario = settings.colore2?.substring(1);
    layer.tiles[indice].varianza = settings.varianza?.toString();
    layer.tiles[indice].tilelabels = null;

    // Chiusura impostazioni
    this.visualizzaImpostazioniTema = false;

    // Salvataggio su database
    this.salvaImpostazioniTemaDatabaseContinuo(indice, settings, layer, tipoLayerSelected);

    // Update tiles
    this.loadTiles.next(true);
  }

  private salvaImpostazioniTemaDatabaseContinuo(indiceTema: number, settings: ImpostazioniTema, layer: TipologiaLayer, tipoLayerSelected: ObjOptionHTML_Out): void {
    const listaTemiDaSalvare: DatiTema[] = [{
      ID: layer.tiles[indiceTema].id,
      Colore_Primario: layer.tiles[indiceTema].colore_primario,
      Colore_Secondario: layer.tiles[indiceTema].colore_secondario,
      Varianza: layer.tiles[indiceTema].varianza,
      TipologiaLayer_Cod: this.sharedDataService.getTipoLayerSelezionato(),
      LayerElementiGrafici_Cod: layer.id,
    }];

    const payload = {
      Tipologia: 2,
      ListaDatiTema: listaTemiDaSalvare
    } as SalvaColoriLayer2_In;

    const tileLabels = JSON.parse(JSON.stringify(settings.tileLabels)) as TipologiaLabelDto[] | null;
    const tileLabelsColorFixed = tileLabels.filter(x => x.operation != Enum_Tipo_Operazione.INSERT).map(dto => ({ ...dto, colore: dto.colore?.substring(1) } as TipologiaLabelDto));
    const deletePayload = { tipoOperazione: Enum_Tipo_Operazione.DELETE, tipologiaLabel: tileLabelsColorFixed } as LayerTilesDescrizione_In;
    const readPayload = {
      TipologiaLayer_cod: +tipoLayerSelected.Option_Value,
      LayerElementiGrafici_Cod: +layer.id,
      LayerTiles_Cod: this.temaSelezionato.id
    } as LeggiLayerTilesDescrizione_In;

    forkJoin([
      this.gisClient.gisSalvaColoriLayer2(payload),
      deletePayload.tipologiaLabel.length == 0 ? of([]) : this.gisClient.gisLayerTilesDescrizione(deletePayload)
    ])
      .pipe(
        switchMap(() => this.gisClient.gisLeggiLayerTilesDescrizione(readPayload)),
        tap(res => this.temaSelezionato.tileLabels = this.parseTipologiaLabelDto(res.RispostaStringa))
      )
      .subscribe({
        next: () => this.giasDialogService.baseSuccess("", "gis.TemaSalvatoOk"),
        error: e => this.giasDialogService.baseError("", "gis.TemaSalvatoNonOk")
      });
  }

  private applicaTematizzazioneLayer(layer: TipologiaLayer): void {
    // Reset first old colors
    this.ripristinaColoreLayer(layer);
    this.googleMapGeoJsonService.applicaTematizzazioneLayer(layer, this.temaSelezionato, this.scalaColori);
  }

  private inizializzaTemaSelezionato(layer: TipologiaLayer): void {
    this.ripristinaColoreLayer(layer);
    this.temaSelezionato = {} as TemaSelezionato;
    this.sharedDataService.setTemaSelezionato(null);
  };

  private ripristinaColoreLayer(layer: TipologiaLayer): void {
    this.googleMapGeoJsonService.changeColorLayer(layer?.id ?? '', false);
  }

  private impostaTemaSelezionato(nuovoTileSelezionato: TipologiaTile | null, layer: TipologiaLayer): void {
    if (nuovoTileSelezionato == null) {
      this.temaSelezionato = null;
      this.sharedDataService.setTemaSelezionato(this.temaSelezionato);
      return;
    }

    this.temaSelezionato = {
      layerId: layer?.id,
      nome: nuovoTileSelezionato.nome,
      id: parseInt(nuovoTileSelezionato.id),
      colore1: '#' + nuovoTileSelezionato.colore_primario,
      colore2: '#' + nuovoTileSelezionato.colore_secondario,
      varianza: parseInt(nuovoTileSelezionato.varianza),
      tileLabels: this.parseTipologiaLabelDto(nuovoTileSelezionato.tilelabels),
      applicaTema: true
    } as TemaSelezionato;

    this.caricaScalaColori(layer);

    this.sharedDataService.setTemaSelezionato(this.temaSelezionato);
  }

  private impostaTemaModificato(temaModificato: TemaSelezionato, layer: TipologiaLayer): void {
    this.temaSelezionato.colore1 = temaModificato.colore1;
    this.temaSelezionato.colore2 = temaModificato.colore2;
    this.temaSelezionato.varianza = temaModificato.varianza;
    this.temaSelezionato.tileLabels = JSON.parse(JSON.stringify(temaModificato.tileLabels))
    this.temaSelezionato.applicaTema = true;

    this.caricaScalaColori(layer);

    this.sharedDataService.setTemaSelezionato(this.temaSelezionato);
  }

  private caricaScalaColori(layer: TipologiaLayer): void {
    if (this.isThemeDiscrete()) {
      this.caricaScalaColoriDiscreta(this.temaSelezionato.tileLabels);
    } else {
      this.caricaScalaColoriContinua(this.temaSelezionato, layer);
    }
  }

  private caricaScalaColoriContinua(temaSelezionato: TemaSelezionato, layer: TipologiaLayer, interval?: [number, number]): void {
    this.scalaColori = [];
    if (temaSelezionato == null) {
      return;
    }

    const colore1_rgb = hex2rgb(temaSelezionato.colore1);
    const colore2_rgb = hex2rgb(temaSelezionato.colore2);
    const varianza = temaSelezionato.varianza;

    const features = this.featureInformationService.getByLayer(layer.id);
    const limiti = this.sharedDataService.getLimitiTematizzazioneLayer(features, layer, temaSelezionato.nome);
    temaSelezionato.valoreMax = limiti.valoreMax;

    const step_varianza = this.calcolaStepVarianza(limiti.valoreMax, limiti.valoreMin);
    const step_r = this.calcolaStepVarianza(colore2_rgb.r, colore1_rgb.r);
    const step_g = this.calcolaStepVarianza(colore2_rgb.g, colore1_rgb.g);
    const step_b = this.calcolaStepVarianza(colore2_rgb.b, colore1_rgb.b);

    const min = interval != null ? interval[0] : null;
    const max = interval != null ? interval[1] : null;
    for (let i = 1; i <= varianza; i++) {
      const valoreScala = this.calcolaValoreScala(limiti.valoreMin, step_varianza, i, 2);
      const objColoreScala = {
        r: this.calcolaValoreScala(colore1_rgb.r, step_r, i, 0),
        g: this.calcolaValoreScala(colore1_rgb.g, step_g, i, 0),
        b: this.calcolaValoreScala(colore1_rgb.b, step_b, i, 0)
      };

      const datiColore: DatiColoreTema = {
        valoreMin: null,
        valoreMax: valoreScala,
        colore: objColoreScala,
        visible: min == null || max == null || (min <= valoreScala && valoreScala <= max)
      };

      this.scalaColori.push(datiColore);
    }

    this.sharedDataService.setScalaColoriTema(this.scalaColori);
  }

  private calcolaStepVarianza(max: number, min: number): number {
    return (max - min) / (this.temaSelezionato.varianza - 1);
  }

  private calcolaValoreScala(base: number, step: number, indice: number, numeroDecimali: number): number {
    const valoreCalcolato = step * (indice - 1);
    const valoreCalcolatoArrotondato = this.funzioniComuniService.roundNumber(valoreCalcolato, numeroDecimali);
    return base + valoreCalcolatoArrotondato;
  }

  private caricaScalaColoriDiscreta(tileLabels: TipologiaLabelDto[]): void {
    this.scalaColori = [];

    for (const tileLabel of tileLabels) {
      this.scalaColori.push({
        valoreMin: tileLabel.singleValue ? tileLabel.valore_associato : tileLabel.valore_min,
        valoreMax: tileLabel.singleValue ? tileLabel.valore_associato : tileLabel.valore_max,
        colore: hex2rgb(tileLabel.colore),
        visible: tileLabel.visible
      });
    }

    this.sharedDataService.setScalaColoriTema(this.scalaColori);
  }

  private parseTipologiaLabelDto(tileLabels: TipologiaLabel[] | null): TipologiaLabelDto[] | null {
    if (tileLabels == null) {
      return null;
    }

    const result = JSON.parse(JSON.stringify(tileLabels)) as TipologiaLabel[];
    return result.map(x => ({
      ...x,
      colore: x.colore == null ? null : `#${x.colore}`,
      operation: Enum_Tipo_Operazione.UPDATE,
      visible: true,
      singleValue: x.valore_associato != 0
    } as TipologiaLabelDto));
  }
}
