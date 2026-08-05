import { ParticelleCatastaliClassamento } from './ParticelleCatastaliClassamento';
import { ParticelleCatastaliMacrouso } from './ParticelleCatastaliMacrouso';
import { ParticelleCatastaliMetodoProduzione } from './ParticelleCatastaliMetodoProduzione';
import { ParticelleCatastaliZona } from './ParticelleCatastaliZona';

export type PKParticelleCatastali = typeof ParticelleCatastali.PK.prototype;
export class ParticelleCatastali {

  Area: number;
  macrousi: ParticelleCatastaliMacrouso[];
  zonizzazione: ParticelleCatastaliZona[];
  classamento: ParticelleCatastaliClassamento[];
  metodoProduzione: ParticelleCatastaliMetodoProduzione[];
  proprietario: string;

  primaryKey: PKParticelleCatastali;

  public static PK = class PKParticelleCatastali {
    Prov: string;
    Com: string;
    Sezione: string;
    Foglio: number;
    Numero: number;
    Subalterno: string;

    constructor (
      Prov: string,
      Com: string,
      Sezione: string,
      Foglio: number,
      Numero: number,
      Subalterno: string
    ) {
      this.Prov = Prov;
      this.Com = Com;
      this.Sezione = Sezione;
      this.Foglio = Foglio;
      this.Numero = Numero;
      this.Subalterno = Subalterno;
    }
  };
}
