import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { Campo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard } from '../master.service';
import { Impresa } from "../../Model/anagrafiche/Impresa";
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {catchError, lastValueFrom, map, Observable, of} from 'rxjs';
import {DatiPrevisionaliColture, DatiPrevisionaliColtureRequest} from '../../Model/anagrafiche/DatiPrevisionaliColture';
import { AnagraficaClient, CentroAziendale as CentroAziendaleQdCA, Impresa as ImpresaQdCA, LeggiSpecieQdC, Lavorazione } from '../api.service';
import { ConsiglioIrrigazione, LeggiConsiglioIrrigazione } from 'app/Model/attivita/dettagli/DettaglioIrrigazione';
import { IMPIANTI_LNK } from 'gias-kendo-grid';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import {AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';

export class LeggiImpianto {
  disciplinare: Disciplinare;
  direttiva_nitrati: Disciplinare;
  centroAziendale: CentroAziendale;
  lavorazione: Lavorazione;
  lavorazioni: Lavorazione[];
  campo: Campo;
  data: Date;
  utilizzoTerreno: UtilizzoTerreno;
  consideraTerrenoNudo: boolean;
  dettagliTerrenoNudo: boolean;
  id_agenda_list: number[];
  ricetta_operazione_cod_list: number[];
  tipo_ricetta: number;
  tipo_attivita: number;
  stato: number;
  tipo_operazione_db: number;
  veg_cod: number;
  dest_cod: number;
  impresa: Impresa;
  filtra_validita_esercizi?: boolean;

  static fromPiva(piva: string) {
    const instance = new LeggiImpianto();
    instance.impresa = new Impresa(piva);
    instance.centroAziendale = CentroAziendale.Empty(piva);
    instance.campo = Campo.Empty(piva);
    instance.id_agenda_list = [];
    instance.lavorazioni = [];
    instance.ricetta_operazione_cod_list = [];
    return instance;
  }
}

export class LeggiMacchineIrrigazione{
  piva: string;
  sa_cod: number;
  appezza: number;
  id_reg: number;
}

export class IrrigazioneImpianto{
  codice: string;
  descrizione: string;
  flagIsMacchina: boolean;
  imp_cod: number;
  efficienza: number;
  portata: number;
}

@Injectable({
  providedIn: 'root'
})

export class ImpiantiService {

  constructor(
    private masterService: MasterService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private translocoService: TranslocoService,
    private anagraficaClient: AnagraficaClient
  ) { }

  CaricaImpianti(p: LeggiImpianto) {
    return new Promise<string>(async (resolve, reject) => {

      this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiImpianto, string>('AnagraficaNG/CaricaGridImpianti', p, false).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();

    });
  }

    CaricaConsiglio(p: LeggiConsiglioIrrigazione) {
    return new Promise<ConsiglioIrrigazione[]>(async (resolve, reject) => {

      this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiConsiglioIrrigazione, ConsiglioIrrigazione[]>('Agenda/LeggiConsigliIrrigazione', p).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();

    });
  }

  Leggi_SpecieVegetali_Attive_Impianti(p: LeggiImpianto, flagPrimaRiga: boolean, descrizioneRigaVuota: string) {
    return new Promise<Specie[] | DestinazioneUso[]>((resolve, reject) => {
      const List = [];

      const R = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiImpianto, UtilizzoTerreno[]>('AnagraficaNG/LeggiSpecieVegetaliAttiveImpianti', p).pipe(map(R => {
        const risp = <UtilizzoTerreno[]>R.RispostaStringa;

        risp.forEach((u: any) => {
          if (u.classType === 'Varieta') {
            List.push(u.specie);
          } else if (u.classType === 'DestinazioneUso') {
            List.push(u);
          }
        });

        if (flagPrimaRiga) {
          const SpecieAttivePrimaRiga = new Specie(-1);
          SpecieAttivePrimaRiga.descrizione = this.translocoService.translate(descrizioneRigaVuota);
          List.unshift(SpecieAttivePrimaRiga);
        }

        resolve(List);
      })).subscribe();

    });
  }

  getSpecieQdCA(centroAziendale: CentroAziendaleQdCA, data: Date, impresa: ImpresaQdCA, consideraTerrenoNudo: boolean,operazioni: Lavorazione[]): Promise<Specie[] | DestinazioneUso[]> {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiSpecieQdC, UtilizzoTerreno[]>('Anagrafica/LeggiSpecieVegetaliQdC', {
      centroAziendale: centroAziendale,
      data: data,
      impresa: impresa,
      consideraTerrenoNudo: consideraTerrenoNudo,
      soloAttiviAllaData: true,
      lavorazioni: operazioni
    }).pipe(
      map(x => {
        const res = x.RispostaStringa?.map((u: any) => u.classType === 'Varieta' ? u.specie : u) ?? [];

        const specieAttivePrimaRiga = new Specie(-1);
        specieAttivePrimaRiga.descrizione = this.translocoService.translate("NessunaSelezione");
        res.unshift(specieAttivePrimaRiga);

        return res;
      }))
    );
  }

  public readDatiPrevisionaliColture(params: DatiPrevisionaliColtureRequest): Observable<rispostaStandard<DatiPrevisionaliColture>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<DatiPrevisionaliColtureRequest, DatiPrevisionaliColture>(
      '/AnagraficaNG/readDatiPrevisionaliColture', params,
      false, false, true, false
    );
  }

  leggiPerCentroSpecie(
    piva: string, sa_cod: number = 0, veg_cod: number = 0,
    startDate: Date = AGRODATAINIZIO, endDate: Date = AGRODATAFINE
  ): Observable<BaseCodeDescrStr[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      IMPIANTI_LNK, {
      piva: piva,
      Sa_Cod: sa_cod,
      Veg_Cod: veg_cod,
      Data_Inizio: startDate,
      Data_Fine: endDate
    }).pipe(
      map(r => r.RispostaStringa as any[]),
      map(imp => imp.map(i => new BaseCodeDescrStr(i.chiave, i.des)))
    );
  }

  Leggi_Macchine_Irrigazione_Impianto(p: LeggiMacchineIrrigazione): Observable<IrrigazioneImpianto[]>{
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiMacchineIrrigazione, IrrigazioneImpianto[]>(
      '/AnagraficaNG/LeggiMacchineIrrigazioneImpianto', p,
      false, false, true, false
    ).pipe(map(r=>{return r.RispostaStringa}));

  }

  public LeggiMacchinaIrrigazioneDefault(p: LeggiMacchineIrrigazione): Observable<IrrigazioneImpianto>{
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiMacchineIrrigazione, IrrigazioneImpianto>(
      '/AnagraficaNG/LeggiMacchinaIrrigazioneDefault', p,
      false, false, true, false
    ).pipe(map(r=>{return r.RispostaStringa}));
  }


  LeggiImpiantoIrrigazione(p: number): Observable<IrrigazioneImpianto>{
    return this.ajaxAgronicaAPIService.ajaxAPIGet<number, IrrigazioneImpianto>(
      '/AnagraficaNG/LeggiImpiantoIrrigazione',
      p,
      false,
      false,
      true,
      false
    ).pipe(map(r=>{
      return r.RispostaStringa
    }));
  }
}
