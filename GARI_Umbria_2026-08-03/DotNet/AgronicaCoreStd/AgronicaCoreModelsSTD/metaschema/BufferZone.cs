using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;


namespace AgronicaCoreModelsSTD.metaschema
{
    public class BufferZone 
    {
        public decimal minimo;
        public decimal massimo;

        public BufferZone()
        {
            minimo = 0;
            massimo = 0;

        }

    }
}

