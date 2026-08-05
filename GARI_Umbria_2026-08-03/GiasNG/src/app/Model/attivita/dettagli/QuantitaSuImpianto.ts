import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { EsercizioCDC } from '../centri_di_costo/EsercizioCDC';

export class QuantitaSuImpianto {
    Qta: number = 0;
    esercizioCDC: EsercizioCDC;
    Magazzino: Fabbricato;
    Lotto: string = '';

}
