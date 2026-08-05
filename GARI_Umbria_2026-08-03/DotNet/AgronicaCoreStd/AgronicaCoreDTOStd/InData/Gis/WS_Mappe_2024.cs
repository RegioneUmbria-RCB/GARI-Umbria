using AgronicaCoreModelsSTD.Gis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class CalendarInitDto
    {
   
        public string wkt { get; set; }

        public string sensor { get; set; }

        public string source { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }

    public class GdalGridDto
    {
        public string path { get; set; }
        
        public string wkt { get; set; }

        public int squareSide { get; set; }

        public int srid { get; set; }
    }

    public class GisSatSentinelOverlayModel
    {
        public string DataRiferimento { get; set; }

        public List<GisSatSentinelOverlayPassageModel> Passaggi { get; set; }
    }

    public class GisSatSentinelOverlayPassageModel
    {
        public GisSatSentinelOverlayTileModel Tile { get; set; }

        public string Url { get; set; }

        public List<GisSatSentinelOverlaySensoreModel> Sensore { get; set; }
    }

    public class GisSatSentinelOverlayTileModel
    {
        public string Tile { get; set; }

        public string PoligonoWktBoundingBox { get; set; }

        public string GEORiferimento_COD { get; set; }
    }

    public class GisSatSentinelOverlaySensoreModel
    {
        public string CodiceSensore { get; set; }

        public string Descrizione { get; set; }

        public List<GisSatSentinelOverlaySensoreDatoRilevato> DatiRilevati { get; set; }
    }

    public class GisSatSentinelOverlaySensoreDatoRilevato
    {
        //<DataMember(Name:="DataRiferimento")>
        public string FormattedReturnDate { get; set; }

        // <IgnoreDataMember>
        public DateTime DataRiferimento
        {
            get
            {
                return DateTime.ParseExact(FormattedReturnDate, "o", CultureInfo.InvariantCulture);
            }
            set
            {
                FormattedReturnDate = value.ToString("o");
            }
        }

        //<DataMember>
        public double Valore { get; set; }
    }

    public class GisDataReadRvalModel<T>
    {
        [JsonProperty("Risultato_Elaborazione")]
        public GeoJson_New<T> MyGeoJson { get; set; }
        [JsonProperty("PfRateoSrv_In")]
        public string PfRateoSrvIn { get; set; }
        [JsonProperty("Messaggi")]
        public string Messages { get; set; }
        [JsonProperty("UtenteRichiedente")]
        public string RequestingUser { get; set; }
    }
}
