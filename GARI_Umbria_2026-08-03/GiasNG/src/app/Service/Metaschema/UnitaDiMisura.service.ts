import { Injectable } from '@angular/core';
import { DettaglioTrattamento } from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { enum_UnitaMisura } from 'app/Model/TipiEnumerativi';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import {DettaglioFertilizzazione} from "../../Model/attivita/dettagli/DettaglioFertilizzazione";
import {DettaglioSemina} from "../../Model/attivita/dettagli/DettaglioSemina";
import {DoseEtichetta} from "../../Model/metaschema/DoseEtichetta";
import {Tipo_Ricetta} from "../../Model/attivita/Attivita";
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {map, Observable} from 'rxjs';
import { Tipo_Attivita } from 'gias-ui-kit';
import {AvversitaGruppo} from "../../Model/metaschema/avversita/AvversitaGruppo";

export class LeggiUnitaDiMisura{

  lavorazione: Lavorazione;

  elem_cod: number;

  avversita: AvversitaGruppo;

  tipo_Attivita: Tipo_Attivita;

  tipo_Ricetta: Tipo_Ricetta;

  dettaglioTrattamento: DettaglioTrattamento;

  dettaglioFertilizzazione: DettaglioFertilizzazione;

  dettaglioSemina: DettaglioSemina;

  doseEtichetta: DoseEtichetta;

  unitaDiMisura: UnitaDiMisura;
}

export class UdmScomposta{
  UDM_radice: number;
  perHa_hl: number;
  MoltiplicatoreDose: number;
}

export class UdmConvertitaToKG_L{
  Udm_Cod_Trasformato: number;
  Moltiplicatore: number;
}

@Injectable({
  providedIn: 'root'
})
export class UnitaDiMisuraService {

  constructor(private ajaxAgronicaService: AjaxAgronicaService,
              private APIService: AjaxAgronicaAPIService,

              private masterService: MasterService) { }

  /*Leggi_UnitaDiMisura_QdC_Old(p: LeggiUnitaDiMisura) {

      return new Promise<UnitaDiMisura[]>(async (resolve, reject) => {

          const parametri: CoreWS_Generic<LeggiUnitaDiMisura> = new CoreWS_Generic
          (
              this.masterService.getCoreWSGenericObjP(),
              p
          );

          const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<UnitaDiMisura[], LeggiUnitaDiMisura>(this.masterService.link_CoreWS + '/Metaschema/UnitaDiMisura.asmx/Leggi_UnitaDiMisura_QdC', parametri);

          resolve(R.RispostaStringa);
      });
  }*/

  Leggi_UnitaDiMisura_QdC(p: LeggiUnitaDiMisura) {

    return new Promise<UnitaDiMisura[]>(async (resolve, reject) => {

      this.APIService.ajaxAPIPost<LeggiUnitaDiMisura, UnitaDiMisura[]>('MetaschemaNG/LeggiUnitaDiMisuraQdC', p).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();

    });
  }

  /** Carica tutte le unità di misura disponibili.
   * @return una lista di unità di misura corredate di simbolo e tipo controllo
   */
  public Leggi_Tutte_UnitaDiMisura(): Observable<UnitaDiMisura[]> {
    return this.APIService.ajaxAPIGet<any, any>('Metaschema/TutteUnitaMisura', "")
      .pipe(map(r => r.RispostaOK ? r.RispostaStringa : []));
  }

  /*Recupera_UdM_da_FrCod_Old(p:LeggiUnitaDiMisura ){
      return new Promise<UnitaDiMisura>(async (resolve, reject) => {

          const parametri: CoreWS_Generic<LeggiUnitaDiMisura> = new CoreWS_Generic
          (
              this.masterService.getCoreWSGenericObjP(),
              p
          );

          const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<UnitaDiMisura, LeggiUnitaDiMisura>(this.masterService.link_CoreWS + '/Metaschema/UnitaDiMisura.asmx/Recupera_UdM_da_FrCod', parametri);

          resolve(R.RispostaStringa);
      });
  }*/

  Recupera_UdM_da_FrCod(p:LeggiUnitaDiMisura ){
    return new Promise<UnitaDiMisura>(async (resolve, reject) => {
      this.APIService.ajaxAPIPost<LeggiUnitaDiMisura, UnitaDiMisura>('MetaschemaNG/RecuperaUdMdaFrCod', p).pipe(map(R => {
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  //Funzioni dell' AgronicaCoreMetaSchemaDAL.UnitaMisura_R utili ma che non fanno letture lato server
  //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

  Converti(Udm_1: UnitaDiMisura, Quantita_da_Convertire: number, Udm_2: UnitaDiMisura){

    switch(Udm_1.codice){
      //partendo dal Tonnellate
      case  enum_UnitaMisura.Tonnellate:
      case enum_UnitaMisura.Tonnellate__HA:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return Quantita_da_Convertire * 1000000;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return Quantita_da_Convertire * 10;
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return Quantita_da_Convertire * 1000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return Quantita_da_Convertire * 1000000000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;

      //partendo dal Quintali
      case enum_UnitaMisura.Quintali:
      case enum_UnitaMisura.Ettolitro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return Quantita_da_Convertire * 100;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return Quantita_da_Convertire * 100000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return Quantita_da_Convertire * 100000000;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return Quantita_da_Convertire / 10
          default:
            console.log('Conversione non Gestita');
            break;
        }

        break;

      //partendo dal KG, KG__HA
      case enum_UnitaMisura.KG:
      case enum_UnitaMisura.KG__HA:
      case enum_UnitaMisura.Chilogrammi__HL:

        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return Quantita_da_Convertire * 1000
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return Quantita_da_Convertire / 100;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return Quantita_da_Convertire * 1000000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;

      //partendo dal Grammi , Grammi__HA
      case enum_UnitaMisura.Grammi:
      case enum_UnitaMisura.Grammi__HA:
      case enum_UnitaMisura.Grammi__HL:
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return Quantita_da_Convertire / 100000;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return Quantita_da_Convertire / 1000000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return Quantita_da_Convertire * 1000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;

      case enum_UnitaMisura.Grammi__Litro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case  enum_UnitaMisura.KG:
          case  enum_UnitaMisura.KG__HA:
          case  enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
          case  enum_UnitaMisura.Ettolitro:
            return this.Converti(new UnitaDiMisura(enum_UnitaMisura.Grammi__HL,"",""),Quantita_da_Convertire,Udm_2);
          case enum_UnitaMisura.Chilogrammi__HL:
          case enum_UnitaMisura.Milligrammi__HL:
          case enum_UnitaMisura.Grammi__HL:
            return this.Converti(new UnitaDiMisura(enum_UnitaMisura.Grammi__HL,"",""),Quantita_da_Convertire / 100,Udm_2);
          case enum_UnitaMisura.Milligrammi__Litro:
            return Quantita_da_Convertire * 1000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;

      case enum_UnitaMisura.Milligrammi__Litro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case  enum_UnitaMisura.KG:
          case  enum_UnitaMisura.KG__HA:
          case  enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
          case  enum_UnitaMisura.Ettolitro:
            return this.Converti(new UnitaDiMisura(enum_UnitaMisura.Milligrammi,"",""),Quantita_da_Convertire,Udm_2);
          case enum_UnitaMisura.Chilogrammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
          case enum_UnitaMisura.Grammi__HL:
            return this.Converti(new UnitaDiMisura(enum_UnitaMisura.Milligrammi__HL,"",""),Quantita_da_Convertire / 100,Udm_2);
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;

      //partendo dal Milligrammi
      case enum_UnitaMisura.Milligrammi:
      case enum_UnitaMisura.Milligrammi__HL:
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return Quantita_da_Convertire / 100000;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
            return Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
            return Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return Quantita_da_Convertire / 100000000;
          case enum_UnitaMisura.Tonnellate:
          case  enum_UnitaMisura.Tonnellate__HA:
            return Quantita_da_Convertire / 1000000000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;
      //partendo dal Millilitri
      case enum_UnitaMisura.Millilitri:
      case enum_UnitaMisura.CentimetriCubi:
      case  enum_UnitaMisura.Millilitri__Ha:
      case enum_UnitaMisura.CC__HL:
      case enum_UnitaMisura.Millilitri__HL:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return  Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Metri_Cubi:
            return   Quantita_da_Convertire / 1000000;
          case enum_UnitaMisura.Ettolitro:
            return  Quantita_da_Convertire / 100000;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;
      //partendo dal Litri
      case enum_UnitaMisura.Litri:
      case enum_UnitaMisura.Litro__HA:
      case enum_UnitaMisura.Litri__HL:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return   Quantita_da_Convertire * 1000;
          case enum_UnitaMisura.Metri_Cubi:
            return  Quantita_da_Convertire / 1000;
          case enum_UnitaMisura.Ettolitro:
            return  Quantita_da_Convertire / 100;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;
      //partendo dal Metri_Cubi
      case enum_UnitaMisura.Metri_Cubi:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return Quantita_da_Convertire * 1000000;
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return Quantita_da_Convertire * 1000;
          case enum_UnitaMisura.Ettolitro:
            return Quantita_da_Convertire * 10;
          default:
            console.log('Conversione non Gestita');
            break;

        }
        break;
      //partendo dal Metri_Cubi
      case enum_UnitaMisura.Ettolitro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return Quantita_da_Convertire * 100000;
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return Quantita_da_Convertire * 100;
          case enum_UnitaMisura.Metri_Cubi:
            return Quantita_da_Convertire / 10;
          default:
            console.log('Conversione non Gestita');
            break;
        }
        break;
      case enum_UnitaMisura.QUINTALI__HA:
        switch (Udm_2.codice) {
          case enum_UnitaMisura.KG__HA:
            return Quantita_da_Convertire * 100;
        }
    }
  }

  ScomponiUdm(UnitaMisura: UnitaDiMisura): UdmScomposta{

    let obj = <UdmScomposta>{
      UDM_radice: 0,
      perHa_hl: 0,
      MoltiplicatoreDose: 1
    };

    switch(UnitaMisura.codice){
      case enum_UnitaMisura.Millilitri__Litro:
        obj.UDM_radice = 101
        obj.perHa_hl = 2121
        obj.MoltiplicatoreDose = 100
        break;
      case enum_UnitaMisura.Grammi__Litro:
        obj.UDM_radice = 3
        obj.perHa_hl = 2121
        obj.MoltiplicatoreDose = 100
        break;
      case enum_UnitaMisura.CC__HL:
        obj.UDM_radice = 104
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Grammi__HL:
        obj.UDM_radice = 3
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Milligrammi__HL:
        obj.UDM_radice = 2032
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Millilitri__HL:
        obj.UDM_radice = 101
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Litri__HL:
        obj.UDM_radice = 29
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Chilogrammi__HL:
        obj.UDM_radice = 2
        obj.perHa_hl = 2121
        break;
      case enum_UnitaMisura.Grammi__HA:
        obj.UDM_radice = 3
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.Litro__HA:
        obj.UDM_radice = 29
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.KG__HA:
        obj.UDM_radice = 2
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.UNITA__HA:
        obj.UDM_radice = enum_UnitaMisura.UNITA
        obj.perHa_hl = 2123
        break;
      //TODO da verificare se è giusta l'UDM_radice
      case enum_UnitaMisura.METRI3__HA:
        obj.UDM_radice = -1
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.Millilitri__Ha:
        obj.UDM_radice = 101
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.NumUnita__HA:
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.Tonnellate__HA:
        obj.UDM_radice = 304
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.Tonnellate__HA_Spighe:
        obj.UDM_radice = 304
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.QUINTALI__HA:
        obj.UDM_radice = 4
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.Numero_Diffusori_HA:
        obj.UDM_radice = enum_UnitaMisura.Numero_Diffusori
        obj.perHa_hl = 2123
        break;
      case enum_UnitaMisura.KG:
        obj.UDM_radice = 2
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Quintali:
        obj.UDM_radice = 4
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Litri:
        obj.UDM_radice = 29
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Millilitri:
        obj.UDM_radice = 101
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Grammi:
        obj.UDM_radice =  3
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.CentimetriCubi:
        obj.UDM_radice = 104
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Tonnellate:
        obj.UDM_radice = 304
        obj.perHa_hl = 0
        break;
      case enum_UnitaMisura.Milligrammi:
        obj.UDM_radice = 2032
        obj.perHa_hl = 0
        break;
      //--------------------
      //concianti
      case 2004:   //l/100 kg di seme
        obj.UDM_radice = 29
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2005:   //ml/100 kg di seme
        obj.UDM_radice = 101
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2015:   //ml/100 kg di bulbilli
        obj.UDM_radice = enum_UnitaMisura.Millilitri
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2017:   //ml/100 kg di semi
        obj.UDM_radice = 101
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2021 :  //ml/kg di semente
        obj.UDM_radice = 101
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 5001003 :   //ml/unità di seme
        obj.UDM_radice = 101
        obj.perHa_hl = 0
        break;
      case 2006:   //g/unita' di seme
        obj.UDM_radice = 3
        obj.perHa_hl = 0
        break;
      case 2007:   //g/100 kg di semente
        obj.UDM_radice = 3
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2009:  //ml/1 t di prodotto
        obj.UDM_radice = enum_UnitaMisura.Millilitri
        obj.perHa_hl = enum_UnitaMisura.Tonnellate;
        break;
      case 2010:   //kg/100 kg di seme
        obj.UDM_radice = 2
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case 2026:  //kg/1 tonnellata di semente
        obj.UDM_radice = 2
        obj.perHa_hl = enum_UnitaMisura.Tonnellate;
        break;



      case 2030: //litri/1.000 piante
        obj.UDM_radice = 29
        obj.perHa_hl = 0
        break;
      case 2018: //grammi/pianta
        obj.UDM_radice = 3
        obj.perHa_hl = 0
        break;
      case 170: //ml/pianta
        obj.UDM_radice = 101
        obj.perHa_hl = 0
        break;

      // post raccolta
      case enum_UnitaMisura.Millilitri__Quintale:
        obj.UDM_radice = enum_UnitaMisura.Millilitri;
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case enum_UnitaMisura.KG__Quintale:
        obj.UDM_radice = enum_UnitaMisura.KG;
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case enum_UnitaMisura.Grammi__Quintale:
        obj.UDM_radice = enum_UnitaMisura.Grammi;
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case enum_UnitaMisura.Litri__Quintale:
        obj.UDM_radice = enum_UnitaMisura.Litri;
        obj.perHa_hl = enum_UnitaMisura.Quintali;
        break;
      case enum_UnitaMisura.Numero_Trappole:
        obj.UDM_radice = enum_UnitaMisura.Numero_Trappole
        obj.perHa_hl = 0
        break;
    }

    return obj;
  }

  ConvertiToKG_L(UnitaMisura: UnitaDiMisura): UdmConvertitaToKG_L{

    let obj = <UdmConvertitaToKG_L>{
      Moltiplicatore: 0,
      Udm_Cod_Trasformato: 0
    };

    switch(UnitaMisura.codice){
      case enum_UnitaMisura.Grammi:
        obj.Moltiplicatore = 0.001
        obj.Udm_Cod_Trasformato = 2
        break;
      case enum_UnitaMisura.Milligrammi:
        obj.Moltiplicatore = 0.000001
        obj.Udm_Cod_Trasformato = 2
        break;
      case enum_UnitaMisura.Quintali:
        obj.Moltiplicatore = 100
        obj.Udm_Cod_Trasformato = 2
        break;
      case enum_UnitaMisura.Tonnellate:
      case enum_UnitaMisura.Metri_Cubi:
        obj.Moltiplicatore = 1000
        obj.Udm_Cod_Trasformato = 2
        break;
      case enum_UnitaMisura.Millilitri:
      case enum_UnitaMisura.CentimetriCubi:
        obj.Moltiplicatore = 0.001
        obj.Udm_Cod_Trasformato = 29
        break;
      case enum_UnitaMisura.KG:
        obj.Moltiplicatore = 1
        obj.Udm_Cod_Trasformato = 2
        break;
      case enum_UnitaMisura.Litri:
        obj.Moltiplicatore = 1
        obj.Udm_Cod_Trasformato = 29
        break;

    }

    return obj;
  }


  Ottieni_Moltiplicatore(Udm_1: UnitaDiMisura, Udm_2: UnitaDiMisura):number{

    let moltiplicatore = 1;

    switch(Udm_1.codice){
      //partendo dal Tonnellate
      case  enum_UnitaMisura.Tonnellate:
      case enum_UnitaMisura.Tonnellate__HA:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return moltiplicatore = 1000000;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 10;
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return moltiplicatore = 1000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return moltiplicatore = 1000000000;
        }
        break;

      //partendo dal Quintali
      case enum_UnitaMisura.Quintali:
      case enum_UnitaMisura.Ettolitro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return moltiplicatore = 100;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return moltiplicatore = 100000;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return moltiplicatore = 100000000;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return moltiplicatore = 0.10;
        }

        break;

      //partendo dal KG, KG__HA
      case enum_UnitaMisura.KG:
      case enum_UnitaMisura.KG__HA:
      case enum_UnitaMisura.Chilogrammi__HL:
      // case 2026: // kg/1 tonnellata di semente:

        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
            return moltiplicatore = 1000
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 0.01;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return moltiplicatore = 0.001;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return moltiplicatore = 1000000;
        }
        break;

      //partendo dal Grammi , Grammi__HA
      case enum_UnitaMisura.Grammi:
      case enum_UnitaMisura.Grammi__HA:
      case enum_UnitaMisura.Grammi__HL:
      case 2007: // g/100 kg di semente
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return moltiplicatore = 0.001;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 0.00001;
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
            return moltiplicatore = 0.000001;
          case enum_UnitaMisura.Milligrammi:
          case enum_UnitaMisura.Milligrammi__HL:
            return moltiplicatore = 1000;
        }
        break;

      case enum_UnitaMisura.Grammi__Litro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case  enum_UnitaMisura.KG:
          case  enum_UnitaMisura.KG__HA:
          case  enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
          case  enum_UnitaMisura.Ettolitro:;
            return this.Ottieni_Moltiplicatore(new UnitaDiMisura(enum_UnitaMisura.Grammi__HL,"",""),Udm_2);
          case enum_UnitaMisura.Chilogrammi__HL:
          case enum_UnitaMisura.Milligrammi__HL:
          case enum_UnitaMisura.Grammi__HL:
            return this.Ottieni_Moltiplicatore(new UnitaDiMisura(enum_UnitaMisura.Grammi__HL,"",""),Udm_2);
          case enum_UnitaMisura.Milligrammi__Litro:
            return moltiplicatore = 1000;
        }
        break;

      case enum_UnitaMisura.Milligrammi__Litro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case  enum_UnitaMisura.KG:
          case  enum_UnitaMisura.KG__HA:
          case  enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Tonnellate:
          case enum_UnitaMisura.Tonnellate__HA:
          case  enum_UnitaMisura.Ettolitro:
            return this.Ottieni_Moltiplicatore(new UnitaDiMisura(enum_UnitaMisura.Milligrammi,"",""),Udm_2);
          case enum_UnitaMisura.Chilogrammi__HL:
          case enum_UnitaMisura.Grammi__Litro:
          case enum_UnitaMisura.Grammi__HL:
            return this.Ottieni_Moltiplicatore(new UnitaDiMisura(enum_UnitaMisura.Milligrammi__HL,"",""),Udm_2);
        }
        break;

      //partendo dal Milligrammi
      case enum_UnitaMisura.Milligrammi:
      case enum_UnitaMisura.Milligrammi__HL:
        switch(Udm_2.codice){
          case enum_UnitaMisura.KG:
          case enum_UnitaMisura.KG__HA:
          case enum_UnitaMisura.Chilogrammi__HL:
            return moltiplicatore = 0.00001;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
            return moltiplicatore = 0.001;
          case enum_UnitaMisura.Grammi:
          case enum_UnitaMisura.Grammi__HA:
          case enum_UnitaMisura.Grammi__HL:
            return moltiplicatore = 0.001;
          case enum_UnitaMisura.Quintali:
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 0.00000001;
          case enum_UnitaMisura.Tonnellate:
          case  enum_UnitaMisura.Tonnellate__HA:
            return moltiplicatore = 0.000000001;
        }
        break;
      //partendo dal Millilitri
      case enum_UnitaMisura.Millilitri:
      case enum_UnitaMisura.CentimetriCubi:
      case  enum_UnitaMisura.Millilitri__Ha:
      case enum_UnitaMisura.CC__HL:
      case enum_UnitaMisura.Millilitri__HL:
      case 2005: // ml/100 kg di seme
      case 2017: // ml/100 kg di semi
      case 2021: // ml/kg di semente
        switch(Udm_2.codice){
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return moltiplicatore = 0.001;
          case enum_UnitaMisura.Metri_Cubi:
            return moltiplicatore = 0.000001;
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 0.00001;
        }
        break;
      //partendo dal Litri
      case enum_UnitaMisura.Litri:
      case enum_UnitaMisura.Litro__HA:
      case enum_UnitaMisura.Litri__HL:
      case 2004: // l/100 kg di seme
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return   moltiplicatore = 1000;
          case enum_UnitaMisura.Metri_Cubi:
            return   moltiplicatore = 0.001;
          case enum_UnitaMisura.Ettolitro:
            return   moltiplicatore = 0.01;
        }
        break;
      case 2009: // ml/1 t di prodotto
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return   moltiplicatore = 1;
          case enum_UnitaMisura.Metri_Cubi:
            return   moltiplicatore = 0.000001;
          case enum_UnitaMisura.Ettolitro:
            return   moltiplicatore = 0.00001;
          case enum_UnitaMisura.Litri:
            return   moltiplicatore = 0.001;
        }
        break;
      case 2015: // ml/100 kg di bulbilli
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return   moltiplicatore = 1;
          case enum_UnitaMisura.Metri_Cubi:
            return   moltiplicatore = 0.000001;
          case enum_UnitaMisura.Ettolitro:
            return   moltiplicatore = 0.00001;
          case enum_UnitaMisura.Litri:
            return   moltiplicatore = 0.001;
        }
        break;
      //partendo dal Metri_Cubi
      case enum_UnitaMisura.Metri_Cubi:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return moltiplicatore = 1000000;
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return moltiplicatore = 1000;
          case enum_UnitaMisura.Ettolitro:
            return moltiplicatore = 10;

        }
        break;
      //partendo dal Metri_Cubi
      case enum_UnitaMisura.Ettolitro:
        switch(Udm_2.codice){
          case enum_UnitaMisura.Millilitri:
          case enum_UnitaMisura.CentimetriCubi:
          case enum_UnitaMisura.Millilitri__Ha:
          case enum_UnitaMisura.CC__HL:
          case enum_UnitaMisura.Millilitri__HL:
            return moltiplicatore = 100000;
          case enum_UnitaMisura.Litri:
          case enum_UnitaMisura.Litro__HA:
          case enum_UnitaMisura.Litri__HL:
            return moltiplicatore = 100;
          case enum_UnitaMisura.Metri_Cubi:
            return moltiplicatore = 0.10;
        }
        break;

    }

    return moltiplicatore;
  }

  Componi_UdM_con_ha_hl(UdM:UnitaDiMisura,ha_hl:number){
    let udm_con_ha_hl = new UnitaDiMisura(0,"","");

    if(ha_hl === enum_UnitaMisura.Ettaro){
      switch(UdM.codice){
        case enum_UnitaMisura.Grammi:
          udm_con_ha_hl.codice = enum_UnitaMisura.Grammi__HA;
          break;
        case enum_UnitaMisura.KG:
          udm_con_ha_hl.codice = enum_UnitaMisura.KG__HA;
          break;
        case enum_UnitaMisura.Metri_Cubi:
          udm_con_ha_hl.codice = enum_UnitaMisura.METRI3__HA;
          break;
        case enum_UnitaMisura.Tonnellate:
          udm_con_ha_hl.codice = enum_UnitaMisura.Tonnellate__HA;
          break;
        case enum_UnitaMisura.Quintali:
          udm_con_ha_hl.codice = enum_UnitaMisura.QUINTALI__HA;
          break;
        case enum_UnitaMisura.Litri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Litro__HA;
          break;
        case enum_UnitaMisura.Millilitri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Millilitri__Ha;
          break;
      }
    }else if(ha_hl === enum_UnitaMisura.Ettolitro){
      switch(UdM.codice){
        case enum_UnitaMisura.CentimetriCubi:
          udm_con_ha_hl.codice = enum_UnitaMisura.CC__HL;
          break;
        case enum_UnitaMisura.Grammi:
          udm_con_ha_hl.codice = enum_UnitaMisura.Grammi__HL;
          break;
        case enum_UnitaMisura.Milligrammi:
          udm_con_ha_hl.codice = enum_UnitaMisura.Milligrammi__HL;
          break;
        case enum_UnitaMisura.Millilitri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Millilitri__HL;
          break;
        case enum_UnitaMisura.Litri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Litri__HL;
          break;
        case enum_UnitaMisura.KG:
          udm_con_ha_hl.codice = enum_UnitaMisura.Chilogrammi__HL;
          break;
      }
    }else if(ha_hl === enum_UnitaMisura.Quintali || ha_hl === enum_UnitaMisura.Tonnellate){
      switch(UdM.codice){
        case enum_UnitaMisura.Grammi:
          udm_con_ha_hl.codice = enum_UnitaMisura.Grammi__Quintale;
          break;
        case enum_UnitaMisura.Millilitri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Millilitri__Quintale;
          break;
        case enum_UnitaMisura.Litri:
          udm_con_ha_hl.codice = enum_UnitaMisura.Litri__Quintale;
          break;
        case enum_UnitaMisura.KG:
          udm_con_ha_hl.codice = enum_UnitaMisura.KG__Quintale;
          break;
      }
    }

    udm_con_ha_hl.simbolo = this.Componi_Simbolo_Ha_Hl(UdM,ha_hl);

    return udm_con_ha_hl;
  }

  Componi_Simbolo_Ha_Hl(udm:UnitaDiMisura,ha_hl:number):string{

    let simbolo = "";

    if(ha_hl === enum_UnitaMisura.Ettolitro){
      simbolo = udm.simbolo+"/hl";
    }else if(ha_hl === enum_UnitaMisura.Ettaro){
      simbolo = udm.simbolo+"/ha";
    }else if(ha_hl === enum_UnitaMisura.Quintali || ha_hl === enum_UnitaMisura.Tonnellate){
      simbolo = udm.simbolo+"/q";
    }

    return simbolo;
  }
}
