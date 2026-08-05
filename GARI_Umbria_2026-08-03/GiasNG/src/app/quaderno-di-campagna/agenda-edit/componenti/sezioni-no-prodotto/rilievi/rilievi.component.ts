import { Component, DoCheck, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { enum_TipoControllo } from 'gias-ui-kit';
import { DettaglioRilievo } from 'app/Model/attivita/dettagli/DettaglioRilievo';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { QdCRilieviService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/rilievi.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { EsercizioRilievoCDC } from '../../../../../Model/attivita/centri_di_costo/EsercizioRilievoCDC';
import { GridRilieviObject } from './rilievi.model';
import { AvversitaRilievo } from '../../../../../Model/metaschema/avversita/AvversitaRilievo';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { EsercizioCDC } from 'app/Model/attivita/centri_di_costo/EsercizioCDC';
import { TranslocoService } from '@jsverse/transloco';
import { faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import {
  CodiciXOperazione,
  DropdownListDisciplinare
} from '../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {
  GridImpiantoSelezionatoModel
} from 'app/quaderno-di-campagna/agenda-edit/quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import { Disciplinare } from '../../../../../Model/metaschema/Disciplinari';
import { MisureAvversitaAnagraficaService } from '../../../service/prodotti/misure-avversita-anagrafica.service';
import {
  MisureIndiciMaturitaAnagraficaService
} from "../../../service/prodotti/misure-indici-maturita-anagrafica.service";
import { Observable, take, tap, of, map, Subscription } from "rxjs";
import { MisuraAvversita } from "../../../../../Model/metaschema/avversita/MisuraAvversita";
import { cloneDeep } from 'lodash';
import { UnitaDiMisura } from "../../../../../Model/metaschema/UnitaDiMisura";
import { GiasDropDownTemplateComponent } from 'gias-ui-kit';
import { enum_LAVCOD } from 'app/Model/TipiEnumerativi';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AvversitaTrappole } from 'app/Model/attivita/dettagli/AvversitaTrappole';
import {UtilizzoAvversitaTrappole} from "../../../../../Model/attivita/dettagli/UtilizzoAvversitaTrappole";

const INDICE_MAT = enum_LAVCOD.RILIEVO_INDICI_MATURITA.toString();
const INDICE_AVV_CAMPO = enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO.toString();
const FASE_FEN = enum_LAVCOD.FASI_FENOLOGICHE.toString();
const DANNI_RACC = enum_LAVCOD.DANNI_RACCOLTA.toString();
const ERBE_INF = enum_LAVCOD.RILIEVO_ERBE_INFESTANTI.toString();
const INDICE_RESA = enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA.toString();
const INDICE_AVV_TRAPPOLE = enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE.toString();

@Component({
  standalone: false,
  selector: 'app-rilievi',
  templateUrl: './rilievi.component.html',
  styleUrls: ['./rilievi.component.css'],
  providers: [GiasDropDownTemplateService, QdCRilieviService]
})
export class RilieviComponent implements OnInit, DoCheck, OnDestroy {

  Subs: Subscription = new Subscription();

  @Input() Rilievo: FormGroup;
  @Input() TipoOp: string; //il tipo di operazione(Indice Maturità, Rilievo avversità...)

  @Input() Causali: FormControl;

  @ViewChild('ddl') ddl;
  @ViewChild('infoElementi') infoElementi;

  public BOOLEAN = enum_TipoControllo.CASELLA_SPUNTA;
  public DATE = enum_TipoControllo.CALENDARIO;
  public DDL = enum_TipoControllo.MENU_DISCESA;
  public TXT = enum_TipoControllo.CASELLA_TESTO;
  public TXT_AREA = enum_TipoControllo.AREA_TESTO;

  public form: FormGroup;
  public qtaType: number = 0;
  public DDLabel: string;
  public NumericLabel = this.transloco.translate("QtaRilevata") + " (*)";
  public DateLabel = this.transloco.translate("Data") + " (*)";
  public MultiSelectLabel = this.transloco.translate("Causale");
  public NoteLabel = this.transloco.translate("Note") + " (*)";
  public ComboLabel = this.transloco.translate("SelezionareUnaVoce") + " (*)";
  public NumeroAvveRilevateLabel = this.transloco.translate("NumMaxCatturaXTrap") + " (*)";
  /**
   * Informazioni riguardo gli elementi nelle ddl
   * es. In rosa è indicata la Fase Fenologica associata alla Fioritura, visualizzata nel QdC
   */
  public info: string;
  protected readonly faWarning = faTriangleExclamation;
  protected format = 'n0';
  private isDDLOpen = false;
  private isDPIValorized = false;

  private selectedOp: any;

  public isRilievoProdPrevista: boolean = false;

  constructor(
    private rilieviService: QdCRilieviService,
    private misureAvversitaService: MisureAvversitaAnagraficaService,
    private misureIndiciService: MisureIndiciMaturitaAnagraficaService,
    private qdcservice: QdCService,
    private giasDialogService: GiasDialogService,
    private transloco: TranslocoService
  ) { }

  public get showInfo(): boolean {
    return (+this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO && this.isDPIValorized)
      || +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.FASI_FENOLOGICHE;
  }

  public get isEdit(): boolean {
    return !this.qdcservice.Sola_Lettura_QdCForm();
  }

  public get isNumeric(): boolean {
    return this.qtaType === enum_TipoControllo.NUMERO_INTERO || this.qtaType === enum_TipoControllo.NUMERO_DECIMALE;
  }

  private get isRilievoFasiFen(): boolean {
    return +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.FASI_FENOLOGICHE;
  }

  private get isRilievoAvversitaCampo(): boolean {
    return +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO;
  }

  private get isRilievoRese(): boolean {
    return +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA;
  }

  private get isRilievoIndiciMaturita(): boolean {
    return +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_INDICI_MATURITA;
  }

  private get isRilievoAvversitaTrappole(): boolean {
    return +this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE;
  }

  ngOnInit(): void {
    this.initForm();
    this.rilieviService.Sezione_Rilievo = this.Rilievo;
    this.info = '';

    if (+this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO) {
      this.info = this.transloco.translate('InfoRilieviAvversita');
    } else if (+this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.FASI_FENOLOGICHE) {
      this.info = this.transloco.translate('InfoRilieviFasiFenologiche');
    }else if (+this.Rilievo.value.Operazione.primaryKey.codice === enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE) {
      this.info = this.transloco.translate('InfoRilieviAvversita');
    }
    this.handleDPI();

    this.qdcservice.caricaCausali(+this.Rilievo.value.Operazione.primaryKey.codice, this.qdcservice.TestataForm.get("Data").value);

    this.Subs.add(this.Causali.valueChanges.subscribe(value => {
      this.form.get('Causali').patchValue(this.Causali.getRawValue());
    }));

    this.Subs.add(
      this.qdcservice.TestataForm.get("Disciplinare")?.valueChanges.subscribe((d: DropdownListDisciplinare) => {

        if (this.ddl.id === INDICE_AVV_CAMPO && this.selectedOp) {

          /** Ricarico le Avversità al cambio del Disciplinare e ricarico anche gli eventuali valori di misureXAvversità (valori della DDL)
          * */

          this.loadOpAvv().pipe(take(1))
            .subscribe(list => {

              this.assignList(list);

              const index = list.findIndex(x => x['codRilievo'] === this.selectedOp.codRilievo);

              /**
               * Se il vecchio indice non è presente nel nuovo elenco imposto a null la combo.
               * Se invece è ancora presente l'indice aggiorno il valore del controllo da mostrare (DDL,Testo,ecc..)
               * **/
              if (index === -1) {
                this.selectedOp = null;
                this.form.get("NomeOp").patchValue(null);
              } else {
                this.selectedOp = list[index];
                this.form.get("NomeOp").patchValue(list[index]);
              }

              this.tryLoadCombo().pipe(take(1)).subscribe(() => this.setQtaControlType());
            });
        }

      })
    );
  }

  ngDoCheck(): void {
    let lavCod = +this.Rilievo.value.Operazione.primaryKey.codice;
    if (lavCod === enum_LAVCOD.FASI_FENOLOGICHE && this.infoElementi)
      this.infoElementi.nativeElement.style.backgroundColor = 'mistyrose';

    let ddlItems = document.getElementsByClassName("k-list-item ng-star-inserted");
    if (!ddlItems || !ddlItems.length || !this.showInfo || !this.isDDLOpen)
      return;

    for (let i = 0; i < ddlItems.length && i < this.ddl.listItems.length; i++) {
      if (lavCod === enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO
        && this.ddl.listItems[i].soglia === '1')
        ddlItems[i]['style'].backgroundColor = 'palegoldenrod';
      else if (lavCod === enum_LAVCOD.FASI_FENOLOGICHE
        && this.ddl.listItems[i].fioritura)
        ddlItems[i]['style'].backgroundColor = 'mistyrose';
    }
  }

  public onOpenDdl(ddlEl: GiasDropDownTemplateComponent) {
    ddlEl.loading = true;
    this.isDDLOpen = true;
    ddlEl.dropdownlist.closed.GiasSubscribe(() => this.isDDLOpen = false);
    if (ddlEl.id === INDICE_MAT)
      this.loadOpMat().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === INDICE_AVV_CAMPO)
      this.loadOpAvv().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === FASE_FEN)
      this.loadOpFen().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === INDICE_RESA)
      this.loadOpRes().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === DANNI_RACC)
      this.loadOpDan().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === ERBE_INF)
      this.loadOpErb().pipe(take(1))
        .subscribe(list => this.assignList(list));
    else if (ddlEl.id === INDICE_AVV_TRAPPOLE)
      this.loadOpAvvTrappole().pipe(take(1))
        .subscribe(list => this.assignList(list));
  }

  onOpChange(event) {
    this.selectedOp = event.data;
    this.tryLoadCombo().pipe(take(1)).subscribe(() => this.setQtaControlType());
  }

  public roundQta() {
    if (this.qtaType === enum_TipoControllo.NUMERO_INTERO) {
      this.form.get("Qta").patchValue(Math.round(this.form.get("Qta").value));
    }
  }

  isSubmitBtnDisabled() {
    return this.form.get("Qta").value === undefined
      || this.form.get("Qta").value === ''
      || (this.qdcservice.ImpiantiSelezionatiFormArray.value.length === 0 &&
          !this.qdcservice.RilievoSenzaImpianti());
  }

  //versione base -> compare una dialog che non fa nulla
  onSubmit() {
    let qta = this.parseQta();
    let dettaglioRilievoTemplate = this.getDettaglioRilievoTemplate(qta);
    let eserciziCDC =  this.qdcservice.GetEserciziCDCSelezionatiModel();

    if (this.isRilievoRese) {
      this.rilieviService.getStimeResa().subscribe(rese => {
        this.addDettagliRilievi(dettaglioRilievoTemplate,eserciziCDC, rese);
      });
      return;
    }else if(this.isRilievoAvversitaTrappole){

      let avversitaTrappole: AvversitaTrappole = this.form.get("NomeOp").getRawValue();

      this.rilieviService.getRowsRilieviAvversitaTrappole(eserciziCDC,dettaglioRilievoTemplate.avversitaGruppo,avversitaTrappole).subscribe(numeroTrappole => {
        this.addDettagliRilievi(dettaglioRilievoTemplate,eserciziCDC, null,numeroTrappole);
      });
      return;
    }

    this.addDettagliRilievi(dettaglioRilievoTemplate,eserciziCDC);
  }

  // PRIVATE FUNCTIONS

  /** Definisce l'operazione da eseguire al cambio dell'operazione */
  private setCodOp = (dettaglio: DettaglioRilievo) => null;

  private tryLoadCombo(): Observable<any> {
    if (this.selectedOp) {
      if (this.isRilievoAvversitaCampo) {
        return this.misureAvversitaService.getMisureAvversita(this.selectedOp.MxAV_Cod, this.qdcservice.getDisciplinareModelValue(+this.Rilievo.value.Operazione.primaryKey.codice))
          .pipe(tap(combo => this.setPresetsIfNotEmpty(combo)));
      } else if (this.isRilievoIndiciMaturita) {
        return this.misureIndiciService.getMisureIndiciMaturita(
          this.selectedOp.codice, this.selectedOp.unitaDiMisura?.codice, this.qdcservice.GetSpeciefromUtilizzoTerreno()
        ).pipe(
          map(combo => combo.map(i => MisuraAvversita.fromBaseCodDescr(i))),
          tap(combo => this.setPresetsIfNotEmpty(combo))
        );
      }
    }

    return of(null);
  }

  /**
   * Se vengono passati dei valori preset, imposta il menù a discesa come tipo di controllo da renderizzare.
   * Internamente, crea una copia dell'unità di misura per il dettaglio rilievo selezionato.
   * Ciò fa in modo che, se la stessa unità di misura è utilizzata con e senza preset per due diversi dettagli,
   * il tipo di controllo verrà renderizzato in modo diverso per i due dettagli senza creare conflitti.
   * @param combo eventuali valori preset per il rilievo
   * @private
   */
  private setPresetsIfNotEmpty(combo: MisuraAvversita[]) {
    if (combo && combo.length > 0) {
      let cpy: UnitaDiMisura = cloneDeep(this.selectedOp.unitaDiMisura);
      cpy.tipoControllo.codice = enum_TipoControllo.MENU_DISCESA;
      this.selectedOp.unitaDiMisura = cpy;
      this.selectedOp.presets = combo;
      this.qtaType = enum_TipoControllo.MENU_DISCESA;
    }
  }

  private setQtaControlType() {
    if (this.selectedOp) {
      this.qtaType = this.selectedOp.unitaDiMisura?.tipoControllo?.codice || enum_TipoControllo.NUMERO_DECIMALE;
      this.format = (this.qtaType == enum_TipoControllo.NUMERO_INTERO) ? 'n0' : 'n2';
      if (this.isRilievoFasiFen || this.qtaType === this.DATE) { // DATA
        this.form.get("Qta").setValue(this.qdcservice.TestataForm.value.Data);
      } else if (this.qtaType === this.DDL) {
        this.form.get("Qta").setValue('');
      } else if (this.qtaType === this.BOOLEAN) {
        this.form.get("Qta").setValue(1);
      } else if (this.qtaType === this.TXT || this.qtaType === this.TXT_AREA) {
        this.form.get("Qta").setValue("");
      } else { // NUMBER
        this.form.get("Qta").setValue(0);
      }
    } else {
      this.qtaType = 0;
    }
  }

  private getNomeCompletoEsercizio(progetto_cod: number): string {
    let impianti = this.qdcservice.ImpiantiForm.value.ImpiantiSelezionati;
    let centri = this.qdcservice.elencoCentriAziendali;
    let imp = impianti.find(i => i.Progetto_Cod === progetto_cod);
    if (imp) {
      let str = imp.App_Nome + ' - ' + imp.Cul_Des;
      let centro = centri.find(c => c.primaryKey.codice === imp.SA_COD);
      if (centro) {
        str = str + ' - ' + centro.nome;
      }
      return '\'' + str + "'";
    }
    return this.transloco.translate("TuttiGliImpianti");
  }

  private addDettagliRilievi(dettaglioRilievoTemplate:DettaglioRilievo,eserciziCDC: EsercizioCDC[], rese?:Map<string,string>, numeroTrappole?:Map<string,UtilizzoAvversitaTrappole[]>) {

    let error_found: boolean = false;

    if(eserciziCDC && eserciziCDC.length > 0){

      if(numeroTrappole){

        let newEserciziCDC: EsercizioCDC[] = [];

        eserciziCDC.forEach(esercizioCDC=>{
          let key = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "|"
            + esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice + "|"
            + esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice + "|"
            + esercizioCDC.esercizio.impiantoPK.codice;

          if(numeroTrappole.has(key)){
            newEserciziCDC.push(esercizioCDC);
          }
        });

        eserciziCDC = cloneDeep(newEserciziCDC);
      }

      for(let i = 0; i < eserciziCDC.length; i++){

        error_found = this.addDettaglioRilievoFG(eserciziCDC[i], dettaglioRilievoTemplate,rese,numeroTrappole);

        if(error_found)
          break;
      }
    }else{
      error_found = this.addDettaglioRilievoFG(null, dettaglioRilievoTemplate,rese,numeroTrappole);
    }

    if(!error_found){
      this.rilieviService.reloadRowsSignal$.next(null);
      this.manageActivities();
    }

  }

  private checkErrors(esercizioCDC: EsercizioCDC, template: DettaglioRilievo) {

    let error_found: boolean = false;

    let progetto_cod: number = 0;

    let title: string = this.transloco.translate("ErroreAggiuntaRilievo");

    let content: string = "";

    if(esercizioCDC)
      progetto_cod = esercizioCDC.esercizio.codice;

    let present = this.checkPresence(progetto_cod);

    if (present) {

      if(progetto_cod > 0){
        content = this.transloco.translate("ErroreRilevoGiaEseguitoSuImpianto", { Rilievo_Des: template.descrizione, Impianto_Des: this.getNomeCompletoEsercizio(progetto_cod) }) ;
      }else{
        content = this.transloco.translate("ErroreRilevoGiaEseguito", { Rilievo_Des: template.descrizione }) ;
      }

      this.giasDialogService.baseError(
        title,
        content,
        false
      );

      error_found = true;
    }

    if(!error_found){
      present = this.checkSpecie();

      if(present){
        content = this.transloco.translate("ErroreIndiciSpecieDifferenti");

        this.giasDialogService.baseError(
          title,
          content,
          false
        );

        error_found = true;
      }

    }

    return error_found;
  }

  private checkPresence(progetto_cod: number): GridRilieviObject {
    const rows = this.rilieviService.rows;
    if (!rows?.length) return undefined;

    let impiantoKey: string = "";
    const impianto: GridImpiantoSelezionatoModel = this.qdcservice.ImpiantiForm.value.ImpiantiSelezionati.find(i => i.Progetto_Cod === progetto_cod);

    if(impianto)
      impiantoKey = impianto.PIVA + "|" + impianto.SA_COD + "|" + impianto.APPEZZA + "|" + impianto.ID_REG + "|" + impianto.Progetto_Cod;

    return rows.find(row => row.Impianto_Key === impiantoKey &&
      row.Descriz_Key === this.selectedOp['codRilievo'] &&
      row.Op_Cod === +this.Rilievo.value.Operazione.primaryKey.codice);
  }

  private loadOpMat() {
    return this.rilieviService.leggiIndiciMaturita(
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice
    );
  }

  private loadOpAvv() {
    return this.rilieviService.leggiAvversita(
      this.Rilievo.value.Operazione.primaryKey.codice,
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice,
      this.qdcservice.getDisciplinareModelValue(+this.TipoOp)
    );
  }

  private loadOpAvvTrappole() {
    return this.rilieviService.leggiAvversitaTrappole(
      this.qdcservice.GetEserciziCDCModel()
    );
  }

  private loadOpFen() {
    return this.rilieviService.leggiFasiFenologiche(
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice
    );
  }

  private loadOpDan() {
    return this.rilieviService.leggiDanniRaccolta(
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice
    );
  }

  private loadOpRes() {
    return this.rilieviService.leggiIndiciReseRaccolta(
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice
    );
  }

  private loadOpErb() {
    return this.rilieviService.leggiErbeInfestanti(
      this.qdcservice.GetSpeciefromUtilizzoTerreno().codice
    );
  }

  private initForm(): void {
    this.form = new FormGroup([]);
    switch (this.TipoOp) {
      case INDICE_MAT:
        this.DDLabel = this.transloco.translate("Indice") + ":";
        this.setCodOp = (dettaglio) => dettaglio.indiceMaturita = this.selectedOp.codice;
        break;
      case INDICE_AVV_CAMPO:
        this.DDLabel = this.transloco.translate("Avversità") + ":";
        this.setCodOp = (dettaglio) => {
          dettaglio.avversitaGruppo = new AvversitaRilievo(this.selectedOp.codice);
          dettaglio.avversitaGruppo.gruppo = this.selectedOp.gruppo;
        };
        break;
      case FASE_FEN:
        this.DDLabel = this.transloco.translate("FaseFenologica") + ":";
        this.setCodOp = (dettaglio) => dettaglio.faseFenologica = this.selectedOp;
        break;
      case DANNI_RACC:
        this.DDLabel = this.transloco.translate("DannoRaccolta") + ":";
        this.setCodOp = (dettaglio) => dettaglio.dannoRaccolta = this.selectedOp.codice;
        break;
      case ERBE_INF:
        this.DDLabel = this.transloco.translate("ErbaInfestante") + ":";
        this.setCodOp = (dettaglio) => {
          dettaglio.erbaInfestante = this.selectedOp;
          dettaglio.unitaDiMisura = new UnitaDiMisura(10, ""); // piante per metroquadro
        };
        break;
      case INDICE_RESA:
        this.isRilievoProdPrevista = true;
        this.DDLabel = this.transloco.translate("IndiceResa") + ":";
        this.setCodOp = (dettaglio) => dettaglio.indiceResa = this.selectedOp.codice;
        break;
      case INDICE_AVV_TRAPPOLE:
        this.DDLabel = this.transloco.translate("Avversità") + ":";
        this.setCodOp = (dettaglio) => {
          dettaglio.avversitaGruppo = new AvversitaRilievo(this.selectedOp.codice);
          dettaglio.avversitaGruppo.gruppo = this.selectedOp.gruppo;
        };
    }
    this.form.addControl('NomeOp', new FormControl());
    this.form.addControl('Qta', new FormControl());

    //aggiungo causale
    this.form.addControl('Causali', new FormControl());
  }

  private parseQta(): string {
    switch (this.qtaType) {
      case enum_TipoControllo.NUMERO_DECIMALE:
        return this.form.get("Qta").value.toString();
      case enum_TipoControllo.NUMERO_INTERO:
        return Math.round(this.form.get("Qta").value).toString();
      case this.DATE:
        return (this.form.get("Qta").value as Date).toLocaleDateString();
      case this.BOOLEAN:
        return this.form.get("Qta").value ? "1" : "0";
      case this.DDL:
        /** La quantità reale viene assegnata in fase di salvataggio:
         {@link QdCFormToAttivitaService.getDettagliRilievi()} */
        return this.form.get("Qta").value.CodiceAnagrafica;
      case this.TXT:
      case this.TXT_AREA:
        return this.form.get("Qta").value.toString();
        break;
    }
  }

  private getDettaglioRilievoTemplate(qta: string): DettaglioRilievo {
    let dettaglioRilievoTemplate = new DettaglioRilievo();
    dettaglioRilievoTemplate.id_agenda = this.getCodiceAttivita();
    dettaglioRilievoTemplate.codRilievo = this.selectedOp.codRilievo;
    dettaglioRilievoTemplate.dataOraRilievo = this.qdcservice.QdCForm.get("Trattamento").get("Testata").get("Data").value;
    dettaglioRilievoTemplate.unitaDiMisura = this.selectedOp.unitaDiMisura;
    dettaglioRilievoTemplate.qtaRilevataString = qta.toString();

    switch (this.qtaType) {
      case this.DATE:
        dettaglioRilievoTemplate.dataOraRilievo = this.form.get("Qta").value;
        break;
      case this.DDL:
        dettaglioRilievoTemplate.presets = this.selectedOp.presets;
        dettaglioRilievoTemplate.qtaRilevata = +qta;
        break;
      default:
        dettaglioRilievoTemplate.qtaRilevata = +qta;
        break;
    }
    this.setCodOp(dettaglioRilievoTemplate);
    dettaglioRilievoTemplate.descrizione = this.selectedOp.descrizione;
    return dettaglioRilievoTemplate;
  }

  private getCodiceAttivita() {
    let cod = "0";
    if (!this.isRilievoFasiFen) {
      let codOp: CodiciXOperazione = this.qdcservice.TestataForm.get("Codici_Attivita").value
        .find((cxo: CodiciXOperazione) =>
          cxo.Operazione.primaryKey.codice === this.Rilievo.value.Operazione.primaryKey.codice
        );
      cod = codOp ? codOp.CodiceAttivita : cod;
    }
    return cod;
  }

  private manageActivities() {
    if (!this.isRilievoFasiFen)
      return;
    let idAttivita = this.qdcservice.TestataForm.get("Codici_Attivita").value as Array<CodiciXOperazione>;
    if (!idAttivita.map((cxo) => cxo.CodiceAttivita).includes('0')) {
      let newAttivita = new CodiciXOperazione(
        '0', '0', '0',
        this.rilieviService.Sezione_Rilievo.value.Operazione, null, null, "", "0", null
      );
      idAttivita.push(newAttivita);
      this.qdcservice.TestataForm.get("Codici_Attivita").patchValue(idAttivita);
    }
  }

  private handleDPI() {
    let dpi: Disciplinare = this.qdcservice.TestataForm.get('Disciplinare').value;
    this.isDPIValorized = (!dpi || !+dpi.codice || dpi.codice === '-999') ? false : true;
    this.qdcservice.TestataForm.get('Disciplinare').valueChanges.GiasSubscribe(dpiChange => {
      this.isDPIValorized = (!dpiChange || !+dpiChange.codice || dpiChange.codice === '-999') ? false : true;
    });
  }

  onSelectionCausale(item: any) {
    //tengo aggiornate le causali sul Form principale
    this.qdcservice.QdCForm.get('Trattamento').get('Causali').patchValue(this.form.get('Causali').getRawValue());
  }

  private onOpenCombo(ddlEl: GiasDropDownTemplateComponent) {
    ddlEl.listItems = this.selectedOp.presets;
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

  public ApplyDivClass(): string {
    if (this.qtaType === this.TXT || this.qtaType === this.TXT_AREA || this.qtaType === this.DDL || this.isNumeric) {
      return "col-12 col-sm-5 d-flex";
    } else if (this.qtaType === this.BOOLEAN) {
      return "col-12 col-sm-2 d-flex";
    } else {
      return "col-12 col-sm-2";
    }
  }


  private assignList(list: Array<any>) {
    this.ddl.listItems = list;
    this.ddl.loading = false;
  }

  private addDettaglioRilievoFG(esercizioCDC: EsercizioCDC,dettaglioRilievoTemplate:DettaglioRilievo, rese?:Map<string,string>, numeroTrappole?:Map<string,UtilizzoAvversitaTrappole[]>): boolean{

    if (this.checkErrors(esercizioCDC, dettaglioRilievoTemplate))
      return true;

    if(esercizioCDC){

      dettaglioRilievoTemplate.esercizioCDC = esercizioCDC as unknown as EsercizioRilievoCDC;

      const impianto: GridImpiantoSelezionatoModel = this.rilieviService.getImpianto(dettaglioRilievoTemplate);

      if(impianto)
        esercizioCDC.esercizio.descrizione = this.rilieviService.getEsercizioDes(impianto.App_Nome,impianto['SA_NOME']);

      let key: string = dettaglioRilievoTemplate.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "|"
        + dettaglioRilievoTemplate.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice + "|"
        + dettaglioRilievoTemplate.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice + "|"
        + dettaglioRilievoTemplate.esercizioCDC.esercizio.impiantoPK.codice;

      if(numeroTrappole){

        let utilizziTrappole: UtilizzoAvversitaTrappole[] = numeroTrappole.get(key);

        if(utilizziTrappole){

          utilizziTrappole.forEach(u=>{
            dettaglioRilievoTemplate.risorsaProdotto = u.risorsaProdotto;
            (this.Rilievo.get("DettagliRilievi") as FormArray).push(dettaglioRilievoTemplate.toFormGroup());
          });
        }
      }else{
        if (rese) {
          key = key + "|" + dettaglioRilievoTemplate.esercizioCDC.esercizio.codice;
          dettaglioRilievoTemplate.resaUltimoRilievo = rese.get(key);
        }

        (this.Rilievo.get("DettagliRilievi") as FormArray).push(dettaglioRilievoTemplate.toFormGroup());
      }
    }else{
      (this.Rilievo.get("DettagliRilievi") as FormArray).push(dettaglioRilievoTemplate.toFormGroup());
    }

    return false;

  }

  private checkSpecie(): GridRilieviObject {
    const rows = this.rilieviService.rows;

    if (!rows?.length) return null;

    let row = null;

    let specie: Specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();

    if(specie){

      let veg_cod: number = specie.codice;

      let righe_filtrate: GridRilieviObject[] = rows.filter(r=>r.SpecieVegetale.codice === veg_cod);

      if(righe_filtrate && rows.length !== righe_filtrate.length)
        row = rows[0];

    }

    return row;
  }

  public GetLabelNameQta(): string{

    let label = "";

    if(this.isRilievoAvversitaTrappole){
      label = this.NumeroAvveRilevateLabel;
    }else{
      label = this.NumericLabel;
    }

    return label;
  }

}
