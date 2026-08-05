import { Component, EventEmitter, Output } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { FileRestrictions, SelectEvent } from '@progress/kendo-angular-upload';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { FeatureService } from 'app/GIS/services/feature.service';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { FileParameter, GisClient } from 'app/Service/api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ISubmitComponent } from '../GIS-gestione-piano-rateo.component';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import { e } from '@angular/cdk/portal-directives.d-BoG39gYN';
import { filter, map, share, switchMap, take } from 'rxjs';

const VALID_EXTENSIONS_LEGACY = [".zip"];
const VALID_EXTENSIONS_ENGINE = [".tif", ".tiff"];

@Component({
  standalone: false,
  selector: 'gis-gestione-piano-rateo-loader',
  templateUrl: './GIS-gestione-piano-rateo-loader.component.html',
  styleUrls: ['./GIS-gestione-piano-rateo-loader.component.css']
})
export class GISGestionePianoRateoLoaderComponent implements ISubmitComponent {

  @Output() close = new EventEmitter<void>();
  isLoading: boolean = false;
  description: string = "";
  rateColumnName: string = "";
  files: FileParameter[] = [];

  isMappePrescrizioneEngineActive$ = this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.MappePrescrizioneEngineIsActive)
    .pipe(
      map(config => config?.Valore?.toLowerCase() === 'true'),
      share()
    );

  restrictions$ = this.isMappePrescrizioneEngineActive$.pipe(
    map(isActive => ({ allowedExtensions: isActive ? VALID_EXTENSIONS_ENGINE : VALID_EXTENSIONS_LEGACY } as FileRestrictions))
  );

  constructor(
    private gisClient: GisClient,
    private funzioniComuniService: FunzioniComuniService,
    private featureService: FeatureService,
    private ricetteService: RicetteService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {
  }

  onFileSelected(event: SelectEvent): void {
    // There should be only one file
    event.files.forEach((file) => {
      this.files = [{ fileName: file.name, data: file.rawFile }];
    });
  }

  onFileRemoved(): void {
    this.files = [];
  }

  isValid(): boolean {
    return this.description != '' && this.rateColumnName != '' && this.files.length == 1;
  }

  submit(): void {
    const title = this.translocoService.translate("gis.GestionePianoRateo");
    this.isMappePrescrizioneEngineActive$.pipe(
      take(1),
      filter(usingEngine => {
        const feature = this.featureService.getFeatureSelezionate()[0];
        if (feature == null) {
          this.close.emit();
          this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaFeature");
          return false;
        }

        const ricetta = this.ricetteService.getSelected()[0];
        if (ricetta == null) {
          this.close.emit();
          this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaRicetta");
          return false;
        }

        const file = this.files[0];
        const validExtensions = usingEngine ? VALID_EXTENSIONS_ENGINE : VALID_EXTENSIONS_LEGACY;
        if (file?.data == null || !validExtensions.some(ext => file?.fileName.endsWith(ext))) {
          const desc = this.translocoService.translate('gis.SelezionaFileValidoConEstensioneX', { extensions: validExtensions.join(', ') });
          this.giasDialogService.baseError(title, desc, false);
          return false;
        }

        return true;
      }),
      switchMap(() => {
        const feature = this.featureService.getFeatureSelezionate()[0];
        const ricetta = this.ricetteService.getSelected()[0];
        const file = this.files[0];

        const chiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(FunzioniComuniService.chiaveAlberoRidottaToBig(feature.properties.chiavealbero));
        const ricetta_Cod = ricetta.Ricetta_Cod.toString();
        const ricetta_operazione_Cod = ricetta.Ricetta_Operazione_Cod.toString();

        this.isLoading = true;
        return this.gisClient
          .gisPfPrescriptionUpload(
          /* chiaveAlbero_TipoNodo */ 60,
          /* chiaveAlbero_Piva */ chiaveAlbero.Piva,
          /* chiaveAlbero_Sa_Cod */ chiaveAlbero.Sa_Cod,
          /* chiaveAlbero_Campo_Cod */ 0,
          /* chiaveAlbero_Appezza */ chiaveAlbero.Appezza,
          /* chiaveAlbero_Id_Imp */ chiaveAlbero.Id_Imp,
          /* chiaveAlbero_p_Part_Cod */ "0",
          /* chiaveAlbero_p_Provincia_Cod */ "0",
          /* chiaveAlbero_p_Comune_Cod */ "0",
          /* chiaveAlbero_p_Sezione */ "0",
          /* chiaveAlbero_p_Foglio */ "0",
          /* chiaveAlbero_p_Numero */ "0",
          /* chiaveAlbero_p_Subalterno */ "0",
          /* chiaveAlbero_Cod_Fiscale */ "0",
          /* chiaveAlbero_Fabbricato_Cod */ "0",
          /* chiaveAlbero_Prodotto_Cod */ "0",
          /* chiaveAlbero_Data_Lavorazione */ "0",
          /* chiaveAlbero_Analisi_Certificato_Cod */ "0",
          /* chiaveAlbero_Analisi_Testata_Cod */ "0",
          /* chiaveAlbero_Analisi_Dettaglio_Cod */ "0",
          /* chiaveAlbero_Analisi_Campione_Cod */ "0",
          /* chiaveAlbero_PianoConcimazioneTestata_Cod */ "0",
          /* chiaveAlbero_Progetto_Cod */ "0",
          /* chiaveAlbero_Programmazione_Cod */ "0",
          /* chiaveAlbero_Programmazione_Entita_Cod */ "0",
          /* chiaveAlbero_Id_Agenda */ "0",
          /* chiaveAlbero_PivaPadre */ "",
          /* chiaveAlbero_Ricetta_Cod */ ricetta_Cod,
          /* chiaveAlbero_RicettaOperazione_Cod */ ricetta_operazione_Cod,
          /* descrizionePiano */ this.description,
          /* psw_SuperUser */ "",
          /* rateColumnName */ this.rateColumnName,
          /* fileZip */ file
          );
      })
    )
      .subscribe({
        next: res => {
          this.giasDialogService.baseSuccess(title, res.RispostaStringa, false);
          this.isLoading = false;
        },
        error: error => {
          this.giasDialogService.baseError(title, FunzioniComuniService.getResponseError(error, this.translocoService), false);
          this.isLoading = false;
        },
        complete: () => this.close.emit()
      });
  }
}
