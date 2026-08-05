import { Risorsa } from './Risorsa';

export class RisorsaTimeSheet extends Risorsa {
    inizio: Date | null;
    fine: Date | null;
    totaleOre: number;
}
