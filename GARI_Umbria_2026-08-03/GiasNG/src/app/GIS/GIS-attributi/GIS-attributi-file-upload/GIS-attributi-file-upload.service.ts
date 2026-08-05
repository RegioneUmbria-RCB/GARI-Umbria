import { Injectable } from '@angular/core';
import { FileRestrictions } from '@progress/kendo-angular-upload';
import { FileParameter, GisClient, RispostaStandard, TipoFileCatasto } from 'app/Service/api.service';
import { DropdownListItem } from 'gias-kendo-grid';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';

@Injectable()
export class GISAttributiFileUploadService {
  private agendaLoadingSubject = new BehaviorSubject<boolean>(false);

  constructor(private gisClient: GisClient) { }

  public get agendaLoading$(): Observable<boolean> {
    return this.agendaLoadingSubject.asObservable();
  }

  // this call must survive after the caller has died
  public caricaFileShape(file: FileParameter, origine: OrigineDatiModel, centro: DropdownListItem, sistema: SistemaRiferimentoItem, validityStart: Date | null, validityEnd: Date | null, piva: string, layerId: number): Observable<RispostaStandard> {
    this.agendaLoadingSubject.next(true);

    return this.gisClient.gisCaricaFileShape(
      /* codice_sistemaRiferimento */ +sistema.Value,
      /* fileZip */ file,
      /* layer_cod */ layerId,
      /* tipologiaShape_cod */ origine.codice,
      /* progressivoGIAS */ 0,
      /* validita_Inizio */ validityStart,
      /* validita_Fine */ validityEnd,
      /* description */ null,
      /* pixelSize */ 0,
      /* datiImpianto_sa_cod */ centro.id,
      /* datiImpianto_campo_cod */ 0,
      /* datiImpianto_piva */ piva,
      /* datiImpianto_id_reg */ 0,
      /* datiImpianto_appezza */ 0,
    ).pipe(
      tap(() => this.agendaLoadingSubject.next(false)),
      catchError(err => {
        this.agendaLoadingSubject.next(false);
        return throwError(() => err);
      })
    );
  }

  public caricaShapeFileRaster(file: FileParameter, origine: OrigineDatiModel, centro: DropdownListItem, description: string, validityStart: Date | null, validityEnd: Date | null, pixesSize: number, piva: string, layerId: number): Observable<RispostaStandard> {
    this.agendaLoadingSubject.next(true);

    return this.gisClient.gisCaricaFileShape(
      /* codice_sistemaRiferimento */ -1,
      /* fileZip */ file,
      /* layer_cod */ layerId,
      /* tipologiaShape_cod */ origine.codice,
      /* progressivoGIAS */ 0,
      /* validita_Inizio */ validityStart,
      /* validita_Fine */ validityEnd,
      /* description */ description,
      /* pixelSize */ pixesSize,
      /* datiImpianto_sa_cod */ centro.id,
      /* datiImpianto_campo_cod */ 0,
      /* datiImpianto_piva */ piva,
      /* datiImpianto_id_reg */ 0,
      /* datiImpianto_appezza */ 0,
    ).pipe(
      tap(() => this.agendaLoadingSubject.next(false)),
      catchError(err => {
        this.agendaLoadingSubject.next(false);
        return throwError(() => err);
      })
    );
  }

  public caricaShapeFileCatasto(
    datiCatasto: DatiCatastoInput,
    sistema: SistemaRiferimentoItem,
    file: FileParameter,
    origine: OrigineDatiModel,
    layerId: number
  ): Observable<RispostaStandard> {
    this.agendaLoadingSubject.next(true);

    return this.gisClient.gisCaricaFileCatasto(
      /* datiCatasto_ComportamentoImportazione */ datiCatasto.comportamentoImportazione,
      /* datiCatasto_CreaLayerTestuale */ datiCatasto.creaLayerTestuale,
      /* datiCatasto_CodBelfiore */ datiCatasto.codBelfiore,
      /* datiCatasto_Provincia */ datiCatasto.provincia,
      /* datiCatasto_Comune */ datiCatasto.comune,
      /* datiCatasto_Sezione */ datiCatasto.sezione,
      /* datiCatasto_Foglio */ datiCatasto.foglio,
      /* datiCatasto_Particella */ datiCatasto.particella,
      /* datiCatasto_Subalterno */ datiCatasto.subalterno,
      /* datiCatasto_IdentificativoEsterno */ datiCatasto.identificativoEsterno,
      /* datiCatasto_FiltroParticelleCatastali */ datiCatasto.filtroParticelleCatastali,
      /* datiCatasto_ListaLayersDXFAgenziaEntrate */ datiCatasto.listaLayersDXFAgenziaEntrate,
      /* datiCatasto_fileCatasto */ datiCatasto.fileCatasto,
      /* codice_sistemaRiferimento */ +sistema.Value,
      /* fileZip */ file,
      /* layer_cod */ layerId,
      /* tipologiaShape_cod */ origine.codice,
      /* progressivoGIAS */ 0,
      /* validita_Inizio */ null,
      /* validita_Fine */ null,
      /* description */ null,
      /* pixelSize */ 0,
      /* datiImpianto_sa_cod */ 0,
      /* datiImpianto_campo_cod */ 0,
      /* datiImpianto_piva */ '',
      /* datiImpianto_id_reg */ 0,
      /* datiImpianto_appezza */ 0,
    ).pipe(
      tap(() => this.agendaLoadingSubject.next(false)),
      catchError(err => {
        this.agendaLoadingSubject.next(false);
        return throwError(() => err);
      })
    );
  }
}

export interface OrigineDatiModel {
  codice: number;
  descrizione: string;
  restrictions: FileRestrictions;
}

export interface SistemaRiferimentoModel {
  default: string;
  ListaSistemiRiferimento: SistemaRiferimentoItem[];
}

export interface SistemaRiferimentoItem {
  Value: string;
  Text: string;
}

export interface FormatoDatiModel {
  codice: TipoFileCatasto;
  descrizione: string;
}

export interface DatiCatastoInput {
  comportamentoImportazione: number,
  creaLayerTestuale: boolean,
  codBelfiore: string,
  provincia: string,
  comune: string,
  sezione: string,
  foglio: string,
  particella: string,
  subalterno: string,
  identificativoEsterno: string,
  filtroParticelleCatastali: string,
  listaLayersDXFAgenziaEntrate: string,
  fileCatasto: TipoFileCatasto,
}