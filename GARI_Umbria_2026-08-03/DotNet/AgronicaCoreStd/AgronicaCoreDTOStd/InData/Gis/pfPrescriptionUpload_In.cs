using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class pfPrescriptionUploadCoreWS_In
    {
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public string DescrizionePiano { get; set; }
        public string psw_SuperUser { get; set; }
        public string rateColumnName { get; set; }
        public string fileName { get; set; }
        public byte[] fileContent { get; set; }

        public pfPrescriptionUploadCoreWS_In() { }

        public pfPrescriptionUploadCoreWS_In(ChiaveAlbero key, string DescrizionePiano, string pwd, string columnName,string filename, byte[] content)
        {
            this.ChiaveAlbero = key;
            this.DescrizionePiano = DescrizionePiano;
            this.psw_SuperUser = pwd;
            this.rateColumnName = columnName;
            this.fileName = filename;
            this.fileContent = content;
        }
    }
}
