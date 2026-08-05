import { Component, Inject, OnDestroy, ViewChild } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { GISAttributiMuzGridConfigService } from "./GIS-attributi-muz-grid-config.service";
import { FeatureService } from "app/GIS/services/feature.service";
import { GoogleMapGeoJsonService } from "app/GIS/google-map/google-map-geojson.service";
import { Observable, catchError, combineLatest, filter, map, of, switchMap, tap, withLatestFrom } from "rxjs";
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { DEFAULT_PLOT, GISAttributiMuzService } from "./GIS-attributi-muz.service";
import { enum_FeatureProperty } from "app/GIS/GIS-enum/GIS-feature";
import { enum_LayerElementiGraficiStd } from "app/GIS/GIS-enum/GIS-layer-elementi-grafici";
import { DatiMUZVisibili_Out, Enum_Operazioni_MUZ, GisClient, LeggiDatiMUZVisibili_In, MUZ } from "app/Service/api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { DrawingService } from "app/GIS/services/drawing.service";
import { LayerService } from "app/GIS/services/layer.service";
import { GiasPolygon } from "app/GIS/models/gias-drawings.model";
import { WKTService } from "app/GIS/services/wkt.service";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { MasterService } from "app/Service/master.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { EditFeatureService } from "app/GIS/services/edit-feature.service";
import { faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import { TranslocoService } from "@jsverse/transloco";
import { SelectionEvent } from "@progress/kendo-angular-grid";
import { enum_OrigineChiamata } from "app/GIS/GIS-enum/GIS-origine-chiamata";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN, SelectedOpts } from 'gias-kendo-grid';
import { FeatureInformationService } from "app/GIS/services/feature-information.service";
import { GISAttributiConfigService } from "../GIS-attributi-grid/GIS-attributi-grid-config.service";

@Component({
  standalone: false,
  selector: 'GIS-attributi-muz-grid',
  templateUrl: './GIS-attributi-muz-grid.component.html',
  styleUrls: ['./GIS-attributi-muz-grid.component.css'],
  providers: [...generateGridProviders(GISAttributiMuzGridConfigService, GISAttributiMuzGridComponent)]
})
export class GISAttributiMuzGridComponent implements OnDestroy {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  faDelete = faTrashAlt;
  initMuzDialogOpen = false;
  piva: string;
  muzToEdit$ = this.gisAttributiMuzService.muzToEdit$;

  newMuz$ = this.drawingService.addEvent
    .pipe(
      withLatestFrom(this.layerService.layerItemSelected$),
      filter(([_, [layer, selected]]) => selected && layer != null && layer.id == enum_LayerElementiGraficiStd.Muz),
      map(([draw, _]: [GiasPolygon, any]) => this.wktService.polygonToDataGeometry(draw.polygon)),
      withLatestFrom(this.gisAttributiMuzService.selectedPlot$),
      map(([polygon, plot]) => GISAttributiMuzGridComponent.completeMuz({ Area_Cod: 0, Geometry: this.wktService.geometryToWKT(polygon) }, plot, this.piva)),
      tap(muz => this.gisAttributiMuzService.nextMuzToEdit(muz))
    );

  editMuz$ = this.editFeatureService.editFeature
    .pipe(
      filter(f => f.getProperty(enum_FeatureProperty.layer) == enum_LayerElementiGraficiStd.Muz),
      withLatestFrom(this.gisAttributiMuzService.selectedPlot$),
      tap(([f, plot]) => {
        if (this.gisAttributiMuzService.isDefaultPlot()) {
          this.giasDialogService.baseInfo('', 'gis.SelezionaUnAppezzamentoDallaSchedaDati', true);
          return;
        }

        const muz = GISAttributiMuzGridComponent.completeMuz({ Area_Cod: parseFloat(f.getProperty(enum_FeatureProperty.Area_Cod) as string) }, plot, this.piva);
        this.editMuz(muz, plot, this.wktService.geometryToWKT(f.getGeometry()));
      }),
    );

  data$ = combineLatest({
    selectedPlot: this.gisAttributiMuzService.selectedPlot$,
    groups: this.gisAttributiMuzService.groups$,
    muzs: this.gisAttributiMuzService.muzs$,
    anagrafica: this.gisAttributiMuzService.muzs$
      .pipe(
        filter(muzs => muzs != null),
        switchMap(muzs => this.getAnagrafica(muzs))
      ),
  });

  featureSelection$ = this.featureService.getFeatureSelezionate$()
    .pipe(tap(features => {
    this.kendoGrid?.setSelectedRows({
      keys: features.map(f => f.properties.Area_Cod),
      resetPreviousSelection: true
    } as SelectedOpts);
  }));

  constructor(
    private gisAttributiMuzService: GISAttributiMuzService,
    private featureService: FeatureService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private gisClient: GisClient,
    private objParametriAgendaService: ObjParametriAgendaService,
    private drawingService: DrawingService,
    private layerService: LayerService,
    private editFeatureService: EditFeatureService,
    private wktService: WKTService,
    private masterService: MasterService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private featureInformationService: FeatureInformationService,
    @Inject(GRID_HTTP_TOKEN) private gridhttpService: GISAttributiConfigService,
  ) {
    this.piva = this.objParametriAgendaService.getObjParamValue().Piva;

    this.selectPlot(DEFAULT_PLOT);
    this.getGroups();
    this.getMuzs();
  }

  ngOnDestroy(): void {
    this.gisAttributiMuzService.nextSelectedPlot(DEFAULT_PLOT);
  }

  focusOnFeature(dataItem: any): void {
    this.googleMapGeoJsonService.selezionaFeatureById(dataItem.chiave, true, false);
  }

  selectionChange(selection: SelectionEvent): void {
    this.featureInformationService.getAll().forEach(f => {
      const unselectedMuz = selection.deselectedRows.map(x => x.dataItem).some(x => x.Area_Cod == f.properties.Area_Cod);
      if (unselectedMuz) {
        this.googleMapGeoJsonService.deselezionaFeature(f, true, false, false, enum_OrigineChiamata.Mappa);
      }

      const selectedMuz = selection.selectedRows.map(x => x.dataItem).some(x => x.Area_Cod == f.properties.Area_Cod);
      if (selectedMuz) {
        this.googleMapGeoJsonService.selezionaFeature(f, true, false, enum_OrigineChiamata.Mappa);
      }
    });
  }

  findSelecetedItem(): void {
    const feature = this.featureService.getUltimaFeatureSelezionata();
    if (feature != null) {
      this.gridhttpService.selectRow(feature?.properties.Area_Cod.toString());
    }
  }

  selectPlot(plot: Plot): void {
    this.gisAttributiMuzService.nextSelectedPlot(plot);
  }

  editMuz(muz: DatiMUZVisibili_Out, plot: Plot, geometry?: string): void {
    // Adding new muz
    if (muz.Area_Cod == 0) {
      this.gisAttributiMuzService.nextMuzToEdit(GISAttributiMuzGridComponent.completeMuz(muz, plot, this.piva));
      return;
    }

    // Updating muz
    this.masterService.set_isLoading({ isLoading: true });
    this.gisClient
      .gisLeggiMUZ({ Area_Cod: muz.Area_Cod, Piva: this.piva })
      .pipe(
        catchError(() => {
          this.giasDialogService.baseError('', 'gis.ErroreNelCaricamentoDatiMuz', true);
          return of({ RispostaStringa: [null] });
        }),
        tap(() => this.masterService.set_isLoading({ isLoading: false })),
        map(res => GISAttributiMuzGridComponent.completeMuz({ ...res.RispostaStringa[0], Geometry: geometry ?? muz['Geometry'] }, plot, this.piva)),
      )
      .subscribe(res => this.gisAttributiMuzService.nextMuzToEdit(res));
  }

  deleteMuz(muz: DatiMUZVisibili_Out, plot: Plot, muzs: DatiMUZVisibili_Out[]): void {
    const payload = {
      Area_Cod: muz.Area_Cod,
      Operazione_Cod: Enum_Operazioni_MUZ.DELETE
    } as MUZ;

    const title = this.translocoService.translate('gis.SicuroDiVolerEliminareLaMuzXNomeSullAppezzamentoXAppezzamentoConGruppoXGruppo', { nome: muz['Descrizione'], appezzamento: plot.Desc, gruppo: plot.Group })
    this.giasDialogService.dialogMessageObs_Result('', title)
      .pipe(
        filter((res: any) => res.returnObj),
        tap(() => this.masterService.set_isLoading({ isLoading: true })),
        switchMap(() => this.gisClient.gisOperazioniMUZ(payload))
      )
      .subscribe({
        next: () => {
          this.masterService.set_isLoading({ isLoading: false });
          this.giasDialogService.baseSuccess('', 'gis.MuzEliminataCorrettamente', true);
          this.gisAttributiMuzService.nextMuzs(muzs.filter(m => m.Area_Cod != muz.Area_Cod));
        },
        error: () => {
          this.masterService.set_isLoading({ isLoading: false });
          this.giasDialogService.baseError('', 'gis.ErroreNellEliminazioneDellaMuz', true);
        },
      });
  }

  onMuzEdited(_: DatiMUZVisibili_Out | null): void {
    this.gisAttributiMuzService.nextMuzToEdit(null);
    this.drawingService.removeAllPolygons();
  }

  private getAnagrafica(muzs: DatiMUZVisibili_Out[]): Observable<Plot[]> {
    const result: Plot[] = [];
    this.featureInformationService.getAll().forEach(f => {
      if (f.properties.layer != enum_LayerElementiGraficiStd.APPEZZAMENTI) {
        return;
      }

      const key = FunzioniComuniService.scomponiChiaveAlbero(f.properties.chiavealbero);
      if (key.Piva != this.piva) {
        return;
      }

      const plot = {
        Piva: key.Piva,
        Sa_Cod: key.Sa_Cod,
        Appezza: key.Appezza,
        Desc: f.properties.etichetta,
        Group: null,
        GroupCod: null
      } as Plot;

      const currentMuzs = muzs.filter(muz => GISAttributiMuzService.plotInMuz(muz, plot));
      plot.Group = currentMuzs.length > 0 ? currentMuzs[0].Gruppo.Gruppo_Area_Des : null
      plot.GroupCod = currentMuzs.length > 0 ? currentMuzs[0].Gruppo.Gruppo_Area_Cod : null

      result.push(plot);
    });

    return of([DEFAULT_PLOT, ...result]);
  }

  private getMuzs(): void {
    const result: number[] = [];
    this.featureInformationService.getAll().forEach(f => {
      const area_cod = f.properties.Area_Cod;
      if (area_cod != 0) {
        result.push(area_cod);
      }
    });

    if (result.length == 0) {
      this.gisAttributiMuzService.nextMuzs([]);
      return;
    }

    const body = { lista_MUZ_Cod: result, piva: '' } as LeggiDatiMUZVisibili_In;
    this.gisClient
      .gisLeggiDatiMUZVisibili(body)
      .subscribe(res => this.gisAttributiMuzService.nextMuzs(res.RispostaStringa));
  }

  private getGroups(): void {
    this.gisClient.gisLeggiListaGruppiMUZ(this.piva)
      .pipe(map(res => res.RispostaStringa))
      .subscribe(groups => this.gisAttributiMuzService.nextGroups(groups));
  }

  private static completeMuz(muz: MUZ, plot: Plot, piva: string): MUZ {
    if (muz == null) {
      return null;
    }

    muz.Altimetria ??= ''
    muz.Analisi_Testate ??= [];
    muz.Analisi_Testate_Aggiungi = [];
    muz.Analisi_Testate_Elimina = [];
    muz.appezzamento ??= [plot];
    muz.Area_Cod ??= 0;
    muz.Area_Des ??= '';
    muz.Geometry ??= '';
    muz.Gruppo_Area_Cod = muz.Gruppo_Area_Cod != 0 && muz.Gruppo_Area_Cod != null ? muz.Gruppo_Area_Cod : plot.GroupCod;
    muz.Gruppo_Area_Des = muz.Gruppo_Area_Des != '' && muz.Gruppo_Area_Des != null ? muz.Gruppo_Area_Des : plot.Group;
    muz.Particelle_Catastali ??= [];
    muz.Particelle_Catastali_Aggiungi ??= [];
    muz.Particelle_Catastali_Elimina ??= [];
    muz.Piva ??= piva;
    muz.SO ??= 0;
    muz.Tessitura_cod ??= 0;
    muz.TipoZona ??= '';
    muz.Validita_Fine = new Date(muz.Validita_Fine ?? AGRODATAFINE);
    muz.Validita_Inizio = new Date(muz.Validita_Inizio ?? AGRODATAINIZIO);

    return muz;
  }
}

export interface Plot {
  Piva: string;
  Sa_Cod: number;
  Appezza: number;
  Desc: string;
  GroupCod: number | null;
  Group: string | null;
}
