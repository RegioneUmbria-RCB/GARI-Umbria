import { Injectable } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
import { FormeGiurificheService } from 'app/Service/Metaschema/forme-giuridiche.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ICodiciTemplateService } from 'app/Utility/Template/codici-template/services/codici-template.service';
import { KendoGridRow } from 'gias-kendo-grid';
import { HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { EMPTY, from, Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ContattiRootService } from './contatti-edit/contattiRoot.service';
import { CaricaDatiDto as ImpresaEditDto } from './imprese-codici-metadata';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {capValidatorAziendeEdit} from 'gias-ui-kit';
import {IntervalloTemporale} from '../../../Model/anagrafiche/IntervalloTemporale';
import {enum_CodiciAnagrafe} from '../../../Model/TipiEnumerativi';
import { Indirizzo } from 'app/Model/anagrafiche/addresses/Indirizzo';
import { TranslocoService } from '@jsverse/transloco';
import {enum_Impostazioni_Utenti} from '../../../Model/Impostazioni_Utenti.enum';
import {PermessiUtenteService} from '../../../Service/permessi-utente.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';

const itemIndex = (item: CodiciAnagrafeValoriChiave, data: CodiciAnagrafeValoriChiave[]): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx].chiave === item.chiave) {
      return idx;
    }
  }

  return -1;
};

@Injectable({ providedIn: 'root'})
export class ImpresaEditService implements ICodiciTemplateService {
  gridId = "ImpresaCodici";
  public gridPublicService: GridPublicService;

  public form: Impresa;
  public codici: CodiciAnagrafeValoriChiave[];

  constructor(
    private impreseService: ImpreseService,
    private parametriAgenda: ObjParametriAgendaService,
    private transloco: TranslocoService,
    private masterService: MasterService,
    private router: Router,
    private fb: FormBuilder,
    private formeGiuridicheService: FormeGiurificheService,
    private permessiUtenteService: PermessiUtenteService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private contattiRootService: ContattiRootService
  ) {}

  updateForm(form: Impresa) {
    this.form = form;
    this.form.codici = this.codici;
  }

  updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave) {
    switch(action) {
      case HttpAction.CREATE:
        const chiave = Math.max.apply(Math, this.codici.map(function (o) {
          return o.chiave;
        }));
        row.chiave = chiave + 1;
        this.codici.push(row);
        break;
      case HttpAction.UPDATE:
        const index = itemIndex(row, this.codici);
        this.codici.splice(index, 1, row);
        break;
      case HttpAction.REMOVE:
        const _index = itemIndex(row, this.codici);
        this.codici.splice(_index, 1);
        break;
    }
    this.gridPublicService.refresh(true);
  }

  private setRowsUniqueId(rows: CodiciAnagrafeValoriChiave[]) {
    let i = 0;
    rows.forEach((row) => {
      row.chiave = i++;
    });
  }

  postForm(): Observable<rispostaStandard<Impresa>> {
    return this.impreseService.ScriviImpresa(this.form, true);
  }

  leggiDropdowns(): Observable<CodiceAnagrafe[]> {
    const agenda = this.parametriAgenda.getObjParamValue();
    return this.impreseService.leggiImpreseCodici(agenda);
  }

  leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]> {
    return of(this.codici);
  }

  caricaDati(indirizzi: Indirizzo): Observable<ImpresaEditDto> {

    this.masterService.set_isLoading({
      isLoading: true,
      message: 'Caricamento in corso'
    });

    const agenda = this.parametriAgenda.getObjParamValue();
    if(agenda.TipoOperazioneDB == Enum_DBTypeOperation.Write)
      agenda.Piva = '';

    // we just care about the sementieri server, not the active sportello
    const data = this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.Is_Sementieri)
      .pipe(
        map(chiave => chiave?.Valore?.toLowerCase() === 'true'),
        switchMap(isSementieri => this.provaCaricareIDati(isSementieri, agenda, indirizzi)),
        map(response => {
          response.ObjParamAgenda = agenda;
          return response;
        }));

    this.masterService.set_isLoading({ isLoading: false });
    return data;
  }

  private provaCaricareIDati(isSementieri: boolean, agenda, indirizzo: Indirizzo): Observable<ImpresaEditDto> {
    let stato : string = indirizzo.stato.codice;
    let promises: Promise<any>[] = [
      this.formeGiuridicheService.leggiFormeGiurifiche(0),
      this.impreseService.leggiCombo_Tecnici(agenda.Piva),
      this.impreseService.leggiCombo_OrganismiDiControllo(agenda.Piva),
      this.impreseService.leggiPadri()
    ];

    if (agenda.Piva != '') {
      promises.push(this.impreseService.leggiImpresa(agenda));
    }

    if (isSementieri) {
      promises.push(this.impreseService.leggiPadreSementieri());
    }

    const data = from(Promise.all(promises)).pipe(map((dati): ImpresaEditDto => {
      let impresa: Impresa;

      const padri:ImpresaPadre[] = dati[3].filter(i => i.partitaIva !== agenda.Piva); // excludes the company itself
      if (agenda.Piva != "") {
        impresa = dati[4].RispostaStringa;
      } else {
        let padre: ImpresaPadre;

        if (isSementieri) {
          const impresaPadreSementieri = dati[4] as ImpresaPadre;
          padre = padri.find((el) => impresaPadreSementieri?.partitaIva == el.partitaIva);
        } else {
	      const defaultFatherCompany = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.ImpresaPadreDefault);

          if (defaultFatherCompany?.Valore != undefined && defaultFatherCompany.Valore != '') {
	        padre = padri.find((el) => defaultFatherCompany.Valore === el.partitaIva);
	      } else {
	        padre = padri.find((el) => this.masterService.objP_server.PivaSuperUser == el.partitaIva);
	      }
        }


        if (!padre) {
          padre = padri[0];
        }

        impresa = new Impresa();
        impresa.partitaIva = '';
        impresa.ragioneSociale = '';
        impresa.CUAA = '';
        impresa.certificazione = [];
        impresa.indirizzi = [
          {
            indirizzo: {
              codice: 0,
              via: '',
              frazione: '',
              istatComune: {
                reg: '000',
                prov: '000',
                com: '000',
                cap: '0000',
                localita: '',
                comuni_prov: '',
                validita: new IntervalloTemporale(),
                codiceBelfiore: ''
              },
              cap: '0000',
              stato: {
                codice: stato,
                descrizione: indirizzo.stato.descrizione,
                codiceNumerico: indirizzo.stato.codiceNumerico,
                codiceAlpha3: indirizzo.stato.codiceAlpha3,
                gestioneGerarchia: indirizzo.stato.gestioneGerarchia
              },
              note: '',
              flag_cancellazione: false,
              geolocation: {lat: 0, lng: 0, isValid: false}
            },
            tipo_Indirizzo: 1,
            flag_cancellazione: false
          }
        ];
        impresa.codici = [];
        impresa.contatti = [];
        impresa.impresaPadre = [padre];
        impresa.tipo_Impresa = 1;
      }

      //imresa.contatti per ogni contatto e poi per ogni risorsa umana aggiungere la row per le risorse umane
      if(impresa.contatti != undefined) {
        let kendoRows: KendoGridRow[] = [];

        impresa.contatti.forEach((contatto) => {
            if (contatto.risorseUmane != undefined) {
              contatto.risorseUmane.forEach((risorsa) => {
                const kendoRow: KendoGridRow = {
                  codice: risorsa.rapportoContabile.codice,
                  settore: risorsa.settore,
                  descrizione: risorsa.rapportoContabile.descrizione,
                  attivita: risorsa.attivita,
                  aziendaProprietaria: contatto.aziendaProprietaria,
                  visibilitaPubblica: contatto.visibilitaPubblica ? this.transloco.translate("Pubblico") : this.transloco.translate('Privato'),
                };
                kendoRows.push(kendoRow);
              });
            }
          }

        );
        this.contattiRootService.gridRowsContattiSource.next(kendoRows);
      }

      this.form = impresa;
      this.codici = impresa.codici.filter(
        c => ![enum_CodiciAnagrafe.Latitudine, enum_CodiciAnagrafe.Longitudine].some(e => e.valueOf() === c.codiceAnagrafe.codice)
      ) as CodiciAnagrafeValoriChiave[];
      this.setRowsUniqueId(this.codici);

      return {
        Impresa: impresa,
        FormeGiuridiche: dati[0],
        Tecnici: dati[1].RispostaStringa,
        Odc: dati[2].RispostaStringa,
        Padri: padri,
        ObjParamAgenda: agenda
      };
    }, catchError(() => EMPTY)));
    return data;
  }

  private partitaIvaRealeValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value;
    if (!value) return null;
    return /^[a-zA-Z0-9]{1,25}$/.test(value) ? null : { invalidPiva: true };
  }

  public GetFormGroupImpresa(): FormGroup {
    return this.fb.group({
      partitaIva: [''],
      partitaIvaReale: ['', this.partitaIvaRealeValidator],
      ragioneSociale: ['', Validators.required],
      forma_Giuridica: new FormControl({ codice: 0, descrizione: '' }),
      tipo_Impresa: [1],
      codici: this.fb.array([]),
      CUAA: [''],
      indirizzi: this.fb.array([this.getIndirizzoAssociatoForm()]),
      contatti: this.fb.array([]),
      impresaPadre: new FormControl([{ ragioneSociale: '', partitaIva: '' }]),
      tecnicoReferente: new FormControl({ codice: 0, descrizione: '' }),
      organismo_di_Controllo: new FormControl({ codice: 0, descrizione: '' }),
      certificazione: new FormControl([]),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      gruppoRaccolta: new FormControl({}),
      disciplinareAziendalePredefinito: new FormControl({}),
      contattoAzienda: new FormControl({})
    });
  }

  private getIndirizzoAssociatoForm() {
    return this.fb.group({
      tipo_indirizzo: [1],
      indirizzo: this.getIndirizzoForm()
    });
  }

  private getIndirizzoForm() {
    return this.fb.group({
      codice: [0],
      via: [''],
      frazione: [''],
      cap: ['00000', capValidatorAziendeEdit()],
      note: ['', Validators.maxLength(50)],
      stato: new FormControl({ codice: 0, descrizione: '' }),
      istatComune: this.fb.group({
        prov: ['000'],
        com: ['000']
      })
    });
  }
}
