using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class LayerTilesDescrizione_In
    {
        public enum_Tipo_Operazione tipoOperazione { get; set; }
        public List<TipologiaLabel> tipologiaLabel { get; set; }
    }

    public enum enum_Tipo_Operazione
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
    }
}
