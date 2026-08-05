using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class MovimentiMagazziniRequest
    {
        public string piva;
        public int sacod;
        public int fabbricato;
        public int categoria;
        public int prodotto;
        public string inizio;
        public string fine;
    }
}
