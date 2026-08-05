import { EventEmitter, Injectable } from "@angular/core";
import { Utente_Impostazioni } from "../../../Model/utente/utente_impostazioni";
import { AjaxAgronicaAPIService } from "../../../Service/ajax-agronica.api.service";
import { BudgetService } from "../../../Service/Budget/budget.service";
import { Imprese_Impostazioni, MasterService, rispostaStandard } from "../../../Service/master.service";
import { PermessiUtenteService } from "../../../Service/permessi-utente.service";
import { cloneDeep } from "lodash";
import { BehaviorSubject, map, Observable, of, switchMap, take, tap } from "rxjs";
import { AziendaCentro } from "../../impostazioni-utente/impostazioni.model";
import { GuidaValoreImpostazione, Impostazione } from 'gias-ui-kit';
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { ImpresaDto } from "../../../Service/api.service";
import { CopiaImpostazioniObj } from "./impostazioni-utenti.service";
import { TranslocoService } from "@jsverse/transloco";


export class LeggiImpostazioni_AziendeCentri {
  constructor(
    public impostazioni: number[],
    public imprese: ImpresaDTO[]
  ) {
  }
}

export class ImpresaDTO implements ImpresaDto {
  constructor(
    public piva?: string,
    public Sa_Cod?: number,
    public rag_soc?: string,
    public Sa_Nome?: string
  ) {
  }
}

@Injectable({
  providedIn: 'root'
})
export class ImpostazioniAziendeCentriService {
  public modeChangeEvent: EventEmitter<{ type: string, value: any }> = new EventEmitter();
  public gridRemoveRow: EventEmitter<{ gridId: string, items: any[] }> = new EventEmitter();

  public imprese_Impostazioni: Imprese_Impostazioni[] = null;
  private impreseImpostazioni$ = new BehaviorSubject(this.imprese_Impostazioni);

  public imprese: AziendaCentro[] = [];
  public ImpresexCentri: AziendaCentro[] = [];
  public impreseInModifica: { impresa: AziendaCentro, impostazioni: Impostazione[] }[] = [];

  public selectionChange$ = new BehaviorSubject([]);
  private _impreseSelezionate = [];
  public set impreseSelezionate(selected: any[] | ImpresaDto[]) {
    this._impreseSelezionate = selected;
    this.selectionChange$.next(selected);
  }

  public get impreseSelezionate(): any[] {
    return this._impreseSelezionate;
  }

  private toSave = { impostazioni: [], imprese: [], hasToShowDialog: false };

  constructor(
    public masterService: MasterService,
    private dialogService: GiasDialogService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private premessiutenteService: PermessiUtenteService,
    private budgetService: BudgetService,
    private transloco: TranslocoService
  ) {
  }

  public leggiSezioniImpostazioniAziendeCentri(): Observable<Impostazione[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<string, any>(
      "AgronicaCoreUtentiBIZ/LeggiSezioniImpostazioni_AziendeCentri", ""
    ).pipe(
      map((data) => data.RispostaStringa),
      map((impostazioni: Impostazione[]) => {
        let imps = impostazioni;
        imps.forEach(i => {
          i.Sezione_Cod = 25;
          i.Sezione_Des = this.transloco.translate("prof.ImpostazioniImprese");
        })
        return this.filterVisibleSettings(imps);
      })
    );
  }

  private filterVisibleSettings(impostazioni: Impostazione[]) {
    if (this.impreseSelezionate.every(i => i.Sa_Cod === 0)) {
      impostazioni = impostazioni.filter(i => i.Impostazione_Azienda == 1);
    } else {
      impostazioni = impostazioni.filter(i => i.Impostazione_AziendaCentro == 1);
    }
    // if (!this.masterService.isSuperuser())
    //   impostazioni = impostazioni.filter(i => !i.Impostazione_SuperUser)
    return impostazioni;
  }

  leggiDatiImpostazioniAziendeCentri(codiciImpostazioni: number[]) {
    const imprese = this.impreseSelezionate.map(i => new ImpresaDTO(i.piva, i.Sa_Cod));
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiImpostazioni_AziendeCentri, any>(
      "AgronicaCoreUtentiBIZ/LeggiValoriImpostazioni_AziendeCentri",
      new LeggiImpostazioni_AziendeCentri(codiciImpostazioni, imprese)
    ).pipe(map((R) => {
      for (let r of R.RispostaStringa) {
        r.data = r.data.map(d => new GuidaValoreImpostazione(
          d.codice, d.descrizione, d.Valore, d.Tipo_campo, d.Note
        ));
      }
      return R.RispostaStringa;
    }));
  }

  public leggiImpreseImpostazioni(imprese: ImpresaDTO[] = [], impostazioni: number[] = []): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiImpostazioni_AziendeCentri, any[]>(
      "AgronicaCoreUtentiBIZ/LeggiImpostazioneImpresaCentro",
      new LeggiImpostazioni_AziendeCentri(impostazioni, imprese)
    ).pipe(take(1), map(R => R.RispostaOK ? R.RispostaStringa : []));
  }

  /** Salva le impostazioi in riferimento alle imprese presenti nella variabile toSave */
  salvaImpostazioniAziendeCentri(impostazioni: Utente_Impostazioni[]) {
    this.toSave.impostazioni = impostazioni;
    this.toSave.imprese = this.gestioneSelezioneTuttiCentri()
    if (this.toSave.hasToShowDialog) {
      this.showDialog().pipe(take(1)).subscribe();
    } else {
      this.salvaImpostazioni(this.toSave.imprese, this.toSave.impostazioni)
    }
  }

  /** Salva le impostazioi in riferimento alle imprese presenti nella variabile toSave */
  salvaImpostazioniAziendeCentriObs(impostazioni: Utente_Impostazioni[]) {
    this.toSave.impostazioni = impostazioni;
    this.toSave.imprese = this.gestioneSelezioneTuttiCentri()
    if (this.toSave.hasToShowDialog) {
      return this.showDialog();
    } else {
      return this.salvaImpostazioniObs(this.toSave.imprese, this.toSave.impostazioni)
    }
  }

  salvaImpostazioni(imprese: AziendaCentro[], impostazioni: Utente_Impostazioni[]) {
    const params = {
      imprese: imprese.map(i => new ImpresaDTO(i.Piva, i.Sa_Cod, i.Sa_Nome)),
      impostazioni: impostazioni
    };
    this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      "AgronicaCoreUtentiBIZ/SalvaImpostazioni_AziendeCentri",
      params
    ).pipe(map((res: rispostaStandard<any>) => {
      if (res.RispostaOK)
        this.updateImpostazioni(impostazioni, imprese)
      return res.RispostaOK;
    })).GiasSubscribe(success => {
      if (success)
        this.dialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
      else
        this.dialogService.baseError('', 'ErroreSalvataggio');
    });
  }

  salvaImpostazioniObs(imprese: AziendaCentro[], impostazioni: Utente_Impostazioni[]): Observable<boolean> {
    const params = {
      imprese: imprese.map(i => new ImpresaDTO(i.Piva, i.Sa_Cod, i.Sa_Nome)),
      impostazioni: impostazioni
    };
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      "AgronicaCoreUtentiBIZ/SalvaImpostazioni_AziendeCentri",
      params
    ).pipe(
      map((res: rispostaStandard<any>) => res.RispostaOK),
      tap((ok: boolean) => {
        if (ok) this.updateImpostazioni(impostazioni, imprese)
      })
    );
  }

  private updateImpostazioni(impostazioni: Utente_Impostazioni[], imprese: AziendaCentro[]) {
    if (impostazioni?.length && imprese?.length) {
      let listaImpostazioniImprese: Imprese_Impostazioni[] = [];
      for (let a of imprese) {
        impostazioni.map(i => new Imprese_Impostazioni(a.Piva, i.Impostazione_Cod, i.Valore, a.Sa_Cod))
          .forEach(newImp => listaImpostazioniImprese.push(newImp))
      }
      this.changeImprese_Impostazioni(listaImpostazioniImprese);
    }
  }

  /**
   * Resetta i valori delle impostazioni specificate a quelli di default.
   * Usata per "cancellare" le impostazioni.
   * @returns un `Observable` che indica quando l'operazione è stata compiuta
   */
  resetImpostazioni(impostazioni: Imprese_Impostazioni[]) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Imprese_Impostazioni[], any>(
      "AgronicaCoreUtentiBIZ/CancellaImpostazioni_AziendeCentri", impostazioni
    ).pipe(take(1), tap(res => {
      if (res.RispostaOK)
        this.dialogService.baseSuccess('EliminazioneAvvenutaConSuccesso', 'prof.ImpostazioniRipristinateADefault');
      else
        this.dialogService.baseError('ErroreEliminazione', 'ImpossibileCompletareOperazione');
    }));
  }

  public copiaImpostazioni(base: string[], template: string, imps: Utente_Impostazioni[] = []) {
    if (imps.length) {
      this.toSave.impostazioni = imps;
    }
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      "AgronicaCoreUtentiBIZ/CopiaImpostazioniAziende",
      new CopiaImpostazioniObj(base, template), true
    ).pipe(map((R) => {
      console.log('impostazioni salvate?', R);
      if (R.RispostaOK) {
        this.dialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
      } else
        this.dialogService.baseError('', 'ErroreSalvataggio');
      return R.RispostaOK;
    }));
  }

  /***** CENTRI *****/

  caricaImpreseXCentri() {
    const pive = this.imprese.map(i => i.Piva);
    this.masterService.set_isLoading({ isLoading: true, message: '' })
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'AnagraficaNG/CaricaCentriImpresa', { Lista: JSON.stringify(pive) }
    ).pipe(map((data) => {
      const centri = (<any>data.RispostaStringa);
      this.ImpresexCentri = [];
      this.generaImpresexCentri(centri);
      this.updateCentriInModifica(centri);
      this.masterService.set_isLoading({ isLoading: false, message: '' })
      return this.ImpresexCentri
    }));
  }

  private generaImpresexCentri(centri) { // O(mn)
    this.imprese.forEach(imp => {
      let centriImpresa = centri.filter(c => c.piva === imp.Piva)
      centriImpresa.forEach(c => {
        let centro = cloneDeep(imp);
        centro.Sa_Cod = c.sa_cod;
        centro.Sa_Nome = c.sa_nome;
        centro.numeroCentri = centriImpresa.length
        this.ImpresexCentri.push(centro);
      })
      if (centriImpresa.length > 1) {
        this.ImpresexCentri.push(imp)
      }
    })
  }

  private updateCentriInModifica(centri) { // O(mn^2)
    let inModifica = cloneDeep(this.impreseInModifica)
    for (let imp of inModifica) {
      if (imp.impresa.Sa_Cod === 0) {
        let centriImpresa = centri.filter(c => c.piva === imp.impresa.Piva)
        centriImpresa.forEach(c => {
          let centro = cloneDeep(imp);
          centro.impresa.Sa_Cod = c.sa_cod;
          centro.impresa.Sa_Nome = c.sa_nome;
          centro.impresa.numeroCentri = centriImpresa.length
          this.impreseInModifica.push(centro);
        })
        let index = this.impreseInModifica.findIndex(i => this.isSameCentro(imp, i))
        this.impreseInModifica.splice(index, 1);
      }
    }
  }

  /***** UTILITY SELEZIONE *****/

  private checkCentersCoherence() {
    // Non si applica se non sto mostrando i centri
    if (this.ImpresexCentri.length === 0) return

    let tuttiCentriItems = this.impreseSelezionate.filter(i => i.Sa_Cod === 0);
    for (let azienda of tuttiCentriItems) {
      if (this.isEverythingSelected(azienda)) {
        // se ho selezionato tutti i centri rimuove il record con 'tutti i centri'
        // dalle imprese selezionate poiché sottointeso
        let index = this.impreseSelezionate.findIndex(i => this.isSameCentro(i, azienda));
        this.impreseSelezionate.splice(index, 1);
      } else {
        // altrimenti aggiunge quelli mancanti
        this.addAllCenters(azienda)
      }
    }
  }

  private isEverythingSelected(azienda): boolean {
    let nCentri = this.ImpresexCentri.filter(i => i.Piva === azienda.piva).length;
    let nSelected = this.impreseSelezionate.filter(i => i.piva === azienda.piva).length;
    return nSelected === nCentri;
  }

  private addAllCenters(azienda) {
    let centers = this.impreseSelezionate.filter(i => i.piva === azienda.piva);
    let excluded = this.getCentriEsclusi(centers)

    let index = this.impreseSelezionate.findIndex(i => this.isSameCentro(i, azienda));
    this.impreseSelezionate.splice(index, 1);

    this.impreseSelezionate = this.impreseSelezionate.concat(excluded);
  }

  /**
   * A function to handle the selection of all centers.
   * Iterates over selected businesses and checks if the centers' selection in valid.
   * If the 'Tutti i Centri' record is selected but not all centers are in the selection,
   * adds all the business' centers to the adjusted selection.
   * Else considers only the selected records.
   * @return {Array} the list of centers after selection handling
   */
  private gestioneSelezioneTuttiCentri() { // O(mn^2)
    const editing = [];
    const distinctPiveSelezionate = new Set(this.impreseSelezionate.map(i => i.piva));
    for (let piva of distinctPiveSelezionate) {
      const totCentri = this.ImpresexCentri.filter(i => i.Piva == piva);
      const centriSelezionati = this.impreseSelezionate.filter(i => i.piva == piva);

      if (centriSelezionati.length > 1 && centriSelezionati.length < totCentri.length && centriSelezionati.some(i => i.Sa_Cod === 0)) {
        this.toSave.hasToShowDialog = true;
        totCentri.forEach(centro => editing.push(centro));
      } else {
        if (totCentri.length === 1) {
          const recordTuttiCentri = cloneDeep(totCentri[0]);
          recordTuttiCentri.Sa_Cod = 0;
          centriSelezionati.push(recordTuttiCentri);
        }
        centriSelezionati.forEach(centro => editing.push(centro));
      }
    }
    return editing;
  }

  private showDialog(): Observable<boolean> {
    return this.dialogService.warningObs('prof.TuttiCentriSelezionati', 'prof.WarningTuttiCentriSelezionati')
      .pipe(switchMap((response: boolean) => {
        if (!response) {
          return this.salvaImpostazioniObs(this.toSave.imprese, this.toSave.impostazioni)
        } else {
          this.toSave.imprese = this.impreseSelezionate;
          this.toSave.hasToShowDialog = false;
          return of(false);
        }
      }));
  }

  private getCentriEsclusi(centriSelezionati: any[]) {
    let esclusi = [];
    let piva = centriSelezionati.at(0).piva;
    let centri = this.ImpresexCentri.filter(c => c.Piva === piva);
    centri.forEach(centro => {
      let presente = centriSelezionati.find(c => c.Sa_Cod === centro.Sa_Cod)
      if (presente === undefined) {
        esclusi.push(centro);
      }
    });
    return esclusi;
  }

  /** Confronto basato su piva e Sa_Cod */
  public isSameCentro(c1, c2): boolean {
    return c1.piva === c2.piva && c1.Sa_Cod === c2.Sa_Cod;
  }

  /** Confronto basato solo su piva */
  public isSameAzienda(c1, c2): boolean {
    return c1.piva === c2.piva;
  }

  changeImprese_Impostazioni(imprese_impostazioni: Imprese_Impostazioni[]) {
    this.impreseImpostazioni$.next(imprese_impostazioni ?? []);
  }

  getCurrentImprese_Impostazioni(): Imprese_Impostazioni[] {
    return this.impreseImpostazioni$.value;
  }

  sonoInBudget(): boolean {
    return this.budgetService.getBudget().activeBudget == true
  }

  getImprese_Impostazioni(piva: string, sa_cod: number): Observable<Array<Imprese_Impostazioni>> {
    //Aggiorno le imprese_impostazioni facendo la lettura lato server solo se
    //la piva non è presente tra l'elenco di imprese_impostazioni
    let leggi_imprese_impostazioni = true;
    let imprese_impostazioni: Imprese_Impostazioni[] = this.getCurrentImprese_Impostazioni();

    if (imprese_impostazioni != undefined && imprese_impostazioni.findIndex(imp => imp.Piva === piva) > -1)
      leggi_imprese_impostazioni = false;

    if (leggi_imprese_impostazioni && this.masterService.InitialLoadCompleteValue()) {
      return this.ajaxAgronicaAPIService.ajaxAPIPost<any, Imprese_Impostazioni[]>('UtilityNG/Get_Imprese_Impostazioni', {
        Piva: piva, Sa_Cod: sa_cod
      }).pipe(take(1), map(R => {
        imprese_impostazioni = R.RispostaStringa;
        if (imprese_impostazioni.length == 0) {
          imprese_impostazioni = [{ Piva: piva, Sa_Cod: sa_cod, Impostazione_Cod: 0, Valore: '' }]
        }
        this.changeImprese_Impostazioni(imprese_impostazioni);
        return imprese_impostazioni;
      }));
    }
    return of([]);
  }


  getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser(Piva: string, Sa_Cod: number, Impostazione_Cod: number): string { // Aggiunta
    let valore = "";
    let impostazione_Centro_Azienda = this.getImpostazione_Centro_Azienda(Piva, Sa_Cod, Impostazione_Cod);

    if (impostazione_Centro_Azienda) {
      if (!impostazione_Centro_Azienda.Valore || impostazione_Centro_Azienda.Valore === "") {
        let impostazione_U = this.premessiutenteService.getImpostazione_Utente(Impostazione_Cod);
        if (impostazione_U && impostazione_U.Valore && impostazione_U.Valore !== "")
          valore = impostazione_U.Valore;
      } else {
        valore = impostazione_Centro_Azienda.Valore;
      }
    } else {
      let impostazione_U = this.premessiutenteService.getImpostazione_Utente(Impostazione_Cod);
      if (impostazione_U && impostazione_U.Valore && impostazione_U.Valore !== "")
        valore = impostazione_U.Valore;
    }

    return valore;
  }

  getImpostazione_Centro_Azienda(Piva: string, Sa_Cod: number, Impostazione_Cod: number) { // Aggiunta
    let impostazione: Imprese_Impostazioni = null;
    let imprese_impostazioni = this.getCurrentImprese_Impostazioni();

    //Prima controllo se esiste l'impostazione per il sa_cod passato se non esiste controllo se c'è per il sa_cod generale (0)
    impostazione = imprese_impostazioni?.find((imp) => imp.Impostazione_Cod === Impostazione_Cod && imp.Piva === Piva &&
      imp.Sa_Cod === Sa_Cod);

    if (!impostazione && Sa_Cod !== 0) {
      impostazione = imprese_impostazioni?.find((imp) => imp.Impostazione_Cod === Impostazione_Cod && imp.Piva === Piva &&
        imp.Sa_Cod === 0);
    }
    return impostazione;
  }

  getImpostazione_Centro_Azienda_per_Elem_Cod(Piva: string, Sa_Cod: number, Impostazione_Cod: number, Elem_Cod: number) {
    let valore: string = "";
    let imprese_impostazioni = this.getImpostazione_Centro_Azienda(Piva, Sa_Cod, Impostazione_Cod);

    if (imprese_impostazioni && imprese_impostazioni.Valore && imprese_impostazioni.Valore !== "") {
      const impostazione: Array<string> = imprese_impostazioni.Valore.split('|');
      for (let i = 0; i < impostazione.length; i++) {
        const elem_cod_impostazione: Array<string> = impostazione[i].split('_');
        if (Elem_Cod === +elem_cod_impostazione[0]) {
          valore = elem_cod_impostazione[1];
          break;
        }
      }
    }

    return valore;
  }

  getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser_per_Elem_Cod(Piva: string, Sa_Cod: number, Impostazione_Cod: number, Elem_Cod: number): string { // Aggiunta
    let valore: string = "";
    let valore_impostazione = this.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser(Piva, Sa_Cod, Impostazione_Cod);
    if (valore_impostazione && valore_impostazione !== "") {
      const impostazione: Array<string> = valore_impostazione.split('|');
      for (let i = 0; i < impostazione.length; i++) {
        const elem_cod_impostazione: Array<string> = impostazione[i].split('_');
        if (Elem_Cod === +elem_cod_impostazione[0]) {
          valore = elem_cod_impostazione[1];
          break;
        }
      }
    }
    return valore;
  }
}
