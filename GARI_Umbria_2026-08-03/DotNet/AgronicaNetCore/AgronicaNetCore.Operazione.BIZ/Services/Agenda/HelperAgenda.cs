using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.costanti;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Globalization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.Agenda
{
    public static class HelperAgenda
    {
        public static InfoOperazione GetInfoOperazione(int lav_cod, Attivita.Tipo_Attivita tipoAttivita)
        {
            var infoOperazione = new InfoOperazione();

            switch (lav_cod)
            {
                case LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME:
                case LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE:
                case LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI:
                case LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE:
                case LAV_COD.LAVCOD_FERTIRRIGAZIONE:
                case LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA:

                    infoOperazione.IsFertilizzazione = true;
                    infoOperazione.IsFertirrigazione = lav_cod == LAV_COD.LAVCOD_FERTIRRIGAZIONE;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
                    infoOperazione.Elem_Cod = ELEM_COD.FERTILIZZANTI;
                    infoOperazione.classType = ClassType.DettaglioFertilizzazione;
                    break;

                case LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO:
                case LAV_COD.LAVCOD_DISERBO:
                case LAV_COD.LAVCOD_DISSECCAMENTO:
                case LAV_COD.LAVCOD_GEODISINFESTAZIONE:
                case LAV_COD.LAVCOD_CONCIA_SEME:
                case LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE:
                case LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE:
                case LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA:
                case LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA:
                case LAV_COD.LAVCOD_REINNESCO_TRAPPOLE:

                    infoOperazione.IsTrattamento = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
                    infoOperazione.Elem_Cod = ELEM_COD.FORMULATI;
                    infoOperazione.classType = ClassType.DettaglioTrattamento;

                    infoOperazione.controlloPolverulenti = lav_cod switch
                    {
                        LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO or
                        LAV_COD.LAVCOD_DISERBO or
                        LAV_COD.LAVCOD_DISSECCAMENTO or
                        LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE or
                        LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA or
                        LAV_COD.LAVCOD_CONCIA_SEME => true,
                        _ => false
                    };

                    infoOperazione.TipoCentroDiCosto = lav_cod switch
                    {
                        LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA or
                        LAV_COD.LAVCOD_CONCIA_SEME => AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.ProdottoDaTrattare,
                        _ => AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.Esercizio
                    };
                    break;

                case LAV_COD.LAVCOD_RACCOLTA:
                    infoOperazione.IsRaccolta = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_RILIEVO_RACCOLTA;
                    infoOperazione.Elem_Cod = ELEM_COD.TRASFORMATI_VEGETALI;
                    infoOperazione.classType = ClassType.DettaglioRaccolta;
                    break;

                case LAV_COD.LAVCOD_SEMINA:
                case LAV_COD.LAVCOD_TRAPIANTO:
                case LAV_COD.LAVCOD_SOVESCIO:
                case LAV_COD.LAVCOD_SOD_SEDDING:
                    infoOperazione.IsSemina = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
                    infoOperazione.Elem_Cod = ELEM_COD.SEMENTI;
                    infoOperazione.classType = ClassType.DettaglioSemina;
                    break;

                case LAV_COD.LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO:
                case LAV_COD.LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO:
                    infoOperazione.isNonUtilizzo = true;
                    infoOperazione.Cau_Mov = lav_cod == LAV_COD.LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO
                        ? CAU_MOV.CAU_LAVORAZIONE
                        : CAU_MOV.CAU_TRATTAMENTO;
                    break;

                case LAV_COD.LAVCOD_ABBATTIMENTO_IMPIANTI:
                    infoOperazione.IsAbbattimento = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
                    break;

                case LAV_COD.LAVCOD_ANDANAMENTO:
                case LAV_COD.LAVCOD_ARATURA:
                case LAV_COD.LAVCOD_DEFOGLIAZIONE:
                case LAV_COD.LAVCOD_ASPORTAZIONE_ORGANI_INFETTI:
                case LAV_COD.LAVCOD_ASSOLCATURA:
                case LAV_COD.LAVCOD_CARICO_MANUALE_FRUTTA:
                case LAV_COD.LAVCOD_CIMATURA:
                case LAV_COD.LAVCOD_DIRADAMENTO_MANUALE:
                case LAV_COD.LAVCOD_DISSODAMENTO:
                case LAV_COD.LAVCOD_ERPICATURA:
                case LAV_COD.LAVCOD_ESTIRPATURA:
                case LAV_COD.LAVCOD_ESPIANTO:
                case LAV_COD.LAVCOD_FALCIACONDIZIONATURA:
                case LAV_COD.LAVCOD_FALCIATURA_ERBAI:
                case LAV_COD.LAVCOD_FORMAZIONE_ARGINELLI:
                case LAV_COD.LAVCOD_FRANGIZOLLATURA:
                case LAV_COD.LAVCOD_FRESATURA:
                case LAV_COD.LAVCOD_IMBALLO_FIENO_ROTOLI:
                case LAV_COD.LAVCOD_INTERRAMENTO_PAGLIE:
                case LAV_COD.LAVCOD_LAVORAZIONE_TRA_FILA:
                case LAV_COD.LAVCOD_LAVORAZIONE_SU_FILA:
                case LAV_COD.LAVCOD_LEGATURA:
                case LAV_COD.LAVCOD_LIVELLAMENTO:
                case LAV_COD.LAVCOD_MANUTENZIONE_ARGINI:
                case LAV_COD.LAVCOD_MESSA_DIMORA_PIANTE:
                case LAV_COD.LAVCOD_MIETITREBBIATURA:
                case LAV_COD.LAVCOD_MINIMUM_TILLAGE:
                case LAV_COD.LAVCOD_PACCIAMATURA:
                case LAV_COD.LAVCOD_POTATURA_SECCA:
                case LAV_COD.LAVCOD_POTATURA_VERDE:
                case LAV_COD.LAVCOD_PRESSATURA:
                case LAV_COD.LAVCOD_RACCOLTA_LEGNA_POTATURA:
                case LAV_COD.LAVCOD_RACCOLTA_MANUALE:
                case LAV_COD.LAVCOD_RACCOLTA_MECCANICA:
                case LAV_COD.LAVCOD_RANGHINATURA:
                case LAV_COD.LAVCOD_RINCALZATURA:
                case LAV_COD.LAVCOD_RIPPATURA:
                case LAV_COD.LAVCOD_RIPUNTATURA:
                case LAV_COD.LAVCOD_RIVOLTAMENTO_FORAGGIO:
                case LAV_COD.LAVCOD_RULLATURA:
                case LAV_COD.LAVCOD_SARCHIATURA:
                case LAV_COD.LAVCOD_SCARIFICATURA:
                case LAV_COD.LAVCOD_SCASSO:
                case LAV_COD.LAVCOD_TRINCIATURA:
                case LAV_COD.LAVCOD_VANGATURA:
                case LAV_COD.LAVCOD_ZAPPATURA:
                case LAV_COD.LAVCOD_GEBIATURA:
                case LAV_COD.LAVCOD_ROMPICROSTA:
                case LAV_COD.LAVCOD_LAVORAZIONE_CONBINATA:
                case LAV_COD.LAVCOD_ERPICATURA_ROTANTE:
                case LAV_COD.LAVCOD_INTERVENTO_ANTIBRINA:
                case LAV_COD.LAVCOD_ALTRE_OPERAZIONI:
                case LAV_COD.LAVCOD_STRIGLIATURA:
                case LAV_COD.LAVCOD_PIRODISERBO:
                case LAV_COD.LAVCOD_PASCOLAMENTO_PROPRIO:
                case LAV_COD.LAVCOD_PASCOLAMENTO_TERZI:
                    infoOperazione.IsLavorazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
                    break;

                case LAV_COD.LAVCOD_COSTI_CDG:
                case LAV_COD.LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI:
                    break;

                case LAV_COD.LAVCOD_CARICO:
                    infoOperazione.IsCarico = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_CARICO;
                    break;

                case LAV_COD.LAVCOD_SCARICO:
                    infoOperazione.IsScarico = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_SCARICO;
                    break;

                case LAV_COD.LAVCOD_VENDITA:
                case LAV_COD.LAVCOD_ACQUISTO:
                case LAV_COD.LAVCOD_TRASFERIMENTO:
                    break;

                case LAV_COD.LAVCOD_FATTURA_RICEVUTA:
                case LAV_COD.LAVCOD_BOLLA_RICEVUTA:
                    infoOperazione.IsCarico = true;
                    infoOperazione.IsRegistrazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_CARICO;
                    break;

                case LAV_COD.LAVCOD_FATTURA_EMESSA:
                case LAV_COD.LAVCOD_BOLLA_EMESSA:
                    infoOperazione.IsScarico = true;
                    infoOperazione.IsRegistrazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_SCARICO;
                    break;

                case LAV_COD.LAVCOD_NOTA_ACCREDITO_RICEVUTA:
                    infoOperazione.IsScarico = true;
                    infoOperazione.IsRegistrazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_SCARICO;
                    break;

                case LAV_COD.LAVCOD_NOTA_ACCREDITO_EMESSA:
                    infoOperazione.IsCarico = true;
                    infoOperazione.IsRegistrazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_CARICO;
                    break;

                case LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI:
                    infoOperazione.IsTrattamento = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
                    infoOperazione.Elem_Cod = ELEM_COD.INSETTI;
                    infoOperazione.classType = ClassType.DettaglioTrattamento;
                    break;

                case LAV_COD.LAVCOD_CONFUSIONE_SESSUALE:
                case LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE:
                    infoOperazione.IsTrattamento = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_TRATTAMENTO;
                    infoOperazione.Elem_Cod = ELEM_COD.TRAPPOLE;
                    break;

                case LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO:
                case LAV_COD.LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE:
                case LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA:
                case LAV_COD.LAVCOD_DANNI_RACCOLTA:
                case LAV_COD.LAVCOD_FASI_FENOLOGICHE:
                case LAV_COD.LAVCOD_RILIEVO_ERBE_INFESTANTI:
                case LAV_COD.LAVCOD_RILIEVO_PIOGGE:
                case LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA:
                    infoOperazione.IsRilievo = true;
                    infoOperazione.Cau_Mov = lav_cod switch
                    {
                        LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA or
                        LAV_COD.LAVCOD_DANNI_RACCOLTA or
                        LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA => CAU_MOV.CAU_RILIEVO_RACCOLTA,
                        _ => CAU_MOV.CAU_RILIEVO_CAMPO
                    };
                    infoOperazione.classType = ClassType.DettaglioRilievo;
                    break;

                case LAV_COD.LAVCOD_VISITA:
                    infoOperazione.IsVisita = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_VISITE_ISPETTIVE;
                    infoOperazione.Elem_Cod = 0;
                    break;

                case LAV_COD.LAVCOD_IRRIGAZIONE:
                    infoOperazione.IsIrrigazione = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_LAVORAZIONE;
                    infoOperazione.Elem_Cod = 0;
                    infoOperazione.classType = ClassType.DettaglioIrrigazione;
                    break;

                case LAV_COD.LAVCOD_CURA:
                case LAV_COD.LAVCOD_MANUTENZIONE_MACCHINE:
                case LAV_COD.LAVCOD_REVISIONE_MACCHINE:
                case LAV_COD.LAVCOD_GESTIONE_RIFIUTI:
                    break;

                case LAV_COD.LAVCOD_INCREMENTO_CONSISTENZE_ZOO:
                case LAV_COD.LAVCOD_NASCITA_ANIMALI:
                case LAV_COD.LAVCOD_ACQUISTO_ANIMALI:
                    infoOperazione.IsZoo = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_CARICO;
                    infoOperazione.Elem_Cod = ELEM_COD.ZOO_CONSISTENZA;
                    break;

                case LAV_COD.LAVCOD_SPOSTAMENTI_ZOO:
                    infoOperazione.IsZoo = true;
                    break;

                case LAV_COD.LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI:
                case LAV_COD.LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI:
                case LAV_COD.LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI:
                case LAV_COD.LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI:
                    infoOperazione.IsZoo = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_ALIMENTAZIONE;
                    break;

                case LAV_COD.LAVCOD_PESATURA_ANIMALI:
                    infoOperazione.IsZoo = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_PESATURA_ANIMALI;
                    break;

                case LAV_COD.LAVCOD_MORTE_ANIMALI:
                case LAV_COD.LAVCOD_MACELLAZIONE_ANIMALI:
                case LAV_COD.LAVCOD_DECREMENTO_CONSISTENZE_ZOO:
                case LAV_COD.LAVCOD_VENDITA_ANIMALI:
                    infoOperazione.IsZoo = true;
                    infoOperazione.Cau_Mov = CAU_MOV.CAU_SCARICO;
                    infoOperazione.Elem_Cod = ELEM_COD.ZOO_CONSISTENZA;
                    break;

                case LAV_COD.LAVCOD_ALTRE_LAVORAZIONI_ZOO:
                    infoOperazione.IsZoo = true;
                    break;
            }

            // PregressoConMultiAvversita
            if (lav_cod == LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI || lav_cod == LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA)
                infoOperazione.PregressoConMultiAvversita = true;

            // TipoOperazioneAgenda
            infoOperazione.TipoOperazioneAgenda = tipoAttivita switch
            {
                Attivita.Tipo_Attivita.QuadernoDiCampagna => Attivita.Tipo_Attivita.QuadernoDiCampagna,
                Attivita.Tipo_Attivita.Ricetta => Attivita.Tipo_Attivita.Ricetta,
                _ => 0
            };

            return infoOperazione;
        }

        public static int GetCodiceProdotto(int pro_Cod, int mat_Cod, InfoOperazione infoOperazione)
        {
            int prodottoCod;

            if (mat_Cod == 0)
            {
                if (infoOperazione.IsSemina) // DT: compatibilità con il pregresso
                {
                    if (pro_Cod == 1)
                        pro_Cod = 0;
                }
                prodottoCod = pro_Cod;
            }
            else
            {
                prodottoCod = Math.Abs(mat_Cod); // DT: ho dovuto mettere abs perchè, in funzione dei contesti, può arrivare negativo
            }

            return prodottoCod;
        }

        public static bool Controlla_Se_Stesso_Prodotto(
            int elem_Cod_1, int pro_Cod_1, int mat_Cod_1, string lotto_1, int sa_Cod_1,
            int elem_Cod_2, int pro_Cod_2, int mat_Cod_2, string lotto_2, int sa_Cod_2)
        {
            bool stesso_Prodotto = false;

            int elem_Cod = elem_Cod_1 == elem_Cod_2 ? elem_Cod_1 : 0;

            switch (elem_Cod)
            {
                case CATEGORIE_MAGAZZINO.SEMENTI:
                case CATEGORIE_MAGAZZINO.TRASFORMATI_VEGETALI:
                    if (mat_Cod_1 == mat_Cod_2 &&
                        lotto_1.ToUpper() == lotto_2.ToUpper() &&
                        sa_Cod_1 == sa_Cod_2)
                    {
                        stesso_Prodotto = true;
                    }
                    break;

                default:
                    if (pro_Cod_1 == pro_Cod_2)
                        stesso_Prodotto = true;
                    break;
            }

            return stesso_Prodotto;
        }

        public static decimal GetDosePerMagazzino(decimal doseTrasformata, int udmIndicata)
        {
            decimal doseIndicata = 0;

            switch (udmIndicata)
            {
                case (int)Enum_UnitaMisura.Grammi:
                    doseIndicata = doseTrasformata * 1000;
                    break;
                case (int)Enum_UnitaMisura.Milligrammi:
                    doseIndicata = doseTrasformata * 1000000;
                    break;
                case (int)Enum_UnitaMisura.Quintali:
                    doseIndicata = doseTrasformata / 100;
                    break;
                case (int)Enum_UnitaMisura.Tonnellate:
                    doseIndicata = doseTrasformata / 1000;
                    break;
                case (int)Enum_UnitaMisura.Millilitri:
                    doseIndicata = doseTrasformata * 1000;
                    break;
                case (int)Enum_UnitaMisura.CentimetriCubi:
                    doseIndicata = doseTrasformata * 1000;
                    break;
                case (int)Enum_UnitaMisura.Metri_Cubi:
                    doseIndicata = doseTrasformata / 1000;
                    break;
                case (int)Enum_UnitaMisura.Litri:
                case (int)Enum_UnitaMisura.KG:
                case (int)Enum_UnitaMisura.Unita_Seme:
                case (int)Enum_UnitaMisura.Num_Piante:
                case (int)Enum_UnitaMisura.Confezioni:
                case (int)Enum_UnitaMisura.Numero:
                case (int)Enum_UnitaMisura.Numero_Trappole:
                    doseIndicata = doseTrasformata;
                    break;
                default:
                    doseIndicata = doseTrasformata;
                    break;
            }

            return doseIndicata;
        }

        public static AvversitaGruppo GetAvversitaGruppo(int av_cod, int av_gru)
        {
            AvversitaGruppo avversitaGruppo = null;

            if (av_cod != 0)
            {
                avversitaGruppo = new Avversita(av_cod);

                // DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                if (av_cod < 0)
                {
                    ((Avversita)avversitaGruppo).gruppo =
                        new GruppoAvversita(av_gru);
                }
                else
                {
                    ((Avversita)avversitaGruppo).gruppo =
                        new GruppoAvversita(0);
                }
            }
            else if (av_gru != 0)
            {
                avversitaGruppo = new GruppoAvversita(av_gru);
            }

            return avversitaGruppo;
        }

        public static int GetTipoRichiesto(string avCod, string avGru, int lav_Cod)
        {
            int tipoRichiesto = 0;
            if (avGru == "-1" && avCod == "-1")
                tipoRichiesto = (int)Enum_TipoFormulato.Coadiuvanti;
            else if (avGru == "-2" && avCod == "-2")
                tipoRichiesto = (int)Enum_TipoFormulato.Corroboranti_Fisiofarmaci;
            else
            {
                switch (lav_Cod)
                {
                    case int n when n == LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO: tipoRichiesto = (int)Enum_TipoFormulato.Antiparassitari; break;
                    case int n when n == LAV_COD.LAVCOD_CONCIA_SEME: tipoRichiesto = (int)Enum_TipoFormulato.Concianti; break;
                    case int n when n == LAV_COD.LAVCOD_DISSECCAMENTO: tipoRichiesto = (int)Enum_TipoFormulato.Disseccanti; break;
                    case int n when n == LAV_COD.LAVCOD_GEODISINFESTAZIONE: tipoRichiesto = (int)Enum_TipoFormulato.Geodisinfestanti; break;
                    case int n when n == LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE: tipoRichiesto = (int)Enum_TipoFormulato.Fitoregolatori; break;
                    case int n when n == LAV_COD.LAVCOD_DISERBO: tipoRichiesto = (int)Enum_TipoFormulato.Diserbanti; break;
                    case int n when n == LAV_COD.LAVCOD_CONFUSIONE_SESSUALE: tipoRichiesto = (int)Enum_TipoFormulato.ConfusioneSessuale; break;
                    case int n when n == LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE: tipoRichiesto = (int)Enum_TipoFormulato.DisorientamentoSessuale; break;
                }
            }
            return tipoRichiesto;
        }

        public static List<PrincipioAttivo> GetPrincipiAttivi(string principiAttiviTitoli, string principiAttiviPesi, string principiAttiviPercentualiSuperficieTrattabile)
        {
            var principiAttivi = new List<PrincipioAttivo>();

            var dictPrincipiPesi = new Dictionary<int, decimal>();
            foreach (var item in principiAttiviPesi.Split('|'))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var arr = item.Split('§');
                    if (!dictPrincipiPesi.ContainsKey(int.Parse(arr[0])))
                        dictPrincipiPesi.Add(int.Parse(arr[0]), decimal.Parse(arr[1], CultureInfo.InvariantCulture));
                }
            }

            var dictPrincipiPerc = new Dictionary<int, decimal>();
            foreach (var item in principiAttiviPercentualiSuperficieTrattabile.Split('|'))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var arr = item.Split('§');
                    if (!dictPrincipiPerc.ContainsKey(int.Parse(arr[0])))
                    {
                        dictPrincipiPerc.Add(int.Parse(arr[0]),
                            arr[1].Contains(",") ? decimal.Parse(arr[1], CultureInfo.GetCultureInfo("it-IT")) : decimal.Parse(arr[1], CultureInfo.InvariantCulture));
                    }
                }
            }

            if (!string.IsNullOrEmpty(principiAttiviTitoli))
            {
                foreach (var item in principiAttiviTitoli.Split('|'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var arr = item.Split('§');
                        if (arr.Length == 2)
                        {
                            var principio = new PrincipioAttivo
                            {
                                codice = int.Parse(arr[0]),
                                titolo = decimal.Parse(arr[1], CultureInfo.InvariantCulture)
                            };
                            int cod = int.Parse(arr[0]);
                            if (dictPrincipiPesi.ContainsKey(cod)) principio.peso = dictPrincipiPesi[cod];
                            if (dictPrincipiPerc.ContainsKey(cod)) principio.percentualeSuperficieTrattabile = dictPrincipiPerc[cod];
                            principiAttivi.Add(principio);
                        }
                    }
                }
            }
            return principiAttivi;
        }

        public static BufferZone GetBufferZone(string buffer)
        {
            var bufferZone = new BufferZone();
            if (!string.IsNullOrEmpty(buffer))
            {
                var arr = buffer.Split('|');
                if (arr.Length == 2)
                {
                    bufferZone.minimo = int.Parse(arr[0]);
                    bufferZone.massimo = int.Parse(arr[1]);
                }
            }
            else
            {
                bufferZone.minimo = 0;
                bufferZone.massimo = 0;
            }
            return bufferZone;
        }


        public static int GetDettaglioProdotto(string extra_str)
        {
            int dettProdotto = 0;
            if (extra_str != null) int.TryParse(extra_str, out dettProdotto);
            return dettProdotto;
        }

        public static int GetTipoFormulato(AvversitaGruppo avversitaGruppo, int lav_cod)
        {
            int tipoFormulato = 0;
            if (avversitaGruppo != null)
            {
                switch (avversitaGruppo.classType)
                {
                    case ClassType.Avversita:
                        tipoFormulato = GetTipoRichiesto(
                            avversitaGruppo.codice.ToString(),
                            ((Avversita)avversitaGruppo).gruppo.codice.ToString(),
                            lav_cod);
                        break;
                    case ClassType.GruppoAvversita:
                        tipoFormulato = GetTipoRichiesto(avversitaGruppo.codice.ToString(), "0", lav_cod);
                        break;
                }
            }
            return tipoFormulato;
        }

        public static List<DoseEtichetta> DosiEtichetta_from_Stringhe(string doseEtichettaDescr, string doseEtichettaValue, AgronicaCoreParametriServer objParametriServer)
        {
            List<DoseEtichetta> dosiEtichette = null;
            if (!string.IsNullOrEmpty(doseEtichettaDescr) && !string.IsNullOrEmpty(doseEtichettaValue))
            {
                dosiEtichette = new List<DoseEtichetta>();
                var arrayDosi = doseEtichettaDescr.Split(new[] { "<br>" }, StringSplitOptions.None);
                var arrayDosiValue = doseEtichettaValue.Split(new[] { "<br>" }, StringSplitOptions.None);

                for (int i = 0; i < arrayDosi.Length; i++)
                {
                    var doseEtichetta = GetDoseEtichettaFromDoseValue(arrayDosiValue[i], objParametriServer);
                    doseEtichetta.CodiceConcatenato = arrayDosiValue[i];
                    doseEtichetta.DescrizioneConcatenata = arrayDosi[i];
                    dosiEtichette.Add(doseEtichetta);
                }
            }
            return dosiEtichette;
        }

        public static Soglia GetSoglia(int soglia_Cod, decimal soglia_Quantita)
        {
            return new Soglia
            {
                codice = soglia_Cod,
                quantita = decimal.Parse(soglia_Quantita.ToString(), CultureInfo.InvariantCulture)
            };
        }

        private static DoseEtichetta GetDoseEtichettaFromDoseValue(string strDoseValue, AgronicaCoreParametri objParametri_Server)
        {
            DoseEtichetta doseEtichetta = null;
            if (!string.IsNullOrEmpty(strDoseValue) && strDoseValue != "0")
            {
                var arr = strDoseValue.Split('$');
                if (arr != null && arr.Length > 0)
                {
                    switch (arr.Length)
                    {
                        case 24:
                            doseEtichetta = new DoseEtichetta
                            {
                                codice = int.Parse(arr[0]),
                                descrizione = "",
                                DoseMin = decimal.Parse(arr[1],CultureInfo.GetCultureInfo("it-IT")),
                                DoseMax = decimal.Parse(arr[2],CultureInfo.GetCultureInfo("it-IT")),
                                Udm = new UnitaDiMisura { codice = int.Parse(arr[3]), simbolo = arr[4] },
                                AcquaMin = decimal.Parse(arr[5],CultureInfo.GetCultureInfo("it-IT")),
                                AcquaMax = decimal.Parse(arr[6],CultureInfo.GetCultureInfo("it-IT")),
                                UdmAcqua = new UnitaDiMisura { codice = int.Parse(arr[7]), simbolo = arr[8] },
                                Da_Epoca = arr[9],
                                A_Epoca = arr[10],
                                Limite = int.Parse(arr[11]),
                                UdmLimite = new UnitaDiMisura { codice = int.Parse(arr[12]), simbolo = arr[13] },
                                strCLTOSS_Grado = arr[14],
                                Flag_Fioritura = new FlagFioritura(int.Parse(arr[15])),
                                IntervalloTrattamenti_Min = int.Parse(arr[16]),
                                IntervalloTrattamenti_Max = int.Parse(arr[17]),
                                Mdi = new Mdi(int.Parse(arr[18])),
                                Flag_Protetto = new FlagProtetto(int.Parse(arr[19])),
                                FormulatiXAllegatiNormative_IDRiga = int.Parse(arr[20]),
                                DataSmaltimentoScorte = arr[21],
                                Gruppo_Dosaggi = int.Parse(arr[22]),
                                Num_Max_Interventi_Globali = int.Parse(arr[23])
                            };
                            break;
                        case 23:
                            doseEtichetta = new DoseEtichetta
                            {
                                codice = int.Parse(arr[0]),
                                descrizione = "",
                                DoseMin = decimal.Parse(arr[1], CultureInfo.GetCultureInfo("it-IT")),
                                DoseMax = decimal.Parse(arr[2], CultureInfo.GetCultureInfo("it-IT")),
                                Udm = new UnitaDiMisura { codice = int.Parse(arr[3]), simbolo = arr[4] },
                                AcquaMin = decimal.Parse(arr[5], CultureInfo.GetCultureInfo("it-IT")),
                                AcquaMax = decimal.Parse(arr[6], CultureInfo.GetCultureInfo("it-IT")),
                                UdmAcqua = new UnitaDiMisura { codice = int.Parse(arr[7]), simbolo = arr[8] },
                                Da_Epoca = arr[9],
                                A_Epoca = arr[10],
                                Limite = int.Parse(arr[11]),
                                UdmLimite = new UnitaDiMisura { codice = int.Parse(arr[12]), simbolo = arr[13] },
                                strCLTOSS_Grado = "",
                                Flag_Fioritura = new FlagFioritura(int.Parse(arr[14])),
                                IntervalloTrattamenti_Min = int.Parse(arr[15]),
                                IntervalloTrattamenti_Max = int.Parse(arr[16]),
                                Mdi = new Mdi(int.Parse(arr[17])),
                                Flag_Protetto = new FlagProtetto(int.Parse(arr[18])),
                                FormulatiXAllegatiNormative_IDRiga = int.Parse(arr[19]),
                                DataSmaltimentoScorte = arr[20],
                                Gruppo_Dosaggi = int.Parse(arr[21]),
                                Num_Max_Interventi_Globali = int.Parse(arr[22])
                            };
                            break;
                        default:
                            doseEtichetta = new DoseEtichetta { codice = int.Parse(arr[0]) };
                            break;
                    }
                }
            }
            return doseEtichetta;
        }
    }
}
