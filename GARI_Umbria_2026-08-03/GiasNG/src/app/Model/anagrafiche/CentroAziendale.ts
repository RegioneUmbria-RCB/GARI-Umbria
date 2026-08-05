import { TipologiaSede} from '../metaschema/TipologiaSede';
import { TitoloDiPossesso} from '../metaschema/TitoloDiPossesso';
import { OrientamentoTecnicoEconomico} from '../metaschema/OrientamentoTecnicoEconomico';
import { BioTipoAttivita } from '../metaschema/BioTipoAttivita';
import { BioOrganismoDiControllo } from '../metaschema/BioOrganismoDiControllo';
import { CatastoCentroAziendale } from './CatastoCentroAziendale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { IndirizzoAssociato } from './addresses/IndirizzoAssociato';
import { IntervalloTemporale } from './IntervalloTemporale';
import { RubricaVoci} from './RubricaVoci';
import { Type } from '@angular/core';
import { DropdownList } from 'gias-kendo-grid';
import { formatDate } from '@angular/common';
import { CentroAziendaleEsternoCollegato } from '../metaschema/CentroAziendaleEsternoCollegato';

export type PKCentroAziendale = typeof CentroAziendale.PK.prototype;
export class CentroAziendale {
  primaryKey: PKCentroAziendale;
  nome: string;                                           // Dati Centro
  tipologia: TipologiaSede;                               // Dati Centro
  titolo_Di_Possesso: TitoloDiPossesso;                   // Dati Centro
  indirizzi: IndirizzoAssociato[];                        // Indirizzo
  lat: number;                                            // Geolocalizzazione
  lng: number;                                            // Geolocalizzazione
  rubricaVoci: RubricaVoci[];                             // Rubrica
  catastoCentroAziendale: CatastoCentroAziendale[];
  codici: CodiciAnagrafeValori[];
  validita: IntervalloTemporale;
  orientamentoTecnicoEconomico: OrientamentoTecnicoEconomico[];
  bioTipoAttivita: BioTipoAttivita;
  bioOrganismoDiControllo: BioOrganismoDiControllo;
  centroAziedaleEsternoCollegato: CentroAziendaleEsternoCollegato;
  flag_cancellazione: boolean;

  public static PK = class CentriPK {
    codice: number;
    partitaIva: string;

    constructor(codice: number, partitaIva: string) {
      this.codice = codice;
      this.partitaIva = partitaIva;
    }
  };

  constructor(primaryKey: PKCentroAziendale) {
    this.primaryKey = primaryKey;
    this.flag_cancellazione = false;
  }

  static Empty(piva: string) {
    return this.getEmptyCentroAziendale(piva);
  }

  static getEmptyCentroAziendale(piva: string) {
    const centro: CentroAziendale = {
      bioOrganismoDiControllo: {codice: '0', descrizione: '' },
      centroAziedaleEsternoCollegato: {codice: '0', descrizione: '' },
      bioTipoAttivita: {codice: ' ', descrizione: '' },
      catastoCentroAziendale: null,
      codici: [],
      flag_cancellazione: false,
      indirizzi: [],
      lat: 0,
      lng: 0,
      nome: '',
      orientamentoTecnicoEconomico: [],
      primaryKey: {
        codice: 0,
        partitaIva: piva
      },
      rubricaVoci: [],
      tipologia: {
        codice: 102,
        descrizione: null,
      },
      titolo_Di_Possesso: { codice: 0, descrizione: ''},
      validita: new IntervalloTemporale(new Date(), new Date())
    };
    let indirizzo: IndirizzoAssociato = {
      flag_cancellazione: false,
      indirizzo: {
        cap: '00000',
        codice: 0,
        flag_cancellazione: false,
        frazione: '',
        istatComune: {
          reg:'000',
          cap: '00000',
          codiceBelfiore: '',
          com: '000',
          comuni_prov: '00',
          localita: '',
          prov: '000',
          validita: null
        },
        note: '',
        stato: {
          codiceNumerico: null,
          codiceAlpha3: null,
          codice: 'IT',
          descrizione: 'Italia',
          gestioneGerarchia: 0
        },
        via: '',
        geolocation: {lat: 0, lng: 0, isValid: false}
      },
      tipo_Indirizzo: 1
    };
    centro.indirizzi.push(indirizzo);

    return centro;
  }

}

export class CentroAziendaleNG {

  public Centro: CentroAziendale;
  public DatiVisibili: CentroDropdownLists;
}

export class CentroDropdownLists {
  public CodiceOperatore: string;
  public Tipologia: AspxDropdown;
  public TitoloPossesso: AspxDropdown;
  public Stati: AspxDropdown;
  public Comune: AspxDropdown;
  public Provincie: AspxDropdown;
  public Codici: AspxDropdown;
  public TipoAttivita: AspxDropdown;
  public OrganismiControllo: AspxDropdown;
  public CentroAziendaleEsternoCollegato: AspxDropdown;
  public Otes: AspxDropdown;
}

export class CentroAziendaleDropdowns {
  public Tipologia: AspxDropdown;
  public TitoloPossesso: AspxDropdown;
  public Stati: AspxDropdown;
  public Comune: AspxDropdown;
  public Provincie: AspxDropdown;
  public Codici: AspxDropdown;
  public TipoAttivita: AspxDropdown;
  public OrganismiControllo: AspxDropdown;
  public CentroAziendaleEsternoCollegato: AspxDropdown;
  public Otes: AspxDropdown;
}

export type AspxDropdown = {
  Items: AspxDropdownItem[];
}
export type AspxDropdownItem = {
  Enabled: boolean;
  Selected: boolean;
  Text: string;
  Value: string;
}
