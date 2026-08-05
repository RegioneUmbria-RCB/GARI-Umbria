using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class DatiGeneraliLayer
    {
        public int TipologiaLayer_Cod { get; set; }
        public string TipologiaLayer_des { get; set; }
        public string PivaSuperUser { get; set; }
        public string Utente { get; set; }
        public int LayerElementiGrafici_Cod { get; set; }
        public string Colore_Selezionato { get; set; }
        public string Colore_Primario { get; set; }
        public string Colore_Secondario { get; set; }
        public string Icona16 { get; set; }
        public string Icona32 { get; set; }
        public int Varianza { get; set; }
        public double Trasparenza { get; set; }
        public int MostraDescrizioneAssociata { get; set; }
        public int ZIndex { get; set; }
        public int flag_visibile { get; set; }
        public int flag_attivo { get; set; }
        public string TipoNodoAlberoAnagrafe { get; set; }
        public string LayerElementiGrafici_Des { get; set; }
        // Dati anagrafica layer
        public int Flag_Inserimento { get; set; }
        public int Flag_Modifica { get; set; }
        public int Flag_Cancellazione { get; set; }
        public int Flag_Informazioni { get; set; }
        //---
        public DatiGeneraliLayer()
        {
        }
    }
}
