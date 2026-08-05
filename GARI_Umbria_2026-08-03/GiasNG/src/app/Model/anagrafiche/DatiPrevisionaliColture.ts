import {BaseCodeDescr} from '../baseClass/baseCodeDescr';
import {BaseCodeDescrStr} from '../baseClass/baseCodeDescrStr';
import {UnitaDiMisura} from '../../Service/api.service';
import {AGRODATAFINE, AGRODATAINIZIO} from '../CostantiPersonalizzate';

export class DatiPrevisionaliColtureRequest {
  public vegCod: BaseCodeDescr;
  public culCod: BaseCodeDescr;
  public grvaCod: BaseCodeDescr;
  public grfiCod: BaseCodeDescr;
  public statoCod: BaseCodeDescr;
  public portCod: BaseCodeDescr;
  public foralCod: BaseCodeDescr;
  public dettSpeciePersonalizzatoCod: BaseCodeDescrStr;
  public regCod: BaseCodeDescr;
  public parametroCod: BaseCodeDescr;
  public reg: BaseCodeDescrStr;
  public prov: BaseCodeDescrStr;
  public codiceStato: BaseCodeDescrStr;
  public validitaInizio: Date;
  public validitaFine: Date;

  constructor() {
    this.vegCod = new BaseCodeDescr(0,'');
    this.culCod = new BaseCodeDescr(0,'');
    this.grvaCod = new BaseCodeDescr(0,'');
    this.grfiCod = new BaseCodeDescr(0,'');
    this.statoCod = new BaseCodeDescr(0,'');
    this.portCod = new BaseCodeDescr(0,'');
    this.foralCod = new BaseCodeDescr(0,'');
    this.dettSpeciePersonalizzatoCod = new BaseCodeDescrStr('');
    this.regCod = new BaseCodeDescr(0,'');
    this.parametroCod = new BaseCodeDescr(0,'');
    this.reg = new BaseCodeDescrStr('');
    this.prov = new BaseCodeDescrStr('');
    this.codiceStato = new BaseCodeDescrStr('');
    this.validitaInizio = AGRODATAINIZIO;
    this.validitaFine = AGRODATAFINE;
  }
}

export class DatiPrevisionaliColture {
  public parametroCod: number;
  public udm: UnitaDiMisura;
  public valore: number;

  constructor() {
    this.parametroCod = 0;
    this.udm = {};
    this.valore = 0;
  }
}

export class DatiPrevisionaliColtureComplete extends DatiPrevisionaliColture {
  public piva: string;
  public id: number;
  public vegCod: number;
  public culCod: number;
  public grvaCod: number;
  public grfiCod: number;
  public statoCod: number;
  public portCod: number;
  public foralCod: number;
  public dettSpeciePersonalizzatoCod: string;
  public regCod: number;
  public reg: string;
  public prov: string;
  public codiceStato: string;
  public validitaInizio: Date;
  public validitaFine: Date;

  constructor() {
    super();
  }
}
