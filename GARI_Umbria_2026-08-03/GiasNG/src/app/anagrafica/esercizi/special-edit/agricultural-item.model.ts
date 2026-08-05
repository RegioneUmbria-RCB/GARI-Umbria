import {IntervalloTemporale} from '../../../Model/anagrafiche/IntervalloTemporale';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../../Model/CostantiPersonalizzate';
import {Esercizio} from '../../../Model/anagrafiche/Esercizio';
import {IListViewItem} from 'gias-ui-kit';
import {Appezzamento, PKAppezzamento} from '../../../Model/anagrafiche/Appezzamento';
import {Impianto, PKImpianto} from '../../../Model/anagrafiche/Impianto';
import {UtilizzoTerreno} from '../../../Model/metaschema/utilizzi/UtilizzoTerreno';
import {Varieta} from '../../../Model/metaschema/utilizzi/Varieta';

export class AgriculturalItem implements IListViewItem {
  description: string;
  key: string | number;
  error: boolean;
  success: boolean;
  warning: boolean;
  piva: string;
  progettoCod: number;
  idReg: number;
  appezza: number;
  saCod: number;
  validity: IntervalloTemporale;
  useOfLand: UtilizzoTerreno;

  constructor(
    key: string | number,
    description: string,
    piva: string,
    saCod: number = 0,
    appezza: number = 0,
    idReg: number = 0,
    progettoCod: number,
    validity: IntervalloTemporale = new IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE),
    useOfLand: UtilizzoTerreno = new Varieta(),
    error: boolean = false,
    success: boolean = false,
    warning: boolean = false
  ) {
    this.key = key;
    this.description = description;
    this.piva = piva;
    this.error = error;
    this.success = success;
    this.warning = warning;
    this.progettoCod = progettoCod;
    this.idReg = idReg;
    this.appezza = appezza;
    this.saCod = saCod;
    this.validity = validity;
    this.useOfLand = useOfLand;
  }

  toAppezzamento(): Appezzamento {
    const plotPK: PKAppezzamento = {
      codice: this.appezza,
      centroAziendalePK: {
        partitaIva: this.piva,
        codice: this.saCod
      }
    };

    let plot: Appezzamento = new Appezzamento(plotPK);
    plot.validita = this.validity;
    plot.descrizione = this.description;

    return plot;
  }

  toImpianto(): Impianto {
    const plotPK: PKAppezzamento = {
      codice: this.appezza,
      centroAziendalePK: {
        partitaIva: this.piva,
        codice: this.saCod
      }
    };

    const plantPK: PKImpianto = {
      codice: this.idReg,
      appezzamentoPK: plotPK
    };

    let plant: Impianto = new Impianto(plantPK);
    plant.validita = this.validity;
    plant.descrizione = this.description;
    plant.utilizzoTerreno = this.useOfLand;

    return plant;
  }

  toEsercizio(): Esercizio {
    let exe: Esercizio = new Esercizio(this.progettoCod, this.description);
    exe.validita = this.validity;
    exe.impiantoPK = {
      codice: this.idReg,
      appezzamentoPK: {
        codice: this.appezza,
        centroAziendalePK: {
          codice: this.saCod,
          partitaIva: this.piva
        }
      }
    };

    return exe;
  }
}
