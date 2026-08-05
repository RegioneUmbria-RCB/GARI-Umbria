using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaNetCore.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda
{
    public class UtilityAgendaClassInitializer : IUtilityAgendaClassInitializer
    {
        #region InfoOperazione
        private readonly IEnumerable<int> _fertilizzazioni = new HashSet<int>
        {
            LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME,
            LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE,
            LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI,
            LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
            LAV_COD.LAVCOD_FERTIRRIGAZIONE,
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
        };
        private readonly IEnumerable<int> _trattamenti = new HashSet<int>
        {
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
            LAV_COD.LAVCOD_DISERBO,
            LAV_COD.LAVCOD_DISSECCAMENTO,
            LAV_COD.LAVCOD_GEODISINFESTAZIONE,
            LAV_COD.LAVCOD_CONCIA_SEME,
            LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
            LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
            LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA
        };
        private readonly IEnumerable<int> _polverulenti = new HashSet<int>
        {
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
            LAV_COD.LAVCOD_DISERBO,
            LAV_COD.LAVCOD_DISSECCAMENTO,
            LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
            LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA,
            LAV_COD.LAVCOD_CONCIA_SEME
        };
        private readonly IEnumerable<int> _semine = new HashSet<int>
        {
            LAV_COD.LAVCOD_SEMINA,
            LAV_COD.LAVCOD_TRAPIANTO,
            LAV_COD.LAVCOD_SOVESCIO,
            LAV_COD.LAVCOD_SOD_SEDDING
        };
        private readonly IEnumerable<int> _lavorazioni = new HashSet<int>
        {
            LAV_COD.LAVCOD_ANDANAMENTO,
            LAV_COD.LAVCOD_ARATURA,
            LAV_COD.LAVCOD_DEFOGLIAZIONE,
            LAV_COD.LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
            LAV_COD.LAVCOD_ASSOLCATURA,
            LAV_COD.LAVCOD_CARICO_MANUALE_FRUTTA,
            LAV_COD.LAVCOD_CIMATURA,
            LAV_COD.LAVCOD_DIRADAMENTO_MANUALE,
            LAV_COD.LAVCOD_DISSODAMENTO,
            LAV_COD.LAVCOD_ERPICATURA,
            LAV_COD.LAVCOD_ESTIRPATURA,
            LAV_COD.LAVCOD_ESPIANTO,
            LAV_COD.LAVCOD_FALCIACONDIZIONATURA,
            LAV_COD.LAVCOD_FALCIATURA_ERBAI,
            LAV_COD.LAVCOD_FORMAZIONE_ARGINELLI,
            LAV_COD.LAVCOD_FRANGIZOLLATURA,
            LAV_COD.LAVCOD_FRESATURA,
            LAV_COD.LAVCOD_IMBALLO_FIENO_ROTOLI,
            LAV_COD.LAVCOD_INTERRAMENTO_PAGLIE,
            LAV_COD.LAVCOD_LAVORAZIONE_TRA_FILA,
            LAV_COD.LAVCOD_LAVORAZIONE_SU_FILA,
            LAV_COD.LAVCOD_LEGATURA,
            LAV_COD.LAVCOD_LIVELLAMENTO,
            LAV_COD.LAVCOD_MANUTENZIONE_ARGINI,
            LAV_COD.LAVCOD_MESSA_DIMORA_PIANTE,
            LAV_COD.LAVCOD_MIETITREBBIATURA,
            LAV_COD.LAVCOD_MINIMUM_TILLAGE,
            LAV_COD.LAVCOD_PACCIAMATURA,
            LAV_COD.LAVCOD_POTATURA_SECCA,
            LAV_COD.LAVCOD_POTATURA_VERDE,
            LAV_COD.LAVCOD_PRESSATURA,
            LAV_COD.LAVCOD_RACCOLTA_LEGNA_POTATURA,
            LAV_COD.LAVCOD_RACCOLTA_MANUALE,
            LAV_COD.LAVCOD_RACCOLTA_MECCANICA,
            LAV_COD.LAVCOD_RANGHINATURA,
            LAV_COD.LAVCOD_RINCALZATURA,
            LAV_COD.LAVCOD_RIPPATURA,
            LAV_COD.LAVCOD_RIPUNTATURA,
            LAV_COD.LAVCOD_RIVOLTAMENTO_FORAGGIO,
            LAV_COD.LAVCOD_RULLATURA,
            LAV_COD.LAVCOD_SARCHIATURA,
            LAV_COD.LAVCOD_SCARIFICATURA,
            LAV_COD.LAVCOD_SCASSO,
            LAV_COD.LAVCOD_TRINCIATURA,
            LAV_COD.LAVCOD_VANGATURA,
            LAV_COD.LAVCOD_ZAPPATURA,
            LAV_COD.LAVCOD_GEBIATURA,
            LAV_COD.LAVCOD_ROMPICROSTA,
            LAV_COD.LAVCOD_LAVORAZIONE_CONBINATA,
            LAV_COD.LAVCOD_ERPICATURA_ROTANTE,
            LAV_COD.LAVCOD_INTERVENTO_ANTIBRINA,
            LAV_COD.LAVCOD_ALTRE_OPERAZIONI,
            LAV_COD.LAVCOD_STRIGLIATURA,
            LAV_COD.LAVCOD_PIRODISERBO,
            LAV_COD.LAVCOD_ABBATTIMENTO_IMPIANTI
        };
        private readonly IEnumerable<int> _trappole = new HashSet<int>
        {
            LAV_COD.LAVCOD_CONFUSIONE_SESSUALE,
            LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE
        };
        private readonly IEnumerable<int> _rilievi = new HashSet<int>
        {
            LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
            LAV_COD.LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
            LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA,
            LAV_COD.LAVCOD_DANNI_RACCOLTA,
            LAV_COD.LAVCOD_FASI_FENOLOGICHE,
            LAV_COD.LAVCOD_RILIEVO_ERBE_INFESTANTI,
            LAV_COD.LAVCOD_RILIEVO_PIOGGE,
            LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
        };

        private InfoOperazione GetInfoFertilizzazione()
        {
            InfoOperazione info = new();
            info.IsFertilizzazione = true;
            info.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
            info.Elem_Cod = ELEM_COD.FERTILIZZANTI;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioFertilizzazione;
            return info;
        }

        private InfoOperazione GetInfoTrattamento(int lavcod)
        {
            InfoOperazione info = new();
            info.IsTrattamento = true;
            info.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
            info.Elem_Cod = ELEM_COD.FORMULATI;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioTrattamento;
            info.controlloPolverulenti = _polverulenti.Contains(lavcod);
            if (lavcod == LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA || lavcod == LAV_COD.LAVCOD_CONCIA_SEME)
            {
                info.TipoCentroDiCosto = AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.ProdottoDaTrattare;
            }
            else
            {
                info.TipoCentroDiCosto = AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.Esercizio;
            }
            return info;
        }

        private InfoOperazione GetInfoRaccolta()
        {
            InfoOperazione info = new();
            info.IsRaccolta = true;
            info.Cau_Mov = CAU_MOV.CAU_RILIEVO_RACCOLTA;
            info.Elem_Cod = ELEM_COD.TRASFORMATI_VEGETALI;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioRaccolta;
            return info;
        }

        private InfoOperazione GetInfoSemina()
        {
            InfoOperazione info = new();
            info.IsSemina = true;
            info.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
            info.Elem_Cod = ELEM_COD.SEMENTI;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioSemina;
            return info;
        }

        private InfoOperazione GetInfoAbbattimento()
        {
            InfoOperazione info = new();
            info.IsAbbattimento = true;
            info.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
            return info;
        }

        private InfoOperazione GetInfoLavorazione()
        {
            InfoOperazione info = new();
            info.IsLavorazione = true;
            info.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
            return info;
        }

        private InfoOperazione GetInfoDistribuzioneInsetti()
        {
            InfoOperazione info = new();
            info.IsTrattamento = true;
            info.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
            info.Elem_Cod = ELEM_COD.INSETTI;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioTrattamento;
            return info;
        }

        private InfoOperazione GetInfoTrappole()
        {
            InfoOperazione info = new();
            info.IsTrattamento = true;
            info.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
            info.Elem_Cod = ELEM_COD.TRAPPOLE;
            return info;
        }

        private InfoOperazione GetInfoRilievo(int lavcod)
        {
            InfoOperazione info = new();
            info.IsRilievo = true;
            if (lavcod == LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA
                || lavcod == LAV_COD.LAVCOD_DANNI_RACCOLTA 
                || lavcod == LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
            {
                info.Cau_Mov = CAU_MOV.CAU_RILIEVO_RACCOLTA;
            } else
            {
                info.Cau_Mov = CAU_MOV.CAU_RILIEVO_CAMPO;
            }
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioRilievo;
            return info;
        }

        private InfoOperazione GetInfoVisita()
        {
            InfoOperazione info = new();
            info.IsVisita = true;
            info.Cau_Mov = CAU_MOV.CAU_VISITE_ISPETTIVE;
            return info;
        }

        private InfoOperazione GetInfoIrrigazione()
        {
            InfoOperazione info = new();
            info.IsIrrigazione = true;
            info.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
            info.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioIrrigazione;
            return info;
        }

        #endregion

        /// <summary>
        /// Porting della funzione GetInfoOperazione in AgronicaCoreModello.Utility_Agenda.
        /// Questa funzione concerne esclusivamente l'InfoOperazione delle operazioni agenda.
        /// </summary>
        /// <param name="lavCod"></param>
        /// <param name="tipoAttivita"></param>
        /// <remarks>
        /// Tralasciata la gestione dei seguenti lavcod che dovranno essere gestiti in un'apposita funzione tematica
        /// (vanno create utility simili per l'area del documentale, delle operazioni zoo etc.):
        ///    LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,
        ///    LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO,
        ///    LAVCOD_COSTI_CDG,
        ///    LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI,
        ///    LAVCOD_SCARICO,
        ///    LAVCOD_CARICO,
        ///    LAVCOD_VENDITA,
        ///    LAVCOD_ACQUISTO,
        ///    LAVCOD_TRASFERIMENTO,
        ///    LAVCOD_FATTURA_EMESSA,
        ///    LAVCOD_FATTURA_RICEVUTA,
        ///    LAVCOD_BOLLA_RICEVUTA,
        ///    LAVCOD_BOLLA_EMESSA,
        ///    LAVCOD_NOTA_ACCREDITO_RICEVUTA,
        ///    LAVCOD_NOTA_ACCREDITO_EMESSA,
        ///    LAVCOD_REINNESCO_TRAPPOLE,
        ///    LAVCOD_CURA,
        ///    LAVCOD_MANUTENZIONE_MACCHINE,
        ///    LAVCOD_REVISIONE_MACCHINE,
        ///    LAVCOD_GESTIONE_RIFIUTI,
        ///    LAVCOD_INCREMENTO_CONSISTENZE_ZOO,
        ///    LAVCOD_NASCITA_ANIMALI,
        ///    LAVCOD_ACQUISTO_ANIMALI,
        ///    LAVCOD_SPOSTAMENTI_ZOO
        ///    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI,
        ///    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
        ///    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI,
        ///    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI,
        ///    LAVCOD_PESATURA_ANIMALI,
        ///    LAVCOD_MORTE_ANIMALI,
        ///    LAVCOD_MACELLAZIONE_ANIMALI,
        ///    LAVCOD_DECREMENTO_CONSISTENZE_ZOO,
        ///    LAVCOD_VENDITA_ANIMALI,
        ///    LAVCOD_ALTRE_LAVORAZIONI_ZOO
        /// </remarks>
        public InfoOperazione GetInfoOperazione(int lavCod, Attivita.Tipo_Attivita tipoAttivita)
        {
            InfoOperazione info = lavCod switch
            {
                LAV_COD.LAVCOD_RACCOLTA => GetInfoRaccolta(),
                LAV_COD.LAVCOD_ABBATTIMENTO_IMPIANTI => GetInfoAbbattimento(),
                LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI => GetInfoDistribuzioneInsetti(),
                LAV_COD.LAVCOD_VISITA => GetInfoVisita(),
                LAV_COD.LAVCOD_IRRIGAZIONE => GetInfoIrrigazione(),
                _ when _fertilizzazioni.Contains(lavCod) => GetInfoFertilizzazione(),
                _ when _trattamenti.Contains(lavCod) => GetInfoTrattamento(lavCod),
                _ when _semine.Contains(lavCod) => GetInfoSemina(),
                _ when _lavorazioni.Contains(lavCod) => GetInfoLavorazione(),
                _ when _trappole.Contains(lavCod) => GetInfoTrappole(),
                _ when _rilievi.Contains(lavCod) => GetInfoRilievo(lavCod),
                _ => new InfoOperazione()
            };
            if (lavCod == LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA || lavCod == LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI)
            {
                info.PregressoConMultiAvversita = true;
            }
            return info;
        }
    }
}
