using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;
using Microsoft.AspNetCore.Http;

namespace AgronicaCoreAPI.DTO
{
    public class pfPrescriptionUpload_In
    {
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public string DescrizionePiano { get; set; }
        public string psw_SuperUser { get; set; }
        public string rateColumnName { get; set; }
        public IFormFile fileZip { get; set; }
    }
}
