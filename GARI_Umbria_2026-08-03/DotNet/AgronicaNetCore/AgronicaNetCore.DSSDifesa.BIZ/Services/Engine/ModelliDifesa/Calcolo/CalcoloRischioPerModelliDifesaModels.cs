using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Input payload for DS05-BL risk model parallel execution.
    /// </summary>
    public class CalcoloRischioPerModelliDifesaRequest
    {
        public List<ModelloDifesaInput> Modelli { get; set; } = new();
        public List<MeteoDataPointInput> MeteoData { get; set; } = new();
        public int LivelloDettaglio { get; set; }
    }

    /// <summary>
    /// Output payload for DS05-BL risk model parallel execution.
    /// </summary>
    public class CalcoloRischioPerModelliDifesaResponse
    {
        public List<ModelloRisultatoOutput> RisultatiModelli { get; set; } = new();
        public string TimestampRaccolta { get; set; } = string.Empty;
        public bool TimeoutGlobaleRaggiunto { get; set; }
    }

    /// <summary>
    /// Model details to invoke for pest risk execution.
    /// </summary>
    public class ModelloDifesaInput
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Single meteorological data point in DS05 input.
    /// </summary>
    public class MeteoDataPointInput
    {
        public string dataOra { get; set; } = string.Empty;
        public decimal? temp { get; set; }
        public decimal? prec { get; set; }
        public decimal? relHum { get; set; }
        public decimal? lw { get; set; }
    }

    /// <summary>
    /// Per-model execution result as requested by DS05 output schema.
    /// </summary>
    public class ModelloRisultatoOutput
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ModelGroup { get; set; }
        public EngineExecutionResponseSynteticOutput? OutputSintetico { get; set; }
        public EngineExecutionResponseAnalyticOutput? OutputAnalitico { get; set; }

        public string TimestampCalcolo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? MessaggioErrore { get; set; }
    }

    /// <summary>
    /// Intermediate API response to the model execution request.
    /// </summary>
    public class EngineExecutionRequestResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ReceivedAt { get; set; }
    }

    /// <summary>
    /// Poll response for status of the execution request.
    /// </summary>
    public class EngineExecutionStatusResponse
    {
        public string Status { get; set; } = string.Empty; // TODO, DONE, ERROR
        public string? ModelGroup { get; set; }
        //public string? ResponseDataStr { get; set; }
        public EngineExecutionResponseSynteticOutput? SynteticOutput {  get; set; }
        public EngineExecutionResponseAnalyticOutput? AnalyticOutput { get; set; }
        public EngineExecutionErrorData? Error { get; set; }
    }

    public class EngineExecutionResponseSynteticOutput
    {
        //public string? ModelGroup { get; set; }
        public string? Model { get; set; }
        public int? Status { get; set; }
        public string? Disease { get; set; }
        public string? Message { get; set; }
        public string? CropType { get; set; }
        public EngineExecutionResponseSynteticOutputSuccessData? ResultSuccess { get; set; }
        public string? StatusVerbose { get; set; }
        public int? ResultProgress { get; set; }
        public string? WarningMessage { get; set; }
    }

    public class EngineExecutionResponseSynteticOutputSuccessData
    {
        public EngineExecutionResponseSynteticOutputSuccessDataBand[] Bands { get; set; } = new EngineExecutionResponseSynteticOutputSuccessDataBand[3];
        public int RiskIndex { get; set; } = 0;
    }

    public class EngineExecutionResponseSynteticOutputSuccessDataBand
    {
        public int Max {  get; set; }
        public int Min { get; set; }
        public string Name { get; set; } = "";
        public string Color { get; set; } = "#000000";
    }

    public class EngineExecutionResponseAnalyticOutput
    {
        //public string? ModelGroup { get; set; }
        public string? Modello_Tabella1 { get; set; }
    }

    public class EngineExecutionErrorData
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
}
