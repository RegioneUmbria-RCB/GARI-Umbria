namespace AgronicaNetCore.RischiMeteo.BIZ.Services.PerimetroRaccolti
{
    /// <summary>
    /// Risposta di <see cref="IPerimetroRaccoltiRischiService.GetPerimetroAsync"/>.
    /// Riferimento spec: DS01-BL CaricamentoPerimetroFiltrato — Output.
    /// </summary>
    public class PerimetroRaccoltiResult
    {
        /// <summary>Esito del caricamento: <c>success</c>, <c>no_data</c> o <c>error</c>.</summary>
        public string StatoCaricamento { get; set; } = string.Empty;

        /// <summary>PIVA della filiera radice usata come punto di partenza del cono di visibilità.</summary>
        public string PivaFiliera { get; set; } = string.Empty;

        /// <summary>Righe del perimetro. Vuota se <c>StatoCaricamento</c> è <c>no_data</c>.</summary>
        public List<PerimetroRigaItem> Perimetro { get; set; } = new();
    }
}
