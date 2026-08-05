using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class VerificaEsistenzaEntitaAnagrafiche
    {
        /// <summary>
        /// Boolean di esistenza impianto
        /// </summary>
        public Boolean EsisteImpianto { get; set; } = false;

        /// <summary>
        /// Boolean di esistenza appezzamento
        /// </summary>
        public Boolean EsisteAppezzamento { get; set; } = false;
    }

}
