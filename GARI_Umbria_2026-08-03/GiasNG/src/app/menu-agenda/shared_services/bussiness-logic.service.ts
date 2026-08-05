import { ElementRef, Injectable, OnDestroy, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';
import { IntlService } from '@progress/kendo-angular-intl';
import { Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import { enum_PagineAgenda_2010, enum_PaginePianoConcimazione_2017, Enum_SiteRedirector } from 'app/Model/siti.enum';
import {
  GestioneRichiesteService,
  KeyValuePair,
  ParametriAggiuntivi_QueryString
} from 'app/Service/gestione-richieste.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GiasKendoGridComponent, KendoGridRow } from 'gias-kendo-grid';
import { FiltersService } from '../components/filters/filters.service';
import { Gias2010Redirector } from '../components/grid-qdc/Gias2010Redirector.service';
import { PulsanteGestioneCostiService } from '../components/grid-qdc/pulsanteGestioneCosti.service';
import {
  BloccaAttivitaAgendaLink,
  BrogliaccioRow,
  CopiaOperazioneSingola as CopiaOperazioneSingolaLink,
  CreateRecipeData,
  CreateRicettaLink,
  lav_cod_non_editabili,
  QdCRow,
  RibaltamentoTypes,
  RicettaRow,
  SbloccaAttivitaAgendaLink,
  TabTypes,
  ZooRow
} from '../components/utils';
import { CopiaOperazioneResult } from '../dtos/dtos';
import { MenuAgendaDataStore } from './menu-agenda-datastore.service';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { AgendaService, AggiungiRicettaAlPUA } from "../../Service/Agenda/Agenda.service";
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { enum_ID_Area_Alert, enum_LAVCOD, enum_PagineGiasNG, enum_Security_Attivita } from "../../Model/TipiEnumerativi";
import { ZooOperationGridFlatItem } from "../../zoo/models/zoo-operation-grid-item.model";
import { from, map, Observable, of, Subscription, take } from 'rxjs';
import { BloccaAttivitaAgenda } from "../../Service/api.service";
import { ObjParametriAgenda, Tipo_Attivita } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import {CAU_TRATTAMENTO} from "../../Model/CostantiPersonalizzate";

export class BloccaAttivitaAgendaImpl implements BloccaAttivitaAgenda {
  constructor(
    public piva: string,
    public sa_cod: number,
    public id_agenda: string,
    public lav_cod: number
  ) {
  }
}

@Injectable()
export class BussinessMenuAgendaService implements OnDestroy {
  /**
   * Use cases in the qdc grid.
   */
  gestioneCosti: PulsanteGestioneCostiService;
  masterService: any;
  Subs = new Subscription();

  private readonly _docContabili = [
    enum_LAVCOD.FATTURA_RICEVUTA, enum_LAVCOD.FATTURA_EMESSA,
    enum_LAVCOD.BOLLA_RICEVUTA, enum_LAVCOD.BOLLA_EMESSA,
    enum_LAVCOD.ACCETTAZIONE_DIVERSI, enum_LAVCOD.DISTINTA_CARICO_ACCETTAZIONE,
    enum_LAVCOD.AUTO_DDT_EMESSO_ACCETTAZIONE,
    enum_LAVCOD.ORDINE_VENDITA, enum_LAVCOD.ORDINE_ACQUISTO
  ];
  private readonly _carichiScarichi = [enum_LAVCOD.CARICO, enum_LAVCOD.SCARICO];

  constructor(
    private dataStore: MenuAgendaDataStore,
    private transloco: TranslocoService,
    private IntlService: IntlService,
    private master: MasterService,
    private filters$: FiltersService,
    private router: Router,
    private agenda: ObjParametriAgendaService,
    public notifications: GiasMessageService,
    private dialogService: GiasDialogService,
    private gestioneRichieste: GestioneRichiesteService,
    private windowService: GiasIFrameWindowService,
    private redirector: Gias2010Redirector,
    renderer: Renderer2,
    private AgendaService: AgendaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private permessiUtenteService: PermessiUtenteService
  ) {
    this.gestioneCosti = new PulsanteGestioneCostiService(
      {
        dataStore: dataStore,
        agenda: agenda,
        notifications: notifications,
        gestioneRichieste: gestioneRichieste,
        renderer: renderer,
        ajaxAgronicaAPIService: ajaxAgronicaAPIService,
        objParametriAgendaService: objParametriAgendaService
      }
    );

    this.tabRicetteVisibile = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Gest_Ricette);
  }

  tabRicetteVisibile: boolean = true;

  /** Gestione costi */
  public applicaStiliGestioneCosti(domElems: any, rows: QdCRow[], qdcGridRef: ElementRef) {
    this.gestioneCosti.applyStyle({
      rows: rows,
      domElems: domElems,
      qdcGridRef: qdcGridRef
    });
  }

  // Gestione stile singolo pulsante
  public applicaStilePulsanteCosti(dataItem: QdCRow) {
    return this.gestioneCosti.getCostiStyleClass(dataItem);
  }

  public vaiAiCosti(row: QdCRow) {
    this.gestioneCosti.vaiAiCosti(row);
  }

  /****/

  public OperazioneBloccata(dataItem: ZooRow) {
    return dataItem.blocco_flag === "1";
  }

  public lavcodNonEditabile(dataItem: ZooRow) {
    return lav_cod_non_editabili.indexOf(parseInt(dataItem.Lav_cod)) >= 0;
  }

  public SenzaPermessoDiModifica(dataItem: ZooRow) {
    return dataItem.PermessoModifica === "False";
  }

  preparaModalRicetta(selezionata: KendoGridRow, righe: KendoGridRow[]) {
    let result: CreateRecipeData = new CreateRecipeData();
    result.lblHtmlRicettaDaCreare = '';
    result.piva_ricetta = selezionata['Piva'];
    result.sa_cod = selezionata['sa_cod'];
    result.id_agenda = selezionata['id_agenda'];
    result.ricettaForm.descrizione = selezionata['Specie_Varieta'];
    result.ricettaForm.nota = '';
    let singolaSpecie = this.transloco.translate('ErroreSingolaSpecie', {});
    result.ErroreSingolaSpecie = singolaSpecie;
    let sacod_ricetta = result.sa_cod;
    let veg_cod = selezionata['Veg_cod'];
    let stesso_vegcod = true;
    let data: Date = selezionata['Data2'] as Date;
    let data_inizio = new Date(data);
    let data_fine = new Date(data);
    // Estraggo gli elementi selezionati
    //let arraySelected = this.elementiGrigliaSelezionati(righe);
    let arraySelected = righe as any[];
    // concateno gli elementi selezionati
    let id_agenda_checked = "";
    let riassunto_op = "";

    for (let iRow in arraySelected) {
      let ricettabile = this.dataStore.lav_cod_ricettabili.indexOf(arraySelected[iRow].Lav_cod) >= 0;
      if (!ricettabile || arraySelected[iRow].Ricetta_Cod !== "0")
        continue;

      if (arraySelected[iRow].Veg_cod !== veg_cod) stesso_vegcod = false;
      id_agenda_checked += arraySelected[iRow].id_agenda + ',';
      let datatmp = new Date(arraySelected[iRow].Data2);
      if (datatmp < data_inizio) data_inizio = datatmp;
      if (datatmp > data_fine) data_fine = datatmp;

      riassunto_op +=
        '<div class="row"><div class="col-lg-12">' + arraySelected[iRow].Data +
        ' - ' + arraySelected[iRow].Lav_Des + ' - ' + arraySelected[iRow].Specie_Varieta + '</div></div>';
      if (arraySelected[iRow].sa_cod !== 0 && arraySelected[iRow].sa_cod !== sacod_ricetta) {
        sacod_ricetta = 0;
      }
    }
    if (id_agenda_checked === "") {
      riassunto_op = '<div class="row"><div class="col-lg-12">' + selezionata['Data']
        + ' - ' + selezionata['Lav_Des'] + ' - ' + selezionata['Specie_Varieta'] + '</div></div>';
    }

    result.ricettaForm.dataInizio = data_inizio;
    result.ricettaForm.dataFine = data_fine;
    result.data_operazione = data_fine.toLocaleString();
    let ricettaInfo = this.transloco.translate('RicettaConterraOperazioni', {});
    if (riassunto_op !== '') {
      riassunto_op = '<div class="row"><div class="col-lg-12">' + ricettaInfo + '</div></div>' + riassunto_op;
    }

    result.lblHtmlRicettaDaCreare = riassunto_op;
    result.stesso_vag_cod = stesso_vegcod;
    this.Subs.add(from(this.AgendaService.Ricetta_Numero_Default(result.piva_ricetta, result.data_operazione)).pipe(take(1)).subscribe(r => {
      result.ricettaForm.numero = r;
      this.dataStore.recipeModalData.next(result);
    }));
  }

  elementiGrigliaSelezionati(righe: KendoGridRow[]) {
    //Estraggo gli elementi selezionati
    //let dati = datiGriglia.dataSource.data();
    let arraySelected = [];
    for (let i = 0; i < righe.length; i++) {
      if (righe[i]['Selected']) {
        arraySelected.push(righe[i]);
      }
    }
    return arraySelected;
  }

  inviaRicettaApp(ricetteList: RicettaRow[]): Promise<any> {
    return this.dataStore.InviaRicettaApp(ricetteList);
  }

  CreaRicetta(righe: KendoGridRow[]) {
    let form = this.dataStore.recipeModalData.getValue().ricettaForm;
    let ricetta_des = form.descrizione;
    let ricetta_numero = form.numero;
    let data_inizio = form.dataInizio;
    let data_fine = form.dataFine;
    let nota_des = form.nota;
    let id_agenda_checked = "";
    let veg_cod = 0;

    if (ricetta_des === '' || ricetta_numero === '') {
      let descrizioneNumObbl = this.transloco.translate('jsMsgInserireDescrizioneNum', {});
      this.dialogService.baseError('', descrizioneNumObbl, false)
    } else {
      ////Estraggo gli elementi selezionati
      let arraySelected = this.elementiGrigliaSelezionati(righe);
      //concateno gli elementi selezionati
      let modal = this.dataStore.recipeModalData.getValue();
      let id_agenda = modal.id_agenda;

      for (let iRow in arraySelected) {
        let found = this.dataStore.lav_cod_ricettabili.indexOf(arraySelected[iRow].Lav_cod) > 0;
        if (arraySelected[iRow].Ricetta_Cod === "0" && found) {

          id_agenda_checked += arraySelected[iRow].id_agenda + ',';
          veg_cod = arraySelected[iRow].Veg_cod;
        }
      }
      if (id_agenda_checked === "") {
        id_agenda_checked = id_agenda + ",";
      }
      id_agenda_checked += "-1";

      let ricetta = this.dataStore.recipeModalData.getValue();
      const params = getParams(data_inizio, data_fine,
        id_agenda_checked, ricetta.id_agenda, ricetta.piva_ricetta,
        ricetta.sa_cod, veg_cod, ricetta_des,
        ricetta_numero, nota_des
      );
      this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(CreateRicettaLink, params)
        .pipe(take(1)).subscribe(risp => {
          if (risp.RispostaOK === true) {
            this.dialogService.baseSuccess('', risp.RispostaStringa);
            this.dataStore.recipeModalData.next(null);
            this.filters$.applyFilters();
          } else {
            this.master.changeErrorMsgType({ show: true, msg: risp.Errore, errorNumber: 0 });
          }
        });
    }
  }

  public copiaOperazione(datiRiga: KendoGridRow, righe: KendoGridRow[]) {
    let chiave = datiRiga['chiave_composita'];
    let data = chiave.split("_")[0];
    let id_agenda = chiave.split("_")[1];
    let lav_cod = chiave.split("_")[2];
    let piva = chiave.split("_")[3];
    let sa_cod = chiave.split("_")[4];
    //Estraggo gli elementi selezionati
    let arraySelected = this.elementiGrigliaSelezionati(righe);
    //concateno gli elementi selezionati
    let id_agenda_checked = "";
    let lav_cod_checked = "";
    for (let iRow in arraySelected) {
      id_agenda_checked += arraySelected[iRow].id_agenda + ',';
      lav_cod_checked += arraySelected[iRow].Lav_cod + ',';
    }
    if (id_agenda_checked === "") {
      id_agenda_checked = id_agenda + ",";
    }
    if (lav_cod_checked === "") {
      lav_cod_checked = lav_cod + ",";
    }
    id_agenda_checked += "-1";
    lav_cod_checked += "-1";
    let params = {
      data: data,
      id_agenda_checked: id_agenda_checked,
      id_agenda: id_agenda,
      lav_cod_checked: lav_cod_checked,
      lav_cod: lav_cod,
      piva: piva,
      sa_cod: sa_cod
    }
    // TODO Razvan. Questa parte non è ancora stata testata.
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(CopiaOperazioneSingolaLink, params);
  }

  copiaCambiaSito(copiaOpRisp: CopiaOperazioneResult) {
    let objAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.agenda.getObjParamValue()));
    objAgenda.Data = this.IntlService.parseDate(copiaOpRisp.data);
    objAgenda.Id_Agenda = +copiaOpRisp.Id_Agenda;
    objAgenda.Lav_Cod = +copiaOpRisp.Lav_Cod;
    objAgenda.Piva = copiaOpRisp.Piva;
    objAgenda.Sa_Cod = +copiaOpRisp.Sa_Cod;
    objAgenda.RagSoc = copiaOpRisp.RagSoc;
    objAgenda.SaNome = copiaOpRisp.SaNome;
    const parametri: ParametriAggiuntivi_QueryString[] = [
      {
        key: "id",
        value: copiaOpRisp.QueryStringIdAgendaChecked,
        codifica: false
      }
    ]
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_DuplicaOperazione,
      parametri,
      objAgenda
    ).then(resp => window.location.href = resp);
  }

  creaNuovaOperazione(ricetta: Tipo_Ricetta) {
    const agenda = this.agenda.getObjParamValue();
    this.redirector.redirectToQdCfromMenuAgenda(0, 0,
      agenda.Piva, agenda.RagSoc,
      0, "", ricetta,
      Enum_DBTypeOperation.Write, this.dataStore.currentTab, new Date(), RibaltamentoTypes.Nessuno, agenda.IdSezione
    );
  }

  public bloccaAttivitaAgenda(attivita: QdCRow[]): Observable<boolean> {
    const params = attivita.map(att => new BloccaAttivitaAgendaImpl(
      att.Piva, +att.sa_cod, att.id_agenda, +att.Lav_cod
    ));
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(BloccaAttivitaAgendaLink, params)
      .pipe(map(R => {
        if (R.RispostaOK)
          this.dialogService.baseSuccess('', 'OperazioneBloccata');
        return R.RispostaOK
      }))
  }

  public bloccaAttivitaZoo(activities: ZooOperationGridFlatItem[]): Observable<boolean> {
    const res = activities.map(att => new BloccaAttivitaAgendaImpl(
      att.Piva, att.Sa_Cod, att.Id_Agenda.toString(), att.Lav_Cod
    ));
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(BloccaAttivitaAgendaLink, res, true)
      .pipe(map(R => {
        if (R.RispostaOK)
          this.dialogService.baseSuccess('', 'OperazioneBloccata');
        return R.RispostaOK
      }))
  }

  public sbloccaAttivitaAgenda(attivita: QdCRow[]): Observable<boolean> {
    const params = attivita.map(att => new BloccaAttivitaAgendaImpl(
      att.Piva, +att.sa_cod, att.id_agenda, +att.Lav_cod
    ));
    return this.ajaxAgronicaAPIService.ajaxAPIPost(SbloccaAttivitaAgendaLink, params, true)
      .pipe(map(R => {
        if (R.RispostaOK)
          this.dialogService.baseSuccess('', 'qdc.OperazioneSbloccata')
        return R.RispostaOK
      }))
  }

  public sbloccaAttivitaZoo(activities: ZooOperationGridFlatItem[]): Observable<boolean> {
    const res = activities.map(att => new BloccaAttivitaAgendaImpl(
      att.Piva, att.Sa_Cod, att.Id_Agenda.toString(), att.Lav_Cod
    ));
    return this.ajaxAgronicaAPIService.ajaxAPIPost(SbloccaAttivitaAgendaLink, res, true)
      .pipe(map(R => {
        if (R.RispostaOK)
          this.dialogService.baseSuccess('', 'qdc.OperazioneSbloccata')
        return R.RispostaOK
      }));
  }

  public async Cancella(gridComponent: GiasKendoGridComponent, row: KendoGridRow) {
    await gridComponent.gridDialogService.showDialog(
      gridComponent.privateService.remove.bind(gridComponent.privateService), row
    )
  }

  /* Pulsanti Documentale */

  public CheckAttachedDocumentsOperationsAndVisits(piva: string, idAgenda: number): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Documenti/EsistonoDocumentiAllegati', {
      Piva: piva, IdAgenda: idAgenda
    }).pipe(map(R => R.RispostaOK ? R.RispostaStringa === "True" : false));
  }

  public CheckAttachedDocumentsRecipes(piva: string, recipeCode: number): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Documenti/EsistonoDocumentiAllegati', {
      Piva: piva, RicettaCod: recipeCode
    }).pipe(map(R => R.RispostaOK ? R.RispostaStringa === "True" : false));
  }

  public ApriKendoWindowRicercaDocumenti(dataItem: any) {
    const params = parseGridRowForDocumentale(dataItem, this.dataStore.currentTab);
    const Id_Area = this.getIdArea(+params.Lav_cod);
    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create("type", "doc"),
      KeyValuePair.Create("p", params.Piva),
      KeyValuePair.Create("id_agenda", params.Id_Agenda),
      KeyValuePair.Create("Ricetta_Operazione_Cod", params.Ricetta_Operazione_Cod.toString()),
      KeyValuePair.Create("area_provenienza", Id_Area.toString())
    ];
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
      parametri
    ).then(link => this.windowService.open({
      title: this.transloco.translate("RicercaDocumenti"),
      content: link,
      height: window.innerHeight * 0.9,
      width: window.innerWidth * 0.9
    }));
  }

  public ApriKendoWindowAggiungiNuovoAllegato(dataItem: any) {
    const params = parseGridRowForDocumentale(dataItem, this.dataStore.currentTab);
    const ID_Alert_Entita = -1;
    const ID_Elenco = -1;
    const Modalita = "doc";
    const Id_Area = this.getIdArea(+params.Lav_cod);
    const Tipologia = 0; //this.getTipologia(+params.Lav_cod);

    const scadstr = JSON.stringify({
      'Piva': params.Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita,
      'Id_Area': Id_Area, 'Tipologia': Tipologia, 'area_provenienza': Id_Area,
      'Id_Agenda': params.Id_Agenda, 'Ricetta_Operazione_Cod': params.Ricetta_Operazione_Cod
    });
    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create("scadstr", scadstr),
      KeyValuePair.Create("type", Modalita),
      KeyValuePair.Create("p", params.Piva)
    ];

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica,
      parametri
    ).then(link => this.windowService.open({
      title: this.transloco.translate("NuovoDocumento"),
      content: link,
      height: window.innerHeight * 0.9,
      width: window.innerWidth * 0.9
    }));
  }

  public getDialogEliminaRicettaBrogliaccio(row: BrogliaccioRow | RicettaRow, showDialog: boolean = true) {
    let msg = "";
    if (!showDialog) {
      return of({ text: "", primary: false, returnObj: this.haveOnlySingleRecipes(row) });
    }

    if (this.tabRicetteVisibile){
      if (this.dataStore.currentTab === TabTypes.Ricette) {
        msg = this.getMessaggioRicetteCollegate(row as RicettaRow);
        if (row['Raccoglitore_Cod'] === '0' &&
          this.dataStore.gridDataRicette.rows.filter(r => r['Selected']).some(r => r['Raccoglitore_Cod'] !== '0')) {
          msg = this.transloco.translate('ConfermaRimozioneRicetteMisteSingole')
        } else if (row['Raccoglitore_Cod'] === '0') {
          msg = this.transloco.translate('ConfermaRimozioneRicetta')
        }
      } else if (this.dataStore.currentTab === TabTypes.Brogliaccio) {
        msg = this.transloco.translate('qdc.ConfermaRimozioneBrogliaccio', {});
        if (row['Raccoglitore_Cod'] !== '0') {
          msg += this.getMessaggioBrogliacciCollegati(row as BrogliaccioRow);
        }
      }
    } else {
      if (this.dataStore.currentTab === TabTypes.Ricette){ //se non si vedono le ricette i brogliacci saranno posizionati con tab = 1
        msg = this.transloco.translate('qdc.ConfermaRimozioneBrogliaccio', {});
        if (row['Raccoglitore_Cod'] !== '0') {
          msg += this.getMessaggioBrogliacciCollegati(row as BrogliaccioRow);
        }
      }
    }

    return from(this.dialogService.baseWarning(
      '', msg, false
    ));
  }

  public getMessaggioRicetteCollegate(row: RicettaRow) {
    let msg = '';
    if (row['Raccoglitore_Cod'] !== '0') {
      let siblings = this.dataStore.gridDataRicette.rows.filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod']);
      msg += this.transloco.translate('ConfermaRimozioneRicetta', {});
      msg += '<BR/>' + this.transloco.translate('AttenzioneRicetteCollegate', [siblings.length]) + '<BR/>';
      msg += siblings.map(r => '- ' + r['Descrizione_Unica'])
        .reduce((a, b) => a + '<BR/>' + b)
      msg += '<BR/><BR/>' + this.transloco.translate('EliminazioneRicettaMultiWarning');
      console.log(siblings)
    }
    return msg
  }

  public getMessaggioBrogliacciCollegati(row: BrogliaccioRow) {
    let msg = '';
    if (row['Raccoglitore_Cod'] !== '0') {
      let siblings = this.dataStore.gridDataBrogliaccio.rows.filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod']);
      msg += '<BR/>' + this.transloco.translate('qdc.AttenzioneBrogliacciCollegati', [siblings.length]) + '<BR/>';
      msg += siblings.map(r => '- ' + r['Descrizione_Unica'])
        .reduce((a, b) => a + '<BR/>' + b)
      msg += '<BR/><BR/>' + this.transloco.translate('qdc.EliminazioneBrogliaccioMultiWarning');
      console.log(siblings)
    }
    return msg;
  }

  private haveOnlySingleRecipes(row: BrogliaccioRow | RicettaRow): boolean {
    if (row instanceof RicettaRow)
      return row['Raccoglitore_Cod'] === '0' &&
        this.dataStore.gridDataRicette.rows.filter(r => r['Selected'])
          .every(r => r['Raccoglitore_Cod'] === '0')
    return row['Raccoglitore_Cod'] === '0'
  }

  Importa_nel_PUA(row: any) {
    let Ricetta_Cod = this.agenda.getObjParamValue().Ricetta_Cod;
    if (Ricetta_Cod > 0 && row) {
      let list_chiave_composita: Array<string> = row.chiave_composita.split("_");
      let data = list_chiave_composita[0];
      let id_agenda = list_chiave_composita[1];
      let lav_cod = list_chiave_composita[2];
      let piva = list_chiave_composita[3];
      let sa_cod = list_chiave_composita[4];
      //Aggiungo le eventuali altre righe da Importare nel PUA
      //concateno gli elementi selezionati
      let id_agenda_checked = "";
      let lav_cod_checked = "";
      let arraySelected = this.elementiGrigliaSelezionati(this.dataStore.gridDataQdc.rows);
      if (arraySelected && arraySelected.length > 0) {
        arraySelected.forEach(r => {
          id_agenda_checked += r.id_agenda + ',';
          lav_cod_checked += r.Lav_cod + ',';
        });
      }
      if (id_agenda_checked === "") {
        id_agenda_checked = id_agenda + ",";
      }
      if (lav_cod_checked === "") {
        lav_cod_checked = lav_cod + ",";
      }
      id_agenda_checked += "-1";
      lav_cod_checked += "-1";

      const Aggiungi: AggiungiRicettaAlPUA = {
        data: data,
        id_agenda: id_agenda,
        id_agenda_checked: id_agenda_checked,
        lav_cod_checked: lav_cod_checked,
        lav_cod: lav_cod,
        piva: piva,
        sa_cod: sa_cod,
        ricetta_cod: Ricetta_Cod.toString()
      }

      this.Subs.add(this.AgendaService.Aggiungi_Ricetta_Al_PUA_MenuAgenda(Aggiungi).subscribe(async r => {
        //Redirect al PUA
        if (r.RispostaOK) {
          let msg: string = this.transloco.translate("qdc.FertilizzazioneImportataNelPUA");
          if (arraySelected.length > 1) {
            msg = this.transloco.translate("qdc.FertilizzazioniImportateNelPUA", { NumFertilizzazioni: arraySelected.length });
          }
          await this.dialogService.baseSuccess('', msg);
          const objAgenda = this.agenda.getObjParamValue();
          objAgenda.Piva = Aggiungi.piva;
          objAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
          window.location.href = await this.gestioneRichieste.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_PianoConcimazione_2017,
            enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti,
            [], objAgenda
          );
        }
      }));
    }
  }

  Vai_Alla_Visita(row: any) {
    let objParam = this.objParametriAgendaService.resettaObjAgenda(this.objParametriAgendaService.getObjParamValue());
    objParam.Piva = this.objParametriAgendaService.getObjParamValue().Piva;
    objParam.RagSoc = this.objParametriAgendaService.getObjParamValue().RagSoc;
    objParam.Lav_Cod = enum_LAVCOD.VISITA;
    objParam.Id_Agenda = row.Id_Agenda_Visita;
    objParam.TipoOperazioneDB = Enum_DBTypeOperation.Update;
    objParam.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    objParam.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
    this.redirector.gestisciRedirectToQdC(objParam).then();
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

  private getIdArea(lavCod: enum_LAVCOD): enum_ID_Area_Alert {
    if (this._docContabili.includes(lavCod)) {
      return enum_ID_Area_Alert.Documenti_Contabili;
    } else if (this._carichiScarichi.includes(lavCod)) {
      return enum_ID_Area_Alert.Carichi_Scarichi
    }
    return enum_ID_Area_Alert.Operazioni_Campagna_QDC;
    ;
  }

  /*
  * @description Nascondo il pulsante di Modifica del Menù Agenda se:
  * - L'installazione della trappola è stata reinnescata
  * - Ho un Reinnesco Trappole nuovo con la categoria di Magazzino Formulati
  * */
  public NascondiModificaInstallazioneReinnescoTrappole(dataItem: KendoGridRow): boolean{
    return dataItem['Installazione_Trappola_Reinnescata'] === 1 ||
          (+dataItem['Lav_cod'] === enum_LAVCOD.REINNESCO_TRAPPOLE &&
            dataItem['Cau_Mov'] === CAU_TRATTAMENTO);
  }

}

function getParams(
  data_inizio: Date, data_fine: Date, id_agenda_checked: string, id_agenda: string,
  piva_ricetta: string, sa_cod: number, veg_cod: number, ricetta_des: string, ricetta_numero: string,
  nota_des: string
) {
  return {
    data_inizio: data_inizio,
    data_fine: data_fine,
    id_agenda_checked: id_agenda_checked,
    id_agenda: id_agenda,
    piva: piva_ricetta,
    sa_cod: sa_cod,
    veg_cod: veg_cod,
    ricetta_des: ricetta_des,
    ricetta_numero: ricetta_numero,
    nota_des: nota_des
  };
}

function parseGridRowForDocumentale(row, currTab: TabTypes): ApriDocumentaleParams {
  let Ricetta_Operazione_Cod: number;
  let Id_Agenda: string;
  let Piva: string;
  let lav_cod: string | number;
  if (currTab === TabTypes.QuadernoDiCampagna) {
    Piva = row.Piva;
    Id_Agenda = row.id_agenda ?? "0";
    Ricetta_Operazione_Cod = 0;
    lav_cod = row.Lav_cod;
  } else if (currTab === TabTypes.Brogliaccio) {
    Piva = row.piva;
    Id_Agenda = "0";
    Ricetta_Operazione_Cod = row.Ricetta_Operazione_Cod ?? 0;
    lav_cod = row.lav_cod;
  } else { // ricetta per forza
    let ricetta = row as RicettaRow;
    Piva = ricetta.piva;
    Id_Agenda = "0";
    Ricetta_Operazione_Cod = ricetta.Ricetta_Operazione_Cod ?? 0;
    lav_cod = ricetta.lav_cod;
  }
  return {
    Lav_cod: lav_cod,
    Piva: Piva,
    Id_Agenda: Id_Agenda,
    Ricetta_Operazione_Cod: Ricetta_Operazione_Cod
  };
}

interface ApriDocumentaleParams {
  Lav_cod: string | number; // todo
  Piva: string;
  Ricetta_Operazione_Cod: number;
  Id_Agenda: string; // todo
}
