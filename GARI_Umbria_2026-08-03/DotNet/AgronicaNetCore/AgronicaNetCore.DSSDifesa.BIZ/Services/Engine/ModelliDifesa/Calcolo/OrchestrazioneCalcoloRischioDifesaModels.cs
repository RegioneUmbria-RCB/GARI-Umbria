using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Input model for full DSS Difesa adversities risk orchestration.
    /// Based on DS07-BL_ Orchestrazione Calcolo Rischio Completa.
    /// </summary>
    public class OrchestrazioneCalcoloRischioDifesaRequest
    {
        public string? StationCod { get; set; }
        public GeographicalCoordinates? Coordinates { get; set; }
        public string DataInizio { get; set; } = string.Empty;
        public string DataFine { get; set; } = string.Empty;
        public string CropCode { get; set; } = string.Empty;
        public string? VarCode { get; set; }
        public List<string>? ModelliCodici { get; set; } = new();
        public int LivelloDettaglio { get; set; }
    }

    /// <summary>
    /// Output model for full DSS Difesa adversities risk orchestration.
    /// </summary>
    public class OrchestrazioneCalcoloRischioDifesaResponse
    {
        public List<ModelloRisultatoOutput> RisultatiModelli { get; set; } = new();
        public string TimestampRaccolta { get; set; } = string.Empty;
        public bool TimeoutGlobaleRaggiunto { get; set; }
    }
}
