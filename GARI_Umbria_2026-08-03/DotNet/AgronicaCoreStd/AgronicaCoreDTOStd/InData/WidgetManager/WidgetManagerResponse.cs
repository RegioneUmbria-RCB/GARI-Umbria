using System;
using System.Collections.Generic;
using System.Text;

namespace InData.WidgetManager
{
    public class widgetManagerResponse
    {
     
        public string message { get; set; }
        public int statusCode { get; set; }
        public string token { get; set; }
        public widgetManagerResponse_DettaglioEsito dettaglioEsito { get; set; }
        public string errori { get; set; }
    }

    public class widgetManagerResponse_DettaglioEsito
    {
        public string tipoRisposta { get; set; }
        public string titoloWidget { get; set; }
        public List<widgetManagerResponse_DettaglioRisposta> dettaglioRisposta { get; set; }
    }

    public class widgetManagerResponse_DettaglioRisposta
    {
        public string idWidget { get; set; }
        public string urlServizio { get; set; }
        public string titolo { get; set; }
        public string descrizione { get; set; }
        public string descrizioneAggiuntiva { get; set; }
        public string urlImmagine { get; set; }
        public string tipoRender { get; set; }
        public bool nascosto { get; set; }
    }
}
