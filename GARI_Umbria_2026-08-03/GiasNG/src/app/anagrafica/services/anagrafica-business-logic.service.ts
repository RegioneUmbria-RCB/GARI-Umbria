import { Injectable } from '@angular/core';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from '../../Service/gestione-richieste.service';
import { TranslocoService } from '@jsverse/transloco';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from '../../Model/siti.enum';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { enum_ID_Area_Alert } from '../../Model/TipiEnumerativi';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { map, Observable } from 'rxjs';

interface IItemParams {
  piva: string;
  Mac_Cod?: number;
}

@Injectable()
export class AnagraficaBusinessLogicService {
  public readonly commands = {
    RICERCA_DOCUMENTI: 0,
    NUOVO_ALLEGATO: 1
  };

  constructor(
    private gestioneRichieste: GestioneRichiesteService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private windowService: GiasIFrameWindowService,
    private transloco: TranslocoService
  ) { }

  public CheckAttachedDocumentsImprese(piva: string): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Documenti/EsistonoDocumentiAllegati', {
      Piva: piva
    }).pipe(map(R => R.RispostaOK ? R.RispostaStringa === "True" : false));
  }
  
  public CheckAttachedDocumentsMacchine(piva: string, macCod: number): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Documenti/EsistonoDocumentiAllegati', {
      Piva: piva, MacCod: macCod
    }).pipe(map(R => R.RispostaOK ? R.RispostaStringa === "True" : false));
  }
    

  public apriKWindowImprese(dataItem: any, command: number) {
    const noCategory = <enum_ID_Area_Alert>0;
    const itemParams = { piva: dataItem.piva };
    if (command === this.commands.RICERCA_DOCUMENTI) {
      this.openDocSearch(itemParams, noCategory);
    } else if (command === this.commands.NUOVO_ALLEGATO) {
      this.openNewAttachment(itemParams, noCategory);
    }
  }

  public apriKWindowMacchine(dataItem: any, command: number) {
    const itemParams = { piva: dataItem.Piva, Mac_Cod: dataItem.Mac_Cod };
    if (command === this.commands.RICERCA_DOCUMENTI) {
      this.openDocSearch(itemParams, enum_ID_Area_Alert.Macchine);
    } else if (command === this.commands.NUOVO_ALLEGATO) {
      this.openNewAttachment(itemParams, enum_ID_Area_Alert.Macchine);
    }
  }

  private openDocSearch(dataItem: IItemParams, cat: enum_ID_Area_Alert) {
    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create('type', 'doc'),
      KeyValuePair.Create('area_provenienza', cat.toString()),
      KeyValuePair.Create('p', dataItem.piva),
    ];
    if (dataItem.Mac_Cod) {
      parametri.push(KeyValuePair.Create('Mac_Cod', dataItem.Mac_Cod.toString()));
    }
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
      parametri
    ).then(link => this.windowService.open({
      title: this.transloco.translate('RicercaDocumenti'),
      content: link,
      height: window.innerHeight * 0.9,
      width: window.innerWidth * 0.9
    }));
  }

  private openNewAttachment(dataItem: IItemParams, cat: enum_ID_Area_Alert) {
    const ID_Elenco = -1;
    const scadstr = JSON.stringify({
      'Piva': dataItem.piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Elenco,
      'Id_Area': cat, 'area_provenienza': cat, 'Mac_Cod': dataItem.Mac_Cod ?? ''
    });
    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create('scadstr', scadstr),
      KeyValuePair.Create('type', 'doc'),
      KeyValuePair.Create('p', dataItem.piva)
    ];
    if (dataItem.Mac_Cod) {
      parametri.push(KeyValuePair.Create('Mac_Cod', dataItem.Mac_Cod.toString()));
    }
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica,
      parametri).then(link => {
        this.windowService.open({
          title: this.transloco.translate('NuovoDocumento'),
          content: link,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        });
      });
  }

}
