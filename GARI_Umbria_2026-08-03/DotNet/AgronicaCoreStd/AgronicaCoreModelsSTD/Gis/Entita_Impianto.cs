using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class Entita_Impianto
    {
        public string Layer { get; set; }
        public string StandardEntita_layerDiAppartenenza { get; set; }
        public string StandardEntita_layerDiAppartenenza_Des { get; set; }
        public string StandardEntita_layerDiAppartenenza_Icona32 { get; set; }
        public string ID { get; set; }
        public string TipoIcona { get; set; }
        public string Z_Index { get; set; }
        public string Entita_Cod { get; set; }
        public string Veg_Cod { get; set; }
        public string Inserimento { get; set; }
        public string Flag_GPS { get; set; }
        public string Etichetta { get; set; }
        public string Modifica { get; set; }
        public string Cancellazione { get; set; }
        public string Informazioni { get; set; }
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public string Testo { get; set; }
        public string AppIdRate { get; set; }
        public string TipologiaGML { get; set; }
        //DA IMPLEMENTARE CLASSE
        public List<Coordinate> Vertici { get; set; }
        public string InOsservazione { get; set; }
        public string Colore_Primario { get; set; }
        public string Colore_Retinatura { get; set; }
        public string Trasparenza { get; set; }
    }
}
