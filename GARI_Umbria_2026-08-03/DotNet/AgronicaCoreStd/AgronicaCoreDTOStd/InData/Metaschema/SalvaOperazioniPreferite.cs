using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class SalvaOperazioniPreferite
    {
        public int[] lista_Lav_Cod { get; set; }
        
        /** Se impostato a `true` esegue un aggiornamento completo delle
         * impostazioni, sostituendo le operazioni selezionate a quelle
         * impostate in precedenza, in caso contrario va semplicemente ad
         * aggiungere le nuove operazioni selezionate*/
        public bool full_update { get; set; } = false;

        //Se Impostato a 1 preferiti ZOO, altrimenti preferiti QDC
        public int? tipoPreferiti { get; set; } = 0;
    }
}
