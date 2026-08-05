using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Notifiche
{
    public class ObjNotifica<T>
    {
        public int id { get; set; }
        public int stato { get; set; }
        public int nretry { get; set; }
        public T payload { get; set; }
        public int Priorita { get; set; }
    }
}
