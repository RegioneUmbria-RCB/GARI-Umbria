import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_ID_Area_Alert, enum_ID_Area_Tipologia } from 'app/Model/TipiEnumerativi';
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from 'app/Model/siti.enum';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { map, Observable } from 'rxjs';

@Injectable()
export class AnalisiDocumentsService {

  constructor(
    private transloco: TranslocoService,
    private gestioneRichieste: GestioneRichiesteService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private windowService: GiasIFrameWindowService,
  ) { }

  public checkHasAtachedDocuments(analisiTestataCod: number, piva: string): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Documenti/EsistonoDocumentiAllegati', {
      Piva: piva, Analisi_Testata_Cod: analisiTestataCod
    }).pipe(map(R => R.RispostaOK ? R.RispostaStringa === "True" : false));
  }

  ApriKendoWindowRicercaDocumenti(analisiTestataCod: number, piva: string) {

    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create('type', 'doc'),
      // KeyValuePair.Create('area_provenienza', Id_Area.toString()),
      KeyValuePair.Create('p', piva),
      KeyValuePair.Create('area_provenienza', (enum_ID_Area_Alert.Analisi).toString()),
      KeyValuePair.Create('Analisi_Testata_Cod', analisiTestataCod.toString())
    ];

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
      parametri).then(link => {
        this.windowService.open({
          title: this.transloco.translate('RicercaDocumenti'),
          content: link,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        });
      });
  }

  ApriKendoWindowAggiungiNuovoAllegato(analisiTestataCod: number, validitaFine: Date, piva: string) {
    let ID_Alert_Entita = -1;
    let ID_Elenco = -1;
    let Modalita = 'doc';

    var scadstr = JSON.stringify({
      'Piva': piva,
      'Data_Scadenza_Analisi': validitaFine,
      'Id_Area': enum_ID_Area_Alert.Analisi,
      'Tipologia': enum_ID_Area_Tipologia.Analisi_terreno,
      'ID_Alert_Entita': ID_Alert_Entita,
      'ID_Elenco': ID_Elenco,
      'Analisi_Testata_Cod': analisiTestataCod,
      'area_provenienza': enum_ID_Area_Alert.Analisi,
      'sito_provenienza': Enum_SiteRedirector.GiasNG
    });

    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create('scadstr', scadstr),
      KeyValuePair.Create('type', Modalita),
      KeyValuePair.Create('p', piva)
    ];

    return this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica,
      parametri).then(link => {
        return this.windowService.open({
          title: this.transloco.translate('NuovoDocumento'),
          content: link,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        });
      });
  }
}