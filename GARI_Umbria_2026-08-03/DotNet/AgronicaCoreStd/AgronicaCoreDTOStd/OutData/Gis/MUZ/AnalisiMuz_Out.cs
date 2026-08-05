namespace AgronicaCoreDTOStd.OutData.Gis.MUZ
{
    /// <summary>
    /// Dati analitici estratti per una singola area omogenea MUZ.
    /// </summary>
    public class AnalisiMuz_Out
    {
        public int muz_id { get; set; }
        public double sabbia_perc { get; set; }
        public double limo_perc { get; set; }
        public double argilla_perc { get; set; }
        public double pH { get; set; }
        public double SO { get; set; }
        public double rapporto_C_N { get; set; }
    }
}
