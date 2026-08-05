using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.avversita
{
    public class GruppoAvversita: AvversitaGruppo
    {
        public GruppoAvversita(int code) : base(code)
        {
            classType = costanti.ClassType.GruppoAvversita;
        }

        public GruppoAvversita() : base()
        {
            classType = costanti.ClassType.GruppoAvversita;
        }
    }
}
