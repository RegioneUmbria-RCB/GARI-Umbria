import { Component, EventEmitter, Output } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { FeatureService } from 'app/GIS/services/feature.service';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { GisClient, PfRateoSrv_In } from 'app/Service/api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ISubmitComponent } from '../GIS-gestione-piano-rateo.component';
import { filter, map } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gis-gestione-piano-rateo-picker',
  templateUrl: './GIS-gestione-piano-rateo-picker.component.html',
  styleUrls: ['./GIS-gestione-piano-rateo-picker.component.css']
})
export class GISGestionePianoRateoPickerComponent implements ISubmitComponent {

  @Output() close = new EventEmitter<void>();

  isLoading: boolean = false;
  description: string = "";
  cellsize: string = "20";
  referenceDate: Date | null = null;
  selectedFeatureGuid$ = this.featureService.getFeatureSelezionate$()
    .pipe(
      filter(features => features.length > 0),
      map(features => features[0].properties.Entita_GUID)
    );

  constructor(
    private gisClient: GisClient,
    private funzioniComuniService: FunzioniComuniService,
    private featureService: FeatureService,
    private ricetteService: RicetteService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService
  ) {
  }

  isValid(): boolean {
    return this.description != '';
  }

  submit(): void {
    const feature = this.featureService.getFeatureSelezionate()[0];
    if (feature == null) {
      this.close.emit();
      this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaFeature");
      return;
    }

    const ricetta = this.ricetteService.getSelected()[0];
    if (ricetta == null) {
      this.close.emit();
      this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaRicetta");
      return;
    }

    const chiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(FunzioniComuniService.chiaveAlberoRidottaToBig(feature.properties.chiavealbero));
    const ricetta_Cod = ricetta.Ricetta_Cod.toString();
    const ricetta_operazione_Cod = ricetta.Ricetta_Operazione_Cod.toString();

    const body = {
      ChiaveAlbero: {
        TipoNodo: 60,
        Piva: chiaveAlbero.Piva,
        Sa_Cod: chiaveAlbero.Sa_Cod,
        Campo_Cod: 0,
        Appezza: chiaveAlbero.Appezza,
        Id_Imp: chiaveAlbero.Id_Imp,
        p_Part_Cod: "0",
        p_Provincia_Cod: "0",
        p_Comune_Cod: "0",
        p_Sezione: "0",
        p_Foglio: "0",
        p_Numero: "0",
        p_Subalterno: "0",
        Cod_Fiscale: "0",
        Fabbricato_Cod: "0",
        Prodotto_Cod: "0",
        Data_Lavorazione: "0",
        Analisi_Certificato_Cod: "0",
        Analisi_Testata_Cod: "0",
        Analisi_Dettaglio_Cod: "0",
        Analisi_Campione_Cod: "0",
        PianoConcimazioneTestata_Cod: "0",
        Progetto_Cod: "0",
        Programmazione_Cod: "0",
        Programmazione_Entita_Cod: "0",
        Id_Agenda: "0",
        PivaPadre: "",
        Ricetta_Cod: ricetta_Cod,
        RicettaOperazione_Cod: ricetta_operazione_Cod
      },
      DescrizionePiano: this.description,
      CellSize: this.cellsize,
      DataRiferimento_LetturaDatiSentinel: this.referenceDate ?? new Date("1899-12-31T23:00:00.000Z"),
      oCfgLetturaPF: {
        ListaCodici_RicetteOperazioniCod_MappePrescrizione: [0],
        ListaCodici_RicetteOperazioniCod_MappeProduzione: [0],
        ListaCodici_AllegatiDocumentiCod_MappePrescrizione: [0],
        ListaCodici_AllegatiDocumentiCod_MappeProduzione: [0]
      },
      psw_SuperUser: ""
    } as PfRateoSrv_In;

    this.isLoading = true;
    const title = this.translocoService.translate("gis.GestionePianoRateo");
    this.gisClient
      .gisPfRateoSrv(body)
      .subscribe({
        next: res => {
          this.giasDialogService.baseSuccess(title, res.RispostaStringa, false);
          this.isLoading = false;
        },
        error: () => {
          this.giasDialogService.baseError("gis.GestionePianoRateo", "ErroreSalvataggio");
          this.isLoading = false;
        },
        complete: () => this.close.emit()
      });
  }
}
