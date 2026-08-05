using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class CaricaFileCompletoCoreWS_In
    {
        public int codice_sistemaRiferimento { get; set; }
        public byte[] fileZip { get; set; }
        public string fileZipName { get; set; }
        public int layer_cod { get; set; }
        public int tipologiaShape_cod { get; set; }
        public int progressivoGIAS { get; set; }

        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }
        public String Description { get; set; }
        public int PixelSize { get; set; }

        public DatiImpianto_Importazione datiImpianto { get; set; }

        public Daticatasto_Importazione datiCatasto { get; set; }

        public CaricaFileCompletoCoreWS_In() { }

        public CaricaFileCompletoCoreWS_In(int codice_sr, 
            byte[] file, 
            string fileName, 
            int layer, 
            int shape, 
            int progressivo, 
            DateTime? inizio,
            DateTime? fine,
            string descrizione,
            int pixelSize,
            DatiImpianto_Importazione impiantoObj,
            Daticatasto_Importazione catastoObj) 
        {
            this.codice_sistemaRiferimento = codice_sr;
            this.fileZip = file;
            this.fileZipName = fileName;
            this.layer_cod = layer;
            this.tipologiaShape_cod = shape;
            this.progressivoGIAS = progressivo;
            this.datiImpianto = impiantoObj;
            this.datiCatasto = catastoObj;

            if (inizio == null)
                this.Validita_Inizio = new DateTime(1900, 1, 1);
            else
                this.Validita_Inizio = inizio;

            if (fine == null)
                this.Validita_Fine = new DateTime(2100, 12, 31);
            else
                this.Validita_Fine = fine;

            this.Description = descrizione;
            this.PixelSize = pixelSize;
        }
    }

}
