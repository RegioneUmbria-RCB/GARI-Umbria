import { ContestoColore, GisLayerColorPickerService, GisLayerColorPickerWindowArgs } from 'app/GIS/GIS-layer-color-picker-window/GIS-layer-color-picker-window.service';
import { GisFixedLayerPropertyService } from 'app/GIS/GIS-fixed-layer-property-window/GIS-fixed-layer-property-window.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { DatiLayer, TipologiaLayer, ObjOptionHTML_Out, RispostaStandard } from 'app/Service/api.service';
import { Observable, switchMap, tap } from 'rxjs';
import { GisToolbarService } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { TranslocoService } from '@jsverse/transloco';

export class BaseLayerListItemComponent {

  enumFeatureType = FeatureType;

  constructor(
    protected pickerService: GisLayerColorPickerService,
    protected fixedLayerService: GisFixedLayerPropertyService,
    protected layerService: LayerService,
    protected gisToolbarService: GisToolbarService,
    protected kendoWindowsService: KendoWindowsService,
    protected translocoService: TranslocoService
  ) { }

  getImageUrl(contactId: number): string {
    return `https://www.telerik.com/kendo-angular-ui-develop/components/listview/assets/contacts/${contactId}.png`;
  }

  getMessagesText(messagesCount: number): string {
    return `${messagesCount} new message${messagesCount > 1 ? 's' : ''}`;
  }

  onColorChange(color: string, item: TipologiaLayer): void {
    // se colore in RGB o RGBA
    // item.colore_1 = color;
    // const colorParts = color.split(',');
    // item.colore_1 = colorParts[0].replace('rgba(', 'rgb(') + ',' + colorParts[1] + ',' + colorParts[2].replace(',', ')') + ')';
    // item.trasparenza = colorParts[3].replace(')', '').trim();

    const backEndColor = this.layerService.getBackEndColorByHex(color, item.trasparenza);
    item.colore_1 = backEndColor.Color;
    item.trasparenza = backEndColor.Transparency;
  }

  submit(item: TipologiaLayer | null, type: ObjOptionHTML_Out): Observable<RispostaStandard> {
    let payloadData: DatiLayer[] = [{
      ID: item?.id,
      Colore_Primario: item?.colore_1,
      Colore_Secondario: null,
      Varianza: item?.varianza,
      Trasparenza: item?.trasparenza?.replace(',', '.'),
      ZIndex: item?.zindex,
      Flag_Visibile: 1, //scommentare se si vuole salvare visibilità +item?.flagvisibile,
      MostraDescrizioneAssociata: item?.MostraDescrizioneAssociata,
      TipologiaLayer_Cod: type.Option_Value
    } as DatiLayer];

    return this.layerService.submit(1, payloadData);
  }

  togglePicker($event: Event, item: TipologiaLayer | null, type: ObjOptionHTML_Out): void {
    $event.stopPropagation();
    const colorPickerWindow = <GisLayerColorPickerWindowArgs>this.kendoWindowsService.getWindowArgs(WindowTypes.ColorPickerWindow);
    if (colorPickerWindow?.openState && colorPickerWindow?.contesto === ContestoColore.Tema) {
      this.kendoWindowsService.close(WindowTypes.ColorPickerWindow);
      setTimeout(() => {
        this.apriColorPicker(item, type);
      }, 200);
    } else {
      this.apriColorPicker(item, type);
    }
  }

  private apriColorPicker(item: TipologiaLayer | null, type: ObjOptionHTML_Out): void {
    this.pickerService
      .open(this.inputColor(item), ContestoColore.Layer)
      .pipe(
        tap(color => this.onColorChange(color, item)),
        switchMap(() => this.submit(item, type)),
        tap(() => this.layerService.LayerItemChangeColor.next(item))
      )
      .subscribe();
  }

  toggleFixedLayerEntita($event: Event, item: TipologiaLayer | null, type: ObjOptionHTML_Out): void {
    $event.stopPropagation();
    this.fixedLayerService
      .open(this.inputColor(item))
      .pipe(
        tap(color => this.onColorChange(color, item)),
        switchMap(() => this.submit(item, type)),
        tap(() => this.layerService.LayerItemChangeColor.next(item))
      )
      .subscribe();
  }

  toggleTheme($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();
    this.layerService.toggleLayerItemSelected(item, true);
    this.gisToolbarService.themeBtnToggle(true);
  }

  toggleVisible($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();
    this.layerService.toggleLayerItemVisible(item);
  }

  toggleDesc($event: Event, item: TipologiaLayer | null, type: ObjOptionHTML_Out): void {
    $event.stopPropagation();
    this.layerService.toggleLayerItemLabelVisible(item);
    this.submit(item, type).subscribe();
  }

  toggleGrouping($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();
    this.layerService.toggleLayerItemGrouping(item);
    // scommentare se si vuole salvare raggruppamento;
    // this.submit().subscribe();
  }

  centraMappaLayer($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();
    this.layerService.layerItemCenterMap(item.id);
  }

  tilesPresenti(item: TipologiaLayer | null): boolean {
    return item.tiles !== null && item.tiles.length > 0;
  }

  toggleAnalisiMappeSatellitari($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();

    const args = this.kendoWindowsService.getWindowArgs(WindowTypes.AnalisiMappeSatellitariWindow);
    args.additionalArgs = { originalTitle: item.nome };
    this.kendoWindowsService.open(WindowTypes.AnalisiMappeSatellitariWindow, args, false);
  }

  openRasterConfig($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();

    const title = `${this.translocoService.translate('gis.ImpostazioniLayers')} ${item.nome}`;
    const args = new WindowArgs(WindowTypes.RasterConfigurationWindow, false, title, null, 340, 340, undefined, undefined, true, true, true, false, false, false, true);
    args.additionalArgs = { raster: item };
    this.kendoWindowsService.open(WindowTypes.RasterConfigurationWindow, args, false);
  }

  inputColor(item: TipologiaLayer | null): string {
    // attualmente la trasparenza arriva dal BE in formato '0,x', quindi è necessario riconvertirla in numero (0.x) e poi convertirlo a sua volta in esadecimale.
    const opacity = Math.round(Math.min(Math.max(+(item?.trasparenza.replace(',', '.')) || 1, 0), 1) * 255);
    return item?.colore_1 + opacity.toString(16);
  }
}
