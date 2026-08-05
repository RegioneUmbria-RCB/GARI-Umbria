import { ElementRef, Injectable } from '@angular/core';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { IndirizzoAssociato } from '../../../Model/anagrafiche/addresses/IndirizzoAssociato';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { BioTipoAttivita } from 'app/Model/metaschema/BioTipoAttivita';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { LoadingService } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DropdownListItem, KendoGridRow } from 'gias-kendo-grid';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take, tap } from 'rxjs/operators';
import { CentriWrapper, CentroKendoServerResult as CentroGridServerResult, KendoCentroRow } from '../centri.models';
import { CentroAziendale as CentroAziendale_API } from '../../../Service/api.service';
import { AjaxAgronicaAPIService } from "../../../Service/ajax-agronica.api.service";
import {GiasIstatService} from '../../../Service/istat/gias-istat.service';


@Injectable({ providedIn: 'root' })
export class CentriService {

  private centri: CentroGridServerResult;

  constructor(
    private agendaService: ObjParametriAgendaService,
    private istatService: GiasIstatService,
    private centriService: CentriAziendaliService,
    private ajaxAPIService: AjaxAgronicaAPIService
  ) { }

  public leggiCentri(partitaIva: string): Observable<CentriWrapper> {
    if (this.centri) {
      return of(this.centri).pipe(
        map((c: any) => new CentriWrapper({
          centri: c,
          prov: this.istatService.getProvincie(),
          stati: this.istatService.getStati()
        }))
      );
    }
    const arrObs = [
      this.centriService.leggiCentri(this.agendaService.getObjParamValue())
    ];

    return forkJoin(arrObs).pipe(map((r: any) => new CentriWrapper({
      centri: r[0],
      prov: r[1],
      stati: r[2]
    })));
  }

  modificaCancellaCentroAziendale(row: KendoGridRow,
    flag_cancellazione: boolean,
    loadingService: LoadingService,
    gridElRef: ElementRef<any>,
    objCentro: CentroAziendale): Observable<any> {

    loadingService.set_isLoading({ isLoading: true, component: gridElRef });
    return this.ajaxAPIService.ajaxAPIPost('AnagraficaNG/ScriviCentro',
      this.getCentroParam(row, flag_cancellazione, objCentro)).pipe(take(1), tap(() => {
        loadingService.set_isLoading({ isLoading: false, component: gridElRef })
      }));
    // return this.AnagraficaNGClient.anagraficaNGScriviCentro(this.getCentroParam(row, flag_cancellazione, objCentro))
    // .pipe(take(1), tap(
    //     () => {loadingService.set_isLoading({ isLoading: false,
    //         component: gridElRef });}
    // ))
  }

  private getCentroParam(
    row: KendoGridRow,
    flag_cancellazione: boolean,
    objCentro: CentroAziendale
  ) {
    let agenda = this.agendaService.getObjParamValue();

    let indirizzo = {
      cap: row['CAP'] ?? '00000',
      codice: row['cod_indirizzo'] ?? 0,
      istatComune: {
        reg: row['reg'] ?? '000',
        com: row['com_cod_istat'] ?? '000',
        localita: row['com_des'] != 'Non Definita' ? row['com_des'] : '',
        comuni_prov: '',
        prov: row['pro_cod_istat'] ?? '000',

        codiceBelfiore: null,
        validita: null,
        cap: null
      },
      frazione: row['frz_des'],
      via: row['ind_des'],
      note: row['note'],
      stato: {
        codice: row['Stato_Cod'] ?? 'IT',

        descrizione: null,
        codiceAlpha3: null,
        codiceNumerico: null,
        gestioneGerarchia: 0
      },
      flag_cancellazione: flag_cancellazione,
      geolocation: {lat:0,lng:0, isValid: false}
    };

    let indirizzoAssociato: IndirizzoAssociato = {
      indirizzo: indirizzo,
      flag_cancellazione: flag_cancellazione,
      tipo_Indirizzo: 1
    };

    let centro: CentroAziendale;
    if (objCentro == null) {
      centro = {
        nome: row['sa_nome'], // row['sa_nome']
        indirizzi: [indirizzoAssociato],
        primaryKey: {
          codice: row['sa_cod'] ?? 0,
          partitaIva: row['Piva'] ?? agenda.Piva
        },
        bioTipoAttivita: new BioTipoAttivita(row['tipoAttivitaCod']),
        validita: new IntervalloTemporale(row['Validita_Inizio'], row['Validita_Fine']),
        flag_cancellazione: flag_cancellazione,

        tipologia: { codice: 102, descrizione: 'Sede Aziendale' },
        titolo_Di_Possesso: null,
        lat: 0,
        lng: 0,
        rubricaVoci: null,
        catastoCentroAziendale: null,
        codici: null,
        orientamentoTecnicoEconomico: null,
        bioOrganismoDiControllo: null,
        centroAziedaleEsternoCollegato: null
      };
    } else {
      centro = objCentro;
      centro.nome = row['sa_nome'];
      centro.indirizzi = [indirizzoAssociato];
      centro.bioTipoAttivita = new BioTipoAttivita(row['tipoAttivitaCod']);
      centro.validita = new IntervalloTemporale(row['Validita_Inizio'], row['Validita_Fine']);
      centro.flag_cancellazione = flag_cancellazione;
    }

    return centro as unknown as CentroAziendale_API;
  }

  getMemoryData(centri: CentroGridServerResult) {
    const provinci = this.istatService.getProvincie();
    const stati = this.istatService.getStati();

    return {
      centri: centri,
      provincie: provinci,
      stati: stati
    };
  }

  public addIfNotExists(ddlItems: DropdownListItem[], ddlItem: DropdownListItem): boolean {
    const exists = ddlItems.some((item) => item.id === ddlItem.id);
    return exists;
  }


  getCentro(id: string): KendoCentroRow {
    return this.centri.rows.find(row => row.chiave === id);
  }
}
