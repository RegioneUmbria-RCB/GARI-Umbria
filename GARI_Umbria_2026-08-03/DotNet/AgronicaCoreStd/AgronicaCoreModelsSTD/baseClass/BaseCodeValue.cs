using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.baseClass
{
    public class BaseCodeValue<C, V>
    {
        public C cod { get; set; }
        public V value { get; set; }
        public string des { get; set; }
        public BaseCodeValue()
        {

        }
        public BaseCodeValue(C cod, V value, string des = "")
        {
            this.cod = cod;
            this.value = value;
            this.des = des;
        }
    }
}
