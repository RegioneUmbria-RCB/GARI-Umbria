using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace InData.AntaresTrace
{
    /// <summary>
    /// DTO per il prodotto fertilizzante utilizzato
    /// </summary>
    public class ProdottRaccoltaDto : IProdottoDto
    {
        /// <summary>
        /// Identificativo unico del prodotto fertilizzante
        /// </summary>
        [Required(ErrorMessage = "L'ID prodotto è obbligatorio")]
        [JsonPropertyName("idProdotto")]
        public int IdProdotto { get; set; }

    }
}