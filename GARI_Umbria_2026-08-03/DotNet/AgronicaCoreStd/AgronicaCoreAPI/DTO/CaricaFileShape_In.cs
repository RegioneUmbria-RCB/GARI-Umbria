using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AgronicaCoreAPI.DTO
{
    public class CaricaFileShape_In 
    {
        public int codice_sistemaRiferimento { get; set; }
        public IFormFile fileZip { get; set; }
        public int layer_cod { get; set; }
        public int tipologiaShape_cod { get; set; }
        public int progressivoGIAS { get; set; }

        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }
        public string Description { get; set; }

        public int PixelSize { get; set; }

        public DatiImpianto_Importazione datiImpianto { get; set; }
    }
}
