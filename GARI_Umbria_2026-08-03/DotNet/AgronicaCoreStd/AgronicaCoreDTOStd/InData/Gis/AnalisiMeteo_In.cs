using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class AnalisiMeteo_In
    {
        public string Latitudine { get; set; }
        public string Longitudine { get; set; }
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public int Flag_TipoAnalisi { get; set; }
        public bool View_Modal { get; set; }

        public ObjParams_Agenda objParametri_Agenda { get; set; }
        public ObjParams_AuditPUA objParametri_AuditPUA { get; set; }
        public ObjParams_Concimazione2017 objParametri_Concimazione2017 { get; set; }
    }
}
