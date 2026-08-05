using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri di salvataggio layer (colorazioni ed altro)
    /// </summary>
    public class SalvaColoriLayer2_In
    {
        /// <summary>
        /// 1 = Layer, 2 = Tema
        /// </summary>
        /// <example>1</example>
        public int Tipologia { get; set; }
        /// <summary>
        /// Dati salvataggio layer
        /// </summary>
        public List<DatiLayer> ListaDatiLayer { get; set; }
        /// <summary>
        /// Dati salvataggio tema
        /// </summary>
        public List<DatiTema> ListaDatiTema { get; set; }
    }

    /// <summary>
    /// Dati salvataggio layer
    /// </summary>
    public class DatiLayer
    {
        /// <summary>
        /// ID layer
        /// </summary>
        /// <example>1</example>
        public string ID { get; set; }
        
        /// <summary>
        /// Colore primario in RGB
        /// </summary>
        /// <example>FFFF00</example>
        public string Colore_Primario { get; set; }

        /// <summary>
        /// Colore secondario in RGB
        /// </summary>
        /// <example>FFFF00</example>
        public string Colore_Secondario { get; set; }

        /// <summary>
        /// Numero di intervalli in cui viene divisa la scala dei colori, calcolata a partire
        /// dal colore primario e secondario (se tipologia = 1 indicare 1 come valore)
        /// </summary>
        /// <example>5</example>
        public string Varianza { get; set; }

        /// <summary>
        /// Trasparenza, indicare un valore compreso fra 0 e 1, a "step" di 0,1 : 0 = trasparente, 1 = pieno
        /// </summary>
        /// <example>0.6</example>
        public string Trasparenza { get; set; }

        /// <summary>
        /// Indica quale layer è in primo piano in base al valore assegnato (valore più alto = in primo piano)
        /// </summary>
        /// <example>10</example>
        public string ZIndex { get; set; }

        /// <summary>
        /// Indica se il layer è visibile nell'elenco (e di conseguenza vengono visualizzati elementi grafici) 
        /// oppure si trova in uno stato nascosto. (1 = visibile, 0 = nascosto)
        /// </summary>
        /// <example>1</example>
        public int Flag_Visibile { get; set; }

        /// <summary>
        /// Mostra la descrizione associata al layer (1 = mostra, 2 = nascondi)
        /// </summary>
        /// <example>1</example>
        public string MostraDescrizioneAssociata { get; set; }

        /// <summary>
        /// Indica la tipologia di appartenenza del layer
        /// </summary>
        /// <example>1</example>
        public string TipologiaLayer_Cod { get; set; }
    }

    /// <summary>
    /// Dati salvataggio tema
    /// </summary>
    public class DatiTema
    {
        /// <summary>
        /// ID tema
        /// </summary>
        /// <example>1</example>
        public string ID { get; set; }

        /// <summary>
        /// Colore primario in RGB
        /// </summary>
        /// <example>FFFF00</example>
        public string Colore_Primario { get; set; }

        /// <summary>
        /// Colore secondario in RGB
        /// </summary>
        /// <example>FFFF00</example>
        public string Colore_Secondario { get; set; }

        /// <summary>
        /// Numero di intervalli in cui viene divisa la scala dei colori, calcolata a partire dal colore primario e secondario (se tipologia = 1 indicare 1 come valore)
        /// </summary>
        /// <example>5</example>
        public string Varianza { get; set; }

        /// <summary>
        /// Indica la tipologia di appartenenza del tema
        /// </summary>
        /// <example>1</example>
        public string TipologiaLayer_Cod { get; set; }

        /// <summary>
        /// Indica il codice layer a cui appartiene il tema
        /// </summary>
        /// <example>50</example>
        public string LayerElementiGrafici_Cod { get; set; }

        /// <summary>
        /// La lista dei LayerTilesDescrizione da aggiungere al tema
        /// </summary>
        public List<TipologiaLabel> layerDescrizione_aggiungi { get; set; }

        /// <summary>
        /// La lista dei LayerTilesDescrizione da modificare per il tema
        /// </summary>
        public List<TipologiaLabel> layerDescrizione_modifica { get; set; }

        /// <summary>
        /// La lista dei LayerTilesDescrizione da eliminare per il tema
        /// </summary>
        public List<TipologiaLabel> layerDescrizione_elimina { get; set; }
    }

}
