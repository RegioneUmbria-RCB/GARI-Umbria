using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class OperazioniBookmark_In
    {
        public Bookmark bookmark { get; set; }

        public enum_Tipo_Operazione_Bookmark TipoOperazione { get; set; }
    }

    public enum enum_Tipo_Operazione_Bookmark
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
    }
}
