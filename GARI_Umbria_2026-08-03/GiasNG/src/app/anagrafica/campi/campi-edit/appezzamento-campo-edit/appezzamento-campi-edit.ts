import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

export class AppezzamentoCampo {
    piva: string;
    sa_cod: number;
    appezza: number;
    campo_cod: number;
    sup_app: number;
    app_nome: string;
    validita: IntervalloTemporale;
    id_reg: number;
    validita_impianto: IntervalloTemporale;
    cul_cod: number;
    cul_des: string;
    veg_des: string;
    flag_cancellazione: boolean;
}
