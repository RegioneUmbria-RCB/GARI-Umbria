import { Injectable } from '@angular/core';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { GiasWindowsService } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { from, Observable } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { Contatto, PK } from 'app/Model/anagrafiche/Contatto';
import { ContattiAssociaUtenteComponent } from './contatti-associa-utente/contatti-associa-utente.component';
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class ContattiEventsService {

  objParametriAgenda: ObjParametriAgenda;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private windowService: GiasWindowsService,
    private APIservice: AjaxAgronicaAPIService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private contattiService: ContattiService
  ) { }

  public onTemplateBtnClick(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    if ((<string>dataItem.chiave).split('_')[0] !== this.objParametriAgenda.Piva) {
      //this.giasMessageService.warningMessage(this.translocoService.translate('ContattoCreatoDaAltraImpresa'));
      this.giasDialogService.baseError('', this.translocoService.translate('ContattoCreatoDaAltraImpresa'), false);
      return;
    }
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgenda.Cod_Contatto = dataItem.CF;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    let piva = (<string>dataItem.chiave).split('_')[0];
    let cod_contatto = (<string>dataItem.chiave).split('_')[2];
    let operazione = 2;
    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New,
      [
        { key: 'codcont', value: cod_contatto, codifica: true },
        { key: 'piva', value: piva, codifica: true },
        { key: 'o', value: operazione.toString(), codifica: true }
      ]).then((val) => {
        window.location.href = val;
      });
  }

  public onNuovo() {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgenda.Cod_Contatto = '';
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New).then((val) => {
      window.location.href = val;
    });
  }

  public info(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgenda.Cod_Contatto = dataItem.CF;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    let piva = (<string>dataItem.chiave).split('_')[0];
    let cod_contatto = (<string>dataItem.chiave).split('_')[2];
    let operazione = 0;
    this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New,
      [
        { key: 'codcont', value: cod_contatto, codifica: true },
        { key: 'piva', value: piva, codifica: true },
        { key: 'o', value: operazione.toString(), codifica: true }
      ]).then((val) => {
        window.location.href = val;
      });
    //this.gestionePassaggioPaginaContatto((<string>dataItem.chiave).split('_')[0], (<string>dataItem.chiave).split('_')[2], 0).subscribe((val) => window.location.href = val);
  }

  public bindUser(dataItem) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    if ((<string>dataItem.chiave).split('_')[0] !== this.objParametriAgenda.Piva) {
      //this.giasMessageService.warningMessage(this.translocoService.translate('ContattoCreatoDaAltraImpresa'));
      this.giasDialogService.baseError('', this.translocoService.translate('ContattoCreatoDaAltraImpresa'), false);
      return;
    }
    this.contattiService.setUtenteDaAssociare(dataItem);
    this.contattiService.setWindowAssociaUtenteRef(
      this.windowService.open({
        title: this.translocoService.translate('AssociaUtente'),
        content: ContattiAssociaUtenteComponent,
        autoFocusedElement: '#associautentecomponentfocus',
        width: window.innerWidth / 2.5
      })
    )
  }

  /**
   *
   * @param contatto dataItem contenente i dettagli del contatto da eliminare,
   * fare riferimento agli oggetti usati in ContattiHttpService
   */
  public remove(contatto) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    if ((<string>contatto.chiave).split('_')[0] !== this.objParametriAgenda.Piva) {
      //this.giasMessageService.warningMessage(this.translocoService.translate('ContattoCreatoDaAltraImpresa'));
      this.giasDialogService.baseError('', this.translocoService.translate('ContattoCreatoDaAltraImpresa'), false);
      //return new Promise((resolve, reject) => { resolve(null) });
      return Promise.resolve(null);
    }
    //this.giasMessageService.warningMessage('WIP!');
    return this.eliminaContattoConDialog(contatto);
  }

  public gestionePassaggioPaginaContatto(piva: string, cod_contatto: string, operazione: number): Observable<string> {
    return from(this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New,
      [
        { key: 'codcont', value: cod_contatto, codifica: true },
        { key: 'piva', value: piva, codifica: true },
        { key: 'o', value: operazione.toString(), codifica: true }
      ]
    ));
  }

  private eliminaContattoConDialog(contatto: any) {
    return new Promise((resolve, reject) => {
      this.giasDialogService.baseWarning(
        '', this.translocoService.translate('ConfermaRimozioneContatto')
      ).then((resp: DialogResult) => {
        if (!resp['returnObj']) return;
        this.eliminaContatto(contatto)
          .then(isDeleted => resolve(isDeleted));
      });
    });
  }

  private eliminaContatto(item: any): Promise<boolean> {
    // chiave di tipo piva, sa_cod, cf
    let chiave: string[] = (item.chiave as string).split('_');

    const contatto = new Contatto();
    contatto.primaryKey = new PK(chiave[0], item.CF);
    contatto.sa_cod = parseInt(item.Sa_Cod, 10);
    contatto.fe_Tipologia_Contatto = item.Tipo_Contatto;

    return new Promise((resolve, reject) => {
      this.APIservice.ajaxAPIPost<Contatto, any>(
        'AnagraficaNG/EliminaContatto', contatto, false, true, false
      ).GiasSubscribe(R => {
        if (!R.RispostaOK) this.giasDialogService.baseError(
          this.translocoService.translate('ImpossibileCompletareOperazione'),
          R.RispostaStringa, false
        )
        else this.giasDialogService.baseSuccess(
          '', 'EliminazioneAvvenutaConSuccesso', true
        )

        resolve(R.RispostaOK === true);
      });
    });
  }

}
