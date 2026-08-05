import {Inject, Injectable} from '@angular/core';
import {HttpAction} from 'gias-kendo-grid';
import {Observable, of, switchMap} from 'rxjs';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {TitoloDiPossesso} from 'app/Model/metaschema/TitoloDiPossesso';
import {ActivatedRoute, Router} from '@angular/router';
import {ParticelleCatastali} from 'app/Model/anagrafiche/ParticelleCatastali';
import {AGRODATAFINE, AGRODATAINIZIO} from 'app/Model/CostantiPersonalizzate';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {CatastoCentroAziendale} from 'app/Model/anagrafiche/CatastoCentroAziendale';
import {CentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {PossessoParticella} from 'app/Model/anagrafiche/PossessoParticella';
import {toInteger} from 'lodash';
import {IntervalloTemporale} from 'app/Model/anagrafiche/IntervalloTemporale';
import {DeleteMessageService} from 'app/Service/delete-message.service';
import {GiasMessageService} from 'app/Service/gias-message.service';
import {Impresa} from 'app/Model/anagrafiche/Impresa';
import {InvestimentoCatastaleComponent} from './investimento-catastale/investimento-catastale.component';
import {Appezzamento} from 'app/Model/anagrafiche/Appezzamento';
import {EditEvent} from '@progress/kendo-angular-grid';
import { GiasWindowsService } from 'gias-ui-kit';
import {CATASTO_SERVICE_TOKEN, CatastoFactoryService} from 'app/Service/ServiceFactory/catasto.factory.service';
import {TipoInvestimento} from 'app/Service/ServiceFactory/investimento-catastale.factory.service';
import {InvestimentoCatastaleFiltriService} from './investimento-catastale/investimento-catastale-filtri.service';
import {TranslocoService} from '@jsverse/transloco';
import {GiasDialogService} from '../../Service/gias-dialog.service';
import {GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString} from '../../Service/gestione-richieste.service';
import {enum_PagineAgenda_2010, Enum_SiteRedirector} from '../../Model/siti.enum';
import {CatastoCustomOperations} from './catasto-grid.service';
import {GiasIFrameWindowService} from 'gias-ui-kit';
import {NumToStr} from '../../Service/utils';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class CatastoGridEventsService {
  objParametriAgenda: ObjParametriAgenda;
  stringaRis: string;
  private catastiSelezionati: Map<CatastoCentroAziendale, string> = new Map<CatastoCentroAziendale, string>();

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService,
    private router: Router,
    private route: ActivatedRoute,
    private deleteMessageService: DeleteMessageService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private windowService: GiasWindowsService,
    private GiasIFrameWindowService: GiasIFrameWindowService,
    private investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService,
    private translocoService: TranslocoService,
    private gestioneRichieste: GestioneRichiesteService
  ) {
    this.stringaRis = "";
  }

  perform(actionType: HttpAction, item: any): Observable<any> {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.prepareParameters(this.objParametriAgenda);
    if (actionType != HttpAction.REMOVE && !this.validateParticella(item)){
      return of(false);
    }
    const part = this.preparePKPart(item);
    let particella: CatastoCentroAziendale;
    return this.catastoService.LeggiParticellaAzienda(part).pipe(
      switchMap((data) => {
        particella = data;
        let newValue = this.prepareParticella(particella, item, actionType);
        return this.catastoService.ScriviParticellaAzienda(null, newValue);
      }),
      switchMap((res) => {
        let cat = res.RispostaStringa
        if (cat){
          if (cat.flag_cancellazione || actionType == HttpAction.REMOVE) {
            if (res.RispostaOK) {
              this.addCatastoSelezionato(item, '');
            } else {
              this.addCatastoSelezionato(item, res.ErroriGias[0].messaggio);
            }
          } else {
            const deletePossessi = particella.possessiParticella.filter((el) => el.flag_cancellazione == true);
            if (deletePossessi.length > 0) {
              if (res.RispostaOK) {
                this.deleteMessageService.catastoDeleteMsg_Succ(cat);
                this.addCatastoSelezionato(item, '');
              } else {
                this.deleteMessageService.catastoDeleteMsg_Fail(cat);
                this.addCatastoSelezionato(item, res.ErroriGias[0].messaggio);
              }
            }
          }
        } else {
          return of(false);
        }
        return of([cat]);
      })
    );
  }

  validateParticella(item: any): boolean{
    return true;
  }

  public preparePKPart(item: any) {
    const particella = new CatastoCentroAziendale();

    if (item.chiave != undefined) {
      particella.centro = new CentroAziendale.PK((item.chiave.split('_')[1]), item.chiave.split('_')[0]);

      particella.particella = new ParticelleCatastali();
      particella.particella.primaryKey = new ParticelleCatastali.PK(
        item.chiave.split('_')[2],
        item.chiave.split('_')[3],
        item.chiave.split('_')[4],
        item.chiave.split('_')[5],
        item.chiave.split('_')[6],
        item.chiave.split('_')[7],
      );
    } else {
      particella.centro = new CentroAziendale.PK(toInteger(item.Sa_Cod), this.objParametriAgenda.Piva);
      particella.particella = new ParticelleCatastali();
      particella.particella.primaryKey = new ParticelleCatastali.PK(
        item.Prov,
        item.Com,
        item.SEZIONE,
        item.FOGLIO,
        item.NUMERO,
        item.SUBALTERNO
      );
    }

    return particella;
  }

  private preparePossesso(item: any) {
    let possessoParticella = new PossessoParticella();
    possessoParticella.codice = (item.Codice == null ? 0 : item.Codice);
    possessoParticella.validita = new IntervalloTemporale(item.Validita_Inizio, item.Validita_Fine);
    possessoParticella.titolo_Di_Possesso = new TitoloDiPossesso(toInteger(item.Titolo_Possesso_Cod));
    possessoParticella.Area = item.Sup_Condotta;
    possessoParticella.codice_particella = item.cod_particella;
    return possessoParticella;
  }

  private prepareParticella(oldValue: CatastoCentroAziendale, item: any, actionType: HttpAction): CatastoCentroAziendale {
    const particella = oldValue;
    let possessoParticella = this.preparePossesso(item);

    this.initCatastoCentroAziendale(particella, item);

    particella.particella.Area = item.Sup_Catastale;
    particella.particella.proprietario = item.Proprietario;
    let res = true;
    for (let pi = 0; pi < particella.possessiParticella.length; pi++) {
      let poss = particella.possessiParticella[pi];
      if (poss.codice == possessoParticella.codice && actionType == HttpAction.REMOVE) {
        poss.flag_cancellazione = true;
      } else if (poss.codice != possessoParticella.codice) {
        if (((poss.validita.inizio >= possessoParticella.validita.inizio && poss.validita.inizio <= possessoParticella.validita.fine) ||
            (poss.validita.fine >= possessoParticella.validita.inizio && poss.validita.fine <= possessoParticella.validita.fine)) &&
          (actionType != HttpAction.REMOVE)
        ) {
          this.giasMessageService.errorMessage(this.translocoService.translate('DatePossessiNonCoerentiSovrapposte'));
          res = false;
          return null;
        }
      }
    }

    if (actionType == HttpAction.UPDATE || actionType == HttpAction.CREATE) {
      let index = particella.possessiParticella.findIndex((el) => el.codice == possessoParticella.codice);
      if (index < 0) {
        particella.possessiParticella.push(possessoParticella);
      } else {
        particella.possessiParticella[index] = possessoParticella;
      }
      if (oldValue.particella.metodoProduzione?.length == 0) {
        if (item.MetodoProduzione_Cod) {
          oldValue.particella.metodoProduzione = [{
            metodoProduzione: {codice: item.MetodoProduzione_Cod, descrizione: item.MetodoProduzione_Des},
            validita: new IntervalloTemporale()
          }];
        } else {
          oldValue.particella.metodoProduzione = [{
            metodoProduzione: {codice: 0, descrizione: ''},
            validita: new IntervalloTemporale()
          }];
        }

      } else if (oldValue.particella.metodoProduzione?.length == 1) {
        oldValue.particella.metodoProduzione[0].metodoProduzione.codice = item.MetodoProduzione_Cod;
        oldValue.particella.metodoProduzione[0].metodoProduzione.descrizione = item.MetodoProduzione_Des;
      } else if (oldValue.particella.metodoProduzione == undefined) {
        oldValue.particella.metodoProduzione = [{
          metodoProduzione: {
            codice: item.MetodoProduzione_Cod,
            descrizione: item.MetodoProduzione_Des
          },
          validita: new IntervalloTemporale()
        }];
      }
    } else {
      particella.flag_cancellazione = true;
      // if (particella.possessiParticella.length <= 1) {
      //   particella.flag_cancellazione = true;
      // }
    }

    return (res ? particella : null);
  }

  private initCatastoCentroAziendale(particella: CatastoCentroAziendale, item: any) {
    if (particella.centro == undefined) {
      particella.centro = new CentroAziendale.PK(item.Sa_Cod, this.objParametriAgenda.Piva);
    }
    if (particella.particella == undefined) {
      particella.particella = new ParticelleCatastali();
      particella.particella.primaryKey = new ParticelleCatastali.PK(item.Prov, item.Com, item.SEZIONE, item.FOGLIO, item.NUMERO, item.SUBALTERNO);
    }
    if (particella.possessiParticella == undefined) {
      particella.possessiParticella = new Array<PossessoParticella>();
    }
  }

  private prepareParameters(agenda: ObjParametriAgenda) {
    agenda.Validita_Inizio = AGRODATAINIZIO;
    agenda.Validita_Fine = AGRODATAFINE;
    agenda.Data = AGRODATAINIZIO;
    return agenda;
  }

  onTemplateBtnClick(dataItem: any): void {
    const catastoEdit = this.catastoService.getParticellaEdit();
    catastoEdit.centro = {partitaIva: dataItem.Piva, codice: dataItem.Sa_Cod};
    catastoEdit.particella = new ParticelleCatastali();
    catastoEdit.particella.primaryKey = {
      Prov: dataItem.Prov,
      Com: dataItem.Com,
      Sezione: dataItem.SEZIONE,
      Foglio: dataItem.FOGLIO,
      Numero: dataItem.NUMERO,
      Subalterno: dataItem.SUBALTERNO
    };
    this.catastoService.changeParticellaEdit(catastoEdit);
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    this.router.navigate(['Catasto-Edit'], {relativeTo: this.route});
  }

  infoCatasto(event: EditEvent) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const dataItem = event.dataItem;
    const catastoEdit = this.catastoService.getParticellaEdit();
    catastoEdit.centro = {partitaIva: dataItem.Piva, codice: dataItem.Sa_Cod};
    catastoEdit.particella = new ParticelleCatastali();
    catastoEdit.particella.primaryKey =
      {
        Prov: dataItem.Prov,
        Com: dataItem.Com,
        Sezione: dataItem.SEZIONE,
        Foglio: dataItem.FOGLIO,
        Numero: dataItem.NUMERO,
        Subalterno: dataItem.SUBALTERNO
      };
    this.catastoService.changeParticellaEdit(catastoEdit);
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    // this.router.navigate(['/Anagrafica/Catasto/Catasto-Edit']);
    this.router.navigate(['Catasto-Edit'], {relativeTo: this.route});
  }

  onInvestimentoCatastale(dataItem: any): void {
    let impresa: Impresa = new Impresa;
    impresa.partitaIva = dataItem.Piva;

    let centro = new CentroAziendale({partitaIva: impresa.partitaIva, codice: parseInt(dataItem.Sa_Cod)});

    let particella = new ParticelleCatastali();
    particella.primaryKey = {
      Prov: dataItem.Prov,
      Com: dataItem.Com,
      Sezione: dataItem.SEZIONE,
      Foglio: dataItem.FOGLIO,
      Numero: dataItem.NUMERO,
      Subalterno: dataItem.SUBALTERNO,
    };

    let impianto: Impianto = null;

    this.investimentoCatastaleFiltriService.filtri = {
      impresa: impresa,
      centro: centro,
      particella: particella,
      impianto: impianto,
      data: AGRODATAINIZIO
    };

    this.investimentoCatastaleFiltriService.tipo = TipoInvestimento.Impianti;

    this.windowService.open({
      title: this.translocoService.translate('InvestimentoCatastale') + ' ( ' + this.translocoService.translate('ProvinciaAbbr') + ':' + dataItem.Prov + ' ' + this.translocoService.translate('ComuneAbbr') + ':' + dataItem.Com + ' ' + this.translocoService.translate('Sezione') + ':' + dataItem.SEZIONE + ' ' + this.translocoService.translate('Foglio') + ':' + dataItem.FOGLIO + ' ' + this.translocoService.translate('Numero') + ':' + dataItem.NUMERO + ' ' + this.translocoService.translate('Subalterno') + ':' + dataItem.SUBALTERNO + ')',
      content: InvestimentoCatastaleComponent,
      //top: 117,
      left: 10,
      width: window.innerWidth - 30,
      autoFocusedElement: '#investimentocatastalecomponentfocus'
    });
  }

  addCatastoSelezionato(catastoCentro: CatastoCentroAziendale, error: string) {
    this.catastiSelezionati.set(catastoCentro, error);
  }

  removeCatastoSelezionato(catastoCentro: CatastoCentroAziendale) {
    this.catastiSelezionati.delete(catastoCentro);
  }

  getCatastiSelezionati() {
    return this.catastiSelezionati;
  }

  clearSelezionati(): void {
    this.catastiSelezionati = new Map<CatastoCentroAziendale, string>();
  }

  msgWarningDelete() {
    if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Delete) {
      let msgFailure = this.translocoService.translate('ErroreCancellazioneCatasti');
      let msgSuccess = this.translocoService.translate('SuccessoCancellazioneCatasti');
      let displaySuccess = false;
      let displayFailure = false;
      this.getCatastiSelezionati().forEach((error, cat) => {
        if (error == '') {
          msgSuccess = msgSuccess.concat('\n' + cat['Sa_Nome'] + ' ' + cat['PROVINCIA'] + ' ' + cat['COMUNE'] + ' ' + cat['FOGLIO'] + ' ' + cat['NUMERO']);
          displaySuccess = true;
        } else {
          msgFailure = msgFailure.concat('\n' + cat['Sa_Nome'] + ' ' + cat['PROVINCIA'] + ' ' + cat['COMUNE'] + ' ' + cat['FOGLIO'] + ' ' + cat['NUMERO'] + ': ' + error);
          displayFailure = true;
        }
      });
      if (displaySuccess) {
        this.giasMessageService.successMessage(msgSuccess);
      }
      if (displayFailure) {
        this.giasDialogService.baseError('', msgFailure);
        //this.giasMessageService.warningMessage(msgFailure);
      }
    }
    this.clearSelezionati();
  }

  public apriKendoWindowPaginaScadenziario(dataItem: any, btn: CatastoCustomOperations) {
    let ID_Alert_Entita: number = -1;
    let ID_Elenco: number = -1;
    let Modalita: string = 'doc';
    let Id_Area: number = 12;
    let Tipologia: number;
    let pageToCall: enum_PagineAgenda_2010;

    let args = this.getPageToCallAndTipologia(btn);
    Tipologia = args.Tipologia;
    pageToCall = args.pageToCall;

    const params: ParametriAggiuntivi_QueryString[] = this.getParamsScadPage(
      ID_Alert_Entita,
      ID_Elenco,
      Modalita,
      Id_Area,
      Tipologia,
      dataItem,
      btn
    );

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      pageToCall,
      params
    ).then(link => {
      this.GiasIFrameWindowService.open({
        title: this.translocoService.translate('Scadenziario'),
        content: link,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9
      });
    });
  }

  private getPageToCallAndTipologia(btn: CatastoCustomOperations): any {
    let _Tipologia: number = -1;
    let _pageToCall: enum_PagineAgenda_2010 = undefined;

    switch (btn) {
      case CatastoCustomOperations.ContrattoDiAffitto:
        _Tipologia = -26;
        _pageToCall = enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica;
        break;
      case CatastoCustomOperations.VisuraCatastale:
        _Tipologia = 15;
        _pageToCall = enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica;
        break;
      case CatastoCustomOperations.MappaCatastale:
        _Tipologia = 16;
        _pageToCall = enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica;
        break;
      case CatastoCustomOperations.NuovoAllegato:
        _Tipologia = -1;
        _pageToCall = enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica;
        break;
      case CatastoCustomOperations.VisioneContrattoDiAffitto:
      case CatastoCustomOperations.VisioneVisuraCatastale:
      case CatastoCustomOperations.VisioneMappaCatastale:
        _Tipologia = 0;
        _pageToCall = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista;
        break;
      default:
        throw new Error('Nessuna operazione impostata!');
    }

    return {pageToCall: _pageToCall, Tipologia: _Tipologia};
  }

  private getParamsScadPage(
    ID_Alert_Entita: number,
    ID_Elenco: number,
    Modalita: string,
    Id_Area: number,
    Tipologia: number,
    dataItem: any,
    btn: CatastoCustomOperations
  ): ParametriAggiuntivi_QueryString[] {
    let params: ParametriAggiuntivi_QueryString[] = [];

    switch (btn) {
      case CatastoCustomOperations.NuovoAllegato:
      case CatastoCustomOperations.ContrattoDiAffitto:
      case CatastoCustomOperations.VisuraCatastale:
      case CatastoCustomOperations.MappaCatastale:
        let scadstr: string = JSON.stringify({
          'Piva': dataItem.Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita,
          'Id_Area': Id_Area, 'Tipologia': Tipologia, 'area_provenienza': Id_Area,
          'Id_Agenda': 0, 'Ricetta_Operazione_Cod': 0, 'Part_Cod': dataItem.part_cod
        });

        params = [
          KeyValuePair.Create('scadstr', scadstr),
          KeyValuePair.Create('type', Modalita),
          KeyValuePair.Create('p', dataItem.Piva),
          KeyValuePair.Create('Part_Cod', dataItem.part_cod)
        ];
        break;
      case CatastoCustomOperations.VisioneContrattoDiAffitto:
      case CatastoCustomOperations.VisioneVisuraCatastale:
      case CatastoCustomOperations.VisioneMappaCatastale:
        params = [
          KeyValuePair.Create('area_provenienza', NumToStr(Id_Area)),
          KeyValuePair.Create('type', Modalita),
          KeyValuePair.Create('p', dataItem.Piva),
          KeyValuePair.Create('nuovo', 'true'),
          KeyValuePair.Create('tipologia_provenienza', NumToStr(16)),
          KeyValuePair.Create('Part_Cod', dataItem.part_cod)
        ];
        break;
      default:
        throw new Error('Nessuna operazione impostata!');
    }

    return params;
  }
}
