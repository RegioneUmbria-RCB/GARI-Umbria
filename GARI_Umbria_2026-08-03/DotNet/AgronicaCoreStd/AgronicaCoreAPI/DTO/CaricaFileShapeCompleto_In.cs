using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AgronicaCoreAPI.DTO
{
    public class CaricaFileShapeCompleto_In : CaricaFileShape_In
    {
        public Daticatasto_Importazione datiCatasto { get; set; }
    }
}
