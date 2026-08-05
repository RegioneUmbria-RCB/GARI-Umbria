using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.baseClass;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using System;
using System.Runtime.InteropServices;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class Controllo_Inserimento_Dose_Prodotto
    {
        public Attivita.Tipo_Attivita tipoAttivita { get; set; }
        public Attivita.Stati statoAttivita { get; set; }
        public List<Parametri_Aggiuntivi_ControllaDosi> parametri_aggiuntivi_list { get; set; }
        public int id_agenda { get; set; }
        public int raccoglitore_cod { get; set; }
        public Lavorazione lavorazione { get; set; }
        public List<RowGridImpianti> row_grid_impianti { get; set; }
        public List<RowGridProdottiDaTrattare> row_grid_prodottiDaTrattare { get; set; }
        public DettaglioTrattamento dettaglioTrattamento { get; set; }
        public DettaglioSemina dettaglioSemina { get; set; }
        public DettaglioFertilizzazione dettaglioFertilizzazione { get; set; }
        public UnitaDiMisura unitadiMisura { get; set; }
        public AvversitaGruppo avversitaGruppo { get; set; }
        public Soglia Soglia_Avversita { get; set; }
        public RisorsaAcqua risorsaAcqua { get; set; }
        public List<DoseEtichetta> dosi_Etichetta { get; set; }
        public decimal N { get; set; }
        public decimal N_Utile { get; set; }
        public decimal P { get; set; }
        public decimal K { get; set; }
        public decimal Cu { get; set; }
        public int flagTipoDose { get; set; }
        public int flagDoseQuantitaTotale { get; set; }
        public decimal doseHa { get; set; }
        public decimal doseHl { get; set; }
        public decimal doseQ { get; set; }
        public decimal quantitaTotale { get; set; }
        public string Data { get; set; }
        public Tipo_Ricetta tipoRicetta { get; set; }
        public Disciplinare disciplinare { get; set; }
        public UtilizzoTerreno utilizzoTerreno { get; set; }
        public Fabbricato Magazzino { get; set; }
        public string Lotto { get; set; }
        /// <summary>
        /// Quando TipoCentroCosto = ProdottoDaTrattareCDC
        /// </summary>
        public decimal qtaProdotto_Trattata { get; set; }
        /// <summary>
        /// Quando TipoCentroCosto = EsercizioCDC
        /// </summary>
        public decimal sup_Trattata { get; set; }
        public decimal sup_Calcolata { get; set; }
        public BaseCodeDescr tipo_Semina { get; set; }
        public decimal efficienza { get; set; }
        public int ricetta_cod { get; set; }
        public decimal DoseConsentitaDiserbo { get; set; }
        public List<RowGridDosi> row_grid_dosi { get; set; }
        public List<RowGridDosi> row_grid_dosi_altre_operazioni { get; set; }
        public int ricetta_operazione_cod { get; set; }
        public int DosiProdottiGridrowId { get; set; }
        public Pua pua { get; set; }
        public Fabbricato Magazzino_Esterno { get; set; }
        public int Categoria_Magazzino { get; set; }
        public Impresa impresa { get; set; }
        public bool Visualizza_Magazzini_Esterni { get; set; }
        public RowGridDosi Original_Row_Value { get; set; }
        public Fabbricato Magazzino_Innesco { get; set; }
        public string Lotto_Innesco { get; set; }
        public UnitaDiMisura UdM_Innesco { get; set; }
        public Fabbricato Magazzino_Agenzia_Innesco { get; set; }
        public decimal DoseTot_Ha_Innesco { get; set; }
    }

    public class RowGridDosi
    {
        public Lavorazione Operazione { get; set; }
        public List<PrincipioAttivo> PrincipiAttivi { get; set; }
        public RisorsaProdotto Prodotto { get; set; }
        public int Fr_Cod { get; set; }
        public UnitaDiMisura UdM { get; set; }
        public decimal Dose_Ha { get; set; }
        public decimal Dose_Hl { get; set; }
        public decimal Dose_Q { get; set; }
        public decimal DoseTot_Ha { get; set; }
        public decimal N { get; set; }
        public decimal N_Utile { get; set; }
        public decimal P { get; set; }
        public decimal K { get; set; }
        public decimal Cu { get; set; }
        public int flagTipoDose { get; set; }
        public int flagDoseQuantitaTotale { get; set; }
        public decimal Efficienza { get; set; }
        public int Sa_Cod { get; set; }
        public int Fabbricato_Cod { get; set; }
        public string Piva { get; set; }
        public string Lotto { get; set; }
        public decimal Sup_Calcolata { get; set; }
        public List<DoseEtichetta> Dosi_Etichetta { get; set; }
        public Soglia Soglia_Avversita { get; set; }
        public AvversitaGruppo Avversita { get; set; }
        public int DosiProdottiGridrowId { get; set; }
        public string Polverulento { get; set; }
        public Fabbricato Magazzino_Esterno { get; set; }
        public Fabbricato Magazzino_Innesco { get; set; }
        public string Lotto_Innesco { get; set; }
        public UnitaDiMisura UdM_Innesco { get; set; }
        public Fabbricato Magazzino_Agenzia_Innesco { get; set; }
        public decimal DoseTot_Ha_Innesco { get; set; }
        public string Piva_Innesco { get; set; }
        public int Sa_Cod_Innesco { get; set; }
        public int Fabbricato_Cod_Innesco { get; set; }
    }

    public class RowGridImpianti
    {
        public string PIVA { get; set; }
        public int SA_COD { get; set; }
        public int APPEZZA { get; set; }
        public int ID_REG { get; set; }
        public int Progetto_Cod { get; set; }
        public string APP_NOME { get; set; }
        public decimal Sup_Imp { get; set; }
        public decimal Sup_Imp_help { get; set; }
        public decimal DistBZ_CorpiIdrici { get; set; }
        public decimal DistBZ_AreeResPub { get; set; }
        public decimal DistBZ_Allevamenti { get; set; }
        public decimal DistBZ_VegNatNonColt { get; set; }
        public decimal Sup_Riduzione_BufferZone { get; set; }
        public decimal Perc_Riduzione_Deriva { get; set; }
        public decimal SupBZ_Riduzione { get; set; }
        public string Validita_Inizio_Distinta { get; set; }
        public string Validita_Fine_Distinta { get; set; }
        public string Data_Raccolta { get; set; }
        public string Data_Raccolta_Prevista { get; set; }
    }

    public class RowGridProdottiDaTrattare
    {
        public string PIVA { get; set; }
        public int SA_COD { get; set; }
        public int FABBRICATO_COD { get; set; }

        public int Mat_Cod { get; set; }
        public int Progetto_Cod { get; set; }
        public string Lotto { get; set; }

        public int Elem_Cod { get; set; }

        public string Codice_Alfanumerico { get; set; }
        public string Prodotto { get; set; }

        /// <summary>
        /// Espressa in QUINTALI!
        /// </summary>
        public decimal QtaProdotto { get; set; }
    }

    public class Parametri_Aggiuntivi_ControllaDosi
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Key_Parametri_Aggiuntivi_ControlloDosi
    {
        public const String mostraWarning_CheckGiacenza = "mostraWarning_CheckGiacenza";
        public const String mostraWarning_CheckMassimali = "mostraWarning_CheckMassimali";
        public const String mostraWarning_CheckEtichetta = "mostraWarning_CheckEtichetta";
        public const String mostraWarning_CheckProdottoInRibaltamento = "mostraWarning_CheckProdottoInRibaltamento";
    }

    public class MacroelementData
    {
        public double InOperazione { get; set; }
        public double Massimo { get; set; }
        public double Distribuito { get; set; }
        public double Distribuito_Ricetta { get; set; }

        public string StrError { get; set; } = "";
        public string StrErrorRecipe { get; set; } = "";

        public bool EccedeMassimale()
        {
            return (Distribuito + InOperazione - Massimo) > 0.001;
        }

        public bool EccedeMassimaleRicetta()
        {
            return (Distribuito_Ricetta + InOperazione - Massimo) > 0.001;
        }
    }
}
