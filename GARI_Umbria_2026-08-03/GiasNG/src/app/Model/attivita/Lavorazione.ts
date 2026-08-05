import {CategoriaOperazione} from './categorie/CategoriaOperazione';
import {Job, PKJob} from './Job';
import {TipiJob} from './TipiJob';

export class Lavorazione extends Job {
  categoriaOperazione: CategoriaOperazione;
  descrizione: string | null = null;

  constructor(codice: string) {
    super({classType: 'Lavorazione', codice: codice} as PKJob);
  }

  getTipo(): TipiJob {
    return TipiJob.LAVORAZIONE;
  }
}
