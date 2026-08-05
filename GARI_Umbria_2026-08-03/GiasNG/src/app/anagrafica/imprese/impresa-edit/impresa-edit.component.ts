import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {FormGroup} from '@angular/forms';
import { Router } from '@angular/router';
import { Contatto, PK } from 'app/Model/anagrafiche/Contatto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { RisorseUmane } from '../../../Model/anagrafiche/RisorseUmane';
import { FormeGiuridiche } from 'app/Model/metaschema/FormeGiuridiche';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  CodiciNazioniISO3166,
  Comune,
  Enum_DBTypeOperation, GiasDropDownTemplateComponent, GiasDropDownTemplateSComponent,
  GiasMultiSelectTemplateService, GiiasMultiselectTemplateSComponent,
  ObjParametriAgenda,
  Provincia, RadioButtonValue
} from 'gias-ui-kit';
import { CODICI_TOKEN } from 'app/Utility/Template/codici-template/models/codici.model';
import {delay, lastValueFrom, map, of, Subscription, switchMap, take, tap} from 'rxjs';
import { MasterService } from '../../../Service/master.service';
import { ObjParametriAgendaService } from '../../../Service/obj-parametri-agenda.service';
import { ImpresaEditService } from './impresa-edit.service';
import { Location } from '@angular/common';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { IMPRESE_SERVICE_TOKEN, ImpreseFactoryService } from 'app/Service/ServiceFactory/imprese.factory.service';
import { ImpreseServiceProvider } from 'app/Service/ServiceFactory/imprese.factory.provider';
import { ContattiRootService } from './contatti-edit/contattiRoot.service';
import { BaseCodeDescr } from '../../../Model/baseClass/baseCodeDescr';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { TranslocoService } from '@jsverse/transloco';
import { CooperativeConfigService } from './cooperative-edit/cooperative-edit-config.service';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { GruppoRaccolta } from '../../../Model/metaschema/GruppoRaccolta';
import { GruppiRaccoltaService } from '../../../Service/GruppiRaccolta/gruppi-raccolta.service';
import {enum_CodiciAnagrafe, enum_salva_in, enum_TipoImpresaGerarchia} from '../../../Model/TipiEnumerativi';
import { LeggiVincoli, VincoliService } from 'app/Service/DPI/vincoli.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Vincolo } from 'app/Model/metaschema/Vincoli';
import {AddressModel} from '../../../Model/anagrafiche/addresses/address.model';
import {LatLng} from '../../../Model/GIS/Utility';
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { Indirizzo } from 'app/Model/anagrafiche/addresses/Indirizzo';
import {RapportoContabile} from '../../../Model/anagrafiche/RapportoContabile';

@Component({
  standalone: false,
  selector: 'gias-impresa-edit',
  templateUrl: './impresa-edit.component.html',
  styleUrls: ['./impresa-edit.component.scss'],
  providers: [
    { provide: CODICI_TOKEN, useClass: ImpresaEditService },
    GiasMultiSelectTemplateService,
    CooperativeConfigService,
    ImpreseServiceProvider
  ],
})

export class ImpresaEditComponent implements OnInit, OnDestroy {

  get companyAddresses(): AddressModel[] {
    return this._companyAddresses;
  }

  impresaSelezionata: boolean;
  objParametriAgenda: ObjParametriAgenda;
  formEnable: boolean;

  FormeGiuridiche: FormeGiuridiche[];
  GruppiRaccolta: GruppoRaccolta[];
  DisciplinareAziendaliPredefiniti: any[];
  Tecnici: Contatto[];
  Padri: ImpresaPadre[];
  ListaCertificazione: BaseCodeDescr[];
  defaultItemContatto = { primaryKey: { partitaIva: '', codice: '' }, descrizione: '' };
  defaultItem = { codice: 0, descrizione: '' };
  defaultItemDisciplinare = { value: 0, text: '' };

  impreseForm: FormGroup;

  TipoImpresa: Array<RadioButtonValue> = [
    { name: this.transloco.translate('Impresa'), value: enum_TipoImpresaGerarchia.Impresa, enable: true },
    { name: this.transloco.translate('Cooperativa'), value: enum_TipoImpresaGerarchia.Cooperativa, enable: true },
    { name: this.transloco.translate('Consorzio'), value: enum_TipoImpresaGerarchia.Consorzio, enable: true },
    { name: this.transloco.translate('OrganizzazioneProduttoreSigla'), value: enum_TipoImpresaGerarchia.OrganizzazioneProduttore, enable: true },
  ];

  data_inizio: Date = AGRODATAINIZIO;
  data_fine: Date = AGRODATAFINE;
  private impresa: Impresa;
  private formCaricaDati: Subscription;
  private valueChangesSub: Subscription;
  private mostraContatti: boolean;
  private _companyAddresses: AddressModel[] = [];

  constructor(
    private masterService: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    @Inject(CODICI_TOKEN) private impresaEditService: ImpresaEditService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private contattiRootService: ContattiRootService,
    private funzioniComuniService: FunzioniComuniService,
    private giasMessageService: GiasMessageService,
    private location: Location,
    private router: Router,
    private transloco: TranslocoService,
    private gruppiRaccoltaservice: GruppiRaccoltaService,
    private vincoliService: VincoliService,
    private ContattiClientService: ContattiService
  ) {  }

  ngOnInit() {
    this.impreseForm = this.impresaEditService.GetFormGroupImpresa();
    this.valueChangesSub = this.impreseForm.valueChanges.subscribe(form => {
      this.impresaEditService.updateForm(form);
    });
    this.initDati();
    this.contattiRootService.setSalvaInPadre(false);
    this.contattiRootService.setCreaContattiPubblici(false);
  }

  ngOnDestroy(): void {
    if (this.formCaricaDati) {
      this.formCaricaDati.unsubscribe();
    }
    if (this.valueChangesSub) {
      this.valueChangesSub.unsubscribe();
    }
  }

  saveAndNew() {
    this.impreseForm.statusChanges.pipe(
      delay(300),
      take(1),
      tap((state) => {
        if (this.impreseForm.valid) {
          this.onSubmit().pipe(
            tap(r => {
              if (r.RispostaOK) {
                this.giasMessageService.successMessage(
                  this.transloco.translate('Impresa') + ': ' + r.RispostaStringa.ragioneSociale + ' ' + this.transloco.translate('ModificataCorrettamente')
                );
                this.masterService.set_isLoading({ message: '', isLoading: false });
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                this.objParametriAgenda.Piva = '';
                let currentUrl = this.router.url;
                this.router.routeReuseStrategy.shouldReuseRoute = () => false;
                this.router.onSameUrlNavigation = 'reload';
                this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
                this.router.navigate([currentUrl]);
              }
            })
          ).subscribe();
        }
      })
    ).subscribe();

    this.impreseForm.markAllAsTouched();
    this.impreseForm.setValue(this.impreseForm.getRawValue());
  }

  save() {
    this.impreseForm.statusChanges.pipe(
      delay(300),
      take(1),
      tap((state) => {
        if (this.impreseForm.valid) {
          this.onSubmit().pipe(
            tap((r) => {
              if (r.RispostaOK) {
                this.giasMessageService.successMessage(this.transloco.translate('Impresa') + ': ' + r?.RispostaStringa.ragioneSociale + ' ' + this.transloco.translate('ModificataCorrettamente'));
                this.location.back();
                this.masterService.set_isLoading({ message: '', isLoading: false });
              }
            })
          ).subscribe();
        }
      })
    ).subscribe();

    this.impreseForm.markAllAsTouched();
    this.impreseForm.setValue(this.impreseForm.getRawValue());
  }

  certificazioneChanged(ddlEl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddlEl, lastValueFrom(this.impreseService.leggi_Certificazioni().pipe(take(1))))
  }

  gruppoRaccoltaChanged(ddlEl: GiasDropDownTemplateComponent | GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    let objParams: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    objParams.Data = new Date();
    UtilityFunctions.loadDropDownItems(
      <GiasDropDownTemplateSComponent>ddlEl,
      lastValueFrom(this.gruppiRaccoltaservice.leggiGruppiRaccoltaValidi(objParams).pipe(take(1)))
    );
  }

  disciplinareAziendalePredefinitoChanged(ddlEl: GiasDropDownTemplateComponent | GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    let intervallo: IntervalloTemporale = new IntervalloTemporale();

    let validita: LeggiVincoli = {
      validita: intervallo
    }
    UtilityFunctions.loadDropDownItems(
      <GiasDropDownTemplateSComponent>ddlEl,
      lastValueFrom(this.vincoliService.leggiVincoliOrdered(intervallo)
        .pipe(take(1), map(x => this.adattaVincoli(x)))));
  }

  onAddressNotesChange(v: string, idx: number): void {
    this._companyAddresses[idx].note = v;
  }

  onAddressFractionChange(v: string, idx: number): void {
    this._companyAddresses[idx].fraction = v;
  }

  onAddressStreetChange(v: string, idx: number): void {
    this._companyAddresses[idx].street = v;
  }

  onAddressZipChange(v: string, idx: number): void {
    this._companyAddresses[idx].zipCode = v;
  }

  onAddressCityChange(v: Comune, idx: number): void {
    this._companyAddresses[idx].city = v;
  }

  onAddressProvinceChange(v: Provincia, idx: number): void {
    this._companyAddresses[idx].province = v;
  }

  onAddressCountryChange(v: CodiciNazioniISO3166, idx: number): void {
    this._companyAddresses[idx].country = v;
  }

  onGeolocationChange(v: LatLng, idx: number): void {
    this._companyAddresses[idx].gelocation = {lat:v.lat,lng:v.lng, isValid: v.isValid};
  }

  showGeolocation(): boolean {
    return true;
  }

  private initDati() {
    this.loadDefaultAddress().pipe(
      switchMap((indirizzi: Indirizzo) => {
        return this.impresaEditService.caricaDati(indirizzi);
      })
    ).subscribe((data) => {
      this.objParametriAgenda = data.ObjParamAgenda;
      this.impresa = data.Impresa;
      this.initImpresa(data.Impresa);

      this.FormeGiuridiche = data.FormeGiuridiche;
      this.Tecnici = data.Tecnici;
      this.Padri = data.Padri;
      this.abilitaDisabilitaForm();

      this.impresaSelezionata = true;
      this.masterService.setCompanyHeader(data.Impresa.ragioneSociale);
      this.mostraContatti = !this.isImpresaSuperUser();
      data.Impresa.indirizzi[0].indirizzo.geolocation = this.getGeolocationFromCodes();
      this._companyAddresses = data.Impresa.indirizzi.map(AddressModel.fromIndirizzoAssociato);
    });
  }

  public loadDefaultAddress() {
    return this.ContattiClientService.leggiIndirizzo();
  }

  private abilitaDisabilitaForm(): void {
    const operazione: Enum_DBTypeOperation = this.objParametriAgenda.TipoOperazioneDB;
    this.formEnable = operazione !== Enum_DBTypeOperation.Read;
  }

  private initImpresa(impresa: Impresa) {
    this.impreseForm.patchValue(impresa);
    const operazione = this.objParametriAgenda.TipoOperazioneDB;
    if (operazione == Enum_DBTypeOperation.Read) {
      this.impreseForm.disable({ emitEvent: false });
    } else {
      this.impreseForm.enable({ emitEvent: false });
    }
  }

  private onSubmit() {
    if (this.impreseForm.valid && (this.funzioniComuniService.codiciValidiNoDate(this.impresaEditService.codici).length == 0)) {
      let impresa: Impresa = this.impreseForm.getRawValue();
      impresa.codici = this.impresaEditService.codici;

      const risorseUmaneArr = this.contattiRootService.getRowsContatti().map(r => {
        let ris_um: RisorseUmane = new RisorseUmane();
        ris_um.codice = 0;
        ris_um.rapportoContabile = new RapportoContabile(r['codice'], r['descrizione']);
        ris_um.attivita = r['attivita'];
        ris_um.settore = r['settore'];
        return ris_um;
      });

      impresa.indirizzi = this._companyAddresses.map(a => a.mapToIndirizzoAsociato());
      impresa.indirizzi.forEach(i => {
        if (!i.indirizzo.istatComune.prov) {
          i.indirizzo.istatComune.prov = '000';
        }
        if (!i.indirizzo.istatComune.com) {
          i.indirizzo.istatComune.com = '000';
        }
      });

      impresa.contatti.forEach(c => {
        c = new Contatto();
        if (this.contattiRootService.getSalvaInPadre()) {
          c.primaryKey = new PK(impresa.impresaPadre[0].partitaIva, impresa.partitaIva);
        } else {
          c.primaryKey = new PK("", impresa.partitaIva);
        }
      })

      impresa.contattoAzienda = impresa.contattoAzienda || {};
      const contattoAzienda = this.contattiRootService.getContattoAzienda();
      impresa.contattoAzienda.contattoPubblico = this.contattiRootService.getCreaContattiPubblici() == null ? false : this.contattiRootService.getCreaContattiPubblici();

      impresa.contattoAzienda.proprietarioContattoAzienda = (contattoAzienda && contattoAzienda.proprietarioContattoAzienda != null)
        ? contattoAzienda.proprietarioContattoAzienda
        : enum_salva_in.SalvaInAziendaSU;

      impresa.contattoAzienda.aziendaCorrentePIVA = this.objParametriAgendaService.getObjParamValue().Piva;

      impresa.contattoAzienda.risorseUmane = risorseUmaneArr.map(ris_um => {
        return {
          ...ris_um,
          contatto: ris_um.contatto
            ? {
                ...ris_um.contatto,
                data_Nascita: ris_um.contatto.data_Nascita
                  ? new Date(ris_um.contatto.data_Nascita)
                  : null
              }
            : undefined
        } as any;
      });

      this.impresaEditService.updateForm(impresa);
      this.masterService.set_isLoading({ message: '', isLoading: true });
      return this.impreseService.ScriviImpresa(impresa, true);
    } else {
      return of(null);
    }
  }

  private isImpresaSuperUser(): boolean {
    return this.impresa.partitaIva == this.masterService.objP_server.PivaSuperUser
  }

  private adattaVincoli(vincoli: Vincolo[]) {
    for (let elem of vincoli) {
      let codiceAdattato = this.adattaCodice(elem.codice);
      elem.codice = codiceAdattato;
    }

    return vincoli;
  }

  private adattaCodice(codice: string) {
    let split = codice.split('_');
    if (split.length > 1)
      return (split[1] + '/' + split[0]);
    else
      return codice;
  }

  private getGeolocationFromCodes(): Required<LatLng> {
    //Raffaele - aggiunto controllo per valori null
    const latCode = this.impresa.codici.find(c => c.codiceAnagrafe.codice === enum_CodiciAnagrafe.Latitudine.valueOf());
    const lngCode = this.impresa.codici.find(c => c.codiceAnagrafe.codice === enum_CodiciAnagrafe.Longitudine.valueOf());
    const lat = latCode && latCode.valore != null ? Number.parseFloat(latCode.valore.replace(',', '.')) : 0;
    const lng = lngCode && lngCode.valore != null ? Number.parseFloat(lngCode.valore.replace(',', '.')) : 0;
    return  {lat: lat, lng: lng, isValid: true};
  }
}
