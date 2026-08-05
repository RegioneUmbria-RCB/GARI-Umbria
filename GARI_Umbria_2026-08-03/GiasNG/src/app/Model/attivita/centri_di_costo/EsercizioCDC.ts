import { Documento } from 'app/Model/documenti/Documento';
import {Esercizio} from '../../anagrafiche/Esercizio';
import {CentroDiCosto, Tipo} from './CentroDiCosto';


export class EsercizioCDC extends CentroDiCosto {
    esercizio: Esercizio;
    superficieTrattata: number;
    superficieRiduzioneBufferZone: number;
    percentualeRiduzioneDeriva: number;
    quantita: number;
    documento: Documento;

    constructor() {
        super();
        this.classType = 'EsercizioCDC';
        this.tipo = Tipo.Esercizio;
    }
}
