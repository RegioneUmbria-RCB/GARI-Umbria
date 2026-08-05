namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale
{
    /// <summary>
    /// Elemento di dropdown per la selezione del tipo di carburante.
    /// Mappa la tabella <c>Carburanti</c> in una struttura usabile dal frontend (valore + etichetta).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input <c>carburanti[].tipo_carburante</c>.
    /// </summary>
    public class TipoCarburanteDropdownItem
    {
        /// <summary>Chiave numerica della riga (<c>Car_Cod</c>), usata come <c>value</c> del dropdown.</summary>
        public int Valore { get; set; }

        /// <summary>Etichetta leggibile (<c>Car_Des</c>), usata come <c>label</c> del dropdown.</summary>
        public string Etichetta { get; set; } = string.Empty;
    }
}
