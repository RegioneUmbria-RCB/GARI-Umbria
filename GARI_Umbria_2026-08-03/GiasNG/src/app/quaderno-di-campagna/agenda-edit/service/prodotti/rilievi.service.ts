import {Injectable} from '@angular/core';
import {TranslocoService} from '@jsverse/transloco';

import {AjaxAgronicaAPIService} from '../../../../Service/ajax-agronica.api.service';
import {enum_LAVCOD} from '../../../../Model/TipiEnumerativi';
import {enum_TipoControllo, rispostaStandard} from 'gias-ui-kit';
import {QdCService} from '../qdc.service';
import {Disciplinare} from '../../../../Model/metaschema/Disciplinari';
import {Avversita} from '../../../../Model/metaschema/avversita/Avversita';
import {GruppoAvversita} from '../../../../Model/metaschema/avversita/GruppoAvversita';
import {UnitaDiMisura} from '../../../../Model/metaschema/UnitaDiMisura';
import {FaseFenologica} from '../../../../Model/metaschema/FaseFenologica';
import {IndiceRilievo} from '../../../../Model/metaschema/IndiceRilievo';
import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {DettaglioRilievo} from '../../../../Model/attivita/dettagli/DettaglioRilievo';
import {BaseCodeDescr} from '../../../../Model/baseClass/baseCodeDescr';
import {Lavorazione} from '../../../../Model/attivita/Lavorazione';
import {BehaviorSubject, filter, forkJoin, map, Observable, of, Subject, tap} from 'rxjs';
import {GridRilieviObject} from '../../componenti/sezioni-no-prodotto/rilievi/rilievi.model';
import {AvversitaRilievo} from '../../../../Model/metaschema/avversita/AvversitaRilievo';
import {AGRODATAINIZIO} from '../../../../Model/CostantiPersonalizzate';
import {
  GridImpiantoSelezionatoModel,
  Sezione_Rilievi
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {EsercizioCDC} from '../../../../Model/attivita/centri_di_costo/EsercizioCDC';
import {UnitaDiMisuraService} from '../../../../Service/Metaschema/UnitaDiMisura.service';
import {BooleanSettings, DropdownListItem, DropdownListWithForm} from 'gias-kendo-grid';
import {Specie} from '../../../../Model/metaschema/utilizzi/Specie';
import {MisureAvversitaAnagraficaService} from './misure-avversita-anagrafica.service';
import {MisuraAvversita} from "../../../../Model/metaschema/avversita/MisuraAvversita";
import {switchMap, take} from "rxjs/operators";
import {MisureIndiciMaturitaAnagraficaService} from "./misure-indici-maturita-anagrafica.service";
import {AgendaService, Leggi_Numero_Trappole} from "../../../../Service/Agenda/Agenda.service";
import {AvversitaGruppo} from "../../../../Model/metaschema/avversita/AvversitaGruppo";
import {RisorsaProdotto} from "../../../../Model/attivita/risorse/RisorsaProdotto";
import { UtilizzoAvversitaTrappole } from 'app/Model/attivita/dettagli/UtilizzoAvversitaTrappole';
import {AvversitaTrappole} from "../../../../Model/attivita/dettagli/AvversitaTrappole";


export class LeggiDatiRilievi {
  // Impostato lato server in base alle impostazioni SU
  public personalizzate = false;

  constructor(
    public lavCod: enum_LAVCOD | number,
    public vegCod: number,
    public dpiCod?: number | string,
    public idRcdpi?: string | number,
    public dpiPubblicoPrivato?: string | number,
    public eserciziCDC?: EsercizioCDC[],
    public avversitaGruppo?: AvversitaGruppo
  ) {  }
}

export class LeggiRilieviProduzione {
  /** @param esercizi progetto_cod degli esercizi interessati, racchiusi da apici
   * (es. `'cod'`) e concatenati tramite virgola (es. `'cod1', 'cod2', ...`)
   * @param data
   */
  constructor(
    public esercizi: string,
    public data: Date
  ) {  }
}

@Injectable()
export class QdCRilieviService {
  public reloadRowsSignal$ = new Subject();
  public _Sezione_Rilievo: FormGroup;
  /** Conserva ultime righe caricate da lla funzione <code>loadRilieviRows</code>.
   * I dati non sono aggiornati con le modifiche.
   * @private
   */
  private _rows: Array<GridRilieviObject> = [];
  private _fasiFenologiche: Map<number, FaseFenologica[]> = new Map<number, FaseFenologica[]>();
  private _erbeInfestanti: Map<number, AvversitaRilievo[]> = new Map<number, AvversitaRilievo[]>();
  private _avversita: Map<string, Avversita[]> = new Map<string, Avversita[]>();
  private _avversitaTrappole: Map<string, AvversitaTrappole[]> = new Map<string, AvversitaTrappole[]>();
  /** Conserva gli impiantiSelezionati collegati ai rilievi */
  private _impianti: Map<string, GridImpiantoSelezionatoModel> = new Map<string, GridImpiantoSelezionatoModel>();

  private readonly language: string;
  private _indiceUdM: Map<number, UnitaDiMisura> = new Map<number, UnitaDiMisura>();
  private udmLoaded$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    public qdcService: QdCService,
    private transloco: TranslocoService,
    private APIService: AjaxAgronicaAPIService,
    private udmService: UnitaDiMisuraService,
    private misureAvversitaAnagraficaService: MisureAvversitaAnagraficaService,
    private misureIndiciMaturitaAnagraficaService: MisureIndiciMaturitaAnagraficaService,
    private agendaService: AgendaService
  ) {
    this.language = this.transloco.getActiveLang();
    this.udmService.Leggi_Tutte_UnitaDiMisura().pipe(take(1))
      .subscribe(list => {
        list?.forEach(udm => this._indiceUdM.set(udm.codice, udm));
        this.udmLoaded$.next(true);
      });
  }

  public get Sezioni_Rilievi(): FormArray {
    let arr = new FormArray([]);
    this.qdcService.Sezioni_Senza_ProdottoFormArray.controls
      .filter((sezione: FormGroup) => this.isRilievo(sezione))
      .map(c => c as FormControl)
      .forEach(c => arr.push(c));
    return arr;
  }

  /** Conserva ultime righe caricate dalla funzione <code>loadRilieviRows</code>.
   * I dati non sono aggiornati con le modifiche.
   */
  public get rows(): Array<GridRilieviObject> {
    return this._rows;
  }
  /** Restituisce la sezione corrente in forma di FormGroup.
   * La struttura dovrebbe essere la stessa di una {@link Sezione_Rilievi}.
   * */
  public get Sezione_Rilievo() {
    return this._Sezione_Rilievo;
  }

  private get specie(): Specie {
    return this.qdcService.GetSpeciefromUtilizzoTerreno();
  }

  private get disciplinare(): Disciplinare {
    let lavCod = this._Sezione_Rilievo
      ? +this._Sezione_Rilievo.value.Operazione.primaryKey.codice
      : enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO;
    let disciplinare = this.qdcService.getDisciplinareModelValue(lavCod);
    if (!disciplinare) {
      disciplinare = new Disciplinare('0');
    }
    return disciplinare;
  }

  public set Sezione_Rilievo(fg: FormGroup) {
    if (!this._Sezione_Rilievo) this._Sezione_Rilievo = fg;
  }

  public get impiantiSelezionati(): GridImpiantoSelezionatoModel[] {
    const codici = this.qdcService.GetEserciziCDCSelezionatiModel().map(cdc => cdc.esercizio.codice);
    return this.qdcService.ImpiantiForm.value.ImpiantiSelezionati.filter(i => codici.includes(i.Progetto_Cod));
  }

  public isRilievo(sezione: FormGroup): boolean {
    let lavCod = +sezione.value.Operazione.primaryKey.codice;
    return this.qdcService.Elenco_Operazioni_Rilievi.includes(lavCod);
  }

  // --- USATE IN GRID-RILIEVI

  public loadRilieviRows(): Observable<Array<GridRilieviObject>> {
    let obs: Array<Observable<GridRilieviObject>> = [];
    for (let sezioneRilievo of this.Sezioni_Rilievi.controls) {
      for (let dettaglioCtrl of (sezioneRilievo.get('DettagliRilievi') as FormArray).controls) {
        obs.push(this.getRowImpiantoxRilievo(dettaglioCtrl as FormGroup, sezioneRilievo as FormGroup)
          .pipe(take(1)));
      }
    }
    if (obs.length === 0) return of([]).pipe(tap(rows=>this._rows = []));
    return forkJoin(obs).pipe(switchMap((rows) => {
      if (+this.Sezioni_Rilievi.value?.at(0)?.Operazione?.primaryKey?.codice === enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA) {
        return this.getStimeResa().pipe(take(1), map(rese => {
          rows.forEach(r => r.ResaAnagrafica = rese.get(r.Impianto_Key));
          return rows;
        }));
      } else return of(rows);
    }), tap(rows => {
      this._rows = rows;
    }));
  }

  public getDescription(lav: Lavorazione, dettaglio: DettaglioRilievo): Observable<any> {
    switch (+lav.primaryKey.codice) {
      case enum_LAVCOD.DANNI_RACCOLTA:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.dannoRaccolta, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.DANNI_RACCOLTA,null,dettaglio)
          });
        return this.getRilievoDanniDesc(dettaglio);
      case enum_LAVCOD.FASI_FENOLOGICHE:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.faseFenologica.codice, this.getDescFaseFenologica(dettaglio.faseFenologica)),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.FASI_FENOLOGICHE,null,dettaglio)
          });
        return this.getRilievoFFDesc(dettaglio);
      case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.avversitaGruppo.codice, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO,null,dettaglio)
          });
        return this.getRilievoAvversitaDesc(dettaglio,+lav.primaryKey.codice);
      case enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.indiceResa, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA,null,dettaglio)
          });
        return this.getRilievoReseDesc(dettaglio);
      case enum_LAVCOD.RILIEVO_INDICI_MATURITA:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.indiceMaturita, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.RILIEVO_INDICI_MATURITA,null,dettaglio)
          });
        return this.getRilievoMaturitaDesc(dettaglio);
      case enum_LAVCOD.RILIEVO_ERBE_INFESTANTI:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.erbaInfestante.codice, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.RILIEVO_ERBE_INFESTANTI,null,dettaglio)
          });
        return this.getRilievoErbeDesc(dettaglio);
      case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:
        if (dettaglio.descrizione)
          return of( {
            ...new BaseCodeDescr(dettaglio.avversitaGruppo.codice, dettaglio.descrizione),
            codRilievo: this.GetCodRilievo(enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE,null,dettaglio)
          });
        return this.getRilievoAvversitaTrappoleDesc(dettaglio,this.qdcService.GetEserciziCDCModel());
    }
  }

  public getDettaglioRilievo(rilievo: GridRilieviObject): { fg: FormGroup; index: number } {
    let index = -1;
    let sezioneRilievo = this.Sezioni_Rilievi.controls.find((ctrl: FormGroup) =>
      +ctrl.value.Operazione?.primaryKey?.codice === rilievo.Op_Cod
    );
    if (!sezioneRilievo || !sezioneRilievo.get('DettagliRilievi'))
      return {fg: null, index: index};

    index = (sezioneRilievo.get('DettagliRilievi') as FormArray).controls
      .findIndex((ctrl: FormGroup) =>
        ctrl.value.codRilievo === rilievo.Descriz_Key &&
        ((!ctrl.value.esercizioCDC && rilievo.Impianto_Cod === 0) ||
          (+ctrl.value.esercizioCDC.esercizio.codice === rilievo.Impianto_Cod))
      );

    let dettaglio: FormGroup = (sezioneRilievo.get('DettagliRilievi') as FormArray)
      .at(index) as FormGroup;
    return {fg: dettaglio, index: index};
  }

  public getStimeResa(): Observable<Map<string, string>> {
    const reseAttese = new Map<string, string>();
    if (!this.qdcService.GetEserciziCDCSelezionatiModel().length) {
      return of(reseAttese);
    }
    const esercizi = this.impiantiSelezionati
      .map(i => {
        let key = i.PIVA + "|" + i.SA_COD + "|" + i.APPEZZA + "|" + i.ID_REG + "|" + i.Progetto_Cod;
        reseAttese.set(key, this.transloco.translate('NessunDatoRegistrato'));
        return "'" + i.Progetto_Cod + "'";
      })
      .reduce((p1, p2) => p1 + ", " + p2);
    return this.APIService.ajaxAPIPost<LeggiRilieviProduzione, any[]>(
      'Statistiche/RilieviProduzione', new LeggiRilieviProduzione(esercizi, new Date())
    ).pipe(take(1),
      map(R => R.RispostaOK ? R.RispostaStringa : []),
      map(rese => {
        rese.forEach(r => {
          const key = r.partitaIva + "|" + r.centroAziendaleCod + "|" + r.appezzamentoCod
            + "|" + r.impiantoCod + "|" + r.esercizioCod;
          const value = r.resaRilievo + ' (' + r.utenteRilievo + ', ' + r.dataRilievo.toLocaleString().split(',')[0] + ')';
          reseAttese.set(key, value);
        });
        return reseAttese;
      })
    );
  }

  public updateRilievo(rigaRilievo: GridRilieviObject) {
    switch (rigaRilievo.QtaObj.controlType) {
      case enum_TipoControllo.NUMERO_INTERO:
      case enum_TipoControllo.NUMERO_DECIMALE:
        this.handleQtaNumberUpdate(rigaRilievo, rigaRilievo.QtaObj.controlType);
        break;
      case enum_TipoControllo.MENU_DISCESA:
        this.handleQtaDDLUpdate(rigaRilievo);
        break;
      case enum_TipoControllo.CALENDARIO:
        this.handleQtaDateUpdate(rigaRilievo);
        break;
      case enum_TipoControllo.CASELLA_SPUNTA:
        this.handleQtaBoolUpdate(rigaRilievo);
        break;
    }
  }

  public removeRilievo(rigaRilievo: GridRilieviObject) {
    let sezioneRilievo = this.Sezioni_Rilievi.controls.find((ctrl: FormGroup) =>
      +ctrl.value.Operazione?.primaryKey?.codice === rigaRilievo.Op_Cod
    );
    if (!sezioneRilievo || !sezioneRilievo.get('DettagliRilievi')) return;
    let i = this.getDettaglioRilievo(rigaRilievo).index;
    if (i >= 0) {
      (sezioneRilievo.get('DettagliRilievi') as FormArray).removeAt(i);
      this.reloadRowsSignal$.next(null);
    }
  }

  // --- LETTURA DDL RILIEVI

  public leggiAvversita(lavCod: number,vegCod: number, dpi: Disciplinare): Observable<Avversita[]> {
    //Carico gli indici solamente se ho selezionato una Specie Vegetale.
    if(lavCod > 0 && vegCod > 0){
      let hash = lavCod + '_' + vegCod + '_' + dpi.codice;
      if (this._avversita.has(hash))
        return of(this._avversita.get(hash));
      let idRcdpi = dpi.raggruppamentiColturaliDPI?.codice ? dpi.raggruppamentiColturaliDPI.codice : 0;

      let params = new LeggiDatiRilievi(
        lavCod, vegCod,
        +dpi.codice, idRcdpi,
        dpi.disciplinarePubblicoPrivato);
      return this.popolaRilievo(params).pipe(map(res => {
        let avv = res.map(i => {
          let codici = (i['cod'] as string).split('|');
          let a = new Avversita(+codici[0]);
          a.classType = '';
          a.gruppo = new GruppoAvversita(+codici[1]);
          a.unitaDiMisura = this.getUdM(+codici[2]);
          a.descrizione = i['des'];
          a.soglia = i['soglia'];
          a['codRilievo'] = codici[0] + "|" + codici[1] + "|" + codici[2];
          a.MxAV_Cod = i.MxAV_Cod;
          return a;
        });
        this._avversita.set(hash, avv);
        return avv;
      }));
    }else{
      return of([]);
    }
  }

  public leggiAvversitaTrappole(eserciziCDC: EsercizioCDC[]): Observable<AvversitaTrappole[]> {

    if(eserciziCDC && eserciziCDC.length > 0){
      /*let hash = lavCod + '_' + vegCod + '_' + dpi.codice;
      if (this._avversita.has(hash))
        return of(this._avversita.get(hash));
      let idRcdpi = dpi.raggruppamentiColturaliDPI?.codice ? dpi.raggruppamentiColturaliDPI.codice : 0;*/

      let params = new LeggiDatiRilievi(enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE,0,0,0,0,eserciziCDC);
      return this.popolaRilievo(params).pipe(map(res=>{

        let avv = res.map(i => {
          i['codRilievo'] = this.GetCodRilievo(enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE,i,null);

          return i;
        });

        return avv;
      }));
    }else{
      return of([]);
    }
  }

  public leggiIndiciMaturita(vegCod: number): Observable<IndiceRilievo[]> {
    //Carico gli indici solamente se ho selezionato una Specie Vegetale oppure la voce 'Nessuna Specie'.
    //Se seleziono un destinazione d'uso non carico nulla
    if(vegCod !== 0){

      let params = new LeggiDatiRilievi(enum_LAVCOD.RILIEVO_INDICI_MATURITA, vegCod);
      return this.popolaRilievo(params)
        .pipe(
          map(res => res.map(i => {
              let codici = (i['cod'] as string).split('|');
              let idx = new IndiceRilievo(+codici[0], i['des']);
              idx.unitaDiMisura = this.getUdM(+codici[1]);
              idx['codRilievo'] = this.GetCodRilievo(enum_LAVCOD.RILIEVO_INDICI_MATURITA,idx,null);
              return idx;
            })
          )
        );
    }else{
      return of([]);
    }
  }

  public leggiDanniRaccolta(vegCod: number): Observable<IndiceRilievo[]> {
    //Carico gli indici solamente se ho selezionato una Specie Vegetale oppure la voce 'Nessuna Specie'.
    //Se seleziono un destinazione d'uso non carico nulla
    if(vegCod !== 0){
      let params = new LeggiDatiRilievi(enum_LAVCOD.DANNI_RACCOLTA, vegCod);
      return this.popolaRilievo(params)
        .pipe(map(res => res.map(i => {
          let codici = (i['cod'] as string).split('|');
          let idx = new IndiceRilievo(+codici[0], i['des']);
          idx.unitaDiMisura = this.getUdM(+codici[1]);
          idx['codRilievo'] = this.GetCodRilievo(enum_LAVCOD.DANNI_RACCOLTA,idx,null);
          return idx;
        })));
    }else{
      return of([]);
    }
  }

  public leggiFasiFenologiche(vegCod: number): Observable<FaseFenologica[]> {

    //Carico gli indici solamente se ho selezionato una Specie Vegetale.
    //Se seleziono un destinazione d'uso non carico nulla
    const specie = this.qdcService.GetSpeciefromUtilizzoTerreno();

    if(specie && specie.codice > 0){

      if (this._fasiFenologiche.has(vegCod))
        return of(this._fasiFenologiche.get(vegCod));

      let params = new LeggiDatiRilievi(enum_LAVCOD.FASI_FENOLOGICHE, vegCod);
      return this.popolaRilievo(params).pipe(map(res => {
        let fasi = res.map(i => {
          const codici = (i['cod'] as string).split('|');
          let ff = new FaseFenologica(+codici[0], i['des']);
          ff.stadioCrescitaBBCH = new BaseCodeDescr(+codici[1]);
          ff.fioritura = +i['fioritura'] === 1;
          ff.stadio = i['stadio'];
          ff.specieVegetale = this.qdcService.GetSpeciefromUtilizzoTerreno();
          ff.descrizione = this.getDescFaseFenologica(ff);

          let udm: UnitaDiMisura = {codice: 0,descrizione: '',tipoControllo: new BaseCodeDescr(enum_TipoControllo.CALENDARIO)};

          return {...ff,unitaDiMisura:udm, codRilievo: this.GetCodRilievo(enum_LAVCOD.FASI_FENOLOGICHE,ff,null)};
        });
        this._fasiFenologiche.set(vegCod, fasi);
        return fasi;
      }));

    }else{
      return of([]);
    }


  }

  public leggiErbeInfestanti(vegCod: number): Observable<AvversitaRilievo[]> {
    //Carico gli indici solamente se ho selezionato una Specie Vegetale.
    if(vegCod > 0){
      if (this._erbeInfestanti.has(vegCod))
        return of(this._erbeInfestanti.get(vegCod));
      let params = new LeggiDatiRilievi(enum_LAVCOD.RILIEVO_ERBE_INFESTANTI, vegCod);
      return this.popolaRilievo(params).pipe(map(res => {
        let erbeInfestanti: AvversitaRilievo[] = res.map(i => this.mapObjToInfestante(i));
        this._erbeInfestanti.set(vegCod, erbeInfestanti);
        return erbeInfestanti;
      }));
    }else{
      return of([]);
    }

  }

  public leggiIndiciReseRaccolta(vegCod: number): Observable<IndiceRilievo[]> {
    //Carico gli indici solamente se ho selezionato una Specie Vegetale.
    if(vegCod > 0){
      let params = new LeggiDatiRilievi(enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA, vegCod);
      return this.popolaRilievo(params)
        .pipe(map(res => res.map(i => {
            let codici = (i['cod'] as string).split('|');
            let idx = new IndiceRilievo(+codici[0], i['des']);
            idx.unitaDiMisura = this.getUdM(+codici[1]);
            idx['codRilievo'] = this.GetCodRilievo(enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA,idx,null);
            return idx;
          })
        ));
    }else{
      return of([]);
    }

  }

  // --- FUNZIONI PRIVATE

  private popolaRilievo(params: LeggiDatiRilievi): Observable<any> {
    return this.APIService.ajaxAPIPost<LeggiDatiRilievi, any>("Agenda/PopolaRilievo", params)
      .pipe(map(R => R.RispostaStringa));
  }

  private getRilievoAvversitaDesc(dettaglio: DettaglioRilievo,lavCod: number): Observable<any> {
    return this.leggiAvversita(lavCod, this.specie.codice, this.disciplinare)
      .pipe(map(avv => avv.find(a =>
        a.codice === dettaglio.avversitaGruppo.codice
        && a.unitaDiMisura.codice === dettaglio.unitaDiMisura.codice
      )));
  }

  private getRilievoErbeDesc(dettaglio: DettaglioRilievo): Observable<any> {
    const specie = this.qdcService.GetSpeciefromUtilizzoTerreno();
    const findSingleWeed = (weeds: AvversitaRilievo[]) => weeds.find(a => (a['codRilievo'] as string).endsWith('|' + dettaglio.erbaInfestante.codice));
    const findWeedGroup = (weeds: AvversitaRilievo[]) => weeds.find(a => (a['codRilievo'] as string).startsWith(dettaglio.erbaInfestante.codice + '|'));
    return this.leggiErbeInfestanti(specie.codice)
      .pipe(map(erbe => dettaglio.erbaInfestante.classType === 'GruppoAvversita'
        ? findWeedGroup(erbe) : findSingleWeed(erbe)
      ));
  }

  private getRilievoFFDesc(dettaglio: DettaglioRilievo): Observable<FaseFenologica> {
    let specie = this.qdcService.GetSpeciefromUtilizzoTerreno();
    return this.leggiFasiFenologiche(specie.codice)
      .pipe(map(fasi =>
        fasi.find(f => f.codice === dettaglio.faseFenologica.codice))
      );
  }

  private getRilievoMaturitaDesc(dettaglio: DettaglioRilievo): Observable<any> {
    let specie = this.qdcService.GetSpeciefromUtilizzoTerreno();
    return this.leggiIndiciMaturita(specie.codice)
      .pipe(map(imats => imats.find(a =>
        a.codice === dettaglio.indiceMaturita
        && a.unitaDiMisura.codice === dettaglio.unitaDiMisura.codice
      )));
  }

  private getRilievoReseDesc(dettaglio: DettaglioRilievo): Observable<any> {
    let specie = this.qdcService.GetSpeciefromUtilizzoTerreno();
    return this.leggiIndiciReseRaccolta(specie.codice)
      .pipe(map(rese => rese.find(a =>
        a.codice === dettaglio.indiceResa
        && a.unitaDiMisura.codice === dettaglio.unitaDiMisura.codice
      )));
  }

  private getRilievoDanniDesc(dettaglio: DettaglioRilievo): Observable<any> {
    let specie = this.qdcService.GetSpeciefromUtilizzoTerreno();
    return this.leggiDanniRaccolta(specie.codice)
      .pipe(map(danni => danni.find(a =>
        a.codice === dettaglio.dannoRaccolta
        && a.unitaDiMisura.codice === dettaglio.unitaDiMisura.codice
      )));
  }

  private getRilievoAvversitaTrappoleDesc(dettaglio: DettaglioRilievo,eserciziCDC: EsercizioCDC[]): Observable<any> {
    return this.leggiAvversitaTrappole(eserciziCDC)
      .pipe(map(avv => avv.find((a: AvversitaTrappole) =>
        a.codice === dettaglio.avversitaGruppo.codice
        && a.unitaDiMisura.codice === dettaglio.unitaDiMisura.codice
      )));
  }

  private mapObjToInfestante(i: any): AvversitaRilievo {
    let infestante: AvversitaRilievo;
    if (+i['av_cod'] === 0) { // Gruppo avversità generico
      infestante = new AvversitaRilievo(+i['cod']);
      infestante.classType = 'GruppoAvversita';
      infestante.descrizione = i['des'];
      infestante.dataSmaltimentoScorte = AGRODATAINIZIO;
    } else { // Avversità singola
      infestante = new AvversitaRilievo(+i['av_cod']);
      infestante.descrizione = i['des'];
      infestante.dataSmaltimentoScorte = AGRODATAINIZIO;
      infestante.gruppo = new GruppoAvversita(+i['cod']);
    }
    infestante['codRilievo'] = this.GetCodRilievo(enum_LAVCOD.RILIEVO_ERBE_INFESTANTI,infestante,null);
    return infestante;
  }

  private getKeyFromCDC(cdc: EsercizioCDC): string {

    let key = "0|0|0|0|0";

    if(cdc){

      key = cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "|"
        + cdc.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice + "|"
        + cdc.esercizio.impiantoPK.appezzamentoPK.codice + "|"
        + cdc.esercizio.impiantoPK.codice + "|"
        + cdc.esercizio.codice;

    }


    return key;


  }

  private getTipoControllo(row: GridRilieviObject, dettaglioCtrl: FormGroup): Observable<enum_TipoControllo> {
    const withPreset = [enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO, enum_LAVCOD.RILIEVO_INDICI_MATURITA];
    let dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    if (row.Op_Cod === enum_LAVCOD.FASI_FENOLOGICHE)
      return this.valorizzaPresets(dettaglioCtrl, enum_TipoControllo.CALENDARIO);
    else if (withPreset.includes(row.Op_Cod) && dettaglio.presets?.length)
      return of(enum_TipoControllo.MENU_DISCESA);
    else
      return this.getTipoControlloPerUdM(dettaglio.unitaDiMisura.codice)
        .pipe(switchMap(ctrl => {
          let tipoControllo = ctrl.codice;
          return this.valorizzaPresets(dettaglioCtrl, tipoControllo);
        }));
  }

  private doesnotSupportPresets(dettaglio: DettaglioRilievo) {
    return !dettaglio.avversitaGruppo && !dettaglio.indiceMaturita;
  }

  private valorizzaPresets(dettaglioCtrl: FormGroup, tipoControllo: enum_TipoControllo): Observable<enum_TipoControllo> {
    let dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    let lav_cod: number = + this._Sezione_Rilievo?.value?.Operazione?.primaryKey?.codice;

    if (this.doesnotSupportPresets(dettaglio) || dettaglio.presets?.length > 0 || lav_cod === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE)
      return of(tipoControllo);

    /* Ho caricato un rilievo già esistente, devo rivalorizzare al volo le misure avversità disponibili */
    if (dettaglio.avversitaGruppo) {
      return this.loadPresetAdversity(dettaglioCtrl, tipoControllo);
    } else if (dettaglio.indiceMaturita) {
      return this.loadPresetMaturityIndexes(dettaglioCtrl, tipoControllo);
    }
  }

  private loadPresetAdversity(dettaglioCtrl: FormGroup, tipoControllo: enum_TipoControllo) {
    const dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    const avv = dettaglio.avversitaGruppo as Avversita;
    avv.unitaDiMisura = this.getUdM(dettaglio.unitaDiMisura.codice);
    return this.misureAvversitaAnagraficaService.getMisureAvversita(avv.MxAV_Cod,this.qdcService.getDisciplinareModelValue(enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO))
      .pipe(map((misure: MisuraAvversita[]) => {
        if (misure.length > 0) {
          tipoControllo = enum_TipoControllo.MENU_DISCESA;
          dettaglio.presets = misure;
          // Perché la riga sotto?
          dettaglio.qtaRilevata = misure.find(m => m.valoreAnagrafica === dettaglio.qtaRilevata)?.CodiceAnagrafica;
          dettaglio.qtaRilevataString = dettaglio.qtaRilevata?.toString() ?? dettaglio.qtaRilevataString;
          dettaglioCtrl.get('presets').patchValue(dettaglio.presets);
          dettaglioCtrl.get('qtaRilevata').patchValue(dettaglio.qtaRilevata);
          dettaglioCtrl.get('qtaRilevataString').patchValue(dettaglio.qtaRilevataString);
        }
        return tipoControllo;
      }));
  }

  private loadPresetMaturityIndexes(dettaglioCtrl: FormGroup, tipoControllo: enum_TipoControllo) {
    const dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    return this.misureIndiciMaturitaAnagraficaService.getMisureIndiciMaturita(
      dettaglio.indiceMaturita, dettaglio.unitaDiMisura.codice, this.qdcService.GetSpeciefromUtilizzoTerreno()
    ).pipe(
      map(presets => presets.map(i => MisuraAvversita.fromBaseCodDescr(i))),
      map((misure: MisuraAvversita[]) => {
        if (misure.length > 0) {
          tipoControllo = enum_TipoControllo.MENU_DISCESA;
          dettaglio.presets = misure;
          // Perché la riga sotto?
          dettaglio.qtaRilevata = misure.find(m => m.valoreAnagrafica === dettaglio.qtaRilevata)?.CodiceAnagrafica;
          dettaglio.qtaRilevataString = dettaglio.qtaRilevata?.toString() ?? dettaglio.qtaRilevataString;
          dettaglioCtrl.get('presets').patchValue(dettaglio.presets);
          dettaglioCtrl.get('qtaRilevata').patchValue(dettaglio.qtaRilevata);
          dettaglioCtrl.get('qtaRilevataString').patchValue(dettaglio.qtaRilevataString);
        }
        return tipoControllo;
      }));
  }

  public getImpianto(dettaglio: DettaglioRilievo): GridImpiantoSelezionatoModel {
    let key = this.getKeyFromCDC(dettaglio.esercizioCDC);
    if (this._impianti.has(key)) {
      return this._impianti.get(key);
    }
    let impianto: GridImpiantoSelezionatoModel;
    const allImpianti: GridImpiantoSelezionatoModel[] = this.qdcService.QdCForm.get('Trattamento').get('Impianti').get("ImpiantiSelezionati").value;

    const centri = this.qdcService.elencoCentriAziendali;

    impianto = allImpianti.find(i => i.Progetto_Cod === dettaglio.esercizioCDC.esercizio.codice
      && i.ID_REG === dettaglio.esercizioCDC.esercizio.impiantoPK.codice
      && i.APPEZZA === dettaglio.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
      && i.SA_COD === dettaglio.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
      && i.PIVA === dettaglio.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva);

    if (impianto) {
      key = impianto.PIVA + "|" + impianto.SA_COD + "|"
        + impianto.APPEZZA + "|" + impianto.ID_REG + "|"
        + impianto.Progetto_Cod;
      impianto['SA_NOME'] = centri.find(c => c.primaryKey.codice === impianto.SA_COD)?.nome;
      this._impianti.set(key, impianto);
    }
    return impianto;
  }

  private getRowImpiantoxRilievo(dettaglioCtrl: FormGroup, sezioneRilievo: FormGroup): Observable<GridRilieviObject> {
    let dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    let progetto_cod: number = 0;

    if(dettaglio.esercizioCDC && dettaglio.esercizioCDC.esercizio)
      progetto_cod = dettaglio.esercizioCDC.esercizio.codice;

    return this.getDescription(sezioneRilievo.value.Operazione, dettaglio).pipe(switchMap(desc => {
      let row =this._rows.find(r =>
        r.Impianto_Cod === progetto_cod && r.Descriz_Key === desc['codRilievo']
      );
      if (row) {
        this.valorizzaImpianto(row, dettaglioCtrl);
        return of(row);
      } else {
        row = new GridRilieviObject();
        row.Op_Cod = +sezioneRilievo.value.Operazione.primaryKey.codice;
        row.Op_Des = sezioneRilievo.value.Operazione.descrizione;
        this.valorizzaDescrizione(row, desc, dettaglioCtrl);
        return this.valorizeDynamicInputSettings(row, dettaglioCtrl).pipe(map(() => {
          this.valorizzaQta(row, dettaglioCtrl);
          this.valorizzaImpianto(row, dettaglioCtrl);
          return row;
        }));
      }
    }));
  }


  private valorizzaDescrizione(row: GridRilieviObject, desc: any, dettaglioCtrl: FormGroup) {
    let dettaglio = dettaglioCtrl.value;
    row.Descriz_Cod = desc.codice;
    row.Descriz_Des = desc.descrizione;
    dettaglioCtrl.get('descrizione').patchValue(desc.descrizione);
    dettaglio.codRilievo = dettaglio.codRilievo ? dettaglio.codRilievo : desc['codRilievo'];
    if (row.Op_Cod === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO) {
      dettaglio.avversitaGruppo.MxAV_Cod = dettaglio.avversitaGruppo.MxAV_Cod || (desc as Avversita).MxAV_Cod || -1;
      dettaglioCtrl.get('avversitaGruppo').patchValue(dettaglio.avversitaGruppo);
    }
    dettaglioCtrl.get('codRilievo').patchValue(dettaglio.codRilievo);
    row.Descriz_Key = dettaglio.codRilievo;

    row.Prodotto_Obj = null;

    row.Pro_Des = "";

    if(row.Op_Cod === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE){

      let utilizzi: UtilizzoAvversitaTrappole[] = desc['utilizzi'];

      let risorsaProdotto: RisorsaProdotto = null;

      if(utilizzi){
        if(utilizzi.length === 1 && utilizzi[0].risorsaProdotto)
          risorsaProdotto = utilizzi[0].risorsaProdotto;
      }else if(dettaglio.risorsaProdotto){
        risorsaProdotto = dettaglio.risorsaProdotto;
      }

      if(risorsaProdotto){
        row.Prodotto_Obj = risorsaProdotto.prodotto;
        row.Pro_Des = this.getDescrizioneProdottoRilievo(risorsaProdotto);
      }
    }
  }

  private valorizzaImpianto(row: GridRilieviObject, dettaglioCtrl: FormGroup): void {

    let dettaglio = dettaglioCtrl.value;

    row.ResaAnagrafica = dettaglio.resaUltimoRilievo ?? null;

    row.SpecieVegetale = this.qdcService.GetSpeciefromUtilizzoTerreno();

    row.Impianto_Cod = 0;

    row.Impianto_Des = this.transloco.translate("TuttiGliImpianti");

    row.Impianto_Key = "";

    let impianto: GridImpiantoSelezionatoModel = this.getImpianto(dettaglio);

    if(impianto){
      row.Impianto_Cod = impianto ? dettaglio.esercizioCDC.esercizio.codice : -1;

      row.Impianto_Des = this.getEsercizioDes(impianto.App_Nome,impianto['SA_NOME']);

      row.Impianto_Key = this.getKeyFromCDC(dettaglio.esercizioCDC);

      //Valorizzo la descrizione dell'esercizioCDC se non è già valorizzata nel fromgroup dei Rilievi
      if(dettaglio.esercizioCDC.esercizio.descrizione === ""){
        dettaglio.esercizioCDC.esercizio.descrizione = row.Impianto_Des;
        dettaglioCtrl.get("esercizioCDC").patchValue(dettaglio.esercizioCDC);
      }

    }

    row.Row_Key = dettaglio.codRilievo + "|" + row.Impianto_Key;

  }

  private valorizzaQta(row: GridRilieviObject, dettaglioCtrl: FormGroup) {
    let dettaglio = dettaglioCtrl.value;

    if(dettaglio.note && dettaglio.note !== ""){
      row.Qta = dettaglio.note;
    }else{
      row.Qta = dettaglio.qtaRilevataString;
    }

    if (!dettaglio.qtaRilevataString.includes("/")) {
      dettaglio.qtaRilevataString = dettaglio.qtaRilevataString.replace(/,/g, '.');
      row.QtaNumber = +dettaglio.qtaRilevataString;
    }
    row.QtaNumber = dettaglio.qtaRilevataString.includes("/") ? null : +dettaglio.qtaRilevataString;
    row.QtaDate = dettaglio.dataOraRilievo instanceof Date ?
      dettaglio.dataOraRilievo : new Date(dettaglio.dataOraRilievo);
  }

  /** Valorizza il campo QtaObj per fornire le informazioni necessarie al
   * {@link DynamicInputComponent} nella griglia. */
  private valorizeDynamicInputSettings(row: GridRilieviObject, dettaglioCtrl: FormGroup) {
    let dettaglio: DettaglioRilievo = dettaglioCtrl.value;
    return this.getTipoControllo(row, dettaglioCtrl)
      .pipe(take(1), map(tipoControllo => {
        row.QtaObj = {
          controlType: tipoControllo || enum_TipoControllo.NUMERO_DECIMALE,
          realField: this.getCampoReale(tipoControllo),
          onEdit: (val: GridRilieviObject) => this.updateRilievo(val)
        };
        if (row.QtaObj.controlType === enum_TipoControllo.MENU_DISCESA) {
          let listItems: DropdownListItem[] = dettaglio.presets.map(p =>
            new DropdownListItem(p.CodiceAnagrafica, p.Descrizione, p.valoreAnagrafica)
          );
          row.QtaObj.ddl = new DropdownListWithForm('id', 'codice', 'descrizione', listItems);
        }
        if (row.QtaObj.controlType === enum_TipoControllo.CASELLA_SPUNTA) {
          row.QtaObj.boolean = new BooleanSettings();
          row.QtaObj.boolean.leftLabel = this.transloco.translate("Assente");
          row.QtaObj.boolean.rightLabel = this.transloco.translate("Presente");
        }
        return;
      }));
  }

  private getCampoReale(tipoControllo) {
    switch (tipoControllo) {
      case enum_TipoControllo.CALENDARIO:
        return "QtaDate";
      case enum_TipoControllo.NUMERO_INTERO:
      case enum_TipoControllo.NUMERO_DECIMALE:
      case enum_TipoControllo.MENU_DISCESA:
        return "QtaNumber";
      case enum_TipoControllo.CASELLA_SPUNTA:
      default:
        return "Qta";
    }
  }

  private handleQtaNumberUpdate(rigaRilievo: GridRilieviObject, controlType: enum_TipoControllo) {
    const found = this.getDettaglioRilievo(rigaRilievo);
    const dettaglio: FormGroup = found.fg;
    if (controlType === enum_TipoControllo.NUMERO_INTERO)
      rigaRilievo.QtaNumber = Math.round(rigaRilievo.QtaNumber);
    dettaglio.get('qtaRilevata').patchValue(rigaRilievo.QtaNumber);
    dettaglio.get('qtaRilevataString').patchValue(rigaRilievo.QtaNumber.toString());
    rigaRilievo.Qta = rigaRilievo.QtaNumber.toString();
    this._rows[found.index].Qta = rigaRilievo.QtaNumber.toString();
    this._rows[found.index].QtaNumber = rigaRilievo.QtaNumber;
  }

  private handleQtaDDLUpdate(rigaRilievo: GridRilieviObject) {
    const found = this.getDettaglioRilievo(rigaRilievo);
    const dettaglio: FormGroup = found.fg;
    const id = rigaRilievo.QtaNumber['id'];
    dettaglio.get('qtaRilevata').patchValue(id);
    dettaglio.get('qtaRilevataString').patchValue(id.toString());
    rigaRilievo.Qta = id.toString();
    this._rows[found.index].Qta = id.toString();
    this._rows[found.index].QtaNumber = id;
    let presets = dettaglio.value.presets || [];
    presets.forEach(preset => preset['Selected'] = (preset.codice === id));
    dettaglio.get('presets').patchValue(presets);
  }

  private handleQtaDateUpdate(rigaRilievo: GridRilieviObject) {
    const found = this.getDettaglioRilievo(rigaRilievo);
    const dettaglio: FormGroup = found.fg;
    dettaglio.get('dataOraRilievo').patchValue(rigaRilievo.QtaDate);
    dettaglio.get('qtaRilevataString').patchValue(rigaRilievo.QtaDate.toLocaleDateString(this.language));
    rigaRilievo.Qta = rigaRilievo.QtaDate.toLocaleDateString(this.language);
    this._rows[found.index].Qta = rigaRilievo.QtaDate.toLocaleDateString(this.language);
    this._rows[found.index].QtaDate = rigaRilievo.QtaDate;
  }

  private handleQtaBoolUpdate(rigaRilievo: GridRilieviObject) {
    const found = this.getDettaglioRilievo(rigaRilievo);
    const dettaglio: FormGroup = found.fg;
    dettaglio.get('qtaRilevata').patchValue(+rigaRilievo.Qta);
    dettaglio.get('qtaRilevataString').patchValue(rigaRilievo.Qta);
    this._rows[found.index].Qta = rigaRilievo.Qta;
  }

  private getUdM(cod: number): UnitaDiMisura {
    if (this._indiceUdM.has(cod)) return this._indiceUdM.get(cod);
    return new UnitaDiMisura(cod, '');
  }

  private getTipoControlloPerUdM(udmCod: number): Observable<BaseCodeDescr> {
    const tipoDefault = new BaseCodeDescr(enum_TipoControllo.NUMERO_DECIMALE);
    if (this._indiceUdM.has(udmCod)) {
      let tipoControllo = this._indiceUdM.get(udmCod).tipoControllo;
      return of((tipoControllo.codice === 0) ? tipoDefault : tipoControllo);
    } else {
      return this.udmLoaded$.pipe(
        filter(x => !!x),
        map(load => {
          if (load) return this._indiceUdM.get(udmCod).tipoControllo;
          else return tipoDefault;
        }));
    }
  }

  public getEsercizioDes(App_Nome: string, SA_NOME: string): string {

    let Esercizio_Des_Concatenato: string = "";

    if(App_Nome && App_Nome !== ""){
      Esercizio_Des_Concatenato = App_Nome;

      if (SA_NOME && SA_NOME !== "") {
        Esercizio_Des_Concatenato += ' - ' + SA_NOME;
      }
    }

    return Esercizio_Des_Concatenato;
  }

  public GetCodRilievo(lav_cod: number, obj: any, dettaglioRilievo: DettaglioRilievo): string{

    let codRilievo: string = "";

    switch (lav_cod) {
      case enum_LAVCOD.DANNI_RACCOLTA:
        if(obj){
          let indice = (obj as IndiceRilievo);

          codRilievo = indice.codice + "|" + indice.unitaDiMisura.codice;
        }else{
          if(dettaglioRilievo){
            codRilievo = dettaglioRilievo.dannoRaccolta + "|" + dettaglioRilievo.unitaDiMisura.codice;
          }
        }
        break;
      case enum_LAVCOD.FASI_FENOLOGICHE:
        if(obj){
          let ff = (obj as FaseFenologica);

          codRilievo = ff.codice + "|" + ff.stadioCrescitaBBCH.codice + "|" + ff.stadio;
        }else{
          if(dettaglioRilievo && dettaglioRilievo.faseFenologica){
            codRilievo = dettaglioRilievo.faseFenologica.codice + "|" + dettaglioRilievo.faseFenologica.stadioCrescitaBBCH.codice + "|" + dettaglioRilievo.faseFenologica.stadio;
          }
        }
        break;
      case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
        if(obj){
          let avv = (obj as Avversita);

          codRilievo = avv.codice + "|" + avv.gruppo.codice + "|" + avv.unitaDiMisura.codice;
        }else{
          if(dettaglioRilievo && dettaglioRilievo.avversitaGruppo){
            codRilievo = dettaglioRilievo.avversitaGruppo.codice + "|" + dettaglioRilievo.avversitaGruppo.gruppo.codice + "|" + dettaglioRilievo.unitaDiMisura.codice;
          }
        }
        break;
      case enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA:
        if(obj){
          let indice = (obj as IndiceRilievo);

          codRilievo = indice.codice + "|" + indice.unitaDiMisura.codice;
        }else{
          if(dettaglioRilievo){
            codRilievo = dettaglioRilievo.indiceResa + "|" + dettaglioRilievo.unitaDiMisura.codice;
          }
        }
        break;
      case enum_LAVCOD.RILIEVO_INDICI_MATURITA:
        if(obj){
          let indice = (obj as IndiceRilievo);

          codRilievo = indice.codice + "|" + indice.unitaDiMisura.codice;
        }else{
          if(dettaglioRilievo){
            codRilievo = dettaglioRilievo.indiceMaturita + "|" + dettaglioRilievo.unitaDiMisura.codice;
          }
        }
        break;
      case enum_LAVCOD.RILIEVO_ERBE_INFESTANTI:

        let av_gru: number = 0;

        let av_cod: number = 0;

        if(obj){
          let avv = (obj as AvversitaRilievo);

          if(avv.gruppo)
            av_gru = avv.gruppo.codice;

          av_cod = avv.codice;
        }else{
          if(dettaglioRilievo){
            if(dettaglioRilievo.erbaInfestante){

              if(dettaglioRilievo.erbaInfestante.classType = "Avversita"){
                av_cod = (dettaglioRilievo.erbaInfestante as Avversita).codice;
              }else if(dettaglioRilievo.erbaInfestante.classType = "GruppoAvversita"){
                av_gru = (dettaglioRilievo.erbaInfestante as GruppoAvversita).codice;
              }

            }
          }
        }

        codRilievo = av_gru + "|" + av_cod;
        break;
        case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:

          if(obj){

            let avv = (obj as AvversitaTrappole);

            let list_distinct_pro_cod: Array<number> = [];

            let udm_cod: number = 0;

            if(avv.utilizzi && avv.utilizzi.length > 0){
              avv.utilizzi.forEach(u=>{
                let pro_cod:number = u.risorsaProdotto.prodotto.codice;

                if(!list_distinct_pro_cod.includes(pro_cod))
                  list_distinct_pro_cod.push(pro_cod);
              });
            }

            if(avv.unitaDiMisura && avv.unitaDiMisura.codice > 0)
              udm_cod = avv.unitaDiMisura.codice;

            codRilievo = avv.codice + "|" + udm_cod;

            if(list_distinct_pro_cod.length > 0)
              codRilievo += "|" + list_distinct_pro_cod.join("|");

          }else{
            if(dettaglioRilievo) {
              codRilievo = dettaglioRilievo.avversitaGruppo.codice + "|" + dettaglioRilievo.unitaDiMisura.codice + "|" + dettaglioRilievo.risorsaProdotto?.prodotto.codice;
            }
          }
          break;
    }

    return codRilievo;
  }

  private getDescFaseFenologica(faseFenologica: FaseFenologica): string{

    let desc:string = "";

    if(faseFenologica){

      desc = faseFenologica.descrizione;

      if(faseFenologica.stadio)
        desc += " ( BBCH " + faseFenologica.stadio + " )";
    }

    return desc;
  }

  public getRowsRilieviAvversitaTrappole(eserciziCDC: EsercizioCDC[],avversitaGruppo: AvversitaGruppo,avversitaTrappole: AvversitaTrappole): Observable<Map<string, UtilizzoAvversitaTrappole[]>>{

    const TrappoleInstallatePerProdotto = new Map<string, UtilizzoAvversitaTrappole[]>();

    if(avversitaTrappole && avversitaTrappole.utilizzi && avversitaTrappole.utilizzi.length > 0){
      avversitaTrappole.utilizzi.forEach(u=>{

        if(u.impianto){
          const key = u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "|" +
            u.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice + "|" + u.impianto.primaryKey.appezzamentoPK.codice
            + "|" + u.impianto.primaryKey.codice;

          if(TrappoleInstallatePerProdotto.has(key)){
            let utilizziTrappoleInseriti: UtilizzoAvversitaTrappole[] = TrappoleInstallatePerProdotto.get(key);

            utilizziTrappoleInseriti.push(u);

            TrappoleInstallatePerProdotto.set(key, utilizziTrappoleInseriti);
          }else{
            TrappoleInstallatePerProdotto.set(key, [u]);
          }
        }

      });

      return of(TrappoleInstallatePerProdotto);
    }else{

      if (!eserciziCDC.length) {
        return of(TrappoleInstallatePerProdotto);
      }

      const leggiNumeroTrappole = <Leggi_Numero_Trappole>{
        eserciziCDC: eserciziCDC,
        avversitaGruppo: avversitaGruppo
      };

      return this.agendaService.Ottieni_Numero_Trappole_Registrate(leggiNumeroTrappole).pipe(take(1),
        map((R:rispostaStandard<any>) => {

          if(R.RispostaOK){
            let numeroTrappole = R.RispostaStringa;

            numeroTrappole.forEach(r => {
              const key = r.Piva + "|" + r.Sa_Cod + "|" + r.Appezza
                + "|" + r.id_reg;
              const value = r.Fr_Des + " (" + r.Dose + ")";
              TrappoleInstallatePerProdotto.set(key, null);
            });
          }

          return TrappoleInstallatePerProdotto;
        })
      );

    }

    return of(TrappoleInstallatePerProdotto);
  }

  public getDescrizioneProdottoRilievo(risorsaProdotto: RisorsaProdotto): string{

    let descr: string = "";

    if(risorsaProdotto && risorsaProdotto.prodotto){

      if(risorsaProdotto.prodotto){
        descr = risorsaProdotto.prodotto.descrizione;
      }

      if(risorsaProdotto.unitaDiMisuraIndicata){

        let QtaStr:string = this.qdcService.decimalpipe.transform(risorsaProdotto.quantitaTotaleReale,this.qdcService.digitsInfo_QdC_4_Decimal,this.qdcService.locale_id);

        descr += " ("+ QtaStr + " " + risorsaProdotto.unitaDiMisuraIndicata.simbolo;
      }

      descr += ")";
    }

    return descr;

  }

}
