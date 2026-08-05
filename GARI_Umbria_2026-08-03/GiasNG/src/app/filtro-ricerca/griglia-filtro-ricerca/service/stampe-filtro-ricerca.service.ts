import { Injectable } from "@angular/core";
import { enum_CodificaStampe } from "app/Model/TipiEnumerativi";
import { BloccoxReport } from "app/filtro-ricerca/utils";

@Injectable()
export class StampeFiltroRicercaService {

    BlocchixReport: BloccoxReport[] = new Array<BloccoxReport>();

    constructor() {
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaColturale_Biologico, true, true, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.EstrattoreDatiGrafici, true, true, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Atto_Notorio, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(-(enum_CodificaStampe.Atto_Notorio), true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaAziendale, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.ImpegnativaColtivazioneConferimento, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaCampagna_2078_Semplificata, true, true, true));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.RegistroTrattamenti_Semplificata, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaRegistrazione_Semplificata, true, true, true));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Quadro_P, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Eurep_Gap_Semplificata, true, true, true));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Bilancio_Fertilizzazioni, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.RegistroTrattamenti_Veneto, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaCampagnaMultiCentro, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Eurep_Gap_Multicentro, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaCampagna_ProvAut_Trento, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaCampagna_Multi_Lombardia, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaInterventiAgronomici, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.RegistroAziendaleUnico, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Registro_Fertilizzazioni, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaTracciabilita, true, true, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.SchedaCatastoeUtilizzi, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Adesione_Etico_Ambientale, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Tenuta_Scheda_Campagna, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Adesione_DPI, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Eurep, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_QC, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Confusione_Sessuale, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Codice_Condotta, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Orticole_Industria, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Pomodoro_Industria, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.Allegato_CatastoeValorizzazioni, true, false, false));
        this.BlocchixReport.push(new BloccoxReport(enum_CodificaStampe.ObiettivoDiProduzioneAsipo, true, false, false));
    }

}