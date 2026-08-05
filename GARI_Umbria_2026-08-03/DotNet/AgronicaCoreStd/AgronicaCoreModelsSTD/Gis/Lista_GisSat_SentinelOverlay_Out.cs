using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Tutta questa libreria è una copia di AgronicaCoreModelloInSviluppo.Gis_Sat_Sentinel_Overlay_list, 
    /// implementata per poterla utilizzarla all'interno del CoreAPI
    /// </summary>
    public class Lista_GisSat_SentinelOverlay_Out
    {
        public List<Gis_Sat_Sentinel_Overlay> ListaOverlayer { get; set; }
    }

    public class Gis_Sat_Sentinel_Overlay
    {
        public string DataRiferimento { get; set; }
        public string DataRiferimentoSAT { get; set; }
        public List<Gis_Sat_Sentinel_Overlay_Passaggio> Passaggi { get; set; }
    }

    public class Gis_Sat_Sentinel_Overlay_Passaggio
    {
        public Gis_Sat_Sentinel_Overlay_Tile Tile { get; set; }
        public string url { get; set; }
        public List<Gis_Sat_Sentinel_Overlay_Sensore> Sensore { get; set; }
    }

    public class Gis_Sat_Sentinel_Overlay_Tile
    {
        public string Tile { get; set; }
        public string PoligonoWktBoundingBox { get; set; }
        public string GEORiferimento_COD { get; set; }
    }

    public class Gis_Sat_Sentinel_Overlay_Sensore
    {
        public string CodiceSensore { get; set; }
        public string Descrizione { get; set; }
        public double perc_cloudfree { get; set; }
        public List<Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato> DatiRilevati { get; set; }
    }

    [DataContract]
    public class Gis_Sat_Sentinel_Overlay_Sensore_DatoRilevato
    {
        [DataMember(Name = "DataRiferimento") ]
        public string FormattedReturnDate { get; set; }

        [IgnoreDataMember]
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

        [DataMember]
        public double Valore { get; set; }
    }

}
