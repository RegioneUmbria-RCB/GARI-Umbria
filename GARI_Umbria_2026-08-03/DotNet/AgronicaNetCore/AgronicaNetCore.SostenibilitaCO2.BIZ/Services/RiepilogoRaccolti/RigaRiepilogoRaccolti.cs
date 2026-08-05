namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti
{
    /// <summary>
    /// Rappresenta una singola riga della tabella di riepilogo raccolti CO₂.
    /// La chiave logica è la combinazione univoca: Azienda + Appezzamento + Specie Colturale + Esercizio.
    /// </summary>
    public class RigaRiepilogoRaccolti
    {
        /// <summary>Ragione sociale dell'azienda figlia (<c>Imprese.Rag_Soc</c>).</summary>
        public string Azienda { get; set; } = string.Empty;
        public string PartitaIva { get; set; } = string.Empty;

        /// <summary>Nome dell'appezzamento (<c>Appezzamento.APP_NOME</c>).</summary>
        public string Appezzamento { get; set; } = string.Empty;

        /// <summary>Nome dell'esercizio/progetto (<c>Imprese_Progetti.Progetto_Nome</c>).</summary>
        public string Esercizio { get; set; } = string.Empty;

        /// <summary>Codice ISO nazione (da <c>Lista_Regioni → Lista_Stati</c>).</summary>
        public string Nazione { get; set; } = string.Empty;

        /// <summary>Descrizione della regione (<c>Lista_Regioni.Reg_Des</c>).</summary>
        public string Regione { get; set; } = string.Empty;
        public string ISTAT_reg { get; set; }

        /// <summary>Codice provincia (<c>Indirizzi.pro_cod</c>).</summary>
        public string Provincia { get; set; } = string.Empty;

        /// <summary>Superficie dell'impianto in ettari (<c>Reg_Impianti.sup_imp</c>).</summary>
        public decimal Superficie { get; set; }

        /// <summary>Descrizione della specie colturale (<c>SpecieVegetali.Veg_Des</c>).</summary>
        public string SpecieColturale { get; set; } = string.Empty;

        /// <summary>Descrizione dei prodotti raccolti, separati da virgola (<c>Materie_Prime.Mat_Des</c>).</summary>
        public string ProdottiRaccolti { get; set; } = string.Empty;

        /// <summary>Codici lotto, separati da virgola (<c>Movimenti_dettagli.Lotto</c>).</summary>
        public string CodiceLotti { get; set; } = string.Empty;

        /// <summary>Data dell'operazione di raccolta più recente.</summary>
        public DateTime? DataUltimaRaccolta { get; set; }

        /// <summary>Peso totale raccolto in Kg (<c>SUM(Movimenti_dettagli.Qta)</c> dove <c>Udm_Cod = 2</c>).</summary>
        public decimal TotaleRaccoltaKg { get; set; }
    }
}
