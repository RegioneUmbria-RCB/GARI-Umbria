using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace AgronicaCoreModelsSTD.Gis
{

    /// <summary>
    /// 
    /// </summary>
    public enum FeatureType
    {
        Point = 1,
        MultiPoint = 2,
        LineString = 3,
        MultiLineString = 4,
        Polygon = 5,
        MultiPolygon = 6,
        Raster = 100
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GisDataReadRval_New<T>
    {
        public GeoJson_New<T> myGeoJson { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GeoJson_Geometry_New
    {
        private FeatureType _type;
        //public object coordinates;
        private object _coordinates;
        public object coordinates
        {
            get
            {
                switch (_type)
                {
                    case FeatureType.Point:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[]>(_coordinates.ToString());
                        } else
                        {
                            return (double[])_coordinates;
                        }
                    case FeatureType.MultiPoint:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[][]>(_coordinates.ToString());
                        }
                        else
                        {
                            return (double[][])_coordinates;
                        }
                    case FeatureType.LineString:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[][]>(_coordinates.ToString());
                        }
                        else
                        {
                            return (double[][])_coordinates;
                        }
                    case FeatureType.MultiLineString:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[][][]>(_coordinates.ToString());
                        }
                        else
                        {
                            return (double[][][])_coordinates;
                        }
                    case FeatureType.Polygon:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[][][]>(_coordinates.ToString());
                        } else
                        {
                            return (double[][][])_coordinates;
                        }
                    case FeatureType.MultiPolygon:
                        if (_coordinates.GetType().Name == "JArray")
                        {
                            return JsonConvert.DeserializeObject<double[][][][]>(_coordinates.ToString());
                        }
                        else
                        {
                            return (double[][][][])_coordinates;
                        }
                    default:
                        return null;
                }
            }

            set
            {
                _coordinates = value;
            }
        }

        public string type
        {
            get
            {
                return Enum.GetName(typeof(FeatureType), _type);
            }

            set 
            {
                _type = (FeatureType)Enum.Parse(typeof(FeatureType), value);
            }
        }

        public GeoJson_Geometry_New()
        {

        }

        public GeoJson_Geometry_New(FeatureType type, string data)
        {
            _type = type;
            switch (_type)
            {
                case FeatureType.Point:
                    _coordinates = JsonConvert.DeserializeObject<double[]>(data);
                    break;
                case FeatureType.MultiPoint:
                    _coordinates = JsonConvert.DeserializeObject<double[][]>(data);
                    break;
                case FeatureType.LineString:
                    _coordinates = JsonConvert.DeserializeObject<double[][]>(data);
                    break;
                case FeatureType.MultiLineString:
                    _coordinates = JsonConvert.DeserializeObject<double[][][]>(data);
                    break;
                case FeatureType.Polygon:
                    _coordinates = JsonConvert.DeserializeObject<double[][][]>(data);
                    break;
                case FeatureType.MultiPolygon:
                    _coordinates = JsonConvert.DeserializeObject<double[][][][]>(data);
                    break;
            }
        }
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeoJson_New<T>
    {
        public GeoJson_Shape_New<T> geoJsonCaricato { get;set;}
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeoJson_Shape_New<T>
    {
        public string type { get; set; }
        public List<GeoJson_Feature_New<T>> features { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeoJson_Feature_New<T>
    {
        public string type { get; set; }
        public GeoJson_Geometry_New geometry { get; set; }
        public T properties { get; set; }
    }

    public class GeoJSONAgroGisProp
    {
        public string layer;
        public string StandardEntita_layerDiAppartenenza;
        public string StandardEntita_layerDiAppartenenza_Des;
        public string StandardEntita_layerDiAppartenenza_Icona32;
        public string id;
        public string tipoicona;
        public string zindex;
        public string Entita_Cod;
        public string Entita_GUID;
        public string veg_cod;
        public string inserimento;
        public string flag_gps;
        public string etichetta;
        public string modifica;
        public string cancellazione;
        public string informazioni;
        public string chiavealbero;
        public string Testo;
        public string AppIdRate;
        public string TipologiaGML;
        public int InOsservazione;
        public string Colore_Primario;
        public string Colore_Retinatura;
        public int Trasparenza;
        public string ParametriVisualizzazioneLayer;
        public int Area_Cod;
        public int GMapsZoomLevel;
        public double TotalOriginalArea;
        public int TotalOriginalFeatureNumber;
        public string Clustered;

        public GeoJSONAgroGisProp() { }
    }

    public class GeoJSONAgroGisPropTreeNode
    {
        public string id;
        public string imageUrl;
        public string style;
        public string text;
        public string type;
        public DateTime startDate;
        public DateTime endDate;

        public GeoJSONAgroGisPropTreeNode() { }
    }

    public class GeoJSONAgroGisPropTreeNodeKey
    {
        public int TipoNodo { get; set; } = 0;
        public string Piva { get; set; } = "0";
        public int Sa_Cod { get; set; } = 0;
        public int Campo_Cod { get; set; } = 0;
        public int Appezza { get; set; } = 0;
        public int Id_Imp { get; set; } = 0;
        public string p_Part_Cod { get; set; } = "0";
        public string p_Provincia_Cod { get; set; } = "0";
        public string p_Comune_Cod { get; set; } = "0";
        public string p_Sezione { get; set; } = "0";
        public string p_Foglio { get; set; } = "0";
        public string p_Numero { get; set; } = "0";
        public string p_Subalterno { get; set; } = "0";
        public string Cod_Fiscale { get; set; } = "0";
        public int Fabbricato_Cod { get; set; } = 0;
        public int Prodotto_Cod { get; set; } = 0;
        public string Data_Lavorazione { get; set; } = "0";
        public int Analisi_Certificato_Cod { get; set; } = 0;
        public int Analisi_Testata_Cod { get; set; } = 0;
        public int Analisi_Dettaglio_Cod { get; set; } = 0;
        public int Analisi_Campione_Cod { get; set; } = 0;
        public int PianoConcimazioneTestata_Cod { get; set; } = 0;
        public int Progetto_Cod { get; set; } = 0;
        public int Programmazione_Cod { get; set; } = 0;
        public int Programmazione_Entita_Cod { get; set; } = 0;
        public int Id_Agenda { get; set; } = 0;
        public string PivaPadre { get; set; } = "";
        public int Ricetta_Cod { get; set; } = 0;
        public int RicettaOperazione_Cod { get; set; } = 0;
        public GeoJSONAgroGisPropTreeNodeKey() { }

        private const string fieldSep = "§";

        public string getKeyString()
        {
            return TipoNodo.ToString() + fieldSep +
                Piva.ToString() + fieldSep +
                Sa_Cod.ToString() + fieldSep +
                Campo_Cod.ToString() + fieldSep +
                Appezza.ToString() + fieldSep +
                Id_Imp.ToString() + fieldSep +
                p_Part_Cod.ToString() + fieldSep +
                p_Provincia_Cod.ToString() + fieldSep +
                p_Comune_Cod.ToString() + fieldSep +
                p_Sezione.ToString() + fieldSep +
                p_Foglio.ToString() + fieldSep +
                p_Numero.ToString() + fieldSep +
                p_Subalterno.ToString() + fieldSep +
                Cod_Fiscale.ToString() + fieldSep +
                Fabbricato_Cod.ToString() + fieldSep +
                Prodotto_Cod.ToString() + fieldSep +
                Data_Lavorazione.ToString() + fieldSep +
                Analisi_Certificato_Cod.ToString() + fieldSep +
                Analisi_Testata_Cod.ToString() + fieldSep +
                Analisi_Dettaglio_Cod.ToString() + fieldSep +
                Analisi_Campione_Cod.ToString() + fieldSep +
                PianoConcimazioneTestata_Cod.ToString() + fieldSep +
                Progetto_Cod.ToString() + fieldSep +
                Programmazione_Cod.ToString() + fieldSep +
                Programmazione_Entita_Cod.ToString() + fieldSep +
                Id_Agenda.ToString() + fieldSep +
                PivaPadre.ToString() + fieldSep +
                Ricetta_Cod.ToString() + fieldSep +
                RicettaOperazione_Cod.ToString();
        }
    }
}
