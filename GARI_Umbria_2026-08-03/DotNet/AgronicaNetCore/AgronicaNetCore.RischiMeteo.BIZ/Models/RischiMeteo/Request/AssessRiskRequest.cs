namespace AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Request
{
    /// <summary>Finestra temporale di riferimento per il calcolo dei rischi.</summary>
    public class FinestraEventiRequest
    {
        public string? start { get; set; }
        public string? end { get; set; }
    }

    /// <summary>Informazioni sulla coltura (codifica, specie, cultivar).</summary>
    public class ColturaRequest
    {
        public string? codifica { get; set; }
        public int? codice_specie { get; set; }
        public int? codice_cultivar { get; set; }
    }

    /// <summary>Geometria geografica dell'impianto (centroide, poligono, proiezione).</summary>
    public class GeoImpiantoRequest
    {
        public string? centroide_wkt { get; set; }
        public string? poligono_wkt { get; set; }
        public string? epsg { get; set; }
    }

    /// <summary>Osservazione fenologica opzionale (data e codice BBCH).</summary>
    public class OsservazioneFenologicaRequest
    {
        public string? data { get; set; }
        public string? bbch { get; set; }
    }

    /// <summary>
    /// Payload di richiesta per l'endpoint <c>POST /v1/rischi/valutazione</c> dell'Engine M2.
    /// Riferimento spec: OpenAPI Engine Rischi Meteoclimatici M2.
    /// </summary>
    public class AssessRiskRequest
    {
        public FinestraEventiRequest? finestra_eventi { get; set; }
        public ColturaRequest? coltura { get; set; }
        public GeoImpiantoRequest? geo_impianto { get; set; }
        public string? data_semina_o_trapianto { get; set; }
        public OsservazioneFenologicaRequest? osservazione_fenologica { get; set; }
        public List<string>? seleziona_avversita { get; set; }
        public bool dettaglio_eventi { get; set; }
    }
}
