import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { AjaxNetCoreDataExchangeService } from 'app/Service/ajax-net-core-data-exchange.service';
import { MetaschemaClient } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {
  AppezzamentiListaRequest,
  AggregaConsiglioNutrizioneRequest,
  DatiWidgetNutrizioneResult,
  SpecieVegetale,
  SalvaConsiglioNutrizioneRequest,
  SalvataggioConsiglioNutrizioneResponse,
  AggregazioneConsiglioNutrizioneResult,
  AppezzamentoNutrizioneDto,
  FasiFenologicheRequest,
  FaseFenologicaCorrenteDto,
} from '../models/dss-nutrizione.model';

/**
 * Service responsible for all API interactions of the DSS Nutrizione feature.
 * Spec reference: UIDS001 FR002
 */
@Injectable()
export class DssNutrizioneService {

  constructor(
    private ajaxService: AjaxNetCoreDataExchangeService,
    private metaschemaClient: MetaschemaClient,
    private objParametriAgendaService: ObjParametriAgendaService,
  ) { }

  private get piva(): string {
    return this.objParametriAgendaService.getObjParamValue()?.Piva ?? '';
  }

  /**
   * Retrieves the list of active appezzamenti for the current company.
   * DS05-BL §Input – CaricamentoDatiWidgetNutrizione
   * FR002 §1 – Lista Appezzamenti Attivi per Azienda
   * Endpoint: POST /DSSNutrizione/widget/dati
   */
  getAppezzamentiAttivi(): Observable<AppezzamentoNutrizioneDto[]> {
    const request: AppezzamentiListaRequest = {
      piva: this.piva,
      sa_cod: 0,
      anno_solare: new Date().getFullYear()
    };
    return this.ajaxService
      .ajaxAPIPost<AppezzamentiListaRequest, DatiWidgetNutrizioneResult>(
        '/DSSNutrizione/widget/dati',
        request,
        false,
        true,
        true
      )
      .pipe(
        map(resp => (resp.RispostaOK ? (resp.RispostaStringa?.Appezzamenti ?? []) : [])),
        catchError(() => of([]))
      );
  }

  /**
   * Retrieves the current consiglio nutrizionale for a single appezzamento.
   * Each widget card loads its own consiglio independently; an error on one
   * does not block the others.
   * DS05B-API §Formato Richiesta
   * FR004 – Recupero Dati Primari e Consiglio Nutrizionale del Singolo Widget
   * Endpoint: POST /DSSNutrizione/appezzamenti/consigli/aggregato
   * @param appezzamento - Appezzamento data from the list phase (DS05-BL §Output).
   *   Piva is already included in the Appezzamento object.
   */
  getConsiglioNutrizionale(
    appezzamento: AppezzamentoNutrizioneDto
  ): Observable<AggregazioneConsiglioNutrizioneResult | null> {
    const request: AggregaConsiglioNutrizioneRequest = {
      Appezzamento: appezzamento,
      SalvaConsiglioNutrizione: false,
    };
    return this.ajaxService
      .ajaxAPIPost<AggregaConsiglioNutrizioneRequest, AggregazioneConsiglioNutrizioneResult>(
        '/DSSNutrizione/appezzamenti/consigli/aggregato',
        request,
        false,
        false,
        false
      )
      .pipe(
        map(resp => (resp.RispostaOK ? (resp.RispostaStringa ?? null) : null)),
        catchError(() => of(null))
      );
  }

  /**
   * Retrieves the list of specie vegetali present in the company's appezzamenti.
   * Used to populate the MultiSelect filter.
   * FR002 §2 – Popolamento Filtro Specie Vegetale
   * Endpoint: GET /Metaschema/specie/aziendali
   */
  getSpecieAziendali(): Observable<SpecieVegetale[]> {
    return this.metaschemaClient
      .metaschemaLeggiSpecieAziendali(this.piva)
      .pipe(
        map(resp => (JSON.parse(resp.RispostaStringa as unknown as string) ?? []) as SpecieVegetale[]),
        catchError(() => of([]))
      );
  }

  /**
   * Persists the currently displayed consiglio nutrizionale for a specific appezzamento.
   * The backend handles versioning of previous advice in the storico.
   * Returns the full response envelope so the caller can inspect `Esito` (SUCCESS / DUPLICATE / ERROR).
   * DS16-API §POST /dss/nutrizione/consigli/salva
   * FR012 – Salvataggio Consiglio Nutrizionale nel Sistema GIAS
   * @param request - Aggregation result and ControllaDuplicati flag.
   */
  salvaConsiglio(request: SalvaConsiglioNutrizioneRequest): Observable<SalvataggioConsiglioNutrizioneResponse | null> {
    return this.ajaxService
      .ajaxAPIPost<SalvaConsiglioNutrizioneRequest, SalvataggioConsiglioNutrizioneResponse>(
        '/DSSNutrizione/consigli/salva',
        request,
        false,
        true,
        true
      )
      .pipe(
        map(resp => resp.RispostaOK === true ? (resp.RispostaStringa ?? null) : null),
        catchError(() => of(null))
      );
  }

  /**
   * Retrieves all phenological phases recorded for the appezzamento's impianto
   * in the current calendar year, in chronological ascending order.
   * Request body is the full AppezzamentoNutrizioneDto (DS05-BL §Output) per DS21-API §Input.
   * DS21-API §POST /DSSNutrizione/fasi-fenologiche-registrate
   * FR011 – Recupero Fasi Fenologiche dell'Impianto
   * @param appezzamento - Appezzamento data from the list phase (DS05-BL §Output).
   */
  getFasiFenologiche(appezzamento: AppezzamentoNutrizioneDto): Observable<FaseFenologicaCorrenteDto[]> {
    const year = new Date().getFullYear();
    const request: FasiFenologicheRequest = {
      Appezzamento: appezzamento,
      ValiditaInizio: `${year}-01-01`,
      ValiditaFine: `${year}-12-31`,
    };
    return this.ajaxService
      .ajaxAPIPost<FasiFenologicheRequest, FaseFenologicaCorrenteDto[]>(
        '/DSSNutrizione/fasi-fenologiche-registrate',
        request,
        false,
        false,
        false
      )
      .pipe(
        map(resp => resp.RispostaOK ? (resp.RispostaStringa ?? []) : []),
      );
  }
}

