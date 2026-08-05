using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class Cultivar_GestioneFiltroUtente_Leggi_IN
    {
        public List<int> Veg_Cod_List { get; set; } = new List<int>();

        public int Cul_Cod { get; set; } = 0;

        public string Cerca_CulDes { get; set; } = string.Empty;

        public bool ControllaLaVisibilitaDelleSpecie { get; set; } = true;
    }
}
