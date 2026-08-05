import {Injectable} from '@angular/core';
import {TranslocoService} from '@jsverse/transloco';
import {CentroAziendale, CentroDropdownLists} from 'app/Model/anagrafiche/CentroAziendale';
import {Impresa} from 'app/Model/anagrafiche/Impresa';
import {AGRODATAFINE, AGRODATAINIZIO} from 'app/Model/CostantiPersonalizzate';
import {map} from 'rxjs';
import {Observable} from 'rxjs/internal/Observable';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {AjaxAgronicaService} from '../ajax-agronica.service';
import {AnagraficaClient} from '../api.service';
import {MasterService, rispostaStandard} from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {UtilizzoTerreno} from '../../Model/metaschema/utilizzi/UtilizzoTerreno';
import {IndirizzoAssociato} from '../../Model/anagrafiche/addresses/IndirizzoAssociato';

export class  LeggiCentriAziendali {
  impresa: Impresa;
  utilizzoTerreno: UtilizzoTerreno;
  data: Date;
  filtra_validita_esercizi?:boolean;
}

export class LeggiIndirizziCentro {
  piva: string;
  sa_cod: number;

  constructor(p: string, cod: number) {
    this.piva = p;
    this.sa_cod = cod;
  }
}

export class LeggiCentriConFiltroUtente{
  PrimaRiga_Flag: boolean;
  PrimaRiga_Text: string;
  PrimaRiga_Value: string;
  Piva: string;
  Flag_SoloCentriAttivi: boolean;
  Tipo_Value: number;
}

@Injectable({
  providedIn: 'root'
})
export class CentriAziendaliService {

  constructor(
    private ajaxAgronicaApiService: AjaxAgronicaAPIService,
    private translocoService: TranslocoService
  ) { }

  private AggiungiRigaVuotaCentri(flagPrimaRiga: boolean, centri: CentroAziendale[],piva: string){
    if(flagPrimaRiga && centri) {
      const centroPrimaRiga = new CentroAziendale({codice: 0, partitaIva: piva} as any);
      centroPrimaRiga.nome = this.translocoService.translate('TuttiICentriAziendali');
      centri.unshift(centroPrimaRiga);
    }
  }

  public leggiCentriAziendaliModelloQdC(p: LeggiCentriAziendali, flagPrimaRiga: boolean){
    return new Promise<CentroAziendale[]>(async (resolve, reject) => {

      this.ajaxAgronicaApiService.ajaxAPIPost<LeggiCentriAziendali, CentroAziendale[]>('AnagraficaNG/LeggiCentriAziendaliModello', p, false).pipe(map(R => {
        const centri: CentroAziendale[] = R.RispostaStringa;

        this.AggiungiRigaVuotaCentri(flagPrimaRiga, centri, p.impresa.partitaIva);

        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  Leggi_Centri_Aziendali_perSpecieImpianti(p: LeggiCentriAziendali, flagPrimaRiga: boolean){
    return new Promise<CentroAziendale[]>(async (resolve, reject) => {
      this.ajaxAgronicaApiService.ajaxAPIPost<LeggiCentriAziendali, CentroAziendale[]>('AnagraficaNG/Leggi_Centri_Aziendali_perSpecieImpianti', p, false).pipe(map(R => {
        const centri=R.RispostaStringa;
        this.AggiungiRigaVuotaCentri(flagPrimaRiga,centri,p.impresa.partitaIva);
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  leggiCentroAziendale(partitaIva: string, codice: number) {
    const objPAgenda = new ObjParametriAgenda();
    objPAgenda.Piva = partitaIva;
    objPAgenda.Sa_Cod = codice;

    const linkCentro = 'AnagraficaNG/LeggiCentro';
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, CentroAziendale>(linkCentro, objPAgenda);
  }

  leggiCentroDropDownLists(partitaIva: string, codice: number) {
    const objPAgenda = new ObjParametriAgenda();
    objPAgenda.Piva = partitaIva;
    objPAgenda.Sa_Cod = codice;

    const linkDdls = 'AnagraficaNG/Leggi_Centro_Dropdowns';
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, CentroDropdownLists>(linkDdls, objPAgenda);
  }

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Centri_Archivio_Lettura',
      objParametriAgenda);
  }

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Centri_Archivio_Scrittura',
      objParametriAgenda)
  }

  leggiCentri(objParametri: ObjParametriAgenda): Observable<any[]> {
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, any[]>('AnagraficaNG/Centri', objParametri).pipe(
      map(r => r.RispostaStringa)
    )
  }

  leggiCentro(objParametri: ObjParametriAgenda): Observable<CentroAziendale>{
    return this.ajaxAgronicaApiService.ajaxAPIPost<ObjParametriAgenda, CentroAziendale>('AnagraficaNG/LeggiCentro', objParametri).pipe(
      map(r =>
        r.RispostaStringa)
    )
  }

  scriviCentro(centro: CentroAziendale): Observable<CentroAziendale>{
    return this.ajaxAgronicaApiService.ajaxAPIPost<CentroAziendale, CentroAziendale>('AnagraficaNG/ScriviCentro', centro).pipe(
      map(r => r.RispostaStringa)
    );
  }

  manageAgenda(agenda: ObjParametriAgenda): ObjParametriAgenda {
    if(agenda?.Piva == null || agenda?.Piva == '') {
      return null;
    }

    agenda.Validita_Inizio = AGRODATAINIZIO;
    agenda.Validita_Fine = AGRODATAFINE;
    return agenda;
  }

  public readCentreAddresses(params: LeggiIndirizziCentro): Observable<rispostaStandard<IndirizzoAssociato[]>> {
    return this.ajaxAgronicaApiService.ajaxAPIPost<LeggiIndirizziCentro, IndirizzoAssociato[]>(
      'AnagraficaNG/readCentreAddresses',
      params,
      false
    );
  }

  public LeggiCentriConFiltroUtente(p: LeggiCentriConFiltroUtente): Observable<any>{
    return this.ajaxAgronicaApiService.ajaxAPIPost<LeggiCentriConFiltroUtente, any>('AnagraficaNG/LeggiCentriConFiltroUtente', p).pipe(
      map(r =>
        r.RispostaStringa)
    )
  }
}
