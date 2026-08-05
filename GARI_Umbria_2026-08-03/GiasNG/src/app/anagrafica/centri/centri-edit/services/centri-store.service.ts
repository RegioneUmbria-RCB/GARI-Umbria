import { Injectable } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AspxDropdownItem, CentroAziendale, CentroAziendaleNG, CentroDropdownLists, PKCentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValori } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { RubricaVoci, RubricaVociConChiave } from 'app/Model/anagrafiche/RubricaVoci';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DropdownListItem, KendoGridRow } from 'gias-kendo-grid';
import { BehaviorSubject, forkJoin, Subject, take } from 'rxjs';
import { GiasCentriDDLItem } from '../utils';
import { GridPublicService } from 'gias-kendo-grid';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Rubrica } from 'app/Model/anagrafiche/Rubrica';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { AjaxAgronicaAPIService } from "../../../../Service/ajax-agronica.api.service";
import { CentriAziendaliService } from "../../../../Service/Anagrafica/centri.service";
import { enum_CodiciAnagrafe } from 'app/Model/TipiEnumerativi';
import { capValidatorCentriEdit, ObjParametriAgenda } from 'gias-ui-kit';

@Injectable({providedIn: 'root' })
export class EditCentroStore extends BehaviorSubject<CentroAziendaleNG>  {
  rubricaPubService: GridPublicService;

  codiciNewElementIndex: number;
  rubricaNewElementIndex: number;

  mainForm: FormGroup;
  centroPK: PKCentroAziendale;
  rubriche: RubricaVoci[];

  public tipologia: GiasCentriDDLItem[];
  public titolo_Di_Possesso: GiasCentriDDLItem[];
  // public ddls: BehaviorSubject<CentroAziendaleAltriDati>;
  public codici: DropdownListItem[];
  public tipoAttivita: GiasCentriDDLItem[];
  public organismiControllo: GiasCentriDDLItem[];
  public otes: GiasCentriDDLItem[];
  public centroAziendaleEsternoCollegato: GiasCentriDDLItem[];

  // Used from the view.
  public stati: GiasCentriDDLItem[];
  public comune: GiasCentriDDLItem[];
  public provincie: GiasCentriDDLItem[];

  public codiciDataReady: Subject<{ codici: CodiciAnagrafeValori[], ddl: DropdownListItem[] }> = new Subject();
  public rubricaDataReady: Subject<{ rubrica: RubricaVociConChiave[] }> = new Subject();

  public codiciPubService: GridPublicService;

  constructor(
    private objParams: ObjParametriAgendaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private agenda: ObjParametriAgendaService,
    private masterService: MasterService,
    private centriService: CentriAziendaliService
  ) {
    super(null);
  }

  leggiDatiCreaNuovoCentro(partitaIva: string) {
    const objPAgenda = new ObjParametriAgenda();
    objPAgenda.Piva = partitaIva;

    const linkDdls = 'AnagraficaNG/Leggi_Centro_Dropdowns';
    const letturaDdls = this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, CentroDropdownLists>(linkDdls, objPAgenda)
      .pipe(take(1)).subscribe((data) => {
        let centroAziendale = CentroAziendale.getEmptyCentroAziendale(partitaIva);
        let dropdownlists = data.RispostaStringa;
        centroAziendale['codiceOperatore'] = dropdownlists.CodiceOperatore;

        this.codiciNewElementIndex = centroAziendale.codici.length;
        this.rubricaNewElementIndex = centroAziendale.rubricaVoci?.length ?? 0;

        this.initializeGridsAndForms(centroAziendale);
        this.creaCentroInizializzaDatiAccessori();

        let tuttiIdati: CentroAziendaleNG = {
          DatiVisibili: dropdownlists,
          Centro: centroAziendale
        }

        this.manageDropdowns(tuttiIdati);
        this.next(tuttiIdati);

        this.codiciDataReady.next( { codici: this.value.Centro.codici, ddl: this.codici })
        this.rubricaDataReady.next( { rubrica: this.value.Centro.rubricaVoci as RubricaVociConChiave[] });
      });
  }

  private _leggiDatiModificaCentro(partitaIva: string, codice: number) {
    //const letturaCentro = this.centriService.leggiCentroAziendale(partitaIva, codice);
    const letturaDdls = this.centriService.leggiCentroDropDownLists(partitaIva, codice);
    //const letturaCentro = this.ajaxAPI.ajaxAPIPost<ObjParametriAgenda, CentroAziendale>('AnagraficaNG/LeggiCentro', objPAgenda);
    const objPAgenda = this.objParams.getObjParamValue();
    const letturaCentro = this.centriService.leggiCentro(objPAgenda);

    forkJoin([letturaCentro, letturaDdls]).pipe(take(1)).subscribe((data) => {
      let centroAziendale = data[0];
      let dropdownlists = data[1].RispostaStringa;
      let form = data[0] as CentroAziendale;
      form['codiceOperatore'] = dropdownlists.CodiceOperatore;

      this.codiciNewElementIndex = centroAziendale.codici.length;
      this.rubricaNewElementIndex = centroAziendale.rubricaVoci?.length ?? 0;

      this.initializeGridsAndForms(form);

      let tuttiIdati: CentroAziendaleNG = {
        DatiVisibili: dropdownlists,
        Centro: centroAziendale
      }

      this.manageDropdowns(tuttiIdati);
      this.next(tuttiIdati);

      this.codiciDataReady.next( { codici: this.value.Centro.codici, ddl: this.codici })
      this.rubricaDataReady.next( { rubrica: this.value.Centro.rubricaVoci as RubricaVociConChiave[] });
    });
  }

  initializeGridsAndForms(centro: CentroAziendale) {
    if (this.mainForm) {
      this.setCodiceOperatore(centro);
      centro.codici?.forEach((s, index) => s['chiave'] = index);
      centro.rubricaVoci?.forEach((s, index) => s['chiave'] = index);
      this.mainForm.patchValue(centro);
      this.rubriche = centro.rubricaVoci;
    }
  }

  private setCodiceOperatore(centro: CentroAziendale) {
    let codiceOperatore = centro.codici.filter(s => s['codiceAnagrafe'].codice == 1003);
    centro['codiceOperatore'] = codiceOperatore[0] == undefined ? '' : codiceOperatore[0].valore;
    let idx = centro.codici.findIndex(s => s['codiceAnagrafe'].codice == 1003);
    if (idx > -1) {
      centro.codici.splice(idx,1);
    }
  }

  creaCentroInizializzaDatiAccessori() {
    if (this.mainForm) {
      this.mainForm.controls['validita'].setValue({
        inizio: AGRODATAINIZIO,
        fine: AGRODATAFINE
      });
    }
  }

  manageDropdowns(httpRisp: CentroAziendaleNG) {
    let ddls = httpRisp.DatiVisibili;
    this.tipologia = ddls.Tipologia.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text, '', true));
    this.titolo_Di_Possesso = ddls.TitoloPossesso.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text, '', true));

    this.otes = ddls.Otes.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text));
    this.centroAziendaleEsternoCollegato = ddls.CentroAziendaleEsternoCollegato.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text));

    this.codici = ddls.Codici.Items.map(s => {
      let code = new DropdownListItem(+s.Value, s.Text);
      return code;
    })
    this.tipoAttivita = ddls.TipoAttivita.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text));
    this.organismiControllo = ddls.OrganismiControllo.Items.map(s => new GiasCentriDDLItem(s.Value, s.Text));

    this.setSelected(ddls, httpRisp.Centro);
  }

  saveCentroAziendale(centro: CentroAziendale) {
    let istat = centro.indirizzi[0].indirizzo.istatComune;
    if(typeof(istat.prov) === "object")
    {
      let comProv = istat.prov as GiasCentriDDLItem;
      istat.prov = comProv.codice as any;
      istat.comuni_prov = comProv.dati as any;
    }

    if(typeof(istat.com) === "object")
    {
      let com = istat.com as GiasCentriDDLItem;
      istat.com = com.codice as any;
      istat.localita = com.descrizione;
    }

    centro.codici = this.setCodiciPerScrittura();
    centro.rubricaVoci = this.setRubrichePerScrittura();

    this.masterService.set_isLoading({ message: '', isLoading: true });
    return this.centriService.scriviCentro(centro);
  }

  setRubrichePerScrittura(): RubricaVoci[] {
    let rubriche = this.rubricaPubService.value.data.rows;
    return rubriche.map(s => {
      let rubrica: RubricaVoci = {
        valore: s['valore'],
        rubrica: new Rubrica(this.getPrimitiveValue(s['codice'])),
        flag_cancellazione: s['flag_cancellazione']
      }
      rubrica.rubrica.tipologia = this.getPrimitiveValue(s['tipologia']) as any as string;
      return rubrica;
    });
  }

  setCodiciPerScrittura(): CodiciAnagrafeValori[] {
    let codici = this.codiciPubService.value.data.rows;
    this.pushCodiceOperatore(codici);
    return codici.map(s => {
      let mappedVal: CodiciAnagrafeValori = {
        codiceAnagrafe: new CodiceAnagrafe(this.getPrimitiveValue(s['codice'])),
        valore: s['valore'],
        validita: new IntervalloTemporale(s['dal'], s['al'])
      };
      mappedVal.codiceAnagrafe.descrizione = s['descrizione']

      return mappedVal;
    });
  }

  private pushCodiceOperatore(codici: KendoGridRow[]) {
    let codiceOperatore: KendoGridRow;

    codiceOperatore = {
      codice: enum_CodiciAnagrafe.CodiceCentro_Attuale,
      valore: this.mainForm.get('codiceOperatore').value,
      dal: AGRODATAINIZIO,
      al: AGRODATAFINE,
      descrizione: ''
    }

    codici.push(codiceOperatore);
  }

  getPrimitiveValue(codice: DropdownListItem | string) {
    if(typeof(codice) === 'object')
      return codice.id as any as number;
    return codice as any as number;
  }

  setSelected(data: CentroDropdownLists, centro: CentroAziendale) {
    centro.orientamentoTecnicoEconomico = data.Otes.Items.filter(s => s.Selected)
      .map(s => this.parseOrientamentoEconomico(s));

    let comuneSelezionata = data.Comune.Items.find(s => s.Selected);
    let com = getComuneForm(this.mainForm);
    if(comuneSelezionata)
      centro.indirizzi.length === 1 && com?.setValue(centro.indirizzi[0].indirizzo?.istatComune?.com);
    // com.setValue(comuneSelezionata.Value.substring(3,6));

    let provincieSelezionata = data.Provincie.Items.find(s => s.Selected);
    let prov = getComuniProvForm(this.mainForm);
    if(provincieSelezionata)
      centro.indirizzi.length === 1 && prov?.setValue(centro.indirizzi[0].indirizzo?.istatComune?.prov)
    //prov.setValue(provincieSelezionata.Value);


    let attivitaSelezionata = data.TipoAttivita.Items.find(s => s.Selected);
    let att = getAttivitaForm(this.mainForm);
    if(attivitaSelezionata)
      att?.setValue(centro.bioTipoAttivita);
    //att.setValue({ codice: attivitaSelezionata.Value, descrizione: attivitaSelezionata.Text });

    let odcSelezionato = data.OrganismiControllo.Items.find(s => s.Selected);
    let odc = getODC(this.mainForm);
    if(odcSelezionato)
      odc?.setValue(centro.bioOrganismoDiControllo);
    //odc.setValue({ codice: odcSelezionato.Value, descrizione: odcSelezionato.Text });
  }

  get inModifica() {
    const agenda = this.objParams.getObjParamValue();
    return !!agenda.Sa_Cod;
  }

  leggiDati() {
    const agenda = this.objParams.getObjParamValue();
    if(agenda.Sa_Cod)
      this._leggiDatiModificaCentro(agenda.Piva, agenda.Sa_Cod);
    else
      this.leggiDatiCreaNuovoCentro(agenda.Piva);
    return agenda;
  }

  getFormGroupIndirizzo(fb: FormBuilder) {
    return fb.group(
      {
        indirizzo: fb.group({
          cap: ['', [Validators.required, capValidatorCentriEdit()]],
          codice: [''],
          flag_cancellazione: [''],
          frazione: [''],
          istatComune: fb.group({
            cap: [''],
            codiceBelfiore: [''],
            com: ['', Validators.required],
            comuni_prov: [''],
            localita: [''],
            prov: ['', Validators.required],
            validita: ['']
          }),
          note: [''],
          stato: [''],
          via: [''],
        }),
        flag_cancellazione: [''],
        tipo_Indirizzo: ['']
      });
  }

  createFormGroup (fb: FormBuilder) {
    const indirizzi = fb.array([]);
    indirizzi.push(<any>this.getFormGroupIndirizzo(<any>fb));

    let ote: GiasCentriDDLItem[] = [];
    return fb.group({
      bioOrganismoDiControllo: [''],
      centroAziendaleEsternoCollegato: [''],
      bioTipoAttivita: [''],
      catastoCentroAziendale: [''],
      flag_cancellazione: [''],
      indirizzi: indirizzi,
      lat: fb.control(0),
      lng: fb.control(0),
      nome: ['', Validators.required],
      orientamentoTecnicoEconomico: ote,
      primaryKey: fb.group({
        codice: [''],
        partitaIva: ['']
      }),
      tipologia: new GiasCentriDDLItem('102', 'Sede Aziendale'),
      titolo_Di_Possesso: new GiasCentriDDLItem('0', 'Altro'),
      validita: fb.group({
        fine: [AGRODATAFINE],
        inizio: [AGRODATAINIZIO]
      }),

      codiceOperatore: fb.control(''),
    });
  }

  parseOrientamentoEconomico(s: AspxDropdownItem) {
    let result = { codice: s.Value as string, descrizione: s.Text }
    return result;
  }

  getForm(fb: FormBuilder): FormGroup {
    if(!this.mainForm)
      this.mainForm = this.createFormGroup(fb);
    return this.mainForm;
  }

  resetForm() {
    this.mainForm = null;
  }

  setCodiciGridPubService(codiciPubService: GridPublicService) {
    this.codiciPubService = codiciPubService;
  }
}
function getComuneForm(mainForm: FormGroup) {
  return (mainForm?.get('indirizzi') as FormArray)?.at(0)?.get('indirizzo')?.get('istatComune')?.get('com');
}

function getComuniProvForm(mainForm: FormGroup) {
  return (mainForm?.get('indirizzi') as FormArray)?.at(0)?.get('indirizzo')?.get('istatComune')?.get('prov');
}

function getAttivitaForm(mainForm: FormGroup) {
  return mainForm?.get('bioTipoAttivita');
}

function getODC(mainForm: FormGroup) {
  return mainForm?.get('bioOrganismoDiControllo');
}
