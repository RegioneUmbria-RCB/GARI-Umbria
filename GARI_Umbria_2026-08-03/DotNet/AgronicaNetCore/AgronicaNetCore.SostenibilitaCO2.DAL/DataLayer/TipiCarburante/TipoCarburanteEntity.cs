namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.TipiCarburante
{
    /// <summary>
    /// Entity che rappresenta una riga della tabella <c>Carburanti</c>.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input <c>carburanti[].tipo_carburante</c>.
    /// </summary>
    public class TipoCarburanteEntity
    {
        /// <summary>Codice identificativo del tipo di carburante (<c>Car_Cod</c>).</summary>
        public int Car_Cod { get; set; }

        /// <summary>Descrizione del tipo di carburante (<c>Car_Des</c>), es. Gasolio, Benzina, GPL, Gas Metano.</summary>
        public string Car_Des { get; set; } = string.Empty;
    }
}
