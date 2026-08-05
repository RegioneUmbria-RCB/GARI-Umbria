using System.Collections.Generic;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// DAL filter for reading H2O lookup rows for a batch of lots.
    /// See DS11-API Endpoint POST Sintesi Lotti Raccolti Sostenibilita H2O, section "Specifiche Tecniche".
    /// </summary>
    public class GetSostH2OLottiBatch
    {
        public string Filiera { get; set; } = string.Empty;

        public string CodProdottoFmp { get; set; } = string.Empty;

        public List<GetSostH2OLottiBatchItem> Lotti { get; set; } = new List<GetSostH2OLottiBatchItem>();
    }

    /// <summary>
    /// Single company/lot key used by the batch DAL filter.
    /// </summary>
    public class GetSostH2OLottiBatchItem
    {
        public string Azienda { get; set; } = string.Empty;

        public string CodLottoFmp { get; set; } = string.Empty;
    }
}
