using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Oggetto restituito a fronte di un salvataggio entità grafica
    /// </summary>
    public class Obj_SalvaGrafica
    {
        /// <summary>
        /// Messaggio elaborazione
        /// </summary>
        public string Messaggio { get; set; }
        /// <summary>
        /// Esito salvataggio. Valori ammessi: True = Positivo, False = Negativo (errori).
        /// </summary>
        public bool Esito { get; set; }
        /// <summary>
        /// Elenco entità salvate
        /// </summary>
        public List<Chiave_SalvaGrafica_Out> Lista_ElementiGrafici { get; set; }
    }
}
