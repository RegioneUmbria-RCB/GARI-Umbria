namespace AgronicaCoreDTOStd.OutData.Gis.MUZ
{
    /// <summary>
    /// Proprietà pedologiche e di classificazione
    /// di una singola MUZ generata dall'engine "SAT e Grandi layer".
    /// Tutti i campi numerici sono nullable poiché l'engine può omettere valori non disponibili.
    /// </summary>
    public class SatMuzProperties_Out
    {
        public string IdMuz { get; set; }
        public string IdClasse { get; set; }
        public int? IdGeom { get; set; }
        public string IdSoil { get; set; }
        public double? Area { get; set; }

        /// <summary>Percentuale di argilla (clay).</summary>
        public double? Clay { get; set; }

        /// <summary>Percentuale di limo (silt).</summary>
        public double? Silt { get; set; }

        /// <summary>Percentuale di sabbia (sand).</summary>
        public double? Sand { get; set; }

        public string SoilTexture { get; set; }

        /// <summary>Conducibilità elettrica.</summary>
        public double? Ce { get; set; }

        /// <summary>Potassio.</summary>
        public double? K { get; set; }

        /// <summary>Azoto totale.</summary>
        public double? NTot { get; set; }

        /// <summary>Carbonio organico.</summary>
        public double? Oc { get; set; }

        /// <summary>Carbonio organico del suolo.</summary>
        public double? Soc { get; set; }

        public double? Ph { get; set; }

        /// <summary>Fosforo.</summary>
        public double? P { get; set; }

        /// <summary>Densità apparente del suolo (Bulk density of fine earth).</summary>
        public double? Bdod { get; set; }

        /// <summary>Capacità di scambio cationico (meq/100g).</summary>
        public double? CscMeq { get; set; }

        public double? Cod { get; set; }

        /// <summary>Sostanza organica.</summary>
        public string So { get; set; }

        public string Ca { get; set; }
        public string Mg { get; set; }
        public string Na { get; set; }
    }
}
