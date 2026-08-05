using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.VisualBasic;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaTimeSheet : Risorsa
    {
        public DateTime? inizio { get; set; }
        public DateTime? fine { get; set; }
        public double totaleOre { get; set; }

        public RisorsaTimeSheet()
        {

        }

        public double getQuantity()
        {
            if (((this.totaleOre < 0.0001d) && ((this.inizio == null) || (this.fine == null))))
                throw new QuantitaNonCalcolabile();

            if ((this.totaleOre > 0))
                return this.totaleOre;

            DateTime dfine = (DateTime)fine;
            DateTime dInizio = (DateTime)inizio;


            var a = dfine - dInizio;
            return a.TotalSeconds / 60 / 60;
        }
    }
}
